// -----------------------------------------------------------------------
// <copyright file="Departure.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a departure from a stop.
    /// </summary>
    public struct Departure
    {
        public int stopId;

        public int routeId;

        public int arrivalTime;

        public int departureTime;
    }
}
