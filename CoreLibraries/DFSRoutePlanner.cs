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

        private Arc traverse(TreeWrapper<INetworkNode> node)
        {
            List<INetworkNode> nodes = new List<INetworkNode>();
            
            while (node.Parent != null)
            {
                nodes.Add(node.Obj);
                node = node.Parent;
            }
            nodes.Reverse();

            DateTime currentTime = DateTime.Parse("11/24/2011 1:23:00 PM");
            for (int i = 0; i < nodes.Count - 2; i++)
            {
                INetworkNode sNode = nodes[i];
                INetworkNode dNode = nodes[i+1];
                if (sNode is INetworkNode && dNode is INetworkNode)
                {
                    foreach (INetworkDataProvider nProvider in networkProviders)
                    {
                        List<Arc> arc = nProvider.GetDistanceBetweenNodes(sNode, dNode, currentTime);
                        //currentTime += arc.;
                    }
                }

            }


            return null;
        }

        public List<Types.Arc>[] Solve(List<DataProviders.INetworkNode> itinerary)
        {

            Stack<TreeWrapper<INetworkNode>> stack = new Stack<TreeWrapper<INetworkNode>>();
            stack.Push(new TreeWrapper<INetworkNode>(itinerary.First(),null));
            DateTime currentTime = DateTime.Now;
            while (stack.Count > 0)
            {
                TreeWrapper<INetworkNode> current = stack.Pop();
                current.Obj.RetrieveData();
                if (NextIterationEvent != null)
                {
                    NextIterationEvent(this, new NextIterationEventArgs(current.Obj));
                }

                foreach (INetworkDataProvider nProvider in networkProviders)
                {
                    List<INetworkNode> nodes = nProvider.GetNodesAtLocation((Location)current.Obj, 1);

                    List<string> routes = nProvider.GetRoutesForNode(current.Obj);

                    foreach (string route in routes)
                    {
                        List<INetworkNode> adjacent = nProvider.GetAdjacentNodes(current.Obj, route);
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
                                localArcs.Add(pProvider.GetDistance((Location)current.Obj, (Location)node));
                            }
                            localArcs.AddRange(nProvider.GetDistanceBetweenNodes(current.Obj, node, DateTime.Now));
                            
                        }
                    }

                    localArcs.Sort(new ArcComparer());
                    foreach (Arc arc in localArcs)
                    {
                        stack.Push(new TreeWrapper<INetworkNode>((INetworkNode)arc.Destination,current));
                    }
                    localArcs.Clear();
                    
                }

            }

            throw new NotImplementedException();
        }
    }
}
