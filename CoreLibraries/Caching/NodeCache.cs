using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RmitJourneyPlanner.CoreLibraries.DataProviders;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Data;

namespace RmitJourneyPlanner.CoreLibraries.Caching
{
    class NodeCache<T> where T : INetworkNode
    {
        DataAccess.MySqlDatabase database;
        private string networkID;



        /// <summary>
        /// Set up the database for the location cache. Clears all data.
        /// </summary>
        public void InitializeCache()
        {
            //Delete any old tables
            database.RunQuery("DROP TABLE NodeCache;");
            //Create new table
            database.RunQuery("CREATE  TABLE `rmitjourneyplanner`.`NodeCache` ( " +
                                "`cacheID` INT UNSIGNED NOT NULL AUTO_INCREMENT ," +
                                "`networkID` VARCHAR(45) NULL, " +
                                "`stopID` VARCHAR(45) NULL ," +
                                "`stopObject` TEXT NULL ," +
                                "PRIMARY KEY (`cacheID`) ," +
                                "INDEX `stopID` (`stopID` ASC) ," +
                                "PACK_KEYS = 1;");





        }

        public void AddCacheEntry(T node)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, node);
               
                string query = String.Format("INSERT INTO NodeCache" +
                                            " (networkID,stopID,stopObject)" +
                                            " VALUES('{0}','{1}','{2}');",
                                            ((INetworkNode)node).ID,
                                            networkID,
                                            writer.ToString());
                database.RunQuery(query);
            }

        }

        public T GetNode(string id)
        {

            string query = String.Format("SELECT stopObject FROM NodeCache WHERE " +
                                        "networkID='{0}' AND " +
                                        "stopID='{1}';",
                                        networkID,
                                        id);
            DataTable result = database.GetDataSet(query);
            if (result.Rows.Count < 1)
            {
                return default(T);
            }
            else if (result.Rows.Count > 1)
            {
                throw new InvalidDataException("The there is more than one record for the specified id.");
            }
            else
            {
                return (T)result.Rows[0][0];
            }

              

            

        }

    }
}
