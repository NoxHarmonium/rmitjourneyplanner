// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   Interface that represents a gene mutator.
    /// </summary>
    public interface IMutator
    {
        #region Public Methods

        /// <summary>
        ///   Mutate the specified critter and return the result
        /// </summary>
        /// <param name="critter"> The critter. </param>
        /// <returns> A mutated critter. </returns>
        Critter Mutate(Critter critter);

        #endregion
    }
}