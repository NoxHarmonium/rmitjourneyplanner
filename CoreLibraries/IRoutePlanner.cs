// -----------------------------------------------------------------------
// <copyright file="IRoutePlanner.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DataProviders;
    using Types;

    /// <summary>
    /// A route planner takes a list of nodes and joins them up using the
    /// provided transport networks.
    /// </summary>
    public interface IRoutePlanner
    {
        /// <summary>
        /// Register a network data provider to use with the route planning.
        /// </summary>
        /// <param name="provider"></param>
        void RegisterNetworkDataProvider(INetworkDataProvider provider);

        /// <summary>
        /// Register a point to point data provider for use with the route planning.
        /// </summary>
        /// <param name="provider"></param>
        void RegisterPointDataProvider(IPointDataProvider provider);

        /// <summary>
        /// Start solving a route
        /// </summary>
        /// <param name="itinerary"></param>
        /// <returns></returns>
        void Start(List<DataProviders.INetworkNode> itinerary);

        /// <summary>
        /// Solve the next iteration of the algorithm.
        /// </summary>
        /// <returns></returns>
        bool SolveStep();

        /// <summary>
        /// Gets the current node being traversed.
        /// </summary>
        INetworkNode Current
        {
            get;
        }

        /// <summary>
        /// Gets the best node found so far.
        /// </summary>
        INetworkNode BestNode
        {
            get;
        }



    }
}
