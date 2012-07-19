// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using RmitJourneyPlanner.CoreLibraries.Comparers;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   Finds the best route between nodes using evolutionary algorithms.
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
        /// The result of each iteration.
        /// </summary>
        private Result result = default(Result);

        /// <summary>
        ///   The population of the evolutionary algorithm.
        /// </summary>
        private List<Critter> population;

        private int progress;

        private int targetProgress;

        /// <summary>
        /// The current iteration of the optimisation.
        /// </summary>
        private int iteration = 0;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="EvolutionaryRoutePlanner" /> class.
        /// </summary>
        /// <param name="properties"> The <see cref="EvolutionaryProperties" /> object containing the properties of the run. </param>
        public EvolutionaryRoutePlanner(EvolutionaryProperties properties)
        {
            this.properties = properties;
        }

        #endregion

        #region Public Properties

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

        /// <summary>
        /// The result of each iteration.
        /// </summary>
        public Result Result
        {
            get
            {
                return this.result;
            }
        }

        public int Progress
        {
            get
            {
                return this.progress;
            }
        }

        public int TargetProgress
        {
            get
            {
                return this.targetProgress;
            }
        }

        /// <summary>
        /// The current iteration of the optimisation.
        /// </summary>
        public int Iteration
        {
            get
            {
                return this.iteration;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Register a network data provider to use with the route planning.
        /// </summary>
        /// <param name="provider"> The network provider to register. </param>
        public void RegisterNetworkDataProvider(INetworkDataProvider provider)
        {
            this.properties.NetworkDataProviders.Add(provider);
        }

        /// <summary>
        ///   Register a point to point data provider for use with the route planning.
        /// </summary>
        /// <param name="provider"> The point to point data provider to register. </param>
        public void RegisterPointDataProvider(IPointDataProvider provider)
        {
            this.properties.PointDataProviders.Add(provider);
        }
        
        /// <summary>
        ///   Repairs a route by taking out duplicates and loops.
        /// </summary>
        /// <param name="route"> The route to repair </param>
        public Route RepairRoute(Route route)
        {
            var newNodes = new List<NodeWrapper<INetworkNode>>();
            List<NodeWrapper<INetworkNode>> oldNodes = route;
            for (int i = 0; i < oldNodes.Count; i++)
            {
                int count = 0;
                int index = -1;
                for (int j = 0; j < oldNodes.Count; j++)
                {
                    INetworkNode node = oldNodes[j].Node;
                    if (node.Equals(oldNodes[i]))
                    {
                        index = j;
                        count++;
                    }
                }

                if (count > 1)
                {
                    newNodes.Add(oldNodes[i]);
                    if (index == -1)
                    {
                        throw new Exception("Houston we have a problem!");
                    }

                    i = index;
                }
                else
                {
                    newNodes.Add(oldNodes[i]);
                }
            }

            var newRoute = new Route(route.Id);
            newRoute.AddRange(newNodes);
            return newRoute;
        }

        /// <summary>
        ///   Solve the next iteration of the algorithm.
        /// </summary>
        /// <returns> The solve step. </returns>
        public bool SolveStep()
        {
            this.iteration = this.Iteration + 1;
            
            Console.WriteLine("Solving step {0}...", this.Iteration);
            this.progress = 0;
            
            var routesUsed = new Dictionary<int, int>();
            
            var sw = Stopwatch.StartNew();
            var eliteCritters = new List<Critter>(this.properties.NumberToKeep);
            var newCritters = new List<Critter>(this.properties.PopulationSize - this.properties.NumberToKeep);
            eliteCritters.AddRange(this.Population.GetRange(0, this.properties.NumberToKeep));

            var matingPool = new List<Critter>();
            
            // Begin tournament selection
            for (int i = 0; i < this.properties.PopulationSize - this.properties.NumberToKeep; i ++)
            {
                var first = (Critter)this.population[this.random.Next(this.population.Count - 1)].Clone();
                var second = (Critter)this.population[this.random.Next(this.population.Count - 1)].Clone();
                var surviver = first.UnifiedFitnessScore > second.UnifiedFitnessScore ? second : first;
                matingPool.Add(surviver);
            }

            for (int i = 0; i < this.properties.PopulationSize - this.properties.NumberToKeep; i += 2)
            {
                var first = (Critter)matingPool[this.random.Next(matingPool.Count - 1)].Clone();
                var second = (Critter)matingPool[this.random.Next(matingPool.Count - 1)].Clone();

                var endPoints = new KeyValuePair<int, int>[2];
                endPoints[0] = new KeyValuePair<int, int>(first.Route.First().Node.Id, first.Route.Last().Node.Id);
                endPoints[1] = new KeyValuePair<int, int>(second.Route.First().Node.Id, second.Route.Last().Node.Id);

                bool doCrossover = this.random.NextDouble() <= this.properties.CrossoverRate;
                bool doMutation = this.random.NextDouble() <= this.properties.MutationRate;
                Critter[] children = doCrossover
                                         ? this.properties.Breeder.Crossover(first, second)
                                         : new[] { first, second };


                if (doMutation)
                {
                    children[0] = this.properties.Mutator.Mutate(children[0]);
                    children[1] = this.properties.Mutator.Mutate(children[1]);
                }



                children[0].Fitness = this.properties.FitnessFunction.GetFitness(children[0].Route);
                var ff = (AlFitnessFunction)this.properties.FitnessFunction;
                foreach (int routeUsed in ff.RoutesUsed)
                {
                    if (!routesUsed.ContainsKey(routeUsed))
                    {
                        routesUsed.Add(routeUsed, 1);
                    }
                    else
                    {
                        routesUsed[routeUsed]++;
                    }

                }
                children[1].Fitness = this.properties.FitnessFunction.GetFitness(children[1].Route);
                
                foreach (int routeUsed in ff.RoutesUsed)
                {
                    if (!routesUsed.ContainsKey(routeUsed))
                    {
                        routesUsed.Add(routeUsed, 1);
                    }
                    else
                    {
                        routesUsed[routeUsed]++;
                    }

                }
               // this.RepairRoute(children[0].Route);
               // this.RepairRoute(children[1].Route);


                newCritters.AddRange(children);
               

                progress++;
            }

            this.Population.Clear();
            this.Population.AddRange(newCritters);

            foreach (var elite in eliteCritters)
            {
                this.properties.FitnessFunction.GetFitness(elite.Route);
                progress++;
                var ff = (AlFitnessFunction)this.properties.FitnessFunction;
                foreach (int routeUsed in ff.RoutesUsed)
                {
                    if (!routesUsed.ContainsKey(routeUsed))
                    {
                        routesUsed.Add(routeUsed, 1);
                    }
                    else
                    {
                        routesUsed[routeUsed]++;
                    }

                }
            }

            /*
            foreach (var eliteCritter in eliteCritters)
            {
                Tools.ToLinkedNodes(eliteCritter.Route.GetNodes(true));
            }
            */
            this.Population.AddRange(eliteCritters);
            this.Population.Sort(new CritterComparer());
            sw.Stop();
            this.result.Totaltime = sw.Elapsed;
            this.result.DiversityMetric = routesUsed.Keys.Count;
            foreach (var critter in this.Population)
            {
                this.result.AverageFitness += critter.Fitness;
            }
            this.result.AverageFitness /= population.Count;
            this.result.MinimumFitness = this.Population[0].Fitness;


            //Tools.SavePopulation(this.population.GetRange(0, 25), ++this.generation, this.properties);

            //this.BestNode = Tools.ToLinkedNodes(this.Population[0].Route);

            Console.WriteLine("Average fitness: {0}", this.result.MinimumFitness);
            return false;
        }

        /// <summary>
        ///   Start solving a route
        /// </summary>
        public void Start()
        {
            this.population = new List<Critter>();
            this.InitPopulation();
            this.iteration = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Create the initial population.
        /// </summary>
        private void InitPopulation()
        {
            
            this.population.Clear();
            var sw = Stopwatch.StartNew();
            var routesUsed = new Dictionary<int, int>();

            targetProgress = this.properties.PopulationSize;
            

            for (int i = 0; i < this.properties.PopulationSize; i++)
            {
                progress = i + 1;
                Route route = null;
                while (route == null)
                {
                    route = this.properties.RouteGenerator.Generate(
                                (INetworkNode)this.properties.Origin.Clone(),
                                (INetworkNode)this.properties.Destination.Clone(),
                                this.properties.DepartureTime);
                }

                
                var critter = new Critter(route, this.properties.FitnessFunction.GetFitness(route));
                Logging.Logger.Log(this, "Member {0}, fitness {1}, total nodes {2}", i,critter.UnifiedFitnessScore,critter.Route.Count);
                this.result.AverageFitness += critter.Fitness;
                var ff = (AlFitnessFunction)this.properties.FitnessFunction;
                foreach (int routeUsed in ff.RoutesUsed)
                {
                    if (!routesUsed.ContainsKey(routeUsed))
                    {
                        routesUsed.Add(routeUsed,1);
                    }
                    else
                    {
                        routesUsed[routeUsed]++;
                    }

                }

                this.Population.Add(critter);
            }
            
            this.Population.Sort(new CritterComparer());

            sw.Stop();
            this.result.Totaltime = sw.Elapsed;
            this.result.DiversityMetric = routesUsed.Keys.Count;
            this.result.AverageFitness /= this.properties.PopulationSize;
            Console.WriteLine("---EVALULATING FITTEST MEMBER---");
            this.result.MinimumFitness = this.properties.FitnessFunction.GetFitness(this.population[0].Route);
            this.result.Population = this.population;
            Console.WriteLine("------------------------");
            //Tools.SavePopulation(this.population, 0, this.properties);
            //this.BestNode = Tools.ToLinkedNodes(this.Population[0].Route);
        }

        #endregion
    }
}