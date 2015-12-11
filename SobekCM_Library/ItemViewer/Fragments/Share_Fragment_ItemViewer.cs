#region Using directives

using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.UI_Configuration;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Tools;

#endregion

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

        /// <summary> Add the main HTML for the main viewer, which is the bulk of the share fragment </summary>
        /// <param name="MainPlaceHolder">Main place holder ( "mainPlaceHolder" ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            StringBuilder responseBuilder = new StringBuilder();

            // Calculate the title and url
            string title = HttpUtility.HtmlEncode(CurrentItem.Bib_Info.Main_Title.Title);
            string share_url = CurrentMode.Base_URL + "/" + CurrentItem.BibID + "/" + CurrentItem.VID;
            if (HttpContext.Current != null)
                share_url = HttpContext.Current.Items["Original_URL"].ToString().Replace("&", "%26").Replace("?", "%3F").Replace("http://", "").Replace("=", "%3D").Replace("\"", "&quot;");

            responseBuilder.AppendLine("<!-- Share form -->");
            responseBuilder.AppendLine("<div id=\"shareform_content\">");

            responseBuilder.AppendLine("<a href=\"http://www.facebook.com/share.php?u=" + share_url + "&amp;t=" + title + "\" target=\"FACEBOOK_WINDOW\" onmouseover=\"facebook_share.src='" + Static_Resources.Facebook_Share_H_Gif + "'\" onfocus=\"facebook_share.src='" + Static_Resources.Facebook_Share_H_Gif + "'\" onmouseout=\"facebook_share.src='" + Static_Resources.Facebook_Share_Gif + "'\" onblur=\"facebook_share.src='" + Static_Resources.Facebook_Share_Gif + "'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"facebook_share\" name=\"facebook_share\" src=\"" + Static_Resources.Facebook_Share_Gif + "\" alt=\"FACEBOOK\" /></a>");
            responseBuilder.AppendLine("<a href=\"http://buzz.yahoo.com/buzz?targetUrl=" + share_url + "&amp;headline=" + title + "\" target=\"YAHOOBUZZ_WINDOW\" onmouseover=\"yahoobuzz_share.src='" + Static_Resources.Yahoobuzz_Share_H_Gif + "'\" onfocus=\"yahoobuzz_share.src='" + Static_Resources.Yahoobuzz_Share_H_Gif + "'\" onmouseout=\"yahoobuzz_share.src='" + Static_Resources.Yahoobuzz_Share_Gif + "'\" onblur=\"yahoobuzz_share.src='" + Static_Resources.Yahoobuzz_Share_Gif + "'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"yahoobuzz_share\" name=\"yahoobuzz_share\" src=\"" + Static_Resources.Yahoobuzz_Share_Gif + "\" alt=\"YAHOO BUZZ\" /></a>");
            responseBuilder.AppendLine("<br />");

            responseBuilder.AppendLine("<a href=\"http://twitter.com/home?status=Currently reading " + share_url + "\" target=\"TWITTER_WINDOW\" onmouseover=\"twitter_share.src='" + Static_Resources.Twitter_Share_H_Gif + "'\" onfocus=\"twitter_share.src='" + Static_Resources.Twitter_Share_H_Gif + "'\" onmouseout=\"twitter_share.src='" + Static_Resources.Twitter_Share_Gif + "'\" onblur=\"twitter_share.src='" + Static_Resources.Twitter_Share_Gif + "'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"twitter_share\" name=\"twitter_share\" src=\"" + Static_Resources.Twitter_Share_Gif + "\" alt=\"TWITTER\" /></a>");
            responseBuilder.AppendLine("<a href=\"http://www.google.com/bookmarks/mark?op=add&amp;bkmk=" + share_url + "&amp;title=" + title + "\" target=\"GOOGLE_WINDOW\" onmouseover=\"google_share.src='" + Static_Resources.Google_Share_H_Gif + "'\" onfocus=\"google_share.src='" + Static_Resources.Google_Share_H_Gif + "'\" onmouseout=\"google_share.src='" + Static_Resources.Google_Share_Gif + "'\" onblur=\"google_share.src='" + Static_Resources.Google_Share_Gif + "'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"google_share\" name=\"google_share\" src=\"" + Static_Resources.Google_Share_Gif + "\" alt=\"GOOGLE SHARE\" /></a>");
            responseBuilder.AppendLine("<br />");

            responseBuilder.AppendLine("<a href=\"http://www.stumbleupon.com/submit?url=" + share_url + "&amp;title=" + title + "\" target=\"STUMBLEUPON_WINDOW\" onmouseover=\"stumbleupon_share.src='" + Static_Resources.Stumbleupon_Share_H_Gif + "'\" onfocus=\"stumbleupon_share.src='" + Static_Resources.Stumbleupon_Share_H_Gif + "'\" onmouseout=\"stumbleupon_share.src='" + Static_Resources.Stumbleupon_Share_Gif + "'\" onblur=\"stumbleupon_share.src='" + Static_Resources.Stumbleupon_Share_Gif + "'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"stumbleupon_share\" name=\"stumbleupon_share\" src=\"" + Static_Resources.Stumbleupon_Share_Gif + "\" alt=\"STUMBLEUPON\" /></a>");
            responseBuilder.AppendLine("<a href=\"http://myweb.yahoo.com/myresults/bookmarklet?t=" + title + "&amp;u=" + share_url + "\" target=\"YAHOO_WINDOW\" onmouseover=\"yahoo_share.src='" + Static_Resources.Yahoo_Share_H_Gif + "'\" onfocus=\"yahoo_share.src='" + Static_Resources.Yahoo_Share_H_Gif + "'\" onmouseout=\"yahoo_share.src='" + Static_Resources.Yahoo_Share_Gif + "'\" onblur=\"yahoo_share.src='" + Static_Resources.Yahoo_Share_Gif + "'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"yahoo_share\" name=\"yahoo_share\" src=\"" + Static_Resources.Yahoo_Share_Gif + "\" alt=\"YAHOO SHARE\" /></a>");
            responseBuilder.AppendLine("<br />");

            responseBuilder.AppendLine("<a href=\"http://digg.com/submit?phase=2&amp;url=" + share_url + "&amp;title=" + title + "\" target=\"DIGG_WINDOW\" onmouseover=\"digg_share.src='" + Static_Resources.Digg_Share_H_Gif + "'\" onfocus=\"digg_share.src='" + Static_Resources.Digg_Share_H_Gif + "'\" onmouseout=\"digg_share.src='" + Static_Resources.Digg_Share_Gif + "'\" onblur=\"digg_share.src='" + Static_Resources.Digg_Share_Gif + "'\"  nclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"digg_share\" name=\"digg_share\" src=\"" + Static_Resources.Digg_Share_Gif + "\" alt=\"DIGG\" /></a>");
            responseBuilder.AppendLine("<a onmouseover=\"favorites_share.src='" + Static_Resources.Facebook_Share_H_Gif + "'\" onfocus=\"favorites_share.src='" + Static_Resources.Facebook_Share_H_Gif + "'\" onmouseout=\"favorites_share.src='" + Static_Resources.Facebook_Share_Gif + "'\" onblur=\"favorites_share.src='" + Static_Resources.Facebook_Share_Gif + "'\" onclick=\"javascript:add_to_favorites();\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"favorites_share\" name=\"favorites_share\" src=\"" + Static_Resources.Facebook_Share_Gif + "\" alt=\"MY FAVORITES\" /></a>");
            responseBuilder.AppendLine("<br />");

            responseBuilder.AppendLine("</div>");
            responseBuilder.AppendLine();

            MainPlaceHolder.Controls.Add(new Literal() { Text = responseBuilder.ToString() });
        }
    }
}
