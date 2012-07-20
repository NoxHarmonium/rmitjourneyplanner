// -----------------------------------------------------------------------
// <copyright file="Node.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.TreeAlgorithms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    /// <summary>
    /// Represents a generic node for use in route searches. 
    /// Basically a wrapper for a generic type to give it a unique address for referencing.
    /// </summary>
    public class NodeWrapper<T> : ICloneable
    {
        private readonly T node;

        private Guid guid;

        /// <summary>
        /// Creates a new instance of the NodeWrapper class.
        /// </summary>
        /// <param name="node"></param>
        public NodeWrapper(T node)
        {
            this.node = node;
            this.Cost = 0;
            guid = Guid.NewGuid();
        }

        /// <summary>
        /// Creates a new instance of the NodeWrapper class.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="cost"></param>
        public NodeWrapper(T node, double cost)
        {
            this.node = node;
            this.Cost = cost;
            guid = Guid.NewGuid();

        }

        /// <summary>
        /// Gets the internal node represented by the wrapper.
        /// </summary>
        public T Node
        {
            get
            {
                return this.node;
            }
        }

        /// <summary>
        ///   Gets or sets the current route that the node is traversing.
        /// </summary>
        public int CurrentRoute { get; set; }

        /// <summary>
        ///   Gets or sets the Euclidian distance to the goal. Used for traversing route trees.
        /// </summary>
        public double EuclidianDistance { get; set; }

        /// <summary>
        ///   Gets or sets the total time taken to reach this node. Used for traversing route trees.
        /// </summary>
        public TimeSpan TotalTime { get; set; }

        /// <summary>
        /// Gets or sets the cost associated with this wrapper.
        /// </summary>
        public double Cost { get; set; }

        public object Clone()
        {
            return new NodeWrapper<T>(this.node, this.Cost);
        }
    }
}
