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
    using System.IO;
    using System.Linq;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.Logging;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
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
            //TODO: Route 1942: Has alternate route at different times. Check it out....


            
            for (int z = 0; z < route.Count - 1; z++)
            {
                bool fool = route[z].Id == 	20041 && route[z+1].Id == 20030;
                var adj = properties.NetworkDataProviders[0].GetAdjacentNodes(route[z]);
                if (!adj.Contains(route[z + 1]) || fool)
                {
                    throw new Exception("Death");
                }

            }

           

            INetworkDataProvider provider = properties.NetworkDataProviders[0];
            DateTime initialDepart = properties.DepartureTime;
            var openRoutes = new List<int>();
            var closedRoutes = new List<int>();
            var closedRoutesIndex = new List<List<ClosedRoute>>();
            var routeIndex = new Dictionary<int, int>();


            for (int i = 0; i < route.Count; i++)
            {
                route[i].RetrieveData();
                //Console.Write("{0:00000}[{1}]: ", route[i].Id,((MetlinkNode)route[i]).StopSpecName);
                closedRoutesIndex.Add(new List<ClosedRoute>());
                var routes = provider.GetRoutesForNode(route[i]);
                
                //TODO: REMOVE FOR PRODUCTION
                routes.Sort();

                foreach (int routeId in routes)
                {
                    //Console.Write("{0:00000}, ", routeId);
                    if (!openRoutes.Contains(routeId) && !closedRoutes.Contains(routeId))
                    {
                        openRoutes.Add(routeId);
                        routeIndex[routeId] = i;

                    }
                }
                Console.WriteLine();

                var newOpenRoute = new List<int>();
                foreach (var openRoute in openRoutes)
                {
                    if (!routes.Contains(openRoute) || (i == route.Count-1))
                    {
                        closedRoutes.Add(openRoute);
                        var cr = new ClosedRoute { end = i, id = openRoute, start = routeIndex[openRoute] };
                        closedRoutesIndex[routeIndex[openRoute]].Add(cr);

                    }
                    else
                    {
                        newOpenRoute.Add(openRoute);
                    }
                }

                openRoutes = newOpenRoute;
            }

            /*
            StreamWriter writer = new StreamWriter("test.csv",false);
            for (int i = 0; i < closedRoutesIndex.Count; i++)
            {
                foreach (var closedRoute in closedRoutesIndex[i])
                {
                    writer.Write("{0:000000}: ", closedRoute.id);
                    for (int j = 0; j < closedRoute.start; j++)
                    {
                         writer.Write(" ");
                    }
                    for (int j = 0; j < closedRoute.end - closedRoute.start; j++)
                    {
                        writer.Write("*");
                    }
                    writer.WriteLine();
                    
                }
             }
           
             * 
             * */
            var links = new Dictionary<int, List<int>>();
            var timeSegments = new List<TransportTimeSpan>();
            //StreamWriter writer = new StreamWriter("test.csv", false);
            //writer.Write("                     ");
            foreach (var routeId in route)
            {
                //writer.Write("{0:00000} ", routeId.Id);

            }

            var pointer = 0;
            var totalTime = default(TimeSpan);
            var currentTime = initialDepart;
            double legs = 0.0;
            //Console.WriteLine("-------Fitness Evaluation-------");
            while (pointer < route.Count - 1)
            {
                var t = closedRoutesIndex[pointer];
                bool first = true;
                TransportTimeSpan minTime = default(TransportTimeSpan);
                int maxLength = 0;
                ClosedRoute bestClosedRoute = default(ClosedRoute);

                //If there are no closed routes for this node, create one.
                if (t.Count == 0)
                {
                    minTime = properties.PointDataProviders[0].EstimateDistance(
                                 (Location)route[pointer], (Location)route[pointer + 1]).Time;
                    bestClosedRoute = new ClosedRoute { id = -2, start = pointer, end = pointer + 1 };

                }
                else
                {


                    foreach (var closedRoute in t)
                    {
                       // Console.WriteLine("      ClosedRoute({0})",closedRoute.ToString());
                        
                        TransportTimeSpan time = default(TransportTimeSpan);
                        bool calced = false;
                        if (closedRoute.Length > 1)
                        {
                            calced = true;
                            if (closedRoute.Length > 3)
                            {
                                string test = "dffd";
                            }
                            time = provider.GetDistanceBetweenNodes(
                                route[closedRoute.start], route[closedRoute.end - 1], currentTime, closedRoute.id);


                        }
                        if (time == default(TransportTimeSpan))
                        {
                            calced = false;
                            time =
                                properties.PointDataProviders[0].EstimateDistance(
                                    (Location)route[closedRoute.start], (Location)route[closedRoute.end]).Time;
                            if (time == default(TransportTimeSpan))
                            {
                                //throw new Exception("Walking time is zero. An error must of occurred.");
                            }
                        }

                        if (first || (time.TotalTime < minTime.TotalTime && closedRoute.Length >= maxLength))
                        {
                            first = false;
                            minTime = time;
                            bestClosedRoute = closedRoute;
                            maxLength = closedRoute.Length;
                        }

                        //writer.Write(
                        //    calced ? "{0:000000} [{1:dd\\.hh\\:mm\\:ss}]: " : "{0:000000}*[{1:dd\\.hh\\:mm\\:ss}]: ",
                        //    closedRoute.id,
                        //    time.TotalTime);



                        //for (int j = 0; j < closedRoute.start; j++)
                        //{
                        //    writer.Write(" ");
                        //}
                        //for (int j = 0; j < closedRoute.end - closedRoute.start; j++)
                        //{
                        //    writer.Write("*");
                        //}
                        //writer.WriteLine();



                    }
                }

                totalTime += minTime.TotalTime;
                currentTime += minTime.TotalTime;

                if (bestClosedRoute.Equals(default(ClosedRoute)))
                {
                    throw new Exception("No minimum route found.");

                }
                if (bestClosedRoute.end <= pointer)
                {
                    throw new Exception("Infinite loop detected.");
                }

                for (int i = pointer; i < bestClosedRoute.end; i++)
                {
                    route[i].CurrentRoute = bestClosedRoute.id;
                }
                pointer = bestClosedRoute.end;

                //Console.WriteLine("[{0}] : [{1}] -> [{2}] ({3})", bestClosedRoute.id, bestClosedRoute.start, bestClosedRoute.end, minTime);
                //initialDepart += minTime.TotalTime;
                //if (bestClosedRoute.id != -1)
                //{
                    legs++;
                //}

            }
            //writer.Close();
            //Console.WriteLine("Total Time: {0}", totalTime);
            //Console.WriteLine("------------------------------");
            return totalTime.TotalSeconds * legs;

            //throw new NotImplementedException();
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