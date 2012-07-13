// -----------------------------------------------------------------------
// <copyright file="Fitness.cs" company="">
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
    /// Represents the different aspects of a journey in order to evaluate fitness.
    /// </summary>
    public class Fitness
    {
        
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

        /// <summary>
        /// The total time spent on a service in the journey.
        /// </summary>
        public TimeSpan TotalTravelTime { get; set; }

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
    }
}
