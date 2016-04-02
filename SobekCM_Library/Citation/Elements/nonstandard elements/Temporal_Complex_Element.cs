#region Using directives

using System;
using System.Collections.ObjectModel;
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
    /// <summary> Element allows entry of a temporal coverage keyword and year range for an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    public class Temporal_Complex_Element : abstract_Element
    {
        /// <summary> Constructor for a new instance of the Temporal_Complex_Element class </summary>
        public Temporal_Complex_Element()
        {
            Repeatable = true;
            Title = "Temporal Coverage";
            html_element_name = "complex_temporal";
	        help_page = "temporal";
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
                const string defaultAcronym = "Enter the period of time which is the subject of this material.";
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

            if (Bib.Bib_Info.TemporalSubjects_Count == 0)
            {
                const int  i = 1;
                Output.Write("              <span class=\"metadata_sublabel2\">" + Translator.Get_Translation("Start Year", CurrentLanguage) + ":</span>");
                Output.Write("<input name=\"" + id_name + "_start" + i + "\" id=\"" + id_name + "_start" + i + "\" class=\"" + html_element_name + "_year_input sbk_Focusable\" type=\"text\" value=\"\" />");
                Output.Write("<span class=\"metadata_sublabel\">" + Translator.Get_Translation("End Year", CurrentLanguage) + ":</span>");
				Output.Write("<input name=\"" + id_name + "_end" + i + "\" id=\"" + id_name + "_end" + i + "\" class=\"" + html_element_name + "_year_input sbk_Focusable\" type=\"text\" value=\"\" />");
                Output.Write("<span class=\"metadata_sublabel\">" + Translator.Get_Translation("Period", CurrentLanguage) + ":</span>");
				Output.WriteLine("<input name=\"" + id_name + "_period" + i + "\" id=\"" + id_name + "_period" + i + "\" class=\"" + html_element_name + "_period_input sbk_Focusable\" type=\"text\" value=\"\" /></div>");
                Output.WriteLine("            </div>");
            }
            else
            {
                ReadOnlyCollection<Temporal_Info> temporalSubjects = Bib.Bib_Info.TemporalSubjects;
                for (int i = 1; i <= temporalSubjects.Count; i++)
                {
                    Output.Write("              <span class=\"metadata_sublabel2\">" + Translator.Get_Translation("Start Year", CurrentLanguage) + ":</span>");
					Output.Write("<input name=\"" + id_name + "_start" + i + "\" id=\"" + id_name + "_start" + i + "\" class=\"" + html_element_name + "_year_input sbk_Focusable\" type=\"text\" value=\"" + temporalSubjects[i - 1].Start_Year.ToString().Replace("-1", "") + "\" />");
                    Output.Write("<span class=\"metadata_sublabel\">" + Translator.Get_Translation("End Year", CurrentLanguage) + ":</span>");
					Output.Write("<input name=\"" + id_name + "_end" + i + "\" id=\"" + id_name + "_end" + i + "\" class=\"" + html_element_name + "_year_input sbk_Focusable\" type=\"text\" value=\"" + temporalSubjects[i - 1].End_Year.ToString().Replace("-1", "") + "\" />");
                    Output.Write("<span class=\"metadata_sublabel\">" + Translator.Get_Translation("Period", CurrentLanguage) + ":</span>");
					Output.Write("<input name=\"" + id_name + "_period" + i + "\" id=\"" + id_name + "_period" + i + "\" class=\"" + html_element_name + "_period_input sbk_Focusable\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(temporalSubjects[i - 1].TimePeriod) + "\" />");

                    Output.WriteLine(i < temporalSubjects.Count ? "<br />" : "\n            </div>");
                }
            }
            Output.WriteLine("          </td>");

            Output.WriteLine("          <td style=\"vertical-align:bottom\" >");
            if (Repeatable)
            {
                Output.WriteLine("            <a title=\"" + Translator.Get_Translation("Click to add a new temporal coverage field", CurrentLanguage) + ".\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onmousedown=\"return add_temporal_element();\"><img class=\"repeat_button\" src=\"" + REPEAT_BUTTON_URL + "\" /></a>");
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
        /// <remarks> This clears any preexisting temporal subjects </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            Bib.Bib_Info.Clear_TemporalSubjects();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            const string HTML_ELEMENT_NAME = "complex_temporal";
            string id = HTML_ELEMENT_NAME.Replace("_", "");
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if ((thisKey.IndexOf(id) == 0) && (thisKey.IndexOf("end") < 0) && (thisKey.IndexOf("period") < 0))
                {
                    string start = HttpContext.Current.Request.Form[thisKey].Trim();

                    string key = thisKey.Replace(id + "_start", "");
                    string end_key = id + "_end" + key;
                    string period_key = id + "_period" + key;

                    string end = HttpContext.Current.Request.Form[end_key].Trim();
                    string period = HttpContext.Current.Request.Form[period_key].Trim();

                    if ((start.Length > 0) || (end.Length > 0) || (period.Length > 0))
                    {
                        int start_year;
                        int end_year;
                        if (!Int32.TryParse(start, out start_year))
                            start_year = -1;
                        if (!Int32.TryParse(end, out end_year))
                            end_year = -1;
                        Bib.Bib_Info.Add_Temporal_Subject(start_year, end_year, period);
                    }
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