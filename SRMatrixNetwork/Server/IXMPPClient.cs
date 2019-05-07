namespace SRMatrixNetwork.Server
{
    public interface IXmppClient
    {
        void Login(string user, string domain, string password);

        event XmppEvent OnMessageReceived;

        void SendMessage(string message);
    }
}