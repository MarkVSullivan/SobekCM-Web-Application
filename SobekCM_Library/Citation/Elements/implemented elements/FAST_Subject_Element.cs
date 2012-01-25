#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;
using SobekCM.Library.Application_State;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows simple entry of the FAST term subject keywords </summary>
    /// <remarks> This class extends the <see cref="textBox_ComboBox_Element"/> class. </remarks>
    public class FAST_Subject_Element: textBox_ComboBox_Element
    {
        /// <summary> Constructor for a new instance of the FAST_Subject_Element class </summary>
        public FAST_Subject_Element()
            : base("FAST Subject", "fastsubject")
        {
            Add_Select_Item("Genre", "genre");
            Add_Select_Item("Geographic", "geographic" );
            Add_Select_Item("Occupational", "occupational");
            Add_Select_Item("Temporal", "temporal");
            Add_Select_Item("Topical", "topic");


            Type = Element_Type.FAST_Subject;
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
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool isMozilla, StringBuilder popup_form_builder, User_Object Current_User, Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string defaultAcronym = "Enter a FAST subject keyword to describe this item.";
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


            render_helper(Output, new List<string>(), new List<string>(), Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears any preexisting series title and other titles </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Do nothing to prepare.. this is an input only element.. it moves up to standard subject area after entry
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string subject_text = String.Empty;

            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_text") == 0)
                {
                    subject_text = HttpContext.Current.Request.Form[thisKey];
                }

                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_select") == 0)
                {
                    string subject_type = HttpContext.Current.Request.Form[thisKey];
                    Subject_Info_Standard standardSubject = new Subject_Info_Standard {Authority = "fast"};

                    if (subject_text.Trim().Length > 0)
                    {
                        switch (subject_type)
                        {
                            case "genre":
                                standardSubject.Add_Genre( subject_type );
                                break;

                            case "geographic":
                                standardSubject.Add_Geographic( subject_type );
                                break;

                            case "occupational":
                                standardSubject.Add_Occupation( subject_type );
                                break;

                            case "temporal":
                                standardSubject.Add_Temporal( subject_type );
                                break;

                            case "topic":
                                standardSubject.Add_Topic( subject_type );
                                break;
                        }

                        if ((standardSubject.Topics_Count > 0) || (standardSubject.Genres_Count > 0) || (standardSubject.Geographics_Count > 0) || (standardSubject.Occupations_Count > 0) || (standardSubject.Temporals_Count > 0))
                            Bib.Bib_Info.Add_Subject(standardSubject);
                    }

                    subject_text = String.Empty;
                }
            }
        }
    }
}
