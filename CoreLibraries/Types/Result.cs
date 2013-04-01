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
        ///   The population of the previous iteration.
        /// </summary>
        private readonly Population population = new Population();

        /// <summary>
        ///   The hypervolume of the iteration.
        /// </summary>
        private double hypervolume = double.NaN;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the amount of unique journeys in the iteration.
        /// </summary>
        public int Cardinality { get; set; }

        /// <summary>
        ///   Gets or sets the hypervolume value of this iteration.
        /// </summary>
        public double Hypervolume
        {
            get
            {
                return this.hypervolume;
            }

            set
            {
                this.hypervolume = value;
            }
        }

        /// <summary>
        ///   Gets or sets the population of the previous iteration.
        /// </summary>
        public Population Population { get; set; }

        /// <summary>
        ///   Gets or sets the total time to execute the iteration.
        /// </summary>
        public TimeSpan Totaltime { get; set; }

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

        /// <summary>
        /// Returns a new result that is a clone this instance.
        /// </summary>
        /// <param name="shallowCopy">Set to trur to clone only the basic parameters and not the population.</param>
        /// <returns>
        /// The clone.
        /// </returns>
        public object Clone(bool shallowCopy)
        {
            if (shallowCopy)
            {
                return new Result
                    {
                        Totaltime = this.Totaltime,
                        Population = null,
                        Cardinality = this.Cardinality,
                        Hypervolume = this.Hypervolume
                    };
            }
            else
            {
                return this.Clone();
            }
        }

        #endregion
    }
}