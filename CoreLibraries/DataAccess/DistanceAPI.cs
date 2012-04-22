// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    #region

    using System;
    using System.Xml;

    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   Interfaces with the Google Distance Matrix API to retrieve distances between points (as navigated by Google Maps).
    /// </summary>
    internal class DistanceApi : XmlRequester
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="DistanceApi" /> class.
        /// </summary>
        public DistanceApi()
            : base(Urls.DistanceApiUrl)
        {
            this.Parameters["sensor"] = "false";
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Returns the distance between 2 points using the default transport mode (driving).
        /// </summary>
        /// <param name="pointA"> The first point </param>
        /// <param name="pointB"> The second point </param>
        /// <returns> The distance between pointA and pointB </returns>
        public Arc GetDistance(Location pointA, Location pointB)
        {
            return this.GetDistance(pointA, pointB, TransportMode.Driving);
        }

        /// <summary>
        ///   Returns the distance between 2 points using the specified transport mode.
        /// </summary>
        /// <param name="pointA"> The first point </param>
        /// <param name="pointB"> The second point </param>
        /// <param name="transportMode"> Specified the mode of transport used between points. </param>
        /// <returns> The distance between pointA and pointB </returns>
        public Arc GetDistance(Location pointA, Location pointB, TransportMode transportMode)
        {
            // Set parameters
            this.Parameters["mode"] = transportMode.ToString().ToLower();
            this.Parameters["origins"] = pointA.ToString();
            this.Parameters["destinations"] = pointB.ToString();

            // Make XML request
            XmlDocument doc = this.Request();

            if (doc["DistanceMatrixResponse"] == null)
            {
                throw new Exception("XML response is invalid.");
            }

            XmlNode response = doc["DistanceMatrixResponse"];

            if (response["status"] == null)
            {
                throw new Exception("XML response is invalid.");
            }

            // Check request status
            if (response["status"].InnerText != "OK")
            {
                throw new GoogleApiException(response["status"].InnerText);
            }

            // Extract element
            XmlNode element = response["row"]["element"];

            // Get results
            var duration = new TimeSpan(0, 0, Convert.ToInt32(element["duration"]["value"].InnerText));
            double distance = Convert.ToDouble(element["distance"]["value"].InnerText);

            // Return new object
            return new Arc(pointA, pointB, duration, distance, default(DateTime), transportMode.ToString());
        }

        #endregion
    }
}