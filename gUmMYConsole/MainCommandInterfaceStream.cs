using System.Windows.Forms;
using ConsoleStream;
using DexNetwork;
using DexNetwork.Structure;

namespace gUmMYConsole
{
    class MainCommandInterfaceStream : ConsoleStreamBase
    {
        private readonly Form _form;
        public Network Network { get; private set; }
        public NetTextMap NetworkTextMap { get; private set; }


        public MainCommandInterfaceStream(Form form)
        {
            _form = form;
        }

        public void FeedOutput(string content)
        {
            FireProcessOutputEvent(content);
        }

        public void StartFeed()
        {
            IsProcessRunning = true;
        }

        public override void ExecuteCommad(string command)
        {
            FireProcessCommadExecute(command);

            string[] split = command.Split(' ');

            if (split[0].Equals("login"))
            {
                //should have name arg and pwd arg
                if (split.Length < 2)
                {
                    FireProcessErrorEvent("ERROR, login syntax: login <acc> <pwd>");
                }
                else
                {
                    FireProcessOutputEvent($"Logged in as {split[1]}");
                    FirePromptChangedEvent(split[1]);
                }
            }
            else if (split[0].Equals("target"))
            {
                if (split.Length < 2)
                {
                    FireProcessErrorEvent("ERROR, login syntax: target <network>");
                }
                else
                {
                    string netName = split[1];


                    string fileName = $"Networks\\{netName}.xml";

                    Network = Serializer.DeserializeNet(fileName);
                    NetworkTextMap = new NetTextMap(Network);

                    FireProcessOutputEvent($"Uplinking network {split[1]}.");
                }
            }
            else if (split[0].Equals("shownet"))
            {
                if (NetworkTextMap == null)
                {
                    FireProcessErrorEvent("CLIENT ERROR: uplink is not set. Use 'target/administrate' to set up the uplink");


                    string netName = "testnet";
                    if (split.Length > 1)
                        netName = split[1];


                    string fileName = $"Networks\\{netName}.xml";

                    Network = Serializer.DeserializeNet(fileName);
                    NetworkTextMap = new NetTextMap(Network);

                    FireProcessOutputEvent($"Uplinking network {netName}.");
                }
                //else todo: remove, replace with else
                {
                    string textMap = NetworkTextMap.GetTextView("", 20);

                    FireProcessOutputEvent(textMap);
                }
            }
        }
    }


}