using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Library.HTML;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.UI;

namespace SobekCM.Library.ItemViewer.HtmlSectionWriters
{
    /// <summary> Item HTML section writer adds the item table of contents to the item display </summary>
    public class TOC_ItemSectionWriter : iItemSectionWriter
    {
        /// <summary> Write the table of contents to the item display html</summary>
        /// <param name="Output"> Stream to which to write </param>
        /// <param name="Prototyper"> Current item viewer prototyper </param>
        /// <param name="CurrentViewer"> Current item viewer which will be used to fill the primary part of the page </param>
        /// <param name="CurrentItem"> Current item which is being displayed </param>
        /// <param name="RequestSpecificValues"> Other, request specific values, such as the current mode, user, etc.. </param>
        /// <param name="Behaviors"> Behaviors for the current view and situation </param>
        public void Write_HTML(TextWriter Output, iItemViewerPrototyper Prototyper, iItemViewer CurrentViewer, BriefItemInfo CurrentItem, RequestCache RequestSpecificValues, List<HtmlSubwriter_Behaviors_Enum> Behaviors)
        {
            // If there is no TOC, just return
            if ((CurrentItem == null) || (CurrentItem.Images_TOC == null) || (CurrentItem.Images_TOC.Count <= 1))
                return;

            string table_of_contents = "TABLE OF CONTENTS";
            //string hide_toc = "HIDE TABLE OF CONTENTS";
            //string show_toc_text = "SHOW TABLE OF CONTENTS";

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
            {
                table_of_contents = "TABLE DES MATIERES";
                //hide_toc = "MASQUER L'INDEX";
                //show_toc_text = "VOIR L'INDEX";
            }

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
            {
                table_of_contents = "INDICE";
                //hide_toc = "ESCONDA INDICE";
                //show_toc_text = "MOSTRAR INDICE";
            }

            // Get the item URL
            string item_url = RequestSpecificValues.Current_Mode.Base_URL + CurrentItem.BibID + "/" + CurrentItem.VID;
            string viewer_code = RequestSpecificValues.Current_Mode.ViewerCode.Replace(RequestSpecificValues.Current_Mode.Page.ToString(), "#");
            if (viewer_code.IndexOf("#") < 0)
                viewer_code = "#";

            Output.WriteLine();
            Output.WriteLine("  <script type=\"text/javascript\" src=\"" + UI_ApplicationCache_Gateway.Configuration.UI.StaticResources.Jstree_Js + "\"></script>");            Output.WriteLine("  <div class=\"sbkIsw_ShowTocRow\">" + table_of_contents + "</div>");
            Output.WriteLine("  <div id=\"tocTree\" class=\"sbkIsw_TocTreeView\">");
            Output.WriteLine("    <ul>");

            List<BriefItem_TocElement> tocElements = CurrentItem.Images_TOC;
            int lastLevel = -1;
            int currentLevel = 0;
            int nextLevel = 0;
            int currentSequence = 0;
            int nextSequence = 0;
            int selectSequence = Convert.ToInt32(RequestSpecificValues.Current_Mode.Page);

            //int sequence = 0;
            for (int i = 0; i < tocElements.Count ; i++ )
            {
                // Get this level and sequence
                currentLevel = tocElements[i].Level.HasValue ? tocElements[i].Level.Value : 1;
                currentSequence = tocElements[i].Sequence;

                // Get the next level and sequence
                if (i + 1 < tocElements.Count)
                {
                    nextSequence = tocElements[i + 1].Sequence;
                }
                else
                {
                    nextSequence = selectSequence + 1;
                }

                // If this is not the first, then look to close any previous ones
                if (i > 0)
                {
                    if (lastLevel == currentLevel)
                    {
                        Output.WriteLine("</li>");
                    }
                    else if (lastLevel < currentLevel)
                    {
                        Output.WriteLine();
                        Output.WriteLine(indent(lastLevel) + "  <ul>");
                    }
                    else // lastLevel > currentLevel
                    {
                        Output.WriteLine("</li>");

                        while (lastLevel > currentLevel)
                        {
                            lastLevel--;

                            Output.WriteLine(indent(lastLevel) + "  </ul>");
                            Output.WriteLine(indent(lastLevel) + "</li>");
                        }
                    }
                }

                // Write this one
                if (Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_TOC_Links))
                {
                    Output.Write(indent(currentLevel) + "<li><abbr title=\"" + tocElements[i].Name.Replace("\"","'") + "\">" + tocElements[i].Shortened_Name);
                }
                else
                {
                    string page_url = item_url + "/" + viewer_code.Replace("#", currentSequence.ToString());

                    Output.Write(indent(currentLevel) + "<li><a href=\"" + page_url + "\" title=\"" + tocElements[i].Name + "\" >");

                    // Is this the current location?
                    if ((selectSequence >= currentSequence) && (selectSequence < nextSequence))
                    {
                        Output.Write("<span class=\"sbkIsw_SelectedTocTreeViewItem\">" + tocElements[i].Shortened_Name + "</span>");
                    }
                    else
                    {
                        Output.Write(tocElements[i].Shortened_Name);
                    }
                    Output.Write("</a>");
                }
                

                lastLevel = currentLevel;
            }

            // Close this
            Output.WriteLine("</li>");
            while (lastLevel > 1)
            {
                lastLevel--;

                Output.WriteLine(indent(lastLevel) + "  </ul>");
                Output.WriteLine(indent(lastLevel) + "</li>");
            }


            Output.WriteLine("    </ul>");
            Output.WriteLine("  </div>");
            Output.WriteLine();

            Output.WriteLine("  <script type=\"text/javascript\">");
            Output.WriteLine("    $('#tocTree').jstree( {\"core\": { \"themes\":{ \"icons\":false } } }).bind(\"select_node.jstree\", function (e, data) { var href = data.node.a_attr.href; document.location.href = href; });");
            Output.WriteLine("    $('#tocTree').jstree('open_all');");
            Output.WriteLine("  </script>");
            Output.WriteLine();



        }

        private string indent(int Level)
        {
            switch (Level)
            {
                case 0:
                    return String.Empty;

                case 1:
                    return "      ";

                case 2:
                    return "          ";

                case 3:
                    return "              ";

                case 4:
                    return "                  ";

                default:
                    StringBuilder builder = new StringBuilder("      ");
                    for (int i = Level; i > 1; i--)
                        builder.Append("    ");
                    return builder.ToString();

            }
        }
    }
}
