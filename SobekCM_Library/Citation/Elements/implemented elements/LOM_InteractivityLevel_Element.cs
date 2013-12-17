#region Using directives

using System;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.LearningObjects;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows entry of the learning object metadata interactivity level field </summary>
    /// <remarks> This class extends the <see cref="comboBox_Element"/> class. </remarks>
    public class LOM_InteractivityLevel_Element : comboBox_Element
    {
        private const string LEVEL1_TEXT = "very low";
        private const string LEVEL2_TEXT = "low";
        private const string LEVEL3_TEXT = "medium";
        private const string LEVEL4_TEXT = "high";
        private const string LEVEL5_TEXT = "very high";

        /// <summary> Constructor for a new instance of the LOM_InteractivityLevel_Element class </summary>
        public LOM_InteractivityLevel_Element()
            : base("Interactivity Level", "lom_interactlevel")
        {
            Repeatable = false;
            Type = Element_Type.LOM_Interactivity_Level;

            items.Clear();
            items.Add(String.Empty);
            items.Add(LEVEL1_TEXT);
            items.Add(LEVEL2_TEXT);
            items.Add(LEVEL3_TEXT);
            items.Add(LEVEL4_TEXT);
            items.Add(LEVEL5_TEXT);
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
                const string DEFAULT_ACRONYM = "Degree of interactivity characterizing this learning object.  Refers to degree to which the learner can influence the aspect or behavior of the learning object.";
                switch (CurrentLanguage)
                {
                    case Web_Language_Enum.English:
                        Acronym = DEFAULT_ACRONYM;
                        break;

                    case Web_Language_Enum.Spanish:
                        Acronym = DEFAULT_ACRONYM;
                        break;

                    case Web_Language_Enum.French:
                        Acronym = DEFAULT_ACRONYM;
                        break;

                    default:
                        Acronym = DEFAULT_ACRONYM;
                        break;
                }
            }

            // Determine the value from the enum
            string value = String.Empty;
            
            // Try to get the learning metadata here
            LearningObjectMetadata lomInfo = Bib.Get_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY) as LearningObjectMetadata;
            if (lomInfo != null)
            {
                switch ( lomInfo.InteractivityLevel )
                {
                    case InteractivityLevelEnum.very_low:
                        value = LEVEL1_TEXT;
                        break;

                    case InteractivityLevelEnum.low:
                        value = LEVEL2_TEXT;
                        break;

                    case InteractivityLevelEnum.medium:
                        value = LEVEL3_TEXT;
                        break;

                    case InteractivityLevelEnum.high:
                        value = LEVEL4_TEXT;
                        break;

                    case InteractivityLevelEnum.very_high:
                        value = LEVEL5_TEXT;
                        break;
                }
            }

            render_helper(Output, value, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This does nothing since this is a singleton value, and is non-repeatable </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Do nothing since there is only one corresponding value
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf(html_element_name.Replace("_","")) == 0)
                {
                    // Get the value from the combo box
                    string value = HttpContext.Current.Request.Form[thisKey].Trim();

                    // Try to get any existing learning object metadata module
                    LearningObjectMetadata lomInfo = Bib.Get_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY) as LearningObjectMetadata;

                    if (value.Length == 0)
                    {
                        // I fhte learning object metadata does exist, set it to undefined
                        if ( lomInfo != null )
                            lomInfo.InteractivityLevel = InteractivityLevelEnum.UNDEFINED;
                    }
                    else
                    {
                        // There is a value, so ensure learning object metadata does exist
                        if (lomInfo == null)
                        {
                            lomInfo = new LearningObjectMetadata();
                            Bib.Add_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY, lomInfo);
                        }

                        // Save the new value
                        switch ( value )
                        {
                            case LEVEL1_TEXT:
                                lomInfo.InteractivityLevel = InteractivityLevelEnum.very_low;
                                break;

                            case LEVEL2_TEXT:
                                lomInfo.InteractivityLevel = InteractivityLevelEnum.low;
                                break;

                            case LEVEL3_TEXT:
                                lomInfo.InteractivityLevel = InteractivityLevelEnum.medium;
                                break;

                            case LEVEL4_TEXT:
                                lomInfo.InteractivityLevel = InteractivityLevelEnum.high;
                                break;

                            case LEVEL5_TEXT:
                                lomInfo.InteractivityLevel = InteractivityLevelEnum.very_high;
                                break;
                        }
                    }
                    return;
                }
            }            
        }
    }
}