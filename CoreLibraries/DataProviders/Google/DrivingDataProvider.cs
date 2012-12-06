// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DrivingDataProvider.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Provides point to point data for driving.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders.Google
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataAccess;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Provides point to point data for driving.
    /// </summary>
    public class DrivingDataProvider : IPointDataProvider
    {
        #region Constants and Fields

        /// <summary>
        ///   The distance api.
        /// </summary>
        private readonly DistanceApi distanceApi;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DrivingDataProvider" /> class.
        /// </summary>
        public DrivingDataProvider()
        {
            this.distanceApi = new DistanceApi();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Estimates the distance between 2 points. This method must always underestimate the time and distance.
        /// </summary>
        /// <param name="locationA">
        /// The first point. 
        /// </param>
        /// <param name="locationB">
        /// The second point. 
        /// </param>
        /// <returns>
        /// The <see cref="Arc"/> representing the distance between the 2 points. 
        /// </returns>
        public Arc EstimateDistance(Location locationA, Location locationB)
        {
            double distance = GeometryHelper.GetStraightLineDistance(locationA, locationB);
            var time = new TimeSpan(0, 0, (int)(distance / 0.0305555556));
            return new Arc(
                locationA, 
                locationB, 
                new TransportTimeSpan { TravelTime = time, WaitingTime = default(TimeSpan) }, 
                distance, 
                default(DateTime), 
                "Driving");
        }

        /// <summary>
        /// Gets the distance driving from locationA to locationB
        /// </summary>
        /// <param name="locationA">
        /// The first point. 
        /// </param>
        /// <param name="locationB">
        /// The second point. 
        /// </param>
        /// <returns>
        /// The <see cref="Arc"/> representing the distance between the 2 points. 
        /// </returns>
        public Arc GetDistance(Location locationA, Location locationB)
        {
            return this.distanceApi.GetDistance(locationA, locationB, TransportMode.Walking);
        }

        /// <summary>
        /// Gets the path driven from locationA to locationB
        /// </summary>
        /// <param name="locationA">
        /// The first point. 
        /// </param>
        /// <param name="locationB">
        /// The second point. 
        /// </param>
        /// <returns>
        /// A list of <see cref="Location"/> objects that designate a path. 
        /// </returns>
        public List<Location> GetPath(Location locationA, Location locationB)
        {
            //// TODO: Impliment Google driving path finding.
            throw new NotImplementedException();
        }

        #endregion
    }
}