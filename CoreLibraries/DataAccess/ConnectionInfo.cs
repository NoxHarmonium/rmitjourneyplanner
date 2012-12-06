// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionInfo.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Contains the information used to connect to the internet.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    #region Using Directives

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
        ////TODO: Merge this into a unified settings class.
        public static WebProxy Proxy { get; set; }

        #endregion
    }
}