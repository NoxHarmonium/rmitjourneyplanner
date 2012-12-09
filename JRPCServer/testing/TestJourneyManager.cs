using System;
using NUnit.Framework;

using RmitJourneyPlanner.CoreLibraries.DataProviders;
using RmitJourneyPlanner.CoreLibraries.DataProviders.Google;
using RmitJourneyPlanner.CoreLibraries.DataProviders.Ptv;
using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
using RmitJourneyPlanner.CoreLibraries.JourneyPlanners;
using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary;
using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary.Breeders;
using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary.FitnessFunctions;
using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary.Mutators;
using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary.RouteGenerators;
using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
using RmitJourneyPlanner.CoreLibraries.DataAccess;
using RmitJourneyPlanner.CoreLibraries.Types;


namespace JRPCServer
{
	[TestFixture]
	public class TestJourneyManager
	{
		
		
		[Test]
		public void TestSerialization()
		{
			
			Journey j = new Journey();
			j.ShortName = "Jeff";
			j.Description ="blah";
			
			j.RunUuids = new[] {"a","b","c"};
			var properties = j.Properties;
			var provider = new PtvDataProvider();
			ObjectCache.RegisterObject(provider);
	        properties.NetworkDataProviders = new [] {provider};
		    properties.PointDataProviders = new [] {new WalkingDataProvider()};
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
            properties.Database = new MySqlDatabase("20110606fordistributionforrmit");
            properties.SearchType = SearchType.Greedy_BiDir;
			properties.Destination = new PtvNode(4,provider);
			properties.Origin = new PtvNode(4,provider);
		
			JourneyManager m = new JourneyManager();
			m.Add(j);
			m.Save();
			m = new JourneyManager();
			var j2 = m.GetJourney(j.Uuid);
		
		
			Assert.That(j.Uuid == j2.Uuid);
			
			
			
			
		}
	}
}

