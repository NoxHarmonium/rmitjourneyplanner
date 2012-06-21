// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.RouteGenerators
{
    #region

    using System;
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.Comparers;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   Generates routes between 2 points.
    /// </summary>
    public class OptimisedRouteGenerator : IRouteGenerator
    {
        #region Constants and Fields

        /// <summary>
        ///   The properties of the current route planner.
        /// </summary>
        private readonly EvolutionaryProperties properties;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="OptimisedRouteGenerator" /> class. Initializes a new instance of the <see
        ///    cref="RandomRouteGenerator" /> class.
        /// </summary>
        /// <param name="properties"> The properties. </param>
        public OptimisedRouteGenerator(EvolutionaryProperties properties)
        {
            this.properties = properties;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Generates a random route between source and destination nodes.
        /// </summary>
        /// <param name="source"> The source node. </param>
        /// <param name="destination"> The destination node. </param>
        /// <param name="startTime"> </param>
        /// <returns> </returns>
        public Route Generate(INetworkNode source, INetworkNode destination, DateTime startTime)
        {
            var random = new Random();
            INetworkNode current = source;
            var currentRoute = new Route(Guid.NewGuid().GetHashCode()) { source };
            INetworkDataProvider nProvider = this.properties.NetworkDataProviders[0];
            IPointDataProvider pProvider = this.properties.PointDataProviders[0];
            DateTime currentTime = this.properties.DepartureTime;

            while (!current.Equals(destination))
            {
                INetworkNode node;
                double maxWalkDistance = this.properties.MaximumWalkDistance;

                double p = random.NextDouble();

                if (p <= this.properties.ProbMinDistance)
                {
                    p = random.NextDouble();

                    if (p >= this.properties.ProbMinTransfers)
                    {
                        node = nProvider.GetNodeClosestToPointWithinArea(current, destination, maxWalkDistance, true);
                    }
                    else
                    {
                        node = nProvider.GetNodeClosestToPoint(destination, current.CurrentRoute)
                               ??
                               (nProvider.GetNodeClosestToPointWithinArea(current, destination, maxWalkDistance, false)
                                ??
                                nProvider.GetNodeClosestToPointWithinArea(current, destination, maxWalkDistance, true));
                    }
                }
                else
                {
                    p = random.NextDouble();
                    if (p <= this.properties.ProbMinTransfers || current.CurrentRoute == -1)
                    {
                        List<INetworkNode> nodes = nProvider.GetNodesAtLocation((Location)current, maxWalkDistance);
                        node = nodes.Count == 0 ? null : nodes[random.Next(nodes.Count - 1)];
                    }
                    else
                    {
                        List<INetworkNode> nodes = nProvider.GetAdjacentNodes(current, current.CurrentRoute);
                        if (nodes.Count == 0)
                        {
                            nodes = nProvider.GetNodesAtLocation((Location)current, maxWalkDistance);
                        }

                        node = nodes[random.Next(nodes.Count - 1)];
                    }
                }

                if (node == null)
                {
                    node = destination;
                }

                // if (currentRoute.GetNodes(true).Contains(node))
                // {
                // continue;
                // }
                node.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)node, (Location)destination);

                List<Arc> arcs = nProvider.GetDistanceBetweenNodes(current, node, currentTime);
                arcs.Add(pProvider.EstimateDistance((Location)current, (Location)node));
                arcs.Add(pProvider.EstimateDistance((Location)current, (Location)destination));
                arcs.Sort(new ArcComparer());

                node = (INetworkNode)arcs[0].Destination;
                node.TransportType = arcs[0].TransportMode;
                node.TotalTime = current.TotalTime + arcs[0].Time.TotalTime;
                node.Parent = current;
                currentRoute.Add(node);
                current = node;
            }

            return currentRoute;
        }

        #endregion
    }
}