using System;
using System.IO;
using System.Text;
using SobekCM.Core.BriefItem;
using SobekCM.Core.UI_Configuration.Citation;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.Citation.SectionWriter
{
    /// <summary> Special citation section writer adds the rights information, which can include
    /// a creative commons icon as well as the rights information </summary>
    /// <remarks> This class implements the <see cref="iCitationSectionWriter"/> interface. </remarks>
    public class Rights_SectionWriter : iCitationSectionWriter
    {
        /// <summary> Returns flag that indicates this citation section writer 
        /// will be writing alues to the output stream </summary>
        /// <param name="ElementInfo"> Additional possible data about this citation element </param>
        /// <param name="Item"> Digital resource to analyze for data to write </param>
        public bool Has_Data_To_Write(CitationElement ElementInfo, BriefItemInfo Item)
        {
            // Always show SOME rights
            return true;
        }

        /// <summary> Wites a section of citation from a provided digital resource </summary>
        /// <param name="ElementInfo"> Additional possible data about this citation element </param>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Item"> Digital resource with all the data to write </param>
        /// <param name="LeftColumnWidth"> Number of pixels of the left column, or the definition terms </param>
        /// <param name="SearchLink"> Beginning of the search link that can be used to allow the web patron to select a term and run a search against this instance </param>
        /// <param name="SearchLinkEnd"> End of the search link that can be used to allow the web patron to select a term and run a search against this instance  </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Write_Citation_Section(CitationElement ElementInfo, StringBuilder Output, BriefItemInfo Item, int LeftColumnWidth, string SearchLink, string SearchLinkEnd, Custom_Tracer Tracer)
        {
            // Set the default (since this always displays)
            string rights_statement = "All applicable rights reserved by the source institution and holding location.";
            string rights_image = String.Empty;
            string uri = String.Empty;

            const string SEE_TEXT = "See License Deed";

            // Look for a match in the item description
            BriefItem_DescriptiveTerm rightsTerm = Item.Get_Description("Rights Management");

            if ((rightsTerm != null) && (rightsTerm.Values.Count > 0))
            {
                rights_statement = rightsTerm.Values[0].Value;

                if ((rightsTerm.Values[0].URIs != null) && (rightsTerm.Values[0].URIs.Count == 1))
                {
                    uri = rightsTerm.Values[0].URIs[0];

                    switch (uri)
                    {
                        case "http://creativecommons.org/licenses/by-nc-nd/3.0/":
                            rights_image = UI_ApplicationCache_Gateway.Configuration.UI.StaticResources.Cc_By_Nc_Nd_Img;
                            break;

                        case "http://creativecommons.org/licenses/by-nc-sa/3.0/":
                            rights_image = UI_ApplicationCache_Gateway.Configuration.UI.StaticResources.Cc_By_Nc_Sa_Img;
                            break;

                        case "http://creativecommons.org/licenses/by-nc/3.0/":
                            rights_image = UI_ApplicationCache_Gateway.Configuration.UI.StaticResources.Cc_By_Nc_Img;
                            break;

                        case "http://creativecommons.org/licenses/by-nd/3.0/":
                            rights_image = UI_ApplicationCache_Gateway.Configuration.UI.StaticResources.Cc_By_Nd_Img;
                            break;

                        case "http://creativecommons.org/licenses/by-sa/3.0/":
                            rights_image = UI_ApplicationCache_Gateway.Configuration.UI.StaticResources.Cc_By_Sa_Img;
                            break;

                        case "http://creativecommons.org/licenses/by/3.0/":
                            rights_image = UI_ApplicationCache_Gateway.Configuration.UI.StaticResources.Cc_By_Img;
                            break;

                        case "http://creativecommons.org/publicdomain/zero/1.0/":
                            rights_image = UI_ApplicationCache_Gateway.Configuration.UI.StaticResources.Cc_Zero_Img;
                            break;
                    }
                }
            }

            string displayLabel = (String.IsNullOrEmpty(ElementInfo.DisplayTerm)) ? "Rights Management" : ElementInfo.DisplayTerm;

            Output.AppendLine("        <dt class=\"sbk_CivRIGHTS_Element\" style=\"width:" + LeftColumnWidth + "px;\" >" + displayLabel + ": </dt>");
            Output.Append("        <dd class=\"sbk_CivRIGHTS_Element\" style=\"margin-left:" + LeftColumnWidth + "px;\"><span itemprop=\"rights\">");

            if ((rights_statement.IndexOf("http://") == 0) || (rights_statement.IndexOf("https://") == 0))
            {
                Output.Append("<a href=\"" + rights_statement + "\" target=\"RIGHTS\" >" + rights_statement + "</a>");
            }
            else
            {
                // Show the rights image?
                if (String.IsNullOrEmpty(rights_image))
                {
                    if (!String.IsNullOrEmpty(uri))
                    {
                        Output.Append(rights_statement + " ( <a href=\"" + uri + "\" alt=\"" + SEE_TEXT + "\" target=\"license\">link</a> )");
                    }
                    else
                    {
                        Output.Append(rights_statement);
                    }
                }
                else
                {
                    // Since the image is derived from the URI, there must be a URI here
                    Output.Append("<a href=\"" + uri + "\" alt=\"" + SEE_TEXT + "\" target=\"cc_license\"><img src=\"" + rights_image + "\" /></a> " + rights_statement);
                }
            }

            Output.AppendLine("</span></dd>");
            Output.AppendLine();
        }
    }
}
