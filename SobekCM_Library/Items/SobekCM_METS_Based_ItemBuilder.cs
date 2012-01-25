#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;
using SobekCM.Bib_Package.Divisions;
using SobekCM.Bib_Package.Maps;
using SobekCM.Bib_Package.Readers;
using SobekCM.Bib_Package.SobekCM_Info;
using SobekCM.Library.Application_State;
using SobekCM.Library.Database;

#endregion

namespace SobekCM.Library.Items
{
    /// <summary> Class reads the METS files for digital resource via HTTP and then constructs the 
    /// related digital resource object </summary>
    public class SobekCM_METS_Based_ItemBuilder
    {
        private int divseq, pageseq;

        /// <summary> Builds an item group object, from a METS file </summary>
        /// <param name="BibID"> Bibliographic identifier for the item group to retrieve </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Item_Group_Object"> [OUT] Fully built item group object </param>
        /// <param name="Items_In_Title"> [OUT] List of all the items in this title </param>
        public void Build_Item_Group(string BibID, Custom_Tracer Tracer, out SobekCM_Items_In_Title Items_In_Title, out SobekCM_Item Item_Group_Object )
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item_Group", "Create the requested item group");
            }

            // Set to NULL by default
            Item_Group_Object = null;
            Items_In_Title = null;

            // Get the basic information about this item
            DataSet itemDetails = SobekCM_Database.Get_Item_Group_Details(BibID, Tracer);

            // If this is NULL then there was an error
            if (itemDetails == null)
                return;

            // Get the location for the METS file from the returned value
            DataRow mainItemRow = itemDetails.Tables[0].Rows[0];
            string metsLocation = mainItemRow["File_Location"].ToString();

            // Get the response object for this METS file
            string metsFile = metsLocation.Replace("\\", "/") + "/" + BibID + ".xml";
            if (metsFile.IndexOf("http:") < 0)
            {
                metsFile = SobekCM_Library_Settings.Image_Server_Network + metsFile;
            }

            // Try to read this METS file
            bool pulledFromMETSFile = true;
            Item_Group_Object = Build_Item_From_METS(metsFile, BibID + ".xml", Tracer);

            // If this failed, just create an item from scratch
            if (Item_Group_Object == null)
            {
                Item_Group_Object = new SobekCM_Item();
                Item_Group_Object.METS.RecordStatus_Enum = METS_Record_Status.BIB_LEVEL;
                Item_Group_Object.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Serial;
                Item_Group_Object.BibID = BibID;
                pulledFromMETSFile = false;
            }

            // Pull values from the database
            Item_Group_Object.SobekCM_Web.GroupTitle = String.Empty;
            Item_Group_Object.SobekCM_Web.File_Root = String.Empty;
            Item_Group_Object.SobekCM_Web.Text_Searchable = false;
            Item_Group_Object.SobekCM_Web.Image_Root = SobekCM_Library_Settings.Image_URL;
            Item_Group_Object.SobekCM_Web.Siblings = 2;
            Item_Group_Object.SobekCM_Web.Static_PageCount = 0;
            Item_Group_Object.SobekCM_Web.Static_Division_Count = 0;
            Item_Group_Object.SobekCM_Web.AssocFilePath = "/" + BibID.Substring(0, 2) + "/" + BibID[2] + BibID[6] + "/" + BibID[4] + BibID[8] + "/" + BibID[3] + BibID[7] + "/" + BibID[5] + BibID[9] + "/";
            Item_Group_Object.SobekCM_Web.GroupID = Convert.ToInt32(mainItemRow["GroupID"]);
            Item_Group_Object.SobekCM_Web.Set_Primary_Identifier(mainItemRow["Primary_Identifier_Type"].ToString(), mainItemRow["Primary_Identifier"].ToString());

            // Add the full citation view and google map if pulled from the METS file
            if (pulledFromMETSFile)
            {
                // In addition, if there is a latitude or longitude listed, add the Google Maps
                if (Item_Group_Object.Bib_Info.hasCoordinateInformation)
                {
                    if ((Item_Group_Object.Bib_Info.Coordinates.Point_Count > 0) || (Item_Group_Object.Bib_Info.Coordinates.Polygon_Count > 0))
                    {
                        Item_Group_Object.SobekCM_Web.Insert_View(0, View_Enum.GOOGLE_MAP);
                    }
                }

                Item_Group_Object.SobekCM_Web.Insert_View(0, View_Enum.CITATION);
            }

            // If this has more than 1 sibling (this count includes itself), add the multi-volumes viewer
            Item_Group_Object.SobekCM_Web.Default_View = Item_Group_Object.SobekCM_Web.Insert_View(0, View_Enum.ALL_VOLUMES, String.Empty, Item_Group_Object.Bib_Info.SobekCM_Type_String);

            // Pull the data from the database
            Item_Group_Object.SobekCM_Web.GroupType = mainItemRow["Type"].ToString();
            Item_Group_Object.SobekCM_Web.GroupTitle = mainItemRow["GroupTitle"].ToString();
            Item_Group_Object.SobekCM_Web.File_Root = mainItemRow["File_Location"].ToString();

