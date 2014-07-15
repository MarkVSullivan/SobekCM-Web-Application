﻿#region Using directives

using System;
using System.IO;
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
    /// <summary> Element displays a form to allow for complete entry of other titles for an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    class Other_Title_Form_Element : abstract_Element
    {
        /// <summary> Constructor for a new instance of the Other_Title_Form_Element class </summary>
        public Other_Title_Form_Element()
        {
            Type = Element_Type.Title_Other;
            Repeatable = true;
            Display_SubType = "form";
            Title = "Other Titles";
            html_element_name = "form_title_other";
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
                const string defaultAcronym = "Enter any other titles which relate to this material";
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
            Output.WriteLine("      <div class=\"form_title_div\">");

            int title_count = 1;
            if (( Bib.Bib_Info.hasSeriesTitle ) && ( Bib.Bib_Info.SeriesTitle.Title.Length > 0))
            {
                Title_Info thisTitle = Bib.Bib_Info.SeriesTitle;

                // Add the link for the series title
                Output.Write("        <a title=\"Click to edit this other title\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_othertitle_line_" + title_count + "')\" onblur=\"link_blurred2('form_othertitle_line_" + title_count + "')\" onkeypress=\"return popup_keypress_focus('form_othertitle_" + title_count + "', 'form_othertitle_line_" + title_count + "', 'formothertitletitle_" + title_count + "', 295, 675, '" + isMozilla.ToString() + "' );\" onclick=\"return popup_focus('form_othertitle_" + title_count + "', 'form_othertitle_line_" + title_count + "', 'formothertitletitle_" + title_count + "', 295, 675 );\"><div class=\"form_linkline form_title_main_line\" id=\"form_othertitle_line_" + title_count + "\">" + thisTitle.NonSort + thisTitle.Title);
                if (thisTitle.Subtitle.Length > 0)
                    Output.Write(" : " + thisTitle.Subtitle);

                Output.WriteLine(" ( <i>Series Title</i> )</div></a>");

                // Add the popup form
                popup_form_builder.AppendLine("<!-- Other Title Form " + title_count + " -->");
                popup_form_builder.AppendLine("<div class=\"title_other_popup_div\" id=\"form_othertitle_" + title_count + "\" style=\"display:none;\">");
                popup_form_builder.AppendLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">EDIT OTHER TITLE</td><td align=\"right\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" alt=\"HELP\" target=\"_" + html_element_name.ToUpper() + "\" >?</a> &nbsp; <a href=\"#template\" alt=\"CLOSE\" onclick=\"close_othertitle_form('form_othertitle_" + title_count + "')\">X</a> &nbsp; </td></tr></table></div>");
                popup_form_builder.AppendLine("  <br />");
                popup_form_builder.AppendLine("  <table class=\"popup_table\">");

                // Add the title type (and optionally display label)
                popup_form_builder.Append("    <tr><td width=\"90px\">Title Type:</td><td colspan=\"2\"><select class=\"formtitle_type_select\" name=\"formothertitletype_" + title_count + "\" id=\"formothertitletype_" + title_count + "\" onchange=\"javascript:other_title_type_change(" + title_count + ")\" >");
                popup_form_builder.Append("<option value=\"abbreviated\">Abbreviated Title</option>");
                popup_form_builder.Append("<option value=\"alternate\">Alternative Title</option>");
                popup_form_builder.Append("<option value=\"series\"  selected=\"selected\" >Series Title</option>");
                popup_form_builder.Append("<option value=\"translated\">Translated Title</option>");
                popup_form_builder.Append("<option value=\"uniform\">Uniform Title</option>");


                popup_form_builder.AppendLine("</select>");
                popup_form_builder.AppendLine("        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;");
                popup_form_builder.AppendLine("        <span id=\"formothertitlesubtype_" + title_count + "\" style=\"display:none;\">Display Label: ");
                popup_form_builder.Append("        <select class=\"formtitle_display_select\" name=\"formothertitledisplay_" + title_count + "\" id=\"formothertitledisplay_" + title_count + "\" >");
                //popup_form_builder.Append("<option value=\"added\">Added title page title</option>");
                //popup_form_builder.Append("<option value=\"alternate\" selected=\"selected\" >Alternate title</option>");
                //popup_form_builder.Append("<option value=\"caption\">Caption title</option>");
                //popup_form_builder.Append("<option value=\"cover\">Cover title</option>");
                //popup_form_builder.Append("<option value=\"distinctive\">Distinctive title</option>");
                //popup_form_builder.Append("<option value=\"other\">Other title</option>");
                //popup_form_builder.Append("<option value=\"portion\">Portion of title</option>");
                //popup_form_builder.Append("<option value=\"parallel\">Parallel title</option>");
                //popup_form_builder.Append("<option value=\"running\">Running title</option>");
                //popup_form_builder.Append("<option value=\"spine\">Spine title</option>");
                popup_form_builder.AppendLine("</select></span></td></tr>");

                // Add the nonsort and language text boxes
                popup_form_builder.Append("    <tr><td>Non Sort:</td><td>");
                popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlenonsort_" + title_count + "\" id=\"formothertitlenonsort_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.NonSort) + "\"  onfocus=\"javascript:textbox_enter('formothertitlenonsort_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlenonsort_" + title_count + "', 'formtitle_small_input')\" />");
                popup_form_builder.Append("</td><td width=\"255px\" >Language: &nbsp; ");
                popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlelanguage_" + title_count + "\" id=\"formothertitlelanguage_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Language) + "\" onfocus=\"javascript:textbox_enter('formothertitlelanguage_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlelanguage_" + title_count + "', 'formtitle_small_input')\" />");
                popup_form_builder.AppendLine("</td></tr>");

                // Add the title and subtitle
                popup_form_builder.AppendLine("    <tr><td>Title:</td><td colspan=\"2\"><input class=\"formtitle_large_input\" name=\"formothertitletitle_" + title_count + "\" id=\"formothertitletitle_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Title) + "\" onfocus=\"javascript:textbox_enter('formothertitletitle_" + title_count + "', 'formtitle_large_input_focused')\" onblur=\"javascript:textbox_leave('formothertitletitle_" + title_count + "', 'formtitle_large_input')\" /></td></tr>");
                popup_form_builder.AppendLine("    <tr><td>Sub Title:</td><td colspan=\"2\"><input class=\"formtitle_large_input\" name=\"formothertitlesubtitle_" + title_count + "\" id=\"formothertitlesubtitle_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Subtitle) + "\" onfocus=\"javascript:textbox_enter('formothertitlesubtitle_" + title_count + "', 'formtitle_large_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlesubtitle_" + title_count + "', 'formtitle_large_input')\" /></td></tr>");

                // Add the part numbers
                popup_form_builder.Append("    <tr><td>Part Numbers:</td><td colspan=\"2\">");
                if (thisTitle.Part_Numbers_Count > 0)
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartnum1_" + title_count + "\" id=\"formothertitlepartnum1_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Part_Numbers[0]) + "\" onfocus=\"javascript:textbox_enter('formothertitlepartnum1_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartnum1_" + title_count + "', 'formtitle_small_input')\" />");
                }
                else
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartnum1_" + title_count + "\" id=\"formothertitlepartnum1_" + title_count + "\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('formothertitlepartnum1_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartnum1_" + title_count + "', 'formtitle_small_input')\" />");
                }
                if (thisTitle.Part_Numbers_Count > 1)
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartnum2_" + title_count + "\" id=\"formothertitlepartnum2_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Part_Numbers[1]) + "\" onfocus=\"javascript:textbox_enter('formothertitlepartnum2_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartnum2_" + title_count + "', 'formtitle_small_input')\" />");
                }
                else
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartnum2_" + title_count + "\" id=\"formothertitlepartnum2_" + title_count + "\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('formothertitlepartnum2_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartnum2_" + title_count + "', 'formtitle_small_input')\" />");
                }
                popup_form_builder.AppendLine("</td></tr>");

                // Add the part names and authority
                popup_form_builder.Append("    <tr><td>Part Names:</td><td>");
                if (thisTitle.Part_Names_Count > 0)
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartname1_" + title_count + "\" id=\"formothertitlepartname1_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Part_Names[0]) + "\" onfocus=\"javascript:textbox_enter('formothertitlepartname1_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartname1_" + title_count + "', 'formtitle_small_input')\" />");
                }
                else
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartname1_" + title_count + "\" id=\"formothertitlepartname1_" + title_count + "\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('formothertitlepartname1_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartname1_" + title_count + "', 'formtitle_small_input')\" />");
                }
                if (thisTitle.Part_Names_Count > 1)
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartname2_" + title_count + "\" id=\"formothertitlepartname2_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Part_Names[1]) + "\" onfocus=\"javascript:textbox_enter('formothertitlepartname2_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartname2_" + title_count + "', 'formtitle_small_input')\" />");
                }
                else
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartname2_" + title_count + "\" id=\"formothertitlepartname2_" + title_count + "\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('formothertitlepartname2_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartname2_" + title_count + "', 'formtitle_small_input')\" />");
                }
                popup_form_builder.Append("<td>Authority: &nbsp; <input class=\"formtitle_small_input\" name=\"formothertitleauthority_" + title_count + "\" id=\"formothertitleauthority_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Authority) + "\"  onfocus=\"javascript:textbox_enter('formothertitleauthority_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitleauthority_" + title_count + "', 'formtitle_small_input')\" />");
                popup_form_builder.AppendLine("</td></tr>");
                popup_form_builder.AppendLine("  </table>");
                popup_form_builder.AppendLine("  <br />");

                // Add the close button
                popup_form_builder.AppendLine("  <center><a href=\"#template\" onclick=\"return close_othertitle_form('form_othertitle_" + title_count + "');\"><img border=\"0\" src=\"" + Close_Button_URL(Skin_Code, Base_URL ) + "\" alt=\"CLOSE\" /></a></center>");
                popup_form_builder.AppendLine("</div>");
                popup_form_builder.AppendLine();

                title_count++;
            }

            // Always have one empty other title
            if ((title_count == 1) && (Bib.Bib_Info.Other_Titles_Count == 0))
            {
                Bib.Bib_Info.Add_Other_Title(String.Empty, Title_Type_Enum.alternative);
            }

            foreach (Title_Info thisTitle in Bib.Bib_Info.Other_Titles)
            {
                // Add the link for the other title
                if ((thisTitle.Title.Trim().Length > 0) || ( thisTitle.NonSort.Trim().Length > 0 ))
                {
                    Output.Write("        <a title=\"Click to edit this other title\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_othertitle_line_" + title_count + "')\" onblur=\"link_blurred2('form_othertitle_line_" + title_count + "')\" onkeypress=\"return popup_keypress_focus('form_othertitle_" + title_count + "', 'form_othertitle_line_" + title_count + "', 'formothertitletitle_" + title_count + "', 295, 675, '" + isMozilla.ToString() + "' );\" onclick=\"return popup_focus('form_othertitle_" + title_count + "', 'form_othertitle_line_" + title_count + "', 'formothertitletitle_" + title_count + "', 295, 675 );\"><div class=\"form_linkline form_title_main_line\" id=\"form_othertitle_line_" + title_count + "\">" + thisTitle.NonSort + thisTitle.Title);
                    if (thisTitle.Subtitle.Length > 0)
                        Output.Write(" : " + thisTitle.Subtitle);
                    switch (thisTitle.Title_Type)
                    {
                        case Title_Type_Enum.abbreviated:
                            Output.Write(" ( <i>Abbreviated Title</i> )");
                            break;

                        case Title_Type_Enum.translated:
                            Output.Write(" ( <i>Translated Title</i> )");
                            break;

                        case Title_Type_Enum.uniform:
                            Output.Write(" ( <i>Uniform Title</i> )");
                            break;

                        default:
                            Output.Write(" ( <i>Alternative Title</i> )");
                            break;
                    }
                }
                else
                {
                    Output.Write("        <a title=\"Click to edit this other title\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_othertitle_line_" + title_count + "')\" onblur=\"link_blurred2('form_othertitle_line_" + title_count + "')\" onkeypress=\"return popup_keypress_focus('form_othertitle_" + title_count + "', 'form_othertitle_line_" + title_count + "', 'formothertitletitle_" + title_count + "', 295, 675, '" + isMozilla.ToString() + "' );\" onclick=\"return popup_focus('form_othertitle_" + title_count + "', 'form_othertitle_line_" + title_count + "', 'formothertitletitle_" + title_count + "', 295, 675 );\"><div class=\"form_linkline_empty form_title_main_line\" id=\"form_othertitle_line_" + title_count + "\"><i>Empty Other Title</i>");
                }

                Output.Write("</div></a>");

                // Add the popup form
                popup_form_builder.AppendLine("<!-- Other Title Form " + title_count + " -->");
                popup_form_builder.AppendLine("<div class=\"title_other_popup_div\" id=\"form_othertitle_" + title_count + "\" style=\"display:none;\">");
                popup_form_builder.AppendLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">EDIT OTHER TITLE</td><td align=\"right\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" alt=\"HELP\" target=\"_" + html_element_name.ToUpper() + "\" >?</a> &nbsp; <a href=\"#template\" alt=\"CLOSE\" onclick=\"close_othertitle_form('form_othertitle_" + title_count + "')\">X</a> &nbsp; </td></tr></table></div>");
                popup_form_builder.AppendLine("  <br />");
                popup_form_builder.AppendLine("  <table class=\"popup_table\">");

                // Add the title type (and optionally display label)
                popup_form_builder.Append("    <tr><td width=\"90px\">Title Type:</td><td colspan=\"2\"><select class=\"formtitle_type_select\" name=\"formothertitletype_" + title_count + "\" id=\"formothertitletype_" + title_count + "\" onchange=\"javascript:other_title_type_change(" + title_count + ")\" >");
                popup_form_builder.Append(thisTitle.Title_Type == Title_Type_Enum.abbreviated
                                              ? "<option value=\"abbreviated\" selected=\"selected\" >Abbreviated Title</option>"
                                              : "<option value=\"abbreviated\">Abbreviated Title</option>");

                if ((thisTitle.Title_Type == Title_Type_Enum.alternative) || ( thisTitle.Title_Type == Title_Type_Enum.UNSPECIFIED ))
                {
                    popup_form_builder.Append("<option value=\"alternate\" selected=\"selected\" >Alternative Title</option>");
                }
                else
                {
                    popup_form_builder.Append("<option value=\"alternate\">Alternative Title</option>");
                }
                popup_form_builder.Append("<option value=\"series\">Series Title</option>");

                popup_form_builder.Append(thisTitle.Title_Type == Title_Type_Enum.translated
                                              ? "<option value=\"translated\" selected=\"selected\" >Translated Title</option>"
                                              : "<option value=\"translated\">Translated Title</option>");

                popup_form_builder.Append(thisTitle.Title_Type == Title_Type_Enum.uniform
                                              ? "<option value=\"uniform\" selected=\"selected\" >Uniform Title</option>"
                                              : "<option value=\"uniform\">Uniform Title</option>");

                popup_form_builder.AppendLine("</select>");

                // Should the SELECT options be pre-established?
                if ((thisTitle.Title_Type == Title_Type_Enum.alternative) || (thisTitle.Title_Type == Title_Type_Enum.uniform))
                {
 
                    popup_form_builder.AppendLine("        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;");
                    popup_form_builder.AppendLine("        <span id=\"formothertitlesubtype_" + title_count + "\">Display Label: ");
                    popup_form_builder.Append(" <select class=\"formtitle_display_select\" name=\"formothertitledisplay_" + title_count + "\" id=\"formothertitledisplay_" + title_count + "\" >");

                    if (thisTitle.Title_Type == Title_Type_Enum.alternative)
                    {
                        popup_form_builder.Append(thisTitle.Display_Label == "Added title page title"
                                                      ? "<option value=\"added\" selected=\"selected\" >Added title page title</option>"
                                                      : "<option value=\"added\">Added title page title</option>");

                        popup_form_builder.Append(thisTitle.Display_Label == "Alternate title"
                                                      ? "<option value=\"alternate\" selected=\"selected\">Alternate title</option>"
                                                      : "<option value=\"alternate\">Alternate title</option>");

                        popup_form_builder.Append(thisTitle.Display_Label == "Caption title"
                                                      ? "<option value=\"caption\" selected=\"selected\">Caption title</option>"
                                                      : "<option value=\"caption\">Caption title</option>");

                        popup_form_builder.Append(thisTitle.Display_Label == "Cover title"
                                                      ? "<option value=\"cover\" selected=\"selected\">Cover title</option>"
                                                      : "<option value=\"cover\">Cover title</option>");

                        popup_form_builder.Append(thisTitle.Display_Label == "Distinctive title"
                                                      ? "<option value=\"distinctive\" selected=\"selected\">Distinctive title</option>"
                                                      : "<option value=\"distinctive\">Distinctive title</option>");

                        popup_form_builder.Append(thisTitle.Display_Label == "Other title"
                                                      ? "<option value=\"other\" selected=\"selected\">Other title</option>"
                                                      : "<option value=\"other\">Other title</option>");

                        popup_form_builder.Append(thisTitle.Display_Label == "Portion of title"
                                                      ? "<option value=\"portion\" selected=\"selected\">Portion of title</option>"
                                                      : "<option value=\"portion\">Portion of title</option>");

                        popup_form_builder.Append(thisTitle.Display_Label == "Parallel title"
                                                      ? "<option value=\"parallel\" selected=\"selected\">Parallel title</option>"
                                                      : "<option value=\"parallel\">Parallel title</option>");

                        popup_form_builder.Append(thisTitle.Display_Label == "Running title"
                                                      ? "<option value=\"running\" selected=\"selected\">Running title</option>"
                                                      : "<option value=\"running\">Running title</option>");

                        popup_form_builder.Append(thisTitle.Display_Label == "Spine title"
                                                      ? "<option value=\"spine\" selected=\"selected\">Spine title</option>"
                                                      : "<option value=\"spine\">Spine title</option>");
                    }
                    else
                    {
                        popup_form_builder.Append(thisTitle.Display_Label == "Main Entry"
                                                      ? "<option value=\"main\" selected=\"selected\">Main Entry</option>"
                                                      : "<option value=\"main\">Main Entry</option>");

                        popup_form_builder.Append(thisTitle.Display_Label == "Uncontrolled Added Entry"
                                                      ? "<option value=\"uncontrolled\" selected=\"selected\">Uncontrolled Added Entry</option>"
                                                      : "<option value=\"uncontrolled\">Uncontrolled Added Entry</option>");

                        if ((thisTitle.Display_Label != "Main Entry") && (thisTitle.Display_Label != "Uncontrolled Added Entry"))
                            popup_form_builder.Append("<option value=\"uniform\" selected=\"selected\">Uniform Title</option>");
                        else
                            popup_form_builder.Append("<option value=\"uniform\">Uniform Title</option>");
                    }


                    popup_form_builder.AppendLine("</select></span></td></tr>");
                }
                else
                {
                    popup_form_builder.AppendLine("        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;");
                    popup_form_builder.AppendLine("        <span id=\"formothertitlesubtype_" + title_count + "\" style=\"display:none;\">Display Label: ");
                    popup_form_builder.Append("        <select class=\"formtitle_display_select\" name=\"formothertitledisplay_" + title_count + "\" id=\"formothertitledisplay_" + title_count + "\" >");
                    //popup_form_builder.Append("<option value=\"added\">Added title page title</option>");
                    //popup_form_builder.Append("<option value=\"alternate\">Alternate title</option>");
                    //popup_form_builder.Append("<option value=\"caption\">Caption title</option>");
                    //popup_form_builder.Append("<option value=\"cover\">Cover title</option>");
                    //popup_form_builder.Append("<option value=\"distinctive\">Distinctive title</option>");
                    //popup_form_builder.Append("<option value=\"other\">Other title</option>");
                    //popup_form_builder.Append("<option value=\"portion\">Portion of title</option>");
                    //popup_form_builder.Append("<option value=\"parallel\">Parallel title</option>");
                    //popup_form_builder.Append("<option value=\"running\">Running title</option>");
                    //popup_form_builder.Append("<option value=\"spine\">Spine title</option>");
                    popup_form_builder.AppendLine("</select></span></td></tr>");
                }
                    
                // Add the nonsort and language text boxes
                popup_form_builder.Append("    <tr><td>Non Sort:</td><td>");
                popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlenonsort_" + title_count + "\" id=\"formothertitlenonsort_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.NonSort) + "\"  onfocus=\"javascript:textbox_enter('formothertitlenonsort_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlenonsort_" + title_count + "', 'formtitle_small_input')\" />");
                popup_form_builder.Append("</td><td width=\"255px\" >Language: &nbsp; ");
                popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlelanguage_" + title_count + "\" id=\"formothertitlelanguage_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Language) + "\" onfocus=\"javascript:textbox_enter('formothertitlelanguage_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlelanguage_" + title_count + "', 'formtitle_small_input')\" />");
                popup_form_builder.AppendLine("</td></tr>");

                // Add the title and subtitle
                popup_form_builder.AppendLine("    <tr><td>Title:</td><td colspan=\"2\"><input class=\"formtitle_large_input\" name=\"formothertitletitle_" + title_count + "\" id=\"formothertitletitle_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Title) + "\" onfocus=\"javascript:textbox_enter('formothertitletitle_" + title_count + "', 'formtitle_large_input_focused')\" onblur=\"javascript:textbox_leave('formothertitletitle_" + title_count + "', 'formtitle_large_input')\" /></td></tr>");
                popup_form_builder.AppendLine("    <tr><td>Sub Title:</td><td colspan=\"2\"><input class=\"formtitle_large_input\" name=\"formothertitlesubtitle_" + title_count + "\" id=\"formothertitlesubtitle_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Subtitle) + "\" onfocus=\"javascript:textbox_enter('formothertitlesubtitle_" + title_count + "', 'formtitle_large_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlesubtitle_" + title_count + "', 'formtitle_large_input')\" /></td></tr>");

                // Add the part numbers
                popup_form_builder.Append("    <tr><td>Part Numbers:</td><td colspan=\"2\">");
                if (thisTitle.Part_Numbers_Count > 0)
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartnum1_" + title_count + "\" id=\"formothertitlepartnum1_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Part_Numbers[0]) + "\" onfocus=\"javascript:textbox_enter('formothertitlepartnum1_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartnum1_" + title_count + "', 'formtitle_small_input')\" />");
                }
                else
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartnum1_" + title_count + "\" id=\"formothertitlepartnum1_" + title_count + "\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('formothertitlepartnum1_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartnum1_" + title_count + "', 'formtitle_small_input')\" />");
                }
                if (thisTitle.Part_Numbers_Count > 1)
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartnum2_" + title_count + "\" id=\"formothertitlepartnum2_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Part_Numbers[1]) + "\" onfocus=\"javascript:textbox_enter('formothertitlepartnum2_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartnum2_" + title_count + "', 'formtitle_small_input')\" />");
                }
                else
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartnum2_" + title_count + "\" id=\"formothertitlepartnum2_" + title_count + "\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('formothertitlepartnum2_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartnum2_" + title_count + "', 'formtitle_small_input')\" />");
                }
                popup_form_builder.AppendLine("</td></tr>");

                // Add the part names and authority
                popup_form_builder.Append("    <tr><td>Part Names:</td><td>");
                if (thisTitle.Part_Names_Count > 0)
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartname1_" + title_count + "\" id=\"formothertitlepartname1_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Part_Names[0]) + "\" onfocus=\"javascript:textbox_enter('formothertitlepartname1_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartname1_" + title_count + "', 'formtitle_small_input')\" />");
                }
                else
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartname1_" + title_count + "\" id=\"formothertitlepartname1_" + title_count + "\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('formothertitlepartname1_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartname1_" + title_count + "', 'formtitle_small_input')\" />");
                }
                if (thisTitle.Part_Names_Count > 1)
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartname2_" + title_count + "\" id=\"formothertitlepartname2_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Part_Names[1]) + "\" onfocus=\"javascript:textbox_enter('formothertitlepartname2_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartname2_" + title_count + "', 'formtitle_small_input')\" />");
                }
                else
                {
                    popup_form_builder.Append("<input class=\"formtitle_small_input\" name=\"formothertitlepartname2_" + title_count + "\" id=\"formothertitlepartname2_" + title_count + "\" type=\"text\" value=\"\" onfocus=\"javascript:textbox_enter('formothertitlepartname2_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitlepartname2_" + title_count + "', 'formtitle_small_input')\" />");
                }
                popup_form_builder.Append("<td>Authority: &nbsp; <input class=\"formtitle_small_input\" name=\"formothertitleauthority_" + title_count + "\" id=\"formothertitleauthority_" + title_count + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisTitle.Authority) + "\"  onfocus=\"javascript:textbox_enter('formothertitleauthority_" + title_count + "', 'formtitle_small_input_focused')\" onblur=\"javascript:textbox_leave('formothertitleauthority_" + title_count + "', 'formtitle_small_input')\" />");
                popup_form_builder.AppendLine("</td></tr>");
                popup_form_builder.AppendLine("  </table>");
                popup_form_builder.AppendLine("  <br />");

                // Add the close button
                popup_form_builder.AppendLine("  <center><a href=\"#template\" onclick=\"return close_othertitle_form('form_othertitle_" + title_count + "');\"><img border=\"0\" src=\"" + Close_Button_URL(Skin_Code, Base_URL ) + "\" alt=\"CLOSE\" /></a></center>");
                popup_form_builder.AppendLine("</div>");
                popup_form_builder.AppendLine();

                title_count++;
            }

            
            // Add the link to add a new other title
            Output.WriteLine("      </div>");
            Output.WriteLine("          </td>");
            Output.WriteLine("          <td valign=\"bottom\" >");
            if (Repeatable)
            {
                Output.WriteLine("            <img title=\"" + Translator.Get_Translation("Click to add a new other title", CurrentLanguage) + ".\" alt=\"+\" border=\"0px\" class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" onmousedown=\"new_title_link_clicked('" + Template_Page + "');\" />");
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
        /// <remarks> This clears any preexisting series title and other titles (not main title) </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Clear part numbers and part names ( collections ) from the series title
            if ( Bib.Bib_Info.hasSeriesTitle )
                Bib.Bib_Info.SeriesTitle.Clear();
            Bib.Bib_Info.Clear_Other_Titles();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf("formothertitletype_") == 0)
                {
                    string type = HttpContext.Current.Request.Form[thisKey].Trim();
                    string diff = thisKey.Replace("formothertitletype_", "");
                    string display = String.Empty;
                    if ( HttpContext.Current.Request.Form["formothertitledisplay_" + diff] != null )
                        display = HttpContext.Current.Request.Form["formothertitledisplay_" + diff].Trim();
                    string nonsort = HttpContext.Current.Request.Form["formothertitlenonsort_" + diff].Trim();
                    string title = HttpContext.Current.Request.Form["formothertitletitle_" + diff].Trim();
                    string subtitle = HttpContext.Current.Request.Form["formothertitlesubtitle_" + diff].Trim();
                    string partnum1 = HttpContext.Current.Request.Form["formothertitlepartnum1_" + diff].Trim();
                    string partnum2 = HttpContext.Current.Request.Form["formothertitlepartnum2_" + diff].Trim();
                    string partname1 = HttpContext.Current.Request.Form["formothertitlepartname1_" + diff].Trim();
                    string partname2 = HttpContext.Current.Request.Form["formothertitlepartname2_" + diff].Trim();
                    string authority = HttpContext.Current.Request.Form["formothertitleauthority_" + diff].Trim();
                    string language = HttpContext.Current.Request.Form["formothertitlelanguage_" + diff].Trim();

                    if (title.Length > 0)
                    {
                        if (type == "series")
                        {
                            Bib.Bib_Info.SeriesTitle.Title = title;
                            Bib.Bib_Info.SeriesTitle.NonSort = nonsort;
                            Bib.Bib_Info.SeriesTitle.Subtitle = subtitle;
                            Bib.Bib_Info.SeriesTitle.Authority = authority;
                            Bib.Bib_Info.SeriesTitle.Language = language;
                            if (partnum1.Length > 0)
                                Bib.Bib_Info.SeriesTitle.Add_Part_Number(partnum1);
                            if (partnum2.Length > 0)
                                Bib.Bib_Info.SeriesTitle.Add_Part_Number(partnum2);
                            if (partname1.Length > 0)
                                Bib.Bib_Info.SeriesTitle.Add_Part_Name(partname1);
                            if (partname2.Length > 0)
                                Bib.Bib_Info.SeriesTitle.Add_Part_Name(partname2);
                        }
                        else
                        {
                            Title_Info thisTitle = new Title_Info
                                                       {
                                                           Title = title,
                                                           NonSort = nonsort,
                                                           Subtitle = subtitle,
                                                           Authority = authority,
                                                           Language = language
                                                       };
                            if (partnum1.Length > 0)
                                thisTitle.Add_Part_Number(partnum1);
                            if (partnum2.Length > 0)
                                thisTitle.Add_Part_Number(partnum2);
                            if (partname1.Length > 0)
                                thisTitle.Add_Part_Name(partname1);
                            if (partname2.Length > 0)
                                thisTitle.Add_Part_Name(partname2);

                            switch (type)
                            {
                                case "abbreviated":
                                    thisTitle.Title_Type = Title_Type_Enum.abbreviated;
                                    break;

                                case "translated":
                                    thisTitle.Title_Type = Title_Type_Enum.translated;
                                    break;

                                case "uniform":
                                    thisTitle.Title_Type = Title_Type_Enum.uniform;
                                    break;

                                default:
                                    thisTitle.Title_Type = Title_Type_Enum.alternative;
                                    break;
                            }

                            if (thisTitle.Title_Type == Title_Type_Enum.alternative)
                            {
                                switch (display)
                                {
                                    case "added":
                                        thisTitle.Display_Label = "Added title page title";
                                        break;

                                    case "alternate":
                                        thisTitle.Display_Label = "Alternate title";
                                        break;

                                    case "caption":
                                        thisTitle.Display_Label = "Caption title";
                                        break;

                                    case "cover":
                                        thisTitle.Display_Label = "Cover title";
                                        break;

                                    case "distinctive":
                                        thisTitle.Display_Label = "Distinctive title";
                                        break;

                                    case "other":
                                        thisTitle.Display_Label = "Other title";
                                        break;

                                    case "portion":
                                        thisTitle.Display_Label = "Portion of title";
                                        break;

                                    case "parallel":
                                        thisTitle.Display_Label = "Parallel title";
                                        break;

                                    case "running":
                                        thisTitle.Display_Label = "Running title";
                                        break;

                                    case "spine":
                                        thisTitle.Display_Label = "Spine title";
                                        break;
                                }
                            }

                            if (thisTitle.Title_Type == Title_Type_Enum.uniform)
                            {
                                switch (display)
                                {
                                    case "uncontrolled":
                                        thisTitle.Display_Label = "Uncontrolled Added Entry";
                                        break;

                                    case "main":
                                        thisTitle.Display_Label = "Main Entry";
                                        break;
                                    
                                    default:
                                        thisTitle.Display_Label = "Uniform Title";
                                        break;
                                }
                            }

                            Bib.Bib_Info.Add_Other_Title(thisTitle);
                        }

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

