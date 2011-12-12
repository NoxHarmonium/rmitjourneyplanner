// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="GeometryHelper.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   A collection of methods to help calculate geometry
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Positioning
{
    #region

    using System;

    #endregion

    /// <summary>
    /// A collection of methods to help calculate geometry
    /// </summary>
    public static class GeometryHelper
    {
        #region Public Methods

        /// <summary>
        /// Gets the distance between 2 points calculcated with the curvature of the earth.
        /// </summary>
        /// <param name="latitude1">
        /// The latitude of the first point.
        /// </param>
        /// <param name="longitude1">
        /// The longitude of the first point.
        /// </param>
        /// <param name="latitude2">
        /// The latitude of the second point.
        /// </param>
        /// <param name="longitude2">
        /// The longitude of the second point.
        /// </param>
        /// <returns>
        /// The straight line distance in kilometers.
        /// </returns>
        public static double GetStraightLineDistance(
            double latitude1, double longitude1, double latitude2, double longitude2)
        {
            return GetStraightLineDistance(new Location(latitude1, longitude1), new Location(latitude2, longitude2));
        }

        /// <summary>
        /// Gets the distance between 2 points calculated with the curvature of the earth.
        /// </summary>
        /// <param name="locationA">
        /// The first point.
        /// </param>
        /// <param name="locationB">
        /// The second point.
        /// </param>
        /// <returns>
        /// The  straight line distance in kilometers.
        /// </returns>
        public static double GetStraightLineDistance(Location locationA, Location locationB)
        {
            /*
             * Sourced  from http://www.movable-type.co.uk/scripts/latlong.html
             * Javascript code:
                var R = 6371; // km
                var dLat = (lat2-lat1).toRad();
                var dLon = (lon2-lon1).toRad();
                var lat1 = lat1.toRad();
                var lat2 = lat2.toRad();

                var a = Math.sin(dLat/2) * Math.sin(dLat/2) +
                        Math.sin(dLon/2) * Math.sin(dLon/2) * Math.cos(lat1) * Math.cos(lat2); 
                var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a)); 
                var d = R * c;
             
             * 
             * */
            const double R = 6371; // Mean radius of the earth in KM
            double dLat = ToRads(locationB.Latitude - locationA.Latitude);
            double dLon = ToRads(locationB.Longitude - locationA.Longitude);
            double lat1 = ToRads(locationA.Latitude);
            double lat2 = ToRads(locationB.Latitude);

            double a = (Math.Sin(dLat / 2.0) * Math.Sin(dLat / 2.0))
                       + (Math.Sin(dLon / 2.0) * Math.Sin(dLon / 2.0) * Math.Cos(lat1) * Math.Cos(lat2));
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        /// <summary>
        /// Converts the specified number to radians to degrees.
        /// </summary>
        /// <param name="x">
        /// The value to convert to degrees.
        /// </param>
        /// <returns>
        /// The value converted to degrees.
        /// </returns>
        public static double ToDegs(double x)
        {
            return x * (180.0 / Math.PI);
        }

        /// <summary>
        /// Converts a given number in degrees into radians.
        /// </summary>
        /// <param name="x">
        /// The value to convert to radians.
        /// </param>
        /// <returns>
        /// The value converted to radians.
        /// </returns>
        public static double ToRads(double x)
        {
            return x * (Math.PI / 180);
        }

        /// <summary>
        /// Calculates the new position if you were to move along the bearing for a specified distance.
        /// </summary>
        /// <param name="initial">
        /// The initial location
        /// </param>
        /// <param name="bearing">
        /// The bearing in degrees from north in degrees.
        /// </param>
        /// <param name="distance">
        /// The distance travelled in kilometers
        /// </param>
        /// <returns>
        /// The new position.
        /// </returns>
        public static Location Travel(Location initial, double bearing, double distance)
        {
            /*
             var lat2 = Math.asin( Math.sin(lat1)*Math.cos(d/R) + 
                      Math.cos(lat1)*Math.sin(d/R)*Math.cos(brng) );
            var lon2 = lon1 + Math.atan2(Math.sin(brng)*Math.sin(d/R)*Math.cos(lat1), 
                             Math.cos(d/R)-Math.sin(lat1)*Math.sin(lat2));
             * */
            const double R = 6371; // Mean radius of the earth in KM
            bearing = ToRads(bearing);
            double lat1 = ToRads(initial.Latitude);
            double lon1 = ToRads(initial.Longitude);
            double lat2 =
                Math.Asin(
                    Math.Sin(lat1) * Math.Cos(distance / R)
                    + Math.Cos(lat1) * Math.Sin(distance / R) * Math.Cos(bearing));
            double lon2 = lon1
                          +
                          Math.Atan2(
                              Math.Sin(bearing) * Math.Sin(distance / R) * Math.Cos(lat1), 
                              Math.Cos(distance / R) - Math.Sin(lat1) * Math.Sin(lat2));

            lon2 = (lon2 + 3.0 * Math.PI) % (2 * Math.PI) - Math.PI;

            return new Location(ToDegs(lat2), ToDegs(lon2));
        }

        #endregion
    }
}