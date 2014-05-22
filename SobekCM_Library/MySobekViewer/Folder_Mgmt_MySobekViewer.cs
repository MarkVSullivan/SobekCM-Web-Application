#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Library.Settings;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Database;
using SobekCM.Library.Email;
using SobekCM.Library.HTML;
using SobekCM.Library.Items;
using SobekCM.Library.MainWriters;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Library.Skins;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.MySobekViewer
{

    /// <summary> Class allows an authenticated user to view the items and searches in their bookshelves (or folders) online </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to display the list of items in each bookshelf or the list of bookshelves</li>
    /// <li>This viewer uses the <see cref="PagedResults_HtmlSubwriter"/> class to display the items just like any search or browse on SobekCM </li>
    /// </ul></remarks>
    public class Folder_Mgmt_MySobekViewer : abstract_MySobekViewer
    {
        private readonly Aggregation_Code_Manager codeManager;
        private readonly Item_Aggregation currentCollection;
        private readonly SobekCM_Skin_Object htmlSkin;
        private readonly Item_Lookup_Object itemList;
        private readonly List<iSearch_Title_Result> pagedResults;
        private readonly string properFolderName;
        private readonly Search_Results_Statistics resultsStatistics;
        private PagedResults_HtmlSubwriter writeResult;

        /// <summary> Constructor for a new instance of the Folder_Mgmt_MySobekViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Results_Statistics"> Information about the entire set of results for the current folder </param>
        /// <param name="Paged_Results"> Single page of results for the current folder, within the entire set </param>
        /// <param name="Code_Manager"> Code manager object maintains mapping between SobekCM codes and greenstone codes (used by result_dataset_html_subwriter)</param>
        /// <param name="Item_List"> Object for pulling additional information about each item during display </param>
        /// <param name="CurrentCollection"> Current item aggregation [UNUSED?] </param>
        /// <param name="HTMLSkin"> HTML interface, which determines the header, footer, stylesheet, and other design elements for the rendered HTML</param>
        /// <param name="Translator"> Translation / language support object for writing the user interface is multiple languages</param>
		/// <param name="CurrentMode"> Mode / navigation information for the current request</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Folder_Mgmt_MySobekViewer(User_Object User,
            Search_Results_Statistics Results_Statistics,
            List<iSearch_Title_Result> Paged_Results,
            Aggregation_Code_Manager Code_Manager,
            Item_Lookup_Object Item_List,
            Item_Aggregation CurrentCollection, 
            SobekCM_Skin_Object HTMLSkin,
            Language_Support_Info Translator, 
            SobekCM_Navigation_Object CurrentMode,
            Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Folder_Mgmt_MySobekViewer.Constructor", String.Empty);

	        currentMode = CurrentMode;
            user = User;
            pagedResults = Paged_Results;
            resultsStatistics = Results_Statistics;
            codeManager = Code_Manager;
            itemList = Item_List;
            htmlSkin = HTMLSkin;
            base.Translator = Translator;
            currentCollection = CurrentCollection;

            properFolderName = String.Empty;
            int current_folder_id = -1;
            if (currentMode.My_Sobek_SubMode.Length > 0)
            {
                // Try to get this user folder from the user object
                User_Folder userFolder = user.Get_Folder( currentMode.My_Sobek_SubMode );

                // If the user folder is null, then this folder is not in the current user object
                // This may still be a valid folder though.  Check this by pulling folder list for this 
                // user again
                if (userFolder == null)
                {
                    // Get the user from the database again
                    User_Object checkFolderUser = SobekCM_Database.Get_User(user.UserID, Tracer);

                    // Look for this folder in the new user object
                    userFolder = checkFolderUser.Get_Folder(currentMode.My_Sobek_SubMode);
                    if (userFolder == null)
                    {
                        // Invalid folder.. should not have gotten this far though
                        HttpContext.Current.Response.Redirect(currentMode.Base_URL, false);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        currentMode.Request_Completed = true;
                        return;
                    }


                    // Save this to the user so this does not have to happen again
                    user.Add_Folder(userFolder);
                }

                // Get the proper name and folder id
                Debug.Assert(userFolder != null, "userFolder != null");
                properFolderName = userFolder.Folder_Name;
                current_folder_id = userFolder.Folder_ID;
            }

            if ((currentMode.isPostBack) || ((HttpContext.Current.Request.Form["item_action"] != null) && (HttpContext.Current.Request.Form["item_action"].Length > 0 )))
            {
                try
                {
                    // Pull the standard values
                    NameValueCollection form = HttpContext.Current.Request.Form;

                    string item_action = form["item_action"].Replace(",","").ToUpper().Trim();
                    string bookshelf_items = form["bookshelf_items"].Trim().Replace("%22", "\"").Replace("%27", "'").Replace("%3D", "=").Replace("%26", "&");
                    string bookshelf_params = form["bookshelf_params"].Trim();
                    string add_bookshelf = String.Empty;
                    if ( form["add_bookshelf"] != null )
                        add_bookshelf = form["add_bookshelf"].Trim();

                    if (item_action == "REFRESH_FOLDER")
                    {
                         refresh_user_folders(user, Tracer);
                         Cached_Data_Manager.Remove_All_User_Folder_Browses(user.UserID, Tracer);
                    }

                    if (item_action == "DELETE_FOLDER")
                    {
                        int folder_id = Convert.ToInt32(bookshelf_items);

                        SobekCM_Database.Delete_User_Folder(user.UserID, folder_id, Tracer);
                        Cached_Data_Manager.Clear_Public_Folder_Info(folder_id, Tracer);
                        refresh_user_folders(user, Tracer);
                    }

                    if (item_action == "NEW_BOOKSHELF")
                    {
                        string folder_name = form["new_bookshelf_name"].Trim().Replace("<", "(").Replace(">", ")");
                        int parent_id = Convert.ToInt32(form["new_bookshelf_parent"]);

                        if (SobekCM_Database.Edit_User_Folder(-1, user.UserID, parent_id, folder_name, false, String.Empty, Tracer) > 0)
                        {
                            refresh_user_folders(user, Tracer);
                        }
                    }


                    if ( item_action == "FOLDER_VISIBILITY" )
                    {
                        User_Folder thisFolder = user.Get_Folder(bookshelf_items);
                        if (bookshelf_params.ToUpper() == "PRIVATE")
                        {
                            if (SobekCM_Database.Edit_User_Folder(thisFolder.Folder_ID, user.UserID, -1, thisFolder.Folder_Name, false, String.Empty, Tracer) >= 0)
                                thisFolder.isPublic = false;

                            Cached_Data_Manager.Clear_Public_Folder_Info(thisFolder.Folder_ID, Tracer);
                        }

                        if (bookshelf_params.ToUpper() == "PUBLIC")
                        {
                            if (SobekCM_Database.Edit_User_Folder(thisFolder.Folder_ID, user.UserID, -1, thisFolder.Folder_Name, true, String.Empty, Tracer) >= 0 )
                                thisFolder.isPublic = true;
                        }                        
                    }

                    if ((item_action == "REMOVE") || ( item_action == "MOVE" ))
                    {
                        if (bookshelf_items.IndexOf("|") > 0)
                        {
                            string[] split_multi_items = bookshelf_items.Split("|".ToCharArray());
                            foreach (string[] split in split_multi_items.Select(ThisItem => ThisItem.Split("_".ToCharArray())).Where(Split => Split.Length == 2))
                            {
                                SobekCM_Database.Delete_Item_From_User_Folder(user.UserID, properFolderName, split[0], split[1], Tracer);
                                if (item_action == "MOVE")
                                {
                                    SobekCM_Database.Add_Item_To_User_Folder(user.UserID, add_bookshelf, split[0], split[1], 0, String.Empty, Tracer);
                                }
                            }

                            // Ensure this user folder is not sitting in the cache
                            Cached_Data_Manager.Remove_User_Folder_Browse(user.UserID, properFolderName, Tracer);
                            Cached_Data_Manager.Clear_Public_Folder_Info(current_folder_id, Tracer);
                            if (item_action == "MOVE")
                            {
                                Cached_Data_Manager.Remove_User_Folder_Browse(user.UserID, add_bookshelf, Tracer);
                                User_Folder moved_to_folder = user.Get_Folder(add_bookshelf);
                                if (moved_to_folder != null)
                                {
                                    Cached_Data_Manager.Clear_Public_Folder_Info(moved_to_folder.Folder_ID, Tracer);
                                }
                            }
                        }
                        else
                        {
                                string[] split = bookshelf_items.Split("_".ToCharArray());
                                if (split.Length == 2)
                                {
                                    SobekCM_Database.Delete_Item_From_User_Folder(user.UserID, properFolderName, split[0], split[1], Tracer);
                                    if (item_action == "MOVE")
                                    {
                                        SobekCM_Database.Add_Item_To_User_Folder(user.UserID, add_bookshelf, split[0], split[1], 1, String.Empty, Tracer);
                                    }
                                }

                            // Ensure this user folder is not sitting in the cache
                            Cached_Data_Manager.Remove_User_Folder_Browse(user.UserID, properFolderName, Tracer);
                            Cached_Data_Manager.Clear_Public_Folder_Info(current_folder_id, Tracer);
                            if (item_action == "MOVE")
                            {
                                Cached_Data_Manager.Remove_User_Folder_Browse(user.UserID, add_bookshelf, Tracer);
                                User_Folder moved_to_folder = user.Get_Folder(add_bookshelf);
                                if (moved_to_folder != null)
                                {
                                    Cached_Data_Manager.Clear_Public_Folder_Info(moved_to_folder.Folder_ID, Tracer);
                                }
                            }
                        }
                    }

                    if ( item_action == "EMAIL" )
                    {
                        string comments = form["email_comments"].Trim().Replace(">",")").Replace("<","(");
                        string email = form["email_address"].Trim();
                        string format = HttpContext.Current.Request.Form["email_format"].Trim().ToUpper();

                            string[] split = bookshelf_items.Split("_".ToCharArray());
                            if (split.Length == 2)
                            {
                                SobekCM_Assistant newAssistant = new SobekCM_Assistant();
                                SobekCM_Item newItem;
                                Page_TreeNode newPage;
                                SobekCM_Items_In_Title itemsInTitle;
	                            newAssistant.Get_Item(currentMode, Item_List, SobekCM_Library_Settings.Image_URL, null, null, user, Tracer, out newItem, out newPage, out itemsInTitle );
                                SobekCM_Database.Add_Item_To_User_Folder(user.UserID, add_bookshelf, split[0], split[1], 1, comments, Tracer);

                                // Determine the email format
                                bool is_html_format = (format != "TEXT");

                                // Send this email
                                Item_Email_Helper.Send_Email(email, String.Empty, comments, user.Full_Name, currentMode.SobekCM_Instance_Abbreviation, newItem, is_html_format, currentMode.Base_URL + newItem.BibID + "/" + newItem.VID, user.UserID);
                            }
                    }

                    if ( item_action == "EDIT_NOTES" )
                    {
                        string notes = form["add_notes"].Trim().Replace(">",")").Replace("<","(");

                            string[] split = bookshelf_items.Split("_".ToCharArray());
                            if (split.Length == 2)
                            {
                                SobekCM_Database.Add_Item_To_User_Folder(user.UserID, add_bookshelf, split[0], split[1], 1, notes, Tracer);
                                Cached_Data_Manager.Remove_User_Folder_Browse(user.UserID, add_bookshelf, Tracer);
                            }

                    }
                }
                catch(Exception)
                {
                    // Catches any errors which may occur.  User will be sent back to the same URL,
                    // so any error that occurs should be obvious to the user
                }

                string return_url = HttpContext.Current.Items["Original_URL"].ToString();
                HttpContext.Current.Response.Redirect(return_url, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                currentMode.Request_Completed = true;
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'Folder Management' </value>
        public override string Web_Title
        {
            get { return "My Library"; }
        }

        /// <summary> Property indicates if this mySobek viewer can contain pop-up forms</summary>
        /// <remarks> If the mySobek viewer contains pop-up forms the overall page renders differently, 
        /// allowing for the blanket division and the popup forms near the top of the rendered HTML </remarks>
        ///<value> This mySobek viewer always returns the value TRUE </value>
        public override bool Contains_Popup_Forms
        {
            get
            {
                return true;
            }
        }

        private void refresh_user_folders(User_Object User, Custom_Tracer Tracer )
        {
            DataSet thisSet = SobekCM_Database.Get_Folder_Search_Information(User.UserID, Tracer);

            // Add the current folder names
            Dictionary<int, User_Folder> folderNodes = new Dictionary<int, User_Folder>();
            List<User_Folder> parentNodes = new List<User_Folder>();
            foreach (DataRow folderRow in thisSet.Tables[0].Rows)
            {
                string folder_name = folderRow["FolderName"].ToString();
                int folderid = Convert.ToInt32(folderRow["UserFolderID"]);
                int parentid = Convert.ToInt32(folderRow["ParentFolderID"]);
                bool isPublic = Convert.ToBoolean(folderRow["isPublic"]);

                User_Folder newFolderNode = new User_Folder(folder_name, folderid) {isPublic = isPublic};
                if (parentid == -1)
                    parentNodes.Add(newFolderNode);
                folderNodes.Add(folderid, newFolderNode);
            }
            foreach (DataRow folderRow in thisSet.Tables[0].Rows)
            {
                int folderid = Convert.ToInt32(folderRow["UserFolderID"]);
                int parentid = Convert.ToInt32(folderRow["ParentFolderID"]);
                if (parentid > 0)
                {
                    folderNodes[parentid].Add_Child_Folder(folderNodes[folderid]);
                }
            }
            user.Clear_Folders();
            foreach (User_Folder root_folder in parentNodes)
                user.Add_Folder(root_folder);
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Folder_Mgmt_MySobekViewer.Write_HTML", String.Empty);

			if (currentMode.My_Sobek_SubMode != "submitted items")
			{
				Output.WriteLine("  <h1>" + Web_Title + "</h1>");
				Output.WriteLine();
			}
        }

        /// <summary> Add the HTML to be added near the top of the page for those viewers that implement pop-up forms for data retrieval </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds any popup divisions for form metadata elements </remarks>
        public override void Add_Popup_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Folder_Mgmt_MySobekViewer.Add_Popup_HTML", "Add any popup divisions for form elements");

			Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.3.custom.min.js\"></script>");
			Output.WriteLine();

            // Add the hidden fields
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"item_action\" name=\"item_action\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"bookshelf_items\" name=\"bookshelf_items\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"bookshelf_params\" name=\"bookshelf_params\" value=\"\" />");
			Output.WriteLine();

            if (currentMode.My_Sobek_SubMode.Length > 0)
            {
                #region Email form

                if (user != null)
                {
                    Output.WriteLine("<!-- Email form -->");
					Output.WriteLine("<div class=\"sbkFmsv_EmailPopup sbkMySobek_PopupForm\" id=\"form_email\" style=\"display:none;\">");
					Output.WriteLine("  <div class=\"sbkMySobek_PopupTitle\"><table style=\"width:100%;\"><tr style=\"height:20px;\"><td style=\"text-align:left;\">SEND THIS ITEM TO A FRIEND</td><td style=\"text-align:right;\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"email_form_close()\">X</a> &nbsp; </td></tr></table></div>");
					

                    Output.WriteLine("  <fieldset><legend>Enter the email information below &nbsp; </legend>");
					Output.WriteLine("    <table class=\"sbkMySobek_PopupTable\">");


                    // Add email address line
	                Output.WriteLine("      <tr>");
					Output.WriteLine("        <td style=\"width:80px\"><label for=\"email_address\">To:</label></td>");
					Output.WriteLine("        <td><input class=\"sbkFmsv_EmailInput sbkMySobek_Focusable\" name=\"email_address\" id=\"email_address\" type=\"text\" value=\"" + user.Email + "\" /></td>");
					Output.WriteLine("      </tr>");

                    // Add comments area
	                Output.WriteLine("      <tr style=\"vertical-align:top\">");
					Output.WriteLine("        <td><label for=\"email_comments\">Comments:</label></td>");
					Output.WriteLine("        <td><textarea rows=\"6\" class=\"sbkFmsv_EmailTextArea sbkMySobek_Focusable\" name=\"email_comments\" id=\"email_comments\"></textarea></td>");
					Output.WriteLine("      </tr>");

                    // Add format area
	                Output.WriteLine("      <tr>");
					Output.WriteLine("        <td>Format:</td>");
	                Output.WriteLine("        <td>");
					Output.WriteLine("            <input type=\"radio\" class=\"sbkMySobek_checkbox\" name=\"email_format\" id=\"email_format_html\" value=\"html\" checked=\"checked\" /> <label for=\"email_format_html\">HTML</label> &nbsp; &nbsp; ");
					Output.WriteLine("            <input type=\"radio\" class=\"sbkMySobek_checkbox\" name=\"email_format\" id=\"email_format_text\" value=\"text\" /> <label for=\"email_format_text\">Plain Text</label>");
					Output.WriteLine("        </td>");
					Output.WriteLine("      </tr>");

                    Output.WriteLine("    </table>");
                    Output.WriteLine("  </fieldset>");
                    Output.WriteLine("  <div class=\"sbk_PopupButtonsDiv\"><a href=\"\" onclick=\"return email_form_close();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/send_button_g.gif\" value=\"Submit\" alt=\"Submit\"></div><br />");
                    Output.WriteLine("</div>");
                    Output.WriteLine();
                }

                #endregion

                #region Move item

                if (user != null)
                {
                    Output.WriteLine("<!-- Move between bookshelves form -->");
                    Output.WriteLine("<div class=\"add_popup_div\" id=\"move_item_form\" style=\"display:none;\">");
                    Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">M<span class=\"smaller\">OVE</span> I<span class=\"smaller\">TEM BETWEEN </span>B<span class=\"smaller\">OOKSHELVES</span></td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"move_form_close()\">X</a> &nbsp; </td></tr></table></div>");
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <fieldset><legend><span id=\"move_legend\">Select new bookshelf for this item</span> &nbsp; </legend>");
                    Output.WriteLine("    <br />");
                    Output.WriteLine("    <table class=\"popup_table\">");


                    // Add the list of all bookshelves
                    Output.Write("      <tr align=\"left\"><td width=\"80px\"><label for=\"add_bookshelf\">Bookshelf:</label></td>");
                    Output.Write("<td><select class=\"email_bookshelf_input\" name=\"add_bookshelf\" id=\"add_bookshelf\">");

                    foreach (User_Folder folder in user.All_Folders)
                    {
                        if (folder.Folder_Name.Length > 80)
                        {
                            Output.Write("<option value=\"" + HttpUtility.HtmlEncode(folder.Folder_Name) + "\">" + HttpUtility.HtmlEncode(folder.Folder_Name.Substring(0, 75)) + "...</option>");
                        }
                        else
                        {
                            if (folder.Folder_Name != "Submitted Items")
                            {
                                if (folder.Folder_Name == properFolderName)
                                    Output.Write("<option value=\"" + HttpUtility.HtmlEncode(folder.Folder_Name) + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(folder.Folder_Name) + "</option>");
                                else
                                    Output.Write("<option value=\"" + HttpUtility.HtmlEncode(folder.Folder_Name) + "\">" + HttpUtility.HtmlEncode(folder.Folder_Name) + "</option>");
                            }
                        }
                    }
                    Output.WriteLine("</select></td></tr>");

                    Output.WriteLine("    </table>");
                    Output.WriteLine("    <br />");
                    Output.WriteLine("  </fieldset><br />");
                    Output.WriteLine("  <center><a href=\"\" onclick=\"return move_form_close();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Submit\" alt=\"Submit\"></center><br />");
                    Output.WriteLine("</div>");
                    Output.WriteLine();
                }

                #endregion

                #region Edit user notes

                if (user != null)
                {
                    Output.WriteLine("<!-- Add/Edit Bookshelf Item Notes -->");
                    Output.WriteLine("<div class=\"add_popup_div\" id=\"add_item_form\" style=\"display:none;\">");
                    Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">A<span class=\"smaller\">DD/</span>E<span class=\"smaller\">DIT</span> N<span class=\"smaller\">OTES FOR</span> B<span class=\"smaller\">OOKSHELF</span> I<span class=\"smaller\">TEM</span></td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"add_item_form_close()\">X</a> &nbsp; </td></tr></table></div>");
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <fieldset><legend>Enter notes for this item in your bookshelf &nbsp; </legend>");
                    Output.WriteLine("    <br />");
                    Output.WriteLine("    <table class=\"popup_table\">");

                    // Add comments area
                    Output.Write("      <tr align=\"left\" valign=\"top\"><td><br /><label for=\"add_notes\">Notes:</label></td>");
                    Output.WriteLine("<td><textarea rows=\"6\" cols=\"70\" name=\"add_notes\" id=\"add_notes\" class=\"add_notes_textarea\" onfocus=\"javascript:textbox_enter('add_notes','add_notes_textarea_focused')\" onblur=\"javascript:textbox_leave('add_notes','add_notes_textarea')\"></textarea></td></tr>");

                    Output.WriteLine("    </table>");
                    Output.WriteLine("    <br />");
                    Output.WriteLine("  </fieldset><br />");
                    Output.WriteLine("  <center><a href=\"\" onclick=\"return add_item_form_close();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Submit\" alt=\"Submit\"></center><br />");
                    Output.WriteLine("</div>");
                    Output.WriteLine();
                }

                #endregion
            }
            else
            {
                #region New bookshelf form

                if (user != null)
                {
                    Output.WriteLine("<!-- New bookshelf form -->");
                    Output.WriteLine("<div class=\"add_popup_div\" id=\"new_bookshelf_form\" style=\"display:none;\">");
                    Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">N<span class=\"smaller\">EW</span> B<span class=\"smaller\">OOKSHELF</span></td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"new_bookshelf_form_close()\">X</a> &nbsp; </td></tr></table></div>");
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <fieldset><legend><span id=\"move_legend\">Enter the information for your new bookshelf</span> &nbsp; </legend>");
                    Output.WriteLine("    <br />");
                    Output.WriteLine("    <table class=\"popup_table\">");

                    // Add the bookshelf name row
                    Output.Write("      <tr align=\"left\"><td width=\"80px\"><label for=\"new_bookshelf_name\">Name:</label></td>");
                    Output.WriteLine("<td><input class=\"email_input\" name=\"new_bookshelf_name\" id=\"new_bookshelf_name\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('new_bookshelf_name', 'email_input_focused')\" onblur=\"javascript:textbox_leave('new_bookshelf_name', 'email_input')\" /></td></tr>");


                    // Add the list of all bookshelves to select a parent
                    Output.Write("      <tr align=\"left\"><td><label for=\"new_bookshelf_parent\">Parent:</label></td>");
                    Output.Write("<td><select class=\"email_bookshelf_input\" name=\"new_bookshelf_parent\" id=\"new_bookshelf_parent\">");
                    Output.Write("<option value=\"-1\" selected=\"selected\" >(none)</option>");

                    foreach (User_Folder folder in user.All_Folders)
                    {

                        if (folder.Folder_Name.Length > 80)
                        {
                            Output.Write("<option value=\"" + folder.Folder_ID + "\">" + HttpUtility.HtmlEncode(folder.Folder_Name.Substring(0, 75)) + "...</option>");
                        }
                        else
                        {
                            if (folder.Folder_Name != "Submitted Items")
                            {
                                Output.Write("<option value=\"" + folder.Folder_ID + "\">" + HttpUtility.HtmlEncode(folder.Folder_Name) + "</option>");
                            }
                        }
                    }
                    Output.WriteLine("</select></td></tr>");

                    Output.WriteLine("    </table>");
                    Output.WriteLine("    <br />");
                    Output.WriteLine("  </fieldset><br />");
                    Output.WriteLine("  <center><a href=\"\" onclick=\"return new_bookshelf_form_close();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Submit\" alt=\"Submit\"></center><br />");
                    Output.WriteLine("</div>");
                    Output.WriteLine();
                }

                #endregion
            }
        }


		/// <summary> Add controls directly to the form in the main control area placeholder </summary>
        /// <param name="MainPlaceHolder"> Main place holder to which all main controls are added </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> The <see cref="PagedResults_HtmlSubwriter"/> class is instantiated and adds controls to the placeholder here </remarks>
        public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Folder_Mgmt_MySobekViewer.Add_Controls", String.Empty);

            // If this is submitted items, don't show the folders
            string redirect_url = String.Empty;
            if (properFolderName != "Submitted Items")
            {
                // Determine the redirect
                currentMode.My_Sobek_SubMode = "XXXXXXXXXXXXXXXXXX";
                Result_Display_Type_Enum currentDisplayType = currentMode.Result_Display_Type;
                currentMode.Result_Display_Type = Result_Display_Type_Enum.Bookshelf;
                redirect_url = currentMode.Redirect_URL();
                currentMode.My_Sobek_SubMode = String.Empty;
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Saved_Searches;
                string saved_search_url = currentMode.Redirect_URL();
                currentMode.Mode = Display_Mode_Enum.Aggregation;
				currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
                currentMode.Home_Type = Home_Type_Enum.Personalized;
                currentMode.Aggregation = String.Empty;
                string personalized_home = currentMode.Redirect_URL();
                currentMode.Mode = Display_Mode_Enum.My_Sobek;
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
                currentMode.My_Sobek_SubMode = properFolderName;
                currentMode.Result_Display_Type = currentDisplayType;

                // Build the tree view object and tree view nodes now
                TreeView treeView1 = new TreeView
                                         {
                                             CssClass = "tree",
                                             EnableClientScript = true,
                                             PopulateNodesFromClient = false,
                                             ShowLines = false
                                         };
                treeView1.NodeStyle.NodeSpacing = new Unit(2);
                treeView1.NodeStyle.CssClass = "TreeNode";
                

                // Add the root my bookshelves node
                TreeNode rootNode = new TreeNode
                                        {
                                            Text = "&nbsp; <a title=\"Manage my library\" href=\"" + redirect_url.Replace("XXXXXXXXXXXXXXXXXX", String.Empty) + "\">My Library  (Manage my bookshelves)</a>",
                                            ImageUrl = currentMode.Base_URL + "default/images/bookshelf.jpg",
                                            SelectAction = TreeNodeSelectAction.None
                                        };
                treeView1.Nodes.Add(rootNode);

                // Add the personalized home page
                TreeNode homeNode = new TreeNode
                                        {
                                            Text = "&nbsp; <a title=\"View my collections home page\" href=\"" + personalized_home + "\">My Collections Home</a>", 
                                            SelectAction = TreeNodeSelectAction.None,
                                            ImageUrl = currentMode.Base_URL + "default/images/home_folder.gif"
                                        };
                rootNode.ChildNodes.Add(homeNode);

                // Add the saved searches node
                TreeNode savedSearchesNode = new TreeNode
                                                 {
                                                     Text ="&nbsp; <a title=\"View my saved searches\" href=\"" + saved_search_url + "\">My Saved Searches</a>",
                                                     SelectAction = TreeNodeSelectAction.None,
                                                     ImageUrl = currentMode.Base_URL + "default/images/saved_searches.gif"
                                                 };
                rootNode.ChildNodes.Add(savedSearchesNode);


               // StringBuilder literalBuilder = new StringBuilder();
                List<TreeNode> selectedNodes = new List<TreeNode>();
                foreach (User_Folder thisFolder in user.Folders)
                {
                    if (thisFolder.Folder_Name != "Submitted Items")
                    {
                        TreeNode folderNode = new TreeNode
                                                  { Text ="&nbsp; <a href=\"" + redirect_url.Replace("XXXXXXXXXXXXXXXXXX", thisFolder.Folder_Name_Encoded) + "\">" + thisFolder.Folder_Name + "</a>" };
                        if (thisFolder.Folder_Name == properFolderName)
                        {
                            selectedNodes.Add(folderNode);
                            if (thisFolder.isPublic)
                            {
                                folderNode.ImageUrl = currentMode.Base_URL + "default/images/open_folder_public.jpg";
                                folderNode.ImageToolTip = "Public folder";
                            }
                            else
                            {
                                folderNode.ImageUrl = currentMode.Base_URL + "default/images/open_folder.jpg";
                            }
                            folderNode.Text = "&nbsp; <span class=\"Selected_TreeNode_Text\">" + thisFolder.Folder_Name + "</span>";
                            folderNode.SelectAction = TreeNodeSelectAction.None;
                        }
                        else
                        {
                            if (thisFolder.isPublic)
                            {
                                folderNode.ImageUrl = currentMode.Base_URL + "default/images/closed_folder_public.jpg";
                                folderNode.ImageToolTip = "Public folder";
                            }
                            else
                            {
                                folderNode.ImageUrl = currentMode.Base_URL + "default/images/closed_folder.jpg";
                            }
                            folderNode.Text = "&nbsp; <a href=\"" + redirect_url.Replace("XXXXXXXXXXXXXXXXXX", thisFolder.Folder_Name_Encoded) + "\">" + thisFolder.Folder_Name + "</a>";
                        }
                        rootNode.ChildNodes.Add(folderNode);

                        // Add all the children nodes as well
                        add_children_nodes(folderNode, thisFolder, properFolderName, redirect_url, selectedNodes);
                    }
                }

                // Collapse the treeview
                treeView1.CollapseAll();
                if (selectedNodes.Count > 0)
                {
                    TreeNode selectedNodeExpander = selectedNodes[0];
                    while (selectedNodeExpander.Parent != null) 
                    {
                        (selectedNodeExpander.Parent).Expand();
                        selectedNodeExpander = selectedNodeExpander.Parent;
                    }
                }
                else
                {
                    rootNode.Expand();
                }


                MainPlaceHolder.Controls.Add(treeView1);
            }

            if (currentMode.My_Sobek_SubMode.Length > 0)
            {
                if ( resultsStatistics.Total_Titles == 0)
                {
                    Literal literal = new Literal();

                    string folder_name = user.Folder_Name(currentMode.My_Sobek_SubMode);
                    currentMode.My_Sobek_SubMode = String.Empty;
                    if (folder_name.Length == 0)
                    {
                        currentMode.Redirect();
                        return;
                    }
                    
                    if (properFolderName != "Submitted Items")
						literal.Text = "<br /><br /><h1>" + folder_name + "</h1><br /><br /><div class=\"SobekHomeText\" ><center><b>This bookshelf is currently empty</b></center><br /><br /><br /></div></div>";
                    else
						literal.Text = "<h1>" + folder_name + "</h1><br /><br /><div class=\"SobekHomeText\" ><center><b>This bookshelf is currently empty</b></center><br /><br /><br /></div></div>";
                    MainPlaceHolder.Controls.Add(literal);

                }
                else
                {

                    writeResult = new PagedResults_HtmlSubwriter(resultsStatistics, pagedResults, codeManager, Translator, itemList, user, currentMode, Tracer)
                                      {
                                          Current_Aggregation = currentCollection,
                                          Skin = htmlSkin,
                                          Mode = currentMode,
                                          Browse_Title = properFolderName,
                                          Outer_Form_Name = "itemNavForm"
                                      };
                    if (properFolderName != "Submitted Items")
                        writeResult.Include_Bookshelf_View = true;

                    // Add some space and then the view type selection box
                    StringBuilder view_type_selection_builder = new StringBuilder( 2000);
                    if (properFolderName != "Submitted Items")
                        view_type_selection_builder.AppendLine("<br /><br />");

                    StringWriter writer = new StringWriter(view_type_selection_builder);
                    writeResult.Write_HTML(writer, Tracer);

                    Literal view_type_selection_literal = new Literal {Text = view_type_selection_builder.ToString()};
                    MainPlaceHolder.Controls.Add(view_type_selection_literal);

                    // Now, add the results controls as well
                    writeResult.Add_Controls(MainPlaceHolder, Tracer);

                    // Close the div
                    Literal final_literal = new Literal {Text = "<br />\n"};
                    MainPlaceHolder.Controls.Add(final_literal);
                }

				//if (( pagedResults != null ) && ( pagedResults.Count > 0))
				//{
				//	Literal literal = new Literal();
				//	literal.Text = "<div class=\"sbkPrsw_ResultsNavBar\">" + Environment.NewLine + "  " + writeResult.Buttons + Environment.NewLine + "  " + writeResult.Showing_Text + Environment.NewLine + "</div>" + Environment.NewLine + "<br />" + Environment.NewLine;
				//	MainPlaceHolder.Controls.Add(literal);
				//}
            }
            else
            {
                // Add the folder management piece here
                StringBuilder bookshelfManageBuilder = new StringBuilder();
                bookshelfManageBuilder.AppendLine("<br /><br />\n<h1>Manage My Bookshelves</h1>\n<div class=\"SobekHomeText\" >");
                bookshelfManageBuilder.AppendLine("  <blockquote>");
                bookshelfManageBuilder.AppendLine("  <table width=\"630px\">");
                bookshelfManageBuilder.AppendLine("    <tr valign=\"middle\">");
                bookshelfManageBuilder.AppendLine("      <td align=\"left\" width=\"30px\"><a href=\"?\" id=\"new_bookshelf_link\" name=\"new_bookshelf_link\" onclick=\"return open_new_bookshelf_folder();\" title=\"Click to add a new bookshelf\" ><img title=\"Click to add a new bookshelf\" src=\"" + currentMode.Base_URL + "default/images/new_folder.jpg\" /></a><td>");
                bookshelfManageBuilder.AppendLine("      <td align=\"left\"><a href=\"?\" id=\"new_bookshelf_link\" name=\"new_bookshelf_link\" onclick=\"return open_new_bookshelf_folder();\" title=\"Click to add a new bookshelf\" >Add New Bookshelf</a><td>");
                bookshelfManageBuilder.AppendLine("      <td align=\"right\" width=\"40px\"><a href=\"?\" id=\"refresh_bookshelf_link\" name=\"refresh_bookshelf_link\" onclick=\"return refresh_bookshelves();\" title=\"Refresh bookshelf list\" ><img title=\"Refresh bookshelf list\" src=\"" + currentMode.Base_URL + "default/images/refresh_folder.jpg\" /></a><td>");
                bookshelfManageBuilder.AppendLine("      <td align=\"left\" width=\"150px\"><a href=\"?\" id=\"refresh_bookshelf_link\" name=\"refresh_bookshelf_link\" onclick=\"return refresh_bookshelves();\" title=\"Refresh bookshelf list\" >Refresh Bookshelves</a><td>");
                bookshelfManageBuilder.AppendLine("    </tr>");
                bookshelfManageBuilder.AppendLine("  </table>");
                bookshelfManageBuilder.AppendLine("  <br /><br />");
                bookshelfManageBuilder.AppendLine("  <table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                bookshelfManageBuilder.AppendLine("    <tr align=\"left\" bgcolor=\"#0022a7\" >");
                bookshelfManageBuilder.AppendLine("      <th width=\"210px\" align=\"left\"><span style=\"color: White\"> &nbsp; ACTIONS</span></th>");
                bookshelfManageBuilder.AppendLine("      <th width=\"40px\" align=\"left\">&nbsp;</th>");
                bookshelfManageBuilder.AppendLine("      <th width=\"380px\" align=\"left\"><span style=\"color: White\">BOOKSHELF NAME</span></th>");
                bookshelfManageBuilder.AppendLine("     </tr>");
                bookshelfManageBuilder.AppendLine("    <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");

                // Write the data for each interface
                int folder_number = 1;
                foreach ( User_Folder thisFolder in user.All_Folders )
                {
                    if (thisFolder.Folder_Name != "Submitted Items")
                    {
                        // Build the action links
                        bookshelfManageBuilder.AppendLine("    <tr align=\"left\" valign=\"center\" >");
                        bookshelfManageBuilder.Append("      <td class=\"SobekFolderActionLink\" >( ");
                        if (thisFolder.Child_Count == 0)
                        {
                            if (user.All_Folders.Count == 1)
                            {
                                bookshelfManageBuilder.Append("<a title=\"Click to delete this bookshelf\" id=\"DELETE_" + folder_number + "\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"alert('You cannot delete your last bookshelf');return false;\">delete</a> | ");
                            }
                            else
                            {
                                bookshelfManageBuilder.Append("<a title=\"Click to delete this bookshelf\" id=\"DELETE_" + folder_number + "\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_folder('" + thisFolder.Folder_ID + "');\">delete</a> | ");
                            }
                        }
                        else
                        {
                            bookshelfManageBuilder.Append("<a title=\"Click to delete this bookshelf\" id=\"DELETE_" + folder_number + "\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"alert('You cannot delete bookshelves which contain other bookshelves');return false;\">delete</a> | ");
                        }
                        if (thisFolder.isPublic)
                        {
                            bookshelfManageBuilder.Append("<a title=\"Make this bookshelf private\" id=\"PUBLIC_" + folder_number + "\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return change_folder_visibility('" + thisFolder.Folder_Name_Encoded + "', 'private');\">make private</a> | ");
                        }
                        else
                        {
                            bookshelfManageBuilder.Append("<a title=\"Make this bookshelf public\" id=\"PUBLIC_" + folder_number + "\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return change_folder_visibility('" + thisFolder.Folder_Name_Encoded + "', 'public');\">make public</a> | ");
                        }
                        bookshelfManageBuilder.AppendLine("<a title=\"Click to manage this bookshelf\" href=\"" + redirect_url.Replace("XXXXXXXXXXXXXXXXXX", thisFolder.Folder_Name_Encoded) + "\">manage</a> )</td>");
                        if (thisFolder.isPublic)
                        {
                            currentMode.Mode = Display_Mode_Enum.Public_Folder;
                            currentMode.FolderID = thisFolder.Folder_ID;
                            bookshelfManageBuilder.AppendLine("      <td><a href=\"" + currentMode.Redirect_URL() + "\"><img title=\"Public folder\" src=\"" + currentMode.Base_URL + "default/images/closed_folder_public.jpg\" /><a/></td>");
                            bookshelfManageBuilder.AppendLine("      <td><a href=\"" + currentMode.Redirect_URL() + "\">" + thisFolder.Folder_Name + "</a></td>");
                            currentMode.Mode = Display_Mode_Enum.My_Sobek;
                        }
                        else
                        {
                            bookshelfManageBuilder.AppendLine("      <td><img title=\"Private folder\" src=\"" + currentMode.Base_URL + "default/images/closed_folder.jpg\" /></td>");
                            bookshelfManageBuilder.AppendLine("      <td>" + thisFolder.Folder_Name + "</td>");
                        }
                        bookshelfManageBuilder.AppendLine("     </tr>");
                        bookshelfManageBuilder.AppendLine("    <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");

                        // Increment the folder number
                        folder_number++;
                    }
                }

                bookshelfManageBuilder.AppendLine("  </table>");
                bookshelfManageBuilder.AppendLine("  </blockquote>");
                bookshelfManageBuilder.AppendLine("</div>");

                // Add this as a literal
                Literal mgmtLiteral = new Literal {Text = bookshelfManageBuilder.ToString()};
                MainPlaceHolder.Controls.Add(mgmtLiteral);
            }
        }

        private void add_children_nodes(TreeNode ParentNode, User_Folder ThisFolder, string SelectedFolder, string RedirectURL, List<TreeNode> SelectedNodes )
        {
            foreach (User_Folder childFolders in ThisFolder.Children)
            {
                TreeNode folderNode = new TreeNode
                                          { Text ="&nbsp; <a href=\"" +RedirectURL.Replace("XXXXXXXXXXXXXXXXXX",childFolders.Folder_Name_Encoded) + "\">" +childFolders.Folder_Name + "</a>" };
                if (childFolders.Folder_Name == SelectedFolder)
                {
                    SelectedNodes.Add(folderNode);
                    if (childFolders.isPublic)
                    {
                        folderNode.ImageUrl = currentMode.Base_URL + "default/images/open_folder_public.jpg";
                        folderNode.ImageToolTip = "Public folder";
                    }
                    else
                    {
                        folderNode.ImageUrl = currentMode.Base_URL + "default/images/open_folder.jpg";
                    }

                    folderNode.Text = "&nbsp; <span class=\"Selected_TreeNode_Text\">" + childFolders.Folder_Name + "</span>";
                    folderNode.SelectAction = TreeNodeSelectAction.None;
                }
                else
                {
                    if (childFolders.isPublic)
                    {
                        folderNode.ImageUrl = currentMode.Base_URL + "default/images/closed_folder_public.jpg";
                        folderNode.ImageToolTip = "Public folder";
                    }
                    else
                    {
                        folderNode.ImageUrl = currentMode.Base_URL + "default/images/closed_folder.jpg";
                    }
                    folderNode.Text = "&nbsp; <a href=\"" + RedirectURL.Replace("XXXXXXXXXXXXXXXXXX", childFolders.Folder_Name_Encoded) + "\">" + childFolders.Folder_Name + "</a>";
                }
                ParentNode.ChildNodes.Add(folderNode);

                // Add all the children nodes as well
                add_children_nodes(folderNode, childFolders, properFolderName, RedirectURL, SelectedNodes);
            }
        }
    }
}
  


