// -----------------------------------------------------------------------
// <copyright file="AdjacencyDepthFirstSearch.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.TreeAlgorithms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AdjacencyDepthFirstSearch : DepthFirstSearch<int>
    {
        private readonly int[][] adjaencyMatrix;

        /// <summary>
        /// Creates a simple adjacency matrix DFS.
        /// </summary>
        /// <param name="adjaencyMatrix"></param>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        public AdjacencyDepthFirstSearch(int[][] adjaencyMatrix, int origin, int destination) : base(true, origin, destination)
        {
            this.adjaencyMatrix = adjaencyMatrix;
        }

        protected override int[] GetChildren(int node)
        {
            var adjacents = new List<int>();
            for (var i = 0; i < adjaencyMatrix[node].Length; i++)
            {
                if (adjaencyMatrix[node][i] == 1)
                {
                    adjacents.Add(i);

                }

            }
            return adjacents.ToArray();
            //return ;
        }

        protected override int[] OrderChildren(int[] nodes)
        {
            return nodes;
        }
    }
}
