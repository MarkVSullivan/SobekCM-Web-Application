#region Using directives

using System;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using SobekCM.Resource_Object;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element displays a form to allow for complete entry of the related URL for an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    class Other_URL_Form_Element : abstract_Element
    {
        /// <summary> Constructor for a new instance of the Other_URL_Form_Element class </summary>
        public Other_URL_Form_Element()
        {
            Repeatable = false;
            Type = Element_Type.OtherURL;
            Display_SubType = "form";
            Title = "Related URL";
            html_element_name = "form_related_url";
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
        /// <remarks> This element appends a popup form to the popup_form_builder</remarks>
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool isMozilla, StringBuilder popup_form_builder, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string defaultAcronym = "Enter any related URL for this material";
                switch (CurrentLanguage)
                {
                    case Web_Language_Enum.English:
                        Acronym = defaultAcronym;
                        break;

                    case Web_Language_Enum.Spanish:
                        Acronym = defaultAcronym;
                        break;

                    case Web_Language_Enum.French:
                        Acronym = defaultAcronym;
                        break;

                    default:
                        Acronym = defaultAcronym;
                        break;
                }
            }

            Output.WriteLine("  <!-- Related URL Form Element -->");
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

            // Write the EAD popup link 
            if (Bib.Bib_Info.Location.Other_URL.Length == 0)
            {
                Output.Write("              <a title=\"Click to edit the related URL information\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_related_url_term')\" onblur=\"link_blurred2('form_related_url_term')\" onkeypress=\"return popup_keypress_focus('form_related_url', 'form_related_url_term', 'form_relatedurl_label', 210, 620, '" + isMozilla.ToString() + "' );\" onclick=\"return popup_focus('form_related_url', 'form_related_url_term', 'form_relatedurl_label', 210, 620 );\"><div class=\"form_linkline_empty form_related_url_line\" id=\"form_related_url_term\">");
                Output.Write("<i>Empty Related URL </i>");
            }
            else
            {
                Output.Write("              <a title=\"Click to edit the related URL information\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_related_url_term')\" onblur=\"link_blurred2('form_related_url_term')\" onkeypress=\"return popup_keypress_focus('form_related_url', 'form_related_url_term', 'form_relatedurl_label', 210, 620, '" + isMozilla.ToString() + "' );\" onclick=\"return popup_focus('form_related_url', 'form_related_url_term', 'form_relatedurl_label', 210, 620 );\"><div class=\"form_linkline form_related_url_line\" id=\"form_related_url_term\">");
                Output.Write(HttpUtility.HtmlEncode(Bib.Bib_Info.Location.Other_URL));
                if (Bib.Bib_Info.Location.Other_URL_Display_Label.Length > 0 )
                {
                    Output.Write(" ( <i>" + HttpUtility.HtmlEncode(Bib.Bib_Info.Location.Other_URL_Display_Label) + "</i> )");
                }
            }
            Output.WriteLine("</div></a>");
            Output.WriteLine("            </div>");
            Output.WriteLine("          </td>");
            Output.WriteLine("          <td valign=\"bottom\" >");
            Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img border=\"0px\" class=\"help_button\" src=\"" + Base_URL + HELP_BUTTON_URL + "\" /></a>");
            Output.WriteLine("          </td>");
            Output.WriteLine("        </tr>");
            Output.WriteLine("      </table>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine();

            // Add the popup form
            popup_form_builder.AppendLine("<!-- Related URL Form -->");
            popup_form_builder.AppendLine("<div class=\"related_url_popup_div\" id=\"form_related_url\" style=\"display:none;\">");
            popup_form_builder.AppendLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">EDIT RELATED URL</td><td align=\"right\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" alt=\"HELP\" target=\"_" + html_element_name.ToUpper() + "\" >?</a> &nbsp; <a href=\"#template\" alt=\"CLOSE\" onclick=\"close_related_url_form()\">X</a> &nbsp; </td></tr></table></div>");
            popup_form_builder.AppendLine("  <br />");
            popup_form_builder.AppendLine("  <table class=\"popup_table\">");

            // Add the rows of data
            popup_form_builder.AppendLine("    <tr><td width=\"70px\">Label:</td><td><input class=\"form_relatedurl_input\" name=\"form_relatedurl_label\" id=\"form_relatedurl_label\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Location.Other_URL_Display_Label) + "\" onfocus=\"javascript:textbox_enter('form_relatedurl_label', 'form_relatedurl_input_focused')\" onblur=\"javascript:textbox_leave('form_relatedurl_label', 'form_relatedurl_input')\" /></td></tr>");
            popup_form_builder.AppendLine("    <tr><td>URL:</td><td><input class=\"form_relatedurl_input\" name=\"form_relatedurl_url\" id=\"form_relatedurl_url\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Location.Other_URL) + "\" onfocus=\"javascript:textbox_enter('form_relatedurl_url', 'form_relatedurl_input_focused')\" onblur=\"javascript:textbox_leave('form_relatedurl_url', 'form_relatedurl_input')\" /></td></tr>");
            popup_form_builder.AppendLine("    <tr><td>Note:</td><td><input class=\"form_relatedurl_input\" name=\"form_relatedurl_note\" id=\"form_relatedurl_note\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Location.Other_URL_Note) + "\" onfocus=\"javascript:textbox_enter('form_relatedurl_note', 'form_relatedurl_input_focused')\" onblur=\"javascript:textbox_leave('form_relatedurl_note', 'form_relatedurl_input')\" /></td></tr>");


            // Finish the popup form
            popup_form_builder.AppendLine("  </table>");
            popup_form_builder.AppendLine("  <br />");
            popup_form_builder.AppendLine("  <center><a href=\"#template\" onclick=\"return close_related_url_form();\"><img border=\"0\" src=\"" + Close_Button_URL(Skin_Code, Base_URL ) + "\" alt=\"CLOSE\" /></a></center>");
            popup_form_builder.AppendLine("</div>");
            popup_form_builder.AppendLine();
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This does nothing since there is only one other url </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Do nothing
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {

            Bib.Bib_Info.Location.Other_URL = HttpContext.Current.Request.Form["form_relatedurl_url"].Trim();
            if ( Bib.Bib_Info.Location.Other_URL.Length == 0 )
            {
                Bib.Bib_Info.Location.Other_URL_Display_Label = String.Empty;
                Bib.Bib_Info.Location.Other_URL_Note = String.Empty;
            }
            else
            {
                Bib.Bib_Info.Location.Other_URL_Display_Label = HttpContext.Current.Request.Form["form_relatedurl_label"].Trim();
                Bib.Bib_Info.Location.Other_URL_Note = HttpContext.Current.Request.Form["form_relatedurl_note"].Trim();
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



