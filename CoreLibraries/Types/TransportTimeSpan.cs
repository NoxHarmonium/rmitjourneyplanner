// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransportTimeSpan.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Represents the 2 components of the time between 2 nodes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region Using Directives

    using System;

    #endregion

    /// <summary>
    /// Represents the 2 components of the time between 2 nodes.
    /// </summary>
    public struct TransportTimeSpan
    {
        #region Constants and Fields

        /// <summary>
        ///   The travel time component.
        /// </summary>
        private TimeSpan travelTime;

        /// <summary>
        ///   The waiting time component.
        /// </summary>
        private TimeSpan waitingTime;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the total time of the traveled arc.
        /// </summary>
        public TimeSpan TotalTime
        {
            get
            {
                return this.waitingTime + this.travelTime;
            }
        }

        /// <summary>
        ///   Gets or sets the travel time component.
        /// </summary>
        public TimeSpan TravelTime
        {
            get
            {
                return this.travelTime;
            }

            set
            {
                this.travelTime = value;
            }
        }

        /// <summary>
        ///   Gets or sets the waiting time component.
        /// </summary>
        public TimeSpan WaitingTime
        {
            get
            {
                return this.waitingTime;
            }

            set
            {
                this.waitingTime = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   The addition operator of the <see cref="TransportTimeSpan"/> class.
        /// </summary>
        /// <param name = "c1">
        ///   The first object to be added.
        /// </param>
        /// <param name = "c2">
        ///   The second object to be added.
        /// </param>
        /// <returns>
        /// The result of the addition.
        /// </returns>
        public static TransportTimeSpan operator +(TransportTimeSpan c1, TransportTimeSpan c2)
        {
            var output = new TransportTimeSpan
                {
                   waitingTime = c1.waitingTime + c2.waitingTime, travelTime = c1.travelTime + c2.travelTime 
                };
            return output;
        }

        /// <summary>
        ///   The equality operator for a <see cref="TransportTimeSpan"/> object.
        /// </summary>
        /// <param name = "c1">
        ///   The object on the left side of the statement.
        /// </param>
        /// <param name = "c2">
        ///  The object on the right side of the statement.
        /// </param>
        /// <returns>
        /// True if <paramref name="c1"/> equals <paramref name="c2"/>, otherwise false.
        /// </returns>
        public static bool operator ==(TransportTimeSpan c1, TransportTimeSpan c2)
        {
            return c1.TotalTime.Equals(c2.TotalTime);
        }

        /// <summary>
        ///   The greater than operator for a <see cref="TransportTimeSpan"/> object.
        /// </summary>
        /// <param name = "c1">
        ///   The object on the left side of the statement.
        /// </param>
        /// <param name = "c2">
        ///  The object on the right side of the statement.
        /// </param>
        /// <returns>
        /// True if <paramref name="c1"/> is greater than <paramref name="c2"/>, otherwise false.
        /// </returns>
        public static bool operator >(TransportTimeSpan c1, TransportTimeSpan c2)
        {
            return c1.TotalTime > c2.TotalTime;
        }

        /// <summary>
        ///   The equality operator for a <see cref="TransportTimeSpan"/> object.
        /// </summary>
        /// <param name = "c1">
        ///   The object on the left side of the statement.
        /// </param>
        /// <param name = "c2">
        ///  The object on the right side of the statement.
        /// </param>
        /// <returns>
        /// True if <paramref name="c1"/> does not equal <paramref name="c2"/>, otherwise false.
        /// </returns>
        public static bool operator !=(TransportTimeSpan c1, TransportTimeSpan c2)
        {
            return c1.TotalTime != c2.TotalTime;
        }

        /// <summary>
        ///   The less than operator for a <see cref="TransportTimeSpan"/> object.
        /// </summary>
        /// <param name = "c1">
        ///   The object on the left side of the statement.
        /// </param>
        /// <param name = "c2">
        ///  The object on the right side of the statement.
        /// </param>
        /// <returns>
        /// True if <paramref name="c1"/> is less than <paramref name="c2"/>, otherwise false.
        /// </returns>
        public static bool operator <(TransportTimeSpan c1, TransportTimeSpan c2)
        {
            return c1.TotalTime < c2.TotalTime;
        }

        /// <summary>
        /// Returns if the 2 TransportTimeSpan objects are equal.
        /// </summary>
        /// <param name="other">
        /// An TransportTimeSpan to compare to.
        /// </param>
        /// <returns>
        /// A boolean value.
        /// </returns>
        public bool Equals(TransportTimeSpan other)
        {
            return other.waitingTime.Equals(this.waitingTime) && other.travelTime.Equals(this.travelTime);
        }

        /// <summary>
        /// Returns if the 2 objects are equal.
        /// </summary>
        /// <param name="obj">
        /// An object to compare to.
        /// </param>
        /// <returns>
        /// A boolean value.
        /// </returns>
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

            return this.Equals((TransportTimeSpan)obj);
        }

        /// <summary>
        /// Returns a pseudo unique identifier for this object.
        /// </summary>
        /// <returns>
        /// The get hash code.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.waitingTime.GetHashCode() * 397) ^ this.travelTime.GetHashCode();
            }
        }

        /// <summary>
        /// Gets the string representation of this object.
        /// </summary>
        /// <returns>
        /// The to string.
        /// </returns>
        public override string ToString()
        {
            return this.TotalTime.ToString();
        }

        #endregion
    }
}