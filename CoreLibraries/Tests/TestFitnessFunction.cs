
 using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
using RmitJourneyPlanner.CoreLibraries.Types;
using System;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;

using RmitJourneyPlanner.CoreLibraries.DataProviders;
using RmitJourneyPlanner.CoreLibraries.DataProviders.Google;
using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners;
//using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Breeders;
//using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Mutators;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.RouteGenerators;
//using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
using RmitJourneyPlanner.CoreLibraries.DataAccess;
//using RmitJourneyPlanner.CoreLibraries.Types;

namespace RmitJourneyPlanner.CoreLibraries.Tests
{
	[TestFixture]
	public class TestFitnessFunction
	{
		private MetlinkDataProvider provider;
		
		
		public TestFitnessFunction ()
		{
			provider = new MetlinkDataProvider();
		    Logging.Logger.LogEvent += (sender, message) => { Console.WriteLine("[{0}]: {1}", sender, message); };
		}
        
        [Test]
        public void TestRandomWalk()
        {

            var properties = new EvolutionaryProperties();
            properties.NetworkDataProviders = new[] { provider };
            properties.Bidirectional = false;
            properties.PointDataProviders = new[] { new WalkingDataProvider() };
            properties.ProbMinDistance = 0.7;
            properties.ProbMinTransfers = 0.2;
            properties.MaximumWalkDistance = 1.5;
            properties.PopulationSize = 100;
            properties.MaxDistance = 0.5;
            properties.InfectionRate = 0.2;
            properties.DepartureTime = DateTime.Parse("2012/7/24 2:00 PM");//DateTime.Parse(date + " "+ time);
            properties.NumberToKeep = 25;
            properties.MutationRate = 0.1;
            properties.CrossoverRate = 0.7;
            //properties.RouteGenerator = new AlRouteGenerator(properties);
            properties.SearchType = SearchType.RW_Standard;
            properties.RouteGenerator = new DFSRoutePlanner(properties);
            properties.Mutator = new StandardMutator(properties);
            properties.Breeder = new StandardBreeder(properties);
            properties.FitnessFunction = new AlFitnessFunction(properties);
            properties.Database = new MySqlDatabase("20110606fordistributionforrmit");
            properties.Destination = new MetlinkNode(20039, provider);//
            //properties.Destination = new MetlinkNode(628,metlinkProvider);
            // new TerminalNode(-1, destination);
            // metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, destination), 0);
            //properties.Origin = new MetlinkNode(19965, metlinkProvider);//
            //metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, origin), 0);
            properties.Origin = new MetlinkNode(19965, provider);//
            //metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, origin), 0);

            properties.Destination.RetrieveData();
            properties.Origin.RetrieveData();
            properties.Objectives = new[] { FitnessParameter.TotalJourneyTime, FitnessParameter.PercentTrains, FitnessParameter.PercentTrams };


            properties.Database.Open();
            //properties.DataStructures = new DataStructures(properties);


            properties.Planner = new MoeaRoutePlanner(properties);

            properties.RouteGenerator.Generate(properties.Origin, properties.Destination,DateTime.Now);






        }

