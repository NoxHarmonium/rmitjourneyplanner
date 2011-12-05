// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="ArcComparer.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Compare object for sorting lists of arcs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Comparers
{
    #region

    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Compare object for sorting lists of arcs
    /// </summary>
    public class ArcComparer : IComparer<Arc>
    {
        #region Public Methods

        /// <summary>
        /// Compares 2 arcs using the time parameter.
        /// </summary>
        /// <param name="x">
        /// The first arc to compare.
        /// </param>
        /// <param name="y">
        /// The second arc to compare.
        /// </param>
        /// <returns>
        /// A number less than 0 if x is less than y, and number greater than 0 if y is less than x and  0 if they are equal./&gt;
        /// </returns>
        public int Compare(Arc x, Arc y)
        {
            return (int)(x.Time - y.Time).TotalMilliseconds;
        }

        #endregion
    }
}