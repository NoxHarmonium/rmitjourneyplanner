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
        TowardsCity = 0,
        FromCity = 1,
        Unknown = 3

    };
}
