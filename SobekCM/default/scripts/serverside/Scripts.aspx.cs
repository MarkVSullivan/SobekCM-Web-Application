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
  public static void SaveItem(String sendData)
  {
    SobekCM.Library.ItemViewer.Viewers.Google_Coordinate_Entry_ItemViewer.SaveContent(sendData);
  }

}
