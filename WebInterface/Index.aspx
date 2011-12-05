<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits=" RmitJourneyPlanner.WebInterface.Index" %>

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
    <form id="form1" runat="server" >
    <div id="sideHatch" runat="server">
        <div id="innerSideHatch" runat="server">
            <h1>
                RMIT Travel Planner
            </h1>
            <div id="topPanel" >
                <div class="textBoxRow">
                    <img alt="Icon of number 0" class="transportImage" src="images/icons/number_0.png" /><asp:TextBox
                        ID="txtSource" runat="server" Text="13 Liverpool St, Coburg, Victoria, Australia"></asp:TextBox></div>
                <div class="textBoxRow">
                    <img alt="Icon of number 1" class="transportImage" src="images/icons/number_1.png" /><asp:TextBox
                        ID="txtDestination" runat="server" Text="1 Lygon St, Carlton, Victoria, Australia"></asp:TextBox></div>
                <div class="textBoxRow">
                    Max walking distance (KM) :<asp:TextBox ID="txtMaxWalk" runat="server" Text="1.5"></asp:TextBox>
                    <asp:Button ID="btnReset" runat="server" Text="Search" />
                </div>
                <div>
                    <p>
                        Iteration count:&nbsp;<span id="iterationCount">0</span>
                    </p>
                </div>
            </div>
            <div>
                <button id="nextStepButton" onclick="next('true'); " type="button">
                    Next Iteration
                </button>
                <button id="autoButton" onclick="auto(); " type="button">
                    Auto Run
                </button>
            </div>
            <p>&nbsp;</p>
            <h3>Best route so far:</h3>
            <div id="dirList">
            </div>
        </div>
    </div>
    </form>
</body>
</html>
