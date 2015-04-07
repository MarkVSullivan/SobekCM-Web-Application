#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Email;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Settings;
using SobekCM.Tools;
using SobekCM.UI_Library;

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
        private readonly List<string> validationErrors;
        private User_Object user;

		private readonly bool registration;
		private readonly bool desire_to_upload;
		private readonly bool send_email_on_submission;
		private readonly bool send_usages_emails;
		private readonly string family_name;
		private readonly string given_name;
		private readonly string nickname;
		private readonly string email;
		private readonly string organization;
		private readonly string college;
		private readonly string department;
		private readonly string unit;
		private readonly string template;
		private readonly string project;
		private readonly string username;
		private readonly string password;
		private readonly string password2;
		private string ufid;
	    private readonly string language;
		private readonly string default_rights;

		private readonly string mySobekText;
		private readonly string accountInfoLabel;
		private readonly string userNameLabel;
		private readonly string personalInfoLabel;
		private readonly string familyNamesLabel;
		private readonly string givenNamesLabel;
		private readonly string nicknameLabel;
		private readonly string emailLabel;
		private readonly string emailStatsLabel;
		private readonly string affilitionInfoLabel;
		private readonly string organizationLabel;
		private readonly string collegeLabel;
		private readonly string departmentLabel;
		private readonly string unitLabel;
		private readonly string selfSubmittalPrefLabel;
		private readonly string sendEmailLabel;
		private readonly string templateLabel;
		private readonly string projectLabel;
		private readonly string defaultRightsLabel;
		private readonly string rightsExplanationLabel;
		private readonly string rightsInstructionLabel;
		private readonly string otherPreferencesLabel;
		private readonly string languageLabel;
		private readonly string passwordLabel;
		private readonly string confirmPasswordLabel;
		private readonly string col1Width;
		private readonly string col2Width;
		private readonly string col3Width;

	    /// <summary> Constructor for a new instance of the Preferences_MySobekViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Preferences_MySobekViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("Preferences_MySobekViewer.Constructor", String.Empty);

            validationErrors = new List<string>();

			// Set the text to use for each value (since we use if for the validation errors as well)
			mySobekText = "my" + RequestSpecificValues.Current_Mode.SobekCM_Instance_Abbreviation;

			// Get the labels to use, by language
			accountInfoLabel = "Account Information";
			userNameLabel = "UserName";
			personalInfoLabel = "Personal Information";
			familyNamesLabel = "Last/Family Name(s)";
			givenNamesLabel = "First/Given Name(s)";
			nicknameLabel = "Nickname";
			emailLabel = "Email";
			emailStatsLabel = "Send me monthly usage statistics for my items";
			affilitionInfoLabel = "Current Affiliation Information";
			organizationLabel = "Organization/University";
			collegeLabel = "College";
			departmentLabel = "Department";
			unitLabel = "Unit";
			selfSubmittalPrefLabel = "Self-Submittal Preferences";
			sendEmailLabel = "Send me an email when I submit new items";
			templateLabel = "Template";
			projectLabel = "Default Metadata";
			defaultRightsLabel = "Default Rights";
			rightsExplanationLabel = "(These are the default rights you give for sharing, repurposing, or remixing your item to other users. You can set this with each new item you submit, but this will be the default that appears.)";
			rightsInstructionLabel = "You may also select a <a title=\"Explanation of different creative commons licenses.\" href=\"http://creativecommons.org/about/licenses/\">Creative Commons License</a> option below.";
			otherPreferencesLabel = "Other Preferences";
			languageLabel = "Language";
			passwordLabel = "Password";
			confirmPasswordLabel = "Confirm Password";
			col1Width = "15px";
			col2Width = "100px";
			col3Width = "605px";

			if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
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

			if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
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

			// Is this for registration
            user = RequestSpecificValues.Current_User;
			registration = (HttpContext.Current.Session["user"] == null);
			if (registration)
			{
				user = new User_Object();
			}

		
			// Set some default first
			send_usages_emails = true;
	        family_name = String.Empty;
			given_name = String.Empty;
			nickname = String.Empty;
			email = String.Empty;
			organization = String.Empty;
			college = String.Empty;
			department = String.Empty;
			unit = String.Empty;
			template = String.Empty;
			project = String.Empty;
			username = String.Empty;
			password = String.Empty;
			password2 = String.Empty;
			ufid = String.Empty;
			language = String.Empty;
			default_rights = String.Empty;

			// Handle post back
			if (RequestSpecificValues.Current_Mode.isPostBack)
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
							if (language_temp == "es")
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
					else if (password.Length < 8)
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
							if (!Int32.TryParse(ufid, out ufid_convert_test))
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
				if (default_rights.Trim().Length > 1000)
				{
					validationErrors.Add("Rights statement truncated to 1000 characters.");
					default_rights = default_rights.Substring(0, 1000);
				}

				if ((registration) && (validationErrors.Count == 0))
				{
					bool email_exists;
					bool username_exists;
                    SobekCM_Database.UserName_Exists(username, email, out username_exists, out email_exists, RequestSpecificValues.Tracer);
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
                    // Ensure the last name and first name are capitalized somewhat
                    bool all_caps = true;
				    bool all_lower = true;
				    foreach (char thisChar in family_name)
				    {
                        if (Char.IsUpper(thisChar))
                            all_lower = false;
                        if (Char.IsLower(thisChar))
                            all_caps = false;

                        if ((!all_caps) && (!all_lower))
                            break;
				    }
				    if ((all_caps) || (all_lower))
				    {
                        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                        family_name = textInfo.ToTitleCase(family_name.ToLower()); //War And Peace
				    }
                    all_lower = true;
                    all_caps = true;
                    foreach (char thisChar in given_name)
                    {
                        if (Char.IsUpper(thisChar))
                            all_lower = false;
                        if (Char.IsLower(thisChar))
                            all_caps = false;

                        if ((!all_caps) && (!all_lower))
                            break;
                    }
                    if ((all_caps) || (all_lower))
                    {
                        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                        given_name = textInfo.ToTitleCase(given_name.ToLower()); //War And Peace
                    }

                    // Now, add this information to the user, so the new user can be saved
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
                    if ((!registration) && (user.Default_Metadata_Sets_Count > 0 ) && ( user.Default_Metadata_Sets[0] != project.Trim()))
					{
						// Determine the in process directory for this
						string user_in_process_directory = UI_ApplicationCache_Gateway.Settings.In_Process_Submission_Location + "\\" + user.UserName;
						if (user.ShibbID.Trim().Length > 0)
							user_in_process_directory = UI_ApplicationCache_Gateway.Settings.In_Process_Submission_Location + "\\" + user.ShibbID;
						if (Directory.Exists(user_in_process_directory))
						{
							if (File.Exists(user_in_process_directory + "\\TEMP000001_00001.mets"))
								File.Delete(user_in_process_directory + "\\TEMP000001_00001.mets");
						}
					}
					user.Set_Current_Default_Metadata(project.Trim());
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
                        SobekCM_Database.Save_User(user, password, user.Authentication_Type, RequestSpecificValues.Tracer);

						// Retrieve the user from the database
                        user = SobekCM_Database.Get_User(username, password, RequestSpecificValues.Tracer);

						// Special code in case this is the very first user
						if (user.UserID == 1)
						{
							// Add each template and project
                            DataSet projectTemplateSet = Engine_Database.Get_All_Template_DefaultMetadatas(RequestSpecificValues.Tracer);
							List<string> templates = (from DataRow thisTemplate in projectTemplateSet.Tables[1].Rows select thisTemplate["TemplateCode"].ToString()).ToList();
							List<string> projects = (from DataRow thisProject in projectTemplateSet.Tables[0].Rows select thisProject["MetadataCode"].ToString()).ToList();

							// Save the updates to this admin user
                            SobekCM_Database.Save_User(user, password, User_Authentication_Type_Enum.Sobek, RequestSpecificValues.Tracer);
                            SobekCM_Database.Update_SobekCM_User(user.UserID, true, true, true, true, true, true, true, "edit_internal", "editmarc_internal", true, true, true, RequestSpecificValues.Tracer);
                            SobekCM_Database.Update_SobekCM_User_DefaultMetadata(user.UserID, new ReadOnlyCollection<string>(projects), RequestSpecificValues.Tracer);
                            SobekCM_Database.Update_SobekCM_User_Templates(user.UserID, new ReadOnlyCollection<string>(templates), RequestSpecificValues.Tracer);

							// Retrieve the user information again
                            user = SobekCM_Database.Get_User(username, password, RequestSpecificValues.Tracer);

                            // Also, use the current email address for some system emails
						    if (user.Email.Length > 0)
						    {
                                SobekCM_Database.Set_Setting("System Email", user.Email);
                                SobekCM_Database.Set_Setting("System Error Email", user.Email);
                                SobekCM_Database.Set_Setting("Privacy Email Address", user.Email);
                                SobekCM_Database.Set_Setting("Email Default From Address", user.Email);
						    }
						}

						user.Is_Just_Registered = true;
						HttpContext.Current.Session["user"] = user;

						// If they want to be able to contribue, send an email
						if (desire_to_upload)
						{
                            Email_Helper.SendEmail(UI_ApplicationCache_Gateway.Settings.System_Email, "Submittal rights requested by " + user.Full_Name, "New user requested ability to submit new items.<br /><br /><blockquote>Name: " + user.Full_Name + "<br />Email: " + user.Email + "<br />Organization: " + user.Organization + "<br />User ID: " + user.UserID + "</blockquote>", true, RequestSpecificValues.Current_Mode.SobekCM_Instance_Name );
						}

						// Email the user their registation information
						if (desire_to_upload)
						{
                            Email_Helper.SendEmail(email, "Welcome to " + mySobekText, "<strong>Thank you for registering for " + mySobekText + "</strong><br /><br />You can access this directly through the following link: <a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "/my\">" + RequestSpecificValues.Current_Mode.Base_URL + "/my</a><br /><br />Full Name: " + user.Full_Name + "<br />User Name: " + user.UserName + "<br /><br />You will receive an email when your request to submit items has been processed.", true, RequestSpecificValues.Current_Mode.SobekCM_Instance_Name);
						}
						else
						{
                            Email_Helper.SendEmail(email, "Welcome to " + mySobekText, "<strong>Thank you for registering for " + mySobekText + "</strong><br /><br />You can access this directly through the following link: <a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "/my\">" + RequestSpecificValues.Current_Mode.Base_URL + "/my</a><br /><br />Full Name: " + user.Full_Name + "<br />User Name: " + user.UserName, true, RequestSpecificValues.Current_Mode.SobekCM_Instance_Name);
						}

						// Now, forward back to the My Sobek home page
						RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;

						// If this is the first user to register (who would have been set to admin), send to the 
						// system-wide settings screen
						if (user.UserID == 1)
						{
							RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
							RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Settings;
						}
						UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
					}
					else
					{
						HttpContext.Current.Session["user"] = user;
                        SobekCM_Database.Save_User(user, String.Empty, user.Authentication_Type, RequestSpecificValues.Tracer);

						// Now, forward back to the My Sobek home page
						RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
						UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
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
				language = user.Preferred_Language;
				send_email_on_submission = user.Send_Email_On_Submission;
				default_rights = user.Default_Rights;

			}
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This value changes; if the user is logged on it returns 'Edit Your Preferences', otherwise it has a 'Register for...' message </value>
        public override string Web_Title
        {
            get
            {
                if (HttpContext.Current.Session["user"] == null)
                {
                    return "Register for My" + RequestSpecificValues.Current_Mode.SobekCM_Instance_Abbreviation;
                }
                return "Edit Your Account Preferences";
            }
        }

	    public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
	    {
		    // Do nothing
	    }

	    /// <summary> Add the HTML to be displayed in the main SobekCM viewer area with the form </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Preferences_MySobekViewer.Write_HTML", "Do nothing");

			Output.WriteLine("<h1>" + Web_Title + "</h1>");
			Output.WriteLine();
            Output.WriteLine("<script src=\"" + Static_Resources.Sobekcm_Metadata_Js + "\" type=\"text/javascript\"></script>");
			Output.WriteLine("<div class=\"SobekHomeText\" >");
			Output.WriteLine("<blockquote>");
			if (registration)
			{
				Output.WriteLine("Registration for " + mySobekText + " is free and open to the public.  Enter your information below to be instantly registered.<br /><br />");
				Output.WriteLine("Account information, name, and email are required for each new account.<br /><br />");
				RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
				Output.WriteLine("Already registered?  <a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Log on</a>.<br /><br />");
				RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
			}
			if (validationErrors.Count > 0)
			{
				Output.WriteLine("<span style=\"color: Red;font-weight:bold;\">The following errors were detected:");
				Output.WriteLine("<blockquote>");
				foreach (string thisError in validationErrors)
				{
					Output.WriteLine(thisError + "<br />");
				}
				Output.WriteLine("</blockquote>");
				Output.WriteLine("</span>");
			}

			Output.WriteLine("<table style=\"width:700px;\" cellpadding=\"5px\" class=\"sbkPmsv_InputTable\" >");
			Output.WriteLine("  <tr><th colspan=\"3\">" + accountInfoLabel + "</td></tr>");
			if (registration)
			{
				// If there was a gatorlink ufid, use that
				if (HttpContext.Current.Session["Gatorlink_UFID"] != null)
					ufid = HttpContext.Current.Session["Gatorlink_UFID"].ToString();

				Output.WriteLine("  <tr><td style=\"width:" + col1Width + "\">&nbsp;</td><td style=\"width:" + col2Width + "\" class=\"sbkPmsv_InputLabel\"><label for=\"prefUsername\">" + userNameLabel + ":</label></td><td width=\"" + col3Width + "\"><input id=\"prefUserName\" name=\"prefUserName\" class=\"preferences_small_input sbk_Focusable\" value=\"" + username + "\" type=\"text\" />   &nbsp; &nbsp; (minimum of eight digits)</td></tr>");
				Output.WriteLine("  <tr><td style=\"width:" + col1Width + "\">&nbsp;</td><td class=\"sbkPmsv_InputLabel\"><label for=\"password_enter\">" + passwordLabel + ":</label></td><td>");

				Output.WriteLine("    <input type=\"password\" id=\"password_enter\" name=\"password_enter\" class=\"preferences_small_input sbk_Focusable\" value=\"\" />");



				Output.WriteLine("     &nbsp; &nbsp; (minimum of eight digits, different than username)</td></tr>");
				Output.WriteLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td class=\"sbkPmsv_InputLabel\"><label for=\"password_confirm\">" + confirmPasswordLabel + ":</label></td><td>");

				Output.WriteLine("    <input type=\"password\" id=\"password_confirm\" name=\"password_confirm\" class=\"preferences_small_input sbk_Focusable\" value=\"\" />");

				Output.WriteLine("     &nbsp; &nbsp; (minimum of eight digits, different than username)</td></tr>");
			}
			else
			{
				Output.WriteLine("  <tr><td style=\"width:" + col1Width + "\">&nbsp;</td><td class=\"sbkPmsv_InputLabel\">" + userNameLabel + ":</td><td>" + user.UserName + "</td></tr>");
				if ((user.ShibbID.Trim().Length > 0) && ( UI_ApplicationCache_Gateway.Settings.Shibboleth != null ) && ( UI_ApplicationCache_Gateway.Settings.Shibboleth.Enabled ) && ( UI_ApplicationCache_Gateway.Settings.Shibboleth.Label.Length > 0 ))
				{
                    Output.WriteLine("  <tr><td width=\"" + col1Width + "\">&nbsp;</td><td class=\"sbkPmsv_InputLabel\">" + UI_ApplicationCache_Gateway.Settings.Shibboleth.Label + ":</td><td>" + user.ShibbID + "</td></tr>");
				}
			}

			Output.WriteLine("  <tr><th colspan=\"3\">" + personalInfoLabel + "</td></tr>");

			Output.WriteLine("  <tr><td style=\"width:" + col1Width + "\">&nbsp;</td><td class=\"sbkPmsv_InputLabel\"><label for=\"prefGivenName\">" + givenNamesLabel + ":</label></td><td><input id=\"prefGivenName\" name=\"prefGivenName\" class=\"preferences_medium_input sbk_Focusable\" value=\"" + given_name + "\" type=\"text\" /></td></tr>");
			Output.WriteLine("  <tr><td>&nbsp;</td><td class=\"sbkPmsv_InputLabel\"><label for=\"prefFamilyName\">" + familyNamesLabel + ":</label></td><td><input id=\"prefFamilyName\" name=\"prefFamilyName\" class=\"preferences_medium_input sbk_Focusable\" value=\"" + family_name + "\" type=\"text\" /></td></tr>");
			Output.WriteLine("  <tr><td>&nbsp;</td><td class=\"sbkPmsv_InputLabel\"><label for=\"prefNickName\">" + nicknameLabel + ":</label></td><td><input id=\"prefNickName\" name=\"prefNickName\" class=\"preferences_medium_input sbk_Focusable\" value=\"" + nickname + "\" type=\"text\" /></td></tr>");
			Output.WriteLine("  <tr><td>&nbsp;</td><td class=\"sbkPmsv_InputLabel\"><label for=\"prefEmail\">" + emailLabel + ":</label></td><td><input id=\"prefEmail\" name=\"prefEmail\" class=\"preferences_medium_input sbk_Focusable\" value=\"" + email + "\" type=\"text\" /></td></tr>");

			if (user.Has_Item_Stats)
			{
				if (!send_usages_emails)
				{
					Output.WriteLine("  <tr><td colspan=\"2\"></td><td><input type=\"checkbox\" value=\"sendemail\" name=\"prefStatsEmail\" id=\"prefStatsEmail\" /><label for=\"prefStatsEmail\">" + emailStatsLabel + "</label></td></tr>");
				}
				else
				{
					Output.WriteLine("  <tr><td colspan=\"2\"></td><td><input type=\"checkbox\" value=\"allowsubmit\" name=\"prefStatsEmail\" id=\"prefStatsEmail\" checked=\"checked\" /><label for=\"prefStatsEmail\">" + emailStatsLabel + "</label></td></tr>");
				}
			}

			Output.WriteLine("  <tr><th colspan=\"3\">" + affilitionInfoLabel + "</td></tr>");

			Output.WriteLine("  <tr><td>&nbsp;</td><td class=\"sbkPmsv_InputLabel\"><label for=\"prefOrganization\">" + organizationLabel + ":</label></td><td><input id=\"prefOrganization\" name=\"prefOrganization\" class=\"preferences_large_input sbk_Focusable\" value=\"" + organization + "\" type=\"text\" /></td></tr>");
			Output.WriteLine("  <tr><td>&nbsp;</td><td class=\"sbkPmsv_InputLabel\"><label for=\"prefCollege\">" + collegeLabel + ":</label></td><td><input id=\"prefCollege\" name=\"prefCollege\" class=\"preferences_large_input sbk_Focusable\" value=\"" + college + "\"type=\"text\" /></td></tr>");
			Output.WriteLine("  <tr><td>&nbsp;</td><td class=\"sbkPmsv_InputLabel\"><label for=\"prefDepartment\">" + departmentLabel + ":</label></td><td><input id=\"prefDepartment\" name=\"prefDepartment\" class=\"preferences_large_input sbk_Focusable\" value=\"" + department + "\"type=\"text\" /></td></tr>");
			Output.WriteLine("  <tr><td>&nbsp;</td><td class=\"sbkPmsv_InputLabel\"><label for=\"prefUnit\">" + unitLabel + ":</label></td><td><input id=\"prefUnit\" name=\"prefUnit\" class=\"preferences_large_input sbk_Focusable\" value=\"" + unit + "\" type=\"text\" /></td></tr>");

            if ((registration) && ( UI_ApplicationCache_Gateway.Settings.Shibboleth != null ) && ( UI_ApplicationCache_Gateway.Settings.Shibboleth.Enabled ) && ( UI_ApplicationCache_Gateway.Settings.Shibboleth.Label.Length > 0 ))
			{
                Output.WriteLine("  <tr><td>&nbsp;</td><td class=\"sbkPmsv_InputLabel\"><label for=\"prefUfid\">" + UI_ApplicationCache_Gateway.Settings.Shibboleth.Label + ":</label></td><td><input id=\"prefUfid\" name=\"prefUfid\" class=\"preferences_small_input sbk_Focusable\" value=\"" + ufid + "\" type=\"text\" />    &nbsp; &nbsp; (optionally provides access through Gatorlink)</td></tr>");
			}


			if (user.Can_Submit)
			{
				Output.WriteLine("  <tr><th colspan=\"3\">" + selfSubmittalPrefLabel + "</td></tr>");

				if (!send_email_on_submission)
				{
					Output.WriteLine("  <tr><td colspan=\"2\"></td><td><input type=\"checkbox\" value=\"sendemail\" name=\"prefSendEmail\" id=\"prefSendEmail\" /><label for=\"prefSendEmail\">" + sendEmailLabel + "</label></td></tr>");
				}
				else
				{
					Output.WriteLine("  <tr><td colspan=\"2\"></td><td><input type=\"checkbox\" value=\"sendemail\" name=\"prefSendEmail\" id=\"prefSendEmail\" checked=\"checked\" /><label for=\"prefSendEmail\">" + sendEmailLabel + "</label></td></tr>");
				}

				if (user.Templates.Count > 0)
				{
					Output.WriteLine("  <tr><td>&nbsp;</td><td class=\"sbkPmsv_InputLabel\">" + templateLabel + ":</td>");
					Output.WriteLine("    <td>");
					Output.WriteLine("      <select name=\"prefTemplate\" id=\"prefTemplate\" class=\"preferences_language_select\" >");
					Output.WriteLine("        <option selected=\"selected\" value=\"" + user.Templates[0] + "\">" + user.Templates[0] + "</option>");
					for (int i = 1; i < user.Templates.Count; i++)
					{
						Output.WriteLine("        <option value=\"" + user.Templates[i] + "\">" + user.Templates[i] + "</option>");
					}
					Output.WriteLine("      </select>");
					Output.WriteLine("    </td>");
					Output.WriteLine("  </tr>");
				}
				if (user.Default_Metadata_Sets.Count > 0)
				{
					Output.WriteLine("  <tr><td>&nbsp;</td><td class=\"sbkPmsv_InputLabel\">" + projectLabel + ":</td>");
					Output.WriteLine("    <td>");
					Output.WriteLine("      <select name=\"prefProject\" id=\"prefProject\" class=\"preferences_language_select\" >");
                    Output.WriteLine("        <option selected=\"selected\" value=\"" + user.Default_Metadata_Sets[0] + "\">" + user.Default_Metadata_Sets[0] + "</option>");
                    for (int i = 1; i < user.Default_Metadata_Sets.Count; i++)
					{
                        Output.WriteLine("        <option value=\"" + user.Default_Metadata_Sets[i] + "\">" + user.Default_Metadata_Sets[i] + "</option>");
					}
					Output.WriteLine("      </select>");
					Output.WriteLine("    </td>");
					Output.WriteLine("  </tr>");
				}
				Output.WriteLine("  <tr style=\"vertical-align:top\"><td>&nbsp;</td><td class=\"sbkPmsv_InputLabel\">" + defaultRightsLabel + ":</td><td>" + rightsExplanationLabel + "</td></tr>");
				Output.WriteLine("  <tr><td colspan=\"2\">&nbsp;<td><textarea rows=\"5\" cols=\"88\" name=\"prefRights\" id=\"prefRights\" class=\"preference_rights_input sbk_Focusable\">" + default_rights + "</textarea></div></td></tr>");
				Output.WriteLine("  <tr valign=\"top\">");
				Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
				Output.WriteLine("    <td>");
				Output.WriteLine("      " + rightsInstructionLabel + "<br />");
				Output.WriteLine("      <table cellpadding=\"3px\" cellspacing=\"3px\" >");
                Output.WriteLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('prefRights','[cc0] The author dedicated the work to the Commons by waiving all of his or her rights to the work worldwide under copyright law and all related or neighboring legal rights he or she had in the work, to the extent allowable by law.');\"><img title=\"You dedicate the work to the Commons by waiving all of your rights to the work worldwide under copyright law and all related or neighboring legal rights you had in the work, to the extent allowable by law.\" src=\"" + Static_Resources.Cc_Zero_Img + "\" /></a></td><td><b>No Copyright</b><br /><i>cc0</i></td></tr>");
                Output.WriteLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('prefRights','[cc by] This item is licensed with the Creative Commons Attribution License.  This license lets others distribute, remix, tweak, and build upon this work, even commercially, as long as they credit the author for the original creation.');\"><img title=\"This license lets others distribute, remix, tweak, and build upon your work, even commercially, as long as they credit you for the original creation.\" src=\"" + Static_Resources.Cc_By_Img + "\" /></a></td><td><b>Attribution</b><br /><i>cc by</i></td></tr>");
                Output.WriteLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('prefRights','[cc by-sa] This item is licensed with the Creative Commons Attribution Share Alike License.  This license lets others remix, tweak, and build upon this work even for commercial reasons, as long as they credit the author and license their new creations under the identical terms.');\"><img title=\"This license lets others remix, tweak, and build upon your work even for commercial reasons, as long as they credit you and license their new creations under the identical terms.\" src=\"" + Static_Resources.Cc_By_Sa_Img + "\" /></a></td><td><b>Attribution Share Alike</b><br /><i>cc by-sa</i></td></tr>");
                Output.WriteLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('prefRights','[cc by-nd] This item is licensed with the Creative Commons Attribution No Derivatives License.  This license allows for redistribution, commercial and non-commercial, as long as it is passed along unchanged and in whole, with credit to the author.');\"><img title=\"This license allows for redistribution, commercial and non-commercial, as long as it is passed along unchanged and in whole, with credit to you.\" src=\"" + Static_Resources.Cc_By_Nd_Img + "\" /></a></td><td><b>Attribution No Derivatives</b><br /><i>cc by-nd</i></td></tr>");
                Output.WriteLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('prefRights','[cc by-nc] This item is licensed with the Creative Commons Attribution Non-Commerical License.  This license lets others remix, tweak, and build upon this work non-commercially, and although their new works must also acknowledge the author and be non-commercial, they don’t have to license their derivative works on the same terms.');\"><img title=\"This license lets others remix, tweak, and build upon your work non-commercially, and although their new works must also acknowledge you and be non-commercial, they don’t have to license their derivative works on the same terms.\" src=\"" + Static_Resources.Cc_By_Nc_Img + "\" /></a></td><td><b>Attribution Non-Commercial</b><br /><i>cc by-nc</i></td></tr>");
				Output.WriteLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('prefRights','[cc by-nc-sa] This item is licensed with the Creative Commons Attribution Non-Commercial Share Alike License.  This license lets others remix, tweak, and build upon this work non-commercially, as long as they credit the author and license their new creations under the identical terms.');\"><img title=\"This license lets others remix, tweak, and build upon your work non-commercially, as long as they credit you and license their new creations under the identical terms.\" src=\"" + Static_Resources.Cc_By_Nc_Sa_Img + "\" /></a></td><td><b>Attribution Non-Commercial Share Alike</b><br /><i>cc by-nc-sa</i></td></tr>");
				Output.WriteLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('prefRights','[cc by-nc-nd] This item is licensed with the Creative Commons Attribution Non-Commercial No Derivative License.  This license allows others to download this work and share them with others as long as they mention the author and link back to the author, but they can’t change them in any way or use them commercially.');\"><img title=\"This license allows others to download your works and share them with others as long as they mention you and link back to you, but they can’t change them in any way or use them commercially.\" src=\"" + Static_Resources.Cc_By_Nc_Nd_Img + "\" /></a></td><td><b>Attribution Non-Commercial No Derivatives</b><br /><i>cc by-nc-nd</i></td></tr>");
				Output.WriteLine("      </table>");
				Output.WriteLine("    </td>");
				Output.WriteLine("  </tr>");

			}

			Output.WriteLine("  <tr><th colspan=\"3\">" + otherPreferencesLabel + "</td></tr>");

			Output.WriteLine("  <tr><td>&nbsp;</td><td class=\"sbkPmsv_InputLabel\">" + languageLabel + ":</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <select name=\"prefLanguage\" id=\"prefLanguage\" class=\"preferences_language_select\" >");
			if ((language != "Français") && (language != "Español"))
			{
				Output.WriteLine("        <option selected=\"selected\" value=\"en\">English</option>");
			}
			else
			{
				Output.WriteLine("        <option value=\"en\">English</option>");
			}
			Output.WriteLine(language == "Français"
								   ? "        <option selected=\"selected\" value=\"fr\">Français</option>"
								   : "        <option value=\"fr\">Français</option>");
			Output.WriteLine(language == "Español"
								   ? "        <option selected=\"selected\" value=\"es\">Español</option>"
								   : "        <option value=\"es\">Español</option>");
			Output.WriteLine("      </select>");
			Output.WriteLine("    </td>");
			Output.WriteLine("  </tr>");

			if (registration)
			{
				if (!desire_to_upload)
				{
					Output.WriteLine("  <tr><td colspan=\"2\">&nbsp;</td><td><input type=\"checkbox\" value=\"allowsubmit\" name=\"prefAllowSubmit\" id=\"prefAllowSubmit\" /><label for=\"prefAllowSubmit\">I would like to be able to submit materials online. (Once your application to submit has been approved, you will receive email notification)</label></td></tr>");
				}
				else
				{
					Output.WriteLine("  <tr><td colspan=\"2\">&nbsp;</td><td><input type=\"checkbox\" value=\"allowsubmit\" name=\"prefAllowSubmit\" id=\"prefAllowSubmit\" checked=\"checked\" /><label for=\"prefAllowSubmit\">I would like to be able to submit materials online. (Once your application to submit has been approved, you will receive email notification)</label></td></tr>");
				}
			}

			Output.WriteLine("  <tr style=\"text-align:right\"><td colspan=\"3\">");
			RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
			Output.WriteLine("    <button onclick=\"window.location.href = '" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\" class=\"sbkMySobek_BigButton\"> CANCEL </button> &nbsp; &nbsp; ");
			RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;

			Output.WriteLine("    <button type=\"submit\" class=\"sbkMySobek_BigButton\"> SUBMIT </button> ");

			Output.WriteLine(registration
				 ? "</td></tr></table></blockquote></div>\n\n<!-- Focus on the first registration text box -->\n<script type=\"text/javascript\">focus_element('prefUsername');</script>"
				 : "</td></tr></table></blockquote></div>\n\n<!-- Focus on the first preferences text box -->\n<script type=\"text/javascript\">focus_element('prefGivenName');</script>");


        }
    }
}
