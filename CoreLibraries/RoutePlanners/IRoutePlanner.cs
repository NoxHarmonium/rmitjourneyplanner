// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRoutePlanner.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   A route planner takes a list of nodes and joins them up using the provided transport networks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners
{
    #region Using Directives

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// A route planner takes a list of nodes and joins them up using the provided transport networks.
    /// </summary>
    public interface IRoutePlanner
    {
        #region Public Properties

        /// <summary>
        ///   Gets the current iteration the route planner is on.
        /// </summary>
        int Iteration { get; }

        /// <summary>
        ///   Gets the result of the last iteration.
        /// </summary>
        Result IterationResult { get; }

        /// <summary>
        ///   Gets the population of the algorithm.
        /// </summary>
        Population Population { get; }

        /// <summary>
        ///   Gets the data structure that holds all the properties related to route planning.
        /// </summary>
        EvolutionaryProperties Properties { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Register a network data provider to use with the route planning.
        /// </summary>
        /// <param name="provider">
        /// </param>
        void RegisterNetworkDataProvider(INetworkDataProvider provider);

        /// <summary>
        /// Register a point to point data provider for use with the route planning.
        /// </summary>
        /// <param name="provider">
        /// </param>
        void RegisterPointDataProvider(IPointDataProvider provider);

        /// <summary>
        /// Solve the next iteration of the algorithm.
        /// </summary>
        /// <returns>
        /// The solve step. 
        /// </returns>
        bool SolveStep();

        /// <summary>
        /// Start solving a route
        /// </summary>
        void Start();

        #endregion
    }
}