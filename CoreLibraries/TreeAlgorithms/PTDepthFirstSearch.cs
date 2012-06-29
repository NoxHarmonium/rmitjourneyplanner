// -----------------------------------------------------------------------
// <copyright file="PTDepthFirstSearch.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.TreeAlgorithms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using RmitJourneyPlanner.CoreLibraries.Comparers;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Positioning;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PTDepthFirstSearch : DepthFirstSearch<INetworkNode>
    {
        private readonly INetworkDataProvider provider;



        public PTDepthFirstSearch(int depth, bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode goal)
            : base(depth, bidirectional, origin, goal)
        {
            this.provider = provider;
        }

        public PTDepthFirstSearch(bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode destination)
            : base(bidirectional,origin, destination)
        {
            this.provider = provider;
        }

        protected override NodeWrapper<INetworkNode>[] GetChildren(INetworkNode node)
        {
            List<INetworkNode> nodes = provider.GetAdjacentNodes(node);
            
            var wrappers = new NodeWrapper<INetworkNode>[nodes.Count];

            for (int i = 0; i < nodes.Count; i++ )
            {
                wrappers[i] = new NodeWrapper<INetworkNode>(nodes[i]);

            }
            
            return wrappers;
        }

        protected override NodeWrapper<INetworkNode>[] OrderChildren(NodeWrapper<INetworkNode>[] nodes)
        {
            var oldNodes = new List<NodeWrapper<INetworkNode>>(nodes);

            
            nodes.StochasticSort(Entropy);

            if (oldNodes.Any(node => !nodes.Contains(node)) || nodes.Any(node => !oldNodes.Contains(node)))
            {
                throw new Exception("Nodes are not consistant after sort.");
            }

            return nodes;
        }
    }
}
