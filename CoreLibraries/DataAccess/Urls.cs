// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Urls.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   A collection of URLs that are used internally by the <see cref="RmitJourneyPlanner.CoreLibraries.DataAccess" /> namespace.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    /// <summary>
    /// A collection of URLs that are used internally by the <see cref="RmitJourneyPlanner.CoreLibraries.DataAccess"/> namespace.
    /// </summary>
    internal static class Urls
    {
        // Google Maps Strings
        #region Constants and Fields

        /// <summary>
        ///   The distance api url.
        /// </summary>
        public const string DistanceApiUrl = @"http://maps.googleapis.com/maps/api/distancematrix/xml";

        /// <summary>
        ///   The geocoding api url.
        /// </summary>
        public const string GeocodingApiUrl = @"http://maps.googleapis.com/maps/api/geocode/xml";

        // Yarra Trams Strings
        // Global

        /// <summary>
        ///   The URL for getDestinationsForAllRoutes.
        /// </summary>
        public const string GetDestinationsForAllRoutes =
            @"http://www.yarratrams.com.au/pidsservice/GetDestinationsForAllRoutes";

        /// <summary>
        ///   The URL for getDestinationsForRoute.
        /// </summary>
        public const string GetDestinationsForRoute =
            @"http://www.yarratrams.com.au/pidsservice/GetDestinationsForRoute";

        /// <summary>
        ///   The URL for getListOfStopsByRouteNoAndDirection.
        /// </summary>
        public const string GetListOfStopsByRouteNoAndDirection =
            @"http://www.yarratrams.com.au/pidsservice/GetListOfStopsByRouteNoAndDirection";

        /// <summary>
        ///   The URL for getMainRoutes.
        /// </summary>
        public const string GetMainRoutes = @"http://www.yarratrams.com.au/pidsservice/GetMainRoutes";

        /// <summary>
        ///   The URL for getMainRoutesForStop.
        /// </summary>
        public const string GetMainRoutesForStop = @"http://www.yarratrams.com.au/pidsservice/GetMainRoutesForStop";

        /// <summary>
        ///   The URL forGetNewClientGuid.
        /// </summary>
        public const string GetNewClientGuid = @"http://www.yarratrams.com.au/pidsservice/GetNewClientGuid";

        /// <summary>
        ///   The URL for getNextPredictedArrivalTimeAtStopsForTramNo.
        /// </summary>
        public const string GetNextPredictedArrivalTimeAtStopsForTramNo =
            @"http://www.yarratrams.com.au/pidsservice/GetNextPredictedArrivalTimeAtStopsForTramNo";

        /// <summary>
        ///   The URL for getNextPredictedRoutesCollection.
        /// </summary>
        public const string GetNextPredictedRoutesCollection =
            @"http://www.yarratrams.com.au/pidsservice/GetNextPredictedRoutesCollection";

        /// <summary>
        ///   The URL for getSchedulesCollection.
        /// </summary>
        public const string GetSchedulesCollection = @"http://www.yarratrams.com.au/pidsservice/GetSchedulesCollection";

        /// <summary>
        ///   The URL for getSchedulesForTrip.
        /// </summary>
        public const string GetSchedulesForTrip = @"http://www.yarratrams.com.au/pidsservice/GetSchedulesForTrip";

        /// <summary>
        ///   The URL for getStopInformation.
        /// </summary>
        public const string GetStopInformation = @"http://www.yarratrams.com.au/pidsservice/GetStopInformation";

        /// <summary>
        ///   The URL for getStopsAndRoutesUpdatesSince.
        /// </summary>
        public const string GetStopsAndRoutesUpdatesSince =
            @"http://www.yarratrams.com.au/pidsservice/GetStopsAndRoutesUpdatesSince";

        /// <summary>
        ///   The URL of the link explaining the geocoding error codes.
        /// </summary>
        public const string GoogleApiGeocodingHelp =
            @"https://developers.google.com/maps/documentation/javascript/geocoding#GeocodingStatusCodes";

        /// <summary>
        ///   The TramTracker namespace.
        /// </summary>
        public const string TramTrackerNameSpace = @"http://www.yarratrams.com.au/pidsservice/";

        /// <summary>
        ///   The TramTracker url.
        /// </summary>
        public const string TramTrackerUrl = @"http://ws.tramtracker.com.au/pidsservice/pids.asmx";

        #endregion
    }
}