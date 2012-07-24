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

        
        protected override NodeWrapper<INetworkNode>[] OrderChildren(NodeWrapper<INetworkNode>[] nodes)
        {
           

            foreach (var wrapper in nodes)
            {
               
                if (Bidirectional)
                {
                    INetworkNode otherCurrent = this.current[CurrentIndex == 0 ? 1 : 0].Node;
                    wrapper.EuclidianDistance = this.Bidirectional && otherCurrent != null
                                                         ? GeometryHelper.GetStraightLineDistance(
                                                             (Location)wrapper.Node, (Location)otherCurrent)
                                                         : GeometryHelper.GetStraightLineDistance(
                                                             (Location)wrapper.Node, (Location)this.Destination);
                }
                else
                {
                    wrapper.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)wrapper.Node, (Location)this.Destination);
                }

                //if (wrapper.Node.TransportType == "Train")
                //{
                //    wrapper.EuclidianDistance /= 2.0;
               // }

            }

            //Array.Sort(nodes, new NodeComparer());
            nodes.StochasticSort(Entropy);

            return nodes;
            //return nodes.OrderBy(n => n.EuclidianDistance).Reverse().ToArray();
        }

    }
}
