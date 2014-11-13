#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.ApplicationBlocks.Data;
using SobekCM.Core.Aggregations;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Results;
using SobekCM.Core.Settings;
using SobekCM.Core.Users;
using SobekCM.EngineLibrary.ApplicationState;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Tools;

#endregion

namespace SobekCM.Engine_Library.Database
{
    public static class Engine_Database
    {
        private const int MAX_PAGE_LOOKAHEAD = 4;
        private const int MIN_PAGE_LOOKAHEAD = 2;
        private const int LOOKAHEAD_FACTOR = 5;
        private const int ALL_AGGREGATIONS_METADATA_COUNT_TO_USE_CACHED = 1000;
         
        private static Exception lastException;
        private static readonly Object itemListPopulationLock = new Object();

        /// <summary> Gets the last exception caught by a database call through this gateway class  </summary>
        public static Exception Last_Exception { get; set; }

        /// <summary> Connection string to the main SobekCM databaase </summary>
        /// <remarks> This database hold all the information about items, item aggregationPermissions, statistics, and tracking information</remarks>
        public static string Connection_String { get; set; }

        /// <summary> Test connectivity to the database </summary>
        /// <returns> TRUE if connection can be made, otherwise FALSE </returns>
        public static bool Test_Connection()
        {

            try
            {
                SqlConnection newConnection = new SqlConnection(Connection_String);
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
        public static bool Test_Connection(string Test_Connection_String)
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
                Tracer.Add_Trace("SobekCM_Database.Get_Item_Group_Details", "");
            }

            try
            {
                SqlParameter[] parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("@BibID", BibID);
                parameters[1] = new SqlParameter("@VID", DBNull.Value);

                // Define a temporary dataset
                DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Details2", parameters);

                // Return the first table from the returned dataset
                return tempSet;
            }
            catch (Exception ee)
            {
                lastException = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Group_Details", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Group_Details", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Group_Details", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

        /// <summary> Gets some basic information about an item before displaying it, such as the descriptive notes from the database, ability to add notes, etc.. </summary>
        /// <param name="BibID"> Bibliographic identifier for the volume to retrieve </param>
        /// <param name="VID"> Volume identifier for the volume to retrieve </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> DataSet with detailed information about this item from the database </returns>
        /// <remarks> This calls the 'SobekCM_Get_Item_Details2' stored procedure </remarks> 
        public static DataSet Get_Item_Details(string BibID, string VID, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_Item_Details", "");
            }

            try
            {
                SqlParameter[] parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("@BibID", BibID);
                parameters[1] = new SqlParameter("@VID", VID);


                // Define a temporary dataset
                DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Details2", parameters);

                // Return the first table from the returned dataset
                return tempSet;
            }
            catch (Exception ee)
            {
                lastException = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Details", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Details", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Details", ee.StackTrace, Custom_Trace_Type_Enum.Error);
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
                    Tracer.Add_Trace("SobekCM_Database.Get_IP_Restriction_Ranges", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_IP_Restriction_Ranges", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_IP_Restriction_Ranges", ee.StackTrace, Custom_Trace_Type_Enum.Error);
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
                Tracer.Add_Trace("SobekCM_Database.StopWords", "Pull search stop words from the database");
            }

            // Build return list
            List<string> returnValue = new List<string>();

            try
            {
                // Create the connection
                using (SqlConnection connect = new SqlConnection(Connection_String))
                {
                    SqlCommand executeCommand = new SqlCommand("SobekCM_Get_Search_Stop_Words", connect) { CommandType = CommandType.StoredProcedure };

                    // Create the data reader
                    connect.Open();
                    using (SqlDataReader reader = executeCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Grab the values out
                            returnValue.Add(reader.GetString(1));
                        }
                        reader.Close();
                    }
                    connect.Close();
                }

                // Return the first table from the returned dataset
                return returnValue;
            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.StopWords", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.StopWords", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.StopWords", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

        /// <summary> Populates the collection of the thematic headings for the main home page </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Thematic_Heading_List"> List to populate with the thematic headings from the database</param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Manager_Get_Thematic_Headings' stored procedure </remarks> 
        public static bool Populate_Thematic_Headings(List<Thematic_Heading> Thematic_Heading_List, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Populate_Thematic_Headings", "Pull thematic heading information from the database");
            }

            try
            {

                // Define a temporary dataset
                DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Manager_Get_Thematic_Headings");

                // If there was no data for this collection and entry point, return null (an ERROR occurred)
                if (tempSet.Tables.Count > 0)
                {
                    // Clear the current list
                    Thematic_Heading_List.Clear();

                    // Add them back
                    Thematic_Heading_List.AddRange(from DataRow thisRow in tempSet.Tables[0].Rows select new Thematic_Heading(Convert.ToInt16(thisRow["ThematicHeadingID"]), thisRow["ThemeName"].ToString()));
                }

                // Return the built collection as readonly
                return true;
            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Populate_Thematic_Headings", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Populate_Thematic_Headings", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Populate_Thematic_Headings", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return false;
            }
        }

        /// <summary> Populates the lookup tables for aliases which point to live aggregationPermissions </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Aggregation_Alias_List"> List of aggregation aliases to populate from the database</param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Get_Item_Aggregation_Aliases' stored procedure </remarks> 
        public static bool Populate_Aggregation_Aliases(Dictionary<string, string> Aggregation_Alias_List, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Populate_Aggregation_Aliases", "Pull item aggregation aliases from the database");
            }

            try
            {
                // Define a temporary dataset
                DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Aggregation_Aliases");

                // If there was no data for this collection and entry point, return null (an ERROR occurred)
                if ((tempSet.Tables.Count > 0) || (tempSet.Tables[0].Rows.Count > 0))
                {
                    // Clear the old list
                    Aggregation_Alias_List.Clear();

                    foreach (DataRow thisRow in tempSet.Tables[0].Rows)
                    {
                        Aggregation_Alias_List[thisRow["AggregationAlias"].ToString()] = thisRow["Code"].ToString().ToLower();
                    }
                }

                return true;
            }
            catch (Exception ee)
            {
                Last_Exception = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Populate_Aggregation_Aliases", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Populate_Aggregation_Aliases", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Populate_Aggregation_Aliases", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return false;
            }
        }

        /// <summary> Gets the list of all user groups </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> List of partly built <see cref="Users.User_Group"/> object </returns>
        /// <remarks> This calls the 'mySobek_Get_All_User_Groups' stored procedure </remarks> 
        public static List<User_Group> Get_All_User_Groups(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_All_User_Groups", String.Empty);
            }

            try
            {
                // Execute this non-query stored procedure
                SqlParameter[] paramList = new SqlParameter[1];

                DataSet resultSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "mySobek_Get_All_User_Groups", paramList);

                List<User_Group> returnValue = new List<User_Group>();

                foreach (DataRow thisRow in resultSet.Tables[0].Rows)
                {
                    string name = thisRow["GroupName"].ToString();
                    string description = thisRow["GroupDescription"].ToString();
                    int usergroupid = Convert.ToInt32(thisRow["UserGroupID"]);
                    bool specialGroup = Convert.ToBoolean(thisRow["IsSpecialGroup"]);

                    User_Group userGroup = new User_Group(name, description, usergroupid);
                    userGroup.IsSpecialGroup = specialGroup;

                    returnValue.Add(userGroup);

                }


                return returnValue;

            }
            catch (Exception ee)
            {
                lastException = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Get_All_User_Groups", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_All_User_Groups", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_All_User_Groups", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }


        /// <summary> Datatable with the information for every html skin from the database </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Datatable with all the html skin information to be loaded into the <see cref="Skins.SobekCM_Skin_Collection"/> object. </returns>
        /// <remarks> This calls the 'SobekCM_Get_Web_Skins' stored procedure </remarks> 
        public static DataTable Get_All_Web_Skins(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_All_Skins", "Pull display skin information from the database");
            }

            // Define a temporary dataset
            DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Web_Skins");

            // If there was no data for this collection and entry point, return null (an ERROR occurred)
            if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
            {
                return null;
            }

            // Return the built search fields object
            return tempSet.Tables[0];
        }


        public static List<string> Get_Viewer_Priority(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_Viewer_Priority", "Pulling from database");
            }

            try
            {
                List<string> returnValue = new List<string>();

                // Define a temporary dataset
                DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Viewer_Priority");

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
                lastException = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Get_Viewer_Priority", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Viewer_Priority", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Viewer_Priority", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

        /// <summary> Populates the code manager object for translating SobekCM codes to greenstone collection codes </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Codes"> Code object to populate with the all the code and aggregation information</param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Get_Codes' stored procedure </remarks> 
        public static bool Populate_Code_Manager(Aggregation_Code_Manager Codes, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Populate_Code_Manager", String.Empty);
            }

            // Create the connection
            using (SqlConnection connect = new SqlConnection(Connection_String))
            {
                SqlCommand executeCommand = new SqlCommand("SobekCM_Get_Codes", connect);

                // Create the data reader
                connect.Open();
                using (SqlDataReader reader = executeCommand.ExecuteReader())
                {
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

                    while (reader.Read())
                    {
                        // Get the list key values out 
                        string code = reader.GetString(CODE_COL).ToUpper();
                        string type = reader.GetString(TYPE_COL);
                        int theme = reader.GetInt32(THEME_COL);

                        // Only do anything else if this is not somehow a repeat
                        if (!Codes.isValidCode(code))
                        {
                            // Create the object
                            Item_Aggregation_Related_Aggregations thisAggr =
                                new Item_Aggregation_Related_Aggregations(code, reader.GetString(NAME_COL),
                                                                          reader.GetString(SHORT_NAME_COL), type,
                                                                          reader.GetBoolean(IS_ACTIVE_COL),
                                                                          reader.GetBoolean(HIDDEN_COL),
                                                                          reader.GetString(DESC_COL),
                                                                          (ushort)reader.GetInt32(ID_COL)) { External_Link = reader.GetString(LINK_COL) };

                            // Add this to the codes manager
                            Codes.Add_Collection(thisAggr, theme);
                        }
                    }
                    reader.Close();
                }
                connect.Close();
            }

            // Succesful
            return true;
        }

        /// <summary> Populates the dictionary of all icons from the database </summary>
        /// <param name="Icon_List"> List of icons to be populated with a successful database pulll </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Icon_List' stored procedure <br /><br />
        /// The lookup values in this dictionary are the icon code uppercased.</remarks> 
        public static bool Populate_Icon_List(Dictionary<string, Wordmark_Icon> Icon_List, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Populate_Icon_List", String.Empty);
            }

            // Create the connection
            using (SqlConnection connect = new SqlConnection(Connection_String))
            {
                SqlCommand executeCommand = new SqlCommand("SobekCM_Icon_List", connect) { CommandType = CommandType.StoredProcedure };


                // Create the data reader
                connect.Open();
                using (SqlDataReader reader = executeCommand.ExecuteReader())
                {
                    // Clear existing icons
                    Icon_List.Clear();

                    while (reader.Read())
                    {
                        string code = reader.GetString(0).ToUpper();
                        Icon_List[code] = new Wordmark_Icon(code, reader.GetString(1), reader.GetString(2), reader.GetString(3));
                    }
                    reader.Close();
                }
                connect.Close();
            }

