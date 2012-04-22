// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Traditional
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using C5;

    using RmitJourneyPlanner.CoreLibraries.Comparers;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   Finds the best route between given nodes using a depth first search.
    /// </summary>
    public class AStarRoutePlanner : IRoutePlanner
    {
        #region Constants and Fields

        /// <summary>
        ///   The network providers.
        /// </summary>
        private readonly List<INetworkDataProvider> networkProviders = new List<INetworkDataProvider>();

        /// <summary>
        ///   The point data providers.
        /// </summary>
        private readonly List<IPointDataProvider> pointDataProviders = new List<IPointDataProvider>();

        /// <summary>
        ///   The start time.
        /// </summary>
        private readonly DateTime startTime;

        // private Caching.ArcCache aCache = new Caching.ArcCache("RoutePlanner");

        /// <summary>
        ///   The closed set of nodes.
        /// </summary>
        private List<INetworkNode> closed;

        /// <summary>
        ///   The current node that is being explored.
        /// </summary>
        private INetworkNode current;

        /// <summary>
        ///   The itinerary for the route planner.
        /// </summary>
        private List<INetworkNode> itinerary;

        /// <summary>
        ///   The min node.
        /// </summary>
        private INetworkNode minNode;

        /// <summary>
        ///   The min time solved.
        /// </summary>
        private TimeSpan minTimeSolved;

        /// <summary>
        ///   The p queue.
        /// </summary>
        private IntervalHeap<INetworkNode> pQueue;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="AStarRoutePlanner" /> class. Initilizes a new instance of a AStarRoutePlanner.
        /// </summary>
        /// <param name="departureTime"> The departure Time. </param>
        public AStarRoutePlanner(DateTime departureTime)
        {
            this.MaxWalkingDistance = 1.0;
            this.startTime = departureTime;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///   This event fires on every iteration of the algorithm.
        /// </summary>
        public event EventHandler<NextIterationEventArgs> NextIterationEvent;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the node of the best path found so far.
        /// </summary>
        public INetworkNode BestNode
        {
            get
            {
                return this.minNode;
            }
        }

        /// <summary>
        ///   Gets the current node being traversed.
        /// </summary>
        public INetworkNode Current
        {
            get
            {
                return this.current;
            }
        }

        /// <summary>
        ///   Gets or sets the max radius to search in around the current node to find another node of a different route or network.
        /// </summary>
        public double MaxWalkingDistance { get; set; }

        /// <summary>
        ///   Gets the population of this route planner.
        /// </summary>
        public List<Critter> Population
        {
            get
            {
                return new List<Critter>
                    { this.BuildCritterFromNode(this.BestNode), this.BuildCritterFromNode(this.Current) };
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Register a new data provider to use in the travel planning.
        /// </summary>
        /// <param name="provider"> A <see cref="INetworkDataProvider" /> object. </param>
        public void RegisterNetworkDataProvider(INetworkDataProvider provider)
        {
            this.networkProviders.Add(provider);
        }

        /// <summary>
        ///   Register a new point to point data provider to use in travel planning.
        /// </summary>
        /// <param name="provider"> A <see cref="IPointDataProvider" /> object. </param>
        public void RegisterPointDataProvider(IPointDataProvider provider)
        {
            this.pointDataProviders.Add(provider);
        }

        /// <summary>
        ///   Solve the next iteration of the algorithm.
        /// </summary>
        /// <returns> The solve step. </returns>
        public bool SolveStep()
        {
            INetworkNode next = this.pQueue.DeleteMin();

            /*
            while (this.pQueue.Count > 0 && this.closed.Contains(next))
            {
                // hasDuplicates(current, next))
                next = this.pQueue.DeleteMin();
            }
            */
            this.current = next;
            this.closed.Add(this.current);

            if (this.closed.Contains(this.itinerary.Last()))
            {
                this.minNode = this.current;
                return true;
            }

            TimeSpan timeToEnd = this.current.TotalTime
                                 +
                                 this.pointDataProviders[0].EstimateDistance(
                                     (Location)this.current, (Location)this.itinerary.Last()).Time;

            if (timeToEnd < this.minTimeSolved)
            {
                var end = (INetworkNode)this.itinerary.Last().Clone();
                end.Parent = this.current;
                end.TotalTime = timeToEnd;
                this.minTimeSolved = timeToEnd;
                this.minNode = end;

                // continue;
            }

            // this.current.RetrieveData();
            if (this.NextIterationEvent != null)
            {
                this.NextIterationEvent(this, new NextIterationEventArgs(this.current));
            }

            INetworkDataProvider tProvider = this.networkProviders[0];
            IPointDataProvider wProvider = this.pointDataProviders[0];

            List<INetworkNode> areaNodes = tProvider.GetNodesAtLocation((Location)this.current, 1.5);

            // areaNodes.Add(itinerary.Last());
            var pArcs = (from node in areaNodes
                         where !node.Equals(this.current) && !this.hasBeenOnRoute(this.current, node)
                         where (node is TerminalNode) || !this.current.CurrentRoute.Equals(node.CurrentRoute)
                         select wProvider.EstimateDistance((Location)this.current, (Location)node)).ToList();

            if (!(this.current is TerminalNode))
            {
                List<INetworkNode> routeNodes = tProvider.GetAdjacentNodes(this.current, this.current.CurrentRoute);
                foreach (INetworkNode node in routeNodes)
                {
                    // node.RetrieveData();
                    if (! this.hasBeenOnRoute(this.current.Parent, node))
                    {
                        pArcs.AddRange(
                            tProvider.GetDistanceBetweenNodes(
                                this.current, node, this.startTime + this.current.TotalTime));
                    }
                }
            }

            // pArcs.Sort(new Comparers.ArcComparer());
            var destinations = new List<INetworkNode>();
            foreach (Arc arc in pArcs)
            {
                // INetworkNode destination = (INetworkNode)((INetworkNode)arc.Destination).Clone();
                var destination = (INetworkNode)arc.Destination;

                if (!this.closed.Contains(destination))
                {
                    // bool same = (destination.Equals( (INetworkNode)arc.Destination));
                    destination.Parent = this.current;
                    destination.TotalTime = this.current.TotalTime + arc.Time;
                    {
                        // if (destination.TotalTime < minTimeSolved)
                        destination.EuclidianDistance = GeometryHelper.GetStraightLineDistance(
                            (Location)destination, (Location)this.itinerary.Last());

                        // destination.CurrentRoute = arc.RouteId;
                        destinations.Add(destination);
                    }
                }
            }

            // destinations.Sort(new Comparers.NodeComparer());
            foreach (INetworkNode node in destinations)
            {
                this.pQueue.Add(node);
            }

            return !(this.pQueue.Count > 0);
        }

        /// <summary>
        ///   Start solving a route
        /// </summary>
        public void Start()
        {
            this.itinerary = itinerary;
            this.pQueue = new IntervalHeap<INetworkNode>(new NodeComparer());
            itinerary.First().TotalTime = new TimeSpan(0, 0, 0);
            this.closed = new List<INetworkNode>();
            this.minTimeSolved = TimeSpan.MaxValue;
            this.minNode = null;
            this.pQueue.Add(itinerary.First());

            this.current = null;

            // while (pQueue.Count > 0)
            // {

            // }
        }

        #endregion

        /*
        /// <summary>
        /// The has duplicates.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="next">
        /// The next.
        /// </param>
        /// <returns>
        /// The has duplicates.
        /// </returns>
        private bool hasDuplicates(INetworkNode current, INetworkNode next)
        {
            if (current != null)
            {
                while (current.Parent != null)
                {
                    if (current.Id.Equals(next.Id))
                    {
                        return true;
                    }

                    current = current.Parent;
                }
            }

            return false;
        }
*/

        #region Methods

        /// <summary>
        ///   Builds a critter from a root node.
        /// </summary>
        /// <param name="node"> The node to build the critter from. </param>
        /// <returns> The critter. </returns>
        private Critter BuildCritterFromNode(INetworkNode node)
        {
            var route = new Route(Guid.NewGuid().GetHashCode());
            var nodes = new List<INetworkNode>();
            while (node != null)
            {
                nodes.Add(node);
                node = node.Parent;
            }

            nodes.Reverse();
            route.AddRange(nodes);

            return new Critter(route, 0.0);
        }

        /// <summary>
        ///   The has been on route.
        /// </summary>
        /// <param name="current"> The current. </param>
        /// <param name="next"> The next. </param>
        /// <returns> The has been on route. </returns>
        private bool hasBeenOnRoute(INetworkNode current, INetworkNode next)
        {
            if (current != null)
            {
                while (current.Parent != null)
                {
                    if (current.CurrentRoute.Equals(next.CurrentRoute))
                    {
                        return true;
                    }

                    current = current.Parent;
                }
            }

            return false;
        }

        #endregion
    }
}