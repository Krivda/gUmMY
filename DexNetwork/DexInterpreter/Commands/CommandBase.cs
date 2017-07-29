using System;
using System.Collections.Generic;

namespace DexNetwork.DexInterpreter.Commands
{

    public enum CommadStatus
    {
        Finished,
        AwaitInput,
        AwaitXMPP,
        NotStarted,
        RequestResume,
    }

    public enum Verbosity
    {
        Full=0,
        Important=1,
        Critical=2
    }

    public abstract class CommandBase
    {
        protected IDexPromise _promise;


        public string CommandName { get; protected set; }

        public int MandatoryParamCount { get; protected set; }
        public int OptionalParamCount { get; protected set; }

        public CommadStatus Status { get; protected set; }
        public string CommandHelpString { get; protected set; }
        public List<string> Parameters { get; protected set; }


        protected CommandBase(IDexPromise promise)
        {
            _promise = promise;
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
        public abstract CommandResult OnXMPPInput(string message);

        public virtual CommandResult Proceed()
        {
            return CreateError($"Task {CommandName} is not resumable");
        }

        protected CommandResult EnsureStatus(CommadStatus status)
        {
            CommandResult result = null;

            if (Status != status)
            {
                result = CreateError($"Incorrected command status. Current status is '{Enum.GetName(typeof(CommadStatus), Status)}',  expected status is '{Enum.GetName(typeof(CommadStatus), status)}'. Command terminated.");
            }

            return result;
        }

        protected CommandResult CreateError(string message)
        {
            var result = new CommandResult();

            result.Error = new TextOutput(Verbosity.Critical, message);
            result.Status = CommadStatus.Finished;
            result.BlockInput = false;

            return result;
        }


        protected CommandResult CreateOutput(TextOutput output, CommadStatus status)
        {
            var result = new CommandResult();

            result.Output.Add(output); 
            result.Status = status;

            return result;
        }
    }
}