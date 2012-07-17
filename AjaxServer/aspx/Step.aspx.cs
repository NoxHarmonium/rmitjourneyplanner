using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AjaxServer.aspx
{
    using AjaxServer.AspxComponents;

    public partial class Step : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Global.busy = true;
            //for (int i = 0; i < 100; i++)
            {
                Global.Planner.SolveStep();
            }
            /*
            try
            {
                Global.Planner.SolveStep();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            */
            Global.busy = false;
            Global.iteration++;
            Response.Write("0");
        }
    }
}