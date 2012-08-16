// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
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

        public Fitness GetFitness(Route route)
        {
            return this.GetFitness(route,properties.DepartureTime);
        }
        
            /// <summary>
        ///   The get fitness.
        /// </summary>
        /// <param name="route"> The route. </param>
        /// <returns> The get fitness. </returns>
        /// <exception cref="Exception"></exception>
        public Fitness GetFitness(Route route, DateTime initialDepart)
        {
            

            //TODO: Remove this line
            //initialDepart = properties.DepartureTime;


            var fitness = new Fitness();
            

            INetworkDataProvider provider = properties.NetworkDataProviders[0];
            
            var openRoutes = new List<int>();
            var closedRoutes = new List<int>();
            var closedRoutesIndex = new List<List<ClosedRoute>>();
            var routeIndex = new Dictionary<int, int>();


            for (int i = 0; i < route.Count; i++)
            {
                route[i].Node.RetrieveData();
                //Console.Write("{0:00000}[{1}]: ", route[i].Id,((MetlinkNode)route[i]).StopSpecName);
                closedRoutesIndex.Add(new List<ClosedRoute>());
                var routes = provider.GetRoutesForNode(route[i].Node);
                
                foreach (int routeId in routes)
                {
                    //Console.Write("{0:00000}, ", routeId);
                    if (!openRoutes.Contains(routeId))// && !closedRoutes.Contains(routeId))
                    {
                        openRoutes.Add(routeId);
                        routeIndex[routeId] = i;

                    }
                }
                //Console.WriteLine();

                var newOpenRoute = new List<int>();
                foreach (var openRoute in openRoutes)
                {
                    if ((!routes.Contains(openRoute) || (i == route.Count - 1)))
                    {

                        if (routeIndex[openRoute] != i - 1 || i == route.Count - 1)
                        {
                            closedRoutes.Add(openRoute);
                            ClosedRoute cr;
                            cr = i != route.Count - 1 ? 
                                new ClosedRoute { end = i - 1, id = openRoute, start = routeIndex[openRoute] } : 
                                new ClosedRoute { end = i, id = openRoute, start = routeIndex[openRoute] };
                            closedRoutesIndex[routeIndex[openRoute]].Add(cr);
                        }

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
            //StreamWriter writer = new StreamWriter("test.csv", false);
            //writer.Write("                     ");
            foreach (var routeId in route)
            {
                //writer.Write("{0:00000} ", routeId.Id);

            }

            var pointer = 0;
            var totalTime = default(TransportTimeSpan);
            var currentTime = initialDepart;
            int legs = 0;
            int totalBus = 0;
            int totalTrain = 0;
            int totalTram = 0;
            bool firstService = true;
       
            //Console.WriteLine("-------UnifiedFitnessScore Evaluation-------");
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
                                 (Location)route[pointer].Node, (Location)route[pointer + 1].Node).Time;
                    bestClosedRoute = new ClosedRoute { id = -2, start = pointer, end = pointer + 1 };

                }
                else
                {


                    foreach (var closedRoute in t)
                    {
                        //Console.WriteLine("      ClosedRoute({0})",closedRoute.ToString());
                        
                        TransportTimeSpan time = default(TransportTimeSpan);
                        bool calced = false;
                        //if (closedRoute.Length > 1)
                        {
                            calced = true;
                            //if (closedRoute.Length > 3)
                            //{
                            //    string test = "dffd";
                            //}
                            //if (closedRoute.end != route.Count - 1)
                            {
                                time = provider.GetDistanceBetweenNodes(
                               route[closedRoute.start].Node, route[closedRoute.end].Node, currentTime, closedRoute.id);
                                if (time.TravelTime < TimeSpan.Zero || time.WaitingTime < TimeSpan.Zero)
                                {
                                    throw new Exception(String.Format("Negitive time encountered. ({0}/{1}) between {2} and {3}.", time.TravelTime, time.WaitingTime, route[closedRoute.start].Node, route[closedRoute.end].Node));
                                }
                            }
                           /*
							else
                            {
                                time = provider.GetDistanceBetweenNodes(
                               route[closedRoute.start].Node, route[closedRoute.end - 1].Node, currentTime, closedRoute.id);

                                if (time.TravelTime < TimeSpan.Zero || time.WaitingTime < TimeSpan.Zero)
                                {
                                    throw new Exception("Negitive time encountered.");
                                }
                            }
                           
							 */

                        }
                        if (time == default(TransportTimeSpan))
                        {
                            calced = false;
                            time =
                                properties.PointDataProviders[0].EstimateDistance(
                                    (Location)route[closedRoute.start].Node, (Location)route[closedRoute.end].Node).Time;
                            if (time == default(TransportTimeSpan))
                            {
                                Logger.Log(this,"Loop detetected");
                                
                                //throw new Exception("Walking time is zero. An error must of occurred.");
                            }
                        }

                        if (first || (time.TotalTime < minTime.TotalTime || (closedRoute.Length >= maxLength && calced)))
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

                if (bestClosedRoute.id != -2)
                {
                    if (firstService)
                    {
                        firstService = false;
                        minTime.WaitingTime = TimeSpan.Zero;
                    }
                    
                    if (minTime.TravelTime < TimeSpan.Zero || minTime.WaitingTime < TimeSpan.Zero)
                    {
                        throw new Exception("Negitive time encountered.");
                    }
                    totalTime += minTime;
                    if (totalTime.TravelTime < TimeSpan.Zero || totalTime.WaitingTime < TimeSpan.Zero)
                    {
                        throw new Exception("Negitive time encountered.");
                    }
                    currentTime += minTime.TotalTime;


                    /*
                    if (bestClosedRoute.Equals(default(ClosedRoute)))
                    {
                        throw new Exception("No minimum route found.");

                    }
                    if (bestClosedRoute.end <= pointer)
                    {
                        throw new Exception("Infinite loop detected.");
                    }

                
                    if (bestClosedRoute.Length == 1)
                    {
                        bestClosedRoute.id = -1;
                    }
                    */


                    for (int i = pointer; i <= bestClosedRoute.end; i++)
                    {
                        route[i].CurrentRoute = bestClosedRoute.id;
                        route[i].TotalTime = totalTime.TotalTime;
                        if (i > 0 && route[i].TotalTime < route[i - 1].TotalTime)
                        {
                            throw new Exception(String.Format("Negitive time encountered. ({0}/{1})", route[i].TotalTime, route[i - 1].TotalTime));
                        }

                    }
                    pointer = bestClosedRoute.end;



                    if (bestClosedRoute.id != -1)
                    {
                        route[bestClosedRoute.start].Node.RetrieveData();
                        switch (route[bestClosedRoute.start].Node.TransportType)
                        {
                            case "Train":
                                totalTrain++;
                                break;
                            case "Tram":
                                totalTram++;
                                break;
                            case "Bus":
                                totalBus++;
                                break;
                        }
                        legs++;
                    }
                    else
                    {
                        fitness.WalkingTime += minTime.TotalTime;
                    }
                    
                    Console.WriteLine(
                        "[{0}] : [{1}] -> [{2}] (W: {3} T: {4})",
                        bestClosedRoute.id,
                        bestClosedRoute.start,
                        bestClosedRoute.end,
                        minTime.WaitingTime,
                        minTime.TravelTime);

                    
                     
                }
                else
                {
                    
                    
                    pointer = bestClosedRoute.end;
                    if (minTime.TravelTime < TimeSpan.Zero || minTime.WaitingTime < TimeSpan.Zero)
                    {
                        throw new Exception(String.Format("Negitive time encountered. ({0}/{1})", minTime.TravelTime, minTime.WaitingTime));
                    }
                    totalTime += minTime;
                    if (totalTime.TravelTime < TimeSpan.Zero || totalTime.WaitingTime < TimeSpan.Zero)
                    {
                        throw new Exception(String.Format("Negitive time encountered. ({0}/{1})", minTime.TravelTime, minTime.WaitingTime));
                    }
                    currentTime += minTime.TotalTime;
                    
                    Console.WriteLine(
                      "[{0}] : [{1}] -> [{2}] (W: {3} T: {4}) ({5}M)",
                      "Walk",
                      bestClosedRoute.start,
                      bestClosedRoute.end,
                      minTime.WaitingTime,
                      minTime.TravelTime, Math.Round(GeometryHelper.GetStraightLineDistance((Location)route[bestClosedRoute.start].Node,(Location)route[bestClosedRoute.end].Node)*1000.0,0));
                     
                    fitness.WalkingTime += minTime.TotalTime;
                }


                if (bestClosedRoute.id == -1 || bestClosedRoute.id == -2)
                {
                    fitness.JourneyLegs.Add(
                       new JourneyLeg(
                           "Walk",
                           (MetlinkNode)route[bestClosedRoute.start].Node,
                           (MetlinkNode)route[bestClosedRoute.end].Node, currentTime - minTime.TotalTime,
                           minTime.TotalTime,
                           bestClosedRoute.id.ToString(CultureInfo.InvariantCulture)));

                }
                else
                {
                    fitness.JourneyLegs.Add(
                       new JourneyLeg(
                           route[bestClosedRoute.start].Node.TransportType,
                           (MetlinkNode)route[bestClosedRoute.start].Node,
                           (MetlinkNode)route[bestClosedRoute.end].Node, currentTime - minTime.TotalTime,
                           minTime.TotalTime,
                           bestClosedRoute.id.ToString(CultureInfo.InvariantCulture)));
                }
               
                //Console.WriteLine("[t] : [{1}] -> [{2}] (W: {3} T: {4})", bestClosedRoute.id, bestClosedRoute.end, bestClosedRoute.end+1, minTime.WaitingTime, minTime.TravelTime);
                //initialDepart += minTime.TotalTime;
                //if (bestClosedRoute.id != -1)
                //{
                    
                //}

            }

            route.Last().TotalTime = route[route.Count - 2].TotalTime;
            //writer.Close();
            //Console.WriteLine("Total Time: {0}", totalTime);
            //Console.WriteLine("------------------------------");
            fitness.TotalTravelTime = totalTime.TravelTime;
            fitness.TotalWaitingTime = totalTime.WaitingTime;
            fitness.TotalJourneyTime = totalTime.TotalTime;
            fitness.Changes = legs;
            //fitness.PercentBuses = new[] { totalBus, totalTrain, totalTram }.Max() / (double) legs;
            
            fitness.PercentBuses = (double)totalBus / legs;
            fitness.PercentTrains = (double)totalTrain / legs;
            fitness.PercentTrams = (double)totalTram / legs;
            //Console.WriteLine("Evaluated fitness: {0}" , fitness);

            return fitness;
          
        }

        #endregion



        #region Methods

        #endregion
    }
}