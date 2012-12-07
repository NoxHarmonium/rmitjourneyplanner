// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandardMutator.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Mutates a critter by selecting 2 random points a generating a new path between them.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Mutators
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
    using RmitJourneyPlanner.CoreLibraries.Types;

    using Random = RmitJourneyPlanner.CoreLibraries.Random;

    #endregion

    /// <summary>
    /// Mutates a critter by selecting 2 random points a generating a new path between them.
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

        #region Public Methods and Operators

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
            var rand = Random.GetInstance();
            double u1 = rand.NextDouble(); // these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // random no

            int crossoverWeight = (int)Tools.Clamp(10 * randStdNormal, -30, 30);

            child.DepartureTime = this.properties.DepartureTime.AddMinutes(crossoverWeight);
            Assert.That(child.DepartureTime != default(DateTime));

            var random = Random.GetInstance();
            List<NodeWrapper<INetworkNode>> nodes = child.Route;
            if (nodes.Count == 1)
            {
                return child;
            }

            int startIndex = random.Next(0, nodes.Count - 2);
            int endIndex = random.Next(startIndex + 1, nodes.Count - 1);
            NodeWrapper<INetworkNode> begin = nodes[startIndex];
            NodeWrapper<INetworkNode> end = nodes[endIndex];
            Route newSegment = this.properties.RouteGenerator.Generate(
                begin.Node, end.Node);
            var newRoute = new Route(Guid.NewGuid().GetHashCode());
            newRoute.AddRange(nodes.GetRange(0, startIndex));
            newRoute.AddRange(newSegment);
            newRoute.AddRange(nodes.GetRange(endIndex + 1, nodes.Count - 1 - endIndex));

            return new Critter((Route)newRoute.Clone(), new Fitness()) { DepartureTime = child.DepartureTime };

            // return child;
        }

        #endregion
    }
}