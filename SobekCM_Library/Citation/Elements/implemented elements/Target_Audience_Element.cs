﻿#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Users;
using SobekCM.Resource_Object;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows entry of the target audience for an item </summary>
    /// <remarks> This class extends the <see cref="multipleTextBox_Element"/> class. </remarks>
    class Target_Audience_Element : multipleTextBox_Element
    {
        /// <summary> Constructor for a new instance of the Target_Audience_Element class </summary>
        public Target_Audience_Element()
            : base("Target Audience", "target_audience")
        {
            Repeatable = true;

            max_boxes = -1;
            boxes_per_line = 3;
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
                const string defaultAcronym = "Enter information about the target audience for this material.";
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

            List<string> audiences = new List<string>();
            if (Bib.Bib_Info.Target_Audiences_Count > 0)
            {
                audiences.AddRange(Bib.Bib_Info.Target_Audiences.Select(thisAudience => thisAudience.Audience));
            }
            render_helper(Output, new ReadOnlyCollection<string>(audiences), Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears any preexisting target audiences </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            Bib.Bib_Info.Clear_Target_Audiences();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string id = html_element_name.Replace("_", "");
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf(id) != 0) continue;

                string audience = HttpContext.Current.Request.Form[thisKey].Trim();
                string scheme = String.Empty;
                string audience_caps = audience.ToUpper();
                if ((audience_caps == "ADOLESCENT") || (audience_caps == "ADULT") || (audience_caps == "GENERAL") || (audience_caps == "PRIMARY") || (audience_caps == "PRE-ADOLESCENT") || (audience_caps == "JUVENILE") || (audience_caps == "PRESCHOOL") || ( audience_caps == "SPECIALIZED" ))
                {
                    scheme = "marctarget";
                }

                Bib.Bib_Info.Add_Target_Audience(audience, scheme);
            }  
        }
    }
}
