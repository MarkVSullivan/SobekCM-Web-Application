#region Using directives

using System;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Users;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows entry of an abstract, including abstract type and language, for an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    public class Abstract_Complex_Element : abstract_Element
    {
        private readonly int cols;
        private readonly int colsMozilla;

        /// <summary> Constructor for a new instance of the Abstract_Complex_Element class </summary>
        public Abstract_Complex_Element()
        {
            Rows = 3;
            Repeatable = true;
            Title = "Abstract";
            cols = TEXT_AREA_COLUMNS;
            colsMozilla = MOZILLA_TEXT_AREA_COLUMNS;
            html_element_name = "complex_abstract";
	        help_page = "abstract";
        }

        /// <summary> Gets and sets the number of lines for this text box </summary>
        protected int Rows { get; set; }

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
                const string DEFAULT_ACRONYM = "Enter your abstract here. If your material does not have an abstract, you may include a summary of your document here.";
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

            // Determine the columns for this text area, based on browser
            int actual_cols = cols;
            if (IsMozilla)
                actual_cols = colsMozilla;

            
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
            Output.WriteLine("        <tr style=\"text-align:left;\">");
            Output.WriteLine("          <td>");
            Output.WriteLine("            <div id=\"" + html_element_name + "_div\">");

            if (Bib.Bib_Info.Abstracts_Count == 0)
            {
                Output.WriteLine("              <div id=\"" + html_element_name + "_topdiv1\">");
                Output.WriteLine("                <span class=\"metadata_sublabel2\">" + Translator.Get_Translation("Type", CurrentLanguage) +":</span>");
                Output.WriteLine("                <select class=\"" + html_element_name + "_type\" name=\"" + id_name + "_type1\" id=\"" + id_name + "_type1\" >");
                Output.WriteLine("                  <option selected=\"selected=\" value=\"\"></option>");
                Output.WriteLine("                  <option value=\"abstract\">Abstract</option>");
                Output.WriteLine("                  <option value=\"content\">Content Advice</option>");
                Output.WriteLine("                  <option value=\"review\">Review</option>");
                Output.WriteLine("                  <option value=\"scope\">Scope and Content</option>");
                Output.WriteLine("                  <option value=\"subject\">Subject</option>");
                Output.WriteLine("                  <option value=\"summary\">Summary</option>");
                Output.WriteLine("                </select>");

                Output.WriteLine("                <span class=\"metadata_sublabel\">" + Translator.Get_Translation("Language", CurrentLanguage) + ":</span>");
				Output.WriteLine("                <input name=\"" + id_name + "_language1\" id=\"" + id_name + "_language1\" class=\"" + html_element_name + "_language sbk_Focusable\" type=\"text\" value=\"\" />");
                Output.WriteLine("              </div>");
				Output.WriteLine("              <textarea rows=\"" + Rows + "\" cols=\"" + actual_cols + "\" name=\"" + id_name + "_textarea1\" id=\"" + id_name + "_textarea1\" class=\"" + html_element_name + "_input sbk_Focusable\" ></textarea>");
            }
            else
            {
                int i = 1;
                foreach (Abstract_Info thisAbstract in Bib.Bib_Info.Abstracts)
                {
                    Output.WriteLine("              <div id=\"" + html_element_name + "_topdiv" + i + "\">");
                    Output.WriteLine("                <span class=\"metadata_sublabel2\">Type:</span>");
                    Output.WriteLine("                <select class=\"" + html_element_name + "_type\" name=\"" + id_name + "_type" + i + "\" id=\"" + id_name + "_type" + i + "\" >");
                    Output.WriteLine("                  <option value=\"\"></option>");

                    Output.WriteLine(thisAbstract.Type.ToLower() != "abstract"
                                         ? "                  <option value=\"abstract\">Abstract</option>"
                                         : "                  <option value=\"abstract\" selected=\"selected\">Abstract</option>");

                    Output.WriteLine(thisAbstract.Type.ToLower() != "content advice"
                                         ? "                  <option value=\"content\">Content Advice</option>"
                                         : "                  <option value=\"content\" selected=\"selected\">Content Advice</option>");

                    Output.WriteLine(thisAbstract.Type.ToLower() != "review"
                                         ? "                  <option value=\"review\">Review</option>"
                                         : "                  <option value=\"review\" selected=\"selected\">Review</option>");

                    Output.WriteLine(thisAbstract.Type.ToLower() != "scope and content"
                                         ? "                  <option value=\"scope\">Scope and Content</option>"
                                         : "                  <option value=\"scope\" selected=\"selected\">Scope and Content</option>");

                    Output.WriteLine(thisAbstract.Type.ToLower() != "subject"
                                         ? "                  <option value=\"subject\">Subject</option>"
                                         : "                  <option value=\"subject\" selected=\"selected\">Subject</option>");

                    Output.WriteLine(thisAbstract.Type.ToLower() != "summary"
                                         ? "                  <option value=\"summary\">Summary</option>"
                                         : "                  <option value=\"summary\" selected=\"selected\">Summary</option>");

                    Output.WriteLine("                </select>");
                    
                    Output.WriteLine("                <span class=\"metadata_sublabel\">Language:</span>");
					Output.WriteLine("                <input name=\"" + id_name + "_language1\" id=\"" + id_name + "_language1\" class=\"" + html_element_name + "_language sbk_Focusable\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisAbstract.Language) + "\" />");
                    Output.WriteLine("              </div>");
					Output.Write("              <textarea rows=\"" + Rows + "\" cols=\"" + actual_cols + "\" name=\"" + id_name + "_textarea" + i + "\" id=\"" + id_name + "_textarea" + i + "\" class=\"" + html_element_name + "_input sbk_Focusable\" >" + HttpUtility.HtmlEncode(thisAbstract.Abstract_Text) + "</textarea>");


                    if ( i < Bib.Bib_Info.Notes_Count )
                    {
                        Output.WriteLine("<br />");
                    }
                    else
                    {
                        Output.WriteLine();
                    }

                    i++;
                }
            }

            Output.WriteLine("            </div>");
            Output.WriteLine("          </td>");

            Output.WriteLine("          <td style=\"vertical-align:bottom\">");
            if (Repeatable)
            {
                Output.WriteLine("            <a title=\"" + Translator.Get_Translation("Click to add a new abstract", CurrentLanguage) + ".\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onmousedown=\"return add_complex_abstract('" + Rows + "','" + actual_cols + "');\"><img class=\"repeat_button\" src=\"" + REPEAT_BUTTON_URL + "\" /></a>");
            }

            Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img class=\"help_button\" src=\"" + HELP_BUTTON_URL + "\" /></a>");

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
        /// <remarks> This clears any preexisting abstracts </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            Bib.Bib_Info.Clear_Abstracts();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf("complexabstract_type") != 0) continue;

                string key = thisKey.Replace("complexabstract_type", "");
                string language_key = "complexabstract_language" + key;
                string textarea_key = "complexabstract_textarea" + key;

                string display = HttpContext.Current.Request.Form[thisKey].Trim();
                string displayLabel = "Abstract";
                string type = String.Empty;
                switch( display )
                {
                    case "content":
                        type = "content advice";
                        displayLabel = "Content Advice";
                        break;

                    case "review":
                        type = "review";
                        displayLabel = "Review";
                        break;

                    case "scope":
                        type = "scope and content";
                        displayLabel = "Scope and Content";
                        break;

                    case "subject":
                        type = "subject";
                        displayLabel = "Subject";
                        break;

                    case "summary":
                        type = "summary";
                        displayLabel = "Summary";
                        break;
                }


                string language = String.Empty;
                if (HttpContext.Current.Request.Form[language_key] != null)
                {
                    language = HttpContext.Current.Request.Form[language_key].Trim();
                }
                string textarea = HttpContext.Current.Request.Form[textarea_key].Trim();

                if (textarea.Length > 0)
                {
                    Bib.Bib_Info.Add_Abstract(textarea, language, type, displayLabel);
                }
            }
        }

        #endregion

        #region Methods Implementing the Abstract Methods from abstract_Element class

        /// <summary> Reads the inner data from the CompleteTemplate XML format </summary>
        /// <param name="XMLReader"> Current template xml configuration reader </param>
        /// <remarks> This procedure does not currently read any inner xml (not yet necessary) </remarks>
        protected override void Inner_Read_Data(XmlTextReader XMLReader)
        {
            // Do nothing
        }

        #endregion
    }
}