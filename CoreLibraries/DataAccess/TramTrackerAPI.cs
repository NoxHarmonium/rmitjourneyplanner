﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TramTrackerAPI.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Interfaces with the Yarra Trams Tramtracker Soap web service to get various types of data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;

    #endregion

    /// <summary>
    /// Interfaces with the Yarra Trams Tramtracker Soap web service to get various types of data.
    /// </summary>
    internal class TramTrackerApi : XmlRequester
    {
        #region Constants and Fields

        /// <summary>
        ///   The regular expression that defines the correct format of a client UUID.
        /// </summary>
        private const string UuidFormat = "[a-zA-Z0-9]{8}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{12}";

        /// <summary>
        ///   The GUID.
        /// </summary>
        private readonly string guid;

        /// <summary>
        ///   The client type.
        /// </summary>
        private string clientType = "TRAMHUNTER";

        /// <summary>
        ///   The client version.
        /// </summary>
        private string clientVersion = "0.0.0.1";

        /// <summary>
        ///   The client web service version.
        /// </summary>
        private string clientWebServiceVersion = "6.4.0.0";

        /// <summary>
        ///   The operating system version.
        /// </summary>
        private string operatingSystemVersion = Environment.OSVersion.VersionString;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the TramTrackerApi class.
        /// </summary>
        public TramTrackerApi()
            : base(Urls.TramTrackerUrl)
        {
            this.RequestType = RequestType.Soap;
            this.SoapXmlNamespace = Urls.TramTrackerNameSpace;
            this.guid = this.GetNewGuid();
            this.HeaderParameters["PidsClientHeader"] = this.GenerateHeader();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the operating system version that is reported to the Tramtracker web service. This is usually auto detected and doesn't need to be changed off the default.
        /// </summary>
        public string ClientOsVersion
        {
            get
            {
                return this.operatingSystemVersion;
            }

            set
            {
                this.operatingSystemVersion = value;
                this.HeaderParameters["PidsClientHeader"] = this.GenerateHeader();
            }
        }

        /// <summary>
        ///   Gets or sets the client type reported to the Tramtracker web service.
        /// </summary>
        public string ClientType
        {
            get
            {
                return this.clientType;
            }

            set
            {
                this.clientType = value;
                this.HeaderParameters["PidsClientHeader"] = this.GenerateHeader();
            }
        }

        /// <summary>
        ///   Gets or sets the client version reported to the Tramtracker web service.
        /// </summary>
        public string ClientVersion
        {
            get
            {
                return this.clientVersion;
            }

            set
            {
                this.clientVersion = value;
                this.HeaderParameters["PidsClientHeader"] = this.GenerateHeader();
            }
        }

        /// <summary>
        ///   Gets or sets the web service version reported to the Tramtracker web service. This shouldn't need to be changed off the default.
        /// </summary>
        public string ClientWebServiceVersion
        {
            get
            {
                return this.clientWebServiceVersion;
            }

            set
            {
                this.clientWebServiceVersion = value;
                this.HeaderParameters["PidsClientHeader"] = this.GenerateHeader();
            }
        }

        /// <summary>
        ///   Gets the client GUID generated by the TramTracker web service.
        /// </summary>
        public string Guid
        {
            get
            {
                return this.guid;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns true if the supplied string is in the correct format for a client UUID.
        /// </summary>
        /// <param name="uuid">
        /// A TramTracker UUID. 
        /// </param>
        /// <returns>
        /// True if the UUID is valid, false if not. 
        /// </returns>
        public static bool IsValidUuid(string uuid)
        {
            return new Regex(UuidFormat).IsMatch(uuid);
        }

        /// <summary>
        /// Gets a dataset containing the end-points of each tram route.
        /// </summary>
        /// <returns>
        /// A <see cref="DataSet"/> containing the results. 
        /// </returns>
        public DataSet GetDestinationsForAllRoutes()
        {
            // Doesn't use  Soap.BuildDataSetFromSoapResponse() as the response is more complex.
            try
            {
                this.Parameters.Clear();

                // Set parameters           
                this.SoapAction = Urls.GetDestinationsForAllRoutes;
                this.Parameters["GetDestinationsForAllRoutes"] = null;
                XmlDocument responseDoc = this.Request();

                XmlNode response = responseDoc["soap:Envelope"]["soap:Body"]["GetDestinationsForAllRoutesResponse"];

                string validationResult = response["validationResult"].InnerText;
                if (validationResult.Contains("Request Denied:"))
                {
                    throw new Exception("Tramtracker webservice error:\n" + validationResult);
                }

                var results = new DataSet();
                results.ReadXml(new StringReader(response.FirstChild.OuterXml), XmlReadMode.ReadSchema);
                results.ReadXml(new StringReader(response.FirstChild.OuterXml), XmlReadMode.DiffGram);
                return results;
            }
            catch (Exception e)
            {
                throw new Exception("XML response is invalid.", e);
            }
        }

        /// <summary>
        /// Gets the 2 end-points for a specific tram route.
        /// </summary>
        /// <param name="routeId">
        /// The route identifier. 
        /// </param>
        /// <returns>
        /// A <see cref="DataSet"/> containing the results. 
        /// </returns>
        public DataSet GetDestinationsForRoute(string routeId)
        {
            this.Parameters.Clear();

            // Set parameters           
            this.SoapAction = Urls.GetDestinationsForRoute;

            // Create data node
            var dict = new Dictionary<string, object>();
            dict["routeNo"] = routeId;

            // XmlNode nodeGetDestinationsForRoute = Soap.BuildXmlFromDictionary(dict, this.SoapXmlNamespace);
            this.Parameters["GetDestinationsForRoute"] = Soap.BuildXmlFromDictionary(dict, this.SoapXmlNamespace);
            XmlDocument responseDoc = this.Request();

            return Soap.BuildDataSetFromSoapResponse(responseDoc);
        }

        /// <summary>
        /// Gets a dataset containing a list of all the stops pertaining to the supplied parameters.
        /// </summary>
        /// <param name="routeId">
        /// The route identifier. 
        /// </param>
        /// <param name="isUpDirection">
        /// A value that determines what direction the route is in. 
        /// </param>
        /// <returns>
        /// A <see cref="DataSet"/> containing the results. 
        /// </returns>
        public DataSet GetListOfStopsByRouteNoAndDirection(string routeId, bool isUpDirection)
        {
            this.Parameters.Clear();

            // Set parameters           
            this.SoapAction = Urls.GetListOfStopsByRouteNoAndDirection;

            // Create data node
            var dict = new Dictionary<string, object>();
            dict["routeNo"] = routeId;
            dict["isUpDirection"] = isUpDirection.ToString(CultureInfo.InvariantCulture).ToLower();

            // XmlNode parameterNodes = Soap.BuildXmlFromDictionary(dict, this.SoapXmlNamespace);
            this.Parameters["GetListOfStopsByRouteNoAndDirection"] = Soap.BuildXmlFromDictionary(
                dict, this.SoapXmlNamespace);
            XmlDocument responseDoc = this.Request();

            return Soap.BuildDataSetFromSoapResponse(responseDoc);
        }

        /// <summary>
        /// Gets a dataset containing a list of all main routes in the tram network.
        /// </summary>
        /// <returns>
        /// A <see cref="DataSet"/> containing the results. 
        /// </returns>
        public DataSet GetMainRoutes()
        {
            this.Parameters.Clear();

            // Set parameters           
            this.SoapAction = Urls.GetMainRoutes;
            this.Parameters["GetMainRoutes"] = null;
            XmlDocument responseDoc = this.Request();

            return Soap.BuildDataSetFromSoapResponse(responseDoc);
        }

        /// <summary>
        /// Gets a dataset containing all the main routes that intersect the supplied stop number.
        /// </summary>
        /// <param name="stopNo">
        /// The TramTracker identifier. 
        /// </param>
        /// <returns>
        /// A <see cref="DataSet"/> containing the results. 
        /// </returns>
        public DataSet GetMainRoutesForStop(string stopNo)
        {
            this.Parameters.Clear();

            // Set parameters           
            this.SoapAction = Urls.GetMainRoutesForStop;

            // Create data node
            var dict = new Dictionary<string, object>();
            dict["stopNo"] = stopNo;

            this.Parameters["GetMainRoutesForStop"] = Soap.BuildXmlFromDictionary(dict, this.SoapXmlNamespace);
            XmlDocument responseDoc = this.Request();

            return Soap.BuildDataSetFromSoapResponse(responseDoc);
        }

        /// <summary>
        /// Gets a dataset containing the predicted arrival time of the supplied tram number.
        /// </summary>
        /// <param name="tramNo">
        /// The number of a tram. 
        /// </param>
        /// <returns>
        /// A <see cref="DataSet"/> containing the results. 
        /// </returns>
        public DataSet GetNextPredictedArivalTime(string tramNo)
        {
            this.Parameters.Clear();

            // Set parameters            
            this.SoapAction = Urls.GetNextPredictedArrivalTimeAtStopsForTramNo;

            // Request data
            // Create data node
            var dict = new Dictionary<string, object>();
            dict["tramNo"] = tramNo;

            this.Parameters["GetNextPredictedArrivalTimeAtStopsForTramNo"] = Soap.BuildXmlFromDictionary(
                dict, this.SoapXmlNamespace);
            XmlDocument responseDoc = this.Request();

            return Soap.BuildDataSetFromSoapResponse(responseDoc);
        }

        /// <summary>
        /// Gets a dataset containing a collection of the next predicted trams for the supplied stop and route.
        /// </summary>
        /// <param name="stopNo">
        /// The TramTracker stop identifier. 
        /// </param>
        /// <param name="routeNo">
        /// The route identifier. 
        /// </param>
        /// <param name="lowFloor">
        /// Determines if a low floored tram is required. 
        /// </param>
        /// <returns>
        /// A <see cref="DataSet"/> containing the results. 
        /// </returns>
        public DataSet GetNextPredictedRoutesCollection(string stopNo, string routeNo, bool lowFloor)
        {
            this.Parameters.Clear();

            // Set parameters            
            this.SoapAction = Urls.GetNextPredictedRoutesCollection;

            // Request data
            // Create data node
            var dict = new Dictionary<string, object>();
            dict["stopNo"] = stopNo;
            dict["routeNo"] = routeNo;
            dict["lowFloor"] = lowFloor.ToString(CultureInfo.InvariantCulture).ToLower();

            this.Parameters["GetNextPredictedRoutesCollection"] = Soap.BuildXmlFromDictionary(
                dict, this.SoapXmlNamespace);
            XmlDocument responseDoc = this.Request();

            return Soap.BuildDataSetFromSoapResponse(responseDoc);
        }

        /// <summary>
        /// Gets a dataset containing a collection of schedules for the supplied stop and route.
        /// </summary>
        /// <param name="stopNo">
        /// The TramTracker stop identifier. 
        /// </param>
        /// <param name="routeNo">
        /// The route identifier. 
        /// </param>
        /// <param name="lowFloor">
        /// Determines if a low floored tram is required. 
        /// </param>
        /// <param name="clientRequestDateTime">
        /// The date and time of the request. 
        /// </param>
        /// <returns>
        /// A <see cref="DataSet"/> containing the results. 
        /// </returns>
        public DataSet GetSchedulesCollection(
            string stopNo, string routeNo, bool lowFloor, DateTime clientRequestDateTime)
        {
            this.Parameters.Clear();

            // Set parameters            
            this.SoapAction = Urls.GetSchedulesCollection;

            // Request data
            // Create data node
            var dict = new Dictionary<string, object>();
            dict["stopNo"] = stopNo;
            dict["routeNo"] = routeNo;
            dict["lowFloor"] = lowFloor.ToString(CultureInfo.InvariantCulture).ToLower();
            dict["clientRequestDateTime"] = XmlConvert.ToString(
                clientRequestDateTime, XmlDateTimeSerializationMode.Local);

            this.Parameters["GetSchedulesCollection"] = Soap.BuildXmlFromDictionary(dict, this.SoapXmlNamespace);
            XmlDocument responseDoc = this.Request();

            return Soap.BuildDataSetFromSoapResponse(responseDoc);
        }

        /// <summary>
        /// Gets a dataset containing the schedules for the specified trip Id.
        /// </summary>
        /// <param name="tripId">
        /// A trip identifier. 
        /// </param>
        /// <param name="scheduledDateTime">
        /// The scheduled date and time of the trip. 
        /// </param>
        /// <returns>
        /// A <see cref="DataSet"/> containing the results. 
        /// </returns>
        public DataSet GetSchedulesForTrip(string tripId, DateTime scheduledDateTime)
        {
            this.Parameters.Clear();

            // Set parameters            
            this.SoapAction = Urls.GetSchedulesForTrip;

            // Request data
            // Create data node
            var dict = new Dictionary<string, object>();
            dict["tripId"] = tripId;
            dict["scheduledDateTime"] = XmlConvert.ToString(scheduledDateTime, XmlDateTimeSerializationMode.Local);

            this.Parameters["GetSchedulesForTrip"] = Soap.BuildXmlFromDictionary(dict, this.SoapXmlNamespace);
            XmlDocument responseDoc = this.Request();

            return Soap.BuildDataSetFromSoapResponse(responseDoc);
        }

        /// <summary>
        /// Returns a dataset containing information about the specified stop such as location.
        /// </summary>
        /// <param name="stopNo">
        /// A TramTracker stop identifier. 
        /// </param>
        /// <returns>
        /// A <see cref="DataSet"/> containing the results. 
        /// </returns>
        public DataSet GetStopInformation(string stopNo)
        {
            this.Parameters.Clear();

            // Set parameters            
            this.SoapAction = Urls.GetStopInformation;

            // Request data
            // Create data node
            var dict = new Dictionary<string, object>();
            dict["stopNo"] = stopNo;

            this.Parameters["GetStopInformation"] = Soap.BuildXmlFromDictionary(dict, this.SoapXmlNamespace);
            XmlDocument responseDoc = this.Request();

            return Soap.BuildDataSetFromSoapResponse(responseDoc);
        }

        /// <summary>
        /// Returns a dataset containing all the routes and stops that have changed since the specified date. This function is very useful for caching.
        /// </summary>
        /// <param name="dateSince">
        /// The specified date and time. 
        /// </param>
        /// <returns>
        /// A <see cref="DataSet"/> containing the results. 
        /// </returns>
        public DataSet GetStopsAndRoutesUpdatesSince(DateTime dateSince)
        {
            this.Parameters.Clear();

            // Set parameters            
            this.SoapAction = Urls.GetStopsAndRoutesUpdatesSince;

            // Request data
            // Create data node
            var dict = new Dictionary<string, object>();
            dict["dateSince"] = XmlConvert.ToString(dateSince, XmlDateTimeSerializationMode.Local);

            this.Parameters["GetStopsAndRoutesUpdatesSince"] = Soap.BuildXmlFromDictionary(dict, this.SoapXmlNamespace);
            XmlDocument responseDoc = this.Request();

            return Soap.BuildDataSetFromSoapResponse(responseDoc);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generate the required header for the TramTracker web service.
        /// </summary>
        /// <returns>
        /// An <see cref="XmlNode"/> containing the required elements for a TramTracker request. 
        /// </returns>
        private XmlNode GenerateHeader()
        {
            /*
              Header Template:
             * <PidsClientHeader xmlns="http://www.yarratrams.com.au/pidsservice/">
                <ClientGuid>guid</ClientGuid>
                <ClientType>string</ClientType>
                <ClientVersion>string</ClientVersion>
                <ClientWebServiceVersion>string</ClientWebServiceVersion>
                <OSVersion>string</OSVersion>
                </PidsClientHeader>
             */
            var doc = new XmlDocument();

            XmlNode clientHeaderNode = doc.CreateElement("PidsClientHeader", this.SoapXmlNamespace);

            XmlNode clientGuidNode = doc.CreateElement("ClientGuid", this.SoapXmlNamespace);
            XmlNode clientTypeNode = doc.CreateElement("ClientType", this.SoapXmlNamespace);
            XmlNode clientVersionNode = doc.CreateElement("ClientVersion", this.SoapXmlNamespace);
            XmlNode clientWebServiceVersionNode = doc.CreateElement("ClientWebServiceVersion", this.SoapXmlNamespace);
            XmlNode operatingSystemVersionNode = doc.CreateElement("OSVersion", this.SoapXmlNamespace);

            clientGuidNode.InnerText = this.guid;
            clientTypeNode.InnerText = this.clientType;
            clientVersionNode.InnerText = this.clientVersion;
            clientWebServiceVersionNode.InnerText = this.clientWebServiceVersion;
            operatingSystemVersionNode.InnerText = this.operatingSystemVersion;

            doc.AppendChild(clientHeaderNode);
            clientHeaderNode.AppendChild(clientGuidNode);
            clientHeaderNode.AppendChild(clientTypeNode);
            clientHeaderNode.AppendChild(clientVersionNode);
            clientHeaderNode.AppendChild(clientWebServiceVersionNode);
            clientHeaderNode.AppendChild(operatingSystemVersionNode);

            return clientHeaderNode;
        }

        /// <summary>
        /// Requests the Tramtracker web service for a new client UUID.
        /// </summary>
        /// <returns>
        /// The new UUID. 
        /// </returns>
        private string GetNewGuid()
        {
            this.Parameters.Clear();

            // Set parameters            
            this.SoapAction = Urls.GetNewClientGuid;
            this.Parameters["GetNewClientGuid"] = null;

            // Request data
            XmlDocument doc = this.Request();

            // Get response data
            XmlNode response = doc["soap:Envelope"]["soap:Body"]["GetNewClientGuidResponse"];
            if (response == null)
            {
                throw new Exception("XML response is invalid.");
            }

            return response["GetNewClientGuidResult"].InnerText;
        }

        #endregion
    }
}