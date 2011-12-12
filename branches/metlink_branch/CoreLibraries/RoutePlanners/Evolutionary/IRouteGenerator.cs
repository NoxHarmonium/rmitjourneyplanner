// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="IRouteGenerator.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   The i route generator.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// The i route generator.
    /// </summary>
    public interface IRouteGenerator
    {
        #region Public Methods

        /// <summary>
        /// Generates a random route between source and destination nodes.
        /// </summary>
        /// <param name="source">
        /// The source node.
        /// </param>
        /// <param name="destination">
        /// The destination node.
        /// </param>
        /// <returns>
        /// </returns>
        Route Generate(INetworkNode source, INetworkNode destination);

        #endregion
    }
}