// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="Route.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Represents a route made up of INetworkNodes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region

    using System;
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    #endregion

    /// <summary>
    /// Represents a route made up of INetworkNodes.
    /// </summary>
    public class Route : ICloneable
    {
        #region Constants and Fields

        /// <summary>
        ///   The down nodes.
        /// </summary>
        private readonly List<INetworkNode> downNodes = new List<INetworkNode>();

        /// <summary>
        ///   The id.
        /// </summary>
        private readonly string id;

        /// <summary>
        ///   The up nodes.
        /// </summary>
        private readonly List<INetworkNode> upNodes = new List<INetworkNode>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Route"/> class. 
        ///   Initializes a new instance of the the Route class.
        /// </summary>
        /// <param name="routeId">
        /// The Id of the specified route.
        /// </param>
        public Route(string routeId)
        {
            this.id = routeId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the route Id.
        /// </summary>
        public string Id
        {
            get
            {
                return this.id;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a node to the specfied route direction.
        /// </summary>
        /// <param name="node">
        /// The node to add to the route.
        /// </param>
        /// <param name="isUpDirection">
        /// A value determining what direction to add the node to.
        /// </param>
        public void AddNode(INetworkNode node, bool isUpDirection)
        {
            if (isUpDirection)
            {
                this.upNodes.Add(node);
            }
            else
            {
                this.downNodes.Add(node);
            }
        }

        /// <summary>
        /// Adds a range of nodes to the specfied route direction.
        /// </summary>
        /// <param name="nodes">
        /// The node collection to add to the route.
        /// </param>
        /// <param name="isUpDirection">
        /// A value determining what direction to add the node to.
        /// </param>
        public void AddNodeRange(List<INetworkNode> nodes, bool isUpDirection)
        {
            if (isUpDirection)
            {
                this.upNodes.AddRange(nodes);
            }
            else
            {
                this.downNodes.AddRange(nodes);
            }

        }

        /// <summary>
        /// Get the 2 adjacent nodes (one node up and one node down).
        /// </summary>
        /// <param name="node">
        /// The node to get the adjacent nodes for.
        /// </param>
        /// <returns>
        /// The 2 adjacent nodes.
        /// </returns>
        public List<INetworkNode> GetAdjacent(INetworkNode node)
        {
            List<INetworkNode> sourceNodes;
            var nodes = new List<INetworkNode>();

            if (this.upNodes.Contains(node))
            {
                sourceNodes = this.upNodes;
            }
            else if (this.downNodes.Contains(node))
            {
                sourceNodes = this.downNodes;
            }
            else
            {
                throw new Exception("Node doesn't exist in route.");
            }

            int index = sourceNodes.IndexOf(node);
            nodes.AddRange(sourceNodes.GetRange(index + 1, sourceNodes.Count - (index + 1)));

            return nodes;
        }

        /// <summary>
        /// Get a list of nodes for the route in the specified direction.
        /// </summary>
        /// <param name="isUpDirection">
        /// A value determining the direction of the route to return nodes for.
        /// </param>
        /// <returns>
        /// A list of <see cref="INetworkNode"/> objects.
        /// </returns>
        public List<INetworkNode> GetNodes(bool isUpDirection)
        {
            var nodes = new List<INetworkNode>();

            if (isUpDirection)
            {
                // nodes.Add(downDestination);
                nodes.AddRange(this.upNodes);

                // nodes.Add(upDestination);
                // nodes.Reverse();
                return nodes;
            }

            // nodes.Add(upDestination);
            nodes.AddRange(this.downNodes);

            // nodes.Add(downDestination);
            // nodes.Reverse();
            return nodes;
        }

        #endregion

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}