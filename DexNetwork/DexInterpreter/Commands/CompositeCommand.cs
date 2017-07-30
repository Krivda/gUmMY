using System;
using System.Collections.Generic;

namespace DexNetwork.DexInterpreter.Commands
{
    abstract class CompositeCommand : CommandBase
    {
        internal class QueuedCommand
        {
            public string CommandLine { get; set; }
            public CommandBase Command { get; set; }
        }

        public List<QueuedCommand> Commands { get; protected set; }
        protected int _commandIndex;

        //should _commands init _commands in the derived class :(
        protected CompositeCommand(IDexPromise promise) : base (promise)
        {
        }

        public override CommandResult OnCommandInput(string input)
        {
            CommandResult result;

            if (Commands == null)
                return CreateError("Commands array is null");

            if (State == CommadState.NotStarted)
            {
                result = ParseArguments(input);
                if (result != null)
                    return result;

                //execution queue started
                _commandIndex = 0;

                result = Proceed();
                return result;
            }
            else if (State == CommadState.AwaitInput)
            {
                if (!(_commandIndex < Commands.Count))
                    return CreateError($"Command index is {_commandIndex} and it exceeds commads count {Commands.Count}");

                result = Commands[_commandIndex].Command.OnCommandInput(input);
                result = HandleLast(result);

                return result;
            }

            return CreateError($"Command {CommandName} is not ready to accept input");
        }

        public override CommandResult OnXmppMessageReceived(string message)
        {
            CommandResult result;

            if (State == CommadState.AwaitXmpp)
            {
                if (Commands != null && _commandIndex < Commands.Count)
                {
                    var command = Commands[_commandIndex];

                    //translate input to child command
                    if (command.Command.State == CommadState.AwaitXmpp)
                    {
                        result = command.Command.OnXmppMessageReceived(message);

                        result = HandleLast(result);

                        return result;
                    }

                    return CreateError($"Child command '{command.Command.CommandName}' is not ready to accept XMPP input");
                }

                return CreateError("_commands not set, or _commandIndex exceeds commands count.");
            }

            result = CreateError($"Command {CommandName} is in the wrong state to accept XMPP messages. Command state is {Enum.GetName(typeof(CommadState), State)}. Message is '{message}'");

            return result;
        }

        public override CommandResult Proceed()
        {
            CommandResult result;

            if (Commands != null && Commands.Count == _commandIndex)
            {
                //commnad index exceeded
                return CreateError("Can't proceed. All commands are executed");
            }

            if (Commands != null && _commandIndex < Commands.Count )
            {
                var command = Commands[_commandIndex];

                //translate input to child command
                if (command.Command.State == CommadState.NotStarted)
                {
                    result = command.Command.OnCommandInput(command.CommandLine);

                    result = HandleLast(result);

                    return result;
                }

                return CreateError($"Child command '{command.Command.CommandName}' is not ready to start");
            }

            return CreateError("_commands not set, or _commandIndex exceeds commands count.");
        }

        private CommandResult HandleLast(CommandResult result)
        {
            //if child command finished
            if (result.State == CommadState.Finished)
            {
                //is last command?
                if (_commandIndex + 1 == Commands.Count)
                {
                    State = CommadState.Finished;
                    result.BlockInput = false;
                    return result;
                }

                _commandIndex++;
                State = CommadState.RequestResume;
                result.BlockInput = true;
                result.State = CommadState.RequestResume;
                return result;
            }

            //if child command is not finished - need to have the same status as the underlying command
            State = result.State;
            return result;
        }
    }

}