// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.Logging;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   The al fitness function.
    /// </summary>
    public class AlFitnessFunction : IFitnessFunction
    {
        #region Constants and Fields

        /// <summary>
        ///   The properties.
        /// </summary>
        private readonly EvolutionaryProperties properties;

        private List<int> routesUsed = new List<int>(); 

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="AlFitnessFunction" /> class.
        /// </summary>
        /// <param name="properties"> The properties. </param>
        public AlFitnessFunction(EvolutionaryProperties properties)
        {
            this.properties = properties;
        }

        public List<int> RoutesUsed
        {
            get
            {
                return this.routesUsed;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   The get fitness.
        /// </summary>
        /// <param name="route"> The route. </param>
        /// <returns> The get fitness. </returns>
        /// <exception cref="Exception"></exception>
        public double GetFitness(Route route)
        {
            routesUsed.Clear();
            var provider = this.properties.NetworkDataProviders[0];

            var condensedRoute = new List<int[]>();
            DateTime departureTime = this.properties.DepartureTime;

            var routeRoutes = route.Select(provider.GetRoutesForNode).ToList();

            int index = 0;
            for (int i = 0; i < routeRoutes.Count; i++)
            {
                if (i == routeRoutes.Count - 1 || !routeRoutes[i].Intersect(routeRoutes[i + 1]).Any())
                {
                    if (index != i)
                    {
                        var availableRoutes = routeRoutes[index].Intersect(routeRoutes[i]);
                        var validRoutes =
                            availableRoutes.Where(
                                validRoute => provider.IsValidOrder(route[index], route[i], validRoute)).ToList();

                        var minTime = new TransportTimeSpan();
                        
                        minTime.TravelTime = TimeSpan.MaxValue;
                        int minRoute = -1;
                        foreach (var routeId in validRoutes)
                        {
                            TransportTimeSpan time = provider.GetDistanceBetweenNodes(route[index], route[i],departureTime,routeId );
                                
                                //.CalculateTime(route[index]
                                //routeId, Convert.ToInt32(route[index].Id), Convert.ToInt32(route[i].Id), departureTime);

                            if (time.TravelTime != default(TimeSpan) && time < minTime)
                            {
                                minTime = time;
                                minRoute = routeId;
                            }
                        }
                        if (minTime.TravelTime != TimeSpan.MaxValue)
                        {
                            condensedRoute.Add(new[] { index, i, minRoute, (int)minTime.TotalTime.TotalSeconds });
                            routesUsed.Add(minRoute);
                        }
                        else
                        {
                            minTime.TravelTime =
                                this.properties.PointDataProviders[0].EstimateDistance(
                                    (Location)route[index], (Location)route[i]).Time;
                            condensedRoute.Add(new[] { index, i, -1, (int)minTime.TotalTime.TotalSeconds });
                            //Logger.Log(this,"No service available. Adding walking link...");
                        }
                        departureTime += minTime.TotalTime;
                        //Logger.Log(this,"{0} ({1}) ---[{4} <{6}>]---> {2} ({3})\n += W: {7} T: {5}", route[index].Id, ((MetlinkNode)route[index]).StopSpecName, route[i].Id, ((MetlinkNode)route[i]).StopSpecName,minRoute,minTime.TravelTime,route[i].TransportType,minTime.WaitingTime);
                    }

                    if (i != routeRoutes.Count - 1)
                    {
                        TimeSpan walkingTime =
                            this.properties.PointDataProviders[0].EstimateDistance(
                                (Location)route[i], (Location)route[i + 1]).Time;

                        condensedRoute.Add(new[] { i, i + 1, -1, (int)walkingTime.TotalSeconds });
                        departureTime += walkingTime;
                        //Logger.Log(this,"{0} ({1}) ---[{4} <{6}>]---> {2} ({3})\n += {5}", route[i].Id, ((MetlinkNode)route[i]).StopSpecName, route[i+1].Id, ((MetlinkNode)route[i+1]).StopSpecName, "Route Link", walkingTime,"Walking");
                    }
                    index = i + 1;
                }

                route[i].TotalTime = departureTime - this.properties.DepartureTime;
            }

            //Logger.Log(this,"Total travel time: {0}", departureTime - this.properties.DepartureTime);
            return condensedRoute.Aggregate<int[], double>(0, (current, arc) => current + arc[3]);
        }

        #endregion

      

        #region Methods

        /// <summary>
        ///   The calculate time.
        /// </summary>
        /// <param name="routeid"> The routeid. </param>
        /// <param name="id1"> The id 1. </param>
        /// <param name="id2"> The id 2. </param>
        /// <param name="departureTime"> The departure time. </param>
        /// <returns> </returns>
        [Obsolete]
        private TimeSpan CalculateTime(int routeid, int id1, int id2, DateTime departureTime)
        {
            string dowFilter = string.Empty;
            for (int j = 0; j < (int)departureTime.DayOfWeek; j++)
            {
                dowFilter = dowFilter.Insert(0, "_");
            }

            dowFilter += "0%";

            string query =
                string.Format(
                    @"select st1.ServiceID, 
                    min(st1.DepartTime), st1.DepartTime,st2.ArriveTime frOM tblSST st1
                    INNER JOIN tblSST st2
                    ON st1.ServiceID=st2.ServiceID
                    WHERE st1.RouteID={3} AND st1.DepartTime>{2} AND st1.SourceID={0} AND st1.DOW LIKE '{4}' 
                    AND st2.DestID={1}
                        ",
                    id1,
                    id2,
                    departureTime.ToString("Hmm"),
                    routeid,
                    dowFilter);

            DataTable result = this.properties.Database.GetDataSet(query);

            // departureTime.
            if (result.Rows.Count > 0 && result.Rows[0][0].ToString() != string.Empty)
            {
                DateTime arrivalTime = this.ParseDate(result.Rows[0]["ArriveTime"].ToString());
                arrivalTime += departureTime.Date - default(DateTime).Date;

                TimeSpan output = arrivalTime - departureTime;
                if (output.Ticks < 0)
                {
                    // throw new Exception("Negitive time span detected.");
                    // Logger.Log(this,"WARNING: Negitive timespan between nodes detected!");
                    return default(TimeSpan);
                }

                return output;
            }
            // Logger.Log(this,"WARNING: Null timespan between nodes detected!");
            return default(TimeSpan);
        }

        /// <summary>
        ///   The parse date.
        /// </summary>
        /// <param name="date"> The date. </param>
        /// <returns> </returns>
        [Obsolete]
        private DateTime ParseDate(string date)
        {
            if (date == "9999")
            {
                date = "0000";
            }
            
            if (date == "0" || date == "-1")
            {
                date = "0000";
            }

            int minutes = Convert.ToInt32(date.Substring(date.Length - 2, 2));
            int hours = Convert.ToInt32(date.Substring(0, date.Length - 2));
            var dt = new DateTime();
            int days = 0;
            if (hours >= 24)
            {
                days = 1;
                hours -= 24;
            }

            dt = dt.Add(new TimeSpan(days, hours, minutes, 0));

            /*
            if (!DateTime.TryParseExact(date, "Hmm", null, DateTimeStyles.None, out dt))
            {
                if (date.Substring(0, 2) == "24")
                {
                    date = "00" + date.Substring(2, 2);
                    if (!DateTime.TryParseExact(date, "Hmm", null, DateTimeStyles.None, out dt))
                    {
                        throw new Exception("Time parsing error");
                    }
                }
            }
            */

            return dt;
        }

        #endregion
    }
}