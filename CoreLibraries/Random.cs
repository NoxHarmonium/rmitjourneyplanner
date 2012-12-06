// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Random.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Wraps <see cref="System.Random" /> to be thread safe. Initializes one random object per thread.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries
{
    #region Using Directives

    using System;

    using NPack;

    #endregion

    /// <summary>
    /// Wraps <see cref="MersenneTwister"/> to be thread safe. Initializes one random object per thread.
    /// </summary>
    public class Random
    {
        #region Constants and Fields

        /// <summary>
        ///   The random.
        /// </summary>
        [ThreadStatic]
        private static MersenneTwister random;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns a thread safe version of the <see cref="MersenneTwister"/> class.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="MersenneTwister"/> class.
        /// </returns>
        public static MersenneTwister GetInstance()
        {
            return random ?? (random = new MersenneTwister());
        }

        #endregion
    }
}