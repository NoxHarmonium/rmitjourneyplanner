// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XMLRequester.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Abstract class that is inherited by classes that wish to request XML from a URL.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Net;
    using System.Xml;

    #endregion

    /// <summary>
    /// Abstract class that is inherited by classes that wish to request XML from a URL.
    /// </summary>
    internal abstract class XmlRequester
    {
        #region Constants and Fields

        /// <summary>
        ///   The base url.
        /// </summary>
        private readonly string baseUrl;

        /// <summary>
        ///   The <see cref = "WebProxy" /> object to use with the request.
        /// </summary>
        private readonly WebProxy proxy;

        /// <summary>
        ///   Determines whether spaces are escaped or not.
        /// </summary>
        private bool escapeSpaces = true;

        /// <summary>
        ///   The header parameters.
        /// </summary>
        private Dictionary<string, object> headerParameters = new Dictionary<string, object>();

        /// <summary>
        ///   The parameters.
        /// </summary>
        private Dictionary<string, object> parameters = new Dictionary<string, object>();

        /// <summary>
        ///   The request type.
        /// </summary>
        private RequestType requestType = RequestType.Get;

        /// <summary>
        ///   The SOAP action.
        /// </summary>
        private string soapAction = string.Empty;

        /// <summary>
        ///   The XML namespace.
        /// </summary>
        private string xmlNamespace = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRequester"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The URL for the XML request without the '?' symbol or parameters. 
        /// </param>
        protected XmlRequester(string baseUrl)
        {
            this.baseUrl = baseUrl;

            // System.Net.ServicePointManager.Expect100Continue = false;
            this.proxy = ConnectionInfo.Proxy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRequester"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// TThe URL for the XML request without the '?' symbol or parameters. 
        /// </param>
        /// <param name="proxy">
        /// The WebProxy object to use with the web request. 
        /// </param>
        protected XmlRequester(string baseUrl, WebProxy proxy)
        {
            this.baseUrl = baseUrl;
            this.proxy = proxy;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets a value indicating whether caching is enabled or not. Caching can speed up XML requests.
        /// </summary>
        public bool CachingEnabled { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether to escape spaces in the URL with a plus symbol as required by the Google API.
        /// </summary>
        public bool EscapeSpaces
        {
            get
            {
                return this.escapeSpaces;
            }

            set
            {
                this.escapeSpaces = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the key value pairs of the Soap header that will be sent in the request.
        /// </summary>
        protected Dictionary<string, object> HeaderParameters
        {
            get
            {
                return this.headerParameters;
            }

            set
            {
                this.headerParameters = value;
            }
        }

        /// <summary>
        ///   Gets or sets the key value pairs that will be sent in the XML request.
        /// </summary>
        protected Dictionary<string, object> Parameters
        {
            get
            {
                return this.parameters;
            }

            set
            {
                this.parameters = value;
            }
        }

        /// <summary>
        ///   Gets or sets the request type of the <see cref = "XmlRequester" /> object.
        /// </summary>
        protected RequestType RequestType
        {
            get
            {
                return this.requestType;
            }

            set
            {
                this.requestType = value;
            }
        }

        /// <summary>
        ///   Gets or sets the SoapAction parameter send in SOAP requests.
        /// </summary>
        protected string SoapAction
        {
            get
            {
                return this.soapAction;
            }

            set
            {
                this.soapAction = value;
            }
        }

        /// <summary>
        ///   Gets or sets the XML namespace used in SOAP requests.
        /// </summary>
        protected string SoapXmlNamespace
        {
            get
            {
                return this.xmlNamespace;
            }

            set
            {
                this.xmlNamespace = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends the XML request with the specified parameters and returns the result.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="XmlDocument"/> containing the results of the request. 
        /// </returns>
        protected virtual XmlDocument Request()
        {
            if (this.requestType == RequestType.Get)
            {
                // Create request URL from parameters
                string requestUrl = this.baseUrl + "?";
                foreach (var kvp in this.parameters)
                {
                    string value = this.escapeSpaces ? kvp.Value as string : ((string)kvp.Value).Replace(" ", "+");
                    requestUrl += kvp.Key + "=" + value + "&";
                }

                var request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Proxy = this.proxy;
                var response = request.GetResponse() as HttpWebResponse;

                // Read response stream into xml object
                var doc = new XmlDocument();

                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        throw new NoNullAllowedException("The response stream was null");
                    }

                    doc.Load(responseStream);
                    responseStream.Close();
                    return doc;
                }
            }

            if (this.requestType == RequestType.Soap)
            {
                XmlDocument doc = Soap.GetSoapTemplate();

                if (this.HeaderParameters.Count > 0)
                {
                    XmlNode headerNode = doc["soap:Envelope"]["soap:Header"];
                    foreach (var kvp in this.HeaderParameters)
                    {
                        XmlNode headerElement = doc.CreateNode(XmlNodeType.Element, kvp.Key, this.xmlNamespace);
                        if (kvp.Value != null)
                        {
                            var value = kvp.Value as XmlNode;
                            foreach (XmlNode node in value.ChildNodes)
                            {
                                XmlNode newNode = doc.ImportNode(node, true);
                                headerElement.AppendChild(newNode);
                            }
                        }

                        headerNode.AppendChild(headerElement);
                    }
                }

                if (this.Parameters.Count > 0)
                {
                    XmlNode bodyNode = doc["soap:Envelope"]["soap:Body"];
                    foreach (var kvp in this.Parameters)
                    {
                        XmlNode bodyElement = doc.CreateNode(XmlNodeType.Element, kvp.Key, this.xmlNamespace);
                        if (kvp.Value != null)
                        {
                            var value = kvp.Value as XmlNode;

                            foreach (XmlNode node in value.ChildNodes)
                            {
                                XmlNode newNode = doc.ImportNode(node, true);
                                bodyElement.AppendChild(newNode);
                            }
                        }

                        bodyNode.AppendChild(bodyElement);
                    }
                }

                var request = WebRequest.Create(this.baseUrl) as HttpWebRequest;
                request.ContentType = "text/xml; charset=utf-8";
                request.Method = "POST";
                request.Accept = "text/xml";
                request.Headers.Add("SOAPAction", this.soapAction);
                request.Proxy = this.proxy;

                using (Stream requestStream = request.GetRequestStream())
                {
                    doc.Save(requestStream);
                    requestStream.Flush();
                    requestStream.Close();

                    var response = request.GetResponse() as HttpWebResponse;

                    // Read response stream into xml object
                    doc = new XmlDocument();
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null)
                        {
                            throw new NoNullAllowedException("The response stream was null");
                        }

                        doc.Load(responseStream);
                        responseStream.Close();
                        return doc;
                    }
                }
            }

            throw new ArgumentException("Incorrect request type: " + this.requestType.ToString());
        }

        #endregion
    }
}