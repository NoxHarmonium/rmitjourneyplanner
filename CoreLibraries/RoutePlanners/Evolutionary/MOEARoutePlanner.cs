// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
using RmitJourneyPlanner.CoreLibraries.Positioning;
using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;

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
    using NUnit.Framework;

    #endregion
    //1/(e^x²)

    /// <summary>
    ///   Finds the best route between nodes using evolutionary algorithms.
    /// </summary>
    public class MoeaRoutePlanner : IRoutePlanner
    {
        #region Constants and Fields

        private const double Epsilon = double.Epsilon;

        /// <summary>
        ///   Contains the state of the evolutionary route planner.
        /// </summary>
        private readonly EvolutionaryProperties properties;

        /// <summary>
        ///   The random.
        /// </summary>
        private readonly Random random = CoreLibraries.Random.GetInstance();

        /// <summary>
        /// The result of each iteration.
        /// </summary>
        private readonly Result result = new Result();

        /// <summary>
        ///   The population of the evolutionary algorithm.
        /// </summary>
        private Population[] population;
        
        /// <summary>
        /// The population of viruses.
        /// </summary>
        private List<List<INetworkNode>> virusPopulation;

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
        public Population Population
        {
            get
            {
                return this.population[0];
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

        /// <summary>
        /// Gets the result of the last iteration.
        /// </summary>
        public Result IterationResult
        {
            get
            {
                return result;
            }
        }

        public List<List<Critter>> Fronts
        {
            get
            {
                return this.f;
            }
        }

        /// <summary>
        ///   Contains the state of the evolutionary route planner.
        /// </summary>
        public EvolutionaryProperties Properties
        {
            get
            {
                return this.properties;
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
            //this.Properties.NetworkDataProviders.Add(provider);
            this.Properties.NetworkDataProviders = new[] { provider };
        }

        /// <summary>
        ///   Register a point to point data provider for use with the route planning.
        /// </summary>
        /// <param name="provider"> The point to point data provider to register. </param>
        public void RegisterPointDataProvider(IPointDataProvider provider)
        {
            this.Properties.PointDataProviders = new[] { provider };
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


        private void nonDominatedSort(List<Critter> r)
        {
            var s = new Dictionary<Critter, List<Critter>>();
            f = new List<List<Critter>> { new List<Critter>(), new List<Critter>() };

            foreach (var p in r)
            {
                s[p] = new List<Critter>();
                p.N = 0;
                p.Rank = 0;
                foreach (var q in r)
                {
                    //if (p == q)
                    //    continue;

                    if (this.Dominates(p, q))
                    {
                        s[p].Add(q);
                    }
                    else if (this.Dominates(q, p))
                    {
                        p.N++;
                    }
                    Assert.That(!(this.Dominates(p, q) && this.Dominates(q, p)));
                }

                if (p.N != 0)
                {
                    continue;
                }
                p.Rank = 1;

                this.f[1].Add(p);
            }

            int i = 1;

            while (f[i].Any())
            {
                var Q = new List<Critter>();
                foreach (var p in f[i])
                {
                    foreach (var q in s[p])
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

            f.RemoveAt(0);


        }

        private void crowdingDistanceAssignment(List<Critter> X)
        {
            int l = X.Count;
            foreach (var x in X)
            {
                x.Distance = 0;
            }
            for (int i = 0; i < this.properties.Objectives.Length; i++)
            {
                X.Sort((x, y) => x.Fitness[(int)properties.Objectives[i]].CompareTo(y.Fitness[(int)properties.Objectives[i]]));
                X[0].Distance = Double.PositiveInfinity;
                X[l - 1].Distance = Double.PositiveInfinity;
                for (int j = 1; j < l - 1; j++)
                {
                    X[j].Distance += (X[j + 1].Fitness[(int)properties.Objectives[i]] - X[j - 1].Fitness[(int)properties.Objectives[i]]);
                }

            }
        }


        private static int CompareCritters(Critter x, Critter y)
        {
            if (x == y)
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

        private Critter TournamentSelect()
        {
            var first = (Critter)this.population[0][this.random.Next(this.population[0].Count - 1)].Clone();
            var second = (Critter)this.population[0][this.random.Next(this.population[0].Count - 1)].Clone();
            if (CompareCritters(first, second) == 1)
            {
                return first;

            }

            return second;


        }


        /// <summary>
        ///   Solve the next iteration of the algorithm.
        /// </summary>
        /// <returns> The solve step. </returns>
        public bool SolveStep()
        {




            checkPopDupes();

            if (this.properties.Objectives.Length < 1)
            {
                throw new Exception("You cannot optimise with 0 objectives.");
            }

            this.iteration = this.Iteration + 1;

            Console.WriteLine("Solving step {0}...", this.Iteration);


            this.progress = 0;

            var sw = Stopwatch.StartNew();



            var r = new Population(population[0].Union(population[1]));



            double jtMaxTime = r.Max(c => c.Fitness.TotalJourneyTime).TotalHours;
            double jtMinTime = r.Min(c => c.Fitness.TotalJourneyTime).TotalHours;
            double ttMaxTime = r.Max(c => c.Fitness.TotalTravelTime).TotalHours;
            double ttMinTime = r.Min(c => c.Fitness.TotalTravelTime).TotalHours;
            int maxChanges = r.Max(c => c.Fitness.Changes);
            int minChanges = r.Min(c => c.Fitness.Changes);

            foreach (var c in r)
            {
                c.Fitness.NormalisedJourneyTime = c.Fitness.TotalJourneyTime.TotalHours / (jtMaxTime - jtMinTime);
                c.Fitness.NormalisedChanges = (double)c.Fitness.Changes / (maxChanges - minChanges);
                c.Fitness.NormalisedTravelTime = c.Fitness.TotalJourneyTime.TotalHours / (ttMaxTime - ttMinTime);

                if (double.IsInfinity(c.Fitness.NormalisedChanges))
                {
                    c.Fitness.NormalisedChanges = 1;
                }
                if ((double.IsInfinity(c.Fitness.NormalisedJourneyTime)))
                {
                    c.Fitness.NormalisedJourneyTime = 1;
                }
                if ((double.IsInfinity(c.Fitness.NormalisedTravelTime)))
                {
                    c.Fitness.NormalisedTravelTime = 1;
                }

                Console.WriteLine("{0}, {1},{2},{3}", c.Fitness.NormalisedJourneyTime, c.Fitness.NormalisedChanges, c.Fitness.PercentBuses, c.Fitness.PercentTrains);
                c.N = 0;
                c.Rank = 0;
                c.Distance = 0;
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

            population[0] = new Population();

            int j = 0;


            while (this.population[0].Count + this.Fronts[j].Count <= this.Properties.PopulationSize)
            {
                var front = this.Fronts[j];
                this.crowdingDistanceAssignment(front);
                this.population[0].AddRange(front);

                j++;
            }

            this.Fronts[j].Sort(CompareCritters);
            this.population[0].AddRange(this.Fronts[j].GetRange(0, this.Properties.PopulationSize - this.population[0].Count));


            population[1] = new Population();

            for (int i = 0; i < this.Properties.PopulationSize / 2; i++)
            {
                var first = TournamentSelect();
                var second = TournamentSelect();



                Assert.That(first.departureTime != default(DateTime) && second.departureTime != default(DateTime));

                bool doCrossover = this.random.NextDouble() <= this.Properties.CrossoverRate;
                bool doMutation = this.random.NextDouble() <= this.Properties.MutationRate;
                bool doInfect = this.random.NextDouble() <= this.Properties.InfectionRate;

               

                Critter[] children = doCrossover
                                         ? this.Properties.Breeder.Crossover(first, second)
                                         : new[] { first, second };

                if (doInfect)
                {
                    /*
                    foreach (var child in children)
                    {
                        var virus = virusPopulation[random.Next(virusPopulation.Count - 1)];
                        var vRoute = new Route(-1, virus);
                        var nodesInCommon = child.Route.Intersect(vRoute).ToList();
                        var linkedNodes = new Dictionary<INetworkNode, INetworkNode>();
                        foreach (var node in vRoute)
                        {
                            foreach (var node2 in child.Route)
                            {
                                if (GeometryHelper.GetStraightLineDistance((Location)node.Node,(Location)node2.Node) < 0.3)
                                {
                                    if (!linkedNodes.ContainsKey(node.Node))
                                    {
                                        linkedNodes.Add(node.Node, node2.Node);
                                        nodesInCommon.Add(node);
                                    }
                                }
                            }
                        }
                        var intersectionNodes = new NodeWrapper<INetworkNode>[2];

                      
                        if (nodesInCommon.Count > 1)
                        {
                            nodesInCommon.Shuffle();
                            intersectionNodes[0] = nodesInCommon[0];
                            intersectionNodes[1] = nodesInCommon[1];



                            int vIntersection1 = vRoute.IndexOf(intersectionNodes[0]);
                            int vIntersection2 = vRoute.IndexOf(intersectionNodes[1]);
                            var childNodes = child.Route.ConvertAll(n => n.Node);


                            int nIntersection = childNodes.IndexOf(intersectionNodes[0].Node);
                            int nIntersection2 = childNodes.IndexOf(intersectionNodes[1].Node);

                            if (nIntersection == -1)
                            {
                                int index = childNodes.IndexOf(linkedNodes[intersectionNodes[0].Node]);
                                childNodes.Insert(index + 1, intersectionNodes[0].Node);
                                nIntersection = index + 1;
                            }

                            if (nIntersection2 == -1)
                            {
                                int index = childNodes.IndexOf(linkedNodes[intersectionNodes[1].Node]);
                                childNodes.Insert(index + 1, intersectionNodes[1].Node);
                                nIntersection2 = index + 1;
                            }

                            var composite = new Route(-1);
                            if (nIntersection2 <= nIntersection || vIntersection2 <= vIntersection1)
                                continue;
                            
                            composite.AddRange(child.Route.GetRange(0, nIntersection));
                            composite.AddRange(vRoute.GetRange(vIntersection1, vIntersection2 - vIntersection1));
                            composite.AddRange(child.Route.GetRange(nIntersection2, child.Route.Count - nIntersection2 - 1));

                            child.Route = composite;




                        }

                    }
                     * */


                }


                if (doMutation)
                {
                    children[0] = this.Properties.Mutator.Mutate(children[0]);
                    children[1] = this.Properties.Mutator.Mutate(children[1]);
                }

                Assert.That(children[0].departureTime != default(DateTime) && children[1].departureTime != default(DateTime));

                if (doCrossover || doMutation || doInfect)
                {
                    children[0].Fitness = this.Properties.FitnessFunction.GetFitness(children[0].Route, children[0].departureTime);
                    children[1].Fitness = this.Properties.FitnessFunction.GetFitness(children[1].Route, children[1].departureTime);
                }
                //var ff = (AlFitnessFunction)this.properties.FitnessFunction;

                Assert.That(children[0].departureTime != default(DateTime) && children[1].departureTime != default(DateTime));

                population[1].AddRange(children);

                progress++;
            }

            /*

           
            var q = new List<Critter>(this.Properties.PopulationSize);
            foreach (var x in f)
            {
                x.Clear();
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
            this.Fronts[j].Sort((x, y) =>
                {
                    if (x.Rank < y.Rank || (x.Rank == y.Rank && x.Distance > y.Distance))
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
                    if (distinct.Any(c2 => Math.Abs(c2.Fitness[fitnessParameter] - c.Fitness[fitnessParameter]) < Epsilon))
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

            this.result.Population = (Population)this.population[0].Clone();


            //foreach (var critter in this.Population)
            //{
            //this.result.AverageFitness += critter.Fitness;
            //}
            //this.result.AverageFitness /= population.Count;
            //var sorted = this.Population.OrderBy(z => z.Fitness.TotalJourneyTime);
            ///this.result.MinimumFitness = sorted.First().Fitness;
            //this.result.BestPath = sorted.First().Route;


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

            this.population = new[] { new Population(), new Population() };

            this.InitPopulation();
            this.iteration = 0;
        }

        #endregion

        #region Methods

        private void checkPopDupes()
        {
            for (int p = 0; p < 2; p++)
            {
                for (int i = 0; i < this.population[p].Count; i++)
                {
                    for (int j = 0; j < this.population[p].Count; j++)
                    {
                        if (i == j) continue;

                        Assert.That(!ReferenceEquals(this.population[p][i], this.population[p][j]));
                    }

                }
            }

        }

        /// <summary>
        ///   Create the initial population.
        /// </summary>
        private void InitPopulation()
        {
            var provider = (MetlinkDataProvider)this.properties.NetworkDataProviders[0];
            this.virusPopulation = new List<List<INetworkNode>>
                                       {

                                           provider.GetLineNodes(2272),
                                           //503i
                                           provider.GetLineNodes(2273),
                                           //503o
                                           provider.GetLineNodes(2282),
                                           //510i
                                           provider.GetLineNodes(2283),
                                           //510o
                                           provider.GetLineNodes(2272),
                                           //503i
                                           provider.GetLineNodes(2273),
                                           //503o
                                           provider.GetLineNodes(3984),
                                           //903i
                                           provider.GetLineNodes(3985),
                                           //903o
                                           provider.GetLineNodes(31),
                                           //Upfield
                                           provider.GetLineNodes(46),
                                           //Upfield
                                           provider.GetLineNodes(21),
                                           //Craigyburn
                                           provider.GetLineNodes(36) //Craigyburn
                                       };


            
            
            
            
            this.population = new[] { new Population(), new Population() };
            var sw = Stopwatch.StartNew();
            var routesUsed = new Dictionary<int, int>();

            targetProgress = this.Properties.PopulationSize * 2;

            for (int p = 0; p < 2; p++)
            {
                for (int i = 0; i < this.Properties.PopulationSize; i++)
                {
                    progress = i + 1;
                    Route route = null;
                    while (route == null)
                    {
                        route =
                            (Route)
                            this.Properties.RouteGenerator.Generate(
                                (INetworkNode)this.Properties.Origin.Clone(),
                                (INetworkNode)this.Properties.Destination.Clone(),
                                this.Properties.DepartureTime).Clone();
                    }


                    var critter = new Critter(route, this.Properties.FitnessFunction.GetFitness(route));
                    critter.departureTime =
                        properties.DepartureTime.AddMinutes(
                            (CoreLibraries.Random.GetInstance().NextDouble() * 30.0) - 15.0);
                    Assert.That(critter.departureTime != default(DateTime));

                    Logging.Logger.Log(
                        this,
                        "Member {0}, fitness {1}, total nodes {2}",
                        i,
                        critter.UnifiedFitnessScore,
                        critter.Route.Count);
                    //this.result.AverageFitness += critter.Fitness;
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
            checkPopDupes();

            // this.Population.Sort(new CritterComparer());

            checkPopDupes();

            sw.Stop();
            this.result.Totaltime = sw.Elapsed;
            this.result.Population = this.population[0];
            //this.result.DiversityMetric = routesUsed.Keys.Count;
            //this.result.AverageFitness /= this.Properties.PopulationSize;
            //Console.WriteLine("---EVALULATING FITTEST MEMBER---");
            //this.result.MinimumFitness = this.Population.OrderBy(f => f.Fitness.TotalJourneyTime).FirstOrDefault().Fitness;
            //this.result.BestPath = this.Population.OrderBy(f => f.Fitness.TotalJourneyTime).FirstOrDefault().Route;

            //this.IterationResult
            Console.WriteLine("------------------------");
            //Tools.SavePopulation(this.population, 0, this.properties);
            //this.BestNode = Tools.ToLinkedNodes(this.Population[0].Route);
        }

        #endregion
    }
}