// -----------------------------------------------------------------------
// <copyright file="Tools.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Xml;
    /// <summary>
    /// Caching tools.
    /// </summary>
    public class CacheTools
    {
        /// <summary>
        /// De escapes an XML string.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string Deescape(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;


            string returnString = s;

            returnString = returnString.Replace("&apos;", "'");

            returnString = returnString.Replace("&quot;", "\"");

            returnString = returnString.Replace("&gt;", ">");

            returnString = returnString.Replace("&lt;", "<");

            //returnString = returnString.Replace("&amp;", "&");



            return returnString;
        }

    }
}
