﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="GoogleApiException.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Represents an error returned from the Google Maps API.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    #region

    using System;

    #endregion

    /// <summary>
    /// Represents an error returned from the Google Maps API.
    /// </summary>
    internal class GoogleApiException : Exception
    {
        #region Constants and Fields

        /// <summary>
        ///   The error code.
        /// </summary>
        private readonly string errorCode;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the GoogleApiException class from the supplied error code.
        /// </summary>
        /// <param name="errorCode">
        /// The error code recieved from the Google Maps API.
        /// </param>
        public GoogleApiException(string errorCode)
            : base(GetDescription(errorCode))
        {
            this.errorCode = errorCode;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the raw error code returned by the Google Maps API.
        /// </summary>
        public string ErrorCode
        {
            get
            {
                return this.errorCode;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts the Google Maps API error codes into a human readable message.
        /// </summary>
        /// <param name="errorCode">
        /// The string recieved from the Google Maps API.
        /// </param>
        /// <returns>
        /// A string containing a description of the error code.
        /// </returns>
        private static string GetDescription(string errorCode)
        {
            switch (errorCode)
            {
                case "INVALID_REQUEST":
                    return "The provided request was invalid.";

                case "MAX_ELEMENTS_EXCEEDED":
                    return "The number of elements in the query exceeds the per-query limit.";

                case "OVER_QUERY_LIMIT":
                    return
                        "You have exceeded the API request limit for the allowed time period. Please try again later.";

                case "REQUEST_DENIED":
                    return "The API has denied access to this application. Please contact Google.";

                case "UNKNOWN_ERROR":
                    return "There was an unknown server error while processing the request.";

                case "ZERO_RESULTS":
                    return "The query executed successfully but no results were found.";

                default:
                    return "An unknown error code or corrupted data was recieved from the Google API.";
            }
        }

        #endregion
    }
}