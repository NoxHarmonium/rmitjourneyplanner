using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RmitJourneyPlanner.CoreLibraries.Positioning
{
    /// <summary>
    /// Represents a  train station, a tram stop or a bus stop
    /// as defined by the TransNET database. 
    /// </summary>
    public class Stop : Location
    {
        private int stopID;        
        
        /// <summary>
        /// Creates a new Stop object from the stop id in the TransNET database.
        /// </summary>
        /// <param name="stopID"></param>
        public Stop(int stopID) : base(0,0)
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


    }
}
