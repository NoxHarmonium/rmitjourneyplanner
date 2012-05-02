// -----------------------------------------------------------------------
// <copyright file="INode.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    /// <summary>
    /// Represents a node that can be used in a search algorithm.
    /// </summary>
    public interface INode
    {
        /// <summary>
        ///   Gets or sets the parent node to this node. Used for traversing route trees.
        /// </summary>
        INode Parent { get; set; }
    }
}
