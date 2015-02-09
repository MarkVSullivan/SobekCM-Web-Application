#region Using directives

using System;
using System.Data;
using System.IO;
using System.Linq;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows an authenticated RequestSpecificValues.Current_User to view their own RequestSpecificValues.Current_User tags, or tags by RequestSpecificValues.Current_User or aggregation, depending on the current RequestSpecificValues.Current_User's rights.</summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to display the RequestSpecificValues.Current_User-entered descriptive tags </li>
    /// </ul></remarks>
    public class User_Tags_MySobekViewer : abstract_MySobekViewer
    {
        /// <summary> Constructor for a new instance of the User_Tags_MySobekViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public User_Tags_MySobekViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("User_Tags_MySobekViewer.Constructor", String.Empty);
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        public override string Web_Title
        {
            get {
                return RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Length == 0 ? "My Descriptive Tags" : "Descriptive Tags";
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("User_Tags_MySobekViewer.Write_HTML", String.Empty);

			Output.WriteLine("<h1>" + Web_Title + "</h1>");
			Output.WriteLine();

            Output.WriteLine("<div class=\"SobekHomeText\">");
            string submode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;

            // Is this either a sys admin or a collection admin/manager
            if ((RequestSpecificValues.Current_User.Is_System_Admin) || (RequestSpecificValues.Current_User.Is_A_Collection_Manager_Or_Admin))
            {
                Output.WriteLine("<blockquote>As a digital collection manager or administrator, you can use this screen to view descriptive tags added to collections of interest, as well as view the descriptive tags you have added to items.</blockquote>");

                Output.WriteLine("<table width=\"750px\" border=\"0\" align=\"center\" cellpadding=\"1\" cellspacing=\"0\">");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td bgcolor=\"#cccccc\">");
                Output.WriteLine("      <table width=\"100%\" border=\"0\" align=\"center\" cellpadding=\"2\" cellspacing=\"0\">");
                Output.WriteLine("        <tr>");
                Output.WriteLine("          <td bgcolor=\"#f4f4f4\"> &nbsp; <span class=\"groupname\"><span class=\"groupnamecaps\">&nbsp;T</span>AGS BY <span class=\"groupnamecaps\">A</span>GGREGATION </span></td>");
                Output.WriteLine("        </tr>");
                Output.WriteLine("      </table>");
                Output.WriteLine("    </td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("</table>");


                Output.WriteLine("<table width=\"700px\" border=\"0\" align=\"center\"><tr><td>");
                Output.WriteLine("<br />Choose an aggregation below to view all tags for that aggregation:");
                Output.WriteLine("<blockquote>");

                if (RequestSpecificValues.Current_User.PermissionedAggregations != null)
                {
                    foreach (User_Permissioned_Aggregation aggregation in RequestSpecificValues.Current_User.PermissionedAggregations)
                    {
                        if (aggregation.IsCurator)
                        {
                            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = aggregation.Code;
                            Output.WriteLine("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + aggregation.Name + "</a><br /><br />");
                        }
                    }
                }

                if (RequestSpecificValues.Current_User.Is_System_Admin)
                {
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "all";
                    Output.WriteLine("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">All Aggregations</a><br /><br />");

                }

                Output.WriteLine("</blockquote>");
                Output.WriteLine("</td></tr></table>");
            }

            // If there is no submode, just pull this users descriptive tags
            if (submode.Length == 0)
            {
                    Output.WriteLine("<table width=\"750px\" border=\"0\" align=\"center\" cellpadding=\"1\" cellspacing=\"0\">");
                    Output.WriteLine("  <tr>");
                    Output.WriteLine("    <td bgcolor=\"#cccccc\">");
                    Output.WriteLine("      <table width=\"100%\" border=\"0\" align=\"center\" cellpadding=\"2\" cellspacing=\"0\">");
                    Output.WriteLine("        <tr>");
                    Output.WriteLine("          <td bgcolor=\"#f4f4f4\"> &nbsp; <span class=\"groupname\"><span class=\"groupnamecaps\">&nbsp;Y</span>OUR <span class=\"groupnamecaps\">D</span>ESCRIPTIVE <span class=\"groupnamecaps\">T</span>AGS </span></td>");
                    Output.WriteLine("        </tr>");
                    Output.WriteLine("      </table>");
                    Output.WriteLine("    </td>");
                    Output.WriteLine("  </tr>");
                    Output.WriteLine("</table>");
             

                // Get the tags by this RequestSpecificValues.Current_User
                DataTable allTags = SobekCM_Database.View_Tags_By_User(RequestSpecificValues.Current_User.UserID, Tracer);
                if ((allTags == null) || (allTags.Rows.Count == 0))
                {
                    Output.WriteLine("<br /><br /><center><b>You have not added any descriptive tags to any items</b></center><br /><br />");
                }
                else
                {
                    Output.WriteLine("<table width=\"700px\" border=\"0\" align=\"center\"><tr><td>");
                    Output.WriteLine("<br />You have added the following " + allTags.Rows.Count + " descriptive tags:");
                    Output.WriteLine("<blockquote>");

                    foreach (DataRow thisTagRow in allTags.Rows)
                    {
                        string description = thisTagRow["Description_Tag"].ToString();
                        string bibid = thisTagRow["BibID"].ToString();
                        string vid = thisTagRow["VID"].ToString();
                        DateTime date = Convert.ToDateTime(thisTagRow["Date_Modified"]);

                        Output.WriteLine(description + "<br /><i>Added by you on " + date.ToShortDateString() + "</i> &nbsp; &nbsp; &nbsp; ( <a href=\"?m=ldFC&b=" + bibid + "&v=" + vid + UrlWriterHelper.URL_Options(RequestSpecificValues.Current_Mode) + "\" target=\"" + bibid + vid + "\">view</a> )<br /><br />");
                    }

                    Output.WriteLine("</blockquote>");
                    Output.WriteLine("</td></tr></table>");
                }
            }
            else
            {
                bool char_found = submode.Any(ThisChar => !Char.IsNumber(ThisChar));
                if (char_found)
                {
                    Output.WriteLine("<table width=\"750px\" border=\"0\" align=\"center\" cellpadding=\"1\" cellspacing=\"0\">");
                    Output.WriteLine("  <tr>");
                    Output.WriteLine("    <td bgcolor=\"#cccccc\">");
                    Output.WriteLine("      <table width=\"100%\" border=\"0\" align=\"center\" cellpadding=\"2\" cellspacing=\"0\">");
                    Output.WriteLine("        <tr>");
                    Output.WriteLine("          <td bgcolor=\"#f4f4f4\"> &nbsp; <span class=\"groupname\"><span class=\"groupnamecaps\">&nbsp;T</span>AGS BY <span class=\"groupnamecaps\">A</span>GGREGATION</span></td>");
                    Output.WriteLine("        </tr>");
                    Output.WriteLine("      </table>");
                    Output.WriteLine("    </td>");
                    Output.WriteLine("  </tr>");
                    Output.WriteLine("</table>");

                    string aggregation_code = submode.ToUpper();
                    if (aggregation_code == "ALL")
                        aggregation_code = String.Empty;

                    DataTable allTags = SobekCM_Database.View_Tags_By_Aggregation(aggregation_code, Tracer);

                    if ((allTags != null) && (allTags.Rows.Count > 0))
                    {
                        Output.WriteLine("<table width=\"700px\" border=\"0\" align=\"center\"><tr><td>");
                        Output.WriteLine("<br />This aggregation includes the following " + allTags.Rows.Count + " descriptive tags:");
                        Output.WriteLine("<blockquote>");

                        foreach (DataRow thisTagRow in allTags.Rows)
                        {
                            string description = thisTagRow["Description_Tag"].ToString();
                            string bibid = thisTagRow["BibID"].ToString();
                            string vid = thisTagRow["VID"].ToString();
                            DateTime date = Convert.ToDateTime(thisTagRow["Date_Modified"]);


                            string first_name = thisTagRow["FirstName"].ToString();
                            string nick_name = thisTagRow["NickName"].ToString();
                            string last_name = thisTagRow["LastName"].ToString();
                            string full_name = first_name + " " + last_name;
                            if (nick_name.Length > 0)
                            {
                                full_name = nick_name + " " + last_name;
                            }

                            Output.Write(description + "<br /><i>Added by " + full_name + " on " + date.ToShortDateString() + "</i> &nbsp; &nbsp; &nbsp; ( <a href=\"?m=ldFC&b=" + bibid + "&v=" + vid + UrlWriterHelper.URL_Options(RequestSpecificValues.Current_Mode) + "\" target=\"" + bibid + vid + "\">view</a> | ");
                            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = thisTagRow["UserID"].ToString();

                            Output.WriteLine("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">view all by this RequestSpecificValues.Current_User</a> )<br /><br />");
                        }

                        Output.WriteLine("</blockquote>");
                        Output.WriteLine("</td></tr></table>");

                    }
                    else
                    {
                        Output.WriteLine("<br /><br /><center><b>This aggregation had no added tags or it is not a valid agggregation code.</b></center><br /><br />");
                    }

                }
                else
                {
                    Output.WriteLine("<table width=\"750px\" border=\"0\" align=\"center\" cellpadding=\"1\" cellspacing=\"0\">");
                    Output.WriteLine("  <tr>");
                    Output.WriteLine("    <td bgcolor=\"#cccccc\">");
                    Output.WriteLine("      <table width=\"100%\" border=\"0\" align=\"center\" cellpadding=\"2\" cellspacing=\"0\">");
                    Output.WriteLine("        <tr>");
                    Output.WriteLine("          <td bgcolor=\"#f4f4f4\"> &nbsp; <span class=\"groupname\"><span class=\"groupnamecaps\">&nbsp;T</span>AGS BY <span class=\"groupnamecaps\">U</span>SER</span></td>");
                    Output.WriteLine("        </tr>");
                    Output.WriteLine("      </table>");
                    Output.WriteLine("    </td>");
                    Output.WriteLine("  </tr>");
                    Output.WriteLine("</table>");


                    int userid = Convert.ToInt32(submode);
                    DataTable allTags = SobekCM_Database.View_Tags_By_User(userid, Tracer);

                    if ((allTags != null) && (allTags.Rows.Count > 0))
                    {
                        Output.WriteLine("<table width=\"700px\" border=\"0\" align=\"center\"><tr><td>");
                        Output.WriteLine("<br />This RequestSpecificValues.Current_User added the following " + allTags.Rows.Count + " descriptive tags:");
                        Output.WriteLine("<blockquote>");

                        foreach (DataRow thisTagRow in allTags.Rows)
                        {
                            string description = thisTagRow["Description_Tag"].ToString();
                            string bibid = thisTagRow["BibID"].ToString();
                            string vid = thisTagRow["VID"].ToString();
                            DateTime date = Convert.ToDateTime(thisTagRow["Date_Modified"]);


                            string first_name = thisTagRow["FirstName"].ToString();
                            string nick_name = thisTagRow["NickName"].ToString();
                            string last_name = thisTagRow["LastName"].ToString();
                            string full_name = first_name + " " + last_name;
                            if (nick_name.Length > 0)
                            {
                                full_name = nick_name + " " + last_name;
                            }

                            Output.WriteLine(description + "<br /><i>Added by " + full_name + " on " + date.ToShortDateString() + "</i> &nbsp; &nbsp; &nbsp; ( <a href=\"?m=ldFC&b=" + bibid + "&v=" + vid + UrlWriterHelper.URL_Options(RequestSpecificValues.Current_Mode) + "\" target=\"" + bibid + vid + "\">view</a> )<br /><br />");
                        }

                        Output.WriteLine("</blockquote>");
                        Output.WriteLine("</td></tr></table>");

                    }
                    else
                    {
                        Output.WriteLine("<br /><br /><center><b>This RequestSpecificValues.Current_User has not added any descriptive tags to any items or it is not a valid userid.</b></center><br /><br />");
                    }
                }
            }

            Output.WriteLine("</div>");
        }
    }
}
