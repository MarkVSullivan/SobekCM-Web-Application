#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Items;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.EAD;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;
using SobekCM.Resource_Object.Metadata_Modules.Maps;
using SobekCM.Tools;

#endregion

namespace SobekCM.Engine_Library.Items
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
			DataSet itemDetails = Engine_Database.Get_Item_Group_Details(BibID, Tracer);

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
                metsFile = Engine_ApplicationCache_Gateway.Settings.Image_Server_Network + metsFile;
			}

			// Try to read this METS file
			bool pulledFromMETSFile = true;
			Item_Group_Object = Build_Item_From_METS(metsFile, BibID + ".xml", Tracer);

			// If this failed, just create an item from scratch
			if (Item_Group_Object == null)
			{
				Item_Group_Object = new SobekCM_Item();
				Item_Group_Object.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Serial;
				Item_Group_Object.BibID = BibID;
				pulledFromMETSFile = false;
			}

			// Set some default and add the management view
			Item_Group_Object.METS_Header.RecordStatus_Enum = METS_Record_Status.BIB_LEVEL;
			Item_Group_Object.Behaviors.Add_View(View_Enum.MANAGE);

			// Pull values from the database
			Item_Group_Object.Behaviors.GroupTitle = String.Empty;
			Item_Group_Object.Behaviors.Set_Primary_Identifier(mainItemRow["Primary_Identifier_Type"].ToString(), mainItemRow["Primary_Identifier"].ToString());
			Item_Group_Object.Behaviors.Text_Searchable = false;

			Item_Group_Object.Web.File_Root = String.Empty;
            Item_Group_Object.Web.Image_Root = Engine_ApplicationCache_Gateway.Settings.Image_URL;
			Item_Group_Object.Web.Siblings = 2;
			Item_Group_Object.Web.Static_PageCount = 0;
			Item_Group_Object.Web.Static_Division_Count = 0;
			Item_Group_Object.Web.AssocFilePath = "/" + BibID.Substring(0, 2) + "/" + BibID[2] + BibID[6] + "/" + BibID[4] + BibID[8] + "/" + BibID[3] + BibID[7] + "/" + BibID[5] + BibID[9] + "/";
			Item_Group_Object.Web.GroupID = Convert.ToInt32(mainItemRow["GroupID"]);

			// Add the full citation view and google map if pulled from the METS file
			if (pulledFromMETSFile)
			{
				// GEt the geospatial metadata module
				GeoSpatial_Information geoInfo = Item_Group_Object.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
				if ((geoInfo != null) && (geoInfo.hasData))
				{
					// In addition, if there is a latitude or longitude listed, add the Google Maps
					if ((geoInfo.Point_Count > 0) || (geoInfo.Polygon_Count > 0))
					{
						Item_Group_Object.Behaviors.Insert_View(0, View_Enum.GOOGLE_MAP);
					}
				}

				Item_Group_Object.Behaviors.Insert_View(0, View_Enum.CITATION);
			}

			// If this has more than 1 sibling (this count includes itself), add the multi-volumes viewer
			Item_Group_Object.Behaviors.Insert_View(0, View_Enum.ALL_VOLUMES, String.Empty, Item_Group_Object.Bib_Info.SobekCM_Type_String);

			// Pull the data from the database
			Item_Group_Object.Behaviors.GroupType = mainItemRow["Type"].ToString();
			Item_Group_Object.Behaviors.GroupTitle = mainItemRow["GroupTitle"].ToString();
			Item_Group_Object.Web.File_Root = mainItemRow["File_Location"].ToString();

			// Create the list of items in this title
			Items_In_Title = new SobekCM_Items_In_Title(itemDetails.Tables[1]);

			// Add the database information to the icons now
			Item_Group_Object.Behaviors.Clear_Wordmarks();
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
				Item_Group_Object.Behaviors.Add_Wordmark(newIcon);
			}

			// Add the web skin codes to this bib-level item as well
			Item_Group_Object.Behaviors.Clear_Web_Skins();
			foreach (DataRow thisRow in itemDetails.Tables[3].Rows)
			{
				Item_Group_Object.Behaviors.Add_Web_Skin(thisRow[0].ToString().ToUpper());
			}

			// Set the aggregationPermissions in the package to the aggregation links from the database
			if (itemDetails.Tables.Count == 6)
			{
				Item_Group_Object.Behaviors.Clear_Aggregations();
                DataTable aggrTable = itemDetails.Tables[4];
                foreach (DataRow thisRow in aggrTable.Rows)
				{
					Item_Group_Object.Behaviors.Add_Aggregation(thisRow[0].ToString());
				}

				// Add the related titles, if there are some
				foreach (DataRow thisRow in itemDetails.Tables[5].Rows)
				{
					string relationship = thisRow["Relationship"].ToString();
					string title = thisRow["GroupTitle"].ToString();
					string bibid = thisRow["BibID"].ToString();
					string link_and_title = "<a href=\"<%BASEURL%>" + bibid + "<%URL_OPTS%>\">" + title + "</a>";
					Item_Group_Object.Web.All_Related_Titles.Add(new Related_Titles(relationship, link_and_title));
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
				string mets_file = METS_Location.Replace("\\", "/") + "/citation_mets.xml";

				SobekCM_Item thisPackage = Build_Item_From_METS(mets_file, "citation_mets.xml", Tracer);

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
		public SobekCM_Item Build_Item(string BibID, string VID, Dictionary<string, Wordmark_Icon> Icon_Dictionary, List<string> Item_Viewer_Priority, Custom_Tracer Tracer)
		{
			if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item", "Create the requested item");
				}

			try
			{
				// Get the basic information about this item
				DataSet itemDetails = Engine_Database.Get_Item_Details(BibID, VID, Tracer);

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
					mets_file = Engine_ApplicationCache_Gateway.Settings.Image_Server_Network + mets_file;
				}

				// Could point directly to a METS file in some off-site location, in which case, use it
				if (mets_location.IndexOf(".mets") > 0)
					mets_file = mets_location;

				// Try to read the service METS
				SobekCM_Item thisPackage = Build_Item_From_METS(mets_file, BibID + "_" + VID + ".mets.xml", Tracer);

				if (thisPackage == null)
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item", "Unable to read the SobekCM service METS file", Custom_Trace_Type_Enum.Error);
					}

					return null;
				}

				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item", "Finish building this item");
				}

				// Check to see if multiple sibling volumes exist
				bool multiple_volumes_exist = Convert.ToInt32(mainItemRow["Total_Volumes"]) > 1;

				// Now finish building the object from the application state values
				Finish_Building_Item(thisPackage, itemDetails, multiple_volumes_exist, Item_Viewer_Priority, Tracer);              

				return thisPackage;
			}
			catch (Exception ee)
			{
				if (Tracer != null)
					Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item", ee.ToString().Replace("\n", "<br />"), Custom_Trace_Type_Enum.Error);
				return null;
			}
		}

		/// <summary> Builds a digital resource for a single volume within a title </summary>
		/// <param name="BibID"> Bibliographic identifier for the title </param>
		/// <param name="VID"> Volume identifier for the title </param>
		/// <param name="METS_Location"> Location of the METS file to read </param>
		/// <param name="Icon_Dictionary"> Dictionary of information about every wordmark/icon in this digital library, used to build the HTML for the icons linked to this digital resource</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Fully built version of a digital resource </returns>
		public SobekCM_Item Build_Item(string METS_Location, string BibID, String VID, Dictionary<string, Wordmark_Icon> Icon_Dictionary, List<string> Item_Viewer_Priority, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item", "Create the requested item");
			}

			try
			{
				// Get the basic information about this item
				DataSet itemDetails = Engine_Database.Get_Item_Details(BibID, VID, Tracer);

				// If the itemdetails was null, this item is somehow invalid item then
				if (itemDetails == null)
					return null;


				// Try to read the service METS
				SobekCM_Item thisPackage = Build_Item_From_METS(METS_Location, String.Empty, Tracer);

				if (thisPackage == null)
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item", "Unable to read the indicated METS file ( " + METS_Location + " )", Custom_Trace_Type_Enum.Error);
					}

					return null;
				}

				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item", "Finish building this item");
				}

				// Now finish building the object from the application state values
				Finish_Building_Item(thisPackage, itemDetails, false, Item_Viewer_Priority, Tracer);

				return thisPackage;
			}
			catch (Exception ee)
			{
				if (Tracer != null)
					Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item", ee.ToString().Replace("\n", "<br />"), Custom_Trace_Type_Enum.Error);
				return null;
			}
		}

		private SobekCM_Item Build_Item_From_METS(string METS_URL, string METS_Name, Custom_Tracer Tracer)
		{
			try
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item_From_METS", "Open http web request stream to METS file ( <a href=\"" + METS_URL + "\">" + METS_Name + "</a> )");
				}

				SobekCM_Item thisPackage = new SobekCM_Item();
				if (METS_URL.IndexOf("http:") >= 0)
				{
					WebRequest objRequest = WebRequest.Create(METS_URL);
					objRequest.Timeout = 5000;
					WebResponse objResponse = objRequest.GetResponse();

					if (Tracer != null)
					{
						Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Build_Item_From_METS", "Read the METS file from the stream");
					}

					// Read the METS file and create the package
					METS_File_ReaderWriter reader = new METS_File_ReaderWriter();
					string errorMessage;
					reader.Read_Metadata(objResponse.GetResponseStream(), thisPackage, null, out errorMessage);
					objResponse.Close();
				}
				else
				{
					if (File.Exists(METS_URL.Replace("/", "\\")))
					{
						// Read the METS file and create the package
						METS_File_ReaderWriter reader = new METS_File_ReaderWriter();
						string errorMessage;
						reader.Read_Metadata(METS_URL.Replace("/", "\\"), thisPackage, null, out errorMessage);
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

		private void Finish_Building_Item(SobekCM_Item Package_To_Finalize, DataSet DatabaseInfo, bool Multiple, List<string> Item_Viewer_Priority, Custom_Tracer Tracer )
		{
			Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Load the data from the database into the resource object");

			if ((DatabaseInfo == null) || (DatabaseInfo.Tables[2] == null) || (DatabaseInfo.Tables[2].Rows.Count == 0))
			{
				Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Invalid data from the database, either not enough tables, or no rows in Tables[2]");
			}

			// Copy over some basic values
			DataRow mainItemRow = DatabaseInfo.Tables[2].Rows[0];
			Package_To_Finalize.Behaviors.Set_Primary_Identifier(mainItemRow["Primary_Identifier_Type"].ToString(), mainItemRow["Primary_Identifier"].ToString());
			Package_To_Finalize.Behaviors.GroupTitle = mainItemRow["GroupTitle"].ToString();
			Package_To_Finalize.Behaviors.GroupType = mainItemRow["GroupType"].ToString();
			Package_To_Finalize.Web.File_Root = mainItemRow["File_Location"].ToString();
			Package_To_Finalize.Web.AssocFilePath = mainItemRow["File_Location"] + "\\" + Package_To_Finalize.VID + "\\";
			Package_To_Finalize.Behaviors.IP_Restriction_Membership = Convert.ToInt16(mainItemRow["IP_Restriction_Mask"]);             
			Package_To_Finalize.Behaviors.CheckOut_Required = Convert.ToBoolean(mainItemRow["CheckoutRequired"]);
			Package_To_Finalize.Behaviors.Text_Searchable = Convert.ToBoolean(mainItemRow["TextSearchable"]);
			Package_To_Finalize.Web.ItemID = Convert.ToInt32(mainItemRow["ItemID"]);
			Package_To_Finalize.Web.GroupID = Convert.ToInt32(mainItemRow["GroupID"]);
			Package_To_Finalize.Behaviors.Suppress_Endeca = Convert.ToBoolean(mainItemRow["SuppressEndeca"]);
			//Package_To_Finalize.Behaviors.Expose_Full_Text_For_Harvesting = Convert.ToBoolean(mainItemRow["SuppressEndeca"]);
			Package_To_Finalize.Tracking.Internal_Comments = mainItemRow["Comments"].ToString();
			Package_To_Finalize.Behaviors.Dark_Flag = Convert.ToBoolean(mainItemRow["Dark"]);
			Package_To_Finalize.Tracking.Born_Digital = Convert.ToBoolean(mainItemRow["Born_Digital"]);
			Package_To_Finalize.Behaviors.Main_Thumbnail = mainItemRow["MainThumbnail"].ToString();
			//Package_To_Finalize.Divisions.Page_Count = Convert.ToInt32(mainItemRow["Pages"]);
			if (mainItemRow["Disposition_Advice"] != DBNull.Value)
				Package_To_Finalize.Tracking.Disposition_Advice = Convert.ToInt16(mainItemRow["Disposition_Advice"]);
			else
				Package_To_Finalize.Tracking.Disposition_Advice = -1;
			if (mainItemRow["Material_Received_Date"] != DBNull.Value)
				Package_To_Finalize.Tracking.Material_Received_Date = Convert.ToDateTime(mainItemRow["Material_Received_Date"]);
			else
				Package_To_Finalize.Tracking.Material_Received_Date = null;
			if (mainItemRow["Material_Recd_Date_Estimated"] != DBNull.Value)
				Package_To_Finalize.Tracking.Material_Rec_Date_Estimated = Convert.ToBoolean(mainItemRow["Material_Recd_Date_Estimated"]);
			if (DatabaseInfo.Tables[2].Columns.Contains("Tracking_Box"))
			{
				if (mainItemRow["Tracking_Box"] != DBNull.Value)
					Package_To_Finalize.Tracking.Tracking_Box= mainItemRow["Tracking_Box"].ToString();
			}

			// Set more of the sobekcm web portions in the item 
			Package_To_Finalize.Web.Set_BibID_VID(Package_To_Finalize.BibID, Package_To_Finalize.VID);
            Package_To_Finalize.Web.Image_Root = Engine_ApplicationCache_Gateway.Settings.Image_URL;
			if (Multiple)
				Package_To_Finalize.Web.Siblings = 2;

			// Set the serial hierarchy from the database (if multiple)
			if ((Multiple) && (mainItemRow["Level1_Text"].ToString().Length > 0))
			{
				Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Assigning serial hierarchy from the database info");

				bool found = false;

				// Get the values from the database first
				string level1_text = mainItemRow["Level1_Text"].ToString();
				string level2_text = mainItemRow["Level2_Text"].ToString();
				string level3_text = mainItemRow["Level3_Text"].ToString();
				int level1_index = Convert.ToInt32(mainItemRow["Level1_Index"]);
				int level2_index = Convert.ToInt32(mainItemRow["Level2_Index"]);
				int level3_index = Convert.ToInt32(mainItemRow["Level3_Index"]);

				// Does this match the enumeration
				if (level1_text.ToUpper().Trim() == Package_To_Finalize.Bib_Info.Series_Part_Info.Enum1.ToUpper().Trim())
				{
					// Copy the database values to the enumeration portion
					Package_To_Finalize.Bib_Info.Series_Part_Info.Enum1 = level1_text;
					Package_To_Finalize.Bib_Info.Series_Part_Info.Enum1_Index = level1_index;
					Package_To_Finalize.Bib_Info.Series_Part_Info.Enum2 = level2_text;
					Package_To_Finalize.Bib_Info.Series_Part_Info.Enum2_Index = level2_index;
					Package_To_Finalize.Bib_Info.Series_Part_Info.Enum3 = level3_text;
					Package_To_Finalize.Bib_Info.Series_Part_Info.Enum3_Index = level3_index;
					found = true;
				}

				// Does this match the chronology
				if ((!found) && (level1_text.ToUpper().Trim() == Package_To_Finalize.Bib_Info.Series_Part_Info.Year.ToUpper().Trim()))
				{
					// Copy the database values to the chronology portion
					Package_To_Finalize.Bib_Info.Series_Part_Info.Year = level1_text;
					Package_To_Finalize.Bib_Info.Series_Part_Info.Year_Index = level1_index;
					Package_To_Finalize.Bib_Info.Series_Part_Info.Month = level2_text;
					Package_To_Finalize.Bib_Info.Series_Part_Info.Month_Index = level2_index;
					Package_To_Finalize.Bib_Info.Series_Part_Info.Day = level3_text;
					Package_To_Finalize.Bib_Info.Series_Part_Info.Day_Index = level3_index;
					found = true;
				}

				if (!found)
				{
					// No match.  If it is numeric, move it to the chronology, otherwise, enumeration
					bool charFound = level1_text.Trim().Any(ThisChar => !Char.IsNumber(ThisChar));

					if (charFound)
					{
						// Copy the database values to the enumeration portion
						Package_To_Finalize.Bib_Info.Series_Part_Info.Enum1 = level1_text;
						Package_To_Finalize.Bib_Info.Series_Part_Info.Enum1_Index = level1_index;
						Package_To_Finalize.Bib_Info.Series_Part_Info.Enum2 = level2_text;
						Package_To_Finalize.Bib_Info.Series_Part_Info.Enum2_Index = level2_index;
						Package_To_Finalize.Bib_Info.Series_Part_Info.Enum3 = level3_text;
						Package_To_Finalize.Bib_Info.Series_Part_Info.Enum3_Index = level3_index;
					}
					else
					{
						// Copy the database values to the chronology portion
						Package_To_Finalize.Bib_Info.Series_Part_Info.Year = level1_text;
						Package_To_Finalize.Bib_Info.Series_Part_Info.Year_Index = level1_index;
						Package_To_Finalize.Bib_Info.Series_Part_Info.Month = level2_text;
						Package_To_Finalize.Bib_Info.Series_Part_Info.Month_Index = level2_index;
						Package_To_Finalize.Bib_Info.Series_Part_Info.Day = level3_text;
						Package_To_Finalize.Bib_Info.Series_Part_Info.Day_Index = level3_index;
					}
				}

				// Copy the database values to the simple serial portion (used to actually determine serial heirarchy)
				Package_To_Finalize.Behaviors.Serial_Info.Clear();
				Package_To_Finalize.Behaviors.Serial_Info.Add_Hierarchy(1, level1_index, level1_text);
				if (level2_text.Length > 0)
				{
					Package_To_Finalize.Behaviors.Serial_Info.Add_Hierarchy(2, level2_index, level2_text);
					if (level3_text.Length > 0)
					{
						Package_To_Finalize.Behaviors.Serial_Info.Add_Hierarchy(3, level3_index, level3_text);
					}
				}
			}

			// See if this can be described
			bool can_describe = false;
			foreach (DataRow thisRow in DatabaseInfo.Tables[1].Rows)
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
			Package_To_Finalize.Behaviors.Can_Be_Described = can_describe;

            // Look for rights information to add
            if (mainItemRow["EmbargoEnd"] != DBNull.Value)
		    {
		        try
		        {
		            DateTime embargoEnd = DateTime.Parse(mainItemRow["EmbargoEnd"].ToString());
                    string origAccessCode = mainItemRow["Original_AccessCode"].ToString();

                    // Is there already a RightsMD module in the item?
                    // Ensure this metadata module extension exists
                    RightsMD_Info rightsInfo = Package_To_Finalize.Get_Metadata_Module(GlobalVar.PALMM_RIGHTSMD_METADATA_MODULE_KEY) as RightsMD_Info;
                    if (rightsInfo == null)
                    {
                        rightsInfo = new RightsMD_Info();
                        Package_To_Finalize.Add_Metadata_Module(GlobalVar.PALMM_RIGHTSMD_METADATA_MODULE_KEY, rightsInfo);
                    }

                    // Add the data
		            rightsInfo.Access_Code_String = origAccessCode;
		            rightsInfo.Embargo_End = embargoEnd;
		        }
		        catch (Exception)
		        {
		            
		        }
		    }

			// Look for user descriptions
			Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Look for user descriptions (or tags)");
			foreach (DataRow thisRow in DatabaseInfo.Tables[0].Rows)
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
					Package_To_Finalize.Behaviors.Add_User_Tag(userid, nick_name + " " + last_name, tag, dateAdded, tagid);
				}
				else
				{
					Package_To_Finalize.Behaviors.Add_User_Tag(userid, first_name + " " + last_name, tag, dateAdded, tagid);
				}
			}

			// Look for ticklers
			Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Load ticklers from the database info");
			foreach (DataRow thisRow in DatabaseInfo.Tables[3].Rows)
			{
				Package_To_Finalize.Behaviors.Add_Tickler(thisRow["MetadataValue"].ToString().Trim());
			}

			// Set the aggregationPermissions in the package to the aggregation links from the database
			Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Load the aggregations from the database info");
			Package_To_Finalize.Behaviors.Clear_Aggregations();
			foreach (DataRow thisRow in DatabaseInfo.Tables[1].Rows)
			{
				if (!Convert.ToBoolean(thisRow["impliedLink"]))
				{
				    string code = thisRow["Code"].ToString();
				    if (String.Compare(code, "all", true) != 0)
				    {
				        Package_To_Finalize.Behaviors.Add_Aggregation(code, thisRow["Name"].ToString(), thisRow["Type"].ToString());
				    }
				}
			}

			// If no collections, add some regardless of whether it was IMPLIED
			if ( Package_To_Finalize.Behaviors.Aggregation_Count == 0)
			{
				foreach (DataRow thisRow in DatabaseInfo.Tables[1].Rows)
				{
                    string code = thisRow["Code"].ToString();
                    if (String.Compare(code, "all", true) != 0)
                    {
                        Package_To_Finalize.Behaviors.Add_Aggregation(code, thisRow["Name"].ToString(), thisRow["Type"].ToString());
                    }
				}
			}

			// Step through each page and set the static page count
			Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Set the static page count");

			pageseq = 0;
			List<Page_TreeNode> pages_encountered = new List<Page_TreeNode>();
			foreach (abstract_TreeNode rootNode in Package_To_Finalize.Divisions.Physical_Tree.Roots)
			{
				recurse_through_nodes(Package_To_Finalize, rootNode, pages_encountered);
			}
			Package_To_Finalize.Web.Static_PageCount = pages_encountered.Count;
			Package_To_Finalize.Web.Static_Division_Count = divseq;

			// Make sure no icons were retained from the METS file itself
			Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Load the wordmarks/icons from the database info");
			Package_To_Finalize.Behaviors.Clear_Wordmarks();

			// Add the icons from the database information
			foreach (DataRow iconRow in DatabaseInfo.Tables[5].Rows)
			{
				string image = iconRow[0].ToString();
				string link = iconRow[1].ToString().Replace("&", "&amp;").Replace("\"", "&quot;");
				string code = iconRow[2].ToString();
				string name = iconRow[3].ToString();
				if ( name.Length == 0 )
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
				Package_To_Finalize.Behaviors.Add_Wordmark(newIcon);
			}

			// Make sure no web skins were retained from the METS file itself
			Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Load the web skins from the database info");
			Package_To_Finalize.Behaviors.Clear_Web_Skins();

			// Add the web skins from the database
			foreach (DataRow skinRow in DatabaseInfo.Tables[6].Rows)
			{
				Package_To_Finalize.Behaviors.Add_Web_Skin(skinRow[0].ToString().ToUpper());
			}

			Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Set the views from a combination of the METS and the database info");

			// Make sure no views were retained from the METS file itself
			Package_To_Finalize.Behaviors.Clear_Views();
			Package_To_Finalize.Behaviors.Clear_Item_Level_Page_Views();

			// If this has more than 1 sibling (this count includes itself), add the multi-volumes viewer
			if (Multiple)
			{
				Package_To_Finalize.Behaviors.Add_View(View_Enum.ALL_VOLUMES, String.Empty, Package_To_Finalize.Bib_Info.SobekCM_Type_String);
			}

			// Add the full citation view and the (hidden) tracking view and some other ALWAYS views
			Package_To_Finalize.Behaviors.Add_View(View_Enum.CITATION);
			Package_To_Finalize.Behaviors.Add_View(View_Enum.TRACKING);
            Package_To_Finalize.Behaviors.Add_View(View_Enum.TRACKING_SHEET);
			Package_To_Finalize.Behaviors.Add_View(View_Enum.GOOGLE_COORDINATE_ENTRY);
			Package_To_Finalize.Behaviors.Add_View(View_Enum.TEST);
			Package_To_Finalize.Behaviors.Add_View(View_Enum.MANAGE);

			// Add the full text searchable
			if ( Package_To_Finalize.Behaviors.Text_Searchable )
				Package_To_Finalize.Behaviors.Add_View(View_Enum.SEARCH);

			// Is there an embedded video?
			if (Package_To_Finalize.Behaviors.Embedded_Video.Length > 0)
				Package_To_Finalize.Behaviors.Add_View(View_Enum.EMBEDDED_VIDEO);

			// If there is no PURL, add one based on how SobekCM operates
			if (Package_To_Finalize.Bib_Info.Location.PURL.Length == 0)
			{
                Package_To_Finalize.Bib_Info.Location.PURL = Engine_ApplicationCache_Gateway.Settings.System_Base_URL + Package_To_Finalize.BibID + "/" + Package_To_Finalize.VID;
				
			}

			// If this is a newspaper, and there is no datecreated, see if we 
			// can make one from the  serial hierarchy
			if (Package_To_Finalize.Behaviors.GroupType.ToUpper() == "NEWSPAPER")
			{
				if ((Package_To_Finalize.Bib_Info.Origin_Info.Date_Created.Length == 0) && (Package_To_Finalize.Bib_Info.Origin_Info.Date_Issued.Length == 0))
				{
					// Is the serial hierarchy three deep?
					if (Package_To_Finalize.Behaviors.hasSerialInformation)
					{
						if (Package_To_Finalize.Behaviors.Serial_Info.Count == 3)
						{
							int year;

							if (Int32.TryParse(Package_To_Finalize.Behaviors.Serial_Info[0].Display, out year))
							{
								int day;
								if (Int32.TryParse(Package_To_Finalize.Behaviors.Serial_Info[2].Display, out day))
								{
									if ((year > 0) && (year < DateTime.Now.Year + 2) && ( day > 0 ) && ( day <= 31 ))
									{
										// Is the month a number?
										int month;
										if (Int32.TryParse(Package_To_Finalize.Behaviors.Serial_Info[1].Display, out month))
										{
											try
											{
												// Do it this way since hopefully that will work for localization issues
												DateTime date = new DateTime(year, month, day);
												Package_To_Finalize.Bib_Info.Origin_Info.Date_Created = date.ToShortDateString();
											}
											catch 
											{
												// If this is an invalid date, catch the error and do nothing
											}
										}
										else
										{
											Package_To_Finalize.Bib_Info.Origin_Info.Date_Created = Package_To_Finalize.Behaviors.Serial_Info[1].Display + " " + day + ", " + year;
										}
									}
								}
							}
						}
						else if ( Package_To_Finalize.Behaviors.Serial_Info.Count == 2 )
						{
							int year;
							if (Int32.TryParse(Package_To_Finalize.Behaviors.Serial_Info[0].Display, out year))
							{
								if ((year > 0) && (year < DateTime.Now.Year + 2) && ( Package_To_Finalize.Behaviors.Serial_Info[1].Display.Length > 0 ))
								{
									Package_To_Finalize.Bib_Info.Origin_Info.Date_Created = Package_To_Finalize.Behaviors.Serial_Info[1].Display + " " + year;
								}
							}
						}
					}
				}
			}

			// IF this is dark, add no other views
			if (Package_To_Finalize.Behaviors.Dark_Flag) return;

			// Check to see which views were present from the database, and build the list
			Dictionary<View_Enum, View_Object> viewsFromDb = new Dictionary<View_Enum, View_Object>();
			foreach (DataRow viewRow in DatabaseInfo.Tables[4].Rows)
			{
				string viewType = viewRow[0].ToString();
				string attribute = viewRow[1].ToString();
				string label = viewRow[2].ToString();

				View_Enum viewTypeEnum = View_Enum.None;
				switch (viewType)
				{
					case "Dataset Codebook":
						viewTypeEnum = View_Enum.DATASET_CODEBOOK;
						break;

					case "Dataset Reports":
						viewTypeEnum = View_Enum.DATASET_REPORTS;
						break;

					case "Dataset View Data":
						viewTypeEnum = View_Enum.DATASET_VIEWDATA;
						break;

					case "Google Map":
						viewTypeEnum = View_Enum.GOOGLE_MAP;
						break;

                    case "Google Map Beta":
                        viewTypeEnum = View_Enum.GOOGLE_MAP_BETA;
                        break;

					case "HTML Viewer":
						viewTypeEnum = View_Enum.HTML;
						break;

					case "JPEG":
						viewTypeEnum = View_Enum.JPEG;
						break;

					case "JPEG/Text Two Up":
						viewTypeEnum = View_Enum.JPEG_TEXT_TWO_UP;
						break;

					case "JPEG2000":
						viewTypeEnum = View_Enum.JPEG2000;
						break;

					case "Page Turner":
						viewTypeEnum = View_Enum.PAGE_TURNER;
						break;

					case "Related Images":
						viewTypeEnum = View_Enum.RELATED_IMAGES;
						break;

					case "TEI":
						viewTypeEnum = View_Enum.TEI;
						break;

					case "Text":
						viewTypeEnum = View_Enum.TEXT;
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

			// Add the dataset views (later we should do some checking here, but for 
			// now just add them if the user selected them.
			if (viewsFromDb.ContainsKey(View_Enum.DATASET_VIEWDATA))
			{
				Package_To_Finalize.Behaviors.Add_View(viewsFromDb[View_Enum.DATASET_VIEWDATA]);
				viewsFromDb.Remove(View_Enum.DATASET_VIEWDATA);
			}
			if (viewsFromDb.ContainsKey(View_Enum.DATASET_CODEBOOK))
			{
				Package_To_Finalize.Behaviors.Add_View(viewsFromDb[View_Enum.DATASET_CODEBOOK]);
				viewsFromDb.Remove(View_Enum.DATASET_CODEBOOK);
			}
			if (viewsFromDb.ContainsKey(View_Enum.DATASET_REPORTS))
			{
				Package_To_Finalize.Behaviors.Add_View(viewsFromDb[View_Enum.DATASET_REPORTS]);
				viewsFromDb.Remove(View_Enum.DATASET_REPORTS);
			}


			// Add the thumbnail view, if requested and has multiple pages
			if (Package_To_Finalize.Divisions.Page_Count > 1)
			{
			    if (viewsFromDb.ContainsKey(View_Enum.RELATED_IMAGES))
			    {
			        Package_To_Finalize.Behaviors.Add_View(viewsFromDb[View_Enum.RELATED_IMAGES]);
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

            // Always add the QC viewer ( the QC viewer will redirect to upload files if there are NO pages)
            Package_To_Finalize.Behaviors.Add_View(View_Enum.QUALITY_CONTROL);

			// If this item has more than one division, look for the TOC viewer
			if ((Package_To_Finalize.Divisions.Has_Multiple_Divisions) && (!Package_To_Finalize.Bib_Info.ImageClass))
			{
				if (viewsFromDb.ContainsKey(View_Enum.TOC))
				{
					Package_To_Finalize.Behaviors.Add_View(viewsFromDb[View_Enum.TOC]);
					viewsFromDb.Remove(View_Enum.TOC);
				}
			}

			// In addition, if there is a latitude or longitude listed, look for the Google Maps
			bool hasCoords = false;
			GeoSpatial_Information geoInfo = (GeoSpatial_Information) Package_To_Finalize.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY);
			if (( geoInfo != null ) && ( geoInfo.hasData ))
			{
				if ((geoInfo.Point_Count > 0) || (geoInfo.Polygon_Count > 0))
				{
					hasCoords = true;
				}
			}
			if (!hasCoords)
			{
				List<abstract_TreeNode> pageList = Package_To_Finalize.Divisions.Physical_Tree.Pages_PreOrder;
				if (pageList.Select(ThisPage => (GeoSpatial_Information) ThisPage.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY)).Where(GeoInfo2 => (GeoInfo2 != null) && (GeoInfo2.hasData)).Any(GeoInfo2 => (GeoInfo2.Point_Count > 0) || (GeoInfo2.Polygon_Count > 0)))
				{
					hasCoords = true;
				}
			}

			if (hasCoords)
			{
				if (viewsFromDb.ContainsKey(View_Enum.GOOGLE_MAP))
				{
					Package_To_Finalize.Behaviors.Add_View(viewsFromDb[View_Enum.GOOGLE_MAP]);
					viewsFromDb.Remove(View_Enum.GOOGLE_MAP);
				}
				else
				{
					Package_To_Finalize.Behaviors.Add_View(View_Enum.GOOGLE_MAP);
				}
			}

			// Step through each download and make sure it is fully built
			if (Package_To_Finalize.Divisions.Download_Tree.Has_Files)
			{
				string ead_file = String.Empty;
				int pdf_download = 0;
				string pdf_download_url = String.Empty;
				int non_flash_downloads = 0;
				List<abstract_TreeNode> downloadPages = Package_To_Finalize.Divisions.Download_Tree.Pages_PreOrder;
				bool download_handled = false;
                string xsl = String.Empty;

                // Keep track of all the unhandled downloads, which will casue a DOWNLOAD tab to appear
			    List<abstract_TreeNode> unhandledDownload = new List<abstract_TreeNode>();

                // Step through each download page
				foreach (Page_TreeNode downloadPage in downloadPages)
				{
                    download_handled = false;

                    // If this page has only a single file, might be handled by a single viewer
                    if ((!download_handled) && (downloadPage.Files.Count == 1))
                    {
                        string extension = downloadPage.Files[0].File_Extension;

                        // Was this an EAD page?
                        switch (extension)
                        {
                            case "XML":
                                if (downloadPage.Label == "EAD")
                                {
                                    Package_To_Finalize.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.EAD;
                                    ead_file = downloadPage.Files[0].System_Name;
                                    download_handled = true;
                                }
                                break;

                            case "SWF":
                                // FLASH files are always handled
                                string flashlabel = downloadPage.Label;
                                Package_To_Finalize.Behaviors.Add_View(View_Enum.FLASH, flashlabel, String.Empty, downloadPage.Files[0].System_Name);
                                download_handled = true;
                                break;

                            case "PDF":
                                pdf_download++;
                                if (pdf_download == 1)
                                {
                                    pdf_download_url = downloadPage.Files[0].System_Name;
                                    download_handled = true;
                                }
                                break;

                            case "XSL":
                                xsl = downloadPage.Files[0].System_Name;
                                download_handled = true;
                                break;

                            case "HTML":
                            case "HTM":
                                if (viewsFromDb.ContainsKey(View_Enum.HTML))
                                {
                                    if (String.Compare(viewsFromDb[View_Enum.HTML].Attributes, downloadPage.Files[0].System_Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                                    {
                                        download_handled = true;
                                    }
                                }
                                break;
                        }
                    }

					// Step through each download file
					if (!download_handled)
					{
					    unhandledDownload.Add(downloadPage);

						foreach (SobekCM_File_Info thisFile in downloadPage.Files)
						{
							if (thisFile.File_Extension == "SWF")
							{
								string flashlabel = downloadPage.Label;
								View_Object newView = Package_To_Finalize.Behaviors.Add_View(View_Enum.FLASH, flashlabel, String.Empty, thisFile.System_Name);
							}
							else
							{
								non_flash_downloads++;
							}

							if (thisFile.File_Extension == "PDF")
							{
                                pdf_download++;
                                if (pdf_download == 0)
                                {
                                    pdf_download_url = thisFile.System_Name;
                                }
							}
						}
					}
				}

				// Some special code for EAD objects
				if ((Package_To_Finalize.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.EAD) && (ead_file.Length > 0))
				{
					// Now, read this EAD file information 
					string ead_file_location = Engine_ApplicationCache_Gateway.Settings.Image_Server_Network + Package_To_Finalize.Web.AssocFilePath + ead_file;
					EAD_File_ReaderWriter reader = new EAD_File_ReaderWriter();
					string errorMessage;
					Dictionary<string, object> options = new Dictionary<string, object>();
                    options["EAD_File_ReaderWriter:XSL_Location"] = Engine_ApplicationCache_Gateway.Settings.System_Base_URL + "default/sobekcm_default.xsl";

					reader.Read_Metadata(ead_file_location, Package_To_Finalize, options, out errorMessage);

					// Clear all existing views
					Package_To_Finalize.Behaviors.Add_View(View_Enum.EAD_DESCRIPTION);

					// Get the metadata module for EADs
					EAD_Info eadInfo = Package_To_Finalize.Get_Metadata_Module(GlobalVar.EAD_METADATA_MODULE_KEY) as EAD_Info;
					if ((eadInfo != null) && (eadInfo.Container_Hierarchy.Containers.Count > 0))
						Package_To_Finalize.Behaviors.Add_View(View_Enum.EAD_CONTAINER_LIST);

				}

				string view_type_of = Package_To_Finalize.Behaviors.Views[0].GetType().ToString();
				string ufdc_type_of = Package_To_Finalize.Behaviors.Views[0].View_Type.ToString();


                if (unhandledDownload.Count > 0 )
				{
					Package_To_Finalize.Behaviors.Add_View(View_Enum.DOWNLOADS);
				}

				if (pdf_download == 1)
				{
                    Package_To_Finalize.Behaviors.Add_View(View_Enum.PDF).FileName = pdf_download_url;
				}
			}
			else
			{
				if (Package_To_Finalize.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Aerial )
				{
					Package_To_Finalize.Behaviors.Add_View(View_Enum.DOWNLOADS);
				}
			}

			// If there is a RELATED URL with youtube, add that viewer
			if ((Package_To_Finalize.Bib_Info.hasLocationInformation) && (Package_To_Finalize.Bib_Info.Location.Other_URL.ToLower().IndexOf("www.youtube.com") >= 0))
			{
				View_Object newViewObj = new View_Object(View_Enum.YOUTUBE_VIDEO);
				Package_To_Finalize.Behaviors.Add_View(newViewObj);
			}

			// Look for the HTML type views next, and possible set some defaults
			if (viewsFromDb.ContainsKey(View_Enum.HTML))
			{
				Package_To_Finalize.Behaviors.Add_View(viewsFromDb[View_Enum.HTML]);
				viewsFromDb.Remove(View_Enum.HTML);
			}

			// Copy the TEI flag
			if (viewsFromDb.ContainsKey(View_Enum.TEI))
			{
				Package_To_Finalize.Behaviors.Add_View(viewsFromDb[View_Enum.TEI]);
				viewsFromDb.Remove(View_Enum.TEI);
			}

			// Look to add any index information here ( such as on SANBORN maps)
			Map_Info mapInfo = (Map_Info) Package_To_Finalize.Get_Metadata_Module(GlobalVar.SOBEKCM_MAPS_METADATA_MODULE_KEY);
			if (mapInfo != null)
			{
				//// Were there streets?
				//if (Package_To_Finalize.Map.Streets.Count > 0)
				//{
				//    returnValue.Item_Views.Add(new ViewerFetcher.Streets_ViewerFetcher());
				//}

				//// Were there features?
				//if (Package_To_Finalize.Map.Features.Count > 0)
				//{
				//    returnValue.Item_Views.Add(new ViewerFetcher.Features_ViewerFetcher());
				//}
			}

			// Look for the RELATED IMAGES view next
			if (viewsFromDb.ContainsKey(View_Enum.RELATED_IMAGES))
			{
				Package_To_Finalize.Behaviors.Add_View(viewsFromDb[View_Enum.RELATED_IMAGES]);
				viewsFromDb.Remove(View_Enum.RELATED_IMAGES);
			}

			// Look for the PAGE TURNER view next
			if (viewsFromDb.ContainsKey(View_Enum.PAGE_TURNER))
			{
				Package_To_Finalize.Behaviors.Add_View(viewsFromDb[View_Enum.PAGE_TURNER]);
				viewsFromDb.Remove(View_Enum.PAGE_TURNER);
			}

			// Finally, add all the ITEM VIEWS
			if ((Package_To_Finalize.Web.Pages_By_Sequence != null) && (Package_To_Finalize.Web.Pages_By_Sequence.Count > 0))
			{
				foreach (View_Object thisObject in viewsFromDb.Values)
				{
					switch (thisObject.View_Type)
					{
						case View_Enum.TEXT:
						case View_Enum.JPEG:
						case View_Enum.JPEG2000:
							Package_To_Finalize.Behaviors.Add_Item_Level_Page_View(thisObject);
							break;
					}
				}
			}

			// Set the default views for this item
			Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Set the default view, if not already assigned");
			Package_To_Finalize.Behaviors.Default_View = null;
			Dictionary<string, View_Object> views_by_view_name = new Dictionary<string, View_Object>();
			foreach (View_Object thisView in Package_To_Finalize.Behaviors.Views)
			{
				if (!views_by_view_name.ContainsKey(thisView.View_Type.ToString()))
					views_by_view_name[thisView.View_Type.ToString()] = thisView;
			}
			foreach (View_Object thisView in Package_To_Finalize.Behaviors.Item_Level_Page_Views)
			{
				if (!views_by_view_name.ContainsKey(thisView.View_Type.ToString()))
					views_by_view_name[thisView.View_Type.ToString()] = thisView;
			}

            //If no viewer priorities have been passed in, add the default one
		    if (Item_Viewer_Priority == null)
		    {
                //TODO: Add default view here if present
		       // if (views_by_view_name != null)
		       //     Package_To_Finalize.Behaviors.Default_View =
		    }
		    else
		    {
		        foreach (string thisViewerType in Item_Viewer_Priority)
		        {
		            if (views_by_view_name.ContainsKey(thisViewerType))
		            {
		                Package_To_Finalize.Behaviors.Default_View = views_by_view_name[thisViewerType];
		                break;
		            }
		        }
		    }

			Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Done merging the database information with the resource object");

		}


		private void recurse_through_nodes( SobekCM_Item ThisPackage, abstract_TreeNode Node, List<Page_TreeNode> PagesEncountered )
		{
			if (Node.Page)
			{
				Page_TreeNode pageNode = (Page_TreeNode)Node;
				if ( !PagesEncountered.Contains( pageNode ))
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
								ThisPackage.Web.Add_Pages_By_Sequence(pageNode);
								PagesEncountered.Add(pageNode);
								page_added = true;
							}
							View_Object thisViewer = thisFile.Get_Viewer();
							if (thisViewer != null)
							{
								string[] codes = thisViewer.Viewer_Codes;
								if ((codes.Length > 0) && (codes[0].Length > 0))
								{
									ThisPackage.Web.Viewer_To_File[pageseq.ToString() + codes[0]] = thisFile;
								}
							}
						}

						// TEST: Special case for text
						if ((ThisPackage.BibID == "UF00001672") || ( ThisPackage.BibID == "TEST000003"))
						{
							if (thisFile.File_Extension.ToLower().IndexOf("jpg") >= 0)
							{
								string filename = thisFile.File_Name_Sans_Extension + ".txt";
								SobekCM_File_Info thisFileInfo = new SobekCM_File_Info(filename);
								ThisPackage.Web.Viewer_To_File[pageseq.ToString() + "t"] = thisFileInfo;
							}
						}
					}
				}
			}
			else
			{
				divseq++;
				Division_TreeNode divNode = (Division_TreeNode)Node;
				foreach (abstract_TreeNode childNode in divNode.Nodes)
				{
					recurse_through_nodes(ThisPackage, childNode, PagesEncountered);
				}
			}
		}

		#endregion
	}
}