// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    /// <summary>
    ///   Represents the direction the public transport is going relative to the city.
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
        ///   The direction of the transport relitive to the city is unknown.
        /// </summary>
        Unknown = 3
    }
}