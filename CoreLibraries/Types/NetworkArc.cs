// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkArc.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Same as a regular Arc but returns nodes rather than locations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region Using Directives

    using System;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Positioning;

    #endregion

    /// <summary>
    /// Same as a regular Arc but returns nodes rather than locations.
    /// </summary>
    public class NetworkArc : Arc
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkArc"/> class. Initializes a new arc defining information between 2 points.
        /// </summary>
        /// <param name="source">
        /// The source node of the arc. 
        /// </param>
        /// <param name="destination">
        /// The destination node of the arc. 
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
        public NetworkArc(
            INetworkNode source, 
            INetworkNode destination, 
            TransportTimeSpan time, 
            double distance, 
            DateTime departureTime, 
            string transportMode)
            : base((Location)source, (Location)destination, time, distance, departureTime, transportMode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkArc"/> class. Initializes a new arc defining information between 2 points.
        /// </summary>
        /// <param name="source">
        /// The source node of the arc. 
        /// </param>
        /// <param name="destination">
        /// The destination node of the arc. 
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
        public NetworkArc(
            INetworkNode source, 
            INetworkNode destination, 
            TransportTimeSpan time, 
            double distance, 
            DateTime departureTime, 
            string transportMode, 
            int routeId)
            : base((Location)source, (Location)destination, time, distance, departureTime, transportMode, routeId)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the destination node of the arc.
        /// </summary>
        public new INetworkNode Destination
        {
            get
            {
                return (INetworkNode)base.Source;
            }
        }

        /// <summary>
        ///   Gets the source node of the arc.
        /// </summary>
        public new INetworkNode Source
        {
            get
            {
                return (INetworkNode)base.Source;
            }
        }

        #endregion
    }
}