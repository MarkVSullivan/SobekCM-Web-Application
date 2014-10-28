#region Using directives

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Users;
using SobekCM.Resource_Object;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element displays a form to allow for complete entry of the serial hiearchy for an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    class Serial_Hierarchy_Panel_Element : abstract_Element
    {
        /// <summary> Constructor for a new instance of the Serial_Hierarchy_Panel_Element class </summary>
        public Serial_Hierarchy_Panel_Element()
        {
            Repeatable = false;
            Type = Element_Type.SerialHierarchy;
            Display_SubType = "panel";
            Title = "Serial Hierarchy";
            html_element_name = "panel_serial_hierarchy";
			help_page = "serialhierarchy2";
        }

        #region iElement Members

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
        /// <remarks> This element appends a popup form to the popup_form_builder</remarks>
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool IsMozilla, StringBuilder PopupFormBuilder, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL)
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string DEFAULT_ACRONYM = "Enter serial hierarchy information which explains how this volume related to the larger body of work.";
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

            Output.WriteLine("  <!-- " + Title + " Form Element -->");
            Output.WriteLine("  <tr>");
            Output.WriteLine("    <td style=\"width:" + LEFT_MARGIN + "px\">&nbsp;</td>");
            if (Acronym.Length > 0)
            {
                Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\"><acronym title=\"" + Acronym + "\">" + Translator.Get_Translation(Title, CurrentLanguage) + ":</acronym></a></td>");
            }
            else
            {
                Output.WriteLine("    <td class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\">" + Translator.Get_Translation(Title, CurrentLanguage) + ":</a></td>");
            }

            Output.WriteLine("    <td>");
            Output.WriteLine("      <table>");

            Output.WriteLine("    <tr><td>&nbsp;</td><td><span style=\"color:Gray;padding-left:55px;\">Display Text</span></td><td><span style=\"color:Gray;\"> &nbsp; Display Order</span></td></tr>");


            // Add the rows of enumeration data
            Output.WriteLine("    <tr><td style=\"width:100px\">Level 1:</td>");
            if (Bib.Behaviors.Serial_Info.Count > 0)
            {
                Output.WriteLine("<td><input type=\"text\" class=\"form_serialhierarchy_text_input sbk_Focusable\" id=\"form_serialhierarchy_enum1text\" name=\"form_serialhierarchy_enum1text\" value=\"" + HttpUtility.HtmlEncode(Bib.Behaviors.Serial_Info[0].Display) + "\" /></td>");
				Output.WriteLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_enum1order\" name=\"form_serialhierarchy_enum1order\" value=\"" + HttpUtility.HtmlEncode(Bib.Behaviors.Serial_Info[0].Order.ToString()) + "\" /></td></tr>");
            }
            else
            {
				Output.WriteLine("<td><input type=\"text\" class=\"form_serialhierarchy_text_input sbk_Focusable\" id=\"form_serialhierarchy_enum1text\" name=\"form_serialhierarchy_enum1text\" value=\"\" /></td>");
				Output.WriteLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_enum1order\" name=\"form_serialhierarchy_enum1order\" value=\"\" /></td></tr>");
            }

            Output.WriteLine("    <tr><td>Level 2:</td>");
            if (Bib.Behaviors.Serial_Info.Count > 1)
            {
				Output.WriteLine("<td><input type=\"text\" class=\"form_serialhierarchy_text_input sbk_Focusable\" id=\"form_serialhierarchy_enum2text\" name=\"form_serialhierarchy_enum2text\" value=\"" + HttpUtility.HtmlEncode(Bib.Behaviors.Serial_Info[1].Display) + "\"  /></td>");
				Output.WriteLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_enum2order\" name=\"form_serialhierarchy_enum2order\" value=\"" + HttpUtility.HtmlEncode(Bib.Behaviors.Serial_Info[1].Order.ToString()) + "\" /></td></tr>");
            }
            else
            {
				Output.WriteLine("<td><input type=\"text\" class=\"form_serialhierarchy_text_input sbk_Focusable\" id=\"form_serialhierarchy_enum2text\" name=\"form_serialhierarchy_enum2text\" value=\"\" /></td>");
				Output.WriteLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_enum2order\" name=\"form_serialhierarchy_enum2order\" value=\"\" /></td></tr>");
            }

            Output.WriteLine("    <tr><td>Level 3:</td>");
            if (Bib.Behaviors.Serial_Info.Count > 2)
            {
				Output.WriteLine("<td><input type=\"text\" class=\"form_serialhierarchy_text_input sbk_Focusable\" id=\"form_serialhierarchy_enum3text\" name=\"form_serialhierarchy_enum3text\" value=\"" + HttpUtility.HtmlEncode(Bib.Behaviors.Serial_Info[2].Display) + "\" /></td>");
				Output.WriteLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_enum3order\" name=\"form_serialhierarchy_enum3order\" value=\"" + HttpUtility.HtmlEncode(Bib.Behaviors.Serial_Info[2].Order.ToString()) + "\" /></td></tr>");
            }
            else
            {
				Output.WriteLine("<td><input type=\"text\" class=\"form_serialhierarchy_text_input sbk_Focusable\" id=\"form_serialhierarchy_enum3text\" name=\"form_serialhierarchy_enum3text\" value=\"\" /></td>");
				Output.WriteLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_enum3order\" name=\"form_serialhierarchy_enum3order\" value=\"\" /></td></tr>");
            }

            Output.WriteLine("      </table>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine();

            //// Determine which is primary, the enumeration or chronology.
            //bool enum_primary = true;
            //if (Bib.Serial_Info.Count > 0)
            //{
            //    if (Bib.Bib_Info.Series_Part_Info.Year == Bib.Serial_Info[0].Display)
            //    {
            //        enum_primary = false;
            //    }
            //}
            //else
            //{
            //    // If no default, set it by type
            //    if (Bib.Bib_Info.Type.Type.ToUpper().IndexOf("NEWSPAPER") >= 0)
            //    {
            //        enum_primary = false;
            //    }
            //}
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears any preexisting serial hierarchy information ( Bib.Serial_Info ) </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Delete existing serial information
            Bib.Behaviors.Serial_Info.Clear();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            // Get these values directly
            string enum1Text = HttpContext.Current.Request.Form["form_serialhierarchy_enum1text"].Trim();
            string enum1OrderString = HttpContext.Current.Request.Form["form_serialhierarchy_enum1order"].Trim();
            string enum2Text = HttpContext.Current.Request.Form["form_serialhierarchy_enum2text"].Trim();
            string enum2OrderString = HttpContext.Current.Request.Form["form_serialhierarchy_enum2order"].Trim();
            string enum3Text = HttpContext.Current.Request.Form["form_serialhierarchy_enum3text"].Trim();
            string enum3OrderString = HttpContext.Current.Request.Form["form_serialhierarchy_enum3order"].Trim();

            // Check to see if the title is in enum1text
            if (enum1Text == "[TITLE]")
            {
                enum1Text = Bib.Bib_Info.Main_Title.Title;
            }

            // Set the serial part information portion first (which contains both enum and chrono values)
            if (enum1Text.Length > 0)
            {
                int level = 1;
                int enum1Order = convert_string_to_int_safely(enum1OrderString);
                if ( enum1Order < 0 )
                {
                    enum1Order = convert_string_to_int_safely(enum1Text);
                }
                if ( enum1Order < 0 )
                {
                    enum1Order = convert_string_to_int_safely(Bib.VID.Replace("VID", ""));
                }
                if ( enum1Order >= 0 )
                {
                    Bib.Behaviors.Serial_Info.Add_Hierarchy(level++, enum1Order, enum1Text);

                    if (enum2Text.Length > 0)
                    {
                        int enum2Order = convert_string_to_int_safely(enum2OrderString);
                        if ( enum2Order < 0 )
                        {
                            enum2Order = convert_string_to_int_safely(enum2Text);
                        }
                        if ( enum2Order >= 0 )
                        {
                            Bib.Behaviors.Serial_Info.Add_Hierarchy(level++, enum2Order, enum2Text);

                            if (enum3Text.Length > 0)
                            {
                                int enum3Order = convert_string_to_int_safely(enum3OrderString);
                                if (enum3Order < 0)
                                {
                                    enum3Order = convert_string_to_int_safely(enum3Text);
                                }
                                if (enum3Order >= 0)
                                {
                                    Bib.Behaviors.Serial_Info.Add_Hierarchy(level, enum3Order, enum3Text);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        private static int convert_string_to_int_safely(string StringValue)
        {
            if (StringValue.Length == 0)
                return -1;

            if (StringValue.Any(ThisChar => !Char.IsNumber(ThisChar)))
            {
                return -1;
            }

            try
            {
                return Convert.ToInt16(StringValue);
            }
            catch
            {
                return -1;
            }
        }

        #region Methods Implementing the Abstract Methods from abstract_Element class

        /// <summary> Reads the inner data from the Template XML format </summary>
        /// <param name="XMLReader"> Current template xml configuration reader </param>
        /// <remarks> This procedure does not currently read any inner xml (not yet necessary) </remarks>
        protected override void Inner_Read_Data(XmlTextReader XMLReader)
        {
            // Do nothing
        }

        #endregion
    }
}