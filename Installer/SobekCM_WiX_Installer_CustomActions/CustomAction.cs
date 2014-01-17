using System;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Win32;
using Microsoft.Deployment.WindowsInstaller;
using Encoder = System.Text.Encoder;
using View = Microsoft.Deployment.WindowsInstaller.View;


namespace SobekCM_WiX_Installer_CustomActions
{

	public class CustomAction
	{
		
		#region Methods to validate credentials ( web and builder )

		/// <summary> Method validates the provided web credentials </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		[CustomAction]
		public static ActionResult ValidateWebCredentials(Session session)
		{
			session["LOG"] = "ValidateWebCredentials: Start";

			// Get the username and password from the session state
			string username = session["WEB_SERVICE_ACCOUNT"];
			string password = session["WEB_SERVICE_PASSWORD"];
			session["WEB_SERVICE_ACCOUNT_VALID"] = null;

			// Get the computer name
			string machine_name = Environment.MachineName.ToLower();
			session["MACHINE_NAME"] = machine_name;
			if ( username.Trim().ToLower().IndexOf(machine_name + "\\") == 0 )
			{
				// Try the local computer first
				try
				{
					PrincipalContext localContext = new PrincipalContext(ContextType.Machine);
					using (localContext)
					{
						session["LOG"] = "ValidateWebCredentials: Validating local user/password for " + username;

						bool isValid = localContext.ValidateCredentials(username, password);
						if (isValid)
						{
							session["LOG"] = "ValidateWebCredentials: Credentials validated";
							session["WEB_SERVICE_ACCOUNT_VALID"] = "1";

							// If the web service account and the builder are the same,
							// go ahead and copy the password over, in case the builder
							// is also installed.
							if (session["WEB_SERVICE_ACCOUNT"] == session["BUILDER_SERVICE_ACCOUNT"])
							{
								session["BUILDER_SERVICE_PASSWORD"] = password;
							}

							return ActionResult.Success;
						}
						else
						{
							session["LOG"] = "ValidateWebCredentials: Credentials NOT VALID via local";
						}
					}
				}
				catch (Exception ex)
				{
					session["LOG"] = "ValidateWebCredentials: exception on local check: " + ex.Message;
				}


				try
				{
					// Test this username and password
					session.Log("ValidateWebCredentials: Pulling principal context (domain)");
					PrincipalContext adContext = new PrincipalContext(ContextType.Domain);
					using (adContext)
					{
						session["LOG"] = "ValidateWebCredentials: Validating network user/password for " + username;

						bool isValid = adContext.ValidateCredentials(username, password);
						if (isValid)
						{
							session["LOG"] = "ValidateWebCredentials: Credentials validated";
							session["WEB_SERVICE_ACCOUNT_VALID"] = "1";

							// If the web service account and the builder are the same,
							// go ahead and copy the password over, in case the builder
							// is also installed.
							if (session["WEB_SERVICE_ACCOUNT"] == session["BUILDER_SERVICE_ACCOUNT"])
							{
								session["BUILDER_SERVICE_PASSWORD"] = password;
							}
						}
						else
						{
							session["LOG"] = "ValidateWebCredentials: Credentials NOT VALID via domain";
						}
					}
				}
				catch (Exception ex)
				{
					session["LOG"] = "ValidateWebCredentials: exception on network check: " + ex.Message;
				}

				return ActionResult.Success;
			}

			try
			{
				// Test this username and password
				session.Log("ValidateWebCredentials: Pulling principal context (domain)");
				PrincipalContext adContext = new PrincipalContext(ContextType.Domain);
				using (adContext)
				{
					session["LOG"] = "ValidateWebCredentials: Validating network user/password for " + username;

					bool isValid = adContext.ValidateCredentials(username, password);
					if (isValid)
					{
						session["LOG"] = "ValidateWebCredentials: Credentials validated";
						session["WEB_SERVICE_ACCOUNT_VALID"] = "1";

						// If the web service account and the builder are the same,
						// go ahead and copy the password over, in case the builder
						// is also installed.
						if (session["WEB_SERVICE_ACCOUNT"] == session["BUILDER_SERVICE_ACCOUNT"])
						{
							session["BUILDER_SERVICE_PASSWORD"] = password;
						}
					}
					else
					{
						session["LOG"] = "ValidateWebCredentials: Credentials NOT VALID via domain";
					}
				}
			}
			catch (Exception ex)
			{
				session["LOG"] = "ValidateWebCredentials: exception on network check: " + ex.Message;
			}

			// Try the local computer second in this case
			try
			{
				PrincipalContext localContext = new PrincipalContext(ContextType.Machine);
				using (localContext)
				{
					session["LOG"] = "ValidateWebCredentials: Validating local user/password for " + username;

					bool isValid = localContext.ValidateCredentials(username, password);
					if (isValid)
					{
						session["LOG"] = "ValidateWebCredentials: Credentials validated";
						session["WEB_SERVICE_ACCOUNT_VALID"] = "1";

						// If the web service account and the builder are the same,
						// go ahead and copy the password over, in case the builder
						// is also installed.
						if (session["WEB_SERVICE_ACCOUNT"] == session["BUILDER_SERVICE_ACCOUNT"])
						{
							session["BUILDER_SERVICE_PASSWORD"] = password;
						}

						return ActionResult.Success;
					}
					else
					{
						session["LOG"] = "ValidateWebCredentials: Credentials NOT VALID via local";
					}
				}
			}
			catch (Exception ex)
			{
				session["LOG"] = "ValidateWebCredentials: exception on local check: " + ex.Message;
			}

			return ActionResult.Success;
		}

