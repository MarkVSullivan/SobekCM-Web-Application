#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Navigation;
using SobekCM.Core.Results;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Renders the usage statistics for a single aggregation </summary>
    /// <remarks> This class implements the <see cref="iAggregationViewer"/> interface and extends the <see cref="abstractAggregationViewer"/> class.<br /><br />
    /// Collection viewers are used when displaying collection home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request to display the usage statistics page, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the Application_State_Builder </li>
    /// <li>Request is analyzed by the <see cref="QueryString_Analyzer"/> and output as a <see cref="Navigation_Object"/>  </li>
    /// <li>Main writer is created for rendering the output, in this case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
    /// <li>To display the requested collection view, the collection subwriter will creates an instance of this class </li>
    /// </ul></remarks>
    public class Private_Items_AggregationViewer : abstractAggregationViewer
    {
        private const decimal RESULTS_PER_PAGE = 20;
        private readonly Private_Items_List privateItems;

        /// <summary> Constructor for a new instance of the Private_Items_AggregationViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="ViewBag"> Aggregation-specific request information, such as aggregation object and any browse object requested </param>
        public Private_Items_AggregationViewer(RequestCache RequestSpecificValues, AggregationViewBag ViewBag)
            : base(RequestSpecificValues, ViewBag)
        {
            // Ensure user has some permissions on this aggregation, or is a power/internal user or admin before showing
            // them this list
            if ((RequestSpecificValues.Current_User == null) || (!RequestSpecificValues.Current_User.LoggedOn))
            {
                RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // Does this user have permissions on this itgem
            if (RequestSpecificValues.Current_User.PermissionedAggregations != null)
            {
                // Do they have some special permissions against this aggregation?
                bool special_permissions_found = false;
                foreach (User_Permissioned_Aggregation permissions in RequestSpecificValues.Current_User.PermissionedAggregations)
                {
                    if (String.Compare(permissions.Code, ViewBag.Hierarchy_Object.Code, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if ((permissions.CanChangeVisibility) || (permissions.CanDelete) || (permissions.CanEditBehaviors) || (permissions.CanEditItems) ||
                            (permissions.CanEditMetadata) || (permissions.CanPerformQc) || (permissions.CanUploadFiles) || (permissions.IsAdmin) || (permissions.IsCurator))
                        {
                            special_permissions_found = true;
                            break;
                        }
                    }
                }

                // Are they a portal/system admin or power user?
                if ((!special_permissions_found) && ((RequestSpecificValues.Current_User.Is_Internal_User) || (RequestSpecificValues.Current_User.Is_Portal_Admin) || (RequestSpecificValues.Current_User.Is_System_Admin)))
                    special_permissions_found = true;

                // If no permissions, forward them back
                if (!special_permissions_found)
                {
                    RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                    UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                    return;
                }
            }
            else
            {
                // Are they a portal/system admin or power user?
                if ((!RequestSpecificValues.Current_User.Is_Internal_User) && (!RequestSpecificValues.Current_User.Is_Portal_Admin) && (!RequestSpecificValues.Current_User.Is_System_Admin))
                {
                    RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                    UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                    return;
                }
            }

            // Get the list of private items
            int current_sort = RequestSpecificValues.Current_Mode.Sort.HasValue ? RequestSpecificValues.Current_Mode.Sort.Value : 0;
            int current_page = RequestSpecificValues.Current_Mode.Page.HasValue ? RequestSpecificValues.Current_Mode.Page.Value : 1;
            privateItems = SobekCM_Database.Tracking_Get_Aggregation_Private_Items(ViewBag.Hierarchy_Object.Code, (int)RESULTS_PER_PAGE, current_page, current_sort, RequestSpecificValues.Tracer);
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

        /// <summary> Gets the collection of special behaviors which this aggregation viewer  requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> AggregationViewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum>
                        {
                            HtmlSubwriter_Behaviors_Enum.Aggregation_Suppress_Home_Text
                        };
            }
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation_Views_Searches_Enum.View_Private_Items"/> enumerational value </value>
        public override Item_Aggregation_Views_Searches_Enum Type
        {
            get { return Item_Aggregation_Views_Searches_Enum.View_Private_Items; }
        }

        /// <summary> Gets flag which indicates whether this is an internal view, which may have a 
        /// slightly different design feel </summary>
        /// <remarks> This returns FALSE by default, but can be overriden by individual viewer implementations</remarks>
        public override bool Is_Internal_View
        {
            get { return true; }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        public override string Viewer_Title
        {
            get { return "Private and Dark Items"; }
        }

        /// <summary> Gets the URL for the icon related to this aggregational viewer task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources.Private_Items_Img; }
        }

        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This does nothing - as an internal type view, this will not be called </remarks>
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            // Do nothing
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

            if (privateItems.TotalItems == 0)
            {
                Output.WriteLine("<div class=\"SobekText\">");
                Output.WriteLine("<br />");
                Output.WriteLine("<p><strong>This collection does not include any PRIVATE or DARK items.</strong></p>");
                Output.WriteLine("<br />");
                Output.WriteLine("</div>");
                return;
            }


            // Get the URL for the sort options
            short sort = RequestSpecificValues.Current_Mode.Sort.HasValue ? RequestSpecificValues.Current_Mode.Sort.Value : ((short) 0);
            RequestSpecificValues.Current_Mode.Sort = 0;
            string url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.Sort = sort;

            if (privateItems.TotalItems > 0)
            {
                Output.WriteLine("<span class=\"SobekResultsSort\">");
                Output.WriteLine("    " + UI_ApplicationCache_Gateway.Translation.Get_Translation("Sort By", RequestSpecificValues.Current_Mode.Language) + ": &nbsp;");
                Output.WriteLine("    <select name=\"sorter_input\" onchange=\"javascript:sort_private_list('" + url + "')\" id=\"sorter_input\">");
                if (RequestSpecificValues.Current_Mode.Sort == 0)
                {
                    Output.WriteLine("      <option value=\"0\" selected=\"selected\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation("BibID / VID", RequestSpecificValues.Current_Mode.Language) + "</option>" + Environment.NewLine);
                }
                else
                {
                    Output.WriteLine("      <option value=\"0\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation("BibID / VID", RequestSpecificValues.Current_Mode.Language) + "</option>" + Environment.NewLine);
                }

                if (RequestSpecificValues.Current_Mode.Sort == 1)
                {
                    Output.WriteLine("      <option value=\"1\" selected=\"selected\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Title / VID", RequestSpecificValues.Current_Mode.Language) + "</option>" + Environment.NewLine);
                }
                else
                {
                    Output.WriteLine("      <option value=\"1\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Title / VID", RequestSpecificValues.Current_Mode.Language) + "</option>" + Environment.NewLine);
                }

                if (RequestSpecificValues.Current_Mode.Sort == 2)
                {
                    Output.WriteLine("      <option value=\"2\" selected=\"selected\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Last Activity Date (most recent first)", RequestSpecificValues.Current_Mode.Language) + "</option>" + Environment.NewLine);
                }
                else
                {
                    Output.WriteLine("      <option value=\"2\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Last Activity Date (most recent first)", RequestSpecificValues.Current_Mode.Language) + "</option>" + Environment.NewLine);
                }

                if (RequestSpecificValues.Current_Mode.Sort == 3)
                {
                    Output.WriteLine("      <option value=\"3\" selected=\"selected\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Last Milestone Date (most recent first)", RequestSpecificValues.Current_Mode.Language) + "</option>" + Environment.NewLine);
                }
                else
                {
                    Output.WriteLine("      <option value=\"3\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Last Milestone Date (most recent first)", RequestSpecificValues.Current_Mode.Language) + "</option>" + Environment.NewLine);
                }

                if (RequestSpecificValues.Current_Mode.Sort == 4)
                {
                    Output.WriteLine("      <option value=\"4\" selected=\"selected\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Last Activity Date (oldest first)", RequestSpecificValues.Current_Mode.Language) + "</option>" + Environment.NewLine);
                }
                else
                {
                    Output.WriteLine("      <option value=\"4\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Last Activity Date (oldest first)", RequestSpecificValues.Current_Mode.Language) + "</option>" + Environment.NewLine);
                }

                if (RequestSpecificValues.Current_Mode.Sort == 5)
                {
                    Output.WriteLine("      <option value=\"5\" selected=\"selected\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Last Milestone Date (oldest first)", RequestSpecificValues.Current_Mode.Language) + "</option>" + Environment.NewLine);
                }
                else
                {
                    Output.WriteLine("      <option value=\"5\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Last Milestone Date (oldest first)", RequestSpecificValues.Current_Mode.Language) + "</option>" + Environment.NewLine);
                }

                if (RequestSpecificValues.Current_Mode.Sort == 6)
                {
                    Output.WriteLine("      <option value=\"6\" selected=\"selected\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Embargo Date", RequestSpecificValues.Current_Mode.Language) + "</option>" + Environment.NewLine);
                }
                else
                {
                    Output.WriteLine("      <option value=\"6\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Embargo Date", RequestSpecificValues.Current_Mode.Language) + "</option>" + Environment.NewLine);
                }
                Output.WriteLine("    </select>");
                Output.WriteLine("</span>");
            }

            const string EMBARGO_DATE_STRING = "Embargoed until {0}";




            Output.WriteLine("<div class=\"SobekText\">");
            Output.WriteLine("<br />");
            Output.WriteLine("<p>Below is the list of all items linked to this aggregation which are either private (in process) or dark.</p>");
            if (privateItems.TotalItems == 1)
                Output.WriteLine("<p>There is only one matching item.</p>");
            else
            {
                if (privateItems.TotalTitles == privateItems.TotalItems)
                {
                    Output.WriteLine("<p>There are a total of " + privateItems.TotalItems + " titles.</p>");
                }
                else
                {
                    Output.Write("<p>There are a total of " + privateItems.TotalItems + " items in ");
                    if (privateItems.TotalTitles == 1)
                        Output.WriteLine("one title.</p>");
                    else
                        Output.WriteLine(privateItems.TotalTitles + " titles.</p>");
                }
            }

            Output.WriteLine("<br />");
            Output.WriteLine("</div>");

            // Should buttons be added here for additional pages?
            if (privateItems.TotalTitles > RESULTS_PER_PAGE)
            {
                // Get the language suffix for the buttons
                string language_suffix = RequestSpecificValues.Current_Mode.Language_Code;
                if (language_suffix.Length > 0)
                    language_suffix = "_" + language_suffix;

                // Get the text for the buttons
                string first_page = "First Page";
                string previous_page = "Previous Page";
                string next_page = "Next Page";
                string last_page = "Last Page";

                if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
                {
                    first_page = "Primera Página";
                    previous_page = "Página Anterior";
                    next_page = "Página Siguiente";
                    last_page = "Última Página";
                }

                if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
                {
                    first_page = "Première Page";
                    previous_page = "Page Précédente";
                    next_page = "Page Suivante";
                    last_page = "Dernière Page";
                }

                // Get the current page
                ushort current_page = RequestSpecificValues.Current_Mode.Page.HasValue ? RequestSpecificValues.Current_Mode.Page.Value : ((ushort) 1 );

                // Should the previous and first buttons be enabled?
                Output.WriteLine("  <span class=\"leftButtons\">");
                if (current_page > 1)
                {
                    RequestSpecificValues.Current_Mode.Page = 1;
                    Output.WriteLine("    &nbsp; &nbsp; &nbsp; <button title=\"" + first_page + "\" class=\"roundbutton\" onclick=\"window.location='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "'; return false;\"><img src=\"" + Static_Resources.Button_First_Arrow_Png + "\" class=\"roundbutton_img_left\" alt=\"\" /> " + first_page + " </button>&nbsp;");

                    RequestSpecificValues.Current_Mode.Page = (ushort)(current_page - 1);
                    Output.WriteLine("    <button title=\"" + previous_page + "\" class=\"roundbutton\" onclick=\"window.location='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "'; return false;\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"roundbutton_img_left\" alt=\"\" /> " + previous_page + " </button>");
                }
                else
                {
                    Output.WriteLine("    <div style=\"width:160px\">&nbsp;</div>");
                }
                Output.WriteLine("  </span>");

                // Calculate the maximum number of pages
                ushort pages = (ushort)(Math.Ceiling(privateItems.TotalTitles / RESULTS_PER_PAGE));

                int start_title = (int) (1 + ((current_page - 1)*RESULTS_PER_PAGE));
                int end_title = (int) (Math.Min(start_title + RESULTS_PER_PAGE, privateItems.TotalTitles));

                Output.WriteLine("<span style=\"text-align:center;margin-left:auto; margin-right:auto\">" + start_title + " - " + end_title + " of " + privateItems.TotalTitles + " matching titles</span>");


                // Should the next and last buttons be enabled?
                Output.WriteLine("  <span class=\"rightButtons\">");
                if (current_page < pages)
                {
                    RequestSpecificValues.Current_Mode.Page = (ushort)(current_page + 1);
                    Output.WriteLine("    <button title=\"" + next_page + "\" class=\"roundbutton\" onclick=\"window.location='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "'; return false;\"> " + next_page + " <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"roundbutton_img_right\" alt=\"\" /></button>&nbsp;");

                    RequestSpecificValues.Current_Mode.Page = pages;
                    Output.WriteLine("    <button title=\"" + last_page + "\" class=\"roundbutton\" onclick=\"window.location='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "'; return false;\"> " + last_page + " <img src=\"" + Static_Resources.Button_Last_Arrow_Png + "\" class=\"roundbutton_img_right\" alt=\"\" /></button>");
                }
                else
                {
                    Output.WriteLine("    <div style=\"width:160px\">&nbsp;</div>");
                }

                Output.WriteLine("  </span>");
                RequestSpecificValues.Current_Mode.Page = current_page;

                Output.WriteLine("<br />");
            }

            Output.WriteLine("<br />");
           
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
            foreach (Private_Items_List_Title thisTitle in privateItems.TitleResults)
            {
                // Is this a single item, or are there multiple items under this title?
                if (thisTitle.Item_Count > 1)
                {
                    // Draw the title row individually first
                    Output.WriteLine("  <tr align=\"left\" onmouseover=\"this.className='tableRowHighlight'\" onmouseout=\"this.className='tableRowNormal'\" onmousedown=\"window.location.href='" + RequestSpecificValues.Current_Mode.Base_URL + thisTitle.BibID + "'\">");
                    Output.WriteLine("    <td width=\"700px\" colspan=\"2\">");
                    Output.WriteLine("      <table>");
                    Output.WriteLine("        <tr><td colspan=\"3\"><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + thisTitle.BibID + "\"><span class=\"privateTableBibTitle\">" + thisTitle.Group_Title + "</span></a></td></tr>");
                    Output.WriteLine("        <tr><td width=\"180px\"><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + thisTitle.BibID + "\">" + thisTitle.BibID + "</a></td><td width=\"180px\">" + thisTitle.Type + "</td><td>( " + thisTitle.Item_Count + " volumes out of " + thisTitle.CompleteItemCount + " total volumes )</td></tr>");
                    Output.WriteLine("      </table>");
                    Output.WriteLine("    </td>");
                    Output.WriteLine("    <td align=\"center\" width=\"200px\"><span class=\"privateTableBibDaysNumber\">" + Math.Floor( DateTime.Now.Subtract( thisTitle.LastActivityDate ).TotalDays ) + "</span><br />days ago</td>");
                    Output.WriteLine("    <td align=\"center\" width=\"200px\"><span class=\"privateTableBibDaysNumber\">" + Math.Floor(DateTime.Now.Subtract(thisTitle.LastMilestoneDate).TotalDays) + "</span><br />days ago</td>");
                    Output.WriteLine("  </tr>");

                    // Now, draw each item row
                    foreach (Private_Items_List_Item thisItem in thisTitle.Items)
                    {
                        Output.WriteLine("  <tr><td width=\"125px\"></td><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");
                        Output.WriteLine("  <tr align=\"left\" onmouseover=\"this.className='tableRowHighlight'\" onmouseout=\"this.className='tableRowNormal'\" onmousedown=\"window.location.href='" + RequestSpecificValues.Current_Mode.Base_URL + thisTitle.BibID + "/" + thisItem.VID + "'\">");
                        Output.WriteLine("    <td width=\"125px\">");
                        Output.WriteLine("    <td width=\"575px\">");
                        Output.WriteLine("      <table>");
                        Output.Write("        <tr><td colspan=\"3\"><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + thisTitle.BibID + "/" + thisItem.VID + "\"><span class=\"privateTableItemTitle\">" + thisItem.Title + "</span></a>");

                        if (( !String.IsNullOrEmpty(thisItem.Creator) ) && ( thisItem.Creator.Length > 2 ))
                            Output.Write(" ( " + thisItem.Creator.Substring(2) + " )");
                        
                        Output.WriteLine("</td></tr>");
                        Output.Write("        <tr><td width=\"180px\"><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + thisTitle.BibID + "/" + thisItem.VID + "\">" + thisTitle.BibID + " : " + thisItem.VID + "</a></td>");
                        if ( !String.IsNullOrEmpty(thisItem.PubDate))
                            Output.WriteLine("<td>Dated " + thisItem.PubDate + "</td><td></td></tr>");
                        else
                            Output.WriteLine("<td></td><td></td></tr>");
                        if ( !String.IsNullOrEmpty(thisItem.Internal_Comments))
                            Output.WriteLine("              <tr><td colspan=\"3\">&ldquo;" + thisItem.Internal_Comments + "&rdquo;</td></tr>");
                        if ( thisItem.EmbargoDate.HasValue )
                            Output.WriteLine("              <tr><td colspan=\"3\">" + String.Format(EMBARGO_DATE_STRING , thisItem.EmbargoDate.Value.ToShortDateString()) + "</td></tr>");
                        Output.WriteLine("      </table>");
                        Output.WriteLine("    </td>");
                        Output.WriteLine("    <td align=\"center\" width=\"200px\">" + thisItem.LastActivityType.ToLower() + "<br /><span class=\"privateTableItemDaysNumber\">" + Math.Floor( DateTime.Now.Subtract( thisItem.LastActivityDate ).TotalDays ) + "</span><br />days ago</td>");
                        Output.WriteLine("    <td align=\"center\" width=\"200px\">" + thisItem.Last_Milestone_String + "<br /><span class=\"privateTableItemDaysNumber\">" + Math.Floor(DateTime.Now.Subtract(thisItem.LastMilestoneDate).TotalDays) + "</span><br />days ago</td>");
                        Output.WriteLine("  </tr>");                    }
                }
                else
                {
                    // Get the single item
                    Private_Items_List_Item thisItem = thisTitle.Items[0];

                    // Draw the integrated title/item row
                    Output.WriteLine("  <tr align=\"left\" onmouseover=\"this.className='tableRowHighlight'\" onmouseout=\"this.className='tableRowNormal'\" onmousedown=\"window.location.href='" + RequestSpecificValues.Current_Mode.Base_URL + thisTitle.BibID + "/" + thisItem.VID + "'\">");
                    Output.WriteLine("      <td width=\"700px\" colspan=\"2\">");
                    Output.WriteLine("          <table>");
                    Output.WriteLine("              <tr><td colspan=\"3\"><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + thisTitle.BibID + "/" + thisItem.VID + "\"><span class=\"privateTableBibTitle\">" + thisItem.Title + "</span></a>");


                    if ((!String.IsNullOrEmpty(thisItem.Creator)) && (thisItem.Creator.Length > 2))
                        Output.Write(" ( " + thisItem.Creator.Substring(2) + " )");

                    Output.WriteLine("</td></tr>");

                    Output.Write("              <tr><td width=\"180px\"><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + thisTitle.BibID + "/" + thisItem.VID + "\">" + thisTitle.BibID + " : " + thisItem.VID + "</a></td><td width=\"180px\">" + thisTitle.Type + "</td>");


                    if (!String.IsNullOrEmpty(thisItem.PubDate))
                        Output.WriteLine("<td>Dated " + thisItem.PubDate + "</td></tr>");
                    else
                        Output.WriteLine("<td></td></tr>");
                    if (!String.IsNullOrEmpty(thisItem.Internal_Comments))
                        Output.WriteLine("              <tr><td colspan=\"3\">&ldquo;" + thisItem.Internal_Comments + "&rdquo;</td></tr>");
                    if (thisItem.EmbargoDate.HasValue)
                        Output.WriteLine("              <tr><td colspan=\"3\">" + String.Format(EMBARGO_DATE_STRING, thisItem.EmbargoDate.Value.ToShortDateString()) + "</td></tr>");

                    Output.WriteLine("          </table>");
                    Output.WriteLine("      </td>");
                    Output.WriteLine("      <td align=\"center\" width=\"200px\">" + thisItem.LastActivityType.ToLower() + "<br /><span class=\"privateTableItemDaysNumber\">" + Math.Floor(DateTime.Now.Subtract(thisItem.LastActivityDate).TotalDays) + "</span><br />days ago</td>");
                    Output.WriteLine("      <td align=\"center\" width=\"200px\">" + thisItem.Last_Milestone_String + "<br /><span class=\"privateTableItemDaysNumber\">" + Math.Floor(DateTime.Now.Subtract(thisItem.LastMilestoneDate).TotalDays) + "</span><br />days ago</td>");
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
