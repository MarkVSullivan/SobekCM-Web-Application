#region Using directives

using System;
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
    /// <summary> Element allows entry of a note, including note type and display label, for an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    public class Note_Complex_Element : abstract_Element
    {
        private readonly int cols;
        private readonly int colsMozilla;

        /// <summary> Constructor for a new instance of the Note_Complex_Element class </summary>
        public Note_Complex_Element()
        {
            Rows = 3;
            html_element_name = "complex_note";
            Repeatable = true;
            Type = Element_Type.Note;
            Display_SubType = "complex";
            Title = "Note";

            cols = TEXT_AREA_COLUMNS;
            colsMozilla = MOZILLA_TEXT_AREA_COLUMNS;
            Include_Statement_Responsibility = true;
            Include_Acquisition_Note = true;

	        help_page = "note";
        }

        /// <summary> Flag indicates whether to include the statement of responsibility in the list of notes </summary>
        /// <remarks> If the full title form element is used in this template, the statement of responsibility will be found there </remarks>
        public bool Include_Statement_Responsibility { get; set; }

        /// <summary> Flag indicates whether to include the include_acquisition_note in the list of notes </summary>
        /// <remarks> If the Acquisition Note element is in the constants, it should not show here (it should be uneditable) </remarks>
        public bool Include_Acquisition_Note { get; set; }

        /// <summary> Gets and sets the number of lines for this text box </summary>
        protected int Rows { get; set; }

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
                const string DEFAULT_ACRONYM = "Enter any notes about this digital manifestation or the original material";
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

            // Determine the columns for this text area, based on browser
            int actual_cols = cols;
            if (IsMozilla)
                actual_cols = colsMozilla;

            
            string id_name = html_element_name.Replace("_", "");

            Output.WriteLine("  <!-- " + Title + " Element -->");
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td width=\"" + LEFT_MARGIN + "px\">&nbsp;</td>");
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
            Output.WriteLine("        <tr style=\text-align:left\">");
            Output.WriteLine("          <td>");
            Output.WriteLine("            <div id=\"" + html_element_name + "_div\">");

            int notes_count = 0;
            if (Bib.Bib_Info.Notes_Count > 0)
            {
                notes_count += Bib.Bib_Info.Notes.Count(ThisNote => ((ThisNote.Note_Type != Note_Type_Enum.statement_of_responsibility) || (Include_Statement_Responsibility)) && (ThisNote.Note_Type != Note_Type_Enum.default_type));
            }

            if (notes_count == 0)
            {
                Output.WriteLine("              <div id=\"" + html_element_name + "_topdiv1\">");
                Output.WriteLine("                <span class=\"metadata_sublabel2\">" + Translator.Get_Translation("Type", CurrentLanguage) + ":</span>");
                Output.WriteLine("                <select class=\"" + html_element_name + "_type\" name=\"" + id_name + "_type1\" id=\"" + id_name + "_type1\" onchange=\"complexnote_type_change('1');\" >");
                Output.WriteLine("                  <option selected=\"selected=\" value=\"500\"></option>");
                Output.WriteLine("                  <option value=\"541\">Acquisition</option>");
                Output.WriteLine("                  <option value=\"530\">Additional Physical Form</option>");
                Output.WriteLine("                  <option value=\"504\">Bibliography</option>");
                Output.WriteLine("                  <option value=\"545\">Biographical</option>");
                Output.WriteLine("                  <option value=\"510\">Citation/Reference</option>");
                Output.WriteLine("                  <option value=\"508\">Creation/Production Credits</option>");
                Output.WriteLine("                  <option value=\"362\">Dates/Sequential Designation</option>");
                Output.WriteLine("                  <option value=\"donation\">Donation</option>");
                Output.WriteLine("                  <option value=\"585\">Exhibitions</option>");
                Output.WriteLine("                  <option value=\"536\">Funding</option>");
                Output.WriteLine("                  <option value=\"internal\">Internal Comments</option>");
                Output.WriteLine("                  <option value=\"550\">Issuing Body</option>");
                Output.WriteLine("                  <option value=\"546\">Language</option>");
                Output.WriteLine("                  <option value=\"515\">Numbering Peculiarities</option>");
                Output.WriteLine("                  <option value=\"535\">Original Location</option>");
                Output.WriteLine("                  <option value=\"534\">Original Version</option>");
                Output.WriteLine("                  <option value=\"561\">Ownership</option>");
                Output.WriteLine("                  <option value=\"511\">Performers</option>");
                Output.WriteLine("                  <option value=\"524\">Preferred Citation</option>");
                Output.WriteLine("                  <option value=\"581\">Publications</option>");
                Output.WriteLine("                  <option value=\"pubstatus\">Publication Status</option>");
                Output.WriteLine("                  <option value=\"506\">Restriction</option>");
                if (Include_Statement_Responsibility)
                {
                    Output.WriteLine("                  <option value=\"245\">Statement of Responsibility</option>");
                }
                Output.WriteLine("                  <option value=\"538\">System Details</option>");
                Output.WriteLine("                  <option value=\"502\">Thesis</option>");
                Output.WriteLine("                  <option value=\"518\">Venue</option>");
                Output.WriteLine("                  <option value=\"562\">Version Identification</option>");
                Output.WriteLine("                </select>");
                Output.WriteLine("              </div>");

                Output.WriteLine("              <textarea rows=\"" + Rows + "\" cols=\"" + actual_cols + "\" name=\"" + id_name + "_textarea1\" id=\"" + id_name + "_textarea1\" class=\"" + html_element_name + "_input sbk_Focusable\" ></textarea>");
            }
            else
            {
                int i = 1;
                foreach (Note_Info thisNote in Bib.Bib_Info.Notes)
                {
                    if (((thisNote.Note_Type != Note_Type_Enum.statement_of_responsibility) || (Include_Statement_Responsibility)) &&
                        ( thisNote.Note_Type != Note_Type_Enum.default_type ))
                    {
                        string note_display_label = String.Empty;
                        string display_label_prompt = "Materials Selected";

                        Output.WriteLine("              <div id=\"" + html_element_name + "_topdiv" + i + "\">");
                        Output.WriteLine("                <span class=\"metadata_sublabel2\">Type:</span>");
                        Output.WriteLine("                <select class=\"" + html_element_name + "_type\" name=\"" + id_name + "_type" + i + "\" id=\"" + id_name + "_type" + i + "\" onchange=\"complexnote_type_change('" + i + "');\" >");

                        if (thisNote.Note_Type == Note_Type_Enum.NONE)
                        {
                            Output.WriteLine("                  <option value=\"500\" selected=\"selected\"></option>");
                            note_display_label = thisNote.Display_Label;
                            display_label_prompt = "Display Label";
                        }
                        else
                            Output.WriteLine("                  <option value=\"500\"></option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.acquisition
                                             ? "                  <option value=\"541\" selected=\"selected\">Acquisition</option>"
                                             : "                  <option value=\"541\">Acquisition</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.additional_physical_form
                                             ? "                  <option value=\"530\" selected=\"selected\">Additional Physical Form</option>"
                                             : "                  <option value=\"530\">Additional Physical Form</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.bibliography
                                             ? "                  <option value=\"504\" selected=\"selected\">Bibliography</option>"
                                             : "                  <option value=\"504\">Bibliography</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.biographical
                                             ? "                  <option value=\"545\" selected=\"selected\">Biographical</option>"
                                             : "                  <option value=\"545\">Biographical</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.citation_reference
                                             ? "                  <option value=\"510\" selected=\"selected\">Citation/Reference</option>"
                                             : "                  <option value=\"510\">Citation/Reference</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.creation_credits
                                             ? "                  <option value=\"508\" selected=\"selected\">Creation/Production Credits</option>"
                                             : "                  <option value=\"508\">Creation/Production Credits</option>");

                        if (thisNote.Note_Type == Note_Type_Enum.dates_sequential_designation)
                        {
                            Output.WriteLine("                  <option value=\"362\" selected=\"selected\">Dates/Sequential Designation</option>");
                            note_display_label = thisNote.Display_Label;
                            display_label_prompt = "Source";
                        }
                        else
                            Output.WriteLine("                  <option value=\"362\">Dates/Sequential Designation</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.donation
                                             ? "                  <option value=\"donation\" selected=\"selected\">Donation</option>"
                                             : "                  <option value=\"donation\">Donation</option>");

                        if (thisNote.Note_Type == Note_Type_Enum.exhibitions)
                        {
                            Output.WriteLine("                  <option value=\"585\" selected=\"selected\">Exhibitions</option>");
                            note_display_label = thisNote.Display_Label;
                        }
                        else
                            Output.WriteLine("                  <option value=\"585\">Exhibitions</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.funding
                                             ? "                  <option value=\"536\" selected=\"selected\">Funding</option>"
                                             : "                  <option value=\"536\">Funding</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.internal_comments
                                             ? "                  <option value=\"internal\" selected=\"selected\">Internal Comments</option>"
                                             : "                  <option value=\"internal\">Internal Comments</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.issuing_body
                                             ? "                  <option value=\"550\" selected=\"selected\">Issuing Body</option>"
                                             : "                  <option value=\"550\">Issuing Body</option>");

                        if (thisNote.Note_Type == Note_Type_Enum.language)
                        {
                            Output.WriteLine("                  <option value=\"546\" selected=\"selected\">Language</option>");
                            note_display_label = thisNote.Display_Label;
                        }
                        else
                            Output.WriteLine("                  <option value=\"546\">Language</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.numbering_peculiarities
                                             ? "                  <option value=\"515\" selected=\"selected\">Numbering Peculiarities</option>"
                                             : "                  <option value=\"515\">Numbering Peculiarities</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.original_location
                                             ? "                  <option value=\"535\" selected=\"selected\">Original Location</option>"
                                             : "                  <option value=\"535\">Original Location</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.original_version
                                             ? "                  <option value=\"534\" selected=\"selected\">Original Version</option>"
                                             : "                  <option value=\"534\">Original Version</option>");

                        if (thisNote.Note_Type == Note_Type_Enum.ownership)
                        {
                            Output.WriteLine("                  <option value=\"561\" selected=\"selected\">Ownership</option>");
                            note_display_label = thisNote.Display_Label;
                        }
                        else
                            Output.WriteLine("                  <option value=\"561\">Ownership</option>");

                        if (thisNote.Note_Type == Note_Type_Enum.performers)
                        {
                            Output.WriteLine("                  <option value=\"511\" selected=\"selected\">Performers</option>");
                            note_display_label = thisNote.Display_Label;
                            display_label_prompt = "Display Label";
                        }
                        else
                            Output.WriteLine("                  <option value=\"511\">Performers</option>");

                        if (thisNote.Note_Type == Note_Type_Enum.preferred_citation)
                        {
                            Output.WriteLine("                  <option value=\"524\" selected=\"selected\">Preferred Citation</option>");
                            note_display_label = thisNote.Display_Label;
                        }
                        else
                            Output.WriteLine("                  <option value=\"524\">Preferred Citation</option>");

                        if (thisNote.Note_Type == Note_Type_Enum.publications)
                        {
                            Output.WriteLine("                  <option value=\"581\" selected=\"selected\">Publications</option>");
                            note_display_label = thisNote.Display_Label;
                        }
                        else
                            Output.WriteLine("                  <option value=\"581\">Publications</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.publication_status
                                             ? "                  <option value=\"pubstatus\" selected=\"selected\">Publication Status</option>"
                                             : "                  <option value=\"pubstatus\">Publication Status</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.restriction
                                             ? "                  <option value=\"506\" selected=\"selected\">Restriction</option>"
                                             : "                  <option value=\"506\">Restriction</option>");

                        if (Include_Statement_Responsibility)
                        {
                            Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.statement_of_responsibility
                                                 ? "                  <option value=\"245\" selected=\"selected\">Statement of Responsibility</option>"
                                                 : "                  <option value=\"245\">Statement of Responsibility</option>");
                        }

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.system_details
                                             ? "                  <option value=\"538\" selected=\"selected\">System Details</option>"
                                             : "                  <option value=\"538\">System Details</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.thesis
                                             ? "                  <option value=\"502\" selected=\"selected\">Thesis</option>"
                                             : "                  <option value=\"502\">Thesis</option>");

                        if (thisNote.Note_Type == Note_Type_Enum.date_venue)
                        {
                            Output.WriteLine("                  <option value=\"518\" selected=\"selected\">Venue</option>");
                            note_display_label = thisNote.Display_Label;
                        }
                        else
                            Output.WriteLine("                  <option value=\"518\">Venue</option>");

                        Output.WriteLine(thisNote.Note_Type == Note_Type_Enum.version_identification
                                             ? "                  <option value=\"562\" selected=\"selected\">Version Identification</option>"
                                             : "                  <option value=\"562\">Version Identification</option>");


                        Output.WriteLine("                </select>");

                        if (note_display_label.Length > 0)
                        {
                            Output.Write("<span class=\"metadata_sublabel\" id=\"complexnote_inputtext" + i + "\" name=\"complexnote_inputtext" + i + "\">" + display_label_prompt + ":</span>");
                            Output.WriteLine("<input class=\"complexnote_input sbk_Focusable\" id=\"complexnote_inputtext" + i + "\" name=\"complexnote_input" + i + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(note_display_label) + "\" />");
                        }

                        Output.WriteLine("              </div>");

                        Output.Write("              <textarea rows=\"" + Rows + "\" cols=\"" + actual_cols + "\" name=\"" + id_name + "_textarea" + i + "\" id=\"" + id_name + "_textarea" + i + "\" class=\"" + html_element_name + "_input sbk_Focusable\" >" + HttpUtility.HtmlEncode(thisNote.Note) + "</textarea>");

                        if (i < Bib.Bib_Info.Notes_Count)
                        {
                            Output.WriteLine("<br />");
                        }
                        else
                        {
                            Output.WriteLine();
                        }

                        i++;
                    }
                }
                Output.WriteLine("            </div>");
                Output.WriteLine("          </td>");
            }

            Output.WriteLine("          <td style=\"vertical-align:bottom\" >");
			
            if (Repeatable)
            {
                Output.WriteLine("            <a title=\"" + Translator.Get_Translation("Click to add a new note", CurrentLanguage) + ".\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onmousedown=\"return add_complex_note('" + Rows + "','" + actual_cols + "');\"><img class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" /></a>");
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
        /// <remarks> This clears any preexisting note that is not a statement of responsibility  </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            if ( Include_Statement_Responsibility )
            {
                Bib.Bib_Info.Clear_Notes();    
            }
            else
            {
                if (Bib.Bib_Info.Notes_Count > 0)
                {
                    List<Note_Info> deletes = Bib.Bib_Info.Notes.Where(ThisNote => (ThisNote.Note_Type != Note_Type_Enum.statement_of_responsibility) && (ThisNote.Note_Type != Note_Type_Enum.default_type)).ToList();

                    foreach (Note_Info thisNote in deletes)
                    {
                        Bib.Bib_Info.Remove_Note(thisNote);
                    }
                }
            }
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            const string HTML_ELEMENT_NAME = "complex_note";
            string id = HTML_ELEMENT_NAME.Replace("_","");
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf("complexnote_type") == 0)
                {
                    string key = thisKey.Replace("complexnote_type", "");
                    string addtl_key = id + "_input" + key;
                    string textarea_key = id + "_textarea" + key;

                    string type = HttpContext.Current.Request.Form[thisKey].Trim();
                    Note_Type_Enum type_enum = Note_Type_Enum.NONE;
                    switch (type)
                    {
                        case "541":
                            type_enum = Note_Type_Enum.acquisition;
                            break;

                        case "530":
                            type_enum = Note_Type_Enum.additional_physical_form;
                            break;

                        case "504":
                            type_enum = Note_Type_Enum.bibliography;
                            break;

                        case "545":
                            type_enum = Note_Type_Enum.biographical;
                            break;

                        case "510":
                            type_enum = Note_Type_Enum.citation_reference;
                            break;

                        case "508":
                            type_enum = Note_Type_Enum.creation_credits;
                            break;

                        case "362":
                            type_enum = Note_Type_Enum.dates_sequential_designation;
                            break;

                        case "donation":
                            type_enum = Note_Type_Enum.donation;
                            break;

                        case "585":
                            type_enum = Note_Type_Enum.exhibitions;
                            break;

                        case "536":
                            type_enum = Note_Type_Enum.funding;
                            break;

                        case "internal":
                            type_enum = Note_Type_Enum.internal_comments;
                            break;

                        case "550":
                            type_enum = Note_Type_Enum.issuing_body;
                            break;

                        case "546":
                            type_enum = Note_Type_Enum.language;
                            break;

                        case "515":
                            type_enum = Note_Type_Enum.numbering_peculiarities;
                            break;

                        case "535":
                            type_enum = Note_Type_Enum.original_location;
                            break;

                        case "534":
                            type_enum = Note_Type_Enum.original_version;
                            break;

                        case "561":
                            type_enum = Note_Type_Enum.ownership;
                            break;

                        case "511":
                            type_enum = Note_Type_Enum.performers;
                            break;

                        case "524":
                            type_enum = Note_Type_Enum.preferred_citation;
                            break;

                        case "581":
                            type_enum = Note_Type_Enum.publications;
                            break;

                        case "506":
                            type_enum = Note_Type_Enum.restriction;
                            break;

                        case "245":
                            type_enum = Note_Type_Enum.statement_of_responsibility;
                            break;

                        case "538":
                            type_enum = Note_Type_Enum.system_details;
                            break;

                        case "502":
                            type_enum = Note_Type_Enum.thesis;
                            break;

                        case "518":
                            type_enum = Note_Type_Enum.date_venue;
                            break;

                        case "562":
                            type_enum = Note_Type_Enum.version_identification;
                            break;

                        case "pubstatus":
                            type_enum = Note_Type_Enum.publication_status;
                            break;
                    }

                    string addtl = String.Empty;
                    if (HttpContext.Current.Request.Form[addtl_key] != null)
                    {
                        addtl = HttpContext.Current.Request.Form[addtl_key].Trim();
                    }
                    string textarea = HttpContext.Current.Request.Form[textarea_key].Trim();

                    if (textarea.Length > 0)
                    {
                        Bib.Bib_Info.Add_Note(textarea, type_enum, addtl);
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