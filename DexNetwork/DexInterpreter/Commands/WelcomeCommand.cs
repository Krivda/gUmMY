namespace DexNetwork.DexInterpreter.Commands
{
    internal class WelcomeCommand : TextOutBase
    {
        public const string CmdName = "wellcome";

        public WelcomeCommand(string text, Verbosity verbosity, IDexPromise promise) : base(promise)
        {
            CommandName = CmdName;
            CommandHelpString = $"{CmdName}";
            _text = text;
            _verbosity = verbosity;
            State = CommandState.NotStarted;
        }



    }
}