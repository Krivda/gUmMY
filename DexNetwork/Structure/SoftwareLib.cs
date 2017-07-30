using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DexNetwork.Structure
{

    [XmlRoot(ElementName = "SoftwareLib")]
    public class SoftwareLib
    {
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
                    AddNewSoft(software);
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

        public void AddNewSoft(Software software)
        {
            if (software.SoftwareType.Equals("exploit"))
            {
                if (Exloits.ContainsKey(software.Code))
                    throw new ArgumentException($"Expolit #{software.Code} is duplicated in library.");
                    
                Exloits.Add(software.Code, software);
            }
            else if (software.SoftwareType.Equals("defence"))
            {
                if (Defences.ContainsKey(software.Code))
                    throw new ArgumentException($"Defence #{software.Code} is duplicated in library.");

                Defences.Add(software.Code, software);
            }
            else
            {
                if (Unknown.ContainsKey(software.Code))
                    throw new ArgumentException($"Unknown soft #{software.Code} is duplicated in library.");

                Unknown.Add(software.Code, software);
            }

            if (Unknown.ContainsKey(software.Code))
                throw new ArgumentException($"Softwere  #{software.Code} is duplicated in library.");

            All.Add(software.Code, software);
        }

        public void DumpToFile()
        {
            Serializer.SerializeNetAndDump(this, _fileName);
        }
    }

    public class Software
    {
        //type='exploit' code='258' effect='get_admins' nodeType='[Administrative interface]'/>

        [XmlAttribute(AttributeName = "type")]
        public string SoftwareType { get; set; }

        [XmlAttribute(AttributeName = "code")]
        public int Code { get; set; }

        [XmlAttribute(AttributeName = "effect")]
        public string Effect { get; set; }

        [XmlAttribute(AttributeName = "inevitableEffect")]
        public string InevitableEffect { get; set; }

        [XmlAttribute(AttributeName = "nodeType")]
        public string NodeTypesString { get; set; }

        [XmlAttribute(AttributeName = "duration")]
        public int Duration { get; set; }

    }
}
