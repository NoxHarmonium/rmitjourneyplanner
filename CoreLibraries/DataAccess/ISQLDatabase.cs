// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="ISQLDatabase.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Represents a database that will accept queries in the SQL format.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    #region

    using System.Data;

    #endregion

    /// <summary>
    /// Represents a database that will accept queries in the SQL format.
    /// </summary>
    public interface ISqlDatabase
    {
        #region Public Methods

        /// <summary>
        /// Begin a block transaction. All queries between this call and EndTransaction will be executed at once.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Close the database.
        /// </summary>
        void Close();

        /// <summary>
        /// End the block transaction and commits the queries.
        /// </summary>
        void EndTransaction();

        /// <summary>
        /// Runs a query and returns the results in a datatable.
        /// </summary>
        /// <param name="query">
        /// The SQL query to submit to the database.
        /// </param>
        /// <returns>
        /// A datatable with the result of the query, or null if there are no results.
        /// </returns>
        DataTable GetDataSet(string query);

        /// <summary>
        /// Open the database for access.
        /// </summary>
        void Open();

        /// <summary>
        /// Runs a query with no result but returns the number of rows affected.
        /// </summary>
        /// <param name="query">
        /// The SQL query to submit to the database.
        /// </param>
        /// <returns>
        /// The number of rows affected by the query.
        /// </returns>
        int RunQuery(string query);

        #endregion
    }
}