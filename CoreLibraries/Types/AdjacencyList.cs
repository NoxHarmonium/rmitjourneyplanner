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

    /// <summary>
    /// TODO: Update summary.
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
        /// The degree.
        /// </summary>
        /// <param name="id">
        /// The id. 
        /// </param>
        /// <returns>
        /// The degree. 
        /// </returns>
        public int Degree(int id)
        {
            return this.list[id].Count;
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion
    }
}