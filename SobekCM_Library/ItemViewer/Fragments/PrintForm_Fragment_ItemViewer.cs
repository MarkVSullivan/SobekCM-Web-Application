using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Library.ItemViewer.Viewers;

namespace SobekCM.Library.ItemViewer.Fragments
{
    /// <summary> Fragment item viewer writes the print form to the stream, to be loaded dynamically
    /// when needed by an item viewer </summary>
    public class PrintForm_Fragment_ItemViewer : baseFragment_ItemViewer
    {
        private Page_TreeNode currentPage;
        private abstractItemViewer pageViewer;

        public PrintForm_Fragment_ItemViewer( Page_TreeNode Current_Page, abstractItemViewer PageViewer )
        {
            currentPage = Current_Page;
            pageViewer = PageViewer;
        }


        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Fragment_PrintForm"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Fragment_PrintForm; }
        }

        public override void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {

            if (CurrentItem.Web.ItemID > 0)
            {
                StringBuilder responseBuilder = new StringBuilder();
                string print_options = String.Empty;
                string url_redirect = CurrentMode.Base_URL + CurrentItem.BibID + "/" + CurrentItem.VID + "/print";
                if (CurrentUser != null)
                    url_redirect = CurrentMode.Base_URL + "l/" + CurrentItem.BibID + "/" + CurrentItem.VID + "/print";

                responseBuilder.AppendLine("<!-- Print item form -->");
				responseBuilder.AppendLine("<div id=\"printform_content\" class=\"sbk_PopupForm\">");
				responseBuilder.AppendLine("  <div class=\"sbk_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left;\">Print Options</td><td style=\"text-align:right\"> <a href=\"#template\" title=\"CLOSE\" onclick=\"print_form_close()\">X</a> &nbsp; </td></tr></table></div>");
                responseBuilder.AppendLine("  <br />");
                responseBuilder.AppendLine("  <fieldset><legend>Select the options below to print this item &nbsp; </legend>");
                responseBuilder.AppendLine("    <blockquote>");
                responseBuilder.AppendLine("    <input type=\"checkbox\" id=\"print_citation\" name=\"print_citation\" checked=\"checked\" /> <label for=\"print_citation\">Include brief citation?</label><br /><br />");
                if (CurrentItem.Web.Static_PageCount == 0)
                {
                    responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"citation_only\" id=\"citation_only\" checked=\"checked\" /> <label for=\"current_page\">Full Citation</label><br />");
                }
                else
                {

                    bool something_selected = false;
                    if ((pageViewer != null) && (pageViewer.ItemViewer_Type == ItemViewer_Type_Enum.Citation))
                    {
                        responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"citation_only\" id=\"citation_only\" class=\"print_radiobutton\" checked=\"checked\" /> <label for=\"citation_only\">Full Citation</label><br />");
                        something_selected = true;
                    }
                    else
                    {
                        responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"citation_only\" id=\"citation_only\" class=\"print_radiobutton\" /> <label for=\"citation_only\">Citation only</label><br />");
                    }

