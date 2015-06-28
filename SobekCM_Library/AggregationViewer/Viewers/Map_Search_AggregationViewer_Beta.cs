#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Text;
using SobekCM.Core.Aggregations;
using SobekCM.Library.HTML;
using SobekCM.Library.Settings;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Beta version of map search (from USACH project)   </summary>
    public class Map_Search_AggregationViewer_Beta : abstractAggregationViewer
    {
        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value -1 </value>
        public int Viewer_Width
        {
            get { return -1; }
        }

        /// <summary> Constructor for a new instance of the Map_Search_AggregationViewer class </summary>
        ///<param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Map_Search_AggregationViewer_Beta(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {

            ////redirect
            //currentMode.Redirect_URL();

            #region MY STUFF

            // Start to build the response
            StringBuilder mapSearchBuilder = new StringBuilder();

            //start of custom content
            mapSearchBuilder.AppendLine("<td> MapSearchHolder");

            #region TEMP HEADER FILES

            ////standard css
            //mapSearchBuilder.AppendLine(" <link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/jquery-ui.css\"/> ");
            //mapSearchBuilder.AppendLine(" <link rel=\"stylesheet\" href=\"" + CurrentMode.Base_URL + "default/jquery-searchbox.css\"/> ");

            ////standard js files
            //mapSearchBuilder.AppendLine(" <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.3.custom.min.js\"></script> ");
            //mapSearchBuilder.AppendLine(" <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-migrate-1.1.1.min.js\"></script> ");
            //mapSearchBuilder.AppendLine(" <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-rotate.js\"></script> ");
            //mapSearchBuilder.AppendLine(" <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-knob.js\"></script> ");
            //mapSearchBuilder.AppendLine(" <script type=\"text/javascript\" src=\"" + CurrentMode.Base_URL + "default/scripts/jquery/jquery-json-2.4.min.js\"></script> ");

            mapSearchBuilder.AppendLine("     <script type=\"text/javascript\" src=\"https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&key=AIzaSyCzliz5FjUlEI9D2605b33-etBrENSSBZM&libraries=drawing\"></script> ");
            mapSearchBuilder.AppendLine("     <script src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/mapsearch/external_markerclusterer_compiled.js\"></script>  ");
            mapSearchBuilder.AppendLine("     <script src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/mapsearch/external_jquery_1.10.2.js\"></script> ");
            mapSearchBuilder.AppendLine("     <script src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/mapsearch/external_jquery_ui.min.js\"></script> ");
            mapSearchBuilder.AppendLine("     <script src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/mapsearch/external_jquery_ui_labeledslider.js\"></script> ");
            mapSearchBuilder.AppendLine("     <script type=\"text/javascript\" src=\"" + Static_Resources.Gmaps_Infobox_Js + "\" /></script> ");
            mapSearchBuilder.AppendLine("     <script type=\"text/javascript\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/mapsearch/custom_geoObjects.js\"></script>  ");
            mapSearchBuilder.AppendLine("     <script type=\"text/javascript\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/mapsearch/sobekcm_mapsearch.js\"></script> ");
            mapSearchBuilder.AppendLine("     <link rel=\"stylesheet\" href=\"" + Static_Resources.Jquery_1_10_2_Js +  "default/external_jquery_ui_1.10.4.css\"> ");
            mapSearchBuilder.AppendLine("     <link rel=\"stylesheet\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/SobekCM_MapSearch.css\"> ");

            //apply theming
            mapSearchBuilder.AppendLine("     <link rel=\"stylesheet\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/themes/mapsearch/custom_theme_grey.css\"> ");

            #endregion

            #region Literal

            //mapSearchBuilder.AppendLine("  ");
            //mapSearchBuilder.AppendLine("  ");
            //mapSearchBuilder.AppendLine("  ");
            //mapSearchBuilder.AppendLine(" <div id=\"container_main\"> ");
            ////mapSearchBuilder.AppendLine("     <div id=\"container_header\"> ");
            ////mapSearchBuilder.AppendLine("     	<div id=\"container_header_pt1\"></div> ");
            ////mapSearchBuilder.AppendLine("         <div id=\"container_header_pt2\"></div> ");
            ////mapSearchBuilder.AppendLine("         <div id=\"container_header_pt3\"></div> ");
            ////mapSearchBuilder.AppendLine("     </div> ");
            //mapSearchBuilder.AppendLine("     <div id=\"container_body\"> ");
            //mapSearchBuilder.AppendLine("     	<div id=\"container_toolbar1\"> ");
            //mapSearchBuilder.AppendLine("         	<div id=\"container_addSearch\"> ");
            //mapSearchBuilder.AppendLine("             	<div id=\"container_searchControl\"> ");
            //mapSearchBuilder.AppendLine("                     <div id=\"searchControl_text\">New Search</div> ");
            //mapSearchBuilder.AppendLine("                     <div id=\"searchControl_inputs\" class=\"form-wrapper cf\"> ");
            //mapSearchBuilder.AppendLine(" 	                    <input type=\"text\" id=\"userDefinedSearch\" placeholder=\"Search for...\" /> ");
            //mapSearchBuilder.AppendLine("                     </div> ");
            //mapSearchBuilder.AppendLine("                 </div> ");
            //mapSearchBuilder.AppendLine("             </div> ");
            //mapSearchBuilder.AppendLine("         	<div id=\"container_searchText\"> ");
            //mapSearchBuilder.AppendLine("             	<div id=\"searchText\"> ");
            //mapSearchBuilder.AppendLine("                     You searched for <div id=\"searchText_filter\">all items <a></a></div> <div id=\"searchText_filter\">something <a></a></div> <div id=\"searchText_filter\">points <a></a></div> <div id=\"searchText_filter\">stuff <a></a></div> <div id=\"searchText_filter\">this <a></a></div> <div id=\"searchText_filter\">that <a></a></div> <div id=\"searchText_filter\">something else <a></a></div> <div id=\"searchText_filter\">bacon <a></a></div> <div id=\"searchText_filter\">st augustine <a></a></div> <div id=\"searchText_filter\">wells <a></a></div> inside the \"Unearthing St. Augustine's Colonial Heritage\" collection.  ");
            //mapSearchBuilder.AppendLine(" 					<!-- Here is enough text for the third line, aaahh the third line! We are not quite there yet... wait for it.. are you waiting? I thought so... --> ");
            //mapSearchBuilder.AppendLine("                 </div> ");
            //mapSearchBuilder.AppendLine("             </div> ");
            //mapSearchBuilder.AppendLine("             <div id=\"container_addFilter\"> ");
            //mapSearchBuilder.AppendLine(" 	            <div id=\"container_filterControl\"> ");
            //mapSearchBuilder.AppendLine("                     <div id=\"filterControl_text\">Add Filter</div> ");
            //mapSearchBuilder.AppendLine("                     <div id=\"filterControl_inputs\" class=\"form-wrapper cf\"> ");
            //mapSearchBuilder.AppendLine("                         <input id=\"userDefinedFilter\" type=\"text\" placeholder=\"Filter for...\"> ");
            //mapSearchBuilder.AppendLine("                         <div id=\"searchBox_in\">IN</div> ");
            //mapSearchBuilder.AppendLine("                         <select id=\"filterList\"> ");
            //mapSearchBuilder.AppendLine("                             <option class=\"filterListItem\">Language</option>  ");
            //mapSearchBuilder.AppendLine("                             <option class=\"filterListItem\">Publisher</option> ");
            //mapSearchBuilder.AppendLine("                             <option class=\"filterListItem\">Topic</option> ");
            //mapSearchBuilder.AppendLine("                             <option class=\"filterListItem\">Geographic Area</option> ");
            //mapSearchBuilder.AppendLine("                             <option class=\"filterListItem\">Genre</option> ");
            //mapSearchBuilder.AppendLine("                         </select> ");
            //mapSearchBuilder.AppendLine("                     </div>   ");
            //mapSearchBuilder.AppendLine("                 </div> ");
            //mapSearchBuilder.AppendLine("             </div>  ");
            //mapSearchBuilder.AppendLine("         </div> ");
            //mapSearchBuilder.AppendLine("     	<div id=\"container_toolbar2\"> ");
            //mapSearchBuilder.AppendLine("         	<div id=\"container_view\"> ");
            //mapSearchBuilder.AppendLine("                 <div id=\"container_view_buttons\"> ");
            //mapSearchBuilder.AppendLine("                     <div id=\"view_geo\" class=\"view_button\"></div> ");
            //mapSearchBuilder.AppendLine("                     <div id=\"view_brief\" class=\"view_button\"></div> ");
            //mapSearchBuilder.AppendLine("                     <div id=\"view_thumb\" class=\"view_button\"></div> ");
            //mapSearchBuilder.AppendLine("                     <div id=\"view_table\" class=\"view_button\"></div> ");
            //mapSearchBuilder.AppendLine("                 </div> ");
            //mapSearchBuilder.AppendLine("             </div> ");
            //mapSearchBuilder.AppendLine("             <div id=\"container_resultsHeader\"> ");
            //mapSearchBuilder.AppendLine("             <div class=\"center-helper\"> ");
            //mapSearchBuilder.AppendLine("             <div class=\"center-inner\"> ");
            //mapSearchBuilder.AppendLine("                 <div id=\"container_resultsPagingLeft\"> ");
            //mapSearchBuilder.AppendLine("                     <div id=\"resultsPagingLeft\"> ");
            //mapSearchBuilder.AppendLine("                         <div id=\"firstPage\" class=\"pagingButton\"> ");
            //mapSearchBuilder.AppendLine("                             <div class=\"line-left\"></div> ");
            //mapSearchBuilder.AppendLine("                             <div class=\"arrow-left\"></div>                   	 ");
            //mapSearchBuilder.AppendLine("                             &nbsp;First ");
            //mapSearchBuilder.AppendLine("                         </div> ");
            //mapSearchBuilder.AppendLine("                         <div id=\"prevPage\" class=\"pagingButton\"> ");
            //mapSearchBuilder.AppendLine("                             <div class=\"arrow-left\"></div> ");
            //mapSearchBuilder.AppendLine("                             &nbsp;Previous ");
            //mapSearchBuilder.AppendLine("                         </div> ");
            //mapSearchBuilder.AppendLine("                     </div> ");
            //mapSearchBuilder.AppendLine("                 </div>             ");
            //mapSearchBuilder.AppendLine("                 <div id=\"container_resultsCounter\"> ");
            //mapSearchBuilder.AppendLine("                     <div id=\"resultsCounter\"></div> ");
            //mapSearchBuilder.AppendLine("                 </div> ");
            //mapSearchBuilder.AppendLine("                 <div id=\"container_resultsPagingRight\"> ");
            //mapSearchBuilder.AppendLine("                     <div id=\"resultsPagingRight\"> ");
            //mapSearchBuilder.AppendLine("                         <div id=\"nextPage\" class=\"pagingButton\"> ");
            //mapSearchBuilder.AppendLine("                             Next&nbsp; ");
            //mapSearchBuilder.AppendLine("                             <div class=\"arrow-right\"></div> ");
            //mapSearchBuilder.AppendLine("                         </div> ");
            //mapSearchBuilder.AppendLine("                         <div id=\"lastPage\" class=\"pagingButton\"> ");
            //mapSearchBuilder.AppendLine("                             Last&nbsp; ");
            //mapSearchBuilder.AppendLine("                             <div class=\"line-right\"></div>  ");
            //mapSearchBuilder.AppendLine("                             <div class=\"arrow-right\"></div> ");
            //mapSearchBuilder.AppendLine("                         </div> ");
            //mapSearchBuilder.AppendLine("                     </div> ");
            //mapSearchBuilder.AppendLine("                 </div>            	 ");
            //mapSearchBuilder.AppendLine("             </div> ");
            //mapSearchBuilder.AppendLine("             </div> ");
            //mapSearchBuilder.AppendLine("             </div> ");
            //mapSearchBuilder.AppendLine("             <div id=\"container_resultsSorter\"> ");
            //mapSearchBuilder.AppendLine("                 <form > ");
            //mapSearchBuilder.AppendLine("                     <select id=\"resultsSorter\" class=\"desc\"> ");
            //mapSearchBuilder.AppendLine("                         <option class=\"opt\">Sort By Title (Desc)</option>  ");
            //mapSearchBuilder.AppendLine("                         <option class=\"opt\">Sort By Title (Asc)</option>  ");
            //mapSearchBuilder.AppendLine("                         <option class=\"opt\">Sort By BibID (Desc)</option> ");
            //mapSearchBuilder.AppendLine("                         <option class=\"opt\">Sort By BibID (Asc)</option> ");
            //mapSearchBuilder.AppendLine("                         <option class=\"opt\">Sort By Date (Desc)</option> ");
            //mapSearchBuilder.AppendLine("                         <option class=\"opt\">Sort By Date (Asc)</option> ");
            //mapSearchBuilder.AppendLine("                     </select> ");
            //mapSearchBuilder.AppendLine("                 </form> ");
            //mapSearchBuilder.AppendLine("             </div> ");
            //mapSearchBuilder.AppendLine("         </div>                 ");
            //mapSearchBuilder.AppendLine("         <div id=\"container_toolbox1\"> ");
            //mapSearchBuilder.AppendLine("         	<div id=\"container_filterBox\"> ");
            //mapSearchBuilder.AppendLine("                 <div id=\"container_filterBox_header\"> ");
            //mapSearchBuilder.AppendLine("                     <div id=\"filterBox_header\">Narrow Results By:</div> ");
            //mapSearchBuilder.AppendLine("                 </div> ");
            //mapSearchBuilder.AppendLine("                  ");
            //mapSearchBuilder.AppendLine("                 <!-- <div id=\"container_addFilter2\"> ");
            //mapSearchBuilder.AppendLine(" 	            <div id=\"container_filterControl2\"> ");
            //mapSearchBuilder.AppendLine("                     <div id=\"filterControl_text2\">Add Filter</div> ");
            //mapSearchBuilder.AppendLine("                     <div id=\"filterControl_inputs2\" class=\"form-wrapper cf\"> ");
            //mapSearchBuilder.AppendLine("                         <input id=\"userDefinedFilter2\" type=\"text\" placeholder=\"Filter for...\"> ");
            //mapSearchBuilder.AppendLine("                         <div id=\"filterBox_in2\">IN</div> ");
            //mapSearchBuilder.AppendLine("                         <select id=\"filterList2\"> ");
            //mapSearchBuilder.AppendLine("                             <option class=\"filterListItem\">Language</option>  ");
            //mapSearchBuilder.AppendLine("                             <option class=\"filterListItem\">Publisher</option> ");
            //mapSearchBuilder.AppendLine("                             <option class=\"filterListItem\">Topic</option> ");
            //mapSearchBuilder.AppendLine("                             <option class=\"filterListItem\">Geographic Area</option> ");
            //mapSearchBuilder.AppendLine("                             <option class=\"filterListItem\">Genre</option> ");
            //mapSearchBuilder.AppendLine("                         </select> ");
            //mapSearchBuilder.AppendLine("                     </div>   ");
            //mapSearchBuilder.AppendLine("                 </div> ");
            //mapSearchBuilder.AppendLine("             	</div> --> ");
            //mapSearchBuilder.AppendLine("                  ");
            //mapSearchBuilder.AppendLine("                 <div id=\"filterBox\"> ");
            //mapSearchBuilder.AppendLine("                     <h3>Language</h3> ");
            //mapSearchBuilder.AppendLine("                     <div> ");
            //mapSearchBuilder.AppendLine("                     	<div class=\"sortResults\">sort A - z</div> ");
            //mapSearchBuilder.AppendLine("                         <ul> ");
            //mapSearchBuilder.AppendLine("                         <li>French <a>(503)</a></li> ");
            //mapSearchBuilder.AppendLine("                         <li>Spanish <a>(74)</a></li> ");
            //mapSearchBuilder.AppendLine("                         <li>French <a>(5)</a></li> ");
            //mapSearchBuilder.AppendLine("                         <li>Other <a>(4)</a></li> ");
            //mapSearchBuilder.AppendLine("                         </ul> ");
            //mapSearchBuilder.AppendLine("                         <div class=\"loadmoreResults\">load more</div> ");
            //mapSearchBuilder.AppendLine("                     </div> ");
            //mapSearchBuilder.AppendLine("                     <h3>Publisher</h3> ");
            //mapSearchBuilder.AppendLine("                     <div> ");
            //mapSearchBuilder.AppendLine("                     	<div class=\"sortResults\">sort A - z</div> ");
            //mapSearchBuilder.AppendLine("                         <ul> ");
            //mapSearchBuilder.AppendLine("                           <li>St. Augustine Restoration, Inc. <a>(1108)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>St. Augustine Historical Restoration and  Preservation Commission <a>(219)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>The St. Augustine Record <a>(80)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>Planning and Building Department, City of St.  Augustine, FL <a>(41)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>Ramola Drost <a>(36)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>National Archives and Records Office, United  States of America <a>(30)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>St. Augustine Historical Society <a>(27)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>Historic St. Augustine Preservation Board <a>(23)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>State Photographic Archives, Strozier Library,  Florida State University <a>(14)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>National Archives <a>(12)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>Florida Souvenir, Co. <a>(10)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>Library of Congress <a>(9)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>Florida State News Bureau <a>(8)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>George A. Smathers Libraries, University of  Florida <a>(7)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>Koppel Color Cards <a>(7)</a></li> ");
            //mapSearchBuilder.AppendLine("                         </ul> ");
            //mapSearchBuilder.AppendLine("                         <div class=\"loadmoreResults\">load more</div> ");
            //mapSearchBuilder.AppendLine("                     </div> ");
            //mapSearchBuilder.AppendLine("                      ");
            //mapSearchBuilder.AppendLine("                     <h3>Topic</h3> ");
            //mapSearchBuilder.AppendLine("                     <div> ");
            //mapSearchBuilder.AppendLine(" 	                    <div class=\"sortResults\">sort A - z</div> ");
            //mapSearchBuilder.AppendLine("                         <ul> ");
            //mapSearchBuilder.AppendLine("                           <li>Saint Augustine <a>(890)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>Saint Augustine, Fl <a>(88)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>46 Saint George Street <a>(73)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>Arrivas House <a>(73)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>143 Saint George Street <a>(65)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>Dr. Peck House <a>(65)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>Pe�a-Peck House <a>(65)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>Burt House <a>(64)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>Ximenez-Fatio House <a>(59)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>43 Saint George Street <a>(56)</a></li> ");
            //mapSearchBuilder.AppendLine("                         </ul> ");
            //mapSearchBuilder.AppendLine("                         <div class=\"loadmoreResults\">load more</div> ");
            //mapSearchBuilder.AppendLine("                     </div> ");
            //mapSearchBuilder.AppendLine("                      ");
            //mapSearchBuilder.AppendLine("                     <h3>Geographic Area</h3> ");
            //mapSearchBuilder.AppendLine("                     <div> ");
            //mapSearchBuilder.AppendLine("                     	<div class=\"sortResults\">sort A - z</div> ");
            //mapSearchBuilder.AppendLine("                         <ul> ");
            //mapSearchBuilder.AppendLine("                           <li>Florida <a>(1269)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>North America <a>(1269)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>United States of America <a>(1268)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>Saint Johns <a>(870)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>Saint Augustine <a>(851)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>St. Augustine <a>(399)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>St. Johns <a>(399)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>United States <a>(1)</a></li> ");
            //mapSearchBuilder.AppendLine("                         </ul> ");
            //mapSearchBuilder.AppendLine("                         <div class=\"loadmoreResults\">load more</div> ");
            //mapSearchBuilder.AppendLine("                     </div> ");
            //mapSearchBuilder.AppendLine("                      ");
            //mapSearchBuilder.AppendLine("                     <h3>Genre</h3> ");
            //mapSearchBuilder.AppendLine("                     <div> ");
            //mapSearchBuilder.AppendLine("                     	<div class=\"sortResults\">sort A - z</div> ");
            //mapSearchBuilder.AppendLine("                         <ul> ");
            //mapSearchBuilder.AppendLine("                           <li>newspaper <a>(63)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>plan <a>(35)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>coastal chart <a>(29)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>map <a>(23)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>plan view <a>(20)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>survey <a>(14)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>plan and profile <a>(8)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>profile <a>(7)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>manuscript <a>(2)</a></li> ");
            //mapSearchBuilder.AppendLine("                           <li>survey and profile <a>(2)</a></li> ");
            //mapSearchBuilder.AppendLine("                         </ul> ");
            //mapSearchBuilder.AppendLine("                         <div class=\"loadmoreResults\">load more</div> ");
            //mapSearchBuilder.AppendLine("                     </div> ");
            //mapSearchBuilder.AppendLine("                      ");
            //mapSearchBuilder.AppendLine("                 </div> ");
            //mapSearchBuilder.AppendLine(" 	        </div> ");
            //mapSearchBuilder.AppendLine("         </div> ");
            //mapSearchBuilder.AppendLine("          ");
            //mapSearchBuilder.AppendLine("         <div id=\"container_toolbar3\"> ");
            //mapSearchBuilder.AppendLine("            	<div id=\"container_timebar\"> ");
            //mapSearchBuilder.AppendLine(" 	        	<div id=\"timebar_value\"></div> ");
            //mapSearchBuilder.AppendLine("                 <div id=\"timebar\"></div> ");
            //mapSearchBuilder.AppendLine("             </div> ");
            //mapSearchBuilder.AppendLine("         </div> ");
            //mapSearchBuilder.AppendLine("          ");
            //mapSearchBuilder.AppendLine("         <div id=\"container_mainContent\"> ");
            //mapSearchBuilder.AppendLine("             <div id=\"container_content1\"></div> ");
            //mapSearchBuilder.AppendLine("             <div id=\"container_content2\"></div> ");
            //mapSearchBuilder.AppendLine("             <div id=\"container_content3\"></div> ");
            //mapSearchBuilder.AppendLine("             <div id=\"container_map\"></div> ");
            //mapSearchBuilder.AppendLine(" 		</div> ");
            //mapSearchBuilder.AppendLine("                      ");
            //mapSearchBuilder.AppendLine("         <div id=\"container_toolbox2\"> ");
            //mapSearchBuilder.AppendLine(" 			<div id=\"container_resultsBox\"> ");
            //mapSearchBuilder.AppendLine("             	 ");
            //mapSearchBuilder.AppendLine("                 <div id=\"resultsBox_header\">Search Results</div> ");
            //mapSearchBuilder.AppendLine("                  ");
            //mapSearchBuilder.AppendLine("                 <div id=\"container_toolbox2_scrollContent\"> ");
            //mapSearchBuilder.AppendLine("                 	<div id=\"resultsBox\"> ");
            //mapSearchBuilder.AppendLine("                         <div id=\"result_1\" class=\"result\"> ");
            //mapSearchBuilder.AppendLine("                             <img class=\"result_thumb\" src=\"http://hlmatt.com/uf/part2/resources/result1.jpg\"/> ");
            //mapSearchBuilder.AppendLine("                             <p class=\"result_desc\">Aritst's rendering for possible reconstruction projects along the South side of Cuna Street</p> ");
            //mapSearchBuilder.AppendLine("                         </div> ");
            //mapSearchBuilder.AppendLine("                         <div id=\"result_2\" class=\"result\"> ");
            //mapSearchBuilder.AppendLine("                             <img class=\"result_thumb\" src=\"http://hlmatt.com/uf/part2/resources/result2.jpg\"/> ");
            //mapSearchBuilder.AppendLine("                             <p class=\"result_desc\">Artist rendering of possible reconstruction projects in Block 4 and Block 5, centered on the Perez Sanchez House reconstruction; Corner of Charlotte Street and Treasury Street</p> ");
            //mapSearchBuilder.AppendLine("                         </div> ");
            //mapSearchBuilder.AppendLine("                         <div id=\"result_3\" class=\"result\"> ");
            //mapSearchBuilder.AppendLine("                             <img class=\"result_thumb\" src=\"http://hlmatt.com/uf/part2/resources/result3.jpg\"/> ");
            //mapSearchBuilder.AppendLine("                             <p class=\"result_desc\">Artists rending of a concept for the reconstruction of the Ribera House, including the Blanco House and Carmona House</p> ");
            //mapSearchBuilder.AppendLine("                         </div> ");
            //mapSearchBuilder.AppendLine("                         <div id=\"result_4\" class=\"result\"> ");
            //mapSearchBuilder.AppendLine("                             <img class=\"result_thumb\" src=\"http://hlmatt.com/uf/part2/resources/result4.jpg\"/> ");
            //mapSearchBuilder.AppendLine("                             <p class=\"result_desc\">Artist's Rendering of Block 8, from the corner of Hypolita Street and Charlotte Street, focusing on the Regidor-Clark House</p> ");
            //mapSearchBuilder.AppendLine("                         </div> ");
            //mapSearchBuilder.AppendLine("                     </div> ");
            //mapSearchBuilder.AppendLine("                 </div> ");
            //mapSearchBuilder.AppendLine("                  ");
            //mapSearchBuilder.AppendLine("             </div> ");
            //mapSearchBuilder.AppendLine("         </div> ");
            //mapSearchBuilder.AppendLine("     </div> ");
            //mapSearchBuilder.AppendLine(" </div> ");
            //mapSearchBuilder.AppendLine("  ");

            #endregion

            //end of custom content
            mapSearchBuilder.AppendLine("</td>");

            Search_Script_Reference = (mapSearchBuilder.ToString());

            #endregion

        }
        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation_Views_Searches_Enum.Map_Search"/> enumerational value </value>
        public override Item_Aggregation_Views_Searches_Enum Type
        {
            get { return Item_Aggregation_Views_Searches_Enum.Map_Search; }
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

        /// <summary> Gets the collection of special behaviors which this aggregation viewer  requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> AggregationViewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum>
                        {
                            HtmlSubwriter_Behaviors_Enum.Aggregation_Suppress_Home_Text
                        };
            }
        }


        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This addes the map search panel which holds the google map, as well as the coordinate entry boxes </remarks>
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {

        }

        /// <summary> Add the HTML to be displayed below the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds the search tips by calling the base method <see cref="abstractAggregationViewer.Add_Simple_Search_Tips"/> </remarks>
        public override void Add_Secondary_HTML(TextWriter Output, Custom_Tracer Tracer)
        {

        }
    }
}
