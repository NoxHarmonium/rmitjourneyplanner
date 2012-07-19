// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Mutators
{
    #region

    using System;
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   Mutates a critter by selecting 2 random points a generating a new path between them.
    /// </summary>
    public class StandardMutator : IMutator
    {
        #region Constants and Fields

        /// <summary>
        ///   The properties of the route planner.
        /// </summary>
        private readonly EvolutionaryProperties properties;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="StandardMutator" /> class.
        /// </summary>
        /// <param name="properties"> The properties. </param>
        public StandardMutator(EvolutionaryProperties properties)
        {
            this.properties = properties;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Mutates a critter
        /// </summary>
        /// <param name="child"> The critter that is to be mutated. </param>
        /// <returns> A mutated critter. </returns>
        public Critter Mutate(Critter child)
        {
            var random = new Random();
            List<NodeWrapper<INetworkNode>> nodes = child.Route;
            if (nodes.Count == 1) return child;
            int startIndex = random.Next(0, nodes.Count - 2);
            int endIndex = random.Next(startIndex + 1, nodes.Count - 1);
            NodeWrapper<INetworkNode> begin = nodes[startIndex];
            NodeWrapper<INetworkNode> end = nodes[endIndex];
            Route newSegment = this.properties.RouteGenerator.Generate(
                begin.Node, end.Node, this.properties.DepartureTime + begin.TotalTime);
            var newRoute = new Route(Guid.NewGuid().GetHashCode());
            newRoute.AddRange(nodes.GetRange(0, startIndex));
            newRoute.AddRange(newSegment);
            newRoute.AddRange(nodes.GetRange(endIndex + 1, nodes.Count - 1 - endIndex));

            return new Critter((Route)newRoute.Clone(),new Fitness());

            // return child;
        }

        #endregion
    }
}