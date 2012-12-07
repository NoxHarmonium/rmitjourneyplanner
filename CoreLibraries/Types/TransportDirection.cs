// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransportDirection.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Represents the direction the public transport is going relative to the city.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    /// <summary>
    /// Represents the direction the public transport is going relative to the city.
    /// </summary>
    public enum TransportDirection
    {
        /// <summary>
        ///   The public transport is travelling towards the city
        /// </summary>
        TowardsCity = 0, 

        /// <summary>
        ///   The public transport is travelling away from the city.
        /// </summary>
        FromCity = 1, 

        /// <summary>
        ///   The direction of the transport relative to the city is unknown.
        /// </summary>
        Unknown = 3
    }
}