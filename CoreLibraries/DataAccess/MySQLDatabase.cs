// -----------------------------------------------------------------------
// <copyright file="MySQLDatabase.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data;
    using System.Data.Sql;
    using MySql.Data.MySqlClient;
    /// <summary>
    /// Represents a MySQL database.
    /// </summary>
    public class MySqlDatabase : ISQLDatabase
    {

        private string connectionString = "server=localhost;User Id=root;password=qwerasdf;Persist Security Info=True;database=RmitJourneyPlanner";
        private MySqlConnection connection;
        private MySqlTransaction transaction = null;

        /// <summary>
        /// Initilizes a new MySqlDatabase
        /// </summary>
        public MySqlDatabase()
        {
            connection = new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Gets or sets the connection string that connects to the MySqlDatabase.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
                if (connection.State != ConnectionState.Closed)
                {
                    throw new Exception("You cannot change the connection string while the connection is open.");
                }
                connection = new MySqlConnection(connectionString);
            }
        }

        /// <summary>
        /// Open the database connection.
        /// </summary>
        public void Open()
        {
            connection.Open();
        }

        /// <summary>
        /// Close the database connection.
        /// </summary>
        public void Close()
        {
            connection.Close();
        }

        /// <summary>
        /// Opens up a database transaction. All queries done between the execution of this and
        /// EndTransaction() will be in the one transaction.
        /// </summary>
        public void BeginTransaction()
        {
            connection.Open();
            transaction = connection.BeginTransaction();
        }


        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public void EndTransaction()
        {
            transaction.Commit();
            transaction = null;
        }

        /// <summary>
        /// Runs a simple query with no return.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int RunQuery(string query)
        {
            if (transaction == null)
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                //Open();
                int result = command.ExecuteNonQuery();
                //Close();
                return result;
            }
            else
            {
                MySqlCommand command = new MySqlCommand(query, connection, transaction);
                return command.ExecuteNonQuery();


            }
        }

        /// <summary>
        /// Runs a query and returns the result in a datatable.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public System.Data.DataTable GetDataSet(string query)
        {
            if (transaction == null)
            {
                DataTable table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                //Open();
                adapter.Fill(table);
                //Close();
                return table;
            }
            else
            {
                throw new NotImplementedException("TODO: Implement transactions with GetDataSet");
            }
        }
    }
}
