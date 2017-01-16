using System;
using System.Collections.Generic;
using System.IO;
using SobekCM.Core.BriefItem;
using SobekCM.Library.HTML;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.UI;

namespace SobekCM.Library.ItemViewer.HtmlSectionWriters
{
    /// <summary> Item HTML section writer adds the standard title bar to the item display </summary>
    public class TitleBar_ItemSectionWriter : iItemSectionWriter
    {
        /// <summary> Write the item title bar to the item display html</summary>
        /// <param name="Output"> Stream to which to write </param>
        /// <param name="Prototyper"> Current item viewer prototyper </param>
        /// <param name="CurrentViewer"> Current item viewer which will be used to fill the primary part of the page </param>
        /// <param name="CurrentItem"> Current item which is being displayed </param>
        /// <param name="RequestSpecificValues"> Other, request specific values, such as the current mode, user, etc.. </param>
        /// <param name="Behaviors"> Behaviors for the current view and situation </param>
        public void Write_HTML(TextWriter Output, iItemViewerPrototyper Prototyper, iItemViewer CurrentViewer, BriefItemInfo CurrentItem, RequestCache RequestSpecificValues, List<HtmlSubwriter_Behaviors_Enum> Behaviors)
        {
            // The item viewer can choose to override the standard item titlebar
            if (!Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Titlebar))
            {
                Output.WriteLine("<!-- Show the title and any other important item information -->");
                Output.WriteLine("<section id=\"sbkIsw_Titlebar\" role=\"banner\">");

                if (String.Compare(CurrentItem.VID, "00000", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    string grouptitle = "NO TITLE";
                    if ((CurrentItem.Behaviors != null) && (!String.IsNullOrWhiteSpace(CurrentItem.Behaviors.GroupTitle)))
                        grouptitle = CurrentItem.Behaviors.GroupTitle;
                    if (grouptitle.Length > 125)
                    {
                        Output.WriteLine("\t<h1 itemprop=\"name\"><abbr title=\"" + grouptitle + "\">" + grouptitle.Substring(0, 120) + "..</abbr></h1>");
                    }
                    else
                    {
                        Output.WriteLine("\t<h1 itemprop=\"name\">" + grouptitle + "</h1>");
                    }
                }
                else
                {
                    string final_title = CurrentItem.Title ?? "NO TITLE";

                    // Add the Title if there is one
                    if (final_title.Length > 0)
                    {
                        // Is this a newspaper?
                        bool newspaper = (String.Compare(CurrentItem.Behaviors.GroupType, "NEWSPAPER", StringComparison.OrdinalIgnoreCase) == 0);

                        // Does a custom setting override the default behavior to add a date?
                        if ((newspaper) && (UI_ApplicationCache_Gateway.Settings.Contains_Additional_Setting("Item Viewer.Include Date In Title")) && (UI_ApplicationCache_Gateway.Settings.Get_Additional_Setting("Item Viewer.Include Date In Title").ToUpper() == "NEVER"))
                            newspaper = false;

                        // Add the date if it should be added
                        if ((newspaper) && (!String.IsNullOrEmpty(CurrentItem.Web.Date)))
                        {
                            if (final_title.Length > 125)
                            {
                                Output.WriteLine("\t<h1 itemprop=\"name\"><abbr title=\"" + final_title + "\">" + final_title.Substring(0, 120) + "..</abbr> ( " + CurrentItem.Web.Date + " )</h1>");
                            }
                            else
                            {
                                Output.WriteLine("\t<h1 itemprop=\"name\">" + final_title + " ( " + CurrentItem.Web.Date + " )</h1>");
                            }
                        }
                        else
                        {
                            if (final_title.Length > 125)
                            {
                                Output.WriteLine("\t<h1 itemprop=\"name\"><abbr title=\"" + final_title + "\">" + final_title.Substring(0, 120) + "..</abbr></h1>");
                            }
                            else
                            {
                                Output.WriteLine("\t<h1 itemprop=\"name\">" + final_title + "</h1>");
                            }
                        }
                    }


                    // Add the link if there is one  
                    // Links_BriefItemMapper
                    if ((CurrentItem.Web != null) && (!String.IsNullOrEmpty(CurrentItem.Web.Title_Box_Additional_Link)))
                    {
                        // Get the translated TYPE
                        string type = UI_ApplicationCache_Gateway.Translation.Get_Translation((CurrentItem.Web.Title_Box_Additional_Link_Type ?? "Related Link"), RequestSpecificValues.Current_Mode.Language);

                        // Add the link
                        Output.WriteLine("\t" + CurrentItem.Web.Title_Box_Additional_Link + " ( " + type + " )<br />");
                    }


                    // If there is an ACCESSION number and this is an ARTIFACT, include that at the top
                    BriefItem_DescriptiveTerm accessNumber = CurrentItem.Get_Description("Accession Number");
                    if ((accessNumber != null) && (accessNumber.Values != null) && (accessNumber.Values.Count > 0))
                    {
                        Output.WriteLine("\t" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Accession number", RequestSpecificValues.Current_Mode.Language) + " " + accessNumber.Values[0].Value + "<br />");
                    }
                }


                Output.WriteLine("</section>");
                Output.WriteLine();
            }
        }
    }
}
