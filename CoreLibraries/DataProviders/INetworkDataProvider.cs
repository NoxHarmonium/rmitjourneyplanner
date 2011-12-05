// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="INetworkDataProvider.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   A NetworkDataProvider is a plugable class that is used to provide information on networks such a train or bus network.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    #region

    using System;
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// A NetworkDataProvider is a plugable class that is used to provide information on networks such a train or bus network.
    /// </summary>
    public interface INetworkDataProvider
    {
        #region Public Methods

        /// <summary>
        /// Gets the network nodes that are on the same route as the specified node.
        /// </summary>
        /// <param name="node">
        /// The specified node.
        /// </param>
        /// <param name="routeId">
        /// The route identifier.
        /// </param>
        /// <returns>
        /// A list of <see cref="INetworkNode"/> objects.
        /// </returns>
        List<INetworkNode> GetAdjacentNodes(INetworkNode node, string routeId);

        /// <summary>
        /// Returns the type of node that this provider services.
        /// </summary>
        /// <returns>
        /// A Type representing the type of node that this provider handles.
        /// </returns>
        Type GetAssociatedType();

        /// <summary>
        /// Gets the shortest distance between nodes.
        /// </summary>
        /// <param name="source">
        /// The source node.
        /// </param>
        /// <param name="destination">
        /// The destination node.
        /// </param>
        /// <param name="time">
        /// The optimum time of departure.
        /// </param>
        /// <returns>
        /// A list of <see cref="Arc"/> objects that represent the multiple ways to get between the 2 points.
        /// </returns>
        List<Arc> GetDistanceBetweenNodes(INetworkNode source, INetworkNode destination, DateTime time);

        /// <summary>
        /// Gets the network node that is at the specfied location
        /// </summary>
        /// <param name="location">
        /// A location.
        /// </param>
        /// <returns>
        /// The node at that location or null if there is no node.
        /// </returns>
        INetworkNode GetNodeAtLocation(Location location);

        /// <summary>
        /// Get the location of the node with the specified Id.
        /// </summary>
        /// <param name="id">
        /// The unique node identifier.
        /// </param>
        /// <returns>
        /// The <see cref="Location"/> of the node.
        /// </returns>
        Location GetNodeLocation(string id);

        /// <summary>
        /// Gets the network nodes that are located within radius distance to the specified location.
        /// </summary>
        /// <param name="location">
        /// The center point for the search.
        /// </param>
        /// <param name="radius">
        /// The distance to look around the center point.
        /// </param>
        /// <returns>
        /// A list of <see cref="INetworkNode"/> objects that are in the specified area.
        /// </returns>
        List<INetworkNode> GetNodesAtLocation(Location location, double radius);

        /// <summary>
        /// Gets a list of routes that this node passes through.
        /// </summary>
        /// <param name="node">
        /// The node you wish to query.
        /// </param>
        /// <returns>
        /// A list of routes that intersect this node.
        /// </returns>
        List<string> GetRoutesForNode(INetworkNode node);

        #endregion
    }
}