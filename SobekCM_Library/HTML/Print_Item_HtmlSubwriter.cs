#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Print item html subwriter renders a single digital resource in a simplified window for printing purposes  </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
    public class Print_Item_HtmlSubwriter : abstractHtmlSubwriter
    {
        /// <summary> Constructor for a new instancee of the Print_Item_HtmlSubwriter class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Print_Item_HtmlSubwriter(RequestCache RequestSpecificValues) : base(RequestSpecificValues) 
        {
            // Do nothing
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
            string mode = RequestSpecificValues.Current_Mode.ViewerCode.ToLower();
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
                            print_pages(include_brief_citation, 1, RequestSpecificValues.Current_Item.Web.Static_PageCount, Output);
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
            if (RequestSpecificValues.Current_Mode.Base_Skin == "ufdc")
            {
                Output.WriteLine("<img src=\"ufdc_banner_" + image_width + ".jpg\" />");
                Output.WriteLine("<br />");
            }

            if (RequestSpecificValues.Current_Mode.Base_Skin == "dloc")
            {
                Output.WriteLine("<img src=\"dloc_banner_" + image_width + ".jpg\" />");
                Output.WriteLine("<br />");
            }

            Output.WriteLine("<table cellspacing=\"5px\" class=\"citation\" width=\"550px\" >");
            Output.WriteLine("  <tr align=\"left\"><td><b>Title:</b> &nbsp; </td><td>" + RequestSpecificValues.Current_Item.Bib_Info.Main_Title + "</td></tr>");
            Output.WriteLine("  <tr align=\"left\"><td><b>URL:</b> &nbsp; </td><td>" + RequestSpecificValues.Current_Mode.Base_URL + "/" + RequestSpecificValues.Current_Item.BibID + "/" + RequestSpecificValues.Current_Item.VID + "</td></tr>");
            Output.WriteLine("  <tr align=\"left\"><td><b>Site:</b> &nbsp; </td><td>" + RequestSpecificValues.Current_Mode.Instance_Name + "</td></tr>");
            Output.WriteLine("</table>");
        }

        private void print_jpeg2000(bool include_brief_citation, int page, TextWriter Output)
        {
            if (include_brief_citation)
                print_brief_citation("700", Output);

            // Get this page
            Page_TreeNode thisPage = RequestSpecificValues.Current_Item.Web.Pages_By_Sequence[page-1];

            // Find the jpeg2000 image and show the image
            foreach (SobekCM_File_Info thisFile in thisPage.Files)
            {
                if (thisFile.System_Name.ToUpper().IndexOf(".JP2") > 0)
                {
                    int zoomlevels = zoom_levels( thisFile.Width, thisFile.Height );
                    int currViewportSize = RequestSpecificValues.Current_Mode.Viewport_Size.HasValue ? RequestSpecificValues.Current_Mode.Viewport_Size.Value : 1;
                    int size_pixels = 512 + (currViewportSize * 256);
                    if (RequestSpecificValues.Current_Mode.Viewport_Size == 3)
                        size_pixels = 1536;
                    int currViewportRotation = RequestSpecificValues.Current_Mode.Viewport_Rotation.HasValue ? RequestSpecificValues.Current_Mode.Viewport_Rotation.Value : 0;
                    int rotation = (currViewportRotation % 4) * 90;

                    string jpeg2000_filename = thisFile.System_Name;
                    if ((jpeg2000_filename.Length > 0) && (jpeg2000_filename[0] != '/'))
                    {
                        jpeg2000_filename = "/UFDC/" + RequestSpecificValues.Current_Item.Web.AssocFilePath + "/" + jpeg2000_filename;
                    }

                    // Build the source URL
                    Output.Write("<img src=\"" + UI_ApplicationCache_Gateway.Settings.JP2ServerUrl + "imageserver?res=" + (zoomlevels - RequestSpecificValues.Current_Mode.Viewport_Zoom + 1) + "&viewwidth=" + size_pixels + "&viewheight=" + size_pixels);
                    if (RequestSpecificValues.Current_Mode.Viewport_Zoom != 1)
                        Output.Write("&x=" + RequestSpecificValues.Current_Mode.Viewport_Point_X + "&y=" + RequestSpecificValues.Current_Mode.Viewport_Point_Y);
                    Output.WriteLine("&rotation=" + rotation + "&filename=" + jpeg2000_filename + "\" />");
                    break;
                }
            }
        }

        private int zoom_levels( int width, int height )
        {
            // Get the current portal size in pixels
            int currViewportSize = RequestSpecificValues.Current_Mode.Viewport_Size.HasValue ? RequestSpecificValues.Current_Mode.Viewport_Size.Value : 1;
            float size_pixels = 512 + (currViewportSize * 256);
            if (RequestSpecificValues.Current_Mode.Viewport_Size == 3)
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
                Page_TreeNode thisPage = RequestSpecificValues.Current_Item.Web.Pages_By_Sequence[page_index];

                // Find the jpeg image and show the image
                foreach (SobekCM_File_Info thisFile in thisPage.Files)
                {
                    if (thisFile.System_Name.IndexOf(".jpg") > 0)
                    {
                        if (page_index > from_page - 1)
                            Output.WriteLine("<br />");

                        Output.WriteLine("<img src=\"" + RequestSpecificValues.Current_Item.Web.Source_URL + "/" + thisFile.System_Name + "\" />");
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
            while (page_index < RequestSpecificValues.Current_Item.Web.Static_PageCount)
            {
                Page_TreeNode thisPage = RequestSpecificValues.Current_Item.Web.Pages_By_Sequence[page_index];

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

                    Output.WriteLine("    <td><img src=\"" + RequestSpecificValues.Current_Item.Web.Source_URL + "/" + thisFile.System_Name.Replace(".jpg", "thm.jpg") + "\" border=\"1\" /><br />" + thisPage.Label + "</td>");
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
            if (RequestSpecificValues.Current_Mode.Base_Skin == "ufdc")
            {
                Output.WriteLine("<img src=\"ufdc_banner_700.jpg\" />");
                Output.WriteLine("<br />");
            }

            if (RequestSpecificValues.Current_Mode.Base_Skin == "dloc")
            {
                Output.WriteLine("<img src=\"dloc_banner_700.jpg\" />");
                Output.WriteLine("<br />");
            }

            Output.WriteLine("<div class=\"SobekCitation\">");
       //     Citation_ItemViewer citationViewer = new Citation_ItemViewer(RequestSpecificValues, false);
       //     Output.WriteLine(citationViewer.Standard_Citation_String(false,null));
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
                return RequestSpecificValues.Current_Item != null ? RequestSpecificValues.Current_Item.Bib_Info.Main_Title.Title : "{0} Item";
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
            Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Item_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");
 
            // Write the style sheet to use 
            Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Print_Css + "\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");
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
