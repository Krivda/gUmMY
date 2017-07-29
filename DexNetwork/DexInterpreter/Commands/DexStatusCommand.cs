using DexNetwork.DexInterpreter.Response;

namespace DexNetwork.DexInterpreter.Commands
{
    public class DexStatusCommand : CommandBase
    {
        private string _waitfor;
        private readonly Verbosity _verbosity;

        public const string CmdName = "status";


        public DexStatusCommand(Verbosity verbosity, IDexPromise promise) : base(promise)
        {
            _verbosity = verbosity;
            CommandHelpString = $"{CmdName}";
            Status = CommadStatus.NotStarted;
        }


        public override CommandResult OnCommandInput(string input)
        {
            CommandResult result = EnsureStatus(CommadStatus.NotStarted);
            if (result != null)
                return result;

            _waitfor = CmdName;
            string xmppCommand = $"{CmdName}";

            Status = CommadStatus.AwaitXMPP;

            result = CreateOutput(new TextOutput(_verbosity, $">> {xmppCommand}"), CommadStatus.AwaitXMPP);
            result.XMPPCommand = xmppCommand;
            result.Status = CommadStatus.AwaitXMPP;

            return result;
        }

        public override CommandResult OnXMPPInput(string message)
        {
            CommandResult result = EnsureStatus(CommadStatus.AwaitXMPP);
            if (result != null)
                return result;

            if ( _waitfor.Equals(CmdName))
            {
                _waitfor = "";
                var commadResult = StatusInstruction.Parse(message);
                if (commadResult == null)
                    return CreateError($"Status command resulted in error. Couldn't parse result\n {message}");


                result = CreateOutput(new TextOutput(_verbosity, message), CommadStatus.Finished);
                result.UpdatedNetStatus = commadResult;
                return result;

            }
            _waitfor = "";
            return CreateError($"Invalid command wait state. Expected {CmdName} output, got {message}.");
        }
    }
}