		[CustomAction]
		public static ActionResult ValidateBuilderCredentials(Session session)
		{
			session["LOG"] = "ValidateBuilderCredentials: Start";

			// Get the username and password from the session state
			// Get the username and password from the session state
			string username = session["BUILDER_SERVICE_ACCOUNT"];
			string password = session["BUILDER_SERVICE_PASSWORD"];
			session["BUILDER_SERVICE_ACCOUNT_VALID"] = null;

			// Get the computer name
			string machine_name = Environment.MachineName.ToLower();
			if ( username.Trim().ToLower().IndexOf(machine_name + "\\") == 0 )
			{
				// Try the local computer first
				try
				{
					PrincipalContext localContext = new PrincipalContext(ContextType.Machine);
					using (localContext)
					{
						session["LOG"] = "ValidateBuilderCredentials: Validating local user/password for " + username;

						bool isValid = localContext.ValidateCredentials(username, password);
						if (isValid)
						{
							session["LOG"] = "ValidateBuilderCredentials: Credentials validated";
							session["BUILDER_SERVICE_ACCOUNT_VALID"] = "1";

							return ActionResult.Success;
						}
						else
						{
							session["LOG"] = "ValidateBuilderCredentials: Credentials NOT VALID via local";
						}
					}
				}
				catch (Exception ex)
				{
					session["LOG"] = "ValidateBuilderCredentials: exception on local check: " + ex.Message;
				}


				try
				{
					// Test this username and password
					session.Log("ValidateBuilderCredentials: Pulling principal context (domain)");
					PrincipalContext adContext = new PrincipalContext(ContextType.Domain);
					using (adContext)
					{
						session["LOG"] = "ValidateBuilderCredentials: Validating network user/password for " + username;

						bool isValid = adContext.ValidateCredentials(username, password);
						if (isValid)
						{
							session["LOG"] = "ValidateBuilderCredentials: Credentials validated";
							session["BUILDER_SERVICE_ACCOUNT_VALID"] = "1";
						}
						else
						{
							session["LOG"] = "ValidateBuilderCredentials: Credentials NOT VALID via domain";
						}
					}
				}
				catch (Exception ex)
				{
					session["LOG"] = "ValidateBuilderCredentials: exception on network check: " + ex.Message;
				}

				return ActionResult.Success;
			}

			try
			{
				// Test this username and password
				session.Log("ValidateWebCredentials: Pulling principal context (domain)");
				PrincipalContext adContext = new PrincipalContext(ContextType.Domain);
				using (adContext)
				{
					session["LOG"] = "ValidateBuilderCredentials: Validating network user/password for " + username;

					bool isValid = adContext.ValidateCredentials(username, password);
					if (isValid)
					{
						session["LOG"] = "ValidateBuilderCredentials: Credentials validated";
						session["BUILDER_SERVICE_ACCOUNT_VALID"] = "1";
					}
					else
					{
						session["LOG"] = "ValidateBuilderCredentials: Credentials NOT VALID via domain";
					}
				}
			}
			catch (Exception ex)
			{
				session["LOG"] = "ValidateBuilderCredentials: exception on network check: " + ex.Message;
			}

			// Try the local computer second in this case
			try
			{
				PrincipalContext localContext = new PrincipalContext(ContextType.Machine);
				using (localContext)
				{
					session["LOG"] = "ValidateWebCredentials: Validating local user/password for " + username;

					bool isValid = localContext.ValidateCredentials(username, password);
					if (isValid)
					{
						session["LOG"] = "ValidateBuilderCredentials: Credentials validated";
						session["BUILDER_SERVICE_ACCOUNT_VALID"] = "1";

						return ActionResult.Success;
					}
					else
					{
						session["LOG"] = "ValidateBuilderCredentials: Credentials NOT VALID via local";
					}
				}
			}
			catch (Exception ex)
			{
				session["LOG"] = "ValidateBuilderCredentials: exception on local check: " + ex.Message;
			}

			return ActionResult.Success;
		}
	
