<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SobekCM.aspx.cs" Inherits="UFDC" validateRequest="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
  <title><%Write_Page_Title();%></title>
    
  <!-- <% Repository_Title(); %> -->

<%Write_Within_HTML_Head();%>
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