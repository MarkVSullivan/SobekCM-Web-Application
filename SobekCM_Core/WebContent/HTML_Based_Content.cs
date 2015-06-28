#region Using directives

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.Aggregations;
using SobekCM.Tools;

#endregion

namespace SobekCM.Core.WebContent
{
    /// <summary> Base abstract class is used for all html content-based browse, info, or
    /// simple CMS-style web content objects.  These are objects which are (possibly) read from
    /// a static HTML file and much of the head information must be maintained </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("webContentPage")]
    public class HTML_Based_Content
    {
        private string code;


        #region Constructors

        /// <summary> Constructor for a new instance of the base_HTML_Content_Object class  </summary>
        public HTML_Based_Content()
        {
            // Set the necessary values to empty initially
            code = String.Empty;
        }

        /// <summary> Constructor for a new instance of the base_HTML_Content_Object class  </summary>
        /// <param name="Code"> SobekCM code for this html content-based object </param>
        public HTML_Based_Content(string Code)
        {
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

            // Save the parameters
            Content = Text;
            this.Title = Title;
        }

        /// <summary> Constructor for a new instance of the base_HTML_Content_Object class  </summary>
        /// <param name="Text"> Static text to use for this item </param>
        /// <param name="Title"> Title to display with this item </param>
        /// <param name="Source"> Source file for this static web-based content object </param>
        /// <remarks> This constructor is mostly used with passing back errors to be displayed. </remarks>
        public HTML_Based_Content(string Text, string Title, string Source )
        {
            // Set the necessary values to empty initially
            code = String.Empty;

            // Save the parameters
            Content = Text;
            this.Title = Title;
            this.Source = Source;
        }

        #endregion

        #region Public properties and methods

        /// <summary> Code for this info or browse page </summary>
        /// <remarks> This is the code that is used in the URL to specify this info or browse page </remarks>
        [DataMember(EmitDefaultValue = false, Name = "code")]
        [XmlAttribute("code")]
        [ProtoMember(1)]
        public string Code
        {
            get { return code; }
            set { code = value.ToLower(); }
        }

        ///// <summary> Banner to display whenever this browse or information page is displayed, which overrides the standard collection banner </summary>
        /// <remarks> If this is [DEFAULT] then the default collection banner will be displayed, otherwise no banner should be shown</remarks>
        [DataMember(EmitDefaultValue = false, Name = "banner")]
        [XmlElement("banner")]
        [ProtoMember(2)]
        public string Banner { get; set; }

        /// <summary> Extra information which was read from the head value and should be included in the final html rendering </summary>
        [DataMember(EmitDefaultValue = false, Name = "extraHeadInfo")]
        [XmlElement("extraHeadInfo")]
        [ProtoMember(3)]
        public string Extra_Head_Info { get; set; }

        /// <summary> Site maps indicate a sitemap XML file to use to render the left navigation bar's tree view (MSDN-style) table of contents </summary>
        [DataMember(EmitDefaultValue = false, Name = "siteMap")]
        [XmlAttribute("siteMap")]
        [ProtoMember(4)]
        public string SiteMap { get; set; }

        /// <summary> Web skin in the head overrides any other web skin which may be present in the URL </summary>
        [DataMember(EmitDefaultValue = false, Name = "webSkin")]
        [XmlAttribute("webSkin")]
        [ProtoMember(5)]
        public string Web_Skin { get; set; }

        /// <summary> Thumbnail is used when displaying this info/browse page as a search result </summary>
        [DataMember(EmitDefaultValue = false, Name = "thumbnail")]
        [XmlElement("thumbnail")]
        [ProtoMember(6)]
        public string Thumbnail { get; set; }

        /// <summary> Title  is used when displaying this info/browse page as a search result </summary>
        [DataMember(EmitDefaultValue = false, Name = "title")]
        [XmlElement("title")]
        [ProtoMember(7)]
        public string Title { get; set; }

        /// <summary> Author is used when displaying this info/browse page as a search result </summary>
        [DataMember(EmitDefaultValue = false, Name = "author")]
        [XmlElement("author")]
        [ProtoMember(8)]
        public string Author { get; set; }

        /// <summary> Date is used when displaying this info/browse page as a search result </summary>
        [DataMember(EmitDefaultValue = false, Name = "date")]
        [XmlElement("date")]
        [ProtoMember(9)]
        public string Date { get; set; }

        /// <summary> Keywords are used when displaying this info/browse page as a search result </summary>
        [DataMember(EmitDefaultValue = false, Name = "keywords")]
        [XmlElement("keywords")]
        [ProtoMember(10)]
        public string Keywords { get; set; }

