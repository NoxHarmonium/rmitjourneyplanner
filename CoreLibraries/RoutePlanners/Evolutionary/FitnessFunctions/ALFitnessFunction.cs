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
            var routeMap = new Dictionary<int, List<MetlinkNode>>();
            var routeIndexes = new Dictionary<int, int>();
            //Maps a tree between routes. Key of pair is route id and value is intersection index
            var routeTree = new Dictionary<int, List<KeyValuePair<int,int>>>();

            //Add initial children
            routeTree[-1] = new List<KeyValuePair<int, int>>();
            foreach (var routeId in provider.GetRoutesForNode(route[0]))
            {
                routeTree[-1].Add(new KeyValuePair<int, int>(routeId,0));
            }

            // Build route legs
            int counter = 0;
             for (int i = 0; i < route.Count; i++)
             {

                var node = route[i];
                List<int> routes = provider.GetRoutesForNode(node);

                foreach (int subroute in routes)
                {
                    if (!routeMap.ContainsKey(subroute))
                    {
                        routeMap.Add(subroute,new List<MetlinkNode>());
                    }
                    routeMap[subroute].Add((MetlinkNode)node);
                    routeIndexes[subroute] = counter;
                }
                counter++;
            }

           

                // Build route search tree
                foreach (var kvp in routeMap)
                {
                    foreach (var kvp2 in routeMap)
                    {
                        if (kvp.Equals(kvp2))
                        {
                            continue;
                        }



                        IEnumerable<MetlinkNode> intersections = kvp.Value.Intersect(kvp2.Value);

                        if (routeIndexes[kvp2.Key] <= routeIndexes[kvp.Key])
                        {
                            continue;
                        }

                        bool any = intersections.Any();
                        if (!any)
                        {
                            if (!routeTree.ContainsKey(kvp.Key))
                            {
                                routeTree[kvp.Key] = new List<KeyValuePair<int, int>>();
                            }
                            routeTree[kvp.Key].Add(new KeyValuePair<int, int>(kvp2.Key, -1));
                        }
                        else
                        {
                            


                            MetlinkNode intersect = intersections.First();



                            int index = kvp.Value.IndexOf(intersect);
                            if (!routeTree.ContainsKey(kvp.Key))
                            {
                                routeTree[kvp.Key] = new List<KeyValuePair<int, int>>();
                            }
                            routeTree[kvp.Key].Add(new KeyValuePair<int, int>(kvp2.Key, index));
                        }
                    }

                }

            foreach (var kvp in routeMap)
            {
                if (kvp.Value.Last() == route.Last())
                {
                    if (!routeTree.ContainsKey(kvp.Key))
                    {
                        routeTree[kvp.Key] = new List<KeyValuePair<int, int>>();
                    }
                    routeTree[kvp.Key].Add(new KeyValuePair<int, int>(-2,kvp.Value.Count-1));

                }
            }

            /*
            for (int i = 0; i < route.Count-1 ; i++)
            {
                var node = route[i];
                var succ = route[i + 1];
                var nodeR = provider.GetRoutesForNode(node);
                var succR = provider.GetRoutesForNode(succ);

                if (nodeR.Intersect(succR).Any())
                {
                    continue;
                }
                foreach (var t in nodeR)
                {
                    foreach (var u in succR)
                    {
                        if (!routeTree.ContainsKey(t))
                        {
                            routeTree[t] = new List<KeyValuePair<int, int>>();
                        }
                        routeTree[t].Add(new KeyValuePair<int, int>(u, -1));
                    }
                }
            }
            */
            //Build possible paths
            NodeWrapper<int> current;
            var stack = new Stack<NodeWrapper<int>>();
            var searchMap = new Dictionary<NodeWrapper<int>, NodeWrapper<int>>();

            stack.Push(new NodeWrapper<int>(-1));

            var solutions = new List<List<NodeWrapper<int>>>();
            while (stack.Count > 0)
            {
                current = stack.Pop();
                

                if (routeTree.ContainsKey(current.Node))
                {
                    var children = routeTree[current.Node];
                    foreach (var child in children)
                    {
                        int destId = child.Key;
                        int index = child.Value;
                        double cost;
                        
                        if (index == 0 || index == -1) cost = 0;
                        else
                            cost =
                                provider.GetDistanceBetweenNodes(
                                    routeMap[current.Node][0],
                                    routeMap[current.Node][index],
                                    initialDepart.AddMilliseconds(current.Cost),
                                    current.Node).TotalTime.TotalMilliseconds;

                        var wrapper = new NodeWrapper<int>(destId, current.Cost + cost);
                        stack.Push(wrapper);
                        searchMap.Add(wrapper, new NodeWrapper<int>(current.Node));
                    }
                }
                else
                {
                    if (current.Node == -2)
                    {
                        solutions.Add(new List<NodeWrapper<int>> { current });
                    }
                    else
                    {
                        
                    }

                }

            }

            foreach (var solution in solutions)
            {
                current = solution[0];
                Console.Write("[Path: Total cost: {0} ]: ",current.Cost);
                while (current.Node != -1)
                {
                    Console.Write("{0} ,",current.Node);
                    current = searchMap[current];

                }
                Console.WriteLine("end.");
            }
           


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