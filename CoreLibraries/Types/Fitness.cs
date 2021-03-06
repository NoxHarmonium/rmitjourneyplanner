﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Fitness.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   A list of the parameters provided by the fitness class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// A list of the parameters provided by the fitness class.
    /// </summary>
    public enum FitnessParameter
    {
        /// <summary>
        ///   Gets the non normalised journey time in fractional minutes.
        /// </summary>
        TotalJourneyMinutes, 

        /// <summary>
        ///   Gets the normalised changes value.
        /// </summary>
        NormalisedChanges, 

        /// <summary>
        ///   The total journey time.
        /// </summary>
        TotalJourneyTime, 

        /// <summary>
        ///   The non normalised changes.
        /// </summary>
        Changes, 

        /// <summary>
        ///   The walking time.
        /// </summary>
        WalkingTime, 

        /// <summary>
        ///   The percent trams.
        /// </summary>
        PercentTrams, 

        /// <summary>
        ///   The percent buses.
        /// </summary>
        PercentBuses, 

        /// <summary>
        ///   The percent trains.
        /// </summary>
        PercentTrains, 

        /// <summary>
        ///   The normalised CO2 emissions.
        /// </summary>
        Co2Emmissions, 

        /// <summary>
        ///   The percent disable friendly.
        /// </summary>
        PercentDisableFriendly, 

        /// <summary>
        ///   The total travel time.
        /// </summary>
        TotalTravelTime, 

        /// <summary>
        ///   The total waiting time.
        /// </summary>
        TotalWaitingTime, 

        /// <summary>
        ///   The total travel minutes.
        /// </summary>
        TotalTravelMinutes, 

        /// <summary>
        ///   The diversity metric.
        /// </summary>
        DiversityMetric, 

        /// <summary>
        ///   The total distance.
        /// </summary>
        TotalDistance
    }

    /// <summary>
    /// Represents the different aspects of a journey in order to evaluate fitness.
    /// </summary>
    public class Fitness : ICloneable, IEquatable<Fitness>, IEqualityComparer<Fitness>
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Fitness" /> class. 
        ///   Initializes a new instance of the <see cref = "T:System.Object" /> class.
        /// </summary>
        public Fitness()
        {
            this.JourneyLegs = new List<JourneyLeg>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the amount of fitness parameters available.
        /// </summary>
        public static int ParameterCount
        {
            get
            {
                return Enum.GetValues(typeof(FitnessParameter)).Length;
            }
        }

        /// <summary>
        ///   Gets or sets the amount of mode changes of the journey.
        /// </summary>
        public int Changes { get; set; }

        /// <summary>
        ///   Gets or sets the amount of CO2 emitted in this journey.
        /// </summary>
        public double Co2Emmissions { get; set; }

        /// <summary>
        ///   Gets or sets the diversity metric.
        /// </summary>
        public double DiversityMetric { get; set; }

        /// <summary>
        ///   Gets or sets the list of legs that make up a journey.
        /// </summary>
        public List<JourneyLeg> JourneyLegs { get; set; }

        /// <summary>
        ///   Gets the amount of fitness parameters available.
        /// </summary>
        public int Length
        {
            get
            {
                return Enum.GetValues(typeof(FitnessParameter)).Length;
            }
        }

        /// <summary>
        ///   Gets or sets the normalised changes value.
        /// </summary>
        public double NormalisedChanges { get; set; }

        /// <summary>
        ///   Gets or sets the normalised journey time of the leg (0.0-1.0).
        /// </summary>
        public double NormalisedJourneyTime { get; set; }

        /// <summary>
        ///   Gets or sets the normalised travel time of the leg (0.0-1.0).
        /// </summary>
        public double NormalisedTravelTime { get; set; }

        /// <summary>
        ///   Gets or sets the percent of the journey that uses buses. (0.0 - 1.0)
        /// </summary>
        public double PercentBuses { get; set; }

        /// <summary>
        ///   Gets or sets the percent of the journey that is accessible for disabled passengers. (0.0 - 1.0)
        /// </summary>
        public double PercentDisableFriendly { get; set; }

        /// <summary>
        ///   Gets or sets the percent of the journey that uses trains. (0.0 - 1.0)
        /// </summary>
        public double PercentTrains { get; set; }

        /// <summary>
        ///   Gets or sets the percent of the journey that uses trams. (0.0 - 1.0)
        /// </summary>
        public double PercentTrams { get; set; }

        /// <summary>
        ///   Gets or sets the total distance of the journey.
        /// </summary>
        public double TotalDistance { get; set; }

        /// <summary>
        ///   Gets or sets the total time of the whole journey.
        /// </summary>
        public TimeSpan TotalJourneyTime { get; set; }

        /// <summary>
        ///   Gets or sets the total time spent on a service in the journey.
        /// </summary>
        public TimeSpan TotalTravelTime { get; set; }

        /// <summary>
        ///   Gets or sets the total time spent waiting for a service in the journey.
        /// </summary>
        public TimeSpan TotalWaitingTime { get; set; }

        /// <summary>
        ///   Gets or sets the total time walking in a journey.
        /// </summary>
        public TimeSpan WalkingTime { get; set; }

        #endregion

        #region Public Indexers

        /// <summary>
        ///   References the different fitness parameters with a <see cref = "FitnessParameter" /> enumeration. The parameters are all returned so that they suit a minimization problem. (Lower is better).
        /// </summary>
        /// <param name = "fitnessParameter">The <see cref = "FitnessParameter" /> value corresponding to a fitness parameter.</param>
        /// <returns>
        ///   The value of the specified fitness parameter.
        /// </returns>
        public double this[FitnessParameter fitnessParameter]
        {
            get
            {
                return this[(int)fitnessParameter];
            }
        }

        /// <summary>
        ///   References the different fitness parameters with an index. The parameters are all returned so that they suit a minimization problem. (Lower is better).
        /// </summary>
        /// <param name = "i">The index of the parameter. <see cref = "FitnessParameter" /></param>
        /// <returns>
        ///   The value of the specified fitness parameter.
        /// </returns>
        /// <exception cref = "ArgumentOutOfRangeException">Thrown if a index is referenced that is out of range.</exception>
        public double this[int i]
        {
            get
            {
                switch ((FitnessParameter)i)
                {
                    case FitnessParameter.TotalDistance:
                        return this.TotalDistance;

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

                    case FitnessParameter.TotalTravelMinutes:
                        return this.TotalTravelTime.TotalMinutes;

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
                        return this.NormalisedTravelTime;

                    case FitnessParameter.TotalWaitingTime:
                        return this.TotalWaitingTime.TotalSeconds;

                    case FitnessParameter.DiversityMetric:
                        return 1 - this.DiversityMetric;
                    default:
                        throw new ArgumentOutOfRangeException("i");
                }
            }
        }

        #endregion

        //// TODO: Do we need the addition and division operators for the Fitness object?
        #region Public Methods and Operators

        /// <summary>
        ///   Adds 2 fitness objects together
        /// </summary>
        /// <param name = "c1">The first <see cref = "Fitness" /> object.</param>
        /// <param name = "c2">The second <see cref = "Fitness" /> object.</param>
        /// <returns>The result of the addition.</returns>
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
                    TotalWaitingTime = c1.TotalWaitingTime + c2.TotalWaitingTime, 
                    DiversityMetric = c1.DiversityMetric + c2.DiversityMetric
                };
            return fitness;
        }

        /// <summary>
        ///   Divides all the internal values by the specified double.
        /// </summary>
        /// <param name = "c1">The <see cref = "Fitness" /> object.</param>
        /// <param name = "c2">The coefficient for the division.</param>
        /// <returns>The result of the division.</returns>
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
                    TotalWaitingTime = TimeSpan.FromMilliseconds(c1.TotalWaitingTime.TotalMilliseconds / c2), 
                    DiversityMetric = c1.DiversityMetric / c2
                };
            return fitness;
        }

        /// <summary>
        ///   The equality operator for a <see cref = "Fitness" /> object.
        /// </summary>
        /// <param name = "left">
        ///   The object on the left side of the statement.
        /// </param>
        /// <param name = "right">
        ///   The object on the right side of the statement.
        /// </param>
        /// <returns>
        ///   If the two objects are equal by value.
        /// </returns>
        public static bool operator ==(Fitness left, Fitness right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///   The inequality operator for a <see cref = "Fitness" /> object.
        /// </summary>
        /// <param name = "left">
        ///   The object on the left side of the statement.
        /// </param>
        /// <param name = "right">
        ///   The object on the right side of the statement.
        /// </param>
        /// <returns>
        ///   If the two objects are equal by value.
        /// </returns>
        public static bool operator !=(Fitness left, Fitness right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a copy of this <see cref="Fitness"/> object.
        /// </summary>
        /// <returns>
        /// The a new instance of the <see cref="Fitness"/> class with the same properties as this one.
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
                    TotalWaitingTime = this.TotalWaitingTime, 
                    DiversityMetric = this.DiversityMetric
                };
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type according to value.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(Fitness other)
        {
            return this.Equals(this, other);
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
        /// Returns if the two specified <see cref="Fitness"/> objects are equal according to value.
        /// </summary>
        /// <param name="c1">
        /// The first object under comparison.
        /// </param>
        /// <param name="c2">
        /// The second object under comparison.
        /// </param>
        /// <returns>
        /// True if the objects are equal, otherwise false.
        /// </returns>
        public bool Equals(Fitness c1, Fitness c2)
        {
            if (ReferenceEquals(null, c1) || ReferenceEquals(null, c2))
            {
                return false;
            }

            if (ReferenceEquals(c1, c2))
            {
                return true;
            }

            return c1.TotalTravelTime.Equals(c2.TotalTravelTime)
                   && c1.TotalWaitingTime.Equals(c2.TotalWaitingTime) && c1.WalkingTime.Equals(c2.WalkingTime)
                   && c1.Changes == c2.Changes && c1.Co2Emmissions.Equals(c2.Co2Emmissions)
                   && c1.PercentTrains.Equals(c2.PercentTrains) && c1.PercentBuses.Equals(c2.PercentBuses)
                   && c1.PercentTrams.Equals(c2.PercentTrams)
                   && c1.NormalisedJourneyTime.Equals(c2.NormalisedJourneyTime)
                   && c1.NormalisedChanges.Equals(c2.NormalisedChanges)
                   && c1.PercentDisableFriendly.Equals(c2.PercentDisableFriendly)
                   && c1.TotalJourneyTime.Equals(c2.TotalJourneyTime);
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
        /// Returns the hash code of a <see cref="Fitness"/> object.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="Fitness"/> object to get the hash code of.
        /// </param>
        /// <returns>
        /// The hash code of the specified object.
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
            return
                string.Format(
                    "Changes: {0}, Co2Emmissions: {1}, JourneyLegs: {2}, NormalisedChanges: {3}, NormalisedJourneyTime: {4}, PercentBuses: {5}, PercentDisableFriendly: {6}, PercentTrains: {7}, PercentTrams: {8}, TotalJourneyTime: {9}, TotalTravelTime: {10}, TotalWaitingTime: {11}, WalkingTime: {12}", 
                    this.Changes, 
                    this.Co2Emmissions, 
                    this.JourneyLegs, 
                    this.NormalisedChanges, 
                    this.NormalisedJourneyTime, 
                    this.PercentBuses, 
                    this.PercentDisableFriendly, 
                    this.PercentTrains, 
                    this.PercentTrams, 
                    this.TotalJourneyTime, 
                    this.TotalTravelTime, 
                    this.TotalWaitingTime, 
                    this.WalkingTime);
        }

        #endregion
    }
}