#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using Microsoft.ApplicationBlocks.Data;
using SobekCM.Core.Aggregations;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Items;
using SobekCM.Core.OAI;
using SobekCM.Core.Results;
using SobekCM.Core.Settings;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Items.Authority;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Tools;
using SobekCM.Tools.FDA;

#endregion

namespace SobekCM.Library.Database
{
	/// <summary> Gateway to the databases used by SobekCM </summary>
	public class SobekCM_Database
	{

        private const int LOOKAHEAD_FACTOR = 5;

		private static string connectionString;
		private static Exception lastException;


		#region Temporary dataset work 

		//static SobekCM_Database()
		//{
		//	DataSet set = EPC_Export;

		//	set.DataSetName = "Key_West_City_Directory";
		//	set.Tables[0].TableName = "Person";
		//	set.Tables[1].TableName = "Corporation";
		//	set.Tables[2].TableName = "Person_Occupation_Link";

		//	// Declare parent column and child column variables.
		//	DataColumn parentColumn;
		//	DataColumn childColumn;
		//	ForeignKeyConstraint foreignKeyConstraint;

		//	// Set parent and child column variables.
		//	parentColumn = set.Tables[0].Columns[0];
		//	childColumn = set.Tables[2].Columns[0];
		//	UniqueConstraint uniqueCon = new UniqueConstraint("PersonIdUniqueConstraint", parentColumn);
		//	set.Tables[0].Constraints.Add(uniqueCon);
		//	foreignKeyConstraint = new ForeignKeyConstraint("PersonOccupationForeignKeyConstraint", parentColumn, childColumn);

		//	set.Tables[2].Constraints.Add(foreignKeyConstraint);




		//	// Declare parent column and child column variables.
		//	DataColumn parentColumn2;
		//	DataColumn childColumn2;
		//	ForeignKeyConstraint foreignKeyConstraint2;

		//	// Set parent and child column variables.
		//	parentColumn2 = set.Tables[1].Columns[0];
		//	childColumn2 = set.Tables[2].Columns[1];
		//	UniqueConstraint uniqueCon2 = new UniqueConstraint("CorporationIdUniqueConstraint", parentColumn2);
		//	set.Tables[1].Constraints.Add(uniqueCon2);
		//	foreignKeyConstraint2 = new ForeignKeyConstraint("CorporationOccupationForeignKeyConstraint", parentColumn2, childColumn2);

		//	set.Tables[2].Constraints.Add(foreignKeyConstraint2);

		//	DataColumn uniqueColumn = set.Tables[1].Columns[1];
		//	UniqueConstraint uniqueCon3 = new UniqueConstraint("CorporationNameUniqueConstraint", uniqueColumn);
		//	set.Tables[1].Constraints.Add(uniqueCon3);

		//	set.Tables[0].Columns["Sex"].MaxLength = 1;


		//	set.Tables[0].ExtendedProperties.Add("Description", "Table contains information about all of the people which appeared within the city directory.  These may also be linked to a corporate body and/or an historic occupation.");
		//	set.Tables[0].Columns[0].Caption = "Unique key for this person within the dataset";
		//	set.Tables[0].Columns[0].AllowDBNull = false;
		//	set.Tables[0].Columns[0].AutoIncrement = true;
		//	set.Tables[0].Columns[1].Caption = "Title of the city directory from which this person's data was derived";
		//	set.Tables[0].Columns[1].AllowDBNull = false;
		//	set.Tables[0].Columns[2].Caption = "Year of the city directory from which this person's data was derived";
		//	set.Tables[0].Columns[2].AllowDBNull = false;
		//	set.Tables[0].Columns[3].Caption = "Page sequence within the city directory that this person's data was derived";
		//	set.Tables[0].Columns[4].Caption = "Original source line from which this person's data was derived";
		//	set.Tables[0].Columns[5].Caption = "Last or family name of this person";
		//	set.Tables[0].Columns[5].AllowDBNull = false;
		//	set.Tables[0].Columns[6].Caption = "First or given name of this person";
		//	set.Tables[0].Columns[6].AllowDBNull = false;
		//	set.Tables[0].Columns[7].Caption = "Middle name of this person";
		//	set.Tables[0].Columns[8].Caption = "Title associated with this person ( i.e., Mr., Reverand, etc..)";
		//	set.Tables[0].Columns[9].Caption = "Normalized racial-ethno affiliation associated with this person by the city directory";
		//	set.Tables[0].Columns[10].Caption = "Name of any spouse (usually just the first name)";
		//	set.Tables[0].Columns[11].Caption = "General note field";
		//	set.Tables[0].Columns[12].Caption = "Location of the home, usually cross-streets or an street address";

		//	set.Tables[1].ExtendedProperties.Add("Description", "Table contains all the corporate bodies from the city directory which were linked directly to a person.");
		//	set.Tables[1].Columns[0].Caption = "Unique key for this corporate body within the dataset";
		//	set.Tables[1].Columns[0].AllowDBNull = false;
		//	set.Tables[1].Columns[0].AutoIncrement = true;
		//	set.Tables[1].Columns[1].Caption = "Name of the corporate body";
		//	set.Tables[1].Columns[1].AllowDBNull = false;

		//	set.Tables[2].ExtendedProperties.Add("Description", "Table joins the people from the city directory with any occupation which was recorded in the directory and also to any corporate body from the directory.");
		//	set.Tables[2].Columns[0].Caption = "Link to a person derived from a city directory";
		//	set.Tables[2].Columns[0].AllowDBNull = false;
		//	set.Tables[2].Columns[1].Caption = "Possible link to a corporate body";
		//	set.Tables[2].Columns[2].Caption = "Historic occupation, as recorded within the city directory";

		//	set.WriteXml(@"C:\GitRepository\SobekCM\SobekCM-Web-Application\SobekCM\temp\gville_epc.xml", XmlWriteMode.WriteSchema);
		//}


		///// <summary> Gets the datatable containging all possible disposition types </summary>
		///// <remarks> This calls the 'Tracking_Get_All_Possible_Disposition_Types' stored procedure. </remarks>
		//public static DataSet EPC_Export
		//{
		//	get
		//	{
		//		DataSet returnSet = SqlHelper.ExecuteDataset(@"data source=lib-ufdc-cache\UFDCPROD;initial catalog=EPC;integrated security=Yes;", CommandType.StoredProcedure, "Export_DataSet");
		//		return returnSet;
		//	}
		//}

		#endregion

		/// <summary> Gets the last exception caught by a database call through this gateway class  </summary>
		public static Exception Last_Exception
		{
			get { return lastException; }
		}

		/// <summary> Connection string to the main SobekCM databaase </summary>
		/// <remarks> This database hold all the information about items, item aggregationPermissions, statistics, and tracking information</remarks>
		public static string Connection_String
		{
		    set
		    {

		        connectionString = value;

                Engine_Database.Connection_String = value;
		        
		    }
			get	{	return connectionString;	}
		}

