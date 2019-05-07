using System;
using System.Collections.Generic;

namespace DexNetwork.DexInterpreter.Commands
{
    class MacroCommand : CompositeCommand
    {
        public const string CmdName = "macro";

        public MacroCommand(IDexPromise promise) : base(promise)
        {
            CommandHelpString = $"<command>[newline][command][newline][command]";
            State = CommandState.NotStarted;
            CommandName = CmdName.ToLower();
            MandatoryParamCount = 0;
            OptionalParamCount = 0;

        }

        public override CommandResult OnCommandInput(string input)
        {
            CommandResult result = EnsureState(CommandState.NotStarted);
            if (result != null)
                return result;

            string[] lines = input.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
            Commands = new List<QueuedCommand>();
            int index = 0;

            foreach (var commandLine in lines)
            {
                if (!string.IsNullOrEmpty(commandLine.Trim()))
                {
                    index++;
                    CommandBase cmd = Promise.CommandResolver.ResolveCommand(commandLine, Promise);
                    if (cmd == null)
                        return CreateError($"Input on line {index} '{commandLine}' is not a valid command");

                    Commands.Add(new QueuedCommand
                    {
                        CommandLine = commandLine,
                        Command = cmd,

                    });
                }
            }

            result = base.OnCommandInput(input);
            return result;
        }


    }
}