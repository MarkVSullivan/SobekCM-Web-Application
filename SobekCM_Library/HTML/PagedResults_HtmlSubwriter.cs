#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.Email;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Library.ResultsViewer;
using SobekCM.Library.Search;
using SobekCM.Library.Settings;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.HTML
{
	/// <summary> Accepts a result set of titles and items and renders the correct page of results in the result view the user has requested </summary>
	/// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
	public class PagedResults_HtmlSubwriter : abstractHtmlSubwriter
	{
		private const int MINIMIZED_FACET_COUNT = 10;
		private const int MAXIMIZED_FACET_COUNT = 100;
		private const int RESULTS_PER_PAGE = 20;

		private readonly Item_Lookup_Object allItems;
		private string leftButtons;
		private string rightButtons;
		private readonly Aggregation_Code_Manager codeManager;
		private readonly User_Object currentUser;
		private readonly string facetInformation;
		private readonly List<iSearch_Title_Result> pagedResults;
		private iResultsViewer resultWriter;
		private readonly Search_Results_Statistics resultsStatistics;
		private string sortOptions;
		private readonly Language_Support_Info translations;
		private int term_counter;

		/// <summary> Constructor for a new instance of the paged_result_html_subwriter class </summary>
		/// <param name="Results_Statistics"> Information about the entire set of results for a search or browse </param>
		/// <param name="Paged_Results"> Single page of results for a search or browse, within the entire set </param>
		/// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
		/// <param name="Translator"> Language support object which handles simple translational duties </param>
		/// <param name="All_Items_Lookup"> Lookup object used to pull basic information about any item loaded into this library </param>
		/// <param name="Current_User"> Currently logged on user </param>
		/// <param name="Current_Mode"> Mode / navigation information for the current request</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public PagedResults_HtmlSubwriter(Search_Results_Statistics Results_Statistics,
			List<iSearch_Title_Result> Paged_Results,
			Aggregation_Code_Manager Code_Manager,
			Language_Support_Info Translator, Item_Lookup_Object All_Items_Lookup, 
			User_Object Current_User,
			SobekCM_Navigation_Object Current_Mode,
			Custom_Tracer Tracer )
		{
			currentUser = Current_User;
			pagedResults = Paged_Results;
			resultsStatistics = Results_Statistics;
			codeManager = Code_Manager;
			translations = Translator;
			Browse_Title = String.Empty;
			allItems = All_Items_Lookup;
			sortOptions = String.Empty;
			leftButtons = String.Empty;
			rightButtons = String.Empty;
			Showing_Text = String.Empty;
			Include_Bookshelf_View = false;
			Outer_Form_Name = String.Empty;
			currentMode= Current_Mode;
			Folder_Owner_Name = String.Empty;
			Folder_Owner_Email = String.Empty;
			term_counter = 0;

			// Try to get the facet configuration information
			facetInformation = "0000000";
			if (HttpContext.Current.Request.Form["facet"] != null)
				facetInformation = HttpContext.Current.Request.Form["facet"].PadRight(7, '0');

			if ( true ) // if (currentMode.isPostBack)
			{
				// Pull the standard values
				NameValueCollection form = HttpContext.Current.Request.Form;

			
				if (form["item_action"] != null)
				{
					string action = form["item_action"].ToLower().Trim();
					string url_description = form["url_description"].Trim();
					
					if (action == "email")
					{
						string address = form["email_address"].Replace(";", ",").Trim();
						string comments = form["email_comments"].Trim();
						string format = form["email_format"].Trim().ToUpper();
						string list_type = form["list_type"].Trim();

						if (address.Length > 0)
						{
							// Determine the email format
							bool is_html_format = format != "TEXT";

							// CC: the user, unless they are already on the list
							string cc_list = currentUser.Email;
							if (address.ToUpper().IndexOf(currentUser.Email.ToUpper()) >= 0)
								cc_list = String.Empty;

							// Send the email
							string any_error = URL_Email_Helper.Send_Email(address, cc_list, comments, currentUser.Full_Name, currentMode.SobekCM_Instance_Abbreviation, is_html_format, HttpContext.Current.Items["Original_URL"].ToString(), url_description, list_type, currentUser.UserID);
							HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", any_error.Length > 0 ? any_error : "Your email has been sent");

							currentMode.isPostBack = true;

							// Do this to force a return trip (cirumnavigate cacheing)
							string original_url = HttpContext.Current.Items["Original_URL"].ToString();
							if ( original_url.IndexOf("?") < 0 )
								HttpContext.Current.Response.Redirect(original_url + "?p=" + DateTime.Now.Millisecond, false);
							else
								HttpContext.Current.Response.Redirect(original_url + "&p=" + DateTime.Now.Millisecond, false);

							HttpContext.Current.ApplicationInstance.CompleteRequest();
							Current_Mode.Request_Completed = true;
							return;
						}
					}

					if (action == "save_search")
					{
						string usernotes = HttpContext.Current.Request.Form["add_notes"].Trim();
						bool open_searches = HttpContext.Current.Request.Form["open_searches"] != null;

						string original_url = HttpContext.Current.Items["Original_URL"].ToString();
						if (SobekCM_Database.Save_User_Search(currentUser.UserID, original_url , url_description, 0, usernotes, Tracer) != -1000)
						{
							if (open_searches)
							{
								HttpContext.Current.Session.Add("ON_LOAD_WINDOW", "?m=lms");
							}

							HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", "Search has been saved to your saved searches.");
						}
						else
						{
							HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", "ERROR encountered while saving!");
						}

						// Do this to force a return trip (cirumnavigate cacheing)
						currentMode.isPostBack = true;
						if (original_url.IndexOf("?") > 0)
						{
							HttpContext.Current.Response.Redirect(original_url + "&p=" + DateTime.Now.Millisecond, false);
						}
						else
						{
							HttpContext.Current.Response.Redirect(original_url + "?p=" + DateTime.Now.Millisecond, false);
						}
						HttpContext.Current.ApplicationInstance.CompleteRequest();
						Current_Mode.Request_Completed = true;
					}
				}
			}
		}

		/// <summary> If the results dataset should be displayed in the context of an outer form (such as in
		/// the case that this is part of the mySobek bookshelf functionality) then the form name should go here.  If 
		/// no outer form name is provided, this will create its own sort form  </summary>
		public string Outer_Form_Name { get; set; }

		/// <summary> Flag indicates if the bookshelf view should be included in the list of possible views  </summary>
		public bool Include_Bookshelf_View { get; set; }

		/// <summary> Name of the owner of this folder </summary>
		public string Folder_Owner_Name { private get; set; }

		/// <summary> Email of the owner of this folder </summary>
		public string Folder_Owner_Email { private get; set; }

		/// <summary> Text which indicates which values of the current result or browse are being shown</summary>
		public string Showing_Text { get; private set; }

		/// <summary> Title for the current view, which is used rather than the search explanation </summary>
		public string Browse_Title { get; set; }


		/// <summary> Creates the specific results viewer according the user's preferences in the current request mode </summary>
		private void create_resultwriter()
		{
			if ( resultsStatistics.Total_Items == 0)
			{
				resultWriter = new No_Results_ResultsViewer
								   {CurrentMode = currentMode, Results_Statistics = resultsStatistics};
				return;
			}

			// If this is default, determine the type from the aggregation (currently) or user
			if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Default)
			{
				if (currentMode.Coordinates.Length > 0)
					currentMode.Result_Display_Type = Result_Display_Type_Enum.Map;
				else
				{
					string user_view = "default";
					if (HttpContext.Current.Session["User_Default_View"] != null)
						user_view = HttpContext.Current.Session["User_Default_View"].ToString();
					currentMode.Result_Display_Type = Current_Aggregation.Default_Result_View;
					switch (user_view)
					{
						case "brief":
							if (Current_Aggregation.Result_Views.Contains(Result_Display_Type_Enum.Brief))
								currentMode.Result_Display_Type = Result_Display_Type_Enum.Brief;
							break;

						case "thumb":
							if (Current_Aggregation.Result_Views.Contains(Result_Display_Type_Enum.Thumbnails))
								currentMode.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
							break;

						case "table":
							if (Current_Aggregation.Result_Views.Contains(Result_Display_Type_Enum.Table))
								currentMode.Result_Display_Type = Result_Display_Type_Enum.Table;
							break;

					}

				}
			}

			// Create the bookshelf view
			if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Bookshelf)
			{
				if (currentMode.Mode == Display_Mode_Enum.My_Sobek)
				{
					resultWriter = new Bookshelf_View_ResultsViewer(allItems, currentUser);
				}
				else
				{
					resultWriter = new Brief_ResultsViewer(allItems);
				}
			}

			// Create the result writer and populate the sort list for BRIEF view
			if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Brief)
			{
				resultWriter = new Brief_ResultsViewer(allItems);
			}

			// Create the result writer and populate the sort list for THUMBNAIL view
			if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Thumbnails)
			{
				resultWriter = new Thumbnail_ResultsViewer(allItems);
				((Thumbnail_ResultsViewer)resultWriter).Code_Manager = codeManager;
			}

			// Create the result writer and populate the sort list for TABLE view
			if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Table)
			{
				resultWriter = new Table_ResultsViewer(allItems);
			}

			// Create the result writer and populate the sort list for FULL view
			if ((currentMode.Result_Display_Type == Result_Display_Type_Enum.Full_Citation) || (currentMode.Result_Display_Type == Result_Display_Type_Enum.Full_Image))
			{
				resultWriter = new Full_ResultsViewer(allItems);
			}

			// Create the result writer and populate the sort list for MAP view
			if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Map)
			{
				resultWriter = new Google_Map_ResultsViewer(allItems);
			}

            // Create the result writer and populate the sort list for MAP view
            if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Map_Beta)
            {
                resultWriter = new Google_Map_ResultsViewer_Beta(currentMode, allItems);
            }

			// Create the result writer and populate the sort list for TEXT view
			if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Export)
			{
				resultWriter = new Export_File_ResultsViewer(allItems, currentUser);
			}

			resultWriter.CurrentMode = currentMode;
			resultWriter.Results_Statistics = resultsStatistics;
			resultWriter.Paged_Results = pagedResults;
			resultWriter.HierarchyObject = Current_Aggregation;
			resultWriter.Translator = translations;
			
			// Populate the sort list and sort the result set
			sortOptions = String.Empty;
			StringBuilder sort_options_builder = new StringBuilder(1000);
			if ((resultWriter.Sortable) && (!currentMode.Is_Robot))
			{
				// Add the special sorts for browses
				if (currentMode.Mode == Display_Mode_Enum.Aggregation) // browse info only
				{
					if (currentMode.Info_Browse_Mode.ToUpper().IndexOf("NEW") >= 0)
					{
						if (currentMode.Sort == 0)
						{
							sort_options_builder.Append("      <option value=\"" + 0 + "\" selected=\"selected\">" + translations.Get_Translation("Date Added", currentMode.Language) + "</option>" + Environment.NewLine );
						}
						else
						{
							sort_options_builder.Append("      <option value=\"" + 0 + "\">" + translations.Get_Translation("Date Added", currentMode.Language) + "</option>" + Environment.NewLine );
						}

						if (currentMode.Sort == 1)
						{
							sort_options_builder.Append("      <option value=\"" + 1 + "\" selected=\"selected\">" + translations.Get_Translation("Title", currentMode.Language) + "</option>" + Environment.NewLine );
						}
						else
						{
							sort_options_builder.Append("      <option value=\"" + 1 + "\">" + translations.Get_Translation("Title", currentMode.Language) + "</option>" + Environment.NewLine );
						}
					}
					else
					{
						if ((currentMode.Sort == 0) || (currentMode.Sort == 1))
						{
							sort_options_builder.Append("      <option value=\"" + 0 + "\" selected=\"selected\">" + translations.Get_Translation("Title", currentMode.Language) + "</option>" + Environment.NewLine );
						}
						else
						{
							sort_options_builder.Append("      <option value=\"" + 0 + "\">" + translations.Get_Translation("Title", currentMode.Language) + "</option>" + Environment.NewLine );
						}
					}
				}

				// Add the special sorts for searches
				if (currentMode.Mode == Display_Mode_Enum.Results)
				{
					if (currentMode.Sort == 0)
					{
						sort_options_builder.Append("      <option value=\"" + 0 + "\" selected=\"selected\">" + translations.Get_Translation("Rank", currentMode.Language) + "</option>" + Environment.NewLine );
					}
					else
					{
						sort_options_builder.Append("      <option value=\"" + 0 + "\">" + translations.Get_Translation("Rank", currentMode.Language) + "</option>" + Environment.NewLine );
					}

					if (currentMode.Sort == 1)
					{
						sort_options_builder.Append("      <option value=\"" + 1 + "\" selected=\"selected\">" + translations.Get_Translation("Title", currentMode.Language) + "</option>" + Environment.NewLine );
					}
					else
					{
						sort_options_builder.Append("      <option value=\"" + 1 + "\">" + translations.Get_Translation("Title", currentMode.Language) + "</option>" + Environment.NewLine );
					}
				}

				// Add the bibid sorts if this is an internal user
				if (currentMode.Internal_User)
				{
					if (currentMode.Sort == 2)
					{
						sort_options_builder.Append("      <option value=\"" + 2 + "\" selected=\"selected\">" + translations.Get_Translation("BibID Ascending", currentMode.Language) + "</option>" + Environment.NewLine );
					}
					else
					{
						sort_options_builder.Append("      <option value=\"" + 2 + "\">" + translations.Get_Translation("BibID Ascending", currentMode.Language) + "</option>" + Environment.NewLine );
					}

					if (currentMode.Sort == 3)
					{
						sort_options_builder.Append("      <option value=\"" + 3 + "\" selected=\"selected\">" + translations.Get_Translation("BibID Descending", currentMode.Language) + "</option>" + Environment.NewLine );
					}
					else
					{
						sort_options_builder.Append("      <option value=\"" + 3 + "\">" + translations.Get_Translation("BibID Descending", currentMode.Language) + "</option>" + Environment.NewLine );
					}
				}

				// Add the publication date sorts
				if (currentMode.Sort == 10)
				{
					sort_options_builder.Append("      <option value=\"" + 10 + "\" selected=\"selected\">" + translations.Get_Translation("Date Ascending", currentMode.Language) + "</option>" + Environment.NewLine );
				}
				else
				{
					sort_options_builder.Append("      <option value=\"" + 10 + "\">" + translations.Get_Translation("Date Ascending", currentMode.Language) + "</option>" + Environment.NewLine );
				}

				if (currentMode.Sort == 11)
				{
					sort_options_builder.Append("      <option value=\"" + 11 + "\" selected=\"selected\">" + translations.Get_Translation("Date Descending", currentMode.Language) + "</option>" + Environment.NewLine );
				}
				else
				{
					sort_options_builder.Append("      <option value=\"" + 11 + "\">" + translations.Get_Translation("Date Descending", currentMode.Language) + "</option>" + Environment.NewLine );
				}

				sortOptions = sort_options_builder.ToString();
			}
		}

		/// <summary> Adds controls to the main navigational page </summary>
		/// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles and sorted by serial hierarchy </returns>
		public void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer )
		{
			Tracer.Add_Trace("paged_result_html_subwriter.Add_Controls", "Adding controls for the result set");

			// If the results have facets, this should be rendered in a table with the facets to the left
			if ((resultsStatistics.Has_Facet_Info) && (resultsStatistics.Total_Items > 1) && (currentMode.Result_Display_Type != Result_Display_Type_Enum.Export) && (currentMode.Result_Display_Type != Result_Display_Type_Enum.Map))
			{
				// Start this table, write the facets, and start the next TD section for the results
				Literal startFacetTable = new Literal { Text = string.Format("<table id=\"sbkPrsw_ResultsOuterTable\">" + Environment.NewLine + "<tr style=\"vertical-align:top;\">" + Environment.NewLine + "<td id=\"sbkPrsw_FacetOuterColumn\">" + Environment.NewLine + "{0}" + Environment.NewLine + "</td>" + Environment.NewLine + "<td>" + Environment.NewLine, Add_Facet_Information(Tracer)) };
				MainPlaceHolder.Controls.Add(startFacetTable);
			}
			else
			{
				Literal startFacetTable = new Literal { Text = "<table style=\"width:100%;\">" + Environment.NewLine + "<tr style=\"vertical-align:top;\">" + Environment.NewLine + "<td style=\"text-align:center\">" + Environment.NewLine };
				MainPlaceHolder.Controls.Add(startFacetTable);
			}

			// Make sure the result writer has been created
			if (resultWriter == null)
				create_resultwriter();
			Debug.Assert(resultWriter != null, "resultWriter != null");
			if (resultWriter == null)
				return;


			if (resultsStatistics.Total_Items == 0)
			{
				resultWriter.Add_HTML(MainPlaceHolder, Tracer);
				return;
			}

			Literal startingLiteral = new Literal{ Text = (currentMode.Result_Display_Type == Result_Display_Type_Enum.Map || currentMode.Result_Display_Type == Result_Display_Type_Enum.Map_Beta) ? "</div>" + Environment.NewLine + "<div class=\"sbkPrsw_ResultsPanel\">" + Environment.NewLine : "<div class=\"sbkPrsw_ResultsPanel\">" + Environment.NewLine};
			MainPlaceHolder.Controls.Add(startingLiteral);

			resultWriter.Add_HTML(MainPlaceHolder, Tracer );

            Literal endingLiteral = new Literal { Text = (currentMode.Result_Display_Type == Result_Display_Type_Enum.Map || currentMode.Result_Display_Type == Result_Display_Type_Enum.Map_Beta) ? "</div>" + Environment.NewLine + "<div id=\"pagecontainer_resumed\">" + Environment.NewLine : "</div>" + Environment.NewLine };
			MainPlaceHolder.Controls.Add(endingLiteral);

			// If the results have facets, end the result table
			Literal endResultTable = new Literal {Text = "</td>" + Environment.NewLine + "</tr>" + Environment.NewLine + "</table>" + Environment.NewLine };
			MainPlaceHolder.Controls.Add(endResultTable);
		}

		/// <summary> Writes the final output to close this result view, including the results page navigation buttons </summary>
		/// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("paged_result_html_subwriter.Write_Final_Html", "Rendering HTML ( finish the main viewer section )");

			if ( resultsStatistics.Total_Items > 0 )
			{
				Output.WriteLine("<div class=\"sbkPrsw_ResultsNavBar\">");
				Output.Write(leftButtons);
				Output.WriteLine("  " + Showing_Text);
				Output.Write(rightButtons);
				Output.WriteLine("</div>");
				Output.WriteLine("<br />");
				Output.WriteLine();
			}
		}

		/// <summary> Writes the HTML generated to browse the list of titles/itemsr  directly to the response stream </summary>
		/// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> TRUE -- Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
		public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("paged_result_html_subwriter.Write_HTML", "Rendering HTML");


			string sort_by = "Sort By";
			string showing_range_text = "{0} - {1} of {2} matching titles";
			string showing_coord_range_text = "{0} - {1} of {2} matching coordinates";

			if (currentMode.Aggregation == "aerials")
			{
				showing_coord_range_text = "{0} - {1} of {2} matching flights";
			}

			if (currentMode.Language == Web_Language_Enum.Spanish)
			{
				sort_by = "Organizar";
				showing_range_text = "{0} - {1} de {2} títulos correspondientes";
			}

			if (currentMode.Language == Web_Language_Enum.French)
			{
				sort_by = "Limiter";
				showing_range_text = "{0} - {1} de {2} titres correspondants";
			}

			if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Map)
				showing_range_text = showing_coord_range_text;

            if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Map_Beta)
                showing_range_text = showing_coord_range_text;

			Display_Mode_Enum initialMode = currentMode.Mode;

			Tracer.Add_Trace("paged_result_html_subwriter.Write_HTML", "Building appropriate ResultsWriter");

			currentMode.Mode = initialMode;
			if (currentMode.Mode == Display_Mode_Enum.Search)
				currentMode.Mode = Display_Mode_Enum.Results;

			// If no results, display different information here
			if ((currentMode.Mode == Display_Mode_Enum.Results) && ( resultsStatistics.Total_Items == 0))
			{
				Output.WriteLine("<div class=\"sbkPrsw_DescPanel\" style=\"margin-top:10px\">");
				Show_Search_Info(Output);
				Output.WriteLine("</div>");
				Output.WriteLine("<div class=\"sbkPrsw_ResultsNavBar\">&nbsp;</div>");
				return true;
			}

			// Make sure the result writer has been created
			if (resultWriter == null)
				create_resultwriter();
		   Debug.Assert(resultWriter != null, "resultWriter != null");
			if (resultWriter == null)
				return false;

			// Determine which rows are being displayed
			int lastRow = currentMode.Page * RESULTS_PER_PAGE;
			int startRow = lastRow - 19;

			// Start the form for this, unless we are already in an appropriate form
			string form_name = Outer_Form_Name;
			if (form_name.Length == 0)
				form_name = "sort_form";
			if (Outer_Form_Name.Length == 0)
			{
				string post_url = HttpUtility.HtmlEncode(HttpContext.Current.Items["Original_URL"].ToString());
				Output.WriteLine("<form name=\"sort_form\" id=\"addedForm\" method=\"post\" action=\"" + post_url + "\" >");
			}

			// Get the name of this
			string currentName = "browse";
			string currentTitle = "Browse";
			if (currentMode.Mode == Display_Mode_Enum.Results)
			{
				currentName = "search";
				currentTitle = "Search";
			}
			if (currentMode.Mode == Display_Mode_Enum.Public_Folder)
			{
				currentName = "public bookshelf";
				currentTitle = "Public Bookshelf";
			}

			// Load the HTML that can be used to customize the search/results bar
			string html_source = String.Empty;
			string fileToRead = HttpContext.Current.Server.MapPath("default/fragments/search_browse_bar.html");
			if (File.Exists(fileToRead))
			{
				html_source = File.ReadAllText(fileToRead);
			}
			
			// Get the value for the <%SORTER%> directive (to sort the results)
			string SORTER = String.Empty;
			if ((resultWriter.Sortable) && (!currentMode.Is_Robot) && (currentMode.Mode != Display_Mode_Enum.My_Sobek) && (currentMode.Mode != Display_Mode_Enum.Public_Folder))
			{
				StringBuilder sorterBuilder = new StringBuilder("  <div class=\"sbkPrsw_ResultsSort\">");
				short current_order = currentMode.Sort;
				currentMode.Sort = 0;
				string url = currentMode.Redirect_URL();
				currentMode.Sort = current_order;
				sorterBuilder.AppendLine("    " + sort_by + ": &nbsp;");
				sorterBuilder.AppendLine("    <select name=\"sorter_input\" onchange=\"sort_results('" + url.Replace("&", "&amp;") + "')\" id=\"sorter_input\" class=\"sbkPrsw_SorterDropDown\">");
				sorterBuilder.AppendLine(sortOptions);
				sorterBuilder.AppendLine("    </select>");
				sorterBuilder.AppendLine("  </div>");
				SORTER = sorterBuilder.ToString();
			}

			// Get the value for the <%DESCRIPTION%> directive (to explain current display)
			string DESCRIPTION = String.Empty;
			string summation;
			if ((currentMode.Mode == Display_Mode_Enum.Aggregation) || (currentMode.Mode == Display_Mode_Enum.Public_Folder) || ((currentMode.Mode == Display_Mode_Enum.My_Sobek) && (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Folder_Management))) // browse info only for aggregation
			{
				if (currentMode.Mode == Display_Mode_Enum.Public_Folder)
				{
					DESCRIPTION = "<h1>&quot;" + translations.Get_Translation(Browse_Title, currentMode.Language) + "&quot;</h1>" + Environment.NewLine + "  <span class=\"sbkPrsw_PublicFolderAuthor\">This is a publicly shared bookshelf of <a href=\"mailto:" + Folder_Owner_Email + "\">" + Folder_Owner_Name + "</a>.</span>";

					summation = translations.Get_Translation(Browse_Title, currentMode.Language) + " (publicly shared folder)";
				}
				else
				{
					DESCRIPTION = "<h1>" + translations.Get_Translation(Browse_Title, currentMode.Language) + "</h1>";
					summation = translations.Get_Translation(Browse_Title, currentMode.Language) + " browse in " + Current_Aggregation.Name; 
				}                   
			}
			else
			{
				StringBuilder descriptionBuilder = new StringBuilder();
				descriptionBuilder.Append("<div class=\"sbkPrsw_ResultsExplanation\">");
				StringBuilder searchInfoBuilder = new StringBuilder();
				StringWriter writer = new StringWriter(searchInfoBuilder);
				Show_Search_Info(writer);
				summation = searchInfoBuilder.ToString().Replace("<i>", "").Replace("</i>", "").Replace("\"", "").Replace("'", "").Replace("\n", "").Replace("\r", "").Replace("&", "%26").Replace("</td>","");
				descriptionBuilder.Append(searchInfoBuilder);
				descriptionBuilder.Append("</div>");
				DESCRIPTION = descriptionBuilder.ToString();
			}


			// Get the value for the <%DESCRIPTION%> directive (to explain current display)
			//ushort current_page = currentMode.Page;
			string SHOWING = String.Empty;
			if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Export)
			{
				SHOWING = resultsStatistics.Total_Items.ToString();
			}
			else
			{
				SHOWING = String.Format(showing_range_text, startRow, Math.Min(lastRow, resultsStatistics.Total_Titles), resultWriter.Total_Results);
				if (startRow == lastRow)
				{
					SHOWING = Showing_Text.Replace(startRow + " - " + startRow, startRow + " ");
				}
			}

			// Get the values for the <%LEFTBUTTONS%> and <%RIGHTBUTTONS%>
			string LEFT_BUTTONS = String.Empty;
			string RIGHT_BUTTONS = String.Empty;
			string first_page = "First Page";
			string previous_page = "Previous Page";
			string next_page = "Next Page";
			string last_page = "Last Page";
			string first_page_text = "First";
			string previous_page_text = "Previous";
			string next_page_text = "Next";
			string last_page_text = "Last";

			if (currentMode.Language == Web_Language_Enum.Spanish)
			{
				first_page = "Primera Página";
				previous_page = "Página Anterior";
				next_page = "Página Siguiente";
				last_page = "Última Página";
				first_page_text = "Primero";
				previous_page_text = "Anterior";
				next_page_text = "Proximo";
				last_page_text = "Último";
			}

			if (currentMode.Language == Web_Language_Enum.French)
			{
				first_page = "Première Page";
				previous_page = "Page Précédente";
				next_page = "Page Suivante";
				last_page = "Dernière Page";
				first_page_text = "Première";
				previous_page_text = "Précédente";
				next_page_text = "Suivante";
				last_page_text = "Derniere";
			}

			// Make sure the result writer has been created
			if (resultWriter == null)
				create_resultwriter();
			if (resultWriter != null)
			{
				Debug.Assert(resultWriter != null, "resultWriter != null");

				if (RESULTS_PER_PAGE < resultWriter.Total_Results)
				{
					ushort current_page = currentMode.Page;
					StringBuilder buttons_builder = new StringBuilder(1000);

					// Should the previous and first buttons be enabled?
					if (current_page > 1)
					{
						buttons_builder.Append("<div class=\"sbkPrsw_LeftButtons\">");
						currentMode.Page = 1;
						buttons_builder.Append("<button title=\"" + first_page + "\" class=\"sbkPrsw_RoundButton\" onclick=\"window.location='" + currentMode.Redirect_URL().Replace("&", "&amp;") + "'; return false;\"><img src=\"" + currentMode.Base_URL + "default/images/button_first_arrow.png\" class=\"roundbutton_img_left\" alt=\"\" />" + first_page_text + "</button>&nbsp;");
						currentMode.Page = (ushort)(current_page - 1);
						buttons_builder.Append("<button title=\"" + previous_page + "\" class=\"sbkPrsw_RoundButton\" onclick=\"window.location='" + currentMode.Redirect_URL().Replace("&", "&amp;") + "'; return false;\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"roundbutton_img_left\" alt=\"\" />" + previous_page_text + "</button>");
						buttons_builder.Append("</div>");
						LEFT_BUTTONS = buttons_builder.ToString();
						buttons_builder.Clear();
					}
					else
					{
						LEFT_BUTTONS = "<div class=\"sbkPrsw_NoLeftButtons\">&nbsp;</div>";
					}


					// Should the next and last buttons be enabled?
					if ((current_page * RESULTS_PER_PAGE) < resultWriter.Total_Results)
					{
						buttons_builder.Append("<div class=\"sbkPrsw_RightButtons\">");
						currentMode.Page = (ushort)(current_page + 1);
						buttons_builder.Append("<button title=\"" + next_page + "\" class=\"sbkPrsw_RoundButton\" onclick=\"window.location='" + currentMode.Redirect_URL().Replace("&", "&amp;") + "'; return false;\">" + next_page_text + "<img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"roundbutton_img_right\" alt=\"\" /></button>&nbsp;");
						currentMode.Page = (ushort)(resultWriter.Total_Results / RESULTS_PER_PAGE);
						if (resultWriter.Total_Results % RESULTS_PER_PAGE > 0)
							currentMode.Page = (ushort)(currentMode.Page + 1);
						buttons_builder.Append("<button title=\"" + last_page + "\" class=\"sbkPrsw_RoundButton\" onclick=\"window.location='" + currentMode.Redirect_URL().Replace("&", "&amp;") + "'; return false;\">" + last_page_text + "<img src=\"" + currentMode.Base_URL + "default/images/button_last_arrow.png\" class=\"roundbutton_img_right\" alt=\"\" /></button>");
						buttons_builder.Append("</div>");
						RIGHT_BUTTONS = buttons_builder.ToString();
					}
					else
					{
						RIGHT_BUTTONS = "<div class=\"sbkPrsw_NoRightButtons\">&nbsp;</div>";
					}

					currentMode.Page = current_page;
				}
			}

			// Empty strings for now
			string brief_view = "BRIEF VIEW";
			string map_view = "MAP VIEW";
			string table_view = "TABLE VIEW";
			string thumbnail_view = "THUMBNAIL VIEW";
			if (Mode.Language == Web_Language_Enum.Spanish)
			{
				map_view = "VISTA MAPA";
				brief_view = "VISTA BREVE";
				table_view = "VISTA TABLERA";
				thumbnail_view = "VISTA MINIATURA";
			}
			if (Mode.Language == Web_Language_Enum.French)
			{
				map_view = "MODE CARTE";
				brief_view = "MODE SIMPLE";
				table_view = "MODE DE TABLE";
				thumbnail_view = "MODE IMAGETTE";
			}
			Result_Display_Type_Enum resultView = Mode.Result_Display_Type;
			StringBuilder iconBuilder = new StringBuilder(1000);
			iconBuilder.AppendLine();
			iconBuilder.AppendLine("    <div class=\"sbkPrsw_ViewIconButtons\">");
			if ((Mode.Coordinates.Length > 0) || (Current_Aggregation.Result_Views.Contains(Result_Display_Type_Enum.Map)))
			{
				if (resultView == Result_Display_Type_Enum.Map)
				{
					iconBuilder.AppendLine("      <img src=\"" + currentMode.Default_Images_URL + "geo_blue.png\" alt=\"MAP\" class=\"sbkPrsw_ViewIconButtonCurrent\"/>");
				}
				else
				{
					Mode.Result_Display_Type = Result_Display_Type_Enum.Map;
					iconBuilder.AppendLine("      <a href=\"" + Mode.Redirect_URL().Replace("&", "&amp;") + "\" title=\"" + map_view + "\"><img src=\"" + currentMode.Default_Images_URL + "geo_blue.png\" alt=\"MAP\" class=\"sbkPrsw_ViewIconButton\"/></a>");
				}
			}

            if ((Mode.Coordinates.Length > 0) || (Current_Aggregation.Result_Views.Contains(Result_Display_Type_Enum.Map_Beta)))
            {
                if (resultView == Result_Display_Type_Enum.Map_Beta)
                {
                    iconBuilder.AppendLine("      <img src=\"" + currentMode.Default_Images_URL + "geo_blue.png\" alt=\"MAP\" class=\"sbkPrsw_ViewIconButtonCurrent\"/>");
                }
                else
                {
                    Mode.Result_Display_Type = Result_Display_Type_Enum.Map_Beta;
                    iconBuilder.AppendLine("      <a href=\"" + Mode.Redirect_URL().Replace("&", "&amp;") + "\" title=\"" + map_view + "\"><img src=\"" + currentMode.Default_Images_URL + "geo_blue.png\" alt=\"MAP\" class=\"sbkPrsw_ViewIconButton\"/></a>");
                }
            }

			if (Current_Aggregation.Result_Views.Contains(Result_Display_Type_Enum.Brief))
			{
				if (resultView == Result_Display_Type_Enum.Brief)
				{
					iconBuilder.AppendLine("      <img src=\"" + currentMode.Default_Images_URL + "brief_blue.png\" alt=\"BRIEF\" class=\"sbkPrsw_ViewIconButtonCurrent\"/>");
				}
				else
				{
					Mode.Result_Display_Type = Result_Display_Type_Enum.Brief;
					iconBuilder.AppendLine("      <a href=\"" + Mode.Redirect_URL().Replace("&", "&amp;") + "\" title=\"" + brief_view + "\"><img src=\"" + currentMode.Default_Images_URL + "brief_blue.png\" alt=\"BRIEF\" class=\"sbkPrsw_ViewIconButton\"/></a>");
				}
			}

			if (Current_Aggregation.Result_Views.Contains(Result_Display_Type_Enum.Table))
			{
				if (resultView == Result_Display_Type_Enum.Table)
				{
					iconBuilder.AppendLine("      <img src=\"" + currentMode.Default_Images_URL + "table_blue.png\" alt=\"TABLE\" class=\"sbkPrsw_ViewIconButtonCurrent\"/>");
				}
				else
				{
					Mode.Result_Display_Type = Result_Display_Type_Enum.Table;
					iconBuilder.AppendLine("      <a href=\"" + Mode.Redirect_URL().Replace("&", "&amp;") + "\" title=\"" + table_view + "\"><img src=\"" + currentMode.Default_Images_URL + "table_blue.png\" alt=\"TABLE\" class=\"sbkPrsw_ViewIconButton\"/></a>");
				}
			}

			if (Current_Aggregation.Result_Views.Contains(Result_Display_Type_Enum.Thumbnails))
			{
				if (resultView == Result_Display_Type_Enum.Thumbnails)
				{
					iconBuilder.AppendLine("      <img src=\"" + currentMode.Default_Images_URL + "thumb_blue.png\" alt=\"THUMB\" class=\"sbkPrsw_ViewIconButtonCurrent\"/>");
				}
				else
				{
					Mode.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
					iconBuilder.AppendLine("      <a href=\"" + Mode.Redirect_URL().Replace("&", "&amp;") + "\" title=\"" + thumbnail_view + "\"><img src=\"" + currentMode.Default_Images_URL + "thumb_blue.png\" alt=\"THUMB\" class=\"sbkPrsw_ViewIconButton\"/></a>");
				}
			}
			Mode.Result_Display_Type = resultView;
			iconBuilder.AppendLine("    </div>");
			string VIEWICONS = iconBuilder.ToString();
            string NEWSEARCH = String.Empty;
            string ADDFILTER = String.Empty;
            
			// Start the division for the sort and then description and buttons, etc..
			switch (currentMode.Mode)
			{
				case Display_Mode_Enum.Public_Folder:
					Output.WriteLine("<div class=\"sbkPrsw_DescPanel sbkPrsw_FolderDescPanel\">");
					break;

				case Display_Mode_Enum.Aggregation:  // browse info only
					Output.WriteLine("<div class=\"sbkPrsw_DescPanel sbkPrsw_BrowseDescPanel\">");
					break;

				default:
					Output.WriteLine("<div class=\"sbkPrsw_DescPanel sbkPrsw_ResultsDescPanel\">");
					break;
			}

			// Now, write this 
			Output.WriteLine(html_source.Replace("<%DESCRIPTION%>", DESCRIPTION).Replace("<%NEWSEARCH%>", NEWSEARCH).Replace("<%ADDFILTER%>", ADDFILTER).Replace("<%VIEWICONS%>", VIEWICONS).Replace("<%LEFTBUTTONS%>", LEFT_BUTTONS).Replace("<%SHOWING%>", SHOWING).Replace("<%RIGHTBUTTONS%>", RIGHT_BUTTONS).Replace("<%SORTER%>", SORTER));

			// End this division
			Output.WriteLine("</div>");
			Output.WriteLine();

			// Configure the way to remove search terms
			if (( SobekCM_Library_Settings.Can_Remove_Single_Term ) && ( term_counter > 0 ))
			{
				Output.WriteLine("<script>");
				for (int i = 1; i <= term_counter; i++)
				{
					Output.WriteLine("  init_search_term('searchterm" + i + "', 'removesearchterm" + i + "');");
				}
				Output.WriteLine("</script>");
				Output.WriteLine();
			}

			// Save the buttons for later, to be used at the bottom of the page
			leftButtons = LEFT_BUTTONS;
			rightButtons = RIGHT_BUTTONS;
	
			// Determine the number of columns for text areas, depending on browser
			int actual_cols = 50;
			if (currentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0)
				actual_cols = 45;

			// Add the hidden field
			Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
			if (currentMode.Mode != Display_Mode_Enum.My_Sobek)
			{
				Output.WriteLine("<input type=\"hidden\" id=\"item_action\" name=\"item_action\" value=\"\" />");
			}
			Output.WriteLine("<input type=\"hidden\" id=\"url_description\" name=\"url_description\" value=\"" + HttpUtility.HtmlEncode(summation) + "\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"list_type\" name=\"list_type\" value=\"" + currentName + "\" />");
			Output.WriteLine();

			// Add the scripts needed
			if (currentMode.Mode != Display_Mode_Enum.My_Sobek)
			{
				Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.3.custom.min.js\"></script>");
				Output.WriteLine();
			}

			#region Email form

			if (currentUser != null)
			{
				Output.WriteLine("<!-- Email form -->");
				Output.WriteLine("<div class=\"email_popup_div\" id=\"form_email\" style=\"display:none;\">");
				Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">S<span class=\"smaller\">END THIS</span> " + currentTitle + "<span class=\"smaller\"> TO A</span> F<span class=\"smaller\">RIEND</span></td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"email_form_close()\">X</a> &nbsp; </td></tr></table></div>");
				Output.WriteLine("  <br />");
				Output.WriteLine("  <fieldset><legend>Enter the email information below &nbsp; </legend>");
				Output.WriteLine("    <br />");
				Output.WriteLine("    <table class=\"popup_table\">");


				// Add email address line
				Output.Write("      <tr align=\"left\"><td width=\"80px\"><label for=\"email_address\">To:</label></td>");
				Output.WriteLine("<td><input class=\"email_input\" name=\"email_address\" id=\"email_address\" type=\"text\" value=\"" + currentUser.Email + "\" onfocus=\"javascript:textbox_enter('email_address', 'email_input_focused')\" onblur=\"javascript:textbox_leave('email_address', 'email_input')\" /></td></tr>");

				// Add comments area
				Output.Write("      <tr align=\"left\" valign=\"top\"><td><br /><label for=\"email_comments\">Comments:</label></td>");
				Output.WriteLine("<td><textarea rows=\"6\" cols=\"" + actual_cols + "\" name=\"email_comments\" id=\"email_comments\" class=\"email_textarea\" onfocus=\"javascript:textbox_enter('email_comments','email_textarea_focused')\" onblur=\"javascript:textbox_leave('email_comments','email_textarea')\"></textarea></td></tr>");

				// Add format area
				Output.Write("      <tr align=\"left\" valign=\"top\"><td>Format:</td>");
				Output.Write("<td><input type=\"radio\" name=\"email_format\" id=\"email_format_html\" value=\"html\" checked=\"checked\" /> <label for=\"email_format_html\">HTML</label> &nbsp; &nbsp; ");
				Output.WriteLine("<input type=\"radio\" name=\"email_format\" id=\"email_format_text\" value=\"text\" /> <label for=\"email_format_text\">Plain Text</label></td></tr>");


				Output.WriteLine("    </table>");
				Output.WriteLine("    <br />");
				Output.WriteLine("  </fieldset><br />");
				Output.WriteLine("  <center><a href=\"\" onclick=\"return email_form_close();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/send_button_g.gif\" value=\"Submit\" alt=\"Submit\"></center><br />");
				Output.WriteLine("</div>");
				Output.WriteLine();
			}

			#endregion

			if (currentMode.Mode != Display_Mode_Enum.My_Sobek)
			{

				#region Save search/browse form

				if (currentUser != null)
				{
					Output.WriteLine("<!-- Save search/browse -->");
					Output.WriteLine("<div class=\"add_popup_div\" id=\"add_item_form\" style=\"display:none;\">");
					Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">A<span class=\"smaller\">DD THIS</span> " + currentTitle + "<span class=\"smaller\"> TO YOUR</span> S<span class=\"smaller\">AVED</span> S<span class=\"smaller\">EARCHES</span></td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"add_item_form_close()\">X</a> &nbsp; </td></tr></table></div>");
					Output.WriteLine("  <br />");
					Output.WriteLine("  <fieldset><legend>Enter notes for this " + currentName + " &nbsp; </legend>");
					Output.WriteLine("    <br />");
					Output.WriteLine("    <table class=\"popup_table\">");

					// Add comments area
					Output.Write("      <tr align=\"left\" valign=\"top\"><td><br /><label for=\"add_notes\">Description:</label></td>");
					Output.WriteLine("<td><textarea rows=\"8\" cols=\"" + actual_cols + "\" name=\"add_notes\" id=\"add_notes\" class=\"add_notes_textarea\" onfocus=\"javascript:textbox_enter('add_notes','add_notes_textarea_focused')\" onblur=\"javascript:textbox_leave('add_notes','add_notes_textarea')\">" + summation + "</textarea></td></tr>");
					Output.WriteLine("      <tr align=\"left\" valign=\"top\"><td>&nbsp;</td><td><input type=\"checkbox\" id=\"open_searches\" name=\"open_searches\" value=\"open\" /> <label for=\"open_searches\">Open saved searches in new window</label></td></tr>");
					Output.WriteLine("    </table>");
					Output.WriteLine("    <br />");
					Output.WriteLine("  </fieldset><br />");
					Output.WriteLine("  <center><a href=\"\" onclick=\"return add_item_form_close();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Submit\" alt=\"Submit\"></center><br />");
					Output.WriteLine("</div>");
					Output.WriteLine();
				}

				#endregion

				#region Share form

				// Calculate the title and url
				string title = HttpUtility.HtmlEncode(summation);
				string share_url = HttpContext.Current.Items["Original_URL"].ToString().Replace("&", "%26").Replace("?", "%3F").Replace("http://", "").Replace("=", "%3D").Replace("\"","&quot;");

				Output.WriteLine("<!-- Share form -->");
				Output.WriteLine("<div class=\"share_popup_div\" id=\"share_form\" style=\"display:none;\">");

				Output.WriteLine("<a href=\"http://www.facebook.com/share.php?u=" + share_url + "&amp;t=" + title + "\" target=\"FACEBOOK_WINDOW\" onmouseover=\"facebook_share.src='" + currentMode.Base_URL + "default/images/facebook_share_h.gif'\" onmouseout=\"facebook_share.src='" + currentMode.Base_URL + "default/images/facebook_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"facebook_share\" name=\"facebook_share\" src=\"" + currentMode.Base_URL + "default/images/facebook_share.gif\" alt=\"FACEBOOK\" /></a>");
				Output.WriteLine("<a href=\"http://buzz.yahoo.com/buzz?targetUrl=" + share_url + "&amp;headline=" + title + "\" target=\"YAHOOBUZZ_WINDOW\" onmouseover=\"yahoobuzz_share.src='" + currentMode.Base_URL + "default/images/yahoobuzz_share_h.gif'\" onmouseout=\"yahoobuzz_share.src='" + currentMode.Base_URL + "default/images/yahoobuzz_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"yahoobuzz_share\" name=\"yahoobuzz_share\" src=\"" + currentMode.Base_URL + "default/images/yahoobuzz_share.gif\" alt=\"YAHOO BUZZ\" /></a>");
				Output.WriteLine("<br />");

				Output.WriteLine("<a href=\"http://twitter.com/home?status=Currently reading " + share_url + "\" target=\"TWITTER_WINDOW\" onmouseover=\"twitter_share.src='" + currentMode.Base_URL + "default/images/twitter_share_h.gif'\" onmouseout=\"twitter_share.src='" + currentMode.Base_URL + "default/images/twitter_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"twitter_share\" name=\"twitter_share\" src=\"" + currentMode.Base_URL + "default/images/twitter_share.gif\" alt=\"TWITTER\" /></a>");
				Output.WriteLine("<a href=\"http://www.google.com/bookmarks/mark?op=add&amp;bkmk=" + share_url + "&amp;title=" + title + "\" target=\"GOOGLE_WINDOW\" onmouseover=\"google_share.src='" + currentMode.Base_URL + "default/images/google_share_h.gif'\" onmouseout=\"google_share.src='" + currentMode.Base_URL + "default/images/google_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"google_share\" name=\"google_share\" src=\"" + currentMode.Base_URL + "default/images/google_share.gif\" alt=\"GOOGLE SHARE\" /></a>");
				Output.WriteLine("<br />");

				Output.WriteLine("<a href=\"http://www.stumbleupon.com/submit?url=" + share_url + "&amp;title=" + title + "\" target=\"STUMBLEUPON_WINDOW\" onmouseover=\"stumbleupon_share.src='" + currentMode.Base_URL + "default/images/stumbleupon_share_h.gif'\" onmouseout=\"stumbleupon_share.src='" + currentMode.Base_URL + "default/images/stumbleupon_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"stumbleupon_share\" name=\"stumbleupon_share\" src=\"" + currentMode.Base_URL + "default/images/stumbleupon_share.gif\" alt=\"STUMBLEUPON\" /></a>");
				Output.WriteLine("<a href=\"http://myweb.yahoo.com/myresults/bookmarklet?t=" + title + "&amp;u=" + share_url + "\" target=\"YAHOO_WINDOW\" onmouseover=\"yahoo_share.src='" + currentMode.Base_URL + "default/images/yahoo_share_h.gif'\" onmouseout=\"yahoo_share.src='" + currentMode.Base_URL + "default/images/yahoo_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"yahoo_share\" name=\"yahoo_share\" src=\"" + currentMode.Base_URL + "default/images/yahoo_share.gif\" alt=\"YAHOO SHARE\" /></a>");
				Output.WriteLine("<br />");

				Output.WriteLine("<a href=\"http://digg.com/submit?phase=2&amp;url=" + share_url + "&amp;title=" + title + "\" target=\"DIGG_WINDOW\" onmouseover=\"digg_share.src='" + currentMode.Base_URL + "default/images/digg_share_h.gif'\" onmouseout=\"digg_share.src='" + currentMode.Base_URL + "default/images/digg_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"digg_share\" name=\"digg_share\" src=\"" + currentMode.Base_URL + "default/images/digg_share.gif\" alt=\"DIGG\" /></a>");
				Output.WriteLine("<a onmouseover=\"favorites_share.src='" + currentMode.Base_URL + "default/images/favorites_share_h.gif'\" onmouseout=\"favorites_share.src='" + currentMode.Base_URL + "default/images/favorites_share.gif'\" onclick=\"javascript:add_to_favorites();\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"favorites_share\" name=\"favorites_share\" src=\"" + currentMode.Base_URL + "default/images/favorites_share.gif\" alt=\"MY FAVORITES\" /></a>");
				Output.WriteLine("<br />");

				Output.WriteLine("</div>");
				Output.WriteLine();

				#endregion

			}

			if (Outer_Form_Name.Length == 0)
				Output.WriteLine("</form>");

			return true;
		}

		/// <summary> Renders the text about this search (i.e., &quot;Your search for ... resulted in ...&quot; )
		/// directly to the output stream </summary>
		/// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
		protected void Show_Search_Info(TextWriter Output)
		{
			string and_language = "and ";
			string or_language = "or ";
			string and_not_language = "not ";
			string no_matches_language = "resulted in no matching records.";
			string one_match_language = "resulted in one matching record.";
			string multiple_records_language = "resulted in {0} matching records.";
			string one_item_language = "resulted in one item in ";
			string multiple_items_language = "resulted in {0} items in ";
			string one_title_language = "one title.";
			string multiple_titles_language = " titles.";

			string between_two_dates = "between {0} and {1} ";
			string on_one_date = "in {0} ";

			// Set special language for aerials
			if (currentMode.Aggregation == "aerials")
			{
				no_matches_language = "resulted in no matching flights.";
				one_match_language = "resulted in one matching flight.";
				multiple_records_language = "resulted in {0} matching flights.";
				one_item_language = "resulted in one flight in ";
				multiple_items_language = "resulted in {0} flights in ";
				one_title_language = "one county.";
				multiple_titles_language = " counties.";
			}
			
			switch (currentMode.Language)
			{
				case Web_Language_Enum.French:
					Output.Write("Votre recherche de <i>" + Current_Aggregation.Name + "</i> en ");
					and_language = "et ";
					or_language = "ou ";
					and_not_language = "non ";

					no_matches_language = "aucun des documents correspondants.";
					one_match_language = ", correpsonde à 1 document.";
					multiple_records_language = ", correpsonde à {0} documents";
					one_item_language = ", correpsonde à 1 document en ";
					multiple_items_language = ", correpsonde à {0} documents en ";
					one_title_language = "1 titre.";
					multiple_titles_language = " titres.";
					break;

				case Web_Language_Enum.Spanish:
					Output.Write("Su búsqueda de <i>" + Current_Aggregation.Name + "</i> en ");
					and_language = "y ";
					or_language = "o ";
					and_not_language = "no ";

					no_matches_language = "no dio lugar a los objetos.";
					one_match_language = ", resultó en 1 objeto.";
					multiple_records_language = ", resultó en {0} objetos.";
					one_item_language = ", resultó en 1 objeto en ";
					multiple_items_language = ", resultó en {0} objetos en ";
					one_title_language = "1 título.";
					multiple_titles_language = " títulos.";
					break;

				default:
					if ((currentMode.Search_Type == Search_Type_Enum.Map)||(currentMode.Search_Type == Search_Type_Enum.Map_Beta))
						Output.Write("Your geographic search of <i>" + Current_Aggregation.Name + "</i> ");
					else
						Output.Write("Your search of <i>" + Current_Aggregation.Name + "</i> for ");
					break;
			}

			// Split the parts
			if ((currentMode.Search_Type != Search_Type_Enum.Map)||(currentMode.Search_Type != Search_Type_Enum.Map_Beta))
			{
				int length_of_explanation = 0;
				List<string> terms = new List<string>();
				List<string> fields = new List<string>();

				// Split the terms correctly
				SobekCM_Assistant.Split_Clean_Search_Terms_Fields(currentMode.Search_String, currentMode.Search_Fields, currentMode.Search_Type, terms, fields, SobekCM_Library_Settings.Search_Stop_Words, currentMode.Search_Precision, ',');

				try
				{
					// Create this differently depending on whether users can remove a search term from their current search
					if (SobekCM_Library_Settings.Can_Remove_Single_Term)
					{
						string current_search_string = currentMode.Search_String;
						string current_search_field = currentMode.Search_Fields;
						Display_Mode_Enum current_display_mode = currentMode.Mode;
						Aggregation_Type_Enum current_aggr_mode = currentMode.Aggregation_Type;
						string current_info_browse_mode = currentMode.Info_Browse_Mode;

						StringBuilder fieldsBuilder = new StringBuilder();
						StringBuilder termsBuilder = new StringBuilder();

						term_counter = 0;
						for (int i = 0; i < Math.Min(terms.Count, fields.Count); i++)
						{
							if ((terms[i].Length > 0) && (fields[i].Length > 0))
							{
								Output.WriteLine();
								Output.Write("        ");

								// Remove the leading + sign
								if (fields[i][0] == '+')
									fields[i] = fields[i].Substring(1);
								if (fields[i][0] == ' ')
									fields[i] = fields[i].Substring(1);

								// Add the 'AND' value
								if (i > 0)
								{
									if (fields[i][0] == '=')
									{
										Output.Write(or_language);
										length_of_explanation += or_language.Length;
										fields[i] = fields[i].Substring(1);
									}
									else
									{
										Output.Write(and_language);
										length_of_explanation += and_language.Length;
									}
								}

								//// This explanataion need to be capped
								//if (length_of_explanation >= 160)
								//{
								//	Output.Write("... ");
								//	break;
								//}

								term_counter++;
								Output.Write("<div id=\"searchterm" + term_counter + "\" class=\"sbkPrsw_SearchTerm\">");

								// Add the term
								if (terms[i].Contains(" "))
								{
									Output.Write("\"" + terms[i].Replace("''''", "'").Replace("''", "'") + "\" ");
									length_of_explanation += terms[i].Length + 1;
								}
								else
								{
									Output.Write("'" + terms[i].Replace("''''", "'").Replace("''", "'") + "' ");
									length_of_explanation += terms[i].Length + 3;
								}

								// Does the field start with a negative?
								if (fields[i][0] == '-')
								{
									Output.Write(and_not_language);
									length_of_explanation += and_not_language.Length;
									fields[i] = fields[i].Substring(1);
								}

								string write_value = Search_Label_from_Sobek_Code(fields[i]).ToLower() + " ";
								Output.Write(write_value);

								// Determine URL of this search without this one term
								if (terms.Count > 1)
								{
									termsBuilder.Clear();
									fieldsBuilder.Clear();

									// Add all fields, EXCEPT the one to be skipped
									for (int j = 0; j < Math.Min(terms.Count, fields.Count); j++)
									{
										if (j != i)
										{
											if (termsBuilder.Length > 0)
												termsBuilder.Append(",");
											termsBuilder.Append(terms[j]);

											if (fieldsBuilder.Length > 0)
												fieldsBuilder.Append(",");
											fieldsBuilder.Append(fields[j]);
										}
									}
									currentMode.Search_String = termsBuilder.ToString();
									currentMode.Search_Fields = fieldsBuilder.ToString();
								}
								else
								{
									if (Current_Aggregation.Views_And_Searches.Contains(Item_Aggregation.CollectionViewsAndSearchesEnum.All_New_Items))
									{
										currentMode.Mode = Display_Mode_Enum.Aggregation;
										currentMode.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;
										currentMode.Info_Browse_Mode = "all";
									}
									else
									{
										currentMode.Mode = Display_Mode_Enum.Aggregation;
										currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
									}
								}


								Output.WriteLine("<a href=\"" + currentMode.Redirect_URL() + "\" title=\"Click to remove this search term\"><img src=\"" + currentMode.Default_Images_URL + "removeIcon.gif\" id=\"removesearchterm" + term_counter + "\" class=\"sbkPrsw_RemoveSearchTerm\" /></a></div>");
								length_of_explanation += write_value.Length;
							}
						}

						currentMode.Search_String = current_search_string;
						currentMode.Search_Fields = current_search_field;
						currentMode.Mode = current_display_mode;
						currentMode.Aggregation_Type = current_aggr_mode;
						currentMode.Info_Browse_Mode = current_info_browse_mode;
					}
					else
					{

						for (int i = 0; (i < terms.Count) && (i < fields.Count); i++)
						{
							if ((terms[i].Length > 0) && (fields[i].Length > 0))
							{
								// Remove the leading + sign
								if (fields[i][0] == '+')
									fields[i] = fields[i].Substring(1);
								if (fields[i][0] == ' ')
									fields[i] = fields[i].Substring(1);

								// Add the 'AND' value
								if (i > 0)
								{
									if (fields[i][0] == '=')
									{
										Output.Write(or_language);
										length_of_explanation += or_language.Length;
										fields[i] = fields[i].Substring(1);
									}
									else
									{
										Output.Write(and_language);
										length_of_explanation += and_language.Length;
									}
								}

								//// This explanataion need to be capped
								//if (length_of_explanation >= 160)
								//{
								//	Output.Write("... ");
								//	break;
								//}

								// Add the term
								if (terms[i].Contains(" "))
								{
									Output.Write("\"" + terms[i].Replace("''''", "'").Replace("''", "'") + "\" ");
									length_of_explanation += terms[i].Length + 1;
								}
								else
								{
									Output.Write("'" + terms[i].Replace("''''", "'").Replace("''", "'") + "' ");
									length_of_explanation += terms[i].Length + 3;
								}

								// Does the field start with a negative?
								if (fields[i][0] == '-')
								{
									Output.Write(and_not_language);
									length_of_explanation += and_not_language.Length;
									fields[i] = fields[i].Substring(1);
								}

								string write_value = Search_Label_from_Sobek_Code(fields[i]).ToLower() + " ";
								Output.Write(write_value);



								length_of_explanation += write_value.Length;
							}
						}

					}
				}
				catch
				{
					Output.Write("UNRECOGNIZED SEARCH ");
				}
			}

			// Add the year date range text here as well
			if (currentMode.DateRange_Year1 >= 0)
			{
				if (currentMode.DateRange_Year2 >= 0)
				{
					Output.Write(between_two_dates, currentMode.DateRange_Year1, currentMode.DateRange_Year2 );
				}
				else
				{
					Output.Write(on_one_date, currentMode.DateRange_Year1);
				}
			}

			if ((resultsStatistics == null) || (resultsStatistics.Total_Titles == 0))
			{
				Output.WriteLine(no_matches_language );
			}
			else
			{
				if (resultsStatistics.Total_Titles == resultsStatistics.Total_Items)
				{
					Output.WriteLine(resultsStatistics.Total_Titles == 1 ? one_match_language : String.Format(multiple_records_language, resultsStatistics.Total_Titles));
				}
				else
				{
					Output.Write(resultsStatistics.Total_Items == 1 ? one_item_language : String.Format(multiple_items_language, resultsStatistics.Total_Items.ToString()));

					if (resultsStatistics.Total_Titles == 1)
					{
						Output.WriteLine(one_title_language);
					}
					else
					{
						Output.WriteLine(resultsStatistics.Total_Titles + multiple_titles_language);
					}
				}
			}
		}

		private string Search_Label_from_Sobek_Code(string Code)
		{
			string in_language = "in ";
			if (currentMode.Language == Web_Language_Enum.French)
			{
				in_language = "en ";
			}
			if (currentMode.Language == Web_Language_Enum.Spanish)
			{
				in_language = "en ";
			}

			if (Code == "ZZ")
				return translations.Get_Translation("anywhere", currentMode.Language);

			Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_Code(Code);
			return (field != null) ? in_language + translations.Get_Translation(field.Display_Term, currentMode.Language) : in_language + Code;
		}

		#region Methods to create the facets on the left side of the results

		/// <summary> Returns the facets for this result/browse as HTML to be added into the form </summary>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> Formatted facet information in HTML table format </returns>
		protected string Add_Facet_Information(Custom_Tracer Tracer)
		{

			string collection = translations.Get_Translation("Collection", currentMode.Language );
			string show_more = translations.Get_Translation("Show More", currentMode.Language); 
			string show_less = translations.Get_Translation("Show Less", currentMode.Language); 
			string sort_by_frequency = translations.Get_Translation("Sort these facets by frequency", currentMode.Language);
			string sort_alphabetically = translations.Get_Translation("Sort these facets alphabetically", currentMode.Language);

			StringBuilder builder = new StringBuilder();

			builder.AppendLine("<input type=\"hidden\" id=\"facet\" name=\"facet\" value=\"" + HttpUtility.HtmlEncode(facetInformation) + "\" />");

			builder.AppendLine("<script type=\"text/javascript\">");
			builder.AppendLine("  //<![CDATA[");
			builder.AppendLine("    function add_facet(code, new_value) {");

			string url = String.Empty;
			string aggregation_url = String.Empty;

			if (currentMode.Mode == Display_Mode_Enum.Aggregation)  // browse info only
			{
				Display_Mode_Enum displayMode = currentMode.Mode;
				currentMode.Mode = Display_Mode_Enum.Results;
				currentMode.Search_Type = Search_Type_Enum.Advanced;
				currentMode.Search_Fields = "<%CODE%>";
				currentMode.Search_String = "<%VALUE%>";
				ushort page = currentMode.Page;
				currentMode.Page = 1;
				url = currentMode.Redirect_URL().Replace("%3c%25", "<%").Replace("%25%3e", "%>").Replace("<%VALUE%>","\"<%VALUE%>\"");
				currentMode.Mode = displayMode;
				currentMode.Page = page;
				currentMode.Search_Fields = String.Empty;
				currentMode.Search_String = String.Empty;

				if ((currentMode.Aggregation.Length == 0) || (currentMode.Aggregation == "all"))
				{
					currentMode.Aggregation = "<%AGGREGATION%>";
					aggregation_url = currentMode.Redirect_URL();
					currentMode.Aggregation = String.Empty;
				}
			}
			else
			{
				if ((currentMode.Aggregation.Length == 0) || ( currentMode.Aggregation == "all" ))
				{
					currentMode.Aggregation = "<%AGGREGATION%>";
					aggregation_url = currentMode.Redirect_URL();
					currentMode.Aggregation = String.Empty;
				}

				if (currentMode.Search_Type == Search_Type_Enum.Advanced)
				{
					string orig_field = currentMode.Search_Fields;
					string orig_terms = currentMode.Search_String;
					currentMode.Search_Fields = currentMode.Search_Fields + ",<%CODE%>";
					currentMode.Search_String = currentMode.Search_String + ",<%VALUE%>";
					ushort page = currentMode.Page;
					currentMode.Page = 1;
					url = currentMode.Redirect_URL().Replace("%3c%25", "<%").Replace("%25%3e", "%>").Replace("<%VALUE%>", "\"<%VALUE%>\"");
					currentMode.Page = page;
					currentMode.Search_Fields = orig_field;
					currentMode.Search_String = orig_terms;
				}
				if ( currentMode.Search_Type == Search_Type_Enum.Basic )
				{
					List<string> output_terms = new List<string>();
					List<string> output_fields = new List<string>();
					SobekCM_Assistant.Split_Clean_Search_Terms_Fields(currentMode.Search_String, currentMode.Search_Fields, currentMode.Search_Type, output_terms, output_fields, SobekCM_Library_Settings.Search_Stop_Words, currentMode.Search_Precision, ',');

					string original_search = currentMode.Search_String;
					currentMode.Search_Type = Search_Type_Enum.Advanced;
					StringBuilder term_builder = new StringBuilder();
					foreach (string thisTerm in output_terms)
					{
						if (term_builder.Length > 0)
							term_builder.Append(",");
						term_builder.Append(thisTerm);
					}
					StringBuilder field_builder = new StringBuilder();
					foreach (string thisField in output_fields)
					{
						if (field_builder.Length > 0)
							field_builder.Append(",");
						field_builder.Append(thisField);
					}
					currentMode.Search_Fields = field_builder.ToString();
					currentMode.Search_String = term_builder.ToString();

					currentMode.Search_Fields = currentMode.Search_Fields + ",<%CODE%>";
					currentMode.Search_String = currentMode.Search_String + ",<%VALUE%>";
					url = currentMode.Redirect_URL().Replace("%3c%25", "<%").Replace("%25%3e", "%>").Replace("<%VALUE%>", "\"<%VALUE%>\""); 

					currentMode.Search_Type = Search_Type_Enum.Basic;
					currentMode.Search_Fields = String.Empty;
					currentMode.Search_String = original_search;

				}
			}
			builder.AppendLine("      var stem_url = '" + url + "';");
			builder.AppendLine("      var new_url = stem_url.replace('<%CODE%>', code).replace('<%VALUE%>', new_value);");
			builder.AppendLine("      window.location.href = new_url;");
			builder.AppendLine("      return false;");
			builder.AppendLine("    }");
			builder.AppendLine("  //]]>");
			builder.AppendLine("</script>");
			builder.AppendLine();

			builder.AppendLine("<div class=\"sbkPrsw_FacetColumn\">");
			builder.AppendLine("<div class=\"sbkPrsw_FacetColumnTitle\">" + translations.Get_Translation("NARROW RESULTS BY", currentMode.Language) + ":</div>");


			// Add the aggregation information first
			if ((( currentMode.Aggregation.Length == 0 ) || ( currentMode.Aggregation == "all")) && (resultsStatistics.Aggregation_Facets_Count > 0))
			{
				string title = collection;
				const int FACET_INDEX = 0;
				int facet_count = 0;
				int total_facets_to_show = MINIMIZED_FACET_COUNT;
				char other_sort_type = '2';
				char other_show_type = '1';
				if ((facetInformation[FACET_INDEX] == '1') || (facetInformation[FACET_INDEX] == '3'))
				{
					total_facets_to_show = MAXIMIZED_FACET_COUNT;
				}

				string resort_image = "2_to_1.gif";
				string sort_instructions = sort_by_frequency;
				switch (facetInformation[FACET_INDEX])
				{
					case '0':
						other_sort_type = '2';
						other_show_type = '1';
						sort_instructions = sort_alphabetically;
						break;

					case '1':
						other_sort_type = '3';
						other_show_type = '0';
						sort_instructions = sort_alphabetically;
						break;

					case '2':
						other_sort_type = '0';
						other_show_type = '3';
						resort_image = "a_to_z.gif";
						break;

					case '3':
						other_sort_type = '1';
						other_show_type = '2';
						resort_image = "a_to_z.gif";
						break;
				}

				builder.AppendLine("<div class=\"sbkPrsw_FacetBoxTitle\">" + title + "</div>");
				builder.AppendLine("<div class=\"sbkPrsw_FacetBox\">");
				if (resultsStatistics.Aggregation_Facets.Count > 1 )
					builder.AppendLine("<div class=\"sbkPrsw_FacetReorder\"><a href=\"\" onclick=\"return set_facet(" + FACET_INDEX + ",'" + other_sort_type + "');\" title=\"" + sort_instructions + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/" + resort_image + "\" alt=\"RESORT\" /></a></div>");
				if ((facetInformation[FACET_INDEX] == '2') || (facetInformation[FACET_INDEX] == '3'))
				{
					SortedList<string, string> order_facets = new SortedList<string, string>();
					while ((facet_count < total_facets_to_show) && (facet_count < resultsStatistics.Aggregation_Facets.Count))
					{
						if (resultsStatistics.Aggregation_Facets[facet_count].Code.ToLower() != "iuf")
						{
							order_facets[resultsStatistics.Aggregation_Facets[facet_count].Facet.ToUpper()] = "<a href=\"" + aggregation_url.Replace("<%AGGREGATION%>", resultsStatistics.Aggregation_Facets[facet_count].Code.ToLower()) + "\">" + resultsStatistics.Aggregation_Facets[facet_count].Facet + "</a> ( " + resultsStatistics.Aggregation_Facets[facet_count].Frequency + " ) <br />";
						}
						facet_count++;
					}
					foreach (string html in order_facets.Values)
					{
						builder.AppendLine(html);
					}
				}
				else
				{
					while ((facet_count < total_facets_to_show) && (facet_count < resultsStatistics.Aggregation_Facets.Count))
					{
						if (resultsStatistics.Aggregation_Facets[facet_count].Code.ToLower() != "iuf")
						{
							builder.AppendLine("<a href=\"" + aggregation_url.Replace("<%AGGREGATION%>", resultsStatistics.Aggregation_Facets[facet_count].Code.ToLower()) + "\">" + resultsStatistics.Aggregation_Facets[facet_count].Facet + "</a> ( " + resultsStatistics.Aggregation_Facets[facet_count].Frequency + " ) <br />");
						}
						facet_count++;
					}
				}
				if (facet_count > MINIMIZED_FACET_COUNT)
				{
					builder.AppendLine("<div class=\"sbkPrsw_ShowHideFacets\"><a href=\"\" onclick=\"return set_facet(" + FACET_INDEX + ",'" + other_show_type + "');\">&lt;&lt; " + show_less + " &nbsp; &nbsp;</a></div>");
				}
				else
				{
					if (facet_count < resultsStatistics.Aggregation_Facets.Count)
					{
						builder.AppendLine("<div class=\"sbkPrsw_ShowHideFacets\"><a href=\"\" onclick=\"return set_facet(" + FACET_INDEX + ",'" + other_show_type + "');\">" + show_more + " &gt;&gt; &nbsp;</a></div>");
					}
				}
				builder.AppendLine("</div>");
			}

			// Add the first facet information 
			if (resultsStatistics.First_Facets_Count > 0)
			{
				Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.First_Facets_MetadataTypeID);
				if (field != null)
				{
					Add_Single_Facet(builder,  translations.Get_Translation(field.Facet_Term, currentMode.Language ), field.Web_Code, show_less, show_more, 1, sort_by_frequency, sort_alphabetically, resultsStatistics.First_Facets);
				}
			}

			// Add the second facet information 
			if (resultsStatistics.Second_Facets_Count > 0)
			{
				Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.Second_Facets_MetadataTypeID);
				if (field != null)
				{
					Add_Single_Facet(builder, translations.Get_Translation(field.Facet_Term, currentMode.Language), field.Web_Code, show_less, show_more, 2, sort_by_frequency, sort_alphabetically, resultsStatistics.Second_Facets);
				}
			}

			// Add the third facet information 
			if (resultsStatistics.Third_Facets_Count > 0)
			{
				Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.Third_Facets_MetadataTypeID);
				if (field != null)
				{
					Add_Single_Facet(builder, translations.Get_Translation(field.Facet_Term, currentMode.Language), field.Web_Code, show_less, show_more, 3, sort_by_frequency, sort_alphabetically, resultsStatistics.Third_Facets);
				}
			}

			// Add the fourth facet information 
			if (resultsStatistics.Fourth_Facets_Count > 0)
			{
				Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.Fourth_Facets_MetadataTypeID);
				if (field != null)
				{
					Add_Single_Facet(builder, translations.Get_Translation(field.Facet_Term, currentMode.Language), field.Web_Code, show_less, show_more, 4, sort_by_frequency, sort_alphabetically, resultsStatistics.Fourth_Facets);
				}
			}

			// Add the fifth facet information 
			if (resultsStatistics.Fifth_Facets_Count > 0)
			{
				Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.Fifth_Facets_MetadataTypeID);
				if (field != null)
				{
					Add_Single_Facet(builder, translations.Get_Translation(field.Facet_Term, currentMode.Language), field.Web_Code, show_less, show_more, 5, sort_by_frequency, sort_alphabetically, resultsStatistics.Fifth_Facets);
				}
			}

			// Add the sixth facet information 
			if (resultsStatistics.Sixth_Facets_Count > 0)
			{
				Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.Sixth_Facets_MetadataTypeID);
				if (field != null)
				{
					Add_Single_Facet(builder, translations.Get_Translation(field.Facet_Term, currentMode.Language), field.Web_Code, show_less, show_more, 6, sort_by_frequency, sort_alphabetically, resultsStatistics.Sixth_Facets);
				}
			}

			// Add the seventh facet information 
			if (resultsStatistics.Seventh_Facets_Count > 0)
			{
				Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.Seventh_Facets_MetadataTypeID);
				if (field != null)
				{
					Add_Single_Facet(builder, translations.Get_Translation(field.Facet_Term, currentMode.Language), field.Web_Code, show_less, show_more, 7, sort_by_frequency, sort_alphabetically, resultsStatistics.Seventh_Facets);
				}
			}

			// Add the eighth facet information 
			if (resultsStatistics.Eighth_Facets_Count > 0)
			{
				Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.Eighth_Facets_MetadataTypeID);
				if (field != null)
				{
					Add_Single_Facet(builder, translations.Get_Translation(field.Facet_Term, currentMode.Language), field.Web_Code, show_less, show_more, 8, sort_by_frequency, sort_alphabetically, resultsStatistics.Eighth_Facets);
				}
			}

			builder.AppendLine("</div>");
			return builder.ToString();
		}


		private void Add_Single_Facet(StringBuilder Builder, string Title, string SearchCode, string ShowLess, string ShowMore, int FacetIndex, string SortByFrequency, string SortAlphabetically, List<Search_Facet> Collection)
		{
			int facet_count = 0;
			int total_facets_to_show = MINIMIZED_FACET_COUNT;
			char other_sort_type = '2';
			char other_show_type = '1';
			if ((facetInformation[FacetIndex - 1] == '1') || ( facetInformation[FacetIndex - 1 ] == '3' ))
			{
				total_facets_to_show = MAXIMIZED_FACET_COUNT;
			}

			string resort_image = "2_to_1.gif";
			string sort_instructions = SortByFrequency;
			switch ( facetInformation[FacetIndex - 1])
			{
				case '0':
					other_sort_type = '2';
					other_show_type = '1';
					sort_instructions = SortAlphabetically;
					break;

				case '1':
					other_sort_type = '3';
					other_show_type = '0';
					sort_instructions = SortAlphabetically;
					break;

				case '2':
					other_sort_type = '0';
					other_show_type = '3';
					resort_image = "a_to_z.gif";
					break;

				case '3':
					other_sort_type = '1';
					other_show_type = '2';
					resort_image = "a_to_z.gif";
					break;
			}

			Builder.AppendLine("<div class=\"sbkPrsw_FacetBoxTitle\">" + Title + "</div>");
			Builder.AppendLine("<div class=\"sbkPrsw_FacetBox\">");
			if (Collection.Count > 1)
			{
				Builder.AppendLine("<div class=\"sbkPrsw_FacetReorder\"><a href=\"\" onclick=\"return set_facet(" + (FacetIndex - 1) + ",'" + other_sort_type + "');\" title=\"" + sort_instructions + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/" + resort_image + "\" alt=\"RESORT\" /></a></div>");
			}
			if ((facetInformation[FacetIndex - 1] == '2') || (facetInformation[FacetIndex - 1] == '3'))
			{
				SortedList<string, string> order_facets = new SortedList<string, string>();
				while ((facet_count < total_facets_to_show) && (facet_count < Collection.Count))
				{
					order_facets[Collection[facet_count].Facet.ToUpper()] = "<a href=\"\" onclick=\"return add_facet('" + SearchCode + "','" + HttpUtility.HtmlEncode(Collection[facet_count].Facet.Replace("&", "")).Replace("'", "\\'").Replace(",", "").Replace("&", "") + "');\">" + Collection[facet_count].Facet.Replace("&", "&amp;") + "</a> ( " + Collection[facet_count].Frequency + " ) <br />";
					facet_count++;
				}
				foreach (string html in order_facets.Values)
				{
					Builder.AppendLine(html);
				}
			}
			else
			{
				while ((facet_count < total_facets_to_show) && (facet_count < Collection.Count))
				{
					Builder.AppendLine("<a href=\"\" onclick=\"return add_facet('" + SearchCode + "','" + HttpUtility.HtmlEncode(Collection[facet_count].Facet.Replace("&", "")).Replace("'", "\\'").Replace(",", "").Replace("&", "") + "');\">" + Collection[facet_count].Facet.Replace("&", "&amp;" ) + "</a> ( " + Collection[facet_count].Frequency + " ) <br />");
					facet_count++;
				}
			}
			if (facet_count > MINIMIZED_FACET_COUNT)
			{
				Builder.AppendLine("<div class=\"sbkPrsw_ShowHideFacets\"><a href=\"\" onclick=\"return set_facet(" + (FacetIndex - 1) + ",'" + other_show_type + "');\">&lt;&lt; " + ShowLess + " &nbsp; &nbsp;</a></div>");
			}
			else
			{
				if (facet_count < Collection.Count)
				{
					Builder.AppendLine("<div class=\"sbkPrsw_ShowHideFacets\"><a href=\"\" onclick=\"return set_facet(" + (FacetIndex - 1) + ",'" + other_show_type + "');\">" + ShowMore + " &gt;&gt; &nbsp;</a></div>");
				}
			}
			Builder.AppendLine("</div>");
		}
		#endregion
	}
}
