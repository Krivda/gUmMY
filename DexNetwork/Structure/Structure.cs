using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DexNetwork.Structure
{

    public class NodeInstance
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "nodeType")]
        public string NodeType { get; set; }
        [XmlAttribute(AttributeName = "disabled")]
        public int Disabled { get; set; }
        [XmlAttribute(AttributeName = "software")]
        public long Software { get; set; }
        [XmlAttribute(AttributeName = "explored")]
        public bool Explored { get; set; }

        [XmlArray("Subnodes")]
        [XmlArrayItem("Node", Type=typeof(NodeInstance))]
        public List<NodeInstance> Subnodes { get; set; }

        [XmlIgnore]
        public int Index { get; set; }

        [XmlIgnore]
        public NodeInstance Parent { get; set; }
        [XmlIgnore]
        public Node Node { get; set; }

        [XmlIgnore]
        public string NodeInstKey { get; set; }


    }

    [XmlRoot(ElementName = "Network")]
    public class Network
    {
        //[XmlElement(ElementName = "Root", Type = typeof(NodeInstance))]
        public NodeInstance Root { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlIgnore]
        public List<List<NodeInstance>> NodeInstByLevel {get; private set; }
        [XmlIgnore]
        //Todo alex: pure nodes (w/o duplicates)
        public Dictionary<string, Node> Nodes { get; private set; }


        public void MakeTreeLike()
        {
            NodeInstByLevel = new List<List<NodeInstance>>();
            Nodes =new Dictionary<string, Node>();

            ProccessInstance(Root, null, 0);

        }

        private void ProccessInstance(NodeInstance nodeInstance, NodeInstance parent, int level)
        {

            if (!Nodes.TryGetValue(nodeInstance.Name, out Node node))
            {
                node = new Node();
                Nodes.Add(nodeInstance.Name, node);
            }

            nodeInstance.Parent = parent;
            nodeInstance.Node = node;

            string instKey;
            int nodeIndex=0;
            if (parent == null)
            {
                instKey = $"root_{nodeInstance.Name}";
                nodeIndex = 0;
            }
            else
            {
                for (int i = 0; i < nodeInstance.Parent.Subnodes.Count; i++)
                {
                    if (nodeInstance.Parent.Subnodes[i].Name.Equals(nodeInstance.Name))
                    {
                        nodeIndex = i;
                        break;
                    }
                }
                instKey = $"{parent.Name}_{nodeInstance.Name}";
            }

            nodeInstance.NodeInstKey = instKey;
            nodeInstance.Index = nodeIndex;

            if (NodeInstByLevel.Count == level)
                NodeInstByLevel.Add(new List<NodeInstance>());

            NodeInstByLevel[level].Add(nodeInstance);
            node.AddInstance(nodeInstance);

            if (nodeInstance.Subnodes == null)
            {
                nodeInstance.Subnodes = new List<NodeInstance>();
            }
            
            
            foreach (var childInstance in nodeInstance.Subnodes)
            {
                ProccessInstance(childInstance, nodeInstance, level + 1);
            }
        }
    }

    public class Node
    {
        public string Name { get; set; }
        public string NodeType { get; set; }
        public int Disabled { get; set; }
        public long Software { get; set; }
        public bool Explored { get; set; }

        public List<NodeInstance> Instances { get; } = new List<NodeInstance>();
        private bool _initialized = false;

        public void AddInstance(NodeInstance inst)
        {

            bool exists = false;
            foreach (var nodeInstance in Instances)
            {
                if (nodeInstance.NodeInstKey.Equals(inst.NodeInstKey))
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
                Instances.Add(inst);

            if (!_initialized)
            {
                Name = inst.Name;
                NodeType = inst.Name;

                Disabled = inst.Disabled;

                Software = inst.Software;

                
                Explored = inst.Explored;
            }
            else
            {
                bool updateInstances = false;
                if (Name != inst.Name)
                    throw new Exception($"Node instance name '{inst.Name}' doesn't math node name {Name}");

                if (NodeType != inst.NodeType)
                    throw new Exception($"Node instance type '{inst.NodeType}' doesn't math node name {NodeType}");

                if (Software != inst.Software)
                    updateInstances = true;

                Software = inst.Software;

                Disabled = inst.Disabled;

                if (!Explored)
                    Explored = inst.Explored;

            }
        }

    }
}
