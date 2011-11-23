using RmitJourneyPlanner.CoreLibraries.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;
using System.Data;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for TramTrackerAPITest and is intended
    ///to contain all TramTrackerAPITest Unit Tests
    ///</summary>
    [TestClass()]
    public class TramTrackerAPITest
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
        ///A test for Guid
        ///</summary>
        [TestMethod()]
        public void GuidTest()
        {
            TramTrackerAPI target = new TramTrackerAPI(); 
            string actual;
            actual = target.Guid;
            
            Assert.IsTrue(target.IsValidUuid(actual));
        }

        /// <summary>
        ///A test for GetDestinationsForAllRoutes
        ///</summary>
        [TestMethod()]
        public void GetDestinationsForAllRoutesTest()
        {
            TramTrackerAPI target = new TramTrackerAPI(); // TODO: Initialize to an appropriate value
            DataSet result = target.GetDestinationsForAllRoutes();
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count > 0);
            Assert.IsTrue(result.Tables[0].Select("RouteNo=1 AND UpStop=false AND Destination='East Coburg'").Length > 0);

        }

        /// <summary>
        ///A test for GetDestinationsForRoute
        ///</summary>
        [TestMethod()]
        public void GetDestinationsForRouteTest()
        {
            TramTrackerAPI target = new TramTrackerAPI(); // TODO: Initialize to an appropriate value
            string routeId = "1"; // TODO: Initialize to an appropriate value
            DataSet result = target.GetDestinationsForRoute(routeId);
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count > 0);
            Assert.IsTrue(((string)result.Tables[0].Rows[0][0]) == "Sth Melb Beach");
            Assert.IsTrue(((string)result.Tables[0].Rows[0][1]) == "East Coburg");
            

        }
    }
}
