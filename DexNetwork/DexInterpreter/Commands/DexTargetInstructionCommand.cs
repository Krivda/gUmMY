using System.IO;
using DexNetwork.Structure;

namespace DexNetwork.DexInterpreter.Commands
{
    class DexTargetInstructionCommand : DexInstructionCommandBase
    {
        private string _network;
        public const string CmdName = "$target";
        private const string INSTRUCTION_NAME = "target";
        private const string RESPONSE_OK = "ok";



        public DexTargetInstructionCommand(Verbosity verbosity, IDexPromise promise) : base(CmdName, verbosity, promise)
        {
            CommandHelpString = "target <networkName>  //i.e target Blackmirror11";
            MandatoryParamCount = 1;
            OptionalParamCount = 0;
        }

        protected override string GetXmppInputForInstruction(string input)
        {
            return $"{INSTRUCTION_NAME} {_network}";
        }

        protected override CommandResult ParseArguments(string command)
        {
            CommandResult result = base.ParseArguments(command);
            if (result != null)
                return result;

            if (Parameters.Count >0 )
            {
                _network = Parameters[0];
            }

            return null;
        }

        protected override CommandResult ProcessXmppMesssage(string message)
        {
            CommandResult result;
            if (message.ToLower().Equals(RESPONSE_OK.ToLower()))
            {
                result = CreateOutput(new TextOutput(Verbosity, $"{message}"), CommandState.Finished);
                result.BlockInput = false;

                string networkFileName = $"Networks//{_network}.xml";

                Network net;
                if (File.Exists(networkFileName))
                {
                    net = Serializer.DeserializeNet(networkFileName);
                    net.Init(networkFileName);
                }
                else
                {
                    net =new Network();
                     net.Init(networkFileName);
                }

                Promise.Network = net;
                net.Dump();

            }
            else
            {
                result = CreateError($"Unknown response while awaiting '{RESPONSE_OK}': {message}");
            }

            return result;
        }
    }
}