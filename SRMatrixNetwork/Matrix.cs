using System;
using NLog;
using Sharp.Xmpp.Client;
using SRMatrixNetwork.Server;
    
namespace SRMatrixNetwork
{
    class Matrix : IXmppClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        //public const string MATRIX_JID = "backfire@xabber.org";
        //public const string MATRIX_JID = "matrix@parfenow.ru";
        public const string MATRIX_JID = "matrix@matrix.evarun.ru";

        public const string MATRIX_RC_JID = "rc-matrix@matrix.evarun.ru";
        public const string MATRIX_DEV_JID = "dev-matrix@matrix.evarun.ru";

        //private const string DEFAULT_HOST = "xmpp.co";
        private const string DEFAULT_HOST = "matrix.evarun.ru";
        private string _destination = MATRIX_JID;
        
        private XmppClient _client;

        public void Login(string user, string password, string realm)
        {

            string hostname = DEFAULT_HOST;

            string username = user;
            string pwd = password;

            if ("rc".Equals(realm))
            {
                _destination = MATRIX_RC_JID;
            }
            else if ("dev".Equals(realm))
            {
                _destination = MATRIX_DEV_JID;
            }
            

            if (username.Contains("@"))
            {
                //override hostname with jid data
                string[] split = username.Split('@');

                username = split[0];
                hostname = split[1];
            }

            _client = new XmppClient(hostname, username, pwd);

            // Setup any event handlers.
            // ...

            // Setup any event handlers before connecting.
            _client.Message += OnServerMessage;
            _client.StatusChanged += Client_StatusChanged;
            _client.Hostname = hostname;

            try
            {
                _client.Connect();
                Logger.Info($"Established connection for {username}@{hostname}.");
            }
            catch (System.Security.Authentication.AuthenticationException ex)
            {
                string message = $"Account {username}@{hostname} is not authorized by matrix server. Bad password?";
                Logger.Error(message);
                throw new Exception(message, ex);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                //rethrow
                throw;
            }
        }

        public event XmppEvent OnMessageReceived;

        private void FireEventOnMessageReceived(XmppEventArgs e)
        {
            OnMessageReceived?.Invoke(this, e);
        }

        private static void Client_StatusChanged(object sender, Sharp.Xmpp.Im.StatusEventArgs e)
        {
            Console.WriteLine($"<{e.Jid.Domain}: {e.Status}");
        }

        private void OnServerMessage(object sender, Sharp.Xmpp.Im.MessageEventArgs e)
        {
            /*XmlDocument doc = new XmlDocument();
            doc.LoadXml(e.Message.Body);*/

            FireEventOnMessageReceived(new XmppEventArgs(e.Message.Body, "message" ));
        }

        public void SendMessage(string message)
        {
            try
            {
                Logger.Info($"\n\nsent: {message}\n");
                _client.SendMessage(_destination, message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }
    }
}
