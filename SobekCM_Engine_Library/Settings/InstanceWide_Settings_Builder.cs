﻿#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using SobekCM.Core.Configuration;
using SobekCM.Core.Database;
using SobekCM.Core.Search;
using SobekCM.Core.Settings;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Engine_Library.Database;
using SobekCM.Resource_Object.OAI.Writer;

#endregion

namespace SobekCM.Engine_Library.Settings
{
    public static  class InstanceWide_Settings_Builder
    {
        #region Constant values 

        /// <summary> Name for the backup files folder within each digital resource </summary>
        private const string BACKUP_FILES_FOLDER_NAME = "sobek_files";

        /// <summary> Current version number associated with this SobekCM digital repository web application </summary>
        private const string CURRENT_WEB_VERSION = "4.7.1";

        /// <summary> Current version number associated with this SobekCM builder application </summary>
        private const string CURRENT_BUILDER_VERSION = "4.7.1";

        /// <summary> Number of ticks that a complete package must age before being processed </summary>
        /// <value> This is currently set to 15 minutes (in ticks) </value>
        private const long COMPLETE_PACKAGE_REQUIRED_AGING = 15L * 60L * 1000000L; // 15 minutes (in ticks)

        /// <summary> Number of ticks that a metadata only package must age before being processed </summary>
        /// <value> This is currently set to 1 minute (in ticks) </value>
        private const long METS_ONLY_PACKAGE_REQUIRED_AGING = 60L * 10000000L; // 1 Minute (in ticks)

        /// <summary> Flag indicates whether checksums should be verified </summary>
        public const bool VERIFY_CHECKSUM = true;

        #endregion

        /// <summary> Refreshes the values from the database settings </summary>
        /// <returns> A fully builder instance-wide setting object </returns>
        public static InstanceWide_Settings Build_Settings()
        {
            InstanceWide_Settings returnValue = new InstanceWide_Settings();

            // Should we read the configuration file?

            returnValue.Base_Directory = AppDomain.CurrentDomain.BaseDirectory;
            Read_Configuration_File(returnValue, returnValue.Base_Directory + "\\config\\sobekcm.config");

            Engine_Database.Connection_String = returnValue.Database_Connections[0].Connection_String;
            Resource_Object.Database.SobekCM_Database.Connection_String = returnValue.Database_Connections[0].Connection_String;

            DataSet sobekCMSettings = Engine_Database.Get_Settings_Complete(null);
            Refresh(returnValue, sobekCMSettings);

            // Try to read the SHIBBOLETH configuration file
            if (File.Exists(returnValue.Base_Directory + "\\config\\user\\sobekcm_shibboleth.config"))
            {
                returnValue.Shibboleth = Shibboleth_Configuration_Reader.Read_Config(returnValue.Base_Directory + "\\config\\user\\sobekcm_shibboleth.config");
            }
            else if (File.Exists(returnValue.Base_Directory + "\\config\\default\\sobekcm_shibboleth.config"))
            {
                returnValue.Shibboleth = Shibboleth_Configuration_Reader.Read_Config(returnValue.Base_Directory + "\\config\\default\\sobekcm_shibboleth.config");
            }

            // Try to read the CONTACT FORM configuration file
            if (File.Exists(returnValue.Base_Directory + "\\config\\user\\sobekcm_contactform.config"))
            {
                returnValue.ContactForm = ContactForm_Configuration_Reader.Read_Config(returnValue.Base_Directory + "\\config\\user\\sobekcm_contactform.config");
            }
            else if (File.Exists(returnValue.Base_Directory + "\\config\\default\\sobekcm_contactform.config"))
            {
                returnValue.ContactForm = ContactForm_Configuration_Reader.Read_Config(returnValue.Base_Directory + "\\config\\default\\sobekcm_contactform.config");
            }

            // Try to read the OAI-PMH configuration file
            if (File.Exists(returnValue.Base_Directory + "\\config\\user\\sobekcm_oaipmh.config"))
            {
                returnValue.OAI_PMH = OAI_PMH_Configuration_Reader.Read_Config(returnValue.Base_Directory + "\\config\\user\\sobekcm_oaipmh.config");
            }
            else if (File.Exists(returnValue.Base_Directory + "\\config\\default\\sobekcm_oaipmh.config"))
            {
                returnValue.OAI_PMH = OAI_PMH_Configuration_Reader.Read_Config(returnValue.Base_Directory + "\\config\\default\\sobekcm_oaipmh.config");
            }

            // Load the OAI-PMH configuration file info into the OAI writer class ( in the resource object library )
            if (returnValue.OAI_PMH == null)
                OAI_PMH_Metadata_Writers.Set_Default_Values();
            else
            {
                OAI_PMH_Metadata_Writers.Clear();
                foreach (OAI_PMH_Metadata_Format thisWriter in returnValue.OAI_PMH.Metadata_Prefixes)
                {
                    if (thisWriter.Enabled)
                    {
                        OAI_PMH_Metadata_Writers.Add_Writer(thisWriter.Prefix, thisWriter.Assembly, thisWriter.Namespace, thisWriter.Class);
                    }
                }
            }

            return returnValue;
        }