            // Succesful
            return true;
        }

        /// <summary> Populates the dictionary of all files and MIME types from the database </summary>
        /// <param name="MIME_List"> List of files and MIME types to be populated with a successful database pulll </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Get_Mime_Types' stored procedure <br /><br />
        /// The lookup values in this dictionary are the file extensions in lower case.</remarks> 
        public static bool Populate_MIME_List(Dictionary<string, Mime_Type_Info> MIME_List, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Populate_MIME_List", String.Empty);
            }

            // Create the connection
            using (SqlConnection connect = new SqlConnection(Connection_String))
            {
                SqlCommand executeCommand = new SqlCommand("SobekCM_Get_Mime_Types", connect) { CommandType = CommandType.StoredProcedure };


                // Create the data reader
                connect.Open();
                using (SqlDataReader reader = executeCommand.ExecuteReader())
                {
                    // Clear existing icons
                    MIME_List.Clear();

                    while (reader.Read())
                    {
                        string extension = reader.GetString(0).ToLower();
                        MIME_List[extension] = new Mime_Type_Info(extension, reader.GetString(1), reader.GetBoolean(2), reader.GetBoolean(3));
                    }
                    reader.Close();
                }
                connect.Close();
            }

            // Succesful
            return true;
        }

        /// <summary> Populates the date range from the database for which statistical information exists </summary>
        /// <param name="Stats_Date_Object"> Statistical range object to hold the beginning and ending of the statistical information </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Statistics_By_Date_Range' stored procedure <br /><br />
        /// This is used by the <see cref="Statistics_HtmlSubwriter"/> class</remarks>
        public static bool Populate_Statistics_Dates(Statistics_Dates Stats_Date_Object, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Populate_Statistics_Dates", "Pulling statistics date information from database");
            }

            try
            {
                // Execute this query stored procedure
                DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Statistics_Dates");

                // Reset the values in the object and then set from the database result
                Stats_Date_Object.Clear();
                Stats_Date_Object.Set_Statistics_Dates(tempSet.Tables[0]);

                // No error encountered
                return true;
            }
            catch (Exception ee)
            {
                lastException = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Populate_Statistics_Dates", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Populate_Statistics_Dates", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Populate_Statistics_Dates", ee.StackTrace, Custom_Trace_Type_Enum.Error);
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
                Tracer.Add_Trace("SobekCM_Database.Populate_Translations", String.Empty);
            }

            try
            {
                // Create the connection
                using (SqlConnection connect = new SqlConnection(Connection_String))
                {
                    SqlCommand executeCommand = new SqlCommand("SobekCM_Get_Translation", connect) { CommandType = CommandType.StoredProcedure };

                    // Clear the translation information
                    Translations.Clear();

                    // Create the data reader
                    connect.Open();
                    using (SqlDataReader reader = executeCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Translations.Add_French(reader.GetString(1), reader.GetString(2));
                            Translations.Add_Spanish(reader.GetString(1), reader.GetString(3));
                        }
                        reader.Close();
                    }
                    connect.Close();
                }

                // Return the first table from the returned dataset
                return true;
            }
            catch (Exception ee)
            {
                lastException = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Populate_Translations", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Populate_Translations", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Populate_Translations", ee.StackTrace, Custom_Trace_Type_Enum.Error);
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
                Tracer.Add_Trace("SobekCM_Database.Populate_URL_Portals", "Pull URL portal information from the database");
            }

            try
            {
                // Build the parameter list
                SqlParameter[] paramList = new SqlParameter[1];
                paramList[0] = new SqlParameter("@activeonly", true);

                // Define a temporary dataset
                DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Get_All_Portals", paramList);

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
                    Tracer.Add_Trace("SobekCM_Database.Populate_URL_Portals", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Populate_URL_Portals", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Populate_URL_Portals", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
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
            DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "mySobek_Get_All_Template_DefaultMetadatas");
            return tempSet;
        }

        /// <summary> Gets complete information for an item which may be missing from the complete list of items </summary>
        /// <param name="BibID"> Bibliographic identifiers for the item of interest </param>
        /// <param name="VID"> Volume identifiers for the item of interest </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Datarow with additional information about an item, including spatial details, publisher, donor, etc.. </returns>
        /// <remarks> This calls the 'SobekCM_Get_Item_Brief_Info' stored procedure </remarks> 
        public static DataRow Get_Item_Information(string BibID, string VID, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_Item_Information", "Trying to pull information for " + BibID + "_" + VID);
            }

            try
            {
                SqlParameter[] parameters = new SqlParameter[3];
                parameters[0] = new SqlParameter("@bibid", BibID);
                parameters[1] = new SqlParameter("@vid", VID);
                parameters[2] = new SqlParameter("@include_aggregations", false);

                // Define a temporary dataset
                DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Brief_Info", parameters);

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
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Information", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Information", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Information", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

        /// <summary> Gets complete information for an item which may be missing from the complete list of items </summary>
        /// <param name="BibID"> Bibliographic identifiers for the item of interest </param>
        /// <param name="VID"> Volume identifiers for the item of interest </param>
        /// <param name="Include_Aggregations"> Flag indicates whether to include the aggregationPermissions </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Datarow with additional information about an item, including spatial details, publisher, donor, etc.. </returns>
        /// <remarks> This calls the 'SobekCM_Get_Item_Brief_Info' stored procedure </remarks> 
        public static DataSet Get_Item_Information(string BibID, string VID, bool Include_Aggregations, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_Item_Information", "Trying to pull information for " + BibID + "_" + VID);
            }

            try
            {
                SqlParameter[] parameters = new SqlParameter[3];
                parameters[0] = new SqlParameter("@bibid", BibID);
                parameters[1] = new SqlParameter("@vid", VID);
                parameters[2] = new SqlParameter("@include_aggregations", Include_Aggregations);

                // Define a temporary dataset
                DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Brief_Info", parameters);

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
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Information", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Information", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Information", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return null;
            }
        }

        /// <summary> Verified the item lookup object is filled, or populates the item lookup object with all the valid bibids and vids in the system </summary>
        /// <param name="Include_Private"> Flag indicates whether to include private items in this list </param>
        /// <param name="ItemLookupObject"> Item lookup object to directly populate from the database </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> TRUE if successful or if the object is already filled, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Item_List_Web' stored procedure </remarks> 
        internal static bool Verify_Item_Lookup_Object(bool Include_Private, Item_Lookup_Object ItemLookupObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Verify_Item_Lookup_Object", String.Empty);
            }

            // If no database string, don't try to connect
            if (String.IsNullOrEmpty(Connection_String))
                return false;

            lock (itemListPopulationLock)
            {
                bool updateList = true;
                if (ItemLookupObject != null)
                {
                    TimeSpan sinceLastUpdate = DateTime.Now.Subtract(ItemLookupObject.Last_Updated);
                    if (sinceLastUpdate.TotalMinutes <= 1)
                        updateList = false;
                }

                if (!updateList)
                {
                    return true;
                }

                if (ItemLookupObject == null)
                    ItemLookupObject = new Item_Lookup_Object();

                // Have the database popoulate the little bit of bibid/vid information we retain
                bool returnValue = Populate_Item_Lookup_Object(Include_Private, ItemLookupObject, Tracer);
                if (returnValue)
                    ItemLookupObject.Last_Updated = DateTime.Now;
                return returnValue;
            }
        }


        /// <summary> Populates the item lookup object with all the valid bibids and vids in the system </summary>
        /// <param name="Include_Private"> Flag indicates whether to include private items in this list </param>
        /// <param name="ItemLookupObject"> Item lookup object to directly populate from the database </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Item_List_Web' stored procedure </remarks> 
        public static bool Populate_Item_Lookup_Object(bool Include_Private, Item_Lookup_Object ItemLookupObject, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Populate_Item_Lookup_Object", String.Empty);
            }

            try
            {

                // Create the connection
                using (SqlConnection connect = new SqlConnection(Connection_String + "Connection Timeout=45"))
                {

                    SqlCommand executeCommand = new SqlCommand("SobekCM_Item_List", connect) { CommandTimeout = 45, CommandType = CommandType.StoredProcedure };
                    executeCommand.Parameters.AddWithValue("@include_private", Include_Private);

                    // Create the data reader
                    connect.Open();
                    using (SqlDataReader reader = executeCommand.ExecuteReader())
                    {
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
                        reader.Close();
                    }
                    connect.Close();
                }

                // Return the first table from the returned dataset
                return true;
            }
            catch (Exception ee)
            {
                lastException = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Populate_Item_Lookup_Object", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Populate_Item_Lookup_Object", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Populate_Item_Lookup_Object", ee.StackTrace, Custom_Trace_Type_Enum.Error);
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
		/// <param name="Include_Private_Items"> Flag indicates whether to include private items in the result set </param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="AggregationCode"> Code for the aggregation of interest ( or empty string to search all aggregationPermissions )</param>
		/// <param name="DateRange_Start"> If this search includes a date range search, start of the date range, or -1</param>
		/// <param name="DateRange_End"> If this search includes a date range search, end of the date range, or -1</param>
		/// <param name="Include_Facets"> Flag indicates whether to include facets </param>
		/// <param name="Facet_Types"> Primary key for the metadata types to include as facets (up to eight)</param>
		/// <param name="Return_Search_Statistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Small arguments object which contains the page of results and optionally statistics about results for the entire search, including complete counts and facet information </returns>
		/// <remarks> This calls the 'SobekCM_Metadata_Search_Paged' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Perform_Metadata_Search_Paged(int Link1, string Term1, int Field1,
																				int Link2, string Term2, int Field2, int Link3, string Term3, int Field3, int Link4, string Term4, int Field4,
																				int Link5, string Term5, int Field5, int Link6, string Term6, int Field6, int Link7, string Term7, int Field7,
																				int Link8, string Term8, int Field8, int Link9, string Term9, int Field9, int Link10, string Term10, int Field10,
																				bool Include_Private_Items, string AggregationCode, long DateRange_Start, long DateRange_End, 
																				int ResultsPerPage, int ResultsPage, int Sort, bool Include_Facets, 
																				List<short> Facet_Types, bool Return_Search_Statistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Perform_Metadata_Search_Paged", "Performing search in database");
			}

			Multiple_Paged_Results_Args returnArgs;

			// Create the connection
			using (SqlConnection connect = new SqlConnection(Connection_String + "Connection Timeout=45"))
			{

				// Create the command 
				SqlCommand executeCommand = new SqlCommand("SobekCM_Metadata_Search_Paged", connect)
												{CommandTimeout = 45, CommandType = CommandType.StoredProcedure};

                executeCommand.Parameters.AddWithValue("@link1", Link1);
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
				executeCommand.Parameters.AddWithValue("@include_private", Include_Private_Items);
				if (AggregationCode.ToUpper() == "ALL")
					AggregationCode = String.Empty;
				executeCommand.Parameters.AddWithValue("@aggregationcode", AggregationCode);
				executeCommand.Parameters.AddWithValue("@daterange_start", DateRange_Start);
				executeCommand.Parameters.AddWithValue("@daterange_end", DateRange_End);
				executeCommand.Parameters.AddWithValue("@pagesize", ResultsPerPage);
				executeCommand.Parameters.AddWithValue("@pagenumber", ResultsPage);
				executeCommand.Parameters.AddWithValue("@sort", Sort);

				// If this is for more than 100 results, don't look ahead
				if (ResultsPerPage > 100)
				{
					executeCommand.Parameters.AddWithValue("@minpagelookahead", 1);
					executeCommand.Parameters.AddWithValue("@maxpagelookahead", 1);
					executeCommand.Parameters.AddWithValue("@lookahead_factor", LOOKAHEAD_FACTOR);
				}
				else
				{
					executeCommand.Parameters.AddWithValue("@minpagelookahead", MIN_PAGE_LOOKAHEAD);
					executeCommand.Parameters.AddWithValue("@maxpagelookahead", MAX_PAGE_LOOKAHEAD);
					executeCommand.Parameters.AddWithValue("@lookahead_factor", LOOKAHEAD_FACTOR);
				}

				if ((Include_Facets) && (Facet_Types != null) && ( Facet_Types.Count > 0 ) && (Return_Search_Statistics))
				{
					executeCommand.Parameters.AddWithValue("@include_facets", true);
					if (Facet_Types.Count > 0)
						executeCommand.Parameters.AddWithValue("@facettype1", Facet_Types[0]);
					else
						executeCommand.Parameters.AddWithValue("@facettype1", -1);
					if (Facet_Types.Count > 1)
						executeCommand.Parameters.AddWithValue("@facettype2", Facet_Types[1]);
					else
						executeCommand.Parameters.AddWithValue("@facettype2", -1);
					if (Facet_Types.Count > 2)
						executeCommand.Parameters.AddWithValue("@facettype3", Facet_Types[2]);
					else
						executeCommand.Parameters.AddWithValue("@facettype3", -1);
					if (Facet_Types.Count > 3)
						executeCommand.Parameters.AddWithValue("@facettype4", Facet_Types[3]);
					else
						executeCommand.Parameters.AddWithValue("@facettype4", -1);
					if (Facet_Types.Count > 4)
						executeCommand.Parameters.AddWithValue("@facettype5", Facet_Types[4]);
					else
						executeCommand.Parameters.AddWithValue("@facettype5", -1);
					if (Facet_Types.Count > 5)
						executeCommand.Parameters.AddWithValue("@facettype6", Facet_Types[5]);
					else
						executeCommand.Parameters.AddWithValue("@facettype6", -1);
					if (Facet_Types.Count > 6)
						executeCommand.Parameters.AddWithValue("@facettype7", Facet_Types[6]);
					else
						executeCommand.Parameters.AddWithValue("@facettype7", -1);
					if (Facet_Types.Count > 7)
						executeCommand.Parameters.AddWithValue("@facettype8", Facet_Types[7]);
					else
						executeCommand.Parameters.AddWithValue("@facettype8", -1);
				}
				else
				{
					executeCommand.Parameters.AddWithValue("@include_facets", false);
					executeCommand.Parameters.AddWithValue("@facettype1", -1);
					executeCommand.Parameters.AddWithValue("@facettype2", -1);
					executeCommand.Parameters.AddWithValue("@facettype3", -1);
					executeCommand.Parameters.AddWithValue("@facettype4", -1);
					executeCommand.Parameters.AddWithValue("@facettype5", -1);
					executeCommand.Parameters.AddWithValue("@facettype6", -1);
					executeCommand.Parameters.AddWithValue("@facettype7", -1);
					executeCommand.Parameters.AddWithValue("@facettype8", -1);
				}

				// Add parameters for total items and total titles
				SqlParameter totalItemsParameter = executeCommand.Parameters.AddWithValue("@total_items", 0);
				totalItemsParameter.Direction = ParameterDirection.InputOutput;

				SqlParameter totalTitlesParameter = executeCommand.Parameters.AddWithValue("@total_titles", 0);
				totalTitlesParameter.Direction = ParameterDirection.InputOutput;

				// Add parameters for items and titles if this search is expanded to include all aggregationPermissions
				SqlParameter expandedItemsParameter = executeCommand.Parameters.AddWithValue("@all_collections_items", 0);
				expandedItemsParameter.Direction = ParameterDirection.InputOutput;

				SqlParameter expandedTitlesParameter = executeCommand.Parameters.AddWithValue("@all_collections_titles", 0);
				expandedTitlesParameter.Direction = ParameterDirection.InputOutput;

				// Create the data reader
				connect.Open();
				using (SqlDataReader reader = executeCommand.ExecuteReader())
				{

					// Create the return argument object
					List<string> metadataLabels = new List<string>(); 
					returnArgs = new Multiple_Paged_Results_Args
									 {Paged_Results = DataReader_To_Result_List_With_LookAhead2(reader, ResultsPerPage, metadataLabels)};

					// Create the overall search statistics?
					if (Return_Search_Statistics)
					{
						Search_Results_Statistics stats = new Search_Results_Statistics(reader, Facet_Types, metadataLabels);
						returnArgs.Statistics = stats;
						reader.Close();
						stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
						stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
						stats.All_Collections_Items = Convert.ToInt32(expandedItemsParameter.Value);
						stats.All_Collections_Titles = Convert.ToInt32(expandedTitlesParameter.Value);
					}
					else
					{
						reader.Close();
					}
				}
				connect.Close();
			}

			// Return the built result arguments
			return returnArgs;
		}

		/// <summary> Performs a basic metadata search over the entire citation, given a search condition, and returns one page of results </summary>
		/// <param name="Search_Condition"> Search condition string to be run against the databasse </param>
		/// <param name="Include_Private_Items"> Flag indicates whether to include private items in the result set </param>
		/// <param name="AggregationCode"> Code for the aggregation of interest ( or empty string to search all aggregationPermissions )</param>
		/// <param name="DateRange_Start"> If this search includes a date range search, start of the date range, or -1</param>
		/// <param name="DateRange_End"> If this search includes a date range search, end of the date range, or -1</param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="Include_Facets"> Flag indicates whether to include facets in the result set </param>
		/// <param name="Facet_Types"> Primary key for the metadata types to include as facets (up to eight)</param>
		/// <param name="Return_Search_Statistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Small arguments object which contains the page of results and optionally statistics about results for the entire search, including complete counts and facet information </returns>
		/// <remarks> This calls the 'SobekCM_Metadata_Basic_Search_Paged' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Perform_Metadata_Search_Paged( string Search_Condition, bool Include_Private_Items, string AggregationCode, long DateRange_Start, long DateRange_End, int ResultsPerPage, int ResultsPage, int Sort, bool Include_Facets, List<short> Facet_Types, bool Return_Search_Statistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Perform_Basic_Search_Paged", "Performing basic search in database");
			}

			Multiple_Paged_Results_Args returnArgs;

			// Create the connection
			using (SqlConnection connect = new SqlConnection(Connection_String + "Connection Timeout=45"))
			{

				// Create the command 
				SqlCommand executeCommand = new SqlCommand("SobekCM_Metadata_Basic_Search_Paged2", connect)
												{CommandTimeout = 45, CommandType = CommandType.StoredProcedure};

				executeCommand.Parameters.AddWithValue("@searchcondition", Search_Condition.Replace("''","'"));
				executeCommand.Parameters.AddWithValue("@include_private", Include_Private_Items);
				if (AggregationCode.ToUpper() == "ALL")
					AggregationCode = String.Empty;
				executeCommand.Parameters.AddWithValue("@aggregationcode", AggregationCode);
				executeCommand.Parameters.AddWithValue("@daterange_start", DateRange_Start);
				executeCommand.Parameters.AddWithValue("@daterange_end", DateRange_End);
				executeCommand.Parameters.AddWithValue("@pagesize", ResultsPerPage);
				executeCommand.Parameters.AddWithValue("@pagenumber", ResultsPage);
				executeCommand.Parameters.AddWithValue("@sort", Sort);

				// If this is for more than 100 results, don't look ahead
				if (ResultsPerPage > 100)
				{
					executeCommand.Parameters.AddWithValue("@minpagelookahead", 1);
					executeCommand.Parameters.AddWithValue("@maxpagelookahead", 1);
					executeCommand.Parameters.AddWithValue("@lookahead_factor", LOOKAHEAD_FACTOR);
				}
				else
				{
					executeCommand.Parameters.AddWithValue("@minpagelookahead", MIN_PAGE_LOOKAHEAD);
					executeCommand.Parameters.AddWithValue("@maxpagelookahead", MAX_PAGE_LOOKAHEAD);
					executeCommand.Parameters.AddWithValue("@lookahead_factor", LOOKAHEAD_FACTOR);
				}

				if ((Include_Facets) && (Facet_Types != null) && ( Facet_Types.Count > 0 ) && (Return_Search_Statistics))
				{
					executeCommand.Parameters.AddWithValue("@include_facets", true);
					if (Facet_Types.Count > 0)
						executeCommand.Parameters.AddWithValue("@facettype1", Facet_Types[0]);
					else
						executeCommand.Parameters.AddWithValue("@facettype1", -1);
					if (Facet_Types.Count > 1)
						executeCommand.Parameters.AddWithValue("@facettype2", Facet_Types[1]);
					else
						executeCommand.Parameters.AddWithValue("@facettype2", -1);
					if (Facet_Types.Count > 2)
						executeCommand.Parameters.AddWithValue("@facettype3", Facet_Types[2]);
					else
						executeCommand.Parameters.AddWithValue("@facettype3", -1);
					if (Facet_Types.Count > 3)
						executeCommand.Parameters.AddWithValue("@facettype4", Facet_Types[3]);
					else
						executeCommand.Parameters.AddWithValue("@facettype4", -1);
					if (Facet_Types.Count > 4)
						executeCommand.Parameters.AddWithValue("@facettype5", Facet_Types[4]);
					else
						executeCommand.Parameters.AddWithValue("@facettype5", -1);
					if (Facet_Types.Count > 5)
						executeCommand.Parameters.AddWithValue("@facettype6", Facet_Types[5]);
					else
						executeCommand.Parameters.AddWithValue("@facettype6", -1);
					if (Facet_Types.Count > 6)
						executeCommand.Parameters.AddWithValue("@facettype7", Facet_Types[6]);
					else
						executeCommand.Parameters.AddWithValue("@facettype7", -1);
					if (Facet_Types.Count > 7)
						executeCommand.Parameters.AddWithValue("@facettype8", Facet_Types[7]);
					else
						executeCommand.Parameters.AddWithValue("@facettype8", -1);
				}
				else
				{
					executeCommand.Parameters.AddWithValue("@include_facets", false);
					executeCommand.Parameters.AddWithValue("@facettype1", -1);
					executeCommand.Parameters.AddWithValue("@facettype2", -1);
					executeCommand.Parameters.AddWithValue("@facettype3", -1);
					executeCommand.Parameters.AddWithValue("@facettype4", -1);
					executeCommand.Parameters.AddWithValue("@facettype5", -1);
					executeCommand.Parameters.AddWithValue("@facettype6", -1);
					executeCommand.Parameters.AddWithValue("@facettype7", -1);
					executeCommand.Parameters.AddWithValue("@facettype8", -1);
				}

				// Add parameters for total items and total titles
				SqlParameter totalItemsParameter = executeCommand.Parameters.AddWithValue("@total_items", 0);
				totalItemsParameter.Direction = ParameterDirection.InputOutput;

				SqlParameter totalTitlesParameter = executeCommand.Parameters.AddWithValue("@total_titles", 0);
				totalTitlesParameter.Direction = ParameterDirection.InputOutput;

				// Add parameters for items and titles if this search is expanded to include all aggregationPermissions
				SqlParameter expandedItemsParameter = executeCommand.Parameters.AddWithValue("@all_collections_items", 0);
				expandedItemsParameter.Direction = ParameterDirection.InputOutput;

				SqlParameter expandedTitlesParameter = executeCommand.Parameters.AddWithValue("@all_collections_titles", 0);
				expandedTitlesParameter.Direction = ParameterDirection.InputOutput;

				// Create the data reader
				connect.Open();
				using (SqlDataReader reader = executeCommand.ExecuteReader())
				{
					// Create the return argument object
					List<string> metadataLabels = new List<string>(); 
					returnArgs = new Multiple_Paged_Results_Args
									 {Paged_Results = DataReader_To_Result_List_With_LookAhead2(reader, ResultsPerPage, metadataLabels)};

					// Create the overall search statistics?
					if (Return_Search_Statistics)
					{
						Search_Results_Statistics stats = new Search_Results_Statistics(reader, Facet_Types, metadataLabels);
						returnArgs.Statistics = stats;
						reader.Close();
						stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
						stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
						stats.All_Collections_Items = Convert.ToInt32(expandedItemsParameter.Value);
						stats.All_Collections_Titles = Convert.ToInt32(expandedTitlesParameter.Value);
					}
					else
					{
						reader.Close();
					}
				}
				connect.Close();
			}

			// Return the built result arguments
			return returnArgs;
		}

		/// <summary> Performs a metadata search for a piece of metadata that EXACTLY matches the provided search term and return one page of results </summary>
		/// <param name="Search_Term"> Search condition string to be run against the databasse </param>
		/// <param name="FieldID"> Primary key for the field to search in the database </param>
		/// <param name="Include_Private_Items"> Flag indicates whether to include private items in the result set </param>
		/// <param name="AggregationCode"> Code for the aggregation of interest ( or empty string to search all aggregationPermissions )</param>
		/// <param name="DateRange_Start"> If this search includes a date range search, start of the date range, or -1</param>
		/// <param name="DateRange_End"> If this search includes a date range search, end of the date range, or -1</param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="Include_Facets"> Flag indicates whether to include facets in the result set </param>
		/// <param name="Facet_Types"> Primary key for the metadata types to include as facets (up to eight)</param>
		/// <param name="Return_Search_Statistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Small arguments object which contains the page of results and optionally statistics about results for the entire search, including complete counts and facet information </returns>
		/// <remarks> This calls the 'SobekCM_Metadata_Exact_Search_Paged' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Perform_Metadata_Exact_Search_Paged(string Search_Term, int FieldID, bool Include_Private_Items, string AggregationCode, long DateRange_Start, long DateRange_End, int ResultsPerPage, int ResultsPage, int Sort, bool Include_Facets, List<short> Facet_Types, bool Return_Search_Statistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Perform_Metadata_Exact_Search_Paged", "Performing exact search in database");
			}

			Multiple_Paged_Results_Args returnArgs;

			// Create the connection
			using (SqlConnection connect = new SqlConnection(Connection_String + "Connection Timeout=45"))
			{

				// Create the command 
				SqlCommand executeCommand = new SqlCommand("SobekCM_Metadata_Exact_Search_Paged2", connect)
												{CommandTimeout = 45, CommandType = CommandType.StoredProcedure};

				executeCommand.Parameters.AddWithValue("@term1", Search_Term);
				executeCommand.Parameters.AddWithValue("@field1", FieldID);
				executeCommand.Parameters.AddWithValue("@include_private", Include_Private_Items);
				if (AggregationCode.ToUpper() == "ALL")
					AggregationCode = String.Empty;
				executeCommand.Parameters.AddWithValue("@aggregationcode", AggregationCode);
				executeCommand.Parameters.AddWithValue("@daterange_start", DateRange_Start);
				executeCommand.Parameters.AddWithValue("@daterange_end", DateRange_End);
				executeCommand.Parameters.AddWithValue("@pagesize", ResultsPerPage);
				executeCommand.Parameters.AddWithValue("@pagenumber", ResultsPage);
				executeCommand.Parameters.AddWithValue("@sort", Sort);

				// If this is for more than 100 results, don't look ahead
				if (ResultsPerPage > 100)
				{
					executeCommand.Parameters.AddWithValue("@minpagelookahead", 1);
					executeCommand.Parameters.AddWithValue("@maxpagelookahead", 1);
					executeCommand.Parameters.AddWithValue("@lookahead_factor", LOOKAHEAD_FACTOR);
				}
				else
				{
					executeCommand.Parameters.AddWithValue("@minpagelookahead", MIN_PAGE_LOOKAHEAD);
					executeCommand.Parameters.AddWithValue("@maxpagelookahead", MAX_PAGE_LOOKAHEAD);
					executeCommand.Parameters.AddWithValue("@lookahead_factor", LOOKAHEAD_FACTOR);
				}

				if ((Include_Facets) && (Facet_Types != null) && ( Facet_Types.Count > 0 ) && (Return_Search_Statistics))
				{
					executeCommand.Parameters.AddWithValue("@include_facets", true);
					if (Facet_Types.Count > 0)
						executeCommand.Parameters.AddWithValue("@facettype1", Facet_Types[0]);
					else
						executeCommand.Parameters.AddWithValue("@facettype1", -1);
					if (Facet_Types.Count > 1)
						executeCommand.Parameters.AddWithValue("@facettype2", Facet_Types[1]);
					else
						executeCommand.Parameters.AddWithValue("@facettype2", -1);
					if (Facet_Types.Count > 2)
						executeCommand.Parameters.AddWithValue("@facettype3", Facet_Types[2]);
					else
						executeCommand.Parameters.AddWithValue("@facettype3", -1);
					if (Facet_Types.Count > 3)
						executeCommand.Parameters.AddWithValue("@facettype4", Facet_Types[3]);
					else
						executeCommand.Parameters.AddWithValue("@facettype4", -1);
					if (Facet_Types.Count > 4)
						executeCommand.Parameters.AddWithValue("@facettype5", Facet_Types[4]);
					else
						executeCommand.Parameters.AddWithValue("@facettype5", -1);
					if (Facet_Types.Count > 5)
						executeCommand.Parameters.AddWithValue("@facettype6", Facet_Types[5]);
					else
						executeCommand.Parameters.AddWithValue("@facettype6", -1);
					if (Facet_Types.Count > 6)
						executeCommand.Parameters.AddWithValue("@facettype7", Facet_Types[6]);
					else
						executeCommand.Parameters.AddWithValue("@facettype7", -1);
					if (Facet_Types.Count > 7)
						executeCommand.Parameters.AddWithValue("@facettype8", Facet_Types[7]);
					else
						executeCommand.Parameters.AddWithValue("@facettype8", -1);
				}
				else
				{
					executeCommand.Parameters.AddWithValue("@include_facets", false);
					executeCommand.Parameters.AddWithValue("@facettype1", -1);
					executeCommand.Parameters.AddWithValue("@facettype2", -1);
					executeCommand.Parameters.AddWithValue("@facettype3", -1);
					executeCommand.Parameters.AddWithValue("@facettype4", -1);
					executeCommand.Parameters.AddWithValue("@facettype5", -1);
					executeCommand.Parameters.AddWithValue("@facettype6", -1);
					executeCommand.Parameters.AddWithValue("@facettype7", -1);
					executeCommand.Parameters.AddWithValue("@facettype8", -1);
				}

				// Add parameters for total items and total titles
				SqlParameter totalItemsParameter = executeCommand.Parameters.AddWithValue("@total_items", 0);
				totalItemsParameter.Direction = ParameterDirection.InputOutput;

				SqlParameter totalTitlesParameter = executeCommand.Parameters.AddWithValue("@total_titles", 0);
				totalTitlesParameter.Direction = ParameterDirection.InputOutput;

				// Add parameters for items and titles if this search is expanded to include all aggregationPermissions
				SqlParameter expandedItemsParameter = executeCommand.Parameters.AddWithValue("@all_collections_items", 0);
				expandedItemsParameter.Direction = ParameterDirection.InputOutput;

				SqlParameter expandedTitlesParameter = executeCommand.Parameters.AddWithValue("@all_collections_titles", 0);
				expandedTitlesParameter.Direction = ParameterDirection.InputOutput;

				// Create the data reader
				connect.Open();
				using (SqlDataReader reader = executeCommand.ExecuteReader())
				{
					if (Tracer != null)
					{
						Tracer.Add_Trace("SobekCM_Database.Perform_Metadata_Exact_Search_Paged", "Building result object from returned value");
					}

					// Create the return argument object
					List<string> metadataLabels = new List<string>(); 
					returnArgs = new Multiple_Paged_Results_Args
									 {Paged_Results = DataReader_To_Result_List_With_LookAhead2(reader, ResultsPerPage, metadataLabels)};

					// Create the overall search statistics?
					if (Return_Search_Statistics)
					{
						Search_Results_Statistics stats = new Search_Results_Statistics(reader, Facet_Types, metadataLabels);
						returnArgs.Statistics = stats;
						reader.Close();
						stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
						stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
						stats.All_Collections_Items = Convert.ToInt32(expandedItemsParameter.Value);
						stats.All_Collections_Titles = Convert.ToInt32(expandedTitlesParameter.Value);
					}
					else
					{
						reader.Close();
					}
				}
				connect.Close();
			}

			// Return the built result arguments
			return returnArgs;
		}

		private static List<List<iSearch_Title_Result>> DataReader_To_Result_List_With_LookAhead2(IDataReader Reader, int ResultsPerPage, List<string> Metadata_Field_Names )
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
				string spatialKML = Reader.GetString(15);
				string cOinSOpenURL = Reader.GetString(16);

				titleResult.Spatial_Coordinates = spatialKML;


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
					Spatial_KML = spatialKML,
					COinS_OpenURL = cOinSOpenURL
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
						Metadata_Field_Names.Add(Reader.GetName(i+1));
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

		private static List<iSearch_Title_Result> DataReader_To_Simple_Result_List2(SqlDataReader Reader, List<string> Metadata_Field_Names)
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
						Metadata_Field_Names.Add(Reader.GetName(i + 1));
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
		/// <param name="Latitude_1"> Latitudinal portion of the first point making up the rectangular bounding box</param>
		/// <param name="Longitude_1"> Longitudinal portion of the first point making up the rectangular bounding box</param>
		/// <param name="Latitude_2"> Latitudinal portion of the second point making up the rectangular bounding box</param>
		/// <param name="Longitude_2"> Longitudinal portion of the second point making up the rectangular bounding box</param>
		/// <param name="Include_Private_Items"> Flag indicates whether to include private items in the result set </param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="Include_Facets"> Flag indicates if facets should be included in the result set </param>
		/// <param name="Facet_Types"> Primary key for the metadata types to include as facets (up to eight)</param>
		/// <param name="Return_Search_Statistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Table with all of the item and item group information within provided bounding box </returns>
		/// <remarks> This calls the 'SobekCM_Get_Items_By_Coordinates' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Get_Items_By_Coordinates(string AggregationCode, double Latitude_1, double Longitude_1, double Latitude_2, double Longitude_2, bool Include_Private_Items, int ResultsPerPage, int ResultsPage, int Sort, bool Include_Facets, List<short> Facet_Types, bool Return_Search_Statistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Get_Items_By_Coordinates", "Pulling data from database");
			}

			Multiple_Paged_Results_Args returnArgs;

			// Create the connection
			using (SqlConnection connect = new SqlConnection(Connection_String + "Connection Timeout=45"))
			{

				// Create the command 
				SqlCommand executeCommand = new SqlCommand("SobekCM_Get_Items_By_Coordinates", connect)
												{CommandType = CommandType.StoredProcedure};
				executeCommand.Parameters.AddWithValue("@lat1", Latitude_1);
				executeCommand.Parameters.AddWithValue("@long1", Longitude_1);
				executeCommand.Parameters.AddWithValue("@lat2", Latitude_2);
				executeCommand.Parameters.AddWithValue("@long2", Longitude_2);
				executeCommand.Parameters.AddWithValue("@include_private", Include_Private_Items);
				executeCommand.Parameters.AddWithValue("@pagesize", ResultsPerPage);
				executeCommand.Parameters.AddWithValue("@pagenumber", ResultsPage);
				executeCommand.Parameters.AddWithValue("@sort", Sort);
				executeCommand.Parameters.AddWithValue("@minpagelookahead", MIN_PAGE_LOOKAHEAD);
				executeCommand.Parameters.AddWithValue("@maxpagelookahead", MAX_PAGE_LOOKAHEAD);
				executeCommand.Parameters.AddWithValue("@lookahead_factor", LOOKAHEAD_FACTOR);
				executeCommand.Parameters.AddWithValue("@aggregationcode", AggregationCode);
				executeCommand.Parameters.AddWithValue("@include_facets", Include_Facets);

				if ((Include_Facets) && (Facet_Types != null) && (Return_Search_Statistics))
				{
					if (Facet_Types.Count > 0)
						executeCommand.Parameters.AddWithValue("@facettype1", Facet_Types[0]);
					else
						executeCommand.Parameters.AddWithValue("@facettype1", -1);
					if (Facet_Types.Count > 1)
						executeCommand.Parameters.AddWithValue("@facettype2", Facet_Types[1]);
					else
						executeCommand.Parameters.AddWithValue("@facettype2", -1);
					if (Facet_Types.Count > 2)
						executeCommand.Parameters.AddWithValue("@facettype3", Facet_Types[2]);
					else
						executeCommand.Parameters.AddWithValue("@facettype3", -1);
					if (Facet_Types.Count > 3)
						executeCommand.Parameters.AddWithValue("@facettype4", Facet_Types[3]);
					else
						executeCommand.Parameters.AddWithValue("@facettype4", -1);
					if (Facet_Types.Count > 4)
						executeCommand.Parameters.AddWithValue("@facettype5", Facet_Types[4]);
					else
						executeCommand.Parameters.AddWithValue("@facettype5", -1);
					if (Facet_Types.Count > 5)
						executeCommand.Parameters.AddWithValue("@facettype6", Facet_Types[5]);
					else
						executeCommand.Parameters.AddWithValue("@facettype6", -1);
					if (Facet_Types.Count > 6)
						executeCommand.Parameters.AddWithValue("@facettype7", Facet_Types[6]);
					else
						executeCommand.Parameters.AddWithValue("@facettype7", -1);
					if (Facet_Types.Count > 7)
						executeCommand.Parameters.AddWithValue("@facettype8", Facet_Types[7]);
					else
						executeCommand.Parameters.AddWithValue("@facettype8", -1);
				}
				else
				{
					executeCommand.Parameters.AddWithValue("@facettype1", -1);
					executeCommand.Parameters.AddWithValue("@facettype2", -1);
					executeCommand.Parameters.AddWithValue("@facettype3", -1);
					executeCommand.Parameters.AddWithValue("@facettype4", -1);
					executeCommand.Parameters.AddWithValue("@facettype5", -1);
					executeCommand.Parameters.AddWithValue("@facettype6", -1);
					executeCommand.Parameters.AddWithValue("@facettype7", -1);
					executeCommand.Parameters.AddWithValue("@facettype8", -1);
				}

				// Add parameters for total items and total titles
				SqlParameter totalItemsParameter = executeCommand.Parameters.AddWithValue("@total_items", 0);
				totalItemsParameter.Direction = ParameterDirection.InputOutput;

				SqlParameter totalTitlesParameter = executeCommand.Parameters.AddWithValue("@total_titles", 0);
				totalTitlesParameter.Direction = ParameterDirection.InputOutput;

				// Create the data reader
				connect.Open();
				using (SqlDataReader reader = executeCommand.ExecuteReader())
				{
					List<string> metadataFields = new List<string>();
					// Create the return argument object
					returnArgs = new Multiple_Paged_Results_Args { Paged_Results = DataReader_To_Result_List_With_LookAhead2(reader, ResultsPerPage, metadataFields) };

					// Create the overall search statistics?
					if (Return_Search_Statistics)
					{
						Search_Results_Statistics stats = new Search_Results_Statistics(reader, Facet_Types, metadataFields);
						returnArgs.Statistics = stats;
						reader.Close();
						stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
						stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
					}
					else
					{
						reader.Close();
					}
				}
				connect.Close();
			}

			// Return the built result arguments
			return returnArgs;
		}

		#endregion

		#region Methods to retrieve item list by OCLC or ALEPH number

		/// <summary> Returns the list of all items/titles which match a given OCLC number </summary>
		/// <param name="OCLC_Number"> OCLC number to look for matching items </param>
		/// <param name="Include_Private_Items"> Flag indicates whether to include private items in the result set </param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="Return_Search_Statistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Table with all of the item and item group information which matches the OCLC number </returns>
		/// <remarks> This calls the 'SobekCM_Items_By_OCLC' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Items_By_OCLC_Number(long OCLC_Number, bool Include_Private_Items, int ResultsPerPage, int Sort, bool Return_Search_Statistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Items_By_OCLC_Number", "Searching by OCLC in the database");
			}

			// Build the parameter list
			SqlParameter[] paramList = new SqlParameter[5];
			paramList[0] = new SqlParameter("@oclc_number", OCLC_Number);
			paramList[1] = new SqlParameter("@include_private", Include_Private_Items);
			paramList[2] = new SqlParameter("@sort", Sort);
			paramList[3] = new SqlParameter("@total_items", 0) {Direction = ParameterDirection.InputOutput};
			paramList[4] = new SqlParameter("@total_titles", 0) {Direction = ParameterDirection.InputOutput};

			// Get the matching reader            
			Multiple_Paged_Results_Args returnArgs;
			using (SqlDataReader reader = SqlHelper.ExecuteReader(Connection_String, CommandType.StoredProcedure, "SobekCM_Items_By_OCLC", paramList))
			{
				List<string>  metadataFields = new List<string>();

				// Create the return argument object
				returnArgs = new Multiple_Paged_Results_Args { Paged_Results = DataReader_To_Result_List_With_LookAhead2(reader, ResultsPerPage, metadataFields) };

				// Create the overall search statistics?
				if (Return_Search_Statistics)
				{
					Search_Results_Statistics stats = new Search_Results_Statistics(reader, null, metadataFields);
					returnArgs.Statistics = stats;
					reader.Close();
					stats.Total_Items = Convert.ToInt32(paramList[3].Value);
					stats.Total_Titles = Convert.ToInt32(paramList[4].Value);
				}
				else
				{
					reader.Close();
				}
			}

			// Return the built results
			return returnArgs;
		}

		/// <summary> Returns the list of all items/titles which match a given ALEPH number </summary>
		/// <param name="ALEPH_Number"> ALEPH number to look for matching items </param>
		/// <param name="Include_Private_Items"> Flag indicates whether to include private items in the result set </param>
		/// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
		/// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
		/// <param name="Return_Search_Statistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <returns> Table with all of the item and item group information which matches the ALEPH number </returns>
		/// <remarks> This calls the 'SobekCM_Items_By_ALEPH' stored procedure </remarks>
		public static Multiple_Paged_Results_Args Items_By_ALEPH_Number(int ALEPH_Number, bool Include_Private_Items, int ResultsPerPage, int Sort, bool Return_Search_Statistics, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("SobekCM_Database.Items_By_ALEPH_Number", "Searching by ALEPH in the database");
			}

			// Build the parameter list
			SqlParameter[] paramList = new SqlParameter[5];
			paramList[0] = new SqlParameter("@aleph_number", ALEPH_Number);
			paramList[1] = new SqlParameter("@include_private", Include_Private_Items);
			paramList[2] = new SqlParameter("@sort", Sort);
			paramList[3] = new SqlParameter("@total_items", 0) {Direction = ParameterDirection.InputOutput};
			paramList[4] = new SqlParameter("@total_titles", 0) {Direction = ParameterDirection.InputOutput};

			// Get the matching reader            
			Multiple_Paged_Results_Args returnArgs;
			using (SqlDataReader reader = SqlHelper.ExecuteReader(Connection_String, CommandType.StoredProcedure, "SobekCM_Items_By_ALEPH", paramList))
			{
				List<string> metadataFields = new List<string>();
					
				// Create the return argument object
				returnArgs = new Multiple_Paged_Results_Args
								 {Paged_Results = DataReader_To_Result_List_With_LookAhead2(reader, ResultsPerPage, metadataFields)};

				// Create the overall search statistics?
				if (Return_Search_Statistics)
				{
					Search_Results_Statistics stats = new Search_Results_Statistics(reader, null, metadataFields);
					returnArgs.Statistics = stats;
					reader.Close();
					stats.Total_Items = Convert.ToInt32(paramList[3].Value);
					stats.Total_Titles = Convert.ToInt32(paramList[4].Value);
				}
				else
				{
					reader.Close();
				}
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
        /// <param name="Include_Facets"> Flag indicates if facets should be included in the final result set</param>
        /// <param name="Facet_Types"> Primary key for the metadata types to include as facets (up to eight)</param>
        /// <param name="Return_Search_Statistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> List of items matching search </returns>
        /// <remarks> This calls the 'mySobek_Get_User_Folder_Browse' stored procedure</remarks> 
        public static Single_Paged_Results_Args Get_User_Folder_Browse(int UserID, string FolderName, int ResultsPerPage, int ResultsPage, bool Include_Facets, List<short> Facet_Types, bool Return_Search_Statistics, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_User_Folder_Browse", String.Empty);
            }

            Single_Paged_Results_Args returnArgs;

            // Create the connection
            using (SqlConnection connect = new SqlConnection(Connection_String + "Connection Timeout=45"))
            {

                // Create the command 
                SqlCommand executeCommand = new SqlCommand("mySobek_Get_User_Folder_Browse", connect) { CommandTimeout = 45, CommandType = CommandType.StoredProcedure };

                executeCommand.Parameters.AddWithValue("@userid", UserID);
                executeCommand.Parameters.AddWithValue("@foldername", FolderName);
                executeCommand.Parameters.AddWithValue("@pagesize", ResultsPerPage);
                executeCommand.Parameters.AddWithValue("@pagenumber", ResultsPage);
                executeCommand.Parameters.AddWithValue("@include_facets", Include_Facets);
                if ((Include_Facets) && (Facet_Types != null))
                {
                    if (Facet_Types.Count > 0)
                        executeCommand.Parameters.AddWithValue("@facettype1", Facet_Types[0]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype1", -1);
                    if (Facet_Types.Count > 1)
                        executeCommand.Parameters.AddWithValue("@facettype2", Facet_Types[1]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype2", -1);
                    if (Facet_Types.Count > 2)
                        executeCommand.Parameters.AddWithValue("@facettype3", Facet_Types[2]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype3", -1);
                    if (Facet_Types.Count > 3)
                        executeCommand.Parameters.AddWithValue("@facettype4", Facet_Types[3]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype4", -1);
                    if (Facet_Types.Count > 4)
                        executeCommand.Parameters.AddWithValue("@facettype5", Facet_Types[4]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype5", -1);
                    if (Facet_Types.Count > 5)
                        executeCommand.Parameters.AddWithValue("@facettype6", Facet_Types[5]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype6", -1);
                    if (Facet_Types.Count > 6)
                        executeCommand.Parameters.AddWithValue("@facettype7", Facet_Types[6]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype7", -1);
                    if (Facet_Types.Count > 7)
                        executeCommand.Parameters.AddWithValue("@facettype8", Facet_Types[7]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype8", -1);
                }
                else
                {
                    executeCommand.Parameters.AddWithValue("@facettype1", -1);
                    executeCommand.Parameters.AddWithValue("@facettype2", -1);
                    executeCommand.Parameters.AddWithValue("@facettype3", -1);
                    executeCommand.Parameters.AddWithValue("@facettype4", -1);
                    executeCommand.Parameters.AddWithValue("@facettype5", -1);
                    executeCommand.Parameters.AddWithValue("@facettype6", -1);
                    executeCommand.Parameters.AddWithValue("@facettype7", -1);
                    executeCommand.Parameters.AddWithValue("@facettype8", -1);
                }

                // Add parameters for total items and total titles
                SqlParameter totalItemsParameter = executeCommand.Parameters.AddWithValue("@total_items", 0);
                totalItemsParameter.Direction = ParameterDirection.InputOutput;

                SqlParameter totalTitlesParameter = executeCommand.Parameters.AddWithValue("@total_titles", 0);
                totalTitlesParameter.Direction = ParameterDirection.InputOutput;


                // Create the data reader
                connect.Open();
                using (SqlDataReader reader = executeCommand.ExecuteReader())
                {
                    // Create the return argument object
                    List<string> metadataLabels = new List<string>();
                    returnArgs = new Single_Paged_Results_Args { Paged_Results = DataReader_To_Simple_Result_List2(reader, metadataLabels) };

                    // Create the overall search statistics?
                    if (Return_Search_Statistics)
                    {
                        Search_Results_Statistics stats = new Search_Results_Statistics(reader, Facet_Types, metadataLabels);
                        returnArgs.Statistics = stats;
                        reader.Close();
                        stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
                        stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
                    }
                    else
                    {
                        reader.Close();
                    }
                }
                connect.Close();
            }

            // Return the built result arguments
            return returnArgs;
        }


        /// <summary> Get a browse of all items in a user's public folder </summary>
        /// <param name="UserFolderID"> Primary key for this user's folder which should be public in the database </param>
        /// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
        /// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
        /// <param name="Include_Facets"> Flag indicates if facets should be included in the final result set</param>
        /// <param name="Facet_Types"> Primary key for the metadata types to include as facets (up to eight)</param>
        /// <param name="Return_Search_Statistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> List of items matching search </returns>
        /// <remarks> This calls the 'mySobek_Get_User_Folder_Browse' stored procedure</remarks> 
        public static Single_Paged_Results_Args Get_Public_Folder_Browse(int UserFolderID, int ResultsPerPage, int ResultsPage, bool Include_Facets, List<short> Facet_Types, bool Return_Search_Statistics, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_Public_Folder_Browse", String.Empty);
            }

            Single_Paged_Results_Args returnArgs;

            // Create the connection
            using (SqlConnection connect = new SqlConnection(Connection_String + "Connection Timeout=45"))
            {
                // Create the command 
                SqlCommand executeCommand = new SqlCommand("mySobek_Get_Public_Folder_Browse", connect) { CommandTimeout = 45, CommandType = CommandType.StoredProcedure };

                executeCommand.Parameters.AddWithValue("@folderid", UserFolderID);
                executeCommand.Parameters.AddWithValue("@pagesize", ResultsPerPage);
                executeCommand.Parameters.AddWithValue("@pagenumber", ResultsPage);
                executeCommand.Parameters.AddWithValue("@include_facets", Include_Facets);
                if ((Include_Facets) && (Facet_Types != null))
                {
                    if (Facet_Types.Count > 0)
                        executeCommand.Parameters.AddWithValue("@facettype1", Facet_Types[0]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype1", -1);
                    if (Facet_Types.Count > 1)
                        executeCommand.Parameters.AddWithValue("@facettype2", Facet_Types[1]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype2", -1);
                    if (Facet_Types.Count > 2)
                        executeCommand.Parameters.AddWithValue("@facettype3", Facet_Types[2]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype3", -1);
                    if (Facet_Types.Count > 3)
                        executeCommand.Parameters.AddWithValue("@facettype4", Facet_Types[3]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype4", -1);
                    if (Facet_Types.Count > 4)
                        executeCommand.Parameters.AddWithValue("@facettype5", Facet_Types[4]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype5", -1);
                    if (Facet_Types.Count > 5)
                        executeCommand.Parameters.AddWithValue("@facettype6", Facet_Types[5]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype6", -1);
                    if (Facet_Types.Count > 6)
                        executeCommand.Parameters.AddWithValue("@facettype7", Facet_Types[6]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype7", -1);
                    if (Facet_Types.Count > 7)
                        executeCommand.Parameters.AddWithValue("@facettype8", Facet_Types[7]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype8", -1);
                }
                else
                {
                    executeCommand.Parameters.AddWithValue("@facettype1", -1);
                    executeCommand.Parameters.AddWithValue("@facettype2", -1);
                    executeCommand.Parameters.AddWithValue("@facettype3", -1);
                    executeCommand.Parameters.AddWithValue("@facettype4", -1);
                    executeCommand.Parameters.AddWithValue("@facettype5", -1);
                    executeCommand.Parameters.AddWithValue("@facettype6", -1);
                    executeCommand.Parameters.AddWithValue("@facettype7", -1);
                    executeCommand.Parameters.AddWithValue("@facettype8", -1);
                }

                // Add parameters for total items and total titles
                SqlParameter totalItemsParameter = executeCommand.Parameters.AddWithValue("@total_items", 0);
                totalItemsParameter.Direction = ParameterDirection.InputOutput;

                SqlParameter totalTitlesParameter = executeCommand.Parameters.AddWithValue("@total_titles", 0);
                totalTitlesParameter.Direction = ParameterDirection.InputOutput;


                // Create the data reader
                connect.Open();
                using (SqlDataReader reader = executeCommand.ExecuteReader())
                {

                    // Create the return argument object
                    List<string> metadataLabels = new List<string>();
                    returnArgs = new Single_Paged_Results_Args { Paged_Results = DataReader_To_Simple_Result_List2(reader, metadataLabels) };

                    // Create the overall search statistics?
                    if (Return_Search_Statistics)
                    {
                        Search_Results_Statistics stats = new Search_Results_Statistics(reader, Facet_Types, metadataLabels);
                        returnArgs.Statistics = stats;
                        reader.Close();
                        stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
                        stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
                    }
                    else
                    {
                        reader.Close();
                    }
                }
                connect.Close();
            }

            // Return the built result arguments
            return returnArgs;
        }

        #endregion

        #region Methods to retrieve the BROWSE information for the entire library

        /// <summary> Gets the collection of all (public) items in the library </summary>
        /// <param name="Only_New_Items"> Flag indicates to only pull items added in the last two weeks</param>
        /// <param name="Include_Private_Items"> Flag indicates whether to include private items in the result set </param>
        /// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
        /// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
        /// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
        /// <param name="Include_Facets"> Flag indicates if facets should be included in the final result set</param>
        /// <param name="Facet_Types"> Primary key for the metadata types to include as facets (up to eight)</param>
        /// <param name="Return_Search_Statistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Table with all of the item and item group information </returns>
        /// <remarks> This calls either the 'SobekCM_Get_All_Browse_Paged' stored procedure </remarks>
        public static Multiple_Paged_Results_Args Get_All_Browse_Paged(bool Only_New_Items, bool Include_Private_Items, int ResultsPerPage, int ResultsPage, int Sort, bool Include_Facets, List<short> Facet_Types, bool Return_Search_Statistics, Custom_Tracer Tracer)
        {
            if (Only_New_Items)
            {
                // Get the date string to use
                DateTime sinceDate = DateTime.Now.Subtract(new TimeSpan(14, 0, 0, 0));
                string dateString = sinceDate.Year.ToString().PadLeft(4, '0') + "-" + sinceDate.Month.ToString().PadLeft(2, '0') + "-" + sinceDate.Day.ToString().PadLeft(2, '0');
                return Get_All_Browse_Paged(dateString, Include_Private_Items, ResultsPerPage, ResultsPage, Sort, Include_Facets, Facet_Types, Return_Search_Statistics, Tracer);
            }

            // 1/1/2000 is a special date in the database, which means NO DATE
            return Get_All_Browse_Paged(String.Empty, Include_Private_Items, ResultsPerPage, ResultsPage, Sort, Include_Facets, Facet_Types, Return_Search_Statistics, Tracer);
        }

        /// <summary> Gets the collection of all (public) items in the library </summary>
        /// <param name="Since_Date"> Date from which to pull the data </param>
        /// <param name="Include_Private_Items"> Flag indicates whether to include private items in the result set </param>
        /// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
        /// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
        /// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
        /// <param name="Include_Facets"> Flag indicates if facets should be included in the final result set</param>
        /// <param name="Facet_Types"> Primary key for the metadata types to include as facets (up to eight)</param>
        /// <param name="Return_Search_Statistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Table with all of the item and item group information </returns>
        /// <remarks> This calls the 'SobekCM_Get_All_Browse_Paged' stored procedure </remarks>
        public static Multiple_Paged_Results_Args Get_All_Browse_Paged(string Since_Date, bool Include_Private_Items, int ResultsPerPage, int ResultsPage, int Sort, bool Include_Facets, List<short> Facet_Types, bool Return_Search_Statistics, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.SobekCM_Get_All_Browse_Paged", "Pulling browse from database");
            }


            Multiple_Paged_Results_Args returnArgs;

            try
            {


                // Create the connection
                using (SqlConnection connect = new SqlConnection(Connection_String + ";Connection Timeout=45"))
                {

                    // Create the command 
                    SqlCommand executeCommand = new SqlCommand("SobekCM_Get_All_Browse_Paged2", connect) { CommandTimeout = 45, CommandType = CommandType.StoredProcedure };

                    if (Since_Date.Length > 0)
                        executeCommand.Parameters.AddWithValue("@date", Since_Date);
                    else
                        executeCommand.Parameters.AddWithValue("@date", DBNull.Value);
                    executeCommand.Parameters.AddWithValue("@include_private", Include_Private_Items);
                    executeCommand.Parameters.AddWithValue("@pagesize", ResultsPerPage);
                    executeCommand.Parameters.AddWithValue("@pagenumber", ResultsPage);
                    executeCommand.Parameters.AddWithValue("@sort", Sort);
                    executeCommand.Parameters.AddWithValue("@minpagelookahead", MIN_PAGE_LOOKAHEAD);
                    executeCommand.Parameters.AddWithValue("@maxpagelookahead", MAX_PAGE_LOOKAHEAD);
                    executeCommand.Parameters.AddWithValue("@lookahead_factor", LOOKAHEAD_FACTOR);
                    executeCommand.Parameters.AddWithValue("@include_facets", Include_Facets);
                    if ((Include_Facets) && (Facet_Types != null))
                    {
                        if (Facet_Types.Count > 0)
                            executeCommand.Parameters.AddWithValue("@facettype1", Facet_Types[0]);
                        else
                            executeCommand.Parameters.AddWithValue("@facettype1", -1);
                        if (Facet_Types.Count > 1)
                            executeCommand.Parameters.AddWithValue("@facettype2", Facet_Types[1]);
                        else
                            executeCommand.Parameters.AddWithValue("@facettype2", -1);
                        if (Facet_Types.Count > 2)
                            executeCommand.Parameters.AddWithValue("@facettype3", Facet_Types[2]);
                        else
                            executeCommand.Parameters.AddWithValue("@facettype3", -1);
                        if (Facet_Types.Count > 3)
                            executeCommand.Parameters.AddWithValue("@facettype4", Facet_Types[3]);
                        else
                            executeCommand.Parameters.AddWithValue("@facettype4", -1);
                        if (Facet_Types.Count > 4)
                            executeCommand.Parameters.AddWithValue("@facettype5", Facet_Types[4]);
                        else
                            executeCommand.Parameters.AddWithValue("@facettype5", -1);
                        if (Facet_Types.Count > 5)
                            executeCommand.Parameters.AddWithValue("@facettype6", Facet_Types[5]);
                        else
                            executeCommand.Parameters.AddWithValue("@facettype6", -1);
                        if (Facet_Types.Count > 6)
                            executeCommand.Parameters.AddWithValue("@facettype7", Facet_Types[6]);
                        else
                            executeCommand.Parameters.AddWithValue("@facettype7", -1);
                        if (Facet_Types.Count > 7)
                            executeCommand.Parameters.AddWithValue("@facettype8", Facet_Types[7]);
                        else
                            executeCommand.Parameters.AddWithValue("@facettype8", -1);
                    }
                    else
                    {
                        executeCommand.Parameters.AddWithValue("@facettype1", -1);
                        executeCommand.Parameters.AddWithValue("@facettype2", -1);
                        executeCommand.Parameters.AddWithValue("@facettype3", -1);
                        executeCommand.Parameters.AddWithValue("@facettype4", -1);
                        executeCommand.Parameters.AddWithValue("@facettype5", -1);
                        executeCommand.Parameters.AddWithValue("@facettype6", -1);
                        executeCommand.Parameters.AddWithValue("@facettype7", -1);
                        executeCommand.Parameters.AddWithValue("@facettype8", -1);
                    }
                    executeCommand.Parameters.AddWithValue("@item_count_to_use_cached", 1000);

                    // Add parameters for total items and total titles
                    SqlParameter totalItemsParameter = executeCommand.Parameters.AddWithValue("@total_items", 0);
                    totalItemsParameter.Direction = ParameterDirection.InputOutput;

                    SqlParameter totalTitlesParameter = executeCommand.Parameters.AddWithValue("@total_titles", 0);
                    totalTitlesParameter.Direction = ParameterDirection.InputOutput;


                    // Create the data reader
                    connect.Open();
                    using (SqlDataReader reader = executeCommand.ExecuteReader())
                    {

                        // Create the return argument object
                        List<string> metadataLabels = new List<string>();
                        returnArgs = new Multiple_Paged_Results_Args { Paged_Results = DataReader_To_Result_List_With_LookAhead2(reader, ResultsPerPage, metadataLabels) };

                        // Create the overall search statistics?
                        if (Return_Search_Statistics)
                        {
                            Search_Results_Statistics stats = new Search_Results_Statistics(reader, Facet_Types, metadataLabels);
                            returnArgs.Statistics = stats;
                            reader.Close();
                            stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
                            stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
                        }
                        else
                        {
                            reader.Close();
                        }
                    }
                    connect.Close();
                }
            }
            catch (Exception ee)
            {
                lastException = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Get_All_Browse_Paged", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_All_Browse_Paged", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_All_Browse_Paged", ee.StackTrace, Custom_Trace_Type_Enum.Error);
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
        /// <param name="Only_New_Items"> Flag indicates to only pull items added in the last two weeks</param>
        /// <param name="Include_Private_Items"> Flag indicates whether to include private items in the result set </param>
        /// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
        /// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
        /// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
        /// <param name="Include_Facets"> Flag indicates if facets should be included in the final result set</param>
        /// <param name="Facet_Types"> Primary key for the metadata types to include as facets (up to eight)</param>
        /// <param name="Return_Search_Statistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Table with all of the item and item group information </returns>
        /// <remarks> This calls either the 'SobekCM_Get_Aggregation_Browse_Paged' stored procedure </remarks>
        public static Multiple_Paged_Results_Args Get_Item_Aggregation_Browse_Paged(string AggregationCode, bool Only_New_Items, bool Include_Private_Items, int ResultsPerPage, int ResultsPage, int Sort, bool Include_Facets, List<short> Facet_Types, bool Return_Search_Statistics, Custom_Tracer Tracer)
        {
            if (Only_New_Items)
            {
                // Get the date string to use
                DateTime sinceDate = DateTime.Now.Subtract(new TimeSpan(14, 0, 0, 0));
                string dateString = sinceDate.Year.ToString().PadLeft(4, '0') + "-" + sinceDate.Month.ToString().PadLeft(2, '0') + "-" + sinceDate.Day.ToString().PadLeft(2, '0');
                return Get_Item_Aggregation_Browse_Paged(AggregationCode, dateString, Include_Private_Items, ResultsPerPage, ResultsPage, Sort, Include_Facets, Facet_Types, Return_Search_Statistics, Tracer);
            }

            // 1/1/2000 is a special date in the database, which means NO DATE
            return Get_Item_Aggregation_Browse_Paged(AggregationCode, "2000-01-01", Include_Private_Items, ResultsPerPage, ResultsPage, Sort, Include_Facets, Facet_Types, Return_Search_Statistics, Tracer);
        }

        /// <summary> Gets the collection of all (public) items linked to an item aggregation </summary>
        /// <param name="AggregationCode"> Code for the item aggregation of interest </param>
        /// <param name="Since_Date"> Date from which to pull the data </param>
        /// <param name="Include_Private_Items"> Flag indicates whether to include private items in the result set </param>
        /// <param name="ResultsPerPage"> Number of results to return per "page" of results </param>
        /// <param name="ResultsPage"> Which page of results to return ( one-based, so the first page is page number of one )</param>
        /// <param name="Sort"> Current sort to use ( 0 = default by search or browse, 1 = title, 10 = date asc, 11 = date desc )</param>
        /// <param name="Include_Facets"> Flag indicates if facets should be included in the final result set</param>
        /// <param name="Facet_Types"> Primary key for the metadata types to include as facets (up to eight)</param>
        /// <param name="Return_Search_Statistics"> Flag indicates whether to create and return statistics about the overall search results, generally set to TRUE for the first page requested and subsequently set to FALSE </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Table with all of the item and item group information </returns>
        /// <remarks> This calls the 'SobekCM_Get_Aggregation_Browse_Paged' stored procedure </remarks>
        public static Multiple_Paged_Results_Args Get_Item_Aggregation_Browse_Paged(string AggregationCode, string Since_Date, bool Include_Private_Items, int ResultsPerPage, int ResultsPage, int Sort, bool Include_Facets, List<short> Facet_Types, bool Return_Search_Statistics, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Browse_Paged", "Pulling browse from database");
            }

            Multiple_Paged_Results_Args returnArgs;

            // Create the connection
            using (SqlConnection connect = new SqlConnection(Connection_String + "Connection Timeout=45"))
            {

                // Create the command 
                SqlCommand executeCommand = new SqlCommand("SobekCM_Get_Aggregation_Browse_Paged2", connect) { CommandTimeout = 45, CommandType = CommandType.StoredProcedure };

                executeCommand.Parameters.AddWithValue("@code", AggregationCode);
                executeCommand.Parameters.AddWithValue("@date", Since_Date);
                executeCommand.Parameters.AddWithValue("@include_private", Include_Private_Items);
                executeCommand.Parameters.AddWithValue("@pagesize", ResultsPerPage);
                executeCommand.Parameters.AddWithValue("@pagenumber", ResultsPage);
                executeCommand.Parameters.AddWithValue("@sort", Sort);

                if (ResultsPerPage > 100)
                {
                    executeCommand.Parameters.AddWithValue("@minpagelookahead", 1);
                    executeCommand.Parameters.AddWithValue("@maxpagelookahead", 1);
                    executeCommand.Parameters.AddWithValue("@lookahead_factor", LOOKAHEAD_FACTOR);
                }
                else
                {
                    executeCommand.Parameters.AddWithValue("@minpagelookahead", MIN_PAGE_LOOKAHEAD);
                    executeCommand.Parameters.AddWithValue("@maxpagelookahead", MAX_PAGE_LOOKAHEAD);
                    executeCommand.Parameters.AddWithValue("@lookahead_factor", LOOKAHEAD_FACTOR);
                }


                if ((Include_Facets) && (Facet_Types != null))
                {
                    executeCommand.Parameters.AddWithValue("@include_facets", true);
                    if (Facet_Types.Count > 0)
                        executeCommand.Parameters.AddWithValue("@facettype1", Facet_Types[0]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype1", -1);
                    if (Facet_Types.Count > 1)
                        executeCommand.Parameters.AddWithValue("@facettype2", Facet_Types[1]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype2", -1);
                    if (Facet_Types.Count > 2)
                        executeCommand.Parameters.AddWithValue("@facettype3", Facet_Types[2]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype3", -1);
                    if (Facet_Types.Count > 3)
                        executeCommand.Parameters.AddWithValue("@facettype4", Facet_Types[3]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype4", -1);
                    if (Facet_Types.Count > 4)
                        executeCommand.Parameters.AddWithValue("@facettype5", Facet_Types[4]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype5", -1);
                    if (Facet_Types.Count > 5)
                        executeCommand.Parameters.AddWithValue("@facettype6", Facet_Types[5]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype6", -1);
                    if (Facet_Types.Count > 6)
                        executeCommand.Parameters.AddWithValue("@facettype7", Facet_Types[6]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype7", -1);
                    if (Facet_Types.Count > 7)
                        executeCommand.Parameters.AddWithValue("@facettype8", Facet_Types[7]);
                    else
                        executeCommand.Parameters.AddWithValue("@facettype8", -1);
                }
                else
                {
                    executeCommand.Parameters.AddWithValue("@include_facets", false);
                    executeCommand.Parameters.AddWithValue("@facettype1", -1);
                    executeCommand.Parameters.AddWithValue("@facettype2", -1);
                    executeCommand.Parameters.AddWithValue("@facettype3", -1);
                    executeCommand.Parameters.AddWithValue("@facettype4", -1);
                    executeCommand.Parameters.AddWithValue("@facettype5", -1);
                    executeCommand.Parameters.AddWithValue("@facettype6", -1);
                    executeCommand.Parameters.AddWithValue("@facettype7", -1);
                    executeCommand.Parameters.AddWithValue("@facettype8", -1);
                }
                executeCommand.Parameters.AddWithValue("@item_count_to_use_cached", 1000);

                // Add parameters for total items and total titles
                SqlParameter totalItemsParameter = executeCommand.Parameters.AddWithValue("@total_items", 0);
                totalItemsParameter.Direction = ParameterDirection.InputOutput;

                SqlParameter totalTitlesParameter = executeCommand.Parameters.AddWithValue("@total_titles", 0);
                totalTitlesParameter.Direction = ParameterDirection.InputOutput;


                // Create the data reader
                connect.Open();
                using (SqlDataReader reader = executeCommand.ExecuteReader())
                {

                    // Create the return argument object
                    List<string> metadataLabels = new List<string>();
                    returnArgs = new Multiple_Paged_Results_Args { Paged_Results = DataReader_To_Result_List_With_LookAhead2(reader, ResultsPerPage, metadataLabels) };

                    // Create the overall search statistics?
                    if (Return_Search_Statistics)
                    {
                        Search_Results_Statistics stats = new Search_Results_Statistics(reader, Facet_Types, metadataLabels);
                        returnArgs.Statistics = stats;
                        reader.Close();
                        stats.Total_Items = Convert.ToInt32(totalItemsParameter.Value);
                        stats.Total_Titles = Convert.ToInt32(totalTitlesParameter.Value);
                    }
                    else
                    {
                        reader.Close();
                    }
                }
                connect.Close();
            }

            // Return the built result arguments
            return returnArgs;
        }

        /// <summary> Gets the list of all data for a particular metadata field in a particular aggregation </summary>
        /// <param name="Aggregation_Code"> Code for the item aggregation </param>
        /// <param name="Metadata_Code"> Metadata code for the field of interest </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> List with all the metadata fields in alphabetical order </returns>
        /// <remarks> This calls the 'SobekCM_Get_Metadata_Browse' stored procedure </remarks>
        public static List<string> Get_Item_Aggregation_Metadata_Browse(string Aggregation_Code, string Metadata_Code, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Metadata_Browse", "Pull the metadata browse");
            }

            // Build the parameter list
            SqlParameter[] paramList = new SqlParameter[3];
            paramList[0] = new SqlParameter("@aggregation_code", Aggregation_Code);
            paramList[1] = new SqlParameter("@metadata_name", Metadata_Code);
            paramList[2] = new SqlParameter("@item_count_to_use_cached", 100);

            // Define a temporary dataset
            DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Metadata_Browse", paramList);

            if (tempSet == null)
                return null;

            DataColumn column = tempSet.Tables[0].Columns[1];
            DataTable table = tempSet.Tables[0];
            return (from DataRow thisRow in table.Rows select thisRow[column].ToString()).ToList();
        }

        /// <summary> Gets the list of unique coordinate points and associated bibid and group title for a single 
        /// item aggregation </summary>
        /// <param name="Aggregation_Code"> Code for the item aggregation </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> DataTable with all the coordinate values </returns>
        /// <remarks> This calls the 'SobekCM_Coordinate_Points_By_Aggregation' stored procedure </remarks>
        public static DataTable Get_All_Coordinate_Points_By_Aggregation(string Aggregation_Code, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_All_Coordinate_Points_By_Aggregation", "Pull the coordinate list");
            }

            // Build the parameter list
            SqlParameter[] paramList = new SqlParameter[1];
            paramList[0] = new SqlParameter("@aggregation_code", Aggregation_Code);

            // Define a temporary dataset
            DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Coordinate_Points_By_Aggregation", paramList);
            return tempSet == null ? null : tempSet.Tables[0];
        }

        #endregion

        #region Methods to get the item aggregation

        /// <summary> Adds the title, item, and page counts to this item aggregation object </summary>
        /// <param name="Aggregation"> Mostly built item aggregation object </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This method calls the stored procedure 'SobekCM_Get_Item_Aggregation2'. </remarks>
        public static bool Get_Item_Aggregation_Counts(Item_Aggregation Aggregation, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Counts", "Add the title, item, and page count to the item aggregation object");
            }

            try
            {
                // Build the parameter list
                SqlParameter[] paramList = new SqlParameter[3];
                paramList[0] = new SqlParameter("@code", Aggregation.Code);
                paramList[1] = new SqlParameter("@include_counts", true);
                paramList[2] = new SqlParameter("@is_robot", false);

                // Define a temporary dataset
                DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Aggregation2", paramList);

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
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Counts", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Counts", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation_Counts", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return false;
            }

        }

        /// <summary> Gets the database information about a single item aggregation </summary>
        /// <param name="Code"> Code specifying the item aggregation to retrieve </param>
        /// <param name="Include_Counts"> Flag indicates whether to pull the title/item/page counts for this aggregation </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Is_Robot"> Flag indicates if this is a request from an indexing robot, which leaves out a good bit of the work </param>
        /// <returns> Arguments which include the <see cref="Item_Aggregation"/> object and a DataTable of the search field information</returns>
        /// <remarks> This method calls the stored procedure 'SobekCM_Get_Item_Aggregation2'. </remarks>
        public static Item_Aggregation Get_Item_Aggregation(string Code, bool Include_Counts, bool Is_Robot, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation", "Pulling item aggregation data from database");
            }

            try
            {
                // Build the parameter list
                SqlParameter[] paramList = new SqlParameter[3];
                paramList[0] = new SqlParameter("@code", Code);
                paramList[1] = new SqlParameter("@include_counts", Include_Counts);
                paramList[2] = new SqlParameter("@is_robot", Is_Robot);

                // Define a temporary dataset
                DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Item_Aggregation2", paramList);

                // If there was no data for this collection and entry point, return null (an ERROR occurred)
                if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
                {
                    return null;
                }

                // Build the collection group object
                Item_Aggregation aggrInfo = create_basic_aggregation_from_datatable(tempSet.Tables[0]);

                // Add the child objects from that table
                add_children(aggrInfo, tempSet.Tables[1]);

                // Add the advanced search values
                if (!Is_Robot)
                {
                    add_advanced_terms(aggrInfo, tempSet.Tables[2]);
                }

                // Add the counts for this item aggregation
                if (Include_Counts)
                {
                    add_counts(aggrInfo, tempSet.Tables[tempSet.Tables.Count - 2]);
                }

                // If this is not a robot, add the parents
                if (!Is_Robot)
                {
                    add_parents(aggrInfo, tempSet.Tables[tempSet.Tables.Count - 1]);
                }

                // Return the built argument set
                return aggrInfo;
            }
            catch (Exception ee)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Item_Aggregation", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                throw ee;
            }
        }

        /// <summary> Gets the database information about the main aggregation, representing the entire web page </summary>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Arguments which include the <see cref="Item_Aggregation"/> object and a DataTable of the search field information</returns>
        /// <remarks> This method calls the stored procedure 'SobekCM_Get_All_Groups'. </remarks>
        public static Item_Aggregation Get_Main_Aggregation(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_Main_Aggregation", "Pulling item aggregation data from database");
            }

            try
            {
                // Build the parameter list
                SqlParameter[] paramList = new SqlParameter[1];
                paramList[0] = new SqlParameter("@metadata_count_to_use_cache", ALL_AGGREGATIONS_METADATA_COUNT_TO_USE_CACHED);

                // Define a temporary dataset
                DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Get_All_Groups", paramList);

                // If there was no data for this collection and entry point, return null (an ERROR occurred)
                if ((tempSet.Tables.Count == 0) || (tempSet.Tables[0] == null) || (tempSet.Tables[0].Rows.Count == 0))
                {
                    return null;
                }

                // Build the collection group object
                Item_Aggregation aggrInfo = create_basic_aggregation_from_datatable(tempSet.Tables[0]);

                // Add the advanced search values
                add_advanced_terms(aggrInfo, tempSet.Tables[1]);

                // Return the built argument set
                return aggrInfo;
            }
            catch (Exception ee)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Get_Main_Aggregation", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Main_Aggregation", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Main_Aggregation", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                throw;
            }
        }

        /// <summary> Adds the entire collection hierarchy under the ALL aggregation object </summary>
        /// <param name="AllInfoObject"> All aggregationPermissions object within which to populate the hierarchy </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This is postponed until it is needed for the TREE VIEW on the home page, to allow the system to start
        /// faster, even with a great number of item aggregationPermissions in the hierarchy </remarks>
        public static bool Add_Children_To_Main_Agg(Item_Aggregation AllInfoObject, Custom_Tracer Tracer)
        {
            DataTable childInfo = Get_Aggregation_Hierarchies(Tracer);
            if (childInfo == null)
                return false;

            add_children(AllInfoObject, childInfo);
            return true;
        }

        /// <summary> Creates the item aggregation object from the datatable extracted from the database </summary>
        /// <param name="BasicInfo">Datatable from database calls ( either SobekCM_Get_Item_Aggregation or SobekCM_Get_All_Groups )</param>
        /// <returns>Minimally built aggregation object</returns>
        /// <remarks>The child and parent information is not yet added to the returned object </remarks>
        private static Item_Aggregation create_basic_aggregation_from_datatable(DataTable BasicInfo)
        {
            // Pull out this row
            DataRow thisRow = BasicInfo.Rows[0];

            string displayOptions = thisRow[15].ToString();
            DateTime lastAdded = new DateTime(2000, 1, 1);
            if (thisRow[16] != DBNull.Value)
                lastAdded = Convert.ToDateTime(thisRow[16]);

            // Build the collection group object
            Item_Aggregation aggrInfo = new Item_Aggregation( Engine_ApplicationCache_Gateway.Settings.Default_UI_Language, Engine_ApplicationCache_Gateway.Settings.Base_Design_Location,
                thisRow[1].ToString().ToLower(), thisRow[4].ToString(), Convert.ToInt32(thisRow[0]), displayOptions, lastAdded)
            {
                Name = thisRow[2].ToString(),
                ShortName = thisRow[3].ToString(),
                Is_Active = Convert.ToBoolean(thisRow[5]),
                Hidden = Convert.ToBoolean(thisRow[6]),
                Has_New_Items = Convert.ToBoolean(thisRow[7]),
                Contact_Email = thisRow[8].ToString(),
                Default_Skin = thisRow[9].ToString(),
                Description = thisRow[10].ToString(),
                Map_Display = Convert.ToUInt16(thisRow[11]),
                Map_Search = Convert.ToUInt16(thisRow[12]),
                OAI_Flag = Convert.ToBoolean(thisRow[13]),
                OAI_Metadata = thisRow[14].ToString(),
                Items_Can_Be_Described = Convert.ToInt16(thisRow[18]),
                External_Link = thisRow[19].ToString()
            };

            if (BasicInfo.Columns.Contains("ThematicHeadingID"))
                aggrInfo.Thematic_Heading_ID = Convert.ToInt32(thisRow["ThematicHeadingID"]);

            // return the built object
            return aggrInfo;
        }

        /// <summary> Adds the child information to the item aggregation object from the datatable extracted from the database </summary>
        /// <param name="AggrInfo">Partially built item aggregation object</param>
        /// <param name="ChildInfo">Datatable from database calls with child item aggregation information ( either SobekCM_Get_Item_Aggregation or SobekCM_Get_All_Groups )</param>
        private static void add_children(Item_Aggregation AggrInfo, DataTable ChildInfo)
        {
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
        private static void add_parents(Item_Aggregation AggrInfo, DataTable ParentInfo)
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
        private static void add_advanced_terms(Item_Aggregation AggrInfo, DataTable SearchTermsTable)
        {
            // Add ANYWHERE first
            AggrInfo.Advanced_Search_Fields.Add(-1);

            // Add values either default values or from the table
            if ((SearchTermsTable == null) || (SearchTermsTable.Rows.Count == 0))
            {
                AggrInfo.Advanced_Search_Fields.Add(4);
                AggrInfo.Advanced_Search_Fields.Add(3);
                AggrInfo.Advanced_Search_Fields.Add(6);
                AggrInfo.Advanced_Search_Fields.Add(5);
                AggrInfo.Advanced_Search_Fields.Add(7);
                AggrInfo.Advanced_Search_Fields.Add(1);
                AggrInfo.Advanced_Search_Fields.Add(2);

                AggrInfo.Browseable_Fields.Add(4);
                AggrInfo.Browseable_Fields.Add(3);
                AggrInfo.Browseable_Fields.Add(6);
                AggrInfo.Browseable_Fields.Add(5);
                AggrInfo.Browseable_Fields.Add(7);
                AggrInfo.Browseable_Fields.Add(1);
                AggrInfo.Browseable_Fields.Add(2);

            }
            else
            {
                short lastTypeId = -1;
                foreach (DataRow thisRow in SearchTermsTable.Rows)
                {
                    short thisTypeId = Convert.ToInt16(thisRow[0]);
                    if ((thisTypeId != lastTypeId) && (!AggrInfo.Advanced_Search_Fields.Contains(thisTypeId)))
                    {
                        AggrInfo.Advanced_Search_Fields.Add(thisTypeId);
                        lastTypeId = thisTypeId;
                    }
                    bool canBrowse = Convert.ToBoolean(thisRow[1]);
                    if ((canBrowse) && (!AggrInfo.Browseable_Fields.Contains(thisTypeId)))
                    {
                        AggrInfo.Browseable_Fields.Add(thisTypeId);
                    }
                }
            }
        }

        /// <summary> Adds the page count, item count, and title count to the item aggregation object from the datatable extracted from the database </summary>
        /// <param name="AggrInfo">Partially built item aggregation object</param>
        /// <param name="CountInfo">Datatable from database calls with page count, item count, and title count ( from either SobekCM_Get_Item_Aggregation or SobekCM_Get_All_Groups )</param>
        private static void add_counts(Item_Aggregation AggrInfo, DataTable CountInfo)
        {
            if (CountInfo.Rows.Count > 0)
            {
                AggrInfo.Page_Count = Convert.ToInt32(CountInfo.Rows[0]["Page_Count"]);
                AggrInfo.Item_Count = Convert.ToInt32(CountInfo.Rows[0]["Item_Count"]);
                AggrInfo.Title_Count = Convert.ToInt32(CountInfo.Rows[0]["Title_Count"]);
            }
        }

        /// <summary>Method used to get the hierarchical relationship between all aggregationPermissions, to be displayed in the 'aggregationPermissions' tab in the internal screen</summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> DataTable with relationships between all aggregationPermissions</returns>
        /// <remarks> This calls the 'SobekCM_Get_Collection_Hierarchies' stored procedure <br /><br />
        /// This is used by the <see cref="Internal_HtmlSubwriter"/> class</remarks>
        public static DataTable Get_Aggregation_Hierarchies(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Get_Aggregation_Hierarchies", "Pulling from database");
            }

            try
            {
                // Define a temporary dataset
                DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Collection_Hierarchies");

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
                    Tracer.Add_Trace("SobekCM_Database.Get_Aggregation_Hierarchies", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Aggregation_Hierarchies", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Get_Aggregation_Hierarchies", ee.StackTrace, Custom_Trace_Type_Enum.Error);
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
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Save_Item_Aggregation' stored procedure in the SobekCM database</remarks> 
        public static bool Save_Item_Aggregation(string Code, string Name, string ShortName, string Description, int ThematicHeadingID, string Type, bool IsActive, bool IsHidden, string ExternalLink, int ParentID, string Username, Custom_Tracer Tracer)
        {
            return Save_Item_Aggregation(-1, Code, Name, ShortName, Description, ThematicHeadingID, Type, IsActive, IsHidden, String.Empty, 0, 0, 0, 0, false, String.Empty, String.Empty, String.Empty, ExternalLink, ParentID, Username, Tracer);
        }

        /// <summary> Save a new item aggregation or edit an existing item aggregation in the database </summary>
        /// <param name="AggregationID"> AggregationID if this is editing an existing one, otherwise -1 </param>
        /// <param name="Code"> Code for this item aggregation </param>
        /// <param name="Name"> Name for this item aggregation </param>
        /// <param name="ShortName"> Short version of this item aggregation </param>
        /// <param name="Description"> Description of this item aggregation </param>
        /// <param name="ThematicHeadingID"> Thematic heading id for this item aggregation (or -1)</param>
        /// <param name="Type"> Type of item aggregation (i.e., Collection Group, Institution, Exhibit, etc..)</param>
        /// <param name="IsActive"> Flag indicates if this item aggregation is active</param>
        /// <param name="IsHidden"> Flag indicates if this item is hidden</param>
        /// <param name="DisplayOptions"> Display options for this item aggregation </param>
        /// <param name="Map_Search"> Map Search value indicates if there is a map search, and the type of search </param>
        /// /// <param name="Map_Search_Beta"> Map Search value indicates if there is a map search, and the type of search </param>
        /// <param name="Map_Display"> Map Display value indicates if there is a map display option when looking at search results or browses </param>
        /// <param name="Map_Display_Beta"> Map Display value indicates if there is a map display option when looking at search results or browses </param>
        /// <param name="OAI_Flag"> Flag indicates if this item aggregation should be available via OAI-PMH </param>
        /// <param name="OAI_Metadata"> Additional metadata about this collection, to be included in the set information in OAI-PMH</param>
        /// <param name="ContactEmail"> Contact email for this item aggregation (can leave blank to use default)</param>
        /// <param name="DefaultInterface"> Default interface for this item aggregation (particularly useful for institutional aggregationPermissions)</param>
        /// <param name="ExternalLink">External link for this item aggregation (used primarily for institutional item aggregationPermissions to provide a link back to the institution's actual home page)</param>
        /// <param name="ParentID"> ID for the item aggregation parent</param>
        /// <param name="Username"> Username saving this new item aggregation, for the item aggregation milestones </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Save_Item_Aggregation' stored procedure in the SobekCM database</remarks> 
        public static bool Save_Item_Aggregation(int AggregationID, string Code, string Name, string ShortName, string Description, int ThematicHeadingID, string Type, bool IsActive, bool IsHidden, string DisplayOptions, int Map_Search, int Map_Search_Beta, int Map_Display, int Map_Display_Beta, bool OAI_Flag, string OAI_Metadata, string ContactEmail, string DefaultInterface, string ExternalLink, int ParentID, string Username, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Database.Save_Item_Aggregation", String.Empty);
            }

            try
            {
                // Build the parameter list
                SqlParameter[] paramList = new SqlParameter[20];
                paramList[0] = new SqlParameter("@aggregationid", AggregationID);
                paramList[1] = new SqlParameter("@code", Code);
                paramList[2] = new SqlParameter("@name", Name);
                paramList[3] = new SqlParameter("@shortname", ShortName);
                paramList[4] = new SqlParameter("@description", Description);
                paramList[5] = new SqlParameter("@thematicHeadingId", ThematicHeadingID);
                paramList[6] = new SqlParameter("@type", Type);
                paramList[7] = new SqlParameter("@isActive", IsActive);
                paramList[8] = new SqlParameter("@hidden", IsHidden);
                paramList[9] = new SqlParameter("@display_options", DisplayOptions);
                paramList[10] = new SqlParameter("@map_search", Map_Search);
                paramList[11] = new SqlParameter("@map_display", Map_Display);
                paramList[12] = new SqlParameter("@oai_flag", OAI_Flag);
                paramList[13] = new SqlParameter("@oai_metadata", OAI_Metadata);
                paramList[14] = new SqlParameter("@contactemail", ContactEmail);
                paramList[15] = new SqlParameter("@defaultinterface", DefaultInterface);
                paramList[16] = new SqlParameter("@externallink", ExternalLink);
                paramList[17] = new SqlParameter("@parentid", ParentID);
                paramList[18] = new SqlParameter("@username", Username);
                paramList[19] = new SqlParameter("@newaggregationid", 0) { Direction = ParameterDirection.InputOutput };

                //BETA
                //paramList[20] = new SqlParameter("@map_search_beta", Map_Search_Beta);
                //paramList[21] = new SqlParameter("@map_display_beta", Map_Display_Beta);

                // Execute this query stored procedure
                SqlHelper.ExecuteNonQuery(Connection_String, CommandType.StoredProcedure, "SobekCM_Save_Item_Aggregation", paramList);

                // Succesful, so return true
                return true;
            }
            catch (Exception ee)
            {
                lastException = ee;
                if (Tracer != null)
                {
                    Tracer.Add_Trace("SobekCM_Database.Save_Item_Aggregation", "Exception caught during database work", Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Save_Item_Aggregation", ee.Message, Custom_Trace_Type_Enum.Error);
                    Tracer.Add_Trace("SobekCM_Database.Save_Item_Aggregation", ee.StackTrace, Custom_Trace_Type_Enum.Error);
                }
                return false;
            }
        }

        #endregion


        /// <summary> Gets all the data necessary for the Builder, including file destination information,
        /// general settings, server information, and the list of each BibID and File_Root </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> DataSet with all the data necessary for the Builder, including file destination information,
        /// general settings, server information</returns>
        /// <remarks> This calls the 'SobekCM_Get_Settings' stored procedure </remarks> 
        public static DataSet Get_Settings_Complete(Custom_Tracer Tracer)
        {
            try
            {
                DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Get_Settings");
                return tempSet;
            }
            catch (Exception ee)
            {
                lastException = ee;
                return null;
            }
        }

        /// <summary> Gets the simple list of items for a single item aggregation, or the list of all items in the library </summary>
        /// <param name="Aggregation_Code"> Code for the item aggregation of interest, or an empty string</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Dataset with the simple list of items, including BibID, VID, Title, CreateDate, and Resource Link </returns>
        /// <remarks> This calls the 'SobekCM_Simple_Item_List' stored procedure </remarks> 
        public static DataSet Simple_Item_List(string Aggregation_Code, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                if (Aggregation_Code.Length == 0)
                    Tracer.Add_Trace("SobekCM_Database.Simple_Item_List", "Pulling simple item list for all items");
                else
                    Tracer.Add_Trace("SobekCM_Database.Simple_Item_List", "Pulling simple item list for '" + Aggregation_Code + "'");
            }

            // Define a temporary dataset
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@collection_code", Aggregation_Code);
            DataSet tempSet = SqlHelper.ExecuteDataset(Connection_String, CommandType.StoredProcedure, "SobekCM_Simple_Item_List", parameters);
            return tempSet;
        }
    }
}
