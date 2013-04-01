// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransportMode.cs" company="RMIT University">
//   Copyright RMIT University 2012.
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
        Bicycling = 2, 

        /// <summary>
        ///   The train.
        /// </summary>
        Train = 3, 

        /// <summary>
        ///   The bus.
        /// </summary>
        Bus = 4, 

        /// <summary>
        ///   The tram.
        /// </summary>
        Tram = 5, 

        /// <summary>
        ///   The unknown.
        /// </summary>
        Unknown = 6
    }
}