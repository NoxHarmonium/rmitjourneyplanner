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

        protected override INetworkNode[] GetChildren(INetworkNode node)
        {
            List<INetworkNode> nodes = provider.GetAdjacentNodes(node);
            /*
            INetworkNode[] clonedNodes = new INetworkNode[nodes.Count];
            for (int i = 0; i < clonedNodes.Length; i++)
            {
                clonedNodes[i] = (INetworkNode) nodes[i].Clone();
            }
            */
                /*
                 * Uncomment for greedy
                 * 
                foreach (var child in nodes)
                {
                    child.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)child, (Location)this.Destination);
                }
                nodes.Sort(new NodeComparer());
                 * */

                return nodes.ToArray();
        }

        protected override INetworkNode[] OrderChildren(INetworkNode[] nodes)
        {
            var oldNodes = new List<INetworkNode>(nodes);

            
            nodes.StochasticSort(Entropy);

            foreach (var node in oldNodes)
            {
                if (!nodes.Contains(node))
                {
                    throw new Exception("Nodes are not consistant after sort.");
                }

            }

            return nodes;
        }
    }
}
