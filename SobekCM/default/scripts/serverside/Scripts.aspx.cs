using System;
using System.IO;
using System.Web.Script.Services;
using System.Web.Services;
using SobekCM.Library.Users;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;

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
                string savedItemCoords = ar[1];
                //get id of item
                //save to mets
                //save to db
                break;
            case "overlay":
                string savedOverlayIndex = ar[1];
                string savedOverlayBounds = ar[2];
                string savedOverlaySource = ar[3];
                string savedOverlayRotation = ar[4];
                //save to mets
                //save to db
                break;
            case "poi":
                string savedPOIType = ar[1];
                string savedPOIDesc = ar[2];
                string savedPOIKML = ar[3];
                //save
                
                //get specific geometry  KML Standard (not used)
                //switch (ar[1]) {
                //    case "marker":
                //        string savedMarkerDesc = ar[1];
                //        string savedMarkerCoords = ar[2];
                //        //save
                //        break;
                //    case "circle":
                //        string savedCircleDesc = ar[1];
                //        string savedCircleCenter = ar[2];
                //        string savedCircleRadius = ar[3];
                //        break;
                //    case "rectangle":
                //        string savedRectangleDesc = ar[1];
                //        string savedRectangleBounds = ar[2];
                //        break;
                //    case "polygon":
                //        string savedPolygonDesc = ar[1];
                //        string savedPolygonPoints = ar[2];
                //        break;
                //    case "polyline":
                //        string savedPolyLineDesc = ar[1];
                //        string savedPolyLinePoints = ar[2];
                //        break;
                //}

                break;
        }

        User_Object Current_User = new User_Object();
        SobekCM_Item Current_Item = new SobekCM_Item();

        string userInProcessDirectory = Current_User.User_InProcess_Directory("mapwork");

        // Ensure the user's process directory exists
        if (!Directory.Exists(userInProcessDirectory))
            Directory.CreateDirectory(userInProcessDirectory);

        // SAVE!

        // Ensure we have a geo-spatial module in the digital resource
        GeoSpatial_Information myGeo = Current_Item.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
        if (myGeo == null)
        {
            myGeo = new GeoSpatial_Information();
            Current_Item.Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, myGeo);
        }

        // Save the item to the temporary location
        Current_Item.Save_METS(userInProcessDirectory + "\\" + Current_Item.BibID + "_" + Current_Item.VID + ".xml");

        string result = "received: " + Convert.ToString(sendData);
        return result;
    }

}