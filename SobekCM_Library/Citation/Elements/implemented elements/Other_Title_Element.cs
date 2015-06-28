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
    /// <summary> Element allows simple entry of the other title(s) for an item, including title type </summary>
    /// <remarks> This class extends the <see cref="textBox_ComboBox_Element"/> class. </remarks>
    public class Other_Title_Element : textBox_ComboBox_Element
    {
        /// <summary> Constructor for a new instance of the Other_Title_Element class </summary>
        public Other_Title_Element() : base( "Other Titles", "othertitle")
        {
            Add_Select_Item("Abbreviated Title", "abbreviated" );
            Add_Select_Item("Alternative Title", "alternate");
            Add_Select_Item("Series Title", "series");
            Add_Select_Item("Subtitle", "subtitle");
            Add_Select_Item("Translated Title", "translated");
            Add_Select_Item("Uniform Title", "uniform");

            Type = Element_Type.Title_Other;
            Repeatable = true;
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
                const string defaultAcronym = "Enter any other titles which relate to this material";
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

            List<string> titles = new List<string>();
            List<string> types = new List<string>();

            if (Bib.Bib_Info.Main_Title.Subtitle.Length > 0)
            {
                titles.Add(Bib.Bib_Info.Main_Title.Subtitle);
                types.Add("Subtitle");
                   
            }

            if (( Bib.Bib_Info.hasSeriesTitle ) && ( Bib.Bib_Info.SeriesTitle.Title.Length > 0))
            {
                titles.Add(Bib.Bib_Info.SeriesTitle.ToString());
                types.Add("Series Title");
            }

            if (Bib.Bib_Info.Other_Titles_Count > 0)
            {
                foreach (Title_Info thisTitle in Bib.Bib_Info.Other_Titles.Where(thisTitle => thisTitle.Title.Length > 0))
                {
                    titles.Add(thisTitle.Title);
                    switch (thisTitle.Title_Type)
                    {
                        case Title_Type_Enum.Abbreviated:
                            types.Add("Abbreviated Title");
                            break;

                        case Title_Type_Enum.Alternative:
                            types.Add("Alternative Title");
                            break;

                        case Title_Type_Enum.Translated:
                            types.Add("Translated Title");
                            break;

                        case Title_Type_Enum.Uniform:
                            types.Add("Uniform Title");
                            break;

                        default:
                            types.Add(String.Empty);
                            break;
                    }
                }
            }

            render_helper(Output, titles, types, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears any preexisting series title and other titles </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Clear the other titles
            if ( Bib.Bib_Info.hasSeriesTitle ) 
                Bib.Bib_Info.SeriesTitle.Clear();
            Bib.Bib_Info.Clear_Other_Titles();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string title_text = String.Empty;

            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_text") == 0)
                {
                    title_text = HttpContext.Current.Request.Form[thisKey];
                }

                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_select") == 0)
                {
                    string title_type = HttpContext.Current.Request.Form[thisKey];

                    if (title_text.Trim().Length > 0)
                    {
                        switch (title_type)
                        {
                            case "abbreviated":
                                Bib.Bib_Info.Add_Other_Title(title_text, Title_Type_Enum.Abbreviated);
                                break;

                            case "alternate":
                                Bib.Bib_Info.Add_Other_Title(title_text, Title_Type_Enum.Alternative);
                                break;

                            case "translated":
                                Bib.Bib_Info.Add_Other_Title(title_text, Title_Type_Enum.Translated);
                                break;

                            case "uniform":
                                Bib.Bib_Info.Add_Other_Title(title_text, Title_Type_Enum.Uniform);
                                break;

                            case "series":
                                Bib.Bib_Info.SeriesTitle.Title = title_text;
                                break;

                            case "subtitle":
                                Bib.Bib_Info.Main_Title.Subtitle = title_text;
                                break;
                        }
                    }
                    else
                    {
                        // Accept an empty subtitle here
                        if (title_type == "subtitle")
                        {
                            Bib.Bib_Info.Main_Title.Subtitle = string.Empty;
                        }
                    }

                    title_text = String.Empty;
                }
            }
        }
    }
}
