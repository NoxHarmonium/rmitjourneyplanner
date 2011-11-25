// -----------------------------------------------------------------------
// <copyright file="TramNetworkProvider.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Caching;

    /// <summary>
    /// Provides network data for the Yarra Trams network.
    /// </summary>
    public class TramNetworkProvider : INetworkDataProvider
    {

        LocationCache lCache = new LocationCache("YarraTrams");
        

        public List<INetworkNode> GetNodesAtLocation(Positioning.Location location, double radius)
        {
            List<string> ids = lCache.GetIdsInRadius(location, radius);
            throw new NotImplementedException();
        }

        public List<INetworkNode> GetAdjacentNodes(INetworkNode node)
        {
            throw new NotImplementedException();
        }

    }
}
