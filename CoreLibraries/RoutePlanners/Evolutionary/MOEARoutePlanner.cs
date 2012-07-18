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
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   Finds the best route between nodes using evolutionary algorithms.
    /// </summary>
    public class MoeaRoutePlanner : IRoutePlanner
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
        private readonly Result result = new Result();

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

        private List<List<Critter>> f = new List<List<Critter>>(); 

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="EvolutionaryRoutePlanner" /> class.
        /// </summary>
        /// <param name="properties"> The <see cref="EvolutionaryProperties" /> object containing the properties of the run. </param>
        public MoeaRoutePlanner(EvolutionaryProperties properties)
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

        public List<List<Critter>> Fronts
        {
            get
            {
                return this.f;
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
        [ObsoleteAttribute]
        public Route RepairRoute(Route route)
        {
            /*
			var newNodes = new List<INetworkNode>();
            List<INetworkNode> oldNodes = route;
            for (int i = 0; i < oldNodes.Count; i++)
            {
                int count = 0;
                int index = -1;
                for (int j = 0; j < oldNodes.Count; j++)
                {
                    INetworkNode node = oldNodes[j];
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
            */
			throw new NotSupportedException("This method is depricated.");
        }

        /// <summary>
        /// Returns if a critter dominates another critter.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        private bool IsDominated(Critter c1, Critter c2)
        {
            var dominated = false;
            var flags = new []{false,false};
            for (int i = 0; i < c1.Fitness.Length; i++)
            {
                if (c1.Fitness[i] < c2.Fitness[i] )
                {
                    flags[0] = true;
                }
                else if (c1.Fitness[i] > c2.Fitness[i])
                {
                    flags[1] = true;
                }
            }

            if (flags[0] && !flags[1]) 
            {
                dominated = true;
            }

            return dominated;


        }


        private void nonDominatedSort(List<Critter> r)
        {
            var s = new Dictionary<Critter, List<Critter>>();
            f = new List<List<Critter>>();
           
            foreach (var p in r)
            {
                s[p] = new List<Critter>();
                p.N = 0;
                foreach (var q in r)
                {
                    if (p == q)
                        continue;
                    
                    if (this.IsDominated(p,q))
                    {
                        s[p].Add(q);
                    }
                    else if (this.IsDominated(q,p))
                    {
                        p.N++;
                    }
                }

                if (p.N == 0)
                {
                    p.Rank = 1;
                    f.Add(new List<Critter>());
                    f[0].Add(p);
                }
            }

            int i = 0;
            while (f[i].Any())
            {
                var Q = new List<Critter>();
                foreach (var p in f[i])
                {
                    foreach(var q in s[p])
                    {
                        q.N--;
                        if (q.N == 0)
                        {
                            q.Rank = i + 1;
                            Q.Add(q);
                        }
                            
                    }
                }
                i++;
                f.Add(new List<Critter>());
                f[i] = Q;
            }

           
        }

        private void crowdingDistanceAssignment(List<Critter> X)
        {
            int l = X.Count;
            foreach (var x in X)
            {
                x.Distance = 0;
            }
            for (int i = 0 ; i < Fitness.ParameterCount; i++)
            {
                X.Sort((x, y) => x.Fitness[i].CompareTo(y.Fitness[i]));
                X[0].Distance = Double.PositiveInfinity;
                X[l - 1].Distance = Double.PositiveInfinity;
                for (int j = 1; j < l-1; j++)
                {
                    X[j].Distance += (X[j + 1].Fitness[i] - X[j - 1].Fitness[i]);
                }

            }
        }


        private static int CompareCritters(Critter x, Critter y)
        {
            if (x==y)
            {
                return 0;
            }
            if (x == null)
            {
                if (y == null)
                {
                    // If x is null and y is null, they're
                    // equal. 
                    return 0;
                }
                // If x is null and y is not null, y
                // is greater. 
                return 1;
            }
            if (x.Rank < y.Rank || (x.Rank == y.Rank && x.Distance > y.Distance))
            {
                return 1;
            }
            return -1;
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
            
            var sw = Stopwatch.StartNew();
           
            var q = new List<Critter>(this.properties.PopulationSize);
            foreach (var x in f)
            {
                x.Clear();
            }
            f.Clear();
            var matingPool = new List<Critter>();

          
            
            for (int i = 0; i < this.properties.PopulationSize/2; i ++)
            {
                var first = (Critter)this.population[this.random.Next(this.population.Count - 1)].Clone();
                var second = first;
                while (second == first)
                {
                    second = (Critter)this.population[this.random.Next(this.population.Count - 1)].Clone();
                }
                
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


                if (doCrossover || doMutation)
                {
                    children[0].Fitness = this.properties.FitnessFunction.GetFitness(children[0].Route);
                    children[1].Fitness = this.properties.FitnessFunction.GetFitness(children[1].Route);
                }
                //var ff = (AlFitnessFunction)this.properties.FitnessFunction;
                

                q.AddRange(children);

                progress++;
            }

           
            
            var r = this.population.Select(p => (Critter)p.Clone()).ToList();

           

            //r.AddRange(this.population);
            r.AddRange(q);

            

            double maxTime = r.Max(c => c.Fitness.TotalJourneyTime).TotalHours;
            double minTime = r.Min(c => c.Fitness.TotalJourneyTime).TotalHours;
            int maxChanges = r.Max(c => c.Fitness.Changes);
            int minChanges = r.Min(c => c.Fitness.Changes);

            foreach (var c in r)
            {
                c.Fitness.NormalisedTravelTime = c.Fitness.TotalJourneyTime.TotalHours / (maxTime - minTime);
                c.Fitness.NormalisedChanges = (double) c.Fitness.Changes / (maxChanges - minChanges);
                Console.WriteLine("Critter normalised values: tt: {0}, ch: {1}",c.Fitness.NormalisedTravelTime,c.Fitness.NormalisedChanges);
                c.N = 0;
                c.Rank = 0;
                c.Distance = 0;
            }

            

            this.nonDominatedSort(r);

            this.population.Clear();
            int j = 0;

            while (this.population.Count + this.Fronts[j].Count <= this.properties.PopulationSize)
            {
                this.crowdingDistanceAssignment(this.Fronts[j]);
                this.population.AddRange(this.Fronts[j]);
                j++;
            }


            
           
            this.Fronts[j].Sort(CompareCritters);
            /*
            this.Fronts[j].Sort((x, y) =>
                {
                    if (x.Rank < y.Rank || (x.Rank == y.Rank && x.Distance > y.Distance))
                    {
                        return 1;
                    }
                    return -1;
                });
            */
            this.population.AddRange(this.Fronts[j].GetRange(0, this.properties.PopulationSize-this.population.Count));

            

           /*
            foreach (var eliteCritter in eliteCritters)
            {
                Tools.ToLinkedNodes(eliteCritter.Route.GetNodes(true));
            }
            */
            
            sw.Stop();
            this.result.Totaltime = sw.Elapsed;
            /*
            foreach (var critter in this.Population)
            {
                this.result.AverageFitness += critter.Fitness;
            }
            this.result.AverageFitness /= population.Count;
            this.result.MinimumFitness = this.Population[0].Fitness;
            */

            //Tools.SavePopulation(this.population.GetRange(0, 25), ++this.generation, this.properties);

            //this.BestNode = Tools.ToLinkedNodes(this.Population[0].Route);

            //Console.WriteLine("Average fitness: {0}", this.result.MinimumFitness);
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
                    route = (Route)this.properties.RouteGenerator.Generate(
                        (INetworkNode)this.properties.Origin.Clone(),
                        (INetworkNode)this.properties.Destination.Clone(),
                        this.properties.DepartureTime).Clone();
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