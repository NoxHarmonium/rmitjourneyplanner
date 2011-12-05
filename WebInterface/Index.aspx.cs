// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Index.aspx.cs" company="RMIT University">
//    Copyright RMIT University 2011
// </copyright>
// <summary>
//   Global object to solve routes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.WebInterface
{
    using System;
    using System.Net;

    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.WebInterface.App_Data;

    /// <summary>
    /// The index.
    /// </summary>
    public partial class Index : System.Web.UI.Page
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
            this.btnReset.Click += this.BtnResetClick;

            CoreLibraries.DataAccess.ConnectionInfo.Proxy =
                new WebProxy(
                    "http://aproxy.rmit.edu.au:8080", false, null, new NetworkCredential("s3229159", "MuchosRowlies1"));

            if (!RouteSolver.Ready)
            {
                RouteSolver.Reset(
                    new Location(this.txtSource.Text), 
                    new Location(this.txtDestination.Text), 
                    Convert.ToDouble(this.txtMaxWalk.Text));
            }

            this.GoogleMapsControl1.Nodes.Clear();

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

            // GoogleMapsControl1.Nodes.Add(
            // RouteSolver.Current
        }

        /// <summary>
        /// The btn reset_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnResetClick(object sender, EventArgs e)
        {
            RouteSolver.Reset(
                new Location(this.txtSource.Text), 
                new Location(this.txtDestination.Text), 
                Convert.ToDouble(this.txtMaxWalk.Text));
        }

        #endregion
    }
}