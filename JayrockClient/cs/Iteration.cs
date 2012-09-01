using System;
using RmitJourneyPlanner.CoreLibraries.Types;

namespace JayrockClient
{
	public class Iteration
	{
		/// <summary>
		/// The population of the iteration.
		/// </summary>
		private Critter[] population;
		
		
		/// <summary>
		/// Gets or sets the population of this iteration.
		/// </summary>
		/// <value>
		/// The population of this iteration.
		/// </value>
		public Critter[] Population {
			get {
				return this.population;
			}
			set {
				population = value;
			}
		}
		
	}
}

