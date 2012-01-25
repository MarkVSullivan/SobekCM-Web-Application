<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UploadProgress.aspx.cs" Inherits="UploadProgress" %>
<%@ Register Assembly="FileUploadLibrary" Namespace="darrenjohnstone.net.FileUpload" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Upload Progress</title>
    <meta http-equiv="refresh" content="5">
    <style type="text/css">
    .upProgressContainer
    {
	    padding-top: 5px;
	    padding-bottom: 5px;
    }

    .upOuterBar
    {
        width: 570px;
        height: 32px;
        border: solid 1px #8a8a8a;
        background-color: #fff;
        overflow: hidden;
    }

    .upInnerBar
    {
        width: 0;
        height: 32px;
        background-color: #cccccc;
        position: relative;
    }

    .upLabel
    {
        width: 100%;
        background-color: Transparent;
        color: #000000;
        text-align: center;
        z-index: 9999;
        position: relative;
        top: -32px;
        font-family: Arial;
        font-size: 9pt;
        font-weight: bold;
    }
    </style>
    <%Add_Stylesheet_Links();%>
    <base target="_blank"/>    
</head>
<body>
    <form id="MainForm" runat="server" target="_blank">
    <div class="upContainer">
        <div class="upOuterBar">
            <asp:Panel runat="server" ID="upProgressBar" CssClass="upInnerBar">
            </asp:Panel>
            <div id="upLabel" class="upLabel">
                <asp:Label runat="server" ID="lblStatus" />
            </div>
        </div>
    </div>
    </form>
</body>
</html>
