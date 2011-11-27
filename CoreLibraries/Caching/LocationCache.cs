﻿// -----------------------------------------------------------------------
// <copyright file="LocationCache.cs" company="RMIT University">
//  By Sean Dawson
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using DataProviders;

    /// <summary>
    /// Allows high speed access and searching of multiple locations.
    /// </summary>
    public class LocationCache
    {

        DataAccess.MySqlDatabase database;
        private string networkID;


        /// <summary>
        /// Initilizes a new location cache.
        /// </summary>
        /// <param name="networkID">The identifier of the network.</param>
        public LocationCache(string networkID)
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
            //database.RunQuery("IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='LocationCache') DROP TABLE LocationCache;");
            //Create new table
            database.RunQuery("CREATE TABLE IF NOT EXISTS `rmitjourneyplanner`.`LocationCache` ( " +
                                "`cacheID` INT UNSIGNED NOT NULL AUTO_INCREMENT ," +                                
                                "`locationID` VARCHAR(45) NULL ," +
                                "`networkID` VARCHAR(45) NULL, " +
                                "`Latitude` DOUBLE NULL ," +
                                "`Longitude` DOUBLE NULL ," +
                                "PRIMARY KEY (`cacheID`) ," +
                                "INDEX `Latitude` (`Latitude` ASC) ," +
                                "INDEX `Longitude` (`Longitude` ASC) )" +
                                "PACK_KEYS = 1;");





        }
        
        public void AddCacheEntry(string id, Location location)
        {
            string query = String.Format("INSERT INTO LocationCache" +
                                            " (networkID,locationID,Latitude,Longitude)" + 
                                            " VALUES({0},'{1}',{2},{3});" ,
                                            networkID,
                                            id,                                           
                                            location.Latitude,
                                            location.Longitude);
            database.RunQuery(query);
        }

        /// <summary>
        /// Gets the transport node ids within a certain distance from a central location.
        /// </summary>
        /// <param name="center">The center location</param>
        /// <param name="radius">The distance to search around the center location in kilometers.</param>
        /// <returns></returns>
        public List<string> GetIdsInRadius(Location center, double radius)
        {
            Location topLeft = GeometryHelper.Travel(center, 315.0, radius);
            Location bottomRight = GeometryHelper.Travel(center, 135.0, radius);

            string query = String.Format("SELECT locationID FROM LocationCache WHERE " +
                                            "Latitude < {0} AND " +
                                            "Longitude > {1} AND " +
                                            "Latitude  > {2} AND " +
                                            "Longitude < {3} AND " + 
                                            "networkID = '{4}';",
                                            topLeft.Latitude,
                                            topLeft.Longitude,
                                            bottomRight.Latitude,
                                            bottomRight.Longitude,
                                            networkID);

            DataTable table = database.GetDataSet(query);

            List<string> ids = new List<string>(table.Rows.Count);
            foreach (DataRow row in table.Rows)
            {
                ids.Add(row[0].ToString());
            }
            

            return ids;


        }

        /// <summary>
        /// Gets the position of the specified location id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Location GetPostition(string id)
        {
            string query = String.Format("SELECT Latitude, Longitude FROM LocationCache WHERE " +
                                            "locationID = '{0}' AND " +                                          
                                            "networkID = '{1}';",
                                            id,                                            
                                            networkID);

            DataTable table = database.GetDataSet(query);

            if (table.Rows.Count > 0)
            {
                return new Location(Convert.ToDouble(table.Rows[0][0]), Convert.ToDouble(table.Rows[0][0]));
            }
            else
            {
                return null;
            }

        }

    }
}
