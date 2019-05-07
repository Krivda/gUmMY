using System.Collections.Generic;
using DexNetwork.DexInterpreter.Response;
using StatusInstruction = SRMatrixNetwork.Commands.response.StatusInstruction;

namespace SRMatrixNetwork.Commands
{


    public class CommandResult
    {
        public List<TextOutput> Output { get; set; } = new List<TextOutput>();
        public TextOutput Error { get; set; }
        public string XMPPCommand { get; set; }

        public CommandState State { get; set; }


        public Dictionary<string, string> Prompt { get; set; }

        public bool BlockInput { get; set; }
        public bool ClearScreen { get; set; }

        public bool XMPPConnected { get; set; }
        public bool TargetSet { get; set; }

        public StatusInstruction UpdatedNetStatus { get; set; }

        public CommandResult()
        {
            State = CommandState.Finished;
        }
    }

}