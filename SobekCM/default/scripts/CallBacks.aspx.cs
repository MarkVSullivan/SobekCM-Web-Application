using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using SobekCM.Core.Settings;
using SobekCM.Library.Database;
using SobekCM.Library.ResultsViewer;
using SobekCM.Library.Settings;

public partial class CallBacks : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [System.Web.Services.WebMethod]
    public static object MapSearch(string sendData)
    {
        SobekCM_Database.Connection_String = InstanceWide_Settings_Singleton.Settings.Database_Connections[0].Connection_String;
        return Google_Map_ResultsViewer.Process_MapSearch_Callback(sendData);
    }
}
