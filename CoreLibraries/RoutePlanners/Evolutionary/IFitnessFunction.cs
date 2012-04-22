// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   Represents a function which evaluates the fitness of a route.
    /// </summary>
    public interface IFitnessFunction
    {
        #region Public Methods

        /// <summary>
        ///   Returns a value representing the fitness of the route.
        /// </summary>
        /// <param name="route"> The route the is to be evaluated. </param>
        /// <returns> A double value representing the fitness. </returns>
        double GetFitness(Route route);

        #endregion
    }
}