// -----------------------------------------------------------------------
// <copyright file="GoogleApiException.cs" company="RMIT University">
// By Sean Dawson
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents an error returned from the Google Maps API.
    /// </summary>
    public class GoogleApiException : Exception
    {
        
       

        private string errorCode;

        /// <summary>
        /// Returns the raw error code returned by the Google Maps API.
        /// </summary>
        public string ErrorCode
        {
            get
            {
                return errorCode;
            }

        }
        
        /// <summary>
        /// Initializes a new instance of the GoogleApiException class from the supplied error code.
        /// </summary>
        /// <param name="errorCode"></param>
        public GoogleApiException(string errorCode)
            : base(GoogleApiException.getDescription(errorCode))
        {
            this.errorCode = errorCode;



        }

        
        /// <summary>
        /// Converts the Google Maps API error codes into a human readable message.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        private static string getDescription(string errorCode)
        {

            switch (errorCode)
            {
                case "INVALID_REQUEST":
                    return "The provided request was invalid.";
                    

                case "MAX_ELEMENTS_EXCEEDED":
                    return "The number of elements in the query exceeds the per-query limit.";
                    

                case "OVER_QUERY_LIMIT":
                    return "You have exceeded the API request limit for the allowed time period. Please try again later.";
                    

                case "REQUEST_DENIED":
                    return "The API has denied access to this application. Please contact Google.";
                    

                case "UNKNOWN_ERROR":
                    return "There was an unknown server error while processing the request.";
                   
                case "ZERO_RESULTS" :
                    return "The query executed successfully but no results were found.";

                default:
                    return "An unknown error code or corrupted data was recieved from the Google API.";
                    
            }
        }


    }
}
