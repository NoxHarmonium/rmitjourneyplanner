// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Departure.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Represents a departure from a stop in the timetable.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    /// <summary>
    /// Represents a departure from a stop in the timetable.
    /// </summary>
    public struct Departure
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the arrival time.
        /// </summary>
        public int ArrivalTime { get; set; }

        /// <summary>
        ///   Gets or sets the departure time.
        /// </summary>
        public int DepartureTime { get; set; }

        /// <summary>
        ///   Gets or sets the order parameter.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        ///   Gets or sets the route identifier.
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        ///   Gets or sets the service identifier.
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        ///   Gets or sets the stop identifier.
        /// </summary>
        public int StopId { get; set; }

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
                    this.StopId, 
                    this.RouteId, 
                    this.ArrivalTime, 
                    this.DepartureTime, 
                    this.ServiceId, 
                    this.Order);
        }

        #endregion
    }
}