// -----------------------------------------------------------------------
// <copyright file="WalkingDataProvider.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using RmitJourneyPlanner.CoreLibraries.DataAccess;

    /// <summary>
    /// Provides point to point data for walking.
    /// </summary>
    public class WalkingDataProvider : IPointDataProvider
    {

        private DistanceAPI distanceAPI;


        /// <summary>
        /// Initializes a new instance of WalkingDataProvider
        /// </summary>
        public WalkingDataProvider()
        {
            distanceAPI = new DistanceAPI();
        }
        
        /// <summary>
        /// Gets the distance walking from locationA to locationB
        /// </summary>
        /// <param name="locationA"></param>
        /// <param name="locationB"></param>
        /// <returns></returns>
        public Types.Arc GetDistance(Positioning.Location locationA, Positioning.Location locationB)
        {
            return distanceAPI.GetDistance(locationA, locationB, Types.TransportMode.Walking);
        }

        /// <summary>
        /// Gets the path walked from locationA to locationB
        /// </summary>
        /// <param name="locationA"></param>
        /// <param name="locationB"></param>
        /// <returns></returns>
        public List<Positioning.Location> GetPath(Positioning.Location locationA, Positioning.Location locationB)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// Estimates the distance between 2 points. This method must always underestimate the time and distance. 
        /// </summary>
        /// <param name="locationA"></param>
        /// <param name="locationB"></param>
        /// <returns></returns>
        public Types.Arc EstimateDistance(Positioning.Location locationA, Positioning.Location locationB)
        {
            
            double distance =  Positioning.GeometryHelper.GetStraightLineDistance(locationA, locationB);
            TimeSpan time = new TimeSpan(0, 0, (int)(distance / 0.00166666667));
            return new Types.Arc(locationA,locationB,time,distance,default(DateTime),"Walking");

        }
    }
}
