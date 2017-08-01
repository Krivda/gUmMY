using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DexNetwork.Structure
{

    [XmlRoot(ElementName = "NodeEffectsLib")]
    public class NodeEffectsLib
    {
        private string _fileName;

        [XmlArray("NodeEffects")]
        [XmlArrayItem("NodeEffect", Type = typeof(NodeEffect))]
        public List<NodeEffect> NodeEffectsList { get; set; }

        [XmlIgnore]
        public Dictionary<string, NodeEffect> NodeEffects { get; set; } = new Dictionary<string, NodeEffect>();

        public void Init(string fileName)
        {
            _fileName = fileName;

            StringBuilder errors = new StringBuilder();
            foreach (var nodeEffect in NodeEffectsList)
            {
                try
                {
                    AddNewEffect(nodeEffect);
                }
                catch (ArgumentException e)
                {
                    errors.AppendLine(e.Message);
                }
            }

            if (!string.IsNullOrEmpty(errors.ToString()))
            {
                throw new Exception($"Can't load node effects lib from {_fileName}: \n{errors}");
            }
        }

        public void AddNewEffect(NodeEffect effect)
        {

            if (NodeEffects.ContainsKey(effect.Name))
                throw new ArgumentException($"Effect  '{effect.Name}' is duplicated in library.");

            NodeEffects.Add(effect.Name, effect);
        }

        public void DumpToFile()
        {
            Serializer.SerializeAndDump(this, _fileName);
        }
    }

    public class NodeEffect
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "description")]
        public string Effect { get; set; }
    }
}
