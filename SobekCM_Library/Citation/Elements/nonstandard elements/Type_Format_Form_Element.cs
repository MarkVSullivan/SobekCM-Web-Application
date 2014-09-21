#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    /// <summary> Element displays a form to allow for complete entry of the type information and access to many of the fields present in the MARC leader and 006-008 fields for an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    public class Type_Format_Form_Element : simpleTextBox_Element
    {
        ///// <summary> Protected field holds the default value(s) </summary>
        //protected List<string> default_values;

        /// <summary> Protected field holds all the possible, selectable values </summary>
        protected List<string> items;

        private string onChange;

        /// <summary> Constructor for a new instance of the Type_Format_Form_Element class </summary>
        public Type_Format_Form_Element()
            : base("Resource Type", "type_format_form")
        {
            items = new List<string>();
            default_values = new List<string>();

            Repeatable = false;
            Type = Element_Type.Type;
            Display_SubType = "form";
	        help_page = "type";
        }

        /// <summary> Sets the postback javascript, if the combo box requires a post back onChange </summary>
        /// <param name="postback_call"> Javascript call to perform onChange </param>
        public void Set_Postback(string postback_call)
        {
            onChange = postback_call;
        }

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
                const string defaultAcronym = "Select the resource type information which best describes this material.";
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

             // Determine the material type
            string instance_value = "Select Material Type";
            bool initial_value = true;
            string thisType = Bib.Bib_Info.SobekCM_Type_String;
            if (thisType.Length > 0)
            {
                if (thisType.ToUpper() != "PROJECT")
                {
                    instance_value = thisType;
                    initial_value = false;
                }
                else
                {
                    if (Bib.Bib_Info.Notes_Count > 0)
                    {
                        foreach (Note_Info thisNote in Bib.Bib_Info.Notes)
                        {
                            if (thisNote.Note_Type == Note_Type_Enum.default_type)
                            {
                                instance_value = thisNote.Note;
                                initial_value = false;
                                break;
                            }
                        }
                    }
                }
            }

            if ((instance_value.Length == 0) && (default_values.Count > 0))
            {
                instance_value = default_values[0];
            }

            // Render the title
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
            Output.WriteLine("            <div id=\"" + html_element_name + "_div\">");

            // Start the select combo box
            if (onChange.Length > 0)
            {
                if (initial_value)
                {
                    Output.WriteLine("              <select class=\"" + html_element_name + "_select_init\" name=\"form_typeformat_type\" id=\"form_typeformat_type\" onChange=\"" + onChange + "\" >");
                }
                else
                {
                    Output.WriteLine("              <select class=\"" + html_element_name + "_select\" name=\"form_typeformat_type\" id=\"form_typeformat_type\" onChange=\"" + onChange + "\" >");
                }
            }
            else
            {
                if (initial_value)
                {
                    Output.WriteLine("              <select class=\"" + html_element_name + "_select_init\" name=\"form_typeformat_type\" id=\"form_typeformat_type\" >");
                }
                else
                {
                    Output.WriteLine("              <select class=\"" + html_element_name + "_select\" name=\"form_typeformat_type\" id=\"form_typeformat_type\" >");
                }
            }

			// Correction for aerials
	        if (instance_value == "Aerial Photography")
		        instance_value = "Aerial";

            bool found_option = false;
            foreach (string thisOption in items)
            {
                if (thisOption == instance_value)
                {
                    Output.WriteLine("                <option selected=\"selected=\" value=\"" + thisOption + "\">" + thisOption + "</option>");
                    found_option = true;
                }
                else
                {
                    Output.WriteLine("                <option value=\"" + thisOption + "\">" + thisOption + "</option>");
                }
            }
            if ((instance_value.Length > 0) && (!found_option))
            {
                Output.WriteLine("                <option selected=\"selected=\" value=\"" + instance_value + "\">" + instance_value + "</option>");
            }
            Output.WriteLine("              </select>");

            #region Determine the values to display for this bib id

            // Collect the data to show
            bool festschrift = false;
            bool indexPresent = false;
            bool conferencePublication = false;
            string targetAudience1 = String.Empty;
			string frequency1 = String.Empty;
            string frequency2 = String.Empty;
            string regularity1 = String.Empty;
            string regularity2 = String.Empty;

            string nature1 = String.Empty;
            string nature2 = String.Empty;
            string nature3 = String.Empty;
            string nature4 = String.Empty;
            string govt = String.Empty;
            string literaryform1 = String.Empty;
            string literaryform2 = String.Empty;
            string biography = String.Empty;
            string subtype = String.Empty;
            string projection = String.Empty;
            string scale = String.Empty;
            string place_code = String.Empty;
            string language_code = String.Empty;
			string computerfileform = String.Empty;

            // Determine which form will be used and set several values
            Type_Format_Type_Enum formType = Type_Format_Type_Enum.None;
            string class_name = "typeformat_popup_div";
            string form_title = "Edit Material Details";
            int windowheight = 270;
            switch (instance_value.ToUpper())
            {
                case "BOOK":
                    formType = Type_Format_Type_Enum.Book;
                    class_name = "typeformat_book_popup_div";
                    form_title = "Edit Book Material Details";
                    windowheight = 515 - 100; // Fudge to move the form down a bit
                    break;

                case "AERIAL":
                case "ARTIFACT":
                case "PHOTOGRAPH":
                case "VIDEO":
                    formType = Type_Format_Type_Enum.Visual_Materials;
                    class_name = "typeformat_visual_popup_div";
                    form_title = "Edit Visual Material Details";
                    windowheight = 390;
                    break;

                case "SERIAL":
                case "NEWSPAPER":
                    formType = Type_Format_Type_Enum.Continuing_Resource;
                    class_name = "typeformat_serial_popup_div";
                    form_title = "Edit Continuing Resources Material Details";
                    windowheight = 430;
                    break;

                case "MAP":
                    formType = Type_Format_Type_Enum.Map;
                    class_name = "typeformat_map_popup_div";
                    form_title = "Edit Map Materials Details";
                    windowheight = 430;
                    break;

				case "DATASET":
					formType = Type_Format_Type_Enum.Computer_Files;
					class_name = "typeformat_computer_popup_div";
					form_title = "Edit Computer File Details";
					windowheight = 430;
					break;

            }


            // List of possible audiences (marcgt)
            List<string> audiences_list = new List<string>(8)
                                              {
                                                  "adolescent",
                                                  "adult",
                                                  "general",
                                                  "juvenile",
                                                  "pre-adolescent",
                                                  "preschool",
                                                  "primary",
                                                  "specialized"
                                              };

            // List of possible nature of contents
            List<string> nature_contents_list = new List<string>(26)
                                                    {
                                                        "abstract or summary",
                                                        "bibliography",
                                                        "calendar",
                                                        "catalog",
                                                        "comic/graphic novel",
                                                        "dictionary",
                                                        "directory",
                                                        "discography",
                                                        "filmography",
                                                        "handbook",
                                                        "index",
                                                        "law report or digest",
                                                        "legal article",
                                                        "legal case and case notes",
                                                        "legislation",
                                                        "offprint",
                                                        "patent",
                                                        "programmed text",
                                                        "review",
                                                        "statistics",
                                                        "survey of literature",
                                                        "technical report",
                                                        "theses",
                                                        "treaty",
                                                        "yearbook"
                                                    };

            // List of government genres
            List<string> government_list = new List<string>(8)
                                               {
                                                   "federal government publication",
                                                   "international intergovernmental publication",
                                                   "local government publication",
                                                   "government publication",
                                                   "government publication (autonomous or semiautonomous component)",
                                                   "government publication (state, provincial, terriorial, dependent)",
                                                   "multilocal government publication",
                                                   "multistate government publication"
                                               };

            // Get list of literary forms
            List<string> literary_form_list = new List<string>(11)
                                                  {
                                                      "comic strip",
                                                      "drama",
                                                      "essay",
                                                      "fiction",
                                                      "humor, satire",
                                                      "letter",
                                                      "novel",
                                                      "non-fiction",
                                                      "poetry",
                                                      "short story",
                                                      "speech"
                                                  };

			// Get list of literary forms
			List<string> computer_file_form_list = new List<string>(12)
                                                  {
                                                      "numeric data",
                                                      "computer program",
                                                      "representational",
                                                      "document",
                                                      "bibliographic data",
                                                      "font",
                                                      "game",
                                                      "sound",
                                                      "interactive multimedia",
                                                      "online system or service",
                                                      "combination",
													  "other computer file"
                                                  };

            // Get list of biography type information
            List<string> biography_list = new List<string>(3)
                                              {"autobiography", "individual biography", "collective biography"};

            // Get list of map subtypes
            List<string> map_subtypes = new List<string>(5) {"atlas", "globe", "map serial", "map series", "single map"};

            // Get the list of visual materials subtypes
            List<string> visual_subtypes = new List<string>(17)
                                               {
                                                   "art original",
                                                   "art reproduction",
                                                   "chart",
                                                   "diorama",
                                                   "filmstrip",
                                                   "flash card",
                                                   "graphic",
                                                   "kit",
                                                   "microscope slide",
                                                   "model",
                                                   "motion picture",
                                                   "picture",
                                                   "realia",
                                                   "slide",
                                                   "technical drawing",
                                                   "toy",
                                                   "transparency"
                                               };

            // Get the list of continuing materials subtypes
            List<string> continuing_subtypes = new List<string>(6)
                                                   {
                                                       "database",
                                                       "loose-leaf",
                                                       "newspaper",
                                                       "periodical",
                                                       "series",
                                                       "web site"
                                                   };

            // Get the list of frequency
            List<string> frequency_list = new List<string>(17)
                                              {
                                                  "annual",
                                                  "biennial",
                                                  "bimonthly",
                                                  "biweekly",
                                                  "continuously updated",
                                                  "daily",
                                                  "monthly",
                                                  "other",
                                                  "quarterly",
                                                  "semiannual",
                                                  "semimonthly",
                                                  "semiweekly",
                                                  "three times a month",
                                                  "three times a week",
                                                  "three times a year",
                                                  "triennial",
                                                  "weekly"
                                              };

            // Get the list of regularities
            List<string> regularity_list = new List<string>(3)
                                               {"completely irregular", "normalized irregular", "regular"};

            // Get the place code
            if (Bib.Bib_Info.Origin_Info.Places_Count > 0)
            {
                foreach (Origin_Info_Place thisPlace in Bib.Bib_Info.Origin_Info.Places.Where(thisPlace => thisPlace.Place_MarcCountry.Length > 0))
                {
                    place_code = thisPlace.Place_MarcCountry;
                    break;
                }
            }

            // Get the language code
            if (Bib.Bib_Info.Languages_Count > 0)
            {
                foreach (Language_Info thisLanguage in Bib.Bib_Info.Languages.Where(thisLanguage => thisLanguage.Language_ISO_Code.Length > 0))
                {
                    language_code = thisLanguage.Language_ISO_Code;
                    break;
                }
            }

            // Set the target audience
            if (Bib.Bib_Info.Target_Audiences_Count > 0)
            {
                foreach (TargetAudience_Info audience in Bib.Bib_Info.Target_Audiences.Where(audience => audiences_list.Contains(audience.Audience.ToLower())))
                {
                    if (targetAudience1.Length == 0)
                    {
                        targetAudience1 = audience.Audience.ToLower();
                        break;
                    }
                }
            }

            // Set the frequencies and regularities
            if (Bib.Bib_Info.Origin_Info.Frequencies_Count > 0)
            {
                foreach (Origin_Info_Frequency thisFrequency in Bib.Bib_Info.Origin_Info.Frequencies)
                {
                    // Check for frequency information
                    if (frequency_list.Contains(thisFrequency.Term.ToLower()))
                    {
                        if (frequency1.Length == 0)
                        {
                            frequency1 = thisFrequency.Term.ToLower();
                        }
                        else
                        {
                            if (frequency2.Length == 0)
                            {
                                frequency2 = thisFrequency.Term.ToLower();
                            }
                        }
                    }

                    // Check for regularity information
                    if (regularity_list.Contains(thisFrequency.Term.ToLower()))
                    {
                        if (regularity1.Length == 0)
                        {
                            regularity1 = thisFrequency.Term.ToLower();
                        }
                        else
                        {
                            if (regularity2.Length == 0)
                            {
                                regularity2 = thisFrequency.Term.ToLower();
                            }
                        }
                    }
                }
            }

            // Step through each genre
            int nature_index = 1;
            int literary_form_index = 1;
            if (Bib.Bib_Info.Genres_Count > 0)
            {
                foreach (Genre_Info thisGenre in Bib.Bib_Info.Genres)
                {
                    if ((thisGenre.Genre_Term == "conference publication") || (thisGenre.Genre_Term == "festschrift") || (thisGenre.Genre_Term == "indexed"))
                    {
                        if (thisGenre.Genre_Term == "conference publication")
                            conferencePublication = true;

                        if (thisGenre.Genre_Term == "festschrift")
                            festschrift = true;

                        if (thisGenre.Genre_Term == "indexed")
                            indexPresent = true;
                    }
                    else
                    {
                        string genre_term_lower = thisGenre.Genre_Term.ToLower();

                        if (biography_list.Contains(genre_term_lower))
                            biography = genre_term_lower;

                        if (government_list.Contains(genre_term_lower))
                            govt = genre_term_lower;

                        if (formType == Type_Format_Type_Enum.Continuing_Resource)
                        {
                            if (continuing_subtypes.Contains(genre_term_lower))
                                subtype = genre_term_lower;
                        }

                        if (formType == Type_Format_Type_Enum.Visual_Materials)
                        {
                            if (visual_subtypes.Contains(genre_term_lower))
                                subtype = genre_term_lower;
                        }

                        if (formType == Type_Format_Type_Enum.Map)
                        {
                            if (map_subtypes.Contains(genre_term_lower))
                                subtype = genre_term_lower;
                        }

						if (formType == Type_Format_Type_Enum.Computer_Files)
						{
							if (computer_file_form_list.Contains(genre_term_lower))
								computerfileform = genre_term_lower;
						}

                        if (nature_contents_list.Contains(genre_term_lower))
                        {
                            switch (nature_index)
                            {
                                case 1:
                                    nature1 = genre_term_lower;
                                    break;

                                case 2:
                                    nature2 = genre_term_lower;
                                    break;

                                case 3:
                                    nature3 = genre_term_lower;
                                    break;

                                case 4:
                                    nature4 = genre_term_lower;
                                    break;
                            }

                            nature_index++;
                        }

                        if (literary_form_list.Contains(genre_term_lower))
                        {
                            switch (literary_form_index)
                            {
                                case 1:
                                    literaryform1 = genre_term_lower;
                                    break;

                                case 2:
                                    literaryform2 = genre_term_lower;
                                    break;
                            }

                            literary_form_index++;
                        }
                    }
                }
            }

            // If this is map, get the projection and scale
            if (formType == Type_Format_Type_Enum.Map)
            {
                // Step through each cartographic
                string scale_034 = String.Empty;
                string scale_255 = String.Empty;
                if ( Bib.Bib_Info.Subjects_Count > 0 )
                {
                    foreach (Subject_Info thisSubject in Bib.Bib_Info.Subjects)
                    {
                        if (thisSubject.Class_Type == Subject_Info_Type.Cartographics)
                        {
                            Subject_Info_Cartographics carto = (Subject_Info_Cartographics)thisSubject;
                            if (carto.Scale.Length > 0)
                            {
                                if (carto.ID.IndexOf("SUBJ034") >= 0)
                                    scale_034 = carto.Scale;
                                else
                                {
                                    if (carto.ID.IndexOf("SUBJ255") >= 0)
                                        scale_255 = carto.Scale;
                                    else
                                        scale = carto.Scale;
                                }
                            }
                        }
                    }
                }

                if (scale_255.Length > 0)
                {
                    scale = scale_255.Replace("Scale", "").Replace("ca.", "").Trim();
                }

                if (scale_034.Length > 0)
                {
                    scale = "1:" + scale_034;
                }
            }


            #endregion

            #region Render the link in the template form

            // Render the label with additional information
            string additional_info = show_material_type_details(Bib);
            string style = "form_linkline";
            if (additional_info.Length == 0)
            {
                additional_info = "<i>no additional type information<i>";
                style = "form_linkline_empty";
            }
            if (additional_info.Length > 60)
                additional_info = additional_info.Substring(0, 60) + "...";
            Output.WriteLine("        &nbsp; &nbsp; ");
            Output.WriteLine("              <a title=\"Click to edit the material details\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_typeformat_term')\" onblur=\"link_blurred2('form_typeformat_term')\" onkeypress=\"return popup_keypress_focus('form_typeformat', 'form_typeformat_term', 'form_typeformat_extent', " + windowheight + ", 675, '" + IsMozilla.ToString() + "' );\" onclick=\"return popup_focus('form_typeformat', 'form_typeformat_term', 'form_typeformat_extent', " + windowheight + ", 675 );\"><span class=\"" + style + " form_typeformat_line\" id=\"form_typeformat_term\">" + additional_info + "</span></a>");
            Output.WriteLine("            </div>");
            Output.WriteLine("          </td>");
            Output.WriteLine("          <td style=\"vertical-align:bottom\" >");
            Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img class=\"help_button\" src=\"" + Base_URL + HELP_BUTTON_URL + "\" /></a>");
            Output.WriteLine("          </td>");
            Output.WriteLine("        </tr>");
            Output.WriteLine("      </table>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine();

            #endregion

            #region Create the pop-up form for the All Materials section

            // Add the popup form
            PopupFormBuilder.AppendLine("<!-- Type Format Form -->");
			PopupFormBuilder.AppendLine("<div class=\"" + class_name + " sbkMetadata_PopupDiv\" id=\"form_typeformat\" style=\"display:none;\">");
			PopupFormBuilder.AppendLine("  <div class=\"sbkMetadata_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left\">" + form_title + "</td><td style=\"text-align:right\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" alt=\"HELP\" target=\"_" + html_element_name.ToUpper() + "\" >?</a> &nbsp; <a href=\"#template\" alt=\"CLOSE\" onclick=\"close_typeformat_form()\">X</a> &nbsp; </td></tr></table></div>");
            PopupFormBuilder.AppendLine("  <br />");
			PopupFormBuilder.AppendLine("  <table class=\"sbkMetadata_PopupTable\">");

            // Add the all materials title
            PopupFormBuilder.AppendLine("    <tr><td colspan=\"3\" class=\"SobekEditItemSectionTitle_first\" >All Materials</td></tr>");

            // Add the extent information
            PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Physical Desc:</td><td colspan=\"2\">");
            PopupFormBuilder.Append("<input class=\"formtype_large_input sbk_Focusable\" name=\"form_typeformat_extent\" id=\"form_typeformat_extent\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Original_Description.Extent) + "\" />");
            PopupFormBuilder.AppendLine("</td></tr>");

            // Add the date range info
            PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Year Range:</td><td colspan=\"2\">");
			PopupFormBuilder.Append("<span class=\"metadata_sublabel2\">Start Year:</span><input class=\"formtype_small_input sbk_Focusable\" name=\"form_typeformat_datestart\" id=\"form_typeformat_datestart\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Origin_Info.MARC_DateIssued_Start) + "\" />");
			PopupFormBuilder.Append("<span class=\"metadata_sublabel\">End Year:</span><input class=\"formtype_small_input sbk_Focusable\" name=\"form_typeformat_dateend\" id=\"form_typeformat_dateend\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Origin_Info.MARC_DateIssued_End) + "\" />");
            PopupFormBuilder.AppendLine("</td></tr>");

            // Add the place code info
            PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Place Code:</td><td colspan=\"2\">");
			PopupFormBuilder.Append("<input class=\"formtype_small_input sbk_Focusable\" name=\"form_typeformat_placecode\" id=\"form_typeformat_placecode\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(place_code) + "\" />");
            PopupFormBuilder.AppendLine("</td></tr>");

            // Add the language code info
            PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Language Code:</td><td colspan=\"2\">");
			PopupFormBuilder.Append("<input class=\"formtype_small_input sbk_Focusable\" name=\"form_typeformat_langcode\" id=\"form_typeformat_langcode\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(language_code) + "\" />");
            PopupFormBuilder.AppendLine("</td></tr>");

            #endregion

            #region Render the 'Book Details' section if this is a book

            if (formType ==  Type_Format_Type_Enum.Book)
            {
                // Add the book details title
                PopupFormBuilder.AppendLine("    <tr><td colspan=\"3\" class=\"SobekEditItemSectionTitle\" >Book Details</td></tr>");

                // Add the target audience
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Target Audience:</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_small_select\" name=\"form_typeformat_audience\" id=\"form_typeformat_audience\" >");
                PopupFormBuilder.Append(targetAudience1.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisAudience in audiences_list)
                {
                    if ( targetAudience1 == thisAudience )
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisAudience + "\" selected=\"selected\">" + thisAudience +"</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisAudience + "\">" + thisAudience + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");
                PopupFormBuilder.AppendLine("</td></tr>");

                // Add the nature of contents
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Nature of contents:</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_nature1\" id=\"form_typeformat_nature1\" >");
                PopupFormBuilder.Append(nature1.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in nature_contents_list)
                {
                    if (nature1 == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");

                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_nature2\" id=\"form_typeformat_nature2\" >");
                PopupFormBuilder.Append(nature2.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in nature_contents_list)
                {
                    if (nature2 == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");

                PopupFormBuilder.AppendLine("</td></tr>");

                PopupFormBuilder.Append("    <tr><td>&nbsp;</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_nature3\" id=\"form_typeformat_nature3\" >");
                PopupFormBuilder.Append(nature3.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in nature_contents_list)
                {
                    if (nature3 == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");

                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_nature4\" id=\"form_typeformat_nature4\" >");
                PopupFormBuilder.Append(nature4.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in nature_contents_list)
                {
                    if (nature4 == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");

              
                PopupFormBuilder.AppendLine("</td></tr>");

                // Add the government publication 
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Government:</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_large_select\" name=\"form_typeformat_govt\" id=\"form_typeformat_govt\" >");
                PopupFormBuilder.Append(govt.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in government_list)
                {
                    if (govt == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");

                PopupFormBuilder.AppendLine("</td></tr>");

                // Add the checkboxes
                PopupFormBuilder.Append("    <tr><td colspan=\"3\"> &nbsp; &nbsp; ");
                PopupFormBuilder.Append(conferencePublication
                                              ? "Conference Publication: <input type=\"checkbox\" name=\"form_typeformat_conference\" checked=\"checked\" /> &nbsp; &nbsp; &nbsp; &nbsp; "
                                              : "Conference Publication: <input type=\"checkbox\" name=\"form_typeformat_conference\" /> &nbsp; &nbsp; &nbsp; &nbsp; ");

                PopupFormBuilder.Append(festschrift
                                              ? "Festschrift: <input type=\"checkbox\" name=\"form_typeformat_festschrift\" checked=\"checked\" /> &nbsp; &nbsp; &nbsp; &nbsp; "
                                              : "Festschrift: <input type=\"checkbox\" name=\"form_typeformat_festschrift\" /> &nbsp; &nbsp; &nbsp; &nbsp; ");

                PopupFormBuilder.Append(indexPresent
                                              ? "Index Present: <input type=\"checkbox\" name=\"form_typeformat_index\" checked=\"checked\" />"
                                              : "Index Present: <input type=\"checkbox\" name=\"form_typeformat_index\" />");

                PopupFormBuilder.AppendLine("</td></tr>");

                // Add the literary form
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Literary Form:</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_literary1\" id=\"form_typeformat_literary1\" >");
                PopupFormBuilder.Append(literaryform1.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in literary_form_list)
                {
                    if (literaryform1 == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");

                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_literary2\" id=\"form_typeformat_literary2\" >");
                PopupFormBuilder.Append(literaryform2.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in literary_form_list)
                {
                    if (literaryform2 == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");
                PopupFormBuilder.AppendLine("</td></tr>");                

                // Add the bibliography info
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Biography:</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_biography\" id=\"form_typeformat_biography\" >");
                PopupFormBuilder.Append(biography.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in biography_list)
                {
                    if (biography == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");
                PopupFormBuilder.AppendLine("</td></tr>");
            }

            #endregion

            #region Render the 'Visual Materials' section if necessary

            if (formType == Type_Format_Type_Enum.Visual_Materials)
            {
                // Add the book details title
                PopupFormBuilder.AppendLine("    <tr><td colspan=\"3\" class=\"SobekEditItemSectionTitle\">Visual Materials Details</td></tr>");

                // Add the target audience
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Target Audience:</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_small_select\" name=\"form_typeformat_audience\" id=\"form_typeformat_audience\" >");
                PopupFormBuilder.Append(targetAudience1.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisAudience in audiences_list)
                {
                    if (targetAudience1 == thisAudience)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisAudience + "\" selected=\"selected\">" + thisAudience + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisAudience + "\">" + thisAudience + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");
                PopupFormBuilder.AppendLine("</td></tr>");

                // Add the subtype
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Sub-type:</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_subtype\" id=\"form_typeformat_subtype\" >");
                PopupFormBuilder.Append(subtype.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in visual_subtypes)
                {
                    if (subtype == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");
                PopupFormBuilder.AppendLine("</td></tr>");

                
                // Add the government publication 
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Government:</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_large_select\" name=\"form_typeformat_govt\" id=\"form_typeformat_govt\" >");
                PopupFormBuilder.Append(govt.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in government_list)
                {
                    if (govt == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");

                PopupFormBuilder.AppendLine("</td></tr>");
            }

            #endregion

            #region Render the 'Map Materials' section if necessary

            if (formType == Type_Format_Type_Enum.Map)
            {
                // Add the book details title
                PopupFormBuilder.AppendLine("    <tr><td colspan=\"3\" class=\"SobekEditItemSectionTitle\">Map Details</td></tr>");

                // Add the projection code and scale
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Projection Code:</td><td>");
				PopupFormBuilder.Append("<input class=\"formtype_small_input sbk_Focusable\" name=\"form_typeformat_projcode\" id=\"form_typeformat_projcode\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(projection) + "\" /></td>");
				PopupFormBuilder.AppendLine("<td width=\"250px\">Scale: &nbsp; &nbsp; <input class=\"formtype_small_input sbk_Focusable\" name=\"form_typeformat_scale\" id=\"form_typeformat_scale\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(scale) + "\" /></td></tr>");

                // Add the subtype
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Sub-type:</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_subtype\" id=\"form_typeformat_subtype\" >");
                PopupFormBuilder.Append(subtype.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in map_subtypes)
                {
                    if (subtype == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");
                PopupFormBuilder.AppendLine("</td></tr>");


                // Add the government publication 
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Government:</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_large_select\" name=\"form_typeformat_govt\" id=\"form_typeformat_govt\" >");
                PopupFormBuilder.Append(govt.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in government_list)
                {
                    if (govt == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");
                PopupFormBuilder.AppendLine("</td></tr>");

                // Add the checkboxes
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Index Present:</td>");
                PopupFormBuilder.Append(indexPresent
                                              ? "<td colspan=\"2\"><input type=\"checkbox\" name=\"form_typeformat_index\" checked=\"checked\" />"
                                              : "<td colspan=\"2\"><input type=\"checkbox\" name=\"form_typeformat_index\" />");

                PopupFormBuilder.Append("</td></tr>");
            }

            #endregion

            #region Render the 'Continuing Resources' section if necessary

            if (formType == Type_Format_Type_Enum.Continuing_Resource)
            {
                // Add the book details title
                PopupFormBuilder.AppendLine("    <tr><td colspan=\"3\" class=\"SobekEditItemSectionTitle\" >Continuing Resources Details</td></tr>");

                // Add the frequency
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Frequency:</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_frequency1\" id=\"form_typeformat_frequency1\" >");
                PopupFormBuilder.Append(frequency1.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in frequency_list)
                {
                    if (frequency1 == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");

                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_frequency2\" id=\"form_typeformat_frequency2\" >");
                PopupFormBuilder.Append(frequency2.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in frequency_list)
                {
                    if (frequency2 == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");
                PopupFormBuilder.AppendLine("</td></tr>");

                // Add the regularity
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Regularity:</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_regularity1\" id=\"form_typeformat_regularity1\" >");
                PopupFormBuilder.Append(regularity1.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in regularity_list)
                {
                    if (regularity1 == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");

                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_regularity2\" id=\"form_typeformat_regularity2\" >");
                PopupFormBuilder.Append(regularity2.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in regularity_list)
                {
                    if (regularity2 == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");
                PopupFormBuilder.AppendLine("</td></tr>");

                // Add the subtype
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Sub-type:</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_subtype\" id=\"form_typeformat_subtype\" >");
                PopupFormBuilder.Append(subtype.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in continuing_subtypes)
                {
                    if (subtype == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");
                PopupFormBuilder.AppendLine("</td></tr>");

                // Add the nature of contents
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Nature of contents:</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_nature1\" id=\"form_typeformat_nature1\" >");
                PopupFormBuilder.Append(nature1.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in nature_contents_list)
                {
                    if (nature1 == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");

                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_nature2\" id=\"form_typeformat_nature2\" >");
                PopupFormBuilder.Append(nature2.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in nature_contents_list)
                {
                    if (nature2 == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");

                PopupFormBuilder.AppendLine("</td></tr>");

                PopupFormBuilder.Append("    <tr><td>&nbsp;</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_nature3\" id=\"form_typeformat_nature3\" >");
                PopupFormBuilder.Append(nature3.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in nature_contents_list)
                {
                    if (nature3 == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");

                PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_nature4\" id=\"form_typeformat_nature4\" >");
                PopupFormBuilder.Append(nature4.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in nature_contents_list)
                {
                    if (nature4 == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");


                PopupFormBuilder.AppendLine("</td></tr>");

                // Add the government publication 
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Government:</td><td colspan=\"2\">");
                PopupFormBuilder.Append("<select class=\"formtype_large_select\" name=\"form_typeformat_govt\" id=\"form_typeformat_govt\" >");
                PopupFormBuilder.Append(govt.Length == 0
                                              ? "<option value=\"\" selected=\"selected\" ></option>"
                                              : "<option value=\"\"></option>");
                foreach (string thisString in government_list)
                {
                    if (govt == thisString)
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
                    }
                    else
                    {
                        PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
                    }
                }
                PopupFormBuilder.Append("</select>");

                PopupFormBuilder.AppendLine("</td></tr>");

                // Add the checkboxes
                PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Conference Production:");
                PopupFormBuilder.Append(conferencePublication
                                              ? "<td colspan=\"2\"><input type=\"checkbox\" name=\"form_typeformat_conference\" checked=\"checked\" /> &nbsp; &nbsp; &nbsp; &nbsp; "
                                              : "<td colspan=\"2\"><input type=\"checkbox\" name=\"form_typeformat_conference\" /> &nbsp; &nbsp; &nbsp; &nbsp; ");
                PopupFormBuilder.AppendLine("</td></tr>");
            }

            #endregion

			#region Render the 'Computer Files' section if necessary 

			if (formType == Type_Format_Type_Enum.Computer_Files)
			{
				// Add the book details title
				PopupFormBuilder.AppendLine("    <tr><td colspan=\"3\" class=\"SobekEditItemSectionTitle\" >Computer Files Details</td></tr>");

				// Add the target audience
				PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Target Audience:</td><td colspan=\"2\">");
				PopupFormBuilder.Append("<select class=\"formtype_small_select\" name=\"form_typeformat_audience\" id=\"form_typeformat_audience\" >");
				PopupFormBuilder.Append(targetAudience1.Length == 0
											  ? "<option value=\"\" selected=\"selected\" ></option>"
											  : "<option value=\"\"></option>");
				foreach (string thisAudience in audiences_list)
				{
					if (targetAudience1 == thisAudience)
					{
						PopupFormBuilder.Append("<option value=\"" + thisAudience + "\" selected=\"selected\">" + thisAudience + "</option>");
					}
					else
					{
						PopupFormBuilder.Append("<option value=\"" + thisAudience + "\">" + thisAudience + "</option>");
					}
				}
				PopupFormBuilder.Append("</select>");
				PopupFormBuilder.AppendLine("</td></tr>");

				// Add the form
				PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; File Type:</td><td colspan=\"2\">");
				PopupFormBuilder.Append("<select class=\"formtype_medium_select\" name=\"form_typeformat_subtype\" id=\"form_typeformat_subtype\" >");
				PopupFormBuilder.Append(subtype.Length == 0
											  ? "<option value=\"\" selected=\"selected\" ></option>"
											  : "<option value=\"\"></option>");
				foreach (string thisString in computer_file_form_list)
				{
					if (computerfileform == thisString)
					{
						PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
					}
					else
					{
						PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
					}
				}
				PopupFormBuilder.Append("</select>");
				PopupFormBuilder.AppendLine("</td></tr>");

	
				// Add the government publication 
				PopupFormBuilder.Append("    <tr><td> &nbsp; &nbsp; Government:</td><td colspan=\"2\">");
				PopupFormBuilder.Append("<select class=\"formtype_large_select\" name=\"form_typeformat_govt\" id=\"form_typeformat_govt\" >");
				PopupFormBuilder.Append(govt.Length == 0
											  ? "<option value=\"\" selected=\"selected\" ></option>"
											  : "<option value=\"\"></option>");
				foreach (string thisString in government_list)
				{
					if (govt == thisString)
					{
						PopupFormBuilder.Append("<option value=\"" + thisString + "\" selected=\"selected\">" + thisString + "</option>");
					}
					else
					{
						PopupFormBuilder.Append("<option value=\"" + thisString + "\">" + thisString + "</option>");
					}
				}
				PopupFormBuilder.Append("</select>");

				PopupFormBuilder.AppendLine("</td></tr>");
			}

			#endregion

			#region Finish the pop-up form

			PopupFormBuilder.AppendLine("    <tr style=\"height:35px; text-align: center; vertical-align: bottom;\">");
			PopupFormBuilder.AppendLine("      <td colspan=\"3\"><button title=\"Close\" class=\"sbkMetadata_RoundButton\" onclick=\"return close_typeformat_form();\">CLOSE</button></td>");
			PopupFormBuilder.AppendLine("    </tr>");
			PopupFormBuilder.AppendLine("  </table>");
			PopupFormBuilder.AppendLine("</div>");
			PopupFormBuilder.AppendLine();

            #endregion


        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears any preexisting marc country codes, marc frequency codes, languages, and genres with marcgt as the authority </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Delete the place which has a marc country code
            if (Bib.Bib_Info.Origin_Info.Places_Count > 0)
            {
                Origin_Info_Place deletePlace = Bib.Bib_Info.Origin_Info.Places.FirstOrDefault(thisPlace => thisPlace.Place_MarcCountry.Length > 0);
                if (deletePlace != null)
                    Bib.Bib_Info.Origin_Info.Remove_Place(deletePlace);
            }

            // Delete all marcfrequency frequency terms
            if (Bib.Bib_Info.Origin_Info.Frequencies_Count > 0)
            {
                List<Origin_Info_Frequency> deleteFrequencies = Bib.Bib_Info.Origin_Info.Frequencies.Where(thisFrequency => thisFrequency.Authority == "marcfrequency").ToList();
                foreach (Origin_Info_Frequency deleteFrequency in deleteFrequencies)
                {
                    Bib.Bib_Info.Origin_Info.Remove_Frequency(deleteFrequency);
                }
            }

            // Delete all languages
            Bib.Bib_Info.Clear_Languages();

            // Delete all marcgt genres
            if (Bib.Bib_Info.Genres_Count > 0)
            {
                List<Genre_Info> deleteGenres = Bib.Bib_Info.Genres.Where(ThisGenre => ThisGenre.Authority == "marcgt").ToList();

                foreach (Genre_Info deleteGenre in deleteGenres)
                {
                    Bib.Bib_Info.Remove_Genre(deleteGenre);
                }
            }

            // If this is a PROJECT, clear teh default type
            if (Bib.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Project )
            {
                if (Bib.Bib_Info.Notes_Count > 0)
                {
                    Note_Info deleteNote = null;
                    foreach (Note_Info thisNote in Bib.Bib_Info.Notes)
                    {
                        if (thisNote.Note_Type == Note_Type_Enum.default_type)
                        {
                            deleteNote = thisNote;
                        }
                    }
                    if (deleteNote != null)
                        Bib.Bib_Info.Remove_Note(deleteNote);
                }
            }
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            // Pull the standard values
            NameValueCollection form = HttpContext.Current.Request.Form;

            string type = form["form_typeformat_type"].Trim();
            string extent = form["form_typeformat_extent"].Trim();
            string datestart = form["form_typeformat_datestart"].Trim();
            string dateend = form["form_typeformat_dateend"].Trim();
            string placecode = form["form_typeformat_placecode"].Trim();
            string langcode = form["form_typeformat_langcode"].Trim();

            // Apply the type
            if (Bib.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Project )
            {
                if (type.Length > 0)
                {
                    Bib.Bib_Info.Add_Note(type, Note_Type_Enum.default_type);
                }
            }
            else
            {
                Bib.Bib_Info.SobekCM_Type_String = type;
            }

            // Apply these standard values 
            Bib.Bib_Info.Original_Description.Extent = extent.Replace("|b ", "").Replace("|c ", "");
            Bib.Bib_Info.Origin_Info.MARC_DateIssued_End = dateend;
            Bib.Bib_Info.Origin_Info.MARC_DateIssued_Start = datestart;
            Bib.Bib_Info.Origin_Info.Insert_Place(String.Empty, placecode, String.Empty);
            Bib.Bib_Info.Add_Language(langcode);

            // Check for target audience
            string audience = form["form_typeformat_audience"];
            if (!string.IsNullOrEmpty(audience))
            {
                bool found = false;
                if (Bib.Bib_Info.Target_Audiences_Count > 0)
                {
                    if (Bib.Bib_Info.Target_Audiences.Any(target => target.Audience == audience))
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    Bib.Bib_Info.Insert_Target_Audience(0, audience, "marctarget" );
                }
            }

            object temp_object = form["form_typeformat_nature1"];
            if (temp_object != null)
            {
                string nature1 = temp_object.ToString();
                if (nature1.Length > 0)
                    Bib.Bib_Info.Add_Genre(nature1, "marcgt");
            }

            temp_object = form["form_typeformat_nature2"];
            if (temp_object != null)
            {
                string nature2 = temp_object.ToString();
                if (nature2.Length > 0)
                    Bib.Bib_Info.Add_Genre(nature2, "marcgt");
            }

            temp_object = form["form_typeformat_nature3"];
            if (temp_object != null)
            {
                string nature3 = temp_object.ToString();
                if (nature3.Length > 0)
                    Bib.Bib_Info.Add_Genre(nature3, "marcgt");
            }

            temp_object = form["form_typeformat_nature4"];
            if (temp_object != null)
            {
                string nature4 = temp_object.ToString();
                if (nature4.Length > 0)
                    Bib.Bib_Info.Add_Genre(nature4, "marcgt");
            }

            temp_object = form["form_typeformat_govt"];
            if (temp_object != null)
            {
                string govt = temp_object.ToString();
                if (govt.Length > 0)
                    Bib.Bib_Info.Add_Genre(govt, "marcgt");
            }

            temp_object = form["form_typeformat_literary1"];
            if (temp_object != null)
            {
                string literary1 = temp_object.ToString();
                if  (literary1.Length > 0)
                    Bib.Bib_Info.Add_Genre(literary1, "marcgt");
            }
            
            temp_object = form["form_typeformat_literary2"];
            if (temp_object != null)
            {
                string literary2 = temp_object.ToString();
                if  (literary2.Length > 0)
                    Bib.Bib_Info.Add_Genre(literary2, "marcgt");
            }

            temp_object = form["form_typeformat_biography"];
            if (temp_object != null)
            {
                string biography = temp_object.ToString();
                if (biography.Length > 0)
                    Bib.Bib_Info.Add_Genre(biography, "marcgt");
            }

            temp_object = form["form_typeformat_frequency1"];
            if (temp_object != null)
            {
                string frequency1 = temp_object.ToString();
                if  (frequency1.Length > 0)
                    Bib.Bib_Info.Origin_Info.Add_Frequency( frequency1, "marcfrequency");
            }

            temp_object = form["form_typeformat_frequency2"];
            if (temp_object != null)
            {
                string frequency2 = temp_object.ToString();
                if (frequency2.Length > 0)
                    Bib.Bib_Info.Origin_Info.Add_Frequency( frequency2, "marcfrequency");
            }

            temp_object = form["form_typeformat_regularity1"];
            if (temp_object != null)
            {
                string regularity1 = temp_object.ToString();
                if (regularity1.Length > 0)
                    Bib.Bib_Info.Origin_Info.Add_Frequency( regularity1, "marcfrequency");
            }

            temp_object = form["form_typeformat_regularity2"];
            if (temp_object != null)
            {
                string regularity2 = temp_object.ToString();
                if  (regularity2.Length > 0)
                    Bib.Bib_Info.Origin_Info.Add_Frequency( regularity2, "marcfrequency");
            }

            temp_object = form["form_typeformat_subtype"];
            if (temp_object != null)
            {
                string subtype = temp_object.ToString();
                if (subtype.Length > 0)
                    Bib.Bib_Info.Add_Genre(subtype, "marcgt");
            }

            temp_object = form["form_typeformat_conference"];
            if (temp_object != null)
            {
                Bib.Bib_Info.Add_Genre("conference publication", "marcgt");
            }

            temp_object = form["form_typeformat_festschrift"];
            if (temp_object != null)
            {
                Bib.Bib_Info.Add_Genre("festschrift", "marcgt");
            }

            temp_object = form["form_typeformat_index"];
            if (temp_object != null)
            {
                Bib.Bib_Info.Add_Genre("indexed", "marcgt");
            }
        }

        #region Method to create the link text

        private string show_material_type_details( SobekCM_Item Bib )
        {
            string place_code = String.Empty;
            string language_code = String.Empty;
            if (Bib.Bib_Info.Origin_Info.Places_Count > 0)
            {
                foreach (Origin_Info_Place thisPlace in Bib.Bib_Info.Origin_Info.Places)
                {
                    if (thisPlace.Place_MarcCountry.Length > 0)
                    {
                        place_code = thisPlace.Place_MarcCountry;
                        break;
                    }
                }
            }
            if (Bib.Bib_Info.Languages_Count > 0)
            {
                foreach (Language_Info thisLanguage in Bib.Bib_Info.Languages)
                {
                    if (thisLanguage.Language_ISO_Code.Length > 0)
                    {
                        language_code = thisLanguage.Language_ISO_Code;
                        break;
                    }
                }
            }
            
            StringBuilder builder = new StringBuilder();
            if (Bib.Bib_Info.Original_Description.Extent.Length > 0)
                builder.Append(Bib.Bib_Info.Original_Description.Extent + " -- ");
            if (place_code.Length > 0)
                builder.Append(place_code + " -- ");
            if (language_code.Length > 0)
                builder.Append(language_code + " -- ");

            if (Bib.Bib_Info.Subjects_Count > 0)
            {
                foreach (Subject_Info thisSubject in Bib.Bib_Info.Subjects)
                {
                    if (thisSubject.Class_Type == Subject_Info_Type.Cartographics)
                    {
                        Subject_Info_Cartographics carto = (Subject_Info_Cartographics)thisSubject;
                        if (carto.Scale.Length > 0)
                        {
                            if (carto.Scale.ToUpper().IndexOf("SCALE") < 0)
                            {
                                builder.Append(carto.Scale + " (scale) -- ");
                            }
                            else
                            {
                                builder.Append(carto.Scale + " -- ");
                            }
                            break;
                        }
                    }
                }
            }

            if (Bib.Bib_Info.Genres_Count > 0)
            {
                foreach (Genre_Info thisGenre in Bib.Bib_Info.Genres)
                {
                    if (thisGenre.Authority == "marcgt")
                        builder.Append(thisGenre.Genre_Term + " -- ");
                }
            }

            if (Bib.Bib_Info.Target_Audiences_Count > 0)
            {
                foreach (TargetAudience_Info thisAudience in Bib.Bib_Info.Target_Audiences)
                {
                    if (thisAudience.Authority == "marctarget")
                        builder.Append(thisAudience.Audience + " -- ");
                }
            }

            string result = builder.ToString().Replace(" -- -- ", " -- ");
            if (result.Length > 4)
            {
                return result.Substring(0, result.Length - 3).Trim();
            }
            return String.Empty;
        }

        #endregion

        #region Methods Implementing the Abstract Methods from abstract_Element class

        /// <summary> Reads the inner data from the Template XML format </summary>
        /// <param name="XMLReader"> Current template xml configuration reader </param>
        /// <remarks> This reads the possible values for the type combo box from a <i>options</i> subelement and the default value from a <i>value</i> subelement </remarks>
        protected override void Inner_Read_Data(XmlTextReader XMLReader)
        {
            default_values.Clear();
            while (XMLReader.Read())
            {
                if ((XMLReader.NodeType == XmlNodeType.Element) && ((XMLReader.Name.ToLower() == "value") || (XMLReader.Name.ToLower() == "options")))
                {
                    if (XMLReader.Name.ToLower() == "value")
                    {
                        XMLReader.Read();
                        default_values.Add(XMLReader.Value.Trim());
                    }
                    else
                    {
                        XMLReader.Read();
                        string options = XMLReader.Value.Trim();
                        items.Clear();
                        if (options.Length > 0)
                        {
                            string[] options_parsed = options.Split(",".ToCharArray());
                            foreach (string thisOption in options_parsed)
                            {
                                if (!items.Contains(thisOption.Trim()))
                                {
                                    items.Add(thisOption.Trim());
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Nested type: Type_Format_Type_Enum

        /// <summary> Enumeration is used to determine the MARC type of material, which additionally
        /// determines the form to display for additional material information </summary>
        private enum Type_Format_Type_Enum : byte
        {
            /// <summary> No identifiable type </summary>
            None = 0,

            /// <summary> Book type material </summary>
            Book = 1,

            /// <summary> Visual materials ( i.e. Photograph, Slides, Video, etc.. ) </summary>
            Visual_Materials,

            /// <summary> Continuing resource type material ( i.e. Newspapers, Serial, etc..) </summary>
            Continuing_Resource,

			/// <summary> Computer files, in particular software or datasets </summary>
			Computer_Files,

            /// <summary> Cartographic type material ( maps, globes, etc.. ) </summary>
            Map
        }

        #endregion
    }
}
