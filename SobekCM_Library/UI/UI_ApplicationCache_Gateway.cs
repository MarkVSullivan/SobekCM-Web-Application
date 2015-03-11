#region Using directives

using System;
using System.Collections.Generic;
using SobekCM.Core.Aggregations;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Settings;
using SobekCM.Core.Skins;
using SobekCM.Core.Users;
using SobekCM.EngineLibrary.ApplicationState;
using SobekCM.Engine_Library.ApplicationState;

#endregion

namespace SobekCM.UI_Library
{
    public static class UI_ApplicationCache_Gateway
    {
        public static void ResetAll()
        {
            Engine_ApplicationCache_Gateway.RefreshAll();
        }

        public static void ResetSettings()
        {
            Engine_ApplicationCache_Gateway.RefreshSettings();
        }

        public static void ResetDefaultMetadataTemplates()
        {
            Engine_ApplicationCache_Gateway.RefreshDefaultMetadataTemplates();
        }

        public static List<Default_Metadata> Global_Default_Metadata
        {
            get { return Engine_ApplicationCache_Gateway.Global_Default_Metadata; }
        }

        public static List<Template> Templates
        {
            get { return Engine_ApplicationCache_Gateway.Templates; }
        }

        public static List<Thematic_Heading> Thematic_Headings
        {
            get { return Engine_ApplicationCache_Gateway.Thematic_Headings; }
        }

        public static Dictionary<string, string> Collection_Aliases
        {
            get { return Engine_ApplicationCache_Gateway.Collection_Aliases; }
        }

        public static Aggregation_Code_Manager Aggregations
        {
            get { return Engine_ApplicationCache_Gateway.Codes; }
        }

        public static Dictionary<string, Wordmark_Icon> Icon_List    
        {
            get { return Engine_ApplicationCache_Gateway.Icon_List; }
        }

        public static DateTime? Last_Refresh
        {
            get { return Engine_ApplicationCache_Gateway.Last_Refresh; }
        }

        public static Recent_Searches Search_History
        {
            get { return Engine_ApplicationCache_Gateway.Search_History; }
        }

        public static Language_Support_Info Translation
        {
            get { return Engine_ApplicationCache_Gateway.Translation; }
        }

        public static Portal_List URL_Portals
        {
            get { return Engine_ApplicationCache_Gateway.URL_Portals; }
        }

        public static Dictionary<string, Mime_Type_Info> Mime_Types
        {
            get { return Engine_ApplicationCache_Gateway.Mime_Types; }
        }
        
        public static List<User_Group> User_Groups
        {
            get { return Engine_ApplicationCache_Gateway.User_Groups; }
        }

        public static Statistics_Dates Stats_Date_Range
        {
            get { return Engine_ApplicationCache_Gateway.Stats_Date_Range; }
        }

        public static Web_Skin_Collection Web_Skin_Collection
        {
            get { return Engine_ApplicationCache_Gateway.Web_Skin_Collection; }
        }

        public static List<string> Search_Stop_Words
        {
            get { return Engine_ApplicationCache_Gateway.StopWords; }
        }

        public static IP_Restriction_Ranges IP_Restrictions
        {
            get { return Engine_ApplicationCache_Gateway.IP_Restrictions; }
        }

        public static List<string> Item_Viewer_Priority
        {
            get { return Engine_ApplicationCache_Gateway.Item_Viewer_Priority; }
        }

        public static InstanceWide_Settings Settings
        {
            get { return Engine_ApplicationCache_Gateway.Settings; }
        }

        public static Item_Lookup_Object Items
        {
            get { return Engine_ApplicationCache_Gateway.Items; }
        }
    }
}
