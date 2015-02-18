#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Email;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Tools;
using SobekCM.UI_Library;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated system admin to view all existing users, and choose a RequestSpecificValues.Current_User to edit </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to view all registered users digital library</li>
    /// </ul></remarks>
    public class Users_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;
        private readonly User_Object editUser;
        private readonly Users_Admin_Mode_Enum mode;

		#region Constructor and code to handle any post backs

		/// <summary> Constructor for a new instance of the Users_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
		/// <remarks> Postback from a RequestSpecificValues.Current_User edit or from reseting a RequestSpecificValues.Current_User's password is handled here in the constructor </remarks>
        public Users_AdminViewer(RequestCache RequestSpecificValues)
            : base(RequestSpecificValues)
		{
            RequestSpecificValues.Tracer.Add_Trace("Users_AdminViewer.Constructor", String.Empty);

			// Ensure the RequestSpecificValues.Current_User is the system admin
			if ((RequestSpecificValues.Current_User == null) || (!RequestSpecificValues.Current_User.Is_System_Admin))
			{
				RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
				RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
				UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
				return;
			}

			// Set the action message to clear initially
			actionMessage = String.Empty;

			// Get the user to edit, if there was a user id in the submode
			editUser = null;
			if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Length > 0)
			{
				try
				{
					int edit_userid = Convert.ToInt32(RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Replace("a", "").Replace("b", "").Replace("c", "").Replace("v", ""));

					// Check this admin's session for this RequestSpecificValues.Current_User object
					Object sessionEditUser = HttpContext.Current.Session["Edit_User_" + edit_userid];
					if (sessionEditUser != null)
						editUser = (User_Object)sessionEditUser;
					else
					{
                        editUser = SobekCM_Database.Get_User(edit_userid, RequestSpecificValues.Tracer);
						editUser.Should_Be_Able_To_Edit_All_Items = false;
						if (editUser.Editable_Regular_Expressions.Any(ThisRegularExpression => ThisRegularExpression == "[A-Z]{2}[A-Z|0-9]{4}[0-9]{4}"))
						{
							editUser.Should_Be_Able_To_Edit_All_Items = true;
						}
					}
				}
				catch (Exception)
				{
					actionMessage = "Error while handing your request";
				}
			}

			// Determine the mode
			mode = Users_Admin_Mode_Enum.List_Users_And_Groups;
			if (editUser != null)
			{
				mode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode.IndexOf("v") > 0 ? Users_Admin_Mode_Enum.View_User : Users_Admin_Mode_Enum.Edit_User;
			}
			else
			{
				RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
			}

			// Perform post back work
			if (RequestSpecificValues.Current_Mode.isPostBack)
			{
				if (mode == Users_Admin_Mode_Enum.List_Users_And_Groups)
				{
					try
					{
						string reset_value = HttpContext.Current.Request.Form["admin_user_reset"];
						if (reset_value.Length > 0)
						{
							int userid = Convert.ToInt32(reset_value);
                            User_Object reset_user = SobekCM_Database.Get_User(userid, RequestSpecificValues.Tracer);

							// Create the random password
							StringBuilder passwordBuilder = new StringBuilder();
							Random randomGenerator = new Random(DateTime.Now.Millisecond);
							while (passwordBuilder.Length < 12)
							{
								switch (randomGenerator.Next(0, 3))
								{
									case 0:
										int randomNumber = randomGenerator.Next(65, 91);
										if ((randomNumber != 79) && (randomNumber != 75)) // Omit the 'O' and the 'K', confusing
											passwordBuilder.Append((char)randomNumber);
										break;

									case 1:
										int randomNumber2 = randomGenerator.Next(97, 123);
										if ((randomNumber2 != 111) && (randomNumber2 != 108) && (randomNumber2 != 107))  // Omit the 'o' and the 'l' and the 'k', confusing
											passwordBuilder.Append((char)randomNumber2);
										break;

									case 2:
										// Zero and one is omitted in this range, confusing
										int randomNumber3 = randomGenerator.Next(50, 58);
										passwordBuilder.Append((char)randomNumber3);
										break;
								}
							}
							string password = passwordBuilder.ToString();

							// Reset this password
                            if (!SobekCM_Database.Reset_User_Password(userid, password, true, RequestSpecificValues.Tracer))
							{
								actionMessage = "ERROR reseting password";
							}
							else
							{
                                if ( Email_Helper.SendEmail(reset_user.Email, "my" + RequestSpecificValues.Current_Mode.SobekCM_Instance_Abbreviation.ToUpper() + " Password Reset", reset_user.Full_Name + ",\n\nYour my" + RequestSpecificValues.Current_Mode.SobekCM_Instance_Abbreviation.ToUpper() + " password has been reset to a temporary password.  The first time you logon, you will be required to change it.\n\n\tUsername: " + reset_user.UserName + "\n\tPassword: " + password + "\n\nYour password is case-sensitive and must be entered exactly as it appears above when logging on.\n\nIf you have any questions or problems logging on, feel free to contact us at " + UI_ApplicationCache_Gateway.Settings.System_Email + ", or reply to this email.\n\n" + RequestSpecificValues.Current_Mode.Base_URL + "my/home\n", false, RequestSpecificValues.Current_Mode.SobekCM_Instance_Name))
								{
									if ((RequestSpecificValues.Current_User.UserID == 1) || (RequestSpecificValues.Current_User.UserID == 2))
										actionMessage = "Reset of password (" + password + ") for '" + reset_user.Full_Name + "' complete";
									else
										actionMessage = "Reset of password for '" + reset_user.Full_Name + "' complete";
								}
								else
								{
									if ((RequestSpecificValues.Current_User.UserID == 1) || (RequestSpecificValues.Current_User.UserID == 2))
										actionMessage = "ERROR while sending new password (" + password + ") to '" + reset_user.Full_Name + "'!";
									else
										actionMessage = "ERROR while sending new password to '" + reset_user.Full_Name + "'!";
								}
							}
						}

						string delete_value = HttpContext.Current.Request.Form["admin_user_group_delete"];
					    if (delete_value.Length > 0)
					    {
					        int deleteId = Convert.ToInt32(delete_value);
					        int result = SobekCM_Database.Delete_User_Group(deleteId, null);
					        switch (result)
					        {
                                case 1: 
                                    actionMessage = "Succesfully deleted user group";
                                    break;

                                case -1:
                                    actionMessage = "ERROR while deleting user group - Cannot delete a user group which is still linked to users";
                                    break;

                                case -2:
                                    actionMessage = "ERROR - You cannot delete a special user group";
                                    break;

                                case -3:
                                    actionMessage = "ERROR while deleting user group - unknown exception caught";
                                    break;

					        }
                            return;
					    }
					}
					catch
					{
						actionMessage = "ERROR while checking postback";
					}
				}

				if ((mode == Users_Admin_Mode_Enum.Edit_User) && (editUser != null))
				{
					// Determine which page you are on
					int page = 1;
					if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.IndexOf("b") > 0)
						page = 2;
					else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.IndexOf("c") > 0)
						page = 3;

					// Get a reference to this form
					NameValueCollection form = HttpContext.Current.Request.Form;
					string[] getKeys = form.AllKeys;

					// Get the curret action
					string action = form["admin_user_save"];

                    // If this is CANCEL, get rid of the currrent edit object in the session
				    if (action == "cancel")
				    {
                        // Clear the RequestSpecificValues.Current_User from the sessions
                        HttpContext.Current.Session["Edit_User_" + editUser.UserID] = null;

                        // Redirect the RequestSpecificValues.Current_User
                        RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
                        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                        return;
				    }

					bool successful_save = true;
					switch (page)
					{
						case 1:
							string editTemplate = "Standard";
							List<string> projects = new List<string>();
							List<string> templates = new List<string>();

							// First, set some flags to FALSE
							editUser.Can_Submit = false;
							editUser.Is_Internal_User = false;
							editUser.Should_Be_Able_To_Edit_All_Items = false;
							editUser.Is_System_Admin = false;
							editUser.Is_Portal_Admin = false;
							editUser.Include_Tracking_In_Standard_Forms = false;

							// Step through each key
							foreach (string thisKey in getKeys)
							{
								switch (thisKey)
								{
									case "admin_user_submit":
										editUser.Can_Submit = true;
										break;

									case "admin_user_internal":
										editUser.Is_Internal_User = true;
										break;

									case "admin_user_editall":
										editUser.Should_Be_Able_To_Edit_All_Items = true;
										break;

									case "admin_user_deleteall":
										editUser.Can_Delete_All = true;
										break;

									case "admin_user_sysadmin":
										editUser.Is_System_Admin = true;
										break;

									case "admin_user_portaladmin":
										editUser.Is_Portal_Admin = true;
										break;

									case "admin_user_includetracking":
										editUser.Include_Tracking_In_Standard_Forms = true;
										break;

									case "admin_user_edittemplate":
										editTemplate = form["admin_user_edittemplate"];
										break;

									case "admin_user_organization":
										editUser.Organization = form["admin_user_organization"];
										break;

									case "admin_user_college":
										editUser.College = form["admin_user_college"];
										break;

									case "admin_user_department":
										editUser.Department = form["admin_user_department"];
										break;

									case "admin_user_unit":
										editUser.Unit = form["admin_user_unit"];
										break;

									case "admin_user_org_code":
										editUser.Organization_Code = form["admin_user_org_code"];
										break;

									default:
										if (thisKey.IndexOf("admin_user_template_") == 0)
										{
											templates.Add(thisKey.Replace("admin_user_template_", ""));
										}
										if (thisKey.IndexOf("admin_user_project_") == 0)
										{
											projects.Add(thisKey.Replace("admin_user_project_", ""));
										}
										break;
								}
							}

							// Determine the name for the actual edit templates from the combo box selection
							editUser.Edit_Template_Code_Simple = "edit";
							editUser.Edit_Template_Code_Complex = "editmarc";
							if (editTemplate == "internal")
							{
								editUser.Edit_Template_Code_Simple = "edit_internal";
								editUser.Edit_Template_Code_Complex = "editmarc_internal";
							}

							// Determine if the projects and templates need to be updated
							bool update_templates_projects = false;
							if ((templates.Count != editUser.Templates.Count) || (projects.Count != editUser.Default_Metadata_Sets.Count))
							{
								update_templates_projects = true;
							}
							else
							{
								// Check all of the templates
								if (templates.Any(template => !editUser.Templates.Contains(template)))
								{
									update_templates_projects = true;
								}

								// Check all the projects
								if (!update_templates_projects)
								{
									if (projects.Any(project => !editUser.Default_Metadata_Sets.Contains(project)))
									{
										update_templates_projects = true;
									}
								}
							}

							// Update the templates and projects, if requested
							if (update_templates_projects)
							{
								// Get the last defaults
								string default_project = String.Empty;
								string default_template = String.Empty;
                                if (editUser.Default_Metadata_Sets.Count > 0)
                                    default_project = editUser.Default_Metadata_Sets[0];
								if (editUser.Templates.Count > 0)
									default_template = editUser.Templates[0];

								// Now, set the RequestSpecificValues.Current_User's template and projects
								editUser.Clear_Default_Metadata_Sets();
								editUser.Clear_Templates();
								foreach (string thisProject in projects)
								{
									editUser.Add_Default_Metadata_Set(thisProject, false);
								}
								foreach (string thisTemplate in templates)
								{
									editUser.Add_Template(thisTemplate, false);
								}

								// Try to add back the defaults, which won't do anything if 
								// the old defaults aren't in the new list
								editUser.Set_Current_Default_Metadata(default_project);
								editUser.Set_Default_Template(default_template);
							}
							break;

						case 2:
							// Check the RequestSpecificValues.Current_User groups for update
							bool update_user_groups = false;
                            List<User_Group> userGroup = Engine_Database.Get_All_User_Groups(RequestSpecificValues.Tracer);
							List<string> newGroups = new List<string>();
                            foreach (User_Group thisRow in userGroup)
							{
								if (form["group_" + thisRow.UserGroupID] != null)
								{
									newGroups.Add(thisRow.Name);
								}
							}

							// Should we add the new RequestSpecificValues.Current_User groups?  Did it change?
							if (newGroups.Count != editUser.User_Groups.Count)
							{
								update_user_groups = true;
							}
							else
							{
								foreach (string thisGroup in newGroups)
								{
									if (!editUser.User_Groups.Contains(thisGroup))
									{
										update_user_groups = true;
										break;
									}
								}
							}
							if (update_user_groups)
							{
								editUser.Clear_UserGroup_Membership();
								foreach (string thisUserGroup in newGroups)
									editUser.Add_User_Group(thisUserGroup);
							}
							break;

						case 3:
							Dictionary<string, User_Permissioned_Aggregation> aggregations = new Dictionary<string, User_Permissioned_Aggregation>();

							// Step through each key
							foreach (string thisKey in getKeys)
							{
								if (thisKey.IndexOf("admin_project_onhome_") == 0)
								{
									string select_project = thisKey.Replace("admin_project_onhome_", "");
									if (aggregations.ContainsKey(select_project))
									{
										aggregations[select_project].OnHomePage = true;
									}
									else
									{
										aggregations.Add(select_project, new User_Permissioned_Aggregation(select_project, String.Empty, false, false, false, true, false));
									}
								}
								if (thisKey.IndexOf("admin_project_select_") == 0)
								{
									string select_project = thisKey.Replace("admin_project_select_", "");
									if (aggregations.ContainsKey(select_project))
									{
										aggregations[select_project].CanSelect = true;
									}
									else
									{
										aggregations.Add(select_project, new User_Permissioned_Aggregation(select_project, String.Empty, true, false, false, false, false));
									}
								}
								if (thisKey.IndexOf("admin_project_editall_") == 0)
								{
									string edit_project = thisKey.Replace("admin_project_edit_", "");
									if (aggregations.ContainsKey(edit_project))
									{
										aggregations[edit_project].CanEditItems = true;
									}
									else
									{
										aggregations.Add(edit_project, new User_Permissioned_Aggregation(edit_project, String.Empty, false, true, false, false, false));
									}
								}
								if (thisKey.IndexOf("admin_project_edit_metadata_") == 0)
								{
									string edit_project = thisKey.Replace("admin_project_edit_metadata_", "");
									if (aggregations.ContainsKey(edit_project))
									{
										aggregations[edit_project].CanEditMetadata = true;
									}
									else
									{
										User_Permissioned_Aggregation thisAggrLink = new User_Permissioned_Aggregation(edit_project, String.Empty, false, false, false, false, false) {CanEditMetadata = true};
									    aggregations.Add(edit_project, thisAggrLink);
									}
								}
								if (thisKey.IndexOf("admin_project_edit_behavior_") == 0)
								{
									string edit_project = thisKey.Replace("admin_project_edit_behavior_", "");
									if (aggregations.ContainsKey(edit_project))
									{
										aggregations[edit_project].CanEditBehaviors = true;
									}
									else
									{
										User_Permissioned_Aggregation thisAggrLink = new User_Permissioned_Aggregation(edit_project, String.Empty, false, false, false, false, false) {CanEditBehaviors = true};
									    aggregations.Add(edit_project, thisAggrLink);
									}
								}
								if (thisKey.IndexOf("admin_project_perform_qc_") == 0)
								{
									string edit_project = thisKey.Replace("admin_project_perform_qc_", "");
									if (aggregations.ContainsKey(edit_project))
									{
										aggregations[edit_project].CanPerformQc = true;
									}
									else
									{
										User_Permissioned_Aggregation thisAggrLink = new User_Permissioned_Aggregation(edit_project, String.Empty, false, false, false, false, false) {CanPerformQc = true};
									    aggregations.Add(edit_project, thisAggrLink);
									}
								}
								if (thisKey.IndexOf("admin_project_upload_files_") == 0)
								{
									string edit_project = thisKey.Replace("admin_project_upload_files_", "");
									if (aggregations.ContainsKey(edit_project))
									{
										aggregations[edit_project].CanUploadFiles = true;
									}
									else
									{
										User_Permissioned_Aggregation thisAggrLink = new User_Permissioned_Aggregation(edit_project, String.Empty, false, false, false, false, false) {CanUploadFiles = true};
									    aggregations.Add(edit_project, thisAggrLink);
									}
								}
								if (thisKey.IndexOf("admin_project_change_visibility_") == 0)
								{
									string edit_project = thisKey.Replace("admin_project_change_visibility_", "");
									if (aggregations.ContainsKey(edit_project))
									{
										aggregations[edit_project].CanChangeVisibility = true;
									}
									else
									{
										User_Permissioned_Aggregation thisAggrLink = new User_Permissioned_Aggregation(edit_project, String.Empty, false, false, false, false, false) {CanChangeVisibility = true};
									    aggregations.Add(edit_project, thisAggrLink);
									}
								}
								if (thisKey.IndexOf("admin_project_can_delete_") == 0)
								{
									string edit_project = thisKey.Replace("admin_project_can_delete_", "");
									if (aggregations.ContainsKey(edit_project))
									{
										aggregations[edit_project].CanDelete = true;
									}
									else
									{
										User_Permissioned_Aggregation thisAggrLink = new User_Permissioned_Aggregation(edit_project, String.Empty, false, false, false, false, false) {CanDelete = true};
									    aggregations.Add(edit_project, thisAggrLink);
									}
								} 
								if (thisKey.IndexOf("admin_project_curator_") == 0)
								{
									string admin_project = thisKey.Replace("admin_project_curator_", "");
									if (aggregations.ContainsKey(admin_project))
									{
										aggregations[admin_project].IsCurator = true;
									}
									else
									{
										aggregations.Add(admin_project, new User_Permissioned_Aggregation(admin_project, String.Empty, false, false, true, false, false));
									}
								}
								if (thisKey.IndexOf("admin_project_admin_") == 0)
								{
									string admin_project = thisKey.Replace("admin_project_admin_", "");
									if (aggregations.ContainsKey(admin_project))
									{
										aggregations[admin_project].IsAdmin = true;
									}
									else
									{
										aggregations.Add(admin_project, new User_Permissioned_Aggregation(admin_project, String.Empty, false, false, false, false, true));
									}
								}
							}

							// Determine if the aggregationPermissions need to be edited
							bool update_aggregations = false;
                            if (editUser.PermissionedAggregations == null || (aggregations.Count != editUser.PermissionedAggregations.Count))
							{
								update_aggregations = true;
							}
							else
							{
								// Build a dictionary of the RequestSpecificValues.Current_User aggregationPermissions as well
                                Dictionary<string, User_Permissioned_Aggregation> existingAggr = editUser.PermissionedAggregations.ToDictionary(ThisAggr => ThisAggr.Code);

								// Check all the aggregationPermissions
								foreach (User_Permissioned_Aggregation adminAggr in aggregations.Values)
								{
									if (existingAggr.ContainsKey(adminAggr.Code))
									{
										if ((adminAggr.CanSelect != existingAggr[adminAggr.Code].CanSelect) || (adminAggr.CanEditMetadata != existingAggr[adminAggr.Code].CanEditMetadata) ||
											(adminAggr.CanEditBehaviors != existingAggr[adminAggr.Code].CanEditBehaviors) || (adminAggr.CanPerformQc != existingAggr[adminAggr.Code].CanPerformQc) ||
											(adminAggr.CanUploadFiles != existingAggr[adminAggr.Code].CanUploadFiles) || (adminAggr.CanChangeVisibility != existingAggr[adminAggr.Code].CanChangeVisibility) ||
											(adminAggr.CanDelete != existingAggr[adminAggr.Code].CanDelete) || (adminAggr.IsCurator != existingAggr[adminAggr.Code].IsCurator) ||
                                            (adminAggr.OnHomePage != existingAggr[adminAggr.Code].OnHomePage) || (adminAggr.IsAdmin != existingAggr[adminAggr.Code].IsAdmin))
										{
											update_aggregations = true;
											break;
										}
									}
									else
									{
										update_aggregations = true;
										break;
									}
								}
							}

							// Update the aggregationPermissions, if requested
							if (update_aggregations)
							{
								editUser.Clear_Aggregations();
								if (aggregations.Count > 0)
								{
									foreach (User_Permissioned_Aggregation dictionaryAggregation in aggregations.Values)
									{
										editUser.Add_Aggregation(dictionaryAggregation.Code, dictionaryAggregation.Name, dictionaryAggregation.CanSelect, dictionaryAggregation.CanEditMetadata, dictionaryAggregation.CanEditBehaviors, dictionaryAggregation.CanPerformQc, dictionaryAggregation.CanUploadFiles, dictionaryAggregation.CanChangeVisibility, dictionaryAggregation.CanDelete, dictionaryAggregation.IsCurator, dictionaryAggregation.OnHomePage, dictionaryAggregation.IsAdmin, false);
									}
								}
							}
							break;
					}

					// Should this be saved to the database?
					if (action == "save")
					{
						// Save this RequestSpecificValues.Current_User
                        SobekCM_Database.Save_User(editUser, String.Empty, RequestSpecificValues.Current_User.Authentication_Type, RequestSpecificValues.Tracer);

						// Update the basic RequestSpecificValues.Current_User information
                        SobekCM_Database.Update_SobekCM_User(editUser.UserID, editUser.Can_Submit, editUser.Is_Internal_User, editUser.Should_Be_Able_To_Edit_All_Items, editUser.Can_Delete_All, editUser.Is_System_Admin, editUser.Is_Portal_Admin, editUser.Include_Tracking_In_Standard_Forms, editUser.Edit_Template_Code_Simple, editUser.Edit_Template_Code_Complex, true, true, true, RequestSpecificValues.Tracer);

						// Update projects, if necessary
                        if (editUser.Default_Metadata_Sets.Count > 0)
						{
                            if (!SobekCM_Database.Update_SobekCM_User_DefaultMetadata(editUser.UserID, editUser.Default_Metadata_Sets, RequestSpecificValues.Tracer))
							{
								successful_save = false;
							}
						}

						// Update templates, if necessary
						if (editUser.Templates_Count > 0)
						{
                            if (!SobekCM_Database.Update_SobekCM_User_Templates(editUser.UserID, editUser.Templates, RequestSpecificValues.Tracer))
							{
								successful_save = false;
							}
						}

						// Save the aggregationPermissions linked to this RequestSpecificValues.Current_User
					    if (editUser.PermissionedAggregations_Count > 0)
					    {
					        if (!SobekCM_Database.Update_SobekCM_User_Aggregations(editUser.UserID, editUser.PermissionedAggregations, RequestSpecificValues.Tracer))
					        {
					            successful_save = false;
					        }
					    }

					    // Save the RequestSpecificValues.Current_User group links
						List<User_Group> userGroup = Engine_Database.Get_All_User_Groups(RequestSpecificValues.Tracer);
						Dictionary<string, int> groupnames_to_id = new Dictionary<string, int>();
						foreach (User_Group thisRow in userGroup)
						{
							groupnames_to_id[thisRow.Name] = Convert.ToInt32(thisRow.UserGroupID);
						}
						foreach (string userGroupName in editUser.User_Groups)
						{
							SobekCM_Database.Link_User_To_User_Group(editUser.UserID, groupnames_to_id[userGroupName]);
						}

						// Forward back to the list of users, if this was successful
						if (successful_save)
						{
							// Clear the RequestSpecificValues.Current_User from the sessions
							HttpContext.Current.Session["Edit_User_" + editUser.UserID] = null;

							// Redirect the RequestSpecificValues.Current_User
							RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
							UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
						}
					}
					else
					{
						// Save to the admins session
						HttpContext.Current.Session["Edit_User_" + editUser.UserID] = editUser;
						RequestSpecificValues.Current_Mode.My_Sobek_SubMode = action;
						UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
					}
				}
			}
		}

		#endregion

        ///// <summary> Property indicates the standard navigation to be included at the top of the page by the
        ///// main MySobek html subwriter. </summary>
        ///// <value> This returns either NONE or ADMIN, depending on whether an individual RequestSpecificValues.Current_User is being edited
        ///// or if the list is being viewed. </value>
        ///// <remarks> This is set to NONE if the viewer will write its own navigation and ADMIN if the standard
        ///// administrative tabs should be included as well.  </remarks>
        //public override MySobek_Included_Navigation_Enum Standard_Navigation_Type
        //{
        //    get
        //    {
        //        if ((RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Length == 0) || (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.IndexOf("v") > 0))
        //            return MySobek_Included_Navigation_Enum.System_Admin;
        //        else                        
        //            return MySobek_Included_Navigation_Enum.NONE;
        //    }
        //}

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns various values depending on the current submode </value>
        public override string Web_Title
        {
            get 
            {
                switch (mode)
                {
                    case Users_Admin_Mode_Enum.List_Users_And_Groups:
                        return "Registered Users and Groups";

                    case Users_Admin_Mode_Enum.Edit_User:
                        return "Edit User";

                    case Users_Admin_Mode_Enum.View_User:
                        return "View User Information";
                }
                return "Registered Users and Groups";
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Users_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
		public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Users_AdminViewer.Write_ItemNavForm_Closing", "Add hidden field");

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_user_reset\" name=\"admin_user_reset\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_user_save\" name=\"admin_user_save\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_user_group_delete\" name=\"admin_user_group_delete\" value=\"\" />");
            Output.WriteLine();

            Tracer.Add_Trace("Users_AdminViewer.Write_ItemNavForm_Closing", "Add the rest of the form");

            Output.WriteLine("<!-- Users_AdminViewer.Write_ItemNavForm_Closing -->");
            Output.WriteLine("<script src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");


            // Is this for a single RequestSpecificValues.Current_User edit more, or to list all the users
            switch (mode)
            {
                case Users_Admin_Mode_Enum.List_Users_And_Groups:
                    Write_User_User_Groups_List(Output, Tracer);
                    break;

                case Users_Admin_Mode_Enum.Edit_User:
                    Write_Edit_User_Form(Output, Tracer);
                    break;

                case Users_Admin_Mode_Enum.View_User:
                    Write_View_User_Form(Output);
                    break;
            }
        }

        private void Write_View_User_Form(TextWriter Output)
        {
            Output.WriteLine("<div class=\"SobekHomeText\">");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <blockquote>");
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
            Output.WriteLine("    <a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Return to user list</a><br /><br />");
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = editUser.UserID.ToString();
            Output.WriteLine("    <a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Edit this user</a>");
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = editUser.UserID.ToString() + "v";
            Output.WriteLine("  </blockquote>");

            Output.WriteLine("  <span class=\"SobekAdminTitle\">Basic Information</span>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <blockquote>");
            Output.WriteLine("  <table cellpadding=\"4px\" >");
            if (editUser.ShibbID.Trim().Length > 0)
                Output.WriteLine("  <tr valign=\"top\"><td><b>UFID:</b></td><td>" + editUser.ShibbID + "</td></tr>");
            Output.WriteLine("  <tr valign=\"top\"><td><b>UserName:</b></td><td>" + editUser.UserName + "</td></tr>");
            Output.WriteLine("  <tr valign=\"top\"><td><b>Email:</b></td><td>" + editUser.Email + "</td></tr>");
            Output.WriteLine("  <tr valign=\"top\"><td><b>Full Name:</b></td><td>" + editUser.Full_Name + "</td></tr>");

            // Build the rights statement
            StringBuilder text_builder = new StringBuilder();
            if (editUser.Can_Submit)
                text_builder.Append("Can submit items<br />");
            if (editUser.Is_Internal_User)
                text_builder.Append("Is power user<br />");            
            if (editUser.Editable_Regular_Expressions.Any(ThisRegularExpression => ThisRegularExpression == "[A-Z]{2}[A-Z|0-9]{4}[0-9]{4}"))
            {
                text_builder.Append("Can edit all items<br />");
            }
            if (editUser.Can_Delete_All)
                text_builder.Append("Can delete all items<br />");
            if (editUser.Is_Portal_Admin)
                text_builder.Append("Is portal administrator<br />");

            if (editUser.Is_System_Admin)
                text_builder.Append("Is system administrator<br />");

            if (editUser.Include_Tracking_In_Standard_Forms)
                text_builder.Append("Tracking data should be included in standard input forms<br />");

            if (text_builder.Length == 0)
            {
                Output.WriteLine("  <tr valign=\"top\"><td><b>Global Permissions:</b></td><td><i>none</i></td></tr>");
            }
            else
            {
                Output.WriteLine("  <tr valign=\"top\"><td><b>Global Permissions:</b></td><td>" + text_builder + "</td></tr>");
                text_builder.Remove(0, text_builder.Length);
            }

            Output.WriteLine("  <tr valign=\"top\"><td><b>Edit Templates:</b></td><td>" + editUser.Edit_Template_Code_Complex + "<br />" + editUser.Edit_Template_Code_Simple + "</td></tr>");

            // Build the templates list
            List<string> addedtemplates = new List<string>();
            foreach (string thisTemplate in editUser.Templates)
            {
                if (!addedtemplates.Contains(thisTemplate))
                {
                    text_builder.Append(thisTemplate + "<br />");
                    addedtemplates.Add(thisTemplate);
                }
            }
            if (text_builder.Length == 0)
            {
                Output.WriteLine("  <tr valign=\"top\"><td><b>Templates:</b></td><td><i>none</i></td></tr>");
            }
            else
            {
                Output.WriteLine("  <tr valign=\"top\"><td><b>Templates:</b></td><td>" + text_builder + "</td></tr>");
                text_builder.Remove(0, text_builder.Length);
            }

            // Build the projects list
            List<string> addedprojects = new List<string>();
            foreach (string thisProject in editUser.Default_Metadata_Sets)
            {
                if (!addedprojects.Contains(thisProject))
                {
                    text_builder.Append(thisProject + "<br />");
                    addedprojects.Add(thisProject);
                }
            }
            if (text_builder.Length == 0)
            {
                Output.WriteLine("  <tr valign=\"top\"><td><b>Default Metadata:</b></td><td><i>none</i></td></tr>");
            }
            else
            {
                Output.WriteLine("  <tr valign=\"top\"><td><b>Default Metadata:</b></td><td>" + text_builder + "</td></tr>");
                text_builder.Remove(0, text_builder.Length);
            }


            Output.WriteLine("  </table>");
            Output.WriteLine("  </blockquote>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <span class=\"SobekAdminTitle\">Group Membership</span>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <blockquote>");
            if (editUser.User_Groups.Count == 0)
            {
                Output.WriteLine("<i> &nbsp;This user is not a member of any user groups</i>");
            }
            else
            {
                foreach (string userGroup in editUser.User_Groups)
                {
                    text_builder.Append(userGroup + "<br />");
                }
                Output.WriteLine("  <table cellpadding=\"4px\" >");
                Output.WriteLine("  <tr valign=\"top\"><td><b>User Groups:</b></td><td>" + text_builder + "</td></tr>");
                Output.WriteLine("  </table>");
            }
            
            Output.WriteLine("  </blockquote>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <span class=\"SobekAdminTitle\">Aggregations</span>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <blockquote>");
            if (editUser.PermissionedAggregations==null || editUser.PermissionedAggregations.Count == 0)
            {

                Output.WriteLine("<i> &nbsp;No special aggregation rights are assigned to this user</i>");

            }
            else
            {
                Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");

                // Is this using detailed permissions?
                bool detailedPermissions = UI_ApplicationCache_Gateway.Settings.Detailed_User_Aggregation_Permissions;

                // Dertermine the number of columns
                int columns = 7;
                if (detailedPermissions)
                    columns = 12;

                // Get the list of collections lists in the RequestSpecificValues.Current_User object
                List<User_Permissioned_Aggregation> aggregations_in_editable_user = editUser.PermissionedAggregations;
                Dictionary<string, User_Permissioned_Aggregation> lookup_aggs = new Dictionary<string, User_Permissioned_Aggregation>();
                foreach (User_Permissioned_Aggregation thisAggr in aggregations_in_editable_user)
                {
                    if (!lookup_aggs.ContainsKey(thisAggr.Code.ToLower()))
                        lookup_aggs[thisAggr.Code.ToLower()] = thisAggr;
                    else
                    {
                        User_Permissioned_Aggregation current = lookup_aggs[thisAggr.Code.ToLower()];
                        if (thisAggr.CanChangeVisibility) current.CanChangeVisibility = true;
                        if (thisAggr.CanDelete) current.CanDelete = true;
                        if (thisAggr.CanEditBehaviors) current.CanEditBehaviors = true;
                        if (thisAggr.CanEditItems) current.CanEditItems = true;
                        if (thisAggr.CanEditMetadata) current.CanEditMetadata = true;
                        if (thisAggr.CanPerformQc) current.CanPerformQc = true;
                        if (thisAggr.CanSelect) current.CanSelect = true;
                        if (thisAggr.CanUploadFiles) current.CanUploadFiles = true;
                        if (thisAggr.IsAdmin) current.IsAdmin = true;
                        if (thisAggr.IsCurator) current.IsCurator = true;
                        if (thisAggr.OnHomePage) current.OnHomePage = true;
                    }
                }

                // Step through each aggregation type
                foreach (string aggregationType in UI_ApplicationCache_Gateway.Aggregations.All_Types)
                {
                    bool type_label_drawn = false;

                    // Show all matching rows
                    foreach (Item_Aggregation_Related_Aggregations thisAggr in UI_ApplicationCache_Gateway.Aggregations.Aggregations_By_Type(aggregationType))
                    {

                        if (lookup_aggs.ContainsKey(thisAggr.Code.ToLower()))
                        {
                            User_Permissioned_Aggregation aggrPermissions = lookup_aggs[thisAggr.Code.ToLower()];

                            if (!type_label_drawn)
                            {
                                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                                if ((aggregationType.Length > 0) && (aggregationType[aggregationType.Length - 1] != 'S'))
                                {
                                    Output.WriteLine("    <td colspan=\"" + columns + "\"><span style=\"color: White\"><b>" + aggregationType.ToUpper() + "S</b></span></td>");
                                }
                                else
                                {
                                    Output.WriteLine("    <td colspan=\"" + columns + "\"><span style=\"color: White\"><b>" + aggregationType.ToUpper() + "</b></span></td>");
                                }
                                Output.WriteLine("  </tr>");

                                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" >");
                                Output.WriteLine("    <td width=\"55px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Is on user's custom home page\">ON<br />HOME</acronym></span></td>");
                                Output.WriteLine("    <td width=\"57px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can select this aggregation when editing or submitting an item\">CAN<br />SELECT</acronym></span></td>");

                                if (detailedPermissions)
                                {
                                    Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">ITEM<br />EDIT<br />METADATA</acronym></span></td>");
                                    Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">ITEM<br />EDIT<br />BEHAVIORS</acronym></span></td>");
                                    Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">ITEM<br />PERFORM<br />QC</acronym></span></td>");
                                    Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">ITEM<br />UPLOAD<br />FILES</acronym></span></td>");
                                    Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">ITEM<br />CHANGE<br />VISIBILITY</acronym></span></td>");
                                    Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">ITEM<br />CAN<br />DELETE</acronym></span></td>");

                                }
                                else
                                {
                                    Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit any item in this aggregation\">CAN<br />EDIT</acronym></span></td>");
                                }

                                Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can perform curatorial or collection manager tasks on this aggregation\">IS<br />CURATOR</acronym></span></td>");
                                Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can perform curatorial or collection manager tasks on this aggregation\">IS<br />ADMIN</acronym></span></td>");
                                Output.WriteLine("    <td align=\"left\" colspan=\"2\"><span style=\"color: White\">ITEM AGGREGATION</span></td>");
                                Output.WriteLine("   </tr>");

                                type_label_drawn = true;
                            }

                            Output.WriteLine("  <tr align=\"left\" >");
                            Output.WriteLine(aggrPermissions.OnHomePage
                                                 ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                                 : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                            Output.WriteLine(aggrPermissions.CanSelect
                                                 ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                                 : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");


                            if (detailedPermissions)
                            {
                                Output.WriteLine(aggrPermissions.CanEditMetadata
                                    ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                    : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                                Output.WriteLine(aggrPermissions.CanEditBehaviors
                                    ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                    : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                                Output.WriteLine(aggrPermissions.CanPerformQc
                                    ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                    : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                                Output.WriteLine(aggrPermissions.CanUploadFiles
                                    ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                    : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                                Output.WriteLine(aggrPermissions.CanChangeVisibility
                                    ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                    : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                                Output.WriteLine(aggrPermissions.CanDelete
                                    ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                    : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                            }
                            else
                            {
                                Output.WriteLine(aggrPermissions.CanEditItems
                                    ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                    : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");
                            }

                            Output.WriteLine(aggrPermissions.IsCurator
                                                 ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                                 : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                            Output.WriteLine(aggrPermissions.IsAdmin
                                                ? "    <td><input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" /></td>"
                                                : "    <td><input type=\"checkbox\" disabled=\"disabled\" /></td>");

                            Output.WriteLine("    <td>" + thisAggr.Code + "</td>");
                            Output.WriteLine("    <td>" + thisAggr.Name + "</td>");
                            Output.WriteLine("   </tr>");
                            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"" + columns + "\"></td></tr>");
                        }
                    }
                }

                Output.WriteLine("</table>");
                Output.WriteLine("<br />");

            }
            Output.WriteLine("  </blockquote>");
            Output.WriteLine("</div>");
        }

        private void Write_Edit_User_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            int page = 1;
            if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.IndexOf("b") > 0)
                page = 2;
            else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.IndexOf("c") > 0)
                page = 3;

            Output.WriteLine("  <div class=\"SobekHomeText\">");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <b>Edit this users's permissions, abilities, and basic information</b>");
            Output.WriteLine("    <ul>");
            Output.WriteLine("      <li>Enter the permissions for this user below and press the SAVE button when all your edits are complete.</li>");
            Output.WriteLine("      <li>For clarification of any terms on this form, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/users\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</li>");
            Output.WriteLine("     </ul>");
            Output.WriteLine("  </div>");

            // Start the outer tab containe
            Output.WriteLine("  <div id=\"tabContainer\" class=\"fulltabs\">");
            Output.WriteLine("  <div class=\"tabs\">");
			Output.WriteLine("    <ul>");
            string last_mode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Replace("b", "").Replace("c", "");
            if (page == 1)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> BASIC INFORMATION </li>");
            }
            else
            {
                Output.WriteLine("      <li onclick=\"return new_user_edit_page('" + editUser.UserID + "a" + "');\">" + " BASIC INFORMATION " + "</li>");
            }

            if (page == 2)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> GROUP MEMBERSHIP </li>");
            }
            else
            {
                Output.WriteLine("    <li onclick=\"return new_user_edit_page('" + editUser.UserID + "b" + "');\">" + " GROUP MEMBERSHIP " + "</li>");
            }

            if (page == 3)
            {
                Output.WriteLine("      <li class=\"tabActiveHeader\"> AGGREGATIONS </li>");
            }
            else
            {
                Output.WriteLine("    <li onclick=\"return new_user_edit_page('" + editUser.UserID + "c" + "');\">" + " AGGREGATIONS " + "</li>");


                //RequestSpecificValues.Current_Mode.My_Sobek_SubMode = edit_user.UserID + "c";
                //Output.WriteLine("    <a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + base.Unselected_Tab_Start + " AGGREGATIONS " + base.Unselected_Tab_End + "</a>");
            }
			Output.WriteLine("    </ul>");
            Output.WriteLine("  </div>");

            Output.WriteLine("    <div class=\"tabscontent\">");
            Output.WriteLine("    	<div class=\"sbkUgav_TabPage\" id=\"tabpage_1\">");

            // Add the buttons
			Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\">");
            Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return cancel_user_edits();return false;\"><img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
			Output.WriteLine("    <button title=\"Save changes to this user group\" class=\"sbkAdm_RoundButton\" onclick=\"return save_user_edits();return false;\">SAVE <img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
			Output.WriteLine("  </div>");
			Output.WriteLine();

			Output.WriteLine("  <br /><br />");
			Output.WriteLine();

            switch (page)
            {
                case 1:
                    Output.WriteLine("  <span class=\"SobekEditItemSectionTitle_first\"> &nbsp; User Information</span>");
                    Output.WriteLine("  <blockquote>");
                    Output.WriteLine("    <table>");

                    if (editUser.ShibbID.Trim().Length > 0)
                    {
                        if (editUser.ShibbID.Length > 4)
                        {
                            Output.Write("      <tr height=\"27px\"><td width=\"80px\">UFID:</td><td width=\"200px\"><span class=\"form_linkline\">" + editUser.ShibbID.Substring(0, 4) + "-" + editUser.ShibbID.Substring(4) + " &nbsp; &nbsp; </span></td>");
                        }
                        else
                        {
                            Output.Write("      <tr height=\"27px\"><td width=\"80px\">UFID:</td><td width=\"200px\"><span class=\"form_linkline\">" + editUser.ShibbID + " &nbsp; &nbsp; </span></td>");
                        }
                    }
                    else
                    {
                        Output.Write("      <tr height=\"27px\"><td width=\"80px\">&nbsp</td><td width=\"200px\">&nbsp;</span></td>");
                    }

                    Output.WriteLine("<td width=\"80\">Email:</td><td><span class=\"form_linkline\">" + editUser.Email + " &nbsp; &nbsp; </span></td></tr>");
                    Output.WriteLine("      <tr height=\"27px\"><td>UserName:</td><td><span class=\"form_linkline\">" + editUser.UserName + " &nbsp; &nbsp; </span></td><td>Full Name:</td><td><span class=\"form_linkline\">" + editUser.Full_Name + " &nbsp; &nbsp; </span></td></tr>");
                    Output.WriteLine("    </table>");
                    Output.WriteLine("  </blockquote>");

                    Output.WriteLine("  <span class=\"SobekEditItemSectionTitle\"> &nbsp; Current Affiliation Information</span><br />");
                    Output.WriteLine("  <blockquote>");
                    Output.WriteLine("    <table>");
                    Output.WriteLine("      <tr height=\"27px\"><td width=\"80px\"><label for=\"admin_user_organization\">Organization/University:</label></td><td><input id=\"admin_user_organization\" name=\"admin_user_organization\" class=\"users_large_input\" value=\"" + editUser.Organization + "\" type=\"text\" onfocus=\"javascript:textbox_enter('admin_user_organization', 'users_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_user_organization', 'users_large_input')\" /></td></tr>");
                    Output.WriteLine("      <tr height=\"27px\"><td width=\"80px\"><label for=\"admin_user_college\">College:</label></td><td><input id=\"admin_user_college\" name=\"admin_user_college\" class=\"users_large_input\" value=\"" + editUser.College + "\"type=\"text\" onfocus=\"javascript:textbox_enter('admin_user_college', 'users_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_user_college', 'users_large_input')\" /></td></tr>");
                    Output.WriteLine("      <tr height=\"27px\"><td width=\"80px\"><label for=\"admin_user_department\">Department:</label></td><td><input id=\"admin_user_department\" name=\"admin_user_department\" class=\"users_large_input\" value=\"" + editUser.Department + "\"type=\"text\" onfocus=\"javascript:textbox_enter('admin_user_department', 'users_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_user_department', 'users_large_input')\" /></td></tr>");
                    Output.WriteLine("      <tr height=\"27px\"><td width=\"80px\"><label for=\"admin_user_unit\">Unit:</label></td><td><input id=\"admin_user_unit\" name=\"admin_user_unit\" class=\"users_large_input\" value=\"" + editUser.Unit + "\" type=\"text\" onfocus=\"javascript:textbox_enter('admin_user_unit', 'users_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_user_unit', 'users_large_input')\" /></td></tr>");
                    Output.WriteLine("      <tr height=\"27px\"><td width=\"80px\"><label for=\"admin_user_org_code\">Code:</label></td><td><input id=\"admin_user_org_code\" name=\"admin_user_org_code\" class=\"users_code_input\" value=\"" + editUser.Organization_Code + "\" type=\"text\" onfocus=\"javascript:textbox_enter('admin_user_org_code', 'users_code_input_focused')\" onblur=\"javascript:textbox_leave('admin_user_org_code', 'users_code_input')\" /></td></tr>");
                    Output.WriteLine("    </table>");
                    Output.WriteLine("  </blockquote>");


                    Output.WriteLine("  <span class=\"SobekEditItemSectionTitle\"> &nbsp; Global Permissions</span><br />");
                    Output.WriteLine(editUser.Can_Submit
                                         ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_submit\" id=\"admin_user_submit\" checked=\"checked\" /> <label for=\"admin_user_submit\">Can submit items</label> <br />"
                                         : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_submit\" id=\"admin_user_submit\" /> <label for=\"admin_user_submit\">Can submit items</label> <br />");

                    Output.WriteLine(editUser.Is_Internal_User
                                         ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_internal\" id=\"admin_user_internal\" checked=\"checked\" /> <label for=\"admin_user_internal\">Is power user</label> <br />"
                                         : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_internal\" id=\"admin_user_internal\" /> <label for=\"admin_user_internal\">Is power user</label> <br />");

                   // bool canEditAll = editUser.Editable_Regular_Expressions.Any(thisRegularExpression => thisRegularExpression == "[A-Z]{2}[A-Z|0-9]{4}[0-9]{4}");
                    Output.WriteLine(editUser.Should_Be_Able_To_Edit_All_Items
                                         ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_editall\" id=\"admin_user_editall\" checked=\"checked\" /> <label for=\"admin_user_editall\">Can edit <u>all</u> items</label> <br />"
                                         : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_editall\" id=\"admin_user_editall\" /> <label for=\"admin_user_editall\">Can edit <u>all</u> items</label> <br />");

                    Output.WriteLine(editUser.Can_Delete_All
					 ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_deleteall\" id=\"admin_user_deleteall\" checked=\"checked\" /> <label for=\"admin_user_deleteall\">Can delete <u>all</u> items</label> <br />"
                     : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_deleteall\" id=\"admin_user_deleteall\" /> <label for=\"admin_user_deleteall\">Can delete <u>all</u> items</label> <br />");


                    Output.WriteLine(editUser.Is_Portal_Admin
                                         ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_portaladmin\" id=\"admin_user_portaladmin\" checked=\"checked\" /> <label for=\"admin_user_portaladmin\">Is portal administrator</label> <br />"
                                         : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_portaladmin\" id=\"admin_user_portaladmin\" /> <label for=\"admin_user_portaladmin\">Is portal administrator</label> <br />");

                    Output.WriteLine(editUser.Is_System_Admin
                                         ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_sysadmin\" id=\"admin_user_sysadmin\" checked=\"checked\" /> <label for=\"admin_user_sysadmin\">Is system administrator</label> <br />"
                                         : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_sysadmin\" id=\"admin_user_sysadmin\" /> <label for=\"admin_user_sysadmin\">Is system administrator</label> <br />");

                    Output.WriteLine(editUser.Include_Tracking_In_Standard_Forms
                                         ? "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_includetracking\" id=\"admin_user_includetracking\" checked=\"checked\" /> <label for=\"admin_user_includetracking\">Tracking data should be included in standard input forms</label> <br />"
                                         : "    <input class=\"admin_user_checkbox\" type=\"checkbox\" name=\"admin_user_includetracking\" id=\"admin_user_includetracking\" /> <label for=\"admin_user_includetracking\">Tracking data should be included in standard input forms</label> <br />");

                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <span class=\"SobekEditItemSectionTitle\"> &nbsp; Templates and Default Metadata</span>");
                    Output.WriteLine("  <blockquote>");
                    Output.WriteLine("    <table>");
                    Output.WriteLine("      <tr height=\"35px\" valign=\"top\" >");
                    Output.WriteLine("        <td width=\"300px\">");
                    Output.WriteLine("          Edit Templates: &nbsp; ");
                    Output.WriteLine("          <select class=\"admin_user_select\" name=\"admin_user_edittemplate\" id=\"admin_user_edittemplate\">");

                    if (editUser.Edit_Template_Code_Simple.ToUpper().IndexOf("INTERNAL") >= 0)
                    {
                        Output.WriteLine("            <option value=\"internal\" selected=\"selected\">Internal</option>");
                        Output.WriteLine("            <option value=\"standard\">Standard</option>");
                    }
                    else
                    {
                        Output.WriteLine("            <option value=\"internal\">Internal</option>");
                        Output.WriteLine("            <option value=\"standard\" selected=\"selected\">Standard</option>");
                    }

                    Output.WriteLine("          </select>");
                    Output.WriteLine("        </td>");
                    Output.WriteLine("        <td> &nbsp; </td>");
                    Output.WriteLine("      </tr>");

                    DataSet projectTemplateSet = Engine_Database.Get_All_Template_DefaultMetadatas(Tracer);

                    Output.WriteLine("      <tr valign=\"top\" >");
                    Output.WriteLine("        <td>");
                    Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");
                    Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                    Output.WriteLine("    <th width=\"180px\" align=\"left\"><span style=\"color: White\">TEMPLATES</span></th>");
                    Output.WriteLine("   </tr>");
                    Output.WriteLine("  <tr ><td bgcolor=\"#e7e7e7\"></td></tr>");

                    ReadOnlyCollection<string> user_templates = editUser.Templates;
                    foreach (DataRow thisTemplate in projectTemplateSet.Tables[1].Rows)
                    {
                        string template_name = thisTemplate["TemplateName"].ToString();
                        string template_code = thisTemplate["TemplateCode"].ToString();

                        Output.Write("  <tr align=\"left\"><td><input type=\"checkbox\" name=\"admin_user_template_" + template_code + "\" id=\"admin_user_template_" + template_code + "\"");
                        if (user_templates.Contains(template_code))
                        {
                            Output.Write(" checked=\"checked\"");
                        }
                        if (template_name.Length > 0)
                        {
                            Output.WriteLine(" /> &nbsp; <acronym title=\"" + template_name.Replace("\"", "'") + "\"><label for=\"admin_user_template_" + template_code + "\">" + template_code + "</label></acronym></td></tr>");
                        }
                        else
                        {
                            Output.WriteLine(" /> &nbsp; <label for=\"admin_user_template_" + template_code + "\">" + template_code + "</label></td></tr>");
                        }
                        Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\"></td></tr>");
                    }
                    Output.WriteLine("</table>");
                    Output.WriteLine("        </td>");

                    Output.WriteLine("        <td>");
                    Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");
                    Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                    Output.WriteLine("    <th width=\"180px\" align=\"left\"><span style=\"color: White\">DEFAULT METADATA</span></th>");
                    Output.WriteLine("   </tr>");
                    Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\"></td></tr>");

                    ReadOnlyCollection<string> user_projects = editUser.Default_Metadata_Sets;
                    foreach (DataRow thisProject in projectTemplateSet.Tables[0].Rows)
                    {
                        string project_name = thisProject["MetadataName"].ToString();
                        string project_code = thisProject["MetadataCode"].ToString();

                        Output.Write("  <tr align=\"left\"><td><input type=\"checkbox\" name=\"admin_user_project_" + project_code + "\" id=\"admin_user_project_" + project_code + "\"");
                        if (user_projects.Contains(project_code))
                        {
                            Output.Write(" checked=\"checked\"");
                        }
                        if (project_name.Length > 0)
                        {
                            Output.WriteLine(" /> &nbsp; <acronym title=\"" + project_name.Replace("\"", "'") + "\"><label for=\"admin_user_project_" + project_code + "\">" + project_code + "</label></acronym></td></tr>");
                        }
                        else
                        {
                            Output.WriteLine(" /> &nbsp; <label for=\"admin_user_project_" + project_code + "\">" + project_code + "</label></td></tr>");
                        }

                        Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\"></td></tr>");
                    }
                    Output.WriteLine("</table>");
                    Output.WriteLine("        </td>");

                    Output.WriteLine("      </tr>");
                    Output.WriteLine("   </table>");
                    Output.WriteLine("  </blockquote>");
                    break;

                case 2:
                    Output.WriteLine("  <span class=\"SobekEditItemSectionTitle_first\"> &nbsp; User Group Membership</span>");
                    Output.WriteLine("  <blockquote>");

                    List<User_Group> userGroup = Engine_Database.Get_All_User_Groups(Tracer);
                    if ((userGroup == null) || (userGroup.Count == 0))
                    {
                        Output.WriteLine("<br />");
                        Output.WriteLine("<b>No user groups exist within this library instance</b>");
                        Output.WriteLine("<br />");
                    }
                    else
                    {
                        Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");
                        Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                        Output.WriteLine("    <td colspan=\"3\"><span style=\"color: White\"><b>USER GROUPS</b></span></td>");
                        Output.WriteLine("   </tr>");
                        //Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" >");
                        //Output.WriteLine("    <td width=\"100px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Is this RequestSpecificValues.Current_User a member of this group?\">IS MEMBER</acronym></span></td>");
                        //Output.WriteLine("    <td width=\"120px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Name of this RequestSpecificValues.Current_User group\">GROUP NAME</acronym></span></td>");
                        //Output.WriteLine("    <td width=\"300px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Description of this RequestSpecificValues.Current_User group\">GROUP DESCRIPTION</acronym></span></td>");
                        //Output.WriteLine("   </tr>");

                        foreach (User_Group thisRow in userGroup)
                        {
                            Output.WriteLine("  <tr align=\"left\" >");                         

                            Output.Write("    <td width=\"50px\" ><input type=\"checkbox\" name=\"group_" + thisRow.UserGroupID + "\" id=\"group_" + thisRow.UserGroupID + "\" ");
                            if ( editUser.User_Groups.Contains( thisRow.Name ))
                                Output.Write(" checked=\"checked\"");
                            Output.WriteLine("/></td>");
                            Output.WriteLine("    <td width=\"150px\" >" + thisRow.Name + "</td>");
                            Output.WriteLine("    <td width=\"400px\">" + thisRow.Description + "</td>");
                            Output.WriteLine("  </tr>");
                            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");
                        }

                        Output.WriteLine("</table>");
                    }

                    Output.WriteLine("  </blockquote>");
                    break;

                case 3:
                    Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");

                    // Get the list of collections lists in the RequestSpecificValues.Current_User object
                    List<User_Permissioned_Aggregation> aggregations_in_editable_user = editUser.PermissionedAggregations;

                    Dictionary<string, List<User_Permissioned_Aggregation>> lookup_aggs = new Dictionary<string, List<User_Permissioned_Aggregation>>();
                    if(aggregations_in_editable_user!=null)
                     foreach (User_Permissioned_Aggregation thisAggr in aggregations_in_editable_user)
					{
						if (lookup_aggs.ContainsKey(thisAggr.Code.ToLower()))
						{
							lookup_aggs[thisAggr.Code.ToLower()].Add(thisAggr);
						}
						else
						{
							List<User_Permissioned_Aggregation> thisAggrList = new List<User_Permissioned_Aggregation>();
							thisAggrList.Add(thisAggr);
							lookup_aggs[thisAggr.Code.ToLower()] = thisAggrList;
						}
					}

					// Determine if this is a detailed view of rights
		            int columns = 8;
					if (UI_ApplicationCache_Gateway.Settings.Detailed_User_Aggregation_Permissions)
					{
						columns = 13;
					}

                    // Step through each aggregation type
                    foreach (string aggregationType in UI_ApplicationCache_Gateway.Aggregations.All_Types)
                    {
                        Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                        if ((aggregationType.Length > 0) && (aggregationType[aggregationType.Length - 1] != 'S'))
                        {
                            Output.WriteLine("    <td colspan=\"" + columns + "\"><span style=\"color: White\"><b>" + aggregationType.ToUpper() + "S</b></span></td>");
                        }
                        else
                        {
							Output.WriteLine("    <td colspan=\"" + columns + "\"><span style=\"color: White\"><b>" + aggregationType.ToUpper() + "</b></span></td>");
                        }
                        Output.WriteLine("  </tr>");

                        Output.WriteLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" >");
                        Output.WriteLine("    <td width=\"55px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Is on user's custom home page\">ON<br />HOME</acronym></span></td>");
                        Output.WriteLine("    <td width=\"57px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can select this aggregation when editing or submitting an item\">CAN<br />SELECT</acronym></span></td>");

						if (UI_ApplicationCache_Gateway.Settings.Detailed_User_Aggregation_Permissions)
						{
							Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">ITEM<br />EDIT<br />METADATA</acronym></span></td>");
							Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">ITEM<br />EDIT<br />BEHAVIORS</acronym></span></td>");
							Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">ITEM<br />PERFORM<br />QC</acronym></span></td>");
							Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">ITEM<br />UPLOAD<br />FILES</acronym></span></td>");
							Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">ITEM<br />CHANGE<br />VISIBILITY</acronym></span></td>");
							Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">ITEM<br />CAN<br />DELETE</acronym></span></td>");
						}
						else
						{
							Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can edit anything about an item in this aggregation ( i.e., behaviors, metadata, visibility, etc.. )\">CAN<br />EDIT</acronym></span></td>");
						}




                        Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can perform curatorial or collection manager tasks on this aggregation\">IS<br />CURATOR</acronym></span></td>");
						Output.WriteLine("    <td width=\"50px\" align=\"left\"><span style=\"color: White\"><acronym title=\"Can perform administrative tasks on this aggregation\">IS<br />ADMIN</acronym></span></td>");
                        Output.WriteLine("    <td align=\"left\" colspan=\"2\"><span style=\"color: White\">ITEM AGGREGATION</span></td>");
                        Output.WriteLine("   </tr>");

                        // Show all matching rows
                        foreach (Item_Aggregation_Related_Aggregations thisAggr in UI_ApplicationCache_Gateway.Aggregations.Aggregations_By_Type(aggregationType))
                        {
                            Output.WriteLine("  <tr align=\"left\" >");
                            if (!lookup_aggs.ContainsKey(thisAggr.Code.ToLower()))
                            {
                                Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_onhome_" + thisAggr.Code + "\" id=\"admin_project_onhome_" + thisAggr.Code + "\" /></td>");
                                Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_select_" + thisAggr.Code + "\" id=\"admin_project_select_" + thisAggr.Code + "\" /></td>");

								if (UI_ApplicationCache_Gateway.Settings.Detailed_User_Aggregation_Permissions)
								{
									Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_edit_metadata_" + thisAggr.Code + "\" id=\"admin_project_edit_metadata_" + thisAggr.Code + "\" /></td>");
									Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_edit_behavior_" + thisAggr.Code + "\" id=\"admin_project_edit_behavior_" + thisAggr.Code + "\" /></td>");
									Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_perform_qc_" + thisAggr.Code + "\" id=\"admin_project_perform_qc_" + thisAggr.Code + "\" /></td>");
									Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_upload_files_" + thisAggr.Code + "\" id=\"admin_project_upload_files_" + thisAggr.Code + "\" /></td>");
									Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_change_visibility_" + thisAggr.Code + "\" id=\"admin_project_change_visibility_" + thisAggr.Code + "\" /></td>");
									Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_can_delete_" + thisAggr.Code + "\" id=\"admin_project_can_delete_" + thisAggr.Code + "\" /></td>");
								}
								else
								{
									Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_edit_" + thisAggr.Code + "\" id=\"admin_project_edit_" + thisAggr.Code + "\" /></td>");
								}

								Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_curator_" + thisAggr.Code + "\" id=\"admin_project_curator_" + thisAggr.Code + "\" /></td>");
                                Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_admin_" + thisAggr.Code + "\" id=\"admin_project_admin_" + thisAggr.Code + "\" /></td>");
                            }
                            else
                            {
                                if (lookup_aggs[thisAggr.Code.ToLower()][0].OnHomePage)
                                    Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_onhome_" + thisAggr.Code + "\" id=\"admin_project_onhome_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
                                else
                                    Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_onhome_" + thisAggr.Code + "\" id=\"admin_project_onhome_" + thisAggr.Code + "\" /></td>");

	                            bool can_select = false;
	                            bool can_select_from_group = false;
								bool can_edit_metadata = false;
								bool can_edit_metadata_from_group = false;
								bool can_edit_behaviors = false;
								bool can_edit_behaviors_from_group = false;
								bool can_perform_qc = false;
								bool can_perform_qc_from_group = false;
								bool can_change_visibility = false;
								bool can_change_visibility_from_group = false;
								bool can_delete_item = false;
								bool can_delete_item_from_group = false;
								bool can_upload_files = false;
								bool can_upload_files_from_group = false;
								bool is_curator = false;
								bool is_curator_from_group = false;
								bool is_admin = false;
								bool is_admin_from_group = false;
								foreach (User_Permissioned_Aggregation thisAggrFromList in lookup_aggs[thisAggr.Code.ToLower()])
								{
									if (thisAggrFromList.CanSelect)
									{
										if (thisAggrFromList.GroupDefined)
										{
											can_select = true;
											can_select_from_group = true;
										}
										else
										{
											can_select = true;
										}
									}
									if (thisAggrFromList.CanEditMetadata)
									{
										if (thisAggrFromList.GroupDefined)
										{
											can_edit_metadata = true;
											can_edit_metadata_from_group = true;
										}
										else
										{
											can_edit_metadata = true;
										}
									}
									if (thisAggrFromList.CanEditBehaviors)
									{
										if (thisAggrFromList.GroupDefined)
										{
											can_edit_behaviors = true;
											can_edit_behaviors_from_group = true;
										}
										else
										{
											can_edit_behaviors = true;
										}
									}
									if (thisAggrFromList.CanChangeVisibility)
									{
										if (thisAggrFromList.GroupDefined)
										{
											can_change_visibility = true;
											can_change_visibility_from_group = true;
										}
										else
										{
											can_change_visibility = true;
										}
									}
									if (thisAggrFromList.CanDelete)
									{
										if (thisAggrFromList.GroupDefined)
										{
											can_delete_item = true;
											can_delete_item_from_group = true;
										}
										else
										{
											can_delete_item = true;
										}
									}
									if (thisAggrFromList.CanUploadFiles)
									{
										if (thisAggrFromList.GroupDefined)
										{
											can_upload_files = true;
											can_upload_files_from_group = true;
										}
										else
										{
											can_upload_files = true;
										}
									}
									if (thisAggrFromList.CanPerformQc)
									{
										if (thisAggrFromList.GroupDefined)
										{
											can_perform_qc = true;
											can_perform_qc_from_group = true;
										}
										else
										{
											can_perform_qc = true;
										}
									}
									if (thisAggrFromList.IsCurator)
									{
										if (thisAggrFromList.GroupDefined)
										{
											is_curator = true;
											is_curator_from_group = true;
										}
										else
										{
											is_curator = true;
										}
									}
									if (thisAggrFromList.IsAdmin)
									{
										if (thisAggrFromList.GroupDefined)
										{
											is_admin = true;
											is_admin_from_group = true;
										}
										else
										{
											is_admin = true;
										}
									}
								}

	                            if (can_select)
	                            {
									if ( can_select_from_group )
			                            Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_select_" + thisAggr.Code + "\" id=\"admin_project_select_" + thisAggr.Code + "\" checked=\"checked\" disabled=\"disabled\" /></td>");
									else
										Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_select_" + thisAggr.Code + "\" id=\"admin_project_select_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
	                            }
	                            else
		                            Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_select_" + thisAggr.Code + "\" id=\"admin_project_select_" + thisAggr.Code + "\" /></td>");


	                            if (UI_ApplicationCache_Gateway.Settings.Detailed_User_Aggregation_Permissions)
								{
									if (can_edit_metadata)
									{
										if ( can_edit_metadata_from_group )
											Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_edit_metadata_" + thisAggr.Code + "\" id=\"admin_project_edit_metadata_" + thisAggr.Code + "\" checked=\"checked\" disabled=\"disabled\" /></td>");
										else
											Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_edit_metadata_" + thisAggr.Code + "\" id=\"admin_project_edit_metadata_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
									}
									else
										Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_edit_metadata_" + thisAggr.Code + "\" id=\"admin_project_edit_metadata_" + thisAggr.Code + "\" /></td>");

									if (can_edit_behaviors)
									{
										if (can_edit_behaviors_from_group)
											Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_edit_behavior_" + thisAggr.Code + "\" id=\"admin_project_edit_behavior_" + thisAggr.Code + "\" checked=\"checked\" disabled=\"disabled\" /></td>");
										else
											Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_edit_behavior_" + thisAggr.Code + "\" id=\"admin_project_edit_behavior_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
									}
									else
										Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_edit_behavior_" + thisAggr.Code + "\" id=\"admin_project_edit_behavior_" + thisAggr.Code + "\" /></td>");

									if ( can_perform_qc)
									{
										if ( can_perform_qc_from_group )
											Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_perform_qc_" + thisAggr.Code + "\" id=\"admin_project_perform_qc_" + thisAggr.Code + "\" checked=\"checked\" disabled=\"disabled\" /></td>");
										else
											Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_perform_qc_" + thisAggr.Code + "\" id=\"admin_project_perform_qc_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
									}
									else
										Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_perform_qc_" + thisAggr.Code + "\" id=\"admin_project_perform_qc_" + thisAggr.Code + "\" /></td>");

									if ( can_upload_files)
									{
										if ( can_upload_files_from_group )
											Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_upload_files_" + thisAggr.Code + "\" id=\"admin_project_upload_files_" + thisAggr.Code + "\" checked=\"checked\" disabled=\"disabled\" /></td>");
										else
											Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_upload_files_" + thisAggr.Code + "\" id=\"admin_project_upload_files_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
									}
									else
										Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_upload_files_" + thisAggr.Code + "\" id=\"admin_project_upload_files_" + thisAggr.Code + "\" /></td>");

									if ( can_change_visibility)
									{
										if ( can_change_visibility_from_group )
											Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_change_visibility_" + thisAggr.Code + "\" id=\"admin_project_change_visibility_" + thisAggr.Code + "\" checked=\"checked\" disabled=\"disabled\" /></td>");
										else
											Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_change_visibility_" + thisAggr.Code + "\" id=\"admin_project_change_visibility_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
									}
									else
										Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_change_visibility_" + thisAggr.Code + "\" id=\"admin_project_change_visibility_" + thisAggr.Code + "\" /></td>");

									if ( can_delete_item )
									{
										if ( can_delete_item_from_group )
											Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_can_delete_" + thisAggr.Code + "\" id=\"admin_project_can_delete_" + thisAggr.Code + "\" checked=\"checked\" disabled=\"disabled\" /></td>"); 
										else
											Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_can_delete_" + thisAggr.Code + "\" id=\"admin_project_can_delete_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
									}
									else
										Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_can_delete_" + thisAggr.Code + "\" id=\"admin_project_can_delete_" + thisAggr.Code + "\" /></td>");
								}
								else
								{
									if (can_edit_metadata)
									{
										if ( can_edit_metadata_from_group )
											Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_editall_" + thisAggr.Code + "\" id=\"admin_project_editall_" + thisAggr.Code + "\" checked=\"checked\" disabled=\"disabled\" /></td>");
										else
											Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_editall_" + thisAggr.Code + "\" id=\"admin_project_editall_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
									}
									else
										Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_editall_" + thisAggr.Code + "\" id=\"admin_project_editall_" + thisAggr.Code + "\" /></td>");
								}

								if (is_curator)
								{
									if ( is_curator_from_group )
										Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_curator_" + thisAggr.Code + "\" id=\"admin_project_curator_" + thisAggr.Code + "\" checked=\"checked\" disabled=\"disabled\" /></td>");
									else
										Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_curator_" + thisAggr.Code + "\" id=\"admin_project_curator_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
								}
                                else
									Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_curator_" + thisAggr.Code + "\" id=\"admin_project_curator_" + thisAggr.Code + "\" /></td>");

								if (is_admin)
								{
									if ( is_admin_from_group )
										Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_admin_" + thisAggr.Code + "\" id=\"admin_project_admin_" + thisAggr.Code + "\" checked=\"checked\" disabled=\"disabled\" /></td>");	
									else
										Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_admin_" + thisAggr.Code + "\" id=\"admin_project_admin_" + thisAggr.Code + "\" checked=\"checked\" /></td>");
								}								else
									Output.WriteLine("    <td><input type=\"checkbox\" name=\"admin_project_admin_" + thisAggr.Code + "\" id=\"admin_project_admin_" + thisAggr.Code + "\" /></td>");


                            }

                            Output.WriteLine("    <td>" + thisAggr.Code + "</td>");
                            Output.WriteLine("    <td>" + thisAggr.Name + "</td>");
                            Output.WriteLine("   </tr>");
							Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"" + columns + "\"></td></tr>");
                        }
                    }

                    Output.WriteLine("</table>");
                    Output.WriteLine("<br />");
                    break;
            }


			// Add the buttons
			RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
			Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\">");
            Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return cancel_user_edits();return false;\"><img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
			Output.WriteLine("    <button title=\"Save changes to this user\" class=\"sbkAdm_RoundButton\" onclick=\"return save_user_edits();return false;\">SAVE <img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
			Output.WriteLine("  </div>");

			Output.WriteLine();
			RequestSpecificValues.Current_Mode.My_Sobek_SubMode = last_mode;

            Output.WriteLine("</div>");
            Output.WriteLine("</div>");

			Output.WriteLine("<br />");
			Output.WriteLine("<br />");

        }

        private void Write_User_User_Groups_List(TextWriter Output, Custom_Tracer Tracer)
        {

            Output.WriteLine("<div class=\"SobekHomeText\">");

            // Display the action message if there is one
            if (actionMessage.Length > 0)
            {
                Output.WriteLine("  <br />");
                Output.WriteLine("  <center><b>" + actionMessage + "</b></center>");
            }

            Output.WriteLine("  <br />");
            Output.WriteLine("  <span class=\"SobekAdminTitle\">Existing User Groups</span>");
            Output.WriteLine("  <br /><br />");

            // get the list of all RequestSpecificValues.Current_User groups
            List<User_Group> userGroup = Engine_Database.Get_All_User_Groups(Tracer);

            // Get the redirect
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "XXXXXXX";
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.User_Groups;
            string redirect = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;

            // Show the RequestSpecificValues.Current_User groups
            if ((userGroup == null) || (userGroup.Count == 0))
            {
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "new";
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.User_Groups;
                Output.WriteLine("<blockquote>No user groups exist within this library instance. <a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Click here to add a new RequestSpecificValues.Current_User group.</a></blockquote>");
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;
            }
            else
            {
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "new";
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.User_Groups;
                Output.WriteLine("  <blockquote>Select a user group to edit or view.  <a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Click here to add a new user group.</a></blockquote>");
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;

                Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");
                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
                Output.WriteLine("    <th width=\"200px\" align=\"left\"><span style=\"color: White\"> &nbsp; ACTIONS</span></th>");
                Output.WriteLine("    <th width=\"140px\" align=\"left\"><span style=\"color: White\">NAME</span></th>");
                Output.WriteLine("    <th align=\"left\"><span style=\"color: White\">DESCRIPTION</span></th>");
                Output.WriteLine("   </tr>");

                foreach (User_Group thisRow in userGroup)
                {
                    Output.WriteLine("  <tr align=\"left\" >");
                    Output.Write("    <td class=\"SobekAdminActionLink\" >( ");

                    Output.Write("<a title=\"Click to edit\" href=\"" + redirect.Replace("XXXXXXX", thisRow.UserGroupID.ToString()) + "\">edit</a> | ");
                    Output.Write("<a title=\"Click to view\" href=\"" + redirect.Replace("XXXXXXX", thisRow.UserGroupID.ToString()) + "v\">view</a>");
                    if (!thisRow.IsSpecialGroup)
                        Output.Write(" | <a title=\"Click to delete this user group entirely\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_user_group('" + thisRow.Name + "'," + thisRow.UserGroupID + ");\">delete</a> ) </td>");
                    else
                        Output.Write(" ) </td>");


                    Output.WriteLine("    <td>" + thisRow.Name + "</td>");
                    Output.WriteLine("    <td>" + thisRow.Description + "</td>");
                    Output.WriteLine("  </tr>");
                    Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");
                }

                Output.WriteLine("</table>");
                Output.WriteLine("  <br />");
            }
            Output.WriteLine("  <br />");

            // List of all users
            Output.WriteLine("  <span class=\"SobekAdminTitle\">Existing Registered Users</span>");
            Output.WriteLine("  <br />");
            Output.WriteLine("  <blockquote>Select a user to edit. Click <i>reset password</i> to email a new temporary password to the user.</blockquote>");
            // Get the list of all users
            DataTable usersTable = SobekCM_Database.Get_All_Users(Tracer);

            Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsWhiteTable\">");
            Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");
            Output.WriteLine("    <th width=\"190px\" align=\"left\"><span style=\"color: White\"> &nbsp; ACTIONS</span></th>");
            Output.WriteLine("    <th width=\"320px\" align=\"left\"><span style=\"color: White\">NAME</span></th>");
            Output.WriteLine("    <th align=\"left\"><span style=\"color: White\">EMAIL</span></th>");
            Output.WriteLine("   </tr>");
            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");

            // Get the redirect
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "XXXXXXX";
            redirect = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;

            // Write the data for each interface
            foreach (DataRow thisRow in usersTable.Rows)
            {
                // Pull all these values
                string userid = thisRow["UserID"].ToString();
                string fullname = thisRow["Full_Name"].ToString();
                string username = thisRow["UserName"].ToString();
                string email = thisRow["EmailAddress"].ToString();

                // Build the action links
                Output.WriteLine("  <tr align=\"left\" >");
                Output.Write("    <td class=\"SobekAdminActionLink\" >( ");

                Output.Write("<a title=\"Click to edit\" href=\"" + redirect.Replace("XXXXXXX", userid) + "\">edit</a> | ");
                Output.Write("<a title=\"Click to reset the password\" id=\"RESET_" + userid + "\" href=\"javascript:reset_password('" + userid + "','" + fullname.Replace("'", "") + "');\">reset password</a> | ");
                Output.Write("<a title=\"Click to view\" href=\"" + redirect.Replace("XXXXXXX", userid) + "v\">view</a> ) </td>");

                // Add the rest of the row with data
                Output.WriteLine("    <td>" + fullname + " ( " + username + " )</span></td>");
                Output.WriteLine("    <td>" + email + "</span></td>");
                Output.WriteLine("   </tr>");
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");

            }

            Output.WriteLine("</table>");
            Output.WriteLine("<br />");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }

        #region Nested type: Users_Admin_Mode_Enum

        private enum Users_Admin_Mode_Enum : byte
        {
            List_Users_And_Groups = 1,
            View_User,
            Edit_User
        }

        #endregion

    }
}
