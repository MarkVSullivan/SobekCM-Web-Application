#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using EngineAgnosticLayerDbAccess;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Database.DataSets;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.OAI.Writer;

#endregion

namespace SobekCM_Resource_Database
{

    /// <summary> SobekCM_Database is the main object used to query the database for information </summary>
    /// <remarks> This class contains a static constructor and contains mostly static 
    /// members.   Since this is a database class, it does not make sense to have multiple
    /// instances to access a single database. <br /><br />
    /// Object created by Mark V Sullivan and Ying Tang (2006) for University of Florida's Digital Library Center.</remarks>
    public class SobekCM_Item_Database
    {
        /// <summary> Private constant string variable stores the connection string 
        /// to get to the SobekCM Database on the SQL server. </summary>
        private static string connectionString = "data source=lib-ufdc-cache\\UFDCPROD;initial catalog=UFDC_prod;integrated security=SSPI;persist security info=False;workstation id=WSID3246;packet size=4096";

        /// <summary> Gets the type of database ( i.e. MSSQL v. PostgreSQL ) </summary>
        public static EalDbTypeEnum DatabaseType { get; set; }

        /// <summary> Static constructor for the SobekCM_Database class </summary>
        static SobekCM_Item_Database()
        {
            DatabaseType = EalDbTypeEnum.MSSQL;
        }

