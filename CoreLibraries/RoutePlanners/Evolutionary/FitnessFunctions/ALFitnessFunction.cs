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

    using NUnit.Framework;

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

        private double EPSILON = 0.0001;

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
            
restart:
            //TODO: Remove this line
            //initialDepart = properties.DepartureTime;


            var fitness = new Fitness();
                var nodeDict = new Dictionary<int, int>();

            INetworkDataProvider provider = properties.NetworkDataProviders[0];
            
            var openRoutes = new List<int>();
            var closedRoutes = new List<int>();
            var closedRoutesIndex = new List<List<ClosedRoute>>();
            var routeIndex = new Dictionary<int, int>();


            for (int i = 0; i <= route.Count; i++)
            {
               
                var routes = new List<int>();

                if (i < route.Count)
                {
                    if (nodeDict.ContainsKey(route[i].Node.Id))
                    {
                        Logger.Log(this, "Loop detected! Removing and restarting...");
                        int index = nodeDict[route[i].Node.Id];
                        route.RemoveRange(index, i - index);
                        goto restart;
                    }

                    nodeDict.Add(route[i].Node.Id, i);
                    
                    route[i].Node.RetrieveData();
                    //Console.Write("{0:00000}[{1}]: ", route[i].Id,((MetlinkNode)route[i]).StopSpecName);
                    closedRoutesIndex.Add(new List<ClosedRoute>());
                    routes = provider.GetRoutesForNode(route[i].Node);

                    foreach (int routeId in routes)
                    {
                        //Console.Write("{0:00000}, ", routeId);
                        if (!openRoutes.Contains(routeId)) // && !closedRoutes.Contains(routeId))
                        {
                            openRoutes.Add(routeId);
                            routeIndex[routeId] = i;

                        }
                    }
                    //Console.WriteLine();
                }
                var newOpenRoute = new List<int>();
                foreach (var openRoute in openRoutes)
                {
                    if (!routes.Contains(openRoute))
                    {
                        closedRoutes.Add(openRoute);
                        var cr = new ClosedRoute { end = i-1, id = openRoute, start = routeIndex[openRoute] };
                        
                        
                        if (cr.Length >= 1)
                        {
                            /*
                            if (route[cr.start].Node.Id == route[cr.end].Node.Id)
                            {
                                Logger.Log(this, "Loop detected! Removing and restarting...");
                                route.RemoveRange(cr.start, cr.end - cr.start);
                                goto restart;
                            }
                             * */
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
            

            var pointer = 0;
            var totalTime = default(TransportTimeSpan);
            int legs = 0;
            int totalBus = 0;
            int totalTrain = 0;
            int totalTram = 0;
            double totalDistance = 0;
            DateTime departTime = initialDepart;
       
            //Console.WriteLine("-------UnifiedFitnessScore Evaluation-------");
            while (pointer < route.Count - 1)
            {
                var t = closedRoutesIndex[pointer];
                TransportMode mode = TransportMode.Unknown;
                
                var bestArcs = t.Select(cr => provider.GetDistanceBetweenNodes(route[cr.start].Node, route[cr.end].Node, departTime).FirstOrDefault()).ToList();

                bestArcs = (from arc in bestArcs where arc != default(Arc) select arc).ToList();

                if (!bestArcs.Any())
                {
                    mode = TransportMode.Walking;
                    var longest = (from cr in t
                                  where cr.Length == t.Max(i => i.Length)
                                  select cr).FirstOrDefault();
                    if (longest.Equals(default(ClosedRoute)))
                    {
                        
                        //If there are no closed routes here, just walk to the next node.
                        bestArcs.Add(
                         properties.PointDataProviders[0].EstimateDistance(
                             (Location)route[pointer].Node,
                             (Location)route[pointer+1].Node));
                    }
                    else
                    {
                        //Calculate the walking disance
                        bestArcs.Add(
                        properties.PointDataProviders[0].EstimateDistance(
                            (Location)route[longest.start].Node,
                            (Location)route[longest.end].Node));
                    }
                    

                }



                bestArcs =
                    (from a in bestArcs where Math.Abs(a.Distance - bestArcs.Max(i => i.Distance)) < EPSILON select a).
                        ToList();

                var bestArc = (from a in bestArcs
                                  where a.Time.TotalTime == bestArcs.Min(i => i.Time.TotalTime)
                                  select a).First();
                

                if (mode == TransportMode.Unknown)
                {
                    mode = route[pointer].Node.TransportType;

                }
                if (mode != TransportMode.Walking)
                {
                    switch (route[pointer].Node.TransportType)
                    {
                        case TransportMode.Train:
                            totalTrain++;
                            break;
                        case TransportMode.Bus:
                            totalBus++;
                            break;
                        case TransportMode.Tram:
                            totalTram++;
                            break;
                        default:
                            break;
                    }

                }
                fitness.JourneyLegs.Add(
                    new JourneyLeg(
                        mode,
                        (MetlinkNode)bestArc.Source,
                        (MetlinkNode)bestArc.Destination, departTime, 
                        bestArc.Time.TotalTime,bestArc.RouteId.ToString()));

                if (mode != TransportMode.Walking)
                {
                    legs++;
                }
                totalTime += bestArc.Time;
                departTime += bestArc.Time.TotalTime;
                totalDistance += GeometryHelper.GetStraightLineDistance(bestArc.Source, bestArc.Destination);
                Assert.That(totalTime.TotalTime != TimeSpan.Zero, "Last arc was zero time.");
                Assert.That(departTime != default(DateTime), "DepartTime is zero.");

                Assert.IsFalse(route[pointer].Node.Id == ((INetworkNode)bestArc.Destination).Id,
                    "Destination is source. There must be a loop.");

                //Advance pointer
                while (route[pointer].Node.Id != ((INetworkNode)bestArc.Destination).Id)
                {
                    pointer++;
                }

                Assert.IsTrue(pointer < route.Count,"Route pointer has overflowed.");
            }

            //route.Last().TotalTime = route[route.Count - 2].TotalTime;
            //writer.Close();
            //Console.WriteLine("Total Time: {0}", totalTime);
            //Console.WriteLine("------------------------------");
            fitness.TotalTravelTime = totalTime.TravelTime;
            fitness.TotalWaitingTime = totalTime.WaitingTime;
            fitness.TotalJourneyTime = totalTime.TotalTime;
            fitness.TotalDistance = totalDistance;
            fitness.Changes = legs;
            //fitness.PercentBuses = new[] { totalBus, totalTrain, totalTram }.Max() / (double) legs;
            if (legs != 0)
            {
                fitness.PercentBuses = (double)totalBus / legs;
                fitness.PercentTrains = (double)totalTrain / legs;
                fitness.PercentTrams = (double)totalTram / legs;

            }
            else
            {
                fitness.PercentBuses = 0;
                fitness.PercentTrains = 0;
                fitness.PercentTrams = 0;
            }
          
            double totalPercent = fitness.PercentBuses + fitness.PercentTrains + fitness.PercentTrams;

            
            Assert.That(totalPercent <= 1.01);
            //Console.WriteLine("Evaluated fitness: {0}" , fitness);

            return fitness;
          
        }

        #endregion



        #region Methods

        #endregion
    }
}