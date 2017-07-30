using DexNetwork.DexInterpreter.Response;

namespace DexNetwork.DexInterpreter.Commands
{
    public abstract class  DexInstructionCommandBase : CommandBase
    {
        private string _waitfor;
        private readonly string _instruction;
        protected readonly Verbosity Verbosity;

        public DexInstructionCommandBase(string instruction, Verbosity verbosity, IDexPromise promise) : base(promise)
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

            _waitfor = _instruction;
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

            if ( _waitfor.Equals(_instruction))
            {
                _waitfor = "";
                return ProcessXmppMesssage(message);
            }
            _waitfor = "";
            return CreateError($"Invalid command wait state. Expected {_instruction} output, got {message}.");
        }

        protected abstract CommandResult ProcessXmppMesssage(string message);

    }
}