		/// <summary> Test connectivity to the database </summary>
		/// <returns> TRUE if connection can be made, otherwise FALSE </returns>
		public static bool Test_Connection()
		{

			try
			{
				SqlConnection newConnection = new SqlConnection(connectionString);
				newConnection.Open();

				newConnection.Close();
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary> Test connectivity to the database </summary>
		/// <returns> TRUE if connection can be made, otherwise FALSE </returns>
		public static bool Test_Connection( string Test_Connection_String )
		{

			try
			{
				SqlConnection newConnection = new SqlConnection(Test_Connection_String);
				newConnection.Open();

				newConnection.Close();
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary> Gets the datatable containging all possible disposition types </summary>
		/// <remarks> This calls the 'Tracking_Get_All_Possible_Disposition_Types' stored procedure. </remarks>
		public static DataTable All_Possible_Disposition_Types
		{
			get
			{
				DataSet returnSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "Tracking_Get_All_Possible_Disposition_Types");
				return returnSet.Tables[0];
			}
		}

		/// <summary> Gets the datatable containging all work flow types </summary>
		/// <remarks> This calls the 'Tracking_Get_All_Possible_Workflows' stored procedure. </remarks>
		public static DataTable All_WorkFlow_Types
		{
			get
			{
				DataSet returnSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "Tracking_Get_All_Possible_Workflows");
				return returnSet.Tables[0];
			}
		}


		/// <summary> Get the list of all tracking boxes from the database </summary>
		/// <remarks> This calls the 'Tracking_Box_List' stored procedure. </remarks>
		public static List<string> All_Tracking_Boxes
		{
			get
			{
				DataSet returnSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "Tracking_Box_List");
				List<string> returnValue = new List<string>();
				if (returnSet != null)
				{
					returnValue.AddRange(from DataRow thisRow in returnSet.Tables[0].Rows where thisRow["Tracking_Box"] != DBNull.Value select thisRow["Tracking_Box"].ToString() into trackingBox where trackingBox.Length > 0 select trackingBox);
				}
				return returnValue;
			}
		}

		/// <summary> Sets the main thumbnail for a given digital resource </summary>
		/// <param name="BibID"> Bibliographic identifier for the item </param>
		/// <param name="VID"> Volume identifier for the item </param>
		/// <param name="MainThumbnail"> Filename for the new main thumbnail </param>
		/// <returns>TRUE if successful, otherwise FALSE</returns>
		/// <remarks> This calls the 'SobekCM_Set_Main_Thumbnail' stored procedure </remarks>
		public static bool Set_Item_Main_Thumbnail(string BibID, string VID, string MainThumbnail)
		{

			try
			{
				// build the parameter list
				SqlParameter[] paramList = new SqlParameter[3];
				paramList[0] = new SqlParameter("@bibid", BibID);
				paramList[1] = new SqlParameter("@vid", VID);
				paramList[2] = new SqlParameter("@mainthumb", MainThumbnail);

				//Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Set_Main_Thumbnail", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		/// <summary> Updates the cached links between aggregationPermissions and metadata, used by larger collections </summary>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'Admin_Update_Cached_Aggregation_Metadata_Links' stored procedure.<br /><br />This runs asychronously as this routine may run for a minute or more.</remarks>
		public static bool Admin_Update_Cached_Aggregation_Metadata_Links()
		{
			try
			{
				// Create the connection
				using (SqlConnection connect = new SqlConnection(connectionString))
				{
					// Create the command 
					SqlCommand executeCommand = new SqlCommand("Admin_Update_Cached_Aggregation_Metadata_Links", connect)
													{CommandType = CommandType.StoredProcedure};

					// Create the data reader
					connect.Open();
					executeCommand.BeginExecuteNonQuery();
				}

				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;

			}
		}

		#region Methods relating to the build error logs

		/// <summary> Gets the list of build errors that have been encountered between two dates </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <param name="StartDate"> Beginning of the date range </param>
		/// <param name="EndDate"> End of the date range</param>
		/// <returns> Datatable of all the build errors encountered </returns>
		/// <remarks> This calls the 'SobekCM_Get_Build_Error_Logs' stored procedure </remarks>
		public static DataTable Builder_Get_Error_Logs(Custom_Tracer Tracer, DateTime StartDate, DateTime EndDate )
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Build_Error_Logs", "Pulling data from database");
			}

			try
			{
				// Execute this query stored procedure
				SqlParameter[] paramList = new SqlParameter[2];
				paramList[0] = new SqlParameter("@firstdate", StartDate);
				paramList[1] = new SqlParameter("@seconddate", EndDate);
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_Build_Error_Logs", paramList);
				return tempSet.Tables[0];
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_Build_Error_Logs", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Build_Error_Logs", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Build_Error_Logs", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Clears the item error log associated with a particular bibid / vid </summary>
		/// <param name="BibID"> Bibliographic identifier for the item (or name of failed process)</param>
		/// <param name="VID"> Volume identifier for the item </param>
		/// <param name="ClearedBy"> Name of user or process that cleared the error </param>
		/// <returns>TRUE if successful, otherwise FALSE</returns>
		/// <remarks> No error is deleted, but this does set a flag on the error indicating it was cleared so it will no longer appear in the list<br /><br />
		/// This calls the 'SobekCM_Clear_Item_Error_Log' stored procedure </remarks>
		public static bool Builder_Clear_Item_Error_Log(string BibID, string VID, string ClearedBy)
		{
			// Note, this is no longer utilized in the new logging system.
			// Keeping this hook while we consider if we should expire errors in the system.
			// Will create new online web interfac and then decide
			return true;

			try
			{
				// build the parameter list
				SqlParameter[] paramList = new SqlParameter[3];
				paramList[0] = new SqlParameter("@BibID", BibID);
				paramList[1] = new SqlParameter("@VID", VID);
				paramList[2] = new SqlParameter("@ClearedBy", ClearedBy);

				//Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Clear_Item_Error_Log", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		/// <summary> Expire older builder logs which can be removed from the system </summary>
		/// <param name="Retain_For_Days"> Number of days of logs which should be retained </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Builder_Expire_Log_Entries' stored procedure </remarks>
		public static bool Builder_Expire_Log_Entries(int Retain_For_Days)
		{
			try
			{
				// build the parameter list
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@Retain_For_Days", Retain_For_Days);

				//Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Builder_Expire_Log_Entries", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		/// <summary> Add a new log entry for the builder </summary>
		/// <param name="RelatedBuilderLogID"> Primary key for a related log id, if this adds detail to an already logged entry </param>
		/// <param name="BibID_VID"> BibID / VID, if this is related to an item error (either existing item, or not) </param>
		/// <param name="LogType"> Type of the log entry ( i.e., Error, Complete, etc.. )</param>
		/// <param name="LogMessage"> Actual log entry message </param>
		/// <param name="MetsType"> Type of the METS file (if related to one) </param>
		/// <returns> The primary key for this new log entry, in case detail logs need to be added </returns>
		/// <remarks> This calls the 'SobekCM_Builder_Add_Log' stored procedure </remarks>
		public static long Builder_Add_Log_Entry(long RelatedBuilderLogID, string BibID_VID, string LogType, string LogMessage, string MetsType )
		{
			try
			{
				// build the parameter list
				SqlParameter[] paramList = new SqlParameter[6];
				if ( RelatedBuilderLogID < 0 )
					paramList[0] = new SqlParameter("@RelatedBuilderLogID", DBNull.Value);
				else
					paramList[0] = new SqlParameter("@RelatedBuilderLogID", RelatedBuilderLogID);

				paramList[1] = new SqlParameter("@BibID_VID", BibID_VID);
				paramList[2] = new SqlParameter("@LogType", LogType);
				paramList[3] = new SqlParameter("@LogMessage", LogMessage);
				paramList[4] = new SqlParameter("@Mets_Type", MetsType);
				paramList[5] = new SqlParameter("@BuilderLogID", -1) {Direction = ParameterDirection.InputOutput};

				//Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Builder_Add_Log", paramList);
				return Convert.ToInt64(paramList[5].Value);
			}
			catch (Exception ee)
			{
				lastException = ee;
				return -1;
			}
		}

		#endregion

		#region Methods relating to usage statistics and item aggregation count statistics

		/// <summary> Pulls the most often hit titles and items, by item aggregation  </summary>
		/// <param name="AggregationCode"> Code for the item aggregation of interest </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataSet with the most often hit items and titles and the number of hits </returns>
		/// <remarks> This calls the 'SobekCM_Statistics_Aggregation_Titles' stored procedure <br /><br />
		/// This is used by the <see cref="Statistics_HtmlSubwriter"/> class</remarks>
		public static DataSet Statistics_Aggregation_Titles( string AggregationCode, Custom_Tracer Tracer )
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Statistics_Aggregation_Titles", "Pulling data from database");
			}

			try
			{
				// Execute this query stored procedure
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@code", AggregationCode);
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Statistics_Aggregation_Titles", paramList);
				return tempSet;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Statistics_Aggregation_Titles", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Statistics_Aggregation_Titles", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Statistics_Aggregation_Titles", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Pulls the complete usage statistics, broken down by each level of the item aggregation hierarchy, between two dates </summary>
		/// <param name="Early_Year">Year portion of the start date</param>
		/// <param name="Early_Month">Month portion of the start date</param>
		/// <param name="Last_Year">Year portion of the last date</param>
		/// <param name="Last_Month">Month portion of the last date</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Complete usage statistics, broken down by each level of the item aggregation hierarchy, between the provided dates</returns>
		/// <remarks> This calls the 'SobekCM_Statistics_By_Date_Range' stored procedure <br /><br />
		/// This is used by the <see cref="Statistics_HtmlSubwriter"/> class</remarks>
		public static DataTable Statistics_By_Date_Range(int Early_Year, int Early_Month, int Last_Year, int Last_Month, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Statistics_By_Date_Range", "Pulling data from database");
			}

			try
			{
				// Execute this query stored procedure
				SqlParameter[] paramList = new SqlParameter[4];
				paramList[0] = new SqlParameter("@year1", Early_Year);
				paramList[1] = new SqlParameter("@month1", Early_Month);
				paramList[2] = new SqlParameter("@year2", Last_Year);
				paramList[3] = new SqlParameter("@month2", Last_Month);
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Statistics_By_Date_Range", paramList);
				return tempSet.Tables[0];
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Statistics_By_Date_Range", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Statistics_By_Date_Range", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Statistics_By_Date_Range", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}



		/// <summary> Returns the month-by-month usage statistics details by item aggregation </summary>
		/// <param name="AggregationCode"> Code for the item aggregation of interest </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Month-by-month usage statistics for item aggregation of interest </returns>
		/// <remarks> Passing 'ALL' in as the aggregation code returns the statistics for all item aggregationPermissions within this library <br /><br />
		/// This calls the 'SobekCM_Get_Collection_Statistics_History' stored procedure <br /><br />
		/// This is used by the <see cref="Statistics_HtmlSubwriter"/> class</remarks>
		public static DataTable Get_Aggregation_Statistics_History(string AggregationCode, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Collection_Statistics_History", "Pulling history for '" + AggregationCode + "' from database");
			}

			try
			{
				// Execute this query stored procedure
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@code", AggregationCode);
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_Collection_Statistics_History", paramList);
				return tempSet.Tables[0];
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_Aggregation_Statistics_History", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Aggregation_Statistics_History", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Aggregation_Statistics_History", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Returns the month-by-month usage statistics details by item and item group </summary>
		/// <param name="BibID"> Bibliographic identifier for the item group of interest </param>
		/// <param name="VID"> Volume identifier for the item of interest </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Month-by-month usage statistics for item and item-group </returns>
		/// <remarks> This calls the 'SobekCM_Get_Item_Statistics' stored procedure  </remarks>
		public static DataSet Get_Item_Statistics_History(string BibID, string VID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Item_Statistics_History", "Pulling history for '" + BibID + "_" + VID + "' from database");
			}

			try
			{
				// Execute this query stored procedure
				SqlParameter[] paramList = new SqlParameter[2];
				paramList[0] = new SqlParameter("@BibID", BibID);
				paramList[1] = new SqlParameter("@VID", VID);
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_Item_Statistics", paramList);
				return tempSet;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_Item_Statistics_History", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Item_Statistics_History", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Item_Statistics_History", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Gets the current title, item, and page count for each item aggregation in the item aggregation hierarchy </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Datatable with all the current title, item, and page count for each item aggregation</returns>
		/// <remarks> This calls the 'SobekCM_Item_Count_By_Collection' stored procedure  <br /><br />
		/// This is used by the <see cref="Internal_HtmlSubwriter"/> class</remarks>
		public static DataTable Get_Item_Aggregation_Count(Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Count", "Pulling list from database");
			}

			try
			{
				// Execute this query stored procedure
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Item_Count_By_Collection");
				return tempSet.Tables[0];
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Count", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Count", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Count", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Gets the title, item, and page count for each item aggregation currently and at some previous point of time </summary>
		/// <param name="Date1"> Date from which to additionally include item count </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Datatable with all the  title, item, and page count for each item aggregation currently and at some previous point of time </returns>
		/// <remarks> This calls the 'SobekCM_Item_Count_By_Collection_By_Dates' stored procedure  <br /><br />
		/// This is used by the <see cref="Internal_HtmlSubwriter"/> class</remarks>
		public static DataTable Get_Item_Aggregation_Count(DateTime Date1, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Count", "Pulling from database ( includes fytd starting " + Date1.ToShortDateString() + ")");
			}

			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[2];
				paramList[0] = new SqlParameter("@date1", Date1);
				paramList[1] = new SqlParameter("@date2", DBNull.Value);

				// Execute this query stored procedure
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Item_Count_By_Collection_By_Date_Range", paramList);
				return tempSet.Tables[0];
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Count", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Count", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Count", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Gets the title, item, and page count for each item aggregation currently and at some previous point of time </summary>
		/// <param name="Date1"> Date from which to additionally include item count </param>
		/// <param name="Date2"> Date to which to additionally include item count </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Datatable with all the  title, item, and page count for each item aggregation at some previous point of time and then the increase in these counts between the two provided dates </returns>
		/// <remarks> This calls the 'SobekCM_Item_Count_By_Collection_By_Date_Range' stored procedure  <br /><br />
		/// This is used by the <see cref="Internal_HtmlSubwriter"/> class</remarks>
		public static DataTable Get_Item_Aggregation_Count_DateRange(DateTime Date1, DateTime Date2, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Count_DateRange", "Pulling from database");
			}

			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[2];
				paramList[0] = new SqlParameter("@date1", Date1);
				paramList[1] = new SqlParameter("@date2", Date2);

				// Execute this query stored procedure
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Item_Count_By_Collection_By_Date_Range", paramList);
				return tempSet.Tables[0];
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Count_DateRange", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Count_DateRange", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Count_DateRange", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}



		/// <summary> Gets the item and page count loaded to this digital library by month and year </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataTable of the count of all items and pages loaded to this digital library by month and year </returns>
		/// <remarks> This calls the 'SobekCM_Page_Item_Count_History' stored procedure </remarks>
		public static DataTable Get_Page_Item_Count_History( Custom_Tracer Tracer )
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Page_Item_Count_History", "Pulling from database");
			}

			try
			{
				// Define a temporary dataset
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Page_Item_Count_History");

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
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_Page_Item_Count_History", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Page_Item_Count_History", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Page_Item_Count_History", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}


		/// <summary> Gets the list of all users that are linked to items which may have usage statistics  </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataTable of all the users linked to items </returns>
		/// <remarks> This calls the 'SobekCM_Stats_Get_Users_Linked_To_Items' stored procedure </remarks>
		public static DataTable Get_Users_Linked_To_Items( Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Users_Linked_To_Items", "Pulling from database");
			}

			try
			{
				// Define a temporary dataset
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Stats_Get_Users_Linked_To_Items" );

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
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_Users_Linked_To_Items", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Users_Linked_To_Items", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Users_Linked_To_Items", ee.StackTrace, Custom_Trace_Type_Enum.Error);
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
		public static DataTable Get_User_Linked_Items_Stats( int UserID, int Month, int Year, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_User_Linked_Items_Stats", "Pulling from database");
			}

			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[3];
				paramList[0] = new SqlParameter("@userid", UserID);
				paramList[1] = new SqlParameter("@month", Month);
				paramList[2] = new SqlParameter("@year", Year);

				// Define a temporary dataset
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Stats_Get_User_Linked_Items_Stats", paramList);

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
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_User_Linked_Items_Stats", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_User_Linked_Items_Stats", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_User_Linked_Items_Stats", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}


		#endregion

        #region Method to return DATATABLE of all items from an aggregation

		/// <summary> Gets the list of unique coordinate points and associated bibid and group title for a single 
		/// item aggregation </summary>
		/// <param name="Aggregation_Code"> Code for the item aggregation </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataTable with all the coordinate values </returns>
		/// <remarks> This calls the 'SobekCM_Coordinate_Points_By_Aggregation' stored procedure </remarks>
		public static DataTable Get_All_Items_By_AggregationID(string Aggregation_Code, List<string> FIDs, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_All_Items_By_AggregationID", "Pull the item list");
            }

            string HOOK_FIDDBCallPrefix = "SobekCM_Metadata_Basic_Search_Table"; //this is the correct sql syntax for searching the db table for a specific metadata type
            int nonFIDsParamCount = 2; //how many non fids are there?

            // Build the parameter list
            SqlParameter[] paramList = new SqlParameter[(FIDs.Count + nonFIDsParamCount)];
            paramList[0] = new SqlParameter("@aggregation_code", Aggregation_Code);
            //paramList[1] = new SqlParameter("@FID1_PassIn", FIDs[0]);
            //paramList[2] = new SqlParameter("@FID2_PassIn", FIDs[1]);
            //paramList[3] = new SqlParameter("@FID3_PassIn", FIDs[2]);
            //paramList[4] = new SqlParameter("@FID4_PassIn", FIDs[3]);
            //paramList[5] = new SqlParameter("@FID5_PassIn", FIDs[4]);
            //paramList[6] = new SqlParameter("@FID6_PassIn", FIDs[5]);
            //paramList[7] = new SqlParameter("@FID7_PassIn", FIDs[6]);
            //paramList[8] = new SqlParameter("@FID8_PassIn", FIDs[7]);
            int paramListIndex = 0; //set where we are at
            int FIDIndex = 0; //where do the fids start (zero)
            foreach (string fiD in FIDs)
            {
                paramListIndex++;
                FIDIndex++;
                paramList[paramListIndex] = new SqlParameter("@FID" + FIDIndex.ToString(), fiD);
            }
            paramList[(paramListIndex + 1)] = new SqlParameter("FIDDBCallPrefix", HOOK_FIDDBCallPrefix);

            // Define a temporary dataset
            DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_All_Items_By_AggregationID", paramList);
            return tempSet == null ? null : tempSet.Tables[0];
        }

        #endregion

        #region Method to return STIRNG of the human readable metadata code

        /// <summary> Gets the human readable name of a metadate id</summary>
        /// <param name="metadataTypeId"> Code for the metadata</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> String with the name of the metadata </returns>
        /// <remarks> This calls the 'SobekCM_Get_Metadata_Name_From_MetadataTypeID' stored procedure </remarks>
        public static string Get_Metadata_Name_From_MetadataTypeID(short metadataTypeId, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_Metadata_Name_From_MetadataTypeID", "Get the metadataID name");
            }

            // Build the parameter list
            SqlParameter[] paramList = new SqlParameter[1];
            paramList[0] = new SqlParameter("@metadataTypeID", metadataTypeId);

            // Define a temporary dataset
            DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_Metadata_Name_From_MetadataTypeID", paramList);
            DataTable tempResult = tempSet.Tables[0];
            return tempResult.Rows[0][0].ToString();
        }

        #endregion

		#region Method to perform a metadata search of items in the database

		/// <summary> Gets the list of metadata fields searchable in the database, along with field number </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataTable with all of the search fields and search field id's for metadata searching </returns>
		/// <remarks> This calls the 'SobekCM_Get_Metadata_Fields' stored procedure  </remarks>
		public static DataTable Get_Metadata_Fields(Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Metadata_Fields", "Pulling from database");
			}

			try
			{
				// Define a temporary dataset
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_Metadata_Fields");

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
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_Metadata_Fields", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Metadata_Fields", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Metadata_Fields", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

        #endregion

        #region Methods to get the information about an ITEM or ITEM GROUP

        /// <summary> Determines what restrictions are present on an item  </summary>
		/// <param name="BibID"> Bibliographic identifier for the volume to retrieve </param>
		/// <param name="VID"> Volume identifier for the volume to retrieve </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataSet with detailed information about this item from the database </returns>
		/// <remarks> This calls the 'SobekCM_Get_Item_Restrictions' stored procedure </remarks> 
		public static void Get_Item_Restrictions(string BibID, string VID, Custom_Tracer Tracer, out bool IsDark, out short IP_Restrction_Mask )
		{
			IsDark = true;
			IP_Restrction_Mask = -1;

			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Item_Restrictions", "");
			}

			try
			{
				SqlParameter[] parameters = new SqlParameter[2];
				parameters[0] = new SqlParameter("@BibID", BibID);
				parameters[1] = new SqlParameter("@VID", VID);


				// Define a temporary dataset
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_Item_Restrictions", parameters);

				// Was there an answer?
				if ((tempSet.Tables.Count > 0) && (tempSet.Tables[0].Rows.Count > 0))
				{
					IP_Restrction_Mask = short.Parse(tempSet.Tables[0].Rows[0]["IP_Restriction_Mask"].ToString());
					IsDark = bool.Parse(tempSet.Tables[0].Rows[0]["Dark"].ToString());
				}
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_Item_Restrictions", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Item_Restrictions", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Item_Restrictions", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
			}
		}

		/// <summary> Gets some basic information about an item before displaing it, such as the descriptive notes from the database, ability to add notes, etc.. </summary>
		/// <param name="ItemID"> Bibliographic identifier for the volume to retrieve </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataSet with detailed information about this item from the database </returns>
		/// <remarks> This calls the 'SobekCM_Get_BibID_VID_From_ItemID' stored procedure </remarks> 
		public static DataRow Lookup_Item_By_ItemID( int ItemID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Lookup_Item_By_ItemID", "Trying to pull information for " + ItemID );
			}

			try
			{
				SqlParameter[] parameters = new SqlParameter[1];
				parameters[0] = new SqlParameter("@itemid", ItemID);

				// Define a temporary dataset
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_BibID_VID_From_ItemID", parameters);

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
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Lookup_Item_By_ItemID", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Lookup_Item_By_ItemID", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Lookup_Item_By_ItemID", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Pulls the item id, main thumbnail, and aggregation codes and adds them to the resource object </summary>
		/// <param name="Resource"> Digital resource object </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Builder_Get_Minimum_Item_Information' stored procedure </remarks> 
		public static bool Add_Minimum_Builder_Information(SobekCM_Item Resource)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[2];
				parameters[0] = new SqlParameter("@bibid", Resource.BibID);
				parameters[1] = new SqlParameter("@vid", Resource.VID);

				// Define a temporary dataset
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Builder_Get_Minimum_Item_Information", parameters);

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
				foreach( DataRow thisRow in tempSet.Tables[1].Rows )
				{
					string code = thisRow[0].ToString();
					Resource.Behaviors.Add_Aggregation(code);
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

					Wordmark_Info newIcon = new Wordmark_Info { HTML = html, Link = link, Title = name, Code = code };
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
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}            
		}

		/// <summary> Pulls the item id by BibID / VID </summary>
		/// <param name="BibID"> Bibliographic identifier for the digital resource object </param>
		/// <param name="VID"> Volume identifier for the digital resource object </param>
		/// <returns> Primary key for this item from the database </returns>
		/// <remarks> This calls the 'SobekCM_Builder_Get_Minimum_Item_Information' stored procedure </remarks> 
		public static int Get_ItemID_From_Bib_VID( string BibID, string VID)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[2];
				parameters[0] = new SqlParameter("@bibid",BibID);
				parameters[1] = new SqlParameter("@vid", VID);

				// Define a temporary dataset
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Builder_Get_Minimum_Item_Information", parameters);

				// If there was no data for this collection and entry point, return null (an ERROR occurred)
				if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
				{
					return -1;
				}

				// Get the item id and the thumbnail from the first table
				return Convert.ToInt32(tempSet.Tables[0].Rows[0][0]);
			}
			catch (Exception ee)
			{
				lastException = ee;
				return -1;
			}
		}

		#endregion

		#region Method to get the information about an ITEM GROUP

		//// THIS HAS BEEN REPLACED BY ITEM GROUP DETAILS (WHICH IS VERY SIMILAR)
		///// <summary> Gets the information about a title (item group) by BibID, including volumes, icons, and skins </summary>
		///// <param name="BibID"> Bibliographic identifier for the title of interest </param>
		///// <param name="tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		///// <returns> Strongly typed dataset with information about the title (item group), including volumes, icons, and skins</returns>
		///// <remarks> This calls the 'SobekCM_Get_Multiple_Volumes' stored procedure </remarks>
		//public static Group_Information Get_Multiple_Volumes(string BibID, Custom_Tracer tracer)
		//{
		//    if (tracer != null)
		//    {
		//        tracer.Add_Trace("SobekCM_Database.Get_Multiple_Volumes", "List of volumes for " + BibID + " pulled from database");
		//    }

		//    try
		//    {
		//        // Create the connection
		//        SqlConnection connect = new SqlConnection(connectionString);

		//        // Create the command 
		//        SqlCommand executeCommand = new SqlCommand("SobekCM_Get_Multiple_Volumes", connect);
		//        executeCommand.CommandType = CommandType.StoredProcedure;
		//        executeCommand.Parameters.AddWithValue("@bibid", BibID);

		//        // Create the adapter
		//        SqlDataAdapter adapter = new SqlDataAdapter(executeCommand);

		//        // Add appropriate table mappings
		//        adapter.TableMappings.Add("Table", "Group");
		//        adapter.TableMappings.Add("Table1", "Item");
		//        adapter.TableMappings.Add("Table2", "Icon");

		//        // Fill the strongly typed dataset
		//        Group_Information thisGroup = new Group_Information();
		//        adapter.Fill(thisGroup);

		//        // If there was either no match, or more than one, return null
		//        if ((thisGroup == null) || (thisGroup.Tables.Count == 0) || (thisGroup.Tables[0] == null) || (thisGroup.Tables[0].Rows.Count == 0))
		//        {
		//            return null;
		//        }


		//        // Return the fully built object
		//        return thisGroup;
		//    }
		//    catch (Exception ee)
		//    {
		//        last_exception = ee;
		//        return null;
		//    }
		//}


		/// <summary> Gets the list of all items within this item group, indicated by BibID </summary>
		/// <param name="BibID"> Bibliographic identifier for the title of interest </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Strongly typed dataset with information about the title (item group), including volumes, icons, and skins</returns>
		/// <remarks> This calls the 'SobekCM_Get_Multiple_Volumes' stored procedure </remarks>
		public static SobekCM_Items_In_Title Get_Multiple_Volumes(string BibID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Multiple_Volumes", "List of volumes for " + BibID + " pulled from database");
			}

			try
			{
				// Create the connection
				SqlConnection connect = new SqlConnection(connectionString);

				// Create the command 
				SqlCommand executeCommand = new SqlCommand("SobekCM_Get_Multiple_Volumes", connect)
												{CommandType = CommandType.StoredProcedure};
				executeCommand.Parameters.AddWithValue("@bibid", BibID);

				// Create the adapter
				SqlDataAdapter adapter = new SqlDataAdapter(executeCommand);

				// Get the datatable
				DataSet valueSet = new DataSet();
				adapter.Fill(valueSet);

				// If there was either no match, or more than one, return null
				if ((valueSet.Tables.Count == 0) || (valueSet.Tables[0] == null) || (valueSet.Tables[0].Rows.Count == 0))
				{
					return null;
				}

				// Create the object
				SobekCM_Items_In_Title returnValue = new SobekCM_Items_In_Title(valueSet.Tables[0]);

				// Return the fully built object
				return returnValue;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_Multiple_Volumes", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Multiple_Volumes", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Multiple_Volumes", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		#endregion

		#region Methods to get the URL portals, edit and save new URL portals, and delete URL portals



		/// <summary> Delete a URL Portal from the database, by primary key </summary>
		/// <param name="Portal_ID"> Primary key for the URL portal to be deleted </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successul, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Delete_Portal' stored procedure </remarks>
		public static bool Delete_URL_Portal( int Portal_ID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Delete_URL_Portal", "Delete a URL Portal by portal id ( " + Portal_ID + " )");
			}

			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@portalid", Portal_ID);

				// Define a temporary dataset
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Delete_Portal", paramList);

				return true;
			}
			catch (Exception ee)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Delete_URL_Portal", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_URL_Portal", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_URL_Portal", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Edit an existing URL Portal or add a new URL portal, by primary key </summary>
		/// <param name="Portal_ID"> Primary key for the URL portal to be edited, or -1 if this is a new URL portal </param>
		/// <param name="Default_Aggregation"> Default aggregation for this URL portal </param>
		/// <param name="Default_Web_Skin"> Default web skin for this URL portal </param>
		/// <param name="BasePurl"> Base PURL , used to override the default PURL built from the current URL</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <param name="Base_URL"> URL used to match the incoming request with this URL portal</param>
		/// <param name="IsActive"> Flag indicates if this URL portal is active</param>
		/// <param name="IsDefault"> Flag indicates if this is the default URL portal, if no other portal match is found</param>
		/// <param name="Abbreviation"> Abbreviation for this system, when referenced by this URL portal</param>
		/// <param name="Name"> Name of this system, when referenced by this URL portal </param>
		/// <returns> New primary key (or existing key) for the URL portal added or edited </returns>
		/// <remarks> This calls the 'SobekCM_Edit_Portal' stored procedure </remarks>
		public static int Edit_URL_Portal(int Portal_ID, string Base_URL, bool IsActive, bool IsDefault, string Abbreviation, string Name, string Default_Aggregation, string Default_Web_Skin, string BasePurl, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Edit_URL_Portal", "Edit an existing URL portal, or add a new one");
			}

			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[10];
				paramList[0] = new SqlParameter("@PortalID", Portal_ID);
				paramList[1] = new SqlParameter("@Base_URL", Base_URL);
				paramList[2] = new SqlParameter("@isActive", IsActive);
				paramList[3] = new SqlParameter("@isDefault", IsDefault);
				paramList[4] = new SqlParameter("@Abbreviation", Abbreviation);
				paramList[5] = new SqlParameter("@Name", Name);
				paramList[6] = new SqlParameter("@Default_Aggregation", Default_Aggregation);
				paramList[7] = new SqlParameter("@Base_PURL", BasePurl);
				paramList[8] = new SqlParameter("@Default_Web_Skin", Default_Web_Skin);
				paramList[9] = new SqlParameter("@NewID", Portal_ID) {Direction = ParameterDirection.InputOutput};

				// Define a temporary dataset
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Edit_Portal", paramList);

				return Convert.ToInt32( paramList[9].Value );
			}
			catch (Exception ee)
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Edit_URL_Portal", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Edit_URL_Portal", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Edit_URL_Portal", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return -1;
			}
		}

		#endregion

		#region Methods to get all of the Application-Wide values





		/// <summary> Get the list of groups, with the top item (VID) </summary>
		/// <returns> List of groups, with the top item (VID) </returns>
		public static DataTable Get_All_Groups_First_VID()
		{
			// Define a temporary dataset
			SqlConnection connection = new SqlConnection(connectionString + "Connection Timeout=45");
			SqlCommand command = new SqlCommand("SobekCM_Get_All_Groups_First_VID", connection) { CommandTimeout = 45, CommandType = CommandType.StoredProcedure };
			SqlDataAdapter adapter = new SqlDataAdapter(command);
			DataSet tempSet = new DataSet();
			adapter.Fill(tempSet);

			// If there was no data for this collection and entry point, return null (an ERROR occurred)
			if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
			{
				return null;
			}

			// Return the first table from the returned dataset
			return tempSet.Tables[0];
		}


		/// <summary> Gets the dataset of all public items and item groups </summary>
		/// <param name="Include_Private"> Flag indicates whether to include private items in this list </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Dataset of all items and item groups </returns>
		/// <remarks> This calls the 'SobekCM_Item_List_Brief2' stored procedure </remarks> 
		public static DataSet Get_Item_List( bool Include_Private, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Item_List", String.Empty);
			}

			// Define a temporary dataset
			SqlConnection connection = new SqlConnection(connectionString + "Connection Timeout=45");
			SqlCommand command = new SqlCommand("SobekCM_Item_List_Brief2", connection)
									 {CommandTimeout = 45, CommandType = CommandType.StoredProcedure};
			command.Parameters.AddWithValue("@include_private", Include_Private);
			SqlDataAdapter adapter = new SqlDataAdapter(command);
			DataSet tempSet = new DataSet();
			adapter.Fill(tempSet);
				
			// If there was no data for this collection and entry point, return null (an ERROR occurred)
			if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
			{
				return null;
			}

			// Return the first table from the returned dataset
			return tempSet;
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
                Tracer.Add_Trace("Engine_Database.Get_IP_Restriction_Range", "Pulls all the IP restriction range information");
            }

            try
            {

                // Create the dataset to fill (could also do a data reader, but we'll do a datatable)
                DataSet fillSet = new DataSet("IP_Restriction_Ranges");

                // Open the SQL connection
                using (SqlConnection sqlConnect = new SqlConnection(Connection_String))
                {
                    try
                    {
                        sqlConnect.Open();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Unable to open connection to the database." + Environment.NewLine + ex.Message, ex);
                    }

                    // Create the sql command / stored procedure
                    SqlCommand cmd = new SqlCommand("SobekCM_Get_All_IP_Restrictions");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = sqlConnect;


                    // Fill the dataset
                    try
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        dataAdapter.Fill(fillSet);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Unable to pull data from the Get_SnailKite_Details procedure in the database." + Environment.NewLine + ex.Message, ex);
                    }


                    // Close the connection (not technical necessary since we put the connection in the
                    // scope of the using brackets.. it would dispose itself anyway)
                    try
                    {
                        sqlConnect.Close();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Unable to close connection to the database." + Environment.NewLine + ex.Message, ex);
                    }
                }

                // Was there a match?
                if ((fillSet.Tables.Count == 0) || (fillSet.Tables[0].Rows.Count == 0))
                    return null;

                // Return the fill set
                return fillSet.Tables[0];
            }
            catch (Exception ee)
            {
                lastException = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Get_IP_Restriction_Ranges", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_IP_Restriction_Ranges", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_IP_Restriction_Ranges", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }


        /// <summary> Gets the details for a single IP restriction ranges, including each single IP and the complete notes associated with the range </summary>
        /// <param name="PrimaryID"> Primary id to the IP restriction range to pull details for </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> DataTable with all the data about the IP ranges used for restrictions </returns>
        /// <remarks> This calls the 'SobekCM_Get_IP_Restriction_Range' stored procedure </remarks> 
        public static DataSet Get_IP_Restriction_Range_Details(int PrimaryID, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_IP_Restriction_Range_Details", "Pulls all the IP restriction range details for range #" + PrimaryID);
            }

            try
           

            {
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@ip_rangeid", PrimaryID);

                // Define a temporary dataset
                DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_IP_Restriction_Range", parameters);

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
                lastException = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Get_IP_Restriction_Range_Details", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_IP_Restriction_Range_Details", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_IP_Restriction_Range_Details", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

		/// <summary> Deletes a single IP address information from an IP restriction range </summary>
		/// <param name="PrimaryID"> Primary key for this single IP address information to delete </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Delete_Single_IP' stored procedure </remarks> 
		public static bool Delete_Single_IP(int PrimaryID, Custom_Tracer Tracer )
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Delete_Single_IP", "Delete single IP information within a range");
			}

			try
			{
				SqlParameter[] parameters = new SqlParameter[1];
				parameters[0] = new SqlParameter("@ip_singleid", PrimaryID);

				// Define a temporary dataset
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Delete_Single_IP", parameters);

				// Return the first table from the returned dataset
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Delete_Single_IP", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Single_IP", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Single_IP", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Adds or edits a single IP address information in an IP restriction range </summary>
		/// <param name="PrimaryID"> Primary key for this single IP address information to add, or -1 to add a new IP address </param>
		/// <param name="IP_RangeID"> Primary key for the IP restriction range to add this single IP address information </param>
		/// <param name="Start_IP"> Beginning of the IP range, or the complete IP address </param>
		/// <param name="End_IP"> End of the IP range, if this was a true range </param>
		/// <param name="Note"> Any note associated with this single IP information </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Primary key for the single IP address information, if no primary key was originally provided </returns>
		/// <remarks> This calls the 'SobekCM_Edit_Single_IP' stored procedure </remarks> 
		public static int Edit_Single_IP(int PrimaryID, int IP_RangeID, string Start_IP, string End_IP, string Note, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Edit_Single_IP", "Edit a single IP within a restriction range");
			}

			try
			{
				SqlParameter[] parameters = new SqlParameter[6];
				parameters[0] = new SqlParameter("@ip_singleid", PrimaryID);
				parameters[1] = new SqlParameter("@ip_rangeid", IP_RangeID );
				parameters[2] = new SqlParameter("@startip", Start_IP);
				parameters[3] = new SqlParameter("@endip", End_IP);
				parameters[4] = new SqlParameter("@notes", Note );
				parameters[5] = new SqlParameter("@new_ip_singleid", -1) {Direction = ParameterDirection.InputOutput};

				// Define a temporary dataset
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Edit_Single_IP", parameters);

				// Return the first table from the returned dataset
				return Convert.ToInt32(parameters[5].Value);
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Edit_Single_IP", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Edit_Single_IP", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Edit_Single_IP", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return -1;
			}
		}


		/// <summary> Edits an existing IP restriction range, or adds a new one </summary>
		/// <param name="IP_RangeID"> Primary key for the IP restriction range  </param>
		/// <param name="Title"> Title for this IP Restriction Range </param>
		/// <param name="Notes"> Notes about this IP Restriction Range (for system admins)</param>
		/// <param name="Item_Restricted_Statement"> Statement used when a user directly requests an item for which they do not the pre-requisite access </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Edit_IP_Range' stored procedure </remarks> 
		public static bool Edit_IP_Range(int IP_RangeID, string Title, string Notes, string Item_Restricted_Statement, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Edit_IP_Range", "Edit an existing IP restriction range");
			}

			try
			{
				SqlParameter[] parameters = new SqlParameter[4];
				parameters[0] = new SqlParameter("@rangeid", IP_RangeID);
				parameters[1] = new SqlParameter("@title", Title);
				parameters[2] = new SqlParameter("@notes", Notes);
				parameters[3] = new SqlParameter("@not_valid_statement", Item_Restricted_Statement);

				// Execute the stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Edit_IP_Range", parameters);

				// Return true if successful
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Edit_IP_Range", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Edit_IP_Range", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Edit_IP_Range", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

        /// <summary> Delete a complete IP range group </summary>
        /// <param name="IdToDelete"> Primary key of the IP range to delete </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Delete_IP_Range' stored procedure </remarks> 
        public static bool Delete_IP_Range(int IdToDelete, Custom_Tracer Tracer)
	    {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Delete_IP_Range", "Delete an existing IP restriction range");
            }

            try
            {
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@rangeid", IdToDelete);

                // Execute the stored procedure
                SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Delete_IP_Range", parameters);

                // Return true if successful
                return true;
            }
            catch (Exception ee)
            {
                lastException = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Delete_IP_Range", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Delete_IP_Range", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Delete_IP_Range", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return false;
            }
	    }

		#endregion

		#region Methods to get authority type information

		/// <summary> Gets the list of all map features linked to a particular item  </summary>
		/// <param name="ItemID"> ItemID for the item of interest</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> List of all features linked to the item of interest </returns>
		/// <remarks> This calls the 'Auth_Get_All_Features_By_Item' stored procedure </remarks> 
		public static Map_Features_DataSet Get_All_Features_By_Item(int ItemID, Custom_Tracer Tracer)
		{
			try
			{
				// Create the connection
				SqlConnection connect = new SqlConnection( connectionString );

				// Create the command 
				SqlCommand executeCommand = new SqlCommand("Auth_Get_All_Features_By_Item", connect)
												{CommandType = CommandType.StoredProcedure};
				executeCommand.Parameters.AddWithValue( "@itemid", ItemID );
				executeCommand.Parameters.AddWithValue( "@filter", 1 );

				// Create the adapter
				SqlDataAdapter adapter = new SqlDataAdapter( executeCommand );

				// Add appropriate table mappings
				adapter.TableMappings.Add("Table", "Features");
				adapter.TableMappings.Add("Table1", "Types");

				// Fill the strongly typed dataset
				Map_Features_DataSet features = new Map_Features_DataSet();
				adapter.Fill( features );

				// Return the fully built object
				return features;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_All_Features_By_Item", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_All_Features_By_Item", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_All_Features_By_Item", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Gets the list of all streets linked to a particular item  </summary>
		/// <param name="ItemID"> ItemID for the item of interest</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> List of all streets linked to the item of interest </returns>
		/// <remarks> This calls the 'Auth_Get_All_Streets_By_Item' stored procedure </remarks> 
		public static Map_Streets_DataSet Get_All_Streets_By_Item(int ItemID, Custom_Tracer Tracer)
		{
			try
			{
				// Create the connection
				SqlConnection connect = new SqlConnection( connectionString );

				// Create the command 
				SqlCommand executeCommand = new SqlCommand("Auth_Get_All_Streets_By_Item", connect)
												{CommandType = CommandType.StoredProcedure};
				executeCommand.Parameters.AddWithValue( "@itemid", ItemID );

				// Create the adapter
				SqlDataAdapter adapter = new SqlDataAdapter( executeCommand );

				// Add appropriate table mappings
				adapter.TableMappings.Add("Table", "Streets");


				// Fill the strongly typed dataset
				Map_Streets_DataSet streets = new Map_Streets_DataSet();
				adapter.Fill( streets );

				// Return the fully built object
				return streets;
			}
			catch ( Exception ee )
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_All_Streets_By_Item", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_All_Streets_By_Item", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_All_Streets_By_Item", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}


		#endregion

		#region My Sobek database calls

		/// <summary> Saves information about a single user </summary>
		/// <param name="User"> <see cref="Users.User_Object"/> with all the information about the single user</param>
		/// <param name="Password"> Plain-text password, which is then encrypted prior to saving</param>
        /// <param name="Authentication_Type"> String which indicates the type of authentication utilized, only important if this is the first time this user has authenticated/registered </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE</returns>
		/// <remarks> This calls the 'mySobek_Save_User' stored procedure</remarks> 
		public static bool Save_User(User_Object User, string Password, User_Authentication_Type_Enum Authentication_Type, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Save_User", String.Empty);
			}

			const string SALT = "This is my salt to add to the password";
			string encryptedPassword = SecurityInfo.SHA1_EncryptString(Password + SALT);

            string auth_string = String.Empty;
            if (Authentication_Type == User_Authentication_Type_Enum.Sobek)
                auth_string = "sobek";
            if (Authentication_Type == User_Authentication_Type_Enum.Shibboleth)
                auth_string = "shibboleth";
            if ((Authentication_Type == User_Authentication_Type_Enum.Windows) || (Authentication_Type == User_Authentication_Type_Enum.LDAP))
                auth_string = "ldap";

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[24];
				paramList[0] = new SqlParameter("@userid", User.UserID);
				paramList[1] = new SqlParameter("@shibbid", User.ShibbID);
				paramList[2] = new SqlParameter("@username", User.UserName);
				paramList[3] = new SqlParameter("@password", encryptedPassword);
				paramList[4] = new SqlParameter("@emailaddress", User.Email);
				paramList[5] = new SqlParameter("@firstname", User.Given_Name);
				paramList[6] = new SqlParameter("@lastname", User.Family_Name);
				paramList[7] = new SqlParameter("@cansubmititems", User.Can_Submit);
				paramList[8] = new SqlParameter("@nickname", User.Nickname);
				paramList[9] = new SqlParameter("@organization", User.Organization);
				paramList[10] = new SqlParameter("@college", User.College);
				paramList[11] = new SqlParameter("@department", User.Department);
				paramList[12] = new SqlParameter("@unit", User.Unit);
				paramList[13] = new SqlParameter("@rights", User.Default_Rights);
				paramList[14] = new SqlParameter("@sendemail", User.Send_Email_On_Submission);
				paramList[15] = new SqlParameter("@language", User.Preferred_Language);
				if (User.Templates.Count > 0)
				{
					paramList[16] = new SqlParameter("@default_template", User.Templates[0]);
				}
				else
				{
					paramList[16] = new SqlParameter("@default_template", String.Empty);
				}
				if (User.Default_Metadata_Sets.Count > 0)
				{
					paramList[17] = new SqlParameter("@default_metadata", User.Default_Metadata_Sets[0]);
				}
				else
				{
					paramList[17] = new SqlParameter("@default_metadata", String.Empty);
				}
				paramList[18] = new SqlParameter("@organization_code", User.Organization_Code);
				paramList[19] = new SqlParameter("@receivestatsemail", User.Receive_Stats_Emails);
                paramList[20] = new SqlParameter("@scanningtechnician", User.Scanning_Technician);
                paramList[21] = new SqlParameter("@processingtechnician", User.Processing_Technician);
                paramList[22] = new SqlParameter("@internalnotes", User.Internal_Notes);
                paramList[23] = new SqlParameter("@authentication", auth_string);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Save_User", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Save_User", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_User", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_User", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}


		}

		/// <summary> Links a single user to a user group  </summary>
		/// <param name="UserID"> Primary key for the user </param>
		/// <param name="UserGroupID"> Primary key for the user group </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'mySobek_Link_User_To_User_Group' stored procedure</remarks> 
		public static bool Link_User_To_User_Group( int UserID, int UserGroupID )
		{
			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[2];
				paramList[0] = new SqlParameter("@userid", UserID);
				paramList[1] = new SqlParameter("@usergroupid", UserGroupID);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Link_User_To_User_Group", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		/// <summary> Change an existing user's password </summary>
		/// <param name="Username"> Username for the user </param>
		/// <param name="CurrentPassword"> Old plain-text password, which is then encrypted prior to saving</param>
		/// <param name="NewPassword"> New plain-text password, which is then encrypted prior to saving</param>
		/// <param name="IsTemporary"> Flag indicates if the new password is temporary and must be changed on the next logon</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE</returns>
		/// <remarks> This calls the 'mySobek_Change_Password' stored procedure</remarks> 
		public static bool Change_Password(string Username, string CurrentPassword, string NewPassword, bool IsTemporary,  Custom_Tracer Tracer )
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Change_Password", String.Empty);
			}

			const string SALT = "This is my salt to add to the password";
			string encryptedCurrentPassword = SecurityInfo.SHA1_EncryptString(CurrentPassword + SALT);
			string encryptedNewPassword = SecurityInfo.SHA1_EncryptString(NewPassword + SALT);
			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[5];
				paramList[0] = new SqlParameter("@username", Username);
				paramList[1] = new SqlParameter("@current_password", encryptedCurrentPassword);
				paramList[2] = new SqlParameter("@new_password", encryptedNewPassword);
				paramList[3] = new SqlParameter("@isTemporaryPassword", IsTemporary);
				paramList[4] = new SqlParameter("@password_changed", false) {Direction = ParameterDirection.InputOutput};

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Change_Password", paramList);


				return Convert.ToBoolean(paramList[4].Value);
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Change_Password", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Change_Password", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Change_Password", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}

		}

		/// <summary> Checks to see if a username or email exist </summary>
		/// <param name="UserName"> Username to check</param>
		/// <param name="Email"> Email address to check</param>
		/// <param name="UserName_Exists"> [OUT] Flag indicates if the username exists</param>
		/// <param name="Email_Exists"> [OUT] Flag indicates if the email exists </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE</returns>
		/// <remarks> This calls the 'mySobek_UserName_Exists' stored procedure<br /><br />
		/// This is used to enforce uniqueness during registration </remarks> 
		public static bool UserName_Exists(string UserName, string Email, out bool UserName_Exists, out bool Email_Exists, Custom_Tracer Tracer )
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.UserName_Exists", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[4];
				paramList[0] = new SqlParameter("@username", UserName);
				paramList[1] = new SqlParameter("@email", Email);
				paramList[2] = new SqlParameter("@UserName_Exists", true) {Direction = ParameterDirection.InputOutput};
				paramList[3] = new SqlParameter("@Email_Exists", true) {Direction = ParameterDirection.InputOutput};

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_UserName_Exists", paramList);

				UserName_Exists = Convert.ToBoolean(paramList[2].Value);
				Email_Exists = Convert.ToBoolean(paramList[3].Value);
				return true;
			}
			catch ( Exception ee )
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.UserName_Exists", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.UserName_Exists", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.UserName_Exists", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				UserName_Exists = true;
				Email_Exists = true;
				return false;
			}
		}

		/// <summary> Updates the flag that indicates the user would like to receive a monthly usage statistics email </summary>
		/// <param name="UserID"> Primary key for this user in the database </param>
		/// <param name="New_Flag"> New value for the flag </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE</returns>
		/// <remarks> This calls the 'mySobek_Set_Receive_Stats_Email_Flag' stored procedure</remarks> 
		public static bool Set_User_Receive_Stats_Email( int UserID, bool New_Flag, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Set_Receive_Stats_Email_Flag", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[2];
				paramList[0] = new SqlParameter("@userid", UserID);
				paramList[1] = new SqlParameter("@newflag", New_Flag);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Set_Receive_Stats_Email_Flag", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Set_Receive_Stats_Email_Flag", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Set_Receive_Stats_Email_Flag", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Set_Receive_Stats_Email_Flag", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Gets basic user information by UserID </summary>
		/// <param name="UserID"> Primary key for this user in the database </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Fully built <see cref="Users.User_Object"/> object </returns>
		/// <remarks> This calls the 'mySobek_Get_User_By_UserID' stored procedure<br /><br />
		/// This is called when a user's cookie exists in a web request</remarks> 
		public static User_Object Get_User(int UserID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_User", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@userid", UserID);

				DataSet resultSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "mySobek_Get_User_By_UserID", paramList);

				if ((resultSet.Tables.Count > 0) && (resultSet.Tables[0].Rows.Count > 0))
				{
					return build_user_object_from_dataset(resultSet);
				}

				// Return the browse id
				return null;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_User", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_User", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_User", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Gets basic user information by the Shibboleth-provided user identifier </summary>
		/// <param name="Shibboleth_ID"> Shibboleth ID (UFID) for the user </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Fully built <see cref="Users.User_Object"/> object </returns>
		/// <remarks> This calls the 'mySobek_Get_User_By_UFID' stored procedure<br /><br />
		/// This method is called when user's logon through the Gatorlink Shibboleth service</remarks> 
		public static User_Object Get_User(string Shibboleth_ID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_User", String.Empty);
			}

			try
			{

				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@shibbid", Shibboleth_ID);

				DataSet resultSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "mySobek_Get_User_By_ShibbID", paramList);

				if ((resultSet.Tables.Count > 0) && (resultSet.Tables[0].Rows.Count > 0))
				{
					return build_user_object_from_dataset(resultSet);
				}

				// Return the browse id
				return null;

			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_User", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_User", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_User", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Gets basic user information by Username (or email) and Password </summary>
		/// <param name="UserName"> UserName (or email address) for the user </param>
		/// <param name="Password"> Plain-text password, which is then encrypted prior to sending to database</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Fully built <see cref="Users.User_Object"/> object </returns>
		/// <remarks> This calls the 'mySobek_Get_User_By_UserName_Password' stored procedure<br /><br />
		/// This is used when a user logs on through the mySobek authentication</remarks> 
		public static User_Object Get_User(string UserName, string Password, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_User", String.Empty);
			}

			try
			{
				const string SALT = "This is my salt to add to the password";
				string encryptedPassword = SecurityInfo.SHA1_EncryptString(Password + SALT);


				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[2];
				paramList[0] = new SqlParameter("@username", UserName);
				paramList[1] = new SqlParameter("@password", encryptedPassword);

				DataSet resultSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "mySobek_Get_User_By_UserName_Password", paramList);

				if ((resultSet.Tables.Count > 0) && (resultSet.Tables[0].Rows.Count > 0))
				{
					return build_user_object_from_dataset(resultSet);
				}

				// Return the browse id
				return null;

			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_User", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_User", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_User", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		private static User_Object build_user_object_from_dataset(DataSet ResultSet )
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

			user.Items_Submitted_Count = ResultSet.Tables[3 ].Rows.Count;
			foreach (DataRow thisRow in ResultSet.Tables[3 ].Rows)
			{
				if (!user.BibIDs.Contains(thisRow["BibID"].ToString().ToUpper()))
					user.Add_BibID(thisRow["BibID"].ToString().ToUpper());
			}

			// Add links to regular expressions
			foreach (DataRow thisRow in ResultSet.Tables[4 ].Rows)
			{
				user.Add_Editable_Regular_Expression(thisRow["EditableRegex"].ToString());
			}

			// Add links to aggregationPermissions
			foreach (DataRow thisRow in ResultSet.Tables[5 ].Rows)
			{

				user.Add_Aggregation(thisRow["Code"].ToString(), thisRow["Name"].ToString(), Convert.ToBoolean(thisRow["CanSelect"]), Convert.ToBoolean(thisRow["CanEditMetadata"]), Convert.ToBoolean(thisRow["CanEditBehaviors"]), Convert.ToBoolean(thisRow["CanPerformQc"]), Convert.ToBoolean(thisRow["CanUploadFiles"]), Convert.ToBoolean(thisRow["CanChangeVisibility"]), Convert.ToBoolean(thisRow["CanDelete"]), Convert.ToBoolean(thisRow["IsCollectionManager"]), Convert.ToBoolean(thisRow["OnHomePage"]), Convert.ToBoolean(thisRow["IsAggregationAdmin"]), Convert.ToBoolean(thisRow["GroupDefined"]));
				
			}

			// Add the current folder names
			Dictionary<int, User_Folder> folderNodes = new Dictionary<int, User_Folder>();
			List<User_Folder> parentNodes = new List<User_Folder>();
			foreach (DataRow folderRow in ResultSet.Tables[6 ].Rows)
			{
				string folderName = folderRow["FolderName"].ToString();
				int folderid = Convert.ToInt32(folderRow["UserFolderID"]);
				int parentid = Convert.ToInt32(folderRow["ParentFolderID"]);
				bool isPublic = Convert.ToBoolean(folderRow["isPublic"]);

				User_Folder newFolderNode = new User_Folder(folderName, folderid) {IsPublic = isPublic};
				if (parentid == -1)
					parentNodes.Add(newFolderNode);
				folderNodes.Add(folderid, newFolderNode);
			}
			foreach (DataRow folderRow in ResultSet.Tables[6 ].Rows)
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
			foreach (DataRow itemRow in ResultSet.Tables[7 ].Rows)
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

		/// <summary> Add a link between a user and an existing item group (by GroupID) </summary>
		/// <param name="UserID"> Primary key for this user in the database </param>
		/// <param name="GroupID"> Primary key for the item group to link this user to</param>
		/// <returns>TRUE if successful, otherwise FALSE</returns>
		/// <remarks> This calls the 'mySobek_Link_User_To_Item' stored procedure</remarks> 
		public static bool Add_User_BibID_Link(int UserID, int GroupID)
		{
			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[2];
				paramList[0] = new SqlParameter("@userid", UserID);
				paramList[1] = new SqlParameter("@groupid", GroupID);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Link_User_To_Item", paramList);

				// Return the browse id
				return true;

			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		/// <summary> Add a link between a user and an existing item and include the type of relationship </summary>
		/// <param name="UserID"> Primary key for this user in the database </param>
		/// <param name="ItemID"> Primary key for the item to link this user to</param>
		/// <param name="RelationshipID"> Primary key for the type of relationship to use </param>
		/// <param name="Change_Existing"> If a relationship already exists, should this override it? </param>
		/// <returns>TRUE if successful, otherwise FALSE</returns>
		/// <remarks> This calls the 'SobekCM_Link_User_To_Item' stored procedure</remarks> 
		public static bool Add_User_Item_Link(int UserID, int ItemID, int RelationshipID, bool Change_Existing )
		{
			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[4];
				paramList[0] = new SqlParameter("@itemid", ItemID);
				paramList[1] = new SqlParameter("@userid", UserID);
				paramList[2] = new SqlParameter("@relationshipid", RelationshipID);
				paramList[3] = new SqlParameter("@change_existing", Change_Existing);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Link_User_To_Item", paramList);

				// Return the browse id
				return true;

			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}
		
		/// <summary> Gets basic information about all the folders and searches saved for a single user </summary>
		/// <param name="UserID"> Primary key for this user in the database </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Information about all folders (and number of items) and saved searches for a user </returns>
		/// <remarks> This calls the 'mySobek_Get_Folder_Search_Information' stored procedure</remarks> 
		public static DataSet Get_Folder_Search_Information(int UserID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Folder_Search_Information", String.Empty);
			}

			try
			{

				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@userid", UserID);

				DataSet resultSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "mySobek_Get_Folder_Search_Information", paramList);

				return resultSet;

			}
			catch ( Exception ee )
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_Folder_Search_Information", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Folder_Search_Information", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Folder_Search_Information", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Deletes a user search from the collection of saved searches </summary>
		/// <param name="UserSearchID"> Primary key for this saved search </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'mySobek_Delete_User_Search' stored procedure</remarks> 
		public static bool Delete_User_Search(int UserSearchID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Delete_User_Search", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@usersearchid", UserSearchID);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Delete_User_Search", paramList);

				// Return TRUE
				return true;

			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Delete_User_Search", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_User_Search", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_User_Search", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Gets the list of all saved user searches and any user comments </summary>
		/// <param name="UserID"> Primary key for this user in the database </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Table of all the saved searches for this user </returns>
		/// <remarks> This calls the 'mySobek_Get_User_Searches' stored procedure</remarks> 
		public static DataTable Get_User_Searches(int UserID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_User_Searches", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@userid", UserID);

				DataSet resultSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "mySobek_Get_User_Searches", paramList);

