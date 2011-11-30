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
        private DFSRoutePlanner planner;

        public INetworkNode Current
        {
            get;
            set;
        }
            
        public INetworkNode Best
        {
            get;
            set;
        }

        public RouteSolver()
        {
            planner = new DFSRoutePlanner();
        }

        public void Reset(Location a, Location b)
        {
            List<INetworkNode> list = new List<INetworkNode>();
            TerminalNode start = new TerminalNode("Start", a.Latitude, a.Longitude);
            TerminalNode end = new TerminalNode("End", b.Latitude, b.Longitude);
            list.Add(start);
            list.Add(end);
            planner.Start(list);
        
        
        
        }

        public bool NextStep()
        {
            return planner.SolveStep();
        }

}