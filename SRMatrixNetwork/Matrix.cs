using System;
using NLog;
using Sharp.Xmpp.Client;
using SRMatrixNetwork.Server;
    
namespace SRMatrixNetwork
{
    class Matrix : IXmppClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private XmppClient _client;

        public void Login(string user, string password, string realm)
        {
            /*Адрес сервера: cyberspace.alice.digital
            Логин: calvin276 @cyberspace
            Пароль: w8119*/

            string hostname = "xmpp.co";
            //string hostname = "xabber.org";
            string username = user;//"calvin";
            string pwd = password;//"fraudfraudfraud";

            if (username.Contains("@"))
            {
                //override hostname with jid data
                string[] split = username.Split('@');

                username = split[0];
                hostname = split[1];
            }


            _client = new XmppClient(hostname, username, hostname, pwd, tls: true);

            // Setup any event handlers.
            // ...

            // Setup any event handlers before connecting.
            _client.Message += OnServerMessage;
            _client.StatusChanged += Client_StatusChanged;
            _client.Hostname = hostname;
            
            try { 
                _client.Connect();
                Logger.Info($"Established connection for {username}@{hostname}.");
            }
            catch (System.Security.Authentication.AuthenticationException)
            {
                throw new Exception($"Account {username}@{hostname} not authorized by matrix server. Bad password?");
            }
        }

        public event XmppEvent OnMessageReceived;

        private void FireEventOnMessageReceived(XmppEventArgs e)
        {
            OnMessageReceived?.Invoke(this, e);
        }

        private static void Client_StatusChanged(object sender, Sharp.Xmpp.Im.StatusEventArgs e)
        {
            System.Console.WriteLine($"<{e.Jid.Domain}: {e.Status}");
        }

        private void OnServerMessage(object sender, Sharp.Xmpp.Im.MessageEventArgs e)
        {
            /*XmlDocument doc = new XmlDocument();
            doc.LoadXml(e.Message.Body);*/

            FireEventOnMessageReceived(new XmppEventArgs(e.Message.Body, "message" ));
        }

        public void SendMessage(string message)
        {
            _client.SendMessage("backfire@xabber.org", message);
        }
    }
}
