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
        private Caching.ArcCache aCache = new Caching.ArcCache("RoutePlanner");
        
        /// <summary>
        /// This event fires on every iteration of the algorithm.
        /// </summary>
        public event EventHandler<NextIterationEventArgs> NextIterationEvent;

        //public event EventHandler

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

        private bool hasDuplicates(INetworkNode current, INetworkNode next)
        {
            if (current != null)
            {
                while (current.Parent != null)
                {
                    if (current.ID == next.ID)
                    {
                        return true;
                    }
                    current = current.Parent;
                }
            }
            return false;
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
            DateTime currentTime = DateTime.Parse("11/30/2011 11:37 AM");
            INetworkNode current = null;
            while (stack.Count > 0)
            {

                
                INetworkNode next = stack.Pop();
                while (hasDuplicates(current,next))
                {
                    next = stack.Pop();
                }
                current = next;
                if (current == itinerary.Last())
                {
                    throw new Exception("Goal found!");
                }
                current.RetrieveData();
                if (NextIterationEvent != null)
                {
                    NextIterationEvent(this, new NextIterationEventArgs(current));
                }

                foreach (INetworkDataProvider nProvider in networkProviders)
                {
                    List<INetworkNode> nodes = nProvider.GetNodesAtLocation((Location)current, 1);
                    nodes.Add(itinerary.Last());

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
                            foreach (IPointDataProvider  pProvider in pointDataProviders)
                            {
                                
                                localArcs.Add(pProvider.EstimateDistance((Location)current, (Location)node));
                                if (node == null)
                                {
                                    throw new Exception("Destination node is null!");
                                }
                            }
                            List<Arc> cachedArcs = aCache.GetArcs((Location)current, (Location)node, currentTime + current.TotalTime, node.CurrentRoute);
                            if (cachedArcs.Count > 0  && cachedArcs[0].TransportMode == "Unavailable")
                            {
                                cachedArcs.Clear();
                            }                             
                            else
                            {
                                if (current.GetType() == nProvider.GetAssociatedType() &&
                                    node.GetType() == nProvider.GetAssociatedType())
                                {
                                    cachedArcs.AddRange(nProvider.GetDistanceBetweenNodes(current, node, currentTime + current.TotalTime));
                                    if (cachedArcs.Count == 0)
                                    {

                                        aCache.AddCacheEntry(currentTime + current.TotalTime,
                                            new Arc(
                                                (Location)current,
                                                (Location)node,
                                                new TimeSpan(999, 999, 999),
                                                99999999.9999,
                                                currentTime + current.TotalTime,
                                                "Unavailable", node.CurrentRoute));

                                    }
                                    else
                                    {
                                        foreach (Arc arc in cachedArcs)
                                        {
                                            aCache.AddCacheEntry(currentTime + current.TotalTime, arc);
                                            if (arc.Destination == null)
                                            {
                                                throw new Exception("Destination node is null!");
                                            }
                                        }
                                    }
                                }
                            }

                            if (cachedArcs.Count > 0)                                
                            {
                               // List<Arc> newCache = new List<Arc>();
                                for (int i = 0; i <cachedArcs.Count; i ++)                                    
                                {
                                    Arc arc = cachedArcs[i];
                                    INetworkNode source = nProvider.GetNodeAtLocation(arc.Source);
                                    INetworkNode destination = nProvider.GetNodeAtLocation(arc.Destination);
                                    if (destination == null)
                                    {
                                        //throw new Exception("Destination node is null!");
                                    }
                                    else
                                    {
                                        cachedArcs[i] = new Arc((Location)source, (Location)destination, arc.Time, arc.Distance, arc.DepartureTime, arc.TransportMode, arc.RouteId);
                                    }
                                   

                                }
                                //nProvider.get


                            }


                            localArcs.AddRange(cachedArcs);
                            

                            node.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)node, (Location)itinerary.Last());
                            
                        }
                    }

                    localArcs.Sort(new Comparers.ArcComparer());
                    List<INetworkNode> newNodes = new List<INetworkNode>();
                    foreach (Arc arc in localArcs)
                    {
                        try
                        {
                            INetworkNode destination = (INetworkNode)arc.Destination;
                            destination.Parent = current;
                            destination.TotalTime = current.TotalTime + arc.Time;
                            destination.CurrentRoute = arc.RouteId;
                            destination.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)destination, (Location)itinerary.Last());
                            if (!newNodes.Contains(destination))
                            {
                                newNodes.Add(destination);
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    localArcs.Clear();

                    newNodes.Sort(new Comparers.NodeComparer());
                    
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
