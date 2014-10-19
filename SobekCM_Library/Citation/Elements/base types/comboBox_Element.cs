#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
	/// <summary> Abstract base class for all elements which are made up of a single combo/select box </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
	public abstract class comboBox_Element : abstract_Element
	{
	    /// <summary> Protected field holds the default value(s) </summary>
        protected List<string> default_values;

        /// <summary> Protected field holds all the possible, selectable values </summary>
        protected List<string> items;

        /// <summary> Protected field holds the onchange event text to add to the html </summary>
        protected string onChange;

	    /// <summary> Protected field holds the flag that tells if a value from the package which is not in the
	    /// provided options should be discarded or permitted </summary>
	    protected bool restrict_values;

	    /// <summary> Constructor for a new instance of the comboBox_Element class </summary>
        /// <param name="Title"> Title for this element </param>
        /// <param name="Html_Element_Name"> Name for the html components and styles for this element </param>
	    protected comboBox_Element(string Title, string Html_Element_Name)
		{
            // Set default title to blank           
            base.Title = Title;
            html_element_name = Html_Element_Name;

            items = new List<string>();
            default_values = new List<string>();

            onChange = String.Empty;
		}

        /// <summary> Adds a default value for this combo box based element </summary>
        /// <param name="defaultValue"> New default value </param>
        public void Add_Default_Value(string defaultValue)
        {
            default_values.Add(defaultValue);
        }

        /// <summary> Sets all of the possible, selectable values </summary>
        /// <param name="values"> Array of possible, selectable values </param>
        public void Set_Values(string[] values)
        {
            items.Clear();
            items.AddRange(values);
        }

		/// <summary> Add a new possible, selectable value to this combo box </summary>
		/// <param name="newItem"> New possible, selectable value </param>
		public void Add_Item( string newItem )
		{
			if ( !items.Contains( newItem ))
			{
				items.Add( newItem );
			}
		}

        /// <summary> Method helps to render all single combo box based elements </summary>
        /// <param name="Output"> Output for the generated html for this element </param>
        /// <param name="instance_value"> Value for the current digital resource to display </param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        protected void render_helper(TextWriter Output, string instance_value, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            render_helper(Output, instance_value, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL, false);
        }
        
        /// <summary> Method helps to render all single combo box based elements </summary>
        /// <param name="Output"> Output for the generated html for this element </param>
        /// <param name="instance_value"> Value for the current digital resource to display </param>
        /// <param name="Skin_Code"> Code for the current html skin </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        /// <param name="initial_value"> Flag indicates if the value in the instance_value param is actually instructional text, and not a true value</param>
        protected void render_helper(TextWriter Output, string instance_value, string Skin_Code, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL, bool initial_value)
        {
            string id_name = html_element_name.Replace("_", "");

            if ((instance_value.Length == 0) && (default_values.Count > 0))
            {
                instance_value = default_values[0];
            }

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
            if (onChange.Length > 0)
            {
                if (initial_value)
                {
                    Output.WriteLine("              <select class=\"" + html_element_name + "_select_init\" name=\"" + id_name + "1\" id=\"" + id_name + "1\" onblur=\"javascript:selectbox_leave('" + id_name + "1', '" + html_element_name + "_select', '" + html_element_name + "_select_init')\" onChange=\"" + onChange + "\" >");
                }
                else
                {
                    Output.WriteLine("              <select class=\"" + html_element_name + "_select\" name=\"" + id_name + "1\" id=\"" + id_name + "1\" onblur=\"javascript:selectbox_leave('" + id_name + "1', '" + html_element_name + "_select', '" + html_element_name + "_select_init')\" onChange=\"" + onChange + "\" >");
                }
            }
            else
            {
                if (initial_value)
                {
                    Output.WriteLine("              <select class=\"" + html_element_name + "_select_init\" name=\"" + id_name + "1\" id=\"" + id_name + "1\" onblur=\"javascript:selectbox_leave('" + id_name + "1', '" + html_element_name + "_select', '" + html_element_name + "_select_init')\" >");
                }
                else
                {
                    Output.WriteLine("              <select class=\"" + html_element_name + "_select\" name=\"" + id_name + "1\" id=\"" + id_name + "1\" onblur=\"javascript:selectbox_leave('" + id_name + "1', '" + html_element_name + "_select', '" + html_element_name + "_select_init')\" >");
                }
            }

            bool found_option = false;
            foreach (string thisOption in items)
            {
                if (thisOption == instance_value)
                {
                    Output.WriteLine("                <option selected=\"selected=\" value=\"" + thisOption + "\">" + thisOption + "</option>");
                    found_option = true;
                }
                else
                {
                    Output.WriteLine("                <option value=\"" + thisOption + "\">" + thisOption + "</option>");
                }
            }
            if ((instance_value.Length > 0) && (!restrict_values) && (!found_option))
            {
                Output.WriteLine("                <option selected=\"selected=\" value=\"" + instance_value + "\">" + instance_value + "</option>");
            }
            Output.WriteLine("              </select>");
            Output.WriteLine("            </div>");
            Output.WriteLine("          </td>");
            Output.WriteLine("          <td style=\"vertical-align:bottom\" >");
            if (Repeatable)
            {
                Output.WriteLine("            <a title=\"" + Translator.Get_Translation("Click to add another " + Title.ToLower(), CurrentLanguage) + ".\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onmousedown=\"return " + html_element_name + "_add_new_item();\"><img class=\"repeat_button\" src=\"" + Base_URL + REPEAT_BUTTON_URL + "\" /></a>");
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
	    /// <remarks> This reads the possible values for the combo box from a <i>options</i> subelement and the default value from a <i>value</i> subelement </remarks>
	    protected override void Inner_Read_Data( XmlTextReader XMLReader )
	    {
	        default_values.Clear();
	        while ( XMLReader.Read() )
	        {
	            if (( XMLReader.NodeType == XmlNodeType.Element ) && (( XMLReader.Name.ToLower() == "value" ) || ( XMLReader.Name.ToLower() == "options" ))) 
	            {
	                if ( XMLReader.Name.ToLower() == "value" )
	                {
	                    XMLReader.Read();
	                    default_values.Add(XMLReader.Value.Trim());
	                }
	                else
	                {
	                    XMLReader.Read();
	                    string options = XMLReader.Value.Trim();
	                    items.Clear();
	                    if ( options.Length > 0 )
	                    {
	                        string[] options_parsed = options.Split(",".ToCharArray());
	                        foreach (string thisOption in options_parsed.Where(thisOption => !items.Contains(thisOption.Trim())))
	                        {
	                            items.Add( thisOption.Trim() );
	                        }
	                    }
	                }
	            }
	        }
	    }

	    #endregion
	}
}
