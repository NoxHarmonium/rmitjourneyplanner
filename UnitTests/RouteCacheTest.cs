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
    using RmitJourneyPlanner.CoreLibraries.DataProviders.YarraTrams;

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
            TramTrackerApi api = new TramTrackerApi();
            DataSet upData = api.GetListOfStopsByRouteNoAndDirection("1",true);
            DataSet downData = api.GetListOfStopsByRouteNoAndDirection("1",false);
            TramStop upDestination = new TramStop(upData.Tables[0].Rows[ upData.Tables[0].Rows.Count-1]["TID"].ToString(),null);
            TramStop downDestination = new TramStop(downData.Tables[0].Rows[downData.Tables[0].Rows.Count-1]["TID"].ToString(),null);

            //this.downDestination = downDestination;
            Route route = new Route("1");
            List<String> upIds = new List<string>();
            List<String> downIds = new List<string>();
        
            foreach(DataRow row in upData.Tables[0].Rows)
            {
                route.AddNode(new TramStop(row["TID"].ToString(), null),true);
                upIds.Add(row["TID"].ToString());
            }
            foreach (DataRow row in downData.Tables[0].Rows)
            {
                route.AddNode(new TramStop(row["TID"].ToString(), null), false);
                downIds.Add(row["TID"].ToString());
            }


            RouteCache target = new RouteCache("Test");
            target.InitializeCache();            
            target.AddCacheEntry("1",upIds,true);
            target.AddCacheEntry("1",downIds,false);
            List<string>[] ids = new List<string>[]{target.GetRoute("1", true),target.GetRoute("1",false)};

            foreach (bool direction in new bool[] { true,false })
            {
                int count = 0;
                int dir = Convert.ToInt32(direction);
                List<INetworkNode> nodes = route.GetNodes(direction);
               
                foreach(TramStop stop in nodes)
                {
                    Assert.AreEqual(stop.Id, ids[dir][count++]);
                }

            }

        }
    }
}
