// -----------------------------------------------------------------------
// <copyright file="ConnectionInfo.cs" company="RMIT University">
// By Sean Dawson
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Net;

    /// <summary>
    /// Contains the information used to connect to the internet.
    /// </summary>
    public static class ConnectionInfo
    {
        private static WebProxy proxy = null;

        /// <summary>
        /// Gets or sets the proxy used by internet connections in this assembly.
        /// </summary>
        public static WebProxy Proxy
        {
            get
            {
                return proxy;
            }
            set
            {
                proxy = value;
            }
        }
    }
}
