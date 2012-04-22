// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region

    using System.Collections;
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;

    #endregion

    /// <summary>
    ///   TODO: Update summary.
    /// </summary>
    public class AdjacencyList : IEnumerable
    {
        #region Constants and Fields

        /// <summary>
        ///   The list.
        /// </summary>
        private readonly Dictionary<int, List<MetlinkNode>> list = new Dictionary<int, List<MetlinkNode>>();

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
        ///   The this.
        /// </summary>
        /// <param name="id"> The id. </param>
        public List<MetlinkNode> this[int id]
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

        #region Public Methods

        /// <summary>
        ///   The degree.
        /// </summary>
        /// <param name="id"> The id. </param>
        /// <returns> The degree. </returns>
        public int Degree(int id)
        {
            return this.list[id].Count;
        }

        /// <summary>
        ///   The get enumerator.
        /// </summary>
        /// <returns> </returns>
        public IEnumerator GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion
    }
}