#region Using directives

using System;
using System.Web.Services;
using System.Web.UI;
using SobekCM.Library.Database;
using SobekCM.Library.ResultsViewer;
using SobekCM.UI_Library;

#endregion

public partial class CallBacks : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static object MapSearch(string sendData)
    {
        SobekCM_Database.Connection_String = UI_ApplicationCache_Gateway.Settings.Database_Connections[0].Connection_String;
        return Google_Map_ResultsViewer.Process_MapSearch_Callback(sendData);
    }
}
