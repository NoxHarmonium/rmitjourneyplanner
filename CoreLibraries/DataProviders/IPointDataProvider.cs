// -----------------------------------------------------------------------
// <copyright file="IPointDataProvider.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using RmitJourneyPlanner.CoreLibraries.Types;
    using RmitJourneyPlanner.CoreLibraries.Positioning;

    /// <summary>
    /// A NetworkDataProvider is a plugable class that is used to provide information on the distance between 2 points such as walking or driving.
    /// </summary>
    public interface IPointDataProvider
    {
        /// <summary>
        /// Gets the distance between 2 points.
        /// </summary>
        /// <param name="locationA"></param>
        /// <param name="locationB"></param>
        /// <returns></returns>
        List<Arc> GetDistance(Location locationA, Location locationB);

        /// <summary>
        /// Gets the path traversed between 2 points.
        /// </summary>
        /// <param name="locationA"></param>
        /// <param name="locationB"></param>
        /// <returns></returns>
        List<Location> GetPath(Location locationA, Location locationB);
    }
}
