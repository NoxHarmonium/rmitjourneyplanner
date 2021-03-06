﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WalkingDataProvider.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Provides point to point data for walking.
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
    /// Provides point to point data for walking.
    /// </summary>
    public class WalkingDataProvider : IPointDataProvider
    {
        #region Constants and Fields

        /// <summary>
        ///   The distance api.
        /// </summary>
        private readonly DistanceApi distanceApi;

        /// <summary>
        ///   The walking speed used in estimating distance.
        /// </summary>
        private double walkSpeed = 4; // KM/h

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "WalkingDataProvider" /> class.
        /// </summary>
        public WalkingDataProvider()
        {
            this.distanceApi = new DistanceApi();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the walking speed used in estimating distance.
        /// </summary>
        public double WalkSpeed
        {
            get
            {
                return this.walkSpeed;
            }

            set
            {
                this.walkSpeed = value;
            }
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
            var time = TimeSpan.FromHours(distance / this.WalkSpeed);
            return new Arc(
                locationA, 
                locationB, 
                new TransportTimeSpan { TravelTime = time, WaitingTime = default(TimeSpan) }, 
                distance, 
                default(DateTime), 
                "Walking");
        }

        /// <summary>
        /// Gets the distance walking from locationA to locationB
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
        /// Gets the path walked from locationA to locationB
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
            //// TODO: Impliment Google walking path finding.
            throw new NotImplementedException();
        }

        #endregion
    }
}