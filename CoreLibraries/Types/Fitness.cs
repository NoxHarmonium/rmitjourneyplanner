// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Fitness.cs" company="RMIT University">
//   Sean Dawson
// </copyright>
// <summary>
//   A list of the parameters provided by the fitness class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A list of the parameters provided by the fitness class.
    /// </summary>
    public enum FitnessParameter
    {

        /// <summary>
        /// Gets the non-normalised journey time in fractional minutes.
        /// </summary>
        TotalJourneyMinutes,

        /// <summary>
        /// Gets the normalised changes value.
        /// </summary>
        NormalisedChanges,

        /// <summary>
        /// The total journey time.
        /// </summary>
        TotalJourneyTime,

        /// <summary>
        /// The non normalise changes.
        /// </summary>
        Changes,

        /// <summary>
        /// The walking time.
        /// </summary>
        WalkingTime,

        /// <summary>
        /// The percent trams.
        /// </summary>
        PercentTrams,

        /// <summary>
        /// The percent buses.
        /// </summary>
        PercentBuses,

        /// <summary>
        /// The percent trains.
        /// </summary>
        PercentTrains,

        /// <summary>
        /// The co 2 emmissions.
        /// </summary>
        Co2Emmissions,

        /// <summary>
        /// The percent disable friendly.
        /// </summary>
        PercentDisableFriendly,

        /// <summary>
        /// The total travel time.
        /// </summary>
        TotalTravelTime,

        /// <summary>
        /// The total waiting time.
        /// </summary>
        TotalWaitingTime
    }

    /// <summary>
    /// Represents the different aspects of a journey in order to evaluate fitness.
    /// </summary>
    public class Fitness : ICloneable, IEquatable<Fitness>, IEqualityComparer<Fitness>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Fitness"/> class. 
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Fitness()
        {
            this.JourneyLegs = new List<JourneyLeg>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the amount of fitness parameters available.
        /// </summary>
        public static int ParameterCount
        {
            get
            {
                return Enum.GetValues(typeof(FitnessParameter)).Length;
            }
        }

        /// <summary>
        /// The amount of mode changes of the journey.
        /// </summary>
        public int Changes { get; set; }

        /// <summary>
        /// The amount of CO2 emmitted in this journey.
        /// </summary>
        public double Co2Emmissions { get; set; }

        /// <summary>
        /// The list of legs that make up a journey.
        /// </summary>
        public List<JourneyLeg> JourneyLegs { get; set; }

        /// <summary>
        /// Returns the amount of fitness parameters available.
        /// </summary>
        public int Length
        {
            get
            {
                return Enum.GetValues(typeof(FitnessParameter)).Length;
            }
        }

        /// <summary>
        /// Gets or sets NormalisedChanges.
        /// </summary>
        public double NormalisedChanges { get; set; }

        /// <summary>
        /// The normalised journey time of the leg (0.0-1.0).
        /// </summary>
        public double NormalisedJourneyTime { get; set; }

        /// <summary>
        /// The normalised travel time of the leg (0.0-1.0).
        /// </summary>
        public double NormalisedTravelTime { get; set; }

        /// <summary>
        /// The percent of the journey that uses buses. (0.0 - 1.0)
        /// </summary>
        public double PercentBuses { get; set; }

        /// <summary>
        /// The percent of the journey that is accessable for disabled passengers. (0.0 - 1.0)
        /// </summary>
        public double PercentDisableFriendly { get; set; }

        /// <summary>
        /// The percent of the journey that uses trains. (0.0 - 1.0)
        /// </summary>
        public double PercentTrains { get; set; }

        /// <summary>
        /// The percent of the journey that uses trams. (0.0 - 1.0)
        /// </summary>
        public double PercentTrams { get; set; }

        /// <summary>
        /// The total time of the whole journey.
        /// </summary>
        public TimeSpan TotalJourneyTime { get; set; }

        /// <summary>
        /// The total time spent on a service in the journey.
        /// </summary>
        public TimeSpan TotalTravelTime { get; set; }

        /// <summary>
        /// The total time spent waiting for a service in the journey.
        /// </summary>
        public TimeSpan TotalWaitingTime { get; set; }

        /// <summary>
        /// Gets or sets the total time walking in a journey.
        /// </summary>
        public TimeSpan WalkingTime { get; set; }

        #endregion

        #region Public Indexers

        /// <summary>
        /// References the different fitness parameters with an index. The parameters are all returned so that they suit a minimization problem. (Lower is better).
        /// </summary>
        /// <param name="i">The index of the parameter. <see cref="FitnessParameter"/></param>
        public double this[FitnessParameter fitnessParameter]
        {
            get
            {

                return this[(int)fitnessParameter];
            }

        }

        /// <summary>
        /// References the different fitness parameters with an index. The parameters are all returned so that they suit a minimization problem. (Lower is better).
        /// </summary>
        /// <param name="i">The index of the parameter. <see cref="FitnessParameter"/></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if a index is refereced that is out of range.</exception>
        public double this[int i]
        {
            get
            {
                switch ((FitnessParameter)i)
                {
                    case FitnessParameter.TotalJourneyTime:
                        if (this.TotalJourneyTime.TotalSeconds <= 0)
                        {
                            return double.MaxValue;
                        }

                        return this.NormalisedJourneyTime;

                    case FitnessParameter.NormalisedChanges:
                        return this.NormalisedChanges;

                    case FitnessParameter.Changes:
                        return this.Changes;

                    case FitnessParameter.TotalJourneyMinutes:
                        return this.TotalJourneyTime.TotalMinutes;

                    case FitnessParameter.WalkingTime:
                        return this.WalkingTime.TotalSeconds;

                    case FitnessParameter.Co2Emmissions:
                        return this.Co2Emmissions;
                    case FitnessParameter.PercentBuses:
                        return 1 - this.PercentBuses;

                    case FitnessParameter.PercentDisableFriendly:
                        return 1 - this.PercentDisableFriendly;

                    case FitnessParameter.PercentTrains:
                        return 1 - this.PercentTrains;

                    case FitnessParameter.PercentTrams:
                        return 1 - this.PercentTrams;

                    case FitnessParameter.TotalTravelTime:
                        return NormalisedTravelTime;

                    case FitnessParameter.TotalWaitingTime:
                        return this.TotalWaitingTime.TotalSeconds;
                    default:
                        throw new ArgumentOutOfRangeException("i");
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds 2 fitness objects together
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static Fitness operator +(Fitness c1, Fitness c2)
        {
            var fitness = new Fitness
            {
                Changes = c1.Changes + c2.Changes,
                Co2Emmissions = c1.Co2Emmissions + c2.Co2Emmissions,
                PercentBuses = c1.PercentBuses + c2.PercentBuses,
                PercentDisableFriendly = c1.PercentDisableFriendly + c2.PercentDisableFriendly,
                PercentTrains = c1.PercentTrains + c2.PercentTrains,
                PercentTrams = c1.PercentTrams + c2.PercentTrams,
                TotalJourneyTime = c1.TotalJourneyTime + c2.TotalJourneyTime,
                TotalTravelTime = c1.TotalTravelTime + c2.TotalTravelTime,
                TotalWaitingTime = c1.TotalWaitingTime + c2.TotalWaitingTime
            };
            return fitness;
        }

        /// <summary>
        /// Divides all the internal values by the specified double.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static Fitness operator /(Fitness c1, double c2)
        {
            var fitness = new Fitness
            {
                Changes = (int)(c1.Changes / c2),
                Co2Emmissions = c1.Co2Emmissions / c2,
                PercentBuses = c1.PercentBuses / c2,
                PercentDisableFriendly = c1.PercentDisableFriendly / c2,
                PercentTrains = c1.PercentTrains / c2,
                PercentTrams = c1.PercentTrams / c2,
                TotalJourneyTime = TimeSpan.FromMilliseconds(c1.TotalJourneyTime.TotalMilliseconds / c2),
                TotalTravelTime = TimeSpan.FromMilliseconds(c1.TotalTravelTime.TotalMilliseconds / c2),
                TotalWaitingTime = TimeSpan.FromMilliseconds(c1.TotalWaitingTime.TotalMilliseconds / c2)
            };
            return fitness;
        }

        /// <summary>
        /// The ==.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator ==(Fitness left, Fitness right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// The !=.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator !=(Fitness left, Fitness right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public object Clone()
        {
            return new Fitness
            {
                Changes = this.Changes,
                Co2Emmissions = this.Co2Emmissions,
                NormalisedChanges = this.NormalisedChanges,
                NormalisedJourneyTime = this.NormalisedJourneyTime,
                PercentBuses = this.PercentBuses,
                PercentDisableFriendly = this.PercentDisableFriendly,
                PercentTrains = this.PercentTrains,
                PercentTrams = this.PercentTrams,
                TotalJourneyTime = this.TotalJourneyTime,
                TotalTravelTime = this.TotalTravelTime,
                JourneyLegs = this.JourneyLegs,
                WalkingTime = this.WalkingTime,
                TotalWaitingTime = this.TotalWaitingTime
            };
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(Fitness other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.TotalTravelTime.Equals(this.TotalTravelTime)
                   && other.TotalWaitingTime.Equals(this.TotalWaitingTime) && other.WalkingTime.Equals(this.WalkingTime)
                   && other.Changes == this.Changes && other.Co2Emmissions.Equals(this.Co2Emmissions)
                   && other.PercentTrains.Equals(this.PercentTrains) && other.PercentBuses.Equals(this.PercentBuses)
                   && other.PercentTrams.Equals(this.PercentTrams)
                   && other.NormalisedJourneyTime.Equals(this.NormalisedJourneyTime)
                   && other.NormalisedChanges.Equals(this.NormalisedChanges)
                   && other.PercentDisableFriendly.Equals(this.PercentDisableFriendly)
                   && other.TotalJourneyTime.Equals(this.TotalJourneyTime);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(Fitness))
            {
                return false;
            }

            return this.Equals((Fitness)obj);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The equals.
        /// </returns>
        public bool Equals(Fitness x, Fitness y)
        {
            return this.Equals(x, y);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = this.TotalTravelTime.GetHashCode();
                result = (result * 397) ^ this.TotalWaitingTime.GetHashCode();
                result = (result * 397) ^ this.WalkingTime.GetHashCode();
                result = (result * 397) ^ this.Changes;
                result = (result * 397) ^ this.Co2Emmissions.GetHashCode();
                result = (result * 397) ^ this.PercentTrains.GetHashCode();
                result = (result * 397) ^ this.PercentBuses.GetHashCode();
                result = (result * 397) ^ this.PercentTrams.GetHashCode();
                result = (result * 397) ^ this.NormalisedJourneyTime.GetHashCode();
                result = (result * 397) ^ this.NormalisedChanges.GetHashCode();
                result = (result * 397) ^ this.PercentDisableFriendly.GetHashCode();
                result = (result * 397) ^ this.TotalJourneyTime.GetHashCode();
                return result;
            }
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The get hash code.
        /// </returns>
        public int GetHashCode(Fitness obj)
        {
            return obj.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("Changes: {0}, Co2Emmissions: {1}, JourneyLegs: {2}, NormalisedChanges: {3}, NormalisedJourneyTime: {4}, PercentBuses: {5}, PercentDisableFriendly: {6}, PercentTrains: {7}, PercentTrams: {8}, TotalJourneyTime: {9}, TotalTravelTime: {10}, TotalWaitingTime: {11}, WalkingTime: {12}", this.Changes, this.Co2Emmissions, this.JourneyLegs, this.NormalisedChanges, this.NormalisedJourneyTime, this.PercentBuses, this.PercentDisableFriendly, this.PercentTrains, this.PercentTrams, this.TotalJourneyTime, this.TotalTravelTime, this.TotalWaitingTime, this.WalkingTime);
        }

        #endregion
    }
}