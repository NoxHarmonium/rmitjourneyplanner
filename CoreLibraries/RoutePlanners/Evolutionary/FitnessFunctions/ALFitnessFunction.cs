// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.Logging;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion


    public struct ClosedRoute
    {
        public int start;

        public int end;

        public int id;


    }

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

        private readonly List<int> routesUsed = new List<int>(); 

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

        /// <summary>
        /// The routes traversed when calculating the fitness.
        /// </summary>
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
            INetworkDataProvider provider = properties.NetworkDataProviders[0];
            DateTime initialDepart = properties.DepartureTime;
            var openRoutes = new List<int>();
            var routeStartIndicies = new Dictionary<int, int>();
            var closedRoutes = new List<ClosedRoute>();
            var map = new Dictionary<int, List<int>>();
            var closedRouteMap = new Dictionary<int, ClosedRoute>();

            for (int i = 0; i < route.Count + 1; i++)
            {
                List<int> routes;
                if (i != route.Count)
                {
                    var node = route[i];
                    routes = provider.GetRoutesForNode(node);
                }
                else
                {
                    routes = new List<int>();
                }
                var newRoutes = routes.Where(routeId => !openRoutes.Contains(routeId)).ToList();
                var closedRouteIds = openRoutes.Where(openRoute => !routes.Contains(openRoute)).ToList();
                foreach (var newRoute in newRoutes)
                {
                    openRoutes.Add(newRoute);
                    routeStartIndicies[newRoute] = i;
                }

                foreach (var closedRouteId in closedRouteIds)
                {
                    openRoutes.Remove(closedRouteId);
                }

                foreach (var closedRouteId in closedRouteIds)
                {
                    var closedRoute = new ClosedRoute { end = i, id = closedRouteId, start = routeStartIndicies[closedRouteId] };
                    closedRouteMap[closedRouteId] = closedRoute;
                    if (i != route.Count)
                    {
                        foreach (var openRoute in openRoutes)
                        {
                            if (!map.ContainsKey(closedRouteId))
                            {
                                map[closedRouteId] = new List<int>();
                            }
                            map[closedRouteId].Add(openRoute);
                        }
                    }
                    else
                    {
                        if (!map.ContainsKey(closedRouteId))
                        {
                            map[closedRouteId] = new List<int>();
                        }
                        map[closedRouteId].Add(-1);
                    }
                    closedRoutes.Add(closedRoute);
                }
                
                
            }
            int current = 110;
            while (current != -1)
            {
                Console.Write("{0}, ", current);
                current = map[current][0];

            }
            Console.WriteLine("");
            throw new NotImplementedException();
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