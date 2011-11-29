using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RmitJourneyPlanner.CoreLibraries.Types;
using RmitJourneyPlanner.CoreLibraries.Positioning;
using System.Xml;

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    /// <summary>
    /// Interfaces with the Google Distance Matrix API to retrieve distances
    /// between points (as navigated by Google Maps).
    /// </summary>
    class DistanceAPI : XMLRequester
    {
        /// <summary>
        /// Initializes a new instance of the DistanceAPI class.
        /// </summary>
        public DistanceAPI()
            : base(Urls.DistanceApiUrl)
        {
            this.Parameters["sensor"] = "false";
        }

        /// <summary>
        /// Returns the distance between 2 points using the default transport mode (driving).
        /// </summary>
        /// <param name="pointA">The first point</param>
        /// <param name="pointB">The second point</param>
        /// <returns>The distance between pointA and pointB</returns>
        public Arc GetDistance(Location pointA, Location pointB)
        {
            return GetDistance(pointA, pointB, TransportMode.Driving);
        }

        /// <summary>
        /// Returns the distance between 2 points using the specified transport mode.
        /// </summary>
        /// <param name="pointA">The first point</param>
        /// <param name="pointB">The second point</param>
        /// <param name="transportMode">Specified the mode of transport used between points.</param>
        /// <returns>The distance between pointA and pointB</returns>
        public Arc GetDistance(Location pointA, Location pointB, TransportMode transportMode)
        {
            //Set parameters
            this.Parameters["mode"] = transportMode.ToString().ToLower();
            this.Parameters["origins"] = pointA.ToString();
            this.Parameters["destinations"] = pointB.ToString();

            //Make XML request
            XmlDocument doc = base.Request();
            XmlNode response = doc["DistanceMatrixResponse"];

            //Check request status
            if (response["status"].InnerText != "OK")
            {
                throw new GoogleApiException(doc["GeocodeResponse"]["status"].InnerText);
            }

            //Extract element
            XmlNode element = response["row"]["element"];

            //Get results
            TimeSpan duration = new TimeSpan(0, 0, Convert.ToInt32(element["duration"]["value"].InnerText));
            double distance = Convert.ToDouble(element["distance"]["value"].InnerText);

            //Return new object
            return new Arc(pointA, pointB,duration,distance,default(DateTime),transportMode.ToString());

        }
        

        
    }
}
