<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SobekCM.aspx.cs" Inherits="SobekMain" validateRequest="false" %>
<!DOCTYPE html>
<html>
<head>
  <title><%Write_Page_Title();%></title>
    
  <!-- <% Repository_Title(); %> -->

<%Write_Within_HTML_Head();%>
	
  <!--[if lt IE 9]>
    <script src="default/scripts/html5shiv/html5shiv.js"></script>
  <![endif]-->
</head>
<body <%Write_Body_Attributes();%>>

<%Write_Html();%>

<form id="fileUploadForm" runat="server" >
    <asp:PlaceHolder id="myUfdcUploadPlaceHolder" runat="server"></asp:PlaceHolder>
<%Write_UploadForm_Additional_HTML();%>
</form>

<form id="itemNavForm" runat="server">
<%Write_ItemNavForm_Opening();%>
    <asp:PlaceHolder id="tocPlaceHolder" runat="server"></asp:PlaceHolder>
<%Write_ItemNavForm_Additional_HTML();%>
    <asp:PlaceHolder id="mainPlaceHolder" runat="server"></asp:PlaceHolder>
<%Write_ItemNavForm_Closing();%>
</form>

<%Write_Final_HTML();%>
    
</body>
</html>