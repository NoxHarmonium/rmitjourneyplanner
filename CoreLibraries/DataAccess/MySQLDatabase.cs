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
    public class MySQLDatabase : ISQLDatabase
    {

        private string connectionString = "server=localhost;User Id=root;Persist Security Info=True;database=RmitJourneyPlanner";
        private MySqlConnection connection;
        private MySqlTransaction transaction = null;


        public void MySqlDatabase()
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

        public void Open()
        {
            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }

        public void BeginTransaction()
        {
            transaction = connection.BeginTransaction();
        }

        public void EndTransaction()
        {
            transaction.Commit()
            transaction = null;
        }

        public int RunQuery(string query)
        {
            if (transaction == null)
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                Open();
                int result = command.ExecuteNonQuery();
                Close();
                return result;
            }
            else
            {
                MySqlCommand command = new MySqlCommand(query, connection, transaction);
                return command.ExecuteNonQuery();


            }
        }

        public System.Data.DataTable GetDataSet(string query)
        {
            if (transaction == null)
            {
                DataTable table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                Open();
                adapter.Fill(table);
                Close();
                return table;
            }
            else
            {
                throw new NotImplementedException("TODO: Implement transactions with GetDataSet");
            }
        }
    }
}
