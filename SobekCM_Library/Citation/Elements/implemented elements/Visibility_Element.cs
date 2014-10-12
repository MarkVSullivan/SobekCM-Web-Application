#region Using directives

using System;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Core.Configuration;
using SobekCM.Resource_Object;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows simple entry of the visibility of this item ( i.e., PRIVATE, PUBLIC, RESTRICTED ) </summary>
    /// <remarks> This class extends the <see cref="comboBox_Element"/> class. </remarks>
    public class Visibility_Element : comboBox_Element
    {
        /// <summary> Constructor for a new instance of the Visibility_Element class </summary>
        public Visibility_Element()
            : base("Visibility", "visibility")
        {
            Repeatable = false;
            Type = Element_Type.Visibility;

            items.Add("PRIVATE");
            items.Add("IP RESTRICTED");
            items.Add("PUBLIC");
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
                const string defaultAcronym = "Select whether this item should be marked private, public, or ip restricted.";
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

            string value = "PRIVATE";
            if (Bib.Behaviors.IP_Restriction_Membership == 0)
                value = "PUBLIC";
            if (Bib.Behaviors.IP_Restriction_Membership > 0)
                value = "IP RESTRICTED";

            render_helper(Output, value, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL, false);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This does nothing since there is only one visibility value </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Do nothing since there is only one visibility value
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
                string thisType = String.Empty;
                if (type_value.IndexOf("Select") < 0)
                {
                    thisType = type_value;
                }
                switch( thisType )
                {
                    case "PUBLIC":
                        Bib.Behaviors.IP_Restriction_Membership = 0;
                        break;

                    case "PRIVATE":
                        Bib.Behaviors.IP_Restriction_Membership = -1;
                        break;

                    case "IP RESTRICTED":
                        Bib.Behaviors.IP_Restriction_Membership = 1;
                        break;
                }
                return;
            }
        }
    }
}
