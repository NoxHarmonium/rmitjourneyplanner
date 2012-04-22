// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    /// <summary>
    ///   Represents the request type for the XMLRequestor class.
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