// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    #region

    using System;
    using System.Globalization;
    using System.Xml;

    using RmitJourneyPlanner.CoreLibraries.Positioning;

    #endregion

    /// <summary>
    ///   Interfaces with the Google Geocoding API to retrieve the coordinates from a location string and vice-versa.
    /// </summary>
    internal class GeocodingApi : XmlRequester
    {
        #region Constants and Fields

        /// <summary>
        ///   Specifies the bottom right corner of the results bounding
        /// </summary>
        private Location boundsBottomRight;

        /// <summary>
        ///   Specifies the top left corner of the results bounding
        /// </summary>
        private Location boundsTopLeft;

        /// <summary>
        ///   Enables bounding of results
        /// </summary>
        private bool enableBounding;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="GeocodingApi" /> class.
        /// </summary>
        public GeocodingApi()
            : base(Urls.GeocodingApiUrl)
        {
            this.Sensor = false;
            this.Region = "au";
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the lower right corner of the bounds when bounding is enabled.
        /// </summary>
        public Location BoundsBottomRight
        {
            get
            {
                return this.boundsBottomRight;
            }

            set
            {
                this.boundsBottomRight = value;
                this.SetBounds();
            }
        }

        /// <summary>
        ///   Gets or sets the upper left corner of the bounds when bounding is enabled.
        /// </summary>
        public Location BoundsUpperLeft
        {
            get
            {
                return this.boundsTopLeft;
            }

            set
            {
                this.boundsTopLeft = value;
                this.SetBounds();
            }
        }

        /// <summary>
        ///   Enable bounding of the result within the square defined by BoundsUpperLeft and BoundsBottomRight.
        /// </summary>
        public bool EnableBounding
        {
            get
            {
                return this.enableBounding;
            }

            set
            {
                this.enableBounding = value;
                this.SetBounds();
            }
        }

        /// <summary>
        ///   Gets or sets the region in which to search in to limit search results.
        /// </summary>
        /// <example>
        ///   au, es
        /// </example>
        public string Region
        {
            get
            {
                return this.Parameters["region"] as string;
            }

            set
            {
                this.Parameters["region"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the request is coming from a device with a location sensor. This is required by the Google Geocoding API.
        /// </summary>
        public bool Sensor
        {
            get
            {
                return Convert.ToBoolean(this.Parameters["sensor"]);
            }

            set
            {
                this.Parameters["sensor"] = value.ToString(CultureInfo.InvariantCulture).ToLower();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Gets a new Location from a Google Maps search string.
        /// </summary>
        /// OK
        /// <example>
        ///   Pizza shops near RMIT University
        /// </example>
        /// <param name="locationString"> A Google Maps search string </param>
        /// <returns> A <see cref="Location" /> object. </returns>
        public Location GetLocation(string locationString)
        {
            // Clear out old request data
            this.Parameters.Remove("latlng");

            // Set new request data
            this.Parameters["address"] = locationString;

            // Perform XML request
            XmlDocument doc = this.Request();

            if (doc["GeocodeResponse"] == null || doc["GeocodeResponse"]["status"] == null)
            {
                throw new Exception("XML response is invalid.");
            }

            // Check request status
            if (doc["GeocodeResponse"]["status"].InnerText != "OK")
            {
                throw new GoogleApiException(doc["GeocodeResponse"]["status"].InnerText);
            }

            // Get the useful node
            XmlNode locationNode = doc["GeocodeResponse"]["result"]["geometry"]["location"];

            // Return the new position
            return new Location(
                Convert.ToDouble(locationNode["lat"].InnerText), Convert.ToDouble(locationNode["lng"].InnerText));
        }

        /// <summary>
        ///   Gets a human readable address that is closest to the provided location.
        /// </summary>
        /// <param name="location"> A location for which you want to find the nearest address </param>
        /// <returns> The get location string. </returns>
        public string GetLocationString(Location location)
        {
            this.Parameters.Remove("address");

            // Set new request data
            this.Parameters["latlng"] = location.ToString();

            // Perform XML request
            XmlDocument doc = this.Request();

            if (doc["GeocodeResponse"] == null || doc["GeocodeResponse"]["status"] == null)
            {
                throw new Exception("XML response is invalid.");
            }

            // Check request status
            if (doc["GeocodeResponse"]["status"].InnerText != "OK")
            {
                throw new Exception("Error processing XML request");
            }

            // Get the useful node
            XmlNode locationNode = doc["GeocodeResponse"]["result"]["formatted_address"];

            // Return the new location string            
            return locationNode.InnerText;
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Sets the bounds parameter for the XML request from the stored Location objects.
        /// </summary>
        private void SetBounds()
        {
            // Error checking
            if (this.boundsTopLeft == null || this.boundsBottomRight == null)
            {
                throw new ArgumentException("Cannot enable results bounding without setting bounds first");
            }

            this.Parameters["bounds"] = this.boundsTopLeft.Latitude.ToString(CultureInfo.InvariantCulture) + ","
                                        + this.boundsTopLeft.Longitude.ToString(CultureInfo.InvariantCulture) + "|"
                                        + this.boundsBottomRight.Latitude.ToString(CultureInfo.InvariantCulture) + ","
                                        + this.boundsBottomRight.Longitude.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }
}