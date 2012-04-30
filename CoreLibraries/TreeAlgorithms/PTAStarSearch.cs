// -----------------------------------------------------------------------
// <copyright file="PTAStarSearch.cs" company="">
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
    public class PTAStarSearch : DepthFirstSearch<INetworkNode>
    {
        private readonly INetworkDataProvider provider;

        public PTAStarSearch(int depth, bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode goal)
            : base(depth, bidirectional, origin, goal)
        {
            this.provider = provider;
        }

        public PTAStarSearch(bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode goal)
            : base(bidirectional,origin, goal)
        {
            this.provider = provider;
        }

        protected override INetworkNode[] GetChildren(INetworkNode node)
        {
            List<INetworkNode> nodes = provider.GetAdjacentNodes(node);

            return nodes.ToArray();
        }

        protected override INetworkNode[] OrderChildren(INetworkNode[] nodes)
        {

            foreach (var node in nodes )
            {
                node.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)node, (Location)this.Goal);
            }
            
            Array.Sort(nodes,new NodeComparer());
            return nodes;
        }
    }
}
