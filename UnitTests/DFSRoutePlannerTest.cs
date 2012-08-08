using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.RouteGenerators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
using RmitJourneyPlanner.CoreLibraries.DataProviders;
using RmitJourneyPlanner.CoreLibraries.Types;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for DFSRoutePlannerTest and is intended
    ///to contain all DFSRoutePlannerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DFSRoutePlannerTest
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
        ///A test for DFSRoutePlanner Constructor
        ///</summary>
        [TestMethod()]
        public void DFSRoutePlannerConstructorTest()
        {
            EvolutionaryProperties properties = null; // TODO: Initialize to an appropriate value
            SearchType searchType = new SearchType(); // TODO: Initialize to an appropriate value
            DFSRoutePlanner target = new DFSRoutePlanner(properties, searchType);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Generate
        ///</summary>
        [TestMethod()]
        public void GenerateTest()
        {
            EvolutionaryProperties properties = null; // TODO: Initialize to an appropriate value
            SearchType searchType = new SearchType(); // TODO: Initialize to an appropriate value
            DFSRoutePlanner target = new DFSRoutePlanner(properties, searchType); // TODO: Initialize to an appropriate value
            INetworkNode source = null; // TODO: Initialize to an appropriate value
            INetworkNode destination = null; // TODO: Initialize to an appropriate value
            DateTime startTime = new DateTime(); // TODO: Initialize to an appropriate value
            Route expected = null; // TODO: Initialize to an appropriate value
            Route actual;
            actual = target.Generate(source, destination, startTime);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
