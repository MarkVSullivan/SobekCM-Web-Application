using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration.Citation;
using SobekCM.Core.Users;
using SobekCM.Library.Citation.SectionWriter;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Standard citation description item viewer prototyper, which is used to create the link in the main menu, 
    /// and to create the viewer itself if the user selects that option </summary>
    public class Citation_Standard_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the Citation_Standard_ItemViewer_Prototyper class </summary>
        public Citation_Standard_ItemViewer_Prototyper()
        {
            ViewerType = "CITATION";
            ViewerCode = "citation";
        }

        /// <summary> Name of this viewer, which matches the viewer name from the database and 
        /// in the configuration files as well.  This is actually populate by the configuration information </summary>
        public string ViewerType { get; set; }

        /// <summary> Code for this viewer, which can also be set from the configuration information </summary>
        public string ViewerCode { get; set; }

        /// <summary> If this viewer is tied to certain files existing in the digital resource, this lists all the 
        /// possible file extensions this supports (from the configuration file usually) </summary>
        public string[] FileExtensions { get; set; }

        /// <summary> Indicates if the specified item matches the basic requirements for this viewer, or
        /// if this viewer should be ignored for this item </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer really should be included </param>
        /// <returns> TRUE if this viewer should generally be included with this item, otherwise FALSE </returns>
        public bool Include_Viewer(BriefItemInfo CurrentItem)
        {
            // If not dark, always show
            if (!CurrentItem.Behaviors.Dark_Flag) return true;

            // If it is dark, use the system setting
            return UI_ApplicationCache_Gateway.Settings.Resources.Show_Citation_For_Dark_Items;
        }

        /// <summary> Flag indicates if this viewer should be override on checkout </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer should really be overriden </param>
        /// <returns> FALSE always, since citation type information should always be shown, even if an item is checked out </returns>
        public bool Override_On_Checkout(BriefItemInfo CurrentItem)
        {
            return false;
        }

        /// <summary> Flag indicates if the current user has access to this viewer for the item </summary>
        /// <param name="CurrentItem"> Digital resource to see if the current user has correct permissions to use this viewer </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="IpRestricted"> Flag indicates if this item is IP restricted AND if the current user is outside the ranges </param>
        /// <returns> TRUE if the user has access to use this viewer, otherwise FALSE </returns>
        public bool Has_Access(BriefItemInfo CurrentItem, User_Object CurrentUser, bool IpRestricted)
        {
            // It can always be shown
            return true;
        }

        /// <summary> Gets the menu items related to this viewer that should be included on the main item (digital resource) menu </summary>
        /// <param name="CurrentItem"> Digital resource object, which can be used to ensure if and how this viewer should appear 
        /// in the main item (digital resource) menu </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="MenuItems"> List of menu items, to which this method may add one or more menu items </param>
        /// <param name="IpRestricted"> Flag indicates if this item is IP restricted AND if the current user is outside the ranges </param>
        public void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems, bool IpRestricted)
        {
            // Get the URL for this
            string previous_code = CurrentRequest.ViewerCode;
            CurrentRequest.ViewerCode = ViewerCode;
            string url = UrlWriterHelper.Redirect_URL(CurrentRequest);
            CurrentRequest.ViewerCode = previous_code;

            // Add the item menu information
            Item_MenuItem menuItem = new Item_MenuItem("Description", "Standard View", null, url, ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="Citation_Standard_ItemViewer"/> class for showing the
        /// standard description citation for a digital resource, during a single execution for a HTTP request </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="Citation_Standard_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new Citation_Standard_ItemViewer(CurrentItem, CurrentUser, CurrentRequest);
        }
    }

    /// <summary> Item viewer displays the descriptive citation in standard, human-readable format for a digital resource </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Citation_Standard_ItemViewer : abstractNoPaginationItemViewer
    {
        private readonly int width = 180;
        private readonly bool isRobot;

        /// <summary> Constructor for a new instance of the Citation_Standard_ItemViewer class, used to display the 
        /// descriptive citation in standard, human-readable format for the digital resource </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        public Citation_Standard_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest)
        {
            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;

            // Set the width
            if ((CurrentRequest.Language == Web_Language_Enum.French) || (CurrentRequest.Language == Web_Language_Enum.Spanish))
                width = 230;

            // Get  the robot flag (if this is rendering for robots, the other citation views are not available)
            isRobot = CurrentRequest.Is_Robot;
        }

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkCsiv_Viewer' </value>
        public override string ViewerBox_CssId
        {
            get { return "sbkCsiv_Viewer"; }
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Citation_Standard_ItemViewer.Write_Main_Viewer_Section", "Write the citation information directly to the output stream");
            }

            // Determine if user can edit
            bool userCanEditItem = false;
            if (CurrentUser != null)
            {
                userCanEditItem = CurrentUser.Can_Edit_This_Item(BriefItem.BibID, BriefItem.Type, BriefItem.Behaviors.Source_Institution_Aggregation, BriefItem.Behaviors.Holding_Location_Aggregation, BriefItem.Behaviors.Aggregation_Code_List);
            }

            // Add the HTML for the citation
            Output.WriteLine("        <!-- CITATION ITEM VIEWER OUTPUT -->");
            Output.WriteLine("        <td>");

            // If this is DARK and the user cannot edit and the flag is not set to show citation, show nothing here
            if ((BriefItem.Behaviors.Dark_Flag) && (!userCanEditItem) && (!UI_ApplicationCache_Gateway.Settings.Resources.Show_Citation_For_Dark_Items))
            {
                Output.WriteLine("          <div id=\"darkItemSuppressCitationMsg\">This item is DARK and cannot be viewed at this time</div>" + Environment.NewLine + "</td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
                return;
            }

            string viewer_code = CurrentRequest.ViewerCode;

            // Get any search terms
            List<string> terms = new List<string>();
            if (!String.IsNullOrWhiteSpace(CurrentRequest.Text_Search))
            {
                string[] splitter = CurrentRequest.Text_Search.Replace("\"", "").Split(" ".ToCharArray());
                terms.AddRange(from thisSplit in splitter where thisSplit.Trim().Length > 0 select thisSplit.Trim());
            }

            // Add the main wrapper division
            // Determine the material type
            string microdata_type = "CreativeWork";
            switch (BriefItem.Type)
            {
                case "BOOK":
                case "SERIAL":
                case "NEWSPAPER":
                    microdata_type = "Book";
                    break;

                case "MAP":
                    microdata_type = "Map";
                    break;

                case "PHOTOGRAPH":
                case "AERIAL":
                    microdata_type = "Photograph";
                    break;
            }

            // Add the main wrapper division, with microdata information
            Output.WriteLine("<div id=\"sbkCiv_Citation\" itemprop=\"about\" itemscope itemtype=\"http://schema.org/" + microdata_type + "\">");

            if (!CurrentRequest.Is_Robot)
                Add_Citation_View_Tabs(Output, BriefItem, CurrentRequest, "CITATION");

            // Now, add the text
            Output.WriteLine();

            if (terms.Count > 0)
            {
                Output.WriteLine(Text_Search_Term_Highlighter.Hightlight_Term_In_HTML(Standard_Citation_String(!isRobot, Tracer), terms, "<span class=\"sbkCiv_TextHighlight\">", "</span>") + Environment.NewLine + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
            }
            else
            {
                Output.WriteLine(Standard_Citation_String(!isRobot, Tracer) + Environment.NewLine + "  </td>" + Environment.NewLine + "  <div id=\"sbkCiv_EmptyRobotDiv\" />" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
            }

            CurrentRequest.ViewerCode = viewer_code;
        }

        #region Code to create the regular citation string

        /// <summary> Returns the basic information about this digital resource in standard format </summary>
        /// <param name="Include_Links"> Flag tells whether to include the search links from this citation view </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> HTML string with the basic information about this digital resource for display </returns>
        public string Standard_Citation_String(bool Include_Links, Custom_Tracer Tracer)
        {

            // Compute the URL to use for all searches from the citation
            Display_Mode_Enum lastMode = CurrentRequest.Mode;
            CurrentRequest.Mode = Display_Mode_Enum.Results;
            CurrentRequest.Search_Type = Search_Type_Enum.Advanced;
            CurrentRequest.Search_String = "<%VALUE%>";
            CurrentRequest.Search_Fields = "<%CODE%>";
            string search_link = "<a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest).Replace("&", "&amp;").Replace("%3c%25", "<%").Replace("%25%3e", "%>").Replace("<%VALUE%>", "&quot;<%VALUE%>&quot;") + "\" target=\"_BLANK\">";
            string search_link_end = "</a>";
            CurrentRequest.Aggregation = String.Empty;
            CurrentRequest.Search_String = String.Empty;
            CurrentRequest.Search_Fields = String.Empty;
            CurrentRequest.Mode = lastMode;

            // If no search links should should be included, clear the search strings
            if (!Include_Links)
            {
                search_link = String.Empty;
                search_link_end = String.Empty;
            }


            if (Tracer != null)
            {
                Tracer.Add_Trace("Citation_Standard_ItemViewer.Standard_Citation_String", "Configuring brief item data into standard citation format");
            }

            // Use string builder to build this
            const string INDENT = "    ";
            StringBuilder result = new StringBuilder();

            // Now, try to add the thumbnail from any page images here
            if (BriefItem.Behaviors.Dark_Flag != true)
            {
                string name_for_image = HttpUtility.HtmlEncode(BriefItem.Title);

                if (!String.IsNullOrEmpty(BriefItem.Behaviors.Main_Thumbnail))
                {
                    
                    result.AppendLine();
                    result.AppendLine(INDENT + "<div id=\"Sbk_CivThumbnailDiv\"><a href=\"" + CurrentRequest.Base_URL + BriefItem.BibID + "/" + BriefItem.VID + "\" ><img src=\"" + BriefItem.Web.Source_URL + "/" + BriefItem.Behaviors.Main_Thumbnail + "\" alt=\"" + name_for_image + "\" id=\"Sbk_CivThumbnailImg\" itemprop=\"primaryImageOfPage\" /></a></div>");
                    result.AppendLine();
                }
                else if ((BriefItem.Images != null ) && ( BriefItem.Images.Count > 0 ))
                {
                    if (BriefItem.Images[0].Files.Count > 0)
                    {
                        string jpeg = String.Empty;
                        foreach (BriefItem_File thisFileInfo in BriefItem.Images[0].Files)
                        {
                            if (thisFileInfo.Name.ToLower().IndexOf(".jpg") > 0)
                            {
                                if (jpeg.Length == 0)
                                    jpeg = thisFileInfo.Name;
                                else if (thisFileInfo.Name.ToLower().IndexOf("thm.jpg") < 0)
                                    jpeg = thisFileInfo.Name;
                            }
                        }

                        string name_of_page = BriefItem.Images[0].Label;
                        name_for_image = name_for_image + " - " + HttpUtility.HtmlEncode(name_of_page);


                        // If a jpeg was found, show it
                        if (jpeg.Length > 0)
                        {
                            result.AppendLine();
                            result.AppendLine(INDENT + "<div id=\"Sbk_CivThumbnailDiv\"><a href=\"" + CurrentRequest.Base_URL + BriefItem.BibID + "/" + BriefItem.VID + "\" ><img src=\"" + BriefItem.Web.Source_URL + "/" + jpeg + "\" alt=\"" + name_for_image + "\" id=\"Sbk_CivThumbnailImg\" itemprop=\"primaryImageOfPage\" /></a></div>");
                            result.AppendLine();
                        }
                    }
                }
            }

            // Step through the citation configuration here
            CitationSet citationSet = UI_ApplicationCache_Gateway.Configuration.UI.CitationViewer.Get_CitationSet();
            foreach (CitationFieldSet fieldsSet in citationSet.FieldSets)
            {
                // Check to see if any of the values indicated in this field set exist
                bool foundExistingData = false;
                foreach (CitationElement thisField in fieldsSet.Elements)
                {
                    // Was this a custom writer?
                    if ((thisField.SectionWriter != null) && (!String.IsNullOrWhiteSpace(thisField.SectionWriter.Class_Name)))
                    {
                        // Try to get the section writer
                        iCitationSectionWriter sectionWriter = SectionWriter_Factory.GetSectionWriter(thisField.SectionWriter.Assembly, thisField.SectionWriter.Class_Name);

                        // If it was found and there is data, then we found some
                        if ((sectionWriter != null) && (sectionWriter.Has_Data_To_Write(thisField, BriefItem)))
                        {
                            foundExistingData = true;
                            break;
                        }
                    }
                    else // Not a custom writer
                    {
                        // Look for a match in the item description
                        BriefItem_DescriptiveTerm briefTerm = BriefItem.Get_Description(thisField.MetadataTerm);

                        // If no match, just continue
                        if ((briefTerm != null) && (briefTerm.Values.Count > 0))
                        {
                            foundExistingData = true;
                            break;
                        }
                    }
                }

                // If no data was found to put in this field set, skip it
                if (!foundExistingData)
                    continue;

                // Start this section
                result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\" id=\"sbkCiv_" + fieldsSet.ID.Replace(" ", "_") + "Section\" >");
                if (!String.IsNullOrEmpty(fieldsSet.Heading))
                {
                    result.AppendLine(INDENT + "<h2>" + UI_ApplicationCache_Gateway.Translation.Get_Translation(fieldsSet.Heading, CurrentRequest.Language) + "</h2>");
                }
                result.AppendLine(INDENT + "  <dl>");

                // Step through all the fields in this field set and write them
                foreach (CitationElement thisField in fieldsSet.Elements)
                {
                    // Was this a custom writer?
                    if ((thisField.SectionWriter != null) && (!String.IsNullOrWhiteSpace(thisField.SectionWriter.Class_Name)))
                    {
                        // Try to get the section writer
                        iCitationSectionWriter sectionWriter = SectionWriter_Factory.GetSectionWriter(thisField.SectionWriter.Assembly, thisField.SectionWriter.Class_Name);

                        // If it was found and there is data, then we found some
                        if ((sectionWriter != null) && (sectionWriter.Has_Data_To_Write(thisField, BriefItem)))
                        {
                            sectionWriter.Write_Citation_Section(thisField, result, BriefItem, width, search_link, search_link_end, Tracer);
                        }
                    }
                    else // Not a custom writer
                    {

                        // Look for a match in the item description
                        BriefItem_DescriptiveTerm briefTerm = BriefItem.Get_Description(thisField.MetadataTerm);

                        // If no match, just continue
                        if ((briefTerm == null) || (briefTerm.Values.Count == 0))
                            continue;

                        // If they can all be listed one after the other do so now
                        if (!thisField.IndividualFields)
                        {
                            List<string> valueArray = new List<string>();
                            foreach (BriefItem_DescTermValue thisValue in briefTerm.Values)
                            {
                                if (!String.IsNullOrEmpty(thisField.SearchCode))
                                {
                                    // It is possible a different search term is valid for this item, so check it
                                    string searchTerm = (!String.IsNullOrWhiteSpace(thisValue.SearchTerm)) ? thisValue.SearchTerm : thisValue.Value;

                                    if (String.IsNullOrEmpty(thisField.ItemProp))
                                    {
                                        if (String.IsNullOrEmpty(thisValue.Authority))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add(search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end);
                                            }
                                            else
                                            {
                                                valueArray.Add(search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Language + " )");
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add(search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + " )");
                                            }
                                            else
                                            {
                                                valueArray.Add(search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + ", " + thisValue.Language + " )");
                                            }
                                        }

                                    }
                                    else
                                    {
                                        if (String.IsNullOrEmpty(thisValue.Authority))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + "</span>");
                                            }
                                            else
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Language + " )" + "</span>");
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + " )" + "</span>");
                                            }
                                            else
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + ", " + thisValue.Language + " )" + "</span>");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(thisField.ItemProp))
                                    {
                                        if (String.IsNullOrEmpty(thisValue.Authority))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add(display_text_from_value(thisValue.Value));
                                            }
                                            else
                                            {
                                                valueArray.Add(display_text_from_value(thisValue.Value) + " ( " + thisValue.Language + " )");
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add(display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + " )");
                                            }
                                            else
                                            {
                                                valueArray.Add(display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + ", " + thisValue.Language + " )");
                                            }
                                        }

                                    }
                                    else
                                    {
                                        if (String.IsNullOrEmpty(thisValue.Authority))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + "</span>");
                                            }
                                            else
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + " ( " + thisValue.Language + " )" + "</span>");
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + " )" + "</span>");
                                            }
                                            else
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + ", " + thisValue.Language + " )" + "</span>");
                                            }
                                        }
                                    }
                                }
                            }

                            // Now, add this to the citation HTML
                            Add_Citation_HTML_Rows(thisField.DisplayTerm, valueArray, INDENT, result);
                        }
                        else
                        {
                            // In this case, each individual value gets its own citation html row
                            foreach (BriefItem_DescTermValue thisValue in briefTerm.Values)
                            {
                                // Determine the label
                                string label = thisField.DisplayTerm;
                                if (thisField.OverrideDisplayTerm == CitationElement_OverrideDispayTerm_Enum.subterm)
                                {
                                    if (!String.IsNullOrEmpty(thisValue.SubTerm))
                                        label = thisValue.SubTerm;
                                }

                                // It is possible a different search term is valid for this item, so check it
                                string searchTerm = (!String.IsNullOrWhiteSpace(thisValue.SearchTerm)) ? thisValue.SearchTerm : thisValue.Value;

                                if (!String.IsNullOrEmpty(thisField.SearchCode))
                                {
                                    if (String.IsNullOrEmpty(thisField.ItemProp))
                                    {
                                        if (String.IsNullOrEmpty(thisValue.Authority))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end, INDENT));
                                            }
                                            else
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Language + " )", INDENT));
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + " )", INDENT));
                                            }
                                            else
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + ", " + thisValue.Language + " )", INDENT));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (String.IsNullOrEmpty(thisValue.Authority))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + "</span>", INDENT));
                                            }
                                            else
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Language + " )" + "</span>", INDENT));
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + " )" + "</span>", INDENT));
                                            }
                                            else
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + ", " + thisValue.Language + " )" + "</span>", INDENT));
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // Since this isn't tied to a search code, we won't build a URL.  But the
                                    // data could still HAVE a URL associated with it.
                                    if ((thisValue.URIs == null) || (thisValue.URIs.Count == 0))
                                    {
                                        if (String.IsNullOrEmpty(thisField.ItemProp))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Authority))
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, display_text_from_value(thisValue.Value), INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, display_text_from_value(thisValue.Value) + " ( " + thisValue.Language + " )", INDENT));
                                                }
                                            }
                                            else
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + " )", INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + ", " + thisValue.Language + " )", INDENT));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Authority))
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + "</span>", INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + " ( " + thisValue.Language + " )" + "</span>", INDENT));
                                                }
                                            }
                                            else
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + " )" + "</span>", INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + ", " + thisValue.Language + " )" + "</span>", INDENT));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // This has a URI
                                        if (String.IsNullOrEmpty(thisField.ItemProp))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Authority))
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>", INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>" + " ( " + thisValue.Language + " )", INDENT));
                                                }
                                            }
                                            else
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>" + " ( " + thisValue.Authority + " )", INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>" + " ( " + thisValue.Authority + ", " + thisValue.Language + " )", INDENT));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Authority))
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>" + "</span>", INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>" + " ( " + thisValue.Language + " )" + "</span>", INDENT));
                                                }
                                            }
                                            else
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>" + " ( " + thisValue.Authority + " )" + "</span>", INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>" + " ( " + thisValue.Authority + ", " + thisValue.Language + " )" + "</span>", INDENT));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // End this division
                result.AppendLine(INDENT + "  </dl>");
                result.AppendLine(INDENT + "</div>");
            }

            //bool internalUser = ((CurrentUser != null) && (CurrentUser.LoggedOn) && (CurrentUser.Is_Internal_User));
            //if (internalUser)
            //{
            //    result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\" id=\"sbkCiv_MetsSection\" >");
            //    result.AppendLine(INDENT + "<h2>METS Information</h2>");
            //    result.AppendLine(INDENT + "  <dl>");
            //    result.Append(Single_Citation_HTML_Row("Format", BriefItem.Type, INDENT));
            //    result.Append(Single_Citation_HTML_Row("Creation Date", CurrentItem.METS_Header.Create_Date.ToShortDateString(), INDENT));
            //    result.Append(Single_Citation_HTML_Row("Last Modified", CurrentItem.METS_Header.Modify_Date.ToShortDateString(), INDENT));
            //    result.Append(Single_Citation_HTML_Row("Last Type", CurrentItem.METS_Header.RecordStatus, INDENT));
            //    result.Append(Single_Citation_HTML_Row("Last User", CurrentItem.METS_Header.Creator_Individual, INDENT));
            //    result.Append(Single_Citation_HTML_Row("System Folder", CurrentItem.Web.AssocFilePath.Replace("/", "\\"), INDENT));

            //    result.AppendLine(INDENT + "  </dl>");
            //    result.AppendLine(INDENT + "</div>");
            //    result.AppendLine();
            //}

            if ((CurrentUser != null) && (CurrentUser.Is_System_Admin))
            {
                result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\" id=\"sbkCiv_AdminSection\" >");
                result.AppendLine(INDENT + "<h2>System Administration Information</h2>");
                result.AppendLine(INDENT + "  <dl>");
                result.Append(Single_Citation_HTML_Row("Item Primary Key", BriefItem.Web.ItemID.ToString(), INDENT));
                result.Append(Single_Citation_HTML_Row("Group Primary Key", BriefItem.Web.GroupID.ToString(), INDENT));
                result.AppendLine(INDENT + "  </dl>");
                result.AppendLine(INDENT + "</div>");
                result.AppendLine();
            }

            result.AppendLine(INDENT + "<br />");
            result.AppendLine("</div>");

            // Return the built string
            return result.ToString();
        }

        private static string display_text_from_value(string Value)
        {
            return HttpUtility.HtmlEncode(Value).Replace("&lt;i&gt;", "<i>").Replace("&lt;/i&gt;", "</i>");
        }

        private static string search_link_from_value(string Value)
        {
            string replacedValue = Value.Replace("&amp;", "&").Replace("&", "").Replace("  ", " ");
            string urlEncode = HttpUtility.UrlEncode(replacedValue);
            return urlEncode != null ? urlEncode.Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+") : String.Empty;
        }

        private void Add_Citation_HTML_Rows(string Row_Name, List<string> Values, string Indent, StringBuilder Results)
        {
            // Only add if there is a value
            if (Values.Count <= 0) return;

            Results.Append(Indent + "    <dt class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"width:" + width + "px;\" >");

            // Add with proper language
            Results.Append(UI_ApplicationCache_Gateway.Translation.Get_Translation(Row_Name, CurrentRequest.Language));

            Results.AppendLine(": </dt>");
            Results.Append(Indent + "    <dd class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"margin-left:" + width + "px;\">");
            bool first = true;
            foreach (string thisValue in Values.Where(ThisValue => ThisValue.Length > 0))
            {
                if (first)
                {
                    Results.Append(thisValue);
                    first = false;
                }
                else
                {
                    Results.Append("<br />" + thisValue);
                }
            }
            Results.AppendLine("</dd>");
            Results.AppendLine();
        }

        private string Single_Citation_HTML_Row(string Row_Name, string Value, string Indent)
        {
            // Only add if there is a value
            if (Value.Length > 0)
            {
                return Indent + "    <dt class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"width:" + width + "px;\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation(Row_Name, CurrentRequest.Language) + ": </dt>" + Environment.NewLine + Indent + "    <dd class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"margin-left:" + width + "px;\">" + Value + "</dd>" + Environment.NewLine + Environment.NewLine;
            }
            return String.Empty;
        }

        #endregion


        /// <summary> Allows controls to be added directory to a place holder, rather than just writing to the output HTML stream </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> This method does nothing, since nothing is added to the place holder as a control for this item viewer </remarks>
        public override void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            // Do nothing
        }

        /// <summary> Write the citation view tabs to the stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="CurrentType"> Type of the current view, so one tab can be marked as current </param>
        /// <remarks> This static method is called from the other viewers that were split off from the old citation viewer 
        /// ( Citation_MARC_ItemViewer, Metadata_Links_ItemViewer, and Usage_Stats_ItemViewer ) </remarks>
        public static void Add_Citation_View_Tabs(TextWriter Output, BriefItemInfo BriefItem, Navigation_Object CurrentRequest, string CurrentType )
        {
            // Set the text
            const string STANDARD_VIEW = "STANDARD VIEW";
            const string MARC_VIEW = "MARC VIEW";
            const string METADATA_VIEW = "METADATA";
            const string STATISTICS_VIEW = "USAGE STATISTICS";

            // Add the tabs for the different citation information
            string orig_viewer_code = CurrentRequest.ViewerCode;
            Output.WriteLine("  <div id=\"sbkCiv_ViewSelectRow\">");
            Output.WriteLine("    <ul class=\"sbk_FauxDownwardTabsList\">");

            // Add the standard tab
            if (CurrentType == "CITATION")
            {
                Output.WriteLine("      <li class=\"current\">" + STANDARD_VIEW + "</li>");
            }
            else
            {
                // Ensure the CITATION is included in the system and in the item (should be though)
                string viewer_code = ItemViewer_Factory.ViewCode_From_ViewType("CITATION");
                if ((BriefItem.Behaviors.Get_Viewer("CITATION") != null) && (!BriefItem.Behaviors.Get_Viewer("CITATION").Excluded) && (!String.IsNullOrEmpty(viewer_code)))
                    Output.WriteLine("      <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest, "citation") + "\">" + STANDARD_VIEW + "</a></li>");
            }

            // Add the MARC tab
            if (CurrentType == "MARC")
            {
                Output.WriteLine("      <li class=\"current\">" + MARC_VIEW + "</li>");
            }
            else
            {
                // Ensure the MARC is included in the system and in the item 
                string viewer_code = ItemViewer_Factory.ViewCode_From_ViewType("MARC");
                if ((BriefItem.Behaviors.Get_Viewer("MARC") != null) && (!BriefItem.Behaviors.Get_Viewer("MARC").Excluded) && (!String.IsNullOrEmpty(viewer_code)))
                    Output.WriteLine("      <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest, "marc") + "\">" + MARC_VIEW + "</a></li>");
            }

            // If this item is an external link item (i.e. has related URL, but no pages or downloads) skip the next parts
            if (BriefItem.Type != "BIBLEVEL") 
            {
                // Add the METADATA links tab
                if (CurrentType == "METADATA")
                {
                    Output.WriteLine("      <li class=\"current\">" + METADATA_VIEW + "</li>");
                }
                else
                {
                    // Ensure the MARC is included in the system and in the item 
                    string viewer_code = ItemViewer_Factory.ViewCode_From_ViewType("METADATA");
                    if ((BriefItem.Behaviors.Get_Viewer("METADATA") != null) && (!BriefItem.Behaviors.Get_Viewer("METADATA").Excluded) && (!String.IsNullOrEmpty(viewer_code)))
                        Output.WriteLine("      <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest, "metadata") + "\">" + METADATA_VIEW + "</a></li>");
                }

                // Add the USAGE STATISTICS links tab
                if (CurrentType == "USAGE")
                {
                    Output.WriteLine("      <li class=\"current\">" + STATISTICS_VIEW + "</li>");
                }
                else
                {
                    // Ensure the MARC is included in the system and in the item 
                    string viewer_code = ItemViewer_Factory.ViewCode_From_ViewType("USAGE");
                    if ((BriefItem.Behaviors.Get_Viewer("USAGE") != null) && (!BriefItem.Behaviors.Get_Viewer("USAGE").Excluded) && (!String.IsNullOrEmpty(viewer_code)))
                        Output.WriteLine("      <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest, "usage") + "\">" + STATISTICS_VIEW + "</a></li>");
                }
            }

            Output.WriteLine("    </ul>");
            Output.WriteLine("  </div>");

            // Restore the current viewer code
            CurrentRequest.ViewerCode = orig_viewer_code;
        }
    }
}
