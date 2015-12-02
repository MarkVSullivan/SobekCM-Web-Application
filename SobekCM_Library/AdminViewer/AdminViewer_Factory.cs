using System;
using System.IO;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Database;
using SobekCM.Library.MySobekViewer;
using SobekCM.Library.UI;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Factory class returns the appropriate admin viewer </summary>
    public static class AdminViewer_Factory
    {
        /// <summary> Returns the appropriate admin viewer, based on requst and system settings </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request  </param>
        /// <returns> Built admin viewer </returns>
        public static iMySobek_Admin_Viewer Get_AdminViewer(RequestCache RequestSpecificValues)
        {

            RequestSpecificValues.Tracer.Add_Trace("Admin_HtmlSubwriter.Get_AdminViewer", "Building the admin viewer object");
            switch (RequestSpecificValues.Current_Mode.Admin_Type)
            {
                case Admin_Type_Enum.Add_Collection_Wizard:
                    return new Add_Collection_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.Aggregation_Single:
                    return new Aggregation_Single_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.Aggregations_Mgmt:
                    return new Aggregations_Mgmt_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.Home:
                    return new Home_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.Builder_Status:
                    return new Builder_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.Skins_Single:
                    return new Skin_Single_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.Skins_Mgmt:
                    return new Skins_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.Aliases:
                    return new Aliases_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.WebContent_Add_New:
                    return new WebContent_Add_New_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.WebContent_Mgmt:
                    return new WebContent_Mgmt_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.WebContent_History:
                    return new WebContent_History_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.WebContent_Single:
                    return new WebContent_Single_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.WebContent_Usage:
                    return new WebContent_Usage_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.Wordmarks:
                    return new Wordmarks_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.URL_Portals:
                    return new Portals_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.Users:
                    return new Users_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.User_Groups:
                    return new User_Group_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.User_Permissions_Reports:
                    return new Permissions_Reports_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.IP_Restrictions:
                    return new IP_Restrictions_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.Thematic_Headings:
                    return new Thematic_Headings_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.Settings:
                    return new Settings_AdminViewer(RequestSpecificValues);

                case Admin_Type_Enum.Default_Metadata:
                    if ((!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.My_Sobek_SubMode)) && (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Length > 1))
                    {
                        string project_code = RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Substring(1);
                        RequestSpecificValues.Tracer.Add_Trace("AdminViewer_Factory.Get_AdminViewer", "Checking cache for valid project file");
                        if (RequestSpecificValues.Current_User != null)
                        {
                            SobekCM_Item projectObject = CachedDataManager.Retrieve_Project(RequestSpecificValues.Current_User.UserID, project_code, RequestSpecificValues.Tracer);
                            if (projectObject != null)
                            {
                                RequestSpecificValues.Tracer.Add_Trace("AdminViewer_Factory.Get_AdminViewer", "Valid default metadata set found in cache");
                                return new Edit_Item_Metadata_MySobekViewer(projectObject, RequestSpecificValues);
                            }
                            else
                            {
                                if (Engine_Database.Get_All_Template_DefaultMetadatas(RequestSpecificValues.Tracer).Tables[0].Select("MetadataCode='" + project_code + "'").Length > 0)
                                {
                                    RequestSpecificValues.Tracer.Add_Trace("AdminViewer_Factory.Get_AdminViewer", "Building default metadata set from (possible) PMETS");
                                    string pmets_file = UI_ApplicationCache_Gateway.Settings.Servers.Base_MySobek_Directory + "projects\\" + RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Substring(1) + ".pmets";
                                    SobekCM_Item pmets_item = File.Exists(pmets_file) ? SobekCM_Item.Read_METS(pmets_file) : new SobekCM_Item();
                                    pmets_item.Bib_Info.Main_Title.Title = "Default metadata set for '" + project_code + "'";
                                    pmets_item.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Project;
                                    pmets_item.BibID = project_code.ToUpper();
                                    pmets_item.VID = "00001";
                                    pmets_item.Source_Directory = UI_ApplicationCache_Gateway.Settings.Servers.Base_MySobek_Directory + "projects\\";

                                    RequestSpecificValues.Tracer.Add_Trace("AdminViewer_Factory.Get_AdminViewer", "Adding project file to cache");

                                    CachedDataManager.Store_Project(RequestSpecificValues.Current_User.UserID, project_code, pmets_item, RequestSpecificValues.Tracer);

                                    return new Edit_Item_Metadata_MySobekViewer(pmets_item, RequestSpecificValues);
                                }
                            }
                        }
                    }

                    // If it made it here, it must be manage all the default metadatas
                    return new Default_Metadata_AdminViewer(RequestSpecificValues);
            }

            return null;
        }
    }
}
