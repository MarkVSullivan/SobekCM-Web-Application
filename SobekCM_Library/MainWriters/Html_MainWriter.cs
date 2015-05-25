#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Email;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MySobekViewer;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.MainWriters
{
    /// <summary> Main writer writes the HTML response to a user's request </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractMainWriter"/>. </remarks>
    public class Html_MainWriter : abstractMainWriter
    {
        // Special HTML sub-writers that need to have some persistance between methods
        private readonly abstractHtmlSubwriter subwriter;

	    /// <summary> Constructor for a new instance of the Html_MainWriter class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
	    public Html_MainWriter( RequestCache RequestSpecificValues ) : base( RequestSpecificValues )
	    {
            // Check the IE hack CSS is loaded
            if (HttpContext.Current.Application["NonIE_Hack_CSS"] == null) 
            {
                string css_file = HttpContext.Current.Server.MapPath("default/SobekCM_NonIE.css");
                if (File.Exists(css_file))
                {
                    try
                    {
                        StreamReader reader = new StreamReader(css_file);
                        HttpContext.Current.Application["NonIE_Hack_CSS"] = reader.ReadToEnd().Trim();
                        reader.Close();
                    }
                    catch (Exception)
                    {
                        HttpContext.Current.Application["NonIE_Hack_CSS"] = "/* ERROR READING FILE: default/SobekCM_NonIE.css */";
                        throw;
                    }
                }
                else
                {
                    HttpContext.Current.Application["NonIE_Hack_CSS"] = String.Empty;
                }
            }

		    // Handle basic events which may be fired by the internal header
            if (HttpContext.Current.Request.Form["internal_header_action"] != null)
            {
                // Pull the action value
                string internalHeaderAction = HttpContext.Current.Request.Form["internal_header_action"].Trim();

                // Was this to hide or show the header?
                if ((internalHeaderAction == "hide") || (internalHeaderAction == "show"))
                {
                    // Pull the current visibility from the session
                    bool shown = !((HttpContext.Current.Session["internal_header"] != null) && (HttpContext.Current.Session["internal_header"].ToString() == "hidden"));
                    if ((internalHeaderAction == "hide") && (shown))
                    {
                        HttpContext.Current.Session["internal_header"] = "hidden";
                        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                        return;
                    }
                    if ((internalHeaderAction == "show") && (!shown))
                    {
                        HttpContext.Current.Session["internal_header"] = "shown";
                        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                        return;
                    }
                }
            }

            try
            {

                // Create the html sub writer now
                switch (RequestSpecificValues.Current_Mode.Mode)
                {
                    case Display_Mode_Enum.Internal:
                        subwriter = new Internal_HtmlSubwriter(RequestSpecificValues);
                        break;

                    case Display_Mode_Enum.Statistics:
                        subwriter = new Statistics_HtmlSubwriter(RequestSpecificValues);
                        break;

                    case Display_Mode_Enum.Preferences:
                        subwriter = new Preferences_HtmlSubwriter(RequestSpecificValues);
                        break;

                    case Display_Mode_Enum.Error:
                        subwriter = new Error_HtmlSubwriter(false, RequestSpecificValues);
                        // Send the email now
                        if (RequestSpecificValues.Current_Mode.Caught_Exception != null)
                        {
                            if ( String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Error_Message))
                                RequestSpecificValues.Current_Mode.Error_Message = "Unknown exception caught";
                            Email_Information(RequestSpecificValues.Current_Mode.Error_Message, RequestSpecificValues.Current_Mode.Caught_Exception, RequestSpecificValues.Tracer, false);
                        }
                        break;

                    case Display_Mode_Enum.Legacy_URL:
                        subwriter = new LegacyUrl_HtmlSubwriter(RequestSpecificValues);
                        break;

                    case Display_Mode_Enum.Item_Print:
                        subwriter = new Print_Item_HtmlSubwriter( RequestSpecificValues );
                        break;

                    case Display_Mode_Enum.Contact:


                        StringBuilder builder = new StringBuilder();
                        builder.Append("\n\nSUBMISSION INFORMATION\n");
                        builder.Append("\tDate:\t\t\t\t" + DateTime.Now.ToString() + "\n");
                        string lastMode = String.Empty;
                        try
                        {
                            if (HttpContext.Current.Session["Last_Mode"] != null)
                                lastMode = HttpContext.Current.Session["Last_Mode"].ToString();
                            builder.Append("\tIP Address:\t\t\t" + HttpContext.Current.Request.UserHostAddress + "\n");
                            builder.Append("\tHost Name:\t\t\t" + HttpContext.Current.Request.UserHostName + "\n");
                            builder.Append("\tBrowser:\t\t\t" + HttpContext.Current.Request.Browser.Browser + "\n");
                            builder.Append("\tBrowser Platform:\t\t" + HttpContext.Current.Request.Browser.Platform + "\n");
                            builder.Append("\tBrowser Version:\t\t" + HttpContext.Current.Request.Browser.Version + "\n");
                            builder.Append("\tBrowser Language:\t\t");
                            bool first = true;
                            string[] languages = HttpContext.Current.Request.UserLanguages;
                            if (languages != null)
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

                            builder.Append("\n\nHISTORY\n");
                            if (HttpContext.Current.Session["LastSearch"] != null)
                                builder.Append("\tLast Search:\t\t" + HttpContext.Current.Session["LastSearch"] + "\n");
                            if (HttpContext.Current.Session["LastResults"] != null)
                                builder.Append("\tLast Results:\t\t" + HttpContext.Current.Session["LastResults"] + "\n");
                            if (HttpContext.Current.Session["Last_Mode"] != null)
                                builder.Append("\tLast Mode:\t\t\t" + HttpContext.Current.Session["Last_Mode"] + "\n");
                            builder.Append("\tURL:\t\t\t\t" + HttpContext.Current.Items["Original_URL"]);
                        }
                        catch
                        {

                        }
                        subwriter = new Contact_HtmlSubwriter(lastMode, builder.ToString(), RequestSpecificValues);
                        break;


                    case Display_Mode_Enum.Contact_Sent:
                        subwriter = new Contact_HtmlSubwriter(String.Empty, String.Empty, RequestSpecificValues);
                        break;

                    case Display_Mode_Enum.Simple_HTML_CMS:
                        subwriter = new Web_Content_HtmlSubwriter(RequestSpecificValues);
                        break;

                    case Display_Mode_Enum.My_Sobek:
                        subwriter = new MySobek_HtmlSubwriter(RequestSpecificValues);
                        break;

                    case Display_Mode_Enum.Administrative:
                        subwriter = new Admin_HtmlSubwriter(RequestSpecificValues);
                        break;

                    case Display_Mode_Enum.Results:
                        subwriter = new Search_Results_HtmlSubwriter(RequestSpecificValues);
                        break;

                    case Display_Mode_Enum.Public_Folder:
                        subwriter = new Public_Folder_HtmlSubwriter(RequestSpecificValues);
                        break;

                    case Display_Mode_Enum.Search:
                    case Display_Mode_Enum.Aggregation:
                        subwriter = new Aggregation_HtmlSubwriter(RequestSpecificValues);
                        break;

                    case Display_Mode_Enum.Item_Display:
                        if ((!RequestSpecificValues.Current_Mode.Invalid_Item.HasValue || !RequestSpecificValues.Current_Mode.Invalid_Item.Value ) && (RequestSpecificValues.Current_Item != null))
                        {
                            bool show_toc = false;
                            if (HttpContext.Current.Session["Show TOC"] != null)
                            {
                                Boolean.TryParse(HttpContext.Current.Session["Show TOC"].ToString(), out show_toc);
                            }

                            // Check that this item is not checked out by another user
                            bool itemCheckedOutByOtherUser = false;
                            if (RequestSpecificValues.Current_Item.Behaviors.CheckOut_Required)
                            {
                                if (!Engine_ApplicationCache_Gateway.Checked_List.Check_Out(RequestSpecificValues.Current_Item.Web.ItemID, HttpContext.Current.Request.UserHostAddress))
                                {
                                    itemCheckedOutByOtherUser = true;
                                }
                            }

                            // Check to see if this is IP restricted
                            string restriction_message = String.Empty;
                            if (RequestSpecificValues.Current_Item.Behaviors.IP_Restriction_Membership > 0)
                            {
                                if (HttpContext.Current != null)
                                {
                                    int user_mask = (int)HttpContext.Current.Session["IP_Range_Membership"];
                                    int comparison = RequestSpecificValues.Current_Item.Behaviors.IP_Restriction_Membership & user_mask;
                                    if (comparison == 0)
                                    {
                                        int restriction = RequestSpecificValues.Current_Item.Behaviors.IP_Restriction_Membership;
                                        int restriction_counter = 1;
                                        while (restriction % 2 != 1)
                                        {
                                            restriction = restriction >> 1;
                                            restriction_counter++;
                                        }
                                        if (Engine_ApplicationCache_Gateway.IP_Restrictions[restriction_counter] != null )
                                            restriction_message = Engine_ApplicationCache_Gateway.IP_Restrictions[restriction_counter].Item_Restricted_Statement;
                                        else
                                            restriction_message = "Restricted Item";
                                    }
                                }
                            }

                            // Create the item viewer writer
                            subwriter = new Item_HtmlSubwriter( show_toc, (UI_ApplicationCache_Gateway.Settings.JP2ServerUrl.Length > 0), restriction_message, RequestSpecificValues );
                            ((Item_HtmlSubwriter)subwriter).Item_Checked_Out_By_Other_User = itemCheckedOutByOtherUser;
                        }
                        else
                        {
                            // Create the invalid item html subwrite and write the HTML
                            subwriter = new Error_HtmlSubwriter(true, RequestSpecificValues);
                        }
                        break;

                }
            }
            catch (Exception ee)
            {
                // Send to the dashboard
                if ((HttpContext.Current.Request.UserHostAddress == "127.0.0.1") || (HttpContext.Current.Request.UserHostAddress == HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"]) || (HttpContext.Current.Request.Url.ToString().IndexOf("localhost") >= 0))
                {
                    RequestSpecificValues.Tracer.Add_Trace("Html_MainWriter.Constructor", "Exception caught!", Custom_Trace_Type_Enum.Error);
                    RequestSpecificValues.Tracer.Add_Trace("Html_MainWriter.Constructor", ee.Message, Custom_Trace_Type_Enum.Error);
                    RequestSpecificValues.Tracer.Add_Trace("Html_MainWriter.Constructor", ee.StackTrace, Custom_Trace_Type_Enum.Error);

                    // Wrap this into the SobekCM Exception
                    SobekCM_Traced_Exception newException = new SobekCM_Traced_Exception("Exception caught while building the mode-specific HTML Subwriter", ee, RequestSpecificValues.Tracer);

                    // Save this to the session state, and then forward to the dashboard
                    HttpContext.Current.Session["Last_Exception"] = newException;
                    HttpContext.Current.Response.Redirect("dashboard.aspx", false);
                    RequestSpecificValues.Current_Mode.Request_Completed = true;
                }
                else
                {
                    subwriter = new Error_HtmlSubwriter(false, RequestSpecificValues);
                }
            }
        }


        /// <summary> Returns a flag indicating if the current request requires the navigation form in the main ASPX
        /// application page, or whether all the html is served directly to the output stream, without the need of this form
        /// or any controls added to it </summary>
        /// <value> The return value of this varies according to the current request </value>
        public override bool Include_Navigation_Form
        {
            get
            {
                switch (RequestSpecificValues.Current_Mode.Mode)
                {
                    case Display_Mode_Enum.Item_Print:
                    case Display_Mode_Enum.Internal:
                    case Display_Mode_Enum.Statistics:
                    case Display_Mode_Enum.Preferences:
                    case Display_Mode_Enum.Search:
                    case Display_Mode_Enum.Contact_Sent:
                    case Display_Mode_Enum.Error:
                    case Display_Mode_Enum.Legacy_URL:
                        return false;

                    case Display_Mode_Enum.Simple_HTML_CMS:
                        return RequestSpecificValues.Site_Map != null;

                    case Display_Mode_Enum.Aggregation:
		                if ((RequestSpecificValues.Current_Mode.Aggregation_Type == Aggregation_Type_Enum.Home) || (RequestSpecificValues.Current_Mode.Aggregation_Type == Aggregation_Type_Enum.Home_Edit))
		                {
                            return false;
		                }
		                return true;

	                default: 
                        return true;
                }
            }
        }


        /// <summary> Returns a flag indicating whether the additional table of contents place holder ( &quot;tocPlaceHolder&quot; ) in the itemNavForm form will be utilized 
        /// for the current request, or if it can be hidden. </summary>
        /// <value> This property always returns TRUE for the Html_MainWriter </value>
        public override bool Include_TOC_Place_Holder
        {
            get
            {
                return true;
            }
        }

        /// <summary> Returns a flag indicating whether the additional place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form will be utilized 
        /// for the current request, or if it can be hidden. </summary>
        /// <value> This property always returns TRUE for the Html_MainWriter </value>
        public override bool Include_Main_Place_Holder
        {
            get
            {
                return true;
            }
        }

		/// <summary> Returns a flag indicating whether the file upload specific holder in the itemNavForm form will be utilized 
		/// for the current request, or if it can be hidden. </summary>
		/// <value> This value can be override by child classes, but by default this returns FALSE </value>
		public override bool File_Upload_Possible
		{
			get
			{
				return subwriter != null && subwriter.Upload_File_Possible;
			}
		}

        /// <summary> Gets the enumeration of the type of main writer </summary>
        /// <value> This property always returns the enumerational value <see cref="SobekCM.UI_Library.Navigation.Writer_Type_Enum.HTML"/>. </value>
        public override Writer_Type_Enum Writer_Type { get { return Writer_Type_Enum.HTML; } }

        /// <summary> Perform all the work of adding to the response stream back to the web user </summary>
        /// <param name="TOC_Place_Holder"> Place holder is used to add more complex server-side objects during execution</param>
        /// <param name="Main_Place_Holder"> Place holder is used to add more complex server-side objects during execution</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Since this class writes all the output directly to the response stream, this method simply returns, without doing anything</remarks>
        public override void Add_Controls( PlaceHolder TOC_Place_Holder, PlaceHolder Main_Place_Holder, Custom_Tracer Tracer)
        {
            // If execution should end, do it now
            if (RequestSpecificValues.Current_Mode.Request_Completed)
                return;

            Tracer.Add_Trace("Html_MainWriter.Add_Controls", "Adding any necessary controls to the placeholders on the page");

            // Render HTML and add controls depending on the current mode
            switch (RequestSpecificValues.Current_Mode.Mode)
            {
                #region Start adding HTML and controls for SIMPLE WEB CONTENT TEXT mode

                case Display_Mode_Enum.Simple_HTML_CMS:
                    // Add any necessary controls
                    if (RequestSpecificValues.Site_Map != null)
                    {
                        ((Web_Content_HtmlSubwriter)subwriter).Add_Controls(Main_Place_Holder, Tracer);
                    }

                    break;

                #endregion

                #region Start adding HTML and controls for MY SOBEK mode

                case Display_Mode_Enum.My_Sobek:

					MySobek_HtmlSubwriter mySobekWriter = subwriter as MySobek_HtmlSubwriter;
					if (mySobekWriter != null)
                    {
                        // Add any necessary controls
                        mySobekWriter.Add_Controls(Main_Place_Holder, Tracer);
                    }
                    break;

                #endregion

                #region Start adding HTML and controls for ADMINISTRATIVE mode

                case Display_Mode_Enum.Administrative:
		            Admin_HtmlSubwriter adminWriter = subwriter as Admin_HtmlSubwriter;
					if (adminWriter != null )
                    {
                        bool add_footer = false;
						// If the my sobek writer contains pop up forms, add the header here first
						if ((adminWriter.Contains_Popup_Forms) && ( !adminWriter.Subwriter_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.MySobek_Subwriter_Mimic_Item_Subwriter)))
						{
                            add_footer = true;

							StringBuilder header_builder = new StringBuilder("Html_Mainwriter:Line415");
							StringWriter header_writer = new StringWriter(header_builder);
							Display_Header(header_writer, Tracer);
							LiteralControl header_literal = new LiteralControl(header_builder.ToString());
							Main_Place_Holder.Controls.Add(header_literal);
						}

                        // Add any necessary controls
                        adminWriter.Add_Controls(Main_Place_Holder, Tracer);

						// Finally, add the footer
                        if (add_footer)
						{
							StringBuilder footer_builder = new StringBuilder();
							StringWriter footer_writer = new StringWriter(footer_builder);
							Display_Footer(footer_writer, Tracer);
							LiteralControl footer_literal = new LiteralControl(footer_builder.ToString());
							Main_Place_Holder.Controls.Add(footer_literal);
						}
                    }

                    break;

                #endregion

                #region Start adding HTML and add controls for RESULTS mode

                case Display_Mode_Enum.Results:
		            Search_Results_HtmlSubwriter searchResultsSub = subwriter as Search_Results_HtmlSubwriter;
					if (searchResultsSub != null )
                    {
                        // Make sure the corresponding 'search' is the latest
                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Search;
                        HttpContext.Current.Session["LastSearch"] = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Results;

                        // Add the controls 
						searchResultsSub.Add_Controls(Main_Place_Holder, Tracer, null);
                    }

                    break;

                #endregion

                #region Add HTML and controls for PUBLIC FOLDER mode

                case Display_Mode_Enum.Public_Folder:
		            Public_Folder_HtmlSubwriter publicFolderSub = subwriter as Public_Folder_HtmlSubwriter;
					if (publicFolderSub != null )
                    {
                        // Also try to add any controls
						publicFolderSub.Add_Controls(Main_Place_Holder, Tracer, null);
                    }
                    break;

                #endregion

                #region Add HTML and controls for COLLECTION VIEWS

                case Display_Mode_Enum.Search:
                case Display_Mode_Enum.Aggregation:
                    Aggregation_HtmlSubwriter aggregationSub = subwriter as Aggregation_HtmlSubwriter;
                    if (aggregationSub != null)
                    {
                        // Also try to add any controls
                        aggregationSub.Add_Controls(Main_Place_Holder, Tracer);
                    }
                    break;

                #endregion

                #region Start adding HTML and add controls for ITEM DISPLAY mode

                case Display_Mode_Enum.Item_Display:
		            Item_HtmlSubwriter itemWriter = subwriter as Item_HtmlSubwriter;
					if (itemWriter != null )
                    {
                        // Add the TOC section
                        Tracer.Add_Trace("Html_MainWriter.Add_Controls", "Allowing item viewer to add table of contents to <i>tocPlaceHolder</i>");
                        itemWriter.Add_Standard_TOC(TOC_Place_Holder, Tracer);

                        // Add the main viewer section
                        itemWriter.Add_Main_Viewer_Section(Main_Place_Holder, Tracer);
                    }
                    break;

                #endregion

                default:
                    Tracer.Add_Trace("Html_MainWriter.Add_Html_And_Controls", "No controls or html added to page");
                    break;
            }
        }

        
        /// <summary> Gets the title to use for this web page, based on the current request mode </summary>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Title to use in the HTML result document </returns>
        public string Get_Page_Title(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Html_MainWriter.Get_Page_Title", "Getting page title");
            }

            string thisTitle = null;
            if (subwriter != null)
                thisTitle = subwriter.WebPage_Title;
            if ( String.IsNullOrEmpty(thisTitle))
                thisTitle = "{0}";

            return String.Format(thisTitle, RequestSpecificValues.Current_Mode.Instance_Abbreviation);
        }

        /// <summary> Writes the style references and other data to the HEAD portion of the web page </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_MainWriter.Add_Style_References", "Adding style references and apple touch icon to HTML");

            // A couple extraordinary cases
            switch (RequestSpecificValues.Current_Mode.Mode)
            {
                case Display_Mode_Enum.Reset:
                case Display_Mode_Enum.Item_Cache_Reload:
                case Display_Mode_Enum.None:
                    Output.WriteLine("  <meta name=\"robots\" content=\"noindex, nofollow\" />");
                    break;
            }

            // Write the style sheet to use 
            Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");

			// Always add jQuery library (changed as of 7/8/2013)
            if ((RequestSpecificValues.Current_Mode.Mode != Display_Mode_Enum.Item_Display) || (RequestSpecificValues.Current_Mode.ViewerCode != "pageturner"))
            {
                Output.WriteLine("  <script type=\"text/javascript\" src=\"" + Static_Resources.Jquery_1_10_2_Js + "\"></script>");
				Output.WriteLine("  <script type=\"text/javascript\" src=\"" + Static_Resources.Sobekcm_Full_Js + "\"></script>");
			}

			// Special code for the menus, if this is not IE
			if (HttpContext.Current.Request.Browser.Browser.IndexOf("IE",StringComparison.OrdinalIgnoreCase) < 0 )
			{
				string non_ie_hack = HttpContext.Current.Application["NonIE_Hack_CSS"] as string;
				if (!String.IsNullOrEmpty(non_ie_hack))
				{
					Output.WriteLine("  <style type=\"text/css\">");
					Output.WriteLine("    " + non_ie_hack);
					Output.WriteLine("  </style>");
				}
			}
			else
			{
				Output.WriteLine("  <!--[if lt IE 9]>");
				Output.WriteLine("    <script src=\"" + Static_Resources.Html5shiv_Js + "\"></script>");
				Output.WriteLine("  <![endif]-->");
			}

            // Add the special code for the html subwriter
            if (subwriter != null)
                subwriter.Write_Within_HTML_Head(Output, RequestSpecificValues.Tracer);

            // Include the interface's style sheet if it has one
            if ((RequestSpecificValues.HTML_Skin != null) && (RequestSpecificValues.HTML_Skin.CSS_Style.Length > 0))
            {
                Output.WriteLine("  <link href=\"" + RequestSpecificValues.Current_Mode.Base_URL + RequestSpecificValues.HTML_Skin.CSS_Style + "\" rel=\"stylesheet\" type=\"text/css\" />");
            }

			// Finally add the aggregation-level CSS if it exists
            if (((RequestSpecificValues.Current_Mode.Mode == Display_Mode_Enum.Aggregation) || (RequestSpecificValues.Current_Mode.Mode == Display_Mode_Enum.Search) || (RequestSpecificValues.Current_Mode.Mode == Display_Mode_Enum.Results)) && (RequestSpecificValues.Hierarchy_Object != null) && ( !String.IsNullOrEmpty(RequestSpecificValues.Hierarchy_Object.CSS_File)))
			{
                Output.WriteLine("  <link href=\"" + RequestSpecificValues.Current_Mode.Base_Design_URL + "aggregations/" + RequestSpecificValues.Hierarchy_Object.Code + "/" + RequestSpecificValues.Hierarchy_Object.CSS_File + "\" rel=\"stylesheet\" type=\"text/css\" />");
			}

            // Add a printer friendly CSS
            Output.WriteLine("  <link rel=\"stylesheet\" href=\"" + Static_Resources.Print_Css + "\" type=\"text/css\" media=\"print\" /> ");

            // Add the apple touch icon
            Output.WriteLine("  <link rel=\"apple-touch-icon\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "design/skins/" + RequestSpecificValues.Current_Mode.Skin + "/iphone-icon.png\" />");
        }


        /// <summary> Gets the body attributes to include within the BODY tag of the main HTML response document </summary>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Body attributes to include in the BODY tag </returns>
        public string Get_Body_Attributes(Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_MainWriter.Get_Body_Attributes", "Adding body attributes to HTML");

            // Get the attributes which should be included by the html sub writer
            List<Tuple<string, string>> bodyAttributes = subwriter.Body_Attributes;

            // Handles special case where a message should be displayed to the user
            // from a previous action
            if (!RequestSpecificValues.Current_Mode.isPostBack)
            {
                if ((HttpContext.Current.Session["ON_LOAD_MESSAGE"] != null) || (HttpContext.Current.Session["ON_LOAD_WINDOW"] != null))
                {
                    // ENsure the body attributes list is not null
                    if (bodyAttributes == null)
                        bodyAttributes = new List<Tuple<string, string>>();

                    // Handle the previously saved actions
                    if (HttpContext.Current.Session["ON_LOAD_MESSAGE"] != null)
                    {
                        string on_load_message = HttpContext.Current.Session["ON_LOAD_MESSAGE"].ToString();
                        if (on_load_message.Length > 0)
                            bodyAttributes.Add(new Tuple<string, string>("onload", "alert('" + on_load_message + "');"));
                        HttpContext.Current.Session.Remove("ON_LOAD_MESSAGE");
                    }
                    if (HttpContext.Current.Session["ON_LOAD_WINDOW"] != null)
                    {
                        string on_load_window = HttpContext.Current.Session["ON_LOAD_WINDOW"].ToString();
                        if (on_load_window.Length > 0)
                            bodyAttributes.Add(new Tuple<string, string>("onload", "window.open('" + on_load_window + "', 'new_" + DateTime.Now.Millisecond + "');"));
                        HttpContext.Current.Session.Remove("ON_LOAD_WINDOW");
                    }
                }
            }

            // If there is nothing to add, return now
            if ((bodyAttributes == null) || (bodyAttributes.Count == 0))
                return String.Empty;

            // Create the string for the body attributes
            Dictionary<string, string> collapsedAttributes = new Dictionary<string, string>();
            foreach (Tuple<string, string> thisAttr in bodyAttributes)
            {
                if (collapsedAttributes.ContainsKey(thisAttr.Item1))
                    collapsedAttributes[thisAttr.Item1] = collapsedAttributes[thisAttr.Item1] + thisAttr.Item2;
                else
                    collapsedAttributes.Add(thisAttr.Item1, thisAttr.Item2);
            }

            // Now, build and return the string
            StringBuilder builder = new StringBuilder(" ");
            foreach (string thisKey in collapsedAttributes.Keys)
            {
                builder.Append(thisKey + "=\"" + collapsedAttributes[thisKey] + "\" ");
            }

            return builder.ToString();
        }

        /// <summary> Perform all the work of adding text directly to the response stream back to the web user </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Html(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_MainWriter.Write_Html", String.Empty);

            // If the subwriter is null, this is an ERROR, but do nothing for now
            if (subwriter == null) return;

            // Start with the basic html at the beginning of the page
            Display_Header(Output, Tracer);

            try
            {
                subwriter.Write_HTML(Output, Tracer);
            }
            catch (Exception ee)
            {
                Email_Information("Error caught in Html_MainWriter", ee, Tracer, true);
                throw new SobekCM_Traced_Exception("Error caught in Html_MainWriter.Write_Html", ee, Tracer);
            }
        }

        /// <summary> Writes the html to the output stream open the itemNavForm, which appears just before the TocPlaceHolder </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
        {
            if (subwriter == null) return;
            Tracer.Add_Trace("Html_MainWriter.Write_Additional_HTML", "Allowing html subwriter to write to the page");

            subwriter.Write_ItemNavForm_Opening(Output, Tracer);
        }

        /// <summary> Writes additional HTML to the output stream just before the main place holder but after the TocPlaceHolder in the itemNavForm form.  </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Write_Additional_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (subwriter == null) return;
            Tracer.Add_Trace("Html_MainWriter.Write_Additional_HTML", "Allowing html subwriter to write to the page");
 
            subwriter.Write_Additional_HTML(Output, Tracer);
        }

        /// <summary> Writes final HTML to the output stream after all the placeholders and just before the itemNavForm is closed.  </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            if (subwriter == null) return;
            Tracer.Add_Trace("Html_MainWriter.Write_Additional_HTML", "Allowing html subwriter to write to the page");

            subwriter.Write_ItemNavForm_Closing(Output, Tracer);
        }

        /// <summary> Writes any final HTML needed after the main place holder directly to the output stream</summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
			if ((RequestSpecificValues.Current_Mode.isPostBack) && (RequestSpecificValues.Current_Mode.Mode != Display_Mode_Enum.My_Sobek) && (RequestSpecificValues.Current_Mode.Mode != Display_Mode_Enum.Administrative)) return;
            if (subwriter == null) return;

            Tracer.Add_Trace("Html_MainWriter.Write_Final_HTML", String.Empty);

            // Allow the html subwriter to write some final HTML
            subwriter.Write_Final_HTML(Output, Tracer);

            // Add the footer if necessary
            if (!subwriter.Subwriter_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Suppress_Footer))
            {
                Display_Footer(Output, Tracer);
            }
        }

        #region Protected internal methods to write the header and footer to the stream

        /// <summary> Writes the header directly to the output stream writer </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        protected internal void Display_Header(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_MainWriter.Display_Header", "Adding header to HTML");

			// If the subwriter is NULL, do nothing (but sure seems like an error!)
	        if (subwriter == null)
		        return;

			// Get the list of behaviors here
	        List<HtmlSubwriter_Behaviors_Enum> behaviors = subwriter.Subwriter_Behaviors;

            // Include a skip to main content?
            if (behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Include_Skip_To_Main_Content_Link))
            {
                Output.WriteLine("<nav id=\"skip-to-main-content\" role=\"navigation\" aria-label=\"Skin to main content\">");
                Output.WriteLine("  <a href=\"#main-content\" class=\"hidden-element\">Skip to main content</a>");
                Output.WriteLine("</nav>");
            }

            //// If no header should be added, just return
            //if (behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Suppress_Header))
            //    return;

            // Should the internal header be added?
            if ((subwriter != null) && (RequestSpecificValues.Current_Mode.Mode != Display_Mode_Enum.My_Sobek) && (RequestSpecificValues.Current_Mode.Mode != Display_Mode_Enum.Administrative) && (RequestSpecificValues.Current_User != null))
            {

                bool displayHeader = false;
                if (( RequestSpecificValues.Current_User.Is_Internal_User ) || ( RequestSpecificValues.Current_User.Is_System_Admin ))
                {
                    displayHeader = true;
                }
                else
                {
                    switch ( RequestSpecificValues.Current_Mode.Mode )
                    {
                        case Display_Mode_Enum.Item_Display:
                            if (RequestSpecificValues.Current_User.Can_Edit_This_Item(RequestSpecificValues.Current_Item.BibID, RequestSpecificValues.Current_Item.Bib_Info.SobekCM_Type_String, RequestSpecificValues.Current_Item.Bib_Info.Source.Code, RequestSpecificValues.Current_Item.Bib_Info.HoldingCode, RequestSpecificValues.Current_Item.Behaviors.Aggregation_Code_List))
                                displayHeader = true;
                            break;

                        case Display_Mode_Enum.Aggregation:
                        case Display_Mode_Enum.Results:
                        case Display_Mode_Enum.Search:
                            if (( RequestSpecificValues.Current_User.Is_Aggregation_Curator( RequestSpecificValues.Current_Mode.Aggregation )) || ( RequestSpecificValues.Current_User.Can_Edit_All_Items( RequestSpecificValues.Current_Mode.Aggregation )))
                            {
                                displayHeader = true;
                            }
                            break;
                    }
                }
                
                if (( displayHeader ) && ( !behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Suppress_Internal_Header)))
                {
                    string return_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                    if ((HttpContext.Current != null) && (HttpContext.Current.Session["Original_URL"] != null))
                        return_url = HttpContext.Current.Session["Original_URL"].ToString();

                    Output.WriteLine("<!-- Start the internal header -->");
                    Output.WriteLine("<form name=\"internalHeaderForm\" method=\"post\" action=\"" + return_url + "\" id=\"internalHeaderForm\"> ");
                    Output.WriteLine();
                    Output.WriteLine("<!-- Hidden field is used for postbacks to add new form elements (i.e., new name, new other titles, etc..) -->");
                    Output.WriteLine("<input type=\"hidden\" id=\"internal_header_action\" name=\"internal_header_action\" value=\"\" />");
                    Output.WriteLine();

                    // Is the header currently hidden?
                    if (HttpContext.Current != null && ((HttpContext.Current.Session["internal_header"] != null) && (HttpContext.Current.Session["internal_header"].ToString() == "hidden")))
                    {
                        Output.WriteLine("  <table cellspacing=\"0\" id=\"internalheader\">");
                        Output.WriteLine("    <tr>");
                        Output.WriteLine("      <td align=\"left\">");
                        Output.WriteLine("        <button title=\"Show Internal Header\" class=\"intheader_button_aggr show_intheader_button_aggr\" onclick=\"return show_internal_header();\"></button>");
                        Output.WriteLine("      </td>");
                        Output.WriteLine("    </tr>");
                        Output.WriteLine("  </table>"); 
                    }
                    else
                    {
                        subwriter.Write_Internal_Header_HTML(Output, RequestSpecificValues.Current_User);
                    }

                    Output.WriteLine("</form>");
                    Output.WriteLine("<!-- End the internal header -->");
                    Output.WriteLine();
                }
            }

            if (!behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Suppress_Header))
                HeaderFooter_Helper_HtmlSubWriter.Add_Header(Output, RequestSpecificValues, subwriter.Container_CssClass, Get_Page_Title(null),  behaviors);

            Output.WriteLine(String.Empty);
        }

        /// <summary> Writes the footer directly to the output stream writer provided </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        protected internal void Display_Footer(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_MainWriter.Display_Footer", "Adding footer to HTML");

			// If the subwriter is NULL, do nothing (but sure seems like an error!)
			if (subwriter == null)
				return;

			// Get the list of behaviors here
			List<HtmlSubwriter_Behaviors_Enum> behaviors = subwriter.Subwriter_Behaviors;

			// If no header should be added, just return
			if (behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Suppress_Footer))
				return;

            HeaderFooter_Helper_HtmlSubWriter.Add_Footer(Output, RequestSpecificValues, behaviors);

            // Add the time and trace at the end
			if ((HttpContext.Current.Request.Url.AbsoluteUri.Contains("shibboleth")) || (RequestSpecificValues.Current_Mode.Trace_Flag_Simple) || ((RequestSpecificValues.Current_User != null) && (RequestSpecificValues.Current_User.Is_System_Admin)))
            {
                Output.WriteLine("<style type=\"text/css\">");
                Output.WriteLine("table.Traceroute { border-width: 2px; border-style: solid; border-color: gray; border-collapse: collapse; background-color: white; font-size: small; }");
                Output.WriteLine("table.Traceroute th { border-width: 2px; padding: 3px; border-style: solid; border-color: gray; background-color: gray; color: white; }");
                Output.WriteLine("table.Traceroute td { border-width: 2px; padding: 3px; border-style: solid; border-color: gray;	background-color: white; }");
                Output.WriteLine("</style>");
                Output.WriteLine("<a href=\"\" onclick=\"return show_trace_route()\" id=\"sbkHmw_TraceRouterShowLink\">show trace route (sys admin)</a>");
                Output.WriteLine("<div id=\"sbkHmw_TraceRouter\" style=\"display:none;\">");

                Output.WriteLine("<br /><br /><b>URL REWRITE</b>");
                if (HttpContext.Current.Items["Original_URL"] == null)
                    Output.WriteLine("<br /><br />Original URL: <i>None found</i><br />");
                else
                    Output.WriteLine("<br /><br />Original URL: " + HttpContext.Current.Items["Original_URL"] + "<br />");

                Output.WriteLine("Current URL: " + HttpContext.Current.Request.Url + "<br />");


                Output.WriteLine("<br /><br /><b>TRACE ROUTE</b>");
                Output.WriteLine("<br /><br />Total Execution Time: " + Tracer.Milliseconds + " Milliseconds<br /><br />");
                Output.WriteLine(Tracer.Complete_Trace + "<br />");
				Output.WriteLine("</div>");
            }
        }

        #endregion

        #region Method to email information during an error

        private static void Email_Information(string EmailTitle, Exception ObjErr, Custom_Tracer Tracer, bool Redirect )
        {
            // Is ther an error email address in the configuration?
            if (UI_ApplicationCache_Gateway.Settings.System_Error_Email.Length > 0)
            {
                try
                {
                    // Build the error message
                    string err;
                    if (ObjErr != null)
                    {
                        if (ObjErr.InnerException != null)
                        {
                            err = "<b>" + HttpContext.Current.Request.UserHostAddress + "</b><br /><br />" +
                                  "Error in!!: " + HttpContext.Current.Items["Original_URL"] + "<br /><br />" +
                                  "Error Message: " + ObjErr.Message + "<br /><br />" +
                                  "Inner Exception: " + ObjErr.InnerException.Message + "<br /><br />" +
                                  "Stack Trace: " + ObjErr.InnerException.StackTrace + "<br /><br />";
                        }
                        else
                        {
                            err = "<b>" + HttpContext.Current.Request.UserHostAddress + "</b><br /><br />" +
                                  "Error in!!: " + HttpContext.Current.Items["Original_URL"] + "<br /><br />" +
                                  "Error Message: " + ObjErr.Message + "<br /><br />" +
                                  "Stack Trace: " + ObjErr.StackTrace + "<br /><br />";

                        }

                        if (ObjErr.Message.IndexOf("Timeout expired") >= 0)
                            EmailTitle = "Database Timeout Expired";
                    }
                    else
                    {
                        err = "<b>" + HttpContext.Current.Request.UserHostAddress + "</b><br /><br />";
                    }

                    // Email the error message
                    if (Tracer != null)
                    {
                        Email_Helper.SendEmail(UI_ApplicationCache_Gateway.Settings.System_Error_Email, EmailTitle, err + "<br /><br />" + Tracer.Text_Trace, true, String.Empty);
                    }
                    else
                    {
                        Email_Helper.SendEmail(UI_ApplicationCache_Gateway.Settings.System_Error_Email, EmailTitle, err, true, String.Empty);
                    }
                }
                catch (Exception)
                {
                    // Failed to send the email.. but not much else to do here really
                }
            }

            try
            {
                StreamWriter writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\temp\\exceptions.txt", true);
                writer.WriteLine();
                writer.WriteLine("Error Caught in Application_Error event ( " + DateTime.Now.ToString() + ")");
                writer.WriteLine("User Host Address: " + HttpContext.Current.Request.UserHostAddress);
                writer.WriteLine("Requested URL: " + HttpContext.Current.Request.Url);
                if (ObjErr is SobekCM_Traced_Exception)
                {
                    SobekCM_Traced_Exception sobekException = (SobekCM_Traced_Exception)ObjErr;
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
                // Nothing else to do here.. no other known way to log this error
            }

            // Forward to our error message
            if (Redirect)
            {
                HttpContext.Current.Response.Redirect(UI_ApplicationCache_Gateway.Settings.System_Error_URL, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }

        #endregion
    }
}
 