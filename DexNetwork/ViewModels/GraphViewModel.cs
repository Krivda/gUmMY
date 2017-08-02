using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace GraphSharpDemo.ViewModels
{
    public class GraphViewModel : BidirectionalGraph<NodeViewModel, NodeEdgeViewModel>
    {
        public GraphViewModel() { }

        public GraphViewModel(bool allowParallelEdges)
            : base(allowParallelEdges) { }

        public GraphViewModel(bool allowParallelEdges, int vertexCapacity)
            : base(allowParallelEdges, vertexCapacity) { }
    }
}
