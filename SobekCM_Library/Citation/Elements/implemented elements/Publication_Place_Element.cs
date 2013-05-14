#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// <summary> Element allows simple entry of the publication place(s) for an item </summary>
    /// <remarks> This class extends the <see cref="simpleTextBox_Element"/> class. </remarks>
    class Publication_Place_Element : simpleTextBox_Element
    {
        /// <summary> Constructor for a new instance of the Publication_Place_Element class </summary>
        public Publication_Place_Element()
            : base("Place of Publication", "pubplace")
        {
            Repeatable = true;
            

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
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool isMozilla, StringBuilder popup_form_builder, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string defaultAcronym = "Enter the geographic location of the publisher(s) of this material";
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
            if (Bib.Bib_Info.Publishers_Count > 0)
            {
                foreach (Publisher_Info thisName in Bib.Bib_Info.Publishers)
                {
                    instanceValues.AddRange(from thisPlace in thisName.Places where thisPlace.Place_Text.Length > 0 select thisPlace.Place_Text);
                }
            }

            render_helper(Output, instanceValues, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This does nothing since there is only one publication place possible with this element(?) </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Do nothing since there is only one title
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            // First collect all the places
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            List<string> publication_places = getKeys.Where(thisKey => thisKey.IndexOf(html_element_name) == 0).Select(thisKey => HttpContext.Current.Request.Form[thisKey]).Where(place_temp => place_temp.Length > 0).ToList();

            // If no places, done
            if (publication_places.Count == 0)
                return;

            // Is there no publishers?
            if (Bib.Bib_Info.Publishers_Count == 0)
            {
                Bib.Bib_Info.Add_Publisher(String.Empty);
            }

            // Is there just one publisher?
            if (Bib.Bib_Info.Publishers_Count == 1)
            {
                ReadOnlyCollection<Publisher_Info> publishers = Bib.Bib_Info.Publishers;
                foreach (string thisPubPlace in publication_places)
                {
                    bool found = publishers[0].Places.Any(thisPlace => thisPlace.Place_Text.ToUpper().Trim() == thisPubPlace.ToUpper().Trim());
                    if (!found)
                    {
                        publishers[0].Add_Place(thisPubPlace);
                    }
                }
            }
            else
            {
                ReadOnlyCollection<Publisher_Info> publishers = Bib.Bib_Info.Publishers;
                for (int i = 0; i < publishers.Count && i < publication_places.Count; i++)
                {
                    publishers[i].Add_Place(publication_places[i]);
                }
            }
        }
    }
}