        /// <summary> Refreshes the values from the database settings </summary>
        /// <returns> A fully builder instance-wide setting object </returns>
        public static InstanceWide_Settings Build_Settings( Database_Instance_Configuration DbInstance )
        {
            InstanceWide_Settings returnValue = new InstanceWide_Settings();

            // Don't read the configuration file now.. we already have the db data
            Engine_Database.Connection_String = DbInstance.Connection_String;

            DataSet sobekCMSettings = Engine_Database.Get_Settings_Complete(null);
            if (sobekCMSettings == null)
                return null;

            Refresh(returnValue, sobekCMSettings);

            // Try to read the SHIBBOLETH configuration file
            if (File.Exists(returnValue.Base_Directory + "\\config\\user\\sobekcm_shibboleth.config"))
            {
                returnValue.Shibboleth = Shibboleth_Configuration_Reader.Read_Config(returnValue.Base_Directory + "\\config\\user\\sobekcm_shibboleth.config");
            }
            else if (File.Exists(returnValue.Base_Directory + "\\config\\default\\sobekcm_shibboleth.config"))
            {
                returnValue.Shibboleth = Shibboleth_Configuration_Reader.Read_Config(returnValue.Base_Directory + "\\config\\default\\sobekcm_shibboleth.config");
            }

            // Try to read the CONTACT FORM configuration file
            if (File.Exists(returnValue.Base_Directory + "\\config\\user\\sobekcm_contactform.config"))
            {
                returnValue.ContactForm = ContactForm_Configuration_Reader.Read_Config(returnValue.Base_Directory + "\\config\\user\\sobekcm_contactform.config");
            }
            else if (File.Exists(returnValue.Base_Directory + "\\config\\default\\sobekcm_contactform.config"))
            {
                returnValue.ContactForm = ContactForm_Configuration_Reader.Read_Config(returnValue.Base_Directory + "\\config\\default\\sobekcm_contactform.config");
            }

            // Try to read the OAI-PMH configuration file
            if (File.Exists(returnValue.Base_Directory + "\\config\\user\\sobekcm_oaipmh.config"))
            {
                returnValue.OAI_PMH = OAI_PMH_Configuration_Reader.Read_Config(returnValue.Base_Directory + "\\config\\user\\sobekcm_oaipmh.config");
            }
            else if (File.Exists(returnValue.Base_Directory + "\\config\\default\\sobekcm_oaipmh.config"))
            {
                returnValue.OAI_PMH = OAI_PMH_Configuration_Reader.Read_Config(returnValue.Base_Directory + "\\config\\default\\sobekcm_oaipmh.config");
            }

            // Load the OAI-PMH configuration file info into the OAI writer class ( in the resource object library )
            if (returnValue.OAI_PMH == null)
                OAI_PMH_Metadata_Writers.Set_Default_Values();
            else
            {
                OAI_PMH_Metadata_Writers.Clear();
                foreach (OAI_PMH_Metadata_Format thisWriter in returnValue.OAI_PMH.Metadata_Prefixes)
                {
                    if (thisWriter.Enabled)
                    {
                        OAI_PMH_Metadata_Writers.Add_Writer(thisWriter.Prefix, thisWriter.Assembly, thisWriter.Namespace, thisWriter.Class);
                    }
                }
            }

            return returnValue;
        }

