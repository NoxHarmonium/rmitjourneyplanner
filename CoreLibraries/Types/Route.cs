// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region

    using System;
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    #endregion

    /// <summary>
    ///   Represents a route made up of INetworkNodes.
    /// </summary>
    public class Route : List<INetworkNode>, ICloneable
    {
        #region Constants and Fields

        /// <summary>
        ///   The id.
        /// </summary>
        private readonly int id;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="Route" /> class. 
        /// </summary>
        /// <param name="routeId"> The Id of the specified route. </param>
        public Route(int routeId)
        {
            this.id = routeId;
        }
        /// <summary>
        ///   Initializes a new instance of the <see cref="Route" /> class. 
        /// </summary>
        /// <param name="routeId"> The Id of the specified route. </param>
        /// <param name="nodes">An list of <see cref="INetworkNode" /> objects to initialize the route with.</param>
        public Route(int routeId, IEnumerable<INetworkNode> nodes ) : base(nodes)
        {
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

        #region Public Methods

        /// <summary>
        /// Returns a new route as a combination of 2 routes joined from the end nodes.
        /// </summary>
        /// <param name="route1"></param>
        /// <param name="route2"></param>
        /// <returns></returns>
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
        ///   Returns a copy of this route and copies all internal nodes.
        /// </summary>
        /// <returns> A new Route object. </returns>
        public object Clone()
        {
            var newRoute = new Route(this.id);
            newRoute.AddRange(this);
            return newRoute;
        }

        #endregion
    }
}