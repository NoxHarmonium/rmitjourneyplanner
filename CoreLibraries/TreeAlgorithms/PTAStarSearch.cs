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
    public class PTAStarSearch : PTUniformCostSearch
    {
        private readonly INetworkDataProvider provider;

        public PTAStarSearch(int depth, bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode goal)
            : base(depth, bidirectional, provider, origin, goal)
        {
            this.provider = provider;
        }

        public PTAStarSearch(bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode destination)
            : base(bidirectional,provider,origin, destination)
        {
            this.provider = provider;
        }

       
        protected override INetworkNode[] OrderChildren(INetworkNode[] nodes)
        {
            nodes = base.OrderChildren(nodes);

            foreach (var node in nodes)
            {
                if (Bidirectional)
                {
                    INetworkNode otherNode = this.current[CurrentIndex == 0 ? 1 : 0].Node;
                    node.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)node, (Location)otherNode);
                }
                else
                {

                    node.EuclidianDistance = GeometryHelper.GetStraightLineDistance(
                        (Location)node, (Location)this.Destination);
                }
                /*
                node.EuclidianDistance = this.Bidirectional ? 
                    GeometryHelper.GetStraightLineDistance((Location)node, (Location)this.current[threadId == 0 ? 1 : 0]) : 
                    GeometryHelper.GetStraightLineDistance((Location)node, (Location)this.Destination);
                * */
            }

            //Array.Sort(nodes, new NodeComparer());
            //Array.Reverse(nodes);
            nodes.StochasticSort(Entropy);
            
            return nodes;
        }
    }
}
