#region Using directives

using System;
using System.IO;
using System.Xml;
using SobekCM.Library.Citation.Elements;

#endregion

namespace SobekCM.Library.Citation.Template
{
    /// <summary> Reader for the template XML configuration file which stores the information about a single metadata template </summary>
    public class Template_XML_Reader
    {
        private bool complexMainTitleExists;

        /// <summary> Reads the template XML configuration file specified into a template object </summary>
        /// <param name="XML_File"> Filename of the template XML configuraiton file to read  </param>
        /// <param name="thisTemplate"> Template object to populate form the configuration file </param>
        /// <param name="exclude_divisions"> Flag indicates whether to include the structure map, if included in the template file </param>
        public void Read_XML( string XML_File, Template thisTemplate, bool exclude_divisions )
        {
            // Set some default for this read
            complexMainTitleExists = false;

            // Load this MXF File
            XmlDocument templateXml = new XmlDocument();
            templateXml.Load( XML_File );

            // create the node reader
            XmlNodeReader nodeReader = new XmlNodeReader( templateXml );

            // Read through all main input template tag is found
            move_to_node( nodeReader, "input_template" );

            // Process all of the header information for this template
            process_template_header( nodeReader, thisTemplate );

            // Process all of the input portion / hierarchy
            process_inputs(nodeReader, thisTemplate, exclude_divisions);

            // Process any constant sectoin
            process_constants( nodeReader, thisTemplate );
        }

        private void process_template_header( XmlNodeReader nodeReader, Template thisTemplate )
        {
            // Read all the nodes
            while ( nodeReader.Read() )
            {
                // Get the node name, trimmed and to upper
                string nodeName = nodeReader.Name.Trim().ToUpper();

                // If this is the inputs or constant start tag, return
                if (( nodeReader.NodeType == XmlNodeType.Element ) && 
                    (( nodeName == "INPUTS" ) || ( nodeName == "CONSTANTS" )))
                {
                    return;
                }

                // If this is the beginning tag for an element, assign the next values accordingly
                if ( nodeReader.NodeType == XmlNodeType.Element )
                {
                    // switch the rest based on the tag name
                    switch( nodeName )
                    {
                        case "BANNER":
                            thisTemplate.Banner = read_text_node(nodeReader);
                            break;

                        case "INCLUDEUSERASAUTHOR":
                            thisTemplate.Include_User_As_Author = Convert.ToBoolean(read_text_node(nodeReader));
                            break;

                        case "UPLOADS":
                            string upload_type_text = read_text_node(nodeReader).Trim().ToUpper();
                            switch (upload_type_text)
                            {
                                case "NONE":
                                    thisTemplate.Upload_Types = Template.Template_Upload_Types.None;
                                    break;

                                case "FILE":
                                    thisTemplate.Upload_Types = Template.Template_Upload_Types.File;
                                    break;

                                case "URL":
                                    thisTemplate.Upload_Types = Template.Template_Upload_Types.URL;
                                    break;

                                case "FILE_OR_URL":
                                    thisTemplate.Upload_Types = Template.Template_Upload_Types.File_or_URL;
                                    break;

                                default:
                                    thisTemplate.Upload_Types = Template.Template_Upload_Types.File;
                                    break;

                            }
                            break;

                        case "UPLOADMANDATORY":
                            thisTemplate.Upload_Mandatory = Convert.ToBoolean(read_text_node(nodeReader));
                            break;

                        case "NAME":
                            thisTemplate.Title = read_text_node(nodeReader);
                            break;

                        case "PERMISSIONS":
                            thisTemplate.Permissions_Agreement = read_text_node(nodeReader);
                            break;

                        case "NOTES":
                            thisTemplate.Notes = (thisTemplate.Notes + "  " + read_text_node( nodeReader )).Trim();
                            break;

                        case "DATECREATED":
                            DateTime dateCreated;
                            if ( DateTime.TryParse(read_text_node(nodeReader), out dateCreated))
                                thisTemplate.DateCreated = dateCreated;
                            break;

                        case "LASTMODIFIED":
                            DateTime lastModified;
                            if (DateTime.TryParse(read_text_node(nodeReader), out lastModified))
                                thisTemplate.LastModified = lastModified;
                            break;

                        case "CREATOR":
                            thisTemplate.Creator = read_text_node( nodeReader );
                            break;

                        case "BIBIDROOT":
                            thisTemplate.BibID_Root = read_text_node(nodeReader);
                            break;

                        case "DEFAULTVISIBILITY":
                            short defaultVisibility;
                            if (Int16.TryParse(read_text_node(nodeReader), out defaultVisibility))
                                thisTemplate.Default_Visibility = defaultVisibility;
                            break;

                        case "EMAILUPONSUBMIT":
                            thisTemplate.Email_Upon_Receipt = read_text_node(nodeReader);
                            break;
                    }
                }
            }
        }

