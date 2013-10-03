using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using SobekCM.Library.HTML;
using SobekCM.Library.Users;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;
using SobekCM.Library.Navigation;
using SobekCM.Resource_Object;
using SobekCM.Library.Settings;

namespace SobekCM.Library.ItemViewer.Viewers
{

    /// <summary> Class to allow a user to add coordinate information to 
    /// a digital resource ( map coverage, points of interest, etc.. ) </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Google_Coordinate_Entry_ItemViewer : abstractItemViewer
    {
        private StringBuilder mapBuilder;
        private List<string> matchingTilesList;
        private static User_Object CurrentUser;
        private static SobekCM_Item CurrentItem;
        private static GeoSpatial_Information geoInfo2;
        private static GeoSpatial_Information itemGeoInfo;
        private static List<Coordinate_Polygon> itemPolygons;
        List<Coordinate_Polygon> allPolygons;
        List<Coordinate_Point> allPoints;
        List<Coordinate_Line> allLines;

        List<Coordinate_Polygon> POI_allPolygons;
        List<Coordinate_Point> POI_allPoints;
        List<Coordinate_Line> POI_allLines;

        //private static string resource_directory = SobekCM_Library_Settings.Image_Server_Network + CurrentItem.Web.AssocFilePath;
        //private static string current_mets = resource_directory + CurrentItem.METS_Header.ObjectID + ".mets.xml";
        
        //init viewer instance
        public Google_Coordinate_Entry_ItemViewer(User_Object Current_User, SobekCM_Item Current_Item, SobekCM_Navigation_Object Current_Mode)
        {
            CurrentUser = Current_User;
            CurrentItem = Current_Item;
            this.CurrentMode = Current_Mode;

            //string resource_directory = SobekCM_Library_Settings.Image_Server_Network + CurrentItem.Web.AssocFilePath;
            //string current_mets = resource_directory + CurrentItem.METS_Header.ObjectID + ".mets.xml";

            // If there is no user, send to the login
            if (CurrentUser == null)
            {
                CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
                CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
                CurrentMode.Redirect();
                return;
            }


            string action = HttpContext.Current.Request.Form["action"] ?? String.Empty;
            string payload = HttpContext.Current.Request.Form["payload"] ?? String.Empty;


            // See if there were hidden requests
            if (!String.IsNullOrEmpty(action))
            {
               if ( action == "save")
                   SaveContent(payload);
            }

            ////create a backup of the mets
            //string backup_directory = SobekCM_Library_Settings.Image_Server_Network + Current_Item.Web.AssocFilePath + SobekCM_Library_Settings.Backup_Files_Folder_Name;
            //string backup_mets_name = backup_directory + "\\" + CurrentItem.METS_Header.ObjectID + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".mets.bak";
            //File.Copy(current_mets, backup_mets_name);
        }

        //parse and save incoming message 
        public static void SaveContent(String sendData)
        {
            //get rid of excess string
            sendData = sendData.Replace("{\"sendData\": \"", "").Replace("{\"sendData\":\"", "");

            //validate
            if (sendData.Length == 0)
                return;

            //ensure we have a geo-spatial module in the digital resource
            GeoSpatial_Information resourceGeoInfo = CurrentItem.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
            //if there was no geo-spatial module
            if (resourceGeoInfo == null)
            {
                //create new geo-spatial module, if we do not already have one
                resourceGeoInfo = new GeoSpatial_Information();
                CurrentItem.Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, resourceGeoInfo);
            }

            //create a new list of all the polygons for a resource item
            itemPolygons = new List<Coordinate_Polygon>();
            List<abstract_TreeNode> pages = CurrentItem.Divisions.Physical_Tree.Pages_PreOrder;
            for (int i = 0; i < pages.Count; i++)
            {
                abstract_TreeNode pageNode = pages[i];
                itemGeoInfo = pageNode.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                if ((itemGeoInfo != null) && (itemGeoInfo.hasData))
                {
                    if (itemGeoInfo.Polygon_Count > 0)
                    {
                        foreach (Coordinate_Polygon thisPolygon in itemGeoInfo.Polygons)
                        {
                            thisPolygon.Page_Sequence = (ushort)(i + 1);
                            itemPolygons.Add(thisPolygon);
                        }
                    }
                }
            }

            //get the length of incoming message
            int index1 = sendData.LastIndexOf("~");
            //split into each save message
            string[] allSaves = sendData.Substring(0, index1).Split('~');
            //hold save type handle
            string saveTypeHandle = null;
            bool firedOnce = true; //check to see if save poi clear has already been fired...
            
            for (int i = 0; i < allSaves.Length; i++)
            {
                //get the length of save message
                int index2 = allSaves[i].LastIndexOf("|");
                //split into save elements
                string[] ar = allSaves[i].Substring(0, index2).Split('|');
                //determine the save type handle (position 0 in array)
                saveTypeHandle = ar[0];
                //determine the save type (position 1 in array)
                string saveType = ar[1];
                //based on saveType, parse into objects

                if (saveTypeHandle == "save")
                {
                    //handle save based on type
                    switch (saveType)
                    {
                        case "item":

                            //prep incoming lat/long
                            string[] temp1 = ar[2].Split(',');
                            double temp1Lat = Convert.ToDouble(temp1[0].Replace("(", ""));
                            double temp1Long = Convert.ToDouble(temp1[1].Replace(")", ""));

                            //clear previous point (if any)
                            resourceGeoInfo.Clear_Points();

                            Coordinate_Point newPoint = new Coordinate_Point(temp1Lat, temp1Long, CurrentItem.METS_Header.ObjectID, "main");
                            //newPoint.Add_FeatureType("main");

                            //add the new point 
                            resourceGeoInfo.Add_Point(newPoint);

                            //save to database
                            //resourceGeoInfo.Save_Additional_Info_To_Database(CurrentItem.id, "connectionString", CurrentItem, "Could Not Save Item To Database");

                            break;
                        case "overlay":
                            //search through existing overlays and modify if match found
                            if (itemPolygons.Count > 0)
                            {
                                foreach (Coordinate_Polygon itemPolygon in itemPolygons)
                                {
                                    if (itemPolygon.Label == ar[2])
                                    {
                                        //prep incoming bounds
                                        string[] temp2 = ar[3].Split(',');
                                        itemPolygon.Clear_Edge_Points();
                                        itemPolygon.Add_Edge_Point(Convert.ToDouble(temp2[0].Replace("(", "")), Convert.ToDouble(temp2[1].Replace(")", "")));
                                        itemPolygon.Add_Edge_Point(Convert.ToDouble(temp2[2].Replace("(", "")), Convert.ToDouble(temp2[3].Replace(")", "")));
                                        itemPolygon.Recalculate_Bounding_Box();

                                        //add the rotation
                                        itemPolygon.Add_Rotation(Convert.ToDouble(ar[5]));
                                    }
                                }
                            }
                            else
                            {
                                //create new polygon
                                Coordinate_Polygon itemPolygon = new Coordinate_Polygon();

                                //add the label
                                if (ar[2] != "undefined")
                                {
                                    itemPolygon.Label = ar[2];
                                }
                                else
                                {
                                    itemPolygon.Label = CurrentItem.Bib_Title;
                                }


                                //prep incoming bounds
                                string[] temp2 = ar[3].Split(',');
                                itemPolygon.Add_Edge_Point(Convert.ToDouble(temp2[0].Replace("(", "")), Convert.ToDouble(temp2[1].Replace(")", "")));
                                itemPolygon.Add_Edge_Point(Convert.ToDouble(temp2[2].Replace("(", "")), Convert.ToDouble(temp2[3].Replace(")", "")));
                                itemPolygon.Recalculate_Bounding_Box();

                                //add the rotation
                                itemPolygon.Add_Rotation(Convert.ToDouble(ar[5]));

                                //add the feature type 
                                itemPolygon.Add_FeatureType("main");

                                //clear previous point (if any)
                                resourceGeoInfo.Clear_Points();

                                //clear previous overlay (if any)
                                resourceGeoInfo.Clear_User_Polygons_And_Lines();

                                //check to see if there is a lower level geo info
                                if (itemGeoInfo != null)
                                {
                                    //add the polygon to the geo info
                                    itemGeoInfo.Add_Polygon(itemPolygon);
                                }
                                else
                                {
                                    //add the polygon to the resource level geo info
                                    resourceGeoInfo.Add_Polygon(itemPolygon);
                                }

                            }

                            //add current geo
                            if (itemGeoInfo != null)
                            {
                                //add to item
                                CurrentItem.Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, itemGeoInfo);
                            }
                            else
                            {
                                if (resourceGeoInfo != null)
                                {
                                    //clear previous point (if any) (used if the overlay was converted from single point)
                                    resourceGeoInfo.Clear_Points();
                                    //add to resource
                                    CurrentItem.Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, resourceGeoInfo);
                                }
                            }


                            break;
                        case "poi":

                            if (firedOnce)
                            {
                                //clear previous poi points
                                resourceGeoInfo.Clear_POIs();
                                firedOnce = false;
                            }
                            

                            //assign values
                            string savedPOIType = ar[2];
                            string savedPOIDesc = ar[3];
                            string savedPOIKML = ar[4];

                            //get specific geometry (KML Standard)
                            switch (ar[2])
                            {
                                case "marker":

                                    //prep incoming lat/long
                                    string[] temp2 = ar[4].Split(',');
                                    double temp2Lat = Convert.ToDouble(temp2[0].Replace("(", ""));
                                    double temp2Long = Convert.ToDouble(temp2[1].Replace(")", ""));

                                    //add the new point 
                                    resourceGeoInfo.Add_Point(temp2Lat, temp2Long, ar[3], "poi");

                                    break;
                                case "circle":

                                    //create new circle
                                    Coordinate_Polygon poiCircle = new Coordinate_Polygon();

                                    //set the label
                                    poiCircle.Label = ar[3];

                                    //set the radius
                                    poiCircle.Add_Radius(Convert.ToDouble(ar[5]));

                                    //add the feature type
                                    poiCircle.Add_FeatureType("poi");

                                    //prep incoming lat/long
                                    string[] temp3 = ar[4].Split(',');
                                    double temp3Lat = Convert.ToDouble(temp3[0].Replace("(", ""));
                                    double temp3Long = Convert.ToDouble(temp3[1].Replace(")", ""));

                                    //add the center point
                                    poiCircle.Add_Edge_Point(temp3Lat, temp3Long, "circleCenter");

                                    //add to the resource obj
                                    resourceGeoInfo.Add_POI_Circle(poiCircle);

                                    break;
                                case "rectangle":

                                    //create new polygon
                                    Coordinate_Polygon poiRectangle = new Coordinate_Polygon();

                                    //add the label
                                    poiRectangle.Label = ar[3];

                                    //add the feature type
                                    poiRectangle.Add_FeatureType("poi");

                                    //prep incoming bounds
                                    string[] temp4 = ar[4].Split(',');
                                    poiRectangle.Add_Edge_Point(Convert.ToDouble(temp4[0].Replace("(", "")), Convert.ToDouble(temp4[1].Replace(")", "")));
                                    poiRectangle.Add_Edge_Point(Convert.ToDouble(temp4[2].Replace("(", "")), Convert.ToDouble(temp4[3].Replace(")", "")));
                                    poiRectangle.Recalculate_Bounding_Box();

                                    //add to resource obj
                                    resourceGeoInfo.Add_POI_Polygon(poiRectangle);

                                    break;
                                case "polygon":

                                    //create new polygon
                                    Coordinate_Polygon poiPolygon = new Coordinate_Polygon();

                                    //add the label
                                    poiPolygon.Label = ar[3];

                                    //add the feature type
                                    poiPolygon.Add_FeatureType("poi");

                                    //add the edge points
                                    for (int i2 = 5; i2 < ar.Length; i2++)
                                    {
                                        string[] temp5 = ar[i2].Split(',');
                                        poiPolygon.Add_Edge_Point(Convert.ToDouble(temp5[0].Replace("(", "")), Convert.ToDouble(temp5[1].Replace(")", "")));
                                    }

                                    //add the polygon
                                    resourceGeoInfo.Add_POI_Polygon(poiPolygon);

                                    break;
                                case "polyline":

                                    //create new line
                                    Coordinate_Line poiLine = new Coordinate_Line();

                                    //add the label
                                    poiLine.Label = ar[3];

                                    //add the feature type
                                    poiLine.Add_FeatureType("poi");

                                    //add the edge points
                                    for (int i2 = 5; i2 < ar.Length; i2++)
                                    {
                                        Coordinate_Point tempPoint = new Coordinate_Point();

                                        string[] temp5 = ar[i2].Split(',');
                                        poiLine.Add_Point(Convert.ToDouble(temp5[0].Replace("(", "")), Convert.ToDouble(temp5[1].Replace(")", "")), "");
                                    }

                                    //add the line
                                    resourceGeoInfo.Add_POI_Line(poiLine);

                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    //skip to the apply
                }
            }

            //create inprocessing directory
            string userInProcessDirectory = CurrentUser.User_InProcess_Directory("mapwork");
            //ensure the user's process directory exists
            if (!Directory.Exists(userInProcessDirectory))
            {
                Directory.CreateDirectory(userInProcessDirectory);
            }
            string resource_directory = SobekCM_Library_Settings.Image_Server_Network + CurrentItem.Web.AssocFilePath;
            string current_mets = resource_directory + CurrentItem.METS_Header.ObjectID + ".mets.xml";
            string metsInProcessFile = userInProcessDirectory + "\\" + CurrentItem.BibID + "_" + CurrentItem.VID + ".mets.xml";

            //determine what to do from here
            switch (saveTypeHandle)
            {
                case "save":
                    //save the item to the temporary location
                    CurrentItem.Save_METS(userInProcessDirectory + "\\" + CurrentItem.BibID + "_" + CurrentItem.VID + ".mets.xml");
                    //CurrentItem.Save_SobekCM_METS(); //save the mets
                    break;
                case "apply":
                    //in theory, this should only be called after the 'save' thus we do not need to save only apply (IE send to live)

                    //move temp mets to prod
                    File.Copy(metsInProcessFile, current_mets, true);
                    File.Delete(metsInProcessFile); //delete it
                    
                    //save the mets (to live)    
                    //CurrentItem.Save_SobekCM_METS();
                    //save to db
                    Resource_Object.Database.SobekCM_Database.Save_Digital_Resource(CurrentItem);
                    break;
                case "reset":
                    //currently not used
                    //reset temp mets
                    //if (File.Exists(userInProcessDirectory + "\\" + CurrentItem.BibID + "_" + CurrentItem.VID + ".mets.xml"))
                    //{
                    //    File.Delete(userInProcessDirectory + "\\" + CurrentItem.BibID + "_" + CurrentItem.VID + ".mets.xml");
                    //}
                    break;
            }
        }

