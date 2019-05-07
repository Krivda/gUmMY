using System;
using System.Collections.Generic;

namespace DexNetwork.DexInterpreter.Commands
{
    public abstract class CompositeCommand : CommandBase
    {
        public class QueuedCommand
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

            if (State == CommandState.NotStarted)
            {
                result = ParseArguments(input);
                if (result != null)
                    return result;

                //execution queue started
                _commandIndex = 0;

                result = Proceed();
                return result;
            }
            else if (State == CommandState.AwaitInput)
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
            base.OnXmppMessageReceived(message);

            CommandResult result;

            if (State == CommandState.AwaitXmpp)
            {
                if (Commands != null && _commandIndex < Commands.Count)
                {
                    var command = Commands[_commandIndex];

                    //translate input to child command
                    if (command.Command.State == CommandState.AwaitXmpp)
                    {
                        result = command.Command.OnXmppMessageReceived(message);

                        result = HandleLast(result);

                        return result;
                    }

                    return CreateError($"Child command '{command.Command.CommandName}' is not ready to accept XMPP input");
                }

                return CreateError("_commands not set, or _commandIndex exceeds commands count.");
            }

            result = CreateError($"Command {CommandName} is in the wrong state to accept XMPP messages. Command state is {Enum.GetName(typeof(CommandState), State)}. Message is '{message}'");

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
                if (command.Command.State == CommandState.NotStarted)
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
            if (result.State == CommandState.Finished)
            {
                //is last command?
                if (_commandIndex + 1 == Commands.Count)
                {
                    State = CommandState.Finished;
                    result.BlockInput = false;
                    return result;
                }

                _commandIndex++;
                State = CommandState.RequestResume;
                result.BlockInput = true;

                if (result.Error != null)
                {
                    //terminate all on error
                    result.State = CommandState.Finished;
                }
                else
                {
                    result.State = CommandState.RequestResume;
                }
                return result;
            }

            //if child command is not finished - need to have the same status as the underlying command
            State = result.State;
            return result;
        }
    }
}