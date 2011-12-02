// -----------------------------------------------------------------------
// <copyright file="RouteSolver.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using RmitJourneyPlanner.CoreLibraries;
using RmitJourneyPlanner.CoreLibraries.Positioning;
using RmitJourneyPlanner.CoreLibraries.DataProviders;
using System.Collections.Generic;
namespace WebInterface
{
    /// <summary>
    /// Global object to solve routes.
    /// </summary>
    public static class RouteSolver
    {
        private static DFSRoutePlanner planner;
        private static bool ready = false;
        private static int iteration = 0;

        public static INetworkNode Current
        {
            get;
            set;
        }

        public static INetworkNode Best
        {
            get;
            set;
        }

        public static bool Ready
        {
            get
            {
                return ready;
            }
            
        }

        public static int CurrentIteration
        {
            get
            {
                return iteration;
            }

        }

        static RouteSolver()
        {
            RmitJourneyPlanner.CoreLibraries.DataAccess.ConnectionInfo.Proxy =
                new System.Net.WebProxy("http://aproxy.rmit.edu.au:8080", false, null, new System.Net.NetworkCredential("s3229159", "MuchosRowlies1"));
            planner = new DFSRoutePlanner();
            planner.RegisterNetworkDataProvider(new TramNetworkProvider());
            planner.RegisterPointDataProvider(new WalkingDataProvider());
        }

        public static void Reset(Location a, Location b, double maxWalk)
        {
            List<INetworkNode> list = new List<INetworkNode>();
            TerminalNode start = new TerminalNode("Start", a.Latitude, a.Longitude);
            TerminalNode end = new TerminalNode("End", b.Latitude, b.Longitude);
            list.Add(start);
            list.Add(end);
            planner.Start(list);
            planner.MaxWalkingDistance = maxWalk;
            iteration = 0;
            ready = true;



        }

        public static bool NextStep()
        {
            iteration++;
            bool success = planner.SolveStep();
            Current = planner.Current;
            Best = planner.BestNode;
            return success;

        }
    }
}