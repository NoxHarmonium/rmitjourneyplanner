namespace AjaxServer.AspxComponents
{
    public static class Global
    {
        public static RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.EvolutionaryRoutePlanner Planner;

        public static bool ready;

        public static bool busy;

        public static int iteration;

        static Global()
        {
            utils.ConsoleHelper.Create();

        }

    }
}