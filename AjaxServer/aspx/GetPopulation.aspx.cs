using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AjaxServer.aspx
{
    using AjaxServer.AspxComponents;

    using RmitJourneyPlanner.CoreLibraries.Types;

    public partial class GetPopulation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int index = 0;
            foreach (Critter c in Global.Planner.Population)
            {
                Response.Write("Member " + index++.ToString() + " (" + c.Fitness + "),");

            }
        }
    }
}