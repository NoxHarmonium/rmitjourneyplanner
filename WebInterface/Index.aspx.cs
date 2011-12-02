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
            btnReset.Click += new EventHandler(btnReset_Click);
            
            RmitJourneyPlanner.CoreLibraries.DataAccess.ConnectionInfo.Proxy =
                new System.Net.WebProxy("http://aproxy.rmit.edu.au:8080", false, null, new NetworkCredential("s3229159", "MuchosRowlies1"));

            if (!RouteSolver.Ready)
            {
                RouteSolver.Reset(new Location(txtSource.Text),
                  new Location(txtDestination.Text),Convert.ToDouble(txtMaxWalk.Text));
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

        void btnReset_Click(object sender, EventArgs e)
        {
            RouteSolver.Reset(new Location(txtSource.Text),
                   new Location(txtDestination.Text), Convert.ToDouble(txtMaxWalk.Text));
        }
    }
}