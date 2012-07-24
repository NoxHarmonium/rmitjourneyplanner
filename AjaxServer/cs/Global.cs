namespace AjaxServer.AspxComponents
{
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;

    public static class Global
    {
        public static IRoutePlanner Planner;

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