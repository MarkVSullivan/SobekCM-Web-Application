#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Users;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows entry of the scale information for a cartographic item </summary>
    /// <remarks> This class extends the <see cref="simpleTextBox_Element"/> class. </remarks>
    public class Scale_Element : simpleTextBox_Element
    {
        /// <summary> Constructor for a new instance of the Scale_Element class </summary>
        public Scale_Element()
            : base("Scale", "scale")
        {
            Repeatable = false;
            Type = Element_Type.Scale;
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
                const string defaultAcronym = "If the original material is cartographic in nature and has scale indicated listed, include it here.";
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

            // Clear out the cartographics
            string scale = String.Empty;
            foreach (Subject_Info thisSubject in Bib.Bib_Info.Subjects.Where(thisSubject => thisSubject.Class_Type == Subject_Info_Type.Cartographics))
            {
                scale = ((Subject_Info_Cartographics)thisSubject).Scale;
                break;
            }

            render_helper(Output, scale, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This does nothing since there is only one edition </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Clear out the cartographics
            List<Subject_Info_Cartographics> clears = Bib.Bib_Info.Subjects.Where(thisSubject => thisSubject.Class_Type == Subject_Info_Type.Cartographics).Cast<Subject_Info_Cartographics>().ToList();

            foreach (Subject_Info_Cartographics clearSubject in clears)
            {
                Bib.Bib_Info.Remove_Subject(clearSubject);
            }  
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string scale in from thisKey in getKeys where thisKey.IndexOf(html_element_name.Replace("_", "")) == 0 select HttpContext.Current.Request.Form[thisKey])
            {
                if (scale.Length > 0)
                {
                    Bib.Bib_Info.Add_Cartographics_Subject().Scale = scale;
                }
                return;
            }
        }
    }
}