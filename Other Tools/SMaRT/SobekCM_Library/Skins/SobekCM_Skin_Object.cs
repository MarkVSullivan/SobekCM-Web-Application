#region Using directives

using System;
using System.IO;

#endregion

namespace SobekCM.Library.Skins
{
    /// <summary> Stores information about an HTML skin, which determines the header, footer, stylesheet, and other design elements for the rendered HTML </summary>
    /// <remarks> This class and concept allows the same pages in this digital library to appear branded in different ways, either as UFDC, dLOC, or something else.  It allows
    /// the rendered html to be altered to match a partner's institutional web pages as well. <br /><br />
    /// Since this class holds the header, footer, and banner information, this HTML skin object is language-specific.</remarks>
    [Serializable]
    public class SobekCM_Skin_Object
    {
        /// <summary> Code for the banner to use, if this is set to override the banner </summary>
        public readonly string Banner_HTML;

        /// <summary> Code for the base skin which this skin derives from  </summary>
        /// <remarks> The base skin is used for many of the common design image files which are reused, such as button images, tab images, etc..<br /><br />
        /// This also corresponds to the location of the base skin files under the design folder.  (i.e., '\design\skins\[CODE]' ) </remarks>
        public readonly string Base_Skin_Code;

        /// <summary> Additional CSS Stylesheet to be included for this skin </summary>
        /// <remarks> The standard SobekCM stylesheet is always included, but this stylesheet can override any styles from the standard </remarks>
        public readonly string CSS_Style;

        /// <summary> Code for this skin </summary>
        /// <remarks> This also corresponds to the location of the main interface files under the design folder.  (i.e., '\design\skins\[CODE]' ) </remarks>
        public readonly string Skin_Code;

        /// <summary> Constructor for a new instance of the SobekCM_Skin_Object class </summary>
        /// <param name="Skin_Code"> Code for this HTML skin</param>
        /// <param name="Base_Skin_Code"> Code for the base HTML skin which this skin derives from</param>
        /// <param name="CSS_Style"> Additional CSS Stylesheet to be included for this HTML skin</param>
        public SobekCM_Skin_Object(string Skin_Code, string Base_Skin_Code, string CSS_Style)
        {
            // Save the parameters
            this.CSS_Style = CSS_Style;
            this.Skin_Code = Skin_Code;
            Override_Banner = false;

            this.Base_Skin_Code = Base_Skin_Code.Length > 0 ? Base_Skin_Code : Skin_Code;

            // Set some defaults
            Banner_HTML = String.Empty;
            Header_HTML = String.Empty;
            Footer_HTML = String.Empty;
            Header_Item_HTML = String.Empty;
            Footer_Item_HTML = String.Empty;
            Language_Code = String.Empty;
        }

        /// <summary> Constructor for a new instance of the SobekCM_Skin_Object class </summary>
        /// <param name="Skin_Code"> Code for this HTML skin</param>
        /// <param name="Base_Skin_Code"> Code for the base HTML skin which this skin derives from</param>
        /// <param name="CSS_Style"> Additional CSS Stylesheet to be included for this HTML skin</param>
        /// <param name="Banner_HTML"> Code for the banner to use, if this is set to override the banner</param>
        public SobekCM_Skin_Object(string Skin_Code, string Base_Skin_Code, string CSS_Style, string Banner_HTML)
        {
            // Save the parameters
            this.CSS_Style = CSS_Style;
            this.Skin_Code = Skin_Code;
            this.Banner_HTML = Banner_HTML;
            Override_Banner = true;

            this.Base_Skin_Code = Base_Skin_Code.Length > 0 ? Base_Skin_Code : Skin_Code;

            // Set some defaults
            Header_HTML = String.Empty;
            Footer_HTML = String.Empty;
            Header_Item_HTML = String.Empty;
            Footer_Item_HTML = String.Empty;
            Language_Code = String.Empty;
        }

        /// <summary>  Flag indicates if the top-level aggregation navigation should be suppressed for this web skin ( i.e., is the top-level navigation embedded into the header file already? ) </summary>
        public bool Suppress_Top_Navigation { get; set; }

        /// <summary> Language code, which indicates which language this skin information pertains to </summary>
        /// <remarks> Since this object holds the header and footer information, this is language-specific </remarks>
        public string Language_Code { get; set; }

