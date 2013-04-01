// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SOAP.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Contains tools to interface with Soap web services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Xml;

    #endregion

    /// <summary>
    /// Contains tools to interface with Soap web services.
    /// </summary>
    internal static class Soap
    {
        #region Constants and Fields

        /// <summary>
        ///   A sample XML soap template that was taken from the Tramtracker web service.
        /// </summary>
        private const string SoapTemplate =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
            +
            "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">"
            + "   <soap:Header> " + "   </soap:Header>" + "   <soap:Body>" + "   </soap:Body>" + "</soap:Envelope>";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The build data set from SOAP response.
        /// </summary>
        /// <param name="responseDoc">
        /// The response XML document from the SOAP request. 
        /// </param>
        /// <returns>
        /// A DataSet loaded from the supplied XML. 
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception when Tramtracker sends an error message.
        /// </exception>
        public static DataSet BuildDataSetFromSoapResponse(XmlDocument responseDoc)
        {
            try
            {
                XmlNode response = responseDoc["soap:Envelope"]["soap:Body"].FirstChild;

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
        /// Builds a set of XML nodes from a dictionary pair of named keys and values. Each key corresponds to a node and each value corresponds to the inner text of that node.
        /// </summary>
        /// <param name="dict">
        /// A dictionary from which to build the XML off. 
        /// </param>
        /// <param name="xmlNamespace">
        /// The namespace of the XML. 
        /// </param>
        /// <returns>
        /// An <see cref="XmlNode"/> representing the supplied dictionary. 
        /// </returns>
        public static XmlNode BuildXmlFromDictionary(Dictionary<string, object> dict, string xmlNamespace)
        {
            var doc = new XmlDocument();
            XmlNode bodyNode = doc.CreateElement("XMLData", xmlNamespace);
            doc.AppendChild(bodyNode);
            foreach (var kvp in dict)
            {
                XmlNode bodyElement = doc.CreateNode(XmlNodeType.Element, kvp.Key, xmlNamespace);
                if (kvp.Value != null)
                {
                    bodyElement.InnerText = kvp.Value.ToString();
                }

                bodyNode.AppendChild(bodyElement);
            }

            return doc.FirstChild;
        }

        /// <summary>
        /// Gets the XML template for a soap request.
        /// </summary>
        /// <returns>
        /// Returns an XML template to form the basis of a SOAP request. 
        /// </returns>
        public static XmlDocument GetSoapTemplate()
        {
            var doc = new XmlDocument();
            doc.LoadXml(SoapTemplate);
            return doc;
        }

        #endregion
    }
}