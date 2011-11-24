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
        public List<INetworkNode> GetNodesAtLocation(Location location, double radius);

        /// <summary>
        /// Gets the network nodes the are adjacent to the specified one.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<INetworkNode> GetAdjacentNodes(INetworkNode node);



        

    }
}
