// -----------------------------------------------------------------------
// <copyright file="ArcCache.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Types;
    using DataProviders;
    using Positioning;
    using System.Data;
    using System.IO;
    /// <summary>
    /// A cache for arcs.
    /// </summary>
    public class ArcCache : IDisposable
    {

         DataAccess.MySqlDatabase database;
        private string networkID;


        /// <summary>
        /// Initilizes a new arc cache.
        /// </summary>
        /// <param name="networkID">The identifier of the network.</param>
        public ArcCache(string networkID)
        {
            database = new DataAccess.MySqlDatabase();
            database.Open();
            this.networkID = networkID;
        }
        
        /// <summary>
        /// Set up the database for the arc cache. Clears all data.
        /// </summary>
        public void InitializeCache()
        {
            //Delete any old tables
            //database.RunQuery("IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='LocationCache') DROP TABLE LocationCache;");
            //Create new table
            database.RunQuery("CREATE TABLE IF NOT EXISTS `rmitjourneyplanner`.`ArcCache` ( " +
                                "`cacheID` INT UNSIGNED NOT NULL AUTO_INCREMENT ," +
                                "`networkID` VARCHAR(45) NULL, " +
                                "`source` VARCHAR(45) NULL ," +
                                "`destination` VARCHAR(45) NULL ," +
                                "`time` INT NULL ," +
                                "`distance` DOUBLE NULL ," +
                                "`requestedTime` DATETIME NULL ," +
                                "`departureTime` DATETIME NULL ," +
                                "`transportMode` VARCHAR(45) NULL ," +
                                "`routeID` VARCHAR(45) NULL ," +
                                "PRIMARY KEY (`cacheID`) ," +
                                "INDEX `sd` (`Source`,`Destination`  ASC) ," +
                                "INDEX `dp` (`departureTime` ASC) )" +
                                "PACK_KEYS = 1; DELETE FROM ArcCache;");

        }

        /// <summary>
        /// Adds a new cache entry into the arc cache.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="location"></param>
        public void AddCacheEntry(DateTime requestedDepartureDate, Arc arc)
        {
            if (arc.Time.Seconds < 0)
            {
                throw new Exception("wtf");
            }
            string query = String.Format("INSERT INTO ArcCache" +
                                            " (networkID,source,destination,time,distance,requestedTime, departureTime,transportMode,routeID)" +
                                            " VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');",
                                            networkID,
                                            arc.Source.ToString(),
                                            arc.Destination.ToString(),
                                            arc.Time.TotalSeconds,
                                            arc.Distance,
                                            requestedDepartureDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                            arc.DepartureTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                            arc.TransportMode,
                                            arc.RouteId);
            database.RunQuery(query);
        }

        public List<Arc> GetArcs(Location source, Location destination, DateTime requestedTime, string routeId)
        {
            if (routeId == null)
            {
                routeId = "";
            }
            
            string query = String.Format("SELECT * FROM ArcCache WHERE " +
                                       "source='{0}' AND " +
                                       "destination='{1}' AND " +
                                       "requestedTime='{2}' AND " +
                                       "routeId='{3}';",
                                       source.ToString(),
                                       destination.ToString(),
                                       requestedTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                       routeId);
           
            DataTable result = database.GetDataSet(query);
            if (result.Rows.Count < 1)
            {
                return new List<Arc>();
            }
            else
            {
                List<Arc> arcs = new List<Arc>();
                foreach (DataRow row in result.Rows)
                {
                Arc arc = new Arc(Location.Parse(row["source"].ToString()),
                                Location.Parse(row["destination"].ToString()),
                                new TimeSpan(0,0,Convert.ToInt32(row["time"])),
                                Convert.ToDouble(row["distance"].ToString()),
                                DateTime.Parse(row["departureTime"].ToString()),
                                row["transportMode"].ToString(),
                                row["routeId"].ToString());
                    arcs.Add(arc);
                }
                return arcs;

            }


        }

        public void Dispose()
        {
            database.Close();
        }
    }
}
