#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Users;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.LearningObjects;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows entry of the learning object metadata intended end user field </summary>
    /// <remarks> This class extends the <see cref="multipleComboBox_Element"/> class. </remarks>
    public class LOM_IntendedUser_Element : multipleComboBox_Element
    {
        private const string level1_text = "teacher";
        private const string level2_text = "author";
        private const string level3_text = "learner";
        private const string level4_text = "manager";

        /// <summary> Constructor for a new instance of the LOM_IntendedUser_Element class </summary>
        public LOM_IntendedUser_Element() : base("Intended User", "lom_intendeduser")
        {
            Repeatable = true;
            Type = Element_Type.LOM_Intended_End_User_Role;
            view_choices_string = String.Empty;

            boxes_per_line = 4;
            max_boxes = 4;

            items.Clear();
            items.Add(level1_text);
            items.Add(level2_text);
            items.Add(level3_text);
            items.Add(level4_text);
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
                const string defaultAcronym = "Principal user(s) for which this learning object was designed, most dominant first.";
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

            // Start the list to collect all current end user roles 
            List<string> endusers = new List<string>();

            // Try to get any existing learning object metadata module
            LearningObjectMetadata lomInfo = Bib.Get_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY) as LearningObjectMetadata;
            if (lomInfo != null)
            {
                foreach (IntendedEndUserRoleEnum endUserRole in lomInfo.IntendedEndUserRoles)
                {
                    switch( endUserRole )
                    {
                        case IntendedEndUserRoleEnum.teacher:
                            endusers.Add(level1_text);
                            break;

                        case IntendedEndUserRoleEnum.author:
                            endusers.Add(level2_text);
                            break;

                        case IntendedEndUserRoleEnum.learner:
                            endusers.Add(level3_text);
                            break;

                        case IntendedEndUserRoleEnum.manager:
                            endusers.Add(level4_text);
                            break;
                    }
                }
            }

            render_helper(Output, endusers, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }



        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears any preexisting learning object metadata intended end user </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Try to get any existing learning object metadata module
            LearningObjectMetadata lomInfo = Bib.Get_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY) as LearningObjectMetadata;
            if ( lomInfo != null )
                lomInfo.Clear_IntendedEndUserRoles();

        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            // Try to get any existing learning object metadata module
            LearningObjectMetadata lomInfo = Bib.Get_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY) as LearningObjectMetadata;

            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf(html_element_name.Replace("_", "")) == 0)
                {
                    // Get the value from the combo box
                    string value = HttpContext.Current.Request.Form[thisKey].Trim();
                    if (value.Length > 0)
                    {
                        // There is a value, so ensure learning object metadata does exist
                        if (lomInfo == null)
                        {
                            lomInfo = new LearningObjectMetadata();
                            Bib.Add_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY, lomInfo);
                        }

                        // Save the new value
                        switch (value)
                        {
                            case level1_text:
                                lomInfo.Add_IntendedEndUserRole(IntendedEndUserRoleEnum.teacher);
                                break;

                            case level2_text:
                                lomInfo.Add_IntendedEndUserRole(IntendedEndUserRoleEnum.author);
                                break;

                            case level3_text:
                                lomInfo.Add_IntendedEndUserRole(IntendedEndUserRoleEnum.learner);
                                break;

                            case level4_text:
                                lomInfo.Add_IntendedEndUserRole(IntendedEndUserRoleEnum.manager);
                                break;
                        }
                    }
                }
            }
        }
    }
}