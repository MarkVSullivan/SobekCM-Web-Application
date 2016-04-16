#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Client;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.UI;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Print item html subwriter renders a single digital resource in a simplified window for printing purposes  </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
    public class Print_Item_HtmlSubwriter : abstractHtmlSubwriter
    {
        private bool isRestricted;
        private string restriction_message;
        private readonly BriefItemInfo currentItem;

        /// <summary> Constructor for a new instancee of the Print_Item_HtmlSubwriter class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Print_Item_HtmlSubwriter(RequestCache RequestSpecificValues) : base(RequestSpecificValues) 
        {
            // Ensure BibID and VID provided
            RequestSpecificValues.Tracer.Add_Trace("Print_Item_HtmlSubwriter.Constructor", "Validate provided bibid / vid");
            if ((String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.BibID)) || (String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.VID)))
            {
                RequestSpecificValues.Tracer.Add_Trace("Print_Item_HtmlSubwriter.Constructor", "BibID or VID was not provided!");
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Error;
                RequestSpecificValues.Current_Mode.Error_Message = "Invalid Request : BibID/VID missing in item behavior request";
                return;
            }

            // Ensure the item is valid
            RequestSpecificValues.Tracer.Add_Trace("Print_Item_HtmlSubwriter.Constructor", "Validate bibid/vid exists");
            if (!UI_ApplicationCache_Gateway.Items.Contains_BibID_VID(RequestSpecificValues.Current_Mode.BibID, RequestSpecificValues.Current_Mode.VID))
            {
                RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Behaviors_MySobekViewer.Constructor", "BibID/VID indicated is not valid", Custom_Trace_Type_Enum.Error);
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Error;
                RequestSpecificValues.Current_Mode.Error_Message = "Invalid Request : BibID/VID indicated is not valid";
                return;
            }



            RequestSpecificValues.Tracer.Add_Trace("Print_Item_HtmlSubwriter.Constructor", "Try to pull this brief item");
            currentItem = SobekEngineClient.Items.Get_Item_Brief(RequestSpecificValues.Current_Mode.BibID, RequestSpecificValues.Current_Mode.VID, true, RequestSpecificValues.Tracer);
            if (currentItem == null)
            {
                RequestSpecificValues.Tracer.Add_Trace("Print_Item_HtmlSubwriter.Constructor", "Unable to build brief item");
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Error;
                RequestSpecificValues.Current_Mode.Error_Message = "Invalid Request : Unable to build brief item";
                return;
            }


            // If no item, then an error occurred
            if (currentItem == null)
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Error;
                RequestSpecificValues.Current_Mode.Error_Message = "Invalid item indicated";
                return;
            }

            // Check for IP restriction
            restriction_message = String.Empty;
            if (currentItem.Behaviors.IP_Restriction_Membership > 0)
            {
                if (HttpContext.Current != null)
                {
                    int user_mask = (int)HttpContext.Current.Session["IP_Range_Membership"];
                    int comparison = currentItem.Behaviors.IP_Restriction_Membership & user_mask;
                    if (comparison == 0)
                    {
                        int restriction = currentItem.Behaviors.IP_Restriction_Membership;
                        int restriction_counter = 1;
                        while (restriction % 2 != 1)
                        {
                            restriction = restriction >> 1;
                            restriction_counter++;
                        }
                        if (UI_ApplicationCache_Gateway.IP_Restrictions[restriction_counter] != null)
                            restriction_message = UI_ApplicationCache_Gateway.IP_Restrictions[restriction_counter].Item_Restricted_Statement;
                        else
                            restriction_message = "Restricted Item";
                    }
                }
            }

            isRestricted = restriction_message.Length > 0;
        }

        /// <summary> Writes the HTML generated to print this item directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE -- Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            // If restricted by IP, just print the message
            if (isRestricted)
            {
                Output.WriteLine(restriction_message);
                return true;
            }

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

                case "jj":
                    if (mode.Length == 2)
                        print_pages(include_brief_citation,1, 1, Output);
                    else
                    {
                        if (mode[2] == '*')
                        {
                            print_pages(include_brief_citation, 1, currentItem.Images.Count, Output);
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
            if (RequestSpecificValues.Current_Mode.Base_Skin_Or_Skin == "ufdc")
            {
                Output.WriteLine("<img src=\"ufdc_banner_" + image_width + ".jpg\" />");
                Output.WriteLine("<br />");
            }

            if (RequestSpecificValues.Current_Mode.Base_Skin_Or_Skin == "dloc")
            {
                Output.WriteLine("<img src=\"dloc_banner_" + image_width + ".jpg\" />");
                Output.WriteLine("<br />");
            }

            Output.WriteLine("<table cellspacing=\"5px\" class=\"citation\" width=\"550px\" >");
            Output.WriteLine("  <tr align=\"left\"><td><b>Title:</b> &nbsp; </td><td>" + currentItem.Title + "</td></tr>");
            Output.WriteLine("  <tr align=\"left\"><td><b>URL:</b> &nbsp; </td><td>" + RequestSpecificValues.Current_Mode.Base_URL + "/" + currentItem.BibID + "/" + currentItem.VID + "</td></tr>");
            Output.WriteLine("  <tr align=\"left\"><td><b>Site:</b> &nbsp; </td><td>" + RequestSpecificValues.Current_Mode.Instance_Name + "</td></tr>");
            Output.WriteLine("</table>");
        }





        private void print_pages(bool include_brief_citation, int from_page, int to_page, TextWriter Output)
        {
            if (include_brief_citation)
                print_brief_citation("700", Output );

            int page_index = from_page - 1;
            while (page_index < to_page)
            {
                // Get this page
                BriefItem_FileGrouping thisPage = currentItem.Images[page_index];

                // Find the jpeg image and show the image
                foreach (BriefItem_File thisFile in thisPage.Files)
                {
                    if (thisFile.Name.IndexOf(".jpg") > 0)
                    {
                        if (page_index > from_page - 1)
                            Output.WriteLine("<br />");

                        Output.WriteLine("<img src=\"" + currentItem.Web.Source_URL + "/" + thisFile.Name + "\" />");
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
            while (page_index < currentItem.Images.Count)
            {
                BriefItem_FileGrouping thisPage = currentItem.Images[page_index];

                // Find the jpeg image
                foreach (BriefItem_File thisFile in thisPage.Files.Where(thisFile => thisFile.Name.IndexOf(".jpg") > 0))
                {
                    // Should a new row be started
                    if (col == 3)
                    {
                        col = 0;
                        Output.WriteLine("  </tr>\n");
                        Output.WriteLine("  <tr align=\"center\" valign=\"top\">");
                    }

                    Output.WriteLine("    <td><img src=\"" + currentItem.Web.Source_URL + "/" + thisFile.Name.Replace(".jpg", "thm.jpg") + "\" border=\"1\" /><br />" + thisPage.Label + "</td>");
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
            if (RequestSpecificValues.Current_Mode.Base_Skin_Or_Skin == "ufdc")
            {
                Output.WriteLine("<img src=\"ufdc_banner_700.jpg\" />");
                Output.WriteLine("<br />");
            }

            if (RequestSpecificValues.Current_Mode.Base_Skin_Or_Skin == "dloc")
            {
                Output.WriteLine("<img src=\"dloc_banner_700.jpg\" />");
                Output.WriteLine("<br />");
            }

            // For this, also need the brief item (OBVIOUSLY NEEDS TO CHANGE!)

            Output.WriteLine("<div class=\"SobekCitation\">");
            Citation_Standard_ItemViewer citationViewer = new Citation_Standard_ItemViewer(currentItem, RequestSpecificValues.Current_User, RequestSpecificValues.Current_Mode);
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
                return currentItem != null ? currentItem.Title : "{0} Item";
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
            Output.WriteLine("  <link href=\"" + Static_Resources_Gateway.Sobekcm_Item_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");
 
            // Write the style sheet to use 
            Output.WriteLine("  <link href=\"" + Static_Resources_Gateway.Sobekcm_Print_Css + "\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");
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
