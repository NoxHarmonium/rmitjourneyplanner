using RmitJourneyPlanner.CoreLibraries.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RmitJourneyPlanner.CoreLibraries.Positioning;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for GeocodingAPITest and is intended
    ///to contain all GeocodingAPITest Unit Tests
    ///</summary>
    [TestClass()]
    public class GeocodingAPITest
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
        ///A test for GetLocation
        ///</summary>
        [TestMethod()]
        public void GetLocationTest()
        {
            GeocodingAPI target = new GeocodingAPI(); 
            string locationString = "Cuzco, Peru"; 
            //string locationString = "Medevigatan M, 25657 RAMLOSA, Sweden"; // 56.0222365,12.7362315
            Location expected = new Location(-13.525, -71.9722222); 
            Location actual;
            actual = target.GetLocation(locationString);
            Assert.AreEqual(expected.ToString(), actual.ToString());
           
        }

        /// <summary>
        ///A test for GetLocationString
        ///</summary>
        [TestMethod()]
        public void GetLocationStringTest()
        {
            GeocodingAPI target = new GeocodingAPI(); 
            Location location = new Location(-13.525, -71.9722222);
            string expected = "El Sol 627 Cuzco Peru";
            string actual;
            actual = target.GetLocationString(location);
            Assert.AreEqual(expected, actual.Replace(",",""));            
        }

       

       
    }
}
