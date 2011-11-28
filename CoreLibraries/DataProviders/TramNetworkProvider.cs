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
    using System.Data;
    using Caching;
    using DataAccess;
    using System.Data;
    using Types;
    using Positioning;

    /// <summary>
    /// Provides network data for the Yarra Trams network.
    /// </summary>
    public class TramNetworkProvider : INetworkDataProvider
    {

        LocationCache lCache = new LocationCache("YarraTrams");
        NodeCache<TramStop> nCache = new NodeCache<TramStop>("YarraTrams");
        RouteCache rCache = new RouteCache("YarraTrams");
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

            DataSet routeData = api.GetMainRoutesForStop(node.ID);
            //TODO: Multiple routes??

            string routeId = routeData.Tables[0].Rows[0]["RouteNo"].ToString();

            List<string> upIds = rCache.GetRoute(routeId, true);
            List<string> downIds = rCache.GetRoute(routeId, false);

            if (upIds == null)
            {
                upIds = new List<string>();
                
                DataSet upData = api.GetListOfStopsByRouteNoAndDirection(routeId, true);
                foreach (DataRow row in upData.Tables[0].Rows)
                {
                    upIds.Add(row["TID"].ToString());
                }
                rCache.AddCacheEntry(routeId,upIds,true);

            }
            if (downIds == null)
            {
                downIds = new List<string>();
                
                DataSet downData = api.GetListOfStopsByRouteNoAndDirection(routeId, false);
                foreach (DataRow row in downData.Tables[0].Rows)
                {
                    downIds.Add(row["TID"].ToString());
                }
                rCache.AddCacheEntry(routeId,downIds,false);
            }

            Route route = new Route(routeId, new TramStop(upIds[0], this), new TramStop(upIds[upIds.Count - 1],this));
            foreach(string id in upIds)
            {
                route.AddNode(new TramStop(id, null),true);
            }
            foreach (string id in downIds)
            {
                route.AddNode(new TramStop(id, null), false);
            }

            
            return route.GetAdjacent(node);

        }

    }
}