		#endregion

		#region Method to validate the SQL server selected

		[CustomAction]
		public static ActionResult VerifySqlConnection(Session session)
		{
			try
			{
				session["LOG"] = "VerifySqlConnection: Begin";

				var builder = new SqlConnectionStringBuilder
				{
					DataSource = session["DB_SERVER"],
					InitialCatalog = "master",
					ConnectTimeout = 5,
					IntegratedSecurity = true
				};

				bool connection_successful = false;
				using (var connection = new SqlConnection(builder.ConnectionString))
				{
					if (CheckConnection(connection, session))
					{
						session["ODBC_CONNECTION_ESTABLISHED"] = "1";
						connection_successful = true;
					}
					else
					{
						session["ODBC_CONNECTION_ESTABLISHED"] = string.Empty;
						session["DATABASE_EXISTS"] = string.Empty;
					}
				}

				if (connection_successful)
				{
					// Check if full text is enabled
					using (var connection = new SqlConnection(builder.ConnectionString))
					{
						session["SQL_FULL_TEXT_INSTALLED"] = String.Empty;
						try
						{
							SqlCommand command = new SqlCommand("select FULLTEXTSERVICEPROPERTY('ISFULLTEXTINSTALLED')", connection);
							connection.Open();
							object fulltextflag = command.ExecuteScalar();
							connection.Close();

							if ((fulltextflag != DBNull.Value) && (fulltextflag.ToString() == "1"))
								session["SQL_FULL_TEXT_INSTALLED"] = "1";

						}
						catch
						{
							// Might error out if the full text not installed or not supportable
						}
					}

					// Check that the database is pre-existing
					builder.InitialCatalog = session["DB_DATABASE"];
					using (var connection = new SqlConnection(builder.ConnectionString))
					{
						if (CheckConnection(connection, session))
						{
							session["DATABASE_EXISTS"] = "1";
						}
						else
						{
							session["LOG"] = "VerifySqlConnection: Exception just means that the " + builder.InitialCatalog + " database does not yet exist";
						}
					}
				}

				session["LOG"] = "VerifySqlConnection: End";
			}
			catch (Exception ex)
			{
				session["LOG"] = "VerifySqlConnection: exception: " +  ex.Message;
				throw;
			}

			return ActionResult.Success;
		}

		private static bool CheckConnection( SqlConnection connection, Session session)
		{
			try
			{
				if (connection == null)
				{
					return false;
				}

				connection.Open();
				var canOpen = connection.State == ConnectionState.Open;
				connection.Close();

				return canOpen;
			}
			catch (SqlException ex)
			{
				session["ODBC_ERROR"] = ex.Message;
				return false;
			}
		}

		#endregion
		
		#region Get the list of web sites on the local machine

		[CustomAction]
		public static ActionResult GetWebSites(Session session)
		{
			try
			{
				View listBoxView = session.Database.OpenView("select * from ListBox");
				View availableWSView = session.Database.OpenView("select * from AvailableWebSites");
				DirectoryEntry iisRoot = new DirectoryEntry("IIS://localhost/W3SVC");
				int order = 1;

				foreach (DirectoryEntry webSite in iisRoot.Children)
				{
					if (webSite.SchemaClassName.ToLower() == "iiswebserver" && webSite.Name.ToLower() != "administration web site")
					{
						StoreWebSiteDataInListBoxTable(webSite, order, listBoxView);
						StoreWebSiteDataInAvailableWebSitesTable(webSite, availableWSView);
						order++;
					}
				}

				session["WEBSITE_LIST_PULLED"] = "1";
			}
			catch (Exception ex)
			{
				session["LOG"] = "GetWebSites: exception: " + ex.Message;
			}

			return ActionResult.Success;
		}

		[CustomAction]
		public static ActionResult UpdatePropsWithSelectedWebSite(Session session)
		{
			try
			{
				string selectedWebSiteId = session["WEBSITE"];
				session["LOG"] = "UpdatePropsWithSelectedWebSite: Found web site id: " + selectedWebSiteId;

				View availableWebSitesView = session.Database.OpenView("Select * from AvailableWebSites where WebSiteNo=" + selectedWebSiteId);
				availableWebSitesView.Execute();

				Record record = availableWebSitesView.Fetch();
				if ((record[1].ToString()) == selectedWebSiteId)
				{
					session["WEBSITE_DESCRIPTION"] = (string)record[2];
					session["WEBSITE_PORT"] = (string)record[3];
					session["WEBSITE_IP"] = (string)record[4];
					session["WEBSITE_HEADER"] = (string)record[5];
				}
			}
			catch (Exception ex)
			{
				session["LOG"] = "UpdatePropsWithSelectedWebSite: exception: " + ex.Message;
				return ActionResult.Failure;
			}

			return ActionResult.Success;
		}

