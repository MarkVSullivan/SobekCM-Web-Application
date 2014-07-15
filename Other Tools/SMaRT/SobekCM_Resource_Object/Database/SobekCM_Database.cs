using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.ApplicationBlocks.Data;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Metadata_Modules;

namespace SobekCM.Resource_Object.Database
{

	/// <summary> SobekCM_Database is the main object used to query the database for information </summary>
	/// <remarks> This class contains a static constructor and contains mostly static 
	/// members.   Since this is a database class, it does not make sense to have multiple
	/// instances to access a single database. <br /><br />
	/// Object created by Mark V Sullivan and Ying Tang (2006) for University of Florida's Digital Library Center.</remarks>
	public class SobekCM_Database
	{
		/// <summary>Method used to get all SobekCM icons</summary>
		/// <returns></returns>
		public static DataRow TEMP_Get_Group_Info_By_BibID( string BibID )
		{
			try
			{
				// build the parameter list
				SqlParameter[] param_list = new SqlParameter[1];
				param_list[0] = new SqlParameter("@bibid", BibID);

				//Execute this non-query stored procedure
				DataSet returnVal = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "TEMP_Get_Group_Info_By_BibID", param_list);

				if (returnVal == null)
					return null;

				if (returnVal.Tables[0].Rows.Count == 0)
					return null;

				return returnVal.Tables[0].Rows[0];
			}
			catch (Exception ee)
			{
				exception_caught("TEMP_Get_Group_Info_By_BibID", ee);
				return null;
			}
		}

		/// <summary> Private constant string variable stores the connection string 
		/// to get to the SobekCM Database on the SQL server. </summary>
		private static string connectionString = "data source=lib-ufdc-cache\\UFDCPROD;initial catalog=UFDC_prod;integrated security=SSPI;persist security info=False;workstation id=WSID3246;packet size=4096";

		/// <summary> Static constructor for the SobekCM_Database class </summary>
		static SobekCM_Database()
		{
			// Do nothing
		}

		#region Methods to save a single digital resource to SobekCM

