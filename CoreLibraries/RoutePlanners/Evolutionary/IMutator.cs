// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMutator.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Interface that represents a gene mutator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region Using Directives

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Interface that represents a gene mutator.
    /// </summary>
    public interface IMutator
    {
        #region Public Methods and Operators

        /// <summary>
        /// Mutate the specified critter and return the result
        /// </summary>
        /// <param name="critter">
        /// The critter. 
        /// </param>
        /// <returns>
        /// A mutated critter. 
        /// </returns>
        Critter Mutate(Critter critter);

        #endregion
    }
}