// -----------------------------------------------------------------------
// <copyright file="Result.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Represents a set of results from a journey planning operation.
    /// </summary>
    public class Result : ICloneable
    {
        /// <summary>
        /// The total time to execute the iteration.
        /// </summary>
        public TimeSpan Totaltime;
        
        /// <summary>
        /// The population of the previous iteration.
        /// </summary>
        public Population Population = new Population();

        /// <summary>
        /// The hypervolume of the iteration.
        /// </summary>
        public double Hypervolume = Double.NaN;

        /// <summary>
        /// Returns a new result that is a clone this instance.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Result
                { Totaltime = this.Totaltime, Population = (Population) this.Population.Clone() };
        }
    }
}
