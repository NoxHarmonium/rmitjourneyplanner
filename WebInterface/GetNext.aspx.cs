using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using RmitJourneyPlanner.CoreLibraries.DataProviders;

namespace WebInterface
{
    public partial class GetNext : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RouteSolver.NextStep();
            INetworkNode current = RouteSolver.Current;
            StringBuilder JSON = new StringBuilder("{\n \"paths\": [{\n");
           
            INetworkNode[] nodelist = new INetworkNode[] { RouteSolver.Current, RouteSolver.Best };
            int count = 0;

            foreach (INetworkNode node in nodelist)
            {
                current = node;


                JSON.Append("\t\"Route\":[ \n ");

                StringBuilder elements = new StringBuilder();
                
                

                if (current != null)
                {
                    bool ifirst = true;
                    while (current != null)
                    {
                        string travelType;
                        if (current.Parent == null)
                        {
                            travelType = "Arrive at the start";
                        }
                        else
                            if ((current.GetType() == current.Parent.GetType()) && current.GetType() == typeof(TramStop))
                            {
                                travelType = "Catch a tram to ";
                            }
                            else
                            {
                                travelType = "Walk to ";
                            }
                        StringBuilder element = new StringBuilder();
                        element.Append("\t\t{\n");
                        TimeSpan totalTime = current.TotalTime;
                        if (current.Parent != null)
                        {
                            totalTime = current.TotalTime - current.Parent.TotalTime;
                        }
                        
                        element.Append(String.Format(
                                    "\t\t\t\"Latitude\": \"{0}\",\n" +
                                    "\t\t\t\"Longitude\": \"{1}\",\n" +
                                    "\t\t\t\"Name\": \"{2}\",\n" +
                                    "\t\t\t\"TotalTime\": \"{3}\",\n" +
                                    "\t\t\t\"Type\": \"{4}\"\n",
                                    current.Latitude,
                                    current.Longitude,
                                    current.ID + String.Format(" ({0})",current.TotalTime),
                                    totalTime,
                                    travelType));

                        current = current.Parent;
                        if (!ifirst)
                        {
                            element.Append("\t\t},\n");
                        }
                        else
                        {
                            element.Append("\t\t}\n");
                            ifirst = false;
                        }
                        

                        elements.Insert(0, element);
                        
                    }
                }
                JSON.Append(elements);
                if (count == 1)
                {
                    JSON.Append("\t]}\n");
                }
                else
                {
                    count++;
                    JSON.Append("\t]},{\n");
                }

                
            }
            JSON.Append("]}\n");
            Response.Clear();
            Response.Write(JSON);
        }
    }
}