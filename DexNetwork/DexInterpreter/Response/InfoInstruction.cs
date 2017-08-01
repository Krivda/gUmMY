using System;
using DexNetwork.Structure;

namespace DexNetwork.DexInterpreter.Response
{
    public class InfoInstruction
    {
        public int Code { get; set; } = 0;
        public string Effect { get; set; } = "";
        public string InevitableEffect { get; set; } = "";
        public string SupportedNodes { get; set; } = "";
        public int Duration { get; set; } = 0;
        public string ProgrammType { get; set; } = "Unknown";

        public string Error { get; set; } = "";

        public Structure.Software Software { get; set; }

        public static string CommandName { get; } = "info";

        public static InfoInstruction Parse(string commandOuptut)
        {
            InfoInstruction result = new InfoInstruction();

            //#2294523 programm info:
            //    Effect: trace
            //    Inevitable effect: logname
            //    Allowed node types:
            //    -Firewall
            //    - Antivirus
            //    - VPN
            //    - Brandmauer
            //    - Router
            //    - Traffic monitor
            //    - Cyptographic system
            //    END ----------------

            //or Incorrect arguments.Usage: info #<program>

            string[] lines = commandOuptut.Split(new [] { Environment.NewLine, "\n" }, StringSplitOptions.None);

            if (lines[0].StartsWith("Incorrect arguments"))
            {
                result.Error = commandOuptut;
            }
            else if (commandOuptut.Contains("------"))
            {
                string supportedNodes = "";
                string delimiter = "";
                foreach (string line in lines)
                {
                    string cmdLine = line.TrimStart().TrimEnd();

                    if (cmdLine.StartsWith("incorrect program"))
                    {
                        result.Error = commandOuptut;
                    }
                    else if (line.Contains("programm info"))
                    {
                        string codeStr = cmdLine.Replace("programm info:", "").Replace("#", "").Trim();
                        result.Code = int.Parse(codeStr);
                    }
                    else if (cmdLine.StartsWith("Effect: "))
                    {
                        result.Effect = cmdLine.Replace("Effect: ", "");
                    }
                    else if (cmdLine.StartsWith("Inevitable effect: "))
                    {
                        result.InevitableEffect = cmdLine.Replace("Inevitable effect: ", "");
                    }
                    else if (cmdLine.StartsWith("Allowed node types:"))
                    {

                    }
                    else if (cmdLine.StartsWith("-") && !line.StartsWith("---"))
                    {
                        supportedNodes += $"{delimiter}{cmdLine.Replace("-", "").Replace(Environment.NewLine, "")}";
                        delimiter = ",";
                    }
                    else if (cmdLine.StartsWith("Duration:"))
                    {
                        string durStr = cmdLine.Replace("Duration:", "").Replace("sec.", "");
                        result.Duration = int.Parse(durStr);
                    }
                }

                result.SupportedNodes = $"[{supportedNodes}]";

                result.Software = new Structure.Software()
                    {
                        Code = result.Code,
                        Duration = result.Duration,
                        InevitableEffect = result.InevitableEffect,
                        Effect = result.Effect,
                        NodeTypesString = result.SupportedNodes,
                        SoftwareType = result.Duration==0? "protection" : "exploit"
                };
            }
            else
            {
                result.Error = $"Unexpected reply: \n{commandOuptut}";
            }

            return result;
        }

        public static string Assemble(string programm, string effect, string inevitableEffect, string nodeTypes, int duration)
        {
            string inevitableEffectString = "";
            if (!string.IsNullOrEmpty(inevitableEffect))
                inevitableEffectString = $"\nInevitable effect: {inevitableEffect}";

            string nodes = "";
            string delim = "";
            foreach (var nodeType in nodeTypes.Replace("[","").Replace("]", "").Split(','))
            {
                nodes += $"{delim} -{nodeType}";
                delim = Environment.NewLine;
            }

            string durationStr = "";
            if (duration != 0)
                durationStr = $"{Environment.NewLine}Duration: {duration}sec.";

            return $@"--------------------
#{programm} programm info:
Effect: {effect}{inevitableEffectString}
Allowed node types:
{nodes}{durationStr}
END ----------------";
        }

        public static string Assemble(Structure.Software programm)
        {
            return Assemble(programm.Code.ToString(), programm.Effect, programm.InevitableEffect, programm.NodeTypesString, programm.Duration); 
        }


    }
}