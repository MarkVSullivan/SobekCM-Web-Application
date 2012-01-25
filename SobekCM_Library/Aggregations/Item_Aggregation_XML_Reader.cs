#region Using directives

using System;
using System.IO;
using System.Xml;
using SobekCM.Library.Application_State;
using SobekCM.Library.Navigation;

#endregion

namespace SobekCM.Library.Aggregations
{
    /// <summary> Class is used to read the item aggregation configuration XML files (when they exist)
    /// and populate the home page source files, banner information, highlights, and the browse and 
    /// informational pages connected to the item aggregation.</summary>
    public class Item_Aggregation_XML_Reader
    {
        /// <summary> Reads the item aggregation configuration file and populates the new data into the
        /// item aggregation object </summary>
        /// <param name="hierarchyObject"> Item aggregation object to populate</param>
        /// <param name="file_location"> Full name of the item aggregation configuration XML file </param>
        public void Add_Info_From_XML_File(Item_Aggregation hierarchyObject, string file_location )
        {
            // Get the directory from the file location
            string directory = (new FileInfo(file_location)).DirectoryName;

            // Load this XML file
            XmlDocument hierarchyXml = new XmlDocument();
            hierarchyXml.Load(file_location);

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
                            read_settings(nodeReader, hierarchyObject);
                            break;

                        case "HI:HOME":
                            read_home(nodeReader, hierarchyObject);
                            break;

                        case "HI:BANNER":
                            read_banners(nodeReader, hierarchyObject);
                            break;

                        case "HI:DIRECTIVES":
                            read_directives(nodeReader, hierarchyObject, directory);
                            break;

                        case "HI:HIGHLIGHTS":
                            read_highlights(nodeReader, hierarchyObject);
                            break;

                        case "HI:BROWSE":
                            read_browse(true, nodeReader, hierarchyObject);
                            break;

                        case "HI:INFO":
                            read_browse(false, nodeReader, hierarchyObject);
                            break;

                        case "HI:RESULTS":
                            read_results_specs(nodeReader, hierarchyObject);
                            break;
                    }
                }
            }
        }

        private static void read_results_specs(XmlNodeReader nodeReader, Item_Aggregation hierarchyObject)
        {
            bool inViews = false;
            while (nodeReader.Read())
            {
                // If this is the beginning tag for an element, assign the next values accordingly
                string nodeName;
                if (nodeReader.NodeType == XmlNodeType.Element)
                {
                    // Get the node name, trimmed and to upper
                    nodeName = nodeReader.Name.Trim().ToUpper();

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
                                if (nodeReader.MoveToAttribute("default"))
                                {
                                    isDefault = true;
                                }
                                if (nodeReader.MoveToAttribute("type"))
                                {
                                    type = nodeReader.Value.ToUpper();
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
                                        if (!hierarchyObject.Result_Views.Contains(displayType))
                                        {
                                            hierarchyObject.Result_Views.Add(displayType);
                                        }
                                        if (isDefault)
                                        {
                                            hierarchyObject.Default_Result_View = displayType;
                                        }
                                    }
                                }
                            }
                            break;

                        case "HI:REMOVE":
                            if (inViews)
                            {
                                string type = String.Empty;
                                if (nodeReader.MoveToAttribute("type"))
                                {
                                    type = nodeReader.Value.ToUpper();
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
                                        if (hierarchyObject.Result_Views.Contains(displayType))
                                        {
                                            hierarchyObject.Result_Views.Remove(displayType);
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }

                // If this is not an end element, continue
                if (nodeReader.NodeType != XmlNodeType.EndElement) continue;

                // Get the node name, trimmed and to upper
                nodeName = nodeReader.Name.Trim().ToUpper();

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

        private static void read_settings(XmlNodeReader nodeReader, Item_Aggregation hierarchyObject)
        {
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
                        case "HI:WEBSKINS":
                            nodeReader.Read();
                            string webskins = nodeReader.Value;
                            string[] splitter = webskins.Split(",".ToCharArray());
                            foreach (string thisSplitter in splitter)
                            {
                                if ( thisSplitter.Length > 0 )
                                    hierarchyObject.Add_Web_Skin(thisSplitter.ToLower());
                            }
                            break;

                        case "HI:FACETS":
                            nodeReader.Read();
                            string facets = nodeReader.Value;
                            string[] splitter2 = facets.Split(",".ToCharArray());
                            hierarchyObject.Clear_Facets();
                            foreach (string thisSplitter2 in splitter2)
                            {
                                    hierarchyObject.Add_Facet(Convert.ToInt16(thisSplitter2));
                            }
                            break;
                    }
                }

                if (nodeReader.NodeType == XmlNodeType.EndElement)
                {
                    if (nodeReader.Name.Trim().ToUpper() == "HI:SETTINGS")
                    {
                        return;
                    }
                }
            }
        }

        private static void read_home(XmlNodeReader nodeReader, Item_Aggregation hierarchyObject)
        {
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
                        case "HI:BODY":
                            if ((nodeReader.HasAttributes) && (nodeReader.MoveToAttribute("lang")))
                            {
                                string bodyLanguage = nodeReader.GetAttribute("lang");
                                nodeReader.Read();
                                hierarchyObject.Add_Home_Page_File(  nodeReader.Value, Language_Enum_Converter.Code_To_Language_Enum(bodyLanguage));
                            }
                            else
                            {
                                nodeReader.Read();
                                hierarchyObject.Add_Home_Page_File( nodeReader.Value, Language_Enum.DEFAULT);
                            }

                            break;
                    }
                }

                if ((nodeReader.NodeType == XmlNodeType.EndElement) && (nodeReader.Name.Trim().ToUpper() == "HI:HOME"))
                {
                    return;
                }
            }
        }

        private static void read_banners(XmlNodeReader nodeReader, Item_Aggregation hierarchyObject )
        {
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
                        case "HI:SOURCE":
                            // Check for any attributes to this banner node
                            string lang = String.Empty;
                            bool special = false;

                            if (nodeReader.HasAttributes)
                            {

                                if (nodeReader.MoveToAttribute("lang"))
                                {
                                    lang = nodeReader.Value.Trim().ToUpper();
                                }
                                if (nodeReader.MoveToAttribute("type"))
                                {
                                    if (nodeReader.Value.Trim().ToUpper() == "HIGHLIGHT")
                                        special = true;
                                }
                            }

                            // Now read the banner information and add to the aggregation object
                            nodeReader.Read();
                            if (special)
                            {
                                hierarchyObject.Add_Front_Banner_Image(nodeReader.Value, Language_Enum_Converter.Code_To_Language_Enum( lang));
                            }
                            else
                            {
                                hierarchyObject.Add_Banner_Image(nodeReader.Value, Language_Enum_Converter.Code_To_Language_Enum(lang));
                            }


                            break;
                    }
                }

                if ((nodeReader.NodeType == XmlNodeType.EndElement) && (nodeReader.Name.Trim().ToUpper() == "HI:BANNER"))
                {
                    return;
                }
            }
        }

        private static void read_directives( XmlNodeReader nodeReader, Item_Aggregation hierarchyObject, string directory )
        {
            string directiveCode = String.Empty;
            string directiveFile = String.Empty;
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
                        case "HI:CODE":
                            nodeReader.Read();
                            directiveCode = nodeReader.Value.Replace("<%","").Replace("%>","");
                            break;

                        case "HI:SOURCE":
                            nodeReader.Read();
                            directiveFile = nodeReader.Value;
                            break;
                    }
                }

                if (nodeReader.NodeType == XmlNodeType.EndElement)
                {
                    if (nodeReader.Name.Trim().ToUpper() == "HI:DIRECTIVE" )
                    {
                        if ((directiveCode.Length > 0) && (directiveFile.Length > 0))
                        {
                            string contents;
                            try
                            {
                                // Look for the matching file
                                if (File.Exists(directory + "\\" + directiveFile))
                                {
                                    StreamReader reader = new StreamReader(directory + "\\" + directiveFile);
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
                            hierarchyObject.Custom_Directives["<%" + directiveCode.ToUpper() + "%>"] = newDirective;

                        }
                        directiveCode = String.Empty;
                        directiveFile = String.Empty;
                    }

                    if (nodeReader.Name.Trim().ToUpper() == "HI:DIRECTIVES")
                    {
                        // Done with all the directives so return
                        return;
                    }
                }
            }
        }
    
        private static void read_highlights( XmlNodeReader nodeReader, Item_Aggregation hierarchyObject )
        {
            Item_Aggregation_Highlights highlight = new Item_Aggregation_Highlights();


            // Determine if this is a rotating type of highlight or not
            if (nodeReader.HasAttributes) 
            {
                if (nodeReader.MoveToAttribute("type"))
                {
                    if (nodeReader.Value == "ROTATING")
                        hierarchyObject.Rotating_Highlights = true;
                }
                if (nodeReader.MoveToAttribute("bannerSide"))
                {
                    if (nodeReader.Value.Trim().ToUpper() == "RIGHT")
                        hierarchyObject.Front_Banner_Left_Side = false;
                }
                if (nodeReader.MoveToAttribute("bannerHeight"))
                {
                    hierarchyObject.Front_Banner_Height = Convert.ToUInt16(nodeReader.Value);
                }
                if (nodeReader.MoveToAttribute("bannerWidth"))
                {
                    hierarchyObject.Front_Banner_Width = Convert.ToUInt16(nodeReader.Value);
                }
            }

            while (nodeReader.Read())
            {
                // If this is the beginning tag for an element, assign the next values accordingly
                if (nodeReader.NodeType == XmlNodeType.Element)
                {
                    // Get the node name, trimmed and to upper
                    string nodeName = nodeReader.Name.Trim().ToUpper();

                    // switch the rest based on the tag name
                    string languageText;
                    switch (nodeName)
                    {
                        case "HI:SOURCE":
                            nodeReader.Read();
                            highlight.Image = nodeReader.Value.ToLower();
                            break;

                        case "HI:LINK":
                            nodeReader.Read();
                            highlight.Link = nodeReader.Value.ToLower();
                            break;

                        case "HI:TOOLTIP":
                            languageText = String.Empty;
                            if ((nodeReader.HasAttributes) && (nodeReader.MoveToAttribute("lang")))
                                languageText = nodeReader.Value.ToUpper();
                            nodeReader.Read();
                            highlight.Add_Tooltip( Language_Enum_Converter.Code_To_Language_Enum(languageText), nodeReader.Value );
                            break;

                        case "HI:TEXT":
                            languageText = String.Empty;
                            if ((nodeReader.HasAttributes) && (nodeReader.MoveToAttribute("lang")))
                                languageText = nodeReader.Value.ToUpper();
                            nodeReader.Read();
                            highlight.Add_Text(Language_Enum_Converter.Code_To_Language_Enum(languageText), nodeReader.Value);
                            break;
                    }
                }

                if (nodeReader.NodeType == XmlNodeType.EndElement)
                {
                    if (nodeReader.Name.Trim().ToUpper() == "HI:HIGHLIGHT" )
                    {
                        hierarchyObject.Highlights.Add(highlight);
                        highlight = new Item_Aggregation_Highlights();
                    }

                    if (nodeReader.Name.Trim().ToUpper() == "HI:HIGHLIGHTS")
                    {
                        // Done with all the highlights so return
                        return;
                    }
                }
            }
        }

        private static void read_browse(bool browse, XmlNodeReader nodeReader, Item_Aggregation hierarchyObject )
        {
            // Create a new browse/info object
            Item_Aggregation_Browse_Info newBrowse = new Item_Aggregation_Browse_Info
                                {
                                    Browse_Type = Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_Home,
                                    Source = Item_Aggregation_Browse_Info.Source_Type.Static_HTML,
                                    Data_Type = Item_Aggregation_Browse_Info.Result_Data_Type.Text
                                };

            bool isDefault = false;

            string code = String.Empty;

            // Determine which XML node name to look for and set browse v. info
            string lastName = "HI:BROWSE";
            if (!browse)
            {
                lastName = "HI:INFO";
                newBrowse.Browse_Type = Item_Aggregation_Browse_Info.Browse_Info_Type.Info;
            }

            // Check for the attributes
            if (nodeReader.HasAttributes)
            {
                if (nodeReader.MoveToAttribute("location"))
                {
                    if (nodeReader.Value == "BROWSEBY")
                        newBrowse.Browse_Type = Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_By;
                }
                if (nodeReader.MoveToAttribute("default"))
                {
                    if (nodeReader.Value == "DEFAULT")
                        isDefault = true;
                }
            }

            // Step through the XML and build this browse/info object
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
                        case "HI:METADATA":
                            nodeReader.Read();
                            newBrowse.Code = nodeReader.Value.ToLower();
                            newBrowse.Source = Item_Aggregation_Browse_Info.Source_Type.Database;
                            newBrowse.Data_Type = Item_Aggregation_Browse_Info.Result_Data_Type.Table;
                            break;

                        case "HI:CODE":
                            nodeReader.Read();
                            newBrowse.Code = nodeReader.Value.ToLower();
                            break;

                        case "HI:TITLE":
                            // Look for a language attached to this title
                            string titleLanguage = String.Empty;
                            if ((nodeReader.HasAttributes) && ( nodeReader.MoveToAttribute("lang")))
                            {
                                titleLanguage = nodeReader.GetAttribute("lang");
                            }
                            
                            // read and save the title
                            nodeReader.Read();
                            newBrowse.Add_Label( nodeReader.Value, Language_Enum_Converter.Code_To_Language_Enum(titleLanguage));
                            break;

                        case "HI:BODY":
                            // Look for a language attached to this title
                            string bodyLanguage = String.Empty;
                            if ((nodeReader.HasAttributes) && (nodeReader.MoveToAttribute("lang")))
                            {
                                bodyLanguage = nodeReader.GetAttribute("lang");
                            }

                            // read and save the title
                            nodeReader.Read();
                            string bodySource = nodeReader.Value;
                            if ((bodySource.IndexOf("http://") < 0) && (bodySource.IndexOf("<%BASEURL%>") < 0))
                                bodySource = SobekCM_Library_Settings.Base_Design_Location + hierarchyObject.objDirectory + bodySource;
                            newBrowse.Add_Static_HTML_Source(bodySource, Language_Enum_Converter.Code_To_Language_Enum(bodyLanguage));
                            break;
                    }
                }

                if (nodeReader.NodeType == XmlNodeType.EndElement)
                {
                    if (nodeReader.Name.Trim().ToUpper() == lastName )
                    {
                        hierarchyObject.Add_Browse_Info(newBrowse);

                        // If this set the default browse by save that information
                        if ((newBrowse.Browse_Type == Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_By) && (isDefault))
                        {
                            hierarchyObject.Default_BrowseBy = newBrowse.Code;
                        }

                        return;
                    }
                }
            }
        }
    }
}
