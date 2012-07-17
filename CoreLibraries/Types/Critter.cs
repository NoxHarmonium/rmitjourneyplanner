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
        ///   The UnifiedFitnessScore.
        /// </summary>
        private double unifiedFitnessScore;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="Critter" /> class.
        /// </summary>
        /// <param name="route"> The route. </param>
        /// <param name="unifiedFitnessScore"> The UnifiedFitnessScore. </param>
        public Critter(Route route, Fitness fitness)
        {
            this.route = route;
            this.Fitness = fitness;
        }

        #endregion

        #region Public Properties

        public int N { get; set; }

        public int Rank { get; set; }

        public double Distance { get; set; }

        /// <summary>
        ///   Gets or sets UnifiedFitnessScore.
        /// </summary>
        public double UnifiedFitnessScore
        {
            get
            {
                return this.unifiedFitnessScore;
            }

            set
            {
                this.unifiedFitnessScore = value;
            }
        }

        /// <summary>
        /// Gets or sets the fitness object for this critter.
        /// </summary>
        public Fitness Fitness { get; set; }

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
            return new Critter((Route)this.Route.Clone(), (Fitness)this.Fitness.Clone());
        }

        #endregion
    }
}