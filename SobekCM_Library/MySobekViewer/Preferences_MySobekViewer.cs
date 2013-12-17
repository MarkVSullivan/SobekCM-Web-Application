#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows an unauthenticated user to register and an authenticated user to change their preferences </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer for either registering or changing their preferences </li>
    /// </ul></remarks>
    public class Preferences_MySobekViewer : abstract_MySobekViewer
    {
        private TextBox confirmPasswordBox;
        private TextBox passwordBox;
        private readonly List<string> validationErrors;

        /// <summary> Constructor for a new instance of the Preferences_MySobekViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Preferences_MySobekViewer(User_Object User, Custom_Tracer Tracer) : base(User)
        {
            Tracer.Add_Trace("Preferences_MySobekViewer.Constructor", String.Empty);

            // Do nothing
            validationErrors = new List<string>();
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This value changes; if the user is logged on it returns 'Edit Your Preferences', otherwise it has a 'Register for...' message </value>
        public override string Web_Title
        {
            get
            {
                if (HttpContext.Current.Session["user"] == null)
                {
                    return "Register for My" + currentMode.SobekCM_Instance_Abbreviation;
                }
                return "Edit Your Account Preferences";
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since this form is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Preferences_MySobekViewer.Write_HTML", "Do nothing");
        }

		/// <summary> Add controls directly to the form in the main control area placeholder </summary>
        /// <param name="MainPlaceHolder"> Main place holder to which all main controls are added </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> The bulk of this page is added here, as controls </remarks>
        public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Preferences_MySobekViewer.Add_Controls", "Do nothing");

            string mySobekText = "my" + currentMode.SobekCM_Instance_Abbreviation;

            // Get the labels to use
            string accountInfoLabel = "Account Information";
            string userNameLabel = "UserName";
            string personalInfoLabel = "Personal Information";
            string familyNamesLabel = "Family Name(s)";
            string givenNamesLabel = "Given Names(s)";
            string nicknameLabel = "Nickname";
            string emailLabel = "Email";
            const string emailStatsLabel = "Send me monthly usage statistics for my items";
            string affilitionInfoLabel = "Current Affiliation Information";
            string organizationLabel = "Organization/University";
            string collegeLabel = "College";
            string departmentLabel = "Department";
            string unitLabel = "Unit";
            string selfSubmittalPrefLabel = "Self-Submittal Preferences";
            string sendEmailLabel = "Send me an email when I submit new items";
            string templateLabel = "Template";
            string projectLabel = "Default Metadata";
            string defaultRightsLabel = "Default Rights";
            string rightsExplanationLabel = "(These are the default rights you give for sharing, repurposing, or remixing your item to other users. You can set this with each new item you submit, but this will be the default that appears.)";
            string rightsInstructionLabel = "You may also select a <a title=\"Explanation of different creative commons licenses.\" href=\"http://creativecommons.org/about/licenses/\">Creative Commons License</a> option below.";
            string otherPreferencesLabel = "Other Preferences";
            string languageLabel = "Language";
            string passwordLabel = "Password";
            string confirmPasswordLabel = "Confirm Password";
            string col1Width = "15px";
            string col2Width = "100px";
            string col3Width = "605px";

            if ( currentMode.Language == Web_Language_Enum.French)
            {
                accountInfoLabel = "Informations sur le Compte";
                userNameLabel = "Nom du Compte";
                personalInfoLabel = "Des Renseignements Personnels";
                familyNamesLabel = "Nom de Famille";
                givenNamesLabel = "Prénoms";
                nicknameLabel = "Pseudo";
                emailLabel = "Email";
                affilitionInfoLabel = "Information Affiliation Actuel";
                organizationLabel = "Organisation / Université";
                collegeLabel = "Collège";
                departmentLabel = "Département";
                unitLabel = "Unité";
                selfSubmittalPrefLabel = "Préférences Auto-Soumission";
                sendEmailLabel = "Envoyez-moi un email lorsque je présente les nouveaux éléments";
                templateLabel = "Modèle";
				projectLabel = "Métadonnées par Défaut";
                defaultRightsLabel = "Droits par Défaut";
                rightsExplanationLabel = "(Ce sont les droits par défaut que vous donnez de partager, d'adapter, ou remixer votre article à d'autres utilisateurs. Vous pouvez fixer cette valeur à chaque nouvel élément que vous soumettez, mais ce sera la valeur par défaut qui s'affiche.)";
                rightsInstructionLabel = "Vous pouvez également sélectionner une option <a title=\"Explication des différentes licences Creative Commons.\" href=\"http://creativecommons.org/about/licenses/\">Creative Commons License</a> ci-dessous.";
                otherPreferencesLabel = "Autres Préférences";
                languageLabel = "Langue";
                passwordLabel = "Mot de Passe";
                confirmPasswordLabel = "Confirmer Mot de Passe";
                col1Width = "10px";
                col2Width = "220px";
                col3Width = "490px";
            }

            if (currentMode.Language == Web_Language_Enum.Spanish)
            {
                accountInfoLabel = "Información de la Cuenta";
                userNameLabel = "Nombre de la Cuenta";
                personalInfoLabel = "Información Personal";
                familyNamesLabel = "Familia Nombre";
                givenNamesLabel = "Nombre de Pila";
                nicknameLabel = "Nickname";
                emailLabel = "Correo Electrónico";
                affilitionInfoLabel = "Información de la Afiliación Actual";
                organizationLabel = "Organización/Universidad";
                collegeLabel = "Colegio";
                departmentLabel = "Departamento";
                unitLabel = "Unidad";
                selfSubmittalPrefLabel = "Preferencias de Presentación Auto-";
                sendEmailLabel = "Enviadme un correo electrónico cuando se presento nuevos temas";
                templateLabel = "Plantilla";
				projectLabel = "Metadatos Predeterminado";
                defaultRightsLabel = "Derechos por Defecto";
                rightsExplanationLabel = "(Estos son los derechos por defecto le dan para compartir, reutilización, o remezclando el tema a otros usuarios. Puede establecer esto con cada artículo nuevo que presentar, pero esto será el valor por defecto que aparece.)";
                rightsInstructionLabel = "También puede seleccionar una opción de  <a title=\"Explicación de las diferentes licencias Creative Commons\" href=\"http://creativecommons.org/about/licenses/\">Creative Commons License</a> a continuación.";
                otherPreferencesLabel = "Otras preferencias";
                languageLabel = "Idioma";
                passwordLabel = "Contraseña";
                confirmPasswordLabel = "Confirmar Contraseña";
                col1Width = "10px";
                col2Width = "220px";
                col3Width = "490px";
            }

            // Is this for registration?       
            bool registration = (HttpContext.Current.Session["user"] == null);
            if (registration)
            {
                user = new User_Object();
            }

            bool desire_to_upload = false;
            bool send_email_on_submission = false;
            bool send_usages_emails = true;
            string family_name = String.Empty;
            string given_name = String.Empty;
            string nickname = String.Empty;
            string email = String.Empty;
            string organization = String.Empty;
            string college = String.Empty;
            string department = String.Empty;
            string unit = String.Empty;
            string template = String.Empty;
            string project = String.Empty;
            string username = String.Empty;
            string password = String.Empty;
            string password2 = String.Empty;
            string ufid = String.Empty;
            string language = String.Empty;
            string default_rights = String.Empty;
            if (currentMode.isPostBack)
            {
                // Loop through and get the dataa
                string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
                foreach (string thisKey in getKeys)
                {
                    switch (thisKey)
                    {
                        case "prefUserName":
                            username = HttpContext.Current.Request.Form[thisKey];
                            break;

                        case "password_enter":
                            password = HttpContext.Current.Request.Form[thisKey];
                            break;

                        case "password_confirm":
                            password2 = HttpContext.Current.Request.Form[thisKey];
                            break;

                        case "prefUfid":
                            ufid = HttpContext.Current.Request.Form[thisKey].Trim().Replace("-", "");
                            break;

                        case "prefFamilyName":
                            family_name = HttpContext.Current.Request.Form[thisKey];
                            break;

                        case "prefGivenName":
                            given_name = HttpContext.Current.Request.Form[thisKey];
                            break;

                        case "prefNickName":
                            nickname = HttpContext.Current.Request.Form[thisKey];
                            break;

                        case "prefEmail":
                            email = HttpContext.Current.Request.Form[thisKey];
                            break;

                        case "prefOrganization":
                            organization = HttpContext.Current.Request.Form[thisKey];
                            break;

                        case "prefCollege":
                            college = HttpContext.Current.Request.Form[thisKey];
                            break;

                        case "prefDepartment":
                            department = HttpContext.Current.Request.Form[thisKey];
                            break;

                        case "prefUnit":
                            unit = HttpContext.Current.Request.Form[thisKey];
                            break;

                        case "prefLanguage":
                            string language_temp = HttpContext.Current.Request.Form[thisKey];
                            if ( language_temp == "es" )
                                language = "Español";
                            if (language_temp == "fr")
                                language = "Français";
                            break;

                        case "prefTemplate":
                            template = HttpContext.Current.Request.Form[thisKey];
                            break;

                        case "prefProject":
                            project = HttpContext.Current.Request.Form[thisKey];
                            break;

                        case "prefAllowSubmit":
                            string submit_value = HttpContext.Current.Request.Form[thisKey];
                            if (submit_value == "allowsubmit")
                                desire_to_upload = true;
                            break;

                        case "prefSendEmail":
                            string submit_value2 = HttpContext.Current.Request.Form[thisKey];
                            send_email_on_submission = submit_value2 == "sendemail";
                            break;

                        case "prefEmailStats":
                            string submit_value3 = HttpContext.Current.Request.Form[thisKey];
                            send_usages_emails = submit_value3 == "sendemail";
                            break;

                        case "prefRights":
                            default_rights = HttpContext.Current.Request.Form[thisKey];
                            break;

                    }
                }

                if (registration)
                {
                    if (username.Trim().Length == 0)
                        validationErrors.Add("Username is a required field");
                    else if (username.Trim().Length < 8)
                        validationErrors.Add("Username must be at least eight digits");
                    if ((password.Trim().Length == 0) || (password2.Trim().Length == 0))
                        validationErrors.Add("Select and confirm a password");
                    if (password.Trim() != password2.Trim())
                        validationErrors.Add("Passwords do not match");
                    else if ( password.Length < 8 )
                        validationErrors.Add("Password must be at least eight digits");
                    if (ufid.Trim().Length > 0)
                    {
                        if (ufid.Trim().Length != 8)
                        {
                            validationErrors.Add("UFIDs are always eight digits");
                        }
                        else
                        {
                            int ufid_convert_test;
                            if ( !Int32.TryParse( ufid, out ufid_convert_test ))
                                validationErrors.Add("UFIDs are always numeric");
                        }
                    }
                }

                // Validate the basic data is okay
                if (family_name.Trim().Length == 0)
                    validationErrors.Add("Family name is a required field");
                if (given_name.Trim().Length == 0)
                    validationErrors.Add("Given name is a required field");
                if ((email.Trim().Length == 0) || (email.IndexOf("@") < 0))
                    validationErrors.Add("A valid email is required");
                if ( default_rights.Trim().Length > 1000 )
                {
                    validationErrors.Add("Rights statement truncated to 1000 characters.");
                    default_rights = default_rights.Substring(0, 1000);
                }

                if ((registration) && (validationErrors.Count == 0))
                {
                    bool email_exists;
                    bool username_exists;
                    SobekCM_Database.UserName_Exists(username, email, out username_exists, out email_exists, Tracer);
                    if (email_exists)
                    {
                        validationErrors.Add("An account for that email address already exists.");
                    }
                    else if (username_exists)
                    {
                        validationErrors.Add("That username is taken.  Please choose another.");
                    }
                }

                if (validationErrors.Count == 0)
                {
                    user.College = college.Trim();
                    user.Department = department.Trim();
                    user.Email = email.Trim();
                    user.Family_Name = family_name.Trim();
                    user.Given_Name = given_name.Trim();
                    user.Nickname = nickname.Trim();
                    user.Organization = organization.Trim();
                    user.Unit = unit.Trim();
                    user.Set_Default_Template(template.Trim());
                    // See if the project is different, if this is not registration
                    if ((!registration) && (user.Projects[0] != project.Trim()))
                    {
                        // Determine the in process directory for this
                        string user_in_process_directory = SobekCM_Library_Settings.In_Process_Submission_Location + "\\" + user.UserName;
                        if (user.ShibbID.Trim().Length > 0)
                            user_in_process_directory = SobekCM_Library_Settings.In_Process_Submission_Location + "\\" + user.ShibbID;
                        if (Directory.Exists(user_in_process_directory))
                        {
                            if (File.Exists(user_in_process_directory + "\\TEMP000001_00001.mets"))
                                File.Delete(user_in_process_directory + "\\TEMP000001_00001.mets");
                        }
                    }
                    user.Set_Default_Project(project.Trim());
                    user.Preferred_Language = language;
                    user.Default_Rights = default_rights;
                    user.Send_Email_On_Submission = send_email_on_submission;
                    user.Receive_Stats_Emails = send_usages_emails;

                    if (registration)
                    {
                        user.Can_Submit = false;
                        user.Send_Email_On_Submission = true;
                        user.ShibbID = ufid;
                        user.UserName = username;
                        user.UserID = -1;

                        // Save this new user
                        SobekCM_Database.Save_User(user, password, Tracer);

                        // Retrieve the user from the database
                        user = SobekCM_Database.Get_User(username, password, Tracer);

                        // Special code in case this is the very first user
                        if (user.UserID == 1)
                        {
                            // Add each template and project
                            DataSet projectTemplateSet = SobekCM_Database.Get_All_Projects_Templates(Tracer);
                            List<string> templates = (from DataRow thisTemplate in projectTemplateSet.Tables[1].Rows select thisTemplate["TemplateCode"].ToString()).ToList();
                            List<string> projects = (from DataRow thisProject in projectTemplateSet.Tables[0].Rows select thisProject["ProjectCode"].ToString()).ToList();

                            // Save the updates to this admin user
                            SobekCM_Database.Save_User(user, password, Tracer);
                            SobekCM_Database.Update_SobekCM_User(user.UserID, true, true, true, true, true, true, true, "edit_internal", "editmarc_internal", true, true, true, Tracer);
                            SobekCM_Database.Update_SobekCM_User_Projects(user.UserID,  new ReadOnlyCollection<string>(projects), Tracer);
                            SobekCM_Database.Update_SobekCM_User_Templates(user.UserID, new ReadOnlyCollection<string>(templates), Tracer);

                            // Retrieve the user information again
                            user = SobekCM_Database.Get_User(username, password, Tracer);
                        }

                        user.Is_Just_Registered = true;
                        HttpContext.Current.Session["user"] = user;

                        // If they want to be able to contribue, send an email
                        if (desire_to_upload)
                        {
                            SobekCM_Database.Send_Database_Email(SobekCM_Library_Settings.System_Email, "Submittal rights requested by " + user.Full_Name, "New user requested ability to submit new items.<br /><br /><blockquote>Name: " + user.Full_Name + "<br />Email: " + user.Email + "<br />Organization: " + user.Organization + "<br />User ID: " + user.UserID + "</blockquote>", true, false, -1, -1);
                        }

                        // Email the user their registation information
                        if (desire_to_upload)
                        {
                            SobekCM_Database.Send_Database_Email(email, "Welcome to " + mySobekText, "<strong>Thank you for registering for " + mySobekText + "</strong><br /><br />You can access this directly through the following link: <a href=\"" + currentMode.Base_URL + "/my\">" + currentMode.Base_URL + "/my</a><br /><br />Full Name: " + user.Full_Name + "<br />User Name: " + user.UserName + "<br /><br />You will receive an email when your request to submit items has been processed.", true, false, -1, -1);
                        }
                        else
                        {
                            SobekCM_Database.Send_Database_Email(email, "Welcome to " + mySobekText, "<strong>Thank you for registering for " + mySobekText + "</strong><br /><br />You can access this directly through the following link: <a href=\"" + currentMode.Base_URL + "/my\">" + currentMode.Base_URL + "/my</a><br /><br />Full Name: " + user.Full_Name + "<br />User Name: " + user.UserName, true, false, -1, -1);
                        }

                        // Now, forward back to the My Sobek home page
                        currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;

                        // If this is the first user to register (who would have been set to admin), send to the 
                        // system-wide settings screen
                        if (user.UserID == 1)
                        {
                            currentMode.Mode = Display_Mode_Enum.Administrative;
                            currentMode.Admin_Type = Admin_Type_Enum.Settings;
                        }
                        currentMode.Redirect();
                        return;
                    }
                    else
                    {
                        HttpContext.Current.Session["user"] = user;
                        SobekCM_Database.Save_User(user, String.Empty, Tracer);

                        // Now, forward back to the My Sobek home page
                        currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                        currentMode.Redirect();
                        return;
                    }
                }
            }
            else
            {
                family_name = user.Family_Name;
                given_name = user.Given_Name;
                nickname = user.Nickname;
                email = user.Email;
                organization = user.Organization;
                college = user.College;
                department = user.Department;
                unit = user.Unit;
                username = user.UserName;
                ufid = user.ShibbID;
                language=user.Preferred_Language;
                send_email_on_submission = user.Send_Email_On_Submission;
                default_rights = user.Default_Rights;

            }



            StringBuilder builder = new StringBuilder(1000);
			builder.AppendLine("<h1>" + Web_Title + "</h1>");
			builder.AppendLine();
            builder.AppendLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_metadata.js\" type=\"text/javascript\"></script>");
            builder.AppendLine("<div class=\"SobekHomeText\" >");
            builder.AppendLine("<blockquote>");
            if (registration)
            {
                builder.AppendLine("Registration for " + mySobekText + " is free and open to the public.  Enter your information below to be instantly registered.<br /><br />");
                builder.AppendLine("Account information, name, and email are required for each new account.<br /><br />");
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
                builder.AppendLine("Already registered?  <a href=\"" + currentMode.Redirect_URL() + "\">Log on</a>.<br /><br />");
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
            }
            if (validationErrors.Count > 0)
            {
                builder.AppendLine("<strong><span style=\"color: Red\">The following errors were detected:");
                builder.AppendLine("<blockquote>");
                foreach (string thisError in validationErrors)
                {
                    builder.AppendLine(thisError + "<br />");
                }
                builder.AppendLine("</blockquote>");
                builder.AppendLine("</span></strong>");
            }

            builder.AppendLine("<table width=\"700px\" cellpadding=\"5px\" class=\"SobekCitationSection1\" >");
            builder.AppendLine("  <tr><td colspan=\"3\" class=\"SobekCitationSectionTitle1\"><b>&nbsp;" + accountInfoLabel + "</b></td></tr>");
            if (registration)
            {
                // If there was a gatorlink ufid, use that
                if (HttpContext.Current.Session["Gatorlink_UFID"] != null)
                    ufid = HttpContext.Current.Session["Gatorlink_UFID"].ToString();

                builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td width=\"" + col2Width + "\"><b><label for=\"prefUsername\">" + userNameLabel + ":</label></b></td><td width=\"" + col3Width + "\"><input id=\"prefUserName\" name=\"prefUserName\" class=\"preferences_small_input\" value=\"" + username + "\" type=\"text\" onfocus=\"javascript:textbox_enter('prefUserName', 'preferences_small_input_focused')\" onblur=\"javascript:textbox_leave('prefUserName', 'preferences_small_input')\" />   &nbsp; &nbsp; (minimum of eight digits)</td></tr>");
                builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td><b><label for=\"password_enter\">" + passwordLabel + ":</label></b></td><td>");
                LiteralControl registerLiteral1 = new LiteralControl(builder.ToString());
                builder.Remove(0, builder.Length);
                MainPlaceHolder.Controls.Add(registerLiteral1);

                passwordBox = new TextBox
                                  {
                                      CssClass = "preferences_small_input sbk_Focusable",
                                      ID = "password_enter",
                                      TextMode = TextBoxMode.Password
                                  };
                MainPlaceHolder.Controls.Add(passwordBox);

                LiteralControl registerLiteral2 = new LiteralControl("   &nbsp; &nbsp; (minimum of eight digits, different than username)</td></tr>" + Environment.NewLine + "<tr><td width=\"" + col1Width + "\">&nbsp;</td><td><b><label for=\"password_confirm\">" + confirmPasswordLabel + ":</label></b></td><td>");
                MainPlaceHolder.Controls.Add(registerLiteral2);

                confirmPasswordBox = new TextBox
                                         {
                                             CssClass = "preferences_small_input sbk_Focusable",
                                             ID = "password_confirm",
                                             TextMode = TextBoxMode.Password
                                         };
                MainPlaceHolder.Controls.Add(confirmPasswordBox);

                builder.AppendLine("   &nbsp; &nbsp; (minimum of eight digits, different than username)</td></tr>");
            }
            else
            {
                builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td><b>" + userNameLabel + ":</b></td><td>" + user.UserName + "</td></tr>");
                builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td><b>UFID:</b></td><td>" + user.ShibbID + "</td></tr>");
            }

            builder.AppendLine("  <tr><td colspan=\"3\" class=\"SobekCitationSectionTitle1\"><b>&nbsp;" + personalInfoLabel + "</b></td></tr>");
            builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td><b><label for=\"prefFamilyName\">" + familyNamesLabel + ":</label></b></td><td><input id=\"prefFamilyName\" name=\"prefFamilyName\" class=\"preferences_medium_input sbk_Focusable\" value=\"" + family_name + "\" type=\"text\" /></td></tr>");
			builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td><b><label for=\"prefGivenName\">" + givenNamesLabel + ":</label></b></td><td><input id=\"prefGivenName\" name=\"prefGivenName\" class=\"preferences_medium_input sbk_Focusable\" value=\"" + given_name + "\" type=\"text\" /></td></tr>");
			builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td><b><label for=\"prefNickName\">" + nicknameLabel + ":</label></b></td><td><input id=\"prefNickName\" name=\"prefNickName\" class=\"preferences_medium_input sbk_Focusable\" value=\"" + nickname + "\" type=\"text\" /></td></tr>");
			builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td><b><label for=\"prefEmail\">" + emailLabel + ":</label></b></td><td><input id=\"prefEmail\" name=\"prefEmail\" class=\"preferences_medium_input sbk_Focusable\" value=\"" + email + "\" type=\"text\" /></td></tr>");

            if (user.Has_Item_Stats)
            {
                if ( !send_usages_emails )
                {
                    builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td>&nbsp;</td><td><input type=\"checkbox\" value=\"sendemail\" name=\"prefStatsEmail\" id=\"prefStatsEmail\" /><label for=\"prefStatsEmail\">" + emailStatsLabel + "</label></td></tr>");
                }
                else
                {
                    builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td colspan=\"2\"><input type=\"checkbox\" value=\"allowsubmit\" name=\"prefStatsEmail\" id=\"prefStatsEmail\" checked=\"checked\" /><label for=\"prefStatsEmail\">" + emailStatsLabel + "</label></td></tr>");
                }
            }
           
            builder.AppendLine("  <tr><td colspan=\"3\" class=\"SobekCitationSectionTitle1\"><b>&nbsp;" + affilitionInfoLabel + "</b></td></tr>");
            builder.AppendLine("  <tr><td>&nbsp;</td><td><b><label for=\"prefOrganization\">" + organizationLabel + ":</label></b></td><td><input id=\"prefOrganization\" name=\"prefOrganization\" class=\"preferences_large_input sbk_Focusable\" value=\"" + organization + "\" type=\"text\" /></td></tr>");
			builder.AppendLine("  <tr align=\"left\" ><td width=\"" + col1Width + "\">&nbsp;</td><td><b><label for=\"prefCollege\">" + collegeLabel + ":</label></b></td><td><input id=\"prefCollege\" name=\"prefCollege\" class=\"preferences_large_input sbk_Focusable\" value=\"" + college + "\"type=\"text\" /></td></tr>");
			builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td><b><label for=\"prefDepartment\">" + departmentLabel + ":</label></b></td><td><input id=\"prefDepartment\" name=\"prefDepartment\" class=\"preferences_large_input sbk_Focusable\" value=\"" + department + "\"type=\"text\" /></td></tr>");
			builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td><b><label for=\"prefUnit\">" + unitLabel + ":</label></b></td><td><input id=\"prefUnit\" name=\"prefUnit\" class=\"preferences_large_input sbk_Focusable\" value=\"" + unit + "\" type=\"text\" /></td></tr>");

            if (( registration ) && ( SobekCM_Library_Settings.Shibboleth_System_URL.Length > 0 ) && ( String.Compare(SobekCM_Library_Settings.Shibboleth_System_Name, "Gatorlink", true ) == 0 ))
            {
				builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td><b><label for=\"prefUfid\">UFID:</label></b></td><td><input id=\"prefUfid\" name=\"prefUfid\" class=\"preferences_small_input sbk_Focusable\" value=\"" + ufid + "\" type=\"text\" />    &nbsp; &nbsp; (optionally provides access through Gatorlink)</td></tr>");
            }


                if (user.Can_Submit)
                {
                    builder.AppendLine("  <tr><td colspan=\"3\" class=\"SobekCitationSectionTitle1\"><b>&nbsp;" + selfSubmittalPrefLabel + "</b></td></tr>");
                    if (!send_email_on_submission)
                    {
                        builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td>&nbsp;</td><td><input type=\"checkbox\" value=\"sendemail\" name=\"prefSendEmail\" id=\"prefSendEmail\" /><label for=\"prefSendEmail\">" + sendEmailLabel + "</label></td></tr>");
                    }
                    else
                    {
                        builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td colspan=\"2\"><input type=\"checkbox\" value=\"allowsubmit\" name=\"prefSendEmail\" id=\"prefSendEmail\" checked=\"checked\" /><label for=\"prefSendEmail\">" + sendEmailLabel + "</label></td></tr>");
                    }

                    if (user.Templates.Count > 0)
                    {
                        builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td><b>" + templateLabel + ":</b></td>");
                        builder.AppendLine("    <td>");
                        builder.AppendLine("      <select name=\"prefTemplate\" id=\"prefTemplate\" class=\"preferences_language_select\" >");
                        builder.AppendLine("        <option selected=\"selected\" value=\"" + user.Templates[0] + "\">" + user.Templates[0] + "</option>");
                        for (int i = 1; i < user.Templates.Count; i++)
                        {
                            builder.AppendLine("        <option value=\"" + user.Templates[i] + "\">" + user.Templates[i] + "</option>");
                        }
                        builder.AppendLine("      </select>");
                        builder.AppendLine("    </td>");
                        builder.AppendLine("  </tr>");
                    }
                    if (user.Projects.Count > 0)
                    {
                        builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td><b>" + projectLabel + ":</b></td>");
                        builder.AppendLine("    <td>");
                        builder.AppendLine("      <select name=\"prefProject\" id=\"prefProject\" class=\"preferences_language_select\" >");
                        builder.AppendLine("        <option selected=\"selected\" value=\"" + user.Projects[0] + "\">" + user.Projects[0] + "</option>");
                        for (int i = 1; i < user.Projects.Count; i++)
                        {
                            builder.AppendLine("        <option value=\"" + user.Projects[i] + "\">" + user.Projects[i] + "</option>");
                        }
                        builder.AppendLine("      </select>");
                        builder.AppendLine("    </td>");
                        builder.AppendLine("  </tr>");
                    }
                    builder.AppendLine("  <tr valign=\"top\"><td width=\"" + col1Width + "\">&nbsp;</td><td><b>" + defaultRightsLabel + ":</b></td><td>" + rightsExplanationLabel + "</td></tr>");
                    builder.AppendLine("  <tr><td colspan=\"2\">&nbsp;<td><textarea rows=\"5\" cols=\"88\" name=\"prefRights\" id=\"prefRights\" class=\"preference_rights_input sbk_Focusable\">" + default_rights + "</textarea></div></td></tr>");
                    builder.AppendLine("  <tr valign=\"top\">");
                    builder.AppendLine("    <td colspan=\"2\">&nbsp;</td>");
                    builder.AppendLine("    <td>");
                    builder.AppendLine("      " + rightsInstructionLabel + "<br />");
                    builder.AppendLine("      <table cellpadding=\"3px\" cellspacing=\"3px\" >");
                    builder.AppendLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('prefRights','[cc0] The author dedicated the work to the Commons by waiving all of his or her rights to the work worldwide under copyright law and all related or neighboring legal rights he or she had in the work, to the extent allowable by law.');\"><img title=\"You dedicate the work to the Commons by waiving all of your rights to the work worldwide under copyright law and all related or neighboring legal rights you had in the work, to the extent allowable by law.\" src=\"" + currentMode.Base_URL + "default/images/cc_zero.png\" /></a></td><td><b>No Copyright</b><br /><i>cc0</i></td></tr>");
                    builder.AppendLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('prefRights','[cc by] This item is licensed with the Creative Commons Attribution License.  This license lets others distribute, remix, tweak, and build upon this work, even commercially, as long as they credit the author for the original creation.');\"><img title=\"This license lets others distribute, remix, tweak, and build upon your work, even commercially, as long as they credit you for the original creation.\" src=\"" + currentMode.Base_URL + "default/images/cc_by.png\" /></a></td><td><b>Attribution</b><br /><i>cc by</i></td></tr>");
                    builder.AppendLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('prefRights','[cc by-sa] This item is licensed with the Creative Commons Attribution Share Alike License.  This license lets others remix, tweak, and build upon this work even for commercial reasons, as long as they credit the author and license their new creations under the identical terms.');\"><img title=\"This license lets others remix, tweak, and build upon your work even for commercial reasons, as long as they credit you and license their new creations under the identical terms.\" src=\"" + currentMode.Base_URL + "default/images/cc_by_sa.png\" /></a></td><td><b>Attribution Share Alike</b><br /><i>cc by-sa</i></td></tr>");
                    builder.AppendLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('prefRights','[cc by-nd] This item is licensed with the Creative Commons Attribution No Derivatives License.  This license allows for redistribution, commercial and non-commercial, as long as it is passed along unchanged and in whole, with credit to the author.');\"><img title=\"This license allows for redistribution, commercial and non-commercial, as long as it is passed along unchanged and in whole, with credit to you.\" src=\"" + currentMode.Base_URL + "default/images/cc_by_nd.png\" /></a></td><td><b>Attribution No Derivatives</b><br /><i>cc by-nd</i></td></tr>");
                    builder.AppendLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('prefRights','[cc by-nc] This item is licensed with the Creative Commons Attribution Non-Commerical License.  This license lets others remix, tweak, and build upon this work non-commercially, and although their new works must also acknowledge the author and be non-commercial, they don’t have to license their derivative works on the same terms.');\"><img title=\"This license lets others remix, tweak, and build upon your work non-commercially, and although their new works must also acknowledge you and be non-commercial, they don’t have to license their derivative works on the same terms.\" src=\"" + currentMode.Base_URL + "default/images/cc_by_nc.png\" /></a></td><td><b>Attribution Non-Commercial</b><br /><i>cc by-nc</i></td></tr>");
                    builder.AppendLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('prefRights','[cc by-nc-sa] This item is licensed with the Creative Commons Attribution Non-Commercial Share Alike License.  This license lets others remix, tweak, and build upon this work non-commercially, as long as they credit the author and license their new creations under the identical terms.');\"><img title=\"This license lets others remix, tweak, and build upon your work non-commercially, as long as they credit you and license their new creations under the identical terms.\" src=\"" + currentMode.Base_URL + "default/images/cc_by_nc_sa.png\" /></a></td><td><b>Attribution Non-Commercial Share Alike</b><br /><i>cc by-nc-sa</i></td></tr>");
                    builder.AppendLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('prefRights','[cc by-nc-nd] This item is licensed with the Creative Commons Attribution Non-Commercial No Derivative License.  This license allows others to download this work and share them with others as long as they mention the author and link back to the author, but they can’t change them in any way or use them commercially.');\"><img title=\"This license allows others to download your works and share them with others as long as they mention you and link back to you, but they can’t change them in any way or use them commercially.\" src=\"" + currentMode.Base_URL + "default/images/cc_by_nc_nd.png\" /></a></td><td><b>Attribution Non-Commercial No Derivatives</b><br /><i>cc by-nc-nd</i></td></tr>");
                    builder.AppendLine("      </table>");
                    builder.AppendLine("    </td>");
                    builder.AppendLine("  </tr>");

                }

            builder.AppendLine("  <tr><td colspan=\"3\" class=\"SobekCitationSectionTitle1\"><b>&nbsp;" + otherPreferencesLabel + "</b></td></tr>");
            builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td><b>" + languageLabel + ":</b></td>");
            builder.AppendLine("    <td>");
            builder.AppendLine("      <select name=\"prefLanguage\" id=\"prefLanguage\" class=\"preferences_language_select\" >");
            if ((language != "Français") && (language != "Español"))
            {
                builder.AppendLine("        <option selected=\"selected\" value=\"en\">English</option>");
            }
            else
            {
                builder.AppendLine("        <option value=\"en\">English</option>");
            }
            builder.AppendLine(language == "Français"
                                   ? "        <option selected=\"selected\" value=\"fr\">Français</option>"
                                   : "        <option value=\"fr\">Français</option>");
            builder.AppendLine(language == "Español"
                                   ? "        <option selected=\"selected\" value=\"es\">Español</option>"
                                   : "        <option value=\"es\">Español</option>");
            builder.AppendLine("      </select>");
            builder.AppendLine("    </td>");
            builder.AppendLine("  </tr>");

            if (registration)
            {
                if (!desire_to_upload)
                {
                    builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td colspan=\"2\"><input type=\"checkbox\" value=\"allowsubmit\" name=\"prefAllowSubmit\" id=\"prefAllowSubmit\" /><label for=\"prefAllowSubmit\">I would like to be able to submit materials online. (Once your application to submit has been approved, you will receive email notification)</label></td></tr>");
                }
                else
                {
                    builder.AppendLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td colspan=\"2\"><input type=\"checkbox\" value=\"allowsubmit\" name=\"prefAllowSubmit\" id=\"prefAllowSubmit\" checked=\"checked\" /><label for=\"prefAllowSubmit\">I would like to be able to submit materials online. (Once your application to submit has been approved, you will receive email notification)</label></td></tr>");
                }
            }

            builder.AppendLine("  <tr align=\"right\"><td colspan=\"3\">");
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
            builder.AppendLine("    <a href=\"" + currentMode.Redirect_URL() + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button.gif\" border=\"0\" alt=\"CANCEL\" /></a> &nbsp; " );
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;

            LiteralControl literal1 = new LiteralControl(builder.ToString());
            MainPlaceHolder.Controls.Add(literal1);

            // Add the submit button
            ImageButton submitButton = new ImageButton
                                           {
                                               ImageUrl = currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button.gif",
                                               AlternateText = "SAVE"
                                           };
            submitButton.Click += submitButton_Click;
            MainPlaceHolder.Controls.Add(submitButton);

            MainPlaceHolder.Controls.Add(registration
                                         ? new LiteralControl( "</td></tr></table></blockquote></div>\n\n<!-- Focus on the first registration text box -->\n<script type=\"text/javascript\">focus_element('prefUsername');</script>\n\n")
                                         : new LiteralControl("</td></tr></table></blockquote></div>\n\n<!-- Focus on the first preferences text box -->\n<script type=\"text/javascript\">focus_element('prefFamilyName');</script>\n\n"));
        }

        void submitButton_Click(object sender, ImageClickEventArgs e)
        {
            
        }
    }
}