        public static bool Refresh( InstanceWide_Settings SettingsObject, DataSet SobekCM_Settings )
        {

            // Set some values that used to be constants in the original settings object.  
            // These are all canidates to be pushed into the database at some point
            SettingsObject.Reserved_Keywords = new List<string> {
                    "l", "my", "fragment", "json",
                    "dataset", "dataprovider", "xml", "textonly", "shibboleth", "internal",
                    "contact", "folder", "admin", "preferences", "stats", "statistics", "adminhelp",
                    "partners", "tree", "brief", "personalized", "all", "new", "map", "advanced",
                    "text", "results", "contains", "exact", "resultslike", "browseby", "info", "sobekcm", "inprocess", "engine"  };

            SettingsObject.Page_Image_Extensions = new List<string> { "JPG", "JP2", "JPX", "GIF", "PNG", "BMP", "JPEG" };
            SettingsObject.Backup_Files_Folder_Name = BACKUP_FILES_FOLDER_NAME;
            SettingsObject.Current_Web_Version = CURRENT_WEB_VERSION;
            SettingsObject.Current_Builder_Version = CURRENT_BUILDER_VERSION;
            SettingsObject.Complete_Package_Required_Aging = COMPLETE_PACKAGE_REQUIRED_AGING;
            SettingsObject.METS_Only_Package_Required_Aging = METS_ONLY_PACKAGE_REQUIRED_AGING;
            SettingsObject.VerifyCheckSum = VERIFY_CHECKSUM;
            SettingsObject.Pull_Facets_On_Browse = true;
            SettingsObject.Pull_Facets_On_Search = true;

            try
            {
                bool error = false;

                // Get the settings table
                DataTable settingsTable = SobekCM_Settings.Tables[0];

                // Create the dictionary for quick lookups for the next work
                Dictionary<string, string> settingsDictionary = new Dictionary<string, string>();
                foreach (DataRow thisRow in settingsTable.Rows)
                {
                    settingsDictionary[thisRow["Setting_Key"].ToString()] = thisRow["Setting_Value"].ToString().Trim();
                }

                // Pull all of the builder settings value ( from UFDC_Builder_Settings )
                Get_String_Value(settingsDictionary, "Application Server Network", SettingsObject, X => X.Application_Server_Network, ref error);
                Get_String_Value(settingsDictionary, "Application Server URL", SettingsObject, X => X.Application_Server_URL, ref error);
                Get_String_Value(settingsDictionary, "Archive DropBox", SettingsObject, X => X.Archive_DropBox, ref error);
                Get_Integer_Value(settingsDictionary, "Builder Log Expiration in Days", SettingsObject, X => X.Builder_Log_Expiration_Days, ref error, 10);
                Get_Integer_Value(settingsDictionary, "Builder Seconds Between Polls", SettingsObject, X => X.Builder_Seconds_Between_Polls, ref error, 60);
                Get_Boolean_Value(settingsDictionary, "Builder Verbose Flag", SettingsObject, X => X.Builder_Verbose_Flag, ref error, false);
                Get_String_Value(settingsDictionary, "Caching Server", SettingsObject, X => X.Caching_Server, ref error);
                Get_Boolean_Value(settingsDictionary, "Can Remove Single Search Term", SettingsObject, X => X.Can_Remove_Single_Term, ref error, true);
                Get_Boolean_Value(settingsDictionary, "Can Submit Edit Online", SettingsObject, X => X.Online_Edit_Submit_Enabled, ref error, false);
                Get_Boolean_Value(settingsDictionary, "Convert Office Files to PDF", SettingsObject, X => X.Convert_Office_Files_To_PDF, ref error, false);
                Get_Boolean_Value(settingsDictionary, "Detailed User Permissions", SettingsObject, X => X.Detailed_User_Aggregation_Permissions, ref error, false);
                Get_String_Value(settingsDictionary, "Document Solr Index URL", SettingsObject, X => X.Document_Solr_Index_URL, ref error);
                Get_Boolean_Value(settingsDictionary, "Facets Collapsible", SettingsObject, X => X.Facets_Collapsible, ref error, false);
                Get_String_Value(settingsDictionary, "FDA Report DropBox", SettingsObject, X => X.FDA_Report_DropBox, ref error);
                Get_String_Value(settingsDictionary, "Files To Exclude From Downloads", SettingsObject, X => X.Files_To_Exclude_From_Downloads, ref error);
                Get_String_Value(settingsDictionary, "Help URL", SettingsObject, X => X.Help_URL_Base, "http://ufdc.ufl.edu/");
                Get_String_Value(settingsDictionary, "Help Metadata URL", SettingsObject, X => X.Metadata_Help_URL_Base, "http://ufdc.ufl.edu/");
                Get_String_Value(settingsDictionary, "Image Server Network", SettingsObject, X => X.Image_Server_Network, ref error);
                Get_String_Value(settingsDictionary, "Image Server URL", SettingsObject, X => X.Image_URL, ref error);
                Get_Boolean_Value(settingsDictionary, "Include TreeView On System Home", SettingsObject, X => X.Include_TreeView_On_System_Home, ref error, false);
                Get_Boolean_Value(settingsDictionary, "Include Partners On System Home", SettingsObject, X => X.Include_Partners_On_System_Home, ref error, false);
                Get_Integer_Value(settingsDictionary, "JPEG Height", SettingsObject, X => X.JPEG_Height, ref error, -1);
                Get_Integer_Value(settingsDictionary, "JPEG Width", SettingsObject, X => X.JPEG_Width, ref error, -1);
                Get_String_Value(settingsDictionary, "JPEG2000 Server", SettingsObject, X => X.JP2ServerUrl, ref error);
                Get_String_Value(settingsDictionary, "JPEG2000 Server Type", SettingsObject, X => X.JP2ServerType, ref error);
                //Get_String_Value(settingsDictionary, "Kakadu JPEG2000 Create Command", ref kakaduJp2CreateCommand, ref error);
                Get_String_Value(settingsDictionary, "Log Files Directory", SettingsObject, X => X.Log_Files_Directory, ref error);
                Get_String_Value(settingsDictionary, "Log Files URL", SettingsObject, X => X.Log_Files_URL, ref error);
                Get_String_Value(settingsDictionary, "Main Builder Input Folder", SettingsObject, X => X.Main_Builder_Input_Folder, String.Empty);
                Get_String_Value(settingsDictionary, "Mango Union Search Base URL", SettingsObject, X => X.Mango_Union_Search_Base_URL, ref error);
                Get_String_Value(settingsDictionary, "Mango Union Search Text", SettingsObject, X => X.Mango_Union_Search_Text, ref error);
                Get_String_Value(settingsDictionary, "OCR Engine Command", SettingsObject, X => X.OCR_Command_Prompt, String.Empty);
                Get_String_Value(settingsDictionary, "Package Archival Folder", SettingsObject, X => X.Package_Archival_Folder, String.Empty);
                Get_String_Value(settingsDictionary, "Page Solr Index URL", SettingsObject, X => X.Page_Solr_Index_URL, String.Empty);
                Get_String_Value(settingsDictionary, "PostArchive Files To Delete", SettingsObject, X => X.PostArchive_Files_To_Delete, String.Empty);
                Get_String_Value(settingsDictionary, "PreArchive Files To Delete", SettingsObject, X => X.PreArchive_Files_To_Delete, String.Empty);
                Get_String_Value(settingsDictionary, "Privacy Email Address", SettingsObject, X => X.Privacy_Email_Address, String.Empty);
                Get_Boolean_Value(settingsDictionary, "Show Florida SUS Settings", SettingsObject, X => X.Show_Florida_SUS_Settings, ref error, false);
                Get_String_Value(settingsDictionary, "SobekCM Image Server", SettingsObject, X => X.SobekCM_ImageServer, String.Empty);
                Get_String_Value(settingsDictionary, "SobekCM Web Server IP", SettingsObject, X => X.SobekCM_Web_Server_IP, String.Empty);
                Get_String_Value(settingsDictionary, "Static Pages Location", SettingsObject, X => X.Static_Pages_Location, ref error);
                Get_Boolean_Value(settingsDictionary, "Statistics Caching Enabled", SettingsObject, X => X.Statistics_Caching_Enabled, ref error, false);
                Get_String_Value(settingsDictionary, "System Base Abbreviation", SettingsObject, X => X.System_Abbreviation, String.Empty);
                Get_String_Value(settingsDictionary, "System Base Name", SettingsObject, X => X.System_Name, SettingsObject.System_Abbreviation);
                Get_String_Value(settingsDictionary, "System Base URL", SettingsObject, X => X.System_Base_URL, String.Empty);
                Get_String_Value(settingsDictionary, "System Email", SettingsObject, X => X.System_Email, ref error);
                Get_String_Value(settingsDictionary, "System Error Email", SettingsObject, X => X.System_Error_Email, String.Empty);
                Get_Integer_Value(settingsDictionary, "Thumbnail Height", SettingsObject, X => X.Thumbnail_Height, ref error, -1);
                Get_Integer_Value(settingsDictionary, "Thumbnail Width", SettingsObject, X => X.Thumbnail_Width, ref error, -1);
                Get_String_Value(settingsDictionary, "Upload File Types", SettingsObject, X => X.Upload_File_Types, ".aif,.aifc,.aiff,.au,.avi,.bz2,.c,.c++,.css,.dbf,.ddl,.doc,.docx,.dtd,.dvi,.flac,.gz,.htm,.html,.java,.jps,.js,.m4p,.mid,.midi,.mp2,.mp3,.mpg,.odp,.ogg,.pdf,.pgm,.ppt,.pptx,.ps,.ra,.ram,.rar,.rm,.rtf,.sgml,.swf,.sxi,.tbz2,.tgz,.wav,.wave,.wma,.wmv,.xls,.xlsx,.xml,.zip");
                Get_String_Value(settingsDictionary, "Upload Image Types", SettingsObject, X => X.Upload_Image_Types, ".txt,.tif,.jpg,.jp2,.pro");
                Get_String_Value(settingsDictionary, "Web In Process Submission Location", SettingsObject, X => X.In_Process_Submission_Location, String.Empty);
                Get_Integer_Value(settingsDictionary, "Web Output Caching Minutes", SettingsObject, X => X.Web_Output_Caching_Minutes, ref error, 0);

                // Load the subsetting object for MarcXML 
                Marc21_Settings marcSettings = new Marc21_Settings();
                Get_String_Value(settingsDictionary, "MarcXML Feed Location", marcSettings, X => X.MarcXML_Feed_Location, String.Empty);
                Get_Boolean_Value(settingsDictionary, "Create MARC Feed By Default", marcSettings, X => X.Build_MARC_Feed_By_Default, ref error, false);
                Get_String_Value(settingsDictionary, "MARC Cataloging Source Code", marcSettings, X => X.Cataloging_Source_Code, String.Empty);
                Get_String_Value(settingsDictionary, "MARC Location Code", marcSettings, X => X.Location_Code, String.Empty);
                Get_String_Value(settingsDictionary, "MARC Reproduction Agency", marcSettings, X => X.Reproduction_Agency, SettingsObject.System_Name);
                Get_String_Value(settingsDictionary, "MARC Reproduction Place", marcSettings, X => X.Reproduction_Place, String.Empty);
                Get_String_Value(settingsDictionary, "MARC XSLT File", marcSettings, X => X.XSLT_File, String.Empty);
                SettingsObject.MarcGeneration = marcSettings;


                // Pull the language last, since it must be converted into a Language_Enum
                Get_String_Value(settingsDictionary, "System Default Language", SettingsObject, X => X.Default_UI_Language_String, "English");
                

                // Pull out some values, which are stored in this portion of the database, 
                // but are not really setting values
                settingsDictionary.Remove("Builder Last Message");
                settingsDictionary.Remove("Builder Last Run Finished");
                settingsDictionary.Remove("Builder Version");
                settingsDictionary.Remove("Builder Operation Flag");

                // Save the remaining values
                SettingsObject.Additional_Settings.Clear();
                foreach (KeyValuePair<string, string> thisSetting in settingsDictionary)
                {
                    SettingsObject.Additional_Settings[thisSetting.Key] = thisSetting.Value;
                }

                // Set the builder folder information
                Set_Builder_Folders(SettingsObject, SobekCM_Settings.Tables[1]);

                // Save the metadata types
                Set_Metadata_Types(SettingsObject, SobekCM_Settings.Tables[2]);

                // Set the workflow and disposition options
                Set_Workflow_And_Disposition_Types(SettingsObject, SobekCM_Settings.Tables[3], SobekCM_Settings.Tables[4]);

                // This fills some dictionaries and such used for easy lookups
                SettingsObject.PostUnSerialization();


                return true;
            }
            catch ( Exception ee )
            {
                return (ee.Message.Length > 0);
                return false;
            }
        }

