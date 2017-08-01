using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DexNetwork.Structure
{
    [XmlRoot(ElementName = "NodeTypesLib")]
    public class NodeTypesLib
    {
        private string _fileName;

        [XmlArray("NodeEffects")]
        [XmlArrayItem("NodeEffect", Type = typeof(NodeEffect))]
        public List<NodeEffect> NodeTypesList { get; set; }


        [XmlIgnore]
        public static Dictionary<string, NodeType> NodeTypes { get; } = new Dictionary<string, NodeType>();

        public class NodeTypeDescription
        {
            public String FullName { get; }
            public String Abbrivation { get; }

            public NodeTypeDescription(string fullName, string abbrivation)
            {
                FullName = fullName;
                Abbrivation = abbrivation;
            }
        }
    }

    public class NodeType
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "description")]
        public string Effect { get; set; }

        [XmlAttribute(AttributeName = "abbrivation")]
        public String Abbrivation { get; }
    }
}
