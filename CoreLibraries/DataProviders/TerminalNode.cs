// -----------------------------------------------------------------------
// <copyright file="TerminalNode.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Positioning;
    using Types;
    using DataProviders;

    /// <summary>
    /// A special class that is used to define terminal points. 
    /// Used to make mixing nodes and location points easier, 
    /// basically a wrapper for a Location class.
    /// </summary>
    public class TerminalNode : Location, INetworkNode
    {
        private string id = String.Empty;

        /// <summary>
        /// Initilizes a new TerminalNode object with a specific location.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public TerminalNode(string id, double latitude, double longitude) : base (latitude,longitude)
        {
            this.id = id;
        }

        /// <summary>
        /// Initilizes a new TerminalNode object with a location string.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="locationString"></param>
        public TerminalNode(string id, string locationString) 
            : base (locationString)
        {
            this.id = id;
        }
        
        /// <summary>
        /// Get the parent of the terminal node. 
        /// This is always null.
        /// </summary>
        public INetworkDataProvider Provider
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Get the user-defined ID of this terminal node.
        /// </summary>
        public string ID
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// This method does nothing in a terminal node.
        /// </summary>
        public void RetrieveData()
        {
            
        }

        /// <summary>
        /// Returns if 2 terminal nodes are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is INetworkNode)
            {
                if (((INetworkNode)obj).ID == id)
                {
                    return true;
                }
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Gets the unique identifier for a terminal node.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        /// <summary>
        /// Determines if this node is equal to another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(INetworkNode other)
        {
            return this.Equals((object)other);
        }


        /// <summary>
        /// Gets the route ID of the terminal node which is always 
        /// an empty string.
        /// </summary>
        public string routeId
        {
            get
            {
                return String.Empty;
            }
        }


        /// <summary>
        /// Gets the current route of a terminal node which is always an empty string.
        /// </summary>
        public string CurrentRoute
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the total time taken to get to the terminal node.
        /// </summary>
        public TimeSpan TotalTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parent of this node, used for tree traversal.
        /// </summary>
        public INetworkNode Parent
        {
            get;
            set;
        }
    }
}
