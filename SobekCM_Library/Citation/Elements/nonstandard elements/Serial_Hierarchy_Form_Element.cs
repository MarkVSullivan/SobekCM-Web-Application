#region Using directives

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using SobekCM.Core.Configuration;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element displays a form to allow for complete entry of the serial hiearchy for an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    class Serial_Hierarchy_Form_Element : abstract_Element
    {
        /// <summary> Constructor for a new instance of the Serial_Hierarchy_Form_Element class </summary>
        public Serial_Hierarchy_Form_Element()
        {
            Repeatable = false;
            Type = Element_Type.SerialHierarchy;
            Display_SubType = "form";
            Title = "Serial Hierarchy";
            html_element_name = "form_serial_hierarchy";
	        help_page = "serialhierarchy";
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
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool IsMozilla, StringBuilder PopupFormBuilder, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string defaultAcronym = "Enter serial hierarchy information which explains how this volume related to the larger body of work.";
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
            Output.WriteLine("        <tr>");
            Output.WriteLine("          <td>");
            Output.WriteLine("            <div id=\"" + html_element_name + "_div\">");
            string serial_hierarchy_string = show_hierarchy_value(Bib.Behaviors.Serial_Info);
            if (serial_hierarchy_string.Length == 0)
            {
                Output.WriteLine("              <a title=\"Click to edit the serial hierarchy\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_serial_hierarchy_term')\" onblur=\"return link_blurred2('form_serial_hierarchy_term')\" onkeypress=\"return popup_keypress_focus('form_serial_hierarchy', 'form_serialhierarchy_enum1text', '" + IsMozilla.ToString() + "' );\" onclick=\"return popup_focus('form_serial_hierarchy', 'form_serialhierarchy_enum1text' );\"><div class=\"form_linkline_empty form_serial_hierarchy_line\" id=\"form_serial_hierarchy_term\"><i>Empty Serial Hierarchy</i></div></a>");
            }
            else
            {
                Output.WriteLine("              <a title=\"Click to edit the serial hierarchy\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_serial_hierarchy_term')\" onblur=\"link_blurred2('form_serial_hierarchy_term')\" onkeypress=\"return popup_keypress_focus('form_serial_hierarchy', 'form_serialhierarchy_enum1text', '" + IsMozilla.ToString() + "' );\" onclick=\"return popup_focus('form_serial_hierarchy', 'form_serialhierarchy_enum1text' );\"><div class=\"form_linkline form_serial_hierarchy_line\" id=\"form_serial_hierarchy_term\">" + serial_hierarchy_string + "</div></a>");
            }
            Output.WriteLine("            </div>");
            Output.WriteLine("          </td>");
            Output.WriteLine("          <td style=\"vertical-align:bottom\" >");
            Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img class=\"help_button\" src=\"" + Base_URL + HELP_BUTTON_URL + "\" /></a>");
            Output.WriteLine("          </td>");
            Output.WriteLine("        </tr>");
            Output.WriteLine("      </table>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine();
            
            // Determine which is primary, the enumeration or chronology.
            bool enum_primary = true;
            if ( Bib.Behaviors.Serial_Info.Count > 0)
            {
                if ( Bib.Bib_Info.Series_Part_Info.Year == Bib.Behaviors.Serial_Info[0].Display)
                {
                    enum_primary = false;
                }
            }
            else
            {
                // If no default, set it by type
                if (Bib.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Newspaper )
                {
                    enum_primary = false;
                }
            }

            // Add the popup form
            PopupFormBuilder.AppendLine("<!-- Serial Hierarchy Form -->");
			PopupFormBuilder.AppendLine("<div class=\"serial_hierarchy_popup_div sbkMetadata_PopupDiv\" id=\"form_serial_hierarchy\" style=\"display:none;\">");
			PopupFormBuilder.AppendLine("  <div class=\"sbkMetadata_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left\">Edit Serial Hierarchy</td><td style=\"text-align:right\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" alt=\"HELP\" target=\"_" + html_element_name.ToUpper() + "\" >?</a> &nbsp; <a href=\"#template\" alt=\"CLOSE\" onclick=\"close_serial_hierarchy()\">X</a> &nbsp; </td></tr></table></div>");
            PopupFormBuilder.AppendLine("  <br />");
			PopupFormBuilder.AppendLine("  <table class=\"sbkMetadata_PopupTable\">");

            // Add the Enumeration Information title
            PopupFormBuilder.AppendLine("    <tr><td colspan=\"4\" class=\"SobekEditItemSectionTitle_first\" >Enumeration Information</td></tr>");

            // Add the enumeration as primary radio button
            PopupFormBuilder.Append("    <tr><td colspan=\"2\"> &nbsp; &nbsp; <input type=\"radio\" id=\"form_serialhierarchy_primary_enum\" name=\"form_serialhierarchy_primary\" value=\"enum\"");
            if (enum_primary)
                PopupFormBuilder.Append(" checked=\"checked\"");
            PopupFormBuilder.AppendLine(" onclick=\"focus_element( 'form_serialhierarchy_enum1text');\" /> <label for=\"form_serialhierarchy_primary_enum\">Primary</label> </td><td><span style=\"color:Gray;padding-left:55px;\">Display Text</span></td><td><span style=\"color:Gray;\"> &nbsp; Display Order</span></td></tr>");

            // determine the first enumeration data
            string enum1 = Bib.Bib_Info.Series_Part_Info.Enum1;
            if (Bib.Bib_Info.Series_Part_Info.Enum1.ToUpper().Replace(" ", "").Replace(".", "") == Bib.Bib_Info.Main_Title.Title.ToUpper().Replace(" ", "").Replace(".", ""))
                enum1 = "[TITLE]";

            // Add the rows of enumeration data
            PopupFormBuilder.Append("    <tr><td style=\"width:90px\">&nbsp;</td><td style=\"width:100px\">(Volume):</td>");
            PopupFormBuilder.Append("<td><input type=\"text\" class=\"form_serialhierarchy_text_input sbk_Focusable\" id=\"form_serialhierarchy_enum1text\" name=\"form_serialhierarchy_enum1text\" value=\"" + HttpUtility.HtmlEncode(enum1) + "\" ></td>");
            if ( Bib.Bib_Info.Series_Part_Info.Enum1_Index >= 0 )
				PopupFormBuilder.AppendLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_enum1order\" name=\"form_serialhierarchy_enum1order\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Series_Part_Info.Enum1_Index.ToString()) + "\" ></td></tr>");
            else
                PopupFormBuilder.AppendLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_enum1order\" name=\"form_serialhierarchy_enum1order\" value=\"\" ></td></tr>");

            PopupFormBuilder.Append("    <tr><td>&nbsp;</td><td>(Issue):</td>");
            PopupFormBuilder.Append("<td><input type=\"text\" class=\"form_serialhierarchy_text_input sbk_Focusable\" id=\"form_serialhierarchy_enum2text\" name=\"form_serialhierarchy_enum2text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Series_Part_Info.Enum2) + "\" ></td>");
            if (Bib.Bib_Info.Series_Part_Info.Enum2_Index >= 0)
				PopupFormBuilder.AppendLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_enum2order\" name=\"form_serialhierarchy_enum2order\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Series_Part_Info.Enum2_Index.ToString()) + "\" ></td></tr>");
            else
				PopupFormBuilder.AppendLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_enum2order\" name=\"form_serialhierarchy_enum2order\" value=\"\" ></td></tr>");

            PopupFormBuilder.Append("    <tr><td>&nbsp;</td><td>(Part):</td>");
			PopupFormBuilder.Append("<td><input type=\"text\" class=\"form_serialhierarchy_text_input sbk_Focusable\" id=\"form_serialhierarchy_enum3text\" name=\"form_serialhierarchy_enum3text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Series_Part_Info.Enum3) + "\" ></td>");
            if (Bib.Bib_Info.Series_Part_Info.Enum3_Index >= 0)
				PopupFormBuilder.AppendLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_enum3order\" name=\"form_serialhierarchy_enum3order\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Series_Part_Info.Enum3_Index.ToString()) + "\" ></td></tr>");
            else
				PopupFormBuilder.AppendLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_enum3order\" name=\"form_serialhierarchy_enum3order\" value=\"\" ></td></tr>");

            // Add the Chronological Information title
			PopupFormBuilder.AppendLine("    <tr><td colspan=\"4\" class=\"SobekEditItemSectionTitle sbk_Focusable\" >Chronological Information</td></tr>");

            // Add the chronology as primary radio button
            PopupFormBuilder.Append("    <tr><td colspan=\"2\"> &nbsp; &nbsp; <input type=\"radio\" id=\"form_serialhierarchy_primary_chrono\" name=\"form_serialhierarchy_primary\" value=\"chrono\"");
            if (!enum_primary)
                PopupFormBuilder.Append(" checked=\"checked\"");
            PopupFormBuilder.AppendLine(" onclick=\"focus_element( 'form_serialhierarchy_chrono1text');\" /> <label for=\"form_serialhierarchy_primary_chrono\">Primary</label> </td><td><span style=\"color:Gray;padding-left:55px;\">Display Text</span></td><td><span style=\"color:Gray;\"> &nbsp; Display Order</span></td></tr>");

            // Add the rows of chronological data
            PopupFormBuilder.Append("    <tr><td>&nbsp;</td><td>Year:</td>");
			PopupFormBuilder.Append("<td><input type=\"text\" class=\"form_serialhierarchy_text_input sbk_Focusable\" id=\"form_serialhierarchy_chrono1text\" name=\"form_serialhierarchy_chrono1text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Series_Part_Info.Year) + "\" ></td>");
            if (Bib.Bib_Info.Series_Part_Info.Year_Index >= 0)
				PopupFormBuilder.AppendLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_chrono1order\" name=\"form_serialhierarchy_chrono1order\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Series_Part_Info.Year_Index.ToString()) + "\" ></td></tr>");
            else
				PopupFormBuilder.AppendLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_chrono1order\" name=\"form_serialhierarchy_chrono1order\" value=\"\" ></td></tr>");

            PopupFormBuilder.Append("    <tr><td>&nbsp;</td><td>(Month):</td>");
			PopupFormBuilder.Append("<td><input type=\"text\" class=\"form_serialhierarchy_text_input sbk_Focusable\" id=\"form_serialhierarchy_chrono2text\" name=\"form_serialhierarchy_chrono2text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Series_Part_Info.Month) + "\" ></td>");
            if (Bib.Bib_Info.Series_Part_Info.Month_Index >= 0)
				PopupFormBuilder.AppendLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_chrono2order\" name=\"form_serialhierarchy_chrono2order\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Series_Part_Info.Month_Index.ToString()) + "\" ></td></tr>");
            else
				PopupFormBuilder.AppendLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_chrono2order\" name=\"form_serialhierarchy_chrono2order\" value=\"\" ></td></tr>");

            PopupFormBuilder.Append("    <tr><td>&nbsp;</td><td>(Day):</td>");
			PopupFormBuilder.Append("<td><input type=\"text\" class=\"form_serialhierarchy_text_input sbk_Focusable\" id=\"form_serialhierarchy_chrono3text\" name=\"form_serialhierarchy_chrono3text\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Series_Part_Info.Day) + "\" ></td>");
            if (Bib.Bib_Info.Series_Part_Info.Day_Index >= 0)
				PopupFormBuilder.AppendLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_chrono3order\" name=\"form_serialhierarchy_chrono3order\" value=\"" + HttpUtility.HtmlEncode(Bib.Bib_Info.Series_Part_Info.Day_Index.ToString()) + "\" ></td></tr>");
            else
				PopupFormBuilder.AppendLine("<td><input type=\"text\" class=\"form_serialhierarchy_order_input sbk_Focusable\" id=\"form_serialhierarchy_chrono3order\" name=\"form_serialhierarchy_chrono3order\" value=\"\" ></td></tr>");


			// Finish the popup form and add the CLOSE button
			PopupFormBuilder.AppendLine("    <tr style=\"height:35px; text-align: center; vertical-align: bottom;\">");
			PopupFormBuilder.AppendLine("      <td colspan=\"3\"><button title=\"Close\" class=\"sbkMetadata_RoundButton\" onclick=\"return close_serial_hierarchy();\">CLOSE</button></td>");
			PopupFormBuilder.AppendLine("    </tr>");
			PopupFormBuilder.AppendLine("  </table>");
			PopupFormBuilder.AppendLine("</div>");
			PopupFormBuilder.AppendLine();
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
            string primary = HttpContext.Current.Request.Form["form_serialhierarchy_primary"].Trim();
            string enum1Text = HttpContext.Current.Request.Form["form_serialhierarchy_enum1text"].Trim();
            string enum1OrderString = HttpContext.Current.Request.Form["form_serialhierarchy_enum1order"].Trim();
            string enum2Text = HttpContext.Current.Request.Form["form_serialhierarchy_enum2text"].Trim();
            string enum2OrderString = HttpContext.Current.Request.Form["form_serialhierarchy_enum2order"].Trim();
            string enum3Text = HttpContext.Current.Request.Form["form_serialhierarchy_enum3text"].Trim();
            string enum3OrderString = HttpContext.Current.Request.Form["form_serialhierarchy_enum3order"].Trim();
            string chrono1Text = HttpContext.Current.Request.Form["form_serialhierarchy_chrono1text"].Trim();
            string chrono1OrderString = HttpContext.Current.Request.Form["form_serialhierarchy_chrono1order"].Trim();
            string chrono2Text = HttpContext.Current.Request.Form["form_serialhierarchy_chrono2text"].Trim();
            string chrono2OrderString = HttpContext.Current.Request.Form["form_serialhierarchy_chrono2order"].Trim();
            string chrono3Text = HttpContext.Current.Request.Form["form_serialhierarchy_chrono3text"].Trim();
            string chrono3OrderString = HttpContext.Current.Request.Form["form_serialhierarchy_chrono3order"].Trim();

            // Check to see if the title is in enum1text
            if (enum1Text == "[TITLE]")
            {
                enum1Text = Bib.Bib_Info.Main_Title.Title;
            }

            // Set the serial part information portion first (which contains both enum and chrono values)
            Bib.Bib_Info.Series_Part_Info.Enum1 = enum1Text;
            Bib.Bib_Info.Series_Part_Info.Enum1_Index = convert_string_to_int_safely ( enum1OrderString );
            Bib.Bib_Info.Series_Part_Info.Enum2 = enum2Text;
            Bib.Bib_Info.Series_Part_Info.Enum2_Index = convert_string_to_int_safely(enum2OrderString);
            Bib.Bib_Info.Series_Part_Info.Enum3 = enum3Text;
            Bib.Bib_Info.Series_Part_Info.Enum3_Index = convert_string_to_int_safely(enum3OrderString);
            Bib.Bib_Info.Series_Part_Info.Year = chrono1Text;
            Bib.Bib_Info.Series_Part_Info.Year_Index = convert_string_to_int_safely(chrono1OrderString);
            Bib.Bib_Info.Series_Part_Info.Month = chrono2Text;
            Bib.Bib_Info.Series_Part_Info.Month_Index = convert_string_to_int_safely(chrono2OrderString);
            Bib.Bib_Info.Series_Part_Info.Day = chrono3Text;
            Bib.Bib_Info.Series_Part_Info.Day_Index = convert_string_to_int_safely(chrono3OrderString);

            // Set some indexes, if they were left empty
            if ((Bib.Bib_Info.Series_Part_Info.Enum1.Length > 0) && (enum1OrderString.Length == 0))
            {
                Bib.Bib_Info.Series_Part_Info.Enum1_Index = convert_string_to_int_safely(Bib.VID.Replace("VID", ""));
            }
            if ((Bib.Bib_Info.Series_Part_Info.Year.Length > 0) && (chrono1OrderString.Length == 0))
            {
                Bib.Bib_Info.Series_Part_Info.Year_Index = convert_string_to_int_safely(Bib.Bib_Info.Series_Part_Info.Year);
            }
            if ((Bib.Bib_Info.Series_Part_Info.Month.Length > 0) && (chrono2OrderString.Length == 0))
            {
                Bib.Bib_Info.Series_Part_Info.Month_Index = convert_string_to_int_safely(Bib.Bib_Info.Series_Part_Info.Month);
                string month_upper = Bib.Bib_Info.Series_Part_Info.Month.ToUpper();
                if ((Bib.Bib_Info.Series_Part_Info.Month_Index < 0) && (month_upper.IndexOf("JAN") >= 0))
                {
                    Bib.Bib_Info.Series_Part_Info.Month_Index = 1;
                }
                if ((Bib.Bib_Info.Series_Part_Info.Month_Index < 0) && (month_upper.IndexOf("FEB") >= 0))
                {
                    Bib.Bib_Info.Series_Part_Info.Month_Index = 2;
                }
                if ((Bib.Bib_Info.Series_Part_Info.Month_Index < 0) && (month_upper.IndexOf("MAR") >= 0))
                {
                    Bib.Bib_Info.Series_Part_Info.Month_Index = 3;
                }
                if ((Bib.Bib_Info.Series_Part_Info.Month_Index < 0) && (month_upper.IndexOf("APR") >= 0))
                {
                    Bib.Bib_Info.Series_Part_Info.Month_Index = 4;
                }
                if ((Bib.Bib_Info.Series_Part_Info.Month_Index < 0) && (month_upper.IndexOf("MAY") >= 0))
                {
                    Bib.Bib_Info.Series_Part_Info.Month_Index = 5;
                }
                if ((Bib.Bib_Info.Series_Part_Info.Month_Index < 0) && (month_upper.IndexOf("JUN") >= 0))
                {
                    Bib.Bib_Info.Series_Part_Info.Month_Index = 6;
                }
                if ((Bib.Bib_Info.Series_Part_Info.Month_Index < 0) && (month_upper.IndexOf("JUL") >= 0))
                {
                    Bib.Bib_Info.Series_Part_Info.Month_Index = 7;
                }
                if ((Bib.Bib_Info.Series_Part_Info.Month_Index < 0) && (month_upper.IndexOf("AUG") >= 0))
                {
                    Bib.Bib_Info.Series_Part_Info.Month_Index = 8;
                }
                if ((Bib.Bib_Info.Series_Part_Info.Month_Index < 0) && (month_upper.IndexOf("SEP") >= 0))
                {
                    Bib.Bib_Info.Series_Part_Info.Month_Index = 9;
                }
                if ((Bib.Bib_Info.Series_Part_Info.Month_Index < 0) && (month_upper.IndexOf("OCT") >= 0))
                {
                    Bib.Bib_Info.Series_Part_Info.Month_Index = 10;
                }
                if ((Bib.Bib_Info.Series_Part_Info.Month_Index < 0) && (month_upper.IndexOf("NOV") >= 0))
                {
                    Bib.Bib_Info.Series_Part_Info.Month_Index = 11;
                }
                if ((Bib.Bib_Info.Series_Part_Info.Month_Index < 0) && (month_upper.IndexOf("DEC") >= 0))
                {
                    Bib.Bib_Info.Series_Part_Info.Month_Index = 12;
                }
            }
            if ((Bib.Bib_Info.Series_Part_Info.Day.Length > 0) && (chrono3OrderString.Length == 0))
            {
                Bib.Bib_Info.Series_Part_Info.Day_Index = convert_string_to_int_safely(Bib.Bib_Info.Series_Part_Info.Day);
            }

            // Now, set the currently in use serial information
            int level = 1;
            if (primary == "enum")
            {
                if (Bib.Bib_Info.Series_Part_Info.Enum1.Length > 0)
                    Bib.Behaviors.Serial_Info.Add_Hierarchy(level++, Bib.Bib_Info.Series_Part_Info.Enum1_Index, Bib.Bib_Info.Series_Part_Info.Enum1);

                if (Bib.Bib_Info.Series_Part_Info.Enum2.Length > 0)
                    Bib.Behaviors.Serial_Info.Add_Hierarchy(level++, Bib.Bib_Info.Series_Part_Info.Enum2_Index, Bib.Bib_Info.Series_Part_Info.Enum2);

                if (Bib.Bib_Info.Series_Part_Info.Enum3.Length > 0)
                    Bib.Behaviors.Serial_Info.Add_Hierarchy(level, Bib.Bib_Info.Series_Part_Info.Enum3_Index, Bib.Bib_Info.Series_Part_Info.Enum3);
            }
            else
            {
                if (Bib.Bib_Info.Series_Part_Info.Year.Length > 0)
                    Bib.Behaviors.Serial_Info.Add_Hierarchy(level++, Bib.Bib_Info.Series_Part_Info.Year_Index, Bib.Bib_Info.Series_Part_Info.Year);

                if (Bib.Bib_Info.Series_Part_Info.Month.Length > 0)
                    Bib.Behaviors.Serial_Info.Add_Hierarchy(level++, Bib.Bib_Info.Series_Part_Info.Month_Index, Bib.Bib_Info.Series_Part_Info.Month);

                if (Bib.Bib_Info.Series_Part_Info.Day.Length > 0)
                    Bib.Behaviors.Serial_Info.Add_Hierarchy(level, Bib.Bib_Info.Series_Part_Info.Day_Index, Bib.Bib_Info.Series_Part_Info.Day);
            }
        }

        #endregion

        private static string show_hierarchy_value( Serial_Info serialInfo )
        {
            StringBuilder builder = new StringBuilder();
            if ((serialInfo != null) && (serialInfo.Count > 0))
            {
                builder.Append(serialInfo[0].Display + " (" + serialInfo[0].Order + ")");
            }
            if ((serialInfo != null) && (serialInfo.Count > 1))
            {
                if (builder.Length > 0)
                    builder.Append(" -- ");
                builder.Append(serialInfo[1].Display + " (" + serialInfo[1].Order + ")");
            }
            if ((serialInfo != null) && (serialInfo.Count > 2))
            {
                if (builder.Length > 0)
                    builder.Append(" -- ");
                builder.Append(serialInfo[2].Display + " (" + serialInfo[2].Order + ")");
            }

            return builder.ToString();
        }

        private static int convert_string_to_int_safely(string string_value)
        {
            if ( string_value.Length == 0 )
                return -1;

            if (string_value.Any(thisChar => !Char.IsNumber(thisChar)))
            {
                return -1;
            }

            try
            {
                return Convert.ToInt16(string_value);
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