        /// <summary> Pulls the item id, main thumbnail, and aggregation codes and adds them to the resource object </summary>
        /// <param name="Resource"> Digital resource object </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Builder_Get_Minimum_Item_Information' stored procedure </remarks> 
        public static bool Add_Minimum_Builder_Information(SobekCM_Item Resource)
        {
            try
            {
                EalDbParameter[] parameters = new EalDbParameter[2];
                parameters[0] = new EalDbParameter("@bibid", Resource.BibID);
                parameters[1] = new EalDbParameter("@vid", Resource.VID);

                // Define a temporary dataset
                DataSet tempSet = EalDbAccess.ExecuteDataset(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Builder_Get_Minimum_Item_Information", parameters);

                // If there was no data for this collection and entry point, return null (an ERROR occurred)
                if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
                {
                    return false;
                }

                // Get the item id and the thumbnail from the first table
                Resource.Web.ItemID = Convert.ToInt32(tempSet.Tables[0].Rows[0][0]);
                Resource.Behaviors.Main_Thumbnail = tempSet.Tables[0].Rows[0][1].ToString();
                Resource.Behaviors.IP_Restriction_Membership = Convert.ToInt16(tempSet.Tables[0].Rows[0][2]);
                Resource.Tracking.Born_Digital = Convert.ToBoolean(tempSet.Tables[0].Rows[0][3]);
                Resource.Web.Siblings = Convert.ToInt32(tempSet.Tables[0].Rows[0][4]) - 1;
                Resource.Behaviors.Dark_Flag = Convert.ToBoolean(tempSet.Tables[0].Rows[0]["Dark"]);

                // Add the aggregation codes
                Resource.Behaviors.Clear_Aggregations();
                foreach (DataRow thisRow in tempSet.Tables[1].Rows)
                {
                    string code = thisRow[0].ToString();
                    string name = thisRow[1].ToString();
                    string type = thisRow[2].ToString();

                    Resource.Behaviors.Add_Aggregation(code, name, type);
                }

                // Add the icons
                Resource.Behaviors.Clear_Wordmarks();
                foreach (DataRow iconRow in tempSet.Tables[2].Rows)
                {
                    string image = iconRow[0].ToString();
                    string link = iconRow[1].ToString().Replace("&", "&amp;").Replace("\"", "&quot;");
                    string code = iconRow[2].ToString();
                    string name = iconRow[3].ToString();
                    if (name.Length == 0)
                        name = code.Replace("&", "&amp;").Replace("\"", "&quot;");

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
                    Resource.Behaviors.Add_Wordmark(newIcon);
                }

                // Add the web skins
                Resource.Behaviors.Clear_Web_Skins();
                foreach (DataRow skinRow in tempSet.Tables[3].Rows)
                {
                    Resource.Behaviors.Add_Web_Skin(skinRow[0].ToString().ToUpper());
                }

                // Return the first table from the returned dataset
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Methods to save a single digital resource to SobekCM

        /// <summary> Save a brand new bibliographic item to the SobekCM database </summary>
        /// <param name="ThisPackage"> New bibliographic package to save to the SobekCM database </param>
        /// <param name="OnlineSubmit"> Flag indicates if this was submitted via the online interface or some other means </param>
        /// <param name="TextFlag"> Flag indicates if there are text files asscociated with this item, which would case the text searchable flag to be set to true </param>
        /// <param name="Userid"> User id of the user that submitted this item, to associate with the user's submitted folder </param>
        /// <param name="Username"> Name of the user that submitted this item, for the worklog history </param>
        /// <param name="Usernotes"> Any user notes entered while sumitting this item </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This saves the item information, the serial hierarchy, the behaviors, tracking information, and links 
        /// to the user and the user's folders, all in a single procedure call </remarks>
        public static bool Save_New_Digital_Resource(SobekCM_Item ThisPackage, bool TextFlag, bool OnlineSubmit, string Username, string Usernotes, int Userid)
        {
            // Save the group information ( group, interfaces, links to collections ) for this item
            ThisPackage.Web.GroupID = Save_Item_Group_Information(ThisPackage, DateTime.Now);

            // Get the pub date and year
            string pubdate = ThisPackage.Bib_Info.Origin_Info.Date_Check_All_Fields;
            int year = -1;
            if (pubdate.Length > 0)
            {
                // Try to get the year
                if (pubdate.Length == 4)
                {
                    Int32.TryParse(pubdate, out year);
                }

                if (year == -1)
                {
                    DateTime date;
                    if (DateTime.TryParse(pubdate, out date))
                    {
                        year = date.Year;
                    }
                }
            }

            // Get the spatial display and subjects information
            StringBuilder spatialDisplayBuilder = new StringBuilder();
            StringBuilder subjectsDisplayBuilder = new StringBuilder();
            foreach (Subject_Info subject in ThisPackage.Bib_Info.Subjects)
            {
                if (subject.Class_Type == Subject_Info_Type.Hierarchical_Spatial)
                {
                }

                if (subject.Class_Type == Subject_Info_Type.Standard)
                {
                    Subject_Info_Standard standardSubject = (Subject_Info_Standard) subject;
                    string subjectText = standardSubject.ToString(false);
                    if (subjectsDisplayBuilder.Length > 0)
                        subjectsDisplayBuilder.Append("|");
                    subjectsDisplayBuilder.Append(subjectText);
                }
            }

            // Get the publishers
            StringBuilder publisher_builder = new StringBuilder();
            foreach (Publisher_Info thisPublisher in ThisPackage.Bib_Info.Publishers)
            {
                if (publisher_builder.Length > 0)
                {
                    publisher_builder.Append("|" + thisPublisher);
                }
                else
                {
                    publisher_builder.Append(thisPublisher);
                }
            }

            // Get the authors
            StringBuilder author_builder = new StringBuilder();
            string mainAuthor = String.Empty;
            if (ThisPackage.Bib_Info.hasMainEntityName)
                mainAuthor = ThisPackage.Bib_Info.Main_Entity_Name.ToString();
            if ((mainAuthor.Length > 0) && (mainAuthor.IndexOf("unknown") < 0))
            {
                author_builder.Append(mainAuthor);
            }
            if (ThisPackage.Bib_Info.Names_Count > 0)
            {
                foreach (Name_Info thisAuthor in ThisPackage.Bib_Info.Names)
                {
                    string thisAuthorString = thisAuthor.ToString();
                    if ((thisAuthorString.Length > 0) && (thisAuthorString.IndexOf("unknown") < 0))
                    {
                        if (author_builder.Length > 0)
                        {
                            author_builder.Append("|" + thisAuthorString);
                        }
                        else
                        {
                            author_builder.Append(thisAuthorString);
                        }
                    }
                }
            }

            // Get the donor
            string donor = String.Empty;
            if (ThisPackage.Bib_Info.Donor != null)
            {
                string donor_temp = ThisPackage.Bib_Info.Donor.ToString();
                if ((donor_temp.Length > 0) && (donor_temp.IndexOf("unknown") < 0))
                    donor = donor_temp;
            }

            // For now, set VRA core extension data to empty.  LATER THIS WILL
            // BE REMOVED FROM THIS STORED PROCEDURE
            StringBuilder materialDisplayBuilder = new StringBuilder();
            string measurements = String.Empty;
            StringBuilder stylePeriodDisplayBuilder = new StringBuilder();
            StringBuilder techniqueDisplayBuilder = new StringBuilder();


            // Pull out the spatial strings (for testing)
            string spatial_kml = String.Empty;
            const double SPATIAL_DISTANCE = -1;
            //if (thisPackage.Bib_Info.hasCoordinateInformation)
            //{
            //    spatial_kml = thisPackage.Bib_Info.Coordinates.SobekCM_Main_Spatial_String;
            //    spatial_distance = thisPackage.Bib_Info.Coordinates.SobekCM_Main_Spatial_Distance;
            //}

            // Get the source and holding codes and the institution display information
            StringBuilder institutionDisplayBuilder = new StringBuilder();
            string source_code = ThisPackage.Bib_Info.Source.Code;
            string holding_code = String.Empty;
            if ((source_code.Length > 0) && (source_code[0] != 'i') && (source_code[0] != 'I'))
            {
                source_code = "i" + source_code;
            }
            if ((source_code.Length > 2) && (source_code.ToUpper().IndexOf("II") == 0))
                source_code = source_code.Substring(1);
            if ((source_code.ToUpper().IndexOf("UF") != 0) && (source_code.ToUpper().IndexOf("IUF") != 0))
            {
                if (ThisPackage.Bib_Info.Source.Statement.Length > 0)
                    institutionDisplayBuilder.Append(ThisPackage.Bib_Info.Source.Statement);
            }

            if (ThisPackage.Bib_Info.hasLocationInformation)
            {
                holding_code = ThisPackage.Bib_Info.Location.Holding_Code;
                if ((holding_code.ToUpper().IndexOf("UF") != 0) && (holding_code.ToUpper().IndexOf("IUF") != 0))
                {
                    if (institutionDisplayBuilder.Length == 0)
                    {
                        institutionDisplayBuilder.Append(ThisPackage.Bib_Info.Location.Holding_Name);
                    }
                    //else
                    //{
                    //    if (thisPackage.Bib_Info.Location.Holding_Name.IndexOf(thisPackage.Bib_Info.Source.Statement) < 0)
                    //    {
                    //        institutionDisplayBuilder.Append("|" + thisPackage.Bib_Info.Location.Holding_Name);
                    //    }
                    //}
                }
            }
            if ((holding_code.Length > 0) && (holding_code[0] != 'i') && (holding_code[0] != 'I'))
            {
                holding_code = "i" + holding_code;
            }
            if ((holding_code.Length > 2) && (holding_code.ToUpper().IndexOf("II") == 0))
                holding_code = holding_code.Substring(1);

            // Determine the link
            string link = String.Empty;
            if ((!ThisPackage.Divisions.Physical_Tree.Has_Files) && (!ThisPackage.Divisions.Download_Tree.Has_Files) && (ThisPackage.Bib_Info.Location.Other_URL.Length > 0))
            {
                link = ThisPackage.Bib_Info.Location.Other_URL;
            }

            // Pull some data out (which may or may not exist)
            string icon1_name = String.Empty;
            string icon2_name = String.Empty;
            string icon3_name = String.Empty;
            string icon4_name = String.Empty;
            string icon5_name = String.Empty;

            // Add the icon names, if they exist
            ThisPackage.Behaviors.Dedupe_Wordmarks();
            if (ThisPackage.Behaviors.Wordmarks.Count > 0)
                icon1_name = ThisPackage.Behaviors.Wordmarks[0].Code;
            if (ThisPackage.Behaviors.Wordmarks.Count > 1)
                icon2_name = ThisPackage.Behaviors.Wordmarks[1].Code;
            if (ThisPackage.Behaviors.Wordmarks.Count > 2)
                icon3_name = ThisPackage.Behaviors.Wordmarks[2].Code;
            if (ThisPackage.Behaviors.Wordmarks.Count > 3)
                icon4_name = ThisPackage.Behaviors.Wordmarks[3].Code;
            if (ThisPackage.Behaviors.Wordmarks.Count > 4)
                icon5_name = ThisPackage.Behaviors.Wordmarks[4].Code;

            // Get the list of aggregation codes
            List<string> aggregationCodes = ThisPackage.Behaviors.Aggregations.Select(Aggregation => Aggregation.Code).ToList();

            // Ensure there are at least seven here
            while (aggregationCodes.Count < 8)
                aggregationCodes.Add(String.Empty);

            //// Collect the behavior information
            //List<int> view_type_ids = new List<int>();
            //List<string> view_labels = new List<string>();
            //List<string> view_attributes = new List<string>();
            //foreach (View_Object thisView in ThisPackage.Behaviors.Item_Level_Page_Views)
            //{
            //    switch (thisView.View_Type)
            //    {
            //        case View_Enum.JPEG:
            //            if (!view_type_ids.Contains(1))
            //            {
            //                view_type_ids.Add(1);
            //                view_labels.Add(String.Empty);
            //                view_attributes.Add(String.Empty);
            //            }
            //            break;

            //        case View_Enum.JPEG_TEXT_TWO_UP:
            //            if (!view_type_ids.Contains(14))
            //            {
            //                view_type_ids.Add(14);
            //                view_labels.Add(String.Empty);
            //                view_attributes.Add(String.Empty);
            //            }
            //            break;

            //        case View_Enum.JPEG2000:
            //            if (!view_type_ids.Contains(2))
            //            {
            //                view_type_ids.Add(2);
            //                view_labels.Add(String.Empty);
            //                view_attributes.Add(String.Empty);
            //            }
            //            break;

            //        case View_Enum.TEXT:
            //            if (!view_type_ids.Contains(3))
            //            {
            //                view_type_ids.Add(3);
            //                view_labels.Add(String.Empty);
            //                view_attributes.Add(String.Empty);
            //            }
            //            break;
            //    }
            //}

            //foreach (View_Object thisView in ThisPackage.Behaviors.Views)
            //{
            //    switch (thisView.View_Type)
            //    {
            //        case View_Enum.DATASET_CODEBOOK:
            //            if (!view_type_ids.Contains(11))
            //            {
            //                view_type_ids.Add(11);
            //                view_labels.Add(String.Empty);
            //                view_attributes.Add(String.Empty);
            //            }
            //            break;

            //        case View_Enum.DATASET_REPORTS:
            //            if (!view_type_ids.Contains(12))
            //            {
            //                view_type_ids.Add(12);
            //                view_labels.Add(String.Empty);
            //                view_attributes.Add(String.Empty);
            //            }
            //            break;

            //        case View_Enum.DATASET_VIEWDATA:
            //            if (!view_type_ids.Contains(13))
            //            {
            //                view_type_ids.Add(13);
            //                view_labels.Add(String.Empty);
            //                view_attributes.Add(String.Empty);
            //            }
            //            break;

            //        case View_Enum.GOOGLE_MAP:
            //            if (!view_type_ids.Contains(5))
            //            {
            //                view_type_ids.Add(5);
            //                view_labels.Add(String.Empty);
            //                view_attributes.Add(String.Empty);
            //            }
            //            break;

            //        case View_Enum.GOOGLE_MAP_BETA:
            //            if (!view_type_ids.Contains(5))
            //            {
            //                view_type_ids.Add(5);
            //                view_labels.Add(String.Empty);
            //                view_attributes.Add(String.Empty);
            //            }
            //            break;

            //        case View_Enum.HTML:
            //            view_type_ids.Add(6);
            //            view_labels.Add(thisView.Label);
            //            view_attributes.Add(thisView.Attributes);
            //            break;

            //        case View_Enum.JPEG:
            //            if (!view_type_ids.Contains(1))
            //            {
            //                view_type_ids.Add(1);
            //                view_labels.Add(String.Empty);
            //                view_attributes.Add(String.Empty);
            //            }
            //            break;

            //        case View_Enum.JPEG_TEXT_TWO_UP:
            //            if (!view_type_ids.Contains(14))
            //            {
            //                view_type_ids.Add(14);
            //                view_labels.Add(String.Empty);
            //                view_attributes.Add(String.Empty);
            //            }
            //            break;

            //        case View_Enum.JPEG2000:
            //            if (!view_type_ids.Contains(2))
            //            {
            //                view_type_ids.Add(2);
            //                view_labels.Add(String.Empty);
            //                view_attributes.Add(String.Empty);
            //            }
            //            break;

            //        case View_Enum.PAGE_TURNER:
            //            if (!view_type_ids.Contains(4))
            //            {
            //                view_type_ids.Add(4);
            //                view_labels.Add(String.Empty);
            //                view_attributes.Add(String.Empty);
            //            }
            //            break;

            //        case View_Enum.RELATED_IMAGES:
            //            view_type_ids.Add(8);
            //            view_labels.Add(String.Empty);
            //            view_attributes.Add(String.Empty);
            //            break;

            //        case View_Enum.TEI:
            //            view_type_ids.Add(10);
            //            view_labels.Add(String.Empty);
            //            view_attributes.Add(String.Empty);
            //            break;

            //        case View_Enum.TEXT:
            //            if (!view_type_ids.Contains(3))
            //            {
            //                view_type_ids.Add(3);
            //                view_labels.Add(String.Empty);
            //                view_attributes.Add(String.Empty);
            //            }
            //            break;

            //        case View_Enum.TOC:
            //            view_type_ids.Add(9);
            //            view_labels.Add(String.Empty);
            //            view_attributes.Add(String.Empty);
            //            break;
            //    }
            //}

            //while (view_type_ids.Count < 6)
            //{
            //    view_type_ids.Add(0);
            //    view_labels.Add(String.Empty);
            //    view_attributes.Add(String.Empty);
            //}

            string level1_text = String.Empty;
            string level2_text = String.Empty;
            string level3_text = String.Empty;
            string level4_text = String.Empty;
            string level5_text = String.Empty;
            int level1_index = -1;
            int level2_index = -1;
            int level3_index = -1;
            int level4_index = -1;
            int level5_index = -1;

            if (ThisPackage.Behaviors.hasSerialInformation)
            {
                if (ThisPackage.Behaviors.Serial_Info.Count > 0)
                {
                    Serial_Info.Single_Serial_Hierarchy level1 = ThisPackage.Behaviors.Serial_Info[0];
                    level1_index = level1.Order;
                    level1_text = level1.Display;
                }

                if (ThisPackage.Behaviors.Serial_Info.Count > 1)
                {
                    Serial_Info.Single_Serial_Hierarchy level2 = ThisPackage.Behaviors.Serial_Info[1];
                    level2_index = level2.Order;
                    level2_text = level2.Display;
                }

                if (ThisPackage.Behaviors.Serial_Info.Count > 2)
                {
                    Serial_Info.Single_Serial_Hierarchy level3 = ThisPackage.Behaviors.Serial_Info[2];
                    level3_index = level3.Order;
                    level3_text = level3.Display;
                }

                if (ThisPackage.Behaviors.Serial_Info.Count > 3)
                {
                    Serial_Info.Single_Serial_Hierarchy level4 = ThisPackage.Behaviors.Serial_Info[3];
                    level4_index = level4.Order;
                    level4_text = level4.Display;
                }

                if (ThisPackage.Behaviors.Serial_Info.Count > 4)
                {
                    Serial_Info.Single_Serial_Hierarchy level5 = ThisPackage.Behaviors.Serial_Info[4];
                    level5_index = level5.Order;
                    level5_text = level5.Display;
                }
            }


            try
            {
                int i = 0;
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[71];
                param_list[i++] = new EalDbParameter("@GroupID", ThisPackage.Web.GroupID);
                param_list[i++] = new EalDbParameter("@VID", ThisPackage.VID);
                param_list[i++] = new EalDbParameter("@PageCount", ThisPackage.Divisions.Page_Count);
                param_list[i++] = new EalDbParameter("@FileCount", ThisPackage.Divisions.Files.Count);
                param_list[i++] = new EalDbParameter("@Title", ThisPackage.Bib_Info.Main_Title.NonSort + ThisPackage.Bib_Info.Main_Title.Title);
                param_list[i++] = new EalDbParameter("@SortTitle", ThisPackage.Bib_Info.SortSafeTitle(ThisPackage.Bib_Info.Main_Title.Title, false));
                param_list[i++] = new EalDbParameter("@AccessMethod", 1);
                param_list[i++] = new EalDbParameter("@Link", link);
                param_list[i++] = new EalDbParameter("@CreateDate", DateTime.Now);
                param_list[i++] = new EalDbParameter("@PubDate", pubdate);
                param_list[i++] = new EalDbParameter("@SortDate", ThisPackage.Bib_Info.SortSafeDate(pubdate));
                param_list[i++] = new EalDbParameter("@Author", author_builder.ToString());
                param_list[i++] = new EalDbParameter("@Spatial_KML", spatial_kml);
                param_list[i++] = new EalDbParameter("@Spatial_KML_Distance", SPATIAL_DISTANCE);
                param_list[i++] = new EalDbParameter("@DiskSize_KB", ThisPackage.DiskSize_KB);
                param_list[i++] = new EalDbParameter("@Spatial_Display", spatialDisplayBuilder.ToString());
                param_list[i++] = new EalDbParameter("@Institution_Display", institutionDisplayBuilder.ToString());
                param_list[i++] = new EalDbParameter("@Edition_Display", ThisPackage.Bib_Info.Origin_Info.Edition);
                param_list[i++] = new EalDbParameter("@Material_Display", materialDisplayBuilder.ToString());
                param_list[i++] = new EalDbParameter("@Measurement_Display", measurements);
                param_list[i++] = new EalDbParameter("@StylePeriod_Display", stylePeriodDisplayBuilder.ToString());
                param_list[i++] = new EalDbParameter("@Technique_Display", techniqueDisplayBuilder.ToString());
                param_list[i++] = new EalDbParameter("@Subjects_Display", subjectsDisplayBuilder.ToString());
                param_list[i++] = new EalDbParameter("@Donor", donor);
                param_list[i++] = new EalDbParameter("@Publisher", publisher_builder.ToString());
                param_list[i++] = new EalDbParameter("@TextSearchable", TextFlag);
                param_list[i++] = new EalDbParameter("@MainThumbnail", ThisPackage.Behaviors.Main_Thumbnail);
                param_list[i++] = new EalDbParameter("@MainJPEG", ThisPackage.Behaviors.Main_Thumbnail.Replace("thm", ""));
                param_list[i++] = new EalDbParameter("@IP_Restriction_Mask", ThisPackage.Behaviors.IP_Restriction_Membership);
                param_list[i++] = new EalDbParameter("@CheckoutRequired", ThisPackage.Behaviors.CheckOut_Required);
                param_list[i++] = new EalDbParameter("@AggregationCode1", aggregationCodes[0]);
                param_list[i++] = new EalDbParameter("@AggregationCode2", aggregationCodes[1]);
                param_list[i++] = new EalDbParameter("@AggregationCode3", aggregationCodes[2]);
                param_list[i++] = new EalDbParameter("@AggregationCode4", aggregationCodes[3]);
                param_list[i++] = new EalDbParameter("@AggregationCode5", aggregationCodes[4]);
                param_list[i++] = new EalDbParameter("@AggregationCode6", aggregationCodes[5]);
                param_list[i++] = new EalDbParameter("@AggregationCode7", aggregationCodes[6]);
                param_list[i++] = new EalDbParameter("@AggregationCode8", aggregationCodes[7]);
                param_list[i++] = new EalDbParameter("@HoldingCode", holding_code);
                param_list[i++] = new EalDbParameter("@SourceCode", source_code);
                param_list[i++] = new EalDbParameter("@Icon1_Name", icon1_name);
                param_list[i++] = new EalDbParameter("@Icon2_Name", icon2_name);
                param_list[i++] = new EalDbParameter("@Icon3_Name", icon3_name);
                param_list[i++] = new EalDbParameter("@Icon4_Name", icon4_name);
                param_list[i++] = new EalDbParameter("@Icon5_Name", icon5_name);

                if (level1_index >= 0)
                {
                    param_list[i++] = new EalDbParameter("@Level1_Text", level1_text);
                    param_list[i++] = new EalDbParameter("@Level1_Index", level1_index);
                }
                else
                {
                    param_list[i++] = new EalDbParameter("@Level1_Text", DBNull.Value);
                    param_list[i++] = new EalDbParameter("@Level1_Index", DBNull.Value);
                }

                if (level2_index >= 0)
                {
                    param_list[i++] = new EalDbParameter("@Level2_Text", level2_text);
                    param_list[i++] = new EalDbParameter("@Level2_Index", level2_index);
                }
                else
                {
                    param_list[i++] = new EalDbParameter("@Level2_Text", DBNull.Value);
                    param_list[i++] = new EalDbParameter("@Level2_Index", DBNull.Value);
                }

                if (level3_index >= 0)
                {
                    param_list[i++] = new EalDbParameter("@Level3_Text", level3_text);
                    param_list[i++] = new EalDbParameter("@Level3_Index", level3_index);
                }
                else
                {
                    param_list[i++] = new EalDbParameter("@Level3_Text", DBNull.Value);
                    param_list[i++] = new EalDbParameter("@Level3_Index", DBNull.Value);
                }

                if (level4_index >= 0)
                {
                    param_list[i++] = new EalDbParameter("@Level4_Text", level4_text);
                    param_list[i++] = new EalDbParameter("@Level4_Index", level4_index);
                }
                else
                {
                    param_list[i++] = new EalDbParameter("@Level4_Text", DBNull.Value);
                    param_list[i++] = new EalDbParameter("@Level4_Index", DBNull.Value);
                }

                if (level5_index >= 0)
                {
                    param_list[i++] = new EalDbParameter("@Level5_Text", level5_text);
                    param_list[i++] = new EalDbParameter("@Level5_Index", level5_index);
                }
                else
                {
                    param_list[i++] = new EalDbParameter("@Level5_Text", DBNull.Value);
                    param_list[i++] = new EalDbParameter("@Level5_Index", DBNull.Value);
                }

                param_list[i++] = new EalDbParameter("@VIDSource", ThisPackage.Tracking.VID_Source);
                param_list[i++] = new EalDbParameter("@CopyrightIndicator", 0);
                param_list[i++] = new EalDbParameter("@Born_Digital", ThisPackage.Tracking.Born_Digital);
                param_list[i++] = new EalDbParameter("@Dark", ThisPackage.Behaviors.Dark_Flag);
                if (ThisPackage.Tracking.Material_Received_Date.HasValue)
                {
                    param_list[i++] = new EalDbParameter("@Material_Received_Date", ThisPackage.Tracking.Material_Received_Date.Value);
                    param_list[i++] = new EalDbParameter("@Material_Recd_Date_Estimated", ThisPackage.Tracking.Material_Rec_Date_Estimated);
                }
                else
                {
                    param_list[i++] = new EalDbParameter("@Material_Received_Date", DBNull.Value);
                    param_list[i++] = new EalDbParameter("@Material_Recd_Date_Estimated", false);
                }

                if (ThisPackage.Tracking.Disposition_Advice <= 0)
                    param_list[i++] = new EalDbParameter("@Disposition_Advice", DBNull.Value);
                else
                    param_list[i++] = new EalDbParameter("@Disposition_Advice", ThisPackage.Tracking.Disposition_Advice);

                param_list[i++] = new EalDbParameter("@Disposition_Advice_Notes", ThisPackage.Tracking.Disposition_Advice_Notes);
                param_list[i++] = new EalDbParameter("@Internal_Comments", ThisPackage.Tracking.Internal_Comments);
                param_list[i++] = new EalDbParameter("@Tracking_Box", ThisPackage.Tracking.Tracking_Box);
                param_list[i++] = new EalDbParameter("@Online_Submit", OnlineSubmit);
                param_list[i++] = new EalDbParameter("@User", Username);
                param_list[i++] = new EalDbParameter("@UserNotes", Usernotes);
                param_list[i++] = new EalDbParameter("@UserID_To_Link", Userid);

                param_list[i] = new EalDbParameter("@ItemID", -1);
                param_list[i++].Direction = ParameterDirection.InputOutput;
                param_list[i] = new EalDbParameter("@New_VID", "00000") {Direction = ParameterDirection.InputOutput};

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Save_New_Item", param_list);

                // Save the item id and VID into the package
                ThisPackage.Web.ItemID = (int) param_list[69].Value;
                ThisPackage.VID = param_list[70].Value.ToString();
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Save_Item", ee);
                return false;
            }

            // If the material reced date is filled in and there are notes about the receipt,
            // add a workflow for this 
            if ((ThisPackage.Tracking.Material_Received_Date.HasValue) && (ThisPackage.Tracking.Material_Received_Notes.Length > 0))
            {
                Update_Material_Received(ThisPackage.Web.ItemID, ThisPackage.Tracking.Material_Received_Date.Value, false, Username, ThisPackage.Tracking.Material_Received_Notes);
            }

            // Save any additional metadat present in the item
            Save_Item_Metadata_Information(ThisPackage);

            // Step through all the metadata modules and allow the modules to save to the database
            if (ThisPackage.Metadata_Modules != null)
            {
                foreach (iMetadata_Module thisModule in ThisPackage.Metadata_Modules)
                {
                    string error_message;
                    thisModule.Save_Additional_Info_To_Database(ThisPackage.Web.ItemID, connectionString, ThisPackage, out error_message);
                }
            }

            //for each page
            List<abstract_TreeNode> pages = ThisPackage.Divisions.Physical_Tree.Pages_PreOrder;
            foreach (abstract_TreeNode t in pages)
            {
                //Step through all the metadata modules and allow the modules to save to the database
                if (t.Metadata_Modules != null)
                {
                    foreach (iMetadata_Module thisModule in t.Metadata_Modules)
                    {
                        string error_message;
                        thisModule.Save_Additional_Info_To_Database(ThisPackage.Web.ItemID, connectionString, ThisPackage, out error_message);
                    }
                }
            }

            // Finally, have the database build the full citation based on each metadata element
            Create_Full_Citation_Value(ThisPackage.Web.ItemID);

            return true;

        }

        /// <summary> Saves this bibliographic package to the SobekCM database </summary>
        /// <param name="ThisPackage"> Bibliographic package to save to the SobekCM database </param>
        /// <param name="Options"> Options used possibly during the saving process </param>
        /// <returns> Item ID for this... (may need it for something in the future?) </returns>
        public static int Save_Digital_Resource(SobekCM_Item ThisPackage, Dictionary<string, object> Options)
        {
            return Save_Digital_Resource(ThisPackage, Options, DateTime.Now, false);
        }

        /// <summary> Saves this bibliographic package to the SobekCM database </summary>
        /// <param name="ThisPackage"> Bibliographic package to save to the SobekCM database </param>
        /// <param name="Options"> Options used possibly during the saving process </param>
        /// <param name="CreateDate"> Date this item was originally created </param>
        /// <param name="Existed"> Flag indicates if this item pre-existed </param>
        /// <returns> Item ID for this... (may need it for something in the future?) </returns>
        public static int Save_Digital_Resource(SobekCM_Item ThisPackage, Dictionary<string, object> Options, DateTime CreateDate, bool Existed)
        {
            // Save the group information ( group, interfaces, links to collections ) for this item
            ThisPackage.Web.GroupID = Save_Item_Group_Information(ThisPackage, CreateDate);

            // Save the actual item information ( item, downloads, icons, link to group ) for this item
            Save_Item_Information(ThisPackage, ThisPackage.Web.GroupID, CreateDate, Options);

            // Save the serial hierarchy information, if there was one
            if ((ThisPackage.Behaviors.hasSerialInformation) && (ThisPackage.Behaviors.Serial_Info.Count > 0))
            {
                Save_Serial_Hierarchy_Information(ThisPackage, ThisPackage.Web.GroupID, ThisPackage.Web.ItemID);
            }

            // Save any additional metadata present in the item
            Save_Item_Metadata_Information(ThisPackage);

            // Step through all the metadata modules and allow the modules to save to the database
            if (ThisPackage.Metadata_Modules != null)
            {
                foreach (iMetadata_Module thisModule in ThisPackage.Metadata_Modules)
                {
                    string error_message;
                    thisModule.Save_Additional_Info_To_Database(ThisPackage.Web.ItemID, connectionString, ThisPackage, out error_message);
                }
            }

            //for each page
            List<abstract_TreeNode> pages = ThisPackage.Divisions.Physical_Tree.Pages_PreOrder;
            foreach (abstract_TreeNode t in pages)
            {
                //GeoSpatial_Information geoInfo = pages[i].Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                //string error_message;
                //Save_Item_Metadata(geoInfo);
                //Save_Item(geoInfo);
                //Save_Item_Information(geoInfo);
                //Save_Item_Metadata_Information(geoInfo);
                //geoInfo.Save_Additional_Info_To_Database(ThisPackage.Web.ItemID, connectionString, ThisPackage, out error_message);

                //Step through all the metadata modules and allow the modules to save to the database
                if (t.Metadata_Modules != null)
                {
                    foreach (iMetadata_Module thisModule in t.Metadata_Modules)
                    {
                        string error_message;
                        thisModule.Save_Additional_Info_To_Database(ThisPackage.Web.ItemID, connectionString, ThisPackage, out error_message);
                    }
                }
            }

            // Finally, have the database build the full citation based on each metadata element
            Create_Full_Citation_Value(ThisPackage.Web.ItemID);

            // Return the item id
            return ThisPackage.Web.ItemID;
        }

        private static void Save_Streets_and_Features_To_Item(SobekCM_Item ThisPackage, int ItemID)
        {
            //// Clear any existing links
            //if (ItemID > 0)
            //{
            //    Clear_Features_Streets_By_Item(ItemID);

            //    // Populate the page id's for the sheets
            //    DataTable pages = null;

            //    // Find the matches, by sequence
            //    Hashtable index_from_id = new Hashtable();
            //    List<abstract_TreeNode> nodes = thisPackage.Divisions.Physical_Tree.Divisions_PreOrder;
            //    int page_sequence = 1;
            //    foreach (abstract_TreeNode node in nodes)
            //    {
            //        if (node.Page)
            //        {
            //            string node_id = node.Node_ID;
            //            if ( node_id.IndexOf("_") > 0 )
            //                node_id = node_id.Substring( 0, node_id.IndexOf("_"));
            //            if (!index_from_id.Contains(node_id))
            //            {
            //                index_from_id.Add(node_id, page_sequence);
            //                page_sequence++;
            //            }
            //        }
            //    }

            //    // Assign the page ids to the map sheets
            //    Maps.Map_Sheet[] allSheets = thisPackage.Map.All_Sheets;
            //    foreach (Maps.Map_Sheet thisSheet in allSheets)
            //    {
            //        if (index_from_id.Contains(thisSheet.FilePtr))
            //        {
            //            DataRow[] page_rows = pages.Select("PageSequence = " + index_from_id[thisSheet.FilePtr]);
            //            if (page_rows.Length > 0)
            //            {
            //                thisSheet.Database_Sheet_ID = Convert.ToInt32(page_rows[0]["PageID"]);
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    Maps.Map_Sheet[] allSheets = thisPackage.Map.All_Sheets;
            //    foreach (Maps.Map_Sheet thisSheet in allSheets)
            //    {
            //        thisSheet.Database_Sheet_ID = -1;
            //    }
            //}

            //// Save all the corporations
            //Maps.Map_Corporation[] corps = thisPackage.Map.All_Corporations;
            //foreach (Maps.Map_Corporation corporation in corps)
            //{
            //    Save_Corporation("CORP" + corporation.CorpID, corporation.Primary_Name);
            //}

            //// Step through each feature
            //string corp_code;
            //int[] feature_pages = new int[5] { -1, -1, -1, -1, -1 };
            //int page_index = 0;
            //foreach (Maps.Map_Info_Tables.FeatureRow thisFeature in thisPackage.Map.Features)
            //{
            //    // Get the corporation code
            //    if (thisFeature.GetCorporation_LinkRows().Length > 0)
            //    {
            //        corp_code = "CORP" + thisFeature.GetCorporation_LinkRows()[0].CorpID;
            //    }
            //    else
            //    {
            //        corp_code = String.Empty;
            //    }

            //    // Clear page references
            //    page_index = 0;
            //    for (int i = 0; i < 5; i++)
            //        feature_pages[i] = -1;

            //    // Get page links
            //    if (thisFeature.GetSheet_LinkRows().Length > 0)
            //    {
            //        Maps.Map_Info_Tables.Sheet_LinkRow[] links = thisFeature.GetSheet_LinkRows();
            //        foreach (Maps.Map_Info_Tables.Sheet_LinkRow linker in links)
            //        {
            //            long sheet_id = linker.SheetID;
            //            Maps.Map_Sheet sheet = thisPackage.Map.Get_Sheet(sheet_id);
            //            if (sheet != null)
            //            {
            //                feature_pages[page_index++] = sheet.Database_Sheet_ID;
            //            }

            //            if (page_index == 5)
            //                break;
            //        }
            //    }

            //    // Add this feature
            //    Save_Feature( "FEAT" + thisFeature.FeatureID, thisFeature.Name, thisFeature.Description, corp_code, false, String.Empty, String.Empty, String.Empty, String.Empty, thisFeature.Type, -1, feature_pages[0], feature_pages[1], feature_pages[2], feature_pages[3], feature_pages[4]);
            //}

            //// Step through each sheet
            //int pageid = -1;
            //foreach (Maps.Map_Info_Tables.StreetRow thisStreet in thisPackage.Map.Streets)
            //{
            //    // Save the page id
            //    pageid = -1;
            //    Maps.Map_Sheet sheet = thisPackage.Map.Get_Sheet(thisStreet.SheetID);
            //    if (sheet != null)
            //        pageid = sheet.Database_Sheet_ID;

            //    // Save this street
            //    Save_Street("STRE" + thisStreet.StreetID, thisStreet.Name, thisStreet.Start, thisStreet.End, thisStreet.Direction, thisStreet.Side, thisStreet.Description, pageid);
            //}
        }

        private static void Save_GeoRegion_Links(SobekCM_Item ThisPackage, int ItemID)
        {
            //// Clear any existing links
            //if (ItemID > 0)
            //{
            //    Clear_Region_Link_By_Item(ItemID);
            //}

            //// Are there spatial links here?
            //if (thisPackage.Bib_Info.Hierarchical_Spatials.Count == 0)
            //    return;

            //// Is there the geocore data pulled?
            //if (geo_core == null)
            //{
            //    geo_core = SobekCM_Database.Get_All_Regions();
            //}

            //// Find the last geo code
            //long last_geo_code = 1;
            //if (geo_core.Rows.Count > 0)
            //{
            //    DataRow last_row = geo_core.Rows[geo_core.Rows.Count - 1];
            //    last_geo_code = Convert.ToInt32(last_row["GeoAuthCode"].ToString().Replace("GEO", ""));
            //}


            //// Now, add each hierarchical spatial
            //string[] code = new string[6] { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty };
            //string[] name = new string[6] { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty };
            //string[] type = new string[6] { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty };
            //int level_index = 0;
            //DataRow[] select_rows;
            //DataRow newRow;
            //foreach (SobekCM.Resource_Object.Bib_Info.Hierarchical_Spatial_Info spatial in thisPackage.Bib_Info.Hierarchical_Spatials)
            //{
            //    // Clear the variables
            //    level_index = 0;
            //    for (int i = 0; i < 6; i++)
            //    {
            //        code[i] = String.Empty;
            //        name[i] = String.Empty;
            //        type[i] = String.Empty;
            //    }             

            //    // Load the city
            //    if (spatial.City.Length > 0)
            //    {
            //        name[level_index] = spatial.City;
            //        type[level_index] = "City";

            //        // Try to find the code for this
            //        select_rows = geo_core.Select("RegionName = '" + name[level_index] + "' and RegionTypeName = '" + type[level_index] + "'");
            //        if (select_rows.Length > 0)
            //        {
            //            code[level_index] = select_rows[0]["GeoAuthCode"].ToString();
            //        }
            //        else
            //        {
            //            // Find the last geo code
            //            last_geo_code++;
            //            code[level_index] = "GEO" + (last_geo_code.ToString()).PadLeft(6, '0');
            //            newRow = geo_core.NewRow();
            //            newRow["GeoAuthCode"] = code[level_index];
            //            newRow["RegionName"] = name[level_index];
            //            newRow["RegionTypeName"] = type[level_index];
            //            geo_core.Rows.Add(newRow);
            //        }
            //        level_index++;
            //    }

            //    // Load the county
            //    if (spatial.County.Length > 0)
            //    {
            //        name[level_index] = spatial.County;
            //        type[level_index] = "County";

            //        // Try to find the code for this
            //        select_rows = geo_core.Select("RegionName = '" + name[level_index] + "' and RegionTypeName = '" + type[level_index] + "'");
            //        if (select_rows.Length > 0)
            //        {
            //            code[level_index] = select_rows[0]["GeoAuthCode"].ToString();
            //        }
            //        else
            //        {
            //            // Find the last geo code
            //            last_geo_code++;
            //            code[level_index] = "GEO" + (last_geo_code.ToString()).PadLeft(6, '0');
            //            newRow = geo_core.NewRow();
            //            newRow["GeoAuthCode"] = code[level_index];
            //            newRow["RegionName"] = name[level_index];
            //            newRow["RegionTypeName"] = type[level_index];
            //            geo_core.Rows.Add(newRow);
            //        }

            //        level_index++;
            //    }

            //    // Load the state
            //    if (spatial.State.Length > 0)
            //    {
            //        name[level_index] = spatial.State;
            //        type[level_index] = "State";

            //        // Try to find the code for this
            //        select_rows = geo_core.Select("RegionName = '" + name[level_index] + "' and RegionTypeName = '" + type[level_index] + "'");
            //        if (select_rows.Length > 0)
            //        {
            //            code[level_index] = select_rows[0]["GeoAuthCode"].ToString();
            //        }
            //        else
            //        {
            //            // Find the last geo code
            //            last_geo_code++;
            //            code[level_index] = "GEO" + (last_geo_code.ToString()).PadLeft(6, '0');
            //            newRow = geo_core.NewRow();
            //            newRow["GeoAuthCode"] = code[level_index];
            //            newRow["RegionName"] = name[level_index];
            //            newRow["RegionTypeName"] = type[level_index];
            //            geo_core.Rows.Add(newRow);
            //        }

            //        level_index++;
            //    }

            //    // Load the country
            //    if (spatial.State.Length > 0)
            //    {
            //        name[level_index] = spatial.Country;
            //        type[level_index] = "Country";

            //        // Try to find the code for this
            //        select_rows = geo_core.Select("RegionName = '" + name[level_index] + "' and RegionTypeName = '" + type[level_index] + "'");
            //        if (select_rows.Length > 0)
            //        {
            //            code[level_index] = select_rows[0]["GeoAuthCode"].ToString();
            //        }
            //        else
            //        {
            //            // Find the last geo code
            //            last_geo_code++;
            //            code[level_index] = "GEO" + (last_geo_code.ToString()).PadLeft(6, '0');
            //            newRow = geo_core.NewRow();
            //            newRow["GeoAuthCode"] = code[level_index];
            //            newRow["RegionName"] = name[level_index];
            //            newRow["RegionTypeName"] = type[level_index];
            //            geo_core.Rows.Add(newRow);
            //        }

            //        level_index++;
            //    }

            //    // Load the continent
            //    if (spatial.State.Length > 0)
            //    {
            //        name[level_index] = spatial.Continent;
            //        type[level_index] = "Continent";

            //        // Try to find the code for this
            //        select_rows = geo_core.Select("RegionName = '" + name[level_index] + "' and RegionTypeName = '" + type[level_index] + "'");
            //        if (select_rows.Length > 0)
            //        {
            //            code[level_index] = select_rows[0]["GeoAuthCode"].ToString();
            //        }
            //        else
            //        {
            //            // Find the last geo code
            //            last_geo_code++;
            //            code[level_index] = "GEO" + (last_geo_code.ToString()).PadLeft(6, '0');
            //            newRow = geo_core.NewRow();
            //            newRow["GeoAuthCode"] = code[level_index];
            //            newRow["RegionName"] = name[level_index];
            //            newRow["RegionTypeName"] = type[level_index];
            //            geo_core.Rows.Add(newRow);
            //        }

            //        level_index++;
            //    }

            //    // Save this to the database
            //    Save_Region_Item_Link(ItemID, code[0], name[0], type[0], code[1], name[1], type[1], code[2], name[2], type[2],
            //        code[3], name[3], type[3], code[4], name[4], type[4], code[5], name[5], type[5]);
            //}

        }

        /// <summary> Get the list of all corporations from the database </summary>
        /// <returns> Table of all corpoorations linked to digital resources (specifically Sanborn Maps) </returns>
        public static DataTable Get_All_Corporations()
        {
            try
            {
                DataSet tempSet = EalDbAccess.ExecuteDataset(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Get_All_Corporations");
                return tempSet.Tables[0];
            }
            catch (Exception ee)
            {
                exception_caught("SobekCM_Get_All_Corporations", ee);
                return null;
            }
        }

        /// <summary> Get the list of all regions from the database </summary>
        /// <returns> List of all regions from the database </returns>
        public static DataTable Get_All_Regions()
        {
            try
            {
                DataSet tempSet = EalDbAccess.ExecuteDataset(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Get_All_Regions");
                return tempSet.Tables[0];
            }
            catch (Exception ee)
            {
                exception_caught("SobekCM_Get_All_Regions", ee);
                return null;
            }
        }

        private static bool Save_Corporation(string CorpAuthCode, string CorporateName)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[2];
                param_list[0] = new EalDbParameter("@CorpAuthCode", CorpAuthCode);
                param_list[1] = new EalDbParameter("@CorporateName", CorporateName);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Save_Corporation", param_list);

                // Return the value
                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Save_Corporation", ee);
                return false;
            }
        }

        private static bool Clear_Features_Streets_By_Item(int ItemID)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[1];
                param_list[0] = new EalDbParameter("@ItemID", ItemID);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Clear_Features_Streets_By_Item", param_list);

                // Return the value
                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Clear_Features_Streets_By_Item", ee);
                return false;
            }
        }

        private static bool Clear_Region_Link_By_Item(int ItemID)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[1];
                param_list[0] = new EalDbParameter("@ItemID", ItemID);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Clear_Region_Link_By_Item", param_list);

                // Return the value
                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Clear_Region_Link_By_Item", ee);
                return false;
            }
        }


