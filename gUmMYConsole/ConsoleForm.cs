using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ConsoleStream;
using SRMatrixNetwork;


namespace gUmMYConsole
{
    public partial class ConsoleForm : Form
    {
        private readonly ConsoleStreamBase _consoleStream;


        public ConsoleForm()
        {
            InitializeComponent();

            _consoleStream = new MatrixProcessor(this);
            //_consoleStream = new MainCommandInterfaceStream(this);

            _consoleStream.OnCommandExecute += ConsoleStreamOnOnProcessInput;
        }

        private void ConsoleStreamOnOnProcessInput(object sender, ConsoleStreamEventArgs args)
        {
            txtStreamInput.Text += $"\n{args.Content}";

        }

        private void CmdSendFromApp_Click(object sender, EventArgs e)
        {
            if (_consoleStream is MainCommandInterfaceStream stream)
            {
                stream.FeedOutput(txtStreamOutput.Text);
            }
        }

        private void ConsoleForm_Load(object sender, EventArgs e)
        {
            consConsole.ConsoleStream = _consoleStream;
            //_consoleStream.StartFeed();
            consConsole.IsInputEnabled = true;
        }

        private void ChkInputable_CheckedChanged(object sender, EventArgs e)
        {
            consConsole.IsInputEnabled = chkInputable.Checked;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int selstart = int.Parse(txtStreamInput.Text);

            string s =
                $"prefix text [color,{Color.Aqua.ToArgb()}: aqua text] other text [color,{Color.Lime.ToArgb()}: lime] of ending.";

            consConsole.InternalRichTextBox.SelectionStart = selstart;
            consConsole.InternalRichTextBox.SelectionLength = consConsole.InternalRichTextBox.Text.Length;
            consConsole.InternalRichTextBox.SelectedText = "";

            //consConsole.Test(0, 1);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            lbDebg.Text = $"SelStrt={consConsole.InternalRichTextBox.SelectionStart}, SelLen={consConsole.InternalRichTextBox.SelectionLength}, ps={consConsole.PromptStart}, is={consConsole.InputStart}, os={consConsole.OutputStart}";
        }

        private void CmdClear_Click(object sender, EventArgs e)
        {
            consConsole.ClearOutput();
            consConsole.Prompt = "";
        }


