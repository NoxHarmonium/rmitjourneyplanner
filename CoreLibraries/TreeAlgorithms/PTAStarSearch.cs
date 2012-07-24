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
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.Types;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PTAStarSearch : PTDepthFirstSearch
    {
        private readonly INetworkDataProvider provider;

        private readonly EvolutionaryProperties properties;

        public PTAStarSearch(int depth, bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode goal)
            : base(depth, bidirectional, provider, origin, goal)
        {
           
            this.provider = provider;
        }

        public PTAStarSearch(bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode destination)
            : base(bidirectional, provider, origin, destination)
        {
           
            this.provider = provider;
        }

        public PTAStarSearch(EvolutionaryProperties properties, bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode destination)
            : base(bidirectional, provider, origin, destination)
        {
           
            this.provider = provider;
            this.properties = properties;
            
        }


        protected override NodeWrapper<INetworkNode>[] OrderChildren(NodeWrapper<INetworkNode>[] nodes)
        {
            //nodes = base.OrderChildren(nodes);

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

                    //wrapper.Cost = GeometryHelper.GetStraightLineDistance((Location)this.current[CurrentIndex].Node, (Location)wrapper.Node);

                   

                }
                else
                {
                    wrapper.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)wrapper.Node, (Location)this.Destination);
                    //wrapper.Cost = GeometryHelper.GetStraightLineDistance((Location)this.current[CurrentIndex].Node, (Location)wrapper.Node);
                   
                }

                if (!provider.GetRoutesForNode(current[CurrentIndex].Node).Intersect(provider.GetRoutesForNode(wrapper.Node)).Any())
                {
                    wrapper.Cost += 10;
                }
                

            }

            //Array.Sort(nodes, new NodeComparer());
            //Array.Reverse(nodes);
            nodes.StochasticSort(Entropy);

            return nodes;
        }
    }
}
