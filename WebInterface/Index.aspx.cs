using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RmitJourneyPlanner.CoreLibraries.Positioning;
using RmitJourneyPlanner.CoreLibraries.DataProviders;
using System.Net;

namespace WebInterface
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RmitJourneyPlanner.CoreLibraries.DataAccess.ConnectionInfo.Proxy =
                new System.Net.WebProxy("http://aproxy.rmit.edu.au:8080", false, null, new NetworkCredential("s3229159", "MuchosRowlies1"));

            if (Request.Params["Next"] == null)
            {
                RouteSolver.Reset(new Location("13 Liverpool St, Coburg, Victoria, Australia"),
                    new Location("1 Lygon St, Carlton, Victoria, Australia"));
            }
            
            INetworkNode current = RouteSolver.Current;
            GoogleMapsControl1.Nodes.Clear();
            /*
            if (current != null)
            {
                while (current.Parent != null)
                {
                    GoogleMapsControl1.Nodes.Add(current);
                    current = current.Parent;
                }
            }
             * */
            //GoogleMapsControl1.Nodes.Add(
            ///RouteSolver.Current
        }
    }
}