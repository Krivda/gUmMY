namespace DexNetwork.DexInterpreter.Commands
{
    abstract class TextOutBase : CommandBase
    {
        protected Verbosity _verbosity = Verbosity.Critical;
        protected string _text = "";

        protected TextOutBase(IDexPromise promise) : base(promise)
        {

        }

        public override CommandResult OnCommandInput(string input)
        {
            if (State == CommandState.NotStarted)
                return CreateOutput(new TextOutput(_verbosity, _text), CommandState.Finished);

            return CreateError($"Command {CommandName} should be in other state but NotStarted");
        }

        public override CommandResult OnXmppMessageReceived(string message)
        {
            return CreateError($"XMPP is not supported for the command {CommandName}");
        }
    }
}