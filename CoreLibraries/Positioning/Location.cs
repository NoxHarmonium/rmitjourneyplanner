using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RmitJourneyPlanner.CoreLibraries.Positioning
{
    /// <summary>
    /// Represents a location on earth.   
    /// </summary>
    public class Location
    {
        
        private double latitude;
        private double longitude;

        /// <summary>
        /// Creates a Location object directly from the latitude and longitude.
        /// </summary>
        /// <param name="latitude">The latitude of the location</param>
        /// <param name="longitude">The longitude of the location</param>
        public Location(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        /// <summary>
        /// Creates a Location object by querying the Google Geocoding API for a location.
        /// </summary>
        /// <param name="location"></param>
        public Location(string location)
        {
            DataAccess.GeocodingAPI geocoding = new DataAccess.GeocodingAPI();
            Location discoveredLocation = geocoding.GetLocation(location);
            this.latitude = discoveredLocation.Latitude;
            this.longitude = discoveredLocation.Longitude;
        }

        /// <summary>
        /// Gets the longitude of this location
        /// </summary>
        public double Longitude
        {
            get
            {
                return longitude;
            }
        }

        /// <summary>
        /// Gets the latitude of this location
        /// </summary>
        public double Latitude
        {
            get
            {
                return latitude;
            }

        }

        /// <summary>
        /// Returns a comma delimited string of latitude and longitude.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0},{1}", latitude,longitude);
        }

    }
}
