// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="EvolutionaryRoutePlanner.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Finds the best route between nodes using evolutionary algorithms.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries
{
    #region

    using System;
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    #endregion

    /// <summary>
    /// Finds the best route between nodes using evolutionary algorithms.
    /// </summary>
    public class EvolutionaryRoutePlanner : IRoutePlanner
    {
        #region Public Properties

        /// <summary>
        ///   Gets the best node found so far.
        /// </summary>
        public INetworkNode BestNode
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///   Gets the current node being traversed.
        /// </summary>
        public INetworkNode Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Register a network data provider to use with the route planning.
        /// </summary>
        /// <param name="provider">
        /// </param>
        public void RegisterNetworkDataProvider(INetworkDataProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Register a point to point data provider for use with the route planning.
        /// </summary>
        /// <param name="provider">
        /// </param>
        public void RegisterPointDataProvider(IPointDataProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Solve the next iteration of the algorithm.
        /// </summary>
        /// <returns>
        /// The solve step.
        /// </returns>
        public bool SolveStep()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Start solving a route
        /// </summary>
        /// <param name="itinerary">
        /// </param>
        public void Start(List<INetworkNode> itinerary)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}