		[Test]
		public void TestRouteGeneration()
		{
			 var properties = new EvolutionaryProperties();
				properties.NetworkDataProviders = new [] {provider};
                properties.Bidirectional = false;
                properties.PointDataProviders = new [] {new WalkingDataProvider()};
                properties.ProbMinDistance = 0.7;
                properties.ProbMinTransfers = 0.2;
                properties.MaximumWalkDistance = 1.5;
                properties.PopulationSize = 100;
                properties.MaxDistance = 0.5;
		        properties.InfectionRate = 0.2;
                properties.DepartureTime = DateTime.Parse("2012/11/10 9:30 AM");//DateTime.Parse(date + " "+ time);
                properties.NumberToKeep = 25;
                properties.MutationRate = 0.25;
                properties.CrossoverRate = 0.7;
                //properties.RouteGenerator = new AlRouteGenerator(properties);
		    properties.SearchType = SearchType.A_Star_BiDir;
                properties.RouteGenerator = new DFSRoutePlanner(properties);
		        properties.Mutator = new StandardMutator(properties);
                properties.Breeder = new TimeBlendBreeder(properties);
                properties.FitnessFunction = new AlFitnessFunction(properties);
                properties.Database = new MySqlDatabase("20110606fordistributionforrmit");
                //properties.Destination = new MetlinkNode(628, provider);// kew
                //properties.Origin = new MetlinkNode(9601, provider);// reynard st

		     //properties.Origin = new MetlinkNode(18536, provider); //abbotsford interchange
                properties.Origin = new MetlinkNode(19965, provider); // Coburg
                properties.Destination = new MetlinkNode(20039, provider); //ascot vale
                //properties.Origin = new MetlinkNode(19036,provider); //Cotham (o)
                //properties.Destination = new MetlinkNode(19943, provider); // Caulfielsd
                //properties.Destination = new MetlinkNode(19933, provider); // Darebinor whateves
                //properties.Destination = new MetlinkNode(19394,provider); // other kew
            // properties.Destination = new MetlinkNode(4141, provider); // Jolimont
                //properties.Destination = new MetlinkNode(628,metlinkProvider);
                   // new TerminalNode(-1, destination);
                   // metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, destination), 0);
                //properties.Origin = new MetlinkNode(19965, metlinkProvider);//
                    //metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, origin), 0);
              
                    //metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, origin), 0);
                   
                properties.Destination.RetrieveData();
                properties.Origin.RetrieveData();
                properties.Objectives = new [] {FitnessParameter.NormalisedChanges, FitnessParameter.TotalJourneyTime, FitnessParameter.DiversityMetric};

                properties.Database.Open();
                //properties.DataStructures = new DataStructures(properties);


                properties.Planner = new MoeaRoutePlanner(properties);

                properties.Planner.Start();
				for(int i = 0; i < 100; i++)
				{
                    properties.Planner.SolveStep();	
				}
		}
		
