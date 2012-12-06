// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PTUniformCostSearch.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.TreeAlgorithms
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    #endregion

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PTUniformCostSearch : DepthFirstSearch<INetworkNode>
    {
        #region Constants and Fields

        /// <summary>
        ///   The departure time.
        /// </summary>
        private readonly DateTime departureTime = DateTime.Now;

        /// <summary>
        ///   The provider.
        /// </summary>
        private readonly INetworkDataProvider provider;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PTUniformCostSearch"/> class.
        /// </summary>
        /// <param name="depth">
        /// The depth.
        /// </param>
        /// <param name="bidirectional">
        /// The bidirectional.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="goal">
        /// The goal.
        /// </param>
        public PTUniformCostSearch(
            int depth, bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode goal)
            : base(depth, bidirectional, origin, goal)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PTUniformCostSearch"/> class.
        /// </summary>
        /// <param name="bidirectional">
        /// The bidirectional.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        public PTUniformCostSearch(
            bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode destination)
            : base(bidirectional, origin, destination)
        {
            this.provider = provider;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get children.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// </returns>
        protected override NodeWrapper<INetworkNode>[] GetChildren(NodeWrapper<INetworkNode> node)
        {
            if (node.CurrentRoute >= 20000)
            {
                return new NodeWrapper<INetworkNode>[0];
            }

            List<INetworkNode> nodes = this.provider.GetAdjacentNodes(node.Node);

            var wrappers = new NodeWrapper<INetworkNode>[nodes.Count];

            for (int i = 0; i < nodes.Count; i++)
            {
                wrappers[i] = new NodeWrapper<INetworkNode>(nodes[i]) { CurrentRoute = node.CurrentRoute + 1 };
            }

            return wrappers;
        }

        /// <summary>
        /// The order children.
        /// </summary>
        /// <param name="nodes">
        /// The nodes.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
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

        #endregion
    }
}