using System.Collections.Generic;
using DexNetwork.DexInterpreter.Response;

namespace DexNetwork.DexInterpreter.Commands
{
    class DexTargetCommand : CommandBase
    {
        private string _awaitFor;
        public const string CmdName = "target";

        private const string RESPONSE_OK = "ok";

        private const string WAIT_TARGET = "wait for target response";
        private const string WAIT_STATUS = "ok";

        public DexTargetCommand(IDexPromise promise) : base(promise)
        {
            CommandHelpString = "target <networkName>  //i.e target Blackmirror11";
            MandatoryParamCount = 1;
            OptionalParamCount = 0;
            Status = CommadStatus.NotStarted;
        }

        public override CommandResult OnCommandInput(string input)
        {
            CommandResult result = EnsureStatus(CommadStatus.AwaitInput);
            if (result != null)
                return result;

            result = ParseArguments(input);
            if (result != null)
                return result;


            if (Parameters[1].Equals(""))
                return CreateError($"Network param is not set. Usage: {CommandHelpString}");

            string xmppCommand = $"{CmdName} Parameters[1]";

            result = CreateOutput(new TextOutput(Verbosity.Critical, $">> ${xmppCommand} "), CommadStatus.AwaitXMPP);
            result.BlockInput = true;

            Status = CommadStatus.AwaitXMPP;
            result.XMPPCommand = StatusInstruction.CommandName;

            _awaitFor = "Ok";

            return result;
        }

        public override CommandResult OnXMPPInput(string message)
        {
            CommandResult result = EnsureStatus(CommadStatus.AwaitXMPP);
            if (result != null)
                return result;

            if (_awaitFor.Equals(WAIT_TARGET))
            {
                if (message.ToLower().Equals(RESPONSE_OK.ToLower()))
                {
                    result = CreateOutput(new TextOutput(Verbosity.Important, $"{message}"), CommadStatus.AwaitXMPP);
                    result.BlockInput = false;
                }
                else
                {
                    result = CreateError($"Unknown response while awaiting '{RESPONSE_OK}': {message}");
                }
            }
            else if (_awaitFor.Equals(WAIT_STATUS))
            {
                StatusInstruction status = StatusInstruction.Parse(message);
                if (status == null)
                {
                    result = CreateError($"Couldn't parse status command result: {message}");
                }
                else
                {
                    result = CreateOutput(new TextOutput(Verbosity.Important, $"{message}"), CommadStatus.Finished);
                    result.BlockInput = false;
                    result.UpdatedNetStatus = status;
                    result.Output.Add(new TextOutput(Verbosity.Important, $"Established link to network {status.Target}!"));
                }
            }
            else
            {
                result = CreateError($"Unxecpted XMPP result. We waited for {_awaitFor}. Result: {message}");
            }

            return result;
        }
    }
}