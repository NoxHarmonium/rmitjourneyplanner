// -----------------------------------------------------------------------
// <copyright file="TransportDirection.cs" company="">
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
    /// Represents the direction the public transport is going relative to the city.
    /// </summary>
    public enum TransportDirection
    {
        /// <summary>
        /// The public transport is travelling towards the city
        /// </summary>
        TowardsCity = 0,
        /// <summary>
        /// The public transport is travelling away from the city.
        /// </summary>
        FromCity = 1,
        /// <summary>
        /// The direction of the transport relitive to the city is unknown.
        /// </summary>
        Unknown = 3

    };
}
