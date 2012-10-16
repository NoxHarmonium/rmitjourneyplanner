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
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
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
            var target = (Location)(CurrentIndex == 0 ? this.Destination : this.Origin);

            foreach (var wrapper in nodes)
            {

                wrapper.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)wrapper.Node, target);
                //wrapper.EuclidianDistance = 1;
                if (((MetlinkDataProvider)provider).RoutesIntersect(wrapper.Node, this.current[0].Node))
                {
                   //wrapper.EuclidianDistance *= 0.5; //best
                    wrapper.EuclidianDistance *= 0.5;

                }
                   
                

                

            }

            //Array.Sort(nodes, new NodeComparer());
            //Array.Reverse(nodes);
            nodes.StochasticSort(Entropy);

            return nodes.Reverse().ToArray();
        }
    }
}
