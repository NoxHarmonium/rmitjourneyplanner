// -----------------------------------------------------------------------
// <copyright file="Departure.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    /// <summary>
    /// Represents a departure from a stop.
    /// </summary>
    public struct Departure
    {
        public int stopId;

        public int routeId;

        public int arrivalTime;

        public int departureTime;

        public int order;

        public bool wrapped;

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("StopId: {0}, RouteId: {1}, ArrivalTime: {2}, DepartureTime: {3}, Order: {4}, Wrapped: {5}", this.stopId, this.routeId, this.arrivalTime, this.departureTime, this.order,this.wrapped);
        }
    }
}
