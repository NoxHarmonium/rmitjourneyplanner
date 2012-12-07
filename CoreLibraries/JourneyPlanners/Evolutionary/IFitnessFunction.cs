// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFitnessFunction.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Represents a function which evaluates the fitness of a route.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary
{
    #region Using Directives

    using System;

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Represents a function which evaluates the fitness of a route.
    /// </summary>
    public interface IFitnessFunction
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the fitness of the specified route.
        /// </summary>
        /// <param name="route">
        /// A route.
        /// </param>
        /// <returns>
        /// A <see cref="Fitness"/> object representing the fitness of the specified route.
        /// </returns>
        Fitness GetFitness(Route route);

        /// <summary>
        /// Gets the fitness of the specified route for a specified departure time.
        /// </summary>
        /// <param name="route">
        /// A route.
        /// </param>
        /// <param name="initialDeparture">
        /// The departure time of the journey you are measuring the fitness of.
        /// </param>
        /// <returns>
        /// A <see cref="Fitness"/> object representing the fitness of the specified route and departure time.
        /// </returns>
        Fitness GetFitness(Route route, DateTime initialDeparture);

        #endregion
    }
}