// -----------------------------------------------------------------------
// <copyright file="NetworkArc.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DataProviders;
    using Positioning;

    /// <summary>
    /// Same as a regular Arc but returns nodes rather than locations.
    /// </summary>
    public class NetworkArc : Arc
    {
            /// <summary>
        /// Initializes a new arc defining information between 2 points.
        /// </summary>
        /// <param name="source">The source node of the arc.</param>
        /// <param name="destination">The destination node of the arc.</param>
        /// <param name="time">The total time of the arc.</param>
        /// <param name="distance">The total distance in Km of the arc.</param>
        /// <param name="departureTime">The departure time of this arc. Set to default(DateTime) if departure time is not relivant.</param>
        /// <param name="transportMode">Sets the transport id used in the arc.</param>
        public NetworkArc(INetworkNode source, INetworkNode destination, TimeSpan time, double distance, DateTime departureTime, string transportMode)
            : base((Location)source, (Location)destination, time, distance, departureTime, transportMode)
        {
        }

        /// <summary>
        /// Initializes a new arc defining information between 2 points.
        /// </summary>
        /// <param name="source">The source node of the arc.</param>
        /// <param name="destination">The destination node of the arc.</param>
        /// <param name="time">The total time of the arc.</param>
        /// <param name="distance">The total distance in Km of the arc.</param>
        /// <param name="departureTime">The departure time of this arc. Set to default(DateTime) if departure time is not relivant.</param>
        /// <param name="transportMode">Sets the transport id used in the arc.</param>
        /// <param name="routeId">Sets the optional route ID.</param>
        public NetworkArc(INetworkNode source, INetworkNode destination, TimeSpan time, double distance, DateTime departureTime, string transportMode,string routeId)
            : base((Location)source, (Location)destination, time, distance, departureTime, transportMode,routeId)

        {
        }
        /// <summary>
        /// Gets the source node of the arc.
        /// </summary>
        public new INetworkNode Source
        {
            get
            {
                return (INetworkNode)base.Source;
            }
        }
        
        /// <summary>
        /// Gets the destination node of the arc.
        /// </summary>
        public new INetworkNode Destination
        {
            get
            {
                return (INetworkNode)base.Source;
            }

        }



    }
}
