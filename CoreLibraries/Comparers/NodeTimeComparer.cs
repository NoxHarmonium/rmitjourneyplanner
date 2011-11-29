// -----------------------------------------------------------------------
// <copyright file="NodeComparer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DataProviders;

    /// <summary>
    /// Compares to nodes on the basis on total time traversed.
    /// </summary>
    public class NodeTimeComparer : IComparer<INetworkNode>
    {
        /// <summary>
        /// Compares 2 nodes on the total time. Sorts descending.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(INetworkNode x, INetworkNode y)
        {
            return (y.TotalTime - x.TotalTime).Milliseconds;
        }
    }
}
