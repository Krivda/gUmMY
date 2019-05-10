using System;

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

            State = CommandState.AwaitXmpp;

            string xmppCommand = GetXmppInputForInstruction(input);

            if (!string.IsNullOrEmpty(xmppCommand))
            {
                result = new CommandResult
                    { XmppCommand = xmppCommand,
                        State = State};

            }
            else
            {
                result = result = new CommandResult
                {
                    State = CommandState.Finished
                }; ;
            }

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