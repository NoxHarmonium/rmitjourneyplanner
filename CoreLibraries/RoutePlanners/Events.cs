﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="Events.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Represents the data produced by a next iteration event.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners
{
    #region

    using System;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    #endregion

    /// <summary>
    /// Represents the data produced by a next iteration event.
    /// </summary>
    public class NextIterationEventArgs : EventArgs
    {
        #region Constants and Fields

        /// <summary>
        ///   The node.
        /// </summary>
        private readonly INetworkNode node;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NextIterationEventArgs"/> class. 
        ///   Initilizes a new instance of a NextIterationEventArgs object.
        /// </summary>
        /// <param name="currentNode">
        /// The current node being traversed.
        /// </param>
        public NextIterationEventArgs(INetworkNode currentNode)
        {
            this.node = currentNode;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the current node that the algorithm is expanding on this iteration.
        /// </summary>
        public INetworkNode CurrentNode
        {
            get
            {
                return this.node;
            }
        }

        #endregion
    }
}