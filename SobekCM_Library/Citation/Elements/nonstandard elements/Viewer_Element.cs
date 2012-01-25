#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.SobekCM_Info;
using SobekCM.Library.Application_State;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows entry of viewer information (item views and item-level-page views) for an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    public class Viewer_Element : abstract_Element
    {
        /// <summary> Constructor for a new instance of the Viewer_Element class </summary>
        public Viewer_Element()
        {
            Repeatable = true;
            Type = Element_Type.Temporal;
            Display_SubType = "complex";
            Title = "Viewer";
            html_element_name = "viewer";
        }

        #region iElement Members

        /// <summary> Renders the HTML for this element </summary>
        /// <param name="Output"> Textwriter to write the HTML for this element </param>
        /// <param name="Bib"> Object to populate this element from </param>
        /// <param name="Skin_Code"> Code for the current skin </param>
        /// <param name="isMozilla"> Flag indicates if the current browse is Mozilla Firefox (different css choices for some elements)</param>
        /// <param name="popup_form_builder"> Builder for any related popup forms for this element </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        /// <remarks> This simple element does not append any popup form to the popup_form_builder</remarks>
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool isMozilla, StringBuilder popup_form_builder, User_Object Current_User, Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string defaultAcronym = "Select the view types for this material when viewed online.";
                switch (CurrentLanguage)
                {
                    case Language_Enum.English:
                        Acronym = defaultAcronym;
                        break;

                    case Language_Enum.Spanish:
                        Acronym = defaultAcronym;
                        break;

                    case Language_Enum.French:
                        Acronym = defaultAcronym;
                        break;

                    default:
                        Acronym = defaultAcronym;
                        break;
                }
            }

            string id_name = html_element_name.Replace("_", "");

            Output.WriteLine("  <!-- " + Title + " Element -->");
            Output.WriteLine("  <tr align=\"left\">");
            Output.WriteLine("    <td width=\"" + LEFT_MARGIN + "px\">&nbsp;</td>");
            if (Acronym.Length > 0)
            {
                Output.WriteLine("    <td valign=\"top\" class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\"><acronym title=\"" + Acronym + "\">" + Translator.Get_Translation(Title, CurrentLanguage) + ":</acronym></a></td>");
            }
            else
            {
                Output.WriteLine("    <td valign=\"top\" class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\">" + Translator.Get_Translation(Title, CurrentLanguage) + ":</a></td>");
            }
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table>");
            Output.WriteLine("        <tr>");
            Output.WriteLine("          <td>");
            Output.WriteLine("            <div id=\"" + html_element_name + "_div\">");

            // Options = NONE, HTML, HTML_MAP, JPEG, JPEG2000, RELATED_IMAGES, TEXT, PAGE TURNER, GOOGLE MAP, EMPTY STRING
            // Get collection of all items
            List<View_Object> views = new List<View_Object>();
            if (Bib.SobekCM_Web.Item_Level_Page_Views_Count > 0)
            {
                views.AddRange(Bib.SobekCM_Web.Item_Level_Page_Views);
            }
            if (Bib.SobekCM_Web.Views_Count > 0)
            {
                views.AddRange(Bib.SobekCM_Web.Views.Where(itemView => (itemView.View_Type != View_Enum.CITATION) && (itemView.View_Type != View_Enum.ALL_VOLUMES) && (itemView.View_Type != View_Enum.DOWNLOADS) && (itemView.View_Type != View_Enum.FLASH) && (itemView.View_Type != View_Enum.GOOGLE_MAP) && (itemView.View_Type != View_Enum.PDF) && (itemView.View_Type != View_Enum.TOC)));
            }

            if (views.Count == 0 )
            {
                const int i = 1;
                // Add the view types select
                Output.Write("<select name=\"" + id_name + "_type" + i + "\" id=\"" + id_name + "_type" + i + "\" class=\"" + html_element_name + "_type\" onchange=\"viewer_type_changed('" + id_name + "_type" + i + "');\">");
                Output.Write("<option value=\"\" selected=\"selected\">&nbsp;</option>");
                Output.Write("<option value=\"html\" >HTML</option>");
                Output.Write("<option value=\"htmlmap\" >HTML Map</option>");
                Output.Write("<option value=\"jpeg\" >JPEG</option>");
                Output.Write("<option value=\"jpeg2000\" >JPEG2000</option>");
                Output.Write("<option value=\"map\">Map Display</option>");
                Output.Write("<option value=\"pageturner\" >Page Turner</option>");
                Output.Write("<option value=\"related\" >Related Images</option>");
                Output.Write("<option value=\"text\" >Text</option>");
                Output.Write("</select>"); 

                // Add the file sublabel
                Output.Write("<span id=\"" + id_name + "_details" + i + "\" style=\"display:none\">");

                Output.Write("<span class=\"metadata_sublabel\">File:</span>");

                // Add the file select
                Output.Write("<select name=\"" + id_name + "_file" + i + "\" id=\"" + id_name + "_file" + i + "\" class=\"" + html_element_name + "_file\">");
                Output.Write("<option value=\"\" selected=\"selected\">&nbsp;</option>");
                Output.Write("</select>");

                // Add the label sublabel
                Output.Write("<span class=\"metadata_sublabel\">Label:</span>");
                
                // Add the label input
                Output.WriteLine("<input name=\"" + id_name + "_label" + i + "\" id=\"" + id_name + "_label" + i + "\" class=\"" + html_element_name + "_label_input\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('" + id_name + "_label" + i + "', '" + html_element_name + "_label_input_focused')\" onblur=\"javascript:textbox_leave('" + id_name + "_label" + i + "', '" + html_element_name + "_label_input')\" /></span>");
                Output.WriteLine("            </div>");
            }
            else
            {
                for (int i = 1; i <= views.Count; i++)
                {
                    // Add the view types select
                    Output.Write("<select name=\"" + id_name + "_type" + i + "\" id=\"" + id_name + "_type" + i + "\" class=\"" + html_element_name + "_type\" onchange=\"viewer_type_changed('" + id_name + "_type" + i + "');\">");
                    Output.Write(views[i - 1].View_Type == View_Enum.None
                                     ? "<option value=\"\" selected=\"selected\" >&nbsp;</option>"
                                     : "<option value=\"\">&nbsp;</option>");

                    Output.Write(views[i - 1].View_Type == View_Enum.HTML
                                     ? "<option value=\"html\" selected=\"selected\" >HTML</option>"
                                     : "<option value=\"html\" >HTML</option>");

                    Output.Write(views[i - 1].View_Type == View_Enum.HTML_MAP
                                     ? "<option value=\"htmlmap\" selected=\"selected\" >HTML Map</option>"
                                     : "<option value=\"htmlmap\" >HTML Map</option>");

                    Output.Write(views[i - 1].View_Type == View_Enum.JPEG
                                     ? "<option value=\"jpeg\" selected=\"selected\" >JPEG</option>"
                                     : "<option value=\"jpeg\" >JPEG</option>");

                    Output.Write(views[i - 1].View_Type == View_Enum.JPEG2000
                                     ? "<option value=\"jpeg2000\" selected=\"selected\" >JPEG2000</option>"
                                     : "<option value=\"jpeg2000\" >JPEG2000</option>");

                    Output.Write(views[i - 1].View_Type == View_Enum.GOOGLE_MAP
                                     ? "<option value=\"map\" selected=\"selected\" >Map Display</option>"
                                     : "<option value=\"map\" >Map Display</option>");

                    Output.Write(views[i - 1].View_Type == View_Enum.PAGE_TURNER
                                     ? "<option value=\"pageturner\" selected=\"selected\" >Page Turner</option>"
                                     : "<option value=\"pageturner\" >Page Turner</option>");

                    Output.Write(views[i - 1].View_Type == View_Enum.RELATED_IMAGES
                                     ? "<option value=\"related\" selected=\"selected\" >Related Images</option>"
                                     : "<option value=\"related\" >Related Images</option>");

                    Output.Write(views[i - 1].View_Type == View_Enum.TEXT
                                     ? "<option value=\"text\" selected=\"selected\" >Text</option>"
                                     : "<option value=\"text\" >Text</option>");

                    Output.Write("</select>");

                    // Add the file sublabel
                    Output.Write("<span id=\"" + id_name + "_details" + i + "\" style=\"display:none\">");
                    Output.Write("<span class=\"metadata_sublabel\">File:</span>");

                    // Add the file select
                    Output.Write("<select name=\"" + id_name + "_file" + i + "\" id=\"" + id_name + "_file" + i + "\" class=\"" + html_element_name + "_file\">");

                    if (views[i - 1].FileName.Length > 0)
                    {
                        Output.Write("<option value=\"\">&nbsp;</option>");
                        Output.Write("<option value=\"" + views[i - 1].FileName + "\" selected=\"selected\">" + views[i - 1].FileName + "</option>");
                    }
                    else
                    {
                        Output.Write("<option value=\"\" selected=\"selected\">&nbsp;</option>");
                    }
                    Output.Write("</select>");

                    // Add the label sublabel
                    Output.Write("<span class=\"metadata_sublabel\">Label:</span>");

                    // Add the label input
                    Output.Write("<input name=\"" + id_name + "_label" + i + "\" id=\"" + id_name + "_label" + i + "\" class=\"" + html_element_name + "_label_input\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(views[i - 1].Label)  + "\" onfocus=\"javascript:textbox_enter('" + id_name + "_label" + i + "', '" + html_element_name + "_label_input_focused')\" onblur=\"javascript:textbox_leave('" + id_name + "_label" + i + "', '" + html_element_name + "_label_input')\" /></span>");

                    Output.WriteLine(i < views.Count ? "<br />" : "</div>");
                }
            }
            Output.WriteLine("          </td>");

            Output.WriteLine("          <td valign=\"bottom\" >");
            if (Repeatable)
            {
                Output.WriteLine("            <a title=\"" + Translator.Get_Translation("Click to add a new type of viewer", CurrentLanguage) + ".\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onmousedown=\"return add_viewer_element();\"><img border=\"0px\" class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" /></a>");
            }
            Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img border=\"0px\" class=\"help_button\" src=\"" + Base_URL + HELP_BUTTON_URL + "\" /></a>");
            Output.WriteLine("          </td>");

            Output.WriteLine("        </tr>");
            Output.WriteLine("      </table>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine();
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears any preexisting item level views and item level page views </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Clear the viewers
            Bib.SobekCM_Web.Clear_Views();
            Bib.SobekCM_Web.Clear_Item_Level_Page_Views();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            // Save each view
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf("viewer_type") == 0)
                {
                    string key = thisKey.Replace("viewer_type", "");
                    string file_key = "viewer_file" + key;
                    string label_key = "viewer_label" + key;

                    string type = HttpContext.Current.Request.Form[thisKey].Trim();
                    string file = String.Empty;
                    string label = String.Empty;
                    View_Enum viewType = View_Enum.None;
                    switch (type)
                    {
                        case "html":
                            viewType = View_Enum.HTML;
                            break;

                        case "htmlmap":
                            viewType = View_Enum.HTML_MAP;
                            break;

                        case "jpeg":
                            viewType = View_Enum.JPEG;
                            break;

                        case "jpeg2000":
                            viewType = View_Enum.JPEG2000;
                            break;

                        case "map":
                            viewType = View_Enum.GOOGLE_MAP;
                            break;

                        case "pageturner":
                            viewType = View_Enum.PAGE_TURNER;
                            break;

                        case "related":
                            viewType = View_Enum.RELATED_IMAGES;
                            break;

                        case "text":
                            viewType = View_Enum.TEXT;
                            break;
                    }

                    // Get the details information for html and html map
                    if ((viewType == View_Enum.HTML_MAP) || (viewType == View_Enum.HTML))
                    {
                        if (HttpContext.Current.Request.Form[file_key] != null)
                        {
                            file = HttpContext.Current.Request.Form[file_key].Trim();
                        }
                        if (HttpContext.Current.Request.Form[label_key] != null)
                        {
                            label = HttpContext.Current.Request.Form[label_key].Trim();
                        }
                    }

                    // Add this view
                    Bib.SobekCM_Web.Add_View(viewType, label, file);
                }
            }
        }

        #endregion

        #region Methods Implementing the Abstract Methods from abstract_Element class

        /// <summary> Reads the inner data from the Template XML format </summary>
        /// <param name="xmlReader"> Current template xml configuration reader </param>
        /// <remarks> This procedure does not currently read any inner xml (not yet necessary) </remarks>
        protected override void Inner_Read_Data(XmlTextReader xmlReader)
        {
            // Do nothing
        }

        #endregion
    }
}


