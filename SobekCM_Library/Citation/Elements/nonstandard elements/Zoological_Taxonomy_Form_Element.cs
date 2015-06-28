#region Using directives

using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Users;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Metadata_Modules;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element displays a form to allow for entry of each level of the zoological taxonomy for an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    class Zoological_Taxonomy_Form_Element : abstract_Element
    {
        /// <summary> Constructor for a new instance of the Hierarchical_zootaxon_Form_Element class </summary>
        public Zoological_Taxonomy_Form_Element()
        {
            Repeatable = false;
            Type = Element_Type.Subject;
            Display_SubType = "form";
            Title = "Taxonomy";
            html_element_name = "form_zootaxon";
            help_page = "zootaxonform";

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
        /// <remarks> This element appends a popup form to the popup_form_builder</remarks>
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool IsMozilla, StringBuilder PopupFormBuilder, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL)
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string DEFAULT_ACRONYM = "Enter zoological taxonomic information about living organisms related to this material.";
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

            Output.WriteLine("  <!-- " + Title + " Form Element -->");
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
            Output.Write("      <div id=\"" + html_element_name + "_div\">");

            // Get the zoological taxonomy
            Zoological_Taxonomy_Info zooInfo = Bib.Get_Metadata_Module(GlobalVar.ZOOLOGICAL_TAXONOMY_METADATA_MODULE_KEY) as Zoological_Taxonomy_Info;
            if (zooInfo == null)
                zooInfo = new Zoological_Taxonomy_Info();


            int zoo_index = 1;

            // Add this subject linke
            string zoo_as_string = zooInfo.ToString();
            if (zoo_as_string.Length > 0)
                Output.Write("\n        <a title=\"Click to edit this zoological taxonomic information\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_zootaxon_term_" + zoo_index + "')\" onblur=\"link_blurred2('form_zootaxon_term_" + zoo_index + "')\" onkeypress=\"return popup_keypress_focus('form_zootaxon_" + zoo_index + "', 'formzootaxonkingdom_" + zoo_index + "', '" + IsMozilla.ToString() + "' );\"  onclick=\"return popup_focus('form_zootaxon_" + zoo_index + "', 'formzootaxonkingdom_" + zoo_index + "' );\"><div class=\"form_linkline form_zootaxon_line\" id=\"form_zootaxon_term_" + zoo_index + "\">" + zoo_as_string + "</div></a>");
            else
                Output.Write("\n        <a title=\"Click to edit this hierarchical zootaxon information\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_zootaxon_term_" + zoo_index + "')\" onblur=\"link_blurred2('form_zootaxon_term_" + zoo_index + "')\" onkeypress=\"return popup_keypress_focus('form_zootaxon_" + zoo_index + "', 'formzootaxonkingdom_" + zoo_index + "', '" + IsMozilla.ToString() + "' );\"  onclick=\"return popup_focus('form_zootaxon_" + zoo_index + "', 'formzootaxonkingdom_" + zoo_index + "' );\"><div class=\"form_linkline_empty form_zootaxon_line\" id=\"form_zootaxon_term_" + zoo_index + "\"><i>No taxonomic information</i></div></a>");

            // Add the popup form
            PopupFormBuilder.AppendLine("<!-- Zoological Taxonomy Form " + zoo_index + " -->");
            PopupFormBuilder.AppendLine("<div class=\"zootaxon_popup_div sbkMetadata_PopupDiv\" id=\"form_zootaxon_" + zoo_index + "\" style=\"display:none;\">");
            PopupFormBuilder.AppendLine("  <div class=\"sbkMetadata_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left\">Edit Zoological Taxonomy</td><td style=\"text-align:right\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" alt=\"HELP\" target=\"_" + html_element_name.ToUpper() + "\" >?</a> &nbsp; <a href=\"#template\" alt=\"CLOSE\" onclick=\"return close_zootaxon_form('form_zootaxon_" + zoo_index + "')\">X</a> &nbsp; </td></tr></table></div>");
            PopupFormBuilder.AppendLine("  <br />");
            PopupFormBuilder.AppendLine("  <table class=\"sbkMetadata_PopupTable\">");

            // Add the rows of data
            PopupFormBuilder.AppendLine("    <tr><td style=\"width:100px;\">Kingdom:</td><td colspan=\"2\"><input class=\"formzootaxon_input sbk_Focusable\" name=\"formzootaxonkingdom_" + zoo_index + "\" id=\"formzootaxonkingdom_" + zoo_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(zooInfo.Kingdom) + "\" /></td></tr>");
            PopupFormBuilder.AppendLine("    <tr><td>Phylum:</td><td colspan=\"2\"><input class=\"formzootaxon_input sbk_Focusable\" name=\"formzootaxonphylum_" + zoo_index + "\" id=\"formzootaxonphylum_" + zoo_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(zooInfo.Phylum) + "\" /></td></tr>");
            PopupFormBuilder.AppendLine("    <tr><td>Class:</td><td colspan=\"2\"><input class=\"formzootaxon_input sbk_Focusable\" name=\"formzootaxonclass_" + zoo_index + "\" id=\"formzootaxonclass_" + zoo_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(zooInfo.Class) + "\" /></td></tr>");
            PopupFormBuilder.AppendLine("    <tr><td>Order:</td><td colspan=\"2\"><input class=\"formzootaxon_input sbk_Focusable\" name=\"formzootaxonorder_" + zoo_index + "\" id=\"formzootaxonorder_" + zoo_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(zooInfo.Order) + "\" /></td></tr>");
            PopupFormBuilder.AppendLine("    <tr><td>Family:</td><td colspan=\"2\"><input class=\"formzootaxon_input sbk_Focusable\" name=\"formzootaxonfamily_" + zoo_index + "\" id=\"formzootaxonfamily_" + zoo_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(zooInfo.Family) + "\" /></td></tr>");
            PopupFormBuilder.AppendLine("    <tr><td>Genus:</td><td colspan=\"2\"><input class=\"formzootaxon_input sbk_Focusable\" name=\"formzootaxongenus_" + zoo_index + "\" id=\"formzootaxongenus_" + zoo_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(zooInfo.Genus) + "\" /></td></tr>");
            PopupFormBuilder.AppendLine("    <tr><td>Species:</td><td colspan=\"2\"><input class=\"formzootaxon_input sbk_Focusable\" name=\"formzootaxonspecies_" + zoo_index + "\" id=\"formzootaxonspecies_" + zoo_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(zooInfo.Specific_Epithet) + "\" /></td></tr>");
            PopupFormBuilder.AppendLine("    <tr><td>Common Name:</td><td colspan=\"2\"><input class=\"formzootaxon_input sbk_Focusable\" name=\"formzootaxoncommon_" + zoo_index + "\" id=\"formzootaxoncommon_" + zoo_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(zooInfo.Common_Name) + "\" /></td></tr>");
            PopupFormBuilder.AppendLine("    <tr style=\"height:35px; text-align: center; vertical-align: bottom;\">");
            PopupFormBuilder.AppendLine("      <td colspan=\"3\"><button title=\"Close\" class=\"sbkMetadata_RoundButton\" onclick=\"return close_zootaxon_form('form_zootaxon_" + zoo_index + "');\">CLOSE</button></td>");
            PopupFormBuilder.AppendLine("    </tr>");

            PopupFormBuilder.AppendLine("  </table>");
            PopupFormBuilder.AppendLine("</div>");
            PopupFormBuilder.AppendLine();

            zoo_index++;

            // Add the link to add a new other subject and close the main element
            Output.WriteLine("\n            </div>");
            Output.WriteLine("          </td>");
            Output.WriteLine("          <td style=\"vertical-align:bottom\" >");
            if (Repeatable)
            {
                Output.WriteLine("            <a title=\"" + Translator.Get_Translation("Click to add a new zoological taxonomy object", CurrentLanguage) + ".\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onmousedown=\"return new_zoo_link_clicked('" + Template_Page + "');\"><img class=\"repeat_button\" src=\"" + REPEAT_BUTTON_URL + "\" /></a>");
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
        /// <remarks> This clears any preexisting hierarchical zootaxon subjects </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Get the zoological taxonomy
            Zoological_Taxonomy_Info zooInfo = Bib.Get_Metadata_Module(GlobalVar.ZOOLOGICAL_TAXONOMY_METADATA_MODULE_KEY) as Zoological_Taxonomy_Info;
            if (zooInfo != null)
                zooInfo.Clear();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            Zoological_Taxonomy_Info zooInfo = Bib.Get_Metadata_Module(GlobalVar.ZOOLOGICAL_TAXONOMY_METADATA_MODULE_KEY) as Zoological_Taxonomy_Info;
            if (zooInfo == null)
                zooInfo = new Zoological_Taxonomy_Info();

            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf("formzootaxonkingdom_") == 0)
                {
                    string diff = thisKey.Replace("formzootaxonkingdom_", "");

                    zooInfo.Kingdom = HttpContext.Current.Request.Form[thisKey];
                    zooInfo.Phylum = HttpContext.Current.Request.Form["formzootaxonphylum_" + diff].Trim();
                    zooInfo.Class = HttpContext.Current.Request.Form["formzootaxonclass_" + diff].Trim();
                    zooInfo.Order = HttpContext.Current.Request.Form["formzootaxonorder_" + diff].Trim();
                    zooInfo.Family = HttpContext.Current.Request.Form["formzootaxonfamily_" + diff].Trim();
                    zooInfo.Genus = HttpContext.Current.Request.Form["formzootaxongenus_" + diff].Trim();
                    zooInfo.Specific_Epithet = HttpContext.Current.Request.Form["formzootaxonspecies_" + diff].Trim();
                    zooInfo.Common_Name = HttpContext.Current.Request.Form["formzootaxoncommon_" + diff].Trim();

                    Bib.Add_Metadata_Module(GlobalVar.ZOOLOGICAL_TAXONOMY_METADATA_MODULE_KEY, zooInfo);
                }
            }
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
