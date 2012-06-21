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

    /// <summary>
    /// Represents a generic node for use in route searches. 
    /// Basically a wrapper for a generic type to give it a unique address for referencing.
    /// </summary>
    public class NodeWrapper<T>
    {
        private readonly T node;

        private double cost;

        /// <summary>
        /// Creates a new instance of the NodeWrapper class.
        /// </summary>
        /// <param name="node"></param>
        public NodeWrapper(T node)
        {
            this.node = node;
            this.cost = 0;
        }

        /// <summary>
        /// Creates a new instance of the NodeWrapper class.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="cost"></param>
        public NodeWrapper(T node, double cost)
        {
            this.node = node;
            this.cost = cost;

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
        /// Gets or sets the cost associated with this wrapper.
        /// </summary>
        public double Cost
        {
            get
            {
                return this.cost;
            }
            set
            {
                this.cost = value;
            }
        }
    }
}
