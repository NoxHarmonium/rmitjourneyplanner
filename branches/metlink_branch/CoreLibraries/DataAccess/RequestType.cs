// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="RequestType.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Represents the request type for the XMLRequestor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    /// <summary>
    /// Represents the request type for the XMLRequestor class.
    /// </summary>
    internal enum RequestType
    {
        /// <summary>
        ///   Use a simple GET html request for the XML.
        /// </summary>
        /// <example>
        ///   Google Maps API.
        /// </example>
        Get = 0, 

        /// <summary>
        ///   Use a SOAP request for the XML.
        /// </summary>
        /// <example>
        ///   Tram Tracker API.
        /// </example>
        Soap = 1
    }
}