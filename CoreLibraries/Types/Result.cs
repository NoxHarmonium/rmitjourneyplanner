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
    public class Result
    {
        /// <summary>
        /// The total time to execute the iteration.
        /// </summary>
        public TimeSpan Totaltime;

        /// <summary>
        /// The minimum fitness value of the iteration.
        /// </summary>
        public Fitness MinimumFitness = new Fitness();

        /// <summary>
        /// The average fitness value of the iteration.
        /// </summary>
        public Fitness AverageFitness = new Fitness();

        /// <summary>
        /// The population of the previous iteration.
        /// </summary>
        public List<Critter> Population = new List<Critter>();

        /// <summary>
        /// A measure of how diverse the population is.
        /// </summary>
        public double DiversityMetric;

        /// <summary>
        /// Returns the string representation of this result.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture,"[Total Time, Min UnifiedFitnessScore, Average UnifiedFitnessScore, Diversity Metric][{0},{1},{2},{3}]", Totaltime, MinimumFitness, AverageFitness,DiversityMetric);
        }

       
    }
}
