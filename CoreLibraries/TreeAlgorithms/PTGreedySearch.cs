// -----------------------------------------------------------------------
// <copyright file="PTGreedySearch.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.TreeAlgorithms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Positioning;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PTGreedySearch : PTDepthFirstSearch
    {
         private readonly INetworkDataProvider provider;

        public PTGreedySearch(int depth, bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode goal)
            : base(depth,bidirectional, provider, origin, goal)
        {
            this.provider = provider;
        }

        public PTGreedySearch(bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode destination)
            : base(bidirectional,provider,origin, destination)
        {
            this.provider = provider;
        }

        
        protected override INetworkNode[] OrderChildren(INetworkNode[] nodes)
        {
           

            foreach (var node in nodes)
            {
                node.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)node, (Location)this.Destination);
                /*
                INetworkNode otherCurrent = this.current[threadId == 0 ? 1 : 0];
                node.EuclidianDistance = this.Bidirectional && otherCurrent != null ?
                    GeometryHelper.GetStraightLineDistance((Location)node, (Location)otherCurrent) : 
                    GeometryHelper.GetStraightLineDistance((Location)node, (Location)this.Destination);
                 */
            }

            //Array.Sort(nodes, new NodeComparer());
            nodes.StochasticSort(Entropy);
            
            return nodes;
        }

    }
}
