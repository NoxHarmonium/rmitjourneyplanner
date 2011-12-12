// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="LocationCache.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Allows high speed access and searching of multiple locations.
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
    using RmitJourneyPlanner.CoreLibraries.Positioning;

    #endregion

    /// <summary>
    /// Allows high speed access and searching of multiple <see cref="Location"/> objects for a network.
    /// </summary>
    internal class LocationCache : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   The SQL database that the cache utilizes.
        /// </summary>
        private readonly MySqlDatabase database;

        /// <summary>
        ///   The network identifier that is used to distinguish <see cref = "Location" /> objects in the cache.
        /// </summary>
        private readonly string networkId;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationCache"/> class.
        /// </summary>
        /// <param name="networkId">
        /// The ID of the calling transport network provider.
        /// </param>
        public LocationCache(string networkId)
        {
            this.database = new MySqlDatabase();
            this.networkId = networkId;
            this.database.Open();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="LocationCache"/> class.
        /// </summary>
        ~LocationCache()
        {
            this.Dispose();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a new cache entry into the location cache.
        /// </summary>
        /// <param name="id">
        /// A unique location identifer.
        /// </param>
        /// <param name="location">
        /// The Location object you want to cache.
        /// </param>
        /// <param name="routeId">
        /// The route identifier of the location if location impliments the <see cref="INetworkNode"/> interface. /&gt;
        /// </param>
        public void AddCacheEntry(string id, Location location, string routeId)
        {
            string query =
                string.Format(
                    "INSERT INTO LocationCache" + " (networkId,locationID,Latitude,Longitude,RouteId)"
                    + " VALUES('{0}','{1}',{2},{3},'{4}');", 
                    this.networkId, 
                    id, 
                    location.Latitude, 
                    location.Longitude, 
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
        /// Returns the identifier of the node at the specfied location.
        /// </summary>
        /// <param name="location">
        /// The <see cref="Location"/> object you want to retreive the identifier for.
        /// </param>
        /// <returns>
        /// The identfier of the given <see cref="Location"/>.
        /// </returns>
        public string GetIdFromLocation(Location location)
        {
            string query =
                string.Format(
                    "SELECT locationID FROM LocationCache WHERE " + "Latitude = {0} AND " + "Longitude = {1} AND "
                    + "networkId = '{2}';", 
                    location.Latitude, 
                    location.Longitude, 
                    this.networkId);

            DataTable table = this.database.GetDataSet(query);
            if (table.Rows.Count > 0)
            {
                return table.Rows[0]["locationID"].ToString();
            }

            return null;
        }

        /// <summary>
        /// Gets identifiers and corosponding route IDs within a certain distance from a central location.
        /// </summary>
        /// <param name="center">
        /// The center location of the search.
        /// </param>
        /// <param name="radius">
        /// The distance to search around the center location in kilometers.
        /// </param>
        /// <returns>
        /// An array of strings containing the matching identifiers.
        /// </returns>
        public List<string[]> GetIdsInRadius(Location center, double radius)
        {
            Location topLeft = GeometryHelper.Travel(center, 315.0, radius);
            Location bottomRight = GeometryHelper.Travel(center, 135.0, radius);

            string query =
                string.Format(
                    "SELECT locationID, RouteId FROM LocationCache WHERE " + "Latitude < {0} AND "
                    + "Longitude > {1} AND " + "Latitude  > {2} AND " + "Longitude < {3} AND " + "networkId = '{4}';", 
                    topLeft.Latitude, 
                    topLeft.Longitude, 
                    bottomRight.Latitude, 
                    bottomRight.Longitude, 
                    this.networkId);

            DataTable table = this.database.GetDataSet(query);

            var ids = new List<string[]>(table.Rows.Count);
            ids.AddRange(from DataRow row in table.Rows select new[] { row[0].ToString(), row[1].ToString() });

            return ids;
        }

        /// <summary>
        /// Gets the <see cref="Location"/> object corrosponding to a given identifier.
        /// </summary>
        /// <param name="id">
        /// The identifier of the <see cref="Location"/> object.
        /// </param>
        /// <returns>
        /// A <see cref="Location"/> object.
        /// </returns>
        public Location GetPostition(string id)
        {
            string query =
                string.Format(
                    "SELECT Latitude, Longitude FROM LocationCache WHERE " + "locationID = '{0}' AND "
                    + "networkId = '{1}';", 
                    id, 
                    this.networkId);

            DataTable table = this.database.GetDataSet(query);

            if (table.Rows.Count > 0)
            {
                return new Location(Convert.ToDouble(table.Rows[0][0]), Convert.ToDouble(table.Rows[0][0]));
            }

            return null;
        }

        /// <summary>
        /// Set up the database for the location cache. Clears all data.
        /// </summary>
        public void InitializeCache()
        {
            // Delete any old tables
            // database.RunQuery("IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='LocationCache') DROP TABLE LocationCache;");
            // Create new table
            this.database.RunQuery(Properties.Resources.rmitjourneyplanner_locationcache);
        }

        #endregion
    }
}