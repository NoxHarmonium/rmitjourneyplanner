// -----------------------------------------------------------------------
// <copyright file="RequestType.cs" company="RMIT University">
// RMIT Travel Planner
// By Sean Dawson
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents the request type for the XMLRequestor class.
    /// </summary>
    enum RequestType
    {
        GET = 0,
        SOAP = 1
    }
}
