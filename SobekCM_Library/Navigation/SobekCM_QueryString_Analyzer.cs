#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;

#endregion

namespace SobekCM.Library.Navigation
{
	/// <summary> Query string analyzer for the new URL query string configuration with URL rewrite </summary>
	/// <remarks> This implements the <see cref="iSobekCM_QueryString_Analyzer"/> interface. <br /> <br />
	/// QueryString_Analyzer is a class which analyzes the query string
	/// passed to the web server along with the URL.  This determines which portion
	/// of the web application to display first, and allows users to cut and paste
	/// a particular search or map. </remarks>
	public class SobekCM_QueryString_Analyzer : iSobekCM_QueryString_Analyzer
	{
		#region Constructor

		#endregion

		#region iSobekCM_QueryString_Analyzer Members

		/// <summary> Parse the query and set the internal variables </summary>
		/// <param name="QueryString"> QueryString collection passed from the main page </param>
		/// <param name="navigator"> Navigation object to hold the mode information </param>
		/// <param name="Base_URL">Requested base URL (without query string, etc..)</param>
		/// <param name="User_Languages"> Languages preferred by user, per their browser settings </param>
		/// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections </param>
		/// <param name="Aggregation_Aliases"> List of all existing aliases for existing aggregations</param>
		/// <param name="All_Items_Lookup"> [REF] Lookup object used to pull basic information about any item loaded into this library</param>
		/// <param name="URL_Portals"> List of all web portals into this system </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public void Parse_Query(NameValueCollection QueryString,
			SobekCM_Navigation_Object navigator,
			string Base_URL,
			string[] User_Languages,
			Aggregation_Code_Manager Code_Manager,
			Dictionary<string, string> Aggregation_Aliases,
			ref Item_Lookup_Object All_Items_Lookup,
			Portal_List URL_Portals,
			Custom_Tracer Tracer )
		{
			// Set default mode to error
			navigator.Mode = Display_Mode_Enum.Error;

			// If this has 'verb' then this is an OAI-PMH request
			if ( QueryString["verb"] != null )
			{
				navigator.Writer_Type = Writer_Type_Enum.OAI;
				return;
			}

			// Is there a TOC state set?
			if (QueryString["toc"] != null)
			{
				if (QueryString["toc"] == "y")
				{
					navigator.TOC_Display = TOC_Display_Type_Enum.Show;
				}
				else if ( QueryString["toc"] == "n" )
				{
					navigator.TOC_Display = TOC_Display_Type_Enum.Hide;
				}
			}

			// Determine the default language, per the browser settings
			if (User_Languages != null)
			{
				foreach (string thisLanguage in User_Languages)
				{
					if (thisLanguage.IndexOf("en") == 0)
					{
						navigator.Default_Language = Web_Language_Enum.English;
						break;
					}

					if (thisLanguage.IndexOf("fr") == 0)
					{
						navigator.Default_Language = Web_Language_Enum.French;
						break;
					}

					if (thisLanguage.IndexOf("es") == 0)
					{
						navigator.Default_Language = Web_Language_Enum.Spanish;
						break;
					}
				}
			}

			// Is there a language defined?  If so, load right into the navigator
			navigator.Language = navigator.Default_Language;
			if ( QueryString["l"] != null )
			{
				if (( QueryString["l"] == "es" ) || ( QueryString["l"] == "sp" ))
				{
					navigator.Language = Web_Language_Enum.Spanish;
				}
				if ( QueryString["l"] == "fr" )
				{
					navigator.Language = Web_Language_Enum.French;
				}
				if (QueryString["l"] == "en")
				{
					navigator.Language = Web_Language_Enum.English;
				}
			}

			// If there is flag indicating to show the trace route, save it
			if (QueryString["trace"] != null)
			{
				navigator.Trace_Flag = QueryString["trace"].ToUpper() == "NO" ? Trace_Flag_Type_Enum.No : Trace_Flag_Type_Enum.Explicit;
			}
			else
			{
				navigator.Trace_Flag = Trace_Flag_Type_Enum.Unspecified;
			}

			// Did the user request to have it render like it would for a search robot?
			if (QueryString["robot"] != null)
			{
				navigator.Is_Robot = true;
			}

            // Was a fragment specified in the query string?
            if (QueryString["fragment"] != null)
            {
                navigator.Fragment = QueryString["fragment"];
            }

            // Get the valid URL Portal
            navigator.Default_Aggregation = "all";
            Portal urlPortal = URL_Portals.Get_Valid_Portal(Base_URL);
            navigator.SobekCM_Instance_Abbreviation = urlPortal.Abbreviation;
            navigator.SobekCM_Instance_Name = urlPortal.Name;
            navigator.Portal_PURL = urlPortal.Base_PURL;
            if (urlPortal.Default_Aggregation.Length > 0)
            {
                navigator.Default_Aggregation = urlPortal.Default_Aggregation;
                navigator.Aggregation = urlPortal.Default_Aggregation;
            }
            if (urlPortal.Default_Web_Skin.Length > 0)
            {
                navigator.Default_Skin = urlPortal.Default_Web_Skin;
                navigator.Skin = urlPortal.Default_Web_Skin;
                navigator.Skin_in_URL = false;
            }

            // Collect the interface string
            if (QueryString["n"] != null)
            {
                string currSkin = HttpUtility.HtmlEncode(QueryString["n"].ToLower().Replace("'", ""));

                // Save the interface
                if (currSkin.Length > 0)
                {
                    if (currSkin.IndexOf(",") > 0)
                        currSkin = currSkin.Substring(0, currSkin.IndexOf(","));
                    navigator.Skin = currSkin.ToLower();
                    navigator.Skin_in_URL = true;
                }
            }
			
			// Parse URL request different now, depending on if this is a legacy URL type or the new URL type
			navigator.Mode = Display_Mode_Enum.None;

			// CHECK FOR LEGACY VALUES 
			// Check for legacy bibid / vid information, since this will be supported indefinitely
			if (( QueryString["b"] != null ) || ( QueryString["bib"] != null ))
			{
				navigator.BibID = QueryString["b"] ?? QueryString["bib"];

				if ( navigator.BibID.Length > 0 )
				{
					navigator.Mode = Display_Mode_Enum.Item_Display;

					if ( QueryString["v"] != null )
						navigator.VID = QueryString["v"];
					else if ( QueryString["vid"] != null )
						navigator.VID = QueryString["vid"];                
				}

				// No other item information is collected here anymore.. just return
				return;
			}


			// Set the default mode
			navigator.Mode = Display_Mode_Enum.Aggregation_Home;
			navigator.Home_Type = Home_Type_Enum.List;

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
					navigator.Writer_Type = Writer_Type_Enum.HTML;
					if ( url_relative_list.Count > 0 )
					{
						switch (url_relative_list[0])
						{
							case "l":
								navigator.Writer_Type = Writer_Type_Enum.HTML_LoggedIn;
								url_relative_list.RemoveAt(0);
								break;

							case "my":
								navigator.Writer_Type = Writer_Type_Enum.HTML_LoggedIn;
								break;

							case "json":
								navigator.Writer_Type = Writer_Type_Enum.JSON;
								url_relative_list.RemoveAt(0);
								break;

							case "dataset":
								navigator.Writer_Type = Writer_Type_Enum.DataSet;
								url_relative_list.RemoveAt(0);
								break;

							case "xml":
								navigator.Writer_Type = Writer_Type_Enum.XML;
								url_relative_list.RemoveAt(0);
								break;

							case "textonly":
								navigator.Writer_Type = Writer_Type_Enum.Text;
								url_relative_list.RemoveAt(0);
								break;
						}
					}

					// Is the first part of the list one of these constants?
					if (( url_relative_list.Count > 0 ) && ( url_relative_list[0].Length > 0 ))
					{
						switch ( url_relative_list[0] )
						{
							case "gatorlink":
							case "shibboleth":
								navigator.Mode = Display_Mode_Enum.My_Sobek;
								navigator.My_Sobek_Type = My_Sobek_Type_Enum.Shibboleth_Landing;
								break;

							case "internal":
								navigator.Mode = Display_Mode_Enum.Internal;
								navigator.Internal_Type = Internal_Type_Enum.Aggregations_List;
								if ( url_relative_list.Count > 1 )
								{
									switch( url_relative_list[1] )
									{
										case "aggregations":
											navigator.Internal_Type = Internal_Type_Enum.Aggregations_List;
											if (url_relative_list.Count > 2)
											{
												if (url_relative_list[2] == "list")
												{
													if (url_relative_list.Count > 3)
														navigator.Info_Browse_Mode = url_relative_list[3];
												}
												else
												{
													navigator.Internal_Type = Internal_Type_Enum.Aggregations;
													navigator.Info_Browse_Mode = url_relative_list[2].Replace("_"," ");
												}
											}
											break;

										case "cache":
											navigator.Internal_Type = Internal_Type_Enum.Cache;
											break;

										case "new":
											navigator.Internal_Type = Internal_Type_Enum.New_Items;
											if (url_relative_list.Count > 2)
											{
												navigator.Info_Browse_Mode = url_relative_list[2];
											}
											break;

										case "failures":
											navigator.Internal_Type = Internal_Type_Enum.Build_Failures;
											if (url_relative_list.Count > 2)
												navigator.Info_Browse_Mode = url_relative_list[2];
											break;
												
										case "wordmarks":
											navigator.Internal_Type = Internal_Type_Enum.Wordmarks;
											break;
									}
								}
								break;
					 

							case "contact":
								navigator.Mode = Display_Mode_Enum.Contact;
								if ( url_relative_list.Count > 1 )
								{
									if (url_relative_list[1] == "sent")
									{
										navigator.Mode = Display_Mode_Enum.Contact_Sent;
									}
									else
									{
										navigator.Aggregation = url_relative_list[1];
									}
								}
								if (QueryString["em"] != null)
									navigator.Error_Message = QueryString["em"];
								break;
					  
							case "folder":
								navigator.Mode = Display_Mode_Enum.Public_Folder;
								if (url_relative_list.Count >= 2)
								{
									try
									{
										navigator.FolderID = Convert.ToInt32(url_relative_list[1]);
									}
									catch
									{
										navigator.FolderID = -1;
									}

									// Look for result display type
									if (url_relative_list.Count >=3 )
									{
										switch (url_relative_list[2])
										{
											case "brief":
												navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;
												break;
											case "export":
												navigator.Result_Display_Type = Result_Display_Type_Enum.Export;
												break;
											case "citation":
												navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Citation;
												break;
											case "image":
												navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Image;
												break;
											case "map":
												navigator.Result_Display_Type = Result_Display_Type_Enum.Map;
												break;
											case "table":
												navigator.Result_Display_Type = Result_Display_Type_Enum.Table;
												break;
											case "thumbs":
												navigator.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
												break;
											default: 
												navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;
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
											navigator.Page = page_result;
										}
									}
								}
								break;
										   

							case "my":
								navigator.Mode = Display_Mode_Enum.My_Sobek;
								if (QueryString["return"] != null)
									navigator.Return_URL = QueryString["return"];
								if ( url_relative_list.Count > 1 )
								{
									switch( url_relative_list[1] )
									{
										case "logon":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
											if (QueryString["return"] != null)
												navigator.Return_URL = QueryString["return"];
											break;

										case "home":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.Home;
											if (QueryString["return"] != null)
												navigator.Return_URL = QueryString["return"];
											break;

										case "delete":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.Delete_Item;
											if (url_relative_list.Count > 2)
												navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												navigator.VID = url_relative_list[3];
											break;

										case "submit":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.New_Item;
											if (url_relative_list.Count > 2)
												navigator.My_Sobek_SubMode = url_relative_list[2];
											break;

										case "behaviors":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Behaviors;
											if (url_relative_list.Count > 2)
												navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												navigator.VID = url_relative_list[3];
											if (url_relative_list.Count > 4)
												navigator.My_Sobek_SubMode = url_relative_list[4];
											break;

										case "edit":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Metadata;
											if (url_relative_list.Count > 2)
												navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												navigator.VID = url_relative_list[3];
											if (url_relative_list.Count > 4)
												navigator.My_Sobek_SubMode = url_relative_list[4];
											break;

										case "files":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.File_Management;
											if (url_relative_list.Count > 2)
												navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												navigator.VID = url_relative_list[3];
											if (url_relative_list.Count > 4)
												navigator.My_Sobek_SubMode = url_relative_list[4];
											break;

                                        case "images":
                                            navigator.My_Sobek_Type = My_Sobek_Type_Enum.Page_Images_Management;
                                            if (url_relative_list.Count > 2)
                                                navigator.BibID = url_relative_list[2].ToUpper();
                                            if (url_relative_list.Count > 3)
                                                navigator.VID = url_relative_list[3];
                                            if (url_relative_list.Count > 4)
                                                navigator.My_Sobek_SubMode = url_relative_list[4];
                                            break;


										case "addvolume":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.Group_Add_Volume;
											if (url_relative_list.Count > 2)
												navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												navigator.My_Sobek_SubMode = url_relative_list[3];
											break;

										case "autofill":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.Group_AutoFill_Volumes;
											if (url_relative_list.Count > 2)
												navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												navigator.My_Sobek_SubMode = url_relative_list[3];
											break;

										case "massupdate":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.Group_Mass_Update_Items;
											if (url_relative_list.Count > 2)
												navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												navigator.My_Sobek_SubMode = url_relative_list[3];
											break;

										case "groupbehaviors":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Group_Behaviors;
											if (url_relative_list.Count > 2)
												navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												navigator.My_Sobek_SubMode = url_relative_list[3];
											break;

										case "serialhierarchy":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Group_Serial_Hierarchy;
											if (url_relative_list.Count > 2)
												navigator.BibID = url_relative_list[2].ToUpper();
											if (url_relative_list.Count > 3)
												navigator.My_Sobek_SubMode = url_relative_list[3];
											break;

										case "bookshelf":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
											navigator.Result_Display_Type = Result_Display_Type_Enum.Bookshelf;
											if (url_relative_list.Count > 2)
											{
												navigator.My_Sobek_SubMode = url_relative_list[2];
												if (url_relative_list.Count > 3)
												{
													switch (navigator.My_Sobek_SubMode)
													{
														case "brief":
															navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;
															navigator.My_Sobek_SubMode = url_relative_list[3];
															break;

														case "export":
															navigator.Result_Display_Type = Result_Display_Type_Enum.Export;
															navigator.My_Sobek_SubMode = url_relative_list[3];
															break;

														case "thumbs":
															navigator.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
															navigator.My_Sobek_SubMode = url_relative_list[3];
															break;

														case "table":
															navigator.Result_Display_Type = Result_Display_Type_Enum.Table;
															navigator.My_Sobek_SubMode = url_relative_list[3];
															break;

														case "citation":
															navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Citation;
															navigator.My_Sobek_SubMode = url_relative_list[3];
															break;

														case "image":
															navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Image;
															navigator.My_Sobek_SubMode = url_relative_list[3];
															break;

														default:
															if (is_String_Number(url_relative_list[3]))
															{
																ushort page_result;
																UInt16.TryParse(url_relative_list[3], out page_result);
																navigator.Page = page_result;
															}
															break;
													}
												}
												if ((url_relative_list.Count > 4) && ( is_String_Number( url_relative_list[4] )))
												{
													ushort page_result;
													UInt16.TryParse(url_relative_list[4], out page_result);
													navigator.Page = page_result;
												}
											}
											break;

										case "preferences":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
											break;

										case "logout":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.Log_Out;
											if (QueryString["return"] != null)
												navigator.Return_URL = QueryString["return"];
											break;

										case "gatorlink":
										case "shibboleth":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.Shibboleth_Landing;
											if (QueryString["return"] != null)
												navigator.Return_URL = QueryString["return"];
											break;
 
										case "searches":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.Saved_Searches;
											break;

										case "tags":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.User_Tags;
											if (url_relative_list.Count > 2)
												navigator.My_Sobek_SubMode = url_relative_list[2];
											break;

										case "stats":
											navigator.My_Sobek_Type = My_Sobek_Type_Enum.User_Usage_Stats;
											if (url_relative_list.Count > 2)
												navigator.My_Sobek_SubMode = url_relative_list[2];
											break;
									}
								}
								break;

                            case "admin":
                                navigator.Mode = Display_Mode_Enum.Administrative;
						        navigator.Admin_Type = Admin_Type_Enum.Home;
                                if (QueryString["return"] != null)
                                    navigator.Return_URL = QueryString["return"];
                                if (url_relative_list.Count > 1)
                                {
                                    switch (url_relative_list[1])
                                    {
                                        case "builder":
                                            navigator.Admin_Type = Admin_Type_Enum.Builder_Status;
                                            break;

                                        case "aggregations":
                                            navigator.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
                                            if (url_relative_list.Count > 2)
                                                navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "editaggr":
                                            navigator.Admin_Type = Admin_Type_Enum.Aggregation_Single;
                                            if (url_relative_list.Count > 2)
                                                navigator.Aggregation = url_relative_list[2];
                                            if (url_relative_list.Count > 3)
                                                navigator.My_Sobek_SubMode = url_relative_list[3];
                                            break;

                                        case "aliases":
                                            navigator.Admin_Type = Admin_Type_Enum.Forwarding;
                                            break;

                                        case "webskins":
                                            navigator.Admin_Type = Admin_Type_Enum.Interfaces;
                                            break;

                                        case "projects":
                                            navigator.Admin_Type = Admin_Type_Enum.Projects;
                                            if (url_relative_list.Count > 2)
                                                navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "restrictions":
                                            navigator.Admin_Type = Admin_Type_Enum.IP_Restrictions;
                                            if (url_relative_list.Count > 2)
                                                navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "portals":
                                            navigator.Admin_Type = Admin_Type_Enum.URL_Portals;
                                            break;

                                        case "users":
                                            navigator.Admin_Type = Admin_Type_Enum.Users;
                                            if (url_relative_list.Count > 2)
                                                navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "groups":
                                            navigator.Admin_Type = Admin_Type_Enum.User_Groups;
                                            if (url_relative_list.Count > 2)
                                                navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "wordmarks":
                                            navigator.Admin_Type = Admin_Type_Enum.Wordmarks;
                                            break;

                                        case "reset":
                                            navigator.Admin_Type = Admin_Type_Enum.Reset;
                                            break;

                                        case "headings":
                                            navigator.Admin_Type = Admin_Type_Enum.Thematic_Headings;
                                            if (url_relative_list.Count > 2)
                                                navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;

                                        case "settings":
                                            navigator.Admin_Type = Admin_Type_Enum.Settings;
                                            if (url_relative_list.Count > 2)
                                                navigator.My_Sobek_SubMode = url_relative_list[2];
                                            break;
                                    }
                                }
                                break;
					 
							case "preferences":
								navigator.Mode = Display_Mode_Enum.Preferences;
								break;

							case "stats":
							case "statistics":
								navigator.Mode = Display_Mode_Enum.Statistics;
								if ( url_relative_list.Count > 1 )
								{
									switch( url_relative_list[1] )
									{
										case "itemcount":
											navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Standard_View;
											if ( url_relative_list.Count > 2 )
											{
												switch( url_relative_list[2])
												{
													case "arbitrary":
														navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Arbitrary_View;
														if (url_relative_list.Count > 3)
														{
															navigator.Info_Browse_Mode = url_relative_list[3];
														}
														break;

													case "growth":
														navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Growth_View;
														break;
												
													case "text":
														navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Text;
														break;

													case "standard":
														navigator.Statistics_Type = Statistics_Type_Enum.Item_Count_Standard_View;
														break;
												}
											}
											break;

										case "searches":
											navigator.Statistics_Type = Statistics_Type_Enum.Recent_Searches;
											break;

										case "usage":
											navigator.Statistics_Type = Statistics_Type_Enum.Usage_Overall;
											if ( url_relative_list.Count > 2 )
											{
												switch( url_relative_list[2])
												{
													case "all":
														navigator.Statistics_Type = Statistics_Type_Enum.Usage_Overall;
														break;
												
													case "history":
														navigator.Statistics_Type = Statistics_Type_Enum.Usage_Collection_History;
														if ( url_relative_list.Count > 3 )
														{
															switch( url_relative_list[3] )
															{
																case "text":
																	navigator.Statistics_Type = Statistics_Type_Enum.Usage_Collection_History_Text;
																	break;

																default:
																	navigator.Info_Browse_Mode = url_relative_list[3];
																	break;
															}

															if ((String.IsNullOrEmpty(navigator.Info_Browse_Mode)) && (url_relative_list.Count > 4))
																navigator.Info_Browse_Mode = url_relative_list[4];
														}
														break;

													case "collections":
														navigator.Statistics_Type = Statistics_Type_Enum.Usage_Collections_By_Date;
														if (url_relative_list.Count > 3)
															navigator.Info_Browse_Mode = url_relative_list[3];
														break;

													case "definitions":
														navigator.Statistics_Type = Statistics_Type_Enum.Usage_Definitions;
														break;

													case "titles":
														navigator.Statistics_Type = Statistics_Type_Enum.Usage_Titles_By_Collection;
														if (( String.IsNullOrEmpty( navigator.Info_Browse_Mode )) && ( url_relative_list.Count > 4 ))
															navigator.Info_Browse_Mode = url_relative_list[4];
														break;

													case "items":
														navigator.Statistics_Type = Statistics_Type_Enum.Usage_Item_Views_By_Date;
														if ( url_relative_list.Count > 3 )
														{
															switch( url_relative_list[3] )
															{
																case "date":
																	navigator.Statistics_Type = Statistics_Type_Enum.Usage_Item_Views_By_Date;
																	break;

																case "top":
																	navigator.Statistics_Type = Statistics_Type_Enum.Usage_Items_By_Collection;
																	break;

																case "text":
																	navigator.Statistics_Type = Statistics_Type_Enum.Usage_By_Date_Text;
																	break;

																default:
																	navigator.Info_Browse_Mode = url_relative_list[3];
																	break;
															}

															if (( String.IsNullOrEmpty( navigator.Info_Browse_Mode )) && ( url_relative_list.Count > 4 ))
																navigator.Info_Browse_Mode = url_relative_list[4];
														}
														break;

												}
											}
											break;
									}
								}
								break;

							case "partners":
								if (navigator.Default_Aggregation == "all")
								{
									navigator.Mode = Display_Mode_Enum.Aggregation_Home;
									navigator.Aggregation = String.Empty;
									navigator.Home_Type = Home_Type_Enum.Partners_List;
									if ((url_relative_list.Count > 1) && (url_relative_list[1] == "thumbs"))
									{
										navigator.Home_Type = Home_Type_Enum.Partners_Thumbnails;
									}
								}
								else
								{
									aggregation_querystring_analyze(navigator, QueryString, navigator.Default_Aggregation, url_relative_list);
								}
								break;

							case "tree":
								navigator.Mode = Display_Mode_Enum.Aggregation_Home;
								navigator.Aggregation = String.Empty;
								navigator.Home_Type = Home_Type_Enum.Tree_Collapsed;
								if ((url_relative_list.Count > 1) && (url_relative_list[1] == "expanded"))
								{
									navigator.Home_Type = Home_Type_Enum.Tree_Expanded;
								}
								break;

							case "brief":
								navigator.Mode = Display_Mode_Enum.Aggregation_Home;
								navigator.Aggregation = String.Empty;
								navigator.Home_Type = Home_Type_Enum.Descriptions;
								break;

							case "personalized":
								navigator.Mode = Display_Mode_Enum.Aggregation_Home;
								navigator.Aggregation = String.Empty;
								navigator.Home_Type = Home_Type_Enum.Personalized;
								break;

							case "all":
								//if ( navigator.Default_Aggregation == "all" )
								//    aggregation_querystring_analyze(navigator, QueryString, "all", url_relative_list.GetRange(1, url_relative_list.Count - 1));
								//else
									aggregation_querystring_analyze(navigator, QueryString, navigator.Default_Aggregation, url_relative_list);
								break;

							case "new":
								//if (navigator.Default_Aggregation != "all")
									aggregation_querystring_analyze(navigator, QueryString, navigator.Default_Aggregation, url_relative_list);
								break;


							case "map":
							case "advanced":
							case "text":
							case "results":
							case "contains":
							case "exact":
							case "resultslike":
							case "browseby":
							case "info":
								aggregation_querystring_analyze(navigator, QueryString, navigator.Default_Aggregation, url_relative_list);
								break;

							// This was none of the main constant mode settings,
							default:
								// First check to see if the first term was an item aggregation alias, which
								// allows for the alias to overwrite an existing aggregation code (limited usability
								// but can be used to hide an existing aggregation easily)
								if (Aggregation_Aliases.ContainsKey(url_relative_list[0]))
								{
									// Perform all aggregation_style checks next
									string aggregation_code = Aggregation_Aliases[url_relative_list[0]];
									navigator.Aggregation_Alias = url_relative_list[0];
									aggregation_querystring_analyze( navigator, QueryString, aggregation_code, url_relative_list.GetRange(1, url_relative_list.Count - 1));
								}
								else if ( Code_Manager.isValidCode( url_relative_list[0] ))
								{ 
									// This is an item aggregation call
									// Perform all aggregation_style checks next
									aggregation_querystring_analyze( navigator, QueryString, url_relative_list[0], url_relative_list.GetRange(1, url_relative_list.Count - 1 ));
								}
								else if ((SobekCM_Database.Verify_Item_Lookup_Object(true, ref All_Items_Lookup, Tracer)) && (All_Items_Lookup.Contains_BibID(url_relative_list[0].ToUpper())))
								{
									// This is a BibID for an existing title with at least one public item
									navigator.BibID = url_relative_list[0].ToUpper();
									navigator.Mode = Display_Mode_Enum.Item_Display;

									// Is the next part a VID?
									int current_list_index = 1;
									if (url_relative_list.Count > 1)
									{
										string possible_vid = url_relative_list[1].Trim().PadLeft(5, '0');
										if ((All_Items_Lookup.Contains_BibID_VID(navigator.BibID, possible_vid)) || ( possible_vid == "00000" ))
										{
											navigator.VID = possible_vid;
											current_list_index++;
										}                                            
									}

									// Look for the item print mode now
									if ((url_relative_list.Count > current_list_index) && (url_relative_list[current_list_index] == "print"))
									{
										// This is an item print request
										navigator.Mode = Display_Mode_Enum.Item_Print;

										// Since we need special characters for ranges, etc.. the viewer code 
										// is in the options query string variable in this case
										if (QueryString["options"] != null)
										{
											navigator.ViewerCode = QueryString["options"];
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
												navigator.ViewerCode = possible_viewercode;

												// Now, get the page
												if ((navigator.ViewerCode.Length > 0) && (Char.IsNumber(navigator.ViewerCode[0])))
												{
													// Look for the first number
													int numberEnd = navigator.ViewerCode.Length;
													int count = 0;
													foreach (char thisChar in navigator.ViewerCode)
													{
														if (!Char.IsNumber(thisChar))
														{
															numberEnd = count;
															break;
														}
														count++;
													}

													// Get the page
													navigator.Page = Convert.ToUInt16(navigator.ViewerCode.Substring(0, numberEnd));
												}
											}
											else
											{
												// Sequence is set to 1
												navigator.Page = 1;
											}

											// Used or discarded the possible viewer code (used unless length of zero )
											current_list_index++;

											// Look for a subpage now, if there since there was a (possible) viewer code
											if (url_relative_list.Count > current_list_index)
											{
												string possible_subpage = url_relative_list[current_list_index].Trim();
												if (is_String_Number(possible_subpage))
												{
													navigator.SubPage = Convert.ToUInt16(possible_subpage);
												}
											}
										}
									}

									// Check for any view port option information in the query string
									if (QueryString["vo"] != null)
									{
										string viewport_options = QueryString["vo"];
										if (viewport_options.Length > 0)
										{
											navigator.Viewport_Size = Convert.ToUInt16("0" + viewport_options[0]);

											if (viewport_options.Length > 1)
											{
												navigator.Viewport_Zoom = (ushort)(Convert.ToUInt16("0" + viewport_options[1]) + 1);
												if (navigator.Viewport_Zoom > 5)
													navigator.Viewport_Zoom = 5;
												if (navigator.Viewport_Zoom < 1)
													navigator.Viewport_Zoom = 1;

												if (viewport_options.Length > 2)
												{
													navigator.Viewport_Rotation = Convert.ToUInt16("0" + viewport_options[2]);
												}
											}
										}
									}

									// Check for view port point in the query string
									if (QueryString["vp"] != null)
									{
										string viewport_point = QueryString["vp"];

										// Get the viewport point
										if ((viewport_point.Length > 0) && (viewport_point.IndexOf(",") > 0))
										{
											string[] split = viewport_point.Split(",".ToCharArray());
											navigator.Viewport_Point_X = Convert.ToInt32(split[0]);
											navigator.Viewport_Point_Y = Convert.ToInt32(split[1]);
										}
									}

                                    // Collect number of thumbnails per page
                                    if (QueryString["nt"] != null)
                                    {
                                        short nt_temp;
                                        if (short.TryParse(QueryString["nt"], out nt_temp))
                                            navigator.Thumbnails_Per_Page = nt_temp;
                                    }

                                    // Collect size of thumbnails per page
                                    if (QueryString["ts"] != null)
                                    {
                                        short ts_temp;
                                        if (short.TryParse(QueryString["ts"], out ts_temp))
                                            navigator.Size_Of_Thumbnails = ts_temp;
                                    }


									// Collect the text search string
									if (QueryString["search"] != null)
										navigator.Text_Search = QueryString["search"].Replace("+"," ");

									// If coordinates were here, save them
									if (QueryString["coord"] != null)
										navigator.Coordinates = QueryString["coord"];

									// If a page is requested by filename (rather than sequenc), collect that
									if (QueryString["file"] != null)
										navigator.Page_By_FileName = QueryString["file"];
								}
								else
								{
									// Maybe this is a web content / info page
									navigator.Mode = Display_Mode_Enum.Simple_HTML_CMS;
									StringBuilder possibleInfoModeBuilder = new StringBuilder();
									if (url_relative_list.Count > 0)
									{
										possibleInfoModeBuilder.Append(url_relative_list[0]);
									}
									for (int i = 1; i < url_relative_list.Count; i++)
									{
										possibleInfoModeBuilder.Append("/" + url_relative_list[i]);
									}

									string possible_info_mode = possibleInfoModeBuilder.ToString().Replace("'", "").Replace("\"", "");
									string filename = possible_info_mode;
									string base_source = SobekCM_Library_Settings.Base_Directory + "design\\webcontent";
									string source = base_source;

									if ((possible_info_mode.IndexOf("\\") > 0) || (possible_info_mode.IndexOf("/") > 0))
									{
										source = source + "\\" + possible_info_mode.Replace("/", "\\");
										string[] split = source.Split("\\".ToCharArray());
										filename = split[split.Length - 1];
										source = source.Substring(0, source.Length - filename.Length);
									}


									if (Directory.Exists(source))
									{
										string[] matching_file = Directory.GetFiles(source, filename + ".htm*");
										if (matching_file.Length > 0)
										{
											navigator.Info_Browse_Mode = possible_info_mode;
											navigator.Page_By_FileName = matching_file[0];
										}
									}

									if ( navigator.Page_By_FileName.Length == 0 )
									{
										// This may point to the default html in the parent directory
										if ((Directory.Exists(source + "\\" + filename)) && (File.Exists(source + "\\" + filename + "\\default.html")))
										{
											navigator.Info_Browse_Mode = possible_info_mode;
											navigator.Page_By_FileName = source + "\\" + filename + "\\default.html";
										}
										else
										{
											if (navigator.Default_Aggregation == "all")
											{
												navigator.Info_Browse_Mode = "default";
												navigator.Page_By_FileName = base_source + "\\default.html";
											}
										}
									}

									// Last choice would be if this is a default aggregation
									if ((navigator.Page_By_FileName.Length == 0) && (navigator.Default_Aggregation != "all"))
									{
										aggregation_querystring_analyze(navigator, QueryString, navigator.Default_Aggregation, url_relative_list);
									}
								}
								break;
						}
					}
				}				
			}
		}

		#endregion

		private void aggregation_querystring_analyze(SobekCM_Navigation_Object navigator, NameValueCollection QueryString, string Aggregation, List<string> remaining_url_redirect_list)
		{
			navigator.Aggregation = Aggregation;
			navigator.Mode = Display_Mode_Enum.Aggregation_Home;

            // Collect any search and search field values
            if (QueryString["t"] != null) navigator.Search_String = QueryString["t"].Trim();
            if (QueryString["f"] != null) navigator.Search_Fields = QueryString["f"].Trim();

			// Look for any more url information
			if (remaining_url_redirect_list.Count > 0)
			{
				switch (remaining_url_redirect_list[0])
				{
					case "usage":
						navigator.Mode = Display_Mode_Enum.Aggregation_Usage_Statistics;
						if (remaining_url_redirect_list.Count > 1)
						{
							navigator.Info_Browse_Mode = remaining_url_redirect_list[1];
						}
						break;

					case "itemcount":
						navigator.Mode = Display_Mode_Enum.Aggregation_Item_Count;
						if (remaining_url_redirect_list.Count > 1)
						{
							navigator.Info_Browse_Mode = remaining_url_redirect_list[1];
						}
						break;

					case "inprocess":
						navigator.Mode = Display_Mode_Enum.Aggregation_Private_Items;
						navigator.Page = 1;
						if (remaining_url_redirect_list.Count > 1)
						{
							if (is_String_Number(remaining_url_redirect_list[1]))
								navigator.Page = Convert.ToUInt16(remaining_url_redirect_list[1]);
						}
						if ((QueryString["o"] != null) && (is_String_Number(QueryString["o"])))
						{
							navigator.Sort = Convert.ToInt16(QueryString["o"]);
						}
						else
						{
							navigator.Sort = 0;
						}
						break;

					case "admin":
						navigator.Mode = Display_Mode_Enum.Aggregation_Admin_View;
						break;


					case "contact":
						navigator.Mode = Display_Mode_Enum.Contact;
						if (remaining_url_redirect_list.Count > 1)
						{
							if (remaining_url_redirect_list[1] == "sent")
							{
								navigator.Mode = Display_Mode_Enum.Contact_Sent;
							}
						}
						if (QueryString["em"] != null)
							navigator.Error_Message = QueryString["em"];
						break;

					case "map":
						navigator.Mode = Display_Mode_Enum.Search;
						navigator.Search_Type = Search_Type_Enum.Map;
						if (remaining_url_redirect_list.Count > 1)
						{
							navigator.Info_Browse_Mode = remaining_url_redirect_list[1];
						}
						break;

					case "advanced":
						navigator.Mode = Display_Mode_Enum.Search;
						navigator.Search_Type = Search_Type_Enum.Advanced;
						break;

					case "text":
						navigator.Mode = Display_Mode_Enum.Search;
						navigator.Search_Type = Search_Type_Enum.Full_Text;
						break;

					case "info":
						navigator.Mode = Display_Mode_Enum.Aggregation_Browse_Info;
						if (remaining_url_redirect_list.Count > 1)
							navigator.Info_Browse_Mode = remaining_url_redirect_list[1];
						break;

					case "browseby":
						navigator.Mode = Display_Mode_Enum.Aggregation_Browse_By;
						if (remaining_url_redirect_list.Count > 1)
							navigator.Info_Browse_Mode = remaining_url_redirect_list[1];
						if ( remaining_url_redirect_list.Count > 2 )
						{
							string possible_page = remaining_url_redirect_list[2];
							bool isNumber = possible_page.All(Char.IsNumber);
							if ( isNumber )
							{
								navigator.Page = Convert.ToUInt16(possible_page);
							}
						}
						break;

					case "geography":
						navigator.Mode = Display_Mode_Enum.Aggregation_Browse_Map;
						break;

					case "results":
					case "resultslike":
					case "contains":
					case "exact":
						switch (remaining_url_redirect_list[0])
						{
							case "results":
								navigator.Search_Precision = Search_Precision_Type_Enum.Inflectional_Form;
								break;

							case "resultslike":
								navigator.Search_Precision = Search_Precision_Type_Enum.Synonmic_Form;
								break;

							case "contains":
								navigator.Search_Precision = Search_Precision_Type_Enum.Contains;
								break;

							case "exact":
								navigator.Search_Precision = Search_Precision_Type_Enum.Exact_Match;
								break;
						}
						navigator.Mode = Display_Mode_Enum.Results;
						navigator.Search_Type = Search_Type_Enum.Basic;
						navigator.Result_Display_Type = Result_Display_Type_Enum.Default;
						navigator.Page = 1;
						navigator.Sort = 0;
						if ((HttpContext.Current != null) && (HttpContext.Current.Session != null) && (HttpContext.Current.Session["User_Default_Sort"] != null))
							navigator.Sort = Convert.ToInt16(HttpContext.Current.Session["User_Default_Sort"]);

						int search_handled_args = 1;

						// Look for result display type
						if (remaining_url_redirect_list.Count > search_handled_args)
						{
							switch (remaining_url_redirect_list[search_handled_args])
							{
								case "brief":
									navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;
									search_handled_args++;
									break;
								case "export":
									navigator.Result_Display_Type = Result_Display_Type_Enum.Export;
									search_handled_args++;
									break;
								case "citation":
									navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Citation;
									search_handled_args++;
									break;
								case "image":
									navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Image;
									search_handled_args++;
									break;
								case "map":
									navigator.Result_Display_Type = Result_Display_Type_Enum.Map;
									search_handled_args++;
									break;
								case "table":
									navigator.Result_Display_Type = Result_Display_Type_Enum.Table;
									search_handled_args++;
									break;
								case "thumbs":
									navigator.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
									search_handled_args++;
									break;
							}
						}

						// Look for a page number
						if (remaining_url_redirect_list.Count > search_handled_args)
						{
							string possible_page = remaining_url_redirect_list[search_handled_args];
							if ((possible_page.Length > 0) && (is_String_Number(possible_page)))
							{
								ushort page_result ;
								UInt16.TryParse(possible_page, out page_result);
								navigator.Page = page_result;
							}
						}

						// Collect the coordinate information from the URL query string
						if (QueryString["coord"] != null)
						{
							navigator.Coordinates = QueryString["coord"].Trim();
							if (navigator.Coordinates.Length > 0)
							{
								navigator.Search_Type = Search_Type_Enum.Map;
								navigator.Search_Fields = String.Empty;
								navigator.Search_String = String.Empty;
							}
						}

                        //// Look for non-map type search information
                        //if ( navigator.Coordinates.Length == 0 )
                        //{
                        //    // Collect any search value
                        //    if (QueryString["t"] != null)
                        //        navigator.Search_String = QueryString["t"].Trim();

                        //    // Collect any fields value
                        //    if (QueryString["f"] != null)
                        //        navigator.Search_Fields = QueryString["f"].Trim();
                        //}

						// Was a search string and fields included?
						if ((navigator.Search_String.Length > 0) && (navigator.Search_Fields.Length > 0))
						{
							navigator.Search_Type = Search_Type_Enum.Advanced;
						}
						else
						{
							navigator.Search_Fields = "ZZ";
						}

						// If no search term, look foor the TEXT-specific term
						if (navigator.Search_String.Length == 0)
						{
							if (QueryString["text"] != null)
							{
								navigator.Search_String = QueryString["text"].Trim();
							}

							if (navigator.Search_String.Length > 0)
							{
								navigator.Search_Type = Search_Type_Enum.Full_Text;
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
								navigator.Sort = sort_result;
							}
						}
						break;

					default:
						navigator.Mode = Display_Mode_Enum.Aggregation_Browse_Info;
						navigator.Info_Browse_Mode = remaining_url_redirect_list[0];
						navigator.Result_Display_Type = Result_Display_Type_Enum.Default;
						navigator.Page = 1;
						navigator.Sort = 0;
						if ((HttpContext.Current != null) && (HttpContext.Current.Session != null) && (HttpContext.Current.Session["User_Default_Sort"] != null))
							navigator.Sort = Convert.ToInt16(HttpContext.Current.Session["User_Default_Sort"]);
						int aggr_handled_args = 1;

						// Look for result display type
						if (remaining_url_redirect_list.Count > aggr_handled_args)
						{
							switch ( remaining_url_redirect_list[ aggr_handled_args ] )
							{
								case "brief":
									navigator.Result_Display_Type = Result_Display_Type_Enum.Brief;
									aggr_handled_args++;
									break;
								case "export":
									navigator.Result_Display_Type = Result_Display_Type_Enum.Export;
									aggr_handled_args++;
									break;
								case "citation":
									navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Citation;
									aggr_handled_args++;
									break;
								case "image":
									navigator.Result_Display_Type = Result_Display_Type_Enum.Full_Image;
									aggr_handled_args++;
									break;
								case "map":
									navigator.Result_Display_Type = Result_Display_Type_Enum.Map;
									aggr_handled_args++;
									break;
								case "table":
									navigator.Result_Display_Type = Result_Display_Type_Enum.Table;
									aggr_handled_args++;
									break;
								case "thumbs":
									navigator.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
									aggr_handled_args++;
									break;
							}
						}

						// Look for a page number
						if (remaining_url_redirect_list.Count > aggr_handled_args)
						{
							string possible_page = remaining_url_redirect_list[aggr_handled_args];
							if (( possible_page.Length > 0 ) && ( is_String_Number(possible_page)))
							{
								ushort page_result;
								UInt16.TryParse(possible_page, out page_result);
								navigator.Page = page_result;
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
								navigator.Sort = sort_result;
							}
						}

						break;
				}
			}
		}

		/// <summary> Method checks to see if this string contains only numbers </summary>
		/// <param name="test_string"> string to check for all numerals </param>
		/// <returns> TRUE if the string is made of all numerals </returns>
		/// <remarks> This just steps through each character in the string and tests with the Char.IsNumber method</remarks>
		private static bool is_String_Number(string test_string)
		{
			// Step through each character and return false if not a number
			return test_string.All(Char.IsNumber);
		}
	}
}
