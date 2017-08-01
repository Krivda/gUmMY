using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DexNetwork.Structure;

namespace UnitTests
{
    public static class NetGenerator
    {

        public static Network GetNet(string netName)
        {
            Network net = null;
            switch (netName)
            {
                case "linked":
                    net = GetNetA();
                    break;
                case "simple":
                    net = GetSimpleNet();
                    break;
                case "complicated":
                    net = GetComplatedNet();
                    break;
            }

            return net;
        }


        private static Network GetNetA()
        {
            Network net = null;
            net = new Network();
            net.NodesList = new List<Node>()
            {
                new Node()
                {
                    Name = "firewall",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "brandmauer1"
                        },
                        new Link()
                        {
                            To = "brandmauer2"
                        },
                    }
                },
                new Node()
                {
                    Name = "brandmauer1",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "VPN1"
                        },
                        new Link()
                        {
                            To = "VPN2"
                        },
                        new Link()
                        {
                            To = "VPN3"
                        },
                    }
                },
                new Node()
                {
                    Name = "VPN1",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "brandmauer2"
                        },
                    }
                },
                new Node()
                {
                    Name = "VPN2",
                    Links = new List<Link>()
                },
                new Node()
                {
                    Name = "VPN3",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "antivirus1"
                        },
                    }
                },
                new Node()
                {
                    Name = "antivirus1",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "datanode1"
                        },
                    }
                },
                new Node()
                {
                    Name = "brandmauer2",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "VPN3"
                        },
                        new Link()
                        {
                            To = "VPN4"
                        },
                    }
                },
                new Node()
                {
                    Name = "VPN4",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "antivirus2"
                        },
                    }
                },
                new Node()
                {
                    Name = "antivirus2",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "datanode1"
                        },
                    }
                },
                new Node()
                {
                    Name = "datanode1",
                    Links = new List<Link>()
                },
            };
            net.Init("");

            return net;
        }

        private static Network GetSimpleNet()
        {
            Network net = new Network();

            net.NodesList = new List<Node>()
            {
                new Node()
                {
                    Name = "firewall",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "VPN"
                        },
                        new Link()
                        {
                            To = "Monitor1"
                        }
                    }
                },

                new Node()
                {
                    Name = "VPN",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "Router1"
                        },
                        new Link()
                        {
                            To = "Monitor3"
                        }
                    }
                },
                new Node()
                {
                    Name = "Monitor1",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "VPN2"
                        },
                        new Link()
                        {
                            To = "Monitor4"
                        }
                    }
                },
                new Node()
                {
                    Name = "Router1",
                    Links = new List<Link>()
                },
                new Node()
                {
                    Name = "Monitor3",
                    Links = new List<Link>()
                },
                new Node()
                {
                    Name = "VPN2",
                    Links = new List<Link>()
                },
                new Node()
                {
                    Name = "Monitor4",
                    Links = new List<Link>()
                },
            };
            net.Init("testfile");
            return net;
        }

        private static Network GetComplatedNet()
        {
            Network net = new Network();
            net.NodesList = new List<Node>()
            {
                new Node()
                {
                    Name = "firewall",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "VPN1"
                        },
                        new Link()
                        {
                            To = "Monitor1"
                        },
                    }
                },
                new Node()
                {
                    Name = "VPN1",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "Router1"
                        },
                        new Link()
                        {
                            To = "Monitor3"
                        },
                    }
                },
                new Node()
                {
                    Name = "Router1",
                    Links = new List<Link>()
                },
                new Node()
                {
                    Name = "Monitor3",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "cryptore1"
                        },
                        new Link()
                        {
                            To = "data core1"
                        },
                        new Link()
                        {
                            To = "data core2"
                        },
                    }
                },
                new Node()
                {
                    Name = "cryptore1",
                    Links = new List<Link>()
                },
                new Node()
                {
                    Name = "data core1",
                    Links = new List<Link>()
                },
                new Node()
                {
                    Name = "data core2",
                    Links = new List<Link>()
                },
                new Node()
                {
                    Name = "Monitor1",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "VPN2"
                        },
                        new Link()
                        {
                            To = "Monitor4"
                        },
                    }
                },
                new Node()
                {
                    Name = "VPN2",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "VPN3"
                        },
                        new Link()
                        {
                            To = "Traffic2"
                        },
                        new Link()
                        {
                            To = "cryptocore3"
                        },
                    }
                },
                new Node()
                {
                    Name = "VPN3",
                    Links = new List<Link>()
                },
                new Node()
                {
                    Name = "Traffic2",
                    Links = new List<Link>()
                },
                new Node()
                {
                    Name = "cryptocore3",
                    Links = new List<Link>()
                },
                new Node()
                {
                    Name = "Monitor4",
                    Links = new List<Link>()
                    {
                        new Link()
                        {
                            To = "Antivirus1"
                        },
                    }
                },
                new Node()
                {
                    Name = "Antivirus1",
                    Links = new List<Link>()
                },
            };
            net.Init("");
            return net;
        }
    }
}
