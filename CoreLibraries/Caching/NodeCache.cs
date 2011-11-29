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
    class NodeCache <T> where T : INetworkNode
    {
        DataAccess.MySqlDatabase database;
        private string networkID;

        /// <summary>
        /// Initilizes a new node cache.
        /// </summary>
        /// <param name="networkID">The identifier of the data source.</param>
        public NodeCache(string networkID)
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
            database.RunQuery("CREATE TABLE IF NOT EXISTS `rmitjourneyplanner`.`NodeCache` ( " +
                                "`cacheID` INT UNSIGNED NOT NULL AUTO_INCREMENT ," +
                                "`networkID` VARCHAR(45) NULL, " +
                                "`stopID` VARCHAR(45) NULL ," +
                                "`stopObject` TEXT ," +
                                "PRIMARY KEY (`cacheID`) ," +
                                "INDEX `stopID` (`stopID` ASC)) " +
                                "PACK_KEYS = 1; DELETE FROM NodeCache;");





        }

        public void AddCacheEntry(string id, DataSet data)
        {
            
            using (StringWriter writer = new StringWriter())
            {
                data.WriteXml(writer,XmlWriteMode.WriteSchema);
                string xml = writer.ToString();
                xml = xml.Replace("'", "\\'");
                string query = String.Format("INSERT INTO NodeCache" +
                                            " (networkID,stopID,stopObject)" +
                                            " VALUES('{0}','{1}','{2}');",
                                            networkID,
                                            id,
                                            writer.ToString());
                database.RunQuery(query);
            }

        }

        public DataSet GetData(string id)
        {

            string query = String.Format("SELECT stopObject FROM NodeCache WHERE " +
                                        "networkID='{0}' AND " +
                                        "stopID='{1}';",
                                        networkID,
                                        id);
            DataTable result = database.GetDataSet(query);
            if (result.Rows.Count < 1)
            {
                return null;
            }
            else if (result.Rows.Count > 1)
            {
                throw new InvalidDataException("The there is more than one record for the specified id.");
            }
            else
            {
                DataSet data = new DataSet();
                string xml = result.Rows[0][0].ToString().Replace("\\'", "'");
                data.ReadXml(new StringReader(xml));
                return data;
            }





        }

        public T GetNode(string id, INetworkDataProvider parent)
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
                DataSet data = new DataSet();
                string xml = result.Rows[0][0].ToString().Replace("\\'", "'");
                data.ReadXml(new StringReader(xml));
                T node = (T) Activator.CreateInstance(typeof(T), new object[] { id, parent });           

                return node;
            }

              

            

        }

    }
}
