// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="IFitnessFunction.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Represents a function which evaluates the fitness of a route.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Represents a function which evaluates the fitness of a route.
    /// </summary>
    public interface IFitnessFunction
    {
        #region Public Methods

        /// <summary>
        /// Returns a value representing the fitness of the route.
        /// </summary>
        /// <param name="route">
        /// The route the is to be evaluated.
        /// </param>
        /// <returns>
        /// A double value representing the fitness.
        /// </returns>
        double GetFitness(Route route);

        #endregion
    }
}