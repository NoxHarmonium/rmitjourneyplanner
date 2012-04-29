// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="ALRouteGenerator.cs">
//   Sean Dawson
// </copyright>
// <summary>
//   The al route generator.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.RouteGenerators
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using RmitJourneyPlanner.CoreLibraries.Comparers;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.Logging;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// The al route generator.
    /// </summary>
    public class BdalRouteGenerator : IRouteGenerator
    {
        #region Constants and Fields

        /// <summary>
        ///   The properties.
        /// </summary>
        private readonly EvolutionaryProperties properties;

        /// <summary>
        ///   The random.
        /// </summary>
        private readonly Random random = new Random();

        /// <summary>
        ///   The destination nodes.
        /// </summary>
        private List<MetlinkNode> destinationNodes;

        /// <summary>
        ///   The origin nodes.
        /// </summary>
        private List<INetworkNode> originNodes;

        /// <summary>
        ///   The stored destination.
        /// </summary>
        private INetworkNode storedDestination;

        /// <summary>
        ///   The stored origin.
        /// </summary>
        private INetworkNode storedOrigin;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlRouteGenerator"/> class.
        /// </summary>
        /// <param name="properties">
        /// The properties. 
        /// </param>
        public BdalRouteGenerator(EvolutionaryProperties properties)
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
        public Route Generate(INetworkNode source, INetworkNode destination, DateTime startTime)
        {
            if (source == destination)
            {
                Route r = new Route(-1);
                r.Add(source);
                return r;
            }
            this.Initialize(source, destination);
            INetworkDataProvider provider = this.properties.NetworkDataProviders[0];

            Regenerate:

            var current = new[]
                {
                    (MetlinkNode)this.originNodes[this.random.Next(this.originNodes.Count - 1)], 
                    this.destinationNodes[this.random.Next(this.destinationNodes.Count - 1)]
                };

            var next = new[] { current[0], current[1] };

            var routeDict = new Dictionary<MetlinkNode, int>();
            var route = new[] { new Route(-1), new Route(-1) };

            
            

            while (current[0] != current[1])
            {
                // Advance pointer
                current[0] = next[0];
                current[1] = next[1];

                next[0] = default(MetlinkNode);
                next[1] = default(MetlinkNode);

                if (current[0].Id != 0)
                {
                    INetworkNode newNode = provider.GetNodeFromId(current[0].Id);
                    route[0].Add(newNode);
                }

                if (current[1].Id != 0)
                {
                    INetworkNode newNode = provider.GetNodeFromId(current[1].Id);
                    route[1].Add(newNode);
                }

                // Logger.Log(this,"-->-->Selecting node id: {0}", current.Id);
                var euclidianDistances = new[] { new Dictionary<int, double>(), new Dictionary<int, double>() };

                // Logger.Log(this,"-->-->-->Adjacent nodes:");
                var adjacent = new[] { provider.GetAdjacentNodes(current[0]), provider.GetAdjacentNodes(current[1]) };

                for (int i = 0; i < 2; i++)
                {
                    foreach (MetlinkNode node in adjacent[i])
                    {
                        if (node.Id != current[i].Id)
                        {
                            // && !routeDict.ContainsKey(node))
                            /*
                                bool cont= true;
                                foreach (var routeID in routeMap[node.Id])
                                {
                                    if (routesVisited.ContainsKey(routeID))
                                    {
                                        cont = false;
                                    }
                                }
                                if (cont)
                                {
                                    continue;
                                }
                                */
                            var location = new Location(node.Latitude, node.Longitude);
                            double euclidianDistance = GeometryHelper.GetStraightLineDistance(location, current[1 - i]);
                            euclidianDistances[i][node.Id] = euclidianDistance;

                            /*Console.WriteLine(
                                    "-->-->--> [id: {0}] Location: {1} Type: {2} ED: {3} [{4} s]",
                                    node.Id,
                                    location,
                                    node.TransportType,
                                    euclidianDistance.ToString("F"),
                                    stopwatch.ElapsedMilliseconds / 1000.0);*/
                        }
                    }

                    double p = this.random.NextDouble();
                    if (p < this.properties.ProbMinDistance)
                    {
                        p = this.random.NextDouble();

                        bool transfer = p < this.properties.ProbMinTransfers;

                        int minNode = -1;

                        double minDist = double.MaxValue;
                        foreach (var kvp in euclidianDistances[i])
                        {
                            bool isTransfer = !provider.RoutesIntersect(provider.GetNodeFromId(kvp.Key), current[i]);

                            if (kvp.Value < minDist && transfer || (!transfer && !isTransfer))
                            {
                                minDist = kvp.Value;
                                minNode = kvp.Key;
                            }
                        }

                        if (minNode != -1)
                        {
                            next[i] = (MetlinkNode)provider.GetNodeFromId(minNode);
                        }
                    }
                    else
                    {
                        while (adjacent[i].Count > 1 && next[i] == default(MetlinkNode))
                        {
                            int index = this.random.Next(adjacent[i].Count - 1);
                            var nextMetlinkNode = (MetlinkNode)adjacent[i].ElementAt(index + 1);
                            if (routeDict.ContainsKey(nextMetlinkNode))
                            {
                                adjacent[i].Remove(nextMetlinkNode);
                            }
                            else
                            {
                                next[i] = nextMetlinkNode;
                            }
                        }
                    }

                    if (next[i] == default(MetlinkNode))
                    {

                        MetlinkNode nextNode;
                        int counter = 1;
                        if (adjacent[i].Count != 1)
                        {
                            
                            do
                            {
                                nextNode = (MetlinkNode)adjacent[i][counter++];
                            }
                            while (routeDict.ContainsKey(nextNode) && counter < adjacent[i].Count - 1);
                        }
                        else
                        {
                            nextNode = (MetlinkNode)adjacent[i][0];
                        }
                        if (counter == adjacent[i].Count)
                        {
                            next[i] = default(MetlinkNode);
                        }
                        else
                        {
                            next[i] = nextNode; // ?? this.destinationNodes[0];
                        }

                        
                    }

                    if (next[i] == default(MetlinkNode))
                    {
                        Console.WriteLine("Regenerating, no possible paths.");
                        goto Regenerate;
                        throw new Exception("Next node is null.");
                    }
                    if (routeDict.ContainsKey(current[i]))
                    {
                        routeDict.Add(current[i], 0);
                    }
                    else
                    {
                     //   Logger.Log(this,"Warning: Duplicate node added...");
                    }

                }
            }

            return Route.Glue(route[0], route[1]);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        private void Initialize(INetworkNode source, INetworkNode destination)
        {
            if (this.storedOrigin == null || this.storedDestination == null || this.storedOrigin.Id != source.Id
                || this.storedDestination.Id != destination.Id)
            {
                this.storedOrigin = source;
                this.storedDestination = destination;

                if (!(source is MetlinkNode))
                {
                    this.originNodes = new List<INetworkNode>();
                    double walkingDistance = 0;

                    //Logger.Log(this, "-->--> Finding source nodes... [{0} s]");

                    while (this.originNodes.Count == 0)
                    {
                        walkingDistance += 0.1;
                        this.originNodes =
                            this.properties.NetworkDataProviders[0].GetNodesAtLocation(
                                (Location)this.properties.Origin, walkingDistance);
                    }

                    //Console.WriteLine(
                    //    "-->--> Found {0} nodes to walk to! (Radius: {1}) [{2} s]", 
                    //    this.originNodes.Count, 
                    //    walkingDistance, 
                    //    0);

                    foreach (INetworkNode networkNode in this.originNodes)
                    {
                        networkNode.EuclidianDistance = GeometryHelper.GetStraightLineDistance(
                            (Location)networkNode, (Location)this.properties.Destination);
                        //Console.WriteLine(
                        //    "-->-->--> [id: {0}] Location: {1} Type: {2} ED: {3} [{4} s]", 
                        //    networkNode.Id, 
                        //    (Location)networkNode, 
                        //    networkNode.TransportType, 
                        //    networkNode.EuclidianDistance.ToString("F"), 
                         //   0);
                    }
                }
                else
                {
                    this.originNodes = new List<INetworkNode> { this.storedOrigin };
                }

                if (!(destination is MetlinkNode))
                {
                    //Console.WriteLine("\n-->--> Finding destination nodes... [{0} s]");
                    this.destinationNodes = new List<MetlinkNode>();
                    double walkingDistance = 0.0;
                    while (this.destinationNodes.Count == 0)
                    {
                        walkingDistance += 0.1;
                        List<INetworkNode> mNodes =
                            this.properties.NetworkDataProviders[0].GetNodesAtLocation(
                                (Location)this.properties.Destination, walkingDistance);
                        foreach (INetworkNode mNode in mNodes)
                        {
                            this.destinationNodes.Add((MetlinkNode)mNode);
                        }
                    }

                    //Console.WriteLine(
                        //"-->--> Found {0} nodes to walk to! (Radius: {1}) [{2} s]", 
                        //this.originNodes.Count, 
                        //walkingDistance, 
                       // 0);
                    foreach (MetlinkNode networkNode in this.destinationNodes)
                    {
                        var loc = new Location(networkNode.Latitude, networkNode.Longitude);
                        double euclidianDistance = GeometryHelper.GetStraightLineDistance(
                            loc, (Location)this.properties.Destination);

                        //Console.WriteLine(
                          //  "-->-->--> [id: {0}] Location: {1} Type: {2} ED: {3} [{4} s]", 
                          //  networkNode.Id, 
                          //  loc, 
                          //  networkNode.TransportType, 
                          //  euclidianDistance.ToString("F"), 
                           // 0);
                    }

                    //Console.WriteLine(
                       // "-->--> Found {0} nodes to walk to! (Radius: {1}) [{2} s]", 
                        //this.originNodes.Count, 
                       // walkingDistance, 
                       // 0);
                    foreach (INetworkNode networkNode in this.originNodes)
                    {
                        networkNode.EuclidianDistance = GeometryHelper.GetStraightLineDistance(
                            (Location)networkNode, (Location)this.properties.Destination);
                        //Console.WriteLine(
                           // "-->-->--> [id: {0}] Location: {1} Type: {2} ED: {3} [{4} s]", 
                            //networkNode.Id, 
                           // (Location)networkNode, 
                            //networkNode.TransportType, 
                            //networkNode.EuclidianDistance.ToString("F"), 
                           // 0);
                    }

                    this.originNodes.Sort(new NodeComparer());
                }
                else
                {
                    this.destinationNodes = new List<MetlinkNode> { (MetlinkNode)this.storedDestination };
                }
            }
        }

        #endregion
    }
}