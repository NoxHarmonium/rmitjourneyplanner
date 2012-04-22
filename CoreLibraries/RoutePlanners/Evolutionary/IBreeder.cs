// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   Represents a class which crosses over 2 routes.
    /// </summary>
    public interface IBreeder
    {
        #region Public Methods

        /// <summary>
        ///   Applies crossover to 2 parents to create a child.
        /// </summary>
        /// <param name="first"> The first parent of the crossover. </param>
        /// <param name="second"> The second parent of the crossover. </param>
        /// <returns> If the operation is successful then the result is returned, otherwise null. </returns>
        Critter[] Crossover(Critter first, Critter second);

        #endregion
    }
}