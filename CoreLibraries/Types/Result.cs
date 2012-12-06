// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Result.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Represents a set of results from a journey planning operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region Using Directives

    using System;

    #endregion

    /// <summary>
    /// Represents a set of results from a journey planning operation.
    /// </summary>
    public class Result : ICloneable
    {
        #region Constants and Fields

        /// <summary>
        ///   The amount of unique journeys in the iteration.
        /// </summary>
        public int Cardinality;

        /// <summary>
        ///   The hypervolume of the iteration.
        /// </summary>
        public double Hypervolume = double.NaN;

        /// <summary>
        ///   The population of the previous iteration.
        /// </summary>
        public Population Population = new Population();

        /// <summary>
        ///   The total time to execute the iteration.
        /// </summary>
        public TimeSpan Totaltime;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns a new result that is a clone this instance.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public object Clone()
        {
            return new Result
                {
                    Totaltime = this.Totaltime, 
                    Population = (Population)this.Population.Clone(), 
                    Cardinality = this.Cardinality, 
                    Hypervolume = this.Hypervolume
                };
        }

        #endregion
    }
}