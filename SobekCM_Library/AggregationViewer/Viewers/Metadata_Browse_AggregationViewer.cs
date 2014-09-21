#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Navigation;
using SobekCM.Library.Search;
using SobekCM.Library.Settings;
using SobekCM.Library.WebContent;
using SobekCM.Tools;
using SobekCM_UI_Library.Navigation;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Renders the screen to allow users to browse the metadata values present in different fields for an item aggregation </summary>
    /// <remarks> This class implements the <see cref="iAggregationViewer"/> interface and extends the <see cref="abstractAggregationViewer"/> class.<br /><br />
    /// Collection viewers are used when displaying collection home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request to display a static browse or info page, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in this case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
    /// <li>To display the requested collection view, the collection subwriter will creates an instance of this class </li>
    /// </ul></remarks>
    public class Metadata_Browse_AggregationViewer : abstractAggregationViewer
    {
        private readonly Item_Aggregation_Child_Page browseObject;
        private readonly List<string> results;

        /// <summary> Constructor for a new instance of the Metadata_Browse_AggregationViewer class </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Current_Aggregation"> Current item aggregation object to display </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Metadata_Browse_AggregationViewer(SobekCM_Navigation_Object Current_Mode, Item_Aggregation Current_Aggregation, Custom_Tracer Tracer): base(Current_Aggregation, Current_Mode)
        {
            string defaultBrowseBy = Current_Aggregation.Default_BrowseBy;

            // If there is not info browse mode listed, use the default
            if (Current_Mode.Info_Browse_Mode.Length == 0)
                Current_Mode.Info_Browse_Mode = defaultBrowseBy;
            if ((Current_Mode.Info_Browse_Mode.Length == 0) && (Current_Aggregation.Has_Browse_By_Pages))
                Current_Mode.Info_Browse_Mode = Current_Aggregation.Browse_By_Pages(Current_Mode.Language)[0].Code;

            // Get this browse
            browseObject = Current_Aggregation.Get_Browse_Info_Object(Current_Mode.Info_Browse_Mode);

            // Was this a metadata browseby, or just a static html?
            if (( browseObject == null ) || ( browseObject.Source != Item_Aggregation_Child_Page.Source_Type.Static_HTML))
            {
                // Determine the correct metadata code
                string metadata_code = Current_Mode.Info_Browse_Mode.Trim().Replace("_", " ");
                Current_Mode.Info_Browse_Mode = metadata_code;

                // Only get values if there was a metadata code
                if (metadata_code.Length > 0)
                {
                    // Check the cache for this value
                    List<string> cacheInstance = Cached_Data_Manager.Retrieve_Aggregation_Metadata_Browse(Current_Mode.Aggregation, Current_Mode.Info_Browse_Mode, Tracer);

                    if (cacheInstance != null)
                    {
                        results = cacheInstance;
                    }
                    else
                    {
                        results = SobekCM_Database.Get_Item_Aggregation_Metadata_Browse(Current_Mode.Aggregation, Current_Mode.Info_Browse_Mode, Tracer);
                        Cached_Data_Manager.Store_Aggregation_Metadata_Browse(Current_Mode.Aggregation, Current_Mode.Info_Browse_Mode, results, Tracer);
                    }
                }
            }
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation.CollectionViewsAndSearchesEnum.Metadata_Browse"/> enumerational value </value>
        public override Item_Aggregation.CollectionViewsAndSearchesEnum Type
        {
            get { return Item_Aggregation.CollectionViewsAndSearchesEnum.Metadata_Browse; }
        }

        /// <summary> Gets flag which indicates whether to always use the home text as the secondary text </summary>
        public override bool Always_Display_Home_Text
        {
            get
            {
                return false;
            }
        }

        /// <summary>Flag indicates whether the subaggregation selection panel is displayed for this collection viewer</summary>
        /// <value> This property always returns the <see cref="Selection_Panel_Display_Enum.Never"/> enumerational value </value>
        public override Selection_Panel_Display_Enum Selection_Panel_Display
        {
            get
            {
                return Selection_Panel_Display_Enum.Never;
            }
        }

        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds the title of the static browse or info into the box </remarks>
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Metadata_Browse_AggregationViewer.Add_Search_Box_HTML", "Adding HTML");
            }

            if (( browseObject != null ) && ( browseObject.Source == Item_Aggregation_Child_Page.Source_Type.Static_HTML))
            {
                Output.WriteLine("  <h1>" + browseObject.Get_Label(currentMode.Language) + "</h1>");

            }
            else
            {
                if ((results != null) && (currentMode.Info_Browse_Mode.Length > 0))
                {
                    Output.WriteLine("  <h1>Browse by " + translator.Get_Translation( currentMode.Info_Browse_Mode, currentMode.Language) + "</h1>");
                }
                else
                {
                    Output.WriteLine("  <h1>" + translator.Get_Translation("Browse by...", currentMode.Language) + "</h1>");
                }
            }
        }

        /// <summary> Add the HTML to be displayed below the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This writes the HTML from the static browse or info page here  </remarks>
        public override void Add_Secondary_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Metadata_Browse_AggregationViewer.Add_Secondary_HTML", "Adding HTML");
            }

            // Get collection of (public) browse bys linked to this aggregation
            ReadOnlyCollection<Item_Aggregation_Child_Page> public_browses = currentCollection.Browse_By_Pages(currentMode.Language);

            // Determine if this is an internal user and create list of internal user browses
            List<string> internal_browses = new List<string>();
            if ((currentUser != null) && ((currentUser.Is_Internal_User) || (currentUser.Is_Aggregation_Curator(currentMode.Aggregation))))
            {
                // Just add every metadata field here
                foreach (Metadata_Search_Field field in SobekCM_Library_Settings.All_Metadata_Fields)
                {
                    if (( field.Web_Code.Length > 0 ) && ( currentCollection.Browseable_Fields.Contains(field.ID)))
                        internal_browses.Add(field.Display_Term);
                }
            }

            // Retain the original short code (or the first public code)
            string original_browse_mode = currentMode.Info_Browse_Mode.ToLower();

            // Get any paging URL and retain original page
            int current_page = currentMode.Page;
            currentMode.Page = 1;
            string page_url = currentMode.Redirect_URL(false);
            string url_options = currentMode.URL_Options();
            if (url_options.Length > 0)
                url_options = "?" + url_options.Replace("&", "&amp");

            if ((public_browses.Count > 1) || (internal_browses.Count > 0 ))
            {
                Output.WriteLine("<table>");
                Output.WriteLine("<tr style=\"vertical-align:top;\">");
				Output.WriteLine("<td id=\"sbkMebv_FacetOuterColumn\">");
				Output.WriteLine("<div class=\"sbkMebv_FacetColumn\">");
				Output.WriteLine("<div class=\"sbkMebv_FacetColumnTitle\">BROWSE BY:</div>");
                Output.WriteLine("<br />");

                if (public_browses.Count > 0)
                {
                    // Sort these by title
                    SortedList<string, Item_Aggregation_Child_Page> sortedBrowses = new SortedList<string, Item_Aggregation_Child_Page>();
                    foreach (Item_Aggregation_Child_Page thisBrowse in public_browses)
                    {
                        if (thisBrowse.Source == Item_Aggregation_Child_Page.Source_Type.Static_HTML)
                        {
                            sortedBrowses[thisBrowse.Code.ToLower()] = thisBrowse;
                        }
                        else
                        {
                            Metadata_Search_Field facetField = SobekCM_Library_Settings.Metadata_Search_Field_By_Name(thisBrowse.Code);
                            if (facetField != null)
                            {
								string facetName = facetField.Display_Term;

                                if (internal_browses.Contains(facetName))
                                    internal_browses.Remove(facetName);

                                sortedBrowses[facetName.ToLower()] = thisBrowse;
                            }
                        }
                    }

	                Output.WriteLine(internal_browses.Count > 0 ? "<b> &nbsp;Public Browses</b><br />" : "<b> &nbsp;Browses</b><br />");

	                Output.WriteLine("<div class=\"sbkMebv_FacetBox\">");
                    foreach (Item_Aggregation_Child_Page thisBrowse in sortedBrowses.Values)
                    {
                        // Static HTML or metadata browse by?
                        if (thisBrowse.Source == Item_Aggregation_Child_Page.Source_Type.Static_HTML)
                        {
                            if (original_browse_mode != thisBrowse.Code)
                            {
                                currentMode.Info_Browse_Mode = thisBrowse.Code;
                                Output.WriteLine("<a href=\"" + currentMode.Redirect_URL().Replace("&", "&amp") + "\">" + thisBrowse.Get_Label( currentMode.Language ) + "</a><br />");
                            }
                            else
                            {
                                Output.WriteLine(thisBrowse.Get_Label(currentMode.Language) + "<br />");
                            }
                        }
                        else
                        {
                            Metadata_Search_Field facetField = SobekCM_Library_Settings.Metadata_Search_Field_By_Display_Name(thisBrowse.Code);
							if (thisBrowse.Code.ToLower().Replace("_", " ") != original_browse_mode.Replace("_", " "))
                            {
                                currentMode.Info_Browse_Mode = thisBrowse.Code.ToLower().Replace(" ", "_");
								Output.WriteLine("<a href=\"" + currentMode.Redirect_URL().Replace("&", "&amp") + "\">" + facetField.Display_Term + "</a><br />");
                            }
                            else
                            {
								Output.WriteLine(facetField.Display_Term + "<br />");
                            }
                        }
                    }

                    Output.WriteLine("</div>");
                    Output.WriteLine("<br />");
                }

                if (internal_browses.Count > 0)
                {
                    Output.WriteLine("<b> &nbsp;Internal Browses</b><br />");
                    Output.WriteLine("<div class=\"sbkMebv_FacetBox\">");

                    foreach (string thisShort in internal_browses)
                    {
                        Metadata_Search_Field facetField = SobekCM_Library_Settings.Metadata_Search_Field_By_Facet_Name(thisShort);
	                    if (facetField != null)
	                    {
		                    if (thisShort.ToLower() != original_browse_mode)
		                    {
			                    currentMode.Info_Browse_Mode = thisShort.ToLower().Replace(" ", "_");
								Output.WriteLine("<a href=\"" + currentMode.Redirect_URL().Replace("&", "&amp") + "\">" + facetField.Display_Term + "</a><br />");
		                    }
		                    else
		                    {
								Output.WriteLine(facetField.Display_Term + "<br />");
		                    }
	                    }
                    }

                    Output.WriteLine("</div>");
                    Output.WriteLine("<br />");
                }
                Output.WriteLine("<br />");
                Output.WriteLine("<br />");
                Output.WriteLine("<br />");
                Output.WriteLine("<br />");
                Output.WriteLine("<br />");
                Output.WriteLine("</div>");
                Output.WriteLine("</td>");
                Output.WriteLine("<td>");
            }
			Output.WriteLine("<div class=\"sbkMebv_ResultsPanel\">");

            currentMode.Info_Browse_Mode = original_browse_mode;

            // Was this static or metadata browse by?
            if (( browseObject != null ) && ( browseObject.Source == Item_Aggregation_Child_Page.Source_Type.Static_HTML))
            {
                // Read the content file for this browse
                HTML_Based_Content staticBrowseContent = browseObject.Get_Static_Content(currentMode.Language, currentMode.Base_URL, SobekCM_Library_Settings.Base_Design_Location + currentCollection.ObjDirectory, Tracer);
           
                // Apply current user settings for this
                string browseInfoDisplayText = staticBrowseContent.Apply_Settings_To_Static_Text(staticBrowseContent.Static_Text, currentCollection, htmlSkin.Skin_Code, htmlSkin.Base_Skin_Code, currentMode.Base_URL, currentMode.URL_Options(), Tracer);

                // Add this to the output stream
                Output.WriteLine(browseInfoDisplayText);
            }
            else
            {

                //Output the results
                if (( results != null ) && (results.Count > 0))
                {
                    // Determine which letters appear
                    List<char> letters_appearing = new List<char>();
                    char last_char = '\n';
                    if (results.Count > 100)
                    {
                        foreach (string thisValue in results)
                        {
                            if (thisValue.Length > 0)
                            {
                                char this_first_char = Char.ToLower(thisValue[0]);
                                int ascii = this_first_char;

                                if (ascii < 97)
                                    this_first_char = 'a';
                                if (ascii > 122)
                                    this_first_char = 'z';

                                if (this_first_char != last_char)
                                {
                                    if (!letters_appearing.Contains(this_first_char))
                                    {
                                        letters_appearing.Add(this_first_char);
                                    }
                                    last_char = this_first_char;
                                }
                            }
                        }
                    }

                    // Get the search URL
                    currentMode.Mode = Display_Mode_Enum.Results;
                    currentMode.Search_Precision = Search_Precision_Type_Enum.Exact_Match;
                    currentMode.Search_Type = Search_Type_Enum.Advanced;
                    Metadata_Search_Field facetField = SobekCM_Library_Settings.Metadata_Search_Field_By_Display_Name(original_browse_mode);
                    currentMode.Search_Fields = facetField.Web_Code;
                    currentMode.Search_String = "\"<%TERM%>\"";
                    string search_url = currentMode.Redirect_URL();

                    Output.WriteLine("<br />");

                    if (results.Count < 100)
                    {
                        foreach (string thisResult in results)
                        {
                            Output.WriteLine("<a href=\"" + search_url.Replace("%3c%25TERM%25%3e", thisResult.Trim().Replace(",", "%2C").Replace("&", "%26").Replace("\"", "%22").Replace("&", "&amp")) + "\">" + thisResult.Replace("\"", "&quot;").Replace("&", "&amp;") + "</a><br />");
                        }
                    }
                    else if (results.Count < 500)
                    {
                        // Determine the actual page first
                        int first_valid_page = -1;
                        if ((letters_appearing.Contains('a')) || (letters_appearing.Contains('b')))
                        {
                            first_valid_page = 1;
                        }

                        if ((letters_appearing.Contains('c')) || (letters_appearing.Contains('d')) || (letters_appearing.Contains('e')))
                        {
                            if (first_valid_page < 0)
                                first_valid_page = 2;
                        }

                        if ((letters_appearing.Contains('f')) || (letters_appearing.Contains('g')) || (letters_appearing.Contains('h')))
                        {
                            if (first_valid_page < 0)
                                first_valid_page = 3;
                        }

                        if ((letters_appearing.Contains('i')) || (letters_appearing.Contains('j')) || (letters_appearing.Contains('k')))
                        {
                            if (first_valid_page < 0)
                                first_valid_page = 4;
                        }

                        if ((letters_appearing.Contains('l')) || (letters_appearing.Contains('m')) || (letters_appearing.Contains('n')))
                        {
                            if (first_valid_page < 0)
                                first_valid_page = 5;
                        }

                        if ((letters_appearing.Contains('o')) || (letters_appearing.Contains('p')) || (letters_appearing.Contains('q')))
                        {
                            if (first_valid_page < 0)
                                first_valid_page = 6;
                        }

                        if ((letters_appearing.Contains('r')) || (letters_appearing.Contains('s')) || (letters_appearing.Contains('t')))
                        {
                            if (first_valid_page < 0)
                                first_valid_page = 7;
                        }

                        if ((letters_appearing.Contains('u')) || (letters_appearing.Contains('v')) || (letters_appearing.Contains('w')))
                        {
                            if (first_valid_page < 0)
                                first_valid_page = 8;
                        }

                        if ((letters_appearing.Contains('x')) || (letters_appearing.Contains('y')) || (letters_appearing.Contains('z')))
                        {
                            if (first_valid_page < 0)
                                first_valid_page = 9;
                        }

                        // Define the limits of the page value
                        if ((current_page < first_valid_page) || (current_page > 9))
                            current_page = first_valid_page;


                        // Add the links for paging through results
						Output.WriteLine("<div class=\"sbkMebv_NavRow\">");
                        if ((letters_appearing.Contains('a')) || (letters_appearing.Contains('b')))
                        {
                            if (current_page == 1)
                            {
								Output.WriteLine("<span class=\"sbkMebv_NavRowCurrent\">AB</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/1" + url_options + "\" class=\"mbb1\">AB</a> &nbsp; ");
                            }
                        }
                        else
                        {
							Output.WriteLine("<span class=\"sbkMebv_NavRowDisabled\">AB</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('c')) || (letters_appearing.Contains('d')) || (letters_appearing.Contains('e')))
                        {
                            if (current_page == 2)
                            {
								Output.WriteLine("<span class=\"sbkMebv_NavRowCurrent\">CDE</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/2" + url_options + "\">CDE</a> &nbsp; ");
                            }
                        }
                        else
                        {
							Output.WriteLine("<span class=\"sbkMebv_NavRowDisabled\">CDE</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('f')) || (letters_appearing.Contains('g')) || (letters_appearing.Contains('h')))
                        {
                            if (current_page == 3)
                            {
								Output.WriteLine("<span class=\"sbkMebv_NavRowCurrent\">FGH</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/3" + url_options + "\">FGH</a> &nbsp; ");
                            }
                        }
                        else
                        {
							Output.WriteLine("<span class=\"sbkMebv_NavRowDisabled\">FGH</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('i')) || (letters_appearing.Contains('j')) || (letters_appearing.Contains('k')))
                        {
                            if (current_page == 4)
                            {
								Output.WriteLine("<span class=\"sbkMebv_NavRowCurrent\">IJK</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/4" + url_options + "\">IJK</a> &nbsp; ");
                            }
                        }
                        else
                        {
							Output.WriteLine("<span class=\"sbkMebv_NavRowDisabled\">IJK</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('l')) || (letters_appearing.Contains('m')) || (letters_appearing.Contains('n')))
                        {
                            if (current_page == 5)
                            {
								Output.WriteLine("<span class=\"sbkMebv_NavRowCurrent\">LMN</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/5" + url_options + "\">LMN</a> &nbsp; ");
                            }
                        }
                        else
                        {
							Output.WriteLine("<span class=\"sbkMebv_NavRowDisabled\">LMN</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('o')) || (letters_appearing.Contains('p')) || (letters_appearing.Contains('q')))
                        {
                            if (current_page == 6)
                            {
								Output.WriteLine("<span class=\"sbkMebv_NavRowCurrent\">OPQ</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/6" + url_options + "\">OPQ</a> &nbsp; ");
                            }
                        }
                        else
                        {
							Output.WriteLine("<span class=\"sbkMebv_NavRowDisabled\">OPQ</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('r')) || (letters_appearing.Contains('s')) || (letters_appearing.Contains('t')))
                        {
                            if (current_page == 7)
                            {
								Output.WriteLine("<span class=\"sbkMebv_NavRowCurrent\">RST</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/7" + url_options + "\">RST</a> &nbsp; ");
                            }
                        }
                        else
                        {
							Output.WriteLine("<span class=\"sbkMebv_NavRowDisabled\">RST</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('u')) || (letters_appearing.Contains('v')) || (letters_appearing.Contains('w')))
                        {
                            if (current_page == 8)
                            {
								Output.WriteLine("<span class=\"sbkMebv_NavRowCurrent\">UVW</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/8" + url_options + "\">UVW</a> &nbsp; ");
                            }
                        }
                        else
                        {
							Output.WriteLine("<span class=\"sbkMebv_NavRowDisabled\">UVW</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('x')) || (letters_appearing.Contains('y')) || (letters_appearing.Contains('z')))
                        {
                            if (current_page == 9)
                            {
								Output.WriteLine("<span class=\"sbkMebv_NavRowCurrent\">XYZ</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/9" + url_options + "\">XYZ</a> &nbsp; ");
                            }
                        }
                        else
                        {
							Output.WriteLine("<span class=\"sbkMebv_NavRowDisabled\" >XYZ</span> &nbsp; ");
                        }

                        Output.WriteLine("</div>");

                        Output.WriteLine("<br />");
                        Output.WriteLine("<br />");



                        // Find the start character and last character, per the page
                        char first_char = ' ';
                        char stop_char = 'c';
                        switch (current_page)
                        {
                            case 2:
                                first_char = 'c';
                                stop_char = 'f';
                                break;

                            case 3:
                                first_char = 'f';
                                stop_char = 'i';
                                break;

                            case 4:
                                first_char = 'i';
                                stop_char = 'l';
                                break;

                            case 5:
                                first_char = 'l';
                                stop_char = 'o';
                                break;

                            case 6:
                                first_char = 'o';
                                stop_char = 'r';
                                break;

                            case 7:
                                first_char = 'r';
                                stop_char = 'u';
                                break;

                            case 8:
                                first_char = 'u';
                                stop_char = 'x';
                                break;

                            case 9:
                                first_char = 'x';
                                stop_char = '}';
                                break;
                        }

                        // Add the pertinent rows
                        foreach (string thisValue in results)
                        {
                            if (thisValue.Length > 0)
                            {
                                char this_first_char = Char.ToLower(thisValue[0]);
                                if ((this_first_char >= first_char) && (this_first_char < stop_char))
                                {
                                    Output.WriteLine("<a href=\"" + search_url.Replace("%3c%25TERM%25%3e", thisValue.Trim().Replace(",", "%2C").Replace("&", "%26").Replace("\"", "%22")).Replace("&","&amp;") + "\">" + thisValue.Replace("\"", "&quot;").Replace("&","&amp;") + "</a><br />");
                                }
                            }
                        }

                    }
                    else
                    {
                        // Determine the first valid page
                        char label_char = 'a';
                        int first_valid_page = -1;
                        int counter = 1;
                        while (label_char <= 'z')
                        {
                            if (letters_appearing.Contains(label_char))
                            {
                                if (first_valid_page < 0)
                                    first_valid_page = counter;
                            }

                            counter++;
                            label_char = (char)((label_char) + 1);
                        }

                        // Define the limits of the page value
                        if ((current_page < first_valid_page) || (current_page > 26))
                            current_page = first_valid_page;


                        // Add the links for paging through results
                        label_char = 'a';
                        counter = 1;
						Output.WriteLine("<div class=\"sbkMebv_NavRow\">");
                        while (label_char <= 'z')
                        {
                            if (letters_appearing.Contains(label_char))
                            {
                                if (current_page == counter)
                                {
									Output.WriteLine("<span class=\"sbkMebv_NavRowCurrent\">" + Char.ToUpper(label_char) + "</span>&nbsp;");
                                }
                                else
                                {
                                    Output.WriteLine("<a href=\"" + page_url + "/" + counter + url_options + "\" >" + Char.ToUpper(label_char) + "</a>&nbsp;");

                                }

                            }
                            else
                            {
								Output.WriteLine("<span class=\"sbkMebv_NavRowDisabled\" >" + Char.ToUpper(label_char) + "</span>&nbsp;");
                            }

                            counter++;
                            label_char = (char)((label_char) + 1);
                        }
                        Output.WriteLine("</div>");

                        Output.WriteLine("<br />");
                        Output.WriteLine("<br />");

                        // Find the start character and last character, per the page
                        char first_char = ' ';
                        char stop_char = 'b';
                        if (current_page > 1)
                        {
                            first_char = (char)(96 + current_page);
                            stop_char = (char)(97 + current_page);

                        }


                        // Add the pertinent rows
                        foreach (string thisValue in results)
                        {
                            if (thisValue.Length > 0)
                            {
                                char this_first_char = Char.ToLower(thisValue[0]);
                                if ((this_first_char >= first_char) && (this_first_char < stop_char))
                                {
                                    Output.WriteLine("<a href=\"" + search_url.Replace("%3c%25TERM%25%3e", thisValue.Trim().Replace(",", "%2C").Replace("&", "%26").Replace("\"", "%22")).Replace("&", "&amp;") + "\">" + thisValue.Replace("\"", "&quot;").Replace("&", "&amp;") + "</a><br />");
                                }
                            }
                        }
                    }
                }
                else
                {
                    Output.WriteLine("<br /><br /><br /><br />");
                    Output.WriteLine(currentMode.Info_Browse_Mode.Length == 0 ? "<center>Select a metadata field to browse by from the list on the left</center>" : "<center>NO MATCHING VALUES</center>");
                    Output.WriteLine("<br /><br />");
                }
            }

            // Set the current mode back
			currentMode.Mode = Display_Mode_Enum.Aggregation;
            currentMode.Aggregation_Type = Aggregation_Type_Enum.Browse_By;

            Output.WriteLine("</div>");
            Output.WriteLine("<br />");

            if ((public_browses.Count > 1) || (internal_browses.Count > 0))
            {
                Output.WriteLine("</td>");
                Output.WriteLine("</tr>");
                Output.WriteLine("</table>");
            }
            Output.WriteLine();
        }
    }
}
