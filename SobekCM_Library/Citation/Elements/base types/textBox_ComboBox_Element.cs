#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Abstract base class for all elements which are made up of a text box followed by a combo/select box </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    public abstract class textBox_ComboBox_Element : abstract_Element
    {
        /// <summary> Protected field contains all the possible strings for the combo/select box </summary>
        protected List<string> possible_select_items_text;

        /// <summary> Protected field contains all the values for the possible strings for the combo/select box </summary>
        protected List<string> possible_select_items_value;

        /// <summary> Protected field contains a possible second label to show before the combo box </summary>
        protected string second_label;


        /// <summary> Constructor for a new instance of the textBox_ComboBox_Element class </summary>
        /// <param name="Title"> Title for this element </param>
        /// <param name="Html_Element_Name"> Name for the html components and styles for this element </param>
        protected textBox_ComboBox_Element(string Title, string Html_Element_Name)
        {
            base.Title = Title;
            html_element_name = Html_Element_Name;
            second_label = String.Empty;

            possible_select_items_text = new List<string>();
            possible_select_items_value = new List<string>();
        }

        /// <summary> Flag indicates if values are limited to those in the drop down list. </summary>
        protected bool Restrict_Values { get; set; }

        /// <summary> Adds a new possible string for the combo/select box, along with associated value </summary>
        /// <param name="text"> Text to display in the combo/select box</param>
        /// <param name="value"> Associated value for this option </param>
        public void Add_Select_Item(string text, string value )
        {
            possible_select_items_text.Add(text);
            possible_select_items_value.Add(value);
        }


        /// <summary> Method helps to render the html for all elements based on textBox_ComboBox_Element class </summary>
        /// <param name="Output"> Output for the generate html for this element </param>
        /// <param name="text_values"> Value(s) for the current digital resource to display in the text box </param>
        /// <param name="select_values"> Value(s) for the current digital resource to display in the combo box</param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        protected void render_helper(TextWriter Output, List<string> text_values, List<string> select_values, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            if (text_values.Count == 0)
            {
                text_values.Add(String.Empty);
                select_values.Add(String.Empty);
            }

            string id_name = html_element_name.Replace("_", "");

            Output.WriteLine("  <!-- " + Title + " Element -->");
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
            Output.WriteLine("      <table><tr><td>");
            Output.WriteLine("      <div id=\"" + html_element_name + "_div\">");
            for (int i = 1; i <= text_values.Count; i++)
            {
                // Write the text box
                Output.Write("        <input name=\"" + id_name + "_text" + i + "\" id=\"" + id_name + "_text" + i + "\" class=\"" + html_element_name + "_input\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(text_values[i - 1]) + "\" onfocus=\"javascript:textbox_enter('" + id_name + "_text" + i + "','" + html_element_name + "_input_focused')\" onblur=\"javascript:textbox_leave('" + id_name + "_text" + i + "','" + html_element_name + "_input')\" />");

                // If there is a second label, draw that
                if (second_label.Length > 0)
                {
                    Output.WriteLine("<span class=\"metadata_sublabel\">" + Translator.Get_Translation(second_label, CurrentLanguage) + ":</span>");
                }
                else
                {
                    Output.WriteLine();
                }

                // Write the combo box
                Output.WriteLine("        <select class=\"" + html_element_name + "_select\" name=\"" + id_name + "_select" + i + "\" id=\"" + id_name + "_select" + i + "\" onfocus=\"javascript:textbox_enter('" + id_name + "_select" + i + "','" + html_element_name + "_select_focused')\" onblur=\"javascript:selectbox_leave('" + id_name + "_select" + i + "','" + html_element_name + "_select','" + html_element_name + "_select_init')\" >");
            
                bool found_option = false;
                for ( int j = 0 ; j < possible_select_items_text.Count ; j++ )
                {
                    if ((i < possible_select_items_text.Count) && (possible_select_items_text[j] == select_values[i - 1]))
                    {
                        if (possible_select_items_value.Count > j)
                        {
                            Output.WriteLine("          <option selected=\"selected=\" value=\"" + possible_select_items_value[j] + "\">" + possible_select_items_text[j] + "</option>");
                        }
                        else
                        {
                            Output.WriteLine("          <option selected=\"selected=\" value=\"" + possible_select_items_text[j] + "\">" + possible_select_items_text[j] + "</option>");
                        }
                        found_option = true;
                    }
                    else
                    {
                        if (possible_select_items_value.Count > j)
                        {
                            Output.WriteLine("          <option value=\"" + possible_select_items_value[j] + "\">" + possible_select_items_text[j] + "</option>");
                        }
                        else
                        {
                            Output.WriteLine("          <option value=\"" + possible_select_items_text[j] + "\">" + possible_select_items_text[j] + "</option>");
                        }
                    }
                }

                if (( i <= select_values.Count ) && ( select_values[i-1].Length > 0  ) && ( !Restrict_Values ) && ( !found_option ))
                {
                    Output.WriteLine("          <option value=\"" + select_values[i-1] + "\" selected=\"selected=\">" + select_values[i-1] + "</option>");
                }
                Output.Write("        </select>");

                if (i == text_values.Count )
                {
                    Output.WriteLine();
                    Output.WriteLine("      </div>");
                }
                else
                {
                    Output.WriteLine("<br />");
                }
            }

            Output.WriteLine("    </td>");
            Output.WriteLine("    <td valign=\"bottom\" >");

            if (Repeatable)
            {
                Output.WriteLine("            <a title=\"" + Translator.Get_Translation("Click to add another " + Title.ToLower(), CurrentLanguage) + ".\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onmousedown=\"return add_text_box_select_element('" + html_element_name + "', '" + second_label + "');\"><img border=\"0px\" class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" /></a>");
            }
            Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img border=\"0px\" class=\"help_button\" src=\"" + Base_URL + HELP_BUTTON_URL + "\" /></a>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr></table></td></tr>");

            Output.WriteLine();
        }

        #region Methods Implementing the Abstract Methods from abstract_Element class

        /// <summary> Reads the inner data from the Template XML format </summary>
        /// <param name="XMLReader"> Current template xml configuration reader </param>
        /// <remarks> This reads the possible values for the combo box from a <i>options</i> subelement </remarks>
        protected override void Inner_Read_Data(XmlTextReader XMLReader)
        {
            while (XMLReader.Read())
            {
                if ((XMLReader.NodeType == XmlNodeType.Element) && (XMLReader.Name.ToLower() == "options"))
                {
                    XMLReader.Read();
                    string options = XMLReader.Value.Trim();
                    possible_select_items_text.Clear();
                    possible_select_items_value.Clear();
                    if (options.Length > 0)
                    {
                        string[] options_parsed = options.Split(",".ToCharArray());
                        foreach (string thisOption in options_parsed)
                        {
                            if (!possible_select_items_text.Contains(thisOption.Trim()))
                            {
                                possible_select_items_text.Add(thisOption.Trim());

                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
