// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="INetworkNode.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Represents a node in a transport network such a train station or tram stop.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    #region

    using System;

    #endregion

    /// <summary>
    /// Represents a node in a transport network such a train station or tram stop.
    /// </summary>
    public interface INetworkNode : IEquatable<INetworkNode>, ICloneable
    {
        #region Public Properties

        /// <summary>
        ///   Gets the route number without any modifiers such as direction.
        /// </summary>
        string BaseRoute { get; }

        /// <summary>
        ///   Gets or sets the current route that the node is traversing.
        /// </summary>
        string CurrentRoute { get; set; }

        /// <summary>
        ///   Gets or sets the Euclidian distance to the goal. Used for traversing route trees.
        /// </summary>
        double EuclidianDistance { get; set; }

        /// <summary>
        ///   Gets a unique identifier for this node inside of it's network.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///   Gets the latitude of this location
        /// </summary>
        double Latitude { get; }

        /// <summary>
        ///   Gets the longitude of this location
        /// </summary>
        double Longitude { get; }

        /// <summary>
        ///   Gets or sets the parent node to this node. Used for traversing 
        ///   route trees.
        /// </summary>
        INetworkNode Parent { get; set; }

        /// <summary>
        ///   Gets the DataProvider that the node belongs to.
        /// </summary>
        INetworkDataProvider Provider { get; }

        /// <summary>
        ///   Gets or sets the total time taken to reach this node. Used for traversing 
        ///   route trees.
        /// </summary>
        TimeSpan TotalTime { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads the properties of the node from the parent.
        /// </summary>
        void RetrieveData();

        #endregion
    }
}