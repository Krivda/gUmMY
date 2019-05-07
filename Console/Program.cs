using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using jabber.client;
using jabber.protocol.client;
using Sharp.Xmpp.Client;


namespace Console
{
    class Program
    {

        static string hostname = "xmpp.co";
        static string username = "calvin"; //gr8b@cyberspace
        static string password = "fraudfraudfraud"; //639924

        // we will wait on this event until we're done sending
        static ManualResetEvent done = new ManualResetEvent(false);

        // if true, output protocol trace to stdout
        const bool VERBOSE = true;
        const string TARGET = "backfire@xmpp.co";


        static void Main(string[] args)
        {

            /*Адрес сервера: cyberspace.alice.digital
            Логин: calvin276 @cyberspace
            Пароль: w8119*/
            /*
            string hostname = "cyberspace.alice.digital";
            string username = "gr8b"; //gr8b@cyberspace
            string password = "639924"; //639924
            */
            //calvin@xmpp.co

            SharpVar();
            //Jnet();

        }




        private static void Client_StatusChanged(object sender, Sharp.Xmpp.Im.StatusEventArgs e)
        {
            System.Console.WriteLine($"<{e.Jid.Domain}: {e.Status}");
        }

        private static void OnNewMessage(object sender, Sharp.Xmpp.Im.MessageEventArgs e)
        {
            System.Console.WriteLine($"{e.Jid.Domain}: {e.Message.Body}");
        }

        private static void SharpVar()
        {
            using (XmppClient client = new XmppClient(hostname, username, "xmpp.co", password, tls: true))
            {
                // Setup any event handlers.
                // ...


                // Setup any event handlers before connecting.
                client.Message += OnNewMessage;

                client.StatusChanged += Client_StatusChanged;

                client.Hostname = hostname;
                client.Connect(); //xmpp.co
                //client.Authenticate(username, password);
                System.Console.WriteLine("Connected as " + client.Jid + Environment.NewLine);
                System.Console.WriteLine(" Type 'send <JID> <Message>' to send a chat message, or 'quit' to exit.");
                System.Console.WriteLine(" Example: send user@domain.com Hello!");
                System.Console.WriteLine();

                while (true)
                {
                    System.Console.Write("> ");
                    string s = System.Console.ReadLine();
                    if (!string.Equals("quit", s))
                    {
                        /*Match m = Regex.Match(s, @"^send\s(?<jid>[^\s]+)\s(?<message>.+)$");
                        if (!m.Success)
                            continue;
                        string recipient = m.Groups["jid"].Value, message = m.Groups["message"].Value;
                        // Send the chat-message.*/
                        client.SendMessage(TARGET, s);
                    }

                    if (s == "quit")
                        return;
                }
            }

        }

        private static void Jnet()
        {
            using (var j = new JabberClient())
            {

                // what user/pass to log in as
                j.User = username;
                j.Server = hostname; // use gmail.com for GoogleTalk
                j.Password = password;

                // don't do extra stuff, please.
                j.AutoPresence = false;
                j.AutoRoster = false;
                j.AutoReconnect = -1;

                // listen for errors.  Always do this!
                j.OnError += j_OnError;

                // what to do when login completes
                j.OnAuthenticate += j_OnAuthenticate;

                // listen for XMPP wire protocol
                if (VERBOSE)
                {
                    j.OnReadText += j_OnReadText;
                    j.OnWriteText += j_OnWriteText;
                    j.OnMessage += j_OnMessage;
                    j.OnProtocol += J_OnProtocol;
                }

                j.Resource = "myComp";
                j.UseAnonymous = false;

                // Set everything in motion
                j.Connect();

                while (true)
                {
                    System.Console.Write("> ");
                    string s = System.Console.ReadLine();
                    if (!string.Equals("quit", s))
                    {
                        /*Match m = Regex.Match(s, @"^send\s(?<jid>[^\s]+)\s(?<message>.+)$");
                        if (!m.Success)
                            continue;
                        string recipient = m.Groups["jid"].Value, message = m.Groups["message"].Value;
                        // Send the chat-message.*/

                        j.Message(TARGET, s);

                    }

                    if (s == "quit")
                        return;
                }
            }

        }

        private static void J_OnProtocol(object sender, System.Xml.XmlElement rp)
        {
            System.Console.WriteLine("PROT" + rp.InnerXml);
        }

        private static void j_OnMessage(object sender, Message message)
        {
            if (message.Body == " ") return; // ignore keep-alive spaces
            System.Console.WriteLine("RECV: " + message.Body);
        }


        static void j_OnWriteText(object sender, string txt)
        {
            if (txt == " ") return; // ignore keep-alive spaces
            System.Console.WriteLine("SEND: " + txt);
        }

        static void j_OnReadText(object sender, string txt)
        {
            if (txt == " ") return; // ignore keep-alive spaces
            System.Console.WriteLine("RECV: " + txt);
        }

        static void j_OnAuthenticate(object sender)
        {
            // Sender is always the JabberClient.
            JabberClient j = (JabberClient)sender;
            j.Message(TARGET, "test");

        }

        static void j_OnError(object sender, Exception ex)
        {
            // There was an error!
            System.Console.WriteLine("Error: " + ex.ToString());

            // Shut down.
            done.Set();
        }


    }
}