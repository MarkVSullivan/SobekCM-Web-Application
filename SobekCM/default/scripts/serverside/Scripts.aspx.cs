using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.IO;

public partial class default_scripts_serverside_Scripts : System.Web.UI.Page
{
    [WebMethod, ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string SaveItem(String sendData)
    {

        int index = sendData.LastIndexOf("|");
        string[] ar = sendData.Substring(0, index).Split('|');

        string saveType = ar[0]; //get what type of save it is

        switch (saveType)
        {
            case "item":
                string savedItemCoord = ar[1];
                break;
            case "overlay":
                string savedOverlayBounds = ar[1];
                string savedOverlaySource = ar[2];
                string savedOverlayRotation = ar[3];
                break;
            case "poi":
                //poi
                break;
        }
        
        string result = "received: " + sendData;
        return result;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}