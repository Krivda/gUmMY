using System;
using DexNetwork.Structure;

namespace DexNetwork.DexInterpreter.Response
{
    public class HackInstruction
    {
        public static string CommandName { get; } = "#";


        public string Error { get; private set; } = "";
        public Node Node { get; set; }

        //executing program #2925 from calvin276 target:BlackMirror11 
        //Node defence: #36900
        //attack failed
        //Trace:
        //Proxy level decreased by 1. 
        //BlackMirror11 security log updated

        //executing program #6050 from calvin276 target:BlackMirror11 
        //Node defence: #6449300
        //attack successfull
        //Node 'firewall' disabled for 900 seconds.


        public static HackInstruction Parse(string commandOuptut)
        {
            HackInstruction result = new HackInstruction();


            /*bool foundDisabled = false;
            string[] lines = commandOuptut.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);

            if (lines[0].StartsWith("Incorrect arguments"))
            {
                result.Error = commandOuptut;
            }
            else if (commandOuptut.Contains("------"))
            {
                bool seenChildNodesMarker = false;

                foreach (string line in lines)
                {
                    string cmdLine = line.TrimStart().TrimEnd().Replace("\n", "").Replace("\r", "");

                    if (cmdLine.Contains("--------------------"))
                    {
                        //empty line at start, no use
                    }
                    if (cmdLine.Contains("not available"))
                    {
                        result.Error = commandOuptut;
                    }
                    else if (line.Contains("properties:"))
                    {
                        //Node "BlackMirror11/firewall" properties:

                        var matches = RegexUtils.GetMatchGroups(cmdLine, NODE_INFO_REGEX);
                        if (matches == null)
                        {
                            result.Error = "can't parse props line, regex failure";
                        }
                        else
                        {
                            result.NetName = matches["netName"];
                            result.Node = new Node() { Effect = "" };
                            result.Node.Links = new List<Link>();
                            result.Node.Name = matches["nodeName"];
                        }
                    }
                    else if (cmdLine.StartsWith("Installed program: "))
                    {
                        //Installed program: #6449300
                        //Installed program: none
                        long software = 0;

                        var matches = RegexUtils.GetMatchGroups(cmdLine, NODE_SOFT_REGEX);
                        if (matches == null)
                        {
                            result.Error = "can't parse installed program, regex failure";
                        }
                        else
                        {
                            if (matches.ContainsKey("softCode"))
                            {
                                software = long.Parse(matches["softCode"]);
                            }
                            else if (matches.ContainsKey("none"))
                            {
                                software = 0;
                            }
                            else
                            {
                                result.Error = "can't parse software code, no softcode / none captured";
                            }
                            result.Node.Software = software;
                        }
                    }
                    else if (cmdLine.StartsWith("Type: "))
                    {
                        //Type: Cyptographic system
                        result.Node.NodeType = cmdLine.Replace("Type: ", "");
                    }
                    else if (cmdLine.StartsWith("DISABLED for: "))
                    {
                        //DISABLED for: 395 sec
                        result.Node.Disabled = int.Parse(cmdLine.Replace("DISABLED for: ", "").Replace(" sec", ""));
                        foundDisabled = true;
                    }
                    else if (cmdLine.StartsWith("Node effect: "))
                    {
                        //Node effect: trace
                        result.Node.Effect = cmdLine.Replace("Node effect: ", "");
                    }
                    else if (cmdLine.StartsWith("Child nodes:"))
                    {
                        //Child nodes:
                        seenChildNodesMarker = true;
                    }
                    else if (seenChildNodesMarker)
                    {
                        var matches = RegexUtils.GetMatchGroups(cmdLine, NODE_NODE_REGEX);
                        if (matches == null)
                        {
                            result.Error = $"Unknown line {cmdLine} or can't parse child nodes";
                        }
                        else
                        {
                            //it can't happen
                            if (result.Node.Links == null)
                                result.Node.Links = new List<Link>();


                            result.Node.Links.Add(new Link()
                            {
                                To = matches["nodeName"],
                                LinkedNode = new Node()
                                {
                                    Name = matches["nodeName"],
                                    NodeType = matches["nodeType"],
                                    Software = long.Parse(matches["softCode"]),
                                }
                            });
                        }


                    }
                }

                if (!foundDisabled)
                    result.Node.Disabled = 0;
            }
            else
            {
                result.Error = $"Unexpected reply: \n{commandOuptut}";
            }
            */
            return result;
        }

        public static string AssembleAccessible(string networkName, Node node)
        {


            string disabledStr = "";
            if (node.Disabled != 0)
                disabledStr = $"\nDISABLED for: {node.Disabled} sec";



            string effectStr = "";
            if (!string.IsNullOrEmpty(node.Effect))
                disabledStr = $"\nNode effect: {node.Effect}";


            string childrenStr = "";
            if (node.GetChildren().Count != 0)
            {
                childrenStr = "Child nodes:";
                int i = 0;
                foreach (Node child in node.GetChildren().Values)
                {
                    //0: antivirus1(Antivirus): #1208700  
                    string childDisabled = "";
                    if (child.Disabled != 0)
                        childDisabled = " DISABLED";

                    childrenStr += $"{Environment.NewLine}{i}: {child.Name}({child.NodeType}){childDisabled}: #{child.Software}";
                    i++;
                }
            }

            return $@"--------------------
Node ""{networkName}/{node.Name}"" properties:
Installed program: #{node.Software}
Type: {node.NodeType}{disabledStr}{effectStr}{childrenStr}

END ----------------";
        }

        public static string AssembleNotAccessible(string networkName, Node node)
        {
            return $@"--------------------
{networkName} / {node.Name}: not available

END ----------------";
        }
    }
}
