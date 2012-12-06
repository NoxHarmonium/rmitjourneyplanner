// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRouteGenerator.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   The i route generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region Using Directives

    using System;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// The i route generator.
    /// </summary>
    public interface IRouteGenerator
    {
        #region Public Methods and Operators

        /// <summary>
        /// Generates a random route between source and destination nodes.
        /// </summary>
        /// <param name="source">
        /// The source node. 
        /// </param>
        /// <param name="destination">
        /// The destination node. 
        /// </param>
        /// <param name="startTime">
        /// The time the route is being generated at. 
        /// </param>
        /// <returns>
        /// </returns>
        Route Generate(INetworkNode source, INetworkNode destination, DateTime startTime);

        #endregion
    }
}