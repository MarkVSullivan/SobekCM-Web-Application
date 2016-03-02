#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using EngineAgnosticLayerDbAccess;
using SobekCM.Core;
using SobekCM.Core.Aggregations;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Results;
using SobekCM.Core.Search;
using SobekCM.Core.Settings;
using SobekCM.Core.Users;
using SobekCM.Core.WebContent;
using SobekCM.Core.WebContent.Hierarchy;
using SobekCM.Core.WebContent.Single;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Tools;

#endregion

namespace SobekCM.Engine_Library.Database
{
    /// <summary> Database gateway for the engine library, used for all database calls </summary>
	public static class Engine_Database
	{
		private const int MAX_PAGE_LOOKAHEAD = 4;
		private const int MIN_PAGE_LOOKAHEAD = 2;
		private const int LOOKAHEAD_FACTOR = 5;
		private const int ALL_AGGREGATIONS_METADATA_COUNT_TO_USE_CACHED = 1000;
		 
		private static readonly Object itemListPopulationLock = new Object();

		/// <summary> Gets the last exception caught by a database call through this gateway class  </summary>
		public static Exception Last_Exception { get; set; }

		/// <summary> Connection string to the main SobekCM databaase </summary>
		/// <remarks> This database hold all the information about items, item aggregationPermissions, statistics, and tracking information</remarks>
		public static string Connection_String { get; set; }

		/// <summary> Gets the type of database ( i.e. MSSQL v. PostgreSQL ) </summary>
		public static EalDbTypeEnum DatabaseType { get; set; }

		/// <summary> Static constructor for this class </summary>
		static Engine_Database()
		{
			DatabaseType = EalDbTypeEnum.MSSQL;
		}

        ///// <summary> Tests this instance.
        ///// </summary>
        ///// <returns></returns>
        //public static bool TEST()
        //{
        //    // Build the parameter list
        //    List<EalDbParameter> parameters = new List<EalDbParameter>();
        //    parameters.Add(new EalDbParameter("@inputValue", 1000));

        //    // Add parameters for total items and total titles
        //    EalDbParameter totalItemsParameter = new EalDbParameter("@returnValue", 0) {Direction = ParameterDirection.InputOutput};
        //    parameters.Add(totalItemsParameter);

        //    // Do the non-query
        //    EalDbAccess.ExecuteNonQuery(DatabaseType, Connection_String, CommandType.StoredProcedure, "TEST_Return", parameters);

        //    // Check to see what value4s are
        //    return (Int32.Parse(totalItemsParameter.Value.ToString()) == 1000);
        //}

		/// <summary> Test connectivity to the database </summary>
		/// <returns> TRUE if connection can be made, otherwise FALSE </returns>
		public static bool Test_Connection()
		{
			return EalDbAccess.Test(DatabaseType, Connection_String);
		}

		/// <summary> Test connectivity to the database </summary>
		/// <returns> TRUE if connection can be made, otherwise FALSE </returns>
		public static bool Test_Connection(string TestConnectionString)
		{
			return EalDbAccess.Test(DatabaseType, TestConnectionString);
		}

		#region Methods to get the information about an ITEM or ITEM GROUP

