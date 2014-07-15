#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Users;

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
            Type = Element_Type.Subject;
            Display_SubType = "form";
            Title = "Spatial Coverage";
            html_element_name = "form_spatial";
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
                const string defaultAcronym = "Enter spatial information about this material hierarchically.  This can either be the source or the subject of the material.";
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

            Output.WriteLine("  <!-- " + Title + " Form Element -->");
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
            Output.Write("      <div id=\"" + html_element_name + "_div\">");

            // Ensure there is at least one spatial subject
            bool found_spatial = false;
            if (Bib.Bib_Info.Subjects_Count > 0)
            {
                if (Bib.Bib_Info.Subjects.Any(thisSubject => thisSubject.Class_Type == Subject_Info_Type.Hierarchical_Spatial))
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
                            Output.Write("\n        <a title=\"Click to edit this hierarchical spatial information\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_spatial_term_" + subject_index + "')\" onblur=\"link_blurred2('form_spatial_term_" + subject_index + "')\" onkeypress=\"return popup_keypress_focus('form_spatial_" + subject_index + "', 'form_spatial_term_" + subject_index + "', 'formspatialcontinent_" + subject_index + "', 490, 550, '" + isMozilla.ToString() + "' );\"  onclick=\"return popup_focus('form_spatial_" + subject_index + "', 'form_spatial_term_" + subject_index + "', 'formspatialcontinent_" + subject_index + "', 490, 550 );\"><div class=\"form_linkline form_spatial_line\" id=\"form_spatial_term_" + subject_index + "\">" + hieroSubject + "</div></a>");
                        else
                            Output.Write("\n        <a title=\"Click to edit this hierarchical spatial information\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_spatial_term_" + subject_index + "')\" onblur=\"link_blurred2('form_spatial_term_" + subject_index + "')\" onkeypress=\"return popup_keypress_focus('form_spatial_" + subject_index + "', 'form_spatial_term_" + subject_index + "', 'formspatialcontinent_" + subject_index + "', 490, 550, '" + isMozilla.ToString() + "' );\"  onclick=\"return popup_focus('form_spatial_" + subject_index + "', 'form_spatial_term_" + subject_index + "', 'formspatialcontinent_" + subject_index + "', 490, 550 );\"><div class=\"form_linkline_empty form_spatial_line\" id=\"form_spatial_term_" + subject_index + "\"><i>Empty Spatial Coverage</i></div></a>");

                        // Add the popup form
                        popup_form_builder.AppendLine("<!-- Hierarchical Spatial Form " + subject_index + " -->");
                        popup_form_builder.AppendLine("<div class=\"spatial_popup_div\" id=\"form_spatial_" + subject_index + "\" style=\"display:none;\">");
                        popup_form_builder.AppendLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">EDIT HIERARCHICAL SPATIAL</td><td align=\"right\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" alt=\"HELP\" target=\"_" + html_element_name.ToUpper() + "\" >?</a> &nbsp; <a href=\"#template\" alt=\"CLOSE\" onclick=\"close_spatial_form('form_spatial_" + subject_index + "')\">X</a> &nbsp; </td></tr></table></div>");
                        popup_form_builder.AppendLine("  <br />");
                        popup_form_builder.AppendLine("  <table class=\"popup_table\">");

                        // Add the rows of data
                        popup_form_builder.AppendLine("    <tr><td width=\"100px\">Continent:</td><td colspan=\"2\"><input class=\"formspatial_input\" name=\"formspatialcontinent_" + subject_index + "\" id=\"formspatialcontinent_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Continent) + "\" onfocus=\"javascript:textbox_enter('formspatialcontinent_" + subject_index + "', 'formspatial_input_focused')\" onblur=\"javascript:textbox_leave('formspatialcontinent_" + subject_index + "', 'formspatial_input')\" /></td></tr>");
                        popup_form_builder.AppendLine("    <tr><td>Country:</td><td colspan=\"2\"><input class=\"formspatial_input\" name=\"formspatialcountry_" + subject_index + "\" id=\"formspatialcountry_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Country) + "\" onfocus=\"javascript:textbox_enter('formspatialcountry_" + subject_index + "', 'formspatial_input_focused')\" onblur=\"javascript:textbox_leave('formspatialcountry_" + subject_index + "', 'formspatial_input')\" /></td></tr>");
                        popup_form_builder.AppendLine("    <tr><td>Province:</td><td colspan=\"2\"><input class=\"formspatial_input\" name=\"formspatialprovince_" + subject_index + "\" id=\"formspatialprovince_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Province) + "\" onfocus=\"javascript:textbox_enter('formspatialprovince_" + subject_index + "', 'formspatial_input_focused')\" onblur=\"javascript:textbox_leave('formspatialprovince_" + subject_index + "', 'formspatial_input')\" /></td></tr>");
                        popup_form_builder.AppendLine("    <tr><td>Region:</td><td colspan=\"2\"><input class=\"formspatial_input\" name=\"formspatialregion_" + subject_index + "\" id=\"formspatialregion_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Region) + "\" onfocus=\"javascript:textbox_enter('formspatialregion_" + subject_index + "', 'formspatial_input_focused')\" onblur=\"javascript:textbox_leave('formspatialregion_" + subject_index + "', 'formspatial_input')\" /></td></tr>");
                        popup_form_builder.AppendLine("    <tr><td>State:</td><td colspan=\"2\"><input class=\"formspatial_input\" name=\"formspatialstate_" + subject_index + "\" id=\"formspatialstate_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.State) + "\" onfocus=\"javascript:textbox_enter('formspatialstate_" + subject_index + "', 'formspatial_input_focused')\" onblur=\"javascript:textbox_leave('formspatialstate_" + subject_index + "', 'formspatial_input')\" /></td></tr>");
                        popup_form_builder.AppendLine("    <tr><td>Territory:</td><td colspan=\"2\"><input class=\"formspatial_input\" name=\"formspatialterritory_" + subject_index + "\" id=\"formspatialterritory_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Territory) + "\" onfocus=\"javascript:textbox_enter('formspatialterritory_" + subject_index + "', 'formspatial_input_focused')\" onblur=\"javascript:textbox_leave('formspatialterritory_" + subject_index + "', 'formspatial_input')\" /></td></tr>");
                        popup_form_builder.AppendLine("    <tr><td>County:</td><td colspan=\"2\"><input class=\"formspatial_input\" name=\"formspatialcounty_" + subject_index + "\" id=\"formspatialcounty_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.County) + "\" onfocus=\"javascript:textbox_enter('formspatialcounty_" + subject_index + "', 'formspatial_input_focused')\" onblur=\"javascript:textbox_leave('formspatialcounty_" + subject_index + "', 'formspatial_input')\" /></td></tr>");
                        popup_form_builder.AppendLine("    <tr><td>City:</td><td colspan=\"2\"><input class=\"formspatial_input\" name=\"formspatialcity_" + subject_index + "\" id=\"formspatialcity_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.City) + "\" onfocus=\"javascript:textbox_enter('formspatialcity_" + subject_index + "', 'formspatial_input_focused')\" onblur=\"javascript:textbox_leave('formspatialcity_" + subject_index + "', 'formspatial_input')\" /></td></tr>");
                        popup_form_builder.AppendLine("    <tr><td>City Section:</td><td colspan=\"2\"><input class=\"formspatial_input\" name=\"formspatialsectioncity_" + subject_index + "\" id=\"formspatialsectioncity_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.CitySection) + "\" onfocus=\"javascript:textbox_enter('formspatialsectioncity_" + subject_index + "', 'formspatial_input_focused')\" onblur=\"javascript:textbox_leave('formspatialsectioncity_" + subject_index + "', 'formspatial_input')\" /></td></tr>");
                        popup_form_builder.AppendLine("    <tr><td>Island:</td><td colspan=\"2\"><input class=\"formspatial_input\" name=\"formspatialisland_" + subject_index + "\" id=\"formspatialisland_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Island) + "\" onfocus=\"javascript:textbox_enter('formspatialisland_" + subject_index + "', 'formspatial_input_focused')\" onblur=\"javascript:textbox_leave('formspatialisland_" + subject_index + "', 'formspatial_input')\" /></td></tr>");
                        popup_form_builder.AppendLine("    <tr><td>Area:</td><td colspan=\"2\"><input class=\"formspatial_input\" name=\"formspatialarea_" + subject_index + "\" id=\"formspatialarea_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Area) + "\" onfocus=\"javascript:textbox_enter('formspatialarea_" + subject_index + "', 'formspatial_input_focused')\" onblur=\"javascript:textbox_leave('formspatialarea_" + subject_index + "', 'formspatial_input')\" /></td></tr>");


                        // Add the authority and language text boxes
                        popup_form_builder.Append("    <tr><td>Authority:</td><td>");
                        popup_form_builder.Append("<input class=\"formspatial_small_input\" name=\"formspatialauthority_" + subject_index + "\" id=\"formspatialauthority_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Authority) + "\"  onfocus=\"javascript:textbox_enter('formspatialauthority_" + subject_index + "', 'formspatial_small_input_focused')\" onblur=\"javascript:textbox_leave('formspatialauthority_" + subject_index + "', 'formspatial_small_input')\" />");
                        popup_form_builder.Append("</td><td width=\"255px\" > &nbsp; Language: &nbsp; ");
                        popup_form_builder.Append("<input class=\"formspatial_small_input\" name=\"formspatiallanguage_" + subject_index + "\" id=\"formspatiallanguage_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(hieroSubject.Language) + "\" onfocus=\"javascript:textbox_enter('formspatiallanguage_" + subject_index + "', 'formspatial_small_input_focused')\" onblur=\"javascript:textbox_leave('formspatiallanguage_" + subject_index + "', 'formspatial_small_input')\" />");
                        popup_form_builder.AppendLine("</td></tr>");
                        popup_form_builder.AppendLine("    <tr height=\"30px\" valign=\"bottom\" ><td colspan=\"3\">");
                        popup_form_builder.AppendLine("      <center>");

                        if (hieroSubject.ID.IndexOf("SUBJ752") >= 0)
                        {
                            popup_form_builder.AppendLine("        <input type=\"radio\" name=\"formspatialtype_" + subject_index + "\" id=\"formspatialtype_subj_" + subject_index + "\" value=\"subject\" /><label for=\"formspatialtype_subj_" + subject_index + "\">Hierarchical Subject</label> &nbsp; &nbsp; ");
                            popup_form_builder.AppendLine("        <input type=\"radio\" name=\"formspatialtype_" + subject_index + "\" id=\"formspatialtype_added_" + subject_index + "\" value=\"addedentry\" checked=\"checked\" /><label for=\"formspatialtype_added_" + subject_index + "\">Added Hierarchical Entry</label>");
                        }
                        else
                        {
                            popup_form_builder.AppendLine("        <input type=\"radio\" name=\"formspatialtype_" + subject_index + "\" id=\"formspatialtype_subj_" + subject_index + "\" value=\"subject\" checked=\"checked\" /><label for=\"formspatialtype_subj_" + subject_index + "\">Hierarchical Subject</label> &nbsp; &nbsp; ");
                            popup_form_builder.AppendLine("        <input type=\"radio\" name=\"formspatialtype_" + subject_index + "\" id=\"formspatialtype_added_" + subject_index + "\" value=\"addedentry\" /><label for=\"formspatialtype_added_" + subject_index + "\">Added Hierarchical Entry</label>");
                        }
                        popup_form_builder.AppendLine("      </center>");
                        popup_form_builder.AppendLine("    </td></tr>");
                        popup_form_builder.AppendLine("  </table>");
                        popup_form_builder.AppendLine("  <br />");
                        popup_form_builder.AppendLine("  <center><a href=\"#template\" onclick=\"return close_spatial_form('form_spatial_" + subject_index + "');\"><img border=\"0\" src=\"" + Close_Button_URL(Skin_Code, Base_URL ) + "\" alt=\"CLOSE\" /></a></center>");
                        popup_form_builder.AppendLine("</div>");
                        popup_form_builder.AppendLine();

                        subject_index++;
                    }
                }
            }

            // Add the link to add a new other subject and close the main element
            Output.WriteLine("\n            </div>");
            Output.WriteLine("          </td>");
            Output.WriteLine("          <td valign=\"bottom\" >");
            if (Repeatable)
            {
                Output.WriteLine("            <a title=\"" + Translator.Get_Translation("Click to add a new spatial coverage object", CurrentLanguage) + ".\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onmousedown=\"return new_spatial_link_clicked('" + Template_Page + "');\"><img border=\"0px\" class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" /></a>");
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
        /// <remarks> This clears any preexisting hierarchical spatial subjects </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            List<Subject_Info> deletes = new List<Subject_Info>();
            if (Bib.Bib_Info.Subjects_Count > 0)
            {
                deletes.AddRange(Bib.Bib_Info.Subjects.Where(thisSubject => thisSubject.Class_Type == Subject_Info_Type.Hierarchical_Spatial));
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
