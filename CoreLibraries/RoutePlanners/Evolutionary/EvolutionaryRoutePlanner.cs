// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="EvolutionaryRoutePlanner.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Finds the best route between nodes using evolutionary algorithms.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using RmitJourneyPlanner.CoreLibraries.Comparers;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Finds the best route between nodes using evolutionary algorithms.
    /// </summary>
    public class EvolutionaryRoutePlanner : IRoutePlanner
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
        ///   The random.
        /// </summary>
        private readonly Random random = new Random();

        /// <summary>
        /// The start time.
        /// </summary>
        private readonly DateTime startTime = default(DateTime);

        /// <summary>
        ///   The distance walked.
        /// </summary>
        private double distanceWalked;

        /// <summary>
        ///   The itinerary.
        /// </summary>
        private List<INetworkNode> itinerary;

        /// <summary>
        ///   The population of the evolutionary algorithm.
        /// </summary>
        private List<Critter> population;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EvolutionaryRoutePlanner"/> class.
        /// </summary>
        /// <param name="departureTime">
        /// The departure Time.
        /// </param>
        public EvolutionaryRoutePlanner(DateTime departureTime)
        {
            this.ProbMinDistance = 0.7;
            this.ProbMinTransfers = 0.2;
            this.MaxWalkingTime = new TimeSpan(0, 0, 25, 0);
            this.PopulationSize = 30;
            this.startTime = departureTime;
            this.ElitePop = 15;
            this.MutationRate = 0.1;

        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the best node found so far.
        /// </summary>
        public INetworkNode BestNode { get; protected set; }

        /// <summary>
        ///   Gets the current node being traversed.
        /// </summary>
        public INetworkNode Current
        {
            get
            {
                return this.BestNode;
            }
        }

        /// <summary>
        ///   Gets or sets the amount of best routes in the population to propogate to the next generation.
        /// </summary>
        public int ElitePop { get; set; }

        /// <summary>
        ///   Gets or set the maximum walking time allowed.
        /// </summary>
        public TimeSpan MaxWalkingTime { get; set; }

        /// <summary>
        ///   Gets or sets the probability that a new child is mutated.
        /// </summary>
        public double MutationRate { get; set; }

        /// <summary>
        ///   Gets or set the population size of the evolutionary algorithm.
        /// </summary>
        public int PopulationSize { get; set; }

        /// <summary>
        ///   Gets or sets the proability of chosing a node with the 
        ///   shortest distance when initilizing.
        /// </summary>
        public double ProbMinDistance { get; set; }

        /// <summary>
        ///   Gets or sets the probability of transfering when 
        ///   initilizing
        /// </summary>
        public double ProbMinTransfers { get; set; }

        /// <summary>
        ///   The population of the evolutionary algorithm.
        /// </summary>
        public List<Critter> Population
        {
            get
            {
                return this.population;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Register a network data provider to use with the route planning.
        /// </summary>
        /// <param name="provider">
        /// The network provider to register.
        /// </param>
        public void RegisterNetworkDataProvider(INetworkDataProvider provider)
        {
            this.networkProviders.Add(provider);
        }

        /// <summary>
        /// Register a point to point data provider for use with the route planning.
        /// </summary>
        /// <param name="provider">
        /// The point to point data provider to register.
        /// </param>
        public void RegisterPointDataProvider(IPointDataProvider provider)
        {
            this.pointDataProviders.Add(provider);
        }

        /// <summary>
        /// Solve the next iteration of the algorithm.
        /// </summary>
        /// <returns>
        /// The solve step.
        /// </returns>
        public bool SolveStep()
        {

            var eliteCritters = new List<Critter>(this.ElitePop);
            var newCritters = new List<Critter>(this.PopulationSize - this.ElitePop);
            eliteCritters.AddRange(this.Population.GetRange(0, this.ElitePop));

            for (int i = 0; i < this.PopulationSize - this.ElitePop; i += 2)
            {
                Critter[] children = null;
                while (children == null)
                {
                     Critter first = eliteCritters[this.random.Next(eliteCritters.Count - 1)];
                     Critter second = eliteCritters[this.random.Next(eliteCritters.Count - 1)];
                
                    children = this.Crossover(first, second);
                }   
                if (this.random.NextDouble() <= this.MutationRate)
                {
                    children[0] = this.Mutate(children[0]);
                    children[1] = this.Mutate(children[1]);
                }

                newCritters.AddRange(children);
            }
            this.Population.Clear();
            this.Population.AddRange(newCritters);
            this.Population.AddRange(eliteCritters);
            this.Population.Sort(new CritterComparer());
            /*
            INetworkNode prev = null;

            // TODO: Desperatly need to fix the fact that get nodes is opposite of insertion!!!
            foreach (var node in this.Population[0].Route.GetNodes(false))
            {
                INetworkNode newNode = (INetworkNode)node.Clone();
                if (prev == null)
                {
                    prev = newNode;
                }
                else
                {
                    newNode.Parent = prev;
                    prev = newNode;
                }
            }
            */
            this.BestNode = ToLinkedNodes(this.Population[0].Route.GetNodes(false));

            return false;
        }

        /// <summary>
        /// Converts a list of nodes into a linked list of nodes.
        /// </summary>
        /// <param name="nodes">The nodes to convert.</param>
        /// <returns>A node that is the head of the linked list.</returns>
        public static INetworkNode ToLinkedNodes(List<INetworkNode> nodes)
        {
            INetworkNode prev = null;

            // TODO: Desperatly need to fix the fact that get nodes is opposite of insertion!!!
            foreach (var node in nodes)
            {
                INetworkNode newNode = (INetworkNode)node.Clone();
                if (prev == null)
                {
                    prev = newNode;
                }
                else
                {
                    newNode.Parent = prev;
                    prev = newNode;
                }
            }
            return prev;
        }

        /// <summary>
        /// Start solving a route
        /// </summary>
        /// <param name="itinerary">
        /// The list of nodes to plan a journey between.
        /// </param>
        public void Start(List<INetworkNode> itinerary)
        {
            // this.InitPopulation(itinerary);
            this.itinerary = itinerary;

            this.population = new List<Critter>();
            this.InitPopulation();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Applies crossover to 2 parents to create a child.
        /// </summary>
        /// <param name="first">
        /// The first parent of the crossover.
        /// </param>
        /// <param name="second">
        /// The second parent of the crossover.
        /// </param>
        /// <returns>
        /// If the operation is successful then the result is returned, otherwise null.
        /// </returns>
        private Critter[] Crossover(Critter first, Critter second)
        {
            var firstNodes = first.Route.GetNodes(false);
            var secondNodes = second.Route.GetNodes(false);
            var crossoverPoints = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < firstNodes.Count; i++)
            {
                for (int j = 0; j < secondNodes.Count; j++)
                {
                    if (firstNodes[i].Equals(secondNodes[j]) && !(firstNodes[i] is TerminalNode) && !(secondNodes[j] is TerminalNode))
                    {
                        crossoverPoints.Add(new KeyValuePair<int, int>(i, j));
                        break;
                    }

                }

            }
            if (crossoverPoints.Count == 0)
            {
                return null;
            }

            var firstChild = new Route(Guid.NewGuid().ToString());
            var secondChild = new Route(Guid.NewGuid().ToString());
            var crossoverPoint = crossoverPoints[this.random.Next(crossoverPoints.Count - 1)];

            firstChild.AddNodeRange(firstNodes.GetRange(0, crossoverPoint.Key), true);
            firstChild.AddNodeRange(secondNodes.GetRange(crossoverPoint.Value, (secondNodes.Count - crossoverPoint.Value)), true);

            secondChild.AddNodeRange(secondNodes.GetRange(0, crossoverPoint.Value), true);
            secondChild.AddNodeRange(
                firstNodes.GetRange(crossoverPoint.Key, firstNodes.Count - crossoverPoint.Key), true);

            return new Critter[]
                {
                    new Critter(
                        firstChild, 
                        this.GetFitness(firstChild)), 
                    new Critter(
                        secondChild, 
                        this.GetFitness(secondChild))
                };



        }

        /// <summary>
        /// Generates a random route between source and destination nodes.
        /// </summary>
        /// <returns>
        /// A random route.
        /// </returns>
        private Route GenerateRandomRoute(INetworkNode source, INetworkNode destination)
        {
            INetworkNode current = source;
            Route currentRoute = new Route(Guid.NewGuid().ToString());
            currentRoute.AddNode(source, true);

            while (!current.Equals(destination))
            {
                double minDistance = double.MaxValue;
                INetworkNode minNode = null;
                double maxWalkDistance = this.MaxWalkingTime.TotalHours * 6.0;

                var candidateNodes = this.networkProviders[0].GetNodesAtLocation(
                    (Location)source, maxWalkDistance);

                if (
                    this.pointDataProviders[0].EstimateDistance((Location)current, (Location)destination).
                        Distance < maxWalkDistance)
                {
                    candidateNodes.Add(destination);
                }
               
                if (candidateNodes.Count == 0)
                {
                    throw new Exception("This node has no neighbors.");

                }

                if (!(current is TerminalNode))
                {
                    candidateNodes.AddRange(this.networkProviders[0].GetAdjacentNodes(current, current.CurrentRoute));
                }

                var transferNodes = new List<INetworkNode>();

                List<INetworkNode> tempCandidates = new List<INetworkNode>(candidateNodes);
                candidateNodes.Clear();
                candidateNodes.AddRange(
                    tempCandidates.Where(candidateNode => !currentRoute.GetNodes(false).Contains(candidateNode)));

                if (candidateNodes.Count == 0)
                {
                    candidateNodes.Add(destination);

                }

                // Calculate minimum distance node
                foreach (var candidateNode in candidateNodes)
                {
                    candidateNode.RetrieveData();
                    candidateNode.EuclidianDistance =
                        this.pointDataProviders[0].EstimateDistance(
                            (Location)candidateNode, (Location)destination).Distance;
                    if (candidateNode.EuclidianDistance < minDistance)
                    {
                        minNode = candidateNode;
                        minDistance = candidateNode.EuclidianDistance;
                    }
                }

                candidateNodes.Remove(minNode);

                // Find nodes that are a transfer.
                foreach (var candidateNode in candidateNodes)
                {
                    if (!(current is TerminalNode))
                    {
                        if (candidateNode.CurrentRoute != current.CurrentRoute)
                        {
                            transferNodes.Add(candidateNode);
                        }
                    }
                }

                candidateNodes.RemoveAll(transferNodes.Contains);

                double p = this.random.NextDouble();

                if (p <= this.ProbMinDistance || (transferNodes.Count == 0 && candidateNodes.Count == 0))
                {
                    /*
                    if (minNode == null)
                    {
                        //throw new NullReferenceException("There are no routes left to take!");
                        minNode = destination;
                    }
                     * */
                    currentRoute.AddNode(minNode, true);

                    current = minNode;
                }
                else if ((p <= this.ProbMinDistance + this.ProbMinTransfers || candidateNodes.Count == 0)
                         && transferNodes.Count != 0)
                {
                    int index = this.random.Next(0, transferNodes.Count - 1);
                    currentRoute.AddNode(transferNodes[index], true);
                    current = transferNodes[index];
                }
                else
                {
                    int index = this.random.Next(0, candidateNodes.Count - 1);
                    currentRoute.AddNode(candidateNodes[index], true);
                    current = candidateNodes[index];
                }

                
                // this.Current = current;
               
            }

            return currentRoute;
        }

        /// <summary>
        /// Returns a value representing the fitness of the route.
        /// </summary>
        /// <param name="route">
        /// The route the is to be evaluated.
        /// </param>
        /// <returns>
        /// A double value representing the fitness.
        /// </returns>
        private double GetFitness(Route route)
        {
            List<INetworkNode> nodes = route.GetNodes(false);
            TimeSpan totalTime = new TimeSpan();

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                List<Arc> arcs =
                    this.networkProviders[0].GetDistanceBetweenNodes(
                        nodes[i], nodes[i + 1], this.startTime + totalTime);
                if (arcs.Count > 0)
                {
                    if (i > 0 && nodes[i].CurrentRoute != nodes[i - 1].CurrentRoute)
                    {
                        totalTime = totalTime.Add(new TimeSpan(0, 0, 0, 30));
                    }
                    totalTime = totalTime.Add(arcs[0].Time);
                }
                else
                {
                    totalTime =
                        totalTime.Add(
                            this.pointDataProviders[0].EstimateDistance((Location)nodes[i], (Location)nodes[i + 1]).Time);
                }
            }

            return totalTime.Ticks / 100000.0;
        }

        /// <summary>
        /// Create the initial population.
        /// </summary>
        private void InitPopulation()
        {
            for (int i = 0; i < this.PopulationSize; i++)
            {
                Route route = null;
                while (route == null)
                {
                    route = this.GenerateRandomRoute(this.itinerary.First(), this.itinerary.Last());
                }
                var critter = new Critter(route, this.GetFitness(route));
                this.Population.Add(critter);
            
            }
            this.Population.Sort(new CritterComparer());
            this.BestNode = ToLinkedNodes(this.Population[0].Route.GetNodes(false));
            

        }

        /// <summary>
        /// Mutates a critter
        /// </summary>
        /// <param name="child">
        /// The critter that is to be mutated.
        /// </param>
        /// <returns>
        /// A mutated critter.
        /// </returns>
        private Critter Mutate(Critter child)
        {
            List<INetworkNode> nodes = child.Route.GetNodes(false);
            int startIndex = this.random.Next(0, nodes.Count - 2);
            int endIndex = this.random.Next(startIndex + 1, nodes.Count - 1);
            INetworkNode begin = nodes[startIndex];
            INetworkNode end = nodes[endIndex];
            Route newSegment = this.GenerateRandomRoute(begin, end);
            Route newRoute = new Route(Guid.NewGuid().ToString());
            newRoute.AddNodeRange(nodes.GetRange(0, startIndex), true);
            newRoute.AddNodeRange(newSegment.GetNodes(false), true);
            newRoute.AddNodeRange(nodes.GetRange(endIndex+1,nodes.Count - 1 - endIndex), true);

            return new Critter(newRoute, this.GetFitness(newRoute));
        }

        #endregion
    }
}