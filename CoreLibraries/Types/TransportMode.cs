// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    /// <summary>
    ///   Defines the transport mode used.
    /// </summary>
    public enum TransportMode
    {
        /// <summary>
        ///   The transport mode is walking.
        /// </summary>
        Walking = 0,

        /// <summary>
        ///   The transport mode is driving.
        /// </summary>
        Driving = 1,

        /// <summary>
        ///   The transport mode is cycling.
        /// </summary>
        Bicycling = 2,
		
		Train = 3,
		
		Bus = 4,
		
		Tram = 5,
		
		Unknown = 6
    }
}