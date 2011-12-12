// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="RandomRouteGenerator.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Generates routes between 2 points.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.RouteGenerators
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Generates routes between 2 points.
    /// </summary>
    public class RandomRouteGenerator : IRouteGenerator
    {
        #region Constants and Fields

        /// <summary>
        ///   The properties of the current route planner.
        /// </summary>
        private readonly EvolutionaryProperties properties;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomRouteGenerator"/> class.
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        public RandomRouteGenerator(EvolutionaryProperties properties)
        {
            this.properties = properties;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generates a random route between source and destination nodes.
        /// </summary>
        /// <param name="source">
        /// The source node.
        /// </param>
        /// <param name="destination">
        /// The destination node.
        /// </param>
        /// <returns>
        /// </returns>
        public Route Generate(INetworkNode source, INetworkNode destination)
        {
            Random random = new Random();
            INetworkNode current = source;
            Route currentRoute = new Route(Guid.NewGuid().ToString());
            currentRoute.AddNode(source, true);

            while (!current.Equals(destination))
            {
                double minDistance = double.MaxValue;
                INetworkNode minNode = null;
                double maxWalkDistance = this.properties.MaximumWalkDistance;

                var candidateNodes = this.properties.NetworkDataProviders[0].GetNodesAtLocation(
                    (Location)source, maxWalkDistance);

                if (
                    this.properties.PointDataProviders[0].EstimateDistance((Location)current, (Location)destination).
                        Distance < maxWalkDistance)
                {
                    candidateNodes.Add(destination);
                }

                if (candidateNodes.Count == 0)
                {
                    throw new Exception("This node has no neighbors.");
                }

                if (!(current is TerminalNode))
                {
                    candidateNodes.AddRange(
                        this.properties.NetworkDataProviders[0].GetAdjacentNodes(current, current.CurrentRoute));
                }

                var transferNodes = new List<INetworkNode>();

                List<INetworkNode> tempCandidates = new List<INetworkNode>(candidateNodes);
                candidateNodes.Clear();
                candidateNodes.AddRange(
                    tempCandidates.Where(candidateNode => !currentRoute.GetNodes(true).Contains(candidateNode)));

                if (candidateNodes.Count == 0)
                {
                    candidateNodes.Add(destination);
                }

                // Calculate minimum distance node
                foreach (var candidateNode in candidateNodes)
                {
                    candidateNode.RetrieveData();
                    candidateNode.EuclidianDistance =
                        this.properties.PointDataProviders[0].EstimateDistance(
                            (Location)candidateNode, (Location)destination).Distance;
                    if (candidateNode.EuclidianDistance < minDistance)
                    {
                        minNode = candidateNode;
                        minDistance = candidateNode.EuclidianDistance;
                    }
                }

                candidateNodes.Remove(minNode);

                // Find nodes that are a transfer.
                foreach (var candidateNode in candidateNodes)
                {
                    if (!(current is TerminalNode))
                    {
                        if (candidateNode.CurrentRoute != current.CurrentRoute)
                        {
                            transferNodes.Add(candidateNode);
                        }
                    }
                }

                candidateNodes.RemoveAll(transferNodes.Contains);

                double p = random.NextDouble();

                if (p <= this.properties.ProbMinDistance || (transferNodes.Count == 0 && candidateNodes.Count == 0))
                {
                    /*
                    if (minNode == null)
                    {
                        //throw new NullReferenceException("There are no routes left to take!");
                        minNode = destination;
                    }
                     * */
                    currentRoute.AddNode(minNode, true);

                    current = minNode;
                }
                else if ((p <= this.properties.ProbMinDistance + this.properties.ProbMinTransfers
                          || candidateNodes.Count == 0) && transferNodes.Count != 0)
                {
                    int index = random.Next(0, transferNodes.Count - 1);
                    currentRoute.AddNode(transferNodes[index], true);
                    current = transferNodes[index];
                }
                else
                {
                    int index = random.Next(0, candidateNodes.Count - 1);
                    currentRoute.AddNode(candidateNodes[index], true);
                    current = candidateNodes[index];
                }
            }

            return currentRoute;
        }

        #endregion
    }
}