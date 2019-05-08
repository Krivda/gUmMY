namespace SRMatrixNetwork.Commands.ServerSide
{
    public abstract class  SRInstructionCommandBase : CommandBase
    {
        private readonly string _instruction;
        protected readonly Verbosity Verbosity;

        protected SRInstructionCommandBase(string instruction, Verbosity verbosity, IDexPromise promise) : base(promise)
        {
            _instruction = instruction;
            Verbosity = verbosity;
            State = CommandState.NotStarted;
        }

        public override CommandResult OnCommandInput(string input)
        {
            CommandResult result = EnsureState(CommandState.NotStarted);
            if (result != null)
                return result;

            result = ParseArguments(input);
            if (result != null)
                return result;

            string xmppCommand = GetXmppInputForInstruction(input);

            State = CommandState.AwaitXmpp;

            result = CreateOutput(new TextOutput(Verbosity, $">> {xmppCommand}"), CommandState.AwaitXmpp);
            result.XmppCommand = xmppCommand;
            result.State = CommandState.AwaitXmpp;

            return result;
        }

        protected virtual string GetXmppInputForInstruction(string input)
        {
            return $"{_instruction}";
        }

        public override CommandResult OnXmppMessageReceived(string message)
        {
            base.OnXmppMessageReceived(message);

            CommandResult result = EnsureState(CommandState.AwaitXmpp);
            if (result != null)
                return result;

            return ProcessXmppMessage(message);

        }

        protected abstract CommandResult ProcessXmppMessage(string message);

    }
}