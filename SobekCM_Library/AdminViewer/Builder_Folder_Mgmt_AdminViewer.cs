using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using SobekCM.Core.Builder;
using SobekCM.Core.Client;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Administrative screen allows an existing incoming builder folder to be
    /// edited or a new builder folder to be added </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class</remarks>
    public class Builder_Folder_Mgmt_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;

        private readonly int folderId;
        private readonly string folderName;
        private readonly string failuresFolder;
        private readonly string inboundFolder;
        private readonly string processingFolder;
        private readonly bool performChecksum;
        private readonly bool archiveTiffs;
        private readonly bool archiveAllFiles;
        private readonly bool allowDeletes;
        private readonly bool allowFoldersNoMetadata;
        private readonly bool allowMetadataUpdates;
        private readonly string bibIdRestrictions;
        private readonly int moduleSetId;
        private readonly bool saved;


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

            // Is there a folder specified?
            folderId = -1;
            if ((RequestSpecificValues.Current_Mode.Remaining_Url_Segments != null) && (RequestSpecificValues.Current_Mode.Remaining_Url_Segments.Length > 0))
            {
                if (String.Compare(RequestSpecificValues.Current_Mode.Remaining_Url_Segments[0], "new", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    if (!Int32.TryParse(RequestSpecificValues.Current_Mode.Remaining_Url_Segments[0], out folderId))
                        folderId = -1;
                }
            }

            // Handle any post backs
            saved = false;
            if (RequestSpecificValues.Current_Mode.isPostBack)
            {
                try
                {
                    // Pull the standard values from the form
                    NameValueCollection form = HttpContext.Current.Request.Form;
                    string action_value = form["admin_builder_folder_action"];

                    // Get the entered values 
                    folderName = form["admin_folder_name"];
                    failuresFolder = form["admin_folder_error"];
                    inboundFolder = form["admin_folder_network"];
                    processingFolder = form["admin_folder_processing"];
                    performChecksum = (form["admin_folder_checksum"] != null);
                    archiveTiffs = (form["admin_folder_archive_tiff"] != null);
                    archiveAllFiles = (form["admin_folder_archive_all"] != null);
                    allowDeletes = (form["admin_folder_allow_delete"] != null);
                    allowFoldersNoMetadata = (form["admin_folder_no_metadata"] != null);
                    allowMetadataUpdates = (form["admin_folder_allow_updates"] != null);

                    // Get the hidden values
                    bibIdRestrictions = form["admin_builder_folder_restrictions"];
                    moduleSetId = Int32.Parse(form["admin_builder_folder_modulesetid"]);

                    // The folders should always end with a slash
                    if ((!String.IsNullOrWhiteSpace(failuresFolder)) && (failuresFolder.Length > 2) && (failuresFolder[failuresFolder.Length - 1] != '\\'))
                        failuresFolder = failuresFolder + "\\";
                    if ((!String.IsNullOrWhiteSpace(inboundFolder)) && (inboundFolder.Length > 2) && (inboundFolder[inboundFolder.Length - 1] != '\\'))
                        inboundFolder = inboundFolder + "\\";
                    if ((!String.IsNullOrWhiteSpace(processingFolder)) && (processingFolder.Length > 2) && (processingFolder[processingFolder.Length - 1] != '\\'))
                        processingFolder = processingFolder + "\\";
                    
                    // Switch, depending on the request
                    if (!String.IsNullOrEmpty(action_value))
                    {
                        // Was this a CANCEL?
                        if (action_value == "cancel")
                        {
                            // Determine URL
                            string returnUrl = RequestSpecificValues.Current_Mode.Base_URL + "l/admin/settings/builder/folders";
                            RequestSpecificValues.Current_Mode.Request_Completed = true;
                            HttpContext.Current.Response.Redirect(returnUrl, false);
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            return;
                        }

                        // Was this a DELETE/
                        if (action_value == "delete")
                        {
                            // Try to delete this folder
                            bool result = SobekCM_Database.Builder_Folder_Delete(folderId, RequestSpecificValues.Tracer);
                            if (!result)
                            {
                                actionMessage = "Unknown error encountered while trying to delete this folder";
                            }
                            else
                            {
                                string returnUrl = RequestSpecificValues.Current_Mode.Base_URL + "l/admin/settings/builder/folders";
                                RequestSpecificValues.Current_Mode.Request_Completed = true;
                                HttpContext.Current.Response.Redirect(returnUrl, false);
                                HttpContext.Current.ApplicationInstance.CompleteRequest();
                                return;
                            }
                        }

                        // Was this a SAVE?
                        if (action_value == "save")
                        {
                            // Perform some validations
                            List<string> errors = new List<string>();
                            if (String.IsNullOrWhiteSpace(folderName)) errors.Add("DESCRIPTIVE NAME is required and missing");
                            if (String.IsNullOrWhiteSpace(failuresFolder)) errors.Add("FAILURES FOLDER is required and missing");
                            if (String.IsNullOrWhiteSpace(inboundFolder)) errors.Add("INBOUND FOLDER is required and missing");
                            if (String.IsNullOrWhiteSpace(processingFolder)) errors.Add("PROCESSING FOLDER is required and missing");

                            // If there were error, prepare the error message and don't save
                            if (errors.Count > 0)
                            {
                                actionMessage = "ERROR: Some required fields are missing:<br /><br />";
                                foreach (string thisError in errors)
                                {
                                    actionMessage = actionMessage + "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; " + thisError + "<br />";
                                }
                            }
                            else
                            {
                                // Try to save this folder
                                bool result = SobekCM_Database.Builder_Folder_Edit(folderId, folderName, inboundFolder, failuresFolder, processingFolder, performChecksum, archiveTiffs, archiveAllFiles, allowDeletes, allowFoldersNoMetadata, bibIdRestrictions, moduleSetId, RequestSpecificValues.Tracer );
                                if (!result)
                                {
                                    actionMessage = "Unknown error encountered while saving folder to the database";
                                }
                                else
                                {
                                    // Successfully saved
                                    saved = true;
                                    actionMessage = "Successfully saved builder folder changes.";

                                    // Clear settings to be pulled again
                                    HttpContext.Current.Session["Admin_Settigs"] = null;

                                    // Assign this to be used by the system
                                    UI_ApplicationCache_Gateway.ResetSettings();

                                    // Also, look to see if a warning might be suitable
                                    List<string> warnings = new List<string>();
                                    try
                                    {
                                        if (!Directory.Exists(failuresFolder)) warnings.Add("Can't verify existence of the FAILURES FOLDER");
                                        if (!Directory.Exists(inboundFolder)) warnings.Add("Can't verify existence of the INBOUND FOLDER");
                                        if (!Directory.Exists(processingFolder)) warnings.Add("Can't verify existence of the PROCESSING FOLDER");
                                    }
                                    catch (Exception)
                                    {
                                        warnings.Clear();
                                        warnings.Add("Exception caught while trying to verify folders.");
                                    }

                                    // Add warnings
                                    if (warnings.Count == 3)
                                    {
                                        // i.e., none of the folders could be verified
                                        actionMessage = actionMessage + "<br /><br />WARNING: Unable to verify existence of any of the folders.  This may be normal since the account under which the web runs does not necessarily need access to the builder folders.";
                                    }
                                    else if ( warnings.Count > 0 )
                                    {
                                        actionMessage = actionMessage + "<br /><br />WARNING: Unable to verify existence of some of the folders:<br /><br />";
                                        foreach (string thisWarning in warnings)
                                        {
                                            actionMessage = actionMessage + "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; " + thisWarning + "<br />";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            else // NOT A POST BACK
            {
                // Configure the values
                folderName = String.Empty;
                failuresFolder = String.Empty;
                inboundFolder = String.Empty;
                processingFolder = String.Empty;
                performChecksum = false;
                archiveTiffs = false;
                archiveAllFiles = false;
                allowDeletes = true;
                allowFoldersNoMetadata = true;
                bibIdRestrictions = String.Empty;
                moduleSetId = 10;

                // Is there a folder specified?
                if (folderId > 0)
                {
                    // Try to get this source folder
                    Builder_Source_Folder sourceFolder = SobekEngineClient.Builder.Get_Builder_Folder(folderId, RequestSpecificValues.Tracer);

                    if (sourceFolder != null)
                    {
                        // Set the values from the existing source folder
                        folderName = sourceFolder.Folder_Name;
                        failuresFolder = sourceFolder.Failures_Folder;
                        inboundFolder = sourceFolder.Inbound_Folder;
                        processingFolder = sourceFolder.Processing_Folder;
                        performChecksum = sourceFolder.Perform_Checksum;
                        archiveTiffs = sourceFolder.Archive_TIFFs;
                        archiveAllFiles = sourceFolder.Archive_All_Files;
                        allowDeletes = sourceFolder.Allow_Deletes;
                        allowFoldersNoMetadata = sourceFolder.Allow_Folders_No_Metadata;
                        allowMetadataUpdates = sourceFolder.Allow_Metadata_Updates;
                        bibIdRestrictions = sourceFolder.BibID_Roots_Restrictions;
                        moduleSetId = sourceFolder.Builder_Module_Set.SetID;
                    }
                    else
                        folderId = -1;
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
            // Add the javascript
            Output.WriteLine("<script src=\"" + Static_Resources_Gateway.Sobekcm_Admin_Js + "\" type=\"text/javascript\"></script>");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden fields are used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_builder_folder_action\" name=\"admin_builder_folder_action\" value=\"\" />");
            Output.WriteLine();

            // Add the hidden field
            Output.WriteLine("<!-- Hidden fields retain two settings which are not currently exposed through the interface -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_builder_folder_modulesetid\" name=\"admin_builder_folder_modulesetid\" value=\"" + moduleSetId + "\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_builder_folder_restrictions\" name=\"admin_builder_folder_restrictions\" value=\"" + bibIdRestrictions + "\" />");
            Output.WriteLine();

            Output.WriteLine("<div class=\"sbkAdm_HomeText\">");

            // Add the action message, if there is one
            if (!String.IsNullOrEmpty(actionMessage))
            {
                // If this is an error, show it differently
                if (actionMessage.IndexOf("Error", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <div id=\"sbkAdm_ActionMessageError\">" + actionMessage + "</div>");
                }
                else
                {
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <div id=\"sbkAdm_ActionMessageSuccess\">" + actionMessage + "</div>");
                }
            }

            // Constant help variables
            const string NAME_HELP = "Enter a descriptive label for this incoming folder.\\n\\nThis will only be used to describe the source of the material in any logs.";
            const string INBOUND_HELP = "Network location where new incoming packages will be written for bulk loading.   \\n\\nThis should be on the same directory system as the inbound and error folders, and should not be a subdirectory of either.";
            const string PROCESSING_HELP = "Network location where packages which are currently being processed will be moved from the inbound network location.   \\n\\nThis should be on the same directory system as the inbound and error folders, and should not be a subdirectory of either.";
            const string FAILURES_HELP = "Network location where any packages which fail validation or fail to load will be moved.   \\n\\nThis should be on the same directory system as the inbound and processing folders, and should not be a subdirectory of either.";
            const string CHECKSUM_HELP = "Flag indicates that if an incoming METS file has checksums, the files will be validated against those checksums.";
            const string ARCHIVE_TIFF_HELP = "Flag indicates that incoming digital resource TIFF files will be copied to the Archival Drop Box directory, if such a value has been set in the main system settings.\\n\\nThis setting has no meaning if the next flag (for archiving ALL files) has been set to TRUE.";
            const string ARCHIVE_ALL_HELP = "Flag indicates that ALL incoming digital resource files will be copied to the Archival Drop Box directory, if such a value has been set in the main system settings.";
            const string DELETE_HELP = "Flag indicates if incoming METS files with a record status of DELETE should be accepted and result in the deletion of the corresponding item from this system.";
            const string NO_METADATA_HELP = "Flag indicates that all incoming digital resources MUST be accompanied by a METS file, or they will be rejected.";
            const string METADATA_UPDATE_HELP = "Flag indicates if incoming METS files with METADATA_UPDATE or PARTIAL are allowed in this folder, which will update just the metadata portion of the METS and possibly add new files.";

            //const string BIB_RESTRICT_HELP = "Flag indicates that incoming packages are restricted by the BibID root.  Packages that begin with a different root are rejected.  Multiple roots are added with a &quot;pipe&quot; between them, such as &quot;UF01|CA|SMI&quot;.";

            Output.WriteLine("<p>Below is the information for an existing builder incoming folder.  Use this form to make any changes to the folder, such as the network folders or any of the folder options.</p>");

            Output.WriteLine("<table id=\"sbkBfmav_DisplayTable\">");

            // Add line for descriptive name
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td id=\"sbkBfmav_DisplayTable_LabelCol\"><label for=\"admin_folder_name\">Descriptive Name:</label></td>");
            Output.WriteLine("    <td id=\"sbkBfmav_DisplayTable_EntryCol\"><input class=\"sbkBfmav_DescNameBox sbkAdmin_Focusable\" name=\"admin_folder_name\" id=\"admin_folder_name\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(folderName ?? String.Empty) + "\" /></td>");
            Output.WriteLine("    <td colspan=\"2\"><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + NAME_HELP + "');\"  title=\"" + NAME_HELP + "\" /></td>");
            Output.WriteLine("  </tr>");

            // Add the network folder
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td><label for=\"admin_folder_network\">Inbound Folder:</label></td>");
            Output.WriteLine("    <td colspan=\"2\"><input class=\"sbkBfmav_NetworkInput sbkAdmin_Focusable\" name=\"admin_folder_network\" id=\"admin_folder_network\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(inboundFolder ?? String.Empty) + "\" /></td>");
            Output.WriteLine("    <td id=\"sbkBfmav_DisplayTable_LastCol\"><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + INBOUND_HELP + "');\"  title=\"" + INBOUND_HELP + "\" /></td>");
            Output.WriteLine("  </tr>");

            // Add the processing folder
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td><label for=\"admin_folder_processing\">Processing Folder:</label></td>");
            Output.WriteLine("    <td colspan=\"2\"><input class=\"sbkBfmav_NetworkInput sbkAdmin_Focusable\" name=\"admin_folder_processing\" id=\"admin_folder_processing\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(processingFolder ?? String.Empty) + "\" /></td>");
            Output.WriteLine("    <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + PROCESSING_HELP + "');\"  title=\"" + PROCESSING_HELP + "\" /></td>");
            Output.WriteLine("  </tr>");

            // Add the error folder
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td><label for=\"admin_folder_error\">Failures Folder:</label></td>");
            Output.WriteLine("    <td colspan=\"2\"><input class=\"sbkBfmav_NetworkInput sbkAdmin_Focusable\" name=\"admin_folder_error\" id=\"admin_folder_error\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(failuresFolder ?? String.Empty) + "\" /></td>");
            Output.WriteLine("    <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + FAILURES_HELP + "');\"  title=\"" + FAILURES_HELP + "\" /></td>");
            Output.WriteLine("  </tr>");

            // Add the folder options title and first folder option ( checksum validation )
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td>Folder Options:</td>");
            Output.WriteLine("    <td colspan=\"3\" style=\"vertical-align: middle\">");
            Output.WriteLine("      <input class=\"sbkBfmav_FolderOptionCheckbox sbkAdmin_Focusable\" name=\"admin_folder_checksum\" id=\"admin_folder_checksum\" type=\"checkbox\" " + (performChecksum ? "checked=\"checked\" " : String.Empty) + " />");
            Output.WriteLine("      <label for=\"admin_folder_checksum\">Perform checksum validation against any checksums in the METS files</label>");
            Output.WriteLine("      <img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + CHECKSUM_HELP + "');\"  title=\"" + CHECKSUM_HELP + "\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add the copy TIFFs to archive directory folder option
            Output.WriteLine("  <tr  id=\"sbkBfmav_ArchiveTiffRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td colspan=\"3\" style=\"vertical-align: middle\">");
            Output.WriteLine("      <input class=\"sbkBfmav_FolderOptionCheckbox sbkAdmin_Focusable\" name=\"admin_folder_archive_tiff\" id=\"admin_folder_archive_tiff\" type=\"checkbox\" " + (archiveTiffs ? "checked=\"checked\" " : String.Empty) + " />");
            Output.WriteLine("      <label for=\"admin_folder_archive_tiff\">Copy all TIFF files to the archiving directory</label>");
            Output.WriteLine("      <img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + ARCHIVE_TIFF_HELP + "');\"  title=\"" + ARCHIVE_TIFF_HELP + "\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add the copy all incoming files to archive directory folder option
            Output.WriteLine("  <tr id=\"sbkBfmav_ArchiveAllRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td colspan=\"3\" style=\"vertical-align: middle\">");
            Output.WriteLine("      <input class=\"sbkBfmav_FolderOptionCheckbox sbkAdmin_Focusable\" name=\"admin_folder_archive_all\" id=\"admin_folder_archive_all\" type=\"checkbox\" " + (archiveAllFiles ? "checked=\"checked\" " : String.Empty) + " />");
            Output.WriteLine("      <label for=\"admin_folder_archive_all\">Copy all incoming files to the archiving directory</label>");
            Output.WriteLine("      <img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + ARCHIVE_ALL_HELP + "');\"  title=\"" + ARCHIVE_ALL_HELP + "\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            //// Add the flag to allow METADATA UPDATE mets files folder option
            //Output.WriteLine("  <tr id=\"sbkBfmav_DeleteRow\">");
            //Output.WriteLine("    <td>&nbsp;</td>");
            //Output.WriteLine("    <td colspan=\"3\" style=\"vertical-align: middle\">");
            //Output.WriteLine("      <input class=\"sbkBfmav_FolderOptionCheckbox sbkAdmin_Focusable\" name=\"admin_folder_allow_updates\" id=\"admin_folder_allow_updates\" type=\"checkbox\" " + (allowMetadataUpdates ? "checked=\"checked\" " : String.Empty) + " />");
            //Output.WriteLine("      <label for=\"admin_folder_allow_updates\">Allow METS files with a RecordStatus of METADATA UPDATE or PARTIAL in this folder</label>");
            //Output.WriteLine("      <img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + METADATA_UPDATE_HELP + "');\"  title=\"" + METADATA_UPDATE_HELP + "\" />");
            //Output.WriteLine("    </td>");
            //Output.WriteLine("  </tr>");

            // Add the flag to allow DELETE mets files folder option
            Output.WriteLine("  <tr id=\"sbkBfmav_DeleteRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td colspan=\"3\" style=\"vertical-align: middle\">");
            Output.WriteLine("      <input class=\"sbkBfmav_FolderOptionCheckbox sbkAdmin_Focusable\" name=\"admin_folder_allow_delete\" id=\"admin_folder_allow_delete\" type=\"checkbox\" " + (allowDeletes ? "checked=\"checked\" " : String.Empty) + " />");
            Output.WriteLine("      <label for=\"admin_folder_allow_delete\">Allow METS files with a RecordStatus of DELETE in this folder</label>");
            Output.WriteLine("      <img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + DELETE_HELP + "');\"  title=\"" + DELETE_HELP + "\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add the flag to allow resource files without METS files
            Output.WriteLine("  <tr id=\"sbkBfmav_NoMetsRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td colspan=\"3\" style=\"vertical-align: middle\">");
            Output.WriteLine("      <input class=\"sbkBfmav_FolderOptionCheckbox sbkAdmin_Focusable\" name=\"admin_folder_no_metadata\" id=\"admin_folder_no_metadata\" type=\"checkbox\" " + (allowFoldersNoMetadata ? "checked=\"checked\" " : String.Empty) + " />");
            Output.WriteLine("      <label for=\"admin_folder_no_metadata\">Allow resource files in appropriately names folders without METS files</label>");
            Output.WriteLine("      <img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + NO_METADATA_HELP + "');\"  title=\"" + NO_METADATA_HELP + "\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            //// Add the flag to restrict to certain BibID roots
            //Output.WriteLine("  <tr id=\"sbkBfmav_BibRestrictionsRow\" style=\"display:none;\">");
            //Output.WriteLine("    <td>&nbsp;</td>");
            //Output.WriteLine("    <td colspan=\"3\" style=\"vertical-align: middle\">");
            //Output.WriteLine("      <input class=\"sbkBfmav_FolderOptionCheckbox sbkAdmin_Focusable\" name=\"admin_folder_bibrestrict\" id=\"admin_folder_bibrestrict\" type=\"checkbox\" " + ((String.IsNullOrWhiteSpace(bibidRootsRestrictions) ? String.Empty : "checked=\"checked\" ")) + " />");
            //Output.WriteLine("      <label for=\"admin_folder_bibrestrict\">Restrict incoming packages to certain BibID roots.</label>");
            //Output.WriteLine("      <img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + BIB_RESTRICT_HELP + "');\"  title=\"" + BIB_RESTRICT_HELP + "\" />");
            //Output.WriteLine("    </td>");
            //Output.WriteLine("  </tr>");

            // Determine button text
            string button_text = "ADD";
            string cancel_button_text = "CANCEL";
            string cancel_text = "Do not apply changes";
            string button_title = "Add this new builder incoming folder";
            if (folderId > 0)
            {
                button_text = "SAVE";
                button_title = "Save builder incoming folder changes";
            }
            if (saved)
            {
                cancel_button_text = "DONE";
                cancel_text = "Done with changes";
            }


            // Add line for button
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td></td>");
            Output.WriteLine("    <td colspan=\"3\">");
            Output.WriteLine("      <button title=\"" + cancel_text + "\" class=\"sbkAdm_RoundButton\" onclick=\"set_hidden_value_postback('admin_builder_folder_action', 'cancel'); return false;\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> " + cancel_button_text + "</button> &nbsp; &nbsp; ");
            if ( folderId > 0 )
                Output.WriteLine("      <button title=\"Delete this incoming folder completely\" class=\"sbkAdm_RoundButton\" onclick=\"set_hidden_value_postback('admin_builder_folder_action', 'delete'); return false;\"> DELETE </button> &nbsp; &nbsp; ");

            Output.WriteLine("      <button title=\"" + button_title + "\" class=\"sbkAdm_RoundButton\" onclick=\"set_hidden_value_postback('admin_builder_folder_action', 'save'); return false;\">SAVE <img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add the SAVE button
            Output.WriteLine("</table>");

            Output.WriteLine("</div>");
        }
    }
}
