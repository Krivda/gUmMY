using System.Collections.Generic;
using System.Xml.Serialization;

namespace DexNetwork.Structure
{
    [XmlRoot(ElementName = "SoftwareLib")]
    public class SoftwareLib
    {
        [XmlArray("Exploits")]
        [XmlArrayItem("software", Type = typeof(Software))]
        public List<Software> Exploits { get; set; }

        [XmlArray("Defence")]
        [XmlArrayItem("software", Type = typeof(Software))]
        public List<Software> Defence { get; set; }
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
