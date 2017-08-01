using System.Collections.Generic;
using System.Text;
using DexNetwork.Structure;
using NUnit.Framework;

namespace UnitTests
{
    public class NetPathsTest
    {

        private Network GetNet(string netName)
        {
            Network net = null;
            if (netName.Equals("testNetA"))
            {
                net = new Network();
                net.Root = new NodeInstance()
                {
                    Name = "firewall",
                    Subnodes = new List<NodeInstance>()
                    {
                        new NodeInstance()
                        {
                            Name = "brandmauer1",
                            Subnodes = new List<NodeInstance>()
                            {
                                new NodeInstance()
                                {
                                    Name = "VPN1",
                                    Subnodes = new List<NodeInstance>()
                                    {
                                        new NodeInstance()
                                        {
                                            Name = "brandmauer2"
                                        }
                                    }
                                },
                                new NodeInstance()
                                {
                                    Name = "VPN2"
                                },
                                new NodeInstance()
                                {
                                    Name = "VPN3",
                                    Subnodes = new List<NodeInstance>()
                                    {
                                        new NodeInstance()
                                        {
                                            Name = "antivirus1",
                                            Subnodes = new List<NodeInstance>()
                                            {
                                                new NodeInstance()
                                                {
                                                    Name = "datanode1"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        new NodeInstance()
                        {
                            Name = "brandmauer2",
                            Subnodes = new List<NodeInstance>()
                            {
                                new NodeInstance()
                                {
                                    Name = "VPN3"
                                },
                                new NodeInstance()
                                {
                                    Name = "VPN4",
                                    Subnodes = new List<NodeInstance>()
                                    {
                                        new NodeInstance()
                                        {
                                            Name = "antivirus2",
                                            Subnodes = new List<NodeInstance>()
                                            {
                                                new NodeInstance()
                                                {
                                                    Name = "datanode1"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
                net.MakeTreeLike();
            }

            return net;
        }

        private string GetStrPath(Dictionary<string, Node> nodes)
        {
            string result="";
            string delim = "";

            foreach (var nodeKey in nodes.Keys)
            {
                result += $"{delim}{nodeKey}";
                delim = ",";
            }

            return result;
        }

        private bool KeysExists<T>(string keys, Dictionary<string, T> dic)
        {
            bool result = true;

            if (string.IsNullOrEmpty(keys) && dic.Count == 0)
                return true;

            foreach (var key in keys.Split(','))
            {
                if (!dic.ContainsKey(key))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }


        [TestCase(TestName = "get parent in net A")]
        public void CheckParents()
        {
            var net = GetNet("testNetA");

            Assert.IsTrue(KeysExists("", net.Nodes["firewall"].GetParents()));

            Assert.IsTrue(KeysExists("firewall", net.Nodes["brandmauer1"].GetParents()));
            Assert.IsTrue(KeysExists("firewall,VPN1", net.Nodes["brandmauer2"].GetParents()));

            Assert.IsTrue(KeysExists("brandmauer1", net.Nodes["VPN1"].GetParents()));
            Assert.IsTrue(KeysExists("brandmauer1", net.Nodes["VPN2"].GetParents()));
            Assert.IsTrue(KeysExists("brandmauer1,brandmauer2", net.Nodes["VPN3"].GetParents()));
            Assert.IsTrue(KeysExists("brandmauer2", net.Nodes["VPN4"].GetParents()));

            Assert.IsTrue(KeysExists("VPN3", net.Nodes["antivirus1"].GetParents()));
            Assert.IsTrue(KeysExists("VPN4", net.Nodes["antivirus2"].GetParents()));

            Assert.IsTrue(KeysExists("antivirus1,antivirus2", net.Nodes["datanode1"].GetParents()));
        }


        [TestCase(TestName = "path to root in net A")]
        public void TestPathInNetA()
        {
            var net = GetNet("testNetA");

            var paths = net.GetStrPathsToRoot(net.Nodes["brandmauer1"]);
            Assert.AreEqual(1, paths.Count);

            paths = net.GetStrPathsToRoot(net.Nodes["brandmauer2"]);
            Assert.AreEqual(2, paths.Count);

            paths = net.GetStrPathsToRoot(net.Nodes["VPN3"]);
            Assert.AreEqual(3, paths.Count);

            paths = net.GetStrPathsToRoot(net.Nodes["datanode1"]);
            Assert.AreEqual(5, paths.Count);
        }

    }
}