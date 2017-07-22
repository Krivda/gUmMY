using System;
using System.Collections.Generic;
using System.Text;
using DexNetwork.Structure;

namespace gUmMYConsole
{
    public class NetTextMap
    {

        /// <summary>
        /// instance of the node (in tree representation each node may be drawn several times)
        /// </summary>
        public class TextNetNodeInstance
        {
            private readonly NodeInstance _node;

            public string LineId { get; }
 
            public TextNetNodeInstance(NodeInstance node, string lineId)
            {
                LineId = lineId;
                _node = node;
            }

            
        }

        public int MaxLevel { get; set; }
        List<string> _lines;
        public Dictionary<string, TextNetNodeInstance> _nodes;


        public Network Network { get; }

        private int _currentLineId;

        public NetTextMap(Network network)
        {
            Network = network;
        }


        public string GetTextView(string nodeFormat, int maxLen)
        {
            _currentLineId = 0;
            _lines = new List<string>();
            _nodes = new Dictionary<string, TextNetNodeInstance>();
            InsertLine(0);


            for (var level = 0; level < Network.NodeInstByLevel.Count; level++)
            {
                List<NodeInstance> levelNodesInstances = Network.NodeInstByLevel[level];
                foreach (var nodeInst in levelNodesInstances)
                {
                    DrawNode(3+maxLen*level + level, nodeFormat, maxLen, nodeInst);
                }
            }

            return GetLinesText();
        }


        private void DrawNode(int hpos, string nodeFormat, int maxLen, NodeInstance netNode)
        {
            int vpos = GetNodeInstLineIndex(netNode);
            string lineId = _lines[vpos].Substring(0, 3);
            TextNetNodeInstance textNode = new TextNetNodeInstance(netNode, lineId);
            _nodes.Add(netNode.NodeInstKey, textNode);

            string nodeText = GetNodeTextView(netNode, nodeFormat, maxLen);

            //draw this node text
            _lines[vpos] = _lines[vpos].ReplaceAt(hpos, maxLen, nodeText);

            List<string> connectorBlock = CreateBlock(netNode.Subnodes.Count);
            int middle = connectorBlock.Count / 2;
            InsertLinesAroundNode(vpos, middle);

            int newBaseNodeIndex = vpos + middle; 

            //draw connectors
            for (int i = 0; i < connectorBlock.Count; i++)
            {
                _lines[newBaseNodeIndex - middle + i] = _lines[newBaseNodeIndex - middle + i].ReplaceAt(hpos+maxLen, 1, connectorBlock[i]);
            }
            
        }

        private void InsertLinesAroundNode(int baseLine, int linesCount)
        {
            for (int i = 0; i < linesCount; i++)
            {
                InsertLine(baseLine + 1);
            }

            for (int i = 0; i < linesCount; i++)
            {
                InsertLine(baseLine);

            }
        }

        private string GetNodeTextView(NodeInstance netNode, string nodeFormat, int maxLen)
        {

            string formatString = $"{{0,-{maxLen}}}";

            string result = string.Format(formatString, netNode.Name);

            return result;
        }

        /// <summary>
        /// Creates connector block for the nodes list
        /// </summary>
        private List<string> CreateBlock(int nodeCount)
        {
            var result = new List<string>();

            //for each line in block
            for (int i = 0; i < nodeCount * 2 - 1; i++)
            {
                string prefix;
                //every odd is a node

                prefix = getPrefix(i, nodeCount);
                result.Add(prefix);

            }

            return result;
        }

        private string getPrefix(int index, int nodesCount)
        {

            //node connectors: top, bot, mid (odd), mid (even), single
            String nodeConnectors = "┌└├┼─";

            //line connectors: top, bot, mid (odd), mid (even)
            String lineConnectors = "  │┤";

            int lines = nodesCount * 2 - 1;
            string prefix;
            //every odd is a node
            string connectors = lineConnectors;
            if (index % 2 == 0)
            {
                connectors = nodeConnectors;
            }


            //if i is middle
            if (index == lines / 2)
            {
                if (lines == 1)
                    prefix = connectors[4].ToString();
                else
                    prefix = connectors[3].ToString();
            }
            //if i is top
            else if (index == 0)
            {
                prefix = connectors[0].ToString();
            }
            //if i is bot
            else if (index == (lines - 1))
            {
                prefix = connectors[1].ToString();
            }
            else
            {
                prefix = connectors[2].ToString();
            }

            return prefix;
        }

        private string InsertLine(int beforeIndex)
        {
            string lineBefore = beforeIndex-1 > -1 ? _lines[beforeIndex - 1] : "";

            string line = $"{++_currentLineId:D3}";

            if (!lineBefore.Equals(""))
            {
                for (int i = 0; i < lineBefore.Length; i++)
                {
                    int charCode = lineBefore[i];
                    if (charCode >= 0x2500 && charCode <= 0x257F && lineBefore[i] != '└')
                        line =line.ReplaceAt(i, 1, "│");
                }
            }

            _lines.Insert(beforeIndex, line);

            return line;
        }

        private int GetNodeInstLineIndex(NodeInstance nodeInst)
        {
            int result;
            if (nodeInst.Parent == null)
            {
                result = 0;
            }
            else
            {
                string lineId = _nodes[nodeInst.Parent.NodeInstKey].LineId;
                int parentLine = GetLineById(lineId);

                //result = parentLine + (nodeInst.Index * 2 - nodeInst.Parent.Subnodes.Count / 2);
                result = parentLine + (nodeInst.Index * 2 - (nodeInst.Parent.Subnodes.Count * 2 - 1) / 2);
            }

            return result;
        }

        private int GetLineById(string lineId)
        {
            for (var index = 0; index < _lines.Count; index++)
            {
                var line = _lines[index];
                if (line.StartsWith(lineId))
                    return index;
            }

            throw new Exception($"Line with id '{lineId}' not found");
        }

        private string GetLinesText()
        {
            StringBuilder linesBuf = new StringBuilder();
            foreach (var line in _lines)
            {
                linesBuf.AppendLine(line.Substring(3).TrimEnd(' '));
            }

            return linesBuf.ToString();
        }
    }
}