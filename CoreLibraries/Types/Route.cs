// -----------------------------------------------------------------------
// <copyright file="Route.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DataProviders;
    /// <summary>
    /// Represents a route made up of INetworkNodes.
    /// </summary>
    public class Route
    {
        private List<INetworkNode> upNodes = new List<INetworkNode>();
        private List<INetworkNode> downNodes = new List<INetworkNode>();
        private INetworkNode upDestination;
        private INetworkNode downDestination;
        private string id;


        /// <summary>
        /// Initializes a new instance of the the Route class.
        /// </summary>
        /// <param name="UpDestination">The end point in the up direction.</param>
        /// <param name="DownDestination">The end point in the down direction.</param>
        public Route(string routeId, INetworkNode upDestination, INetworkNode downDestination)
        {
            this.upDestination = upDestination;
            this.downDestination = downDestination;
            this.id = routeId;
        }

       


        /// <summary>
        /// Adds a node to the specfied route direction.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="isUpDirection"></param>
        public void AddNode(INetworkNode node,bool isUpDirection)
        {
            if (isUpDirection)
            {
                upNodes.Add(node);
            }
            else
            {
                downNodes.Add(node);
            }
        }

        /// <summary>
        /// Get a list of nodes for the route in the specified direction.
        /// </summary>
        /// <param name="isUpDirection"></param>
        /// <returns></returns>
        public List<INetworkNode> GetNodes(bool isUpDirection)
        {

            if (isUpDirection)
            {
                return upNodes;
            }
            else
            {
                return downNodes;
            }
        }

        /// <summary>
        /// Get the 2 adjacent nodes (one node up and one node down).
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<INetworkNode> GetAdjacent(INetworkNode node)
        {
            List<INetworkNode> nodes = new List<INetworkNode>();
            if (node == upDestination)
            {
                nodes.Add(downNodes[0]);
                nodes.Add(upNodes[upNodes.Count - 1]);
            }
            else if (node == downDestination)
            {
                nodes.Add(upNodes[0]);
                nodes.Add(downNodes[downNodes.Count - 1]);
            }
            else
            {
                int upIndex = upNodes.IndexOf(node) + 1;
                if (upIndex > upNodes.Count - 1)
                {
                    nodes.Add(upDestination);
                }
                else
                {
                    nodes.Add(upNodes[upIndex + 1]);
                }
                

                int downIndex = downNodes.IndexOf(node);
                if (downIndex > downNodes.Count - 1)
                {
                    nodes.Add(downDestination);
                }
                else
                {
                    nodes.Add(downNodes[downIndex + 1]);
                }
            }
            return nodes;
        }

        public string ID
        {
            get
            {
                return id;
            }
        }



    }
}
