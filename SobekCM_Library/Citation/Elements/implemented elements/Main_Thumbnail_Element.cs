#region Using directives

using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Bib_Package;
using SobekCM.Library.Application_State;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows entry of the main thumbnail for an item </summary>
    /// <remarks> This class extends the <see cref="comboBox_Element"/> class. </remarks>
    public class Main_Thumbnail_Element : comboBox_Element
    {
        /// <summary> Constructor for a new instance of the Main_Thumbnail_Element class </summary>
        public Main_Thumbnail_Element()
            : base("Main Thumbnail", "main_thumbnail")
        {
            Repeatable = false;
            Type = Element_Type.MainThumbnail;

            items.Clear();
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
                const string defaultAcronym = "Select the image to use as the main thumbnail when searching or browsing.";
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

            // Add all the possible thumbnail files
            items.Clear();
            ReadOnlyCollection<string> files = Bib.SobekCM_Web.Get_Thumbnail_Files(SobekCM_Library_Settings.Image_Server_Network + Bib.SobekCM_Web.AssocFilePath);
            foreach (string thisFile in files)
            {
                items.Add(thisFile);
            }

            render_helper(Output, Bib.SobekCM_Web.Main_Thumbnail, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This does nothing since there is only one main thumbnail </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Do nothing since there is only one thumbnail
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys.Where(thisKey => thisKey.IndexOf(html_element_name.Replace("_","")) == 0))
            {
                Bib.SobekCM_Web.Main_Thumbnail = HttpContext.Current.Request.Form[thisKey];
                return;
            }
        }
    }
}