        #region Helper methods tor setting individual setting values 

        private static void Get_String_Value<T>(Dictionary<string, string> Settings_Dictionary, string Key, T OutObj, Expression<Func<T, string>> OutExpr, ref bool Error)
        {
            if (Settings_Dictionary.ContainsKey(Key))
            {
                string newValue = Settings_Dictionary[Key].Trim();
                Settings_Dictionary.Remove(Key);

                MemberExpression expr = (MemberExpression)OutExpr.Body;
                PropertyInfo prop = (PropertyInfo)expr.Member;
                prop.SetValue(OutObj, newValue, null);
            }
            else
            {
                Error = true;
            }
        }

        private static void Get_String_Value<T>(Dictionary<string, string> Settings_Dictionary, string Key, T OutObj, Expression<Func<T, string>> OutExpr, string Default_Value)
        {
            if (Settings_Dictionary.ContainsKey(Key))
            {
                string newValue = Settings_Dictionary[Key].Trim();
                Settings_Dictionary.Remove(Key);

                MemberExpression expr = (MemberExpression)OutExpr.Body;
                PropertyInfo prop = (PropertyInfo)expr.Member;
                prop.SetValue(OutObj, newValue, null);
            }
            else
            {
                MemberExpression expr = (MemberExpression)OutExpr.Body;
                PropertyInfo prop = (PropertyInfo)expr.Member;
                prop.SetValue(OutObj, Default_Value, null);
            }
        }



