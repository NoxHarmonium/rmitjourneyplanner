// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region

    using System;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   The i route generator.
    /// </summary>
    public interface IRouteGenerator
    {
        #region Public Methods

        /// <summary>
        ///   Generates a random route between source and destination nodes.
        /// </summary>
        /// <param name="source"> The source node. </param>
        /// <param name="destination"> The destination node. </param>
        /// <param name="startTime"> The time the route is being generated at. </param>
        /// <returns> </returns>
        Route Generate(INetworkNode source, INetworkNode destination, DateTime startTime);

        #endregion
    }
}