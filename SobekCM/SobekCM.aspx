<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SobekCM.aspx.cs" Inherits="SobekMain" validateRequest="false" %>
<!DOCTYPE html>
<html>
<head>
  <title><%Write_Page_Title();%></title>
    
  <!-- <% Repository_Title(); %> -->

<%Write_Within_HTML_Head();%>

</head>
<body <%Write_Body_Attributes();%>>

<%Write_Html();%>

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