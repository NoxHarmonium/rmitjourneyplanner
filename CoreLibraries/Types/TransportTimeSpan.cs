// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransportTimeSpan.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
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
        ///   The travel time component.
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
        ///   The waiting time component.
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
        ///   The +.
        /// </summary>
        /// <param name = "c1">
        ///   The c 1.
        /// </param>
        /// <param name = "c2">
        ///   The c 2.
        /// </param>
        /// <returns>
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
        ///   The ==.
        /// </summary>
        /// <param name = "c1">
        ///   The c 1.
        /// </param>
        /// <param name = "c2">
        ///   The c 2.
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator ==(TransportTimeSpan c1, TransportTimeSpan c2)
        {
            return c1.TotalTime.Equals(c2.TotalTime);
        }

        /// <summary>
        ///   The &gt;.
        /// </summary>
        /// <param name = "c1">
        ///   The c 1.
        /// </param>
        /// <param name = "c2">
        ///   The c 2.
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator >(TransportTimeSpan c1, TransportTimeSpan c2)
        {
            return c1.TotalTime > c2.TotalTime;
        }

        /// <summary>
        ///   The !=.
        /// </summary>
        /// <param name = "c1">
        ///   The c 1.
        /// </param>
        /// <param name = "c2">
        ///   The c 2.
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator !=(TransportTimeSpan c1, TransportTimeSpan c2)
        {
            return c1.TotalTime != c2.TotalTime;
        }

        /// <summary>
        ///   The &lt;.
        /// </summary>
        /// <param name = "c1">
        ///   The c 1.
        /// </param>
        /// <param name = "c2">
        ///   The c 2.
        /// </param>
        /// <returns>
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