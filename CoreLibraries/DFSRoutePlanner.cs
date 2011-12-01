// -----------------------------------------------------------------------
// <copyright file="DFSRoutePlanner.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DataProviders;
    using Types;
    using Positioning;

    /// <summary>
    /// Finds the best route between given nodes using a depth first search.
    /// </summary>
    public class DFSRoutePlanner : IRoutePlanner
    {
        private List<INetworkDataProvider> networkProviders = new List<INetworkDataProvider>();
        private List<IPointDataProvider> pointDataProviders = new List<IPointDataProvider>();
        private Caching.ArcCache aCache = new Caching.ArcCache("RoutePlanner");

        private Stack<INetworkNode> stack;
        private List<DataProviders.INetworkNode> itinerary;
        private TimeSpan minTimeSolved;
        private INetworkNode minNode;
        private DateTime startTime;
        private INetworkNode current = null;

        /// <summary>
        /// Gets the current node being traversed.
        /// </summary>
        public INetworkNode Current
        {
            get
            {
                return current;
            }
        }

        /// <summary>
        /// Gets the node of the best path found so far.
        /// </summary>
        public INetworkNode BestNode
        {
            get
            {
                return minNode;
            }
        }
        
        /// <summary>
        /// This event fires on every iteration of the algorithm.
        /// </summary>
        public event EventHandler<NextIterationEventArgs> NextIterationEvent;

        //public event EventHandler

        /// <summary>
        /// Register a new data provider to use in the travel planning.
        /// </summary>
        /// <param name="provider"></param>
        public void RegisterNetworkDataProvider(DataProviders.INetworkDataProvider provider)
        {
            networkProviders.Add(provider);
        }


        /// <summary>
        /// Register a new point to point data provider to use in travel planning.
        /// </summary>
        /// <param name="provider"></param>
        public void RegisterPointDataProvider(DataProviders.IPointDataProvider provider)
        {
            pointDataProviders.Add(provider);
        }

        private bool hasDuplicates(INetworkNode current, INetworkNode next)
        {
            if (current != null)
            {
                while (current.Parent != null)
                {
                    if (current.ID == next.ID)
                    {
                        return true;
                    }
                    current = current.Parent;
                }
            }
            return false;
        }

        private bool hasBeenOnRoute(INetworkNode current, INetworkNode next)
        {
            if (current != null)
            {
                while (current.Parent != null)
                {
                    if (current.CurrentRoute == next.CurrentRoute)
                    {
                        return true;
                    }
                    current = current.Parent;
                    
                }
            }
            return false;
        }

        
        
        /// <summary>
        /// Start solving a route
        /// </summary>
        /// <param name="itinerary"></param>
        /// <returns></returns>
        public void Start(List<DataProviders.INetworkNode> itinerary)
        {

            this.itinerary = itinerary;
            stack = new Stack<INetworkNode>();
            itinerary.First().TotalTime = new TimeSpan(0, 0, 0);
            minTimeSolved = TimeSpan.MaxValue;
            minNode = null;
            stack.Push(itinerary.First());
            startTime = DateTime.Parse("11/30/2011 11:37 AM");
            current = null;
            //while (stack.Count > 0)
            //{


                
            //}

        }

        /// <summary>
        /// Solve the next iteration of the algorithm.
        /// </summary>
        /// <returns></returns>
        public bool SolveStep()
        {
            INetworkNode next = stack.Pop();
            while (hasDuplicates(current, next))
            {
                next = stack.Pop();
            }
            current = next;
            if (current == itinerary.Last())
            {
                if (current.TotalTime < minTimeSolved)
                {
                    minTimeSolved = current.TotalTime;
                    minNode = current;
                    //continue;
                }
            }
            current.RetrieveData();

            if (NextIterationEvent != null)
            {
                NextIterationEvent(this, new NextIterationEventArgs(current));
            }

            TramNetworkProvider tProvider = (TramNetworkProvider)networkProviders[0];
            WalkingDataProvider wProvider = (WalkingDataProvider)pointDataProviders[0];

            List<INetworkNode> areaNodes = tProvider.GetNodesAtLocation((Location)current, 1.0);
            areaNodes.Add(itinerary.Last());
            List<Arc> pArcs = new List<Arc>();
            foreach (INetworkNode node in areaNodes)
            {
                if (node != current && current.CurrentRoute != node.CurrentRoute && !hasBeenOnRoute(current, node))
                {
                    node.RetrieveData();
                    pArcs.Add(wProvider.EstimateDistance((Location)current, (Location)node));
                }
            }

            if (!(current is TerminalNode))
            {

                List<INetworkNode> routeNodes = tProvider.GetAdjacentNodes(current, current.CurrentRoute);
                foreach (INetworkNode node in routeNodes)
                {
                    node.RetrieveData();
                    pArcs.AddRange(tProvider.GetDistanceBetweenNodes(current, node, startTime + current.TotalTime));
                }

            }

            // pArcs.Sort(new Comparers.ArcComparer());
            
            List<INetworkNode> destinations = new List<INetworkNode>();
            foreach (Arc arc in pArcs)
            {
                INetworkNode destination = (INetworkNode)((INetworkNode)arc.Destination).Clone();
                ;
                destination.Parent = current;
                destination.TotalTime = current.TotalTime + arc.Time;

                if (destination.TotalTime < minTimeSolved)
                {
                    destination.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)destination, (Location)itinerary.Last());
                    //destination.CurrentRoute = arc.RouteId;
                    destinations.Add(destination);
                }

            }

            destinations.Sort(new Comparers.NodeComparer());

            foreach (INetworkNode node in destinations)
            {
                stack.Push(node);
            }

            return (stack.Count > 0);
        }
    }
}
