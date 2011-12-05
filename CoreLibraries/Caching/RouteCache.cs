// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="RouteCache.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Caches route data for public transport.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Caching
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using RmitJourneyPlanner.CoreLibraries.DataAccess;

    #endregion

    /// <summary>
    /// Caches route data for public transport.
    /// </summary>
    internal class RouteCache : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   The SQL database that the cache utilizes.
        /// </summary>
        private readonly MySqlDatabase database;

        /// <summary>
        ///   The network identifier which distinguishes different networks.
        /// </summary>
        private readonly string networkId;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteCache"/> class.
        /// </summary>
        /// <param name="networkId">
        /// The ID of the calling transport network provider.
        /// </param>
        public RouteCache(string networkId)
        {
            this.database = new MySqlDatabase();
            this.networkId = networkId;
            this.database.Open();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RouteCache"/> class.
        /// </summary>
        ~RouteCache()
        {
            this.Dispose();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a route to the cache.
        /// </summary>
        /// <param name="routeId">
        /// The route identifier.
        /// </param>
        /// <param name="ids">
        /// A list of node identifiers that make up the route.
        /// </param>
        /// <param name="isUpDirection">
        /// Specifies the direction of the route.
        /// </param>
        public void AddCacheEntry(string routeId, List<string> ids, bool isUpDirection)
        {
            // database.BeginTransaction();
            int index = 0;

            foreach (string id in ids)
            {
                string query =
                    string.Format(
                        "INSERT INTO RouteCache" + " (networkId,RouteId,stopID,stopIndex,stopDirection)"
                        + " VALUES('{0}','{1}','{2}','{3}','{4}');", 
                        this.networkId, 
                        routeId, 
                        id, 
                        index++, 
                        Convert.ToInt16(isUpDirection));
                this.database.RunQuery(query);
            }

            // database.EndTransaction();
        }

        /// <summary>
        /// Cleans up resources used by this object
        /// </summary>
        public void Dispose()
        {
            this.database.Close();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns a list of node identifiers that corrospond to the specifed route and direction.
        /// </summary>
        /// <param name="routeId">
        /// The route identifier.
        /// </param>
        /// <param name="isUpDirection">
        /// Specifies which direction of the route to use.
        /// </param>
        /// <returns>
        /// A list of node identifiers.
        /// </returns>
        public List<string> GetRoute(string routeId, bool isUpDirection)
        {
            string query =
                string.Format(
                    "SELECT RouteId,stopID,stopIndex,stopDirection " + "FROM RouteCache " + "WHERE RouteId='{0}' "
                    + "AND stopDirection='{1}' ", 
                    routeId, 
                    Convert.ToInt16(isUpDirection));

            DataTable data = this.database.GetDataSet(query);

            if (data.Rows.Count > 0)
            {
                return (from DataRow row in data.Rows select row["stopID"].ToString()).ToList();
            }

            return null;
        }

        /// <summary>
        /// Set up the database for the location cache. Clears all data.
        /// </summary>
        public void InitializeCache()
        {
            // Delete any old tables
            // database.RunQuery("DROP TABLE NodeCache;");
            // Create new table
            this.database.RunQuery(
                "CREATE TABLE IF NOT EXISTS `rmitjourneyplanner`.`RouteCache` ( "
                + "`cacheID` INT UNSIGNED NOT NULL AUTO_INCREMENT ," + "`networkId` VARCHAR(45) NULL, "
                + "`RouteId` VARCHAR(45) NULL ," + "`stopID` VARCHAR(45) NULL ," + "`stopIndex` INT UNSIGNED ,"
                + "`stopDirection` INT UNSIGNED ," + "PRIMARY KEY (`cacheID`) ," + "INDEX `RouteId` (`RouteId` ASC)) "
                + "PACK_KEYS = 1;" + "DELETE FROM RouteCache;");
        }

        #endregion
    }
}