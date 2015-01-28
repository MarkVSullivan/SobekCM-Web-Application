#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Navigation;
using SobekCM.Core.Skins;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.MainWriters;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.HTML
{
	/// <summary> Abstract class which all HTML subwriters must extend.  This class contains some of the
	/// basic HTML-writing helper values and contains some of the values used by many of the subclasses.
	/// HTML subwriters are the top level writing classes employed by the <see cref="Html_MainWriter"/>. </summary>
	public abstract class abstractHtmlSubwriter
	{
        /// <summary> Protected field contains the information specific to the current request </summary>
        protected RequestCache RequestSpecificValues;

        /// <summary> Base constructor </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
	    protected abstractHtmlSubwriter(RequestCache RequestSpecificValues)
	    {
	        this.RequestSpecificValues = RequestSpecificValues;
	    }

		/// <summary> Adds the banner to the response stream from either the html web skin
		/// or from the current item aggreagtion object, depending on flags in the web skin object </summary>
		/// <param name="Output"> Stream to which to write the HTML for the banner </param>
		/// <param name="Banner_Division_Name"> Name for the wrapper division around the banner </param>
		/// <param name="Hierarchy_Object"> Current item aggregation object to display </param>
		/// <param name="HTML_Skin"> HTML Web skin which controls the overall appearance of this digital library </param>
		/// <param name="CurrentMode"> Mode / navigation information for the current request</param>
		/// <remarks> This is called by several html subwriters that otherwise tell this class to suppress writing the banner </remarks>
		public static void Add_Banner(TextWriter Output, string Banner_Division_Name, SobekCM_Navigation_Object CurrentMode, SobekCM_Skin_Object HTML_Skin, Item_Aggregation Hierarchy_Object)
		{
			Output.WriteLine("<!-- Write the main collection, interface, or institution banner -->");
			if ((HTML_Skin != null) && (HTML_Skin.Override_Banner))
			{
				Output.WriteLine(HTML_Skin.Banner_HTML);
			}
			else
			{
				string url_options = UrlWriterHelper.URL_Options(CurrentMode);
				if (url_options.Length > 0)
					url_options = "?" + url_options;

				if ((Hierarchy_Object != null) && (Hierarchy_Object.Code != "all"))
				{
                    Output.WriteLine("<div id=\"sbkAhs_BannerDiv\"><a alt=\"" + Hierarchy_Object.ShortName + "\" href=\"" + CurrentMode.Base_URL + Hierarchy_Object.Code + url_options + "\"><img id=\"mainBanner\" src=\"" + CurrentMode.Base_URL + Hierarchy_Object.Get_Banner_Image(HTML_Skin) + "\" alt=\"\" /></a></div>");
				}
				else
				{
                    if ((Hierarchy_Object != null) && (Hierarchy_Object.Get_Banner_Image(HTML_Skin).Length > 0))
					{
						Output.WriteLine("<div id=\"sbkAhs_BannerDiv\"><a href=\"" + CurrentMode.Base_URL + url_options + "\"><img id=\"mainBanner\" src=\"" + CurrentMode.Base_URL + Hierarchy_Object.Get_Banner_Image( HTML_Skin) + "\" alt=\"\" /></a></div>");
					}
					else
					{
						string skin_url = CurrentMode.Base_Design_URL + "skins/" + CurrentMode.Skin + "/";
						Output.WriteLine("<div id=\"sbkAhs_BannerDiv\"><a href=\"" + CurrentMode.Base_URL + url_options + "\"><img id=\"mainBanner\" src=\"" + skin_url + "default.jpg\" alt=\"\" /></a></div>");
					}
				}
			}
			Output.WriteLine();
		}


		/// <summary> Empty list of behaviors, returned by default </summary>
		/// <remarks> This just prevents an empty set from having to be created over and over </remarks>
		protected List<HtmlSubwriter_Behaviors_Enum> emptybehaviors = new List<HtmlSubwriter_Behaviors_Enum>();

	    /// <summary> Returns a flag indicating whether the file upload specific holder in the itemNavForm form will be utilized 
		/// for the current request, or if it can be hidden. </summary>
		/// <value> This value can be override by child classes, but by default this returns FALSE </value>
		public virtual bool Upload_File_Possible
		{
			get { return false; }
		}

		/// <summary> Write any additional values within the HTML Head of the
		/// final served page </summary>
		/// <param name="Output"> Output stream currently within the HTML head tags </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <remarks> By default this does nothing, but can be overwritten by all the individual html subwriters </remarks>
		public virtual void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
		{
			// Do nothing by default
		}

		/// <summary> Adds the internal header HTML for this specific HTML writer </summary>
		/// <param name="Output"> Stream to which to write the HTML for the internal header information </param>
		/// <param name="Current_User"> Currently logged on user, to determine specific rights </param>
		public virtual void Write_Internal_Header_HTML(TextWriter Output, User_Object Current_User)
		{
			Output.WriteLine("  <table id=\"sbk_InternalHeader\">");
			Output.WriteLine("    <tr>");
			Output.WriteLine("      <td style=\"text-align:left;\">");
			Output.WriteLine("          <button title=\"Hide Internal Header\" class=\"intheader_button_aggr hide_intheader_button_aggr\" onclick=\"return hide_internal_header();\"></button>");
			Output.WriteLine("      </td>");
			Write_Internal_Header_Search_Box(Output);
			Output.WriteLine("    </tr>");
			Output.WriteLine("  </table>");
		}

		/// <summary> Adds the internal header search box to the current output stream  </summary>
		/// <param name="Output"> Output stream to write the html for the internal header search box to </param>
		protected void Write_Internal_Header_Search_Box(TextWriter Output)
		{
			Output.WriteLine("      <td style=\"text-align:right; vertical-align:middle; width:340px;\">");
			Output.WriteLine("        <table>");
			Output.WriteLine("          <tr style=\"vertical-align:top; height: 16px;\">");
			Output.WriteLine("            <td valign=\"top\">");
			Output.Write("              <input name=\"internalSearchTextBox\" type=\"text\" id=\"internalSearchTextBox\" class=\"SobekInternalSearchBox\" value=\"\" onfocus=\"javascript:textbox_enter('internalSearchTextBox', 'SobekInternalSearchBox_focused')\" onblur=\"javascript:textbox_leave('internalSearchTextBox', 'SobekInternalSearchBox')\"");
            if (RequestSpecificValues.Current_Mode.Browser_Type.IndexOf("IE") >= 0)
                Output.WriteLine(" onkeydown=\"internalTrapKD(event, '" + RequestSpecificValues.Current_Mode.Base_URL + "contains');\" />");
			else
                Output.WriteLine(" onkeydown=\"return internalTrapKD(event, '" + RequestSpecificValues.Current_Mode.Base_URL + "contains');\" />");
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
            Output.WriteLine("              <a onclick=\"internal_search('" + RequestSpecificValues.Current_Mode.Base_URL + "contains')\"><img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/go_gray.gif\" title=\"Perform search\" alt=\"Perform search\" style=\"margin-top: 1px\" /></a>");
			Output.WriteLine("              &nbsp;");
			Output.WriteLine("            </td>");
			Output.WriteLine("          </tr>");
			Output.WriteLine("        </table>");
			Output.WriteLine("      </td> ");
		}


		/// <summary> Writes the HTML generated by this abstract html subwriter directly to the response stream </summary>
		/// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
		public abstract bool Write_HTML(TextWriter Output, Custom_Tracer Tracer);

		/// <summary> Writes the html to the output stream open the itemNavForm, which appears just before the TocPlaceHolder </summary>
		/// <param name="Output">Stream to directly write to</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public virtual void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
		{
			// Do nothing by default
		}

		/// <summary> Writes additional HTML to the output stream just before the main place holder but after the TocPlaceHolder in the itemNavForm form.  </summary>
		/// <param name="Output">Stream to directly write to</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public virtual void Write_Additional_HTML(TextWriter Output, Custom_Tracer Tracer)
		{
			// Do nothing by default
		}

		/// <summary> Writes final HTML to the output stream after all the placeholders and just before the itemNavForm is closed.  </summary>
		/// <param name="Output"> Stream to which to write the text for this main writer </param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public virtual void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
		{
			// Do nothing by default
		}

		/// <summary> Writes final HTML after all the forms </summary>
		/// <param name="Output">Stream to directly write to</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public virtual void Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer)
		{
			// Do nothing by default
		}

		/// <summary> Gets the collection of special behaviors which this subwriter
		/// requests from the main HTML writer. </summary>
		/// <remarks> By default, this returns an empty list </remarks>
		public virtual List<HtmlSubwriter_Behaviors_Enum> Subwriter_Behaviors
		{
			get { return emptybehaviors; }
		}

		/// <summary> Gets the collection of body attributes to be included 
		/// within the HTML body tag (usually to add events to the body) </summary>
		public virtual List<Tuple<string, string>> Body_Attributes
		{
			get { return null; }
		}

		/// <summary> Title for this web page </summary>
		/// <value> This value is set by each of the sub classes </value>
		public virtual string WebPage_Title
		{
			get { return "{0}"; }
		}

		/// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
		/// <value> By default, returns 'container-inner' </value>
		public virtual string Container_CssClass
		{
			get { return "container-inner"; }
		}
	}
}
