namespace SRMatrixNetwork.Server
{
    public interface IXmppClient
    {
        void Login(string user, string password, string realm);

        event XmppEvent OnMessageReceived;

        void SendMessage(string message);
    }
}