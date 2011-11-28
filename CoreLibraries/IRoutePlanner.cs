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
        /// Take a list of nodes and attempt to find the best path or paths between them.
        /// </summary>
        /// <param name="itinerary"></param>
        /// <returns></returns>
        List<Arc>[] Solve(List<INetworkNode> itinerary);

    }
}
