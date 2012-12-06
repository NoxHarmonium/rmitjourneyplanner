// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MySQLDatabase.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Represents a MySQL database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    #region Using Directives

    using System;
    using System.Data;

    using MySql.Data.MySqlClient;

    #endregion

    /// <summary>
    /// Represents a MySQL database.
    /// </summary>
    public class MySqlDatabase : ISqlDatabase, IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   Provides locking functionality to prevent multiple simultaneous queries from different threads. 
        ///   This was added as it was suspected to be causing issues.
        /// </summary>
        ////TODO: Investigate multithreaded database access and whether it is actually an issue.
        private readonly object databaseLock = new object();

        /// <summary>
        ///   The connection.
        /// </summary>
        private MySqlConnection connection;

        /// <summary>
        ///   The connection string.
        /// </summary>
        private string connectionString = Settings.Entries["ConnectionString"];

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
            this.connectionString = Settings.Entries["ConnectionString"];
            this.connection = new MySqlConnection(this.connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDatabase"/> class with a specified database name.
        /// </summary>
        /// <param name="database">
        /// The database. 
        /// </param>
        [Obsolete(
            "You can no longer initialise this object by passing a database name. You must define the connection string property in the Settings class. This constructor ignores the database parameter.")]
        public MySqlDatabase(string database)
        {
            this.connectionString = Settings.Entries["ConnectionString"];
            this.connection = new MySqlConnection(this.connectionString);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MySqlDatabase"/> class. Finalizes this database object and makes sure that the connection is closed.
        /// </summary>
        ~MySqlDatabase()
        {
            this.Dispose(false);
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

        #region Public Methods and Operators

        /// <summary>
        /// Opens up a database transaction. All queries done between the execution of this and EndTransaction() will be in the one transaction.
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
        /// Closes all connections and disposes of this object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Closes all connections and disposes of this object.
        /// </summary>
        /// <param name="disposing">
        /// True if called from finalizer, false if called by programmer.
        /// </param>
        public void Dispose(bool disposing)
        {
            if (this.connection.State == ConnectionState.Open)
            {
                this.connection.Close();
            }

            this.connection.Dispose();
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
            if (this.transaction != null)
            {
                ////TODO: Implement transactions with GetDataSet
                throw new NotImplementedException("Transactions are not implimented with GetDataSet at this time.");
            }

            lock (this.databaseLock)
            {
                var table = new DataTable();
                var adapter = new MySqlDataAdapter(query, this.connection);

                // Open();
                adapter.Fill(table);

                // Close();
                return table;
            }
        }

        /// <summary>
        /// Uses a fast <see cref="MySqlDataReader"/> object to put a single query result an array of objects representing each column.
        /// </summary>
        /// <param name="query">
        /// The SQL query to submit to the database. 
        /// </param>
        /// <returns>
        /// An array of objects representing the result.
        /// </returns>
        public object[] GetFastData(string query)
        {
            if (this.transaction != null)
            {
                ////TODO: Implement transactions with GetFastData
                throw new NotImplementedException("Transactions are not implimented with GetFastData at this time.");
            }

            lock (this.databaseLock)
            {
                var command = new MySqlCommand(query, this.connection);
                var reader = command.ExecuteReader(CommandBehavior.SingleResult);
                var o = new object[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    reader.Read();
                    o[i] = reader.GetValue(i);
                }

                reader.Close();
                return o;
            }
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
            lock (this.databaseLock)
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
        }

        #endregion
    }
}