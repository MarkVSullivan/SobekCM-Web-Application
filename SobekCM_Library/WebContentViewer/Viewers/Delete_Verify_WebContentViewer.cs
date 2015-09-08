using System;
using System.IO;
using SobekCM.Core.Client;
using SobekCM.Core.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Tools;

namespace SobekCM.Library.WebContentViewer.Viewers
{
    /// <summary> Web content viewer requests verification to delete a web content page or redirect before it is deleted </summary>
    /// <remarks> This viewer extends the <see cref="abstractWebContentViewer" /> abstract class and implements the <see cref="iWebContentViewer"/> interface. </remarks>
    public class Delete_Verify_WebContentViewer : abstractWebContentViewer
    {
        private string ErrorMessage;

        /// <summary>  Constructor for a new instance of the Delete_Verify_WebContentViewer class  </summary>
        /// <param name="RequestSpecificValues">  All the necessary, non-global data specific to the current request  </param>
        public Delete_Verify_WebContentViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            ErrorMessage = SobekEngineClient.WebContent.Delete_HTML_Based_Content(RequestSpecificValues.Static_Web_Content.WebContentID.Value, "TEST", RequestSpecificValues.Tracer);

        }


        /// <summary> Gets the type of specialized web content viewer </summary>
        public override WebContent_Type_Enum Type { get { return WebContent_Type_Enum.Delete_Verify; }}

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        public override string Viewer_Title
        {
            get { return "Verify Web Content Deletion"; }
        }

        /// <summary> Gets the URL for the icon related to this web content viewer task </summary>
        public override string Viewer_Icon
        {
            get { return null; }
        }

        /// <summary> Add the HTML to be displayed </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Delete_Verify_WebContentViewer.Add_HTML", "No html added");
            }

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_delete_item\" name=\"admin_delete_item\" value=\"\" />");
            Output.WriteLine();

            if (!String.IsNullOrEmpty(ErrorMessage))
            {
                Output.WriteLine("<h1>Msg: " + ErrorMessage + "</h1>");
            }

            Output.WriteLine("<div class=\"Wchs_Text\">");
            Output.WriteLine("  <p>This form allows you to delete a web content page from the system.  The source files will remain, but the page or redirect will be removed from the system.</p>");
            Output.WriteLine();
            Output.WriteLine("  <table id=\"sbkWchs_DeleteTable\">");
            Output.WriteLine("    <tr><td>Title: &nbsp; </td><td>" + RequestSpecificValues.Static_Web_Content.Title + "</td></tr>");
            string url = RequestSpecificValues.Static_Web_Content.URL(RequestSpecificValues.Current_Mode.Base_URL);
            Output.WriteLine("    <tr><td>URL:</td><td><a href=\"" + url + "\">" + url + "</a></td></tr>");
            Output.WriteLine("  </table>");
            Output.WriteLine();
            Output.WriteLine("  <p>Enter DELETE in the textbox below and select GO to complete this deletion.</p>");
            Output.WriteLine("  <div id=\"sbkWchs_DeleteVerifyDiv\">");
            Output.WriteLine("    <input class=\"sbkDimv_input sbk_Focusable\" name=\"admin_delete_confirm\" id=\"admin_delete_confirm\" type=\"text\" value=\"\" /> &nbsp; &nbsp; ");
            Output.WriteLine("    <button title=\"Confirm delete of this page\" class=\"roundbutton\" onclick=\"delete_item(); return false;\">CONFIRM <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
            Output.WriteLine("  </div>");
            Output.WriteLine("</div>");

            Output.WriteLine();
            Output.WriteLine("<!-- Focus on confirm box -->");
            Output.WriteLine("<script type=\"text/javascript\">focus_element('admin_delete_confirm');</script>");
            Output.WriteLine();
        }
    }
}
