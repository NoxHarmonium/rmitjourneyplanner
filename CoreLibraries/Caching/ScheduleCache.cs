// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="ScheduleCache.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Caches Yarra Trams schedule data.
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
    /// Caches Yarra Trams schedule data.
    /// </summary>
    internal class ScheduleCache : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   The SQL database that the cache utilizes.
        /// </summary>
        private readonly MySqlDatabase database;

        /// <summary>
        ///   The network identifier to distinguish different schedule caches.
        /// </summary>
        private readonly string networkId;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleCache"/> class.
        /// </summary>
        /// <param name="networkId">
        /// The ID of the calling transport network provider.
        /// </param>
        public ScheduleCache(string networkId)
        {
            this.database = new MySqlDatabase();
            this.networkId = networkId;
            this.database.Open();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ScheduleCache"/> class. 
        ///   A method called by the garbage collector to clean up this object.
        /// </summary>
        ~ScheduleCache()
        {
            this.Dispose();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a schedule collection to the cache.
        /// </summary>
        /// <param name="node">
        /// The node the schedule relates to.
        /// </param>
        /// <param name="routeId">
        /// The route identifier that the schedule relates to.
        /// </param>
        /// <param name="lowFloor">
        /// Determines if the schedule includes only low floor trams.
        /// </param>
        /// <param name="requestDate">
        /// The request time the schedule was created for.
        /// </param>
        /// <param name="data">
        /// The <see cref="DataSet"/> containing the schedule data.
        /// </param>
        public void AddScheduleCollection(
            INetworkNode node, string routeId, bool lowFloor, DateTime requestDate, DataSet data)
        {
            // database.BeginTransaction();
            using (var writer = new StringWriter())
            {
                data.WriteXml(writer, XmlWriteMode.WriteSchema);

                string query =
                    string.Format(
                        "INSERT INTO ScheduleCollectionCache" + " (networkId,stopID,routeID,lowFloor,requestDate,data)"
                        + " VALUES('{0}','{1}','{2}','{3}','{4}','{5}');", 
                        this.networkId, 
                        node.Id, 
                        routeId, 
                        Convert.ToInt32(lowFloor), 
                        requestDate.ToString("yyyy-MM-dd HH:mm:ss"), 
                        SecurityElement.Escape(writer.ToString()));
                this.database.RunQuery(query);
            }

            // database.EndTransaction();
        }

        /// <summary>
        /// Inserts a new trip schedule into the cache.
        /// </summary>
        /// <param name="tripId">
        /// The unique identifier of the trip.
        /// </param>
        /// <param name="scheduledDateTime">
        /// The date and time the trip was scheduled.
        /// </param>
        /// <param name="data">
        /// The <see cref="DataSet"/> containing the schedule data.
        /// </param>
        public void AddTripSchedule(string tripId, DateTime scheduledDateTime, DataSet data)
        {
            using (var writer = new StringWriter())
            {
                data.WriteXml(writer, XmlWriteMode.WriteSchema);

                string query =
                    string.Format(
                        "INSERT INTO TripScheduleCache" + " (networkId,tripId,scheduledDateTime,data)"
                        + " VALUES('{0}','{1}','{2}','{3}');", 
                        this.networkId, 
                        tripId, 
                        scheduledDateTime.ToString("yyyy-MM-dd HH:mm:ss"), 
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
        /// Gets the schedule collection DataSet for the supplied parameters.
        /// </summary>
        /// <param name="node">
        /// The node the schedule relates to.
        /// </param>
        /// <param name="routeId">
        /// The route identifier that the schedule relates to.
        /// </param>
        /// <param name="lowFloor">
        /// Determines if the requested schedule is for a low floor tram.
        /// </param>
        /// <param name="requestDate">
        /// The request date and time the schedule was requested for.
        /// </param>
        /// <returns>
        /// A <see cref="DataSet"/> containing the schedule data.
        /// </returns>
        public DataSet GetScheduleCollection(INetworkNode node, string routeId, bool lowFloor, DateTime requestDate)
        {
            string query =
                string.Format(
                    "SELECT data FROM ScheduleCollectionCache" + " WHERE networkId='{0}' AND " + " stopID='{1}' AND "
                    + "routeID='{2}' AND " + "lowFloor='{3}' AND " + "requestDate='{4}';", 
                    this.networkId, 
                    node.Id, 
                    routeId, 
                    Convert.ToInt32(lowFloor), 
                    requestDate.ToString("yyyy-MM-dd HH:mm:ss"));

            DataTable result = this.database.GetDataSet(query);
            if (result.Rows.Count == 0)
            {
                return null;
            }

            if (result.Rows.Count > 1)
            {
                throw new InvalidDataException("The there is more than one record for the specified parameters.");
            }

            string xmlData = result.Rows[0]["data"].ToString();
            xmlData = CacheTools.Deescape(xmlData);
            var reader = new StringReader(xmlData);
            var schedule = new DataSet();
            schedule.ReadXml(reader, XmlReadMode.ReadSchema);
            return schedule;
        }

        /// <summary>
        /// Gets the trip schedule DataSet for the given parameters.
        /// </summary>
        /// <param name="tripId">
        /// The unique trip identifier.
        /// </param>
        /// <param name="scheduledDateTime">
        /// The scheduled date and time of the trip.
        /// </param>
        /// <returns>
        /// A <see cref="DataSet"/> containing the trip data.
        /// </returns>
        public DataSet GetTripSchedule(string tripId, DateTime scheduledDateTime)
        {
            string query =
                string.Format(
                    "SELECT data FROM TripScheduleCache" + " WHERE networkId='{0}' AND " + " tripId='{1}' AND "
                    + "scheduledDateTime='{2}'", 
                    this.networkId, 
                    tripId, 
                    scheduledDateTime.ToString("yyyy-MM-dd HH:mm:ss"));

            DataTable result = this.database.GetDataSet(query);
            if (result.Rows.Count == 0)
            {
                return null;
            }

            if (result.Rows.Count > 1)
            {
                throw new InvalidDataException("The there is more than one record for the specified parameters.");
            }

            string xmlData = result.Rows[0]["data"].ToString();
            xmlData = CacheTools.Deescape(xmlData);
            var reader = new StringReader(xmlData);
            var schedule = new DataSet();
            schedule.ReadXml(reader, XmlReadMode.ReadSchema);
            return schedule;
        }

        /// <summary>
        /// Set up the database for the location cache. Clears all data.
        /// </summary>
        public void InitializeCache()
        {
            // Delete any old tables
            // database.RunQuery("DROP TABLE NodeCache;");
            // Create new table
            this.database.RunQuery(Properties.Resources.rmitjourneyplanner_schedulecollectioncache);
            this.database.RunQuery(Properties.Resources.rmitjourneyplanner_tripschedulecache);
        }

        #endregion
    }
}