        private static void Get_Boolean_Value<T>(Dictionary<string, string> Settings_Dictionary, string Key, T OutObj, Expression<Func<T, bool>> OutExpr, ref bool Error, bool? Default_Value)
        {
            if (Settings_Dictionary.ContainsKey(Key))
            {
                string value_as_string = Settings_Dictionary[Key].Trim();
                bool value_as_bool;
                if (bool.TryParse(value_as_string, out value_as_bool))
                {
                    MemberExpression expr = (MemberExpression) OutExpr.Body;
                    PropertyInfo prop = (PropertyInfo) expr.Member;
                    prop.SetValue(OutObj, value_as_bool, null);

                    Settings_Dictionary.Remove(Key);

                    return;
                }
            }

            if (Default_Value.HasValue)
            {
                MemberExpression expr = (MemberExpression) OutExpr.Body;
                PropertyInfo prop = (PropertyInfo) expr.Member;
                prop.SetValue(OutObj, Default_Value.Value, null);
            }
            else
            {
                Error = true;
            }
        }

        private static void Get_Integer_Value<T>(Dictionary<string, string> Settings_Dictionary, string Key, T OutObj, Expression<Func<T, int>> OutExpr, ref bool Error, int Default_Value)
        {
            if (Settings_Dictionary.ContainsKey(Key))
            {
                string value_as_string = Settings_Dictionary[Key].Trim();
                int value_as_int;
                if (int.TryParse(value_as_string, out value_as_int))
                {
                    MemberExpression expr = (MemberExpression) OutExpr.Body;
                    PropertyInfo prop = (PropertyInfo) expr.Member;
                    prop.SetValue(OutObj, value_as_int, null);

                    Settings_Dictionary.Remove(Key);
                }
                else
                {
                    Error = true;
                }
            }
            else
            {
                MemberExpression expr = (MemberExpression)OutExpr.Body;
                PropertyInfo prop = (PropertyInfo)expr.Member;
                prop.SetValue(OutObj, Default_Value, null);
            }
        }