		[Test]
		public void TestSingleTramRoutes()
		{
		var tests = new List<KeyValuePair<Route, TransportTimeSpan>>();

            
            //Single Legs
            
            var tram19Route = new Route(19,new []
                {
                    new MetlinkNode(17306,provider), 
                    new MetlinkNode(17307,provider), 
                    new MetlinkNode(17308,provider), 
                    new MetlinkNode(17309,provider), 
                    new MetlinkNode(17310,provider), 
                    new MetlinkNode(17311,provider), 
                    new MetlinkNode(17312,provider), 
                    new MetlinkNode(17313,provider), 
                    new MetlinkNode(17314,provider), 
                    new MetlinkNode(17315,provider), 
                    new MetlinkNode(17316,provider), 
                    new MetlinkNode(17317,provider), 
                    new MetlinkNode(17318,provider), 
                    new MetlinkNode(17319,provider), 
                    new MetlinkNode(17320,provider), 
                    new MetlinkNode(17321,provider), 
                    new MetlinkNode(17322,provider), 
                    new MetlinkNode(17323,provider), 
                    new MetlinkNode(17324,provider), 
                    new MetlinkNode(17325,provider), 
                    new MetlinkNode(17327,provider), 
                    new MetlinkNode(17328,provider), 
                    new MetlinkNode(17329,provider), 
                    new MetlinkNode(17330,provider), 
                    new MetlinkNode(17331,provider), 
                    new MetlinkNode(17332,provider), 
                    new MetlinkNode(17333,provider), 
                    new MetlinkNode(17334,provider), 
                    new MetlinkNode(17335,provider), 
                    new MetlinkNode(17868,provider), 
                    new MetlinkNode(17869,provider), 
                    new MetlinkNode(17870,provider), 
                    new MetlinkNode(17871,provider), 
                    new MetlinkNode(17872,provider), 
                    new MetlinkNode(17873,provider), 
                    new MetlinkNode(17874,provider), 
                    new MetlinkNode(17875,provider), 
                    new MetlinkNode(17876,provider), 
                    new MetlinkNode(17877,provider)
                });



           
            var tram19Down = new Route(19)
                {
                    new MetlinkNode(17850, provider),
                    new MetlinkNode(17851, provider),
                    new MetlinkNode(17852, provider),
                    new MetlinkNode(17853, provider),
                    new MetlinkNode(17854, provider),
                    new MetlinkNode(17855, provider),
                    new MetlinkNode(17856, provider),
                    new MetlinkNode(17865, provider),
                    new MetlinkNode(17866, provider),
                    new MetlinkNode(17867, provider),
                    new MetlinkNode(16736, provider),
                    new MetlinkNode(16735, provider),
                    new MetlinkNode(16734, provider),
                    new MetlinkNode(16733, provider),
                    new MetlinkNode(16732, provider),
                    new MetlinkNode(16731, provider),
                    new MetlinkNode(16730, provider),
                    new MetlinkNode(16729, provider),
                    new MetlinkNode(16728, provider),
                    new MetlinkNode(16727, provider),
                    new MetlinkNode(16726, provider),
                    new MetlinkNode(16725, provider),
                    new MetlinkNode(16724, provider),
                    new MetlinkNode(16723, provider),
                    new MetlinkNode(16722, provider),
                    new MetlinkNode(16721, provider),
                    new MetlinkNode(16720, provider),
                    new MetlinkNode(16719, provider),
                    new MetlinkNode(21961, provider),
                    new MetlinkNode(16717, provider),
                    new MetlinkNode(16716, provider),
                    new MetlinkNode(16715, provider),
                    new MetlinkNode(16714, provider),
                    new MetlinkNode(16713, provider),
                    new MetlinkNode(12777, provider),
                    new MetlinkNode(2113, provider),
                    new MetlinkNode(1784, provider),
                    new MetlinkNode(1292, provider),
                    new MetlinkNode(378, provider),
                    new MetlinkNode(323, provider),
                    new MetlinkNode(17306, provider)
                };

            var tram55down = new Route(55)
                {
                    new MetlinkNode(18221, provider),
                    new MetlinkNode(18222, provider),
                    new MetlinkNode(18223, provider),
                    new MetlinkNode(18233, provider),
                    new MetlinkNode(18234, provider),
                    new MetlinkNode(18450, provider),
                    new MetlinkNode(18465, provider),
                    new MetlinkNode(18466, provider),
                    new MetlinkNode(18467, provider),
                    new MetlinkNode(18469, provider),
                    new MetlinkNode(18199, provider),
                    new MetlinkNode(18200, provider),
                    new MetlinkNode(18201, provider),
                    new MetlinkNode(18202, provider),
                    new MetlinkNode(18206, provider),
                    new MetlinkNode(18203, provider),
                    new MetlinkNode(20493, provider),
                    new MetlinkNode(20490, provider),
                    new MetlinkNode(19591, provider),
                    new MetlinkNode(19699, provider),
                    new MetlinkNode(19592, provider),
                    new MetlinkNode(19593, provider),
                    new MetlinkNode(19594, provider),
                    new MetlinkNode(18542, provider),
                    new MetlinkNode(18543, provider),
                    new MetlinkNode(18544, provider),
                    new MetlinkNode(18545, provider),
                    new MetlinkNode(18546, provider),
                    new MetlinkNode(18547, provider),
                    new MetlinkNode(18548, provider),
                    new MetlinkNode(18549, provider),
                    new MetlinkNode(18550, provider),
                    new MetlinkNode(18551, provider),
                    new MetlinkNode(18552, provider),
                    new MetlinkNode(18553, provider),
                    new MetlinkNode(18554, provider),
                    new MetlinkNode(18555, provider),
                    new MetlinkNode(18556, provider),
                    new MetlinkNode(18557, provider),
                    new MetlinkNode(18558, provider),
                    new MetlinkNode(18559, provider),
                    new MetlinkNode(18560, provider),
                    new MetlinkNode(18561, provider),
                    new MetlinkNode(18562, provider),
                    new MetlinkNode(18563, provider),
                    new MetlinkNode(18564, provider),
                    new MetlinkNode(18565, provider),
                    new MetlinkNode(18086, provider)
                };

            var tram55up = new Route(55)
                {
                    new MetlinkNode(18086, provider),
                    new MetlinkNode(18085, provider),
                    new MetlinkNode(18084, provider),
                    new MetlinkNode(18083, provider),
                    new MetlinkNode(18082, provider),
                    new MetlinkNode(18081, provider),
                    new MetlinkNode(18080, provider),
                    new MetlinkNode(18079, provider),
                    new MetlinkNode(18078, provider),
                    new MetlinkNode(18077, provider),
                    new MetlinkNode(18076, provider),
                    new MetlinkNode(18075, provider),
                    new MetlinkNode(18074, provider),
                    new MetlinkNode(18073, provider),
                    new MetlinkNode(18072, provider),
                    new MetlinkNode(18071, provider),
                    new MetlinkNode(18069, provider),
                    new MetlinkNode(18068, provider),
                    new MetlinkNode(18067, provider),
                    new MetlinkNode(18066, provider),
                    new MetlinkNode(18065, provider),
                    new MetlinkNode(18064, provider),
                    new MetlinkNode(18063, provider),
                    new MetlinkNode(18062, provider),
                    new MetlinkNode(19256, provider),
                    new MetlinkNode(19255, provider),
                    new MetlinkNode(19254, provider),
                    new MetlinkNode(19253, provider),
                    new MetlinkNode(19252, provider),
                    new MetlinkNode(18204, provider),
                    new MetlinkNode(18205, provider),
                    new MetlinkNode(18207, provider),
                    new MetlinkNode(18208, provider),
                    new MetlinkNode(18209, provider),
                    new MetlinkNode(18210, provider),
                    new MetlinkNode(18211, provider),
                    new MetlinkNode(18212, provider),
                    new MetlinkNode(18213, provider),
                    new MetlinkNode(18214, provider),
                    new MetlinkNode(18215, provider),
                    new MetlinkNode(18216, provider),
                    new MetlinkNode(18217, provider),
                    new MetlinkNode(18218, provider),
                    new MetlinkNode(18219, provider),
                    new MetlinkNode(18220, provider)
                };
			
			//var route1 = new Route(-1)
			//{
				//new MetlinkNode(0,provider)					
				
			//};

		    var expressTest = new Route(-1)
		        {
		            
                     new MetlinkNode(19975 ,provider),
                     new MetlinkNode(19974 ,provider),
                     new MetlinkNode(20019 ,provider),
                     new MetlinkNode(20017 ,provider),
                     new MetlinkNode(20016 ,provider),
                     new MetlinkNode(20015 ,provider),
                     new MetlinkNode(20014 ,provider),
                     new MetlinkNode(20013 ,provider)

		        };

		    var bigTest = new Route(-1)
		        {
		            new MetlinkNode(19965, provider),
		            new MetlinkNode(19966, provider),
		            new MetlinkNode(19967, provider),
		            new MetlinkNode(19968, provider),
		            new MetlinkNode(19969, provider),
		            new MetlinkNode(19970, provider),
		            new MetlinkNode(45656, provider),
		            new MetlinkNode(45657, provider),
		            new MetlinkNode(45658, provider),
		            new MetlinkNode(45659, provider),
		            new MetlinkNode(45660, provider),
		            new MetlinkNode(45661, provider),
		            new MetlinkNode(4531, provider),
		            new MetlinkNode(4530, provider),
		            new MetlinkNode(20391, provider),
		            new MetlinkNode(4529, provider),
		            new MetlinkNode(20390, provider),
		            new MetlinkNode(4528, provider),
		            new MetlinkNode(18778, provider),
		            new MetlinkNode(7961, provider),
		            new MetlinkNode(3218, provider),
		            new MetlinkNode(20039, provider)
		        };

            tests.Add(new KeyValuePair<Route, TransportTimeSpan>(
                tram19Route, new TransportTimeSpan { WaitingTime = new TimeSpan(0, 0, 0), TravelTime = new TimeSpan(0, 40, 0) }));
            tests.Add(new KeyValuePair<Route, TransportTimeSpan>(
                tram19Down, new TransportTimeSpan { WaitingTime = new TimeSpan(0, 3, 0), TravelTime = new TimeSpan(0, 40, 0) }));
            tests.Add(new KeyValuePair<Route, TransportTimeSpan>(
                tram55down, new TransportTimeSpan { WaitingTime = new TimeSpan(0, 3, 0), TravelTime = new TimeSpan(0, 48-3, 0) }));
            tests.Add(new KeyValuePair<Route, TransportTimeSpan>(
               tram55up, new TransportTimeSpan { WaitingTime = new TimeSpan(0, 10, 0), TravelTime = new TimeSpan(0, 45, 0) }));
             tests.Add(new KeyValuePair<Route, TransportTimeSpan>(
                bigTest , new TransportTimeSpan { WaitingTime = new TimeSpan(0, 10, 0), TravelTime = new TimeSpan(0, 45, 0) }));
            
            
            var properties = new EvolutionaryProperties();
           
			
			
			properties.NetworkDataProviders = new [] {provider};
            properties.PointDataProviders = new [] {new WalkingDataProvider()};
            AlFitnessFunction target;
            properties.FitnessFunction = target = new AlFitnessFunction(properties); 
            
            DateTime initialDepart = DateTime.Parse("8/08/2012 6:00 PM");
            foreach (var keyValuePair in tests)
            {
                Console.WriteLine("Getting fitness from {0} to {1}",keyValuePair.Key.First(),keyValuePair.Key.Last());
				var actual = target.GetFitness(keyValuePair.Key, initialDepart);
                Assert.AreEqual(keyValuePair.Value.TotalTime,actual.TotalJourneyTime);
            }		

            for (int i = 0; i < 24; i++)
            {
                DateTime newDepart = initialDepart - TimeSpan.FromHours(i / 2f);
                var actual = target.GetFitness(expressTest, newDepart);
                Assert.That(actual.TotalTravelTime == TimeSpan.FromMinutes(13));
                Assert.That(actual.TotalWaitingTime < TimeSpan.FromMinutes(30));

            }
		}

