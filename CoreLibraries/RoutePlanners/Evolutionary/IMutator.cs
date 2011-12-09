// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="IMutator.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Interface that represents a gene mutator.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Interface that represents a gene mutator.
    /// </summary>
    public interface IMutator
    {
        #region Public Methods

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