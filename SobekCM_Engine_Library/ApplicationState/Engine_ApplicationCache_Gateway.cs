#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using SobekCM.Core.Aggregations;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Settings;
using SobekCM.Core.Skins;
using SobekCM.Core.Users;
using SobekCM.EngineLibrary.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Resource_Object.Database;

#endregion

namespace SobekCM.Engine_Library.ApplicationState
{
    public static class Engine_ApplicationCache_Gateway
    {
        /// <summary> Constructor for this gateway class, which sets the last refresh time </summary>
        static Engine_ApplicationCache_Gateway()
        {
            Last_Refresh = DateTime.Now;
        }

        public static DateTime? Last_Refresh;

        public static void ClearAll()
        {
            Last_Refresh = DateTime.Now;
        }

        public static bool RefreshAll( Database_Instance_Configuration DbInstance )
        {
            bool error = !RefreshSettings(DbInstance);
            error = error | !RefreshStatsDateRange();
            error = error | !RefreshTranslations();
            error = error | !RefreshWebSkins();
            error = error | !RefreshCodes();
            error = error | !RefreshItems();
            error = error | !RefreshStopWords();
            error = error | !RefreshIP_Restrictions();
            error = error | !RefreshThematicHeadings();
            error = error | !RefreshItemViewerPriority();
            error = error | !RefreshUserGroups();
            error = error | !RefreshCollectionAliases();
            error = error | !RefreshMimeTypes();
            error = error | !RefreshIcons();
            error = error | !RefreshDefaultMetadataTemplates();
            error = error | !RefreshUrlPortals();

            return !error;
        }
        
        public static bool RefreshAll()
        {
            bool error = !RefreshSettings();
            error = error | !RefreshStatsDateRange();
            error = error | !RefreshTranslations();
            error = error | !RefreshWebSkins();
            error = error | !RefreshCodes();
            error = error | !RefreshItems();
            error = error | !RefreshStopWords();
            error = error | !RefreshIP_Restrictions();
            error = error | !RefreshThematicHeadings();
            error = error | !RefreshItemViewerPriority();
            error = error | !RefreshUserGroups();
            error = error | !RefreshCollectionAliases();
            error = error | !RefreshMimeTypes();
            error = error | !RefreshIcons();
            error = error | !RefreshDefaultMetadataTemplates();
            error = error | !RefreshUrlPortals();

            return !error;
        }

        #region Properties and methods for the statistics date range

        private static Statistics_Dates statsDates;
        private static readonly Object statsDatesLock = new Object();

