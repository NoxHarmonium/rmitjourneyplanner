// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners
{
    #region

    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   A route planner takes a list of nodes and joins them up using the provided transport networks.
    /// </summary>
    public interface IRoutePlanner
    {
        #region Public Properties

        /// <summary>
        ///   Gets the population of the algorithm.
        /// </summary>
        List<Critter> Population { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Register a network data provider to use with the route planning.
        /// </summary>
        /// <param name="provider"> </param>
        void RegisterNetworkDataProvider(INetworkDataProvider provider);

        /// <summary>
        ///   Register a point to point data provider for use with the route planning.
        /// </summary>
        /// <param name="provider"> </param>
        void RegisterPointDataProvider(IPointDataProvider provider);

        /// <summary>
        ///   Solve the next iteration of the algorithm.
        /// </summary>
        /// <returns> The solve step. </returns>
        bool SolveStep();

        /// <summary>
        ///   Start solving a route
        /// </summary>
        void Start();

        #endregion
    }
}