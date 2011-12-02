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
    public class NodeComparer : IComparer<INetworkNode>
    {
        /// <summary>
        /// Compares 2 nodes on the total time. Sorts descending.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(INetworkNode x, INetworkNode y)
        {
            //double fx = x.TotalTime.Milliseconds + (x.EuclidianDistance * 100000.0);
            //double fy = y.TotalTime.Milliseconds + (y.EuclidianDistance * 100000.0);

             
            

            double fx = ((double)x.TotalTime.Ticks/1000000000.0 + x.EuclidianDistance);
            double fy = ((double)y.TotalTime.Ticks/1000000000.0 + y.EuclidianDistance);
            if (x.EuclidianDistance == 0)
            {
                fx = 0;
            }

            if (y.EuclidianDistance == 0)
            {
                fy = 0;
            }
            int d = (int)fy - (int)fx;
            return d;
            
            //return ((int)(y.TotalTime - x.TotalTime).TotalMilliseconds) + (int)((y.EuclidianDistance-x.EuclidianDistance)*1000.0);
            //int d = (int)((y.EuclidianDistance - x.EuclidianDistance) * 1000.0);
            //return d;
        }
    }
}
