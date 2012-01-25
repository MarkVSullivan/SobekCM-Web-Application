#region Using directives

using System;
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
    /// <summary> Element displays a form to allow for complete entry of the main title for an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    class Title_Main_Form_Element : abstract_Element
    {
        /// <summary> Constructor for a new instance of the Title_Main_Form_Element class </summary>
        public Title_Main_Form_Element()
        {
            Repeatable = false;
            Type = Element_Type.Title;
            Display_SubType = "form";
            Title = "Main Title";
            html_element_name = "form_title_main";
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
                const string defaultAcronym = "Enter the title of this material here.";
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

            // Determine the statement of responsibility
            string statement_responsibility = String.Empty;
            if (Bib.Bib_Info.Notes_Count > 0)
            {
                foreach (Note_Info thisNote in Bib.Bib_Info.Notes)
                {
                    if (thisNote.Note_Type == Note_Type_Enum.statement_of_responsibility)
                    {
                        statement_responsibility = HttpUtility.HtmlEncode(thisNote.Note);
                        break;
                    }
                }
            }

            // Render this in HTML
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
            Output.WriteLine("            <div class=\"form_title_div\">");

            // Add the link for the main title
            if (Bib.Bib_Info.Main_Title.Title.Length == 0)
            {
                Output.Write("              <a title=\"Click to edit the main title\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_maintitle_term')\" onblur=\"link_blurred2('form_maintitle_term')\" onkeypress=\"return popup_keypress_focus('form_maintitle', 'form_maintitle_term', 'formmaintitletitle', 195, 675, '" + isMozilla.ToString() + "');\" onclick=\"return popup_focus('form_maintitle', 'form_maintitle_term', 'formmaintitletitle', 195, 675);\"><div class=\"form_linkline_empty form_title_main_line\" id=\"form_maintitle_term\"><i>Empty Main Title</i>");
            }
            else
            {
                Output.Write("              <a title=\"Click to edit the main title\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_maintitle_term')\" onblur=\"link_blurred2('form_maintitle_term')\" onkeypress=\"return popup_keypress_focus('form_maintitle', 'form_maintitle_term', 'formmaintitletitle', 195, 675, '" + isMozilla.ToString() + "');\" onclick=\"return popup_focus('form_maintitle', 'form_maintitle_term', 'formmaintitletitle', 195, 675);\"><div class=\"form_linkline form_title_main_line\" id=\"form_maintitle_term\">" + Bib.Bib_Info.Main_Title.NonSort + Bib.Bib_Info.Main_Title.Title);
                if (Bib.Bib_Info.Main_Title.Subtitle.Length > 0)
                    Output.Write(" : " + Bib.Bib_Info.Main_Title.Subtitle);
            }
            Output.WriteLine( "</div></a> ");

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
            popup_form_builder.AppendLine("<!-- Main Title Form -->");
            popup_form_builder.AppendLine("<div class=\"title_main_popup_div\" id=\"form_maintitle\" style=\"display:none;\">");
            popup_form_builder.AppendLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">EDIT MAIN TITLE</td><td align=\"right\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" alt=\"HELP\" target=\"_" + html_element_name.ToUpper() + "\" >?</a> &nbsp; <a href=\"#template\" alt=\"CLOSE\" onclick=\"close_maintitle_form('form_maintitle')\">X</a> &nbsp; </td></tr></table></div>");
            popup_form_builder.AppendLine("  <br />");
            popup_form_builder.AppendLine("  <table class=\"popup_table\">");

            // Add the title type (and optionally display label)
            popup_form_builder.AppendLine("    <tr><td width=\"90px\">Title Type:</td><td colspan=\"2\"><span style=\"color: Blue; padding-bottom: 7px;\" >Main Title</span></td></tr>");

            // Add the nonsort and language text boxes
            popup_form_builder.Append("    <tr><td>Non Sort:</td><td>");
            popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formmaintitlenonsort\" id=\"formmainttitlenonsort\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.NonSort) + "\"  onfocus=\"javascript:textbox_enter('formmainttitlenonsort', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formmainttitlenonsort', 'formtitle_small_input')\" />");
            popup_form_builder.Append("</td><td width=\"255px\" >Language: &nbsp; ");
            popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formmaintitlelanguage\" id=\"formmainttitlelanguage\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Language) + "\" onfocus=\"javascript:textbox_enter('formmainttitlelanguage', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formmainttitlelanguage', 'formtitle_small_input')\" />");
            popup_form_builder.AppendLine("</td></tr>");

            // Add the title, subtitle, and statement of responsibility
            popup_form_builder.AppendLine("    <tr><td>Title:</td><td colspan=\"2\"><input class=\"formtitle_large_input\" name=\"formmaintitletitle\" id=\"formmaintitletitle\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Title) + "\" onfocus=\"javascript:textbox_enter('formmaintitletitle', 'formtitle_large_input_focused')\" onblur=\"javascript:textbox_leave('formmaintitletitle', 'formtitle_large_input')\" /></td></tr>");
            popup_form_builder.AppendLine("    <tr><td>Sub Title:</td><td colspan=\"2\"><input class=\"formtitle_large_input\" name=\"formmaintitlesubtitle\" id=\"formmaintitlesubtitle\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Subtitle) + "\" onfocus=\"javascript:textbox_enter('formmaintitlesubtitle', 'formtitle_large_input_focused')\" onblur=\"javascript:textbox_leave('formmaintitlesubtitle', 'formtitle_large_input')\" /></td></tr>");
            popup_form_builder.AppendLine("    <tr><td colspan=\"3\">Statement of Responsibility: &nbsp; &nbsp; <input class=\"formtitle_medium_input\" name=\"formmaintitlestatement\" id=\"formmaintitlestatement\" type=\"text\" value=\"" + statement_responsibility + "\" onfocus=\"javascript:textbox_enter('formmaintitlestatement', 'formtitle_medium_input_focused')\" onblur=\"javascript:textbox_leave('formmaintitlestatement', 'formtitle_medium_input')\" /></td></tr>");

            // Add the part numbers
            popup_form_builder.Append("    <tr><td>Part Numbers:</td><td colspan=\"2\">");
            if ( Bib.Bib_Info.Main_Title.Part_Numbers_Count > 0 )
            {
                popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formmaintitlepartnum1\" id=\"formmaintitlepartnum1\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Part_Numbers[0]) + "\" onfocus=\"javascript:textbox_enter('formmaintitlepartnum1', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formmaintitlepartnum1', 'formtitle_small_input')\" />");
            }
            else
            {
                popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formmaintitlepartnum1\" id=\"formmaintitlepartnum1\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('formmaintitlepartnum1', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formmaintitlepartnum1', 'formtitle_small_input')\" />");
            }
            if (Bib.Bib_Info.Main_Title.Part_Numbers_Count > 1)
            {
                popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formmaintitlepartnum2\" id=\"formmaintitlepartnum2\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Part_Numbers[1]) + "\" onfocus=\"javascript:textbox_enter('formmaintitlepartnum2', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formmaintitlepartnum2', 'formtitle_small_input')\" />");
            }
            else
            {
                popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formmaintitlepartnum2\" id=\"formmaintitlepartnum2\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('formmaintitlepartnum2', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formmaintitlepartnum2', 'formtitle_small_input')\" />");
            }
            popup_form_builder.AppendLine("</td></tr>");

            // Add the part names and authority
            popup_form_builder.Append("    <tr><td>Part Names:</td><td>");
            if (Bib.Bib_Info.Main_Title.Part_Names_Count > 0)
            {
                popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formmaintitlepartname1\" id=\"formmaintitlepartname1\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Part_Names[0]) + "\" onfocus=\"javascript:textbox_enter('formmaintitlepartname1', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formmaintitlepartname1', 'formtitle_small_input')\" />");
            }
            else
            {
                popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formmaintitlepartname1\" id=\"formmaintitlepartname1\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('formmaintitlepartname1', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formmaintitlepartname1', 'formtitle_small_input')\" />");
            }
            if (Bib.Bib_Info.Main_Title.Part_Names_Count > 1)
            {
                popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formmaintitlepartname2\" id=\"formmaintitlepartname2\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Part_Names[1]) + "\" onfocus=\"javascript:textbox_enter('formmaintitlepartname2', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formmaintitlepartname2', 'formtitle_small_input')\" />");
            }
            else
            {
                popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formmaintitlepartname2\" id=\"formmaintitlepartname2\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('formmaintitlepartname2', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formmaintitlepartname2', 'formtitle_small_input')\" />");
            }
            popup_form_builder.Append("<td>Authority: &nbsp; <input class=\"formtitle_small_input\" name=\"formmaintitleauthority\" id=\"formmaintitleauthority\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Main_Title.Authority) + "\"  onfocus=\"javascript:textbox_enter('formmaintitleauthority', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formmaintitleauthority', 'formtitle_small_input')\" />");
            popup_form_builder.AppendLine("</td></tr>");
            popup_form_builder.AppendLine("  </table>");
            popup_form_builder.AppendLine("  <br />");

            // Add the close button
            popup_form_builder.AppendLine("  <center><a href=\"#template\" onclick=\"return close_maintitle_form('form_maintitle');\"><img border=\"0\" src=\"" + Close_Button_URL(Skin_Code, Base_URL ) + "\" alt=\"CLOSE\" /></a></center>");
            popup_form_builder.AppendLine("</div>");
            popup_form_builder.AppendLine();
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
                List<Note_Info> deletes = Bib.Bib_Info.Notes.Where(thisNote => thisNote.Note_Type == Note_Type_Enum.statement_of_responsibility).ToList();
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
                        Bib.Bib_Info.Add_Note( statement, Note_Type_Enum.statement_of_responsibility );
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
