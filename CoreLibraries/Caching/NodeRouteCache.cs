// -----------------------------------------------------------------------
// <copyright file="NodeRouteCache.cs" company="">
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
    /// <summary>
    /// Caches the routes that intersect a node.
    /// </summary>
    public class NodeRouteCache
    {


        DataAccess.MySqlDatabase database;
        private string networkID;


        /// <summary>
        /// Initilizes a new node/route cache.
        /// </summary>
        /// <param name="networkID">The identifier of the network.</param>
        public NodeRouteCache(string networkID)
        {
            database = new DataAccess.MySqlDatabase();
            this.networkID = networkID;
        }
        
        /// <summary>
        /// Set up the database for the node/route cache. Clears all data.
        /// </summary>
        public void InitializeCache()
        {
            //Delete any old tables
            //database.RunQuery("IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='LocationCache') DROP TABLE LocationCache;");
            //Create new table
            database.RunQuery("CREATE TABLE IF NOT EXISTS `rmitjourneyplanner`.`NodeRouteCache` ( " +
                                "`cacheID` INT UNSIGNED NOT NULL AUTO_INCREMENT ," +
                                "`networkID` VARCHAR(45) NULL, " +
                                "`nodeId` VARCHAR(45) NULL ," +
                                "`routeId` VARCHAR(45) NULL ," +
                                "PRIMARY KEY (`cacheID`) ," +
                                "INDEX `sd` (`NodeId` ASC)) " +
                                "PACK_KEYS = 1; DELETE FROM NodeRouteCache;");

            


        }

        /// <summary>
        /// Adds a new cache entry into the node/route cache.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="location"></param>
        public void AddCacheEntry(INetworkNode node, string routeId)
        {
           
            string query = String.Format("INSERT INTO NodeRouteCache" +
                                            " (networkID,nodeId,routeId)" +
                                            " VALUES('{0}','{1}','{2}');",
                                            networkID,
                                            node.ID,
                                            routeId);
            database.RunQuery(query);
        }

        public List<string> GetRoutes(INetworkNode node)
        {
            string query = String.Format("SELECT routeId FROM NodeRouteCache" +
                                            " where nodeId='{0}';",
                                            node.ID);

            DataTable result = database.GetDataSet(query);
            if (result == null || result.Rows.Count == 0)
            {
                return null;
            }
            List<string> routes = new List<string>();
            foreach (DataRow row in result.Rows)
            {
                routes.Add(row[0].ToString());
            }

            return routes;

        }

    }
}
