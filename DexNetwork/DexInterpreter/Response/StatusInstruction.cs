using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DexNetwork.Structure;
using DexNetwork.Utils;

namespace DexNetwork.DexInterpreter.Response
{
    public class StatusInstruction
    {
        public string Login { get; set; }
        public string Target { get; set; }
        public string AdminSystem { get; set; }
        public int Proxy { get; set; }
        public string VisibleAs { get; set; }
        public string Error { get; set; }

        public static string CommandName { get; }  = "status";

        private const string REGEX =
            @"^(?<login>[\w@]+) status:
Current target: (?<target>\w+)
Current administrating system: (?<system>\w+)";


        public static StatusInstruction Parse(string commandOuptut)
        {
            StatusInstruction result = new StatusInstruction();

            Regex regex = new Regex(REGEX, RegexOptions.Multiline);
            Match match = regex.Match(commandOuptut);

            if (match.Success)
            {
                result.Login = match.Groups["login"].Value;
                result.Target = match.Groups["target"].Value;
                result.AdminSystem = match.Groups["system"].Value;
                result.VisibleAs = match.Groups["visibleLogin"].Value;
               
            }
            else
            {
                result = new StatusInstruction()
                {
                    Error = $"Unexpected reply: \n{commandOuptut}"
                };
            }


            string[] lines = commandOuptut.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);

            foreach (string line in lines)
            {
                string cmdLine = line.TrimStart().TrimEnd().Replace("\n", "").Replace("\r", "");

                if (cmdLine.StartsWith("Proxy level: "))
                {
                    result.Proxy = int.Parse(cmdLine.Replace("Proxy level: ", "").Trim());
                }
                else if (cmdLine.StartsWith("Warning: proxy not available"))
                {
                    result.Proxy = 0;
                }

            }

            return result;
        }

        public static string Assemble(string login, string target, string amdinSystem, int proxyLevel, string visibleAs)
        {
            string prxStr = "";

            if (proxyLevel == 0)
                prxStr = "Warning: proxy not available";
            else
                prxStr = $"Proxy level: {proxyLevel}{Environment.NewLine}Current proxy address: {visibleAs}";

            return $@"{login} status:
Current target: {target}
Current administrating system: {amdinSystem}
{prxStr}";
        }
    }
}