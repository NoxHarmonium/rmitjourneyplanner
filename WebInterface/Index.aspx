<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="WebInterface.Index" %>

<%@ Register Src="WebControls/GoogleMapsControl.ascx" TagName="GoogleMapsControl"
    TagPrefix="uc1" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Test page</title>
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no" />
    <link href="Style.css" rel="Stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-1.4.1.js" />
</head>
<body>
    <uc1:GoogleMapsControl ID="GoogleMapsControl1" runat="server" />
    <form id="form1" runat="server">
    <div id="sideHatch" runat="server">
        <div id="innerSideHatch" runat="server">
            <h1>
                RMIT Travel Planner</h1>
            <div id="directionsList">
            </div>
            <div><p>Iteration count:&nbsp;<span id="iterationCount">0</span></p></div>
            <button id="nextStepButton" onclick="next()" type="button">
                Next Iteration
            </button>
            <button id="Button1" onclick="auto()" type="button">
                Auto Run
            </button>
            
            <div id="dirList">
            </div>
        </div>
    </div>
    </form>
</body>
</html>
