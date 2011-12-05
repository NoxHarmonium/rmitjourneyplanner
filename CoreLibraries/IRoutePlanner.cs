// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="IRoutePlanner.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   A route planner takes a list of nodes and joins them up using the
//   provided transport networks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries
{
    #region

    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    #endregion

    /// <summary>
    /// A route planner takes a list of nodes and joins them up using the
    ///   provided transport networks.
    /// </summary>
    public interface IRoutePlanner
    {
        #region Public Properties

        /// <summary>
        ///   Gets the best node found so far.
        /// </summary>
        INetworkNode BestNode { get; }

        /// <summary>
        ///   Gets the current node being traversed.
        /// </summary>
        INetworkNode Current { get; }

        #endregion

        #region Public Methods

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
        /// <param name="itinerary">
        /// </param>
        void Start(List<INetworkNode> itinerary);

        #endregion
    }
}