        /// <summary> Refresh the statistics date range by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshStatsDateRange()
        {
            try
            {
                lock (statsDatesLock)
                {
                    if (statsDates == null)
                    {
                        statsDates = new Statistics_Dates();
                    }

                    // Get the data from the database
                    Engine_Database.Populate_Statistics_Dates(statsDates, null);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Get the statistics date range (or build the object and return it) </summary>
        public static Statistics_Dates Stats_Date_Range
        {
            get
            {
                lock (statsDatesLock)
                {
                    if (statsDates == null)
                    {
                        statsDates = new Statistics_Dates();

                        Engine_Database.Populate_Statistics_Dates(statsDates, null);
                    }

                    return statsDates;
                }
            }
        }

        #endregion

        #region Properties and methods for the translation object 

        private static Language_Support_Info translations;
        private static readonly Object translationLock = new Object();

        /// <summary> Refresh the translation object by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshTranslations()
        {
            try
            {
                lock (translationLock)
                {
                    if (translations == null)
                    {
                        translations = new Language_Support_Info();
                    }

                    // Get the data from the database
                    Engine_Database.Populate_Translations(translations, null);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Get the translation object (or build the object and return it) </summary>
        public static Language_Support_Info Translation
        {
            get
            {
                lock (translationLock)
                {
                    if (translations == null)
                    {
                        translations = new Language_Support_Info();

                        Engine_Database.Populate_Translations(translations, null);
                    }

                    return translations;
                }
            }
        }

        #endregion

        #region Properties and methods for the web skin collection

        private static SobekCM_Skin_Collection webSkins;
        private static readonly Object webSkinsLock = new Object();

        /// <summary> Refresh the web skin collection by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshWebSkins()
        {
            try
            {
                lock (webSkinsLock)
                {
                    if (webSkins == null)
                    {
                        webSkins = new SobekCM_Skin_Collection();
                    }

                    // Get the data from the database
                    DataTable skinData = Engine_Database.Get_All_Web_Skins(null);

                    // Clear existing interfaces
                    webSkins.Clear();

                    // Just return if the data appears bad..
                    if ((skinData == null) || (skinData.Rows.Count == 0))
                        return false;

                    // Set the data table
                    webSkins.Skin_Table = skinData;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Get the web skin collection object (or build the object and return it) </summary>
        public static SobekCM_Skin_Collection Web_Skin_Collection
        {
            get
            {
                lock (webSkinsLock)
                {
                    if (webSkins == null)
                    {
                        webSkins = new SobekCM_Skin_Collection();

                        // Get the data from the database
                        DataTable skinData = Engine_Database.Get_All_Web_Skins(null);

                        // Clear existing interfaces
                        webSkins.Clear();

                        // Set the data table
                        webSkins.Skin_Table = skinData;
                    }

                    return webSkins;
                }
            }
        }

        #endregion

        #region Properties and methods for the URL portals list

        private static Portal_List portals;
        private static readonly Object portalsLock = new Object();

        /// <summary> Refresh the URL portals list by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshUrlPortals()
        {
            try
            {
                lock (portalsLock)
                {
                    if (portals == null)
                    {
                        portals = new Portal_List();
                    }

                    Engine_Database.Populate_URL_Portals(portals, null);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Get the URL portal list object (or build the object and return it) </summary>
        public static Portal_List URL_Portals
        {
            get
            {
                lock (portalsLock)
                {
                    if (portals == null)
                    {
                        portals = new Portal_List();
                        Engine_Database.Populate_URL_Portals(portals, null);
                    }

                    return portals;
                }
            }
        }

        #endregion

        #region Properties and methods for the aggregation codes list

        private static Aggregation_Code_Manager codes;

        private static readonly Object codesLock = new Object();

        /// <summary> Refresh the aggregation code list by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshCodes()
        {
            try
            {
                lock (codesLock)
                {
                    if (codes == null)
                    {
                        codes = new Aggregation_Code_Manager();
                    }

                    Engine_Database.Populate_Code_Manager(codes, null);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Get the aggregation code list object (or build the object and return it) </summary>
        public static Aggregation_Code_Manager Codes
        {
            get
            {
                lock (codesLock)
                {
                    if (codes == null)
                    {
                        codes = new Aggregation_Code_Manager();
                        Engine_Database.Populate_Code_Manager(codes, null);
                    }

                    return codes;
                }
            }
        }

        #endregion

        #region Properties and methods for the item lookup object

        private static Item_Lookup_Object itemLookup;

        private static readonly Object itemLookupLock = new Object();

        /// <summary> Refresh the item lookup object by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshItems()
        {
            try
            {
                lock (itemLookupLock)
                {
                    if (itemLookup == null)
                    {
                        itemLookup = new Item_Lookup_Object();
                    }

                    Engine_Database.Verify_Item_Lookup_Object(true, itemLookup, null);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Get the item lookup object (or build the object and return it) </summary>
        public static Item_Lookup_Object Items
        {
            get
            {
                lock (itemLookupLock)
                {
                    if (itemLookup == null)
                    {
                        itemLookup = new Item_Lookup_Object();
                        Engine_Database.Verify_Item_Lookup_Object(true, itemLookup, null);
                    }
                    
                    return itemLookup;
                }
            }
            //set
            //{
            //    itemLookup = value;
            //}
        }

        #endregion

        #region Properties and methods for the instance-wide settings

        private static InstanceWide_Settings settings;
        private static readonly Object settingsLock = new Object();

        /// <summary> Refresh the settings object by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshSettings(Database_Instance_Configuration DbInstance )
        {
            try
            {
                lock (settingsLock)
                {
                    if (settings == null)
                        settings = InstanceWide_Settings_Builder.Build_Settings(DbInstance);
                    else
                    {
                        InstanceWide_Settings newSettings = InstanceWide_Settings_Builder.Build_Settings(DbInstance);
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

        /// <summary> Refresh the settings object by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshSettings()
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

        /// <summary> Refresh the list of search stop words for database searching by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshStopWords()
        {
            try
            {
                lock (searchStopWordsLock)
                {
                    if ( searchStopWords != null )
                        searchStopWords.Clear();
                    searchStopWords = Engine_Database.Search_Stop_Words(null);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Get the list of search stop words for database searching (or build the collection and return it) </summary>
        public static List<string> StopWords
        {
            get
            {
                lock (searchStopWordsLock)
                {
                    return searchStopWords ?? (searchStopWords = Engine_Database.Search_Stop_Words(null));
                }
            }
        }


        #endregion

        #region Properties and methods about the IP restriction lists

        private static IP_Restriction_Ranges ipRestrictions;
        private static readonly Object ipRestrictionsLock = new Object();

        /// <summary> Refresh the list of ip restriction ranges by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshIP_Restrictions()
        {
            try
            {
                lock (settingsLock)
                {
                    lock (ipRestrictionsLock)
                    {
                        DataTable ipRestrictionTbl = Engine_Database.Get_IP_Restriction_Ranges(null);
                        if (ipRestrictionTbl != null)
                        {
                            ipRestrictions = new IP_Restriction_Ranges();
                            ipRestrictions.Populate_IP_Ranges(ipRestrictionTbl);
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

        /// <summary> Get the list of ip restriction ranges (or build the object and return it) </summary>
        public static IP_Restriction_Ranges IP_Restrictions
        {
            get
            {
                // This just ensures another thread doesn't pull a half built list
                lock (ipRestrictionsLock)
                {
                    if (ipRestrictions == null)
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

        #region Properties and methods about search history collection

        private static Recent_Searches searchHistory;
        private static readonly Object searchHistoryLock = new Object();

        /// <summary> Get the search history (or build the object and return it) </summary>
        public static Recent_Searches Search_History
        {
            get
            {
                // This just ensures another thread doesn't pull a half built list
                lock (searchHistoryLock)
                {
                    return searchHistory ?? (searchHistory = new Recent_Searches());
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

        #region Properties and methods about the thematic headings list

        private static List<Thematic_Heading> thematicHeadings;
        private static readonly Object thematicHeadingsLock = new Object();

        /// <summary> Refresh the list of thematic headings by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshThematicHeadings()
        {
            try
            {
                lock (thematicHeadingsLock)
                {
                    if ( thematicHeadings != null )
                        thematicHeadings.Clear();
                    if (!Engine_Database.Populate_Thematic_Headings(Thematic_Headings, null))
                    {
                        thematicHeadings = null;
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Get the list of thematic headings for database searching (or build the collection and return it) </summary>
        public static List<Thematic_Heading> Thematic_Headings
        {
            get
            {
                lock (thematicHeadingsLock)
                {
                    if (thematicHeadings == null)
                    {
                        thematicHeadings = new List<Thematic_Heading>();
                        if (!Engine_Database.Populate_Thematic_Headings(Thematic_Headings, null))
                        {
                            thematicHeadings = null;
                            throw Engine_Database.Last_Exception;
                        }
                    }

                    return thematicHeadings;
                }
            }
        }


        #endregion

        #region Properties and methods about the item viewer priority list

        private static List<string> itemViewerPriority;
        private static readonly Object itemViewerPriorityLock = new Object();

        /// <summary> Refresh the list of item viewer priority by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshItemViewerPriority()
        {
            try
            {
                lock (itemViewerPriorityLock)
                {
                    if (itemViewerPriority != null )
                        itemViewerPriority.Clear();
                    itemViewerPriority = Engine_Database.Get_Viewer_Priority(null);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Get the list of item viewer priority  (or build the collection and return it) </summary>
        public static List<string> Item_Viewer_Priority
        {
            get
            {
                lock (itemViewerPriorityLock)
                {
                    return itemViewerPriority ?? (itemViewerPriority = Engine_Database.Get_Viewer_Priority(null));
                }
            }
        }


        #endregion

        #region Properties and methods about the list of all user groups 

        private static List<User_Group> userGroups;
        private static readonly Object userGroupsLock = new Object();

        /// <summary> Refresh the list of all user groups by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshUserGroups()
        {
            try
            {
                lock (userGroupsLock)
                {
                    if ( userGroups != null )
                        userGroups.Clear();
                    userGroups = Engine_Database.Get_All_User_Groups(null);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Get the list of all user groups (or build the collection and return it) </summary>
        public static List<User_Group> User_Groups
        {
            get
            {
                lock (userGroupsLock)
                {
                    return userGroups ?? (userGroups = Engine_Database.Get_All_User_Groups(null));
                }
            }
        }


        #endregion

        #region Properties and methods about the collection aliases dictionary

        private static Dictionary<string, string> collectionAliases;
        private static readonly Object collectionAliasesLock = new Object();

        /// <summary> Refresh the list of aggregation/collection aliases by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshCollectionAliases()
        {
            try
            {
                lock (collectionAliasesLock)
                {
                    if (collectionAliases == null)
                        collectionAliases = new Dictionary<string, string>();

                    Engine_Database.Populate_Aggregation_Aliases(collectionAliases, null);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Get the dictionary of collection aliases (or build the collection and return it) </summary>
        public static Dictionary<string, string> Collection_Aliases
        {
            get
            {
                lock (collectionAliasesLock)
                {
                    if (collectionAliases == null)
                    {
                        collectionAliases = new Dictionary<string, string>();
                        Engine_Database.Populate_Aggregation_Aliases(collectionAliases, null);
                    }

                    return collectionAliases;
                }
            }
        }


        #endregion

        #region Properties and methods about the mime types dictionary

        private static Dictionary<string, Mime_Type_Info> mimeTypes;
        private static readonly Object mimeTypesLock = new Object();

        /// <summary> Refresh the list of mime types by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshMimeTypes()
        {
            try
            {
                lock (mimeTypesLock)
                {
                    if (mimeTypes == null)
                        mimeTypes = new Dictionary<string, Mime_Type_Info>();

                    Engine_Database.Populate_MIME_List(mimeTypes, null);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Get the dictionary of mime types (or build the collection and return it) </summary>
        public static Dictionary<string, Mime_Type_Info> Mime_Types
        {
            get
            {
                lock (mimeTypesLock)
                {
                    if (mimeTypes == null)
                    {
                        mimeTypes = new Dictionary<string, Mime_Type_Info>();
                        Engine_Database.Populate_MIME_List(mimeTypes, null);
                    }

                    return mimeTypes;
                }
            }
        }


        #endregion

        #region Properties and methods about the icon/wordmarks dictionary

        private static Dictionary<string, Wordmark_Icon> iconList;
        private static readonly Object iconListLock = new Object();

        /// <summary> Refresh the list of icon/wordmarks by pulling the data back from the database </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool RefreshIcons()
        {
            try
            {
                lock (iconListLock)
                {
                    if (iconList == null)
                        iconList = new Dictionary<string, Wordmark_Icon>();

                    Engine_Database.Populate_Icon_List(iconList, null);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Get the dictionary of icon/wordmarks (or build the collection and return it) </summary>
        public static Dictionary<string, Wordmark_Icon> Icon_List
        {
            get
            {
                lock (iconListLock)
                {
                    if (iconList == null)
                    {
                        iconList = new Dictionary<string, Wordmark_Icon>();
                        Engine_Database.Populate_Icon_List(iconList, null);
                    }

                    return iconList;
                }
            }
        }


        #endregion

        #region Properties and methods about the default metadata sets and templates

        private static List<Template> templateList;
        private static List<Default_Metadata> defaultMetadataList;
        private static readonly Object templateMetadataLock = new Object();

        private static void load_metadata_template()
        {
            // Get the list of all projects
            DataSet projectsSet = Engine_Database.Get_All_Template_DefaultMetadatas(null);
            if (projectsSet != null)
            {
                if (templateList == null)
                    templateList = new List<Template>();
                else
                    templateList.Clear();

                if (defaultMetadataList == null)
                    defaultMetadataList = new List<Default_Metadata>();
                else
                    defaultMetadataList.Clear();

                // Add each default metadata set
                foreach (DataRow thisRow in projectsSet.Tables[0].Rows)
                {
                    string code = thisRow["MetadataCode"].ToString();
                    string name = thisRow["MetadataName"].ToString();
                    string description = thisRow["Description"].ToString();

                    defaultMetadataList.Add(new Default_Metadata(code, name, description));
                }

                // Add each project
                foreach (DataRow thisRow in projectsSet.Tables[1].Rows)
                {
                    string code = thisRow["TemplateCode"].ToString();
                    string name = thisRow["TemplateName"].ToString();
                    string description = thisRow["Description"].ToString();

                    templateList.Add(new Template(code, name, description));
                }
            }  
        }

        public static List<Default_Metadata> Global_Default_Metadata
        {
            get
            {
                lock (templateMetadataLock)
                {
                    if ((templateList == null) || ( defaultMetadataList == null ))
                    {
                        load_metadata_template();
                    }

                    return defaultMetadataList;
                }
            }
        }

        public static List<Template> Templates
        {
            get
            {
                lock (templateMetadataLock)
                {
                    if ((templateList == null) || (defaultMetadataList == null))
                    {
                        load_metadata_template();
                    }

                    return templateList;
                }
            }
        }

        public static bool RefreshDefaultMetadataTemplates()
        {
            defaultMetadataList = null;
            templateList = null;
            return true;
        }

        #endregion
    }
}
