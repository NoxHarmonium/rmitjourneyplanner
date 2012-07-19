﻿// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    #region

    using System;

    using RmitJourneyPlanner.CoreLibraries.Positioning;

    #endregion

    /// <summary>
    ///   A special class that is used to define terminal points. Used to make mixing nodes and location points easier, basically a wrapper for a Location class.
    /// </summary>
    public class TerminalNode : Location, INetworkNode
    {
        #region Constants and Fields

        /// <summary>
        ///   The unique identifier of the node.
        /// </summary>
        private readonly int id = -1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="TerminalNode" /> class using a latitude and longitude.
        /// </summary>
        /// <param name="id"> A unique identifier for this node. </param>
        /// <param name="latitude"> The latitude value of the this node. </param>
        /// <param name="longitude"> The longitude value of this node. </param>
        public TerminalNode(int id, double latitude, double longitude)
            : base(latitude, longitude)
        {
            this.id = id;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TerminalNode" /> class using a location string which is interpreted by the Google Maps API.
        /// </summary>
        /// <param name="id"> A unique identifier for this node. </param>
        /// <param name="locationString"> The location string that will be parsed by the Google Maps API to get a latitude and longitude. </param>
        public TerminalNode(int id, string locationString)
            : base(locationString)
        {
            this.id = id;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the current route of a terminal node which is always an empty string.
        /// </summary>
        public int RouteId
        {
            get
            {
                return -1;
            }

            set
            {
            }
        }

        /// <summary>
        ///   Gets or sets the Euclidian distance to the goal. Used for traversing route trees.
        /// </summary>
        public double EuclidianDistance { get; set; }

        /// <summary>
        ///   Gets the user-defined identifier of this terminal node.
        /// </summary>
        public int Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        ///   Gets or sets the parent of this node, used for tree traversal.
        /// </summary>
        public INetworkNode Parent { get; set; }

        /// <summary>
        ///   Gets the parent of the terminal node. This is always null.
        /// </summary>
        public INetworkDataProvider Provider
        {
            get
            {
                return null;
            }
        }

        

        /// <summary>
        ///   Gets or sets the total time taken to get to the terminal node.
        /// </summary>
        public TimeSpan TotalTime { get; set; }

        /// <summary>
        ///   Gets or sets the type of transport this node services.
        /// </summary>
        public string TransportType { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Creates a new terminal node with the same properties.
        /// </summary>
        /// <returns> The clone. </returns>
        public object Clone()
        {
            var newNode = new TerminalNode(this.id, this.Latitude, this.Longitude) { TotalTime = this.TotalTime };
            return newNode;
        }

        /// <summary>
        ///   Returns if 2 terminal nodes are equal.
        /// </summary>
        /// <param name="obj"> The object to compare this node to. </param>
        /// <returns> The equals. </returns>
        public override bool Equals(object obj)
        {
            var networkNode = obj as INetworkNode;
            if (networkNode != null)
            {
                if ((networkNode).Id == this.id)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///   Determines if this node is equal to another.
        /// </summary>
        /// <param name="other"> The node to compare this node to. </param>
        /// <returns> The equals. </returns>
        public bool Equals(INetworkNode other)
        {
            return this.Equals((object)other);
        }

        /// <summary>
        ///   Gets the unique identifier for a terminal node.
        /// </summary>
        /// <returns> The get hash code. </returns>
        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }

        /// <summary>
        ///   This method does nothing in a terminal node.
        /// </summary>
        public void RetrieveData()
        {
        }

        #endregion

        Types.INode Types.INode.Parent
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}