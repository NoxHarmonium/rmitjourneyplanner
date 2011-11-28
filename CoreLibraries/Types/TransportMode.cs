using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    /// <summary>
    /// Defines the point to point transport mode used.
    /// </summary>
    public enum TransportMode
    {
        /// <summary>
        /// The transport mode is walking. 
        /// </summary>
        Walking = 0,
        /// <summary>
        /// The transport mode is driving.
        /// </summary>
        Driving = 1,
        /// <summary>
        /// The transport mode is cycling.
        /// </summary>
        Bicycling = 2
    }
}
