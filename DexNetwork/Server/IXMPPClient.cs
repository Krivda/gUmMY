namespace DexNetwork.Server
{
    public interface IXMPPClient
    {
        void Login(string user, string domain, string password);

        event XMPPEvent OnMessageReceived;

        void SendMessage(string message);
    }
}