namespace DexNetwork.Server
{
    public interface IXMPPClient
    {
        void Login(string user, string domain, string password);

        event XMPPEvent OnMessageRecieved;

        void SendMessage(string message);
    }
}