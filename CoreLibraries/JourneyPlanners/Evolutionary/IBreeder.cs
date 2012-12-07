// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBreeder.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Represents a class which crosses over 2 routes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary
{
    #region Using Directives

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Represents a class which crosses over 2 routes.
    /// </summary>
    public interface IBreeder
    {
        #region Public Methods and Operators

        /// <summary>
        /// Applies crossover to 2 parents to create a child.
        /// </summary>
        /// <param name="first">
        /// The first parent of the crossover. 
        /// </param>
        /// <param name="second">
        /// The second parent of the crossover. 
        /// </param>
        /// <returns>
        /// If the operation is successful then the result is returned, otherwise null. 
        /// </returns>
        Critter[] Crossover(Critter first, Critter second);

        #endregion
    }
}