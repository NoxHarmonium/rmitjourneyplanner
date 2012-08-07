using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//#define moea

namespace AjaxServer.aspx
{
    using System.Threading;

    using AjaxServer.AspxComponents;

    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.Types;

    public partial class GetPath : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string[] members = Request["members"].Split(new [] {','});
            foreach (string memberString in members)
            {
                string[] components = memberString.Split('|');
                int limit = int.MaxValue;
               // bool biDir = false;
                if (components.Length > 1)
                {
                    limit = Convert.ToInt32(components[1]);
                    //biDir = Convert.ToBoolean(Convert.ToInt32(components[2]));
                }
                if (components[0].Trim() != "undefined" && memberString.Trim() != String.Empty && memberString.Trim() != "undefined")
                {
                    
                    #if moea
                    
                        Critter member =
                            Global.Planner.Fronts[0].OrderBy(t => t.Fitness.NormalisedTravelTime).GroupBy(
                                t => t.Fitness).Select(g => g.First()).ToList()[int.Parse(components[0])];
                        Global.Planner.Properties.FitnessFunction.GetFitness(member.Route);
                    #else
                    Critter member = Global.Planner.Population.OrderBy(t => t.Fitness.NormalisedTravelTime).ToList()[int.Parse(components[0])];//.GroupBy(t => t.Fitness).Select(g => g.First()).ToList()[int.Parse(components[0])];
                        Global.Planner.Properties.FitnessFunction.GetFitness(member.Route);
                    #endif

                  
                    
                    int i = 0;
                    foreach (var leg in member.Fitness.JourneyLegs)
                    {
                        if (i++ >= limit)
                        {
                            break;
                        }
                        MetlinkNode mn = leg.Origin;
                        
                        Response.Write(mn.Latitude + "," + mn.Longitude + "," + leg.TotalTime.TotalMinutes + "," + leg.TransportMode + "," + leg.Origin.StopSpecName + "<br>" + leg.Destination.StopSpecName + "," + leg.RouteId1 + ";");
                    }
                    Response.Write("#");
                }

            }
        }
    }
}