using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RmitJourneyPlanner.CoreLibraries.Types;
using RmitJourneyPlanner.CoreLibraries.DataProviders;


namespace RmitJourneyPlanner.CoreLibraries.Caching
{
    /// <summary>
    /// Caches route data for public transport.
    /// </summary>
    class RouteCache
    {
        DataAccess.MySqlDatabase database;
        private string networkID;

        /// <summary>
        /// Initilizes a new node cache.
        /// </summary>
        /// <param name="networkID">The identifier of the data source.</param>
        public RouteCache(string networkID)
        {
            database = new DataAccess.MySqlDatabase();
            this.networkID = networkID;
        }

        /// <summary>
        /// Set up the database for the location cache. Clears all data.
        /// </summary>
        public void InitializeCache()
        {
            //Delete any old tables
            //database.RunQuery("DROP TABLE NodeCache;");
            //Create new table
            database.RunQuery("CREATE TABLE IF NOT EXISTS `rmitjourneyplanner`.`RouteCache` ( " +
                                "`cacheID` INT UNSIGNED NOT NULL AUTO_INCREMENT ," +
                                "`networkID` VARCHAR(45) NULL, " +
                                "`routeID` VARCHAR(45) NULL ," +
                                "`stopID` VARCHAR(45) NULL ," +
                                "`stopIndex` INT UNSIGNED ," +
                                "`stopDirection` INT UNSIGNED ," +
                                "PRIMARY KEY (`cacheID`) ," +
                                "INDEX `routeID` (`routeID` ASC)) " +
                                "PACK_KEYS = 1;");



        }

        /// <summary>
        /// Adds a route to the cache.
        /// </summary>
        /// <param name="route"></param>
        public void AddCacheEntry(Route route)
        {
            //database.BeginTransaction();

            foreach (bool direction in new bool[] { true, false })
            {
                int index = 0;
                foreach (INetworkNode node in route.GetNodes(direction))
                {
                    string query = string.Format("INSERT INTO RouteCache" +
                                            " (networkID,routeID,stopID,stopIndex,stopDirection)" +
                                            " VALUES('{0}','{1}','{2}','{3}','{4}');",
                                            networkID,
                                            route.ID,
                                            node.ID,
                                            index++,
                                            Convert.ToInt16(direction));
                    database.RunQuery(query);
                }
            }

           // database.EndTransaction();

        }

        /// <summary>
        /// Returns a list of IDs the corrospond to the specifed route a direction.
        /// </summary>
        /// <param name="routeId"></param>
        /// <param name="isUpDirection"></param>
        /// <returns></returns>
        public List<string> GetRoute(string routeId,bool isUpDirection)
        {
            
            
            
                string query = string.Format("SELECT routeID,stopID,stopIndex,stopDirection " +
                                            "FROM RouteCache " + 
                                            "WHERE routeID='{0}' " +
                                            "AND stopDirection='{1}' " +
                                            "ORDER BY stopIndex",
                                            routeId,
                                            Convert.ToInt16(isUpDirection));

                DataTable data = database.GetDataSet(query);

                List<string> ids = new List<string>();
                
                foreach (DataRow row in data.Rows)
                {
                    ids.Add(row["routeID"].ToString());
                }

                return ids;
            
            
        } 


    }
}
