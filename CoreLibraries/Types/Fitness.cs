// -----------------------------------------------------------------------
// <copyright file="Fitness.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A list of the parameters provided by the fitness class.
    /// </summary>
    public enum FitnessParameters 
    {
        //TotalJourneyTime,
        Changes,
        //WalkingTime,
        PercentTrams,
        //PercentBuses,
        PercentTrains,
        //PercentTrams,
       // Co2Emmissions,
        //PercentDisableFriendly,
      
        //TotalTravelTime ,
        //TotalWaitingTime

    }

    /// <summary>
    /// Represents the different aspects of a journey in order to evaluate fitness.
    /// </summary>
    public class Fitness : ICloneable, IEquatable<Fitness>, IEqualityComparer<Fitness>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Fitness()
        {
            JourneyLegs = new List<JourneyLeg>();
        }

        /// <summary>
        /// Returns the amount of fitness parameters available.
        /// </summary>
        public int Length
        {
            get
            {
                return Enum.GetValues(typeof(FitnessParameters)).Length;
            }
        }

        /// <summary>
        /// Returns the amount of fitness parameters available.
        /// </summary>
        public static int ParameterCount
        {
            get
            {
                return Enum.GetValues(typeof(FitnessParameters)).Length;
            }
        }

       
        
        /// <summary>
        /// References the different fitness parameters with an index. The parameters are all returned so that they suit a minimization problem. (Lower is better).
        /// </summary>
        /// <param name="i">The index of the parameter. <see cref="FitnessParameters"/></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if a index is refereced that is out of range.</exception>
        public double this [int i]
        {

            get
            {
                switch ((FitnessParameters)i)
                {
                    //case FitnessParameters.TotalJourneyTime:
                        //if (this.TotalJourneyTime.TotalSeconds <= 0) return double.MaxValue;
                    //    return NormalisedTravelTime;
                     
                    case FitnessParameters.Changes:
                        return this.NormalisedChanges;

                    // case FitnessParameters.WalkingTime:
                     //  return this.WalkingTime.TotalSeconds;
                    //case FitnessParameters.Co2Emmissions:
                    //    return this.Co2Emmissions;
//
                   // case FitnessParameters.PercentBuses:
                  //     return (1 - this.PercentBuses);

                    //case FitnessParameters.PercentDisableFriendly:
                    //    return 1 - this.PercentDisableFriendly;

                   case FitnessParameters.PercentTrains:
                      return 1 - this.PercentTrains;

                    case FitnessParameters.PercentTrams:
                       return 1 - this.PercentTrams;

                   // case FitnessParameters.PercentTrams:
                    //   return 1 - this.PercentTrams;

                    //case FitnessParameters.TotalTravelTime:
                   //     return this.TotalTravelTime.TotalSeconds;
                        
                    //case FitnessParameters.TotalWaitingTime:
                    //    return this.TotalWaitingTime.TotalSeconds;
                    default:
                        throw new ArgumentOutOfRangeException("i");
                }
            }
        }
        
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
        public static Fitness operator / (Fitness c1, double c2)
        {
            var fitness = new Fitness
            {
                Changes = (int) (c1.Changes / c2),
                Co2Emmissions = c1.Co2Emmissions / c2,
                PercentBuses = c1.PercentBuses / c2,
                PercentDisableFriendly = c1.PercentDisableFriendly /c2,
                PercentTrains = c1.PercentTrains / c2,
                PercentTrams = c1.PercentTrams / c2,
                TotalJourneyTime = TimeSpan.FromMilliseconds(c1.TotalJourneyTime.TotalMilliseconds / c2),
                TotalTravelTime = TimeSpan.FromMilliseconds(c1.TotalTravelTime.TotalMilliseconds / c2),
                TotalWaitingTime = TimeSpan.FromMilliseconds(c1.TotalWaitingTime.TotalMilliseconds / c2)
            };
            return fitness;

        }
        /// <summary>
        /// The total time spent waiting for a service in the journey.
        /// </summary>
        public TimeSpan TotalWaitingTime { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("TotalWaitingTime: {0}, TotalTravelTime: {1}, TotalJourneyTime: {2}, Changes: {3}, Co2Emmissions: {4}, PercentTrains: {5}, PercentBuses: {6}, PercentTrams: {7}, PercentDisableFriendly: {8}", this.TotalWaitingTime, this.TotalTravelTime, this.TotalJourneyTime, this.Changes, this.Co2Emmissions, this.PercentTrains, this.PercentBuses, this.PercentTrams, this.PercentDisableFriendly);
        }

        public object Clone()
        {
            return new Fitness()
                {
                    Changes = this.Changes,
                    Co2Emmissions = this.Co2Emmissions,
                    NormalisedChanges = this.NormalisedChanges,
                    NormalisedTravelTime = this.NormalisedTravelTime,
                    PercentBuses = this.PercentBuses,
                    PercentDisableFriendly = this.PercentDisableFriendly,
                    PercentTrains = this.PercentTrains,
                    PercentTrams = this.PercentTrams,
                    TotalJourneyTime = this.TotalJourneyTime,
                    TotalTravelTime = this.TotalTravelTime,
                    JourneyLegs =  this.JourneyLegs
                };
        }

        /// <summary>
        /// The total time spent on a service in the journey.
        /// </summary>
        public TimeSpan TotalTravelTime { get; set; }

        /// <summary>
        /// Gets or sets the total time walking in a journey.
        /// </summary>
        public TimeSpan WalkingTime { get; set; }

        /// <summary>
        /// The total time of the whole journey.
        /// </summary>
        public TimeSpan TotalJourneyTime { get; set; }

        /// <summary>
        /// The amount of mode changes of the journey.
        /// </summary>
        public int Changes { get; set; }

        /// <summary>
        /// The amount of CO2 emmitted in this journey.
        /// </summary>
        public double Co2Emmissions { get; set; }

        /// <summary>
        /// The percent of the journey that uses trains. (0.0 - 1.0)
        /// </summary>
        public double PercentTrains { get; set; }

        /// <summary>
        /// The percent of the journey that uses buses. (0.0 - 1.0)
        /// </summary>
        public double PercentBuses { get; set; }

        /// <summary>
        /// The percent of the journey that uses trams. (0.0 - 1.0)
        /// </summary>
        public double PercentTrams { get; set; }

        /// <summary>
        /// The percent of the journey that is accessable for disabled passengers. (0.0 - 1.0)
        /// </summary>
        public double PercentDisableFriendly { get; set; }

        /// <summary>
        /// The normalised journey time of the leg (0.0-1.0).
        /// </summary>
        public double NormalisedTravelTime { get; set; }

        /// <summary>
        /// The list of legs that make up a journey.
        /// </summary>
        public List<JourneyLeg> JourneyLegs { get; set; } 

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
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
            return other.TotalTravelTime.Equals(this.TotalTravelTime) && other.TotalWaitingTime.Equals(this.TotalWaitingTime) && other.WalkingTime.Equals(this.WalkingTime) && other.Changes == this.Changes && other.Co2Emmissions.Equals(this.Co2Emmissions) && other.PercentTrains.Equals(this.PercentTrains) && other.PercentBuses.Equals(this.PercentBuses) && other.PercentTrams.Equals(this.PercentTrams) && other.NormalisedTravelTime.Equals(this.NormalisedTravelTime) && other.NormalisedChanges.Equals(this.NormalisedChanges) && other.PercentDisableFriendly.Equals(this.PercentDisableFriendly) && other.TotalJourneyTime.Equals(this.TotalJourneyTime);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
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
            return Equals((Fitness)obj);
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
                result = (result * 397) ^ this.NormalisedTravelTime.GetHashCode();
                result = (result * 397) ^ this.NormalisedChanges.GetHashCode();
                result = (result * 397) ^ this.PercentDisableFriendly.GetHashCode();
                result = (result * 397) ^ this.TotalJourneyTime.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(Fitness left, Fitness right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Fitness left, Fitness right)
        {
            return !left.Equals(right);
        }

        public double NormalisedChanges { get; set; }

        public bool Equals(Fitness x, Fitness y)
        {
            return this.Equals(x, y);
        }

        public int GetHashCode(Fitness obj)
        {
            return obj.GetHashCode();
        }
    }
}
