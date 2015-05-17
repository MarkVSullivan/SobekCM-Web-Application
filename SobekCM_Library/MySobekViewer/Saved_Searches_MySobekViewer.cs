#region Using directives

using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Web;
using SobekCM.Core.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Tools;

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
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to display the item for editing</li>
    /// </ul></remarks>
    public class Saved_Searches_MySobekViewer : abstract_MySobekViewer
    {
        /// <summary> Constructor for a new instance of the Saved_Searches_MySobekViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Saved_Searches_MySobekViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("Saved_Searches_MySobekViewer.Constructor", String.Empty);

            if (RequestSpecificValues.Current_Mode.isPostBack)
            {
                // Pull the standard values
                NameValueCollection form = HttpContext.Current.Request.Form;

                string item_action = form["item_action"].ToUpper().Trim();
                string folder_id = form["folder_id"].Trim();

                if (item_action == "REMOVE")
                {
                    int folder_id_int;
                    if (Int32.TryParse(folder_id, out folder_id_int))
                        SobekCM_Database.Delete_User_Search(folder_id_int, RequestSpecificValues.Tracer);
                }

                HttpContext.Current.Response.Redirect(HttpContext.Current.Items["Original_URL"].ToString(), false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                RequestSpecificValues.Current_Mode.Request_Completed = true;
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'Folder Management' </value>
        public override string Web_Title
        {
            get { return "My Saved Searches"; }
        }

        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            // Do nothing
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area with the form </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Saved_Searches_MySobekViewer.Write_HTML", String.Empty);
            DataTable searchesTable = SobekCM_Database.Get_User_Searches(RequestSpecificValues.Current_User.UserID, Tracer);


			Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
			Output.WriteLine("<input type=\"hidden\" id=\"item_action\" name=\"item_action\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"folder_id\" name=\"folder_id\" value=\"\" />");

			Output.WriteLine("<h1>" + Web_Title + "</h1>");
			Output.WriteLine();

			Output.WriteLine("<div class=\"SobekHomeText\" >");
			if (searchesTable.Rows.Count > 0)
			{

				Output.WriteLine("  <blockquote>");
				Output.WriteLine("  <table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
				Output.WriteLine("    <tr align=\"left\" bgcolor=\"#0022a7\" >");
				Output.WriteLine("      <th width=\"120px\" align=\"left\"><span style=\"color: White\"> &nbsp; ACTIONS</span></th>");
				Output.WriteLine("      <th width=\"480px\" align=\"left\"><span style=\"color: White\">SAVED SEARCH</span></th>");
				Output.WriteLine("     </tr>");
				Output.WriteLine("    <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");



				// Write the data for each interface
				foreach (DataRow thisRow in searchesTable.Rows)
				{
					int usersearchid = Convert.ToInt32(thisRow["UserSearchID"]);
					string search_url = thisRow["SearchURL"].ToString();
					string search_desc = thisRow["UserNotes"].ToString();

					// Build the action links
					Output.WriteLine("    <tr align=\"left\" valign=\"center\" >");
					Output.Write("      <td class=\"SobekFolderActionLink\" >( ");
					Output.Write("<a title=\"Click to delete this saved search\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_search('" + usersearchid + "');\">delete</a> | ");
					Output.WriteLine("<a title=\"Click to view this search\" href=\"" + search_url + "\">view</a> )</td>");
					Output.WriteLine("      <td><a href=\"" + search_url + "\">" + search_desc + "</a></td>");
					Output.WriteLine("     </tr>");
					Output.WriteLine("    <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
				}

				Output.WriteLine("  </table>");
				Output.WriteLine("  </blockquote>");

			}
			else
			{
				Output.WriteLine("<blockquote>You do not have any saved searches or browses.<br /><br />To add a search or browse, use the ADD button while viewing the results of your search or browse.</blockquote><br />");
			}
			Output.WriteLine("</div>");
        }
    }
}
  


