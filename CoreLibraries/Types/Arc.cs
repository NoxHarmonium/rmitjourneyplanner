// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arc.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   An object representing the time and distance between 2 points
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region Using Directives

    using System;
    using System.Globalization;

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
        private readonly int routeId;

        /// <summary>
        ///   The source.
        /// </summary>
        private readonly Location source;

        /// <summary>
        ///   The time.
        /// </summary>
        private readonly TransportTimeSpan time;

        /// <summary>
        ///   The transport mode.
        /// </summary>
        private readonly string transportMode;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Arc"/> class. Initializes a new arc defining information between 2 points.
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
        /// The departure time of this arc. Set to default(DateTime) if departure time is not relevant. 
        /// </param>
        /// <param name="transportMode">
        /// Sets the transport id used in the arc. 
        /// </param>
        public Arc(
            Location source, 
            Location destination, 
            TransportTimeSpan time, 
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
        /// Initializes a new instance of the <see cref="Arc"/> class. Initializes a new arc defining information between 2 points.
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
        /// The departure time of this arc. Set to default(DateTime) if departure time is not relevant. 
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
            TransportTimeSpan time, 
            double distance, 
            DateTime departureTime, 
            string transportMode, 
            int routeId)
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
        ///   Gets the departure time of this arc. If the departure time is equal to default(DateTime) then departure time is irrelevant.
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
        public int RouteId
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
        ///   Gets the time between the 2 points using the specified transport mode.
        /// </summary>
        public TransportTimeSpan Time
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

        #region Public Methods and Operators

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
                if (otherDistance.Time.TotalTime.Minutes == this.Time.TotalTime.Minutes
                    && otherDistance.Time.TotalTime.Hours == this.Time.TotalTime.Hours
                    && (otherDistance.Distance - this.Distance) < 500)
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this arc.
        /// </summary>
        /// <returns>
        /// A unique (ideally) hash code representing this arc. 
        /// </returns>
        public override int GetHashCode()
        {
            return
                (this.time.TotalTime.TotalSeconds + this.distance.ToString(CultureInfo.InvariantCulture)).GetHashCode();
        }

        #endregion
    }
}