		/// <summary> Gets some basic information about an item group before displaying it, such as the descriptive notes from the database, ability to add notes, etc.. </summary>
		/// <param name="BibID"> Bibliographic identifier for the item group to retrieve </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataSet with detailed information about this item group from the database </returns>
		/// <remarks> This calls the 'SobekCM_Get_Item_Details2' stored procedure, passing in NULL for the volume id </remarks> 
		public static DataSet Get_Item_Group_Details(string BibID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_Item_Group_Details", "");
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[2];
				parameters[0] = new EalDbParameter("@BibID", BibID);
				parameters[1] = new EalDbParameter("@VID", DBNull.Value);

				// Define a temporary dataset
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Details2", parameters);

				// Return the first table from the returned dataset
				return tempSet;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Get_Item_Group_Details", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Item_Group_Details", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Item_Group_Details", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Gets some basic information about an item before displaying it, such as the descriptive notes from the database, ability to add notes, etc.. </summary>
		/// <param name="BibID"> Bibliographic identifier for the volume to retrieve </param>
		/// <param name="Vid"> Volume identifier for the volume to retrieve </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataSet with detailed information about this item from the database </returns>
		/// <remarks> This calls the 'SobekCM_Get_Item_Details2' stored procedure </remarks> 
		public static DataSet Get_Item_Details(string BibID, string Vid, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_Item_Details", "");
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[2];
				parameters[0] = new EalDbParameter("@BibID", BibID);
				parameters[1] = new EalDbParameter("@VID", Vid);


				// Define a temporary dataset
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Details2", parameters);

				// Return the first table from the returned dataset
				return tempSet;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Get_Item_Details", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Item_Details", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Item_Details", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}


		#endregion

		#region Methods to support the restriction by IP addresses

		/// <summary> Gets the list of all the IP ranges for restriction, including each single IP information in those ranges </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataTable with all the data about the IP ranges used for restrictions </returns>
		/// <remarks> This calls the 'SobekCM_Get_All_IP_Restrictions' stored procedure </remarks> 
		public static DataTable Get_IP_Restriction_Ranges(Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_IP_Restriction_Ranges", "Pulls all the IP restriction range information");
			}

			try
			{

				// Create the dataset to fill (could also do a data reader, but we'll do a datatable)
				DataSet fillSet = EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_All_IP_Restrictions");

				// Was there a match?
				if (fillSet.Tables.Count == 0)
					return null;

				// Return the fill set
				return fillSet.Tables[0];
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Get_IP_Restriction_Ranges", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_IP_Restriction_Ranges", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_IP_Restriction_Ranges", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		#endregion

		#region Methods to support pulling data needed for the application cache

		/// <summary> Gets the list of all search stop words which are ignored during searching ( such as 'The', 'A', etc.. ) </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> List of all the search stop words from the database </returns>
		/// <remarks> This calls the 'SobekCM_Get_Search_Stop_Words' stored procedure </remarks>
		public static List<string> Search_Stop_Words(Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.StopWords", "Pull search stop words from the database");
			}

			// Build return list
			List<string> returnValue = new List<string>();

			try
			{
				// Create the database agnostic reader
				EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Search_Stop_Words");

				while (readerWrapper.Reader.Read())
				{
					// Grab the values out
					returnValue.Add(readerWrapper.Reader.GetString(1));
				}

				// Close the reader (which also closes the connection)
				readerWrapper.Close();

				// Return the first table from the returned dataset
				return returnValue;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.StopWords", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.StopWords", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.StopWords", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Populates the collection of the thematic headings for the main home page </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <param name="ThematicHeadingList"> List to populate with the thematic headings from the database</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Manager_Get_Thematic_Headings' stored procedure </remarks> 
		public static bool Populate_Thematic_Headings(List<Thematic_Heading> ThematicHeadingList, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Populate_Thematic_Headings", "Pull thematic heading information from the database");
			}

			try
			{

				// Define a temporary dataset
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Manager_Get_Thematic_Headings");

				// If there was no data for this collection and entry point, return null (an ERROR occurred)
				if (tempSet.Tables.Count > 0)
				{
					// Clear the current list
					ThematicHeadingList.Clear();

					// Add them back
					ThematicHeadingList.AddRange(from DataRow thisRow in tempSet.Tables[0].Rows select new Thematic_Heading(Convert.ToInt16(thisRow["ThematicHeadingID"]), thisRow["ThemeName"].ToString()));
				}

				// Return the built collection as readonly
				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Populate_Thematic_Headings", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Populate_Thematic_Headings", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Populate_Thematic_Headings", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Populates the lookup tables for aliases which point to live aggregationPermissions </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <param name="AggregationAliasList"> List of aggregation aliases to populate from the database</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Get_Item_Aggregation_Aliases' stored procedure </remarks> 
		public static bool Populate_Aggregation_Aliases(Dictionary<string, string> AggregationAliasList, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Populate_Aggregation_Aliases", "Pull item aggregation aliases from the database");
			}

			try
			{
				// Define a temporary dataset
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Aggregation_Aliases");

				// If there was no data for this collection and entry point, return null (an ERROR occurred)
				if ((tempSet.Tables.Count > 0) || (tempSet.Tables[0].Rows.Count > 0))
				{
					// Clear the old list
					AggregationAliasList.Clear();

					foreach (DataRow thisRow in tempSet.Tables[0].Rows)
					{
						AggregationAliasList[thisRow["AggregationAlias"].ToString()] = thisRow["Code"].ToString().ToLower();
					}
				}

				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Populate_Aggregation_Aliases", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Populate_Aggregation_Aliases", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Populate_Aggregation_Aliases", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Gets the list of all user groups </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> List of partly built <see cref="SobekCM.Core.Users.User_Group"/> object </returns>
		/// <remarks> This calls the 'mySobek_Get_All_User_Groups' stored procedure </remarks> 
		public static List<User_Group> Get_All_User_Groups(Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_All_User_Groups", String.Empty);
			}

			try
			{
				DataSet resultSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "mySobek_Get_All_User_Groups");

				List<User_Group> returnValue = new List<User_Group>();

				foreach (DataRow thisRow in resultSet.Tables[0].Rows)
				{
					string name = thisRow["GroupName"].ToString();
					string description = thisRow["GroupDescription"].ToString();
					int usergroupid = Convert.ToInt32(thisRow["UserGroupID"]);
					bool specialGroup = Convert.ToBoolean(thisRow["IsSpecialGroup"]);

					User_Group userGroup = new User_Group(name, description, usergroupid) {IsSpecialGroup = specialGroup};

					returnValue.Add(userGroup);

				}


				return returnValue;

			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Get_All_User_Groups", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_All_User_Groups", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_All_User_Groups", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}


		/// <summary> Datatable with the information for every html skin from the database </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> Datatable with all the html skin information to be loaded into the Web_Skin_Collection object. </returns>
		/// <remarks> This calls the 'SobekCM_Get_Web_Skins' stored procedure </remarks> 
		public static DataTable Get_All_Web_Skins(Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_All_Skins", "Pull display skin information from the database");
			}

			// Define a temporary dataset
			DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Web_Skins");

			// If there was no data for this collection and entry point, return null (an ERROR occurred)
			if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
			{
				return null;
			}

			// Return the built search fields object
			return tempSet.Tables[0];
		}


		/// <summary> Get the list of viewer priority set at the instance level </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering  </param>
		/// <returns> List of all the viewers </returns>
		/// <remarks> This calls the 'SobekCM_Get_Viewer_Priority' stored procedure </remarks> 
		public static List<string> Get_Viewer_Priority(Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_Viewer_Priority", "Pulling from database");
			}

			try
			{
				List<string> returnValue = new List<string>();

				// Define a temporary dataset
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Viewer_Priority");

				// If there was no data for this collection and entry point, return null (an ERROR occurred)
				if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
				{
					return returnValue;
				}

				// Return the first table from the returned dataset
				foreach (DataRow thisRow in tempSet.Tables[0].Rows)
				{
					returnValue.Add(thisRow["ViewType"].ToString());
				}
				return returnValue;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Get_Viewer_Priority", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Viewer_Priority", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Viewer_Priority", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Populates the code manager object for translating SobekCM codes to greenstone collection codes </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <param name="Codes"> Code object to populate with the all the code and aggregation information</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Get_Item_Aggregation_AllCodes' stored procedure </remarks> 
		public static bool Populate_Code_Manager(Aggregation_Code_Manager Codes, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Populate_Code_Manager", String.Empty);
			}

			// Create the database agnostic reader
			EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Aggregation_AllCodes");

			// Pull out the database reader
			DbDataReader reader = readerWrapper.Reader;

			// Clear the codes list and then move in the new data
			Codes.Clear();

			// get the column indexes out
			const int CODE_COL = 0;
			const int TYPE_COL = 1;
			const int NAME_COL = 2;
			const int SHORT_NAME_COL = 3;
			const int IS_ACTIVE_COL = 4;
			const int HIDDEN_COL = 5;
			const int ID_COL = 6;
			const int DESC_COL = 7;
			const int THEME_COL = 8;
			const int LINK_COL = 9;
			const int THEME_NAME_COL = 12;
			const int PARENT_SHORT_NAME = 13;
			const int PARENT_NAME = 14;
			const int PARENT_CODE = 15;

			Item_Aggregation_Related_Aggregations lastAggr = null;

			while (reader.Read())
			{
				// Get the list key values out 
				string code = reader.GetString(CODE_COL).ToUpper();
				string type = reader.GetString(TYPE_COL);

				if ((lastAggr != null) && (lastAggr.Code == code))
				{
					if (!reader.IsDBNull(PARENT_CODE))
					{
						string second_parent_code = reader.GetString(PARENT_CODE).ToUpper();
						string second_parent_name = reader.GetString(PARENT_NAME).ToUpper();
						string second_parent_short = reader.GetString(PARENT_SHORT_NAME);
						lastAggr.Add_Parent_Aggregation(second_parent_code, second_parent_name, second_parent_short);
					}
				}
				else
				{
					// Only do anything else if this is not somehow a repeat
					if (!Codes.isValidCode(code))
					{
						// Create the object
						lastAggr =
							new Item_Aggregation_Related_Aggregations(code, reader.GetString(NAME_COL),
								reader.GetString(SHORT_NAME_COL), type,
								reader.GetBoolean(IS_ACTIVE_COL),
								reader.GetBoolean(HIDDEN_COL),
								reader.GetString(DESC_COL),
								(ushort) reader.GetInt32(ID_COL));

						if (!reader.IsDBNull(LINK_COL))
							lastAggr.External_Link = reader.GetString(LINK_COL);

						if (!reader.IsDBNull(THEME_NAME_COL))
						{
							string theme_name = reader.GetString(THEME_NAME_COL);
							int theme = reader.GetInt32(THEME_COL);
							if (theme > 0)
							{
								lastAggr.Thematic_Heading = new Thematic_Heading(theme, theme_name);
							}
						}

						if (!reader.IsDBNull(PARENT_CODE))
						{
							string parent_code = reader.GetString(PARENT_CODE).ToUpper();
							string parent_name = reader.GetString(PARENT_NAME).ToUpper();
							string parent_short = reader.GetString(PARENT_SHORT_NAME);
							lastAggr.Add_Parent_Aggregation(parent_code, parent_name, parent_short);
						}

						// Add this to the codes manager
						Codes.Add_Collection(lastAggr);
					}
				}
			}

			// Close the reader (which also closes the connection)
			readerWrapper.Close();

			// Succesful
			return true;
		}

		/// <summary> Populates the dictionary of all icons from the database </summary>
		/// <param name="IconList"> List of icons to be populated with a successful database pulll </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Icon_List' stored procedure <br /><br />
		/// The lookup values in this dictionary are the icon code uppercased.</remarks> 
		public static bool Populate_Icon_List(Dictionary<string, Wordmark_Icon> IconList, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Populate_Icon_List", String.Empty);
			}

			// Create the database agnostic reader
			EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Icon_List");

			// Pull out the database reader
			DbDataReader reader = readerWrapper.Reader;

			// Clear existing icons
			IconList.Clear();

			while (reader.Read())
			{
				string code = reader.GetString(0).ToUpper();
				IconList[code] = new Wordmark_Icon(code, reader.GetString(1), reader.GetString(2), reader.GetString(3));
			}

			// Close the reader (which also closes the connection)
			readerWrapper.Close();

			// Succesful
			return true;
		}

		/// <summary> Populates the dictionary of all files and MIME types from the database </summary>
		/// <param name="MimeList"> List of files and MIME types to be populated with a successful database pulll </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Get_Mime_Types' stored procedure <br /><br />
		/// The lookup values in this dictionary are the file extensions in lower case.</remarks> 
		public static bool Populate_MIME_List(Dictionary<string, Mime_Type_Info> MimeList, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Populate_MIME_List", String.Empty);
			}

			// Create the database agnostic reader
			EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Mime_Types");

			// Pull out the database reader
			DbDataReader reader = readerWrapper.Reader;

			// Clear existing icons
			MimeList.Clear();

			while (reader.Read())
			{
				string extension = reader.GetString(0).ToLower();
				MimeList[extension] = new Mime_Type_Info(extension, reader.GetString(1), reader.GetBoolean(2), reader.GetBoolean(3));
			}

			// Close the reader (which also closes the connection)
			readerWrapper.Close();

			// Succesful
			return true;
		}

		/// <summary> Populates the date range from the database for which statistical information exists </summary>
		/// <param name="StatsDateObject"> Statistical range object to hold the beginning and ending of the statistical information </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Statistics_By_Date_Range' stored procedure <br /><br />
		/// This is used by the Statistics_HtmlSubwriter class</remarks>
		public static bool Populate_Statistics_Dates(Statistics_Dates StatsDateObject, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Populate_Statistics_Dates", "Pulling statistics date information from database");
			}

			try
			{
				// Execute this query stored procedure
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Statistics_Dates");

				// Reset the values in the object and then set from the database result
				StatsDateObject.Clear();
				StatsDateObject.Set_Statistics_Dates(tempSet.Tables[0]);

				// No error encountered
				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Populate_Statistics_Dates", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Populate_Statistics_Dates", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Populate_Statistics_Dates", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}


		/// <summary> Populates the translation / language support object for translating common UI terms </summary>
		/// <param name="Translations"> Translations object to populate from the database </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Get_Translation' stored procedure </remarks> 
		public static bool Populate_Translations(Language_Support_Info Translations, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Populate_Translations", String.Empty);
			}

			try
			{
				// Create the database agnostic reader
				EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Translation");

				// Pull out the database reader
				DbDataReader reader = readerWrapper.Reader;

				// Clear the translation information
				Translations.Clear();

				while (reader.Read())
				{
					Translations.Add_French(reader.GetString(1), reader.GetString(2));
					Translations.Add_Spanish(reader.GetString(1), reader.GetString(3));
				}

				// Close the reader (which also closes the connection)
				readerWrapper.Close();

				// Return the first table from the returned dataset
				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Populate_Translations", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Populate_Translations", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Populate_Translations", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}


		/// <summary> Populates the collection of possible portals from the database </summary>
		/// <param name="Portals"> List of possible URL portals into this library/cms </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successul, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Get_All_Portals' stored procedure </remarks>
		public static bool Populate_URL_Portals(Portal_List Portals, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Populate_URL_Portals", "Pull URL portal information from the database");
			}

			try
			{
				// Build the parameter list
				EalDbParameter[] paramList = new EalDbParameter[1];
				paramList[0] = new EalDbParameter("@activeonly", true);

				// Define a temporary dataset
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_All_Portals", paramList);

				lock (Portals)
				{
					// Clear the current list
					Portals.Clear();

					// If there was no data for this collection and entry point, return null (an ERROR occurred)
					if (tempSet.Tables.Count > 0)
					{
						// Add each provided portal
						foreach (DataRow thisRow in tempSet.Tables[0].Rows)
						{
							// Pull the basic data for this portal
							int portalId = Convert.ToInt16(thisRow[0]);
							string baseUrl = thisRow[1].ToString().Trim();
							bool isDefault = Convert.ToBoolean(thisRow[3]);
							string abbreviation = thisRow[4].ToString().Trim();
							string name = thisRow[5].ToString().Trim();
							string basePurl = thisRow[6].ToString().Trim();

							if (isDefault)
							{
								if ((baseUrl == "*") || (baseUrl == "default"))
									baseUrl = String.Empty;
							}

							// Get matching skins and aggregationPermissions
							DataRow[] aggrs = tempSet.Tables[1].Select("PortalID=" + portalId);
							DataRow[] skins = tempSet.Tables[2].Select("PortalID=" + portalId);

							// Find the default aggregation
							string defaultAggr = String.Empty;
							if (aggrs.Length > 0)
								defaultAggr = aggrs[0][1].ToString().ToLower();

							// Find the default skin
							string defaultSkin = String.Empty;
							if (skins.Length > 0)
								defaultSkin = skins[0][1].ToString().ToLower();

							// Add this portal
							Portal newPortal = Portals.Add_Portal(portalId, name, abbreviation, defaultAggr, defaultSkin, baseUrl, basePurl);

							// If this is default, set it
							if (isDefault)
								Portals.Default_Portal = newPortal;
						}
					}
				}

				if (Portals.Count == 0)
				{
					// Add the default url portal then
					Portals.Default_Portal = Portals.Add_Portal(-1, "Default SobekCM Library", "Sobek", "all", "sobek", "", "");
				}

				// Return the built collection as readonly
				return true;
			}
			catch (Exception ee)
			{
				// Add the default url portal then
				Portals.Default_Portal = Portals.Add_Portal(-1, "Default SobekCM Library", "Sobek", "all", "sobek", "", "");
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Populate_URL_Portals", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Populate_URL_Portals", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Populate_URL_Portals", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		#endregion

		#region Methods related to the Thematic Heading values

		/// <summary> Saves a new thematic heading or updates an existing thematic heading </summary>
		/// <param name="ThematicHeadingID"> Primary key for the existing thematic heading, or -1 for a new heading </param>
		/// <param name="ThemeOrder"> Order of this thematic heading, within the rest of the headings </param>
		/// <param name="ThemeName"> Display name for this thematic heading</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Thematic heading id, or -1 if there was an error </returns>
		/// <remarks> This calls the 'SobekCM_Edit_Thematic_Heading' stored procedure </remarks> 
		public static int Edit_Thematic_Heading(int ThematicHeadingID, int ThemeOrder, string ThemeName, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Edit_Thematic_Heading", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				EalDbParameter[] paramList = new EalDbParameter[4];
				paramList[0] = new EalDbParameter("@thematicheadingid", ThematicHeadingID);
				paramList[1] = new EalDbParameter("@themeorder", ThemeOrder);
				paramList[2] = new EalDbParameter("@themename", ThemeName);
				paramList[3] = new EalDbParameter("@newid", -1) { Direction = ParameterDirection.Output };

				EalDbAccess.ExecuteNonQuery( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Edit_Thematic_Heading", paramList);

				return Convert.ToInt32(paramList[3].Value);
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				return -1;
			}
		}

		/// <summary> Deletes a thematic heading from the database  </summary>
		/// <param name="ThematicHeadingID"> Primary key for the thematic heading to delete </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE</returns>
		/// <remarks> This calls the 'SobekCM_Delete_Thematic_Heading' stored procedure </remarks> 
		public static bool Delete_Thematic_Heading(int ThematicHeadingID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Delete_Thematic_Heading", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				EalDbParameter[] paramList = new EalDbParameter[1];
				paramList[0] = new EalDbParameter("@thematicheadingid", ThematicHeadingID);

				EalDbAccess.ExecuteNonQuery( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Delete_Thematic_Heading", paramList);
				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				return false;
			}
		}

		#endregion


		/// <summary> Gets the dataset with all default metadata and all templates </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataSet with list of all default metadata sets and tables </returns>
		/// <remarks> This calls the 'mySobek_Get_All_Template_DefaultMetadatas' stored procedure</remarks> 
		public static DataSet Get_All_Template_DefaultMetadatas(Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_All_Projects_DefaultMetadatas", String.Empty);
			}

			// Define a temporary dataset
			DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "mySobek_Get_All_Template_DefaultMetadatas");
			return tempSet;
		}

		/// <summary> Gets complete information for an item which may be missing from the complete list of items </summary>
		/// <param name="BibID"> Bibliographic identifiers for the item of interest </param>
		/// <param name="Vid"> Volume identifiers for the item of interest </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Datarow with additional information about an item, including spatial details, publisher, donor, etc.. </returns>
		/// <remarks> This calls the 'SobekCM_Get_Item_Brief_Info' stored procedure </remarks> 
		public static DataRow Get_Item_Information(string BibID, string Vid, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_Item_Information", "Trying to pull information for " + BibID + "_" + Vid);
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[3];
				parameters[0] = new EalDbParameter("@bibid", BibID);
				parameters[1] = new EalDbParameter("@vid", Vid);
				parameters[2] = new EalDbParameter("@include_aggregations", false);

				// Define a temporary dataset
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Brief_Info", parameters);

				// If there was no data for this collection and entry point, return null (an ERROR occurred)
				if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
				{
					return null;
				}

				// Return the first table from the returned dataset
				return tempSet.Tables[0].Rows[0];
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Get_Item_Information", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Item_Information", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Item_Information", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Gets complete information for an item which may be missing from the complete list of items </summary>
		/// <param name="BibID"> Bibliographic identifiers for the item of interest </param>
		/// <param name="Vid"> Volume identifiers for the item of interest </param>
		/// <param name="IncludeAggregations"> Flag indicates whether to include the aggregationPermissions </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Datarow with additional information about an item, including spatial details, publisher, donor, etc.. </returns>
		/// <remarks> This calls the 'SobekCM_Get_Item_Brief_Info' stored procedure </remarks> 
		public static DataSet Get_Item_Information(string BibID, string Vid, bool IncludeAggregations, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_Item_Information", "Trying to pull information for " + BibID + "_" + Vid);
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[3];
				parameters[0] = new EalDbParameter("@bibid", BibID);
				parameters[1] = new EalDbParameter("@vid", Vid);
				parameters[2] = new EalDbParameter("@include_aggregations", IncludeAggregations);

				// Define a temporary dataset
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Brief_Info", parameters);

				// If there was no data for this collection and entry point, return null (an ERROR occurred)
				if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
				{
					return null;
				}

				// Return the first table from the returned dataset
				return tempSet;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Get_Item_Information", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Item_Information", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Item_Information", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Verified the item lookup object is filled, or populates the item lookup object with all the valid bibids and vids in the system </summary>
		/// <param name="AlwaysRefresh"> Flag indicates if this should be refreshed, whether or not the item lookup object is filled </param>
		/// <param name="IncludePrivate"> Flag indicates whether to include private items in this list </param>
		/// <param name="ItemLookupObject"> Item lookup object to directly populate from the database </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful or if the object is already filled, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Item_List_Web' stored procedure </remarks> 
		internal static bool Verify_Item_Lookup_Object(bool AlwaysRefresh, bool IncludePrivate, Item_Lookup_Object ItemLookupObject, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Verify_Item_Lookup_Object", String.Empty);
			}

			// If no database string, don't try to connect
			if (String.IsNullOrEmpty(Connection_String))
				return false;

			lock (itemListPopulationLock)
			{
				bool updateList = true;
				if (( !AlwaysRefresh ) && ( ItemLookupObject != null))
				{
					TimeSpan sinceLastUpdate = DateTime.Now.Subtract(ItemLookupObject.Last_Updated);
					if (sinceLastUpdate.TotalMinutes <= 1)
						updateList = false;
				}

				if (!updateList)
				{
					return true;
				}

				// Have the database popoulate the little bit of bibid/vid information we retain
				bool returnValue = Populate_Item_Lookup_Object(IncludePrivate, ItemLookupObject, Tracer);
				if ((returnValue) && ( ItemLookupObject != null ))
					ItemLookupObject.Last_Updated = DateTime.Now;
				return returnValue;
			}
		}


		/// <summary> Populates the item lookup object with all the valid bibids and vids in the system </summary>
		/// <param name="IncludePrivate"> Flag indicates whether to include private items in this list </param>
		/// <param name="ItemLookupObject"> Item lookup object to directly populate from the database </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Item_List_Web' stored procedure </remarks> 
		public static bool Populate_Item_Lookup_Object(bool IncludePrivate, Item_Lookup_Object ItemLookupObject, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Populate_Item_Lookup_Object", String.Empty);
			}

			try
			{
				// Create the parameter list
				EalDbParameter[] parameters = new EalDbParameter[1];
				parameters[0] = new EalDbParameter("@include_private", IncludePrivate);

				// Get the data reader (wrapper)
				EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String + "Connection Timeout=45", CommandType.StoredProcedure, "SobekCM_Item_List", parameters);

				// Pull out the database reader
				DbDataReader reader = readerWrapper.Reader;

				// Clear existing volumes
				ItemLookupObject.Clear();
				ItemLookupObject.Last_Updated = DateTime.Now;

				string currentBibid = String.Empty;
				Multiple_Volume_Item currentVolume = null;
				while (reader.Read())
				{
					// Grab the values out
					string newBib = reader.GetString(0);
					string newVid = reader.GetString(1);
					short newMask = reader.GetInt16(2);
					string title = reader.GetString(3);

					// Create a new multiple volume object?
					if (newBib != currentBibid)
					{
						currentBibid = newBib;
						currentVolume = new Multiple_Volume_Item(newBib);
						ItemLookupObject.Add_Title(currentVolume);
					}

					// Add this volume
					Single_Item newItem = new Single_Item(newVid, newMask, title);
					if (currentVolume != null) currentVolume.Add_Item(newItem);
				}

				// Close the reader (which also closes the connection)
				readerWrapper.Close();

				// Return the first table from the returned dataset
				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Populate_Item_Lookup_Object", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Populate_Item_Lookup_Object", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Populate_Item_Lookup_Object", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		#region Methods to perform database searches


		/// <summary> Perform a metadata search against items in the database and return one page of results </summary>
		/// <param name="Link1"> Link for the first term, can only be used to NOT the first term ( 2=NOT )</param>
		/// <param name="Term1"> First search term for this metadata search </param>
		/// <param name="Field1"> Field number to search for (or -1 to search all fields)</param>
		/// <param name="Link2"> Link between the first and second terms ( 0=AND, 1=OR, 2=AND NOT )</param>
		/// <param name="Term2"> Second search term for this metadata search </param>
		/// <param name="Field2"> Field number to search for (or -1 to search all fields)</param>
		/// <param name="Link3">Link between the second and third search terms ( 0=AND, 1=OR, 2=AND NOT )</param>
		/// <param name="Term3"> Third search term for this metadata search </param>
		/// <param name="Field3"> Field number to search for (or -1 to search all fields)</param>
		/// <param name="Link4">Link between the third and fourth search terms ( 0=AND, 1=OR, 2=AND NOT )</param>
		/// <param name="Term4"> Fourth search term for this metadata search </param>
		/// <param name="Field4"> Field number to search for (or -1 to search all fields)</param>
		/// <param name="Link5">Link between the fourth and fifth search terms ( 0=AND, 1=OR, 2=AND NOT )</param>
		/// <param name="Term5"> Fifth search term for this metadata search </param>
		/// <param name="Field5"> Field number to search for (or -1 to search all fields)</param>
		/// <param name="Link6">Link between the fifth and sixth search terms ( 0=AND, 1=OR, 2=AND NOT )</param>
		/// <param name="Term6"> Sixth search term for this metadata search </param>
		/// <param name="Field6"> Field number to search for (or -1 to search all fields)</param>
		/// <param name="Link7">Link between the sixth and seventh search terms ( 0=AND, 1=OR, 2=AND NOT )</param>
		/// <param name="Term7"> Seventh search term for this metadata search </param>
		/// <param name="Field7"> Field number to search for (or -1 to search all fields)</param>
		/// <param name="Link8">Link between the seventh and eighth search terms ( 0=AND, 1=OR, 2=AND NOT )</param>
		/// <param name="Term8"> Eighth search term for this metadata search </param>
		/// <param name="Field8"> Field number to search for (or -1 to search all fields)</param>
		/// <param name="Link9">Link between the eighth and ninth search terms ( 0=AND, 1=OR, 2=AND NOT )</param>
		/// <param name="Term9"> Ninth search term for this metadata search </param>
		/// <param name="Field9"> FIeld number to search for (or -1 to search all fields)</param>
		/// <param name="Link10">Link between the ninth and tenth search terms ( 0=AND, 1=OR, 2=AND NOT )</param>
		/// <param name="Term10"> Tenth search term for this metadata search </param>
		/// <param name="Field10"> Field number to search for (or -1 to search all fields)</param>
		/// <param name="IncludePrivateItems"> Flag indicates whether to include private items in the result set </param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="AggregationCode"> Code for the aggregation of interest ( or empty string to search all aggregationPermissions )</param>
		/// <param name="DateRangeStart"> If this search includes a date range search, start of the date range, or -1</param>
		/// <param name="DateRangeEnd"> If this search includes a date range search, end of the date range, or -1</param>
		/// <param name="IncludeFacets"> Flag indicates whether to include facets </param>
		/// <param name="FacetTypes"> Primary key for the metadata types to include as facets (up to eight)</param>
		/// <param name="ReturnSearchStatistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Small arguments object which contains the page of results and optionally statistics about results for the entire search, including complete counts and facet information </returns>
		/// <remarks> This calls the 'SobekCM_Metadata_Search_Paged' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Perform_Metadata_Search_Paged(int Link1, string Term1, int Field1,
			int Link2, string Term2, int Field2, int Link3, string Term3, int Field3, int Link4, string Term4, int Field4,
			int Link5, string Term5, int Field5, int Link6, string Term6, int Field6, int Link7, string Term7, int Field7,
			int Link8, string Term8, int Field8, int Link9, string Term9, int Field9, int Link10, string Term10, int Field10,
			bool IncludePrivateItems, string AggregationCode, long DateRangeStart, long DateRangeEnd,
			int ResultsPerPage, int ResultsPage, int Sort, bool IncludeFacets,
			List<short> FacetTypes, bool ReturnSearchStatistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
                Tracer.Add_Trace("Engine_Database.Perform_Metadata_Search_Paged", "Performing search in database ( stored procedure SobekCM_Metadata_Search_Paged )");
			}

			// Build the parameter list
			List<EalDbParameter> parameters = new List<EalDbParameter>
			{
			    new EalDbParameter("@link1", Link1), 
                new EalDbParameter("@term1", Term1), 
                new EalDbParameter("@field1", Field1), 
                new EalDbParameter("@link2", Link2), 
                new EalDbParameter("@term2", Term2), 
                new EalDbParameter("@field2", Field2), 
                new EalDbParameter("@link3", Link3), 
                new EalDbParameter("@term3", Term3), 
                new EalDbParameter("@field3", Field3), 
                new EalDbParameter("@link4", Link4), 
                new EalDbParameter("@term4", Term4), 
                new EalDbParameter("@field4", Field4), 
                new EalDbParameter("@link5", Link5), 
                new EalDbParameter("@term5", Term5), 
                new EalDbParameter("@field5", Field5), 
                new EalDbParameter("@link6", Link6), 
                new EalDbParameter("@term6", Term6), 
                new EalDbParameter("@field6", Field6), 
                new EalDbParameter("@link7", Link7), 
                new EalDbParameter("@term7", Term7), 
                new EalDbParameter("@field7", Field7), 
                new EalDbParameter("@link8", Link8), 
                new EalDbParameter("@term8", Term8), 
                new EalDbParameter("@field8", Field8), 
                new EalDbParameter("@link9", Link9), 
                new EalDbParameter("@term9", Term9), 
                new EalDbParameter("@field9", Field9), 
                new EalDbParameter("@link10", Link10), 
                new EalDbParameter("@term10", Term10), 
                new EalDbParameter("@field10", Field10), 
                new EalDbParameter("@include_private", IncludePrivateItems)
			};
		    if (AggregationCode.ToUpper() == "ALL")
				AggregationCode = String.Empty;
			parameters.Add(new EalDbParameter("@aggregationcode", AggregationCode));
			parameters.Add(new EalDbParameter("@daterange_start", DateRangeStart));
			parameters.Add(new EalDbParameter("@daterange_end", DateRangeEnd));
			parameters.Add(new EalDbParameter("@pagesize", ResultsPerPage));
			parameters.Add(new EalDbParameter("@pagenumber", ResultsPage));
			parameters.Add(new EalDbParameter("@sort", Sort));

			// If this is for more than 100 results, don't look ahead
			if (ResultsPerPage > 100)
			{
				parameters.Add(new EalDbParameter("@minpagelookahead", 1));
				parameters.Add(new EalDbParameter("@maxpagelookahead", 1));
				parameters.Add(new EalDbParameter("@lookahead_factor", LOOKAHEAD_FACTOR));
			}
			else
			{
				parameters.Add(new EalDbParameter("@minpagelookahead", MIN_PAGE_LOOKAHEAD));
				parameters.Add(new EalDbParameter("@maxpagelookahead", MAX_PAGE_LOOKAHEAD));
				parameters.Add(new EalDbParameter("@lookahead_factor", LOOKAHEAD_FACTOR));
			}

			if ((IncludeFacets) && (FacetTypes != null) && (FacetTypes.Count > 0) && (ReturnSearchStatistics))
			{
				parameters.Add(new EalDbParameter("@include_facets", true));
				parameters.Add(FacetTypes.Count > 0 ? new EalDbParameter("@facettype1", FacetTypes[0]) : new EalDbParameter("@facettype1", -1));
				parameters.Add(FacetTypes.Count > 1 ? new EalDbParameter("@facettype2", FacetTypes[1]) : new EalDbParameter("@facettype2", -1));
				parameters.Add(FacetTypes.Count > 2 ? new EalDbParameter("@facettype3", FacetTypes[2]) : new EalDbParameter("@facettype3", -1));
				parameters.Add(FacetTypes.Count > 3 ? new EalDbParameter("@facettype4", FacetTypes[3]) : new EalDbParameter("@facettype4", -1));
				parameters.Add(FacetTypes.Count > 4 ? new EalDbParameter("@facettype5", FacetTypes[4]) : new EalDbParameter("@facettype5", -1));
				parameters.Add(FacetTypes.Count > 5 ? new EalDbParameter("@facettype6", FacetTypes[5]) : new EalDbParameter("@facettype6", -1));
				parameters.Add(FacetTypes.Count > 6 ? new EalDbParameter("@facettype7", FacetTypes[6]) : new EalDbParameter("@facettype7", -1));
				parameters.Add(FacetTypes.Count > 7 ? new EalDbParameter("@facettype8", FacetTypes[7]) : new EalDbParameter("@facettype8", -1));
			}
			else
			{
				parameters.Add(new EalDbParameter("@include_facets", false));
				parameters.Add(new EalDbParameter("@facettype1", -1));
				parameters.Add(new EalDbParameter("@facettype2", -1));
				parameters.Add(new EalDbParameter("@facettype3", -1));
				parameters.Add(new EalDbParameter("@facettype4", -1));
				parameters.Add(new EalDbParameter("@facettype5", -1));
				parameters.Add(new EalDbParameter("@facettype6", -1));
				parameters.Add(new EalDbParameter("@facettype7", -1));
				parameters.Add(new EalDbParameter("@facettype8", -1));
			}

			// Add parameters for total items and total titles
			EalDbParameter totalItemsParameter = new EalDbParameter("@total_items", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalItemsParameter);

			EalDbParameter totalTitlesParameter = new EalDbParameter("@total_titles", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalTitlesParameter);

			// Add parameters for items and titles if this search is expanded to include all aggregationPermissions
			EalDbParameter expandedItemsParameter = new EalDbParameter("@all_collections_items", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(expandedItemsParameter);

			EalDbParameter expandedTitlesParameter = new EalDbParameter("@all_collections_titles", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(expandedTitlesParameter);

			// Get the data reader (wrapper)
			EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String + "Connection Timeout=45", CommandType.StoredProcedure, "SobekCM_Metadata_Search_Paged", parameters);

			// Create the return argument object
			List<string> metadataLabels = new List<string>();
			Multiple_Paged_Results_Args returnArgs = new Multiple_Paged_Results_Args
			{
				Paged_Results = DataReader_To_Result_List_With_LookAhead2(readerWrapper.Reader, ResultsPerPage, metadataLabels)
			};

			// Create the overall search statistics?
			if (ReturnSearchStatistics)
			{
				Search_Results_Statistics stats = new Search_Results_Statistics(readerWrapper.Reader, FacetTypes, metadataLabels);
				returnArgs.Statistics = stats;
				readerWrapper.Close();
				stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
				stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
                int allItems = Convert.ToInt32(expandedItemsParameter.Value);
			    int allTitles = Convert.ToInt32(expandedTitlesParameter.Value);
                if ( allItems > 0 ) stats.All_Collections_Items = allItems;
                if ( allTitles > 0 ) stats.All_Collections_Titles = allTitles;

                foreach (Search_Facet_Collection thisFacet in stats.Facet_Collections)
                {
                    Metadata_Search_Field field = Engine_ApplicationCache_Gateway.Settings.Metadata_Search_Field_By_ID(thisFacet.MetadataTypeID);
                    thisFacet.MetadataTerm = field.Facet_Term;
                }
			}
			else
			{
				// Close the reader (which also closes the connection)
				readerWrapper.Close();
			}

			// Return the built result arguments
			return returnArgs;
		}

		/// <summary> Performs a basic metadata search over the entire citation, given a search condition, and returns one page of results </summary>
		/// <param name="SearchCondition"> Search condition string to be run against the databasse </param>
		/// <param name="IncludePrivateItems"> Flag indicates whether to include private items in the result set </param>
		/// <param name="AggregationCode"> Code for the aggregation of interest ( or empty string to search all aggregationPermissions )</param>
		/// <param name="DateRangeStart"> If this search includes a date range search, start of the date range, or -1</param>
		/// <param name="DateRangeEnd"> If this search includes a date range search, end of the date range, or -1</param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="IncludeFacets"> Flag indicates whether to include facets in the result set </param>
		/// <param name="FacetTypes"> Primary key for the metadata types to include as facets (up to eight)</param>
		/// <param name="ReturnSearchStatistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Small arguments object which contains the page of results and optionally statistics about results for the entire search, including complete counts and facet information </returns>
		/// <remarks> This calls the 'SobekCM_Metadata_Basic_Search_Paged' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Perform_Metadata_Search_Paged(string SearchCondition, bool IncludePrivateItems, string AggregationCode, long DateRangeStart, long DateRangeEnd, int ResultsPerPage, int ResultsPage, int Sort, bool IncludeFacets, List<short> FacetTypes, bool ReturnSearchStatistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
                Tracer.Add_Trace("Engine_Database.Perform_Basic_Search_Paged", "Performing basic search in database  ( stored procedure SobekCM_Metadata_Basic_Search_Paged2 )");
			}

            if (AggregationCode.ToUpper() == "ALL")
                AggregationCode = String.Empty;

			// Build the list of parameters
			List<EalDbParameter> parameters = new List<EalDbParameter>
			{
			    new EalDbParameter("@searchcondition", SearchCondition.Replace("''", "'")),
			    new EalDbParameter("@include_private", IncludePrivateItems), 
                new EalDbParameter("@aggregationcode", AggregationCode), 
                new EalDbParameter("@daterange_start", DateRangeStart), 
                new EalDbParameter("@daterange_end", DateRangeEnd), 
                new EalDbParameter("@pagesize", ResultsPerPage), 
                new EalDbParameter("@pagenumber", ResultsPage), 
                new EalDbParameter("@sort", Sort)
			};

		    // If this is for more than 100 results, don't look ahead
			if (ResultsPerPage > 100)
			{
				parameters.Add(new EalDbParameter("@minpagelookahead", 1));
				parameters.Add(new EalDbParameter("@maxpagelookahead", 1));
				parameters.Add(new EalDbParameter("@lookahead_factor", LOOKAHEAD_FACTOR));
			}
			else
			{
				parameters.Add(new EalDbParameter("@minpagelookahead", MIN_PAGE_LOOKAHEAD));
				parameters.Add(new EalDbParameter("@maxpagelookahead", MAX_PAGE_LOOKAHEAD));
				parameters.Add(new EalDbParameter("@lookahead_factor", LOOKAHEAD_FACTOR));
			}

			if ((IncludeFacets) && (FacetTypes != null) && (FacetTypes.Count > 0) && (ReturnSearchStatistics))
			{
				parameters.Add(new EalDbParameter("@include_facets", true));
				parameters.Add(FacetTypes.Count > 0 ? new EalDbParameter("@facettype1", FacetTypes[0]) : new EalDbParameter("@facettype1", -1));
				parameters.Add(FacetTypes.Count > 1 ? new EalDbParameter("@facettype2", FacetTypes[1]) : new EalDbParameter("@facettype2", -1));
				parameters.Add(FacetTypes.Count > 2 ? new EalDbParameter("@facettype3", FacetTypes[2]) : new EalDbParameter("@facettype3", -1));
				parameters.Add(FacetTypes.Count > 3 ? new EalDbParameter("@facettype4", FacetTypes[3]) : new EalDbParameter("@facettype4", -1));
				parameters.Add(FacetTypes.Count > 4 ? new EalDbParameter("@facettype5", FacetTypes[4]) : new EalDbParameter("@facettype5", -1));
				parameters.Add(FacetTypes.Count > 5 ? new EalDbParameter("@facettype6", FacetTypes[5]) : new EalDbParameter("@facettype6", -1));
				parameters.Add(FacetTypes.Count > 6 ? new EalDbParameter("@facettype7", FacetTypes[6]) : new EalDbParameter("@facettype7", -1));
				parameters.Add(FacetTypes.Count > 7 ? new EalDbParameter("@facettype8", FacetTypes[7]) : new EalDbParameter("@facettype8", -1));
			}
			else
			{
				parameters.Add(new EalDbParameter("@include_facets", false));
				parameters.Add(new EalDbParameter("@facettype1", -1));
				parameters.Add(new EalDbParameter("@facettype2", -1));
				parameters.Add(new EalDbParameter("@facettype3", -1));
				parameters.Add(new EalDbParameter("@facettype4", -1));
				parameters.Add(new EalDbParameter("@facettype5", -1));
				parameters.Add(new EalDbParameter("@facettype6", -1));
				parameters.Add(new EalDbParameter("@facettype7", -1));
				parameters.Add(new EalDbParameter("@facettype8", -1));
			}

			// Add parameters for total items and total titles
			EalDbParameter totalItemsParameter = new EalDbParameter("@total_items", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalItemsParameter);

			EalDbParameter totalTitlesParameter = new EalDbParameter("@total_titles", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalTitlesParameter);

			// Add parameters for items and titles if this search is expanded to include all aggregationPermissions
			EalDbParameter expandedItemsParameter = new EalDbParameter("@all_collections_items", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(expandedItemsParameter);

			EalDbParameter expandedTitlesParameter = new EalDbParameter("@all_collections_titles", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(expandedTitlesParameter);

			// Create the database agnostic reader
			EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String + "Connection Timeout=45", CommandType.StoredProcedure, "SobekCM_Metadata_Basic_Search_Paged2", parameters);

			// Pull out the database reader
			DbDataReader reader = readerWrapper.Reader;

			// Create the return argument object
			List<string> metadataLabels = new List<string>();
			Multiple_Paged_Results_Args returnArgs = new Multiple_Paged_Results_Args
			{
				Paged_Results = DataReader_To_Result_List_With_LookAhead2(reader, ResultsPerPage, metadataLabels)
			};

			// Create the overall search statistics?
			if (ReturnSearchStatistics)
			{
				Search_Results_Statistics stats = new Search_Results_Statistics(reader, FacetTypes, metadataLabels);
				returnArgs.Statistics = stats;
				readerWrapper.Close();
				stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
				stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
                int allItems = Convert.ToInt32(expandedItemsParameter.Value);
                int allTitles = Convert.ToInt32(expandedTitlesParameter.Value);
                if (allItems > 0) stats.All_Collections_Items = allItems;
                if (allTitles > 0) stats.All_Collections_Titles = allTitles;

                foreach (Search_Facet_Collection thisFacet in stats.Facet_Collections)
                {
                    Metadata_Search_Field field = Engine_ApplicationCache_Gateway.Settings.Metadata_Search_Field_By_ID(thisFacet.MetadataTypeID);
                    thisFacet.MetadataTerm = field.Facet_Term;
                }
            }
			else
			{
				// Close the reader (which also closes the connection)
				readerWrapper.Close();
			}

			// Return the built result arguments
			return returnArgs;
		}

		/// <summary> Performs a metadata search for a piece of metadata that EXACTLY matches the provided search term and return one page of results </summary>
		/// <param name="SearchTerm"> Search condition string to be run against the databasse </param>
		/// <param name="FieldID"> Primary key for the field to search in the database </param>
		/// <param name="IncludePrivateItems"> Flag indicates whether to include private items in the result set </param>
		/// <param name="AggregationCode"> Code for the aggregation of interest ( or empty string to search all aggregationPermissions )</param>
		/// <param name="DateRangeStart"> If this search includes a date range search, start of the date range, or -1</param>
		/// <param name="DateRangeEnd"> If this search includes a date range search, end of the date range, or -1</param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="IncludeFacets"> Flag indicates whether to include facets in the result set </param>
		/// <param name="FacetTypes"> Primary key for the metadata types to include as facets (up to eight)</param>
		/// <param name="ReturnSearchStatistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Small arguments object which contains the page of results and optionally statistics about results for the entire search, including complete counts and facet information </returns>
		/// <remarks> This calls the 'SobekCM_Metadata_Exact_Search_Paged2' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Perform_Metadata_Exact_Search_Paged(string SearchTerm, int FieldID, bool IncludePrivateItems, string AggregationCode, long DateRangeStart, long DateRangeEnd, int ResultsPerPage, int ResultsPage, int Sort, bool IncludeFacets, List<short> FacetTypes, bool ReturnSearchStatistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Perform_Metadata_Exact_Search_Paged", "Performing exact search in database");
			}

            if (AggregationCode.ToUpper() == "ALL")
                AggregationCode = String.Empty;

			// Build the parameters
			List<EalDbParameter> parameters = new List<EalDbParameter>
			{
			    new EalDbParameter("@term1", SearchTerm), 
                new EalDbParameter("@field1", FieldID), 
                new EalDbParameter("@include_private", IncludePrivateItems), 
                new EalDbParameter("@aggregationcode", AggregationCode), 
                new EalDbParameter("@daterange_start", DateRangeStart), 
                new EalDbParameter("@daterange_end", DateRangeEnd), 
                new EalDbParameter("@pagesize", ResultsPerPage), 
                new EalDbParameter("@pagenumber", ResultsPage), 
                new EalDbParameter("@sort", Sort)
			};

		    // If this is for more than 100 results, don't look ahead
			if (ResultsPerPage > 100)
			{
				parameters.Add(new EalDbParameter("@minpagelookahead", 1));
				parameters.Add(new EalDbParameter("@maxpagelookahead", 1));
				parameters.Add(new EalDbParameter("@lookahead_factor", LOOKAHEAD_FACTOR));
			}
			else
			{
				parameters.Add(new EalDbParameter("@minpagelookahead", MIN_PAGE_LOOKAHEAD));
				parameters.Add(new EalDbParameter("@maxpagelookahead", MAX_PAGE_LOOKAHEAD));
				parameters.Add(new EalDbParameter("@lookahead_factor", LOOKAHEAD_FACTOR));
			}

			if ((IncludeFacets) && (FacetTypes != null) && (FacetTypes.Count > 0) && (ReturnSearchStatistics))
			{
				parameters.Add(new EalDbParameter("@include_facets", true));
				parameters.Add(FacetTypes.Count > 0 ? new EalDbParameter("@facettype1", FacetTypes[0]) : new EalDbParameter("@facettype1", -1));
				parameters.Add(FacetTypes.Count > 1 ? new EalDbParameter("@facettype2", FacetTypes[1]) : new EalDbParameter("@facettype2", -1));
				parameters.Add(FacetTypes.Count > 2 ? new EalDbParameter("@facettype3", FacetTypes[2]) : new EalDbParameter("@facettype3", -1));
				parameters.Add(FacetTypes.Count > 3 ? new EalDbParameter("@facettype4", FacetTypes[3]) : new EalDbParameter("@facettype4", -1));
				parameters.Add(FacetTypes.Count > 4 ? new EalDbParameter("@facettype5", FacetTypes[4]) : new EalDbParameter("@facettype5", -1));
				parameters.Add(FacetTypes.Count > 5 ? new EalDbParameter("@facettype6", FacetTypes[5]) : new EalDbParameter("@facettype6", -1));
				parameters.Add(FacetTypes.Count > 6 ? new EalDbParameter("@facettype7", FacetTypes[6]) : new EalDbParameter("@facettype7", -1));
				parameters.Add(FacetTypes.Count > 7 ? new EalDbParameter("@facettype8", FacetTypes[7]) : new EalDbParameter("@facettype8", -1));
			}
			else
			{
				parameters.Add(new EalDbParameter("@include_facets", false));
				parameters.Add(new EalDbParameter("@facettype1", -1));
				parameters.Add(new EalDbParameter("@facettype2", -1));
				parameters.Add(new EalDbParameter("@facettype3", -1));
				parameters.Add(new EalDbParameter("@facettype4", -1));
				parameters.Add(new EalDbParameter("@facettype5", -1));
				parameters.Add(new EalDbParameter("@facettype6", -1));
				parameters.Add(new EalDbParameter("@facettype7", -1));
				parameters.Add(new EalDbParameter("@facettype8", -1));
			}

			// Add parameters for total items and total titles
			EalDbParameter totalItemsParameter = new EalDbParameter("@total_items", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalItemsParameter);

			EalDbParameter totalTitlesParameter = new EalDbParameter("@total_titles", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalTitlesParameter);

			// Add parameters for items and titles if this search is expanded to include all aggregationPermissions
			EalDbParameter expandedItemsParameter = new EalDbParameter("@all_collections_items", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(expandedItemsParameter);

			EalDbParameter expandedTitlesParameter = new EalDbParameter("@all_collections_titles", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(expandedTitlesParameter);

			// Create the database agnostic reader
			EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String + "Connection Timeout=45", CommandType.StoredProcedure, "SobekCM_Metadata_Exact_Search_Paged2", parameters);

			// Pull out the database reader
			DbDataReader reader = readerWrapper.Reader;


			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Perform_Metadata_Exact_Search_Paged", "Building result object from returned value");
			}

			// Create the return argument object
			List<string> metadataLabels = new List<string>();
			Multiple_Paged_Results_Args returnArgs = new Multiple_Paged_Results_Args
			{
				Paged_Results = DataReader_To_Result_List_With_LookAhead2(reader, ResultsPerPage, metadataLabels)
			};

			// Create the overall search statistics?
			if (ReturnSearchStatistics)
			{
				Search_Results_Statistics stats = new Search_Results_Statistics(reader, FacetTypes, metadataLabels);
				returnArgs.Statistics = stats;
				readerWrapper.Close();
				stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
				stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
                int allItems = Convert.ToInt32(expandedItemsParameter.Value);
                int allTitles = Convert.ToInt32(expandedTitlesParameter.Value);
                if (allItems > 0) stats.All_Collections_Items = allItems;
                if (allTitles > 0) stats.All_Collections_Titles = allTitles;

                foreach (Search_Facet_Collection thisFacet in stats.Facet_Collections)
                {
                    Metadata_Search_Field field = Engine_ApplicationCache_Gateway.Settings.Metadata_Search_Field_By_ID(thisFacet.MetadataTypeID);
                    thisFacet.MetadataTerm = field.Facet_Term;
                }
            }
			else
			{
				// Close the reader (which also closes the connection)
				readerWrapper.Close();
			}

			// Return the built result arguments
			return returnArgs;
		}

		private static List<List<iSearch_Title_Result>> DataReader_To_Result_List_With_LookAhead2(DbDataReader Reader, int ResultsPerPage, List<string> MetadataFieldNames )
		{
			// Create return list
			List<List<iSearch_Title_Result>> returnValue = new List<List<iSearch_Title_Result>>();

			// Create some lists used during the construction
			Dictionary<int, Database_Title_Result> titleLookupByRowNumber = new Dictionary<int, Database_Title_Result>();
			Dictionary<int, Database_Item_Result> itemLookupByItemID = new Dictionary<int, Database_Item_Result>();
			Dictionary<int, int> rowNumberLookupByItemID = new Dictionary<int, int>();

			// May have not values returned
			if (Reader.FieldCount < 5)
				return null;

			// Get all the main title values first
			int minimumRownumber = -1;
			while (Reader.Read())
			{
				// Create new database title object for this
				Database_Title_Result result = new Database_Title_Result
					{
						RowNumber = Reader.GetInt32(0),
						BibID = Reader.GetString(1),
						GroupTitle = Reader.GetString(2),
						OPAC_Number = Reader.GetInt32(3),
						OCLC_Number = Reader.GetInt64(4),
						GroupThumbnail = Reader.GetString(5),
						MaterialType = Reader.GetString(6),
						Primary_Identifier_Type = Reader.GetString(7),
						Primary_Identifier = Reader.GetString(8)
					};

				titleLookupByRowNumber.Add(result.RowNumber, result);

				if (minimumRownumber == -1)
				{
					minimumRownumber = result.RowNumber;
				}
			}

			// Move to the item system-required information table
			Reader.NextResult();

			// If there were no titles, then there are no results
			if (titleLookupByRowNumber.Count == 0)
				return returnValue;

			// Step through all the item rows, build the item, and add to the title 
			Database_Title_Result titleResult = titleLookupByRowNumber[minimumRownumber];
			List<iSearch_Title_Result> currentList = new List<iSearch_Title_Result> { titleResult };
			returnValue.Add(currentList);
			int lastRownumber = titleResult.RowNumber;
			int titlesInCurrentList = 1;
			while (Reader.Read())
			{
				// Ensure this is the right title for this item 
				int thisRownumber = Reader.GetInt32(0);
				if (thisRownumber != lastRownumber)
				{
					titleResult = titleLookupByRowNumber[thisRownumber];
					lastRownumber = thisRownumber;

					// If this is now twenty in the current list, add this to the returnvalue
					if (titlesInCurrentList == ResultsPerPage)
					{
						currentList = new List<iSearch_Title_Result>();
						returnValue.Add(currentList);
						titlesInCurrentList = 0;
					}

					// Add this title to the paged list
					currentList.Add(titleResult);
					titlesInCurrentList++;
				}

				int itemID = Reader.GetInt32(1);
				string vid = Reader.GetString(2);
				string title = Reader.GetString(3);
				short ipRestrictionMask = Reader.GetInt16(4);
				string mainThumbnail = Reader.GetString(5);
				short level1Index = (short) Reader.GetInt32(6);
				string level1Text = Reader.GetString(7);
				short level2Index = (short) Reader.GetInt32(8);
				string level2Text = Reader.GetString(9);
				short level3Index = (short) Reader.GetInt32(10);
				string level3Text = Reader.GetString(11);
				string pubDate = Reader.GetString(12);
				int pageCount = Reader.GetInt32(13);
				string link = Reader.GetString(14);
				string spatialKml = Reader.GetString(15);
				string cOinSOpenUrl = Reader.GetString(16);

				titleResult.Spatial_Coordinates = spatialKml;


				// Create new database item object for this
				Database_Item_Result result = new Database_Item_Result
				{
					ItemID = itemID,
					VID = vid,
					Title = title,
					IP_Restriction_Mask = ipRestrictionMask,
					MainThumbnail = mainThumbnail,
					Level1_Index = level1Index,
					Level1_Text = level1Text,
					Level2_Index = level2Index,
					Level2_Text = level2Text,
					Level3_Index = level3Index,
					Level3_Text = level3Text,
					PubDate = pubDate,
					PageCount = pageCount,
					Link = link,
					Spatial_KML = spatialKml,
					COinS_OpenURL = cOinSOpenUrl
				};

				//// Create new database item object for this
				//Database_Item_Result result = new Database_Item_Result
				//{
				//	ItemID = Reader.GetInt32(1),
				//	VID = Reader.GetString(2),
				//	Title = Reader.GetString(3),
				//	IP_Restriction_Mask = Reader.GetInt16(4),
				//	MainThumbnail = Reader.GetString(5),
				//	Level1_Index = (short)Reader.GetInt32(6),
				//	Level1_Text = Reader.GetString(7),
				//	Level2_Index = (short)Reader.GetInt32(8),
				//	Level2_Text = Reader.GetString(9),
				//	Level3_Index = (short)Reader.GetInt32(10),
				//	Level3_Text = Reader.GetString(11),
				//	PubDate = Reader.GetString(12),
				//	PageCount = Reader.GetInt32(13),
				//	Link = Reader.GetString(14),
				//	Spatial_KML = Reader.GetString(15),
				//	COinS_OpenURL = Reader.GetString(16)
				//};

				// Save to the hash lookup for adding display metadata
				itemLookupByItemID[result.ItemID] = result;
				rowNumberLookupByItemID[result.ItemID] = thisRownumber;

				// Add this to the title object
				titleResult.Add_Item_Result(result);
			}

			// Move to the item aggregation-configured display information table
			Reader.NextResult();

			// Set some values for checking for uniformity of values
			const int ITEMS_TO_CHECK_IN_EACH_TITLE = 20;
			bool first_item_analyzed = true;
			List<bool> checking_fields = new List<bool>();
			int display_fields_count = 0;
			int itemcount = 0;
			int lastRowNumber = -1;
			while (Reader.Read())
			{
				// Get the item id and then work back to the local title id
				int itemId = Reader.GetInt32(0);
				int rowNumber = rowNumberLookupByItemID[itemId];

				// If this is the very first item analyzed, need to do some work first
				if (first_item_analyzed)
				{
					// Save the number of display fields
					display_fields_count = Reader.FieldCount - 1;

					// Add a boolean for each display field
					for (int i = 0; i < display_fields_count; i++)
					{
						// Add the default boolean value here
						checking_fields.Add(true);

						// Save the metadata label
						MetadataFieldNames.Add(Reader.GetName(i+1));
					}

					// Done with the first row analysis, so ensure it does not repeat
					first_item_analyzed = false;
				}

				// Is this is the start of a new title row?
				if (lastRowNumber != rowNumber)
				{
					// Get this title object
					titleResult = titleLookupByRowNumber[rowNumber];

					// Set items analyzed for this title to zero
					itemcount = 0;

					// Back to checking each metadata field since this is a new title
					for (int i = 0; i < display_fields_count; i++)
						checking_fields[i] = true;

					// Save this row numbe as the last row number analyzed
					lastRowNumber = rowNumber;
				}

				if (itemcount == 0)
				{
					// Set all the initial display values (at the title level) from
					// this item's display information 
					titleResult.Metadata_Display_Values = new string[display_fields_count];
					for (int i = 0; i < display_fields_count; i++)
					{
						if (Reader.IsDBNull(i + 1))
							titleResult.Metadata_Display_Values[i] = String.Empty;
						else
							titleResult.Metadata_Display_Values[i] = Reader.GetString(i + 1);
					}
				}
				else if (itemcount < ITEMS_TO_CHECK_IN_EACH_TITLE)
				{
					// Compare the values attached with each display piece of metadata
					// from the title with this additional, individual item.  If the 
					// values are the same, it should display at the title level, but 
					// if they are different, we will not display the values at that level
					for (int i = 0; i < display_fields_count; i++)
					{
						// If we already found a mismatch for this metadata field, then
						// no need to continue checking
						if (checking_fields[i])
						{
							string thisField = String.Empty;
							if (!Reader.IsDBNull(i + 1))
								titleResult.Metadata_Display_Values[i] = Reader.GetString(i + 1);

							if (String.Compare(titleResult.Metadata_Display_Values[i], thisField, StringComparison.InvariantCultureIgnoreCase) != 0)
							{
								titleResult.Metadata_Display_Values[i] = "*";
								checking_fields[i] = false;
							}
						}
					}
				}
			}

			return returnValue;
		}

		private static List<iSearch_Title_Result> DataReader_To_Simple_Result_List2(DbDataReader Reader, List<string> MetadataFieldNames)
		{
			// Create return list
			List<iSearch_Title_Result> returnValue = new List<iSearch_Title_Result>();

			// Create some lists used during the construction
			Dictionary<int, Database_Title_Result> titleLookupByRowNumber = new Dictionary<int, Database_Title_Result>();
			Dictionary<int, Database_Item_Result> itemLookupByItemID = new Dictionary<int, Database_Item_Result>();
			Dictionary<int, int> rowNumberLookupByItemID = new Dictionary<int, int>();

			// May have not values returned
			if (Reader.FieldCount < 5)
				return null;

			// Get all the main title values first
			int minimumRownumber = -1;
			while (Reader.Read())
			{
				// Create new database title object for this
				Database_Title_Result result = new Database_Title_Result
				{
					RowNumber = Reader.GetInt32(0),
					BibID = Reader.GetString(1),
					GroupTitle = Reader.GetString(2),
					OPAC_Number = Reader.GetInt32(3),
					OCLC_Number = Reader.GetInt64(4),
					GroupThumbnail = Reader.GetString(5),
					MaterialType = Reader.GetString(6),
					Primary_Identifier_Type = Reader.GetString(7),
					Primary_Identifier = Reader.GetString(8)
				};

				titleLookupByRowNumber.Add(result.RowNumber, result);

				if (minimumRownumber == -1)
				{
					minimumRownumber = result.RowNumber;
				}
			}

			// Move to the item system-required information table
			Reader.NextResult();

			// If there were no titles, then there are no results
			if (titleLookupByRowNumber.Count == 0)
				return returnValue;

			// Step through all the item rows, build the item, and add to the title 
			Database_Title_Result titleResult = titleLookupByRowNumber[minimumRownumber];
			returnValue.Add(titleResult);
			int lastRownumber = titleResult.RowNumber;
			while (Reader.Read())
			{
				// Ensure this is the right title for this item 
				int thisRownumber = Reader.GetInt32(0);
				if (thisRownumber != lastRownumber)
				{
					titleResult = titleLookupByRowNumber[thisRownumber];
					lastRownumber = thisRownumber;

					// Add this title to the list
					returnValue.Add(titleResult);
				}

				// Create new database item object for this
				Database_Item_Result result = new Database_Item_Result
				{
					ItemID = Reader.GetInt32(1),
					VID = Reader.GetString(2),
					Title = Reader.GetString(3),
					IP_Restriction_Mask = Reader.GetInt16(4),
					MainThumbnail = Reader.GetString(5),
					Level1_Index = (short)Reader.GetInt32(6),
					Level1_Text = Reader.GetString(7),
					Level2_Index = (short)Reader.GetInt32(8),
					Level2_Text = Reader.GetString(9),
					Level3_Index = (short)Reader.GetInt32(10),
					Level3_Text = Reader.GetString(11),
					PubDate = Reader.GetString(12),
					PageCount = Reader.GetInt32(13),
					Link = Reader.GetString(14),
					Spatial_KML = Reader.GetString(15),
					COinS_OpenURL = Reader.GetString(16)
				};

				// Save to the hash lookup for adding display metadata
				itemLookupByItemID[result.ItemID] = result;
				rowNumberLookupByItemID[result.ItemID] = thisRownumber;

				// Add this to the title object
				titleResult.Add_Item_Result(result);
			}

			// Move to the item aggregation-configured display information table
			Reader.NextResult();

			// Set some values for checking for uniformity of values
			const int ITEMS_TO_CHECK_IN_EACH_TITLE = 20;
			bool first_item_analyzed = true;
			List<bool> checking_fields = new List<bool>();
			int display_fields_count = 0;
			int itemcount = 0;
			int lastRowNumber = -1;
			while (Reader.Read())
			{
				// Get the item id and then work back to the local title id
				int itemId = Reader.GetInt32(0);
				int rowNumber = rowNumberLookupByItemID[itemId];

				// If this is the very first item analyzed, need to do some work first
				if (first_item_analyzed)
				{
					// Save the number of display fields
					display_fields_count = Reader.FieldCount - 1;

					// Add a boolean for each display field
					for (int i = 0; i < display_fields_count; i++)
					{
						// Add the default boolean value here
						checking_fields.Add(true);

						// Save the metadata label
						MetadataFieldNames.Add(Reader.GetName(i + 1));
					}

					// Done with the first row analysis, so ensure it does not repeat
					first_item_analyzed = false;
				}

				// Is this is the start of a new title row?
				if (lastRowNumber != rowNumber)
				{
					// Get this title object
					titleResult = titleLookupByRowNumber[rowNumber];

					// Set items analyzed for this title to zero
					itemcount = 0;

					// Back to checking each metadata field since this is a new title
					for (int i = 0; i < display_fields_count; i++)
						checking_fields[i] = true;

					// Save this row numbe as the last row number analyzed
					lastRowNumber = rowNumber;
				}

				if (itemcount == 0)
				{
					// Set all the initial display values (at the title level) from
					// this item's display information 
					titleResult.Metadata_Display_Values = new string[display_fields_count];
					for (int i = 0; i < display_fields_count; i++)
					{
						titleResult.Metadata_Display_Values[i] = Reader.GetString(i + 1);
					}
				}
				else if (itemcount < ITEMS_TO_CHECK_IN_EACH_TITLE)
				{
					// Compare the values attached with each display piece of metadata
					// from the title with this additional, individual item.  If the 
					// values are the same, it should display at the title level, but 
					// if they are different, we will not display the values at that level
					for (int i = 0; i < display_fields_count; i++)
					{
						// If we already found a mismatch for this metadata field, then
						// no need to continue checking
						if (checking_fields[i])
						{
							if (String.Compare(titleResult.Metadata_Display_Values[i], Reader.GetString(i + 1), StringComparison.InvariantCultureIgnoreCase) != 0)
							{
								titleResult.Metadata_Display_Values[i] = "*";
								checking_fields[i] = false;
							}
						}
					}
				}
			}

			return returnValue;
		}

		#endregion

		#region Method to perform a coordinate/geographic search of items in the database

		/// <summary> Performs geographic search for items within provided rectangular bounding box and linked to item aggregation of interest </summary>
		/// <param name="AggregationCode"> Code for the item aggregation of interest </param>
		/// <param name="Latitude1"> Latitudinal portion of the first point making up the rectangular bounding box</param>
		/// <param name="Longitude1"> Longitudinal portion of the first point making up the rectangular bounding box</param>
		/// <param name="Latitude2"> Latitudinal portion of the second point making up the rectangular bounding box</param>
		/// <param name="Longitude2"> Longitudinal portion of the second point making up the rectangular bounding box</param>
		/// <param name="IncludePrivateItems"> Flag indicates whether to include private items in the result set </param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="IncludeFacets"> Flag indicates if facets should be included in the result set </param>
		/// <param name="FacetTypes"> Primary key for the metadata types to include as facets (up to eight)</param>
		/// <param name="ReturnSearchStatistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Table with all of the item and item group information within provided bounding box </returns>
		/// <remarks> This calls the 'SobekCM_Get_Items_By_Coordinates' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Get_Items_By_Coordinates(string AggregationCode, double Latitude1, double Longitude1, double Latitude2, double Longitude2, bool IncludePrivateItems, int ResultsPerPage, int ResultsPage, int Sort, bool IncludeFacets, List<short> FacetTypes, bool ReturnSearchStatistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_Items_By_Coordinates", "Pulling data from database");
			}