		private static void StoreWebSiteDataInListBoxTable(DirectoryEntry webSite, int order, View listBoxView)
		{
			Record newListBoxRecord = new Record(4);
			newListBoxRecord[1] = "WEBSITE";
			newListBoxRecord[2] = order;
			newListBoxRecord[3] = webSite.Name;
			newListBoxRecord[4] = webSite.Properties["ServerComment"].Value;

			listBoxView.Modify(ViewModifyMode.InsertTemporary, newListBoxRecord);
		}


		private static void StoreWebSiteDataInAvailableWebSitesTable(DirectoryEntry webSite, View availableWSView)
		{
			//Get Ip, Port and Header from server bindings
			string[] serverBindings = ((string)webSite.Properties["ServerBindings"].Value).Split(':');
			string ip = serverBindings[0];
			string port = serverBindings[1];
			string header = serverBindings[2];

			Record newFoundWebSiteRecord = new Record(5);
			newFoundWebSiteRecord[1] = webSite.Name;
			newFoundWebSiteRecord[2] = webSite.Properties["ServerComment"].Value;
			newFoundWebSiteRecord[3] = port;
			newFoundWebSiteRecord[4] = ip;
			newFoundWebSiteRecord[5] = header;

			availableWSView.Modify(ViewModifyMode.InsertTemporary, newFoundWebSiteRecord);
		}

		#endregion

		#region Ensure that a web site is selected (used when non-admin or no IIS6 metabase, so can't choose website)

		[CustomAction]
		public static ActionResult EnsureWebSiteSelected(Session session)
		{
			try
			{
				string desc = session["WEBSITE_DESCRIPTION"];
				string port = session["WEBSITE_PORT"];
				if ((String.IsNullOrEmpty(desc)) || (String.IsNullOrEmpty(port)))
				{
					session.Log("EnsureWebSiteSelected : Setting to default web site, port 80");
					session["WEBSITE_DESCRIPTION"] = "Default Web Site";
					session["WEBSITE_PORT"] = "80";
					session["WEBSITE_IP"] = "*";
					session["WEBSITE_HEADER"] = "";
				}
			}
			catch (Exception ex)
			{
				session["LOG"] = "EnsureWebSiteSelected: exception: " + ex.Message;
				return ActionResult.Failure;
			}



			return ActionResult.Success;
		}

		#endregion

		#region Finalize the database install/configuration

		[CustomAction]
		public static ActionResult FinalizeDatabaseConfig(Session session)
		{

			session.Log("FinalizeDatabaseConfig: Retrieving values from custom action data");
			string db_name = session.CustomActionData["db_name_arg"];
			string db_server = session.CustomActionData["db_server_arg"];
			string web_account = session.CustomActionData["web_account_arg"];
			string institution_name = session.CustomActionData["inst_name_arg"];
			string institution_code = session.CustomActionData["inst_code_arg"];


			try
			{
				session.Log("FinalizeDatabaseConfig: Begin");

				var builder = new SqlConnectionStringBuilder
				{
					DataSource = db_server,
					InitialCatalog = db_name,
					ConnectTimeout = 15,
					IntegratedSecurity = true
				};

				bool connection_successful = false;
				using (var connection = new SqlConnection(builder.ConnectionString))
				{
					session.Log("FinalizeDatabaseConfig: Open connection");
					connection.Open();

								// Create the default institution name/code?
					if ((!String.IsNullOrEmpty(institution_code)) && (!String.IsNullOrEmpty(institution_name)))
					{
						session.Log("FinalizeWebConfig: Institution code/name provided");
						string cleaned_code = Clean_Code(institution_code);
						if (cleaned_code.Length > 0)
						{
							// Add to the webskin table
							session.Log("FinalizeDatabaseConfig: Add new web skin for code " + cleaned_code);
							SqlCommand command = new SqlCommand("if ( not exists ( select * from SobekCM_Web_Skin where WebSkinCode='" + cleaned_code + "' )) begin insert into SobekCM_Web_Skin ( WebSkinCode, OverrideHeaderFooter, OverrideBanner, BaseWebSkin, Notes, Build_On_Launch, SuppressTopNavigation ) values ( '" + institution_code + "', 'true', 'false', '', '" + institution_name + " Web Skin', 'true', 'false' ); end;", connection);
							command.ExecuteNonQuery();

							// Set the portal name and abbreviation
							session.Log("FinalizeDatabaseConfig: Update portal for system code and system name");
							command = new SqlCommand("update SobekCM_Portal_URL set Abbreviation='" + cleaned_code + "', Name='" + institution_name + " Portal' where len(Base_URL) = 0;", connection);
							command.ExecuteNonQuery();

							// The portal should use the new web skin
							session.Log("FinalizeDatabaseConfig: Set portal to use new " + cleaned_code + " web skin");
							command = new SqlCommand("update SobekCM_Portal_Web_Skin_Link set WebSkinID = ( select WebSkinID from SobekCM_Web_Skin where WebSkinCode='" + cleaned_code + "' )  where PortalID = ( select PortalID from SobekCM_Portal_URL where len(Base_URL) = 0);", connection);
							command.ExecuteNonQuery();

						}
					}

					session.Log("FinalizeDatabaseConfig: Close connection");
					connection.Close();
				}

				session.Log("FinalizeDatabaseConfig: End");
			}
			catch (Exception ex)
			{
				session.Log("FinalizeDatabaseConfig: exception: " +  ex.Message);
			}

			return ActionResult.Success;
		}

