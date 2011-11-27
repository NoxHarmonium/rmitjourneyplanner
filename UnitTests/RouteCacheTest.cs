using RmitJourneyPlanner.CoreLibraries.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RmitJourneyPlanner.CoreLibraries.DataAccess;
using RmitJourneyPlanner.CoreLibraries.DataProviders;
using RmitJourneyPlanner.CoreLibraries.Types;
using System.Data;
using System.Collections.Generic;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for RouteCacheTest and is intended
    ///to contain all RouteCacheTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RouteCacheTest
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
        ///A test for RouteCache Constructor
        ///</summary>
        [TestMethod()]
        public void RouteCacheConstructorTest()
        {
            TramTrackerAPI api = new TramTrackerAPI();
            DataSet upData = api.GetListOfStopsByRouteNoAndDirection("8",true);
            DataSet downData = api.GetListOfStopsByRouteNoAndDirection("8",false);
            TramStop upDestination = new TramStop(upData.Tables[0].Rows[ upData.Tables[0].Rows.Count-1]["TID"].ToString(),null);
            TramStop downDestination = new TramStop(downData.Tables[0].Rows[downData.Tables[0].Rows.Count-1]["TID"].ToString(),null);

            Route route = new Route("8", upDestination, downDestination);



            RouteCache target = new RouteCache("8");
            target.InitializeCache();            
            target.AddCacheEntry(route);
            List<string>[] ids = new List<string>[]{target.GetRoute("8", true),target.GetRoute("8",false)};

            foreach (bool direction in new bool[] { false, true })
            {
                int count = 0;
                int dir = Convert.ToInt32(direction);

                foreach(TramStop stop in route.GetNodes(direction))
                {
                    Assert.AreEqual(stop.ID, ids[dir][count++]);
                }

            }

        }
    }
}
