// -----------------------------------------------------------------------
// <copyright file="IDataProvider.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.Types;


    /// <summary>
    /// A NetworkDataProvider is a plugable class that is used to provide information on networks such a train or bus network.
    /// </summary>
    public interface INetworkDataProvider
    {
        /// <summary>
        /// Gets the network nodes that are located within radius distance to the specified location.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        List<INetworkNode> GetNodesAtLocation(Location location, double radius);

        /// <summary>
        /// Gets the network nodes the are adjacent to the specified one.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        List<INetworkNode> GetAdjacentNodes(INetworkNode node);

        /// <summary>
        /// Gets the shortest distance between nodes.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        Arc GetDistanceBetweenNodes(INetworkNode source, INetworkNode destination, DateTime time);

        /// <summary>
        /// Get the location of the node with the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Location GetNodeLocation(string id);

        
        

    }
}
