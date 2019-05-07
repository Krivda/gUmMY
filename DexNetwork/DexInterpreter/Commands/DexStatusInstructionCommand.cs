using DexNetwork.DexInterpreter.Response;

namespace DexNetwork.DexInterpreter.Commands
{
    public class DexStatusInstructionCommand : DexInstructionCommandBase
    {
        public const string CmdName = "status";


        public DexStatusInstructionCommand(Verbosity verbosity, IDexPromise promise) : base(CmdName, verbosity, promise)
        {
            CommandHelpString = $"{DexStatusInstructionCommand.CmdName}";
            MandatoryParamCount = 0;
            OptionalParamCount = 0;
        }


        protected override CommandResult ProcessXmppMesssage(string message)
        {
            CommandResult result;
            var commadResult = StatusInstruction.Parse(message);
            if (commadResult == null)
                return CreateError($"{CmdName} command resulted in error. Couldn't parse result\n {message}");

            if (! string.IsNullOrEmpty(commadResult.Error))
                return CreateError($"{CmdName} command resulted in error. {commadResult.Error}");


            result = CreateOutput(new TextOutput(Verbosity, message), CommandState.Finished);
            result.UpdatedNetStatus = commadResult;
            return result;
        
        }
    }
}