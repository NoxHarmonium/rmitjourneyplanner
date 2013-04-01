// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdjacencyList.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region Using Directives

    using System.Collections;
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders.Ptv;

    #endregion

    //// TODO: Make this generic so that you can have different kinds of nodes.

    /// <summary>
    /// A class that handles adjacency relationships between <see cref="PtvNode"/> objects.
    /// </summary>
    public class AdjacencyList : IEnumerable
    {
        #region Constants and Fields

        /// <summary>
        ///   The list.
        /// </summary>
        private readonly Dictionary<int, List<PtvNode>> list = new Dictionary<int, List<PtvNode>>();

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets NodeCount.
        /// </summary>
        public int NodeCount
        {
            get
            {
                return this.list.Count;
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        ///   Gets a list of adjacent nodes to the specified id.
        /// </summary>
        /// <param name = "id"> The node id. </param>
        /// <returns>A list of adjacent nodes to the specified id.</returns>
        public List<PtvNode> this[int id]
        {
            get
            {
                return this.list[id];
            }

            set
            {
                this.list[id] = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns the number of adjacent nodes to the specified node ID.
        /// </summary>
        /// <param name="id">
        /// The node id. 
        /// </param>
        /// <returns>
        /// The number of adjacent nodes to the specified node ID.
        /// </returns>
        public int Degree(int id)
        {
            return this.list[id].Count;
        }

        /// <summary>
        /// Gets an enumerator to enumerate over all the node in the adjacency list.
        /// </summary>
        /// <returns>
        /// A enumerator for use in for loops.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion
    }
}