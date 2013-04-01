// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JourneyLeg.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Represents one leg of a PT journey.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region Using Directives

    using System;

    using RmitJourneyPlanner.CoreLibraries.DataProviders.Ptv;

    #endregion

    /// <summary>
    /// Represents one leg of a PT journey.
    /// </summary>
    public class JourneyLeg
    {
        #region Constants and Fields

        /// <summary>
        ///   The route ID of the leg.
        /// </summary>
        private readonly string routeId;

        /// <summary>
        ///   The departure time of the leg.
        /// </summary>
        private readonly DateTime departureTime;

        /// <summary>
        ///   The node at the end of the leg.
        /// </summary>
        private readonly PtvNode destination;

        /// <summary>
        ///   The node at the beginning of the leg.
        /// </summary>
        private readonly PtvNode origin;

        /// <summary>
        ///   The total time of the leg.
        /// </summary>
        private readonly TimeSpan totalTime;

        /// <summary>
        ///   The transport mode of the leg.
        /// </summary>
        private readonly TransportMode transportMode;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JourneyLeg"/> class.
        /// </summary>
        /// <param name="transportMode">
        /// The transport Mode.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="departureTime">
        /// The departure Time.
        /// </param>
        /// <param name="totalTime">
        /// The total Time.
        /// </param>
        /// <param name="routeId">
        /// The route Id.
        /// </param>
        public JourneyLeg(
            TransportMode transportMode, 
            PtvNode origin, 
            PtvNode destination, 
            DateTime departureTime, 
            TimeSpan totalTime, 
            string routeId)
        {
            this.transportMode = transportMode;
            this.origin = origin;
            this.destination = destination;
            this.departureTime = departureTime;
            this.totalTime = totalTime;
            this.routeId = routeId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the departure time of the leg.
        /// </summary>
        public DateTime DepartureTime
        {
            get
            {
                return this.departureTime;
            }
        }

        /// <summary>
        ///   Gets the node at the end of the leg.
        /// </summary>
        public PtvNode Destination
        {
            get
            {
                return this.destination;
            }
        }

        /// <summary>
        ///   Gets the node at the beginning of the leg.
        /// </summary>
        public PtvNode Origin
        {
            get
            {
                return this.origin;
            }
        }

        /// <summary>
        ///   Gets the route ID of the leg.
        /// </summary>
        public string RouteId1
        {
            get
            {
                return this.routeId;
            }
        }

        /// <summary>
        ///   Gets the total time of the leg.
        /// </summary>
        public TimeSpan TotalTime
        {
            get
            {
                return this.totalTime;
            }
        }

        /// <summary>
        ///   Gets the transport mode of the leg.
        /// </summary>
        public TransportMode TransportMode
        {
            get
            {
                return this.transportMode;
            }
        }

        #endregion
    }
}