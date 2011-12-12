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
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Finds the best route between nodes using evolutionary algorithms.
    /// </summary>
    public class EvolutionaryRoutePlanner : IRoutePlanner
    {
        #region Constants and Fields

        /// <summary>
        ///   Contains the state of the evolutionary route planner.
        /// </summary>
        private readonly EvolutionaryProperties properties;

        /// <summary>
        ///   The random.
        /// </summary>
        private readonly Random random = new Random();

        /// <summary>
        ///   The population of the evolutionary algorithm.
        /// </summary>
        private List<Critter> population;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EvolutionaryRoutePlanner"/> class.
        /// </summary>
        /// <param name="properties">
        /// The <see cref="EvolutionaryProperties"/> object containing the properties of the run.
        /// </param>
        public EvolutionaryRoutePlanner(EvolutionaryProperties properties)
        {
            this.properties = properties;
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
            this.properties.NetworkDataProviders.Add(provider);
        }

        /// <summary>
        /// Register a point to point data provider for use with the route planning.
        /// </summary>
        /// <param name="provider">
        /// The point to point data provider to register.
        /// </param>
        public void RegisterPointDataProvider(IPointDataProvider provider)
        {
            this.properties.PointDataProviders.Add(provider);
        }

        /// <summary>
        /// Solve the next iteration of the algorithm.
        /// </summary>
        /// <returns>
        /// The solve step.
        /// </returns>
        public bool SolveStep()
        {
            var eliteCritters = new List<Critter>(this.properties.NumberToKeep);
            var newCritters = new List<Critter>(this.properties.PopulationSize - this.properties.NumberToKeep);
            eliteCritters.AddRange(this.Population.GetRange(0, this.properties.NumberToKeep));

            for (int i = 0; i < this.properties.PopulationSize - this.properties.NumberToKeep; i += 2)
            {
                Critter[] children = null;
                while (children == null)
                {
                    Critter first = (Critter)eliteCritters[this.random.Next(eliteCritters.Count - 1)].Clone();
                    Critter second = (Critter)eliteCritters[this.random.Next(eliteCritters.Count - 1)].Clone();

                    children = this.properties.Breeder.Crossover(first, second);
                }

                if (this.random.NextDouble() <= this.properties.MutationRate)
                {
                    children[0] = this.Mutate(children[0]);
                    children[1] = this.Mutate(children[1]);
                }

                Tools.ToLinkedNodes(children[0].Route.GetNodes(true));
                Tools.ToLinkedNodes(children[1].Route.GetNodes(true));
                newCritters.AddRange(children);
            }

            this.Population.Clear();
            this.Population.AddRange(newCritters);
            foreach (var eliteCritter in eliteCritters)
            {
                Tools.ToLinkedNodes(eliteCritter.Route.GetNodes(true));
            }

            this.Population.AddRange(eliteCritters);
            this.Population.Sort(new CritterComparer());

            this.BestNode = Tools.ToLinkedNodes(this.Population[0].Route.GetNodes(true));

            return false;
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
            this.properties.Origin = itinerary.First();
            this.properties.Destination = itinerary.Last();

            this.population = new List<Critter>();
            this.InitPopulation();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create the initial population.
        /// </summary>
        private void InitPopulation()
        {
            for (int i = 0; i < this.properties.PopulationSize; i++)
            {
                Route route = null;
                while (route == null)
                {
                    route = this.properties.RouteGenerator.Generate(
                        (INetworkNode)this.properties.Origin.Clone(), (INetworkNode)this.properties.Destination.Clone());
                }

                var critter = new Critter(route, this.properties.FitnessFunction.GetFitness(route));
                this.Population.Add(critter);
            }

            this.Population.Sort(new CritterComparer());
            this.BestNode = Tools.ToLinkedNodes(this.Population[0].Route.GetNodes(true));
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
            List<INetworkNode> nodes = child.Route.GetNodes(true);
            int startIndex = this.random.Next(0, nodes.Count - 2);
            int endIndex = this.random.Next(startIndex + 1, nodes.Count - 1);
            INetworkNode begin = nodes[startIndex];
            INetworkNode end = nodes[endIndex];
            Route newSegment = this.properties.RouteGenerator.Generate(begin, end);
            Route newRoute = new Route(Guid.NewGuid().ToString());
            newRoute.AddNodeRange(nodes.GetRange(0, startIndex), true);
            newRoute.AddNodeRange(newSegment.GetNodes(true), true);
            newRoute.AddNodeRange(nodes.GetRange(endIndex + 1, nodes.Count - 1 - endIndex), true);

            return new Critter(newRoute, this.properties.FitnessFunction.GetFitness(newRoute));
        }

        #endregion
    }
}