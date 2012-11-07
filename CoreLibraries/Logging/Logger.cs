// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   The handler for log events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Logging
{
    /// <summary>
    /// The handler for log events.
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="message">
    /// </param>
    public delegate void LogEventHandler(object sender, string message);

    /// <summary>
    /// The handler for progress update events.
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="progress">
    /// </param>
    public delegate void ProgressEventHandler(object sender, int progress);

    /// <summary>
    /// Static class to be used a universal hook into the logging system.
    /// </summary>
    public static class Logger
    {
        #region Public Events

        /// <summary>
        ///   Triggered when the Log method is called.
        /// </summary>
        public static event LogEventHandler LogEvent;

        /// <summary>
        ///   Triggered when the UpdateProgress method is called.
        /// </summary>
        public static event ProgressEventHandler ProgressEvent;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Trigger a log event causing the message to be recieved by all registered objects.
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="message">
        /// </param>
        public static void Log(object sender, string message)
        {
            if (LogEvent != null)
            {
                LogEvent(sender, message);
            }
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <param name="list">
        /// The list. 
        /// </param>
        public static void Log(object sender, string message, params object[] list)
        {
            if (LogEvent != null)
            {
                LogEvent(sender, string.Format(message, list));
            }
        }

        /// <summary>
        /// Trigger a progress update event causing a message to be recieved by all registered objects.
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="progress">
        /// </param>
        public static void UpdateProgress(object sender, int progress)
        {
            if (ProgressEvent != null)
            {
                ProgressEvent(sender, progress);
            }
        }

        #endregion
    }
}