        #endregion

        private static void Set_Builder_Folders(InstanceWide_Settings SettingsObject, DataTable BuilderFoldersTable)
        {
            SettingsObject.Incoming_Folders.Clear();
            foreach (DataRow thisRow in BuilderFoldersTable.Rows)
            {
                Builder_Source_Folder newFolder = new Builder_Source_Folder
                {
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
                    BibID_Roots_Restrictions = thisRow["BibID_Roots_Restrictions"].ToString()
                };

                if (thisRow["Can_Move_To_Content_Folder"] == DBNull.Value)
                    newFolder.Can_Move_To_Content_Folder = null;
                else
                    newFolder.Can_Move_To_Content_Folder = Convert.ToBoolean(thisRow["Can_Move_To_Content_Folder"]);
                newFolder.Module_Configuration_Raw = thisRow["ModuleConfig"].ToString();

                SettingsObject.Incoming_Folders.Add(newFolder);
            }
        }


        /// <summary> Saves all the metadata types from the database, to be treated as constant settings
        /// for the period of time the web application functios </summary>
        /// <param name="SettingsObject"> Settings instance to be populated with the metadata types </param>
        /// <param name="MetadataTypesTable"> DataTable with all the possible metadata types, from the database </param>
        private static void Set_Metadata_Types(InstanceWide_Settings SettingsObject, DataTable MetadataTypesTable)
        {
            SettingsObject.Metadata_Search_Fields.Clear();

            // Add ANYWHERE
            SettingsObject.Metadata_Search_Fields.Add(new Metadata_Search_Field(-1, String.Empty, "Anywhere", "ZZ", "all", "Anywhere"));

            // Add OCLC
            SettingsObject.Metadata_Search_Fields.Add(new Metadata_Search_Field(-2, String.Empty, "OCLC", "OC", "oclc", "OCLC"));


            // Add ALEPH
            SettingsObject.Metadata_Search_Fields.Add(new Metadata_Search_Field(-3, String.Empty, "ALEPH", "AL", "aleph", "ALEPH"));

            // Add Full Text
            SettingsObject.Metadata_Search_Fields.Add(new Metadata_Search_Field(-4, String.Empty, "Full Text", "TX", "fulltext", "Full Text"));


            // Get the data columns
            DataColumn idColumn = MetadataTypesTable.Columns["MetadataTypeID"];
            DataColumn codeColumn = MetadataTypesTable.Columns["SobekCode"];
            DataColumn displayColumn = MetadataTypesTable.Columns["DisplayTerm"];
            DataColumn facetColumn = MetadataTypesTable.Columns["FacetTerm"];
            DataColumn solrColumn = MetadataTypesTable.Columns["SolrCode"];
            DataColumn nameColumn = MetadataTypesTable.Columns["MetadataName"];

            // Now add the rest of the fields
            foreach (DataRow thisRow in MetadataTypesTable.Rows)
            {
                // Only add here if there is a sobek code
                if (thisRow[codeColumn] == DBNull.Value) continue;

                // Retrieve each individual value
                short id = Convert.ToInt16(thisRow[idColumn]);
                string code = thisRow[codeColumn].ToString().Trim();
                string display = thisRow[displayColumn].ToString();
                string facet = thisRow[facetColumn].ToString();
                string solr = thisRow[solrColumn].ToString();
                string name = thisRow[nameColumn].ToString();

                // Create the new field object
                Metadata_Search_Field newField = new Metadata_Search_Field(id, facet, display, code, solr, name);

                // Add this to the collections
                SettingsObject.Metadata_Search_Fields.Add(newField);
            }
        }


