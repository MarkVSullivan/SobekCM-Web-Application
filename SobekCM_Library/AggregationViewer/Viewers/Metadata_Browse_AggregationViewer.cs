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
using SobekCM.Library.WebContent;

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
        private readonly Item_Aggregation_Browse_Info browseObject;
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
            if (( browseObject == null ) || ( browseObject.Source != Item_Aggregation_Browse_Info.Source_Type.Static_HTML))
            {
                // Determine the correct metadata code
                string metadata_code = Long_to_Short(Current_Mode.Info_Browse_Mode.Trim());
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

            if (( browseObject != null ) && ( browseObject.Source == Item_Aggregation_Browse_Info.Source_Type.Static_HTML))
            {
                Output.WriteLine("  <h1>" + browseObject.Get_Label(currentMode.Language) + "</h1>");

            }
            else
            {
                if ((results != null) && (currentMode.Info_Browse_Mode.Length > 0))
                {
                    Output.WriteLine("  <h1>Browse by " + translator.Get_Translation(Short_to_Title(currentMode.Info_Browse_Mode), currentMode.Language) + "</h1>");
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
            ReadOnlyCollection<Item_Aggregation_Browse_Info> public_browses = currentCollection.Browse_By_Pages(currentMode.Language);

            // Determine if this is an internal user and create list of internal user browses
            List<string> internal_browses = new List<string>();
            if ((currentUser != null) && ((currentUser.Is_Internal_User) || (currentUser.Is_Aggregation_Curator(currentMode.Aggregation))))
            {
                internal_browses.Add("affiliation");
                internal_browses.Add("attribution");
                internal_browses.Add("creator");
                internal_browses.Add("cultural context");
                internal_browses.Add("donor");
                internal_browses.Add("format");
                internal_browses.Add("frequency");
                internal_browses.Add("genre");
                internal_browses.Add("country");
                internal_browses.Add("state");
                internal_browses.Add("county");
                internal_browses.Add("city");
                internal_browses.Add("holding location");
                internal_browses.Add("language");
                internal_browses.Add("material");
                internal_browses.Add("type");
                internal_browses.Add("publication place");
                internal_browses.Add("publisher");
                internal_browses.Add("source institution");
                internal_browses.Add("style period");
                internal_browses.Add("all subjects");
                internal_browses.Add("subject keyword");
                internal_browses.Add("temporal subject");
                internal_browses.Add("spatial coverage");
                internal_browses.Add("name as subject");
                internal_browses.Add("title as subject");
                internal_browses.Add("target audience");
                internal_browses.Add("technique");
                internal_browses.Add("tickler");
                internal_browses.Add("title");
            }

            // Retain the original short code (or the first public code)
            string original_browse_mode = currentMode.Info_Browse_Mode;
            string original_short_code = Long_to_Short(currentMode.Info_Browse_Mode);

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
                Output.WriteLine("<tr valign=\"top\">");
                Output.WriteLine("<td width=\"240px\" valign=\"top\" height=\"100%\">");
                Output.WriteLine("<div class=\"SobekFacetColumn\">");
                Output.WriteLine("<div class=\"SobekFacetColumnTitle\">BROWSE BY:</div>");
                Output.WriteLine("<br />");

                if (public_browses.Count > 0)
                {
                    // Sort these by title
                    SortedList<string, Item_Aggregation_Browse_Info> sortedBrowses = new SortedList<string, Item_Aggregation_Browse_Info>();
                    foreach (Item_Aggregation_Browse_Info thisBrowse in public_browses)
                    {
                        string short_code = Long_to_Short(thisBrowse.Code);
                        if (internal_browses.Contains(short_code))
                            internal_browses.Remove(short_code);
                        sortedBrowses[Short_to_Title(short_code)] = thisBrowse;
                    }

                    Output.WriteLine("<b> &nbsp;Public Browses</b><br />");
                    Output.WriteLine("<div class=\"SobekFacetBox\">");
                    foreach (Item_Aggregation_Browse_Info thisBrowse in sortedBrowses.Values)
                    {
                        // Static HTML or metadata browse by?
                        if (thisBrowse.Source == Item_Aggregation_Browse_Info.Source_Type.Static_HTML)
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
                            string short_code = Long_to_Short(thisBrowse.Code);
                            if (short_code != original_short_code)
                            {
                                currentMode.Info_Browse_Mode = short_code.Replace(" ", "_");
                                Output.WriteLine("<a href=\"" + currentMode.Redirect_URL().Replace("&", "&amp") + "\">" + Short_to_Title(short_code) + "</a><br />");
                            }
                            else
                            {
                                Output.WriteLine(Short_to_Title(short_code) + "<br />");
                            }
                        }
                    }

                    Output.WriteLine("</div>");
                    Output.WriteLine("<br />");
                }

                if (internal_browses.Count > 0)
                {
                    Output.WriteLine("<b> &nbsp;Internal Browses</b><br />");
                    Output.WriteLine("<div class=\"SobekFacetBox\">");

                    foreach (string thisShort in internal_browses)
                    {
                        if (thisShort != original_short_code)
                        {
                            currentMode.Info_Browse_Mode = thisShort.Replace(" ", "_");
                            Output.WriteLine("<a href=\"" + currentMode.Redirect_URL().Replace("&","&amp")+ "\">" + Short_to_Title(thisShort) + "</a><br />");
                        }
                        else
                        {
                            Output.WriteLine(Short_to_Title(thisShort) + "<br />");
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
            Output.WriteLine("<div class=\"SobekResultsPanel\" align=\"center\">");

            currentMode.Info_Browse_Mode = original_browse_mode;

            // Was this static or metadata browse by?
            if (( browseObject != null ) && ( browseObject.Source == Item_Aggregation_Browse_Info.Source_Type.Static_HTML))
            {
                // Read the content file for this browse
                HTML_Based_Content staticBrowseContent = browseObject.Get_Static_Content( currentMode.Language, currentMode.Base_URL, Tracer );
           
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
                    currentMode.Search_Fields = Short_to_Code(original_short_code);
                    currentMode.Search_String = "\"<%TERM%>\"";
                    string search_url = currentMode.Redirect_URL();

                    Output.WriteLine("<br />");

                    if (results.Count < 100)
                    {
                        foreach (string thisResult in results)
                        {
                            Output.WriteLine("<a href=\"" + search_url.Replace("%3c%25TERM%25%3e", thisResult.Trim().Replace(",", "%2C").Replace("&", "%26").Replace("\"", "%22")).Replace("&", "&amp") + "\">" + thisResult.Replace("\"", "&quot;").Replace("&", "&amp;") + "</a><br />");
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
                        Output.WriteLine("<div class=\"mbb1_div\">");
                        if ((letters_appearing.Contains('a')) || (letters_appearing.Contains('b')))
                        {
                            if (current_page == 1)
                            {
                                Output.WriteLine("<span class=\"mbb1_current\">AB</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/1" + url_options + "\" class=\"mbb1\">AB</a> &nbsp; ");
                            }
                        }
                        else
                        {
                            Output.WriteLine("<span class=\"mbb1_disabled\">AB</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('c')) || (letters_appearing.Contains('d')) || (letters_appearing.Contains('e')))
                        {
                            if (current_page == 2)
                            {
                                Output.WriteLine("<span class=\"mbb1_current\">CDE</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/2" + url_options + "\">CDE</a> &nbsp; ");
                            }
                        }
                        else
                        {
                            Output.WriteLine("<span class=\"mbb1_disabled\">CDE</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('f')) || (letters_appearing.Contains('g')) || (letters_appearing.Contains('h')))
                        {
                            if (current_page == 3)
                            {
                                Output.WriteLine("<span class=\"mbb1_current\">FGH</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/3" + url_options + "\">FGH</a> &nbsp; ");
                            }
                        }
                        else
                        {
                            Output.WriteLine("<span class=\"mbb1_disabled\">FGH</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('i')) || (letters_appearing.Contains('j')) || (letters_appearing.Contains('k')))
                        {
                            if (current_page == 4)
                            {
                                Output.WriteLine("<span class=\"mbb1_current\">IJK</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/4" + url_options + "\">IJK</a> &nbsp; ");
                            }
                        }
                        else
                        {
                            Output.WriteLine("<span class=\"mbb1_disabled\">IJK</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('l')) || (letters_appearing.Contains('m')) || (letters_appearing.Contains('n')))
                        {
                            if (current_page == 5)
                            {
                                Output.WriteLine("<span class=\"mbb1_current\">LMN</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/5" + url_options + "\">LMN</a> &nbsp; ");
                            }
                        }
                        else
                        {
                            Output.WriteLine("<span class=\"mbb1_disabled\">LMN</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('o')) || (letters_appearing.Contains('p')) || (letters_appearing.Contains('q')))
                        {
                            if (current_page == 6)
                            {
                                Output.WriteLine("<span class=\"mbb1_current\">OPQ</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/6" + url_options + "\">OPQ</a> &nbsp; ");
                            }
                        }
                        else
                        {
                            Output.WriteLine("<span class=\"mbb1_disabled\">OPQ</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('r')) || (letters_appearing.Contains('s')) || (letters_appearing.Contains('t')))
                        {
                            if (current_page == 7)
                            {
                                Output.WriteLine("<span class=\"mbb1_current\">RST</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/7" + url_options + "\">RST</a> &nbsp; ");
                            }
                        }
                        else
                        {
                            Output.WriteLine("<span class=\"mbb1_disabled\">RST</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('u')) || (letters_appearing.Contains('v')) || (letters_appearing.Contains('w')))
                        {
                            if (current_page == 8)
                            {
                                Output.WriteLine("<span class=\"mbb1_current\">UVW</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/8" + url_options + "\">UVW</a> &nbsp; ");
                            }
                        }
                        else
                        {
                            Output.WriteLine("<span class=\"mbb1_disabled\">UVW</span> &nbsp; ");
                        }

                        if ((letters_appearing.Contains('x')) || (letters_appearing.Contains('y')) || (letters_appearing.Contains('z')))
                        {
                            if (current_page == 9)
                            {
                                Output.WriteLine("<span class=\"mbb1_current\">XYZ</span> &nbsp; ");
                            }
                            else
                            {
                                Output.WriteLine("<a href=\"" + page_url + "/9" + url_options + "\">XYZ</a> &nbsp; ");
                            }
                        }
                        else
                        {
                            Output.WriteLine("<span class=\"mbb1_disabled\" >XYZ</span> &nbsp; ");
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
                        Output.WriteLine("<div class=\"mbb1_div\">");
                        while (label_char <= 'z')
                        {
                            if (letters_appearing.Contains(label_char))
                            {
                                if (current_page == counter)
                                {
                                    Output.WriteLine("<span class=\"mbb1_current\">" + Char.ToUpper(label_char) + "</span>&nbsp;");
                                }
                                else
                                {
                                    Output.WriteLine("<a href=\"" + page_url + "/" + counter + url_options + "\" >" + Char.ToUpper(label_char) + "</a>&nbsp;");

                                }

                            }
                            else
                            {
                                Output.WriteLine("<span class=\"mbb1_disabled\" >" + Char.ToUpper(label_char) + "</span>&nbsp;");
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
            currentMode.Mode = Display_Mode_Enum.Aggregation_Browse_By;

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

        private static string Short_to_Code(string Short)
        {
            switch (Short.ToLower().Replace("_", " "))
            {
                case "title":
                    return "TI";

                case "type":
                    return "TY";

                case "identifier":
                case "identifiers":
                    return "ID";

                case "language":
                    return "LA";

                case "creator":
                    return "AU";

                case "publisher":
                    return "PU";

                case "publication place":
                    return "PP";

                case "subject keyword":
                    return "TO";

                case "genre":
                    return "GE";

                case "target audience":
                    return "TA";

                case "spatial coverage":
                    return "SP";

                case "country":
                    return "CO";

                case "state":
                    return "ST";

                case "county":
                    return "CT";

                case "city":
                    return "CI";

                case "source institution":
                    return "SO";

                case "holding location":
                    return "HO";
                    
                case "tickler":
                    return "TL";

                case "donor":
                    return "DO";

                case "format":
                    return "FO";

                case "publication date":
                    return "DA";

                case "affiliation":
                    return "AF";

                case "frequency":
                    return "FR";

                case "name as subject":
                    return "SN";

                case "title as subject":
                    return "ST";

                case "all subjects":
                    return "SU";

                case "temporal subject":
                    return "TE";

                case "attribution":
                    return "AT";

                case "material":
                    return "MA";

                case "style period":
                    return "SY";

                case "technique":
                    return "TQ";

                case "cultural context":
                    return "CC";

                case "inscription":
                    return "IN";

            }

            return "ZZ";
        }

        private string Short_to_Title(string Short)
        {
            switch (Short.ToLower().Replace("_"," "))
            {
                case "title":
                    return "Titles";

                case "type":
                    return "Material Types";

                case "identifier":
                    return "Identifiers";

                case "language":
                    return "Languages";

                case "creator":
                    return "Creators / Contributors";

                case "publisher":
                    return "Publishers";

                case "publication place":
                    return "Publication Place";

                case "subject keyword":
                    return "Subjects ( Topical )";

                case "genre":
                    return "Subjects ( Genre )";

                case "target audience":
                    return "Target Audiences";

                case "spatial coverage":
                    return "Subjects ( Geographic )";

                case "country":
                    return "Geographic ( Countries )";

                case "state":
                    return "Geographic ( States )";

                case "county":
                    return "Geographic ( Counties )";

                case "city":
                    return "Geographic ( Cities )";

                case "source institution":
                    return "Source Institutions";

                case "holding location":
                    return "Holding Locations";

                case "identifiers":
                    return "Identifiers";

                case "tickler":
                    return "Ticklers";

                case "donor":
                    return "Donors";

                case "format":
                    return "Format / Physical Desc";

                case "publication date":
                    return "Publication Dates";

                case "affiliation":
                    return "Affiliations";

                case "frequency":
                    return "Frequencies";

                case "name as subject":
                    return "Subjects ( Name )";

                case "title as subject":
                    return "Subjects ( Title )";

                case "all subjects":
                    return "Subjects ( All )";

                case "temporal subject":
                    return "Subjects ( Temporal )";

                case "attribution":
                    return "Attributions";

                case "material":
                    return "Materials";

                case "style period":
                    return "Style / Period";

                case "technique":
                    return "Technique";

                case "cultural context":
                    return "Cultural Context";

                case "inscription":
                    return "Inscription";
            }

            return Short;
        }

        private string Long_to_Short(string Long)
        {
            switch (Long.Replace(" ", "_").ToUpper())
            {
                case "TITLE":
                case "TITLES":
                    return "title";

                case "TYPE":
                case "RESOURCE_TYPE":
                case "MATERIAL_TYPE":
                    return "type";

                case "LANGUAGE":
                case "LANGUAGES":
                    return "language";

                case "CREATOR":
                case "CREATORS":
                case "AUTHOR":
                case "AUTHORS":
                case "CONTRIBUTOR":
                    return "creator";

                case "PUBLISHER":
                case "PUBLISHERS":
                case "MANUFACTURER":
                case "MANUFACTURERS":
                    return "publisher";

                case "PUBLICATION_PLACE":
                case "PLACE":
                    return "publication place";

                case "SUBJECT_KEYWORD":
                case "SUBJECT_KEYWORDS":
                case "SUBJECT":
                case "SUBJECTS":
                    return "subject keyword";

                case "GENRE":
                case "GENRES":
                    return "genre";

                case "TARGET_AUDIENCE":
                case "TARGET_AUDIENCES":
                case "AUDIENCE":
                case "AUDIENCES":
                    return "target audience";

                case "SPATIAL_COVERAGE":
                case "SPATIAL_COVERAGES":
                case "SPATIAL":
                    return "spatial coverage";

                case "COUNTRY":
                case "COUNTRIES":
                    return "country";

                case "STATE":
                case "STATES":
                    return "state";

                case "COUNTY":
                case "COUNTIES":
                    return "county";

                case "CITY":
                case "CITIES":
                    return "city";

                case "SOURCE_INSTITUTION":
                case "SOURCE":
                    return "source institution";

                case "HOLDING_LOCATION":
                case "HOLDING":
                    return "holding location";

                case "IDENTIFIER":
                case "IDENTIFIERS":
                    return "identifier";

                case "TICKLER":
                case "TKR":
                case "TICKLERS":
                    return "tickler";

                case "DONOR":
                case "DONORS":
                    return "donor";

                case "FORMAT":
                case "FORMATS":
                    return "format";

                case "DATE":
                case "DATES":
                case "PUBLICATION_DATE":
                    return "publication date";

                case "AFFILIATION":
                case "AFFILIATIONS":
                    return "affiliation";

                case "FREQUENCY":
                case "FREQUENCIES":
                    return "frequency";

                case "NAME_AS_SUBJECT":
                case "NAMES_AS_SUBJECT":
                case "NAMED_SUBJECT":
                    return "name as subject";

                case "TITLE_AS_SUBJECT":
                case "TITLES_AS_SUBJECT":
                    return "title as subject";

                case "ALL_SUBJECTS":
                    return "all subjects";

                case "TEMPORAL":
                case "TEMPORALS":
                case "TEMPORAL_SUBJECT":
                case "TEMPORAL_SUBJECTS":
                    return "temporal subject";

                case "ATTRIBUTION":
                case "ATTRIBUTIONS":
                    return "attribution";

                case "MATERIALS":
                    return "material";

                case "STYLE_PERIOD":
                    return "style period";

                case "TECHNIQUE":
                    return "technique";

                case "CULTURAL_CONTEXT":
                    return "cultural context";

                case "INSCRIPTION":
                    return "inscription";
            }

            return Long.ToLower().Replace("_", " ");
        }
    }
}
