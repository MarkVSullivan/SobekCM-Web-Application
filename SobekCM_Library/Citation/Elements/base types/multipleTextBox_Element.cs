#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Web;
using System.Xml;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Abstract base class for all elements which are made up of multiple small text boxes </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    public abstract class multipleTextBox_Element : abstract_Element
    {
        /// <summary> Protected field holds how many boxes are allowed per line, or -1 if there is no limit </summary>
        protected int boxes_per_line = -1;

        /// <summary> Protected field holds all the default values to display </summary>
        protected List<string> default_values;

        /// <summary> Protected field holds how many boxes are allowed total for this element, or -1 if there is no limit </summary>
        protected int max_boxes = -1;

        /// <summary> Protected field holds any html to insert as the view choices option after the boxes </summary>
        protected string view_choices_string;

        /// <summary> Constructor for a new instance of the multipleTextBox_Element class </summary>
        /// <param name="Title"> Title for this element </param>
        /// <param name="Html_Element_Name"> Name for the html components and styles for this element </param>
        protected multipleTextBox_Element(string Title, string Html_Element_Name)
        {
            base.Title = Title;
            html_element_name = Html_Element_Name;
            view_choices_string = String.Empty;

            default_values = new List<string>();
        }

        /// <summary> Adds a new default value for this multiple text box type element </summary>
        /// <param name="defaultValue"> New default value</param>
        public void Add_Default_Value(string defaultValue)
        {
            default_values.Add(defaultValue);
        }

        /// <summary> Method helps to render all multiple text box based elements </summary>
        /// <param name="Output"> Output for the generated html for this element </param>
        /// <param name="instance_values"> Value(s) for the current digital resource to display</param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        protected void render_helper(TextWriter Output, ReadOnlyCollection<string> instance_values, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            List<string> allValues = new List<string>();
            allValues.AddRange(instance_values);

            if (allValues.Count == 0)
            {
                render_helper(Output, String.Empty, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
                return;
            }

            if (allValues.Count == 1)
            {
                render_helper(Output, allValues[0], Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
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
                    if (i == allValues.Count)
                    {
                        Output.WriteLine("              <input name=\"" + id_name + i + "\" id=\"" + id_name + i + "\" class=\"" + html_element_name + "_input sbk_Focusable\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(allValues[i - 1].Replace("<i>", "").Replace("</i>", "")) + "\" /></div>");
                    }
                    else
                    {
						Output.WriteLine("              <input name=\"" + id_name + i + "\" id=\"" + id_name + i + "\" class=\"" + html_element_name + "_input sbk_Focusable\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(allValues[i - 1].Replace("<i>", "").Replace("</i>", "")) + "\" />");
                    }
                }

                Output.WriteLine("          </td>");
                Output.WriteLine("          <td style=\"vertical-align:bottom\" >");

                if (view_choices_string.Length > 0)
                {
                    Output.WriteLine("            " + view_choices_string.Replace("<%WEBSKIN%>", Skin_Code).Replace("<%?URLOPTS%>", "") + "&nbsp; ");
                }

                if ((Repeatable) && (( max_boxes < 0 ) || ( allValues.Count < max_boxes )))
                {
                    Output.WriteLine("          <span id=\"" + html_element_name + "_repeaticon\" name=\"" + html_element_name + "_repeaticon\"><img title=\"" + Translator.Get_Translation("Click to add another " + Title.ToLower(), CurrentLanguage) + ".\" alt=\"+\" class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" onmousedown=\"add_new_multi_element('" + html_element_name + "', " + instance_values.Count + "," + max_boxes + "," + boxes_per_line + "); return false;\" /></span>");
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

        /// <summary> Method helps to render all multiple text box based elements </summary>
        /// <param name="Output"> Output for the generated html for this element </param>
        /// <param name="instance_value"> Value for the current digital resource to display</param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        protected void render_helper(TextWriter Output, string instance_value, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            string id_name = html_element_name.Replace("_", "");

            Output.WriteLine("  <!-- " + Title + " Element -->");
            Output.WriteLine("  <tr align=\"left\">");
            Output.WriteLine("    <td width=\"" + LEFT_MARGIN + "px\">&nbsp;</td>");
            if (Read_Only)
            {
                Output.WriteLine("    <td valign=\"top\" class=\"metadata_label\">" + Translator.Get_Translation(Title, CurrentLanguage) + ":</b></td>");
            }
            else
            {
                if (Acronym.Length > 0)
                {
                    Output.WriteLine("    <td valign=\"top\" class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\"><acronym title=\"" + Acronym + "\">" + Translator.Get_Translation(Title, CurrentLanguage) + ":</acronym></a></td>");
                }
                else
                {
                    Output.WriteLine("    <td valign=\"top\" class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\">" + Translator.Get_Translation(Title, CurrentLanguage) + ":</a></td>");
                }
            }

            if (Read_Only)
            {
                Output.WriteLine("    <td>" + instance_value + "</td>");
            }
            else
            {
                Output.WriteLine("    <td>");
                Output.WriteLine("      <table>");
                Output.WriteLine("        <tr>");
                Output.WriteLine("          <td>");
                Output.WriteLine("            <div id=\"" + html_element_name + "_div\">");
                Output.WriteLine("              <input name=\"" + id_name + "1\" id=\"" + id_name + "1\" class=\"" + html_element_name + "_input sbk_Focusable\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(instance_value.Replace("<i>", "").Replace("</i>", "")) + "\" /></div>");
                Output.WriteLine("          </td>");
                Output.WriteLine("          <td valign=\"bottom\" >");

                if (view_choices_string.Length > 0)
                {
                    Output.WriteLine("            " + view_choices_string.Replace("<%WEBSKIN%>", Skin_Code).Replace("<%?URLOPTS%>", "") + "&nbsp; ");
                }

                if (Repeatable)
                {
                    Output.WriteLine("          <span id=\"" + html_element_name + "_repeaticon\" name=\"" + html_element_name + "_repeaticon\"><img title=\"" + Translator.Get_Translation("Click to add another " + Title.ToLower(), CurrentLanguage) + ".\" alt=\"+\" border=\"0px\" class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" onmousedown=\"add_new_multi_element('" + html_element_name + "', 1," + max_boxes + "," + boxes_per_line + "); return false;\" /></span>");
                }

                Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img border=\"0px\" class=\"help_button\" src=\"" + Base_URL + HELP_BUTTON_URL + "\" /></a>");

                Output.WriteLine("          </td>");
                Output.WriteLine("        </tr>");
                Output.WriteLine("      </table>");
                Output.WriteLine("    </td>");
            }

            Output.WriteLine("  </tr>");
            Output.WriteLine();
        }

        #region Methods Implementing the Abstract Methods from abstract_Element class


        /// <summary> Reads the inner data from the Template XML format </summary>
        /// <param name="XMLReader"> Current template xml configuration reader </param>
        /// <remarks> This reads the default value from a <i>value</i> subelement </remarks>
        protected override void Inner_Read_Data( XmlTextReader XMLReader )
        {
            while ( XMLReader.Read() )
            {
                if (( XMLReader.NodeType == XmlNodeType.Element ) && ( XMLReader.Name.ToLower() == "value" ))
                {
                    XMLReader.Read();
                    default_values.Add(XMLReader.Value.Trim());
                    return;
                }
            }
        }

        #endregion
    }
}
