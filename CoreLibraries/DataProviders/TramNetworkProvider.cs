﻿// -----------------------------------------------------------------------
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

        /// <summary>
        /// Gets a DataSet containing all the data pertaining to a node.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataSet GetNodeData(string id)
        {
            DataSet data = nCache.GetData(id);
            if (data == null)
            {
                data = api.GetStopInformation(id);
                nCache.AddCacheEntry(id, data);
            }
            return data;
        }
        
        /// <summary>
        /// Gets the position of the specified node.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Location GetNodeLocation(string id)
        {
            return lCache.GetPostition(id);
        }

        /// <summary>
        /// Gets a list of nodes that are within a certain radius of a location.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the nodes that are directly connected to the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="routeId"></param>
        /// <returns></returns>
        public List<INetworkNode> GetAdjacentNodes(INetworkNode node, string routeId)
        {


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
                route.AddNode(new TramStop(id, this),true);
            }
            foreach (string id in downIds)
            {
                route.AddNode(new TramStop(id, this), false);
            }

            
            return route.GetAdjacent(node);

        }


        /// <summary>
        /// Gets the arc between 2 nodes, taking into account the time of the next departure.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="requestTime"></param>
        /// <returns></returns>
        public List<Arc> GetDistanceBetweenNodes(INetworkNode source, INetworkNode destination,DateTime requestTime)
        {
            if (source is TramStop && destination is TramStop)
            {
                //First we generate the routes that intersect both nodes. 
                DataSet sRouteData = api.GetMainRoutesForStop(source.ID);
                DataSet dRouteData = api.GetMainRoutesForStop(destination.ID);

                if (dRouteData.Tables.Count < 1 || sRouteData.Tables.Count < 1)
                {
                    return new List<Arc>();
                }

                List<string> xRows = new List<string>();

                foreach (DataRow sRow in sRouteData.Tables[0].Rows)
                {
                    foreach (DataRow dRow in dRouteData.Tables[0].Rows)
                    {
                        if (sRow[0].ToString() == dRow[0].ToString())
                        {
                            if (!xRows.Contains(sRow[0].ToString()))
                            {
                                xRows.Add(sRow[0].ToString());
                            }
                        }
                    }
                }

                //TODO: Find schedules of intersecting routes....


                List<Arc> arcs = new List<Arc>();
                foreach (string route in xRows)
                {
                    DataTable data = api.GetSchedulesCollection(source.ID, route, false, requestTime).Tables[0];
                    if (data.Rows.Count > 0)
                    {
                        DateTime bestDepartureTime = (DateTime)data.Rows[0]["PredictedArrivalDateTime"];
                        DateTime bestArrivalTime = default(DateTime);
                        string tripID = data.Rows[0]["TripID"].ToString();
                        DataTable tripData = api.GetSchedulesForTrip(tripID, bestDepartureTime).Tables[0];
                        foreach (DataRow row in tripData.Rows)
                        {
                            if (row["StopNo"].ToString() == destination.ID)
                            {
                                bestArrivalTime = (DateTime)row["ScheduledArrivalDateTime"];
                                break;
                            }

                        }

                        if (bestArrivalTime == default(DateTime))
                        {
                            break;
                        }

                        TimeSpan travelTime = bestArrivalTime - bestDepartureTime;
                        TimeSpan waitingTime = bestDepartureTime - requestTime;
                        TimeSpan totalTime = travelTime + waitingTime;

                        NetworkArc arc = new NetworkArc(source, destination, totalTime,
                                         GeometryHelper.GetStraightLineDistance(source.Latitude, source.Longitude, destination.Latitude, destination.Longitude),
                                         bestDepartureTime,
                                         "YarraTrams");
                        arcs.Add(arc);

                    }
                }






                return arcs;
            }
            else
            {
                return new List<Arc>();
            }

        }

        /// <summary>
        /// Gets a list of routes that this node passes through.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<string> GetRoutesForNode(INetworkNode node)
        {
            if (node is TramStop)
            {
                List<string> routes = new List<string>();
                DataSet routeData = api.GetMainRoutesForStop(node.ID);
                if (routeData.Tables.Count < 1)
                {
                    return new List<string>();
                }

                foreach (DataRow row in routeData.Tables[0].Rows)
                {
                    routes.Add(row["RouteNo"].ToString());
                }
                return routes;
            }
            else
            {
                return new List<string>();
            }



        }

        /// <summary>
        /// Gets the type of node associated with this provider.
        /// </summary>
        /// <returns></returns>
        public Type GetAssociatedType()
        {
            return typeof(TramStop);
        }
    }
}
