#region Using directives

using System.IO;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Aggregation viewer shows all the options that a logged in user has for managing the current
    /// aggregation </summary>
    public class Manage_Menu_AggregationViewer : abstractAggregationViewer
    {
        /// <summary> Constructor for a new instance of the Manage_Menu_AggregationViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="ViewBag"> Aggregation-specific request information, such as aggregation object and any browse object requested </param>
        public Manage_Menu_AggregationViewer(RequestCache RequestSpecificValues, AggregationViewBag ViewBag)
            : base(RequestSpecificValues, ViewBag)
        {
            // User must AT LEAST be logged on, return
            if ((RequestSpecificValues.Current_User == null) || (!RequestSpecificValues.Current_User.LoggedOn))
            {
                RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
            }
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        public override Item_Aggregation_Views_Searches_Enum Type
        {
            get { return Item_Aggregation_Views_Searches_Enum.Manage_Menu; }
        }

        /// <summary> Flag which indicates whether the selection panel should be displayed </summary>
        /// <value> This defaults to <see cref="Selection_Panel_Display_Enum.Selectable"/> but is overwritten by most collection viewers </value>
        public override Selection_Panel_Display_Enum Selection_Panel_Display
        {
            get { return Selection_Panel_Display_Enum.Never; }
        }

        /// <summary> Gets flag which indicates whether this is an internal view, which may have a 
        /// slightly different design feel </summary>
        /// <remarks> This returns FALSE by default, but can be overriden by individual viewer implementations</remarks>
        public override bool Is_Internal_View
        {
            get { return true; }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        public override string Viewer_Title
        {
            get { return "Aggregation Management Menu"; }
        }

        /// <summary> Gets the URL for the icon related to this aggregational viewer task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources.Manage_Collection_Img; }
        }

        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This does nothing - as an internal type view, this will not be called </remarks>
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            // Do nothing
        }

        /// <summary> Add the main HTML to be added to the page </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This writes the HTML from the static browse or info page here  </remarks>
        public override void Add_Secondary_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
          
            bool isAll = (RequestSpecificValues.Current_Mode.Aggregation.Length == 0) || (RequestSpecificValues.Current_Mode.Aggregation.ToUpper() == "ALL");




            Output.WriteLine("  <table id=\"sbkMmav_MainTable\">");
            Output.WriteLine("    <tr class=\"sbkMmav_HeaderRow\"><td colspan=\"3\">How would you like to manage this collection today?</td></tr>");


            // Collect all the URLs
            RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Private_Items;
            string private_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Item_Count;
            string item_count_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Usage_Statistics;
            string usage_stats_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Work_History;
            string history_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.User_Permissions;
            string permissions_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            string browseby_url = RequestSpecificValues.Current_Mode.Base_URL + "/l/" + ViewBag.Hierarchy_Object.Code + "/browseby";

            // Special links if this is the ALL collection
            if (isAll)
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Statistics;

                // Add button to view item count information
                RequestSpecificValues.Current_Mode.Statistics_Type = Statistics_Type_Enum.Item_Count_Standard_View;
                item_count_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

                // Add button to view usage statistics information
                RequestSpecificValues.Current_Mode.Statistics_Type = Statistics_Type_Enum.Usage_Overall;
                usage_stats_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;

            }

            // Add admin view is system administrator
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aggregation_Single;
                string admin_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;

            // Add the link for the private and dark items
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("      <td style=\"width:60px\"><a href=\"" + private_url + "\"><img src=\"" + Static_Resources.Private_Items_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + private_url + "\">View Private and Dark Items</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the private and dark items which are a part of this collection, along with the last milestone information for each item.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr class=\"sbkMmav_SpacerRow\"><td colspan=\"3\"></td></tr>");

            // Add the link for the item count
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("      <td style=\"width:60px\"><a href=\"" + item_count_url + "\"><img src=\"" + Static_Resources.Item_Count_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + item_count_url + "\">View Item Count in Collection</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the total item count within this collection, as well as a count of how many items are in currently in process within each major milestone.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr class=\"sbkMmav_SpacerRow\"><td colspan=\"3\"></td></tr>");

            // Add the link for the usage stats
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td style=\"width:50px\">&nbsp;</td>");
            Output.WriteLine("      <td style=\"width:60px\"><a href=\"" + usage_stats_url + "\"><img src=\"" + Static_Resources.Usage_Img_Large + "\" /></a></td>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        <a href=\"" + usage_stats_url + "\">View History of Collection Use</a>");
            Output.WriteLine("        <div class=\"sbkMmav_Desc\">View ths usage statistics for this collection, and for all items within this collection.</div>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr class=\"sbkMmav_SpacerRow\"><td colspan=\"3\"></td></tr>");

            // Add the metadata browse option
            if (!isAll)
            {
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td style=\"width:50px\">&nbsp;</td>");
                Output.WriteLine("      <td style=\"width:60px\"><a href=\"" + browseby_url + "\"><img src=\"" + Static_Resources.Metadata_Browse_Img_Large + "\" /></a></td>");
                Output.WriteLine("      <td>");
                Output.WriteLine("        <a href=\"" + browseby_url + "\">View Metadata Browses</a>");
                Output.WriteLine("        <div class=\"sbkMmav_Desc\">View public and/or administrative metadata browses with the complete list of all metadata in the searchable metadata fields for this collection.</div>");
                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr class=\"sbkMmav_SpacerRow\"><td colspan=\"3\"></td></tr>");
            }


            // Add the link for aggregation management
            if ((RequestSpecificValues.Current_User.Is_System_Admin) || (RequestSpecificValues.Current_User.Is_Portal_Admin) || (RequestSpecificValues.Current_User.Is_Aggregation_Curator(ViewBag.Hierarchy_Object.Code)))
            {

                // Add the link for the work log history
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td style=\"width:50px\">&nbsp;</td>");
                Output.WriteLine("      <td style=\"width:60px\"><a href=\"" + history_url + "\"><img src=\"" + Static_Resources.View_Work_Log_Img_Large + "\" /></a></td>");
                Output.WriteLine("      <td>");
                Output.WriteLine("        <a href=\"" + history_url + "\">View Collection Change Log</a>");
                Output.WriteLine("        <div class=\"sbkMmav_Desc\">View the change log for this collection and the design files under this collection.  This does not include the history of digital reources loaded into this collection.</div>");
                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr class=\"sbkMmav_SpacerRow\"><td colspan=\"3\"></td></tr>");

                // Add the link for the user permissions
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td style=\"width:50px\">&nbsp;</td>");
                Output.WriteLine("      <td style=\"width:60px\"><a href=\"" + permissions_url + "\"><img src=\"" + Static_Resources.User_Permission_Img_Large + "\" /></a></td>");
                Output.WriteLine("      <td>");
                Output.WriteLine("        <a href=\"" + permissions_url + "\">View User Permissions</a>");
                Output.WriteLine("        <div class=\"sbkMmav_Desc\">View special user permissions granted to users over this collection.  This includes permissions assigned individually, as well as permissions assigned through user groups.</div>");
                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr class=\"sbkMmav_SpacerRow\"><td colspan=\"3\"></td></tr>");


                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td style=\"width:50px\">&nbsp;</td>");
                Output.WriteLine("      <td style=\"width:60px\"><a href=\"" + admin_url + "\"><img src=\"" + Static_Resources.Admin_View_Img_Large + "\" /></a></td>");
                Output.WriteLine("      <td>");
                Output.WriteLine("        <a href=\"" + admin_url + "\">Collection Administration</a>");
                Output.WriteLine("        <div class=\"sbkMmav_Desc\">Perform administrative duties against this collection, changing the appearance, facets, results types, and much more.</div>");
                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr class=\"sbkMmav_SpacerRow\"><td colspan=\"3\"></td></tr>");
            }

            Output.WriteLine("  </table>");
        }
    }
}
