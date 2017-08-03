namespace DexNetwork.DexInterpreter.Commands
{
    class DexDirectInstructionCommand : DexInstructionCommandBase
    {
        public const string CmdName = "!";

        public DexDirectInstructionCommand(Verbosity verbosity, IDexPromise promise) : base(CmdName, verbosity, promise)
        {
            CommandHelpString = "!<any text>";
            MandatoryParamCount = 0;
        }

        protected override string GetXmppInputForInstruction(string input)
        {
            return input.Substring(1);
        }

        protected override CommandResult ProcessXmppMesssage(string message)
        {
            return CreateOutput(new TextOutput(Verbosity, message), CommadState.Finished);
        }
    }
}