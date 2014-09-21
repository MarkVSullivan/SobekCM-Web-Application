#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Abstract base class for all elements which are made up of multiple small combo/select boxes </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    public abstract class multipleComboBox_Element : abstract_Element
	{
        /// <summary> Protected field holds how many boxes are allowed per line, or -1 if there is no limit </summary>
        protected int boxes_per_line = -1;

        /// <summary> Protected field holds all the possible, selectable values </summary>
        protected List<string> items;

        /// <summary> Protected field holds how many boxes are allowed total for this element, or -1 if there is no limit </summary>
        protected int max_boxes = -1;

        /// <summary> Protected field holds any html to insert as the view choices option after the boxes </summary>
        protected string view_choices_string;

        /// <summary> Constructor for a new instance of the multipleComboBox_Element class </summary>
        /// <param name="Title"> Title for this element </param>
        /// <param name="Html_Element_Name"> Name for the html components and styles for this element </param>
        protected multipleComboBox_Element(string Title, string Html_Element_Name)
		{
			base.Title = Title;
            html_element_name = Html_Element_Name;
            view_choices_string = String.Empty;

            items = new List<string>();
		}

        /// <summary> Adds a possible, selectable value to the combo/select box </summary>
        /// <param name="newItem"> New possible, selectable value </param>
        public void Add_Item(string newItem)
        {
            if (!items.Contains(newItem))
            {
                items.Add(newItem);
            }
        }

        /// <summary> Adds a series of possible, selectable values to the combo/select box </summary>
        /// <param name="New_Items"> List of new possible, selectable value </param>
        public void Add_Items(string[] New_Items)
        {
            foreach (string thisItem in New_Items)
            {
                Add_Item(thisItem);
            }
        }

        /// <summary> Method helps to render all multiple combo box based elements </summary>
        /// <param name="Output"> Output for the generated html for this element </param>
        /// <param name="instance_values"> Value(s) for the current digital resource to display</param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        protected void render_helper(TextWriter Output, List<string> instance_values, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL)
        {
            render_helper(Output, new ReadOnlyCollection<string>(instance_values), items, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL );
        }

        /// <summary> Method helps to render all multiple combo box based elements </summary>
        /// <param name="Output"> Output for the generated html for this element </param>
        /// <param name="instance_values"> Value(s) for the current digital resource to display</param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        protected void render_helper(TextWriter Output, ReadOnlyCollection<string> instance_values, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL)
        {
            render_helper(Output, instance_values, items, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Method helps to render all multiple combo box based elements </summary>
        /// <param name="Output"> Output for the generated html for this element </param>
        /// <param name="instance_values"> Value(s) for the current digital resource to display</param>
        /// <param name="possible_values"> Possible vlaues for the combo boxes </param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        protected void render_helper(TextWriter Output, List<string> instance_values, List<string> possible_values, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL)
        {
            render_helper(Output, new ReadOnlyCollection<string>(instance_values), possible_values, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Method helps to render all multiple combo box based elements </summary>
        /// <param name="Output"> Output for the generated html for this element </param>
        /// <param name="instance_value"> Value for the current digital resource to display</param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        protected void render_helper(TextWriter Output, string instance_value, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL)
        {
            render_helper(Output, instance_value, items, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Method helps to render all multiple combo box based elements </summary>
        /// <param name="Output"> Output for the generated html for this element </param>
        /// <param name="instance_values"> Value(s) for the current digital resource to display</param>
        /// <param name="possible_values"> Possible vlaues for the combo boxes </param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        protected void render_helper(TextWriter Output, ReadOnlyCollection<string> instance_values, List<string> possible_values, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            List<string> allValues = new List<string>();
            allValues.AddRange(instance_values);

            if (allValues.Count == 0)
            {
                render_helper(Output, String.Empty, possible_values, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
                return;
            }

            if (allValues.Count == 1)
            {
                render_helper(Output, allValues[0], possible_values, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
                return;
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


            if (Read_Only)
            {
                Output.Write("    <td>");
                for (int i = 0; i < instance_values.Count; i++)
                {
                    Output.Write(instance_values[i]);
                    if (i < (instance_values.Count - 1))
                        Output.Write("<br />");
                }
                Output.WriteLine("</td>");
            }
            else
            {
                Output.WriteLine("    <td>");
                Output.WriteLine("      <table>");
                Output.WriteLine("        <tr>");
                Output.WriteLine("          <td>");
                Output.WriteLine("            <div id=\"" + html_element_name + "_div\">");
                for (int i = 1; i <= allValues.Count; i++)
                {
                    string value = allValues[i - 1];
                    Output.WriteLine("              <select name=\"" + id_name + i + "\" id=\"" + id_name + i + "\" class=\"" + html_element_name + "_input\" type=\"text\" >");
                    bool found = false;
                    if (value.Length == 0)
                    {
                        found = true;
                        Output.WriteLine("                <option value=\"\" selected=\"selected\" >&nbsp;</option>");
                    }
                    else
                    {
                        Output.WriteLine("                <option value=\"\">&nbsp;</option>");
                    }
                    foreach (string item in possible_values)
                    {
                        if (item.ToUpper() == value.ToUpper())
                        {
                            found = true;
                            Output.WriteLine("                <option value=\"" + item + "\" selected=\"selected\" >" + item + "</option>");
                        }
                        else
                        {
                            Output.WriteLine("                <option value=\"" + item + "\">" + item + "</option>");
                        }
                    }
                    if (!found)
                    {
                        Output.WriteLine("                <option value=\"" + value + "\" selected=\"selected\" >" + value + "</option>");
                    }
                    Output.WriteLine("              </select>");

                    if (i == allValues.Count)
                    {
                        Output.WriteLine("</div>");
                    }
                }

                Output.WriteLine("          </td>");
                Output.WriteLine("          <td style=\"vertical-align:bottom\" >");

                if (view_choices_string.Length > 0)
                {
                    Output.WriteLine("            " + view_choices_string.Replace("<%WEBSKIN%>", Skin_Code) + "&nbsp; ");
                }

                if ((Repeatable) && ((max_boxes < 0) || (allValues.Count < max_boxes)))
                {
                    Output.WriteLine("          <span id=\"" + html_element_name + "_repeaticon\" name=\"" + html_element_name + "_repeaticon\"><img title=\"" + Translator.Get_Translation("Click to add another " + Title.ToLower(), CurrentLanguage) + ".\" alt=\"+\" class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" onmousedown=\"add_new_multi_combo_element('" + html_element_name + "', " + allValues.Count + "," + max_boxes + "," + boxes_per_line + "); return false;\" /></span>");
                }

                Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img class=\"help_button\" src=\"" + Base_URL + HELP_BUTTON_URL + "\" /></a>");

                Output.WriteLine("          </td>");
                Output.WriteLine("        </tr>");
                Output.WriteLine("      </table>");
                Output.WriteLine("    </td>");
            }

            Output.WriteLine("  </tr>");
            Output.WriteLine();
        }


        /// <summary> Method helps to render all multiple combo box based elements </summary>
        /// <param name="Output"> Output for the generated html for this element </param>
        /// <param name="instance_value"> Value for the current digital resource to display</param>
        /// <param name="possible_values"> Possible vlaues for this combo boxes </param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        protected void render_helper(TextWriter Output, string instance_value, List<string> possible_values, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
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


            if (Read_Only)
            {
                Output.Write("    <td>");
                Output.Write(instance_value);
                Output.WriteLine("</td>");
            }
            else
            {
                Output.WriteLine("    <td>");
                Output.WriteLine("      <table>");
                Output.WriteLine("        <tr>");
                Output.WriteLine("          <td>");
                Output.WriteLine("            <div id=\"" + html_element_name + "_div\">");
                
                const int i = 1;

                string value = instance_value;
                    Output.WriteLine("              <select name=\"" + id_name + i + "\" id=\"" + id_name + i + "\" class=\"" + html_element_name + "_input\" type=\"text\" >");
                    bool found = false;
                    if (value.Length == 0)
                    {
                        found = true;
                        Output.WriteLine("                <option value=\"\" selected=\"selected\" >&nbsp;</option>");
                    }
                    else
                    {
                        Output.WriteLine("                <option value=\"\">&nbsp;</option>");
                    }
                    foreach (string item in possible_values)
                    {
                        if (item.ToUpper() == value.ToUpper())
                        {
                            found = true;
                            Output.WriteLine("                <option value=\"" + item + "\" selected=\"selected\" >" + item + "</option>");
                        }
                        else
                        {
                            Output.WriteLine("                <option value=\"" + item + "\">" + item + "</option>");
                        }
                    }
                    if (!found)
                    {
                        Output.WriteLine("                <option value=\"" + value + "\" selected=\"selected\" >" + value + "</option>");
                    }
                    Output.WriteLine("              </select>");

                    Output.WriteLine("</div>");
                }

                Output.WriteLine("          </td>");
                Output.WriteLine("          <td style=\"vertical-align:bottom\" >");

                if (view_choices_string.Length > 0)
                {
                    Output.WriteLine("            " + view_choices_string.Replace("<%WEBSKIN%>", Skin_Code).Replace("<%?URLOPTS%>","") + "&nbsp; ");
                }

                if (Repeatable)
                {
                    Output.WriteLine("          <span id=\"" + html_element_name + "_repeaticon\" name=\"" + html_element_name + "_repeaticon\"><img title=\"" + Translator.Get_Translation("Click to add another " + Title.ToLower(), CurrentLanguage) + ".\" alt=\"+\" class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" onmousedown=\"add_new_multi_combo_element('" + html_element_name + "', 1," + max_boxes + "," + boxes_per_line + "); return false;\" /></span>");
                }

                Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img class=\"help_button\" src=\"" + Base_URL + HELP_BUTTON_URL + "\" /></a>");


                Output.WriteLine("          </td>");
                Output.WriteLine("        </tr>");
                Output.WriteLine("      </table>");
                Output.WriteLine("    </td>");
            

            Output.WriteLine("  </tr>");
            Output.WriteLine();
        }

        #region Methods Implementing the Abstract Methods from abstract_Element class

        /// <summary> Reads the inner data from the Template XML format </summary>
        /// <param name="XMLReader"> Current template xml configuration reader </param>
        /// <remarks> This reads the default value from a <i>value</i> subelement </remarks>
        protected override void Inner_Read_Data(XmlTextReader XMLReader)
        {
            while ( XMLReader.Read() )
            {
                if ( XMLReader.NodeType == XmlNodeType.Element ) 
                {
                    if (XMLReader.Name.ToLower() == "value")
                    {
                        XMLReader.Read();
                        items.Add(XMLReader.Value.Trim());
                    }
                    if (XMLReader.Name.ToLower() == "options")
                    {
                        XMLReader.Read();
                        string options = XMLReader.Value.Trim();
                        items.Clear();
                        if (options.Length > 0)
                        {
                            string[] options_parsed = options.Split(",".ToCharArray());
                            foreach (string thisOption in options_parsed.Where(thisOption => !items.Contains(thisOption.Trim())))
                            {
                                items.Add(thisOption.Trim());
                            }
                        }
                    }
                }
            }
        }

        #endregion
	}
}