			// Build the parameters
			List<EalDbParameter> parameters = new List<EalDbParameter>();
			parameters.Add(new EalDbParameter("@lat1", Latitude1));
			parameters.Add(new EalDbParameter("@long1", Longitude1));
		    if ((Latitude1 == Latitude2) && (Longitude1 == Longitude2))
		    {
                parameters.Add(new EalDbParameter("@lat2", DBNull.Value));
                parameters.Add(new EalDbParameter("@long2", DBNull.Value));
		    }
		    else
		    {
                parameters.Add(new EalDbParameter("@lat2", Latitude2));
                parameters.Add(new EalDbParameter("@long2", Longitude2));
		    }

			parameters.Add(new EalDbParameter("@include_private", IncludePrivateItems));
            parameters.Add(new EalDbParameter("@aggregationcode", AggregationCode));
			parameters.Add(new EalDbParameter("@pagesize", ResultsPerPage));
			parameters.Add(new EalDbParameter("@pagenumber", ResultsPage));
			parameters.Add(new EalDbParameter("@sort", Sort));
			parameters.Add(new EalDbParameter("@minpagelookahead", MIN_PAGE_LOOKAHEAD));
			parameters.Add(new EalDbParameter("@maxpagelookahead", MAX_PAGE_LOOKAHEAD));
			parameters.Add(new EalDbParameter("@lookahead_factor", LOOKAHEAD_FACTOR));
			parameters.Add(new EalDbParameter("@include_facets", IncludeFacets));

			if ((IncludeFacets) && (FacetTypes != null) && (ReturnSearchStatistics))
			{
				parameters.Add(FacetTypes.Count > 0 ? new EalDbParameter("@facettype1", FacetTypes[0]) : new EalDbParameter("@facettype1", -1));
				parameters.Add(FacetTypes.Count > 1 ? new EalDbParameter("@facettype2", FacetTypes[1]) : new EalDbParameter("@facettype2", -1));
				parameters.Add(FacetTypes.Count > 2 ? new EalDbParameter("@facettype3", FacetTypes[2]) : new EalDbParameter("@facettype3", -1));
				parameters.Add(FacetTypes.Count > 3 ? new EalDbParameter("@facettype4", FacetTypes[3]) : new EalDbParameter("@facettype4", -1));
				parameters.Add(FacetTypes.Count > 4 ? new EalDbParameter("@facettype5", FacetTypes[4]) : new EalDbParameter("@facettype5", -1));
				parameters.Add(FacetTypes.Count > 5 ? new EalDbParameter("@facettype6", FacetTypes[5]) : new EalDbParameter("@facettype6", -1));
				parameters.Add(FacetTypes.Count > 6 ? new EalDbParameter("@facettype7", FacetTypes[6]) : new EalDbParameter("@facettype7", -1));
				parameters.Add(FacetTypes.Count > 7 ? new EalDbParameter("@facettype8", FacetTypes[7]) : new EalDbParameter("@facettype8", -1));
			}
			else
			{
				parameters.Add(new EalDbParameter("@facettype1", -1));
				parameters.Add(new EalDbParameter("@facettype2", -1));
				parameters.Add(new EalDbParameter("@facettype3", -1));
				parameters.Add(new EalDbParameter("@facettype4", -1));
				parameters.Add(new EalDbParameter("@facettype5", -1));
				parameters.Add(new EalDbParameter("@facettype6", -1));
				parameters.Add(new EalDbParameter("@facettype7", -1));
				parameters.Add(new EalDbParameter("@facettype8", -1));
			}

			// Add parameters for total items and total titles
			EalDbParameter totalItemsParameter = new EalDbParameter("@total_items", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalItemsParameter);

			EalDbParameter totalTitlesParameter = new EalDbParameter("@total_titles", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalTitlesParameter);

			// Create the database agnostic reader
			EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String + "Connection Timeout=45", CommandType.StoredProcedure, "SobekCM_Get_Items_By_Coordinates", parameters);

			// Pull out the database reader
			DbDataReader reader = readerWrapper.Reader;


