using System;
using NUnit.Framework;

using RmitJourneyPlanner.CoreLibraries.DataProviders;
using RmitJourneyPlanner.CoreLibraries.DataProviders.Google;
using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Breeders;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Mutators;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.RouteGenerators;
using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
using RmitJourneyPlanner.CoreLibraries.DataAccess;
using RmitJourneyPlanner.CoreLibraries.Types;


namespace JRPCServer
{
    [TestFixture]
    public class TestOptimisationManager
    {
        [Test]
        public void TestQueue()
        {
            Journey j = new Journey();
            j.ShortName = "Jeff";
            j.Description = "blah";

            j.RunUuids = new[] { "a", "b", "c" };
            var properties = j.Properties;
            var provider = new MetlinkDataProvider();
            ObjectCache.RegisterObject(provider);
            properties.NetworkDataProviders = new[] { provider };
            properties.PointDataProviders = new[] { new WalkingDataProvider() };
            properties.ProbMinDistance = 0.7;
            properties.ProbMinTransfers = 0.2;
            properties.MaximumWalkDistance = 1.5;
            properties.PopulationSize = 100;
            properties.MaxDistance = 0.5;
            properties.DepartureTime = DateTime.Now;
            properties.NumberToKeep = 25;
            properties.MutationRate = 0.1;
            properties.CrossoverRate = 0.7;
            //properties.Bidirectional = bidir;
            //properties.RouteGenerator = new AlRouteGenerator(properties);
            properties.SearchType = SearchType.Greedy_BiDir;
            properties.RouteGenerator = new DFSRoutePlanner(properties);
            properties.Mutator = new StandardMutator(properties);
            properties.Breeder = new StandardBreeder(properties);
            properties.FitnessFunction = new AlFitnessFunction(properties);
            properties.Database = new MySqlDatabase();
            properties.SearchType = SearchType.Greedy_BiDir;
            properties.Destination = new MetlinkNode(22180, provider);
            properties.Origin = new MetlinkNode(19843, provider);
            properties.Planner = new MoeaRoutePlanner(properties);
            properties.MaxIterations = 25;
            properties.Objectives = new[]
                { FitnessParameter.Changes, FitnessParameter.PercentTrains, FitnessParameter.PercentBuses };

            JourneyManager jm = new JourneyManager();
            jm.Clean();
            jm.Add(j);
            JourneyOptimiser jo = new JourneyOptimiser(jm);
            jo.EnqueueJourney(j.Uuid,2);
            jo.WaitOnOptimisation();
            if (jo.ThrownException != null)
            {
                throw jo.ThrownException;
            }

        }
    }



}