                    if ((pageViewer != null) && (pageViewer.ItemViewer_Type == ItemViewer_Type_Enum.Related_Images))
                    {
                        responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"contact_sheet\" id=\"contact_sheet\" class=\"print_radiobutton\" checked=\"checked\" /> <label for=\"contact_sheet\">Print thumbnails</label><br />");
                        something_selected = true;
                    }
                    else
                    {
                        if (CurrentItem.Behaviors.Views.Contains(new View_Object(View_Enum.RELATED_IMAGES)))
                        {
                            responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"contact_sheet\" id=\"contact_sheet\" class=\"print_radiobutton\" /> <label for=\"contact_sheet\">Print thumbnails</label><br />");
                        }
                    }

                    if ((pageViewer != null) && (pageViewer.ItemViewer_Type == ItemViewer_Type_Enum.JPEG2000))
                    {
                        int adjustedZoom = CurrentMode.Viewport_Zoom - 1;
                        if (adjustedZoom > 0)
                        {
                            if ((CurrentMode.Viewport_Size > 0) || (adjustedZoom > 0) || (CurrentMode.Viewport_Rotation > 0))
                            {
                                if (CurrentMode.Viewport_Rotation > 0)
                                {
                                    print_options = "&vo=" + CurrentMode.Viewport_Size.ToString() + adjustedZoom.ToString() + CurrentMode.Viewport_Rotation;
                                }
                                else
                                {
                                    if (adjustedZoom > 0)
                                    {
                                        print_options = "&vo=" + CurrentMode.Viewport_Size.ToString() + adjustedZoom.ToString();
                                    }
                                    else
                                    {
                                        print_options = "&vo=" + CurrentMode.Viewport_Size.ToString();
                                    }
                                }
                            }

                            // Only add the point if it is not 0,0
                            if ((CurrentMode.Viewport_Point_X > 0) || (CurrentMode.Viewport_Point_Y > 0))
                                print_options = print_options + "&vp=" + CurrentMode.Viewport_Point_X + "," + CurrentMode.Viewport_Point_Y;

                            responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"current_view\" id=\"current_view\" class=\"print_radiobutton\" checked=\"checked\" /> <label for=\"current_view\">Print current view</label><br />");
                            responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"current_page\" id=\"current_page\" class=\"print_radiobutton\" /> <label for=\"current_page\">Print current page</label><br />");
                        }
                        else
                        {
                            responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"current_page\" id=\"current_page\" class=\"print_radiobutton\" checked=\"checked\" /> <label for=\"current_page\">Print current page</label><br />");
                        }
                        something_selected = true;
                    }

                    if ((pageViewer != null) && (pageViewer.ItemViewer_Type == ItemViewer_Type_Enum.JPEG))
                    {
                        responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"current_page\" id=\"current_page\" checked=\"checked\" class=\"print_radiobutton\" /> <label for=\"current_page\">Print current page</label><br />");
                        something_selected = true;
                    }

                    if (CurrentItem.Web.Static_PageCount > 1)
                    {
                        // Add the all pages option
                        responseBuilder.AppendLine(!something_selected
                                             ? "    <input type=\"radio\" name=\"print_pages\" value=\"all_pages\" id=\"all_pages\" class=\"print_radiobutton\" checked=\"checked\" /> <label for=\"all_pages\">Print all pages</label><br />"
                                             : "    <input type=\"radio\" name=\"print_pages\" value=\"all_pages\" id=\"all_pages\" class=\"print_radiobutton\" /> <label for=\"all_pages\">Print all pages</label><br />");

                        // Build the options for selecting a page
                        StringBuilder optionBuilder = new StringBuilder();
                        int sequence = 1;
                        foreach (Page_TreeNode thisPage in CurrentItem.Web.Pages_By_Sequence)
                        {
                            if (thisPage.Label.Length > 25)
                            {
                                if ((currentPage != null) && (thisPage == currentPage))
                                {
                                    optionBuilder.Append("<option value=\"" + sequence + "\" selected=\"selected\">" + thisPage.Label.Substring(0, 20) + "...</option> ");
                                }
                                else
                                {
                                    optionBuilder.Append("<option value=\"" + sequence + "\">" + thisPage.Label.Substring(0, 20) + "...</option> ");
                                }
                            }
                            else
                            {
                                if ((currentPage != null) && (thisPage == currentPage))
                                {
                                    optionBuilder.Append("<option value=\"" + sequence + "\" selected=\"selected\">" + thisPage.Label + "</option> ");
                                }
                                else
                                {
                                    optionBuilder.Append("<option value=\"" + sequence + "\">" + thisPage.Label + "</option> ");
                                }
                            }

                            sequence++;
                        }

                        responseBuilder.AppendLine("    <input type=\"radio\" name=\"print_pages\" value=\"range_page\" id=\"range_page\" class=\"print_radiobutton\" /> <label for=\"range_page\">Print a range of pages</label> <label for=\"print_from\">from</label> <select id=\"print_from\" name=\"print_from\">" + optionBuilder + "</select> <label for=\"print_to\">to</label> <select id=\"print_to\" name=\"print_to\">" + optionBuilder + "</select>");
                    }

                    //if ((currentUser != null) && (currentUser.Is_Internal_User))
                    //{
                    //    responseBuilder.AppendLine("    <br /><br /><input type=\"radio\" name=\"print_pages\" value=\"tracking_sheet\" id=\"tracking_sheet\" class=\"print_radiobutton\"  > <label for=\"tracking_sheet\">Print tracking sheet (internal users)</label><br />");
                    //}
                }
                responseBuilder.AppendLine("    </blockquote>");
                responseBuilder.AppendLine("  </fieldset><br />");
				responseBuilder.AppendLine("  <div style=\"text-align:center; font-size:1.3em;\">");
				responseBuilder.AppendLine("    <button title=\"Send\" class=\"roundbutton\" onclick=\"return print_form_close();\"> CANCEL </button> &nbsp; &nbsp; ");
				responseBuilder.AppendLine("    <button title=\"Send\" class=\"roundbutton\" onclick=\"return print_item('" + CurrentMode.Page + "','" + url_redirect + "','" + print_options + "');\"> PRINT </button>");
				responseBuilder.AppendLine("  </div><br />");
                responseBuilder.AppendLine("</div>");
                responseBuilder.AppendLine();

                MainPlaceHolder.Controls.Add(new Literal() {Text = responseBuilder.ToString()});
            }
        }
    }
}
