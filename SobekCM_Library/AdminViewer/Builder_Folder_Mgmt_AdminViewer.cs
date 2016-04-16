using System;
using System.IO;
using System.Linq;
using System.Web;
using SobekCM.Core.Builder;
using SobekCM.Core.Client;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Administrative screen allows an existing incoming builder folder to be
    /// edited or a new builder folder to be added </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class</remarks>
    public class Builder_Folder_Mgmt_AdminViewer : abstract_AdminViewer
    {
        private readonly Builder_Source_Folder sourceFolder;

        /// <summary> Constructor for a new instance of the Builder_Folder_Mgmt_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Builder_Folder_Mgmt_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            // Verify this user can edit this
            bool allowEdit = (((!UI_ApplicationCache_Gateway.Settings.Servers.isHosted) && (RequestSpecificValues.Current_User.Is_System_Admin)) || (RequestSpecificValues.Current_User.Is_Host_Admin));
            if (!allowEdit)
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            sourceFolder = new Builder_Source_Folder();

            // Is there a folder specified?
            if ((RequestSpecificValues.Current_Mode.Remaining_Url_Segments != null) && (RequestSpecificValues.Current_Mode.Remaining_Url_Segments.Length > 0))
            {
                if (String.Compare(RequestSpecificValues.Current_Mode.Remaining_Url_Segments[0], "new", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    int folderid;
                    if (Int32.TryParse(RequestSpecificValues.Current_Mode.Remaining_Url_Segments[0], out folderid))
                    {
                        sourceFolder = SobekEngineClient.Builder.Get_Builder_Folder(folderid, RequestSpecificValues.Tracer);
                    }
                }
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'SobekCM Builder Folder Management' </value>
        public override string Web_Title
        {
            get { return "SobekCM Builder Folder Management"; }
        }

        /// <summary> Gets the URL for the icon related to this administrative task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources_Gateway.Gears_Img; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Builder_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("<div class=\"sbkAdm_HomeText\">");

            const string NAME_HELP = "Enter a descriptive label for this incoming folder.\\n\\nThis will only be used to describe the source of the material in any logs.";
            const string INBOUND_HELP = "Network location where new incoming packages will be written for bulk loading.   \\n\\nThis should be on the same directory system as the inbound and error folders, and should not be a subdirectory of either.";
            const string PROCESSING_HELP = "Network location where packages which are currently being processed will be moved from the inbound network location.   \\n\\nThis should be on the same directory system as the inbound and error folders, and should not be a subdirectory of either.";
            const string FAILURES_HELP = "Network location where any packages which fail validation or fail to load will be moved.   \\n\\nThis should be on the same directory system as the inbound and processing folders, and should not be a subdirectory of either.";
            const string CHECKSUM_HELP = "Flag indicates that if an incoming METS file has checksums, the files will be validated against those checksums.";
            const string ARCHIVE_TIFF_HELP = "Flag indicates that incoming digital resource TIFF files will be copied to the Archival Drop Box directory, if such a value has been set in the main system settings.\\n\\nThis setting has no meaning if the next flag (for archiving ALL files) has been set to TRUE.";
            const string ARCHIVE_ALL_HELP = "Flag indicates that ALL incoming digital resource files will be copied to the Archival Drop Box directory, if such a value has been set in the main system settings.";
            const string DELETE_HELP = "Flag indicates if incoming METS files with a record status of DELETE should be accepted and result in the deletion of the corresponding item from this system.";
            const string NO_METADATA_HELP = "Flag indicates that all incoming digital resources MUST be accompanied by a METS file, or they will be rejected.";
            const string BIB_RESTRICT_HELP = "Flag indicates that incoming packages are restricted by the BibID root.  Packages that begin with a different root are rejected.  Multiple roots are added with a &quot;pipe&quot; between them, such as &quot;UF01|CA|SMI&quot;.";

            Output.WriteLine("<p>Below is the information for an existing builder incoming folder.  Use this form to make any changes to the folder, such as the network folders or any of the folder options.</p>");

            Output.WriteLine("<table id=\"sbkBfmav_DisplayTable\">");

            // Add line for descriptive name
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td id=\"sbkBfmav_DisplayTable_LabelCol\"><label for=\"admin_folder_name\">Descriptive Name:</label></td>");
            Output.WriteLine("    <td id=\"sbkBfmav_DisplayTable_EntryCol\"><input class=\"sbkBfmav_DescNameBox sbkAdmin_Focusable\" name=\"admin_folder_name\" id=\"admin_folder_name\" type=\"text\" value=\"" + (sourceFolder.Folder_Name ?? String.Empty) + "\" /></td>");
            Output.WriteLine("    <td colspan=\"2\"><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + NAME_HELP + "');\"  title=\"" + NAME_HELP + "\" /></td>");
            Output.WriteLine("  </tr>");

            // Add the network folder
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td><label for=\"admin_folder_network\">Inbound Folder:</label></td>");
            Output.WriteLine("    <td colspan=\"2\"><input class=\"sbkBfmav_NetworkInput sbkAdmin_Focusable\" name=\"admin_folder_network\" id=\"admin_folder_network\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(sourceFolder.Inbound_Folder ?? String.Empty) + "\" /></td>");
            Output.WriteLine("    <td id=\"sbkBfmav_DisplayTable_LastCol\"><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + INBOUND_HELP + "');\"  title=\"" + INBOUND_HELP + "\" /></td>");
            Output.WriteLine("  </tr>");

            // Add the processing folder
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td><label for=\"admin_folder_processing\">Processing Folder:</label></td>");
            Output.WriteLine("    <td colspan=\"2\"><input class=\"sbkBfmav_NetworkInput sbkAdmin_Focusable\" name=\"admin_folder_processing\" id=\"admin_folder_processing\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(sourceFolder.Processing_Folder ?? String.Empty) + "\" /></td>");
            Output.WriteLine("    <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + PROCESSING_HELP + "');\"  title=\"" + PROCESSING_HELP + "\" /></td>");
            Output.WriteLine("  </tr>");

            // Add the error folder
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td><label for=\"admin_folder_error\">Failures Folder:</label></td>");
            Output.WriteLine("    <td colspan=\"2\"><input class=\"sbkBfmav_NetworkInput sbkAdmin_Focusable\" name=\"admin_folder_error\" id=\"admin_folder_error\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(sourceFolder.Failures_Folder ?? String.Empty) + "\" /></td>");
            Output.WriteLine("    <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + FAILURES_HELP + "');\"  title=\"" + FAILURES_HELP + "\" /></td>");
            Output.WriteLine("  </tr>");

            // Add the folder options title and first folder option ( checksum validation )
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td>Folder Options:</td>");
            Output.WriteLine("    <td colspan=\"3\" style=\"vertical-align: middle\">");
            Output.WriteLine("      <input class=\"sbkBfmav_FolderOptionCheckbox sbkAdmin_Focusable\" name=\"admin_folder_checksum\" id=\"admin_folder_checksum\" type=\"checkbox\" " + (sourceFolder.Perform_Checksum ? "checked=\"checked\" " : String.Empty) + " />");
            Output.WriteLine("      <label for=\"admin_folder_checksum\">Perform checksum validation against any checksums in the METS files</label>");
            Output.WriteLine("      <img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + CHECKSUM_HELP + "');\"  title=\"" + CHECKSUM_HELP + "\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add the copy TIFFs to archive directory folder option
            Output.WriteLine("  <tr  id=\"sbkBfmav_ArchiveTiffRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td colspan=\"3\" style=\"vertical-align: middle\">");
            Output.WriteLine("      <input class=\"sbkBfmav_FolderOptionCheckbox sbkAdmin_Focusable\" name=\"admin_folder_archive_tiff\" id=\"admin_folder_archive_tiff\" type=\"checkbox\" " + (sourceFolder.Archive_TIFFs ? "checked=\"checked\" " : String.Empty) + " />");
            Output.WriteLine("      <label for=\"admin_folder_archive_tiff\">Copy all TIFF files to the archiving directory</label>");
            Output.WriteLine("      <img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + ARCHIVE_TIFF_HELP + "');\"  title=\"" + ARCHIVE_TIFF_HELP + "\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add the copy all incoming files to archive directory folder option
            Output.WriteLine("  <tr id=\"sbkBfmav_ArchiveAllRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td colspan=\"3\" style=\"vertical-align: middle\">");
            Output.WriteLine("      <input class=\"sbkBfmav_FolderOptionCheckbox sbkAdmin_Focusable\" name=\"admin_folder_archive_all\" id=\"admin_folder_archive_all\" type=\"checkbox\" " + (sourceFolder.Archive_All_Files ? "checked=\"checked\" " : String.Empty) + " />");
            Output.WriteLine("      <label for=\"admin_folder_archive_all\">Copy all incoming files to the archiving directory</label>");
            Output.WriteLine("      <img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + ARCHIVE_ALL_HELP + "');\"  title=\"" + ARCHIVE_ALL_HELP + "\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add the flag to allow DELETE mets files folder option
            Output.WriteLine("  <tr id=\"sbkBfmav_DeleteRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td colspan=\"3\" style=\"vertical-align: middle\">");
            Output.WriteLine("      <input class=\"sbkBfmav_FolderOptionCheckbox sbkAdmin_Focusable\" name=\"admin_folder_allow_delete\" id=\"admin_folder_allow_delete\" type=\"checkbox\" " + (sourceFolder.Allow_Deletes ? "checked=\"checked\" " : String.Empty) + " />");
            Output.WriteLine("      <label for=\"admin_folder_allow_delete\">Allow METS files with a RecordStatus of DELETE in this folder</label>");
            Output.WriteLine("      <img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + DELETE_HELP + "');\"  title=\"" + DELETE_HELP + "\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add the flag to allow resource files without METS files
            Output.WriteLine("  <tr id=\"sbkBfmav_NoMetsRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td colspan=\"3\" style=\"vertical-align: middle\">");
            Output.WriteLine("      <input class=\"sbkBfmav_FolderOptionCheckbox sbkAdmin_Focusable\" name=\"admin_folder_no_metadata\" id=\"admin_folder_no_metadata\" type=\"checkbox\" " + (sourceFolder.Allow_Folders_No_Metadata ? "checked=\"checked\" " : String.Empty) + " />");
            Output.WriteLine("      <label for=\"admin_folder_no_metadata\">Allow resource files in appropriately names folders without METS files</label>");
            Output.WriteLine("      <img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + NO_METADATA_HELP + "');\"  title=\"" + NO_METADATA_HELP + "\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add the flag to restrict to certain BibID roots
            Output.WriteLine("  <tr id=\"sbkBfmav_BibRestrictionsRow\" style=\"display:none;\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td colspan=\"3\" style=\"vertical-align: middle\">");
            Output.WriteLine("      <input class=\"sbkBfmav_FolderOptionCheckbox sbkAdmin_Focusable\" name=\"admin_folder_bibrestrict\" id=\"admin_folder_bibrestrict\" type=\"checkbox\" " + ((String.IsNullOrWhiteSpace(sourceFolder.BibID_Roots_Restrictions) ? String.Empty : "checked=\"checked\" ")) + " />");
            Output.WriteLine("      <label for=\"admin_folder_bibrestrict\">Restrict incoming packages to certain BibID roots.</label>");
            Output.WriteLine("      <img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + BIB_RESTRICT_HELP + "');\"  title=\"" + BIB_RESTRICT_HELP + "\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Determine button text
            string button_text = "ADD";
            string button_title = "Add this new builder incoming folder";
            if (sourceFolder.IncomingFolderID > 0)
            {
                button_text = "SAVE";
                button_title = "Save builder incoming folder changes";
            }


            // Add line for button
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td></td>");
            Output.WriteLine("    <td colspan=\"3\"><button title=\"" + button_title + "\" id=\"sbkBfmav_Button\" class=\"sbkAdm_RoundButton\" onclick=\"return save_new_child_page();\">" + button_text + "</button></td>");
            Output.WriteLine("  </tr>");

            // Add the SAVE button
            Output.WriteLine("</table>");

            Output.WriteLine("</div>");
        }
    }
}
