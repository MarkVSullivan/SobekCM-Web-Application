#region Using directives

using System;
using System.Data;
using System.IO;
using System.Linq;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows an authenticated user to view their own user tags, or tags by user or aggregation, depending on the current user's rights.</summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to display the user-entered descriptive tags </li>
    /// </ul></remarks>
    public class User_Tags_MySobekViewer : abstract_MySobekViewer
    {
        /// <summary> Constructor for a new instance of the User_Tags_MySobekViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public User_Tags_MySobekViewer(User_Object User, Custom_Tracer Tracer) : base(User)
        {
            Tracer.Add_Trace("User_Tags_MySobekViewer.Constructor", String.Empty);
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        public override string Web_Title
        {
            get {
                return currentMode.My_Sobek_SubMode.Length == 0 ? "My Descriptive Tags" : "Descriptive Tags";
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("User_Tags_MySobekViewer.Write_HTML", String.Empty);

            Output.WriteLine("<div class=\"SobekHomeText\">");
            string submode = currentMode.My_Sobek_SubMode;

            // Is this either a sys admin or a collection admin/manager
            if ((user.Is_System_Admin) || (user.Is_A_Collection_Manager_Or_Admin))
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

                foreach (User_Editable_Aggregation aggregation in user.Aggregations)
                {
                    if (aggregation.IsCurator )
                    {
                        currentMode.My_Sobek_SubMode = aggregation.Code;
                        Output.WriteLine("<a href=\"" + currentMode.Redirect_URL() + "\">" + aggregation.Name + "</a><br /><br />");
                    }
                }

                if (user.Is_System_Admin)
                {
                    currentMode.My_Sobek_SubMode = "all";
                    Output.WriteLine("<a href=\"" + currentMode.Redirect_URL() + "\">All Aggregations</a><br /><br />");

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
             

                // Get the tags by this user
                DataTable allTags = SobekCM_Database.View_Tags_By_User(user.UserID, Tracer);
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

                        Output.WriteLine(description + "<br /><i>Added by you on " + date.ToShortDateString() + "</i> &nbsp; &nbsp; &nbsp; ( <a href=\"?m=ldFC&b=" + bibid + "&v=" + vid + currentMode.URL_Options() + "\" target=\"" + bibid + vid + "\">view</a> )<br /><br />");
                    }

                    Output.WriteLine("</blockquote>");
                    Output.WriteLine("</td></tr></table>");
                }
            }
            else
            {
                bool char_found = submode.Any(thisChar => !Char.IsNumber(thisChar));
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

                            Output.Write(description + "<br /><i>Added by " + full_name + " on " + date.ToShortDateString() + "</i> &nbsp; &nbsp; &nbsp; ( <a href=\"?m=ldFC&b=" + bibid + "&v=" + vid + currentMode.URL_Options() + "\" target=\"" + bibid + vid + "\">view</a> | ");
                            currentMode.My_Sobek_SubMode = thisTagRow["UserID"].ToString();

                            Output.WriteLine("<a href=\"" + currentMode.Redirect_URL() + "\">view all by this user</a> )<br /><br />");
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
                        Output.WriteLine("<br />This user added the following " + allTags.Rows.Count + " descriptive tags:");
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

                            Output.WriteLine(description + "<br /><i>Added by " + full_name + " on " + date.ToShortDateString() + "</i> &nbsp; &nbsp; &nbsp; ( <a href=\"?m=ldFC&b=" + bibid + "&v=" + vid + currentMode.URL_Options() + "\" target=\"" + bibid + vid + "\">view</a> )<br /><br />");
                        }

                        Output.WriteLine("</blockquote>");
                        Output.WriteLine("</td></tr></table>");

                    }
                    else
                    {
                        Output.WriteLine("<br /><br /><center><b>This user has not added any descriptive tags to any items or it is not a valid userid.</b></center><br /><br />");
                    }
                }
            }

            Output.WriteLine("</div>");
        }
    }
}
