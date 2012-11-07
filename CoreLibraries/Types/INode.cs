// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INode.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Represents a node that can be used in a search algorithm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    /// <summary>
    /// Represents a node that can be used in a search algorithm.
    /// </summary>
    public interface INode
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the parent node to this node. Used for traversing route trees.
        /// </summary>
        INode Parent { get; set; }

        #endregion
    }
}