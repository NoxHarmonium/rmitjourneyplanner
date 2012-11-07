// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdjacencyList.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
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

    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;

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
        /// <param name = "id"> The id. </param>
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