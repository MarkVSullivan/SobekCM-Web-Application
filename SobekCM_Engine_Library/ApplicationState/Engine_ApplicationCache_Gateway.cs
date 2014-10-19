#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using SobekCM.Core.Aggregations;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Settings;
using SobekCM.Core.Skins;
using SobekCM.Core.Users;
using SobekCM.EngineLibrary.ApplicationState;
using SobekCM.Engine_Library.Database;

#endregion

namespace SobekCM.Engine_Library.ApplicationState
{
    public static class Engine_ApplicationCache_Gateway
    {

        public static List<Thematic_Heading> Thematic_Headings;
        public static Dictionary<string, string> Collection_Aliases;
        public static Aggregation_Code_Manager Codes;
        public static Dictionary<string, Wordmark_Icon> Icon_List;
        public static DateTime? Last_Refresh;
        public static Recent_Searches Search_History;
        public static Language_Support_Info Translation;
        public static Portal_List URL_Portals;
        public static Dictionary<string, Mime_Type_Info> Mime_Types;
        public static List<string> Item_Viewer_Priority;
        public static List<User_Group> User_Groups;
        public static Statistics_Dates Stats_Date_Range;
        public static SobekCM_Skin_Collection Web_Skin_Collection;




        public static void RefreshAll()
        {
            try
            {
                lock (settingsLock)
                {
                    if (Settings == null)
                        Settings = InstanceWide_Settings_Builder.Build_Settings();
                    else
                    {
                        InstanceWide_Settings newSettings = InstanceWide_Settings_Builder.Build_Settings();
                        Settings = newSettings;
                    }
                }
            }
            catch
            {
                
            }
        }

        public static bool RefreshSettings()
        {
            try
            {
                lock (settingsLock)
                {
                    if (Settings == null)
                        Settings = InstanceWide_Settings_Builder.Build_Settings();
                    else
                    {
                        InstanceWide_Settings newSettings = InstanceWide_Settings_Builder.Build_Settings();
                        Settings = newSettings;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Properties and methods for the item lookup object

        private static Item_Lookup_Object itemLookup;

        private static readonly Object thisLock = new Object();

        /// <summary> Get the settings object (or build the object and return it) </summary>
        public static Item_Lookup_Object Items
        {
            get
            {
                lock (thisLock)
                {
                    return null;
                }
            }
            set
            {
                itemLookup = value;
            }
        }

        #endregion


        #region Properties and methods for the instance-wide settings

        private static InstanceWide_Settings settings;
        private static object settingsLock;

        public static bool ResetSettings()
        {
            try
            {
                lock (settingsLock)
                {
                    if (settings == null)
                        settings = InstanceWide_Settings_Builder.Build_Settings();
                    else
                    {
                        InstanceWide_Settings newSettings = InstanceWide_Settings_Builder.Build_Settings();
                        settings = newSettings;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary> Get the settings object (or build the object and return it) </summary>
        public static InstanceWide_Settings Settings
        {
            get
            {
                lock (settingsLock)
                {
                    return settings ?? (settings = InstanceWide_Settings_Builder.Build_Settings());
                }
            }
            set
            {
                settings = value;
            }
        }

        #endregion

        #region Properties and methods about the search stop words list


        private static List<string> searchStopWords;
        private static readonly Object searchStopWordsLock = new Object();


        public static List<string> Search_Stop_Words
        {
            get
            {
                lock (searchStopWordsLock)
                {
                    searchStopWords = Engine_Database.Search_Stop_Words(null);

                    return searchStopWords;
                }
            }
        }


        #endregion

        #region Properties and methods about the IP restriction lists

        private static IP_Restriction_Ranges ipRestrictions;
        private static readonly Object ipRestrictionsLock = new Object();

        /// <summary> Get the list of ip restriction ranges (or build the object and return it) </summary>
        public static IP_Restriction_Ranges IP_Restrictions
        {
            get
            {
                // This just ensures another thread doesn't pull a half built list
                lock (ipRestrictionsLock)
                {
                    if (ipRestrictionsLock == null)
                    {
                        DataTable ipRestrictionTbl = Engine_Database.Get_IP_Restriction_Ranges(null);
                        if (ipRestrictionTbl != null)
                        {
                            ipRestrictions = new IP_Restriction_Ranges();
                            ipRestrictions.Populate_IP_Ranges(ipRestrictionTbl);
                        }
                    }

                    return ipRestrictions;
                }
            }
        }


        #endregion

        #region Properties and methods about the checked out list

        private static Checked_Out_Items_List checkedList;
        private static readonly Object checkedListLock = new Object();

        /// <summary> Get the checked out list of items object (or build the object and return it) </summary>
        public static Checked_Out_Items_List Checked_List
        {
            get
            {
                // This just ensures another thread doesn't pull a half built list
                lock (checkedListLock)
                {
                    return checkedList ?? (checkedList = new Checked_Out_Items_List());
                }
            }
        }

        #endregion
    }
}