            // Create the list of items in this title
            Items_In_Title = new SobekCM_Items_In_Title(itemDetails.Tables[1]);

            // Add the database information to the icons now
            Item_Group_Object.SobekCM_Web.Clear_Wordmarks();
            foreach (DataRow thisIconRow in itemDetails.Tables[2].Rows)
            {
                Wordmark_Info newIcon = new Wordmark_Info();
                if ( thisIconRow["Link"].ToString().Length == 0)
                {
                    newIcon.Title = thisIconRow["Icon_Name"].ToString();
                    newIcon.Link = thisIconRow["Link"].ToString();
                    newIcon.HTML = "<img class=\"SobekItemWordmark\" src=\"<%BASEURL%>design/wordmarks/" + thisIconRow["Icon_URL"].ToString().Replace("&","&amp;") + "\" title=\"" + newIcon.Title.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" alt=\"" + newIcon.Title.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" />";
                }
                else
                {
                    newIcon.Title = thisIconRow["Icon_Name"].ToString();
                    newIcon.Link = thisIconRow["Link"].ToString();
                    newIcon.HTML = "<a href=\"" + newIcon.Link + "\" target=\"_blank\"><img class=\"SobekItemWordmark\" src=\"<%BASEURL%>design/wordmarks/" + thisIconRow["Icon_URL"].ToString().Replace("&", "&amp;").Replace("\"", "&quot;") + "\" title=\"" + newIcon.Title.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" alt=\"" + newIcon.Title.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a>";
                }
                Item_Group_Object.SobekCM_Web.Add_Wordmark(newIcon);
            }

            // Add the web skin codes to this bib-level item as well
            Item_Group_Object.SobekCM_Web.Clear_Web_Skins();
            foreach (DataRow thisRow in itemDetails.Tables[3].Rows)
            {
                Item_Group_Object.SobekCM_Web.Add_Web_Skin(thisRow[0].ToString().ToUpper());
            }

