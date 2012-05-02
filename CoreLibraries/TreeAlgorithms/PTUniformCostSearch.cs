// -----------------------------------------------------------------------
// <copyright file="PTGreedySeach.cs" company="">
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
    using RmitJourneyPlanner.CoreLibraries.Types;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PTUniformCostSearch : DepthFirstSearch<INetworkNode>
    {
        private readonly INetworkDataProvider provider;

        
        private readonly DateTime departureTime = DateTime.Now;

        public PTUniformCostSearch(int depth, bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode goal)
            : base(depth, bidirectional, origin, goal)
        {
            this.provider = provider;
        }

        public PTUniformCostSearch(bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode goal)
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
              
                   //List<Arc> arcs = provider.GetDistanceBetweenNodes(current, node, departureTime);
                    //arcs.Sort(new ArcComparer());
                    //node.CurrentRoute = arcs[0].RouteId;
                    //node.TotalTime = arcs[0].Time;
                
                
                //node.TotalTime = provider.GetDistanceBetweenNodes(current,node,departureTime + current.TotalTime).
                
                
                double distance = GeometryHelper.GetStraightLineDistance((Location)current[threadId], (Location)node);
               if (current[threadId].TransportType == node.TransportType)
               {
                   switch (current[threadId].TransportType)
                   {
                       case "Train":
                           node.TotalTime = TimeSpan.FromHours(distance / 50);
                           break;

                       case "Bus":
                           node.TotalTime = TimeSpan.FromHours(distance / 30);
                           break;

                       case "Tram":
                           node.TotalTime = TimeSpan.FromHours(distance / 40);
                           break;

                       default:
                           Console.WriteLine("Unknown transport type: " + current[threadId].TransportType);
                           node.TotalTime = TimeSpan.FromHours(9999);
                           break;
                   }

               }
               else
               {
                   node.TotalTime = TimeSpan.FromHours(distance / 4);
               }
               
            }
            
            //Array.Sort(nodes,new NodeComparer());
            nodes.StochasticSort(0.5);
            return nodes;
        }
    }
}
