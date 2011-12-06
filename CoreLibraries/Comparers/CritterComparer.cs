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
    /// TODO: Update summary.
    /// </summary>
    public class CritterComparer : IComparer<Types.Critter>
    {

        public int Compare(Types.Critter x, Types.Critter y)
        {
            return (int)Math.Round(y.Fitness - x.Fitness);
        }
    }
}
