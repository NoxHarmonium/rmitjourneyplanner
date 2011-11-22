using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    public static class Urls
    {
        //Google Maps Strings
        public const string GeocodingApiUrl         = @"http://maps.googleapis.com/maps/api/geocode/xml";
        public const string DistanceApiUrl          = @"http://maps.googleapis.com/maps/api/distancematrix/xml";
        
       
        
        //Yarra Trams Strings
        //  Global
        public const string TramTrackerUrl          = @"http://ws.tramtracker.com.au/pidsservice/pids.asmx";
        public const string TramTrackerNameSpace    = @"http://www.yarratrams.com.au/pidsservice/";
        
        //  Actions
        public const string GetNewClientGuid        = @"http://www.yarratrams.com.au/pidsservice/GetNewClientGuid";
    }
}
