// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Departure.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Represents a departure from a stop.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    /// <summary>
    /// Represents a departure from a stop.
    /// </summary>
    public struct Departure
    {
        #region Constants and Fields

        /// <summary>
        ///   The arrival time.
        /// </summary>
        public int arrivalTime;

        /// <summary>
        ///   The departure time.
        /// </summary>
        public int departureTime;

        /// <summary>
        ///   The order.
        /// </summary>
        public int order;

        /// <summary>
        ///   The route id.
        /// </summary>
        public int routeId;

        /// <summary>
        ///   The service id.
        /// </summary>
        public int serviceId;

        /// <summary>
        ///   The stop id.
        /// </summary>
        public int stopId;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return
                string.Format(
                    "StopId: {0}, RouteId: {1}, ArrivalTime: {2}, DepartureTime: {3}, ServiceId: {4}, Order: {5}", 
                    this.stopId, 
                    this.routeId, 
                    this.arrivalTime, 
                    this.departureTime, 
                    this.serviceId, 
                    this.order);
        }

        #endregion
    }
}