        private void CmdDumpNet_Click(object sender, EventArgs e)
        {
            //stub for commented out method
            throw new NotImplementedException();
        }
        /*private void CmdDumpNet_Click(object sender, EventArgs e)
        {
            Network net = new Network();

            net.Name = "BlackMirror11";/*
            net.Root = new NodeInstance()
            {
                Disabled = 0,
                Explored = true,
                Name = "firewall",
                NodeType = "Firewall",
                Software = 4000000,
                Subnodes = new List<NodeInstance>
                {
                    new NodeInstance
                    {
                        //012087000: antivirus1 (Antivirus): 1208700  
                        Disabled = 0,
                        Explored = true,
                        Name = "antivirus1",
                        NodeType = "Antivirus",
                        Software = 1208700,
                        Subnodes = new List<NodeInstance>()
                        {
                            new NodeInstance
                            {
                                //0: brandmauer2 (Brandmauer): 1559025
                                Disabled = 0,
                                Explored = true,
                                Name = "brandmauer2",
                                NodeType = "Brandmauer",
                                Software = 1208700,
                                Subnodes = new List<NodeInstance>()
                                {
                                    new NodeInstance
                                    {
                                        //0: VPN1 (VPN): 7684425
                                        Disabled = 0,
                                        Explored = true,
                                        Name = "VPN1",
                                        NodeType = "VPN",
                                        Software = 7684425,
                                        Subnodes = new List<NodeInstance>()
                                        {
                                            new NodeInstance
                                            {
                                                //0: antivirus3 (Antivirus): 16354975
                                                Disabled = 0,
                                                Explored = true,
                                                Name = "antivirus3",
                                                NodeType = "Antivirus",
                                                Software = 16354975,
                                                Subnodes = new List<NodeInstance>()
                                                {
                                                    new NodeInstance
                                                    {
                                                        //0: system_information (Data): 151335275
                                                        Disabled = 0,
                                                        Explored = true,
                                                        Name = "system_information",
                                                        NodeType = "Data",
                                                        Software = 151335275,
                                                        Subnodes = new List<NodeInstance>()
                                                    },
                                                    new NodeInstance
                                                    {
                                                        //1: router1 (Router): 432225
                                                        Disabled = 0,
                                                        Explored = true,
                                                        Name = "router1",
                                                        NodeType = "Router",
                                                        Software = 432225,
                                                        Subnodes = new List<NodeInstance>()
                                                    }
                                                }
                                            },
                                            new NodeInstance
                                            {
                                                //1: traffic_monitor1 (Traffic monitor): 1019788 847
                                                Disabled = 0,
                                                Explored = true,
                                                Name = "traffic_monitor1",
                                                NodeType = "Traffic monitor",
                                                Software = 1019788,
                                                Subnodes = new List<NodeInstance>()
                                            }
                                        }
                                    },
                                    new NodeInstance
                                    {
                                        //1: cryptocore2 (Cyptographic system): 36900
                                        Disabled = 0,
                                        Explored = true,
                                        Name = "cryptocore2",
                                        NodeType = "Cyptographic system",
                                        Software = 36900,
                                        Subnodes = new List<NodeInstance>()
                                        {
                                            new NodeInstance
                                            {
                                                //0: router1 (Router): 432225
                                                Disabled = 0,
                                                Explored = true,
                                                Name = "router1",
                                                NodeType = "Router",
                                                Software = 432225,
                                                Subnodes = new List<NodeInstance>()
                                                {
                                                    new NodeInstance()
                                                    {
                                                        //1: VPN4 (VPN): 2209900
                                                        Disabled = 0,
                                                        Explored = true,
                                                        Name = "VPN4",
                                                        NodeType = "VPN",
                                                        Software = 2209900,
                                                        Subnodes = new List<NodeInstance>()
                                                    }
                                                }
                                            },
                                            new NodeInstance
                                            {
                                                //1: cryptocore4 (Cyptographic system): 117468
                                                Disabled = 0,
                                                Explored = true,
                                                Name = "cryptocore4",
                                                NodeType = "Cyptographic system",
                                                Software = 432225,
                                                Subnodes = new List<NodeInstance>()
                                            },
                                            new NodeInstance
                                            {
                                                //2: router2 (Router): 2150775
                                                Disabled = 0,
                                                Explored = true,
                                                Name = "router2",
                                                NodeType = "Router",
                                                Software = 432225,
                                                Subnodes = new List<NodeInstance>()
                                            }
                                        }
                                    }
                                }
                            },
                            new NodeInstance
                            {
                                //1: VPN3 (VPN): 7993700
                                Disabled = 0,
                                
                                Explored = true,
                                Name = "VPN3",
                                NodeType = "VPN",
                                Software = 7993700,
                                Subnodes = new List<NodeInstance>()
                            }
                        }
                    },
                    new NodeInstance
                    {
                        //Node "BlackMirror11/antivirus2" properties:
                        //Installed program: 2739100
                        //Type: Antivirus
                        Disabled = 0,
                        
                        Explored = true,
                        Name = "antivirus2",
                        NodeType = "Antivirus",
                        Software = 2739100,
                        Subnodes = new List<NodeInstance>()
                        {
                            new NodeInstance
                            {
                                //0: VPN2 (VPN): 834372
                                Disabled = 0,
                                
                                Explored = true,
                                Name = "VPN2",
                                NodeType = "VPN",
                                Software = 834372,
                                Subnodes = new List<NodeInstance>()
                            },
                            new NodeInstance
                            {
                                //1: brandmauer3 (Brandmauer): 2294523
                                Disabled = 0,
                                
                                Explored = true,
                                Name = "brandmauer3",
                                NodeType = "Brandmauer",
                                Software = 2294523,
                                Subnodes = new List<NodeInstance>()
                            },
                        }
                    }
                }
            };
            
            string netName = "BlackMirror11";
            string path = $@"Networks/{netName}.xml";
            //_consoleStream.FeedOutput(Serializer.SerializeNetAndDump(net, path));
            //_consoleStream.FeedOutput($"\n Dumpted to {Path.GetFullPath(path)} ");


            net = Serializer.DeserializeNet(path);
        }*/

        private void cmdSoft_Click(object sender, EventArgs e)
        {
            Regex regex = new Regex("(#\\d+)", RegexOptions.Multiline);
            Match match = regex.Match(txtStreamOutput.Text);

            while (match.Success)
            {
                txtStreamInput.Text += $"\ninfo {match.Groups[0]} {txtStreamOutput.Lines[0]} {txtStreamOutput.Lines[1]}";
                match = match.NextMatch();
            }
        }

    }
}
