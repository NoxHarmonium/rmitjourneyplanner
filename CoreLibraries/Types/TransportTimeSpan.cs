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
            return c1.TotalTime.Equals(c2.TotalTime);
        }

        public static bool operator !=(TransportTimeSpan c1, TransportTimeSpan c2)
        {
            return c1.TotalTime != c2.TotalTime;
        }

        public static TransportTimeSpan operator +(TransportTimeSpan c1, TransportTimeSpan c2)
        {
            var output = new TransportTimeSpan
                { waitingTime = c1.waitingTime + c2.waitingTime, travelTime = c1.travelTime + c2.travelTime };
            return output;
        }

        /// <summary>
        /// The waiting time component.
        /// </summary>
        private TimeSpan waitingTime;

        /// <summary>
        /// The travel time component.
        /// </summary>
        private TimeSpan travelTime;

        /// <summary>
        /// Gets the total time of the traveled arc.
        /// </summary>
        public TimeSpan TotalTime
        {
            get
            {
                return this.waitingTime + this.travelTime;
            }
        }

        /// <summary>
        /// The waiting time component.
        /// </summary>
        public TimeSpan WaitingTime
        {
            get
            {
                return waitingTime;
            }
            set
            {
                waitingTime = value;
            }
        }

        /// <summary>
        /// The travel time component.
        /// </summary>
        public TimeSpan TravelTime
        {
            get
            {
                return travelTime;
            }
            set
            {
                travelTime = value;
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

        /// <summary>
        /// Returns if the 2 TransportTimeSpan objects are equal.
        /// </summary>
        /// <param name="other">An TransportTimeSpan to compare to.</param>
        /// <returns>A boolean value.</returns>
        public bool Equals(TransportTimeSpan other)
        {
            return other.waitingTime.Equals(this.waitingTime) && other.travelTime.Equals(this.travelTime);
        }

        /// <summary>
        /// Returns if the 2 objects are equal.
        /// </summary>
        /// <param name="obj">An object to compare to.</param>
        /// <returns>A boolean value.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (obj.GetType() != typeof(TransportTimeSpan))
            {
                return false;
            }
            return Equals((TransportTimeSpan)obj);
        }

        /// <summary>
        /// Returns a pseudo unique identifier for this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.waitingTime.GetHashCode() * 397) ^ this.travelTime.GetHashCode();
            }
        }
    }
}
