// -----------------------------------------------------------------------
// <copyright file="EvolutionaryRoutePlanner.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Finds the best route between nodes using evolutionary algorithms.
    /// </summary>
    public class EvolutionaryRoutePlanner : IRoutePlanner
    {

        /// <summary>
        /// Register a network data provider to use with the route planning.
        /// </summary>
        /// <param name="provider"></param>
        public void RegisterNetworkDataProvider(DataProviders.INetworkDataProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Register a point to point data provider for use with the route planning.
        /// </summary>
        /// <param name="provider"></param>
        public void RegisterPointDataProvider(DataProviders.IPointDataProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Start solving a route
        /// </summary>
        /// <param name="itinerary"></param>
        /// <returns></returns>
        public void Start(List<DataProviders.INetworkNode> itinerary)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Solve the next iteration of the algorithm.
        /// </summary>
        /// <returns></returns>
        public bool SolveStep()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current node being traversed.
        /// </summary>
        public DataProviders.INetworkNode Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the best node found so far.
        /// </summary>
        public DataProviders.INetworkNode BestNode
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
