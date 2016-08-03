#region Using directives

using System;
using System.Text;
using System.Web;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;

#endregion

namespace SobekCM.Core.Navigation
{
    /// <summary> Class is used to write the standard SobekCM URL from the navigation object  </summary>
    public static class UrlWriterHelper
    {
        /// <summary> URL to send to if there is an unhandled error durig the URL construction </summary>
        public static string Unhandled_Error_URL { get; set;  }

        #region iSobekCM_Navigation_Object Members

        /// <summary> Returns the URL to redirect the user's browser, based on the current
        /// mode and specifics for this mode. </summary>
        /// <param name="Current_Mode"> Current navigation object which inludes all the navigation necessary objects </param>
        /// <param name="Item_View_Code">Item view code to display</param>
        /// <param name="Include_URL_Opts"> Flag indicates whether to include URL opts or not </param>
        /// <returns> String to be attached to the end of the main application name to redirect
        /// the current user's browser.  </returns>
        public static string Redirect_URL(Navigation_Object Current_Mode, string Item_View_Code, bool Include_URL_Opts)
        {
            string this_base_url = Current_Mode.Base_URL;

            // Determine the aggregation code to use
            string adjusted_aggregation = Current_Mode.Aggregation;
            if (!String.IsNullOrEmpty( Current_Mode.Aggregation_Alias))
                adjusted_aggregation = Current_Mode.Aggregation_Alias;


            // Add the writer type if it is not HTML 
            switch (Current_Mode.Writer_Type)
            {
                case Writer_Type_Enum.DataSet:
                    this_base_url = this_base_url + "dataset/";
                    break;

                case Writer_Type_Enum.Data_Provider:
                    this_base_url = this_base_url + "dataprovider/";
                    break;

                case Writer_Type_Enum.XML:
                    this_base_url = this_base_url + "xml/";
                    break;

                case Writer_Type_Enum.HTML_LoggedIn:
                    if (Current_Mode.Mode != Display_Mode_Enum.My_Sobek)
                    {
                        this_base_url = this_base_url + "l/";
                    }
                    break;

                case Writer_Type_Enum.Text:
                    this_base_url = this_base_url + "textonly/";
                    break;

                case Writer_Type_Enum.JSON:
                    this_base_url = this_base_url + "json/";
                    break;
            }

            string url_options = URL_Options(Current_Mode);
            string urlOptions1 = String.Empty;
            string urlOptions2 = String.Empty;
            if ((url_options.Length > 0) && (Include_URL_Opts))
            {
                urlOptions1 = "?" + url_options;
                urlOptions2 = "&" + url_options;
            }

            switch (Current_Mode.Mode)
            {
                case Display_Mode_Enum.Reports:
                    if (!String.IsNullOrEmpty(Current_Mode.Report_Name))
                        return this_base_url + "reports/" + Current_Mode.Report_Name + urlOptions1;
                    return this_base_url + "reports" + urlOptions1;

                case Display_Mode_Enum.Error:
                    return Unhandled_Error_URL;

                case Display_Mode_Enum.Internal:
                    switch (Current_Mode.Internal_Type)
                    {
                        case Internal_Type_Enum.Aggregations_List:
                            return this_base_url + "internal/aggregations/list" + urlOptions1;

                        case Internal_Type_Enum.Cache:
                            return this_base_url + "internal/cache" + urlOptions1;

                        case Internal_Type_Enum.Aggregations_Tree:
                            return this_base_url + "internal/aggregations/tree/" + urlOptions1;

                        case Internal_Type_Enum.New_Items:
                            if (!String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                                return this_base_url + "internal/new/" + Current_Mode.Info_Browse_Mode + urlOptions1;
                            return this_base_url + "internal/new" + urlOptions1;

                        case Internal_Type_Enum.Build_Failures:
                            return this_base_url + "internal/failures" + urlOptions1;

                        case Internal_Type_Enum.Wordmarks:
                            return this_base_url + "internal/wordmarks" + urlOptions1;
                    }
                    break;

                case Display_Mode_Enum.Contact:
                    if (!String.IsNullOrEmpty(Current_Mode.Aggregation))
                    {
                        if (!String.IsNullOrEmpty(Current_Mode.Error_Message))
                            return this_base_url + "contact/" + Current_Mode.Aggregation + "?em=" + HttpUtility.HtmlEncode(Current_Mode.Error_Message) + urlOptions2;
                        return this_base_url + "contact/" + Current_Mode.Aggregation + urlOptions1;
                    }
                    if (!String.IsNullOrEmpty(Current_Mode.Error_Message))
                        return this_base_url + "contact?em=" + HttpUtility.HtmlEncode(Current_Mode.Error_Message) + urlOptions2;
                    return this_base_url + "contact" + urlOptions1;

                case Display_Mode_Enum.Contact_Sent:
                    return this_base_url + "contact/sent" + urlOptions1;

                case Display_Mode_Enum.Public_Folder:
                    if (Current_Mode.FolderID > 0)
                    {
                        StringBuilder folderBuilder = new StringBuilder(this_base_url + "folder/" + Current_Mode.FolderID);
                        switch (Current_Mode.Result_Display_Type)
                        {
                            case Result_Display_Type_Enum.Brief:
                                folderBuilder.Append("/brief");
                                break;
                            case Result_Display_Type_Enum.Export:
                                folderBuilder.Append("/export");
                                break;
                            case Result_Display_Type_Enum.Full_Citation:
                                folderBuilder.Append("/citation");
                                break;
                            case Result_Display_Type_Enum.Full_Image:
                                folderBuilder.Append("/image");
                                break;
                            case Result_Display_Type_Enum.Map:
                                folderBuilder.Append("/map");
                                break;
                            case Result_Display_Type_Enum.Map_Beta:
                                folderBuilder.Append("/mapbeta");
                                break;
                            case Result_Display_Type_Enum.Table:
                                folderBuilder.Append("/table");
                                break;
                            case Result_Display_Type_Enum.Thumbnails:
                                folderBuilder.Append("/thumbs");
                                break;
                            default:
                                folderBuilder.Append("/brief");
                                break;
                        }
                        if (Current_Mode.Page > 1)
                        {
                            folderBuilder.Append("/" + Current_Mode.Page.ToString());
                        }
                        return folderBuilder + urlOptions1;
                    }
                    return this_base_url + "folder" + urlOptions1;

                case Display_Mode_Enum.Simple_HTML_CMS:
                    if ( String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                        return this_base_url + urlOptions1;

                    string simple_html_cms_url = this_base_url + Current_Mode.Info_Browse_Mode + urlOptions1;
                    switch (Current_Mode.WebContent_Type)
                    {
                        case WebContent_Type_Enum.Delete_Verify:
                            return (simple_html_cms_url.IndexOf("?") > 0) ? simple_html_cms_url + "&mode=verify" : simple_html_cms_url + "?mode=verify";

                        case WebContent_Type_Enum.Edit:
                            return (simple_html_cms_url.IndexOf("?") > 0) ? simple_html_cms_url + "&mode=edit" : simple_html_cms_url + "?mode=edit";

                        case WebContent_Type_Enum.Manage_Menu:
                            return (simple_html_cms_url.IndexOf("?") > 0) ? simple_html_cms_url + "&mode=menu" : simple_html_cms_url + "?mode=menu";

                        case WebContent_Type_Enum.Milestones:
                            return (simple_html_cms_url.IndexOf("?") > 0) ? simple_html_cms_url + "&mode=miletsones" : simple_html_cms_url + "?mode=milestones";

                        case WebContent_Type_Enum.Permissions:
                            return (simple_html_cms_url.IndexOf("?") > 0) ? simple_html_cms_url + "&mode=permissions" : simple_html_cms_url + "?mode=permissions";

                        case WebContent_Type_Enum.Usage:
                            return (simple_html_cms_url.IndexOf("?") > 0) ? simple_html_cms_url + "&mode=usage" : simple_html_cms_url + "?mode=usage";

                        default:
                            return simple_html_cms_url;
                    }

                case Display_Mode_Enum.My_Sobek:
                    switch (Current_Mode.My_Sobek_Type)
                    {
                        case My_Sobek_Type_Enum.Logon:
                            if (!String.IsNullOrEmpty(Current_Mode.Return_URL))
                            {
                                return this_base_url + "my/logon?return=" + HttpUtility.UrlEncode(Current_Mode.Return_URL).Replace("%2c", ",") + urlOptions2;
                            }
                            return this_base_url + "my/logon" + urlOptions1;

                        case My_Sobek_Type_Enum.Home:
                            if (!String.IsNullOrEmpty(Current_Mode.Return_URL))
                                return this_base_url + "my/home?return=" + HttpUtility.UrlEncode(Current_Mode.Return_URL).Replace("%2c", ",") + urlOptions2;
                            return this_base_url + "my/home" + urlOptions1;

                        case My_Sobek_Type_Enum.Delete_Item:
                            return this_base_url + "my/delete/" + Current_Mode.BibID + "/" + Current_Mode.VID + urlOptions1;

                        case My_Sobek_Type_Enum.New_Item:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "my/submit/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "my/submit" + urlOptions1;

                        case My_Sobek_Type_Enum.Edit_Item_Behaviors:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "my/behaviors/" + Current_Mode.BibID + "/" + Current_Mode.VID + "/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "my/behaviors/" + Current_Mode.BibID + "/" + Current_Mode.VID + urlOptions1;

                        case My_Sobek_Type_Enum.Edit_Item_Metadata:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "my/edit/" + Current_Mode.BibID + "/" + Current_Mode.VID + "/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "my/edit/" + Current_Mode.BibID + "/" + Current_Mode.VID + urlOptions1;

                        case My_Sobek_Type_Enum.Edit_Item_Permissions:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "my/itempermissions/" + Current_Mode.BibID + "/" + Current_Mode.VID + "/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "my/itempermissions/" + Current_Mode.BibID + "/" + Current_Mode.VID + urlOptions1;

                        case My_Sobek_Type_Enum.File_Management:
                            return this_base_url + "my/files/" + Current_Mode.BibID + "/" + Current_Mode.VID + urlOptions1;

                        case My_Sobek_Type_Enum.Page_Images_Management:
                            return this_base_url + "my/images/" + Current_Mode.BibID + "/" + Current_Mode.VID + urlOptions1;

                        case My_Sobek_Type_Enum.Edit_Group_Behaviors:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "my/groupbehaviors/" + Current_Mode.BibID + "/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "my/groupbehaviors/" + Current_Mode.BibID + urlOptions1;

                        case My_Sobek_Type_Enum.Edit_Group_Serial_Hierarchy:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "my/serialhierarchy/" + Current_Mode.BibID + "/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "my/serialhierarchy/" + Current_Mode.BibID + urlOptions1;

                        case My_Sobek_Type_Enum.Group_Add_Volume:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "my/addvolume/" + Current_Mode.BibID + "/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "my/addvolume/" + Current_Mode.BibID + urlOptions1;

                        case My_Sobek_Type_Enum.Group_AutoFill_Volumes:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "my/autofill/" + Current_Mode.BibID + "/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "my/autofill/" + Current_Mode.BibID + urlOptions1;

                        case My_Sobek_Type_Enum.Group_Mass_Update_Items:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "my/massupdate/" + Current_Mode.BibID + "/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "my/massupdate/" + Current_Mode.BibID + urlOptions1;

                        case My_Sobek_Type_Enum.Folder_Management:
                            switch (Current_Mode.Result_Display_Type)
                            {
                                case Result_Display_Type_Enum.Brief:
                                    if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                    {
                                        if (Current_Mode.Page > 1)
                                        {
                                            return this_base_url + "my/bookshelf/brief/" + Current_Mode.My_Sobek_SubMode + "/" + Current_Mode.Page + urlOptions1;
                                        }
                                        return this_base_url + "my/bookshelf/brief/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                                    }
                                    return this_base_url + "my/bookshelf" + urlOptions1;

                                case Result_Display_Type_Enum.Export:
                                    if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                    {
                                        if (Current_Mode.Page > 1)
                                        {
                                            return this_base_url + "my/bookshelf/export/" + Current_Mode.My_Sobek_SubMode + "/" + Current_Mode.Page + urlOptions1;
                                        }
                                        return this_base_url + "my/bookshelf/export/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                                    }
                                    return this_base_url + "my/bookshelf" + urlOptions1;

                                case Result_Display_Type_Enum.Full_Citation:
                                    if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                    {
                                        if (Current_Mode.Page > 1)
                                        {
                                            return this_base_url + "my/bookshelf/citation/" + Current_Mode.My_Sobek_SubMode + "/" + Current_Mode.Page + urlOptions1;
                                        }
                                        return this_base_url + "my/bookshelf/citation/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                                    }
                                    return this_base_url + "my/bookshelf" + urlOptions1;

                                case Result_Display_Type_Enum.Full_Image:
                                    if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                    {
                                        if (Current_Mode.Page > 1)
                                        {
                                            return this_base_url + "my/bookshelf/image/" + Current_Mode.My_Sobek_SubMode + "/" + Current_Mode.Page + urlOptions1;
                                        }
                                        return this_base_url + "my/bookshelf/image/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                                    }
                                    return this_base_url + "my/bookshelf" + urlOptions1;

                                case Result_Display_Type_Enum.Table:
                                    if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                    {
                                        if (Current_Mode.Page > 1)
                                        {
                                            return this_base_url + "my/bookshelf/table/" + Current_Mode.My_Sobek_SubMode + "/" + Current_Mode.Page + urlOptions1;
                                        }
                                        return this_base_url + "my/bookshelf/table/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                                    }
                                    return this_base_url + "my/bookshelf" + urlOptions1;

                                case Result_Display_Type_Enum.Thumbnails:
                                    if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                    {
                                        if (Current_Mode.Page > 1)
                                        {
                                            return this_base_url + "my/bookshelf/thumbs/" + Current_Mode.My_Sobek_SubMode + "/" + Current_Mode.Page + urlOptions1;
                                        }
                                        return this_base_url + "my/bookshelf/thumbs/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                                    }
                                    return this_base_url + "my/bookshelf" + urlOptions1;

                                default:
                                    if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                    {
                                        if (Current_Mode.Page > 1)
                                        {
                                            return this_base_url + "my/bookshelf/" + Current_Mode.My_Sobek_SubMode + "/" + Current_Mode.Page + urlOptions1;
                                        }
                                        return this_base_url + "my/bookshelf/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                                    }
                                    return this_base_url + "my/bookshelf" + urlOptions1;
                            }


                        case My_Sobek_Type_Enum.Preferences:
                            return this_base_url + "my/preferences" + urlOptions1;

                        case My_Sobek_Type_Enum.Log_Out:
                            if (!String.IsNullOrEmpty(Current_Mode.Return_URL))
                            {
                                string modified_return_url = Current_Mode.Return_URL;
                                if ((modified_return_url.IndexOf("my/") == 0) || (modified_return_url == "my"))
                                    modified_return_url = string.Empty;
                                if (modified_return_url.IndexOf("l/") == 0)
                                {
                                    modified_return_url = modified_return_url.Length == 2 ? String.Empty : Current_Mode.Return_URL.Substring(2);
                                }

                                if (modified_return_url.Length > 0)
                                    return this_base_url + "my/logout?return=" + HttpUtility.UrlEncode(modified_return_url).Replace("%2c", ",") + urlOptions2;
                                return this_base_url + "my/logout" + urlOptions1;
                            }
                            return this_base_url + "my/logout" + urlOptions1;

                        case My_Sobek_Type_Enum.Shibboleth_Landing:
                            if (!String.IsNullOrEmpty(Current_Mode.Return_URL))
                                return this_base_url + "my/shibboleth?return=" + HttpUtility.UrlEncode(Current_Mode.Return_URL).Replace("%2c", ",") + urlOptions2;
                            return this_base_url + "my/shibboleth" + urlOptions1;

                        case My_Sobek_Type_Enum.Saved_Searches:
                            return this_base_url + "my/searches" + urlOptions1;

                        case My_Sobek_Type_Enum.Item_Tracking:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "my/itemtracking/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "my/itemtracking" + urlOptions1;

                        case My_Sobek_Type_Enum.User_Tags:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "my/tags/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "my/tags" + urlOptions1;

                        case My_Sobek_Type_Enum.User_Usage_Stats:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "my/stats/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "my/stats" + urlOptions1;
                    }
                    break;

                case Display_Mode_Enum.Administrative:
                    switch (Current_Mode.Admin_Type)
                    {
                        case Admin_Type_Enum.Home:
                            return this_base_url + "admin" + urlOptions1;

                        case Admin_Type_Enum.Add_Collection_Wizard:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "admin/addcoll/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "admin/addcoll" + urlOptions1;

                        case Admin_Type_Enum.Aggregation_Single:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "admin/editaggr/" + Current_Mode.Aggregation + "/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "admin/editaggr/" + Current_Mode.Aggregation + urlOptions1;

                        case Admin_Type_Enum.Aggregations_Mgmt:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "admin/aggregations/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "admin/aggregations" + urlOptions1;

                        case Admin_Type_Enum.Builder_Status:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "admin/builder/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "admin/builder" + urlOptions1;

                        case Admin_Type_Enum.Aliases:
                            return this_base_url + "admin/aliases" + urlOptions1;

                        case Admin_Type_Enum.Default_Metadata:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "admin/defaults/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "admin/defaults" + urlOptions1;

                        case Admin_Type_Enum.IP_Restrictions:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "admin/restrictions/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "admin/restrictions" + urlOptions1;

                        case Admin_Type_Enum.URL_Portals:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "admin/portals/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "admin/portals" + urlOptions1;

                        case Admin_Type_Enum.Users:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "admin/users/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "admin/users" + urlOptions1;

                        case Admin_Type_Enum.User_Groups:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "admin/groups/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "admin/groups" + urlOptions1;

                        case Admin_Type_Enum.User_Permissions_Reports:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "admin/permissions/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "admin/permissions" + urlOptions1;

                        case Admin_Type_Enum.WebContent_Add_New:
                            return this_base_url + "admin/webadd" + urlOptions1;

                        case Admin_Type_Enum.WebContent_History:
                            return this_base_url + "admin/webhistory" + urlOptions1;

                        case Admin_Type_Enum.WebContent_Mgmt:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "admin/webcontent/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "admin/webcontent" + urlOptions1;

                        case Admin_Type_Enum.WebContent_Single:
                            if ((Current_Mode.WebContentID.HasValue) && (Current_Mode.WebContentID > 0))
                            {
                                if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                    return this_base_url + "admin/websingle/" + Current_Mode.WebContentID + "/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                                return this_base_url + "admin/websingle/" + Current_Mode.WebContentID + urlOptions1;
                            }
                            return this_base_url + "admin/webcontent" + urlOptions1;

                        case Admin_Type_Enum.WebContent_Usage:
                            return this_base_url + "admin/webusage" + urlOptions1;

                        case Admin_Type_Enum.Wordmarks:
                            return this_base_url + "admin/wordmarks" + urlOptions1;

                        case Admin_Type_Enum.Reset:
                            return this_base_url + "admin/reset" + urlOptions1;

                        case Admin_Type_Enum.Thematic_Headings:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "admin/headings/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "admin/headings" + urlOptions1;

                        case Admin_Type_Enum.TEI:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "admin/tei/" + Current_Mode.My_Sobek_SubMode + urlOptions1;
                            return this_base_url + "admin/tei" + urlOptions1;

                        case Admin_Type_Enum.Skins_Mgmt:
                            return this_base_url + "admin/webskins" + urlOptions1;

                        case Admin_Type_Enum.Skins_Single:
                            if (!String.IsNullOrEmpty(Current_Mode.My_Sobek_SubMode))
                                return this_base_url + "admin/editskin/" + Current_Mode.My_Sobek_SubMode.Replace("|", "/") + urlOptions1;
                            return this_base_url + "admin/webskins" + urlOptions1;

                        case Admin_Type_Enum.Settings:
                            return create_basic_url(this_base_url, "admin/settings", Current_Mode.Remaining_Url_Segments, urlOptions1);

                        case Admin_Type_Enum.Builder_Folder_Mgmt:
                            return create_basic_url(this_base_url, "admin/builderfolder", Current_Mode.Remaining_Url_Segments, urlOptions1);

                        default:
                            return this_base_url + "admin" + urlOptions1;
                    }

                case Display_Mode_Enum.Preferences:
                    return this_base_url + "preferences" + urlOptions1;

                case Display_Mode_Enum.Statistics:
                    switch (Current_Mode.Statistics_Type)
                    {
                        case Statistics_Type_Enum.Item_Count_Standard_View:
                            return this_base_url + "stats/itemcount" + urlOptions1;

                        case Statistics_Type_Enum.Item_Count_Growth_View:
                            return this_base_url + "stats/itemcount/growth" + urlOptions1;

                        case Statistics_Type_Enum.Item_Count_Arbitrary_View:
                            if (!String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                                return this_base_url + "stats/itemcount/arbitrary/" + Current_Mode.Info_Browse_Mode + urlOptions1;
                            return this_base_url + "stats/itemcount/arbitrary" + urlOptions1;

                        case Statistics_Type_Enum.Item_Count_Text:
                            return this_base_url + "stats/itemcount/text" + urlOptions1;

                        case Statistics_Type_Enum.Recent_Searches:
                            return this_base_url + "stats/searches" + urlOptions1;

                        case Statistics_Type_Enum.Usage_Overall:
                            return this_base_url + "stats/usage" + urlOptions1;

                        case Statistics_Type_Enum.Usage_Collection_History:
                            if (!String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                                return this_base_url + "stats/usage/history/" + Current_Mode.Info_Browse_Mode + urlOptions1;
                            return this_base_url + "stats/usage/history" + urlOptions1;

                        case Statistics_Type_Enum.Usage_Collection_History_Text:
                            if (!String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                                return this_base_url + "stats/usage/history/text/" + Current_Mode.Info_Browse_Mode + urlOptions1;
                            return this_base_url + "stats/usage/history/text" + urlOptions1;

                        case Statistics_Type_Enum.Usage_Collections_By_Date:
                            if (!String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                                return this_base_url + "stats/usage/collections/" + Current_Mode.Info_Browse_Mode + urlOptions1;
                            return this_base_url + "stats/usage/collections" + urlOptions1;

                        case Statistics_Type_Enum.Usage_Definitions:
                            return this_base_url + "stats/usage/definitions" + urlOptions1;

                        case Statistics_Type_Enum.Usage_Item_Views_By_Date:
                            if (!String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                                return this_base_url + "stats/usage/items/" + Current_Mode.Info_Browse_Mode + urlOptions1;
                            return this_base_url + "stats/usage/items" + urlOptions1;

                        case Statistics_Type_Enum.Usage_Items_By_Collection:
                            if (!String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                                return this_base_url + "stats/usage/items/top/" + Current_Mode.Info_Browse_Mode + urlOptions1;
                            return this_base_url + "stats/usage/items/top" + urlOptions1;

                        case Statistics_Type_Enum.Usage_Titles_By_Collection:
                            if (!String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                                return this_base_url + "stats/usage/titles/top/" + Current_Mode.Info_Browse_Mode + urlOptions1;
                            return this_base_url + "stats/usage/titles/top" + urlOptions1;

                        case Statistics_Type_Enum.Usage_By_Date_Text:
                            if (!String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                                return this_base_url + "stats/usage/items/text/" + Current_Mode.Info_Browse_Mode + urlOptions1;
                            return this_base_url + "stats/usage/items/text" + urlOptions1;
                    }
                    break;

                case Display_Mode_Enum.Item_Display:
                case Display_Mode_Enum.Item_Print:
                    if (!String.IsNullOrEmpty(Current_Mode.BibID))
                    {
                        // Build the url for this item
                        StringBuilder itemDisplayBuilder = new StringBuilder(this_base_url + Current_Mode.BibID.ToUpper(), 100);
                        if (!String.IsNullOrEmpty(Current_Mode.VID))
                            itemDisplayBuilder.Append("/" + Current_Mode.VID);
                        if (Current_Mode.Mode == Display_Mode_Enum.Item_Print)
                            itemDisplayBuilder.Append("/print");
                        if (!String.IsNullOrEmpty(Item_View_Code))
                            itemDisplayBuilder.Append("/" + Item_View_Code);
                        if (Current_Mode.SubPage > 1)
                            itemDisplayBuilder.Append("/" + Current_Mode.SubPage.ToString());

                        bool query_string_started = false;

                        // Check for any query string to be included 
                        if (((String.IsNullOrEmpty(Item_View_Code)) && (!String.IsNullOrEmpty(Current_Mode.Page_By_FileName))) || (!String.IsNullOrEmpty(Current_Mode.Text_Search)) || (!String.IsNullOrEmpty(Current_Mode.Coordinates)))
                        {
                            // Add either the text search or text display, if they exist
                            if (!String.IsNullOrEmpty(Current_Mode.Text_Search))
                            {
                                itemDisplayBuilder.Append("?search=" + HttpUtility.UrlEncode(Current_Mode.Text_Search));
                                query_string_started = true;
                            }

                            // Add the coordinates if they exist
                            if (!String.IsNullOrEmpty(Current_Mode.Coordinates))
                            {
                                if (!query_string_started)
                                {
                                    itemDisplayBuilder.Append("?coord=" + Current_Mode.Coordinates);
                                    query_string_started = true;
                                }
                                else
                                {
                                    itemDisplayBuilder.Append("&coord=" + Current_Mode.Coordinates);
                                }
                            }
                        }

                        //Add the number and size of thumbnails if this is the THUMBNAILS (Related Images) View
                        if (( !String.IsNullOrEmpty(Current_Mode.ViewerCode)) && (Current_Mode.ViewerCode.IndexOf("thumbs") >= 0) && (Current_Mode.Thumbnails_Per_Page >= -1))
                        {
                            if (!query_string_started)
                            {
                                itemDisplayBuilder.Append("?nt=" + Current_Mode.Thumbnails_Per_Page);
                                query_string_started = true;
                            }
                            else
                            {
                                itemDisplayBuilder.Append("&nt=" + Current_Mode.Thumbnails_Per_Page);
                            }
                        }

                        if (( !String.IsNullOrEmpty(Current_Mode.ViewerCode)) && (Current_Mode.ViewerCode.IndexOf("thumbs") >= 0) && (Current_Mode.Size_Of_Thumbnails > 0))
                        {
                            if (!query_string_started)
                            {
                                itemDisplayBuilder.Append("?ts=" + Current_Mode.Size_Of_Thumbnails);
                                query_string_started = true;
                            }
                            else
                            {
                                itemDisplayBuilder.Append("&ts=" + Current_Mode.Size_Of_Thumbnails);
                            }
                        }

                        //Add the number, size of thumbnails and autonumbering mode if this is the QUALITY CONTROL (QC) View
                        if (( !String.IsNullOrEmpty(Current_Mode.ViewerCode)) && (Current_Mode.ViewerCode.IndexOf("qc") >= 0) && (Current_Mode.Thumbnails_Per_Page >= -1))
                        {
                            if (!query_string_started)
                            {
                                itemDisplayBuilder.Append("?nt=" + Current_Mode.Thumbnails_Per_Page);
                                query_string_started = true;
                            }
                            else
                            {
                                itemDisplayBuilder.Append("&nt=" + Current_Mode.Thumbnails_Per_Page);
                            }
                        }

                        if (( !String.IsNullOrEmpty(Current_Mode.ViewerCode)) && (Current_Mode.ViewerCode.IndexOf("qc") >= 0) && (Current_Mode.Size_Of_Thumbnails > 0))
                        {
                            if (!query_string_started)
                            {
                                itemDisplayBuilder.Append("?ts=" + Current_Mode.Size_Of_Thumbnails);
                                query_string_started = true;
                            }
                            else
                            {
                                itemDisplayBuilder.Append("&ts=" + Current_Mode.Size_Of_Thumbnails);
                            }
                        }

                        // Add the page by file information, if there is no viewer code
                        if ((String.IsNullOrEmpty(Item_View_Code)) && (!String.IsNullOrEmpty(Current_Mode.Page_By_FileName)))
                        {
                            if (!query_string_started)
                            {
                                itemDisplayBuilder.Append("?file=" + Current_Mode.Page_By_FileName);
                            }
                            else
                            {
                                itemDisplayBuilder.Append("&file=" + Current_Mode.Page_By_FileName);
                            }
                        }


                        string returnValue = itemDisplayBuilder.ToString();
                        return (returnValue.IndexOf("?") > 0) ? returnValue + urlOptions2 : returnValue + urlOptions1;
                    }
                    break;

                case Display_Mode_Enum.Search:
                    if ((!String.IsNullOrEmpty(adjusted_aggregation)) && ((String.IsNullOrEmpty(Current_Mode.Default_Aggregation)) || (adjusted_aggregation != Current_Mode.Default_Aggregation)))
                    {
                        switch (Current_Mode.Search_Type)
                        {
                            case Search_Type_Enum.Advanced:
                                return this_base_url + adjusted_aggregation + "/advanced" + urlOptions1;
                            case Search_Type_Enum.Map:
                                if (( !String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode )) && ( Current_Mode.Info_Browse_Mode == "1"))
                                    return this_base_url + adjusted_aggregation + "/map/1" + urlOptions1;
                                return this_base_url + adjusted_aggregation + "/map" + urlOptions1;
                            case Search_Type_Enum.Full_Text:
                            case Search_Type_Enum.dLOC_Full_Text:
                                return this_base_url + adjusted_aggregation + "/text" + urlOptions1;
                            default:
                                return this_base_url + adjusted_aggregation;
                        }
                    }
                    switch (Current_Mode.Search_Type)
                    {
                        case Search_Type_Enum.Advanced:
                            return this_base_url + "advanced" + urlOptions1;
                        case Search_Type_Enum.Map:
                            return this_base_url + "map" + urlOptions1;
                        case Search_Type_Enum.Full_Text:
                        case Search_Type_Enum.dLOC_Full_Text:
                            return this_base_url + "text" + urlOptions1;
                        default:
                            return this_base_url + urlOptions1;
                    }


                case Display_Mode_Enum.Results:
                    StringBuilder results_url_builder = new StringBuilder(this_base_url);
                    if ((!String.IsNullOrEmpty(adjusted_aggregation)) && ((String.IsNullOrEmpty(Current_Mode.Default_Aggregation)) || (adjusted_aggregation != Current_Mode.Default_Aggregation)))
                    {
                        results_url_builder.Append(adjusted_aggregation + "/");
                    }
                    switch (Current_Mode.Search_Precision)
                    {
                        case Search_Precision_Type_Enum.Contains:
                            results_url_builder.Append("contains/");
                            break;

                        case Search_Precision_Type_Enum.Synonmic_Form:
                            results_url_builder.Append("resultslike/");
                            break;

                        case Search_Precision_Type_Enum.Inflectional_Form:
                            results_url_builder.Append("results/");
                            break;

                        case Search_Precision_Type_Enum.Exact_Match:
                            results_url_builder.Append("exact/");
                            break;
                    }

                    // Add the results display type into the search results URL
                    switch (Current_Mode.Result_Display_Type)
                    {
                        case Result_Display_Type_Enum.Brief:
                            results_url_builder.Append("brief/");
                            break;
                        case Result_Display_Type_Enum.Export:
                            results_url_builder.Append("export/");
                            break;
                        case Result_Display_Type_Enum.Full_Citation:
                            results_url_builder.Append("citation/");
                            break;
                        case Result_Display_Type_Enum.Full_Image:
                            results_url_builder.Append("image/");
                            break;
                        case Result_Display_Type_Enum.Map:
                            results_url_builder.Append("map/");
                            break;
                        case Result_Display_Type_Enum.Map_Beta:
                            results_url_builder.Append("mapbeta/");
                            break;
                        case Result_Display_Type_Enum.Table:
                            results_url_builder.Append("table/");
                            break;
                        case Result_Display_Type_Enum.Thumbnails:
                            results_url_builder.Append("thumbs/");
                            break;
                    }
                    // Add the page into the search results URL
                    if ((Current_Mode.Page.HasValue ) && ( Current_Mode.Page.Value > 1))
                    {
                        results_url_builder.Append(Current_Mode.Page.ToString() + "/");
                    }

                    bool queryStringBegun = false;

                    // Add the search terms onto the search results URL
                    if (( !String.IsNullOrEmpty(Current_Mode.Search_String)) || (!String.IsNullOrEmpty(Current_Mode.Search_Fields)))
                    {
                        if ((Current_Mode.Search_Type == Search_Type_Enum.Basic) && (Current_Mode.Search_String.Length > 0))
                        {
                            results_url_builder.Append("?t=" + HttpUtility.UrlEncode(Current_Mode.Search_String).Replace("%2c", ","));
                            queryStringBegun = true;
                        }

                        if (Current_Mode.Search_Type == Search_Type_Enum.Advanced)
                        {
                            if (Current_Mode.Search_String.Length > 0)
                            {
                                results_url_builder.Append("?t=" + HttpUtility.UrlEncode(Current_Mode.Search_String).Replace("%2c", ",") + "&f=" + Current_Mode.Search_Fields);
                            }
                            else
                            {
                                results_url_builder.Append("?f=" + Current_Mode.Search_Fields);
                            }
                            queryStringBegun = true;
                        }

                        if (Current_Mode.Search_Type == Search_Type_Enum.Full_Text)
                        {
                            results_url_builder.Append("?text=" + HttpUtility.UrlEncode(Current_Mode.Search_String).Replace("%2c", ","));
                            queryStringBegun = true;
                        }

                        if ((Current_Mode.Search_Type == Search_Type_Enum.Map) && ( !String.IsNullOrEmpty(Current_Mode.Coordinates)))
                        {
                            results_url_builder.Append("?coord=" + HttpUtility.UrlEncode(Current_Mode.Coordinates).Replace("%2c", ","));
                            queryStringBegun = true;
                        }

                        // Add the sort order
                        if (Current_Mode.Sort > 0)
                        {
                            results_url_builder.Append("&o=" + Current_Mode.Sort);
                        }
                    }

                    // Add the year or date values
                    if (Current_Mode.DateRange_Date1 >= 0)
                    {
                        if (!queryStringBegun)
                        {
                            results_url_builder.Append("?dt1=" + Current_Mode.DateRange_Date1);
                            //queryStringBegun = true;
                        }
                        else
                        {
                            results_url_builder.Append("&dt1=" + Current_Mode.DateRange_Date1);
                        }
                        if (Current_Mode.DateRange_Date2 >= 0)
                        {
                            results_url_builder.Append("&dt2=" + Current_Mode.DateRange_Date2);
                        }
                    }
                    else if (Current_Mode.DateRange_Year1 >= 0)
                    {
                        if (!queryStringBegun)
                        {
                            results_url_builder.Append("?yr1=" + Current_Mode.DateRange_Year1);
                            //queryStringBegun = true;
                        }
                        else
                        {
                            results_url_builder.Append("&yr1=" + Current_Mode.DateRange_Year1);
                        }
                        if (Current_Mode.DateRange_Year2 >= 0)
                        {
                            results_url_builder.Append("&yr2=" + Current_Mode.DateRange_Year2);
                        }
                    }

                    string returnValue2 = results_url_builder.ToString();
                    if (returnValue2.IndexOf("?") > 0)
                        return returnValue2 + urlOptions2;
                    return returnValue2 + urlOptions1;


                case Display_Mode_Enum.Aggregation:
                    switch (Current_Mode.Aggregation_Type)
                    {
                        case Aggregation_Type_Enum.Home:
                            if ((!String.IsNullOrEmpty(adjusted_aggregation)) && ((String.IsNullOrEmpty(Current_Mode.Default_Aggregation)) || (adjusted_aggregation != Current_Mode.Default_Aggregation)))
                                return this_base_url + adjusted_aggregation + urlOptions1;
                            switch (Current_Mode.Home_Type)
                            {
                                case Home_Type_Enum.Descriptions:
                                    return this_base_url + "brief" + urlOptions1;

                                case Home_Type_Enum.Tree:
                                    return this_base_url + "tree" + urlOptions1;

                                case Home_Type_Enum.Partners_List:
                                    return this_base_url + "partners" + urlOptions1;

                                case Home_Type_Enum.Partners_Thumbnails:
                                    return this_base_url + "partners/thumbs" + urlOptions1;

                                case Home_Type_Enum.Personalized:
                                    return this_base_url + "personalized" + urlOptions1;

                                default:
                                    return this_base_url + urlOptions1;
                            }

                        case Aggregation_Type_Enum.Home_Edit:
                            if ((!String.IsNullOrEmpty(adjusted_aggregation)) && (adjusted_aggregation != "all"))
                            {
                                return this_base_url + adjusted_aggregation + "/edit" + urlOptions1;
                            }
                            return this_base_url + "edit" + urlOptions1;

                        case Aggregation_Type_Enum.Browse_By:
                            string browse_by_mode = String.Empty;
                            if (!String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                                browse_by_mode = Current_Mode.Info_Browse_Mode.Replace(" ", "_");
                            if ((!String.IsNullOrEmpty(adjusted_aggregation)) && ((String.IsNullOrEmpty(Current_Mode.Default_Aggregation)) || (adjusted_aggregation != Current_Mode.Default_Aggregation)))
                            {
                                if ((Current_Mode.Page > 1) && (browse_by_mode.Length > 0))
                                {
                                    return this_base_url + adjusted_aggregation + "/browseby/" + browse_by_mode + "/" + Current_Mode.Page + urlOptions1;
                                }
                                return this_base_url + adjusted_aggregation + "/browseby/" + browse_by_mode + urlOptions1;
                            }
                            if ((Current_Mode.Page > 1) && (browse_by_mode.Length > 0))
                            {
                                return this_base_url + "browseby/" + browse_by_mode + "/" + Current_Mode.Page + urlOptions1;
                            }
                            return this_base_url + "browseby/" + browse_by_mode + urlOptions1;

                        case Aggregation_Type_Enum.Browse_Map:
                            if (String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                            {
                                if ((!String.IsNullOrEmpty(adjusted_aggregation)) && ((String.IsNullOrEmpty(Current_Mode.Default_Aggregation)) || (adjusted_aggregation != Current_Mode.Default_Aggregation)))
                                {
                                    return this_base_url + adjusted_aggregation + "/geography" + urlOptions1;
                                }
                                return this_base_url + "geography" + urlOptions1;
                            }
                            else
                            {
                                if ((!String.IsNullOrEmpty(adjusted_aggregation)) && ((String.IsNullOrEmpty(Current_Mode.Default_Aggregation)) || (adjusted_aggregation != Current_Mode.Default_Aggregation)))
                                {
                                    return this_base_url + adjusted_aggregation + "/geography/" + Current_Mode.Info_Browse_Mode + urlOptions1;
                                }
                                return this_base_url + "geography/" + Current_Mode.Info_Browse_Mode + urlOptions1;
                            }

                        case Aggregation_Type_Enum.Manage_Menu:
                            if ((!String.IsNullOrEmpty(adjusted_aggregation)) && (adjusted_aggregation != "all"))
                            {
                                return this_base_url + adjusted_aggregation + "/manage" + urlOptions1;
                            }
                            return this_base_url + "aggrmanage" + urlOptions1;

                        case Aggregation_Type_Enum.Work_History:
                            if ((!String.IsNullOrEmpty(adjusted_aggregation)) && (adjusted_aggregation != "all"))
                            {
                                return this_base_url + adjusted_aggregation + "/history" + urlOptions1;
                            }
                            return this_base_url + "aggrhistory" + urlOptions1;

                        case Aggregation_Type_Enum.User_Permissions:
                            if ((!String.IsNullOrEmpty(adjusted_aggregation)) && (adjusted_aggregation != "all"))
                            {
                                return this_base_url + adjusted_aggregation + "/permissions" + urlOptions1;
                            }
                            return this_base_url + "aggrpermissions" + urlOptions1;

                        case Aggregation_Type_Enum.Item_Count:
                            if (String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                            {
                                if (!String.IsNullOrEmpty(adjusted_aggregation))
                                {
                                    return this_base_url + adjusted_aggregation + "/itemcount" + urlOptions1;
                                }
                                return this_base_url + "itemcount" + urlOptions1;
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(adjusted_aggregation))
                                {
                                    return this_base_url + adjusted_aggregation + "/itemcount/" + Current_Mode.Info_Browse_Mode + urlOptions1;
                                }
                                return this_base_url + "itemcount/" + Current_Mode.Info_Browse_Mode + urlOptions1;
                            }

                        case Aggregation_Type_Enum.Usage_Statistics:
                            if (String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                            {
                                if (!String.IsNullOrEmpty(adjusted_aggregation))
                                {
                                    return this_base_url + adjusted_aggregation + "/usage" + urlOptions1;
                                }
                                return this_base_url + "usage" + urlOptions1;
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(adjusted_aggregation))
                                {
                                    return this_base_url + adjusted_aggregation + "/usage/" + Current_Mode.Info_Browse_Mode + urlOptions1;
                                }
                                return this_base_url + "usage/" + Current_Mode.Info_Browse_Mode + urlOptions1; 
                            }

                        case Aggregation_Type_Enum.Private_Items:
                            if (Current_Mode.Page > 1)
                            {
                                if (!String.IsNullOrEmpty(adjusted_aggregation))
                                {
                                    if ((!Current_Mode.Sort.HasValue) || (Current_Mode.Sort == 0))
                                        return this_base_url + adjusted_aggregation + "/inprocess/" + Current_Mode.Page + urlOptions1;
                                    return this_base_url + adjusted_aggregation + "/inprocess/" + Current_Mode.Page + "?o=" + Current_Mode.Sort + urlOptions2;
                                }
                                if ((!Current_Mode.Sort.HasValue) || (Current_Mode.Sort == 0))
                                    return this_base_url + "inprocess/" + Current_Mode.Page + urlOptions1;
                                return this_base_url + "inprocess/" + Current_Mode.Page + "?o=" + Current_Mode.Sort + urlOptions2;
                            }
                            if (!String.IsNullOrEmpty(adjusted_aggregation))
                            {
                                if ((!Current_Mode.Sort.HasValue) || (Current_Mode.Sort == 0))
                                    return this_base_url + adjusted_aggregation + "/inprocess" + urlOptions1;
                                return this_base_url + adjusted_aggregation + "/inprocess?o=" + Current_Mode.Sort + urlOptions2;
                            }
                            if ((!Current_Mode.Sort.HasValue) || (Current_Mode.Sort == 0))
                                return this_base_url + "inprocess" + urlOptions1;
                            return this_base_url + "inprocess?o=" + Current_Mode.Sort + urlOptions2;

                        case Aggregation_Type_Enum.Browse_Info:
                            if (Current_Mode.Sort > 0)
                            {
                                if (url_options.Length > 0)
                                {
                                    urlOptions1 = "?" + url_options + "&o=" + Current_Mode.Sort;
                                }
                                else
                                {
                                    urlOptions1 = "?o=" + Current_Mode.Sort;
                                }
                            }

                            // If somehow the info browse code is NULL or EMPTY, just go to the aggregation again
                            if ( String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                                return this_base_url + adjusted_aggregation + urlOptions1;

                            if ((!String.IsNullOrEmpty(adjusted_aggregation)) && ((String.IsNullOrEmpty(Current_Mode.Default_Aggregation)) || (adjusted_aggregation != Current_Mode.Default_Aggregation)))
                            {
                                if (Current_Mode.Page > 1)
                                {
                                    switch (Current_Mode.Result_Display_Type)
                                    {
                                        case Result_Display_Type_Enum.Brief:
                                            return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/brief/" + Current_Mode.Page + urlOptions1;
                                        case Result_Display_Type_Enum.Export:
                                            return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/export/" + Current_Mode.Page + urlOptions1;
                                        case Result_Display_Type_Enum.Full_Citation:
                                            return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/citation/" + Current_Mode.Page + urlOptions1;
                                        case Result_Display_Type_Enum.Full_Image:
                                            return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/image/" + Current_Mode.Page + urlOptions1;
                                        case Result_Display_Type_Enum.Map:
                                            return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/map/" + Current_Mode.Page + urlOptions1;
                                        case Result_Display_Type_Enum.Map_Beta:
                                            return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/mapbeta/" + Current_Mode.Page + urlOptions1;
                                        case Result_Display_Type_Enum.Table:
                                            return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/table/" + Current_Mode.Page + urlOptions1;
                                        case Result_Display_Type_Enum.Thumbnails:
                                            return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/thumbs/" + Current_Mode.Page + urlOptions1;
                                        default:
                                            return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/" + Current_Mode.Page + urlOptions1;
                                    }
                                }
                                switch (Current_Mode.Result_Display_Type)
                                {
                                    case Result_Display_Type_Enum.Brief:
                                        return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/brief" + urlOptions1;
                                    case Result_Display_Type_Enum.Export:
                                        return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/export" + urlOptions1;
                                    case Result_Display_Type_Enum.Full_Citation:
                                        return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/citation" + urlOptions1;
                                    case Result_Display_Type_Enum.Full_Image:
                                        return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/image" + urlOptions1;
                                    case Result_Display_Type_Enum.Map:
                                        return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/map" + urlOptions1;
                                    case Result_Display_Type_Enum.Map_Beta:
                                        return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/mapbeta" + urlOptions1;
                                    case Result_Display_Type_Enum.Table:
                                        return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/table" + urlOptions1;
                                    case Result_Display_Type_Enum.Thumbnails:
                                        return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/thumbs" + urlOptions1;
                                    default:
                                        return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + urlOptions1;
                                }
                            }
                            // See if you need to include 'info' here
                            string pre_mode_string = "info/";
                            if ((Current_Mode.Info_Browse_Mode == "all") || (Current_Mode.Info_Browse_Mode == "new"))
                                pre_mode_string = String.Empty;
                            if (Current_Mode.Page > 1)
                            {
                                switch (Current_Mode.Result_Display_Type)
                                {
                                    case Result_Display_Type_Enum.Brief:
                                        return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/brief/" + Current_Mode.Page + urlOptions1;
                                    case Result_Display_Type_Enum.Export:
                                        return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/export/" + Current_Mode.Page + urlOptions1;
                                    case Result_Display_Type_Enum.Full_Citation:
                                        return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/citation/" + Current_Mode.Page + urlOptions1;
                                    case Result_Display_Type_Enum.Full_Image:
                                        return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/image/" + Current_Mode.Page + urlOptions1;
                                    case Result_Display_Type_Enum.Map:
                                        return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/map/" + Current_Mode.Page + urlOptions1;
                                    case Result_Display_Type_Enum.Map_Beta:
                                        return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/mapsearch/" + Current_Mode.Page + urlOptions1;
                                    case Result_Display_Type_Enum.Table:
                                        return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/table/" + Current_Mode.Page + urlOptions1;
                                    case Result_Display_Type_Enum.Thumbnails:
                                        return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/thumbs/" + Current_Mode.Page + urlOptions1;
                                    default:
                                        return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/" + Current_Mode.Page + urlOptions1;
                                }
                            }
                            switch (Current_Mode.Result_Display_Type)
                            {
                                case Result_Display_Type_Enum.Brief:
                                    return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/brief" + urlOptions1;
                                case Result_Display_Type_Enum.Export:
                                    return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/export" + urlOptions1;
                                case Result_Display_Type_Enum.Full_Citation:
                                    return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/citation" + urlOptions1;
                                case Result_Display_Type_Enum.Full_Image:
                                    return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/image" + urlOptions1;
                                case Result_Display_Type_Enum.Map:
                                    return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/map" + urlOptions1;
                                case Result_Display_Type_Enum.Map_Beta:
                                    return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/mapsearch" + urlOptions1;
                                case Result_Display_Type_Enum.Table:
                                    return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/table" + urlOptions1;
                                case Result_Display_Type_Enum.Thumbnails:
                                    return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + "/thumbs" + urlOptions1;
                                default:
                                    return this_base_url + pre_mode_string + Current_Mode.Info_Browse_Mode + urlOptions1;
                            }

                        case Aggregation_Type_Enum.Child_Page_Edit:
                            // If somehow the info browse code is NULL or EMPTY, just go to the aggregation again
                            if (String.IsNullOrEmpty(Current_Mode.Info_Browse_Mode))
                                return this_base_url + adjusted_aggregation + urlOptions1;

                            if ((!String.IsNullOrEmpty(adjusted_aggregation)) && (( String.IsNullOrEmpty(Current_Mode.Default_Aggregation)) || (adjusted_aggregation != Current_Mode.Default_Aggregation)))
                            {
                                return this_base_url + adjusted_aggregation + "/" + Current_Mode.Info_Browse_Mode + "/edit" + urlOptions1;
                            }
                            // See if you need to include 'info' here
                            string pre_mode_string2 = "info/";
                            if ((Current_Mode.Info_Browse_Mode == "all") || (Current_Mode.Info_Browse_Mode == "new"))
                                pre_mode_string2 = String.Empty;
                            return this_base_url + pre_mode_string2 + Current_Mode.Info_Browse_Mode + "/edit" + urlOptions1;
                    }
                    break;


            }

            return this_base_url + "unknown";
        }

        private static string create_basic_url(string this_base_url, string main_codes, string[] Remaining_Url_Segments, string urlOptions1)
        {
            if ((Remaining_Url_Segments == null) || (Remaining_Url_Segments.Length == 0))
            {
                return this_base_url + main_codes + urlOptions1;
            }
            if (Remaining_Url_Segments.Length == 1)
                return this_base_url + main_codes + "/" + Remaining_Url_Segments[0] + urlOptions1;
            if (Remaining_Url_Segments.Length == 2)
                return this_base_url + main_codes + "/" + Remaining_Url_Segments[0] + "/" + Remaining_Url_Segments[1] + urlOptions1;
            if (Remaining_Url_Segments.Length == 3)
                return this_base_url + main_codes + "/" + Remaining_Url_Segments[0] + "/" + Remaining_Url_Segments[1] + "/" + Remaining_Url_Segments[2] + urlOptions1;
            if (Remaining_Url_Segments.Length == 4)
                return this_base_url + main_codes + "/" + Remaining_Url_Segments[0] + "/" + Remaining_Url_Segments[1] + "/" + Remaining_Url_Segments[2] + "/" + Remaining_Url_Segments[3] + urlOptions1;
            if (Remaining_Url_Segments.Length == 5)
                return this_base_url + main_codes + "/" + Remaining_Url_Segments[0] + "/" + Remaining_Url_Segments[1] + "/" + Remaining_Url_Segments[2] + "/" + Remaining_Url_Segments[3] + "/" + Remaining_Url_Segments[4] + urlOptions1;
            if (Remaining_Url_Segments.Length == 6)
                return this_base_url + main_codes + "/" + Remaining_Url_Segments[0] + "/" + Remaining_Url_Segments[1] + "/" + Remaining_Url_Segments[2] + "/" + Remaining_Url_Segments[3] + "/" + Remaining_Url_Segments[4] + "/" + Remaining_Url_Segments[5] + urlOptions1;
            if (Remaining_Url_Segments.Length == 7)
                return this_base_url + main_codes + "/" + Remaining_Url_Segments[0] + "/" + Remaining_Url_Segments[1] + "/" + Remaining_Url_Segments[2] + "/" + Remaining_Url_Segments[3] + "/" + Remaining_Url_Segments[4] + "/" + Remaining_Url_Segments[5] + "/" + Remaining_Url_Segments[6] + urlOptions1;

            StringBuilder returnUrl = new StringBuilder(this_base_url + main_codes + "/" + Remaining_Url_Segments[0] + "/" + Remaining_Url_Segments[1] + "/" + Remaining_Url_Segments[2] + "/" + Remaining_Url_Segments[3] + "/" + Remaining_Url_Segments[4] + "/" + Remaining_Url_Segments[5] + "/" + Remaining_Url_Segments[6]);
            for (int i = 7; i < Remaining_Url_Segments.Length; i++)
                returnUrl.Append("/" + Remaining_Url_Segments[i]);
            returnUrl.Append(urlOptions1);
            return returnUrl.ToString();
        }


        /// <summary> Returns the URL options the user has currently set </summary>
        /// <returns>URL options</returns>
        public static string URL_Options(Navigation_Object Current_Mode)
        {
            // If this is a robot, we do not allow them to set anything through the query string
            if (Current_Mode.Is_Robot)
                return String.Empty;

            // Define the StringBuilder
            StringBuilder redirect = new StringBuilder();

            if (Current_Mode.Trace_Flag == Trace_Flag_Type_Enum.Explicit)
                redirect.Append("trace=yes");

            // Was there an interface?
            if (!string.IsNullOrEmpty(Current_Mode.Skin) && (String.Compare(Current_Mode.Skin, Current_Mode.Default_Skin, StringComparison.OrdinalIgnoreCase) != 0))
            {
                if (redirect.Length > 0)
                    redirect.Append("&");
                redirect.Append("n=" + Current_Mode.Skin.ToLower());
            }

            // Add language if it is not the browser default
            if ((Current_Mode.Language != Current_Mode.Default_Language) && (Current_Mode.Language != Web_Language_Enum.DEFAULT))
            {
                if (Current_Mode.Language == Web_Language_Enum.TEMPLATE)
                {
                    if (redirect.Length > 0)
                        redirect.Append("&");
                    redirect.Append("l=XXXXX");
                }
                else
                {
                    if (redirect.Length > 0)
                        redirect.Append("&");
                    redirect.Append("l=" + Web_Language_Enum_Converter.Enum_To_Code(Current_Mode.Language).ToLower());
                }
            }

            // Return the built string
            return redirect.ToString();
        }

        #endregion

        /// <summary> Returns the URL to redirect the user's browser, based on the current
        /// mode and specifics for this mode. </summary>
        /// <returns> String to be attached to the end of the main application name to redirect
        /// the current user's browser.  </returns>
        public static string Redirect_URL(Navigation_Object Current_Mode)
        {
            return Redirect_URL(Current_Mode, Current_Mode.ViewerCode, true);
        }

        /// <summary> Returns the URL to redirect the user's browser, based on the current
        /// mode and specifics for this mode. </summary>
        /// <param name="Current_Mode"> Current navigation object which contains the information </param>
        /// <param name="Include_URL_Opts"> Flag indicates whether to include URL opts or not </param>
        /// <returns> String to be attached to the end of the main application name to redirect
        /// the current user's browser.  </returns>
        public static string Redirect_URL(Navigation_Object Current_Mode, bool Include_URL_Opts)
        {
            return Redirect_URL(Current_Mode, Current_Mode.ViewerCode, Include_URL_Opts);
        }

        /// <summary> Returns the URL to redirect the user's browser, based on the current
        /// mode and specifics for this mode. </summary>
        /// <param name="Current_Mode"> Current navigation object which contains the information </param>
        /// <param name="Item_View_Code">Item view code to display</param>
        /// <returns> String to be attached to the end of the main application name to redirect
        /// the current user's browser.  </returns>
        public static string Redirect_URL(Navigation_Object Current_Mode, string Item_View_Code)
        {
            return Redirect_URL(Current_Mode, Item_View_Code, true);
        }

        /// <summary> Redirect the user to the current mode's URL </summary>
        /// <remarks> This does not stop execution immediately (which would raise a ThreadAbortedException
        /// and be costly in terms of performance) but it does set the 
        /// Request_Completed flag, which should be checked and will effectively stop any 
        /// further actions. </remarks>
        public static void Redirect(Navigation_Object Current_Mode)
        {
            Current_Mode.Request_Completed = true;
            HttpContext.Current.Response.Redirect(Redirect_URL(Current_Mode), false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();

        }

        /// <summary> Redirect the user to the current mode's URL </summary>
        /// <param name="Current_Mode"> Current navigation object which contains the information  </param>
        /// <param name="Flush_Response"> Flag indicates if the response should be flushed</param>
        /// <remarks> This does not stop execution immediately (which would raise a ThreadAbortedException
        /// and be costly in terms of performance) but it does set the 
        /// Request_Completed flag, which should be checked and will effectively stop any 
        /// further actions. </remarks>
        public static void Redirect(Navigation_Object Current_Mode, bool Flush_Response)
        {
            if (Flush_Response)
                HttpContext.Current.Response.Flush();
            Current_Mode.Request_Completed = true;
            HttpContext.Current.Response.Redirect(Redirect_URL(Current_Mode), false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}
