// -----------------------------------------------------------------------
// <copyright file="TransportTimeSpan.cs" company="">
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
    /// Represents the 2 components of the time between 2 nodes.
    /// </summary>
    public struct TransportTimeSpan
    {

        public static bool operator <(TransportTimeSpan c1, TransportTimeSpan c2)
        {
            return c1.TotalTime < c2.TotalTime;
        }

        public static bool operator >(TransportTimeSpan c1, TransportTimeSpan c2)
        {
            return c1.TotalTime > c2.TotalTime;
        }

        public static bool operator ==(TransportTimeSpan c1, TransportTimeSpan c2)
        {
            return c1.TotalTime == c2.TotalTime;
        }

        public static bool operator !=(TransportTimeSpan c1, TransportTimeSpan c2)
        {
            return c1.TotalTime != c2.TotalTime;
        }

        /// <summary>
        /// The waiting time component.
        /// </summary>
        public TimeSpan WaitingTime;

        /// <summary>
        /// The travel time component.
        /// </summary>
        public TimeSpan TravelTime;

        /// <summary>
        /// Gets the total time of the traveled arc.
        /// </summary>
        public TimeSpan TotalTime
        {
            get
            {
                return WaitingTime + TravelTime;
            }
        }

        /// <summary>
        /// Gets the string representation of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.TotalTime.ToString();
        }

    }
}
