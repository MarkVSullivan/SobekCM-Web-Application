#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows entry of the identifier for this item in the primary catalog record system </summary>
    /// <remarks> This class extends the <see cref="simpleTextBox_Element"/> class. </remarks>
    public class Catalog_Record_Number_Element : simpleTextBox_Element
    {
        /// <summary> Constructor for a new instance of the Catalog_Record_Number_Element class </summary>
        public Catalog_Record_Number_Element() : base("Catalog Number:", "catalognum")
        {
            Repeatable = false;
            Type = Element_Type.Catalog_Record_Number;
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
                const string defaultAcronym = "Identifier/number for this item in the primary catalog record system";
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

            const string identifierType1 = "CATRECORDNUM";
            string identifier_type2 = identifierType1;
            if (label_from_template_file.Length > 0)
            {
                identifier_type2 = label_from_template_file.Replace(" Number", "").Trim().ToUpper();
            }
            string identifier_value = String.Empty;
            foreach (Identifier_Info thisIdentifier in Bib.Bib_Info.Identifiers.Where(thisIdentifier => (thisIdentifier.Type.ToUpper() == identifierType1) || (thisIdentifier.Type.ToUpper() == identifier_type2)))
            {
                identifier_value = thisIdentifier.Identifier;
                break;
            }

            render_helper(Output, identifier_value, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> <remarks> Clears any existing catalog record numbers </remarks> </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            const string identifierType1 = "CATRECORDNUM";
            string identifier_type2 = identifierType1;
            if (label_from_template_file.Length > 0)
            {
                identifier_type2 = label_from_template_file.Replace(" Number", "").Trim().ToUpper();
            }

            List<Identifier_Info> deletes = Bib.Bib_Info.Identifiers.Where(thisIdentifier => (thisIdentifier.Type.ToUpper() == identifierType1) || (thisIdentifier.Type.ToUpper() == identifier_type2)).ToList();
            foreach (Identifier_Info thisIdentifier in deletes)
            {
                Bib.Bib_Info.Remove_Identifier(thisIdentifier);
            }
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
                    string identifier_type = "CATRECORDNUM";
                    if (label_from_template_file.Length > 0)
                    {
                        identifier_type = label_from_template_file.Replace(" Number", "").Trim().ToUpper();
                    }

                    string newNumber = HttpContext.Current.Request.Form[thisKey];
                    Bib.Bib_Info.Add_Identifier(newNumber, identifier_type);
                    return;
                }
            }
        }
    }
}