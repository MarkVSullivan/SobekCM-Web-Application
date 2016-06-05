using System;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.UI_Configuration.Citation;
using SobekCM.Core.Users;
using SobekCM.Library.UI;
using SobekCM.Resource_Object;

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows user to select the citation set that this item should appear under </summary>
    /// <remarks> This class extends the <see cref="comboBox_Element"/> class. </remarks>
    public class CitationSet_Element : comboBox_Element
    {
        /// <summary> Constructor for a new instance of the CitationSet_Element class </summary>
        public CitationSet_Element() : base("Citation Set", "citation")
        {
            Repeatable = false;

            items.Add("");

            string defaultSet = UI_ApplicationCache_Gateway.Configuration.UI.CitationViewer.DefaultCitationSet;
            if (!String.IsNullOrEmpty(defaultSet))
                items.Add(defaultSet);

            foreach (CitationSet citationSet in UI_ApplicationCache_Gateway.Configuration.UI.CitationViewer.CitationSets)
            {
                if (( String.IsNullOrEmpty(defaultSet)) || ( String.Compare(defaultSet, citationSet.Name, StringComparison.OrdinalIgnoreCase ) != 0 ))
                    items.Add(citationSet.Name);
            }
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
                const string defaultAcronym = "Select the citation set under which this should appear.  This can affect the way the description or citation appears for this item.";
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

            string value = Bib.Behaviors.CitationSet;

            render_helper(Output, value, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL, false);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This does nothing since there is only one citation set value </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Do nothing since there is only one citation set value
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf(html_element_name.Replace("_", "")) != 0) continue;

                string type_value = HttpContext.Current.Request.Form[thisKey];
                if (type_value.IndexOf("Select") < 0)
                {
                    Bib.Behaviors.CitationSet = type_value;
                }
                return;
            }
        }
    }
}
