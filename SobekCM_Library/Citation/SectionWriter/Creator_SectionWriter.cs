using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SobekCM.Core.BriefItem;
using SobekCM.Core.UI_Configuration.Citation;
using SobekCM.Tools;

namespace SobekCM.Library.Citation.SectionWriter
{
    /// <summary> Special citation section writer adds the creator information, which includes the role
    /// while not making the role part of the search term </summary>
    /// <remarks> This class implements the <see cref="iCitationSectionWriter"/> interface. </remarks>
    public class Creator_SectionWriter : iCitationSectionWriter
    {
        /// <summary> Returns flag that indicates this citation section writer 
        /// will be writing alues to the output stream </summary>
        /// <param name="ElementInfo"> Additional possible data about this citation element </param>
        /// <param name="Item"> Digital resource to analyze for data to write </param>
        public bool Has_Data_To_Write(CitationElement ElementInfo, BriefItemInfo Item)
        {
            // Look for a match in the item description
            BriefItem_DescriptiveTerm firstCreator = Item.Get_Description("Creator");
            return ((firstCreator != null) && (firstCreator.Values.Count > 0));
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

            BriefItem_DescriptiveTerm creatorList = Item.Get_Description("Creator");

            bool first_creator = true;

            Output.AppendLine("        <dt class=\"sbk_CivCREATOR_Element\" style=\"width:" + LeftColumnWidth + "px;\" >Creators: </dt>");
            Output.Append("        <dd class=\"sbk_CivCREATOR_Element\" style=\"margin-left:" + LeftColumnWidth + "px;\" >");

            // Determine if ths standard itemprop was overriden
            string itemProp = (!String.IsNullOrWhiteSpace(ElementInfo.ItemProp)) ? ElementInfo.ItemProp : "creator";

            // Determine if the standard search code was overriden
            string searchCode = (!String.IsNullOrWhiteSpace(ElementInfo.SearchCode)) ? ElementInfo.SearchCode : "AU";

            foreach (BriefItem_DescTermValue thisValue in creatorList.Values)
            {
                // Was this the first?
                if (first_creator) first_creator = false;
                else Output.AppendLine("<br />");

                // It is possible a different search term is valid for this item, so check it
                string searchTerm = (!String.IsNullOrWhiteSpace(thisValue.SearchTerm)) ? thisValue.SearchTerm : thisValue.Value;

                Output.Append("<span itemprop=\"" + itemProp + "\">" + SearchLink.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", searchCode) + display_text_from_value(thisValue.Value) + SearchLinkEnd);

                if (!String.IsNullOrEmpty(thisValue.Authority))
                {
                    Output.Append(" ( " + display_text_from_value(thisValue.Authority) + " ) ");
                }

                if (!String.IsNullOrEmpty(thisValue.SubTerm))
                {
                    Output.Append(" ( <i>" + display_text_from_value(thisValue.SubTerm) + "</i> ) ");
                }


                Output.AppendLine("</span>");
            }
        }

        private static string display_text_from_value(string Value)
        {
            return HttpUtility.HtmlEncode(Value).Replace("&lt;i&gt;", "<i>").Replace("&lt;/i&gt;", "</i>");
        }

        private static string search_link_from_value(string Value)
        {
            string replacedValue = Value.Replace("&amp;", "&").Replace("&", "").Replace("  ", " ");
            string urlEncode = HttpUtility.UrlEncode(replacedValue);
            return urlEncode != null ? urlEncode.Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+") : String.Empty;
        }
    }
}