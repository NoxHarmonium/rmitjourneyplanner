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
    public class PTAStarSearch : PTDepthFirstSearch
    {
        private readonly INetworkDataProvider provider;

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


        protected override NodeWrapper<INetworkNode>[] OrderChildren(NodeWrapper<INetworkNode>[] nodes)
        {
            //nodes = base.OrderChildren(nodes);

            foreach (var wrapper in nodes)
            {
                if (this.Bidirectional)
                {
                    INetworkNode otherNode = this.current[CurrentIndex == 0 ? 1 : 0].Node;
                    wrapper.Node.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)wrapper.Node, (Location)otherNode);
                }
                else
                {

                    wrapper.Node.EuclidianDistance = GeometryHelper.GetStraightLineDistance(
                        (Location)wrapper.Node, (Location)this.Destination);
                }
                /*
                node.EuclidianDistance = this.Bidirectional ? 
                    GeometryHelper.GetStraightLineDistance((Location)node, (Location)this.current[threadId == 0 ? 1 : 0]) : 
                    GeometryHelper.GetStraightLineDistance((Location)node, (Location)this.Destination);
                 * 
                */

                
                double distance = GeometryHelper.GetStraightLineDistance((Location)current[CurrentIndex].Node, (Location)wrapper.Node);
                

                if (current[CurrentIndex].Node.TransportType != wrapper.Node.TransportType)
                {
                    distance *= 2;
                }
                /*
                else
                {
                    switch (current[CurrentIndex].Node.TransportType)
                    {
                        case "Train":
                            distance /= 60;
                            break;
                        case "Bus":
                            distance /= 30;
                            break;
                           
                        case "Tram" :
                            distance /= 40;
                            break;




                    }

               

                }
                 */
                wrapper.Cost = distance;// +current[CurrentIndex].Cost;
                //wrapper.Cost = current[CurrentIndex].Cost;





            }

            //Array.Sort(nodes, new NodeComparer());
            //Array.Reverse(nodes);
            nodes.StochasticSort(Entropy);

            return nodes;
        }
    }
}
