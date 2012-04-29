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

        public PTDepthFirstSearch(int depth, INetworkDataProvider provider, INetworkNode origin, INetworkNode goal)
            : base(depth, origin, goal)
        {
            this.provider = provider;
        }

        public PTDepthFirstSearch(INetworkDataProvider provider, INetworkNode origin, INetworkNode goal)
            : base(origin, goal)
        {
            this.provider = provider;
        }

        protected override INetworkNode[] GetChildren(INetworkNode node)
        {
            List<INetworkNode> nodes = provider.GetAdjacentNodes(node);

            /*
             * Uncomment for greedy
             * 
            foreach (var child in nodes)
            {
                child.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)child, (Location)this.Goal);
            }
            nodes.Sort(new NodeComparer());
             * */

            return nodes.ToArray();
        }
    }
}
