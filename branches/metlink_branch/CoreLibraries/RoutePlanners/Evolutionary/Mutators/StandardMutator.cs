// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="StandardMutator.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Mutates a critter by selecting 2 random points a generating a new
//   path between them.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Mutators
{
    #region

    using System;
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Mutates a critter by selecting 2 random points a generating a new 
    ///   path between them.
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
        /// Initializes a new instance of the <see cref="StandardMutator"/> class.
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        public StandardMutator(EvolutionaryProperties properties)
        {
            this.properties = properties;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Mutates a critter
        /// </summary>
        /// <param name="child">
        /// The critter that is to be mutated.
        /// </param>
        /// <returns>
        /// A mutated critter.
        /// </returns>
        public Critter Mutate(Critter child)
        {
            Random random = new Random();
            List<INetworkNode> nodes = child.Route.GetNodes(true);
            int startIndex = random.Next(0, nodes.Count - 2);
            int endIndex = random.Next(startIndex + 1, nodes.Count - 1);
            INetworkNode begin = nodes[startIndex];
            INetworkNode end = nodes[endIndex];
            Route newSegment = this.properties.RouteGenerator.Generate(begin, end);
            Route newRoute = new Route(Guid.NewGuid().ToString());
            newRoute.AddNodeRange(nodes.GetRange(0, startIndex), true);
            newRoute.AddNodeRange(newSegment.GetNodes(true), true);
            newRoute.AddNodeRange(nodes.GetRange(endIndex + 1, nodes.Count - 1 - endIndex), true);

            return new Critter(newRoute, this.properties.FitnessFunction.GetFitness(newRoute));
        }

        #endregion
    }
}