using RmitJourneyPlanner.CoreLibraries.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RmitJourneyPlanner.CoreLibraries.Positioning;
using RmitJourneyPlanner.CoreLibraries.Types;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for DistanceAPITest and is intended
    ///to contain all DistanceAPITest Unit Tests
    ///</summary>
    [TestClass()]
    public class DistanceAPITest
    {


        private TestContext testContextInstance;

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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
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
        ///A test for GetDistance
        ///</summary>
        [TestMethod()]
        public void GetDistanceTest()
        {
            DistanceApi target = new DistanceApi();
            Location pointA = new Location("Lonsdale Street, Melbourne, Victoria, Australia");
            Location pointB = new Location("Royal Park, Parkville, Victoria, Australia");
            TransportMode transportMode = TransportMode.Driving;
            Arc expected = new Arc(pointA,pointB,new TimeSpan(0,11,0),4700.0,default(DateTime),TransportMode.Driving.ToString()); 
            Arc actual;
            actual = target.GetDistance(pointA, pointB, transportMode);
            Assert.AreEqual(expected, actual);
           
        }

        /// <summary>
        ///A test for GetDistance
        ///</summary>
        [TestMethod()]
        public void GetDistanceTest1()
        {
            DistanceApi target = new DistanceApi();
            Location pointA = new Location("Lonsdale Street, Melbourne, Victoria, Australia");
            Location pointB = new Location("Royal Park, Parkville, Victoria, Australia");
            Arc expected = new Arc(pointA,pointB,new TimeSpan(0, 11, 0), 4700.0,default(DateTime), TransportMode.Driving.ToString());
            Arc actual;
            actual = target.GetDistance(pointA, pointB);
            Assert.AreEqual(expected, actual);
           
        }
    }
}
