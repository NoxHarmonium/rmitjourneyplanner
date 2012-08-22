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
    using RmitJourneyPlanner.CoreLibraries.Logging;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Breeders;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Mutators;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.RouteGenerators;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;

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
                Logger.LogEvent += (o, message) => { Console.WriteLine(message); };
                string origin = Request.Params["txtOrigin"];
                string destination = Request.Params["txtDestination"];
                string date = Request.Params["txtDate"];
                string time = Request.Params["txtTime"];
                Console.WriteLine(Request.Params["chkBiDir"]);
                bool biDir = this.Request.Params["chkBiDir"] == "on";
                INetworkDataProvider metlinkProvider = new MetlinkDataProvider();

                EvolutionaryProperties properties = new EvolutionaryProperties();
                properties.Bidirectional = false;
                properties.PointDataProviders.Add(new WalkingDataProvider());
                properties.NetworkDataProviders.Add(metlinkProvider);
                properties.ProbMinDistance = 0.7;
                properties.ProbMinTransfers = 0.2;
                properties.MaximumWalkDistance = 1.5;
                properties.PopulationSize = 100;
                properties.MaxDistance = 0.5;
                properties.DepartureTime = DateTime.Parse("2012/7/24 6:00 PM");//DateTime.Parse(date + " "+ time);
                properties.NumberToKeep = 25;
                properties.MutationRate = 0.3;
                properties.CrossoverRate = 0.7;
                //properties.RouteGenerator = new AlRouteGenerator(properties);
                properties.RouteGenerator = new DFSRoutePlanner(properties,SearchType.Greedy_BiDir);
                properties.Mutator = new StandardMutator(properties);
                properties.Breeder = new StandardBreeder(properties);
                properties.FitnessFunction = new AlFitnessFunction(properties);
                properties.Database = new MySqlDatabase("20110606fordistributionforrmit");
                //properties.Destination = new MetlinkNode(20039,metlinkProvider);//
                //properties.Destination = new MetlinkNode(628,metlinkProvider);
                properties.Destination = new MetlinkNode(19843,metlinkProvider);
                   // new TerminalNode(-1, destination);
                   // metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, destination), 0);
                //properties.Origin = new MetlinkNode(19965, metlinkProvider);//
                    //metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, origin), 0);
                properties.Origin = new MetlinkNode(22180, metlinkProvider);
                   
                properties.Destination.RetrieveData();
                properties.Origin.RetrieveData();


                properties.Database.Open();
                //properties.DataStructures = new DataStructures(properties);

                Global.Planner = new MoeaRoutePlanner(properties);
              
                RmitJourneyPlanner.CoreLibraries.Logging.Logger.LogEvent += this.Logger_LogEvent;
                
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
				//throw;
               
            }
           
            
            
            //Global.Planner = new EvolutionaryRoutePlanner();
        }

        void Logger_LogEvent(object sender, string message)
        {
            Console.WriteLine(message);
        }
    }
}