using System.Text.RegularExpressions;
using System.Windows.Forms;

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
Current administrating system: (?<system>\w+)
Proxy level: (?<proxy>\w+)
Current proxy address: (?<visibleLogin>[\w@]+)";


        public static StatusInstruction Parse(string commandOuptut)
        {
            StatusInstruction result = null;

            Regex regex = new Regex(REGEX, RegexOptions.Multiline);

            Match match = regex.Match(commandOuptut);

            if (match.Success)
            {
                result = new StatusInstruction()
                {
                    Login = match.Groups["login"].Value,
                    Target = match.Groups["target"].Value,
                    AdminSystem = match.Groups["system"].Value,
                    Proxy = int.Parse(match.Groups["proxy"].Value),
                    VisibleAs = match.Groups["visibleLogin"].Value
                };
            }
            else
            {
                result = new StatusInstruction()
                {
                    Error = $"Unexpected reply: \n{commandOuptut}"
                };
            }

            return result;
        }

        public static string Assemble(string login, string target, string amdinSystem, int proxyLevel, string visibleAs)
        {
            return $@"{login} status:
Current target: {target}
Current administrating system: {amdinSystem}
Proxy level: {proxyLevel}
Current proxy address: {visibleAs}";
        }
    }
}