// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteSolver.cs" company="RMIT University">
//    Copyright RMIT University 2011
// </copyright>
// <summary>
//   Global object to solve routes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.WebInterface.App_Data
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using RmitJourneyPlanner.CoreLibraries;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.Types;

    /// <summary>
    /// Global object to solve routes.
    /// </summary>
    public static class RouteSolver
    {
        #region Constants and Fields
        
        /// <summary>
        /// The planner.
        /// </summary>
        private static readonly EvolutionaryRoutePlanner Planner;

        /// <summary>
        /// The iteration.
        /// </summary>
        private static int iteration;

        /// <summary>
        /// The ready.
        /// </summary>
        private static bool ready;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="RouteSolver"/> class.
        /// </summary>
        static RouteSolver()
        {
            CoreLibraries.DataAccess.ConnectionInfo.Proxy =
                new WebProxy(
                    "http://aproxy.rmit.edu.au:8080", 
                    false, 
                    null, 
                    new NetworkCredential("s3229159", "MuchosRowlies1"));
            Planner = new EvolutionaryRoutePlanner(DateTime.Parse("11/30/2011 11:37 AM"));
            Planner.RegisterNetworkDataProvider(new TramNetworkProvider());
            Planner.RegisterPointDataProvider(new WalkingDataProvider());
            population = new List<Critter>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Best.
        /// </summary>
        public static INetworkNode Best { get; set; }

        /// <summary>
        /// Gets or sets Current.
        /// </summary>
        public static INetworkNode Current { get; set; }

        /// <summary>
        /// Gets or sets population.
        /// </summary>
        public static List<Critter> population
        {
            get; set;
        } 

        /// <summary>
        /// Gets CurrentIteration.
        /// </summary>
        public static int CurrentIteration
        {
            get
            {
                return iteration;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Ready.
        /// </summary>
        public static bool Ready
        {
            get
            {
                return ready;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes the next iteration of the <see cref="IRoutePlanner"/> object.
        /// </summary>
        /// <returns>
        /// The result of the iteration.
        /// </returns>
        public static bool NextStep()
        {
            iteration++;
            bool success = Planner.SolveStep();
            Current = Planner.Current;
            Best = Planner.BestNode;
            population = Planner.Population;
            return success;
        }

        /// <summary>
        /// Resets the algorithm.
        /// </summary>
        /// <param name="source">
        /// The first location.
        /// </param>
        /// <param name="destination">
        /// The second location.
        /// </param>
        /// <param name="maxWalk">
        /// The maximium walking distance in kilometers.
        /// </param>
        public static void Reset(Location source, Location destination, double maxWalk)
        {
            var list = new List<INetworkNode>();
            var start = new TerminalNode("Start", source.Latitude, source.Longitude);
            var end = new TerminalNode("End", destination.Latitude, destination.Longitude);
            list.Add(start);
            list.Add(end);
            Planner.Start(list);
            //Planner.MaxWalkingDistance = maxWalk;
            //Planner.MaxWalkingTime
            iteration = 0;
            ready = true;
            Best = Planner.BestNode;
            population = Planner.Population;

        }

        #endregion
    }
}