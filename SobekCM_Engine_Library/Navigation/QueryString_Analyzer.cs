#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Web;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Navigation;
using SobekCM.Core.WebContent.Hierarchy;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Tools;

#endregion

namespace SobekCM.Engine_Library.Navigation
{
	/// <summary> Query string analyzer for the new URL query string configuration with URL rewrite </summary>
	/// <remarks> QueryString_Analyzer is a class which analyzes the query string
	/// passed to the web server along with the URL.  This determines which portion
	/// of the web application to display first, and allows users to cut and paste
	/// a particular search or map. </remarks>
	public static class QueryString_Analyzer 
	{
		#region Constructor

		#endregion

		#region iSobekCM_QueryString_Analyzer Members

	    /// <summary> Parse the query and set the internal variables </summary>
	    /// <param name="QueryString"> QueryString collection passed from the main page </param>
	    /// <param name="Navigator"> Navigation object to hold the mode information </param>
	    /// <param name="Base_URL">Requested base URL (without query string, etc..)</param>
	    /// <param name="User_Languages"> Languages preferred by user, per their browser settings </param>
	    /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections </param>
	    /// <param name="Aggregation_Aliases"> List of all existing aliases for existing aggregationPermissions</param>
	    /// <param name="All_Items_Lookup"> [REF] Lookup object used to pull basic information about any item loaded into this library</param>
	    /// <param name="URL_Portals"> List of all web portals into this system </param>
	    /// <param name="WebHierarchy"> Hierarchy of all non-aggregational web content pages and redirects </param>
	    /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
	    public static void Parse_Query(NameValueCollection QueryString,
			Navigation_Object Navigator,
			string Base_URL,
			string[] User_Languages,
			Aggregation_Code_Manager Code_Manager,
			Dictionary<string, string> Aggregation_Aliases,
			Item_Lookup_Object All_Items_Lookup,
			Portal_List URL_Portals,
            WebContent_Hierarchy WebHierarchy,
			Custom_Tracer Tracer )
		{
		    if (Tracer != null)
		        Tracer.Add_Trace("QueryString_Analyzer.Parse_Query", "Parse the query into the provided Navigation_Object");

			// Set default mode to error
			Navigator.Mode = Display_Mode_Enum.Error;

			// If this has 'verb' then this is an OAI-PMH request
			if ( QueryString["verb"] != null )
			{
				Navigator.Writer_Type = Writer_Type_Enum.OAI;
				return;
			}

			// Is there a TOC state set?
			if (QueryString["toc"] != null)
			{
				if (QueryString["toc"] == "y")
				{
					Navigator.TOC_Display = TOC_Display_Type_Enum.Show;
				}
				else if ( QueryString["toc"] == "n" )
				{
					Navigator.TOC_Display = TOC_Display_Type_Enum.Hide;
				}
			}

			// Determine the default language, per the browser settings
			if (User_Languages != null)
			{
				foreach (string thisLanguage in User_Languages)
				{
					if (thisLanguage.IndexOf("en") == 0)
					{
						Navigator.Default_Language = Web_Language_Enum.English;
						break;
					}

					if (thisLanguage.IndexOf("fr") == 0)
					{
						Navigator.Default_Language = Web_Language_Enum.French;
						break;
					}

					if (thisLanguage.IndexOf("es") == 0)
					{
						Navigator.Default_Language = Web_Language_Enum.Spanish;
						break;
					}
				}
			}

			// Is there a language defined?  If so, load right into the navigator
			Navigator.Language = Navigator.Default_Language;
			if ( !String.IsNullOrEmpty(QueryString["l"]))
			{
				Navigator.Language = Web_Language_Enum_Converter.Code_To_Enum(QueryString["l"]);
			}

			// If there is flag indicating to show the trace route, save it
			if (QueryString["trace"] != null)
			{
				Navigator.Trace_Flag = QueryString["trace"].ToUpper() == "NO" ? Trace_Flag_Type_Enum.No : Trace_Flag_Type_Enum.Explicit;
			}
			else
			{
				Navigator.Trace_Flag = Trace_Flag_Type_Enum.Unspecified;
			}

			// Did the user request to have it render like it would for a search robot?
			if (QueryString["robot"] != null)
			{
				Navigator.Is_Robot = true;
			}

            // Was a fragment specified in the query string?
            if (QueryString["fragment"] != null)
            {
                Navigator.Fragment = QueryString["fragment"];
            }

            // Get the valid URL Portal
            Navigator.Default_Aggregation = "all";
            Portal urlPortal = URL_Portals.Get_Valid_Portal(Base_URL);
            Navigator.Instance_Abbreviation = urlPortal.Abbreviation;
            Navigator.Instance_Name = urlPortal.Name;
            if ( !String.IsNullOrEmpty(urlPortal.Base_PURL ))
                Navigator.Portal_PURL = urlPortal.Base_PURL;
		    if (String.IsNullOrEmpty(urlPortal.Default_Aggregation))
		    {
		        Navigator.Aggregation = "";
		    }
		    else
		    {
                Navigator.Default_Aggregation = urlPortal.Default_Aggregation;
                Navigator.Aggregation = urlPortal.Default_Aggregation; 
		    }
            if (!String.IsNullOrEmpty(urlPortal.Default_Web_Skin))
            {
                Navigator.Default_Skin = urlPortal.Default_Web_Skin;
                Navigator.Skin = urlPortal.Default_Web_Skin;
                Navigator.Skin_In_URL = false;
            }

            // Collect the interface string
            if (QueryString["n"] != null)
            {
                string currSkin = QueryString["n"].ToLower().Replace("'", "");

                // Save the interface
                if (currSkin.Length > 0)
                {
                    if (currSkin.IndexOf(",") > 0)
                        currSkin = currSkin.Substring(0, currSkin.IndexOf(","));
                    Navigator.Skin = currSkin.ToLower();
                    Navigator.Skin_In_URL = true;
                }
            }
			
			// Parse URL request different now, depending on if this is a legacy URL type or the new URL type
			Navigator.Mode = Display_Mode_Enum.None;

			// CHECK FOR LEGACY VALUES 
			// Check for legacy bibid / vid information, since this will be supported indefinitely
			if (( QueryString["b"] != null ) || ( QueryString["bib"] != null ))
			{
				Navigator.BibID = QueryString["b"] ?? QueryString["bib"];

				if ( Navigator.BibID.Length > 0 )
				{
					Navigator.Mode = Display_Mode_Enum.Item_Display;

					if ( QueryString["v"] != null )
						Navigator.VID = QueryString["v"];
					else if ( QueryString["vid"] != null )
						Navigator.VID = QueryString["vid"];                
				}

				// No other item information is collected here anymore.. just return
				return;
			}


			// Set the default mode
			Navigator.Mode = Display_Mode_Enum.Aggregation;
			Navigator.Aggregation_Type = Aggregation_Type_Enum.Home;
			Navigator.Home_Type = Home_Type_Enum.List;

			// Get any url rewrite which occurred
			if (QueryString["urlrelative"] != null)
			{
				string urlrewrite = QueryString["urlrelative"].ToLower();
				if (urlrewrite.Length > 0)
				{
					// Split the url relative list
					string[] url_relative_info = urlrewrite.Split("/".ToCharArray());
					List<string> url_relative_list = (from thisPart in url_relative_info where thisPart.Length > 0 select thisPart.ToLower()).ToList();

					// Determine the main writer (html, json, xml, oai-pmh, etc..)
					Navigator.Writer_Type = Writer_Type_Enum.HTML;
					if ( url_relative_list.Count > 0 )
					{
						switch (url_relative_list[0])
						{
							case "l":
								Navigator.Writer_Type = Writer_Type_Enum.HTML_LoggedIn;
								url_relative_list.RemoveAt(0);
								break;

							case "my":
								Navigator.Writer_Type = Writer_Type_Enum.HTML_LoggedIn;
								break;

							case "json":
								Navigator.Writer_Type = Writer_Type_Enum.JSON;
								url_relative_list.RemoveAt(0);
								break;

							case "dataset":
								Navigator.Writer_Type = Writer_Type_Enum.DataSet;
								url_relative_list.RemoveAt(0);
								break;

							case "dataprovider":
								Navigator.Writer_Type = Writer_Type_Enum.Data_Provider;
								url_relative_list.RemoveAt(0);
								break;

							case "xml":
								Navigator.Writer_Type = Writer_Type_Enum.XML;
								url_relative_list.RemoveAt(0);
								break;

							case "textonly":
								Navigator.Writer_Type = Writer_Type_Enum.Text;
								url_relative_list.RemoveAt(0);
								break;
						}
					}

					// Is the first part of the list one of these constants?
					if (( url_relative_list.Count > 0 ) && ( url_relative_list[0].Length > 0 ))
					{
						switch ( url_relative_list[0] )
						{
							case "shibboleth":
								Navigator.Mode = Display_Mode_Enum.My_Sobek;
								Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Shibboleth_Landing;
								break;

							case "internal":
								Navigator.Mode = Display_Mode_Enum.Internal;
								Navigator.Internal_Type = Internal_Type_Enum.Aggregations_List;
								if ( url_relative_list.Count > 1 )
								{
									switch( url_relative_list[1] )
									{
										case "aggregations":
											Navigator.Internal_Type = Internal_Type_Enum.Aggregations_List;
											if (url_relative_list.Count > 2)
											{
												if (url_relative_list[2] == "tree")
												{
												    Navigator.Internal_Type = Internal_Type_Enum.Aggregations_Tree;
												}
											}
											break;

										case "cache":
											Navigator.Internal_Type = Internal_Type_Enum.Cache;
											break;

										case "new":
											Navigator.Internal_Type = Internal_Type_Enum.New_Items;
											if (url_relative_list.Count > 2)
											{
												Navigator.Info_Browse_Mode = url_relative_list[2];
											}
											break;

										case "failures":
											Navigator.Internal_Type = Internal_Type_Enum.Build_Failures;
											if (url_relative_list.Count > 2)
												Navigator.Info_Browse_Mode = url_relative_list[2];
											break;
												
										case "wordmarks":
											Navigator.Internal_Type = Internal_Type_Enum.Wordmarks;
											break;
									}
								}
								break;
					 

							case "contact":
								Navigator.Mode = Display_Mode_Enum.Contact;
								if ( url_relative_list.Count > 1 )
								{
									if (url_relative_list[1] == "sent")
									{
										Navigator.Mode = Display_Mode_Enum.Contact_Sent;
									}
									else
									{
										Navigator.Aggregation = url_relative_list[1];
									}
								}
								if (QueryString["em"] != null)
									Navigator.Error_Message = QueryString["em"];
								break;
					  
							case "folder":
								Navigator.Mode = Display_Mode_Enum.Public_Folder;
								if (url_relative_list.Count >= 2)
								{
									try
									{
										Navigator.FolderID = Convert.ToInt32(url_relative_list[1]);
									}
									catch
									{
										Navigator.FolderID = -1;
									}

									// Look for result display type
									if (url_relative_list.Count >=3 )
									{
										switch (url_relative_list[2])
										{
											case "brief":
												Navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;
												break;
											case "export":
												Navigator.Result_Display_Type = Result_Display_Type_Enum.Export;
												break;
											case "citation":
												Navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Citation;
												break;
											case "image":
												Navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Image;
												break;
											case "map":
												Navigator.Result_Display_Type = Result_Display_Type_Enum.Map;
												break;
                                            case "mapbeta":
                                                Navigator.Result_Display_Type = Result_Display_Type_Enum.Map_Beta;
                                                break;
											case "table":
												Navigator.Result_Display_Type = Result_Display_Type_Enum.Table;
												break;
											case "thumbs":
												Navigator.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
												break;
											default: 
												Navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;
												break;
										}
									}


									// Look for a page number
									if (url_relative_list.Count >= 4 )
									{
										string possible_page = url_relative_list[3];
										if ((possible_page.Length > 0) && (is_String_Number(possible_page)))
										{
											ushort page_result;
											UInt16.TryParse(possible_page, out page_result);
											Navigator.Page = page_result;
										}
									}
								}
								break;

                            case "register":
                                Navigator.Mode = Display_Mode_Enum.My_Sobek;
                                Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
                                break;
										   

							case "my":
								Navigator.Mode = Display_Mode_Enum.My_Sobek;
                                Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Home;
								if (QueryString["return"] != null)
									Navigator.Return_URL = QueryString["return"];
								if ( url_relative_list.Count > 1 )
								{
									switch( url_relative_list[1] )
									{
										case "logon":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
											if (QueryString["return"] != null)
												Navigator.Return_URL = QueryString["return"];
											break;

										case "home":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Home;
											if (QueryString["return"] != null)
												Navigator.Return_URL = QueryString["return"];
											break;

										case "delete":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Delete_Item;
											if (url_relative_list.Count > 2)
												Navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												Navigator.VID = url_relative_list[3];
											break;

										case "submit":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.New_Item;
											if (url_relative_list.Count > 2)
												Navigator.My_Sobek_SubMode = url_relative_list[2];
											break;

                                        case "teisubmit":
                                            Navigator.My_Sobek_Type = My_Sobek_Type_Enum.New_TEI_Item;
                                            if (url_relative_list.Count > 2)
                                                Navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "itempermissions":
                                            Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Permissions;
                                            if (url_relative_list.Count > 2)
                                                Navigator.BibID = url_relative_list[2].ToUpper();
                                            if (url_relative_list.Count > 3)
                                                Navigator.VID = url_relative_list[3];
                                            if (url_relative_list.Count > 4)
                                                Navigator.My_Sobek_SubMode = url_relative_list[4];
                                            break;

										case "behaviors":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Behaviors;
											if (url_relative_list.Count > 2)
												Navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												Navigator.VID = url_relative_list[3];
											if (url_relative_list.Count > 4)
												Navigator.My_Sobek_SubMode = url_relative_list[4];
											break;

										case "edit":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Metadata;
											if (url_relative_list.Count > 2)
												Navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												Navigator.VID = url_relative_list[3];
											if (url_relative_list.Count > 4)
												Navigator.My_Sobek_SubMode = url_relative_list[4];
											break;

                                        case "teiedit":
                                            Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Edit_TEI_Item;
                                            if (url_relative_list.Count > 2)
                                                Navigator.BibID = url_relative_list[2].ToUpper();
                                            if (url_relative_list.Count > 3)
                                                Navigator.VID = url_relative_list[3];
                                            if (url_relative_list.Count > 4)
                                                Navigator.My_Sobek_SubMode = url_relative_list[4];
                                            break;

										case "files":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.File_Management;
											if (url_relative_list.Count > 2)
												Navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												Navigator.VID = url_relative_list[3];
											if (url_relative_list.Count > 4)
												Navigator.My_Sobek_SubMode = url_relative_list[4];
											break;

                                        case "images":
                                            Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Page_Images_Management;
                                            if (url_relative_list.Count > 2)
                                                Navigator.BibID = url_relative_list[2].ToUpper();
                                            if (url_relative_list.Count > 3)
                                                Navigator.VID = url_relative_list[3];
                                            if (url_relative_list.Count > 4)
                                                Navigator.My_Sobek_SubMode = url_relative_list[4];
                                            break;


										case "addvolume":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Group_Add_Volume;
											if (url_relative_list.Count > 2)
												Navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												Navigator.My_Sobek_SubMode = url_relative_list[3];
											break;

										case "autofill":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Group_AutoFill_Volumes;
											if (url_relative_list.Count > 2)
												Navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												Navigator.My_Sobek_SubMode = url_relative_list[3];
											break;

										case "massupdate":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Group_Mass_Update_Items;
											if (url_relative_list.Count > 2)
												Navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												Navigator.My_Sobek_SubMode = url_relative_list[3];
											break;

										case "groupbehaviors":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Group_Behaviors;
											if (url_relative_list.Count > 2)
												Navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												Navigator.My_Sobek_SubMode = url_relative_list[3];
											break;

										case "serialhierarchy":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Group_Serial_Hierarchy;
											if (url_relative_list.Count > 2)
												Navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												Navigator.My_Sobek_SubMode = url_relative_list[3];
											break;

										case "bookshelf":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
											Navigator.Result_Display_Type = Result_Display_Type_Enum.Bookshelf;
											if (url_relative_list.Count > 2)
											{
												Navigator.My_Sobek_SubMode = url_relative_list[2];
												if (url_relative_list.Count > 3)
												{
													switch (Navigator.My_Sobek_SubMode)
													{
														case "brief":
															Navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;
															Navigator.My_Sobek_SubMode = url_relative_list[3];
															break;

														case "export":
															Navigator.Result_Display_Type = Result_Display_Type_Enum.Export;
															Navigator.My_Sobek_SubMode = url_relative_list[3];
															break;

														case "thumbs":
															Navigator.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
															Navigator.My_Sobek_SubMode = url_relative_list[3];
															break;

														case "table":
															Navigator.Result_Display_Type = Result_Display_Type_Enum.Table;
															Navigator.My_Sobek_SubMode = url_relative_list[3];
															break;

														case "citation":
															Navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Citation;
															Navigator.My_Sobek_SubMode = url_relative_list[3];
															break;

														case "image":
															Navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Image;
															Navigator.My_Sobek_SubMode = url_relative_list[3];
															break;

														default:
															if (is_String_Number(url_relative_list[3]))
															{
																ushort page_result;
																UInt16.TryParse(url_relative_list[3], out page_result);
																Navigator.Page = page_result;
															}
															break;
													}
												}
												if ((url_relative_list.Count > 4) && ( is_String_Number( url_relative_list[4] )))
												{
													ushort page_result;
													UInt16.TryParse(url_relative_list[4], out page_result);
													Navigator.Page = page_result;
												}
											}
											break;

										case "preferences":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
											break;

										case "logout":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Log_Out;
											if (QueryString["return"] != null)
												Navigator.Return_URL = QueryString["return"];
											break;

										case "shibboleth":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Shibboleth_Landing;
											if (QueryString["return"] != null)
												Navigator.Return_URL = QueryString["return"];
											break;

                                        case "itemtracking":
                                            Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Item_Tracking;
                                            //if(url_relative_list.Count>3 && is_String_Number(url_relative_list[3]))
                                            //    Navigator.My_Sobek_SubMode = url_relative_list[3];
                                            break;

										case "searches":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.Saved_Searches;
											break;

										case "tags":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.User_Tags;
											if (url_relative_list.Count > 2)
												Navigator.My_Sobek_SubMode = url_relative_list[2];
											break;

										case "stats":
											Navigator.My_Sobek_Type = My_Sobek_Type_Enum.User_Usage_Stats;
											if (url_relative_list.Count > 2)
												Navigator.My_Sobek_SubMode = url_relative_list[2];
											break;
									}
								}
								break;

                            case "admin":
                                Navigator.Mode = Display_Mode_Enum.Administrative;
						        Navigator.Admin_Type = Admin_Type_Enum.Home;
                                if (QueryString["return"] != null)
                                    Navigator.Return_URL = QueryString["return"];
                                if (url_relative_list.Count > 1)
                                {
                                    switch (url_relative_list[1])
                                    {
                                        case "builder":
                                            Navigator.Admin_Type = Admin_Type_Enum.Builder_Status;
                                            if (url_relative_list.Count > 2)
                                                Navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "addcoll":
                                            Navigator.Admin_Type = Admin_Type_Enum.Add_Collection_Wizard;
                                            if (url_relative_list.Count > 2)
                                                Navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "aggregations":
                                            Navigator.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
                                            if (url_relative_list.Count > 2)
                                                Navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "editaggr":
                                            Navigator.Admin_Type = Admin_Type_Enum.Aggregation_Single;
                                            if (url_relative_list.Count > 2)
                                                Navigator.Aggregation = url_relative_list[2];
                                            if (url_relative_list.Count > 3)
                                                Navigator.My_Sobek_SubMode = url_relative_list[3];
                                            break;

                                        case "aliases":
                                            Navigator.Admin_Type = Admin_Type_Enum.Aliases;
                                            break;

                                        case "webskins":
                                            Navigator.Admin_Type = Admin_Type_Enum.Skins_Mgmt;
                                            break;

                                        case "editskin":
                                            Navigator.Admin_Type = Admin_Type_Enum.Skins_Single;
                                            if (url_relative_list.Count > 2)
                                                Navigator.My_Sobek_SubMode = url_relative_list[2];
                                            if (url_relative_list.Count > 3)
                                                Navigator.My_Sobek_SubMode = url_relative_list[2] + "/" + url_relative_list[3];
                                            break;

                                        case "defaults":
                                            Navigator.Admin_Type = Admin_Type_Enum.Default_Metadata;
                                            if (url_relative_list.Count > 2)
                                                Navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "restrictions":
                                            Navigator.Admin_Type = Admin_Type_Enum.IP_Restrictions;
                                            if (url_relative_list.Count > 2)
                                                Navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "portals":
                                            Navigator.Admin_Type = Admin_Type_Enum.URL_Portals;
                                            break;

                                        case "users":
                                            Navigator.Admin_Type = Admin_Type_Enum.Users;
                                            if (url_relative_list.Count > 2)
                                                Navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "groups":
                                            Navigator.Admin_Type = Admin_Type_Enum.User_Groups;
                                            if (url_relative_list.Count > 2)
                                                Navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "permissions":
                                            Navigator.Admin_Type = Admin_Type_Enum.User_Permissions_Reports;
                                            if (url_relative_list.Count > 2)
                                                Navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "webadd":
                                            Navigator.Admin_Type = Admin_Type_Enum.WebContent_Add_New;
                                            if (url_relative_list.Count > 2)
                                                Navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "webcontent":
                                            Navigator.Admin_Type = Admin_Type_Enum.WebContent_Mgmt;
                                            if (url_relative_list.Count > 2)
                                                Navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "webhistory":
                                            Navigator.Admin_Type = Admin_Type_Enum.WebContent_History;
                                            break;

                                        case "websingle":
                                            Navigator.Admin_Type = Admin_Type_Enum.WebContent_Single;
                                            if (url_relative_list.Count > 2)
                                            {
                                                int possiblewebid;
                                                if (Int32.TryParse(url_relative_list[2], out possiblewebid))
                                                {
                                                    Navigator.WebContentID = possiblewebid;
                                                }
                                                if (url_relative_list.Count > 3)
                                                {
                                                    Navigator.My_Sobek_SubMode = url_relative_list[3];
                                                }
                                            }
                                            if ( Navigator.WebContentID < 1 )
                                                Navigator.Admin_Type = Admin_Type_Enum.WebContent_Mgmt;
                                            break;
                                            
                                        case "webusage":
                                            Navigator.Admin_Type = Admin_Type_Enum.WebContent_Usage;
                                            break;

                                        case "wordmarks":
                                            Navigator.Admin_Type = Admin_Type_Enum.Wordmarks;
                                            break;

                                        case "reset":
                                            Navigator.Admin_Type = Admin_Type_Enum.Reset;
                                            break;

                                        case "headings":
                                            Navigator.Admin_Type = Admin_Type_Enum.Thematic_Headings;
                                            if (url_relative_list.Count > 2)
                                                Navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "tei":
                                            Navigator.Admin_Type = Admin_Type_Enum.TEI;
                                            if (url_relative_list.Count > 2)
                                                Navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "settings":
                                            Navigator.Admin_Type = Admin_Type_Enum.Settings;
                                            Navigator.Remaining_Url_Segments = copy_remaining_segments_as_array(url_relative_list, 2);
                                            break;

                                        case "builderfolder":
                                            Navigator.Admin_Type = Admin_Type_Enum.Builder_Folder_Mgmt;
                                            Navigator.Remaining_Url_Segments = copy_remaining_segments_as_array(url_relative_list, 2);
                                            break;
                                    }
                                }
                                break;
					 
							case "preferences":
								Navigator.Mode = Display_Mode_Enum.Preferences;
								break;

							case "reports":
								Navigator.Mode = Display_Mode_Enum.Reports;
								if (url_relative_list.Count > 1)
								{
									Navigator.Report_Name = url_relative_list[1];
								}
								break;

							case "stats":
							case "statistics":
								Navigator.Mode = Display_Mode_Enum.Statistics;
						        Navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Standard_View;
								if ( url_relative_list.Count > 1 )
								{
									switch( url_relative_list[1] )
									{
										case "itemcount":
											Navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Standard_View;
											if ( url_relative_list.Count > 2 )
											{
												switch( url_relative_list[2])
												{
													case "arbitrary":
														Navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Arbitrary_View;
														if (url_relative_list.Count > 3)
														{
															Navigator.Info_Browse_Mode = url_relative_list[3];
														}
														break;

													case "growth":
														Navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Growth_View;
														break;
												
													case "text":
														Navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Text;
														break;

													case "standard":
														Navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Standard_View;
														break;
												}
											}
											break;

										case "searches":
											Navigator.Statistics_Type = Statistics_Type_Enum.Recent_Searches;
											break;

										case "usage":
											Navigator.Statistics_Type = Statistics_Type_Enum.Usage_Overall;
											if ( url_relative_list.Count > 2 )
											{
												switch( url_relative_list[2])
												{
													case "all":
														Navigator.Statistics_Type = Statistics_Type_Enum.Usage_Overall;
														break;
												
													case "history":
														Navigator.Statistics_Type = Statistics_Type_Enum.Usage_Collection_History;
														if ( url_relative_list.Count > 3 )
														{
															switch( url_relative_list[3] )
															{
																case "text":
																	Navigator.Statistics_Type = Statistics_Type_Enum.Usage_Collection_History_Text;
																	break;

																default:
																	Navigator.Info_Browse_Mode = url_relative_list[3];
																	break;
															}

															if ((String.IsNullOrEmpty(Navigator.Info_Browse_Mode)) && (url_relative_list.Count > 4))
																Navigator.Info_Browse_Mode = url_relative_list[4];
														}
														break;

													case "collections":
														Navigator.Statistics_Type = Statistics_Type_Enum.Usage_Collections_By_Date;
														if (url_relative_list.Count > 3)
															Navigator.Info_Browse_Mode = url_relative_list[3];
														break;

													case "definitions":
														Navigator.Statistics_Type = Statistics_Type_Enum.Usage_Definitions;
														break;

													case "titles":
														Navigator.Statistics_Type = Statistics_Type_Enum.Usage_Titles_By_Collection;
														if (( String.IsNullOrEmpty( Navigator.Info_Browse_Mode )) && ( url_relative_list.Count > 4 ))
															Navigator.Info_Browse_Mode = url_relative_list[4];
														break;

													case "items":
														Navigator.Statistics_Type = Statistics_Type_Enum.Usage_Item_Views_By_Date;
														if ( url_relative_list.Count > 3 )
														{
															switch( url_relative_list[3] )
															{
																case "date":
																	Navigator.Statistics_Type = Statistics_Type_Enum.Usage_Item_Views_By_Date;
																	break;

																case "top":
																	Navigator.Statistics_Type = Statistics_Type_Enum.Usage_Items_By_Collection;
																	break;

																case "text":
																	Navigator.Statistics_Type = Statistics_Type_Enum.Usage_By_Date_Text;
																	break;

																default:
																	Navigator.Info_Browse_Mode = url_relative_list[3];
																	break;
															}

															if (( String.IsNullOrEmpty( Navigator.Info_Browse_Mode )) && ( url_relative_list.Count > 4 ))
																Navigator.Info_Browse_Mode = url_relative_list[4];
														}
														break;

												}
											}
											break;
									}
								}
								break;

							case "partners":
								if (( String.IsNullOrEmpty(Navigator.Default_Aggregation)) || ( Navigator.Default_Aggregation == "all"))
								{
									Navigator.Mode = Display_Mode_Enum.Aggregation;
									Navigator.Aggregation_Type = Aggregation_Type_Enum.Home;
									Navigator.Aggregation = String.Empty;
									Navigator.Home_Type = Home_Type_Enum.Partners_List;
									if ((url_relative_list.Count > 1) && (url_relative_list[1] == "thumbs"))
									{
										Navigator.Home_Type = Home_Type_Enum.Partners_Thumbnails;
									}
								}
								else
								{
									aggregation_querystring_analyze(Navigator, QueryString, Navigator.Default_Aggregation, url_relative_list);
								}
								break;

							case "tree":
								Navigator.Mode = Display_Mode_Enum.Aggregation;
								Navigator.Aggregation_Type = Aggregation_Type_Enum.Home;
								Navigator.Aggregation = String.Empty;
								Navigator.Home_Type = Home_Type_Enum.Tree;
								break;

							case "brief":
								Navigator.Mode = Display_Mode_Enum.Aggregation;
								Navigator.Aggregation_Type = Aggregation_Type_Enum.Home;
								Navigator.Aggregation = String.Empty;
								Navigator.Home_Type = Home_Type_Enum.Descriptions;
								break;

							case "personalized":
								Navigator.Mode = Display_Mode_Enum.Aggregation;
								Navigator.Aggregation_Type = Aggregation_Type_Enum.Home;
								Navigator.Aggregation = String.Empty;
								Navigator.Home_Type = Home_Type_Enum.Personalized;
								break;


                            case "inprocess":
                                Navigator.Aggregation = String.Empty;
                                Navigator.Mode = Display_Mode_Enum.Aggregation;
                                Navigator.Aggregation_Type = Aggregation_Type_Enum.Home;
                                Navigator.Aggregation_Type = Aggregation_Type_Enum.Private_Items;
                                Navigator.Page = 1;
                                if (url_relative_list.Count > 1)
                                {
                                    if (is_String_Number(url_relative_list[1]))
                                        Navigator.Page = Convert.ToUInt16(url_relative_list[1]);
                                }
                                if ((QueryString["o"] != null) && (is_String_Number(QueryString["o"])))
                                {
                                    Navigator.Sort = Convert.ToInt16(QueryString["o"]);
                                }
                                else
                                {
                                    Navigator.Sort = 0;
                                }
                                break;

							case "all":
							case "new":
							case "edit":
							case "map":
                            case "mapbeta":
                            case "advanced":
							case "text":
							case "results":
							case "contains":
							case "exact":
							case "resultslike":
							case "browseby":
							case "info":
                            case "aggrmanage":
                            case "aggrhistory":
                            case "aggrpermissions":
                            case "geography":
								aggregation_querystring_analyze(Navigator, QueryString, Navigator.Default_Aggregation, url_relative_list);
								break;

							// This was none of the main constant mode settings,
							default:
                                // Always check the top-level static web content pages and redirects hierarchy first
						        if ((WebHierarchy != null) && (WebHierarchy.Root_Count > 0))
						        {
						            WebContent_Hierarchy_Node matchedNode = WebHierarchy.Find(url_relative_list);
						            if (matchedNode != null)
						            {
                                        // Maybe this is a web content / info page
                                        Navigator.Mode = Display_Mode_Enum.Simple_HTML_CMS;

                                        // Get the URL reassembled
                                        string possible_info_mode = String.Empty;
                                        if (url_relative_list.Count == 1)
                                            possible_info_mode = url_relative_list[0];
                                        else if (url_relative_list.Count == 2)
                                            possible_info_mode = url_relative_list[0] + "/" + url_relative_list[1];
                                        else if (url_relative_list.Count == 3)
                                            possible_info_mode = url_relative_list[0] + "/" + url_relative_list[1] + "/" + url_relative_list[2];
                                        else if (url_relative_list.Count == 4)
                                            possible_info_mode = url_relative_list[0] + "/" + url_relative_list[1] + "/" + url_relative_list[2] + "/" + url_relative_list[3];
                                        else if (url_relative_list.Count > 4)
                                        {
                                            StringBuilder possibleInfoModeBuilder = new StringBuilder();
                                            if (url_relative_list.Count > 0)
                                            {
                                                possibleInfoModeBuilder.Append(url_relative_list[0]);
                                            }
                                            for (int i = 1; i < url_relative_list.Count; i++)
                                            {
                                                possibleInfoModeBuilder.Append("/" + url_relative_list[i]);
                                            }
                                            possible_info_mode = possibleInfoModeBuilder.ToString().Replace("'", "").Replace("\"", "");
                                        }

                                        // Set the source location
                                        Navigator.Info_Browse_Mode = possible_info_mode;
                                        Navigator.Page_By_FileName = Engine_ApplicationCache_Gateway.Settings.Servers.Base_Directory + "design\\webcontent\\" + possible_info_mode.Replace("/","\\") + "\\default.html";
						                Navigator.WebContentID = matchedNode.WebContentID;
						                Navigator.Redirect = matchedNode.Redirect;

                                        //// If it is missing, mark that
                                        //if ((!File.Exists(Navigator.Page_By_FileName)) && ( String.IsNullOrEmpty(Navigator.Redirect)))
                                        //{
                                        //    Navigator.Missing = true;
                                        //    Navigator.Info_Browse_Mode = possible_info_mode;
                                        //    Navigator.Page_By_FileName = Engine_ApplicationCache_Gateway.Settings.Servers.Base_Directory + "design\\webcontent\\missing.html";
                                        //}

                                        // If something was found, then check for submodes
                                        Navigator.WebContent_Type = WebContent_Type_Enum.Display;
						                if (!String.IsNullOrEmpty(QueryString["mode"]))
						                {
						                    switch (QueryString["mode"].ToLower())
						                    {
						                        case "edit":
						                            Navigator.WebContent_Type = WebContent_Type_Enum.Edit;
						                            break;

						                        case "menu":
						                            Navigator.WebContent_Type = WebContent_Type_Enum.Manage_Menu;
						                            break;

						                        case "milestones":
						                            Navigator.WebContent_Type = WebContent_Type_Enum.Milestones;
						                            break;

						                        case "permissions":
						                            Navigator.WebContent_Type = WebContent_Type_Enum.Permissions;
						                            break;

						                        case "usage":
						                            Navigator.WebContent_Type = WebContent_Type_Enum.Usage;
						                            break;

						                        case "verify":
						                            Navigator.WebContent_Type = WebContent_Type_Enum.Delete_Verify;
						                            break;
						                    }
						                }

                                        return;
						            }
						        }

								// Check to see if the first term was an item aggregation alias, which
								// allows for the alias to overwrite an existing aggregation code (limited usability
								// but can be used to hide an existing aggregation easily)
								if (Aggregation_Aliases.ContainsKey(url_relative_list[0]))
								{
									// Perform all aggregation_style checks next
									string aggregation_code = Aggregation_Aliases[url_relative_list[0]];
									Navigator.Aggregation_Alias = url_relative_list[0];
									aggregation_querystring_analyze( Navigator, QueryString, aggregation_code, url_relative_list.GetRange(1, url_relative_list.Count - 1));
								}
								else if ( Code_Manager.isValidCode( url_relative_list[0] ))
								{ 
									// This is an item aggregation call
									// Perform all aggregation_style checks next
									aggregation_querystring_analyze( Navigator, QueryString, url_relative_list[0], url_relative_list.GetRange(1, url_relative_list.Count - 1 ));
								}
								else if (All_Items_Lookup.Contains_BibID(url_relative_list[0].ToUpper()))
								{
									// This is a BibID for an existing title with at least one public item
									Navigator.BibID = url_relative_list[0].ToUpper();
									Navigator.Mode = Display_Mode_Enum.Item_Display;

									// Is the next part a VID?
									int current_list_index = 1;
									if (url_relative_list.Count > 1)
									{
										string possible_vid = url_relative_list[1].Trim().PadLeft(5, '0');
										if ((All_Items_Lookup.Contains_BibID_VID(Navigator.BibID, possible_vid)) || ( possible_vid == "00000" ))
										{
											Navigator.VID = possible_vid;
											current_list_index++;
										}                                            
									}

									// Look for the item print mode now
									if ((url_relative_list.Count > current_list_index) && (url_relative_list[current_list_index] == "print"))
									{
										// This is an item print request
										Navigator.Mode = Display_Mode_Enum.Item_Print;

										// Since we need special characters for ranges, etc.. the viewer code 
										// is in the options query string variable in this case
										if (QueryString["options"] != null)
										{
											Navigator.ViewerCode = QueryString["options"];
										}
									}
									else
									{
										// Look for the viewercode next
										if (url_relative_list.Count > current_list_index)
										{
											string possible_viewercode = url_relative_list[current_list_index].Trim();

											// Get the view code
											if (possible_viewercode.Length > 0)
											{
												// Get the viewer code
												Navigator.ViewerCode = possible_viewercode;

												// Now, get the page
												if ((Navigator.ViewerCode.Length > 0) && (Char.IsNumber(Navigator.ViewerCode[0])))
												{
													// Look for the first number
													int numberEnd = Navigator.ViewerCode.Length;
													int count = 0;
													foreach (char thisChar in Navigator.ViewerCode)
													{
														if (!Char.IsNumber(thisChar))
														{
															numberEnd = count;
															break;
														}
														count++;
													}

													// Get the page
                                                    ushort testPage;
                                                    if (UInt16.TryParse(Navigator.ViewerCode.Substring(0, numberEnd), out testPage))
                                                        Navigator.Page = testPage;
												}
											}
											else
											{
												// Sequence is set to 1
												Navigator.Page = 1;
											}

											// Used or discarded the possible viewer code (used unless length of zero )
											current_list_index++;

											// Look for a subpage now, if there since there was a (possible) viewer code
											if (url_relative_list.Count > current_list_index)
											{
												string possible_subpage = url_relative_list[current_list_index].Trim();
												if (is_String_Number(possible_subpage))
												{
                                                    ushort testSubPage;
                                                    if (UInt16.TryParse(possible_subpage, out testSubPage))
                                                        Navigator.SubPage = testSubPage;
												}
											}
										}
									}

                                    // Collect number of thumbnails per page
                                    if (QueryString["nt"] != null)
                                    {
                                        short nt_temp;
                                        if (short.TryParse(QueryString["nt"], out nt_temp))
                                            Navigator.Thumbnails_Per_Page = nt_temp;
                                    }

                                    // Collect size of thumbnails per page
                                    if (QueryString["ts"] != null)
                                    {
                                        short ts_temp;
                                        if (short.TryParse(QueryString["ts"], out ts_temp))
                                            Navigator.Size_Of_Thumbnails = ts_temp;
                                    }


									// Collect the text search string
									if (QueryString["search"] != null)
										Navigator.Text_Search = QueryString["search"].Replace("+"," ");

									// If coordinates were here, save them
									if (QueryString["coord"] != null)
										Navigator.Coordinates = QueryString["coord"];

									// If a page is requested by filename (rather than sequenc), collect that
									if (QueryString["file"] != null)
										Navigator.Page_By_FileName = QueryString["file"];
								}
								else if ((String.IsNullOrEmpty(Navigator.Page_By_FileName)) && ((String.IsNullOrEmpty(Navigator.Default_Aggregation)) || (Navigator.Default_Aggregation == "all")))
								{
                                    // This may be a top-level aggregation call
								    // aggregation_querystring_analyze(Navigator, QueryString, Navigator.Default_Aggregation, url_relative_list);

                                    // Pass this unmatched query to the simple html cms to show the missing (custom) screen
                                    Navigator.Mode = Display_Mode_Enum.Simple_HTML_CMS;

								    string possible_info_mode = String.Empty;
								    if (url_relative_list.Count == 1)
								        possible_info_mode = url_relative_list[0];
                                    else if (url_relative_list.Count == 2)
                                        possible_info_mode = url_relative_list[0] + "/" + url_relative_list[1];
                                    else if (url_relative_list.Count == 3)
                                        possible_info_mode = url_relative_list[0] + "/" + url_relative_list[1] + "/" + url_relative_list[2];
                                    else if (url_relative_list.Count == 4)
                                        possible_info_mode = url_relative_list[0] + "/" + url_relative_list[1] + "/" + url_relative_list[2] + "/" + url_relative_list[3];
                                    else if ( url_relative_list.Count > 4)
                                    {
                                        StringBuilder possibleInfoModeBuilder = new StringBuilder();
                                        if (url_relative_list.Count > 0)
                                        {
                                            possibleInfoModeBuilder.Append(url_relative_list[0]);
                                        }
                                        for (int i = 1; i < url_relative_list.Count; i++)
                                        {
                                            possibleInfoModeBuilder.Append("/" + url_relative_list[i]);
                                        }
                                        possible_info_mode = possibleInfoModeBuilder.ToString().Replace("'", "").Replace("\"", "");
                                    }

								    
                                    string base_source = Engine_ApplicationCache_Gateway.Settings.Servers.Base_Directory + "design\\webcontent";

                                    // Set the source location
                                    Navigator.Missing = true;
                                    Navigator.Info_Browse_Mode = possible_info_mode;
                                    Navigator.WebContent_Type = WebContent_Type_Enum.Display;
                                    Navigator.Page_By_FileName = base_source + "\\missing.html";
						            Navigator.WebContentID = -1;
								}
						        break;
						}
					}
				}				
			}
		}