		#endregion

		#region Finalize the web site install/configuration

		[CustomAction]
		public static ActionResult FinalizeWebConfig(Session session)
		{
			session.Log("FinalizeWebConfig: Retrieving values from custom action data");
			string db_name = session.CustomActionData["db_name_arg"];
			string db_server = session.CustomActionData["db_server_arg"];
			string web_account = session.CustomActionData["web_account_arg"];
			string web_directory = session.CustomActionData["web_directory_arg"];
			string virtual_directory = session.CustomActionData["virtual_dir_arg"];
			string institution_name = session.CustomActionData["inst_name_arg"];
			string institution_code = session.CustomActionData["inst_code_arg"];

			// Get the machine IP address
			session.Log("FinalizeWebConfig: Get host IP");
			IPHostEntry host;
			string localIP = String.Empty;
			host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					localIP = ip.ToString();
				}
			}
			
			// Create the sobekcm.config file
			try
			{
				session.Log("FinalizeWebConfig: Creating sobekcm.config file");
				string config_file = web_directory + "\\config\\sobekcm.config";
				if (!File.Exists(config_file))
				{
					StreamWriter writer = new StreamWriter(config_file);
					writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?>");
					writer.WriteLine("<configuration>");
					writer.WriteLine("<connection_string type=\"MSSQL\">data source=" + db_server + ";initial catalog=" + db_name + ";integrated security=Yes;</connection_string>");
					writer.WriteLine("<error_emails></error_emails>");
					if (localIP.Length > 0)
					{
						if (virtual_directory.Length > 0)
						{
							writer.WriteLine("<error_page>http://" + localIP + "/" + virtual_directory + "/error.html</error_page>");
						}
						else
						{
							writer.WriteLine("<error_page>http://" + localIP + "/error.html</error_page>");
						}
					}
					else
					{
						writer.WriteLine("<error_page></error_page>");
					}
					
					writer.WriteLine("</configuration>");
					writer.Flush();
					writer.Close();
				}

			}
			catch (Exception ee)
			{
				string msg = ee.Message;
			}

			// Strangely, the sample skin is installed in the wrong place
			try
			{

				if (String.Compare(virtual_directory, "SobekCM", StringComparison.OrdinalIgnoreCase) != 0)
				{
					string wrong_folder = web_directory + "\\SobekCM\\design\\skins\\Sample";
					if (Directory.Exists(wrong_folder))
					{
						string design_folder = web_directory + "\\design\\skins\\";
						if (!Directory.Exists(design_folder))
							Directory.CreateDirectory(design_folder);
						string right_folder = web_directory + "\\design\\skins\\Sample";
						if (!Directory.Exists(right_folder))
						{
							Directory.Move(wrong_folder, right_folder);
							Directory.Delete(web_directory + "\\SobekCM\\design\\skins");
							Directory.Delete(web_directory + "\\SobekCM\\design");
							Directory.Delete(web_directory + "\\SobekCM");
						}
					}
					else
					{
						string parent_dir = (new DirectoryInfo(web_directory)).Parent.FullName.ToString();
						string wrong_folder2 = parent_dir + "\\SobekCM\\design\\skins\\Sample";
						if (Directory.Exists(wrong_folder2))
						{
							string design_folder = web_directory + "\\design\\skins\\";
							if (!Directory.Exists(design_folder))
								Directory.CreateDirectory(design_folder);
							string right_folder = web_directory + "\\design\\skins\\Sample";
							if (!Directory.Exists(right_folder))
							{
								Directory.Move(wrong_folder2, right_folder);
								Directory.Delete(parent_dir + "\\SobekCM\\design\\skins");
								Directory.Delete(parent_dir + "\\SobekCM\\design");
								Directory.Delete(parent_dir + "\\SobekCM");
							}
						}
					}
				}
			}
			catch (Exception)
			{
				
			}

