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

        public void SetNetwork(Network network)
        {
            InitializeGraph(network);
        }

        public MainWindowViewModel()
        {
            _selectNodeCommand = new DelegateCommand<object>(OnSelectedNode);

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
            Graph = new GraphViewModel(true);

            Vertices = new List<NodeViewModel>();
            Edges = new List<NodeEdgeViewModel>();

            foreach (var node in net.Nodes)
            {
                var nodeVM = new NodeViewModel()
                {
                    Name = node.Value.Name,
                    Type = node.Value.NodeType,
                    Defense = node.Value.Software,
                    IsDisabled = (node.Value.Disabled > 0), //???
                    IsExplored = node.Value.Explored,
                    Effect = node.Value.Effect,
                    DisabledTimeLeft = TimeSpan.FromSeconds(node.Value.Disabled),
                    //LockedTimeLeft = TimeSpan.FromSeconds(node.Value.) TODO!!! отслеживание залоченных нод
                };

                Vertices.Add(nodeVM);
            }

            foreach (var node in net.Nodes)
            {
                
                    foreach (var subnode in node.Value.Links)
                    {
                        var n = Vertices.First(v => v.Name == node.Value.Name);
                        var sn = Vertices.First(v => v.Name == subnode.LinkedNode.Name);

                        Edges.Add(new NodeEdgeViewModel(node.Value.Name + "-" + subnode.LinkedNode.Name, n, sn));
                    }
            }

            Graph.AddVertexRange(Vertices);
            Graph.AddEdgeRange(Edges);
        }

        private void OnSelectedNode(object obj)
        {
            Vertices.ForEach(d => d.IsSelected = false);

            SelectedNode = obj as NodeViewModel;
            SelectedNode.IsSelected = true;
            NotifyPropertyChanged("SelectedNode");

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
