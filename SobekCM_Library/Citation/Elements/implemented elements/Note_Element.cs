#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Core.Configuration;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows simple entry of the note(s) for an item </summary>
    /// <remarks> This class extends the <see cref="textArea_Element"/> class. </remarks>
    public class Note_Element : textArea_Element
    {
        /// <summary> Constructor for a new instance of the Note_Element class </summary>
        public Note_Element()
            : base("Note", "note")
        {
            Repeatable = true;
            Type = Element_Type.Note;
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
                const string defaultAcronym = "Enter any notes about this digital manifestation or the original material";
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

            List<string> instanceValues = new List<string>();
            if (Bib.Bib_Info.Notes_Count > 0)
            {
                instanceValues.AddRange(from thisNote in Bib.Bib_Info.Notes where (thisNote.Note_Type != Note_Type_Enum.publication_status) && (thisNote.Note_Type != Note_Type_Enum.funding) && (thisNote.Note_Type != Note_Type_Enum.acquisition) && (thisNote.Note_Type != Note_Type_Enum.default_type) select thisNote.ToString().Replace("<b>", "(").Replace("</b>", ")"));
            }

            render_helper(Output, instanceValues, Skin_Code, IsMozilla, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears any preexisting notes which is neither the publication status note nor the funding note </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            if (Bib.Bib_Info.Notes_Count > 0)
            {
                List<Note_Info> deleteNotes = Bib.Bib_Info.Notes.Where(thisNote => (thisNote.Note_Type != Note_Type_Enum.publication_status) && (thisNote.Note_Type != Note_Type_Enum.funding) && (thisNote.Note_Type != Note_Type_Enum.acquisition) && (thisNote.Note_Type != Note_Type_Enum.default_type)).ToList();
                foreach (Note_Info thisNote in deleteNotes)
                {
                    Bib.Bib_Info.Remove_Note(thisNote);
                }
            }
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys.Where(thisKey => thisKey.IndexOf(html_element_name) == 0))
            {
                Bib.Bib_Info.Add_Note(HttpContext.Current.Request.Form[thisKey]);
            }
        }
    }
}
