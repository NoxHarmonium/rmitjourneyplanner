using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    
    /// <summary>
    /// Abstract class that is inherited by classes who
    /// wish to request XML from a URL.
    /// </summary>
    abstract class XMLRequester
    {
        private string baseUrl = null;
        private bool escapeSpaces = true;
        private bool cachingEnabled = false;
        private Dictionary<String, object> parameters = new Dictionary<string,object>();
        private Dictionary<String, object> headerParameters = new Dictionary<string, object>();
        private RequestType requestType = RequestType.GET;
        private string xmlNamespace = "";
        private string soapAction = "";
       

        /// <summary>
        /// initializes the XMLRequester with a URL.
        /// </summary>
        /// <param name="baseUrl">The URL for the XML requst without the '?' symbol or parameters</param>
        public XMLRequester(string baseUrl)
        {
            this.baseUrl = baseUrl;
            //System.Net.ServicePointManager.Expect100Continue = false;
           
           
        }

        /// <summary>
        /// Gets or sets whether caching is enabled or not. 
        /// Caching can speed up XML requests.
        /// </summary>
        public bool CachingEnabled
        {
            get
            {
                return cachingEnabled;
            }
            set
            {
                cachingEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to escape spaces in the URL with a plus symbol as requred by the Google API.
        /// </summary>
        public bool EscapeSpaces
        {
            get
            {
                return escapeSpaces;
            }
            set
            {
                escapeSpaces = value;
            }


        }

        /// <summary>
        /// Gets or sets the key value pairs that will be sent in the XML request.
        /// </summary>
        protected Dictionary<String, object> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        /// <summary>
        /// Gets or sets the key value pairs of the SOAP header that will be sent in the request.
        /// </summary>
        protected Dictionary<String, object> HeaderParameters
        {
            get { return headerParameters; }
            set { headerParameters = value; }
        }

        /// <summary>
        /// Gets or sets the request type of the XMLRequester object.
        /// </summary>
        protected RequestType RequestType
        {
            get
            {
                return requestType;
            }
            set
            {
                requestType = value;
            }
        }

        /// <summary>
        /// Gets or sets the XML namespace used in SOAP requests.
        /// </summary>
        protected string SoapXmlNamespace
        {
            get
            {
                return xmlNamespace;
            }
            set
            {
                xmlNamespace = value;
            }
        }

        /// <summary>
        /// Gets or sets the SoapAction parameter send in SOAP requests.
        /// </summary>
        protected string SoapAction
        {
            get
            {
                return soapAction;
            }
            set
            {
                soapAction = value;
            }
        }

   

        /// <summary>
        /// Sends the XML request with the specified parameters and returns the result.
        /// </summary>
        /// <returns></returns>
        protected virtual XmlDocument Request()
        {
            if (requestType == RequestType.GET)
            {
                // Create request URL from parameters
                string requestURL = baseUrl + "?";
                foreach (KeyValuePair<String, object> kvp in parameters)
                {
                    string value = escapeSpaces ? kvp.Value as string : ((string)kvp.Value).Replace(" ", "+");
                    requestURL += kvp.Key + "=" + value + "&";
                }


                HttpWebRequest request = HttpWebRequest.Create(requestURL) as HttpWebRequest;
                request.Proxy = new WebProxy("http://aproxy.rmit.edu.au:8080", false, new string[] { }, new NetworkCredential("s3229159", "MuchosRowlies1"));
                
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                //Read response stream into xml object
                XmlDocument doc = new XmlDocument();
                Stream responseStream = response.GetResponseStream();
                doc.Load(responseStream);
                responseStream.Close();                         
                return doc;
            }
            else if (requestType == RequestType.SOAP)
            {
                XmlDocument doc = SOAP.GetSoapTemplate();

                if (HeaderParameters.Count > 0)
                {
                    XmlNode headerNode = doc["soap:Envelope"]["soap:Header"];
                    foreach (KeyValuePair<String, object> kvp in HeaderParameters)
                    {
                        XmlNode headerElement = doc.CreateNode(XmlNodeType.Element, kvp.Key, xmlNamespace);
                        if (kvp.Value != null)
                        {
                            XmlNode value = kvp.Value as XmlNode;
                            foreach (XmlNode node in value.ChildNodes)
                            {
                                XmlNode newNode = doc.ImportNode(node, true);
                                headerElement.AppendChild(newNode);
                            }
                           
                        }
                        headerNode.AppendChild(headerElement);
                    }                   
                }
                
                if (Parameters.Count > 0)
                {
                    XmlNode bodyNode = doc["soap:Envelope"]["soap:Body"];
                    foreach (KeyValuePair<String, object> kvp in Parameters)
                    {
                        XmlNode bodyElement = doc.CreateNode(XmlNodeType.Element, kvp.Key, xmlNamespace);
                        if (kvp.Value != null)
                        {
                            XmlNode value = kvp.Value as XmlNode;
                            foreach (XmlNode node in value.ChildNodes)
                            {
                                XmlNode newNode = doc.ImportNode(node, true);
                                bodyElement.AppendChild(newNode);
                            }

                        }                        
                        bodyNode.AppendChild(bodyElement);
                        
                    }
                }

                HttpWebRequest request = HttpWebRequest.Create(baseUrl) as HttpWebRequest;
                request.ContentType = "text/xml; charset=utf-8";
                request.Method = "POST";
                request.Accept = "text/xml";
                request.Headers.Add("SOAPAction", soapAction);
                request.Proxy = new WebProxy("http://aproxy.rmit.edu.au:8080", false, new string[] { }, new NetworkCredential("s3229159", "MuchosRowlies1"));

                Stream requestStream = request.GetRequestStream();
                

                doc.Save(requestStream);
                requestStream.Flush();
                requestStream.Close();

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                

                //Read response stream into xml object
                doc = new XmlDocument();
                Stream responseStream = response.GetResponseStream();
                doc.Load(responseStream);
                responseStream.Close();
                return doc;

            }
            else
            {
                throw new ArgumentException("Incorrect request type: " + requestType.ToString());
            }
        }

        


    }
}
