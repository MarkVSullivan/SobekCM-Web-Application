#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;
using SobekCM.Library.Application_State;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element displays a form to allow for complete entry of the standard subjects (including topical, geographic, temporal, genre, etc..) for an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    public class Subject_Keyword_Standard_Form_Element : abstract_Element
    {
        /// <summary> Constructor for a new instance of the Subject_Keyword_Standard_Form_Element class </summary>
        public Subject_Keyword_Standard_Form_Element()
        {
            Repeatable = true;
            Type = Element_Type.Subject;
            Display_SubType = "form";
            Title = "Subject Keywords";
            html_element_name = "form_subject";
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
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool isMozilla, StringBuilder popup_form_builder, User_Object Current_User, Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string defaultAcronym = "Enter any subject keyword to describe your material here, along with the vocabulary from which this subject term was pulled.";
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

            // Ensure there is at least one standard subject
            bool found_standard = false;
            if (Bib.Bib_Info.Subjects_Count > 0)
            {
                if (Bib.Bib_Info.Subjects.Any(thisSubject => thisSubject.Class_Type == Subject_Info_Type.Standard))
                {
                    found_standard = true;
                }
            }
            if (!found_standard)
            {
                Bib.Bib_Info.Add_Subject();
            }

            Output.WriteLine("    <td>");
            Output.WriteLine("      <table>");
            Output.WriteLine("        <tr>");
            Output.WriteLine("          <td>");
            Output.Write("      <div id=\"" + html_element_name + "_div\">");
            int subject_index = 1;

            foreach( Subject_Info thisSubject in Bib.Bib_Info.Subjects )
            {

                if ( thisSubject.Class_Type == Subject_Info_Type.Standard )
                {
                    Subject_Info_Standard standSubject = ( Subject_Info_Standard ) thisSubject;

                    // Add this subject linke
                    if ( standSubject.hasData )
                        Output.Write("\n        <a title=\"Click to edit this subject information\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_subject_term_" + subject_index + "')\" onblur=\"link_blurred2('form_subject_term_" + subject_index + "')\" onkeypress=\"return popup_keypress_focus('form_subject_" + subject_index + "', 'form_subject_term_" + subject_index + "', 'formsubjecttopic1_" + subject_index + "', 210, 600, '" + isMozilla.ToString() + "' );\" onclick=\"return popup_focus('form_subject_" + subject_index + "', 'form_subject_term_" + subject_index + "', 'formsubjecttopic1_" + subject_index + "', 210, 600 );\"><div class=\"form_linkline form_subject_line\" id=\"form_subject_term_" + subject_index + "\">" + standSubject + "</div></a>");
                    else
                        Output.Write("\n        <a title=\"Click to edit this subject information\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_subject_term_" + subject_index + "')\" onblur=\"link_blurred2('form_subject_term_" + subject_index + "')\" onkeypress=\"return popup_keypress_focus('form_subject_" + subject_index + "', 'form_subject_term_" + subject_index + "', 'formsubjecttopic1_" + subject_index + "', 210, 600, '" + isMozilla.ToString() + "' );\" onclick=\"return popup_focus('form_subject_" + subject_index + "', 'form_subject_term_" + subject_index + "', 'formsubjecttopic1_" + subject_index + "', 210, 600 );\"><div class=\"form_linkline_empty form_subject_line\" id=\"form_subject_term_" + subject_index + "\"><i>Empty Subject Keyword</i></div></a>");

                    // Add the popup form
                    popup_form_builder.AppendLine("<!-- Subject Keyword Form " + subject_index + " -->");
                    popup_form_builder.AppendLine("<div class=\"subject_popup_div\" id=\"form_subject_" + subject_index + "\" style=\"display:none;\">");
                    popup_form_builder.AppendLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">EDIT SUBJECT</td><td align=\"right\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" alt=\"HELP\" target=\"_" + html_element_name.ToUpper() + "\" >?</a> &nbsp; <a href=\"#template\" alt=\"CLOSE\" onclick=\"close_subject_form('form_subject_" + subject_index + "')\">X</a> &nbsp; </td></tr></table></div>");
                    popup_form_builder.AppendLine("  <br />");
                    popup_form_builder.AppendLine("  <table class=\"popup_table\">");

                    // Add the options for this subject ( standard, name, or title)
                    popup_form_builder.Append("    <tr><td>Subject Type:</td><td><select class=\"formsubject_type\" name=\"formsubjecttype_" + subject_index + "\" id=\"formsubjecttype_" + subject_index + "\" onfocus=\"javascript:textbox_enter('formsubjecttype_" + subject_index + "', 'formsubject_type_focused')\" onblur=\"javascript:textbox_leave('formsubjecttype_" + subject_index + "', 'formsubject_type')\">");
                    popup_form_builder.Append("<option value=\"standard\" selected=\"selected\">Standard</option>");
                    popup_form_builder.AppendLine("</select></td>");
                    popup_form_builder.Append("        <td>MARC: &nbsp; <select class=\"formsubject_map\" name=\"formsubjectmap_" + subject_index + "\" id=\"formsubjectmap_" + subject_index + "\" onfocus=\"javascript:textbox_enter('formsubjectmap_" + subject_index + "', 'formsubject_map_focused')\" onblur=\"javascript:textbox_leave('formsubjectmap_" + subject_index + "', 'formsubject_map')\">");

                    popup_form_builder.Append(standSubject.ID.IndexOf("SUBJ") < 0
                                                  ? "<option value=\"none\" selected=\"selected\" >&nbsp;</option>"
                                                  : "<option value=\"none\" >&nbsp;</option>");

                    popup_form_builder.Append(standSubject.ID.IndexOf("SUBJ648") == 0
                                                  ? "<option value=\"648\" selected=\"selected\" >648 - Chronological Term</option>"
                                                  : "<option value=\"648\" >648 - Chronological Term</option>");

                    popup_form_builder.Append(standSubject.ID.IndexOf("SUBJ650") == 0
                                                  ? "<option value=\"650\" selected=\"selected\" >650 - Topical Term</option>"
                                                  : "<option value=\"650\" >650 - Topical Term</option>");

                    popup_form_builder.Append(standSubject.ID.IndexOf("SUBJ651") == 0
                                                  ? "<option value=\"651\" selected=\"selected\" >651 - Geographic Name</option>"
                                                  : "<option value=\"651\" >651 - Geographic Name</option>");

                    popup_form_builder.Append(standSubject.ID.IndexOf("SUBJ653") == 0
                                                  ? "<option value=\"653\" selected=\"selected\" >653 - Uncontrolled Index</option>"
                                                  : "<option value=\"653\" >653 - Uncontrolled Index</option>");

                    popup_form_builder.Append(standSubject.ID.IndexOf("SUBJ654") == 0
                                                  ? "<option value=\"654\" selected=\"selected\" >654 - Faceted Topical</option>"
                                                  : "<option value=\"654\" >654 - Faceted Topical</option>");

                    popup_form_builder.Append(standSubject.ID.IndexOf("SUBJ655") == 0
                                                  ? "<option value=\"655\" selected=\"selected\" >655 - Genre / Form</option>"
                                                  : "<option value=\"655\" >655 - Genre / Form</option>");

                    popup_form_builder.Append(standSubject.ID.IndexOf("SUBJ656") == 0
                                                  ? "<option value=\"656\" selected=\"selected\" >656 - Occupation</option>"
                                                  : "<option value=\"656\" >656 - Occupation</option>");

                    popup_form_builder.Append(standSubject.ID.IndexOf("SUBJ657") == 0
                                                  ? "<option value=\"657\" selected=\"selected\" >657 - Function</option>"
                                                  : "<option value=\"657\" >657 - Function</option>");

                    popup_form_builder.Append(standSubject.ID.IndexOf("SUBJ690") == 0
                                                  ? "<option value=\"690\" selected=\"selected\" >690 - Local Topical</option>"
                                                  : "<option value=\"690\" >690 - Local Topical</option>");

                    popup_form_builder.Append(standSubject.ID.IndexOf("SUBJ691") == 0
                                                  ? "<option value=\"691\" selected=\"selected\" >691 - Local Geographic</option>"
                                                  : "<option value=\"691\" >691 - Local Geographic</option>");

                    popup_form_builder.AppendLine("</select></td></tr>");

                    // Add the first row of topical term boxes
                    popup_form_builder.Append("    <tr><td>Topical Term(s):</td><td colspan=\"2\">");
                    if (standSubject.Topics_Count >= 1)
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjecttopic1_" + subject_index + "\" id=\"formsubjecttopic1_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Topics[0]) + "\" onfocus=\"javascript:textbox_enter('formsubjecttopic1_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjecttopic1_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }
                    else
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjecttopic1_" + subject_index + "\" id=\"formsubjecttopic1_" + subject_index + "\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('formsubjecttopic1_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjecttopic1_" + subject_index + "', 'formsubject_medium_input')\" />");                    
                    }

                    if (standSubject.Topics_Count >= 2)
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjecttopic2_" + subject_index + "\" id=\"formsubjecttopic2_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Topics[1]) + "\"  onfocus=\"javascript:textbox_enter('formsubjecttopic2_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjecttopic2_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }
                    else
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjecttopic2_" + subject_index + "\" id=\"formsubjecttopic2_" + subject_index + "\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('formsubjecttopic2_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjecttopic2_" + subject_index + "', 'formsubject_medium_input')\"  />");
                    }
                    popup_form_builder.AppendLine("</td></tr>");

                    // Add the second row of topical term boxes
                    popup_form_builder.Append("    <tr><td>&nbsp;</td><td colspan=\"2\">");
                    if (standSubject.Topics_Count >= 3)
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjecttopic3_" + subject_index + "\" id=\"formsubjecttopic3_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Topics[2]) + "\"  onfocus=\"javascript:textbox_enter('formsubjecttopic3_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjecttopic3_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }
                    else
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjecttopic3_" + subject_index + "\" id=\"formsubjecttopic3_" + subject_index + "\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('formsubjecttopic3_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjecttopic3_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }

                    if (standSubject.Topics_Count >= 4)
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjecttopic4_" + subject_index + "\" id=\"formsubjecttopic4_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Topics[3]) + "\"  onfocus=\"javascript:textbox_enter('formsubjecttopic4_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjecttopic4_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }
                    else
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjecttopic4_" + subject_index + "\" id=\"formsubjecttopic4_" + subject_index + "\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('formsubjecttopic4_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjecttopic4_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }
                    popup_form_builder.AppendLine("</td></tr>");

                    // Add the two chronological term boxes
                    popup_form_builder.Append("    <tr><td>Chronological Term(s):</td><td colspan=\"2\">");
                    if (standSubject.Temporals_Count >= 1)
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjecttemporal1_" + subject_index + "\" id=\"formsubjecttemporal1_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Temporals[0]) + "\"  onfocus=\"javascript:textbox_enter('formsubjecttemporal1_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjecttemporal1_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }
                    else
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjecttemporal1_" + subject_index + "\" id=\"formsubjecttemporal1_" + subject_index + "\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('formsubjecttemporal1_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjecttemporal1_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }

                    if (standSubject.Temporals_Count >= 2)
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjecttemporal2_" + subject_index + "\" id=\"formsubjecttemporal2_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Temporals[1]) + "\"  onfocus=\"javascript:textbox_enter('formsubjecttemporal2_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjecttemporal2_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }
                    else
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjecttemporal2_" + subject_index + "\" id=\"formsubjecttemporal2_" + subject_index + "\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('formsubjecttemporal2_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjecttemporal2_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }
                    popup_form_builder.AppendLine("</td></tr>");

                    // Add the two geographic term boxes
                    popup_form_builder.Append("    <tr><td>Geographic Term(s):</td><td colspan=\"2\">");
                    if (standSubject.Geographics_Count >= 1)
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjectgeo1_" + subject_index + "\" id=\"formsubjectgeo1_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Geographics[0]) + "\"  onfocus=\"javascript:textbox_enter('formsubjectgeo1_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjectgeo1_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }
                    else
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjectgeo1_" + subject_index + "\" id=\"formsubjectgeo1_" + subject_index + "\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('formsubjectgeo1_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjectgeo1_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }

                    if (standSubject.Geographics_Count >= 2)
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjectgeo2_" + subject_index + "\" id=\"formsubjectgeo2_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Geographics[1]) + "\"  onfocus=\"javascript:textbox_enter('formsubjectgeo2_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjectgeo2_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }
                    else
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjectgeo2_" + subject_index + "\" id=\"formsubjectgeo2_" + subject_index + "\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('formsubjectgeo2_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjectgeo2_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }
                    popup_form_builder.AppendLine("</td></tr>");

                    // Add the two genre term boxes
                    popup_form_builder.Append("    <tr><td>Form / Genre Term(s):</td><td colspan=\"2\">");
                    if (standSubject.Genres_Count >= 1)
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjectgenre1_" + subject_index + "\" id=\"formsubjectgenre1_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Genres[0]) + "\"  onfocus=\"javascript:textbox_enter('formsubjectgenre1_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjectgenre1_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }
                    else
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjectgenre1_" + subject_index + "\" id=\"formsubjectgenre1_" + subject_index + "\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('formsubjectgenre1_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjectgenre1_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }

                    if (standSubject.Genres_Count >= 2)
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjectgenre2_" + subject_index + "\" id=\"formsubjectgenre2_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Genres[1]) + "\"  onfocus=\"javascript:textbox_enter('formsubjectgenre2_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjectgenre2_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }
                    else
                    {
                        popup_form_builder.Append("<input class=\"formsubject_medium_input\" name=\"formsubjectgenre2_" + subject_index + "\" id=\"formsubjectgenre2_" + subject_index + "\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('formsubjectgenre2_" + subject_index + "', 'formsubject_medium_input_focused')\" onblur=\"javascript:textbox_leave('formsubjectgenre2_" + subject_index + "', 'formsubject_medium_input')\" />");
                    }
                    popup_form_builder.AppendLine("</td></tr>");

                    // Add the occupational term box
                    popup_form_builder.Append("    <tr><td>Occupation:</td><td colspan=\"2\">");
                    if (standSubject.Occupations_Count >= 1)
                    {
                        popup_form_builder.Append("<input class=\"formsubject_large_input\" name=\"formsubjectoccup1_" + subject_index + "\" id=\"formsubjectoccup1_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Occupations[0]) + "\"  onfocus=\"javascript:textbox_enter('formsubjectoccup1_" + subject_index + "', 'formsubject_large_input_focused')\" onblur=\"javascript:textbox_leave('formsubjectoccup1_" + subject_index + "', 'formsubject_large_input')\" />");
                    }
                    else
                    {
                        popup_form_builder.Append("<input class=\"formsubject_large_input\" name=\"formsubjectoccup1_" + subject_index + "\" id=\"formsubjectoccup1_" + subject_index + "\" type=\"text\" value=\"\"  onfocus=\"javascript:textbox_enter('formsubjectoccup1_" + subject_index + "', 'formsubject_large_input_focused')\" onblur=\"javascript:textbox_leave('formsubjectoccup1_" + subject_index + "', 'formsubject_large_input')\" />");
                    }
                    popup_form_builder.AppendLine("</td></tr>");

                    // Add the authority and language text boxes
                    popup_form_builder.Append("    <tr><td>Authority:</td><td>");
                    if (standSubject.Authority.Length > 0 )
                    {
                        popup_form_builder.Append("<input class=\"formsubject_small_input\" name=\"formsubjectauthority_" + subject_index + "\" id=\"formsubjectauthority_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Authority) + "\"  onfocus=\"javascript:textbox_enter('formsubjectauthority_" + subject_index + "', 'formsubject_small_input_focused')\" onblur=\"javascript:textbox_leave('formsubjectauthority_" + subject_index + "', 'formsubject_small_input')\" />");
                    }
                    else
                    {
                        popup_form_builder.Append("<input class=\"formsubject_small_input\" name=\"formsubjectauthority_" + subject_index + "\" id=\"formsubjectauthority_" + subject_index + "\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('formsubjectauthority_" + subject_index + "', 'formsubject_small_input_focused')\" onblur=\"javascript:textbox_leave('formsubjectauthority_" + subject_index + "', 'formsubject_small_input')\" />");
                    }
                    popup_form_builder.Append("</td><td width=\"255px\" > &nbsp; Language: &nbsp; ");
                    if (standSubject.Language.Length > 0)
                    {
                        popup_form_builder.Append("<input class=\"formsubject_small_input\" name=\"formsubjectlanguage_" + subject_index + "\" id=\"formsubjectlanguage_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Language) + "\" onfocus=\"javascript:textbox_enter('formsubjectlanguage_" + subject_index + "', 'formsubject_small_input_focused')\" onblur=\"javascript:textbox_leave('formsubjectlanguage_" + subject_index + "', 'formsubject_small_input')\" />");
                    }
                    else
                    {
                        popup_form_builder.Append("<input class=\"formsubject_small_input\" name=\"formsubjectlanguage_" + subject_index + "\" id=\"formsubjectlanguage_" + subject_index + "\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('formsubjectlanguage_" + subject_index + "', 'formsubject_small_input_focused')\" onblur=\"javascript:textbox_leave('formsubjectlanguage_" + subject_index + "', 'formsubject_small_input')\" />");
                    }

                    popup_form_builder.AppendLine("  </td></tr>");
                    popup_form_builder.AppendLine("  </table>");
                    popup_form_builder.AppendLine("  <br />");
                    popup_form_builder.AppendLine("  <center><a href=\"#template\" onclick=\"return close_subject_form('form_subject_" + subject_index + "');\"><img border=\"0\" src=\"" + Close_Button_URL(Skin_Code, Base_URL ) + "\" alt=\"CLOSE\" /></a></center>");
                    popup_form_builder.AppendLine("</div>");
                    popup_form_builder.AppendLine();

                    subject_index++;
                }
            }

            // Add the link to add a new other subject and close the main element
            Output.WriteLine("\n            </div>");
            Output.WriteLine("          </td>");

            Output.WriteLine("          <td valign=\"bottom\" >");
            if (Repeatable)
            {
                Output.WriteLine("            <img title=\"" + Translator.Get_Translation("Click to add a new named subject", CurrentLanguage) + ".\" alt=\"+\" border=\"0px\" class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" onmousedown=\"new_subject_link_clicked('" + Template_Page + "');\" />");
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
        /// <remarks> This clears any preexisting standard subjects ( not hierarchical geographic, title, etc..) </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            if (Bib.Bib_Info.Subjects_Count > 0)
            {
                List<Subject_Info> deletes = Bib.Bib_Info.Subjects.Where(thisSubject => thisSubject.Class_Type == Subject_Info_Type.Standard).ToList();
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
                if (thisKey.IndexOf("formsubjecttype_") == 0) 
                {
                    string diff = thisKey.Replace("formsubjecttype_","");
                    string topic1 = HttpContext.Current.Request.Form["formsubjecttopic1_" + diff].Trim();
                    string topic2 = HttpContext.Current.Request.Form["formsubjecttopic2_" + diff].Trim();
                    string topic3 = HttpContext.Current.Request.Form["formsubjecttopic3_" + diff].Trim();
                    string topic4 = HttpContext.Current.Request.Form["formsubjecttopic4_" + diff].Trim();
                    string temporal1 = HttpContext.Current.Request.Form["formsubjecttemporal1_" + diff].Trim();
                    string temporal2 = HttpContext.Current.Request.Form["formsubjecttemporal2_" + diff].Trim();
                    string geographic1 = HttpContext.Current.Request.Form["formsubjectgeo1_" + diff].Trim();
                    string geographic2 = HttpContext.Current.Request.Form["formsubjectgeo2_" + diff].Trim();
                    string genre1 = HttpContext.Current.Request.Form["formsubjectgenre1_" + diff].Trim();
                    string genre2 = HttpContext.Current.Request.Form["formsubjectgenre2_" + diff].Trim();
                    string occcupation = HttpContext.Current.Request.Form["formsubjectoccup1_" + diff].Trim();
                    string authority = HttpContext.Current.Request.Form["formsubjectauthority_" + diff].Trim();
                    string language = HttpContext.Current.Request.Form["formsubjectlanguage_" + diff].Trim();
                    string marc = HttpContext.Current.Request.Form["formsubjectmap_" + diff].Trim();

                    if ((topic1.Length > 0) || (topic2.Length > 0) || (topic3.Length > 0) || (topic4.Length > 0) ||
                        (temporal1.Length > 0) || (temporal2.Length > 0) || (geographic1.Length > 0) || (geographic2.Length > 0) ||
                        (genre1.Length > 0) || (genre2.Length > 0) || (occcupation.Length > 0))
                    {
                        Subject_Info_Standard newSubject = new Subject_Info_Standard();
                        if (topic1.Length > 0) newSubject.Add_Topic(topic1);
                        if (topic2.Length > 0) newSubject.Add_Topic(topic2);
                        if (topic3.Length > 0) newSubject.Add_Topic(topic3);
                        if (topic4.Length > 0) newSubject.Add_Topic(topic4);
                        if (temporal1.Length > 0) newSubject.Add_Temporal(temporal1);
                        if (temporal2.Length > 0) newSubject.Add_Temporal(temporal2);
                        if (geographic1.Length > 0) newSubject.Add_Geographic(geographic1);
                        if (geographic2.Length > 0) newSubject.Add_Geographic(geographic2);
                        if (genre1.Length > 0) newSubject.Add_Genre(genre1);
                        if (genre2.Length > 0) newSubject.Add_Genre(genre2);
                        if (occcupation.Length > 0) newSubject.Add_Occupation(occcupation);
                        newSubject.Authority = authority;
                        newSubject.Language = language;
                        if ((marc.Length > 0) && (marc != "none"))
                        {
                            newSubject.ID = "SUBJ" + marc + "_" + diff;
                        }
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
