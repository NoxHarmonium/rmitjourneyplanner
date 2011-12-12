// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoogleMapsControl.ascx.cs" company="">
//   
// </copyright>
// <summary>
//   The google maps control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RMITTravelPlanner.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    /// <summary>
    /// The google maps control.
    /// </summary>
    public partial class GoogleMapsControl : System.Web.UI.UserControl
    {
        #region Constants and Fields

        /// <summary>
        /// The nodes.
        /// </summary>
        private List<INetworkNode> nodes = new List<INetworkNode>();

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets a list of nodes that corrospond to points on the map.
        /// </summary>
        public List<INetworkNode> Nodes
        {
            get
            {
                return this.nodes;
            }

            set
            {
                this.nodes = value;
            }
        }

        #endregion

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
        protected void Page_Load(object sender, EventArgs e)
        {
            this.addJavascript("var map;");

            string csJavascript = "function addMarkers(){";
            foreach (INetworkNode node in this.nodes)
            {
                csJavascript += this.addMarker(node.Latitude, node.Longitude, node.Id);
            }

            csJavascript += "}";
            csJavascript += this.generateMapCode(-37.780323, 144.957103);

            // string include = "<script type=\"text/javascript\" src=\"http://maps.googleapis.com/maps/api/js?sensor=false\"></script>\n";
            string header = string.Format("<script type=\"text/javascript\">{0}</script>\n", csJavascript);

            HtmlGenericControl myJavaScript = new HtmlGenericControl();
            myJavaScript.TagName = "script";
            myJavaScript.Attributes.Add("type", "text/javascript");
            myJavaScript.Attributes.Add("src", "http://maps.googleapis.com/maps/api/js?sensor=false");
            this.Page.Header.Controls.Add(myJavaScript);
            myJavaScript = new HtmlGenericControl();
            myJavaScript.TagName = "script";
            myJavaScript.Attributes.Add("type", "text/javascript");
            myJavaScript.Attributes.Add("src", "./Code.js");
            this.Page.Header.Controls.Add(myJavaScript);

            this.addJavascript(csJavascript);

        }

        /// <summary>
        /// The add javascript.
        /// </summary>
        /// <param name="js">
        /// The js.
        /// </param>
        private void addJavascript(string js)
        {
            HtmlGenericControl myJavaScript2 = new HtmlGenericControl();
            myJavaScript2.TagName = "script";
            myJavaScript2.Attributes.Add("type", "text/javascript");
            myJavaScript2.InnerHtml = js;
            this.Page.Header.Controls.Add(myJavaScript2);
            this.Page.Header.Controls.Add(new LiteralControl("\r\n"));
        }

        /// <summary>
        /// The add marker.
        /// </summary>
        /// <param name="latitude">
        /// The latitude.
        /// </param>
        /// <param name="longitude">
        /// The longitude.
        /// </param>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <returns>
        /// The add marker.
        /// </returns>
        private string addMarker(double latitude, double longitude, string label)
        {
            string latlng = string.Format("new google.maps.LatLng({0},{1})", latitude, longitude);
            string marker =
                string.Format(
                    "var marker = new google.maps.Marker({{position: {0}, map: window.map,title:\"{1}\"}});", 
                    latlng, 
                    label);

            // marker += "alert(window.map);";
            // addJavascript(marker);
            return marker;
        }

        /// <summary>
        /// The generate map code.
        /// </summary>
        /// <param name="latitude">
        /// The latitude.
        /// </param>
        /// <param name="longitude">
        /// The longitude.
        /// </param>
        /// <returns>
        /// The generate map code.
        /// </returns>
        private string generateMapCode(double latitude, double longitude)
        {
            StringBuilder output = new StringBuilder();

            output.Append("function initialize() {");
            output.Append(string.Format("var latlng = new google.maps.LatLng({0}, {1});", latitude, longitude));
            output.Append("var myOptions = { zoom: 13, center: latlng,  mapTypeId: google.maps.MapTypeId.ROADMAP};");
            output.Append("window.map = new google.maps.Map(document.getElementById(\"map_canvas\"), myOptions);");

            // output.Append("alert(\"here!\");");
            // string latlng = String.Format("new google.maps.LatLng({0},{1})", latitude+0.5, longitude-0.5); ;
            // string marker = String.Format("var marker = new google.maps.Marker({{position: {0}, map: window.map,title:\"{1}\"}});", latlng, "Test");
            // output.Append(marker);
            output.Append("addMarkers();");

            // output.Append("");
            output.Append("}");

            output.Append("window.onload = initialize;");

            return output.ToString();
        }

        #endregion
    }
}