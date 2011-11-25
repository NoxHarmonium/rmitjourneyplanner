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
    using DataAccess;
    using System.Data;

    /// <summary>
    /// Provides network data for the Yarra Trams network.
    /// </summary>
    public class TramNetworkProvider : INetworkDataProvider
    {

        LocationCache lCache = new LocationCache("YarraTrams");
        NodeCache nCache = new NodeCache("YarraTrams");
        TramTrackerAPI api = new TramTrackerAPI();
        

        public List<INetworkNode> GetNodesAtLocation(Positioning.Location location, double radius)
        {
            List<INetworkNode> nodes = new List<INetworkNode>();
            List<string> ids = lCache.GetIdsInRadius(location, radius);
            foreach (string id in ids)
            {
                TramStop stop = nCache.GetNode(id, this);
                if (stop == null)
                {
                    DataSet data = api.GetStopInformation(id);
                    stop = new TramStop(this, data);
                    nCache.AddCacheEntry(id,data);

                }
                
                nodes.Add(stop);             
                
            }

            return nodes;
        }

        public List<INetworkNode> GetAdjacentNodes(INetworkNode node)
        {
            throw new NotImplementedException();
        }

    }
}
