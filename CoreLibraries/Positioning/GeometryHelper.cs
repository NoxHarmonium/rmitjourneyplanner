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
        public static double ToRads(double x)
        {
            return x * (Math.PI / 180);

        }
        
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
