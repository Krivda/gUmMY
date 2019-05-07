using System;

namespace SRMatrixNetwork.Server
{
    public delegate void XmppEvent(IXmppClient sender, XmppEventArgs args);

    public class XmppEventArgs
    {
        public String Message { get; }
        public String XmppEvent { get; }

        public XmppEventArgs(string message, string xmppEvent)
        {
            Message = message;
            XmppEvent = xmppEvent;
        }
    }
}