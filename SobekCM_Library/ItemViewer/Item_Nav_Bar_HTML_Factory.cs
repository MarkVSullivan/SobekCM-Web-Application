#region Using directives

using System.Collections.Generic;
using SobekCM.Bib_Package.SobekCM_Info;
using SobekCM.Library.Application_State;
using SobekCM.Library.Navigation;

#endregion

namespace SobekCM.Library.ItemViewer
{
    /// <summary> Class is used to generate the HTML for the nav bar in SobekCM at the item level </summary>
    public class Item_Nav_Bar_HTML_Factory
    {
        /// <summary> Get the navigation bar html for a view, given information about the current request </summary>
        /// <param name="Item_View"> View for which to generate the html </param>
        /// <param name="Resource_Type"> Current resource type, which determines the text in several viewer's tabs</param>
        /// <param name="Skin_Code"> Code for the current web sking, which determines which tab images to use </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Page_Sequence"> Current page sequence </param>
        /// <param name="Translator"> Language support object provides support for translating common user interface elements, like the names of these tabs </param>
        /// <param name="Show_Zoomable"> Flag indicates if the zoomable server is online and should be displayable </param>
        /// <returns> Collection of the html for the navigation bar (one view could have multiple tabs)</returns>
        public static List<string> Get_Nav_Bar_HTML(View_Object Item_View, string Resource_Type, 
            string Skin_Code, SobekCM_Navigation_Object Current_Mode, int Page_Sequence,
            Language_Support_Info Translator, bool Show_Zoomable )
        {
            List<string> returnVal = new List<string>();

            switch (Item_View.View_Type)
            {
                case View_Enum.ALL_VOLUMES:
                    string allVolumeCode = "allvolumes";
                    string resource_type_upper = Resource_Type.ToUpper();
                    if (Current_Mode.ViewerCode.IndexOf("allvolumes") == 0)
                        allVolumeCode = Current_Mode.ViewerCode;
                    if (resource_type_upper.IndexOf("NEWSPAPER") >= 0)
                    {
                        returnVal.Add(HTML_Helper(Skin_Code, allVolumeCode, Translator.Get_Translation("ALL ISSUES", Current_Mode.Language), Current_Mode));
                    }
                    else
                    {
                        if (resource_type_upper.IndexOf("MAP") >= 0)
                        {
                            returnVal.Add(HTML_Helper(Skin_Code, allVolumeCode, Translator.Get_Translation("RELATED MAPS", Current_Mode.Language), Current_Mode));
                        }
                        else
                        {
                            returnVal.Add(resource_type_upper.IndexOf("AERIAL") >= 0
                                              ? HTML_Helper(Skin_Code, allVolumeCode, Translator.Get_Translation("RELATED FLIGHTS", Current_Mode.Language), Current_Mode)
                                              : HTML_Helper(Skin_Code, allVolumeCode, Translator.Get_Translation("ALL VOLUMES", Current_Mode.Language), Current_Mode));
                        }
                    }
                    break;

                case View_Enum.CITATION:
                    if ((Current_Mode.ViewerCode == "citation") || ( Current_Mode.ViewerCode == "marc" ) || ( Current_Mode.ViewerCode == "metadata" ) || ( Current_Mode.ViewerCode == "usage" ))
                    {
                        returnVal.Add(HTML_Helper(Skin_Code, Current_Mode.ViewerCode, Translator.Get_Translation("CITATION", Current_Mode.Language), Current_Mode));
                    }
                    else
                    {
                        returnVal.Add(HTML_Helper(Skin_Code, "citation", Translator.Get_Translation("CITATION", Current_Mode.Language), Current_Mode));
                    }
                    break;

                case View_Enum.SEARCH:
                    returnVal.Add(HTML_Helper(Skin_Code, "search", Translator.Get_Translation("SEARCH", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.DOWNLOADS:
                    returnVal.Add(HTML_Helper(Skin_Code, "downloads", Translator.Get_Translation("DOWNLOADS", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.FEATURES:
                    returnVal.Add(HTML_Helper(Skin_Code, "features", Translator.Get_Translation("FEATURES", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.FLASH:
                    returnVal.Add(Item_View.Label.Length == 0
                                      ? HTML_Helper(Skin_Code, "flash", Translator.Get_Translation("FLASH VIEW", Current_Mode.Language), Current_Mode)
                                      : HTML_Helper(Skin_Code, "flash", Translator.Get_Translation(Item_View.Label.ToUpper(), Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.GOOGLE_MAP:
                    if (Current_Mode.Coordinates.Length > 0)
                    {
                        returnVal.Add(Current_Mode.ViewerCode == "mapsearch"
                                          ? HTML_Helper(Skin_Code, "mapsearch", Translator.Get_Translation("MAP SEARCH", Current_Mode.Language), Current_Mode)
                                          : HTML_Helper(Skin_Code, "map", Translator.Get_Translation("SEARCH RESULTS", Current_Mode.Language), Current_Mode));
                    }
                    else
                    {
                        returnVal.Add(HTML_Helper(Skin_Code, "map", Translator.Get_Translation("MAP IT!", Current_Mode.Language), Current_Mode));
                    }
                    break;

                case View_Enum.HTML:
                    returnVal.Add(Item_View.Label.Length > 0
                                      ? HTML_Helper(Skin_Code, "html", Item_View.Label.ToUpper(), Current_Mode)
                                      : HTML_Helper(Skin_Code, "html", "HTML LINK", Current_Mode));
                    break;

                case View_Enum.HTML_MAP:
                    returnVal.Add(HTML_Helper(Skin_Code, "htmlmap", Translator.Get_Translation(Item_View.Label.ToUpper(), Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.JPEG:
                    returnVal.Add(Resource_Type.ToUpper().IndexOf("MAP") >= 0
                                      ? HTML_Helper(Skin_Code, Page_Sequence.ToString() + "j", Translator.Get_Translation("MAP IMAGE", Current_Mode.Language), Current_Mode)
                                      : HTML_Helper(Skin_Code, Page_Sequence.ToString() + "j",  Translator.Get_Translation("PAGE IMAGE", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.JPEG2000:
                    if (Show_Zoomable)
                    {
                        returnVal.Add(HTML_Helper(Skin_Code, Page_Sequence.ToString() + "x", Translator.Get_Translation("ZOOMABLE", Current_Mode.Language), Current_Mode));
                    }
                    break;

                case View_Enum.RELATED_IMAGES:
                    returnVal.Add(Current_Mode.ViewerCode.IndexOf("thumbs") >= 0
                                      ? HTML_Helper(Skin_Code, Current_Mode.ViewerCode, Translator.Get_Translation("THUMBNAILS", Current_Mode.Language), Current_Mode)
                                      : HTML_Helper(Skin_Code, "thumbs", Translator.Get_Translation("THUMBNAILS", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.SIMPLE_HTML_LINK:
                    returnVal.Add("<li> <a href=\"" + Item_View.Attributes + "\" target=\"_blank\" alt=\"Link to '" + Item_View.Label + "'\"> " + Translator.Get_Translation(Item_View.Label.ToUpper(), Current_Mode.Language) + " </a></li>");
                    break;

                case View_Enum.STREETS:
                    returnVal.Add(HTML_Helper(Skin_Code, "streets", Translator.Get_Translation("STREETS", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.TEXT:
                    returnVal.Add(HTML_Helper(Skin_Code, (Page_Sequence + 1).ToString() + "T", Translator.Get_Translation("PAGE TEXT", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.TOC:
                     // returnVal.Add(base.HTML_Helper(Skin_Code, "TC", "Table of Contents", Current_Mode));
                    break;

                case View_Enum.PDF:
                    returnVal.Add(HTML_Helper(Skin_Code, "pdf", Translator.Get_Translation("PDF VIEWER", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.RESTRICTED:
                    returnVal.Add(HTML_Helper(Skin_Code, "restricted", Translator.Get_Translation("RESTRICTED", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.EAD_CONTAINER_LIST:
                    returnVal.Add(HTML_Helper(Skin_Code, "container", Translator.Get_Translation("CONTAINER LIST", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.EAD_DESCRIPTION:
                    returnVal.Add(HTML_Helper(Skin_Code, "description", Translator.Get_Translation("DESCRIPTION", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.PAGE_TURNER:
                    returnVal.Add(HTML_Helper(Skin_Code, "pageturner", Translator.Get_Translation("PAGE TURNER", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.YOUTUBE_VIDEO:
                    returnVal.Add(HTML_Helper(Skin_Code, "youtube", Translator.Get_Translation("VIDEO", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.TRACKING:
                    // DO nothing in this case.. do not write any tab
                    break;
            }

            return returnVal;
        }

        private static string HTML_Helper(string interface_code, string Viewer_Code, string Display_Text, SobekCM_Navigation_Object Current_Mode)
        {
            if (Current_Mode.ViewerCode == Viewer_Code)
            {
                string selectedTabStart = "<img src=\"" + Current_Mode.Base_URL + "design/skins/" + interface_code + "/tabs/cL_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab_s\"> ";
                string selectedTabEnd = " </span><img src=\"" + Current_Mode.Base_URL + "design/skins/" + interface_code + "/tabs/cR_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />";
                return selectedTabStart + Display_Text + selectedTabEnd;
            }

            string unselectedTabStart = "<img src=\"" + Current_Mode.Base_URL + "design/skins/" + interface_code + "/tabs/cL.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab\"> ";
            string unselectedTabEnd = " </span><img src=\"" + Current_Mode.Base_URL + "design/skins/" + interface_code + "/tabs/cR.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />";

            // When rendering for robots, provide the text and image, but not the text
            if (Current_Mode.Is_Robot)
            {
                return unselectedTabStart + Display_Text + unselectedTabEnd;
            }
            
            string current_viewer_code = Current_Mode.ViewerCode;
            Current_Mode.ViewerCode = Viewer_Code;
            string toReturn = "<a href=\"" + Current_Mode.Redirect_URL() + "\"> " + unselectedTabStart + Display_Text + unselectedTabEnd + " </a>";
            Current_Mode.ViewerCode = current_viewer_code;
            return toReturn;
        }
    }
}
