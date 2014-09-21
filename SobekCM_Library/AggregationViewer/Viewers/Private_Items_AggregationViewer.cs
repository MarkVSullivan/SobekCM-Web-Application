#region Using directives

using System;
using System.IO;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Renders the usage statistics for a single aggregation </summary>
    /// <remarks> This class implements the <see cref="iAggregationViewer"/> interface and extends the <see cref="abstractAggregationViewer"/> class.<br /><br />
    /// Collection viewers are used when displaying collection home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request to display the usage statistics page, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in this case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
    /// <li>To display the requested collection view, the collection subwriter will creates an instance of this class </li>
    /// </ul></remarks>
    public class Private_Items_AggregationViewer : abstractAggregationViewer
    {
        private const decimal RESULTS_PER_PAGE = 20;
        private readonly Private_Items_List privateItems;

        /// <summary> Constructor for a new instance of the Private_Items_AggregationViewer class </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Current_Aggregation"> Current item aggregation object to display </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Private_Items_AggregationViewer(SobekCM_Navigation_Object Current_Mode, Item_Aggregation Current_Aggregation, Custom_Tracer Tracer): base(Current_Aggregation, Current_Mode)
        {
            privateItems = SobekCM_Database.Tracking_Get_Aggregation_Private_Items(currentCollection.Code, (int) RESULTS_PER_PAGE, currentMode.Page, currentMode.Sort, Tracer);
        }

        /// <summary> Gets flag which indicates whether to always use the home text as the secondary text </summary>
        public override bool Always_Display_Home_Text
        {
            get
            {
                return false;
            }
        }

        /// <summary>Flag indicates whether the subaggregation selection panel is displayed for this collection viewer</summary>
        /// <value> This property always returns the <see cref="Selection_Panel_Display_Enum.Never"/> enumerational value </value>
        public override Selection_Panel_Display_Enum Selection_Panel_Display
        {
            get
            {
                return Selection_Panel_Display_Enum.Never;
            }
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation.CollectionViewsAndSearchesEnum.View_Private_Items"/> enumerational value </value>
        public override Item_Aggregation.CollectionViewsAndSearchesEnum Type
        {
            get { return Item_Aggregation.CollectionViewsAndSearchesEnum.View_Private_Items; }
        }

        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds the title of the into the box </remarks>
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            // Get the URL for the sort options
            short sort = currentMode.Sort;
            currentMode.Sort = 0;
            string url = currentMode.Redirect_URL();
            currentMode.Sort = sort;

            Output.WriteLine("<span class=\"SobekResultsSort\">");
            Output.WriteLine("    " + translator.Get_Translation("Sort By", currentMode.Language) + ": &nbsp;");
            Output.WriteLine("    <select name=\"sorter_input\" onchange=\"javascript:sort_private_list('" + url + "')\" id=\"sorter_input\">");
            if (currentMode.Sort == 0)
            {
                Output.WriteLine("      <option value=\"0\" selected=\"selected\">" + translator.Get_Translation("BibID / VID", currentMode.Language) + "</option>" + Environment.NewLine );
            }
            else
            {
                Output.WriteLine("      <option value=\"0\">" + translator.Get_Translation("BibID / VID", currentMode.Language) + "</option>" + Environment.NewLine );
            }

            if (currentMode.Sort == 1)
            {
                Output.WriteLine("      <option value=\"1\" selected=\"selected\">" + translator.Get_Translation("Title / VID", currentMode.Language) + "</option>" + Environment.NewLine );
            }
            else
            {
                Output.WriteLine("      <option value=\"1\">" + translator.Get_Translation("Title / VID", currentMode.Language) + "</option>" + Environment.NewLine );
            }

            if (currentMode.Sort == 2)
            {
                Output.WriteLine("      <option value=\"2\" selected=\"selected\">" + translator.Get_Translation("Last Activity Date (most recent first)", currentMode.Language) + "</option>" + Environment.NewLine );
            }
            else
            {
                Output.WriteLine("      <option value=\"2\">" + translator.Get_Translation("Last Activity Date (most recent first)", currentMode.Language) + "</option>" + Environment.NewLine );
            }

            if (currentMode.Sort == 3)
            {
                Output.WriteLine("      <option value=\"3\" selected=\"selected\">" + translator.Get_Translation("Last Milestone Date (most recent first)", currentMode.Language) + "</option>" + Environment.NewLine );
            }
            else
            {
                Output.WriteLine("      <option value=\"3\">" + translator.Get_Translation("Last Milestone Date (most recent first)", currentMode.Language) + "</option>" + Environment.NewLine );
            }

            if (currentMode.Sort == 4)
            {
                Output.WriteLine("      <option value=\"4\" selected=\"selected\">" + translator.Get_Translation("Last Activity Date (oldest first)", currentMode.Language) + "</option>" + Environment.NewLine );
            }
            else
            {
                Output.WriteLine("      <option value=\"4\">" + translator.Get_Translation("Last Activity Date (oldest first)", currentMode.Language) + "</option>" + Environment.NewLine );
            }

            if (currentMode.Sort == 5)
            {
                Output.WriteLine("      <option value=\"5\" selected=\"selected\">" + translator.Get_Translation("Last Milestone Date (oldest first)", currentMode.Language) + "</option>" + Environment.NewLine );
            }
            else
            {
                Output.WriteLine("      <option value=\"5\">" + translator.Get_Translation("Last Milestone Date (oldest first)", currentMode.Language) + "</option>" + Environment.NewLine );
            }
            Output.WriteLine("    </select>");
            Output.WriteLine("</span>");
            Output.WriteLine("<h1>Private and Dark Items</h1>");

            // Should buttons be added here for additional pages?
            if (privateItems.Total_Titles > RESULTS_PER_PAGE)
            {
                // Get the language suffix for the buttons
                string language_suffix = currentMode.Language_Code;
                if (language_suffix.Length > 0)
                    language_suffix = "_" + language_suffix;

                // Get the text for the buttons
                string first_page = "First Page";
                string previous_page = "Previous Page";
                string next_page = "Next Page";
                string last_page = "Last Page";

                if (currentMode.Language == Web_Language_Enum.Spanish)
                {
                    first_page = "Primera Página";
                    previous_page = "Página Anterior";
                    next_page = "Página Siguiente";
                    last_page = "Última Página";
                }

                if (currentMode.Language == Web_Language_Enum.French)
                {
                    first_page = "Première Page";
                    previous_page = "Page Précédente";
                    next_page = "Page Suivante";
                    last_page = "Dernière Page";
                }

                // Get the current page
                ushort current_page = currentMode.Page;

                // Should the previous and first buttons be enabled?
                Output.WriteLine("  <span class=\"leftButtons\">");
                if (current_page > 1)
                {
                    currentMode.Page = 1;
                    Output.WriteLine("    &nbsp; &nbsp; &nbsp; <a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/first_button" + language_suffix + ".gif\" border=\"0\" alt=\"" + first_page + "\" /></a> &nbsp; ");
                    currentMode.Page = (ushort)(current_page - 1);
                    Output.WriteLine("    <a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/previous_button" + language_suffix + ".gif\" border=\"0\" alt=\"" + previous_page + "\" /></a>");
                }
                else
                {
                    Output.WriteLine("    <img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/no_button_spacer.gif\" />");
                }
                Output.WriteLine("  </span>");

                // Calculate the maximum number of pages
                ushort pages = (ushort)(Math.Ceiling(privateItems.Total_Titles / RESULTS_PER_PAGE));


                // Should the next and last buttons be enabled?
                Output.WriteLine("  <span class=\"rightButtons\">");
                if (current_page < pages)
                {
                    currentMode.Page = (ushort)(current_page + 1);
                    Output.WriteLine("    <a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/next_button" + language_suffix + ".gif\" border=\"0\" alt=\"" + next_page + "\" /></a>&nbsp;");
                    currentMode.Page = pages;
                    Output.WriteLine("    <a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/last_button" + language_suffix + ".gif\" border=\"0\" alt=\"" + last_page + "\" /></a> &nbsp; &nbsp; &nbsp; ");
                }
                else
                {
                    Output.WriteLine("    <img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/no_button_spacer.gif\" />");
                }
                Output.WriteLine("  </span>");
                currentMode.Page = current_page;

                Output.WriteLine("<br />");
            }

            Output.WriteLine("<br />");
        }

        /// <summary> Add the HTML to be displayed below the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This writes the HTML from the static browse or info page here  </remarks>
        public override void Add_Secondary_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Private_Items_AggregationViewer.Add_Secondary_HTML", "Adding HTML");
            }

            if (privateItems == null)
            {
                Output.WriteLine("<div class=\"SobekText\">");
                Output.WriteLine("<p><strong>ERROR PULLING INFORMATION FROM DATABASE</strong></p>");
                Output.WriteLine("</div>");
                return;
            }

            if (privateItems.Total_Items == 0)
            {
                Output.WriteLine("<div class=\"SobekText\">");
                Output.WriteLine("<br />");
                Output.WriteLine("<p><strong>This collection does not include any PRIVATE or DARK items.</strong></p>");
                Output.WriteLine("<br />");
                Output.WriteLine("</div>");
                return;
            }

            Output.WriteLine("<div class=\"SobekText\">");
            Output.WriteLine("<br />");
            Output.WriteLine("<p>Below is the list of all items linked to this aggregation which are either private (in process) or dark.</p>");
            if (privateItems.Total_Items == 1)
                Output.WriteLine("<p>There is only one matching item.</p>");
            else
            {
                if (privateItems.Total_Titles == privateItems.Total_Items)
                {
                    Output.WriteLine("<p>There are a total of " + privateItems.Total_Items + " titles.</p>");
                }
                else
                {
                    Output.Write("<p>There are a total of " + privateItems.Total_Items + " items in ");
                    if (privateItems.Total_Titles == 1)
                        Output.WriteLine("one title.</p>");
                    else
                        Output.WriteLine(privateItems.Total_Titles + " titles.</p>");
                }
            }

            Output.WriteLine("<br />");
            Output.WriteLine("</div>");
           
            // Start the table to display
            Output.WriteLine("</div>");
            Output.WriteLine("<center>");

            // Start the table and add the header
            Output.WriteLine("<table width=\"1100px\" border=\"0px\" cellspacing=\"0px\" class=\"privateTable\">");
            Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\">");
            Output.WriteLine("    <th align=\"left\" colspan=\"2\"><span style=\"color: White\"> &nbsp;<b>TITLE</b></span></th>");
            Output.WriteLine("    <th align=\"center\"><span style=\"color: White\"><b>LAST ACTIVITY</b></span></th>");
            Output.WriteLine("    <th align=\"center\"><span style=\"color: White\"><b>LAST MILESTONE</b></span></th>");
            Output.WriteLine("  </tr>");

            // Draw each title/item in this page of results
            foreach (Private_Items_List_Title thisTitle in privateItems.Title_Results)
            {
                // Is this a single item, or are there multiple items under this title?
                if (thisTitle.Item_Count > 1)
                {
                    // Draw the title row individually first
                    Output.WriteLine("  <tr align=\"left\" onmouseover=\"this.className='tableRowHighlight'\" onmouseout=\"this.className='tableRowNormal'\" onmousedown=\"window.location.href='" + currentMode.Base_URL + thisTitle.BibID + "'\">");
                    Output.WriteLine("    <td width=\"700px\" colspan=\"2\">");
                    Output.WriteLine("      <table>");
                    Output.WriteLine("        <tr><td colspan=\"3\"><a href=\"" + currentMode.Base_URL + thisTitle.BibID + "\"><span class=\"privateTableBibTitle\">" + thisTitle.Group_Title + "</span></a></td></tr>");
                    Output.WriteLine("        <tr><td width=\"180px\"><a href=\"" + currentMode.Base_URL + thisTitle.BibID + "\">" + thisTitle.BibID + "</a></td><td width=\"180px\">" + thisTitle.Type + "</td><td>( " + thisTitle.Item_Count + " volumes out of " + thisTitle.Complete_Item_Count + " total volumes )</td></tr>");
                    Output.WriteLine("      </table>");
                    Output.WriteLine("    </td>");
                    Output.WriteLine("    <td align=\"center\" width=\"200px\"><span class=\"privateTableBibDaysNumber\">" + Math.Floor( DateTime.Now.Subtract( thisTitle.Last_Activity_Date ).TotalDays ) + "</span><br />days ago</td>");
                    Output.WriteLine("    <td align=\"center\" width=\"200px\"><span class=\"privateTableBibDaysNumber\">" + Math.Floor(DateTime.Now.Subtract(thisTitle.Last_Milestone_Date).TotalDays) + "</span><br />days ago</td>");
                    Output.WriteLine("  </tr>");

                    // Now, draw each item row
                    foreach (Private_Items_List_Item thisItem in thisTitle.Items)
                    {
                        Output.WriteLine("  <tr><td width=\"125px\"></td><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");
                        Output.WriteLine("  <tr align=\"left\" onmouseover=\"this.className='tableRowHighlight'\" onmouseout=\"this.className='tableRowNormal'\" onmousedown=\"window.location.href='" + currentMode.Base_URL + thisTitle.BibID + "/" + thisItem.VID + "'\">");
                        Output.WriteLine("    <td width=\"125px\">");
                        Output.WriteLine("    <td width=\"575px\">");
                        Output.WriteLine("      <table>");
                        Output.WriteLine("        <tr><td colspan=\"3\"><a href=\"" + currentMode.Base_URL + thisTitle.BibID + "/" + thisItem.VID + "\"><span class=\"privateTableItemTitle\">" + thisItem.Title + "</span></a></td></tr>");
                        Output.Write("        <tr><td width=\"180px\"><a href=\"" + currentMode.Base_URL + thisTitle.BibID + "/" + thisItem.VID + "\">" + thisTitle.BibID + " : " + thisItem.VID + "</a></td>");
                        if (thisItem.PubDate.Length > 0)
                            Output.WriteLine("<td>Dated " + thisItem.PubDate + "</td><td></td></tr>");
                        else
                            Output.WriteLine("<td></td><td></td></tr>");
                        if (thisItem.Internal_Comments.Length > 0)
                            Output.WriteLine("              <tr><td colspan=\"3\">&ldquo;" + thisItem.Internal_Comments + "&rdquo;</td></tr>");
                        Output.WriteLine("      </table>");
                        Output.WriteLine("    </td>");
                        Output.WriteLine("    <td align=\"center\" width=\"200px\">" + thisItem.Last_Activity_Type.ToLower() + "<br /><span class=\"privateTableItemDaysNumber\">" + Math.Floor( DateTime.Now.Subtract( thisItem.Last_Activity_Date ).TotalDays ) + "</span><br />days ago</td>");
                        Output.WriteLine("    <td align=\"center\" width=\"200px\">" + thisItem.Last_Milestone_String + "<br /><span class=\"privateTableItemDaysNumber\">" + Math.Floor(DateTime.Now.Subtract(thisItem.Last_Milestone_Date).TotalDays) + "</span><br />days ago</td>");
                        Output.WriteLine("  </tr>");                    }
                }
                else
                {
                    // Get the single item
                    Private_Items_List_Item thisItem = thisTitle.Items[0];

                    // Draw the integrated title/item row
                    Output.WriteLine("  <tr align=\"left\" onmouseover=\"this.className='tableRowHighlight'\" onmouseout=\"this.className='tableRowNormal'\" onmousedown=\"window.location.href='" + currentMode.Base_URL + thisTitle.BibID + "/" + thisItem.VID + "'\">");
                    Output.WriteLine("      <td width=\"700px\" colspan=\"2\">");
                    Output.WriteLine("          <table>");
                    Output.WriteLine("              <tr><td colspan=\"3\"><a href=\"" + currentMode.Base_URL + thisTitle.BibID + "/" + thisItem.VID + "\"><span class=\"privateTableBibTitle\">" + thisItem.Title + "</span></a></td></tr>");
                    Output.Write("              <tr><td width=\"180px\"><a href=\"" + currentMode.Base_URL + thisTitle.BibID + "/" + thisItem.VID + "\">" + thisTitle.BibID + " : " + thisItem.VID + "</a></td><td width=\"180px\">" + thisTitle.Type + "</td>");
                    if ( thisItem.PubDate.Length > 0 )
                        Output.WriteLine("<td>Dated " + thisItem.PubDate + "</td></tr>");
                    else
                        Output.WriteLine("<td></td></tr>");
                    if ( thisItem.Internal_Comments.Length > 0 )
                        Output.WriteLine("              <tr><td colspan=\"3\">&ldquo;" + thisItem.Internal_Comments + "&rdquo;</td></tr>");
                    Output.WriteLine("          </table>");
                    Output.WriteLine("      </td>");
                    Output.WriteLine("      <td align=\"center\" width=\"200px\">" + thisItem.Last_Activity_Type.ToLower() + "<br /><span class=\"privateTableItemDaysNumber\">" + Math.Floor(DateTime.Now.Subtract(thisItem.Last_Activity_Date).TotalDays) + "</span><br />days ago</td>");
                    Output.WriteLine("      <td align=\"center\" width=\"200px\">" + thisItem.Last_Milestone_String + "<br /><span class=\"privateTableItemDaysNumber\">" + Math.Floor(DateTime.Now.Subtract(thisItem.Last_Milestone_Date).TotalDays) + "</span><br />days ago</td>");
                    Output.WriteLine("    </tr>");

                }

                // Add a final line in the table
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>");
            }

            // End the table
            Output.WriteLine("</table>");

            Output.WriteLine("</center>");
            Output.WriteLine("<div id=\"pagecontainer_resumed\">");
            Output.WriteLine("<br /> <br />");

        }
    }
}
