using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DexNetwork.DexInterpreter.Commands;
using DexNetwork.DexInterpreter.Response;
using DexNetwork.Structure;

namespace DexNetwork.Server
{
    class XMPPLocal : IXMPPClient
    {
        private string _user;
        private string _domain;
        private string _target;
        private string _admSystem;
        private int _proxyLevel;
        private string _visibleAs;
        private SoftwareLib _software;
        public Dictionary<string, Network> AvailableNetworks { get; } = new Dictionary<string, Network>();


        public void Login(string user, string domain, string password)
        {
            _user = user;
            _domain = domain;
            _target = "none";
            _admSystem = "none";
            _proxyLevel = 6;
            _visibleAs = "kenguru2157@sydney";

            string softLib = Path.GetFullPath(@"Software/lib.xml");

            _software = Serializer.DeserializeSoft(softLib);

            //EmulateResponse($"Logged in to LOCAL XMPP Emulation. Software loaded from {softLib}");
        }
        
        public event XMPPEvent OnMessageRecieved;

        private void FireEventOnMessageRecieved(XMPPEventArgs e)
        {
            OnMessageRecieved?.Invoke(this, e);
        }

        public void SendMessage(string message)
        {
            if (message.StartsWith("target"))
            {
                EmulateResponse("ok");
                //404 target system:2739100 not found
            }
            else if (message.StartsWith("status"))
            {
                EmulateResponse(StatusInstruction.Assemble($"{_user}@{_domain}", _target, _admSystem,_proxyLevel, _visibleAs));
            }
        }

        public void AddNetwork(Network net)
        {
            AvailableNetworks.Add(net.Name, net);
        }

        private void EmulateResponse(string response)
        {
            Task.Run(async delegate
            {
                await Task.Delay(new Random().Next(500, 1500));
                FireEventOnMessageRecieved(new XMPPEventArgs(response, "Message"));
            });
        }

    }

}
