using Sharp.Xmpp.Client;

namespace DexNetwork.Server
{
    class Matrix : IXMPPClient
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


            _client = new XmppClient(hostname, username, pwd, tls: true);
            
                // Setup any event handlers.
                // ...


                // Setup any event handlers before connecting.
                _client.Message += OnServerMessage;

                _client.StatusChanged += Client_StatusChanged;

                _client.Hostname = hostname;
                _client.Connect(null);
            }

        private void FireEventOnMessageReceived(XMPPEventArgs e)
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

            FireEventOnMessageReceived(new XMPPEventArgs(e.Message.Body, "message" ));
        }

        public event XMPPEvent OnMessageReceived;

        public void SendMessage(string message)
        {
            _client.SendMessage("backfire@xabber.org", message);
        }
    }
}
