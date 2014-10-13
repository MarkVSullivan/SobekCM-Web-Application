#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
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
    /// <summary> Element allows simple entry of the genre keywords for an item </summary>
    /// <remarks> This class extends the <see cref="textBox_TextBox_Element"/> class. </remarks>
    public class Genre_Element : textBox_TextBox_Element
    {
        /// <summary> Constructor for a new instance of the Genre_Element class </summary>
        public Genre_Element()
            : base("Genre", "genre")
        {
            second_label = "Authority";
            Repeatable = true;
            Type = Element_Type.Genre;
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
                const string defaultAcronym = "Genres to which this material belongs.  (i.e., catalogs, government publication, etc..)";
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

            List<string> terms = new List<string>();
            List<string> schemes = new List<string>();
            if (Bib.Bib_Info.Genres_Count > 0)
            {
                foreach (Genre_Info thisGenre in Bib.Bib_Info.Genres)
                {
                    terms.Add(thisGenre.Genre_Term);
                    schemes.Add(thisGenre.Authority);
                }
            }

            render_helper(Output, terms, schemes, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears any preexisting genres </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            Bib.Bib_Info.Clear_Genres();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            Dictionary<string, string> terms = new Dictionary<string, string>();
            Dictionary<string, string> schemes = new Dictionary<string, string>();

            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_first") == 0)
                {
                    string term = HttpContext.Current.Request.Form[thisKey];
                    string index = thisKey.Replace(html_element_name.Replace("_", "") + "_first", "");
                    terms[index] = term;
                }

                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_second") == 0)
                {
                    string scheme = HttpContext.Current.Request.Form[thisKey];
                    string index = thisKey.Replace(html_element_name.Replace("_", "") + "_second", "");
                    schemes[index] = scheme;
                }
            }

            foreach (string index in terms.Keys)
            {
                Bib.Bib_Info.Add_Genre(terms[index], schemes.ContainsKey(index) ? schemes[index] : String.Empty);
            }
        }
    }
}
