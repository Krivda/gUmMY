using System.Collections.Generic;

namespace DexNetwork.DexInterpreter.Commands
{
    class TargetCommand : CompositeCommand
    {
        public const string CmdName = "target";

        public string Network { get; protected set; }

        public TargetCommand(IDexPromise promise) : base(promise)
        {
            CommandHelpString = $"{CmdName} <network>";
            State = CommadState.NotStarted;
            CommandName = CmdName.ToLower();
            MandatoryParamCount = 1;
        }

        public override CommandResult OnCommandInput(string input)
        {
            CommandResult result = EnsureState(CommadState.NotStarted);
            if (result != null)
                return result;

            result = ParseArguments(input);
            if (result != null)
                return result;

            Network = Parameters[0];

            Commands = new List<QueuedCommand>();

            Commands.Add(new QueuedCommand
            {
                CommandLine = $"${DexTargetInstructionCommand.CmdName} {Network}",
                Command = new DexTargetInstructionCommand(Verbosity.Important, Promise),
            });

            Commands.Add(new QueuedCommand
            {
                CommandLine = $"{DexStatusInstructionCommand.CmdName}",
                Command = new DexStatusInstructionCommand(Verbosity.Normal, Promise),
            });

            //todo: look fw?

            result = base.OnCommandInput(input);

            return result;

        }

    }
}