using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;
using System.IO;

namespace RMITTravelPlanner.WebControls
{
    public partial class GoogleMapsControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            addJavascript("var map;");


            string csJavascript = generateMapCode(-34.397, 150.644);
            string include = "<script type=\"text/javascript\" src=\"http://maps.googleapis.com/maps/api/js?sensor=false\"></script>\n";
            
            string header = String.Format("<script type=\"text/javascript\">{0}</script>\n", csJavascript);

            HtmlGenericControl myJavaScript = new HtmlGenericControl();
            myJavaScript.TagName = "script";
            myJavaScript.Attributes.Add("type", "text/javascript");
            myJavaScript.Attributes.Add("src","http://maps.googleapis.com/maps/api/js?sensor=false");
            Page.Header.Controls.Add(myJavaScript);

            addJavascript(csJavascript);

            //addMarker(-34.397, 150.0,"Tasty!");
           
        }

        private void addJavascript(string js)
        {
            HtmlGenericControl myJavaScript2 = new HtmlGenericControl();
            myJavaScript2.TagName = "script";
            myJavaScript2.Attributes.Add("type", "text/javascript");
            myJavaScript2.InnerHtml = js;
            Page.Header.Controls.Add(myJavaScript2);
            Page.Header.Controls.Add(new LiteralControl("\r\n"));
           

        }

        private void addMarker(double latitude, double longitude,string label)
        {
            string latlng = String.Format("new google.maps.LatLng({0},{1})", latitude, longitude); ;
            string marker = String.Format("var marker = new google.maps.Marker({{position: {0}, map: window.map,title:\"{1}\"}});", latlng,label);
            marker += "alert(window.map);";
            addJavascript(marker);

        }

        private string generateMapCode(double latitude, double longitude)
        {
            
            
            StringBuilder output = new StringBuilder();
            
            output.Append("function initialize() {");
            output.Append(String.Format("var latlng = new google.maps.LatLng({0}, {1});", latitude, longitude));
            output.Append("var myOptions = { zoom: 8, center: latlng,  mapTypeId: google.maps.MapTypeId.ROADMAP};");
            output.Append("window.map = new google.maps.Map(document.getElementById(\"map_canvas\"), myOptions);");
            //output.Append("alert(\"here!\");");
            string latlng = String.Format("new google.maps.LatLng({0},{1})", latitude+0.5, longitude-0.5); ;
            string marker = String.Format("var marker = new google.maps.Marker({{position: {0}, map: window.map,title:\"{1}\"}});", latlng, "Test");
            output.Append(marker);
            output.Append("}");

            output.Append("window.onload = initialize;");

            return output.ToString();
        }
    }


 
}