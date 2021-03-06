﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INetworkDataProvider.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   A class that is used to provide information on networks such a train or bus network and can be plugged into journey planners.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Data;

    using RmitJourneyPlanner.CoreLibraries.DataProviders.Ptv;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// A class that is used to provide information on networks such a train or bus network and can be plugged into journey planners.
    /// </summary>
    public interface INetworkDataProvider
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the network nodes that are adjacent to the specified node.
        /// </summary>
        /// <param name="node">
        /// The specified node. 
        /// </param>
        /// <returns>
        /// A list of <see cref="INetworkNode"/> objects. 
        /// </returns>
        List<INetworkNode> GetAdjacentNodes(INetworkNode node);

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
        List<INetworkNode> GetAdjacentNodes(INetworkNode node, int routeId);

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
        /// <param name="departureTime">
        /// The optimum time of departure. 
        /// </param>
        /// <returns>
        /// A list of <see cref="Arc"/> objects that represent the multiple ways to get between the 2 points. 
        /// </returns>
        List<Arc> GetDistanceBetweenNodes(INetworkNode source, INetworkNode destination, DateTime departureTime);

        /// <summary>
        /// Gets the network nodes that are located within radius distance to the specified location.
        /// </summary>
        /// <param name="source">
        /// The source point. 
        /// </param>
        /// <param name="destination">
        /// The point that is the target of the search. 
        /// </param>
        /// <param name="radius">
        /// The distance to look around the source point. 
        /// </param>
        /// <param name="allowTransfer">
        /// Determines whether nodes that are of a different line are considered.
        /// </param>
        /// <returns>
        /// The <see cref="INetworkNode"/> object that is the closest to the destination inside the radius. 
        /// </returns>
        INetworkNode GetNodeClosestToPointWithinArea(
            INetworkNode source, INetworkNode destination, double radius, bool allowTransfer);

        /// <summary>
        /// Gets a DataTable containing data relating to the specified node id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// A DataTable containing data relating to the specified node id.
        /// </returns>
        DataTable GetNodeData(int id);

        /// <summary>
        /// Returns a node from a given Id.
        /// </summary>
        /// <param name="id">
        /// The identifier of the node. 
        /// </param>
        /// <returns>
        /// A node. 
        /// </returns>
        INetworkNode GetNodeFromId(int id);

        /// <summary>
        /// Get the location of the node with the specified Id.
        /// </summary>
        /// <param name="id">
        /// The unique node identifier. 
        /// </param>
        /// <returns>
        /// The <see cref="Location"/> of the node. 
        /// </returns>
        Location GetNodeLocation(int id);

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
        List<NodeWrapper<PtvNode>> GetNodesAtLocation(Location location, double radius);

        /// <summary>
        /// Gets a list of routes that this node passes through.
        /// </summary>
        /// <param name="node">
        /// The node you wish to query. 
        /// </param>
        /// <returns>
        /// A list of routes that intersect this node. 
        /// </returns>
        List<int> GetRoutesForNode(INetworkNode node);

        /// <summary>
        /// Returns true if there is at least 1 route common between the 2 nodes.
        /// </summary>
        /// <param name="first">
        /// The first node to test. 
        /// </param>
        /// <param name="second">
        /// The second node to test. 
        /// </param>
        /// <returns>
        /// A boolean value. 
        /// </returns>
        bool RoutesIntersect(INetworkNode first, INetworkNode second);

        #endregion
    }
}