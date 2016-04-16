#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Engine_Library.Configuration;
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
    /// Aggregation viewers are used when displaying aggregation home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request to display the usage statistics page, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the Application_State_Builder </li>
    /// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/>  </li>
    /// <li>Main writer is created for rendering the output, in this case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
    /// <li>To display the requested collection view, the collection subwriter will creates an instance of this class </li>
    /// </ul></remarks>
    public class Usage_Statistics_AggregationViewer : abstractAggregationViewer
    {
        /// <summary> Constructor for a new instance of the Usage_Statistics_AggregationViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="ViewBag"> Aggregation-specific request information, such as aggregation object and any browse object requested </param>
        public Usage_Statistics_AggregationViewer(RequestCache RequestSpecificValues, AggregationViewBag ViewBag)
            : base(RequestSpecificValues, ViewBag)
        {
            // Everything done in base class constructor
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
        /// <value> This returns the <see cref="Item_Aggregation_Views_Searches_Enum.Usage_Statistics"/> enumerational value </value>
        public override Item_Aggregation_Views_Searches_Enum Type
        {
            get { return Item_Aggregation_Views_Searches_Enum.Usage_Statistics; }
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
            get
            {
                // Normalize the submode
                string submode = "views";
                if (!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Info_Browse_Mode))
                    submode = RequestSpecificValues.Current_Mode.Info_Browse_Mode.ToLower();

                if ((submode != "views") && (submode != "itemviews") && (submode != "titles") && (submode != "items") && (submode != "definitions"))
                {
                    submode = "views";
                }

                // Show the next data, depending on type
                switch (submode)
                {
                    case "views":
                        return "History of Collection-Level Usage";

                    case "itemviews":
                        return "History of Item Usage";

                    case "titles":
                        return "Most Accessed Titles";

                    case "items":
                        return "Most Accessed Items";

                    case "definitions":
                        return "Definitions of Terms Used";

                    default:
                        return "History of Collection-Level Usage";
                }
            }
        }

        /// <summary> Gets the URL for the icon related to this aggregational viewer task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources_Gateway.Usage_Img; }
        }

        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds the title of the into the box </remarks>
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
                Tracer.Add_Trace("Usage_Statistics_AggregationViewer.Add_Secondary_HTML", "Adding HTML");
            }

            const string COLLECTION_VIEWS = "COLLECTION VIEWS";
            const string ITEM_VIEWS = "ITEM VIEWS";
            const string TOP_TITLES = "TOP TITLES";
            const string TOP_ITEMS = "TOP ITEMS";
            const string DEFINITIONS = "DEFINITIONS";

            Output.WriteLine("<div class=\"ShowSelectRow\">");
			Output.WriteLine("  <ul class=\"sbk_FauxDownwardTabsList\">");

            // Save and normalize the submode
            string submode = "views";
            if (!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Info_Browse_Mode))
                submode = RequestSpecificValues.Current_Mode.Info_Browse_Mode.ToLower();
            if ((submode != "views") && (submode != "itemviews") && (submode != "titles") && (submode != "items") && (submode != "definitions"))
            {
                submode = "views";
            }


            if (submode == "views")
            {
                Output.WriteLine("    <li class=\"current\">" + COLLECTION_VIEWS + "</li>");
            }
            else
            {
                RequestSpecificValues.Current_Mode.Info_Browse_Mode = "views";
                Output.WriteLine("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + COLLECTION_VIEWS + "</a></li>");
            }

            if (submode == "itemviews")
            {
                Output.WriteLine("    <li class=\"current\">" + ITEM_VIEWS + "</li>");
            }
            else
            {
                RequestSpecificValues.Current_Mode.Info_Browse_Mode = "itemviews";
                Output.WriteLine("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + ITEM_VIEWS + "</a></li>");
            }

            if (submode == "titles")
            {
                Output.WriteLine("    <li class=\"current\">" + TOP_TITLES + "</li>");
            }
            else
            {
                RequestSpecificValues.Current_Mode.Info_Browse_Mode = "titles";
                Output.WriteLine("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + TOP_TITLES + "</a></li>");
            }

            if (submode == "items")
            {
                Output.WriteLine("    <li class=\"current\">" + TOP_ITEMS + "</li>");
            }
            else
            {
                RequestSpecificValues.Current_Mode.Info_Browse_Mode = "items";
                Output.WriteLine("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + TOP_ITEMS + "</a></li>");
            }

            if (submode == "definitions")
            {
                Output.WriteLine("    <li class=\"current\">" + DEFINITIONS + "</li>");
            }
            else
            {
                RequestSpecificValues.Current_Mode.Info_Browse_Mode = "definitions";
                Output.WriteLine("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + DEFINITIONS + "</a></li>");
            }
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = submode;
			Output.WriteLine("  </ul>");
            Output.WriteLine("</div>");
            Output.WriteLine("<br />");

            // Show the next data, depending on type
            switch (submode)
            {
                case "views":
                    add_collection_usage_history(Output, SobekCM_Database.Get_Aggregation_Statistics_History(ViewBag.Hierarchy_Object.Code, Tracer), Tracer);
                    break;

                case "itemviews":
                    add_item_usage_history(Output, SobekCM_Database.Get_Aggregation_Statistics_History(ViewBag.Hierarchy_Object.Code, Tracer), Tracer);
                    break;

                case "titles":
                    add_titles_by_collection(Output, ViewBag.Hierarchy_Object.Code, Tracer);
                    break;

                case "items":
                    add_items_by_collection(Output, ViewBag.Hierarchy_Object.Code, Tracer);
                    break;

                case "definitions":
                    add_usage_definitions(Output, Tracer);
                    break;
            }
        }

        private static string Month_From_Int(int Month_Int)
        {
            string monthString1 = "Invalid";
            switch (Month_Int)
            {
                case 1:
                    monthString1 = "January";
                    break;

                case 2:
                    monthString1 = "February";
                    break;

                case 3:
                    monthString1 = "March";
                    break;

                case 4:
                    monthString1 = "April";
                    break;

                case 5:
                    monthString1 = "May";
                    break;

                case 6:
                    monthString1 = "June";
                    break;

                case 7:
                    monthString1 = "July";
                    break;

                case 8:
                    monthString1 = "August";
                    break;

                case 9:
                    monthString1 = "September";
                    break;

                case 10:
                    monthString1 = "October";
                    break;

                case 11:
                    monthString1 = "November";
                    break;

                case 12:
                    monthString1 = "December";
                    break;
            }
            return monthString1;
        }

        #region Method to add collection history as html

        private void add_collection_usage_history(TextWriter Output, DataTable StatsCount, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Usage_Statistics_AggregationViewer.add_collection_history", "Rendering HTML");

            Output.WriteLine("<div class=\"SobekText\">");
            Output.WriteLine("<p>Usage history for this collection is displayed below. This history includes just the top-level views of the collection.</p>");

            RequestSpecificValues.Current_Mode.Info_Browse_Mode = "definitions";
            Output.WriteLine("<p>The <a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Definitions page</a> provides more details about the statistics and words used below.</p>");
            Output.WriteLine("</div>");
            Output.WriteLine("<center>");

            Output.WriteLine("  <table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
            Output.WriteLine("    <tr align=\"right\" bgcolor=\"#0022a7\" >");
            Output.WriteLine("      <th width=\"120px\" align=\"left\"><span style=\"color: White\">DATE</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">TOTAL<br />VIEWS</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">VISITS</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">MAIN <br />PAGES</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">BROWSES</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">SEARCH<br />RESULTS</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">TITLE<br />VIEWS</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">ITEM<br />VIEWS</span></th>");
            Output.WriteLine("    </tr>");

            const int COLUMNS = 8;
            string lastYear = String.Empty;
            int hits = 0;
            int sessions = 0;
            int mainPages = 0;
            int browses = 0;
            int searchResults = 0;
            int titleHits = 0;
            int itemHits = 0;

            // Add the collection level information
            if (StatsCount != null)
            {
                foreach (DataRow thisRow in StatsCount.Rows)
                {
                    if (thisRow[0].ToString() != lastYear)
                    {
                        Output.WriteLine("    <tr><td bgcolor=\"#7d90d5\" colspan=\"" + COLUMNS + "\"><span style=\"color: White\"><b> " + thisRow[0] + " STATISTICS</b></span></td></tr>");
                        lastYear = thisRow[0].ToString();
                    }
                    else
                    {
                        Output.WriteLine("    <tr><td bgcolor=\"#e7e7e7\" colspan=\"" + COLUMNS + "\"></td></tr>");
                    }
                    Output.WriteLine("    <tr align=\"right\" >");
                    Output.WriteLine("      <td align=\"left\">" + Month_From_Int(Convert.ToInt32(thisRow[1])) + " " + thisRow[0] + "</td>");

                    hits += Convert.ToInt32(thisRow[2]);
                    Output.WriteLine("      <td>" + thisRow[2] + "</td>");

                    sessions += Convert.ToInt32(thisRow[3]);
                    Output.WriteLine("      <td>" + thisRow[3] + "</td>");

                    int thisRowMainPage = Convert.ToInt32(thisRow[4]) + Convert.ToInt32(thisRow[6]);
                    mainPages += thisRowMainPage;
                    Output.WriteLine("      <td>" + thisRowMainPage + "</td>");

                    browses += Convert.ToInt32(thisRow[5]);
                    Output.WriteLine("      <td>" + thisRow[5] + "</td>");

                    searchResults += Convert.ToInt32(thisRow[7]);
                    Output.WriteLine("      <td>" + thisRow[7] + "</td>");

                    if (thisRow[8] != DBNull.Value)
                    {
                        titleHits += Convert.ToInt32(thisRow[8]);
                        Output.WriteLine("      <td>" + thisRow[8] + "</td>");
                    }
                    else
                    {
                        Output.WriteLine("      <td>0</td>");
                    }
                    if (thisRow[9] != DBNull.Value)
                    {
                        itemHits += Convert.ToInt32(thisRow[9]);
                        Output.WriteLine("      <td>" + thisRow[9] + "</td>");
                    }
                    else
                    {
                        Output.WriteLine("      <td>0</td>");
                    }

                    Output.WriteLine("    </tr>");
                }
                Output.WriteLine("    <tr><td bgcolor=\"Black\" colspan=\"" + COLUMNS + "\"></td></tr>");
                Output.WriteLine("    <tr align=\"right\" >");
                Output.WriteLine("      <td align=\"left\"><b>TOTAL</b></td>");
                Output.WriteLine("      <td><b>" + hits + "</td>");
                Output.WriteLine("      <td><b>" + sessions + "</td>");
                Output.WriteLine("      <td><b>" + mainPages + "</td>");
                Output.WriteLine("      <td><b>" + browses + "</td>");
                Output.WriteLine("      <td><b>" + searchResults + "</td>");
                Output.WriteLine("      <td><b>" + titleHits + "</td>");
                Output.WriteLine("      <td><b>" + itemHits + "</td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("  </table>");
            }

            Output.WriteLine("  <br /> <br />");
            Output.WriteLine("</center>");
        }

        #endregion

        #region Method to add item usage history as html

        private void add_item_usage_history(TextWriter Output, DataTable StatsCount, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Usage_Statistics_AggregationViewer.add_collection_history", "Rendering HTML");

            Output.WriteLine("<div class=\"SobekText\">");
            Output.WriteLine("<p>Usage history for the items within this collection are displayed below.</p>");

            RequestSpecificValues.Current_Mode.Info_Browse_Mode = "definitions";
            Output.WriteLine("<p>The <a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Definitions page</a> provides more details about the statistics and words used below.</p>");
            Output.WriteLine("</div>");
            Output.WriteLine("<center>");


            int jpegViews = 0;
            int zoomViews = 0;
            int thumbViews = 0;
            int flashViews = 0;
            int googleMapViews = 0;
            int downloadViews = 0;
            int citationViews = 0;
            int textSearchViews = 0;
            int staticViews = 0;

            Output.WriteLine("  <table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
            Output.WriteLine("    <tr align=\"right\" bgcolor=\"#0022a7\" >");
            Output.WriteLine("      <th width=\"120px\" align=\"left\"><span style=\"color: White\">DATE</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">JPEG<br />VIEWS</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">ZOOMABLE<br />VIEWS</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">CITATION<br />VIEWS</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">THUMBNAIL<br />VIEWS</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">TEXT<br />SEARCHES</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">FLASH<br />VIEWS</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">MAP<br />VIEWS</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">DOWNLOAD<br />VIEWS</span></th>");
            Output.WriteLine("      <th width=\"90px\" align=\"right\"><span style=\"color: White\">STATIC<br />VIEWS</span></th>");
            Output.WriteLine("    </tr>");

            const int COLUMNS = 10;
            string lastYear = String.Empty;
            if (StatsCount != null)
            {
                foreach (DataRow thisRow in StatsCount.Rows)
                {
                    if (thisRow[0].ToString() != lastYear)
                    {
                        Output.WriteLine("    <tr><td bgcolor=\"#7d90d5\" colspan=\"" + COLUMNS + "\"><span style=\"color: White\"><b> " + thisRow[0] + " STATISTICS</b></span></td></tr>");
                        lastYear = thisRow[0].ToString();
                    }
                    else
                    {
                        Output.WriteLine("    <tr><td bgcolor=\"#e7e7e7\" colspan=\"" + COLUMNS + "\"></td></tr>");
                    }
                    Output.WriteLine("    <tr align=\"right\" >");
                    Output.WriteLine("      <td align=\"left\">" + Month_From_Int(Convert.ToInt32(thisRow[1])) + " " + thisRow[0] + "</td>");

                    if (thisRow[10] != DBNull.Value)
                    {
                        jpegViews += Convert.ToInt32(thisRow[10]);
                        Output.WriteLine("      <td>" + thisRow[10] + "</td>");
                    }
                    else
                    {
                        Output.WriteLine("      <td>0</td>");
                    }
                    if (thisRow[11] != DBNull.Value)
                    {
                        zoomViews += Convert.ToInt32(thisRow[11]);
                        Output.WriteLine("      <td>" + thisRow[11] + "</td>");
                    }
                    else
                    {
                        Output.WriteLine("      <td>0</td>");
                    }
                    if (thisRow[12] != DBNull.Value)
                    {
                        citationViews += Convert.ToInt32(thisRow[12]);
                        Output.WriteLine("      <td>" + thisRow[12] + "</td>");
                    }
                    else
                    {
                        Output.WriteLine("      <td>0</td>");
                    }
                    if (thisRow[13] != DBNull.Value)
                    {
                        thumbViews += Convert.ToInt32(thisRow[13]);
                        Output.WriteLine("      <td>" + thisRow[13] + "</td>");
                    }
                    else
                    {
                        Output.WriteLine("      <td>0</td>");
                    }
                    if (thisRow[14] != DBNull.Value)
                    {
                        textSearchViews += Convert.ToInt32(thisRow[14]);
                        Output.WriteLine("      <td>" + thisRow[14] + "</td>");
                    }
                    else
                    {
                        Output.WriteLine("      <td>0</td>");
                    }
                    if (thisRow[15] != DBNull.Value)
                    {
                        flashViews += Convert.ToInt32(thisRow[15]);
                        Output.WriteLine("      <td>" + thisRow[15] + "</td>");
                    }
                    else
                    {
                        Output.WriteLine("      <td>0</td>");
                    }
                    if (thisRow[16] != DBNull.Value)
                    {
                        googleMapViews += Convert.ToInt32(thisRow[16]);
                        Output.WriteLine("      <td>" + thisRow[16] + "</td>");
                    }
                    else
                    {
                        Output.WriteLine("      <td>0</td>");
                    }
                    if (thisRow[17] != DBNull.Value)
                    {
                        downloadViews += Convert.ToInt32(thisRow[17]);
                        Output.WriteLine("      <td>" + thisRow[17] + "</td>");
                    }
                    else
                    {
                        Output.WriteLine("      <td>0</td>");
                    }
                    if (thisRow[18] != DBNull.Value)
                    {
                        staticViews += Convert.ToInt32(thisRow[18]);
                        Output.WriteLine("      <td>" + thisRow[18] + "</td>");
                    }
                    else
                    {
                        Output.WriteLine("      <td>0</td>");
                    }
                    Output.WriteLine("    </tr>");
                }

                Output.WriteLine("    <tr><td bgcolor=\"Black\" colspan=\"" + COLUMNS + "\"></td></tr>");
                Output.WriteLine("    <tr align=\"right\" >");
                Output.WriteLine("      <td align=\"left\"><b>TOTAL</b></td>");
                Output.WriteLine("      <td><b>" + jpegViews + "</td>");
                Output.WriteLine("      <td><b>" + zoomViews + "</td>");
                Output.WriteLine("      <td><b>" + citationViews + "</td>");
                Output.WriteLine("      <td><b>" + thumbViews + "</td>");
                Output.WriteLine("      <td><b>" + textSearchViews + "</td>");
                Output.WriteLine("      <td><b>" + flashViews + "</td>");
                Output.WriteLine("      <td><b>" + googleMapViews + "</td>");
                Output.WriteLine("      <td><b>" + downloadViews + "</td>");
                Output.WriteLine("      <td><b>" + staticViews + "</td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("  </table>");
            }
            Output.WriteLine("  <br /> <br />");
            Output.WriteLine("</center>");
        }

        #endregion

        #region Method to add the usage defintions

        private void add_usage_definitions(TextWriter Output, Custom_Tracer Tracer)
        {
            // See if the FAQ is present for this collection
            string directory = UI_ApplicationCache_Gateway.Settings.Servers.Base_Design_Location + "\\extra\\stats";
            string usageDefinitions = String.Empty;
            if (Directory.Exists(directory))
            {
                if (File.Exists(directory + "\\stats_usage_definitions.txt"))
                {
                    if (Tracer != null)
                    {
                        Tracer.Add_Trace("Usage_Statistics_AggregationViewer.add_usage_definitions", "Loading usage definitions");
                    }

                    try
                    {
                        StreamReader faqReader = new StreamReader(directory + "\\stats_usage_definitions.txt");
                        usageDefinitions = faqReader.ReadToEnd();
                        faqReader.Close();
                    }
                    catch (Exception)
                    {
                        // If there is an error here, no problem.. just uses the default
                    }
                }
            }

            if (usageDefinitions.Length > 0)
            {
                string urloptions = UrlWriterHelper.URL_Options(RequestSpecificValues.Current_Mode);
                if (urloptions.Length > 0)
                    urloptions = "?" + urloptions;

                if ( Tracer != null )
                    Tracer.Add_Trace("Usage_Statistics_AggregationViewer.add_usage_definitions", "Rendering HTML read from source file");
                Output.WriteLine("<div class=\"SobekText\">");
                Output.WriteLine(usageDefinitions.Replace("<%BASEURL%>", RequestSpecificValues.Current_Mode.Base_URL).Replace("<%?URLOPTS%>", urloptions));
                Output.WriteLine("</div>");

            }
            else
            {
                if ( Tracer != null )
                    Tracer.Add_Trace("Usage_Statistics_AggregationViewer.add_usage_definitions", "Rendering Default HTML");
                Output.WriteLine("<div class=\"SobekText\">");
                Output.WriteLine("<p>The following terms are defined below:</p>");

                Output.WriteLine("<table width=\"600px\" border=\"0\" align=\"center\">");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td><a href=\"#Collection_Hierarchy\">Collection Hierarchy</a></td>");
                Output.WriteLine("    <td><a href=\"#Collection_Groups\">Collection Groups</a></td>");
                Output.WriteLine("    <td><a href=\"#Collections\">Collections</a></td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td><a href=\"#SubCollections\">SubCollections</a></td>");
                Output.WriteLine("    <td><a href=\"#Views\">Views</a></td>");
                Output.WriteLine("    <td><a href=\"#Visits\">Visits</a></td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td><a href=\"#Main_Pages\">Main Pages</a></td>");
                Output.WriteLine("    <td><a href=\"#Browses\">Browses</a></td>");
                Output.WriteLine("    <td><a href=\"#Titles_Items\">Titles and Items</a></td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td><a href=\"#Title_Views\">Title Views</a></td>");
                Output.WriteLine("    <td><a href=\"#Item_Views\">Item Views</a></td>");
                Output.WriteLine("    <td><a href=\"#Citation_Views\">Citation Views</a></td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td><a href=\"#Text_Searches\">Text Searches</a></td>");
                Output.WriteLine("    <td><a href=\"#Static_Views\">Static Views</a></td>");
                Output.WriteLine("    <td>&nbsp;</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("</table>");

                Output.WriteLine("<h2>Defined Terms</h2>");
                Output.WriteLine();

                Output.WriteLine("<a name=\"Collection_Hierarchy\" ></a>");
                Output.WriteLine("<h3>COLLECTION HIERARCHY</h3>");
                Output.WriteLine("<p>Collections are organized by Collection Groups, which contain Collections and Collections contain Subcollections. This hierarchical organization allows for general searches and browses at the Collection Group level and for granular searches at the Collection level for optimum usability for multiple user needs. <br /><br />");
                Output.WriteLine("In reading the statistics by Collection, views and searches done from the main page and the Collection Group pages are not within collections and so are not included in the Collection statistics.</p>");

                Output.WriteLine("<a name=\"Collection_Groups\" ></a>");
                Output.WriteLine("<h3>COLLECTION GROUPS</h3>");
                Output.WriteLine("<p>Collection groups are aggregations of collections in this library. The Collection Groups simplify searching across multiple Collections simultaneously. Collection Groups also connect less tightly related materials to increase the likelihood for serendipity, where users may be searching for one topic and may easily stumble across something related and critically useful that they had not considered. Thus, Collection Groups are usually constructed topically. <br /><br />");
                Output.WriteLine("As an aggregate, views at the Collection Group level do not count toward any particular Collection and are not included in the Collection based statistics.</p>");

                Output.WriteLine("<a name=\"Collections\" ></a>");
                Output.WriteLine("<h3>COLLECTIONS</h3>");
                Output.WriteLine("<p>Collections are the main method for defining and collecting related materials and are the most familiar hierarchical structures for subject specialists, partners, and other internal users. A single Collection can exist in several Collection Groups, and a single Collection can have many subcollections.  <br /><br />");
                Output.WriteLine("A single item may be in several Collections, but one Collection is always selected as primary so all item views will be within a single Collection. </p>");

                Output.WriteLine("<a name=\"SubCollections\" ></a>");
                Output.WriteLine("<h3>SUBCOLLECTIONS</h3>");
                Output.WriteLine("<p>The smallest collected unit is the Subcollection. A single item can belong to several Subcollections under the same collection, or to multiple Collections and to Subcollections within each Collection. <br /><br />");
                Output.WriteLine("Because all Subcollection items will have a primary Collection, the usage statistics for Subcollections are also included in the Collection usage statistics. </p>");

                Output.WriteLine("<a name=\"Views\" ></a>");
                Output.WriteLine("<h3>VIEWS</h3>");
                Output.WriteLine("<p>Views are the actual page hits. Each time a person goes to " + RequestSpecificValues.Current_Mode.Instance_Abbreviation + " it counts as a view. The " + RequestSpecificValues.Current_Mode.Instance_Abbreviation + " statistics are cleaned so that views from robots, which search engines use to index websites, are removed. If they were not removed, the views on all collections and items would be much higher. Web usage statistics are always somewhat fallible, and this is one of the means for ensuring better quality usage statistics. <br /><br />");
                Output.WriteLine("Some web statistics count &quot;page item downloads&quot; as views, which is highly inaccurate because each page has multiple items on it. For instance, the digital library main page, " + RequestSpecificValues.Current_Mode.Instance_Abbreviation + ", includes the page HTML and all of the images. If the statistics counted each “page item download” as a hit, each single view to the main page would be counted as over 30 “page item downloads.” To make matters more confusing, some digital repositories only offer PDF downloads for users to view items. Those digital repositories track &quot;item downloads&quot; and those are most equivalent to our statistics for usage by &quot;item.&quot; </p>");

                Output.WriteLine("<a name=\"Visits\" ></a>");
                Output.WriteLine("<h3>VISITS</h3>");
                Output.WriteLine("<p>Each time a person goes to this digital library it counts as a view, but that means a single user going to the site repeatedly can log a large number of views. Visits provide a better statistic for how many different “unique” users are using the site. Visits include all views from a particular IP address (the user’s computer web address when connected) as recorded in the web log file within an hour.  <br /><br />");
                Output.WriteLine("This is also a fallible statistic since users’ IP addresses are frequently reused on networks.  Connecting to free wireless means that network gives your computer an IP address, and then when you disconnect that IP address will be given to the next user who needs it. For a campus based resource with so many on campus users connecting through the VPN or from on campus, the margin for error increases for visit-based statistics. </p>");

                Output.WriteLine("<a name=\"Main_Pages\" ></a>");
                Output.WriteLine("<h3>MAIN PAGES</h3>");
                Output.WriteLine("<p>For each of the elements in the Collection Hierarchy, the main pages are the home or landing pages, the search pages, the contact pages, and any other supplemental pages.  <br /><br />");
                Output.WriteLine("When users conduct a search through the Collection pages and view the results, those search result pages are also included in the main pages. Once a user clicks on one of the items in the search results, that item is not one of the main pages. The views for search results by thumbnail, table, and brief modes are all included in the main pages for the Collection.</p>");

                Output.WriteLine("<a name=\"Browses\" ></a>");
                Output.WriteLine("<h3>BROWSES</h3>");
                Output.WriteLine("<p>Browses include views against standard browses, such as <i>All Items</i> and <i>New Items</i> (when available).  It also includes all views of non-standard browses.</p>");

                Output.WriteLine("<a name=\"Search_Results\" ></a>");
                Output.WriteLine("<h3>SEARCH RESULTS</h3>");
                Output.WriteLine("<p>Search result views includes every view of a section of search results, and includes searches which returned zero results.</p>");

                Output.WriteLine("<a name=\"Titles_Items\" ></a>");
                Output.WriteLine("<h3>TITLES & ITEMS</h3>");
                Output.WriteLine("<p>Titles are for single bibliographic units, like a book or a newspaper. Items are the volumes within titles. Thus, one book may have one title and one item where one newspaper may have one title and thousands of items.  <br /><br />");
                Output.WriteLine("Titles with only one item (or volume) appear functionally equivalent to users. However for items like newspapers, a single title may correspond to thousands of items. <br /><br />");
                Output.WriteLine("Readers of the technical documentation and internal users know titles by their bibliographic identifier (BIBID) and items within each title by the BIBID plus the volume identifier (VID).</p>");

                Output.WriteLine("<a name=\"Title_Views\" ></a>");
                Output.WriteLine("<h3>TITLE VIEWS</h3>");
                Output.WriteLine("<p>Title views include all views at the title level.</p>");

                Output.WriteLine("<a name=\"Item_Views\" ></a>");
                Output.WriteLine("<h3>ITEM VIEWS</h3>");
                Output.WriteLine("<p>Item views include views at the item level only.</p>");

                Output.WriteLine("<a name=\"Citation_Views\" ></a>");
                Output.WriteLine("<h3>CITATION VIEWS</h3>");
                Output.WriteLine("<p>For each item, the default view is set to the page item (zoomable or static based on user selection and the availability of each of the views for that item). All items also include a “Citation View” that is not selected by default. The “Citation Views” counts the number of times a user chooses the “Citation View” for an item.</p>");

                Output.WriteLine("<a name=\"Text_Searches\" ></a>");
                Output.WriteLine("<h3>TEXT SEARCHES</h3>");
                Output.WriteLine("<p>Text searches are item-level searches within the text of a single document.  This returns the pages upon which the term or terms appear.</p>");

                Output.WriteLine("<a name=\"Static_Views\" ></a>");
                Output.WriteLine("<h3>STATIC VIEWS</h3>");
                Output.WriteLine("<p>For each item in this library, a static page is generated for search engines to index.  When an item appears in the search results in a standard search engine, the link forwards the user to the static page.  Any additional navigation moves the user into the dynamically generated pages within this library.  Attempts have been made to remove all the search engine indexing views from these numbers.  These numbers represent the number of users that entered this library from a search engine.</p>");
                Output.WriteLine("</div>");
            }
        }

        #endregion

        #region Method to add the list of most used items by collection

        private void add_items_by_collection(TextWriter Output, string Collection, Custom_Tracer Tracer)
        {
            DataSet itemsListSet = SobekCM_Database.Statistics_Aggregation_Titles(Collection, Tracer);
            DataTable itemsList = itemsListSet.Tables[0];

            Tracer.Add_Trace("Usage_Statistics_AggregationViewer.add_items_by_collection", "Rendering HTML");

            Output.WriteLine("<div class=\"SobekText\">");
            Output.WriteLine("<p>The most commonly utilized items for this collection appear below.</p>");

            RequestSpecificValues.Current_Mode.Info_Browse_Mode = "definitions";
            Output.WriteLine("<p>The <a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Definitions page</a> provides more details about the statistics and words used below.</p>");

            Output.WriteLine();

            Output.WriteLine("<center>");
            Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
            Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
            Output.WriteLine("    <th width=\"90px\" align=\"left\"><span style=\"color: White\">BIBID</span></th>");
            Output.WriteLine("    <th width=\"50px\" align=\"left\"><span style=\"color: White\">VID</span></th>");
            Output.WriteLine("    <th width=\"430px\" align=\"left\"><span style=\"color: White\">TITLE</span></th>");
            Output.WriteLine("    <th width=\"90px\" align=\"right\"><span style=\"color: White\">VIEWS</span></th>");
            Output.WriteLine("  </tr>");

            if (itemsList != null)
            {
                int itemCount = 0;
                foreach (DataRow thisRow in itemsList.Rows)
                {
                    if (itemCount == 100)
                        break;

                    Output.WriteLine("  <tr align=\"left\" >");
                    Output.WriteLine("    <td>" + thisRow[0] + "</td>");
                    Output.WriteLine("    <td>" + thisRow[1] + "</td>");
                    Output.WriteLine("    <td><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + thisRow[0] + "/" + thisRow[1] + "\">" + thisRow[2] + "</a></td>");
                    Output.WriteLine("    <td align=\"right\">" + thisRow[3] + "</td>");
                    Output.WriteLine("  </tr>");
                    Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>");
                    itemCount++;
                }
            }

            Output.WriteLine("</table>");
            Output.WriteLine("</center>");
            Output.WriteLine("<br /> <br />");
            Output.WriteLine("</div>");
        }
        #endregion

        #region Method to add the list of most used titles by collection

        private void add_titles_by_collection(TextWriter Output, string Collection, Custom_Tracer Tracer)
        {
            DataSet itemsListSet = SobekCM_Database.Statistics_Aggregation_Titles(Collection, Tracer);
            DataTable titleList = itemsListSet.Tables[1];

            Tracer.Add_Trace("Usage_Statistics_AggregationViewer.add_titles_by_collection", "Rendering HTML");

            Output.WriteLine("<div class=\"SobekText\">");
            Output.WriteLine("<p>The most commonly utilized titles by collection appear below.</p>");

            RequestSpecificValues.Current_Mode.Info_Browse_Mode = "definitions";
            Output.WriteLine("<p>The <a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Definitions page</a> provides more details about the statistics and words used below.</p>");
            Output.WriteLine();

            Output.WriteLine("<center>");
            Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
            Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
            Output.WriteLine("    <th width=\"90px\" align=\"left\"><span style=\"color: White\">BIBID</span></th>");
            Output.WriteLine("    <th width=\"480px\" align=\"left\"><span style=\"color: White\">TITLE</span></th>");
            Output.WriteLine("    <th width=\"90px\" align=\"right\"><span style=\"color: White\">VIEWS</span></th>");
            Output.WriteLine("  </tr>");

            if (titleList != null)
            {
                int itemCount = 0;
                foreach (DataRow thisRow in titleList.Rows)
                {
                    if (itemCount == 100)
                        break;

                    Output.WriteLine("  <tr align=\"left\" >");
                    Output.WriteLine("    <td>" + thisRow[0] + "</td>");
                    Output.WriteLine("    <td><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + thisRow[0] + "\">" + thisRow[1] + "</a></td>");
                    Output.WriteLine("    <td align=\"right\">" + thisRow[2] + "</td>");
                    Output.WriteLine("  </tr>");
                    Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");
                    itemCount++;
                }
            }

            Output.WriteLine("</table>");
            Output.WriteLine("</center>");
            Output.WriteLine("<br /> <br />");
            Output.WriteLine("</div>");
        }


        #endregion
    }
}
