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
		private int pageseq;

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
                metsFile = Engine_ApplicationCache_Gateway.Settings.Servers.Image_Server_Network + metsFile;
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
			Item_Group_Object.Behaviors.Add_View("MANAGE");

			// Pull values from the database
			Item_Group_Object.Behaviors.GroupTitle = String.Empty;
			Item_Group_Object.Behaviors.Set_Primary_Identifier(mainItemRow["Primary_Identifier_Type"].ToString(), mainItemRow["Primary_Identifier"].ToString());
			Item_Group_Object.Behaviors.Text_Searchable = false;

			Item_Group_Object.Web.File_Root = String.Empty;
            Item_Group_Object.Web.Image_Root = Engine_ApplicationCache_Gateway.Settings.Servers.Image_URL;
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
						Item_Group_Object.Behaviors.Insert_View(0, "GOOGLE_MAP");
					}
				}

				Item_Group_Object.Behaviors.Insert_View(0, "CITATION");
			}

			// If this has more than 1 sibling (this count includes itself), add the multi-volumes viewer
			Item_Group_Object.Behaviors.Insert_View(0, "ALL_VOLUMES", String.Empty, Item_Group_Object.Bib_Info.SobekCM_Type_String);

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
					newIcon.HTML = "<img class=\"SobekItemWordmark\" src=\"<%BASEURL%>design/wordmarks/" + thisIconRow["Icon_URL"].ToString().Replace("&","&amp;") + "\" alt=\"" + newIcon.Title.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" />";
				}
				else
				{
					newIcon.Title = thisIconRow["Icon_Name"].ToString();
					newIcon.Link = thisIconRow["Link"].ToString();
					newIcon.HTML = "<a href=\"" + newIcon.Link + "\" target=\"_blank\"><img class=\"SobekItemWordmark\" src=\"<%BASEURL%>design/wordmarks/" + thisIconRow["Icon_URL"].ToString().Replace("&", "&amp;").Replace("\"", "&quot;") + "\" alt=\"" + newIcon.Title.Replace("&", "&amp;").Replace("\"", "&quot;") + "\" /></a>";
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
					Item_Group_Object.Web.All_Related_Titles.Add(new Related_Titles(relationship, title, "<%BASEURL%>" + bibid + "<%URL_OPTS%>\">"));
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
        /// <param name="Item_Viewer_Priority"> List of the globally defined item viewer priorities </param>
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
					mets_file = Engine_ApplicationCache_Gateway.Settings.Servers.Image_Server_Network + mets_file;
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
        /// <param name="Item_Viewer_Priority">  List of the globally defined item viewer priorities  </param>
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
			catch ( Exception ee )
			{
			    if (ee.Message.Length > 0)
			        return new SobekCM_Item();
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
		    else
		    {
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
		                Package_To_Finalize.Tracking.Tracking_Box = mainItemRow["Tracking_Box"].ToString();
		        }
		        if (mainItemRow["CitationSet"] != DBNull.Value)
		            Package_To_Finalize.Behaviors.CitationSet = mainItemRow["CitationSet"].ToString();

		        // Set more of the sobekcm web portions in the item 
		        Package_To_Finalize.Web.Set_BibID_VID(Package_To_Finalize.BibID, Package_To_Finalize.VID);
		        Package_To_Finalize.Web.Image_Root = Engine_ApplicationCache_Gateway.Settings.Servers.Image_URL;
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

			// Set the aggregations in the package to the aggregation links from the database
			Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Load the aggregations from the database info");
			Package_To_Finalize.Behaviors.Clear_Aggregations();
			foreach (DataRow thisRow in DatabaseInfo.Tables[1].Rows)
			{
				if (!Convert.ToBoolean(thisRow["impliedLink"]))
				{
				    string code = thisRow["Code"].ToString();
				    if (String.Compare(code, "all", StringComparison.OrdinalIgnoreCase) != 0)
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
                    if (String.Compare(code, "all", StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        Package_To_Finalize.Behaviors.Add_Aggregation(code, thisRow["Name"].ToString(), thisRow["Type"].ToString());
                    }
				}
			}

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
						html = "<a href=\"" + link + "\"><img class=\"SobekItemWordmark\" src=\"<%BASEURL%>design/wordmarks/" + image + "\" alt=\"" + name + "\" /></a>";
					}
					else
					{
						html = "<a href=\"" + link + "\" target=\"_blank\"><img class=\"SobekItemWordmark\" src=\"<%BASEURL%>design/wordmarks/" + image + "\" alt=\"" + name + "\" /></a>";
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

            // Add the key/value settings
		    foreach (DataRow settingRow in DatabaseInfo.Tables[7].Rows)
		    {
		        Package_To_Finalize.Behaviors.Settings.Add(new Tuple<string, string>(settingRow["Setting_Key"].ToString(), settingRow["Setting_Value"].ToString()));
		    }

			Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Set the views from a combination of the METS and the database info");

			// Make sure no views were retained from the METS file itself
			Package_To_Finalize.Behaviors.Clear_Views();
			Package_To_Finalize.Behaviors.Clear_Item_Level_Page_Views();

			// If there is no PURL, add one based on how SobekCM operates
			if (Package_To_Finalize.Bib_Info.Location.PURL.Length == 0)
			{
                Package_To_Finalize.Bib_Info.Location.PURL = Engine_ApplicationCache_Gateway.Settings.Servers.System_Base_URL + Package_To_Finalize.BibID + "/" + Package_To_Finalize.VID;
				
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
			foreach (DataRow viewRow in DatabaseInfo.Tables[4].Rows)
			{
				string viewType = viewRow[0].ToString();
				string attribute = viewRow[1].ToString();
				string label = viewRow[2].ToString();
			    float menuOrder = float.Parse(viewRow[3].ToString());
			    bool exclude = bool.Parse(viewRow[4].ToString());

                Package_To_Finalize.Behaviors.Add_View(viewType, label, attribute, menuOrder, exclude);
			}

            // We will continue to set the static page count
            // Step through each page and set the static page count
            Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Set the static page count");
            pageseq = 0;
            List<Page_TreeNode> pages_encountered = new List<Page_TreeNode>();
            foreach (abstract_TreeNode rootNode in Package_To_Finalize.Divisions.Physical_Tree.Roots)
            {
                recurse_through_nodes( rootNode, pages_encountered);
            }
            Package_To_Finalize.Web.Static_PageCount = pages_encountered.Count;

			Tracer.Add_Trace("SobekCM_METS_Based_ItemBuilder.Finish_Building_Item", "Done merging the database information with the resource object");

		}

        private void recurse_through_nodes(abstract_TreeNode Node, List<Page_TreeNode> PagesEncountered)
        {
            if (Node.Page)
            {
                Page_TreeNode pageNode = (Page_TreeNode)Node;
                if (!PagesEncountered.Contains(pageNode))
                {
                    pageseq++;
                }
            }
            else
            {
                Division_TreeNode divNode = (Division_TreeNode)Node;
                foreach (abstract_TreeNode childNode in divNode.Nodes)
                {
                    recurse_through_nodes(childNode, PagesEncountered);
                }
            }
        }



		#endregion
	}
}