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
    using Positioning;

    /// <summary>
    /// Provides network data for the Yarra Trams network.
    /// </summary>
    public class TramNetworkProvider : INetworkDataProvider
    {

        LocationCache lCache = new LocationCache("YarraTrams");
        NodeCache<TramStop> nCache = new NodeCache<TramStop>("YarraTrams");
        TramTrackerAPI api = new TramTrackerAPI();

        public DataSet GetNodeData(string id)
        {
            DataSet data = api.GetStopInformation(id); 
            nCache.AddCacheEntry(id, data);      
            return data;
        }

        public Location GetNodeLocation(string id)
        {
            return lCache.GetPostition(id);
        }

        public List<INetworkNode> GetNodesAtLocation(Positioning.Location location, double radius)
        {
            List<INetworkNode> nodes = new List<INetworkNode>();
            List<string> ids = lCache.GetIdsInRadius(location, radius);
            foreach (string id in ids)
            {
                TramStop stop = nCache.GetNode(id, this);
                if (stop == null)
                {                   
                    stop = new TramStop(id,this);                   
                }
                
                nodes.Add(stop);             
                
            }

            return nodes;
        }

        public List<INetworkNode> GetAdjacentNodes(INetworkNode node)
        {
            //api.GetListOfStopsByRouteNoAndDirection
            throw new NotImplementedException();
        }

    }
}
