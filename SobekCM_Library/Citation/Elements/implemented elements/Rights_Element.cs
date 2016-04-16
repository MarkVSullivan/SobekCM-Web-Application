#region Using directives

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Resource_Object;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows simple entry of the rights for an item </summary>
    /// <remarks> This class extends the <see cref="textArea_Element"/> class. </remarks>
    public class Rights_Element : textArea_Element
    {
        private string baseURL;

        /// <summary> Constructor for a new instance of the Rights_Element class </summary>
        public Rights_Element()
            : base("Rights Management", "rights_mgmt")
        {
            Repeatable = false;
            baseURL = String.Empty;
        }

        /// <summary> Sets the base url for the current request </summary>
        /// <param name="Base_URL"> Current Base URL for this request </param>
        public override void Set_Base_URL(string Base_URL)
        {
            baseURL = Base_URL;
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
                const string defaultAcronym = "Enter the rights you give for sharing, repurposing, or remixing your item to other users.  You may also select a creative commons license below.";
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

            // render_helper(Output, Bib.Bib_Info.Access_Condition.Text, Skin_Code, isMozilla, Current_User, CurrentLanguage, Translator);

            string id_name = html_element_name.Replace("_", "");

            int actual_cols = cols;
            if (IsMozilla)
                actual_cols = cols_mozilla;

            Output.WriteLine("  <!-- " + Title + " Element -->");
            Output.WriteLine("  <tr align=\"left\">");
            Output.WriteLine("    <td width=\"" + LEFT_MARGIN + "px\">&nbsp;</td>");
            if (Read_Only)
            {
                Output.WriteLine("    <td valign=\"top\" class=\"metadata_label\">" + Title + ":</b></td>");
            }
            else
            {
                if (Acronym.Length > 0)
                {
                    Output.WriteLine("    <td valign=\"top\" class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\"><acronym title=\"" + Acronym + "\">" + Title + ":</acronym></a></td>");
                }
                else
                {
                    Output.WriteLine("    <td valign=\"top\" class=\"metadata_label\"><a href=\"" + Help_URL(Skin_Code, Base_URL) + "\" target=\"_" + html_element_name.ToUpper() + "\">" + Title + ":</a></td>");
                }
            }
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table>");
            Output.WriteLine("        <tr>");
            Output.WriteLine("          <td>");
            Output.WriteLine("            <div id=\"" + html_element_name + "_div\">");
            Output.WriteLine("              <textarea rows=\"" + rows + "\" cols=\"" + actual_cols + "\" name=\"" + id_name + "1\" id=\"" + id_name + "1\" class=\"" + html_element_name + "_input\" onfocus=\"javascript:textbox_enter('" + id_name + "1','" + html_element_name + "_input_focused')\" onblur=\"javascript:textbox_leave('" + id_name + "1','" + html_element_name + "_input')\">" + HttpUtility.HtmlEncode(Bib.Bib_Info.Access_Condition.Text.Trim()) + "</textarea>");
            Output.WriteLine("              <div class=\"ShowOptionsRow\">");
			Output.WriteLine("                <ul class=\"sbk_FauxDownwardTabsList\">");
			Output.WriteLine("                  <li><a href=\"\" onclick=\"return open_cc_rights();\">CREATIVE COMMONS</a></li>");
            Output.WriteLine("                  <li><a href=\"\" onclick=\"return open_uf_rights();\">UF RIGHTS STATEMENTS</a></li>");
			Output.WriteLine("                </ul>");
            Output.WriteLine("              </div>");
            Output.WriteLine("            </div>");
            Output.WriteLine("          </td>");
            Output.WriteLine("          <td valign=\"bottom\" >");
            Output.WriteLine("            <a target=\"_" + html_element_name.ToUpper() + "\"  title=\"" + Translator.Get_Translation("Get help.", CurrentLanguage) + "\" href=\"" + Help_URL(Skin_Code, Base_URL) + "\" ><img border=\"0px\" class=\"help_button_rightsmgmt\" src=\"" + HELP_BUTTON_URL + "\" /></a>");
            Output.WriteLine("          </td>");
            Output.WriteLine("        </tr>");
            Output.WriteLine("      </table>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine();


            Output.WriteLine("  <tr align=\"left\">");
           Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
           Output.WriteLine("    <td>");
            Output.WriteLine("      <table id=\"cc_rights\" cellpadding=\"3px\" cellspacing=\"3px\" style=\"display:none;\">");
            Output.WriteLine("        <tr><td colspan=\"2\">You may also select a <a title=\"Explanation of different creative commons licenses.\" href=\"http://creativecommons.org/about/licenses/\">Creative Commons License</a> option below.<br /></td></tr>");
            Output.WriteLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('rightsmgmt1','[cc0] The author dedicated the work to the Commons by waiving all of his or her rights to the work worldwide under copyright law and all related or neighboring legal rights he or she had in the work, to the extent allowable by law.');\"><img title=\"You dedicate the work to the Commons by waiving all of your rights to the work worldwide under copyright law and all related or neighboring legal rights you had in the work, to the extent allowable by law.\" src=\"" + Static_Resources_Gateway.Cc_Zero_Img + "\" /></a></td><td><b>No Copyright</b><br /><i>cc0</i></td></tr>");
            Output.WriteLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('rightsmgmt1','[cc by] This item is licensed with the Creative Commons Attribution License.  This license lets others distribute, remix, tweak, and build upon this work, even commercially, as long as they credit the author for the original creation.');\"><img title=\"This license lets others distribute, remix, tweak, and build upon your work, even commercially, as long as they credit you for the original creation.\" src=\"" + Static_Resources_Gateway.Cc_By_Img + "\" /></a></td><td><b>Attribution</b><br /><i>cc by</i></td></tr>");
            Output.WriteLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('rightsmgmt1','[cc by-sa] This item is licensed with the Creative Commons Attribution Share Alike License.  This license lets others remix, tweak, and build upon this work even for commercial reasons, as long as they credit the author and license their new creations under the identical terms.');\"><img title=\"This license lets others remix, tweak, and build upon your work even for commercial reasons, as long as they credit you and license their new creations under the identical terms.\" src=\"" + Static_Resources_Gateway.Cc_By_Sa_Img + "\" /></a></td><td><b>Attribution Share Alike</b><br /><i>cc by-sa</i></td></tr>");
            Output.WriteLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('rightsmgmt1','[cc by-nd] This item is licensed with the Creative Commons Attribution No Derivatives License.  This license allows for redistribution, commercial and non-commercial, as long as it is passed along unchanged and in whole, with credit to the author.');\"><img title=\"This license allows for redistribution, commercial and non-commercial, as long as it is passed along unchanged and in whole, with credit to you.\" src=\"" + Static_Resources_Gateway.Cc_By_Nd_Img + "\" /></a></td><td><b>Attribution No Derivatives</b><br /><i>cc by-nd</i></td></tr>");
            Output.WriteLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('rightsmgmt1','[cc by-nc] This item is licensed with the Creative Commons Attribution Non-Commerical License.  This license lets others remix, tweak, and build upon this work non-commercially, and although their new works must also acknowledge the author and be non-commercial, they don’t have to license their derivative works on the same terms.');\"><img title=\"This license lets others remix, tweak, and build upon your work non-commercially, and although their new works must also acknowledge you and be non-commercial, they don’t have to license their derivative works on the same terms.\" src=\"" + Static_Resources_Gateway.Cc_By_Nc_Img + "\" /></a></td><td><b>Attribution Non-Commercial</b><br /><i>cc by-nc</i></td></tr>");
            Output.WriteLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('rightsmgmt1','[cc by-nc-sa] This item is licensed with the Creative Commons Attribution Non-Commercial Share Alike License.  This license lets others remix, tweak, and build upon this work non-commercially, as long as they credit the author and license their new creations under the identical terms.');\"><img title=\"This license lets others remix, tweak, and build upon your work non-commercially, as long as they credit you and license their new creations under the identical terms.\" src=\"" + Static_Resources_Gateway.Cc_By_Nc_Sa_Img + "\" /></a></td><td><b>Attribution Non-Commercial Share Alike</b><br /><i>cc by-nc-sa</i></td></tr>");
            Output.WriteLine("        <tr><td> &nbsp; <a href=\"\" onclick=\"return set_cc_rights('rightsmgmt1','[cc by-nc-nd] This item is licensed with the Creative Commons Attribution Non-Commercial No Derivative License.  This license allows others to download this work and share them with others as long as they mention the author and link back to the author, but they can’t change them in any way or use them commercially.');\"><img title=\"This license allows others to download your works and share them with others as long as they mention you and link back to you, but they can’t change them in any way or use them commercially.\" src=\"" + Static_Resources_Gateway.Cc_By_Nc_Nd_Img + "\" /></a></td><td><b>Attribution Non-Commercial No Derivatives</b><br /><i>cc by-nc-nd</i></td></tr>");
            Output.WriteLine("      </table>");
           Output.WriteLine("    </td>");
           Output.WriteLine("  </tr>");

            //Add the table for the UF rights statements
     
            Output.WriteLine("<tr align=\"left\">");
            Output.WriteLine("<td colspan=\"2\">&nbsp;</td>");
            Output.WriteLine("<td>");
            Output.WriteLine("        <table id=\"uf_rights\" cellpadding=\"3pc\" cellspacing=\"3pc\" style=\"display:none;\">");
            Output.WriteLine("          <tr><td colspan=\"2\">You may also select a UF Rights Statement option below.<br/></td></tr>");
            Output.WriteLine("          <tr><td  class=\"uf_rights_column\" colspan=\"2\">&#8226; &nbsp;&nbsp;<span title=\"Copyright Board of Trustees of the University of Florida\" style=\"color:#333;\" onclick=\"return set_uf_rights('rightsmgmt1','Copyright Board of Trustees of the University of Florida');\" ><b>UF Copyright</b></span></td></tr>");
            Output.WriteLine("          <tr><td  class=\"uf_rights_column\" colspan=\"2\">&#8226; &nbsp;&nbsp;<span title=\"Copyright [name of copyright holder or Creator or Publisher as appropriate]. Permission granted to University of Florida to digitize and display this item for non-profit research and educational purposes. Any reuse of this item in excess of fair use or other copyright exemptions requires permission of the copyright holder.\"  style=\"color:#333;\" onclick=\"return set_uf_rights('rightsmgmt1','Copyright [name of copyright holder or Creator or Publisher as appropriate]. Permission granted to University of Florida to digitize and display this item for non-profit research and educational purposes. Any reuse of this item in excess of fair use or other copyright exemptions requires permission of the copyright holder.');\"><b>With Permission</b></span></td></tr>");
            Output.WriteLine("          <tr><td  class=\"uf_rights_column\" colspan=\"2\">&#8226; &nbsp;&nbsp;<span title=\"The University of Florida George A. Smathers Libraries respect the intellectual property rights of others and do not claim any copyright interest in this item. This item may be protected by copyright but is made available here under a claim of fair use (17 U.S.C. §107) for non-profit research and educational purposes. Users of this work have responsibility for determining copyright status prior to reusing, publishing or reproducing this item for purposes other than what is allowed by fair use or other copyright exemptions. Any reuse of this item in excess of fair use or other copyright exemptions requires permission of the copyright holder. The Smathers Libraries would like to learn more about this item and invite individuals or organizations to contact Digital Services (UFDC@uflib.ufl.edu) with any additional information they can provide.\" style=\"color:#333;\" onclick=\"return set_uf_rights('rightsmgmt1','The University of Florida George A. Smathers Libraries respect the intellectual property rights of others and do not claim any copyright interest in this item. This item may be protected by copyright but is made available here under a claim of fair use (17 U.S.C. §107) for non-profit research and educational purposes. Users of this work have responsibility for determining copyright status prior to reusing, publishing or reproducing this item for purposes other than what is allowed by fair use or other copyright exemptions. Any reuse of this item in excess of fair use or other copyright exemptions requires permission of the copyright holder. The Smathers Libraries would like to learn more about this item and invite individuals or organizations to contact Digital Services (UFDC@uflib.ufl.edu) with any additional information they can provide.');\"><b>Fair Use</b></span></td></tr>");
            Output.WriteLine("          <tr><td  class=\"uf_rights_column\" colspan=\"2\">&#8226; &nbsp;&nbsp;<span title=\"This item is a work of the U.S. federal government and not subject to copyright pursuant to 17 U.S.C. §105.\" style=\"color:#333;\" onclick=\"return set_uf_rights('rightsmgmt1','This item is a work of the U.S. federal government and not subject to copyright pursuant to 17 U.S.C. §105.');\"><b>Federal Government Document</b></span></td></tr>");
            Output.WriteLine("          <tr><td  class=\"uf_rights_column\" colspan=\"2\">&#8226; &nbsp;&nbsp;<span title=\"This item is presumed to be in the public domain.  The University of Florida George A. Smathers Libraries respect the intellectual property rights of others and do not claim any copyright interest in this item. Users of this work have responsibility for determining copyright status prior to reusing, publishing or reproducing this item for purposes other than what is allowed by fair use or other copyright exemptions. Any reuse of this item in excess of fair use or other copyright exemptions may require permission of the copyright holder. The Smathers Libraries would like to learn more about this item and invite individuals or organizations to contact Digital Services (UFDC@uflib.ufl.edu) with any additional information they can provide.\" style=\"color:#333;\" onclick=\"return set_uf_rights('rightsmgmt1','This item is presumed to be in the public domain.  The University of Florida George A. Smathers Libraries respect the intellectual property rights of others and do not claim any copyright interest in this item. Users of this work have responsibility for determining copyright status prior to reusing, publishing or reproducing this item for purposes other than what is allowed by fair use or other copyright exemptions. Any reuse of this item in excess of fair use or other copyright exemptions may require permission of the copyright holder. The Smathers Libraries would like to learn more about this item and invite individuals or organizations to contact Digital Services (UFDC@uflib.ufl.edu) with any additional information they can provide.');\"><b>Public Domain Presumed</b></span></td></tr>");
            Output.WriteLine("          <tr><td  class=\"uf_rights_column\" colspan=\"2\">&#8226; &nbsp;&nbsp;<span title=\"Copyright [name of dissertation author]. Permission granted to the University of Florida to digitize, archive and distribute this item for non-profit research and educational purposes. Any reuse of this item in excess of fair use or other copyright exemptions requires permission of the copyright holder.\" style=\"color:#333\" onclick=\"return set_uf_rights('rightsmgmt1','Copyright [name of dissertation author]. Permission granted to the University of Florida to digitize, archive and distribute this item for non-profit research and educational purposes. Any reuse of this item in excess of fair use or other copyright exemptions requires permission of the copyright holder.');\"><b>RDS Permission</b></span></td></tr>");
            Output.WriteLine("          <tr><td  class=\"uf_rights_column\" colspan=\"2\">&#8226; &nbsp;&nbsp;<span title=\"The University of Florida George A. Smathers Libraries respect the intellectual property rights of others and do not claim any copyright interest in this item. This item may be protected by copyright but is made available here under a claim of fair use (17 U.S.C. §107) for non-profit research and educational purposes. Users of this work have responsibility for determining copyright status prior to reusing, publishing or reproducing this item for purposes other than what is allowed by fair use or other copyright exemptions. Any reuse of this item in excess of fair use or other copyright exemptions requires permission of the copyright holder. The Smathers Libraries would like to learn more about this item and invite individuals or organizations to contact the RDS coordinator (ufdissertations@uflib.ufl.edu) with any additional information they can provide.\" style=\"color:#333\" onclick=\"return set_uf_rights('rightsmgmt1','The University of Florida George A. Smathers Libraries respect the intellectual property rights of others and do not claim any copyright interest in this item. This item may be protected by copyright but is made available here under a claim of fair use (17 U.S.C. §107) for non-profit research and educational purposes. Users of this work have responsibility for determining copyright status prior to reusing, publishing or reproducing this item for purposes other than what is allowed by fair use or other copyright exemptions. Any reuse of this item in excess of fair use or other copyright exemptions requires permission of the copyright holder. The Smathers Libraries would like to learn more about this item and invite individuals or organizations to contact the RDS coordinator (ufdissertations@uflib.ufl.edu) with any additional information they can provide.');\"><b>RDS No Response</b></td></tr>");
            Output.WriteLine("          <tr><td  class=\"uf_rights_column\" colspan=\"2\">&#8226; &nbsp;&nbsp;<span title=\"This item is presumed in the public domain according to the terms of the Retrospective Dissertation Scanning (RDS) policy, which may be viewed at http://ufdc.ufl.edu/AA00007596/00001. The University of Florida George A. Smathers Libraries respect the intellectual property rights of others and do not claim any copyright interest in this item. Users of this work have responsibility for determining copyright status prior to reusing, publishing or reproducing this item for purposes other than what is allowed by fair use or other copyright exemptions. Any reuse of this item in excess of fair use or other copyright exemptions requires permission of the copyright holder. The Smathers Libraries would like to learn more about this item and invite individuals or organizations to contact the RDS coordinator (ufdissertations@uflib.ufl.edu) with any additional information they can provide.\" style=\"color:#333\" onclick=\"return set_uf_rights('rightsmgmt1','This item is presumed in the public domain according to the terms of the Retrospective Dissertation Scanning (RDS) policy, which may be viewed at http://ufdc.ufl.edu/AA00007596/00001. The University of Florida George A. Smathers Libraries respect the intellectual property rights of others and do not claim any copyright interest in this item. Users of this work have responsibility for determining copyright status prior to reusing, publishing or reproducing this item for purposes other than what is allowed by fair use or other copyright exemptions. Any reuse of this item in excess of fair use or other copyright exemptions requires permission of the copyright holder. The Smathers Libraries would like to learn more about this item and invite individuals or organizations to contact the RDS coordinator (ufdissertations@uflib.ufl.edu) with any additional information they can provide.');\"><b>RDS Public Domain</b></span></td></tr>");
            Output.WriteLine("          <tr><td  class=\"uf_rights_column\" colspan=\"2\">&#8226; &nbsp;&nbsp;<span title=\"Copyright [name of dissertation author]. Permission granted to the University of Florida to digitize, archive and distribute this item for non-profit research and educational purposes. Any reuse of this item in excess of fair use or other copyright exemptions requires permission of the copyright holder.\" style=\"color:#333\" onclick=\"return set_uf_rights('rightsmgmt1','Copyright [name of dissertation author]. Permission granted to the University of Florida to digitize, archive and distribute this item for non-profit research and educational purposes. Any reuse of this item in excess of fair use or other copyright exemptions requires permission of the copyright holder.');\"><b>Dissertation/Thesis Permission</b></span></td></tr>");
            Output.WriteLine("          <tr><td  class=\"uf_rights_column\" colspan=\"2\">&#8226; &nbsp;&nbsp;<span title=\"This document is copyrighted by the University of Florida, Institute of Food and Agricultural Sciences (UF/IFAS) for the people of the State of Florida. UF/IFAS retains all rights under all conventions, but permits free reproduction by all agents and offices of the Cooperative Extension Service and the people of the State of Florida. Permission is granted to others to use these materials in part or in full for educational purposes, provided that full credit is given to the UF/IFAS, citing the publication, its source, and date of publication.\" style=\"color:#333\" onclick=\"return set_uf_rights('rightsmgmt1','This document is copyrighted by the University of Florida, Institute of Food and Agricultural Sciences (UF/IFAS) for the people of the State of Florida. UF/IFAS retains all rights under all conventions, but permits free reproduction by all agents and offices of the Cooperative Extension Service and the people of the State of Florida. Permission is granted to others to use these materials in part or in full for educational purposes, provided that full credit is given to the UF/IFAS, citing the publication, its source, and date of publication.');\"><b>IFAS Extension Documents</b></span></td></tr>");
            Output.WriteLine("          <tr><td  class=\"uf_rights_column\" colspan=\"2\">&#8226; &nbsp;&nbsp;<span title=\"This item was contributed to the Digital Library of the Caribbean (dLOC) by the source institution listed in the metadata. This item may or may not be protected by copyright in the country where it was produced. Users of this work have responsibility for determining copyright status prior to reusing, publishing or reproducing this item for purposes other than what is allowed by applicable law, including any applicable international copyright treaty or fair use or fair dealing statutes, which dLOC partners have explicitly supported and endorsed. Any reuse of this item in excess of applicable copyright exceptions may require permission. dLOC would encourage users to contact the source institution directly or dloc@fiu.edu to request more information about copyright status or to provide additional information about the item.\" style=\"color:#333\" onclick=\"return set_uf_rights('rightsmgmt1','This item was contributed to the Digital Library of the Caribbean (dLOC) by the source institution listed in the metadata. This item may or may not be protected by copyright in the country where it was produced. Users of this work have responsibility for determining copyright status prior to reusing, publishing or reproducing this item for purposes other than what is allowed by applicable law, including any applicable international copyright treaty or fair use or fair dealing statutes, which dLOC partners have explicitly supported and endorsed. Any reuse of this item in excess of applicable copyright exceptions may require permission. dLOC would encourage users to contact the source institution directly or dloc@fiu.edu to request more information about copyright status or to provide additional information about the item.');\"><b>dLOC</b></span></td></tr>");

            Output.WriteLine("         </table>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine("");
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This does nothing since there is only one rights statement </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Do nothing since there is only one rights statement
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys.Where(thisKey => thisKey.IndexOf(html_element_name.Replace("_", "")) == 0))
            {
                Bib.Bib_Info.Access_Condition.Text = HttpContext.Current.Request.Form[thisKey];
                return;
            }
        }
    }
}
