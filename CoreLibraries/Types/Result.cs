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
        public List<Critter> Population = new List<Critter>();

        /// <summary>
        /// Returns a new result that is a clone this instance.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Result
                { Totaltime = this.Totaltime, Population = this.Population.ConvertAll(t => (Critter)t.Clone()) };
        }
    }
}
