namespace DexNetwork.DexInterpreter.Commands
{
    public abstract class  DexInstructionCommandBase : CommandBase
    {
        private readonly string _instruction;
        protected readonly Verbosity Verbosity;

        protected DexInstructionCommandBase(string instruction, Verbosity verbosity, IDexPromise promise) : base(promise)
        {
            _instruction = instruction;
            Verbosity = verbosity;
            State = CommadState.NotStarted;
        }

        public override CommandResult OnCommandInput(string input)
        {
            CommandResult result = EnsureState(CommadState.NotStarted);
            if (result != null)
                return result;

            result = ParseArguments(input);
            if (result != null)
                return result;

            string xmppCommand = GetXmppInputForInstruction(input);

            State = CommadState.AwaitXmpp;

            result = CreateOutput(new TextOutput(Verbosity, $">> {xmppCommand}"), CommadState.AwaitXmpp);
            result.XMPPCommand = xmppCommand;
            result.State = CommadState.AwaitXmpp;

            return result;
        }

        protected virtual string GetXmppInputForInstruction(string input)
        {
            return $"{_instruction}";
        }

        public override CommandResult OnXmppMessageReceived(string message)
        {
            CommandResult result = EnsureState(CommadState.AwaitXmpp);
            if (result != null)
                return result;

            return ProcessXmppMesssage(message);

        }

        protected abstract CommandResult ProcessXmppMesssage(string message);

    }
}