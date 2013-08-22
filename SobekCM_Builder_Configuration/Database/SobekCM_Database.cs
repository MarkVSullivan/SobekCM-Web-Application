using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SobekCM.Configuration.Database
{
    class SobekCM_Database
    {
        private static string database_string = String.Empty;
        private static Exception last_exception;

        public static bool Test_Connection(string ConnectionString )
        {

            try
            {
                SqlConnection newConnection = new SqlConnection(ConnectionString);
                newConnection.Open();

                newConnection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string Database_String
        {
            get { return database_string; }
            set { database_string = value; }
        }

        public static Exception Last_Exception
        {
            get { return last_exception; }
        }


        /// <summary> Gets the values from the builder settings table in the database </summary>
        /// <returns> Dictionary of all the keys and values in the builder settings table </returns>
        /// <remarks> This calls the 'SobekCM_Get_Settings' stored procedure </remarks> 
        public static Dictionary<string, string> Get_Settings()
        {
            Dictionary<string, string> returnValue = new Dictionary<string, string>();

            try
            {
                // Create the connection
                using (SqlConnection connect = new SqlConnection(database_string))
                {
                    // Create the command 
                    SqlCommand executeCommand = new SqlCommand("SobekCM_Get_Settings", connect);
                    executeCommand.CommandTimeout = 30;
                    executeCommand.CommandType = CommandType.StoredProcedure;
                    executeCommand.Parameters.AddWithValue("@include_items", false);

                    // Create the data reader
                    connect.Open();
                    using (SqlDataReader reader = executeCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string key = reader.GetString(0);
                            string value = reader.GetString(1);
                            returnValue[key] = value;
                        }

                        reader.Close();
                    }

                    connect.Close();
                }
            }
            catch (Exception ee)
            {
                last_exception = ee;
                return null;
                
            }

            return returnValue;
        }

        /// <summary> Sets a value in the settings table </summary>
        /// <param name="Setting_Key"> Key for the setting to update or insert </param>
        /// <param name="Setting_Value"> Value for the setting to update or insert </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Set_Setting_Value' stored procedure </remarks> 
        public static bool Set_Setting_Value(string Setting_Key, string Setting_Value)
        {
            try
            {
                // Create the connection
                using (SqlConnection connect = new SqlConnection(database_string))
                {
                    // Create the command 
                    SqlCommand executeCommand = new SqlCommand("SobekCM_Set_Setting_Value", connect);
                    executeCommand.CommandTimeout = 30;
                    executeCommand.CommandType = CommandType.StoredProcedure;
                    executeCommand.Parameters.AddWithValue("@Setting_Key", Setting_Key);
                    executeCommand.Parameters.AddWithValue("@Setting_Value", Setting_Value);

                    // Create the data reader
                    connect.Open();
                    executeCommand.ExecuteNonQuery();
                    connect.Close();
                }

                return true;
            }
            catch (Exception ee)
            {
                last_exception = ee;
                return false;

            }
        }

        /// <summary> Gets the incoming folder information for the SobekCM Builder/Bulk Loader  </summary>
        /// <returns> DataSet with all the incoming folder information for the SobekCM Builder/Bulk Loader, or NULL if an error occurs</returns>
        /// <remarks> This calls the 'SobekCM_Get_Builder_Settings' stored procedure </remarks> 
        public static DataTable Get_Builder_Incoming_Folders()
        {
            try
            {
                // Create the return value
                DataSet returnValue = new DataSet("Builder_Settings");

                // Create the connection
                using (SqlConnection connect = new SqlConnection(database_string))
                {
                    // Create the command 
                    SqlCommand executeCommand = new SqlCommand("SobekCM_Get_Builder_Settings", connect);
                    executeCommand.CommandTimeout = 30;
                    executeCommand.CommandType = CommandType.StoredProcedure;
                    executeCommand.Parameters.AddWithValue("@include_items", false);

                    // Create the adapter to fill the data set
                    SqlDataAdapter executeAdapter = new SqlDataAdapter(executeCommand);
                    
                    // Populate the data set
                    connect.Open();
                    executeAdapter.Fill( returnValue );
                }

                return returnValue.Tables[1];
            }
            catch (Exception ee)
            {
                last_exception = ee;
                return null;
            }
        }

        
        /// <summary> Deletes an existing builder incoming folder from the table </summary>
        /// <param name="FolderID"> Primary key for the builder incoming folder to delete </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Builder_Incoming_Folder_Delete' stored procedure </remarks> 
        public static bool Delete_Builder_Incoming_Folder( int FolderID )
        {
            try
            {
                // Create the connection
                using (SqlConnection connect = new SqlConnection(database_string))
                {
                    // Create the command 
                    SqlCommand executeCommand = new SqlCommand("SobekCM_Builder_Incoming_Folder_Delete", connect);
                    executeCommand.CommandTimeout = 30;
                    executeCommand.CommandType = CommandType.StoredProcedure;
                    executeCommand.Parameters.AddWithValue("@IncomingFolderId", FolderID);

                    // Create the data reader
                    connect.Open();
                    executeCommand.ExecuteNonQuery();
                    connect.Close();
                }

                return true;
            }
            catch (Exception ee)
            {
                last_exception = ee;
                return false;

            }
        }

        /// <summary> Edits an existing builder incoming folder or adds a new folder </summary>
        /// <param name="FolderID"> Primary key for the builder incoming folder to delete </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This calls the 'SobekCM_Builder_Incoming_Folder_Edit' stored procedure </remarks> 
        public static bool Edit_Builder_Incoming_Folder(int FolderID, string Folder_Name, string Network_Folder, string Error_Folder, string Processing_Folder,
            bool Perform_Checksum, bool Archive_TIFF, bool Archive_All_Files, bool Allow_Deletes, bool Allow_Folders_No_Metadata, bool Contains_Institutional_Folders )
        {
            try
            {
                // Create the connection
                using (SqlConnection connect = new SqlConnection(database_string))
                {
                    // Create the command 
                    SqlCommand executeCommand = new SqlCommand("SobekCM_Builder_Incoming_Folder_Edit", connect);
                    executeCommand.CommandTimeout = 30;
                    executeCommand.CommandType = CommandType.StoredProcedure;
                    executeCommand.Parameters.AddWithValue("@IncomingFolderId", FolderID);
                    executeCommand.Parameters.AddWithValue("@NetworkFolder", Network_Folder);
                    executeCommand.Parameters.AddWithValue("@ErrorFolder", Error_Folder);
                    executeCommand.Parameters.AddWithValue("@ProcessingFolder", Processing_Folder);
                    executeCommand.Parameters.AddWithValue("@Perform_Checksum_Validation", Perform_Checksum);
                    executeCommand.Parameters.AddWithValue("@Archive_TIFF", Archive_TIFF);
                    executeCommand.Parameters.AddWithValue("@Archive_All_Files", Archive_All_Files);
                    executeCommand.Parameters.AddWithValue("@Allow_Deletes", Allow_Deletes);
                    executeCommand.Parameters.AddWithValue("@Allow_Folders_No_Metadata", Allow_Folders_No_Metadata);
                    executeCommand.Parameters.AddWithValue("@Contains_Institutional_Folders", Contains_Institutional_Folders);
                    executeCommand.Parameters.AddWithValue("@FolderName", Folder_Name);
                    executeCommand.Parameters.AddWithValue("@NewID", -1).Direction = ParameterDirection.Output;

                    // Create the data reader
                    connect.Open();
                    executeCommand.ExecuteNonQuery();
                    connect.Close();
                }

                return true;
            }
            catch (Exception ee)
            {
                last_exception = ee;
                return false;

            }
        }
    }
}
