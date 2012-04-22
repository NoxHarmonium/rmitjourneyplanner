// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region

    using System;

    #endregion

    /// <summary>
    ///   Represents a member of a population in an evolutionary algorithm.
    /// </summary>
    public class Critter : ICloneable
    {
        #region Constants and Fields

        /// <summary>
        ///   The route.
        /// </summary>
        private readonly Route route;

        /// <summary>
        ///   The fitness.
        /// </summary>
        private double fitness;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="Critter" /> class.
        /// </summary>
        /// <param name="route"> The route. </param>
        /// <param name="fitness"> The fitness. </param>
        public Critter(Route route, double fitness)
        {
            this.route = route;
            this.fitness = fitness;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets Fitness.
        /// </summary>
        public double Fitness
        {
            get
            {
                return this.fitness;
            }

            set
            {
                this.fitness = value;
            }
        }

        /// <summary>
        ///   Gets Route.
        /// </summary>
        public Route Route
        {
            get
            {
                return this.route;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Return a copy of this critter.
        /// </summary>
        /// <returns> A cloned critter. </returns>
        public object Clone()
        {
            return new Critter((Route)this.Route.Clone(), this.fitness);
        }

        #endregion
    }
}