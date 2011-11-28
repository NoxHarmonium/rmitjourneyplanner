using RmitJourneyPlanner.CoreLibraries.DataProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using RmitJourneyPlanner.CoreLibraries.Types;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for TramNetworkProviderTest and is intended
    ///to contain all TramNetworkProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TramNetworkProviderTest
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
        ///A test for GetAdjacentNodes
        ///</summary>
        [TestMethod()]
        public void GetAdjacentNodesTest()
        {
            TramNetworkProvider target = new TramNetworkProvider();
            INetworkNode node = new TramStop("2210", target);
            List<INetworkNode> expected = null; // TODO: Initialize to an appropriate value
            List<INetworkNode> actual;
            actual = target.GetAdjacentNodes(node);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDistanceBetweenNodes
        ///</summary>
        [TestMethod()]
        public void GetDistanceBetweenNodesTest()
        {
            //Test simple route
            TramNetworkProvider target = new TramNetworkProvider();
            INetworkNode source = new TramStop("1551",target);
            INetworkNode destination = new TramStop("1216", target); 
            DateTime requestTime = DateTime.Parse("11/24/2011 1:23:00 PM"); 
            List<Arc> actual;
            actual = target.GetDistanceBetweenNodes(source, destination, requestTime);
            Assert.AreEqual(new TimeSpan(0,14,0), actual[0].Time);

            //Test 2 routes parralell
            source = new TramStop("1214", target);
            destination = new TramStop("1216", target);
            actual = target.GetDistanceBetweenNodes(source, destination, requestTime);
            Assert.AreEqual(new TimeSpan(0, 8, 0), actual[0].Time);
            Assert.AreEqual(new TimeSpan(0, 2, 0), actual[1].Time);
            

            //Test overnight 
            requestTime = DateTime.Parse("11/24/2011 1:23:00 AM");
            actual = target.GetDistanceBetweenNodes(source, destination, requestTime);
            Assert.AreEqual(new TimeSpan(3 ,29 ,0), actual[0].Time);
            Assert.AreEqual(new TimeSpan(4, 11, 0), actual[1].Time);

        }
    }
}