        private void process_inputs( XmlNodeReader nodeReader, Template thisTemplate, bool exclude_divisions )
        {
            // Keep track of the current pages and panels
            Template_Page currentPage = null;
            Template_Panel currentPanel = null;
            bool inPanel = false;

            // Read all the nodes
            while ( nodeReader.Read() )
            {
                // Get the node name, trimmed and to upper
                string nodeName = nodeReader.Name.Trim().ToUpper();

                // If this is the inputs or constant start tag, return
                if ((( nodeReader.NodeType == XmlNodeType.EndElement ) && ( nodeName == "INPUTS" )) ||
                    (( nodeReader.NodeType == XmlNodeType.Element ) && ( nodeReader.Name == "CONSTANTS")))
                {
                    return;
                }

                // If this is the beginning tag for an element, assign the next values accordingly
                if ( nodeReader.NodeType == XmlNodeType.Element )
                {
                    // Does this start a new page?
                    if ( nodeName == "PAGE" )
                    {
                        // Set the inPanel flag to false
                        inPanel = false;

                        // Create the new page and add to this template
                        currentPage = new Template_Page();
                        thisTemplate.Add_Page( currentPage );
                    }

                    // Does this start a new panel?
                    if (( nodeName == "PANEL" ) && ( currentPage != null ))
                    {
                        // Set the inPanel flag to true
                        inPanel = true;

                        // Create the new panel and add to the current page
                        currentPanel = new Template_Panel();
                        currentPage.Add_Panel( currentPanel );
                    }

                    // Is this a name element?
                    if ((nodeName == "NAME") && (currentPage != null))
                    {
                        // Get the text
                        string title = read_text_node( nodeReader );

                        // Set the name for either the page or panel
                        if ( inPanel )
                        {
                            currentPanel.Title = title;
                        }
                        else
                        {
                            currentPage.Title = title;
                        }
                    }

                    // Is this a name element?
                    if ((nodeName == "INSTRUCTIONS") && (currentPage != null))
                    {
                        // Get the text
                        string instructions = read_text_node(nodeReader);

                        // Set the name for either the page or panel
                        if (!inPanel)
                        {
                            currentPage.Instructions = instructions;
                        }
                    }

                    // Is this a new element?
                    if ((nodeName == "ELEMENT") && (nodeReader.HasAttributes) && (currentPanel != null))
                    {
                        abstract_Element currentElement = process_element( nodeReader, thisTemplate.InputPages.Count );
                        if (( currentElement != null ) && (( !exclude_divisions ) || ( currentElement.Type != Element_Type.Structure_Map )))
                            currentPanel.Add_Element( currentElement );
                    }
                }
            }
        }

