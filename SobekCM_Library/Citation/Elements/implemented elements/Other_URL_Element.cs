#region Using directives

using System;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Users;
using SobekCM.Resource_Object;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element displays a form to allow for simple entry of a related URL for an item </summary>
    /// <remarks> This class extends the <see cref="simpleTextBox_Element"/> class. </remarks>
    public class Other_URL_Element : simpleTextBox_Element
    {
        /// <summary> Constructor for a new instance of the Identifier_Fixed_Type_Element class </summary>
        public Other_URL_Element()
            : base("Related URL", "related_url")
        {
            Repeatable = false;
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
                const string defaultAcronym = "Enter any related URL for this material";
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

            if ((label_from_template_file.Length > 0) && (fixed_type_from_template_file.Length == 0))
            {
                fixed_type_from_template_file = label_from_template_file;
            }
            if ((label_from_template_file.Length == 0) && (fixed_type_from_template_file.Length > 0))
            {
                label_from_template_file = fixed_type_from_template_file;
            }

            if (label_from_template_file.Length > 0)
                Title = label_from_template_file;

            render_helper(Output, Bib.Bib_Info.Location.Other_URL, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL, "fixed" + fixed_type_from_template_file.Replace(" ", "_").Replace("'","").ToLower() + "other_url");
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This does nothing since there is only one other url </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Do nothing
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string form_id = "fixed" + label_from_template_file.Replace(" ", "_").Replace("'", "").ToLower() + "other_url1";
            Bib.Bib_Info.Location.Other_URL = HttpContext.Current.Request.Form[form_id];
            if (Bib.Bib_Info.Location.Other_URL.Length == 0)
            {
                Bib.Bib_Info.Location.Other_URL_Display_Label = String.Empty;
                Bib.Bib_Info.Location.Other_URL_Note = String.Empty;
            }
            else
            {
                Bib.Bib_Info.Location.Other_URL_Display_Label = fixed_type_from_template_file;
                Bib.Bib_Info.Location.Other_URL_Note = String.Empty;
            }
        }
    }
}
