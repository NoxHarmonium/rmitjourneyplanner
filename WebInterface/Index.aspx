<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="WebInterface.Index" %>

<%@ Register src="WebControls/GoogleMapsControl.ascx" tagname="GoogleMapsControl" tagprefix="uc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Test page</title>
    
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no" />
    <style type="text/css">
      html { height: 100% }
      body { height: 100%; margin: 0; padding: 0 }
      #map_canvas { height: 100% }
    </style>

</head>
<body>
    <uc1:GoogleMapsControl ID="GoogleMapsControl1" runat="server" />
    <form id="form1" runat="server">
    
    
        
    
    
    </form>
</body>
</html>

