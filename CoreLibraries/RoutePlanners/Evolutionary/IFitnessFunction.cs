// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFitnessFunction.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Represents a function which evaluates the fitness of a route.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
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
        /// Returns a value representing the fitness of the route.
        /// </summary>
        /// <param name="route">
        /// The route the is to be evaluated. 
        /// </param>
        /// <returns>
        /// A double value representing the fitness. 
        /// </returns>
        Fitness GetFitness(Route route);

        /// <summary>
        /// The get fitness.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="initialDeparture">
        /// The initial departure.
        /// </param>
        /// <returns>
        /// </returns>
        Fitness GetFitness(Route route, DateTime initialDeparture);

        #endregion
    }
}