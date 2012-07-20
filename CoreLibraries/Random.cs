using System;

namespace RmitJourneyPlanner.CoreLibraries
{
	public class Random : System.Random
	{
		[ThreadStatic]
		private System.Random random = new System.Random();
		
		[ThreadStatic]
		private int seed = -1;
		
		public static System.Random GetInstance()
		{
			
			if (random == null)
			{
				random = new Random();
			}
			return random;
		}		
		
	}
}

