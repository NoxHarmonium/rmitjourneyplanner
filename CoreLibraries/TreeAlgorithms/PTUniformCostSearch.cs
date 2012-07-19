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

        public PTUniformCostSearch(bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode destination)
            : base(bidirectional,origin, destination)
        {
            this.provider = provider;
        }

        protected override NodeWrapper<INetworkNode>[] GetChildren(INetworkNode node)
        {
            List<INetworkNode> nodes = provider.GetAdjacentNodes(node);

            var wrappers = new NodeWrapper<INetworkNode>[nodes.Count];

            for (int i = 0; i < nodes.Count; i++)
            {
                wrappers[i] = new NodeWrapper<INetworkNode>(nodes[i]);

            }

            return wrappers;
        }

        protected override NodeWrapper<INetworkNode>[] OrderChildren(NodeWrapper<INetworkNode>[] nodes)
        {
            /*
            foreach (var node in nodes )
            {
              
                   //List<Arc> arcs = provider.GetDistanceBetweenNodes(current, node, departureTime);
                    //arcs.Sort(new ArcComparer());
                    //node.RouteId = arcs[0].RouteId;
                    //node.TotalTime = arcs[0].Time;
                
                
                //node.TotalTime = provider.GetDistanceBetweenNodes(current,node,departureTime + current.TotalTime).
                
                
                double distance = GeometryHelper.GetStraightLineDistance((Location)current[0].Node, (Location)node);
               if (current[0].Node.TransportType == node.TransportType)
               {
                   switch (current[0].Node.TransportType)
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
         

            //Array.Sort(nodes,new NodeComparer());
            nodes.StochasticSort(0.5);
            return nodes;
             * */
            throw new NotImplementedException();
        }
    }
}
