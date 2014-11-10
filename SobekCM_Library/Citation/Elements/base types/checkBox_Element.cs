#region Using directives

using System.IO;
using System.Xml;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Abstract base class for all elements which are made up of a single check box</summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    public abstract class checkBox_Element : abstract_Element
    {
        private readonly string checkBoxText;

        /// <summary> Protected field holds the default value for this element </summary>
        protected bool default_value;

        /// <summary> Constructor for a new instance of the simpleTextBox_Element class </summary>
        /// <param name="Title"> Title for this element </param>
        /// <param name="Html_Element_Name"> Name for the html components and styles for this element </param>
        /// <param name="Check_Box_Text"> Text to include after the checkbox (related to the element title ) </param>
        protected checkBox_Element(string Title, string Html_Element_Name, string Check_Box_Text)
        {
            base.Title = Title;
            html_element_name = Html_Element_Name;
            checkBoxText = Check_Box_Text;
            Repeatable = false;
            default_value = false;            
        }

        /// <summary> Method helps to render all simple text box based elements </summary>
        /// <param name="Output"> Output for the generated html for this element </param>
        /// <param name="instance_value"> Value for the current digital resource to display</param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        protected void render_helper(TextWriter Output, bool instance_value, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL)
        {
            Output.WriteLine("  <!-- " + Title + " Element -->");
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td style=\"width:" + LEFT_MARGIN + "px\">&nbsp;</td>");

            // Get the label to show
            string label_to_show = Title.Replace(":", "");

            if (Acronym.Length > 0)
            {
                Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\"><acronym title=\"" + Acronym + "\">" + Translator.Get_Translation(label_to_show, CurrentLanguage) + ":</acronym></a></td>");
            }
            else
            {
                Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\">" + Translator.Get_Translation(label_to_show, CurrentLanguage) + ":</a></td>");
            }

            Output.WriteLine("    <td>");
            Output.WriteLine("      <table>");
            Output.WriteLine("        <tr>");
            Output.WriteLine("          <td>");
            if (!instance_value)
            {
                Output.WriteLine("            <input type=\"checkbox\" name=\"" + html_element_name + "\" id=\"" + html_element_name + "\"><label for=\"" + html_element_name + "\">" + checkBoxText + "</label>");
            }
            else
            {
                Output.WriteLine("            <input type=\"checkbox\" name=\"" + html_element_name + "\" id=\"" + html_element_name + "\" checked=\"checked\"><label for=\"" + html_element_name + "\">" + checkBoxText + "</label>");
            }
            Output.WriteLine("          </td>");
            Output.WriteLine("          <td style=\"vertical-align:bottom\" >");


            Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img class=\"help_button\" src=\"" + Base_URL + HELP_BUTTON_URL + "\" /></a>");

            Output.WriteLine("          </td>");
            Output.WriteLine("        </tr>");
            Output.WriteLine("      </table>");

            Output.WriteLine("    </td>");

            Output.WriteLine("  </tr>");
            Output.WriteLine();
        }

        /// <summary> Reads the inner data from the CompleteTemplate XML format </summary>
        /// <param name="XMLReader"> Current template xml configuration reader </param>
        /// <remarks> This does nothing in the checkbox elements </remarks>
        protected override void Inner_Read_Data(XmlTextReader XMLReader)
        {
            // Does nothing
        }
    }
}
