using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    public static class Urls
    {
        public const string GeocodingApiUrl = @"http://maps.googleapis.com/maps/api/geocode/xml";
        public const string DistanceApiUrl = @"http://maps.googleapis.com/maps/api/distancematrix/xml";
        public const string TramTrackerUrl = @"http://ws.tramtracker.com.au/pidsservice/pids.asmx";
    }
}
