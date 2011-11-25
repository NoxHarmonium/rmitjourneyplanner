using RmitJourneyPlanner.CoreLibraries.Positioning;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for GeometryHelperTest and is intended
    ///to contain all GeometryHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GeometryHelperTest
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
        ///A test for GetStraightLineDistance
        ///</summary>
        [TestMethod()]
        public void GetStraightLineDistanceTest()
        {
            Location locationA = new Location(50.066389,5.714722); // TODO: Initialize to an appropriate value
            Location locationB = new Location(58.643889, 3.07); // TODO: Initialize to an appropriate value
            double expected = 968.9F; // TODO: Initialize to an appropriate value
            double actual;
            actual = GeometryHelper.GetStraightLineDistance(locationA, locationB);
            Assert.AreEqual(expected, actual,0.1);            
        }

        /// <summary>
        ///A test for ToRads
        ///</summary>
        [TestMethod()]
        public void ToRadsTest()
        {
            double x = 360F; 
            double expected = 2.0*Math.PI; 
            double actual;
            actual = GeometryHelper.ToRads(x);
            Assert.AreEqual(expected, actual, 0.00001);
        }

        /// <summary>
        ///A test for Travel
        ///</summary>
        [TestMethod()]
        public void TravelTest()
        {
            Location initial = new Location(53.320556,1.729722);
            double bearing = 0F;
            double distance = 124.8F; 
            Location expected = new Location(54.442778, 1.729722);
            Location actual;
            actual = GeometryHelper.Travel(initial, bearing, distance);
            Assert.AreEqual(expected.Latitude, actual.Latitude,0.001);
            Assert.AreEqual(expected.Longitude, actual.Longitude, 0.001);
            
        }
    }
}
