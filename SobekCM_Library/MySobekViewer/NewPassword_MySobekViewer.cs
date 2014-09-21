#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Core.Users;
using SobekCM.Tools;
using SobekCM_UI_Library.Navigation;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows an authenticated user to change their password </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer for changing their password </li>
    /// </ul></remarks>
    public class NewPassword_MySobekViewer : abstract_MySobekViewer
    {
        private TextBox confirmPasswordBox;
        private TextBox existingPasswordBox;
        private TextBox passwordBox;
        private readonly List<string> validationErrors;

        /// <summary> Constructor for a new instance of the NewPassword_MySobekViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public NewPassword_MySobekViewer(User_Object User, Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("NewPassword_MySobekViewer.Constructor", String.Empty);

            // Do nothing
            validationErrors = new List<string>();
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'Change your password' </value>
        public override string Web_Title
        {
            get 
            {
                return "Change your password";
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since this form is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("NewPassword_MySobekViewer.Write_HTML", "Do nothing");
        }

		/// <summary> Add controls directly to the form in the main control area placeholder </summary>
        /// <param name="MainPlaceHolder"> Main place holder to which all main controls are added </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> The bulk of this page is added here, as controls </remarks>
        public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("NewPassword_MySobekViewer.Add_Controls", "");

            // Is this for registration?       
            bool registration = (HttpContext.Current.Session["user"] == null);
            if (registration)
            {
                user = new User_Object();
            }

            string current_password = String.Empty;
            string new_password = String.Empty;
            string new_password2 = String.Empty;

            if (currentMode.isPostBack)
            {
                // Loop through and get the dataa
                string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
                foreach (string thisKey in getKeys)
                {
                    switch (thisKey)
                    {
                        case "current_password_enter":
                            current_password = HttpContext.Current.Request.Form[thisKey];
                            break;

                        case "new_password_enter":
                            new_password = HttpContext.Current.Request.Form[thisKey];
                            break;

                        case "new_password_confirm":
                            new_password2 = HttpContext.Current.Request.Form[thisKey];
                            break;
                    }
                }

                if ((new_password.Trim().Length == 0) || (new_password2.Trim().Length == 0))
                    validationErrors.Add("Select and confirm a new password");
                if (new_password != new_password2)
                    validationErrors.Add("New passwords do not match");
                else if ((new_password.Length < 8) && ( new_password.Length > 0 ))
                    validationErrors.Add("Password must be at least eight digits");
                if (validationErrors.Count == 0)
                {
                    if (new_password == current_password)
                    {
                        validationErrors.Add("The new password cannot match the old password");
                    }
                }

                if (validationErrors.Count == 0)
                {
                    bool success = SobekCM_Database.Change_Password(user.UserName, current_password, new_password, false, Tracer);
                    if (success)
                    {
                        user.Is_Temporary_Password = false;
                        // Forward back to their original URL (unless the original URL was this logon page)
                        string raw_url = (HttpContext.Current.Request.RawUrl);
                        if (raw_url.ToUpper().IndexOf("M=HML") > 0)
                        {
                            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                            currentMode.Redirect();
                            return;
                        }
                        else
                        {
                            HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl, false);
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            currentMode.Request_Completed = true;
                            return;
                        }
                    }
                    else
                    {
                        validationErrors.Add("Unable to change password.  Verify current password.");
                    }
                }
            }

            StringBuilder literalBuilder = new StringBuilder(1000);
            literalBuilder.AppendLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_metadata.js\" type=\"text/javascript\"></script>");
            literalBuilder.AppendLine("<div class=\"SobekHomeText\" >");
            literalBuilder.AppendLine("<br />");
            literalBuilder.AppendLine("<blockquote>");
            literalBuilder.AppendLine(user.Is_Temporary_Password
                                          ? "You are required to change your password to continue."
                                          : "Please enter your existing password and your new password.");
            if (validationErrors.Count > 0)
            {
                literalBuilder.AppendLine("<br /><br /><strong><span style=\"color: Red\">The following errors were detected:");
                literalBuilder.AppendLine("<blockquote>");
                foreach (string thisError in validationErrors)
                {
                    literalBuilder.AppendLine(thisError + "<br />");
                }
                literalBuilder.AppendLine("</blockquote>");
                literalBuilder.AppendLine("</span></strong>");
            }
            literalBuilder.AppendLine("</blockquote>");
            literalBuilder.AppendLine("<table width=\"700px\"><tr><td width=\"180px\">&nbsp;</td>");
            literalBuilder.AppendLine("<td width=\"200px\"><label for=\"current_password_enter\">Existing Password:</label></td>");
            literalBuilder.AppendLine("<td width=\"180px\">");
            LiteralControl literal1 = new LiteralControl(literalBuilder.ToString());
            literalBuilder.Remove(0, literalBuilder.Length);
            MainPlaceHolder.Controls.Add(literal1);

            existingPasswordBox = new TextBox
                                      {
                                          CssClass = "preferences_small_input",
                                          ID = "current_password_enter",
                                          TextMode = TextBoxMode.Password
                                      };
            existingPasswordBox.Attributes.Add("onfocus", "textbox_enter('current_password_enter', 'preferences_small_input_focused')");
            existingPasswordBox.Attributes.Add("onblur", "textbox_leave('current_password_enter', 'preferences_small_input')");
            MainPlaceHolder.Controls.Add(existingPasswordBox);

            LiteralControl literal2 = new LiteralControl("</td><td width=\"140px\">&nbsp;</td></tr>" + Environment.NewLine + "<tr><td>&nbsp;</td><td><label for=\"new_password_enter\">New Password:</label></td><td>");
            MainPlaceHolder.Controls.Add(literal2);

            passwordBox = new TextBox
                              {
                                  CssClass = "preferences_small_input",
                                  ID = "new_password_enter",
                                  TextMode = TextBoxMode.Password
                              };
            passwordBox.Attributes.Add("onfocus", "textbox_enter('new_password_enter', 'preferences_small_input_focused')");
            passwordBox.Attributes.Add("onblur", "textbox_leave('new_password_enter', 'preferences_small_input')");
            MainPlaceHolder.Controls.Add(passwordBox);

            LiteralControl literal3 = new LiteralControl("</td><td>&nbsp;</td></tr>" + Environment.NewLine + "<tr><td>&nbsp;</td><td><label for=\"new_password_confirm\">Confirm New Password:</label></td><td>");
            MainPlaceHolder.Controls.Add(literal3);

            confirmPasswordBox = new TextBox
                                     {
                                         CssClass = "preferences_small_input",
                                         ID = "new_password_confirm",
                                         TextMode = TextBoxMode.Password
                                     };
            confirmPasswordBox.Attributes.Add("onfocus", "textbox_enter('new_password_confirm', 'preferences_small_input_focused')");
            confirmPasswordBox.Attributes.Add("onblur", "textbox_leave('new_password_confirm', 'preferences_small_input')");
            MainPlaceHolder.Controls.Add(confirmPasswordBox);

            literalBuilder.AppendLine("   </td><td>&nbsp;</td></tr>");
            literalBuilder.AppendLine("  <tr align=\"right\" valign=\"bottom\" height=\"50px\" ><td colspan=\"3\">");
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Log_Out;
            literalBuilder.AppendLine("    <a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button.gif\" border=\"0\" alt=\"CANCEL\" /></a> &nbsp; ");
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.New_Password;

            LiteralControl literal4 = new LiteralControl(literalBuilder.ToString());
            MainPlaceHolder.Controls.Add(literal4);

            // Add the submit button
            ImageButton submitButton = new ImageButton
                                           {
                                               ImageUrl = currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button.gif",
                                               AlternateText = "SAVE"
                                           };
            submitButton.Click += submitButton_Click;
            MainPlaceHolder.Controls.Add(submitButton);

            LiteralControl literal5 = new LiteralControl("</td></tr></table></blockquote></div>\n\n<!-- Focus on current password text box -->\n<script type=\"text/javascript\">focus_element('current_password_enter');</script>\n\n");
            MainPlaceHolder.Controls.Add(literal5);

        }

        void submitButton_Click(object sender, ImageClickEventArgs e)
        {
            
        }
    }
}



