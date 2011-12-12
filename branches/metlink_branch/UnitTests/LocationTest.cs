using RmitJourneyPlanner.CoreLibraries.Positioning;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for LocationTest and is intended
    ///to contain all LocationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LocationTest
    {

        //Allowed error on floats
        private double delta = 0.01;
        
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
        ///A test for Location Constructor
        ///</summary>
        [TestMethod()]
        public void LocationConstructorTest()
        {
            string location = "Melbourne, Australia";
            Location target = new Location(location);
            Assert.AreEqual(target.Latitude, -37.811954,delta);
            Assert.AreEqual(target.Longitude, 144.963126, delta);     
        }

        /// <summary>
        ///A test for Location Constructor
        ///</summary>
        [TestMethod()]
        public void LocationConstructorTest1()
        {
            double latitude = -37.811954; 
            double longitude = 144.963126;
            Location target = new Location(latitude, longitude);
            Assert.AreEqual(target.Latitude, -37.811954, delta);
            Assert.AreEqual(target.Longitude, 144.963126, delta);   
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            string location = "Melbourne, Australia";
            Location target = new Location(location);
            string expected = "-37.8131869,144.9629796";
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);            
        }

        
    }
}
