#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Divisions;

#endregion


namespace SobekCM.Resource_Object.Metadata_File_ReaderWriters
{
    public class TEI_File_ReaderWriter : XML_Writing_Base_Type, iMetadata_File_ReaderWriter
    {
        #region iMetadata_File_ReaderWriter Members

        /// <summary> Flag indicates if this reader/writer can read from files and streams </summary>
        /// <value> This property always returns FALSE </value>
        public bool canRead
        {
            get { return false; }
        }

        /// <summary> Flag indicates if this reader/writer can write to files and streams </summary>
        /// <value> This property always return TRUE </value>
        public bool canWrite
        {
            get { return true; }
        }

        /// <summary> Full name which best describes the metadata format this reader/writer utilizes (i.e. Dublin Core, Greenstone file, etc.. ) </summary>
        /// <value>This property always returns 'Text Encoding Initiative (TEI)'</value>
        public string Metadata_Type_Name
        {
            get { return "Text Encoding Initiative (TEI)"; }
        }

        /// <summary> Abbreviation for the metadata format utilized by this reader/writer (i.e., DC, MODS, GSA, etc.. ) </summary>
        /// <value> This property always returns 'TEI'</value>
        public string Metadata_Type_Abbreviation
        {
            get { return "TEI"; }
        }

        /// <summary> Reads metadata from an existing metadata file and saves to the provided item/package </summary>
        /// <param name="MetadataFilePathName"> Path and name of the metadata file to read </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        /// <remarks>This reader accepts two option values.  'EAD_File_ReaderWriter:XSL_Location' gives the location of a XSL
        /// file, which can be used to transform the description XML read from this EAD into HTML (or another format of XML).  
        /// 'EAD_File_ReaderWriter:Analyze_Description' indicates whether to analyze the description section of the EAD and
        /// read it into the item. (Default is TRUE).</remarks>
        public bool Read_Metadata(string MetadataFilePathName, SobekCM_Item Return_Package, Dictionary<string, object> Options, out string Error_Message)
        {
            throw new NotImplementedException();
        }

        /// <summary> Reads metadata from an open stream and saves to the provided item/package </summary>
        /// <param name="Input_Stream"> Open stream to read metadata from </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        /// <remarks>This reader accepts two option values.  'EAD_File_ReaderWriter:XSL_Location' gives the location of a XSL
        /// file, which can be used to transform the description XML read from this EAD into HTML (or another format of XML).  
        /// 'EAD_File_ReaderWriter:Analyze_Description' indicates whether to analyze the description section of the EAD and
        /// read it into the item. (Default is TRUE).</remarks>
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
            StreamWriter results = null;
            bool returnValue;
            try
            {
                results = new StreamWriter(MetadataFilePathName, false, Encoding.UTF8);
                returnValue = Write_Metadata(results, Item_To_Save, Options, out Error_Message);

            }
            catch (Exception ee)
            {
                Error_Message = "Error writing TEI metadata to file '" + MetadataFilePathName + ": " + ee.Message;
                returnValue = false;
            }
            finally
            {
                if (results != null)
                {
                    results.Flush();
                    results.Close();
                }
            }

            return returnValue;
        }

        /// <summary> Writes the formatted metadata from the provided item to a TextWriter (usually to an output stream) </summary>
        /// <param name="Output_Stream"></param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_Metadata(TextWriter Output_Stream, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            string distributor = "University of Florida Digital Collections";
            string distributor_email = "ufdc@uflib.ufl.edu";
            string purl = "http://ufdc.ufl.edu/" + Item_To_Save.BibID + "/" + Item_To_Save.VID;
            string source_directory = Item_To_Save.Source_Directory;

            // Set default error message
            Error_Message = String.Empty;

            // Add the XML declaration
            Output_Stream.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>");

            Output_Stream.WriteLine("<TEI xmlns=\"http://www.tei-c.org/ns/1.0\">");
            Output_Stream.WriteLine("\t<teiHeader>");
            Output_Stream.WriteLine("\t\t<fileDesc>");
            Output_Stream.WriteLine("\t\t\t<titleStmt>");
            Output_Stream.WriteLine("\t\t\t\t<title>" + Convert_String_To_XML_Safe( Item_To_Save.Bib_Info.Main_Title.ToString()) + "</title>");

            if ( Item_To_Save.Bib_Info.Main_Entity_Name.hasData )
            {
                Output_Stream.WriteLine("\t\t\t\t<author>" + Convert_String_To_XML_Safe( Item_To_Save.Bib_Info.Main_Title.ToString()) + "</author>");
            }

