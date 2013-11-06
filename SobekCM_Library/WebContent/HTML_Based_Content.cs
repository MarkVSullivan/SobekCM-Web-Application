#region Using directives

using System;
using System.IO;
using System.Web;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Configuration;

#endregion

namespace SobekCM.Library.WebContent
{
    /// <summary> Base abstract class is used for all html content-based browse, info, or
    /// simple CMS-style web content objects.  These are objects which are (possibly) read from
    /// a static HTML file and much of the head information must be maintained </summary>
    [Serializable]
    public class HTML_Based_Content
    {
        private string author;
        private string banner;
        private string code;
        private string date;
        private string description;
        private string extraHeadInfo;
        private string keywords;
        private string sitemap;
        private string staticText;
        private string thumbnail;
        private string title;
        private string webskin;

        #region Constructors

        /// <summary> Constructor for a new instance of the base_HTML_Content_Object class  </summary>
        public HTML_Based_Content()
        {
            // Set the necessary values to empty initially
            code = String.Empty;
            banner = String.Empty;
        }

        /// <summary> Constructor for a new instance of the base_HTML_Content_Object class  </summary>
        /// <param name="Code"> SobekCM code for this html content-based object </param>
        public HTML_Based_Content(string Code)
        {
            // Set the necessary values to empty initially
            banner = String.Empty;

            // Save the parameter
            code = Code;
        }

        /// <summary> Constructor for a new instance of the base_HTML_Content_Object class  </summary>
        /// <param name="Text"> Static text to use for this item </param>
        /// <param name="Title"> Title to display with this item </param>
        /// <remarks> This constructor is mostly used with passing back errors to be displayed. </remarks>
        public HTML_Based_Content(string Text, string Title)
        {
            // Set the necessary values to empty initially
            code = String.Empty;
            banner = String.Empty;

            // Save the parameters
            staticText = Text;
            title = Title;
        }

        #endregion

        #region Public methods

        /// <summary> Code for this info or browse page </summary>
        /// <remarks> This is the code that is used in the URL to specify this info or browse page </remarks>
        public string Code
        {
            get { return code ?? String.Empty; }
            set { code = value.ToLower(); }
        }

        ///// <summary> Banner to display whenever this browse or information page is displayed, which overrides the standard collection banner </summary>
        /// <remarks> If this is [DEFAULT] then the default collection banner will be displayed, otherwise no banner should be shown</remarks>
        public string Banner
        {
            get { return banner ?? String.Empty; }
            set { banner = value; }
        }

        /// <summary> Extra information which was read from the head value and should be included in the final html rendering </summary>
        public string Extra_Head_Info
        {
            get { return extraHeadInfo ?? String.Empty; }
            set { extraHeadInfo = value; }
        }

        /// <summary> Site maps indicate a sitemap XML file to use to render the left navigation bar's tree view (MSDN-style) table of contents </summary>
        public string SiteMap
        {
            get { return sitemap ?? String.Empty; }
            set { sitemap = value; }
        }

        /// <summary> Web skin in the head overrides any other web skin which may be present in the URL </summary>
        public string Web_Skin
        {
            get { return webskin ?? String.Empty; }
            set { webskin = value; }
        }

        /// <summary> Thumbnail is used when displaying this info/browse page as a search result </summary>
        public string Thumbnail
        {
            get { return thumbnail ?? String.Empty; }
            set { thumbnail = value; }
        }

        /// <summary> Title  is used when displaying this info/browse page as a search result </summary>
        public string Title
        {
            get { return title ?? String.Empty; }
            set { title = value; }
        }

        /// <summary> Author is used when displaying this info/browse page as a search result </summary>
        public string Author
        {
            get { return author ?? String.Empty; }
            set { author = value; }
        }

        /// <summary> Date is used when displaying this info/browse page as a search result </summary>
        public string Date
        {
            get { return date ?? String.Empty; }
            set { date = value; }
        }

        /// <summary> Keywords are used when displaying this info/browse page as a search result </summary>
        public string Keywords
        {
            get { return keywords ?? String.Empty; }
            set { keywords = value; }
        }

        /// <summary> Description is used when displaying this info/browse page as a search result </summary>
        public string Description
        {
            get { return description ?? String.Empty; }
            set { description = value; }
        }

        /// <summary> Static text included as the body of the static HTML file </summary>
        public string Static_Text
        {
            get { return staticText ?? String.Empty;  }
            set { staticText = value; }
        }

        #endregion

        #region Method to take the static text, do standard replacements, and return the user-customized text

