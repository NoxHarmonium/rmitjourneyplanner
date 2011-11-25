// -----------------------------------------------------------------------
// <copyright file="SQLDatabase.cs" company="RMIT University">
// By Sean Dawson
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data;

    /// <summary>
    /// Represents a database that will accept queries in the SQL format.
    /// </summary>
    public interface ISQLDatabase
    {

               
        /// <summary>
        /// Open the database for access.
        /// </summary>
        void Open();

        /// <summary>
        /// Close the database.
        /// </summary>
        void Close();
        
        /// <summary>
        /// Begin a block transaction. All queries between this call and EndTransaction will be executed at once.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// End the block transaction and commits the queries.
        /// </summary>
        void EndTransaction();

        /// <summary>
        /// Runs a query with no result but returns the number of rows affected.
        /// </summary>
        /// <param name="query"></param>
        int RunQuery(string query);
        
        /// <summary>
        /// Runs a query and returns the results in a datatable.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        DataTable GetDataSet(string query);

    }
}
