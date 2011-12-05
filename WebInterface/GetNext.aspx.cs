// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetNext.aspx.cs" company="RMIT University">
//    Copyright RMIT University 2011
// </copyright>
// <summary>
//   Global object to solve routes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.WebInterface
{
    using System;
    using System.Text;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.WebInterface.App_Data;

    /// <summary>
    /// The get next.
    /// </summary>
    public partial class GetNext : System.Web.UI.Page
    {
        #region Methods

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PageLoad(object sender, EventArgs e)
        {
            bool success = false;
            if (this.Request.Params["next"] == "true")
            {
                success = RouteSolver.NextStep();
            }

            if (!success)
            {
                var json = new StringBuilder("{\n \"paths\": [{\n");

                var nodelist = new[] { RouteSolver.Current, RouteSolver.Best };
                int count = 0;

                foreach (INetworkNode node in nodelist)
                {
                    INetworkNode current = node;

                    json.Append("\t\"Route\":[ \n ");

                    var elements = new StringBuilder();

                    string baseImage = count == 0 ? "images/icons/red/" : "images/icons/blue/";

                    if (current != null)
                    {
                        bool ifirst = true;
                        while (current != null)
                        {
                            string travelType;
                            string image = baseImage;
                            if (current.Parent == null)
                            {
                                travelType = "Begin at the ";
                                image += "../number_0.png";
                            }
                            else if ((current.GetType() == current.Parent.GetType())
                                     && current.GetType() == typeof(TramStop))
                            {
                                travelType = "Catch a tram to ";
                                image += "tramway.png";
                            }
                            else
                            {
                                travelType = "Walk to ";
                                image += "pedestriancrossing.png";
                            }

                            var element = new StringBuilder();
                            element.Append("\t\t{\n");
                            TimeSpan totalTime = current.TotalTime;
                            if (current.Parent != null)
                            {
                                totalTime = current.TotalTime - current.Parent.TotalTime;
                            }

                            element.Append(
                                string.Format(
                                    "\t\t\t\"Latitude\": \"{0}\",\n" + "\t\t\t\"Longitude\": \"{1}\",\n"
                                    + "\t\t\t\"Name\": \"{2}\",\n" + "\t\t\t\"TotalTime\": \"{3}\",\n"
                                    + "\t\t\t\"Image\": \"{4}\",\n" + "\t\t\t\"Type\": \"{5}\"\n", 
                                    current.Latitude, 
                                    current.Longitude, 
                                    current.Id + string.Format(" ({0})", current.TotalTime), 
                                    totalTime, 
                                    image, 
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

                    json.Append(elements);
                    if (count == 1)
                    {
                        json.Append("\t]}\n");
                    }
                    else
                    {
                        count++;
                        json.Append("\t]},{\n");
                    }
                }

                json.Append("], \"iteration\" : \"" + RouteSolver.CurrentIteration.ToString() + "\"}\n");
                this.Response.Clear();
                this.Response.Write(json);
            }
            else
            {
                this.Response.Clear();
                this.Response.Write("{ \"success\": \"true\" }");
            }
        }

        #endregion
    }
}