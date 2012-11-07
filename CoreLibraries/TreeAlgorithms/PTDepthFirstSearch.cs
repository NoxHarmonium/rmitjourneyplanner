// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PTDepthFirstSearch.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.TreeAlgorithms
{
    #region Using Directives

    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    #endregion

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PTDepthFirstSearch : DepthFirstSearch<INetworkNode>
    {
        #region Constants and Fields

        /// <summary>
        ///   The provider.
        /// </summary>
        private readonly INetworkDataProvider provider;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PTDepthFirstSearch"/> class.
        /// </summary>
        /// <param name="depth">
        /// The depth.
        /// </param>
        /// <param name="bidirectional">
        /// The bidirectional.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="goal">
        /// The goal.
        /// </param>
        public PTDepthFirstSearch(
            int depth, bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode goal)
            : base(depth, bidirectional, origin, goal)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PTDepthFirstSearch"/> class.
        /// </summary>
        /// <param name="bidirectional">
        /// The bidirectional.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        public PTDepthFirstSearch(
            bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode destination)
            : base(bidirectional, origin, destination)
        {
            this.provider = provider;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get children.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// </returns>
        protected override NodeWrapper<INetworkNode>[] GetChildren(NodeWrapper<INetworkNode> node)
        {
            if (node.CurrentRoute >= 20000)
            {
                return new NodeWrapper<INetworkNode>[0];
            }

            List<INetworkNode> nodes = this.provider.GetAdjacentNodes(node.Node);

            var wrappers = new NodeWrapper<INetworkNode>[nodes.Count];

            for (int i = 0; i < nodes.Count; i++)
            {
                wrappers[i] = new NodeWrapper<INetworkNode>(nodes[i]) { CurrentRoute = node.CurrentRoute + 1 };
            }

            return wrappers;
        }

        /// <summary>
        /// The order children.
        /// </summary>
        /// <param name="nodes">
        /// The nodes.
        /// </param>
        /// <returns>
        /// </returns>
        protected override NodeWrapper<INetworkNode>[] OrderChildren(NodeWrapper<INetworkNode>[] nodes)
        {
            var oldNodes = new List<NodeWrapper<INetworkNode>>(nodes);

            oldNodes.Shuffle();

            // nodes.StochasticSort(Entropy);

            // if (oldNodes.Any(node => !nodes.Contains(node)) || nodes.Any(node => !oldNodes.Contains(node)))
            // {
            // throw new Exception("Nodes are not consistant after sort.");
            // }
            return oldNodes.ToArray();
        }

        #endregion
    }
}