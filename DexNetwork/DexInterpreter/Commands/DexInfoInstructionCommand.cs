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
            InfoInstruction infoResult = null;

            try
            {
                infoResult = InfoInstruction.Parse(message);
            }
            catch (Exception e)
            {
                return CreateError($"Can't parse {CmdName} instruction output: got exception \n{e}");
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
            

            CommandResult result = CreateOutput(updateResult, CommadState.Finished);

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
                libSoft.Effect = infoResultSoftware.Effect;
                updateString.AppendLine($"Effect updated from {libSoft.Effect} to {infoResultSoftware.Effect}");
            }
            if (!libSoft.InevitableEffect.Equals(infoResultSoftware.InevitableEffect))
            {
                libSoft.InevitableEffect = infoResultSoftware.InevitableEffect;
                updateString.AppendLine($"Inevitable Effect updated from {libSoft.InevitableEffect} to {infoResultSoftware.InevitableEffect}");
            }
            if (!libSoft.NodeTypesString.Equals(infoResultSoftware.NodeTypesString))
            {
                libSoft.NodeTypesString = infoResultSoftware.NodeTypesString;
                updateString.AppendLine($"Supported Node Types updated from {libSoft.NodeTypesString} to {infoResultSoftware.NodeTypesString}");
            }
            if (!libSoft.Duration.Equals(infoResultSoftware.Duration))
            {
                libSoft.Duration = infoResultSoftware.Duration;
                updateString.AppendLine($"Supported Node Types updated from {libSoft.Duration} to {infoResultSoftware.Duration}");
            }

            //check if there was an update
            if (updateString.ToString().Equals(""))
            {
                //no changes found
                result = new TextOutput(Verbosity, $"No new info on sofware #{libSoft.Code} got.");
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
                catch (Exception e)
                {
                    string errorMsg = $"Can't extract software code from param {Parameters[1]}";

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