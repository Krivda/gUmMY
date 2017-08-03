using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DexNetwork.Structure
{

    [XmlRoot(ElementName = "SoftwareLib")]
    public class SoftwareLib
    {

        public enum SoftwareCheckResult
        {
            Valid = 0,
            Unknown = 1,
            Invalid = 2,
        }


        private string _fileName;

        [XmlArray("AllSoft")]
        [XmlArrayItem("software", Type = typeof(Software))]
        public List<Software> Software { get; set; }

        [XmlIgnore]
        public Dictionary<long, Software> Exloits { get; set; } = new Dictionary<long, Software>();
        [XmlIgnore]
        public Dictionary<long, Software> Defences { get; set; } = new Dictionary<long, Software>();
        [XmlIgnore]
        public Dictionary<long, Software> Unknown { get; set; } = new Dictionary<long, Software>();

        [XmlIgnore]
        public Dictionary<long, Software> All { get; set; } = new Dictionary<long, Software>();

        public void Init(string fileName)
        {
            _fileName = fileName;

            StringBuilder errors = new StringBuilder();
            foreach (var software in Software)
            {
                try
                {
                    AddNewSoft(software, "");
                }
                catch (ArgumentException e)
                {
                    errors.AppendLine(e.Message);
                }
            }

            if (!string.IsNullOrEmpty(errors.ToString()))
            {
                throw new Exception($"Can't load software lib: \n{errors}");
            }
        }

        public void AddNewSoft(Software software, string networkName)
        {
            if (string.IsNullOrEmpty(software.SoftwareType))
                software.SoftwareType = "";


            if (string.IsNullOrEmpty(software.CodeStr))
                software.CodeStr = "";

            
            if (software.Code==0)
                software.Code = long.Parse(software.CodeStr);

            if (software.Code != 0)
            {
                software.CodeStr = software.Code.ToString();
            }

            if (string.IsNullOrEmpty(software.Effect))
                software.Effect = "";


            if (string.IsNullOrEmpty(software.InevitableEffect))
                software.InevitableEffect = "";

            if (string.IsNullOrEmpty(software.NodeTypesString))
                software.NodeTypesString = "";

            if (string.IsNullOrEmpty(software.seenInNet))
            {
                software.seenInNet = "";
            }

            if (!string.IsNullOrEmpty(networkName))
            {
                if (!software.seenInNet.Contains(networkName))
                {
                    software.seenInNet += $",{networkName}";
                }
            }

            if (software.SoftwareType.Equals("exploit"))
            {
                if (Exloits.ContainsKey(software.Code))
                    throw new ArgumentException($"Expolit #{software.Code} is duplicated in library.");

                Software.Add(software);
                Exloits.Add(software.Code, software);
            }
            else if (software.SoftwareType.Equals("defence"))
            {
                if (Defences.ContainsKey(software.Code))
                    throw new ArgumentException($"Defence #{software.Code} is duplicated in library.");

                Defences.Add(software.Code, software);
                Software.Add(software);
            }
            else
            {
                if (Unknown.ContainsKey(software.Code))
                    throw new ArgumentException($"Unknown soft #{software.Code} is duplicated in library.");

                Unknown.Add(software.Code, software);
                Software.Add(software);
            }

            if (All.ContainsKey(software.Code))
                throw new ArgumentException($"Software  #{software.Code} is duplicated in library.");

            All.Add(software.Code, software);

            

        }

        public void DumpToFile()
        {
            Serializer.SerializeAndDump(this, _fileName);
        }

        public SoftwareCheckResult CheckExploitCompatibility(long exploit, Node node)
        {

            SoftwareCheckResult result = SoftwareCheckResult.Invalid;

            Software libExploit;
            if (!Exloits.TryGetValue(exploit, out libExploit))
            {
                
                //just check numbers
                if (CheckAttackRule(exploit, node.Software))
                {
                    return SoftwareCheckResult.Unknown;
                }
                else
                {
                    return SoftwareCheckResult.Invalid;
                }
            }
            else
            {
                //check node types fit
                if (libExploit.NodeTypesString.Contains(node.NodeType))
                {
                    //just check numbers
                    if (CheckAttackRule(exploit, node.Software))
                    {
                        return SoftwareCheckResult.Valid;
                    }
                    else
                    {
                        return SoftwareCheckResult.Unknown;
                    }
                }
                else
                {
                    return SoftwareCheckResult.Invalid;
                }
            }
        }

        public bool CheckAttackRule(long attack, long defence)
        {
            return true;
        }

    }

    public class Software
    {
        //type='exploit' code='258' effect='get_admins' nodeType='[Administrative interface]'/>

        [XmlAttribute(AttributeName = "type")]
        public string SoftwareType { get; set; }

        [XmlAttribute(AttributeName = "code")]
        public string CodeStr { get; set; }

        [XmlIgnore]
        public long Code { get; set; }

        [XmlAttribute(AttributeName = "effect")]
        public string Effect { get; set; }

        [XmlAttribute(AttributeName = "inevitableEffect")]
        public string InevitableEffect { get; set; }

        [XmlAttribute(AttributeName = "nodeType")]
        public string NodeTypesString { get; set; }

        [XmlAttribute(AttributeName = "duration")]
        public int Duration { get; set; }

        [XmlAttribute(AttributeName = "seenInNet")]
        public string seenInNet { get; set; }

        [XmlAttribute(AttributeName = "divisor")]
        public long Divisor { get; set; }

        [XmlAttribute(AttributeName = "broken")]
        public bool Broken { get; set; }


    }
}