		/// <summary> Save a brand new bibliographic item to the SobekCM database </summary>
		/// <param name="thisPackage"> New bibliographic package to save to the SobekCM database </param>
		/// <param name="online_submit"> Flag indicates if this was submitted via the online interface or some other means </param>
		/// <param name="textFlag"> Flag indicates if there are text files asscociated with this item, which would case the text searchable flag to be set to true </param>
		/// <param name="userid"> User id of the user that submitted this item, to associate with the user's submitted folder </param>
		/// <param name="username"> Name of the user that submitted this item, for the worklog history </param>
		/// <param name="usernotes"> Any user notes entered while sumitting this item </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This saves the item information, the serial hierarchy, the behaviors, tracking information, and links 
		/// to the user and the user's folders, all in a single procedure call </remarks>
		public static bool Save_New_Digital_Resource(SobekCM_Item thisPackage, bool textFlag, bool online_submit, string username, string usernotes, int userid )
		{
			// Save the group information ( group, interfaces, links to collections ) for this item
            thisPackage.Web.GroupID = Save_Item_Group_Information(thisPackage, DateTime.Now);

			// Try to get the year
			int year = -1;
			if (thisPackage.Bib_Info.Origin_Info.Date_Issued.Length > 0)
			{
				if (thisPackage.Bib_Info.Origin_Info.Date_Issued.Length == 4)
				{
					try
					{
						year = Convert.ToInt32(thisPackage.Bib_Info.Origin_Info.Date_Issued);
					}
					catch { }
				}

				if (year == -1)
				{
					try
					{
						DateTime date = Convert.ToDateTime(thisPackage.Bib_Info.Origin_Info.Date_Issued);
						year = date.Year;
					}
					catch { }
				}
			}

			// Get the display pub date
			string pubdate = thisPackage.Bib_Info.Origin_Info.Date_Issued;
			if (pubdate.Length == 0)
				pubdate = thisPackage.Bib_Info.Origin_Info.Date_Created;

			// Get the spatial display and subjects information
			StringBuilder spatialDisplayBuilder = new StringBuilder();
			StringBuilder subjectsDisplayBuilder = new StringBuilder();
			Bib_Info.Subject_Info_HierarchicalGeographic hierarchicalSubject;
			Bib_Info.Subject_Info_Standard standardSubject;
			foreach (Bib_Info.Subject_Info subject in thisPackage.Bib_Info.Subjects)
			{
				if (subject.Class_Type == Bib_Info.Subject_Info_Type.Hierarchical_Spatial)
				{
					hierarchicalSubject = (Bib_Info.Subject_Info_HierarchicalGeographic)subject;
				}

				if (subject.Class_Type == Bib_Info.Subject_Info_Type.Standard)
				{
					standardSubject = (Bib_Info.Subject_Info_Standard)subject;
					string subjectText = standardSubject.ToString(false);
					if (subjectsDisplayBuilder.Length > 0)
						subjectsDisplayBuilder.Append("|");
					subjectsDisplayBuilder.Append(subjectText);
				}
			}

			// Get the publishers
			StringBuilder publisher_builder = new StringBuilder();
			foreach (Bib_Info.Publisher_Info thisPublisher in thisPackage.Bib_Info.Publishers)
			{
				if (publisher_builder.Length > 0)
				{
					publisher_builder.Append("|" + thisPublisher.ToString());
				}
				else
				{
					publisher_builder.Append(thisPublisher.ToString());
				}
			}

			// Get the authors
			StringBuilder author_builder = new StringBuilder();
			string mainAuthor = String.Empty;
			if (thisPackage.Bib_Info.hasMainEntityName)
				mainAuthor = thisPackage.Bib_Info.Main_Entity_Name.ToString();
			if ((mainAuthor.Length > 0) && (mainAuthor.IndexOf("unknown") < 0))
			{
				author_builder.Append(mainAuthor);
			}
			if (thisPackage.Bib_Info.Names_Count > 0)
			{
				foreach (Bib_Info.Name_Info thisAuthor in thisPackage.Bib_Info.Names)
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
			if (thisPackage.Bib_Info.Donor != null)
			{
				string donor_temp = thisPackage.Bib_Info.Donor.ToString();
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
            double spatial_distance = -1;
            //if (thisPackage.Bib_Info.hasCoordinateInformation)
            //{
            //    spatial_kml = thisPackage.Bib_Info.Coordinates.SobekCM_Main_Spatial_String;
            //    spatial_distance = thisPackage.Bib_Info.Coordinates.SobekCM_Main_Spatial_Distance;
            //}

			// Get the source and holding codes and the institution display information
			StringBuilder institutionDisplayBuilder = new StringBuilder();
			string source_code = thisPackage.Bib_Info.Source.Code;
			string holding_code = String.Empty;
			if ((source_code.Length > 0) && (source_code[0] != 'i') && (source_code[0] != 'I'))
			{
				source_code = "i" + source_code;
			}
			if ((source_code.Length > 2) && (source_code.ToUpper().IndexOf("II") == 0))
				source_code = source_code.Substring(1);
			if ((source_code.ToUpper().IndexOf("UF") != 0) && (source_code.ToUpper().IndexOf("IUF") != 0))
			{
				if (thisPackage.Bib_Info.Source.Statement.Length > 0)
					institutionDisplayBuilder.Append(thisPackage.Bib_Info.Source.Statement);
			}

			string purl = String.Empty;
			if (thisPackage.Bib_Info.hasLocationInformation)
			{
				purl = thisPackage.Bib_Info.Location.PURL;
				holding_code = thisPackage.Bib_Info.Location.Holding_Code;
				if ((holding_code.ToUpper().IndexOf("UF") != 0) && (holding_code.ToUpper().IndexOf("IUF") != 0))
				{
					if (institutionDisplayBuilder.Length == 0)
					{
						institutionDisplayBuilder.Append(thisPackage.Bib_Info.Location.Holding_Name);
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
			if ((!thisPackage.Divisions.Physical_Tree.Has_Files) && (!thisPackage.Divisions.Download_Tree.Has_Files) && (thisPackage.Bib_Info.Location.Other_URL.Length > 0))
			{
				link = thisPackage.Bib_Info.Location.Other_URL;
			}

			// Pull some data out (which may or may not exist)
			string icon1_name = String.Empty;
			string icon2_name = String.Empty;
			string icon3_name = String.Empty;
			string icon4_name = String.Empty;
			string icon5_name = String.Empty;

			// Add the icon names, if they exist
			thisPackage.Behaviors.Dedupe_Wordmarks();
			if (thisPackage.Behaviors.Wordmarks.Count > 0)
				icon1_name = thisPackage.Behaviors.Wordmarks[0].Code;
			if (thisPackage.Behaviors.Wordmarks.Count > 1)
				icon2_name = thisPackage.Behaviors.Wordmarks[1].Code;
			if (thisPackage.Behaviors.Wordmarks.Count > 2)
				icon3_name = thisPackage.Behaviors.Wordmarks[2].Code;
			if (thisPackage.Behaviors.Wordmarks.Count > 3)
				icon4_name = thisPackage.Behaviors.Wordmarks[3].Code;
			if (thisPackage.Behaviors.Wordmarks.Count > 4)
				icon5_name = thisPackage.Behaviors.Wordmarks[4].Code;

			// Get the list of aggregation codes
			List<string> aggregationCodes = new List<string>();
			foreach (Aggregation_Info aggregation in thisPackage.Behaviors.Aggregations)
			{
				aggregationCodes.Add(aggregation.Code);
			}

			// Ensure there are at least seven here
			while (aggregationCodes.Count < 8)
				aggregationCodes.Add(String.Empty);

			// Collect the behavior information
			List<int> view_type_ids = new List<int>();
			List<string> view_labels = new List<string>();
			List<string> view_attributes = new List<string>();
			foreach (View_Object thisView in thisPackage.Behaviors.Item_Level_Page_Views)
			{
				switch (thisView.View_Type)
				{
					case View_Enum.JPEG:
						if (!view_type_ids.Contains(1))
						{
							view_type_ids.Add(1);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;

					case View_Enum.JPEG2000:
						if (!view_type_ids.Contains(2))
						{
							view_type_ids.Add(2);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;

					case View_Enum.TEXT:
						if (!view_type_ids.Contains(3))
						{
							view_type_ids.Add(3);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;
				}
			}

			foreach (View_Object thisView in thisPackage.Behaviors.Views)
			{
				switch (thisView.View_Type)
				{
					case View_Enum.RELATED_IMAGES:
						view_type_ids.Add(8);
						view_labels.Add(String.Empty);
						view_attributes.Add(String.Empty);
						break;

					case View_Enum.JPEG:
						if (!view_type_ids.Contains(1))
						{
							view_type_ids.Add(1);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;

					case View_Enum.JPEG2000:
						if (!view_type_ids.Contains(2))
						{
							view_type_ids.Add(2);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;

					case View_Enum.TEXT:
						if (!view_type_ids.Contains(3))
						{
							view_type_ids.Add(3);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;

					case View_Enum.PAGE_TURNER:
						if (!view_type_ids.Contains(4))
						{
							view_type_ids.Add(4);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;

					case View_Enum.GOOGLE_MAP:
						if (!view_type_ids.Contains(5))
						{
							view_type_ids.Add(5);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;

					case View_Enum.HTML:
						view_type_ids.Add(6);
						view_labels.Add(thisView.Label);
						view_attributes.Add(thisView.Attributes);
						break;

					case View_Enum.HTML_MAP:
						view_type_ids.Add(7);
						view_labels.Add(thisView.Label);
						view_attributes.Add(thisView.Attributes);
						break;

					case View_Enum.TOC:
						view_type_ids.Add(9);
						view_labels.Add(String.Empty);
						view_attributes.Add(String.Empty);
						break;
				}
			}

			while (view_type_ids.Count < 6)
			{
				view_type_ids.Add(0);
				view_labels.Add(String.Empty);
				view_attributes.Add(String.Empty);
			}

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

			if (thisPackage.Behaviors.hasSerialInformation)
			{
				if (thisPackage.Behaviors.Serial_Info.Count > 0)
				{
					Serial_Info.Single_Serial_Hierarchy level1 = thisPackage.Behaviors.Serial_Info[0];
					level1_index = level1.Order;
					level1_text = level1.Display;
				}

				if (thisPackage.Behaviors.Serial_Info.Count > 1)
				{
					Serial_Info.Single_Serial_Hierarchy level2 = thisPackage.Behaviors.Serial_Info[1];
					level2_index = level2.Order;
					level2_text = level2.Display;
				}

				if (thisPackage.Behaviors.Serial_Info.Count > 2)
				{
					Serial_Info.Single_Serial_Hierarchy level3 = thisPackage.Behaviors.Serial_Info[2];
					level3_index = level3.Order;
					level3_text = level3.Display;
				}

				if (thisPackage.Behaviors.Serial_Info.Count > 3)
				{
					Serial_Info.Single_Serial_Hierarchy level4 = thisPackage.Behaviors.Serial_Info[3];
					level4_index = level4.Order;
					level4_text = level4.Display;
				}

				if (thisPackage.Behaviors.Serial_Info.Count > 4)
				{
					Serial_Info.Single_Serial_Hierarchy level5 = thisPackage.Behaviors.Serial_Info[4];
					level5_index = level5.Order;
					level5_text = level5.Display;
				}
			}


			try
			{
				int i = 0;
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[89];
                param_list[i++] = new SqlParameter("@GroupID", thisPackage.Web.GroupID);
				param_list[i++] = new SqlParameter("@VID", thisPackage.VID);
				param_list[i++] = new SqlParameter("@PageCount", thisPackage.Divisions.Page_Count);
				param_list[i++] = new SqlParameter("@FileCount", thisPackage.Divisions.Files.Count);
				param_list[i++] = new SqlParameter("@Title", thisPackage.Bib_Info.Main_Title.NonSort + thisPackage.Bib_Info.Main_Title.Title);
				param_list[i++] = new SqlParameter("@SortTitle", thisPackage.Bib_Info.sortSafeTitle(thisPackage.Bib_Info.Main_Title.Title, false));
				param_list[i++] = new SqlParameter("@AccessMethod", 1);
				param_list[i++] = new SqlParameter("@Link", link);
				param_list[i++] = new SqlParameter("@CreateDate", DateTime.Now);
				param_list[i++] = new SqlParameter("@PubDate", pubdate);
				param_list[i++] = new SqlParameter("@SortDate", thisPackage.Bib_Info.sortSafeDate(pubdate));
				param_list[i++] = new SqlParameter("@Author", author_builder.ToString());
				param_list[i++] = new SqlParameter("@Spatial_KML", spatial_kml);
				param_list[i++] = new SqlParameter("@Spatial_KML_Distance", spatial_distance);
				param_list[i++] = new SqlParameter("@DiskSize_MB", thisPackage.DiskSize_MB);
				param_list[i++] = new SqlParameter("@Spatial_Display", spatialDisplayBuilder.ToString());
				param_list[i++] = new SqlParameter("@Institution_Display", institutionDisplayBuilder.ToString());
				param_list[i++] = new SqlParameter("@Edition_Display", thisPackage.Bib_Info.Origin_Info.Edition);
				param_list[i++] = new SqlParameter("@Material_Display", materialDisplayBuilder.ToString());
				param_list[i++] = new SqlParameter("@Measurement_Display", measurements);
				param_list[i++] = new SqlParameter("@StylePeriod_Display", stylePeriodDisplayBuilder.ToString());
				param_list[i++] = new SqlParameter("@Technique_Display", techniqueDisplayBuilder.ToString());
				param_list[i++] = new SqlParameter("@Subjects_Display", subjectsDisplayBuilder.ToString());
				param_list[i++] = new SqlParameter("@Donor", donor);
				param_list[i++] = new SqlParameter("@Publisher", publisher_builder.ToString());
				param_list[i++] = new SqlParameter("@TextSearchable", textFlag);
				param_list[i++] = new SqlParameter("@MainThumbnail", thisPackage.Behaviors.Main_Thumbnail);
				param_list[i++] = new SqlParameter("@MainJPEG", thisPackage.Behaviors.Main_Thumbnail.Replace("thm", ""));
				param_list[i++] = new SqlParameter("@IP_Restriction_Mask", (short)thisPackage.Behaviors.IP_Restriction_Membership);
				param_list[i++] = new SqlParameter("@CheckoutRequired", thisPackage.Behaviors.CheckOut_Required);
				param_list[i++] = new SqlParameter("@AggregationCode1", aggregationCodes[0]);
				param_list[i++] = new SqlParameter("@AggregationCode2", aggregationCodes[1]);
				param_list[i++] = new SqlParameter("@AggregationCode3", aggregationCodes[2]);
				param_list[i++] = new SqlParameter("@AggregationCode4", aggregationCodes[3]);
				param_list[i++] = new SqlParameter("@AggregationCode5", aggregationCodes[4]);
				param_list[i++] = new SqlParameter("@AggregationCode6", aggregationCodes[5]);
				param_list[i++] = new SqlParameter("@AggregationCode7", aggregationCodes[6]);
				param_list[i++] = new SqlParameter("@AggregationCode8", aggregationCodes[7]);
				param_list[i++] = new SqlParameter("@HoldingCode", holding_code);
				param_list[i++] = new SqlParameter("@SourceCode", source_code);
				param_list[i++] = new SqlParameter("@Icon1_Name", icon1_name);
				param_list[i++] = new SqlParameter("@Icon2_Name", icon2_name);
				param_list[i++] = new SqlParameter("@Icon3_Name", icon3_name);
				param_list[i++] = new SqlParameter("@Icon4_Name", icon4_name);
				param_list[i++] = new SqlParameter("@Icon5_Name", icon5_name);
				param_list[i++] = new SqlParameter("@Viewer1_TypeID", view_type_ids[0]);
				param_list[i++] = new SqlParameter("@Viewer1_Label", view_labels[0]);
				param_list[i++] = new SqlParameter("@Viewer1_Attribute", view_attributes[0]);
				param_list[i++] = new SqlParameter("@Viewer2_TypeID", view_type_ids[1]);
				param_list[i++] = new SqlParameter("@Viewer2_Label", view_labels[1]);
				param_list[i++] = new SqlParameter("@Viewer2_Attribute", view_attributes[1]);
				param_list[i++] = new SqlParameter("@Viewer3_TypeID", view_type_ids[2]);
				param_list[i++] = new SqlParameter("@Viewer3_Label", view_labels[2]);
				param_list[i++] = new SqlParameter("@Viewer3_Attribute", view_attributes[2]);
				param_list[i++] = new SqlParameter("@Viewer4_TypeID", view_type_ids[3]);
				param_list[i++] = new SqlParameter("@Viewer4_Label", view_labels[3]);
				param_list[i++] = new SqlParameter("@Viewer4_Attribute", view_attributes[3]);
				param_list[i++] = new SqlParameter("@Viewer5_TypeID", view_type_ids[4]);
				param_list[i++] = new SqlParameter("@Viewer5_Label", view_labels[4]);
				param_list[i++] = new SqlParameter("@Viewer5_Attribute", view_attributes[4]);
				param_list[i++] = new SqlParameter("@Viewer6_TypeID", view_type_ids[5]);
				param_list[i++] = new SqlParameter("@Viewer6_Label", view_labels[5]);
				param_list[i++] = new SqlParameter("@Viewer6_Attribute", view_attributes[5]);

				if (level1_index >= 0)
				{
					param_list[i++] = new SqlParameter("@Level1_Text", level1_text);
					param_list[i++] = new SqlParameter("@Level1_Index", level1_index);
				}
				else
				{
					param_list[i++] = new SqlParameter("@Level1_Text", DBNull.Value);
					param_list[i++] = new SqlParameter("@Level1_Index", DBNull.Value);
				}

				if (level2_index >= 0)
				{
					param_list[i++] = new SqlParameter("@Level2_Text", level2_text);
					param_list[i++] = new SqlParameter("@Level2_Index", level2_index);
				}
				else
				{
					param_list[i++] = new SqlParameter("@Level2_Text", DBNull.Value);
					param_list[i++] = new SqlParameter("@Level2_Index", DBNull.Value);
				}

				if (level3_index >= 0)
				{
					param_list[i++] = new SqlParameter("@Level3_Text", level3_text);
					param_list[i++] = new SqlParameter("@Level3_Index", level3_index);
				}
				else
				{
					param_list[i++] = new SqlParameter("@Level3_Text", DBNull.Value);
					param_list[i++] = new SqlParameter("@Level3_Index", DBNull.Value);
				}

				if (level4_index >= 0)
				{
					param_list[i++] = new SqlParameter("@Level4_Text", level4_text);
					param_list[i++] = new SqlParameter("@Level4_Index", level4_index);
				}
				else
				{
					param_list[i++] = new SqlParameter("@Level4_Text", DBNull.Value);
					param_list[i++] = new SqlParameter("@Level4_Index", DBNull.Value);
				}

				if (level5_index >= 0)
				{
					param_list[i++] = new SqlParameter("@Level5_Text", level5_text);
					param_list[i++] = new SqlParameter("@Level5_Index", level5_index);
				}
				else
				{
					param_list[i++] = new SqlParameter("@Level5_Text", DBNull.Value);
					param_list[i++] = new SqlParameter("@Level5_Index", DBNull.Value);
				}

				param_list[i++] = new SqlParameter("@VIDSource", thisPackage.Tracking.VID_Source);
				param_list[i++] = new SqlParameter("@CopyrightIndicator", 0);
				param_list[i++] = new SqlParameter("@Born_Digital", thisPackage.Tracking.Born_Digital);
				param_list[i++] = new SqlParameter("@Dark", thisPackage.Behaviors.Dark_Flag);
				if (thisPackage.Tracking.Material_Received_Date.HasValue)
				{
					param_list[i++] = new SqlParameter("@Material_Received_Date", thisPackage.Tracking.Material_Received_Date.Value);
					param_list[i++] = new SqlParameter("@Material_Recd_Date_Estimated", thisPackage.Tracking.Material_Rec_Date_Estimated);
				}
				else
				{
					param_list[i++] = new SqlParameter("@Material_Received_Date", DBNull.Value);
					param_list[i++] = new SqlParameter("@Material_Recd_Date_Estimated", false);
				}

				if (thisPackage.Tracking.Disposition_Advice <= 0)
					param_list[i++] = new SqlParameter("@Disposition_Advice", DBNull.Value);
				else
					param_list[i++] = new SqlParameter("@Disposition_Advice", thisPackage.Tracking.Disposition_Advice);

				param_list[i++] = new SqlParameter("@Disposition_Advice_Notes", thisPackage.Tracking.Disposition_Advice_Notes);
				param_list[i++] = new SqlParameter("@Internal_Comments", thisPackage.Tracking.Internal_Comments);
				param_list[i++] = new SqlParameter("@Tracking_Box", thisPackage.Tracking.Tracking_Box);
				param_list[i++] = new SqlParameter("@Online_Submit", online_submit);
				param_list[i++] = new SqlParameter("@User", username);
				param_list[i++] = new SqlParameter("@UserNotes", usernotes);
				param_list[i++] = new SqlParameter("@UserID_To_Link", userid);

				param_list[i] = new SqlParameter("@ItemID", -1);
				param_list[i++].Direction = ParameterDirection.InputOutput;
				param_list[i] = new SqlParameter("@New_VID", "00000");
				param_list[i++].Direction = ParameterDirection.InputOutput;

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Save_New_Item", param_list);


				// Save the item id and VID into the package
                thisPackage.Web.ItemID = (int)param_list[87].Value;
				thisPackage.VID = param_list[88].Value.ToString();
			}
			catch (Exception ee)
			{
				// Pass this exception onto the method to handle it
				exception_caught("SobekCM_Save_Item", ee);
				return false;
			}

			// If the material reced date is filled in and there are notes about the receipt,
			// add a workflow for this 
			if ((thisPackage.Tracking.Material_Received_Date.HasValue) && (thisPackage.Tracking.Material_Received_Notes.Length > 0))
			{
                Update_Material_Received(thisPackage.Web.ItemID, thisPackage.Tracking.Material_Received_Date.Value, false, username, thisPackage.Tracking.Material_Received_Notes);
			}

			// Save any additional metadat present in the item
			Save_Item_Metadata_Information(thisPackage);

            //// Save coordinates, if there are some
            //if (thisPackage.Bib_Info.hasCoordinateInformation)
            //{
            //    Save_Coordinates_Footprint(thisPackage, thisPackage.Behaviors.ItemID);
            //}

			return true;

		}
		
		/// <summary> Saves this bibliographic package to the SobekCM database </summary>
		/// <param name="thisPackage"> Bibliographic package to save to the SobekCM database </param>
		/// <returns> Item ID for this... (may need it for something in the future?) </returns>
		public static int Save_Digital_Resource( SobekCM_Item thisPackage )
		{
			return Save_Digital_Resource(thisPackage, DateTime.Now, false);
		}

		/// <summary> Saves this bibliographic package to the SobekCM database </summary>
		/// <param name="thisPackage"> Bibliographic package to save to the SobekCM database </param>
		/// <param name="createDate"> Date this item was originally created </param>
		/// <param name="Existed"> Flag indicates if this item pre-existed </param>
		/// <returns> Item ID for this... (may need it for something in the future?) </returns>
		public static int Save_Digital_Resource(SobekCM_Item thisPackage, DateTime createDate, bool Existed)
		{
			// Save the group information ( group, interfaces, links to collections ) for this item
            thisPackage.Web.GroupID = Save_Item_Group_Information(thisPackage, createDate);

			// Save the actual item information ( item, downloads, icons, link to group ) for this item
            Save_Item_Information(thisPackage, thisPackage.Web.GroupID, createDate);

			// Save the serial hierarchy information, if there was one
			if (( thisPackage.Behaviors.hasSerialInformation ) && (  thisPackage.Behaviors.Serial_Info.Count > 0 ))
			{
                Save_Serial_Hierarchy_Information(thisPackage, thisPackage.Web.GroupID, thisPackage.Web.ItemID);
			}

			// Save any additional metadat present in the item
			Save_Item_Metadata_Information(thisPackage);

            // Step through all the metadata modules and allow the modules to save to the database
            if (thisPackage.Metadata_Modules_Count > 0)
            {
                string error_message;
                foreach (iMetadata_Module thisModule in thisPackage.All_Metadata_Modules)
                {
                    thisModule.Save_Additional_Info_To_Database(thisPackage.Web.ItemID, connectionString, thisPackage, out error_message);
                }
            }

			// Return the item id
            return thisPackage.Web.ItemID;
		}



		private static void Save_Streets_and_Features_To_Item(SobekCM_Item thisPackage, int ItemID)
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

		private static void Save_GeoRegion_Links( SobekCM_Item thisPackage, int ItemID )
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
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_All_Corporations");
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
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_All_Regions");
				return tempSet.Tables[0];
			}
			catch (Exception ee)
			{
				exception_caught("SobekCM_Get_All_Regions", ee);
				return null;
			}
		}

		private static bool Save_Corporation( string CorpAuthCode, string CorporateName )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[2];
				param_list[0] = new SqlParameter("@CorpAuthCode", CorpAuthCode);
				param_list[1] = new SqlParameter("@CorporateName", CorporateName);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Save_Corporation", param_list);

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
				SqlParameter[] param_list = new SqlParameter[1];
				param_list[0] = new SqlParameter( "@ItemID", ItemID );

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Clear_Features_Streets_By_Item", param_list);

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
				SqlParameter[] param_list = new SqlParameter[1];
				param_list[0] = new SqlParameter( "@ItemID", ItemID );

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Clear_Region_Link_By_Item", param_list);

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
				SqlParameter[] param_list = new SqlParameter[1];
				param_list[0] = new SqlParameter("@ItemID", ItemID);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Lock_Item", param_list);

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

		private static bool Save_Region_Item_Link( int ItemID, string GeoAuthCode, string Name, string Type, string P_Code, string P_Name, string P_Type, string P2_Code, string P2_Name, string P2_Type, 
			string P3_Code, string P3_Name, string P3_Type, string P4_Code, string P4_Name, string P4_Type, string P5_Code, string P5_Name, string P5_Type )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[19];
				param_list[0] = new SqlParameter( "@GeoAuthCode", GeoAuthCode );
				param_list[1] = new SqlParameter( "@ItemID", ItemID );
				param_list[2] = new SqlParameter( "@RegionName", Name );
				param_list[3] = new SqlParameter( "@RegionType", Type );
				param_list[4] = new SqlParameter( "@P_RegionAuthCode", P_Code );
				param_list[5] = new SqlParameter( "@P_RegionName", P_Name );
				param_list[6] = new SqlParameter( "@P_RegionType", P_Type );
				param_list[7] = new SqlParameter( "@P2_RegionAuthCode", P2_Code );
				param_list[8] = new SqlParameter( "@P2_RegionName", P2_Name );
				param_list[9] = new SqlParameter( "@P2_RegionType", P2_Type );
				param_list[10] = new SqlParameter( "@P3_RegionAuthCode", P3_Code );
				param_list[11] = new SqlParameter( "@P3_RegionName", P3_Name );
				param_list[12] = new SqlParameter( "@P3_RegionType", P3_Type );
				param_list[13] = new SqlParameter( "@P4_RegionAuthCode", P4_Code );
				param_list[14] = new SqlParameter( "@P4_RegionName", P4_Name );
				param_list[15] = new SqlParameter( "@P4_RegionType", P4_Type );
				param_list[16] = new SqlParameter( "@P5_RegionAuthCode", P5_Code );
				param_list[17] = new SqlParameter( "@P5_RegionName", P5_Name );
				param_list[18] = new SqlParameter( "@P5_RegionType", P5_Type );

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery( connectionString, CommandType.StoredProcedure, "SobekCM_Save_Region_Item_Link", param_list );

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

		private static bool Save_Feature( string FeatAuthCode, string FeatureName, string LocationDesc, string CorpAuthCode, bool AA_Indicated,
			string Albers_X, string Albers_Y, string Latitude, string Longitude, string FeatureType, int FeatureTypeYear, int PageID1,
			int PageID2, int PageID3, int PageID4, int PageID5 )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[16];
				param_list[0] = new SqlParameter("@FeatAuthCode", FeatAuthCode);
				param_list[1] = new SqlParameter("@FeatureName", FeatureName);
				param_list[2] = new SqlParameter("@LocationDesc", LocationDesc);
				param_list[3] = new SqlParameter("@CorpAuthCode", CorpAuthCode);
				param_list[4] = new SqlParameter("@AA_Indicated", AA_Indicated);
				param_list[5] = new SqlParameter("@Albers_X", Albers_X);
				param_list[6] = new SqlParameter("@Albers_Y", Albers_Y);
				param_list[7] = new SqlParameter("@Latitude", Latitude);
				param_list[8] = new SqlParameter("@Longitude", Longitude);
				param_list[9] = new SqlParameter("@FeatureType", FeatureType);
				param_list[10] = new SqlParameter("@FeatureTypeYear", FeatureTypeYear);
				param_list[11] = new SqlParameter("@PageID1", PageID1);
				param_list[12] = new SqlParameter("@PageID2", PageID2);
				param_list[13] = new SqlParameter("@PageID3", PageID3);
				param_list[14] = new SqlParameter("@PageID4", PageID4);
				param_list[15] = new SqlParameter("@PageID5", PageID5);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Save_Feature", param_list);

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

		private static bool Save_Street( string StreetAuthCode, string StreetName, long StartAddress, long EndAddress, 
			string StreetDirection, string StreetSide, string SegmentDesc, int PageID )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[8];
				param_list[0] = new SqlParameter("@StreetAuthCode", StreetAuthCode);
				param_list[1] = new SqlParameter("@StreetName", StreetName);
				param_list[2] = new SqlParameter("@StartAddress", StartAddress);
				param_list[3] = new SqlParameter("@EndAddress", EndAddress);
				param_list[4] = new SqlParameter("@StreetDirection", StreetDirection);
				param_list[5] = new SqlParameter("@StreetSide", StreetSide);
				param_list[6] = new SqlParameter("@SegmentDesc", SegmentDesc);
				param_list[7] = new SqlParameter("@PageID", PageID);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Save_Street_Page_Link", param_list);

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
		/// <param name="thisPackage"> Bibliographic package to save to the SobekCM database </param>
		/// <param name="createDate"> Day this item group was created</param>
		/// <returns> Group ID for this package </returns>
		private static int Save_Item_Group_Information( SobekCM_Item thisPackage, DateTime createDate )
		{
			// Pull some data out (which may or may not exist)
			string groupTitle = thisPackage.Bib_Info.Main_Title.Title;

			// Try to determine an integer value for ALEPH number
			string ALEPH = thisPackage.Bib_Info.ALEPH_Record;
			int ALEPH_Number = -1;
			if (ALEPH.Length > 0)
			{
				bool isNumber = true;
				foreach (char thisChar in ALEPH)
				{
					if (!Char.IsNumber(thisChar))
					{
						isNumber = false;
						break;
					}
				}

				if (isNumber)
				{
					try
					{
						ALEPH_Number = Convert.ToInt32(ALEPH);
					}
					catch
					{

					}
				}
			}

			// Try to determine a long integer value for OCLC number
			string OCLC = thisPackage.Bib_Info.OCLC_Record;
			int OCLC_Number = -1;
			if (OCLC.Length > 0)
			{
				bool isNumber = true;
				foreach (char thisChar in OCLC)
				{
					if (!Char.IsNumber(thisChar))
					{
						isNumber = false;
						break;
					}
				}

				if (isNumber)
				{
					try
					{
						OCLC_Number = Convert.ToInt32(OCLC);
					}
					catch
					{

					}
				}
			}

			// Look for primary identifier
			string primary_alternate_type = thisPackage.Behaviors.Primary_Identifier.Type;
			string primary_alternate_id = thisPackage.Behaviors.Primary_Identifier.Identifier;
			string accession_type = String.Empty;
			string accession_id = String.Empty;
			if (( thisPackage.Bib_Info.Identifiers_Count > 0 ) && (primary_alternate_id.Length == 0) || (primary_alternate_type.Length == 0))
			{
				// Get the type here
			    bool artifact = thisPackage.Bib_Info.SobekCM_Type == SobekCM.Resource_Object.Bib_Info.TypeOfResource_SobekCM_Enum.Artifact;

			    // Step through the identifiers
				foreach (Bib_Info.Identifier_Info thisIdentifier in thisPackage.Bib_Info.Identifiers)
				{
					if (thisIdentifier.Type.IndexOf("*") >= 0)
					{
						primary_alternate_type = thisIdentifier.Type.Replace("*", "");
						primary_alternate_id = thisIdentifier.Identifier;
						break;
					}
					else if ( artifact )
					{
						if (thisIdentifier.Type.ToUpper().IndexOf("ACCESSION") >= 0)
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
            Save_Item_Group_Args saveArgs = Save_Item_Group(thisPackage.BibID, groupTitle, thisPackage.Bib_Info.sortSafeTitle(groupTitle, true), thisPackage.Bib_Info.SobekCM_Type_String, thisPackage.Web.File_Root, String.Empty, false, createDate, OCLC_Number, ALEPH_Number, thisPackage.Tracking.Large_Format, thisPackage.Tracking.Track_By_Month, thisPackage.Tracking.Never_Overlay_Record, primary_alternate_type, primary_alternate_id);
            thisPackage.Web.GroupID = saveArgs.GroupID;
			thisPackage.BibID = saveArgs.New_BibID;

			// If this was a new bib id, or only a single item belongs to this item group, 
			// try to update the OAI-PMH data stored for this item
            if ((saveArgs.Is_New) || ((thisPackage.Web.Siblings.HasValue) && (thisPackage.Web.Siblings < 1)))
			{
				// Get the OAI-PMH dublin core information
				StringBuilder oaiDataBuilder = new StringBuilder(1000);
				StringWriter writer = new StringWriter(oaiDataBuilder);
                METS_Sec_ReaderWriters.DC_METS_dmdSec_ReaderWriter.Write_Simple_Dublin_Core(writer, thisPackage.Bib_Info);

                // Also add the URL as identifier
                if (thisPackage.Web.Service_URL.Length > 0)
                    oaiDataBuilder.AppendLine("<dc:identifier>" + thisPackage.Web.Service_URL.Replace("/" + thisPackage.VID, "") + "</dc:identifier>");
				Save_Item_Group_OAI(saveArgs.GroupID, oaiDataBuilder.ToString(), "oai_dc", false);

				writer.Flush();
				writer.Close();
			}

			// If there were web skins, save that as well
            Save_Item_Group_Web_Skins(thisPackage.Web.GroupID, thisPackage);

			// Now, also save any external record numbers ( OCLC, ALEPH, etc.. )
			bool aleph_or_oclc_exists = false;
			if (thisPackage.Bib_Info.Identifiers_Count > 0)
			{
				ReadOnlyCollection<SobekCM.Resource_Object.Bib_Info.Identifier_Info> identifiers = thisPackage.Bib_Info.Identifiers;
				foreach (SobekCM.Resource_Object.Bib_Info.Identifier_Info identifier in identifiers)
				{
					string identifier_type_upper = identifier.Type.ToUpper();
					bool added = false;
					if (identifier_type_upper.IndexOf("OCLC") >= 0)
					{
                        Add_External_Record_Number(thisPackage.Web.GroupID, identifier.Identifier, "OCLC");
						aleph_or_oclc_exists = true;
						added = true;
					}
					if ((!added) && (identifier_type_upper.IndexOf("ALEPH") >= 0))
					{
                        Add_External_Record_Number(thisPackage.Web.GroupID, identifier.Identifier, "ALEPH");
						aleph_or_oclc_exists = true;
						added = true;
					}
					if ((!added) && (identifier_type_upper.IndexOf("LTUF") >= 0))
					{
                        Add_External_Record_Number(thisPackage.Web.GroupID, identifier.Identifier, "LTUF");
						added = true;
					}
					if ((!added) && (identifier_type_upper.IndexOf("LTQF") >= 0))
					{
                        Add_External_Record_Number(thisPackage.Web.GroupID, identifier.Identifier, "LTQF");
						added = true;
					}
					if ((!added) && (identifier_type_upper.IndexOf("LCCN") >= 0))
					{
                        Add_External_Record_Number(thisPackage.Web.GroupID, identifier.Identifier, "LCCN");
						added = true;
					}
					if ((!added) && (identifier_type_upper.IndexOf("ISBN") >= 0))
					{
                        Add_External_Record_Number(thisPackage.Web.GroupID, identifier.Identifier, "ISBN");
						added = true;
					}
					if ((!added) && (identifier_type_upper.IndexOf("ISSN") >= 0))
					{
                        Add_External_Record_Number(thisPackage.Web.GroupID, identifier.Identifier, "ISSN");
						added = true;
					}
				}
			}

			// Set the endeca suppress flag (which may not be actually used)
			if (aleph_or_oclc_exists)
				thisPackage.Behaviors.Suppress_Endeca = false;

            return thisPackage.Web.GroupID;

		}
		
		/// <summary> Adds an external record number to an item group (i.e., OCLC or ALEPH number) </summary>
		/// <param name="GroupID"> Primary key to this item group </param>
		/// <param name="Identifier"> Value for the external identifier </param>
		/// <param name="Type"> String which indicates the type of indentifier </param>
		public static void Add_External_Record_Number( int GroupID, string Identifier, string Type )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[3];
				param_list[0] = new SqlParameter("@groupID", GroupID);
				param_list[1] = new SqlParameter("@extRecordValue", Identifier);
				param_list[2] = new SqlParameter("@extRecordType", Type);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Add_External_Record_Number", param_list);
			}
			catch (Exception ee)
			{
				// Pass this exception onto the method to handle it
				exception_caught("SobekCM_Add_External_Record_Number", ee);
			}
		}
          
		/// <summary> Saves information about single item </summary>
		/// <param name="thisPackage"></param>
		/// <param name="GroupID"></param>
		/// <param name="CreateDate"> Day this item was created </param>
		/// <returns>TRUE if successful, otherwise FALSE </returns>
		private static bool Save_Item_Information( SobekCM_Item thisPackage, int GroupID, DateTime CreateDate )
		{
			// Try to get the year
			int year = -1;
			if ( thisPackage.Bib_Info.Origin_Info.Date_Issued.Length > 0)
			{
				if (thisPackage.Bib_Info.Origin_Info.Date_Issued.Length == 4)
				{
					try
					{
						year = Convert.ToInt32(thisPackage.Bib_Info.Origin_Info.Date_Issued);
					}
					catch { }
				}

				if (year == -1)
				{
					try
					{
						DateTime date = Convert.ToDateTime(thisPackage.Bib_Info.Origin_Info.Date_Issued);
						year = date.Year;
					}
					catch { }
				}
			}

			// Get the display pub date
			string pubdate = thisPackage.Bib_Info.Origin_Info.Date_Issued;
			if (pubdate.Length == 0)
				pubdate = thisPackage.Bib_Info.Origin_Info.Date_Created;

			// Get the spatial display and subjects information
			StringBuilder spatialDisplayBuilder = new StringBuilder();
			StringBuilder subjectsDisplayBuilder = new StringBuilder();
			Bib_Info.Subject_Info_HierarchicalGeographic hierarchicalSubject;
			Bib_Info.Subject_Info_Standard standardSubject;
			foreach (Bib_Info.Subject_Info subject in thisPackage.Bib_Info.Subjects)
			{
				if (subject.Class_Type == Bib_Info.Subject_Info_Type.Hierarchical_Spatial)
				{
					hierarchicalSubject = (Bib_Info.Subject_Info_HierarchicalGeographic)subject;
				}

				if (subject.Class_Type == Bib_Info.Subject_Info_Type.Standard)
				{
					standardSubject = (Bib_Info.Subject_Info_Standard)subject;
					string subjectText = standardSubject.ToString(false);
					if (subjectsDisplayBuilder.Length > 0)
						subjectsDisplayBuilder.Append("|");
					subjectsDisplayBuilder.Append(subjectText);
				}
			}

			// Get the publishers
			StringBuilder publisher_builder = new StringBuilder();
			foreach (Bib_Info.Publisher_Info thisPublisher in thisPackage.Bib_Info.Publishers)
			{
				if (publisher_builder.Length > 0)
				{
					publisher_builder.Append("|" + thisPublisher.ToString());
				}
				else
				{
					publisher_builder.Append(thisPublisher.ToString());
				}
			}

			// Get the authors
			StringBuilder author_builder = new StringBuilder();
			string mainAuthor = String.Empty;
			if ( thisPackage.Bib_Info.hasMainEntityName)
				mainAuthor = thisPackage.Bib_Info.Main_Entity_Name.ToString();
			if ((mainAuthor.Length > 0) && ( mainAuthor.IndexOf("unknown") < 0 ))
			{
				author_builder.Append(mainAuthor);
			}
			if (thisPackage.Bib_Info.Names_Count > 0)
			{
				foreach (Bib_Info.Name_Info thisAuthor in thisPackage.Bib_Info.Names)
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
			if (thisPackage.Bib_Info.Donor != null )
			{
				string donor_temp = thisPackage.Bib_Info.Donor.ToString();
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
			string source_code = thisPackage.Bib_Info.Source.Code;
			string holding_code = String.Empty;
			if ((source_code.Length > 0) && (source_code[0] != 'i') && (source_code[0] != 'I'))
			{
				source_code = "i" + source_code;
			}
			if ((source_code.Length > 2) && (source_code.ToUpper().IndexOf("II") == 0))
				source_code = source_code.Substring(1);
			if ((source_code.ToUpper().IndexOf("UF") != 0) && (source_code.ToUpper().IndexOf("IUF") != 0))
			{
				if (thisPackage.Bib_Info.Source.Statement.Length > 0)
					institutionDisplayBuilder.Append(thisPackage.Bib_Info.Source.Statement);
			}

			string purl = String.Empty;
			if (thisPackage.Bib_Info.hasLocationInformation)
			{
				purl = thisPackage.Bib_Info.Location.PURL;
				holding_code = thisPackage.Bib_Info.Location.Holding_Code;
				if ((holding_code.ToUpper().IndexOf("UF") != 0) && (holding_code.ToUpper().IndexOf("IUF") != 0))
				{
					if ( institutionDisplayBuilder.Length == 0 )
					{
						institutionDisplayBuilder.Append(thisPackage.Bib_Info.Location.Holding_Name);
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
			double spatial_distance = -1;
            //if (thisPackage.Bib_Info.hasCoordinateInformation)
            //{
            //    spatial_kml = thisPackage.Bib_Info.Coordinates.SobekCM_Main_Spatial_String;
            //    spatial_distance = thisPackage.Bib_Info.Coordinates.SobekCM_Main_Spatial_Distance;
            //}

			// Determine the link
			string link = String.Empty;
			if (( !thisPackage.Divisions.Physical_Tree.Has_Files ) && ( !thisPackage.Divisions.Download_Tree.Has_Files ) && ( thisPackage.Bib_Info.Location.Other_URL.Length > 0 ))
			{
				link = thisPackage.Bib_Info.Location.Other_URL;
			}

			// Save the main information, and return the item id
			Save_Item_Args returnVal = Save_Item( GroupID, thisPackage.VID, thisPackage.Divisions.Page_Count, thisPackage.Divisions.Files.Count,
				thisPackage.Bib_Info.Main_Title.NonSort + thisPackage.Bib_Info.Main_Title.Title, thisPackage.Bib_Info.sortSafeTitle(thisPackage.Bib_Info.Main_Title.Title, false),
				link, CreateDate, pubdate, thisPackage.Bib_Info.sortSafeDate(pubdate), holding_code, 
				source_code, author_builder.ToString(), spatial_kml, spatial_distance, thisPackage.DiskSize_MB, donor, publisher_builder.ToString(),
				spatialDisplayBuilder.ToString(),institutionDisplayBuilder.ToString(), thisPackage.Bib_Info.Origin_Info.Edition, materialDisplayBuilder.ToString(),
				measurements, stylePeriodDisplayBuilder.ToString(), techniqueDisplayBuilder.ToString(), subjectsDisplayBuilder.ToString());

			// If this was existing, clear the old data
			if (returnVal.Existing)
			{
				Clear_Old_Item_Info(returnVal.ItemID);
			}

			// Save the item id and VID into the package
            thisPackage.Web.ItemID = returnVal.ItemID;
			thisPackage.VID = returnVal.New_VID;

			//// Save all the behavior information as well
			//Save_Behaviors(thisPackage, textFlag);	

			return true;
		}

		/// <summary> Save all the item behaviors associated with a SobekCM Digital Resource  </summary>
		/// <param name="thisPackage"> Digital resource which needs to have its behaviors saved to the SobekCM database </param>
		/// <param name="textFlag"> Flag indicates if this item has text files </param>
		/// <param name="Mass_Update_Mode"> Flag indicates if this is a mass-update mode, in which case all items within a single item group will be updated </param>
		public static void Save_Behaviors(SobekCM_Item thisPackage, bool textFlag, bool Mass_Update_Mode )
		{
			// Get the source and holding codes
			string source_code = thisPackage.Bib_Info.Source.Code;
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
			if (thisPackage.Behaviors.Wordmarks.Count > 0)
				icon1_name = thisPackage.Behaviors.Wordmarks[0].Code;
			if (thisPackage.Behaviors.Wordmarks.Count > 1)
				icon2_name = thisPackage.Behaviors.Wordmarks[1].Code;
			if (thisPackage.Behaviors.Wordmarks.Count > 2)
				icon3_name = thisPackage.Behaviors.Wordmarks[2].Code;
			if (thisPackage.Behaviors.Wordmarks.Count > 3)
				icon4_name = thisPackage.Behaviors.Wordmarks[3].Code;
			if (thisPackage.Behaviors.Wordmarks.Count > 4)
				icon5_name = thisPackage.Behaviors.Wordmarks[4].Code;

			// Get the list of aggregation codes
			List<string> aggregationCodes = new List<string>();
			foreach (Aggregation_Info aggregation in thisPackage.Behaviors.Aggregations)
			{
				aggregationCodes.Add(aggregation.Code);
			}

			// Ensure there are at least seven here
			while (aggregationCodes.Count < 8)
				aggregationCodes.Add(String.Empty);

			// Collect the behavior information
			List<int> view_type_ids = new List<int>();
			List<string> view_labels = new List<string>();
			List<string> view_attributes = new List<string>();
			foreach (View_Object thisView in thisPackage.Behaviors.Item_Level_Page_Views)
			{
				switch (thisView.View_Type)
				{
					case View_Enum.JPEG:
						if (!view_type_ids.Contains(1))
						{
							view_type_ids.Add(1);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;

					case View_Enum.JPEG2000:
						if (!view_type_ids.Contains(2))
						{
							view_type_ids.Add(2);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;

					case View_Enum.TEXT:
						if (!view_type_ids.Contains(3))
						{
							view_type_ids.Add(3);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;
				}
			}

			foreach ( View_Object thisView in thisPackage.Behaviors.Views)
			{
				switch (thisView.View_Type)
				{
					case View_Enum.RELATED_IMAGES:
						view_type_ids.Add(8);
						view_labels.Add(String.Empty);
						view_attributes.Add(String.Empty);
						break;

					case View_Enum.JPEG:
						if (!view_type_ids.Contains(1))
						{
							view_type_ids.Add(1);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;

					case View_Enum.JPEG2000:
						if (!view_type_ids.Contains(2))
						{
							view_type_ids.Add(2);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;

					case View_Enum.TEXT:
						if (!view_type_ids.Contains(3))
						{
							view_type_ids.Add(3);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;

					case View_Enum.PAGE_TURNER:
						if (!view_type_ids.Contains(4))
						{
							view_type_ids.Add(4);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;

					case View_Enum.GOOGLE_MAP:
						if (!view_type_ids.Contains(5))
						{
							view_type_ids.Add(5);
							view_labels.Add(String.Empty);
							view_attributes.Add(String.Empty);
						}
						break;

					case View_Enum.HTML:
						view_type_ids.Add(6);
						view_labels.Add(thisView.Label);
						view_attributes.Add(thisView.Attributes);
						break;

					case View_Enum.HTML_MAP:
						view_type_ids.Add(7);
						view_labels.Add(thisView.Label);
						view_attributes.Add(thisView.Attributes);
						break;

					case View_Enum.TOC:
						view_type_ids.Add(9);
						view_labels.Add(String.Empty);
						view_attributes.Add(String.Empty);
						break;

                    case View_Enum.TEI:
                        view_type_ids.Add(10);
                        view_labels.Add(String.Empty);
                        view_attributes.Add(String.Empty);
                        break;
				}
			}

			while (view_type_ids.Count < 6)
			{
				view_type_ids.Add(0);
				view_labels.Add(String.Empty);
				view_attributes.Add(String.Empty);
			}

            // Determine flags for restriction and dark
            bool darkFlag = thisPackage.Behaviors.Dark_Flag;
            bool darkFlag_Null = thisPackage.Behaviors.Dark_Flag_Is_Null;
            short ip_restrict = (short)thisPackage.Behaviors.IP_Restriction_Membership;
            bool ip_restrict_Null = thisPackage.Behaviors.IP_Restriction_Membership_Is_Null;

			if (Mass_Update_Mode)
			{
                Mass_Update_Item_Behaviors(thisPackage.Web.GroupID, ip_restrict_Null, ip_restrict,
					thisPackage.Behaviors.CheckOut_Required_Is_Null, thisPackage.Behaviors.CheckOut_Required,
					darkFlag_Null, darkFlag,
					thisPackage.Tracking.Born_Digital_Is_Null, thisPackage.Tracking.Born_Digital,
					aggregationCodes[0], aggregationCodes[1], aggregationCodes[2], aggregationCodes[3], aggregationCodes[4], aggregationCodes[5], aggregationCodes[6],
					aggregationCodes[7], holding_code, source_code, icon1_name, icon2_name, icon3_name, icon4_name, icon5_name,
					view_type_ids[0], view_labels[0], view_attributes[0],
					view_type_ids[1], view_labels[1], view_attributes[1],
					view_type_ids[2], view_labels[2], view_attributes[2],
					view_type_ids[3], view_labels[3], view_attributes[3],
					view_type_ids[4], view_labels[4], view_attributes[4],
					view_type_ids[5], view_labels[5], view_attributes[5]);
			}
			else
			{



                Save_Item_Behaviors(thisPackage.Web.ItemID, textFlag, thisPackage.Behaviors.Main_Thumbnail, 
                    thisPackage.Behaviors.Main_Thumbnail.Replace("thm", ""), ip_restrict,
                    thisPackage.Behaviors.CheckOut_Required, darkFlag, thisPackage.Tracking.Born_Digital, thisPackage.Tracking.Disposition_Advice, thisPackage.Tracking.Disposition_Advice_Notes,
					thisPackage.Tracking.Material_Received_Date, thisPackage.Tracking.Material_Rec_Date_Estimated, thisPackage.Tracking.Tracking_Box, aggregationCodes[0], aggregationCodes[1], aggregationCodes[2], aggregationCodes[3], aggregationCodes[4], aggregationCodes[5], aggregationCodes[6],
					aggregationCodes[7], holding_code, source_code, icon1_name, icon2_name, icon3_name, icon4_name, icon5_name,
					view_type_ids[0], view_labels[0], view_attributes[0],
					view_type_ids[1], view_labels[1], view_attributes[1],
					view_type_ids[2], view_labels[2], view_attributes[2],
					view_type_ids[3], view_labels[3], view_attributes[3],
					view_type_ids[4], view_labels[4], view_attributes[4],
					view_type_ids[5], view_labels[5], view_attributes[5], thisPackage.Behaviors.Left_To_Right );
			}
		}

		/// <summary> Saves the serial hierarchy information for a single item </summary>
		/// <param name="thisPackage"></param>
		/// <param name="GroupID"></param>
		/// <param name="ItemID"></param>
		public static void Save_Serial_Hierarchy_Information( SobekCM_Item thisPackage, int GroupID, int ItemID )
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

			if (thisPackage.Behaviors.hasSerialInformation)
			{
				if (thisPackage.Behaviors.Serial_Info.Count > 0)
				{
					Serial_Info.Single_Serial_Hierarchy level1 = thisPackage.Behaviors.Serial_Info[0];
					level1_index = level1.Order;
					level1_text = level1.Display;
				}

				if (thisPackage.Behaviors.Serial_Info.Count > 1)
				{
					Serial_Info.Single_Serial_Hierarchy level2 = thisPackage.Behaviors.Serial_Info[1];
					level2_index = level2.Order;
					level2_text = level2.Display;
				}

				if (thisPackage.Behaviors.Serial_Info.Count > 2)
				{
					Serial_Info.Single_Serial_Hierarchy level3 = thisPackage.Behaviors.Serial_Info[2];
					level3_index = level3.Order;
					level3_text = level3.Display;
				}

				if (thisPackage.Behaviors.Serial_Info.Count > 3)
				{
					Serial_Info.Single_Serial_Hierarchy level4 = thisPackage.Behaviors.Serial_Info[3];
					level4_index = level4.Order;
					level4_text = level4.Display;
				}

				if (thisPackage.Behaviors.Serial_Info.Count > 4)
				{
					Serial_Info.Single_Serial_Hierarchy level5 = thisPackage.Behaviors.Serial_Info[4];
					level5_index = level5.Order;
					level5_text = level5.Display;
				}

				
				for (int i = 0; i < thisPackage.Behaviors.Serial_Info.Count; i++)
				{
					builder.Append(thisPackage.Behaviors.Serial_Info[i].Display + "|" + thisPackage.Behaviors.Serial_Info[i].Order);
					if ((i + 1) < thisPackage.Behaviors.Serial_Info.Count)
						builder.Append(";");
				}
			}

			// Call the stored procedure
			SobekCM_Database.Save_Serial_Hierarchy(GroupID, ItemID, level1_text, level1_index, level2_text, level2_index, level3_text, level3_index, level4_text, level4_index, level5_text, level5_index, builder.ToString() );
		}

		private static bool Save_Item_Metadata_Information( SobekCM_Item thisPackage )
		{
			// Clear any existing item metadata
            SobekCM_Database.Clear_Item_Metadata(thisPackage.Web.ItemID, false);

			// Build lists of the metadata now
            List<KeyValuePair<string, string>> metadataTerms = new List<KeyValuePair<string, string>>();

            // Add the BibID
            metadataTerms.Add(new KeyValuePair<string, string>("BibID", thisPackage.BibID));

            // Add the main bibliographic terms
            metadataTerms.AddRange(thisPackage.Bib_Info.Metadata_Search_Terms);

            // Step through all the metadata modules and add any additional metadata search terms
            if (thisPackage.Metadata_Modules_Count > 0)
            {
                foreach (iMetadata_Module thisModule in thisPackage.All_Metadata_Modules)
                {
                    List<KeyValuePair<string, string>> moduleMetadata = thisModule.Metadata_Search_Terms;
                    if ( moduleMetadata != null )
                        metadataTerms.AddRange(moduleMetadata);
                }
            }

			// Add serial information
			if (thisPackage.Behaviors.hasSerialInformation)
			{
				if (thisPackage.Behaviors.Serial_Info.Count > 0)
				{
                    metadataTerms.Add(new KeyValuePair<string, string>("Other Citation", thisPackage.Behaviors.Serial_Info[0].Display));
				}

				if (thisPackage.Behaviors.Serial_Info.Count > 1)
				{
                    metadataTerms.Add(new KeyValuePair<string, string>("Other Citation", thisPackage.Behaviors.Serial_Info[1].Display));
				}

				if (thisPackage.Behaviors.Serial_Info.Count > 2)
				{
                    metadataTerms.Add(new KeyValuePair<string, string>("Other Citation", thisPackage.Behaviors.Serial_Info[2].Display));
				}

				if (thisPackage.Behaviors.Serial_Info.Count > 3)
				{
                    metadataTerms.Add(new KeyValuePair<string, string>("Other Citation", thisPackage.Behaviors.Serial_Info[3].Display));
				}

				if (thisPackage.Behaviors.Serial_Info.Count > 4)
				{
                    metadataTerms.Add(new KeyValuePair<string, string>("Other Citation", thisPackage.Behaviors.Serial_Info[4].Display));
				}
			}

			// Add in the ticklers
			if (thisPackage.Behaviors.Ticklers_Count > 0)
			{
				foreach (string thisTickler in thisPackage.Behaviors.Ticklers)
				{
                    metadataTerms.Add(new KeyValuePair<string, string>("Tickler", thisTickler));
				}
			}

			// Add any download MIME Types
			if ( thisPackage.Divisions.Download_Tree.Has_Files )
			{
				List<string> mimeTypes = new List<string>();
				List<SobekCM_File_Info> allDownloads = thisPackage.Divisions.Download_Tree.All_Files;
				foreach (SobekCM_File_Info thisDownload in allDownloads)
				{
					string thisMimeType = thisDownload.MIME_Type(thisDownload.File_Extension);
					if (( thisMimeType.Length > 0 ) && ( !mimeTypes.Contains ( thisMimeType )))
					{
						mimeTypes.Add( thisMimeType );
					}
				}
				foreach( string thisMimeType in mimeTypes )
				{
                    metadataTerms.Add(new KeyValuePair<string, string>("MIME Type", thisMimeType));
				}
			}

			// Add all the TOC here
			List<string> tocterms = new List<string>();
			List<SobekCM.Resource_Object.Divisions.abstract_TreeNode> divsAndPages = thisPackage.Divisions.Physical_Tree.Divisions_PreOrder;
			foreach (SobekCM.Resource_Object.Divisions.abstract_TreeNode thisNode in divsAndPages)
			{
				if (thisNode.Page)
				{
					// Cast to a page to continnue
					SobekCM.Resource_Object.Divisions.Page_TreeNode pageNode = (SobekCM.Resource_Object.Divisions.Page_TreeNode)thisNode;

					// If this is a unique page label, add it
					if (pageNode.Label.Length > 0) 
					{
						if ((pageNode.Label.ToUpper().IndexOf("PAGE ") < 0) && ( tocterms.Contains(pageNode.Label )))
							tocterms.Add(pageNode.Label);
					}
				}
				else
				{
					// Add the label or type for this division
					if ((thisNode.Label.Length > 0) && ( tocterms.Contains(thisNode.Label )))
						tocterms.Add(thisNode.Label);
					else if ((thisNode.Type.Length > 0) && ( tocterms.Contains(thisNode.Type )))
						tocterms.Add(thisNode.Type);
				}
			}
			foreach( string thisTocTerm in tocterms )
			{
                metadataTerms.Add(new KeyValuePair<string, string>("TOC", thisTocTerm));
			}

			//WHEN 'ZT Kingdom' then 40
			//WHEN 'ZT Phylum' then 41
			//WHEN 'ZT Class' then 42
			//WHEN 'ZT Order' then 43
			//WHEN 'ZT Family' then 44
			//WHEN 'ZT Genus' then 45
			//WHEN 'ZT Species' then 46
			//WHEN 'ZT Common Name' then 47
			//WHEN 'ZT Scientific Name' then 48
			//WHEN 'ZT All Taxonomy' then 49

          
			// Just add blanks in at the end to get this to an increment of ten
            while ((metadataTerms.Count % 10) != 0)
			{
                metadataTerms.Add(new KeyValuePair<string, string>(String.Empty, String.Empty));
			}

			// Now, save this metadata to the database
			int current_index = 0;
            while ((current_index + 10) <= metadataTerms.Count)
			{
				// Save the next ten values
                SobekCM_Database.Save_Item_Metadata(thisPackage.Web.ItemID, 
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

			// Finally, have the database build the full citation based on each metadata element
            SobekCM_Database.Create_Full_Citation_Value(thisPackage.Web.ItemID);

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
			public Save_Item_Group_Args(int GroupID, string New_BibID, bool Is_New )
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
		protected static Save_Item_Group_Args Save_Item_Group(string BibID, string GroupTitle, string SortTitle, string Type, string File_Root, string Group_Thumbnail, bool Update_Existing, DateTime CreateDate, long OCLC_Number, int ALEPH_Number, bool Large_Format, bool Track_By_Month, bool Never_Overlay_Record, string Primary_Identifier_Type, string Primary_Identifier )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[17];
				param_list[0] = new SqlParameter("@BibID", BibID);
				param_list[1] = new SqlParameter("@GroupTitle", GroupTitle);
				param_list[2] = new SqlParameter("@SortTitle", SortTitle);
				param_list[3] = new SqlParameter("@Type", Type);
				param_list[4] = new SqlParameter("@File_Location", File_Root);
				if ( OCLC_Number < 0 )
					param_list[5] = new SqlParameter("@OCLC_Number", 1);
				else
					param_list[5] = new SqlParameter("@OCLC_Number", OCLC_Number);
				if ( ALEPH_Number < 0 )
					param_list[6] = new SqlParameter("@ALEPH_Number", 1);
				else
					param_list[6] = new SqlParameter("@ALEPH_Number", ALEPH_Number);
				param_list[7] = new SqlParameter("@Group_Thumbnail", Group_Thumbnail );
				param_list[8] = new SqlParameter("@Large_Format", Large_Format);
				param_list[9] = new SqlParameter("@Track_By_Month", Track_By_Month);
				param_list[10] = new SqlParameter("@Never_Overlay_Record", Never_Overlay_Record);
				param_list[11] = new SqlParameter("@Update_Existing", Update_Existing);
				param_list[12] = new SqlParameter("@PrimaryIdentifierType", Primary_Identifier_Type);
				param_list[13] = new SqlParameter("@PrimaryIdentifier", Primary_Identifier);
				param_list[14] = new SqlParameter("@GroupID", -1);
				param_list[14].Direction = ParameterDirection.InputOutput;
				param_list[15] = new SqlParameter("@New_BibID", "0000000000" );
				param_list[15].Direction = ParameterDirection.InputOutput;
				param_list[16] = new SqlParameter("@New_Group", false);
				param_list[16].Direction = ParameterDirection.InputOutput;

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Save_Item_Group", param_list);

				// Get the values to return
				int groupid = (int)param_list[14].Value;
				string bibid = param_list[15].Value.ToString();
				bool is_new = Convert.ToBoolean(param_list[16].Value);

				// Return the value
				return new Save_Item_Group_Args(groupid, bibid, is_new );
			}
			catch (Exception ee)
			{
				// Pass this exception onto the method to handle it
				exception_caught("SobekCM_Save_Item_Group", ee);
				return new Save_Item_Group_Args(-1, String.Empty, false);
			}
		}

		/// <summary> Saves the OAI-PMH data associated with an item group </summary>
		/// <param name="GroupID"> Primary key for this item group within the database </param>
		/// <param name="Data_Code"> Code for this metadata type being saved for this item group ( i.e. 'oai_dc' )</param>
		/// <param name="OAI_Data"> Data to be stored for this item </param>
		/// <param name="Always_Overlay"> Flag indicates if this should overlay the OAI, even if it already exists </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks>This method calls the stored procedure 'SobekCM_Add_OAI_PMH_Data'. <br /><br />
		/// If the OAI-PMH data is locked via the database flag on this data, this does not change the OAI stored for the item group.</remarks>
		public static bool Save_Item_Group_OAI(int GroupID, string OAI_Data, string Data_Code, bool Always_Overlay)
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[4];
				param_list[0] = new SqlParameter("@groupid", GroupID);
				param_list[1] = new SqlParameter("@data_code", Data_Code);
				param_list[2] = new SqlParameter("@oai_data", OAI_Data);
				param_list[3] = new SqlParameter("@overlay_existing", Always_Overlay);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Add_OAI_PMH_Data", param_list);

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
		/// <param name="thisPackage"> Digital resource object from which to save the web skins </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This method calls the stored procedure 'SobekCM_Save_Item_Group_Web_Skins'. </remarks>
		/// <exception cref="ApplicationException"> Exception is thrown if an error is caught during 
		/// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
		public static bool Save_Item_Group_Web_Skins(int GroupID, SobekCM_Item thisPackage )
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
			if (thisPackage.Behaviors.Web_Skins.Count > 0)
			{
				primaryInterface = thisPackage.Behaviors.Web_Skins[0];
				if (thisPackage.Behaviors.Web_Skins.Count > 1)
					altInterface1 = thisPackage.Behaviors.Web_Skins[1];
				if (thisPackage.Behaviors.Web_Skins.Count > 2)
					altInterface2 = thisPackage.Behaviors.Web_Skins[2];
				if (thisPackage.Behaviors.Web_Skins.Count > 3)
					altInterface3 = thisPackage.Behaviors.Web_Skins[3];
				if (thisPackage.Behaviors.Web_Skins.Count > 4)
					altInterface4 = thisPackage.Behaviors.Web_Skins[4];
				if (thisPackage.Behaviors.Web_Skins.Count > 5)
					altInterface5 = thisPackage.Behaviors.Web_Skins[5];
				if (thisPackage.Behaviors.Web_Skins.Count > 6)
					altInterface6 = thisPackage.Behaviors.Web_Skins[6];
				if (thisPackage.Behaviors.Web_Skins.Count > 7)
					altInterface7 = thisPackage.Behaviors.Web_Skins[7];
				if (thisPackage.Behaviors.Web_Skins.Count > 8)
					altInterface8 = thisPackage.Behaviors.Web_Skins[8];
				if (thisPackage.Behaviors.Web_Skins.Count > 9)
					altInterface9 = thisPackage.Behaviors.Web_Skins[9];


			}
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[11];
				param_list[0] = new SqlParameter("@GroupID", GroupID);
				param_list[1] = new SqlParameter("@Primary_WebSkin", primaryInterface);
				param_list[2] = new SqlParameter("@Alt_WebSkin1", altInterface1);
				param_list[3] = new SqlParameter("@Alt_WebSkin2", altInterface2);
				param_list[4] = new SqlParameter("@Alt_WebSkin3", altInterface3);
				param_list[5] = new SqlParameter("@Alt_WebSkin4", altInterface4);
				param_list[6] = new SqlParameter("@Alt_WebSkin5", altInterface5);
				param_list[7] = new SqlParameter("@Alt_WebSkin6", altInterface6);
				param_list[8] = new SqlParameter("@Alt_WebSkin7", altInterface7);
				param_list[9] = new SqlParameter("@Alt_WebSkin8", altInterface8);
				param_list[10] = new SqlParameter("@Alt_WebSkin9", altInterface9);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Save_Item_Group_Web_Skins", param_list);

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
			public Save_Item_Args( int ItemID, bool Existing, string New_VID )
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
		/// <param name="DiskSize_MB"> Total size (in MB) of this entire package on the digital library </param>
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
			double DiskSize_MB, string Donor, string Publisher, string Spatial_Display, string Institution_Display, 
			string Edition, string Material_Display, string Measurement_Display, string StylePeriod_Display, string Technique_Display,
			string Subjects_Display )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[30];
				param_list[0] = new SqlParameter("@GroupID", GroupID);
				param_list[1] = new SqlParameter("@VID", VID);
				param_list[2] = new SqlParameter("@PageCount", PageCount);
				param_list[3] = new SqlParameter("@FileCount", FileCount);
				param_list[4] = new SqlParameter("@Title", Title);
				param_list[5] = new SqlParameter("@SortTitle", SortTitle);
				param_list[6] = new SqlParameter("@AccessMethod", 1);
				param_list[7] = new SqlParameter("@Link", Link);
				param_list[8] = new SqlParameter("@CreateDate", CreateDate);
				param_list[9] = new SqlParameter("@PubDate", PubDate);
				param_list[10] = new SqlParameter("@SortDate", SortDate);
				param_list[11] = new SqlParameter("@HoldingCode", Holding_Code);
				param_list[12] = new SqlParameter("@SourceCode", Source_Code);
				param_list[13] = new SqlParameter("@Author", Author);
				param_list[14] = new SqlParameter("@Spatial_KML", Spatial_KML);
				param_list[15] = new SqlParameter("@Spatial_KML_Distance", Spatial_KML_Distance);
				param_list[16] = new SqlParameter("@DiskSize_MB", DiskSize_MB);
				param_list[17] = new SqlParameter("@Spatial_Display", Spatial_Display);
				param_list[18] = new SqlParameter("@Institution_Display", Institution_Display);
				param_list[19] = new SqlParameter("@Edition_Display", Edition );
				param_list[20] = new SqlParameter("@Material_Display", Material_Display);
				param_list[21] = new SqlParameter("@Measurement_Display", Measurement_Display);
				param_list[22] = new SqlParameter("@StylePeriod_Display", StylePeriod_Display);
				param_list[23] = new SqlParameter("@Technique_Display", Technique_Display );
				param_list[24] = new SqlParameter("@Subjects_Display", Subjects_Display );
				param_list[25] = new SqlParameter("@Donor", Donor);
				param_list[26] = new SqlParameter("@Publisher", Publisher);
				param_list[27] = new SqlParameter("@ItemID", -1);
				param_list[27].Direction = ParameterDirection.InputOutput;
				param_list[28] = new SqlParameter("@Existing", false);
				param_list[28].Direction = ParameterDirection.InputOutput;
				param_list[29] = new SqlParameter("@New_VID", "00000");
				param_list[29].Direction = ParameterDirection.InputOutput;

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Save_Item", param_list);

				// Return the value
				int ItemID = (int)param_list[27].Value;
				bool existing = (bool)param_list[28].Value;
				string new_vid = param_list[29].Value.ToString();
				return new Save_Item_Args(ItemID, existing, new_vid);
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
		protected static bool Clear_Item_Metadata(int ItemID, bool Clear_Non_Metadata_Values )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[2];
				param_list[0] = new SqlParameter("@itemid", ItemID);
				param_list[1] = new SqlParameter("@clear_non_mets_values", Clear_Non_Metadata_Values);
 
				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Metadata_Clear2", param_list);
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
			string Metadata_Type10, string Metadata_Value10 )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[21];
				param_list[0] = new SqlParameter("@itemid", ItemID);
				param_list[1] = new SqlParameter("@metadata_type1", Metadata_Type1);
				param_list[2] = new SqlParameter("@metadata_value1", Metadata_Value1.Trim());
				param_list[3] = new SqlParameter("@metadata_type2", Metadata_Type2);
                param_list[4] = new SqlParameter("@metadata_value2", Metadata_Value2.Trim());
				param_list[5] = new SqlParameter("@metadata_type3", Metadata_Type3);
                param_list[6] = new SqlParameter("@metadata_value3", Metadata_Value3.Trim());
				param_list[7] = new SqlParameter("@metadata_type4", Metadata_Type4);
                param_list[8] = new SqlParameter("@metadata_value4", Metadata_Value4.Trim());
				param_list[9] = new SqlParameter("@metadata_type5", Metadata_Type5);
                param_list[10] = new SqlParameter("@metadata_value5", Metadata_Value5.Trim());
				param_list[11] = new SqlParameter("@metadata_type6", Metadata_Type6);
                param_list[12] = new SqlParameter("@metadata_value6", Metadata_Value6.Trim());
				param_list[13] = new SqlParameter("@metadata_type7", Metadata_Type7);
                param_list[14] = new SqlParameter("@metadata_value7", Metadata_Value7.Trim());
				param_list[15] = new SqlParameter("@metadata_type8", Metadata_Type8);
                param_list[16] = new SqlParameter("@metadata_value8", Metadata_Value8.Trim());
				param_list[17] = new SqlParameter("@metadata_type9", Metadata_Type9);
                param_list[18] = new SqlParameter("@metadata_value9", Metadata_Value9.Trim());
				param_list[19] = new SqlParameter("@metadata_type10", Metadata_Type10);
                param_list[20] = new SqlParameter("@metadata_value10", Metadata_Value10.Trim());

				// Execute this non-query stored procedure
                SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Metadata_Save", param_list);
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
				SqlParameter[] param_list = new SqlParameter[1];
				param_list[0] = new SqlParameter("@ItemID", ItemID);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Create_Full_Citation_Value", param_list);

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
		protected static bool Clear_Old_Item_Info( int ItemID )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[1];
				param_list[0] = new SqlParameter( "@ItemID", ItemID );

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Clear_Old_Item_Info", param_list);

				return true;
			}
			catch (Exception ee)
			{
				// Pass this exception onto the method to handle it
				exception_caught("SobekCM_Clear_Old_Item_Info", ee);
				return false;
			}
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
		/// <param name="Left_To_Right"> Flag indicates this item is read from Left-to-Right, rather than standard Right-to-Left</param>
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
			int Viewer1_Type, string Viewer1_Label, string Viewer1_Attributes, int Viewer2_Type, string Viewer2_Label, string Viewer2_Attributes,
			int Viewer3_Type, string Viewer3_Label, string Viewer3_Attributes, int Viewer4_Type, string Viewer4_Label, string Viewer4_Attributes, 
			int Viewer5_Type, string Viewer5_Label, string Viewer5_Attributes, int Viewer6_Type, string Viewer6_Label, string Viewer6_Attributes, bool Left_To_Right )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[47];
				param_list[0] = new SqlParameter( "@ItemID", ItemID );
				param_list[1] = new SqlParameter( "@TextSearchable", TextSearchable );
				param_list[2] = new SqlParameter( "@MainThumbnail", MainThumbnail );
				param_list[3] = new SqlParameter( "@MainJPEG", MainJPEG );
				param_list[4] = new SqlParameter( "@IP_Restriction_Mask", IP_Restriction_Mask);
				param_list[5] = new SqlParameter( "@CheckoutRequired", CheckoutRequired );
				param_list[6] = new SqlParameter( "@Dark_Flag", Dark_Flag );
				param_list[7] = new SqlParameter( "@Born_Digital", Born_Digital );
				if ( DispositionAdvice <= 0 )
					param_list[8] = new SqlParameter("@Disposition_Advice", DBNull.Value);
				else
					param_list[8] = new SqlParameter("@Disposition_Advice", DispositionAdvice);
				param_list[9] = new SqlParameter("@Disposition_Advice_Notes", DispositionAdviceNotes);

				if ( Material_Received_Date.HasValue )
					param_list[10] = new SqlParameter("@Material_Received_Date", Material_Received_Date.Value);
				else
					param_list[10] = new SqlParameter("@Material_Received_Date", DBNull.Value);
				param_list[11] = new SqlParameter( "@Material_Recd_Date_Estimated", Material_Recd_Date_Estimated);
				param_list[12] = new SqlParameter("@Tracking_Box", Tracking_Box);
				param_list[13] = new SqlParameter( "@AggregationCode1", AggregationCode1 );
				param_list[14] = new SqlParameter( "@AggregationCode2", AggregationCode2);
				param_list[15] = new SqlParameter( "@AggregationCode3", AggregationCode3);
				param_list[16] = new SqlParameter( "@AggregationCode4", AggregationCode4);
				param_list[17] = new SqlParameter( "@AggregationCode5", AggregationCode5);
				param_list[18] = new SqlParameter( "@AggregationCode6", AggregationCode6);
				param_list[19] = new SqlParameter( "@AggregationCode7", AggregationCode7);
				param_list[20] = new SqlParameter( "@AggregationCode8", AggregationCode8);
				param_list[21] = new SqlParameter( "@HoldingCode", HoldingCode );
				param_list[22] = new SqlParameter( "@SourceCode", SourceCode );
				param_list[23] = new SqlParameter( "@Icon1_Name", Icon1_Name );
				param_list[24] = new SqlParameter( "@Icon2_Name", Icon2_Name );
				param_list[25] = new SqlParameter( "@Icon3_Name", Icon3_Name );
				param_list[26] = new SqlParameter( "@Icon4_Name", Icon4_Name );
				param_list[27] = new SqlParameter( "@Icon5_Name", Icon5_Name );
				param_list[28] = new SqlParameter( "@Viewer1_TypeID", Viewer1_Type);
				param_list[29] = new SqlParameter( "@Viewer1_Label", Viewer1_Label);
				param_list[30] = new SqlParameter( "@Viewer1_Attribute", Viewer1_Attributes);
				param_list[31] = new SqlParameter( "@Viewer2_TypeID", Viewer2_Type);
				param_list[32] = new SqlParameter( "@Viewer2_Label", Viewer2_Label);
				param_list[33] = new SqlParameter( "@Viewer2_Attribute", Viewer2_Attributes);
				param_list[34] = new SqlParameter( "@Viewer3_TypeID", Viewer3_Type);
				param_list[35] = new SqlParameter( "@Viewer3_Label", Viewer3_Label);
				param_list[36] = new SqlParameter( "@Viewer3_Attribute", Viewer3_Attributes);
				param_list[37] = new SqlParameter( "@Viewer4_TypeID", Viewer4_Type);
				param_list[38] = new SqlParameter( "@Viewer4_Label", Viewer4_Label);
				param_list[39] = new SqlParameter( "@Viewer4_Attribute", Viewer4_Attributes);
				param_list[40] = new SqlParameter( "@Viewer5_TypeID", Viewer5_Type);
				param_list[41] = new SqlParameter( "@Viewer5_Label", Viewer5_Label);
				param_list[42] = new SqlParameter( "@Viewer5_Attribute", Viewer5_Attributes);
				param_list[43] = new SqlParameter( "@Viewer6_TypeID", Viewer6_Type);
				param_list[44] = new SqlParameter( "@Viewer6_Label", Viewer6_Label);
				param_list[45] = new SqlParameter( "@Viewer6_Attribute", Viewer6_Attributes);
				param_list[46] = new SqlParameter( "@Left_To_Right", Left_To_Right );

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Save_Item_Behaviors", param_list);

				return true;
			}
			catch (Exception ee)
			{
				// Pass this exception onto the method to handle it
				exception_caught("SobekCM_Save_Item_Behaviors", ee);
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
				SqlParameter[] param_list = new SqlParameter[11];
				param_list[0] = new SqlParameter("@BibID", BibID);
				param_list[1] = new SqlParameter("@VID", VID);
				param_list[2] = new SqlParameter("@Viewer1_TypeID", Viewer1_Type);
				param_list[3] = new SqlParameter("@Viewer1_Label", Viewer1_Label);
				param_list[4] = new SqlParameter("@Viewer1_Attribute", Viewer1_Attributes);
				param_list[5] = new SqlParameter("@Viewer2_TypeID", Viewer2_Type);
				param_list[6] = new SqlParameter("@Viewer2_Label", Viewer2_Label);
				param_list[7] = new SqlParameter("@Viewer2_Attribute", Viewer2_Attributes);
				param_list[8] = new SqlParameter("@Viewer3_TypeID", Viewer3_Type);
				param_list[9] = new SqlParameter("@Viewer3_Label", Viewer3_Label);
				param_list[10] = new SqlParameter("@Viewer3_Attribute", Viewer3_Attributes);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Save_Item_Views", param_list);

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
			int Viewer1_Type, string Viewer1_Label, string Viewer1_Attributes, int Viewer2_Type, string Viewer2_Label, string Viewer2_Attributes,
			int Viewer3_Type, string Viewer3_Label, string Viewer3_Attributes, int Viewer4_Type, string Viewer4_Label, string Viewer4_Attributes,
			int Viewer5_Type, string Viewer5_Label, string Viewer5_Attributes, int Viewer6_Type, string Viewer6_Label, string Viewer6_Attributes)
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[38];
				param_list[0] = new SqlParameter("@GroupID", GroupID);

				if ( Set_IP_Restriction_Mask )
					param_list[1] = new SqlParameter("@IP_Restriction_Mask", IP_Restriction_Mask);
				else
					param_list[1] = new SqlParameter("@IP_Restriction_Mask", DBNull.Value);

				if (Set_CheckoutRequired)
					param_list[2] = new SqlParameter("@CheckoutRequired", CheckoutRequired);
				else
					param_list[2] = new SqlParameter("@CheckoutRequired", DBNull.Value);

				if (Set_Dark_Flag)
					param_list[3] = new SqlParameter("@Dark_Flag", Dark_Flag);
				else
					param_list[3] = new SqlParameter("@Dark_Flag", DBNull.Value);

				if ( Set_Born_Digital )
					param_list[4] = new SqlParameter("@Born_Digital", Born_Digital);
				else
					param_list[4] = new SqlParameter("@Born_Digital", DBNull.Value);

				param_list[5] = new SqlParameter("@AggregationCode1", AggregationCode1);
				param_list[6] = new SqlParameter("@AggregationCode2", AggregationCode2);
				param_list[7] = new SqlParameter("@AggregationCode3", AggregationCode3);
				param_list[8] = new SqlParameter("@AggregationCode4", AggregationCode4);
				param_list[9] = new SqlParameter("@AggregationCode5", AggregationCode5);
				param_list[10] = new SqlParameter("@AggregationCode6", AggregationCode6);
				param_list[11] = new SqlParameter("@AggregationCode7", AggregationCode7);
				param_list[12] = new SqlParameter("@AggregationCode8", AggregationCode8);
				param_list[13] = new SqlParameter("@HoldingCode", HoldingCode);
				param_list[14] = new SqlParameter("@SourceCode", SourceCode);
				param_list[15] = new SqlParameter("@Icon1_Name", Icon1_Name);
				param_list[16] = new SqlParameter("@Icon2_Name", Icon2_Name);
				param_list[17] = new SqlParameter("@Icon3_Name", Icon3_Name);
				param_list[18] = new SqlParameter("@Icon4_Name", Icon4_Name);
				param_list[19] = new SqlParameter("@Icon5_Name", Icon5_Name);
				param_list[20] = new SqlParameter("@Viewer1_TypeID", Viewer1_Type);
				param_list[21] = new SqlParameter("@Viewer1_Label", Viewer1_Label);
				param_list[22] = new SqlParameter("@Viewer1_Attribute", Viewer1_Attributes);
				param_list[23] = new SqlParameter("@Viewer2_TypeID", Viewer2_Type);
				param_list[24] = new SqlParameter("@Viewer2_Label", Viewer2_Label);
				param_list[25] = new SqlParameter("@Viewer2_Attribute", Viewer2_Attributes);
				param_list[26] = new SqlParameter("@Viewer3_TypeID", Viewer3_Type);
				param_list[27] = new SqlParameter("@Viewer3_Label", Viewer3_Label);
				param_list[28] = new SqlParameter("@Viewer3_Attribute", Viewer3_Attributes);
				param_list[29] = new SqlParameter("@Viewer4_TypeID", Viewer4_Type);
				param_list[30] = new SqlParameter("@Viewer4_Label", Viewer4_Label);
				param_list[31] = new SqlParameter("@Viewer4_Attribute", Viewer4_Attributes);
				param_list[32] = new SqlParameter("@Viewer5_TypeID", Viewer5_Type);
				param_list[33] = new SqlParameter("@Viewer5_Label", Viewer5_Label);
				param_list[34] = new SqlParameter("@Viewer5_Attribute", Viewer5_Attributes);
				param_list[35] = new SqlParameter("@Viewer6_TypeID", Viewer6_Type);
				param_list[36] = new SqlParameter("@Viewer6_Label", Viewer6_Label);
				param_list[37] = new SqlParameter("@Viewer6_Attribute", Viewer6_Attributes);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Mass_Update_Item_Behaviors", param_list);

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
		protected static void Save_Serial_Hierarchy( int GroupID, int ItemID, string Level1_Text, int Level1_Index, string Level2_Text, int Level2_Index, string Level3_Text, int Level3_Index, string Level4_Text, int Level4_Index, string Level5_Text, int Level5_Index, string SerialHierarchy )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[13];
				param_list[0] = new SqlParameter( "@GroupID", GroupID );
				param_list[1] = new SqlParameter( "@ItemID", ItemID );
				if ( Level1_Index >= 0 )
				{
					param_list[2] = new SqlParameter( "@Level1_Text", Level1_Text );
					param_list[3] = new SqlParameter( "@Level1_Index", Level1_Index );
				}
				else
				{
					param_list[2] = new SqlParameter( "@Level1_Text", DBNull.Value );
					param_list[3] = new SqlParameter( "@Level1_Index", DBNull.Value );
				}

				if ( Level2_Index >= 0 )
				{
					param_list[4] = new SqlParameter( "@Level2_Text", Level2_Text );
					param_list[5] = new SqlParameter( "@Level2_Index", Level2_Index );
				}
				else
				{
					param_list[4] = new SqlParameter( "@Level2_Text", DBNull.Value );
					param_list[5] = new SqlParameter( "@Level2_Index", DBNull.Value );
				}

				if ( Level3_Index >= 0 )
				{
					param_list[6] = new SqlParameter( "@Level3_Text", Level3_Text );
					param_list[7] = new SqlParameter( "@Level3_Index", Level3_Index );
				}
				else
				{
					param_list[6] = new SqlParameter( "@Level3_Text", DBNull.Value );
					param_list[7] = new SqlParameter( "@Level3_Index", DBNull.Value );
				}

				if ( Level4_Index >= 0 )
				{
					param_list[8] = new SqlParameter( "@Level4_Text", Level4_Text );
					param_list[9] = new SqlParameter( "@Level4_Index", Level4_Index );
				}
				else
				{
					param_list[8] = new SqlParameter( "@Level4_Text", DBNull.Value );
					param_list[9] = new SqlParameter( "@Level4_Index", DBNull.Value );
				}

				if ( Level5_Index >= 0 )
				{
					param_list[10] = new SqlParameter( "@Level5_Text", Level5_Text );
					param_list[11] = new SqlParameter( "@Level5_Index", Level5_Index );
				}
				else
				{
					param_list[10] = new SqlParameter( "@Level5_Text", DBNull.Value );
					param_list[11] = new SqlParameter( "@Level5_Index", DBNull.Value );
				}

				param_list[12] = new SqlParameter("@SerialHierarchy", SerialHierarchy);


				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Save_Serial_Hierarchy", param_list);
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

		/// <summary> Saves the IP Restriction mask which determines public, private, or restricted by IP </summary>
		/// <param name="ItemID">Item ID for the item to save the new visibility for </param>
		/// <param name="New_Restriction_Mask"> New value for the visibility of this item </param>
		/// <param name="UserName"> Username that performed the change </param>
		/// <param name="ProgressNote"> Note to attach to this change in item's accessibility</param>
		/// <remarks> This method calls the stored procedure 'SobekCM_Set_IP_Restriction_Mask'. </remarks>
		public static bool Set_IP_Restriction_Mask(int ItemID, int New_Restriction_Mask, string UserName, string ProgressNote )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[4];
				param_list[0] = new SqlParameter("@itemid", ItemID);
				param_list[1] = new SqlParameter("@newipmask", New_Restriction_Mask);
				param_list[2] = new SqlParameter("@user", UserName);
				param_list[3] = new SqlParameter("@progressnote", ProgressNote);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Set_IP_Restriction_Mask", param_list);

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
				SqlParameter[] param_list = new SqlParameter[2];
				param_list[0] = new SqlParameter( "@itemid", ItemID );
				param_list[1] = new SqlParameter( "@newcomments", Internal_Comments );

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Set_Item_Comments", param_list);

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
		public static bool Update_Item_Group(string BibID, string GroupTitle, string SortTitle, string Group_Thumbnail, string Primary_Identifier_Type, string Primary_Identifier )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[6];
				param_list[0] = new SqlParameter("@bibid", BibID);
				param_list[1] = new SqlParameter("@grouptitle", GroupTitle);
				param_list[2] = new SqlParameter("@sorttitle", SortTitle);
				param_list[3] = new SqlParameter("@groupthumbnail", Group_Thumbnail);
				param_list[4] = new SqlParameter("@PrimaryIdentifierType", Primary_Identifier_Type);
				param_list[5] = new SqlParameter("@PrimaryIdentifier", Primary_Identifier);


				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Update_Item_Group", param_list);

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
		public static bool Save_New_Tracking_Box(int ItemID, string Tracking_Box )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[2];
				param_list[0] = new SqlParameter("@Tracking_Box", Tracking_Box);
				param_list[1] = new SqlParameter("@ItemID", ItemID);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tracking_Update_Tracking_Box", param_list);

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
				SqlParameter[] param_list = new SqlParameter[3];
				param_list[0] = new SqlParameter("@Disposition_Advice", DispositionTypeID);
				param_list[1] = new SqlParameter("@Disposition_Advice_Notes", Notes);
				param_list[2] = new SqlParameter("@ItemID", ItemID);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tracking_Update_Disposition_Advice", param_list);

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
		public static bool Update_Disposition(int ItemID, int DispositionTypeID, string Notes, DateTime DispositionDate, string UserName )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[5];
				param_list[0] = new SqlParameter("@Disposition_Date", DispositionDate);
				param_list[1] = new SqlParameter("@Disposition_Type", DispositionTypeID);
				param_list[2] = new SqlParameter("@Disposition_Notes", Notes);
				param_list[3] = new SqlParameter("@ItemID", ItemID);
				param_list[4] = new SqlParameter("@UserName", UserName);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tracking_Update_Disposition", param_list);

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
				SqlParameter[] param_list = new SqlParameter[6];
				param_list[0] = new SqlParameter("@itemid", ItemID);
				param_list[1] = new SqlParameter("@user", UserName);
				param_list[2] = new SqlParameter("@progressnote", Notes);
				param_list[3] = new SqlParameter("@workflow", Workflow_Type);
				param_list[4] = new SqlParameter("@storagelocation", StorageLocation);
				param_list[5] = new SqlParameter("@date", Date);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tracking_Add_Past_Workflow_By_ItemID", param_list);

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
				SqlParameter[] param_list = new SqlParameter[5];
				param_list[0] = new SqlParameter("@itemid", ItemID);
				param_list[1] = new SqlParameter("@user", UserName);
				param_list[2] = new SqlParameter("@progressnote", Notes);
				param_list[3] = new SqlParameter("@workflow", Workflow_Type);
				param_list[4] = new SqlParameter("@storagelocation", StorageLocation);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tracking_Add_Workflow_By_ItemID", param_list);

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
				SqlParameter[] param_list = new SqlParameter[5];
				param_list[0] = new SqlParameter("@ItemID", ItemID);
				param_list[1] = new SqlParameter("@Last_Milestone", Last_Milestone);
				param_list[2] = new SqlParameter("@Milestone_Date", Milestone_Date);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tracking_Update_Digitization_Milestones", param_list);

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
		public static bool Update_Material_Received(int ItemID, DateTime Date, bool Estimated, string UserName, string Notes )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[5];
				param_list[0] = new SqlParameter("@Material_Received_Date", Date);
				param_list[1] = new SqlParameter("@Material_Recd_Date_Estimated", Estimated);
				param_list[2] = new SqlParameter("@ItemID", ItemID);
				param_list[3] = new SqlParameter("@User", UserName);
				param_list[4] = new SqlParameter("@ProgressNote", Notes);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tracking_Update_Material_Received", param_list);

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
		public static bool Update_Born_Digital_Flag(int ItemID, bool Born_Digital_Flag )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[2];
				param_list[0] = new SqlParameter("@Born_Digital", Born_Digital_Flag);
				param_list[1] = new SqlParameter("@ItemID", ItemID);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tracking_Update_Born_Digital", param_list);

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
		/// <param name="DiskSize_MB"> Size of the entire digital resource on the image server  </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Update_Item_Online_Statistics' stored procedure </remarks> 
		public static bool Update_Item_Online_Statistics(string BibID, string VID, int PageCount, int FileCount, double DiskSize_MB)
		{
			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[5];
				param_list[0] = new SqlParameter("@bibid", BibID);
				param_list[1] = new SqlParameter("@vid", VID);
				param_list[2] = new SqlParameter("@pagecount", PageCount);
				param_list[3] = new SqlParameter("@filecount", FileCount);
				param_list[4] = new SqlParameter("@disksize_mb", DiskSize_MB);

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Update_Item_Online_Statistics", param_list);

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
				SqlParameter[] param_list = new SqlParameter[2];
				param_list[0] = new SqlParameter("@bibid", BibID);
				param_list[1] = new SqlParameter("@vid", VID);

				// Execute this query stored procedure
				DataSet resultSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_ItemID", param_list);

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
				bool isNumeric = true;
				foreach (char thisChar in OCLC)
				{
					if (!Char.IsNumber(thisChar))
					{
						isNumeric = false;
						break;
					}
				}
				if (isNumeric)
				{
					try
					{
						oclc = Convert.ToInt64(OCLC);
					}
					catch
					{

					}
				}
			}
			if (Local_Catalog_ID.Length > 0)
			{
				bool isNumeric = true;
				foreach (char thisChar in Local_Catalog_ID)
				{
					if (!Char.IsNumber(thisChar))
					{
						isNumeric = false;
						break;
					}
				}
				if (isNumeric)
				{
					try
					{
						aleph = Convert.ToInt32(Local_Catalog_ID);
					}
					catch
					{

					}
				}
			}

			try
			{
				// Build the parameter list
				SqlParameter[] param_list = new SqlParameter[4];
				param_list[0] = new SqlParameter("@bibid", BibID);
				param_list[1] = new SqlParameter("@vid", VID);
				param_list[2] = new SqlParameter("@OCLC_Number", oclc);
				param_list[3] = new SqlParameter("@Local_Cat_Number", aleph);

				// Execute this query stored procedure
				DataSet resultSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Check_For_Record_Existence", param_list);

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


        #region Public non-database related methods

        /// <summary> Gets a flag indicating if the provided string appears to be in bib id format </summary>
		/// <param name="test_string"> string to check for bib id format </param>
		/// <returns> TRUE if this string appears to be in bib id format, otherwise FALSE </returns>
		public static bool is_bibid_format( string test_string )
		{
			// Must be 10 characters long to start with
			if ( test_string.Length != Bib_Length)
				return false;

			// Use regular expressions to check format
			Regex myReg = new Regex("[A-Z]{2}[A-Z|0-9]{4}[0-9]{4}");
			return myReg.IsMatch( test_string.ToUpper() );
		}

		/// <summary> Static method to set if a string is a vid VIDS format</summary>
		/// <param name="test_string"> string to check for vid VIDS format</param>
		/// <returns>TRUE if this string appears to be in VID format, otherwise FALSE</returns>
		public static bool is_vids_format( string test_string)
		{
			if (test_string.Length != Vids_Length)
				return false; 
			return Regex.Match(test_string.ToUpper(), @"^[0-9]{5}$").Success;
		}

		#endregion

		#region Custom Public Properties

		/// <summary> Gets the length of the bib id </summary>
		public static int Bib_Length
		{
			get
			{
				// Return the length
				return 10;
			}
		}

		/// <summary> Gest the lenght of the vids </summary>
		public static int Vids_Length
		{
			get 
			{
				return 5;
			}
		}
		
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
		protected static bool THROW_EXCEPTIONS = true;

		/// <summary> Flag indicates whether a message should be displayed when
		/// errors occur. </summary>
		/// <remarks> Set this flag to TRUE to show a message box when errors occur. </remarks>
		protected static bool DISPLAY_ERRORS = false;

		/// <summary> Flag indicates if the text of the internal exception should
		/// be included in any message or exception thrown.  </summary>
		/// <remarks> Set to TRUE to show the text from the inner exception. </remarks>
		protected static bool DISPLAY_INNER_EXCEPTIONS = false;

		/// <summary> Error string displayed in the case of an error </summary>
		private static string ERROR_STRING = "Error while executing stored procedure '{0}'.       ";

		#endregion

		#region Helper methods and classes

		/// <summary> Method is called when an exception is caught while accessing the database. </summary>
		/// <param name="stored_procedure_name"> Name of the stored procedure called </param>
		/// <param name="exception"> Exception caught while accessing the database </param>
		/// <exception cref="SobekCM_Database_Exception"> Exception is thrown if an error is caught during 
		/// the database work and the THROW_EXCEPTIONS internal flag is set to true. </exception>
		private static void exception_caught( string stored_procedure_name, Exception exception )
		{
			// Determine the text to either show or throw
			string exception_text = string.Format( ERROR_STRING, stored_procedure_name );

			// Show the internal error if that flag is set
			if ( DISPLAY_INNER_EXCEPTIONS )
				exception_text = exception_text + "\n\n" + exception.ToString();

			//// If display is set, then display the errors
			//if ( DISPLAY_ERRORS )
			//{
			//    MessageBox.Show( exception_text, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			//}

			// If an exception should be thrown, throw it
			if ( THROW_EXCEPTIONS )
			{
				throw new SobekCM_Database_Exception( exception_text );
			}
		}

		/// <summary> SobekCM_Database_Exception is an exception which can be thrown when there
		/// is an error while accessing the database.  This extends the <see cref="ApplicationException"/>
		/// class.  </summary>
		internal class SobekCM_Database_Exception : ApplicationException
		{
			/// <summary> Constructor for a new SobekCM_Database_Exception object </summary>
			/// <param name="exceptionText"> Text of the exception to be displayed </param>
			public SobekCM_Database_Exception( string exceptionText ) : base ( exceptionText )
			{
				// All work completed in the base class
			}
		}

		#endregion

		#region Methods to support HTTP retrieval of item list and METS from a SobekCM instance

		/// <summary> Returns the list of items currently loaded to a SobekCM instance </summary>
		/// <param name="SobekCM_Base_URL">Base SobekCM URL</param>
		/// <returns>DataSet</returns>
		public static DataSets.SobekCM_All_Items Current_SobekCM_Items(string SobekCM_Base_URL)
		{
			try
			{
				// Create the return value data set
				DataSets.SobekCM_All_Items returnVal = new DataSets.SobekCM_All_Items();

				// Create the stream to get the information from the web
				WebResponse objResponse;
				WebRequest objRequest = System.Net.HttpWebRequest.Create(SobekCM_Base_URL);
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
		/// <param name="package_resource_url">URL for this packages resources</param>
		/// <param name="bibid">Bib ID for this package</param>
		/// <param name="vid">VID for this package</param>
		/// <returns>METS file as a string</returns>
		public static string Download_METS(string package_resource_url, string bibid, string vid)
		{
			try
			{
				string download_url = package_resource_url + "/" + bibid + "_" + vid + ".METS_Header.xml";
				string mets_file = GetHtmlPage(download_url, 15);
				return mets_file;
			}
			catch
			{
				return String.Empty;
			}
		}

		/// <summary> Retrive html as a string from a web page </summary>
		/// <param name="strURL"> URL to use to retrieve the source </param>
		/// <param name="Seconds_to_TimeOut"> Seconds before the HTML request times out </param>
		/// <returns> Web page as string </returns>
		public static String GetHtmlPage(string strURL, int Seconds_to_TimeOut)
		{
			try
			{
				// the html retrieved from the page
				String strResult;
				WebResponse objResponse;
				WebRequest objRequest = System.Net.HttpWebRequest.Create(strURL);
				objRequest.Timeout = Seconds_to_TimeOut * 1000;
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
		public static DataSets.SobekCM_All_Items Simple_Item_List(string Aggregation_Code)
		{
			try
			{
				// Create the connection
				SqlConnection connect = new SqlConnection(connectionString);

				// Create the command 
				SqlCommand executeCommand = new SqlCommand("SobekCM_Simple_Item_List", connect);
				executeCommand.CommandType = CommandType.StoredProcedure;
				executeCommand.Parameters.AddWithValue("@collection_code", Aggregation_Code);

				// Create the adapter
				SqlDataAdapter adapter = new SqlDataAdapter(executeCommand);

				// Add appropriate table mappings
				adapter.TableMappings.Add("Table", "SobekCM_Item");

				// Create the strongly-typed dataset
				Database.DataSets.SobekCM_All_Items itemList = new Database.DataSets.SobekCM_All_Items();

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
				SqlParameter[] param_list = new SqlParameter[2];
				param_list[0] = new SqlParameter("@BibID", BibID);
				param_list[1] = new SqlParameter("@VID", VID);

				// Define a temporary dataset
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "Tivoli_Get_File_By_Bib_VID", param_list);
				if ((tempSet == null) || (tempSet.Tables.Count == 0) || (tempSet.Tables[0].Rows.Count == 0))
					return null;
				else
					return tempSet.Tables[0];
			}
			catch
			{
				// Pass this exception onto the method to handle it
				return null;

			}
		}
 
	}
}
