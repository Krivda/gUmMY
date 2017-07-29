using System;
using System.Text.RegularExpressions;
using Sharp.Xmpp.Client;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {

            /*Адрес сервера: cyberspace.alice.digital
            Логин: calvin276 @cyberspace
            Пароль: w8119*/

            string hostname = "cyberspace.alice.digital";
            string username = "calvin276";
            string password = "w8119";

            using (XmppClient client = new XmppClient(hostname, username, "cyberspace", password, tls:false))
            {
                // Setup any event handlers.
                // ...

                
                // Setup any event handlers before connecting.
                client.Message += OnNewMessage;

                client.StatusChanged += Client_StatusChanged;

                client.Hostname = hostname;
                client.Connect("cyberspace");
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
                        client.SendMessage("darknet@cyberspace", "status");
                    }
                    if (s == "quit")
                        return;
                }
            }
        }

        private static void Client_StatusChanged(object sender, Sharp.Xmpp.Im.StatusEventArgs e)
        {
            System.Console.WriteLine($"<{e.Jid.Domain}: {e.Status}");
        }

        private static void OnNewMessage(object sender, Sharp.Xmpp.Im.MessageEventArgs e)
        {
            System.Console.WriteLine($"{e.Jid.Domain}: {e.Message}");
        }
    }
}