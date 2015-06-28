#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM.Resource_Object.Metadata_File_ReaderWriters
{
    /// <summary> MXF metadata reader/writer </summary>
    /// <remarks>MXF files were used by PALMM for many years to submit bibliographic packages.  This has now been replaced by METS, but many legacy files still remain. </remarks>
    public class MXF_File_ReaderWriter : iMetadata_File_ReaderWriter
    {
        #region iMetadata_File_ReaderWriter Members

        /// <summary> Flag indicates if this reader/writer can read from files and streams </summary>
        /// <value> This property always returns TRUE </value>
        public bool canRead
        {
            get { return true; }
        }

        /// <summary> Flag indicates if this reader/writer can write to files and streams </summary>
        /// <value> This property always return FALSE </value>
        public bool canWrite
        {
            get { return false; }
        }

        /// <summary> Full name which best describes the metadata format this reader/writer utilizes (i.e. Dublin Core, Greenstone file, etc.. ) </summary>
        /// <value>This property always returns 'Metadata eXchange Format'</value>
        public string Metadata_Type_Name
        {
            get { return "Metadata eXchange Format"; }
        }

        /// <summary> Abbreviation for the metadata format utilized by this reader/writer (i.e., DC, MODS, GSA, etc.. ) </summary>
        /// <value> This property always returns 'MXF'</value>
        public string Metadata_Type_Abbreviation
        {
            get { return "MXF"; }
        }

        /// <summary> Reads metadata from an existing metadata file and saves to the provided item/package </summary>
        /// <param name="MetadataFilePathName"> Path and name of the metadata file to read </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Read_Metadata(string MetadataFilePathName, SobekCM_Item Return_Package, Dictionary<string, object> Options, out string Error_Message)
        {
            // Set default error outpt message
            Error_Message = String.Empty;

            // Old MXF files will not have a VID, so assign 00001
            if (Return_Package.VID.Length == 0)
            {
                Return_Package.VID = "00001";
            }

            // Load this MXF File
            XmlDocument mxfXML = new XmlDocument();
            mxfXML.Load(MetadataFilePathName);

            // Set the source directory correctly
            Return_Package.Source_Directory = (new FileInfo(MetadataFilePathName)).DirectoryName;

            // create the node reader
            XmlNodeReader nodeReader = new XmlNodeReader(mxfXML);

            // Read through all the nodes until the package tag is found
            move_to_node(nodeReader, "package");

            // Get the package attribute
            process_package_tag(nodeReader, Return_Package);

            // Read through all the nodes until the packageDesc section is found
            move_to_node(nodeReader, "packageDesc");

            // Process all of the packageDesc sections
            process_packageDesc(nodeReader, Return_Package);

            // Read through to the beginning of the entity description
            move_to_node(nodeReader, "entityDesc");

            // Process the entire entity tag and the projects tag
            process_entity_tag_and_project(nodeReader, Return_Package);

            // Set the object id and some other METS values not in the MXF
            Return_Package.METS_Header.ObjectID = Return_Package.BibID + "_" + Return_Package.VID;

            // Move to the bibDesc
            move_to_node(nodeReader, "bibDesc");

            // Process the bib desc section
            process_bib_desc(nodeReader, Return_Package);

            // Will just use a text reader to step through all of the division information
            process_divisions(MetadataFilePathName, Return_Package);

            nodeReader.Close();

            return true;
        }

        /// <summary> Reads metadata from an open stream and saves to the provided item/package </summary>
        /// <param name="Input_Stream"> Open stream to read metadata from </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Read_Metadata(Stream Input_Stream, SobekCM_Item Return_Package, Dictionary<string, object> Options, out string Error_Message)
        {
            throw new NotImplementedException();
        }

        /// <summary> Writes the formatted metadata from the provided item to a file </summary>
        /// <param name="MetadataFilePathName"> Path and name of the metadata file to write</param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_Metadata(string MetadataFilePathName, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            throw new NotImplementedException();
        }

        /// <summary> Writes the formatted metadata from the provided item to a TextWriter (usually to an output stream) </summary>
        /// <param name="Output_Stream"></param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_Metadata(TextWriter Output_Stream, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Methods pulled from old MXF_Reader for reading

        private void process_divisions(string MXF_File, SobekCM_Item thisPackage)
        {
            // Get the directory
            string directory = (new FileInfo(MXF_File)).Directory.ToString();
            thisPackage.Source_Directory = directory;

            // Create the reader to step through all of the lines of MXF
            StreamReader reader = new StreamReader(MXF_File);

            // Clear out all the divisions
            thisPackage.Divisions.Clear();

            // Get to the closing tag for the bibdesc
            string skip = reader.ReadLine();
            while ((skip != null) && (skip.Trim().ToUpper() != "</BIBDESC>"))
            {
                skip = reader.ReadLine();
            }

            // Define the necessary variables for the work below
            //int fileid = 1, divid = 1, pageid = 1, indexOf;
            //ushort order = 1;
            //short depth = -1;
            //string fileName, next_sub, type, next_upper, lastDivID = String.Empty;
            //string[] splitter;
            //abstract_TreeNode lastDivision = null;
            //abstract_TreeNode[] treeNodes = new abstract_TreeNode[10];
            //Hashtable filesHandled = new Hashtable();

            //// Loop through each line
            //string next = reader.ReadLine();
            //while( next != null )
            //{
            //    // Trim this
            //    next = next.Trim();
            //    next_upper = next.ToUpper();

            //    // Save this if this is either <HEAD, <FIL, or <DIV
            //    if ( next.Length > 4 )
            //    {
            //        next_sub = next_upper.Substring( 0, 4 );
            //        if (( next_sub == "<HEA" ) || ( next_sub == "<FIL" ) || ( next_sub == "<DIV" ) || ( next_sub == "</DI"))
            //        {
            //            // Is this a page/file definition?
            //            if (( next_sub == "<FIL" ) && ( next_upper.IndexOf("FILENAME") > 0 ))
            //            {
            //                // Get the file name
            //                indexOf = next_upper.IndexOf("<FILENAME>");
            //                fileName = next.Substring( indexOf + 10, next_upper.IndexOf("</FILENAME>" ) - indexOf - 10 );
            //                splitter = fileName.Split( "\\/.".ToCharArray() );
            //                fileName = splitter[ splitter.Length - 2 ];

            //                // Add the files if they have not been handled already
            //                if ( !filesHandled.Contains( fileName ))
            //                {
            //                    // Get the files 
            //                    string[] files = Directory.GetFiles( directory, fileName + ".*" );

            //                    // Mark this file as handled
            //                    filesHandled.Add( fileName, fileName );

            //                    // Add each file 
            //                    foreach( string thisFile in files )
            //                    {
            //                        // Add this file itself
            //                        fileName = (( new FileInfo( thisFile )).Name );
            //                        if ( fileName.ToUpper().IndexOf(".TIF") > 0 )
            //                        {
            //                            thisPackage.Divisions.Add_File( "F" + fileid, fileName, "G" + ( pageid - 1 ));
            //                            thisPackage.Divisions.Add_Page_File_Link( lastDivID, "F" + fileid );
            //                        }
            //                        if ( fileName.ToUpper().IndexOf(".TXT") > 0 )
            //                        {
            //                            thisPackage.Divisions.Add_File( "T" + fileid, fileName, "G" + ( pageid - 1 ));
            //                            thisPackage.Divisions.Add_Page_File_Link( lastDivID, "T" + fileid );
            //                        }
            //                        if ( fileName.ToUpper().IndexOf(".JPG") > 0 )
            //                        {
            //                            thisPackage.Divisions.Add_File( "J" + fileid, fileName, "G" + ( pageid - 1 ));
            //                            thisPackage.Divisions.Add_Page_File_Link( lastDivID, "J" + fileid );
            //                        }
            //                        if ( fileName.ToUpper().IndexOf(".JP2") > 0 )
            //                        {
            //                            thisPackage.Divisions.Add_File( "E" + fileid, fileName, "G" + ( pageid - 1 ));
            //                            thisPackage.Divisions.Add_Page_File_Link( lastDivID, "E" + fileid );
            //                        }
            //                    }

            //                    // Increment the file and division count
            //                    fileid++;
            //                }
            //            }


            //            // Is this the beginning of a division?
            //            if ( next_sub == "<DIV" )
            //            {
            //                // Get the TYPE, which is a required field
            //                type = "Chapter";
            //                if ( next_upper.IndexOf("TYPE=") > 0 )
            //                {
            //                    int typeIndex = next_upper.IndexOf("TYPE=") + 5;
            //                    type = next.Substring( typeIndex, next.IndexOf(">") - typeIndex ).Replace("\"","").Trim();
            //                }

            //                // Only continue if this is not the 'main' division
            //                if ( !type.ToUpper().Equals( "MAIN" ))
            //                {
            //                    // Create the identifier
            //                    if ( type.ToUpper().Equals( "PAGE" ))
            //                    {
            //                        lastDivID = "P" + pageid;
            //                        pageid++;
            //                    }
            //                    else
            //                    {
            //                        lastDivID = "D" + divid;
            //                        divid++;
            //                    }

            //                    // Create a new division, depending on which level this is
            //                    //if ( depth < 0 )
            //                    //{
            //                    //    lastDivision = thisPackage.Divisions.Add_Division( String.Empty, lastDivID, type, type, order );
            //                    //}
            //                    //else
            //                    //{
            //                    //    lastDivision = thisPackage.Divisions.Add_Division( ((abstract_TreeNode) treeNodes[depth]).Node_ID, lastDivID, type, type, order );
            //                    //}

            //                    // Move to the next depth and order
            //                    depth++;
            //                    order++;

            //                    // Save this in the list
            //                    treeNodes[ depth ] = lastDivision;
            //                }
            //            }

            //            // Is this the end of a division?
            //            if ( next_sub == "</DI")
            //            {
            //                // step back the depth one
            //                depth--;

            //                // Get the last division
            //                if ( depth >= 0 )
            //                {
            //                    lastDivision = (Division_TreeNode) treeNodes[ depth ];
            //                }
            //                else
            //                {
            //                    lastDivision = null;
            //                }
            //            }

            //            // Is this a HEAD tag for the last division?
            //            if (( next_sub == "<HEA" ) || ( next_sub == "<DIV" ))
            //            {
            //                if ( lastDivision != null )
            //                {
            //                    int headStart = next_upper.IndexOf("<HEAD>");
            //                    int headEnd = next_upper.IndexOf("</HEAD>");
            //                    if (( headStart >= 0 ) && ( headEnd > 0 ) && ( headStart < headEnd ))
            //                    {
            //                        lastDivision.Label = next.Substring( headStart + 6, headEnd - headStart - 6 );
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // Read the next line in 
            //    next = reader.ReadLine();
            //}

            // Close the reader
            reader.Close();
        }

        private void move_to_node(XmlNodeReader nodeReader, string nodeName)
        {
            while ((nodeReader.Read()) && (nodeReader.Name.Trim() != nodeName))
            {
                // Do nothing here... 
            }
        }

        private string read_text_node(XmlNodeReader nodeReader)
        {
            if ((nodeReader.Read()) && (nodeReader.NodeType == XmlNodeType.Text))
            {
                return nodeReader.Value.Trim();
            }
            else
            {
                return String.Empty;
            }
        }

        private void process_package_tag(XmlNodeReader nodeReader, SobekCM_Item thisPackage)
        {
            // Get the value of whether this is a new package
            if (nodeReader.HasAttributes)
            {
                // Move to what should be the bib id attribute
                nodeReader.MoveToAttribute(0);
                if (nodeReader.Name == "new")
                {
                    thisPackage.METS_Header.RecordStatus_Enum = METS_Record_Status.COMPLETE;
                }
            }
        }

        private void process_packageDesc(XmlNodeReader nodeReader, SobekCM_Item thisPackage)
        {
            // Get the bib id first
            if (nodeReader.HasAttributes)
            {
                // Move to what should be the bib id attribute
                nodeReader.MoveToAttribute(0);
                if (nodeReader.Name == "id")
                    thisPackage.BibID = nodeReader.Value.Trim();
            }

            // Read in the rest of the data until the closing packageDesc tag
            while ((nodeReader.Read()) && (nodeReader.Name.Trim() != "packageDesc"))
            {
                // Is this the opening tag for contrib?
                if ((nodeReader.NodeType == XmlNodeType.Element) && (nodeReader.Name.Trim() == "contrib"))
                {
                    // Get the attribute
                    if (nodeReader.HasAttributes)
                    {
                        nodeReader.MoveToAttribute(0);
                        if (nodeReader.Name.Trim() == "creator")
                        {
                            // Get the value
                            string creatorValue = nodeReader.Value;

                            // If there is a comma, parse it
                            if (creatorValue.IndexOf(",") > 0)
                            {
                                string[] split = creatorValue.Split(",".ToCharArray());
                                thisPackage.METS_Header.Creator_Software = split[0].Trim();
                                thisPackage.METS_Header.Creator_Individual = split[1].Trim();
                            }
                            else
                            {
                                thisPackage.METS_Header.Creator_Individual = creatorValue.Trim();
                            }
                        }
                    }

                    // Get the text of this... this is the institution that made the MXF
                    if ((nodeReader.Read()) && (nodeReader.NodeType == XmlNodeType.Text))
                    {
                        thisPackage.METS_Header.Creator_Organization = nodeReader.Value.Trim();
                    }
                }

                // Is this the opening tag for the timestamp?
                if ((nodeReader.NodeType == XmlNodeType.Element) && (nodeReader.Name.Trim() == "timestamp"))
                {
                    // Get the text of this... this is the timestamp
                    if ((nodeReader.Read()) && (nodeReader.NodeType == XmlNodeType.Text))
                    {
                        string mxf_dateString = nodeReader.Value.Trim();
                        string dateString = mxf_dateString.Substring(0, 4) + "/" + mxf_dateString.Substring(4, 2) +
                                            "/" + mxf_dateString.Substring(6, 2) + " " + mxf_dateString.Substring(9, 2) + ":" + mxf_dateString.Substring(11, 2) + ":" + mxf_dateString.Substring(13, 2);
                        thisPackage.METS_Header.Create_Date = Convert.ToDateTime(dateString);
                    }
                }
            }
        }

        private void process_entity_tag_and_project(XmlNodeReader nodeReader, SobekCM_Item thisPackage)
        {
            // Process the attributes on the entityDesc 
            int attributes = nodeReader.AttributeCount;
            for (int i = 0; i < attributes; i++)
            {
                // Go to this attribute
                nodeReader.MoveToAttribute(i);

                // If this is type, save it
                if (nodeReader.Name.Trim().ToUpper() == "TYPE")
                {
                    thisPackage.Bib_Info.Type.Add_Uncontrolled_Type(nodeReader.Value.Trim());
                }

                // If this is the source code, save that
                if (nodeReader.Name.Trim().ToUpper() == "SOURCE")
                {
                    thisPackage.Bib_Info.Source.Code = nodeReader.Value.Trim();
                    thisPackage.Bib_Info.Source.Statement = nodeReader.Value.Trim();
                }
            }

            // Read the project code.. first being the primary
            move_to_node(nodeReader, "projects");

            // Get the text from this node
            nodeReader.Read();
            string projectText = nodeReader.Value.Trim();
            thisPackage.Behaviors.Add_Aggregation(projectText);
        }

        private void process_bib_desc(XmlNodeReader nodeReader, SobekCM_Item thisPackage)
        {
            // Set some counters for the creator role/date and contributor role/date
            int creatorRole = 0;
            int creatorDate = 0;
            //int contribRole = 0;
            //int contribDate = 0;

            // Read all the nodes
            while (nodeReader.Read())
            {
                // If this is the end tag for bibDesc, return
                if ((nodeReader.NodeType == XmlNodeType.EndElement) && (nodeReader.Name.Trim().ToUpper() == "BIBDESC"))
                {
                    return;
                }

                // If this is the beginning tag for an element, assign the next values accordingly
                if (nodeReader.NodeType == XmlNodeType.Element)
                {
                    // Switch based on the element name
                    switch (nodeReader.Name.Trim().ToUpper())
                    {
                        case "DC.TITLE":
                            thisPackage.Bib_Info.Main_Title.Title = read_text_node(nodeReader);
                            break;
                        case "DC.RIGHTS":
                            thisPackage.Bib_Info.Access_Condition.Text = read_text_node(nodeReader);
                            break;
                        case "DC.IDENTIFIER":
                            thisPackage.Bib_Info.Add_Identifier(read_text_node(nodeReader));
                            break;
                        case "DC.DATE":
                            thisPackage.Bib_Info.Origin_Info.Date_Issued = read_text_node(nodeReader);
                            break;
                        case "DC.CREATOR":
                            thisPackage.Bib_Info.Add_Named_Entity(read_text_node(nodeReader), "creator");
                            break;
                        case "PALMM.CREATORROLE":
                            if (thisPackage.Bib_Info.Names_Count > creatorRole)
                            {
                                if (thisPackage.Bib_Info.Names[creatorRole].Roles.Count == 0)
                                {
                                    thisPackage.Bib_Info.Names[creatorRole].Roles.Add(new Name_Info_Role(read_text_node(nodeReader), Name_Info_Role_Type_Enum.Text));
                                }
                                else
                                {
                                    thisPackage.Bib_Info.Names[creatorRole].Roles[0].Role = read_text_node(nodeReader);
                                }
                                creatorRole++;
                            }
                            break;
                        case "PALMM.CREATORDATES":
                            if (thisPackage.Bib_Info.Names_Count > creatorDate)
                            {
                                thisPackage.Bib_Info.Names[creatorDate++].Dates = read_text_node(nodeReader);
                            }
                            break;
                        case "DC.CONTRIBUTOR":
                            thisPackage.Bib_Info.Add_Named_Entity(new Name_Info(read_text_node(nodeReader), "contributor"));
                            break;
                        case "DC.DESCRIPTION":
                            thisPackage.Bib_Info.Add_Note(new Note_Info(read_text_node(nodeReader)));
                            break;
                        case "DC.SUBJECT":
                            if (nodeReader.HasAttributes)
                            {
                                nodeReader.MoveToAttribute(0);
                                string scheme = nodeReader.Value.Trim();
                                thisPackage.Bib_Info.Add_Subject(new Subject_Info_Standard(read_text_node(nodeReader), scheme));
                            }
                            else
                            {
                                thisPackage.Bib_Info.Add_Subject(new Subject_Info_Standard(read_text_node(nodeReader), String.Empty));
                            }
                            break;
                        case "PALMM.SPATIALNAME":
                            if (nodeReader.HasAttributes)
                            {
                                nodeReader.MoveToAttribute(0);
                                string scheme = nodeReader.Value.Trim();
                                Subject_Info_HierarchicalGeographic thisSpatial = new Subject_Info_HierarchicalGeographic();
                                thisSpatial.Authority = scheme;
                                thisSpatial.Area = read_text_node(nodeReader);
                                thisPackage.Bib_Info.Add_Subject(thisSpatial);
                            }
                            else
                            {
                                Subject_Info_HierarchicalGeographic thisSpatial = new Subject_Info_HierarchicalGeographic();
                                thisSpatial.Area = read_text_node(nodeReader);
                                thisPackage.Bib_Info.Add_Subject(thisSpatial);
                            }
                            break;
                        case "DC.FORMAT.EXTENT":
                            thisPackage.Bib_Info.Original_Description.Extent = read_text_node(nodeReader);
                            break;
                        case "DC.TYPE":
                            if (thisPackage.Bib_Info.Original_Description.Extent.Length > 0)
                            {
                                thisPackage.Bib_Info.Original_Description.Extent = read_text_node(nodeReader) + " ( " + thisPackage.Bib_Info.Original_Description.Extent + " )";
                            }
                            else
                            {
                                thisPackage.Bib_Info.Original_Description.Extent = read_text_node(nodeReader);
                            }
                            break;
                        case "PALMM.LOCATION":
                            thisPackage.Bib_Info.Location.Holding_Name = read_text_node(nodeReader);
                            break;
                        case "PALMM.NOTES":
                            thisPackage.Bib_Info.Add_Note(read_text_node(nodeReader));
                            break;
                    }
                }
            }
        }

        #endregion

        #region MXF_Element class is used internally

        /// <summary>Basic MXF element </summary>
        /// <remarks>This class is used temporarily while parsing an existing MXF files by the <see cref="MXF_File_ReaderWriter"/> <br /> <br />
        /// Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
        public class MXF_Element
        {
            /// <summary> Names of any attributes </summary>
            public string[] attribute_names;

            /// <summary> Value of any attributes </summary>
            public string[] attribute_texts;

            /// <summary> Name of this MXF element </summary>
            public string name;

            /// <summary> Text of this MXF element </summary>
            public string text;

            /// <summary> Constructor for a new instance of the MXF_Element class </summary>
            public MXF_Element()
            {
                attribute_names = new string[5];
                attribute_texts = new string[5];
            }
        }

        #endregion
    }
}