using SRMatrixNetwork.Commands.response;

namespace SRMatrixNetwork.Commands.ServerSide
{
    public class SRStatusInstructionCommand : SRInstructionCommandBase
    {
        public const string CmdName = "status";


        public SRStatusInstructionCommand(Commands.Verbosity verbosity, IDexPromise promise) : base(CmdName, verbosity, promise)
        {
            CommandHelpString = $"{CmdName}";
            MandatoryParamCount = 0;
            OptionalParamCount = 0;
        }


        protected override CommandResult ProcessXmppMessage(string message)
        {
            var commandResult = StatusInstruction.Parse(message);
            if (commandResult == null)
                return CreateError($"{CmdName} command resulted in error. Couldn't parse result\n {message}");

            if (! string.IsNullOrEmpty(commandResult.Error))
                return CreateError($"{CmdName} command resulted in error. {commandResult.Error}");


            var result = CreateOutput(new TextOutput(Verbosity, message), CommandState.Finished);
            result.UpdatedNetStatus = commandResult;
            return result;
        }
    }
}