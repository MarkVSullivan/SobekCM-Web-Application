#region Using directives

using System;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Users;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element displays a form to allow for complete entry of related items to an item </summary>
    /// <remarks> This class implements the <see cref="iElement"/> interface and extends the <see cref="abstract_Element"/> class. </remarks>
    class Related_Item_Form_Element : abstract_Element
    {
        /// <summary> Constructor for a new instance of the Related_Item_Form_Element class </summary>
        public Related_Item_Form_Element()
        {
            Repeatable = true;
            Title = "Related Item";
            html_element_name = "form_related_item";
	        help_page = "relateditem";
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
                const string DEFAULT_ACRONYM = "Enter information about any related items here.";
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

            // Make sure there is at least one related item
            if (Bib.Bib_Info.RelatedItems_Count == 0)
                Bib.Bib_Info.Add_Related_Item(new Related_Item_Info());
 
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table>");
            Output.WriteLine("        <tr>");
            Output.WriteLine("          <td>");
            Output.Write("      <div id=\"" + html_element_name + "_div\">");
            int item_index = 1;
            if (Bib.Bib_Info.RelatedItems_Count > 0)
            {
                foreach (Related_Item_Info thisItem in Bib.Bib_Info.RelatedItems)
                {
                    // Add this related item links
                    if ((thisItem.hasMainTitle) && (thisItem.Main_Title.Title.Length > 0))
                    {
                        Output.Write("\n        <a title=\"Click to edit this related item\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_related_item_term_" + item_index + "')\" onblur=\"link_blurred2('form_related_item_term_" + item_index + "')\" onkeypress=\"return popup_keypress_focus('form_related_item_" + item_index + "', 'form_relateditem_title_" + item_index + "', '" + IsMozilla.ToString() + "' );\" onclick=\"return popup_focus('form_related_item_" + item_index + "', 'form_relateditem_title_" + item_index + "' );\"><div class=\"form_linkline form_related_item_line\" id=\"form_related_item_term_" + item_index + "\">");

                        if (thisItem.URL_Display_Label.Length > 0)
                        {
                            Output.Write("( <i>" + thisItem.URL_Display_Label + "</i> ) " + thisItem.Main_Title.Title);
                        }
                        else
                        {
                            string relation = String.Empty;
                            switch (thisItem.Relationship)
                            {
                                case Related_Item_Type_Enum.Succeeding:
                                    relation = "( <i>Succeeded by</i> ) ";
                                    break;

                                case Related_Item_Type_Enum.OtherVersion:
                                    relation = "( <i>Other Version</i> ) ";
                                    break;

                                case Related_Item_Type_Enum.OtherFormat:
                                    relation = "( <i>Other Format</i> ) ";
                                    break;

                                case Related_Item_Type_Enum.Preceding:
                                    relation = "( <i>Preceded by</i> ) ";
                                    break;

                                case Related_Item_Type_Enum.Host:
                                    relation = "( <i>Host</i> ) ";
                                    break;
                            }
                            Output.Write(relation + thisItem.Main_Title.Title);
                        }
                    }
                    else
                    {
                        Output.Write("\n        <a title=\"Click to edit this related item\" href=\"" + Base_URL + "l/technical/javascriptrequired\" onfocus=\"link_focused2('form_related_item_term_" + item_index + "')\" onblur=\"link_blurred2('form_related_item_term_" + item_index + "')\" onkeypress=\"return popup_keypress_focus('form_related_item_" + item_index + "', 'form_relateditem_title_" + item_index + "', '" + IsMozilla.ToString() + "' );\" onclick=\"return popup_focus('form_related_item_" + item_index + "', 'form_relateditem_title_" + item_index + "' );\"><div class=\"form_linkline_empty form_related_item_line\" id=\"form_related_item_term_" + item_index + "\">");

                        Output.Write("<i>Empty Related Item</i>");
                    }

                    Output.Write("</div></a>");

                    // Add the popup form
                    PopupFormBuilder.AppendLine("<!-- Related Item Form " + item_index + " -->");
					PopupFormBuilder.AppendLine("<div class=\"related_item_popup_div sbkMetadata_PopupDiv\" id=\"form_related_item_" + item_index + "\" style=\"display:none;\">");
					PopupFormBuilder.AppendLine("  <div class=\"sbkMetadata_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left\">Edit Related Items</td><td style=\"text-align:right\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" alt=\"HELP\" target=\"_" + html_element_name.ToUpper() + "\" >?</a> &nbsp; <a href=\"#template\" alt=\"CLOSE\" onclick=\"close_related_item_form('form_related_item_" + item_index + "')\">X</a> &nbsp; </td></tr></table></div>");
                    PopupFormBuilder.AppendLine("  <br />");
					PopupFormBuilder.AppendLine("  <table class=\"sbkMetadata_PopupTable\">");

                    // Add the relation and display label
                    PopupFormBuilder.Append("    <tr><td style=\"width:90px\">Relation:</td><td><select class=\"form_relateditem_select\" name=\"form_relateditem_relation_" + item_index + "\" id=\"form_relateditem_relation_" + item_index + "\" >");
                    PopupFormBuilder.Append(thisItem.Relationship == Related_Item_Type_Enum.UNKNOWN
                                                  ? "<option value=\"\" selected=\"selected\" >&nbsp;</option>"
                                                  : "<option value=\"\">&nbsp;</option>");

                    PopupFormBuilder.Append(thisItem.Relationship == Related_Item_Type_Enum.Host
                                                  ? "<option value=\"host\" selected=\"selected\" >Host</option>"
                                                  : "<option value=\"host\">Host</option>");

                    PopupFormBuilder.Append(thisItem.Relationship == Related_Item_Type_Enum.OtherFormat
                                                  ? "<option value=\"other_format\" selected=\"selected\" >Other Format</option>"
                                                  : "<option value=\"other_format\">Other Format</option>");

                    PopupFormBuilder.Append(thisItem.Relationship == Related_Item_Type_Enum.OtherVersion
                                                  ? "<option value=\"other_version\" selected=\"selected\" >Other Version</option>"
                                                  : "<option value=\"other_version\">Other Version</option>");

                    PopupFormBuilder.Append(thisItem.Relationship == Related_Item_Type_Enum.Preceding
                                                  ? "<option value=\"preceding\" selected=\"selected\" >Preceding</option>"
                                                  : "<option value=\"preceding\">Preceding</option>");

                    PopupFormBuilder.Append(thisItem.Relationship == Related_Item_Type_Enum.Succeeding
                                                  ? "<option value=\"succeeding\" selected=\"selected\" >Succeeding</option>"
                                                  : "<option value=\"succeeding\">Succeeding</option>");

                    PopupFormBuilder.Append("</select></td>");
                    PopupFormBuilder.AppendLine("      <td style=\"width:255px\">Display Label: &nbsp; <input class=\"form_relateditem_medium_input sbk_Focusable\" name=\"form_relateditem_display_" + item_index + "\" id=\"form_relateditem_display_" + item_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisItem.URL_Display_Label) + "\" /></td>");
                    PopupFormBuilder.AppendLine("    </tr>");

                    string issn = String.Empty;
                    string oclc = String.Empty;
                    string lccn = String.Empty;
                    if (thisItem.Identifiers_Count > 0)
                    {
                        foreach (Identifier_Info thisIdentifier in thisItem.Identifiers)
                        {
                            switch (thisIdentifier.Type.ToUpper())
                            {
                                case "ISSN":
                                    issn = thisIdentifier.Identifier;
                                    break;

                                case "OCLC":
                                    oclc = thisIdentifier.Identifier;
                                    break;

                                case "LCCN":
                                    lccn = thisIdentifier.Identifier;
                                    break;
                            }
                        }
                    }

                    // Add the title and URL rows
                    string related_title = String.Empty;
                    if (thisItem.hasMainTitle)
                        related_title = thisItem.Main_Title.Title;

					PopupFormBuilder.AppendLine("    <tr><td>Title:</td><td colspan=\"2\"><input class=\"form_relateditem_large_input sbk_Focusable\" name=\"form_relateditem_title_" + item_index + "\" id=\"form_relateditem_title_" + item_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(related_title) + "\" /></td></tr>");
					PopupFormBuilder.AppendLine("    <tr><td>URL:</td><td colspan=\"2\"><input class=\"form_relateditem_large_input sbk_Focusable\" name=\"form_relateditem_url_" + item_index + "\" id=\"form_relateditem_url_" + item_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisItem.URL) + "\" /></td></tr>");

                    // Add the system ID and ISSN row
					PopupFormBuilder.AppendLine("    <tr><td>System ID:</td><td><input class=\"form_relateditem_medium_input sbk_Focusable\" name=\"form_relateditem_sobekid_" + item_index + "\" id=\"form_relateditem_sobekid_" + item_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(thisItem.SobekCM_ID) + "\" /></td>");
					PopupFormBuilder.AppendLine("        <td>ISSN: &nbsp; <input class=\"form_relateditem_medium_input sbk_Focusable\" name=\"form_relateditem_issn_" + item_index + "\" id=\"form_relateditem_issn_" + item_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(issn) + "\" /></td></tr>");

                    // Add the OCLC and LCCN row
                    PopupFormBuilder.AppendLine("    <tr><td>OCLC:</td><td><input class=\"form_relateditem_medium_input sbk_Focusable\" name=\"form_relateditem_oclc_" + item_index + "\" id=\"form_relateditem_oclc_" + item_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(oclc) + "\" /></td>");
					PopupFormBuilder.AppendLine("        <td>LCCN: &nbsp; <input class=\"form_relateditem_medium_input sbk_Focusable\" name=\"form_relateditem_lccn_" + item_index + "\" id=\"form_relateditem_lccn_" + item_index + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(lccn) + "\" /></td></tr>");

					// Finish the popup form and add the CLOSE button
					PopupFormBuilder.AppendLine("    <tr style=\"height:35px; text-align: center; vertical-align: bottom;\">");
					PopupFormBuilder.AppendLine("      <td colspan=\"3\"><button title=\"Close\" class=\"sbkMetadata_RoundButton\" onclick=\"return close_related_item_form('form_related_item_" + item_index + "');\">CLOSE</button></td>");
					PopupFormBuilder.AppendLine("    </tr>");
					PopupFormBuilder.AppendLine("  </table>");
					PopupFormBuilder.AppendLine("</div>");
					PopupFormBuilder.AppendLine();

                    item_index++;
                }
            }

            // Add the link to add a new related item and close the main element
            Output.WriteLine("\n            </div>");
            Output.WriteLine("          </td>");

            Output.WriteLine("          <td style=\"vertical-align:bottom\" >");
            if (Repeatable)
            {
                Output.WriteLine("            <img title=\"" + Translator.Get_Translation("Click to add a new related item", CurrentLanguage) + ".\" alt=\"+\" class=\"repeat_button\" src=\"" + REPEAT_BUTTON_URL + "\" onmousedown=\"new_relateditem_link_clicked('" + Template_Page + "');\" />");
            }
            Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img class=\"help_button\" src=\"" + HELP_BUTTON_URL + "\" /></a>");
            Output.WriteLine("          </td>");
            Output.WriteLine("        </tr>");
            Output.WriteLine("      </table>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine();
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears any preexisting related items </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            Bib.Bib_Info.Clear_Related_Items();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf("form_relateditem_relation_") == 0)
                {
                    string diff = thisKey.Replace("form_relateditem_relation_", "");
                    string relation = HttpContext.Current.Request.Form[thisKey];
                    string display = HttpContext.Current.Request.Form["form_relateditem_display_" + diff].Trim();
                    string title = HttpContext.Current.Request.Form["form_relateditem_title_" + diff].Trim();
                    string url = HttpContext.Current.Request.Form["form_relateditem_url_" + diff].Trim();
                    string sobekid = HttpContext.Current.Request.Form["form_relateditem_sobekid_" + diff].Trim();
                    string issn = HttpContext.Current.Request.Form["form_relateditem_issn_" + diff].Trim();
                    string oclc = HttpContext.Current.Request.Form["form_relateditem_oclc_" + diff].Trim();
                    string lccn = HttpContext.Current.Request.Form["form_relateditem_lccn_" + diff].Trim();

                    if ((title.Length > 0) || (url.Length > 0) || (sobekid.Length > 0) || (issn.Length > 0) ||
                        (oclc.Length > 0) || (lccn.Length > 0))
                    {
                        Related_Item_Info newItem = new Related_Item_Info();

                        switch (relation)
                        {
                            case "host":
                                newItem.Relationship = Related_Item_Type_Enum.Host;
                                break;

                            case "other_format":
                                newItem.Relationship = Related_Item_Type_Enum.OtherFormat;
                                break;

                            case "other_version":
                                newItem.Relationship = Related_Item_Type_Enum.OtherVersion;
                                break;

                            case "preceding":
                                newItem.Relationship = Related_Item_Type_Enum.Preceding;
                                break;

                            case "succeeding":
                                newItem.Relationship = Related_Item_Type_Enum.Succeeding;
                                break;
                        }
                        newItem.URL_Display_Label = display;
                        if (title.Length > 0)
                        {
                            newItem.Main_Title.Title = title;
                        }
                        newItem.URL = url;
                        newItem.SobekCM_ID = sobekid;
                        if (issn.Length > 0)
                            newItem.Add_Identifier(issn, "ISSN");
                        if (oclc.Length > 0)
                            newItem.Add_Identifier(oclc, "OCLC");
                        if (lccn.Length > 0)
                            newItem.Add_Identifier(lccn, "LCCN");

                        Bib.Bib_Info.Add_Related_Item(newItem);
                    }
                }
            }
        }

        #endregion

        #region Methods Implementing the Abstract Methods from abstract_Element class

        /// <summary> Reads the inner data from the CompleteTemplate XML format </summary>
        /// <param name="XMLReader"> Current template xml configuration reader </param>
        /// <remarks> This procedure does not currently read any inner xml (not yet necessary) </remarks>
        protected override void Inner_Read_Data(XmlTextReader XMLReader)
        {
            // Do nothing
        }

        #endregion
    }
}