			// If there is no ALL folder, move the sample_all over
			bool new_all_folder = false;
			try
			{
				session.Log("FinalizeWebConfig: Checking for existing ALL aggregation folder");
				string all_folder = web_directory + "\\design\\aggregations\\all";
				if ((!Directory.Exists(all_folder)) && ( Directory.Exists(web_directory + "\\design\\aggregations\\sample_all")))
				{
					Directory.Move(web_directory + "\\design\\aggregations\\sample_all", all_folder);
					new_all_folder = true;
				}

			}
			catch (Exception)
			{
				
			}


			// Create the default institution name/code?
			if ((!String.IsNullOrEmpty(institution_code)) && (!String.IsNullOrEmpty(institution_name)))
			{
				session.Log("FinalizeWebConfig: Institution code/name provided");
				string cleaned_code = Clean_Code(institution_code);
				if (cleaned_code.Length > 0)
				{
					session.Log("FinalizeWebConfig: Creating web skin design folder for " + cleaned_code);
					string design_folder = web_directory + "\\design\\skins\\" + cleaned_code;
					if (!Directory.Exists(design_folder))
						Directory.CreateDirectory(design_folder);

					string design_buttons = design_folder + "\\buttons";
					string sample_buttons = web_directory + "\\design\\skins\\sample\\buttons";
					string[] all_buttons = Directory.GetFiles(sample_buttons);
					if (!Directory.Exists(design_buttons))
						Directory.CreateDirectory(design_buttons);
					foreach (string thisFile in all_buttons)
					{
						File.Copy(thisFile, design_buttons + "\\" + Path.GetFileName(thisFile), false);
					}
					File.Copy(web_directory + "\\design\\skins\\sample\\sample.css", design_folder + "\\" + cleaned_code + ".css");
					File.Copy(web_directory + "\\design\\skins\\sample\\iphone-icon.png", design_folder + "\\iphone-icon.png");
					File.Copy(web_directory + "\\design\\skins\\sample\\new_element.jpg", design_folder + "\\new_element.jpg");
					File.Copy(web_directory + "\\design\\skins\\sample\\new_element_demo.jpg", design_folder + "\\new_element_demo.jpg");
					string design_html = design_folder + "\\html";
					if (!Directory.Exists(design_html))
						Directory.CreateDirectory(design_html);

					
					if (!File.Exists(design_html + "\\header.html"))
					{
						session.Log("FinalizeWebConfig: Creating skin header.html for " + cleaned_code);
						StreamWriter writer = new StreamWriter(design_html + "\\header.html");
						writer.WriteLine("<!-- Main header table includes static links, breadcrumbs, and other programmatically added links -->");
						writer.WriteLine("<header>  ");
  						writer.WriteLine("  <table style=\"border-collapse:collapse; border-spacing: 0;\" id=\"header\">");
    					writer.WriteLine("    <tr>");
      					writer.WriteLine("      <td class=\"headerBackground\">");
						writer.WriteLine("        <a href=\"<%BASEURL%>\">" + institution_name + "</a>");
      					writer.WriteLine("      </td>");
      					writer.WriteLine("      <td class=\"firstLinks\">");
        				writer.WriteLine("        <!-- Put an image or other links here -->");
      					writer.WriteLine("      </td>");
    					writer.WriteLine("    </tr>");
						writer.WriteLine("    <tr style=\"height:12px\">");
						writer.WriteLine("      <td class=\"HeaderNavLinks\" style=\"text-align:left;\"><%BREADCRUMBS%></td>");
						writer.WriteLine("      <td class=\"HeaderNavLinks\" style=\"text-align:right;\">");
						writer.WriteLine("        <%MYSOBEK%>&nbsp; | &nbsp; <a href=\"<%BASEURL%>HELP\">Help</a>&nbsp; ");
						writer.WriteLine("      </td>");
						writer.WriteLine("    </tr>");
						writer.WriteLine("  </table>");
						writer.WriteLine("</header>");
						writer.WriteLine();
						writer.WriteLine("<%BANNER%>");
						writer.WriteLine();
						writer.WriteLine("<!-- Blankets out the rest of the web form when a pop-up form is envoked -->");
						writer.WriteLine("<div id=\"blanket_outer\" style=\"display:none;\"></div>");
						writer.Flush();
						writer.Close();
					}


					if (!File.Exists(design_html + "\\header_item.html"))
					{
						session.Log("FinalizeWebConfig: Creating skin header_item.html for " + cleaned_code);
						StreamWriter writer = new StreamWriter(design_html + "\\header_item.html");
						writer.WriteLine("<!-- Blankets out the rest of the web form when a pop-up form is envoked -->");
						writer.WriteLine("<div id=\"blanket_outer\" style=\"display:none;\"></div>");
						writer.WriteLine();
						writer.WriteLine("<!-- Main header table includes static links, breadcrumbs, and other programmatically added links -->");
						writer.WriteLine("<header>  ");
						writer.WriteLine("  <table style=\"border-collapse:collapse; border-spacing: 0;\" id=\"header\">");
						writer.WriteLine("    <tr>");
						writer.WriteLine("      <td class=\"headerBackground\">");
						writer.WriteLine("        <a href=\"<%BASEURL%>\">" + institution_name + "</a>");
						writer.WriteLine("      </td>");
						writer.WriteLine("      <td class=\"firstLinks\">");
						writer.WriteLine("        <!-- Put an image or other links here -->");
						writer.WriteLine("      </td>");
						writer.WriteLine("    </tr>");
						writer.WriteLine("    <tr class=\"HeaderNavRow_item\">");
						writer.WriteLine("      <td class=\"HeaderNavLinks_item\" style=\"text-align:left;\"><%BREADCRUMBS%></td>");
						writer.WriteLine("      <td class=\"HeaderNavLinks_item\" style=\"text-align:right;\">");
						writer.WriteLine("        <%MYSOBEK%>&nbsp; | &nbsp; <a href=\"<%BASEURL%>HELP\">Help</a>&nbsp; ");
						writer.WriteLine("      </td>");
						writer.WriteLine("    </tr>");
						writer.WriteLine("  </table>");
						writer.WriteLine("</header>");
						writer.WriteLine();
						writer.WriteLine("<%BANNER%>");
						writer.Flush();
						writer.Close();
					}

					if (!File.Exists(design_html + "\\footer.html"))
					{
						session.Log("FinalizeWebConfig: Creating skin footer.html for " + cleaned_code);
						StreamWriter writer = new StreamWriter(design_html + "\\footer.html");
						writer.WriteLine("<!-- Footer divisions complete the web page -->");
						writer.WriteLine("<footer id=\"footer\"> ");
						writer.WriteLine("  <nav>");
						writer.WriteLine("    <p><a href=\"<%BASEURL%>contact\">Contact Us</a> | ");
						writer.WriteLine("       <a href=\"<%BASEURL%>permissions\">Permissions</a> | ");
						writer.WriteLine("       <a href=\"<%BASEURL%>stats\">Statistics</a> | ");
						writer.WriteLine("       <a href=\"<%BASEURL%>internal\">Internal</a> | ");
						writer.WriteLine("       <a href=\"<%BASEURL%>rss\">RSS</a></p> ");
						writer.WriteLine("  </nav>");
						writer.WriteLine();
						writer.WriteLine("  <div id=\"Wordmark\"> ");
						writer.WriteLine("    <!-- Put image here -->  ");
						writer.WriteLine("  </div> ");
						writer.WriteLine("  <div id=\"Copyright\"> ");
						writer.WriteLine("    &copy; " + DateTime.Now.Year + ", " + institution_name + ". <br /> ");
						writer.WriteLine("    Powered by <a href=\"http://sobekrepository.org\">SobekCM</a> - <a href=\"http://ufdc.ufl.edu/l/sobekcm/development/history\"><%VERSION%></a> ");
						writer.WriteLine("  </div>");
						writer.WriteLine("</footer>");
						writer.Flush();
						writer.Close();
					}

					if (!File.Exists(design_html + "\\footer_item.html"))
					{
						session.Log("FinalizeWebConfig: Copying footer.html to footer_item.html for " + cleaned_code);
						File.Copy(design_html + "\\footer.html", design_html + "\\footer_item.html");
					}

					// Create the banner with the name of the collection
					if (Directory.Exists(web_directory + "\\default\\banner_images"))
					{
						session.Log("FinalizeWebConfig: Creating main banner for the repository with institution name");
						try
						{
							string[] banners = Directory.GetFiles(web_directory + "\\default\\banner_images", "*.jpg");
							Random randomizer = new Random();
							string banner_to_use = banners[randomizer.Next(0, banners.Length - 1)];
							Bitmap bitmap = (Bitmap) System.Drawing.Bitmap.FromFile(banner_to_use);

							RectangleF rectf = new RectangleF( 30, bitmap.Height - 55, bitmap.Width - 40, 40 );
					        Graphics g = Graphics.FromImage(bitmap);
					        g.SmoothingMode = SmoothingMode.AntiAlias;
							g.InterpolationMode = InterpolationMode.HighQualityBicubic;
							g.PixelOffsetMode = PixelOffsetMode.HighQuality;
							g.DrawString( institution_name, new Font("Thaoma",30,FontStyle.Bold), Brushes.Black, rectf);
							g.Flush();

							string new_file = web_directory + "\\design\\aggregations\\all\\images\\banners\\coll.jpg";
							if ( File.Exists(new_file))
								File.Delete(new_file);
							bitmap.Save(new_file, ImageFormat.Jpeg);
						}
						catch (Exception  ee)
						{
							string msg = ee.Message;
						}
					}

				}
			}

