// -----------------------------------------------------------------------
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
    using RmitJourneyPlanner.CoreLibraries.Positioning;

    /// <summary>
    /// Allows high speed access and searching of multiple locations.
    /// </summary>
    public class LocationCache
    {

        DataAccess.MySQLDatabase database = new DataAccess.MySQLDatabase();
        
        /// <summary>
        /// Set up the database for the location cache. Clears all data.
        /// </summary>
        public void InitializeCache()
        {
            //Delete any old tables
            database.RunQuery("DROP TABLE LocationCache;");
            //Create new table
            database.RunQuery("CREATE  TABLE `rmitjourneyplanner`.`LocationCache` ( " +                                "`cacheID` INT UNSIGNED NOT NULL AUTO_INCREMENT ," +                                "`locationID` VARCHAR(45) NULL ," +                                "`Latitude` DOUBLE NULL ," +                                "`Longitude` DOUBLE NULL ," +                                "PRIMARY KEY (`cacheID`) ," +                                "INDEX `Latitude` (`Latitude` ASC) ," +                                "INDEX `Longitude` (`Longitude` ASC) )" +                                "PACK_KEYS = 1;");




        }
        
        public void AddCacheEntry(string id, Location location)
        {
            string query = String.Format("INSERT INTO LocationCache" +
                                            " VALUES('',{0},{1},{2});" ,
                                            id,
                                            location.Latitude,
                                            location.Longitude);
            database.RunQuery(query);
        }

        public string GetIdsInRadius(Location center, double radius)
        {



            return null;


        }

    }
}
