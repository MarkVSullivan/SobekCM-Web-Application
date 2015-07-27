using System.IO;
using SobekCM.Core.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Tools;

namespace SobekCM.Library.WebContentViewer.Viewers
{
    /// <summary> Web content viewer shows a managament menu with all the options for the current user </summary>
    /// <remarks> This viewer extends the <see cref="abstractWebContentViewer" /> abstract class and implements the <see cref="iWebContentViewer"/> interface. </remarks>
    public class Manage_Menu_WebContentViewer : abstractWebContentViewer
    {
        /// <summary> Constructor for a new instance of the Manage_Menu_WebContentViewer class </summary>
        /// <param name="RequestSpecificValues">  All the necessary, non-global data specific to the current request  </param>
        public Manage_Menu_WebContentViewer(RequestCache RequestSpecificValues) : base ( RequestSpecificValues )
        {

        }

        /// <summary> Gets the type of specialized web content viewer </summary>
        public override WebContent_Type_Enum Type
        {
            get { return WebContent_Type_Enum.Manage_Menu; }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        public override string Viewer_Title
        {
            get { return "Web Content Management Menu"; }
        }

        /// <summary> Gets the URL for the icon related to this web content viewer task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources.Manage_Collection_Img; }
        }

        /// <summary> Add the HTML to be displayed </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Manage_Menu_WebContentViewer.Add_HTML", "Start to add the html content");
            }

            Output.WriteLine("  <table id=\"sbkMmav_MainTable\">");

            string type1 = "web content page";
            string type2 = "Web Content Page";

            Output.WriteLine("    <tr class=\"sbkMmav_HeaderRow\"><td colspan=\"3\">How would you like to manage this " + type1 + " today?</td></tr>");


            // Collect all the URLs
            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Simple_HTML_CMS;
            RequestSpecificValues.Current_Mode.WebContent_Type = WebContent_Type_Enum.Display;
            string view_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            RequestSpecificValues.Current_Mode.WebContent_Type = WebContent_Type_Enum.Usage;
            string usage_stats_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            RequestSpecificValues.Current_Mode.WebContent_Type = WebContent_Type_Enum.Milestones;
            string history_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            RequestSpecificValues.Current_Mode.WebContent_Type = WebContent_Type_Enum.Permissions;
            string permissions_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            RequestSpecificValues.Current_Mode.WebContent_Type = WebContent_Type_Enum.Delete_Verify;
            string delete_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);


            // Add admin view is system administrator
            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
            RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.WebContent_Single;
            string admin_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Simple_HTML_CMS;

            // Add the link for viewing this
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("      <td style=\"width:60px\"><a href=\"" + view_url + "\"><img src=\"" + Static_Resources.WebContent_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + view_url + "\">View " + type2 + "</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View this web content page or global redirect, as it will be viewed by public, non-authenticated users.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr class=\"sbkMmav_SpacerRow\"><td colspan=\"3\"></td></tr>");

            // Add the link for the usage stats
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("      <td style=\"width:60px\"><a href=\"" + usage_stats_url + "\"><img src=\"" + Static_Resources.Usage_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + usage_stats_url + "\">View History of Use</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View ths usage statistics for this web content page or global redirect.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr class=\"sbkMmav_SpacerRow\"><td colspan=\"3\"></td></tr>");
            
            // Add the link for aggregation management
            if ((RequestSpecificValues.Current_User.Is_System_Admin) || (RequestSpecificValues.Current_User.Is_Portal_Admin))
            {

                // Add the link for the work log history
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td style=\"width:50px\">&nbsp;</td>");
                Output.WriteLine("      <td style=\"width:60px\"><a href=\"" + history_url + "\"><img src=\"" + Static_Resources.View_Work_Log_Img_Large + "\" /></a></td>");
                Output.WriteLine("      <td>");
                Output.WriteLine("        <a href=\"" + history_url + "\">View Change Log</a>");
                Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the change log for this web content page or global redirect and the design files under this collection.</div>");
                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr class=\"sbkMmav_SpacerRow\"><td colspan=\"3\"></td></tr>");

                // Add the link for the user permissions
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td style=\"width:50px\">&nbsp;</td>");
                Output.WriteLine("      <td style=\"width:60px\"><a href=\"" + permissions_url + "\"><img src=\"" + Static_Resources.User_Permission_Img_Large + "\" /></a></td>");
                Output.WriteLine("      <td>");
                Output.WriteLine("        <a href=\"" + permissions_url + "\">View User Permissions</a>");
                Output.WriteLine("        <div class=\"sbkMmav_Desc\">View special user permissions granted to users over this web content page or global redirect.  This includes permissions assigned individually, as well as permissions assigned through user groups.</div>");
                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr class=\"sbkMmav_SpacerRow\"><td colspan=\"3\"></td></tr>");

                // Add the link to delete this web page
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td style=\"width:50px\">&nbsp;</td>");
                Output.WriteLine("      <td style=\"width:60px\"><a href=\"" + delete_url + "\"><img src=\"" + Static_Resources.Delete_Item_Icon_Png + "\" /></a></td>");
                Output.WriteLine("      <td>");
                Output.WriteLine("        <a href=\"" + delete_url + "\">Delete " + type2 + "</a>");
                Output.WriteLine("        <div class=\"sbkMmav_Desc\">Delete this web content page or global redirect from the system entirely.</div>");
                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr class=\"sbkMmav_SpacerRow\"><td colspan=\"3\"></td></tr>");

                // Add the link to administer
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td style=\"width:50px\">&nbsp;</td>");
                Output.WriteLine("      <td style=\"width:60px\"><a href=\"" + admin_url + "\"><img src=\"" + Static_Resources.Admin_View_Img_Large + "\" /></a></td>");
                Output.WriteLine("      <td>");
                Output.WriteLine("        <a href=\"" + admin_url + "\">Web Content Administration</a>");
                Output.WriteLine("        <div class=\"sbkMmav_Desc\">Perform administrative duties against this web content page or global redirect, changing the appearance, viewing the child list, and managing files related to this page.</div>");
                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr class=\"sbkMmav_SpacerRow\"><td colspan=\"3\"></td></tr>");
            }

            Output.WriteLine("  </table>");

            if (Tracer != null)
            {
                Tracer.Add_Trace("Manage_Menu_WebContentViewer.Add_HTML", "Done adding the html content");
            }
        }
    }
}
