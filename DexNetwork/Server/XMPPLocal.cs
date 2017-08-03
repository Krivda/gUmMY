using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DexNetwork.DexInterpreter.Response;
using DexNetwork.Structure;

namespace DexNetwork.Server
{
    class XmppLocal : IXMPPClient
    {
        private string _user;
        private string _domain;
        private string _target;
        private string _admSystem;
        private int _proxyLevel;
        private string _visibleAs;
        private SoftwareLib _softwareLib;
        private Network _network;
        public Dictionary<string, Network> AvailableNetworks { get; } = new Dictionary<string, Network>();


        public void Login(string user, string domain, string password)
        {
            _user = user;
            _domain = domain;
            _target = "none";
            _admSystem = "none";
            _proxyLevel = 7;
            _visibleAs = "kenguru2157@sydney";

            string softLib = Path.GetFullPath(@"Server/Data/lib.xml");

            _softwareLib = Serializer.Deserialize<SoftwareLib>(softLib);
            _softwareLib.Init(@"Server/Data/lib.xml");

            //EmulateResponse($"Logged in to LOCAL XMPP Emulation. Software loaded from {softLib}");
        }
        
        public event XMPPEvent OnMessageReceived;

        private void FireEventOnMessageRecieved(XMPPEventArgs e)
        {
            OnMessageReceived?.Invoke(this, e);
        }

        public void SendMessage(string message)
        {
            if (message.StartsWith("target"))
            {
                if (message.Split(' ').Length > 1)
                {
                    _target = message.Split(' ')[1];

                    string netPath = $"Server//Data//{_target}.xml";

                    _network = Serializer.DeserializeNet(netPath);

                }

                EmulateResponse("ok");
                //404 target system:2739100 not found
            }
            else if (message.StartsWith("status"))
            {
                EmulateResponse(StatusInstruction.Assemble($"{_user}@{_domain}", 
                    string.IsNullOrEmpty(_target)?"none":_target,
                    _admSystem,
                    _proxyLevel,
                    _visibleAs));
            }
            else if (message.StartsWith("info"))
            {
                string codeStr = message.Replace("info #", "");
                long softCode;

                string incorrectProgMessage = @"--------------------
incorrect program #{0}
END----------------";

                if (!long.TryParse(codeStr, out softCode))
                {
                    EmulateResponse(string.Format(incorrectProgMessage, codeStr));
                }
                else
                {
                    Structure.Software libSoft;
                    if (!_softwareLib.All.TryGetValue(softCode, out libSoft))
                    {
                        EmulateResponse(string.Format(incorrectProgMessage, codeStr));
                        return;
                    }
                    //soft found 
                    EmulateResponse(InfoInstruction.Assemble(libSoft));
                }
            }
            else if (message.StartsWith("look"))
            {
                //todo: check errors

                string nodeName = message.Replace("look ", "");

                Node node;
                if (_network == null)
                {
                    EmulateResponse("network not found");    
                }

                if (!_network.Nodes.TryGetValue(nodeName, out node))
                {
                    EmulateResponse($@"--------------------
{_network.Name} / {nodeName}: not available

END----------------");
                }
                else
                {
                    string response = LookInstruction.AssembleAccessible(_network.Name, node);
                    EmulateResponse(response);
                }

            }
            else if (message.StartsWith("#"))
            {
                EmulateResponse("Not implemented");
            }
            else
            {
                EmulateResponse("command no found");
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
