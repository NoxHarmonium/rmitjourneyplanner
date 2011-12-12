﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="EvolutionaryProperties.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Contains the properties related to one instance of an <see cref="EvolutionaryRoutePlanner" />.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region

    using System;
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    #endregion



    /// <summary>
    /// Contains the properties related to one instance of an <see cref="EvolutionaryRoutePlanner"/>.
    /// </summary>
    public class EvolutionaryProperties
    {
        
        public EvolutionaryProperties()
        {
            this.NetworkDataProviders = new List<INetworkDataProvider>();
            this.PointDataProviders = new List<IPointDataProvider>();
        }
        
        #region Public Properties

        /// <summary>
        ///   Gets or sets the breeder object used when crossing over routes.
        /// </summary>
        public IBreeder Breeder { get; set; }

        /// <summary>
        ///   Gets or sets the percentage of the population that will be crossed over on
        ///   every iteration.
        /// </summary>
        public double CrossoverRate { get; set; }

        /// <summary>
        ///   Gets or sets the departure time of the route planner.
        /// </summary>
        public DateTime DepartureTime { get; set; }

        /// <summary>
        ///   Gets or sets the destination of the route.
        /// </summary>
        public INetworkNode Destination { get; set; }

        /// <summary>
        ///   Gets or sets the fitness function used to evaluate routes.
        /// </summary>
        public IFitnessFunction FitnessFunction { get; set; }

        /// <summary>
        ///   Gets or sets the maximum distance that can be walked.
        /// </summary>
        public double MaximumWalkDistance { get; set; }

        /// <summary>
        ///   Gets or sets the percentage of critters that are mutated on every iteration.
        /// </summary>
        public double MutationRate { get; set; }

        /// <summary>
        ///   Gets or sets the mutator object used when mutating routes.
        /// </summary>
        public IMutator Mutator { get; set; }

        /// <summary>
        ///   Gets or sets a list of network data providers used to solve the route.
        /// </summary>
        public List<INetworkDataProvider> NetworkDataProviders { get; set; }

        /// <summary>
        ///   Gets or sets number of the population to keep and use in the breeding process.
        /// </summary>
        public int NumberToKeep { get; set; }

        /// <summary>
        ///   Gets or sets the origin of the route.
        /// </summary>
        public INetworkNode Origin { get; set; }

        /// <summary>
        ///   Gets or sets a list of point to point data providers used to solve the route.
        /// </summary>
        public List<IPointDataProvider> PointDataProviders { get; set; }

        /// <summary>
        ///   Gets or sets the population size of the route planner.
        /// </summary>
        public int PopulationSize { get; set; }

        /// <summary>
        ///   Gets or sets the probabiliy that a node that is closest to the goal will be chosen
        ///   when initalizing routes.
        /// </summary>
        public double ProbMinDistance { get; set; }

        /// <summary>
        ///   Gets or sets probability that a node of a different route will be chosen
        ///   when initilizing routes.
        /// </summary>
        public double ProbMinTransfers { get; set; }

        /// <summary>
        ///   Gets or sets the object used to build routes between 2 nodes.
        /// </summary>
        public IRouteGenerator RouteGenerator { get; set; }

        #endregion
    }
}