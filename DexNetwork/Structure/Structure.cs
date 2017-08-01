using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace DexNetwork.Structure
{

    public class NodeInstance
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "nodeType")]
        public string NodeType { get; set; }

        [XmlAttribute(AttributeName = "effect")]
        public string Effect { get; set; }

        [XmlIgnore]
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
        [XmlIgnore]
        private string _networkFileName;

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

            if (Root != null)
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

        public void Init(string networkFileName)
        {
            _networkFileName = networkFileName;
            MakeTreeLike();
        }

        public void Dump()
        {
            Serializer.SerializeNetAndDump(this, _networkFileName);
        }

        public List<List<string>> GetStrPathsToRoot(Node node)
        {
            var strPaths = GetPathToNodeReverse(Nodes["firewall"], new List<string>() { node.Name });

            return strPaths;
        }

        public List<List<Node>> GetNodePathsToRoot(Node node)
        {
            var strPaths = GetStrPathsToRoot(node);

            return StringPathsToNodePaths(strPaths);
        }

        private List<List<Node>> StringPathsToNodePaths(List<List<string>> strPaths)
        {
            List<List<Node>> result = new List<List<Node>>();

            foreach (var strPath in strPaths)
            {
                List<Node> rootPath = new List<Node>();
                result.Add(rootPath);

                foreach (var nodeName in strPath)
                {
                    rootPath.Add(Nodes[nodeName]);
                }
            }

            return result;
        }

        private List<List<string>> GetPathToNodeReverse(Node target, List<string> currPath)
        {
            List<List<string>> paths = new List<List<string>>();
            Node node = Nodes[currPath[currPath.Count - 1]];

            Dictionary<string, Node> parentNodes = node.GetParents();

            foreach (var parent in parentNodes)
            {
                //ignore loops loop
                if (currPath.Contains(parent.Key))
                {
                    // do nothing
                }
                else
                {
                    var newFork = new List<string>(currPath);
                    newFork.Add(parent.Key);

                    if (parent.Key != target.Name)
                    {
                        var forks = GetPathToNodeReverse(target, newFork);
                        foreach (var fork in forks)
                        {
                            if (fork.Count > 0 && fork[fork.Count - 1].Equals(target.Name))
                            {
                                paths.Add(fork);
                            }
                        }
                    }
                    else
                    {
                        //parent is a target: this is already a good path
                        paths.Add(newFork);
                    }

                }
            }
            return paths;
        }
    }

    public class Node
    {
        public string Name { get; set; }
        public string NodeType { get; set; }
        public int Disabled { get; set; }
        public long Software { get; set; }
        public bool Explored { get; set; }
        public string Effect { get; set; }

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
            {
                Instances.Add(inst);
            }
                

            if (!_initialized)
            {
                Name = inst.Name;
                NodeType = inst.Name;

                Disabled = inst.Disabled;

                Software = inst.Software;
                Effect = inst.Effect;
                
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

        public Dictionary<string, Node> GetParents()
        {
            Dictionary<string, Node> result = new Dictionary<string, Node>();

            foreach (var nodeInstance in Instances)
            {
                if (nodeInstance.Parent != null)
                {
                    // check if we already met this parent?
                    if (!result.ContainsKey(nodeInstance.Parent.Node.Name))
                    {
                        result.Add(nodeInstance.Parent.Node.Name, nodeInstance.Parent.Node);
                    }
                }
            }

            return result;
        }

        public Dictionary<string, Node> GetChildren()
        {
            Dictionary<string, Node> result = new Dictionary<string, Node>();

            foreach (var nodeInstance in Instances)
            {
                if (nodeInstance.Subnodes != null)
                {
                    foreach (var subnode in nodeInstance.Subnodes)
                    {
                        // check if we already met this parent?
                        if (!result.ContainsKey(subnode.Node.Name))
                        {
                            result.Add(subnode.Node.Name, subnode.Node);
                        }
                    }
                }
            }

            return result;
        }


    }
}
