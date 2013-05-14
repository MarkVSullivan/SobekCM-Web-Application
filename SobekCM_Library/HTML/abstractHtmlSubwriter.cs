#region Using directives

using System;
using System.IO;
using SobekCM.Library.Aggregations;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Skins;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Abstract class which all HTML subwriters must extend.  This class contains some of the
    /// basic HTML-writing helper values and contains some of the values used by many of the subclasses.
    /// HTML subwriters are the top level writing classes employed by the <see cref="Html_MainWriter"/>. </summary>
    public abstract class abstractHtmlSubwriter
    {
        private const string SELECTED_TAB_START_ORIG = "<img src=\"{0}design/skins/{1}/tabs/cL_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab_s\"> ";
        private const string SELECTED_TAB_END_ORIG = " </span><img src=\"{0}design/skins/{1}/tabs/cR_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />";
        private const string UNSELECTED_TAB_START_ORIG = "<img src=\"{0}design/skins/{1}/tabs/cL.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab\"> ";
        private const string UNSELECTED_TAB_END_ORIG = " </span><img src=\"{0}design/skins/{1}/tabs/cR.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />";
        private const string DOWN_TAB_START_ORIG = "<img src=\"{0}design/skins/{1}/tabs/cLD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab\">";
        private const string DOWN_TAB_END_ORIG = "</span><img src=\"{0}design/skins/{1}/tabs/cRD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />";
        private const string DOWN_SELECTED_TAB_START_ORIG = "<img src=\"{0}design/skins/{1}/tabs/cLD_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab_s\">";
        private const string DOWN_SELECTED_TAB_END_ORIG = "</span><img src=\"{0}design/skins/{1}/tabs/cRD_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />";

        /// <summary> Protected field contains the html for the end of an downard selected tab </summary>
        /// <value>Always returns the value '&lt;/span&gt;&lt;img src=&quot;design/skins/[SKIN_CODE]/tabs/cRD_s.gif&quot; border=&quot;0&quot; class=&quot;tab_image&quot; />'</value>
        protected string Down_Selected_Tab_End;

        /// <summary> Protected field contains the html for the beginning of an downard selected tab </summary>
        /// <value>Always returns the value '&lt;img src=&quot;design/skins/[SKIN_CODE]/tabs/cLD_s.gif&quot; border=&quot;0&quot; class=&quot;tab_image&quot; /&gt;&lt;span class=&quot;tab_s&quot;&gt;'</value>
        protected string Down_Selected_Tab_Start;

        /// <summary> Protected field contains the html for the end of an downward unselected tab </summary>
        /// <value>Always returns the value '&lt;/span&gt;&lt;img src=&quot;design/skins/[SKIN_CODE]/tabs/cRD.gif&quot; border=&quot;0&quot; class=&quot;tab_image&quot; /&gt;'</value>
        protected string Down_Tab_End;

        /// <summary> Protected field contains the html for the beginning of an downward unselected tab </summary>
        /// <value>Always returns the value '&lt;img src=&quot;design/skins/[SKIN_CODE]}/tabs/cLD.gif&quot; border=&quot;0&quot; class=&quot;tab_image&quot; /&gt;&lt;span class=&quot;tab&quot;&gt;'</value>
        protected string Down_Tab_Start;

        /// <summary> Protected field contains the html for the end of an upright selected tab </summary>
        /// <value>Always returns the value '&lt;/span&gt;&lt;img src=&quot;design/skins/[SKIN_CODE]/tabs/cR_s.gif&quot; border=&quot;0&quot; class=&quot;tab_image&quot; /&gt;'</value>
        protected string Selected_Tab_End;

        /// <summary> Protected field contains the html for the beginning of an upright selected tab </summary>
        /// <value>Always returns the value '&lt;img src=&quot;design/skins/[SKIN_CODE]/tabs/cL_s.gif&quot; border=&quot;0&quot; class=&quot;tab_image&quot; /&gt;'&lt;span class=&quot;tab_s&quot;></value>
        protected string Selected_Tab_Start;

        /// <summary> Protected field contains the html for the end of an upright unselected tab </summary>
        /// <value>Always returns the value '&lt;/span&gt;&lt;img src=&quot;design/skins/[SKIN_CODE]/tabs/cR.gif&quot; border=&quot;0&quot; class=&quot;tab_image&quot; /&gt;'</value>
        protected string Unselected_Tab_End;

        /// <summary> Protected field contains the html for the beginning of an upright unselected tab </summary>
        /// <value>Always returns the value '&lt;img src=&quot;design/skins/[SKIN_CODE]/tabs/cL.gif&quot; border=&quot;0&quot; class=&quot;tab_image&quot; /&gt;&lt;span class=&quot;tab&quot;&gt;'</value>
        protected string Unselected_Tab_Start;

        /// <summary> Protected field contains the mode / navigation information for the current request</summary>
        protected SobekCM_Navigation_Object currentMode;

        /// <summary> Protected field contains the HTML Web skin which controls the overall appearance of this digital library </summary>
        protected SobekCM_Skin_Object htmlSkin;

        /// <summary> Current item aggregation object  </summary>
        public Item_Aggregation Hierarchy_Object { get; set; }

        /// <summary> HTML Web skin which controls the overall appearance of this digital library </summary>
        /// <remarks> This also sets all of the protected tab html strings </remarks>
        public SobekCM_Skin_Object Skin
        {
            get { return htmlSkin; }
            set 
            { 
                htmlSkin = value;

                if (currentMode != null)
                {
                    Selected_Tab_Start = String.Format(SELECTED_TAB_START_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                    Selected_Tab_End = String.Format(SELECTED_TAB_END_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                    Unselected_Tab_Start = String.Format(UNSELECTED_TAB_START_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                    Unselected_Tab_End = String.Format(UNSELECTED_TAB_END_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                    Down_Tab_Start = String.Format(DOWN_TAB_START_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                    Down_Tab_End = String.Format(DOWN_TAB_END_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                    Down_Selected_Tab_Start = String.Format(DOWN_SELECTED_TAB_START_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                    Down_Selected_Tab_End = String.Format(DOWN_SELECTED_TAB_END_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                }
            }
        }

        /// <summary>  Mode / navigation information for the current request </summary>
        /// <remarks> This also sets all of the protected tab html strings </remarks>
        public SobekCM_Navigation_Object Mode
        {
            get { return currentMode; }
            set 
            { 
                currentMode = value;

                if (htmlSkin != null)
                {
                    Selected_Tab_Start = String.Format(SELECTED_TAB_START_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                    Selected_Tab_End = String.Format(SELECTED_TAB_END_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                    Unselected_Tab_Start = String.Format(UNSELECTED_TAB_START_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                    Unselected_Tab_End = String.Format(UNSELECTED_TAB_END_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                    Down_Tab_Start = String.Format(DOWN_TAB_START_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                    Down_Tab_End = String.Format(DOWN_TAB_END_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                    Down_Selected_Tab_Start = String.Format(DOWN_SELECTED_TAB_START_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                    Down_Selected_Tab_End = String.Format(DOWN_SELECTED_TAB_END_ORIG, currentMode.Base_URL, htmlSkin.Base_Skin_Code);
                }          
            }
        }

        /// <summary> Title for this web page </summary>
        /// <value> This value is set by each of the sub classes </value>
        public string WebPage_Title { get; protected set; }

        /// <summary> Writes the HTML generated by this abstract html subwriter directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public abstract bool Write_HTML(TextWriter Output, Custom_Tracer Tracer );

        /// <summary> Adds the internal header HTML for this specific HTML writer </summary>
        /// <param name="Output"> Stream to which to write the HTML for the internal header information </param>
        /// <param name="Current_User"> Currently logged on user, to determine specific rights </param>
        public virtual void Add_Internal_Header_HTML( TextWriter Output, User_Object Current_User )
        {
            Output.WriteLine("  <table cellspacing=\"0\" cellpadding=\"2\" id=\"internalheader\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td align=\"left\">");
            Output.WriteLine("          <button title=\"Hide Internal Header\" class=\"intheader_button_aggr hide_intheader_button_aggr\" onclick=\"return hide_internal_header();\"></button>");
            Output.WriteLine("      </td>");
            Add_Internal_Header_Search_Box(Output);
            Output.WriteLine("    </tr>");
            Output.WriteLine("  </table>"); 
        }

        /// <summary> Adds the internal header search box to the current output stream  </summary>
        /// <param name="Output"> Output stream to write the html for the internal header search box to </param>
        protected void Add_Internal_Header_Search_Box(TextWriter Output)
        {
                Output.WriteLine("      <td align=\"right\" valign=\"middle\" width=\"340px\">");
                Output.WriteLine("        <table>");
                Output.WriteLine("          <tr height=\"16px\" valign=\"top\">");
                Output.WriteLine("            <td valign=\"top\">");
                Output.Write("              <input name=\"internalSearchTextBox\" type=\"text\" id=\"internalSearchTextBox\" class=\"SobekInternalSearchBox\" value=\"\" onfocus=\"javascript:textbox_enter('internalSearchTextBox', 'SobekInternalSearchBox_focused')\" onblur=\"javascript:textbox_leave('internalSearchTextBox', 'SobekInternalSearchBox')\"");
                if (currentMode.Browser_Type.IndexOf("IE") >= 0)
                    Output.WriteLine(" onkeydown=\"internalTrapKD(event, '" + currentMode.Base_URL + "contains');\" />");
                else
                    Output.WriteLine(" onkeydown=\"return internalTrapKD(event, '" + currentMode.Base_URL + "contains');\" />");
                Output.WriteLine("              <select name=\"internalDropDownList\" id=\"internalDropDownList\" class=\"SobekInternalSelectBox\" >");
                Output.WriteLine("                <option value=\"BI\" selected=\"selected\">BibID</option>");
                Output.WriteLine("                <option value=\"OC\">OCLC Number</option>");
                Output.WriteLine("                <option value=\"AL\">ALEPH Number</option>");
                Output.WriteLine("                <option value=\"ZZ\">Anywhere</option>");
                Output.WriteLine("                <option value=\"TI\">Title</option>");
                Output.WriteLine("                <option value=\"AU\">Author</option>");
                Output.WriteLine("                <option value=\"SU\">Subject Keywords</option>");
                Output.WriteLine("                <option value=\"CO\">Country</option>");
                Output.WriteLine("                <option value=\"ST\">State</option>");
                Output.WriteLine("                <option value=\"CT\">County</option>");
                Output.WriteLine("                <option value=\"CI\">City</option>");
                Output.WriteLine("                <option value=\"PP\">Place of Publication</option>");
                Output.WriteLine("                <option value=\"SP\">Spatial Coverage</option>");
                Output.WriteLine("                <option value=\"TY\">Type</option>");
                Output.WriteLine("                <option value=\"LA\">Language</option>");
                Output.WriteLine("                <option value=\"PU\">Publisher</option>");
                Output.WriteLine("                <option value=\"GE\">Genre</option>");
                Output.WriteLine("                <option value=\"TA\">Target Audience</option>");
                Output.WriteLine("                <option value=\"DO\">Donor</option>");
                Output.WriteLine("                <option value=\"AT\">Attribution</option>");
                Output.WriteLine("                <option value=\"TL\">Tickler</option>");
                Output.WriteLine("                <option value=\"NO\">Notes</option>");
                Output.WriteLine("                <option value=\"ID\">Identifier</option>");
                Output.WriteLine("                <option value=\"FR\">Frequency</option>");
                Output.WriteLine("                <option value=\"TB\">Tracking Box</option>");
                Output.WriteLine("              </select>");
                Output.WriteLine("            </td>");
                Output.WriteLine("            <td>");
                Output.WriteLine("              <a onclick=\"internal_search('" + currentMode.Base_URL + "contains')\"><img src=\"" + currentMode.Base_URL + "default/images/go_gray.gif\" title=\"Perform search\" alt=\"Perform search\" style=\"margin-top: 1px\" /></a>");
                Output.WriteLine("              &nbsp;");
                Output.WriteLine("            </td>");
                Output.WriteLine("          </tr>");
                Output.WriteLine("        </table>");
                Output.WriteLine("      </td> ");
        }

        #region Method to add the banner HTML

        /// <summary> Flag indicates if a banner should be included </summary>
        /// <remarks> This defaults to TRUE, but can be overriden by the different html subwriters </remarks>
        public virtual bool Include_Banner
        {
            get
            {
                return true;
            }
        }

        /// <summary> A banner which should be used, rather than the default banner </summary>
        /// <remarks> This defaults to the empty strnig, but can be overriden by the different html subwriters </remarks>
        public virtual string Override_Banner
        {
            get { return String.Empty; }
        }


        #endregion
    }
}
