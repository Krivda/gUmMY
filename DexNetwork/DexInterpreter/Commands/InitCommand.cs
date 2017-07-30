using System;
using System.Collections.Generic;

namespace DexNetwork.DexInterpreter.Commands
{
    class InitCommand : CompositeCommand
    {
        public const string CmdName = "init";

        public string Login { get; protected set; }
        public string Realm { get; protected set; }
        public string Password { get; protected set; }


        public InitCommand(IDexPromise promise) :base (promise)
        {
            CommandHelpString = $"{CmdName} <login> <pwd> [realm]";
            State = CommadState.NotStarted;
            Promise = promise;
            CommandName = CmdName.ToLower();
            MandatoryParamCount = 2;
            OptionalParamCount = 1;

        }

        public override CommandResult OnCommandInput(string input)
        {
            CommandResult result = EnsureState(CommadState.NotStarted);
            if (result != null)
                return result;

            result = ParseArguments(input);
            if (result != null)
                return result;

            Login = Parameters[0];
            Password = Parameters[1];
            Realm = Parameters[2];

            Commands = new List<QueuedCommand>();

            Commands.Add(new QueuedCommand
            {
                CommandLine = $"{WellcomeCommand.CmdName}",
                Command =new WellcomeCommand("Wellcome ! ", Verbosity.Normal, Promise),
                
            });

            Commands.Add(new QueuedCommand
            {
                CommandLine = $"{LoginCommand.CmdName} {Login} {Password} {Realm}",
                Command = new LoginCommand(Promise),
            });

            Commands.Add(new QueuedCommand
            {
                CommandLine = $"{DexStatusInstructionCommand.CmdName}",
                Command = new DexStatusInstructionCommand(Verbosity.Normal, Promise),
            });

            result = base.OnCommandInput(input);

            return result;
           
        }

    }
}