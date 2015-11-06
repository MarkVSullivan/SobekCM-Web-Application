#region Using directives

using System;
using System.Collections.Generic;
using SobekCM.Core.Aggregations;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Client;
using SobekCM.Core.Settings;
using SobekCM.Core.Skins;
using SobekCM.Core.Users;
using SobekCM.Core.WebContent.Hierarchy;
using SobekCM.Engine_Library.ApplicationState;

#endregion

namespace SobekCM.Library.UI
{
    /// <summary> Gateway to all the application-level cached data for the user interface </summary>
    public static class UI_ApplicationCache_Gateway
    {
        /// <summary> Resets all the application-level data  </summary>
        public static void ResetAll()
        {
            Engine_ApplicationCache_Gateway.RefreshAll();

            WebContent_Hierarchy_Clear();
        }

        /// <summary> Refresh the settings object by pulling the data back from the database </summary>
        public static void ResetSettings()
        {
            Engine_ApplicationCache_Gateway.RefreshSettings();
        }

        /// <summary> Clears the lists of globally defined default metadata sets and metadata input templates, so they 
        /// will be refreshed next time they are requested </summary>
        public static void ResetDefaultMetadataTemplates()
        {
            Engine_ApplicationCache_Gateway.RefreshDefaultMetadataTemplates();
        }

        /// <summary> List of all the globally defined default metadata sets for this instance </summary>
        public static List<Default_Metadata> Global_Default_Metadata
        {
            get { return Engine_ApplicationCache_Gateway.Global_Default_Metadata; }
        }

        /// <summary> List of all the globally defined metadata templates within this instance </summary>
        public static List<Template> Templates
        {
            get { return Engine_ApplicationCache_Gateway.Templates; }
        }

        /// <summary> Get the list of thematic headings for database searching (or build the collection and return it) </summary>
        public static List<Thematic_Heading> Thematic_Headings
        {
            get { return Engine_ApplicationCache_Gateway.Thematic_Headings; }
        }

        /// <summary> Get the dictionary of collection aliases (or build the collection and return it) </summary>
        public static Dictionary<string, string> Collection_Aliases
        {
            get { return Engine_ApplicationCache_Gateway.Collection_Aliases; }
        }

        /// <summary> Get the aggregation code list object (or build the object and return it) </summary>
        public static Aggregation_Code_Manager Aggregations
        {
            get { return Engine_ApplicationCache_Gateway.Codes; }
        }

        /// <summary> Get the dictionary of icon/wordmarks (or build the collection and return it) </summary>
        public static Dictionary<string, Wordmark_Icon> Icon_List    
        {
            get { return Engine_ApplicationCache_Gateway.Icon_List; }
        }

        /// <summary> Last time the date time value was refreshed </summary>
        public static DateTime? Last_Refresh
        {
            get { return Engine_ApplicationCache_Gateway.Last_Refresh; }
        }

        /// <summary> Get the search history (or build the object and return it) </summary>
        public static Recent_Searches Search_History
        {
            get { return Engine_ApplicationCache_Gateway.Search_History; }
        }

        /// <summary> Get the translation object (or build the object and return it) </summary>
        public static Language_Support_Info Translation
        {
            get { return Engine_ApplicationCache_Gateway.Translation; }
        }

        /// <summary> Get the URL portal list object (or build the object and return it) </summary>
        public static Portal_List URL_Portals
        {
            get { return Engine_ApplicationCache_Gateway.URL_Portals; }
        }

        /// <summary> Get the dictionary of mime types (or build the collection and return it) </summary>
        public static Dictionary<string, Mime_Type_Info> Mime_Types
        {
            get { return Engine_ApplicationCache_Gateway.Mime_Types; }
        }

        /// <summary> Get the list of all user groups (or build the collection and return it) </summary>
        public static List<User_Group> User_Groups
        {
            get { return Engine_ApplicationCache_Gateway.User_Groups; }
        }

        /// <summary> Get the statistics date range (or build the object and return it) </summary>
        public static Statistics_Dates Stats_Date_Range
        {
            get { return Engine_ApplicationCache_Gateway.Stats_Date_Range; }
        }

        /// <summary> Get the web skin collection object (or build the object and return it) </summary>
        public static Web_Skin_Collection Web_Skin_Collection
        {
            get { return Engine_ApplicationCache_Gateway.Web_Skin_Collection; }
        }

        /// <summary> Get the list of search stop words for database searching (or build the collection and return it) </summary>
        public static List<string> Search_Stop_Words
        {
            get { return Engine_ApplicationCache_Gateway.StopWords; }
        }

        /// <summary> Get the list of ip restriction ranges (or build the object and return it) </summary>
        public static IP_Restriction_Ranges IP_Restrictions
        {
            get { return Engine_ApplicationCache_Gateway.IP_Restrictions; }
        }

        /// <summary> Get the list of item viewer priority  (or build the collection and return it) </summary>
        public static List<string> Item_Viewer_Priority
        {
            get { return Engine_ApplicationCache_Gateway.Item_Viewer_Priority; }
        }

        /// <summary> Get the settings object (or build the object and return it) </summary>
        public static InstanceWide_Settings Settings
        {
            get { return Engine_ApplicationCache_Gateway.Settings; }
        }

        /// <summary> Get the item lookup object (or build the object and return it) </summary>
        public static Item_Lookup_Object Items
        {
            get { return Engine_ApplicationCache_Gateway.Items; }
        }


        private static WebContent_Hierarchy webContentHierarchy;

        /// <summary> Get the complete hierarchy of non-aggregational static web content pages and redirects, used for navigation </summary>
        public static WebContent_Hierarchy WebContent_Hierarchy
        {
            get
            {
                // If the web content hierarchy object is already built, return it
                if (webContentHierarchy != null)
                    return webContentHierarchy;

                // Not built, so retrieve from the engine and then store
                // this commonly used object here in this static class
                webContentHierarchy = SobekEngineClient.WebContent.Get_Hierarchy(false, null);
                return webContentHierarchy;
            }
        }

        /// <summary> Clear the cached web content hierarchy data </summary>
        public static void WebContent_Hierarchy_Clear()
        {
            webContentHierarchy = null;
        }
    }
}
