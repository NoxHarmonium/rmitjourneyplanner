﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Route.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Represents a route made up of INetworkNodes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Ptv;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;

    #endregion

    /// <summary>
    /// Represents a route made up of INetworkNodes.
    /// </summary>
    public class Route : List<NodeWrapper<INetworkNode>>, ICloneable
    {
        #region Constants and Fields

        /// <summary>
        ///   The id.
        /// </summary>
        private readonly int id;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Route"/> class.
        /// </summary>
        /// <param name="routeId">
        /// The Id of the specified route. 
        /// </param>
        public Route(int routeId)
        {
            this.id = routeId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Route"/> class.
        /// </summary>
        /// <param name="routeId">
        /// The Id of the specified route. 
        /// </param>
        /// <param name="nodes">
        /// An list of <see cref="INetworkNode"/> objects to initialize the route with.
        /// </param>
        public Route(int routeId, IEnumerable<NodeWrapper<INetworkNode>> nodes)
            : base(nodes)
        {
            this.id = routeId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Route"/> class.
        /// </summary>
        /// <param name="routeId">
        /// The Id of the specified route. 
        /// </param>
        /// <param name="nodes">
        /// An list of <see cref="INetworkNode"/> objects to initialize the route with.
        /// </param>
        public Route(int routeId, IEnumerable<INetworkNode> nodes)
        {
            foreach (INetworkNode node in nodes)
            {
                this.Add(node);
            }

            this.id = routeId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the route Id.
        /// </summary>
        public int Id
        {
            get
            {
                return this.id;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns a new route as a combination of 2 routes joined from the end nodes.
        /// </summary>
        /// <param name="route1">
        /// The first route to be combined.
        /// </param>
        /// <param name="route2">
        /// The second route to be combined.
        /// </param>
        /// <returns>
        /// The two routes combined.
        /// </returns>
        public static Route Glue(Route route1, Route route2)
        {
            Route r = new Route(-1);
            r.AddRange(route1);
            route2.Reverse();
            route2.RemoveAt(0);
            r.AddRange(route2);
            return r;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        public void Add(INetworkNode node)
        {
            this.Add(new NodeWrapper<INetworkNode>(node));
        }

        /// <summary>
        /// Returns a copy of this route and copies all internal nodes.
        /// </summary>
        /// <returns>
        /// A new Route object. 
        /// </returns>
        public object Clone()
        {
            var newRoute = new Route(this.id);
            newRoute.AddRange(this.Select(node => (NodeWrapper<INetworkNode>)node.Clone()));
            return newRoute;
        }

        /// <summary>
        /// Returns the string representation of this object.
        /// </summary>
        /// <returns>
        /// A human readable string representing this object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Route: {0}", string.Join(",", this.Select(n => ((PtvNode)n.Node).StopSpecName)));
        }

        #endregion
    }
}