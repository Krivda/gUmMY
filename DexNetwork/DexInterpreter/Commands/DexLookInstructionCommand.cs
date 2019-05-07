using System;
using System.Linq;
using System.Text;
using DexNetwork.DexInterpreter.Response;
using DexNetwork.Structure;

namespace DexNetwork.DexInterpreter.Commands
{
    class DexLookInstructionCommand : DexInstructionCommandBase
    {
        private string _nodeName;
        public const string CmdName = "look";

        public DexLookInstructionCommand(Verbosity verbosity, IDexPromise promise) : base(CmdName, verbosity, promise)
        {
            CommandHelpString = "look <node>";
            MandatoryParamCount = 1;
        }

        protected override string GetXmppInputForInstruction(string input)
        {
            return $"{CmdName} {_nodeName}";
        }

        protected override CommandResult ProcessXmppMesssage(string message)
        {
            LookInstruction infoResult;

            try
            {
                infoResult = LookInstruction.Parse(message);
            }
            catch (Exception e)
            {
                return CreateError($"Can't parse {CmdName} instruction output: got exception \n{e}\n{message}");
            }

            if (!string.IsNullOrEmpty(infoResult.Error))
            {
                return CreateError(infoResult.Error);
            }


            TextOutput updateResult;
            try
            {
                updateResult = SyncNodeInfo(infoResult.Node);
            }
            catch (Exception e)
            {
                return CreateError($"Command {CmdName} failed. Can't sync node {infoResult.Node.Name} to net: \n{e}\n{message}");
            }


            CommandResult result = CreateOutput(new TextOutput(Verbosity, message), CommandState.Finished);
            result.Output.Add(updateResult);

            return result;

        }

        private TextOutput SyncNodeInfo(Node gotNode)
        {
            TextOutput result;

            Node knownNode;

            if (!Promise.Network.Nodes.TryGetValue(gotNode.Name, out knownNode))
            {
                //new software found.
                throw new Exception("Can't sync node back to net, it's not found in net. That should never happen");
            }

            // known node, trying to update
            StringBuilder updateString = new StringBuilder();


            knownNode.Explored = true;

            if (!knownNode.Effect.Equals(gotNode.Effect))
            {
                updateString.AppendLine($"Effect updated from {knownNode.Effect} to {gotNode.Effect}");
                knownNode.Effect = gotNode.Effect;
            }
            if (gotNode.Disabled !=0)
            {
                updateString.AppendLine($"Disabled updated from {knownNode.Disabled} to {gotNode.Disabled}");
                knownNode.Disabled = gotNode.Disabled;
            }

            if (!knownNode.Software.Equals(gotNode.Software))
            {
                updateString.AppendLine($"Software updated from {knownNode.Software} to {gotNode.Software}");
                knownNode.Disabled = gotNode.Disabled;
            }

            foreach (var gotNodeLink in gotNode.Links)
            {
                var knownNodeLink = knownNode.Links.FirstOrDefault(l => l.To.Equals(gotNodeLink.To));
                if (knownNodeLink != null)
                {
                    if (! gotNodeLink.LinkedNode.Software.Equals(knownNodeLink.LinkedNode.Software))
                    {
                        updateString.AppendLine($"Child Node  {knownNodeLink.To} software updated from {knownNodeLink.LinkedNode.Software} to {gotNodeLink.LinkedNode.Software}");
                        knownNodeLink.LinkedNode.Software = gotNodeLink.LinkedNode.Software;

                    }
                }
            }

            //check if there was an update
            if (updateString.ToString().Equals(""))
            {
                //no changes found
                result = new TextOutput(Verbosity, $"Got no new info on node #{knownNode.Name}.");
            }
            else
            {
                //changes found, dumping and updating
                Promise.Network.DumpToFile();
                result = new TextOutput(Verbosity, $"Node {knownNode.Name}  updated: \n{updateString}");
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
                    //save node name
                    _nodeName = Parameters[0];
                }
                catch (Exception)
                {
                    string errorMsg = $"Can't extract node name from param {Parameters[0]}";

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
                            baseRes.Error = new TextOutput(Verbosity.Critical, errorMsg);
                        }

                    }
                }
            }

            return baseRes;
        }
    }
}
