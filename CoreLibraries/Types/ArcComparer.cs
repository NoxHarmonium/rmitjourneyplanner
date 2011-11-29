// -----------------------------------------------------------------------
// <copyright file="ArcComparer.cs" company="">
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
    /// Compare object for sorting lists of arcs
    /// </summary>
    public class ArcComparer : IComparer<Arc>
    {
        /// <summary>
        /// Compares 2 arcs and returns the difference between them.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(Arc x, Arc y)
        {
            return (x.Time - y.Time).Milliseconds;
        }
    }
}
