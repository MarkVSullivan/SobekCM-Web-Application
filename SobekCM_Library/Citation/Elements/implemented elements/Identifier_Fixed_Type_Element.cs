#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// <summary> Element allows entry of an identifier with a fixed type, which also appears as the title for the metadata element </summary>
    /// <remarks> This class extends the <see cref="simpleTextBox_Element"/> class. </remarks>
    public class Identifier_Fixed_Type_Element : simpleTextBox_Element
    {
        /// <summary> Constructor for a new instance of the Identifier_Fixed_Type_Element class </summary>
        public Identifier_Fixed_Type_Element()
            : base("Identifier", "identifier")
        {
            Repeatable = true;
            Display_SubType = "fixed_type";
            Type = Element_Type.Identifier;
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
                const string defaultAcronym = "Enter the fixed type of identifier.";
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

            List<string> terms = new List<string>();
            if (Bib.Bib_Info.Identifiers_Count > 0)
            {
                terms.AddRange(Bib.Bib_Info.Identifiers.Select(thisIdentifier => thisIdentifier.Identifier));
            }
 
            Title = label_from_template_file;
            if (label_from_template_file.Length == 0)
                Title = "MISSING LABEL!";

            render_helper(Output, terms, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL,  fixed_type_from_template_file.Replace(" ", "_").ToLower() + "fixedidentifier");
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears any preexisting identifiers </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            Bib.Bib_Info.Clear_Identifiers();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string special_id = fixed_type_from_template_file.Replace(" ", "_").ToLower() + "fixedidentifier";
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys.Where(thisKey => thisKey.IndexOf(special_id) == 0))
            {
                Bib.Bib_Info.Add_Identifier(HttpContext.Current.Request.Form[thisKey], fixed_type_from_template_file);
            }
        }
    }
}
