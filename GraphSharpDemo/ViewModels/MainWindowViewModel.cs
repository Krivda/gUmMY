using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using GraphSharp.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DexNetwork.Structure;
using DexNetwork;

namespace GraphSharpDemo.ViewModels
{
    public class GraphViewModelLayout : GraphLayout<NodeViewModel, NodeEdgeViewModel, GraphViewModel> { }



    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Data

        private string layoutAlgorithmType;
        private GraphViewModel graph;
        private List<String> layoutAlgorithmTypes = new List<string>();
        private int count;
        private DelegateCommand<object> _selectNodeCommand;

        public NodeViewModel SelectedNode { get; set; }

        public ICommand SelectNodeCommand { get { return _selectNodeCommand; } }


        public List<NodeViewModel> Vertices { get; set; }

        public List<NodeEdgeViewModel> Edges { get; set; }

        #endregion

        #region Ctor
        public MainWindowViewModel()
        {
            string netName = "BlackMirror11";
            string path = $@"Networks/{netName}.xml";

            Network net = Serializer.Deserialize(path);

            InitializeGraph(net);






            _selectNodeCommand = new DelegateCommand<object>(OnSelectedNode);

            Graph = new GraphViewModel(true);

            InitializeGraph(net);
            Graph.AddVertexRange(Vertices);
            Graph.AddEdgeRange(Edges);


            //Add Layout Algorithm Types
            layoutAlgorithmTypes.Add("BoundedFR");
            layoutAlgorithmTypes.Add("Circular");
            layoutAlgorithmTypes.Add("CompoundFDP");
            layoutAlgorithmTypes.Add("EfficientSugiyama");
            layoutAlgorithmTypes.Add("FR");
            layoutAlgorithmTypes.Add("ISOM");
            layoutAlgorithmTypes.Add("KK");
            layoutAlgorithmTypes.Add("LinLog");
            layoutAlgorithmTypes.Add("Tree");

            //Pick a default Layout Algorithm Type
            LayoutAlgorithmType = "LinLog";
        }

        private void InitializeGraph(Network net)
        {
            Vertices = new List<NodeViewModel>();
            Edges = new List<NodeEdgeViewModel>();

            foreach (var node in net.Nodes)
            {
                var nodeVM = new NodeViewModel()
                {
                    Name = node.Value.Name,
                    Type = node.Value.NodeType,
                    Defense = int.Parse(node.Value.Software.Trim(new[] {' ', '#'})),
                    IsDisabled = (node.Value.Disabled == 1), //???
                    IsExplored = node.Value.Explored,
                };

                Vertices.Add(nodeVM);
            }

            foreach (var node in net.Nodes)
            {
                foreach (var inst in node.Value.Instances)
                {
                    foreach (var subnode in inst.Subnodes)
                    {
                        var n = Vertices.First(v => v.Name == node.Value.Name);
                        var sn = Vertices.First(v => v.Name == subnode.Name);

                        Edges.Add(new NodeEdgeViewModel(node.Value.Name + "-" + subnode.Name, n, sn));
                    }
                }
            }
        }

        //private void processNode(Node node)
        //{
        //    var nvm = new NodeViewModel();

        //    nvm.Name = node.Name;
        //    nvm.Type = node.NodeType;



        //    foreach (var instances in node.Instances)
        //    {
        //        Vertices.Add(new NodeViewModel()
        //        { }
        //        );


        //    }

        //}