        /// <summary> Saves the possible workflow types and disposition values from the database, to be treated as constant settings
        /// for the period of time the web application functions </summary>
        /// <param name="SettingsObject"> Settings instance to be populated with the metadata types </param>
        /// <param name="WorkflowTable"> DataTable with all the possible workflow types, from the database </param>
        /// <param name="DispositionTypeTable"> DataTable with all the possible disposition types, from the database </param>
        private static void Set_Workflow_And_Disposition_Types(InstanceWide_Settings SettingsObject, DataTable WorkflowTable, DataTable DispositionTypeTable)
        {

            foreach (DataRow thisRow in DispositionTypeTable.Rows)
            {
                int id = Convert.ToInt32(thisRow["DispositionID"]);
                string future = thisRow["DispositionFuture"].ToString();
                string past = thisRow["DispositionPast"].ToString();

                Disposition_Option newOption = new Disposition_Option(id, past, future);
                SettingsObject.Disposition_Options.Add(newOption);
            }

            foreach (DataRow thisRow in WorkflowTable.Rows)
            {
                int id = Convert.ToInt32(thisRow["WorkFlowID"]);
                string workflow = thisRow["WorkFlowName"].ToString();

                Workflow_Type newWorkFlow = new Workflow_Type(id, workflow);
                SettingsObject.Workflow_Types.Add(newWorkFlow);
            }
        }


