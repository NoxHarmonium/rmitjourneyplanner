using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
                    Critter member = Global.Planner.Fronts[0].OrderBy(t => t.Fitness.NormalisedTravelTime).ToList()[int.Parse(components[0])];
                    Global.Planner.Properties.FitnessFunction.GetFitness(member.Route);

                    int i = 0;
                    foreach (var node in member.Route)
                    {
                        if (i++ >= limit)
                        {
                            break;
                        }
                        MetlinkNode mn = node.Node as MetlinkNode;
                        string name = "Terminal";
                        if (mn != null)
                        {
                            while (Global.busy)
                            {
                                Thread.Sleep(50);
                            }
                            mn.RetrieveData();
                            name = mn.StopSpecName;
                        }
                        Response.Write(node.Node.Latitude + "," + node.Node.Longitude + "," + node.TotalTime.TotalMinutes + "," +node.Node.TransportType + "," + name + "," + node.CurrentRoute + ";");
                    }
                    Response.Write("#");
                }

            }
        }
    }
}