        [Test]
        public void TestMultiModeRoutes()
        {
            var properties = new EvolutionaryProperties();
            properties.NetworkDataProviders = new [] {provider};
            properties.PointDataProviders = new [] {new WalkingDataProvider()};
            AlFitnessFunction target;
            properties.FitnessFunction = target = new AlFitnessFunction(properties); 
            
            //LineID    LineMainID  LineDesc    DirectionDesc   LineFrom    LineTo  
            //3984      1695        903i        To Altona       Mordialloc  Altona      
            //3985      1695        903o        To Mordialloc   Altona      Mordialloc   

            //Train: Coburg Station 19965 --> Walk
            // Bus:Coburg Station -- 2262-->18773 Essendon Station
            // Train 
            var route1 = new Route(-1)
            {
                new MetlinkNode(19965,provider),

                //new MetlinkNode(2262,provider), 
                //new MetlinkNode(2263,provider), 
                new MetlinkNode(2262,provider), 
                new MetlinkNode(10037,provider), 
                new MetlinkNode(10036,provider), 
                new MetlinkNode(10035,provider), 
                new MetlinkNode(10034,provider), 
                new MetlinkNode(10033,provider), 
                new MetlinkNode(10032,provider), 
                new MetlinkNode(40895,provider), 
                new MetlinkNode(40896,provider), 
                new MetlinkNode(40897,provider), 
                new MetlinkNode(40898,provider), 
                new MetlinkNode(9089,provider), 
                new MetlinkNode(9088,provider), 
                new MetlinkNode(18773,provider), 

                new MetlinkNode(20037,provider), 
                new MetlinkNode(20038,provider), 
                new MetlinkNode(20039,provider), 
            };


            DateTime initialDepart = DateTime.Parse("8/08/2012 6:00 PM");
            var actual = target.GetFitness(route1, initialDepart);
            Assert.AreEqual(41, actual.TotalJourneyTime.Minutes);
        }
	}
}

