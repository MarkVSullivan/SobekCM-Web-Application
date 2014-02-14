using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml;
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
        private static User_Object CurrentUser;
        private static SobekCM_Item CurrentItem;
        private static GeoSpatial_Information geoInfo2;
        List<Coordinate_Polygon> allPolygons;
        List<Coordinate_Point> allPoints;
        List<Coordinate_Line> allLines;
        List<Coordinate_Circle> allCircles;
        
        /// <summary> init viewer instance </summary>
        public Google_Coordinate_Entry_ItemViewer(User_Object Current_User, SobekCM_Item Current_Item, SobekCM_Navigation_Object Current_Mode)
        {
            try
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

            //holds actions from page
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
            catch (Exception)
            {
                //Custom_Tracer.Add_Trace("MapEdit Start Failure");
                throw new ApplicationException("MapEdit Start Failure");
                throw;
            }
        }

        /// <summary> parse and save incoming message  </summary>
        /// <param name="sendData"> message from page </param>
        public static void SaveContent(String sendData)
        {
            try
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

            //get the pages
            List<abstract_TreeNode> pages = CurrentItem.Divisions.Physical_Tree.Pages_PreOrder;

            //create a new list of all the polygons for a resource item
            Dictionary<string, Page_TreeNode> pageLookup = new Dictionary<string, Page_TreeNode>();
            int page_index = 1;
            foreach (Page_TreeNode pageNode in pages)
            {
                if (pageNode.Label.Length == 0)
                    pageLookup["Page " + page_index] = pageNode;
                else
                    pageLookup[pageNode.Label] = pageNode;
                page_index++;
            }

            //get the length of incoming message
            int index1 = sendData.LastIndexOf("~", StringComparison.Ordinal);

            //split into each save message
            string[] allSaves = sendData.Substring(0, index1).Split('~');

            //hold save type handle
            string saveTypeHandle = null;
            //go through each item to save and check for ovelrays and item only not pois (ORDER does matter because these will be saved to db before pois are saved)
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
                        #region item
                        case "item":
                            //prep incoming lat/long
                            string[] temp1 = ar[2].Split(',');
                            double temp1Lat = Convert.ToDouble(temp1[0].Replace("(", ""));
                            double temp1Long = Convert.ToDouble(temp1[1].Replace(")", ""));
                            ////clear specific geo obj
                            //resourceGeoInfo.Clear_Specific(Convert.ToString(ar[3]));
                            //clear all the previous mains featureTypes (this will work for an item because there is only ever one item)
                            resourceGeoInfo.Clear_NonPOIs();
                            //add the point obj
                            Coordinate_Point newPoint = new Coordinate_Point(temp1Lat, temp1Long, CurrentItem.METS_Header.ObjectID, "main");
                            //add the new point 
                            resourceGeoInfo.Add_Point(newPoint);
                            //save to db
                            Resource_Object.Database.SobekCM_Database.Save_Digital_Resource(CurrentItem);
                            break;
                        #endregion
                        #region overlay
                        case "overlay":
                            //parse the array id of the page
                            int arrayId = (Convert.ToInt32(ar[2]) - 1); //is this always true (minus one of the human page id)?
                            //add the label to page obj
                            pages[arrayId].Label = ar[3];
                            //get the geocoordinate object for that pageId
                            GeoSpatial_Information pageGeo = pages[arrayId].Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                            //if there isnt any already there
                            if (pageGeo == null)
                            {
                                //create new
                                pageGeo = new GeoSpatial_Information();
                                //create a polygon
                                Coordinate_Polygon pagePolygon = new Coordinate_Polygon();
                                //prep incoming bounds
                                string[] temp2 = ar[4].Split(',');
                                pagePolygon.Clear_Edge_Points();
                                pagePolygon.Add_Edge_Point(Convert.ToDouble(temp2[0].Replace("(", "")), Convert.ToDouble(temp2[1].Replace(")", "")));
                                pagePolygon.Add_Edge_Point(Convert.ToDouble(temp2[2].Replace("(", "")), Convert.ToDouble(temp2[3].Replace(")", "")));
                                pagePolygon.Recalculate_Bounding_Box();
                                //add the rotation
                                double result;
                                pagePolygon.Rotation = Double.TryParse(ar[6], out result) ? result : 0;
                                //add the featureType (explicitly add to make sure it is there)
                                pagePolygon.FeatureType = "main";
                                //add the label
                                pagePolygon.Label = ar[3];
                                //add the polygon type
                                pagePolygon.PolygonType = "rectangle";
                                //add polygon to pagegeo
                                pageGeo.Add_Polygon(pagePolygon);
                            }
                            else
                            {
                                try
                                {
                                    //get current polygon info
                                    Coordinate_Polygon pagePolygon = pageGeo.Polygons[0];
                                    //prep incoming bounds
                                    string[] temp2 = ar[4].Split(',');
                                    pagePolygon.Clear_Edge_Points();
                                    pagePolygon.Add_Edge_Point(Convert.ToDouble(temp2[0].Replace("(", "")), Convert.ToDouble(temp2[1].Replace(")", "")));
                                    pagePolygon.Add_Edge_Point(Convert.ToDouble(temp2[2].Replace("(", "")), Convert.ToDouble(temp2[3].Replace(")", "")));
                                    pagePolygon.Recalculate_Bounding_Box();
                                    //add the rotation
                                    double result;
                                    pagePolygon.Rotation = Double.TryParse(ar[6], out result) ? result : 0;
                                    //add the featureType (explicitly add to make sure it is there)
                                    pagePolygon.FeatureType = "main";
                                    //add the label
                                    pagePolygon.Label = ar[3];
                                    //add the polygon type
                                    pagePolygon.PolygonType = "rectangle";
                                    //clear all previous nonPOIs for this page (NOTE: this will only work if there is only one main page item)
                                    pageGeo.Clear_NonPOIs();
                                    //add polygon to pagegeo
                                    pageGeo.Add_Polygon(pagePolygon);
                                }
                                catch (Exception)
                                {
                                    //there were no polygons
                                    try
                                    {
                                        //make a polygon
                                        Coordinate_Polygon pagePolygon = new Coordinate_Polygon();
                                        //prep incoming bounds
                                        string[] temp2 = ar[4].Split(',');
                                        pagePolygon.Clear_Edge_Points();
                                        pagePolygon.Add_Edge_Point(Convert.ToDouble(temp2[0].Replace("(", "")), Convert.ToDouble(temp2[1].Replace(")", "")));
                                        pagePolygon.Add_Edge_Point(Convert.ToDouble(temp2[2].Replace("(", "")), Convert.ToDouble(temp2[3].Replace(")", "")));
                                        pagePolygon.Recalculate_Bounding_Box();
                                        //add the rotation
                                        double result;
                                        pagePolygon.Rotation = Double.TryParse(ar[6], out result) ? result : 0;
                                        //add the featureType (explicitly add to make sure it is there)
                                        pagePolygon.FeatureType = "main";
                                        //add the label
                                        pagePolygon.Label = ar[3];
                                        //add the polygon type
                                        pagePolygon.PolygonType = "rectangle";
                                        //clear all previous nonPOIs for this page (NOTE: this will only work if there is only one main page item)
                                        pageGeo.Clear_NonPOIs();
                                        //add polygon to pagegeo
                                        pageGeo.Add_Polygon(pagePolygon);
                                    }
                                    catch (Exception)
                                    {
                                        //welp...
                                    }
                                }
                            }
                            //add the pagegeo obj
                            pages[arrayId].Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, pageGeo);
                            //save to db
                            Resource_Object.Database.SobekCM_Database.Save_Digital_Resource(CurrentItem);
                            break;
                        #endregion
                    }
                }
                else
                {
                    if (saveTypeHandle == "delete")
                    {
                        switch (saveType)
                        {
                            #region item
                            case "item":
                                //clear nonpoipoints
                                resourceGeoInfo.Clear_NonPOIPoints();
                                //save to db
                                Resource_Object.Database.SobekCM_Database.Save_Digital_Resource(CurrentItem);
                                break;
                            #endregion
                            #region overlay
                            case "overlay":
                                try
                                {
                                    //parse the array id of the page
                                    int arrayId = (Convert.ToInt32(ar[2]) - 1); //is this always true (minus one of the human page id)?
                                    //get the geocoordinate object for that pageId
                                    GeoSpatial_Information pageGeo = pages[arrayId].Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                                    Coordinate_Polygon pagePolygon = pageGeo.Polygons[0];

                                    //reset edgepoints
                                    pagePolygon.Clear_Edge_Points();
                                    //reset rotation
                                    pagePolygon.Rotation = 0;
                                    //add the featureType (explicitly add to make sure it is there)
                                    pagePolygon.FeatureType = "hidden";
                                    //add the polygon type
                                    pagePolygon.PolygonType = "hidden";
                                    //clear all previous nonPOIs for this page (NOTE: this will only work if there is only one main page item)
                                    pageGeo.Clear_NonPOIs();
                                    //add polygon to pagegeo
                                    pageGeo.Add_Polygon(pagePolygon);

                                    ////if there isnt any already there
                                    //if (pageGeo != null)
                                    //    pageGeo.Remove_Polygon(pageGeo.Polygons[0]);

                                    //add the pagegeo obj
                                    pages[arrayId].Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, pageGeo);

                                    //save to db
                                    Resource_Object.Database.SobekCM_Database.Save_Digital_Resource(CurrentItem);
                                }
                                catch (Exception)
                                {
                                    //
                                }
                                
                                break;
                            #endregion
                        }
                    }
                }
            }

            //check to see if save poi clear has already been fired...
            bool firedOnce = true; 
            //go through each item to save and check for pois only
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
                        #region poi
                        case "poi":
                            //fixes bug
                            if (firedOnce)
                            {
                                //clear previous poi points
                                resourceGeoInfo.Clear_POIs();
                                firedOnce = false;
                            }
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
                                    Coordinate_Circle poiCircle = new Coordinate_Circle();
                                    //set the label
                                    poiCircle.Label = ar[3];
                                    //set the radius
                                    poiCircle.Radius = Convert.ToDouble(ar[5]);
                                    //add the feature type
                                    poiCircle.FeatureType = "poi";
                                    //add the incoming lat/long
                                    string[] temp3 = ar[4].Split(',');
                                    poiCircle.Latitude = Convert.ToDouble(temp3[0].Replace("(", ""));
                                    poiCircle.Longitude = Convert.ToDouble(temp3[1].Replace(")", ""));
                                    //add to the resource obj
                                    resourceGeoInfo.Add_Circle(poiCircle);
                                    break;
                                case "rectangle":
                                    //create new polygon
                                    Coordinate_Polygon poiRectangle = new Coordinate_Polygon();
                                    //add the label
                                    poiRectangle.Label = ar[3];
                                    //add the feature type
                                    poiRectangle.FeatureType = "poi";
                                    //add the polygon type
                                    poiRectangle.PolygonType = "rectangle";
                                    //add the incoming bounds
                                    string[] temp4 = ar[4].Split(',');
                                    poiRectangle.Add_Edge_Point(Convert.ToDouble(temp4[0].Replace("(", "")), Convert.ToDouble(temp4[1].Replace(")", "")));
                                    poiRectangle.Add_Edge_Point(Convert.ToDouble(temp4[2].Replace("(", "")), Convert.ToDouble(temp4[3].Replace(")", "")));
                                    poiRectangle.Recalculate_Bounding_Box();
                                    //add to resource obj
                                    resourceGeoInfo.Add_Polygon(poiRectangle);
                                    break;
                                case "polygon":
                                    //create new polygon
                                    Coordinate_Polygon poiPolygon = new Coordinate_Polygon();
                                    //add the label
                                    poiPolygon.Label = ar[3];
                                    //add the feature type
                                    poiPolygon.FeatureType = "poi";
                                    //add the edge points
                                    for (int i2 = 5; i2 < ar.Length; i2++)
                                    {
                                        string[] temp5 = ar[i2].Split(',');
                                        poiPolygon.Add_Edge_Point(Convert.ToDouble(temp5[0].Replace("(", "")), Convert.ToDouble(temp5[1].Replace(")", "")));
                                    }
                                    //add the polygon
                                    resourceGeoInfo.Add_Polygon(poiPolygon);
                                    break;
                                case "polyline":
                                    //create new line
                                    Coordinate_Line poiLine = new Coordinate_Line();
                                    //add the label
                                    poiLine.Label = ar[3];
                                    //add the feature type
                                    poiLine.FeatureType = "poi";
                                    //add the edge points
                                    for (int i2 = 5; i2 < ar.Length; i2++)
                                    {
                                        string[] temp5 = ar[i2].Split(',');
                                        poiLine.Add_Point(Convert.ToDouble(temp5[0].Replace("(", "")), Convert.ToDouble(temp5[1].Replace(")", "")), "");
                                    }
                                    //add the line
                                    resourceGeoInfo.Add_Line(poiLine);
                                    break;
                            }
                            break;
                        #endregion
                    }
                }
                else
                {
                    //skip to the apply
                }
            }

            #region prep saving dir
            //create inprocessing directory
            string userInProcessDirectory = CurrentUser.User_InProcess_Directory("mapwork");
            string backupDirectory = SobekCM_Library_Settings.Image_Server_Network + CurrentItem.Web.AssocFilePath + SobekCM_Library_Settings.BACKUP_FILES_FOLDER_NAME;

            //ensure the user's process directory exists
            if (!Directory.Exists(userInProcessDirectory))
                Directory.CreateDirectory(userInProcessDirectory);
            //ensure the backup directory exists
            if (!Directory.Exists(backupDirectory))
                Directory.CreateDirectory(backupDirectory);

            string resource_directory = SobekCM_Library_Settings.Image_Server_Network + CurrentItem.Web.AssocFilePath;
            string current_mets = resource_directory + CurrentItem.METS_Header.ObjectID + ".mets.xml";
            string backup_mets = backupDirectory + "\\" + CurrentItem.METS_Header.ObjectID + "_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".mets.xml.BAK";
            string metsInProcessFile = userInProcessDirectory + "\\" + CurrentItem.BibID + "_" + CurrentItem.VID + ".mets.xml";
            #endregion

            #region Save mets and db
            //save the item to the temporary location
            CurrentItem.Save_METS(userInProcessDirectory + "\\" + CurrentItem.BibID + "_" + CurrentItem.VID + ".mets.xml");
            //move temp mets to prod
            File.Copy(metsInProcessFile, current_mets, true);
            //delete in process mets file 
            File.Delete(metsInProcessFile);
            //create a backup mets file
            File.Copy(current_mets, backup_mets, true);
            #endregion

            }
            catch (Exception)
            {
                //Custom_Tracer.Add_Trace("MapEdit Save Error");
                throw new ApplicationException("MapEdit Save Error");
                //throw;
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
            Body_Attributes.Add(new Tuple<string, string>("onresize", "MAPEDITOR.UTILITIES.resizeView();"));
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
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Left_Navigation_Bar,
						HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Full_JQuery_UI
                    };
            }
        }

        /// <summary> Abstract method adds the main view section to the page turner </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            try
            {

            // Start to build the response
            StringBuilder mapeditBuilder = new StringBuilder();

            //page content
            mapeditBuilder.AppendLine("<td>");

            mapeditBuilder.AppendLine(" <input type=\"hidden\" id=\"action\" name=\"action\" value=\"\" /> ");
            mapeditBuilder.AppendLine(" <input type=\"hidden\" id=\"payload\" name=\"payload\" value=\"\" /> ");
            mapeditBuilder.AppendLine("  ");

            //loading blanket
            mapeditBuilder.AppendLine("  ");
            mapeditBuilder.AppendLine(" <div id=\"mapedit_blanket_loading\"><div>Loading...<br/><br/><img src=\"" + CurrentMode.Base_URL + "default/images/mapedit/ajax-loader.gif\"></div></div> ");
            mapeditBuilder.AppendLine("  ");

            //standard css
            mapeditBuilder.AppendLine(" <link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/jquery-ui.css\"/> ");
            mapeditBuilder.AppendLine(" <link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/jquery-searchbox.css\"/> ");

            //custom css
            mapeditBuilder.AppendLine(" <link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_Mapedit.css\"/> ");
            mapeditBuilder.AppendLine(" <link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_Mapedit_Theme_Default.css\"/> ");
            
            //standard js files
			mapeditBuilder.AppendLine(" <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.3.custom.min.js\"></script> ");
            mapeditBuilder.AppendLine(" <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-migrate-1.1.1.min.js\"></script> ");
            mapeditBuilder.AppendLine(" <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-rotate.js\"></script> ");
            mapeditBuilder.AppendLine(" <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-knob.js\"></script> ");
            mapeditBuilder.AppendLine(" <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-json-2.4.min.js\"></script> ");
            mapeditBuilder.AppendLine(" <script type=\"text/javascript\" src=\"https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&key=AIzaSyCzliz5FjUlEI9D2605b33-etBrENSSBZM&libraries=drawing\"></script> ");
            mapeditBuilder.AppendLine(" <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/gmaps-infobox.js\"></script> ");

            //custom
            #region

            mapeditBuilder.AppendLine(" ");
            mapeditBuilder.AppendLine(" <script type=\"text/javascript\"> ");
            mapeditBuilder.AppendLine(" ");
            
            //create build time
            string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            System.IO.Stream s = null;
            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            catch (Exception)
            {
                Tracer.Add_Trace("Could Not Create Build Time");
                throw;
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }
            int i2 = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = System.BitConverter.ToInt32(b, i2 + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
            string debugTime_buildTimestamp = dt.ToString();
            //get current timestamp
            TimeSpan span = (dt - new DateTime(1970, 1, 1, 0, 0, 0, 0));
            double debugTime_unixTimestamp = span.TotalSeconds;
            
            //setup server to client vars writer
            mapeditBuilder.AppendLine(" // Add Server Vars ");
            mapeditBuilder.AppendLine(" function initServerToClientVars(){ ");
            mapeditBuilder.AppendLine("  try{ ");
            mapeditBuilder.AppendLine("   //MAPEDITOR.TRACER.addTracer(\"[INFO]: initServerToClientVars started...\"); ");
            mapeditBuilder.AppendLine("   MAPEDITOR.GLOBAL.DEFINES.baseURL = \"" + CurrentMode.Base_URL + "\"; //add baseURL ");
            mapeditBuilder.AppendLine("   MAPEDITOR.GLOBAL.DEFINES.debugUnixTimeStamp = " + debugTime_unixTimestamp + "; //add debugTime ");
            mapeditBuilder.AppendLine("   MAPEDITOR.GLOBAL.DEFINES.debugBuildTimeStamp = \"" + debugTime_buildTimestamp + "\"; //add debugTimestamp ");
            //detemrine if debugging
            if (CurrentMode.Base_URL.Contains("localhost"))
                mapeditBuilder.AppendLine("   MAPEDITOR.GLOBAL.DEFINES.debuggerOn = true; //debugger flag ");
            else
                mapeditBuilder.AppendLine("   MAPEDITOR.GLOBAL.DEFINES.debuggerOn = false; //debugger flag ");

            mapeditBuilder.AppendLine("   //MAPEDITOR.TRACER.addTracer(\"[INFO]: initServerToClientVars completed...\"); ");
            mapeditBuilder.AppendLine("  }catch (err){ ");
            mapeditBuilder.AppendLine("  //MAPEDITOR.TRACER.addTracer(\"[ERROR]: initServerToClientVars failed...\"); ");
            mapeditBuilder.AppendLine("  } ");
            mapeditBuilder.AppendLine(" } ");
            mapeditBuilder.AppendLine(" ");

            //config settings writer section 
            mapeditBuilder.AppendLine(" // Add Config Settings Objects ");
            mapeditBuilder.AppendLine(" function initConfigSettings(){ ");
            mapeditBuilder.AppendLine("  try{ ");
            mapeditBuilder.AppendLine("    //MAPEDITOR.TRACER.addTracer(\"[INFO]: initConfigSettings started...\"); ");

            //add collection load type and attributes
            #region

            //init collection load type
            String collectionLoadType;
            //init collectionLoadParams
            List<string> collectionLoadParams = new List<string>();
            //init collectionIdsFromConfig
            List<string> collectionIdsFromConfig = new List<string>();
            //read collectionIds from config.xml file
            try
            {
                string configFilePath = SobekCM_Library_Settings.Application_Server_Network + "/config/sobekcm_mapedit.config";
#if DEBUG
                configFilePath = @"C:\Users\cadetpeters89\Documents\CUSTOM\projects\git\SobekCM-Web-Application\SobekCM\config\sobekcm_mapedit.config"; //2do make this dynamic
#endif
                //read through and get all the ids
                using (XmlReader reader = XmlReader.Create(configFilePath))
                {
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            // Get element name and switch on it.
                            switch (reader.Name)
                            {
                                case "collection":
                                    collectionIdsFromConfig.Add(reader.GetAttribute("id"));
                                    break;
                            }
                        }
                    }
                }

                //go through each collectionId in mapEditConfig.xml
                foreach (string collectionId in collectionIdsFromConfig)
                {
                    //compare collectionID to all collectionIDs 
                    if ((CurrentItem.Behaviors.Aggregations[0].Code == collectionId) || (CurrentItem.BibID == collectionId) || (CurrentItem.BibID.Contains(collectionId)))
                    {
                        //if found, assign "readFromXML" as first param
                        collectionLoadParams.Add("readFromXML");
                        //go through each  xml param and assign
                        using (XmlReader reader = XmlReader.Create(configFilePath))
                        {
                            while (reader.Read())
                            {
                                // Only detect start elements.
                                if (reader.IsStartElement())
                                {
                                    // Get element name and switch on it.
                                    switch (reader.Name)
                                    {
                                        case "collection":
                                            //verify that this is the right collectionId
                                            if (reader[0] == collectionId)
                                            {
                                                //start with the second attribute and add them so long as there is one to add
                                                for (int i = 1; i < reader.AttributeCount; i++)
                                                {
                                                    //move to the attribute
                                                    reader.MoveToAttribute(i);
                                                    //store param
                                                    collectionLoadParams.Add(reader.Value); //not currently used
                                                    //add param to page
                                                    mapeditBuilder.AppendLine("   MAPEDITOR.GLOBAL.DEFINES." + reader.Name + " = " + reader.Value + "; //adding "+reader.Name); //could not determine how to get attribute's name
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                //add collection params to js var
                //mapeditBuilder.AppendLine("   MAPEDITOR.GLOBAL.DEFINES.collectionLoadParams = \"" + collectionLoadParams + "\"; //add collectionLoadParams ");
            }
            catch (Exception)
            {
                Tracer.Add_Trace("Could Not Load MapEdit Configs");
                throw;
            }
            //determine if there is a custom collection to load
            if (collectionLoadParams.Count > 1)
                collectionLoadType = collectionLoadParams[0];
            else
                collectionLoadType = "default";
            //add collection load type to page
            mapeditBuilder.AppendLine("   MAPEDITOR.GLOBAL.DEFINES.collectionLoadType = \"" + collectionLoadType + "\"; //add collectionLoadType ");

            #endregion

            mapeditBuilder.AppendLine("    //MAPEDITOR.TRACER.addTracer(\"[INFO]: initConfigSettings completed...\"); ");
            mapeditBuilder.AppendLine("  }catch (err){ ");
            mapeditBuilder.AppendLine("  //MAPEDITOR.TRACER.addTracer(\"[ERROR]: initConfigSettings failed...\"); ");
            mapeditBuilder.AppendLine("  } ");
            mapeditBuilder.AppendLine(" } ");
            mapeditBuilder.AppendLine(" ");


            //geo objects writer section 
            mapeditBuilder.AppendLine(" // Add Geo Objects ");
            mapeditBuilder.AppendLine(" function initGeoObjects(){ ");
            mapeditBuilder.AppendLine("  try{ ");
            mapeditBuilder.AppendLine("    //MAPEDITOR.TRACER.addTracer(\"[INFO]: initGeoObjects started...\"); ");
            mapeditBuilder.AppendLine(" ");

            // Add the geo info
            if (CurrentItem != null)
            {

                allPolygons = new List<Coordinate_Polygon>();
                allPoints = new List<Coordinate_Point>();
                allLines = new List<Coordinate_Line>();
                allCircles = new List<Coordinate_Circle>();

                // Collect all the polygons, points, and lines
                #region
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
                    if (geoInfo.Circle_Count > 0)
                    {
                        foreach (Coordinate_Circle thisCircle in geoInfo.Circles)
                            allCircles.Add(thisCircle);
                    }
                }
                #endregion

                // Collect all the pages and their data
                #region
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
                                thisPolygon.Page_Sequence = (ushort) (i + 1);
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
                        if (geoInfo2.Circle_Count > 0)
                        {
                            foreach (Coordinate_Circle thisCircle in geoInfo2.Circles)
                            {
                                allCircles.Add(thisCircle);
                            }
                        }
                    }
                }
                #endregion

                // Add all the points to the page
                #region
                if (allPoints.Count > 0)
                {
                    //add each point
                    for (int point = 0; point < allPoints.Count; point++)
                    {
                        //add the featureType
                        mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPointFeatureType[" + point + "] = \"" + allPoints[point].FeatureType + "\";");
                        //add the label
                        if (!String.IsNullOrEmpty(allPoints[point].Label))
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPointLabel[" + point + "] = \"" + Convert_String_To_XML_Safe(allPoints[point].Label) + "\"; ");
                        else
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPointLabel[" + point + "] = \"" + Convert_String_To_XML_Safe(CurrentItem.Bib_Title) + "\"; ");
                        //add the center point
                        mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPointCenter[" + point + "] = new google.maps.LatLng(" + allPoints[point].Latitude + "," + allPoints[point].Longitude + "); ");
                        //add the image url (if not a poi)
                        if (allPoints[point].FeatureType != "poi")
                        {
                            try
                            {
                                //get image url myway
                                string current_image_file = CurrentItem.Web.Source_URL + "/" + CurrentItem.VID + ".jpg";
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPointSourceURL[" + point + "] = \"" + current_image_file + "\"; ");
                            }
                            catch (Exception)
                            {
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPointSourceURL[" + point + "] = \"\"; ");
                            }
                        }
                    }
                    mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.displayIncomingPoints();");
                    mapeditBuilder.AppendLine(" ");
                }
                #endregion

                // Add all the circles to page
                #region
                if (allCircles.Count > 0)
                {
                    //add each circle
                    for (int circle = 0; circle < allCircles.Count; circle++)
                    {
                        //add the featuretype
                        mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingCircleFeatureType[" + circle + "] = \"" + allCircles[circle].FeatureType + "\";");
                        //add the label
                        if (!String.IsNullOrEmpty(allCircles[circle].Label))
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingCircleLabel[" + circle + "] = \"" + Convert_String_To_XML_Safe(allCircles[circle].Label) + "\"; ");
                        else
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingCircleLabel[" + circle + "] = \"" + Convert_String_To_XML_Safe(CurrentItem.Bib_Title) + "\"; ");
                        //add the center point
                        mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingCircleCenter[" + circle + "] = new google.maps.LatLng(" + allCircles[circle].Latitude + "," + allCircles[circle].Longitude + "); ");
                        //add the radius
                        mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingCircleRadius[" + circle + "] = " + allCircles[circle].Radius + "; ");
                    }
                    mapeditBuilder.AppendLine(" ");
                    mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.displayIncomingCircles();");
                    mapeditBuilder.AppendLine(" ");
                }
                #endregion

                //Add all the Lines to page
                #region
                if (allLines.Count > 0)
                {
                    //add each line
                    for (int line = 0; line < allLines.Count; line++)
                    {
                        //add the featuretype
                        mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingLineFeatureType[" + line + "] = \"" + allLines[line].FeatureType + "\";");
                        //add the label
                        if (!String.IsNullOrEmpty(allLines[line].Label))
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingLineLabel[" + line + "] = \"" + Convert_String_To_XML_Safe(allLines[line].Label) + "\"; ");
                        else
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingLineLabel[" + line + "] = \"" + Convert_String_To_XML_Safe(CurrentItem.Bib_Title) + "\"; ");
                        //add the Line path
                        mapeditBuilder.Append("      MAPEDITOR.GLOBAL.DEFINES.incomingLinePath[" + line + "] = [ ");
                        int linePointCurrentCount = 0;
                        foreach (Coordinate_Point thisPoint in allLines[line].Points)
                        {
                            linePointCurrentCount++;
                            //determine if this is the last edge point (fixes the js issue where the trailing , could cause older browsers to crash)
                            if (linePointCurrentCount == allLines[line].Point_Count)
                                mapeditBuilder.Append("new google.maps.LatLng(" + thisPoint.Latitude + "," + thisPoint.Longitude + ") ");
                            else
                                mapeditBuilder.Append("new google.maps.LatLng(" + thisPoint.Latitude + "," + thisPoint.Longitude + "), ");
                        }
                        mapeditBuilder.AppendLine("];");

                    }
                    mapeditBuilder.AppendLine(" ");
                    mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.displayIncomingLines();");
                    mapeditBuilder.AppendLine(" ");
                }
                #endregion

                // Add all the polygons to page
                #region
                //iteraters 
                int totalAddedPolygonIndex = 0; //this holds the index counter or code/array counter (IE starts with 0)
                int totalAddedPolygonCount = 0; //this holds the real (human) count not the index (IE starts at 1)
                //go through and add the existing polygons
                if ((allPolygons.Count > 0) && (allPolygons[0].Edge_Points_Count > 1))
                {

                    #region Add overlays first! this fixes index issues wtotalAddedPolygonIndexhin (thus index is always id minus 1)
                    foreach (Coordinate_Polygon itemPolygon in allPolygons)
                    {
                        //do this so long as it is not a poi
                        if (itemPolygon.FeatureType != "poi")
                        {
                            //add the featureType
                            if (itemPolygon.FeatureType == "")
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[" + totalAddedPolygonIndex + "] = \"main\";");
                            else
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[" + totalAddedPolygonIndex + "] = \"" + itemPolygon.FeatureType + "\";");

                            //add the polygonType
                            if (itemPolygon.PolygonType == "")
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPolygonType[" + totalAddedPolygonIndex + "] = \"rectangle\";");
                            else
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPolygonType[" + totalAddedPolygonIndex + "] = \"" + itemPolygon.PolygonType + "\";");
                            
                            //add the label
                            if (Convert_String_To_XML_Safe(itemPolygon.Label) != "")
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[" + totalAddedPolygonIndex + "] = \"" + Convert_String_To_XML_Safe(itemPolygon.Label) + "\";");
                            else
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[" + totalAddedPolygonIndex + "] = \"Page " + (totalAddedPolygonIndex + 1) + "\";"); //2do localize this text???
                            
                            //create the bounds string
                            string bounds = "new google.maps.LatLngBounds( ";
                            string bounds1 = "new google.maps.LatLng";
                            string bounds2 = "new google.maps.LatLng";
                            int localtotalAddedPolygonIndex = 0;
                            //determine how to handle bounds (2 edgepoints vs 4)
                            foreach (Coordinate_Point thisPoint in itemPolygon.Edge_Points)
                            {
                                if (itemPolygon.Edge_Points_Count == 2)
                                {
                                    if (localtotalAddedPolygonIndex == 0)
                                        bounds2 += "(" + Convert.ToString(thisPoint.Latitude) + "," + Convert.ToString(thisPoint.Longitude) + ")";
                                    if (localtotalAddedPolygonIndex == 1)
                                        bounds1 += "(" + Convert.ToString(thisPoint.Latitude) + "," + Convert.ToString(thisPoint.Longitude) + ")";
                                    localtotalAddedPolygonIndex++;
                                }
                                else
                                {
                                    if (itemPolygon.Edge_Points_Count == 4)
                                    {
                                        if (localtotalAddedPolygonIndex == 0)
                                            bounds1 += "(" + Convert.ToString(thisPoint.Latitude) + "," + Convert.ToString(thisPoint.Longitude) + ")";
                                        if (localtotalAddedPolygonIndex == 2)
                                            bounds2 += "(" + Convert.ToString(thisPoint.Latitude) + "," + Convert.ToString(thisPoint.Longitude) + ")";
                                        localtotalAddedPolygonIndex++;
                                    }
                                }
                            }
                            //finish bounds formatting
                            bounds += bounds2 + ", " + bounds1;
                            bounds += ")";
                            //add the bounds
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPath[" + totalAddedPolygonIndex + "] = " + bounds + ";"); //changed from bounds
                            
                            //add image url
                            try
                            {
                                //your way
                                List<SobekCM_File_Info> first_page_files = CurrentItem.Web.Pages_By_Sequence[totalAddedPolygonIndex].Files;
                                string first_page_jpeg = String.Empty;
                                foreach (SobekCM_File_Info thisFile in first_page_files)
                                {
                                    if ((thisFile.System_Name.ToLower().IndexOf(".jpg") > 0) && (thisFile.System_Name.ToLower().IndexOf("thm.jpg") < 0))
                                    {
                                        first_page_jpeg = thisFile.System_Name;
                                        break;
                                    }
                                }
                                string first_page_complete_url = "\"" + CurrentItem.Web.Source_URL + "/" + first_page_jpeg + "\"";
                                ////polygonURL[totalAddedPolygonIndex] = first_page_complete_url;
                                //polygonURL.Add(first_page_complete_url);
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL[" + totalAddedPolygonIndex + "] = " + first_page_complete_url + ";");
                            }
                            catch (Exception)
                            {
                                //there is no image
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL[" + totalAddedPolygonIndex + "] = null;");
                            }
                            
                            //add rotation
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[" + totalAddedPolygonIndex + "] = " + itemPolygon.Rotation + ";");
                            
                            //add page sequence
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[" + totalAddedPolygonIndex + "] = " + itemPolygon.Page_Sequence + ";");
                            
                            //increment the totalAddedPolygonCount (the actual number of polys added)
                            totalAddedPolygonCount++;

                            //iterate index
                            totalAddedPolygonIndex++;
                        }
                    }
                    #endregion

                    #region Add the page info so we can convert to overlays in the app
                    foreach (var page in pages)
                    {
                        if (totalAddedPolygonIndex < pages.Count)
                        {
                            //increment the totalAddedPolygonCount (the actual number of polys added)
                            totalAddedPolygonCount++;
                            //add featuretype
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[" + totalAddedPolygonIndex + "] = \"hidden\";");
                            //add polygontype
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPolygonType[" + totalAddedPolygonIndex + "] = \"hidden\";");
                            //add the label
                            if (Convert_String_To_XML_Safe(pages[totalAddedPolygonIndex].Label) != "")
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[" + totalAddedPolygonIndex + "] = \"" + Convert_String_To_XML_Safe(pages[totalAddedPolygonIndex].Label) + "\";");
                            else
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[" + totalAddedPolygonIndex + "] = \"Page " + (totalAddedPolygonIndex + 1) + "\";"); //2do localize this text???
                            //add page sequence
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[" + totalAddedPolygonIndex + "] = " + totalAddedPolygonCount + ";");
                            //add image url
                            try
                            {
                                //your way
                                List<SobekCM_File_Info> first_page_files = CurrentItem.Web.Pages_By_Sequence[totalAddedPolygonIndex].Files;

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
                                ////polygonURL[totalAddedPolygonIndex] = first_page_complete_url;
                                //polygonURL.Add(first_page_complete_url);
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL[" + totalAddedPolygonIndex + "] = " + first_page_complete_url + ";");
                            }
                            catch (Exception)
                            {
                                //my way
                                string current_image_file = CurrentItem.Web.Source_URL + "/" + CurrentItem.VID + ".jpg";
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL[" + totalAddedPolygonIndex + "] = \"" + current_image_file + "\"; ");
                                //throw;
                            }
                            //increment index
                            totalAddedPolygonIndex++;
                        }
                    }
                    #endregion

                    #region Add pois (order of adding is important)
                    foreach (Coordinate_Polygon itemPolygon in allPolygons)
                    {
                        if (itemPolygon.FeatureType == "poi")
                        {
                            //add the featureType
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[" + totalAddedPolygonIndex + "] = \"" + itemPolygon.FeatureType + "\";");
                            //add the polygonType
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPolygonType[" + totalAddedPolygonIndex + "] = \"" + itemPolygon.PolygonType + "\";");
                            //add the label
                            if (Convert_String_To_XML_Safe(itemPolygon.Label) != "")
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[" + totalAddedPolygonIndex + "] = \"" + Convert_String_To_XML_Safe(itemPolygon.Label) + "\";");
                            else
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[" + totalAddedPolygonIndex + "] = \"Page " + (totalAddedPolygonIndex + 1) + "\";"); //2do localize this text???
                            //add the polygon path
                            mapeditBuilder.Append("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPath[" + totalAddedPolygonIndex + "] = [ ");
                            int edgePointCurrentCount = 0;
                            foreach (Coordinate_Point thisPoint in itemPolygon.Edge_Points)
                            {
                                edgePointCurrentCount++;
                                //determine if this is the last edge point (fixes the js issue where the trailing , could cause older browsers to crash)
                                if (edgePointCurrentCount == itemPolygon.Edge_Points_Count)
                                    mapeditBuilder.Append("new google.maps.LatLng(" + thisPoint.Latitude + "," + thisPoint.Longitude + ") ");
                                else
                                    mapeditBuilder.Append("new google.maps.LatLng(" + thisPoint.Latitude + "," + thisPoint.Longitude + "), ");
                            }
                            mapeditBuilder.AppendLine("];");
                            //iterate
                            totalAddedPolygonIndex++;
                        }
                    }
                    #endregion
                    
                    mapeditBuilder.AppendLine(" ");
                    mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.displayIncomingPolygons(); ");
                    mapeditBuilder.AppendLine(" ");
                }
                else
                {
                    #region Add the page info so we can convert to overlays in the app
                    foreach (var page in pages)
                    {
                        if (totalAddedPolygonIndex < pages.Count)
                        {
                            //add featuretype
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[" + totalAddedPolygonIndex + "] = \"hidden\";");
                            //add polygontype
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPolygonType[" + totalAddedPolygonIndex + "] = \"hidden\";");
                            //add the label
                            if (Convert_String_To_XML_Safe(page.Label) != "")
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[" + totalAddedPolygonIndex + "] = \"" + Convert_String_To_XML_Safe(page.Label) + "\";");
                            else
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[" + totalAddedPolygonIndex + "] = \"Page " + (totalAddedPolygonIndex+1) + "\";"); //2do localize this text???
                            //add page sequence
                            mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[" + totalAddedPolygonIndex + "] = " + (totalAddedPolygonIndex + 1) + ";");
                            //add image url
                            try
                            {
                                //your way
                                List<SobekCM_File_Info> first_page_files = CurrentItem.Web.Pages_By_Sequence[totalAddedPolygonIndex].Files;

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
                                ////polygonURL[totalAddedPolygonIndex] = first_page_complete_url;
                                //polygonURL.Add(first_page_complete_url);
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL[" + totalAddedPolygonIndex + "] = " + first_page_complete_url + ";");
                            }
                            catch (Exception)
                            {
                                //my way
                                string current_image_file = CurrentItem.Web.Source_URL + "/" + CurrentItem.VID + ".jpg";
                                mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL[" + totalAddedPolygonIndex + "] = \"" + current_image_file + "\"; ");
                                //throw;
                            }
                            totalAddedPolygonIndex++;
                        }
                    }
                    #endregion
                    mapeditBuilder.AppendLine(" ");
                    mapeditBuilder.AppendLine("      MAPEDITOR.GLOBAL.displayIncomingPolygons(); ");
                    mapeditBuilder.AppendLine(" ");
                }
                #endregion
            }
            mapeditBuilder.AppendLine("    //MAPEDITOR.TRACER.addTracer(\"[INFO]: initGeoObjects completed...\"); ");
            mapeditBuilder.AppendLine("  }catch (err){ ");
            mapeditBuilder.AppendLine("    //MAPEDITOR.TRACER.addTracer(\"[ERROR]: initGeoObjects failed...\"); ");
            mapeditBuilder.AppendLine("  } ");
            mapeditBuilder.AppendLine(" }");
            mapeditBuilder.AppendLine(" ");
            mapeditBuilder.AppendLine(" </script> ");
            mapeditBuilder.AppendLine(" ");

            #endregion
            
            //determine if we are in devmode
            bool rapidTest = CurrentMode.Base_URL.Contains("localhost");
            if (rapidTest)
            {
                string[] lines = File.ReadAllLines(@"C:\Users\cadetpeters89\Documents\CUSTOM\projects\git\SobekCM-Web-Application\SobekCM\dev\mapedit\mapedit.html"); //2do make this dynamic
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
                mapeditBuilder.AppendLine("  ");
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
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_itemDelete\"></a></li> ");
                mapeditBuilder.AppendLine("                     </ul> ");
                mapeditBuilder.AppendLine("                 </li> ");
                mapeditBuilder.AppendLine("                 <li> ");
                mapeditBuilder.AppendLine("                     <a id=\"content_menubar_manageOverlay\"></a> ");
                mapeditBuilder.AppendLine("                     <ul> ");
                mapeditBuilder.AppendLine("                         <li><a id=\"content_menubar_overlayGetUserLocation\"></a></li> ");
                mapeditBuilder.AppendLine("                         <!--<li><a id=\"content_menubar_overlayEdit\"></a></li>--> ");
                mapeditBuilder.AppendLine("                         <!--<li><a id=\"content_menubar_overlayPlace\"></a></li>--> ");
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
                mapeditBuilder.AppendLine("                         <!--<li><a id=\"content_menubar_poiPlace\"></a></li>--> ");
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
                mapeditBuilder.AppendLine("  ");
                mapeditBuilder.AppendLine(" <div id=\"mapedit_container_pane_1\"> ");
                mapeditBuilder.AppendLine("     <div id=\"mapedit_container_toolbar\">        ");
                mapeditBuilder.AppendLine("         <div class=\"toolbar_grouping\"> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_reset\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_toggleMapControls\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_toggleToolbox\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("         </div> ");
                mapeditBuilder.AppendLine("         <div class=\"toolbar_grouping\"> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_layerRoadmap\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_layerTerrain\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_layerSatellite\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_layerHybrid\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_layerCustom\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_layerReset\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("         </div> ");
                mapeditBuilder.AppendLine("         <div class=\"toolbar_grouping\"> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_panUp\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_panLeft\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_panReset\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_panRight\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_panDown\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("         </div> ");
                mapeditBuilder.AppendLine("         <div class=\"toolbar_grouping\"> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_zoomIn\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_zoomReset\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_zoomOut\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("         </div> ");
                mapeditBuilder.AppendLine("         <div class=\"toolbar_grouping\"> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_manageItem\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_manageOverlay\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_managePOI\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("             <div id=\"content_toolbar_button_manageSearch\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("         </div> ");
                mapeditBuilder.AppendLine("         <div class=\"toolbar_grouping\"> ");
                mapeditBuilder.AppendLine("             <div class=\"mapedit_container_search\"> ");
                mapeditBuilder.AppendLine("                 <input id=\"content_toolbar_searchField\" class=\"search\" type=\"text\" placeholder=\"\" onClick=\"this.select();\" /> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbar_searchButton\" class=\"searchActionHandle\"></div> ");
                mapeditBuilder.AppendLine("             </div> ");
                mapeditBuilder.AppendLine("         </div> ");
                mapeditBuilder.AppendLine("     </div> ");
                mapeditBuilder.AppendLine(" </div> ");
                mapeditBuilder.AppendLine("  ");
                mapeditBuilder.AppendLine(" <div id=\"mapedit_container\"> ");
                mapeditBuilder.AppendLine("      ");
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
                mapeditBuilder.AppendLine("                             <div class=\"lineBreak\"></div> ");
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
                mapeditBuilder.AppendLine("                             <div class=\"button2\"> <input type=\"button\" id=\"content_toolbox_button_deleteItem\" > </div> ");
                mapeditBuilder.AppendLine("                         </div> ");
                mapeditBuilder.AppendLine("                     </div> ");
                mapeditBuilder.AppendLine("                 </div> ");
                mapeditBuilder.AppendLine("                 <div id=\"content_toolbox_tab4_header\" class=\"tab-title\"></div> ");
                mapeditBuilder.AppendLine("                 <div id=\"overlayACL\" class=\"tab\"> ");
                mapeditBuilder.AppendLine("                     <div class=\"toolbox_tab-content\"> ");
                mapeditBuilder.AppendLine("                         <!--<div id=\"content_toolbox_button_overlayEdit\" class=\"button\"></div>--> ");
                mapeditBuilder.AppendLine("                         <!--<div id=\"content_toolbox_button_overlayPlace\" class=\"button\"></div>--> ");
                mapeditBuilder.AppendLine("                         <div id=\"content_toolbox_button_overlayGetUserLocation\" class=\"button\"></div> ");
                mapeditBuilder.AppendLine("                         <div id=\"content_toolbox_button_overlayToggle\" class=\"button\"></div> ");
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
                mapeditBuilder.AppendLine("                         <!--<div id=\"content_toolbox_button_placePOI\" class=\"button\"></div>--> ");
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
            mapeditBuilder.AppendLine(" <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/sobekcm_mapedit.js\"></script> ");
            mapeditBuilder.AppendLine(" <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/mapedit/gmaps-markerwithlabel-1.9.1.js\"></script> "); //must load after custom

            //end of custom content
            mapeditBuilder.AppendLine("</td>");

            // Add the literal to the placeholder
            Literal placeHolderText = new Literal();
            placeHolderText.Text = mapeditBuilder.ToString();
            MainPlaceHolder.Controls.Add(placeHolderText);

            }
            catch (Exception)
            {
                Tracer.Add_Trace("Could Not Create MapEdit Page");
                throw;
            }

        }
    }
}