            Output_Stream.WriteLine("\t\t\t</titleStmt>");
            Output_Stream.WriteLine("\t\t\t<publicationStmt>");

            // Add the date created
            Output_Stream.WriteLine("\t\t\t\t<date>" + DateTime.Now.Year + "</date>");

            // Add the repository as the distributor
            Output_Stream.WriteLine("\t\t\t\t<distributor>" + Convert_String_To_XML_Safe(distributor) + "</distributor>");
            Output_Stream.WriteLine("\t\t\t\t<email>" + distributor_email + "</email>");
            

            // Add all the identifiers
            Output_Stream.WriteLine("\t\t\t\t<idno>" + purl + "</idno>");

            // Add the availability statement
            if ( Item_To_Save.Bib_Info.Access_Condition.Text.Length > 0 )
            {
                Output_Stream.WriteLine("\t\t\t\t<availability status=\"restricted\">");
                Output_Stream.WriteLine("\t\t\t\t\t<p>" + Convert_String_To_XML_Safe(Item_To_Save.Bib_Info.Access_Condition.Text) + "</p>");
                Output_Stream.WriteLine("\t\t\t\t</availability>");
            }
            Output_Stream.WriteLine("\t\t\t</publicationStmt>");

            Output_Stream.WriteLine("\t\t\t<sourceDesc>");
            Output_Stream.WriteLine("\t\t\t\t<biblFull>");

            Output_Stream.WriteLine("\t\t\t\t\t<titleStmt>");
            Output_Stream.WriteLine("\t\t\t\t\t\t<title>" + Convert_String_To_XML_Safe( Item_To_Save.Bib_Info.Main_Title.ToString()) + "</title>");

            if ( Item_To_Save.Bib_Info.Main_Entity_Name.hasData )
            {
                Output_Stream.WriteLine("\t\t\t\t\t\t<author>" + Convert_String_To_XML_Safe( Item_To_Save.Bib_Info.Main_Entity_Name.ToString(false)) + "</author>");
            }
            foreach( Name_Info thisName in Item_To_Save.Bib_Info.Names )
            {
                string role = String.Empty;
                if ( thisName.Roles.Count > 0 )
                {
                    role = thisName.Roles[0].Role;
                    foreach( Name_Info_Role thisRole in thisName.Roles )
                    {
                        if ( thisRole.Role_Type == Name_Info_Role_Type_Enum.text )
                        {
                            role = thisRole.Role;
                            break;
                        }
                    }
                }
                role = role.Replace("Author, Primary", "");
                if (role.Length > 0)
                {
                    Output_Stream.WriteLine("\t\t\t\t\t\t<author role=\"" + role + "\">" + Convert_String_To_XML_Safe(thisName.ToString(false)) + "</author>");
                }
                else
                {
                    Output_Stream.WriteLine("\t\t\t\t\t\t<author>" + Convert_String_To_XML_Safe(thisName.ToString(false)) + "</author>");
                }
            }

            Output_Stream.WriteLine("\t\t\t\t\t</titleStmt>");

            if ( Item_To_Save.Bib_Info.Original_Description.Extent.Length > 0 )
            {
                Output_Stream.WriteLine("\t\t\t\t\t<extent>" + Convert_String_To_XML_Safe(Item_To_Save.Bib_Info.Original_Description.Extent)  + "</extent>");
            }

            Output_Stream.WriteLine("\t\t\t\t\t<publicationStmt>");

            // Add the publishers
            foreach( Publisher_Info pubInfo in Item_To_Save.Bib_Info.Publishers )
            {
                if ( pubInfo.Name.Length > 0 )
                    Output_Stream.WriteLine("\t\t\t\t\t\t<publisher>" + Convert_String_To_XML_Safe(pubInfo.Name) + "</publisher>");
                foreach( Origin_Info_Place place in pubInfo.Places )
                {
                    if ( place.Place_Text.Length > 0 )
                        Output_Stream.WriteLine("\t\t\t\t\t\t<pubPlace>" + Convert_String_To_XML_Safe(place.Place_Text) + "</pubPlace>");
                }
            }