        private static bool Lock_Digital_Resource(int ItemID)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[1];
                param_list[0] = new EalDbParameter("@ItemID", ItemID);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Lock_Item", param_list);

                // Return the value
                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Lock_Item", ee);
                return false;
            }
        }

        private static bool Save_Region_Item_Link(int ItemID, string GeoAuthCode, string Name, string Type, string P_Code, string P_Name, string P_Type, string P2_Code, string P2_Name, string P2_Type, string P3_Code, string P3_Name, string P3_Type, string P4_Code, string P4_Name, string P4_Type, string P5_Code, string P5_Name, string P5_Type)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[19];
                param_list[0] = new EalDbParameter("@GeoAuthCode", GeoAuthCode);
                param_list[1] = new EalDbParameter("@ItemID", ItemID);
                param_list[2] = new EalDbParameter("@RegionName", Name);
                param_list[3] = new EalDbParameter("@RegionType", Type);
                param_list[4] = new EalDbParameter("@P_RegionAuthCode", P_Code);
                param_list[5] = new EalDbParameter("@P_RegionName", P_Name);
                param_list[6] = new EalDbParameter("@P_RegionType", P_Type);
                param_list[7] = new EalDbParameter("@P2_RegionAuthCode", P2_Code);
                param_list[8] = new EalDbParameter("@P2_RegionName", P2_Name);
                param_list[9] = new EalDbParameter("@P2_RegionType", P2_Type);
                param_list[10] = new EalDbParameter("@P3_RegionAuthCode", P3_Code);
                param_list[11] = new EalDbParameter("@P3_RegionName", P3_Name);
                param_list[12] = new EalDbParameter("@P3_RegionType", P3_Type);
                param_list[13] = new EalDbParameter("@P4_RegionAuthCode", P4_Code);
                param_list[14] = new EalDbParameter("@P4_RegionName", P4_Name);
                param_list[15] = new EalDbParameter("@P4_RegionType", P4_Type);
                param_list[16] = new EalDbParameter("@P5_RegionAuthCode", P5_Code);
                param_list[17] = new EalDbParameter("@P5_RegionName", P5_Name);
                param_list[18] = new EalDbParameter("@P5_RegionType", P5_Type);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Save_Region_Item_Link", param_list);

                // Return the value
                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Save_Region_Item_Link", ee);
                return false;
            }
        }

        private static bool Save_Feature(string FeatAuthCode, string FeatureName, string LocationDesc, string CorpAuthCode, bool AA_Indicated,
            string Albers_X, string Albers_Y, string Latitude, string Longitude, string FeatureType, int FeatureTypeYear, int PageID1,
            int PageID2, int PageID3, int PageID4, int PageID5)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[16];
                param_list[0] = new EalDbParameter("@FeatAuthCode", FeatAuthCode);
                param_list[1] = new EalDbParameter("@FeatureName", FeatureName);
                param_list[2] = new EalDbParameter("@LocationDesc", LocationDesc);
                param_list[3] = new EalDbParameter("@CorpAuthCode", CorpAuthCode);
                param_list[4] = new EalDbParameter("@AA_Indicated", AA_Indicated);
                param_list[5] = new EalDbParameter("@Albers_X", Albers_X);
                param_list[6] = new EalDbParameter("@Albers_Y", Albers_Y);
                param_list[7] = new EalDbParameter("@Latitude", Latitude);
                param_list[8] = new EalDbParameter("@Longitude", Longitude);
                param_list[9] = new EalDbParameter("@FeatureType", FeatureType);
                param_list[10] = new EalDbParameter("@FeatureTypeYear", FeatureTypeYear);
                param_list[11] = new EalDbParameter("@PageID1", PageID1);
                param_list[12] = new EalDbParameter("@PageID2", PageID2);
                param_list[13] = new EalDbParameter("@PageID3", PageID3);
                param_list[14] = new EalDbParameter("@PageID4", PageID4);
                param_list[15] = new EalDbParameter("@PageID5", PageID5);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Save_Feature", param_list);

                // Return the value
                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Save_Feature", ee);
                return false;
            }
        }

        private static bool Save_Street(string StreetAuthCode, string StreetName, long StartAddress, long EndAddress,
            string StreetDirection, string StreetSide, string SegmentDesc, int PageID)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[8];
                param_list[0] = new EalDbParameter("@StreetAuthCode", StreetAuthCode);
                param_list[1] = new EalDbParameter("@StreetName", StreetName);
                param_list[2] = new EalDbParameter("@StartAddress", StartAddress);
                param_list[3] = new EalDbParameter("@EndAddress", EndAddress);
                param_list[4] = new EalDbParameter("@StreetDirection", StreetDirection);
                param_list[5] = new EalDbParameter("@StreetSide", StreetSide);
                param_list[6] = new EalDbParameter("@SegmentDesc", SegmentDesc);
                param_list[7] = new EalDbParameter("@PageID", PageID);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Save_Street_Page_Link", param_list);

                // Return the value
                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Save_Street_Page_Link", ee);
                return false;
            }
        }

        #region Private helper methods for saving an individual item

        /// <summary> Saves the item group information for this bibliographic package to the SobekCM database </summary>
        /// <param name="ThisPackage"> Bibliographic package to save to the SobekCM database </param>
        /// <param name="CreateDate"> Day this item group was created</param>
        /// <returns> Group ID for this package </returns>
        private static int Save_Item_Group_Information(SobekCM_Item ThisPackage, DateTime CreateDate)
        {
            // Pull some data out (which may or may not exist)
            string groupTitle = ThisPackage.Bib_Info.Main_Title.Title;

            // Try to determine an integer value for ALEPH number
            string aleph = ThisPackage.Bib_Info.ALEPH_Record;
            int alephNumber = -1;
            if (aleph.Length > 0)
            {
                int aleph_temp;
                if (Int32.TryParse(aleph, out aleph_temp))
                    alephNumber = Convert.ToInt32(aleph_temp);
            }

            // Try to determine a long integer value for OCLC number
            string oclc = ThisPackage.Bib_Info.OCLC_Record;
            int oclcNumber = -1;
            if (oclc.Length > 0)
            {
                int oclc_temp;
                if (Int32.TryParse(oclc, out oclc_temp))
                    oclcNumber = oclc_temp;
            }

            // Look for primary identifier
            string primary_alternate_type = ThisPackage.Behaviors.Primary_Identifier.Type;
            string primary_alternate_id = ThisPackage.Behaviors.Primary_Identifier.Identifier;
            string accession_type = String.Empty;
            string accession_id = String.Empty;
            if ((ThisPackage.Bib_Info.Identifiers_Count > 0) && (primary_alternate_id.Length == 0) || (primary_alternate_type.Length == 0))
            {
                // Get the type here
                bool artifact = ThisPackage.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Artifact;

                // Step through the identifiers
                foreach (Identifier_Info thisIdentifier in ThisPackage.Bib_Info.Identifiers)
                {
                    if (thisIdentifier.Type.IndexOf("*") >= 0)
                    {
                        primary_alternate_type = thisIdentifier.Type.Replace("*", "");
                        primary_alternate_id = thisIdentifier.Identifier;
                        break;
                    }

                    if (artifact)
                    {
                        if ((thisIdentifier.Type.ToUpper().IndexOf("ACCESSION") >= 0) || (thisIdentifier.Type.ToUpper().IndexOf("ACCN") >= 0))
                        {
                            accession_type = thisIdentifier.Type;
                            accession_id = thisIdentifier.Identifier;
                        }
                    }
                }
            }

            // If no primary was found, but an accession was found, use that
            if ((primary_alternate_id.Length == 0) || (primary_alternate_type.Length == 0))
            {
                if ((accession_id.Length > 0) && (accession_type.Length > 0))
                {
                    primary_alternate_type = accession_type;
                    primary_alternate_id = accession_id;
                }
            }

            // Save the main information, and keep the item id
            Save_Item_Group_Args saveArgs = Save_Item_Group(ThisPackage.BibID, groupTitle, ThisPackage.Bib_Info.SortSafeTitle(groupTitle, true), ThisPackage.Bib_Info.SobekCM_Type_String, ThisPackage.Web.File_Root, String.Empty, false, CreateDate, oclcNumber, alephNumber, ThisPackage.Tracking.Large_Format, ThisPackage.Tracking.Track_By_Month, ThisPackage.Tracking.Never_Overlay_Record, primary_alternate_type, primary_alternate_id);
            ThisPackage.Web.GroupID = saveArgs.GroupID;
            ThisPackage.BibID = saveArgs.New_BibID;

            // If there were web skins, save that as well
            Save_Item_Group_Web_Skins(ThisPackage.Web.GroupID, ThisPackage);

            // Now, also save any external record numbers ( OCLC, ALEPH, etc.. )
            bool aleph_or_oclc_exists = false;
            if (ThisPackage.Bib_Info.Identifiers_Count > 0)
            {
                ReadOnlyCollection<Identifier_Info> identifiers = ThisPackage.Bib_Info.Identifiers;
                foreach (Identifier_Info identifier in identifiers)
                {
                    string identifier_type_upper = identifier.Type.ToUpper();
                    bool added = false;
                    if (identifier_type_upper.IndexOf("OCLC") >= 0)
                    {
                        Add_External_Record_Number(ThisPackage.Web.GroupID, identifier.Identifier, "OCLC");
                        aleph_or_oclc_exists = true;
                        added = true;
                    }
                    if ((!added) && (identifier_type_upper.IndexOf("ALEPH") >= 0))
                    {
                        Add_External_Record_Number(ThisPackage.Web.GroupID, identifier.Identifier, "ALEPH");
                        aleph_or_oclc_exists = true;
                        added = true;
                    }
                    if ((!added) && (identifier_type_upper.IndexOf("LTUF") >= 0))
                    {
                        Add_External_Record_Number(ThisPackage.Web.GroupID, identifier.Identifier, "LTUF");
                        added = true;
                    }
                    if ((!added) && (identifier_type_upper.IndexOf("LTQF") >= 0))
                    {
                        Add_External_Record_Number(ThisPackage.Web.GroupID, identifier.Identifier, "LTQF");
                        added = true;
                    }
                    if ((!added) && (identifier_type_upper.IndexOf("LCCN") >= 0))
                    {
                        Add_External_Record_Number(ThisPackage.Web.GroupID, identifier.Identifier, "LCCN");
                        added = true;
                    }
                    if ((!added) && (identifier_type_upper.IndexOf("ISBN") >= 0))
                    {
                        Add_External_Record_Number(ThisPackage.Web.GroupID, identifier.Identifier, "ISBN");
                        added = true;
                    }
                    if ((!added) && (identifier_type_upper.IndexOf("ISSN") >= 0))
                    {
                        Add_External_Record_Number(ThisPackage.Web.GroupID, identifier.Identifier, "ISSN");
                    }
                    if ((!added) && ((identifier_type_upper.IndexOf("ACCESSION") >= 0) || (identifier_type_upper.IndexOf("ACCN") >= 0)))
                    {
                        Add_External_Record_Number(ThisPackage.Web.GroupID, identifier.Identifier, "ACCESSION");
                    }
                }
            }

            // Set the endeca suppress flag (which may not be actually used)
            if (aleph_or_oclc_exists)
                ThisPackage.Behaviors.Suppress_Endeca = false;

            return ThisPackage.Web.GroupID;

        }

        /// <summary> Adds an external record number to an item group (i.e., OCLC or ALEPH number) </summary>
        /// <param name="GroupID"> Primary key to this item group </param>
        /// <param name="Identifier"> Value for the external identifier </param>
        /// <param name="Type"> String which indicates the type of indentifier </param>
        public static void Add_External_Record_Number(int GroupID, string Identifier, string Type)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[3];
                param_list[0] = new EalDbParameter("@groupID", GroupID);
                param_list[1] = new EalDbParameter("@extRecordValue", Identifier);
                param_list[2] = new EalDbParameter("@extRecordType", Type);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Add_External_Record_Number", param_list);
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Add_External_Record_Number", ee);
            }
        }

        /// <summary> Saves information about single item </summary>
        /// <param name="ThisPackage"></param>
        /// <param name="GroupID"></param>
        /// <param name="CreateDate"> Day this item was created </param>
        /// <param name="Options"> Instance wide options related to saving this item </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        private static bool Save_Item_Information(SobekCM_Item ThisPackage, int GroupID, DateTime CreateDate, Dictionary<string, object> Options)
        {
            // Get the pub date and year
            string pubdate = ThisPackage.Bib_Info.Origin_Info.Date_Check_All_Fields;
            int year = -1;
            if (pubdate.Length > 0)
            {
                // Try to get the year
                if (pubdate.Length == 4)
                {
                    Int32.TryParse(pubdate, out year);
                }

                if (year == -1)
                {
                    DateTime date;
                    if (DateTime.TryParse(pubdate, out date))
                    {
                        year = date.Year;
                    }
                }
            }

            // Get the spatial display and subjects information
            StringBuilder spatialDisplayBuilder = new StringBuilder();
            StringBuilder subjectsDisplayBuilder = new StringBuilder();
            foreach (Subject_Info subject in ThisPackage.Bib_Info.Subjects)
            {
                if (subject.Class_Type == Subject_Info_Type.Standard)
                {
                    Subject_Info_Standard standardSubject = (Subject_Info_Standard) subject;
                    string subjectText = standardSubject.ToString(false);
                    if (subjectsDisplayBuilder.Length > 0)
                        subjectsDisplayBuilder.Append("|");
                    subjectsDisplayBuilder.Append(subjectText);
                }
            }

            // Get the publishers
            StringBuilder publisher_builder = new StringBuilder();
            foreach (Publisher_Info thisPublisher in ThisPackage.Bib_Info.Publishers)
            {
                if (publisher_builder.Length > 0)
                {
                    publisher_builder.Append("|" + thisPublisher);
                }
                else
                {
                    publisher_builder.Append(thisPublisher);
                }
            }

            // Get the authors
            StringBuilder author_builder = new StringBuilder();
            string mainAuthor = String.Empty;
            if (ThisPackage.Bib_Info.hasMainEntityName)
                mainAuthor = ThisPackage.Bib_Info.Main_Entity_Name.ToString();
            if ((mainAuthor.Length > 0) && (mainAuthor.IndexOf("unknown") < 0))
            {
                author_builder.Append(mainAuthor);
            }
            if (ThisPackage.Bib_Info.Names_Count > 0)
            {
                foreach (Name_Info thisAuthor in ThisPackage.Bib_Info.Names)
                {
                    string thisAuthorString = thisAuthor.ToString();
                    if ((thisAuthorString.Length > 0) && (thisAuthorString.IndexOf("unknown") < 0))
                    {
                        if (author_builder.Length > 0)
                        {
                            author_builder.Append("|" + thisAuthorString);
                        }
                        else
                        {
                            author_builder.Append(thisAuthorString);
                        }
                    }
                }
            }

            // Get the donor
            string donor = String.Empty;
            if (ThisPackage.Bib_Info.Donor != null)
            {
                string donor_temp = ThisPackage.Bib_Info.Donor.ToString();
                if ((donor_temp.Length > 0) && (donor_temp.IndexOf("unknown") < 0))
                    donor = donor_temp;
            }

            // Get the material display information
            StringBuilder materialDisplayBuilder = new StringBuilder();
            string measurements = String.Empty;
            StringBuilder stylePeriodDisplayBuilder = new StringBuilder();
            StringBuilder techniqueDisplayBuilder = new StringBuilder();

            // Get the source and holding codes and the institution display information
            StringBuilder institutionDisplayBuilder = new StringBuilder();
            string source_code = ThisPackage.Bib_Info.Source.Code;
            string holding_code = String.Empty;
            if ((source_code.Length > 0) && (source_code[0] != 'i') && (source_code[0] != 'I'))
            {
                source_code = "i" + source_code;
            }
            if ((source_code.Length > 2) && (source_code.ToUpper().IndexOf("II") == 0))
                source_code = source_code.Substring(1);
            if ((source_code.ToUpper().IndexOf("UF") != 0) && (source_code.ToUpper().IndexOf("IUF") != 0))
            {
                if (ThisPackage.Bib_Info.Source.Statement.Length > 0)
                    institutionDisplayBuilder.Append(ThisPackage.Bib_Info.Source.Statement);
            }

            if (ThisPackage.Bib_Info.hasLocationInformation)
            {
                holding_code = ThisPackage.Bib_Info.Location.Holding_Code;
                if ((holding_code.ToUpper().IndexOf("UF") != 0) && (holding_code.ToUpper().IndexOf("IUF") != 0))
                {
                    if (institutionDisplayBuilder.Length == 0)
                    {
                        institutionDisplayBuilder.Append(ThisPackage.Bib_Info.Location.Holding_Name);
                    }
                    //else
                    //{
                    //    if (thisPackage.Bib_Info.Location.Holding_Name.IndexOf(thisPackage.Bib_Info.Source.Statement) < 0)
                    //    {
                    //        institutionDisplayBuilder.Append("|" + thisPackage.Bib_Info.Location.Holding_Name);
                    //    }
                    //}
                }
            }
            if ((holding_code.Length > 0) && (holding_code[0] != 'i') && (holding_code[0] != 'I'))
            {
                holding_code = "i" + holding_code;
            }
            if ((holding_code.Length > 2) && (holding_code.ToUpper().IndexOf("II") == 0))
                holding_code = holding_code.Substring(1);

            // Pull out the spatial strings (for testing)
            string spatial_kml = String.Empty;
            const double SPATIAL_DISTANCE = -1;
            //if (thisPackage.Bib_Info.hasCoordinateInformation)
            //{
            //    spatial_kml = thisPackage.Bib_Info.Coordinates.SobekCM_Main_Spatial_String;
            //    spatial_distance = thisPackage.Bib_Info.Coordinates.SobekCM_Main_Spatial_Distance;
            //}

            // Determine the link
            string link = String.Empty;
            if ((!ThisPackage.Divisions.Physical_Tree.Has_Files) && (!ThisPackage.Divisions.Download_Tree.Has_Files) && (ThisPackage.Bib_Info.Location.Other_URL.Length > 0))
            {
                link = ThisPackage.Bib_Info.Location.Other_URL;
            }

            // Save the main information, and return the item id
            Save_Item_Args returnVal = Save_Item(GroupID, ThisPackage.VID, ThisPackage.Divisions.Page_Count, ThisPackage.Divisions.Files.Count,
                ThisPackage.Bib_Info.Main_Title.NonSort + ThisPackage.Bib_Info.Main_Title.Title, ThisPackage.Bib_Info.SortSafeTitle(ThisPackage.Bib_Info.Main_Title.Title, false),
                link, CreateDate, pubdate, ThisPackage.Bib_Info.SortSafeDate(pubdate), holding_code,
                source_code, author_builder.ToString(), spatial_kml, SPATIAL_DISTANCE, ThisPackage.DiskSize_KB, donor, publisher_builder.ToString(),
                spatialDisplayBuilder.ToString(), institutionDisplayBuilder.ToString(), ThisPackage.Bib_Info.Origin_Info.Edition, materialDisplayBuilder.ToString(),
                measurements, stylePeriodDisplayBuilder.ToString(), techniqueDisplayBuilder.ToString(), subjectsDisplayBuilder.ToString());

            // If this was existing, clear the old data
            if (returnVal.Existing)
            {
                Clear_Old_Item_Info(returnVal.ItemID);
            }

            // Save the item id and VID into the package
            ThisPackage.Web.ItemID = returnVal.ItemID;
            ThisPackage.VID = returnVal.New_VID;

            // Get the OAI-PMH dublin core information
            Save_OAI_Information(ThisPackage, Options);


            return true;
        }

        /// <summary> Saves the OAI-PMH information associated with an item </summary>
        /// <param name="ThisPackage"> Package to save as various OAI-PMH enabled formats </param>
        /// <param name="Options"> System options </param>
        public static void Save_OAI_Information(SobekCM_Item ThisPackage, Dictionary<string, object> Options)
        {
            List<Tuple<string, string>> oai_records = OAI_PMH_Metadata_Writers.Get_OAI_PMH_Metadata_Records(ThisPackage, Options);

            foreach (Tuple<string, string> thisRecord in oai_records)
            {
                Save_Item_OAI(ThisPackage.Web.ItemID, thisRecord.Item2, thisRecord.Item1);
            }
        }

        /// <summary> Save all the item behaviors associated with a SobekCM Digital Resource  </summary>
        /// <param name="ThisPackage"> Digital resource which needs to have its behaviors saved to the SobekCM database </param>
        /// <param name="TextFlag"> Flag indicates if this item has text files </param>
        /// <param name="Mass_Update_Mode"> Flag indicates if this is a mass-update mode, in which case all items within a single item group will be updated </param>
        /// <param name="Minimal_Mode"> Flag indicates if this is just additive with new viewers and setting the text flag</param>
        public static void Save_Behaviors(SobekCM_Item ThisPackage, bool TextFlag, bool Mass_Update_Mode, bool Minimal_Mode)
        {
            // Get the source and holding codes
            string source_code = ThisPackage.Bib_Info.Source.Code;
            string holding_code = String.Empty;
            if ((source_code.Length > 0) && (source_code[0] != 'i') && (source_code[0] != 'I'))
            {
                source_code = "i" + source_code;
            }
            if ((holding_code.Length > 0) && (holding_code[0] != 'i') && (holding_code[0] != 'I'))
            {
                holding_code = "i" + holding_code;
            }

            // Pull some data out (which may or may not exist)
            string icon1_name = String.Empty;
            string icon2_name = String.Empty;
            string icon3_name = String.Empty;
            string icon4_name = String.Empty;
            string icon5_name = String.Empty;

            // Add the icon names, if they exist
            if (ThisPackage.Behaviors.Wordmarks.Count > 0)
                icon1_name = ThisPackage.Behaviors.Wordmarks[0].Code;
            if (ThisPackage.Behaviors.Wordmarks.Count > 1)
                icon2_name = ThisPackage.Behaviors.Wordmarks[1].Code;
            if (ThisPackage.Behaviors.Wordmarks.Count > 2)
                icon3_name = ThisPackage.Behaviors.Wordmarks[2].Code;
            if (ThisPackage.Behaviors.Wordmarks.Count > 3)
                icon4_name = ThisPackage.Behaviors.Wordmarks[3].Code;
            if (ThisPackage.Behaviors.Wordmarks.Count > 4)
                icon5_name = ThisPackage.Behaviors.Wordmarks[4].Code;

            // Get the list of aggregation codes
            List<string> aggregationCodes = ThisPackage.Behaviors.Aggregations.Select(Aggregation => Aggregation.Code).ToList();

            // Ensure there are at least seven here
            while (aggregationCodes.Count < 8)
                aggregationCodes.Add(String.Empty);

            // Determine flags for restriction and dark
            bool darkFlag = ThisPackage.Behaviors.Dark_Flag;
            bool darkFlag_Null = ThisPackage.Behaviors.Dark_Flag_Is_Null;
            short ip_restrict = ThisPackage.Behaviors.IP_Restriction_Membership;
            bool ipRestrictNull = ThisPackage.Behaviors.IP_Restriction_Membership_Is_Null;

            if (Mass_Update_Mode)
            {
                // Siunce this is a mass update, no viewer analysis is really needed here.. Just collect them all to add
                // Collect the behavior information
                List<string> view_types = new List<string>();
                List<string> view_labels = new List<string>();
                List<string> view_attributes = new List<string>();

                if (ThisPackage.Behaviors.Views != null)
                {
                    foreach (View_Object thisView in ThisPackage.Behaviors.Views)
                    {
                        if (!view_types.Contains(thisView.View_Type))
                        {
                            view_types.Add(thisView.View_Type);
                            view_labels.Add(thisView.Label);
                            view_attributes.Add(thisView.Attributes);
                        }
                    }
                }

                // Fill up the views to the required six
                while (view_types.Count < 6)
                {
                    view_types.Add(String.Empty);
                    view_labels.Add(String.Empty);
                    view_attributes.Add(String.Empty);
                }

                // Update the behaviors of all items within the single group ( by groupid )
                Mass_Update_Item_Behaviors(ThisPackage.Web.GroupID, ipRestrictNull, ip_restrict,
                    ThisPackage.Behaviors.CheckOut_Required_Is_Null, ThisPackage.Behaviors.CheckOut_Required,
                    darkFlag_Null, darkFlag,
                    ThisPackage.Tracking.Born_Digital_Is_Null, ThisPackage.Tracking.Born_Digital,
                    aggregationCodes[0], aggregationCodes[1], aggregationCodes[2], aggregationCodes[3], aggregationCodes[4], aggregationCodes[5], aggregationCodes[6],
                    aggregationCodes[7], holding_code, source_code, icon1_name, icon2_name, icon3_name, icon4_name, icon5_name,
                    view_types[0], view_labels[0], view_attributes[0],
                    view_types[1], view_labels[1], view_attributes[1],
                    view_types[2], view_labels[2], view_attributes[2],
                    view_types[3], view_labels[3], view_attributes[3],
                    view_types[4], view_labels[4], view_attributes[4],
                    view_types[5], view_labels[5], view_attributes[5]);
            }
            else
            {
                // Minimal mode just adds the text flag now
                if (Minimal_Mode)
                {
                    Save_Item_Behaviors_Minimal(ThisPackage.Web.ItemID, TextFlag);
                }
                else
                {
                    // Save all the main behaviors
                    Save_Item_Behaviors(ThisPackage.Web.ItemID, TextFlag, ThisPackage.Behaviors.Main_Thumbnail,
                        ThisPackage.Behaviors.Main_Thumbnail.Replace("thm", ""), ip_restrict,
                        ThisPackage.Behaviors.CheckOut_Required, darkFlag, ThisPackage.Tracking.Born_Digital, ThisPackage.Tracking.Disposition_Advice, ThisPackage.Tracking.Disposition_Advice_Notes,
                        ThisPackage.Tracking.Material_Received_Date, ThisPackage.Tracking.Material_Rec_Date_Estimated, ThisPackage.Tracking.Tracking_Box, aggregationCodes[0], aggregationCodes[1], aggregationCodes[2], aggregationCodes[3], aggregationCodes[4], aggregationCodes[5], aggregationCodes[6],
                        aggregationCodes[7], holding_code, source_code, icon1_name, icon2_name, icon3_name, icon4_name, icon5_name, ThisPackage.Behaviors.Left_To_Right, ThisPackage.Behaviors.CitationSet);

                    // Also, save the ticklers
                    string tickler1 = String.Empty;
                    string tickler2 = String.Empty;
                    string tickler3 = String.Empty;
                    string tickler4 = String.Empty;
                    string tickler5 = String.Empty;

                    if (ThisPackage.Behaviors.Ticklers_Count > 0)
                        tickler1 = ThisPackage.Behaviors.Ticklers[0];
                    if (ThisPackage.Behaviors.Ticklers_Count > 1)
                        tickler2 = ThisPackage.Behaviors.Ticklers[1];
                    if (ThisPackage.Behaviors.Ticklers_Count > 2)
                        tickler3 = ThisPackage.Behaviors.Ticklers[2];
                    if (ThisPackage.Behaviors.Ticklers_Count > 3)
                        tickler4 = ThisPackage.Behaviors.Ticklers[3];
                    if (ThisPackage.Behaviors.Ticklers_Count > 4)
                        tickler5 = ThisPackage.Behaviors.Ticklers[4];

                    Save_Item_Ticklers(ThisPackage.Web.ItemID, tickler1, tickler2, tickler3, tickler4, tickler5);
                }

                // If the views object is NULL then don't do anything here
                if ((ThisPackage.Behaviors.Views != null) && (ThisPackage.Behaviors.Views.Count > 0))
                {

                    // Determine which viewers to add or remove from this single item
                    List<View_Object> currentViews = Get_Current_Item_Viewers(ThisPackage.BibID, ThisPackage.VID);
                    List<View_Object> removeViews = new List<View_Object>();
                    List<View_Object> addViews = new List<View_Object>();

                    // Use a found flag to determine which current viewers are NOT present in the new list
                    // booleans start with a default value of 'false', so need to initialize
                    bool[] foundFlag = new bool[currentViews.Count];

                    // Use a dictionary for quick lookup in current views
                    Dictionary<string, int> currentViewTypeToIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    for (int i = 0; i < currentViews.Count; i++)
                    {
                        currentViewTypeToIndex[currentViews[i].View_Type] = i;
                    }
                    if (ThisPackage.Behaviors.Views != null)
                    {
                        // Now, step through all the views in the object to save
                        foreach (View_Object thisView in ThisPackage.Behaviors.Views)
                        {
                            // If this view is present only as an exclusion, skip it.  
                            // If it is removing an existing view, that will be handled in the next FOR anyway
                            if (thisView.Exclude) continue;

                            // Does this view already exists?
                            if (currentViewTypeToIndex.ContainsKey(thisView.View_Type))
                            {
                                // Get the index
                                int viewIndex = currentViewTypeToIndex[thisView.View_Type];
                                View_Object currentView = currentViews[viewIndex];

                                // Compare the label and attribute.. is there a change there?
                                if ((String.Compare(thisView.Attributes, currentView.Attributes, StringComparison.Ordinal) != 0) ||
                                    (String.Compare(thisView.Label, currentView.Label, StringComparison.Ordinal) != 0) ||
                                    (currentView.Exclude))
                                {
                                    // Even though this existed, since the label or attribute are different, this 
                                    // view will be sent to the 'add view' method
                                    addViews.Add(thisView);
                                }

                                // This existing view was handled already
                                foundFlag[viewIndex] = true;
                            }
                            else
                            {
                                // This is a new view type, so it will be added, assuming it is not an exclusion
                                addViews.Add(thisView);
                            }
                        }
                    }

                    // Look for views to remove from the database
                    for (int i = 0; i < foundFlag.Length; i++)
                    {
                        // If this view was not found in the object to save, it should be excluded
                        // or removed from the database
                        if (!foundFlag[i])
                        {
                            // If this view is just an exclusion view, then no need to 'remove it'
                            if (!currentViews[i].Exclude)
                                removeViews.Add(currentViews[i]);
                        }
                    }

                    // With the two lists in hand, now add any new views
                    int add_start_index = 0;
                    while (addViews.Count > add_start_index)
                    {
                        // Add the next six views
                        Save_Item_Add_Viewers(ThisPackage.Web.ItemID,
                            (addViews.Count > add_start_index ? addViews[add_start_index].View_Type : String.Empty),
                            (addViews.Count > add_start_index ? addViews[add_start_index].Label : String.Empty),
                            (addViews.Count > add_start_index ? addViews[add_start_index].Attributes : String.Empty),
                            (addViews.Count > add_start_index + 1 ? addViews[add_start_index + 1].View_Type : String.Empty),
                            (addViews.Count > add_start_index + 1 ? addViews[add_start_index + 1].Label : String.Empty),
                            (addViews.Count > add_start_index + 1 ? addViews[add_start_index + 1].Attributes : String.Empty),
                            (addViews.Count > add_start_index + 2 ? addViews[add_start_index + 2].View_Type : String.Empty),
                            (addViews.Count > add_start_index + 2 ? addViews[add_start_index + 2].Label : String.Empty),
                            (addViews.Count > add_start_index + 2 ? addViews[add_start_index + 2].Attributes : String.Empty),
                            (addViews.Count > add_start_index + 3 ? addViews[add_start_index + 3].View_Type : String.Empty),
                            (addViews.Count > add_start_index + 3 ? addViews[add_start_index + 3].Label : String.Empty),
                            (addViews.Count > add_start_index + 3 ? addViews[add_start_index + 3].Attributes : String.Empty),
                            (addViews.Count > add_start_index + 4 ? addViews[add_start_index + 4].View_Type : String.Empty),
                            (addViews.Count > add_start_index + 4 ? addViews[add_start_index + 4].Label : String.Empty),
                            (addViews.Count > add_start_index + 4 ? addViews[add_start_index + 4].Attributes : String.Empty),
                            (addViews.Count > add_start_index + 5 ? addViews[add_start_index + 5].View_Type : String.Empty),
                            (addViews.Count > add_start_index + 5 ? addViews[add_start_index + 5].Label : String.Empty),
                            (addViews.Count > add_start_index + 5 ? addViews[add_start_index + 5].Attributes : String.Empty));

                        // Handled these six adds
                        add_start_index += 6;
                    }

                    // Also handle any removes
                    int remove_start_index = 0;
                    while (removeViews.Count > remove_start_index)
                    {
                        // Remove the next six views
                        Save_Item_Remove_Viewers(ThisPackage.Web.ItemID,
                            (removeViews.Count > remove_start_index ? removeViews[remove_start_index].View_Type : String.Empty),
                            (removeViews.Count > remove_start_index + 1 ? removeViews[remove_start_index + 1].View_Type : String.Empty),
                            (removeViews.Count > remove_start_index + 2 ? removeViews[remove_start_index + 2].View_Type : String.Empty),
                            (removeViews.Count > remove_start_index + 3 ? removeViews[remove_start_index + 3].View_Type : String.Empty),
                            (removeViews.Count > remove_start_index + 4 ? removeViews[remove_start_index + 4].View_Type : String.Empty),
                            (removeViews.Count > remove_start_index + 5 ? removeViews[remove_start_index + 5].View_Type : String.Empty));

                        // Handled these six removes
                        remove_start_index += 6;
                    }
                }
            }
        }

        /// <summary> Saves the serial hierarchy information for a single item </summary>
        /// <param name="ThisPackage"></param>
        /// <param name="GroupID"></param>
        /// <param name="ItemID"></param>
        public static void Save_Serial_Hierarchy_Information(SobekCM_Item ThisPackage, int GroupID, int ItemID)
        {
            string level1_text = String.Empty;
            string level2_text = String.Empty;
            string level3_text = String.Empty;
            string level4_text = String.Empty;
            string level5_text = String.Empty;
            int level1_index = -1;
            int level2_index = -1;
            int level3_index = -1;
            int level4_index = -1;
            int level5_index = -1;

            StringBuilder builder = new StringBuilder();

            if (ThisPackage.Behaviors.hasSerialInformation)
            {
                if (ThisPackage.Behaviors.Serial_Info.Count > 0)
                {
                    Serial_Info.Single_Serial_Hierarchy level1 = ThisPackage.Behaviors.Serial_Info[0];
                    level1_index = level1.Order;
                    level1_text = level1.Display;
                }

                if (ThisPackage.Behaviors.Serial_Info.Count > 1)
                {
                    Serial_Info.Single_Serial_Hierarchy level2 = ThisPackage.Behaviors.Serial_Info[1];
                    level2_index = level2.Order;
                    level2_text = level2.Display;
                }

                if (ThisPackage.Behaviors.Serial_Info.Count > 2)
                {
                    Serial_Info.Single_Serial_Hierarchy level3 = ThisPackage.Behaviors.Serial_Info[2];
                    level3_index = level3.Order;
                    level3_text = level3.Display;
                }

                if (ThisPackage.Behaviors.Serial_Info.Count > 3)
                {
                    Serial_Info.Single_Serial_Hierarchy level4 = ThisPackage.Behaviors.Serial_Info[3];
                    level4_index = level4.Order;
                    level4_text = level4.Display;
                }

                if (ThisPackage.Behaviors.Serial_Info.Count > 4)
                {
                    Serial_Info.Single_Serial_Hierarchy level5 = ThisPackage.Behaviors.Serial_Info[4];
                    level5_index = level5.Order;
                    level5_text = level5.Display;
                }


                for (int i = 0; i < ThisPackage.Behaviors.Serial_Info.Count; i++)
                {
                    builder.Append(ThisPackage.Behaviors.Serial_Info[i].Display + "|" + ThisPackage.Behaviors.Serial_Info[i].Order);
                    if ((i + 1) < ThisPackage.Behaviors.Serial_Info.Count)
                        builder.Append(";");
                }
            }

            // Call the stored procedure
            Save_Serial_Hierarchy(GroupID, ItemID, level1_text, level1_index, level2_text, level2_index, level3_text, level3_index, level4_text, level4_index, level5_text, level5_index, builder.ToString());
        }

        private static bool Save_Item_Metadata_Information(SobekCM_Item ThisPackage)
        {
            // Clear any existing item metadata
            Clear_Item_Metadata(ThisPackage.Web.ItemID, false);

            // Build lists of the metadata now
            List<KeyValuePair<string, string>> metadataTerms = new List<KeyValuePair<string, string>>();

            // Add the BibID
            metadataTerms.Add(new KeyValuePair<string, string>("BibID", ThisPackage.BibID));

            // Add the main bibliographic terms
            metadataTerms.AddRange(ThisPackage.Bib_Info.Metadata_Search_Terms);

            // Step through all the metadata modules and add any additional metadata search terms
            if (ThisPackage.Metadata_Modules != null)
            {
                foreach (iMetadata_Module thisModule in ThisPackage.Metadata_Modules)
                {
                    List<KeyValuePair<string, string>> moduleMetadata = thisModule.Metadata_Search_Terms;
                    if (moduleMetadata != null)
                        metadataTerms.AddRange(moduleMetadata);
                }
            }

            // Add serial information
            if (ThisPackage.Behaviors.hasSerialInformation)
            {
                if (ThisPackage.Behaviors.Serial_Info.Count > 0)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Other Citation", ThisPackage.Behaviors.Serial_Info[0].Display));
                }

                if (ThisPackage.Behaviors.Serial_Info.Count > 1)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Other Citation", ThisPackage.Behaviors.Serial_Info[1].Display));
                }

                if (ThisPackage.Behaviors.Serial_Info.Count > 2)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Other Citation", ThisPackage.Behaviors.Serial_Info[2].Display));
                }

                if (ThisPackage.Behaviors.Serial_Info.Count > 3)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Other Citation", ThisPackage.Behaviors.Serial_Info[3].Display));
                }

                if (ThisPackage.Behaviors.Serial_Info.Count > 4)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Other Citation", ThisPackage.Behaviors.Serial_Info[4].Display));
                }
            }

            // Add in the ticklers
            if (ThisPackage.Behaviors.Ticklers_Count > 0)
            {
                foreach (string thisTickler in ThisPackage.Behaviors.Ticklers)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Tickler", thisTickler));
                }
            }

            // Allow the division/file tree to save metadata here 
            metadataTerms.AddRange(ThisPackage.Divisions.Metadata_Search_Terms);

            //modify by Keven for dPanther            
            string source_code = ThisPackage.Bib_Info.Source.Code;

            if ((source_code.Length > 0) && (source_code[0] != 'i') && (source_code[0] != 'I'))
            {
                source_code = "i" + source_code;
            }
            if ((source_code.Length > 2) && (source_code.ToUpper().IndexOf("II") == 0))
                source_code = source_code.Substring(1);

            for (int aggNum = 0; aggNum < ThisPackage.Behaviors.Aggregation_Code_List.Count; aggNum++)
            {
                metadataTerms.Add(new KeyValuePair<string, string>("Aggregation", ThisPackage.Behaviors.Aggregation_Code_List[aggNum]));
            }


            // Just add blanks in at the end to get this to an increment of ten
            while ((metadataTerms.Count%10) != 0)
            {
                metadataTerms.Add(new KeyValuePair<string, string>(String.Empty, String.Empty));
            }

            // Now, save this metadata to the database
            int current_index = 0;
            while ((current_index + 10) <= metadataTerms.Count)
            {
                // Save the next ten values
                Save_Item_Metadata(ThisPackage.Web.ItemID,
                    metadataTerms[current_index].Key, metadataTerms[current_index].Value,
                    metadataTerms[current_index + 1].Key, metadataTerms[current_index + 1].Value,
                    metadataTerms[current_index + 2].Key, metadataTerms[current_index + 2].Value,
                    metadataTerms[current_index + 3].Key, metadataTerms[current_index + 3].Value,
                    metadataTerms[current_index + 4].Key, metadataTerms[current_index + 4].Value,
                    metadataTerms[current_index + 5].Key, metadataTerms[current_index + 5].Value,
                    metadataTerms[current_index + 6].Key, metadataTerms[current_index + 6].Value,
                    metadataTerms[current_index + 7].Key, metadataTerms[current_index + 7].Value,
                    metadataTerms[current_index + 8].Key, metadataTerms[current_index + 8].Value,
                    metadataTerms[current_index + 9].Key, metadataTerms[current_index + 9].Value);

                // Increment curent index
                current_index += 10;
            }

            return true;
        }

        #endregion

        #region Stored Procedure Calls


        /// <summary> Stores the arguments from saving an item group to the SobekCM databse </summary>
        public struct Save_Item_Group_Args
        {
            /// <summary> Group ID for this item group / title </summary>
            public readonly int GroupID;

            /// <summary> New BibID for this item group / title  </summary>
            public readonly string New_BibID;

            /// <summary> Flag indicates if this was a new item group in the database </summary>
            public readonly bool Is_New;

            /// <summary> Constructor for a new instance of the Save_Item_Group_Args class </summary>
            /// <param name="GroupID">GroupID ID for this item group / title</param>
            /// <param name="New_BibID">New BibID for this item group / title</param>
            /// <param name="Is_New">Flag indicates if this was a new item group in the database</param>
            public Save_Item_Group_Args(int GroupID, string New_BibID, bool Is_New)
            {
                this.GroupID = GroupID;
                this.New_BibID = New_BibID;
                this.Is_New = Is_New;
            }
        }

        /// <summary> Saves the main information about an item group in SobekCM </summary>
        /// <param name="BibID">Bib ID for this group</param>
        /// <param name="GroupTitle">Title for this group</param>
        /// <param name="SortTitle">Sort title for this item group</param>
        /// <param name="CreateDate"> Day this item was created </param>
        /// <param name="File_Root"> File root for this item group's files </param>
        /// <param name="Update_Existing"> Flag indicates to update this bib id if it exists</param>
        /// <param name="Type">Resource type for this group</param>
        /// <param name="ALEPH_Number"> ALEPH record number for this item group</param>
        /// <param name="OCLC_Number"> OCLC record number for this item group</param>
        /// <param name="Group_Thumbnail"> Thumbnail to use for this item group </param>
        /// <param name="Large_Format"> Flag indicates if this a large format item, which will affect size of service image files </param>
        /// <param name="Never_Overlay_Record"> Flag indicates to never overlay this record from the original ALEPH or OCLC record </param>
        /// <param name="Track_By_Month"> Flag indicates this material should be tracked by month, along with other items within the same title </param>
        /// <param name="Primary_Identifier_Type"> Primary identifier type for this item group </param>
        /// <param name="Primary_Identifier"> Primary identifier for this item group </param>
        /// <returns> Arguments which include the group id and bibid for this item group / title </returns>
        /// <remarks> This method calls the stored procedure 'SobekCM_Save_Item_Group'. </remarks>
        /// <exception cref="ApplicationException"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        protected static Save_Item_Group_Args Save_Item_Group(string BibID, string GroupTitle, string SortTitle, string Type, string File_Root, string Group_Thumbnail, bool Update_Existing, DateTime CreateDate, long OCLC_Number, int ALEPH_Number, bool Large_Format, bool Track_By_Month, bool Never_Overlay_Record, string Primary_Identifier_Type, string Primary_Identifier)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[17];
                param_list[0] = new EalDbParameter("@BibID", BibID);
                param_list[1] = new EalDbParameter("@GroupTitle", GroupTitle);
                param_list[2] = new EalDbParameter("@SortTitle", SortTitle);
                param_list[3] = new EalDbParameter("@Type", Type);
                param_list[4] = new EalDbParameter("@File_Location", File_Root);
                if (OCLC_Number < 0)
                    param_list[5] = new EalDbParameter("@OCLC_Number", 1);
                else
                    param_list[5] = new EalDbParameter("@OCLC_Number", OCLC_Number);
                if (ALEPH_Number < 0)
                    param_list[6] = new EalDbParameter("@ALEPH_Number", 1);
                else
                    param_list[6] = new EalDbParameter("@ALEPH_Number", ALEPH_Number);
                param_list[7] = new EalDbParameter("@Group_Thumbnail", Group_Thumbnail);
                param_list[8] = new EalDbParameter("@Large_Format", Large_Format);
                param_list[9] = new EalDbParameter("@Track_By_Month", Track_By_Month);
                param_list[10] = new EalDbParameter("@Never_Overlay_Record", Never_Overlay_Record);
                param_list[11] = new EalDbParameter("@Update_Existing", Update_Existing);
                param_list[12] = new EalDbParameter("@PrimaryIdentifierType", Primary_Identifier_Type);
                param_list[13] = new EalDbParameter("@PrimaryIdentifier", Primary_Identifier);
                param_list[14] = new EalDbParameter("@GroupID", -1) {Direction = ParameterDirection.InputOutput};
                param_list[15] = new EalDbParameter("@New_BibID", "0000000000") {Direction = ParameterDirection.InputOutput};
                param_list[16] = new EalDbParameter("@New_Group", false) {Direction = ParameterDirection.InputOutput};

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Save_Item_Group", param_list);

                // Get the values to return
                int groupid = (int) param_list[14].Value;
                string bibid = param_list[15].Value.ToString();
                bool is_new = Convert.ToBoolean(param_list[16].Value);

                // Return the value
                return new Save_Item_Group_Args(groupid, bibid, is_new);
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Save_Item_Group", ee);
                return new Save_Item_Group_Args(-1, String.Empty, false);
            }
        }

        /// <summary> Saves the OAI-PMH data associated with an item </summary>
        /// <param name="ItemID"> Primary key for this item within the database </param>
        /// <param name="Data_Code"> Code for this metadata type being saved for this item ( i.e. 'oai_dc' )</param>
        /// <param name="OAI_Data"> Data to be stored for this item </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks>This method calls the stored procedure 'SobekCM_Add_OAI_PMH_Data'. <br /><br />
        /// If the OAI-PMH data is locked via the database flag on this data, this does not change the OAI stored for the item.</remarks>
        public static bool Save_Item_OAI(int ItemID, string OAI_Data, string Data_Code)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[3];
                param_list[0] = new EalDbParameter("@itemid", ItemID);
                param_list[1] = new EalDbParameter("@data_code", Data_Code);
                param_list[2] = new EalDbParameter("@oai_data", OAI_Data);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Add_OAI_PMH_Data", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Add_OAI_PMH_Data", ee);
                return false;
            }
        }

        /// <summary> Saves the links between an item group and the web skins possible </summary>
        /// <param name="GroupID"> Primary key to this item group / title in the SobekCM database </param>
        /// <param name="ThisPackage"> Digital resource object from which to save the web skins </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This method calls the stored procedure 'SobekCM_Save_Item_Group_Web_Skins'. </remarks>
        /// <exception cref="ApplicationException"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        public static bool Save_Item_Group_Web_Skins(int GroupID, SobekCM_Item ThisPackage)
        {
            // Get all the web skin restrictions from this package, if they exist
            string primaryInterface = String.Empty;
            string altInterface1 = String.Empty;
            string altInterface2 = String.Empty;
            string altInterface3 = String.Empty;
            string altInterface4 = String.Empty;
            string altInterface5 = String.Empty;
            string altInterface6 = String.Empty;
            string altInterface7 = String.Empty;
            string altInterface8 = String.Empty;
            string altInterface9 = String.Empty;
            if (ThisPackage.Behaviors.Web_Skins.Count > 0)
            {
                primaryInterface = ThisPackage.Behaviors.Web_Skins[0];
                if (ThisPackage.Behaviors.Web_Skins.Count > 1)
                    altInterface1 = ThisPackage.Behaviors.Web_Skins[1];
                if (ThisPackage.Behaviors.Web_Skins.Count > 2)
                    altInterface2 = ThisPackage.Behaviors.Web_Skins[2];
                if (ThisPackage.Behaviors.Web_Skins.Count > 3)
                    altInterface3 = ThisPackage.Behaviors.Web_Skins[3];
                if (ThisPackage.Behaviors.Web_Skins.Count > 4)
                    altInterface4 = ThisPackage.Behaviors.Web_Skins[4];
                if (ThisPackage.Behaviors.Web_Skins.Count > 5)
                    altInterface5 = ThisPackage.Behaviors.Web_Skins[5];
                if (ThisPackage.Behaviors.Web_Skins.Count > 6)
                    altInterface6 = ThisPackage.Behaviors.Web_Skins[6];
                if (ThisPackage.Behaviors.Web_Skins.Count > 7)
                    altInterface7 = ThisPackage.Behaviors.Web_Skins[7];
                if (ThisPackage.Behaviors.Web_Skins.Count > 8)
                    altInterface8 = ThisPackage.Behaviors.Web_Skins[8];
                if (ThisPackage.Behaviors.Web_Skins.Count > 9)
                    altInterface9 = ThisPackage.Behaviors.Web_Skins[9];


            }
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[11];
                param_list[0] = new EalDbParameter("@GroupID", GroupID);
                param_list[1] = new EalDbParameter("@Primary_WebSkin", primaryInterface);
                param_list[2] = new EalDbParameter("@Alt_WebSkin1", altInterface1);
                param_list[3] = new EalDbParameter("@Alt_WebSkin2", altInterface2);
                param_list[4] = new EalDbParameter("@Alt_WebSkin3", altInterface3);
                param_list[5] = new EalDbParameter("@Alt_WebSkin4", altInterface4);
                param_list[6] = new EalDbParameter("@Alt_WebSkin5", altInterface5);
                param_list[7] = new EalDbParameter("@Alt_WebSkin6", altInterface6);
                param_list[8] = new EalDbParameter("@Alt_WebSkin7", altInterface7);
                param_list[9] = new EalDbParameter("@Alt_WebSkin8", altInterface8);
                param_list[10] = new EalDbParameter("@Alt_WebSkin9", altInterface9);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Save_Item_Group_Web_Skins", param_list);

                // Return the value
                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Save_Item_Group_Web_Skins", ee);
                return false;
            }
        }

        /// <summary> Stores the arguments from saving a bibliographic item to the SobekCM databse </summary>
        public struct Save_Item_Args
        {
            /// <summary> Item ID for this item </summary>
            public readonly int ItemID;

            /// <summary> TRUE if this item already existed, FALSE otherwise </summary>
            public readonly bool Existing;

            /// <summary> New VID for this item  </summary>
            public readonly string New_VID;

            /// <summary> Constructor for a new instance of the Save_Item_Args class </summary>
            /// <param name="ItemID">Item ID for this item</param>
            /// <param name="Existing">TRUE if this item already existed, FALSE otherwise</param>
            /// <param name="New_VID"> New VID for this sabed item </param>
            public Save_Item_Args(int ItemID, bool Existing, string New_VID)
            {
                this.ItemID = ItemID;
                this.Existing = Existing;
                this.New_VID = New_VID;
            }
        }

        /// <summary> Stores information about a single digital resource ( existing or new ) </summary>
        /// <param name="GroupID"> Primary key for the item group / title / bib id</param>
        /// <param name="VID"> Volume identifier for this item </param>
        /// <param name="PageCount"> Number of pages present in this item </param>
        /// <param name="FileCount"> Number of files present in this item </param>
        /// <param name="Title"> Main title for this volume </param>
        /// <param name="SortTitle"> Sort title for this volume ( all caps, and without the leading articles like 'The', 'A', 'Le', etc.. )</param>
        /// <param name="Link"> Link to this item, if this is not loaded into this digital library </param>
        /// <param name="CreateDate"> Create date for this item </param>
        /// <param name="PubDate"> Publication date string </param>
        /// <param name="SortDate"> Sort date is used to sort all items by publication date </param>
        /// <param name="Holding_Code"> Code for the holding location related to this item </param>
        /// <param name="Source_Code"> Code for the source institution related to this item </param>
        /// <param name="Author"> Display field includes all authors' names </param>
        /// <param name="Spatial_KML"> List of main coordinate points in the main display for this item</param>
        /// <param name="Spatial_KML_Distance"> Distance of the hypotenuse of the bounding box made up by the coordinates in this item </param>
        /// <param name="DiskSizeMb"> Total size (in MB) of this entire package on the digital library </param>
        /// <param name="Donor">Donor string to display for this item within any search or browse results</param>
        /// <param name="Publisher">Publishers string to display for this item within any search or browse results ( multiple publishers are seperated by a '|' character )</param>
        /// <param name="Edition"> Edition/state string to display for this item within any search or browse results</param>
        /// <param name="Institution_Display"> Institution statement to display for this item within any search or browse results</param>
        /// <param name="Material_Display"> Materials (i.e. type of materials used to create object) to display for this item within any search or browse results </param>
        /// <param name="Measurement_Display"> Measurement information to display for this item within any search or browse results</param>
        /// <param name="Spatial_Display"> Spatial information string to display for this item within any search or browse results ( multiple compound spatials are seperated by a '|' character )</param>
        /// <param name="StylePeriod_Display"> Style/Period string to display for this item within any search or browse results</param>
        /// <param name="Subjects_Display"> Subjects information string to display for this item within any search or browse results ( multiple compound subjects are seperated by a '|' character )</param>
        /// <param name="Technique_Display">Techniques information string to display for this item within any search or browse results </param>
        /// <returns> Arguments which indicate the item id for this item and whether a new volume was added or not </returns>
        /// <remarks> This calls the 'SobekCM_Save_Item' stored procedure in the SobekCM database </remarks>
        protected static Save_Item_Args Save_Item(int GroupID, string VID, int PageCount, int FileCount,
            string Title, string SortTitle, string Link, DateTime CreateDate, string PubDate, int SortDate,
            string Holding_Code, string Source_Code, string Author, string Spatial_KML, double Spatial_KML_Distance,
            double DiskSizeMb, string Donor, string Publisher, string Spatial_Display, string Institution_Display,
            string Edition, string Material_Display, string Measurement_Display, string StylePeriod_Display, string Technique_Display,
            string Subjects_Display)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[30];
                param_list[0] = new EalDbParameter("@GroupID", GroupID);
                param_list[1] = new EalDbParameter("@VID", VID);
                param_list[2] = new EalDbParameter("@PageCount", PageCount);
                param_list[3] = new EalDbParameter("@FileCount", FileCount);
                param_list[4] = new EalDbParameter("@Title", Title);
                param_list[5] = new EalDbParameter("@SortTitle", SortTitle);
                param_list[6] = new EalDbParameter("@AccessMethod", 1);
                param_list[7] = new EalDbParameter("@Link", Link);
                param_list[8] = new EalDbParameter("@CreateDate", CreateDate);
                param_list[9] = new EalDbParameter("@PubDate", PubDate);
                param_list[10] = new EalDbParameter("@SortDate", SortDate);
                param_list[11] = new EalDbParameter("@HoldingCode", Holding_Code);
                param_list[12] = new EalDbParameter("@SourceCode", Source_Code);
                param_list[13] = new EalDbParameter("@Author", Author);
                param_list[14] = new EalDbParameter("@Spatial_KML", Spatial_KML);
                param_list[15] = new EalDbParameter("@Spatial_KML_Distance", Spatial_KML_Distance);
                param_list[16] = new EalDbParameter("@DiskSize_KB", DiskSizeMb);
                param_list[17] = new EalDbParameter("@Spatial_Display", Spatial_Display);
                param_list[18] = new EalDbParameter("@Institution_Display", Institution_Display);
                param_list[19] = new EalDbParameter("@Edition_Display", Edition);
                param_list[20] = new EalDbParameter("@Material_Display", Material_Display);
                param_list[21] = new EalDbParameter("@Measurement_Display", Measurement_Display);
                param_list[22] = new EalDbParameter("@StylePeriod_Display", StylePeriod_Display);
                param_list[23] = new EalDbParameter("@Technique_Display", Technique_Display);
                param_list[24] = new EalDbParameter("@Subjects_Display", Subjects_Display);
                param_list[25] = new EalDbParameter("@Donor", Donor);
                param_list[26] = new EalDbParameter("@Publisher", Publisher);
                param_list[27] = new EalDbParameter("@ItemID", -1) {Direction = ParameterDirection.InputOutput};
                param_list[28] = new EalDbParameter("@Existing", false) {Direction = ParameterDirection.InputOutput};
                param_list[29] = new EalDbParameter("@New_VID", "00000") {Direction = ParameterDirection.InputOutput};

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Save_Item", param_list);

                // Return the value
                int itemID = (int) param_list[27].Value;
                bool existing = (bool) param_list[28].Value;
                string new_vid = param_list[29].Value.ToString();
                return new Save_Item_Args(itemID, existing, new_vid);
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Save_Item", ee);
                return new Save_Item_Args(-1, false, String.Empty);
            }
        }

        /// <summary> Clears all the metadata associated with a particular item in the database </summary>
        /// <param name="ItemID"> Primary key for the item to clear the searchable metadata values</param>
        /// <param name="Clear_Non_Metadata_Values">Flag indicates if the values which are not derived from the metadata file should be cleared as well </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Metadata_Clear2' stored procedure in the SobekCM database </remarks>
        protected static bool Clear_Item_Metadata(int ItemID, bool Clear_Non_Metadata_Values)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[2];
                param_list[0] = new EalDbParameter("@itemid", ItemID);
                param_list[1] = new EalDbParameter("@clear_non_mets_values", Clear_Non_Metadata_Values);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Metadata_Clear2", param_list);
                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Metadata_Clear2", ee);
                return false;
            }
        }

        /// <summary> Saves metadata from an item into the tables for searching metadata </summary>
        /// <param name="ItemID"> Primary key for the item to which to tag this metadata </param>
        /// <param name="Metadata_Type1"> Type string for the first metadata value </param>
        /// <param name="Metadata_Value1"> Value of the first piece of metadata </param>
        /// <param name="Metadata_Type2">Type string for the second metadata value</param>
        /// <param name="Metadata_Value2"> Value of the second piece of metadata</param>
        /// <param name="Metadata_Type3">Type string for the third metadata value</param>
        /// <param name="Metadata_Value3"> Value of the third piece of metadata</param>
        /// <param name="Metadata_Type4">Type string for the fourth metadata value</param>
        /// <param name="Metadata_Value4"> Value of the fourth piece of metadata</param>
        /// <param name="Metadata_Type5">Type string for the fifth metadata value</param>
        /// <param name="Metadata_Value5"> Value of the fifth piece of metadata</param>
        /// <param name="Metadata_Type6">Type string for the sixth metadata value</param>
        /// <param name="Metadata_Value6"> Value of the sixth piece of metadata</param>
        /// <param name="Metadata_Type7">Type string for the seventh metadata value</param>
        /// <param name="Metadata_Value7"> Value of the seventh piece of metadata</param>
        /// <param name="Metadata_Type8">Type string for the eight metadata value</param>
        /// <param name="Metadata_Value8"> Value of the eight piece of metadata</param>
        /// <param name="Metadata_Type9">Type string for the ninth metadata value</param>
        /// <param name="Metadata_Value9"> Value of the ninth piece of metadata</param>
        /// <param name="Metadata_Type10">Type string for the tenth metadata value</param>
        /// <param name="Metadata_Value10"> Value of the tenth piece of metadata</param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Metadata_Save' stored procedure in the SobekCM database </remarks>
        protected static bool Save_Item_Metadata(int ItemID, string Metadata_Type1, string Metadata_Value1,
            string Metadata_Type2, string Metadata_Value2, string Metadata_Type3, string Metadata_Value3,
            string Metadata_Type4, string Metadata_Value4, string Metadata_Type5, string Metadata_Value5,
            string Metadata_Type6, string Metadata_Value6, string Metadata_Type7, string Metadata_Value7,
            string Metadata_Type8, string Metadata_Value8, string Metadata_Type9, string Metadata_Value9,
            string Metadata_Type10, string Metadata_Value10)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[21];
                param_list[0] = new EalDbParameter("@itemid", ItemID);
                param_list[1] = new EalDbParameter("@metadata_type1", Metadata_Type1);
                param_list[2] = new EalDbParameter("@metadata_value1", Metadata_Value1.Trim());
                param_list[3] = new EalDbParameter("@metadata_type2", Metadata_Type2);
                param_list[4] = new EalDbParameter("@metadata_value2", Metadata_Value2.Trim());
                param_list[5] = new EalDbParameter("@metadata_type3", Metadata_Type3);
                param_list[6] = new EalDbParameter("@metadata_value3", Metadata_Value3.Trim());
                param_list[7] = new EalDbParameter("@metadata_type4", Metadata_Type4);
                param_list[8] = new EalDbParameter("@metadata_value4", Metadata_Value4.Trim());
                param_list[9] = new EalDbParameter("@metadata_type5", Metadata_Type5);
                param_list[10] = new EalDbParameter("@metadata_value5", Metadata_Value5.Trim());
                param_list[11] = new EalDbParameter("@metadata_type6", Metadata_Type6);
                param_list[12] = new EalDbParameter("@metadata_value6", Metadata_Value6.Trim());
                param_list[13] = new EalDbParameter("@metadata_type7", Metadata_Type7);
                param_list[14] = new EalDbParameter("@metadata_value7", Metadata_Value7.Trim());
                param_list[15] = new EalDbParameter("@metadata_type8", Metadata_Type8);
                param_list[16] = new EalDbParameter("@metadata_value8", Metadata_Value8.Trim());
                param_list[17] = new EalDbParameter("@metadata_type9", Metadata_Type9);
                param_list[18] = new EalDbParameter("@metadata_value9", Metadata_Value9.Trim());
                param_list[19] = new EalDbParameter("@metadata_type10", Metadata_Type10);
                param_list[20] = new EalDbParameter("@metadata_value10", Metadata_Value10.Trim());

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Metadata_Save", param_list);
                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Metadata_Save", ee);
                return false;
            }
        }

        /// <summary> Causes the database to build the searchable full citation cell for basic searching from
        /// all of the discrete metadata elements stored for this item </summary>
        /// <param name="ItemID">Item ID of the item </param>
        /// <remarks> This method calls the stored procedure 'SobekCM_Create_Full_Citation_Value'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        public static bool Create_Full_Citation_Value(int ItemID)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[1];
                param_list[0] = new EalDbParameter("@ItemID", ItemID);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Create_Full_Citation_Value", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Create_Full_Citation_Value", ee);
                return false;
            }
        }

        /// <summary> Clears all the old information out from the database </summary>
        /// <param name="ItemID">Item ID of the item to clear</param>
        /// <remarks> This method calls the stored procedure 'SobekCM_Clear_Old_Item_Info'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        protected static bool Clear_Old_Item_Info(int ItemID)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[1];
                param_list[0] = new EalDbParameter("@ItemID", ItemID);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Clear_Old_Item_Info", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Clear_Old_Item_Info", ee);
                return false;
            }
        }

        private static List<View_Object> Get_Current_Item_Viewers(string BibID, string VID )
        {
            // Build the parameters list
            List<EalDbParameter> parameters = new List<EalDbParameter>
	        {
	            new EalDbParameter("@bibid", BibID), 
                new EalDbParameter("@vid", VID)
	        };

            // Create the database agnostic reader
            EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Viewers", parameters);

            // Pull out the database reader
            DbDataReader reader = readerWrapper.Reader;

            //Build the return list
            List<View_Object> returnValue = new List<View_Object>();

            // Get all the main title values first
            while (reader.Read())
            {
                string viewType = reader.GetString(0);
                string attributes = reader.GetString(1);
                string label = reader.GetString(2);
                double menuOrder = reader.GetDouble(3);
                bool exclude = reader.GetBoolean(4);

                // Create new database title object for this
                View_Object result = new View_Object
                {
                    View_Type = viewType,
                    Attributes = attributes,
                    Label = label,
                    MenuOrder = (float) menuOrder,
                    Exclude = exclude
                };

                returnValue.Add(result);
            }

            // Close the reader (which also closes the connection)
            readerWrapper.Close();

            return returnValue;
        }

        /// <summary> Saves the behaviors for a single item in a SobekCM digital library </summary>
        /// <param name="ItemID">Item ID to associate these icons and downlaods with</param>
        /// <param name="TextSearchable"> Flag indicates if this item is text searchable </param>
        /// <param name="MainThumbnail"> Main thumbnail file name </param>
        /// <param name="MainJPEG"> Main page jpeg image (for full results view) </param>
        /// <param name="IP_Restriction_Mask">IP Restriction mask for this item </param>
        /// <param name="CheckoutRequired"> Flag indicates if this is a single-use item which requires checkout</param>
        /// <param name="Dark_Flag"> Flag indicates if this item is DARK and should not be made public or restricted (i.e., remains private)</param>
        /// <param name="Born_Digital"> Flag indicates if this material was born digitally, rather than digitized </param>
        /// <param name="DispositionAdvice"> Key to how this item will be disposed of once complete, or -1 for no advice </param>
        /// <param name="DispositionAdviceNotes"> Notes to further explain how this item should be disposed of (i.e., return to whom, etc.)</param>
        /// <param name="Material_Received_Date"> Day the material was received into the digitization office </param>
        /// <param name="Material_Recd_Date_Estimated"> Flag indicates if the date above is an estimate </param>
        /// <param name="Tracking_Box"> Tracking box this item currently resides in or is tracked with </param>
        /// <param name="AggregationCode1"> Code for the first aggregation this item belongs to </param>
        /// <param name="AggregationCode2"> Code for the second aggregation this item belongs to </param>
        /// <param name="AggregationCode3"> Code for the third aggregation this item belongs to </param>
        /// <param name="AggregationCode4"> Code for the fourth aggregation this item belongs to </param>
        /// <param name="AggregationCode5"> Code for the fifth aggregation this item belongs to </param>
        /// <param name="AggregationCode6"> Code for the sixth aggregation this item belongs to </param>
        /// <param name="AggregationCode7"> Code for the seventh aggregation this item belongs to </param>
        /// <param name="AggregationCode8"> Code for the eighth aggregation this item belongs to </param>
        /// <param name="HoldingCode"> Holding code for this item's holding location aggregation </param>
        /// <param name="SourceCode"> Source location code for this item's source location </param>
        /// <param name="Icon1_Name">Name of the first icon</param>
        /// <param name="Icon2_Name">Name of the second icon</param>
        /// <param name="Icon3_Name">Name of the third icon</param>
        /// <param name="Icon4_Name">Name of the fourth icon</param>
        /// <param name="Icon5_Name">Name of the fifth icon</param>
        /// <param name="Left_To_Right"> Flag indicates this item is read from Left-to-Right, rather than standard Right-to-Left</param>
        /// <param name="CitationSet"> Citation set to use when displaying this item</param>
        /// <remarks> This method calls the stored procedure 'SobekCM_Save_Item_Behaviors'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        protected static bool Save_Item_Behaviors(int ItemID, bool TextSearchable, string MainThumbnail,
            string MainJPEG, short IP_Restriction_Mask, bool CheckoutRequired, bool Dark_Flag, bool Born_Digital,
            short DispositionAdvice, string DispositionAdviceNotes, Nullable<DateTime> Material_Received_Date, bool Material_Recd_Date_Estimated,
            string Tracking_Box, string AggregationCode1, string AggregationCode2,
            string AggregationCode3, string AggregationCode4, string AggregationCode5, string AggregationCode6,
            string AggregationCode7, string AggregationCode8, string HoldingCode, string SourceCode,
            string Icon1_Name, string Icon2_Name, string Icon3_Name, string Icon4_Name, string Icon5_Name, 
            bool Left_To_Right, string CitationSet )
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[30];
                param_list[0] = new EalDbParameter("@ItemID", ItemID);
                param_list[1] = new EalDbParameter("@TextSearchable", TextSearchable);
                param_list[2] = new EalDbParameter("@MainThumbnail", MainThumbnail);
                param_list[3] = new EalDbParameter("@MainJPEG", MainJPEG);
                param_list[4] = new EalDbParameter("@IP_Restriction_Mask", IP_Restriction_Mask);
                param_list[5] = new EalDbParameter("@CheckoutRequired", CheckoutRequired);
                param_list[6] = new EalDbParameter("@Dark_Flag", Dark_Flag);
                param_list[7] = new EalDbParameter("@Born_Digital", Born_Digital);
                if (DispositionAdvice <= 0)
                    param_list[8] = new EalDbParameter("@Disposition_Advice", DBNull.Value);
                else
                    param_list[8] = new EalDbParameter("@Disposition_Advice", DispositionAdvice);
                param_list[9] = new EalDbParameter("@Disposition_Advice_Notes", DispositionAdviceNotes);

                if (Material_Received_Date.HasValue)
                    param_list[10] = new EalDbParameter("@Material_Received_Date", Material_Received_Date.Value);
                else
                    param_list[10] = new EalDbParameter("@Material_Received_Date", DBNull.Value);
                param_list[11] = new EalDbParameter("@Material_Recd_Date_Estimated", Material_Recd_Date_Estimated);
                param_list[12] = new EalDbParameter("@Tracking_Box", Tracking_Box);
                param_list[13] = new EalDbParameter("@AggregationCode1", AggregationCode1);
                param_list[14] = new EalDbParameter("@AggregationCode2", AggregationCode2);
                param_list[15] = new EalDbParameter("@AggregationCode3", AggregationCode3);
                param_list[16] = new EalDbParameter("@AggregationCode4", AggregationCode4);
                param_list[17] = new EalDbParameter("@AggregationCode5", AggregationCode5);
                param_list[18] = new EalDbParameter("@AggregationCode6", AggregationCode6);
                param_list[19] = new EalDbParameter("@AggregationCode7", AggregationCode7);
                param_list[20] = new EalDbParameter("@AggregationCode8", AggregationCode8);
                param_list[21] = new EalDbParameter("@HoldingCode", HoldingCode);
                param_list[22] = new EalDbParameter("@SourceCode", SourceCode);
                param_list[23] = new EalDbParameter("@Icon1_Name", Icon1_Name);
                param_list[24] = new EalDbParameter("@Icon2_Name", Icon2_Name);
                param_list[25] = new EalDbParameter("@Icon3_Name", Icon3_Name);
                param_list[26] = new EalDbParameter("@Icon4_Name", Icon4_Name);
                param_list[27] = new EalDbParameter("@Icon5_Name", Icon5_Name);
                param_list[28] = new EalDbParameter("@Left_To_Right", Left_To_Right);
                param_list[29] = new EalDbParameter("@CitationSet", CitationSet);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Save_Item_Behaviors", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Save_Item_Behaviors", ee);
                return false;
            }
        }



        /// <summary> Saves the minimal behaviors for a single item in a SobekCM digital library </summary>
        /// <param name="ItemID">Item ID to associate these icons and downlaods with</param>
        /// <param name="TextSearchable"> Flag indicates if this item is text searchable </param>
        /// <remarks> This method calls the stored procedure 'SobekCM_Save_Item_Behaviors_Minimal'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        protected static bool Save_Item_Behaviors_Minimal(int ItemID, bool TextSearchable )
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[20];
                param_list[0] = new EalDbParameter("@ItemID", ItemID);
                param_list[1] = new EalDbParameter("@TextSearchable", TextSearchable);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Save_Item_Behaviors_Minimal", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Save_Item_Behaviors_Minimal", ee);
                return false;
            }
        }

        /// <summary> Add or update a setting related to an individual digital resource </summary>
        /// <param name="ItemID"> ItemID to attach this setting value </param>
        /// <param name="Setting_Key"> Key for this setting value </param>
        /// <param name="Setting_Value"> Value for this setting</param>
        /// <returns> TRUE if succesful, otherwise FALSE </returns>
        /// <remarks> This method calls the stored procedure 'SobekCM_Set_Item_Setting_Value'. </remarks>
        public static bool Set_Item_Setting_Value(int ItemID, string Setting_Key, string Setting_Value)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[3];
                param_list[0] = new EalDbParameter("@ItemID", ItemID);
                param_list[1] = new EalDbParameter("@Setting_Key", Setting_Key);
                param_list[2] = new EalDbParameter("@Setting_Value", Setting_Value);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Set_Item_Setting_Value", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Save_Item_Behaviors_Minimal", ee);
                return false;
            }
        }


        /// <summary> Add an new item viewer linked to an item </summary>
        /// <param name="ItemID">Item ID to associate these icons and downlaods with</param>
        /// <param name="Viewer_Type"> Viewer type code for the new viewer type in the SobekCM database </param>
        /// <param name="Viewer_Label"> Label to be displayed for the new viewer of this item </param>
        /// <param name="Viewer_Attributes"> Optional attributes for the new viewer of this item </param>
        /// <remarks> This method calls the stored procedure 'SobekCM_Add_Item_Viewers'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        public static bool Save_Item_Add_Viewer(int ItemID, string Viewer_Type, string Viewer_Label, string Viewer_Attributes )
        {
            return Save_Item_Add_Viewers(ItemID, Viewer_Type, Viewer_Label, Viewer_Attributes, String.Empty, String.Empty, String.Empty,
                String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty,
                String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty);
        }

        /// <summary> Add, or update existing, item viewers linked to an item </summary>
        /// <param name="ItemID">Item ID to associate these icons and downlaods with</param>
        /// <param name="Viewer1_Type"> Viewer type code for the first viewer type in the SobekCM database </param>
        /// <param name="Viewer1_Label"> Label to be displayed for the first viewer of this item </param>
        /// <param name="Viewer1_Attributes"> Optional attributes for the first viewer of this item </param>
        /// <param name="Viewer2_Type"> Viewer type code for the second viewer type in the SobekCM database </param>
        /// <param name="Viewer2_Label"> Label to be displayed for the second viewer of this item </param>
        /// <param name="Viewer2_Attributes"> Optional attributes for the second viewer of this item </param>
        /// <param name="Viewer3_Type"> Viewer type code for the third viewer type in the SobekCM database </param>
        /// <param name="Viewer3_Label"> Label to be displayed for the third viewer of this item </param>
        /// <param name="Viewer3_Attributes"> Optional attributes for the third viewer of this item </param>
        /// <param name="Viewer4_Type"> Viewer type code for the fourth viewer type in the SobekCM database </param>
        /// <param name="Viewer4_Label"> Label to be displayed for the fourth viewer of this item </param>
        /// <param name="Viewer4_Attributes"> Optional attributes for the fourth viewer of this item </param>
        /// <param name="Viewer5_Type"> Viewer type code for the fifth viewer type in the SobekCM database </param>
        /// <param name="Viewer5_Label"> Label to be displayed for the fifth viewer of this item </param>
        /// <param name="Viewer5_Attributes"> Optional attributes for the fifth viewer of this item </param>
        /// <param name="Viewer6_Type"> Viewer type code for the sixth viewer type in the SobekCM database </param>
        /// <param name="Viewer6_Label"> Label to be displayed for the sixth viewer of this item </param>
        /// <param name="Viewer6_Attributes"> Optional attributes for the sixth viewer of this item </param>
        /// <remarks> This method calls the stored procedure 'SobekCM_Add_Item_Viewers'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        protected static bool Save_Item_Add_Viewers(int ItemID,
            string Viewer1_Type, string Viewer1_Label, string Viewer1_Attributes, string Viewer2_Type, string Viewer2_Label, string Viewer2_Attributes,
            string Viewer3_Type, string Viewer3_Label, string Viewer3_Attributes, string Viewer4_Type, string Viewer4_Label, string Viewer4_Attributes,
            string Viewer5_Type, string Viewer5_Label, string Viewer5_Attributes, string Viewer6_Type, string Viewer6_Label, string Viewer6_Attributes)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[19];
                param_list[0] = new EalDbParameter("@ItemID", ItemID);
                param_list[1] = new EalDbParameter("@Viewer1_Type", Viewer1_Type);
                param_list[2] = new EalDbParameter("@Viewer1_Label", Viewer1_Label);
                param_list[3] = new EalDbParameter("@Viewer1_Attribute", Viewer1_Attributes);
                param_list[4] = new EalDbParameter("@Viewer2_Type", Viewer2_Type);
                param_list[5] = new EalDbParameter("@Viewer2_Label", Viewer2_Label);
                param_list[6] = new EalDbParameter("@Viewer2_Attribute", Viewer2_Attributes);
                param_list[7] = new EalDbParameter("@Viewer3_Type", Viewer3_Type);
                param_list[8] = new EalDbParameter("@Viewer3_Label", Viewer3_Label);
                param_list[9] = new EalDbParameter("@Viewer3_Attribute", Viewer3_Attributes);
                param_list[10] = new EalDbParameter("@Viewer4_Type", Viewer4_Type);
                param_list[11] = new EalDbParameter("@Viewer4_Label", Viewer4_Label);
                param_list[12] = new EalDbParameter("@Viewer4_Attribute", Viewer4_Attributes);
                param_list[13] = new EalDbParameter("@Viewer5_Type", Viewer5_Type);
                param_list[14] = new EalDbParameter("@Viewer5_Label", Viewer5_Label);
                param_list[15] = new EalDbParameter("@Viewer5_Attribute", Viewer5_Attributes);
                param_list[16] = new EalDbParameter("@Viewer6_Type", Viewer6_Type);
                param_list[17] = new EalDbParameter("@Viewer6_Label", Viewer6_Label);
                param_list[18] = new EalDbParameter("@Viewer6_Attribute", Viewer6_Attributes);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Add_Item_Viewers", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Add_Item_Viewers", ee);
                return false;
            }
        }

        /// <summary> Remove (and exclude) item viewers linked to an item </summary>
        /// <param name="ItemID">Item ID to associate these icons and downlaods with</param>
        /// <param name="Viewer1_Type"> Viewer type code for the first viewer type in the SobekCM database to exlude </param>
        /// <param name="Viewer2_Type"> Viewer type code for the second viewer type in the SobekCM database to exlude </param>
        /// <param name="Viewer3_Type"> Viewer type code for the third viewer type in the SobekCM database to exlude </param>
        /// <param name="Viewer4_Type"> Viewer type code for the fourth viewer type in the SobekCM database to exlude </param>
        /// <param name="Viewer5_Type"> Viewer type code for the fifth viewer type in the SobekCM database to exlude </param>
        /// <param name="Viewer6_Type"> Viewer type code for the sixth viewer type in the SobekCM database to exlude </param>
        /// <remarks> This method calls the stored procedure 'SobekCM_Remove_Item_Viewers'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        protected static bool Save_Item_Remove_Viewers(int ItemID, string Viewer1_Type, string Viewer2_Type, string Viewer3_Type,
            string Viewer4_Type, string Viewer5_Type, string Viewer6_Type)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[7];
                param_list[0] = new EalDbParameter("@ItemID", ItemID);
                param_list[1] = new EalDbParameter("@Viewer1_Type", Viewer1_Type);
                param_list[2] = new EalDbParameter("@Viewer2_Type", Viewer2_Type);
                param_list[3] = new EalDbParameter("@Viewer3_Type", Viewer3_Type);
                param_list[4] = new EalDbParameter("@Viewer4_Type", Viewer4_Type);
                param_list[5] = new EalDbParameter("@Viewer5_Type", Viewer5_Type);
                param_list[6] = new EalDbParameter("@Viewer6_Type", Viewer6_Type);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Remove_Item_Viewers", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Remove_Item_Viewers", ee);
                return false;
            }
        }

        /// <summary> Saves up to five ticklers for a single item in a SobekCM digital library </summary>
        /// <param name="ItemID"> Item ID to associate these ticklers with </param>
        /// <param name="Tickler1"> Tickler to save for this item </param>
        /// <param name="Tickler2"> Tickler to save for this item </param>
        /// <param name="Tickler3"> Tickler to save for this item </param>
        /// <param name="Tickler4"> Tickler to save for this item </param>
        /// <param name="Tickler5"> Tickler to save for this item </param>
        /// <remarks> This method calls the stored procedure 'SobekCM_Save_Item_Ticklers'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        protected static bool Save_Item_Ticklers(int ItemID, string Tickler1, string Tickler2, string Tickler3, string Tickler4, string Tickler5)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[6];
                param_list[0] = new EalDbParameter("@ItemID", ItemID);
                param_list[1] = new EalDbParameter("@Tickler1", Tickler1);
                param_list[2] = new EalDbParameter("@Tickler2", Tickler2);
                param_list[3] = new EalDbParameter("@Tickler3", Tickler3);
                param_list[4] = new EalDbParameter("@Tickler4", Tickler4);
                param_list[5] = new EalDbParameter("@Tickler5", Tickler5);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Save_Item_Ticklers", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Save_Item_Ticklers", ee);
                return false;
            }
        }


        /// <summary> Adds up to three additional views for a single item in a SobekCM digital library </summary>
        /// <param name="BibID"> Bibliographic identifier for the item to add </param>
        /// <param name="VID"> Volume identifier for the item to add </param>
        /// <param name="Viewer1_Type"> Primary key for the first viewer type in the SobekCM database </param>
        /// <param name="Viewer1_Label"> Label to be displayed for the first viewer of this item </param>
        /// <param name="Viewer1_Attributes"> Optional attributes for the first viewer of this item </param>
        /// <param name="Viewer2_Type"> Primary key for the second viewer type in the SobekCM database </param>
        /// <param name="Viewer2_Label"> Label to be displayed for the second viewer of this item </param>
        /// <param name="Viewer2_Attributes"> Optional attributes for the second viewer of this item </param>
        /// <param name="Viewer3_Type"> Primary key for the third viewer type in the SobekCM database </param>
        /// <param name="Viewer3_Label"> Label to be displayed for the third viewer of this item </param>
        /// <param name="Viewer3_Attributes"> Optional attributes for the third viewer of this item </param>
        /// <remarks> This method calls the stored procedure 'SobekCM_Save_Item_Views'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        public static bool Save_Item_Views(string BibID, string VID,
            int Viewer1_Type, string Viewer1_Label, string Viewer1_Attributes, int Viewer2_Type, string Viewer2_Label, string Viewer2_Attributes,
            int Viewer3_Type, string Viewer3_Label, string Viewer3_Attributes)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[11];
                param_list[0] = new EalDbParameter("@BibID", BibID);
                param_list[1] = new EalDbParameter("@VID", VID);
                param_list[2] = new EalDbParameter("@Viewer1_TypeID", Viewer1_Type);
                param_list[3] = new EalDbParameter("@Viewer1_Label", Viewer1_Label);
                param_list[4] = new EalDbParameter("@Viewer1_Attribute", Viewer1_Attributes);
                param_list[5] = new EalDbParameter("@Viewer2_TypeID", Viewer2_Type);
                param_list[6] = new EalDbParameter("@Viewer2_Label", Viewer2_Label);
                param_list[7] = new EalDbParameter("@Viewer2_Attribute", Viewer2_Attributes);
                param_list[8] = new EalDbParameter("@Viewer3_TypeID", Viewer3_Type);
                param_list[9] = new EalDbParameter("@Viewer3_Label", Viewer3_Label);
                param_list[10] = new EalDbParameter("@Viewer3_Attribute", Viewer3_Attributes);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Save_Item_Views", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Save_Item_Views", ee);
                return false;
            }
        }

        /// <summary> Mass updates the behaviors for all items within a single item group </summary>
        /// <param name="GroupID">GroupID for the item group to mass update all item behaviors for </param>
        /// <param name="IP_Restriction_Mask">IP Restriction mask for this item </param>
        /// <param name="CheckoutRequired"> Flag indicates if this is a single-use item which requires checkout</param>
        /// <param name="AggregationCode1"> Code for the first aggregation this item belongs to </param>
        /// <param name="AggregationCode2"> Code for the second aggregation this item belongs to </param>
        /// <param name="AggregationCode3"> Code for the third aggregation this item belongs to </param>
        /// <param name="AggregationCode4"> Code for the fourth aggregation this item belongs to </param>
        /// <param name="AggregationCode5"> Code for the fifth aggregation this item belongs to </param>
        /// <param name="AggregationCode6"> Code for the sixth aggregation this item belongs to </param>
        /// <param name="AggregationCode7"> Code for the seventh aggregation this item belongs to </param>
        /// <param name="AggregationCode8"> Code for the eighth aggregation this item belongs to </param>
        /// <param name="HoldingCode"> Holding code for this item's holding location aggregation </param>
        /// <param name="SourceCode"> Source location code for this item's source location </param>
        /// <param name="Icon1_Name">Name of the first icon</param>
        /// <param name="Icon2_Name">Name of the second icon</param>
        /// <param name="Icon3_Name">Name of the third icon</param>
        /// <param name="Icon4_Name">Name of the fourth icon</param>
        /// <param name="Icon5_Name">Name of the fifth icon</param>
        /// <param name="Viewer1_Type"> Primary key for the first viewer type in the SobekCM database </param>
        /// <param name="Viewer1_Label"> Label to be displayed for the first viewer of this item </param>
        /// <param name="Viewer1_Attributes"> Optional attributes for the first viewer of this item </param>
        /// <param name="Viewer2_Type"> Primary key for the second viewer type in the SobekCM database </param>
        /// <param name="Viewer2_Label"> Label to be displayed for the second viewer of this item </param>
        /// <param name="Viewer2_Attributes"> Optional attributes for the second viewer of this item </param>
        /// <param name="Viewer3_Type"> Primary key for the third viewer type in the SobekCM database </param>
        /// <param name="Viewer3_Label"> Label to be displayed for the third viewer of this item </param>
        /// <param name="Viewer3_Attributes"> Optional attributes for the third viewer of this item </param>
        /// <param name="Viewer4_Type"> Primary key for the fourth viewer type in the SobekCM database </param>
        /// <param name="Viewer4_Label"> Label to be displayed for the fourth viewer of this item </param>
        /// <param name="Viewer4_Attributes"> Optional attributes for the fourth viewer of this item </param>
        /// <param name="Viewer5_Type"> Primary key for the fifth viewer type in the SobekCM database </param>
        /// <param name="Viewer5_Label"> Label to be displayed for the fifth viewer of this item </param>
        /// <param name="Viewer5_Attributes"> Optional attributes for the fifth viewer of this item </param>
        /// <param name="Viewer6_Type"> Primary key for the sixth viewer type in the SobekCM database </param>
        /// <param name="Viewer6_Label"> Label to be displayed for the sixth viewer of this item </param>
        /// <param name="Viewer6_Attributes"> Optional attributes for the sixth viewer of this item </param>
        /// <param name="Born_Digital"> Flag indicates this item was born digitally, rather than scanned in-house</param>
        /// <param name="Dark_Flag"> Flag indicates this item is permanently DARK and can not be made public without first un-darking the item </param>
        /// <param name="Set_Born_Digital"> Flag indicates whether the born digital flag should be set for all items in this title </param>
        /// <param name="Set_CheckoutRequired"> Flag indicates whether the checkout required flag should be set for all items in this title </param>
        /// <param name="Set_Dark_Flag"> Flag indicates if the dark flag should be set for all items in this title </param>
        /// <param name="Set_IP_Restriction_Mask"> Flag indicates if the IP restriction mask should be set for all items in this title </param>
        /// <remarks> This method calls the stored procedure 'SobekCM_Mass_Update_Item_Behaviors2'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        protected static bool Mass_Update_Item_Behaviors(int GroupID, bool Set_IP_Restriction_Mask, short IP_Restriction_Mask,
            bool Set_CheckoutRequired, bool CheckoutRequired, bool Set_Dark_Flag, bool Dark_Flag,
            bool Set_Born_Digital, bool Born_Digital, string AggregationCode1, string AggregationCode2,
            string AggregationCode3, string AggregationCode4, string AggregationCode5, string AggregationCode6,
            string AggregationCode7, string AggregationCode8, string HoldingCode, string SourceCode,
            string Icon1_Name, string Icon2_Name, string Icon3_Name, string Icon4_Name, string Icon5_Name,
            string Viewer1_Type, string Viewer1_Label, string Viewer1_Attributes, string Viewer2_Type, string Viewer2_Label, string Viewer2_Attributes,
            string Viewer3_Type, string Viewer3_Label, string Viewer3_Attributes, string Viewer4_Type, string Viewer4_Label, string Viewer4_Attributes,
            string Viewer5_Type, string Viewer5_Label, string Viewer5_Attributes, string Viewer6_Type, string Viewer6_Label, string Viewer6_Attributes)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[38];
                param_list[0] = new EalDbParameter("@GroupID", GroupID);

                if (Set_IP_Restriction_Mask)
                    param_list[1] = new EalDbParameter("@IP_Restriction_Mask", IP_Restriction_Mask);
                else
                    param_list[1] = new EalDbParameter("@IP_Restriction_Mask", DBNull.Value);

                if (Set_CheckoutRequired)
                    param_list[2] = new EalDbParameter("@CheckoutRequired", CheckoutRequired);
                else
                    param_list[2] = new EalDbParameter("@CheckoutRequired", DBNull.Value);

                if (Set_Dark_Flag)
                    param_list[3] = new EalDbParameter("@Dark_Flag", Dark_Flag);
                else
                    param_list[3] = new EalDbParameter("@Dark_Flag", DBNull.Value);

                if (Set_Born_Digital)
                    param_list[4] = new EalDbParameter("@Born_Digital", Born_Digital);
                else
                    param_list[4] = new EalDbParameter("@Born_Digital", DBNull.Value);

                param_list[5] = new EalDbParameter("@AggregationCode1", AggregationCode1);
                param_list[6] = new EalDbParameter("@AggregationCode2", AggregationCode2);
                param_list[7] = new EalDbParameter("@AggregationCode3", AggregationCode3);
                param_list[8] = new EalDbParameter("@AggregationCode4", AggregationCode4);
                param_list[9] = new EalDbParameter("@AggregationCode5", AggregationCode5);
                param_list[10] = new EalDbParameter("@AggregationCode6", AggregationCode6);
                param_list[11] = new EalDbParameter("@AggregationCode7", AggregationCode7);
                param_list[12] = new EalDbParameter("@AggregationCode8", AggregationCode8);
                param_list[13] = new EalDbParameter("@HoldingCode", HoldingCode);
                param_list[14] = new EalDbParameter("@SourceCode", SourceCode);
                param_list[15] = new EalDbParameter("@Icon1_Name", Icon1_Name);
                param_list[16] = new EalDbParameter("@Icon2_Name", Icon2_Name);
                param_list[17] = new EalDbParameter("@Icon3_Name", Icon3_Name);
                param_list[18] = new EalDbParameter("@Icon4_Name", Icon4_Name);
                param_list[19] = new EalDbParameter("@Icon5_Name", Icon5_Name);
                param_list[20] = new EalDbParameter("@Viewer1_Type", Viewer1_Type);
                param_list[21] = new EalDbParameter("@Viewer1_Label", Viewer1_Label);
                param_list[22] = new EalDbParameter("@Viewer1_Attribute", Viewer1_Attributes);
                param_list[23] = new EalDbParameter("@Viewer2_Type", Viewer2_Type);
                param_list[24] = new EalDbParameter("@Viewer2_Label", Viewer2_Label);
                param_list[25] = new EalDbParameter("@Viewer2_Attribute", Viewer2_Attributes);
                param_list[26] = new EalDbParameter("@Viewer3_Type", Viewer3_Type);
                param_list[27] = new EalDbParameter("@Viewer3_Label", Viewer3_Label);
                param_list[28] = new EalDbParameter("@Viewer3_Attribute", Viewer3_Attributes);
                param_list[29] = new EalDbParameter("@Viewer4_Type", Viewer4_Type);
                param_list[30] = new EalDbParameter("@Viewer4_Label", Viewer4_Label);
                param_list[31] = new EalDbParameter("@Viewer4_Attribute", Viewer4_Attributes);
                param_list[32] = new EalDbParameter("@Viewer5_Type", Viewer5_Type);
                param_list[33] = new EalDbParameter("@Viewer5_Label", Viewer5_Label);
                param_list[34] = new EalDbParameter("@Viewer5_Attribute", Viewer5_Attributes);
                param_list[35] = new EalDbParameter("@Viewer6_Type", Viewer6_Type);
                param_list[36] = new EalDbParameter("@Viewer6_Label", Viewer6_Label);
                param_list[37] = new EalDbParameter("@Viewer6_Attribute", Viewer6_Attributes);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Mass_Update_Item_Behaviors", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Mass_Update_Item_Behaviors2", ee);
                return false;
            }
        }

        /// <summary> Saves the serial hierarchy and link between an item and an item group </summary>
        /// <param name="GroupID">Group ID this item belongs to</param>
        /// <param name="ItemID">Item id for the individual item</param>
        /// <param name="Level1_Text">Text to display for the first hierarchy</param>
        /// <param name="Level1_Index">Order for this item within other first hierarchy items</param>
        /// <param name="Level2_Text">Text to display for the second hierarchy</param>
        /// <param name="Level2_Index">Order for this item within other second hierarchy items</param>
        /// <param name="Level3_Text">Text to display for the third hierarchy</param>
        /// <param name="Level3_Index">Order for this item within other third hierarchy items</param>
        /// <param name="Level4_Text">Text to display for the fourth hierarchy</param>
        /// <param name="Level4_Index">Order for this item within other fourth hierarchy items</param>
        /// <param name="Level5_Text">Text to display for the fifth hierarchy</param>
        /// <param name="Level5_Index">Order for this item within other fifth hierarchy items</param>
        /// <param name="SerialHierarchy"> Serial hierarchy as a single</param>
        /// <remarks> This method calls the stored procedure 'SobekCM_Save_Serial_Hierarchy'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        protected static void Save_Serial_Hierarchy(int GroupID, int ItemID, string Level1_Text, int Level1_Index, string Level2_Text, int Level2_Index, string Level3_Text, int Level3_Index, string Level4_Text, int Level4_Index, string Level5_Text, int Level5_Index, string SerialHierarchy)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[13];
                param_list[0] = new EalDbParameter("@GroupID", GroupID);
                param_list[1] = new EalDbParameter("@ItemID", ItemID);
                if (Level1_Index >= 0)
                {
                    param_list[2] = new EalDbParameter("@Level1_Text", Level1_Text);
                    param_list[3] = new EalDbParameter("@Level1_Index", Level1_Index);
                }
                else
                {
                    param_list[2] = new EalDbParameter("@Level1_Text", DBNull.Value);
                    param_list[3] = new EalDbParameter("@Level1_Index", DBNull.Value);
                }

                if (Level2_Index >= 0)
                {
                    param_list[4] = new EalDbParameter("@Level2_Text", Level2_Text);
                    param_list[5] = new EalDbParameter("@Level2_Index", Level2_Index);
                }
                else
                {
                    param_list[4] = new EalDbParameter("@Level2_Text", DBNull.Value);
                    param_list[5] = new EalDbParameter("@Level2_Index", DBNull.Value);
                }

                if (Level3_Index >= 0)
                {
                    param_list[6] = new EalDbParameter("@Level3_Text", Level3_Text);
                    param_list[7] = new EalDbParameter("@Level3_Index", Level3_Index);
                }
                else
                {
                    param_list[6] = new EalDbParameter("@Level3_Text", DBNull.Value);
                    param_list[7] = new EalDbParameter("@Level3_Index", DBNull.Value);
                }

                if (Level4_Index >= 0)
                {
                    param_list[8] = new EalDbParameter("@Level4_Text", Level4_Text);
                    param_list[9] = new EalDbParameter("@Level4_Index", Level4_Index);
                }
                else
                {
                    param_list[8] = new EalDbParameter("@Level4_Text", DBNull.Value);
                    param_list[9] = new EalDbParameter("@Level4_Index", DBNull.Value);
                }

                if (Level5_Index >= 0)
                {
                    param_list[10] = new EalDbParameter("@Level5_Text", Level5_Text);
                    param_list[11] = new EalDbParameter("@Level5_Index", Level5_Index);
                }
                else
                {
                    param_list[10] = new EalDbParameter("@Level5_Text", DBNull.Value);
                    param_list[11] = new EalDbParameter("@Level5_Index", DBNull.Value);
                }

                param_list[12] = new EalDbParameter("@SerialHierarchy", SerialHierarchy);


                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Save_Serial_Hierarchy", param_list);
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Save_Serial_Hierarchy", ee);
            }
        }

        #endregion

        #endregion

        #region Methods to change one or two elements about an single digital resource or title


        /// <summary>
        /// Updates the relevant item info in the database after QC (main thumbnail info, pagecount, filecount, disksize)
        /// </summary>
        /// <param name="BibID"></param>BibID for the item
        /// <param name="VID"></param>VID for this item
        /// <param name="MainThumbnailFileName"></param>Filename of the main thumbnail image (.thm extension)
        /// <param name="MainJpgFileName"></param>Filename of the main thumbnail JPEG image (.jpg extension)
        /// <param name="PageCount"></param>Updated count of the pages for this item
        /// <param name="FileCount"></param>Total count of all the files for the pages of this item
        /// <param name="DisksizeMb"></param>Total disk space occupied by all the files of this item
        /// <param name="Notes"></param>Notes/Comments entered by the user through the QC interface
        /// <param name="User"></param>Logged in user info
        /// <returns></returns>
        public static bool QC_Update_Item_Info(string BibID, string VID, string User, string MainThumbnailFileName, string MainJpgFileName, int PageCount, int FileCount, double DisksizeMb, string Notes)
        {
            try
            {
                int itemID = Get_ItemID(BibID, VID);
                //Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[8];
                param_list[0] = new EalDbParameter("@itemid", itemID);
                param_list[1] = new EalDbParameter("@notes", Notes);
                param_list[2] = new EalDbParameter("@onlineuser", User);
                param_list[3] = new EalDbParameter("@mainthumbnail", MainThumbnailFileName);
                param_list[4] = new EalDbParameter("@mainjpeg", MainJpgFileName);
                param_list[5] = new EalDbParameter("@pagecount", PageCount);
                param_list[6] = new EalDbParameter("@filecount", FileCount);
                param_list[7] = new EalDbParameter("@disksize_kb", DisksizeMb);


                //Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "Tracking_Submit_Online_Page_Division", param_list);

                return true;

            }
            catch (Exception e)
            {
                //Pass this exception onto the method to handle it
                exception_caught("Tracking_Submit_Online_Page_Division", e);
                return false;
            }
        }


        /// <summary> Sets the general visibility and embargo information for a single item </summary>
        /// <param name="ItemID"></param>
        /// <param name="NewRestrictionMask"></param>
        /// <param name="DarkFlag"></param>
        /// <param name="EmbargoDate"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        /// <remarks> This method calls the stored procedure 'SobekCM_Set_Item_Visibility'. </remarks>
        public static bool Set_Item_Visibility(int ItemID, int NewRestrictionMask, bool DarkFlag, DateTime? EmbargoDate, string UserName)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[5];
                param_list[0] = new EalDbParameter("@ItemID", ItemID);
                param_list[1] = new EalDbParameter("@IpRestrictionMask", NewRestrictionMask);
                param_list[2] = new EalDbParameter("@DarkFlag", DarkFlag);

                if (EmbargoDate.HasValue)
                    param_list[3] = new EalDbParameter("@EmbargoDate", EmbargoDate.Value);
                else
                    param_list[3] = new EalDbParameter("@EmbargoDate", DBNull.Value);

                param_list[4] = new EalDbParameter("@User", UserName);


                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Set_Item_Visibility", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Set_IP_Restriction_Mask", ee);
                return false;
            }
        }


        /// <summary> Saves the IP Restriction mask which determines public, private, or restricted by IP </summary>
        /// <param name="ItemID">Item ID for the item to save the new visibility for </param>
        /// <param name="New_Restriction_Mask"> New value for the visibility of this item </param>
        /// <param name="UserName"> Username that performed the change </param>
        /// <param name="ProgressNote"> Note to attach to this change in item's accessibility</param>
        /// <remarks> This method calls the stored procedure 'SobekCM_Set_IP_Restriction_Mask'. </remarks>
        public static bool Set_IP_Restriction_Mask(int ItemID, int New_Restriction_Mask, string UserName, string ProgressNote)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[4];
                param_list[0] = new EalDbParameter("@itemid", ItemID);
                param_list[1] = new EalDbParameter("@newipmask", New_Restriction_Mask);
                param_list[2] = new EalDbParameter("@user", UserName);
                param_list[3] = new EalDbParameter("@progressnote", ProgressNote);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Set_IP_Restriction_Mask", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Set_IP_Restriction_Mask", ee);
                return false;
            }
        }

        /// <summary> Saves the internal comments related to an item </summary>
        /// <param name="ItemID">Item ID to associate this internal comment with </param>
        /// <param name="Internal_Comments">New internal comments</param>
        /// <remarks> This method calls the stored procedure 'SobekCM_Set_Item_Comments'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        public static bool Save_Item_Internal_Comments(int ItemID, string Internal_Comments)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[2];
                param_list[0] = new EalDbParameter("@itemid", ItemID);
                param_list[1] = new EalDbParameter("@newcomments", Internal_Comments);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Set_Item_Comments", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Set_Item_Comments", ee);
                return false;
            }
        }

        /// <summary> Saves the basic title information for an item group </summary>
        /// <param name="BibID"> Bibliographic identifier for the item group to update </param>
        /// <param name="GroupTitle"> New group title for this item group </param>
        /// <param name="SortTitle"> New sort title for this item group </param>
        /// <param name="Group_Thumbnail"> Thumbnail to use for this item group </param>
        /// <param name="Primary_Identifier_Type"> Primary identifier type for this item group </param>
        /// <param name="Primary_Identifier"> Primary identifier for this item group </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Update_Item_Group' stored procedure </remarks> 
        public static bool Update_Item_Group(string BibID, string GroupTitle, string SortTitle, string Group_Thumbnail, string Primary_Identifier_Type, string Primary_Identifier)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[6];
                param_list[0] = new EalDbParameter("@bibid", BibID);
                param_list[1] = new EalDbParameter("@grouptitle", GroupTitle);
                param_list[2] = new EalDbParameter("@sorttitle", SortTitle);
                param_list[3] = new EalDbParameter("@groupthumbnail", Group_Thumbnail);
                param_list[4] = new EalDbParameter("@PrimaryIdentifierType", Primary_Identifier_Type);
                param_list[5] = new EalDbParameter("@PrimaryIdentifier", Primary_Identifier);


                // Execute this query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Update_Item_Group", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Save_Item_Group_Title", ee);
                return false;
            }
        }

        /// <summary> Saves the tracking box information for a single item </summary>
        /// <param name="ItemID">Item ID to associate this tracking box with </param>
        /// <param name="Tracking_Box">New tracking box</param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This method calls the stored procedure 'Tracking_Update_Tracking_Box'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        public static bool Save_New_Tracking_Box(int ItemID, string Tracking_Box)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[2];
                param_list[0] = new EalDbParameter("@Tracking_Box", Tracking_Box);
                param_list[1] = new EalDbParameter("@ItemID", ItemID);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "Tracking_Update_Tracking_Box", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("Tracking_Update_Tracking_Box", ee);
                return false;
            }
        }

        /// <summary> Update the disposition advice on what to do when the physical material leaves the digitization location </summary>
        /// <param name="ItemID"> ItemID for which to update the disposition advice </param>
        /// <param name="DispositionTypeID"> Primary key to the disposition type from the database </param>
        /// <param name="Notes"> Any associated notes included about the disposition advice </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This method calls the stored procedure 'Tracking_Update_Disposition_Advice'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        public static bool Edit_Disposition_Advice(int ItemID, int DispositionTypeID, string Notes)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[3];
                param_list[0] = new EalDbParameter("@Disposition_Advice", DispositionTypeID);
                param_list[1] = new EalDbParameter("@Disposition_Advice_Notes", Notes);
                param_list[2] = new EalDbParameter("@ItemID", ItemID);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "Tracking_Update_Disposition_Advice", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("Tracking_Update_Disposition_Advice", ee);
                return false;
            }
        }

        /// <summary> Update the disposition information when the physical material leaves the digitization location </summary>
        /// <param name="ItemID"> ItemID for which to update the disposition </param>
        /// <param name="DispositionTypeID"> Primary key to the disposition type from the database </param>
        /// <param name="Notes"> Any notes associated with the disposition </param>
        /// <param name="DispositionDate"> Date the disposition occurred </param>
        /// <param name="UserName"> User who added this disposition information (for the work history)</param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This method calls the stored procedure 'Tracking_Update_Disposition'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        public static bool Update_Disposition(int ItemID, int DispositionTypeID, string Notes, DateTime DispositionDate, string UserName)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[5];
                param_list[0] = new EalDbParameter("@Disposition_Date", DispositionDate);
                param_list[1] = new EalDbParameter("@Disposition_Type", DispositionTypeID);
                param_list[2] = new EalDbParameter("@Disposition_Notes", Notes);
                param_list[3] = new EalDbParameter("@ItemID", ItemID);
                param_list[4] = new EalDbParameter("@UserName", UserName);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "Tracking_Update_Disposition", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("Tracking_Update_Disposition", ee);
                return false;
            }
        }

        /// <summary> Add a worklog history entry for some previous datetime </summary>
        /// <param name="ItemID"> ItemID for which to add a workflow history entry </param>
        /// <param name="Workflow_Type"> Name of the workflow to add to this item  </param>
        /// <param name="Notes"> Any notes associated with the workflow </param>
        /// <param name="Date"> Date the workflow occurred </param>
        /// <param name="UserName"> User who added this worklog enty </param>
        /// <param name="StorageLocation"> Location this work occured on the network or detached drives </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This method calls the stored procedure 'Tracking_Add_Past_Workflow_By_ItemID'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        public static bool Add_Past_Workflow(int ItemID, string Workflow_Type, string Notes, DateTime Date, string UserName, string StorageLocation)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[6];
                param_list[0] = new EalDbParameter("@itemid", ItemID);
                param_list[1] = new EalDbParameter("@user", UserName);
                param_list[2] = new EalDbParameter("@progressnote", Notes);
                param_list[3] = new EalDbParameter("@workflow", Workflow_Type);
                param_list[4] = new EalDbParameter("@storagelocation", StorageLocation);
                param_list[5] = new EalDbParameter("@date", Date);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "Tracking_Add_Past_Workflow_By_ItemID", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("Tracking_Add_Past_Workflow_By_ItemID", ee);
                return false;
            }
        }


        /// <summary> Add a worklog entry for a current event </summary>
        /// <param name="ItemID"> ItemID for which to add a workflow history entry </param>
        /// <param name="Workflow_Type"> Name of the workflow to add to this item  </param>
        /// <param name="Notes"> Any notes associated with the workflow </param>
        /// <param name="UserName"> User who added this worklog enty </param>
        /// <param name="StorageLocation"> Location this work occured on the network or detached drives </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This method calls the stored procedure 'Tracking_Add_Workflow_By_ItemID'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        public static bool Add_Workflow(int ItemID, string Workflow_Type, string Notes, string UserName, string StorageLocation)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[5];
                param_list[0] = new EalDbParameter("@itemid", ItemID);
                param_list[1] = new EalDbParameter("@user", UserName);
                param_list[2] = new EalDbParameter("@progressnote", Notes);
                param_list[3] = new EalDbParameter("@workflow", Workflow_Type);
                param_list[4] = new EalDbParameter("@storagelocation", StorageLocation);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "Tracking_Add_Workflow_By_ItemID", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("Tracking_Add_Past_Workflow_By_ItemID", ee);
                return false;
            }
        }

        /// <summary> Update the digitization milestones </summary>
        /// <param name="ItemID"> Primary key for the item to update the digitization miliestons</param>
        /// <param name="Last_Milestone"> Last milesont completed ( 1=Digital Acquisition Copmlete, 2=Post-Acquisition Processing Complete, 3=QC Complete, 4=Online Complete )</param>
        /// <param name="Milestone_Date"> Date the last milestone was performed </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This method calls the stored procedure 'Tracking_Update_Digitization_Milestones'. </remarks>
        public static bool Update_Digitization_Milestone(int ItemID, int Last_Milestone, DateTime Milestone_Date)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[3];
                param_list[0] = new EalDbParameter("@ItemID", ItemID);
                param_list[1] = new EalDbParameter("@Last_Milestone", Last_Milestone);
                param_list[2] = new EalDbParameter("@Milestone_Date", Milestone_Date);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "Tracking_Update_Digitization_Milestones", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("Tracking_Update_Digitization_Milestones", ee);
                return false;
            }
        }

        /// <summary> Update information regarding the receipt of material </summary>
        /// <param name="ItemID"> ItemID for which to add material receipt information </param>
        /// <param name="Date"> Date the material was received </param>
        /// <param name="Estimated"> Flag indicates if this material receipt is estimated </param>
        /// <param name="UserName"> Name of the user marking the material received </param>
        /// <param name="Notes"> Notes regarding the receipt of the material </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This method calls the stored procedure 'Tracking_Update_Material_Received'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        public static bool Update_Material_Received(int ItemID, DateTime Date, bool Estimated, string UserName, string Notes)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[5];
                param_list[0] = new EalDbParameter("@Material_Received_Date", Date);
                param_list[1] = new EalDbParameter("@Material_Recd_Date_Estimated", Estimated);
                param_list[2] = new EalDbParameter("@ItemID", ItemID);
                param_list[3] = new EalDbParameter("@User", UserName);
                param_list[4] = new EalDbParameter("@ProgressNote", Notes);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "Tracking_Update_Material_Received", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("Tracking_Update_Material_Received", ee);
                return false;
            }
        }

        /// <summary> Update the born digital flag for an item </summary>
        /// <param name="ItemID"> ItemID for which to update born digital flag </param>
        /// <param name="Born_Digital_Flag"> New value for the born digital flag on this item </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This method calls the stored procedure 'Tracking_Update_Born_Digital'. </remarks>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        public static bool Update_Born_Digital_Flag(int ItemID, bool Born_Digital_Flag)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[2];
                param_list[0] = new EalDbParameter("@Born_Digital", Born_Digital_Flag);
                param_list[1] = new EalDbParameter("@ItemID", ItemID);

                // Execute this non-query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "Tracking_Update_Born_Digital", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("Tracking_Update_Born_Digital", ee);
                return false;
            }
        }


        #endregion

        #region Method to update the physical statistics for an item in a SobekCM library

        /// <summary> Update the physical statistics ( files, pages, size ) of an item </summary>
        /// <param name="BibID"> Bibliographic identifier for the item group to update </param>
        /// <param name="VID"> Volume identifier for the item/volume to update </param>
        /// <param name="PageCount"> Number of pages linked to this item in the library </param>
        /// <param name="FileCount"> Number of page image files linked to this item in the library </param>
        /// <param name="DiskSizeMb"> Size of the entire digital resource on the image server  </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Update_Item_Online_Statistics' stored procedure </remarks> 
        public static bool Update_Item_Online_Statistics(string BibID, string VID, int PageCount, int FileCount, double DiskSizeMb)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[5];
                param_list[0] = new EalDbParameter("@bibid", BibID);
                param_list[1] = new EalDbParameter("@vid", VID);
                param_list[2] = new EalDbParameter("@pagecount", PageCount);
                param_list[3] = new EalDbParameter("@filecount", FileCount);
                param_list[4] = new EalDbParameter("@disksize_kb", DiskSizeMb);

                // Execute this query stored procedure
                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Update_Item_Online_Statistics", param_list);

                return true;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Update_Item_Online_Statistics", ee);
                return false;
            }
        }

        #endregion

        #region Method to check for existence of a record in a SobekCM library or get ItemID

        /// <summary> Gets the item id for an resource in a SobekCM library, by BibID and VID </summary>
        /// <param name="BibID"> Bibliographic identifier to check </param>
        /// <param name="VID"> Volume identifier to check </param>
        /// <returns> ItemID for this item from the database, or -1 </returns>
        /// <remarks> This calls the 'SobekCM_Get_ItemID' stored procedure </remarks> 
        public static int Get_ItemID(string BibID, string VID)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[2];
                param_list[0] = new EalDbParameter("@bibid", BibID);
                param_list[1] = new EalDbParameter("@vid", VID);

                // Execute this query stored procedure
                DataSet resultSet = EalDbAccess.ExecuteDataset(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Get_ItemID", param_list);

                if ((resultSet != null) && (resultSet.Tables[0].Rows.Count > 0))
                    return Convert.ToInt32(resultSet.Tables[0].Rows[0][0]);

                return -1;
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Get_ItemID", ee);
                return -1;
            }
        }

        /// <summary> Checks for similar records within the database </summary>
        /// <param name="BibID"> Bibliographic identifier to check </param>
        /// <param name="VID"> Volume identifier to check </param>
        /// <param name="OCLC"> OCLC Record number to look for existence </param>
        /// <param name="Local_Catalog_ID"> Local catalog record number to look for existence </param>
        /// <returns> DataTable with any matching results </returns>
        /// <remarks> This calls the 'SobekCM_Check_For_Record_Existence' stored procedure </remarks> 
        public static DataTable Check_For_Record_Existence(string BibID, string VID, string OCLC, string Local_Catalog_ID)
        {
            // Get oclc and/or aleph number
            long oclc = -999;
            int aleph = -999;
            if (OCLC.Length > 0)
            {
                long oclc_temp;
                if (Int64.TryParse(OCLC, out oclc_temp))
                    oclc = oclc_temp;
            }
            if (Local_Catalog_ID.Length > 0)
            {
                int catalog_temp;
                if (Int32.TryParse(Local_Catalog_ID, out catalog_temp))
                    aleph = catalog_temp;
            }

            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[4];
                param_list[0] = new EalDbParameter("@bibid", BibID);
                param_list[1] = new EalDbParameter("@vid", VID);
                param_list[2] = new EalDbParameter("@OCLC_Number", oclc);
                param_list[3] = new EalDbParameter("@Local_Cat_Number", aleph);

                // Execute this query stored procedure
                DataSet resultSet = EalDbAccess.ExecuteDataset(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_Check_For_Record_Existence", param_list);

                if (resultSet != null)
                    return resultSet.Tables[0];
            }
            catch (Exception ee)
            {
                // Pass this exception onto the method to handle it
                exception_caught("SobekCM_Check_For_Record_Existence", ee);
            }

            return null;
        }

        #endregion

        #region Custom Public Properties



        /// <summary> Gets and sets the connection string to the SobekCM database </summary>
        public static string Connection_String
        {
            get
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
            }
        }


        #endregion

        #region Internal Flags

        /// <summary> Flag indicates whether exceptions should be thrown </summary>
        /// <remarks> If this flag is set to TRUE, a <see cref="SobekCM_Database_Exception"/> 
        /// will be thrown if any error occurs while accessing the database. </remarks>
        protected static bool ThrowExceptions = true;

        /// <summary> Flag indicates whether a message should be displayed when
        /// errors occur. </summary>
        /// <remarks> Set this flag to TRUE to show a message box when errors occur. </remarks>
        protected static bool DisplayErrors = false;

        /// <summary> Flag indicates if the text of the internal exception should
        /// be included in any message or exception thrown.  </summary>
        /// <remarks> Set to TRUE to show the text from the inner exception. </remarks>
        protected static bool DisplayInnerExceptions = false;

        /// <summary> Error string displayed in the case of an error </summary>
        private const string ERROR_STRING = "Error while executing stored procedure '{0}'.       ";

        #endregion

        #region Helper methods and classes

        /// <summary> Method is called when an exception is caught while accessing the database. </summary>
        /// <param name="StoredProcedureName"> Name of the stored procedure called </param>
        /// <param name="Exception"> Exception caught while accessing the database </param>
        /// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
        /// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
        private static void exception_caught(string StoredProcedureName, Exception Exception)
        {
            // Determine the text to either show or throw
            string exception_text = string.Format(ERROR_STRING, StoredProcedureName);

            // Show the internal error if that flag is set
            if (DisplayInnerExceptions)
                exception_text = exception_text + "\n\n" + Exception;

            //// If display is set, then display the errors
            //if ( DISPLAY_ERRORS )
            //{
            //    MessageBox.Show( exception_text, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            //}

            // If an exception should be thrown, throw it
            if (ThrowExceptions)
            {
                throw new SobekCM_Database_Exception(exception_text);
            }
        }

        /// <summary> SobekCM_Database_Exception is an exception which can be thrown when there
        /// is an error while accessing the database.  This extends the <see cref="ApplicationException"/>
        /// class.  </summary>
        internal class SobekCM_Database_Exception : ApplicationException
        {
            /// <summary> Constructor for a new SobekCM_Database_Exception object </summary>
            /// <param name="ExceptionText"> Text of the exception to be displayed </param>
            public SobekCM_Database_Exception(string ExceptionText)
                : base(ExceptionText)
            {
                // All work completed in the base class
            }
        }

        #endregion

        #region Methods to support HTTP retrieval of item list and METS from a SobekCM instance

        /// <summary> Returns the list of items currently loaded to a SobekCM instance </summary>
        /// <param name="SobekCM_Base_URL">Base SobekCM URL</param>
        /// <returns>DataSet</returns>
        public static SobekCM_All_Items Current_SobekCM_Items(string SobekCM_Base_URL)
        {
            try
            {
                // Create the return value data set
                SobekCM_All_Items returnVal = new SobekCM_All_Items();

                // Create the stream to get the information from the web
                WebResponse objResponse;
                WebRequest objRequest = HttpWebRequest.Create(SobekCM_Base_URL);
                objRequest.Timeout = 15000;
                objResponse = objRequest.GetResponse();

                // Load the data into the DataSet
                returnVal.ReadXml(objResponse.GetResponseStream());

                // Return this value
                return returnVal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary> Downloads a METS file from the SobekCM web page </summary>
        /// <param name="PackageResourceURL">URL for this packages resources</param>
        /// <param name="BIBID">Bib ID for this package</param>
        /// <param name="VID">VID for this package</param>
        /// <returns>METS file as a string</returns>
        public static string Download_METS(string PackageResourceURL, string BIBID, string VID)
        {
            try
            {
                string download_url = PackageResourceURL + "/" + BIBID + "_" + VID + ".METS_Header.xml";
                string mets_file = GetHtmlPage(download_url, 15);
                return mets_file;
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary> Retrive html as a string from a web page </summary>
        /// <param name="StrURL"> URL to use to retrieve the source </param>
        /// <param name="SecondsToTimeOut"> Seconds before the HTML request times out </param>
        /// <returns> Web page as string </returns>
        public static String GetHtmlPage(string StrURL, int SecondsToTimeOut)
        {
            try
            {
                // the html retrieved from the page
                String strResult;
                WebResponse objResponse;
                WebRequest objRequest = HttpWebRequest.Create(StrURL);
                objRequest.Timeout = SecondsToTimeOut * 1000;
                objResponse = objRequest.GetResponse();
                // the using keyword will automatically dispose the object 
                // once complete
                using (StreamReader sr =
                           new StreamReader(objResponse.GetResponseStream()))
                {
                    strResult = sr.ReadToEnd();
                    // Close and clean up the StreamReader
                    sr.Close();
                }
                return strResult;
            }
            catch
            {
                return String.Empty;

            }
        }

        #endregion

        /// <summary> Gets the simple list of items for a single item aggregation, or all of the library </summary>
        /// <param name="Aggregation_Code"> Code for the item aggregation of interest, or an empty string</param>
        /// <returns> Dataset with the simple list of items, including BibID, VID, Title, CreateDate, and Resource Link </returns>
        /// <remarks> This calls the 'SobekCM_Simple_Item_List' stored procedure in the main database</remarks> 
        public static SobekCM_All_Items Simple_Item_List(string Aggregation_Code)
        {
            try
            {
                // Create the connection
                SqlConnection connect = new SqlConnection(connectionString);

                // Create the command 
                SqlCommand executeCommand = new SqlCommand("SobekCM_Simple_Item_List", connect) { CommandType = CommandType.StoredProcedure };
                executeCommand.Parameters.AddWithValue("@collection_code", Aggregation_Code);

                // Create the adapter
                SqlDataAdapter adapter = new SqlDataAdapter(executeCommand);

                // Add appropriate table mappings
                adapter.TableMappings.Add("Table", "SobekCM_Item");

                // Create the strongly-typed dataset
                SobekCM_All_Items itemList = new SobekCM_All_Items();

                // Fill the strongly typed dataset
                adapter.Fill(itemList);

                return itemList;
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        /// <summary> Get the list of all archived TIVOLI files by BibID and VID </summary>
        /// <param name="BibID"> Bibliographic identifier </param>
        /// <param name="VID"> Volume identifier </param>
        /// <returns> List of all the files archived for a particular digital resource </returns>
        /// <remarks> This calls the 'Tivoli_Get_File_By_Bib_VID' stored procedure in the main SobekCM database</remarks> 
        public static DataTable Tivoli_Get_Archived_Files(string BibID, string VID)
        {
            try
            {
                // Build the parameter list
                EalDbParameter[] param_list = new EalDbParameter[2];
                param_list[0] = new EalDbParameter("@BibID", BibID);
                param_list[1] = new EalDbParameter("@VID", VID);

                // Define a temporary dataset
                DataSet tempSet = EalDbAccess.ExecuteDataset(DatabaseType, connectionString, CommandType.StoredProcedure, "Tivoli_Get_File_By_Bib_VID", param_list);
                if ((tempSet == null) || (tempSet.Tables.Count == 0) || (tempSet.Tables[0].Rows.Count == 0))
                    return null;
                return tempSet.Tables[0];
            }
            catch
            {
                // Pass this exception onto the method to handle it
                return null;

            }
        }

        #region Quality Control related methods
        /// <summary> Get the list of all the QC Page errors for a single item </summary>
        /// <param name="ItemID">ItemID</param>
        /// <returns></returns>
        public static DataTable Get_QC_Errors_For_Item(int ItemID)
        {
            try
            {
                //Add the parameters to this command
                EalDbParameter[] parameters = new EalDbParameter[1];
                parameters[0] = new EalDbParameter("@itemID", ItemID);

                DataSet returnSet = EalDbAccess.ExecuteDataset(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_QC_Get_Errors", parameters);
                if ((returnSet != null) && (returnSet.Tables.Count > 0))
                {
                    return returnSet.Tables[0];
                }

                return null;
            }
            catch (Exception ee)
            {
                throw new ApplicationException("Error getting the QC errors for this item." + ee.Message);
            }

        }

        /// <summary> Save QC error for a single page of a single item </summary>
        /// <param name="ItemID">ItemID</param>
        /// <param name="Filename">Root filename of this page</param>
        /// <param name="ErrorCode">Error Code for this error</param>
        /// <param name="Description">Error Description</param>
        /// <param name="isVolumeError">Indicates if this error is a Volume error or not</param>
        /// <returns></returns>
        public static int Save_QC_Error(int ItemID, string Filename, string ErrorCode, string Description, bool isVolumeError)
        {
            try
            {
                //Add the parameters to this command
                EalDbParameter[] parameters = new EalDbParameter[6];
                parameters[0] = new EalDbParameter("@itemID", ItemID);
                parameters[1] = new EalDbParameter("@filename", Filename);
                parameters[2] = new EalDbParameter("@errorCode", ErrorCode);
                parameters[3] = new EalDbParameter("@description", Description);
                parameters[4] = new EalDbParameter("@isVolumeError", isVolumeError);
                parameters[5] = new EalDbParameter("@errorID", DbType.Int64) { Direction = ParameterDirection.Output };

                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_QC_Save_Error", parameters);

                int errorID;
                Int32.TryParse(parameters[5].Value.ToString(), out errorID);
                return errorID;

            }
            catch (Exception ee)
            {
                throw new ApplicationException("Error saving QC page error to the database" + ee.Message);
            }
        }

        /// <summary> Delete the QC error for a single page of an item from the database </summary>
        /// <param name="itemID"></param>
        /// <param name="filename"></param>
        public static void Delete_QC_Error(int itemID, string filename)
        {
            try
            {
                //Add the parameters to this command
                EalDbParameter[] parameters = new EalDbParameter[2];
                parameters[0] = new EalDbParameter("@itemID", itemID);
                parameters[1] = new EalDbParameter("@filename", filename);

                EalDbAccess.ExecuteNonQuery(DatabaseType, connectionString, CommandType.StoredProcedure, "SobekCM_QC_Delete_Error", parameters);
            }

            catch (Exception ee)
            {
                throw new ApplicationException("Error deleting QC error from the database." + ee.Message);
            }
        }
        #endregion

    }
}