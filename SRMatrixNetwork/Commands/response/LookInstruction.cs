using System;
using System.Collections.Generic;
using DexNetwork.Structure;
using DexNetwork.Utils;

namespace SRMatrixNetwork.Commands.response
{
    public class LookInstruction
    {
        public static string CommandName { get; } = "look";

        private const string NODE_INFO_REGEX = @"Node ""(?<netName>\w+)/(?<nodeName>\w+)"" properties:";
        private const string NODE_SOFT_REGEX = @"Installed program: ((#(?<softCode>\d+))|(?<none>none))";
        private const string NODE_NODE_REGEX = @"(?<index>\d): (?<nodeName>\w+)\((?<nodeType>\w+)\): #(?<softCode>\d+)(?<disabled> \w+)?";

        public string NetName { get; private set; } = "";
        public string Error { get; private set; } = "";
        public Node Node { get; set; }

        public static LookInstruction Parse(string commandOuptut)
        {
            LookInstruction result = new LookInstruction();

            //--------------------
            //BlackMirror11 / 0: not available
            //
            //END----------------


            //Type: Cyptographic system

            //DISABLED for: 395 sec
            //Node effect: trace
            //Child nodes:
            //0: antivirus4(Antivirus): #405900 DISABLED 
            //1: router2(Router): #2150775


            //--------------------
            //Node "BlackMirror11/firewall" properties:
            //Installed program: #6449300
            //Type: Firewall
            //DISABLED for: 885 sec
            //Child nodes:
            //0: antivirus1(Antivirus): #1208700  
            //1: antivirus2(Antivirus): #2739100  
            //
            //END----------------

            bool foundDisabled = false;
            string[] lines = commandOuptut.Split(new [] { Environment.NewLine, "\n" }, StringSplitOptions.None);

            if (lines[0].StartsWith("Incorrect arguments"))
            {
                result.Error = commandOuptut;
            }
            else if (commandOuptut.Contains("------"))
            {
                bool seenChildNodesMarker = false;

                foreach (string line in lines)
                {
                    string cmdLine = line.TrimStart().TrimEnd().Replace("\n","").Replace("\r", "");

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
                            result.Node = new Node() {Effect = ""};
                            result.Node.Links = new List<Link>();
                            result.Node.Name = matches["nodeName"];
                        }
                    }
                    else if (cmdLine.StartsWith("Installed program: "))
                    {
                        //Installed program: #6449300
                        //Installed program: none
                        long software=0;

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

                if (!foundDisabled && result.Node != null)
                    result.Node.Disabled = 0;
            }
            else
            {
                result.Error = $"Unexpected reply: \n{commandOuptut}";
            }

            return result;
        }

        public static string AssembleAccessible(string networkName, Node node)
        {


            string disabledStr = "";
            if (node.Disabled!=0)
                disabledStr = $"\nDISABLED for: {node.Disabled} sec";


            
            string effectStr = "";
            if (! string.IsNullOrEmpty(node.Effect))
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