        /// <summary> Apply the individual user settings to this static text, replacing all directives with the actual data from this HTTP request </summary>
        /// <param name="Display_Text"> Display text to apply the individual request's settings to </param>
        /// <param name="Aggregation"> Current item aggregation, used for any custom directives which may exist </param>
        /// <param name="Web_Skin_Code"> Code for the current web skin</param>
        /// <param name="Base_Skin_Code"> Code for the base web skin from which the current web skin inherits</param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        /// <param name="URL_Options"> Current URL Options </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Static text for this info/browse with substitutions made to support the current request </returns>
        /// <remarks> This actually reads the HTML file each time this is requested </remarks>
        public string Apply_Settings_To_Static_Text(string Display_Text, Item_Aggregation Aggregation, string Web_Skin_Code, string Base_Skin_Code, string Base_URL, string URL_Options, Custom_Tracer Tracer)
        {
            // Replace any item aggregation specific custom directives
            if (Aggregation != null)
            {
                foreach (string thisKey in Aggregation.Custom_Directives.Keys)
                {
                    if (Display_Text.IndexOf(thisKey) > 0)
                    {
                        Display_Text = Display_Text.Replace(thisKey, Aggregation.Custom_Directives[thisKey].Replacement_HTML);
                    }
                }
            }

            string urlOptions1 = String.Empty;
            string urlOptions2 = String.Empty;
            if (URL_Options.Length > 0)
            {
                urlOptions1 = "?" + URL_Options;
                urlOptions2 = "&" + URL_Options;
            }

            // Replace the standard directives next
            Display_Text = Display_Text.Replace("<%URLOPTS%>", URL_Options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2).Replace("<%INTERFACE%>", Web_Skin_Code).Replace("<%WEBSKIN%>", Web_Skin_Code).Replace("<%BASEURL%>", Base_URL);

            // Replace some additional (more complex) values
            string tabstart = "<img src=\"" + Base_URL + "design/skins/" + Base_Skin_Code + "/tabs/cLD.gif\" border=\"0\" class=\"tab_image\" /><span class=\"tab\">";
            string tabend = "</span><img src=\"" + Base_URL + "design/skins/" + Base_Skin_Code + "/tabs/cRD.gif\" border=\"0\" class=\"tab_image\" />";
            string select_tabstart = "<img src=\"" + Base_URL + "design/skins/" + Base_Skin_Code + "/tabs/cLD_s.gif\" border=\"0\" class=\"tab_image\" /><span class=\"tab_s\">";
            string select_tabend = "</span><img src=\"" + Base_URL + "design/skins/" + Base_Skin_Code + "/tabs/cRD_s.gif\" border=\"0\" class=\"tab_image\" />";
            Display_Text = Display_Text.Replace("<%TABSTART%>", tabstart).Replace("<%TABEND%>", tabend).Replace("<%SELECTED_TABSTART%>", select_tabstart).Replace("<%SELECTED_TABEND%>", select_tabend);

            if (Display_Text.IndexOf("<body>") >= 0)
            {
                Display_Text = Display_Text.Substring(Display_Text.IndexOf("<body>") + 6);
                Display_Text = Display_Text.Substring(0, Display_Text.IndexOf("</body>"));
            }
            return Display_Text;
        }

        #endregion

		/// <summary> Saves all this data to a file </summary>
		/// <param name="File"> Name (and path) of the file to save </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		public bool Save_To_File(string File)
		{
			try
			{
				StreamWriter writer = new StreamWriter(File, false);
				writer.WriteLine("<html>");
				writer.WriteLine("  <head>");
				if (!String.IsNullOrEmpty(title))
					writer.WriteLine("    <title>" + HttpUtility.HtmlEncode(title) + "</title>");
				if (!String.IsNullOrEmpty(author))
					writer.WriteLine("    <meta name=\"author\" content=\"" + HttpUtility.HtmlEncode(author) + "\" />");
				if (!String.IsNullOrEmpty(date))
					writer.WriteLine("    <meta name=\"date\" content=\"" + date + "\" />");
				if (!String.IsNullOrEmpty(keywords))
					writer.WriteLine("    <meta name=\"keywords\" content=\"" + HttpUtility.HtmlEncode(keywords) + "\" />");
				if (!String.IsNullOrEmpty(description))
					writer.WriteLine("    <meta name=\"description\" content=\"" + HttpUtility.HtmlEncode(description) + "\" />");
				if (!String.IsNullOrEmpty(banner))
					writer.WriteLine("    <meta name=\"banner\" content=\"" + HttpUtility.HtmlEncode(banner) + "\" />");
				if (!String.IsNullOrEmpty(thumbnail))
					writer.WriteLine("    <meta name=\"thumbnail\" content=\"" + HttpUtility.HtmlEncode(thumbnail) + "\" />");
				if (!String.IsNullOrEmpty(code))
					writer.WriteLine("    <meta name=\"code\" content=\"" + HttpUtility.HtmlEncode(code) + "\" />");
				if (!String.IsNullOrEmpty(sitemap))
					writer.WriteLine("    <meta name=\"sitemap\" content=\"" + HttpUtility.HtmlEncode(sitemap) + "\" />");
				if (!String.IsNullOrEmpty(webskin))
					writer.WriteLine("    <meta name=\"code\" content=\"" + HttpUtility.HtmlEncode(webskin) + "\" />");

				if ( !String.IsNullOrEmpty(extraHeadInfo))
					writer.WriteLine(extraHeadInfo);

				writer.WriteLine("  </head>");
				writer.WriteLine("  <body>");
				writer.WriteLine(staticText);
				writer.WriteLine("  </body>");
				writer.WriteLine("</html>");
				writer.WriteLine();
				writer.Flush();
				writer.Close();


				return true;
			}
			catch (Exception ee)
			{
				return false;
			}
		}
    }
}
