// -----------------------------------------------------------------------
// <copyright file="JourneyLeg.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;

    /// <summary>
    /// Represents one leg of a PT journey.
    /// </summary>
    public class JourneyLeg
    {
        /// <summary>
        /// The transport mode of the leg.
        /// </summary>
        private string transportMode;

        /// <summary>
        /// The node at the beginning of the leg.
        /// </summary>
        private MetlinkNode origin;

        /// <summary>
        /// The node at the end of the leg.
        /// </summary>
        private MetlinkNode destination;

        /// <summary>
        /// The departure time of the leg.
        /// </summary>
        private DateTime departureTime;

        /// <summary>
        /// The total time of the leg.
        /// </summary>
        private TimeSpan totalTime;

        /// <summary>
        /// The route ID of the leg.
        /// </summary>
        private string RouteId;

        /// <summary>
        /// Initializes a new instance of the <see cref="JourneyLeg"/> class.
        /// </summary>
        public JourneyLeg(string transportMode, MetlinkNode origin, MetlinkNode destination, DateTime departureTime, TimeSpan totalTime, string routeId)
        {
            this.transportMode = transportMode;
            this.origin = origin;
            this.destination = destination;
            this.departureTime = departureTime;
            this.totalTime = totalTime;
            this.RouteId = routeId;
        }

        /// <summary>
        /// The route ID of the leg.
        /// </summary>
        public string RouteId1
        {
            get
            {
                return this.RouteId;
            }
        }

        /// <summary>
        /// The transport mode of the leg.
        /// </summary>
        public string TransportMode
        {
            get
            {
                return this.transportMode;
            }
        }

        /// <summary>
        /// The node at the beginning of the leg.
        /// </summary>
        public MetlinkNode Origin
        {
            get
            {
                return this.origin;
            }
        }

        /// <summary>
        /// The node at the end of the leg.
        /// </summary>
        public MetlinkNode Destination
        {
            get
            {
                return this.destination;
            }
        }

  

        /// <summary>
        /// The departure time of the leg.
        /// </summary>
        public DateTime DepartureTime
        {
            get
            {
                return this.departureTime;
            }
        }

        /// <summary>
        /// The total time of the leg.
        /// </summary>
        public TimeSpan TotalTime
        {
            get
            {
                return this.totalTime;
            }
        }
    }
}
