// -----------------------------------------------------------------------
// <copyright file="CritterComparer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Compares 2 critters based on thier fitness.
    /// </summary>
    public class CritterComparer : IComparer<Types.Critter>
    {
        /// <summary>
        /// Compares 2 critters based on thier fitness.
        /// </summary>
        /// <param name="x">
        /// The first critter.
        /// </param>
        /// <param name="y">
        /// The second critter.
        /// </param>
        /// <returns>
        /// </returns>
        public int Compare(Types.Critter x, Types.Critter y)
        {
            return (int)Math.Round(y.Fitness - x.Fitness);
        }
    }
}
