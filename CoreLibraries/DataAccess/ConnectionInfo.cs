// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="ConnectionInfo.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Contains the information used to connect to the internet.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    #region

    using System.Net;

    #endregion

    /// <summary>
    /// Contains the information used to connect to the internet.
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