#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using SobekCM.Core.Navigation;
using SobekCM.Core.Settings;
using SobekCM.Core.Users;
using SobekCM.Library.Database;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM
{
	public partial class Files : Page
	{
		private string file_url;


		protected void Page_Load(object Sender, EventArgs E)
		{
			// Pull out the http request
			HttpRequest request = HttpContext.Current.Request;

			if (String.IsNullOrEmpty(SobekCM_Database.Connection_String))
			{
				Custom_Tracer tracer = new Custom_Tracer();
				try
				{

					tracer.Add_Trace("SobekCM_Page_Globals.Constructor", String.Empty);

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
						string errorMessage;
                        if ((UI_ApplicationCache_Gateway.Settings.Database_Connection == null ) || (String.IsNullOrEmpty(UI_ApplicationCache_Gateway.Settings.Database_Connection.Connection_String)))
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
                                errorMessage = "Error connecting to the database and pulling necessary data.<br /><br />Confirm the following:<ul><li>Database connection string is correct ( " + UI_ApplicationCache_Gateway.Settings.Database_Connection.Connection_String + ")</li><li>IIS is configured correctly to use anonymous authentication</li><li>Anonymous user (or service account) is part of the sobek_users role in the database.</li></ul>";
							}
						}
						// Wrap this into the SobekCM Exception
						SobekCM_Traced_Exception newException = new SobekCM_Traced_Exception(errorMessage, ee, tracer);

						// Save this to the session state, and then forward to the dashboard
						HttpContext.Current.Session["Last_Exception"] = newException;
						HttpContext.Current.Response.Redirect("dashboard.aspx", true);
					}
					else
					{
						throw ee;
					}
				}
			}


			string bibID = null;
			string vid = null;

			// Is this a robot?  They should never get access to files this way
			if (Navigation_Object.Is_UserAgent_IP_Robot(request.UserAgent, request.UserHostAddress))
			{
				Response.Clear();
				Response.Output.WriteLine("RESTRICTED ITEM");
				return;
			}

			// Get any url rewrite which occurred
			if (Request.QueryString["urlrelative"] != null)
			{
				string urlrewrite = Request.QueryString["urlrelative"].ToLower();
				if (urlrewrite.Length > 4)
				{
					// Split the url relative list
					string[] url_relative_info = urlrewrite.Split("/".ToCharArray());
					List<string> url_relative_list = (from thisPart in url_relative_info where thisPart.Length > 0 select thisPart.ToLower()).ToList();

					// Now, look for BIBID and VID
					//if ((SobekCM_Database.Verify_Item_Lookup_Object(true, ref Global.Item_List, null)) && (Global.Item_List.Contains_BibID(url_relative_list[2].ToUpper())))
					if ((url_relative_list.Count > 2) && (url_relative_list[2].Length == 10))
					{
						// This is a BibID for an existing title with at least one public item
						bibID = url_relative_list[2].ToUpper();

						// Is the next part a VID?
						if (url_relative_list.Count > 3)
						{
							string possible_vid = url_relative_list[3].Trim().PadLeft(5, '0');
							int vid_as_int;
							if (Int32.TryParse(possible_vid, out vid_as_int))
								vid = possible_vid;
						}
					}

					// Only continue if there is a BibID / VID
					if ((!String.IsNullOrEmpty(bibID)) && (!String.IsNullOrEmpty(vid)))
					{
						// Determine the new URL
                        StringBuilder urlBuilder = new StringBuilder(UI_ApplicationCache_Gateway.Settings.Servers.Image_Server_Network + bibID.Substring(0, 2) + "\\" + bibID.Substring(2, 2) + "\\" + bibID.Substring(4, 2) + "\\" + bibID.Substring(6, 2) + "\\" + bibID.Substring(8) + "\\" + vid + "\\" + url_relative_list[4], 250);
						for (int i = 5; i < url_relative_list.Count; i++)
						{
							urlBuilder.Append("\\" + url_relative_list[i]);
						}

						file_url = urlBuilder.ToString();


						// Get the extension
						string extension = Path.GetExtension(file_url);
						if (extension != null)
						{
							// Lookup the MIME type by extension
							Mime_Type_Info mimeType = null;
							if (UI_ApplicationCache_Gateway.Mime_Types.ContainsKey(extension.ToLower()))
								mimeType = UI_ApplicationCache_Gateway.Mime_Types[extension.ToLower()];

							if ((mimeType != null) && (!mimeType.isBlocked))
							{
								// Since everything is valid, check the database
								bool isDark;
								short restrictions;
								SobekCM_Database.Get_Item_Restrictions(bibID, vid, null, out isDark, out restrictions);

								// If not DARK, and is restricted, check for access here						
								if ((!isDark) && (restrictions > 0))
								{
									// Does this user already have IP restriction mask determined?
									// Determine which IP Ranges this IP address belongs to, if not already determined.

									if (HttpContext.Current.Session["IP_Range_Membership"] == null)
									{
										int ip_mask = UI_ApplicationCache_Gateway.IP_Restrictions.Restrictive_Range_Membership(request.UserHostAddress);
										HttpContext.Current.Session["IP_Range_Membership"] = ip_mask;
									}

									int current_user_mask = Convert.ToInt32(HttpContext.Current.Session["IP_Range_Membership"]);

									// Perform bitwise comparison
									int comparison = restrictions & current_user_mask;
									if (comparison == 0)
									{
										// If the user is Shibboleth authenticated, that is okay
										User_Object possible_user = HttpContext.Current.Session["user"] as User_Object;
										if (( possible_user == null ) || ( possible_user.Authentication_Type != User_Authentication_Type_Enum.Shibboleth ))
                                            isDark = true;
									}
								}

								if (!isDark)
								{
									// Should this be forwarded for this mimetype?
									if (mimeType.shouldForward)
									{
                                        StringBuilder forwardBuilder = new StringBuilder(UI_ApplicationCache_Gateway.Settings.Servers.Image_URL + bibID.Substring(0, 2) + "/" + bibID.Substring(2, 2) + "/" + bibID.Substring(4, 2) + "/" + bibID.Substring(6, 2) + "/" + bibID.Substring(8) + "/" + vid + "/" + url_relative_list[4], 250);
										for (int i = 5; i < url_relative_list.Count; i++)
										{
											forwardBuilder.Append("/" + url_relative_list[i]);
										}
										Response.Redirect(forwardBuilder.ToString());
									}
									else
									{
										Response.Clear();
										Response.ContentType = mimeType.MIME_Type;

										string filename = file_url;

										if (File.Exists(filename))
										{
											using (FileStream sourceStream = File.OpenRead(filename))
											{
												sourceStream.CopyTo(Response.OutputStream, 32768);
											}
										}

										Response.End();
									}
								}
								else
								{
									Response.Clear();
									Response.Output.WriteLine("RESTRICTED ITEM");
								}
							}
						}
					}
				}
			}

			//public static async Task CopyToAsync(this Stream source, Stream destination)
			//{
			//    int i = 0;
			//    var buffers = new [] { new byte[0x1000], new byte[0x1000] };
			//    Task writeTask = null;
			//    while(true)
			//    {
			//        var readTask = source.ReadAsync(buffers[i], 0, buffers[i].Length))>0;
			//        if (writeTask != null) await Task.WhenAll(readTask, writeTask);
			//        int bytesRead = await readTask;
			//        if (bytesRead == 0) break;
			//        writeTask = destination.WriteAsync(buffers[i], 0, bytesRead);
			//        i ^= 1; // swap buffers
			//    }
			//}

		}
	}
}