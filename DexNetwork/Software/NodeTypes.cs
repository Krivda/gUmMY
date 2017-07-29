using System;
using System.Collections.Generic;

namespace DexNetwork.Software
{
    public class NodeTypesList
    {
        public static Dictionary<string, NodeTypeDescription> NodeTypes { get; } = new Dictionary<string, NodeTypeDescription>();

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

        static NodeTypesList()
        {
            NodeTypes.Add("Administrative interface", new NodeTypeDescription("Administrative interface", "AD"));
            NodeTypes.Add("Firewall", new NodeTypeDescription("Firewall", "FW"));
            NodeTypes.Add("Antivirus", new NodeTypeDescription("Antivirus", "AV"));
            NodeTypes.Add("VPN", new NodeTypeDescription("VPN", "VN"));
            NodeTypes.Add("Brandmauer", new NodeTypeDescription("Brandmauer", "BM"));
            NodeTypes.Add("Router", new NodeTypeDescription("Router", "RT"));
            NodeTypes.Add("Traffic monitor", new NodeTypeDescription("Traffic monitor", "TM"));
            NodeTypes.Add("Cyptographic system", new NodeTypeDescription("Cyptographic system", "CR"));
            NodeTypes.Add("Data", new NodeTypeDescription("Data", "DA"));
            NodeTypes.Add("Bank account", new NodeTypeDescription("Bank account", "BA"));
            NodeTypes.Add("Finance", new NodeTypeDescription("Finance", "FI"));
            NodeTypes.Add("Corporate HQ", new NodeTypeDescription("Corporate HQ", "HQ"));
        }
    }
}
