namespace AjaxServer.AspxComponents
{
    public static class Global
    {
        public static RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.MoeaRoutePlanner Planner;

        public static bool ready;

        public static bool busy;

        public static int iteration;

        static Global()
        {
            
#if __MonoCS__
		//Dont do anything if mono	
#else
			utils.ConsoleHelper.Create();
#endif
        }

    }
}