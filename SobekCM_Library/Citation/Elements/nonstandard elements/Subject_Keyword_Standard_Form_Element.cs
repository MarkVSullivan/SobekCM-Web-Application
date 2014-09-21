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
using SobekCM.Core.Users;

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
	        help_page = "subjectform";
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
                const string DEFAULT_ACRONYM = "Enter any subject keyword to describe your material here, along with the vocabulary from which this subject term was pulled.";
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

            // Ensure there is at least one standard subject
            bool found_standard = false;
            if (Bib.Bib_Info.Subjects_Count > 0)
            {
                if (Bib.Bib_Info.Subjects.Any(ThisSubject => ThisSubject.Class_Type == Subject_Info_Type.Standard))
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
                        Output.Write("\n        <a title=\"Click to edit this subject information\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_subject_term_" + subject_index + "')\" onblur=\"link_blurred2('form_subject_term_" + subject_index + "')\" onkeypress=\"return popup_keypress_focus('form_subject_" + subject_index + "', 'formsubjecttopic1_" + subject_index + "', '" + IsMozilla.ToString() + "' );\" onclick=\"return popup_focus('form_subject_" + subject_index + "', 'formsubjecttopic1_" + subject_index + "' );\"><div class=\"form_linkline form_subject_line\" id=\"form_subject_term_" + subject_index + "\">" + standSubject + "</div></a>");
                    else
                        Output.Write("\n        <a title=\"Click to edit this subject information\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_subject_term_" + subject_index + "')\" onblur=\"link_blurred2('form_subject_term_" + subject_index + "')\" onkeypress=\"return popup_keypress_focus('form_subject_" + subject_index + "', 'formsubjecttopic1_" + subject_index + "', '" + IsMozilla.ToString() + "' );\" onclick=\"return popup_focus('form_subject_" + subject_index + "', 'formsubjecttopic1_" + subject_index + "' );\"><div class=\"form_linkline_empty form_subject_line\" id=\"form_subject_term_" + subject_index + "\"><i>Empty Subject Keyword</i></div></a>");

                    // Add the popup form
                    PopupFormBuilder.AppendLine("<!-- Subject Keyword Form " + subject_index + " -->");
					PopupFormBuilder.AppendLine("<div class=\"subject_popup_div sbkMetadata_PopupDiv\" id=\"form_subject_" + subject_index + "\" style=\"display:none;\">");
					PopupFormBuilder.AppendLine("  <div class=\"sbkMetadata_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left\">Edit Subject</td><td style=\"text-align:right\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" alt=\"HELP\" target=\"_" + html_element_name.ToUpper() + "\" >?</a> &nbsp; <a href=\"#template\" alt=\"CLOSE\" onclick=\"close_subject_form('form_subject_" + subject_index + "')\">X</a> &nbsp; </td></tr></table></div>");
                    PopupFormBuilder.AppendLine("  <br />");
					PopupFormBuilder.AppendLine("  <table class=\"sbkMetadata_PopupTable\">");

                    // Add the options for this subject ( standard, name, or title)
                    PopupFormBuilder.Append("    <tr><td>Subject Type:</td><td><select class=\"formsubject_type\" name=\"formsubjecttype_" + subject_index + "\" id=\"formsubjecttype_" + subject_index + "\" >");
                    PopupFormBuilder.Append("<option value=\"standard\" selected=\"selected\">Standard</option>");
                    PopupFormBuilder.AppendLine("</select></td>");
                    PopupFormBuilder.Append("        <td>MARC: &nbsp; <select class=\"formsubject_map\" name=\"formsubjectmap_" + subject_index + "\" id=\"formsubjectmap_" + subject_index + "\" >");

                    PopupFormBuilder.Append(standSubject.ID.IndexOf("SUBJ") < 0
                                                  ? "<option value=\"none\" selected=\"selected\" >&nbsp;</option>"
                                                  : "<option value=\"none\" >&nbsp;</option>");

                    PopupFormBuilder.Append(standSubject.ID.IndexOf("SUBJ648") == 0
                                                  ? "<option value=\"648\" selected=\"selected\" >648 - Chronological Term</option>"
                                                  : "<option value=\"648\" >648 - Chronological Term</option>");

                    PopupFormBuilder.Append(standSubject.ID.IndexOf("SUBJ650") == 0
                                                  ? "<option value=\"650\" selected=\"selected\" >650 - Topical Term</option>"
                                                  : "<option value=\"650\" >650 - Topical Term</option>");

                    PopupFormBuilder.Append(standSubject.ID.IndexOf("SUBJ651") == 0
                                                  ? "<option value=\"651\" selected=\"selected\" >651 - Geographic Name</option>"
                                                  : "<option value=\"651\" >651 - Geographic Name</option>");

                    PopupFormBuilder.Append(standSubject.ID.IndexOf("SUBJ653") == 0
                                                  ? "<option value=\"653\" selected=\"selected\" >653 - Uncontrolled Index</option>"
                                                  : "<option value=\"653\" >653 - Uncontrolled Index</option>");

                    PopupFormBuilder.Append(standSubject.ID.IndexOf("SUBJ654") == 0
                                                  ? "<option value=\"654\" selected=\"selected\" >654 - Faceted Topical</option>"
                                                  : "<option value=\"654\" >654 - Faceted Topical</option>");

                    PopupFormBuilder.Append(standSubject.ID.IndexOf("SUBJ655") == 0
                                                  ? "<option value=\"655\" selected=\"selected\" >655 - Genre / Form</option>"
                                                  : "<option value=\"655\" >655 - Genre / Form</option>");

                    PopupFormBuilder.Append(standSubject.ID.IndexOf("SUBJ656") == 0
                                                  ? "<option value=\"656\" selected=\"selected\" >656 - Occupation</option>"
                                                  : "<option value=\"656\" >656 - Occupation</option>");

                    PopupFormBuilder.Append(standSubject.ID.IndexOf("SUBJ657") == 0
                                                  ? "<option value=\"657\" selected=\"selected\" >657 - Function</option>"
                                                  : "<option value=\"657\" >657 - Function</option>");

                    PopupFormBuilder.Append(standSubject.ID.IndexOf("SUBJ690") == 0
                                                  ? "<option value=\"690\" selected=\"selected\" >690 - Local Topical</option>"
                                                  : "<option value=\"690\" >690 - Local Topical</option>");

                    PopupFormBuilder.Append(standSubject.ID.IndexOf("SUBJ691") == 0
                                                  ? "<option value=\"691\" selected=\"selected\" >691 - Local Geographic</option>"
                                                  : "<option value=\"691\" >691 - Local Geographic</option>");

                    PopupFormBuilder.AppendLine("</select></td></tr>");

                    // Add the first row of topical term boxes
                    PopupFormBuilder.Append("    <tr><td>Topical Term(s):</td><td colspan=\"2\">");
                    if (standSubject.Topics_Count >= 1)
                    {
                        PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjecttopic1_" + subject_index + "\" id=\"formsubjecttopic1_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Topics[0]) + "\" />");
                    }
                    else
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjecttopic1_" + subject_index + "\" id=\"formsubjecttopic1_" + subject_index + "\" type=\"text\" value=\"\" />");                    
                    }

                    if (standSubject.Topics_Count >= 2)
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjecttopic2_" + subject_index + "\" id=\"formsubjecttopic2_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Topics[1]) + "\" />");
                    }
                    else
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjecttopic2_" + subject_index + "\" id=\"formsubjecttopic2_" + subject_index + "\" type=\"text\" value=\"\" />");
                    }
                    PopupFormBuilder.AppendLine("</td></tr>");

                    // Add the second row of topical term boxes
                    PopupFormBuilder.Append("    <tr><td>&nbsp;</td><td colspan=\"2\">");
                    if (standSubject.Topics_Count >= 3)
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjecttopic3_" + subject_index + "\" id=\"formsubjecttopic3_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Topics[2]) + "\" />");
                    }
                    else
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjecttopic3_" + subject_index + "\" id=\"formsubjecttopic3_" + subject_index + "\" type=\"text\" value=\"\" />");
                    }

                    if (standSubject.Topics_Count >= 4)
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjecttopic4_" + subject_index + "\" id=\"formsubjecttopic4_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Topics[3]) + "\" />");
                    }
                    else
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjecttopic4_" + subject_index + "\" id=\"formsubjecttopic4_" + subject_index + "\" type=\"text\" value=\"\" />");
                    }
                    PopupFormBuilder.AppendLine("</td></tr>");

                    // Add the two chronological term boxes
                    PopupFormBuilder.Append("    <tr><td>Chronological Term(s):</td><td colspan=\"2\">");
                    if (standSubject.Temporals_Count >= 1)
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjecttemporal1_" + subject_index + "\" id=\"formsubjecttemporal1_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Temporals[0]) + "\" />");
                    }
                    else
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjecttemporal1_" + subject_index + "\" id=\"formsubjecttemporal1_" + subject_index + "\" type=\"text\" value=\"\" />");
                    }

                    if (standSubject.Temporals_Count >= 2)
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjecttemporal2_" + subject_index + "\" id=\"formsubjecttemporal2_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Temporals[1]) + "\" />");
                    }
                    else
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjecttemporal2_" + subject_index + "\" id=\"formsubjecttemporal2_" + subject_index + "\" type=\"text\" value=\"\" />");
                    }
                    PopupFormBuilder.AppendLine("</td></tr>");

                    // Add the two geographic term boxes
                    PopupFormBuilder.Append("    <tr><td>Geographic Term(s):</td><td colspan=\"2\">");
                    if (standSubject.Geographics_Count >= 1)
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjectgeo1_" + subject_index + "\" id=\"formsubjectgeo1_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Geographics[0]) + "\" />");
                    }
                    else
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjectgeo1_" + subject_index + "\" id=\"formsubjectgeo1_" + subject_index + "\" type=\"text\" value=\"\" />");
                    }

                    if (standSubject.Geographics_Count >= 2)
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjectgeo2_" + subject_index + "\" id=\"formsubjectgeo2_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Geographics[1]) + "\" />");
                    }
                    else
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjectgeo2_" + subject_index + "\" id=\"formsubjectgeo2_" + subject_index + "\" type=\"text\" value=\"\" />");
                    }
                    PopupFormBuilder.AppendLine("</td></tr>");

                    // Add the two genre term boxes
                    PopupFormBuilder.Append("    <tr><td>Form / Genre Term(s):</td><td colspan=\"2\">");
                    if (standSubject.Genres_Count >= 1)
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjectgenre1_" + subject_index + "\" id=\"formsubjectgenre1_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Genres[0]) + "\" />");
                    }
                    else
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjectgenre1_" + subject_index + "\" id=\"formsubjectgenre1_" + subject_index + "\" type=\"text\" value=\"\" />");
                    }

                    if (standSubject.Genres_Count >= 2)
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjectgenre2_" + subject_index + "\" id=\"formsubjectgenre2_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Genres[1]) + "\" />");
                    }
                    else
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_medium_input sbk_Focusable\" name=\"formsubjectgenre2_" + subject_index + "\" id=\"formsubjectgenre2_" + subject_index + "\" type=\"text\" value=\"\" />");
                    }
                    PopupFormBuilder.AppendLine("</td></tr>");

                    // Add the occupational term box
                    PopupFormBuilder.Append("    <tr><td>Occupation:</td><td colspan=\"2\">");
                    if (standSubject.Occupations_Count >= 1)
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_large_input sbk_Focusable\" name=\"formsubjectoccup1_" + subject_index + "\" id=\"formsubjectoccup1_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Occupations[0]) + "\" />");
                    }
                    else
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_large_input sbk_Focusable\" name=\"formsubjectoccup1_" + subject_index + "\" id=\"formsubjectoccup1_" + subject_index + "\" type=\"text\" value=\"\" />");
                    }
                    PopupFormBuilder.AppendLine("</td></tr>");

                    // Add the authority and language text boxes
                    PopupFormBuilder.Append("    <tr><td>Authority:</td><td>");
                    if (standSubject.Authority.Length > 0 )
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_small_input sbk_Focusable\" name=\"formsubjectauthority_" + subject_index + "\" id=\"formsubjectauthority_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Authority) + "\" />");
                    }
                    else
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_small_input sbk_Focusable\" name=\"formsubjectauthority_" + subject_index + "\" id=\"formsubjectauthority_" + subject_index + "\" type=\"text\" value=\"\" />");
                    }
                    PopupFormBuilder.Append("</td><td width=\"255px\" > &nbsp; Language: &nbsp; ");
                    if (standSubject.Language.Length > 0)
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_small_input sbk_Focusable\" name=\"formsubjectlanguage_" + subject_index + "\" id=\"formsubjectlanguage_" + subject_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(standSubject.Language) + "\" />");
                    }
                    else
                    {
						PopupFormBuilder.Append("<input class=\"formsubject_small_input sbk_Focusable\" name=\"formsubjectlanguage_" + subject_index + "\" id=\"formsubjectlanguage_" + subject_index + "\" type=\"text\" value=\"\" />");
                    }
					PopupFormBuilder.AppendLine("  </td></tr>");

					// Finish the popup form and add the CLOSE button
					PopupFormBuilder.AppendLine("    <tr style=\"height:35px; text-align: center; vertical-align: bottom;\">");
					PopupFormBuilder.AppendLine("      <td colspan=\"3\"><button title=\"Close\" class=\"sbkMetadata_RoundButton\" onclick=\"return close_subject_form('form_subject_" + subject_index + "');\">CLOSE</button></td>");
					PopupFormBuilder.AppendLine("    </tr>");
					PopupFormBuilder.AppendLine("  </table>");
					PopupFormBuilder.AppendLine("</div>");
					PopupFormBuilder.AppendLine();

                    subject_index++;
                }
            }

            // Add the link to add a new other subject and close the main element
            Output.WriteLine("\n            </div>");
            Output.WriteLine("          </td>");

            Output.WriteLine("          <td style=\"vertical-align:bottom\" >");
            if (Repeatable)
            {
                Output.WriteLine("            <img title=\"" + Translator.Get_Translation("Click to add a new named subject", CurrentLanguage) + ".\" alt=\"+\" class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" onmousedown=\"new_subject_link_clicked('" + Template_Page + "');\" />");
            }
            Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img class=\"help_button\" src=\"" + Base_URL + HELP_BUTTON_URL + "\" /></a>");
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
        /// <param name="XMLReader"> Current template xml configuration reader </param>
        /// <remarks> This procedure does not currently read any inner xml (not yet necessary) </remarks>
        protected override void Inner_Read_Data(XmlTextReader XMLReader)
        {
            // Do nothing
        }

        #endregion
    }
}
