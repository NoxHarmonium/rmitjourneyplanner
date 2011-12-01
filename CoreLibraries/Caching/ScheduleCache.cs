// -----------------------------------------------------------------------
// <copyright file="ScheduleCaching.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DataProviders;
    using Types;
    using System.Data;
    using System.IO;
    using System.Web;
    /// <summary>
    /// Caches Yarra Trams schedule data.
    /// </summary>
    public class ScheduleCache : IDisposable
    {

         DataAccess.MySqlDatabase database;
        private string networkID;

        /// <summary>
        /// Initilizes a new node cache.
        /// </summary>
        /// <param name="networkID">The identifier of the data source.</param>
        public ScheduleCache(string networkID)
        {
            database = new DataAccess.MySqlDatabase();
            this.networkID = networkID;
            database.Open();
        }

        /// <summary>
        /// Set up the database for the location cache. Clears all data.
        /// </summary>
        public void InitializeCache()
        {
            //Delete any old tables
            //database.RunQuery("DROP TABLE NodeCache;");
            //Create new table
            database.RunQuery("CREATE TABLE IF NOT EXISTS `rmitjourneyplanner`.`ScheduleCollectionCache` ( " +
                                "`cacheID` INT UNSIGNED NOT NULL AUTO_INCREMENT ," +
                                "`networkID` VARCHAR(45) NULL, " +
                                "`stopID` VARCHAR(45) NULL ," +
                                "`routeID` VARCHAR(45) NULL ," +
                                "`lowFloor` INT UNSIGNED ," +
                                "`requestDate` DATETIME NULL ," +
                                "`data` TEXT ," +
                                "PRIMARY KEY (`cacheID`)) " +
                                "PACK_KEYS = 1;" +
                                "DELETE FROM ScheduleCollectionCache;"
                                );

            database.RunQuery("CREATE TABLE IF NOT EXISTS `rmitjourneyplanner`.`TripScheduleCache` ( " +
                               "`cacheID` INT UNSIGNED NOT NULL AUTO_INCREMENT ," +
                               "`networkID` VARCHAR(45) NULL, " +
                               "`tripID` INT UNSIGNED ," +
                               "`scheduledDateTime` DATETIME ," +
                               "`data` TEXT ," +
                               "PRIMARY KEY (`cacheID`)) " +
                               "PACK_KEYS = 1;" +
                               "DELETE FROM TripScheduleCache;"
                               );

            



        }

        public void AddTripSchedule(string tripID, DateTime scheduledDateTime,DataSet data)
        {
            using (StringWriter writer = new StringWriter())
            {
                data.WriteXml(writer, XmlWriteMode.WriteSchema);

                string query = string.Format("INSERT INTO TripScheduleCache" +
                                        " (networkID,tripID,scheduledDateTime,data)" +
                                        " VALUES('{0}','{1}','{2}','{3}');",
                                        networkID,
                                        tripID,
                                        scheduledDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                System.Security.SecurityElement.Escape(
                                        writer.ToString()));
                database.RunQuery(query);
            }

        }

        public DataSet GetTripSchedule(string tripID, DateTime scheduledDateTime)
        {
            string query = string.Format("SELECT data FROM TripScheduleCache" +
                                        " WHERE networkID='{0}' AND " +
                                        " tripID='{1}' AND " +
                                        "scheduledDateTime='{2}'",
                                        networkID,
                                        tripID,
                                        scheduledDateTime.ToString("yyyy-MM-dd HH:mm:ss"));

            DataTable result = database.GetDataSet(query);
            if (result.Rows.Count == 0)
            {
                return null;
            }
            else if (result.Rows.Count > 1)
            {
                throw new InvalidDataException("The there is more than one record for the specified parameters.");
            }
            else
            {
                string xmlData = result.Rows[0]["data"].ToString();
                xmlData = CacheTools.Deescape(xmlData);
                StringReader reader = new StringReader(xmlData);
                DataSet schedule = new DataSet();
                schedule.ReadXml(reader, XmlReadMode.ReadSchema);
                return schedule;
            }


        }

        /// <summary>
        /// Adds a schedule collection to the cache.
        /// </summary>
        public void AddScheduleCollection(INetworkNode node, string routeId,bool lowFloor,DateTime requestDate,DataSet data)
        {
            //database.BeginTransaction();
            using (StringWriter writer = new StringWriter())
            {
                data.WriteXml(writer, XmlWriteMode.WriteSchema);

                string query = string.Format("INSERT INTO ScheduleCollectionCache" +
                                        " (networkID,stopID,routeID,lowFloor,requestDate,data)" +
                                        " VALUES('{0}','{1}','{2}','{3}','{4}','{5}');",
                                        networkID,
                                        node.ID,
                                        routeId,
                                        Convert.ToInt32(lowFloor),
                                        requestDate.ToString("yyyy-MM-dd HH:mm:ss"),
                System.Security.SecurityElement.Escape(
                                        writer.ToString()));
                database.RunQuery(query);
            }

           // database.EndTransaction();

        }

        public DataSet GetScheduleCollection(INetworkNode node, string routeId, bool lowFloor, DateTime requestDate)
        {
            string query = string.Format("SELECT data FROM ScheduleCollectionCache" +
                                        " WHERE networkID='{0}' AND " +
                                        " stopID='{1}' AND " +
                                        "routeID='{2}' AND " + 
                                        "lowFloor='{3}' AND "+ 
                                        "requestDate='{4}';",
                                        networkID,
                                        node.ID,
                                        routeId,
                                        Convert.ToInt32(lowFloor),
                                        requestDate.ToString("yyyy-MM-dd HH:mm:ss"));

            DataTable result = database.GetDataSet(query);
            if (result.Rows.Count == 0)
            {
                return null;
            }
            else if (result.Rows.Count > 1)
            {
                throw new InvalidDataException("The there is more than one record for the specified parameters.");
            }
            else
            {
                string xmlData = result.Rows[0]["data"].ToString();
                xmlData = CacheTools.Deescape(xmlData);
                StringReader reader = new StringReader(xmlData);
                DataSet schedule = new DataSet();
                schedule.ReadXml(reader, XmlReadMode.ReadSchema);
                return schedule;
            }


        }

        public void Dispose()
        {
            database.Close();
        }
    }
}
