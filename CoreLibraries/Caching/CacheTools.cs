// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="CacheTools.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Caching tools.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Caching
{
    /// <summary>
    /// Caching tools.
    /// </summary>
    internal class CacheTools
    {
        #region Public Methods

        /// <summary>
        /// De escapes an XML string.
        /// </summary>
        /// <param name="xmlString">
        /// An XML string to escape.
        /// </param>
        /// <returns>
        /// A correctly escaped XML string.
        /// </returns>
        /// <remarks>
        /// When storing XML in a database there are certain symbols that will
        ///   be intepreted as SQL code. This method replaces dangerous symbols such
        ///   as quotation marks with a special symbols that are safe to store in a database.
        ///   If you want to use the XML code after retrieving it from the database you
        ///   may have to de-escape it.
        /// </remarks>
        public static string Deescape(string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString))
            {
                return xmlString;
            }

            string returnString = xmlString;

            // Replace dangerous tokens.
            returnString = returnString.Replace("&apos;", "'");
            returnString = returnString.Replace("&quot;", "\"");
            returnString = returnString.Replace("&gt;", ">");
            returnString = returnString.Replace("&lt;", "<");

            return returnString;
        }

        #endregion
    }
}