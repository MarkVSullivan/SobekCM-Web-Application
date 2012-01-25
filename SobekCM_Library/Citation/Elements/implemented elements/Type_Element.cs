#region Using directives

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;
using SobekCM.Library.Application_State;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows simple entry of material type for an item </summary>
    /// <remarks> This class extends the <see cref="comboBox_Element"/> class. </remarks>
    public class Type_Element : comboBox_Element
    {
        /// <summary> Constructor for a new instance of the Type_Element class </summary>
        public Type_Element() : base("Resource Type", "type")
        {
            Repeatable = false;
            Type = Element_Type.Type;            
        }

        /// <summary> Sets the postback javascript, if the combo box requires a post back onChange </summary>
        /// <param name="postback_call"> Javascript call to perform onChange </param>
        public void Set_Postback(string postback_call)
        {
            onChange = postback_call; 
        }

        /// <summary> Renders the HTML for this element </summary>
        /// <param name="Output"> Textwriter to write the HTML for this element </param>
        /// <param name="Bib"> Object to populate this element from </param>
        /// <param name="Skin_Code"> Code for the current skin </param>
        /// <param name="isMozilla"> Flag indicates if the current browse is Mozilla Firefox (different css choices for some elements)</param>
        /// <param name="popup_form_builder"> Builder for any related popup forms for this element </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        /// <remarks> This simple element does not append any popup form to the popup_form_builder</remarks>
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool isMozilla, StringBuilder popup_form_builder, User_Object Current_User, Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string defaultAcronym = "Select the resource type information which best describes this material.";
                switch (CurrentLanguage)
                {
                    case Language_Enum.English:
                        Acronym = defaultAcronym;
                        break;

                    case Language_Enum.Spanish:
                        Acronym = defaultAcronym;
                        break;

                    case Language_Enum.French:
                        Acronym = defaultAcronym;
                        break;

                    default:
                        Acronym = defaultAcronym;
                        break;
                }
            }

            string thisType = Bib.Bib_Info.SobekCM_Type_String;
            if ( Bib.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Project )
            {
                thisType = String.Empty;
                if (Bib.Bib_Info.Notes_Count > 0)
                {
                    foreach (Note_Info thisNote in Bib.Bib_Info.Notes.Where(thisNote => thisNote.Note_Type == Note_Type_Enum.default_type))
                    {
                        thisType = thisNote.Note;
                        break;
                    }
                }
            }

            // Determine the material type
            if (thisType.Length == 0)
            {
                render_helper(Output, "Select Material Type", Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL, true);
            }
            else
            {
                render_helper(Output, thisType, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL, false);
            }
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This does nothing since there is only one type </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Do nothing since there is only one type
            if (Bib.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Project ) 
            {
                if (Bib.Bib_Info.Notes_Count > 0)
                {
                    Note_Info deleteNote = null;
                    foreach (Note_Info thisNote in Bib.Bib_Info.Notes.Where(thisNote => thisNote.Note_Type == Note_Type_Enum.default_type))
                    {
                        deleteNote = thisNote;
                    }
                    if (deleteNote != null)
                        Bib.Bib_Info.Remove_Note(deleteNote);
                }
            }
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
                if (Bib.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Project )
                {
                    if (thisType.Length > 0)
                    {
                        Bib.Bib_Info.Add_Note(thisType, Note_Type_Enum.default_type);
                    }
                }
                else
                {
                    Bib.Bib_Info.SobekCM_Type_String = thisType;
                }
                return;
            }            
        }

        /// <summary> Saves the constants to the bib id </summary>
        /// <param name="Bib"> Object into which to save this element's constant data </param>
        public override void Save_Constant_To_Bib(SobekCM_Item Bib)
        {
            if (default_values.Count > 0)
            {
                Bib.Bib_Info.SobekCM_Type_String = default_values[0];
            }
        }
    }
}
