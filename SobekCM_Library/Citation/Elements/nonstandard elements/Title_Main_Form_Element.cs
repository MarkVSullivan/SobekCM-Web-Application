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
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element displays a form to allow for complete entry of the main title for an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    class Title_Main_Form_Element : abstract_Element
    {
        /// <summary> Constructor for a new instance of the Title_Main_Form_Element class </summary>
        public Title_Main_Form_Element()
        {
            Repeatable = false;
            Title = "Main Title";
            html_element_name = "form_title_main";
	        help_page = "title";
        }

        #region iElement Members

        /// <summary> Options dictionary allows template elements to register certain options or information
        /// which may be used by other template elements </summary>
        /// <remarks> This adds a special flag to indicate there is a seperate contributor element ( title_form_included = true ) </remarks>
        public override Dictionary<string, string> Options
        {
            get { return base.Options; }
            set
            {
                base.Options = value;

                Options["title_form_included"] = "true";
            }
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
                const string DEFAULT_ACRONYM = "Enter the title of this material here.";
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

            // Determine the statement of responsibility
            string statement_responsibility = String.Empty;
            if (Bib.Bib_Info.Notes_Count > 0)
            {
                foreach (Note_Info thisNote in Bib.Bib_Info.Notes)
                {
                    if (thisNote.Note_Type == Note_Type_Enum.StatementOfResponsibility)
                    {
                        statement_responsibility = HttpUtility.HtmlEncode(thisNote.Note);
                        break;
                    }
                }
            }

            // Render this in HTML
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
            Output.WriteLine("            <div class=\"form_title_div\">");

            // Add the link for the main title
            if (Bib.Bib_Info.Main_Title.Title.Length == 0)
            {
                Output.Write("              <a title=\"Click to edit the main title\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_maintitle_term')\" onblur=\"link_blurred2('form_maintitle_term')\" onkeypress=\"return popup_keypress_focus('form_maintitle', 'formmaintitletitle', '" + IsMozilla.ToString() + "');\" onclick=\"return popup_focus('form_maintitle', 'formmaintitletitle');\"><div class=\"form_linkline_empty form_title_main_line\" id=\"form_maintitle_term\"><i>Empty Main Title</i>");
            }
            else
            {
                Output.Write("              <a title=\"Click to edit the main title\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_maintitle_term')\" onblur=\"link_blurred2('form_maintitle_term')\" onkeypress=\"return popup_keypress_focus('form_maintitle', 'formmaintitletitle', '" + IsMozilla.ToString() + "');\" onclick=\"return popup_focus('form_maintitle', 'formmaintitletitle');\"><div class=\"form_linkline form_title_main_line\" id=\"form_maintitle_term\">" + Bib.Bib_Info.Main_Title.NonSort + Bib.Bib_Info.Main_Title.Title);
                if (Bib.Bib_Info.Main_Title.Subtitle.Length > 0)
                    Output.Write(" : " + Bib.Bib_Info.Main_Title.Subtitle);
            }
            Output.WriteLine( "</div></a> ");

            Output.WriteLine("            </div>");
            Output.WriteLine("          </td>");
            Output.WriteLine("          <td style=\"vertical-align:bottom\" >");
            Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img class=\"help_button\" src=\"" + HELP_BUTTON_URL + "\" /></a>");
            Output.WriteLine("          </td>");
            Output.WriteLine("        </tr>");
            Output.WriteLine("      </table>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine();

            // Add the popup form
            PopupFormBuilder.AppendLine("<!-- Main Title Form -->");
			PopupFormBuilder.AppendLine("<div class=\"title_main_popup_div sbkMetadata_PopupDiv\" id=\"form_maintitle\" style=\"display:none;\">");
			PopupFormBuilder.AppendLine("  <div class=\"sbkMetadata_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left\">Edit Main Title</td><td style=\"text-align:right\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" alt=\"HELP\" target=\"_" + html_element_name.ToUpper() + "\" >?</a> &nbsp; <a href=\"#template\" alt=\"CLOSE\" onclick=\"close_maintitle_form('form_maintitle')\">X</a> &nbsp; </td></tr></table></div>");
            PopupFormBuilder.AppendLine("  <br />");
			PopupFormBuilder.AppendLine("  <table class=\"sbkMetadata_PopupTable\">");

            // Add the title type (and optionally display label)
            PopupFormBuilder.AppendLine("    <tr><td style=\"width:90px\">Title Type:</td><td colspan=\"2\"><span style=\"color: Blue; padding-bottom: 7px;\" >Main Title</span></td></tr>");

            // Add the nonsort and language text boxes
            PopupFormBuilder.Append("    <tr><td>Non Sort:</td><td>");
            PopupFormBuilder.Append("<input class=\"formtitle_small_input sbk_Focusable\" name=\"formmaintitlenonsort\" id=\"formmainttitlenonsort\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.NonSort) + "\" />");
            PopupFormBuilder.Append("</td><td style=\"width:255px\" >Language: &nbsp; ");
			PopupFormBuilder.Append("<input class=\"formtitle_small_input sbk_Focusable\" name=\"formmaintitlelanguage\" id=\"formmainttitlelanguage\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Language) + "\" />");
            PopupFormBuilder.AppendLine("</td></tr>");

            // Add the title, subtitle, and statement of responsibility
			PopupFormBuilder.AppendLine("    <tr><td>Title:</td><td colspan=\"2\"><input class=\"formtitle_large_input sbk_Focusable\" name=\"formmaintitletitle\" id=\"formmaintitletitle\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Title) + "\" /></td></tr>");
			PopupFormBuilder.AppendLine("    <tr><td>Sub Title:</td><td colspan=\"2\"><input class=\"formtitle_large_input sbk_Focusable\" name=\"formmaintitlesubtitle\" id=\"formmaintitlesubtitle\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Subtitle) + "\" /></td></tr>");
			PopupFormBuilder.AppendLine("    <tr><td colspan=\"3\">Statement of Responsibility: &nbsp; &nbsp; <input class=\"formtitle_medium_input sbk_Focusable\" name=\"formmaintitlestatement\" id=\"formmaintitlestatement\" type=\"text\" value=\"" + statement_responsibility + "\" /></td></tr>");

            // Add the part numbers
            PopupFormBuilder.Append("    <tr><td>Part Numbers:</td><td colspan=\"2\">");
            if ( Bib.Bib_Info.Main_Title.Part_Numbers_Count > 0 )
            {
				PopupFormBuilder.Append("<input class=\"formtitle_small_input sbk_Focusable\" name=\"formmaintitlepartnum1\" id=\"formmaintitlepartnum1\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Part_Numbers[0]) + "\" />");
            }
            else
            {
				PopupFormBuilder.Append("<input class=\"formtitle_small_input sbk_Focusable\" name=\"formmaintitlepartnum1\" id=\"formmaintitlepartnum1\" type=\"text\" value=\"\" />");
            }
            if (Bib.Bib_Info.Main_Title.Part_Numbers_Count > 1)
            {
				PopupFormBuilder.Append("<input class=\"formtitle_small_input sbk_Focusable\" name=\"formmaintitlepartnum2\" id=\"formmaintitlepartnum2\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Part_Numbers[1]) + "\" />");
            }
            else
            {
				PopupFormBuilder.Append("<input class=\"formtitle_small_input sbk_Focusable\" name=\"formmaintitlepartnum2\" id=\"formmaintitlepartnum2\" type=\"text\" value=\"\" />");
            }
            PopupFormBuilder.AppendLine("</td></tr>");

            // Add the part names and authority
            PopupFormBuilder.Append("    <tr><td>Part Names:</td><td>");
            if (Bib.Bib_Info.Main_Title.Part_Names_Count > 0)
            {
				PopupFormBuilder.Append("<input class=\"formtitle_small_input sbk_Focusable\" name=\"formmaintitlepartname1\" id=\"formmaintitlepartname1\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Part_Names[0]) + "\" />");
            }
            else
            {
				PopupFormBuilder.Append("<input class=\"formtitle_small_input sbk_Focusable\" name=\"formmaintitlepartname1\" id=\"formmaintitlepartname1\" type=\"text\" value=\"\" />");
            }
            if (Bib.Bib_Info.Main_Title.Part_Names_Count > 1)
            {
				PopupFormBuilder.Append("<input class=\"formtitle_small_input sbk_Focusable\" name=\"formmaintitlepartname2\" id=\"formmaintitlepartname2\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Part_Names[1]) + "\" />");
            }
            else
            {
				PopupFormBuilder.Append("<input class=\"formtitle_small_input sbk_Focusable\" name=\"formmaintitlepartname2\" id=\"formmaintitlepartname2\" type=\"text\" value=\"\" />");
            }
			PopupFormBuilder.Append("<td>Authority: &nbsp; <input class=\"formtitle_small_input sbk_Focusable\" name=\"formmaintitleauthority\" id=\"formmaintitleauthority\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Authority) + "\" />");
            PopupFormBuilder.AppendLine("</td></tr>");

			// Finish the popup form and add the CLOSE button
			PopupFormBuilder.AppendLine("    <tr style=\"height:35px; text-align: center; vertical-align: bottom;\">");
			PopupFormBuilder.AppendLine("      <td colspan=\"3\"><button title=\"Close\" class=\"sbkMetadata_RoundButton\" onclick=\"return close_maintitle_form('form_maintitle');\">CLOSE</button></td>");
			PopupFormBuilder.AppendLine("    </tr>");
			PopupFormBuilder.AppendLine("  </table>");
			PopupFormBuilder.AppendLine("</div>");
			PopupFormBuilder.AppendLine();

        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears the main title's part numbers and part names, and the statement of responsibility note </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Clear part numbers and part names ( collections )
            Bib.Bib_Info.Main_Title.Clear_Part_Names();
            Bib.Bib_Info.Main_Title.Clear_Part_Numbers();

            
            // Remove all statement of responsibilities
            if (Bib.Bib_Info.Notes_Count > 0)
            {
                List<Note_Info> deletes = Bib.Bib_Info.Notes.Where(ThisNote => ThisNote.Note_Type == Note_Type_Enum.StatementOfResponsibility).ToList();
                foreach (Note_Info thisNote in deletes)
                {
                    Bib.Bib_Info.Remove_Note(thisNote);
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
                if (thisKey.IndexOf("formmaintitlenonsort") == 0)
                {
                    Bib.Bib_Info.Main_Title.NonSort = HttpContext.Current.Request.Form[thisKey];
                    string title = HttpContext.Current.Request.Form["formmaintitletitle"].Trim();
                    if ( title.Length > 0 )
                        Bib.Bib_Info.Main_Title.Title = title;
                    Bib.Bib_Info.Main_Title.Subtitle = HttpContext.Current.Request.Form["formmaintitlesubtitle"].Trim();

                    string statement = HttpContext.Current.Request.Form["formmaintitlestatement"].Trim();
                    if ( statement.Length > 0 )
                    {
                        Bib.Bib_Info.Add_Note( statement, Note_Type_Enum.StatementOfResponsibility );
                    }

                    string partnum1 = HttpContext.Current.Request.Form["formmaintitlepartnum1"].Trim();
                    string partnum2 = HttpContext.Current.Request.Form["formmaintitlepartnum2"].Trim();
                    if ( partnum1.Length > 0 )
                        Bib.Bib_Info.Main_Title.Add_Part_Number( partnum1 );
                    if ( partnum2.Length > 0 )
                        Bib.Bib_Info.Main_Title.Add_Part_Number(partnum2);
                    
                    string partname1 = HttpContext.Current.Request.Form["formmaintitlepartname1"].Trim();
                    string partname2 = HttpContext.Current.Request.Form["formmaintitlepartname2"].Trim();
                    if ( partname1.Length > 0 )
                        Bib.Bib_Info.Main_Title.Add_Part_Name( partname1 );
                    if ( partname2.Length > 0 )
                        Bib.Bib_Info.Main_Title.Add_Part_Name(partname2);

                    Bib.Bib_Info.Main_Title.Authority = HttpContext.Current.Request.Form["formmaintitleauthority"].Trim();
                    Bib.Bib_Info.Main_Title.Language = HttpContext.Current.Request.Form["formmaintitlelanguage"].Trim();
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
