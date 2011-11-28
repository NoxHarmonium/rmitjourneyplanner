// -----------------------------------------------------------------------
// <copyright file="DrivingDataProvider.cs" company="">
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
    /// Provides point to point data for driving.
    /// </summary>
    public class DrivingDataProvider : IPointDataProvider
    {
         private DistanceAPI distanceAPI;

        /// <summary>
        /// Initilizes a new instance of DrivingDataProvider.
        /// </summary>
        public DrivingDataProvider()
        {
            distanceAPI = new DistanceAPI();
        }
        
        /// <summary>
        /// Gets the distance driving from locationA to locationB
        /// </summary>
        /// <param name="locationA"></param>
        /// <param name="locationB"></param>
        /// <returns></returns>
        public Types.Arc GetDistance(Positioning.Location locationA, Positioning.Location locationB)
        {
            return distanceAPI.GetDistance(locationA, locationB, Types.TransportMode.Walking);
        }

        /// <summary>
        /// Gets the path driven from locationA to locationB
        /// </summary>
        /// <param name="locationA"></param>
        /// <param name="locationB"></param>
        /// <returns></returns>
        public List<Positioning.Location> GetPath(Positioning.Location locationA, Positioning.Location locationB)
        {
            throw new NotImplementedException();
        }
    }
}
