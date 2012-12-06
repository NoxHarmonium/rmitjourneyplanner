// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PTGreedySearch.cs" company="RMIT University">
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
    using RmitJourneyPlanner.CoreLibraries.Positioning;

    #endregion

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PTGreedySearch : PTDepthFirstSearch
    {
        #region Constants and Fields

        /// <summary>
        ///   The provider.
        /// </summary>
        private readonly INetworkDataProvider provider;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PTGreedySearch"/> class.
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
        public PTGreedySearch(
            int depth, bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode goal)
            : base(depth, bidirectional, provider, origin, goal)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PTGreedySearch"/> class.
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
        public PTGreedySearch(
            bool bidirectional, INetworkDataProvider provider, INetworkNode origin, INetworkNode destination)
            : base(bidirectional, provider, origin, destination)
        {
            this.provider = provider;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the specified nodes sorted stochastically according to
        /// their Euclidian distance value only.
        /// </summary>
        /// <param name="nodes">
        /// The nodes to be sorted.
        /// </param>
        /// <returns>
        /// The nodes stochastically sorted.
        /// </returns>
        protected override NodeWrapper<INetworkNode>[] OrderChildren(NodeWrapper<INetworkNode>[] nodes)
        {
            var target = (Location)(this.CurrentIndex == 0 ? this.Destination : this.Origin);
            foreach (var wrapper in nodes)
            {
                /*
                if (Bidirectional)
                {
                    INetworkNode otherCurrent = this.current[CurrentIndex == 0 ? 1 : 0].Node;
                    /*
                    wrapper.EuclidianDistance = this.Bidirectional && otherCurrent != null
                                                         ? GeometryHelper.GetStraightLineDistance(
                                                             (Location)wrapper.Node, (Location)otherCurrent)
                                                         : GeometryHelper.GetStraightLineDistance(
                                                             (Location)wrapper.Node, (Location)this.Destination);
                     * 

                    
                }
                else
                {
                    wrapper.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)wrapper.Node, (Location)this.Destination);
                }

                //if (wrapper.Node.TransportType == "Train")
                //{
                //    wrapper.EuclidianDistance /= 2.0;
               // }
                 * 
                 **/
                wrapper.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)wrapper.Node, target);
            }

            // Array.Sort(nodes, new NodeComparer());
            nodes.StochasticSort(this.Entropy);

            return nodes.Reverse().ToArray();

            // return nodes.OrderBy(n => n.EuclidianDistance).Reverse().ToArray();
        }

        #endregion
    }
}