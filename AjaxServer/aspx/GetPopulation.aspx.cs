using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//#define moea

namespace AjaxServer.aspx
{
    using AjaxServer.AspxComponents;

    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.Types;

    public partial class GetPopulation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int index = 0;
            //Fitness fitness = new Fitness();
            //if (Global.Planner is MoeaRoutePlanner)
                


#if moea
                foreach (Critter c in Global.Planner.Fronts[0].OrderBy(t => t.Fitness.NormalisedTravelTime).GroupBy(t => t.Fitness).Select(g => g.First()))
                {
                    Response.Write("Member " + index++.ToString() + " (r:" + c.Rank + " n:" + c.N + "),");

                }
#else     
                foreach (
                    //Critter c in
                    //    Global.Planner.Population.OrderBy(t => t.Fitness.NormalisedTravelTime).GroupBy(t => t.Fitness).
                          //  Select(g => g.First()))
                          Critter c in
                        Global.Planner.Population.OrderBy(t => t.Fitness.NormalisedTravelTime))
                {
                    Response.Write("Member " + index++.ToString() + " (r:" + c.Rank + " n:" + c.N + "),");

                }
#endif

        }
    }
}