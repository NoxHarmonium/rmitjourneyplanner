// -----------------------------------------------------------------------
// <copyright file="TramTrackerAPI.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Data;
    using System.IO;
    /// <summary>
    /// Interfaces with the Yarra Trams Tramtracker SOAP webservice to get various data.
    /// </summary>
    public class TramTrackerAPI : XMLRequester
    {
        /// <summary>
        /// The regular expression that defines the correct format of a client UUID.
        /// </summary>
        private const string uuidFormat = "[a-zA-Z0-9]{8}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{12}";
        
        private string guid;
        private string clientType = "TRAMHUNTER";
        private string clientVersion = "0.0.0.1";
        private string clientWebServiceVersion = "6.4.0.0";
        private string osVersion = Environment.OSVersion.VersionString;

        /// <summary>
        /// Initializes a new instance of the TramTrackerAPI class.
        /// </summary>
        public TramTrackerAPI() : base (Urls.TramTrackerUrl)
        {
            base.RequestType = RequestType.SOAP;
            base.SoapXmlNamespace = Urls.TramTrackerNameSpace;
            this.guid = getNewGuid();
            this.HeaderParameters["PidsClientHeader"] = generateHeader();
            
        }

        /// <summary>
        /// Returns true if the supplied string is in the correct format for a client UUID.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public bool IsValidUuid(string uuid)
        {
            return new Regex(uuidFormat).IsMatch(uuid);
        }


       

        /// <summary>
        /// Gets a dataset containing the end-points of each tram route.
        /// </summary>
        /// <returns></returns>
        public DataSet GetDestinationsForAllRoutes()
        {
                      
            this.Parameters.Clear();
            //Set parameters           
            base.SoapAction = Urls.GetDestinationsForAllRoutes;
            this.Parameters["GetDestinationsForAllRoutes"] = null;
            XmlDocument responseDoc = base.Request();
            
            XmlNode response = responseDoc["soap:Envelope"]["soap:Body"]["GetDestinationsForAllRoutesResponse"];
            string validationResult = response["validationResult"].InnerText;
            if (validationResult.Contains("Request Denied:"))
            {
                throw new Exception("Tramtracker webservice error:\n" + validationResult);
            }
            
                  
            DataSet results = new DataSet();
            results.ReadXml(new StringReader(response.FirstChild.OuterXml),XmlReadMode.ReadSchema);
            results.ReadXml(new StringReader(response.FirstChild.OuterXml), XmlReadMode.DiffGram);
            return results;


        }

        /// <summary>
        /// Gets the 2 end-points for a specific tram route.
        /// </summary>        
        /// <param name="routeId"></param>
        /// <returns></returns>
        public DataSet GetDestinationsForRoute(string routeId)
        {
            this.Parameters.Clear();
            //Set parameters           
            base.SoapAction = Urls.GetDestinationsForRoute;

            //Create data node
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["routeNo"] = routeId;

           
            XmlNode nGetDestinationsForRoute = SOAP.BuildXmlFromDictionary(dict, SoapXmlNamespace);
            this.Parameters["GetDestinationsForRoute"] = SOAP.BuildXmlFromDictionary(dict, SoapXmlNamespace);
            XmlDocument responseDoc = base.Request();

            return SOAP.BuildDataSetFromSoapResponse(responseDoc);          
        }

        /// <summary>
        /// Gets a dataset containing a list of all the stops pertaining to the supplied parameters.
        /// </summary>
        /// <param name="routeId"></param>
        /// <param name="isUpDirection"></param>
        /// <returns></returns>
        public DataSet GetListOfStopsByRouteNoAndDirection(string routeId, Boolean isUpDirection)
        {
            this.Parameters.Clear();
            //Set parameters           
            base.SoapAction = Urls.GetListOfStopsByRouteNoAndDirection;

            //Create data node
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["routeNo"] = routeId;
            dict["isUpDirection"] = isUpDirection.ToString().ToLower();

            XmlNode ParameterNodes = SOAP.BuildXmlFromDictionary(dict, SoapXmlNamespace);
            this.Parameters["GetListOfStopsByRouteNoAndDirection"] = SOAP.BuildXmlFromDictionary(dict, SoapXmlNamespace);
            XmlDocument responseDoc = base.Request();

            return SOAP.BuildDataSetFromSoapResponse(responseDoc);                  
        }


        /// <summary>
        /// Gets a dataset containing a list of all main routes in the tram network.
        /// </summary>
        /// <returns></returns>
        public DataSet GetMainRoutes()
        {

            this.Parameters.Clear();
            //Set parameters           
            base.SoapAction = Urls.GetMainRoutes;
            this.Parameters["GetMainRoutes"] = null;
            XmlDocument responseDoc = base.Request();


            return SOAP.BuildDataSetFromSoapResponse(responseDoc);


        }

        /// <summary>
        /// Gets a dataset containing all the main routes that intersect the supplied stop number.
        /// </summary>
        /// <param name="stopNo"></param>
        /// <returns></returns>
        public DataSet GetMainRoutesForStop(string stopNo)
        {
            this.Parameters.Clear();
            //Set parameters           
            base.SoapAction = Urls.GetMainRoutesForStop;
            
            //Create data node
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["stopNo"] = stopNo;

            this.Parameters["GetMainRoutesForStop"] = SOAP.BuildXmlFromDictionary(dict,SoapXmlNamespace);
            XmlDocument responseDoc = base.Request();


            return SOAP.BuildDataSetFromSoapResponse(responseDoc);          
        }
        
        /// <summary>
        /// Requests the Tramtracker webservice for a new client UUID.
        /// </summary>
        /// <returns></returns>
        private string getNewGuid()
        {
            this.Parameters.Clear();
            //Set parameters            
            base.SoapAction = Urls.GetNewClientGuid;
            base.Parameters["GetNewClientGuid"] = null;
            //Request data
            XmlDocument doc = base.Request();
            
            //Get response data
            XmlNode response = doc["soap:Envelope"]["soap:Body"]["GetNewClientGuidResponse"];
            return response["GetNewClientGuidResult"].InnerText;
        }

        /// <summary>
        /// Gets a dataset containing the predicted arrival time of the supplied tram number.
        /// </summary>
        /// <param name="tramNo"></param>
        /// <returns></returns>
        public DataSet GetNextPredictedArivalTime(string tramNo)
        {
            this.Parameters.Clear();
            //Set parameters            
            base.SoapAction = Urls.GetNextPredictedArivalTimeAtStopsForTramNo;
            
            //Request data
            //Create data node
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["tramNo"] = tramNo;

            this.Parameters["GetNextPredictedArivalTimeAtStopsForTramNo"] = SOAP.BuildXmlFromDictionary(dict, SoapXmlNamespace);
            XmlDocument responseDoc = base.Request();


            return SOAP.BuildDataSetFromSoapResponse(responseDoc);    

        }

        /// <summary>
        /// Gets a dataset containing a collection of the next predicted trams for the supplied stop and route.
        /// </summary>
        /// <param name="tramNo"></param>
        /// <returns></returns>
        public DataSet GetNextPredictedRoutesCollection(string stopNo,string routeNo, bool lowFloor)
        {
            this.Parameters.Clear();
            //Set parameters            
            base.SoapAction = Urls.GetNextPredictedRoutesCollection;

            //Request data
            //Create data node
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["stopNo"] = stopNo;
            dict["routeNo"] = routeNo;
            dict["lowFloor"] = lowFloor.ToString().ToLower();

            this.Parameters["GetNextPredictedRoutesCollection"] = SOAP.BuildXmlFromDictionary(dict, SoapXmlNamespace);
            XmlDocument responseDoc = base.Request();


            return SOAP.BuildDataSetFromSoapResponse(responseDoc);

        }


        /// <summary>
        /// Gets a dataset containing a collection of schedules for the supplied stop and route.
        /// </summary>
        /// <param name="tramNo"></param>
        /// <returns></returns>
        public DataSet GetSchedulesCollection(string stopNo, string routeNo, bool lowFloor)
        {
            this.Parameters.Clear();
            //Set parameters            
            base.SoapAction = Urls.GetSchedulesCollection;

            //Request data
            //Create data node
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["stopNo"] = stopNo;
            dict["routeNo"] = routeNo;
            dict["lowFloor"] = lowFloor.ToString().ToLower();

            this.Parameters["GetSchedulesCollection"] = SOAP.BuildXmlFromDictionary(dict, SoapXmlNamespace);
            XmlDocument responseDoc = base.Request();


            return SOAP.BuildDataSetFromSoapResponse(responseDoc);

        }

        /// <summary>
        /// Gets a dataset containing the schedules for the specified trip ID.
        /// </summary>
        /// <param name="tripID"></param>
        /// <param name="scheduledDateTime"></param>
        /// <returns></returns>
        public DataSet GetSchedulesForTrip(string tripID, DateTime scheduledDateTime)
        {
            this.Parameters.Clear();
            //Set parameters            
            base.SoapAction = Urls.GetSchedulesForTrip;

            //Request data
            //Create data node
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["tripID"] = tripID;
            dict["scheduledDateTime"] = scheduledDateTime.ToString();


            this.Parameters["GetSchedulesForTrip"] = SOAP.BuildXmlFromDictionary(dict, SoapXmlNamespace);
            XmlDocument responseDoc = base.Request();


            return SOAP.BuildDataSetFromSoapResponse(responseDoc);

        }

        /// <summary>
        /// Returns a dataset containing information about the specified stop such as location.
        /// </summary>
        /// <param name="stopNo"></param>
        /// <returns></returns>
        public DataSet GetStopInformation(string stopNo)
        {
            this.Parameters.Clear();
            //Set parameters            
            base.SoapAction = Urls.GetStopInformation;

            //Request data
            //Create data node
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["stopNo"] = stopNo;

            this.Parameters["GetStopInformation"] = SOAP.BuildXmlFromDictionary(dict, SoapXmlNamespace);
            XmlDocument responseDoc = base.Request();


            return SOAP.BuildDataSetFromSoapResponse(responseDoc);

        }

        /// <summary>
        /// Returns a dataset containing all the routes and stops that have changed
        /// since the specified date. This function is very useful for caching.
        /// </summary>
        /// <param name="dateSince"></param>
        /// <returns></returns>
        public DataSet GetStopsAndRouteUpdatesSince(DateTime dateSince)
        {
            this.Parameters.Clear();
            //Set parameters            
            base.SoapAction = Urls.GetStopsAndRouteUpdatesSince;

            //Request data
            //Create data node
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["dateSince"] = dateSince.ToString();

            this.Parameters["GetStopsAndRouteUpdatesSince"] = SOAP.BuildXmlFromDictionary(dict, SoapXmlNamespace);
            XmlDocument responseDoc = base.Request();


            return SOAP.BuildDataSetFromSoapResponse(responseDoc);

        }
        
        /// <summary>
        /// Gets the client GUID generated by the TramTracker webservice.
        /// </summary>
        public string Guid
        {
            get
            {
                return guid;
            }
        }

        /// <summary>
        /// Gets or sets the client type reported to the Tramtracker webservice.
        /// </summary>
        public string ClientType
        {
            get
            {
                return clientType;
            }
            set
            {
                clientType = value;
                this.HeaderParameters["PidsClientHeader"] = generateHeader();
            }
        }

        /// <summary>
        /// Gets or sets the client version reported to the Tramtracker webservice.
        /// </summary>
        public string ClientVersion
        {
            get
            {
                return clientVersion;
            }
            set
            {
                clientVersion = value;
                this.HeaderParameters["PidsClientHeader"] = generateHeader();
            }
        }

        /// <summary>
        /// Gets or sets the web service version reported to the Tramtracker webservice. 
        /// This shouldn't need to be changed off the default.
        /// </summary>
        public string ClientWebServiceVersion
        {
            get
            {
                return clientWebServiceVersion;
            }
            set
            {
                clientWebServiceVersion = value;
                this.HeaderParameters["PidsClientHeader"] = generateHeader();
            }
        }

        /// <summary>
        /// Gets or sets the operating system version that is reported to the Tramtracker webservice.
        /// This is usually autodetected and doesn't need to be changed off the default.
        /// </summary>
        public string ClientOSVersion
        {
            get
            {
                return osVersion;
            }
            set
            {
                osVersion = value;
                this.HeaderParameters["PidsClientHeader"] = generateHeader();
            }

        }

        /// <summary>
        /// Generate the required header for the Tramtracker webservice.
        /// </summary>
        /// <returns></returns>
        private XmlNode generateHeader()
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
            XmlDocument doc = new XmlDocument();

            XmlNode nClientHeader = doc.CreateElement("PidsClientHeader", base.SoapXmlNamespace);
            
            XmlNode nClientGuid = doc.CreateElement("ClientGuid",base.SoapXmlNamespace);
            XmlNode nClientType = doc.CreateElement("ClientType", base.SoapXmlNamespace);
            XmlNode nClientVersion = doc.CreateElement("ClientVersion", base.SoapXmlNamespace);
            XmlNode nClientWebServiceVersion = doc.CreateElement("ClientWebServiceVersion", base.SoapXmlNamespace);
            XmlNode nOsVersion = doc.CreateElement("OSVersion", base.SoapXmlNamespace);
            
            nClientGuid.InnerText= guid;
            nClientType.InnerText = clientType;
            nClientVersion.InnerText = clientVersion;
            nClientWebServiceVersion.InnerText = clientWebServiceVersion;
            nOsVersion.InnerText = osVersion;

            doc.AppendChild(nClientHeader);
            nClientHeader.AppendChild(nClientGuid);
            nClientHeader.AppendChild(nClientType);
            nClientHeader.AppendChild(nClientVersion);
            nClientHeader.AppendChild(nClientWebServiceVersion);
            nClientHeader.AppendChild(nOsVersion);

            return nClientHeader;
        }

        

    }
}
