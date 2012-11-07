// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JourneyLeg.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Represents one leg of a PT journey.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region Using Directives

    using System;

    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;

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
        private readonly string RouteId;

        /// <summary>
        ///   The departure time of the leg.
        /// </summary>
        private readonly DateTime departureTime;

        /// <summary>
        ///   The node at the end of the leg.
        /// </summary>
        private readonly MetlinkNode destination;

        /// <summary>
        ///   The node at the beginning of the leg.
        /// </summary>
        private readonly MetlinkNode origin;

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
            MetlinkNode origin, 
            MetlinkNode destination, 
            DateTime departureTime, 
            TimeSpan totalTime, 
            string routeId)
        {
            this.transportMode = transportMode;
            this.origin = origin;
            this.destination = destination;
            this.departureTime = departureTime;
            this.totalTime = totalTime;
            this.RouteId = routeId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   The departure time of the leg.
        /// </summary>
        public DateTime DepartureTime
        {
            get
            {
                return this.departureTime;
            }
        }

        /// <summary>
        ///   The node at the end of the leg.
        /// </summary>
        public MetlinkNode Destination
        {
            get
            {
                return this.destination;
            }
        }

        /// <summary>
        ///   The node at the beginning of the leg.
        /// </summary>
        public MetlinkNode Origin
        {
            get
            {
                return this.origin;
            }
        }

        /// <summary>
        ///   The route ID of the leg.
        /// </summary>
        public string RouteId1
        {
            get
            {
                return this.RouteId;
            }
        }

        /// <summary>
        ///   The total time of the leg.
        /// </summary>
        public TimeSpan TotalTime
        {
            get
            {
                return this.totalTime;
            }
        }

        /// <summary>
        ///   The transport mode of the leg.
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