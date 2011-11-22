using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RmitJourneyPlanner.CoreLibraries.Types;
using RmitJourneyPlanner.CoreLibraries.DataAccess;
using RmitJourneyPlanner.CoreLibraries.Positioning;


namespace RmitJourneyPlanner.CoreLibraries.Positioning
{
    /// <summary>
    /// Represents a tram stop in the Yarra Trams database.
    /// </summary>
    public class TramStop : Location
    {
        private int stopID;
        private int routeID;
        private int routeDirection;

        /// <summary>
        /// Creates a new Stop object from the stop id in the TransNET database.
        /// </summary>
        /// <param name="stopID"></param>
        public TramStop(int stopID) : base(0,0)
        {
            throw new NotImplementedException("TODO: Lookup stop id for location");
        }

        /// <summary>
        /// Gets the stop ID of this stop.
        /// </summary>
        public int StopID
        {
            get 
            { 
                return stopID; 
            }
        }

        /// <summary>
        /// Returns distance to another Location via a tram route or walking.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override Arc DistanceTo(Location location)
        {
            // If the other location is a tram stop then calculate via tram routes
            if (location is TramStop)
            {
                throw new NotImplementedException("TODO: Create distance finding via tram network");
            }
            else
            {
                //Otherwise calculate via walking
                return base.DistanceTo(location);
            }
        }


    }
}
