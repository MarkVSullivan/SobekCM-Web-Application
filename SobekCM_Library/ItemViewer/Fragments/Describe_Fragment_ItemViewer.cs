#region Using directives

using System;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Fragments
{
    /// <summary> Fragment item viewer writes the describe form to the stream, to be loaded dynamically
    /// when needed by an item viewer </summary>
    public class Describe_Fragment_ItemViewer : baseFragment_ItemViewer
    {
        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Fragment_AddForm"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Fragment_AddForm; }
        }

        public override void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            if ((CurrentItem.Behaviors.Can_Be_Described) && (CurrentUser != null))
            {
                // Determine the number of columns for text areas, depending on browser
                int actual_cols = 50;
                if (( !String.IsNullOrEmpty(CurrentMode.Browser_Type)) && (CurrentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0))
                    actual_cols = 45;

                StringBuilder responseBuilder = new StringBuilder();
                responseBuilder.AppendLine("<!-- Add descriptive tage form  -->");
                responseBuilder.AppendLine("<div class=\"describe_popup_div\" id=\"describe_item_form\" style=\"display:none;\">");
                responseBuilder.AppendLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">A<span class=\"smaller\">DD </span> I<span class=\"smaller\">TEM </span> D<span class=\"smaller\">ESCRIPTION</span></td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"describe_item_form_close()\">X</a> &nbsp; </td></tr></table></div>");
                responseBuilder.AppendLine("  <br />");
                responseBuilder.AppendLine("  <fieldset><legend>Enter a description or notes to add to this item &nbsp; </legend>");
                responseBuilder.AppendLine("    <br />");
                responseBuilder.AppendLine("    <table class=\"popup_table\">");

                // Add comments area
                responseBuilder.Append("      <tr align=\"left\" valign=\"top\"><td><br /><label for=\"add_notes\">Notes:</label></td>");
                responseBuilder.AppendLine("<td><textarea rows=\"10\" cols=\"" + actual_cols + "\" name=\"add_tag\" id=\"add_tag\" class=\"add_notes_textarea\" onfocus=\"javascript:textbox_enter('add_tag','add_notes_textarea_focused')\" onblur=\"javascript:textbox_leave('add_tag','add_notes_textarea')\"></textarea></td></tr>");
                responseBuilder.AppendLine("    </table>");
                responseBuilder.AppendLine("    <br />");
                responseBuilder.AppendLine("  </fieldset><br />");
                responseBuilder.AppendLine("  <center><a href=\"\" onclick=\"return describe_item_form_close();\"><img border=\"0\" src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin_Or_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + CurrentMode.Base_URL + "design/skins/" + CurrentMode.Base_Skin_Or_Skin + "/buttons/save_button_g.gif\" value=\"Submit\" alt=\"Submit\" ></center><br />");
                responseBuilder.AppendLine("</div>");
                responseBuilder.AppendLine();

                MainPlaceHolder.Controls.Add(new Literal() { Text = responseBuilder.ToString() });
            }
        }
    }
}