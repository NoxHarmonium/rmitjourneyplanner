using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AjaxServer.aspx
{
    using AjaxServer.AspxComponents;

    using RmitJourneyPlanner.CoreLibraries.DataAccess;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Google;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Breeders;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Mutators;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.RouteGenerators;

    public partial class InitSearch : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Global.busy)
            {
                Response.Write("A search is currently in progress. Please wait...");    

            }
            else
            try
            {
                string origin = Request.Params["txtOrigin"];
                string destination = Request.Params["txtDestination"];
                string date = Request.Params["txtDate"];
                string time = Request.Params["txtTime"];

                INetworkDataProvider metlinkProvider = new MetlinkDataProvider();

                EvolutionaryProperties properties = new EvolutionaryProperties();
                properties.PointDataProviders.Add(new WalkingDataProvider());
                properties.NetworkDataProviders.Add(metlinkProvider);
                properties.ProbMinDistance = 0.7;
                properties.ProbMinTransfers = 0.2;
                properties.MaximumWalkDistance = 1.5;
                properties.PopulationSize = 100;
                properties.MaxDistance = 0.5;
                properties.DepartureTime = DateTime.Parse(date + " " + time);
                properties.NumberToKeep = 25;
                properties.MutationRate = 0.2;
                properties.CrossoverRate = 0.7;
                properties.RouteGenerator = new AlRouteGenerator(properties);
                properties.Mutator = new StandardMutator(properties);
                properties.Breeder = new StandardBreeder(properties);
                properties.FitnessFunction = new AlFitnessFunction(properties);
                properties.Database = new MySqlDatabase("20110606fordistributionforrmit");
                properties.Destination = new TerminalNode(-1, destination);
                properties.Origin = new TerminalNode(-1, origin);
                properties.Destination.RetrieveData();


                properties.Database.Open();
                //properties.DataStructures = new DataStructures(properties);

                Global.Planner = new EvolutionaryRoutePlanner(properties);
                RmitJourneyPlanner.CoreLibraries.Logging.Logger.LogEvent += new RmitJourneyPlanner.CoreLibraries.Logging.LogEventHandler(Logger_LogEvent);
                
            }
            catch (Exception ex)
            {
                Response.Write(ex.GetType() + "<br/>" + ex.Message);
                if (ex.InnerException != null)
                {
                    Response.Write("<br/>" + ex.InnerException.Message);
                    if (ex.InnerException.InnerException != null)
                    {
                        Response.Write("<br/>" + ex.InnerException.InnerException.Message);
                    }
                }
               
            }
           
            
            
            //Global.Planner = new EvolutionaryRoutePlanner();
        }

        void Logger_LogEvent(object sender, string message)
        {
            Console.WriteLine(message);
        }
    }
}