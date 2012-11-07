// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Events.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Represents the data produced by a next iteration event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners
{
    #region Using Directives

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
        /// Initializes a new instance of the <see cref="NextIterationEventArgs"/> class. Initilizes a new instance of a NextIterationEventArgs object.
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