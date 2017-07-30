namespace DexNetwork.DexInterpreter.Commands
{
    internal class WellcomeCommand : TextOutBase
    {
        public const string CmdName = "wellcome";

        public WellcomeCommand(string text, Verbosity verbosity, IDexPromise promise) : base(promise)
        {
            CommandName = CmdName;
            CommandHelpString = $"{CmdName}";
            _text = text;
            _verbosity = verbosity;
            State = CommadState.NotStarted;
        }



    }
}