#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Core.Aggregations;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Users;
using SobekCM.Resource_Object;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows entry of the source institution (code and statement) for an item </summary>
    /// <remarks> This class extends the <see cref="comboBox_TextBox_Element"/> class. </remarks>
    public class Source_Element : comboBox_TextBox_Element
    {
        private Dictionary<string, string> codeToNameDictionary;

        /// <summary> Constructor for a new instance of the Source_Element class </summary>
        public Source_Element()
            : base("Source Institution", "source")
        {
            Repeatable = false;
            possible_select_items.Add("");

            clear_textbox_on_combobox_change = true;
        }

        /// <summary> Sets the list of all valid codes for this element from the main aggregation table </summary>
        /// <param name="CodeManager"> Code manager with list of all aggregationPermissions </param>
        internal void Add_Codes(Aggregation_Code_Manager CodeManager)
        {
            codeToNameDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (possible_select_items.Count <= 2)
            {
                SortedList<string, string> tempItemList = new SortedList<string, string>();
                foreach (string thisType in CodeManager.All_Types)
                {
                    if (thisType.IndexOf("Institution") >= 0)
                    {
                        ReadOnlyCollection<Item_Aggregation_Related_Aggregations> matchingAggr = CodeManager.Aggregations_By_Type(thisType);
                        foreach (Item_Aggregation_Related_Aggregations thisAggr in matchingAggr)
                        {
                            if (thisAggr.Code.Length > 1)
                            {
                                if ((thisAggr.Code[0] == 'i') || (thisAggr.Code[0] == 'I'))
                                {
                                    if (!tempItemList.ContainsKey(thisAggr.Code.Substring(1)))
                                    {
                                        codeToNameDictionary[thisAggr.Code.Substring(1).ToUpper()] = thisAggr.Name;
                                        tempItemList.Add(thisAggr.Code.Substring(1), thisAggr.Code.Substring(1));
                                    }
                                }
                                else
                                {
                                    if (!tempItemList.ContainsKey(thisAggr.Code))
                                    {
                                        codeToNameDictionary[thisAggr.Code.ToUpper()] = thisAggr.Name;
                                        tempItemList.Add(thisAggr.Code, thisAggr.Code);
                                    }
                                }
                            }
                        }
                    }
                }

                IList<string> keys = tempItemList.Keys;
                foreach (string thisKey in keys)
                {
                    possible_select_items.Add(tempItemList[thisKey].ToUpper());
                    if (codeToNameDictionary.ContainsKey(thisKey))
                    {
                        Add_Code_Statement_Link(thisKey, codeToNameDictionary[thisKey]);
                    }
                }


            }
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
                const string DEFAULT_ACRONYM = "Enter information about the institution which created the digital manifestation or entered this bibliographic data.";
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

            // Check the user to see if this should be limited
            bool some_set_as_selectable = false;
            List<string> possibles = new List<string> {Bib.Bib_Info.Source.Code.ToUpper()};
            if ((!Current_User.Is_Internal_User) && (Current_User.PermissionedAggregations != null ))
            {
                // Are there aggregationPermissions set aside for the user?
                List<User_Permissioned_Aggregation> allAggrs = Current_User.PermissionedAggregations;

                foreach (User_Permissioned_Aggregation thisAggr in allAggrs.Where(ThisAggr => ThisAggr.CanSelect))
                {
                    some_set_as_selectable = true;
                    string code = thisAggr.Code.ToUpper();
                    if ((code.Length > 1) && (code[0] == 'I'))
                        code = code.Substring(1);
                    if ((possible_select_items.Contains(code)) && (!possibles.Contains(code)))
                        possibles.Add(code);
                }
            }

            // Determine the current value to show
            string source_code = Bib.Bib_Info.Source.Code.ToUpper();

            // Render this
            if (some_set_as_selectable)
            {
                render_helper(Output, source_code, possibles, Bib.Bib_Info.Source.Statement, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL, false);
            }
            else
            {
                render_helper(Output, source_code, Bib.Bib_Info.Source.Statement, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
            }
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This does nothing since there is only one source institution </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Do nothing since there is only one holding location
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_select") == 0)
                {
                    string thisValue = HttpContext.Current.Request.Form[thisKey].ToUpper();

                    // Get rid of the institution name at the end of the value
                    if (thisValue.IndexOf("|") > 0)
                        thisValue = thisValue.Substring(0, thisValue.IndexOf("|"));

                    Bib.Bib_Info.Source.Code = thisValue;
                }

                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_text") == 0)
                {
                    string temp = HttpContext.Current.Request.Form[thisKey];
                    if ((temp.Trim().Length == 0) && (Bib.Bib_Info.Source.Code.Length > 0))
                    {
                        if ((codeToNameDictionary != null) && (codeToNameDictionary.ContainsKey(Bib.Bib_Info.Source.Code)))
                        {
                            Bib.Bib_Info.Source.Statement = codeToNameDictionary[Bib.Bib_Info.Source.Code];
                        }
                    }
                    else
                    {
                        Bib.Bib_Info.Source.Statement = temp;
                    }
                }
            }
        }

        /// <summary> Saves the constants to the bib id </summary>
        /// <param name="Bib"> Object into which to save this element's constant data </param>
        public override void Save_Constant_To_Bib(SobekCM_Item Bib)
        {
            if ((default_codes.Count > 0 ) || ( default_values.Count > 0 ))
            {
                if ((default_codes.Count > 0) && (default_codes[0].Length > 0))
                    Bib.Bib_Info.Source.Code = default_codes[0];
                if ((default_values.Count > 0) && (default_values[0].Length > 0))
                    Bib.Bib_Info.Source.Statement = default_values[0];
            }
        }
    }
}
