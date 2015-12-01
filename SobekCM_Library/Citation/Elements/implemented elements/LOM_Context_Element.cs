#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    /// <summary> Element allows entry of the learning object metadata context field </summary>
    /// <remarks> This class extends the <see cref="comboBox_TextBox_Element"/> class. </remarks>
    public class LOM_Context_Element : comboBox_TextBox_Element
    {
        /// <summary> Constructor for a new instance of the LOM_Context_Element class </summary>
        public LOM_Context_Element() : base("Context", "lom_context")
        {
            Repeatable = false;
            possible_select_items.Clear();
            possible_select_items.Add("Group");
            Type = Element_Type.LOM_Context;
            second_label = String.Empty;
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
                const string defaultAcronym = " Principal environment within which the learning and use of the learning object is intended to take place.";
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

            // Start the lists to get the current values
            List<string> type = new List<string>();
            List<string> levels = new List<string>();

            // Try to get any existing learning object metadata module
            LearningObjectMetadata lomInfo = Bib.Get_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY) as LearningObjectMetadata;
            if (lomInfo != null)
            {
                foreach (LOM_VocabularyState thisType in lomInfo.Contexts)
                {
                    if (thisType.Value.Trim().Length > 0)
                    {
                        type.Add(thisType.Source);
                        levels.Add(thisType.Value);
                    }
                }
            }

            if (type.Count == 0)
            {
                render_helper(Output, String.Empty, String.Empty, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL, false);
            }
            else
            {
                render_helper(Output, type, levels, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
            }
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears the list of contexts linked to this item </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Try to get any existing learning object metadata module
            LearningObjectMetadata lomInfo = Bib.Get_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY) as LearningObjectMetadata;
            if (lomInfo != null)
                lomInfo.Clear_Contexts();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            // Try to get any existing learning object metadata module
            LearningObjectMetadata lomInfo = Bib.Get_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY) as LearningObjectMetadata;

            // Pull the standard values
            NameValueCollection form = HttpContext.Current.Request.Form;

            foreach (string thisKey in form.AllKeys)
            {
                if (thisKey.IndexOf("lomcontext_select") == 0)
                {
                    string diff = thisKey.Replace("lomcontext_select", "");
                    string select_value = form[thisKey];
                    string text_value = form["lomcontext_text" + diff];

                    if ((select_value.Length > 0) && (text_value.Length > 0))
                    {
                        // There is a value, so ensure learning object metadata does exist
                        if (lomInfo == null)
                        {
                            lomInfo = new LearningObjectMetadata();
                            Bib.Add_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY, lomInfo);
                        }

                        // Add the value
                        lomInfo.Add_Context(text_value, select_value);
                    }
                }
            }
        }

    }
}
