// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region

    using System;

    using NUnit.Framework;

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
        private Route route;

        /// <summary>
        ///   The UnifiedFitnessScore.
        /// </summary>
        private double unifiedFitnessScore;
		
		/// <summary>
		/// The departure time.
		/// </summary>
		public DateTime departureTime;

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
		
		/// <summary>
		/// Gets or sets the age of this critter.
		/// </summary>
		/// <value>
		/// The age of this critter.
		/// </value>
		public int Age {get; set;}
		
		/// <summary>
		/// Gets or sets the N value of this critter.
		/// </summary>
		/// <value>
		/// The N value of this critter.
		/// </value>
        public int N { get; set; }
		
		/// <summary>
		/// Gets or sets the rank of this critter.
		/// </summary>
		/// <value>
		/// The rank of this critter.
		/// </value>
        public int Rank { get; set; }
		
		/// <summary>
		/// Gets or sets the distance value of this critter.
		/// </summary>
		/// <value>
		/// The distance value of this critter.
		/// </value>
        public double Distance { get; set; }

      	/// <summary>
      	/// Gets or sets the unified fitness score of this critter.
      	/// </summary>
      	/// <value>
      	/// The unified fitness score of this critter.
      	/// </value>
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
        /// <value>
        /// The fitness object for this critter.
        /// </value>
        public Fitness Fitness { get; set; }

        /// <summary>
        ///   Gets or sets the route associated with this critter.
        /// </summary>
        public Route Route
        {
            get
            {
                return this.route;
            }
			set
			{
				this.route = value;	
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
            Assert.That(this.departureTime != default(DateTime));
            var cloned = new Critter((Route)this.Route.Clone(), (Fitness)this.Fitness.Clone()) { departureTime = this.departureTime, Age = this.Age, N = this.N, Rank = this.Rank };
            Assert.That(cloned.departureTime != default(DateTime));
            return cloned;
        }

        #endregion
    }
}