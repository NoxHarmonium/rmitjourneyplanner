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
            : base(bidirectional,provider,origin, destination)
        {
            this.provider = provider;
        }

       
        protected override INetworkNode[] OrderChildren(INetworkNode[] nodes)
        {
            //nodes = base.OrderChildren(nodes);

            foreach (var node in nodes)
            {
                if (this.Bidirectional)
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

               double distance = GeometryHelper.GetStraightLineDistance((Location)current[0].Node, (Location)node);
               if (current[CurrentIndex].Node.TransportType == node.TransportType)
               {
                   switch (current[CurrentIndex].Node.TransportType)
                   {
                       case "Train":
                           node.TotalTime = TimeSpan.FromHours(distance / 60);
                           break;

                       case "Bus":
                           node.TotalTime = TimeSpan.FromHours(distance / 30);
                           break;

                       case "Tram":
                           node.TotalTime = TimeSpan.FromHours(distance / 40);
                           break;

                       case "V/Line Coach":
                       case "Regional Bus":
                             node.TotalTime = TimeSpan.FromHours(distance / 50);
                           break;
                            break;


                       default:
                           Console.WriteLine("Unknown transport type: " + current[0].Node.TransportType);
                           node.TotalTime = TimeSpan.FromHours(9999);
                           break;
                   }

               }
               else
               {
                   node.TotalTime = TimeSpan.FromHours(distance / 2);
               }
               
            
            }

            //Array.Sort(nodes, new NodeComparer());
            //Array.Reverse(nodes);
            nodes.StochasticSort(Entropy);
            
            return nodes;
        }
    }
}
