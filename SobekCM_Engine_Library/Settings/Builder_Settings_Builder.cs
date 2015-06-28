#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using SobekCM.Core.Settings;

#endregion

namespace SobekCM.Engine_Library.Settings
{
    /// <summary> Creates the builder settings, which holds data that is used by the builder </summary>
    public class Builder_Settings_Builder
    {
        /// <summary> Refreshes the specified builder settings object, from the information pulled from the database </summary>
        /// <param name="SettingsObject"> Current builer settings object to refresh </param>
        /// <param name="SobekCM_Settings"> Dataset of all the builder settings, from the instance database </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool Refresh(Builder_Settings SettingsObject, DataSet SobekCM_Settings)
        {
            SettingsObject.Clear();
            try
            {
                Dictionary<int, List<Builder_Source_Folder>> folder_to_set_dictionary = new Dictionary<int, List<Builder_Source_Folder>>();
                Dictionary<int, List<Builder_Module_Setting>> setid_to_modules = new Dictionary<int, List<Builder_Module_Setting>>();

                Set_Builder_Folders(SettingsObject, SobekCM_Settings.Tables[0], folder_to_set_dictionary);

                Set_NonScheduled_Modules(SettingsObject, SobekCM_Settings.Tables[1], setid_to_modules);

                // Link the folders to the builder module sets
                foreach (KeyValuePair<int, List<Builder_Module_Setting>> module in setid_to_modules)
                {
                    if (folder_to_set_dictionary.ContainsKey(module.Key))
                    {
                        List<Builder_Source_Folder> folders = folder_to_set_dictionary[module.Key];
                        foreach (Builder_Source_Folder thisFolder in folders)
                        {
                            thisFolder.Builder_Module_Settings = module.Value;
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void Set_NonScheduled_Modules(Builder_Settings SettingsObject, DataTable BuilderFoldersTable, Dictionary<int, List<Builder_Module_Setting>> SetidToModules)
        {
            //DataColumn moduleIdColumn = BuilderFoldersTable.Columns["ModuleID"];
            DataColumn assemblyColumn = BuilderFoldersTable.Columns["Assembly"];
            DataColumn classColumn = BuilderFoldersTable.Columns["Class"];
           // DataColumn descColumn = BuilderFoldersTable.Columns["ModuleDesc"];
            DataColumn args1Column = BuilderFoldersTable.Columns["Argument1"];
            DataColumn args2Column = BuilderFoldersTable.Columns["Argument2"];
            DataColumn args3Column = BuilderFoldersTable.Columns["Argument3"];
           // DataColumn enabledColumn = BuilderFoldersTable.Columns["Enabled"];
            DataColumn setIdColumn = BuilderFoldersTable.Columns["ModuleSetID"];
           // DataColumn setNameColumn = BuilderFoldersTable.Columns["SetName"];
           // DataColumn setEnabledColumn = BuilderFoldersTable.Columns["SetEnabled"];
            DataColumn typeAbbrevColumn = BuilderFoldersTable.Columns["TypeAbbrev"];
           // DataColumn typeDescColumn = BuilderFoldersTable.Columns["TypeDescription"];


            int prevSetid = -1;
            List<Builder_Module_Setting> folderSettings = new List<Builder_Module_Setting>();
            foreach (DataRow thisRow in BuilderFoldersTable.Rows)
            {
                string type = thisRow[typeAbbrevColumn].ToString().ToUpper();

                Builder_Module_Setting newSetting = new Builder_Module_Setting
                {
                    Class = thisRow[classColumn].ToString()
                };
                if (thisRow[assemblyColumn] != DBNull.Value)
                    newSetting.Assembly = thisRow[assemblyColumn].ToString();
                if (thisRow[args1Column] != DBNull.Value)
                    newSetting.Argument1 = thisRow[args1Column].ToString();
                if (thisRow[args2Column] != DBNull.Value)
                    newSetting.Argument2 = thisRow[args2Column].ToString();
                if (thisRow[args3Column] != DBNull.Value)
                    newSetting.Argument3 = thisRow[args3Column].ToString();

                switch (type)
                {
                    case "PRE":
                        SettingsObject.PreProcessModulesSettings.Add(newSetting);
                        break;

                    case "POST":
                        SettingsObject.PostProcessModulesSettings.Add(newSetting);
                        break;

                    case "NEW":
                        SettingsObject.ItemProcessModulesSettings.Add(newSetting);
                        break;

                    case "DELT":
                        SettingsObject.ItemDeleteModulesSettings.Add(newSetting);
                        break;

                    case "FOLD":
                        int setId = Int32.Parse(thisRow[setIdColumn].ToString());
                        if (prevSetid != setId)
                        {
                            if (folderSettings.Count > 0)
                            {
                                SetidToModules[prevSetid] = folderSettings;
                                folderSettings = new List<Builder_Module_Setting>();
                            }
                            prevSetid = setId;
                            folderSettings.Add(newSetting);
                        }
                        else
                        {
                            folderSettings.Add(newSetting);
                        }
                        break;
                }
            }

            if (folderSettings.Count > 0)
            {
                SetidToModules[prevSetid] = folderSettings;
            }
        }
        

        private static void Set_Builder_Folders(Builder_Settings SettingsObject, DataTable BuilderFoldersTable, Dictionary<int, List<Builder_Source_Folder>> FolderToSetDictionary)
        {
            SettingsObject.IncomingFolders.Clear();
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

                if (( thisRow["ModuleSetID"] != null) && ( thisRow["ModuleSetID"].ToString().Length > 0 ))
                {
                    int id = Int32.Parse(thisRow["ModuleSetID"].ToString());
                    if (FolderToSetDictionary.ContainsKey(id))
                        FolderToSetDictionary[id].Add(newFolder);
                    else
                    {
                        FolderToSetDictionary[id] = new List<Builder_Source_Folder> {newFolder};
                    }
                }

                SettingsObject.IncomingFolders.Add(newFolder);
            }
        }
    }
}
