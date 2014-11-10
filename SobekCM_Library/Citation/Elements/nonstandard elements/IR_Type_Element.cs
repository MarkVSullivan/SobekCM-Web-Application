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
using SobekCM.Core.Users;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows IR-specific entry of the material type and related larger body of work for an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    public class IR_Type_Element : abstract_Element
    {
        /// <summary> Protected field holds the default value(s) </summary>
        private List<string> default_values;

        /// <summary> Protected field holds all the possible, selectable values </summary>
		private List<string> items;

        /// <summary> Protected field holds the flag that tells if a value from the package which is not in the
        /// provided options should be discarded or permitted </summary>
		private bool restrict_values;

        /// <summary> Constructor for a new instance of the IR_Type_Element class </summary>
        public IR_Type_Element()
        {
            Repeatable = false;
            Type = Element_Type.Type;
            Display_SubType = "ir";

            // Set default title to blank
            restrict_values = false;
            items = new List<string>();
            Title = Title;
            html_element_name = "type_ir";
	        help_page = "typeir";
            default_values = new List<string>();
           
        }

        #region iElement Members

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
        /// <remarks> This simple element does not append any popup form to the popup_form_builder</remarks>
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool IsMozilla, StringBuilder PopupFormBuilder, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string DEFAULT_ACRONYM = "Select the type which best categorizes this material and provide information about the larger body of work on the next line.";
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

            // Determine the material type
            string material_type;
            string sobekcm_type = Bib.Bib_Info.SobekCM_Type_String;
            if (sobekcm_type.Length == 0)
                material_type = "Select Material Type";
            else
            {
                material_type = sobekcm_type == "Archival" ? Bib.Bib_Info.Original_Description.Extent : sobekcm_type;
            }

            // Determine the text for the larger body of work
            string larger_text;
            string larger_value = String.Empty;
            bool other_value = false;
            switch (material_type)
            {
                case "Book Chapter":
                    larger_text = "Book Title:";
                    larger_value = Bib.Bib_Info.SeriesTitle.ToString();
                    break;
                
                case "Conference Papers":
                case "Conference Proceedings":
                    larger_text = "Conference Name:";
                    if (Bib.Bib_Info.Names_Count > 0)
                    {
                        foreach (Name_Info thisName in Bib.Bib_Info.Names)
                        {
                            if ((thisName.Name_Type == Name_Info_Type_Enum.conference) && (thisName.Full_Name.Length > 0))
                            {
                                larger_value = thisName.ToString();
                                break;
                            }
                        }
                    }
                    break;

                case "Course Material":
                    larger_text = "Course Name:";
                    larger_value = Bib.Bib_Info.SeriesTitle.ToString();
                    break;
                    
                case "Journal Article":
                    larger_text = "Journal Citation:";
                    larger_value = Bib.Bib_Info.SeriesTitle.ToString();
                    break;
                
                case "Technical Reports":
                    larger_text = "Series Title:";
                    larger_value = Bib.Bib_Info.SeriesTitle.ToString();
                    break;

                case "Select Material Type":
                    larger_text = "Larger Body of Work:";
                    larger_value = Bib.Bib_Info.SeriesTitle.ToString();
                    break;

                default:
                    material_type = "Other";
                    larger_text = "Larger Body of Work:";
                    larger_value = Bib.Bib_Info.SeriesTitle.ToString();
                    other_value = true;
                    break;
            }

            string id_name ="irtype";
            string html_element_name_irtype = "ir_type";
            string title = "Material Type";

            if ((material_type.Length == 0) && (default_values.Count > 0))
            {
                material_type = default_values[0];
            }

            Output.WriteLine("  <!-- Institutional Repository Material Type Element -->");
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td style=\"width:" + LEFT_MARGIN + "px\">&nbsp;</td>");
            if (Acronym.Length > 0)
            {
                Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name_irtype.ToUpper() + "\"><acronym title=\"" + Acronym + "\">" + Translator.Get_Translation(title, CurrentLanguage) + ":</acronym></a></td>");
            }
            else
            {
                Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name_irtype.ToUpper() + "\">" + Translator.Get_Translation(title, CurrentLanguage) + ":</a></td>");
            }
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table>");
            Output.WriteLine("        <tr>");
            Output.WriteLine("          <td>");
            Output.WriteLine("            <div id=\"" + html_element_name_irtype + "_div\">");
            if (material_type.IndexOf("Select") == 0)
            {
                Output.WriteLine("              <select class=\"" + html_element_name_irtype + "_input_init\" name=\"" + id_name + "\" id=\"" + id_name + "\" onchange=\"javascript:ir_type_change()\" >");
            }
            else
            {
                Output.WriteLine("              <select class=\"" + html_element_name_irtype + "_input\" name=\"" + id_name + "\" id=\"" + id_name + "\" onchange=\"javascript:ir_type_change()\" >");
            }

            bool found_option = false;
            foreach (string thisOption in items)
            {
                if (thisOption == material_type)
                {
                    Output.WriteLine("                <option selected=\"selected=\" value=\"" + thisOption + "\">" + thisOption + "</option>");
                    found_option = true;
                }
                else
                {
                    Output.WriteLine("                <option value=\"" + thisOption + "\">" + thisOption + "</option>");
                }
            }
            if ((material_type.Length > 0) && (!restrict_values) && (!found_option))
            {
                Output.WriteLine("                <option selected=\"selected=\" value=\"" + material_type + "\">" + material_type + "</option>");
            }
            Output.WriteLine("              </select>");
            if (other_value)
            {
                Output.WriteLine("              <span class=\"metadata_sublabel\" id=\"irtype_othertext\" name=\"irtype_othertext\">" + Translator.Get_Translation("Specify Type", CurrentLanguage) + ":</span>");
                Output.WriteLine("              <input type=\"text\" class=\"irtype_other_input sbk_Focusable\" id=\"irtype_otherinput\" name=\"irtype_otherinput\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Original_Description.Extent) + "\" />");

            }
            Output.WriteLine("              </div>");
            Output.WriteLine("          </td>");
            Output.WriteLine("          <td valign=\"bottom\" >");
            Output.WriteLine("            <a target=\"_" + html_element_name_irtype.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img class=\"help_button\" src=\"" + Base_URL + HELP_BUTTON_URL + "\" /></a>");
            Output.WriteLine("          </td>");
            Output.WriteLine("        </tr>");
            Output.WriteLine("      </table>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine();

            id_name = "largerbody";
            html_element_name_irtype = "larger_body";
            title = larger_text;

            Output.WriteLine("  <!-- Institutional Repository Larger Body of Work (IR_Type_Element) -->");
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td style\"" + LEFT_MARGIN + "px\">&nbsp;</td>");
            if (Read_Only)
            {
                Output.WriteLine("    <td class=\"metadata_label\">" + Translator.Get_Translation(title, CurrentLanguage) + "</td>");
            }
            else
            {
                if (Acronym.Length > 0)
                {
                    Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name_irtype.ToUpper() + "\"><acronym title=\"" + Acronym + "\"><span id=\"larger_body_label\">" + title + "</span></acronym></a></td>");
                }
                else
                {
                    Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name_irtype.ToUpper() + "\"><span id=\"larger_body_label\">" + title + "</span></a></td>");
                }
            }


            if (Read_Only)
            {
                Output.Write("    <td>");
                Output.Write(larger_value);
                Output.WriteLine("</td>");
            }
            else
            {
                Output.WriteLine("    <td>");
                Output.WriteLine("      <table><tr><td>");
                Output.WriteLine("      <div id=\"" + html_element_name_irtype + "_div\">");
                Output.WriteLine("      <input name=\"" + id_name + "\" id=\"" + id_name + "\" class=\"" + html_element_name_irtype + "_input sbk_Focusable\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(larger_value.Replace("<i>", "").Replace("</i>", "")) + "\" /></div>");
                Output.WriteLine("    </td>");
                Output.WriteLine("         <td vstyle=\"vertical-align=:bottom\" >");
                Output.WriteLine("            <a target=\"_" + html_element_name_irtype.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img class=\"help_button\" src=\"" + Base_URL + HELP_BUTTON_URL + "\" /></a>");
                Output.WriteLine("          </td>");
                Output.WriteLine("  </tr></table>");
                Output.WriteLine("  </td>");
            }

            Output.WriteLine("  </tr>");
            Output.WriteLine();
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This does nothing since there is only one type </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Do nothing since there is only one title
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            string type = String.Empty;
            foreach (string thisKey in getKeys)
            {
                if (thisKey == "irtype")
                {
                    type = HttpContext.Current.Request.Form[thisKey];
                    if ((type != "Select Material Type") && ( type != "Other" ))
                    {
	                    Bib.Bib_Info.SobekCM_Type_String = type;

	                    if (Bib.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.UNKNOWN)
	                    {
		                    Bib.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Archival;
		                    Bib.Bib_Info.Original_Description.Extent = type;
	                    }
                    }
                }

                if (thisKey == "irtype_otherinput")
                {
                    string other_type = HttpContext.Current.Request.Form[thisKey];
                    if (other_type.Trim().Length > 0)
                    {
                        Bib.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Archival;
                        Bib.Bib_Info.Original_Description.Extent = HttpContext.Current.Request.Form[thisKey];
                    }
                }

                if (thisKey.IndexOf("largerbody") == 0)
                {
                    string largerbody = HttpContext.Current.Request.Form[thisKey];
                    if (largerbody.Trim().Length > 0)
                    {
                        switch (type)
                        {
                            case "Book Chapter":
                                Bib.Bib_Info.SeriesTitle.Clear();
                                Bib.Bib_Info.SeriesTitle.Title = largerbody;
                                break;

                            case "Conference Papers":
                            case "Conference Proceedings":
                                bool found_conference_name = false;
                                if (Bib.Bib_Info.Names_Count > 0)
                                {
                                    foreach (Name_Info thisName in Bib.Bib_Info.Names)
                                    {
                                        if (thisName.Name_Type == Name_Info_Type_Enum.conference)
                                        {
                                            thisName.Clear();
                                            thisName.Name_Type = Name_Info_Type_Enum.conference;
                                            thisName.Full_Name = largerbody;
                                            found_conference_name = true;
                                            break;
                                        }
                                    }
                                }
                                if (!found_conference_name)
                                {
                                    Name_Info conferenceName = new Name_Info
                                                                   {
                                                                       Name_Type = Name_Info_Type_Enum.conference,
                                                                       Full_Name = largerbody
                                                                   };
                                    Bib.Bib_Info.Add_Named_Entity(conferenceName);
                                }
                                break;

                            case "Course Material":
                                Bib.Bib_Info.SeriesTitle.Clear();
                                Bib.Bib_Info.SeriesTitle.Title = largerbody;
                                break;

                            case "Journal Article":
                                Bib.Bib_Info.SeriesTitle.Clear();
                                Bib.Bib_Info.SeriesTitle.Title = largerbody;
                                break;

                            case "Technical Reports":
                                Bib.Bib_Info.SeriesTitle.Clear();
                                Bib.Bib_Info.SeriesTitle.Title = largerbody;
                                break;

                            default:
                                Bib.Bib_Info.SeriesTitle.Clear();
                                Bib.Bib_Info.SeriesTitle.Title = largerbody;
                                break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods Implementing the Abstract Methods from abstract_Element class

        /// <summary> Reads the inner data from the CompleteTemplate XML format </summary>
        /// <param name="XMLReader"> Current template xml configuration reader </param>
        /// <remarks> This reads the possible values for the type combo box from a <i>options</i> subelement and the default value from a <i>value</i> subelement </remarks>
        protected override void Inner_Read_Data(XmlTextReader XMLReader)
        {
            default_values.Clear();
            while (XMLReader.Read())
            {
                if ((XMLReader.NodeType == XmlNodeType.Element) && ((XMLReader.Name.ToLower() == "value") || (XMLReader.Name.ToLower() == "options")))
                {
                    if (XMLReader.Name.ToLower() == "value")
                    {
                        XMLReader.Read();
                        default_values.Add(XMLReader.Value.Trim());
                    }
                    else
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
        public void Add_Item(string newItem)
        {
            if (!items.Contains(newItem))
            {
                items.Add(newItem);
            }
        }
    }
}