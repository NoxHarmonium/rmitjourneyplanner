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
        NodeRouteCache nrCache = new NodeRouteCache("YarraTrams");
        TramTrackerAPI api = new TramTrackerAPI();
        ScheduleCache sCache = new ScheduleCache("YarraTrams");

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
            List<string[]> ids = lCache.GetIdsInRadius(location, radius);
            foreach (string[] id in ids)
            {
                TramStop stop = nCache.GetNode(id[0], this);
                
                if (stop == null)
                {                   
                    stop = new TramStop(id[0],this);
                }
                stop.CurrentRoute = id[1];
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
                TramStop stop = new TramStop(id, this);
                stop.CurrentRoute = routeId;
                route.AddNode(stop,true);
            }
            foreach (string id in downIds)
            {
                TramStop stop = new TramStop(id, this);
                stop.CurrentRoute = routeId;
                route.AddNode(stop, false);
            }
            node.CurrentRoute = routeId;
            
            return route.GetAdjacent(node);

        }

        /// <summary>
        /// Gets the network node that is at the specfied location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public INetworkNode GetNodeAtLocation(Location location)
        {
            string id = lCache.GetIdFromLocation(location);
            if (id != null)
            {
                return new TramStop(lCache.GetIdFromLocation(location), this);
            }
            else
            {
                return null;
            }
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
                List<string> sRoutes = nrCache.GetRoutes(source);
                List<string> dRoutes = nrCache.GetRoutes(destination);
                

                //First we generate the routes that intersect both nodes. 
                if (sRoutes == null)
                {
                    sRoutes = new List<string>();
                    DataSet sRouteData = api.GetMainRoutesForStop(source.ID);
                    if (sRouteData.Tables.Count != 0)
                    {
                        foreach (DataRow sRow in sRouteData.Tables[0].Rows)
                        {
                            sRoutes.Add(sRow[0].ToString());
                            nrCache.AddCacheEntry(source, sRow[0].ToString());
                        }
                    }
                }

                if (dRoutes == null)
                {
                    dRoutes = new List<string>();
                    DataSet dRouteData = api.GetMainRoutesForStop(destination.ID);
                    if (dRouteData.Tables.Count != 0)
                    {
                        foreach (DataRow dRow in dRouteData.Tables[0].Rows)
                        {
                            dRoutes.Add(dRow[0].ToString());
                            nrCache.AddCacheEntry(destination, dRow[0].ToString());
                        }
                    }
                }




                if (dRoutes.Count < 1 || sRoutes.Count < 1)
                {
                    return new List<Arc>();
                }

                List<string> xRows = new List<string>();

                foreach (string sRow in sRoutes)
                {
                    foreach (string dRow in dRoutes)
                    {
                        if (sRow == dRow)
                        {
                            if (!xRows.Contains(sRow))
                            {
                                xRows.Add(sRow);
                            }
                        }
                    }
                }

                //TODO: Find schedules of intersecting routes....


                List<Arc> arcs = new List<Arc>();
                foreach (string route in xRows)
                {

                    DataSet scheduleData = sCache.GetScheduleCollection(source, route, false, requestTime);
                    if (scheduleData == null)
                    {
                        scheduleData = api.GetSchedulesCollection(source.ID, route, false, requestTime);
                        sCache.AddScheduleCollection(source, route, false, requestTime,scheduleData);
                    }
                    DataTable data = scheduleData.Tables[0];
                    if (data.Rows.Count > 0)
                    {
                        DateTime bestDepartureTime = default(DateTime);
                        foreach (DataRow row in data.Rows)
                        {
                            if ((DateTime)data.Rows[0]["PredictedArrivalDateTime"] >= requestTime)
                            {
                                bestDepartureTime = (DateTime)data.Rows[0]["PredictedArrivalDateTime"];
                                break;
                            }
                        }
                        if (bestDepartureTime == default(DateTime))
                        {
                            throw new Exception("No suitable departure times?");
                        }
                        DateTime bestArrivalTime = default(DateTime);
                        string tripID = data.Rows[0]["TripID"].ToString();

                        DataSet tripData = sCache.GetTripSchedule(tripID, bestDepartureTime);
                        if (tripData == null)
                        {
                            tripData = api.GetSchedulesForTrip(tripID, bestDepartureTime);
                            sCache.AddTripSchedule(tripID, bestDepartureTime, tripData);
                        }
                        
                        DataTable tripTable = tripData.Tables[0];
                        
                        
                        bool correctDir = false;
                        foreach (DataRow row in tripTable.Rows)
                        {
                            if (row["StopNo"].ToString() == source.ID)
                            {
                                correctDir = true;
                            }
                            if (row["StopNo"].ToString() == destination.ID && correctDir)
                            {
                                bestArrivalTime = (DateTime)row["ScheduledArrivalDateTime"];
                                break;
                            }

                        }
                       

                        if (bestArrivalTime == default(DateTime))
                        {
                            break;
                        }

                        if (bestArrivalTime < bestDepartureTime)
                        {
                            throw new Exception("Arriving before leaving??");
                        
                        }
                        if (bestArrivalTime == bestDepartureTime)
                        {
                            //bestArrivalTime += new TimeSpan(0, 0, 1);
                        }


                        TimeSpan travelTime = bestArrivalTime - bestDepartureTime;
                        TimeSpan waitingTime = bestDepartureTime - requestTime;
                        TimeSpan totalTime = travelTime + waitingTime;

                        NetworkArc arc = new NetworkArc(source, destination, totalTime,
                                         GeometryHelper.GetStraightLineDistance(source.Latitude, source.Longitude, destination.Latitude, destination.Longitude),
                                         bestDepartureTime,
                                         "YarraTrams",route);
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
