// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CallLog.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Records the calling methods of a given method.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Logging
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    #endregion

    /// <summary>
    /// Records the calling methods of a given method.
    /// </summary>
    public class CallLog
    {
        #region Constants and Fields

        /// <summary>
        ///   The dictionary that holds all the call data.
        /// </summary>
        private readonly Dictionary<string, int> dict = new Dictionary<string, int>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Log the calling method of the method that calls this method.
        /// </summary>
        public void Log()
        {
            var st = new StackTrace(true);

            string name = string.Format(
                "{0}({1}).{2}({3}).{4}({5}).{6}({7}).{8}({9}).{10}({11}).{12}({13})", 
                st.GetFrame(8).GetMethod().Name, 
                st.GetFrame(8).GetFileLineNumber(), 
                st.GetFrame(7).GetMethod().Name, 
                st.GetFrame(7).GetFileLineNumber(), 
                st.GetFrame(6).GetMethod().Name, 
                st.GetFrame(6).GetFileLineNumber(), 
                st.GetFrame(5).GetMethod().Name, 
                st.GetFrame(5).GetFileLineNumber(), 
                st.GetFrame(4).GetMethod().Name, 
                st.GetFrame(4).GetFileLineNumber(), 
                st.GetFrame(3).GetMethod().Name, 
                st.GetFrame(3).GetFileLineNumber(), 
                st.GetFrame(2).GetMethod().Name, 
                st.GetFrame(2).GetFileLineNumber());

            if (!this.dict.ContainsKey(name))
            {
                this.dict[name] = 0;
            }

            this.dict[name]++;
        }

        /// <summary>
        /// Returns the recorded values.
        /// </summary>
        /// <returns>
        /// The to string. 
        /// </returns>
        public override string ToString()
        {
            return this.dict.Aggregate(
                string.Empty, 
                (current, kvp) => current + (kvp.Key + ": " + kvp.Value.ToString(CultureInfo.InvariantCulture) + "\n"));
        }

        #endregion
    }
}