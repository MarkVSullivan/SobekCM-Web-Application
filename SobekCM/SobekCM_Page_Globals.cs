#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Core.Aggregations;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Items;
using SobekCM.Core.Navigation;
using SobekCM.Core.Results;
using SobekCM.Core.SiteMap;
using SobekCM.Core.Skins;
using SobekCM.Core.Users;
using SobekCM.Core.WebContent;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library;
using SobekCM.Library.Database;
using SobekCM.Library.MainWriters;
using SobekCM.Engine.MemoryMgmt;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Tools;
using SobekCM.UI_Library;

#endregion

namespace SobekCM
{
	public class SobekCM_Page_Globals
	{
		#region Private class members

		public string browse_info_display_text;
		public SobekCM_Item currentItem;
		public SobekCM_Navigation_Object currentMode;
		public Page_TreeNode currentPage;
		public User_Object currentUser;
		public Item_Aggregation hierarchyObject;
		public SobekCM_Skin_Object htmlSkin;
		public SobekCM_Items_In_Title itemsInTitle;
		public abstractMainWriter mainWriter;
		public List<iSearch_Title_Result> pagedSearchResults;
		public Public_User_Folder publicFolder;
		public Search_Results_Statistics searchResultStatistics;
		public SobekCM_SiteMap siteMap;
		public HTML_Based_Content staticWebContent;
		public Item_Aggregation_Child_Page thisBrowseObject;
		public Custom_Tracer tracer;

		#endregion

		#region Constructor for this class

