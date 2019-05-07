using Sharp.Xmpp.Client;
using SRMatrixNetwork.Server;
    
namespace SRMatrixNetwork
{
    class Matrix : IXmppClient
    {
        private XmppClient _client;

        public void Login(string user, string domain, string password)
        {
            /*Адрес сервера: cyberspace.alice.digital
            Логин: calvin276 @cyberspace
            Пароль: w8119*/

            string hostname = "xmpp.co";
            //string hostname = "xabber.org";
            string username = user;//"calvin";
            string pwd = password;//"fraudfraudfraud";


            _client = new XmppClient(hostname, username, hostname, pwd, tls: true);
            
                // Setup any event handlers.
                // ...


                // Setup any event handlers before connecting.
                _client.Message += OnServerMessage;

                _client.StatusChanged += Client_StatusChanged;

                _client.Hostname = hostname;
                _client.Connect();
            }

        event XmppEvent IXmppClient.OnMessageReceived
        {
            add => throw new System.NotImplementedException();
            remove => throw new System.NotImplementedException();
        }

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

        public event XmppEvent OnMessageReceived;

        public void SendMessage(string message)
        {
            _client.SendMessage("backfire@xabber.org", message);
        }
    }
}
