#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;

#endregion

namespace SobekCM.Engine_Library.Aggregations
{
    /// <summary> Class is used to read the item aggregation configuration XML files (when they exist)
    /// and populate the home page source files, banner information, highlights, and the browse and 
    /// informational pages connected to the item aggregation.</summary>
    public class Item_Aggregation_XML_Reader
    {
        /// <summary> Reads the item aggregation configuration file and populates the new data into the
        /// item aggregation object </summary>
        /// <param name="HierarchyObject"> Item aggregation object to populate</param>
        /// <param name="FileLocation"> Full name of the item aggregation configuration XML file </param>
        public void Add_Info_From_XML_File(Complete_Item_Aggregation HierarchyObject, string FileLocation )
        {
            // Get the directory from the file location
            string directory = (new FileInfo(FileLocation)).DirectoryName;

            // Load this XML file
            XmlDocument hierarchyXml = new XmlDocument();
            hierarchyXml.Load(FileLocation);

            // create the node reader
            XmlNodeReader nodeReader = new XmlNodeReader(hierarchyXml);

            // Read all the nodes
            while (nodeReader.Read())
            {
                // If this is the beginning tag for an element, assign the next values accordingly
                if (nodeReader.NodeType == XmlNodeType.Element)
                {
                    // Get the node name, trimmed and to upper
                    string nodeName = nodeReader.Name.Trim().ToUpper();

                    // switch the rest based on the tag name
                    switch (nodeName)
                    {
                        case "HI:SETTINGS":
                            read_settings(nodeReader, HierarchyObject);
                            break;

                        case "HI:HOME":
                            read_home(nodeReader, HierarchyObject);
                            break;

                        case "HI:BANNER":
                            read_banners(nodeReader, HierarchyObject);
                            break;

                        case "HI:DIRECTIVES":
                            read_directives(nodeReader, HierarchyObject, directory);
                            break;

                        case "HI:HIGHLIGHTS":
                            read_highlights(nodeReader, HierarchyObject);
                            break;

                        case "HI:BROWSE":
                            read_browse(true, nodeReader, HierarchyObject);
                            break;

                        case "HI:INFO":
                            read_browse(false, nodeReader, HierarchyObject);
                            break;

                        case "HI:RESULTS":
                            read_results_specs(nodeReader, HierarchyObject);
                            break;
                    }
                }
            }
        }

