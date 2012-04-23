// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    #region

    using System;
    using System.Data;
    using System.Data.SqlClient;

    #endregion

    /// <summary>
    ///   Allows queries to the Metlink database.
    /// </summary>
    public class MetlinkDatabase : ISqlDatabase, IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   The connection string to connect to the database.
        /// </summary>
        private const string ConnectionString =
            @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=E:\20110606ForDistributionForRMIT.mdb";

        /// <summary>
        ///   The connection to the database.
        /// </summary>
        private readonly SqlConnection connection = new SqlConnection(ConnectionString);

        #endregion

        #region Public Methods

        /// <summary>
        ///   Begin a block transaction. All queries between this call and EndTransaction will be executed at once.
        /// </summary>
        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Close the database.
        /// </summary>
        public void Close()
        {
            this.connection.Close();
        }

        /// <summary>
        ///   End the block transaction and commits the queries.
        /// </summary>
        public void EndTransaction()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Runs a query and returns the results in a datatable.
        /// </summary>
        /// <param name="query"> The SQL query to submit to the database. </param>
        /// <returns> A datatable with the result of the query, or null if there are no results. </returns>
        public DataTable GetDataSet(string query)
        {
            var table = new DataTable();
            var adapter = new SqlDataAdapter(query, this.connection);

            // Open();
            adapter.Fill(table);

            // Close();
            return table;
        }

        /// <summary>
        ///   Open the database for access.
        /// </summary>
        public void Open()
        {
        }

        /// <summary>
        ///   Runs a query with no result but returns the number of rows affected.
        /// </summary>
        /// <param name="query"> The SQL query to submit to the database. </param>
        /// <returns> The number of rows affected by the query. </returns>
        public int RunQuery(string query)
        {
            var command = new SqlCommand(query, this.connection);

            // Open();
            int result = command.ExecuteNonQuery();

            // Close();
            return result;
        }

        #endregion

        ~MetlinkDatabase()
        {
            this.Dispose();
        }




        public void Dispose()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            connection.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}