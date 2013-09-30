#region Using directives

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Renders the administrative view of a given item aggregation </summary>
    /// <remarks> This class implements the <see cref="iAggregationViewer"/> interface and extends the <see cref="abstractAggregationViewer"/> class.<br /><br />
    /// Aggregation viewers are used when displaying aggregation home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request to display the administrative view of an item aggregation, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in this case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
    /// <li>To display the requested collection view, the collection subwriter will creates an instance of this class </li>
    /// </ul></remarks>
    public class Admin_AggregationViewer : abstractAggregationViewer
    {
        /// <summary> Constructor for a new instance of the Admin_AggregationViewer class </summary>
        /// <param name="Current_Aggregation"> Current item aggregation object </param>
        public Admin_AggregationViewer(Item_Aggregation Current_Aggregation):base(Current_Aggregation, null )
        {
            // Everything done in the base constructor
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation.CollectionViewsAndSearchesEnum.Admin_View"/> enumerational value </value>
        public override Item_Aggregation.CollectionViewsAndSearchesEnum Type
        {
            get { return Item_Aggregation.CollectionViewsAndSearchesEnum.Admin_View;  }
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

        /// <summary> Flag indicates whether the secondary text requires controls </summary>
        /// <value> This property always returns the value FALSE </value>
        public override bool Always_Display_Home_Text
        {
            get
            {
                return false;
            }
        }

        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("<h1>Administrative View</h1>");
        }

        /// <summary> Add the HTML to be displayed below the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds the search tips by calling the base method <see cref="abstractAggregationViewer.Add_Simple_Search_Tips"/> </remarks>
        public override void Add_Secondary_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Admin_AggregationViewer.Add_Secondary_HTML");
            }

            const string general = "GENERAL";
            const string buildLog = "BUILD LOG";

            Output.WriteLine("<div class=\"ShowSelectRow\">");

            string submode = currentMode.Info_Browse_Mode;
            if (submode != "log")
            {
                Output.WriteLine("  <img src=\"" + currentMode.Base_Design_URL + "skins/" + currentMode.Base_Skin + "/tabs/cLD_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab_s\">" + general + "</span><img src=\"" + currentMode.Base_Design_URL + "skins/" + currentMode.Base_Skin + "/tabs/cRD_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /></a>");
            }
            else
            {
                currentMode.Info_Browse_Mode = String.Empty;
                Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_Design_URL + "skins/" + currentMode.Base_Skin + "/tabs/cLD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab\">" + general + "</span><img src=\"" + currentMode.Base_Design_URL + "skins/" + currentMode.Base_Skin + "/tabs/cRD.gif\" border=\"0\" class=\tab_image\" alt=\"\" /></a>");
            }

            if ( currentCollection.Aggregation_Type.ToUpper().IndexOf("COLLECTION GROUP") >= 0)
            {
                if (submode == "log")
                {
                    Output.WriteLine("  <img src=\"" + currentMode.Base_Design_URL + "skins/" + currentMode.Base_Skin + "/tabs/cLD_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab_s\">" + buildLog + "</span><img src=\"" + currentMode.Base_Design_URL + "skins/" + currentMode.Base_Skin + "/tabs/cRD_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /></a>");
                }
                else
                {
                    currentMode.Info_Browse_Mode = "log";
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_Design_URL + "/skins/" + currentMode.Base_Skin + "/tabs/cLD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab\">" + buildLog + "</span><img src=\"" + currentMode.Base_Design_URL + "/skins/" + currentMode.Base_Skin + "/tabs/cRD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /></a>");
                }
            }
            currentMode.Info_Browse_Mode = submode;

            Output.WriteLine("</div>");
            Output.WriteLine("<br />");
            Output.WriteLine("<div class=\"SobekHomeText\">");

            // Add the log
            if (currentMode.Info_Browse_Mode == "log")
            {
                DataTable log = SobekCM_Database.Get_Aggregation_Build_Log(currentCollection.Aggregation_ID);
                if ((log == null) || (log.Rows.Count == 0))
                {
                    Output.WriteLine("<br /><center><b>NO BUILD LOG FOUND FOR THIS AGGREGATION</b></center><br /><br />");
                }
                else
                {
                    Output.WriteLine("<blockquote>");
                    Output.WriteLine("<table>");
                    foreach (DataRow thisRow in log.Rows)
                    {
                        string date = thisRow["LogDate"].ToString();
                        bool isError = Convert.ToBoolean(thisRow["isError"]);
                        string entry = thisRow["LogEntry"].ToString();

                        if (isError)
                        {
                            Output.WriteLine("<tr><td><span style=\"color:red;\">" + date + "</span></td><td><span style=\"color:red;\">" + entry + "</span></td></tr>");
                        }
                        else
                        {
                            if (entry.IndexOf("(COMPLETE)") >= 0)
                            {
                                Output.WriteLine("<tr><td><span style=\"color:blue;\">" + date + "</span></td><td><span style=\"color:blue;\"><b>" + entry.Replace("(COMPLETE)", "") + "</b></span></td></tr>");
                            }
                            else
                            {
                                Output.WriteLine("<tr><td>" + date + "</td><td>" + entry + "</td></tr>");
                            }
                        }
                    }
                    Output.WriteLine("</table>");
                    Output.WriteLine("</blockquote>");
                }
            }
            else
            {
                Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" >");
                Output.WriteLine("    <td colspan=\"2\"><span style=\"color: White\" ><b>PUBLIC PROPERTIES</b></span></td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                Output.WriteLine("    <th><span style=\"color: White\">NAME</span></th>");
                Output.WriteLine("    <th><span style=\"color: White\">VALUE</span></th>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Aggregation_Type</td>");
                Output.WriteLine("    <td>" + currentCollection.Aggregation_Type + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>AggregationID</td>");
                Output.WriteLine("    <td>" + currentCollection.Aggregation_ID + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Banner_Image</td>");
                Output.WriteLine("    <td>" + currentCollection.Banner_Image( currentMode.Language, htmlSkin ) + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Children_Count</td>");
                Output.WriteLine("    <td>" + currentCollection.Children_Count + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Code</td>");
                Output.WriteLine("    <td>" + currentCollection.Code + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Contact_Email</td>");
                Output.WriteLine("    <td>" + currentCollection.Contact_Email + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Default_Skin</td>");
                Output.WriteLine("    <td>" + currentCollection.Default_Skin + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Description</td>");
                Output.WriteLine("    <td>" + currentCollection.Description + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Display_Options</td>");
                Output.WriteLine("    <td>" + currentCollection.Display_Options + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Has_New_Items</td>");
                Output.WriteLine("    <td>" + currentCollection.Has_New_Items + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Hidden</td>");
                Output.WriteLine("    <td>" + currentCollection.Hidden + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Home_Page_File</td>");
                Output.WriteLine("    <td>" + currentCollection.Home_Page_File( currentMode.Language ) + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Is_Active</td>");
                Output.WriteLine("    <td>" + currentCollection.Is_Active + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Last_Item_Added</td>");
                Output.WriteLine("    <td>" + currentCollection.Last_Item_Added.ToShortDateString() + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Load_Email</td>");
                Output.WriteLine("    <td>" + currentCollection.Load_Email + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Map_Display</td>");
                Output.WriteLine("    <td>" + currentCollection.Map_Display + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Map_Search</td>");
                switch (currentCollection.Map_Search)
                {
                    case 0:
                        Output.WriteLine("    <td>0 <i>( world - default )</i></td>");
                        break;
                    case 1:
                        Output.WriteLine("    <td>1 <i>( florida )</i></td>");
                        break;
                    case 2:
                        Output.WriteLine("    <td>2 <i>( united states )</i></td>");
                        break;
                    case 3:
                        Output.WriteLine("    <td>3 <i>( north america )</i></td>");
                        break;
                    case 4:
                        Output.WriteLine("    <td>4 <i>( caribbean )</i></td>");
                        break;
                    case 5:
                        Output.WriteLine("    <td>5 <i>( south america )</i></td>");
                        break;
                    case 6:
                        Output.WriteLine("    <td>6 <i>( africa )</i></td>");
                        break;
                    case 7:
                        Output.WriteLine("    <td>7 <i>( europe )</i></td>");
                        break;
                    case 8:
                        Output.WriteLine("    <td>8 <i>( asia )</i></td>");
                        break;
                    case 9:
                        Output.WriteLine("    <td>9 <i>( middle east )</i></td>");
                        break;
                    default:
                        Output.WriteLine("    <td>" + currentCollection.Map_Search + "</td>");
                        break;
                }
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Metadata_Code</td>");
                Output.WriteLine("    <td>" + currentCollection.Metadata_Code + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Name</td>");
                Output.WriteLine("    <td>" + currentCollection.Name + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>OAI_Flag</td>");
                Output.WriteLine("    <td>" + currentCollection.OAI_Flag + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>OAI_Metadata</td>");
                if (currentCollection.OAI_Metadata.Trim().Length == 0)
                {
                    Output.WriteLine("    <td><i>(empty)</i></td>");
                }
                else
                {
                    Output.WriteLine("    <td>" + currentCollection.OAI_Metadata.Replace("<", "&lt;").Replace(">", "&gt;") + "</td>");
                }
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>objDirectory</td>");
                Output.WriteLine("    <td>" + currentCollection.ObjDirectory + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Parent_Count</td>");
                Output.WriteLine("    <td>" + currentCollection.Parent_Count + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>ShortName</td>");
                Output.WriteLine("    <td>" + currentCollection.ShortName + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Show_New_Item_Browse</td>");
                Output.WriteLine("    <td>" + currentCollection.Show_New_Item_Browse + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>Thematic_Heading_ID</td>");
                if (currentCollection.Thematic_Heading_ID < 0 )
                {
                    Output.WriteLine("    <td><i>(none)</i></td>");
                }
                else
                {
                    Output.WriteLine("    <td>" + currentCollection.Thematic_Heading_ID + "</td>");
                }
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                Output.WriteLine("</table>");
                Output.WriteLine("<br /><br />");

                if (currentCollection.Children_Count > 0)
                {
                    Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                    Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" >");
                    Output.WriteLine("    <td colspan=\"5\"><span style=\"color: White\" ><b>CHILDREN</b></span></td>");
                    Output.WriteLine("  </tr>");
                    Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                    Output.WriteLine("    <th width=\"80px\" align=\"left\"><span style=\"color: White; text-align:left\">CODE</span></th>");
                    Output.WriteLine("    <th width=\"150px\" align=\"left\"><span style=\"color: White; text-align:left\">TYPE</span></th>");
                    Output.WriteLine("    <th width=\"300px\" align=\"left\"><span style=\"color: White; text-align:left\">NAME</span></th>");
                    Output.WriteLine("    <th width=\"100px\" align=\"left\"><span style=\"color: White; text-align:left\">ACTIVE</span></th>");
                    Output.WriteLine("    <th><span style=\"color: White; text-align:left\">HIDDEN</span></th>");
                    Output.WriteLine("  </tr>");
                    foreach (Item_Aggregation_Related_Aggregations thisChild in currentCollection.Children)
                    {
                        Output.WriteLine("  <tr align=\"left\">");
                        Output.WriteLine("    <td>" + thisChild.Code + "</td>");
                        Output.WriteLine("    <td>" + thisChild.Type + "</td>");
                        Output.WriteLine("    <td>" + thisChild.Name + "</td>");
                        Output.WriteLine("    <td>" + thisChild.Active + "</td>");
                        Output.WriteLine("    <td>" + thisChild.Hidden + "</td>");
                        Output.WriteLine("  </tr>");
                        Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"5\"></td></tr>");
                    }
                    Output.WriteLine("</table>");
                    Output.WriteLine("<br /><br />");
                }

                if (currentCollection.Parent_Count > 0)
                {
                    Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                    Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" >");
                    Output.WriteLine("    <td colspan=\"5\"><span style=\"color: White\" ><b>PARENTS</b></span></td>");
                    Output.WriteLine("  </tr>");
                    Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                    Output.WriteLine("    <th width=\"80px\" align=\"left\"><span style=\"color: White; text-align:left\">CODE</span></th>");
                    Output.WriteLine("    <th width=\"150px\" align=\"left\"><span style=\"color: White; text-align:left\">TYPE</span></th>");
                    Output.WriteLine("    <th width=\"300px\" align=\"left\"><span style=\"color: White; text-align:left\">NAME</span></th>");
                    Output.WriteLine("    <th width=\"100px\" align=\"left\"><span style=\"color: White; text-align:left\">ACTIVE</span></th>");
                    Output.WriteLine("    <th align=\"left\"><span style=\"color: White; text-align:left\">HIDDEN</span></th>");
                    Output.WriteLine("  </tr>");
                    foreach (Item_Aggregation_Related_Aggregations thisParent in currentCollection.Parents)
                    {
                        Output.WriteLine("  <tr align=\"left\">");
                        Output.WriteLine("    <td>" + thisParent.Code + "</td>");
                        Output.WriteLine("    <td>" + thisParent.Type + "</td>");
                        Output.WriteLine("    <td>" + thisParent.Name + "</td>");
                        Output.WriteLine("    <td>" + thisParent.Active + "</td>");
                        Output.WriteLine("    <td>" + thisParent.Hidden + "</td>");
                        Output.WriteLine("  </tr>");
                        Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"5\"></td></tr>");
                    }
                    Output.WriteLine("</table>");
                    Output.WriteLine("<br /><br />");
                }

                ReadOnlyCollection<Item_Aggregation_Browse_Info> browses = currentCollection.Browse_Home_Pages(currentMode.Language);
                if (browses.Count > 0)
                {
                    Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                    Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" >");
                    Output.WriteLine("    <td colspan=\"3\"><span style=\"color: White\" ><b>BROWSE PAGES</b></span></td>");
                    Output.WriteLine("  </tr>");
                    Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                    Output.WriteLine("    <th width=\"80px\" align=\"left\"><span style=\"color: White; text-align:left\">CODE</span></th>");
                    Output.WriteLine("    <th width=\"60px\" align=\"left\"><span style=\"color: White; text-align:left\">TYPE</span></th>");
                    Output.WriteLine("    <th align=\"left\"><span style=\"color: White; text-align:left\">LABEL / SOURCE</span></th>");
                    Output.WriteLine("  </tr>");
                    foreach (Item_Aggregation_Browse_Info thisBrowse in browses)
                    {
                        Output.WriteLine("  <tr align=\"left\">");
                        Output.WriteLine("    <td>" + thisBrowse.Code + "</td>");
                        switch (thisBrowse.Data_Type)
                        {
                            case Item_Aggregation_Browse_Info.Result_Data_Type.Table:
                                Output.WriteLine("    <td>Table</td>");
                                break;

                            case Item_Aggregation_Browse_Info.Result_Data_Type.Text:
                                Output.WriteLine("    <td>Text</td>");
                                break;

                            default:
                                Output.WriteLine("    <td><i>null</i></td>");
                                break;
                        }
                        Output.Write("    <td>" + thisBrowse.Get_Label( currentMode.Language ) + "<br />");

                        switch (thisBrowse.Source)
                        {
                            case Item_Aggregation_Browse_Info.Source_Type.Database:
                                Output.WriteLine("<i>From Database</i></td>");
                                break;

                            case Item_Aggregation_Browse_Info.Source_Type.Static_HTML:
                                string staticHtmlSource = thisBrowse.Get_Static_HTML_Source(currentMode.Language);
                                if ((staticHtmlSource.Length > 0) && (staticHtmlSource.IndexOf("design") > 0))
                                {
                                    Output.WriteLine(staticHtmlSource.Substring(staticHtmlSource.IndexOf("design")) + "</td>");
                                }
                                else
                                {
                                    Output.WriteLine("Static HTML</td>");
                                }
                                break;

                            default:
                                Output.WriteLine("<i>null</i></td>");
                                break;
                        }
                        Output.WriteLine("  </tr>");
                        Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");
                    }
                    Output.WriteLine("</table>");
                    Output.WriteLine("<br /><br />");
                }

                ReadOnlyCollection<Item_Aggregation_Browse_Info> infos = currentCollection.Info_Pages;
                if (infos.Count > 0)
                {
                    Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                    Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" >");
                    Output.WriteLine("    <td colspan=\"3\"><span style=\"color: White\" ><b>INFO PAGES</b></span></td>");
                    Output.WriteLine("  </tr>");
                    Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                    Output.WriteLine("    <th width=\"80px\" align=\"left\"><span style=\"color: White; text-align:left\">CODE</span></th>");
                    Output.WriteLine("    <th width=\"60px\" align=\"left\"><span style=\"color: White; text-align:left\">TYPE</span></th>");
                    Output.WriteLine("    <th align=\"left\"><span style=\"color: White; text-align:left\">LABEL / SOURCE</span></th>");
                    Output.WriteLine("  </tr>");
                    foreach (Item_Aggregation_Browse_Info thisBrowse in infos)
                    {
                        Output.WriteLine("  <tr align=\"left\">");
                        Output.WriteLine("    <td>" + thisBrowse.Code + "</td>");
                        switch (thisBrowse.Data_Type)
                        {
                            case Item_Aggregation_Browse_Info.Result_Data_Type.Table:
                                Output.WriteLine("    <td>Table</td>");
                                break;

                            case Item_Aggregation_Browse_Info.Result_Data_Type.Text:
                                Output.WriteLine("    <td>Text</td>");
                                break;

                            default:
                                Output.WriteLine("    <td><i>null</i></td>");
                                break;
                        }
                        Output.Write("    <td>" + thisBrowse.Get_Label(currentMode.Language) + "<br />");

                        switch (thisBrowse.Source)
                        {
                            case Item_Aggregation_Browse_Info.Source_Type.Database:
                                Output.WriteLine("<i>From Database</i></td>");
                                break;

                            case Item_Aggregation_Browse_Info.Source_Type.Static_HTML:
                                string staticHtmlSource = thisBrowse.Get_Static_HTML_Source(currentMode.Language);
                                if ((staticHtmlSource.Length > 0) && (staticHtmlSource.IndexOf("design") > 0))
                                {
                                    Output.WriteLine(staticHtmlSource.Substring(staticHtmlSource.IndexOf("design")) + "</td>");
                                }
                                else
                                {
                                    Output.WriteLine("Static HTML</td>");
                                }
                                break;

                            default:
                                Output.WriteLine("<i>null</i></td>");
                                break;
                        }
                        Output.WriteLine("  </tr>");
                        Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");
                    }
                    Output.WriteLine("</table>");
                    Output.WriteLine("<br /><br />");
                }

                Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\" width=\"300px\">");
                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" >");
                Output.WriteLine("    <td><span style=\"color: White\" ><b>VIEWS AND SEARCHES</b></span></td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                Output.WriteLine("    <th align=\"left\"><span style=\"color: White; text-align:left\">TYPE</span></th>");
                Output.WriteLine("  </tr>");

                foreach (Item_Aggregation.CollectionViewsAndSearchesEnum thisView in currentCollection.Views_And_Searches)
                {
                    Output.WriteLine("  <tr>");
                    switch (thisView)
                    {
                        case Item_Aggregation.CollectionViewsAndSearchesEnum.Admin_View:
                            Output.WriteLine("    <td>Admin view</td>");
                            break;

                        case Item_Aggregation.CollectionViewsAndSearchesEnum.Advanced_Search:
                            Output.WriteLine("    <td>Advanced Search</td>");
                            break;

                        case Item_Aggregation.CollectionViewsAndSearchesEnum.All_New_Items:
                            Output.WriteLine("    <td>All / New Items Browses</td>");
                            break;

                        case Item_Aggregation.CollectionViewsAndSearchesEnum.Basic_Search:
                            Output.WriteLine("    <td>Basic Search</td>");
                            break;

                        case Item_Aggregation.CollectionViewsAndSearchesEnum.DataSet_Browse:
                            Output.WriteLine("    <td>DataSet Browse</td>");
                            break;

                        case Item_Aggregation.CollectionViewsAndSearchesEnum.dLOC_FullText_Search:
                            Output.WriteLine("    <td>dLOC Search</td>");
                            break;

                        case Item_Aggregation.CollectionViewsAndSearchesEnum.Map_Search:
                            Output.WriteLine("    <td>Map Search</td>");
                            break;

                        case Item_Aggregation.CollectionViewsAndSearchesEnum.Newspaper_Search:
                            Output.WriteLine("    <td>Newspaper Search</td>");
                            break;

                        case Item_Aggregation.CollectionViewsAndSearchesEnum.No_Home_Search:
                            Output.WriteLine("    <td>No Home Search</td>");
                            break;

                        case Item_Aggregation.CollectionViewsAndSearchesEnum.Static_Browse_Info:
                            Output.WriteLine("    <td>Static Browse Info</td>");
                            break;
                    }
                    Output.WriteLine("  </tr>");
                    Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\"></td></tr>");
                }
                Output.WriteLine("</table>");
                Output.WriteLine("<br /><br />");

            }
            Output.WriteLine("<br />");

            Output.WriteLine("</div>");

        }
    }
}
