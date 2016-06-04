#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Users;
using SobekCM.Library.UI;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;

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
            Title = "Viewer";
            html_element_name = "viewer";
        }

        #region iElement Members

        /// <summary> Renders the HTML for this element </summary>
        /// <param name="Output"> Textwriter to write the HTML for this element </param>
        /// <param name="Bib"> Object to populate this element from </param>
        /// <param name="Skin_Code"> Code for the current skin </param>
        /// <param name="IsMozilla"> Flag indicates if the current browse is Mozilla Firefox (different css choices for some elements)</param>
        /// <param name="PopupFormBuilder"> Builder for any related popup forms for this element </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        /// <remarks> This simple element does not append any popup form to the popup_form_builder</remarks>
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool IsMozilla, StringBuilder PopupFormBuilder, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string DEFAULT_ACRONYM = "Select the view types for this material when viewed online.";
                switch (CurrentLanguage)
                {
                    case Web_Language_Enum.English:
                        Acronym = DEFAULT_ACRONYM;
                        break;

                    case Web_Language_Enum.Spanish:
                        Acronym = DEFAULT_ACRONYM;
                        break;

                    case Web_Language_Enum.French:
                        Acronym = DEFAULT_ACRONYM;
                        break;

                    default:
                        Acronym = DEFAULT_ACRONYM;
                        break;
                }
            }

            // Get the list of viewers in the system available
            List<string> systemViewers = new List<string>();
            foreach (var viewer in UI_ApplicationCache_Gateway.Configuration.UI.WriterViewers.Items.Viewers)
            {
                if ((viewer.Enabled) && (!viewer.ManagementViewer))
                {
                    systemViewers.Add(viewer.ViewerType);
                }
            }

            string id_name = html_element_name.Replace("_", "");

            Output.WriteLine("  <!-- " + Title + " Element -->");
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td style=\"width:" + LEFT_MARGIN + "px\">&nbsp;</td>");
            if (Acronym.Length > 0)
            {
                Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\"><acronym title=\"" + Acronym + "\">" + Translator.Get_Translation(Title, CurrentLanguage) + ":</acronym></a></td>");
            }
            else
            {
                Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\">" + Translator.Get_Translation(Title, CurrentLanguage) + ":</a></td>");
            }
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table>");
            Output.WriteLine("        <tr>");
            Output.WriteLine("          <td>");
            Output.WriteLine("            <div id=\"" + html_element_name + "_div\">");

            // Options = NONE, HTML, HTML_MAP, JPEG, JPEG2000, RELATED_IMAGES, TEXT, PAGE TURNER, GOOGLE MAP, EMPTY STRING
            // Get collection of all items
            List<View_Object> views = new List<View_Object>();
            if (Bib.Behaviors.Views_Count > 0)
            {
                views.AddRange(Bib.Behaviors.Views.Where(ThisView => !ThisView.Exclude));
            }

            if (views.Count == 0 )
            {
                const int i = 1;

                // Add the view types select
                Output.Write("<select name=\"" + id_name + "_type" + i + "\" id=\"" + id_name + "_type" + i + "\" class=\"" + html_element_name + "_type\" onchange=\"viewer_type_changed('" + id_name + "_type" + i + "');\">");
                Output.Write("<option value=\"\" selected=\"selected\">&nbsp;</option>");

                foreach (string systemViewer in systemViewers)
                {
                    Output.Write("<option value=\"" + systemViewer + "\">" + systemViewer.Replace("_", " ") + "</option>");
                }

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
                Output.WriteLine("<input name=\"" + id_name + "_label" + i + "\" id=\"" + id_name + "_label" + i + "\" class=\"" + html_element_name + "_label_input sbk_Focusable\" type=\"text\" value=\"\" /></span>");
                Output.WriteLine("            </div>");
            }
            else
            {
                int viewCount = 1;
                foreach (View_Object thisView in views)
                {
                    // Add the view types select
                    Output.Write("<select name=\"" + id_name + "_type" + viewCount + "\" id=\"" + id_name + "_type" + viewCount + "\" class=\"" + html_element_name + "_type\" onchange=\"viewer_type_changed('" + id_name + "_type" + viewCount + "');\">");
                    Output.Write("<option value=\"\" selected=\"selected\">&nbsp;</option>");

                    foreach (string systemViewer in systemViewers)
                    {
                        if (String.Compare(thisView.View_Type, systemViewer, StringComparison.OrdinalIgnoreCase) == 0)
                            Output.Write("<option value=\"" + systemViewer + "\" selected=\"selected\" >" + systemViewer.Replace("_", " ") + "</option>");
                        else
                            Output.Write("<option value=\"" + systemViewer + "\">" + systemViewer.Replace("_", " ") + "</option>");
                    }
                    Output.Write("</select>");

                    // Add the file sublabel
                    Output.Write("<span id=\"" + id_name + "_details" + viewCount + "\" style=\"display:none\">");
                    Output.Write("<span class=\"metadata_sublabel\">File:</span>");

                    // Add the file select
                    Output.Write("<select name=\"" + id_name + "_file" + viewCount + "\" id=\"" + id_name + "_file" + viewCount + "\" class=\"" + html_element_name + "_file\">");

                    if (thisView.Attributes.Length > 0)
                    {
                        Output.Write("<option value=\"\">&nbsp;</option>");
                        Output.Write("<option value=\"" + thisView.Attributes + "\" selected=\"selected\">" + thisView.Attributes + "</option>");
                    }
                    else
                    {
                        Output.Write("<option value=\"\" selected=\"selected\">&nbsp;</option>");
                    }
                    Output.Write("</select>");

                    // Add the label sublabel
                    Output.Write("<span class=\"metadata_sublabel\">Label:</span>");

                    // Add the label input
                    Output.Write("<input name=\"" + id_name + "_label" + viewCount + "\" id=\"" + id_name + "_label" + viewCount + "\" class=\"" + html_element_name + "_label_input sbk_Focusable\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisView.Label) + "\" /></span>");

                    Output.WriteLine("<br />");

                    viewCount++;
                }

                // Add an empty viewer line as well
                // Add the view types select
                Output.Write("<select name=\"" + id_name + "_type" + viewCount + "\" id=\"" + id_name + "_type" + viewCount + "\" class=\"" + html_element_name + "_type\" onchange=\"viewer_type_changed('" + id_name + "_type" + viewCount + "');\">");
                Output.Write("<option value=\"\" selected=\"selected\">&nbsp;</option>");

                foreach (string systemViewer in systemViewers)
                {
                    Output.Write("<option value=\"" + systemViewer + "\">" + systemViewer.Replace("_", " ") + "</option>");
                }
                Output.Write("</select>");

                // Add the file sublabel
                Output.Write("<span id=\"" + id_name + "_details" + viewCount + "\" style=\"display:none\">");
                Output.Write("<span class=\"metadata_sublabel\">File:</span>");

                // Add the file select
                Output.Write("<select name=\"" + id_name + "_file" + viewCount + "\" id=\"" + id_name + "_file" + viewCount + "\" class=\"" + html_element_name + "_file\">");

                Output.Write("<option value=\"\" selected=\"selected\">&nbsp;</option>");
                
                Output.Write("</select>");

                // Add the label sublabel
                Output.Write("<span class=\"metadata_sublabel\">Label:</span>");

                // Add the label input
                Output.Write("<input name=\"" + id_name + "_label" + viewCount + "\" id=\"" + id_name + "_label" + viewCount + "\" class=\"" + html_element_name + "_label_input sbk_Focusable\" type=\"text\" value=\"\" /></span>");


                Output.WriteLine("</div>");

            }
            Output.WriteLine("          </td>");

            Output.WriteLine("          <td style=\"vertical-align:bottom\" >");
            if (Repeatable)
            {
                Output.WriteLine("            <a title=\"" + Translator.Get_Translation("Click to add a new type of viewer", CurrentLanguage) + ".\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onmousedown=\"return add_viewer_element();\"><img border=\"0px\" class=\"repeat_button\" src=\"" + REPEAT_BUTTON_URL + "\" /></a>");
            }
            Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img class=\"help_button\" src=\"" + HELP_BUTTON_URL + "\" /></a>");
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

        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            // Build a dictionary of the current views
            Dictionary<string, View_Object> typeToViewObjectDictionary = new Dictionary<string,View_Object>(StringComparer.OrdinalIgnoreCase);
            foreach (View_Object thisView in Bib.Behaviors.Views)
            {
                typeToViewObjectDictionary[thisView.View_Type] = thisView;
            }

            // Clear the viewers
            Bib.Behaviors.Clear_Views();

            // Save each view
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf("viewer_type") == 0)
                {
                    // Get the viewer type
                    string key = thisKey.Replace("viewer_type", "");
                    string type = HttpContext.Current.Request.Form[thisKey].Trim();

                    // Did this type already exist?
                    if (typeToViewObjectDictionary.ContainsKey(type))
                    {
                        // View already exists
                        View_Object addBackView = typeToViewObjectDictionary[type].Copy();
                        addBackView.Exclude = false;
                        Bib.Behaviors.Add_View(addBackView);
                    }
                    else
                    {
                        string file_key = "viewer_file" + key;
                        string label_key = "viewer_label" + key;
                        string file = String.Empty;
                        string label = String.Empty;

                        // A new view type, so add this view
                        Bib.Behaviors.Add_View(type, label, file);
                    }

                    //// Get the details information for html and html map
                    //if (type == "HTML")
                    //{
                    //    if (HttpContext.Current.Request.Form[file_key] != null)
                    //    {
                    //        file = HttpContext.Current.Request.Form[file_key].Trim();
                    //    }
                    //    if (HttpContext.Current.Request.Form[label_key] != null)
                    //    {
                    //        label = HttpContext.Current.Request.Form[label_key].Trim();
                    //    }
                    //}


                }
            }

            typeToViewObjectDictionary.Clear();
        }

        #endregion

        #region Methods Implementing the Abstract Methods from abstract_Element class

        /// <summary> Reads the inner data from the CompleteTemplate XML format </summary>
        /// <param name="XMLReader"> Current template xml configuration reader </param>
        /// <remarks> This procedure does not currently read any inner xml (not yet necessary) </remarks>
        protected override void Inner_Read_Data(XmlTextReader XMLReader)
        {
            // Do nothing
        }

        #endregion
    }
}


