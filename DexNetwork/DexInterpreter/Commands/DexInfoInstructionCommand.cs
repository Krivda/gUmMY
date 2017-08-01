using System;
using System.Text;
using DexNetwork.DexInterpreter.Response;

namespace DexNetwork.DexInterpreter.Commands
{
    class DexInfoInstructionCommand : DexInstructionCommandBase
    {
        private long _softwareCode;
        public const string CmdName = "info";

        public DexInfoInstructionCommand(Verbosity verbosity, IDexPromise promise) : base(CmdName, verbosity, promise)
        {
            CommandHelpString = "info #<software code>";
            MandatoryParamCount = 1;
        }

        protected override string GetXmppInputForInstruction(string input)
        {
            return $"{CmdName} #{_softwareCode}";
        }

        protected override CommandResult ProcessXmppMesssage(string message)
        {
            InfoInstruction infoResult;

            try
            {
                infoResult = InfoInstruction.Parse(message);
            }
            catch (Exception e)
            {
                return CreateError($"Can't parse {CmdName} instruction output: got exception \n{e}");
            }

            if (! string.IsNullOrEmpty(infoResult.Error))
            {
                return CreateError(infoResult.Error);
            }


            TextOutput updateResult;
            try
            {
                updateResult = SyncSoftwareInfo(infoResult.Software);
            }
            catch (Exception e)
            {
                return CreateError($"Command {CmdName} failed. Can't sync command {infoResult.Code} to lib: \n{e}");
            }

            //todo: sync soft effect info

            CommandResult result = CreateOutput(new TextOutput(Verbosity, message), CommadState.Finished);
            result.Output.Add(updateResult);

            return result;

        }

        private TextOutput SyncSoftwareInfo(Structure.Software infoResultSoftware)
        {
            TextOutput result;

            Structure.Software libSoft;

            if (!Promise.SoftwareLib.All.TryGetValue(infoResultSoftware.Code, out libSoft))
            {
                //new software found.
                Promise.SoftwareLib.AddNewSoft(infoResultSoftware);

                result = new TextOutput(Verbosity, $"New {infoResultSoftware.SoftwareType} software added to Lib!");
                return result;
            }

            // known soft, trying to update
            StringBuilder updateString = new StringBuilder();

            if (!libSoft.Effect.Equals(infoResultSoftware.Effect))
            {
                updateString.AppendLine($"Effect updated from {libSoft.Effect} to {infoResultSoftware.Effect}");
                libSoft.Effect = infoResultSoftware.Effect;
            }
            if (!libSoft.InevitableEffect.Equals(infoResultSoftware.InevitableEffect))
            {
                updateString.AppendLine($"Inevitable Effect updated from {libSoft.InevitableEffect} to {infoResultSoftware.InevitableEffect}");
                libSoft.InevitableEffect = infoResultSoftware.InevitableEffect;
            }
            if (!libSoft.NodeTypesString.Equals(infoResultSoftware.NodeTypesString))
            {
                updateString.AppendLine($"Supported Node Types updated from {libSoft.NodeTypesString} to {infoResultSoftware.NodeTypesString}");
                libSoft.NodeTypesString = infoResultSoftware.NodeTypesString;
            }
            if (!libSoft.Duration.Equals(infoResultSoftware.Duration))
            {
                updateString.AppendLine($"Supported Node Types updated from {libSoft.Duration} to {infoResultSoftware.Duration}");
                libSoft.Duration = infoResultSoftware.Duration;
            }

            //check if there was an update
            if (updateString.ToString().Equals(""))
            {
                //no changes found
                result = new TextOutput(Verbosity, $"Got no new info on sofware #{libSoft.Code}.");
            }
            else
            {
                //changes found, dumping and updating
                Promise.SoftwareLib.DumpToFile();
                result = new TextOutput(Verbosity, $"Sofware #{libSoft.Code}  updated: \n{updateString}");
            }

            return result;
        }

        protected override CommandResult ParseArguments(string command)
        {
            var baseRes = base.ParseArguments(command);


            if (Parameters.Count == 1)
            {
                try
                {
                    //save software code
                    _softwareCode = long.Parse(Parameters[0].Replace("#", ""));
                }
                catch (Exception)
                {
                    string errorMsg = $"Can't extract software code from param {Parameters[0]}";

                    if (baseRes == null)
                    {
                        baseRes = CreateError(errorMsg);
                    }
                    else
                    {
                        if (baseRes.Error != null)
                        {
                            baseRes.Error.Text += $"\n{errorMsg}";
                        }
                        else
                        {
                            baseRes.Error  = new TextOutput(Verbosity.Critical, errorMsg);
                        }
                            
                    }
                }
            }

            return baseRes;
        }
    }
}