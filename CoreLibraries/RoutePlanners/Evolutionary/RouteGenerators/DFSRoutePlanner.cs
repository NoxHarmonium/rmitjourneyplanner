// -----------------------------------------------------------------------
// <copyright file="DFSRoutePlanner.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.RouteGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
    using RmitJourneyPlanner.CoreLibraries.Types;

    public enum SearchType
    {
        DFS_Standard,
        DFS_BiDir,
        Greedy_Standard,
        Greedy_BiDir,
        A_Star_Standard,
        A_Star_BiDir
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DFSRoutePlanner : IRouteGenerator 
    {


        private readonly EvolutionaryProperties properties;

        private readonly SearchType searchType;
  

        public DFSRoutePlanner(EvolutionaryProperties properties)
        {
            this.properties = properties;
            this.searchType = properties.SearchType;
        }
        
        public Route Generate(INetworkNode source, INetworkNode destination, DateTime startTime)
        {
            if (source.Id == -1)
            {
                source = properties.NetworkDataProviders[0].GetNodeClosestToPointWithinArea(source, source, 1.0, true);
            }
            if (destination.Id == -1)
            {
                destination = properties.NetworkDataProviders[0].GetNodeClosestToPointWithinArea(destination, destination, 1.0, true);
            }

            PTDepthFirstSearch searchAlgorithm;
            
            switch (searchType)
            {
                case SearchType.DFS_Standard:
                    searchAlgorithm = new PTDepthFirstSearch(false, properties.NetworkDataProviders[0], source, destination);
                    break;
                case SearchType.DFS_BiDir:
                    searchAlgorithm = new PTDepthFirstSearch(true, properties.NetworkDataProviders[0], source, destination);
                    break;
                case SearchType.Greedy_Standard:
                    searchAlgorithm = new PTGreedySearch(false, properties.NetworkDataProviders[0], source, destination);
                    break;
                case SearchType.Greedy_BiDir:
                    searchAlgorithm = new PTGreedySearch(true, properties.NetworkDataProviders[0], source, destination);
                    break;
                case SearchType.A_Star_Standard:
                    searchAlgorithm = new PTAStarSearch(false, properties.NetworkDataProviders[0], source, destination);
                    break;
                case SearchType.A_Star_BiDir:
                    searchAlgorithm = new PTAStarSearch(true, properties.NetworkDataProviders[0], source, destination);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            //PTDepthFirstSearch searchAlgorithm = new PTDepthFirstSearch(properties.Bidirectional,properties.NetworkDataProviders[0],source,destination);
            INetworkNode[] nodes = searchAlgorithm.Run();
            //if (nodes.First() != destination || nodes.Last() != source)
            //{
            //    throw new Exception("Path is invalid!");
            //}
           // searchAlgorithm.Entropy = 0.0;


            switch (searchType)
            {
                case SearchType.DFS_Standard:
                    nodes = nodes.Reverse().ToArray();
                    break;
                case SearchType.Greedy_Standard:
                    nodes = nodes.Reverse().ToArray();
                    break;
                    
            }
            //if (!properties.Bidirectional)
            //{
                
            //}
			
			
			//Check for duplicate nodes
            //foreach (var networkNode in nodes)
            //{
            //    INetworkNode node = networkNode;
            //    var instances = from n in nodes where n.Id == node.Id select n;
            //    Assert.True(instances.Count() == 1);
            //}

                return new Route(-1, nodes);
        }
    }
}