        /// <summary> Description is used when displaying this info/browse page as a search result </summary>
        [DataMember(EmitDefaultValue = false, Name = "description")]
        [XmlElement("description")]
        [ProtoMember(11)]
        public string Description { get; set; }

        /// <summary> Text to display in the primary display region </summary>
        [DataMember(EmitDefaultValue = false, Name = "content")]
        [XmlElement("content")]
        [ProtoMember(12)]
        public string Content { get; set; }

        /// <summary> Text to display in the primary display region </summary>
        [DataMember(EmitDefaultValue = false, Name = "includeMenu")]
        [XmlIgnore]
        [ProtoMember(13)]
        public bool? IncludeMenu { get; set; }

        /// <summary> Text to display in the primary display region </summary>
        /// <remarks> This is for the XML serialization portions </remarks>
        [IgnoreDataMember]
        [XmlAttribute("includeMenu")]
        public string IncludeMenu_AsString
        {
            get {
                return IncludeMenu.HasValue ? IncludeMenu.ToString() : null;
            }
            set
            {
                bool temp;
                if (Boolean.TryParse(value, out temp))
                    IncludeMenu = temp;
            }
        }

        /// <summary> Source for this html-based web content </summary>
        [DataMember(EmitDefaultValue = false, Name = "source")]
        [XmlAttribute("source")]
        [ProtoMember(14)]
        public string Source { get; set; }

        /// <summary> Static text included as the body of the static HTML file if item aggregation custom directives
        /// appears in the content source (otherwise NULL) </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public string ContentSource { get; set; }

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
            //// Replace any item aggregation specific custom directives
            //if ((Aggregation != null) && ( Aggregation.Custom_Directives != null ))
            //{
            //    foreach (string thisKey in Aggregation.Custom_Directives.Keys)
            //    {
            //        if (Display_Text.IndexOf(thisKey) > 0)
            //        {
            //            Display_Text = Display_Text.Replace(thisKey, Aggregation.Custom_Directives[thisKey].Replacement_HTML);
            //        }
            //    }
            //}

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
		public bool Save_To_File( string File)
		{
			try
			{
				StreamWriter writer = new StreamWriter(File, false);
				writer.WriteLine("<html>");
				writer.WriteLine("  <head>");
				if (!String.IsNullOrEmpty(Title))
					writer.WriteLine("    <title>" + HttpUtility.HtmlEncode(Title) + "</title>");
				if (!String.IsNullOrEmpty(Author))
					writer.WriteLine("    <meta name=\"author\" content=\"" + HttpUtility.HtmlEncode(Author) + "\" />");
				if (!String.IsNullOrEmpty(Date))
					writer.WriteLine("    <meta name=\"date\" content=\"" + Date + "\" />");
				if (!String.IsNullOrEmpty(Keywords))
					writer.WriteLine("    <meta name=\"keywords\" content=\"" + HttpUtility.HtmlEncode(Keywords) + "\" />");
				if (!String.IsNullOrEmpty(Description))
					writer.WriteLine("    <meta name=\"description\" content=\"" + HttpUtility.HtmlEncode(Description) + "\" />");
				if (!String.IsNullOrEmpty(Banner))
					writer.WriteLine("    <meta name=\"banner\" content=\"" + HttpUtility.HtmlEncode(Banner) + "\" />");
				if (!String.IsNullOrEmpty(Thumbnail))
					writer.WriteLine("    <meta name=\"thumbnail\" content=\"" + HttpUtility.HtmlEncode(Thumbnail) + "\" />");
				if (!String.IsNullOrEmpty(code))
					writer.WriteLine("    <meta name=\"code\" content=\"" + HttpUtility.HtmlEncode(code) + "\" />");
				if (!String.IsNullOrEmpty(SiteMap))
					writer.WriteLine("    <meta name=\"sitemap\" content=\"" + HttpUtility.HtmlEncode(SiteMap) + "\" />");
				if (!String.IsNullOrEmpty(Web_Skin))
					writer.WriteLine("    <meta name=\"code\" content=\"" + HttpUtility.HtmlEncode(Web_Skin) + "\" />");
                if ((IncludeMenu.HasValue) && (IncludeMenu.Value))
                    writer.WriteLine("    <meta name=\"menu\" content=\"true\" />");

				if ( !String.IsNullOrEmpty(Extra_Head_Info))
					writer.WriteLine(Extra_Head_Info);

				writer.WriteLine("  </head>");
				writer.WriteLine("  <body>");
				writer.WriteLine(Content);
				writer.WriteLine("  </body>");
				writer.WriteLine("</html>");
				writer.WriteLine();
				writer.Flush();
				writer.Close();


				return true;
			}
			catch 
			{
				return false;
			}
		}
    }
}
