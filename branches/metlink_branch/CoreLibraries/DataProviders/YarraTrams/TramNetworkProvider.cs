// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="TramNetworkProvider.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Provides network data for the Yarra Trams network.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders.YarraTrams
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using RmitJourneyPlanner.CoreLibraries.Caching;
    using RmitJourneyPlanner.CoreLibraries.DataAccess;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Provides network data for the Yarra Trams network.
    /// </summary>
    public class TramNetworkProvider : INetworkDataProvider
    {
        #region Constants and Fields

        /// <summary>
        ///   The api.
        /// </summary>
        private readonly TramTrackerApi api = new TramTrackerApi();

        /// <summary>
        ///   The location cache.
        /// </summary>
        private readonly LocationCache lCache = new LocationCache("YarraTrams");

        /// <summary>
        ///   The n cache.
        /// </summary>
        private readonly NodeCache<TramStop> nCache = new NodeCache<TramStop>("YarraTrams");

        /// <summary>
        ///   The nr cache.
        /// </summary>
        private readonly NodeRouteCache nrCache = new NodeRouteCache("YarraTrams");

        /// <summary>
        ///   The r cache.
        /// </summary>
        private readonly RouteCache rCache = new RouteCache("YarraTrams");

        /// <summary>
        ///   The s cache.
        /// </summary>
        private readonly ScheduleCache sCache = new ScheduleCache("YarraTrams");

        #endregion

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
        public List<INetworkNode> GetAdjacentNodes(INetworkNode node, string routeId)
        {
            if (routeId.Split('_')[1] == "up")
            {
                List<string> upIds = this.rCache.GetRoute(routeId, true);
                if (upIds == null)
                {
                    DataSet upData = this.api.GetListOfStopsByRouteNoAndDirection(routeId.Split('_')[0], true);
                    upIds = (from DataRow row in upData.Tables[0].Rows select row["TID"].ToString()).ToList();

                    this.rCache.AddCacheEntry(routeId, upIds, true);
                }

                var route = new Route(routeId);
                foreach (string id in upIds)
                {
                    TramStop stop = this.nCache.GetNode(id, this) ?? new TramStop(id, this);

                    stop.CurrentRoute = routeId;
                    route.AddNode(stop, true);
                }

                node.CurrentRoute = routeId;
                return route.GetAdjacent(node);
            }

            if (routeId.Split('_')[1] == "down")
            {
                List<string> downIds = this.rCache.GetRoute(routeId, false);

                if (downIds == null)
                {
                    DataSet downData = this.api.GetListOfStopsByRouteNoAndDirection(routeId.Split('_')[0], false);
                    downIds = (from DataRow row in downData.Tables[0].Rows select row["TID"].ToString()).ToList();

                    this.rCache.AddCacheEntry(routeId, downIds, false);
                }

                var route = new Route(routeId);
                foreach (string id in downIds)
                {
                    TramStop stop = this.nCache.GetNode(id, this) ?? new TramStop(id, this);

                    stop.CurrentRoute = routeId;
                    route.AddNode(stop, false);
                }

                node.CurrentRoute = routeId;
                return route.GetAdjacent(node);
            }

            throw new Exception("Route Id needs to be in format <id>_<direction> where direction is 'up' or 'down'");
        }

        /// <summary>
        /// Returns the type of node that this provider services.
        /// </summary>
        /// <returns>
        /// A Type representing the type of node that this provider handles.
        /// </returns>
        public Type GetAssociatedType()
        {
            return typeof(TramStop);
        }

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
        public List<Arc> GetDistanceBetweenNodes(INetworkNode source, INetworkNode destination, DateTime time)
        {
            if (source is TramStop && destination is TramStop)
            {
                List<string> sRoutes = this.nrCache.GetRoutes(source);
                List<string> dRoutes = this.nrCache.GetRoutes(destination);

                // First we generate the routes that intersect both nodes. 
                if (sRoutes == null)
                {
                    sRoutes = new List<string>();
                    DataSet sRouteData = this.api.GetMainRoutesForStop(source.Id);
                    if (sRouteData.Tables.Count != 0)
                    {
                        foreach (DataRow sRow in sRouteData.Tables[0].Rows)
                        {
                            sRoutes.Add(sRow[0].ToString());
                            this.nrCache.AddCacheEntry(source, sRow[0].ToString());
                        }
                    }
                }

                if (dRoutes == null)
                {
                    dRoutes = new List<string>();
                    DataSet dRouteData = this.api.GetMainRoutesForStop(destination.Id);
                    if (dRouteData.Tables.Count != 0)
                    {
                        foreach (DataRow dRow in dRouteData.Tables[0].Rows)
                        {
                            dRoutes.Add(dRow[0].ToString());
                            this.nrCache.AddCacheEntry(destination, dRow[0].ToString());
                        }
                    }
                }

                if (dRoutes.Count < 1 || sRoutes.Count < 1)
                {
                    return new List<Arc>();
                }

                var xRows = new List<string>();

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

                // TODO: Find schedules of intersecting routes....
                var arcs = new List<Arc>();
                foreach (string route in xRows)
                {
                    DataSet scheduleData = this.sCache.GetScheduleCollection(source, route, false, time);
                    if (scheduleData == null)
                    {
                        scheduleData = this.api.GetSchedulesCollection(source.Id, route, false, time);
                        this.sCache.AddScheduleCollection(source, route, false, time, scheduleData);
                    }

                    DataTable data = scheduleData.Tables[0];
                    if (data.Rows.Count > 0)
                    {
                        DateTime bestDepartureTime = default(DateTime);
                        if (
                            data.Rows.Cast<DataRow>().Any(
                                row => (DateTime)data.Rows[0]["PredictedArrivalDateTime"] >= time))
                        {
                            bestDepartureTime = (DateTime)data.Rows[0]["PredictedArrivalDateTime"];
                        }

                        if (bestDepartureTime == default(DateTime))
                        {
                            throw new Exception("No suitable departure times?");
                        }

                        DateTime bestArrivalTime = default(DateTime);
                        string tripId = data.Rows[0]["TripID"].ToString();

                        DataSet tripData = this.sCache.GetTripSchedule(tripId, bestDepartureTime);
                        if (tripData == null)
                        {
                            tripData = this.api.GetSchedulesForTrip(tripId, bestDepartureTime);
                            this.sCache.AddTripSchedule(tripId, bestDepartureTime, tripData);
                        }

                        DataTable tripTable = tripData.Tables[0];

                        bool correctDir = false;
                        foreach (DataRow row in tripTable.Rows)
                        {
                            if (row["StopNo"].ToString() == source.Id)
                            {
                                correctDir = true;
                            }

                            if (row["StopNo"].ToString() == destination.Id && correctDir)
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
                            // bestArrivalTime += new TimeSpan(0, 0, 1);
                        }

                        TimeSpan travelTime = bestArrivalTime - bestDepartureTime;
                        TimeSpan waitingTime = bestDepartureTime - time;
                        TimeSpan totalTime = travelTime + waitingTime;

                        double distance = GeometryHelper.GetStraightLineDistance(
                            source.Latitude, source.Longitude, destination.Latitude, destination.Longitude);

                        var arc = new NetworkArc(
                            source, destination, totalTime, distance, bestDepartureTime, "YarraTrams", route);
                        arcs.Add(arc);
                    }
                }

                return arcs;
            }

            return new List<Arc>();
        }

        /// <summary>
        /// Gets a DataSet containing all the data pertaining to a TramStop.
        /// </summary>
        /// <param name="id">
        /// The TramTracker stop identifier.
        /// </param>
        /// <returns>
        /// A <see cref="DataSet"/>
        /// </returns>
        public DataSet GetNodeData(string id)
        {
            DataSet data = this.nCache.GetData(id);
            if (data == null)
            {
                data = this.api.GetStopInformation(id);
                this.nCache.AddCacheEntry(id, data);
            }

            return data;
        }

        /// <summary>
        /// Get the location of the node with the specified Id.
        /// </summary>
        /// <param name="id">
        /// The unique node identifier.
        /// </param>
        /// <returns>
        /// The <see cref="Location"/> of the node.
        /// </returns>
        public Location GetNodeLocation(string id)
        {
            return this.lCache.GetPostition(id);
        }

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
        public List<INetworkNode> GetNodesAtLocation(Location location, double radius)
        {
            var nodes = new List<INetworkNode>();
            List<string[]> ids = this.lCache.GetIdsInRadius(location, radius);
            foreach (var id in ids)
            {
                TramStop stop = this.nCache.GetNode(id[0], this) ?? new TramStop(id[0], this);

                stop.CurrentRoute = id[1];
                nodes.Add(stop);
            }

            return nodes;
        }

        /// <summary>
        /// Gets a list of routes that this node passes through.
        /// </summary>
        /// <param name="node">
        /// The node you wish to query.
        /// </param>
        /// <returns>
        /// A list of routes that intersect this node.
        /// </returns>
        public List<string> GetRoutesForNode(INetworkNode node)
        {
            if (node is TramStop)
            {
                DataSet routeData = this.api.GetMainRoutesForStop(node.Id);
                if (routeData.Tables.Count < 1)
                {
                    return new List<string>();
                }

                return (from DataRow row in routeData.Tables[0].Rows select row["RouteNo"].ToString()).ToList();
            }

            return new List<string>();
        }

        #endregion
    }
}