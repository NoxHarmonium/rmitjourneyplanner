// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EARoutePlanner.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Finds the best route between nodes using evolutionary algorithms.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using NPack;

    using NUnit.Framework;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Logging;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions;
    using RmitJourneyPlanner.CoreLibraries.Types;

    using Random = RmitJourneyPlanner.CoreLibraries.Random;

    #endregion

    // 1/(e^x²)
    //// TODO: Take out all the multiobjective references.
    
    /// <summary>
    /// Finds the best route between nodes using evolutionary algorithms.
    /// </summary>
    public class EaRoutePlanner : IRoutePlanner
    {
        #region Constants and Fields

        /// <summary>
        ///   The epsilon.
        /// </summary>
        private const double Epsilon = double.Epsilon;

        /// <summary>
        ///   Contains the state of the evolutionary route planner.
        /// </summary>
        private readonly EvolutionaryProperties properties;

        /// <summary>
        ///   The random.
        /// </summary>
        private readonly MersenneTwister random = Random.GetInstance();

        /// <summary>
        ///   The result of each iteration.
        /// </summary>
        private readonly Result result = new Result();

        /// <summary>
        ///   The f.
        /// </summary>
        private List<List<Critter>> f = new List<List<Critter>>();

        /// <summary>
        ///   The current iteration of the optimisation.
        /// </summary>
        private int iteration;

        /// <summary>
        ///   The population of the evolutionary algorithm.
        /// </summary>
        private Population[] population;

        /// <summary>
        ///   The progress.
        /// </summary>
        private int progress;

        /// <summary>
        ///   The target progress.
        /// </summary>
        private int targetProgress;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EaRoutePlanner"/> class.
        /// </summary>
        /// <param name="properties">
        /// The <see cref="EvolutionaryProperties"/> object containing the properties of the run. 
        /// </param>
        public EaRoutePlanner(EvolutionaryProperties properties)
        {
            this.properties = properties;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the fronts associated with non-dominated sorting.
        /// </summary>
        public List<List<Critter>> Fronts
        {
            get
            {
                return this.f;
            }
        }

        /// <summary>
        ///   Gets the current iteration of the optimisation.
        /// </summary>
        public int Iteration
        {
            get
            {
                return this.iteration;
            }
        }

        /// <summary>
        ///   Gets the result of the last iteration.
        /// </summary>
        public Result IterationResult
        {
            get
            {
                return this.result;
            }
        }

        /// <summary>
        ///   Gets the population of the evolutionary algorithm.
        /// </summary>
        public Population Population
        {
            get
            {
                return this.population[0];
            }
        }

        /// <summary>
        ///   Gets Progress.
        /// </summary>
        public int Progress
        {
            get
            {
                return this.progress;
            }
        }

        /// <summary>
        ///   Gets the the object representing all the properties of this route planner./>
        /// </summary>
        public EvolutionaryProperties Properties
        {
            get
            {
                return this.properties;
            }
        }

        /// <summary>
        ///   Gets the result of each iteration.
        /// </summary>
        public Result Result
        {
            get
            {
                return this.result;
            }
        }

        /// <summary>
        ///   Gets the target progress value.
        /// </summary>
        public int TargetProgress
        {
            get
            {
                return this.targetProgress;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns if a critter dominates another critter.
        /// </summary>
        /// <param name="c1">
        /// The first critter under comparison.
        /// </param>
        /// <param name="c2">
        /// The second critter under comparison.
        /// </param>
        /// <returns>
        /// True if c1 dominates c2, otherwise false.
        /// </returns>
        public bool Dominates(Critter c1, Critter c2)
        {
            var dominated = false;
            var flags = new[] { false, false };
            foreach (FitnessParameter t in this.properties.Objectives)
            {
                if (c1.Fitness[t] < c2.Fitness[t])
                {
                    flags[0] = true;
                }
                else if (c1.Fitness[t] > c2.Fitness[t])
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

        /// <summary>
        /// Register a network data provider to use with the route planning.
        /// </summary>
        /// <param name="provider">
        /// The network provider to register. 
        /// </param>
        public void RegisterNetworkDataProvider(INetworkDataProvider provider)
        {
            // this.Properties.NetworkDataProviders.Add(provider);
            this.Properties.NetworkDataProviders = new[] { provider };
        }

        /// <summary>
        /// Register a point to point data provider for use with the route planning.
        /// </summary>
        /// <param name="provider">
        /// The point to point data provider to register. 
        /// </param>
        public void RegisterPointDataProvider(IPointDataProvider provider)
        {
            this.Properties.PointDataProviders = new[] { provider };
        }

        /// <summary>
        /// Solve the next iteration of the algorithm.
        /// </summary>
        public void SolveStep()
        {
            // checkPopDupes();
            if (this.properties.Objectives.Length < 1)
            {
                throw new Exception("You cannot optimise with 0 objectives.");
            }

            this.iteration = this.Iteration + 1;

            Console.WriteLine("Solving step {0}...", this.Iteration);

            this.progress = 0;

            var sw = Stopwatch.StartNew();

            // var r = new Population(population[0].Union(population[1]));

            /*
            foreach (var c in r)
            {
                double totalDistance = 0.0;
                foreach (var c2 in r)
                {
                    /*
                    totalDistance += Math.Max(Math.Max(Math.Abs(c.Fitness.PercentBuses - c2.Fitness.PercentBuses) ,
                                     Math.Abs(c.Fitness.PercentTrains - c2.Fitness.PercentTrains)) ,
                                     Math.Abs(c.Fitness.PercentTrams - c2.Fitness.PercentTrams));
                    

                    totalDistance += Math.Sqrt(Math.Pow(c.Fitness.PercentBuses - c2.Fitness.PercentBuses,2.0)+
                                   Math.Pow(c.Fitness.PercentTrains - c2.Fitness.PercentTrains,2.0) +
                                    Math.Pow(c.Fitness.PercentTrams - c2.Fitness.PercentTrams,2.0));
                }
                c.Fitness.DiversityMetric = totalDistance;
            }


            double jtMaxTime = r.Max(c => c.Fitness.TotalJourneyTime).TotalHours;
            double jtMinTime = r.Min(c => c.Fitness.TotalJourneyTime).TotalHours;
            double ttMaxTime = r.Max(c => c.Fitness.TotalDistance);
            double ttMinTime = r.Min(c => c.Fitness.TotalDistance);
            double minDM = r.Min(c => c.Fitness.DiversityMetric);
            double maxDM = r.Max(c => c.Fitness.DiversityMetric);
            int maxChanges = r.Max(c => c.Fitness.Changes);
            int minChanges = r.Min(c => c.Fitness.Changes);

           


            foreach (var c in r)
            {
                c.Fitness.NormalisedJourneyTime = (c.Fitness.TotalJourneyTime.TotalHours-jtMinTime) / (jtMaxTime - jtMinTime);
                c.Fitness.NormalisedChanges = (double)(c.Fitness.Changes-minChanges) / (maxChanges - minChanges);
                c.Fitness.NormalisedTravelTime = (c.Fitness.TotalDistance-ttMinTime) / (ttMaxTime - ttMinTime);
                c.Fitness.DiversityMetric = (c.Fitness.DiversityMetric-minDM)/(maxDM - minDM);

                
                
                if (double.IsInfinity(c.Fitness.NormalisedChanges))
                {
                    c.Fitness.NormalisedChanges = 0;
                }
                if ((double.IsInfinity(c.Fitness.NormalisedJourneyTime)))
                {
                    c.Fitness.NormalisedJourneyTime = 0;
                }
                if ((double.IsInfinity(c.Fitness.NormalisedTravelTime)))
                {
                    c.Fitness.NormalisedTravelTime = 0;
                }

                   if ((double.IsInfinity(c.Fitness.DiversityMetric)))
                {
                    c.Fitness.DiversityMetric = 0;
                }
                 
                Console.WriteLine("{0}, {1},{2},{3}", c.Fitness.NormalisedJourneyTime, c.Fitness.NormalisedChanges, c.Fitness.PercentBuses, c.Fitness.PercentTrains);
                c.N = 0;
                c.Rank = 0;
                //c.Distance = 0;
            }


            this.nonDominatedSort(r);
            
            var ranks = r.GroupBy(g => g.Rank);
            foreach (var rank in ranks)
            {
                foreach (var c1 in rank)
                {
                    foreach (var c2 in rank)
                    {
                        Assert.That(!this.Dominates(c1, c2), "Members of a front should be non dominate to each other.");

                    }
                }

            }
            */
            this.population[1] = new Population();

            for (int i = 0; i < this.Properties.PopulationSize / 2; i++)
            {
                var first = this.TournamentSelect();
                var second = this.TournamentSelect();

                Assert.That(first.DepartureTime != default(DateTime) && second.DepartureTime != default(DateTime));

                bool doCrossover = this.random.NextDouble() <= this.Properties.CrossoverRate;
                bool doMutation = this.random.NextDouble() <= this.Properties.MutationRate;
                Critter[] children = doCrossover
                                         ? this.Properties.Breeder.Crossover(first, second)
                                         : new[] { first, second };

                if (doMutation)
                {
                    children[0] = this.Properties.Mutator.Mutate(children[0]);
                    children[1] = this.Properties.Mutator.Mutate(children[1]);
                }

                Assert.That(
                    children[0].DepartureTime != default(DateTime) && children[1].DepartureTime != default(DateTime));

                if (doCrossover || doMutation)
                {
                    children[0].Fitness = this.Properties.FitnessFunction.GetFitness(
                        children[0].Route, children[0].DepartureTime);
                    children[1].Fitness = this.Properties.FitnessFunction.GetFitness(
                        children[1].Route, children[1].DepartureTime);
                }

                // var ff = (AlFitnessFunction)this.properties.FitnessFunction;
                Assert.That(
                    children[0].DepartureTime != default(DateTime) && children[1].DepartureTime != default(DateTime));

                this.population[1].AddRange(children);

                this.progress++;
            }

            /*

           
            var q = new List<Critter>(this.Properties.PopulationSize);
            foreach (var c1 in f)
            {
                c1.Clear();
            }
            f.Clear();
         

          
            
            for (int i = 0; i < this.Properties.PopulationSize/2; i ++)
            {
                var first = TournamentSelect();
                var second = TournamentSelect();
                
                 

                Assert.That(first.departureTime != default(DateTime) && second.departureTime != default(DateTime));

                bool doCrossover = this.random.NextDouble() <= this.Properties.CrossoverRate;
                bool doMutation = this.random.NextDouble() <= this.Properties.MutationRate;
                Critter[] children = doCrossover
                                         ? this.Properties.Breeder.Crossover(first, second)
                                         : new[] { first, second };


                if (doMutation)
                {
                    children[0] = this.Properties.Mutator.Mutate(children[0]);
                    children[1] = this.Properties.Mutator.Mutate(children[1]);
                }

                Assert.That(children[0].departureTime != default(DateTime) && children[1].departureTime != default(DateTime));

                if (doCrossover || doMutation)
                {
                    children[0].Fitness = this.Properties.FitnessFunction.GetFitness(children[0].Route, children[0].departureTime);
                    children[1].Fitness = this.Properties.FitnessFunction.GetFitness(children[1].Route, children[1].departureTime);
                }
                //var ff = (AlFitnessFunction)this.properties.FitnessFunction;

                Assert.That(children[0].departureTime != default(DateTime) && children[1].departureTime != default(DateTime));

                q.AddRange(children);

                progress++;
            }

           
            
            var r = this.population.Select(p => (Critter)p.Clone()).ToList();

            checkPopDupes();


            //r.AddRange(this.population);
            r.AddRange(q);

           
            
            

            double maxTime = r.Max(c => c.Fitness.TotalJourneyTime).TotalHours;
            double minTime = r.Min(c => c.Fitness.TotalJourneyTime).TotalHours;
            int maxChanges = r.Max(c => c.Fitness.Changes);
            int minChanges = r.Min(c => c.Fitness.Changes);

            foreach (var c in r)
            {
                c.Fitness.NormalisedJourneyTime = c.Fitness.TotalJourneyTime.TotalHours / (maxTime - minTime);
                c.Fitness.NormalisedChanges = (double) c.Fitness.Changes / (maxChanges - minChanges);
                Console.WriteLine("{0}, {1},{2},{3}",c.Fitness.NormalisedJourneyTime,c.Fitness.NormalisedChanges,c.Fitness.PercentBuses,c.Fitness.PercentTrains);
                c.N = 0;
                c.Rank = 0;
                c.Distance = 0;
            }


            //this.crowdingDistanceAssignment(r);

            this.nonDominatedSort(r);

            foreach (var front in Fronts)
            {
                foreach (var c1 in front)
                {
                    foreach (var c2 in front)
                    {
                        Assert.That(!this.Dominates(c1,c2), "Members of a front should be non dominate to each other.");
                        
                    }
                }
            }

            checkPopDupes();

            this.population.Clear();
            int j = 0;

           
            while (this.population.Count + this.Fronts[j].Count <= this.Properties.PopulationSize)
            {
                var front = this.Fronts[j];
                this.crowdingDistanceAssignment(front);
                this.population.AddRange(front);

                j++;
            }


            for (int i = 0; i < this.Fronts.Count; i++)
            {
                var front = this.Fronts[i];
                foreach (var critter in front)
                {
                    Assert.That(critter.Rank == i + 1);
                        
                }
            }
            /*
            for (int i = 0; i < this.Fronts.Count; i++)
            {
                var front = this.Fronts[i];
                foreach (var critter in front)
                {
                    critter.Rank = i + 1;
                }
            }
            

            var ranks = this.population.GroupBy(g => g.Rank);
            foreach (var rank in ranks)
            {
                foreach (var c1 in rank)
                {
                    foreach (var c2 in rank)
                    {
                        Assert.That(!this.Dominates(c1, c2), "Members of a front should be non dominate to each other.");

                    }
                }

            }


            checkPopDupes();

         
        
           
            this.Fronts[j].Sort(CompareCritters);
            /*
            this.Fronts[j].Sort((c1, c2) =>
                {
                    if (c1.Rank < c2.Rank || (c1.Rank == c2.Rank && c1.Distance > c2.Distance))
                    {
                        return 1;
                    }
                    return -1;
                });
            
  
            this.population.AddRange(this.Fronts[j].GetRange(0, this.Properties.PopulationSize-this.population.Count));


            checkPopDupes();


        
           /*
            foreach (var eliteCritter in eliteCritters)
            {
                Tools.ToLinkedNodes(eliteCritter.Route.GetNodes(true));
            }
            

            /*
            this.nonDominatedSort(this.population);

            var ranks = this.population.GroupBy(g => g.Rank);
            foreach (var rank in ranks)
            {
                foreach (var c1 in rank)
                {
                    foreach (var c2 in rank)
                    {
                        Assert.That(!this.Dominates(c1, c2), "Members of a front should be non dominate to each other.");

                    }
                }

            }
            

            sw.Stop();

            this.result.Totaltime = sw.Elapsed;

            ranks = this.population.GroupBy(g => g.Rank);
            foreach (var rank in ranks)
            {
                foreach (var c1 in rank)
                {
                    foreach (var c2 in rank)
                    {
                        Assert.That(!this.Dominates(c1, c2), "Members of a front should be non dominate to each other.");

                    }
                }

            }
            */
            var distinct = new Population();

            foreach (var c in this.population[0])
            {
                bool same = false;
                foreach (var fitnessParameter in this.properties.Objectives)
                {
                    if (
                        distinct.Any(
                            c2 => Math.Abs(c2.Fitness[fitnessParameter] - c.Fitness[fitnessParameter]) < Epsilon))
                    {
                        same = true;
                    }
                }

                if (!same)
                {
                    distinct.Add(c);
                }
            }

            this.result.Cardinality = distinct.Count;

            /*
             ranks = distinct.GroupBy(g => g.Rank);
            foreach (var rank in ranks)
            {
                foreach (var c1 in rank)
                {
                    foreach (var c2 in rank)
                    {
                        Assert.That(!this.Dominates(c1, c2), "Members of a front should be non dominate to each other.");

                    }
                }

            }
            */
            this.population[0] = this.population[1];
            this.result.Population = (Population)this.population[0].Clone();

            // foreach (var critter in this.Population)
            // {
            // this.result.AverageFitness += critter.Fitness;
            // }
            // this.result.AverageFitness /= population.Count;
            // var sorted = this.Population.OrderBy(z => z.Fitness.TotalJourneyTime);
            // this.result.MinimumFitness = sorted.First().Fitness;
            // this.result.BestPath = sorted.First().Route;

            // Tools.SavePopulation(this.population.GetRange(0, 25), ++this.generation, this.properties);

            // this.BestNode = Tools.ToLinkedNodes(this.Population[0].Route);

            // Console.WriteLine("Average fitness: {0}", this.result.MinimumFitness);
            // return false;
        }

        /// <summary>
        /// Start solving a route
        /// </summary>
        public void Start()
        {
            this.population = new[] { new Population(), new Population() };

            this.InitPopulation();
            this.iteration = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// A utility function to compare two critters.
        /// </summary>
        /// <param name="c1">
        /// The first critter under comparison.
        /// </param>
        /// <param name="c2">
        /// The second critter under comparison.
        /// </param>
        /// <returns>
        /// The standard integer values used by sorting functions.
        /// </returns>
        private static int CompareCritters(Critter c1, Critter c2)
        {
            if (c1 == c2)
            {
                return 0;
            }

            if (c1 == null)
            {
                if (c2 == null)
                {
                    // If c1 is null and c2 is null, they're
                    // equal. 
                    return 0;
                }

                // If c1 is null and c2 is not null, c2
                // is greater. 
                return 1;
            }

            if (c1.Fitness.TotalJourneyTime < c2.Fitness.TotalJourneyTime)
            {
                return 1;
            }

            return -1;
        }

        /// <summary>
        /// Create the initial population.
        /// </summary>
        private void InitPopulation()
        {
            this.population = new[] { new Population(), new Population() };
            var sw = Stopwatch.StartNew();
            var routesUsed = new Dictionary<int, int>();

            this.targetProgress = this.Properties.PopulationSize;

            for (int p = 0; p < 1; p++)
            {
                for (int i = 0; i < this.Properties.PopulationSize * 2; i++)
                {
                    this.progress = i + 1;
                    Route route = null;
                    while (route == null)
                    {
                        route =
                            (Route)
                            this.Properties.RouteGenerator.Generate(
                                (INetworkNode)this.Properties.Origin.Clone(), 
                                (INetworkNode)this.Properties.Destination.Clone()).Clone();
                    }

                    var critter = new Critter(route, new Fitness());
                    critter.DepartureTime =
                        this.properties.DepartureTime.AddMinutes((Random.GetInstance().NextDouble() * 60.0) - 30.0);

                    critter.Fitness = this.Properties.FitnessFunction.GetFitness(route, critter.DepartureTime);
                    Assert.That(critter.DepartureTime != default(DateTime));

                    Logger.Log(
                        this, 
                        "Member {0}, fitness {1}, total nodes {2}", 
                        i, 
                        critter.UnifiedFitnessScore, 
                        critter.Route.Count);

                    // this.result.AverageFitness += critter.Fitness;
                    var ff = (AlFitnessFunction)this.Properties.FitnessFunction;
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

                    this.population[p].Add(critter);
                }
            }

            sw.Stop();
            this.result.Totaltime = sw.Elapsed;
            this.result.Population = this.population[0];

            // this.result.DiversityMetric = routesUsed.Keys.Count;
            // this.result.AverageFitness /= this.Properties.PopulationSize;
            // Console.WriteLine("---EVALULATING FITTEST MEMBER---");
            // this.result.MinimumFitness = this.Population.OrderBy(f => f.Fitness.TotalJourneyTime).FirstOrDefault().Fitness;
            // this.result.BestPath = this.Population.OrderBy(f => f.Fitness.TotalJourneyTime).FirstOrDefault().Route;

            // this.IterationResult
            Console.WriteLine("------------------------");

            // Tools.SavePopulation(this.population, 0, this.properties);
            // this.BestNode = Tools.ToLinkedNodes(this.Population[0].Route);
        }

        /// <summary>
        /// Performs a binary tournament selection on the current population.
        /// </summary>
        /// <returns>
        /// The critter that wins the tournament.
        /// </returns>
        private Critter TournamentSelect()
        {
            ////TODO: Make tournament select able to handle a larger tournament size (n>2) 
            
            var first = (Critter)this.population[0][this.random.Next(this.population[0].Count - 1)].Clone();
            var second = (Critter)this.population[0][this.random.Next(this.population[0].Count - 1)].Clone();

            /*
            int attempts = 0;
            while (first.Route.Count == second.Route.Count && first.Route.Select(n2=>n2.Node).Intersect(second.Route.Select(n=>n.Node)).Count() == first.Route.Count)
            {
                attempts++;
                second = (Critter)this.population[0][this.random.Next(this.population[0].Count - 1)].Clone();
                if (attempts > 100 )
                {
                    break;
                }
            }
             */
            if (CompareCritters(first, second) == 1)
            {
                return first;
            }

            return second;
        }

        #endregion
    }
}