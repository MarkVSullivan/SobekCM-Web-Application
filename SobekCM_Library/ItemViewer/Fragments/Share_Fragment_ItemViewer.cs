using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Library.ItemViewer.Viewers;

namespace SobekCM.Library.ItemViewer.Fragments
{
    /// <summary> Fragment item viewer writes the share/social media form to the stream, to be loaded dynamically
    /// when needed by an item viewer </summary>
    public class Share_Fragment_ItemViewer : baseFragment_ItemViewer
    {
        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Fragment_ShareForm"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Fragment_ShareForm; }
        }

        public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            StringBuilder responseBuilder = new StringBuilder();

            // Calculate the title and url
            string title = HttpUtility.HtmlEncode(CurrentItem.Bib_Info.Main_Title.Title);
            string share_url = CurrentMode.Base_URL + "/" + CurrentItem.BibID + "/" + CurrentItem.VID;
            if (HttpContext.Current != null)
                share_url = HttpContext.Current.Items["Original_URL"].ToString().Replace("&", "%26").Replace("?", "%3F").Replace("http://", "").Replace("=", "%3D").Replace("\"", "&quot;");

            responseBuilder.AppendLine("<!-- Share form -->");
            responseBuilder.AppendLine("<div id=\"shareform_content\">");

            responseBuilder.AppendLine("<a href=\"http://www.facebook.com/share.php?u=" + share_url + "&amp;t=" + title + "\" target=\"FACEBOOK_WINDOW\" onmouseover=\"facebook_share.src='" + CurrentMode.Base_URL + "default/images/facebook_share_h.gif'\" onmouseout=\"facebook_share.src='" + CurrentMode.Base_URL + "default/images/facebook_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"facebook_share\" name=\"facebook_share\" src=\"" + CurrentMode.Base_URL + "default/images/facebook_share.gif\" alt=\"FACEBOOK\" /></a>");
            responseBuilder.AppendLine("<a href=\"http://buzz.yahoo.com/buzz?targetUrl=" + share_url + "&amp;headline=" + title + "\" target=\"YAHOOBUZZ_WINDOW\" onmouseover=\"yahoobuzz_share.src='" + CurrentMode.Base_URL + "default/images/yahoobuzz_share_h.gif'\" onmouseout=\"yahoobuzz_share.src='" + CurrentMode.Base_URL + "default/images/yahoobuzz_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"yahoobuzz_share\" name=\"yahoobuzz_share\" src=\"" + CurrentMode.Base_URL + "default/images/yahoobuzz_share.gif\" alt=\"YAHOO BUZZ\" /></a>");
            responseBuilder.AppendLine("<br />");

            responseBuilder.AppendLine("<a href=\"http://twitter.com/home?status=Currently reading " + share_url + "\" target=\"TWITTER_WINDOW\" onmouseover=\"twitter_share.src='" + CurrentMode.Base_URL + "default/images/twitter_share_h.gif'\" onmouseout=\"twitter_share.src='" + CurrentMode.Base_URL + "default/images/twitter_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"twitter_share\" name=\"twitter_share\" src=\"" + CurrentMode.Base_URL + "default/images/twitter_share.gif\" alt=\"TWITTER\" /></a>");
            responseBuilder.AppendLine("<a href=\"http://www.google.com/bookmarks/mark?op=add&amp;bkmk=" + share_url + "&amp;title=" + title + "\" target=\"GOOGLE_WINDOW\" onmouseover=\"google_share.src='" + CurrentMode.Base_URL + "default/images/google_share_h.gif'\" onmouseout=\"google_share.src='" + CurrentMode.Base_URL + "default/images/google_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"google_share\" name=\"google_share\" src=\"" + CurrentMode.Base_URL + "default/images/google_share.gif\" alt=\"GOOGLE SHARE\" /></a>");
            responseBuilder.AppendLine("<br />");

            responseBuilder.AppendLine("<a href=\"http://www.stumbleupon.com/submit?url=" + share_url + "&amp;title=" + title + "\" target=\"STUMBLEUPON_WINDOW\" onmouseover=\"stumbleupon_share.src='" + CurrentMode.Base_URL + "default/images/stumbleupon_share_h.gif'\" onmouseout=\"stumbleupon_share.src='" + CurrentMode.Base_URL + "default/images/stumbleupon_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"stumbleupon_share\" name=\"stumbleupon_share\" src=\"" + CurrentMode.Base_URL + "default/images/stumbleupon_share.gif\" alt=\"STUMBLEUPON\" /></a>");
            responseBuilder.AppendLine("<a href=\"http://myweb.yahoo.com/myresults/bookmarklet?t=" + title + "&amp;u=" + share_url + "\" target=\"YAHOO_WINDOW\" onmouseover=\"yahoo_share.src='" + CurrentMode.Base_URL + "default/images/yahoo_share_h.gif'\" onmouseout=\"yahoo_share.src='" + CurrentMode.Base_URL + "default/images/yahoo_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"yahoo_share\" name=\"yahoo_share\" src=\"" + CurrentMode.Base_URL + "default/images/yahoo_share.gif\" alt=\"YAHOO SHARE\" /></a>");
            responseBuilder.AppendLine("<br />");

            responseBuilder.AppendLine("<a href=\"http://digg.com/submit?phase=2&amp;url=" + share_url + "&amp;title=" + title + "\" target=\"DIGG_WINDOW\" onmouseover=\"digg_share.src='" + CurrentMode.Base_URL + "default/images/digg_share_h.gif'\" onmouseout=\"digg_share.src='" + CurrentMode.Base_URL + "default/images/digg_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"digg_share\" name=\"digg_share\" src=\"" + CurrentMode.Base_URL + "default/images/digg_share.gif\" alt=\"DIGG\" /></a>");
            responseBuilder.AppendLine("<a onmouseover=\"favorites_share.src='" + CurrentMode.Base_URL + "default/images/favorites_share_h.gif'\" onmouseout=\"favorites_share.src='" + CurrentMode.Base_URL + "default/images/favorites_share.gif'\" onclick=\"javascript:add_to_favorites();\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"favorites_share\" name=\"favorites_share\" src=\"" + CurrentMode.Base_URL + "default/images/favorites_share.gif\" alt=\"MY FAVORITES\" /></a>");
            responseBuilder.AppendLine("<br />");

            responseBuilder.AppendLine("</div>");
            responseBuilder.AppendLine();

            placeHolder.Controls.Add(new Literal() { Text = responseBuilder.ToString() });
        }
    }
}
