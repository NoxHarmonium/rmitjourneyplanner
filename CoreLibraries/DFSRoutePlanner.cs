// -----------------------------------------------------------------------
// <copyright file="DFSRoutePlanner.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DataProviders;
    using Types;
    using Positioning;

    /// <summary>
    /// Finds the best route between given nodes using a depth first search.
    /// </summary>
    public class DFSRoutePlanner : IRoutePlanner
    {
        private List<INetworkDataProvider> networkProviders = new List<INetworkDataProvider>();
        private List<IPointDataProvider> pointDataProviders = new List<IPointDataProvider>();
        
        /// <summary>
        /// This event fires on every iteration of the algorithm.
        /// </summary>
        public event EventHandler<NextIterationEventArgs> NextIterationEvent;

        /// <summary>
        /// Register a new data provider to use in the travel planning.
        /// </summary>
        /// <param name="provider"></param>
        public void RegisterNetworkDataProvider(DataProviders.INetworkDataProvider provider)
        {
            networkProviders.Add(provider);
        }


        /// <summary>
        /// Register a new point to point data provider to use in travel planning.
        /// </summary>
        /// <param name="provider"></param>
        public void RegisterPointDataProvider(DataProviders.IPointDataProvider provider)
        {
            pointDataProviders.Add(provider);
        }

        
        /// <summary>
        /// Return the best route found between the point
        /// </summary>
        /// <param name="itinerary"></param>
        /// <returns></returns>
        public List<Types.Arc>[] Solve(List<DataProviders.INetworkNode> itinerary)
        {

            Stack<INetworkNode> stack = new Stack<INetworkNode>();
            stack.Push(itinerary.First());
            DateTime currentTime = DateTime.Now;
            while (stack.Count > 0)
            {
                INetworkNode current = stack.Pop();
                current.RetrieveData();
                if (NextIterationEvent != null)
                {
                    NextIterationEvent(this, new NextIterationEventArgs(current));
                }

                foreach (INetworkDataProvider nProvider in networkProviders)
                {
                    List<INetworkNode> nodes = nProvider.GetNodesAtLocation((Location)current, 1);

                    if (nProvider.GetAssociatedType() == current.GetType())
                    {
                        List<string> routes = nProvider.GetRoutesForNode(current);

                        foreach (string route in routes)
                        {
                            List<INetworkNode> adjacent = nProvider.GetAdjacentNodes(current, route);
                            nodes.AddRange(adjacent);
                        }
                    }

                    List<Arc> localArcs = new List<Arc>();
                    foreach (INetworkNode node in nodes)
                    {
                        if (!node.Equals(current))
                        {
                            
                                                      
                            node.RetrieveData();
                            foreach (IPointDataProvider pProvider in pointDataProviders)
                            {
                                localArcs.Add(pProvider.GetDistance((Location)current, (Location)node));
                            }
                            localArcs.AddRange(nProvider.GetDistanceBetweenNodes(current, node, DateTime.Now));
                            
                        }
                    }

                    localArcs.Sort(new Comparers.ArcComparer());
                    List<INetworkNode> newNodes = new List<INetworkNode>();
                    foreach (Arc arc in localArcs)
                    {
                        INetworkNode destination = (INetworkNode)arc.Destination;
                        destination.Parent = current;
                        destination.TotalTime = current.TotalTime + arc.Time;
                        newNodes.Add(destination);
                    }
                    localArcs.Clear();

                    newNodes.Sort(new Comparers.NodeTimeComparer());
                    foreach (INetworkNode node in newNodes)
                    {
                        stack.Push(node);
                    }
                    
                }

            }

            throw new NotImplementedException();
        }
    }
}
