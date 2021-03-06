﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvolutionaryProperties.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Contains the properties related to a journey planning run.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary
{
    #region Using Directives

    using System;

    using RmitJourneyPlanner.CoreLibraries.DataAccess;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.JourneyPlanners;
    using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary.RouteGenerators;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Contains the properties related to a journey planning run.
    /// </summary>
    public class EvolutionaryProperties : ICloneable
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "EvolutionaryProperties" /> class.
        /// </summary>
        public EvolutionaryProperties()
        {
            this.NetworkDataProviders = new INetworkDataProvider[0]; // new List<INetworkDataProvider>();
            this.PointDataProviders = new IPointDataProvider[0];
            this.Objectives = new FitnessParameter[0];
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the breeder object used when crossing over routes.
        /// </summary>
        public IBreeder Breeder { get; set; }

        /// <summary>
        ///   Gets or sets the percentage of the population that will be crossed over on every iteration.
        /// </summary>
        public double CrossoverRate { get; set; }

        /// <summary>
        ///   Gets or sets Database.
        /// </summary>
        public MySqlDatabase Database { get; set; }

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
        ///   Gets or sets the maximum number of iterations that the journey optimisation can run for.
        /// </summary>
        public int MaxIterations { get; set; }

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
        public INetworkDataProvider[] NetworkDataProviders { get; set; }

        /// <summary>
        ///   Gets or sets the objectives used in the optimisation.
        /// </summary>
        public FitnessParameter[] Objectives { get; set; }

        /// <summary>
        ///   Gets or sets the origin of the route.
        /// </summary>
        public INetworkNode Origin { get; set; }

        /// <summary>
        ///   Gets or sets the route planner used to optimise the journey.
        /// </summary>
        public IJourneyPlanner Planner { get; set; }

        /// <summary>
        ///   Gets or sets a list of point to point data providers used to solve the route.
        /// </summary>
        public IPointDataProvider[] PointDataProviders { get; set; }

        /// <summary>
        ///   Gets or sets the population size of the route planner.
        /// </summary>
        public int PopulationSize { get; set; }

        /// <summary>
        ///   Gets or sets the object used to build routes between 2 nodes.
        /// </summary>
        public IRouteGenerator RouteGenerator { get; set; }

        /// <summary>
        ///   Gets or sets the type of the search.
        /// </summary>
        /// <value>
        ///   The type of the search.
        /// </value>
        public SearchType SearchType { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public object Clone()
        {
            return new EvolutionaryProperties
                {
                    Breeder = this.Breeder, 
                    CrossoverRate = this.CrossoverRate, 
                    Database = this.Database, 
                    DepartureTime = this.DepartureTime, 
                    Destination = this.Destination, 
                    FitnessFunction = this.FitnessFunction, 
                    MutationRate = this.MutationRate, 
                    Mutator = this.Mutator, 
                    NetworkDataProviders = this.NetworkDataProviders, 
                    Objectives = this.Objectives, 
                    Origin = this.Origin, 
                    Planner = this.Planner, 
                    PointDataProviders = this.PointDataProviders, 
                    PopulationSize = this.PopulationSize, 
                    RouteGenerator = this.RouteGenerator, 
                    SearchType = this.SearchType, 
                    MaxIterations = this.MaxIterations, 
                };
        }

        #endregion
    }
}