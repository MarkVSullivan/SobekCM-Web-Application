#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Abstract base class for all elements which are made up of two text boxes  </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    public abstract class textBox_TextBox_Element : abstract_Element
	{
        /// <summary> Protected field holds any label to place before the first text box </summary>
        protected string first_label;

        /// <summary> Protected field holds any label to place before the second text box </summary>
        protected string second_label;

        /// <summary> Constructor for a new instance of the textBox_TextBox_Element class </summary>
        /// <param name="Title"> Title for this element </param>
        /// <param name="Html_Element_Name"> Name for the html components and styles for this element </param>
        protected textBox_TextBox_Element(string Title, string Html_Element_Name)
		{
			base.Title = Title;
            html_element_name = Html_Element_Name;
            first_label = String.Empty;
            second_label = String.Empty;
		}

        /// <summary> Method helps to render the html for all elements based on textBox_TextBox_Element class </summary>
        /// <param name="Output"> Output for the generate html for this element </param>
        /// <param name="instance_values_text1"> Value(s) for the current digital resource to display in the first text box</param>
        /// <param name="instance_values_text2" >Value(s) for the current digital resource to display in the second text box</param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        protected void render_helper(TextWriter Output, List<string> instance_values_text1, List<string> instance_values_text2, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            if ((instance_values_text1.Count == 0) && ( instance_values_text2.Count == 0 ))
            {
                render_helper(Output, String.Empty, String.Empty, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
                return;
            }

            if ((instance_values_text1.Count == 1) && (instance_values_text2.Count == 1))
            {
                render_helper(Output, instance_values_text1[0], instance_values_text2[0], Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
                return;
            }


            string id_name = html_element_name.Replace("_", "");

            Output.WriteLine("  <!-- " + Title.Replace(":","") + " Element -->");
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td style=\"width:" + LEFT_MARGIN + "px\">&nbsp;</td>");
            if (Title.IndexOf(":") < 0)
            {
                if (Acronym.Length > 0)
                {
                    Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\"><acronym title=\"" + Acronym + "\">" + Translator.Get_Translation(Title, CurrentLanguage) + ":</acronym></a></td>");
                }
                else
                {
                    Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\">" + Translator.Get_Translation(Title, CurrentLanguage) + ":</a></td>");
                }
            }
            else
            {
                if (Acronym.Length > 0)
                {
                    Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\"><acronym title=\"" + Acronym + "\">" + Translator.Get_Translation(Title, CurrentLanguage) + "</acronym></a></td>");
                }
                else
                {
                    Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\">" + Translator.Get_Translation(Title, CurrentLanguage) + "</a></td>");
                }
            }
            Output.WriteLine("    <td>");

                Output.WriteLine("      <table>");
                Output.WriteLine("        <tr>");
                Output.WriteLine("          <td>");
                Output.WriteLine("            <div id=\"" + html_element_name + "_div\">");

                for (int i = 1; i <= instance_values_text1.Count; i++)
                {
                    // Write the first text
                    if (first_label.Length > 0)
                    {
                        Output.Write("              <span class=\"metadata_sublabel2\">" + Translator.Get_Translation(first_label, CurrentLanguage) + ":</span>");
                    }
                    else
                    {
                        Output.Write("              ");
                    }

                    // Write the first text box
					Output.Write("<input name=\"" + id_name + "_first" + i + "\" id=\"" + id_name + "_first" + i + "\" class=\"" + html_element_name + "_first_input sbk_Focusable\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(instance_values_text1[i - 1]) + "\" />");

                    // Write the second text
                    if (second_label.Length > 0)
                    {
                        Output.Write("<span class=\"metadata_sublabel\">" + Translator.Get_Translation(second_label, CurrentLanguage) + ":</span>");
                    }

                    // Write the second text box
                    Output.Write("<input name=\"" + id_name + "_second" + i + "\" id=\"" + id_name + "_second" + i + "\" class=\"" + html_element_name + "_second_input sbk_Focusable\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(instance_values_text2[i - 1]) + "\"  />");
                    Output.WriteLine(i < instance_values_text1.Count ? "<br />" : "</div>");
                }

                Output.WriteLine("          </td>");
                Output.WriteLine("          <td style=\"vertical-align:bottom\" >");
                if (Repeatable)
                {
                    Output.WriteLine("            <a title=\"" + Translator.Get_Translation("Click to add another " + Title.ToLower(), CurrentLanguage) + ".\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onmousedown=\"return add_two_text_box_element('" + html_element_name + "','" + first_label + "','" + second_label + "');\"><img class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" /></a>");
                }
                Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img class=\"help_button\" src=\"" + Base_URL + HELP_BUTTON_URL + "\" /></a>");
                Output.WriteLine("          </td>"); Output.WriteLine("        </tr>");
                Output.WriteLine("      </table>");
         
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine();
        }


        /// <summary> Method helps to render the html for all elements based on textBox_TextBox_Element class </summary>
        /// <param name="Output"> Output for the generate html for this element </param>
        /// <param name="instance_value_text1"> Value for the current digital resource to display in the first text box</param>
        /// <param name="instance_value_text2" >Value for the current digital resource to display in the second text box</param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        protected void render_helper(TextWriter Output, string instance_value_text1, string instance_value_text2, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL)
        {
            string id_name = html_element_name.Replace("_", "");

            Output.WriteLine("  <!-- " + Title + " Element -->");
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td style=\"width:" + LEFT_MARGIN + "px\">&nbsp;</td>");
            if (Title.IndexOf(":") < 0)
            {
                if (Read_Only)
                {
                    Output.WriteLine("    <td class=\"metadata_label\">" + Title + ":</b></td>");
                }
                else
                {
                    if (Acronym.Length > 0)
                    {
                        Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\"><acronym title=\"" + Acronym + "\">" + Title + ":</acronym></a></td>");
                    }
                    else
                    {
                        Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\">" + Title + ":</a></td>");
                    }
                }
            }
            else
            {
                if (Read_Only)
                {
                    Output.WriteLine("    <td class=\"metadata_label\">" + Title + "</b></td>");
                }
                else
                {
                    if (Acronym.Length > 0)
                    {
                        Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\"><acronym title=\"" + Acronym + "\">" + Title + "</acronym></a></td>");
                    }
                    else
                    {
                        Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\">" + Title + "</a></td>");
                    }
                }
            }
            Output.WriteLine("    <td>");

            Output.WriteLine("      <table>");
            Output.WriteLine("        <tr>");
            Output.WriteLine("          <td>");
            Output.WriteLine("            <div id=\"" + html_element_name + "_div\">");

            const int i = 1;

            // Write the first text
            if (first_label.Length > 0)
            {
                Output.Write("              <span class=\"metadata_sublabel2\">" + first_label + ":</span>");
            }

            // Write the first text box
            Output.Write("<input name=\"" + id_name + "_first" + i + "\" id=\"" + id_name + "_first" + i + "\" class=\"" + html_element_name + "_first_input sbk_Focusable\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(instance_value_text1) + "\" />");

            // Write the second text
            if (second_label.Length > 0)
            {
                Output.Write("<span class=\"metadata_sublabel\">" + second_label + ":</span>");
            }

            // Write the second text box
			Output.Write("<input name=\"" + id_name + "_second" + i + "\" id=\"" + id_name + "_second" + i + "\" class=\"" + html_element_name + "_second_input sbk_Focusable\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(instance_value_text2) + "\" />");
            Output.WriteLine("</div>");
            Output.WriteLine("          </td>");
            Output.WriteLine("          <td style=\"vertical-align:bottom\" >");
            if (Repeatable)
            {
                Output.WriteLine("            <a title=\"" + Translator.Get_Translation("Click to add another " + Title.ToLower(), CurrentLanguage) + ".\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onmousedown=\"return add_two_text_box_element('" + html_element_name + "','" + first_label + "','" + second_label + "');\"><img class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" /></a>");
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
        /// <remarks> This procedure does not currently read any inner xml (not yet necessary) </remarks>
        protected override void Inner_Read_Data( XmlTextReader XMLReader )
        {
            // Do nothing
        }

        #endregion
	}
}
