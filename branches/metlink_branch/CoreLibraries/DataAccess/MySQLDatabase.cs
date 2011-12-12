// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="MySQLDatabase.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Represents a MySQL database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    #region

    using System;
    using System.Data;

    using MySql.Data.MySqlClient;

    #endregion

    /// <summary>
    /// Represents a MySQL database.
    /// </summary>
    public class MySqlDatabase : ISqlDatabase
    {
        #region Constants and Fields

        /// <summary>
        ///   The connection.
        /// </summary>
        private MySqlConnection connection;

        /// <summary>
        ///   The connection string.
        /// </summary>
        private string connectionString =
            "server=localhost;User Id=root;password=qwerasdf;Persist Security Info=True;database=RmitJourneyPlanner";

        /// <summary>
        ///   The transaction.
        /// </summary>
        private MySqlTransaction transaction;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MySqlDatabase" /> class.
        /// </summary>
        public MySqlDatabase()
        {
            this.connection = new MySqlConnection(this.connectionString);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the connection string that connects to the MySqlDatabase.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }

            set
            {
                this.connectionString = value;
                if (this.connection.State != ConnectionState.Closed)
                {
                    throw new Exception("You cannot change the connection string while the connection is open.");
                }

                this.connection = new MySqlConnection(this.connectionString);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens up a database transaction. All queries done between the execution of this and
        ///   EndTransaction() will be in the one transaction.
        /// </summary>
        public void BeginTransaction()
        {
            this.connection.Open();
            this.transaction = this.connection.BeginTransaction();
        }

        /// <summary>
        /// Close the database connection.
        /// </summary>
        public void Close()
        {
            this.connection.Close();
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public void EndTransaction()
        {
            this.transaction.Commit();
            this.transaction = null;
        }

        /// <summary>
        /// Runs a query and returns the results in a datatable.
        /// </summary>
        /// <param name="query">
        /// The SQL query to submit to the database.
        /// </param>
        /// <returns>
        /// A datatable with the result of the query, or null if there are no results.
        /// </returns>
        public DataTable GetDataSet(string query)
        {
            if (this.transaction == null)
            {
                var table = new DataTable();
                var adapter = new MySqlDataAdapter(query, this.connection);

                // Open();
                adapter.Fill(table);

                // Close();
                return table;
            }

            throw new NotImplementedException("TODO: Implement transactions with GetDataSet");
        }

        /// <summary>
        /// Open the database connection.
        /// </summary>
        public void Open()
        {
            this.connection.Open();
        }

        /// <summary>
        /// Runs a query with no result but returns the number of rows affected.
        /// </summary>
        /// <param name="query">
        /// The SQL query to submit to the database.
        /// </param>
        /// <returns>
        /// The number of rows affected by the query.
        /// </returns>
        public int RunQuery(string query)
        {
            if (this.transaction == null)
            {
                var command = new MySqlCommand(query, this.connection);

                // Open();
                int result = command.ExecuteNonQuery();

                // Close();
                return result;
            }
            else
            {
                var command = new MySqlCommand(query, this.connection, this.transaction);
                return command.ExecuteNonQuery();
            }
        }

        #endregion
    }
}