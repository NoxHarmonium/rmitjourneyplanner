// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="TransportMode.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Defines the transport mode used.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    /// <summary>
    /// Defines the transport mode used.
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
        Bicycling = 2
    }
}