            // Set the aggregations in the package to the aggregation links from the database
            if (itemDetails.Tables.Count == 6)
            {
                Item_Group_Object.SobekCM_Web.Clear_Aggregations();
                foreach (DataRow thisRow in itemDetails.Tables[4].Rows)
                {
                    Item_Group_Object.SobekCM_Web.Add_Aggregation(thisRow[0].ToString());
                }

                // Add the related titles, if there are some
                foreach (DataRow thisRow in itemDetails.Tables[5].Rows)
                {
                    string relationship = thisRow["Relationship"].ToString();
                    string title = thisRow["GroupTitle"].ToString();
                    string bibid = thisRow["BibID"].ToString();
                    string link_and_title = "<a href=\"<%BASEURL%>" + bibid + "<%URL_OPTS%>\">" + title + "</a>";
                    Item_Group_Object.SobekCM_Web.All_Related_Titles.Add(new Related_Titles(relationship, link_and_title));
                }
            }
        }

        /// <summary> Builds a brief version of a digital resource, used when displaying the 'FULL VIEW' in 
        /// a search result or browse list </summary>
        /// <param name="METS_Location"> Location (URL) of the METS file to read via HTTP </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Briefly built version of a digital resource </returns>
        public SobekCM_Item Build_Brief_Item(string METS_Location, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Brief_Item", "Create the requested item");
            }

            try
            {
                // Get the response object for this METS file
                //string mets_file = METS_Location.Replace(".53/",".47/") + "/ufdc_mets2.xml";
                string mets_file = METS_Location.Replace("\\", "/") + "/citation_mets.xml";

                SobekCM_Item thisPackage = Build_Item_From_METS(mets_file, "citation_mets.xml", Tracer);

                // Check if that failed
                if (thisPackage == null)
                {
                    const string secondMETSFile = "ufdc_mets.xml";
                    thisPackage = Build_Item_From_METS(METS_Location + "/" + secondMETSFile, secondMETSFile, Tracer);
                }

                if (thisPackage == null)
                {
                    if (Tracer != null)
                        Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Brief_Item", "Unable to find/read either METS file", Custom_Trace_Type_Enum.Error);
                }

                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Brief_Item", "Finished building this item");
                }

                return thisPackage;
            }
            catch (Exception ee)
            {
                if (Tracer != null)
                    Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Brief_Item", ee.ToString().Replace("\n", "<br />"), Custom_Trace_Type_Enum.Error);
                return null;
            }
        }

        /// <summary> Builds a digital resource for a single volume within a title </summary>
        /// <param name="BibID"> Bibliographic identifier for the title </param>
        /// <param name="VID"> Volume identifier for the title </param>
        /// <param name="Icon_Dictionary"> Dictionary of information about every wordmark/icon in this digital library, used to build the HTML for the icons linked to this digital resource</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Fully built version of a digital resource </returns>
        public SobekCM_Item Build_Item(string BibID, string VID, Dictionary<string, Wordmark_Icon> Icon_Dictionary, Custom_Tracer Tracer)
        {
            if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item", "Create the requested item");
                }

            try
            {
                // Get the basic information about this item
                DataSet itemDetails = SobekCM_Database.Get_Item_Details(BibID, VID, Tracer);

                // If the itemdetails was null, this item is somehow invalid item then
                if ( itemDetails == null )
                    return null;

                // Get the location for this METS file from the returned value
                DataRow mainItemRow = itemDetails.Tables[2].Rows[0];
                string mets_location = mainItemRow["File_Location"] + "/" + VID;
                if ( mets_location.Length < 10 )
                {
                    mets_location = BibID.Substring(0, 2) + "/" + BibID.Substring(2, 2) + "/" + BibID.Substring(4, 2) + "/" + BibID.Substring(6, 2) + "/" + BibID.Substring(8) + "/" + VID;
                }
               


                // Get the response object for this METS file
                string mets_file = mets_location.Replace("\\", "/") + "/" + BibID + "_" + VID + ".mets.xml";
                if (mets_file.IndexOf("http:") < 0)
                {
                    mets_file = SobekCM_Library_Settings.Image_Server_Network + mets_file;
                }

                // Could point directly to a METS file in some off-site location, in which case, use it
                if (mets_location.IndexOf(".mets") > 0)
                    mets_file = mets_location;

                // Try to read the UFDC service METS
                SobekCM_Item thisPackage = Build_Item_From_METS(mets_file, BibID + "_" + VID + ".mets.xml", Tracer);

                if (thisPackage == null)
                {
                    if (Tracer != null)
                    {
                        Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item", "Unable to read the UFDC service METS file", Custom_Trace_Type_Enum.Error);
                    }

                    return null;
                }

                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item", "Finish building this item");
                }

                // Check to see if multiple sibling volumes exist
                bool multiple_volumes_exist = false;
                if (Convert.ToInt32(mainItemRow["Total_Volumes"]) > 1)
                {
                    multiple_volumes_exist = true;
                }

                // Now finish building the object from the application state values
                Finish_Building_Item(thisPackage, itemDetails, multiple_volumes_exist);              

                return thisPackage;
            }
            catch (Exception ee)
            {
                if (Tracer != null)
                    Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item", ee.ToString().Replace("\n", "<br />"), Custom_Trace_Type_Enum.Error);
                return null;
            }
        }

        private SobekCM_Item Build_Item_From_METS(string METS_URL, string METS_Name, Custom_Tracer tracer)
        {
            try
            {
                if (tracer != null)
                {
                    tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item_From_METS", "Open http web request stream to METS file ( <a href=\"" + METS_URL + "\">" + METS_Name + "</a> )");
                }

                SobekCM_Item thisPackage = new SobekCM_Item();
                if (METS_URL.IndexOf("http:") >= 0)
                {
                    WebRequest objRequest = WebRequest.Create(METS_URL);
                    objRequest.Timeout = 5000;
                    WebResponse objResponse = objRequest.GetResponse();

                    if (tracer != null)
                    {
                        tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item_From_METS", "Read the METS file from the stream");
                    }

                    // Read the METS file and create the package
                    METS_Reader reader = new METS_Reader();
                    reader.Read_METS(objResponse.GetResponseStream(), thisPackage);
                    objResponse.Close();
                }
                else
                {
                    if (File.Exists(METS_URL.Replace("/", "\\")))
                    {
                        // Read the METS file and create the package
                        METS_Reader reader = new METS_Reader();
                        reader.Read_METS(METS_URL.Replace("/", "\\"), thisPackage);
                    }
                    else
                    {
                        return null;
                    }
                }

                return thisPackage;
            }
            catch
            {
                return null;
            }
        }

        #region Private methods for finalizing builds

        private void Finish_Building_Item(SobekCM_Item thisPackage, DataSet databaseInfo, bool multiple)
        {
            // Copy over some basic values
            DataRow mainItemRow = databaseInfo.Tables[2].Rows[0];
            thisPackage.SobekCM_Web.Set_Primary_Identifier(mainItemRow["Primary_Identifier_Type"].ToString(), mainItemRow["Primary_Identifier"].ToString());
            thisPackage.SobekCM_Web.GroupTitle = mainItemRow["GroupTitle"].ToString();
            thisPackage.SobekCM_Web.File_Root = mainItemRow["File_Location"].ToString();
            thisPackage.SobekCM_Web.AssocFilePath = mainItemRow["File_Location"] + "\\" + thisPackage.VID + "\\";
            thisPackage.SobekCM_Web.IP_Restriction_Membership = Convert.ToInt16(mainItemRow["IP_Restriction_Mask"]);             
            thisPackage.SobekCM_Web.CheckOut_Required = Convert.ToBoolean(mainItemRow["CheckoutRequired"]);
            thisPackage.SobekCM_Web.Text_Searchable = Convert.ToBoolean(mainItemRow["TextSearchable"]);
            thisPackage.SobekCM_Web.ItemID = Convert.ToInt32(mainItemRow["ItemID"]);
            thisPackage.SobekCM_Web.GroupID = Convert.ToInt32(mainItemRow["GroupID"]);
            thisPackage.SobekCM_Web.Suppress_Endeca = Convert.ToBoolean(mainItemRow["SuppressEndeca"]);
            //thisPackage.SobekCM_Web.Expose_Full_Text_For_Harvesting = Convert.ToBoolean(mainItemRow["SuppressEndeca"]);
            thisPackage.Tracking.Internal_Comments = mainItemRow["Comments"].ToString();
            thisPackage.SobekCM_Web.Dark_Flag = Convert.ToBoolean(mainItemRow["Dark"]);
            thisPackage.Tracking.Born_Digital = Convert.ToBoolean(mainItemRow["Born_Digital"]);
            //thisPackage.Divisions.Page_Count = Convert.ToInt32(mainItemRow["Pages"]);
            if (mainItemRow["Disposition_Advice"] != DBNull.Value)
                thisPackage.Tracking.Disposition_Advice = Convert.ToInt16(mainItemRow["Disposition_Advice"]);
            else
                thisPackage.Tracking.Disposition_Advice = -1;
            if (mainItemRow["Material_Received_Date"] != DBNull.Value)
                thisPackage.Tracking.Material_Received_Date = Convert.ToDateTime(mainItemRow["Material_Received_Date"]);
            else
                thisPackage.Tracking.Material_Received_Date = null;
            if (mainItemRow["Material_Recd_Date_Estimated"] != DBNull.Value)
                thisPackage.Tracking.Material_Rec_Date_Estimated = Convert.ToBoolean(mainItemRow["Material_Recd_Date_Estimated"]);
            if (databaseInfo.Tables[2].Columns.Contains("Tracking_Box"))
            {
                if (mainItemRow["Tracking_Box"] != DBNull.Value)
                    thisPackage.Tracking.Tracking_Box= mainItemRow["Tracking_Box"].ToString();
            }


            // Set more of the sobekcm web portions in the item 
            thisPackage.SobekCM_Web.Set_BibID_VID(thisPackage.BibID, thisPackage.VID);
            thisPackage.SobekCM_Web.Image_Root = SobekCM_Library_Settings.Image_URL;
            if (multiple)
                thisPackage.SobekCM_Web.Siblings = 2;

            // Set the serial hierarchy from the database (if multiple)
            if ((multiple) && (mainItemRow["Level1_Text"].ToString().Length > 0))
            {
                bool found = false;

                // Get the values from the database first
                string level1_text = mainItemRow["Level1_Text"].ToString();
                string level2_text = mainItemRow["Level2_Text"].ToString();
                string level3_text = mainItemRow["Level3_Text"].ToString();
                int level1_index = Convert.ToInt32(mainItemRow["Level1_Index"]);
                int level2_index = Convert.ToInt32(mainItemRow["Level2_Index"]);
                int level3_index = Convert.ToInt32(mainItemRow["Level3_Index"]);

                // Does this match the enumeration
                if (level1_text.ToUpper().Trim() == thisPackage.Bib_Info.Series_Part_Info.Enum1.ToUpper().Trim())
                {
                    // Copy the database values to the enumeration portion
                    thisPackage.Bib_Info.Series_Part_Info.Enum1 = level1_text;
                    thisPackage.Bib_Info.Series_Part_Info.Enum1_Index = level1_index;
                    thisPackage.Bib_Info.Series_Part_Info.Enum2 = level2_text;
                    thisPackage.Bib_Info.Series_Part_Info.Enum2_Index = level2_index;
                    thisPackage.Bib_Info.Series_Part_Info.Enum3 = level3_text;
                    thisPackage.Bib_Info.Series_Part_Info.Enum3_Index = level3_index;
                    found = true;
                }

                // Does this match the chronology
                if ((!found) && (level1_text.ToUpper().Trim() == thisPackage.Bib_Info.Series_Part_Info.Year.ToUpper().Trim()))
                {
                    // Copy the database values to the chronology portion
                    thisPackage.Bib_Info.Series_Part_Info.Year = level1_text;
                    thisPackage.Bib_Info.Series_Part_Info.Year_Index = level1_index;
                    thisPackage.Bib_Info.Series_Part_Info.Month = level2_text;
                    thisPackage.Bib_Info.Series_Part_Info.Month_Index = level2_index;
                    thisPackage.Bib_Info.Series_Part_Info.Day = level3_text;
                    thisPackage.Bib_Info.Series_Part_Info.Day_Index = level3_index;
                    found = true;
                }

                if (!found)
                {
                    // No match.  If it is numeric, move it to the chronology, otherwise, enumeration
                    bool charFound = level1_text.Trim().Any(thisChar => !Char.IsNumber(thisChar));

                    if (charFound)
                    {
                        // Copy the database values to the enumeration portion
                        thisPackage.Bib_Info.Series_Part_Info.Enum1 = level1_text;
                        thisPackage.Bib_Info.Series_Part_Info.Enum1_Index = level1_index;
                        thisPackage.Bib_Info.Series_Part_Info.Enum2 = level2_text;
                        thisPackage.Bib_Info.Series_Part_Info.Enum2_Index = level2_index;
                        thisPackage.Bib_Info.Series_Part_Info.Enum3 = level3_text;
                        thisPackage.Bib_Info.Series_Part_Info.Enum3_Index = level3_index;
                    }
                    else
                    {
                        // Copy the database values to the chronology portion
                        thisPackage.Bib_Info.Series_Part_Info.Year = level1_text;
                        thisPackage.Bib_Info.Series_Part_Info.Year_Index = level1_index;
                        thisPackage.Bib_Info.Series_Part_Info.Month = level2_text;
                        thisPackage.Bib_Info.Series_Part_Info.Month_Index = level2_index;
                        thisPackage.Bib_Info.Series_Part_Info.Day = level3_text;
                        thisPackage.Bib_Info.Series_Part_Info.Day_Index = level3_index;
                    }
                }

                // Copy the database values to the simple serial portion (used to actually determine serial heirarchy)
                thisPackage.Serial_Info.Clear();
                thisPackage.Serial_Info.Add_Hierarchy(1, level1_index, level1_text);
                if (level2_text.Length > 0)
                {
                    thisPackage.Serial_Info.Add_Hierarchy(2, level2_index, level2_text);
                    if (level3_text.Length > 0)
                    {
                        thisPackage.Serial_Info.Add_Hierarchy(3, level3_index, level3_text);
                    }
                }
            }

            // See if this can be described
            bool can_describe = false;
            foreach (DataRow thisRow in databaseInfo.Tables[1].Rows)
            {
                int thisAggregationValue = Convert.ToInt16(thisRow["Items_Can_Be_Described"]);
                if (thisAggregationValue == 0)
                {
                    can_describe = false;
                    break;
                }
                if (thisAggregationValue == 2)
                {
                    can_describe = true;
                }
            }
            thisPackage.SobekCM_Web.Can_Be_Described = can_describe;

            // Look for user descriptions
            foreach (DataRow thisRow in databaseInfo.Tables[0].Rows)
            {
                string first_name = thisRow["FirstName"].ToString();
                string nick_name = thisRow["NickName"].ToString();
                string last_name = thisRow["LastName"].ToString();
                int userid = Convert.ToInt32(thisRow["UserID"]);
                string tag = thisRow["Description_Tag"].ToString();
                int tagid = Convert.ToInt32(thisRow["TagID"]);
                DateTime dateAdded = Convert.ToDateTime(thisRow["Date_Modified"]);

                if (nick_name.Length > 0)
                {
                    thisPackage.SobekCM_Web.Add_User_Tag(userid, nick_name + " " + last_name, tag, dateAdded, tagid);
                }
                else
                {
                    thisPackage.SobekCM_Web.Add_User_Tag(userid, first_name + " " + last_name, tag, dateAdded, tagid);
                }
            }

            // Look for ticklers
            foreach (DataRow thisRow in databaseInfo.Tables[3].Rows)
            {
                thisPackage.SobekCM_Web.Add_Tickler(thisRow["MetadataValue"].ToString().Trim());
            }

            // Set the aggregations in the package to the aggregation links from the database
            thisPackage.SobekCM_Web.Clear_Aggregations();
            foreach (DataRow thisRow in databaseInfo.Tables[1].Rows)
            {
                if (!Convert.ToBoolean(thisRow["impliedLink"]))
                {
                    thisPackage.SobekCM_Web.Add_Aggregation(thisRow["Code"].ToString());
                }
            }

            // If no collections, add some regardless of whether it was IMPLIED
            if ( thisPackage.SobekCM_Web.Aggregation_Count == 0)
            {
                foreach (DataRow thisRow in databaseInfo.Tables[1].Rows)
                {
                    if (thisRow["Type"].ToString().ToUpper() == "COLLECTION")
                        thisPackage.SobekCM_Web.Add_Aggregation(thisRow["Code"].ToString());
                }
            }

            // Step through each page and set the static page count
            pageseq = 0;
            List<Page_TreeNode> pages_encountered = new List<Page_TreeNode>();
            foreach (abstract_TreeNode rootNode in thisPackage.Divisions.Physical_Tree.Roots)
            {
                recurse_through_nodes(thisPackage, rootNode, pages_encountered);
            }
            thisPackage.SobekCM_Web.Static_PageCount = pages_encountered.Count;
            thisPackage.SobekCM_Web.Static_Division_Count = divseq;

            // Make sure no icons were retained from the METS file itself
            thisPackage.SobekCM_Web.Clear_Wordmarks();

            // Add the icons from the database information
            foreach (DataRow iconRow in databaseInfo.Tables[5].Rows)
            {
                string image = iconRow[0].ToString();
                string link = iconRow[1].ToString().Replace("&", "&amp;").Replace("\"", "&quot;");
                string code = iconRow[2].ToString();
                string name = code.Replace("&", "&amp;").Replace("\"", "&quot;"); 

                string html;
                if (link.Length == 0)
                {
                    html = "<img class=\"SobekItemWordmark\" src=\"<%BASEURL%>design/wordmarks/" + image + "\" title=\"" + name + "\" alt=\"" + name + "\" />";
                }
                else
                {
                    if (link[0] == '?')
                    {
                        html = "<a href=\"" + link + "\"><img class=\"SobekItemWordmark\" src=\"<%BASEURL%>design/wordmarks/" + image + "\" title=\"" + name + "\" alt=\"" + name + "\" /></a>";
                    }
                    else
                    {
                        html = "<a href=\"" + link + "\" target=\"_blank\"><img class=\"SobekItemWordmark\" src=\"<%BASEURL%>design/wordmarks/" + image + "\" title=\"" + name + "\" alt=\"" + name + "\" /></a>";
                    }
                }

                Wordmark_Info newIcon = new Wordmark_Info {HTML = html, Link = link, Title = name, Code = code};
                thisPackage.SobekCM_Web.Add_Wordmark(newIcon);
            }

            // Make sure no web skins were retained from the METS file itself
            thisPackage.SobekCM_Web.Clear_Web_Skins();

            // Add the web skins from the database
            foreach (DataRow skinRow in databaseInfo.Tables[6].Rows)
            {
                thisPackage.SobekCM_Web.Add_Web_Skin(skinRow[0].ToString().ToUpper());
            }

            // Make sure no views were retained from the METS file itself
            thisPackage.SobekCM_Web.Clear_Views();

            // If this has more than 1 sibling (this count includes itself), add the multi-volumes viewer
            if (multiple)
            {
                thisPackage.SobekCM_Web.Add_View(View_Enum.ALL_VOLUMES, String.Empty, thisPackage.Bib_Info.SobekCM_Type_String);
            }

            // Add the full citation view and the (hidden) tracking view 
            thisPackage.SobekCM_Web.Add_View(View_Enum.CITATION);
            thisPackage.SobekCM_Web.Add_View(View_Enum.TRACKING);



            // Add the full text 
            if ( thisPackage.SobekCM_Web.Text_Searchable )
                thisPackage.SobekCM_Web.Add_View(View_Enum.SEARCH);

            // IF this is dark, add no other views
            if (!thisPackage.SobekCM_Web.Dark_Flag)
            {
                // Check to see which views were present from the database, and build the list
                Dictionary<View_Enum, View_Object> viewsFromDb = new Dictionary<View_Enum, View_Object>();
                foreach (DataRow viewRow in databaseInfo.Tables[4].Rows)
                {
                    string viewType = viewRow[0].ToString();
                    string attribute = viewRow[1].ToString();
                    string label = viewRow[2].ToString();

                    View_Enum viewTypeEnum = View_Enum.None;
                    switch (viewType)
                    {
                        case "JPEG":
                            viewTypeEnum = View_Enum.JPEG;
                            break;

                        case "JPEG2000":
                            viewTypeEnum = View_Enum.JPEG2000;
                            break;

                        case "Text":
                            viewTypeEnum = View_Enum.TEXT;
                            break;

                        case "Page Turner":
                            viewTypeEnum = View_Enum.PAGE_TURNER;
                            break;

                        case "Google Map":
                            viewTypeEnum = View_Enum.GOOGLE_MAP;
                            break;

                        case "HTML Viewer":
                            viewTypeEnum = View_Enum.HTML;
                            break;

                        case "HTML Map Viewer":
                            viewTypeEnum = View_Enum.HTML_MAP;
                            break;

                        case "Related Images":
                            viewTypeEnum = View_Enum.RELATED_IMAGES;
                            break;

                        case "TOC":
                            viewTypeEnum = View_Enum.TOC;
                            break;
                    }

                    if (viewTypeEnum != View_Enum.None)
                    {
                        viewsFromDb[viewTypeEnum] = new View_Object(viewTypeEnum, label, attribute);
                    }
                }

                // Add the thumbnail view, if requested and has multiple pages
                if (thisPackage.Divisions.Page_Count > 1)
                {
                    if (viewsFromDb.ContainsKey(View_Enum.RELATED_IMAGES))
                    {
                        thisPackage.SobekCM_Web.Add_View(viewsFromDb[View_Enum.RELATED_IMAGES]);
                        viewsFromDb.Remove(View_Enum.RELATED_IMAGES);
                    }
                }
                else
                {
                    if (viewsFromDb.ContainsKey(View_Enum.RELATED_IMAGES))
                    {
                        viewsFromDb.Remove(View_Enum.RELATED_IMAGES);
                    }
                }

                // If this item has more than one division, look for the TOC viewer
                if ((thisPackage.Divisions.Has_Multiple_Divisions) && (!thisPackage.Bib_Info.ImageClass))
                {
                    if (viewsFromDb.ContainsKey(View_Enum.TOC))
                    {
                        thisPackage.SobekCM_Web.Add_View(viewsFromDb[View_Enum.TOC]);
                        viewsFromDb.Remove(View_Enum.TOC);
                    }
                }

                // In addition, if there is a latitude or longitude listed, look for the Google Maps
                if (thisPackage.Bib_Info.hasCoordinateInformation)
                {
                    if ((thisPackage.Bib_Info.Coordinates.Point_Count > 0) || (thisPackage.Bib_Info.Coordinates.Polygon_Count > 0))
                    {
                        if (viewsFromDb.ContainsKey(View_Enum.GOOGLE_MAP))
                        {
                            thisPackage.SobekCM_Web.Add_View(viewsFromDb[View_Enum.GOOGLE_MAP]);
                            viewsFromDb.Remove(View_Enum.GOOGLE_MAP);
                        }
                        else
                        {
                            thisPackage.SobekCM_Web.Add_View(View_Enum.GOOGLE_MAP);
                        }
                    }
                }

   

                // Step through each download and make sure it is fully built
                if (thisPackage.Divisions.Download_Tree.Has_Files)
                {
                    string ead_file = String.Empty;
                    int pdf_download = 0;
                    string pdf_download_url = String.Empty;
                    int non_flash_downloads = 0;
                    List<abstract_TreeNode> downloadPages = thisPackage.Divisions.Download_Tree.Pages_PreOrder;
                    foreach (Page_TreeNode downloadPage in downloadPages)
                    {
                        // Was this an EAD page?
                        if ((downloadPage.Label == "EAD") && (downloadPage.Files.Count == 1))
                        {
                            ead_file = downloadPage.Files[0].System_Name;
                        }

                        // Was this an XSL/EAD page?
                        if ((downloadPage.Label == "XSL") && (downloadPage.Files.Count == 1))
                        {
                        }

                        // Step through each download file
                        foreach (SobekCM_File_Info thisFile in downloadPage.Files)
                        {
                            if (thisFile.File_Extension == "SWF")
                            {
                                string flashlabel = downloadPage.Label;
                                View_Object newView = thisPackage.SobekCM_Web.Add_View(View_Enum.FLASH, flashlabel, String.Empty, thisFile.System_Name);
                                thisPackage.SobekCM_Web.Default_View = newView;
                            }
                            else
                            {
                                non_flash_downloads++;
                            }

                            if (thisFile.File_Extension == "PDF")
                            {
                                pdf_download++;
                                pdf_download_url = thisFile.System_Name;
                            }
                        }
                    }

                    if (((non_flash_downloads > 0) && (pdf_download != 1)) || ((non_flash_downloads > 1) && (pdf_download == 1)))
                    {

                        if (thisPackage.SobekCM_Web.Static_PageCount == 0)
                            thisPackage.SobekCM_Web.Default_View = thisPackage.SobekCM_Web.Add_View(View_Enum.DOWNLOADS);
                        else
                            thisPackage.SobekCM_Web.Add_View(View_Enum.DOWNLOADS);
                    }

                    if (pdf_download == 1)
                    {
                        if (thisPackage.SobekCM_Web.Static_PageCount == 0)
                        {
                            thisPackage.SobekCM_Web.Default_View = thisPackage.SobekCM_Web.Add_View(View_Enum.PDF);
                            thisPackage.SobekCM_Web.Default_View.FileName = pdf_download_url;
                        }
                        else
                        {
                            thisPackage.SobekCM_Web.Add_View(View_Enum.PDF).FileName = pdf_download_url;
                        }
                    }

                    // Some special code for EAD objects
                    if ((thisPackage.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Archival ) && (ead_file.Length > 0))
                    {
                        // Now, read this EAD file information 
                        string ead_file_location = SobekCM_Library_Settings.Image_Server_Network + thisPackage.SobekCM_Web.AssocFilePath + ead_file;
                        EAD_Reader.Add_EAD_Information(thisPackage, ead_file_location, SobekCM_Library_Settings.System_Base_URL + "default/sobekcm_default.xsl");

                        // Clear all existing views
                        thisPackage.SobekCM_Web.Clear_Views();
                        thisPackage.SobekCM_Web.Add_View(View_Enum.CITATION);
                        thisPackage.SobekCM_Web.Default_View = thisPackage.SobekCM_Web.Add_View(View_Enum.EAD_DESCRIPTION);
                        if (thisPackage.EAD.Container_Hierarchy.C_Tags.Count > 0)
                            thisPackage.SobekCM_Web.Add_View(View_Enum.EAD_CONTAINER_LIST);

                    }
                }
                else
                {
                    if (thisPackage.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Aerial )
                    {
                        thisPackage.SobekCM_Web.Add_View(View_Enum.DOWNLOADS);
                    }
                }

                // If there is a RELATED URL with youtube, add that viewer
                if ((thisPackage.Bib_Info.hasLocationInformation) && (thisPackage.Bib_Info.Location.Other_URL.ToLower().IndexOf("www.youtube.com") >= 0))
                {
                    View_Object newViewObj = new View_Object(View_Enum.YOUTUBE_VIDEO);
                    thisPackage.SobekCM_Web.Add_View(newViewObj);
                    thisPackage.SobekCM_Web.Default_View = newViewObj;
                }

                // Look for the HTML type views next, and possible set some defaults
                if (viewsFromDb.ContainsKey(View_Enum.HTML))
                {
                    thisPackage.SobekCM_Web.Add_View(viewsFromDb[View_Enum.HTML]);
                    thisPackage.SobekCM_Web.Default_View = viewsFromDb[View_Enum.HTML];
                    viewsFromDb.Remove(View_Enum.HTML);
                }

                // Look for the HTML MAP type views next, and possible set some defaults
                if (viewsFromDb.ContainsKey(View_Enum.HTML_MAP))
                {
                    thisPackage.SobekCM_Web.Add_View(viewsFromDb[View_Enum.HTML_MAP]);
                    thisPackage.SobekCM_Web.Default_View = viewsFromDb[View_Enum.HTML_MAP];
                    viewsFromDb.Remove(View_Enum.HTML_MAP);
                }

                // Look to add any index information here ( such as on SANBORN maps)
                if (thisPackage.Map != null)
                {
                    // Was there a HTML map here?
                    if (thisPackage.Map.Index_Count > 0)
                    {
                        Map_Index thisIndex = thisPackage.Map.Get_Index(0);
                        View_Object newMapSanbView = thisPackage.SobekCM_Web.Add_View(View_Enum.HTML_MAP, thisIndex.Title, thisIndex.Image_File + ";" + thisIndex.HTML_File);
                        thisPackage.SobekCM_Web.Default_View = newMapSanbView;
                    }

                    //// Were there streets?
                    //if (thisPackage.Map.Streets.Count > 0)
                    //{
                    //    returnValue.Item_Views.Add(new ViewerFetcher.Streets_ViewerFetcher());
                    //}

                    //// Were there features?
                    //if (thisPackage.Map.Features.Count > 0)
                    //{
                    //    returnValue.Item_Views.Add(new ViewerFetcher.Features_ViewerFetcher());
                    //}
                }

                // Look for the RELATED IMAGES view next
                if (viewsFromDb.ContainsKey(View_Enum.RELATED_IMAGES))
                {
                    thisPackage.SobekCM_Web.Add_View(viewsFromDb[View_Enum.RELATED_IMAGES]);
                    viewsFromDb.Remove(View_Enum.RELATED_IMAGES);
                }

                // Look for the PAGE TURNER view next
                if (viewsFromDb.ContainsKey(View_Enum.PAGE_TURNER))
                {
                    thisPackage.SobekCM_Web.Add_View(viewsFromDb[View_Enum.PAGE_TURNER]);
                    viewsFromDb.Remove(View_Enum.PAGE_TURNER);
                }

                // Finally, add all the ITEM VIEWS
                foreach (View_Object thisObject in viewsFromDb.Values)
                {
                    switch (thisObject.View_Type)
                    {
                        case View_Enum.TEXT:
                        case View_Enum.JPEG:
                        case View_Enum.JPEG2000:
                            thisPackage.SobekCM_Web.Add_Item_Level_Page_View(thisObject);
                            break;
                    }
                }
            }
        }


        private void recurse_through_nodes( SobekCM_Item thisPackage, abstract_TreeNode node, List<Page_TreeNode> pages_encountered )
        {
            if (node.Page)
            {
                Page_TreeNode pageNode = (Page_TreeNode)node;
                if ( !pages_encountered.Contains( pageNode ))
                {
                    pageseq++;

                    // Add each of the files view codes to the list
                    bool page_added = false;
                    foreach (SobekCM_File_Info thisFile in pageNode.Files)
                    {
                        string upper_name = thisFile.System_Name.ToUpper();
                        if ((upper_name.IndexOf("SOUNDFILESONLY") < 0) && ( upper_name.IndexOf("FILMONLY") < 0 ) && ( upper_name.IndexOf("MULTIMEDIA") < 0 ) && ( upper_name.IndexOf("THM.JPG") < 0 ))
                        {
                            if (!page_added)
                            {
                                // Add this to the simple page collection
                                thisPackage.SobekCM_Web.Add_Pages_By_Sequence(pageNode);
                                pages_encountered.Add(pageNode);
                                page_added = true;
                            }
                            View_Object thisViewer = thisFile.Get_Viewer();
                            if (thisViewer != null)
                            {
                                string[] codes = thisViewer.Viewer_Codes;
                                if ((codes.Length > 0) && (codes[0].Length > 0))
                                {
                                    thisPackage.SobekCM_Web.Viewer_To_File[pageseq.ToString() + codes[0]] = thisFile;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                divseq++;
                Division_TreeNode divNode = (Division_TreeNode)node;
                foreach (abstract_TreeNode childNode in divNode.Nodes)
                {
                    recurse_through_nodes(thisPackage, childNode, pages_encountered);
                }
            }
        }

        #endregion
    }
}