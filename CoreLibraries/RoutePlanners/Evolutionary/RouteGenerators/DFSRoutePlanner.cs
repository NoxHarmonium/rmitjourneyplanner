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

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
    using RmitJourneyPlanner.CoreLibraries.Types;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DFSRoutePlanner : IRouteGenerator 
    {


        private readonly EvolutionaryProperties properties;
  

        public DFSRoutePlanner(EvolutionaryProperties properties)
        {
            this.properties = properties;
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

            PTDepthFirstSearch searchAlgorithm = new PTGreedySearch(true, properties.NetworkDataProviders[0], source, destination);
            //PTDepthFirstSearch searchAlgorithm = new PTDepthFirstSearch(properties.Bidirectional,properties.NetworkDataProviders[0],source,destination);
            INetworkNode[] nodes = searchAlgorithm.Run();
            //if (nodes.First() != destination || nodes.Last() != source)
            //{
            //    throw new Exception("Path is invalid!");
            //}
           // searchAlgorithm.Entropy = 0.0;

         

            //if (!properties.Bidirectional)
            //{
                //nodes = nodes.Reverse().ToArray();
            //}

          

                return new Route(-1, nodes);
        }
    }
}
