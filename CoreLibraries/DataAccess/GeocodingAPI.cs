using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    /// <summary>
    /// Interfaces with the Google Geocoding API to retrieve the 
    /// coordinates from a location string and vice-versa.
    /// </summary>
    public class GeocodingAPI : XMLRequester
    {

        /// <summary>
        /// Enables bounding of results
        /// </summary>
        private bool enableBounding = false;
        
        /// <summary>
        /// Specifies the top left corner of the results bounding
        /// </summary>
        private Positioning.Location boundsTopLeft = null;

        /// <summary>
        /// Specifies the bottom right corner of the results bounding
        /// </summary>
        private Positioning.Location boundsBottomRight = null;
        
        /// <summary>
        /// Creates a new GeocodingAPI object.
        /// </summary>
        public GeocodingAPI() : base(Urls.GeocodingApiUrl) 
        {

            this.Sensor = false;
            this.Region = "au";

        
        }

        /// <summary>
        /// Gets or sets the region in which to search in to limit search results.
        /// </summary>
        /// <example>au, es</example>
        public string Region
        {
            get
            {
                return this.Parameters["region"];
            }
            set
            {
                this.Parameters["region"] = value;
            }
        }

        /// <summary>
        /// Gets or sets if the request is coming from a device with a location sensor. This is required by the Google Geocoding API.
        /// </summary>
        public bool Sensor
        {
            get
            {
                return Convert.ToBoolean(this.Parameters["sensor"]);
            }
            set
            {
                this.Parameters["sensor"] = value.ToString().ToLower();
            }

        }

        /// <summary>
        /// Enable bounding of the result within the square defined by BoundsUpperLeft and BoundsBottomRight. 
        /// </summary>
        public bool EnableBounding
        {
            get
            {
                return enableBounding;
            }
            set
            {
                enableBounding = value;
                setBounds();
            }

        }

        /// <summary>
        /// Gets or sets the upper left corner of the bounds when bounding is enabled.
        /// </summary>
        public Positioning.Location BoundsUpperLeft
        {
            get
            {
                return boundsTopLeft;
            }
            set
            {
                boundsTopLeft = value;
                setBounds();
            }
        }

        /// <summary>
        /// Gets or sets the lower right corner of the bounds when bounding is enabled.
        /// </summary>
        public Positioning.Location BoundsBottomRight
        {
            get
            {
                return boundsBottomRight;
            }
            set
            {
                boundsBottomRight = value;
                setBounds();
            }
        }

        /// <summary>
        /// Sets the bounds parameter for the XML request from the stored Location objects.
        /// </summary>
        private void setBounds()
        {
            //Error checking
            if (boundsTopLeft == null || boundsBottomRight == null)
            {
                throw new ArgumentException("Cannot enable results bounding without setting bounds first");
            }
                        
            Parameters["bounds"] = 
                boundsTopLeft.Latitude.ToString() + "," + 
                boundsTopLeft.Longitude.ToString() + "|" +
                boundsBottomRight.Latitude.ToString() + "," +
                boundsBottomRight.Longitude.ToString();

        }
        
        /// <summary>
        /// Gets a new Location from a Google Maps search string.
        /// </summary>OK
        /// <example>Pizza shops near RMIT University</example>
        /// <param name="location">A Google Maps search string</param>
        /// <returns></returns>
        public Positioning.Location GetLocation(string locationString)
        {
            //Clear out old request data
            this.Parameters.Remove("latlng");

            //Set new request data
            this.Parameters["address"] = locationString;
            
            //Perform XML request
            XmlDocument doc = base.Request();

            //Check request status
            if (doc["GeocodeResponse"]["status"].InnerText != "OK")
            {
                throw new Exception("Error processing XML request");
            }

            //Get the useful node
            XmlNode locationNode = doc["GeocodeResponse"]["result"]["geometry"]["location"];

            //Return the new position
            return new Positioning.Location(
                Convert.ToDouble(locationNode["lat"].InnerText), 
                Convert.ToDouble(locationNode["lng"].InnerText)
                );
        }

        /// <summary>
        /// Gets a human readable address that is closest to the provided location.
        /// </summary>
        /// <param name="location">A location for which you want to find the nearest address</param>
        /// <returns></returns>
        public string GetLocationString(Positioning.Location location)
        {
            this.Parameters.Remove("address");

            //Set new request data
            this.Parameters["latlng"] = location.ToString();

            //Perform XML request
            XmlDocument doc = base.Request();

            //Check request status
            if (doc["GeocodeResponse"]["status"].InnerText != "OK")
            {
                throw new Exception("Error processing XML request");
            }

            //Get the useful node
            XmlNode locationNode = doc["GeocodeResponse"]["result"]["formatted_address"];

            //Return the new location string            
            return locationNode.InnerText;
        }



       

    }
}
