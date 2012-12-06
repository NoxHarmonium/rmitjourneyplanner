// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeWrapper.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Represents a generic node for use in route searches.
//   Basically a wrapper for a generic type to give it a unique address for referencing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.TreeAlgorithms
{
    #region Using Directives

    using System;

    #endregion

    /// <summary>
    /// Represents a generic node for use in route searches. 
    ///   Basically a wrapper for a generic type to give it a unique address for referencing.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class NodeWrapper<T> : ICloneable
    {
        #region Constants and Fields

        /// <summary>
        ///   The node.
        /// </summary>
        private readonly T node;

        /// <summary>
        ///   The guid.
        /// </summary>
        private Guid guid;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeWrapper{T}"/> class. 
        ///   Creates a new instance of the NodeWrapper class.
        /// </summary>
        /// <param name="node">
        /// The node to wrap.
        /// </param>
        public NodeWrapper(T node)
        {
            this.node = node;
            this.Cost = 0;
            this.guid = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeWrapper{T}"/> class. 
        ///   Creates a new instance of the NodeWrapper class.
        /// </summary>
        /// <param name="node">
        /// </param>
        /// <param name="cost">
        /// </param>
        public NodeWrapper(T node, double cost)
        {
            this.node = node;
            this.Cost = cost;
            this.guid = Guid.NewGuid();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the cost associated with this wrapper.
        /// </summary>
        public double Cost { get; set; }

        /// <summary>
        ///   Gets or sets the current route that the node is traversing.
        /// </summary>
        public int CurrentRoute { get; set; }

        /// <summary>
        ///   Gets or sets the Euclidian distance to the goal. Used for traversing route trees.
        /// </summary>
        public double EuclidianDistance { get; set; }

        /// <summary>
        ///   Gets the internal node represented by the wrapper.
        /// </summary>
        public T Node
        {
            get
            {
                return this.node;
            }
        }

        /// <summary>
        ///   Gets or sets the total time taken to reach this node. Used for traversing route trees.
        /// </summary>
        public TimeSpan TotalTime { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public object Clone()
        {
            return new NodeWrapper<T>(this.node, this.Cost);
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The to string.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[NodeWrapper: node={0}]", this.node);
        }

        #endregion
    }
}