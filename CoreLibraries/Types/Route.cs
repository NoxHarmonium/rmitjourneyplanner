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
        /// <param name="routeId">The ID of the specified route.</param>
        /// <param name="upDestination">The end point in the up direction.</param>
        /// <param name="downDestination">The end point in the down direction.</param>
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
            List<INetworkNode> nodes = new List<INetworkNode>();

            //Huh? Fix directions!
            if (!isUpDirection)
            {
                //nodes.Add(downDestination);
                nodes.AddRange(upNodes);
                //nodes.Add(upDestination);
                //nodes.Reverse();
                return nodes;
            }
            else
            {
                //nodes.Add(upDestination);
                nodes.AddRange(downNodes);
                //nodes.Add(downDestination);
                //nodes.Reverse();
                return nodes;
            }
        }

        /// <summary>
        /// Get the 2 adjacent nodes (one node up and one node down).
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<INetworkNode> GetAdjacent(INetworkNode node)
        {
            List<INetworkNode> sourceNodes;
            List<INetworkNode> nodes = new List<INetworkNode>();

            if (upNodes.Contains(node))
            {
                sourceNodes = upNodes;
            }
            else if (downNodes.Contains(node))
            {
                sourceNodes = downNodes;
            }
            else
            {
                throw new Exception("Node doesn't exist in route.");
            }


            int index = sourceNodes.IndexOf(node);
            nodes.AddRange(sourceNodes.GetRange(index+1,sourceNodes.Count-(index+1)));
            
           
            return nodes;
        }

        /// <summary>
        /// Gets the route ID.
        /// </summary>
        public string ID
        {
            get
            {
                return id;
            }
        }



    }
}
