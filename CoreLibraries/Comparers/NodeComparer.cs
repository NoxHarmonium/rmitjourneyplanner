// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.Comparers
{
    #region

    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;

    #endregion

    /// <summary>
    ///   Compares to nodes on the basis on total time traversed.
    /// </summary>
    public class NodeComparer<T> : IComparer<NodeWrapper<T>> where T : INetworkNode
    {
        #region Public Methods

        /// <summary>
        ///   Compares 2 nodes on the total time. Sorts descending.
        /// </summary>
        /// <param name="x"> The first node to compare. </param>
        /// <param name="y"> The second node to compare. </param>
        /// <returns> A number less than 0 if x is less than y, and number greater than 0 if y is less than x and 0 if they are equal./&gt; </returns>
        public int Compare(NodeWrapper<T> x, NodeWrapper<T> y)
        {
            double fx = (x.TotalTime.Ticks / 1000000000.0) + x.EuclidianDistance;
            double fy = (y.TotalTime.Ticks / 1000000000.0) + y.EuclidianDistance;
            if (x.EuclidianDistance == 0)
            {
                fx = 0;
            }

            if (y.EuclidianDistance == 0)
            {
                fy = 0;
            }

            int d = (int)(fx * 1000) - (int)(fy * 1000);
            return d;

            // return ((int)(y.TotalTime - x.TotalTime).TotalMilliseconds) + (int)((y.EuclidianDistance-x.EuclidianDistance)*1000.0);
            // int d = (int)((y.EuclidianDistance - x.EuclidianDistance) * 1000.0);
            // return d;
        }

        #endregion
    }
}