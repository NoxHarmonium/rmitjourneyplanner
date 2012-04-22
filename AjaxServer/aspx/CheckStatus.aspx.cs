using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AjaxServer.aspx
{
    using AjaxServer.AspxComponents;

    public partial class CheckStatus : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Global.busy)
            {
                if (Global.ready)
                {
                    Response.Write("3");
                }
                else
                {
                    Response.Write("1");
                }
                
            }
            else
            {
                Response.Write("2," + Global.Planner.Progress + "," + Global.Planner.TargetProgress + "," + Global.iteration);
            }
        }
    }
}