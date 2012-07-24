using System;

namespace RmitJourneyPlanner.CoreLibraries
{
	/// <summary>
	/// Wraps <see cref="System.Random"/> to be thread safe. Initializes one random object per thread.
	/// </summary>
    public class Random
	{
	    [ThreadStatic]
	    private static System.Random random;

	
        /// <summary>
        /// Returns a thread safe version of the <see cref="System.Random"/> class.
        /// </summary>
        /// <returns></returns>
		public static System.Random GetInstance()
		{
		    return random ?? (random = new System.Random(0));
		}
	}
}