        /// <summary> Gets the number of pages for this viewer </summary>
        /// <value> This is a single page viewer, so this property always returns the value 1</value>
        public override int PageCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary> Gets the flag that indicates if the page selector should be shown </summary>
        /// <value> This is a single page viewer, so this property always returns NONE</value>
        public override ItemViewer_PageSelector_Type_Enum Page_Selector
        {
            get
            {
                return ItemViewer_PageSelector_Type_Enum.NONE;
            }
        }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value 800 </value>
        public override int Viewer_Width
        {
            get { return -1; }
        }

        /// <summary> Property gets the type of item viewer </summary>
        /// <value> This always returns ItemViewer_Type_Enum.Google_Coordinate_Entry </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Google_Coordinate_Entry; }
        }

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        public override void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            Body_Attributes.Clear();
            Body_Attributes.Add(new Tuple<string, string>("onresize", "resizeView();"));
        }

        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum> 
                    {
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_NonWindowed_Mode,
                        HtmlSubwriter_Behaviors_Enum.Suppress_Footer,
                        HtmlSubwriter_Behaviors_Enum.Suppress_Internal_Header,
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Item_Menu,
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Left_Navigation_Bar
                    };
            }
        }

        /// <summary> Abstract method adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            // Start to build the response
            StringBuilder mapeditBuilder = new StringBuilder();

            //page content
            mapeditBuilder.AppendLine("<td>");
            

            //used to force doctype html5 and css3
            //mapeditBuilder.AppendLine("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">");

            mapeditBuilder.AppendLine("<input type=\"hidden\" id=\"action\" name=\"action\" value=\"\" />");
            mapeditBuilder.AppendLine("<input type=\"hidden\" id=\"payload\" name=\"payload\" value=\"\" />");
            mapeditBuilder.AppendLine("");

            //standard css
            mapeditBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/jquery-ui.css\"/>");
            mapeditBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/jquery-searchbox.css\"/>");
            mapeditBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/lytebox/lytebox.css\"/>");
            mapeditBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/lytebox/lytebox.css\"/>");
            mapeditBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_ItemMenus.css\"/>");

            //custom css
            mapeditBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_Mapedit_Theme_Default.css\"/>");
            mapeditBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_Mapedit_Layout_Default.css\"/>");
            mapeditBuilder.AppendLine("<link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_Mapedit_Other.css\"/>");

            mapeditBuilder.AppendLine("");
            mapeditBuilder.AppendLine(" <div id=\"mapedit_blanket_loading\"><div>Loading...<br/><br/><img src=\"" + CurrentMode.Base_URL + "default/images/mapedit/ajax-loader.gif\"></div></div>");
            mapeditBuilder.AppendLine("");

            //standard js files
            mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/jquery-ui-1.10.1.js\"></script>");
            mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/jquery-migrate-1.1.1.min.js\"></script>");
            mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/jquery-rotate.js\"></script>");
            mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/jquery-knob.js\"></script>");
            mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/jquery-json-2.4.min\"></script>");
            mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/lytebox/lytebox.js\"></script>");
            mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/gmaps-infobox.js\"></script>");
            mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/superfish/superfish.js\"></script>");
            mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/superfish/hoverintent.js\"></script>");
            mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"http://maps.googleapis.com/maps/api/js?key=AIzaSyCzliz5FjUlEI9D2605b33-etBrENSSBZM&sensor=false\"></script>");
            mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&libraries=drawing\"></script>");

            //custom js 
            #region

            mapeditBuilder.AppendLine(" ");
            mapeditBuilder.AppendLine(" <script type=\"text/javascript\"> ");
            mapeditBuilder.AppendLine(" ");

            //setup server to client vars writer
            mapeditBuilder.AppendLine(" <!-- Add Server Vars -->");
            mapeditBuilder.AppendLine(" function initServerToClientVars(){ ");
            mapeditBuilder.AppendLine("   <!-- Add Base URL Var -->");
            mapeditBuilder.AppendLine("   globalVar.baseURL = \"" + CurrentMode.Base_URL + "\"; ");
            mapeditBuilder.AppendLine(" } ");
            mapeditBuilder.AppendLine(" ");

            //geo objects writer section 
            mapeditBuilder.AppendLine(" <!-- Begin Geo Objects Writer --> ");
            mapeditBuilder.AppendLine(" ");
            mapeditBuilder.AppendLine(" function initGeoObjects(){ ");
            mapeditBuilder.AppendLine(" ");

            #endregion

            //write geo objects as js vars
            #region

            mapBuilder = new StringBuilder();

            // Keep track of any matching tiles
            matchingTilesList = new List<string>();

            // Add the points
            if (CurrentItem != null)
            {
                allPolygons = new List<Coordinate_Polygon>();
                allPoints = new List<Coordinate_Point>();
                allLines = new List<Coordinate_Line>();

                POI_allPolygons = new List<Coordinate_Polygon>();
                POI_allPoints = new List<Coordinate_Point>();
                POI_allLines = new List<Coordinate_Line>();

                // Collect all the polygons, points, and lines
                GeoSpatial_Information geoInfo = CurrentItem.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                if ((geoInfo != null) && (geoInfo.hasData))
                {
                    if (geoInfo.Polygon_Count > 0)
                    {
                        foreach (Coordinate_Polygon thisPolygon in geoInfo.Polygons)
                            allPolygons.Add(thisPolygon);
                    }
                    if (geoInfo.Line_Count > 0)
                    {
                        foreach (Coordinate_Line thisLine in geoInfo.Lines)
                            allLines.Add(thisLine);
                    }
                    if (geoInfo.Point_Count > 0)
                    {
                        foreach (Coordinate_Point thisPoint in geoInfo.Points)
                            allPoints.Add(thisPoint);
                    }
                    //if (geoInfo.POI_Polygon_Count > 0)
                    //{
                    //    foreach (Coordinate_Polygon thisPolygon in geoInfo.POI_Polygons)
                    //        POI_allPolygons.Add(thisPolygon);
                    //}
                    //if (geoInfo.POI_Line_Count > 0)
                    //{
                    //    foreach (Coordinate_Line thisLine in geoInfo.POI_Lines)
                    //        POI_allLines.Add(thisLine);
                    //}
                    //if (geoInfo.POI_Point_Count > 0)
                    //{
                    //    foreach (Coordinate_Point thisPoint in geoInfo.POI_Points)
                    //        POI_allPoints.Add(thisPoint);
                    //}
                }


                List<abstract_TreeNode> pages = CurrentItem.Divisions.Physical_Tree.Pages_PreOrder;
                for (int i = 0; i < pages.Count; i++)
                {
                    abstract_TreeNode pageNode = pages[i];
                    geoInfo2 = pageNode.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                    if ((geoInfo2 != null) && (geoInfo2.hasData))
                    {
                        if (geoInfo2.Polygon_Count > 0)
                        {
                            foreach (Coordinate_Polygon thisPolygon in geoInfo2.Polygons)
                            {
                                thisPolygon.Page_Sequence = (ushort)(i + 1);
                                allPolygons.Add(thisPolygon);
                            }
                        }
                        if (geoInfo2.Line_Count > 0)
                        {
                            foreach (Coordinate_Line thisLine in geoInfo2.Lines)
                            {
                                allLines.Add(thisLine);
                            }
                        }
                        if (geoInfo2.Point_Count > 0)
                        {
                            foreach (Coordinate_Point thisPoint in geoInfo2.Points)
                            {
                                allPoints.Add(thisPoint);
                            }
                        }
                    }
                }

                // Add all the polygons now
                List<string> polygonBounds = new List<string>();
                List<string> polygonURL = new List<string>();
                List<double> polygonRotation = new List<double>();
                int it = 0;
                if ((allPolygons.Count > 0) && (allPolygons[0].Edge_Points_Count > 1))
                {
                    // Add each polygon 
                    foreach (Coordinate_Polygon itemPolygon in allPolygons)
                    {
                        //get and set the bounds
                        string bounds = "new google.maps.LatLngBounds( ";
                        int localit = 0;
                        string bounds1 = "new google.maps.LatLng";
                        string bounds2 = "new google.maps.LatLng";
                        foreach (Coordinate_Point thisPoint in itemPolygon.Edge_Points)
                        {
                            if (itemPolygon.Edge_Points_Count == 2)
                            {
                                if (localit == 0)
                                {
                                    bounds2 += "(" + Convert.ToString(thisPoint.Latitude) + "," + Convert.ToString(thisPoint.Longitude) + ")";
                                }
                                if (localit == 1)
                                {
                                    bounds1 += "(" + Convert.ToString(thisPoint.Latitude) + "," + Convert.ToString(thisPoint.Longitude) + ")";
                                }
                                localit++;
                            }
                            if (itemPolygon.Edge_Points_Count == 4)
                            {
                                if (localit == 0)
                                {
                                    bounds1 += "(" + Convert.ToString(thisPoint.Latitude) + "," + Convert.ToString(thisPoint.Longitude) + ")";
                                }
                                if (localit == 2)
                                {
                                    bounds2 += "(" + Convert.ToString(thisPoint.Latitude) + "," + Convert.ToString(thisPoint.Longitude) + ")";
                                }
                                localit++;
                            }
                        }
                        bounds += bounds2 + ", " + bounds1;
                        bounds += ")";
                        polygonBounds.Add(bounds);
                        mapeditBuilder.AppendLine("      globalVar.incomingOverlayBounds[" + it + "] = " + bounds + ";");

                        //add the label of the polygon
                        mapeditBuilder.AppendLine("      globalVar.incomingOverlayLabel[" + it + "] = \"" + itemPolygon.Label + "\";");

                        //get the image url
                        List<SobekCM_File_Info> first_page_files = ((Page_TreeNode)CurrentItem.Divisions.Physical_Tree.Pages_PreOrder[it]).Files;
                        string first_page_jpeg = String.Empty;
                        foreach (SobekCM_File_Info thisFile in first_page_files)
                        {
                            if ((thisFile.System_Name.ToLower().IndexOf(".jpg") > 0) &&
                                (thisFile.System_Name.ToLower().IndexOf("thm.jpg") < 0))
                            {
                                first_page_jpeg = thisFile.System_Name;
                                break;
                            }
                        }
                        string first_page_complete_url = "\"" + CurrentItem.Web.Source_URL + "/" + first_page_jpeg + "\"";
                        //polygonURL[it] = first_page_complete_url;
                        polygonURL.Add(first_page_complete_url);
                        mapeditBuilder.AppendLine("      globalVar.incomingOverlaySourceURL[" + it + "] = " + polygonURL[it] + ";");

                        //get and set the rotation value
                        //polygonRotation.Add(0);
                        //mapeditBuilder.AppendLine("      globalVar.incomingOverlayRotation[" + it + "] = " + polygonRotation[it] + ";");
                        mapeditBuilder.AppendLine("      globalVar.incomingOverlayRotation[" + it + "] = " + itemPolygon.polygonRotation + ";");

                        mapeditBuilder.AppendLine("      globalVar.incomingOverlayFeatureType[" + it + "] = \"" + itemPolygon.featureType + "\";");

                        //iterate
                        it++;

                    }
                    mapeditBuilder.AppendLine(" ");
                    mapeditBuilder.AppendLine("      displayIncomingOverlays(); ");
                    mapeditBuilder.AppendLine(" ");
                }

                // Draw all the single points 
                if (allPoints.Count > 0)
                {
                    //add each point
                    for (int point = 0; point < allPoints.Count; point++)
                    {
                        mapeditBuilder.AppendLine("      globalVar.incomingPointFeatureType[" + point + "] = \"" + allPoints[point].FeatureType + "\";");

                        mapeditBuilder.AppendLine("      globalVar.incomingPointCenter[" + point + "] = new google.maps.LatLng(" + allPoints[point].Latitude + "," + allPoints[point].Longitude + "); ");

                        if (allPoints[point].Label != "")
                        {
                            mapeditBuilder.AppendLine("      globalVar.incomingPointLabel[" + point + "] = \"" + allPoints[point].Label + "\"; ");
                        }
                        else
                        {
                            mapeditBuilder.AppendLine("      globalVar.incomingPointLabel[" + point + "] = \"" + CurrentItem.Bib_Title + "\"; ");
                        }


                        try
                        {
                            //get the image url
                            List<SobekCM_File_Info> first_page_files =
                                ((Page_TreeNode)CurrentItem.Divisions.Physical_Tree.Pages_PreOrder[it]).Files;
                            string first_page_jpeg = String.Empty;
                            foreach (SobekCM_File_Info thisFile in first_page_files)
                            {
                                if ((thisFile.System_Name.ToLower().IndexOf(".jpg") > 0) &&
                                    (thisFile.System_Name.ToLower().IndexOf("thm.jpg") < 0))
                                {
                                    first_page_jpeg = thisFile.System_Name;
                                    break;
                                }
                            }
                            string first_page_complete_url = CurrentItem.Web.Source_URL + "/" + first_page_jpeg;

                            mapeditBuilder.AppendLine("      globalVar.incomingPointSourceURL[" + point + "] = \"" + first_page_complete_url + "\"; ");
                        }
                        catch (Exception)
                        {
                            mapeditBuilder.AppendLine("      globalVar.incomingPointSourceURL[" + point + "] = \"\" ");
                            //throw;
                        }


                    }
                    mapeditBuilder.AppendLine(" ");
                    mapeditBuilder.AppendLine("      displayIncomingPoints();");
                    mapeditBuilder.AppendLine(" ");
                }
                else
                {
                    //this just preps in case we need to convert to an overlay
                    if (allPoints.Count > 0)
                    {
                        try
                        {
                            //get the image url
                            List<SobekCM_File_Info> first_page_files = ((Page_TreeNode) CurrentItem.Divisions.Physical_Tree.Pages_PreOrder[it]).Files;
                            string first_page_jpeg = String.Empty;
                            foreach (SobekCM_File_Info thisFile in first_page_files)
                            {
                                if ((thisFile.System_Name.ToLower().IndexOf(".jpg") > 0) &&
                                    (thisFile.System_Name.ToLower().IndexOf("thm.jpg") < 0))
                                {
                                    first_page_jpeg = thisFile.System_Name;
                                    break;
                                }
                            }
                            string first_page_complete_url = CurrentItem.Web.Source_URL + "/" + first_page_jpeg;

                            mapeditBuilder.AppendLine("      globalVar.incomingPointSourceURL[0] = \"" +
                                                      first_page_complete_url + "\"; ");
                            mapeditBuilder.AppendLine("      globalVar.incomingPointLabel[0] = \"" +
                                                      CurrentItem.Bib_Title + "\"; ");

                        }
                        catch (Exception)
                        {
                            mapeditBuilder.AppendLine("      globalVar.incomingPointSourceURL[0] = \"\" ");
                            mapeditBuilder.AppendLine("      globalVar.incomingPointLabel[0] = \"" +
                                                      CurrentItem.Bib_Title + "\"; ");
                            //throw;
                        }
                        mapeditBuilder.AppendLine(" ");
                        mapeditBuilder.AppendLine("      displayIncomingPoints();");
                        mapeditBuilder.AppendLine(" ");
                    }
                }

                mapeditBuilder.AppendLine(" }");
                mapeditBuilder.AppendLine(" ");
                mapeditBuilder.AppendLine(" <!-- End Geo Objects Writer --> ");
                mapeditBuilder.AppendLine(" ");
                mapeditBuilder.AppendLine(" </script> ");
                mapeditBuilder.AppendLine(" ");

            }

            #endregion

            bool rapidTest = CurrentMode.Base_URL.Contains("localhost");

            if (rapidTest == true)
            {
                //string mapeditHTMLFile = @""+ CurrentMode.Base_URL + "default/mapedit.txt";
                //string[] lines = File.ReadAllLines(@"http://hlmatt.com/uf/mapedit.txt");
                string[] lines = File.ReadAllLines(@"C:\Users\cadetpeters89\Documents\CUSTOM\projects\git\SobekCM-Web-Application\SobekCM\dev\mapedit\mapedit.html");
                for (int i = 0; i < lines.Length; i++)
                {
                    mapeditBuilder.AppendLine(Convert.ToString(lines[i]));
                }
            }
            else
            {
                //html page literal
                #region html page literat

                mapeditBuilder.AppendLine(" <div id=\"mapedit_container_message\"> ");
                mapeditBuilder.AppendLine("     <div id=\"content_message\"></div> ");
                mapeditBuilder.AppendLine(" </div> ");
                mapeditBuilder.AppendLine(" <div id=\"mapedit_container_pane_0\"> ");
                mapeditBuilder.AppendLine("     <ul class=\"sf-menu\"> ");
                mapeditBuilder.AppendLine("         <li> ");
                mapeditBuilder.AppendLine("             <a id=\"content_menubar_header1\"></a> ");
                mapeditBuilder.AppendLine("             <ul> ");
                mapeditBuilder.AppendLine("                 <li><a id=\"content_menubar_save\"></a></li> ");
                mapeditBuilder.AppendLine("                 <li><a id=\"content_menubar_cancel\"></a></li> ");
                mapeditBuilder.AppendLine("                 <li><a id=\"content_menubar_reset\"></a></li> ");
                mapeditBuilder.AppendLine("             </ul> ");
                mapeditBuilder.AppendLine("         </li> ");
                mapeditBuilder.AppendLine("         <li> ");
                mapeditBuilder.AppendLine("             <a id=\"content_menubar_header2\"></a> ");
                mapeditBuilder.AppendLine("             <ul> ");
                mapeditBuilder.AppendLine("                 <li> ");
                mapeditBuilder.AppendLine("                     <a id=\"content_menubar_header2Sub1\"></a> ");
                mapeditBuilder.AppendLine("                     <ul> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_toggleMapControls\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_toggleToolbox\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_toggleToolbar\"></a></li> ");
                mapeditBuilder.AppendLine("                     </ul> ");
                mapeditBuilder.AppendLine("                 </li> ");
                mapeditBuilder.AppendLine("                 <li> ");
                mapeditBuilder.AppendLine("                     <a id=\"content_menubar_header2Sub2\"></a> ");
                mapeditBuilder.AppendLine("                     <ul> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_layerRoadmap\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_layerSatellite\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_layerHybrid\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_layerTerrain\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_layerCustom\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_layerReset\"></a></li> ");
                mapeditBuilder.AppendLine("                     </ul> ");
                mapeditBuilder.AppendLine("                 </li> ");
                mapeditBuilder.AppendLine("                 <li> ");
                mapeditBuilder.AppendLine("                     <a id=\"content_menubar_header2Sub3\"></a> ");
                mapeditBuilder.AppendLine("                     <ul> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_zoomIn\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_zoomOut\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_zoomReset\"></a></li> ");
                mapeditBuilder.AppendLine("                     </ul> ");
                mapeditBuilder.AppendLine("                 </li> ");
                mapeditBuilder.AppendLine("                 <li> ");
                mapeditBuilder.AppendLine("                     <a id=\"content_menubar_header2Sub4\"></a> ");
                mapeditBuilder.AppendLine("                     <ul> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_panUp\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_panRight\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_panDown\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_panLeft\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_panReset\"></a></li> ");
                mapeditBuilder.AppendLine("                     </ul> ");
                mapeditBuilder.AppendLine("                 </li> ");
                mapeditBuilder.AppendLine("             </ul> ");
                mapeditBuilder.AppendLine("         </li> ");
                mapeditBuilder.AppendLine("         <li> ");
                mapeditBuilder.AppendLine("             <a id=\"content_menubar_header3\"></a> ");
                mapeditBuilder.AppendLine("             <ul>                ");
                mapeditBuilder.AppendLine("                 <li> ");
                mapeditBuilder.AppendLine("                     <a id=\"content_menubar_manageSearch\"></a> ");
                mapeditBuilder.AppendLine("                     <ul> ");
                mapeditBuilder.AppendLine("                         <li> ");
                mapeditBuilder.AppendLine("                             <div class=\"mapedit_container_search\"> ");
                mapeditBuilder.AppendLine("                                 <input id=\"content_menubar_searchField\" class=\"search\" type=\"text\" placeholder=\"\" onClick=\"this.select();\" /> ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_menubar_searchButton\" class=\"searchActionHandle\"></div> ");
                mapeditBuilder.AppendLine("                             </div> ");
                mapeditBuilder.AppendLine("                         </li> ");
                mapeditBuilder.AppendLine("                     </ul> ");
                mapeditBuilder.AppendLine("                 </li> ");
                mapeditBuilder.AppendLine("                 <li> ");
                mapeditBuilder.AppendLine("                     <a id=\"content_menubar_manageItem\"></a> ");
                mapeditBuilder.AppendLine("                     <ul> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_itemGetUserLocation\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_itemPlace\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_useSearchAsLocation\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_convertToOverlay\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_itemReset\"></a></li> ");
                mapeditBuilder.AppendLine("                     </ul> ");
                mapeditBuilder.AppendLine("                 </li> ");
                mapeditBuilder.AppendLine("                 <li> ");
                mapeditBuilder.AppendLine("                     <a id=\"content_menubar_manageOverlay\"></a> ");
                mapeditBuilder.AppendLine("                     <ul> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_overlayGetUserLocation\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_overlayEdit\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_overlayPlace\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_overlayToggle\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li> ");
                mapeditBuilder.AppendLine("                             <a id=\"content_menubar_header3Sub3Sub1\"></a> ");
                mapeditBuilder.AppendLine("                             <ul> ");
                mapeditBuilder.AppendLine("                                 <li><a id=\"content_menubar_rotationClockwise\"></a></li> ");
                mapeditBuilder.AppendLine("                                 <li><a id=\"content_menubar_rotationCounterClockwise\"></a></li> ");
                mapeditBuilder.AppendLine("                                 <li><a id=\"content_menubar_rotationReset\"></a></li> ");
                mapeditBuilder.AppendLine("                             </ul> ");
                mapeditBuilder.AppendLine("                         </li> ");
                mapeditBuilder.AppendLine("                         <li> ");
                mapeditBuilder.AppendLine("                             <a id=\"content_menubar_header3Sub3Sub2\"></a> ");
                mapeditBuilder.AppendLine("                             <ul> ");
                mapeditBuilder.AppendLine("                                 <li><a id=\"content_menubar_transparencyDarker\"></a></li> ");
                mapeditBuilder.AppendLine("                                 <li><a id=\"content_menubar_transparencyLighter\"></a></li> ");
                mapeditBuilder.AppendLine("                                 <li><a id=\"content_menubar_transparencyReset\"></a></li> ");
                mapeditBuilder.AppendLine("                             </ul> ");
                mapeditBuilder.AppendLine("                         </li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_overlayReset\" ></a></li> ");
                mapeditBuilder.AppendLine("                     </ul> ");
                mapeditBuilder.AppendLine("                 </li> ");
                mapeditBuilder.AppendLine("                 <li> ");
                mapeditBuilder.AppendLine("                     <a id=\"content_menubar_managePOI\"></a> ");
                mapeditBuilder.AppendLine("                     <ul> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_poiGetUserLocation\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_poiPlace\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_poiToggle\"></a></li> ");
                mapeditBuilder.AppendLine("                         <li> ");
                mapeditBuilder.AppendLine("                             <a id=\"content_menubar_header3Sub4Sub1\"></a> ");
                mapeditBuilder.AppendLine("                             <ul> ");
                mapeditBuilder.AppendLine("                                 <li><a id=\"content_menubar_poiMarker\"></a></li> ");
                mapeditBuilder.AppendLine("                                 <li><a id=\"content_menubar_poiCircle\"></a></li> ");
                mapeditBuilder.AppendLine("                                 <li><a id=\"content_menubar_poiRectangle\"></a></li> ");
                mapeditBuilder.AppendLine("                                 <li><a id=\"content_menubar_poiPolygon\"></a></li> ");
                mapeditBuilder.AppendLine("                                 <li><a id=\"content_menubar_poiLine\"></a></li> ");
                mapeditBuilder.AppendLine("                             </ul> ");
                mapeditBuilder.AppendLine("                         </li> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_poiReset\"></a></li> ");
                mapeditBuilder.AppendLine("                     </ul> ");
                mapeditBuilder.AppendLine("                 </li> ");
                mapeditBuilder.AppendLine("             </ul> ");
                mapeditBuilder.AppendLine("         </li> ");
                mapeditBuilder.AppendLine("         <li> ");
                mapeditBuilder.AppendLine("             <a id=\"content_menubar_header4\"></a> ");
                mapeditBuilder.AppendLine("             <ul> ");
                mapeditBuilder.AppendLine("                 <li><a id=\"content_menubar_documentation\"></a></li> ");
                mapeditBuilder.AppendLine("                 <li><a id=\"content_menubar_reportAProblem\"></a></li> ");
                mapeditBuilder.AppendLine("             </ul> ");
                mapeditBuilder.AppendLine("         </li> ");
                mapeditBuilder.AppendLine("     </ul>     ");
                mapeditBuilder.AppendLine(" </div> ");
                mapeditBuilder.AppendLine(" <div id=\"mapedit_container\"> ");
                mapeditBuilder.AppendLine("      ");
                mapeditBuilder.AppendLine("     <div id=\"mapedit_container_pane_1\"> ");
                mapeditBuilder.AppendLine("         <div id=\"mapedit_container_toolbar\">        ");
                mapeditBuilder.AppendLine("             <div class=\"toolbar_grouping\"> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_reset\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_toggleMapControls\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_toggleToolbox\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             </div> ");
                mapeditBuilder.AppendLine("             <div class=\"toolbar_grouping\"> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_layerRoadmap\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_layerTerrain\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_layerSatellite\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_layerHybrid\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_layerCustom\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_layerReset\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             </div> ");
                mapeditBuilder.AppendLine("             <div class=\"toolbar_grouping\"> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_panUp\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_panLeft\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_panReset\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_panRight\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_panDown\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             </div> ");
                mapeditBuilder.AppendLine("             <div class=\"toolbar_grouping\"> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_zoomIn\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_zoomReset\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_zoomOut\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             </div> ");
                mapeditBuilder.AppendLine("             <div class=\"toolbar_grouping\"> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_manageItem\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_manageOverlay\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_managePOI\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_button_manageSearch\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             </div> ");
                mapeditBuilder.AppendLine("             <div class=\"toolbar_grouping\"> ");
                mapeditBuilder.AppendLine("                 <div class=\"mapedit_container_search\"> ");
                mapeditBuilder.AppendLine("                     <input id=\"content_toolbar_searchField\" class=\"search\" type=\"text\" placeholder=\"\" onClick=\"this.select();\" /> ");
                mapeditBuilder.AppendLine("                     <div id=\"content_toolbar_searchButton\" class=\"searchActionHandle\"></div> ");
                mapeditBuilder.AppendLine("                 </div> ");
                mapeditBuilder.AppendLine("             </div> ");
                mapeditBuilder.AppendLine("         </div> ");
                mapeditBuilder.AppendLine("     </div> ");
                mapeditBuilder.AppendLine("  ");
                mapeditBuilder.AppendLine("     <div id=\"mapedit_container_toolbarGrabber\"> ");
                mapeditBuilder.AppendLine("         <div id=\"content_toolbarGrabber\"></div> ");
                mapeditBuilder.AppendLine("     </div>     ");
                mapeditBuilder.AppendLine("  ");
                mapeditBuilder.AppendLine("     <div id=\"mapedit_container_pane_2\"> ");
                mapeditBuilder.AppendLine("          ");
                mapeditBuilder.AppendLine("         <!--<div id=\"mapedit_container_message\"> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_message\"></div> ");
                mapeditBuilder.AppendLine("             </div>--> ");
                mapeditBuilder.AppendLine("          ");
                mapeditBuilder.AppendLine("         <div id=\"mapedit_container_toolbox\" class=\"ui-widget-content\"> ");
                mapeditBuilder.AppendLine("             <div id=\"mapedit_container_toolboxMinibar\"> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_minibar_icon\"></div>  ");
                mapeditBuilder.AppendLine("                 <div id=\"content_minibar_header\"></div>  ");
                mapeditBuilder.AppendLine("                 <div id=\"content_minibar_button_close\"></div>  ");
                mapeditBuilder.AppendLine("                 <div id=\"content_minibar_button_maximize\"></div>  ");
                mapeditBuilder.AppendLine("                 <div id=\"content_minibar_button_minimize\"></div>  ");
                mapeditBuilder.AppendLine("             </div> ");
                mapeditBuilder.AppendLine("             <div id=\"mapedit_container_toolboxTabs\"> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbox_tab1_header\" class=\"tab-title\"></div> ");
                mapeditBuilder.AppendLine("                 <div class=\"tab\"> ");
                mapeditBuilder.AppendLine("                     <div class=\"toolbox_tab-content\"> ");
                mapeditBuilder.AppendLine("                         <div id=\"mapedit_container_toolbox_tab1\"> ");
                mapeditBuilder.AppendLine("                             <div id=\"mapedit_container_grid\"> ");
                mapeditBuilder.AppendLine("                              ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_layerRoadmap\" class=\"x button\"></div> ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_layerTerrain\" class=\"x button\"></div> ");
                mapeditBuilder.AppendLine("                                 <div class=\"x half\"></div> ");
                mapeditBuilder.AppendLine("                                 <div class=\"x\"></div> ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_panUp\" class=\"x button\"></div> ");
                mapeditBuilder.AppendLine("                                 <div class=\"x\"></div> ");
                mapeditBuilder.AppendLine("  ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_layerSatellite\" class=\"x y button\"></div> ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_layerHybrid\" class=\"x button\"></div> ");
                mapeditBuilder.AppendLine("                                 <div class=\"x half\"></div> ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_panLeft\" class=\"x button\"></div> ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_panReset\" class=\"x button\"></div> ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_panRight\" class=\"x button\"></div> ");
                mapeditBuilder.AppendLine("                              ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_layerCustom\" class=\"x button\"></div> ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_layerReset\" class=\"x button\"></div> ");
                mapeditBuilder.AppendLine("                                 <div class=\"x half\"></div> ");
                mapeditBuilder.AppendLine("                                 <div class=\"x\"></div> ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_panDown\" class=\"x button\"></div> ");
                mapeditBuilder.AppendLine("                                 <div class=\"x\"></div> ");
                mapeditBuilder.AppendLine("  ");
                mapeditBuilder.AppendLine("                                 <div class=\"x y half\"></div> ");
                mapeditBuilder.AppendLine("  ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_reset\" class=\"x y button\"></div> ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_toggleMapControls\" class=\"x button\"></div> ");
                mapeditBuilder.AppendLine("                                 <div class=\"x half\"></div> ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_zoomIn\" class=\"x button\"></div> ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_zoomReset\" class=\"x button\"></div> ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_button_zoomOut\" class=\"x button\"></div> ");
                mapeditBuilder.AppendLine("                              ");
                mapeditBuilder.AppendLine("                             </div> ");
                mapeditBuilder.AppendLine("                         </div> ");
                mapeditBuilder.AppendLine("                     </div> ");
                mapeditBuilder.AppendLine("                 </div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbox_tab2_header\" class=\"tab-title\"></div> ");
                mapeditBuilder.AppendLine("                 <div class=\"tab\"> ");
                mapeditBuilder.AppendLine("                     <div class=\"toolbox_tab-content\"> ");
                mapeditBuilder.AppendLine("                         <div id=\"mapedit_container_toolbox_tab2\"> ");
                mapeditBuilder.AppendLine("                             <div id=\"content_toolbox_button_manageItem\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                             <div id=\"content_toolbox_button_manageOverlay\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                             <div id=\"content_toolbox_button_managePOI\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                             <div class=\"lineBreak\"></div> ");
                mapeditBuilder.AppendLine("                             <div class=\"mapedit_container_search\"> ");
                mapeditBuilder.AppendLine("                                 <input id=\"content_toolbox_searchField\" class=\"search\" type=\"text\" placeholder=\"\" onClick=\"this.select();\" /> ");
                mapeditBuilder.AppendLine("                                 <div id=\"content_toolbox_searchButton\" class=\"searchActionHandle\"></div> ");
                mapeditBuilder.AppendLine("                             </div> ");
                mapeditBuilder.AppendLine("                             <div class=\"lineBreak\"></div> ");
                mapeditBuilder.AppendLine("                             <div id=\"searchResults_container\"> ");
                mapeditBuilder.AppendLine("                                 <div id=\"searchResults_scoll_container\"> ");
                mapeditBuilder.AppendLine("                                     <div id=\"searchResults_list\"></div> ");
                mapeditBuilder.AppendLine("                                 </div> ");
                mapeditBuilder.AppendLine("                             </div>  ");
                mapeditBuilder.AppendLine("                         </div> ");
                mapeditBuilder.AppendLine("                     </div> ");
                mapeditBuilder.AppendLine("                 </div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbox_tab3_header\" class=\"tab-title\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"itemACL\" class=\"tab\"> ");
                mapeditBuilder.AppendLine("                     <div class=\"toolbox_tab-content\"> ");
                mapeditBuilder.AppendLine("                         <div id=\"mapedit_container_toolbox_tab3\"> ");
                mapeditBuilder.AppendLine("                             <div id=\"content_toolbox_button_itemPlace\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                             <div id=\"content_toolbox_button_itemGetUserLocation\" class=\"button\"></div>   ");
                mapeditBuilder.AppendLine("                             <div id=\"content_toolbox_button_useSearchAsLocation\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                             <div id=\"content_toolbox_button_convertToOverlay\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                             <div class=\"lineBreak\"></div> ");
                mapeditBuilder.AppendLine("                             <textarea id=\"content_toolbox_posItem\" class=\"tab-field\" rows=\"2\" cols=\"24\" placeholder=\"Selected Lat/Long\"></textarea> ");
                mapeditBuilder.AppendLine("                             <div class=\"lineBreak\"></div> ");
                mapeditBuilder.AppendLine("                             <textarea id=\"content_toolbox_rgItem\" class=\"tab-field\" rows=\"3\" cols=\"24\" placeholder=\"Nearest Address\"></textarea> ");
                mapeditBuilder.AppendLine("                             <div class=\"lineBreak\"></div> ");
                mapeditBuilder.AppendLine("                             <div class=\"button2\"> <input type=\"button\" id=\"content_toolbox_button_saveItem\" > </div> ");
                mapeditBuilder.AppendLine("                             <div class=\"button2\"> <input type=\"button\" id=\"content_toolbox_button_clearItem\" > </div> ");
                mapeditBuilder.AppendLine("                         </div> ");
                mapeditBuilder.AppendLine("                     </div> ");
                mapeditBuilder.AppendLine("                 </div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbox_tab4_header\" class=\"tab-title\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"overlayACL\" class=\"tab\"> ");
                mapeditBuilder.AppendLine("                     <div class=\"toolbox_tab-content\"> ");
                mapeditBuilder.AppendLine("                         <div id=\"content_toolbox_button_overlayEdit\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                         <div id=\"content_toolbox_button_overlayPlace\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                         <div id=\"content_toolbox_button_overlayGetUserLocation\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                         <div id=\"content_toolbox_button_overlayToggle\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("  ");
                mapeditBuilder.AppendLine("                         <div class=\"lineBreak\"></div> ");
                mapeditBuilder.AppendLine("                         <div id=\"mapedit_container_toolbox_overlayTools\"> ");
                mapeditBuilder.AppendLine("                             <div id=\"rotation\"> ");
                mapeditBuilder.AppendLine("                                 <div id=\"rotationKnob\"> ");
                mapeditBuilder.AppendLine("                                     <input class=\"knob\" data-displayInput=\"false\" data-width=\"68\" data-step=\"1\" data-min=\"0\" data-max=\"360\" data-cursor=true data-bgColor=\"#B2B2B2\" data-fgColor=\"#111111\" data-thickness=\"0.3\" value=\"0\"> ");
                mapeditBuilder.AppendLine("                                 </div> ");
                mapeditBuilder.AppendLine("                                 <div id=\"mapedit_container_toolbox_rotationButtons\"> ");
                mapeditBuilder.AppendLine("                                     <div id=\"content_toolbox_rotationCounterClockwise\" class=\"button3\"></div> ");
                mapeditBuilder.AppendLine("                                     <div id=\"content_toolbox_rotationReset\" class=\"button3\"></div> ");
                mapeditBuilder.AppendLine("                                     <div id=\"content_toolbox_rotationClockwise\" class=\"button3\"></div> ");
                mapeditBuilder.AppendLine("                                 </div> ");
                mapeditBuilder.AppendLine("                             </div> ");
                mapeditBuilder.AppendLine("                             <div id=\"transparency\"> ");
                mapeditBuilder.AppendLine("                                 <div id=\"overlayTransparencySlider\"></div> ");
                mapeditBuilder.AppendLine("                             </div> ");
                mapeditBuilder.AppendLine("                         </div> ");
                mapeditBuilder.AppendLine("                         <div class=\"lineBreak\"></div> ");
                mapeditBuilder.AppendLine("                         <div id=\"overlayList_container\"> ");
                mapeditBuilder.AppendLine("                             <div id=\"overlayList_scoll_container\"> ");
                mapeditBuilder.AppendLine("                                 <div id=\"overlayList\"></div> ");
                mapeditBuilder.AppendLine("                             </div> ");
                mapeditBuilder.AppendLine("                         </div>   ");
                mapeditBuilder.AppendLine("                         <div class=\"lineBreak\"></div> ");
                mapeditBuilder.AppendLine("                         <div class=\"button2\"> <input type=\"button\" id=\"content_toolbox_button_saveOverlay\" > </div> ");
                mapeditBuilder.AppendLine("                         <div class=\"button2\"> <input type=\"button\" id=\"content_toolbox_button_clearOverlay\" > </div> ");
                mapeditBuilder.AppendLine("                     </div> ");
                mapeditBuilder.AppendLine("                 </div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbox_tab5_header\" class=\"tab-title\"></div> ");
                mapeditBuilder.AppendLine("                 <div class=\"tab\"> ");
                mapeditBuilder.AppendLine("                     <div class=\"toolbox_tab-content\"> ");
                mapeditBuilder.AppendLine("                         <div id=\"content_toolbox_button_placePOI\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                         <div id=\"content_toolbox_button_poiGetUserLocation\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                         <div id=\"content_toolbox_button_poiToggle\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                         <div class=\"lineBreak\"></div> ");
                mapeditBuilder.AppendLine("                         <div id=\"content_toolbox_button_poiMarker\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                         <div id=\"content_toolbox_button_poiCircle\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                         <div id=\"content_toolbox_button_poiRectangle\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                         <div id=\"content_toolbox_button_poiPolygon\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                         <div id=\"content_toolbox_button_poiLine\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                         <div class=\"lineBreak\"></div> ");
                mapeditBuilder.AppendLine("                         <div id=\"poiList_container\"> ");
                mapeditBuilder.AppendLine("                             <div id=\"poiList_scoll_container\"> ");
                mapeditBuilder.AppendLine("                                 <div id=\"poiList\"></div> ");
                mapeditBuilder.AppendLine("                             </div> ");
                mapeditBuilder.AppendLine("                         </div>   ");
                mapeditBuilder.AppendLine("                         <div class=\"lineBreak\"></div> ");
                mapeditBuilder.AppendLine("                         <div class=\"button2\"> <input type=\"button\" id=\"content_toolbox_button_savePOI\" > </div> ");
                mapeditBuilder.AppendLine("                         <div class=\"button2\"> <input type=\"button\" id=\"content_toolbox_button_clearPOI\" > </div> ");
                mapeditBuilder.AppendLine("                     </div> ");
                mapeditBuilder.AppendLine("                 </div> ");
                mapeditBuilder.AppendLine("             </div> ");
                mapeditBuilder.AppendLine("         </div>     ");
                mapeditBuilder.AppendLine("         <div id=\"googleMap\"></div> ");
                mapeditBuilder.AppendLine("     </div> ");
                mapeditBuilder.AppendLine(" </div> ");
                mapeditBuilder.AppendLine(" <div id=\"debugs\"></div> ");


                #endregion

            }

            //custom js files (load order does matter)
            mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/sobekcm_mapedit.js\"></script>");
            //mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/sobekcm_mapedit_declarations.js\"></script>");
            //mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/sobekcm_mapedit_localization.js\"></script>");
            //mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/sobekcm_mapedit_listeners.js\"></script>");
            //mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/sobekcm_mapedit_listener_actions.js\"></script>");
            //mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/sobekcm_mapedit_gmap.js\"></script>");
            //mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/sobekcm_mapedit_utilities.js\"></script>");
            //mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/sobekcm_mapedit_other.js\"></script>");
            mapeditBuilder.AppendLine("<script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/gmaps-markerwithlabel-1.8.1.min.js\"></script>"); //must load after custom

            //end of custom content
            mapeditBuilder.AppendLine("</td>");

            // Add the literal to the placeholder
            Literal placeHolderText = new Literal();
            placeHolderText.Text = mapeditBuilder.ToString();
            placeHolder.Controls.Add(placeHolderText);

        }
    }
}
