#region Using directives

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
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element displays a form to allow for entry of each level of the hierachical spatial subjects for an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    class Hierarchical_Spatial_Form_Element : abstract_Element
    {
        /// <summary> Constructor for a new instance of the Hierarchical_Spatial_Form_Element class </summary>
        public Hierarchical_Spatial_Form_Element()
        {
            Repeatable = true;
            Title = "Spatial Coverage";
            html_element_name = "form_spatial";
	        help_page = "spatialform";
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
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool IsMozilla, StringBuilder PopupFormBuilder, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string DEFAULT_ACRONYM = "Enter spatial information about this material hierarchically.  This can either be the source or the subject of the material.";
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

            // Ensure there is at least one spatial subject
            bool found_spatial = false;
            if (Bib.Bib_Info.Subjects_Count > 0)
            {
                if (Bib.Bib_Info.Subjects.Any(ThisSubject => ThisSubject.Class_Type == Subject_Info_Type.Hierarchical_Spatial))
                {
                    found_spatial = true;
                }
            }
            if (!found_spatial)
            {
                Bib.Bib_Info.Add_Hierarchical_Geographic_Subject();
            }

            int subject_index = 1;
            if (Bib.Bib_Info.Subjects_Count > 0)
            {
                foreach (Subject_Info thisSubject in Bib.Bib_Info.Subjects)
                {

                    if (thisSubject.Class_Type == Subject_Info_Type.Hierarchical_Spatial)
                    {
                        Subject_Info_HierarchicalGeographic hieroSubject = (Subject_Info_HierarchicalGeographic)thisSubject;

                        // Add this subject linke
                        if (hieroSubject.hasData)
                            Output.Write("\n        <a title=\"Click to edit this hierarchical spatial information\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_spatial_term_" + subject_index + "')\" onblur=\"link_blurred2('form_spatial_term_" + subject_index + "')\" onkeypress=\"return popup_keypress_focus('form_spatial_" + subject_index + "', 'formspatialcontinent_" + subject_index + "', '" + IsMozilla.ToString() + "' );\"  onclick=\"return popup_focus('form_spatial_" + subject_index + "', 'formspatialcontinent_" + subject_index + "' );\"><div class=\"form_linkline form_spatial_line\" id=\"form_spatial_term_" + subject_index + "\">" + hieroSubject + "</div></a>");
                        else
                            Output.Write("\n        <a title=\"Click to edit this hierarchical spatial information\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_spatial_term_" + subject_index + "')\" onblur=\"link_blurred2('form_spatial_term_" + subject_index + "')\" onkeypress=\"return popup_keypress_focus('form_spatial_" + subject_index + "', 'formspatialcontinent_" + subject_index + "', '" + IsMozilla.ToString() + "' );\"  onclick=\"return popup_focus('form_spatial_" + subject_index + "', 'formspatialcontinent_" + subject_index + "' );\"><div class=\"form_linkline_empty form_spatial_line\" id=\"form_spatial_term_" + subject_index + "\"><i>Empty Spatial Coverage</i></div></a>");

                        // Add the popup form
                        PopupFormBuilder.AppendLine("<!-- Hierarchical Spatial Form " + subject_index + " -->");
						PopupFormBuilder.AppendLine("<div class=\"spatial_popup_div sbkMetadata_PopupDiv\" id=\"form_spatial_" + subject_index + "\" style=\"display:none;\">");
						PopupFormBuilder.AppendLine("  <div class=\"sbkMetadata_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left\">Edit Hierarchical Spatial</td><td style=\"text-align:right\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" alt=\"HELP\" target=\"_" + html_element_name.ToUpper() + "\" >?</a> &nbsp; <a href=\"#template\" alt=\"CLOSE\" onclick=\"close_spatial_form('form_spatial_" + subject_index + "')\">X</a> &nbsp; </td></tr></table></div>");
                        PopupFormBuilder.AppendLine("  <br />");
						PopupFormBuilder.AppendLine("  <table class=\"sbkMetadata_PopupTable\">");

                        // Add the rows of data
						PopupFormBuilder.AppendLine("    <tr><td style=\"width:100px;\">Continent:</td><td colspan=\"2\"><input class=\"formspatial_input sbk_Focusable\" name=\"formspatialcontinent_" + subject_index + "\" id=\"formspatialcontinent_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Continent) + "\" /></td></tr>");
						PopupFormBuilder.AppendLine("    <tr><td>Country:</td><td colspan=\"2\"><input class=\"formspatial_input sbk_Focusable\" name=\"formspatialcountry_" + subject_index + "\" id=\"formspatialcountry_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Country) + "\" /></td></tr>");
						PopupFormBuilder.AppendLine("    <tr><td>Province:</td><td colspan=\"2\"><input class=\"formspatial_input sbk_Focusable\" name=\"formspatialprovince_" + subject_index + "\" id=\"formspatialprovince_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Province) + "\" /></td></tr>");
						PopupFormBuilder.AppendLine("    <tr><td>Region:</td><td colspan=\"2\"><input class=\"formspatial_input sbk_Focusable\" name=\"formspatialregion_" + subject_index + "\" id=\"formspatialregion_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Region) + "\" /></td></tr>");
						PopupFormBuilder.AppendLine("    <tr><td>State:</td><td colspan=\"2\"><input class=\"formspatial_input sbk_Focusable\" name=\"formspatialstate_" + subject_index + "\" id=\"formspatialstate_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.State) + "\" /></td></tr>");
						PopupFormBuilder.AppendLine("    <tr><td>Territory:</td><td colspan=\"2\"><input class=\"formspatial_input sbk_Focusable\" name=\"formspatialterritory_" + subject_index + "\" id=\"formspatialterritory_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Territory) + "\" /></td></tr>");
						PopupFormBuilder.AppendLine("    <tr><td>County:</td><td colspan=\"2\"><input class=\"formspatial_input sbk_Focusable\" name=\"formspatialcounty_" + subject_index + "\" id=\"formspatialcounty_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.County) + "\" /></td></tr>");
						PopupFormBuilder.AppendLine("    <tr><td>City:</td><td colspan=\"2\"><input class=\"formspatial_input sbk_Focusable\" name=\"formspatialcity_" + subject_index + "\" id=\"formspatialcity_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.City) + "\" /></td></tr>");
						PopupFormBuilder.AppendLine("    <tr><td>City Section:</td><td colspan=\"2\"><input class=\"formspatial_input sbk_Focusable\" name=\"formspatialsectioncity_" + subject_index + "\" id=\"formspatialsectioncity_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.CitySection) + "\" /></td></tr>");
						PopupFormBuilder.AppendLine("    <tr><td>Island:</td><td colspan=\"2\"><input class=\"formspatial_input sbk_Focusable\" name=\"formspatialisland_" + subject_index + "\" id=\"formspatialisland_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Island) + "\" /></td></tr>");
						PopupFormBuilder.AppendLine("    <tr><td>Area:</td><td colspan=\"2\"><input class=\"formspatial_input sbk_Focusable\" name=\"formspatialarea_" + subject_index + "\" id=\"formspatialarea_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Area) + "\" /></td></tr>");


                        // Add the authority and language text boxes
                        PopupFormBuilder.Append("    <tr><td>Authority:</td><td>");
                        PopupFormBuilder.Append("<input class=\"formspatial_small_input sbk_Focusable\" name=\"formspatialauthority_" + subject_index + "\" id=\"formspatialauthority_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Authority) + "\" />");
                        PopupFormBuilder.Append("</td><td style=\"width:255px\" > &nbsp; Language: &nbsp; ");
						PopupFormBuilder.Append("<input class=\"formspatial_small_input sbk_Focusable\" name=\"formspatiallanguage_" + subject_index + "\" id=\"formspatiallanguage_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Language) + "\" />");
                        PopupFormBuilder.AppendLine("</td></tr>");
                        PopupFormBuilder.AppendLine("    <tr style=\"height:30px; vertical-align:bottom\"><td colspan=\"3\" style=\"text-align:center\">");

                        if (hieroSubject.ID.IndexOf("SUBJ752") >= 0)
                        {
                            PopupFormBuilder.AppendLine("        <input type=\"radio\" name=\"formspatialtype_" + subject_index + "\" id=\"formspatialtype_subj_" + subject_index + "\" value=\"subject\" /><label for=\"formspatialtype_subj_" + subject_index + "\">Hierarchical Subject</label> &nbsp; &nbsp; ");
                            PopupFormBuilder.AppendLine("        <input type=\"radio\" name=\"formspatialtype_" + subject_index + "\" id=\"formspatialtype_added_" + subject_index + "\" value=\"addedentry\" checked=\"checked\" /><label for=\"formspatialtype_added_" + subject_index + "\">Added Hierarchical Entry</label>");
                        }
                        else
                        {
                            PopupFormBuilder.AppendLine("        <input type=\"radio\" name=\"formspatialtype_" + subject_index + "\" id=\"formspatialtype_subj_" + subject_index + "\" value=\"subject\" checked=\"checked\" /><label for=\"formspatialtype_subj_" + subject_index + "\">Hierarchical Subject</label> &nbsp; &nbsp; ");
                            PopupFormBuilder.AppendLine("        <input type=\"radio\" name=\"formspatialtype_" + subject_index + "\" id=\"formspatialtype_added_" + subject_index + "\" value=\"addedentry\" /><label for=\"formspatialtype_added_" + subject_index + "\">Added Hierarchical Entry</label>");
                        }
                        PopupFormBuilder.AppendLine("    </td></tr>");
						PopupFormBuilder.AppendLine("    <tr style=\"height:35px; text-align: center; vertical-align: bottom;\">");
						PopupFormBuilder.AppendLine("      <td colspan=\"3\"><button title=\"Close\" class=\"sbkMetadata_RoundButton\" onclick=\"return close_spatial_form('form_spatial_" + subject_index + "');\">CLOSE</button></td>");
						PopupFormBuilder.AppendLine("    </tr>");

                        PopupFormBuilder.AppendLine("  </table>");
                        PopupFormBuilder.AppendLine("</div>");
                        PopupFormBuilder.AppendLine();

                        subject_index++;
                    }
                }
            }

            // Add the link to add a new other subject and close the main element
            Output.WriteLine("\n            </div>");
            Output.WriteLine("          </td>");
            Output.WriteLine("          <td style=\"vertical-align:bottom\" >");
            if (Repeatable)
            {
                Output.WriteLine("            <a title=\"" + Translator.Get_Translation("Click to add a new spatial coverage object", CurrentLanguage) + ".\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onmousedown=\"return new_spatial_link_clicked('" + Template_Page + "');\"><img class=\"repeat_button\" src=\"" + REPEAT_BUTTON_URL + "\" /></a>");
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
        /// <remarks> This clears any preexisting hierarchical spatial subjects </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            List<Subject_Info> deletes = new List<Subject_Info>();
            if (Bib.Bib_Info.Subjects_Count > 0)
            {
                deletes.AddRange(Bib.Bib_Info.Subjects.Where(ThisSubject => ThisSubject.Class_Type == Subject_Info_Type.Hierarchical_Spatial));
                foreach (Subject_Info thisSubject in deletes)
                {
                    Bib.Bib_Info.Remove_Subject(thisSubject);
                }
            }
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf("formspatialcontinent_") == 0)
                {
                    string diff = thisKey.Replace("formspatialcontinent_", "");
                    string continent = HttpContext.Current.Request.Form[thisKey];
                    string country = HttpContext.Current.Request.Form["formspatialcountry_" + diff].Trim();
                    string province = HttpContext.Current.Request.Form["formspatialprovince_" + diff].Trim();
                    string region = HttpContext.Current.Request.Form["formspatialregion_" + diff].Trim();
                    string state = HttpContext.Current.Request.Form["formspatialstate_" + diff].Trim();
                    string territory = HttpContext.Current.Request.Form["formspatialterritory_" + diff].Trim();
                    string county = HttpContext.Current.Request.Form["formspatialcounty_" + diff].Trim();
                    string city = HttpContext.Current.Request.Form["formspatialcity_" + diff].Trim();
                    string citysection = HttpContext.Current.Request.Form["formspatialsectioncity_" + diff].Trim();
                    string island = HttpContext.Current.Request.Form["formspatialisland_" + diff].Trim();
                    string area = HttpContext.Current.Request.Form["formspatialarea_" + diff].Trim();

                    string authority = HttpContext.Current.Request.Form["formspatialauthority_" + diff].Trim();
                    string language = HttpContext.Current.Request.Form["formspatiallanguage_" + diff].Trim();

                    string type = HttpContext.Current.Request.Form["formspatialtype_" + diff].Trim();


                    if ((area.Length > 0) || (continent.Length > 0) || (country.Length > 0) || (province.Length > 0) ||
                        (region.Length > 0) || (state.Length > 0) || (territory.Length > 0) || (county.Length > 0) ||
                        (city.Length > 0) || (island.Length > 0))
                    {
                        Subject_Info_HierarchicalGeographic newSubject = new Subject_Info_HierarchicalGeographic
                                                                             {
                                                                                 Continent = continent,
                                                                                 Country = country,
                                                                                 Province = province,
                                                                                 Region = region,
                                                                                 State = state,
                                                                                 Territory = territory,
                                                                                 County = county,
                                                                                 City = city,
                                                                                 CitySection = citysection,
                                                                                 Island = island,
                                                                                 Area = area,
                                                                                 Authority = authority,
                                                                                 Language = language,
                                                                                 ID = type == "addedentry" ? "SUBJ752" : "SUBJ662"
                                                                             };

                        Bib.Bib_Info.Add_Subject(newSubject);
                    }
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
