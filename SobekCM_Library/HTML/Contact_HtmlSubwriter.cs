#region Using directives

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SobekCM.Core;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Email;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Contact html subwriter renders the contact us screen and the subsequent 'Your message was sent' screen </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
    public class Contact_HtmlSubwriter : abstractHtmlSubwriter
    {
        private readonly string lastMode;
        private readonly ContactForm_Configuration configuration;
        private readonly Dictionary<string, string> postBackValues;
        private string errorMsg;
        private bool email_invalid;

        /// <summary> Constructor for a new instance of the Contact_HtmlSubwriter class </summary>
        /// <param name="Last_Mode"> URL for the last mode this user was in before selecting contact us</param>
        /// <param name="UserHistoryRequestInfo"> Some history and user information to include in the final email </param>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Contact_HtmlSubwriter(string Last_Mode, string UserHistoryRequestInfo, RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            // Save the parameters
            lastMode = Last_Mode;

            // Set the error message to an empty string to start with
            errorMsg = String.Empty;

            // Determine the configuration to use for this contact us form
            configuration = UI_ApplicationCache_Gateway.Configuration.ContactForm;
            if ((RequestSpecificValues.Hierarchy_Object != null) && (RequestSpecificValues.Hierarchy_Object.ContactForm != null))
                configuration = RequestSpecificValues.Hierarchy_Object.ContactForm;

            postBackValues = new Dictionary<string, string>();
            foreach (string thisKey in HttpContext.Current.Request.Form.AllKeys)
            {
                if (thisKey != "item_action")
                {
                    string value = HttpContext.Current.Request.Form[thisKey];
                    if (!String.IsNullOrEmpty(value))
                    {
                        postBackValues[thisKey] = value;
                    }
                }
            }

            // If this is a post back, send email
            if (HttpContext.Current.Request.Form["item_action"] == null) return;

            string action = HttpContext.Current.Request.Form["item_action"];
            if (action == "email")
            {
                // Some values to collect information
                string subject = "Contact [" + RequestSpecificValues.Current_Mode.Instance_Abbreviation + " Submission]";
                string message_from = RequestSpecificValues.Current_Mode.Instance_Abbreviation + "<" + UI_ApplicationCache_Gateway.Settings.Email.Setup.DefaultFromAddress + ">";
                if (!String.IsNullOrEmpty(UI_ApplicationCache_Gateway.Settings.Email.Setup.DefaultFromDisplay))
                {
                    message_from = UI_ApplicationCache_Gateway.Settings.Email.Setup.DefaultFromDisplay + "<" + UI_ApplicationCache_Gateway.Settings.Email.Setup.DefaultFromAddress + ">";
                }
                int text_area_count = configuration.TextAreaElementCount;
                StringBuilder emailBuilder = new StringBuilder();

                // Make sure all the required fields are completed and build the emails
                StringBuilder errorBuilder = new StringBuilder();
                int control_count = 1;
                foreach (ContactForm_Configuration_Element thisElement in configuration.FormElements)
                {
                    if ((thisElement.Element_Type != ContactForm_Configuration_Element_Type_Enum.HiddenValue) && (thisElement.Element_Type != ContactForm_Configuration_Element_Type_Enum.ExplanationText))
                    {
                        // Determine the name of this control
                        string control_name = String.Empty;
                        if (!String.IsNullOrEmpty(thisElement.Name))
                            control_name = thisElement.Name.Replace(" ", "_");
                        if (thisElement.Element_Type == ContactForm_Configuration_Element_Type_Enum.Subject)
                            control_name = "subject";
                        if (thisElement.Element_Type == ContactForm_Configuration_Element_Type_Enum.Email)
                            control_name = "email";
                        if (String.IsNullOrEmpty(control_name))
                            control_name = "Control" + control_count;

                        if (!postBackValues.ContainsKey(control_name))
                        {
                            if (thisElement.Required)
                                errorBuilder.Append(thisElement.QueryText.Get_Value(RequestSpecificValues.Current_Mode.Language).Replace(":", "") + "<br />");
                        }
                        else
                        {
                            if (thisElement.Element_Type == ContactForm_Configuration_Element_Type_Enum.Subject)
                            {
                                subject = postBackValues[control_name] + " [" + RequestSpecificValues.Current_Mode.Instance_Abbreviation + " Submission]";
                            }
                            else if (thisElement.Element_Type == ContactForm_Configuration_Element_Type_Enum.Email)
                            {
                                string entered_message_from = postBackValues[control_name];

                                if (!IsValidEmail(entered_message_from))
                                {
                                    errorBuilder.Append(thisElement.QueryText.Get_Value(RequestSpecificValues.Current_Mode.Language).Replace(":", "") + " (INVALID) <br />");
                                }

                                message_from = RequestSpecificValues.Current_Mode.Instance_Abbreviation + "<" + entered_message_from + ">";
                                if (!String.IsNullOrEmpty(UI_ApplicationCache_Gateway.Settings.Email.Setup.DefaultFromDisplay))
                                {
                                    message_from = UI_ApplicationCache_Gateway.Settings.Email.Setup.DefaultFromDisplay + "<" + entered_message_from + ">";
                                }

                                emailBuilder.Append("Email:\t\t" + entered_message_from + "\n");

                            }
                            else if (thisElement.Element_Type == ContactForm_Configuration_Element_Type_Enum.TextArea)
                            {
                                if (text_area_count == 1)
                                {
                                    emailBuilder.Insert(0, postBackValues[control_name] + "\n\n");
                                }
                                else
                                {
                                    if (emailBuilder.Length > 0)
                                        emailBuilder.Append("\n");
                                    emailBuilder.Append(thisElement.QueryText.Get_Value(RequestSpecificValues.Current_Mode.Language) + "\n");
                                    emailBuilder.Append(postBackValues[control_name] + "\n\n");
                                }
                            }
                            else
                            {

                                emailBuilder.Append(control_name.Replace("_", " ") + ":\t\t" + postBackValues[control_name] + "\n");
                            }
                        }

                        control_count++;
                    }
                }

                if (errorBuilder.Length > 0)
                {
                    errorMsg = errorBuilder.ToString();
                    return;
                }

                // Create the final body
                string email_body = emailBuilder + "\n\n" + UserHistoryRequestInfo;

                // Determine the sendee
                string sendTo = UI_ApplicationCache_Gateway.Settings.Email.System_Email;
                if ((RequestSpecificValues.Hierarchy_Object != null) && (!String.IsNullOrEmpty(RequestSpecificValues.Hierarchy_Object.Contact_Email)))
                {
                    sendTo = RequestSpecificValues.Hierarchy_Object.Contact_Email.Replace(";", ",");
                }

                int userid = -1;
                if (RequestSpecificValues.Current_User != null)
                    userid = RequestSpecificValues.Current_User.UserID;

                EmailInfo newEmail = new EmailInfo
                {
                    Body = email_body,
                    isContactUs = true,
                    isHTML = false,
                    RecipientsList = sendTo,
                    Subject = subject,
                    UserID = userid,
                    FromAddress = message_from
                };

                string error_msg;
                bool email_error = !Email_Helper.SendEmail(newEmail, out error_msg);


                // Send back to the home for this collection, sub, or group
                if (email_error)
                {
                    HttpContext.Current.Response.Redirect(UI_ApplicationCache_Gateway.Settings.Servers.System_Error_URL, false);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    // Send back to the home for this collection, sub, or group
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Contact_Sent;
                    UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                }
            }
        }

        private bool IsValidEmail(string strIn)
        {
            if (String.IsNullOrEmpty(strIn))
                return false;

            email_invalid = false;

            // Use IdnMapping class to convert Unicode domain names. 
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (email_invalid)
                return false;

            // Return true if strIn is in valid e-mail format. 
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                email_invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }

        /// <summary> Write any additional values within the HTML Head of the
        /// final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> By default this does nothing, but can be overwritten by all the individual html subwriters </remarks>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            // Based on display mode, add ROBOT instructions
            switch (RequestSpecificValues.Current_Mode.Mode)
            {
                case Display_Mode_Enum.Contact:
                    Output.WriteLine("  <meta name=\"robots\" content=\"index, nofollow\" />");
                    break;

                case Display_Mode_Enum.Contact_Sent:
                    Output.WriteLine("  <meta name=\"robots\" content=\"noindex, nofollow\" />");
                    break;
            }
        }

        /// <summary> Title for this web page </summary>
        public override string WebPage_Title
        {
            get
            {
                return RequestSpecificValues.Current_Mode.Mode == Display_Mode_Enum.Contact_Sent ? "{0} Contact Sent" : "{0} Contact Us";
            }
        }

        /// <summary> Writes the HTML generated by this contact us html subwriter directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE -- Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Contact_HtmlSubwriter.Write_HTML", "Rendering HTML");

            // Start the page container
            Output.WriteLine("<div id=\"pagecontainer\">");
            Output.WriteLine("<br />");

            Output.WriteLine("<br /><br />");
            Output.WriteLine();

            if (RequestSpecificValues.Current_Mode.Mode == Display_Mode_Enum.Contact_Sent)
            {
                Output.WriteLine("<div class=\"SobekHomeText\">");
                Output.WriteLine("<table width=\"700\" border=\"0\" align=\"center\">");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td align=\"center\" >");
                Output.WriteLine("      <b>Your email has been sent.</b>");
                Output.WriteLine("      <br /><br />");
                Output.WriteLine("      <a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "\">Click here to return to the digital collection home</a>");
                Output.WriteLine("      <br /><br />");
                if (( !String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Browser_Type)) && (RequestSpecificValues.Current_Mode.Browser_Type.IndexOf("IE") >= 0))
                {
                    Output.WriteLine("      <a href=\"javascript:window.close();\">Click here to close this tab in your browser</a>");
                }
                Output.WriteLine("    </td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("</table>");
                Output.WriteLine("<br /><br />");
                Output.WriteLine("</div>");
            }
            else
            {
                string contact_us_title = "Contact Us";
                string please_complete = "Please complete the following required fields:";
                string submit = "Submit";
                string cancel = "Cancel";

                if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
                {
                    contact_us_title = "Contactez Nous";
                    please_complete = "Veuillez remplir les champs obligatoires indiqués:";
                    submit = "Soumettre";
                    cancel = "Annuler";
                }

                if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
                {
                    contact_us_title = "Contáctenos";
                    please_complete = "Por Favor llene la información Requerida:";
                    submit = "Mandar";
                    cancel = "Cancelar";
                }

                // Start this form
                string post_url = HttpUtility.HtmlEncode(HttpContext.Current.Items["Original_URL"].ToString());
                Output.WriteLine("<form name=\"email_form\" method=\"post\" action=\"" + post_url + "\" id=\"addedForm\" >");

                // Add the hidden field
                Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
                Output.WriteLine("<input type=\"hidden\" id=\"item_action\" name=\"item_action\" value=\"\" />");


                Output.WriteLine("<div class=\"sbkChsw_Panel\">");
                Output.WriteLine("  <h1>" + contact_us_title + "</h1>");

                if (errorMsg.Length > 0)
                {
                    Output.WriteLine("  <div class=\"sbkChsw_Errors\">" + please_complete + "<blockquote>" + errorMsg + "</blockquote></div>");
                }

                Output.WriteLine("  <div class=\"sbkChsw_SubPanel\">");

                //if ((RequestSpecificValues.Current_Mode.Aggregation == "juv") || (RequestSpecificValues.Current_Mode.Aggregation == "alice"))
                //{
                //    Output.WriteLine("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "bookvalue\" target=\"_bookvalue\">Click here if you are asking about your own copy of a book.</a><br /><br />");
                //}

                string firstControl = String.Empty;
                bool inElementBlock = false;
                int control_count = 1;
                foreach (ContactForm_Configuration_Element thisElement in configuration.FormElements)
                {
                    if (thisElement.Element_Type == ContactForm_Configuration_Element_Type_Enum.HiddenValue)
                    {
                        if ((thisElement.UserAttribute != User_Object_Attribute_Mapping_Enum.NONE) && (RequestSpecificValues.Current_User != null))
                        {
                            string userAttrValue = RequestSpecificValues.Current_User.Get_Value_By_Mapping(thisElement.UserAttribute);
                            Output.WriteLine("    <input type=\"hidden\" id=\"" + thisElement.Name + "\" name=\"" + thisElement.Name + "\" value=\"" + HttpContext.Current.Server.HtmlEncode(userAttrValue) + "\" />");
                        }
                        else
                        {
                            Output.WriteLine("    <input type=\"hidden\" id=\"" + thisElement.Name + "\" name=\"" + thisElement.Name + "\" value=\"\" />");
                        }
                    }
                    else if (thisElement.Element_Type == ContactForm_Configuration_Element_Type_Enum.ExplanationText)
                    {
                        // If this is in an element block, close that now
                        if (inElementBlock)
                        {
                            Output.WriteLine("    </div>");
                            inElementBlock = false;
                        }

                        // This is an explanation text, or prompt for the user
                        string cssClass = "sbkChsw_ExplanationText";
                        if (!String.IsNullOrEmpty(thisElement.CssClass))
                            cssClass = thisElement.CssClass;
                        Output.WriteLine("    <div class=\"" + cssClass + "\">" + thisElement.QueryText.Get_Value(RequestSpecificValues.Current_Mode.Language) + "</div>");
                    }
                    else
                    {
                        // This is a user input element

                        // If this is NOT in an element block, close that now
                        if (!inElementBlock)
                        {
                            Output.WriteLine("    <div class=\"sbkChsw_ElementBlock\">");
                            inElementBlock = true;
                        }

                        // Determine the name of this control
                        string control_name = String.Empty;
                        if (!String.IsNullOrEmpty(thisElement.Name))
                            control_name = thisElement.Name.Replace(" ", "_");
                        if (thisElement.Element_Type == ContactForm_Configuration_Element_Type_Enum.Subject)
                            control_name = "subject";
                        if (thisElement.Element_Type == ContactForm_Configuration_Element_Type_Enum.Email)
                            control_name = "email";
                        if (String.IsNullOrEmpty(control_name))
                            control_name = "Control" + control_count;

                        // If this maps from a user value, get that now
                        string initialValue = String.Empty;
                        if ((thisElement.UserAttribute != User_Object_Attribute_Mapping_Enum.NONE) && (RequestSpecificValues.Current_User != null))
                        {
                            initialValue = RequestSpecificValues.Current_User.Get_Value_By_Mapping(thisElement.UserAttribute);
                        }

                        // But, if this was already posted back, get that value
                        if (postBackValues.ContainsKey(control_name))
                        {
                            initialValue = postBackValues[control_name];
                        }

                        // Get the css
                        string cssClass = "sbkChsw_Element";
                        if (!String.IsNullOrEmpty(thisElement.CssClass))
                            cssClass = thisElement.CssClass;



                        // If this is the firest element of this type, then get th ename
                        if (firstControl.Length == 0)
                            firstControl = control_name;

                        // Start this element
                        Output.WriteLine("      <div class=\"" + cssClass + "\">");
                        switch (thisElement.Element_Type)
                        {
                            case ContactForm_Configuration_Element_Type_Enum.TextBox:
                                Output.WriteLine("        <label for=\"" + control_name + "\">" + thisElement.QueryText.Get_Value(RequestSpecificValues.Current_Mode.Language) + "</label> ");
                                Output.WriteLine("        <input name=\"" + control_name + "\" id=\"" + control_name + "\" type=\"text\" value=\"" + initialValue + "\" class=\"sbk_Focusable\" />");
                                break;

                            case ContactForm_Configuration_Element_Type_Enum.Subject:
                                Output.WriteLine("        <label for=\"subject\">" + thisElement.QueryText.Get_Value(RequestSpecificValues.Current_Mode.Language) + "</label> ");
                                Output.WriteLine("        <input name=\"subject\" id=\"subject\" type=\"text\" value=\"" + initialValue + "\" class=\"sbk_Focusable\" />");
                                break;

                            case ContactForm_Configuration_Element_Type_Enum.Email:
                                Output.WriteLine("        <label for=\"email\">" + thisElement.QueryText.Get_Value(RequestSpecificValues.Current_Mode.Language) + "</label> ");
                                Output.WriteLine("        <input name=\"email\" id=\"email\" type=\"text\" value=\"" + initialValue + "\" class=\"sbk_Focusable\" />");
                                break;

                            case ContactForm_Configuration_Element_Type_Enum.TextArea:
                                Output.WriteLine("        <label for=\"" + control_name + "\">" + thisElement.QueryText.Get_Value(RequestSpecificValues.Current_Mode.Language) + "</label>  <br />");
                                Output.WriteLine("        <textarea name=\"" + control_name + "\" id=\"" + control_name + "\" class=\"sbk_Focusable\" >" + initialValue + "</textarea>");
                                break;

                            case ContactForm_Configuration_Element_Type_Enum.CheckBoxSet:
                                Output.WriteLine("        " + thisElement.QueryText.Get_Value(RequestSpecificValues.Current_Mode.Language) + " ( " + initialValue + ")<br />");
                                initialValue = initialValue + ",";
                                foreach (string thisOption in thisElement.Options)
                                {
                                    if (initialValue.IndexOf(thisOption + ",") >= 0)
                                    {
                                        Output.WriteLine("          <input type=\"checkbox\" name=\"" + control_name + "\" value=\"" + thisOption + "\" checked=\"checked\" />" + thisOption + "<br />");
                                    }
                                    else
                                    {
                                        Output.WriteLine("          <input type=\"checkbox\" name=\"" + control_name + "\" value=\"" + thisOption + "\" />" + thisOption + "<br />");
                                    }
                                }
                                break;

                            case ContactForm_Configuration_Element_Type_Enum.RadioSet:
                                Output.WriteLine("        " + thisElement.QueryText.Get_Value(RequestSpecificValues.Current_Mode.Language) + "<br />");
                                foreach (string thisOption in thisElement.Options)
                                {
                                    if (thisOption == initialValue)
                                    {
                                        Output.WriteLine("          <input type=\"radio\" name=\"" + control_name + "\" value=\"" + thisOption + "\" checked=\"checked\" />" + thisOption + "<br />");

                                    }
                                    else
                                    {
                                        Output.WriteLine("          <input type=\"radio\" name=\"" + control_name + "\" value=\"" + thisOption + "\" />" + thisOption + "<br />");
                                    }
                                }
                                break;


                            case ContactForm_Configuration_Element_Type_Enum.SelectBox:
                                Output.WriteLine("        " + thisElement.QueryText.Get_Value(RequestSpecificValues.Current_Mode.Language));
                                // Output.WriteLine("        <fieldset>");
                                Output.WriteLine("      <select name=\"" + control_name + "\" id=\"" + control_name + "\" >");
                                Output.WriteLine("          <option></option>");

                                foreach (string thisOption in thisElement.Options)
                                {
                                    if (thisOption == initialValue)
                                    {
                                        Output.WriteLine("          <option selected=\"selected\">" + thisOption + "</option>");
                                    }
                                    else
                                    {
                                        Output.WriteLine("          <option>" + thisOption + "</option>");
                                    }
                                }
                                Output.WriteLine("      </select>");
                                // Output.WriteLine("        </fieldset>");
                                break;

                        }
                        Output.WriteLine("      </div>");

                        control_count++;
                    }


                }

                // If this is in an element block, close that now
                if (inElementBlock)
                {
                    Output.WriteLine("    </div>");
                }

                Output.WriteLine("    <div id=\"sbkChsw_ButtonDiv\">");

                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                string return_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Contact;

                if ((lastMode.Length > 0) && (lastMode.IndexOf("contact", StringComparison.InvariantCultureIgnoreCase) < 0))
                {
                    if (lastMode.IndexOf("http", StringComparison.InvariantCultureIgnoreCase) == 0)
                        return_url = lastMode;
                    else
                        return_url = "?" + lastMode;
                }

                Output.WriteLine("      <button title=\"" + cancel + "\" class=\"sbkChsw_Button\" onclick=\"window.location.href='" + return_url + "'; return false;\" type=\"button\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> " + cancel + "</button> &nbsp; &nbsp; ");
                Output.WriteLine("      <button title=\"" + submit + "\" class=\"sbkChsw_Button\" onclick=\"return send_contact_email();\" type=\"submit\">" + submit + " <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");


                Output.WriteLine("    </div>");

                Output.WriteLine("  </div>");
                Output.WriteLine("</div>");
                Output.WriteLine("");
                if (firstControl.Length > 0)
                {
                    Output.WriteLine("<!-- Focus on the first control -->");
                    Output.WriteLine("<script type=\"text/javascript\">focus_element('" + firstControl + "');</script>");
                    Output.WriteLine("");
                }
                Output.WriteLine("<br />");
                Output.WriteLine("");
                Output.WriteLine("</form>");
            }

            Output.WriteLine("<!-- Close the pagecontainer div -->");
            Output.WriteLine("</div>");
            Output.WriteLine();

            return true;
        }

        ///// <summary> Gets the collection of special behaviors which this subwriter
        ///// requests from the main HTML writer. </summary>
        //public override List<HtmlSubwriter_Behaviors_Enum> Subwriter_Behaviors
        //{
        //    get { return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Suppress_Banner }; }
        //}
    }
}
