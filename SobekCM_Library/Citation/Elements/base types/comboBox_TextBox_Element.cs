#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using SobekCM.Core.Configuration;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Abstract base class for all elements which are made up of a single combo/select box followed by a text box  </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    public abstract class comboBox_TextBox_Element : abstract_Element
    {
        /// <summary> Flag indicates if the text box should be cleared when the combo box changes </summary>
        protected bool clear_textbox_on_combobox_change;

        /// <summary> Protected field holds the default value(s) for the combo box </summary>
        protected List<string> default_codes;

        /// <summary> Protected field holds the default value(s) for the text box </summary>
        protected List<string> default_values;

        /// <summary> Protected field holds all of the possible, selectable values for the combo box </summary>
        protected List<string> possible_select_items;

        /// <summary> Protected field holds any label to place before the text box, after the combo box </summary>
        protected string second_label;

        /// <summary> Constructor for a new instance of the comboBox_TextBox_Element class </summary>
        /// <param name="Title"> Title for this element </param>
        /// <param name="Html_Element_Name"> Name for the html components and styles for this element </param>
        protected comboBox_TextBox_Element(string Title, string Html_Element_Name)
        {
            base.Title = Title;
            html_element_name = Html_Element_Name;
            Restrict_Values = false;
            possible_select_items = new List<string>();
            second_label = String.Empty;

            default_codes = new List<string>();
            default_values = new List<string>();
        }

        /// <summary> Flag indicates if values are limited to those in the drop down list. </summary>
        protected bool Restrict_Values { get; set; }

        /// <summary> Adds a possible, selectable value to the combo/select box </summary>
        /// <param name="newitem"> New possible, selectable value </param>
        public void Add_Item(string newitem )
        {
            possible_select_items.Add(newitem);
        }

        /// <summary> Method helps to render the html for all elements based on the comboBox_TextBox_Element class </summary>
        /// <param name="Output"> Output for the generate html for this element </param>
        /// <param name="select_value"> Value for the current digital resource to display in the combo box</param>
        /// <param name="text_value"> Value for the current digital resource to display in the text box</param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        protected void render_helper(TextWriter Output, string select_value, string text_value, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL)
        {
            render_helper(Output, select_value, possible_select_items, text_value, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL, false);
        }

        /// <summary> Method helps to render the html for all elements based on comboBox_TextBox_Element class </summary>
        /// <param name="Output"> Output for the generate html for this element </param>
        /// <param name="select_value"> Value for the current digital resource to display in the combo box</param>
        /// <param name="text_value"> Value for the current digital resource to display in the text box</param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        /// <param name="initial_value"> Flag indicates if the value in the select_value param is actually instructional text, and not a true value</param>
        protected void render_helper(TextWriter Output, string select_value, string text_value, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL, bool initial_value)
        {
            render_helper(Output, select_value, possible_select_items, text_value, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL, initial_value);
        }


        /// <summary> Method helps to render the html for all elements based on comboBox_TextBox_Element class </summary>
        /// <param name="Output"> Output for the generate html for this element </param>
        /// <param name="select_values"> Value(s) for the current digital resource to display in the combo box</param>
        /// <param name="text_values"> Value(s) for the current digital resource to display in the text box </param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        protected void render_helper(TextWriter Output, List<string> select_values, List<string> text_values, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL)
        {
            if (text_values.Count == 0)
            {
                render_helper(Output, String.Empty, String.Empty, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
            }

            string id_name = html_element_name.Replace("_", "");

            Output.WriteLine("  <!-- " + Title + " Element -->");
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
            for (int i = 1; i <= text_values.Count; i++)
            {
                // Write the combo box
                if (clear_textbox_on_combobox_change)
                {
                    Output.Write("            <select class=\"" + html_element_name + "_select\" name=\"" + id_name + "_select" + i + "\" id=\"" + id_name + "_select" + i + "\" onblur=\"javascript:selectbox_leave('" + id_name + "_select" + i + "','" + html_element_name + "_select', '" + html_element_name + "_select_init')\" onchange=\"clear_textbox('" + id_name + "_text" + i + "')\" >");
                }
                else
                {
                    Output.Write("            <select class=\"" + html_element_name + "_select\" name=\"" + id_name + "_select" + i + "\" id=\"" + id_name + "_select" + i + "\" onblur=\"javascript:selectbox_leave('" + id_name + "_select" + i + "','" + html_element_name + "_select', '" + html_element_name + "_select_init')\" >");
                }

                bool found_option = false;
                foreach (string thisOption in possible_select_items)
                {
                    if ((i < possible_select_items.Count) && (thisOption == select_values[i - 1]))
                    {
                        Output.Write("<option value=\"" + thisOption + "\" selected=\"selected=\">" + thisOption + "</option>");
                        found_option = true;
                    }
                    else
                    {
                        Output.Write("<option value=\"" + thisOption + "\" >" + thisOption + "</option>");
                    }
                }

                if ((i <= select_values.Count) && (select_values[i - 1].Length > 0) && (!Restrict_Values) && (!found_option))
                {
                    Output.Write("<option value=\"" + select_values[i - 1] + "\" selected=\"selected=\">" + select_values[i - 1] + "</option>");
                }
                Output.Write("</select>");

                // Write the second text
                if (second_label.Length > 0)
                {
                    Output.Write("<span class=\"metadata_sublabel\">" + second_label + ":</span>");
                }

                // Write the text box
				Output.Write("<input name=\"" + id_name + "_text" + i + "\" id=\"" + id_name + "_text" + i + "\" class=\"" + html_element_name + "_input sbk_Focusable\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(text_values[i - 1]) + "\" />");

                Output.WriteLine(i == text_values.Count ? "</div>" : "<br />");
            }

            Output.WriteLine("        </td>");
            Output.WriteLine("        <td valign=\"bottom\" >");
            if (Repeatable)
            {
                Output.WriteLine("          <a title=\"" + Translator.Get_Translation("Click to add another " + Title.ToLower(), CurrentLanguage) + ".\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onmousedown=\"return " + html_element_name + "_add_new_item();\"><img class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" /></a>");
            }

            Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img class=\"help_button\" src=\"" + Base_URL + HELP_BUTTON_URL + "\" /></a>");

            Output.WriteLine("        </td>");
            Output.WriteLine("      </tr>");
            Output.WriteLine("    </table>");

            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine();
        }

        /// <summary> Method helps to render the html for all elements based on comboBox_TextBox_Element class </summary>
        /// <param name="Output"> Output for the generate html for this element </param>
        /// <param name="select_value"> Value for the current digital resource to display in the combo box</param>
        /// <param name="userdefined_possible"> List of possible select values, set by the user </param>
        /// <param name="text_value"> Value for the current digital resource to display in the text box</param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        /// <param name="initial_value"> Flag indicates if the value in the select_value param is actually instructional text, and not a true value</param>
        protected void render_helper(TextWriter Output, string select_value, List<string> userdefined_possible, string text_value, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL, bool initial_value)
        {
            string id_name = html_element_name.Replace("_", "");

            Output.WriteLine("  <!-- " + Title + " Element -->");
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

            const int i = 1;
            
                // Write the combo box
            // Write the combo box
            if (clear_textbox_on_combobox_change)
            {
                Output.Write("            <select class=\"" + html_element_name + "_select\" name=\"" + id_name + "_select" + i + "\" id=\"" + id_name + "_select" + i + "\" onblur=\"javascript:selectbox_leave('" + id_name + "_select" + i + "','" + html_element_name + "_select', '" + html_element_name + "_select_init')\" onchange=\"clear_textbox('" + id_name + "_text" + i + "')\" >");
            }
            else
            {
                Output.Write("            <select class=\"" + html_element_name + "_select\" name=\"" + id_name + "_select" + i + "\" id=\"" + id_name + "_select" + i + "\" onblur=\"javascript:selectbox_leave('" + id_name + "_select" + i + "','" + html_element_name + "_select', '" + html_element_name + "_select_init')\" >");
            }


                bool found_option = false;
                foreach (string thisOption in possible_select_items)
                {
                    if ((i < possible_select_items.Count) && (thisOption == select_value))
                    {
                        Output.Write("<option value=\"" + thisOption + "\" selected=\"selected=\">" + thisOption + "</option>");
                        found_option = true;
                    }
                    else
                    {
                        Output.Write("<option value=\"" + thisOption + "\" >" + thisOption + "</option>");
                    }
                }

                if ((select_value.Length > 0) && (!Restrict_Values) && (!found_option))
                {
                    Output.Write("<option value=\"" + select_value + "\" selected=\"selected=\">" + select_value + "</option>");
                }
                Output.Write("</select>");

                // Write the second text
                if (second_label.Length > 0)
                {
                    Output.Write("<span class=\"metadata_sublabel\">" + second_label + ":</span>");
                }

                // Write the text box
				Output.Write("<input name=\"" + id_name + "_text" + i + "\" id=\"" + id_name + "_text" + i + "\" class=\"" + html_element_name + "_input sbk_Focusable\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(text_value) + "\" />");

                Output.WriteLine("</div>");
        

            Output.WriteLine("        </td>");
            Output.WriteLine("        <td style=\"vertical-align:bottom\" >");
            if (Repeatable)
            {
                Output.WriteLine("          <a title=\"" + Translator.Get_Translation("Click to add another " + Title.ToLower(), CurrentLanguage) + ".\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onmousedown=\"return " + html_element_name + "_add_new_item();\"><img class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" /></a>");
            }

            Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img class=\"help_button\" src=\"" + Base_URL + HELP_BUTTON_URL + "\" /></a>");

            Output.WriteLine("        </td>");
            Output.WriteLine("      </tr>");
            Output.WriteLine("    </table>");

            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine();
        }

        #region Methods Implementing the Abstract Methods from abstract_Element class

        /// <summary> Reads the inner data from the Template XML format </summary>
        /// <param name="XMLReader"> Current template xml configuration reader </param>
        /// <remarks> This reads the possible values for the combo box from an <i>options</i> subelement.  The default value for the combo box is from a <i>code</i> subelement and the default value for the text box is from a <i>statement</i> subelement. </remarks>
        protected override void Inner_Read_Data(XmlTextReader XMLReader)
        {
            while (XMLReader.Read())
            {
                if ((XMLReader.NodeType == XmlNodeType.Element) && (XMLReader.Name.ToLower() == "options"))
                {
                    XMLReader.Read();
                    string options = XMLReader.Value.Trim();
                    possible_select_items.Clear();
                    if (options.Length > 0)
                    {
                        SortedList<string, string> sorted_codes = new SortedList<string, string>();
                        string[] options_parsed = options.Split(",".ToCharArray());
                        foreach (string thisOption in options_parsed.Where(thisOption => !sorted_codes.ContainsKey(thisOption.Trim())))
                        {
                            sorted_codes.Add(thisOption.Trim(), thisOption.Trim());
                        }

                        possible_select_items.AddRange(sorted_codes.Values);
                    }
                }

                if ((XMLReader.NodeType == XmlNodeType.Element) && (XMLReader.Name.ToLower() == "code"))
                {
                    XMLReader.Read();
                    default_codes.Add(XMLReader.Value.Trim());
                }

                if ((XMLReader.NodeType == XmlNodeType.Element) && (XMLReader.Name.ToLower() == "statement"))
                {
                    XMLReader.Read();
                    default_values.Add(XMLReader.Value.Trim());
                }
            }
        }

        #endregion
    }
}
