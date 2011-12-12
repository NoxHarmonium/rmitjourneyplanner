// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="NodeCache.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   The node cache.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Caching
{
    #region

    using System;
    using System.Data;
    using System.IO;
    using System.Security;

    using RmitJourneyPlanner.CoreLibraries.DataAccess;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    #endregion

    /// <summary>
    /// The node cache.
    /// </summary>
    /// <typeparam name="T">
    /// A type that implements the <see cref="INetworkNode"/> interface.
    /// </typeparam>
    internal class NodeCache<T> : IDisposable
        where T : INetworkNode
    {
        #region Constants and Fields

        /// <summary>
        ///   The SQL database that the cache utilizes.
        /// </summary>
        private readonly MySqlDatabase database;

        /// <summary>
        ///   The network identifier that is used to distinguish the nodes.
        /// </summary>
        private readonly string networkId;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCache{T}"/> class.
        /// </summary>
        /// <param name="networkId">
        /// The ID of the calling transport network provider.
        /// </param>
        public NodeCache(string networkId)
        {
            this.database = new MySqlDatabase();
            this.networkId = networkId;
            this.database.Open();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="NodeCache{T}"/> class.
        /// </summary>
        ~NodeCache()
        {
            this.Dispose();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a new object that impliments <see cref="INetworkNode"/> to the cache.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the node.
        /// </param>
        /// <param name="data">
        /// The dataset containing the data related to the node.
        /// </param>
        public void AddCacheEntry(string id, DataSet data)
        {
            using (var writer = new StringWriter())
            {
                data.WriteXml(writer, XmlWriteMode.WriteSchema);
                string query =
                    string.Format(
                        "INSERT INTO NodeCache" + " (networkId,stopID,stopObject)" + " VALUES('{0}','{1}','{2}');", 
                        this.networkId, 
                        id, 
                        SecurityElement.Escape(writer.ToString()));
                this.database.RunQuery(query);
            }
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
        /// Gets the data stored for the specified node identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the node.
        /// </param>
        /// <returns>
        /// A <see cref="DataSet"/> containing the data related to the node.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// Thrown if more than 1 record is retrieved for the given identifier. As the identifier 
        ///   is constrained as unique this would be invalid and would suggest database corruption.
        /// </exception>
        public DataSet GetData(string id)
        {
            string query =
                string.Format(
                    "SELECT stopObject FROM NodeCache WHERE " + "networkId='{0}' AND " + "stopID='{1}';", 
                    this.networkId, 
                    id);
            DataTable result = this.database.GetDataSet(query);
            if (result.Rows.Count < 1)
            {
                return null;
            }

            if (result.Rows.Count > 1)
            {
                throw new InvalidDataException("The there is more than one record for the specified id.");
            }

            var data = new DataSet();
            string xml = result.Rows[0][0].ToString(); // Replace("\\'", "'");
            xml = CacheTools.Deescape(xml);
            data.ReadXml(new StringReader(xml));
            return data;
        }

        /// <summary>
        /// Initializes a new instance of the type <typeparamref name="T"/> and populates it with the 
        ///   stored data.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the node. 
        /// </param>
        /// <param name="parent">
        /// The parent <see cref="INetworkDataProvider"/> of the retrieved node.
        /// </param>
        /// <returns>
        /// A new instance of the type <typeparamref name="T"/> populated with the data stored in the cache.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// Thrown if more than 1 record is retrieved for the given identifier. As the identifier 
        ///   is constrained as unique this would be invalid and would suggest database corruption.
        /// </exception>
        public T GetNode(string id, INetworkDataProvider parent)
        {
            string query =
                string.Format(
                    "SELECT stopObject FROM NodeCache WHERE " + "networkId='{0}' AND " + "stopID='{1}';", 
                    this.networkId, 
                    id);
            DataTable result = this.database.GetDataSet(query);
            if (result.Rows.Count < 1)
            {
                return default(T);
            }

            if (result.Rows.Count > 1)
            {
                throw new InvalidDataException("The there is more than one record for the specified id.");
            }

            var data = new DataSet();
            string xml = CacheTools.Deescape(result.Rows[0][0].ToString());
            data.ReadXml(new StringReader(xml));
            var node = (T)Activator.CreateInstance(typeof(T), new object[] { id, parent });

            return node;
        }

        /// <summary>
        /// Set up the database for the location cache. Clears all data.
        /// </summary>
        public void InitializeCache()
        {
            // Delete any old tables
            // database.RunQuery("DROP TABLE NodeCache;");
            // Create new table
            this.database.RunQuery(Properties.Resources.rmitjourneyplanner_nodecache);
        }

        #endregion
    }
}