				return resultSet.Tables[0];

			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_User_Searches", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_User_Searches", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_User_Searches", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}

		/// <summary> Saves a search to the user's saved searches </summary>
		/// <param name="UserID"> Primary key for this user in the database </param>
		/// <param name="Search_URL"> SobekCM search URL </param>
		/// <param name="Search_Description"> Programmatic description of this search</param>
		/// <param name="ItemOrder"> Order for this search within the folder</param>
		/// <param name="UserNotes"> Notes from the user about this search </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> New UserSearchID, or -1 if this edits an existing one </returns>
		/// <remarks> This calls the 'mySobek_Save_User_Search' stored procedure</remarks> 
		public static int Save_User_Search(int UserID, string Search_URL, string Search_Description, int ItemOrder, string UserNotes, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Save_User_Search", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[6];
				paramList[0] = new SqlParameter("@userid", UserID);
				paramList[1] = new SqlParameter("@searchurl", Search_URL);
				paramList[2] = new SqlParameter("@searchdescription", Search_Description);
				paramList[3] = new SqlParameter("@itemorder", ItemOrder);
				paramList[4] = new SqlParameter("@usernotes", UserNotes);
				paramList[5] = new SqlParameter("@new_usersearchid", -1) {Direction = ParameterDirection.InputOutput};

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Save_User_Search", paramList);


				// Return TRUE
				return Convert.ToInt32(paramList[5].Value);

			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Save_User_Search", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_User_Search", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_User_Search", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return -1000;
			}
		}

		/// <summary> Remove an item from the user's folder </summary>
		/// <param name="UserID"> Primary key for this user in the database </param>
		/// <param name="FolderName"> Name of this user's folder </param>
		/// <param name="BibID"> Bibliographic identifier for this title / item group </param>
		/// <param name="VID"> Volume identifier for this one volume within a title / item group </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'mySobek_Delete_Item_From_User_Folder' stored procedure</remarks> 
		public static bool Delete_Item_From_User_Folder(int UserID, string FolderName, string BibID, string VID, Custom_Tracer Tracer )
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Delete_Item_From_User_Folder", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[4];
				paramList[0] = new SqlParameter("@userid", UserID);
				paramList[1] = new SqlParameter("@foldername", FolderName);
				paramList[2] = new SqlParameter("@bibid", BibID);
				paramList[3] = new SqlParameter("@vid", VID);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Delete_Item_From_User_Folder", paramList);

				// Return TRUE
				return true;

			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Delete_Item_From_User_Folder", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Item_From_User_Folder", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Item_From_User_Folder", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Remove an item from any user folder it currently resides in (besides Submitted Items)</summary>
		/// <param name="UserID"> Primary key for this user in the database </param>
		/// <param name="BibID"> Bibliographic identifier for this title / item group </param>
		/// <param name="VID"> Volume identifier for this one volume within a title / item group </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'mySobek_Delete_Item_From_All_User_Folders' stored procedure</remarks> 
		public static bool Delete_Item_From_User_Folders(int UserID, string BibID, string VID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Delete_Item_From_User_Folder", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[3];
				paramList[0] = new SqlParameter("@userid", UserID);
				paramList[1] = new SqlParameter("@bibid", BibID);
				paramList[2] = new SqlParameter("@vid", VID);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Delete_Item_From_All_User_Folders", paramList);

				// Return TRUE
				return true;

			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Delete_Item_From_User_Folders", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Item_From_User_Folders", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Item_From_User_Folders", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Adds a digital resource to a user folder, or edits an existing item </summary>
		/// <param name="UserID"> Primary key for this user in the database </param>
		/// <param name="FolderName"> Name of this user's folder </param>
		/// <param name="BibID"> Bibliographic identifier for this title / item group </param>
		/// <param name="VID"> Volume identifier for this one volume within a title / item group </param>
		/// <param name="ItemOrder"> Order for this item within the folder</param>
		/// <param name="UserNotes"> Notes from the user about this item </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'mySobek_Add_Item_To_User_Folder' stored procedure</remarks> 
		public static bool Add_Item_To_User_Folder(int UserID, string FolderName, string BibID, string VID, int ItemOrder, string UserNotes, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Add_Item_To_User_Folder", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[6];
				paramList[0] = new SqlParameter("@userid", UserID);
				paramList[1] = new SqlParameter("@foldername", FolderName);
				paramList[2] = new SqlParameter("@bibid", BibID);
				paramList[3] = new SqlParameter("@vid", VID);
				paramList[4] = new SqlParameter("@itemorder", ItemOrder);
				paramList[5] = new SqlParameter("@usernotes", UserNotes);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Add_Item_To_User_Folder", paramList);

				// Return TRUE
				return true;

			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Add_Item_To_User_Folder", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Add_Item_To_User_Folder", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Add_Item_To_User_Folder", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}



		/// <summary> Deletes a folder from a user </summary>
		/// <param name="UserID"> Primary key for this user from the database</param>
		/// <param name="UserFolderID"> Primary key for this folder from the database</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'mySobek_Delete_User_Folder' stored procedure</remarks> 
		public static bool Delete_User_Folder(int UserID, int UserFolderID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Delete_User_Folder", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[2];
				paramList[0] = new SqlParameter("@userfolderid", UserFolderID);
				paramList[1] = new SqlParameter("@userid", UserID);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Delete_User_Folder", paramList);

				// Return TRUE
				return true;

			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Delete_User_Folder", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_User_Folder", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_User_Folder", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Edit an existing user folder, or add a new user folder </summary>
		/// <param name="UserFolderID"> Primary key for the folder, if this is an edit, otherwise -1</param>
		/// <param name="UserID"> Primary key for this user from the database</param>
		/// <param name="ParentFolderID"> Key for the parent folder for this new folder</param>
		/// <param name="FolderName"> Name for this new folder</param>
		/// <param name="IsPublic"> Flag indicates if this folder is public </param>
		/// <param name="Description"> Description for this folder </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Primary key for this new folder, or -1 if an error occurred </returns>
		/// <remarks> This calls the 'mySobek_Edit_User_Folder' stored procedure</remarks> 
		public static int Edit_User_Folder(int UserFolderID, int UserID, int ParentFolderID, string FolderName, bool IsPublic, string Description, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Edit_User_Folder", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[7];
				paramList[0] = new SqlParameter("@userfolderid", UserFolderID);
				paramList[1] = new SqlParameter("@userid", UserID);
				paramList[2] = new SqlParameter("@parentfolderid", ParentFolderID);
				paramList[3] = new SqlParameter("@foldername", FolderName);
				paramList[4] = new SqlParameter("@is_public", IsPublic);
				paramList[5] = new SqlParameter("@description", Description);
				paramList[6] = new SqlParameter("@new_folder_id", 0) {Direction = ParameterDirection.InputOutput};

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Edit_User_Folder", paramList);

				// Return TRUE
				return Convert.ToInt32(paramList[6].Value);

			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Edit_User_Folder", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Edit_User_Folder", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Edit_User_Folder", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return -1;
			}
		}

		/// <summary> Sets the flag indicating an aggregation should appear on a user's home page </summary>
		/// <param name="UserID"> Primary key for the user</param>
		/// <param name="AggregationID"> Primary key for the aggregation </param>
		/// <param name="NewFlag"> New flag indicates if this should be on the home page </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'mySobek_Set_Aggregation_Home_Page_Flag' stored procedure</remarks> 
		public static bool User_Set_Aggregation_Home_Page_Flag(int UserID, int AggregationID, bool NewFlag, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.User_Set_Aggregation_Home_Page_Flag", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[3];
				paramList[0] = new SqlParameter("@userid", UserID);
				paramList[1] = new SqlParameter("@aggregationid", AggregationID);
				paramList[2] = new SqlParameter("@onhomepage", NewFlag);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Set_Aggregation_Home_Page_Flag", paramList);

				// Return TRUE
				return true;

			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.User_Set_Aggregation_Home_Page_Flag", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.User_Set_Aggregation_Home_Page_Flag", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.User_Set_Aggregation_Home_Page_Flag", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Gets the information about a folder which should be public </summary>
		/// <param name="UserFolderID"> ID for the user folder to retrieve </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Built public user folder regardless if it is public or not.  A non-public folder will only be populated with FALSE for the isPublic field. </returns>
		/// <remarks> This calls the 'mySobek_Get_Folder_Information' stored procedure</remarks> 
		public static Public_User_Folder Get_Public_User_Folder(int UserFolderID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Public_User_Folder", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@folderid", UserFolderID);

				DataSet resultSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "mySobek_Get_Folder_Information", paramList);

				// Build the returnvalue
				if ((resultSet == null) || (resultSet.Tables.Count == 0) || (resultSet.Tables[0].Rows.Count == 0))
					return new Public_User_Folder(false);

				// Check that it is really public
				bool isPublic = Convert.ToBoolean(resultSet.Tables[0].Rows[0]["isPublic"]);
				if ( !isPublic )
					return new Public_User_Folder(false);

				// Pull out the row and all the values
				DataRow thisRow = resultSet.Tables[0].Rows[0];
				string folderName = thisRow["FolderName"].ToString();
				string folderDescription = thisRow["FolderDescription"].ToString();
				int userID = Convert.ToInt32(thisRow["UserID"]);
				string firstName = thisRow["FirstName"].ToString();
				string lastName = thisRow["LastName"].ToString();
				string nickname = thisRow["NickName"].ToString();
				string email = thisRow["EmailAddress"].ToString();               

				// Return the folder object
				Public_User_Folder returnValue = new Public_User_Folder(UserFolderID, folderName, folderDescription, userID, firstName, lastName, nickname, email, true);
				return returnValue;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_Public_User_Folder", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Public_User_Folder", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_Public_User_Folder", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}


		/// <summary> Gets the information about a single user group </summary>
		/// <param name="UserGroupID"> Primary key for this user group from the database </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Fully built <see cref="Users.User_Group"/> object </returns>
		/// <remarks> This calls the 'mySobek_Get_User_Group' stored procedure </remarks> 
		public static User_Group Get_User_Group(int UserGroupID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_User_Group", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@usergroupid", UserGroupID);

				DataSet resultSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "mySobek_Get_User_Group", paramList);

				if ((resultSet.Tables.Count > 0) && (resultSet.Tables[0].Rows.Count > 0))
				{


					DataRow userRow = resultSet.Tables[0].Rows[0];
					string name = userRow["GroupName"].ToString();
					string description = userRow["GroupDescription"].ToString();
					int usergroupid = Convert.ToInt32(userRow["UserGroupID"]);
					User_Group group = new User_Group(name, description, usergroupid);
					group.CanSubmit = Convert.ToBoolean(userRow["Can_Submit_Items"]);
					group.IsInternalUser = Convert.ToBoolean(userRow["Internal_User"]);
					group.IsSystemAdmin = Convert.ToBoolean(userRow["IsSystemAdmin"]);

					foreach (DataRow thisRow in resultSet.Tables[1].Rows)
					{
						group.Add_Template(thisRow["TemplateCode"].ToString());
					}

					foreach (DataRow thisRow in resultSet.Tables[2].Rows)
					{
						group.Add_Default_Metadata_Set(thisRow["MetadataCode"].ToString());
					}

					// Add links to regular expressions
					foreach (DataRow thisRow in resultSet.Tables[3].Rows)
					{
						group.Add_Editable_Regular_Expression(thisRow["EditableRegex"].ToString());
					}

					// Add links to aggregationPermissions
					foreach (DataRow thisRow in resultSet.Tables[4].Rows)
					{
						group.Add_Aggregation(thisRow["Code"].ToString(), thisRow["Name"].ToString(), Convert.ToBoolean(thisRow["CanSelect"]), Convert.ToBoolean(thisRow["CanEditMetadata"]), Convert.ToBoolean(thisRow["CanEditBehaviors"]), Convert.ToBoolean(thisRow["CanPerformQc"]), Convert.ToBoolean(thisRow["CanUploadFiles"]), Convert.ToBoolean(thisRow["CanChangeVisibility"]), Convert.ToBoolean(thisRow["CanDelete"]), Convert.ToBoolean(thisRow["IsCurator"]), Convert.ToBoolean(thisRow["IsAdmin"]));
					}

					// Add the basic information about users in this user group
					foreach (DataRow thisRow in resultSet.Tables[5].Rows)
					{
						int userid = Convert.ToInt32(thisRow["UserID"]);
						string username = thisRow["UserName"].ToString();
						string email = thisRow["EmailAddress"].ToString();
						string firstname = thisRow["FirstName"].ToString();
						string nickname = thisRow["NickName"].ToString();
						string lastname = thisRow["LastName"].ToString();
						string fullname = firstname + " " + lastname;
						if (nickname.Length > 0)
						{
							fullname = nickname + " " + lastname;
						}

						group.Add_User(username, fullname, email, userid);
					}

					return group;
				}

				// Return NULL if there was an error
				return null;

			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Get_User_Group", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_User_Group", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Get_User_Group", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}


 

		#endregion

		#region Methods used in support of semi-public descriptive tagging

		/// <summary> Adds a descriptive tag to an existing item by a logged-in user </summary>
		/// <param name="UserID"> Primary key for the user adding the descriptive tag </param>
		/// <param name="TagID"> Primary key for a descriptive tag, if this is an edit </param>
		/// <param name="ItemID"> Primary key for the digital resource to tag </param>
		/// <param name="Added_Description"> User-entered descriptive tag </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> New tag id if this is a new descriptive tag </returns>
		/// <remarks> This calls the 'mySobek_Add_Description_Tag' stored procedure</remarks> 
		public static int Add_Description_Tag(int UserID, int TagID, int ItemID, string Added_Description, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Add_Description_Tag", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[5];
				paramList[0] = new SqlParameter("@UserID", UserID);
				paramList[1] = new SqlParameter("@TagID", TagID);
				paramList[2] = new SqlParameter("@ItemID", ItemID);
				paramList[3] = new SqlParameter("@Description ", Added_Description);
				paramList[4] = new SqlParameter("@new_TagID", -1) {Direction = ParameterDirection.InputOutput};

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Add_Description_Tag", paramList);

				// Return TRUE
				int returnValue = Convert.ToInt32(paramList[4].Value);
				return returnValue;

			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Add_Description_Tag", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Add_Description_Tag", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Add_Description_Tag", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return -1;
			}
		}

		/// <summary> Delete's a user's descriptive tage </summary>
		/// <param name="TagID"> Primary key for the entered the descriptive tag </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successul, otherwise FALSE </returns>
		/// <remarks> This calls the 'mySobek_Delete_Description_Tag' stored procedure</remarks> 
		public static bool Delete_Description_Tag(int TagID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Delete_Description_Tag", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@TagID", TagID);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Delete_Description_Tag", paramList);

