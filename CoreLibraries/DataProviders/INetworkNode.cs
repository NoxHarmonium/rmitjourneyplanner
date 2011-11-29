// -----------------------------------------------------------------------
// <copyright file="INetworkNode.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a node in a transport network such a train station or tram stop.
    /// </summary>
    public interface INetworkNode : IEquatable<INetworkNode>
    {
        /// <summary>
        /// Gets the DataProvider that the node belongs to.
        /// </summary>
        INetworkDataProvider Provider
        {
            get;
        }

        /// <summary>
        /// Gets or sets the total time taken to reach this node. Used for traversing 
        /// route trees.
        /// </summary>
        public TimeSpan TotalTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parent node to this node. Used for traversing 
        /// route trees.
        /// </summary>
        public INetworkNode Parent
        {
            get;
            set;
        }

        /// <summary>
        /// Return a unique identifier for this node inside of it's network.
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// Loads the properties of the node from the parent.
        /// </summary>
        void RetrieveData();

        /// <summary>
        /// Gets the longitude of this location
        /// </summary>
        double Longitude
        {
            get;
        }

        /// <summary>
        /// Gets the latitude of this location
        /// </summary>
        double Latitude
        {
            get;
        }

        string CurrentRoute
        {
            get;
            set;
        }
    }
}
