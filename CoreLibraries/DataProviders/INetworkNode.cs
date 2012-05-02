// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    #region

    using System;

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   Represents a node in a transport network such a train station or tram stop.
    /// </summary>
    public interface INetworkNode : IEquatable<INetworkNode>, ICloneable, INode
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the current route that the node is traversing.
        /// </summary>
        int CurrentRoute { get; set; }

        /// <summary>
        ///   Gets or sets the Euclidian distance to the goal. Used for traversing route trees.
        /// </summary>
        double EuclidianDistance { get; set; }

        /// <summary>
        ///   Gets a unique identifier for this node inside of it's network.
        /// </summary>
        int Id { get; }

        /// <summary>
        ///   Gets the latitude of this location
        /// </summary>
        double Latitude { get; }

        /// <summary>
        ///   Gets the longitude of this location
        /// </summary>
        double Longitude { get; }

      

        /// <summary>
        ///   Gets the DataProvider that the node belongs to.
        /// </summary>
        INetworkDataProvider Provider { get; }

        /// <summary>
        ///   Gets or sets the total time taken to reach this node. Used for traversing route trees.
        /// </summary>
        TimeSpan TotalTime { get; set; }

        /// <summary>
        ///   Gets or sets the type of transport this node services.
        /// </summary>
        string TransportType { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Loads the properties of the node from the parent.
        /// </summary>
        void RetrieveData();

        #endregion
    }
}