        private abstract_Element process_element( XmlNodeReader nodeReader, int current_page_count )
        {
            string type = String.Empty;
            string subtype = String.Empty;

            // Step through all the attributes until the type is found
            nodeReader.MoveToFirstAttribute();
            do
            {
                // Get the type attribute
                if ( nodeReader.Name.ToUpper().Trim() == "TYPE" )
                {
                    type = nodeReader.Value;
                }

                // Get the subtype attribute
                if ( nodeReader.Name.ToUpper().Trim() == "SUBTYPE" )
                {
                    subtype = nodeReader.Value;
                }

            } while (nodeReader.MoveToNextAttribute() );

            // Make sure a type was specified
            if ( type == String.Empty )
                return null;

            // Build the element
            abstract_Element newElement = Element_Factory.getElement( type, subtype );

            // If thie element was null, return null
            if (newElement == null)
                return null;

            // Set the page number for post back reasons
            newElement.Template_Page = current_page_count;

            // Some special logic here
            if ((newElement.Type == Element_Type.Type) && ( newElement.Display_SubType == "form" ))
            {
                (( Type_Format_Form_Element )newElement).Set_Postback("javascript:__doPostBack('newpagebutton" + current_page_count + "','')");
            }


            if ((newElement.Type == Element_Type.Title) && (newElement.Display_SubType == "form"))
            {
                complexMainTitleExists = true;
            }

            if ((newElement.Type == Element_Type.Note) && (newElement.Display_SubType == "complex"))
            {
                ((Note_Complex_Element)newElement).Include_Statement_Responsibility = !complexMainTitleExists;
            }

            // Now, step through all the attributes again
            nodeReader.MoveToFirstAttribute();
            do
            {

                    switch( nodeReader.Name.ToUpper().Trim() )
                    {
                        case "REPEATABLE":
                            bool repeatable;
                            if ( Boolean.TryParse( nodeReader.Value, out repeatable ))
                                newElement.Repeatable = repeatable;
                            break;
                        case "MANDATORY":
                            bool mandatory;
                            if (Boolean.TryParse(nodeReader.Value, out mandatory))
                                newElement.Mandatory = mandatory;
                            break;
                        case "READONLY":
                            bool isReadOnly;
                            if (Boolean.TryParse(nodeReader.Value, out isReadOnly))
                                newElement.Read_Only = isReadOnly;
                            break;
                        case "ACRONYM":
                            newElement.Acronym = nodeReader.Value;
                            break;
                    }
            } while (nodeReader.MoveToNextAttribute() );

            // Move back to the element, if there were attributes (should be)
            nodeReader.MoveToElement();

            // Is there element_data?
            if ( !nodeReader.IsEmptyElement )
            {
                nodeReader.Read();
                if (( nodeReader.NodeType == XmlNodeType.Element ) && ( nodeReader.Name.ToLower() == "element_data" ))
                {
                    // Create the new tree
                    StringWriter sw = new StringWriter();
                    XmlTextWriter tw = new XmlTextWriter( sw );
                    tw.WriteNode( nodeReader, true );
                    tw.Close();

                    // Let the element process this inner data
                    newElement.Read_XML( new XmlTextReader( new StringReader( sw.ToString() )));
                }
            }

            // Return this built element
            return newElement;
        }

        private void process_constants( XmlNodeReader nodeReader, Template thisTemplate )
        {
            // Read all the nodes
            while ( nodeReader.Read() )
            {
                // Get the node name, trimmed and to upper
                string nodeName = nodeReader.Name.Trim().ToUpper();

                // If this is the inputs or constant start tag, return
                if (( nodeReader.NodeType == XmlNodeType.EndElement ) && ( nodeName == "CONSTANTS" ))
                {
                    return;
                }

                // If this is the beginning tag for an element, assign the next values accordingly
                if (( nodeReader.NodeType == XmlNodeType.Element ) && ( nodeName == "ELEMENT" ) && ( nodeReader.HasAttributes ))
                {
                    abstract_Element newConstant = process_element( nodeReader, -1 );
                    if (newConstant != null)
                    {
                        newConstant.isConstant = true;
                        thisTemplate.Add_Constant(newConstant);
                    }
                }
            }
        }

        private static void move_to_node( XmlNodeReader nodeReader, string nodeName )
        {
            while (( nodeReader.Read() ) && ( nodeReader.Name.Trim() != nodeName ))
            {
                // Do nothing here... 
            }
        }

        private static string read_text_node( XmlNodeReader nodeReader )
        {
            if (( nodeReader.Read() ) && ( nodeReader.NodeType == XmlNodeType.Text ))
            {
                return nodeReader.Value.Trim();
            }
            return String.Empty;
        }
    }
}
