// -----------------------------------------------------------------------
// <copyright file="GeometryHelper.cs" company="RMIT University">
// By Sean Dawson
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Positioning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// A collection of methods to help calculate geometry
    /// </summary>
    public static class GeometryHelper
    {
        
        /// <summary>
        /// Converts a given number in degrees into radians.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double ToRads(double x)
        {
            return x * (Math.PI / 180);

        }

        /// <summary>
        /// Converts the specified number to degrees from radians.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double ToDegs(double x)
        {
            return x * (180.0 / Math.PI);
        }

        /// <summary>
        /// Calculates the new position if you were to move along the bearing for a specified distance.
        /// </summary>
        /// <param name="initial">The initial location</param>
        /// <param name="bearing">The bearing in degrees from north in degrees.</param>
        /// <param name="distance">The distance travelled in kilometers</param>
        /// <returns></returns>
        public static Location Travel(Location initial, double bearing, double distance)
        {

            /*
             var lat2 = Math.asin( Math.sin(lat1)*Math.cos(d/R) + 
                      Math.cos(lat1)*Math.sin(d/R)*Math.cos(brng) );
            var lon2 = lon1 + Math.atan2(Math.sin(brng)*Math.sin(d/R)*Math.cos(lat1), 
                             Math.cos(d/R)-Math.sin(lat1)*Math.sin(lat2));
             * */
            double r = 6371; //Mean radius of the earth in KM
            bearing = ToRads(bearing);
            double lat1 = ToRads(initial.Latitude);
            double lon1 = ToRads(initial.Longitude);
            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(distance / r) +
                Math.Cos(lat1) * Math.Sin(distance / r) * Math.Cos(bearing));
            double lon2 = lon1 + Math.Atan2(Math.Sin(bearing) * Math.Sin(distance / r) * Math.Cos(lat1),
                Math.Cos(distance / r) - Math.Sin(lat1) * Math.Sin(lat2));
            
            lon2 = (lon2 + 3.0 * Math.PI) % (2 * Math.PI) - Math.PI;
            
            return new Location(ToDegs(lat2),ToDegs(lon2));
        }

        /// <summary>
        /// Gets the distance between 2 points calculated with the curvature of the earth.
        /// </summary>
        /// <param name="locationA"></param>
        /// <param name="locationB"></param>
        /// <returns></returns>
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

            double r = 6371; //Mean radius of the earth in KM
            double dLat = ToRads(locationB.Latitude - locationA.Latitude);
            double dLon = ToRads(locationB.Longitude - locationA.Longitude);
            double lat1 = ToRads(locationA.Latitude);
            double lat2 = ToRads(locationB.Latitude);

            double a = Math.Sin(dLat / 2.0) * Math.Sin(dLat / 2.0) +
                Math.Sin(dLon / 2.0) * Math.Sin(dLon / 2.0) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return r * c;
        }

    }
}
