#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SobekCM.Library.Settings;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Library.Application_State;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.Navigation;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Print item html subwriter renders a single digital resource in a simplified window for printing purposes  </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
    public class Print_Item_HtmlSubwriter : abstractHtmlSubwriter
    {
        private readonly Aggregation_Code_Manager codeManager;
        private readonly SobekCM_Item currentItem;
        private readonly Language_Support_Info translations;

        /// <summary> Constructor for a new instancee of the Print_Item_HtmlSubwriter class </summary>
        /// <param name="Current_Item">Current item to display </param>
        /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        public Print_Item_HtmlSubwriter(SobekCM_Item Current_Item,
            Aggregation_Code_Manager Code_Manager,
            Language_Support_Info Translator,
            SobekCM_Navigation_Object Current_Mode )
        {
            currentMode = Current_Mode;
            currentItem = Current_Item;
            codeManager = Code_Manager;
            translations = Translator;
        }

        /// <summary> Writes the HTML generated to print this item directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE -- Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("<center>");

            // Determine some variables
            bool include_brief_citation = false;
            string mode = currentMode.ViewerCode.ToLower();
            if (mode.Length < 2 )
                mode = "jj1";
            if (mode[0] == '1')
            {
                include_brief_citation = true;
                mode = mode.Substring(1);
            }
            else if (mode[0] == '0')
                mode = mode.Substring(1);

            if (mode.Length < 2)
                mode = "jj1";
            switch (mode.Substring(0, 2))
            {
                case "tr":
                    print_tracking_sheet(Output);
                    break;

                case "fc":
                    print_full_citation(Output);
                    break;

                case "ri":
                    print_thumbnails(include_brief_citation, Output);
                    break;

                case "xx":
                    int zoom_page = 1;
                    if (mode.Length > 2)
                    {
                        Int32.TryParse(mode.Substring(2), out zoom_page);
                    }
                    print_jpeg2000(include_brief_citation, zoom_page, Output);
                    break;

                case "jj":
                    if (mode.Length == 2)
                        print_pages(include_brief_citation,1, 1, Output);
                    else
                    {
                        if (mode[2] == '*')
                        {
                            print_pages(include_brief_citation, 1, currentItem.Web.Static_PageCount, Output);
                        }
                        else
                        {
                            try
                            {
                                string page_part = mode.Substring(2);
                                if (page_part.IndexOf("-") > 0)
                                {
                                    string[] splitter = page_part.Split("-".ToCharArray());
                                    int from_page = Convert.ToInt32(splitter[0]) + 1;
                                    int to_page = Convert.ToInt32(splitter[1]) + 1;
                                    print_pages(include_brief_citation,  Math.Min(to_page, from_page), Math.Max(to_page, from_page), Output);
                                }
                                else
                                {
                                    int only_page = Convert.ToInt32(page_part);
                                    print_pages(include_brief_citation, only_page, only_page, Output);
                                }
                            }
                            catch
                            {
                                print_pages(include_brief_citation, 1, 1, Output);
                            }
                        }
                    }
                    break;

            }


            Output.WriteLine("</center>");
            return true;
        }

        private void print_brief_citation( string image_width, TextWriter Output)
        {
            if (currentMode.Base_Skin == "ufdc")
            {
                Output.WriteLine("<img src=\"" + currentMode.Default_Images_URL + "ufdc_banner_" + image_width + ".jpg\" />");
                Output.WriteLine("<br />");
            }

            if (currentMode.Base_Skin == "dloc")
            {
                Output.WriteLine("<img src=\"" + currentMode.Default_Images_URL + "dloc_banner_" + image_width + ".jpg\" />");
                Output.WriteLine("<br />");
            }

            Output.WriteLine("<table cellspacing=\"5px\" class=\"citation\" width=\"550px\" >");
            Output.WriteLine("  <tr align=\"left\"><td><b>Title:</b> &nbsp; </td><td>" + currentItem.Bib_Info.Main_Title + "</td></tr>");
            Output.WriteLine("  <tr align=\"left\"><td><b>URL:</b> &nbsp; </td><td>" + currentMode.Base_URL + "/" + currentItem.BibID + "/" + currentItem.VID + "</td></tr>");
            Output.WriteLine("  <tr align=\"left\"><td><b>Site:</b> &nbsp; </td><td>" + currentMode.SobekCM_Instance_Name + "</td></tr>");
            Output.WriteLine("</table>");
        }

        private void print_jpeg2000(bool include_brief_citation, int page, TextWriter Output)
        {
            if (include_brief_citation)
                print_brief_citation("700", Output);

            // Get this page
            Page_TreeNode thisPage = currentItem.Web.Pages_By_Sequence[page-1];

            // Find the jpeg2000 image and show the image
            foreach (SobekCM_File_Info thisFile in thisPage.Files)
            {
                if (thisFile.System_Name.ToUpper().IndexOf(".JP2") > 0)
                {
                    int zoomlevels = zoom_levels( thisFile.Width, thisFile.Height );
                    int size_pixels = 512 + (currentMode.Viewport_Size * 256);
                    if (currentMode.Viewport_Size == 3)
                        size_pixels = 1536;
                    int rotation = (currentMode.Viewport_Rotation % 4) * 90;

                    string jpeg2000_filename = thisFile.System_Name;
                    if ((jpeg2000_filename.Length > 0) && (jpeg2000_filename[0] != '/'))
                    {
                        jpeg2000_filename = "/UFDC/" + currentItem.Web.AssocFilePath + "/" + jpeg2000_filename;
                    }

                    // Build the source URL
                    Output.Write("<img src=\"" + SobekCM_Library_Settings.JP2ServerUrl + "imageserver?res=" + (zoomlevels - currentMode.Viewport_Zoom + 1) + "&viewwidth=" + size_pixels + "&viewheight=" + size_pixels);
                    if (currentMode.Viewport_Zoom != 1)
                        Output.Write("&x=" + currentMode.Viewport_Point_X + "&y=" + currentMode.Viewport_Point_Y);
                    Output.WriteLine("&rotation=" + rotation + "&filename=" + jpeg2000_filename + "\" />");
                    break;
                }
            }
        }

        private int zoom_levels( int width, int height )
        {
            // Get the current portal size in pixels
            float size_pixels = 512 + (currentMode.Viewport_Size * 256);
            if (currentMode.Viewport_Size == 3)
                size_pixels = 1536;

            // Get the factor 
            float width_factor = (width) / size_pixels;
            float height_factor = (height) / size_pixels;
            float max_factor = Math.Max(width_factor, height_factor);

            // Return the zoom level
            if ((max_factor > 1) && (max_factor <= 2))
                return 2;
            if ((max_factor > 2) && (max_factor <= 4))
                return 3;
            if ((max_factor > 4) && (max_factor <= 8))
                return 4;
            if (max_factor > 8)
                return 5;

            // If it made it here, image must be very small!
            return 1;
        }

        private void print_pages(bool include_brief_citation, int from_page, int to_page, TextWriter Output)
        {
            if (include_brief_citation)
                print_brief_citation("700", Output );

            int page_index = from_page - 1;
            while (page_index < to_page)
            {
                // Get this page
                Page_TreeNode thisPage = currentItem.Web.Pages_By_Sequence[page_index];

                // Find the jpeg image and show the image
                foreach (SobekCM_File_Info thisFile in thisPage.Files)
                {
                    if (thisFile.System_Name.IndexOf(".jpg") > 0)
                    {
                        if (page_index > from_page - 1)
                            Output.WriteLine("<br />");

                        Output.WriteLine("<img src=\"" + currentItem.Web.Source_URL + "/" + thisFile.System_Name + "\" />");
                        break;
                    }
                }

                // Go to next page
                page_index++;
            }
        }

        private void print_thumbnails(bool include_brief_citation, TextWriter Output)
        {
            if (include_brief_citation)
                print_brief_citation("550", Output );

            Output.WriteLine("<table cellspacing=\"10px\" class=\"thumbnails\">");
            Output.WriteLine("  <tr align=\"center\" valign=\"top\">");

            int page_index = 0;
            int col = 0;
            while (page_index < currentItem.Web.Static_PageCount)
            {
                Page_TreeNode thisPage = currentItem.Web.Pages_By_Sequence[page_index];

                // Find the jpeg image
                foreach (SobekCM_File_Info thisFile in thisPage.Files.Where(thisFile => thisFile.System_Name.IndexOf(".jpg") > 0))
                {
                    // Should a new row be started
                    if (col == 3)
                    {
                        col = 0;
                        Output.WriteLine("  </tr>\n");
                        Output.WriteLine("  <tr align=\"center\" valign=\"top\">");
                    }

                    Output.WriteLine("    <td><img src=\"" + currentItem.Web.Source_URL + "/" + thisFile.System_Name.Replace(".jpg", "thm.jpg") + "\" border=\"1\" /><br />" + thisPage.Label + "</td>");
                    col++;
                    break;
                }

                // Go to next page
                page_index++;
            }
            if (col == 1)
                Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
            if (col == 2)
                Output.WriteLine("    <td>&nbsp;</td>");

            Output.WriteLine("  </tr>\n");
            Output.WriteLine("</table>\n");

        }

        private void print_full_citation(TextWriter Output)
        {
            Output.WriteLine("</center>");
            if (currentMode.Base_Skin == "ufdc")
            {
                Output.WriteLine("<img src=\"" + currentMode.Default_Images_URL + "ufdc_banner_700.jpg\" />");
                Output.WriteLine("<br />");
            }

            if (currentMode.Base_Skin == "dloc")
            {
                Output.WriteLine("<img src=\"" + currentMode.Default_Images_URL + "dloc_banner_700.jpg\" />");
                Output.WriteLine("<br />");
            }

            Output.WriteLine("<div class=\"SobekCitation\">");
            Citation_ItemViewer citationViewer = new Citation_ItemViewer(translations, codeManager, false)
                                                     {CurrentItem = currentItem, CurrentMode = currentMode};
            Output.WriteLine(citationViewer.Standard_Citation_String(false,null));
            Output.WriteLine("</div>");
        }

        private void print_tracking_sheet(TextWriter Output)
        {
            Output.WriteLine("PRINT TRACKING SHEET");
        }

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        public override List<Tuple<string, string>> Body_Attributes
        {
            get
            {
                List<Tuple<string, string>> returnValue = new List<Tuple<string, string>>();

                returnValue.Add(new Tuple<string, string>("onload", "window.print();window.close();"));
 
                return returnValue;
            }
        }

        /// <summary> Title for this web page </summary>
        public override string WebPage_Title
        {
            get {
                return currentItem != null ? currentItem.Bib_Info.Main_Title.Title : "{0} Item";
            }
        }

        /// <summary> Write any additional values within the HTML Head of the
        /// final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <meta name=\"robots\" content=\"noindex, nofollow\" />");

            // Write the style sheet to use 
            Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM_Item.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");

            // Write the style sheet to use 
            Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM_Print.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");
        }

        /// <summary> Gets the collection of special behaviors which this subwriter
        /// requests from the main HTML writer. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> Subwriter_Behaviors
        {
            get { return new List<HtmlSubwriter_Behaviors_Enum>() {HtmlSubwriter_Behaviors_Enum.Suppress_Footer}; }
        }

		/// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
		/// <value> Always returns an empty string </value>
		public override string Container_CssClass
		{
			get { return String.Empty; }
		}
    }
}