        private static void read_results_specs(XmlNodeReader NodeReader, Complete_Item_Aggregation HierarchyObject)
        {
            bool inViews = false;
            while (NodeReader.Read())
            {
                // If this is the beginning tag for an element, assign the next values accordingly
                string nodeName;
                if (NodeReader.NodeType == XmlNodeType.Element)
                {
                    // Get the node name, trimmed and to upper
                    nodeName = NodeReader.Name.Trim().ToUpper();

                    switch (nodeName)
                    {
                        case "HI:VIEWS":
                            inViews = true;
                            break;

                        case "HI:ADD":
                            if ( inViews )
                            {
                                bool isDefault = false;
                                string type = String.Empty;
                                if (NodeReader.MoveToAttribute("default"))
                                {
                                    isDefault = true;
                                }
                                if (NodeReader.MoveToAttribute("type"))
                                {
                                    type = NodeReader.Value.ToUpper();
                                }
                                if (type.Length > 0)
                                {
                                    Result_Display_Type_Enum displayType = Result_Display_Type_Enum.Default;
                                    switch (type)
                                    {
                                        case "BRIEF":
                                            displayType = Result_Display_Type_Enum.Brief;
                                            break;

                                        case "FULL":
                                            displayType = Result_Display_Type_Enum.Full_Citation;
                                            break;

                                        case "THUMBNAIL":
                                            displayType = Result_Display_Type_Enum.Thumbnails;
                                            break;

                                        case "TABLE":
                                            displayType = Result_Display_Type_Enum.Table;
                                            break;

                                        case "MAP":
                                            displayType = Result_Display_Type_Enum.Map;
                                            break;
                                    }
                                    if (displayType != Result_Display_Type_Enum.Default)
                                    {
                                        if (!HierarchyObject.Result_Views.Contains(displayType))
                                        {
                                            HierarchyObject.Result_Views.Add(displayType);
                                        }
                                        if (isDefault)
                                        {
                                            HierarchyObject.Default_Result_View = displayType;
                                        }
                                    }
                                }
                            }
                            break;

                        case "HI:REMOVE":
                            if (inViews)
                            {
                                string type = String.Empty;
                                if (NodeReader.MoveToAttribute("type"))
                                {
                                    type = NodeReader.Value.ToUpper();
                                }
                                if (type.Length > 0)
                                {
                                    Result_Display_Type_Enum displayType = Result_Display_Type_Enum.Default;
                                    switch (type)
                                    {
                                        case "BRIEF":
                                            displayType = Result_Display_Type_Enum.Brief;
                                            break;

                                        case "FULL":
                                            displayType = Result_Display_Type_Enum.Full_Citation;
                                            break;

                                        case "THUMBNAIL":
                                            displayType = Result_Display_Type_Enum.Thumbnails;
                                            break;

                                        case "TABLE":
                                            displayType = Result_Display_Type_Enum.Table;
                                            break;

                                        case "MAP":
                                            displayType = Result_Display_Type_Enum.Map;
                                            break;
                                    }
                                    if (displayType != Result_Display_Type_Enum.Default)
                                    {
                                        if (HierarchyObject.Result_Views.Contains(displayType))
                                        {
                                            HierarchyObject.Result_Views.Remove(displayType);
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }

                // If this is not an end element, continue
                if (NodeReader.NodeType != XmlNodeType.EndElement) continue;

                // Get the node name, trimmed and to upper
                nodeName = NodeReader.Name.Trim().ToUpper();

                switch ( nodeName )
                {
                    case "HI:VIEWS":
                        inViews = false;
                        break;

                    case "HI:RESULTS":
                        return;
                }
            }
        }

        private static void read_settings(XmlNodeReader NodeReader, Complete_Item_Aggregation HierarchyObject)
        {
            while (NodeReader.Read())
            {
                // If this is the beginning tag for an element, assign the next values accordingly
                if (NodeReader.NodeType == XmlNodeType.Element)
                {
                    // Get the node name, trimmed and to upper
                    string nodeName = NodeReader.Name.Trim().ToUpper();

                    // switch the rest based on the tag name
                    switch (nodeName)
                    {
                        case "HI:WEBSKINS":
                            NodeReader.Read();
                            string webskins = NodeReader.Value;
                            string[] splitter = webskins.Split(",".ToCharArray());
                            foreach (string thisSplitter in splitter)
                            {
                                if ( thisSplitter.Length > 0 )
                                    HierarchyObject.Add_Web_Skin(thisSplitter.ToLower());
                            }
                            break;

						case "HI:CSS":
							NodeReader.Read();
							HierarchyObject.CSS_File = NodeReader.Value.Trim();
							break;

						case "HI:CUSTOMHOME":
							NodeReader.Read();
                            // No longer do anything with this tag
							// HierarchyObject.Custom_Home_Page_Source_File = NodeReader.Value.Trim();
							break;

                        case "HI:FACETS":
                            NodeReader.Read();
                            string facets = NodeReader.Value;
                            string[] splitter2 = facets.Split(",".ToCharArray());
                            HierarchyObject.Clear_Facets();
                            foreach (string thisSplitter2 in splitter2)
                            {
                                    HierarchyObject.Add_Facet(Convert.ToInt16(thisSplitter2));
                            }
                            break;
                    }
                }

                if (NodeReader.NodeType == XmlNodeType.EndElement)
                {
                    if (NodeReader.Name.Trim().ToUpper() == "HI:SETTINGS")
                    {
                        return;
                    }
                }
            }
        }

        private static void read_home(XmlNodeReader NodeReader, Complete_Item_Aggregation HierarchyObject)
        {
            while (NodeReader.Read())
            {
                // If this is the beginning tag for an element, assign the next values accordingly
                if (NodeReader.NodeType == XmlNodeType.Element)
                {
                    // Get the node name, trimmed and to upper
                    string nodeName = NodeReader.Name.Trim().ToUpper();

                    // switch the rest based on the tag name
                    switch (nodeName)
                    {
                        case "HI:BODY":
                            Web_Language_Enum langEnum = Web_Language_Enum.DEFAULT;
                            bool isCustom = false;
                            if ((NodeReader.HasAttributes) && (NodeReader.MoveToAttribute("lang")))
                            {
                                string bodyLanguage = NodeReader.GetAttribute("lang");
                                langEnum = Web_Language_Enum_Converter.Code_To_Enum(bodyLanguage);
                            }
                            if ((NodeReader.HasAttributes) && (NodeReader.MoveToAttribute("isCustom")))
                            {
                                string attribute = NodeReader.GetAttribute("isCustom");
                                if (attribute != null && attribute.ToLower() == "true")
                                    isCustom = true;
                            }

                            NodeReader.Read();
                            HierarchyObject.Add_Home_Page_File(NodeReader.Value, langEnum, isCustom );
                            break;
                    }
                }

                if ((NodeReader.NodeType == XmlNodeType.EndElement) && (NodeReader.Name.Trim().ToUpper() == "HI:HOME"))
                {
                    return;
                }
            }
        }

        private static void read_banners(XmlNodeReader NodeReader, Complete_Item_Aggregation HierarchyObject)
        {
            while (NodeReader.Read())
            {
                // If this is the beginning tag for an element, assign the next values accordingly
                if (NodeReader.NodeType == XmlNodeType.Element)
                {
                    // Get the node name, trimmed and to upper
                    string nodeName = NodeReader.Name.Trim().ToUpper();

                    // switch the rest based on the tag name
                    switch (nodeName)
                    {
                        case "HI:SOURCE":
                            // Check for any attributes to this banner node
                            string lang = String.Empty;
                            bool special = false;
                            Item_Aggregation_Front_Banner_Type_Enum type = Item_Aggregation_Front_Banner_Type_Enum.Left;
		                    ushort width = 550;
		                    ushort height = 230;

                            if (NodeReader.HasAttributes)
                            {

                                if (NodeReader.MoveToAttribute("lang"))
                                {
                                    lang = NodeReader.Value.Trim().ToUpper();
                                }
                                if (NodeReader.MoveToAttribute("type"))
                                {
                                    if ((NodeReader.Value.Trim().ToUpper() == "HIGHLIGHT") || ( NodeReader.Value.Trim().ToUpper() == "FRONT"))
                                        special = true;
                                }
								if (NodeReader.MoveToAttribute("side"))
								{
									switch (NodeReader.Value.Trim().ToUpper())
									{
										case "RIGHT":
                                            type = Item_Aggregation_Front_Banner_Type_Enum.Right;
											break;

										case "LEFT":
                                            type = Item_Aggregation_Front_Banner_Type_Enum.Left;
											break;

										case "FULL":
                                            type = Item_Aggregation_Front_Banner_Type_Enum.Full;
											break;
									}
								}
								if (NodeReader.MoveToAttribute("width"))
								{
									ushort.TryParse(NodeReader.Value, out width);

								}
								if (NodeReader.MoveToAttribute("height"))
								{
									ushort.TryParse(NodeReader.Value, out height);
								}
                            }

                            // Now read the banner information and add to the aggregation object
                            NodeReader.Read();
                            if (special)
                            {
                                Item_Aggregation_Front_Banner bannerObj = HierarchyObject.Add_Front_Banner_Image(NodeReader.Value, Web_Language_Enum_Converter.Code_To_Enum( lang));
	                            bannerObj.Width = width;
	                            bannerObj.Height = height;
	                            bannerObj.Type = type;
                            }
                            else
                            {
                                HierarchyObject.Add_Banner_Image(NodeReader.Value, Web_Language_Enum_Converter.Code_To_Enum(lang));
                            }


                            break;
                    }
                }

                if ((NodeReader.NodeType == XmlNodeType.EndElement) && (NodeReader.Name.Trim().ToUpper() == "HI:BANNER"))
                {
                    return;
                }
            }
        }

        private static void read_directives(XmlNodeReader NodeReader, Complete_Item_Aggregation HierarchyObject, string Directory)
        {
            string directiveCode = String.Empty;
            string directiveFile = String.Empty;
            while (NodeReader.Read())
            {
                // If this is the beginning tag for an element, assign the next values accordingly
                if (NodeReader.NodeType == XmlNodeType.Element)
                {
                    // Get the node name, trimmed and to upper
                    string nodeName = NodeReader.Name.Trim().ToUpper();

                    // switch the rest based on the tag name
                    switch (nodeName)
                    {
                        case "HI:CODE":
                            NodeReader.Read();
                            directiveCode = NodeReader.Value.Replace("<%","").Replace("%>","");
                            break;

                        case "HI:SOURCE":
                            NodeReader.Read();
                            directiveFile = NodeReader.Value;
                            break;
                    }
                }

                if (NodeReader.NodeType == XmlNodeType.EndElement)
                {
                    if (NodeReader.Name.Trim().ToUpper() == "HI:DIRECTIVE" )
                    {
                        if ((directiveCode.Length > 0) && (directiveFile.Length > 0))
                        {
                            string contents;
                            try
                            {
                                // Look for the matching file
                                if (File.Exists(Directory + "\\" + directiveFile))
                                {
                                    StreamReader reader = new StreamReader(Directory + "\\" + directiveFile);
                                    contents = reader.ReadToEnd();
                                    reader.Close();

                                }
                                else
                                {
                                    contents = "MISSING DIRECTIVE SOURCE FILE ('" + directiveFile + "')";
                                }
                            }
                            catch
                            {
                                contents = "EXCEPTION WHILE READING DIRECTIVE SOURCE FILE ('" + directiveFile + "')";
                            }

                            // Create the custom derivative object
                            Item_Aggregation_Custom_Directive newDirective = new Item_Aggregation_Custom_Directive(directiveCode, directiveFile, contents);
                            if ( HierarchyObject.Custom_Directives == null )
                                HierarchyObject.Custom_Directives = new Dictionary<string, Item_Aggregation_Custom_Directive>()
                            ;
                            HierarchyObject.Custom_Directives["<%" + directiveCode.ToUpper() + "%>"] = newDirective;

                        }
                        directiveCode = String.Empty;
                        directiveFile = String.Empty;
                    }

                    if (NodeReader.Name.Trim().ToUpper() == "HI:DIRECTIVES")
                    {
                        // Done with all the directives so return
                        return;
                    }
                }
            }
        }

        private static void read_highlights(XmlNodeReader NodeReader, Complete_Item_Aggregation HierarchyObject)
        {
            Complete_Item_Aggregation_Highlights highlight = new Complete_Item_Aggregation_Highlights();


            // Determine if this is a rotating type of highlight or not
            if (NodeReader.HasAttributes) 
            {
                if (NodeReader.MoveToAttribute("type"))
                {
                    if (NodeReader.Value == "ROTATING")
                        HierarchyObject.Rotating_Highlights = true;
                }

                if (HierarchyObject.Front_Banner_Dictionary != null)
                {
                    // The following three values are for reading legacy XML files.  These 
                    // data fields have been moved to be attached to the actual banner
                    if (NodeReader.MoveToAttribute("bannerSide"))
                    {

                        if (NodeReader.Value.Trim().ToUpper() == "RIGHT")
                        {
                            foreach (KeyValuePair<Web_Language_Enum, Item_Aggregation_Front_Banner> banners in HierarchyObject.Front_Banner_Dictionary)
                                banners.Value.Type = Item_Aggregation_Front_Banner_Type_Enum.Right;
                        }
                        else
                        {
                            foreach (KeyValuePair<Web_Language_Enum, Item_Aggregation_Front_Banner> banners in HierarchyObject.Front_Banner_Dictionary)
                                banners.Value.Type = Item_Aggregation_Front_Banner_Type_Enum.Left;
                        }
                    }
                    if (NodeReader.MoveToAttribute("bannerHeight"))
                    {
                        foreach (KeyValuePair<Web_Language_Enum, Item_Aggregation_Front_Banner> banners in HierarchyObject.Front_Banner_Dictionary)
                            banners.Value.Height = Convert.ToUInt16(NodeReader.Value);
                    }
                    if (NodeReader.MoveToAttribute("bannerWidth"))
                    {
                        foreach (KeyValuePair<Web_Language_Enum, Item_Aggregation_Front_Banner> banners in HierarchyObject.Front_Banner_Dictionary)
                            banners.Value.Width = Convert.ToUInt16(NodeReader.Value);
                    }
                }
            }

            while (NodeReader.Read())
            {
                // If this is the beginning tag for an element, assign the next values accordingly
                if (NodeReader.NodeType == XmlNodeType.Element)
                {
                    // Get the node name, trimmed and to upper
                    string nodeName = NodeReader.Name.Trim().ToUpper();

                    // switch the rest based on the tag name
                    string languageText;
                    switch (nodeName)
                    {
                        case "HI:SOURCE":
                            NodeReader.Read();
                            highlight.Image = NodeReader.Value.ToLower();
                            break;

                        case "HI:LINK":
                            NodeReader.Read();
                            highlight.Link = NodeReader.Value.ToLower();
                            break;

                        case "HI:TOOLTIP":
                            languageText = String.Empty;
                            if ((NodeReader.HasAttributes) && (NodeReader.MoveToAttribute("lang")))
                                languageText = NodeReader.Value.ToUpper();
                            NodeReader.Read();
                            highlight.Add_Tooltip( Web_Language_Enum_Converter.Code_To_Enum(languageText), NodeReader.Value );
                            break;

                        case "HI:TEXT":
                            languageText = String.Empty;
                            if ((NodeReader.HasAttributes) && (NodeReader.MoveToAttribute("lang")))
                                languageText = NodeReader.Value.ToUpper();
                            NodeReader.Read();
                            highlight.Add_Text(Web_Language_Enum_Converter.Code_To_Enum(languageText), NodeReader.Value);
                            break;
                    }
                }

                if (NodeReader.NodeType == XmlNodeType.EndElement)
                {
                    if (NodeReader.Name.Trim().ToUpper() == "HI:HIGHLIGHT" )
                    {
                        if (HierarchyObject.Highlights == null)
                            HierarchyObject.Highlights = new List<Complete_Item_Aggregation_Highlights>();
                        HierarchyObject.Highlights.Add(highlight);
                        highlight = new Complete_Item_Aggregation_Highlights();
                    }

                    if (NodeReader.Name.Trim().ToUpper() == "HI:HIGHLIGHTS")
                    {
                        // Done with all the highlights so return
                        return;
                    }
                }
            }
        }

        private static void read_browse(bool Browse, XmlNodeReader NodeReader, Complete_Item_Aggregation HierarchyObject)
        {
            // Create a new browse/info object
            Complete_Item_Aggregation_Child_Page newBrowse = new Complete_Item_Aggregation_Child_Page
                                {
                                    Browse_Type = Item_Aggregation_Child_Visibility_Enum.Main_Menu,
                                    Source_Data_Type = Item_Aggregation_Child_Source_Data_Enum.Static_HTML
                                };

            bool isDefault = false;

	        // Determine which XML node name to look for and set browse v. info
            string lastName = "HI:BROWSE";
            if (!Browse)
            {
                lastName = "HI:INFO";
                newBrowse.Browse_Type = Item_Aggregation_Child_Visibility_Enum.None;
            }

            // Check for the attributes
            if (NodeReader.HasAttributes)
            {
                if (NodeReader.MoveToAttribute("location"))
                {
                    if (NodeReader.Value == "BROWSEBY")
                        newBrowse.Browse_Type = Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By;
                }
                if (NodeReader.MoveToAttribute("default"))
                {
                    if (NodeReader.Value == "DEFAULT")
                        isDefault = true;
                }
				if (NodeReader.MoveToAttribute("visibility"))
				{
					switch (NodeReader.Value)
					{
						case "NONE":
                            newBrowse.Browse_Type = Item_Aggregation_Child_Visibility_Enum.None;
							break;

						case "MAIN_MENU":
                            newBrowse.Browse_Type = Item_Aggregation_Child_Visibility_Enum.Main_Menu;
							break;

						case "BROWSEBY":
                            newBrowse.Browse_Type = Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By;
							break;
					}
				}
				if (NodeReader.MoveToAttribute("parent"))
				{
					newBrowse.Parent_Code = NodeReader.Value;
				}
            }

            // Step through the XML and build this browse/info object
            while (NodeReader.Read())
            {
                // If this is the beginning tag for an element, assign the next values accordingly
                if (NodeReader.NodeType == XmlNodeType.Element)
                {
                    // Get the node name, trimmed and to upper
                    string nodeName = NodeReader.Name.Trim().ToUpper();

                    // switch the rest based on the tag name
                    switch (nodeName)
                    {
                        case "HI:METADATA":
                            NodeReader.Read();
                            newBrowse.Code = NodeReader.Value.ToLower();
                            newBrowse.Source_Data_Type = Item_Aggregation_Child_Source_Data_Enum.Database_Table;
                            break;

                        case "HI:CODE":
                            NodeReader.Read();
                            newBrowse.Code = NodeReader.Value.ToLower();
                            break;

                        case "HI:TITLE":
                            // Look for a language attached to this title
                            string titleLanguage = String.Empty;
                            if ((NodeReader.HasAttributes) && ( NodeReader.MoveToAttribute("lang")))
                            {
                                titleLanguage = NodeReader.GetAttribute("lang");
                            }
                            
                            // read and save the title
                            NodeReader.Read();
                            newBrowse.Add_Label( NodeReader.Value, Web_Language_Enum_Converter.Code_To_Enum(titleLanguage));
                            break;

                        case "HI:BODY":
                            // Look for a language attached to this title
                            string bodyLanguage = String.Empty;
                            if ((NodeReader.HasAttributes) && (NodeReader.MoveToAttribute("lang")))
                            {
                                bodyLanguage = NodeReader.GetAttribute("lang");
                            }

                            // read and save the title
                            NodeReader.Read();
                            string bodySource = NodeReader.Value;
                            newBrowse.Add_Static_HTML_Source(bodySource, Web_Language_Enum_Converter.Code_To_Enum(bodyLanguage));
                            break;
                    }
                }

                if (NodeReader.NodeType == XmlNodeType.EndElement)
                {
                    if (NodeReader.Name.Trim().ToUpper() == lastName )
                    {
						// Don't add ALL or NEW here
	                    if ((String.Compare(newBrowse.Code, "all", StringComparison.InvariantCultureIgnoreCase) != 0) && (String.Compare(newBrowse.Code, "new", StringComparison.InvariantCultureIgnoreCase) != 0))
	                    {
		                    HierarchyObject.Add_Child_Page(newBrowse);
		                    //HierarchyObject.Add

		                    // If this set the default browse by save that information
                            if ((newBrowse.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By) && (isDefault))
		                    {
			                    HierarchyObject.Default_BrowseBy = newBrowse.Code;
		                    }
	                    }

	                    return;
                    }
                }
            }
        }
    }
}