			List<string> metadataFields = new List<string>();
			// Create the return argument object
			Multiple_Paged_Results_Args returnArgs = new Multiple_Paged_Results_Args {Paged_Results = DataReader_To_Result_List_With_LookAhead2(reader, ResultsPerPage, metadataFields)};

			// Create the overall search statistics?
			if (ReturnSearchStatistics)
			{
				Search_Results_Statistics stats = new Search_Results_Statistics(reader, FacetTypes, metadataFields);
				returnArgs.Statistics = stats;
				readerWrapper.Close();
				stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
				stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
			}
			else
			{
				// Close the reader (which also closes the connection)
				readerWrapper.Close();
			}

			// Return the built result arguments
			return returnArgs;
		}

		#endregion

		#region Methods to retrieve item list by OCLC or ALEPH number

		/// <summary> Returns the list of all items/titles which match a given OCLC number </summary>
		/// <param name="OclcNumber"> OCLC number to look for matching items </param>
		/// <param name="IncludePrivateItems"> Flag indicates whether to include private items in the result set </param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="ReturnSearchStatistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Table with all of the item and item group information which matches the OCLC number </returns>
		/// <remarks> This calls the 'SobekCM_Items_By_OCLC' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Items_By_OCLC_Number(long OclcNumber, bool IncludePrivateItems, int ResultsPerPage, int Sort, bool ReturnSearchStatistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Items_By_OCLC_Number", "Searching by OCLC in the database");
			}

			// Build the parameter list
			EalDbParameter[] paramList = new EalDbParameter[5];
			paramList[0] = new EalDbParameter("@oclc_number", OclcNumber);
			paramList[1] = new EalDbParameter("@include_private", IncludePrivateItems);
			paramList[2] = new EalDbParameter("@sort", Sort);
			paramList[3] = new EalDbParameter("@total_items", 0) {Direction = ParameterDirection.InputOutput};
			paramList[4] = new EalDbParameter("@total_titles", 0) {Direction = ParameterDirection.InputOutput};

			// Create the database agnostic reader
			EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Items_By_OCLC", paramList);

			// Pull out the database reader
			DbDataReader reader = readerWrapper.Reader;

			// Create the return argument object
			List<string> metadataFields = new List<string>();
			Multiple_Paged_Results_Args returnArgs = new Multiple_Paged_Results_Args
			{
				Paged_Results = DataReader_To_Result_List_With_LookAhead2(reader, ResultsPerPage, metadataFields)
			};

			// Create the overall search statistics?
			if (ReturnSearchStatistics)
			{
				Search_Results_Statistics stats = new Search_Results_Statistics(reader, null, metadataFields);
				returnArgs.Statistics = stats;
				readerWrapper.Close();
				stats.Total_Items = Convert.ToInt32(paramList[3].Value);
				stats.Total_Titles = Convert.ToInt32(paramList[4].Value);
			}
			else
			{
				// Close the reader (which also closes the connection)
				readerWrapper.Close();
			}

			// Return the built results
			return returnArgs;
		}

		/// <summary> Returns the list of all items/titles which match a given ALEPH number </summary>
		/// <param name="AlephNumber"> ALEPH number to look for matching items </param>
		/// <param name="IncludePrivateItems"> Flag indicates whether to include private items in the result set </param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="ReturnSearchStatistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Table with all of the item and item group information which matches the ALEPH number </returns>
		/// <remarks> This calls the 'SobekCM_Items_By_ALEPH' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Items_By_ALEPH_Number(int AlephNumber, bool IncludePrivateItems, int ResultsPerPage, int Sort, bool ReturnSearchStatistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Items_By_ALEPH_Number", "Searching by ALEPH in the database");
			}

			// Build the parameter list
			EalDbParameter[] paramList = new EalDbParameter[5];
			paramList[0] = new EalDbParameter("@aleph_number", AlephNumber);
			paramList[1] = new EalDbParameter("@include_private", IncludePrivateItems);
			paramList[2] = new EalDbParameter("@sort", Sort);
			paramList[3] = new EalDbParameter("@total_items", 0) {Direction = ParameterDirection.InputOutput};
			paramList[4] = new EalDbParameter("@total_titles", 0) {Direction = ParameterDirection.InputOutput};

			// Create the database agnostic reader
			EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Items_By_ALEPH", paramList);

			// Pull out the database reader
			DbDataReader reader = readerWrapper.Reader;       

			// Create the return argument object
			List<string> metadataFields = new List<string>();
			Multiple_Paged_Results_Args returnArgs = new Multiple_Paged_Results_Args
			{Paged_Results = DataReader_To_Result_List_With_LookAhead2(reader, ResultsPerPage, metadataFields)};

			// Create the overall search statistics?
			if (ReturnSearchStatistics)
			{
				Search_Results_Statistics stats = new Search_Results_Statistics(reader, null, metadataFields);
				returnArgs.Statistics = stats;
				readerWrapper.Close();
				stats.Total_Items = Convert.ToInt32(paramList[3].Value);
				stats.Total_Titles = Convert.ToInt32(paramList[4].Value);
			}
			else
			{
				// Close the reader (which also closes the connection)
				readerWrapper.Close();
			}

			// Return the built results
			return returnArgs;
		}

		#endregion

		#region Methods to get the items within a user's folder or a public folder (works like searches)

		/// <summary> Get a browse of all items in a user's folder </summary>
		/// <param name="UserID"> Primary key for this user in the database </param>
		/// <param name="FolderName"> Name of this user's folder </param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
		/// <param name="IncludeFacets"> Flag indicates if facets should be included in the final result set</param>
		/// <param name="FacetTypes"> Primary key for the metadata types to include as facets (up to eight)</param>
		/// <param name="ReturnSearchStatistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> List of items matching search </returns>
		/// <remarks> This calls the 'mySobek_Get_User_Folder_Browse' stored procedure</remarks> 
		public static Single_Paged_Results_Args Get_User_Folder_Browse(int UserID, string FolderName, int ResultsPerPage, int ResultsPage, bool IncludeFacets, List<short> FacetTypes, bool ReturnSearchStatistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_User_Folder_Browse", String.Empty);
			}

			// Build the parameters
			List<EalDbParameter> parameters = new List<EalDbParameter>
			{
			    new EalDbParameter("@userid", UserID), 
                new EalDbParameter("@foldername", FolderName), 
                new EalDbParameter("@pagesize", ResultsPerPage), 
                new EalDbParameter("@pagenumber", ResultsPage), 
                new EalDbParameter("@include_facets", IncludeFacets)
			};
		    if ((IncludeFacets) && (FacetTypes != null))
			{
				parameters.Add(FacetTypes.Count > 0 ? new EalDbParameter("@facettype1", FacetTypes[0]) : new EalDbParameter("@facettype1", -1));
				parameters.Add(FacetTypes.Count > 1 ? new EalDbParameter("@facettype2", FacetTypes[1]) : new EalDbParameter("@facettype2", -1));
				parameters.Add(FacetTypes.Count > 2 ? new EalDbParameter("@facettype3", FacetTypes[2]) : new EalDbParameter("@facettype3", -1));
				parameters.Add(FacetTypes.Count > 3 ? new EalDbParameter("@facettype4", FacetTypes[3]) : new EalDbParameter("@facettype4", -1));
				parameters.Add(FacetTypes.Count > 4 ? new EalDbParameter("@facettype5", FacetTypes[4]) : new EalDbParameter("@facettype5", -1));
				parameters.Add(FacetTypes.Count > 5 ? new EalDbParameter("@facettype6", FacetTypes[5]) : new EalDbParameter("@facettype6", -1));
				parameters.Add(FacetTypes.Count > 6 ? new EalDbParameter("@facettype7", FacetTypes[6]) : new EalDbParameter("@facettype7", -1));
				parameters.Add(FacetTypes.Count > 7 ? new EalDbParameter("@facettype8", FacetTypes[7]) : new EalDbParameter("@facettype8", -1));
			}
			else
			{
				parameters.Add(new EalDbParameter("@facettype1", -1));
				parameters.Add(new EalDbParameter("@facettype2", -1));
				parameters.Add(new EalDbParameter("@facettype3", -1));
				parameters.Add(new EalDbParameter("@facettype4", -1));
				parameters.Add(new EalDbParameter("@facettype5", -1));
				parameters.Add(new EalDbParameter("@facettype6", -1));
				parameters.Add(new EalDbParameter("@facettype7", -1));
				parameters.Add(new EalDbParameter("@facettype8", -1));
			}

			// Add parameters for total items and total titles
			EalDbParameter totalItemsParameter = new EalDbParameter("@total_items", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalItemsParameter);

			EalDbParameter totalTitlesParameter = new EalDbParameter("@total_titles", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalTitlesParameter);

			// Create the database agnostic reader
			EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String + "Connection Timeout=45", CommandType.StoredProcedure, "mySobek_Get_User_Folder_Browse", parameters);

			// Pull out the database reader
			DbDataReader reader = readerWrapper.Reader;

			// Create the return argument object
			List<string> metadataLabels = new List<string>();
			Single_Paged_Results_Args returnArgs = new Single_Paged_Results_Args {Paged_Results = DataReader_To_Simple_Result_List2(reader, metadataLabels)};

			// Create the overall search statistics?
			if (ReturnSearchStatistics)
			{
				Search_Results_Statistics stats = new Search_Results_Statistics(reader, FacetTypes, metadataLabels);
				returnArgs.Statistics = stats;
				readerWrapper.Close();
				stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
				stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
			}
			else
			{
				// Close the reader (which also closes the connection)
				readerWrapper.Close();
			}

			// Return the built result arguments
			return returnArgs;
		}


		/// <summary> Get a browse of all items in a user's public folder </summary>
		/// <param name="UserFolderID"> Primary key for this user's folder which should be public in the database </param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
		/// <param name="IncludeFacets"> Flag indicates if facets should be included in the final result set</param>
		/// <param name="FacetTypes"> Primary key for the metadata types to include as facets (up to eight)</param>
		/// <param name="ReturnSearchStatistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> List of items matching search </returns>
		/// <remarks> This calls the 'mySobek_Get_Public_Folder_Browse' stored procedure</remarks> 
		public static Single_Paged_Results_Args Get_Public_Folder_Browse(int UserFolderID, int ResultsPerPage, int ResultsPage, bool IncludeFacets, List<short> FacetTypes, bool ReturnSearchStatistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_Public_Folder_Browse", String.Empty);
			}

			// Build the paremeters list
			List<EalDbParameter> parameters = new List<EalDbParameter>
			{
			    new EalDbParameter("@folderid", UserFolderID), 
                new EalDbParameter("@pagesize", ResultsPerPage), 
                new EalDbParameter("@pagenumber", ResultsPage), 
                new EalDbParameter("@include_facets", IncludeFacets)
			};
		    if ((IncludeFacets) && (FacetTypes != null))
			{
				parameters.Add(FacetTypes.Count > 0 ? new EalDbParameter("@facettype1", FacetTypes[0]) : new EalDbParameter("@facettype1", -1));
				parameters.Add(FacetTypes.Count > 1 ? new EalDbParameter("@facettype2", FacetTypes[1]) : new EalDbParameter("@facettype2", -1));
				parameters.Add(FacetTypes.Count > 2 ? new EalDbParameter("@facettype3", FacetTypes[2]) : new EalDbParameter("@facettype3", -1));
				parameters.Add(FacetTypes.Count > 3 ? new EalDbParameter("@facettype4", FacetTypes[3]) : new EalDbParameter("@facettype4", -1));
				parameters.Add(FacetTypes.Count > 4 ? new EalDbParameter("@facettype5", FacetTypes[4]) : new EalDbParameter("@facettype5", -1));
				parameters.Add(FacetTypes.Count > 5 ? new EalDbParameter("@facettype6", FacetTypes[5]) : new EalDbParameter("@facettype6", -1));
				parameters.Add(FacetTypes.Count > 6 ? new EalDbParameter("@facettype7", FacetTypes[6]) : new EalDbParameter("@facettype7", -1));
				parameters.Add(FacetTypes.Count > 7 ? new EalDbParameter("@facettype8", FacetTypes[7]) : new EalDbParameter("@facettype8", -1));
			}
			else
			{
				parameters.Add(new EalDbParameter("@facettype1", -1));
				parameters.Add(new EalDbParameter("@facettype2", -1));
				parameters.Add(new EalDbParameter("@facettype3", -1));
				parameters.Add(new EalDbParameter("@facettype4", -1));
				parameters.Add(new EalDbParameter("@facettype5", -1));
				parameters.Add(new EalDbParameter("@facettype6", -1));
				parameters.Add(new EalDbParameter("@facettype7", -1));
				parameters.Add(new EalDbParameter("@facettype8", -1));
			}

			// Add parameters for total items and total titles
			EalDbParameter totalItemsParameter = new EalDbParameter("@total_items", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalItemsParameter);

			EalDbParameter totalTitlesParameter = new EalDbParameter("@total_titles", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalTitlesParameter);

			// Create the database agnostic reader
			EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String + "Connection Timeout=45", CommandType.StoredProcedure, "mySobek_Get_Public_Folder_Browse", parameters);

			// Pull out the database reader
			DbDataReader reader = readerWrapper.Reader;

			// Create the return argument object
			List<string> metadataLabels = new List<string>();
			Single_Paged_Results_Args returnArgs = new Single_Paged_Results_Args {Paged_Results = DataReader_To_Simple_Result_List2(reader, metadataLabels)};

			// Create the overall search statistics?
			if (ReturnSearchStatistics)
			{
				Search_Results_Statistics stats = new Search_Results_Statistics(reader, FacetTypes, metadataLabels);
				returnArgs.Statistics = stats;
				readerWrapper.Close();
				stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
				stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
			}
			else
			{
				// Close the reader (which also closes the connection)
				readerWrapper.Close();
			}

			// Return the built result arguments
			return returnArgs;
		}

		#endregion

		#region Methods to retrieve the BROWSE information for the entire library

		/// <summary> Gets the collection of all (public) items in the library </summary>
		/// <param name="OnlyNewItems"> Flag indicates to only pull items added in the last two weeks</param>
		/// <param name="IncludePrivateItems"> Flag indicates whether to include private items in the result set </param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="IncludeFacets"> Flag indicates if facets should be included in the final result set</param>
		/// <param name="FacetTypes"> Primary key for the metadata types to include as facets (up to eight)</param>
		/// <param name="ReturnSearchStatistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Table with all of the item and item group information </returns>
		/// <remarks> This calls either the 'SobekCM_Get_All_Browse_Paged' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Get_All_Browse_Paged(bool OnlyNewItems, bool IncludePrivateItems, int ResultsPerPage, int ResultsPage, int Sort, bool IncludeFacets, List<short> FacetTypes, bool ReturnSearchStatistics, Custom_Tracer Tracer)
		{
			if (OnlyNewItems)
			{
				// Get the date string to use
				DateTime sinceDate = DateTime.Now.Subtract(new TimeSpan(14, 0, 0, 0));
				string dateString = sinceDate.Year.ToString().PadLeft(4, '0') + "-" + sinceDate.Month.ToString().PadLeft(2, '0') + "-" + sinceDate.Day.ToString().PadLeft(2, '0');
				return Get_All_Browse_Paged(dateString, IncludePrivateItems, ResultsPerPage, ResultsPage, Sort, IncludeFacets, FacetTypes, ReturnSearchStatistics, Tracer);
			}

			// 1/1/2000 is a special date in the database, which means NO DATE
			return Get_All_Browse_Paged(String.Empty, IncludePrivateItems, ResultsPerPage, ResultsPage, Sort, IncludeFacets, FacetTypes, ReturnSearchStatistics, Tracer);
		}

		/// <summary> Gets the collection of all (public) items in the library </summary>
		/// <param name="SinceDate"> Date from which to pull the data </param>
		/// <param name="IncludePrivateItems"> Flag indicates whether to include private items in the result set </param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="IncludeFacets"> Flag indicates if facets should be included in the final result set</param>
		/// <param name="FacetTypes"> Primary key for the metadata types to include as facets (up to eight)</param>
		/// <param name="ReturnSearchStatistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Table with all of the item and item group information </returns>
		/// <remarks> This calls the 'SobekCM_Get_All_Browse_Paged2' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Get_All_Browse_Paged(string SinceDate, bool IncludePrivateItems, int ResultsPerPage, int ResultsPage, int Sort, bool IncludeFacets, List<short> FacetTypes, bool ReturnSearchStatistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_All_Browse_Paged", "Pulling browse from database");
			}
			
			// Create the parameter list
			List<EalDbParameter> parameters = new List<EalDbParameter>
			{
			    SinceDate.Length > 0 ? new EalDbParameter("@date", SinceDate) : new EalDbParameter("@date", DBNull.Value), 
                new EalDbParameter("@include_private", IncludePrivateItems), 
                new EalDbParameter("@pagesize", ResultsPerPage), 
                new EalDbParameter("@pagenumber", ResultsPage), 
                new EalDbParameter("@sort", Sort), 
                new EalDbParameter("@minpagelookahead", MIN_PAGE_LOOKAHEAD), 
                new EalDbParameter("@maxpagelookahead", MAX_PAGE_LOOKAHEAD), 
                new EalDbParameter("@lookahead_factor", LOOKAHEAD_FACTOR), 
                new EalDbParameter("@include_facets", IncludeFacets)
			};
		    if ((IncludeFacets) && (FacetTypes != null))
			{
				parameters.Add(FacetTypes.Count > 0 ? new EalDbParameter("@facettype1", FacetTypes[0]) : new EalDbParameter("@facettype1", -1));
				parameters.Add(FacetTypes.Count > 1 ? new EalDbParameter("@facettype2", FacetTypes[1]) : new EalDbParameter("@facettype2", -1));
				parameters.Add(FacetTypes.Count > 2 ? new EalDbParameter("@facettype3", FacetTypes[2]) : new EalDbParameter("@facettype3", -1));
				parameters.Add(FacetTypes.Count > 3 ? new EalDbParameter("@facettype4", FacetTypes[3]) : new EalDbParameter("@facettype4", -1));
				parameters.Add(FacetTypes.Count > 4 ? new EalDbParameter("@facettype5", FacetTypes[4]) : new EalDbParameter("@facettype5", -1));
				parameters.Add(FacetTypes.Count > 5 ? new EalDbParameter("@facettype6", FacetTypes[5]) : new EalDbParameter("@facettype6", -1));
				parameters.Add(FacetTypes.Count > 6 ? new EalDbParameter("@facettype7", FacetTypes[6]) : new EalDbParameter("@facettype7", -1));
				parameters.Add(FacetTypes.Count > 7 ? new EalDbParameter("@facettype8", FacetTypes[7]) : new EalDbParameter("@facettype8", -1));
			}
			else
			{
				parameters.Add(new EalDbParameter("@facettype1", -1));
				parameters.Add(new EalDbParameter("@facettype2", -1));
				parameters.Add(new EalDbParameter("@facettype3", -1));
				parameters.Add(new EalDbParameter("@facettype4", -1));
				parameters.Add(new EalDbParameter("@facettype5", -1));
				parameters.Add(new EalDbParameter("@facettype6", -1));
				parameters.Add(new EalDbParameter("@facettype7", -1));
				parameters.Add(new EalDbParameter("@facettype8", -1));
			}
			parameters.Add(new EalDbParameter("@item_count_to_use_cached", 1000));

			// Add parameters for total items and total titles
			EalDbParameter totalItemsParameter = new EalDbParameter("@total_items", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalItemsParameter);

			EalDbParameter totalTitlesParameter = new EalDbParameter("@total_titles", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalTitlesParameter);


			Multiple_Paged_Results_Args returnArgs;
			try
			{

				// Create the database agnostic reader
				EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String + ";Connection Timeout=45", CommandType.StoredProcedure, "SobekCM_Get_All_Browse_Paged2", parameters);

				// Pull out the database reader
				DbDataReader reader = readerWrapper.Reader;

				// Create the return argument object
				List<string> metadataLabels = new List<string>();
				returnArgs = new Multiple_Paged_Results_Args
				{
					Paged_Results = DataReader_To_Result_List_With_LookAhead2(reader, ResultsPerPage, metadataLabels)
				};

				// Create the overall search statistics?
				if (ReturnSearchStatistics)
				{
					Search_Results_Statistics stats = new Search_Results_Statistics(reader, FacetTypes, metadataLabels);
					returnArgs.Statistics = stats;
					readerWrapper.Close();
					stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
					stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
				}
				else
				{
					// Close the reader (which also closes the connection)
					readerWrapper.Close();
				}
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Get_All_Browse_Paged", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_All_Browse_Paged", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_All_Browse_Paged", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				throw;
			}

			// Return the built result arguments
			return returnArgs;
		}

		#endregion

		#region Method to retrieve the BROWSE information from the database for an item aggregation

		/// <summary> Gets the collection of all (public) items linked to an item aggregation </summary>
		/// <param name="AggregationCode"> Code for the item aggregation of interest </param>
		/// <param name="OnlyNewItems"> Flag indicates to only pull items added in the last two weeks</param>
		/// <param name="IncludePrivateItems"> Flag indicates whether to include private items in the result set </param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="IncludeFacets"> Flag indicates if facets should be included in the final result set</param>
		/// <param name="FacetTypes"> Primary key for the metadata types to include as facets (up to eight)</param>
		/// <param name="ReturnSearchStatistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Table with all of the item and item group information </returns>
		/// <remarks> This calls either the 'SobekCM_Get_Aggregation_Browse_Paged' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Get_Item_Aggregation_Browse_Paged(string AggregationCode, bool OnlyNewItems, bool IncludePrivateItems, int ResultsPerPage, int ResultsPage, int Sort, bool IncludeFacets, List<short> FacetTypes, bool ReturnSearchStatistics, Custom_Tracer Tracer)
		{
			if (OnlyNewItems)
			{
				// Get the date string to use
				DateTime sinceDate = DateTime.Now.Subtract(new TimeSpan(14, 0, 0, 0));
				string dateString = sinceDate.Year.ToString().PadLeft(4, '0') + "-" + sinceDate.Month.ToString().PadLeft(2, '0') + "-" + sinceDate.Day.ToString().PadLeft(2, '0');
				return Get_Item_Aggregation_Browse_Paged(AggregationCode, dateString, IncludePrivateItems, ResultsPerPage, ResultsPage, Sort, IncludeFacets, FacetTypes, ReturnSearchStatistics, Tracer);
			}

			// 1/1/2000 is a special date in the database, which means NO DATE
			return Get_Item_Aggregation_Browse_Paged(AggregationCode, "2000-01-01", IncludePrivateItems, ResultsPerPage, ResultsPage, Sort, IncludeFacets, FacetTypes, ReturnSearchStatistics, Tracer);
		}

		/// <summary> Gets the collection of all (public) items linked to an item aggregation </summary>
		/// <param name="AggregationCode"> Code for the item aggregation of interest </param>
		/// <param name="SinceDate"> Date from which to pull the data </param>
		/// <param name="IncludePrivateItems"> Flag indicates whether to include private items in the result set </param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="IncludeFacets"> Flag indicates if facets should be included in the final result set</param>
		/// <param name="FacetTypes"> Primary key for the metadata types to include as facets (up to eight)</param>
		/// <param name="ReturnSearchStatistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Table with all of the item and item group information </returns>
		/// <remarks> This calls the 'SobekCM_Get_Aggregation_Browse_Paged2' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Get_Item_Aggregation_Browse_Paged(string AggregationCode, string SinceDate, bool IncludePrivateItems, int ResultsPerPage, int ResultsPage, int Sort, bool IncludeFacets, List<short> FacetTypes, bool ReturnSearchStatistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_Item_Aggregation_Browse_Paged", "Pulling browse from database");
			}

			// Build the parameters list
			List<EalDbParameter> parameters = new List<EalDbParameter>
			{
			    new EalDbParameter("@code", AggregationCode), 
                new EalDbParameter("@date", SinceDate), 
                new EalDbParameter("@include_private", IncludePrivateItems), 
                new EalDbParameter("@pagesize", ResultsPerPage), 
                new EalDbParameter("@pagenumber", ResultsPage), 
                new EalDbParameter("@sort", Sort)
			};

		    if (ResultsPerPage > 100)
			{
				parameters.Add(new EalDbParameter("@minpagelookahead", 1));
				parameters.Add(new EalDbParameter("@maxpagelookahead", 1));
				parameters.Add(new EalDbParameter("@lookahead_factor", LOOKAHEAD_FACTOR));
			}
			else
			{
				parameters.Add(new EalDbParameter("@minpagelookahead", MIN_PAGE_LOOKAHEAD));
				parameters.Add(new EalDbParameter("@maxpagelookahead", MAX_PAGE_LOOKAHEAD));
				parameters.Add(new EalDbParameter("@lookahead_factor", LOOKAHEAD_FACTOR));
			}


			if ((IncludeFacets) && (FacetTypes != null))
			{
				parameters.Add(new EalDbParameter("@include_facets", true));
				parameters.Add(FacetTypes.Count > 0 ? new EalDbParameter("@facettype1", FacetTypes[0]) : new EalDbParameter("@facettype1", -1));
				parameters.Add(FacetTypes.Count > 1 ? new EalDbParameter("@facettype2", FacetTypes[1]) : new EalDbParameter("@facettype2", -1));
				parameters.Add(FacetTypes.Count > 2 ? new EalDbParameter("@facettype3", FacetTypes[2]) : new EalDbParameter("@facettype3", -1));
				parameters.Add(FacetTypes.Count > 3 ? new EalDbParameter("@facettype4", FacetTypes[3]) : new EalDbParameter("@facettype4", -1));
				parameters.Add(FacetTypes.Count > 4 ? new EalDbParameter("@facettype5", FacetTypes[4]) : new EalDbParameter("@facettype5", -1));
				parameters.Add(FacetTypes.Count > 5 ? new EalDbParameter("@facettype6", FacetTypes[5]) : new EalDbParameter("@facettype6", -1));
				parameters.Add(FacetTypes.Count > 6 ? new EalDbParameter("@facettype7", FacetTypes[6]) : new EalDbParameter("@facettype7", -1));
				parameters.Add(FacetTypes.Count > 7 ? new EalDbParameter("@facettype8", FacetTypes[7]) : new EalDbParameter("@facettype8", -1));
			}
			else
			{
				parameters.Add(new EalDbParameter("@include_facets", false));
				parameters.Add(new EalDbParameter("@facettype1", -1));
				parameters.Add(new EalDbParameter("@facettype2", -1));
				parameters.Add(new EalDbParameter("@facettype3", -1));
				parameters.Add(new EalDbParameter("@facettype4", -1));
				parameters.Add(new EalDbParameter("@facettype5", -1));
				parameters.Add(new EalDbParameter("@facettype6", -1));
				parameters.Add(new EalDbParameter("@facettype7", -1));
				parameters.Add(new EalDbParameter("@facettype8", -1));
			}
			parameters.Add(new EalDbParameter("@item_count_to_use_cached", 1000));

			// Add parameters for total items and total titles
			EalDbParameter totalItemsParameter = new EalDbParameter("@total_items", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalItemsParameter);

			EalDbParameter totalTitlesParameter = new EalDbParameter("@total_titles", 0) {Direction = ParameterDirection.InputOutput};
			parameters.Add(totalTitlesParameter);

			// Create the database agnostic reader
			EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String + "Connection Timeout=45", CommandType.StoredProcedure, "SobekCM_Get_Aggregation_Browse_Paged2", parameters);

			// Pull out the database reader
			DbDataReader reader = readerWrapper.Reader;

			// Create the return argument object
			List<string> metadataLabels = new List<string>();
			Multiple_Paged_Results_Args returnArgs = new Multiple_Paged_Results_Args {Paged_Results = DataReader_To_Result_List_With_LookAhead2(reader, ResultsPerPage, metadataLabels)};

			// Create the overall search statistics?
			if (ReturnSearchStatistics)
			{
				Search_Results_Statistics stats = new Search_Results_Statistics(reader, FacetTypes, metadataLabels);
				returnArgs.Statistics = stats;
				readerWrapper.Close();
				stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
				stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
			}
			else
			{
				// Close the reader (which also closes the connection)
				readerWrapper.Close();
			}



			// Return the built result arguments
			return returnArgs;
		}

		/// <summary> Gets the list of all data for a particular metadata field in a particular aggregation </summary>
		/// <param name="AggregationCode"> Code for the item aggregation </param>
		/// <param name="MetadataCode"> Metadata code for the field of interest </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> List with all the metadata fields in alphabetical order </returns>
		/// <remarks> This calls the 'SobekCM_Get_Metadata_Browse' stored procedure </remarks>
		public static List<string> Get_Item_Aggregation_Metadata_Browse(string AggregationCode, string MetadataCode, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_Item_Aggregation_Metadata_Browse", "Pull the metadata browse");
			}

			// Build the parameter list
			EalDbParameter[] paramList = new EalDbParameter[3];
			paramList[0] = new EalDbParameter("@aggregation_code", AggregationCode);
			paramList[1] = new EalDbParameter("@metadata_name", MetadataCode);
			paramList[2] = new EalDbParameter("@item_count_to_use_cached", 1);

			// Define a temporary dataset
			DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Metadata_Browse", paramList);

			if (tempSet == null)
				return null;

			DataColumn column = tempSet.Tables[0].Columns[1];
			DataTable table = tempSet.Tables[0];
			return (from DataRow thisRow in table.Rows select thisRow[column].ToString()).ToList();
		}

		/// <summary> Gets the list of unique coordinate points and associated bibid and group title for a single 
		/// item aggregation </summary>
		/// <param name="AggregationCode"> Code for the item aggregation </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataTable with all the coordinate values </returns>
		/// <remarks> This calls the 'SobekCM_Coordinate_Points_By_Aggregation' stored procedure </remarks>
		public static DataTable Get_All_Coordinate_Points_By_Aggregation(string AggregationCode, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_All_Coordinate_Points_By_Aggregation", "Pull the coordinate list");
			}

			// Build the parameter list
			EalDbParameter[] paramList = new EalDbParameter[1];
			paramList[0] = new EalDbParameter("@aggregation_code", AggregationCode);

			// Define a temporary dataset
			DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Coordinate_Points_By_Aggregation", paramList);
			return tempSet == null ? null : tempSet.Tables[0];
		}

		#endregion

		#region Methods to get the item aggregation

		/// <summary> Adds the title, item, and page counts to this item aggregation object </summary>
		/// <param name="Aggregation"> Mostly built item aggregation object </param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This method calls the stored procedure 'SobekCM_Get_Item_Aggregation2'. </remarks>
		public static bool Get_Item_Aggregation_Counts(Complete_Item_Aggregation Aggregation, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_Item_Aggregation_Counts", "Add the title, item, and page count to the item aggregation object");
			}

			try
			{
				// Build the parameter list
				EalDbParameter[] paramList = new EalDbParameter[3];
				paramList[0] = new EalDbParameter("@code", Aggregation.Code);
				paramList[1] = new EalDbParameter("@include_counts", true);
				paramList[2] = new EalDbParameter("@is_robot", false);

				// Define a temporary dataset
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Aggregation2", paramList);

				// Add the counts for this item aggregation
				if (tempSet.Tables.Count > 4)
				{
					add_counts(Aggregation, tempSet.Tables[4]);
				}


				// Return the built argument set
				return true;
			}
			catch (Exception ee)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Get_Item_Aggregation_Counts", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Item_Aggregation_Counts", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Item_Aggregation_Counts", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}

		}

		/// <summary> Gets the database information about a single item aggregation </summary>
		/// <param name="Code"> Code specifying the item aggregation to retrieve </param>
		/// <param name="IncludeCounts"> Flag indicates whether to pull the title/item/page counts for this aggregation </param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Arguments which include the <see cref="Item_Aggregation"/> object and a DataTable of the search field information</returns>
		/// <remarks> This method calls the stored procedure 'SobekCM_Get_Item_Aggregation2'. </remarks>
		public static Complete_Item_Aggregation Get_Item_Aggregation(string Code, bool IncludeCounts, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_Item_Aggregation", "Pulling item aggregation data from database");
			}

			try
			{
				// Build the parameter list
				EalDbParameter[] paramList = new EalDbParameter[1];
				paramList[0] = new EalDbParameter("@code", Code);

				// Define a temporary dataset
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Aggregation", paramList);

				// If there was no data for this collection and entry point, return null (an ERROR occurred)
				if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
				{
					return null;
				}

				// Build the collection group object
				Complete_Item_Aggregation aggrInfo = create_basic_aggregation_from_datatable(tempSet.Tables[0]);

				// Add the child objects from that table
				add_children(aggrInfo, tempSet.Tables[1]);

				// Add the advanced search values
				add_advanced_terms(aggrInfo, tempSet.Tables[2]);

				// Add the parents
				add_parents(aggrInfo, tempSet.Tables[3]);

                // Determine the middle point and zoom for the extent of the coordinates
			    if (( tempSet.Tables[4].Rows.Count > 0 ) && ( tempSet.Tables[4].Rows[0][0] != DBNull.Value))
			    {
                    try
                    {
                        DataRow coordsRow = tempSet.Tables[4].Rows[0];
                        decimal min_latitude = Decimal.Parse(coordsRow["Min_Latitude"].ToString());
                        decimal max_latitude = Decimal.Parse(coordsRow["Max_Latitude"].ToString());
                        decimal min_longitude = Decimal.Parse(coordsRow["Min_Longitude"].ToString());
                        decimal max_longitude = Decimal.Parse(coordsRow["Max_Longitude"].ToString());

                        // Determine the center point
                        decimal center_latitude = (min_latitude + max_latitude)/2m;
                        decimal center_longitude = (min_longitude + max_longitude) / 2m;

                        // Determine the zoom
                        double GLOBE_WIDTH = 256; // a constant in Google's map projection
                        double angle = (double) (max_longitude - min_longitude);
                        if (angle < 0)
                        {
                            angle += 360;
                        }
                        double intermediaryValue = Math.Log(600d*360d/angle/GLOBE_WIDTH);
                        int zoom = (int) (Math.Round(intermediaryValue/0.6931471805599453));

                        // Add this to the item (may be changed when the aggregation XML config is read though)
                        aggrInfo.Map_Search_Display = new Item_Aggregation_Map_Coverage_Info(Item_Aggregation_Map_Coverage_Type_Enum.COMPUTED, zoom, center_longitude, center_latitude);
                    }
                    catch { }

			    }

				// Return the built argument set
				return aggrInfo;
			}
			catch (Exception ee)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Get_Item_Aggregation", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Item_Aggregation", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Item_Aggregation", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}

				throw;
			}
		}

		/// <summary> Gets the database information about the main aggregation, representing the entire web page </summary>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Arguments which include the <see cref="Item_Aggregation"/> object and a DataTable of the search field information</returns>
		/// <remarks> This method calls the stored procedure 'SobekCM_Get_All_Groups'. </remarks>
		public static Complete_Item_Aggregation Get_Main_Aggregation(Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_Main_Aggregation", "Pulling item aggregation data from database");
			}

			try
			{
				// Build the parameter list
				EalDbParameter[] paramList = new EalDbParameter[1];
				paramList[0] = new EalDbParameter("@metadata_count_to_use_cache", ALL_AGGREGATIONS_METADATA_COUNT_TO_USE_CACHED);

				// Define a temporary dataset
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_All_Groups", paramList);

				// If there was no data for this collection and entry point, return null (an ERROR occurred)
				if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
				{
					return null;
				}

				// Build the collection group object
				Complete_Item_Aggregation aggrInfo = create_basic_aggregation_from_datatable(tempSet.Tables[0]);

				// Add the advanced search values
				add_advanced_terms(aggrInfo, tempSet.Tables[1]);

                // Determine the middle point and zoom for the extent of the coordinates
                if ((tempSet.Tables[2].Rows.Count > 0) && (tempSet.Tables[2].Rows[0][0] != DBNull.Value))
                {
                    try
                    {
                        DataRow coordsRow = tempSet.Tables[2].Rows[0];
                        decimal min_latitude = Decimal.Parse(coordsRow["Min_Latitude"].ToString());
                        decimal max_latitude = Decimal.Parse(coordsRow["Max_Latitude"].ToString());
                        decimal min_longitude = Decimal.Parse(coordsRow["Min_Longitude"].ToString());
                        decimal max_longitude = Decimal.Parse(coordsRow["Max_Longitude"].ToString());

                        // Determine the center point
                        decimal center_latitude = (min_latitude + max_latitude) / 2m;
                        decimal center_longitude = (min_longitude + max_longitude) / 2m;

                        // Determine the zoom
                        double GLOBE_WIDTH = 256; // a constant in Google's map projection
                        double angle = (double)(max_longitude - min_longitude);
                        if (angle < 0)
                        {
                            angle += 360;
                        }
                        double intermediaryValue = Math.Log(600d * 360d / angle / GLOBE_WIDTH);
                        int zoom = (int)(Math.Round(intermediaryValue / 0.6931471805599453));

                        // Add this to the item (may be changed when the aggregation XML config is read though)
                        aggrInfo.Map_Search_Display = new Item_Aggregation_Map_Coverage_Info(Item_Aggregation_Map_Coverage_Type_Enum.COMPUTED, zoom, center_longitude, center_latitude);
                    }
                    catch { }

                }

				// Return the built argument set
				return aggrInfo;
			}
			catch (Exception ee)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Get_Main_Aggregation", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Main_Aggregation", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Main_Aggregation", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				throw;
			}
		}

		/// <summary> Creates the item aggregation object from the datatable extracted from the database </summary>
		/// <param name="BasicInfo">Datatable from database calls ( either SobekCM_Get_Item_Aggregation or SobekCM_Get_All_Groups )</param>
		/// <returns>Minimally built aggregation object</returns>
		/// <remarks>The child and parent information is not yet added to the returned object </remarks>
		private static Complete_Item_Aggregation create_basic_aggregation_from_datatable(DataTable BasicInfo)
		{
			// Pull out this row
			DataRow thisRow = BasicInfo.Rows[0];

			string displayOptions = thisRow[15].ToString();
			DateTime lastAdded = new DateTime(2000, 1, 1);
			if (thisRow[16] != DBNull.Value)
				lastAdded = Convert.ToDateTime(thisRow[16]);

			// Build the collection group object
			Complete_Item_Aggregation aggrInfo = new Complete_Item_Aggregation(Engine_ApplicationCache_Gateway.Settings.System.Default_UI_Language,
				thisRow[1].ToString().ToLower(), thisRow[4].ToString(), Convert.ToInt32(thisRow[0]), displayOptions, lastAdded)
			{
				Name = thisRow[2].ToString(),
				ShortName = thisRow[3].ToString(),
				Active = Convert.ToBoolean(thisRow[5]),
				Hidden = Convert.ToBoolean(thisRow[6]),
				Has_New_Items = Convert.ToBoolean(thisRow[7]),
                //Map_Display = Convert.ToUInt16(thisRow[11]),
                //Map_Search = Convert.ToUInt16(thisRow[12]),
				OAI_Enabled = Convert.ToBoolean(thisRow[13]),
				Items_Can_Be_Described = Convert.ToInt16(thisRow[18]),
			};

			if ((thisRow[8] != DBNull.Value) && (thisRow[8].ToString().Length > 0))
				aggrInfo.Contact_Email = thisRow[8].ToString();
			if ((thisRow[9] != DBNull.Value) && (thisRow[9].ToString().Length > 0))
				aggrInfo.Default_Skin = thisRow[9].ToString();
			if ((thisRow[10] != DBNull.Value) && (thisRow[10].ToString().Length > 0))
				aggrInfo.Description = thisRow[10].ToString();
			if ((thisRow[14] != DBNull.Value) && (thisRow[14].ToString().Length > 0))
				aggrInfo.OAI_Metadata = thisRow[14].ToString();
			if ((thisRow[19] != DBNull.Value) && (thisRow[19].ToString().Length > 0))
				aggrInfo.External_Link = thisRow[19].ToString();

			if (BasicInfo.Columns.Contains("ThematicHeadingID"))
			{
				if (thisRow["ThematicHeadingID"] != DBNull.Value)
				{
					int thematicHeadingId = Convert.ToInt32(thisRow["ThematicHeadingID"]);

					if (thematicHeadingId > 0)
					{
						string thematicHeading = thisRow["ThemeName"].ToString();
						aggrInfo.Thematic_Heading = new Thematic_Heading(thematicHeadingId, thematicHeading);
					} 
				}
			}

			// return the built object
			return aggrInfo;
		}

		/// <summary> Adds the child information to the item aggregation object from the datatable extracted from the database </summary>
		/// <param name="AggrInfo">Partially built item aggregation object</param>
		/// <param name="ChildInfo">Datatable from database calls with child item aggregation information ( either SobekCM_Get_Item_Aggregation or SobekCM_Get_All_Groups )</param>
		private static void add_children(Complete_Item_Aggregation AggrInfo, DataTable ChildInfo)
		{
			if (ChildInfo.Rows.Count == 0)
				return;

			string childTypes = String.Empty;

			// Build a dictionary of nodes while building this tree
			Dictionary<string, Item_Aggregation_Related_Aggregations> nodes = new Dictionary<string, Item_Aggregation_Related_Aggregations>(ChildInfo.Rows.Count);

			// Step through each row of children
			foreach (DataRow thisRow in ChildInfo.Rows)
			{
				// pull some of the basic data out
				int hierarchyLevel = Convert.ToInt16(thisRow[5]);
				string code = thisRow[0].ToString().ToLower();
				string parentCode = thisRow[1].ToString().ToLower();

				// If this does not already exist, create it
				if (!nodes.ContainsKey(code))
				{
					// Create the object
					Item_Aggregation_Related_Aggregations childObject = new Item_Aggregation_Related_Aggregations(code, thisRow[2].ToString(), thisRow[4].ToString(), Convert.ToBoolean(thisRow[6]), Convert.ToBoolean(thisRow[7]));

					// Add this object to the node dictionary
					nodes.Add(code, childObject);

					// If this is not ALL, no need to add the full hierarchy
					if ((AggrInfo.Code == "all") || (hierarchyLevel == -1))
					{
						// Check for parent in the node list
						if ((parentCode.Length > 0) && (AggrInfo.Code != parentCode) && (nodes.ContainsKey(parentCode)))
						{
							nodes[parentCode].Add_Child_Aggregation(childObject);
						}
					}

					// If this is the first hierarchy, add to the main item aggregation object
					if (hierarchyLevel == -1)
					{
						AggrInfo.Add_Child_Aggregation(childObject);

						// If this is active and not hidden, check the type and save to list
						if ((!childObject.Hidden) && (childObject.Active))
						{
							if (childTypes.Length == 0)
								childTypes = childObject.Type + "s";
							else if (childTypes != childObject.Type)
								childTypes = "SubCollections";
						}
					}
				}
			}

			// Save the type for the child collections
			AggrInfo.Child_Types = childTypes;
		}

		/// <summary> Adds the child information to the item aggregation object from the datatable extracted from the database </summary>
		/// <param name="AggrInfo">Partially built item aggregation object</param>
		/// <param name="ParentInfo">Datatable from database calls with parent item aggregation information ( from  SobekCM_Get_Item_Aggregation only )</param>
		private static void add_parents(Complete_Item_Aggregation AggrInfo, DataTable ParentInfo)
		{
			foreach (DataRow parentRow in ParentInfo.Rows)
			{
				Item_Aggregation_Related_Aggregations parentObject = new Item_Aggregation_Related_Aggregations(parentRow[0].ToString(), parentRow[1].ToString(), parentRow[3].ToString(), Convert.ToBoolean(parentRow[4]), false);
				AggrInfo.Add_Parent_Aggregation(parentObject);
			}
		}

		/// <summary> Adds the search terms to display under advanced search from the datatable extracted from the database 
		/// and also the list of browseable fields for this collection </summary>
		/// <param name="AggrInfo">Partially built item aggregation object</param>
		/// <param name="SearchTermsTable"> Table of all advanced search values </param>
		private static void add_advanced_terms(Complete_Item_Aggregation AggrInfo, DataTable SearchTermsTable)
		{
			// Add ANYWHERE first
			AggrInfo.Search_Fields.Add(new Complete_Item_Aggregation_Metadata_Type(-1, "Anywhere", "ZZ"));

			// Add values either default values or from the table
			if ((SearchTermsTable != null) && (SearchTermsTable.Rows.Count > 0))
			{
				foreach (DataRow thisRow in SearchTermsTable.Rows)
				{
					short thisTypeId = Convert.ToInt16(thisRow[0]);
					bool canBrowse = Convert.ToBoolean(thisRow[1]);
					string displayTerm = thisRow[2].ToString();
					string sobekCode = thisRow[3].ToString();
					string solrCode = thisRow[4].ToString();

					Complete_Item_Aggregation_Metadata_Type metadataType = new Complete_Item_Aggregation_Metadata_Type(thisTypeId, displayTerm, sobekCode) {SolrCode = solrCode};

					if (!AggrInfo.Search_Fields.Contains(metadataType))
					{
						AggrInfo.Search_Fields.Add(metadataType);
					}
					
					if ((canBrowse) && (!AggrInfo.Browseable_Fields.Contains(metadataType)))
					{
						AggrInfo.Browseable_Fields.Add(metadataType);
					}
				}
			}
		}

		/// <summary> Adds the page count, item count, and title count to the item aggregation object from the datatable extracted from the database </summary>
		/// <param name="AggrInfo">Partially built item aggregation object</param>
		/// <param name="CountInfo">Datatable from database calls with page count, item count, and title count ( from either SobekCM_Get_Item_Aggregation or SobekCM_Get_All_Groups )</param>
		private static void add_counts(Complete_Item_Aggregation AggrInfo, DataTable CountInfo)
		{
			if (CountInfo.Rows.Count > 0)
			{
				AggrInfo.Statistics = new Item_Aggregation_Statistics
				{
					Page_Count = Convert.ToInt32(CountInfo.Rows[0]["Page_Count"]), 
					Item_Count = Convert.ToInt32(CountInfo.Rows[0]["Item_Count"]), 
					Title_Count = Convert.ToInt32(CountInfo.Rows[0]["Title_Count"])
				};
			}
		}

		/// <summary>Method used to get the hierarchical relationship between all aggregationPermissions, to be displayed in the 'aggregationPermissions' tab in the internal screen</summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataTable with relationships between all aggregationPermissions</returns>
		/// <remarks> This calls the 'SobekCM_Get_Collection_Hierarchies' stored procedure <br /><br />
		/// This is used by the Internal_HtmlSubwriter class</remarks>
		public static DataSet Get_Aggregation_Hierarchies(Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_Aggregation_Hierarchies", "Pulling from database");
			}

			try
			{
				// Define a temporary dataset
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Collection_Hierarchies");

				// If there was no data for this collection and entry point, return null (an ERROR occurred)
				if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
				{
					return null;
				}

				// Return the first table from the returned dataset
				return tempSet;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Get_Aggregation_Hierarchies", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Aggregation_Hierarchies", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Aggregation_Hierarchies", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		#endregion

		#region Methods to save the item aggregation to the databaase

		/// <summary> Save a new item aggregation with the basic details provided in the new aggregation form </summary>
		/// <param name="Code"> Code for this item aggregation </param>
		/// <param name="Name"> Name for this item aggregation </param>
		/// <param name="ShortName"> Short version of this item aggregation </param>
		/// <param name="Description"> Description of this item aggregation </param>
		/// <param name="ThematicHeadingID"> Thematic heading id for this item aggregation (or -1)</param>
		/// <param name="Type"> Type of item aggregation (i.e., Collection Group, Institution, Exhibit, etc..)</param>
		/// <param name="IsActive"> Flag indicates if this item aggregation is active</param>
		/// <param name="IsHidden"> Flag indicates if this item is hidden</param>
		/// <param name="ParentID"> ID for the item aggregation parent</param>
		/// <param name="ExternalLink">External link for this item aggregation (used primarily for institutional item aggregationPermissions to provide a link back to the institution's actual home page)</param>
		/// <param name="Username"> Username saving this new item aggregation, for the item aggregation milestones </param>
		/// <param name="LanguageVariants"> Details which language variants exist for this item aggregation </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Save_Item_Aggregation' stored procedure in the SobekCM database</remarks> 
		public static bool Save_Item_Aggregation(string Code, string Name, string ShortName, string Description, int ThematicHeadingID, string Type, bool IsActive, bool IsHidden, string ExternalLink, int ParentID, string Username, string LanguageVariants, Custom_Tracer Tracer)
		{
			return Save_Item_Aggregation(-1, Code, Name, ShortName, Description, new Thematic_Heading(ThematicHeadingID, String.Empty), Type, IsActive, IsHidden, String.Empty, 0, 0, 0, 0, false, String.Empty, String.Empty, String.Empty, ExternalLink, ParentID, Username, LanguageVariants, Tracer);
		}

		/// <summary> Save a new item aggregation or edit an existing item aggregation in the database </summary>
		/// <param name="AggregationID"> AggregationID if this is editing an existing one, otherwise -1 </param>
		/// <param name="Code"> Code for this item aggregation </param>
		/// <param name="Name"> Name for this item aggregation </param>
		/// <param name="ShortName"> Short version of this item aggregation </param>
		/// <param name="Description"> Description of this item aggregation </param>
		/// <param name="ThematicHeading"> Thematic heading for this item aggregation (or null)</param>
		/// <param name="Type"> Type of item aggregation (i.e., Collection Group, Institution, Exhibit, etc..)</param>
		/// <param name="IsActive"> Flag indicates if this item aggregation is active</param>
		/// <param name="IsHidden"> Flag indicates if this item is hidden</param>
		/// <param name="DisplayOptions"> Display options for this item aggregation </param>
		/// <param name="MapSearch"> Map Search value indicates if there is a map search, and the type of search </param>
		/// /// <param name="MapSearchBeta"> Map Search value indicates if there is a map search, and the type of search </param>
		/// <param name="MapDisplay"> Map Display value indicates if there is a map display option when looking at search results or browses </param>
		/// <param name="MapDisplayBeta"> Map Display value indicates if there is a map display option when looking at search results or browses </param>
		/// <param name="OaiFlag"> Flag indicates if this item aggregation should be available via OAI-PMH </param>
		/// <param name="OaiMetadata"> Additional metadata about this collection, to be included in the set information in OAI-PMH</param>
		/// <param name="ContactEmail"> Contact email for this item aggregation (can leave blank to use default)</param>
		/// <param name="DefaultInterface"> Default interface for this item aggregation (particularly useful for institutional aggregationPermissions)</param>
		/// <param name="ExternalLink">External link for this item aggregation (used primarily for institutional item aggregationPermissions to provide a link back to the institution's actual home page)</param>
		/// <param name="ParentID"> ID for the item aggregation parent</param>
		/// <param name="Username"> Username saving this new item aggregation, for the item aggregation milestones </param>
		/// <param name="LanguageVariants"> Details which language variants exist for this item aggregation </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Save_Item_Aggregation' stored procedure in the SobekCM database</remarks> 
		public static bool Save_Item_Aggregation(int AggregationID, string Code, string Name, string ShortName, string Description, Thematic_Heading ThematicHeading, string Type, bool IsActive, bool IsHidden, string DisplayOptions, int MapSearch, int MapSearchBeta, int MapDisplay, int MapDisplayBeta, bool OaiFlag, string OaiMetadata, string ContactEmail, string DefaultInterface, string ExternalLink, int ParentID, string Username, string LanguageVariants, Custom_Tracer Tracer)
		{
			Last_Exception = null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Save_Item_Aggregation", String.Empty);
			}

			try
			{
				// Build the parameter list
				EalDbParameter[] paramList = new EalDbParameter[21];
				paramList[0] = new EalDbParameter("@aggregationid", AggregationID);
				paramList[1] = new EalDbParameter("@code", Code);
				paramList[2] = new EalDbParameter("@name", Name);
				paramList[3] = new EalDbParameter("@shortname", ShortName);
				paramList[4] = new EalDbParameter("@description", Description);
				if (ThematicHeading != null )
					paramList[5] = new EalDbParameter("@thematicHeadingId", ThematicHeading.ID);
				else
					paramList[5] = new EalDbParameter("@thematicHeadingId", -1);
				paramList[6] = new EalDbParameter("@type", Type);
				paramList[7] = new EalDbParameter("@isActive", IsActive);
				paramList[8] = new EalDbParameter("@hidden", IsHidden);
				paramList[9] = new EalDbParameter("@display_options", DisplayOptions);
				paramList[10] = new EalDbParameter("@map_search", MapSearch);
				paramList[11] = new EalDbParameter("@map_display", MapDisplay);
				paramList[12] = new EalDbParameter("@oai_flag", OaiFlag);
				paramList[13] = new EalDbParameter("@oai_metadata", OaiMetadata);
				if ( ContactEmail == null )
					paramList[14] = new EalDbParameter("@contactemail", String.Empty);
				else
					paramList[14] = new EalDbParameter("@contactemail", ContactEmail);
				paramList[15] = new EalDbParameter("@defaultinterface", DefaultInterface);
				paramList[16] = new EalDbParameter("@externallink", ExternalLink);
				paramList[17] = new EalDbParameter("@parentid", ParentID);
				paramList[18] = new EalDbParameter("@username", Username);
				paramList[19] = new EalDbParameter("@languageVariants", LanguageVariants);
				paramList[20] = new EalDbParameter("@newaggregationid", 0) { Direction = ParameterDirection.InputOutput };

				//BETA
				//paramList[20] = new EalDbParameter("@map_search_beta", Map_Search_Beta);
				//paramList[21] = new EalDbParameter("@map_display_beta", Map_Display_Beta);

				// Execute this query stored procedure
				EalDbAccess.ExecuteNonQuery( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Save_Item_Aggregation", paramList);

				// Succesful, so return true
				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Save_Item_Aggregation", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Save_Item_Aggregation", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Save_Item_Aggregation", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		#endregion

		#region Methods relating to sending emails from and logging emails in the database

		/// <summary> Send an email using database mail through the database </summary>
		/// <param name="RecipientList"> List of recepients, seperated by a semi-colon </param>
		/// <param name="SubjectLine"> Subject line for the email to send </param>
		/// <param name="EmailBody"> Body of the email to send</param>
		/// <param name="FromAddress"> Address this is FROM to override system </param>
		/// <param name="ReplyTo"> Address this should have as REPLY TO from system </param>
		/// <param name="IsHtml"> Flag indicates if the email body is HTML-encoded, or plain text </param>
		/// <param name="IsContactUs"> Flag indicates if this was sent from the 'Contact Us' feature of the library, rather than from a mySobek feature such as email your bookshelf </param>
		/// <param name="ReplyToEmailID"> Primary key of the previous email, if this is a reply to a previously logged email </param>
		/// <param name="UserID"> UserID that sent this message.  This is used to restrict the number of messages sent by the same user in the same day </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Send_Email' stored procedure to send and log this email. </remarks>
		public static bool Send_Database_Email(string RecipientList, string SubjectLine, string EmailBody, string FromAddress, string ReplyTo, bool IsHtml, bool IsContactUs, int ReplyToEmailID, int UserID)
		{
			try
			{
				// Build the parameter list
				EalDbParameter[] paramList = new EalDbParameter[9];
				paramList[0] = new EalDbParameter("@recipients_list", RecipientList.Replace(",",";"));
				paramList[1] = new EalDbParameter("@subject_line", SubjectLine);
				paramList[2] = new EalDbParameter("@email_body", EmailBody);
				if ( String.IsNullOrEmpty(FromAddress))
					paramList[3] = new EalDbParameter("@from_address", DBNull.Value);
				else
					paramList[3] = new EalDbParameter("@from_address", FromAddress);

				if (String.IsNullOrEmpty(ReplyTo))
					paramList[4] = new EalDbParameter("@reply_to", DBNull.Value);
				else
					paramList[4] = new EalDbParameter("@reply_to", ReplyTo);

				paramList[5] = new EalDbParameter("@html_format", IsHtml);
				paramList[6] = new EalDbParameter("@contact_us", IsContactUs);
				if (ReplyToEmailID > 0)
				{
					paramList[7] = new EalDbParameter("@replytoemailid", ReplyToEmailID);
				}
				else
				{
					paramList[7] = new EalDbParameter("@replytoemailid", DBNull.Value);
				}
				paramList[8] = new EalDbParameter("@userid", UserID);

				// Execute this non-query stored procedure
				EalDbAccess.ExecuteNonQuery( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Send_Email", paramList);

				return true;
			}
			catch (Exception ee)
			{
				// Pass this exception onto the method to handle it
				Last_Exception = ee;
				return false;
			}
		}

		/// <summary> Log the fact an email was sent via a different system than the databse mail </summary>
		/// <param name="Sender"> Name of the sender indicated in the sent email </param>
		/// <param name="RecipientList"> List of recepients, seperated by a semi-colon </param>
		/// <param name="SubjectLine"> Subject line for the email to log </param>
		/// <param name="EmailBody"> Body of the email to log</param>
		/// <param name="IsHtml"> Flag indicates if the email body is HTML-encoded, or plain text </param>
		/// <param name="IsContactUs"> Flag indicates if this was sent from the 'Contact Us' feature of the library, rather than from a mySobek feature such as email your bookshelf </param>
		/// <param name="ReplyToEmailID"> Primary key of the previous email, if this is a reply to a previously logged email </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Log_Email' stored procedure. </remarks>
		public static bool Log_Sent_Email(string Sender, string RecipientList, string SubjectLine, string EmailBody, bool IsHtml, bool IsContactUs, int ReplyToEmailID)
		{
			try
			{
				// Build the parameter list
				EalDbParameter[] paramList = new EalDbParameter[7];
				paramList[0] = new EalDbParameter("@sender", Sender);
				paramList[1] = new EalDbParameter("@recipients_list", RecipientList);
				paramList[2] = new EalDbParameter("@subject_line", SubjectLine);
				paramList[3] = new EalDbParameter("@email_body", EmailBody);
				paramList[4] = new EalDbParameter("@html_format", IsHtml);
				paramList[5] = new EalDbParameter("@contact_us", IsContactUs);
				if (ReplyToEmailID > 0)
				{
					paramList[6] = new EalDbParameter("@replytoemailid", ReplyToEmailID);
				}
				else
				{
					paramList[6] = new EalDbParameter("@replytoemailid", DBNull.Value);
				}

				// Execute this non-query stored procedure
				EalDbAccess.ExecuteNonQuery( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Log_Email", paramList);

				return true;
			}
			catch (Exception ee)
			{
				// Pass this exception onto the method to handle it
				Last_Exception = ee;
				return false;
			}
		}

		#endregion


		/// <summary> Gets all the setting information necessary for SobekCM </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <param name="IncludeAdminViewInfo"> Flag indicates if the administrative view information should be included </param>
		/// <returns> DataSet with all the data necessary for the Builder, including file destination information,
		/// general settings, server information</returns>
		/// <remarks> This calls the 'SobekCM_Get_Settings' stored procedure </remarks> 
		public static DataSet Get_Settings_Complete( bool IncludeAdminViewInfo, Custom_Tracer Tracer)
		{
			try
			{
                EalDbParameter[] parameters = new EalDbParameter[1];
                parameters[0] = new EalDbParameter("@IncludeAdminViewInfo", IncludeAdminViewInfo);

                DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Settings", parameters);


                //DataSet tempSet = EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Settings");
				return tempSet;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				return null;
			}
		}

		/// <summary> Gets the list of modules and incoming folders for the builder </summary>
		/// <param name="IncludeDisabled"> Flag indicates whether all the disabled modules should be returned </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataSet with all the data necessary for the Builder, including file destination information,
		/// general settings, server information</returns>
		/// <remarks> This calls the 'SobekCM_Builder_Get_Settings' stored procedure </remarks> 
		public static DataSet Get_Builder_Settings( bool IncludeDisabled, Custom_Tracer Tracer  )
		{
		    Last_Exception = null;

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[1];
				parameters[0] = new EalDbParameter("@include_disabled", IncludeDisabled);
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Builder_Get_Settings", parameters);
				return tempSet;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				return null;
			}
		}

        #region Methods that support the builder integration into the UI (engine)

        /// <summary> Gets the matching builder logs, including a possible restriction by date range or bibid/vid </summary>
        /// <param name="StartDate"> Possibly the starting date for the log range </param>
        /// <param name="EndDate"> Possibly the ending date for the log range </param>
        /// <param name="BibVidFilter"> Any search filter to see only particular BibID or BibID/VID</param>
        /// <param name="IncludeNoWorkFlag"> Flag indicates if 'No Work' entries should be included</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Dataset with all the builder logs </returns>
        public static DataSet Builder_Log_Search(DateTime? StartDate, DateTime? EndDate, string BibVidFilter, bool IncludeNoWorkFlag, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.Builder_Log_Search", "Pulling builder logs for filter specified");
            }

            try
            {
                EalDbParameter[] parameters = new EalDbParameter[4];
                if (StartDate.HasValue)
                    parameters[0] = new EalDbParameter("@startdate", StartDate.Value);
                else
                    parameters[0] = new EalDbParameter("@startdate", DBNull.Value);

                if (EndDate.HasValue)
                    parameters[1] = new EalDbParameter("@enddate", EndDate.Value);
                else
                    parameters[1] = new EalDbParameter("@enddate", DBNull.Value);

                parameters[2] = new EalDbParameter("@bibid_vid", BibVidFilter);
                parameters[3] = new EalDbParameter("@include_no_work_entries", IncludeNoWorkFlag);
                DataSet tempSet = EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Builder_Log_Search", parameters);
                return tempSet;
            }
            catch (Exception ee)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.Builder_Log_Search", "Error during execution: " + ee.Message);
                }
                Last_Exception = ee;
                return null;
            }
        }

        /// <summary> Gets the most recent updates for the builder including the builder settings and scheduled tasks </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Dataset with all the builder updates </returns>
        public static DataSet Builder_Get_Recent_Updates(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.Builder_Get_Recent_Updates", "Pulling the latest builder information");
            }

            try
            {
                DataSet tempSet = EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Builder_Get_Latest_Update");
                return tempSet;
            }
            catch (Exception ee)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.Builder_Get_Recent_Updates", "Error during execution: " + ee.Message);
                }
                Last_Exception = ee;
                return null;
            }
        }

        /// <summary> Gets the most recent updates for the builder including the builder settings and scheduled tasks </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Dataset with all the builder updates </returns>
        public static List<Builder_Module_Set_Info> Builder_Get_Folder_Module_Sets(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.Builder_Get_Folder_Module_Sets", "Pulling the information about the current folder module sets");
            }

            try
            {
                // Pull the data
                DataSet tempSet = EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Builder_Get_Folder_Module_Sets");
                
                // Build the module sets to return
                List<Builder_Module_Set_Info> returnvalue = new List<Builder_Module_Set_Info>();
                foreach (DataRow thisRow in tempSet.Tables[0].Rows)
                {
                    Builder_Module_Set_Info thisModule = new Builder_Module_Set_Info
                    {
                        SetID = Int32.Parse(thisRow["ModuleSetID"].ToString()), 
                        SetName = thisRow["SetName"].ToString(), 
                        Used_Count = Int32.Parse(thisRow["UsedCount"].ToString())
                    };

                    returnvalue.Add(thisModule);
                }

                return returnvalue;
            }
            catch (Exception ee)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.Builder_Get_Folder_Module_Sets", "Error during execution: " + ee.Message);
                }
                Last_Exception = ee;
                return null;
            }
        }

        /// <summary> Gets the latest information about a builder source folder, by primary key </summary>
        /// <param name="FolderID"> Primary key for the builder incoming folder to retrieve </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built information about a single builder source folder </returns>
        /// <remarks> This calls the 'SobekCM_Builder_Get_Incoming_Folder' stored procedure </remarks> 
        public static Builder_Source_Folder Builder_Get_Incoming_Folder(int FolderID, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.Builder_Get_Incoming_Folder", "Get latest information for a builder incoming folder");
            }

            try
            {
                EalDbParameter[] parameters = new EalDbParameter[1];
                parameters[0] = new EalDbParameter("@FolderId", FolderID);

                DataSet resultSet = EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Builder_Get_Incoming_Folder", parameters);

                if ((resultSet == null) || (resultSet.Tables.Count == 0) || (resultSet.Tables[0].Rows.Count == 0))
                    return null;

                DataRow thisRow = resultSet.Tables[0].Rows[0];
                Builder_Source_Folder newFolder = new Builder_Source_Folder
                {
                    IncomingFolderID = Convert.ToInt32(thisRow["IncomingFolderId"]),
                    Folder_Name = thisRow["FolderName"].ToString(),
                    Inbound_Folder = thisRow["NetworkFolder"].ToString(),
                    Failures_Folder = thisRow["ErrorFolder"].ToString(),
                    Processing_Folder = thisRow["ProcessingFolder"].ToString(),
                    Perform_Checksum = Convert.ToBoolean(thisRow["Perform_Checksum_Validation"]),
                    Archive_TIFFs = Convert.ToBoolean(thisRow["Archive_TIFF"]),
                    Archive_All_Files = Convert.ToBoolean(thisRow["Archive_All_Files"]),
                    Allow_Deletes = Convert.ToBoolean(thisRow["Allow_Deletes"]),
                    Allow_Folders_No_Metadata = Convert.ToBoolean(thisRow["Allow_Folders_No_Metadata"]),
                    Allow_Metadata_Updates = Convert.ToBoolean(thisRow["Allow_Metadata_Updates"]),
                    BibID_Roots_Restrictions = thisRow["BibID_Roots_Restrictions"].ToString(),
                    Builder_Module_Set = { SetID = Convert.ToInt32(thisRow["ModuleSetID"]), SetName = thisRow["SetName"].ToString() }
                };

                return newFolder;
                
            }
            catch (Exception ee)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.Builder_Get_Incoming_Folder", "Error during execution: " + ee.Message);
                }
                Last_Exception = ee;
                return null;
            }
        }

        /// <summary> Deletes an existing builder incoming folder from the table </summary>
        /// <param name="FolderID"> Primary key for the builder incoming folder to delete </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Builder_Incoming_Folder_Delete' stored procedure </remarks> 
        public static bool Builder_Delete_Incoming_Folder(int FolderID, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.Builder_Delete_Incoming_Folder", "Processing delete for an incoming folder");
            }

            try
            {
                EalDbParameter[] parameters = new EalDbParameter[1];
                parameters[0] = new EalDbParameter("@IncomingFolderId", FolderID);

                EalDbAccess.ExecuteNonQuery(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Builder_Incoming_Folder_Delete", parameters);

                return true;
            }
            catch (Exception ee)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.Builder_Delete_Incoming_Folder", "Error during execution: " + ee.Message);
                }
                Last_Exception = ee;
                return false;
            }
        }

        /// <summary> Edits an existing builder incoming folder or adds a new folder </summary>
        /// <param name="FolderID"> Primary key for the builder incoming folder to edit </param>
        /// <param name="Folder_Name"> Name of this folder (human friendly) </param>
        /// <param name="Network_Folder"> Directory for this incoming folder on the network </param>
        /// <param name="Error_Folder"> Directory for the error folder for this incoming folder on the network  </param>
        /// <param name="Processing_Folder"> Directory for the processing folder for this incoming folder on the network </param>
        /// <param name="Perform_Checksum"> Flag indicates if a checksum check should occur on incoming resources </param>
        /// <param name="Archive_TIFF"> Flag indicates if TIFF files should be archived </param>
        /// <param name="Archive_All_Files"> Flag indicates if all files should be archived  </param>
        /// <param name="Allow_Deletes"> Flag indicates if DELETE METS can be processed from this incoming folder </param>
        /// <param name="Allow_Folders_No_Metadata"> Flag indicates if folders without any metadata should be processed from this incoming folder </param>
        /// <param name="BibID_Roots_Restrictions"> Any restrictions on the incoming BibIDs that can be processed from this incoming folder </param>
        /// <param name="ModuleSetID"> Primary key to the builder module set for this folder </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Builder_Incoming_Folder_Edit' stored procedure </remarks> 
        public static bool Builder_Edit_Incoming_Folder(int FolderID, string Folder_Name, string Network_Folder, string Error_Folder, string Processing_Folder,
            bool Perform_Checksum, bool Archive_TIFF, bool Archive_All_Files, bool Allow_Deletes, bool Allow_Folders_No_Metadata, 
            string BibID_Roots_Restrictions, int ModuleSetID, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.Builder_Edit_Incoming_Folder", "Processing edit for an incoming folder");
            }

            try
            {
                EalDbParameter[] parameters = new EalDbParameter[13];
                parameters[0] = new EalDbParameter("@IncomingFolderId", FolderID);
                parameters[1] = new EalDbParameter("@NetworkFolder", Network_Folder);
                parameters[2] = new EalDbParameter("@ErrorFolder", Error_Folder);
                parameters[3] = new EalDbParameter("@ProcessingFolder", Processing_Folder);
                parameters[4] = new EalDbParameter("@Perform_Checksum_Validation", Perform_Checksum);
                parameters[5] = new EalDbParameter("@Archive_TIFF", Archive_TIFF);
                parameters[6] = new EalDbParameter("@Archive_All_Files", Archive_All_Files);
                parameters[7] = new EalDbParameter("@Allow_Deletes", Allow_Deletes);
                parameters[8] = new EalDbParameter("@Allow_Folders_No_Metadata", Allow_Folders_No_Metadata);
                parameters[9] = new EalDbParameter("@FolderName", Folder_Name);
                parameters[10] = new EalDbParameter("@BibID_Roots_Restrictions", BibID_Roots_Restrictions);
                parameters[11] = new EalDbParameter("@ModuleSetID", ModuleSetID);
                parameters[12] = new EalDbParameter("@NewID", -1) { Direction = ParameterDirection.InputOutput };

                EalDbAccess.ExecuteNonQuery(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Builder_Incoming_Folder_Edit", parameters);

                return true;
            }
            catch (Exception ee)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.Builder_Edit_Incoming_Folder", "Error during execution: " + ee.Message);
                }
                Last_Exception = ee;
                return false;
            }
        }



        #endregion

 

		/// <summary> Gets the simple list of items for a single item aggregation, or the list of all items in the library </summary>
		/// <param name="AggregationCode"> Code for the item aggregation of interest, or an empty string</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> Dataset with the simple list of items, including BibID, VID, Title, CreateDate, and Resource Link </returns>
		/// <remarks> This calls the 'SobekCM_Simple_Item_List' stored procedure </remarks> 
		public static DataSet Simple_Item_List(string AggregationCode, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				if (AggregationCode.Length == 0)
					Tracer.Add_Trace("Engine_Database.Simple_Item_List", "Pulling simple item list for all items");
				else
					Tracer.Add_Trace("Engine_Database.Simple_Item_List", "Pulling simple item list for '" + AggregationCode + "'");
			}

			// Define a temporary dataset
			EalDbParameter[] parameters = new EalDbParameter[1];
			parameters[0] = new EalDbParameter("@collection_code", AggregationCode);
			DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Simple_Item_List", parameters);
			return tempSet;
		}

		/// <summary> Marks an item as been editing online through the web interface </summary>
		/// <param name="ItemID"> Primary key for the item having a progress/worklog entry added </param>
		/// <param name="User">User name who did the edit</param>
		/// <param name="UserNotes">Any user notes about this edit</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'Tracking_Online_Edit_Complete' stored procedure. </remarks>
		public static bool Tracking_Online_Edit_Complete(int ItemID, string User, string UserNotes)
		{
			try
			{
				// Build the parameter list
				EalDbParameter[] paramList = new EalDbParameter[3];
				paramList[0] = new EalDbParameter("@itemid", ItemID);
				paramList[1] = new EalDbParameter("@user", User);
				paramList[2] = new EalDbParameter("@usernotes", UserNotes);

				// Execute this non-query stored procedure
				EalDbAccess.ExecuteNonQuery( DatabaseType, Connection_String, CommandType.StoredProcedure, "Tracking_Online_Edit_Complete", paramList);

				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				return false;
			}
		}

		/// <summary> Gets a random item from the database </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> A tuple with a random BibID/VID as the return </returns>
		/// <remarks> This calls the 'SobekCM_Random_Item' stored procedure </remarks> 
		public static Tuple<string,string> Get_Random_Item(Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_Random_Item", "Get random item");
			}

			// Define a temporary dataset
			DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Random_Item");

			if ((tempSet != null) && (tempSet.Tables.Count > 0) && (tempSet.Tables[0].Rows.Count > 0))
			{
				string bibid = tempSet.Tables[0].Rows[0]["BibID"].ToString();
				string vid = tempSet.Tables[0].Rows[0]["VID"].ToString();

				return new Tuple<string, string>(bibid, vid);
			}

			return null;

		}

		#region Methods to support the collection of usage statitics from the IIS logs

		/// <summary> Gets all the tables ued during the process of reading the statistics 
		/// from the web iis logs and creating the associated commands  </summary>
		/// <returns> Large dataset with several tables ( all items, all titles, aggregationPermissions, etc.. )</returns>
		public static DataSet Get_Statistics_Lookup_Tables()
		{
			return EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Statistics_Lookup_Tables");
		}

		/// <summary> Save the top-level usage statistics for this instance for a single month </summary>
		/// <param name="Year"> Year of this usage </param>
		/// <param name="Month"> Month of this usage </param>
		/// <param name="Hits"> Number of hits at the instance level </param>
		/// <param name="Sessions"> Number of sessions at the instance level </param>
		/// <param name="RobotHits"> Number of robot hits at the instance level </param>
		/// <param name="XmlHits"> Number of XML hits at the instance level </param>
		/// <param name="OaiHits"> Number of OAI-PMH hits at the instance level </param>
		/// <param name="JsonHits"> Number of JSON hits at the instance level </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> TRUE if successfully logged, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Statistics_Save_TopLevel' stored procedure </remarks> 
		public static bool Save_TopLevel_Statistics(int Year, int Month, int Hits, int Sessions, int RobotHits, int XmlHits, int OaiHits, int JsonHits, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Save_TopLevel_Statistics", "");
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[8];
				parameters[0] = new EalDbParameter("@year", Year);
				parameters[1] = new EalDbParameter("@month", Month);
				parameters[2] = new EalDbParameter("@hits", Hits);
				parameters[3] = new EalDbParameter("@sessions", Sessions);
				parameters[4] = new EalDbParameter("@robot_hits", RobotHits);
				parameters[5] = new EalDbParameter("@xml_hits", XmlHits);
				parameters[6] = new EalDbParameter("@oai_hits", OaiHits);
				parameters[7] = new EalDbParameter("@json_hits", JsonHits);
	  
				// Define a temporary dataset
				EalDbAccess.ExecuteNonQuery( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Statistics_Save_TopLevel", parameters);

				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Save_TopLevel_Statistics", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Save_TopLevel_Statistics", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Save_TopLevel_Statistics", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Save usage statistics for top-level web content pages </summary>
		/// <param name="Year"> Year of this usage </param>
		/// <param name="Month"> Month of this usage </param>
		/// <param name="Hits"> Number of hits on this page </param>
		/// <param name="HitsComplete"> Number of hits on this page and all child pages </param>
		/// <param name="Level1"> Level 1 of the URL for this web content page </param>
		/// <param name="Level2"> Level 2 of the URL for this web content page </param>
		/// <param name="Level3"> Level 3 of the URL for this web content page </param>
		/// <param name="Level4"> Level 4 of the URL for this web content page </param>
		/// <param name="Level5"> Level 5 of the URL for this web content page </param>
		/// <param name="Level6"> Level 6 of the URL for this web content page </param>
		/// <param name="Level7"> Level 7 of the URL for this web content page </param>
		/// <param name="Level8"> Level 8 of the URL for this web content page </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> TRUE if successfully logged, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Statistics_Save_WebContent' stored procedure </remarks> 
		public static bool Save_WebContent_Statistics(int Year, int Month, int Hits, int HitsComplete, string Level1, string Level2, string Level3, string Level4, string Level5, string Level6, string Level7, string Level8, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Save_WebContent_Statistics", "");
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[12];
				parameters[0] = new EalDbParameter("@year", Year);
				parameters[1] = new EalDbParameter("@month", Month);
				parameters[2] = new EalDbParameter("@hits", Hits);
				parameters[3] = new EalDbParameter("@hits_complete", HitsComplete);
				parameters[4] = new EalDbParameter("@level1", Level1);
				parameters[5] = new EalDbParameter("@level2", Level2);
				parameters[6] = new EalDbParameter("@level3", Level3);
				parameters[7] = new EalDbParameter("@level4", Level4);
				parameters[8] = new EalDbParameter("@level5", Level5);
				parameters[9] = new EalDbParameter("@level6", Level6);
				parameters[10] = new EalDbParameter("@level7", Level7);
				parameters[11] = new EalDbParameter("@level8", Level8);
	  
				// Define a temporary dataset
				EalDbAccess.ExecuteNonQuery( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Statistics_Save_WebContent", parameters);

				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Save_WebContent_Statistics", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Save_WebContent_Statistics", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Save_WebContent_Statistics", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}


		/// <summary> Save the usage statistics for a single URL portal </summary>
		/// <param name="PortalID"> Primary key for this URL portal </param>
		/// <param name="Year"> Year of this usage </param>
		/// <param name="Month"> Month of this usage </param>
		/// <param name="Hits"> Total number of hits for this month </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> TRUE if successfully logged, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Statistics_Save_Portal' stored procedure </remarks> 
		public static bool Save_Portal_Statistics(int PortalID, int Year, int Month, int Hits, Custom_Tracer Tracer )
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Save_Portal_Statistics", "");
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[4];
				parameters[0] = new EalDbParameter("@year", Year);
				parameters[1] = new EalDbParameter("@month", Month);
				parameters[2] = new EalDbParameter("@hits", Hits);
				parameters[3] = new EalDbParameter("@portalid", PortalID);
				
				// Define a temporary dataset
				EalDbAccess.ExecuteNonQuery( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Statistics_Save_Portal", parameters);

				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Save_Portal_Statistics", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Save_Portal_Statistics", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Save_Portal_Statistics", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Save usage statistics for the aggregation-level web pages </summary>
		/// <param name="AggregationID"> Primary key for this aggregation </param>
		/// <param name="Year"> Year of this usage </param>
		/// <param name="Month"> Month of this usage </param>
		/// <param name="Hits"> Number of hits against this aggregation </param>
		/// <param name="Sessions"> Number of sessions which used this aggregation </param>
		/// <param name="HomePageViews"> Number of home page views </param>
		/// <param name="BrowseViews"> Number of times users looked at browses under this aggregation </param>
		/// <param name="AdvancedSearchViews"> Number of times users used the advanced search option </param>
		/// <param name="SearchResultViews"> Number of times users viewed search results under this aggregation </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> TRUE if successfully logged, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Statistics_Save_Aggregation' stored procedure </remarks> 
		public static bool Save_Aggregation_Statistics(int AggregationID, int Year, int Month, int Hits, 
			int Sessions, int HomePageViews, int BrowseViews, int AdvancedSearchViews, int SearchResultViews, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Save_Aggregation_Statistics", "");
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[9];
				parameters[0] = new EalDbParameter("@aggregationid", AggregationID);
				parameters[1] = new EalDbParameter("@year", Year);
				parameters[2] = new EalDbParameter("@month", Month);
				parameters[3] = new EalDbParameter("@hits", Hits);
				parameters[4] = new EalDbParameter("@sessions", Sessions);
				parameters[5] = new EalDbParameter("@home_page_views", HomePageViews);
				parameters[6] = new EalDbParameter("@browse_views", BrowseViews);
				parameters[7] = new EalDbParameter("@advanced_search_views", AdvancedSearchViews);
				parameters[8] = new EalDbParameter("@search_results_views", SearchResultViews);

				// Define a temporary dataset
				EalDbAccess.ExecuteNonQuery( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Statistics_Save_Aggregation", parameters);

				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Save_Aggregation_Statistics", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Save_Aggregation_Statistics", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Save_Aggregation_Statistics", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Save usage statistics at the item level (BibID) level </summary>
		/// <param name="GroupID"> Primary key for item group from the database </param>
		/// <param name="Year"> Year of this usage </param>
		/// <param name="Month"> Month of this usage </param>
		/// <param name="Hits"> Number of hits against the item group (and not a child item/vid) </param>
		/// <param name="Sessions"> Number of sessions that looked at this item group at the item group level </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> TRUE if successfully logged, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Statistics_Save_Item_Group' stored procedure </remarks> 
		public static bool Save_Item_Group_Statistics(int GroupID, int Year, int Month, int Hits, int Sessions, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Save_Item_Group_Statistics", "");
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[5];
				parameters[0] = new EalDbParameter("@year", Year);
				parameters[1] = new EalDbParameter("@month", Month);
				parameters[2] = new EalDbParameter("@hits", Hits);
				parameters[3] = new EalDbParameter("@sessions", Sessions);
				parameters[4] = new EalDbParameter("@groupid", GroupID);

				// Define a temporary dataset
				EalDbAccess.ExecuteNonQuery( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Statistics_Save_Item_Group", parameters);

				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Save_Item_Group_Statistics", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Save_Item_Group_Statistics", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Save_Item_Group_Statistics", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Save the item-level usage statistics </summary>
		/// <param name="ItemID"> Primary key for the digital resource from the database </param>
		/// <param name="Year"> Year of this usage </param>
		/// <param name="Month"> Month of this usage </param>
		/// <param name="Hits"> Number of hits against this digital resource </param>
		/// <param name="Sessions"> Number of sessions that used this digital resource </param>
		/// <param name="JpegViews"> Number of JPEG views </param>
		/// <param name="ZoomableViews"> Number of zoomable JPEG2000 views </param>
		/// <param name="CitationViews"> Number of citation views </param>
		/// <param name="ThumbnailViews"> Number of thumbnail views </param>
		/// <param name="TextSearchViews"> Number of text search views </param>
		/// <param name="FlashViews"> Number of flash views </param>
		/// <param name="GoogleMapViews"> Number of google map views </param>
		/// <param name="DownloadViews"> Number of download views </param>
		/// <param name="StaticViews"> Number of static views </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> TRUE if successfully logged, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Statistics_Save_Item' stored procedure </remarks> 
		public static bool Save_Item_Statistics(int ItemID, int Year, int Month, int Hits, int Sessions, int JpegViews, int ZoomableViews,
			int CitationViews, int ThumbnailViews, int TextSearchViews, int FlashViews, int GoogleMapViews, int DownloadViews, 
			int StaticViews, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Save_Item_Statistics", "");
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[14];
				parameters[0] = new EalDbParameter("@year", Year);
				parameters[1] = new EalDbParameter("@month", Month);
				parameters[2] = new EalDbParameter("@hits", Hits);
				parameters[3] = new EalDbParameter("@sessions", Sessions);
				parameters[4] = new EalDbParameter("@itemid", ItemID);
				parameters[5] = new EalDbParameter("@jpeg_views", JpegViews);
				parameters[6] = new EalDbParameter("@zoomable_views", ZoomableViews);
				parameters[7] = new EalDbParameter("@citation_views", CitationViews);
				parameters[8] = new EalDbParameter("@thumbnail_views", ThumbnailViews);
				parameters[9] = new EalDbParameter("@text_search_views", TextSearchViews);
				parameters[10] = new EalDbParameter("@flash_views", FlashViews);
				parameters[11] = new EalDbParameter("@google_map_views", GoogleMapViews);
				parameters[12] = new EalDbParameter("@download_views", DownloadViews);
				parameters[13] = new EalDbParameter("@static_views", StaticViews);

				// Define a temporary dataset
				EalDbAccess.ExecuteNonQuery( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Statistics_Save_Item", parameters);

				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Save_Item_Statistics", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Save_Item_Statistics", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Save_Item_Statistics", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Aggregate all the item-level and item group-level hits up the hierarchy to the aggregations </summary>
		/// <param name="Year"> Year of this usage </param>
		/// <param name="Month"> Month of this usage </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> TRUE if successfully aggregated, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Statistics_Aggregate' stored procedure </remarks> 
		public static string Aggregate_Statistics(int Year, int Month, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Aggregate_Statistics", "");
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[3];
				parameters[0] = new EalDbParameter("@statyear", Year);
				parameters[1] = new EalDbParameter("@statmonth", Month);
				EalDbParameter returnMsg = parameters[2] = new EalDbParameter("@message", Month);
				returnMsg.Direction = ParameterDirection.InputOutput;

				// Define a temporary dataset
				EalDbAccess.ExecuteNonQuery( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Statistics_Aggregate", parameters);

				return returnMsg.Value.ToString();
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Aggregate_Statistics", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Aggregate_Statistics", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Aggregate_Statistics", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return "Exception caught";
			}
		}

		/// <summary> Gets the list of all users that are linked to items which may have usage statistics  </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataTable of all the users linked to items </returns>
		/// <remarks> This calls the 'SobekCM_Stats_Get_Users_Linked_To_Items' stored procedure </remarks>
		public static DataTable Get_Users_Linked_To_Items(Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_Users_Linked_To_Items", "Pulling from database");
			}

			try
			{
				// Define a temporary dataset
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Stats_Get_Users_Linked_To_Items");

				// If there was no data for this collection and entry point, return null (an ERROR occurred)
				if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
				{
					return null;
				}

				// Return the first table from the returned dataset
				return tempSet.Tables[0];
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Get_Users_Linked_To_Items", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Users_Linked_To_Items", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_Users_Linked_To_Items", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}


		/// <summary> Gets the basic usage statistics for all items linked to a user </summary>
		/// <param name="UserID"> Primary key for the user of interest, for which to pull the item usage stats </param>
		/// <param name="Month"> Month for which to pull the usage information </param>
		/// <param name="Year"> Year for which to pull the usage information </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataTable of the basic usage statistics for all items linked to a user for a single month and year </returns>
		/// <remarks> This calls the 'SobekCM_Stats_Get_User_Linked_Items_Stats' stored procedure </remarks>
		public static DataTable Get_User_Linked_Items_Stats(int UserID, int Month, int Year, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.Get_User_Linked_Items_Stats", "Pulling from database");
			}

			try
			{
				// Build the parameter list
				EalDbParameter[] paramList = new EalDbParameter[3];
				paramList[0] = new EalDbParameter("@userid", UserID);
				paramList[1] = new EalDbParameter("@month", Month);
				paramList[2] = new EalDbParameter("@year", Year);

				// Define a temporary dataset
				DataSet tempSet = EalDbAccess.ExecuteDataset( DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_Stats_Get_User_Linked_Items_Stats", paramList);

				// If there was no data for this collection and entry point, return null (an ERROR occurred)
				if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
				{
					return null;
				}

				// Return the first table from the returned dataset
				return tempSet.Tables[0];
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.Get_User_Linked_Items_Stats", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_User_Linked_Items_Stats", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.Get_User_Linked_Items_Stats", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		#endregion

		#region Methods to support the web content pages

		/// <summary> Add a new web content page </summary>
		/// <param name="Level1"> Level 1 of the URL for this web content page </param>
		/// <param name="Level2"> Level 2 of the URL for this web content page </param>
		/// <param name="Level3"> Level 3 of the URL for this web content page </param>
		/// <param name="Level4"> Level 4 of the URL for this web content page </param>
		/// <param name="Level5"> Level 5 of the URL for this web content page </param>
		/// <param name="Level6"> Level 6 of the URL for this web content page </param>
		/// <param name="Level7"> Level 7 of the URL for this web content page </param>
		/// <param name="Level8"> Level 8 of the URL for this web content page </param>
		/// <param name="Username"> Name of the user that performed this ADD (or restored of a previously deleted page)</param>
		/// <param name="Title"> Title for this new web page </param>
		/// <param name="Summary"> Summary for this new web page </param>
		/// <param name="Redirect"> If this is actually a redirect URL, this will be the URL that it should resolve to </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> Primary key for this new web content page ( or -1 if ERROR ) </returns>
		/// <remarks> This calls the 'SobekCM_WebContent_Add' stored procedure </remarks> 
		public static int WebContent_Add_Page(string Level1, string Level2, string Level3, string Level4, string Level5, string Level6, string Level7, string Level8, string Username, string Title, string Summary, string Redirect, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.WebContent_Add_Page", "");
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[13];
				parameters[0] = new EalDbParameter("@level1", Level1);
				parameters[1] = new EalDbParameter("@level2", Level2);
				parameters[2] = new EalDbParameter("@level3", Level3);
				parameters[3] = new EalDbParameter("@level4", Level4);
				parameters[4] = new EalDbParameter("@level5", Level5);
				parameters[5] = new EalDbParameter("@level6", Level6);
				parameters[6] = new EalDbParameter("@level7", Level7);
				parameters[7] = new EalDbParameter("@level8", Level8);
				parameters[8] = new EalDbParameter("@username", Username);
				parameters[9] = new EalDbParameter("@title", Title);
				parameters[10] = new EalDbParameter("@summary", Summary);
                parameters[11] = new EalDbParameter("@redirect", Redirect);
				parameters[12] = new EalDbParameter("@WebContentID", -1) { Direction = ParameterDirection.InputOutput };

			    // Define a temporary dataset
				EalDbAccess.ExecuteNonQuery(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_WebContent_Add", parameters);

				// Get the new primary key and return it
				return Int32.Parse(parameters[12].Value.ToString());
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.WebContent_Add_Page", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.WebContent_Add_Page", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.WebContent_Add_Page", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return -1;
			}
		}

        /// <summary> Edit an existing web content page </summary>
        /// <param name="WebContentID"> Primary key to the existing web content page </param>
        /// <param name="Title"> New title for this web page </param>
        /// <param name="Summary"> New summary for this new web page </param>
        /// <param name="Redirect"> If this is actually a redirect URL, this will be the URL that it should resolve to </param>
        /// <param name="User"> User who edited this page or redirect </param>
        /// <param name="MilestoneText"> Specific text for the milestone </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_WebContent_Edit' stored procedure </remarks> 
        public static bool WebContent_Edit_Page(int WebContentID, string Title, string Summary, string Redirect, string User, string MilestoneText, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.WebContent_Edit_Page", "");
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[6];
				parameters[0] = new EalDbParameter("@WebContentID", WebContentID);
                parameters[1] = new EalDbParameter("@UserName", User);
				parameters[2] = new EalDbParameter("@Title", Title);
				parameters[3] = new EalDbParameter("@Summary", Summary);
                parameters[4] = new EalDbParameter("@Redirect", Redirect);
                parameters[5] = new EalDbParameter("@MilestoneText", MilestoneText);

				// Define a temporary dataset
				EalDbAccess.ExecuteNonQuery(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_WebContent_Edit", parameters);

				// Get the new primary key and return it
				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.WebContent_Edit_Page", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.WebContent_Edit_Page", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.WebContent_Edit_Page", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

        /// <summary> Gets the basic information about a web content page, by primary key </summary>
        /// <param name="WebContentID"> Primary key for this web content page in the database </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Built web content basic info object, or NULL if not found at all </returns>
        /// <remarks> This calls the 'SobekCM_WebContent_Get_Page_ID' stored procedure </remarks> 
        public static WebContent_Basic_Info WebContent_Get_Page(int WebContentID, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.WebContent_Get_Page (by id)", "");
            }

            try
            {
                EalDbParameter[] parameters = new EalDbParameter[1];
                parameters[0] = new EalDbParameter("@WebContentID", WebContentID);

                // Define a temporary dataset
                DataSet value = EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_WebContent_Get_Page_ID", parameters);

                // If nothing was returned, return NULL
                if ((value.Tables.Count == 0) || (value.Tables[0].Rows.Count == 0))
                    return null;

                // Get the values from the returned object
                DataRow pageRow = value.Tables[0].Rows[0];
                int webid = Int32.Parse(pageRow["WebContentID"].ToString());
                string title = pageRow["Title"].ToString();
                string summary = pageRow["Summary"].ToString();
                bool deleted = bool.Parse(pageRow["Deleted"].ToString());
                string redirect = pageRow["Redirect"].ToString();

                // Build and return the basic info object
                WebContent_Basic_Info returnValue = new WebContent_Basic_Info(webid, title, summary, deleted, redirect);
                if (bool.Parse(pageRow["Locked"].ToString()))
                    returnValue.Locked = true;

                // Also, add the levels
                if ((pageRow["Level1"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level1"].ToString())))
                {
                    returnValue.Level1 = pageRow["Level1"].ToString();
                    if ((pageRow["Level2"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level2"].ToString())))
                    {
                        returnValue.Level2 = pageRow["Level2"].ToString();
                        if ((pageRow["Level3"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level3"].ToString())))
                        {
                            returnValue.Level3 = pageRow["Level3"].ToString();
                            if ((pageRow["Level4"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level4"].ToString())))
                            {
                                returnValue.Level4 = pageRow["Level4"].ToString();
                                if ((pageRow["Level5"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level5"].ToString())))
                                {
                                    returnValue.Level5 = pageRow["Level5"].ToString();
                                    if ((pageRow["Level6"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level6"].ToString())))
                                    {
                                        returnValue.Level6 = pageRow["Level6"].ToString();
                                        if ((pageRow["Level7"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level7"].ToString())))
                                        {
                                            returnValue.Level7 = pageRow["Level7"].ToString();
                                            if ((pageRow["Level8"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level8"].ToString())))
                                                returnValue.Level8 = pageRow["Level8"].ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return returnValue;

            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_Page (by id)", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_Page (by id)", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_Page (by id)", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

		/// <summary> Gets the basic information about a web content page, by the URL segments </summary>
		/// <param name="Level1"> Level 1 of the URL for this web content page </param>
		/// <param name="Level2"> Level 2 of the URL for this web content page </param>
		/// <param name="Level3"> Level 3 of the URL for this web content page </param>
		/// <param name="Level4"> Level 4 of the URL for this web content page </param>
		/// <param name="Level5"> Level 5 of the URL for this web content page </param>
		/// <param name="Level6"> Level 6 of the URL for this web content page </param>
		/// <param name="Level7"> Level 7 of the URL for this web content page </param>
		/// <param name="Level8"> Level 8 of the URL for this web content page </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> Built web content basic info object, or NULL if not found at all </returns>
		/// <remarks> This calls the 'SobekCM_WebContent_Get_Page' stored procedure </remarks> 
		public static WebContent_Basic_Info WebContent_Get_Page(string Level1, string Level2, string Level3, string Level4, string Level5, string Level6, string Level7, string Level8, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.WebContent_Get_Page", "");
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[8];
				parameters[0] = new EalDbParameter("@level1", Level1);
				parameters[1] = new EalDbParameter("@level2", Level2);
				parameters[2] = new EalDbParameter("@level3", Level3);
				parameters[3] = new EalDbParameter("@level4", Level4);
				parameters[4] = new EalDbParameter("@level5", Level5);
				parameters[5] = new EalDbParameter("@level6", Level6);
				parameters[6] = new EalDbParameter("@level7", Level7);
				parameters[7] = new EalDbParameter("@level8", Level8);

				// Define a temporary dataset
				DataSet value = EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_WebContent_Get_Page", parameters);

				// If nothing was returned, return NULL
				if ((value.Tables.Count == 0) || (value.Tables[0].Rows.Count == 0))
					return null;

				// Get the values from the returned object
				DataRow pageRow = value.Tables[0].Rows[0];
				int webid = Int32.Parse(pageRow["WebContentID"].ToString());
				string title = pageRow["Title"].ToString();
				string summary = pageRow["Summary"].ToString();
				bool deleted = bool.Parse(pageRow["Deleted"].ToString());
			    string redirect = pageRow["Redirect"].ToString();

				// Build and return the basic info object
				WebContent_Basic_Info returnValue = new WebContent_Basic_Info(webid, title, summary, deleted, redirect);
                if (bool.Parse(pageRow["Locked"].ToString()))
                    returnValue.Locked = true;

                // Also, add the levels
			    if ((pageRow["Level1"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level1"].ToString())))
			    {
			        returnValue.Level1 = pageRow["Level1"].ToString();
			        if ((pageRow["Level2"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level2"].ToString())))
			        {
			            returnValue.Level2 = pageRow["Level2"].ToString();
			            if ((pageRow["Level3"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level3"].ToString())))
			            {
			                returnValue.Level3 = pageRow["Level3"].ToString();
			                if ((pageRow["Level4"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level4"].ToString())))
			                {
			                    returnValue.Level4 = pageRow["Level4"].ToString();
			                    if ((pageRow["Level5"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level5"].ToString())))
			                    {
			                        returnValue.Level5 = pageRow["Level5"].ToString();
			                        if ((pageRow["Level6"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level6"].ToString())))
			                        {
			                            returnValue.Level6 = pageRow["Level6"].ToString();
			                            if ((pageRow["Level7"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level7"].ToString())))
			                            {
			                                returnValue.Level7 = pageRow["Level7"].ToString();
                                            if ((pageRow["Level8"] != DBNull.Value) && (!String.IsNullOrEmpty(pageRow["Level8"].ToString()))) 
                                                returnValue.Level8 = pageRow["Level8"].ToString();
			                            }
			                        }
			                    }
			                }
			            }
			        }
			    }

                return returnValue;

			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.WebContent_Get_Page", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.WebContent_Get_Page", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.WebContent_Get_Page", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}


		/// <summary> Add a new milestone to an existing web content page </summary>
		/// <param name="WebContentID"> Primary key to the existing web content page </param>
		/// <param name="Milestone"> Text of the milestone to be added </param>
		/// <param name="MilestoneUser"> User name for the milestone being added </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_WebContent_Add_Milestone' stored procedure </remarks> 
		public static bool WebContent_Add_Milestone(int WebContentID, string Milestone, string MilestoneUser, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.WebContent_Add_Milestone", "");
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[3];
				parameters[0] = new EalDbParameter("@WebContentID", WebContentID);
				parameters[1] = new EalDbParameter("@Milestone", Milestone);
				parameters[2] = new EalDbParameter("@MilestoneUser", MilestoneUser);

				// Define a temporary dataset
				EalDbAccess.ExecuteNonQuery(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_WebContent_Add_Milestone", parameters);

				// Get the new primary key and return it
				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.WebContent_Add_Milestone", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.WebContent_Add_Milestone", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.WebContent_Add_Milestone", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Delete an existing web content page (and mark in the milestones) </summary>
		/// <param name="WebContentID"> Primary key to the existing web content page </param>
		/// <param name="Reason"> Optional reason for the deletion </param>
		/// <param name="MilestoneUser"> User name for the milestone to be being added </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_WebContent_Delete' stored procedure </remarks> 
		public static bool WebContent_Delete_Page(int WebContentID, string Reason, string MilestoneUser, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Engine_Database.WebContent_Delete_Page", "");
			}

			try
			{
				EalDbParameter[] parameters = new EalDbParameter[3];
				parameters[0] = new EalDbParameter("@WebContentID", WebContentID);
				parameters[1] = new EalDbParameter("@Reason", Reason);
				parameters[2] = new EalDbParameter("@MilestoneUser", MilestoneUser);

				// Define a temporary dataset
				EalDbAccess.ExecuteNonQuery(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_WebContent_Delete", parameters);

				// Get the new primary key and return it
				return true;
			}
			catch (Exception ee)
			{
				Last_Exception = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("Engine_Database.WebContent_Delete_Page", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.WebContent_Delete_Page", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Engine_Database.WebContent_Delete_Page", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

        /// <summary> Get the usage statistics for a single web content page </summary>
        /// <param name="WebContentID"> Primary key to the web page in question </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Single web content usage report wrapper around the list of monthly usage hits </returns>
        /// <remarks> This calls the 'SobekCM_WebContent_Get_Usage' stored procedure </remarks> 
        public static Single_WebContent_Usage_Report WebContent_Get_Usage(int WebContentID, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.WebContent_Get_Usage", "Pull single web page usage report from the database");
            }

            // Build return list
            Single_WebContent_Usage_Report returnValue = new Single_WebContent_Usage_Report {WebContentID = WebContentID};

            try
            {
                EalDbParameter[] parameters = new EalDbParameter[1];
                parameters[0] = new EalDbParameter("@WebContentID", WebContentID);

                // Create the database agnostic reader
                EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_WebContent_Get_Usage", parameters );

                // Read through each year/month
                DbDataReader reader = readerWrapper.Reader;
                while (readerWrapper.Reader.Read())
                {
                    // Grab the values out
                    short year = reader.GetInt16(0);
                    short month = reader.GetInt16(1);
                    int hits = reader.GetInt32(2);
                    int hitsComplete = reader.GetInt32(3);

                    // Build the hit object
                    Single_WebContent_Usage hitObject = new Single_WebContent_Usage(year, month, hits, hitsComplete);

                    // Add the hit object to the list
                    returnValue.Usage.Add(hitObject);
                }

                // Close the reader (which also closes the connection)
                readerWrapper.Close();

                // Return the built list
                return returnValue;
            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_Usage", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_Usage", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_Usage", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }


        /// <summary> Get the milestones / history for a single web content page </summary>
        /// <param name="WebContentID"> Primary key to the web page in question </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Single page milestone report wrapper </returns>
        /// <remarks> This calls the 'SobekCM_WebContent_Get_Milestones' stored procedure </remarks> 
        public static Single_WebContent_Change_Report WebContent_Get_Milestones(int WebContentID, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.WebContent_Get_Milestones", "Pull single web page milestones report from the database");
            }

            // Build return list
            Single_WebContent_Change_Report returnValue = new Single_WebContent_Change_Report { WebContentID = WebContentID };

            try
            {
                EalDbParameter[] parameters = new EalDbParameter[1];
                parameters[0] = new EalDbParameter("@WebContentID", WebContentID);

                // Create the database agnostic reader
                EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_WebContent_Get_Milestones", parameters);

                // Read through each milestone
                DbDataReader reader = readerWrapper.Reader;
                while (readerWrapper.Reader.Read())
                {
                    // Grab the values out
                    string milestone = reader.GetString(0);
                    DateTime date = reader.GetDateTime(1);
                    string user = reader.GetString(2);

                    // Build the hit object
                    Milestone_Entry hitObject = new Milestone_Entry(date, user, milestone );

                    // Add the hit object to the list
                    returnValue.Changes.Add(hitObject);
                }

                // Close the reader (which also closes the connection)
                readerWrapper.Close();

                // Return the built list
                return returnValue;
            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_Milestones", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_Milestones", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_Milestones", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

        /// <summary> Get the global milestones of all changes to all top-level static html pages </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> DataSet of milestones </returns>
        /// <remarks> This calls the 'SobekCM_WebContent_Get_Recent_Changes' stored procedure </remarks> 
        public static DataSet WebContent_Get_Recent_Changes(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.WebContent_Get_Recent_Changes", "Pull list of recently changed web pages from the database");
            }

            try
            {
                return EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_WebContent_Get_Recent_Changes");
            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_Recent_Changes", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_Recent_Changes", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_Recent_Changes", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

        /// <summary> Gets the dataset of all global content pages (excluding redirects) </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> DataSet of milestones </returns>
        /// <remarks> This calls the 'SobekCM_WebContent_All_Pages' stored procedure </remarks> 
        public static DataSet WebContent_Get_All_Pages(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.WebContent_Get_All_Pages", "Gets the list of all content pages (excluding redirects)");
            }

            try
            {
                return EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_WebContent_All_Pages");
            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_All_Pages", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_All_Pages", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_All_Pages", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

        /// <summary> Gets the dataset of all global redirects </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> DataSet of redirects from within the web content system </returns>
        /// <remarks> This calls the 'SobekCM_WebContent_All_Redirects' stored procedure </remarks> 
        public static DataSet WebContent_Get_All_Redirects(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.WebContent_Get_All_Redirects", "Gets the list of all global redirects");
            }

            try
            {
                return EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_WebContent_All_Redirects");
            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_All_Redirects", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_All_Redirects", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_All_Redirects", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

        /// <summary> Gets the dataset of all global content pages AND redirects </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> DataSet of pages and redirects </returns>
        /// <remarks> This calls the 'SobekCM_WebContent_All' stored procedure </remarks> 
        public static DataSet WebContent_Get_All(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.WebContent_Get_All", "Gets the list of all content pages and redirects");
            }

            try
            {
                return EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_WebContent_All");
            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_All", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_All", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_All", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

        /// <summary> Gets the hierarchy of all global content pages AND redirects, used for looking for a match from a requested URL </summary>
        /// <param name="ReturnValue"> Web content hierarchy object to populate - should be pre-cleared </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Flag indicating if this ran successfully, TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_WebContent_All_Brief' stored procedure </remarks> 
        public static bool WebContent_Populate_All_Hierarchy(WebContent_Hierarchy ReturnValue, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.WebContent_Populate_All_Hierarchy", "Gets the hierarchy tree of all content pages and redirects");
            }

            try
            {
                // Create the database agnostic reader
                EalDbReaderWrapper readerWrapper = EalDbAccess.ExecuteDataReader(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_WebContent_All_Brief");

                // Get ready to step through all the rows returned from the reader
                WebContent_Hierarchy_Node[] list = new WebContent_Hierarchy_Node[7];

                // Read through each milestone
                DbDataReader reader = readerWrapper.Reader;
                while (readerWrapper.Reader.Read())
                {
                    // Grab the values out
                    string redirect = (!reader.IsDBNull(9)) ? reader.GetString(9) : null;
                    int id = reader.GetInt32(0);
 
                    // Handle the first segment
                    string segment1 = reader.GetString(1);
                    if ((list[0] == null) || (String.Compare(list[0].Segment, segment1, StringComparison.OrdinalIgnoreCase)  != 0))
                    {
                        // Build the node and add to the root nodes
                        WebContent_Hierarchy_Node newNode = new WebContent_Hierarchy_Node(segment1, id, redirect);
                        ReturnValue.Add_Child( newNode );

                        // If there are additional non-null segments, than this node does not represent the 
                        // actual node that corresponds to this web content page or redirect
                        if (!reader.IsDBNull(2))
                            newNode.WebContentID = -1;

                        // Also save in the right spot in the list and clear the next level
                        list[0] = newNode;
                        list[1] = null;
                    }

                    // If there is a second segment here, handle that
                    if (!reader.IsDBNull(2))
                    {
                        string segment2 = reader.GetString(2);

                        // Is this a new segment 2 value?
                        if ((list[1] == null) || (String.Compare(list[1].Segment, segment2, StringComparison.OrdinalIgnoreCase) != 0))
                        {
                            // Build the node and add to the current parent node
                            WebContent_Hierarchy_Node newNode = new WebContent_Hierarchy_Node(segment2, id, redirect);
                            list[0].Add_Child(newNode);

                            // If there are additional non-null segments, than this node does not represent the 
                            // actual node that corresponds to this web content page or redirect
                            if (!reader.IsDBNull(3))
                                newNode.WebContentID = -1;

                            // Also save in the right spot in the list and clear the next level
                            list[1] = newNode;
                            list[2] = null;
                        }

                        // Is there a third segment to add as well
                        if (!reader.IsDBNull(3))
                        {
                            string segment3 = reader.GetString(3);

                            // Is this a new segment 3 value?
                            if ((list[2] == null) || (String.Compare(list[2].Segment, segment3, StringComparison.OrdinalIgnoreCase) != 0))
                            {
                                // Build the node and add to the current parent node
                                WebContent_Hierarchy_Node newNode = new WebContent_Hierarchy_Node(segment3, id, redirect);
                                list[1].Add_Child(newNode);

                                // If there are additional non-null segments, than this node does not represent the 
                                // actual node that corresponds to this web content page or redirect
                                if (!reader.IsDBNull(4))
                                    newNode.WebContentID = -1;

                                // Also save in the right spot in the list and clear the next level
                                list[2] = newNode;
                                list[3] = null;
                            }

                            // Is there a fourth segment to add as well
                            if (!reader.IsDBNull(4))
                            {
                                string segment4 = reader.GetString(4);

                                // Is this a new segment 4 value?
                                if ((list[3] == null) || (String.Compare(list[3].Segment, segment4, StringComparison.OrdinalIgnoreCase) != 0))
                                {
                                    // Build the node and add to the current parent node
                                    WebContent_Hierarchy_Node newNode = new WebContent_Hierarchy_Node(segment4, id, redirect);
                                    list[2].Add_Child(newNode);

                                    // If there are additional non-null segments, than this node does not represent the 
                                    // actual node that corresponds to this web content page or redirect
                                    if (!reader.IsDBNull(5))
                                        newNode.WebContentID = -1;

                                    // Also save in the right spot in the list and clear the next level
                                    list[3] = newNode;
                                    list[4] = null;
                                }

                                // Is there a fifth segment to add as well
                                if (!reader.IsDBNull(5))
                                {
                                    string segment5 = reader.GetString(5);

                                    // Is this a new segment 5 value?
                                    if ((list[4] == null) || (String.Compare(list[4].Segment, segment5, StringComparison.OrdinalIgnoreCase) != 0))
                                    {
                                        // Build the node and add to the current parent node
                                        WebContent_Hierarchy_Node newNode = new WebContent_Hierarchy_Node(segment5, id, redirect);
                                        list[3].Add_Child(newNode);

                                        // If there are additional non-null segments, than this node does not represent the 
                                        // actual node that corresponds to this web content page or redirect
                                        if (!reader.IsDBNull(6))
                                            newNode.WebContentID = -1;

                                        // Also save in the right spot in the list and clear the next level
                                        list[4] = newNode;
                                        list[5] = null;
                                    }

                                    // Is there a sixth segment to add as well
                                    if (!reader.IsDBNull(6))
                                    {
                                        string segment6 = reader.GetString(6);

                                        // Is this a new segment 6 value?
                                        if ((list[5] == null) || (String.Compare(list[5].Segment, segment6, StringComparison.OrdinalIgnoreCase) != 0))
                                        {
                                            // Build the node and add to the current parent node
                                            WebContent_Hierarchy_Node newNode = new WebContent_Hierarchy_Node(segment6, id, redirect);
                                            list[4].Add_Child(newNode);

                                            // If there are additional non-null segments, than this node does not represent the 
                                            // actual node that corresponds to this web content page or redirect
                                            if (!reader.IsDBNull(7))
                                                newNode.WebContentID = -1;

                                            // Also save in the right spot in the list and clear the next level
                                            list[5] = newNode;
                                            list[6] = null;
                                        }

                                        // Is there a seventh segment to add as well
                                        if (!reader.IsDBNull(7))
                                        {
                                            string segment7 = reader.GetString(7);

                                            // Is this a new segment 7 value?
                                            if ((list[6] == null) || (String.Compare(list[6].Segment, segment7, StringComparison.OrdinalIgnoreCase) != 0))
                                            {
                                                // Build the node and add to the current parent node
                                                WebContent_Hierarchy_Node newNode = new WebContent_Hierarchy_Node(segment7, id, redirect);
                                                list[5].Add_Child(newNode);

                                                // If there are additional non-null segments, than this node does not represent the 
                                                // actual node that corresponds to this web content page or redirect
                                                if (!reader.IsDBNull(8))
                                                    newNode.WebContentID = -1;

                                                // Also save in the right spot in the list.. no next level to clear
                                                list[6] = newNode; 
                                            }

                                            // Is there an eigth segment? (Will always be a new one since the bottom of the unique hierarchy list)
                                            if (!reader.IsDBNull(8))
                                            {
                                                string segment8 = reader.GetString(8);

                                                // Build the node and add to the current parent node
                                                WebContent_Hierarchy_Node newNode = new WebContent_Hierarchy_Node(segment8, id, redirect);
                                                list[6].Add_Child(newNode);

                                                // Note, this last node always represents the web content page or redirect if
                                                // it made it this far.  No need to check next level and assign id to -1 if not null
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Close the reader (which also closes the connection)
                readerWrapper.Close();

                // Return the built list
                return true;
            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.WebContent_Populate_All_Hierarchy", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Populate_All_Hierarchy", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Populate_All_Hierarchy", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return false;
            }
        }

        /// <summary> Gets the usage report for all top-level web content pages between two dates </summary> 
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Year1"> Year for the beginning of the range to pull stats for </param>
        /// <param name="Month1"> Month for the beginning of the range to pull stats for </param>
        /// <param name="Year2"> Year for the end of the range to pull stats for </param>
        /// <param name="Month2"> Month for the end of the range to pull stats for </param>
        /// <returns> Dataset of usage on top-level pages between two dates </returns>
        /// <remarks> This calls the 'SobekCM_WebContent_Usage_Report' stored procedure </remarks> 
        public static DataSet WebContent_Get_Usage_Report(int Year1, int Month1, int Year2, int Month2, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.WebContent_Get_Usage_Report", "Pull the stats between " + Month1 + "/" + Year1 + " and " + Month2 + "/" + Year2 );
            }

            try
            {
                EalDbParameter[] parameters = new EalDbParameter[4];
                parameters[0] = new EalDbParameter("@year1", Year1);
                parameters[1] = new EalDbParameter("@month1", Month1);
                parameters[2] = new EalDbParameter("@year2", Year2);
                parameters[3] = new EalDbParameter("@month2", Month2);

                return EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_WebContent_Usage_Report", parameters);
            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_Usage_Report", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_Usage_Report", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Get_Usage_Report", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

        /// <summary> Flag indicates if there is usage logged for web content pages within the system </summary> 
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Dataset of usage on top-level pages between two dates </returns>
        /// <remarks> This calls the 'SobekCM_WebContent_Usage_Report' stored procedure </remarks> 
        public static bool WebContent_Has_Usage( Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.WebContent_Has_Usage", "Pull flag indicating if usage is logged for web content entities (pages or redirects)");
            }

            try
            {
                EalDbParameter[] parameters = new EalDbParameter[1];
                parameters[0] = new EalDbParameter("@value", false) {Direction = ParameterDirection.InputOutput};

                EalDbAccess.ExecuteNonQuery(DatabaseType, Connection_String, CommandType.StoredProcedure, "SobekCM_WebContent_Has_Usage", parameters);

                return bool.Parse(parameters[0].Value.ToString());
            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.WebContent_Has_Usage", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Has_Usage", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.WebContent_Has_Usage", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return false;
            }
        }

        #endregion

        #region Methods to retrieve a user object

        /// <summary> Gets basic user information by UserID </summary>
        /// <param name="UserID"> Primary key for this user in the database </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Fully built <see cref="SobekCM.Core.Users.User_Object"/> object </returns>
        /// <remarks> This calls the 'mySobek_Get_User_By_UserID' stored procedure<br /><br />
        /// This is called when a user's cookie exists in a web request</remarks> 
        public static User_Object Get_User(int UserID, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.Get_User", String.Empty);
            }

            try
            {
                // Execute this non-query stored procedure
                EalDbParameter[] paramList = new EalDbParameter[1];
                paramList[0] = new EalDbParameter("@userid", UserID);

                DataSet resultSet = EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "mySobek_Get_User_By_UserID", paramList);

                if ((resultSet.Tables.Count > 0) && (resultSet.Tables[0].Rows.Count > 0))
                {
                    return build_user_object_from_dataset(resultSet);
                }

                // Return the browse id
                return null;
            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.Get_User", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.Get_User", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.Get_User", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

        /// <summary> Gets basic user information by the Shibboleth-provided user identifier </summary>
        /// <param name="ShibbolethID"> Shibboleth ID (UFID) for the user </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Fully built <see cref="SobekCM.Core.Users.User_Object"/> object </returns>
        /// <remarks> This calls the 'mySobek_Get_User_By_UFID' stored procedure<br /><br />
        /// This method is called when user's logon through the Gatorlink Shibboleth service</remarks> 
        public static User_Object Get_User(string ShibbolethID, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.Get_User", String.Empty);
            }

            try
            {

                // Execute this non-query stored procedure
                EalDbParameter[] paramList = new EalDbParameter[1];
                paramList[0] = new EalDbParameter("@shibbid", ShibbolethID);

                DataSet resultSet = EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "mySobek_Get_User_By_ShibbID", paramList);

                if ((resultSet.Tables.Count > 0) && (resultSet.Tables[0].Rows.Count > 0))
                {
                    return build_user_object_from_dataset(resultSet);
                }

                // Return the browse id
                return null;

            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.Get_User", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.Get_User", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.Get_User", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

        /// <summary> Gets basic user information by Username (or email) and Password </summary>
        /// <param name="UserName"> UserName (or email address) for the user </param>
        /// <param name="Password"> Plain-text password, which is then encrypted prior to sending to database</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Fully built <see cref="SobekCM.Core.Users.User_Object"/> object </returns>
        /// <remarks> This calls the 'mySobek_Get_User_By_UserName_Password' stored procedure<br /><br />
        /// This is used when a user logs on through the mySobek authentication</remarks> 
        public static User_Object Get_User(string UserName, string Password, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Engine_Database.Get_User", String.Empty);
            }

            try
            {
                const string SALT = "This is my salt to add to the password";
                string encryptedPassword = SecurityInfo.SHA1_EncryptString(Password + SALT);


                // Execute this non-query stored procedure
                EalDbParameter[] paramList = new EalDbParameter[2];
                paramList[0] = new EalDbParameter("@username", UserName);
                paramList[1] = new EalDbParameter("@password", encryptedPassword);

                DataSet resultSet = EalDbAccess.ExecuteDataset(DatabaseType, Connection_String, CommandType.StoredProcedure, "mySobek_Get_User_By_UserName_Password", paramList);

                if ((resultSet.Tables.Count > 0) && (resultSet.Tables[0].Rows.Count > 0))
                {
                    return build_user_object_from_dataset(resultSet);
                }

                // Return the browse id
                return null;

            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("Engine_Database.Get_User", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.Get_User", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("Engine_Database.Get_User", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

        private static User_Object build_user_object_from_dataset(DataSet ResultSet)
        {
            User_Object user = new User_Object();

            DataRow userRow = ResultSet.Tables[0].Rows[0];
            user.ShibbID = userRow["ShibbID"].ToString();
            user.UserID = Convert.ToInt32(userRow["UserID"]);
            user.UserName = userRow["username"].ToString();
            user.Email = userRow["EmailAddress"].ToString();
            user.Given_Name = userRow["FirstName"].ToString();
            user.Family_Name = userRow["LastName"].ToString();
            user.Send_Email_On_Submission = Convert.ToBoolean(userRow["SendEmailOnSubmission"]);
            user.Can_Submit = Convert.ToBoolean(userRow["Can_Submit_Items"]);
            user.Is_Temporary_Password = Convert.ToBoolean(userRow["isTemporary_Password"]);
            user.Nickname = userRow["Nickname"].ToString();
            user.Organization = userRow["Organization"].ToString();
            user.Organization_Code = userRow["OrganizationCode"].ToString();
            user.Department = userRow["Department"].ToString();
            user.College = userRow["College"].ToString();
            user.Unit = userRow["Unit"].ToString();
            user.Default_Rights = userRow["Rights"].ToString();
            user.Preferred_Language = userRow["Language"].ToString();
            user.Is_Internal_User = Convert.ToBoolean(userRow["Internal_User"]);
            user.Edit_Template_Code_Simple = userRow["EditTemplate"].ToString();
            user.Edit_Template_Code_Complex = userRow["EditTemplateMarc"].ToString();
            user.Can_Delete_All = Convert.ToBoolean(userRow["Can_Delete_All_Items"]);
            user.Is_System_Admin = Convert.ToBoolean(userRow["IsSystemAdmin"]);
            user.Is_Portal_Admin = Convert.ToBoolean(userRow["IsPortalAdmin"]);
            user.Is_Host_Admin = Convert.ToBoolean(userRow["IsHostAdmin"]);
            user.Include_Tracking_In_Standard_Forms = Convert.ToBoolean(userRow["Include_Tracking_Standard_Forms"]);
            user.Receive_Stats_Emails = Convert.ToBoolean(userRow["Receive_Stats_Emails"]);
            user.Has_Item_Stats = Convert.ToBoolean(userRow["Has_Item_Stats"]);
            user.LoggedOn = true;
            user.Internal_Notes = userRow["InternalNotes"].ToString();
            user.Processing_Technician = Convert.ToBoolean(userRow["ProcessingTechnician"]);
            user.Scanning_Technician = Convert.ToBoolean(userRow["ScanningTechnician"]);

            if (Convert.ToInt32(userRow["descriptions"]) > 0)
                user.Has_Descriptive_Tags = true;

            foreach (DataRow thisRow in ResultSet.Tables[1].Rows)
            {
                user.Add_Template(thisRow["TemplateCode"].ToString(), Convert.ToBoolean(thisRow["GroupDefined"].ToString()));
            }

            foreach (DataRow thisRow in ResultSet.Tables[2].Rows)
            {
                user.Add_Default_Metadata_Set(thisRow["MetadataCode"].ToString(), Convert.ToBoolean(thisRow["GroupDefined"].ToString()));
            }

            user.Items_Submitted_Count = ResultSet.Tables[3].Rows.Count;
            foreach (DataRow thisRow in ResultSet.Tables[3].Rows)
            {
                if (!user.BibIDs.Contains(thisRow["BibID"].ToString().ToUpper()))
                    user.Add_BibID(thisRow["BibID"].ToString().ToUpper());
            }

            // Add links to regular expressions
            foreach (DataRow thisRow in ResultSet.Tables[4].Rows)
            {
                user.Add_Editable_Regular_Expression(thisRow["EditableRegex"].ToString());
            }

            // Add links to aggregationPermissions
            foreach (DataRow thisRow in ResultSet.Tables[5].Rows)
            {

                user.Add_Aggregation(thisRow["Code"].ToString(), thisRow["Name"].ToString(), Convert.ToBoolean(thisRow["CanSelect"]), Convert.ToBoolean(thisRow["CanEditMetadata"]), Convert.ToBoolean(thisRow["CanEditBehaviors"]), Convert.ToBoolean(thisRow["CanPerformQc"]), Convert.ToBoolean(thisRow["CanUploadFiles"]), Convert.ToBoolean(thisRow["CanChangeVisibility"]), Convert.ToBoolean(thisRow["CanDelete"]), Convert.ToBoolean(thisRow["IsCollectionManager"]), Convert.ToBoolean(thisRow["OnHomePage"]), Convert.ToBoolean(thisRow["IsAggregationAdmin"]), Convert.ToBoolean(thisRow["GroupDefined"]));

            }

            // Add the current folder names
            Dictionary<int, User_Folder> folderNodes = new Dictionary<int, User_Folder>();
            List<User_Folder> parentNodes = new List<User_Folder>();
            foreach (DataRow folderRow in ResultSet.Tables[6].Rows)
            {
                string folderName = folderRow["FolderName"].ToString();
                int folderid = Convert.ToInt32(folderRow["UserFolderID"]);
                int parentid = Convert.ToInt32(folderRow["ParentFolderID"]);
                bool isPublic = Convert.ToBoolean(folderRow["isPublic"]);

                User_Folder newFolderNode = new User_Folder(folderName, folderid) { IsPublic = isPublic };
                if (parentid == -1)
                    parentNodes.Add(newFolderNode);
                folderNodes.Add(folderid, newFolderNode);
            }
            foreach (DataRow folderRow in ResultSet.Tables[6].Rows)
            {
                int folderid = Convert.ToInt32(folderRow["UserFolderID"]);
                int parentid = Convert.ToInt32(folderRow["ParentFolderID"]);
                if (parentid > 0)
                {
                    folderNodes[parentid].Add_Child_Folder(folderNodes[folderid]);
                }
            }
            foreach (User_Folder rootFolder in parentNodes)
                user.Add_Folder(rootFolder);

            // Get the list of BibID/VID associated with this
            foreach (DataRow itemRow in ResultSet.Tables[7].Rows)
            {
                user.Add_Bookshelf_Item(itemRow["BibID"].ToString(), itemRow["VID"].ToString());
            }

            // Add the user groups to which this user is a member
            foreach (DataRow groupRow in ResultSet.Tables[8].Rows)
            {
                user.Add_User_Group(groupRow[0].ToString());
            }

            // Get all the user settings
            foreach (DataRow settingRow in ResultSet.Tables[9].Rows)
            {
                user.Add_Setting(settingRow["Setting_Key"].ToString(), settingRow["Setting_Value"].ToString(), false);
            }

            return user;
        }

        #endregion

    }
}
