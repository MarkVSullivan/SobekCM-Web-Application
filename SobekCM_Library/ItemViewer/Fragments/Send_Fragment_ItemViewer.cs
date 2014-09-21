using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Fragments
{
    /// <summary> Fragment item viewer writes the send/email form to the stream, to be loaded dynamically
    /// when needed by an item viewer </summary>
    public class Send_Fragment_ItemViewer : baseFragment_ItemViewer
    {
        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Fragment_SendForm"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Fragment_SendForm; }
        }

        public override void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            if (CurrentUser != null)
            {
                StringBuilder responseBuilder = new StringBuilder();

                // Determine the number of columns for text areas, depending on browser
                int actual_cols = 50;
                if (CurrentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0)
                    actual_cols = 45;

                responseBuilder.AppendLine("<!-- Email form -->");
				responseBuilder.AppendLine("<div id=\"emailform_content\" class=\"sbk_PopupForm\">");
				responseBuilder.AppendLine("  <div class=\"sbk_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left;\">Send this Item to a Friend</td><td style=\"text-align:right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"email_form_close()\">X</a> &nbsp; </td></tr></table></div>");
                responseBuilder.AppendLine("  <br />");
                responseBuilder.AppendLine("  <fieldset><legend>Enter the email information below &nbsp; </legend>");
                responseBuilder.AppendLine("    <br />");
				responseBuilder.AppendLine("    <table class=\"sbk_PopupTable\">");


                // Add email address line
                responseBuilder.Append("      <tr><td style=\"width:80px\"><label for=\"email_address\">To:</label></td>");
				responseBuilder.AppendLine("<td><input class=\"email_input sbk_Focusable\" name=\"email_address\" id=\"email_address\" type=\"text\" value=\"" + CurrentUser.Email + "\" /></td></tr>");

                // Add comments area
                responseBuilder.Append("      <tr style=\"vertical-align:top\"><td><br /><label for=\"email_comments\">Comments:</label></td>");
				responseBuilder.AppendLine("<td><textarea rows=\"6\" cols=\"" + actual_cols + "\" name=\"email_comments\" id=\"email_comments\" class=\"email_textarea sbk_Focusable\" ></textarea></td></tr>");

                // Add format area
				responseBuilder.Append("      <tr style=\"vertical-align:top\"><td>Format:</td>");
                responseBuilder.Append("<td><input type=\"radio\" name=\"email_format\" id=\"email_format_html\" value=\"html\" checked=\"checked\" /> <label for=\"email_format_html\">HTML</label> &nbsp; &nbsp; ");
                responseBuilder.AppendLine("<input type=\"radio\" name=\"email_format\" id=\"email_format_text\" value=\"text\" /> <label for=\"email_format_text\">Plain Text</label></td></tr>");

                responseBuilder.AppendLine("    </table>");
                responseBuilder.AppendLine("    <br />");
                responseBuilder.AppendLine("  </fieldset><br />");
	            responseBuilder.AppendLine("  <div style=\"text-align:center; font-size:1.3em;\">");
	            responseBuilder.AppendLine("    <button title=\"Send\" class=\"roundbutton\" onclick=\"return email_form_close();\"> CANCEL </button> &nbsp; &nbsp; ");
				responseBuilder.AppendLine("    <button title=\"Send\" class=\"roundbutton\" type=\"submit\"> SEND </button>");
                responseBuilder.AppendLine("  </div><br />");
				responseBuilder.AppendLine("</div>");
                responseBuilder.AppendLine();

                MainPlaceHolder.Controls.Add(new Literal() { Text = responseBuilder.ToString() });
            }
        }
    }
}