        /// <summary> Reads the inficated configuration file </summary>
        /// <param name="SettingsObject"> Settings instance to be populated with the metadata types </param>
        /// <param name="ConfigFile"> Configuration file to read </param>
        /// <exception>File is checked for existence first, otherwise all encountered exceptions will be thrown</exception>
        public static void Read_Configuration_File(InstanceWide_Settings SettingsObject, string ConfigFile)
        {
            if (!File.Exists(ConfigFile))
                return;

            SettingsObject.Database_Connections.Clear();

            StreamReader reader = new StreamReader(ConfigFile);
            XmlTextReader xmlReader = new XmlTextReader(reader);
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    string node_name = xmlReader.Name.ToLower();
                    switch (node_name)
                    {
                        case "connection_string":
                            Database_Instance_Configuration newDb = new Database_Instance_Configuration();
                            if (xmlReader.MoveToAttribute("type"))
                            {
                                if (xmlReader.Value.ToLower() == "postgresql")
                                    newDb.Database_Type = SobekCM_Database_Type_Enum.PostgreSQL;
                            }
                            if (xmlReader.MoveToAttribute("active"))
                            {
                                if (xmlReader.Value.ToLower() == "false")
                                    newDb.Is_Active = false;
                            }
                            if (xmlReader.MoveToAttribute("canAbort"))
                            {
                                if (xmlReader.Value.ToLower() == "false")
                                    newDb.Can_Abort = false;
                            }
                            if (xmlReader.MoveToAttribute("isHosted"))
                            {
                                if (xmlReader.Value.ToLower() == "true")
                                    SettingsObject.isHosted = true;
                            }
                            if (xmlReader.MoveToAttribute("name"))
                                newDb.Name = xmlReader.Value.Trim();

                            xmlReader.Read();
                            newDb.Connection_String = xmlReader.Value;
                            if (newDb.Name.Length == 0)
                                newDb.Name = "Connection" + (SettingsObject.Database_Connections.Count + 1);
                            SettingsObject.Database_Connections.Add(newDb);
                            break;

                        case "erroremails":
                            xmlReader.Read();
                            SettingsObject.System_Error_Email = xmlReader.Value;
                            break;

                        case "errorpage":
                            xmlReader.Read();
                            SettingsObject.System_Error_URL = xmlReader.Value;
                            break;

                        case "ghostscript_executable":
                            xmlReader.Read();
                            SettingsObject.Ghostscript_Executable = xmlReader.Value;
                            break;

                        case "imagemagick_executable":
                            xmlReader.Read();
                            SettingsObject.ImageMagick_Executable = xmlReader.Value;
                            break;

                        case "pause_between_polls":
                            xmlReader.Read();
                            int testValue;
                            if (Int32.TryParse(xmlReader.Value, out testValue))
                                SettingsObject.Builder_Override_Seconds_Between_Polls = testValue;
                            break;

                        case "publish_logs_directory":
                            xmlReader.Read();
                            SettingsObject.Builder_Logs_Publish_Directory = xmlReader.Value;
                            break;

                    }
                }
            }

            xmlReader.Close();
            reader.Close();
        }
    }
}
