// HTML5 10/15/2013

#region Using directives

using System;
using System.IO;
using System.Web;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Tools;
using SobekCM_UI_Library.Navigation;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows a user to logon, either with using the mySobek authentication, or clicking the link for Gatorlink authentication </summary>
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
		/// <param name="CurrentMode"> Mode / navigation information for the current request (including interface code) </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Logon_MySobekViewer(SobekCM_Navigation_Object CurrentMode, Custom_Tracer Tracer)
            : base(null)
        {
	        Tracer.Add_Trace("Logon_MySobekViewer.Constructor", String.Empty);

			this.CurrentMode = CurrentMode;

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

                if ((!String.IsNullOrEmpty(possible_password)) && (!String.IsNullOrEmpty(possible_username)))
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
                            currentMode.Redirect();
                        }
                        else
                        {
                            HttpContext.Current.Response.Redirect(raw_url, false);
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            currentMode.Request_Completed = true;
                        }
                    }
                    else
                    {
	                    errorMessage = "Invalid user/password entered";
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
        /// <value> The value of this message changes depending on which instance or by which URL the query arrives ( i.e., UDC, dLOC, etc.. )</value>
        public override string Web_Title
        {
            get 
            {
                return "Logon to My" + currentMode.SobekCM_Instance_Abbreviation;
            }
        }

		/// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
	    {
			// Get ready to draw the tabs
			string my_sobek = "my" + currentMode.SobekCM_Instance_Abbreviation;

			Output.WriteLine("<br />");
			Output.WriteLine("<h1>Logon to " + my_sobek + "</h1>");
			Output.WriteLine();

			// If there was an error, show it
			if (errorMessage.Length > 0)
			{
				Output.WriteLine("<div class=\"sbkLomv_ErrorMsg\">" + errorMessage + "</div>");
			}

			Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_metadata.js\" type=\"text/javascript\"></script>");
			Output.WriteLine("<div class=\"sbkMySobek_HomeText\" >");
			Output.WriteLine("  <br />");
			Output.WriteLine("  <p>The feature you are trying to access requires a valid logon.<p>");
			Output.WriteLine("  <p>Please choose the appropriate logon directly below.</p>");
			Output.WriteLine("  <ul id=\"sbkLomv_OptionsList\">");

			if (currentMode.SobekCM_Instance_Abbreviation == "dLOC")
			{
				Output.WriteLine("    <li><span style=\"font-weight:bold\">If you have a valid myDLOC logon</span>, <a id=\"form_logon_term\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return popup_mysobek_form('form_logon', 'logon_username');\">Sign on with myDLOC authentication</a>.</li>");

				if ((SobekCM_Library_Settings.Shibboleth_System_URL.Length > 0) && (SobekCM_Library_Settings.Shibboleth_System_Name.Length > 0))
				{
					Output.WriteLine("    <li><span style=\"font-weight:bold\">If you have a valid " + SobekCM_Library_Settings.Shibboleth_System_Name + " ID</span>, <a href=\"" + SobekCM_Library_Settings.Shibboleth_System_URL + "\">Sign on with your " + SobekCM_Library_Settings.Shibboleth_System_Name + " here</a>.</li>");
				}
			}
			else
			{
				if ((SobekCM_Library_Settings.Shibboleth_System_URL.Length > 0) && (SobekCM_Library_Settings.Shibboleth_System_Name.Length > 0))
				{
					Output.WriteLine("    <li><span style=\"font-weight:bold\">If you have a valid " + SobekCM_Library_Settings.Shibboleth_System_Name + " ID</span>, <a href=\"" + SobekCM_Library_Settings.Shibboleth_System_URL + "\">Sign on with your " + SobekCM_Library_Settings.Shibboleth_System_Name + " here</a>.</li>");
				}

				Output.WriteLine("    <li><span style=\"font-weight:bold\">If you have a valid my" + currentMode.SobekCM_Instance_Abbreviation + " logon</span>, <a id=\"form_logon_term\" href=\"" + currentMode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return popup_mysobek_form('form_logon', 'logon_username');\">Sign on with my" + currentMode.SobekCM_Instance_Abbreviation + " authentication here</a>.</li>");
			}

			currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
			Output.Write("    <li><span style=\"font-weight:bold\">Not registered yet?</span> <a href=\"" + currentMode.Redirect_URL() + "\">Register now</a> or ");

			currentMode.Mode = Display_Mode_Enum.Contact;
			Output.WriteLine(" <a href=\"" + currentMode.Redirect_URL() + "\">Contact Us</a></li>");
			currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
			currentMode.Mode = Display_Mode_Enum.My_Sobek;

			Output.WriteLine("  </ul>");

			Output.WriteLine("</div>");

			Output.WriteLine("<br />");
			Output.WriteLine("<br />");
	    }

	    /// <summary> Add the HTML to be added near the top of the page for those viewers that implement pop-up forms for data retrieval </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        ///  <remarks> This adds the pop-up form for logging on through mySobek authentication </remarks>
        public override void Add_Popup_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Logon_MySobekViewer.Add_Popup_HTML", "Add any popup divisions for form elements");

			Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.3.custom.min.js\"></script>");

            // Add the popup form
            Output.WriteLine("<!-- mySobek Log On Form -->");
			Output.WriteLine("<div class=\"sbkLomv_PopupDiv\" id=\"form_logon\" style=\"display:none;\">");
			Output.WriteLine("  <div class=\"sbkMySobek_PopupTitle\"><table style=\"width:100%;\"><tr><td style=\"text-align:left;\">LOG IN</td><td style=\"text-align:right;\"><a href=\"" + currentMode.Base_URL + "logon/help\" target=\"_FORM_SIGNON_HELP\" >?</a> &nbsp; <a href=\"#template\" onclick=\"return close_mysobek_form('form_logon');\">X</a> &nbsp; </td></tr></table></div>");
            Output.WriteLine("  <br />");
			Output.WriteLine("  <table class=\"sbkMySobek_PopupTable\">");

            // Add the rows of data
			Output.WriteLine("    <tr><td style=\"width:140px;\">Username or email:</td><td><input class=\"sbkLomv_username_input sbkMySobek_Focusable\" name=\"logon_username\" id=\"logon_username\" type=\"text\" value=\"\" onkeydown=\"logonTrapKD(event);\" /></td></tr>");
			Output.WriteLine("    <tr><td>Password:</td><td><input class=\"sbkLomv_password_input sbkMySobek_Focusable\" name=\"logon_password\" id=\"logon_password\" type=\"password\" value=\"\" onkeydown=\"logonTrapKD(event);\" /></td></tr>");
			Output.WriteLine("    <tr><td>&nbsp;</td><td><input type=\"checkbox\" value=\"rememberme\" class=\"sbkMySobek_checkbox\" name=\"rememberme\" id=\"rememberme\" /> <label for=\"rememberme\">Remember me</label><br /><br /></td></tr>");

			// Add the buttons 
			Output.WriteLine("    <tr style=\"height:35px; text-align: center; vertical-align: bottom;\">");
			Output.WriteLine("      <td colspan=\"2\">");
			Output.WriteLine("        <button title=\"Close\" class=\"sbkMySobek_BigButton\" onclick=\"return close_mysobek_form('form_logon');\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL &nbsp; </button> &nbsp; &nbsp; ");
			Output.WriteLine("        <button title=\"Login\" class=\"sbkMySobek_BigButton\" type=\"submit\"> &nbsp; LOGIN <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
			Output.WriteLine("      </td>");
			Output.WriteLine("    </tr>");

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
