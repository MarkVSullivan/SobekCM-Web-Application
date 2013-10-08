#region Using directives

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
    /// <summary> Element allows simple entry of the creator(s) name for an item </summary>
    /// <remarks> This class extends the <see cref="simpleTextBox_Element"/> class. </remarks>
    public class Creator_Element : simpleTextBox_Element
    {
        /// <summary> Constructor for a new instance of the Creator_Element class </summary>
        public Creator_Element()
            : base("Creator", "creator")
        {
            Repeatable = true;
            Display_SubType = "simple";
            Type = Element_Type.Creator;
        }

        /// <summary> Sets the flag that the contributor is included in this element, so 
        /// they should be omitted here </summary>
        public bool Contributor_Included { private get; set; }

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
                const string defaultAcronym = "Enter each person or group which created this material. Personal names should be entered as [Family Name], [Given Name].";
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
            if (( Bib.Bib_Info.hasMainEntityName ) && ( Bib.Bib_Info.Main_Entity_Name.ToString().Length > 0))
            {
                string main_name_as_string = Bib.Bib_Info.Main_Entity_Name.ToString();
                if ((main_name_as_string != "unknown") && (main_name_as_string.Length > 0))
                    instanceValues.Add(main_name_as_string);
            }
            foreach (Name_Info thisName in Bib.Bib_Info.Names)
            {
                bool include = true;
                if (Contributor_Included)
                {
                    if (thisName.Roles.Any(thisRole => thisRole.Role.ToLower() == "contributor"))
                    {
                        include = false;
                    }
                }
                if (include)
                {
                    string name_as_string = thisName.ToString();
                    if ((name_as_string != "unknown") && (name_as_string.Length > 0))
                        instanceValues.Add(name_as_string);
                }
            }

            render_helper(Output, instanceValues, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears the main entity name and any other names associated with the digital resource </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            if ( Bib.Bib_Info.hasMainEntityName )
               Bib.Bib_Info.Main_Entity_Name.Clear();
            Bib.Bib_Info.Clear_Names();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf( html_element_name ) == 0)
                {
                    Bib.Bib_Info.Add_Named_Entity(new Name_Info(HttpContext.Current.Request.Form[thisKey], ""));
                }
            }  
        }
    }
}
