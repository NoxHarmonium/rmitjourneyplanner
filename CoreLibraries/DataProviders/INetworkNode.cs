// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INetworkNode.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Represents a node in a transport network such a train station or tram stop.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    #region Using Directives

    using System;

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Represents a node in a transport network such a train station or tram stop.
    /// </summary>
    public interface INetworkNode : IEquatable<INetworkNode>, ICloneable, INode
    {
        #region Public Properties

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
        ///   Gets or sets the current route that the node is traversing.
        /// </summary>
        int RouteId { get; set; }

        /// <summary>
        ///   Gets or sets the type of transport this node services.
        /// </summary>
        TransportMode TransportType { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Loads the properties of the node from the parent.
        /// </summary>
        void RetrieveData();

        #endregion
    }
}