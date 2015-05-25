<%@ Page Language="C#" AutoEventWireup="true" validateRequest="false" CodeBehind="SobekCM.aspx.cs" Inherits="SobekCM.SobekCM" %>
<!DOCTYPE html>
<html lang="<%Write_Lang_Code();%>">
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