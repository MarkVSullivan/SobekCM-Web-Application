#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Resource_Object;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows entry of the aggregationPermissions for an item to be linked </summary>
    /// <remarks> This class extends the <see cref="multipleComboBox_Element"/> class. </remarks>
    public class Aggregations_Element : multipleComboBox_Element
    {
        /// <summary> Constructor for a new instance of the Aggregations_Element class </summary>
        public Aggregations_Element()
            : base("Aggregation", "collection")
        {
            Repeatable = true;
            Type = Element_Type.Aggregations;
            view_choices_string = String.Empty;

            boxes_per_line = 3;
            max_boxes = 9;
        }

        /// <summary> Sets the base url for the current request </summary>
        /// <param name="Base_URL"> Current Base URL for this request </param>
        public override void  Set_Base_URL(string Base_URL)
        {
            view_choices_string = "<a href=\"" + Base_URL + "l/internal/colls<%?URLOPTS%>\" title=\"View all collections\" target=\"_COLLECTIONLIST\"><img src=\"" + Base_URL + "design/skins/<%WEBSKIN%>/buttons/magnify.jpg\" /></a>";
        }

        /// <summary> Sets the list of all valid codes for this element from the main aggregation table </summary>
        /// <param name="codeManager"> Code manager with list of all aggregationPermissions </param>
        internal void Add_Codes(Aggregation_Code_Manager codeManager)
        {
            if (items.Count == 0)
            {
                SortedList<string, string> tempItemList = new SortedList<string, string>();
                ReadOnlyCollection<Item_Aggregation_Related_Aggregations> subcollections = codeManager.All_Aggregations;
                foreach (Item_Aggregation_Related_Aggregations thisAggr in subcollections)
                {
                    if (!tempItemList.ContainsKey(thisAggr.Code))
                    {
                        tempItemList.Add(thisAggr.Code, thisAggr.Code);
                    }
                }
                IList<string> keys = tempItemList.Keys;
                foreach (string thisKey in keys)
                {
                    items.Add(tempItemList[thisKey].ToUpper());
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
                const string defaultAcronym = "Select the collections to which this item should belong";
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

            List<string> codes = new List<string>();
            List<string> possibles = new List<string>();
            if (Bib.Behaviors.Aggregation_Count > 0)
            {
                codes.AddRange(Bib.Behaviors.Aggregation_Code_List);
                possibles.AddRange(Bib.Behaviors.Aggregation_Code_List);
            }

            // Check the user to see if this should be limited
            bool some_set_as_selectable = false;
            if (!Current_User.Is_Internal_User)
            {
                // Are there aggregationPermissions set aside for the user?
                List<User_Permissioned_Aggregation> allAggrs = Current_User.PermissionedAggregations;

                foreach( User_Permissioned_Aggregation thisAggr in allAggrs )
                {
                    if (thisAggr.CanSelect)
                    {
                        some_set_as_selectable = true;
                        if ((items.Contains(thisAggr.Code)) && (!possibles.Contains(thisAggr.Code)))
                            possibles.Add(thisAggr.Code);                           
                    }
                }
            }

            if (some_set_as_selectable)
            {
                render_helper(Output, codes, possibles, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
            }
            else
            {
                render_helper(Output, codes, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
            }
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears any preexisting alternate collections </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            Bib.Behaviors.Clear_Aggregations();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf(html_element_name.Replace("_", "")) == 0)
                {
                    string code = HttpContext.Current.Request.Form[thisKey].ToUpper();
                    Bib.Behaviors.Add_Aggregation(code);
                }
            }
        }
    }
}