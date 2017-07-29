using System;

namespace DexNetwork.Server
{
    public delegate void XMPPEvent(IXMPPClient sender, XMPPEventArgs args);

    public class XMPPEventArgs
    {
        public String Message { get; }
        public String XMPPEvent { get; }

        public XMPPEventArgs(string message, string xmppEvent)
        {
            Message = message;
            XMPPEvent = xmppEvent;
        }
    }
}