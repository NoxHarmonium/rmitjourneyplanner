using RmitJourneyPlanner.CoreLibraries.DataProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
    }
}
