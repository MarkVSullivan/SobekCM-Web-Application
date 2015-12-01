#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Users;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows simple entry of frequency for a continuing resource type of item </summary>
    /// <remarks> This class extends the <see cref="comboBox_Element"/> class. </remarks>
    public class Frequency_Element: multipleComboBox_Element
    {
        /// <summary> Constructor for a new instance of the Frequency_Element class </summary>
        public Frequency_Element()
            : base("Frequency", "frequency")
        {
            Repeatable = true;
            Type = Element_Type.Frequency;
            Add_Items(new[] { "", "annual", "biennial", "bimonthly", "biweekly","continuously updated","daily","monthly","other",
                "quarterly","regular", "semiannual","semimonthly","semiweekly","three times a month","three times a week","three times a year","triennial","weekly" });

            boxes_per_line = 3;
            max_boxes = 6;
        }

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
                const string defaultAcronym = "Select the frequency for this continuing resource type material.";
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

            List<string> frequencies = new List<string>();
            foreach (Origin_Info_Frequency frequency in Bib.Bib_Info.Origin_Info.Frequencies)
            {
                if (!frequencies.Contains(frequency.Term.ToLower()))
                    frequencies.Add(frequency.Term.ToLower());
            }
            render_helper(Output, frequencies, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This does nothing since there is only one type </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
             Bib.Bib_Info.Origin_Info.Clear_Frequencies();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf(html_element_name.Replace("_", "")) == 0)
                {
                    string frequency = HttpContext.Current.Request.Form[thisKey];
                    if (frequency.Length > 0)
                    {
                        string authority = String.Empty;
                        if ((frequency == "annual") || (frequency == "biennial") || (frequency == "bimonthly") || (frequency == "biweekly") ||
                            (frequency == "continuously updated") || (frequency == "daily") || (frequency == "monthly") || (frequency == "other") ||
                            (frequency == "quarterly") || (frequency == "semiannual") || (frequency == "semimonthly") || (frequency == "semiweekly") ||
                            (frequency == "regular") || (frequency == "three times a month") || (frequency == "three times a week") || (frequency == "three times a year") || (frequency == "triennial") || (frequency == "weekly"))
                        {
                            authority = "marcfrequency";
                        }

                        Bib.Bib_Info.Origin_Info.Add_Frequency(frequency, authority);
                    }
                }
            }           
        }
    }
}
