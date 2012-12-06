// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PTAStarSearch.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.TreeAlgorithms
{
    #region Using Directives

    using System.Linq;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Ptv;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;

    #endregion

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PTAStarSearch : PTDepthFirstSearch
    {
        #region Constants and Fields

        /// <summary>
        ///   The properties.
        /// </summary>
        private readonly EvolutionaryProperties properties;

        /// <summary>
        ///   The provider.
        /// </summary>
        private readonly INetworkDataProvider provider;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PTAStarSearch"/> class.
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
        public PTAStarSearch(
            int depth, bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode goal)
            : base(depth, bidirectional, provider, origin, goal)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PTAStarSearch"/> class.
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
        public PTAStarSearch(
            bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode destination)
            : base(bidirectional, provider, origin, destination)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PTAStarSearch"/> class.
        /// </summary>
        /// <param name="properties">
        /// The properties.
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
        /// <param name="destination">
        /// The destination.
        /// </param>
        public PTAStarSearch(
            EvolutionaryProperties properties, 
            bool bidirectional, 
            INetworkDataProvider provider, 
            INetworkNode origin, 
            INetworkNode destination)
            : base(bidirectional, provider, origin, destination)
        {
            this.provider = provider;
            this.properties = properties;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the specified nodes sorted stochastically according to
        /// their Euclidian distance value with a penalty for node changes.
        /// </summary>
        /// <param name="nodes">
        /// The nodes to be sorted.
        /// </param>
        /// <returns>
        /// The nodes stochastically sorted.
        /// </returns>
        protected override NodeWrapper<INetworkNode>[] OrderChildren(NodeWrapper<INetworkNode>[] nodes)
        {
            // nodes = base.OrderChildren(nodes);
            var target = (Location)(this.CurrentIndex == 0 ? this.Destination : this.Origin);

            foreach (var wrapper in nodes)
            {
                wrapper.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)wrapper.Node, target);

                // wrapper.EuclidianDistance = 1;
                if (((PtvDataProvider)this.provider).RoutesIntersect(wrapper.Node, this.Current[0].Node))
                {
                    // wrapper.EuclidianDistance *= 0.5; //best
                    wrapper.EuclidianDistance *= 0.5;
                }
            }

            // Array.Sort(nodes, new NodeComparer());
            // Array.Reverse(nodes);
            nodes.StochasticSort(this.Entropy);

            return nodes.Reverse().ToArray();
        }

        #endregion
    }
}