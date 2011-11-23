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

    /// <summary>
    /// Contains tools to interface with SOAP webservices.
    /// </summary>
    public static class SOAP
    {
        private const string soapTemplate = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                            "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                                            "   <soap:Header> " +
                                            "   </soap:Header>" +
                                            "   <soap:Body>" +
                                            "   </soap:Body>" +
                                            "</soap:Envelope>";

       
        public static XmlDocument GetSoapTemplate()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(soapTemplate);
            return doc;
        }

       
    }
}
