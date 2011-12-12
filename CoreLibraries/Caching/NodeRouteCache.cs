// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="NodeRouteCache.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Caches the routes that intersect a node.
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
    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    #endregion

    /// <summary>
    /// Caches the the relationships between nodes and routes. Used to determine what routes intersect a given node.
    /// </summary>
    internal class NodeRouteCache : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   The SQL database that the cache utilizes.
        /// </summary>
        private readonly MySqlDatabase database;

        /// <summary>
        ///   The network identifier that is used to distinguish the node/route relations.
        /// </summary>
        private readonly string networkId;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeRouteCache"/> class.
        /// </summary>
        /// <param name="networkId">
        /// The ID of the calling transport network provider.
        /// </param>
        public NodeRouteCache(string networkId)
        {
            this.database = new MySqlDatabase();
            this.networkId = networkId;
            this.database.Open();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="NodeRouteCache"/> class.
        /// </summary>
        ~NodeRouteCache()
        {
            this.Dispose();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a new cache entry into the node/route cache.
        /// </summary>
        /// <param name="node">
        /// The node that is to be added to the cache.
        /// </param>
        /// <param name="routeId">
        /// The route identifier that will be associated with the node.
        /// </param>
        public void AddCacheEntry(INetworkNode node, string routeId)
        {
            string query =
                string.Format(
                    "INSERT INTO NodeRouteCache" + " (networkId,nodeId,RouteId)" + " VALUES('{0}','{1}','{2}');", 
                    this.networkId, 
                    node.Id, 
                    routeId);
            this.database.RunQuery(query);
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
        /// Gets a list of routes that intersect with the given node.
        /// </summary>
        /// <param name="node">
        /// The node for which you want to find intersecting routes.
        /// </param>
        /// <returns>
        /// A list of route identifiers that corrospond to the routes that intersect the given node.
        /// </returns>
        public List<string> GetRoutes(INetworkNode node)
        {
            string query = string.Format("SELECT RouteId FROM NodeRouteCache" + " where nodeId='{0}';", node.Id);

            DataTable result = this.database.GetDataSet(query);
            if (result == null || result.Rows.Count == 0)
            {
                return null;
            }

            return (from DataRow row in result.Rows select row[0].ToString()).ToList();
        }

        /// <summary>
        /// Set up the database for the node/route cache. Clears all data.
        /// </summary>
        public void InitializeCache()
        {
            // Delete any old tables
            // database.RunQuery("IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='LocationCache') DROP TABLE LocationCache;");
            // Create new table
            this.database.RunQuery(Properties.Resources.rmitjourneyplanner_noderoutecache);
        }

        #endregion
    }
}