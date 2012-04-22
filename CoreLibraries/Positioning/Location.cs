// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.Positioning
{
    #region

    using System;

    using RmitJourneyPlanner.CoreLibraries.DataAccess;
    using RmitJourneyPlanner.CoreLibraries.Logging;

    #endregion

    /// <summary>
    ///   Represents a location on earth.
    /// </summary>
    public class Location
    {
        #region Constants and Fields

        /// <summary>
        ///   The latitude.
        /// </summary>
        private double latitude;

        /// <summary>
        ///   The longitude.
        /// </summary>
        private double longitude;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="Location" /> class. Creates a Location object directly from the latitude and longitude.
        /// </summary>
        /// <param name="latitude"> The latitude of the location </param>
        /// <param name="longitude"> The longitude of the location </param>
        public Location(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Location" /> class. Creates a Location object directly from the latitude and longitude.
        /// </summary>
        /// <param name="latitude"> The latitude of the location </param>
        /// <param name="longitude"> The longitude of the location </param>
        public Location(float latitude, float longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Location" /> class.
        /// </summary>
        /// <param name="location"> The location string to be resolved by the Google Maps API. </param>
        public Location(string location)
        {
            Logger.Log(this,"Querying Google API for location: {0}", location);
            var geocoding = new GeocodingApi();
            Location discoveredLocation = geocoding.GetLocation(location);
            this.latitude = discoveredLocation.Latitude;
            this.longitude = discoveredLocation.Longitude;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets (protected) the latitude of this location
        /// </summary>
        public double Latitude
        {
            get
            {
                return this.latitude;
            }

            protected set
            {
                this.latitude = value;
            }
        }

        /// <summary>
        ///   Gets or sets (protected) the longitude of this location
        /// </summary>
        public double Longitude
        {
            get
            {
                return this.longitude;
            }

            protected set
            {
                this.longitude = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Parses a comma delimted latitude and longitude string and returns a corrosponding location object.
        /// </summary>
        /// <param name="locationString"> The location string to parse to a location. </param>
        /// <returns> A <see cref="Location" /> object that represents the provided location string. </returns>
        public static Location Parse(string locationString)
        {
            int commaIndex = locationString.IndexOf(",", StringComparison.Ordinal);
            string lat = locationString.Substring(0, commaIndex);
            string lon = locationString.Substring(
                locationString.IndexOf(",", StringComparison.Ordinal) + 1, locationString.Length - commaIndex - 1);
            return new Location(Convert.ToDouble(lat), Convert.ToDouble(lon));
        }

        /// <summary>
        ///   Returns a comma delimited string of latitude and longitude.
        /// </summary>
        /// <returns> A string. </returns>
        public override string ToString()
        {
            return string.Format("{0},{1}", this.latitude, this.longitude);
        }

        #endregion
    }
}