        private void InitializeGraph()
        {
            Vertices = new List<NodeViewModel>();
            Vertices.Add(new NodeViewModel()
            {
                Name = "firewall",
                AttackCodes = new ObservableCollection<int>() { 180, 256, 578, 3456 },
                Defense = 327546823,
                DefenseInfo = "Effect: disable \n Allowed node types: \n -Firewall \n - Antivirus \n - VPN \n - Brandmauer \n - Router \n - Traffic monitor \n - Cyptographic system \n Duration: 600sec.",
                DefenseLog = "2067-07-14 22:56:37: WARNING! Unautorized access to ManInBlack/system_information. Information was copied by unknown user! \n" +
                             "2067 - 07 - 14 22:55:48: Analyze effect worked: \n" +
                             "2067 - 07 - 14 22:55:48: WARNING!Attack attempt detected on ManInBlack / cryptocore1.Hostile program code '#396' \n" +
                             "2067 - 07 - 14 22:55:48: Trace effect worked: \n" +
                             "2067 - 07 - 14 22:55:48: WARNING!Attack attempt detected from address citizen5984@california, on ManInBlack / cryptocore1 \n" +
                             "2067 - 07 - 14 22:55:48: Analyze effect worked:",
                DisabledTimeLeft = TimeSpan.FromSeconds(500),
                IsDisabled = true,
                IsExplored = true,
                IsLocked = false,
                LockedTimeLeft = TimeSpan.FromSeconds(0),
                Type = "Firewall"
            });

            Vertices.Add(new NodeViewModel()
            {
                Name = "antivirus1",
                AttackCodes = new ObservableCollection<int>() { 14, 767, 1578, 8346 },
                Defense = 1231111113,
                DefenseInfo = "Effect: disable \n Allowed node types: \n -Firewall \n - Antivirus \n - VPN \n - Brandmauer \n - Router \n - Traffic monitor \n - Cyptographic system \n Duration: 600sec.",
                DefenseLog = "2067-07-14 22:56:37: WARNING! Unautorized access to ManInBlack/system_information. Information was copied by unknown user! \n" +
                             "2067 - 07 - 14 22:55:48: Analyze effect worked: \n" +
                             "2067 - 07 - 14 22:55:48: WARNING!Attack attempt detected on ManInBlack / cryptocore1.Hostile program code '#396' \n" +
                             "2067 - 07 - 14 22:55:48: Trace effect worked: \n" +
                             "2067 - 07 - 14 22:55:48: WARNING!Attack attempt detected from address citizen5984@california, on ManInBlack / cryptocore1 \n" +
                             "2067 - 07 - 14 22:55:48: Analyze effect worked:",
                DisabledTimeLeft = TimeSpan.FromSeconds(450),
                IsDisabled = true,
                IsExplored = true,
                IsLocked = false,
                LockedTimeLeft = TimeSpan.FromSeconds(0),
                Type = "Antivirus"
            });

            Vertices.Add(new NodeViewModel()
            {
                Name = "antivirus2",
                AttackCodes = new ObservableCollection<int>() { 223, 455, 765, 12333 },
                Defense = 77644423,
                DefenseInfo = "Effect: disable \n Allowed node types: \n -Firewall \n - Antivirus \n - VPN \n - Brandmauer \n - Router \n - Traffic monitor \n - Cyptographic system \n Duration: 600sec.",
                DefenseLog = "2067-07-14 22:56:37: WARNING! Unautorized access to ManInBlack/system_information. Information was copied by unknown user! \n" +
                             "2067 - 07 - 14 22:55:48: Analyze effect worked: \n" +
                             "2067 - 07 - 14 22:55:48: WARNING!Attack attempt detected on ManInBlack / cryptocore1.Hostile program code '#396' \n" +
                             "2067 - 07 - 14 22:55:48: Trace effect worked: \n" +
                             "2067 - 07 - 14 22:55:48: WARNING!Attack attempt detected from address citizen5984@california, on ManInBlack / cryptocore1 \n" +
                             "2067 - 07 - 14 22:55:48: Analyze effect worked:",
                DisabledTimeLeft = TimeSpan.FromSeconds(0),
                IsDisabled = false,
                IsExplored = false,
                IsLocked = false,
                LockedTimeLeft = TimeSpan.FromSeconds(0),
                Type = "Antivirus"
            });



            //foreach (NodeViewModel vertex in ExistingVertices)
            //    Graph.AddVertex(vertex);

            Edges = new List<NodeEdgeViewModel>();

            Edges.Add(AddNewGraphEdge(Vertices[0], Vertices[1]));
            Edges.Add(AddNewGraphEdge(Vertices[0], Vertices[2]));
            Edges.Add(AddNewGraphEdge(Vertices[1], Vertices[2]));
        }

        private void OnSelectedNode(object obj)
        {
            SelectedNode = obj as NodeViewModel;
            NotifyPropertyChanged("SelectedNode");
            //Vertices.ForEach(d => d.IsSelected = false);
            //Graph.Clear();
            //SelectedNode.IsSelected = true;
            //Graph.AddVertexRange(Vertices);
            //Graph.AddEdgeRange(Edges);
        }

        #endregion


        #region Private Methods
        private NodeEdgeViewModel AddNewGraphEdge(NodeViewModel from, NodeViewModel to)
        {
            string edgeString = string.Format("{0}-{1} Connected", from.Name, to.Name);
            NodeEdgeViewModel newEdge = new NodeEdgeViewModel(edgeString, from, to);
            return newEdge;
        }


        #endregion

        #region Public Properties

        public List<String> LayoutAlgorithmTypes
        {
            get { return layoutAlgorithmTypes; }
        }


        public string LayoutAlgorithmType
        {
            get { return layoutAlgorithmType; }
            set
            {
                layoutAlgorithmType = value;
                NotifyPropertyChanged("LayoutAlgorithmType");
            }
        }



        public GraphViewModel Graph
        {
            get { return graph; }
            set
            {
                graph = value;
                NotifyPropertyChanged("Graph");
            }
        }
        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}
