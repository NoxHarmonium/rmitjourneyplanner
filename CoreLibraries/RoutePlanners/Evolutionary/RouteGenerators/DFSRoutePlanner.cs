// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DFSRoutePlanner.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   The search type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.RouteGenerators
{
    #region Using Directives

    using System;
    using System.Linq;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// The search type.
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        ///   The r w_ standard.
        /// </summary>
        RW_Standard, 

        /// <summary>
        ///   The r w_ bi dir.
        /// </summary>
        RW_BiDir, 

        /// <summary>
        ///   The df s_ standard.
        /// </summary>
        DFS_Standard, 

        /// <summary>
        ///   The df s_ bi dir.
        /// </summary>
        DFS_BiDir, 

        /// <summary>
        ///   The greedy_ standard.
        /// </summary>
        Greedy_Standard, 

        /// <summary>
        ///   The greedy_ bi dir.
        /// </summary>
        Greedy_BiDir, 

        /// <summary>
        ///   The a_ star_ standard.
        /// </summary>
        A_Star_Standard, 

        /// <summary>
        ///   The a_ star_ bi dir.
        /// </summary>
        A_Star_BiDir
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DFSRoutePlanner : IRouteGenerator
    {
        #region Constants and Fields

        /// <summary>
        ///   The properties.
        /// </summary>
        private readonly EvolutionaryProperties properties;

        /// <summary>
        ///   The search type.
        /// </summary>
        private SearchType searchType;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DFSRoutePlanner"/> class.
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        public DFSRoutePlanner(EvolutionaryProperties properties)
        {
            this.properties = properties;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The generate.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="startTime">
        /// The start time.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public Route Generate(INetworkNode source, INetworkNode destination, DateTime startTime)
        {
            this.searchType = this.properties.SearchType;
            if (source.Id == -1)
            {
                source = this.properties.NetworkDataProviders[0].GetNodeClosestToPointWithinArea(
                    source, source, 1.0, true);
            }

            if (destination.Id == -1)
            {
                destination = this.properties.NetworkDataProviders[0].GetNodeClosestToPointWithinArea(
                    destination, destination, 1.0, true);
            }

            PTDepthFirstSearch searchAlgorithm;

            switch (this.searchType)
            {
                case SearchType.DFS_Standard:
                    searchAlgorithm = new PTDepthFirstSearch(
                        false, this.properties.NetworkDataProviders[0], source, destination);
                    break;
                case SearchType.DFS_BiDir:
                    searchAlgorithm = new PTDepthFirstSearch(
                        true, this.properties.NetworkDataProviders[0], source, destination);
                    break;
                case SearchType.Greedy_Standard:
                    searchAlgorithm = new PTGreedySearch(
                        false, this.properties.NetworkDataProviders[0], source, destination);
                    break;
                case SearchType.Greedy_BiDir:
                    searchAlgorithm = new PTGreedySearch(
                        true, this.properties.NetworkDataProviders[0], source, destination);
                    break;
                case SearchType.A_Star_Standard:
                    searchAlgorithm = new PTAStarSearch(
                        false, this.properties.NetworkDataProviders[0], source, destination);
                    break;
                case SearchType.A_Star_BiDir:
                    searchAlgorithm = new PTAStarSearch(
                        true, this.properties.NetworkDataProviders[0], source, destination);
                    break;
                case SearchType.RW_Standard:
                    searchAlgorithm = new PTDepthFirstSearch(
                        false, this.properties.NetworkDataProviders[0], source, destination) {
                                                                                                UseVisited = false 
                                                                                             };
                    break;
                case SearchType.RW_BiDir:
                    searchAlgorithm = new PTDepthFirstSearch(
                        true, this.properties.NetworkDataProviders[0], source, destination) {
                                                                                               UseVisited = false 
                                                                                            };
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            // PTDepthFirstSearch searchAlgorithm = new PTDepthFirstSearch(properties.Bidirectional,properties.NetworkDataProviders[0],source,destination);
            INetworkNode[] nodes = searchAlgorithm.Run();

            // if (nodes.First() != destination || nodes.Last() != source)
            // {
            // throw new Exception("Path is invalid!");
            // }
            // searchAlgorithm.Entropy = 0.0;
            switch (this.searchType)
            {
                case SearchType.DFS_Standard:
                    nodes = nodes.Reverse().ToArray();
                    break;
                case SearchType.Greedy_Standard:
                    nodes = nodes.Reverse().ToArray();
                    break;
                case SearchType.A_Star_Standard:
                    nodes = nodes.Reverse().ToArray();
                    break;
            }

            // if (!properties.Bidirectional)
            // {

            // }

            // Check for duplicate nodes
            // foreach (var networkNode in nodes)
            // {
            // INetworkNode node = networkNode;
            // var instances = from n in nodes where n.Id == node.Id select n;
            // Assert.True(instances.Count() == 1);
            // }
            return new Route(-1, nodes);
        }

        #endregion
    }
}