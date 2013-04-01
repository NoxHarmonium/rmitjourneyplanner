// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Iteration.cs" company="">
//   
// </copyright>
// <summary>
//   The iteration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    #region Using Directives

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// The iteration.
    /// </summary>
    public class Iteration
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the population of this iteration.
        /// </summary>
        /// <value>
        ///   The population of this iteration.
        /// </value>
        public Critter[] Population { get; set; }

        #endregion
    }
}