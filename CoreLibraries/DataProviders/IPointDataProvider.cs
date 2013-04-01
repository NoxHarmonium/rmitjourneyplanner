// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPointDataProvider.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   A class that is used to provide information on the distance between 2 points such as walking or driving.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    #region Using Directives

    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// A class that is used to provide information on the distance between 2 points such as walking or driving.
    /// </summary>
    public interface IPointDataProvider
    {
        #region Public Methods and Operators

        /// <summary>
        /// Estimates the distance between 2 points. This method must always underestimate the time and distance.
        /// </summary>
        /// <param name="locationA">
        /// The first point. 
        /// </param>
        /// <param name="locationB">
        /// The second point. 
        /// </param>
        /// <returns>
        /// An <see cref="Arc"/> object representing the distance between the 2 points. 
        /// </returns>
        Arc EstimateDistance(Location locationA, Location locationB);

        /// <summary>
        /// Gets the distance between 2 points.
        /// </summary>
        /// <param name="locationA">
        /// The first point. 
        /// </param>
        /// <param name="locationB">
        /// The second point. 
        /// </param>
        /// <returns>
        /// An <see cref="Arc"/> object representing the distance between the 2 points. 
        /// </returns>
        Arc GetDistance(Location locationA, Location locationB);

        /// <summary>
        /// Gets the path traversed between 2 points.
        /// </summary>
        /// <param name="locationA">
        /// The first point. 
        /// </param>
        /// <param name="locationB">
        /// The second point. 
        /// </param>
        /// <returns>
        /// A list of <see cref="Location"/> objects representing the path. 
        /// </returns>
        List<Location> GetPath(Location locationA, Location locationB);

        #endregion
    }
}