				// Return TRUE
				return true;

			}
			catch (Exception ee)
			{
				lastException = ee;
				if ( Tracer != null )
				{
					Tracer.Add_Trace("SobekCM_Database.Delete_Description_Tag", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Description_Tag", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Description_Tag", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> List all descriptive tags added by a single user </summary>
		/// <param name="UserID"> Primary key for the user that entered the descriptive tags (or -1 to get ALL tags)</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataTable with all of the user's descriptive tags </returns>
		/// <remarks> This calls the 'mySobek_View_All_User_Tags' stored procedure</remarks> 
		public static DataTable View_Tags_By_User(int UserID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.View_Tags_By_User", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@UserID", UserID);

				DataSet resultSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "mySobek_View_All_User_Tags", paramList);

				return resultSet.Tables[0];
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.View_Tags_By_User", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.View_Tags_By_User", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.View_Tags_By_User", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}


		/// <summary> List all descriptive tags added by a single user </summary>
		/// <param name="AggregationCode"> Aggregation code for which to pull all descriptive tags added </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataTable with all of the descriptive tags added to items within the aggregation of interest </returns>
		/// <remarks> This calls the 'SobekCM_Get_Description_Tags_By_Aggregation' stored procedure  </remarks> 
		public static DataTable View_Tags_By_Aggregation( string AggregationCode, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.View_Tags_By_Aggregation", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@aggregationcode", AggregationCode);

				DataSet resultSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_Description_Tags_By_Aggregation", paramList);

				return resultSet.Tables[0];
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.View_Tags_By_Aggregation", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.View_Tags_By_Aggregation", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.View_Tags_By_Aggregation", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
		}


		#endregion

		#region Properties used by the SobekCM Builder (moved from the bib package)

		/// <summary> Gets the script from the database used for collection building </summary>
		/// <remarks> This calls the 'SobekCM_Get_ColBuild_Script' stored procedure </remarks> 
		public static DataTable CollectionBuild_Script
		{
			get
			{
				try
				{
					DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_ColBuild_Script");
					return tempSet.Tables[0];
				}
				catch (Exception ee)
				{
					lastException = ee;
					return null;
				}
			}
		}


		/// <summary> Gets the values from the builder settings table in the database </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Dictionary of all the keys and values in the builder settings table </returns>
		/// <remarks> This calls the 'SobekCM_Get_Settings' stored procedure </remarks> 
		public static Dictionary<string,string> Get_Settings(Custom_Tracer Tracer)
		{
			Dictionary<string, string> returnValue = new Dictionary<string, string>();

			try
			{

				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_Settings");
				if ((tempSet.Tables.Count > 0) && (tempSet.Tables[0].Rows.Count > 0))
				{
					foreach (DataRow thisRow in tempSet.Tables[0].Rows)
					{
						returnValue[thisRow["Setting_Key"].ToString()] = thisRow["Setting_Value"].ToString();
					}
				}
			}
			catch (Exception ee)
			{
				lastException = ee;
			}

			return returnValue;
		}

		/// <summary> Sets a value for an individual user's setting </summary>
		/// <param name="UserID"> Primary key for this user in the database </param>
		/// <param name="Setting_Key"> Key for the setting to update or insert </param>
		/// <param name="Setting_Value"> Value for the setting to update or insert </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Set_User_Setting_Value' stored procedure </remarks> 
		public static bool Set_User_Setting(int UserID, string Setting_Key, string Setting_Value)
		{
			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[3];
				paramList[0] = new SqlParameter("@UserID", UserID);
				paramList[1] = new SqlParameter("@Setting_Key", Setting_Key);
				paramList[2] = new SqlParameter("@Setting_Value", Setting_Value);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Set_User_Setting_Value", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		/// <summary> Sets a value in the settings table </summary>
		/// <param name="Setting_Key"> Key for the setting to update or insert </param>
		/// <param name="Setting_Value"> Value for the setting to update or insert </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Set_Setting_Value' stored procedure </remarks> 
		public static bool Set_Setting(string Setting_Key, string Setting_Value)
		{
			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[2];
				paramList[0] = new SqlParameter("@Setting_Key", Setting_Key);
				paramList[1] = new SqlParameter("@Setting_Value", Setting_Value);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Set_Setting_Value", paramList);
				return true;
			}
			catch ( Exception ee )
			{
				lastException = ee;
				return false;
			}
		}


		/// <summary> Delete a value from the settings table </summary>
		/// <param name="Setting_Key"> Key for the setting to update or insert </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Delete_Settinge' stored procedure </remarks> 
		public static bool Delete_Setting(string Setting_Key)
		{
			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@Setting_Key", Setting_Key);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Delete_Setting", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		#endregion

		#region Methods used by the SobekCM Builder  (moved from bib package)

		/// <summary> Gets list of item groups (BibID's) for inclusion in the production MarcXML load </summary>
		/// <value> Datatable with the list of records </value>
		/// <remarks> This calls the 'SobekCM_MarcXML_Production_Feed' stored procedure </remarks>
		public static DataTable MarcXML_Production_Feed_Records
		{
			get
			{
				lastException = null;
				try
				{
					// Define a temporary dataset
					DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_MarcXML_Production_Feed");

					// Return the first table from the returned dataset
					return tempSet.Tables[0];
				}
				catch (Exception ee)
				{
					lastException = ee;
					return null;
				}
			}
		}

		/// <summary> Gets list of item groups (BibID's) for inclusion in the test MarcXML load </summary>
		/// <value> Datatable with the list of records </value>
		/// <remarks> This calls the 'SobekCM_MarcXML_Test_Feed' stored procedure </remarks>
		public static DataTable MarcXML_Test_Feed_Records
		{
			get
			{
				lastException = null;
				try
				{
					// Define a temporary dataset
					DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_MarcXML_Test_Feed");

					// Return the first table from the returned dataset
					return tempSet.Tables[0];
				}
				catch (Exception ee)
				{
					lastException = ee;
					return null;
				}
			}
		}

		/// <summary>method used to set the new items flag of a specified item aggregation</summary>
		/// <param name="AggregationCode">Code for this SobekCM item aggregation</param>
		/// <param name="NewItemFlag">Status for the new item flag</param>
		/// <returns>TRUE if successful, otherwise FALSE</returns>
		/// <remarks> This method calls the stored procedure 'SobekCM_Set_Aggregation_NewItem_Flag'. </remarks>
		public static bool Set_Aggregation_NewItem_Flag(string AggregationCode, bool NewItemFlag)
		{
			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[2];
				paramList[0] = new SqlParameter("@code", AggregationCode);
				paramList[1] = new SqlParameter("@newitemflag", NewItemFlag);
				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Set_Aggregation_NewItem_Flag", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}


		/// <summary> Get a list of collections which have new items and are ready to be built</summary>
		/// <returns> DataTable of collections waiting to be built</returns>
		/// <remarks> This method calls the stored procedure 'SobekCM_Get_CollectionList_toBuild'. </remarks>
		public static DataTable Get_CollectionList_ReadyToBuild()
		{
			try
			{
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_CollectionList_toBuild");
				return tempSet.Tables[0];
			}
			catch (Exception ee)
			{
				lastException = ee;
				return null;
			}
		}


		/// <summary> Deletes an item from the SobekCM database</summary>
		/// <param name="BibID"> Bibliographic identifier for the item to delete</param>
		/// <param name="VID"> Volume identifier for the item to delete</param>
		/// <param name="As_Admin"> Indicates this is an admin, who can delete ANY item, not just those without archives attached </param>
		/// <param name="Delete_Message"> Message to include on any archive remnants after an admin delete </param>
		/// <returns> TRUE if successful, otherwise FALSE</returns>
		/// <remarks> This method calls the stored procedure 'SobekCM_Delete_Item'. <br /><br />
		/// This just marks a flag on the item (and item group) as deleted, it does not actually remove the data from the database</remarks>
		public static bool Delete_SobekCM_Item(string BibID, string VID, bool As_Admin, string Delete_Message )
		{
			try
			{
				// build the parameter list
				SqlParameter[] paramList = new SqlParameter[4];
				paramList[0] = new SqlParameter("@bibid", BibID);
				paramList[1] = new SqlParameter("@vid", VID);
				paramList[2] = new SqlParameter("@as_admin", As_Admin);
				paramList[3] = new SqlParameter("@delete_message", Delete_Message);

				//Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Delete_Item", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		#endregion

		#region Methods used to mark an item as needing additional work in the builder and pulling that list for the builder

		/// <summary> Gets the list of all items currently flagged for needing additional work </summary>
		/// <remarks> This calls the 'SobekCM_Get_Items_Needing_Aditional_Work' stored procedure. </remarks>
		public static DataTable Items_Needing_Aditional_Work
		{
			get
			{
				try
				{
					DataSet returnSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_Items_Needing_Aditional_Work");
					return returnSet.Tables[0];
				}
				catch
				{
					return null;
				}
			}
		}

		/// <summary> Update the additional work neeed flag, which flag an item for additional follow up work in the builder </summary>
		/// <param name="ItemID"> Primary key for the item for which to update the additional work needed flag</param>
		/// <param name="New_Flag"> New flag for the additional follow up work </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successul, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Update_Additional_Work_Needed_Flag' stored procedure</remarks> 
		public static bool Update_Additional_Work_Needed_Flag(int ItemID, bool New_Flag, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Update_Additional_Work_Needed_Flag", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[2];
				paramList[0] = new SqlParameter("@itemid", ItemID);
				paramList[1] = new SqlParameter("@newflag", New_Flag);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Update_Additional_Work_Needed_Flag", paramList);

				// Return TRUE
				return true;

			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Update_Additional_Work_Needed_Flag", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_Additional_Work_Needed_Flag", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_Additional_Work_Needed_Flag", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		#endregion

		#region Methods used for SobekCM Administrative Tasks (moved from SobekCM Manager )

		#region Methods related to the Thematic Heading values

		/// <summary> Saves a new thematic heading or updates an existing thematic heading </summary>
		/// <param name="ThematicHeadingID"> Primary key for the existing thematic heading, or -1 for a new heading </param>
		/// <param name="ThemeOrder"> Order of this thematic heading, within the rest of the headings </param>
		/// <param name="ThemeName"> Display name for this thematic heading</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Thematic heading id, or -1 if there was an error </returns>
		/// <remarks> This calls the 'SobekCM_Edit_Thematic_Heading' stored procedure </remarks> 
		public static int Edit_Thematic_Heading( int ThematicHeadingID, int ThemeOrder, string ThemeName, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Edit_Thematic_Heading", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[4];
				paramList[0] = new SqlParameter("@thematicheadingid", ThematicHeadingID);
				paramList[1] = new SqlParameter("@themeorder", ThemeOrder);
				paramList[2] = new SqlParameter("@themename", ThemeName);
				paramList[3] = new SqlParameter("@newid", -1) {Direction = ParameterDirection.Output};

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Edit_Thematic_Heading", paramList);

				return Convert.ToInt32(paramList[3].Value);
			}
			catch (Exception ee)
			{
				lastException = ee;
				return -1;
			}
		}

		/// <summary> Deletes a thematic heading from the database  </summary>
		/// <param name="ThematicHeadingID"> Primary key for the thematic heading to delete </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE</returns>
		/// <remarks> This calls the 'SobekCM_Delete_Thematic_Heading' stored procedure </remarks> 
		public static bool Delete_Thematic_Heading( int ThematicHeadingID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Delete_Thematic_Heading", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@thematicheadingid", ThematicHeadingID);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Delete_Thematic_Heading", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		#endregion


		/// <summary> Saves a item aggregation alias for future use </summary>
		/// <param name="Alias"> Alias string which will forward to a item aggregation </param>
		/// <param name="Aggregation_Code"> Code for the item aggregation to forward to </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE</returns>
		/// <remarks> This calls the 'SobekCM_Save_Item_Aggregation_Alias' stored procedure </remarks> 
		public static bool Save_Aggregation_Alias(string Alias, string Aggregation_Code, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Save_Aggregation_Alias", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[2];
				paramList[0] = new SqlParameter("@alias", Alias);
				paramList[1] = new SqlParameter("@aggregation_code", Aggregation_Code);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Save_Item_Aggregation_Alias", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Save_Aggregation_Alias", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_Aggregation_Alias", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_Aggregation_Alias", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Deletes an item aggregation alias by alias code </summary>
		/// <param name="Alias"> Alias string which forwarded to a item aggregation </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE</returns>
		/// <remarks> This calls the 'SobekCM_Delete_Item_Aggregation_Alias' stored procedure </remarks> 
		public static bool Delete_Aggregation_Alias(string Alias, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Delete_Aggregation_Alias", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@alias", Alias);

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Delete_Item_Aggregation_Alias", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Delete_Aggregation_Alias", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Aggregation_Alias", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Aggregation_Alias", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Saves a HTML skin to the database </summary>
		/// <param name="Skin_Code"> Code for this HTML skin </param>
		/// <param name="Base_Skin_Code"> Base skin code from which this html skin inherits </param>
		/// <param name="OverrideBanner"> Flag indicates this skin overrides the default banner </param>
		/// <param name="OverrideHeaderFooter"> Flag indicates this skin overrides the default header/footer</param>
		/// <param name="Banner_Link"> Link to which the banner sends the user </param>
		/// <param name="Notes"> Notes on this skin ( name, use, etc...) </param>
		/// <param name="Build_On_Launch"> Flag indicates if this skin should be built upon launch ( i.e., is this a heavily used web skin? )</param>
		/// <param name="Suppress_Top_Navigation"> Flag indicates if the top-level aggregation navigation should be suppressed for this web skin ( i.e., is the top-level navigation embedded into the header file already? )</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Add_Web_Skin' stored procedure </remarks> 
		public static bool Save_Web_Skin(string Skin_Code, string Base_Skin_Code, bool OverrideBanner, bool OverrideHeaderFooter, string Banner_Link, string Notes, bool Build_On_Launch, bool Suppress_Top_Navigation, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Save_Skin", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[8];
				paramList[0] = new SqlParameter("@webskincode", Skin_Code);
				paramList[1] = new SqlParameter("@basewebskin", Base_Skin_Code);
				paramList[2] = new SqlParameter("@overridebanner", OverrideBanner);
				paramList[3] = new SqlParameter("@overrideheaderfooter", OverrideHeaderFooter);
				paramList[4] = new SqlParameter("@bannerlink", Banner_Link);
				paramList[5] = new SqlParameter("@notes", Notes);
				paramList[6] = new SqlParameter("@build_on_launch", Build_On_Launch);
				paramList[7] = new SqlParameter("@suppress_top_nav", Suppress_Top_Navigation  );

				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Add_Web_Skin", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Save_Web_Skin", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_Web_Skin", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_Web_Skin", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Deletes a HTML web skin fromo the database </summary>
		/// <param name="Skin_Code"> Code for the  HTML web skin to delete </param>
		/// <param name="Force_Delete"> Flag indicates if this should be deleted, even if things are still attached to this web skin (system admin)</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Delete_Web_Skin' stored procedure </remarks> 
		public static bool Delete_Web_Skin(string Skin_Code, bool Force_Delete, Custom_Tracer Tracer)
		{
			lastException = null;

			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Delete_Web_Skin", String.Empty);
			}

			try
			{
				// Execute this non-query stored procedure
				SqlParameter[] paramList = new SqlParameter[3];
				paramList[0] = new SqlParameter("@webskincode", Skin_Code);
				paramList[1] = new SqlParameter("@force_delete", Force_Delete);
				paramList[2] = new SqlParameter("@links", -1) { Direction = ParameterDirection.Output };

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Delete_Web_Skin", paramList);

				if (Convert.ToInt32(paramList[2].Value) > 0)
				{
					return false;
				}
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Delete_Web_Skin", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Web_Skin", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Web_Skin", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Saves information about a new icon/wordmark or modify an existing one </summary>
		/// <param name="Icon_Name"> Code identifier for this icon/wordmark</param>
		/// <param name="Icon_File"> Filename for this icon/wordmark</param>
		/// <param name="Icon_Link">  Link that clicking on this icon/wordmark will forward the user to</param>
		/// <param name="Icon_Title"> Title for this icon, which appears when you hover over the icon </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Primary key for this new icon (or wordmark), or -1 if this action failed</returns>
		/// <remarks> This calls the 'SobekCM_Save_Icon' stored procedure </remarks> 
		public static int Save_Icon(string Icon_Name, string Icon_File, string Icon_Link, string Icon_Title, Custom_Tracer Tracer)
		{
			return Save_Icon( Icon_Name, Icon_File, Icon_Link, 80, Icon_Title, Tracer);
		}

		/// <summary> Saves information about a new icon/wordmark or modify an existing one </summary>
		/// <param name="Icon_Name"> Code identifier for this icon/wordmark</param>
		/// <param name="Icon_File"> Filename for this icon/wordmark</param>
		/// <param name="Icon_Link">  Link that clicking on this icon/wordmark will forward the user to</param>
		/// <param name="Height"> Height for this icon/wordmark </param>
		/// <param name="Icon_Title"> Title for this icon, which appears when you hover over the icon </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Primary key for this new icon (or wordmark), or -1 if this action failed</returns>
		/// <remarks> This calls the 'SobekCM_Save_Icon' stored procedure </remarks> 
		public static int Save_Icon(string Icon_Name, string Icon_File, string Icon_Link, int Height, string Icon_Title, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Save_Icon", String.Empty);
			}

			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[7];
				paramList[0] = new SqlParameter("@iconid", -1 );
				paramList[1] = new SqlParameter("@icon_name", Icon_Name);
				paramList[2] = new SqlParameter("@icon_url", Icon_File);
				paramList[3] = new SqlParameter("@link", Icon_Link);
				paramList[4] = new SqlParameter("@height", Height);
				paramList[5] = new SqlParameter("@title", Icon_Title);
				paramList[6] = new SqlParameter("@new_iconid", -1) {Direction = ParameterDirection.Output};

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Save_Icon", paramList);

				// Return the new icon id
				return Convert.ToInt32(paramList[6].Value);
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Save_Icon", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_Icon", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_Icon", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return -1;
			}
		}

		/// <summary> Deletes an existing wordmark/icon if it is not linked to any titles in the database </summary>
		/// <param name="Icon_Code"> Wordmark/icon code for the wordmark to delete </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successfully deleted, otherwise FALSE indicating the icon is linked to some titles and cannot be deleted </returns>
		/// <remarks> This calls the 'SobekCM_Delete_Icon' stored procedure </remarks> 
		public static bool Delete_Icon( string Icon_Code, Custom_Tracer Tracer )
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Delete_Icon", String.Empty);
			}

			try
			{
				// Clear the last exception first
				lastException = null;

				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[2];
				paramList[0] = new SqlParameter("@icon_code", Icon_Code);
				paramList[1] = new SqlParameter("@links", -1) {Direction = ParameterDirection.Output};

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Delete_Icon", paramList);

				if (Convert.ToInt32(paramList[1].Value) > 0)
				{
					return false;
				}
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Delete_Icon", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Icon", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Icon", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Gets the datatable of all item aggregation codes </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataTable with list of all item aggregationPermissions' code, type, name, and mapping to Greenstone </returns>
		/// <remarks> This calls the 'SobekCM_Get_Codes' stored procedure </remarks> 
		public static DataTable Get_Codes_Item_Aggregations(Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Codes_Item_Aggregations", String.Empty);
			}

			// Define a temporary dataset
			DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_Codes");
			return tempSet.Tables[0];
		}
			   
		/// <summary> Gets the datatable of all users from the mySobek / personalization database </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> DataTable with list of all users' id, full name, and username </returns>
		/// <remarks> This calls the 'mySobek_Get_All_Users' stored procedure</remarks> 
		public static DataTable Get_All_Users(Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_All_Users", String.Empty);
			}

			// Define a temporary dataset
			DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "mySobek_Get_All_Users");
			return tempSet.Tables[0];
		}


		/// <summary> Updates an existing item aggregation's data that appears in the basic edit aggregation form </summary>
		/// <param name="Code"> Code for this item aggregation </param>
		/// <param name="Name"> Name for this item aggregation </param>
		/// <param name="ShortName"> Short version of this item aggregation </param>
		/// <param name="IsActive"> Flag indicates if this item aggregation is active</param>
		/// <param name="IsHidden"> Flag indicates if this item is hidden</param>
		/// <param name="External_Link">External link for this item aggregation (usually used for institutional aggregationPermissions)</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Update_Item_Aggregation' stored procedure in the SobekCM database</remarks> 
		public static bool Update_Item_Aggregation(string Code, string Name, string ShortName, bool IsActive, bool IsHidden, string External_Link, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Update_Item_Aggregation", String.Empty);
			}

			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[6];
				paramList[0] = new SqlParameter("@code", Code);
				paramList[1] = new SqlParameter("@name", Name);
				paramList[2] = new SqlParameter("@shortname", ShortName);
				paramList[3] = new SqlParameter("@isActive", IsActive);
				paramList[4] = new SqlParameter("@hidden", IsHidden);
				paramList[5] = new SqlParameter("@externallink", External_Link);

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Update_Item_Aggregation", paramList);

				// Succesful, so return true
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Update_Item_Aggregation", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_Item_Aggregation", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_Item_Aggregation", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}



		/// <summary> Delete an item aggregation from the database </summary>
		/// <param name="Code"> Aggregation code for the aggregation to delete</param>
		/// <param name="Username"> Name of the user that deleted this aggregation, for the milestones </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <param name="Error_Message"> [OUT] Error message, if there was an error</param>
		/// <param name="Is_SysAdmin"> Flag indicates if this is a system admin, who can delete aggregationPermissions with items </param>
		/// <returns> Error code - 0 if there was no error </returns>
		/// <remarks> This calls the 'SobekCM_Delete_Item_Aggregation' stored procedure</remarks> 
		public static int Delete_Item_Aggregation(string Code, bool Is_SysAdmin, string Username, Custom_Tracer Tracer, out string Error_Message )
		{
			Error_Message = String.Empty;

			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Delete_Item_Aggregation", String.Empty);
			}

			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[5];
				paramList[0] = new SqlParameter("@aggrcode", Code);
				paramList[1] = new SqlParameter("@isadmin", Is_SysAdmin);
				paramList[2] = new SqlParameter("@username", Username);
				paramList[3] = new SqlParameter("@message", "                                                                                               ") { Direction = ParameterDirection.InputOutput };
				paramList[4] = new SqlParameter("@errorcode", -1) { Direction = ParameterDirection.InputOutput };

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Delete_Item_Aggregation", paramList);

				Error_Message = paramList[3].Value.ToString();

				// Save the error message
				// Succesful, so return new id, if there was one
				return Convert.ToInt32(paramList[4].Value);
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Delete_Item_Aggregation", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Item_Aggregation", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Item_Aggregation", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return -1;
			}
		}

		/// <summary> Add a new milestone to an existing item aggregation </summary>
		/// <param name="AggregationCode"> Item aggregation code </param>
		/// <param name="Milestone"> Milestone to add to this item aggregation </param>
		/// <param name="User"> User name which performed this work </param>
		/// <returns> TRUE if successful saving the new milestone </returns>
		/// <remarks> This calls the 'SobekCM_Add_Item_Aggregation_Milestone' stored procedure</remarks> 
		public static bool Save_Item_Aggregation_Milestone(string AggregationCode, string Milestone, string User )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[3];
				paramList[0] = new SqlParameter("@AggregationCode", AggregationCode);
				paramList[1] = new SqlParameter("@Milestone", Milestone);
				paramList[2] = new SqlParameter("@MilestoneUser", User);

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Add_Item_Aggregation_Milestone", paramList);

				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		/// <summary> Gets all the milestones for a single item aggregation  </summary>
		/// <param name="AggregationCode"> Item aggregation code </param>
		/// <returns> Table of latest updates </returns>
		/// <remarks> This calls the 'SobekCM_Add_Item_Aggregation_Milestone' stored procedure</remarks> 
		public static DataTable Get_Item_Aggregation_Milestone(string AggregationCode)
		{
			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@AggregationCode", AggregationCode);

				// Execute this query stored procedure
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Add_Item_Aggregation_Milestone", paramList);

				return tempSet.Tables[0];
			}
			catch (Exception ee)
			{
				lastException = ee;
				return null;
			}
		}

		/// <summary> Sets a user's password to the newly provided one </summary>
		/// <param name="UserID"> Primary key for this user from the database </param>
		/// <param name="New_Password"> New password (unencrypted) to set for this user </param>
		/// <param name="Is_Temporary_Password"> Flag indicates if this is a temporary password that must be reset the first time the user logs on</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwsie FALSE  </returns>
		/// <remarks> This calls the 'mySobek_Reset_User_Password' stored procedure</remarks> 
		public static bool Reset_User_Password(int UserID, string New_Password, bool Is_Temporary_Password, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Reset_User_Password", String.Empty);
			}

			const string SALT = "This is my salt to add to the password";
			string encryptedPassword = SecurityInfo.SHA1_EncryptString(New_Password + SALT);

			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[3];
				paramList[0] = new SqlParameter("@userid", UserID);
				paramList[1] = new SqlParameter("@password", encryptedPassword);
				paramList[2] = new SqlParameter("@is_temporary", Is_Temporary_Password);

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Reset_User_Password", paramList);

				// Succesful, so return true
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Reset_User_Password", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Reset_User_Password", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Reset_User_Password", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Sets some of the permissions values for a single user </summary>
		/// <param name="UserID"> Primary key for this user from the database </param>
		/// <param name="canSubmit"> Flag indicates if this user can submit items </param>
		/// <param name="Is_Internal"> Flag indicates if this user is considered an 'internal user'</param>
		/// <param name="Can_Edit_All"> Flag indicates if this user is authorized to edit all items in the library</param>
		/// <param name="Is_Portal_Admin"> Flag indicates if this user is a portal Administrator </param>
		/// <param name="Can_Delete_All"> Flag indicates if this user can delete anything in the repository </param>
		/// <param name="Is_System_Admin"> Flag indicates if this user is a system Administrator</param>
		/// <param name="Include_Tracking_Standard_Forms"> Flag indicates if this user should have tracking portions appear in their standard forms </param>
		/// <param name="Edit_Template"> CompleteTemplate name for editing non-MARC records </param>
		/// <param name="Edit_Template_MARC"> CompleteTemplate name for editing MARC-derived records </param>
		/// <param name="Clear_Projects_Templates"> Flag indicates whether to clear projects and templates for this user </param>
		/// <param name="Clear_Aggregation_Links"> Flag indicates whether to clear item aggregationPermissions linked to this user</param>
		/// <param name="Clear_User_Groups"> Flag indicates whether to clear user group membership for this user </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'mySobek_Update_User' stored procedure</remarks> 
		public static bool Update_SobekCM_User(int UserID, bool Can_Submit, bool Is_Internal, bool Can_Edit_All, bool Can_Delete_All, bool Is_System_Admin, bool Is_Portal_Admin, bool Include_Tracking_Standard_Forms, string Edit_Template, string Edit_Template_MARC, bool Clear_Projects_Templates, bool Clear_Aggregation_Links, bool Clear_User_Groups, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User", String.Empty);
			}

			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[13];
				paramList[0] = new SqlParameter("@userid", UserID);
				paramList[1] = new SqlParameter("@can_submit", Can_Submit);
				paramList[2] = new SqlParameter("@is_internal", Is_Internal);
				paramList[3] = new SqlParameter("@can_edit_all", Can_Edit_All);
				paramList[4] = new SqlParameter("@can_delete_all", Can_Delete_All);
				paramList[5] = new SqlParameter("@is_portal_admin", Is_Portal_Admin);
				paramList[6] = new SqlParameter("@is_system_admin", Is_System_Admin);
				paramList[7] = new SqlParameter("@include_tracking_standard_forms", Include_Tracking_Standard_Forms);
				paramList[8] = new SqlParameter("@edit_template", Edit_Template);
				paramList[9] = new SqlParameter("@edit_template_marc", Edit_Template_MARC);
				paramList[10] = new SqlParameter("@clear_projects_templates", Clear_Projects_Templates);
				paramList[11] = new SqlParameter("@clear_aggregation_links", Clear_Aggregation_Links);
				paramList[12] = new SqlParameter("@clear_user_groups", Clear_User_Groups);

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Update_User", paramList);

				// Succesful, so return true
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Sets the list of templates possible for a given user </summary>
		/// <param name="UserID"> Primary key for this user from the database </param>
		/// <param name="Templates"> List of templates to link to this user </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'mySobek_Add_User_Templates_Link' stored procedure</remarks> 
		public static bool Update_SobekCM_User_Templates(int UserID, ReadOnlyCollection<string> Templates, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Templates", String.Empty);
			}

			// Call the routine
			try
			{
				// Build the parameter list for the first run
				SqlParameter[] paramList = new SqlParameter[6];
				paramList[0] = new SqlParameter("@userid", UserID);

				if (Templates.Count > 0)
					paramList[1] = new SqlParameter("@template_default", Templates[0]);
				else
					paramList[1] = new SqlParameter("@template_default", String.Empty);

				if (Templates.Count > 1)
					paramList[2] = new SqlParameter("@template2", Templates[1]);
				else
					paramList[2] = new SqlParameter("@template2", String.Empty);

				if (Templates.Count > 2)
					paramList[3] = new SqlParameter("@template3", Templates[2]);
				else
					paramList[3] = new SqlParameter("@template3", String.Empty);

				if (Templates.Count > 3)
					paramList[4] = new SqlParameter("@template4", Templates[3]);
				else
					paramList[4] = new SqlParameter("@template4", String.Empty);

				if (Templates.Count > 4)
					paramList[5] = new SqlParameter("@template5", Templates[4]);
				else
					paramList[5] = new SqlParameter("@template5", String.Empty);

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Add_User_Templates_Link", paramList);

				int currentIndex = 5;
				while (Templates.Count > currentIndex)
				{
					paramList[0] = new SqlParameter("@userid", UserID);
					paramList[1] = new SqlParameter("@template_default", String.Empty);

					if (Templates.Count > currentIndex)
						paramList[2] = new SqlParameter("@template2", Templates[currentIndex]);
					else
						paramList[2] = new SqlParameter("@template2", String.Empty);

					if (Templates.Count > currentIndex + 1 )
						paramList[3] = new SqlParameter("@template3", Templates[currentIndex + 1]);
					else
						paramList[3] = new SqlParameter("@template3", String.Empty);

					if (Templates.Count > currentIndex + 2)
						paramList[4] = new SqlParameter("@template4", Templates[currentIndex + 2]);
					else
						paramList[4] = new SqlParameter("@template4", String.Empty);

					if (Templates.Count > currentIndex + 3)
						paramList[5] = new SqlParameter("@template5", Templates[currentIndex + 3]);
					else
						paramList[5] = new SqlParameter("@template5", String.Empty);

					// Execute this query stored procedure
					SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Add_User_Templates_Link", paramList);

					currentIndex += 4;
				} 

				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Templates", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Templates", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Templates", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Sets the list of default metadata sets possible for a given user </summary>
		/// <param name="UserID"> Primary key for this user from the database </param>
		/// <param name="MetadataSets"> List of default metadata sets to link to this user</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'mySobek_Add_User_DefaultMetadata_Link' stored procedure</remarks> 
		public static bool Update_SobekCM_User_DefaultMetadata(int UserID, ReadOnlyCollection<string> MetadataSets, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_DefaultMetadata", String.Empty);
			}

			// Call the routine
			try
			{
				// Build the parameter list for the first run
				SqlParameter[] paramList = new SqlParameter[6];
				paramList[0] = new SqlParameter("@userid", UserID);
				if (MetadataSets.Count > 0)
					paramList[1] = new SqlParameter("@metadata_default", MetadataSets[0]);
				else
					paramList[1] = new SqlParameter("@metadata_default", String.Empty);

				if (MetadataSets.Count > 1)
					paramList[2] = new SqlParameter("@metadata2", MetadataSets[1]);
				else
					paramList[2] = new SqlParameter("@metadata2", String.Empty);

				if (MetadataSets.Count > 2)
					paramList[3] = new SqlParameter("@metadata3", MetadataSets[2]);
				else
					paramList[3] = new SqlParameter("@metadata3", String.Empty);

				if (MetadataSets.Count > 3)
					paramList[4] = new SqlParameter("@metadata4", MetadataSets[3]);
				else
					paramList[4] = new SqlParameter("@metadata4", String.Empty);

				if (MetadataSets.Count > 4)
					paramList[5] = new SqlParameter("@metadata5", MetadataSets[4]);
				else
					paramList[5] = new SqlParameter("@metadata5", String.Empty);

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Add_User_DefaultMetadata_Link", paramList);

				int currentIndex = 5;
				while (MetadataSets.Count > currentIndex)
				{
					paramList[0] = new SqlParameter("@userid", UserID);
					paramList[1] = new SqlParameter("@metadata_default", String.Empty);

					if (MetadataSets.Count > currentIndex)
						paramList[2] = new SqlParameter("@metadata2", MetadataSets[currentIndex]);
					else
						paramList[2] = new SqlParameter("@metadata2", String.Empty);

					if (MetadataSets.Count > currentIndex + 1)
						paramList[3] = new SqlParameter("@metadata3", MetadataSets[currentIndex + 1]);
					else
						paramList[3] = new SqlParameter("@metadata3", String.Empty);

					if (MetadataSets.Count > currentIndex + 2)
						paramList[4] = new SqlParameter("@metadata4", MetadataSets[currentIndex + 2]);
					else
						paramList[4] = new SqlParameter("@metadata4", String.Empty);

					if (MetadataSets.Count > currentIndex + 3)
						paramList[5] = new SqlParameter("@metadata5", MetadataSets[currentIndex + 3]);
					else
						paramList[5] = new SqlParameter("@metadata5", String.Empty);

					// Execute this query stored procedure
					SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Add_User_DefaultMetadata_Link", paramList);

					currentIndex += 4;
				}

				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_DefaultMetadata", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_DefaultMetadata", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_DefaultMetadata", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Sets the list of aggregationPermissions and permissions tagged to a given user </summary>
		/// <param name="UserID"> Primary key for this user from the database </param>
		/// <param name="Aggregations"> List of aggregationPermissions and permissions to link to this user </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Add_User_Aggregations_Link' stored procedure</remarks> 
		public static bool Update_SobekCM_User_Aggregations(int UserID, List<User_Permissioned_Aggregation> Aggregations, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Aggregations", String.Empty);
			}

			// Call the routine
			try
			{
				// Build the parameter list for the first run
				SqlParameter[] paramList = new SqlParameter[34];
				paramList[0] = new SqlParameter("@UserID", UserID);

				if (( Aggregations != null ) && ( Aggregations.Count > 0))
				{
					paramList[1] = new SqlParameter("@AggregationCode1", Aggregations[0].Code);
					paramList[2] = new SqlParameter("@canSelect1", Aggregations[0].CanSelect);
					paramList[3] = new SqlParameter("@canEditMetadata1", Aggregations[0].CanEditMetadata);
					paramList[4] = new SqlParameter("@canEditBehaviors1", Aggregations[0].CanEditBehaviors);
					paramList[5] = new SqlParameter("@canPerformQc1", Aggregations[0].CanPerformQc);
					paramList[6] = new SqlParameter("@canUploadFiles1", Aggregations[0].CanUploadFiles);
					paramList[7] = new SqlParameter("@canChangeVisibility1", Aggregations[0].CanChangeVisibility);
					paramList[8] = new SqlParameter("@canDelete1", Aggregations[0].CanDelete);
					paramList[9] = new SqlParameter("@isCurator1", Aggregations[0].IsCurator);
					paramList[10] = new SqlParameter("@onHomePage1", Aggregations[0].OnHomePage);
					paramList[11] = new SqlParameter("@isAdmin1", Aggregations[0].IsAdmin);
				}
				else
				{
					paramList[1] = new SqlParameter("@AggregationCode1", String.Empty);
					paramList[2] = new SqlParameter("@canSelect1", false);
					paramList[3] = new SqlParameter("@canEditMetadata1", false);
					paramList[4] = new SqlParameter("@canEditBehaviors1", false);
					paramList[5] = new SqlParameter("@canPerformQc1", false);
					paramList[6] = new SqlParameter("@canUploadFiles1", false);
					paramList[7] = new SqlParameter("@canChangeVisibility1", false);
					paramList[8] = new SqlParameter("@canDelete1", false);
					paramList[9] = new SqlParameter("@isCurator1", false);
					paramList[10] = new SqlParameter("@onHomePage1", false);
					paramList[11] = new SqlParameter("@isAdmin1", false);
				}

				if (( Aggregations != null ) && ( Aggregations.Count > 1))
				{
					paramList[12] = new SqlParameter("@AggregationCode2", Aggregations[1].Code);
					paramList[13] = new SqlParameter("@canSelect2", Aggregations[1].CanSelect);
					paramList[14] = new SqlParameter("@canEditMetadata2", Aggregations[1].CanEditMetadata);
					paramList[15] = new SqlParameter("@canEditBehaviors2", Aggregations[1].CanEditBehaviors);
					paramList[16] = new SqlParameter("@canPerformQc2", Aggregations[1].CanPerformQc);
					paramList[17] = new SqlParameter("@canUploadFiles2", Aggregations[1].CanUploadFiles);
					paramList[18] = new SqlParameter("@canChangeVisibility2", Aggregations[1].CanChangeVisibility);
					paramList[19] = new SqlParameter("@canDelete2", Aggregations[1].CanDelete);
					paramList[20] = new SqlParameter("@isCurator2", Aggregations[1].IsCurator);
					paramList[21] = new SqlParameter("@onHomePage2", Aggregations[1].OnHomePage);
					paramList[22] = new SqlParameter("@isAdmin2", Aggregations[1].IsAdmin);
				}
				else
				{
					paramList[12] = new SqlParameter("@AggregationCode2", String.Empty);
					paramList[13] = new SqlParameter("@canSelect2", false);
					paramList[14] = new SqlParameter("@canEditMetadata2", false);
					paramList[15] = new SqlParameter("@canEditBehaviors2", false);
					paramList[16] = new SqlParameter("@canPerformQc2", false);
					paramList[17] = new SqlParameter("@canUploadFiles2", false);
					paramList[18] = new SqlParameter("@canChangeVisibility2", false);
					paramList[19] = new SqlParameter("@canDelete2", false);
					paramList[20] = new SqlParameter("@isCurator2", false);
					paramList[21] = new SqlParameter("@onHomePage2", false);
					paramList[22] = new SqlParameter("@isAdmin2", false);
				}


				if (( Aggregations != null ) && ( Aggregations.Count > 2))
				{
					paramList[23] = new SqlParameter("@AggregationCode3", Aggregations[2].Code);
					paramList[24] = new SqlParameter("@canSelect3", Aggregations[2].CanSelect);
					paramList[25] = new SqlParameter("@canEditMetadata3", Aggregations[2].CanEditMetadata);
					paramList[26] = new SqlParameter("@canEditBehaviors3", Aggregations[2].CanEditBehaviors);
					paramList[27] = new SqlParameter("@canPerformQc3", Aggregations[2].CanPerformQc);
					paramList[28] = new SqlParameter("@canUploadFiles3", Aggregations[2].CanUploadFiles);
					paramList[29] = new SqlParameter("@canChangeVisibility3", Aggregations[2].CanChangeVisibility);
					paramList[30] = new SqlParameter("@canDelete3", Aggregations[2].CanDelete);
					paramList[31] = new SqlParameter("@isCurator3", Aggregations[2].IsCurator);
					paramList[32] = new SqlParameter("@onHomePage3", Aggregations[2].OnHomePage);
					paramList[33] = new SqlParameter("@isAdmin3", Aggregations[2].IsAdmin);
				}
				else
				{
					paramList[23] = new SqlParameter("@AggregationCode3", String.Empty);
					paramList[24] = new SqlParameter("@canSelect3", false);
					paramList[25] = new SqlParameter("@canEditMetadata3", false);
					paramList[26] = new SqlParameter("@canEditBehaviors3", false);
					paramList[27] = new SqlParameter("@canPerformQc3", false);
					paramList[28] = new SqlParameter("@canUploadFiles3", false);
					paramList[29] = new SqlParameter("@canChangeVisibility3", false);
					paramList[30] = new SqlParameter("@canDelete3", false);
					paramList[31] = new SqlParameter("@isCurator3", false);
					paramList[32] = new SqlParameter("@onHomePage3", false);
					paramList[33] = new SqlParameter("@isAdmin3", false);
				}
				
				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Add_User_Aggregations_Link", paramList);

				int currentIndex = 3;
				while (( Aggregations != null ) && ( Aggregations.Count > currentIndex))
				{
					// Build the parameter list for the first run
					paramList[0] = new SqlParameter("@UserID", UserID);

					if (Aggregations.Count > currentIndex)
					{
						paramList[1] = new SqlParameter("@AggregationCode1", Aggregations[currentIndex].Code);
						paramList[2] = new SqlParameter("@canSelect1", Aggregations[currentIndex].CanSelect);
						paramList[3] = new SqlParameter("@canEditMetadata1", Aggregations[currentIndex].CanEditMetadata);
						paramList[4] = new SqlParameter("@canEditBehaviors1", Aggregations[currentIndex].CanEditBehaviors);
						paramList[5] = new SqlParameter("@canPerformQc1", Aggregations[currentIndex].CanPerformQc);
						paramList[6] = new SqlParameter("@canUploadFiles1", Aggregations[currentIndex].CanUploadFiles);
						paramList[7] = new SqlParameter("@canChangeVisibility1", Aggregations[currentIndex].CanChangeVisibility);
						paramList[8] = new SqlParameter("@canDelete1", Aggregations[currentIndex].CanDelete);
						paramList[9] = new SqlParameter("@isCurator1", Aggregations[currentIndex].IsCurator);
						paramList[10] = new SqlParameter("@onHomePage1", Aggregations[currentIndex].OnHomePage);
						paramList[11] = new SqlParameter("@isAdmin1", Aggregations[currentIndex].IsAdmin);
					}
					else
					{
						paramList[1] = new SqlParameter("@AggregationCode1", String.Empty);
						paramList[2] = new SqlParameter("@canSelect1", false);
						paramList[3] = new SqlParameter("@canEditMetadata1", false);
						paramList[4] = new SqlParameter("@canEditBehaviors1", false);
						paramList[5] = new SqlParameter("@canPerformQc1", false);
						paramList[6] = new SqlParameter("@canUploadFiles1", false);
						paramList[7] = new SqlParameter("@canChangeVisibility1", false);
						paramList[8] = new SqlParameter("@canDelete1", false);
						paramList[9] = new SqlParameter("@isCurator1", false);
						paramList[10] = new SqlParameter("@onHomePage1", false);
						paramList[11] = new SqlParameter("@isAdmin1", false);
					}

					if (Aggregations.Count > currentIndex + 1)
					{
						paramList[12] = new SqlParameter("@AggregationCode2", Aggregations[currentIndex + 1].Code);
						paramList[13] = new SqlParameter("@canSelect2", Aggregations[currentIndex + 1].CanSelect);
						paramList[14] = new SqlParameter("@canEditMetadata2", Aggregations[currentIndex + 1].CanEditMetadata);
						paramList[15] = new SqlParameter("@canEditBehaviors2", Aggregations[currentIndex + 1].CanEditBehaviors);
						paramList[16] = new SqlParameter("@canPerformQc2", Aggregations[currentIndex + 1].CanPerformQc);
						paramList[17] = new SqlParameter("@canUploadFiles2", Aggregations[currentIndex + 1].CanUploadFiles);
						paramList[18] = new SqlParameter("@canChangeVisibility2", Aggregations[currentIndex + 1].CanChangeVisibility);
						paramList[19] = new SqlParameter("@canDelete2", Aggregations[currentIndex + 1].CanDelete);
						paramList[20] = new SqlParameter("@isCurator2", Aggregations[currentIndex + 1].IsCurator);
						paramList[21] = new SqlParameter("@onHomePage2", Aggregations[currentIndex + 1].OnHomePage);
						paramList[22] = new SqlParameter("@isAdmin2", Aggregations[currentIndex + 1].IsAdmin);
					}
					else
					{
						paramList[12] = new SqlParameter("@AggregationCode2", String.Empty);
						paramList[13] = new SqlParameter("@canSelect2", false);
						paramList[14] = new SqlParameter("@canEditMetadata2", false);
						paramList[15] = new SqlParameter("@canEditBehaviors2", false);
						paramList[16] = new SqlParameter("@canPerformQc2", false);
						paramList[17] = new SqlParameter("@canUploadFiles2", false);
						paramList[18] = new SqlParameter("@canChangeVisibility2", false);
						paramList[19] = new SqlParameter("@canDelete2", false);
						paramList[20] = new SqlParameter("@isCurator2", false);
						paramList[21] = new SqlParameter("@onHomePage2", false);
						paramList[22] = new SqlParameter("@isAdmin2", false);
					}


					if (Aggregations.Count > currentIndex + 2)
					{
						paramList[23] = new SqlParameter("@AggregationCode3", Aggregations[currentIndex + 2].Code);
						paramList[24] = new SqlParameter("@canSelect3", Aggregations[currentIndex + 2].CanSelect);
						paramList[25] = new SqlParameter("@canEditMetadata3", Aggregations[currentIndex + 2].CanEditMetadata);
						paramList[26] = new SqlParameter("@canEditBehaviors3", Aggregations[currentIndex + 2].CanEditBehaviors);
						paramList[27] = new SqlParameter("@canPerformQc3", Aggregations[currentIndex + 2].CanPerformQc);
						paramList[28] = new SqlParameter("@canUploadFiles3", Aggregations[currentIndex + 2].CanUploadFiles);
						paramList[29] = new SqlParameter("@canChangeVisibility3", Aggregations[currentIndex + 2].CanChangeVisibility);
						paramList[30] = new SqlParameter("@canDelete3", Aggregations[currentIndex + 2].CanDelete);
						paramList[31] = new SqlParameter("@isCurator3", Aggregations[currentIndex + 2].IsCurator);
						paramList[32] = new SqlParameter("@onHomePage3", Aggregations[currentIndex + 2].OnHomePage);
						paramList[33] = new SqlParameter("@isAdmin3", Aggregations[currentIndex + 2].IsAdmin);
					}
					else
					{
						paramList[23] = new SqlParameter("@AggregationCode3", String.Empty);
						paramList[24] = new SqlParameter("@canSelect3", false);
						paramList[25] = new SqlParameter("@canEditMetadata3", false);
						paramList[26] = new SqlParameter("@canEditBehaviors3", false);
						paramList[27] = new SqlParameter("@canPerformQc3", false);
						paramList[28] = new SqlParameter("@canUploadFiles3", false);
						paramList[29] = new SqlParameter("@canChangeVisibility3", false);
						paramList[30] = new SqlParameter("@canDelete3", false);
						paramList[31] = new SqlParameter("@isCurator3", false);
						paramList[32] = new SqlParameter("@onHomePage3", false);
						paramList[33] = new SqlParameter("@isAdmin3", false);
					}
					 
					// Execute this query stored procedure
					SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Add_User_Aggregations_Link", paramList);

					currentIndex += 3;
				}

				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Aggregations", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Aggregations", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Aggregations", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}


		/// <summary> Sets some of the basic information and global permissions values for a single user group </summary>
		/// <param name="UserGroupID"> Primary key for this user group from the database, or -1 for a new user group </param>
		/// <param name="GroupName"> Name of this user group </param>
		/// <param name="GroupDescription"> Basic description of this user group </param>
		/// <param name="canSubmit"> Flag indicates if this user group can submit items </param>
		/// <param name="Is_Internal"> Flag indicates if this user group is considered an 'internal user'</param>
		/// <param name="Can_Edit_All"> Flag indicates if this user group is authorized to edit all items in the library</param>
		/// <param name="Is_System_Admin"> Flag indicates if this user group is a system Administrator</param>
		/// <param name="Is_Portal_Admin"> Flag indicated if this user group is a portal administrator </param>
		/// <param name="Include_Tracking_Standard_Forms"> Should this user's settings include the tracking form portions? </param>
		/// <param name="Clear_Metadata_Templates"> Flag indicates whether to clear default metadata sets and templates for this user group </param>
		/// <param name="Clear_Aggregation_Links"> Flag indicates whether to clear item aggregationPermissions linked to this user group </param>
		/// <param name="Clear_Editable_Links"> Flag indicates whether to clear the link between this user group and editable regex expressions  </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> UserGroupId for a new user group, if this was to save a new one </returns>
		/// <remarks> This calls the 'mySobek_Save_User_Group' stored procedure</remarks> 
		public static int Save_User_Group(int UserGroupID, string GroupName, string GroupDescription, bool Can_Submit, bool Is_Internal, bool Can_Edit_All, bool Is_System_Admin, bool Is_Portal_Admin, bool Include_Tracking_Standard_Forms, bool Clear_Metadata_Templates, bool Clear_Aggregation_Links, bool Clear_Editable_Links, bool Is_Sobek_Default, bool Is_Shibboleth_Default, bool Is_LDAP_Default, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Save_User_Group", String.Empty);
			}

			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[17];
				paramList[0] = new SqlParameter("@usergroupid", UserGroupID);
				paramList[1] = new SqlParameter("@groupname", GroupName);
				paramList[2] = new SqlParameter("@groupdescription", GroupDescription);
				paramList[3] = new SqlParameter("@can_submit_items", Can_Submit);
				paramList[4] = new SqlParameter("@is_internal", Is_Internal);
				paramList[6] = new SqlParameter("@can_edit_all", Can_Edit_All);
				paramList[7] = new SqlParameter("@is_system_admin", Is_System_Admin);
				paramList[8] = new SqlParameter("@is_portal_admin", Is_Portal_Admin);
				paramList[9] = new SqlParameter("@include_tracking_standard_forms", Include_Tracking_Standard_Forms );
				paramList[10] = new SqlParameter("@clear_metadata_templates", Clear_Metadata_Templates);
				paramList[11] = new SqlParameter("@clear_aggregation_links", Clear_Aggregation_Links);
				paramList[12] = new SqlParameter("@clear_editable_links", Clear_Editable_Links);
				paramList[13] = new SqlParameter("@is_sobek_default", Is_Sobek_Default);
                paramList[14] = new SqlParameter("@is_shibboleth_default", Is_Shibboleth_Default);
                paramList[15] = new SqlParameter("@is_ldap_default", Is_LDAP_Default);
				paramList[16] = new SqlParameter("@new_usergroupid", UserGroupID) {Direction = ParameterDirection.InputOutput};

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Save_User_Group", paramList);

				// Succesful, so return new id, if there was one
				return Convert.ToInt32(paramList[16].Value);
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Save_User_Group", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_User_Group", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_User_Group", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return -1;
			}
		}

		/// <summary> Sets the list of templates possible for a given user group </summary>
		/// <param name="UserGroupID"> Primary key for this user group from the database </param>
		/// <param name="Templates"> List of templates to link to this user group </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'mySobek_Add_User_Group_Templates_Link' stored procedure</remarks> 
		public static bool Update_SobekCM_User_Group_Templates(int UserGroupID, List<string> Templates, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Group_Templates", String.Empty);
			}

			// Ensure five values
			while (Templates.Count < 5)
				Templates.Add(String.Empty);

			// Call the routine
			try
			{
				// Build the parameter list for the first run
				SqlParameter[] paramList = new SqlParameter[6];
				paramList[0] = new SqlParameter("@usergroupid", UserGroupID);
				paramList[1] = new SqlParameter("@template1", Templates[0]);
				paramList[2] = new SqlParameter("@template2", Templates[1]);
				paramList[3] = new SqlParameter("@template3", Templates[2]);
				paramList[4] = new SqlParameter("@template4", Templates[3]);
				paramList[5] = new SqlParameter("@template5", Templates[4]);

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Add_User_Group_Templates_Link", paramList);

				int currentIndex = 5;
				while (Templates.Count > currentIndex)
				{
					while (Templates.Count < currentIndex + 4)
						Templates.Add(String.Empty);

					paramList[0] = new SqlParameter("@usergroupid", UserGroupID);
					paramList[1] = new SqlParameter("@template1", String.Empty);
					paramList[2] = new SqlParameter("@template2", Templates[currentIndex]);
					paramList[3] = new SqlParameter("@template3", Templates[currentIndex + 1]);
					paramList[4] = new SqlParameter("@template4", Templates[currentIndex + 2]);
					paramList[5] = new SqlParameter("@template5", Templates[currentIndex + 3]);

					// Execute this query stored procedure
					SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Add_User_Group_Templates_Link", paramList);

					currentIndex += 4;
				}

				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Group_Templates", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Group_Templates", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Group_Templates", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Sets the list of default metadata sets possible for a given user group </summary>
		/// <param name="UserGroupID"> Primary key for this user group from the database </param>
		/// <param name="MetadataSets"> List of default metadata sets to link to this user group</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'mySobek_Add_User_Group_Metadata_Link' stored procedure</remarks> 
		public static bool Update_SobekCM_User_Group_DefaultMetadata(int UserGroupID, List<string> MetadataSets, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Group_DefaultMetadata", String.Empty);
			}

			// Ensure five values
			while (MetadataSets.Count < 5)
				MetadataSets.Add(String.Empty);

			// Call the routine
			try
			{
				// Build the parameter list for the first run
				SqlParameter[] paramList = new SqlParameter[6];
				paramList[0] = new SqlParameter("@usergroupid", UserGroupID);
				paramList[1] = new SqlParameter("@metadata1", MetadataSets[0]);
				paramList[2] = new SqlParameter("@metadata2", MetadataSets[1]);
				paramList[3] = new SqlParameter("@metadata3", MetadataSets[2]);
				paramList[4] = new SqlParameter("@metadata4", MetadataSets[3]);
				paramList[5] = new SqlParameter("@metadata5", MetadataSets[4]);

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Add_User_Group_Metadata_Link", paramList);

				int currentIndex = 5;
				while (MetadataSets.Count > currentIndex)
				{
					while (MetadataSets.Count < currentIndex + 4)
						MetadataSets.Add(String.Empty);

					paramList[0] = new SqlParameter("@usergroupid", UserGroupID);
					paramList[1] = new SqlParameter("@metadata1", String.Empty);
					paramList[2] = new SqlParameter("@metadata2", MetadataSets[currentIndex]);
					paramList[3] = new SqlParameter("@metadata3", MetadataSets[currentIndex + 1]);
					paramList[4] = new SqlParameter("@metadata4", MetadataSets[currentIndex + 2]);
					paramList[5] = new SqlParameter("@metadata5", MetadataSets[currentIndex + 3]);

					// Execute this query stored procedure
					SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Add_User_Group_Metadata_Link", paramList);

					currentIndex += 4;
				}

				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Group_DefaultMetadata", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Group_DefaultMetadata", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Group_DefaultMetadata", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Sets the list of aggregationPermissions and permissions tagged to a given user group</summary>
		/// <param name="UserGroupID"> Primary key for this user group from the database </param>
		/// <param name="Aggregations"> List of aggregationPermissions and permissions to link to this user group </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Add_User_Group_Aggregations_Link' stored procedure</remarks> 
        public static bool Update_SobekCM_User_Group_Aggregations(int UserGroupID, List<User_Permissioned_Aggregation> Aggregations, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Group_Aggregations", String.Empty);
			}

			// Call the routine
			try
			{
				// Build the parameter list for the first run
				SqlParameter[] paramList = new SqlParameter[34];
				paramList[0] = new SqlParameter("@UserGroupID", UserGroupID);

				if (Aggregations.Count > 0)
				{
					paramList[1] = new SqlParameter("@AggregationCode1", Aggregations[0].Code);
					paramList[2] = new SqlParameter("@canSelect1", Aggregations[0].CanSelect);
					paramList[3] = new SqlParameter("@canEditMetadata1", Aggregations[0].CanEditMetadata);
					paramList[4] = new SqlParameter("@canEditBehaviors1", Aggregations[0].CanEditBehaviors);
					paramList[5] = new SqlParameter("@canPerformQc1", Aggregations[0].CanPerformQc);
					paramList[6] = new SqlParameter("@canUploadFiles1", Aggregations[0].CanUploadFiles);
					paramList[7] = new SqlParameter("@canChangeVisibility1", Aggregations[0].CanChangeVisibility);
					paramList[8] = new SqlParameter("@canDelete1", Aggregations[0].CanDelete);
					paramList[9] = new SqlParameter("@isCurator1", Aggregations[0].IsCurator);
					paramList[10] = new SqlParameter("@onHomePage1", false);
					paramList[11] = new SqlParameter("@isAdmin1", Aggregations[0].IsAdmin);
				}
				else
				{
					paramList[1] = new SqlParameter("@AggregationCode1", String.Empty);
					paramList[2] = new SqlParameter("@canSelect1", false);
					paramList[3] = new SqlParameter("@canEditMetadata1", false);
					paramList[4] = new SqlParameter("@canEditBehaviors1", false);
					paramList[5] = new SqlParameter("@canPerformQc1", false);
					paramList[6] = new SqlParameter("@canUploadFiles1", false);
					paramList[7] = new SqlParameter("@canChangeVisibility1", false);
					paramList[8] = new SqlParameter("@canDelete1", false);
					paramList[9] = new SqlParameter("@isCurator1", false);
					paramList[10] = new SqlParameter("@onHomePage1", false);
					paramList[11] = new SqlParameter("@isAdmin1", false);
				}

				if (Aggregations.Count > 1)
				{
					paramList[12] = new SqlParameter("@AggregationCode2", Aggregations[1].Code);
					paramList[13] = new SqlParameter("@canSelect2", Aggregations[1].CanSelect);
					paramList[14] = new SqlParameter("@canEditMetadata2", Aggregations[1].CanEditMetadata);
					paramList[15] = new SqlParameter("@canEditBehaviors2", Aggregations[1].CanEditBehaviors);
					paramList[16] = new SqlParameter("@canPerformQc2", Aggregations[1].CanPerformQc);
					paramList[17] = new SqlParameter("@canUploadFiles2", Aggregations[1].CanUploadFiles);
					paramList[18] = new SqlParameter("@canChangeVisibility2", Aggregations[1].CanChangeVisibility);
					paramList[19] = new SqlParameter("@canDelete2", Aggregations[1].CanDelete);
					paramList[20] = new SqlParameter("@isCurator2", Aggregations[1].IsCurator);
					paramList[21] = new SqlParameter("@onHomePage2", false);
					paramList[22] = new SqlParameter("@isAdmin2", Aggregations[1].IsAdmin);
				}
				else
				{
					paramList[12] = new SqlParameter("@AggregationCode2", String.Empty);
					paramList[13] = new SqlParameter("@canSelect2", false);
					paramList[14] = new SqlParameter("@canEditMetadata2", false);
					paramList[15] = new SqlParameter("@canEditBehaviors2", false);
					paramList[16] = new SqlParameter("@canPerformQc2", false);
					paramList[17] = new SqlParameter("@canUploadFiles2", false);
					paramList[18] = new SqlParameter("@canChangeVisibility2", false);
					paramList[19] = new SqlParameter("@canDelete2", false);
					paramList[20] = new SqlParameter("@isCurator2", false);
					paramList[21] = new SqlParameter("@onHomePage2", false);
					paramList[22] = new SqlParameter("@isAdmin2", false);
				}


				if (Aggregations.Count > 2)
				{
					paramList[23] = new SqlParameter("@AggregationCode3", Aggregations[2].Code);
					paramList[24] = new SqlParameter("@canSelect3", Aggregations[2].CanSelect);
					paramList[25] = new SqlParameter("@canEditMetadata3", Aggregations[2].CanEditMetadata);
					paramList[26] = new SqlParameter("@canEditBehaviors3", Aggregations[2].CanEditBehaviors);
					paramList[27] = new SqlParameter("@canPerformQc3", Aggregations[2].CanPerformQc);
					paramList[28] = new SqlParameter("@canUploadFiles3", Aggregations[2].CanUploadFiles);
					paramList[29] = new SqlParameter("@canChangeVisibility3", Aggregations[2].CanChangeVisibility);
					paramList[30] = new SqlParameter("@canDelete3", Aggregations[2].CanDelete);
					paramList[31] = new SqlParameter("@isCurator3", Aggregations[2].IsCurator);
					paramList[32] = new SqlParameter("@onHomePage3", false);
					paramList[33] = new SqlParameter("@isAdmin3", Aggregations[2].IsAdmin);
				}
				else
				{
					paramList[23] = new SqlParameter("@AggregationCode3", String.Empty);
					paramList[24] = new SqlParameter("@canSelect3", false);
					paramList[25] = new SqlParameter("@canEditMetadata3", false);
					paramList[26] = new SqlParameter("@canEditBehaviors3", false);
					paramList[27] = new SqlParameter("@canPerformQc3", false);
					paramList[28] = new SqlParameter("@canUploadFiles3", false);
					paramList[29] = new SqlParameter("@canChangeVisibility3", false);
					paramList[30] = new SqlParameter("@canDelete3", false);
					paramList[31] = new SqlParameter("@isCurator3", false);
					paramList[32] = new SqlParameter("@onHomePage3", false);
					paramList[33] = new SqlParameter("@isAdmin3", false);
				}

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Add_User_Group_Aggregations_Link", paramList);

				int currentIndex = 3;
				while (Aggregations.Count > currentIndex)
				{
					// Build the parameter list for the first run
					paramList[0] = new SqlParameter("@UserGroupID", UserGroupID);

					if (Aggregations.Count > currentIndex)
					{
						paramList[1] = new SqlParameter("@AggregationCode1", Aggregations[currentIndex].Code);
						paramList[2] = new SqlParameter("@canSelect1", Aggregations[currentIndex].CanSelect);
						paramList[3] = new SqlParameter("@canEditMetadata1", Aggregations[currentIndex].CanEditMetadata);
						paramList[4] = new SqlParameter("@canEditBehaviors1", Aggregations[currentIndex].CanEditBehaviors);
						paramList[5] = new SqlParameter("@canPerformQc1", Aggregations[currentIndex].CanPerformQc);
						paramList[6] = new SqlParameter("@canUploadFiles1", Aggregations[currentIndex].CanUploadFiles);
						paramList[7] = new SqlParameter("@canChangeVisibility1", Aggregations[currentIndex].CanChangeVisibility);
						paramList[8] = new SqlParameter("@canDelete1", Aggregations[currentIndex].CanDelete);
						paramList[9] = new SqlParameter("@isCurator1", Aggregations[currentIndex].IsCurator);
						paramList[10] = new SqlParameter("@onHomePage1", false);
						paramList[11] = new SqlParameter("@isAdmin1", Aggregations[currentIndex].IsAdmin);
					}
					else
					{
						paramList[1] = new SqlParameter("@AggregationCode1", String.Empty);
						paramList[2] = new SqlParameter("@canSelect1", false);
						paramList[3] = new SqlParameter("@canEditMetadata1", false);
						paramList[4] = new SqlParameter("@canEditBehaviors1", false);
						paramList[5] = new SqlParameter("@canPerformQc1", false);
						paramList[6] = new SqlParameter("@canUploadFiles1", false);
						paramList[7] = new SqlParameter("@canChangeVisibility1", false);
						paramList[8] = new SqlParameter("@canDelete1", false);
						paramList[9] = new SqlParameter("@isCurator1", false);
						paramList[10] = new SqlParameter("@onHomePage1", false);
						paramList[11] = new SqlParameter("@isAdmin1", false);
					}

					if (Aggregations.Count > currentIndex + 1)
					{
						paramList[12] = new SqlParameter("@AggregationCode2", Aggregations[currentIndex + 1].Code);
						paramList[13] = new SqlParameter("@canSelect2", Aggregations[currentIndex + 1].CanSelect);
						paramList[14] = new SqlParameter("@canEditMetadata2", Aggregations[currentIndex + 1].CanEditMetadata);
						paramList[15] = new SqlParameter("@canEditBehaviors2", Aggregations[currentIndex + 1].CanEditBehaviors);
						paramList[16] = new SqlParameter("@canPerformQc2", Aggregations[currentIndex + 1].CanPerformQc);
						paramList[17] = new SqlParameter("@canUploadFiles2", Aggregations[currentIndex + 1].CanUploadFiles);
						paramList[18] = new SqlParameter("@canChangeVisibility2", Aggregations[currentIndex + 1].CanChangeVisibility);
						paramList[19] = new SqlParameter("@canDelete2", Aggregations[currentIndex + 1].CanDelete);
						paramList[20] = new SqlParameter("@isCurator2", Aggregations[currentIndex + 1].IsCurator);
						paramList[21] = new SqlParameter("@onHomePage2", false);
						paramList[22] = new SqlParameter("@isAdmin2", Aggregations[currentIndex + 1].IsAdmin);
					}
					else
					{
						paramList[12] = new SqlParameter("@AggregationCode2", String.Empty);
						paramList[13] = new SqlParameter("@canSelect2", false);
						paramList[14] = new SqlParameter("@canEditMetadata2", false);
						paramList[15] = new SqlParameter("@canEditBehaviors2", false);
						paramList[16] = new SqlParameter("@canPerformQc2", false);
						paramList[17] = new SqlParameter("@canUploadFiles2", false);
						paramList[18] = new SqlParameter("@canChangeVisibility2", false);
						paramList[19] = new SqlParameter("@canDelete2", false);
						paramList[20] = new SqlParameter("@isCurator2", false);
						paramList[21] = new SqlParameter("@onHomePage2", false);
						paramList[22] = new SqlParameter("@isAdmin2", false);
					}


					if (Aggregations.Count > currentIndex + 2)
					{
						paramList[23] = new SqlParameter("@AggregationCode3", Aggregations[currentIndex + 2].Code);
						paramList[24] = new SqlParameter("@canSelect3", Aggregations[currentIndex + 2].CanSelect);
						paramList[25] = new SqlParameter("@canEditMetadata3", Aggregations[currentIndex + 2].CanEditMetadata);
						paramList[26] = new SqlParameter("@canEditBehaviors3", Aggregations[currentIndex + 2].CanEditBehaviors);
						paramList[27] = new SqlParameter("@canPerformQc3", Aggregations[currentIndex + 2].CanPerformQc);
						paramList[28] = new SqlParameter("@canUploadFiles3", Aggregations[currentIndex + 2].CanUploadFiles);
						paramList[29] = new SqlParameter("@canChangeVisibility3", Aggregations[currentIndex + 2].CanChangeVisibility);
						paramList[30] = new SqlParameter("@canDelete3", Aggregations[currentIndex + 2].CanDelete);
						paramList[31] = new SqlParameter("@isCurator3", Aggregations[currentIndex + 2].IsCurator);
						paramList[32] = new SqlParameter("@onHomePage3", false);
						paramList[33] = new SqlParameter("@isAdmin3", Aggregations[currentIndex + 2].IsAdmin);
					}
					else
					{
						paramList[23] = new SqlParameter("@AggregationCode3", String.Empty);
						paramList[24] = new SqlParameter("@canSelect3", false);
						paramList[25] = new SqlParameter("@canEditMetadata3", false);
						paramList[26] = new SqlParameter("@canEditBehaviors3", false);
						paramList[27] = new SqlParameter("@canPerformQc3", false);
						paramList[28] = new SqlParameter("@canUploadFiles3", false);
						paramList[29] = new SqlParameter("@canChangeVisibility3", false);
						paramList[30] = new SqlParameter("@canDelete3", false);
						paramList[31] = new SqlParameter("@isCurator3", false);
						paramList[32] = new SqlParameter("@onHomePage3", false);
						paramList[33] = new SqlParameter("@isAdmin3", false);
					}

					// Execute this query stored procedure
					SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Add_User_Group_Aggregations_Link", paramList);

					currentIndex += 3;
				}


				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Group_Aggregations", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Group_Aggregations", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Update_SobekCM_User_Group_Aggregations", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

        /// <summary> Deletes a user group, if there are no users attached and if it is not a special group </summary>
        /// <param name="UserGroupID"> Primary key for this user group from the database</param>
        /// <returns> Message value ( -1=users attached, -2=special group, -3=exception, 1 = success) </returns>
        /// <remarks> This calls the 'mySobek_Delete_User_Group' stored procedure</remarks> 
        public static int Delete_User_Group(int UserGroupID, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Delete_User_Group", String.Empty);
            }

            try
            {
                // Build the parameter list
                SqlParameter[] paramList = new SqlParameter[2];
                paramList[0] = new SqlParameter("@usergroupid", UserGroupID);
                paramList[1] = new SqlParameter("@message", 1) { Direction = ParameterDirection.InputOutput };

                // Execute this query stored procedure
                SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Delete_User_Group", paramList);

                // Succesful, so return new id, if there was one
                return Convert.ToInt32(paramList[1].Value);
            }
            catch (Exception ee)
            {
                lastException = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Delete_User_Group", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Delete_User_Group", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Delete_User_Group", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return -3;
            }
        }

	    /// <summary> Saves a new default metadata set, or edits an existing default metadata name </summary>
	    /// <param name="Code"> Code for the new default metadata set, or set to edit </param>
	    /// <param name="Name"> Name for this default metadata set </param>
	    /// <param name="Description"> Full description for this default metadata set </param>
	    /// <param name="UserID"> UserID, if this is not a global default metadata set </param>
	    /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
	    /// <returns> TRUE if successful, otherwise FALSE </returns>
	    /// <remarks> This calls the 'mySobek_Save_DefaultMetadata' stored procedure</remarks> 
	    public static bool Save_Default_Metadata(string Code, string Name, string Description, int UserID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Save_Default_Metadata", String.Empty);
			}

			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[4];
				paramList[0] = new SqlParameter("@metadata_code", Code);
                paramList[1] = new SqlParameter("@metadata_name", Name);
                paramList[2] = new SqlParameter("@description", Description);
                paramList[3] = new SqlParameter("@userid", UserID);

                if (UserID <= 0)
                    paramList[3].Value = DBNull.Value;

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Save_DefaultMetadata", paramList);

				// Succesful, so return true
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Save_Default_Metadata", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_Default_Metadata", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_Default_Metadata", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Deletes an existing default metadata </summary>
        /// <param name="Code"> Code for the default metadata to delete </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'mySobek_Delete_Project' stored procedure</remarks> 
		public static bool Delete_Default_Metadata(string Code, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Delete_Project", String.Empty);
			}

			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@MetadataCode", Code);

				// Execute this query stored procedure
                SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Delete_DefaultMetadata", paramList);

				// Succesful, so return true
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Delete_Project", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Project", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Delete_Project", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}

		/// <summary> Saves a new template, or edits an existing template name </summary>
		/// <param name="Code"> Code for the new template, or template to edit </param>
		/// <param name="Name"> Name for this template </param>
		/// <param name="Description"> Complete description of this template </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'mySobek_Save_Template' stored procedure</remarks> 
		public static bool Save_Template(string Code, string Name, string Description, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Save_Template", String.Empty);
			}

			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[3];
				paramList[0] = new SqlParameter("@project_code", Code);
				paramList[1] = new SqlParameter("@project_name", Name);
                paramList[2] = new SqlParameter("@description", Name);

				// Execute this query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "mySobek_Save_Template", paramList);

				// Succesful, so return true
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
					Tracer.Add_Trace("SobekCM_Database.Save_Template", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_Template", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("SobekCM_Database.Save_Template", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return false;
			}
		}


		/// <summary> Gets the build log for a particular aggregation </summary>
		/// <param name="AggregationID"> Primary key for this aggregation in the database </param>
		/// <returns> Aggregation build log table </returns>
		/// <remarks> This calls the 'SobekCM_Build_Log_Get' stored procedure </remarks> 
		public static DataTable Get_Aggregation_Build_Log(int AggregationID)
		{

			try
			{
				// build the parameter list
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@aggregationid", AggregationID);

				// Get the table
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Build_Log_Get", paramList);

				// Return true, since no exception caught
				return tempSet.Tables[0];

			}
			catch (Exception ee)
			{
				lastException = ee;
				return null;
			}
		}

		#endregion

		#region Methods used by the SobekCM Manager Application

		#region Methods to get and edit information about the ITEM GROUP

		/// <summary> Saves the serial hierarchy and link between an item and an item group </summary>
		/// <param name="GroupID"> Group ID this item belongs to </param>
		/// <param name="ItemID"> Primary key for the item itself </param>
		/// <param name="Level1_Text"> Text for the FIRST level of serial hierarchy relating this item to the item group </param>
		/// <param name="Level1_Index"> Sorting index for the FIRST level of serial hierarchy relating this item to the item group </param>
		/// <param name="Level2_Text"> Text for the SECOND level of serial hierarchy relating this item to the item group </param>
		/// <param name="Level2_Index"> Sorting index for the SECOND level of serial hierarchy relating this item to the item group</param>
		/// <param name="Level3_Text"> Text for the THIRD level of serial hierarchy relating this item to the item group </param>
		/// <param name="Level3_Index"> Sorting index for the THIRD level of serial hierarchy relating this item to the item group</param>
		/// <param name="Level4_Text"> Text for the FOURTH level of serial hierarchy relating this item to the item group </param>
		/// <param name="Level4_Index"> Sorting index for the FOURTH level of serial hierarchy relating this item to the item group</param>
		/// <param name="Level5_Text"> Text for the FIFTH level of serial hierarchy relating this item to the item group </param>
		/// <param name="Level5_Index"> Sorting index for the FIFTH level of serial hierarchy relating this item to the item group</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'SobekCM_Save_Serial_Hierarchy' stored procedure </remarks> 
		public static bool Save_Serial_Hierarchy(int GroupID, int ItemID, string Level1_Text, int Level1_Index,
												 string Level2_Text, int Level2_Index, string Level3_Text, int Level3_Index, string Level4_Text, 
												 int Level4_Index, string Level5_Text, int Level5_Index )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[13];
				paramList[0] = new SqlParameter("@GroupID", GroupID);
				paramList[1] = new SqlParameter("@ItemID", ItemID);
				paramList[2] = new SqlParameter("@Level1_Text", Level1_Text);
				paramList[3] = new SqlParameter("@Level1_Index", Level1_Index);
				paramList[4] = new SqlParameter("@Level2_Text", Level2_Text);
				paramList[5] = new SqlParameter("@Level2_Index", Level2_Index);
				paramList[6] = new SqlParameter("@Level3_Text", Level3_Text);
				paramList[7] = new SqlParameter("@Level3_Index", Level3_Index);
				paramList[8] = new SqlParameter("@Level4_Text", Level4_Text);
				paramList[9] = new SqlParameter("@Level4_Index", Level4_Index);
				paramList[10] = new SqlParameter("@Level5_Text", Level5_Text);
				paramList[11] = new SqlParameter("@Level5_Index", Level5_Index);
				paramList[12] = new SqlParameter("@SerialHierarchy", String.Empty);


				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "SobekCM_Save_Serial_Hierarchy", paramList);

				return true;
			}
			catch (Exception ee)
			{
				// Pass this exception onto the method to handle it
				lastException = ee;
				return false;
			}
		}

		#endregion

		/// <summary> Gets the list of items that were recently quality control accepted and still
		/// need to be set to PUBLIC or RESTRICTED online </summary>
		/// <remarks> This calls the 'Tracking_Items_Pending_Online_Complete' stored procedure </remarks> 
		public static DataTable Items_Pending_Online_Complete
		{
			get
			{
				try
				{
					// Create the connection
					SqlConnection connect = new SqlConnection(connectionString);

					// Create the command 
					SqlCommand executeCommand = new SqlCommand("Tracking_Items_Pending_Online_Complete", connect)
													{CommandType = CommandType.StoredProcedure};

					// Create the adapter
					SqlDataAdapter adapter = new SqlDataAdapter(executeCommand);

					// Create the dataset
					DataSet list = new DataSet();

					// Fill the dataset
					adapter.Fill(list);

					// Return the first table
					return list.Tables[0];

				}
				catch (Exception ee)
				{
					lastException = ee;
					return null;
				}
			}
		}

		/// <summary> Gets the report of all newspaper items which do not have serial information </summary>
		/// <remarks> This calls the 'SobekCM_Manager_Newspapers_Without_Serial_Info' stored procedure </remarks> 
		public static DataTable Newspapers_Without_Serial_Info
		{
			get
			{
				try
				{
					// Create the connection
					SqlConnection connect = new SqlConnection(connectionString);

					// Create the command 
					SqlCommand executeCommand = new SqlCommand("SobekCM_Manager_Newspapers_Without_Serial_Info", connect)
													{CommandType = CommandType.StoredProcedure};

					// Create the adapter
					SqlDataAdapter adapter = new SqlDataAdapter(executeCommand);

					// Create the dataset
					DataSet list = new DataSet();

					// Fill the dataset
					adapter.Fill(list);

					// Return the first table
					return list.Tables[0];

				}
				catch (Exception ee)
				{
					lastException = ee;
					return null;
				}
			}
		}




		/// <summary> Returns the primary key for this item group, identified by bibliographic identifier </summary>
		/// <param name="BibID"> Bibliographic identifier to pull the primary key from the database for </param>
		/// <returns> GroupID for this bibliographic identifier, or -1 if missing</returns>
		/// <remarks> This calls the 'SobekCM_Manager_GroupID_From_BibID' stored procedure </remarks> 
		public static int Get_GroupID_From_BibID(string BibID)
		{
			try
			{
				// Clear the last exception in this case
				lastException = null;

				// Create the connection
				SqlConnection connect = new SqlConnection(connectionString);

				// Create the command 
				SqlCommand executeCommand = new SqlCommand("SobekCM_Manager_GroupID_From_BibID", connect)
												{CommandType = CommandType.StoredProcedure};
				executeCommand.Parameters.AddWithValue("@bibid", BibID);

				// Create the adapter
				SqlDataAdapter adapter = new SqlDataAdapter(executeCommand);

				// Create the dataset
				DataSet list = new DataSet();

				// Fill the dataset
				adapter.Fill(list);

				// If there is a match return it
				return (list.Tables[0].Rows.Count > 0) ? Convert.ToInt32(list.Tables[0].Rows[0][0]) : -1;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return -1;
			}
		}

		/// <summary> Gets the size of the online files and the size of the archived files, by aggregation </summary>
		/// <param name="AggregationCode1"> Code for the primary aggregation  </param>
		/// <param name="AggregationCode2"> Code for the secondary aggregation </param>
		/// <param name="Online_Stats_Type"> Flag indicates if online content reporting should be included ( 0=skip, 1=summary, 2=details )</param>
		/// <param name="Archival_Stats_Type"> Flag indicates if locally archived reporting should be included ( 0=skip, 1=summary, 2=details )</param>
		/// <returns> Dataset with two tables, first is the online space, and second is the archived space </returns>
		/// <remarks> If two codes are passed in, then the values returned is the size of all items which exist
		///  in both the provided aggregationPermissions.  Otherwise, it is just the size of all items in the primary 
		///  aggregation. <br /><br /> This calls the 'SobekCM_Online_Archived_Space' stored procedure </remarks> 
		public static DataSet Online_Archived_Space(string AggregationCode1, string AggregationCode2, short Online_Stats_Type, short Archival_Stats_Type)
		{
			try
			{
				// Create the connection
				SqlConnection connect = new SqlConnection(connectionString);

				// Create the command 
				SqlCommand executeCommand = new SqlCommand("SobekCM_Online_Archived_Space", connect)
				{
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = 120
				};
				executeCommand.Parameters.AddWithValue("@code1", AggregationCode1);
				executeCommand.Parameters.AddWithValue("@code2", AggregationCode2);
				executeCommand.Parameters.AddWithValue("@include_online", Online_Stats_Type);
				executeCommand.Parameters.AddWithValue("@include_archive", Archival_Stats_Type);

				// Create the adapter
				SqlDataAdapter adapter = new SqlDataAdapter(executeCommand);

				// Create the dataset
				DataSet list = new DataSet();

				// Fill the dataset
				adapter.Fill(list);

				return list;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return null;
			}
		}

		#endregion

		#region Methods to interact with the TIVOLI archive file log in the database

		/// <summary> Get the list of all archived TIVOLI files by BibID and VID </summary>
		/// <param name="BibID"> Bibliographic identifier </param>
		/// <param name="VID"> Volume identifier </param>
		/// <returns> List of all the files archived for a particular digital resource </returns>
		/// <remarks> This calls the 'Tivoli_Get_File_By_Bib_VID' stored procedure </remarks> 
		public static DataTable Tivoli_Get_Archived_Files(string BibID, string VID)
		{
			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[2];
				paramList[0] = new SqlParameter("@BibID", BibID);
				paramList[1] = new SqlParameter("@VID", VID);

				// Define a temporary dataset
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "Tivoli_Get_File_By_Bib_VID", paramList);
				return ((tempSet == null) || (tempSet.Tables.Count == 0) || (tempSet.Tables[0].Rows.Count == 0)) ? null : tempSet.Tables[0];
			}
			catch 
			{
				// Return null for this case
				return null;

			}
		}

		/// <summary> Add information about a single file to the archived TIVOLI </summary>
		/// <param name="BibID"> Bibliographic identifier </param>
		/// <param name="VID"> Volume identifier </param>
		/// <param name="Folder"> Name of the folder </param>
		/// <param name="FileName"> Name of the archived file </param>
		/// <param name="FileSize"> Size of the archived file </param>
		/// <param name="LastWriteDate"> Last modified write date of the archived file </param>
		/// <param name="ItemID"> Primary key for this item </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'Tivoli_Add_File_Archive_Log' stored procedure </remarks> 
		public static bool Tivoli_Add_File_Archive_Log(string BibID, string VID, string Folder, string FileName, long FileSize, DateTime LastWriteDate, int ItemID )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[7];
				paramList[0] = new SqlParameter("@BibID", BibID);
				paramList[1] = new SqlParameter("@VID", VID);
				paramList[2] = new SqlParameter("@Folder", Folder);
				paramList[3] = new SqlParameter("@FileName", FileName);
				paramList[4] = new SqlParameter("@Size", FileSize);
				paramList[5] = new SqlParameter("@LastWriteDate", LastWriteDate);
				paramList[6] = new SqlParameter("@ItemID", ItemID);

				// Define a temporary dataset
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tivoli_Add_File_Archive_Log", paramList);

				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		/// <summary> Adds a worklog that items were archived (tivoli)'d for a specific item </summary>
		/// <param name="BibID"> Bibliographic identifier </param>
		/// <param name="VID"> Volume identifier </param>
		/// <param name="User"> User linked to this progress ( usually blank since this is performed by the Tivoli Processor ) </param>
		/// <param name="UserNotes"> Notes about this process worklog </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'Tracking_Archive_Complete' stored procedure </remarks> 
		public static bool Tivoli_Archive_Complete(string BibID, string VID, string User, string UserNotes )
		{
			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[4];
				paramList[0] = new SqlParameter("@BibID", BibID);
				paramList[1] = new SqlParameter("@VID", VID);
				paramList[2] = new SqlParameter("@User", User);
				paramList[3] = new SqlParameter("@UserNotes", UserNotes);

				// Define a temporary dataset
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tracking_Archive_Complete", paramList);

				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		/// <summary> Gets the list of outstanding archive (tivoli) file requests </summary>
		/// <returns> Table with all the outstanding archive (tivoli) file requests </returns>
		/// <remarks> This calls the 'Tivoli_Outstanding_File_Requests' stored procedure </remarks> 
		public static DataTable Tivoli_Outstanding_File_Requests()
		{
			try
			{
				// Define a temporary dataset
				DataSet returnSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "Tivoli_Outstanding_File_Requests");
				if (returnSet != null)
					return returnSet.Tables[0];

				return null;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return null;
			}
		}

		/// <summary> Completes a given archive tivoli file request in the database </summary>
		/// <param name="TivoliRequestID">Primary key for the tivolie request which either completed or failed </param>
		/// <param name="Email_Body"> Body of the response email </param>
		/// <param name="Email_Subject">Subject line to use for the response email </param>
		/// <param name="IsFailure"> Flag indicates if this represents a failure to retrieve the material from TIVOLI</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'Tracking_Archive_Complete' stored procedure </remarks> 
		public static bool Tivoli_Complete_File_Request(int TivoliRequestID, string Email_Body, string Email_Subject, bool IsFailure)
		{
			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[4];
				paramList[0] = new SqlParameter("@tivolirequestid", TivoliRequestID);
				paramList[1] = new SqlParameter("@email_body", Email_Body);
				paramList[2] = new SqlParameter("@email_subject", Email_Subject);
				paramList[3] = new SqlParameter("@isFailure", IsFailure);

				// Define a temporary dataset
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tivoli_Complete_File_Request", paramList);

				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		/// <summary> Requests a package or file from the archives/tivoli </summary>
		/// <param name="BibID"> Bibliographic identifier (BibID) for the item to retrieve files for </param>
		/// <param name="VID"> Volume identifier (VID) for the item to retrieve files for </param>
		/// <param name="Files"> Files to retrieve from archives/tivoli </param>
		/// <param name="UserName"> Name of the user requesting the retrieval </param>
		/// <param name="EmailAddress"> Email address for the user requesting the retrieval </param>
		/// <param name="RequestNote"> Any custom request note, to be returned in the email once retrieval is complete </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'Tivoli_Request_File' stored procedure </remarks> 
		public static bool Tivoli_Request_File( string BibID, string VID, string Files, string UserName, string EmailAddress, string RequestNote )
		{
			try
			{
				string folder = BibID + "\\" + VID;
				if (Files.Length == 0)
					Files = "*";

				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[5];
				paramList[0] = new SqlParameter("@folder", folder);
				paramList[1] = new SqlParameter("@filename", Files);
				paramList[2] = new SqlParameter("@username", UserName);
				paramList[3] = new SqlParameter("@emailaddress", EmailAddress);
				paramList[4] = new SqlParameter("@requestnote", RequestNote);

				// Define a temporary dataset
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tivoli_Request_File", paramList);

				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}


		#endregion

		#region Methods to pull lists of items for the SMaRT tracking application

		/// <summary> Gets the collection of all items linked to an item aggregation  </summary>
		/// <param name="AggregationCode"> Code for the item aggregation of interest </param>
		/// <returns> Table with all of the item and item group information </returns>
		/// <remarks> This calls the 'Tracking_Get_Aggregation_Browse' stored procedure.</remarks>
		public static DataSet Tracking_Get_Item_Aggregation_Browse(string AggregationCode )
		{
			// Create the connection
			SqlConnection connect = new SqlConnection(connectionString + "Connection Timeout=45");

			// Create the command 
			SqlCommand executeCommand = new SqlCommand("Tracking_Get_Aggregation_Browse", connect)
											{CommandTimeout = 45, CommandType = CommandType.StoredProcedure};

			executeCommand.Parameters.AddWithValue("@code", AggregationCode);

			// Create the adapter
			SqlDataAdapter adapter = new SqlDataAdapter(executeCommand);

			// Pull the raw data
			DataSet rawData = new DataSet();
			adapter.Fill(rawData);

			// Return the built results
			return rawData;
		}

		/// <summary> Gets the list of all private and dark items linked to an item aggregation  </summary>
		/// <param name="AggregationCode"> Code for the item aggregation of interest </param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="Tracer"> Tracer object keeps track of all executions that take place while meeting a user's request </param>
		/// <returns> Table with all of the item and item group information </returns>
		/// <remarks> This calls the 'Tracking_Get_Aggregation_Privates' stored procedure.</remarks>
		public static Private_Items_List Tracking_Get_Aggregation_Private_Items(string AggregationCode, int ResultsPerPage, int ResultsPage, int Sort, Custom_Tracer Tracer )
		{
			if (Tracer != null)
				Tracer.Add_Trace("SobekCM_Database.Tracking_Get_Aggregation_Private_Items", "Pulling list of private items for this aggregation");

			// Create the connection
			SqlConnection connect = new SqlConnection(connectionString + "Connection Timeout=45");

			// Create the command 
			SqlCommand executeCommand = new SqlCommand("Tracking_Get_Aggregation_Privates", connect)
											{CommandTimeout = 45, CommandType = CommandType.StoredProcedure};

			executeCommand.Parameters.AddWithValue("@code", AggregationCode);
			executeCommand.Parameters.AddWithValue("@pagesize", ResultsPerPage);
			executeCommand.Parameters.AddWithValue("@pagenumber", ResultsPage);
			executeCommand.Parameters.AddWithValue("@sort", Sort);
			executeCommand.Parameters.AddWithValue("@minpagelookahead", 1);
			executeCommand.Parameters.AddWithValue("@maxpagelookahead", 1);
			executeCommand.Parameters.AddWithValue("@lookahead_factor", LOOKAHEAD_FACTOR);

			// Add parameters for total items and total titles
			SqlParameter totalItemsParameter = executeCommand.Parameters.AddWithValue("@total_items", 0);
			totalItemsParameter.Direction = ParameterDirection.InputOutput;

			SqlParameter totalTitlesParameter = executeCommand.Parameters.AddWithValue("@total_titles", 0);
			totalTitlesParameter.Direction = ParameterDirection.InputOutput;

			// Create the data reader
			connect.Open();
			Private_Items_List returnArgs;
			using (SqlDataReader reader = executeCommand.ExecuteReader())
			{

				// Create the return argument object
				returnArgs = new Private_Items_List {TitleResults = DataReader_To_Private_Items_List(reader)};

				// Close the reader
				reader.Close();

				// Store the total items/titles
				returnArgs.TotalItems = Convert.ToInt32(totalItemsParameter.Value);
				returnArgs.TotalTitles = Convert.ToInt32(totalTitlesParameter.Value);
			}
			connect.Close();

			if (Tracer != null)
				Tracer.Add_Trace("SobekCM_Database.Tracking_Get_Aggregation_Private_Items", "Done pulling list of private items");

			return returnArgs;
		}

		private static List<Private_Items_List_Title> DataReader_To_Private_Items_List(SqlDataReader Reader)
		{
			// Create return list
			List<Private_Items_List_Title> returnValue = new List<Private_Items_List_Title>();

			Dictionary<int, int> lookup = new Dictionary<int, int>();

			// Get all the main title values first
			while (Reader.Read())
			{
				// Create new database title object for this
				Private_Items_List_Title result = new Private_Items_List_Title
													  {
														  RowNumber = Reader.GetInt32(0),
														  BibID = Reader.GetString(1),
														  Group_Title = Reader.GetString(2),
														  Type = Reader.GetString(3),
														  LastActivityDate = Reader.GetDateTime(6),
														  LastMilestoneDate = Reader.GetDateTime(7),
														  CompleteItemCount = Reader.GetInt32(8),
														  PrimaryIdentifierType = Reader.GetString(9),
														  PrimaryIdentifier = Reader.GetString(10)
													  };

				returnValue.Add(result);

				lookup.Add(result.RowNumber, returnValue.Count - 1);
			}

			// Move to the item table
			Reader.NextResult();

			// If there were no titles, then there are no results
			if (returnValue.Count == 0)
				return returnValue;


			// Step through all the item rows, build the item, and add to the title 
			Private_Items_List_Title titleResult = returnValue[0];
			int lastRownumber = titleResult.RowNumber;
			while (Reader.Read())
			{
				// Ensure this is the right title for this item 
				int thisRownumber = Reader.GetInt32(0);
				if (thisRownumber != lastRownumber)
				{
					titleResult = returnValue[lookup[thisRownumber]];
					lastRownumber = thisRownumber;
				}

				// Create new database item object for this
				Private_Items_List_Item result = new Private_Items_List_Item
													 {
														 VID = Reader.GetString(1),
														 Title = Reader.GetString(2),
														 LocallyArchived = Reader.GetBoolean(5),
														 RemotelyArchived = Reader.GetBoolean(6),
														 AggregationCodes = Reader.GetString(7),
														 LastActivityDate = Reader.GetDateTime(8),
														 LastActivityType = Reader.GetString(9),
														 LastMilestone = Reader.GetInt32(10),
														 LastMilestoneDate = Reader.GetDateTime(11)
													 };

                // Pull the values that are nullable
			    string comments = Reader.GetString(3);
			    string pubdate = Reader.GetString(4);
			    
			    string creator = Reader.GetString(19);

                // Assign the values if there are values
			    if (comments.Length > 0) result.Internal_Comments = comments;
                if (pubdate.Length > 0) result.PubDate = pubdate;
                if (creator.Length > 0) result.Creator = creator;

				// Assign the embargo end
			    if (!Reader.IsDBNull(18))
			    {
                    DateTime embargoEnd = Reader.GetDateTime(18);
			        if (embargoEnd.Year < 9999)
			            result.EmbargoDate = embargoEnd;
			    }

			    // Add this to the title object
				titleResult.Add_Item_Result(result);
			}

			return returnValue;
		}




		/// <summary> Perform a metadata search against items in the database </summary>
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
		/// <param name="AggregationCode"> Code for the aggregation of interest ( or empty string to search all aggregationPermissions )</param>
		/// <returns> Table with all of the item and item group information which matches the metadata search </returns>
		/// <remarks> This calls the 'Tracking_Metadata_Search' stored procedure.</remarks>
		public static DataSet Tracking_Metadata_Search(string Term1, int Field1,
													   int Link2, string Term2, int Field2, int Link3, string Term3, int Field3, int Link4, string Term4, int Field4,
													   int Link5, string Term5, int Field5, int Link6, string Term6, int Field6, int Link7, string Term7, int Field7,
													   int Link8, string Term8, int Field8, int Link9, string Term9, int Field9, int Link10, string Term10, int Field10,
													   string AggregationCode )
		{
			// Create the connection
			SqlConnection connect = new SqlConnection(connectionString + "Connection Timeout=45");

			// Create the command 
			SqlCommand executeCommand = new SqlCommand("Tracking_Metadata_Search", connect)
											{CommandTimeout = 45, CommandType = CommandType.StoredProcedure};

			executeCommand.Parameters.AddWithValue("@term1", Term1);
			executeCommand.Parameters.AddWithValue("@field1", Field1);
			executeCommand.Parameters.AddWithValue("@link2", Link2);
			executeCommand.Parameters.AddWithValue("@term2", Term2);
			executeCommand.Parameters.AddWithValue("@field2", Field2);
			executeCommand.Parameters.AddWithValue("@link3", Link3);
			executeCommand.Parameters.AddWithValue("@term3", Term3);
			executeCommand.Parameters.AddWithValue("@field3", Field3);
			executeCommand.Parameters.AddWithValue("@link4", Link4);
			executeCommand.Parameters.AddWithValue("@term4", Term4);
			executeCommand.Parameters.AddWithValue("@field4", Field4);
			executeCommand.Parameters.AddWithValue("@link5", Link5);
			executeCommand.Parameters.AddWithValue("@term5", Term5);
			executeCommand.Parameters.AddWithValue("@field5", Field5);
			executeCommand.Parameters.AddWithValue("@link6", Link6);
			executeCommand.Parameters.AddWithValue("@term6", Term6);
			executeCommand.Parameters.AddWithValue("@field6", Field6);
			executeCommand.Parameters.AddWithValue("@link7", Link7);
			executeCommand.Parameters.AddWithValue("@term7", Term7);
			executeCommand.Parameters.AddWithValue("@field7", Field7);
			executeCommand.Parameters.AddWithValue("@link8", Link8);
			executeCommand.Parameters.AddWithValue("@term8", Term8);
			executeCommand.Parameters.AddWithValue("@field8", Field8);
			executeCommand.Parameters.AddWithValue("@link9", Link9);
			executeCommand.Parameters.AddWithValue("@term9", Term9);
			executeCommand.Parameters.AddWithValue("@field9", Field9);
			executeCommand.Parameters.AddWithValue("@link10", Link10);
			executeCommand.Parameters.AddWithValue("@term10", Term10);
			executeCommand.Parameters.AddWithValue("@field10", Field10);
			if (AggregationCode.ToUpper() == "ALL")
				AggregationCode = String.Empty;
			executeCommand.Parameters.AddWithValue("@aggregationcode", AggregationCode);

			// Create the adapter
			SqlDataAdapter adapter = new SqlDataAdapter(executeCommand);

			// Pull the raw data
			DataSet rawData = new DataSet();
			adapter.Fill(rawData);

			// Return the built results
			return rawData;
		}

		/// <summary> Performs a basic metadata search over the entire citation, given a search condition </summary>
		/// <param name="Search_Condition"> Search condition string to be run against the databasse </param>
		/// <param name="AggregationCode"> Code for the aggregation of interest ( or empty string to search all aggregationPermissions )</param>
		/// <returns> Table with all of the item and item group information which matches the metadata search </returns>
		/// <remarks> This calls the 'Tracking_Metadata_Basic_Search' stored procedure.</remarks>
		public static DataSet Tracking_Metadata_Search(string Search_Condition, string AggregationCode )
		{
			// Create the connection
			SqlConnection connect = new SqlConnection(connectionString + "Connection Timeout=45");

			// Create the command 
			SqlCommand executeCommand = new SqlCommand("Tracking_Metadata_Basic_Search", connect)
											{CommandTimeout = 45, CommandType = CommandType.StoredProcedure};

			executeCommand.Parameters.AddWithValue("@searchcondition", Search_Condition);
			executeCommand.Parameters.AddWithValue("@aggregationcode", AggregationCode);


			// Create the adapter
			SqlDataAdapter adapter = new SqlDataAdapter(executeCommand);

			// Pull the raw data
			DataSet rawData = new DataSet();
			adapter.Fill(rawData);

			// Return the built results
			return rawData;
		}

		/// <summary> Performs a metadata search for a piece of metadata that EXACTLY matches the provided search term </summary>
		/// <param name="Search_Term"> Search condition string to be run against the databasse </param>
		/// <param name="FieldID"> Primary key for the field to search in the database </param>
		/// <param name="AggregationCode"> Code for the aggregation of interest ( or empty string to search all aggregationPermissions )</param>
		/// <returns> Table with all of the item and item group information which matches the metadata search </returns>
		/// <remarks> This calls the 'Tracking_Metadata_Exact_Search' stored procedure.</remarks>
		public static DataSet Tracking_Metadata_Exact_Search(string Search_Term, int FieldID, string AggregationCode )
		{
			// Create the connection
			SqlConnection connect = new SqlConnection(connectionString + "Connection Timeout=45");

			// Create the command 
			SqlCommand executeCommand = new SqlCommand("Tracking_Metadata_Exact_Search", connect)
											{CommandTimeout = 45, CommandType = CommandType.StoredProcedure};

			executeCommand.Parameters.AddWithValue("@term1", Search_Term.Replace("''", "'"));
			executeCommand.Parameters.AddWithValue("@field1", FieldID);
			if (AggregationCode.ToUpper() == "ALL")
				AggregationCode = String.Empty;
			executeCommand.Parameters.AddWithValue("@aggregationcode", AggregationCode);

			// Create the adapter
			SqlDataAdapter adapter = new SqlDataAdapter(executeCommand);

			// Pull the raw data
			DataSet rawData = new DataSet();
			adapter.Fill(rawData);

			// Return the built results
			return rawData;
		}

		/// <summary> Returns the list of all items/titles which match a given OCLC number </summary>
		/// <param name="OCLC_Number"> OCLC number to look for matching items </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Table with all of the item and item group information which matches the OCLC number </returns>
		/// <remarks> This calls the 'Tracking_Items_By_OCLC' stored procedure <br /><br />
		/// This is very similar to the <see cref="SobekCM_Database.Items_By_OCLC_Number" /> method, except it returns more information, since
		/// the tracking application does not have basic information about each item/title in its cache, unlike the
		/// web server application, which does cache this information. </remarks>
		public static DataSet Tracking_Items_By_OCLC_Number(long OCLC_Number, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Items_By_OCLC_Number", "Searching by OCLC in the database");
			}

			// Build the parameter list
			SqlParameter[] paramList = new SqlParameter[1];
			paramList[0] = new SqlParameter("@oclc_number", OCLC_Number);

			// Get the matching set
			DataSet rawData = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "Tracking_Items_By_OCLC", paramList);

			// Return the built results
			return rawData;
		}

		/// <summary> Returns the list of all items/titles which match a given ALEPH number </summary>
		/// <param name="ALEPH_Number"> ALEPH number to look for matching items </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Table with all of the item and item group information which matches the ALEPH number </returns>
		/// <remarks> This calls the 'Tracking_Items_By_ALEPH' stored procedure. <br /><br />
		/// This is very similar to the <see cref="SobekCM_Database.Items_By_ALEPH_Number" /> method, except it returns more information, since
		/// the tracking application does not have basic information about each item/title in its cache, unlike the
		/// web server application, which does cache this information. </remarks>
		public static DataSet Tracking_Items_By_ALEPH_Number(int ALEPH_Number, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Items_By_ALEPH_Number", "Searching by ALEPH in the database");
			}

			// Build the parameter list
			SqlParameter[] paramList = new SqlParameter[1];
			paramList[0] = new SqlParameter("@aleph_number", ALEPH_Number);

			// Get the matching set
			DataSet rawData = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "Tracking_Items_By_ALEPH", paramList);

			// Return the built results
			return rawData;
		}

		/// <summary> Gets the list of all items within this item group, indicated by BibID, including additional information for the SMaRT tracking application </summary>
		/// <param name="BibID"> Bibliographic identifier for the title of interest </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Strongly typed dataset with information about the title (item group), including volumes, icons, and skins</returns>
		/// <remarks> This calls the 'Tracking_Get_Multiple_Volumes' stored procedure <br /><br />
		/// This is very similar to the <see cref="SobekCM_Database.Get_Multiple_Volumes" /> method, except it returns more information, since
		/// the tracking application does not have basic information about each item/title in its cache, unlike the
		/// web server application, which does cache this information. </remarks>
		public static SobekCM_Items_In_Title Tracking_Multiple_Volumes(string BibID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Tracking_Multiple_Volumes", "List of volumes for " + BibID + " pulled from database");
			}

			try
			{
				// Create the connection
				SqlConnection connect = new SqlConnection(connectionString);

				// Create the command 
				SqlCommand executeCommand = new SqlCommand("Tracking_Get_Multiple_Volumes", connect)
												{CommandType = CommandType.StoredProcedure};
				executeCommand.Parameters.AddWithValue("@bibid", BibID);

				// Create the adapter
				SqlDataAdapter adapter = new SqlDataAdapter(executeCommand);

				// Get the datatable
				DataSet valueSet = new DataSet();
				adapter.Fill(valueSet);

				// If there was either no match, or more than one, return null
				if ((valueSet.Tables.Count == 0) || (valueSet.Tables[0] == null) || (valueSet.Tables[0].Rows.Count == 0))
				{
					return null;
				}

				// Create the object
				SobekCM_Items_In_Title returnValue = new SobekCM_Items_In_Title(valueSet.Tables[0]);

				// Return the fully built object
				return returnValue;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return null;
			}
		}

		/// <summary> Gets the high level report of which items exist in which milestone for an aggregation </summary>
		/// <param name="AggregationCode"> Code for the item aggregation of interest </param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Table with the milestone information </returns>
		/// <remarks> This calls the 'Tracking_Item_Milestone_Report' stored procedure.</remarks>
		public static DataTable Tracking_Get_Milestone_Report(string AggregationCode, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Tracking_Get_Milestone_Report", "");
			}

			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@aggregation_code", AggregationCode);

				// Define a temporary dataset
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "Tracking_Item_Milestone_Report", paramList);

				// Return the built argument set
				return tempSet.Tables[0];
			}
			catch
			{
				return null;
			}
		}

		#endregion

		#region Methods pulled over from old Tracking Database

		/// <summary> Gets the history and archive information about a single item from the tracking database</summary>
		/// <param name="ItemID"> Primary key for this item in the database </param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns>Dataset which contains the history and archive information for this item</returns>
		/// <remarks> This calls the 'Tracking_Get_History_Archives' stored procedure. </remarks>
		public static DataSet Tracking_Get_History_Archives(int ItemID, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Tracking_Get_History_Archives", String.Empty);
			}

			try
			{
				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@itemid", ItemID);

				return SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "Tracking_Get_History_Archives", paramList);
			}
			catch (Exception ee)
			{
				lastException = ee;
				return null;
			}
		}

		/// <summary> Gets the list of all items which have been modified in this library from the history/workflow information over the last week </summary>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Dataset which contains all items which have recently been modified in this library from the tracking database's history/workflow information</returns>
		/// <remarks> This calls the 'Tracking_Update_List' stored procedure. </remarks>
		public static DataTable Tracking_Update_List(Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("SobekCM_Database.Tracking_Update_List", String.Empty);

			try
			{
				DateTime sinceDate = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0));

				SqlParameter[] paramList = new SqlParameter[1];
				paramList[0] = new SqlParameter("@sinceDate", sinceDate.ToShortDateString());

				return SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "Tracking_Update_List", paramList).Tables[0];
			}
			catch ( Exception ee )
			{
				lastException = ee;
				return null;
			}
		}



		/// <summary> Marks an item as having been submitted online </summary>
		/// <param name="ItemID"> Primary key for the item having a progress/worklog entry added </param>
		/// <param name="User">User name who submitted this item</param>
		/// <param name="UserNotes">Any user notes about this new item</param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'Tracking_Online_Submit_Complete' stored procedure. </remarks>
		public static bool Tracking_Online_Submit_Complete(int ItemID, string User, string UserNotes)
		{
			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[3];
				paramList[0] = new SqlParameter("@itemid", ItemID);
				paramList[1] = new SqlParameter("@user", User);
				paramList[2] = new SqlParameter("@usernotes", UserNotes);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tracking_Online_Submit_Complete", paramList);

				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		/// <summary> Marks an item as having been loaded as a new item by the bulk loader </summary>
		/// <param name="BibID"> Bibliographic identifier for the item to which to add the new history/worklog </param>
		/// <param name="VID"> Volume identifier for the item to which to add the new history/worklog </param>
		/// <param name="User"> User who performed this work or initiated this work </param>
		/// <param name="UserNotes"> Any notes generated during the work or by the work initiator </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'Tracking_Load_New_Complete' stored procedure. </remarks>
		public static bool Tracking_Load_New_Complete(string BibID, string VID, string User, string UserNotes)
		{
			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[4];
				paramList[0] = new SqlParameter("@bibid", BibID);
				paramList[1] = new SqlParameter("@vid", VID);
				paramList[2] = new SqlParameter("@user", User);
				paramList[3] = new SqlParameter("@usernotes", UserNotes);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tracking_Load_New_Complete", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		/// <summary> Marks an item as having been loaded as a replacement item by the bulk loader </summary>
		/// <param name="BibID"> Bibliographic identifier for the item to which to add the new history/worklog </param>
		/// <param name="VID"> Volume identifier for the item to which to add the new history/worklog </param>
		/// <param name="User"> User who performed this work or initiated this work </param>
		/// <param name="UserNotes"> Any notes generated during the work or by the work initiator </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'Tracking_Load_Replacement_Complete' stored procedure. </remarks>
		public static bool Tracking_Load_Replacement_Complete(string BibID, string VID, string User, string UserNotes)
		{
			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[4];
				paramList[0] = new SqlParameter("@bibid", BibID);
				paramList[1] = new SqlParameter("@vid", VID);
				paramList[2] = new SqlParameter("@user", User);
				paramList[3] = new SqlParameter("@usernotes", UserNotes);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tracking_Load_Replacement_Complete", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		/// <summary> Marks an item as having a metadata update loaded by the bulk loader </summary>
		/// <param name="BibID"> Bibliographic identifier for the item to which to add the new history/worklog </param>
		/// <param name="VID"> Volume identifier for the item to which to add the new history/worklog </param>
		/// <param name="User"> User who performed this work or initiated this work </param>
		/// <param name="UserNotes"> Any notes generated during the work or by the work initiator </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'Tracking_Load_Metadata_Update_Complete' stored procedure. </remarks>
		public static bool Tracking_Load_Metadata_Update_Complete(string BibID, string VID, string User, string UserNotes)
		{
			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[4];
				paramList[0] = new SqlParameter("@bibid", BibID);
				paramList[1] = new SqlParameter("@vid", VID);
				paramList[2] = new SqlParameter("@user", User);
				paramList[3] = new SqlParameter("@usernotes", UserNotes);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tracking_Load_Metadata_Update_Complete", paramList);
				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		/// <summary> Marks an item as been digitally acquired </summary>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'Tracking_Digital_Acquisition_Complete' stored procedure. </remarks>
		public static bool Tracking_Digital_Acquisition_Complete( string BibID, string VID, string User, string Location, DateTime Date ) 
		{
			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[5];
				paramList[0] = new SqlParameter("@bibid", BibID);
				paramList[1] = new SqlParameter("@vid", VID);
				paramList[2] = new SqlParameter("@user", User);
				paramList[3] = new SqlParameter("@storagelocation", Location);
				paramList[4] = new SqlParameter("@date", Date);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tracking_Digital_Acquisition_Complete", paramList);

				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		/// <summary> Marks an item as been image processed </summary>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		/// <remarks> This calls the 'Tracking_Image_Processing_Complete' stored procedure. </remarks>
		public static bool Tracking_Image_Processing_Complete(string BibID, string VID, string User, string Location, DateTime Date)
		{
			try
			{
				// Build the parameter list
				SqlParameter[] paramList = new SqlParameter[5];
				paramList[0] = new SqlParameter("@bibid", BibID);
				paramList[1] = new SqlParameter("@vid", VID);
				paramList[2] = new SqlParameter("@user", User);
				paramList[3] = new SqlParameter("@storagelocation", Location);
				paramList[4] = new SqlParameter("@date", Date);

				// Execute this non-query stored procedure
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "Tracking_Image_Processing_Complete", paramList);

				return true;
			}
			catch (Exception ee)
			{
				lastException = ee;
				return false;
			}
		}

		#endregion

		#region Method to save a FDA report to the database

		/// <summary> Saves all the pertinent information from a received Florida Digital Archive (FDA) ingest report </summary>
		/// <param name="Report"> Object containing all the data from the received FDA report </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		public static bool FDA_Report_Save(FDA_Report_Data Report)
		{
			// Try to get the bibid and vid from the package name
			string bibid = String.Empty;
			string vid = String.Empty;
			if ((Report.Package.Length == 16) && (Report.Package[10] == '_'))
			{
				bibid = Report.Package.Substring(0, 10);
				vid = Report.Package.Substring(11, 5);
			}

			// If the package name was bib id without VID
			if (Report.Package.Length == 10)
			{
				bibid = Report.Package;
			}

			// Save the report information to the database
			int reportid = FDA_Report_Save(Report.Package, Report.IEID, Report.Report_Type_String, Report.Date, Report.Account, Report.Project, Report.Warnings, Report.Message_Note, bibid, vid);

			// If no error, continue
			return reportid > 0;
		}

		/// <summary> Save the information about a FDA report to the database </summary>
		/// <param name="Package">ID of the submission package sent to FDA.  (End user's id)</param>
		/// <param name="IEID">Intellectual Entity ID assigned by FDA</param>
		/// <param name="FdaReportType">Type of FDA report received</param>
		/// <param name="Report_Date">Date FDA was generated</param>
		/// <param name="Account">Account information for the FDA submission package</param>
		/// <param name="Project">Project information for the FDA submission package</param>
		/// <param name="Warnings">Number of warnings in this package</param>
		/// <param name="BibID">Bibliographic Identifier</param>
		/// <param name="VID">Volume Identifier</param>
		/// <param name="Message"> Message included in the FDA report received </param>
		/// <returns>Primary key for the report in the database, or -1 on failure</returns>
		/// <remarks>This calls the FDA_Report_Save stored procedure in the database</remarks>
		public static int FDA_Report_Save(string Package, string IEID, string FdaReportType, DateTime Report_Date, string Account, string Project, int Warnings, string Message, string BibID, string VID)
		{
			// If there is no connection string, return -1
			if (connectionString.Length == 0)
				return -1;

			try
			{
				// Create the SQL Connection
				SqlConnection sqlConn = new SqlConnection(connectionString);

				// Create the SQL Command
				SqlCommand addReport = new SqlCommand("FDA_Report_Save", sqlConn)
										   {CommandType = CommandType.StoredProcedure};

				// Add all of the parameters
				addReport.Parameters.AddWithValue("@Package", Package);
				addReport.Parameters.AddWithValue("@IEID", IEID);
				addReport.Parameters.AddWithValue("@FdaReportType", FdaReportType);
				addReport.Parameters.AddWithValue("@Report_Date", Report_Date);
				addReport.Parameters.AddWithValue("@Account", Account);
				addReport.Parameters.AddWithValue("@Project", Project);
				addReport.Parameters.AddWithValue("@Warnings", Warnings);
				addReport.Parameters.AddWithValue("@Message", Message);
				addReport.Parameters.AddWithValue("@BibID", BibID);
				addReport.Parameters.AddWithValue("@VID", VID);

				// Add a final parameter to receive the primary key back from the database
				addReport.Parameters.AddWithValue("@FdaReportID", -1);
				addReport.Parameters["@FdaReportID"].Direction = ParameterDirection.InputOutput;

				// Open the SQL Connection and execute the stored procedure
				sqlConn.Open();
				addReport.ExecuteNonQuery();
				sqlConn.Close();

				// Get and return the primary key
				return Convert.ToInt32(addReport.Parameters["@FdaReportID"].Value);
			}
			catch
			{
				// In the case of an error, return -1
				return -1;
			}
		}

		#endregion

		#region Methods related to OAI-PMH

		/// <summary> Gets the list of all OAI-enabled item aggregationPermissions </summary>
		/// <returns> DataTable with all the data about the OAI-enabled item aggregationPermissions, including code, name, description, last item added date, and any aggregation-level OAI_Metadata  </returns>
		/// <remarks> This calls the 'SobekCM_Get_OAI_Sets' stored procedure  <br /><br />
		/// This is called by the <see cref="Oai_MainWriter"/> class. </remarks> 
		public static DataTable Get_OAI_Sets()
		{
			// Define a temporary dataset
			DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_OAI_Sets");

			// If there was no data for this collection and entry point, return null (an ERROR occurred)
			if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null))
			{
				return null;
			}

			// Return the first table from the returned dataset
			return tempSet.Tables[0];
		}

		/// <summary> Returns a list of either identifiers or records for either the entire system or a single
		/// set, to be served through the OAI-PMH server  </summary>
		/// <param name="Set_Code"> Code the OAI-PMH set (which is really an aggregation code)</param>
		/// <param name="Data_Code"> Code for the metadata to be served ( usually oai_dc )</param>
		/// <param name="From_Date"> Date from which to pull records which have changed </param>
		/// <param name="Until_Date"> Date to pull up to by last modified date on the records </param>
		/// <param name="Page_Size"> Number of records to include in a single 'page' of OAI-PMH results </param>
		/// <param name="Page_Number"> Page number of the results to return </param>
		/// <param name="Include_Record"> Flag indicates whether the full records should be included, or just the identifier </param>
		/// <returns> DataTable of all the OAI-PMH record information </returns>
		/// <remarks> This calls the 'SobekCM_Get_OAI_Data' stored procedure  <br /><br />
		/// This is called by the <see cref="Oai_MainWriter"/> class. </remarks> 
		public static List<OAI_Record> Get_OAI_Data( string Set_Code, string Data_Code, DateTime From_Date, DateTime Until_Date, int Page_Size, int Page_Number, bool Include_Record )
		{
			// Create the connection
			SqlConnection connect = new SqlConnection(connectionString);

			// Create the command 
			SqlCommand executeCommand = new SqlCommand("SobekCM_Get_OAI_Data", connect) { CommandType = CommandType.StoredProcedure };

			executeCommand.Parameters.AddWithValue("@aggregationcode", Set_Code);
			executeCommand.Parameters.AddWithValue("@data_code", Data_Code);
			executeCommand.Parameters.AddWithValue("@from", From_Date);
			executeCommand.Parameters.AddWithValue("@until", Until_Date);
			executeCommand.Parameters.AddWithValue("@pagesize", Page_Size);
			executeCommand.Parameters.AddWithValue("@pagenumber", Page_Number);
			executeCommand.Parameters.AddWithValue("@include_data", Include_Record);

			// Create the data reader
			connect.Open();
			List<OAI_Record> returnVal = new List<OAI_Record>();
			using (SqlDataReader reader = executeCommand.ExecuteReader())
			{
				// Read in each row
				while (reader.Read())
				{
					returnVal.Add(Include_Record ? new OAI_Record(reader.GetString(1), reader.GetString(2), reader.GetDateTime(3)) : new OAI_Record(reader.GetString(1), reader.GetDateTime(2)));
				}

				// Close the reader
				reader.Close();
			}
			connect.Close();

			return returnVal;
		}

		/// <summary> Returns a single OAI-PMH record, by identifier ( BibID ) </summary>
		/// <param name="Identifier"> Code the OAI-PMH record  (which is really the BibID)</param>
		/// <param name="Data_Code"> Code for the metadata to be served ( usually oai_dc )</param>
		/// <returns> Single OAI-PMH record </returns>
		/// <remarks> This calls the 'SobekCM_Get_OAI_Data_Item' stored procedure  <br /><br />
		/// This is called by the <see cref="Oai_MainWriter"/> class. </remarks> 
		public static OAI_Record Get_OAI_Record( string Identifier, string Data_Code )
		{
			// Create the connection
			SqlConnection connect = new SqlConnection(connectionString);

			// Create the command 
			SqlCommand executeCommand = new SqlCommand("SobekCM_Get_OAI_Data_Item", connect) { CommandType = CommandType.StoredProcedure };

			executeCommand.Parameters.AddWithValue("@bibid", Identifier);
			executeCommand.Parameters.AddWithValue("@data_code", Data_Code);

			// Create the data reader
			connect.Open();
			OAI_Record returnRecord = null;
			using (SqlDataReader reader = executeCommand.ExecuteReader())
			{
				// Read in the first row
				if (reader.Read())
				{
					returnRecord = new OAI_Record(reader.GetString(1), reader.GetString(2), reader.GetDateTime(3));
				}

				// Close the reader
				reader.Close();
			}
			connect.Close();

			return returnRecord;
		}

		#endregion

		#region Methods used by the Statistics Usage Reader

		/// <summary> Gets all the tables ued during the process of reading the statistics 
		/// from the web iis logs and creating the associated SQL commands  </summary>
		/// <returns> Large dataset with several tables ( all items, all titles, aggregationPermissions, etc.. )</returns>
		public static DataSet Get_Statistics_Lookup_Tables()
		{
			// Create the connection
			SqlConnection connect = new SqlConnection(connectionString);

			// Create the command 
			SqlCommand executeCommand = new SqlCommand("SobekCM_Statistics_Lookup_Tables", connect) {CommandType = CommandType.StoredProcedure};

			// Create the adapter
			SqlDataAdapter adapter = new SqlDataAdapter(executeCommand);

			// Create the dataset
			DataSet returnValue = new DataSet();

			// Fill the dataset
			adapter.Fill(returnValue);

			// Return the results
			return returnValue;
		}


		#endregion

		#region Methods used by the Track_Item_MySobekViewer
	 
		/// <summary> Gets the list of users who are Scanning or Processing Technicians </summary>
		/// <returns>DataTable containing users who are Scanning or Processing Technicians</returns>
		public static DataTable Tracking_Get_Users_Scanning_Processing()
		{
			// Create the connection
			SqlConnection connect = new SqlConnection(connectionString);
			
			try
			{
				//Create the command
				SqlCommand cmd = new SqlCommand("dbo.Tracking_Get_Users_Scanning_Processing", connect) {CommandType = CommandType.StoredProcedure};

				//Open the connection
				connect.Open();

				SqlDataAdapter adapter = new SqlDataAdapter(cmd);

				DataTable returnValue = new DataTable();
				adapter.Fill(returnValue);
				
				//Close the connection
				connect.Close();

				//Return the data table
				return (returnValue);

			}
			catch (Exception ee)
			{
				throw new ApplicationException("Error retrieving the list of users who are scanning/processing technicians from the Database"+ee.Message);
			}

		}

		/// <summary> Gets the list of users who are Scanning or Processing Technicians </summary>
		/// <returns>DataTable containing users who are Scanning or Processing Technicians</returns>
		public static DataTable Tracking_Get_Scanners_List()
		{
			// Create the connection
			SqlConnection connect = new SqlConnection(connectionString);

			try
			{
				//Create the command
				SqlCommand cmd = new SqlCommand("dbo.Tracking_Get_Scanners_List", connect) {CommandType = CommandType.StoredProcedure};

				//Open the connection
				connect.Open();

				SqlDataAdapter adapter = new SqlDataAdapter(cmd);

				DataTable returnValue = new DataTable();
				adapter.Fill(returnValue);

				//Close the connection
				connect.Close();

				//Return the data table
				return (returnValue);

			}
			catch (Exception ee)
			{
				throw new ApplicationException("Error retrieving the list of users who are scanning/processing technicians from the Database" + ee.Message);
			}

		}

		/// <summary> Gets the corresponding BibID, VID for a given itemID </summary>
		/// <param name="ItemID"> Primary identifier for this item from the database </param>
		/// <returns> Datarow with the BibID/VID </returns>
		public static DataRow Tracking_Get_Item_Info_from_ItemID(int ItemID)
		{
			// Create the connection
			SqlConnection connect = new SqlConnection(connectionString);

			try
			{
				//Create the command
				SqlCommand cmd = new SqlCommand("Tracking_Get_Item_Info_from_ItemID", connect) {CommandType = CommandType.StoredProcedure};
				cmd.Parameters.AddWithValue("@itemID", ItemID);

				//Open the connection
				connect.Open();

				SqlDataAdapter adapter = new SqlDataAdapter(cmd);

				DataTable returnValue = new DataTable();
				adapter.Fill(returnValue);

				//Close the connection
				connect.Close();

				//Return the data table
				return (returnValue.Rows[0]);

			}
			catch (Exception ee)
			{
				throw new ApplicationException("Error retrieving item details from itemID from the Database" + ee.Message);
			}

		}

		/// <summary> Gets the related workflows for an item by ItemID </summary>
		/// <param name="ItemID"> Primary key for this item in the database </param>
		/// <param name="EventNum"> Number of the event </param>
		/// <returns> DataTable of previously saved workflows for this item</returns>
		public static DataTable Tracking_Get_Open_Workflows(int ItemID, int EventNum)
		{
			// Create the connection
			SqlConnection connect = new SqlConnection(connectionString);

			try
			{
				//Create the command
				SqlCommand cmd = new SqlCommand("SobekCM_Get_Last_Open_Workflow_By_ItemID", connect) {CommandType = CommandType.StoredProcedure};
				cmd.Parameters.AddWithValue("@itemID", ItemID);
				cmd.Parameters.AddWithValue("@EventNumber", EventNum);
			 
				//Open the connection
				connect.Open();

				SqlDataAdapter adapter = new SqlDataAdapter(cmd);

				DataTable returnValue = new DataTable();
				adapter.Fill(returnValue);

				//Close the connection
				connect.Close();

				//Return the data table
				return (returnValue);

			}
			catch (Exception ee)
			{
				throw new ApplicationException("Error retrieving last open workflow by itemID from the Database" + ee.Message);
			}

		}


		/// <summary>Gets all tracking workflow entries created by a single user </summary>
		/// <param name="username">User Name</param>
		/// <returns>DataTable of all previous entries for this user</returns>
		public static DataTable Tracking_Get_All_Entries_By_User(string username)
		{
			DataTable returnValue = new DataTable();
			// Create the connection
			SqlConnection connect = new SqlConnection(connectionString);

			try
			{
				//Create the command
				SqlCommand cmd = new SqlCommand("Tracking_Get_All_Entries_By_User", connect) { CommandType = CommandType.StoredProcedure };
				cmd.Parameters.AddWithValue("@username", username);
				

				//Open the connection
				connect.Open();

				SqlDataAdapter adapter = new SqlDataAdapter(cmd);
				
				adapter.Fill(returnValue);

				//Close the connection
				connect.Close();

				//Return the data table
				return (returnValue);

			}
			catch (Exception ee)
			{
				throw new ApplicationException("Error retrieving previous tracking entries for user "+username+" from the DB. " + ee.Message);
			}

		}


		/// <summary> Save a new workflow entry during tracking</summary>
		/// <param name="itemID"></param>
		/// <param name="workPerformedBy"></param>
		/// <param name="relatedEquipment"></param>
		/// <param name="dateStarted"></param>
		/// <param name="dateCompleted"></param>
		/// <param name="EventNum"></param>
		/// <param name="StartEvent"></param>
		/// <param name="EndEvent"></param>
		/// <param name="Start_End_Event"></param>
		/// <returns></returns>
		public static int Tracking_Save_New_Workflow(int itemID, string workPerformedBy, string relatedEquipment, SqlDateTime dateStarted, SqlDateTime dateCompleted, int EventNum, int StartEvent, int EndEvent, int Start_End_Event)
		{
			int this_workflow_id=-1;  
			// Create the connection
			SqlConnection connect = new SqlConnection(connectionString);

			try
			{
				//Open the connection
				connect.Open();

				//Create the command
				SqlCommand cmd = new SqlCommand("Tracking_Add_New_Workflow", connect) {CommandType = CommandType.StoredProcedure};
				cmd.Parameters.AddWithValue("@itemid", itemID);
				cmd.Parameters.AddWithValue("@user", workPerformedBy);
				cmd.Parameters.AddWithValue("@dateStarted", dateStarted);
				cmd.Parameters.AddWithValue("@dateCompleted", dateCompleted);
				cmd.Parameters.AddWithValue("@relatedEquipment", relatedEquipment);
				cmd.Parameters.AddWithValue("@EventNumber", EventNum);
				cmd.Parameters.AddWithValue("@StartEventNumber", StartEvent);
				cmd.Parameters.AddWithValue("@EndEventNumber", EndEvent);
				cmd.Parameters.AddWithValue("@Start_End_Event", Start_End_Event);
			   
				//Add the output parameter to get back the workflow id for this entry
				SqlParameter outputParam = cmd.Parameters.Add("@workflow_entry_id", SqlDbType.Int);
				outputParam.Direction = ParameterDirection.Output;

				cmd.ExecuteNonQuery();
				this_workflow_id = Convert.ToInt32(outputParam.Value);

			  
			}
			catch (Exception ee)
			{
				throw new ApplicationException("Error saving workflow entry to the database. "+ee.Message);
			}
				connect.Close();
			return this_workflow_id;
		}


/// <summary> Update an already saved tracking workflow entry </summary>
/// <param name="workflowID"></param>
/// <param name="itemID"></param>
/// <param name="workPerformedBy"></param>
/// <param name="dateStarted"></param>
/// <param name="dateCompleted"></param>
/// <param name="relatedEquipment"></param>
/// <param name="eventNumber"></param>
/// <param name="startEventNumber"></param>
/// <param name="endEventNum"></param>
		public static void Tracking_Update_Workflow(int workflowID, int itemID, string workPerformedBy, SqlDateTime dateStarted, SqlDateTime dateCompleted, string relatedEquipment, int eventNumber, int startEventNumber, int endEventNum)
		{
			 // Create the connection
			SqlConnection connect = new SqlConnection(connectionString);

			try
			{
				//Open the connection
				connect.Open();

				//Create the command
				SqlCommand cmd = new SqlCommand("Tracking_Update_Workflow", connect) {CommandType = CommandType.StoredProcedure};
				cmd.Parameters.AddWithValue("@workflow_entry_id", workflowID);
				cmd.Parameters.AddWithValue("@itemid", itemID);
				cmd.Parameters.AddWithValue("@user", workPerformedBy);
				cmd.Parameters.AddWithValue("@dateStarted", dateStarted);
				cmd.Parameters.AddWithValue("@dateCompleted", dateCompleted);
				cmd.Parameters.AddWithValue("@relatedEquipment", relatedEquipment);
				cmd.Parameters.AddWithValue("@EventNumber", eventNumber);
				cmd.Parameters.AddWithValue("@StartEventNumber", startEventNumber);
				cmd.Parameters.AddWithValue("@EndEventNumber", endEventNum);


				cmd.ExecuteNonQuery();
			}
			catch (Exception ee)
			{
				throw new ApplicationException("Error updating tracking workflow "+ee.Message);
			}
			connect.Close();
			
		}


		public static void Tracking_Delete_Workflow(int workflow_id)
		{
			  // Create the connection
			SqlConnection connect = new SqlConnection(connectionString);

			try
			{
				//Open the connection
				connect.Open();

				//Create the command
				SqlCommand cmd = new SqlCommand("Tracking_Delete_Workflow", connect) {CommandType = CommandType.StoredProcedure};
				cmd.Parameters.AddWithValue("@workflow_entry_id", workflow_id);

				cmd.ExecuteNonQuery();
			}
			catch (Exception ee)
			{
				throw new ApplicationException("Error deleting workflow" +ee.Message);
			}
			connect.Close();
		}

		#endregion

		#region Methods supporting USFLDC_Redirection_Service method in SobekCM_URL_Rewriter

		/// <summary> Gets aggregation code from CID in aggregation description</summary>
		/// <param name="cid"> CID for the digital collection </param>
		/// <returns> Aggregation Code </returns>
		public static String Get_AggregationCode_From_CID(String cid)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[1];
				parameters[0] = new SqlParameter("@cid", cid);

				// Define a temporary dataset
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "USF_Get_AggregationCode_From_CID", parameters);

				// If there was no data for this collection and entry point, return null (an ERROR occurred)
				if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
				{
					return null;
				}

				// Return the aggregation code from the first table
				return tempSet.Tables[0].Rows[0][0].ToString();
			}
			catch (Exception ee)
			{
				lastException = ee;
				return null;
			}
		}

		/// <summary> Pulls the BibID, VID via the Identifier </summary>
		/// <param name="identifier"> Identifier (PURL Handle) for the digital resource object </param>
		/// <returns> BibID_VID </returns>
		public static String Get_BibID_VID_From_Identifier(string identifier)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[1];
				parameters[0] = new SqlParameter("@identifier", identifier);
		   
				// Define a temporary dataset
				DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_BibID_VID_From_Identifier", parameters);

				// If there was no data for this collection and entry point, return null (an ERROR occurred)
				if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
				{
					return null;
				}

				// return BibID and VID
				return tempSet.Tables[0].Rows[0][0] + "/" + tempSet.Tables[0].Rows[0][1];
			}
			catch (Exception ee)
			{
				lastException = ee;
				return null;
			}
		}

		#endregion

        #region Methods used to support the SobekCM_Project Element (saving, deleting, retrieving,...)

        /// <summary> Save a new, or edit an existing Project in the database </summary>
        /// <param name="ProjectID"></param>
        /// <param name="ProjectCode"></param>
        /// <param name="ProjectName"></param>
        /// <param name="ProjectManager"></param>
        /// <param name="GrantID"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="isActive"></param>
        /// <param name="Description"></param>
        /// <param name="Specifications"></param>
        /// <param name="Priority"></param>
        /// <param name="QC_Profile"></param>
        /// <param name="TargetItemCount"></param>
        /// <param name="TargetPageCount"></param>
        /// <param name="Comments"></param>
        /// <param name="CopyrightPermissions"></param>
        /// <returns>The ProjectID of the inserted/edited row</returns>
        public static int Save_Project(Custom_Tracer Tracer, int ProjectID, string ProjectCode, string ProjectName, string ProjectManager, string GrantID, DateTime StartDate, DateTime EndDate, bool isActive, string Description, string Specifications, string Priority, string QC_Profile, int TargetItemCount, int TargetPageCount, string Comments, string CopyrightPermissions)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Save_Project", "Saving to the database");
            }

            int New_ProjectID = ProjectID;
           
            // Create the connection
            SqlConnection connect = new SqlConnection(connectionString);

            try
            {
                //Open the connection
                connect.Open();

                //Create the command
                SqlCommand cmd = new SqlCommand("SobekCM_Save_Project", connect) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                cmd.Parameters.AddWithValue("@ProjectCode", ProjectCode);
                cmd.Parameters.AddWithValue("@ProjectName", ProjectName);
                cmd.Parameters.AddWithValue("@ProjectManager", ProjectManager);
                cmd.Parameters.AddWithValue("@GrantID", GrantID);
                cmd.Parameters.AddWithValue("@StartDate", StartDate);
                cmd.Parameters.AddWithValue("@EndDate", EndDate);
                cmd.Parameters.AddWithValue("@isActive", isActive);
                cmd.Parameters.AddWithValue("@Description", Description);
                cmd.Parameters.AddWithValue("@Specifications", Specifications);
                cmd.Parameters.AddWithValue("@Priority", Priority);
                cmd.Parameters.AddWithValue("@QC_Profile", QC_Profile);
                cmd.Parameters.AddWithValue("@TargetItemCount", TargetItemCount);
                cmd.Parameters.AddWithValue("@TargetPageCount", TargetPageCount);
                cmd.Parameters.AddWithValue("@Comments", Comments);
                cmd.Parameters.AddWithValue("@CopyrightPermissions", CopyrightPermissions);
                

                //Add the output parameter to get back the new ProjectID for this newly added project, or existing ProjectID if this has been updated
                SqlParameter outputParam = cmd.Parameters.Add("@New_ProjectID", SqlDbType.Int);
                outputParam.Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                New_ProjectID = Convert.ToInt32(outputParam.Value);

            }
            catch (Exception ee)
            {
                throw new ApplicationException("Error saving this Project to the database. " + ee.Message);
            }
            connect.Close();
            return New_ProjectID;
        }

        /// <summary> Save the new Project-Aggregation link to the database. </summary>
        /// <param name="Tracer"></param>
        /// <param name="ProjectID"></param>
        /// <param name="AggregationID"></param>
        public static void Add_Project_Aggregation_Link(Custom_Tracer Tracer,int ProjectID, int AggregationID)
        {
               if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Add_Project_Aggregation_Link", "Saving link to the database");
            }

          // Create the connection
            SqlConnection connect = new SqlConnection(connectionString);

            try
            {
                //Open the connection
                connect.Open();

                //Create the command
                SqlCommand cmd = new SqlCommand("SobekCM_Save_Project_Aggregation_Link", connect) {CommandType = CommandType.StoredProcedure};
                cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                cmd.Parameters.AddWithValue("@AggregationID", AggregationID);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                throw new ApplicationException("Error adding Project-Aggregation link" + ee.Message);
            }
            connect.Close();
        }

        /// <summary> Save the Project_Default Metadata link to the database </summary>
        /// <param name="Tracer"></param>
        /// <param name="ProjectID"></param>
        /// <param name="DefaultMetadataID"></param>
        public static void Add_Project_DefaultMetadata_Link(Custom_Tracer Tracer, int ProjectID, int DefaultMetadataID)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Add_Project_DefaultMetadata_Link", "Saving link to the database");
            }

            // Create the connection
            SqlConnection connect = new SqlConnection(connectionString);

            try
            {
                //Open the connection
                connect.Open();

                //Create the command
                SqlCommand cmd = new SqlCommand("SobekCM_Save_Project_DefaultMetadata_Link", connect) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                cmd.Parameters.AddWithValue("@DefaultMetadataID", DefaultMetadataID);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                throw new ApplicationException("Error adding Project-DefaultMetadata link" + ee.Message);
            }
            connect.Close();
        }

        /// <summary> Save Project, Input template link to the database </summary>
        /// <param name="Tracer"></param>
        /// <param name="ProjectID"></param>
        /// <param name="TemplateID"></param>
        public static void Add_Project_Template_Link(Custom_Tracer Tracer, int ProjectID, int TemplateID)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Add_Project_Template_Link", "Saving link to the database");
            }

            // Create the connection
            SqlConnection connect = new SqlConnection(connectionString);

            try
            {
                //Open the connection
                connect.Open();

                //Create the command
                SqlCommand cmd = new SqlCommand("SobekCM_Save_Project_Template_Link", connect) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                cmd.Parameters.AddWithValue("@TemplateID", TemplateID);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                throw new ApplicationException("Error adding Project-TemplateID link" + ee.Message);
            }
            connect.Close();
        }

        /// <summary> Save Project, Item link to the database </summary>
        /// <param name="Tracer"></param>
        /// <param name="ProjectID"></param>
        /// <param name="ItemID"></param>
        public static void Add_Project_Item_Link(Custom_Tracer Tracer, int ProjectID, int ItemID)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Add_Project_Item_Link", "Saving link to the database");
            }

            // Create the connection
            SqlConnection connect = new SqlConnection(connectionString);

            try
            {
                //Open the connection
                connect.Open();

                //Create the command
                SqlCommand cmd = new SqlCommand("SobekCM_Save_Project_Item_Link", connect) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                cmd.Parameters.AddWithValue("@ItemID", ItemID);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                throw new ApplicationException("Error adding Project-ItemID link" + ee.Message);
            }
            connect.Close();
        }

        /// <summary> Delete a Project, Item link from the database </summary>
        /// <param name="Tracer"></param>
        /// <param name="ProjectID"></param>
        /// <param name="ItemID"></param>
        public static void Delete_Project_Item_Link(Custom_Tracer Tracer, int ProjectID, int ItemID)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Delete_Project_Item_Link", "Deleting link from the database");
            }

            // Create the connection
            SqlConnection connect = new SqlConnection(connectionString);

            try
            {
                //Open the connection
                connect.Open();

                //Create the command
                SqlCommand cmd = new SqlCommand("SobekCM_Delete_Project_Item_Link", connect) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                cmd.Parameters.AddWithValue("@ItemID", ItemID);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                throw new ApplicationException("Error deleting Project-ItemID link" + ee.Message);
            }
            connect.Close();
        }

        /// <summary> Delete a project, CompleteTemplate link from the database </summary>
        /// <param name="Tracer"></param>
        /// <param name="ProjectID"></param>
        /// <param name="TemplateID"></param>
        public static void Delete_Project_Template_Link(Custom_Tracer Tracer, int ProjectID, int TemplateID)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Delete_Project_Template_Link", "Deleting link from the database");
            }

            // Create the connection
            SqlConnection connect = new SqlConnection(connectionString);

            try
            {
                //Open the connection
                connect.Open();

                //Create the command
                SqlCommand cmd = new SqlCommand("SobekCM_Delete_Project_Template_Link", connect) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                cmd.Parameters.AddWithValue("@TemplateID", TemplateID);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                throw new ApplicationException("Error deleting Project-CompleteTemplate link" + ee.Message);
            }
            connect.Close();
        }

        /// <summary> Delete the Project, default metadata link from the database </summary>
        /// <param name="Tracer"></param>
        /// <param name="ProjectID"></param>
        /// <param name="DefaultMetadataID"></param>
        public static void Delete_Project_DefaultMetadata_Link(Custom_Tracer Tracer, int ProjectID, int DefaultMetadataID)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Delete_Project_DefaultMetadata_Link", "Deleting link from the database");
            }

            // Create the connection
            SqlConnection connect = new SqlConnection(connectionString);

            try
            {
                //Open the connection
                connect.Open();

                //Create the command
                SqlCommand cmd = new SqlCommand("SobekCM_Delete_Project_DefaultMetadata_Link", connect) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                cmd.Parameters.AddWithValue("@DefaultMetadataID", DefaultMetadataID);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                throw new ApplicationException("Error deleting Project-DefaultMetadata link" + ee.Message);
            }
            connect.Close();
        }

        /// <summary> Delete Project, Aggregation Link from the database </summary>
        /// <param name="Tracer"></param>
        /// <param name="ProjectID"></param>
        /// <param name="AggregationID"></param>
        public static void Delete_Project_Aggregation_Link(Custom_Tracer Tracer, int ProjectID, int AggregationID)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Delete_Project_Aggregation_Link", "Deleting link from the database");
            }

            // Create the connection
            SqlConnection connect = new SqlConnection(connectionString);

            try
            {
                //Open the connection
                connect.Open();

                //Create the command
                SqlCommand cmd = new SqlCommand("SobekCM_Delete_Project_Aggregation_Link", connect) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ProjectID", ProjectID);
                cmd.Parameters.AddWithValue("@AggregationID", AggregationID);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                throw new ApplicationException("Error deleting Project-Aggregation link" + ee.Message);
            }
            connect.Close();
        }

        /// <summary> Get the list of Aggregation IDs associated with a project </summary>
        /// <param name="Tracer"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public static List<int> Get_Aggregations_By_ProjectID(Custom_Tracer Tracer, int ProjectID)
        {
            if (Tracer != null)
			{
                Tracer.Add_Trace("SobekCM_Database.Get_Aggregations_By_ProjectID", "Pulling from database");
			}

			try
			{
				List<int> returnValue = new List<int>();

				// Define a temporary dataset
                DataSet tempSet = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, "SobekCM_Get_Aggregations_By_ProjectID");

				// If there was no data for this collection and entry point, return null (an ERROR occurred)
				if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
				{
					return returnValue;
				}

				// Return the first table from the returned dataset
				foreach (DataRow thisRow in tempSet.Tables[0].Rows)
				{
					returnValue.Add(Convert.ToInt32(thisRow[0]));
				}
				return returnValue;
			}
			catch (Exception ee)
			{
				lastException = ee;
				if (Tracer != null)
				{
                    Tracer.Add_Trace("SobekCM_Database.Get_Aggregations_By_ProjectID", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Aggregations_By_ProjectID", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Aggregations_By_ProjectID", ee.StackTrace, Custom_Trace_Type_Enum.Error);
				}
				return null;
			}
	}

        //TODO: Add methods to get the default metadata, current input template
        //TODO: Add the method to get the list of active, inactive projects

        
        #endregion



	}

}
