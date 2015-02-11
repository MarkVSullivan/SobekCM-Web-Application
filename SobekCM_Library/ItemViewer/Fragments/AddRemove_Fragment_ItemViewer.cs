#region Using directives

using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.Users;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Fragments
{
    /// <summary> Fragment item viewer writes the bookshelf add/remove form to the stream, to be loaded dynamically
    /// when needed by an item viewer </summary>
    public class AddRemove_Fragment_ItemViewer : baseFragment_ItemViewer
    {
        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Fragment_AddForm"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Fragment_AddForm; }
        }

        public override void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            // Set caching on this
            HttpContext.Current.Response.Cache.SetCacheability( HttpCacheability.NoCache );

            if ((CurrentItem.Web.ItemID > 0) && (CurrentUser != null) && (!CurrentUser.Is_In_Bookshelf(CurrentItem.BibID, CurrentItem.VID)))
            {
                StringBuilder responseBuilder = new StringBuilder();

                // Determine the number of columns for text areas, depending on browser
                int actual_cols = 50;
                if (CurrentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0)
                    actual_cols = 45;

                responseBuilder.AppendLine("<!-- Add to bookshelf form -->");
				responseBuilder.AppendLine("<div id=\"addform_content\" class=\"sbk_PopupForm\" style=\"width:530px;\">");
				responseBuilder.AppendLine("  <div class=\"sbk_PopupTitle\"><table style=\"width:100%\"><tr><td style=\"text-align:left;\">Add this item to your Bookshelf</td><td style=\"text-align:right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"add_item_form_close()\">X</a> &nbsp; </td></tr></table></div>");
                responseBuilder.AppendLine("  <br />");
                responseBuilder.AppendLine("  <fieldset><legend>Enter notes for this item in your bookshelf &nbsp; </legend>");
                responseBuilder.AppendLine("    <br />");
				responseBuilder.AppendLine("    <table class=\"sbk_PopupTable\">");


                // Add bookshelf choices
                responseBuilder.Append("      <tr><td style=\"width:80px\"><label for=\"add_bookshelf\">Bookshelf:</label></td>");
                responseBuilder.Append("<td><select class=\"email_bookshelf_input\" name=\"add_bookshelf\" id=\"add_bookshelf\">");

                foreach (User_Folder folder in CurrentUser.All_Folders)
                {
                    if (folder.Folder_Name.Length > 80)
                    {
                        responseBuilder.Append("<option value=\"" + HttpUtility.HtmlEncode(folder.Folder_Name) + "\">" + HttpUtility.HtmlEncode(folder.Folder_Name.Substring(0, 75)) + "...</option>");
                    }
                    else
                    {
                        if (folder.Folder_Name != "Submitted Items")
                        {
                            if (folder.Folder_Name == "My Bookshelf")
                                responseBuilder.Append("<option value=\"" + HttpUtility.HtmlEncode(folder.Folder_Name) + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(folder.Folder_Name) + "</option>");
                            else
                                responseBuilder.Append("<option value=\"" + HttpUtility.HtmlEncode(folder.Folder_Name) + "\">" + HttpUtility.HtmlEncode(folder.Folder_Name) + "</option>");
                        }
                    }
                }
                responseBuilder.AppendLine("</select></td></tr>");

                // Add comments area
                responseBuilder.Append("      <tr style=\"vertical-align:top\"><td><br /><label for=\"add_notes\">Notes:</label></td>");
                responseBuilder.AppendLine("<td><textarea rows=\"6\" cols=\"" + actual_cols + "\" name=\"add_notes\" id=\"add_notes\" class=\"add_notes_textarea\" onfocus=\"javascript:textbox_enter('add_notes','add_notes_textarea_focused')\" onblur=\"javascript:textbox_leave('add_notes','add_notes_textarea')\"></textarea></td></tr>");
				responseBuilder.AppendLine("      <tr style=\"vertical-align:top\"><td>&nbsp;</td><td><input type=\"checkbox\" id=\"open_bookshelf\" name=\"open_bookshelf\" value=\"open\" /> <label for=\"open_bookshelf\">Open bookshelf in new window</label></td></tr>");
                responseBuilder.AppendLine("    </table>");
                responseBuilder.AppendLine("    <br />");
                responseBuilder.AppendLine("  </fieldset><br />");
				responseBuilder.AppendLine("  <div style=\"text-align:center; font-size:1.3em;\">");
				responseBuilder.AppendLine("    <button title=\"Cancel\" class=\"roundbutton\" onclick=\"return add_item_form_close();\"> CANCEL </button> &nbsp; &nbsp; ");
				responseBuilder.AppendLine("    <button title=\"Send\" class=\"roundbutton\" type=\"submit\"> SAVE </button>");
				responseBuilder.AppendLine("  </div><br />");
                responseBuilder.AppendLine("</div>");
                responseBuilder.AppendLine();

                MainPlaceHolder.Controls.Add(new Literal() { Text = responseBuilder.ToString() });
            }
        }
    }
}
