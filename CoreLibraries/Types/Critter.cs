// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="Critter.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Represents a member of a population in an evolutionary algorithm.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    /// <summary>
    /// Represents a member of a population in an evolutionary algorithm.
    /// </summary>
    public class Critter
    {
        #region Constants and Fields

        /// <summary>
        /// The fitness.
        /// </summary>
        private readonly double fitness;

        /// <summary>
        /// The route.
        /// </summary>
        private readonly Route route;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Critter"/> class.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="fitness">
        /// The fitness.
        /// </param>
        public Critter(Route route, double fitness)
        {
            this.route = route;
            this.fitness = fitness;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Fitness.
        /// </summary>
        public double Fitness
        {
            get
            {
                return this.fitness;
            }
        }

        /// <summary>
        /// Gets Route.
        /// </summary>
        public Route Route
        {
            get
            {
                return this.route;
            }
        }

        #endregion
    }
}