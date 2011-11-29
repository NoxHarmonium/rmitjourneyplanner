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
        
        public event EventHandler<NextIterationEventArgs> NextIterationEvent;

        public void RegisterNetworkDataProvider(DataProviders.INetworkDataProvider provider)
        {
            networkProviders.Add(provider);
        }

        public void RegisterPointDataProvider(DataProviders.IPointDataProvider provider)
        {
            pointDataProviders.Add(provider);
        }

        

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

                    List<string> routes = nProvider.GetRoutesForNode(current);

                    foreach (string route in routes)
                    {
                        List<INetworkNode> adjacent = nProvider.GetAdjacentNodes(current, route);
                        nodes.AddRange(adjacent);
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

                    localArcs.Sort(new ArcComparer());
                    foreach (Arc arc in localArcs)
                    {
                        stack.Push((INetworkNode)arc.Destination);
                    }
                    localArcs.Clear();
                    
                }

            }

            throw new NotImplementedException();
        }
    }
}
