﻿using RmitJourneyPlanner.CoreLibraries.DataAccess;
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
            TramTrackerAPI target = new TramTrackerAPI(); 
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
            TramTrackerAPI target = new TramTrackerAPI(); 
            string routeId = "1"; 
            DataSet result = target.GetDestinationsForRoute(routeId);
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count > 0);
            Assert.IsTrue(((string)result.Tables[0].Rows[0][0]) == "Sth Melb Beach");
            Assert.IsTrue(((string)result.Tables[0].Rows[0][1]) == "East Coburg");
            

        }

        /// <summary>
        ///A test for GetListOfStopsByRouteNoAndDirection
        ///</summary>
        [TestMethod()]
        public void GetListOfStopsByRouteNoAndDirectionTest()
        {
            TramTrackerAPI target = new TramTrackerAPI(); // TODO: Initialize to an appropriate value
            string routeId = "1"; 
            bool isUpDirection = false; 
            DataSet result = target.GetListOfStopsByRouteNoAndDirection(routeId, isUpDirection);
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count == 54);
            
        }

        /// <summary>
        ///A test for GetMainRoutes
        ///</summary>
        [TestMethod()]
        public void GetMainRoutesTest()
        {
            TramTrackerAPI target = new TramTrackerAPI();            
            DataSet result = target.GetMainRoutes();
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count == 30);
        }

        /// <summary>
        ///A test for GetMainRoutesForStop
        ///</summary>
        [TestMethod()]
        public void GetMainRoutesForStopTest()
        {
            TramTrackerAPI target = new TramTrackerAPI(); 
            string stopNo = "1551";             
            DataSet result = target.GetMainRoutesForStop(stopNo);
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows[0][0].ToString() == "8");
        }

        /// <summary>
        ///A test for GetNextPredictedArivalTime
        ///</summary>
        [TestMethod()]
        public void GetNextPredictedArivalTimeTest()
        {
            TramTrackerAPI target = new TramTrackerAPI(); 
            string tramNo = "56";             
            DataSet result = target.GetNextPredictedArivalTime(tramNo);            
            Assert.Inconclusive("Don't know how to find tram number.");
        }
    }
}
