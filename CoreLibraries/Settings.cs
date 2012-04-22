﻿// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Xml;

    #endregion

    /// <summary>
    ///   A static class that shares variables around the class library. Used for things such as loggers and should be used sparingly.
    /// </summary>
    public static class Settings
    {
        #region Constants and Fields

        /// <summary>
        ///   The dictionary that faciliates InternalEntries.
        /// </summary>
        private static readonly Dictionary<string, string> InternalEntries;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref="Settings" /> class.
        /// </summary>
        static Settings()
        {
            InternalEntries = new Dictionary<string, string>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("./Settings.xml");
                foreach (XmlNode child in doc["settings"].ChildNodes)
                {
                    string name = child["name"].InnerText;
                    string value = child["value"].InnerText;
                    InternalEntries.Add(name, value);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Unable to read settings file. Make sure that Settings.xml is found in the same directory as this assembly.",e);
            }
            
            
        
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the dictionary that faciliates InternalEntries.
        /// </summary>
        public static Dictionary<string, string> Entries
        {
            get
            {
                return InternalEntries;
            }
        }

        #endregion
    }
}