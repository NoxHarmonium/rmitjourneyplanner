using RmitJourneyPlanner.CoreLibraries.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;
using System.Data;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Reflection;

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
        /// Set this to true to dump the dataset results into the folder specified by dumpFolder.
        /// </summary>
        private bool dumpResults = false;
        /// <summary>
        /// Specifies the folder that the dataset results will be dumped into when dumpResults is true.
        /// </summary>
        private const string dumpFolder = @"C:\temp\yarratramsdatasets";

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


        private void doDumpResults(DataSet results, object[] parameters)
        {
            
            if (dumpResults)
            {
                StackTrace stackTrace = new StackTrace();

                // get calling method name
                string calling_name = stackTrace.GetFrame(1).GetMethod().Name;
                //rel="stylesheet" href="../csstg.css" type="text/css">

               
                       


            
                //StreamWriter w = new StreamWriter(dumpFolder + "\\" + results.Tables[0].TableName + ".html");
                XmlDocument doc = new XmlDocument();

                XmlNode html = doc.CreateElement("html");

                XmlNode head = doc.CreateElement("head");
                XmlNode css = doc.CreateElement("link");
                XmlAttribute rel = doc.CreateAttribute("rel");
                XmlAttribute href = doc.CreateAttribute("href");
                XmlAttribute type = doc.CreateAttribute("type");
                rel.Value = "stylesheet";
                href.Value = @"dataset.css";
                type.Value = @"text/css";
                css.Attributes.Append(rel);
                css.Attributes.Append(href);
                css.Attributes.Append(type);
                

                XmlNode body = doc.CreateElement("body");
                
                XmlNode title = doc.CreateElement("h1");
                title.InnerText = calling_name;

                XmlNode paramList = doc.CreateElement("ul");

                MethodInfo[] methodInfos = typeof(TramTrackerApi).GetMethods();
                int count = 0;
                foreach (MethodInfo info in methodInfos)
                {
                    if (calling_name.Contains(info.Name))
                    {
                        foreach (ParameterInfo p in info.GetParameters())
                        {
                            XmlNode nP = doc.CreateElement("li");
                            nP.InnerText = p.Name + " = " + parameters[count++].ToString();
                            paramList.AppendChild(nP);
                        }
                    }
                }



                
                XmlNode table = doc.CreateElement("table");
                
                XmlNode tableHeader = doc.CreateElement("tr");
                foreach (DataColumn col in results.Tables[0].Columns)
                {
                    XmlNode headerCell = doc.CreateElement("th");
                    headerCell.InnerText = col.ColumnName;
                    tableHeader.AppendChild(headerCell);
                }
                table.AppendChild(tableHeader);
                foreach (DataRow row in results.Tables[0].Rows)
                {
                    XmlNode tableRow = doc.CreateElement("tr");
                    foreach (object item in row.ItemArray)
                    {
                        XmlNode tableCell = doc.CreateElement("td");
                        tableCell.InnerText = item.ToString();
                        tableRow.AppendChild(tableCell);
                    }
                    table.AppendChild(tableRow);
                }

                head.AppendChild(css);
                html.AppendChild(head);
                
                body.AppendChild(title);
                body.AppendChild(paramList);
                body.AppendChild(table);
                html.AppendChild(body);
                doc.AppendChild(html);

                doc.Save(dumpFolder + "\\" + results.Tables[0].TableName + ".html");

            }
        }

        /// <summary>
        ///A test for Guid
        ///</summary>
        [TestMethod()]
        public void GuidTest()
        {
            TramTrackerApi target = new TramTrackerApi(); 
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
            TramTrackerApi target = new TramTrackerApi(); 
            DataSet result = target.GetDestinationsForAllRoutes();
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count > 0);
            Assert.IsTrue(result.Tables[0].Select("RouteNo=1 AND UpStop=false AND Destination='East Coburg'").Length > 0);
            doDumpResults(result, new object[] {});

        }

        /// <summary>
        ///A test for GetDestinationsForRoute
        ///</summary>
        [TestMethod()]
        public void GetDestinationsForRouteTest()
        {
            TramTrackerApi target = new TramTrackerApi(); 
            string routeId = "1"; 
            DataSet result = target.GetDestinationsForRoute(routeId);
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count > 0);
            Assert.IsTrue(((string)result.Tables[0].Rows[0][0]) == "Sth Melb Beach");
            Assert.IsTrue(((string)result.Tables[0].Rows[0][1]) == "East Coburg");

            doDumpResults(result, new object[] { routeId });
        }

        /// <summary>
        ///A test for GetListOfStopsByRouteNoAndDirection
        ///</summary>
        [TestMethod()]
        public void GetListOfStopsByRouteNoAndDirectionTest()
        {
            TramTrackerApi target = new TramTrackerApi(); // TODO: Initialize to an appropriate value
            string routeId = "1"; 
            bool isUpDirection = false; 
            DataSet result = target.GetListOfStopsByRouteNoAndDirection(routeId, isUpDirection);
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count == 54);
            doDumpResults(result, new object[] {routeId, isUpDirection });
        }

        /// <summary>
        ///A test for GetMainRoutes
        ///</summary>
        [TestMethod()]
        public void GetMainRoutesTest()
        {
            TramTrackerApi target = new TramTrackerApi();            
            DataSet result = target.GetMainRoutes();
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count == 30);
            doDumpResults(result, new object[] {});
        }

        /// <summary>
        ///A test for GetMainRoutesForStop
        ///</summary>
        [TestMethod()]
        public void GetMainRoutesForStopTest()
        {
            TramTrackerApi target = new TramTrackerApi(); 
            string stopNo = "1551";             
            DataSet result = target.GetMainRoutesForStop(stopNo);
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows[0][0].ToString() == "8");
            doDumpResults(result, new object[] { stopNo });
        }

        /// <summary>
        ///A test for GetNextPredictedArivalTime
        ///</summary>
        [TestMethod()]
        public void GetNextPredictedArivalTimeTest()
        {
           // Assert.Inconclusive("Don't know how to find tram number.");
            TramTrackerApi target = new TramTrackerApi();
            string tramNo = "3001"; //C Class - Citadis    
            DataSet result = target.GetNextPredictedArivalTime(tramNo);
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count > 0);
            doDumpResults(result, new object[] { tramNo });
            
        }

        /// <summary>
        ///A test for GetNextPredictedRoutesCollection
        ///</summary>
        [TestMethod()]
        public void GetNextPredictedRoutesCollectionTest()
        {
            TramTrackerApi target = new TramTrackerApi();
            string stopNo = "1551";
            string routeNo = "8"; 
            bool lowFloor = false;
            DataSet result = target.GetNextPredictedRoutesCollection(stopNo, routeNo, lowFloor);

            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count > 0);
            doDumpResults(result,new object[] {stopNo,routeNo,lowFloor});
        }

        /// <summary>
        ///A test for GetSchedulesCollection
        ///</summary>
        [TestMethod()]
        public void GetSchedulesCollectionTest()
        {
            TramTrackerApi target = new TramTrackerApi(); 
            string stopNo = "1551"; 
            string routeNo = "8";
            bool lowFloor = false;
            DateTime requestTime = DateTime.Now;
            DataSet result = target.GetSchedulesCollection(stopNo, routeNo, lowFloor,requestTime);
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count > 0);
            doDumpResults(result, new object[] { stopNo, routeNo, lowFloor,requestTime });
        }

        /// <summary>
        ///A test for GetSchedulesForTrip
        ///</summary>
        [TestMethod()]
        public void GetSchedulesForTripTest()
        {


            TramTrackerApi target = new TramTrackerApi();
             DateTime scheduledDateTime = DateTime.Now;
             DataSet schedules = target.GetSchedulesCollection("1551", "8", false, scheduledDateTime);
            string tripID = schedules.Tables[0].Rows[0][1].ToString();
            //Assert.Inconclusive("Need to use the GetSchedulesCollection to get a trip id.");            
            
            
            DataSet result = target.GetSchedulesForTrip(tripID, scheduledDateTime);
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count > 0);
            doDumpResults(result, new object[] { tripID, scheduledDateTime });
        }

        /// <summary>
        ///A test for GetStopInformation
        ///</summary>
        [TestMethod()]
        public void GetStopInformationTest()
        {
            TramTrackerApi target = new TramTrackerApi();
            string stopNo = "1551";
            
            DataSet result = target.GetStopInformation(stopNo);
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count > 0);
            //Test with some random facts about stop
            Assert.IsTrue(result.Tables[0].Rows[0]["SuburbName"].ToString() == "Coburg");
            Assert.IsTrue(result.Tables[0].Rows[0]["CityDirection"].ToString() == "towards City");
            doDumpResults(result, new object[] { stopNo });
        }

        /// <summary>
        ///A test for GetStopsAndRouteUpdatesSince
        ///</summary>
        [TestMethod()]
        public void GetStopsAndRouteUpdatesSinceTest()
        {
            TramTrackerApi target = new TramTrackerApi(); 
            DateTime dateSince = DateTime.Parse("2/5/2010"); 
            DataSet result = target.GetStopsAndRoutesUpdatesSince(dateSince);           
            Assert.IsTrue(result.Tables.Count > 0);
            Assert.IsTrue(result.Tables[0].Rows.Count > 0);
            doDumpResults(result, new object[] { dateSince });
        }
    }
}