		#endregion

	    private static string[] copy_remaining_segments_as_array(List<string> url_relative_list, int start_index)
	    {
	        if (url_relative_list.Count <= start_index)
	            return null;
	        if (url_relative_list.Count == start_index + 1)
	            return new string[] {url_relative_list[start_index]};
            if (url_relative_list.Count == start_index + 2)
                return new string[] { url_relative_list[start_index], url_relative_list[start_index+1] };
            if (url_relative_list.Count == start_index + 3)
                return new string[] { url_relative_list[start_index], url_relative_list[start_index + 1], url_relative_list[start_index + 2] };
            if (url_relative_list.Count == start_index + 4)
                return new string[] { url_relative_list[start_index], url_relative_list[start_index + 1], url_relative_list[start_index + 2], url_relative_list[start_index + 3] };
            if (url_relative_list.Count == start_index + 5)
                return new string[] { url_relative_list[start_index], url_relative_list[start_index + 1], url_relative_list[start_index + 2], url_relative_list[start_index + 3], url_relative_list[start_index + 4] };
            if (url_relative_list.Count == start_index + 6)
                return new string[] { url_relative_list[start_index], url_relative_list[start_index + 1], url_relative_list[start_index + 2], url_relative_list[start_index + 3], url_relative_list[start_index + 4], url_relative_list[start_index + 5] };
            if (url_relative_list.Count == start_index + 7)
                return new string[] { url_relative_list[start_index], url_relative_list[start_index + 1], url_relative_list[start_index + 2], url_relative_list[start_index + 3], url_relative_list[start_index + 4], url_relative_list[start_index + 5], url_relative_list[start_index + 6] };
            if (url_relative_list.Count == start_index + 8)
                return new string[] { url_relative_list[start_index], url_relative_list[start_index + 1], url_relative_list[start_index + 2], url_relative_list[start_index + 3], url_relative_list[start_index + 4], url_relative_list[start_index + 5], url_relative_list[start_index + 6], url_relative_list[start_index + 7] };

            List<string> allRemaining = new List<string>();
            for( int i = start_index ; i < url_relative_list.Count ; i++ )
                allRemaining.Add(url_relative_list[i]);
	        return allRemaining.ToArray();
	    }

