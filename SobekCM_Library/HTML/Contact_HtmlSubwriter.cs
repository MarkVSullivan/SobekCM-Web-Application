#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Web;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Contact html subwriter renders the contact us screen and the subsequent 'Your message was sent' screen </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
    public class Contact_HtmlSubwriter : abstractHtmlSubwriter
    {
        private readonly string lastMode;
        private readonly string userHistoryRequestInfo;

        /// <summary> Constructor for a new instance of the Contact_HtmlSubwriter class </summary>
        /// <param name="Last_Mode"> URL for the last mode this user was in before selecting contact us</param>
        /// <param name="UserHistoryRequestInfo"> Some history and user information to include in the final email </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Hierarchy_Object"> Current item aggregation object to display </param>
        public Contact_HtmlSubwriter(string Last_Mode, string UserHistoryRequestInfo, SobekCM_Navigation_Object Current_Mode, Item_Aggregation Hierarchy_Object)
        {
            // Save the parameters
            lastMode = Last_Mode;
            userHistoryRequestInfo = UserHistoryRequestInfo;
            currentMode = Current_Mode;
            this.Hierarchy_Object = Hierarchy_Object;

            // If this is a post back, send email
            if (HttpContext.Current.Request.Form["item_action"] == null) return;

            string action = HttpContext.Current.Request.Form["item_action"];
            if (action == "email")
            {
                string notes = HttpContext.Current.Request.Form["notesTextBox"].Trim();
                string subject = HttpContext.Current.Request.Form["subjectTextBox"].Trim();
                string message_from = SobekCM_Library_Settings.System_Email;
                string email = HttpContext.Current.Request.Form["emailTextBox"].Trim();
                string name = HttpContext.Current.Request.Form["nameTextBox"].Trim();

                if ((notes.Length > 0) || (subject.Length > 0))
                {
                    // Create the mail message
                    if (email.Length > 0)
                    {
                        message_from = email;
                    }

                    // Start the body
                    StringBuilder builder = new StringBuilder(1000);
                    builder.Append(notes + "\n\n\n\n");
                    builder.Append("The following information is collected to allow us better serve your needs.\n\n");
                    builder.Append("PERSONAL INFORMATION\n");
                    builder.Append("\tName:\t\t\t\t" + name + "\n");
                    builder.Append("\tEmail:\t\t\t" + email + "\n");
                    builder.Append(userHistoryRequestInfo);
                    string email_body = builder.ToString();

                    try
                    {
                        MailMessage myMail = new MailMessage(message_from, base.Hierarchy_Object.Contact_Email.Replace(";", ","))
                                                 {
                                                     Subject =subject + "  [" + currentMode.SobekCM_Instance_Abbreviation +" Submission]",
                                                     Body = email_body
                                                 };
                        // Mail this
                        SmtpClient client = new SmtpClient("smtp.ufl.edu");
                        client.Send(myMail);

                        // Log this
                        string sender = message_from;
                        if (name.Length > 0)
                            sender = name + " ( " + message_from + " )";
                        SobekCM_Database.Log_Sent_Email(sender, base.Hierarchy_Object.Contact_Email.Replace(";", ","), subject + "  [" + currentMode.SobekCM_Instance_Abbreviation + " Submission]", email_body, false, true, -1);

                        // Send back to the home for this collection, sub, or group
                        Current_Mode.Mode = Display_Mode_Enum.Contact_Sent;
                        Current_Mode.Redirect();
                        return;
                    }
                    catch
                    {
                        bool email_error = SobekCM_Database.Send_Database_Email(base.Hierarchy_Object.Contact_Email.Replace(";", ","), subject + "  [" + currentMode.SobekCM_Instance_Abbreviation + " Submission]", email_body, false, true, -1, -1 );

                        // Send back to the home for this collection, sub, or group
                        if (email_error)
                        {
                            HttpContext.Current.Response.Redirect(SobekCM_Library_Settings.System_Error_URL, false);
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            return;
                        }
                        else
                        {
                            // Send back to the home for this collection, sub, or group
                            Current_Mode.Mode = Display_Mode_Enum.Contact_Sent;
                            Current_Mode.Redirect();
                            return;
                        }
                    }
                }
            }
        }


        /// <summary> Write any additional values within the HTML Head of the
        /// final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> By default this does nothing, but can be overwritten by all the individual html subwriters </remarks>
        public virtual void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            // Based on display mode, add ROBOT instructions
            switch (currentMode.Mode)
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
                return currentMode.Mode == Display_Mode_Enum.Contact_Sent ? "{0} Contact Sent" : "{0} Contact Us";
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

            if (currentMode.Mode == Display_Mode_Enum.Contact_Sent)
            {
                Output.WriteLine("<div class=\"SobekHomeText\">");
                Output.WriteLine("<table width=\"700\" border=\"0\" align=\"center\">");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td align=\"center\" >");
                Output.WriteLine("      <b>Your email has been sent.</b>");
                Output.WriteLine("      <br /><br />");
                Output.WriteLine("      <a href=\"" + currentMode.Base_URL + "\">Click here to return to the digital collection home</a>");
                Output.WriteLine("      <br /><br />");
                if (currentMode.Browser_Type.IndexOf("IE") >= 0)
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
                string subject_text = "Enter a subject here:";
                string notes_text = "Describe your question or problem here:";
                string may_we = "May we contact you?  If so, please provide the following information:";
                string name = "Enter your name here:";
                string email = "Enter your e-mail address here:";
                string submit = "Submit";
                string cancel = "Cancel";

                if (currentMode.Language == Web_Language_Enum.French)
                {
                    contact_us_title = "Contactez Nous";
                    please_complete = "Veuillez remplir les champs obligatoires indiqués:";
                    subject_text = "Entrez votre sujet ici:";
                    notes_text = "Décrivez votre question ici:";
                    may_we = "Mai-nous communiquer avec vous? Dans l'affirmative, s’il vous plaît fournir les informations suivantes:";
                    name = "Votre nom ici:";
                    email = "Votre addresse de courriel electronique ici:";
                    submit = "Soumettre";
                    cancel = "Annuler";
                }

                if (currentMode.Language == Web_Language_Enum.Spanish)
                {
                    contact_us_title = "Contáctenos";
                    please_complete = "Por Favor llene la información Requerida:";
                    subject_text = "Ponga un Sujeto Aquí:";
                    notes_text = "Describa su problema or pregunta Aquí:";
                    may_we = "¿Podemos comunicarnos con usted? En caso afirmativo, sírvase proporcionar la siguiente información:";
                    name = "Su Nombre Aquí:";
                    email = "Su Dirección de Correo Electrónico (Email) Aquí:";
                    submit = "Mandar";
                    cancel = "Cancelar";
                }

                // Determine any initial values
                string name_value = string.Empty;
                string email_value = string.Empty;
                string notes_value = String.Empty;
                string subject_value = String.Empty;
                if ((!currentMode.isPostBack) && ( HttpContext.Current.Session["user"] != null ))
                {
                    User_Object user = (User_Object) HttpContext.Current.Session["user"];
                    name_value = user.Full_Name;
                    email_value = user.Email;
                }
                if (currentMode.Error_Message.Length > 0)
                {
                    subject_value = currentMode.SobekCM_Instance_Abbreviation + " Error";
                    notes_value = currentMode.Error_Message;
                }

                // Start this form
                string post_url = HttpUtility.HtmlEncode(HttpContext.Current.Items["Original_URL"].ToString());
                Output.WriteLine("<form name=\"email_form\" method=\"post\" action=\"" + post_url + "\" id=\"addedForm\" >");

                // Add the hidden field
                Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
                Output.WriteLine("<input type=\"hidden\" id=\"item_action\" name=\"item_action\" value=\"\" />");


                Output.WriteLine("<div class=\"SobekSearchPanel\">");
                Output.WriteLine("<h1>" + contact_us_title + "</h1>");

                Output.WriteLine("<blockquote>");

                if ((currentMode.Aggregation == "juv") || (currentMode.Aggregation == "alice"))
                {
                    Output.WriteLine("<a href=\"" + currentMode.Base_URL + "bookvalue\" target=\"_bookvalue\">Click here if you are asking about your own copy of a book.</a><br /><br />");
                }

                Output.WriteLine("<strong>" + please_complete + "</strong>");
                Output.WriteLine("<br /><br />");
                Output.WriteLine("<blockquote>");
                Output.WriteLine("<label for=\"subjectTextBox\">" + subject_text + "</label> ");
                Output.WriteLine("<input name=\"subjectTextBox\" value=\"" + subject_value + "\" type=\"text\" id=\"subjectTextBox\" class=\"SobekContactBox\" onfocus=\"textbox_enter('subjectTextBox', 'SobekContactBox_focused')\" onblur=\"textbox_leave('subjectTextBox', 'SobekContactBox')\" />");
                Output.WriteLine("<br /> <br />");
                Output.WriteLine("<label for=\"notesTextBox\">" + notes_text + "</label> <br />");
                Output.WriteLine("<textarea name=\"notesTextBox\" rows=\"8\" cols=\"100\" id=\"notesTextBox\" class=\"SobekContactCommentBox\" onfocus=\"textbox_enter('notesTextBox', 'SobekContactCommentBox_focused')\" onblur=\"textbox_leave('notesTextBox', 'SobekContactCommentBox')\">" + notes_value + "</textarea>");
                Output.WriteLine("<br /><br />");
                Output.WriteLine("</blockquote>");
                Output.WriteLine("<strong>" + may_we + "</strong>");
                Output.WriteLine("<blockquote>");
                Output.WriteLine("<label for=\"nameTextBox\">" + name + "</label>");
                Output.WriteLine("<input name=\"nameTextBox\" type=\"text\" value=\"" + name_value +"\" id=\"nameTextBox\" class=\"SobekContactBox\" onfocus=\"textbox_enter('nameTextBox', 'SobekContactBox_focused')\" onblur=\"textbox_leave('nameTextBox', 'SobekContactBox')\" />");
                Output.WriteLine("<br /><br />");
                Output.WriteLine("<label for=\"emailTextBox\">" + email + "</label><input name=\"emailTextBox\" type=\"text\" value=\"" + email_value + "\" id=\"emailTextBox\" class=\"SobekContactBox\" onfocus=\"textbox_enter('emailTextBox', 'SobekContactBox_focused')\" onblur=\"textbox_leave('emailTextBox', 'SobekContactBox')\" />");
                Output.WriteLine("<br /><br /><br />");
                Output.WriteLine("<center>");

                if (lastMode.Length > 0)
                    Output.Write("<a href=\"?" + lastMode + "\">");
                else
                {
                    currentMode.Mode = Display_Mode_Enum.Aggregation;
					currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
                    Output.Write("<a href=\"" + currentMode.Redirect_URL() + "\">");
                    currentMode.Mode = Display_Mode_Enum.Contact;
                }

                Output.WriteLine("<img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"" + cancel + "\" /></a> &nbsp; &nbsp; ");
                Output.WriteLine("<a href=\"?\" onclick=\"return send_contact_email();\" ><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/send_button_g.gif\" alt=\"" + submit + "\" /></a>");
                Output.WriteLine("</center>");
                Output.WriteLine("</blockquote>");
                Output.WriteLine("</blockquote>");
                Output.WriteLine("</div>");
                Output.WriteLine("");
                Output.WriteLine("<!-- Focus on subject text box -->");
                Output.WriteLine("<script type=\"text/javascript\">focus_element('subjectTextBox');</script>");
                Output.WriteLine("");
                Output.WriteLine("<br />");
                Output.WriteLine("");
                Output.WriteLine("</form>");
            }

            Output.WriteLine("<!-- Close the pagecontainer div -->");
            Output.WriteLine("</div>");
            Output.WriteLine();

            return true;
        }





        public override List<HtmlSubwriter_Behaviors_Enum> Subwriter_Behaviors
        {
            get { return new List<HtmlSubwriter_Behaviors_Enum>() { HtmlSubwriter_Behaviors_Enum.Suppress_Banner }; }
        }
    }
}
