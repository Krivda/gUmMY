using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DexNetwork.DexInterpreter.Response;

namespace DexNetwork.Utils
{
    public static class RegexUtils
    {

        public static Dictionary<string, string> GetMatchGroups(string text, string pattern)
        {
            Dictionary<string, string> namedCaptureDictionary = null;
            
            Regex regex = new Regex(pattern);

            Match match = regex.Match(text);

            if (match.Success)
            {
                namedCaptureDictionary = new Dictionary<string, string>();
                GroupCollection groups = match.Groups;
                string[] groupNames = regex.GetGroupNames();
                foreach (string groupName in groupNames)
                    if (groups[groupName].Captures.Count > 0)
                        namedCaptureDictionary.Add(groupName, groups[groupName].Value);

            }
            return namedCaptureDictionary;
        }
    }
}
