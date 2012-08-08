using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
using RmitJourneyPlanner.CoreLibraries.Types;

namespace UnitTests
{
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Google;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;

    /// <summary>
    ///This is a test class for AlFitnessFunctionTest and is intended
    ///to contain all AlFitnessFunctionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AlFitnessFunctionTest
    {


        private TestContext testContextInstance;

        private static MetlinkDataProvider provider;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            provider = new MetlinkDataProvider();
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for AlFitnessFunction Constructor
        ///</summary>
        [TestMethod()]
        public void AlFitnessFunctionConstructorTest()
        {
            var properties = new EvolutionaryProperties {DepartureTime = DateTime.Parse("8/08/2012 6:00 PM")};
            properties.NetworkDataProviders.Add(provider);
            properties.FitnessFunction = new AlFitnessFunction(properties);
        }

        /// <summary>
        ///A test for GetFitness
        ///</summary>
        [TestMethod()]
        [DeploymentItem("UnitTests/Settings.xml")]
        public void GetFitnessTest()
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


            tests.Add(new KeyValuePair<Route, TransportTimeSpan>(
                tram19Route, new TransportTimeSpan { WaitingTime = new TimeSpan(0, 2, 0), TravelTime = new TimeSpan(0, 38, 0) }));
            tests.Add(new KeyValuePair<Route, TransportTimeSpan>(
                tram19Down, new TransportTimeSpan { WaitingTime = new TimeSpan(0, 3, 0), TravelTime = new TimeSpan(0, 40, 0) }));
            tests.Add(new KeyValuePair<Route, TransportTimeSpan>(
                tram55down, new TransportTimeSpan { WaitingTime = new TimeSpan(0, 3, 0), TravelTime = new TimeSpan(0, 45, 0) }));
            tests.Add(new KeyValuePair<Route, TransportTimeSpan>(
               tram55up, new TransportTimeSpan { WaitingTime = new TimeSpan(0, 10, 0), TravelTime = new TimeSpan(0, 45, 0) }));
            
            var properties = new EvolutionaryProperties();
            properties.NetworkDataProviders.Add(provider);
            properties.PointDataProviders.Add(new WalkingDataProvider());
            AlFitnessFunction target;
            properties.FitnessFunction = target = new AlFitnessFunction(properties); 
            
            DateTime initialDepart = DateTime.Parse("8/08/2012 6:00 PM");
            foreach (var keyValuePair in tests)
            {
                var actual = target.GetFitness(keyValuePair.Key, initialDepart);
                Assert.AreEqual(keyValuePair.Value,actual.TotalJourneyTime);
            }
            
           
        }
        
   
    }
}
