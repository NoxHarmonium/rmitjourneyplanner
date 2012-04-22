// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.Comparers
{
    #region

    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   Compares 2 critters based on thier fitness.
    /// </summary>
    public class CritterComparer : IComparer<Critter>
    {
        #region Public Methods

        /// <summary>
        ///   Compares 2 critters based on thier fitness.
        /// </summary>
        /// <param name="x"> The first critter. </param>
        /// <param name="y"> The second critter. </param>
        /// <returns> The compare. </returns>
        public int Compare(Critter x, Critter y)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (x.Fitness == y.Fitness) // ReSharper restore CompareOfFloatsByEqualityOperator
            {
                return 0;
            }

            if (x.Fitness > y.Fitness)
            {
                return 1;
            }

            return -1;
        }

        #endregion
    }
}