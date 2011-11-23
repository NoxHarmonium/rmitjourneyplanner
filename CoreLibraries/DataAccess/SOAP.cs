// -----------------------------------------------------------------------
// <copyright file="SOAP.cs" company="RMIT University">
// RMIT Travel Planner
// By Sean Dawson
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Data;
    using System.IO;

    /// <summary>
    /// Contains tools to interface with SOAP webservices.
    /// </summary>
    public static class SOAP
    {
        /// <summary>
        /// A sample XML soap template that was taken from the Tramtracker webservice.
        /// </summary>
        private const string soapTemplate = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                            "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                                            "   <soap:Header> " +
                                            "   </soap:Header>" +
                                            "   <soap:Body>" +
                                            "   </soap:Body>" +
                                            "</soap:Envelope>";

       
        /// <summary>
        /// Gets the XML template for a soap request.
        /// </summary>
        /// <returns></returns>
        public static XmlDocument GetSoapTemplate()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(soapTemplate);
            return doc;
        }

        /// <summary>
        /// Builds a set of XML nodes from a dictionary pair of named keys and values. 
        /// Each key corresponds to a node and each value corresponds to the inner text of that node.
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="xmlNamespace"></param>
        /// <returns></returns>
        public static XmlNode BuildXmlFromDictionary(Dictionary<String, object> dict, string xmlNamespace)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode bodyNode = doc.CreateElement("XMLData",xmlNamespace);
            doc.AppendChild(bodyNode);
            foreach (KeyValuePair<String, object> kvp in dict)
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

        public static DataSet BuildDataSetFromSoapResponse(XmlDocument responseDoc)
        {
            XmlNode response = responseDoc["soap:Envelope"]["soap:Body"].FirstChild;
            string validationResult = response["validationResult"].InnerText;
            if (validationResult.Contains("Request Denied:"))
            {
                throw new Exception("Tramtracker webservice error:\n" + validationResult);
            }


            DataSet results = new DataSet();
            results.ReadXml(new StringReader(response.FirstChild.OuterXml), XmlReadMode.ReadSchema);
            results.ReadXml(new StringReader(response.FirstChild.OuterXml), XmlReadMode.DiffGram);

            return results;
        }

       
    }
}
