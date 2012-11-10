// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestFitnessFunction.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   The test fitness function.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Tests
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using RmitJourneyPlanner.CoreLibraries.DataAccess;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Google;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.Logging;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Breeders;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Mutators;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.RouteGenerators;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// The test fitness function.
    /// </summary>
    [TestFixture]
    public class TestFitnessFunction
    {
        #region Constants and Fields

        /// <summary>
        ///   The provider.
        /// </summary>
        private readonly PtvDataProvider provider;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TestFitnessFunction" /> class.
        /// </summary>
        public TestFitnessFunction()
        {
            this.provider = new PtvDataProvider();
            Logger.LogEvent += (sender, message) => { Console.WriteLine("[{0}]: {1}", sender, message); };
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The test multi mode routes.
        /// </summary>
        [Test]
        public void TestMultiModeRoutes()
        {
            var properties = new EvolutionaryProperties();
            properties.NetworkDataProviders = new[] { this.provider };
            properties.PointDataProviders = new[] { new WalkingDataProvider() };
            AlFitnessFunction target;
            properties.FitnessFunction = target = new AlFitnessFunction(properties);

            // LineID    LineMainID  LineDesc    DirectionDesc   LineFrom    LineTo  
            // 3984      1695        903i        To Altona       Mordialloc  Altona      
            // 3985      1695        903o        To Mordialloc   Altona      Mordialloc   

            // Train: Coburg Station 19965 --> Walk
            // Bus:Coburg Station -- 2262-->18773 Essendon Station
            // Train 
            var route1 = new Route(-1)
                {
                    new PtvNode(19965, this.provider), 
                    // new PtvNode(2262,provider), 
                    // new PtvNode(2263,provider), 
                    new PtvNode(2262, this.provider), 
                    new PtvNode(10037, this.provider), 
                    new PtvNode(10036, this.provider), 
                    new PtvNode(10035, this.provider), 
                    new PtvNode(10034, this.provider), 
                    new PtvNode(10033, this.provider), 
                    new PtvNode(10032, this.provider), 
                    new PtvNode(40895, this.provider), 
                    new PtvNode(40896, this.provider), 
                    new PtvNode(40897, this.provider), 
                    new PtvNode(40898, this.provider), 
                    new PtvNode(9089, this.provider), 
                    new PtvNode(9088, this.provider), 
                    new PtvNode(18773, this.provider), 
                    new PtvNode(20037, this.provider), 
                    new PtvNode(20038, this.provider), 
                    new PtvNode(20039, this.provider), 
                };

            DateTime initialDepart = DateTime.Parse("8/08/2012 6:00 PM");
            var actual = target.GetFitness(route1, initialDepart);
            Assert.AreEqual(41, actual.TotalJourneyTime.Minutes);
        }

        /// <summary>
        /// The test random walk.
        /// </summary>
        [Test]
        public void TestRandomWalk()
        {
            var properties = new EvolutionaryProperties();
            properties.NetworkDataProviders = new[] { this.provider };
            properties.Bidirectional = false;
            properties.PointDataProviders = new[] { new WalkingDataProvider() };
            properties.ProbMinDistance = 0.7;
            properties.ProbMinTransfers = 0.2;
            properties.MaximumWalkDistance = 1.5;
            properties.PopulationSize = 100;
            properties.MaxDistance = 0.5;
            properties.InfectionRate = 0.2;
            properties.DepartureTime = DateTime.Parse("2012/7/24 2:00 PM"); // DateTime.Parse(date + " "+ time);
            properties.NumberToKeep = 25;
            properties.MutationRate = 0.1;
            properties.CrossoverRate = 0.7;

            // properties.RouteGenerator = new AlRouteGenerator(properties);
            properties.SearchType = SearchType.RW_Standard;
            properties.RouteGenerator = new DFSRoutePlanner(properties);
            properties.Mutator = new StandardMutator(properties);
            properties.Breeder = new StandardBreeder(properties);
            properties.FitnessFunction = new AlFitnessFunction(properties);
            properties.Database = new MySqlDatabase();

            // properties.Destination = new PtvNode(628,metlinkProvider);
            // new TerminalNode(-1, destination);
            // metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, destination), 0);
            // properties.Origin = new PtvNode(19965, metlinkProvider);//
            // metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, origin), 0);
            // metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, origin), 0);
            properties.Destination.RetrieveData();
            properties.Origin.RetrieveData();
            properties.Objectives = new[]
                {
                   FitnessParameter.TotalJourneyTime, FitnessParameter.PercentTrains, FitnessParameter.PercentTrams 
                };

            properties.Database.Open();

            // properties.DataStructures = new DataStructures(properties);
            properties.Planner = new MoeaRoutePlanner(properties);

            properties.RouteGenerator.Generate(properties.Origin, properties.Destination, DateTime.Now);
        }

        /// <summary>
        /// The test route generation.
        /// </summary>
        [Test]
        public void TestRouteGeneration()
        {
            var properties = new EvolutionaryProperties();
            properties.NetworkDataProviders = new[] { this.provider };
            properties.Bidirectional = false;
            properties.PointDataProviders = new[] { new WalkingDataProvider() };
            properties.ProbMinDistance = 0.7;
            properties.ProbMinTransfers = 0.2;
            properties.MaximumWalkDistance = 1.5;
            properties.PopulationSize = 100;
            properties.MaxDistance = 0.5;
            properties.InfectionRate = 0.2;
            properties.DepartureTime = DateTime.Parse("2012/10/17 9:30 AM"); // DateTime.Parse(date + " "+ time);
            properties.NumberToKeep = 25;
            properties.MutationRate = 0.25;
            properties.CrossoverRate = 0.7;

            // properties.RouteGenerator = new AlRouteGenerator(properties);
            properties.SearchType = SearchType.A_Star_BiDir;
            properties.RouteGenerator = new DFSRoutePlanner(properties);
            properties.Mutator = new StandardMutator(properties);
            properties.Breeder = new TimeBlendBreeder(properties);
            properties.FitnessFunction = new AlFitnessFunction(properties);
            properties.Database = new MySqlDatabase();

            // properties.Destination = new PtvNode(628, provider);// kew
            // properties.Origin = new PtvNode(9601, provider);// reynard st
            // properties.Origin = new PtvNode( 20013, provider); // Bell
            // properties.Destination = new PtvNode(20042, provider); //Boxhill
            properties.Origin = new PtvNode(18536, this.provider); // abbotsford interchange

            // properties.Origin = new PtvNode(19965, provider); // Coburg
            // properties.Destination = new PtvNode(20039, provider); //ascot vale
            // properties.Origin = new PtvNode(19036,provider); //Cotham (o)
            // properties.Destination = new PtvNode(19943, provider); // Caulfielsd
            // properties.Destination = new PtvNode(19933, provider); // Darebinor whateves
            // properties.Destination = new PtvNode(19394,provider); // other kew
            properties.Destination = new PtvNode(4141, this.provider); // Jolimont

            // properties.Destination = new PtvNode(628,metlinkProvider);
            // new TerminalNode(-1, destination);
            // metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, destination), 0);
            // properties.Origin = new PtvNode(19965, metlinkProvider);//
            // metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, origin), 0);

            // metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, origin), 0);
            properties.Destination.RetrieveData();
            properties.Origin.RetrieveData();
            properties.Objectives = new[]
                {
                    FitnessParameter.TotalJourneyTime, FitnessParameter.DiversityMetric, FitnessParameter.TotalTravelTime
                };

            properties.Database.Open();

            // properties.DataStructures = new DataStructures(properties);
            properties.Planner = new MoeaRoutePlanner(properties);

            properties.Planner.Start();
            for (int i = 0; i < 100; i++)
            {
                properties.Planner.SolveStep();
            }
        }

        /// <summary>
        /// The test single tram routes.
        /// </summary>
        [Test]
        public void TestSingleTramRoutes()
        {
            var tests = new List<KeyValuePair<Route, TransportTimeSpan>>();

            // Single Legs
            var bus512Route = new Route(
                512, 
                new[]
                    {
                        new PtvNode(9335, this.provider), new PtvNode(9334, this.provider), 
                        new PtvNode(9333, this.provider), new PtvNode(9332, this.provider), 
                        new PtvNode(9331, this.provider), new PtvNode(9330, this.provider), 
                        new PtvNode(9329, this.provider), new PtvNode(9328, this.provider), 
                        new PtvNode(9327, this.provider), new PtvNode(9326, this.provider), 
                        new PtvNode(9325, this.provider), new PtvNode(9324, this.provider), 
                        new PtvNode(9323, this.provider), new PtvNode(20754, this.provider), 
                        new PtvNode(9321, this.provider), new PtvNode(9320, this.provider), 
                        new PtvNode(9319, this.provider), new PtvNode(9318, this.provider), 
                        new PtvNode(9317, this.provider), new PtvNode(9316, this.provider), 
                        new PtvNode(21267, this.provider), new PtvNode(21268, this.provider), 
                        new PtvNode(9315, this.provider), new PtvNode(20755, this.provider), 
                        new PtvNode(9314, this.provider), new PtvNode(9313, this.provider), 
                        new PtvNode(9312, this.provider), new PtvNode(20756, this.provider), 
                        new PtvNode(9310, this.provider), new PtvNode(20757, this.provider)
                    });

            /*
            var tram19Route = new Route(19,new []
                {
                    new PtvNode(17306,provider), 
                    new PtvNode(17307,provider), 
                    new PtvNode(17308,provider), 
                    new PtvNode(17309,provider), 
                    new PtvNode(17310,provider), 
                    new PtvNode(17311,provider), 
                    new PtvNode(17312,provider), 
                    new PtvNode(17313,provider), 
                    new PtvNode(17314,provider), 
                    new PtvNode(17315,provider), 
                    new PtvNode(17316,provider), 
                    new PtvNode(17317,provider), 
                    new PtvNode(17318,provider), 
                    new PtvNode(17319,provider), 
                    new PtvNode(17320,provider), 
                    new PtvNode(17321,provider), 
                    new PtvNode(17322,provider), 
                    new PtvNode(17323,provider), 
                    new PtvNode(17324,provider), 
                    new PtvNode(17325,provider), 
                    new PtvNode(17327,provider), 
                    new PtvNode(17328,provider), 
                    new PtvNode(17329,provider), 
                    new PtvNode(17330,provider), 
                    new PtvNode(17331,provider), 
                    new PtvNode(17332,provider), 
                    new PtvNode(17333,provider), 
                    new PtvNode(17334,provider), 
                    new PtvNode(17335,provider), 
                    new PtvNode(17868,provider), 
                    new PtvNode(17869,provider), 
                    new PtvNode(17871,provider), 
                    new PtvNode(17872,provider), 
                    new PtvNode(17873,provider), 
                    new PtvNode(17874,provider), 
                    new PtvNode(17875,provider), 
                    new PtvNode(17876,provider), 
                    new PtvNode(17877,provider)
                });



           
            var tram19Down = new Route(19)
                {
                    new PtvNode(17850, provider),
                    new PtvNode(17851, provider),
                    new PtvNode(17852, provider),
                    new PtvNode(17853, provider),
                    new PtvNode(17854, provider),
                    new PtvNode(17855, provider),
                    new PtvNode(17856, provider),
                    new PtvNode(17865, provider),
                    new PtvNode(17866, provider),
                    new PtvNode(17867, provider),
                    new PtvNode(16736, provider),
                    new PtvNode(16735, provider),
                    new PtvNode(16734, provider),
                    new PtvNode(16733, provider),
                    new PtvNode(16732, provider),
                    new PtvNode(16731, provider),
                    new PtvNode(16730, provider),
                    new PtvNode(16729, provider),
                    new PtvNode(16728, provider),
                    new PtvNode(16727, provider),
                    new PtvNode(16726, provider),
                    new PtvNode(16725, provider),
                    new PtvNode(16724, provider),
                    new PtvNode(16723, provider),
                    new PtvNode(16722, provider),
                    new PtvNode(16721, provider),
                    new PtvNode(16720, provider),
                    new PtvNode(16719, provider),
                    new PtvNode(21961, provider),
                    new PtvNode(16717, provider),
                    new PtvNode(16716, provider),
                    new PtvNode(16715, provider),
                    new PtvNode(16714, provider),
                    new PtvNode(16713, provider),
                    new PtvNode(12777, provider),
                    new PtvNode(2113, provider),
                    new PtvNode(1784, provider),
                    new PtvNode(1292, provider),
                    new PtvNode(378, provider),
                    new PtvNode(323, provider),
                    new PtvNode(17306, provider)
                };

            var tram55down = new Route(55)
                {
                    new PtvNode(18221, provider),
                    new PtvNode(18222, provider),
                    new PtvNode(18223, provider),
                    new PtvNode(18233, provider),
                    new PtvNode(18234, provider),
                    new PtvNode(18450, provider),
                    new PtvNode(18465, provider),
                    new PtvNode(18466, provider),
                    new PtvNode(18467, provider),
                    new PtvNode(18469, provider),
                    new PtvNode(18199, provider),
                    new PtvNode(18200, provider),
                    new PtvNode(18201, provider),
                    new PtvNode(18202, provider),
                    new PtvNode(18206, provider),
                    new PtvNode(18203, provider),
                    new PtvNode(20493, provider),
                    new PtvNode(20490, provider),
                    new PtvNode(19591, provider),
                    new PtvNode(19699, provider),
                    new PtvNode(19592, provider),
                    new PtvNode(19593, provider),
                    new PtvNode(19594, provider),
                    new PtvNode(18542, provider),
                    new PtvNode(18543, provider),
                    new PtvNode(18544, provider),
                    new PtvNode(18545, provider),
                    new PtvNode(18546, provider),
                    new PtvNode(18547, provider),
                    new PtvNode(18548, provider),
                    new PtvNode(18549, provider),
                    new PtvNode(18550, provider),
                    new PtvNode(18551, provider),
                    new PtvNode(18552, provider),
                    new PtvNode(18553, provider),
                    new PtvNode(18554, provider),
                    new PtvNode(18555, provider),
                    new PtvNode(18556, provider),
                    new PtvNode(18557, provider),
                    new PtvNode(18558, provider),
                    new PtvNode(18559, provider),
                    new PtvNode(18560, provider),
                    new PtvNode(18561, provider),
                    new PtvNode(18562, provider),
                    new PtvNode(18563, provider),
                    new PtvNode(18564, provider),
                    new PtvNode(18565, provider),
                    new PtvNode(18086, provider)
                };

            var tram55up = new Route(55)
                {
                    new PtvNode(18086, provider),
                    new PtvNode(18085, provider),
                    new PtvNode(18084, provider),
                    new PtvNode(18083, provider),
                    new PtvNode(18082, provider),
                    new PtvNode(18081, provider),
                    new PtvNode(18080, provider),
                    new PtvNode(18079, provider),
                    new PtvNode(18078, provider),
                    new PtvNode(18077, provider),
                    new PtvNode(18076, provider),
                    new PtvNode(18075, provider),
                    new PtvNode(18074, provider),
                    new PtvNode(18073, provider),
                    new PtvNode(18072, provider),
                    new PtvNode(18071, provider),
                    new PtvNode(18069, provider),
                    new PtvNode(18068, provider),
                    new PtvNode(18067, provider),
                    new PtvNode(18066, provider),
                    new PtvNode(18065, provider),
                    new PtvNode(18064, provider),
                    new PtvNode(18063, provider),
                    new PtvNode(18062, provider),
                    new PtvNode(19256, provider),
                    new PtvNode(19255, provider),
                    new PtvNode(19254, provider),
                    new PtvNode(19253, provider),
                    new PtvNode(19252, provider),
                    new PtvNode(18204, provider),
                    new PtvNode(18205, provider),
                    new PtvNode(18207, provider),
                    new PtvNode(18208, provider),
                    new PtvNode(18209, provider),
                    new PtvNode(18210, provider),
                    new PtvNode(18211, provider),
                    new PtvNode(18212, provider),
                    new PtvNode(18213, provider),
                    new PtvNode(18214, provider),
                    new PtvNode(18215, provider),
                    new PtvNode(18216, provider),
                    new PtvNode(18217, provider),
                    new PtvNode(18218, provider),
                    new PtvNode(18219, provider),
                    new PtvNode(18220, provider)
                };
			
			//var route1 = new Route(-1)
			//{
				//new PtvNode(0,provider)					
				
			//};

             */
            var expressTest = new Route(-1)
                {
                    new PtvNode(19975, this.provider), 
                    new PtvNode(19974, this.provider), 
                    new PtvNode(20019, this.provider), 
                    new PtvNode(20017, this.provider), 
                    new PtvNode(20016, this.provider), 
                    new PtvNode(20015, this.provider), 
                    new PtvNode(20014, this.provider), 
                    new PtvNode(20013, this.provider)
                };

            /*
		    var bigTest = new Route(-1)
		        {
		            new PtvNode(19965, provider),
		            new PtvNode(19966, provider),
		            new PtvNode(19967, provider),
		            new PtvNode(19968, provider),
		            new PtvNode(19969, provider),
		            new PtvNode(19970, provider),
		            new PtvNode(45656, provider),
		            new PtvNode(45657, provider),
		            new PtvNode(45658, provider),
		            new PtvNode(45659, provider),
		            new PtvNode(45660, provider),
		            new PtvNode(45661, provider),
		            new PtvNode(4531, provider),
		            new PtvNode(4530, provider),
		            new PtvNode(20391, provider),
		            new PtvNode(4529, provider),
		            new PtvNode(20390, provider),
		            new PtvNode(4528, provider),
		            new PtvNode(18778, provider),
		            new PtvNode(7961, provider),
		            new PtvNode(3218, provider),
		            new PtvNode(20039, provider)
		        };
             * 

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
            */
            tests.Add(new KeyValuePair<Route, TransportTimeSpan>(bus512Route, new TransportTimeSpan()));

            var properties = new EvolutionaryProperties();

            properties.NetworkDataProviders = new[] { this.provider };
            properties.PointDataProviders = new[] { new WalkingDataProvider() };
            AlFitnessFunction target;
            properties.FitnessFunction = target = new AlFitnessFunction(properties);

            DateTime initialDepart = DateTime.Parse("8/08/2012 3:00 PM");
            foreach (var keyValuePair in tests)
            {
                Console.WriteLine("Getting fitness from {0} to {1}", keyValuePair.Key.First(), keyValuePair.Key.Last());
                var actual = target.GetFitness(keyValuePair.Key, initialDepart);
                Assert.AreEqual(keyValuePair.Value.TotalTime, actual.TotalJourneyTime);
            }

            for (int i = 0; i < 24; i++)
            {
                DateTime newDepart = initialDepart - TimeSpan.FromHours(i / 2f);
                var actual = target.GetFitness(expressTest, newDepart);
                Assert.That(actual.TotalTravelTime == TimeSpan.FromMinutes(13));
                Assert.That(actual.TotalWaitingTime < TimeSpan.FromMinutes(30));
            }
        }

        #endregion
    }
}