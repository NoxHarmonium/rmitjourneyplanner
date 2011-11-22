// -----------------------------------------------------------------------
// <copyright file="Distance.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// An object representing the time and distance between 2 points
    /// </summary>
    public class Arc
    {
        private TimeSpan time;      
        private double distance;
        private TransportMode transportMode;
       
        public Arc(TimeSpan time, double distance, TransportMode transportMode)
        {
            this.time = time;
            this.distance = distance;
            this.transportMode = transportMode;
        }

        /// <summary>
        /// Gets the time between the 2 points using the specfied transport mode.
        /// </summary>
        public TimeSpan Time
        {
            get { return time; }            
        }
        /// <summary>
        /// Gets the distance between 2 points in meters.
        /// </summary>
        public double Distance
        {
            get { return distance; }           
        }

        /// <summary>
        /// Gets the transport mode used to get these distance statistics.
        /// </summary>
        public TransportMode TransportMode
        {
            get { return transportMode; }            
        }

        /// <summary>
        /// Returns if 2 distance objects are equal within 500 meters and to the minute.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() == this.GetType())
            {

                Arc otherDistance = (Arc)obj;
                if (otherDistance.Time.Minutes == this.Time.Minutes &&
                    otherDistance.Time.Hours == this.Time.Hours &&
                    (otherDistance.Distance - this.Distance) < 500)
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        

    }
}