        /// <summary> Flag indicates if this skin has a banner which should override any aggregation-specific banner </summary>
        public bool Override_Banner { get; set; }

        /// <summary> HTML for the standard header, to be included when rendering an HTML page  </summary>
        public string Header_HTML { get; set; }

        /// <summary> HTML for the standard footer, to be included when rendering an HTML page  </summary>
        public string Footer_HTML { get; set; }

        /// <summary> HTML for the item-specific header, to be included when rendering an HTML page from the item viewer  </summary>
        public string Header_Item_HTML { get; set; }

        /// <summary> HTML for the item-specific footer, to be included when rendering an HTML page from the item viewer  </summary>
        public string Footer_Item_HTML { get; set; }

        /// <summary> Method sets the header and footer to be used by this HTML skin </summary>
        /// <param name="Header_HTML"> HTML to use for the standard header </param>
        /// <param name="Footer_HTML"> HTML to use for the standard footer </param>
        internal void Set_Header_Footer_HTML(string Header_HTML, string Footer_HTML)
        {
            this.Header_HTML = Header_HTML ?? String.Empty;
            this.Footer_HTML = Footer_HTML ?? String.Empty;
            Header_Item_HTML = String.Empty;
            Footer_Item_HTML = String.Empty;
        }

        /// <summary> Method sets the header and footer to be used by this HTML skin </summary>
        /// <param name="Header_HTML"> HTML to use for the standard header </param>
        /// <param name="Footer_HTML"> HTML to use for the standard footer </param>
        /// <param name="Header_Item_HTML"> HTML to use for the item-specific header (used when displaying an item in the item viewer)</param>
        /// <param name="Footer_Item_HTML"> HTML to use for the item-specific footer (used when displaying an item in the item viewer)</param>
        internal void Set_Header_Footer_HTML(string Header_HTML, string Footer_HTML, string Header_Item_HTML, string Footer_Item_HTML)
        {
            this.Header_HTML = Header_HTML ?? String.Empty;
            this.Footer_HTML = Footer_HTML ?? String.Empty;
            this.Header_Item_HTML = Header_Item_HTML ?? String.Empty;
            this.Footer_Item_HTML = Footer_Item_HTML ?? String.Empty;
        }

        /// <summary> Method sets the header and footer by passing in the names for the source files containing the HTML for the headers and footers </summary>
        /// <param name="Header_Source"> Name for the file containing the standard header HTML </param>
        /// <param name="Footer_Source"> Name for the file containing the standard footer HTML </param>
        /// <param name="Header_Item_Source"> Name for the file containing the item-specific header HTML (used when displaying an item in the item viewer) </param>
        /// <param name="Footer_Item_Source"> Name for the file containing the item-specific footer HTML (used when displaying an item in the item viewer) </param>
        internal void Set_Header_Footer_Source( string Header_Source, string Footer_Source, string Header_Item_Source, string Footer_Item_Source )
        {
            // Get the header and footer html from the source information
            try
            {
                StreamReader reader = new StreamReader(Header_Source);
                Header_HTML = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ee )
            {
                Header_HTML = "Error reading header source ( " + Header_Source + "): " + ee.Message;
            }

            try
            {
                StreamReader reader = new StreamReader( Footer_Source );
                Footer_HTML = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ee)
            {
                Footer_HTML = "Error reading footer source ( " + Footer_Source + "): " + ee.Message;
            }

            if (Header_Item_Source.Length > 0)
            {
                try
                {
                    StreamReader reader = new StreamReader(Header_Item_Source);
                    Header_Item_HTML = reader.ReadToEnd();
                    reader.Close();
                }
                catch (Exception ee)
                {
                    Header_Item_HTML = "Error reading header item source ( " + Header_Item_Source + "): " + ee.Message;
                }
            }

            if (Footer_Item_Source.Length > 0)
            {
                try
                {
                    StreamReader reader = new StreamReader(Footer_Item_Source);
                    Footer_Item_HTML = reader.ReadToEnd();
                    reader.Close();
                }
                catch (Exception ee)
                {
                    Footer_Item_HTML = "Error reading footer item source ( " + Footer_Item_Source + "): " + ee.Message;
                }
            }
        }
    }
}
