#region Using directives

using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Library.Application_State;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.MySobekViewer
{

    /// <summary> Class allows an authenticated user to view their saved searches </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to display the item for editing</li>
    /// </ul></remarks>
    public class Saved_Searches_MySobekViewer : abstract_MySobekViewer
    {
        /// <summary> Constructor for a new instance of the Saved_Searches_MySobekViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Translator"> Translation / language support object for writing the user interface is multiple languages</param>
        /// <param name="currentMode"> Mode / navigation information for the current request</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Saved_Searches_MySobekViewer(User_Object User, 
            Language_Support_Info Translator, 
            SobekCM_Navigation_Object currentMode,
            Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Saved_Searches_MySobekViewer.Constructor", String.Empty);

            user = User;
            base.Translator = Translator;

            if (currentMode.isPostBack)
            {
                // Pull the standard values
                NameValueCollection form = HttpContext.Current.Request.Form;

                string item_action = form["item_action"].ToUpper().Trim();
                string folder_id = form["folder_id"].Trim();

                if (item_action == "REMOVE")
                {
                    int folder_id_int;
                    if (Int32.TryParse(folder_id, out folder_id_int))
                        SobekCM_Database.Delete_User_Search(folder_id_int, Tracer);
                }

                HttpContext.Current.Response.Redirect(HttpContext.Current.Items["Original_URL"].ToString(), false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                currentMode.Request_Completed = true;
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'Folder Management' </value>
        public override string Web_Title
        {
            get { return "My Saved Searches"; }
        }

        /// <summary> Property indicates if this mySobek Viewer can contain pop-up forms</summary>
        /// <remarks> If the mySobek viewer contains pop-up forms the overall page renders differently, 
        /// allowing for the blanket division and the popup forms near the top of the rendered HTML </remarks>
        ///<value> This mySobek viewer always returns the value TRUE </value>
        public override bool Contains_Popup_Forms
        {
            get
            {
                return false;
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Saved_Searches_MySobekViewer.Write_HTML", String.Empty);

            
        }

		/// <summary> Add controls directly to the form in the main control area placeholder </summary>
        /// <param name="MainPlaceHolder"> Main place holder to which all main controls are added </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> The <see cref="PagedResults_HtmlSubwriter"/> class is instantiated and adds controls to the placeholder here </remarks>
        public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Saved_Searches_MySobekViewer.Add_Controls", String.Empty);
            DataTable searchesTable = SobekCM_Database.Get_User_Searches(user.UserID, Tracer);

            StringBuilder saveSearchBuilder = new StringBuilder(1000);

            saveSearchBuilder.AppendLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            saveSearchBuilder.AppendLine("<input type=\"hidden\" id=\"item_action\" name=\"item_action\" value=\"\" />");
            saveSearchBuilder.AppendLine("<input type=\"hidden\" id=\"folder_id\" name=\"folder_id\" value=\"\" />");

            saveSearchBuilder.AppendLine("<div class=\"SobekHomeText\" >");
            if (searchesTable.Rows.Count > 0)
            {

                saveSearchBuilder.AppendLine("  <blockquote>");
                saveSearchBuilder.AppendLine("  <table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                saveSearchBuilder.AppendLine("    <tr align=\"left\" bgcolor=\"#0022a7\" >");
                saveSearchBuilder.AppendLine("      <th width=\"120px\" align=\"left\"><span style=\"color: White\"> &nbsp; ACTIONS</span></th>");
                saveSearchBuilder.AppendLine("      <th width=\"480px\" align=\"left\"><span style=\"color: White\">SAVED SEARCH</span></th>");
                saveSearchBuilder.AppendLine("     </tr>");
                saveSearchBuilder.AppendLine("    <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");



                // Write the data for each interface
                foreach (DataRow thisRow in searchesTable.Rows)
                {
                    int usersearchid = Convert.ToInt32(thisRow["UserSearchID"]);
                    string search_url = thisRow["SearchURL"].ToString();
                    string search_desc = thisRow["UserNotes"].ToString();

                    // Build the action links
                    saveSearchBuilder.AppendLine("    <tr align=\"left\" valign=\"center\" >");
                    saveSearchBuilder.Append("      <td class=\"SobekFolderActionLink\" >( ");
                    saveSearchBuilder.Append("<a title=\"Click to delete this saved search\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_search('" + usersearchid + "');\">delete</a> | ");
                    saveSearchBuilder.AppendLine("<a title=\"Click to view this search\" href=\"" + search_url + "\">view</a> )</td>");
                    saveSearchBuilder.AppendLine("      <td><a href=\"" + search_url + "\">" + search_desc + "</a></td>");
                    saveSearchBuilder.AppendLine("     </tr>");
                    saveSearchBuilder.AppendLine("    <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                }

                saveSearchBuilder.AppendLine("  </table>");
                saveSearchBuilder.AppendLine("  </blockquote>");

            }
            else
            {
                saveSearchBuilder.AppendLine("<blockquote>You do not have any saved searches or browses.<br /><br />To add a search or browse, use the ADD button while viewing the results of your search or browse.</blockquote><br />");
            }
            saveSearchBuilder.AppendLine("</div>");

            // Add this as a literal
            Literal mgmtLiteral = new Literal {Text = saveSearchBuilder.ToString()};
            MainPlaceHolder.Controls.Add(mgmtLiteral);
        }
    }
}
  