			session.Log("FinalizeWebConfig: End");

			return ActionResult.Success;
		}

		#endregion

		#region Launch the digital repository page

		[CustomAction]
		public static ActionResult LaunchApplication(Session session)
		{
			string virtual_directory = session["VIRTUAL_DIRECTORY"];

			if (virtual_directory.Length > 0)
			{
				System.Diagnostics.Process.Start("http://localhost/" + virtual_directory + "/my/preferences");
			}
			else
			{
				System.Diagnostics.Process.Start("http://localhost/my/preferences");
			}

			return ActionResult.Success;
		}

		#endregion

		#region Launch the pre-requisites help page

		[CustomAction]
		public static ActionResult ShowHelp(Session session)
		{
			System.Diagnostics.Process.Start("http://ufdc.ufl.edu/software/requirements");
			return ActionResult.Success;
		}

		#endregion

		#region Custom action to check for the Apache Tomcat root install path

		[CustomAction]
		public static ActionResult CheckRemainingRequirements(Session session)
		{
			// System.Diagnostics.Debugger.Launch();

			// Look for the TOMCAT DIRECTORY
			session["TOMCAT_DIRECTORY"] = String.Empty;
			try
			{
				string possibleKey = Get_Registry_Value(@"SOFTWARE\Apache Software Foundation\Tomcat\7.0\Tomcat7", "InstallPath");
				if (!String.IsNullOrEmpty(possibleKey))
				{
					session["TOMCAT_DIRECTORY"] = possibleKey;
				}
				else
				{
					possibleKey = Get_Registry_Value(@"SOFTWARE\Apache Software Foundation\Tomcat\6.0", "InstallPath");
					if (!String.IsNullOrEmpty(possibleKey))
					{
						session["TOMCAT_DIRECTORY"] = possibleKey;
					}
				}
			}
			catch (Exception)
			{
				// Suppress exceptions, the user can navigate here
			}

			// LOOK FOR THE GHOSTSCRIPT DIRECTORY
			string possible_ghost = Look_For_Variable_Registry_Key("SOFTWARE\\GPL Ghostscript", "GS_DLL");
			if ( !String.IsNullOrEmpty(possible_ghost))
				session["GHOSTSCRIPT_DIRECTORY"] = possible_ghost;
			else
				session["GHOSTSCRIPT_DIRECTORY"] = String.Empty;

			// LOOK FOR THE IMAGEMAGICK DIRECTORY
			string possible_imagemagick = Look_For_Variable_Registry_Key("SOFTWARE\\ImageMagick", "BinPath");
			if (!String.IsNullOrEmpty(possible_imagemagick))
				session["IMAGEMAGICK_DIRECTORY"] = possible_imagemagick;
			else
				session["IMAGEMAGICK_DIRECTORY"] = String.Empty;

			return ActionResult.Success;
		}

		#endregion

		private static string Clean_Code(string Code)
		{
			StringBuilder builder = new StringBuilder();
			foreach (char thisChar in Code)
			{
				if (Char.IsLetter(thisChar))
					builder.Append(thisChar);
			}
			return builder.ToString();
		}

		private static string Look_For_Variable_Registry_Key(string Manufacturer, string KeyName)
		{
			RegistryKey localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			localKey = localKey.OpenSubKey(Manufacturer);
			if (localKey != null)
			{
				string[] subkeys = localKey.GetSubKeyNames();
				foreach (string thisSubKey in subkeys)
				{
					RegistryKey subKey = localKey.OpenSubKey(thisSubKey);
					string value64 = subKey.GetValue(KeyName) as string;
					if (!String.IsNullOrEmpty(value64))
						return value64;
				}
			}
			RegistryKey localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
			localKey32 = localKey32.OpenSubKey(Manufacturer);
			if (localKey32 != null)
			{
				string[] subkeys = localKey32.GetSubKeyNames();
				foreach (string thisSubKey in subkeys)
				{
					RegistryKey subKey = localKey32.OpenSubKey(thisSubKey);
					string value32 = subKey.GetValue(KeyName) as string;
					if (!String.IsNullOrEmpty(value32))
						return value32;
				}
			}
			return null;
		}

		private static string Get_Registry_Value(string KeyPath, string KeyName)
		{
			RegistryKey localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			localKey = localKey.OpenSubKey(KeyPath);
			if (localKey != null)
			{
				string tomcat6_value64 = localKey.GetValue(KeyName) as string;
				if (tomcat6_value64 != null)
				{
					return tomcat6_value64;
				}
			}
			RegistryKey localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
			localKey32 = localKey32.OpenSubKey(KeyPath);
			if (localKey32 != null)
			{
				string tomcat6_value32 = localKey32.GetValue(KeyName) as string;
				if (tomcat6_value32 != null)
				{
					return tomcat6_value32;
				}
			}

			return null;
		}
	}
}
