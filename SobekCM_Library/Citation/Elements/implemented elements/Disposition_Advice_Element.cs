#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Settings;
using SobekCM.Core.Users;
using SobekCM.Resource_Object;
using SobekCM.UI_Library;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows entry of the disposition advice, both the controlled field and free-text field</summary>
    /// <remarks> This class extends the <see cref="comboBox_TextBox_Element"/> class. </remarks>
    public class Disposition_Advice_Element : comboBox_TextBox_Element
    {
        /// <summary> Constructor for a new instance of the Disposition_Advice_Element class </summary>
        public Disposition_Advice_Element()
            : base("Disposition Advice", "disposition_advice")
        {
            Repeatable = false;
            possible_select_items.Clear();

            List<Disposition_Option> futureTypes = UI_ApplicationCache_Gateway.Settings.Disposition_Options;
            possible_select_items.Add(String.Empty);
            foreach (Disposition_Option thisType in futureTypes)
            {
                possible_select_items.Add(thisType.Future);
            }
            Type = Element_Type.Disposition_Advice;
            second_label = "Notes";
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
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool IsMozilla, StringBuilder PopupFormBuilder, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL)
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string defaultAcronym = "Provide instruction on how the physical material should be treated after digization is complete.";
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

            string term = String.Empty;
            if ( Bib.Tracking.Disposition_Advice > 0 )
                term =  UI_ApplicationCache_Gateway.Settings.Disposition_Term_Future( Bib.Tracking.Disposition_Advice);
            render_helper(Output, term, Bib.Tracking.Disposition_Advice_Notes, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL, false);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears the notes and advice </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            Bib.Tracking.Disposition_Advice = -1;
            Bib.Tracking.Disposition_Advice_Notes = String.Empty;
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            // Pull the standard values
            NameValueCollection form = HttpContext.Current.Request.Form;

            string advice = String.Empty;
            string notes = String.Empty;

            foreach (string thisKey in form.AllKeys)
            {
                if (thisKey.IndexOf("dispositionadvice_select") == 0)
                {
                    advice = form[thisKey];
                }

                if ( thisKey.IndexOf("dispositionadvice_text") == 0 )
                {
                    notes = form[thisKey];
                }
            }

            if (advice.Length > 0)
            {
                Bib.Tracking.Disposition_Advice = (short) UI_ApplicationCache_Gateway.Settings.Disposition_ID_Future(advice);
                Bib.Tracking.Disposition_Advice_Notes = notes;

            }
        }
    }
}



