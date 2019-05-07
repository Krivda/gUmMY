using System;
using System.Collections.Generic;
using NLog;

namespace DexNetwork.DexInterpreter.Commands
{

    public enum CommandState
    {
        Finished,
        AwaitInput,
        AwaitXmpp,
        NotStarted,
        RequestResume,
    }

    public enum Verbosity
    {
        Normal=0,
        Important=1,
        Critical=2
    }

    public abstract class CommandBase
    {
        protected IDexPromise Promise;

        protected Logger Logger = LogManager.GetCurrentClassLogger();


        public string CommandName { get; protected set; }

        public int MandatoryParamCount { get; protected set; }
        public int OptionalParamCount { get; protected set; }

        public CommandState State { get; protected set; }
        public string CommandHelpString { get; protected set; }
        public List<string> Parameters { get; protected set; }


        protected CommandBase(IDexPromise promise)
        {
            Promise = promise;
        }

        protected virtual CommandResult ParseArguments(string command)
        {
            try
            {
                string[] split = command.Split(' ');
                Parameters = new List<string>();

                if (split.Length == 0)
                {
                    return CreateError("Command is empty.");
                }

                if (!(split.Length > 0))
                {
                    //check command name
                    if (!split[0].Equals(CommandName))
                        return CreateError($"Wrong command. Got {command}, expected {CommandName}.");
                }

                if (split.Length < MandatoryParamCount + 1)
                    return CreateError($"Wrong arguments count. Got {split.Length - 1}, expected {MandatoryParamCount}.\nCommand syntax: {CommandHelpString}.");

                for (int i = 1; i < MandatoryParamCount + OptionalParamCount + 1; i++)
                {

                    if (i < MandatoryParamCount + 1)
                    {
                        if (String.IsNullOrEmpty(split[i].Trim()))
                        {
                            return CreateError($"{i - 1} param is missing.\nCommand syntax: {CommandHelpString}.");
                        }

                        Parameters.Add(split[i]);
                    }
                    else
                    {
                        //fill params colletion. If param is present = store his value, else - store ""
                        if (i < split.Length)
                        {
                            Parameters.Add(split[i]);
                        }
                        else
                            Parameters.Add("");
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                return CreateError($"Can't parse command '{CommandName}'. Exception:\n{e}");
            }
        }
        

        public abstract CommandResult OnCommandInput(string input);

        public virtual CommandResult OnXmppMessageReceived(string message)
        {
            Logger.Info("Session started");

            return null;
        }

        public virtual CommandResult Proceed()
        {
            return CreateError($"Task {CommandName} is not resumable");
        }

        protected CommandResult EnsureState(CommandState state)
        {
            CommandResult result = null;

            if (State != state)
            {
                result = CreateError($"Incorrect command state. Current state is '{Enum.GetName(typeof(CommandState), State)}',  expected state is '{Enum.GetName(typeof(CommandState), state)}'. Command terminated.");
            }

            return result;
        }

        protected CommandResult CreateError(string message)
        {
            var result = new CommandResult();

            result.Error = new TextOutput(Verbosity.Critical, message);
            result.State = CommandState.Finished;
            result.BlockInput = false;

            return result;
        }


        protected CommandResult CreateOutput(TextOutput output, CommandState state)
        {
            var result = new CommandResult();

            result.Output.Add(output); 
            result.State = state;

            return result;
        }
    }
}