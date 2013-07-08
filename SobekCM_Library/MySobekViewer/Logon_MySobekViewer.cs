#region Using directives

using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows a user to logon, either with using the myUFDC authentication, or clicking the link for Gatorlink authentication </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to allow the user to logon </li>
    /// </ul></remarks>
    public class Logon_MySobekViewer : abstract_MySobekViewer
    {
        private readonly string errorMessage;

        /// <summary> Constructor for a new instance of the Home_MySobekViewer class </summary>
        /// <param name="currentMode"> Mode / navigation information for the current request (including interface code) </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Logon_MySobekViewer(SobekCM_Navigation_Object currentMode, Custom_Tracer Tracer)
            : base(null)
        {
                Tracer.Add_Trace("Logon_MySobekViewer.Constructor", String.Empty);

                CurrentMode = currentMode;

                errorMessage = String.Empty;

                // If this is a postback, check to see if the user is valid
                if (currentMode.isPostBack)
                {
                    string possible_username = String.Empty;
                    string possible_password = String.Empty;
                    bool remember_me = false;

                    string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
                    foreach (string thisKey in getKeys)
                    {
                        switch (thisKey)
                        {
                            case "logon_username":
                                possible_username = HttpContext.Current.Request.Form[thisKey].Trim();
                                break;

                            case "logon_password":
                                possible_password = HttpContext.Current.Request.Form[thisKey].Trim();
                                break;

                            case "rememberme":
                                if (HttpContext.Current.Request.Form[thisKey].Trim() == "rememberme")
                                    remember_me = true;
                                break;
                        }
                    }

                    if (( !String.IsNullOrEmpty(possible_password) ) && ( !String.IsNullOrEmpty(possible_username)))
                    {
                        user = SobekCM_Database.Get_User(possible_username, possible_password, Tracer);
                        if (user != null)
                        {
                            // The user was valid here, so save this user information
                            HttpContext.Current.Session["user"] = user;

                            // Should we remember this user via cookies?
                            if (remember_me)
                            {
                                HttpCookie userCookie = new HttpCookie("SobekUser");
                                userCookie.Values["userid"] = user.UserID.ToString();
                                userCookie.Values["security_hash"] = user.Security_Hash(HttpContext.Current.Request.UserHostAddress);
                                userCookie.Expires = DateTime.Now.AddDays(14);
                                HttpContext.Current.Response.Cookies.Add(userCookie);
                            }

                            // Forward back to their original URL (unless the original URL was this logon page)
                            string raw_url = HttpContext.Current.Items["Original_URL"].ToString();
                            if (raw_url.ToLower().IndexOf("my/logon") > 0)
                            {
                                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                                HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
                            }
                            else
                            {
                                HttpContext.Current.Response.Redirect(raw_url);
                            }
                        }
                    }
                }
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

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> The value of this message changes depending on which instance or by which URL the query arrives ( i.e., UFDC, dLOC, etc.. )</value>
        public override string Web_Title
        {
            get 
            {
                return "Logon to My" + currentMode.SobekCM_Instance_Abbreviation;
            }
        }

        /// <summary> Add controls directly to the form, either to the main control area or to the file upload placeholder </summary>
        /// <param name="placeHolder"> Main place holder to which all main controls are added </param>
        /// <param name="uploadFilesPlaceHolder"> Place holder is used for uploading file </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        ///  <remarks> No controls are added here, although some children class override this virtual method to add controls </remarks>
        public override void Add_Controls(PlaceHolder placeHolder, PlaceHolder uploadFilesPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Logon_MySobekViewer.Add_Controls", String.Empty);
            StringBuilder literalBuilder = new StringBuilder();

            // Get ready to draw the tabs
            string sobek_home = currentMode.SobekCM_Instance_Abbreviation.ToUpper() + " HOME";
            string my_sobek = "my" + currentMode.SobekCM_Instance_Abbreviation;

            literalBuilder.AppendLine("<div class=\"ViewsBrowsesRow\">");
            literalBuilder.AppendLine("");

            // Write the Sobek home tab
            currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
            literalBuilder.AppendLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + sobek_home + Unselected_Tab_End + "</a>");
            currentMode.Mode = Display_Mode_Enum.My_Sobek;
            literalBuilder.AppendLine("");
            literalBuilder.AppendLine("</div>");
            literalBuilder.AppendLine();

            literalBuilder.AppendLine("<div class=\"SobekSearchPanel\">");
            literalBuilder.AppendLine("  <h1>Logon to " + my_sobek + "</h1>");
            literalBuilder.AppendLine("</div>");
            literalBuilder.AppendLine();

            literalBuilder.AppendLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_metadata.js\" type=\"text/javascript\"></script>");
            literalBuilder.AppendLine("<div class=\"SobekHomeText\" >");
            literalBuilder.AppendLine("<br />");
            literalBuilder.AppendLine("<blockquote>");
            literalBuilder.AppendLine("The feature you are trying to access requires a valid logon.<br /><br />Please choose the appropriate logon directly below.  <br /><br />");
            literalBuilder.AppendLine("<ul>");
            if (currentMode.SobekCM_Instance_Abbreviation == "dLOC")
            {
                literalBuilder.AppendLine("<li><b>If you have a valid myDLOC logon</b>, <a id=\"form_logon_term\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return popup_focus('form_logon', 'form_logon_term', 'logon_username', 280, 400 );\">Sign on with myDLOC authentication</a>.<br /><br /></li>");

                if ((SobekCM_Library_Settings.Shibboleth_System_URL.Length > 0) && ( SobekCM_Library_Settings.Shibboleth_System_Name.Length > 0 ))
                {
                    literalBuilder.AppendLine("<li><b>If you have a valid " + SobekCM_Library_Settings.Shibboleth_System_Name + " ID</b>, <a href=\"" + SobekCM_Library_Settings.Shibboleth_System_URL + "\">Sign on with your " + SobekCM_Library_Settings.Shibboleth_System_Name + " here</a>.<br /><br /></li>");
                }
            }
            else
            {
                if ((SobekCM_Library_Settings.Shibboleth_System_URL.Length > 0) && (SobekCM_Library_Settings.Shibboleth_System_Name.Length > 0))
                {
                    literalBuilder.AppendLine("<li><b>If you have a valid " + SobekCM_Library_Settings.Shibboleth_System_Name + " ID</b>, <a href=\"" + SobekCM_Library_Settings.Shibboleth_System_URL + "\">Sign on with your " + SobekCM_Library_Settings.Shibboleth_System_Name + " here</a>.<br /><br /></li>");
                } 
                
                literalBuilder.AppendLine("<li><b>If you have a valid my" + currentMode.SobekCM_Instance_Abbreviation + " logon</b>, <a id=\"form_logon_term\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return popup_focus('form_logon', 'form_logon_term', 'logon_username', 280, 400 );\">Sign on with my" + currentMode.SobekCM_Instance_Abbreviation + " authentication here</a>.<br /><br /></li>");
            }

            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
            literalBuilder.Append("<li><b>Not registered yet?</b> <a href=\"" + currentMode.Redirect_URL() + "\">Register now</a> or ");

            currentMode.Mode = Display_Mode_Enum.Contact;
            literalBuilder.AppendLine(" <a href=\"" + currentMode.Redirect_URL() + "\">Contact Us</a></li>");
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
            currentMode.Mode = Display_Mode_Enum.My_Sobek;

            literalBuilder.AppendLine("</ul>");
            literalBuilder.AppendLine("</blockquote>");
            literalBuilder.AppendLine("<br />");

            literalBuilder.AppendLine("</div>");
            literalBuilder.AppendLine("<br />");
            literalBuilder.AppendLine("<br />");

            LiteralControl literal2 = new LiteralControl(literalBuilder.ToString());
            placeHolder.Controls.Add(literal2);
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of any form) </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This writes any error message that may exist during the last logon attempt </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (errorMessage.Length > 0)
            {
                Tracer.Add_Trace("Logon_MySobekViewer.Write_HTML", "Writing error message");
                Output.WriteLine("<br />");
                Output.WriteLine("<span style=\"color: red;\"><b>" + errorMessage + "</b></span>");
            }
        }

        /// <summary> Add the HTML to be added near the top of the page for those viewers that implement pop-up forms for data retrieval </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        ///  <remarks> This adds the pop-up form for logging on through myUFDC authentication </remarks>
        public override void Add_Popup_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Logon_MySobekViewer.Add_Popup_HTML", "Add any popup divisions for form elements");

            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.8.16.custom.min.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");

            // Add the popup form
            Output.WriteLine("<!-- mySobek Log On Form -->");
            Output.WriteLine("<div class=\"logon_popup_div\" id=\"form_logon\" style=\"display:none;\">");
            Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">LOG IN</td><td align=\"right\"><a href=\"" + currentMode.Base_URL + "logon/help\" target=\"_FORM_SIGNON_HELP\" >?</a> &nbsp; <a href=\"#template\" onclick=\"close_logon_form()\">X</a> &nbsp; </td></tr></table></div>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <table class=\"popup_table\">");

            // Add the rows of data
            Output.WriteLine("    <tr><td width=\"140px\">Username or email:</td><td><input class=\"logon_username_input\" name=\"logon_username\" id=\"logon_username\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('logon_username', 'logon_username_input_focused')\" onblur=\"javascript:textbox_leave('logon_username', 'logon_username_input')\" onkeydown=\"logonTrapKD(event);\" /></td></tr>");
            Output.WriteLine("    <tr><td>Password:</td><td><input class=\"logon_password_input\" name=\"logon_password\" id=\"logon_password\" type=\"password\" value=\"\" onfocus=\"javascript:textbox_enter('logon_password', 'logon_password_input_focused')\" onblur=\"javascript:textbox_leave('logon_password', 'logon_password_input')\" onkeydown=\"logonTrapKD(event);\" /></td></tr>");
            Output.WriteLine("    <tr><td>&nbsp;</td><td><input type=\"checkbox\" value=\"rememberme\" name=\"rememberme\" id=\"rememberme\" /> Remember me<br /><br /></td></tr>");

            Output.WriteLine("    <tr><td colspan=\"2\"><center>");
            Output.WriteLine("      <a href=\"#template\" onclick=\"return close_logon_form();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_big_button.gif\" alt=\"CANCEL\" /></a> &nbsp; ");
            Output.WriteLine("      <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/login_big_button.gif\" value=\"Submit\" alt=\"Submit\" />");
            Output.WriteLine("    </center></td></tr>");

            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
            Output.WriteLine("    <tr><td colspan=\"2\"><br />Not registered yet?  <a href=\"" + currentMode.Redirect_URL() + "\">Register now</a>.");
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
            currentMode.Mode = Display_Mode_Enum.Contact;
            Output.WriteLine("    <br /><br />Forgot your username or password?  Please <a href=\"" + currentMode.Redirect_URL() + "\">contact us</a>.</td></tr>");
            currentMode.Mode = Display_Mode_Enum.My_Sobek;

            // Finish the popup form
            Output.WriteLine("  </table>");
            Output.WriteLine("  <br />");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }
    }
}
