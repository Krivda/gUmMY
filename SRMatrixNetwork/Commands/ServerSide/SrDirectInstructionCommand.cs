namespace SRMatrixNetwork.Commands.ServerSide
{
    class SRDirectInstructionCommand : SRInstructionCommandBase
    {
        public const string CmdName = "";

        public SRDirectInstructionCommand(Commands.Verbosity verbosity, IDexPromise promise) : base(CmdName, verbosity, promise)
        {
            CommandHelpString = "<any text>";
            MandatoryParamCount = 0;
        }

        protected override string GetXmppInputForInstruction(string input)
        {
            State = CommandState.Finished;
            return input;
        }

        protected override CommandResult ProcessXmppMessage(string message)
        {
            return CreateOutput(new TextOutput(Verbosity, message), Commands.CommandState.Finished);
        }
    }
}