		public SobekCM_Page_Globals(bool isPostBack, string page_name)
		{
			// Pull out the http request
			HttpRequest request = HttpContext.Current.Request;

			// Get the base url
			string base_url = request.Url.AbsoluteUri.ToLower().Replace("sobekcm.aspx", "");
			if (base_url.IndexOf("?") > 0)
				base_url = base_url.Substring(0, base_url.IndexOf("?"));

			try
			{
				tracer = new Custom_Tracer();
				tracer.Add_Trace("SobekCM_Page_Globals.Constructor", String.Empty);
			    SobekCM_Database.Connection_String = UI_ApplicationCache_Gateway.Settings.Database_Connections[0].Connection_String;


				// Check that something is saved for the original requested URL (may not exist if not forwarded)
				if (!HttpContext.Current.Items.Contains("Original_URL"))
					HttpContext.Current.Items["Original_URL"] = request.Url.ToString();
			}
			catch (Exception ee)
			{
				// Send to the dashboard
				if ((HttpContext.Current.Request.UserHostAddress == "127.0.0.1") || (HttpContext.Current.Request.UserHostAddress == HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"]) || (HttpContext.Current.Request.Url.ToString().IndexOf("localhost") >= 0))
				{
					// Create an error message 
					string errorMessage = "Error caught while validating application state";
                    if ((UI_ApplicationCache_Gateway.Settings.Database_Connections.Count == 0) || (String.IsNullOrEmpty(UI_ApplicationCache_Gateway.Settings.Database_Connections[0].Connection_String)))
					{
						errorMessage = "No database connection string found!";
						string configFileLocation = AppDomain.CurrentDomain.BaseDirectory + "config/sobekcm.xml";
						try
						{
							if (!File.Exists(configFileLocation))
							{
								errorMessage = "Missing config/sobekcm.xml configuration file on the web server.<br />Ensure the configuration file 'sobekcm.xml' exists in a 'config' subfolder directly under the web application.<br />Example configuration is:" +
								               "<div style=\"background-color: #bbbbbb; margin-left: 30px; margin-top:10px; padding: 3px;\">&lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot; standalone=&quot;yes&quot;  ?&gt;<br /> &lt;configuration&gt;<br /> &nbsp; &nbsp &lt;connection_string type=&quot;MSSQL&quot;&gt;data source=localhost\\instance;initial catalog=SobekCM;integrated security=Yes;&lt;/connection_string&gt;<br /> &nbsp; &nbsp &lt;error_emails&gt;marsull@uflib.ufl.edu&lt;/error_emails&gt;<br /> &nbsp; &nbsp &lt;error_page&gt;http://ufdc.ufl.edu/error.html&lt;/error_page&gt;<br />&lt;/configuration&gt;</div>";
							}
						}
						catch
						{
							errorMessage = "No database connection string found.<br />Likely an error reading the configuration file due to permissions on the web server.<br />Ensure the configuration file 'sobekcm.xml' exists in a 'config' subfolder directly under the web application.<br />Example configuration is:" +
							               "<div style=\"background-color: #bbbbbb; margin-left: 30px; margin-top:10px; padding: 3px;\">&lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot; standalone=&quot;yes&quot;  ?&gt;<br /> &lt;configuration&gt;<br /> &nbsp; &nbsp &lt;connection_string type=&quot;MSSQL&quot;&gt;data source=localhost\\instance;initial catalog=SobekCM;integrated security=Yes;&lt;/connection_string&gt;<br /> &nbsp; &nbsp &lt;error_emails&gt;marsull@uflib.ufl.edu&lt;/error_emails&gt;<br /> &nbsp; &nbsp &lt;error_page&gt;http://ufdc.ufl.edu/error.html&lt;/error_page&gt;<br />&lt;/configuration&gt;</div>";
						}
					}
					else
					{
						if (ee.Message.IndexOf("The EXECUTE permission") >= 0)
						{
							errorMessage = "Permissions error while connecting to the database and pulling necessary data.<br /><br />Confirm the following:<ul><li>IIS is configured correctly to use anonymous authentication</li><li>Anonymous user (or service account) is part of the sobek_users role in the database.</li></ul>";
						}
						else
						{
							errorMessage = "Error connecting to the database and pulling necessary data.<br /><br />Confirm the following:<ul><li>Database connection string is correct ( " + UI_ApplicationCache_Gateway.Settings.Database_Connections[0].Connection_String + ")</li><li>IIS is configured correctly to use anonymous authentication</li><li>Anonymous user (or service account) is part of the sobek_users role in the database.</li></ul>";
						}
					}
					// Wrap this into the SobekCM Exception
					SobekCM_Traced_Exception newException = new SobekCM_Traced_Exception(errorMessage, ee, tracer);

					// Save this to the session state, and then forward to the dashboard
					HttpContext.Current.Session["Last_Exception"] = newException;
					HttpContext.Current.Response.Redirect("dashboard.aspx", false);
					HttpContext.Current.ApplicationInstance.CompleteRequest();
					return;
				}
				else
				{
					throw ee;
				}
			}

			// Analyze the response and get the mode
			try
			{
			    currentMode = new SobekCM_Navigation_Object();
			    SobekCM_QueryString_Analyzer.Parse_Query(request.QueryString, currentMode, base_url, request.UserLanguages, UI_ApplicationCache_Gateway.Aggregations, UI_ApplicationCache_Gateway.Collection_Aliases, UI_ApplicationCache_Gateway.Items, UI_ApplicationCache_Gateway.URL_Portals, tracer);

                currentMode.Base_URL=base_url;
			    currentMode.isPostBack = isPostBack;
                currentMode.Browser_Type = request.Browser.Type.ToUpper();
				currentMode.Set_Robot_Flag(request.UserAgent, request.UserHostAddress);
			}
			catch
			{
				HttpContext.Current.Response.Status = "301 Moved Permanently";
				HttpContext.Current.Response.AddHeader("Location", base_url);
				HttpContext.Current.ApplicationInstance.CompleteRequest();
				return;
			}

			// If this was for HTML, but was at the data, just convert to XML 
			if ((page_name == "SOBEKCM_DATA") && (currentMode.Writer_Type != Writer_Type_Enum.XML) && (currentMode.Writer_Type != Writer_Type_Enum.JSON) && (currentMode.Writer_Type != Writer_Type_Enum.DataSet) && (currentMode.Writer_Type != Writer_Type_Enum.Data_Provider))
				currentMode.Writer_Type = Writer_Type_Enum.XML;


			tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Navigation Object created from URI query string");

			try
			{
				if (currentMode.Aggregation.ToUpper() == "EPCSANB")
				{
					HttpContext.Current.Response.Redirect(@"http://www.uflib.ufl.edu/epc/", false);
					HttpContext.Current.ApplicationInstance.CompleteRequest();
					currentMode.Request_Completed = true;
					return;
				}

				// If this was an error, redirect now
				if (currentMode.Mode == Display_Mode_Enum.Error)
				{
					return;
				}

				// All the user stuff can be skipped if this was from a robot
				if (!currentMode.Is_Robot)
				{
					// Determine which IP Ranges this IP address belongs to, if not already determined.
					if (HttpContext.Current.Session["IP_Range_Membership"] == null)
					{
					    int ip_mask = UI_ApplicationCache_Gateway.IP_Restrictions.Restrictive_Range_Membership(request.UserHostAddress);
						HttpContext.Current.Session["IP_Range_Membership"] = ip_mask;
					}

					// Set the Session TOC, if provided
					if (currentMode.TOC_Display != TOC_Display_Type_Enum.Undetermined)
					{
						if (currentMode.TOC_Display == TOC_Display_Type_Enum.Hide)
						{
							HttpContext.Current.Session["Show TOC"] = false;
						}
						else
						{
							HttpContext.Current.Session["Show TOC"] = true;
						}
					}

					// Only do any of the user stuff if this is from the main SobekCM page
					if (page_name == "SOBEKCM")
					{
						tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Checking for logged on user by cookie or session");
						perform_user_checks(isPostBack);
					}

					// If this is a system admin, they can run as a different user actually
					if ((currentUser != null) && (currentUser.Is_System_Admin) && (request.QueryString["userid"] != null))
					{
						try
						{
							int userid = Convert.ToInt32(request.QueryString["userid"]);
							User_Object mirroredUser = SobekCM_Database.Get_User(userid, tracer);
							if (mirroredUser != null)
							{
								// Replace the user information in the session state
								HttpContext.Current.Session["user"] = mirroredUser;
								currentUser = mirroredUser;
							}
						}
						catch (Exception)
						{
							// Nothing to do here.. shouldn't ever really be here..
						}
					}

					if (currentMode.Request_Completed)
						return;

					// If this was a call for RESET, clear the memory
					if ((currentMode.Mode == Display_Mode_Enum.Administrative) && (currentMode.Admin_Type == Admin_Type_Enum.Reset))
					{
						Reset_Memory();

						// Since this reset, send to the admin, memory management portion
						currentMode.Mode = Display_Mode_Enum.Internal;
						currentMode.Internal_Type = Internal_Type_Enum.Cache;
					}
				}
				else // THIS IS A ROBOT REQUEST
				{
					Perform_Search_Engine_Robot_Checks(currentMode, request.QueryString);
				}

				// If this is for a public folder, get the data
				if (currentMode.Mode == Display_Mode_Enum.Public_Folder)
				{
					Public_Folder();
				}

				// Get the item now, so that you can set the collection code, if there was none listed
                if ((currentMode.Mode == Display_Mode_Enum.Item_Display) || (currentMode.Mode == Display_Mode_Enum.Item_Print) || ((currentMode.Mode == Display_Mode_Enum.My_Sobek) && ((currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Item_Metadata) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Item_Behaviors) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Item_Permissions)))
					|| ((currentMode.Mode == Display_Mode_Enum.My_Sobek) && ((currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Group_Behaviors) ||  (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Group_Serial_Hierarchy) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Group_Add_Volume) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Group_AutoFill_Volumes) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Group_Mass_Update_Items) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.File_Management) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Page_Images_Management) || (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Delete_Item))))
				{
					Display_Item();
				}

				// Was this a robot?
				if (currentMode.Request_Completed)
					return;

				// Get the group, collection, or subcollection from the database or cache.
				// This also makes sure they are a proper hierarchy. 
				Get_Entire_Collection_Hierarchy();

				// Run the search if this should be done now
				if (currentMode.Mode == Display_Mode_Enum.Results)
				{
					Search_Block();
				}

				// Run the browse/info work if it is of those modes
				if ((currentMode.Mode == Display_Mode_Enum.Aggregation) && ((currentMode.Aggregation_Type == Aggregation_Type_Enum.Browse_Info) || (currentMode.Aggregation_Type == Aggregation_Type_Enum.Child_Page_Edit)))
				{
					Browse_Info_Block();
				}

				if (currentMode.Mode == Display_Mode_Enum.My_Sobek)
				{
					MySobekCM_Block();
				}

				// Run the simple text block if this is that mode
				if (currentMode.Mode == Display_Mode_Enum.Simple_HTML_CMS)
				{
					Simple_Web_Content_Text_Block();
				}
			}
			catch (OutOfMemoryException ee)
			{
				if (currentMode != null)
				{
					currentMode.Mode = Display_Mode_Enum.Error;
					currentMode.Error_Message = "Out of memory exception caught";
					currentMode.Caught_Exception = ee;
				}
				else
				{
					Email_Information("Fatal Out of memory exception caught", ee);
				}
			}
			catch (Exception ee)
			{
				if (currentMode != null)
				{
					currentMode.Mode = Display_Mode_Enum.Error;
					currentMode.Error_Message = "Unknown error occurred";
					currentMode.Caught_Exception = ee;
				}
				else
				{
					Email_Information("Unknown Fatal Error Occurred", ee);
				}
			}
		}

		#endregion

		#region Special checks for search engine robot URL behaviors

		private void Perform_Search_Engine_Robot_Checks(SobekCM_Navigation_Object CurrentModeCheck, NameValueCollection QueryString)
		{
			// Some writers should not be selected yet
			if ((CurrentModeCheck.Writer_Type != Writer_Type_Enum.HTML) && (CurrentModeCheck.Writer_Type != Writer_Type_Enum.HTML_Echo) && (CurrentModeCheck.Writer_Type != Writer_Type_Enum.OAI))
			{
				HttpContext.Current.Response.Status = "301 Moved Permanently";
				HttpContext.Current.Response.AddHeader("Location", CurrentModeCheck.Base_URL);
				HttpContext.Current.ApplicationInstance.CompleteRequest();
				currentMode.Request_Completed = true;
				return;
			}

			// There are some spots which robots are never allowed to go, just
			// by virtue of the fact they don't logon
			if ((CurrentModeCheck.Mode == Display_Mode_Enum.Internal) || (CurrentModeCheck.Mode == Display_Mode_Enum.My_Sobek) || (CurrentModeCheck.Mode == Display_Mode_Enum.Administrative) || (CurrentModeCheck.Mode == Display_Mode_Enum.Reset) || (CurrentModeCheck.Mode == Display_Mode_Enum.Item_Cache_Reload) || (CurrentModeCheck.Mode == Display_Mode_Enum.Results) || (CurrentModeCheck.Mode == Display_Mode_Enum.Public_Folder) || ((CurrentModeCheck.Mode == Display_Mode_Enum.Aggregation) && (CurrentModeCheck.Aggregation_Type == Aggregation_Type_Enum.Browse_By)) || (CurrentModeCheck.Mode == Display_Mode_Enum.Item_Print))
			{
				HttpContext.Current.Response.Status = "301 Moved Permanently";
				HttpContext.Current.Response.AddHeader("Location", CurrentModeCheck.Base_URL);
				HttpContext.Current.ApplicationInstance.CompleteRequest();
				currentMode.Request_Completed = true;
				return;
			}

			// Browse are okay, except when it is the NEW
			if ((CurrentModeCheck.Mode == Display_Mode_Enum.Aggregation) && (CurrentModeCheck.Aggregation_Type == Aggregation_Type_Enum.Browse_Info) && (CurrentModeCheck.Info_Browse_Mode == "new"))
			{
				CurrentModeCheck.Info_Browse_Mode = "all";

				HttpContext.Current.Response.Status = "301 Moved Permanently";
				HttpContext.Current.Response.AddHeader("Location", UrlWriterHelper.Redirect_URL(CurrentModeCheck));
				HttpContext.Current.ApplicationInstance.CompleteRequest();
				currentMode.Request_Completed = true;
				return;
			}

			// Going to the search page is okay, except for ADVANCED searches ( results aren't okay, but going to the search page is okay )
			if ((CurrentModeCheck.Mode == Display_Mode_Enum.Search) && (CurrentModeCheck.Search_Type == Search_Type_Enum.Advanced))
			{
				CurrentModeCheck.Mode = Display_Mode_Enum.Aggregation;
				CurrentModeCheck.Aggregation_Type = Aggregation_Type_Enum.Home;
				HttpContext.Current.Response.Status = "301 Moved Permanently";
				HttpContext.Current.Response.AddHeader("Location", UrlWriterHelper.Redirect_URL(CurrentModeCheck));
				HttpContext.Current.ApplicationInstance.CompleteRequest();
				currentMode.Request_Completed = true;
				return;
			}

			// If this was a legacy type request, forward to the new URL
			if ((QueryString["b"] != null) || (QueryString["m"] != null) || (QueryString["g"] != null) || (QueryString["c"] != null) || (QueryString["s"] != null) || (QueryString["a"] != null))
			{
				HttpContext.Current.Response.Status = "301 Moved Permanently";
				HttpContext.Current.Response.AddHeader("Location", UrlWriterHelper.Redirect_URL(CurrentModeCheck));
				HttpContext.Current.ApplicationInstance.CompleteRequest();
				currentMode.Request_Completed = true;
				return;
			}

			// Get the depth of the url relative 
			// Try to determine if this is a legacy type URL and how deep the urlrelative is
			int url_relative_depth = 0;
			string[] url_relative_info = null;
			if (QueryString["urlrelative"] != null)
			{
				string urlrewrite = QueryString["urlrelative"].ToLower();
				if (urlrewrite.Length > 0)
				{
					// Split the url relative list
					url_relative_info = urlrewrite.Split("/".ToCharArray());
					url_relative_depth = url_relative_info.Length;
				}
			}

			// For STATISTICS, handle some specific cases and enforce appropriate URLs
			if (CurrentModeCheck.Mode == Display_Mode_Enum.Statistics)
			{
				// Some submodes are off limites
				if ((CurrentModeCheck.Statistics_Type != Statistics_Type_Enum.Item_Count_Growth_View) && (CurrentModeCheck.Statistics_Type != Statistics_Type_Enum.Item_Count_Standard_View) && (CurrentModeCheck.Statistics_Type != Statistics_Type_Enum.Item_Count_Text) && (CurrentModeCheck.Statistics_Type != Statistics_Type_Enum.Usage_Definitions) && (CurrentModeCheck.Statistics_Type != Statistics_Type_Enum.Usage_Overall))
				{
					CurrentModeCheck.Statistics_Type = Statistics_Type_Enum.Usage_Overall;
					HttpContext.Current.Response.Status = "301 Moved Permanently";
					HttpContext.Current.Response.AddHeader("Location", UrlWriterHelper.Redirect_URL(CurrentModeCheck));
					HttpContext.Current.ApplicationInstance.CompleteRequest();
					currentMode.Request_Completed = true;
					return;
				}

				// Ensure the URL behaved correctly
				switch (CurrentModeCheck.Statistics_Type)
				{
					case Statistics_Type_Enum.Item_Count_Text:
					case Statistics_Type_Enum.Item_Count_Growth_View:
					case Statistics_Type_Enum.Usage_Definitions:
						if (url_relative_depth > 3)
						{
							HttpContext.Current.Response.Status = "301 Moved Permanently";
							HttpContext.Current.Response.AddHeader("Location", UrlWriterHelper.Redirect_URL(CurrentModeCheck));
							HttpContext.Current.ApplicationInstance.CompleteRequest();
							currentMode.Request_Completed = true;
							return;
						}
						break;

					case Statistics_Type_Enum.Usage_Overall:
						if (url_relative_depth > 2)
						{
							HttpContext.Current.Response.Status = "301 Moved Permanently";
							HttpContext.Current.Response.AddHeader("Location", UrlWriterHelper.Redirect_URL(CurrentModeCheck));
							HttpContext.Current.ApplicationInstance.CompleteRequest();
							currentMode.Request_Completed = true;
							return;
						}
						break;

					case Statistics_Type_Enum.Item_Count_Standard_View:
						if (url_relative_depth > 2)
						{
							HttpContext.Current.Response.Status = "301 Moved Permanently";
							HttpContext.Current.Response.AddHeader("Location", UrlWriterHelper.Redirect_URL(CurrentModeCheck));
							HttpContext.Current.ApplicationInstance.CompleteRequest();
							currentMode.Request_Completed = true;
							return;
						}
						else if (url_relative_depth == 2)
						{
							if (url_relative_info[1] != "itemcount")
							{
								HttpContext.Current.Response.Status = "301 Moved Permanently";
								HttpContext.Current.Response.AddHeader("Location", UrlWriterHelper.Redirect_URL(CurrentModeCheck));
								HttpContext.Current.ApplicationInstance.CompleteRequest();
								currentMode.Request_Completed = true;
								return;
							}
						}
						break;
				}
			}

			// For AGGREGATION HOME handle some cases
			if ((CurrentModeCheck.Mode == Display_Mode_Enum.Aggregation) && ((currentMode.Aggregation_Type == Aggregation_Type_Enum.Home) || (currentMode.Aggregation_Type == Aggregation_Type_Enum.Home_Edit)))
			{
				// Different code depending on if this is an aggregation or not
				if ((CurrentModeCheck.Aggregation.Length == 0) || (CurrentModeCheck.Aggregation == "all"))
				{
					switch (CurrentModeCheck.Home_Type)
					{
						case Home_Type_Enum.List:
							if (url_relative_depth > 0)
							{
								HttpContext.Current.Response.Status = "301 Moved Permanently";
								HttpContext.Current.Response.AddHeader("Location", UrlWriterHelper.Redirect_URL(CurrentModeCheck));
								HttpContext.Current.ApplicationInstance.CompleteRequest();
								currentMode.Request_Completed = true;
								return;
							}
							break;

						case Home_Type_Enum.Descriptions:
						case Home_Type_Enum.Tree_Collapsed:
						case Home_Type_Enum.Partners_List:
							if (url_relative_depth > 1)
							{
								HttpContext.Current.Response.Status = "301 Moved Permanently";
								HttpContext.Current.Response.AddHeader("Location", UrlWriterHelper.Redirect_URL(CurrentModeCheck));
								HttpContext.Current.ApplicationInstance.CompleteRequest();
								currentMode.Request_Completed = true;
								return;
							}
							break;

						case Home_Type_Enum.Tree_Expanded:
						case Home_Type_Enum.Partners_Thumbnails:
							if (url_relative_depth > 2)
							{
								HttpContext.Current.Response.Status = "301 Moved Permanently";
								HttpContext.Current.Response.AddHeader("Location", UrlWriterHelper.Redirect_URL(CurrentModeCheck));
								HttpContext.Current.ApplicationInstance.CompleteRequest();
								currentMode.Request_Completed = true;
								return;
							}
							break;

						case Home_Type_Enum.Personalized:
							HttpContext.Current.Response.Status = "301 Moved Permanently";
							HttpContext.Current.Response.AddHeader("Location", UrlWriterHelper.Redirect_URL(CurrentModeCheck));
							HttpContext.Current.ApplicationInstance.CompleteRequest();
							currentMode.Request_Completed = true;
							return;
					}
				}
			}

			// Ensure this is requesting the item without a viewercode and without extraneous information
			if (CurrentModeCheck.Mode == Display_Mode_Enum.Item_Display)
			{
				if ((CurrentModeCheck.ViewerCode.Length > 0) || (url_relative_depth > 2))
				{
					CurrentModeCheck.ViewerCode = String.Empty;

					HttpContext.Current.Response.Status = "301 Moved Permanently";
					HttpContext.Current.Response.AddHeader("Location", UrlWriterHelper.Redirect_URL(CurrentModeCheck));
					HttpContext.Current.ApplicationInstance.CompleteRequest();
					currentMode.Request_Completed = true;
					return;
				}
			}
		}

		#endregion

		#region Method performs user checks against session, cookie, database, etc..

		private void perform_user_checks(bool isPostBack)
		{
			// If the mode is NULL or the request was already completed, do nothing
			if ((currentMode == null) || (currentMode.Request_Completed))
				return;

			tracer.Add_Trace("SobekCM_Page_Globals.Perform_User_Checks", "In user checks portion");

			// If this is to log out of my sobekcm, clear user id and forward back to sobekcm
			if ((currentMode.Mode == Display_Mode_Enum.My_Sobek) && (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Log_Out))
			{
				tracer.Add_Trace("SobekCM_Page_Globals.Perform_User_Checks", "User logged out");

				// Delete any user cookie
				HttpCookie userCookie = new HttpCookie("SobekUser");
				userCookie.Values["userid"] = String.Empty;
				userCookie.Values["security_hash"] = String.Empty;
				userCookie.Expires = DateTime.Now.AddDays(-1);
				HttpContext.Current.Response.Cookies.Add(userCookie);

				// Delete from memory
				HttpContext.Current.Session["userid"] = 0;
				HttpContext.Current.Session["user"] = null;

				// Determine new redirect location
				string redirect = currentMode.Base_URL + currentMode.Return_URL;
				if (((currentMode.Return_URL.IndexOf("admin") >= 0) && (currentMode.Return_URL.IndexOf("admin") <= 1)) ||
				    ((currentMode.Return_URL.IndexOf("mysobek") >= 0) && (currentMode.Return_URL.IndexOf("mysobek") <= 1)))
					redirect = currentMode.Base_URL;

				HttpContext.Current.Response.Redirect(redirect, false);
				HttpContext.Current.ApplicationInstance.CompleteRequest();
				currentMode.Request_Completed = true;
				return;
			}

			// If there is already a user logged on, do nothing here
			if (HttpContext.Current.Session["user"] == null)
			{
				// If this is a responce from Shibboleth/Gatorlink, get the user information and register them if necessary
			    if ((UI_ApplicationCache_Gateway.Settings.Shibboleth != null) && (UI_ApplicationCache_Gateway.Settings.Shibboleth.Enabled))
			    {
			        string shibboleth_id = HttpContext.Current.Request.ServerVariables[UI_ApplicationCache_Gateway.Settings.Shibboleth.UserIdentityAttribute];
			        if (shibboleth_id == null)
			        {
			            if (UI_ApplicationCache_Gateway.Settings.Shibboleth.Debug)
			            {
                            tracer.Add_Trace("SobekCM_Page_Globals.Constructor", UI_ApplicationCache_Gateway.Settings.Shibboleth.UserIdentityAttribute + " server variable NOT found");

			                // For debugging purposes, if this SHOULD have included SHibboleth information, show in the trace route
			                if (HttpContext.Current.Request.Url.AbsoluteUri.Contains("shibboleth"))
			                {
			                    foreach (string var in HttpContext.Current.Request.ServerVariables)
			                    {
			                        tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Server Variables: " + var + " --> " + HttpContext.Current.Request.ServerVariables[var]);
			                    }
			                }
			            }
			        }
			        else
			        {
			            if (UI_ApplicationCache_Gateway.Settings.Shibboleth.Debug)
			            {
                            tracer.Add_Trace("SobekCM_Page_Globals.Constructor", UI_ApplicationCache_Gateway.Settings.Shibboleth.UserIdentityAttribute + " server variable found");

                            tracer.Add_Trace("SobekCM_Page_Globals.Constructor", UI_ApplicationCache_Gateway.Settings.Shibboleth.UserIdentityAttribute + " server variable = '" + shibboleth_id + "'");

			                // For debugging purposes, if this SHOULD have included SHibboleth information, show in the trace route
                            if (HttpContext.Current.Request.Url.AbsoluteUri.Contains("shibboleth"))
                            {
                                foreach (string var in HttpContext.Current.Request.ServerVariables)
                                {
                                    tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Server Variables: " + var + " --> " + HttpContext.Current.Request.ServerVariables[var]);
                                }
                            }
			            }

			            if (shibboleth_id.Length > 0)
			            {
			                tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Pulling from database by shibboleth id");

			                User_Object possible_user_by_shibboleth_id = SobekCM_Database.Get_User(shibboleth_id, tracer);

			                // Check to see if we got a valid user back
			                if (possible_user_by_shibboleth_id != null)
			                {
			                    if (UI_ApplicationCache_Gateway.Settings.Shibboleth.Debug)
			                    {
			                        // Set the user information from the server variables here 
			                        foreach (string var in HttpContext.Current.Request.ServerVariables)
			                        {
			                            User_Object_Attribute_Mapping_Enum mapping = UI_ApplicationCache_Gateway.Settings.Shibboleth.Get_User_Object_Mapping(var);
			                            if (mapping != User_Object_Attribute_Mapping_Enum.NONE)
			                            {
			                                string value = HttpContext.Current.Request.ServerVariables[var];

			                                if (UI_ApplicationCache_Gateway.Settings.Shibboleth.Debug)
			                                {
			                                    tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Server Variable " + var + " ( " + value + " ) would have been mapped to " + User_Object_Attribute_Mapping_Enum_Converter.ToString(mapping));
			                                }
			                            }
			                            else if (UI_ApplicationCache_Gateway.Settings.Shibboleth.Debug)
			                            {
			                                tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Server Variable " + var + " is not mapped to a user attribute");
			                            }
			                        }

			                        // Set any constants as well
			                        foreach (KeyValuePair<User_Object_Attribute_Mapping_Enum, string> constantMapping in UI_ApplicationCache_Gateway.Settings.Shibboleth.Constants)
			                        {
			                            if (constantMapping.Key != User_Object_Attribute_Mapping_Enum.NONE)
			                            {
			                                if (UI_ApplicationCache_Gateway.Settings.Shibboleth.Debug)
			                                {
			                                    tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Constant value ( " + constantMapping.Value + " ) would have been set to " + User_Object_Attribute_Mapping_Enum_Converter.ToString(constantMapping.Key));
			                                }
			                            }
			                        }
			                    }

			                    tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Setting session user from shibboleth id");
			                    possible_user_by_shibboleth_id.Authentication_Type = User_Authentication_Type_Enum.Shibboleth;
			                    HttpContext.Current.Session["user"] = possible_user_by_shibboleth_id;
			                }
			                else
			                {
			                    tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "User from shibboleth id was null.. adding user");

			                    // Now build the user object
			                    User_Object newUser = new User_Object();
			                    if ((HttpContext.Current.Request.ServerVariables["HTTP_PRIMARY-AFFILIATION"] != null) && (HttpContext.Current.Request.ServerVariables["HTTP_PRIMARY-AFFILIATION"].IndexOf("F") >= 0))
			                        newUser.Can_Submit = true;
			                    else
			                        newUser.Can_Submit = false;
			                    newUser.Send_Email_On_Submission = true;
			                    newUser.Email = String.Empty;
			                    newUser.Family_Name = String.Empty;
			                    newUser.Given_Name = String.Empty;
			                    newUser.Organization = String.Empty;
			                    newUser.ShibbID = shibboleth_id;
			                    newUser.UserID = -1;

			                    // Set the user information from the server variables here 
			                    foreach (string var in HttpContext.Current.Request.ServerVariables)
			                    {
			                        User_Object_Attribute_Mapping_Enum mapping = UI_ApplicationCache_Gateway.Settings.Shibboleth.Get_User_Object_Mapping(var);
			                        if (mapping != User_Object_Attribute_Mapping_Enum.NONE)
			                        {
			                            string value = HttpContext.Current.Request.ServerVariables[var];
			                            newUser.Set_Value_By_Mapping(mapping, value);

			                            if (UI_ApplicationCache_Gateway.Settings.Shibboleth.Debug)
			                            {
			                                tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Server Variable " + var + " ( " + value + " ) mapped to " + User_Object_Attribute_Mapping_Enum_Converter.ToString(mapping));
			                            }
			                        }
			                        else if (UI_ApplicationCache_Gateway.Settings.Shibboleth.Debug)
			                        {
			                            tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Server Variable " + var + " is not mapped to a user attribute");
			                        }
			                    }

			                    // Set any constants as well
			                    foreach (KeyValuePair<User_Object_Attribute_Mapping_Enum, string> constantMapping in UI_ApplicationCache_Gateway.Settings.Shibboleth.Constants)
			                    {
			                        if (constantMapping.Key != User_Object_Attribute_Mapping_Enum.NONE)
			                        {
                                        newUser.Set_Value_By_Mapping(constantMapping.Key, constantMapping.Value);

			                            if (UI_ApplicationCache_Gateway.Settings.Shibboleth.Debug)
			                            {
			                                tracer.Add_Trace("SobekCM_Page_Globals.Constructor", "Setting constant value ( " + constantMapping.Value + " ) to " + User_Object_Attribute_Mapping_Enum_Converter.ToString(constantMapping.Key));
			                            }
			                        }
			                    }

			                    // Set the username
			                    if (String.IsNullOrEmpty(newUser.UserName))
			                    {
			                        if (newUser.Email.Length > 0)
			                            newUser.UserName = newUser.Email;
			                        else
			                            newUser.UserName = newUser.Family_Name + shibboleth_id;
			                    }

			                    // Set a random password
			                    StringBuilder passwordBuilder = new StringBuilder();
			                    Random randomGenerator = new Random(DateTime.Now.Millisecond);
			                    for (int i = 0; i < 5; i++)
			                    {
			                        int randomNumber = randomGenerator.Next(97, 122);
			                        passwordBuilder.Append((char) randomNumber);

			                        int randomNumber2 = randomGenerator.Next(65, 90);
			                        passwordBuilder.Append((char) randomNumber2);
			                    }
			                    string password = passwordBuilder.ToString();

			                    // Now, save this user
			                    SobekCM_Database.Save_User(newUser, password, newUser.Authentication_Type, tracer);

			                    // Now, pull back out of the database
			                    User_Object possible_user_by_shib2 = SobekCM_Database.Get_User(shibboleth_id, tracer);
			                    possible_user_by_shib2.Is_Just_Registered = true;
			                    possible_user_by_shib2.Authentication_Type = User_Authentication_Type_Enum.Shibboleth;
			                    HttpContext.Current.Session["user"] = possible_user_by_shib2;
			                }

			                if (HttpContext.Current.Session["user"] != null)
			                {
			                    currentMode.Mode = Display_Mode_Enum.My_Sobek;
			                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
			                }
			                else
			                {
			                    currentMode.Mode = Display_Mode_Enum.Aggregation;
			                    currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
			                    currentMode.Aggregation = String.Empty;
			                }

			                if (!UI_ApplicationCache_Gateway.Settings.Shibboleth.Debug)
			                {
			                    UrlWriterHelper.Redirect(currentMode);
			                }
			            }
			        }
			    }

			    // If the user information is still missing , but the SobekUser cookie exists, then pull 
				// the user information from the SobekUser cookie in the current requests
				if ((HttpContext.Current.Session["user"] == null) && (HttpContext.Current.Request.Cookies["SobekUser"] != null))
				{
					string userid_string = HttpContext.Current.Request.Cookies["SobekUser"]["userid"];
					int userid = -1;

					bool valid_perhaps = userid_string.All(Char.IsNumber);
					if (valid_perhaps)
						Int32.TryParse(userid_string, out userid);

					if (userid > 0)
					{
						User_Object possible_user = SobekCM_Database.Get_User(userid, tracer);
						if (possible_user != null)
						{
							string cookie_security_hash = HttpContext.Current.Request.Cookies["SobekUser"]["security_hash"];
							if (cookie_security_hash == possible_user.Security_Hash(HttpContext.Current.Request.UserHostAddress))
							{
								HttpContext.Current.Session["user"] = possible_user;
							}
							else
							{
								// Security hash did not match, so clear the cookie
								HttpCookie userCookie = new HttpCookie("SobekUser");
								userCookie.Values["userid"] = String.Empty;
								userCookie.Values["security_hash"] = String.Empty;
								userCookie.Expires = DateTime.Now.AddDays(-1);
								HttpContext.Current.Response.Cookies.Add(userCookie);
							}
						}
					}
				}
			}

			// If this is not a post back, set the html writer code to 'l' or 'h' depending on whether logged on
			if (!isPostBack)
			{
				if (HttpContext.Current.Session["user"] != null)
				{
					if (currentMode.Writer_Type == Writer_Type_Enum.HTML)
					{
						// If this is really a deprecated URL, don't try to forwaard
						if ((currentMode.Mode != Display_Mode_Enum.Item_Display) || (currentMode.BibID.Length > 0) || (currentMode.ItemID_DEPRECATED <= 0))
						{
							currentMode.Writer_Type = Writer_Type_Enum.HTML_LoggedIn;
							UrlWriterHelper.Redirect(currentMode);
							return;
						}
					}
					else
					{
						if ((currentMode.Mode == Display_Mode_Enum.My_Sobek) && (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Logon))
						{
							currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
						}
					}
				}
				else
				{
					if ((currentMode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn) && (currentMode.My_Sobek_Type != My_Sobek_Type_Enum.Logon) && (currentMode.My_Sobek_Type != My_Sobek_Type_Enum.Preferences))
					{
						switch (currentMode.Mode)
						{
							case Display_Mode_Enum.My_Sobek:
								currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
								break;

							case Display_Mode_Enum.Item_Display:
								currentMode.Writer_Type = Writer_Type_Enum.HTML;
								// If this is really a deprecated URL, don't try to forwaard
								if ((currentMode.BibID.Length > 0) || (currentMode.ItemID_DEPRECATED <= 0))
								{
									UrlWriterHelper.Redirect(currentMode);
									return;
								}
								break;

							default:
								currentMode.Writer_Type = Writer_Type_Enum.HTML;
								UrlWriterHelper.Redirect(currentMode);
								return;

						}
					}

					// If this is requesting an internal page and there is no user, send to the logon page
					if (currentMode.Mode == Display_Mode_Enum.Internal)
					{
						currentMode.Mode = Display_Mode_Enum.My_Sobek;
						currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
					}
				}
			}
			else // This IS a postback
			{
				// If this is a postback from the logon page being inserted in front of the INTERNAL pages,
				// then the postback request needs to be handled by the logon page
				if ((currentMode.Mode == Display_Mode_Enum.Internal) && (HttpContext.Current.Session["user"] == null))
				{
					currentMode.Mode = Display_Mode_Enum.My_Sobek;
					currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
				}
			}

			// Set the internal DLC flag
			if (HttpContext.Current.Session["user"] != null) 
			{
				currentUser = (User_Object) HttpContext.Current.Session["user"];
				currentMode.Internal_User = currentUser.Is_Internal_User;

				// Check if this is an administrative task that the current user does not have access to
				if ((!currentUser.Is_System_Admin) && (!currentUser.Is_Portal_Admin) && (currentMode.Mode == Display_Mode_Enum.Administrative) && (currentMode.Admin_Type != Admin_Type_Enum.Aggregation_Single))
				{
                    if (currentUser.LoggedOn)
                    {
                        currentMode.Mode = Display_Mode_Enum.My_Sobek;
                        currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                    }
                    else
                    {
                        currentMode.Mode = Display_Mode_Enum.Aggregation;
                        currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
                        currentMode.Aggregation = String.Empty;
                    }
				}
			}
			else
			{
				if ((currentMode.Mode == Display_Mode_Enum.My_Sobek) && (currentMode.My_Sobek_Type != My_Sobek_Type_Enum.Preferences))
				{
					currentMode.Logon_Required = true;
				}

				if ((currentMode.Mode == Display_Mode_Enum.Aggregation) && (currentMode.Aggregation_Type == Aggregation_Type_Enum.Home) && (currentMode.Home_Type == Home_Type_Enum.Personalized))
					currentMode.Home_Type = Home_Type_Enum.List;
			}

            //// Create the empty user
            //if (currentUser == null)
            //{
            //    currentUser = new User_Object();
            //    HttpContext.Current.Session["user"] = currentUser;
            //}
		}

		#endregion

		#region Method called during Page Load

		public void On_Page_Load()
		{
			if ((currentMode != null) && (!currentMode.Request_Completed))
			{
				// If this is not a post back, log it
				if (!currentMode.isPostBack)
				{
					tracer.Add_Trace("SobekCM_Page_Globals.Constructor.On_Page_Load", String.Empty);
				}

				Set_Main_Writer();
			}
		}

		#endregion

		public void Set_Main_Writer()
		{
			// If this is for HTML or HTML logged in, try to get the web skin object
			string current_skin_code = currentMode.Skin.ToUpper();
			if ((currentMode.Writer_Type == Writer_Type_Enum.HTML) || (currentMode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
			{
				// Check if a different skin should be used if this is an item display
				if ((currentMode.Mode == Display_Mode_Enum.Item_Display) || (currentMode.Mode == Display_Mode_Enum.Item_Print))
				{
					if ((currentItem != null) && (currentItem.Behaviors.Web_Skin_Count > 0))
					{
						if (!currentItem.Behaviors.Web_Skins.Contains(current_skin_code))
						{
							string new_skin_code = currentItem.Behaviors.Web_Skins[0];
							current_skin_code = new_skin_code;
						}
					}
				}

				// Check if a differente skin should be used if this is a collection display
				if ((hierarchyObject != null) && (hierarchyObject.Web_Skins.Count > 0))
				{
					if (!hierarchyObject.Web_Skins.Contains(current_skin_code.ToLower()))
					{
						current_skin_code = hierarchyObject.Web_Skins[0];
					}
				}

				SobekCM_Assistant assistant = new SobekCM_Assistant();

				// Try to get the web skin from the cache or skin collection, otherwise build it
				htmlSkin = assistant.Get_HTML_Skin(current_skin_code, currentMode, UI_ApplicationCache_Gateway.Web_Skin_Collection, true, tracer);

				// If there was no web skin returned, forward user to URL with no web skin. 
				// This happens if the web skin code is invalid.  If a robot, just return a bad request 
				// value though.
				if (htmlSkin == null)
				{
					if ((currentMode == null) || (currentMode.Is_Robot))
					{
						HttpContext.Current.Response.StatusCode = 404;
						HttpContext.Current.Response.Output.WriteLine("404 - INVALID URL");
						HttpContext.Current.ApplicationInstance.CompleteRequest();
						currentMode.Request_Completed = true;
					}
					else
					{
						currentMode.Skin = String.Empty;
						UrlWriterHelper.Redirect(currentMode);
						return;
					}

					return;
				}
            }

            // Build the RequestCache object
		    RequestCache RequestSpecificValues = new RequestCache(currentMode, hierarchyObject, searchResultStatistics, pagedSearchResults, thisBrowseObject, currentItem, currentPage, htmlSkin, currentUser, publicFolder, siteMap, itemsInTitle, staticWebContent, tracer);

            if ((currentMode.Writer_Type == Writer_Type_Enum.HTML) || (currentMode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
            {
                mainWriter = new Html_MainWriter(RequestSpecificValues);
            }

			// Load the OAI writer
			if (currentMode.Writer_Type == Writer_Type_Enum.OAI)
			{
                mainWriter = new Oai_MainWriter(HttpContext.Current.Request.QueryString, RequestSpecificValues);
			}

			// Load the DataSet writer
			if (currentMode.Writer_Type == Writer_Type_Enum.DataSet)
			{
                mainWriter = new Dataset_MainWriter(RequestSpecificValues);
			}

			// Load the DataProvider writer
			if (currentMode.Writer_Type == Writer_Type_Enum.Data_Provider)
			{
                mainWriter = new DataProvider_MainWriter(RequestSpecificValues);
			}

			// Load the XML writer
			if (currentMode.Writer_Type == Writer_Type_Enum.XML)
			{
                mainWriter = new Xml_MainWriter(RequestSpecificValues);
			}

			// Load the JSON writer
			if (currentMode.Writer_Type == Writer_Type_Enum.JSON)
			{
                mainWriter = new Json_MainWriter(RequestSpecificValues, UI_ApplicationCache_Gateway.Settings.Image_URL);
			}

			// Load the HTML ECHO writer
			if (currentMode.Writer_Type == Writer_Type_Enum.HTML_Echo)
			{
                mainWriter = new Html_Echo_MainWriter(RequestSpecificValues, browse_info_display_text);
			}

			// Default to HTML
			if (mainWriter == null)
			{
                mainWriter = new Html_MainWriter(RequestSpecificValues);
			}
		}

		#region Block for displaying a public folder

		private void Public_Folder()
		{
			tracer.Add_Trace("SobekCM_Page_Globals.Public_Folder", "Retrieving public folder information and browse");

			SobekCM_Assistant assistant = new SobekCM_Assistant();
			bool result = assistant.Get_Public_User_Folder(currentMode.FolderID, currentMode.Page, tracer, out publicFolder, out searchResultStatistics, out pagedSearchResults);

			if ((!result) || (!publicFolder.IsPublic))
			{
				currentMode.Error_Message = "Invalid or private bookshelf";
				currentMode.Mode = Display_Mode_Enum.Error;
			}
		}

		#endregion

		#region Block for displaying a single item

		private void Display_Item()
		{
			tracer.Add_Trace("SobekCM_Page_Globals.Display_Item", "Retrieving item or group information");

			// Build the SobekCM assistant
			SobekCM_Assistant assistant = new SobekCM_Assistant();

			// If this is a robot, then get the text from the static page
			if (currentMode.Is_Robot)
			{
				string directory = currentMode.BibID.Substring(0, 2) + "/" + currentMode.BibID.Substring(2, 2) + "/" + currentMode.BibID.Substring(4, 2) + "/" + currentMode.BibID.Substring(6, 2) + "/" + currentMode.BibID.Substring(8);
				string redirect_dir = "~/data/" + directory + "/" + currentMode.BibID + "_" + currentMode.VID + ".html";
				HttpContext.Current.RewritePath(redirect_dir, true);
				currentMode.Request_Completed = true;
			}
			else
			{
                if (!assistant.Get_Item(currentMode, UI_ApplicationCache_Gateway.Items, UI_ApplicationCache_Gateway.Settings.Image_URL,
                                        UI_ApplicationCache_Gateway.Icon_List, UI_ApplicationCache_Gateway.Item_Viewer_Priority, currentUser, tracer, out currentItem, out currentPage, out itemsInTitle))
				{
					if ((currentMode.Mode == Display_Mode_Enum.Legacy_URL) || (currentMode.Invalid_Item))
					{
						if (currentMode.Mode != Display_Mode_Enum.Legacy_URL)
						{
							currentMode.Mode = Display_Mode_Enum.Error;
							currentMode.Error_Message = "Invalid Item Requested";
						}
					}
					else
					{
						Email_Information("Unable to find metadata for valid item", null);
						currentMode.Mode = Display_Mode_Enum.Aggregation;
						currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
						UrlWriterHelper.Redirect(currentMode);
						// return;
					}
				}
			}
		}

		#endregion

		#region Block for displaying simple text with an interface

		private void Simple_Web_Content_Text_Block()
		{
			tracer.Add_Trace("SobekCM_Page_Globals.Simple_Web_Content_Text_Block", "Retrieiving Simple Web Content Object");

			SobekCM_Assistant assistant = new SobekCM_Assistant();
            if (!assistant.Get_Simple_Web_Content_Text(currentMode, UI_ApplicationCache_Gateway.Settings.Base_Directory, tracer,
			                                           out staticWebContent, out siteMap))
			{
				currentMode.Mode = Display_Mode_Enum.Error;
				return;
			}

			// If the web skin is indicated in the browse file, set that
			if (staticWebContent.Web_Skin.Length > 0)
			{
				currentMode.Default_Skin = staticWebContent.Web_Skin;
				currentMode.Skin = staticWebContent.Web_Skin;
			}
		}

		#endregion

		#region Block for browsing and info

		private void Browse_Info_Block()
		{
			tracer.Add_Trace("SobekCM_Page_Globals.Browse_Info_Block", "Retrieiving Browse/Info Object");

			SobekCM_Assistant assistant = new SobekCM_Assistant();

			// If this is a robot, then get the text from the static page
			if ((currentMode.Is_Robot) && (currentMode.Info_Browse_Mode == "all"))
			{
				browse_info_display_text = assistant.Get_All_Browse_Static_HTML(currentMode, tracer);
				currentMode.Writer_Type = Writer_Type_Enum.HTML_Echo;
			}
			else
			{
                if (!assistant.Get_Browse_Info(currentMode, hierarchyObject, UI_ApplicationCache_Gateway.Settings.Base_Directory, tracer, out thisBrowseObject, out searchResultStatistics, out pagedSearchResults, out staticWebContent))
				{
					currentMode.Mode = Display_Mode_Enum.Error;
				}
			}
		}

		#endregion

		#region Block for searching

		private void Search_Block()
		{
			tracer.Add_Trace("SobekCM_Page_Globals.Search_Block", "Retreiving search results");

			try
			{
				// If there is no search term, forward back to the collection
				if ((currentMode.Search_String.Length == 0) && (currentMode.Coordinates.Length == 0))
				{
					currentMode.Mode = Display_Mode_Enum.Aggregation;
					currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
					UrlWriterHelper.Redirect(currentMode);
					return;
				}

				SobekCM_Assistant assistant = new SobekCM_Assistant();
                assistant.Get_Search_Results(currentMode, UI_ApplicationCache_Gateway.Items, hierarchyObject, UI_ApplicationCache_Gateway.Search_Stop_Words, tracer, out searchResultStatistics, out pagedSearchResults);

				if ((!currentMode.isPostBack) && (UI_ApplicationCache_Gateway.Search_History != null))
				{
                    UI_ApplicationCache_Gateway.Search_History.Add_New_Search(Get_Search_From_Mode(currentMode, HttpContext.Current.Request.UserHostAddress, currentMode.Search_Type, hierarchyObject.Name, currentMode.Search_String ));
				}
			}
			catch (Exception ee)
			{
				currentMode.Mode = Display_Mode_Enum.Error;
				currentMode.Error_Message = "Unable to perform search at this time ";
				if (hierarchyObject == null)
					currentMode.Error_Message = "Unlable to perform search - hierarchyObject = null";
				currentMode.Caught_Exception = ee;
			}
		}

	    private Recent_Searches.Search Get_Search_From_Mode(SobekCM_Navigation_Object currentMode, string SessionIP, Search_Type_Enum Search_Type, string Aggregation, string Search_Terms)
	    {
	        Recent_Searches.Search returnValue = new Recent_Searches.Search();

	        returnValue.Time = DateTime.Now.ToShortDateString().Replace("/", "-") + " " + DateTime.Now.ToShortTimeString().Replace(" ", "");

	        // Save some of the values
	        returnValue.SessionIP = SessionIP;
	        switch (Search_Type)
	        {
	            case Search_Type_Enum.Advanced:
	                returnValue.Search_Type = "Advanced";
	                break;

	            case Search_Type_Enum.Basic:
	                returnValue.Search_Type = "Basic";
	                break;

	            case Search_Type_Enum.Newspaper:
	                returnValue.Search_Type = "Newspaper";
	                break;

	            case Search_Type_Enum.Map:
	                returnValue.Search_Type = "Map";
	                break;

	            default:
	                returnValue.Search_Type = "Unknown";
	                break;
	        }

	        // Save the collection as a link
	        Display_Mode_Enum lastMode = currentMode.Mode;
	        currentMode.Mode = Display_Mode_Enum.Aggregation;
	        currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
	        returnValue.Aggregation = "<a href=\"" + UrlWriterHelper.Redirect_URL(currentMode) + "\">" + Aggregation.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a>";

	        // Save the search terms as a link to the search
	        currentMode.Mode = lastMode;
	        returnValue.Search_Terms = "<a href=\"" + UrlWriterHelper.Redirect_URL(currentMode) + "\">" + Search_Terms.Replace("&", "&amp;").Replace("\"", "&quot;") + "</a>";

            return returnValue;
	    }

	    #endregion

		#region Block for MySobek

		private void MySobekCM_Block()
		{
			if ((currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Folder_Management) && (HttpContext.Current.Session["user"] != null) && (currentMode.My_Sobek_SubMode.Length > 0))
			{
				tracer.Add_Trace("SobekCM_Page_Globals.MySobekCM_Block", "Retrieiving Browse/Info Object");

				User_Object userObj = (User_Object) HttpContext.Current.Session["user"];

				// For EXPORT option, include ALL the items
				int results_per_page = 20;
				int current_page = currentMode.Page;
				if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Export)
				{
					results_per_page = 10000;
					current_page = 1;
				}

				// Get the folder
				SobekCM_Assistant assistant = new SobekCM_Assistant();
				if (!assistant.Get_User_Folder(currentMode.My_Sobek_SubMode, userObj.UserID, results_per_page, current_page, tracer, out searchResultStatistics, out pagedSearchResults))
				{
					currentMode.Mode = Display_Mode_Enum.Error;
				}
			}
		}


		#endregion

		#region Methods retrieve groups, collections, or subcollections from the database or cache

		private void Get_Entire_Collection_Hierarchy()
		{
			// If the mode is NULL or the request was already completed, do nothing
			if ((currentMode == null) || (currentMode.Request_Completed))
				return;

			tracer.Add_Trace("SobekCM_Page_Globals.Get_Entire_Collection_Hierarchy", "Retrieving hierarchy information");

			// Check that the current aggregation code is valid
            if (!UI_ApplicationCache_Gateway.Aggregations.isValidCode(currentMode.Aggregation))
			{
				// Is there a "forward value"
                if (UI_ApplicationCache_Gateway.Collection_Aliases.ContainsKey(currentMode.Aggregation))
				{
                    currentMode.Aggregation = UI_ApplicationCache_Gateway.Collection_Aliases[currentMode.Aggregation];
				}
			}

			SobekCM_Assistant assistant = new SobekCM_Assistant();
            if (!assistant.Get_Entire_Collection_Hierarchy(currentMode, UI_ApplicationCache_Gateway.Aggregations, tracer, out hierarchyObject))
			{
				currentMode.Mode = Display_Mode_Enum.Error;
			}
		}

		#endregion

		#region Methods to reset the memory and the item cache

		private void Reset_Memory()
		{
			tracer.Add_Trace("SobekCM_Page_Globals.Reset_Memory", "Clearing cache and application of data");

			// Clear the cache
			Cached_Data_Manager.Clear_Cache();

			// Clear the application portions as well
			HttpContext.Current.Application.RemoveAll();

			// Refresh the application settings
		    UI_ApplicationCache_Gateway.ResetSettings();

            UI_ApplicationCache_Gateway.ResetAll();

			// Since this reset, send to the admin, memory management portion
			currentMode.Mode = Display_Mode_Enum.Internal;
			currentMode.Internal_Type = Internal_Type_Enum.Cache;
		}

		#endregion

		#region Method to email information during an error

		public void Email_Information(string EmailTitle, Exception ObjErr)
		{
			Email_Information(EmailTitle, ObjErr, true);
		}

		public void Email_Information(string EmailTitle, Exception ObjErr, bool Redirect)
		{
			try
			{
				StreamWriter writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\temp\\exceptions.txt", true);
				writer.WriteLine();
				writer.WriteLine("Error logged in SobekCM_Page_Globals.Email_Information ( " + DateTime.Now.ToString() + ")");
				writer.WriteLine("User Host Address: " + HttpContext.Current.Request.UserHostAddress);
				writer.WriteLine("Requested URL: " + HttpContext.Current.Request.Url);
				if (ObjErr is SobekCM_Traced_Exception)
				{
					SobekCM_Traced_Exception sobekException = (SobekCM_Traced_Exception) ObjErr;

					writer.WriteLine("Error Message: " + sobekException.InnerException.Message);
					writer.WriteLine("Stack Trace: " + ObjErr.StackTrace);
					writer.WriteLine("Error Message:" + sobekException.InnerException.StackTrace);
					writer.WriteLine();
					writer.WriteLine(sobekException.Trace_Route);
				}
				else
				{

					writer.WriteLine("Error Message: " + ObjErr.Message);
					writer.WriteLine("Stack Trace: " + ObjErr.StackTrace);
				}

				writer.WriteLine();
				writer.WriteLine("------------------------------------------------------------------");
				writer.Flush();
				writer.Close();
			}
			catch (Exception)
			{
				// Already catching errors.. nothing else to realy do here if this causes an error as well
			}

			try
			{
				// Build the error message
				string err;
				if (ObjErr != null)
				{
					err = "<b>" + HttpContext.Current.Request.UserHostAddress + "</b><br /><br />" +
					      "Error in: " + HttpContext.Current.Request.Url + "<br />" +
					      "Error Message: " + ObjErr.Message + "<br /><br />" +
					      "Stack Trace: " + ObjErr.StackTrace.Replace("\r", "<br />") + "<br /><br />";

					if (ObjErr.Message.IndexOf("Timeout expired") >= 0)
						EmailTitle = "Database Timeout Expired";
				}
				else
				{
					err = "<b>" + HttpContext.Current.Request.UserHostAddress + "</b><br /><br />" +
					      "Error in: " + HttpContext.Current.Request.Url + "<br />" +
					      "Error Message: " + EmailTitle;
				}

                SobekCM_Database.Send_Database_Email(UI_ApplicationCache_Gateway.Settings.System_Error_Email, EmailTitle, err, true, false, -1, -1);

			}
			catch (Exception)
			{
				// Already catching errors.. nothing else to realy do here if this causes an error as well
			}

			// Forward to our error message
			if (Redirect)
			{
				// Forward to our error message
                HttpContext.Current.Response.Redirect(UI_ApplicationCache_Gateway.Settings.System_Error_URL, false);
				HttpContext.Current.ApplicationInstance.CompleteRequest();
				if (currentMode != null)
					currentMode.Request_Completed = true;
			}
		}

		private void send_error_email()
		{
			try
			{
				// Start the body
				StringBuilder builder = new StringBuilder();
				builder.Append("\n\nSUBMISSION INFORMATION\n");
				builder.Append("\tDate:\t\t\t\t" + DateTime.Now.ToString() + "\n");
				builder.Append("\tIP Address:\t\t\t" + HttpContext.Current.Request.UserHostAddress + "\n");
				builder.Append("\tHost Name:\t\t\t" + HttpContext.Current.Request.UserHostName + "\n");
				builder.Append("\tBrowser:\t\t\t" + HttpContext.Current.Request.Browser.Browser + "\n");
				builder.Append("\tBrowser Platform:\t\t" + HttpContext.Current.Request.Browser.Platform + "\n");
				builder.Append("\tBrowser Version:\t\t" + HttpContext.Current.Request.Browser.Version + "\n");
				builder.Append("\tBrowser Language:\t\t");
				bool first = true;
				string[] languages = HttpContext.Current.Request.UserLanguages;
				if (languages != null)
				{
					foreach (string thisLanguage in languages)
					{
						if (first)
						{
							builder.Append(thisLanguage);
							first = false;
						}
						else
						{
							builder.Append(", " + thisLanguage);
						}
					}
				}

				builder.AppendLine("HISTORY");
				if (HttpContext.Current.Session["LastSearch"] != null)
					builder.AppendLine("\tLast Search:\t\t" + HttpContext.Current.Session["LastSearch"]);
				if (HttpContext.Current.Session["LastResults"] != null)
					builder.AppendLine("\tLast Results:\t\t" + HttpContext.Current.Session["LastResults"]);
				if (HttpContext.Current.Session["Last_Mode"] != null)
					builder.AppendLine("\tLast Mode:\t\t\t?" + HttpContext.Current.Session["Last_Mode"]);
				if (HttpContext.Current.Items.Contains("Original_URL"))
					builder.AppendLine("\tURL:\t\t\t\t" + HttpContext.Current.Items["Original_URL"]);
				else
					builder.AppendLine("\tURL:\t\t\t\t" + HttpContext.Current.Request.Url);

				// Send this email
				try
				{
                    SobekCM_Database.Send_Database_Email(UI_ApplicationCache_Gateway.Settings.System_Error_Email, "SobekCM Exception Caught  [Invalid Item Requested]", builder.ToString(), false, false, -1, -1);
				}
				catch (Exception)
				{
					// Already catching errors.. nothing else to realy do here if this causes an error as well
				}

			}
			catch (Exception)
			{
				// Already catching errors.. nothing else to realy do here if this causes an error as well
			}

			// Forward to our error message
            HttpContext.Current.Response.Redirect(UI_ApplicationCache_Gateway.Settings.System_Error_URL, false);
			HttpContext.Current.ApplicationInstance.CompleteRequest();
			if (currentMode != null)
				currentMode.Request_Completed = true;
		}

		#endregion
	}

}