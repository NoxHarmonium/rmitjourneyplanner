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
    public abstract class XMLRequester
    {
        private string baseUrl = null;
        private bool escapeSpaces = true;
        private bool cachingEnabled = false;
        private Dictionary<String, String> parameters = new Dictionary<string,string>();

        

        /// <summary>
        /// initializes the XMLRequester with a URL.
        /// </summary>
        /// <param name="baseUrl">The URL for the XML requst without the '?' symbol or parameters</param>
        public XMLRequester(string baseUrl)
        {
            this.baseUrl = baseUrl;
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
        public Dictionary<String, String> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        /// <summary>
        /// Sends the XML request with the specified parameters and returns the result.
        /// </summary>
        /// <returns></returns>
        protected virtual XmlDocument Request()
        {
            // Create request URL from parameters
            string requestURL = baseUrl + "?";
            foreach (KeyValuePair<String, String> kvp in parameters)
            {
                string value = escapeSpaces ? kvp.Value : kvp.Value.Replace(" ","+");
                requestURL += kvp.Key + "=" + value + "&";
            }

            
            HttpWebRequest request = HttpWebRequest.Create(requestURL) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            
            //Read response stream into xml object
            XmlDocument doc = new XmlDocument();
            using (StreamReader r = new StreamReader(response.GetResponseStream()))
            {                
                doc.LoadXml(r.ReadToEnd());
            }

            return doc; 
        }


    }
}