		private static void aggregation_querystring_analyze(Navigation_Object Navigator, NameValueCollection QueryString, string Aggregation, List<string> RemainingURLRedirectList)
		{
            // If the aggrgeation passed in was null or empty, use ALL
		    if (String.IsNullOrEmpty(Aggregation))
		        Aggregation = "all";

			Navigator.Aggregation = Aggregation;
			Navigator.Mode = Display_Mode_Enum.Aggregation;
			Navigator.Aggregation_Type = Aggregation_Type_Enum.Home;

            // Collect any search and search field values
            if (QueryString["t"] != null) Navigator.Search_String = QueryString["t"].Trim();
            if (QueryString["f"] != null) Navigator.Search_Fields = QueryString["f"].Trim();

			// Look for any more url information
			if (RemainingURLRedirectList.Count > 0)
			{
				switch (RemainingURLRedirectList[0])
				{


					case "edit":
						Navigator.Aggregation_Type = Aggregation_Type_Enum.Home_Edit;
						break;

					case "itemcount":
						Navigator.Aggregation_Type = Aggregation_Type_Enum.Item_Count;
						if (RemainingURLRedirectList.Count > 1)
						{
							Navigator.Info_Browse_Mode = RemainingURLRedirectList[1];
						}
						break;

					case "inprocess":
						Navigator.Aggregation_Type = Aggregation_Type_Enum.Private_Items;
						Navigator.Page = 1;
						if (RemainingURLRedirectList.Count > 1)
						{
							if (is_String_Number(RemainingURLRedirectList[1]))
								Navigator.Page = Convert.ToUInt16(RemainingURLRedirectList[1]);
						}
						if ((QueryString["o"] != null) && (is_String_Number(QueryString["o"])))
						{
							Navigator.Sort = Convert.ToInt16(QueryString["o"]);
						}
						else
						{
							Navigator.Sort = 0;
						}
						break;

					case "contact":
						Navigator.Mode = Display_Mode_Enum.Contact;
						if (RemainingURLRedirectList.Count > 1)
						{
							if (RemainingURLRedirectList[1] == "sent")
							{
								Navigator.Mode = Display_Mode_Enum.Contact_Sent;
							}
						}
						if (QueryString["em"] != null)
							Navigator.Error_Message = QueryString["em"];
						break;

                    case "manage":
                    case "aggrmanage":
                        Navigator.Aggregation_Type = Aggregation_Type_Enum.Manage_Menu;
                        break;
                        
                    case "permissions":
                    case "aggrpermissions":
                        Navigator.Aggregation_Type = Aggregation_Type_Enum.User_Permissions;
                        break;

                    case "history":
                    case "aggrhistory":
                        Navigator.Aggregation_Type = Aggregation_Type_Enum.Work_History;
                        break;

                    case "geography":
                        Navigator.Aggregation_Type = Aggregation_Type_Enum.Browse_Map;
                        break;

                    case "usage":
                        Navigator.Aggregation_Type = Aggregation_Type_Enum.Usage_Statistics;
                        if (RemainingURLRedirectList.Count > 1)
                        {
                            Navigator.Info_Browse_Mode = RemainingURLRedirectList[1];
                        }
                        break;

					case "map":
						Navigator.Mode = Display_Mode_Enum.Search;
						Navigator.Search_Type = Search_Type_Enum.Map;
						if (RemainingURLRedirectList.Count > 1)
						{
							Navigator.Info_Browse_Mode = RemainingURLRedirectList[1];
						}
						break;

                    case "mapbeta":
                        Navigator.Mode = Display_Mode_Enum.Search;
                        Navigator.Search_Type = Search_Type_Enum.Map_Beta;
                        if (RemainingURLRedirectList.Count > 1)
                        {
                            Navigator.Info_Browse_Mode = RemainingURLRedirectList[1];
                        }
                        break;



					case "advanced":
						Navigator.Mode = Display_Mode_Enum.Search;
						Navigator.Search_Type = Search_Type_Enum.Advanced;
						break;

					case "text":
						Navigator.Mode = Display_Mode_Enum.Search;
						Navigator.Search_Type = Search_Type_Enum.Full_Text;
						break;

					case "info":
						Navigator.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;
						if (RemainingURLRedirectList.Count > 1)
							Navigator.Info_Browse_Mode = RemainingURLRedirectList[1];
						if ((RemainingURLRedirectList.Count > 2) && (RemainingURLRedirectList[2] == "edit"))
						{
							Navigator.Aggregation_Type = Aggregation_Type_Enum.Child_Page_Edit;
						}
						break;

					case "browseby":
						Navigator.Aggregation_Type = Aggregation_Type_Enum.Browse_By;
						if (RemainingURLRedirectList.Count > 1)
							Navigator.Info_Browse_Mode = RemainingURLRedirectList[1];
						if ( RemainingURLRedirectList.Count > 2 )
						{
							string possible_page = RemainingURLRedirectList[2];
							bool isNumber = possible_page.All(Char.IsNumber);
							if ( isNumber )
							{
								Navigator.Page = Convert.ToUInt16(possible_page);
							}
						}
						break;


					case "results":
					case "resultslike":
					case "contains":
					case "exact":
						switch (RemainingURLRedirectList[0])
						{
							case "results":
								Navigator.Search_Precision = Search_Precision_Type_Enum.Inflectional_Form;
								break;

							case "resultslike":
								Navigator.Search_Precision = Search_Precision_Type_Enum.Synonmic_Form;
								break;

							case "contains":
								Navigator.Search_Precision = Search_Precision_Type_Enum.Contains;
								break;

							case "exact":
								Navigator.Search_Precision = Search_Precision_Type_Enum.Exact_Match;
								break;
						}
						Navigator.Mode = Display_Mode_Enum.Results;
						Navigator.Search_Type = Search_Type_Enum.Basic;
						Navigator.Result_Display_Type = Result_Display_Type_Enum.Default;
						Navigator.Page = 1;
						Navigator.Sort = 0;
						if ((HttpContext.Current != null) && (HttpContext.Current.Session != null) && (HttpContext.Current.Session["User_Default_Sort"] != null))
							Navigator.Sort = Convert.ToInt16(HttpContext.Current.Session["User_Default_Sort"]);

						int search_handled_args = 1;

						// Look for result display type
						if (RemainingURLRedirectList.Count > search_handled_args)
						{
							switch (RemainingURLRedirectList[search_handled_args])
							{
								case "brief":
									Navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;
									search_handled_args++;
									break;
								case "export":
									Navigator.Result_Display_Type = Result_Display_Type_Enum.Export;
									search_handled_args++;
									break;
								case "citation":
									Navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Citation;
									search_handled_args++;
									break;
								case "image":
									Navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Image;
									search_handled_args++;
									break;
								case "map":
									Navigator.Result_Display_Type = Result_Display_Type_Enum.Map;
									search_handled_args++;
									break;
                                case "mapbeta":
                                    Navigator.Result_Display_Type = Result_Display_Type_Enum.Map_Beta;
                                    search_handled_args++;
                                    break;
								case "table":
									Navigator.Result_Display_Type = Result_Display_Type_Enum.Table;
									search_handled_args++;
									break;
								case "thumbs":
									Navigator.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
									search_handled_args++;
									break;
							}
						}

						// Look for a page number
						if (RemainingURLRedirectList.Count > search_handled_args)
						{
							string possible_page = RemainingURLRedirectList[search_handled_args];
							if ((possible_page.Length > 0) && (is_String_Number(possible_page)))
							{
								ushort page_result ;
								UInt16.TryParse(possible_page, out page_result);
								Navigator.Page = page_result;
							}
						}

						// Collect the coordinate information from the URL query string
						if (QueryString["coord"] != null)
						{
							Navigator.Coordinates = QueryString["coord"].Trim();
							if (!String.IsNullOrEmpty(Navigator.Coordinates))
							{
								Navigator.Search_Type = Search_Type_Enum.Map;
								Navigator.Search_Fields = String.Empty;
								Navigator.Search_String = String.Empty;
							}
						}

                        // Collect any date range that may have existed
						if (QueryString["yr1"] != null)
						{
							short year1;
							if (Int16.TryParse(QueryString["yr1"], out year1))
								Navigator.DateRange_Year1 = year1;
						}
						if (QueryString["yr2"] != null)
						{
							short year2;
							if (Int16.TryParse(QueryString["yr2"], out year2))
								Navigator.DateRange_Year2 = year2;
						}
						if ((Navigator.DateRange_Year1.HasValue ) && ( Navigator.DateRange_Year2.HasValue ) && (Navigator.DateRange_Year1.Value > Navigator.DateRange_Year2.Value ))
						{
							short temp = Navigator.DateRange_Year1.Value;
                            Navigator.DateRange_Year1 = Navigator.DateRange_Year2.Value;
							Navigator.DateRange_Year2 = temp;
						}
						if (QueryString["da1"] != null)
						{
							long date1;
							if (Int64.TryParse(QueryString["da1"], out date1))
								Navigator.DateRange_Date1 = date1;
						}
						if (QueryString["da2"] != null)
						{
							long date2;
							if (Int64.TryParse(QueryString["da2"], out date2))
								Navigator.DateRange_Date2 = date2;
						}

						// Was a search string and fields included?
						if ((Navigator.Search_String.Length > 0) && (Navigator.Search_Fields.Length > 0))
						{
							Navigator.Search_Type = Search_Type_Enum.Advanced;
						}
						else
						{
							Navigator.Search_Fields = "ZZ";
						}

						// If no search term, look foor the TEXT-specific term
						if (Navigator.Search_String.Length == 0)
						{
							if (QueryString["text"] != null)
							{
								Navigator.Search_String = QueryString["text"].Trim();
							}

							if (Navigator.Search_String.Length > 0)
							{
								Navigator.Search_Type = Search_Type_Enum.Full_Text;
							}
						}                        

						// Check for any sort value
						if (QueryString["o"] != null) 
						{
							string sort = QueryString["o"];
							if (is_String_Number(sort))
							{
								short sort_result ;
								Int16.TryParse(sort, out sort_result);
								Navigator.Sort = sort_result;
							}
						}
						break;

					default:
						Navigator.Mode = Display_Mode_Enum.Aggregation;
						Navigator.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;
						Navigator.Info_Browse_Mode = RemainingURLRedirectList[0];
						Navigator.Result_Display_Type = Result_Display_Type_Enum.Default;
						Navigator.Page = 1;
						Navigator.Sort = 0;
						if ((HttpContext.Current != null) && (HttpContext.Current.Session != null) && (HttpContext.Current.Session["User_Default_Sort"] != null))
							Navigator.Sort = Convert.ToInt16(HttpContext.Current.Session["User_Default_Sort"]);
						int aggr_handled_args = 1;

						// Look for result display type
						if (RemainingURLRedirectList.Count > aggr_handled_args)
						{
							switch ( RemainingURLRedirectList[ aggr_handled_args ] )
							{
								case "brief":
									Navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;
									aggr_handled_args++;
									break;
								case "export":
									Navigator.Result_Display_Type = Result_Display_Type_Enum.Export;
									aggr_handled_args++;
									break;
								case "citation":
									Navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Citation;
									aggr_handled_args++;
									break;
								case "image":
									Navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Image;
									aggr_handled_args++;
									break;
								case "map":
									Navigator.Result_Display_Type = Result_Display_Type_Enum.Map;
									aggr_handled_args++;
									break;
                                case "mapbeta":
                                    Navigator.Result_Display_Type = Result_Display_Type_Enum.Map_Beta;
                                    aggr_handled_args++;
                                    break;
								case "table":
									Navigator.Result_Display_Type = Result_Display_Type_Enum.Table;
									aggr_handled_args++;
									break;
								case "thumbs":
									Navigator.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
									aggr_handled_args++;
									break;
								case "edit":
									Navigator.Aggregation_Type = Aggregation_Type_Enum.Child_Page_Edit;
									aggr_handled_args++;
									break;
							}
						}

						// Look for a page number
						if (RemainingURLRedirectList.Count > aggr_handled_args)
						{
							string possible_page = RemainingURLRedirectList[aggr_handled_args];
							if (( possible_page.Length > 0 ) && ( is_String_Number(possible_page)))
							{
								ushort page_result;
								UInt16.TryParse(possible_page, out page_result);
								Navigator.Page = page_result;
							}
						}

						// Check for any sort value
						if (QueryString["o"] != null)
						{
							string sort = QueryString["o"];
							if (is_String_Number(sort))
							{
								short sort_result;
								Int16.TryParse(sort, out sort_result);
								Navigator.Sort = sort_result;
							}
						}

						break;
				}
			}
		}

		/// <summary> Method checks to see if this string contains only numbers </summary>
		/// <param name="TestString"> string to check for all numerals </param>
		/// <returns> TRUE if the string is made of all numerals </returns>
		/// <remarks> This just steps through each character in the string and tests with the Char.IsNumber method</remarks>
		private static bool is_String_Number(string TestString)
		{
			// Step through each character and return false if not a number
			return TestString.All(Char.IsNumber);
		}
	}
}
