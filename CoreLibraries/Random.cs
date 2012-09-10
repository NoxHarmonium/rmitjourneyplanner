using System;

namespace RmitJourneyPlanner.CoreLibraries
{
    using NPack;

    /// <summary>
	/// Wraps <see cref="System.Random"/> to be thread safe. Initializes one random object per thread.
	/// </summary>
    public class Random
	{
	    [ThreadStatic]
        private static MersenneTwister random;

	
        /// <summary>
        /// Returns a thread safe version of the <see cref="System.Random"/> class.
        /// </summary>
        /// <returns></returns>
		public static MersenneTwister GetInstance()
		{
            return random ?? (random = new MersenneTwister());
		}
	}
}

