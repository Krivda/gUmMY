using System.Collections.Generic;
using DexNetwork.Structure;

namespace DexNetwork.DexInterpreter.Commands
{
    public class HackCommand : CompositeCommand
    {
        public const string CmdName = "hack";

        private string _nodeName;
        private long _softwareCode;

        private string _waitForInput = "";

        private CommadState _delayedState;
        private Node _node;
        private Software _software;

        public HackCommand(IDexPromise promise) : base(promise)
        {
            CommandHelpString = $"{CmdName} <node> #<software>";
            State = CommadState.NotStarted;
            CommandName = CmdName.ToLower();
            MandatoryParamCount = 2;
        }

        public override CommandResult OnCommandInput(string input)
        {

            CommandResult result;
            if (_waitForInput.Equals("confirmHack"))
            {
                result = EnsureState(CommadState.AwaitInput);
                if (result != null)
                    return result;

                if ("y".Equals(input.ToLower().Trim()))
                {
                    result = CreateOutput(new TextOutput(Verbosity.Critical, "Proceeding with hack"), _delayedState);
                    return result;
                }
                else
                {
                    result = CreateOutput(new TextOutput(Verbosity.Critical, "Ok, command terminated."), CommadState.Finished);
                    return result;
                }
            }


            result = EnsureState(CommadState.NotStarted);
            if (result != null)
                return result;

            if (State == CommadState.NotStarted)
            {
                result = ParseArguments(input);
                if (result != null)
                    return result;

                _nodeName = Parameters[0];
                _softwareCode = long.Parse(Parameters[1].Replace("#", "").Trim());

                Commands = new List<QueuedCommand>();

                Commands.Add(new QueuedCommand
                {
                    CommandLine = $"{DexLookInstructionCommand.CmdName} {_nodeName}",
                    Command = new DexLookInstructionCommand(Verbosity.Important, Promise),
                });

                Commands.Add(new QueuedCommand
                {
                    CommandLine = $"#{_softwareCode} {_nodeName}",
                    Command = new DexHackInstructionCommand(Verbosity.Normal, Promise),
                });

                Commands.Add(new QueuedCommand
                {
                    CommandLine = $"{DexStatusInstructionCommand.CmdName}",
                    Command = new DexStatusInstructionCommand(Verbosity.Normal, Promise),
                });

                result = base.OnCommandInput(input);
            }
            return result;
        }


        private bool ValidateParamsAfterLook(CommandResult lookCommandResult)
        {
            TextOutput validationResultMessage = new TextOutput(Verbosity.Critical, "$\n Validating");

            lookCommandResult.Output.Add(validationResultMessage);

            bool validationSuccess = true;

            Node node;
            if (Promise.Network.Nodes.TryGetValue(_nodeName, out node))
            {
                _node = node;

                var compatLevel =  Promise.SoftwareLib.CheckExploitCompatibility(_softwareCode, _node);
                if (compatLevel != SoftwareLib.SoftwareCheckResult.Valid)
                {
                    if (compatLevel == SoftwareLib.SoftwareCheckResult.Unknown)
                    {
                        lookCommandResult.Output.Add(new TextOutput(Verbosity.Critical, $"It's UNKNOWN if exploit {_softwareCode} will surpass node {_nodeName} defence! (Math is ok, but not enough info on node or software to validate fitness)"));
                        validationSuccess = false;
                    }
                    else
                    {
                        lookCommandResult.Output.Add(new TextOutput(Verbosity.Critical, $"*****WARNING*****{_softwareCode} WONT surpass node {_nodeName} defence!"));
                        validationSuccess = false;
                    }
                }
            }
            else
            {
                lookCommandResult.Output.Add(new TextOutput(Verbosity.Critical, $"Node {_nodeName} is not known to exist in network. "));
                validationSuccess = false;
            }


            Software exploit;
            if (Promise.SoftwareLib.Exloits.TryGetValue(_softwareCode, out exploit))
            {
                _software = exploit;
            }
            else
            {
                lookCommandResult.Output.Add(new TextOutput(Verbosity.Critical, $"Exploit #{_softwareCode} is NOT known (not in local lib)! Better use 'info #{_softwareCode}' before hacking with it"));
                validationSuccess = false;
            }

            if (validationSuccess)
            {
                validationResultMessage.Text = "\nHack validation revealed issues:";
                lookCommandResult.Output.Add(new TextOutput(Verbosity.Critical, $"\n\n Do you want to procced anyway ? (y/n) "));
            }
            else
            {
                validationResultMessage.Text = "\nValidation - OK.";
            }

            return validationSuccess;

        }

        public override CommandResult OnXmppMessageReceived(string message)
        {
            var result = base.OnXmppMessageReceived(message);

            if (_commandIndex == 1) //look finished
            {

                if (result.Error == null) // no reason to validate after error
                {
                    if (!ValidateParamsAfterLook(result))
                    {
                        result.Output.Add(new TextOutput(Verbosity.Critical, "\n Hack is UNSAFE. Proceed (y/n)?"));
                        _delayedState = result.State;
                        _waitForInput = "confirmHack";
                        result.State = CommadState.AwaitInput;
                        result.BlockInput = false;

                        State = CommadState.AwaitInput;
                    }
                }
            }

            return result;
        }
    }
}
