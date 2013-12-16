#region Using directives

using System.Collections.Generic;
using System.IO;
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
    /// <summary> Element allows entry of the classifications ( classifications and authority) for an item </summary>
    /// <remarks> This class extends the <see cref="textBox_ComboBox_Element"/> class. </remarks>
    public class Classification_Element : textBox_ComboBox_Element
    {
        /// <summary> Constructor for a new instance of the Classification_Element class </summary>
        public Classification_Element()
            : base("Classification", "classification")
        {
            second_label = "Authority";
            Repeatable = true;
            Type = Element_Type.Classification;

            Add_Select_Item("", "");            
            Add_Select_Item("CANDOCS", "candocs");
            Add_Select_Item("DDC", "ddc");
            Add_Select_Item("LCC", "lcc");            
            Add_Select_Item("NLM", "nlm");
            Add_Select_Item("SUDOCS", "sudocs");
            Add_Select_Item("UDC", "udc");
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
                const string defaultAcronym = "Classification or call number for this item, usually associated with an authority.";
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
            if (Bib.Bib_Info.Classifications_Count > 0)
            {
                foreach (Classification_Info thisClassification in Bib.Bib_Info.Classifications)
                {
                    terms.Add(thisClassification.Classification);
                    schemes.Add(thisClassification.Authority.ToUpper());
                }
            }

            render_helper(Output, terms, schemes, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This does not do anything because this form element does no expose a couple rarely used subelements.   An attempt is made
        /// to keep those subelements if no change was done on the classification and authority. </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Do nothing
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            // Get the list of classifications
            List<Classification_Info> workableList = new List<Classification_Info>();
            if (Bib.Bib_Info.Classifications_Count > 0)
            {
                workableList.AddRange(Bib.Bib_Info.Classifications);
            }

            // Collect all the strings from the form
            Dictionary<string, string> classifications = new Dictionary<string, string>();
            Dictionary<string, string> authorities = new Dictionary<string, string>();
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_text") == 0)
                {
                    string term = HttpContext.Current.Request.Form[thisKey];
                    string index = thisKey.Replace(html_element_name.Replace("_", "") + "_text", "");
                    classifications[index] = term;
                }

                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_select") == 0)
                {
                    string scheme = HttpContext.Current.Request.Form[thisKey];
                    string index = thisKey.Replace(html_element_name.Replace("_", "") + "_select", "");
                    authorities[index] = scheme.ToLower();
                }
            }

            // Step through and add each classification
            foreach (string index in classifications.Keys)
            {
                Classification_Info newClassification = new Classification_Info
                                                            {Classification = classifications[index]};
                if (authorities.ContainsKey(index))
                {
                    newClassification.Authority = authorities[index];
                }

                // Was there a match already?
                bool found = false;
                foreach (Classification_Info thisClassification in workableList)
                {
                    if ((newClassification.Classification.Trim() == thisClassification.Classification.Trim()) && (newClassification.Authority == thisClassification.Authority.Trim()))
                    {
                        newClassification = thisClassification;
                        found = true;
                        break;
                    }
                }

                // If found, remove from existing list
                if (found)
                    workableList.Remove(newClassification);

                // Add to bib
                Bib.Bib_Info.Add_Classification(newClassification);
            }

            // Remove any remaining classifications
            foreach (Classification_Info deleteClassification in workableList)
            {
                Bib.Bib_Info.Remove_Classification(deleteClassification);
            }
        }
    }
}
