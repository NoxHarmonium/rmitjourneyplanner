// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PTDepthFirstSearch.cs" company="RMIT University">
//   Copyright RMIT University 2012.
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
        /// Gets the child nodes for the specified <see cref="INetworkNode"/>.
        /// </summary>
        /// <param name="node">
        /// The node that you wish to find the children of.
        /// </param>
        /// <returns>
        /// A list of child nodes.
        /// </returns>
        protected override NodeWrapper<INetworkNode>[] GetChildren(NodeWrapper<INetworkNode> node)
        {
            List<INetworkNode> nodes = this.provider.GetAdjacentNodes(node.Node);

            var wrappers = new NodeWrapper<INetworkNode>[nodes.Count];

            for (int i = 0; i < nodes.Count; i++)
            {
                wrappers[i] = new NodeWrapper<INetworkNode>(nodes[i]);
            }

            return wrappers;
        }

        /// <summary>
        /// Returns the specified nodes stochastically sorted on no
        /// parameter (shuffled).
        /// </summary>
        /// <param name="nodes">
        /// The nodes to be sorted.
        /// </param>
        /// <returns>
        /// The nodes stochastically sorted.
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