// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="Arc.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   An object representing the time and distance between 2 points
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region

    using System;

    using RmitJourneyPlanner.CoreLibraries.Positioning;

    #endregion

    /// <summary>
    /// An object representing the time and distance between 2 points
    /// </summary>
    public class Arc
    {
        #region Constants and Fields

        /// <summary>
        ///   The departure time.
        /// </summary>
        private readonly DateTime departureTime;

        /// <summary>
        ///   The destination.
        /// </summary>
        private readonly Location destination;

        /// <summary>
        ///   The distance.
        /// </summary>
        private readonly double distance;

        /// <summary>
        ///   The route id.
        /// </summary>
        private readonly string routeId;

        /// <summary>
        ///   The source.
        /// </summary>
        private readonly Location source;

        /// <summary>
        ///   The time.
        /// </summary>
        private readonly TimeSpan time;

        /// <summary>
        ///   The transport mode.
        /// </summary>
        private readonly string transportMode;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Arc"/> class. 
        ///   Initializes a new arc defining information between 2 points.
        /// </summary>
        /// <param name="source">
        /// The source location of the arc.
        /// </param>
        /// <param name="destination">
        /// The destination location of the arc.
        /// </param>
        /// <param name="time">
        /// The total time of the arc.
        /// </param>
        /// <param name="distance">
        /// The total distance in Km of the arc.
        /// </param>
        /// <param name="departureTime">
        /// The departure time of this arc. Set to default(DateTime) if departure time is not relivant.
        /// </param>
        /// <param name="transportMode">
        /// Sets the transport id used in the arc.
        /// </param>
        public Arc(
            Location source, 
            Location destination, 
            TimeSpan time, 
            double distance, 
            DateTime departureTime, 
            string transportMode)
        {
            this.source = source;
            this.destination = destination;
            this.time = time;
            this.distance = distance;
            this.transportMode = transportMode;
            this.departureTime = departureTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Arc"/> class. 
        ///   Initializes a new arc defining information between 2 points.
        /// </summary>
        /// <param name="source">
        /// The source location of the arc.
        /// </param>
        /// <param name="destination">
        /// The destination location of the arc.
        /// </param>
        /// <param name="time">
        /// The total time of the arc.
        /// </param>
        /// <param name="distance">
        /// The total distance in Km of the arc.
        /// </param>
        /// <param name="departureTime">
        /// The departure time of this arc. Set to default(DateTime) if departure time is not relivant.
        /// </param>
        /// <param name="transportMode">
        /// Sets the transport id used in the arc.
        /// </param>
        /// <param name="routeId">
        /// Sets the optional route Id.
        /// </param>
        public Arc(
            Location source, 
            Location destination, 
            TimeSpan time, 
            double distance, 
            DateTime departureTime, 
            string transportMode, 
            string routeId)
        {
            this.source = source;
            this.destination = destination;
            this.time = time;
            this.distance = distance;
            this.transportMode = transportMode;
            this.departureTime = departureTime;
            this.routeId = routeId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the departure time of this arc. If the departure time is equal to default(DateTime) then
        ///   departure time is irrelivant.
        /// </summary>
        public DateTime DepartureTime
        {
            get
            {
                return this.departureTime;
            }
        }

        /// <summary>
        ///   Gets the destination of the arc.
        /// </summary>
        public Location Destination
        {
            get
            {
                return this.destination;
            }
        }

        /// <summary>
        ///   Gets the distance between 2 points in meters.
        /// </summary>
        public double Distance
        {
            get
            {
                return this.distance;
            }
        }

        /// <summary>
        ///   Gets the optional route id for this arc.
        /// </summary>
        public string RouteId
        {
            get
            {
                return this.routeId;
            }
        }

        /// <summary>
        ///   Gets the source of the arc.
        /// </summary>
        public Location Source
        {
            get
            {
                return this.source;
            }
        }

        /// <summary>
        ///   Gets the time between the 2 points using the specfied transport mode.
        /// </summary>
        public TimeSpan Time
        {
            get
            {
                return this.time;
            }
        }

        /// <summary>
        ///   Gets the transport mode used to get these distance statistics.
        /// </summary>
        public string TransportMode
        {
            get
            {
                return this.transportMode;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns if 2 distance objects are equal within 500 meters and to the minute.
        /// </summary>
        /// <param name="obj">
        /// The other location to compare this to.
        /// </param>
        /// <returns>
        /// A value indicating if the 2 objects are equal.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() == this.GetType())
            {
                var otherDistance = (Arc)obj;
                if (otherDistance.Time.Minutes == this.Time.Minutes && otherDistance.Time.Hours == this.Time.Hours
                    && (otherDistance.Distance - this.Distance) < 500)
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Serves as a hash funtion for an Arc.
        /// </summary>
        /// <returns>
        /// The get hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}