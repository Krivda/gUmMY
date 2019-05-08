using System.Collections.Generic;
using StatusInstruction = SRMatrixNetwork.Commands.response.StatusInstruction;

namespace SRMatrixNetwork.Commands
{


    public class CommandResult
    {
        public List<TextOutput> Output { get; set; } = new List<TextOutput>();
        public TextOutput Error { get; set; }
        public string XmppCommand { get; set; }

        public CommandState State { get; set; }


        public Dictionary<string, string> Prompt { get; set; }

        public bool BlockInput { get; set; }
        public bool ClearScreen { get; set; }

        /**
         * Indicates, that Xmpp connectivity established as the result of command execution
         */
        public bool XmppConnected { get; set; }
        public bool TargetSet { get; set; }

        public StatusInstruction UpdatedNetStatus { get; set; }

        public CommandResult()
        {
            State = CommandState.Finished;
        }
    }

}