            // Add the date
            if ( Item_To_Save.Bib_Info.Origin_Info.Date_Issued.Length > 0 )
            {
                Output_Stream.WriteLine("\t\t\t\t\t\t<date>" + Convert_String_To_XML_Safe(Item_To_Save.Bib_Info.Origin_Info.Date_Issued) + "</date>");
            }
            else if ( Item_To_Save.Bib_Info.Origin_Info.Date_Created.Length > 0 )
            {
                Output_Stream.WriteLine("\t\t\t\t\t\t<date>" + Convert_String_To_XML_Safe(Item_To_Save.Bib_Info.Origin_Info.Date_Created) + "</date>");
            }           

            // Add all the identifiers
            foreach( Identifier_Info thisIdentifier in Item_To_Save.Bib_Info.Identifiers )
            {
                Output_Stream.WriteLine("\t\t\t\t\t\t<idno type=\"" + thisIdentifier.Type + "\">" + thisIdentifier.Identifier  + "</idno>");
            }

            // Add the availability statement
            if ( Item_To_Save.Bib_Info.Access_Condition.Text.Length > 0 )
            {
                Output_Stream.WriteLine("\t\t\t\t\t\t<availability status=\"restricted\">");
                Output_Stream.WriteLine("\t\t\t\t\t\t\t<p>" + Convert_String_To_XML_Safe(Item_To_Save.Bib_Info.Access_Condition.Text) + "</p>");
                Output_Stream.WriteLine("\t\t\t\t\t\t</availability>");
            }
            Output_Stream.WriteLine("\t\t\t\t\t</publicationStmt>");

            Output_Stream.WriteLine("\t\t\t\t\t<notesStmt>");

            foreach( Note_Info thisNote in Item_To_Save.Bib_Info.Notes )
            {
                Output_Stream.WriteLine("\t\t\t\t\t\t<note anchored=\"true\">" + Convert_String_To_XML_Safe(thisNote.Note) + "</note>");
            }

            Output_Stream.WriteLine("\t\t\t\t\t</notesStmt>");
            Output_Stream.WriteLine("\t\t\t\t</biblFull>");
            Output_Stream.WriteLine("\t\t\t</sourceDesc>");
            Output_Stream.WriteLine("\t\t</fileDesc>");

            Output_Stream.WriteLine("\t\t<encodingDesc>");
            Output_Stream.WriteLine("\t\t\t<classDecl>");
            Output_Stream.WriteLine("\t\t\t\t<taxonomy xml:id=\"LCSH\"> <bibl>Library of Congress Subject Headings</bibl> </taxonomy>");
            Output_Stream.WriteLine("\t\t\t</classDecl>");
            Output_Stream.WriteLine("\t\t</encodingDesc>");

            Output_Stream.WriteLine("\t\t<profileDesc>");

            // Add the languages
            if ( Item_To_Save.Bib_Info.Languages_Count > 0 )
            {
                Output_Stream.WriteLine("\t\t\t<langUsage>");
                foreach( Language_Info thisLanguage in Item_To_Save.Bib_Info.Languages )
                {
                    if ( thisLanguage.Language_Text.Length > 0 )
                    {
                        Output_Stream.Write("\t\t\t\t<language");
                        if ( thisLanguage.Language_ISO_Code.Length > 0 )
                            Output_Stream.Write(" ident=\"" + thisLanguage.Language_ISO_Code + "\"");
                        else if ( thisLanguage.Language_RFC_Code.Length > 0 )
                            Output_Stream.Write(" ident=\"" + thisLanguage.Language_RFC_Code + "\"");
                        
                        Output_Stream.WriteLine(">" + Convert_String_To_XML_Safe(thisLanguage.Language_Text) + "</language>");
                    }
                }
                Output_Stream.WriteLine("\t\t\t</langUsage>");
            }

