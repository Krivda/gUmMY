using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DexNetwork.Structure
{

    /*public class NodeInstance
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
    }*/

    [XmlRoot(ElementName = "Network")]
    public class Network
    {
        [XmlIgnore]
        private string _networkFileName;

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlArray("Nodes")]
        [XmlArrayItem("Node", Type = typeof(Node))]
        public List<Node> NodesList { get; set; }

        [XmlIgnore]
        public List<List<Node>> NodesByLevel {get; private set; }
        [XmlIgnore]

        public Dictionary<string, Node> Nodes { get; private set; }

        private void ProccessNode(Node node,  int level)
        {
            //first time met a new level
            if (!(level < NodesByLevel.Count))
            {
                NodesByLevel.Add(new List<Node>());
            }


            if (!NodesByLevel[level].Contains(node))
            {
                NodesByLevel[level].Add(node);
            }


            int index = 0;
            foreach (var childLink in node.Links)
            {
                Node childNode;
                
                //check if child is a valid node
                if (!Nodes.TryGetValue(childLink.To, out childNode))
                    throw new Exception($"Node {node.Name} has a link to a non-existing node named {childLink.To} ");

                childLink.LinkedNode = childNode;
                childNode.Index = index;
                index++;

                ProccessNode(childNode, level + 1);
            }
        }

        public void Init(string networkFileName)
        {
            _networkFileName = networkFileName;

            NodesByLevel = new List<List<Node>>();
            Nodes = new Dictionary<string, Node>();

            foreach (var node in NodesList)
            {
                string nodeNotUniqueErr = "These nodes are not unique";
                bool errFound = false;
                if (Nodes.ContainsKey(node.Name))
                {
                    nodeNotUniqueErr += $"Node {node.Name} is not unique";
                    errFound = true;
                }
                else
                {
                    Nodes.Add(node.Name, node);
                    node.Network = this;
                }

                if (errFound)
                {
                    throw new Exception(nodeNotUniqueErr);
                }
            }

            Node root;
            if (!Nodes.TryGetValue("firewall", out root))
                throw new Exception("Network doesn't have firewall node");
            
            
            ProccessNode(root, 0);
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
                    var newFork = new List<string>(currPath) {parent.Key};

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

    public class Link
    {
        [XmlAttribute(AttributeName = "to")]
        public string To { get; set; }

        [XmlIgnore]
        public Node LinkedNode { get; set; }

    }

    public class Node
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "nodeType")]
        public string NodeType { get; set; }
        [XmlAttribute(AttributeName = "effect")]
        public string Effect { get; set; }
        [XmlAttribute(AttributeName = "software")]
        public long Software { get; set; }
        [XmlAttribute(AttributeName = "explored")]
        public bool Explored { get; set; }

        [XmlIgnore]
        public int Index { get; set; }

        [XmlIgnore]
        public int Disabled { get; set; }

        [XmlIgnore]
        public Network Network { get; set; }

        [XmlArray("Links")]
        [XmlArrayItem("Link", Type = typeof(Link))]
        public List<Link> Links { get; set; }


        private bool _initialized = false;

        public Dictionary<string, Node> GetParents()
        {
            Dictionary<string, Node> result = new Dictionary<string, Node>();

            foreach (var node in Network.NodesList)
            {
                foreach (var nodeLink in node.Links)
                {
                    if (nodeLink.To.Equals(Name))
                    {
                        result.Add(node.Name, node);
                    }
                }
            }
            return result;
        }

        public Dictionary<string, Node> GetChildren()
        {
            Dictionary<string, Node> result = new Dictionary<string, Node>();

            foreach (var link in Links)
            {
                result.Add(link.LinkedNode.Name, link.LinkedNode);
            }

            return result;
        }


    }
}
