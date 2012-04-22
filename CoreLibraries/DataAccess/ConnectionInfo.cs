// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    #region

    using System.Net;

    #endregion

    /// <summary>
    ///   Contains the information used to connect to the internet.
    /// </summary>
    public static class ConnectionInfo
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the proxy used by internet connections in this assembly.
        /// </summary>
        public static WebProxy Proxy { get; set; }

        #endregion
    }
}