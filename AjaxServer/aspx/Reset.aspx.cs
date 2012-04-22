﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AjaxServer.aspx
{
    using AjaxServer.AspxComponents;

    public partial class Reset : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Global.busy = true;
            Global.Planner.Start();
            Global.busy = false;
            Global.ready = true;
        }
    }
}