            // Add all the subject keywords
            if ( Item_To_Save.Bib_Info.Subjects_Count > 0 )
            {
                // Categorize the terms
                List<string> lcsh = new List<string>();
                List<string> non = new List<string>();
                foreach( Subject_Info thisSubj in Item_To_Save.Bib_Info.Subjects )
                {
                    if ( String.Equals( thisSubj.Authority, "lcsh", StringComparison.OrdinalIgnoreCase) )
                        lcsh.Add(thisSubj.ToString(false));
                    else
                        non.Add(thisSubj.ToString(false));
                }


                Output_Stream.WriteLine("\t\t\t<textClass>");

                // Add all the lcsh first
                if ( lcsh.Count > 0 )
                {
                    Output_Stream.WriteLine("\t\t\t\t<keywords scheme=\"#LCSH\">");
                    Output_Stream.WriteLine("\t\t\t\t\t<list>");
                    foreach( string thisLcsh in lcsh )
                    {
                        Output_Stream.WriteLine("\t\t\t\t\t\t<item>" + Convert_String_To_XML_Safe(thisLcsh) + "</item>");
                    }
                    Output_Stream.WriteLine("\t\t\t\t\t</list>");
                    Output_Stream.WriteLine("\t\t\t\t</keywords>");
                }

                // Add all the non-lcsh next
                if ( non.Count > 0 )
                {
                    Output_Stream.WriteLine("\t\t\t\t<keywords>");
                    Output_Stream.WriteLine("\t\t\t\t\t<list>");
                    foreach( string thisNon in non )
                    {
                        Output_Stream.WriteLine("\t\t\t\t\t\t<item>" + Convert_String_To_XML_Safe(thisNon) + "</item>");
                    }
                    Output_Stream.WriteLine("\t\t\t\t\t</list>");
                    Output_Stream.WriteLine("\t\t\t\t</keywords>");
                }


                Output_Stream.WriteLine("\t\t\t</textClass>");
            }

            Output_Stream.WriteLine("\t\t</profileDesc>");
            Output_Stream.WriteLine("\t\t<revisionDesc>");
            Output_Stream.WriteLine("\t\t\t<change when=\"" + DateTime.Now.Year + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Day.ToString().PadLeft(2,'0') + "\">TEI auto-generated from digital resource</change>");
            Output_Stream.WriteLine("\t\t</revisionDesc>");
            Output_Stream.WriteLine("\t</teiHeader>");

            if (source_directory.Length > 0)
            {
                Output_Stream.WriteLine("<text>");
                Output_Stream.WriteLine("<body>");
                int page_count = 1;
                foreach (abstract_TreeNode rootNode in Item_To_Save.Divisions.Physical_Tree.Roots)
                {
                    recursively_add_div_page_text(source_directory, rootNode, Output_Stream, ref page_count);
                }

                Output_Stream.WriteLine("</body>");
                Output_Stream.WriteLine("</text>");
            }

            Output_Stream.WriteLine("</TEI>");

            return true;

        }

        private void recursively_add_div_page_text( string source_directory, abstract_TreeNode thisNode, TextWriter Output_Stream, ref int page_count )
        {
            if (thisNode.Page)
            {
                Page_TreeNode pageNode = (Page_TreeNode) thisNode;

                if (pageNode.Files.Count > 0)
                {
                    string pageimage = String.Empty;
                    string textfilename = pageNode.Files[0].File_Name_Sans_Extension + ".txt";
                    foreach (SobekCM_File_Info thisFile in pageNode.Files)
                    {
                        if (thisFile.File_Extension.ToLower() == "txt")
                            textfilename = thisFile.System_Name;
                        if ((thisFile.File_Extension.ToLower() == "jpg") && (thisFile.System_Name.ToLower().IndexOf("thm.jpg") < 0))
                            pageimage = thisFile.System_Name;
                    }
                    
                    // Add the page break first
                    Output_Stream.Write("<pb n=\"" + page_count + "\"");
                    if ( pageimage.Length > 0 )
                        Output_Stream.Write(" facs=\"" + Convert_String_To_XML_Safe(pageimage) + "\"");
                    Output_Stream.WriteLine(" />");

                    // Does the text file exist?
                    string text_file = source_directory + "\\" + textfilename;
                    try
                    {
                        if (System.IO.File.Exists(text_file))
                        {
                            Output_Stream.WriteLine(Convert_String_To_XML_Safe(System.IO.File.ReadAllText(text_file)));
                        }
                    }
                    catch { }

                }
                page_count++;
            }
            else
            {
                Division_TreeNode divNode = (Division_TreeNode) thisNode;
                if (thisNode.Type != "main")
                {
                    Output_Stream.WriteLine("<div type=\"" + thisNode.Type + "\">");
                    if ((thisNode.Label.Length > 0) && (thisNode.Label != thisNode.Type))
                    {
                        Output_Stream.WriteLine("<head>" + Convert_String_To_XML_Safe(thisNode.Label) + "</head>");
                    }
                }

                // Now, step through child nodes
                foreach (abstract_TreeNode childNode in divNode.Nodes)
                {
                    recursively_add_div_page_text(source_directory, childNode, Output_Stream, ref page_count);
                }

                if (thisNode.Type != "main")
                {
                    Output_Stream.WriteLine("</div>");

                }
            }


        }

        #endregion
    }
}
