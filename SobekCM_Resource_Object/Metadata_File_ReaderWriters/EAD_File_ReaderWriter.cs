#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.EAD;

#endregion

namespace SobekCM.Resource_Object.Metadata_File_ReaderWriters
{
    /// <summary>  Reads the container list and grabs the entire description section from an EAD
    /// file into an existing item for display within SobekCM </summary>
    public class EAD_File_ReaderWriter : iMetadata_File_ReaderWriter
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
        /// <value>This property always returns 'Encoded Archival Descriptor'</value>
        public string Metadata_Type_Name
        {
            get { return "Encoded Archival Descriptor"; }
        }

        /// <summary> Abbreviation for the metadata format utilized by this reader/writer (i.e., DC, MODS, GSA, etc.. ) </summary>
        /// <value> This property always returns 'EAD'</value>
        public string Metadata_Type_Abbreviation
        {
            get { return "EAD"; }
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
            Stream reader = new FileStream(MetadataFilePathName, FileMode.Open, FileAccess.Read);
            bool returnValue = Read_Metadata(reader, Return_Package, Options, out Error_Message);
            reader.Close();

            FileInfo eadFileInfo = new FileInfo(MetadataFilePathName);
            Return_Package.Source_Directory = eadFileInfo.DirectoryName;
            if (Return_Package.BibID.Length == 0)
                Return_Package.BibID = eadFileInfo.Name.Replace(".xml", "");
            Return_Package.VID = "00001";
            Return_Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Archival;
            Return_Package.Bib_Info.Type.Collection = true;

            return returnValue;
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
            // Ensure this metadata module extension exists
            EAD_Info eadInfo = Return_Package.Get_Metadata_Module(GlobalVar.EAD_METADATA_MODULE_KEY) as EAD_Info;
            if (eadInfo == null)
            {
                eadInfo = new EAD_Info();
                Return_Package.Add_Metadata_Module(GlobalVar.EAD_METADATA_MODULE_KEY, eadInfo);
            }

            // Set a couple defaults first
            Return_Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Archival;
            Return_Package.Bib_Info.Type.Collection = true;
            Error_Message = String.Empty;

            // Check for some options
            string XSL_Location = String.Empty;
            bool Analyze_Description = true;
            if (Options != null)
            {
                if (Options.ContainsKey("EAD_File_ReaderWriter:XSL_Location"))
                {
                    XSL_Location = Options["EAD_File_ReaderWriter:XSL_Location"].ToString();
                }
                if (Options.ContainsKey("EAD_File_ReaderWriter:Analyze_Description"))
                {
                    bool.TryParse(Options["EAD_File_ReaderWriter:Analyze_Description"].ToString(), out Analyze_Description);
                }
            }


            // Use a string builder to seperate the description and container sections
            StringBuilder description_builder = new StringBuilder(20000);
            StringBuilder container_builder = new StringBuilder(20000);

            // Read through with a simple stream reader first
            StreamReader reader = new StreamReader(Input_Stream);
            string line = reader.ReadLine();
            bool in_container_list = false;

            // Step through each line
            while (line != null)
            {
                if (!in_container_list)
                {
                    // Do not import the XML stylesheet portion
                    if ((line.IndexOf("xml-stylesheet") < 0) && (line.IndexOf("<!DOCTYPE ") < 0))
                    {
                        // Does the start a DSC section?
                        if ((line.IndexOf("<dsc ") >= 0) || (line.IndexOf("<dsc>") > 0))
                        {
                            in_container_list = true;
                            container_builder.AppendLine(line);
                        }
                        else
                        {
                            description_builder.AppendLine(line);
                        }
                    }
                }
                else
                {
                    // Add this to the container builder
                    container_builder.AppendLine(line);

                    // Does this end the container list section?
                    if (line.IndexOf("</dsc>") >= 0)
                        in_container_list = false;
                }

                // Get the next line
                line = reader.ReadLine();
            }

            // Close the reader
            reader.Close();

            // Just assign all the description section first
            eadInfo.Full_Description = description_builder.ToString();

            // Should the decrpition additionally be analyzed?
            if (Analyze_Description)
            {
                // Try to read the XML
                try
                {
                    XmlTextReader reader2 = new XmlTextReader(description_builder.ToString());

                    // Initial doctype declaration sometimes throws an error for a missing EAD.dtd.
                    bool ead_start_found = false;
                    int error = 0;
                    while ((!ead_start_found) && (error < 5))
                    {
                        try
                        {
                            reader2.Read();
                            if ((reader2.NodeType == XmlNodeType.Element) && (reader2.Name.ToLower() == GlobalVar.EAD_METADATA_MODULE_KEY))
                            {
                                ead_start_found = true;
                            }
                        }
                        catch
                        {
                            error++;
                        }
                    }

                    // Now read the body of the EAD
                    while (reader2.Read())
                    {
                        if (reader2.NodeType == XmlNodeType.Element)
                        {
                            string nodeName = reader2.Name.ToLower();
                            switch (nodeName)
                            {
                                case "descrules":
                                    string descrules_text = reader2.ReadInnerXml().ToUpper();
                                    if (descrules_text.IndexOf("FINDING AID PREPARED USING ") == 0)
                                    {
                                        Return_Package.Bib_Info.Record.Description_Standard = descrules_text.Replace("FINDING AID PREPARED USING ", "");
                                    }
                                    else
                                    {
                                        string[] likely_description_standards = {"DACS", "APPM", "AACR2", "RDA", "ISADG", "ISAD", "MAD", "RAD"};
                                        foreach (string likely_standard in likely_description_standards)
                                        {
                                            if (descrules_text.IndexOf(likely_standard) >= 0)
                                            {
                                                Return_Package.Bib_Info.Record.Description_Standard = likely_standard;
                                                break;
                                            }
                                        }
                                    }
                                    break;

                                case "unittitle":
                                    while (reader2.Read())
                                        if (reader2.NodeType == XmlNodeType.Text)
                                        {
                                            Return_Package.Bib_Info.Main_Title.Title = reader2.Value;
                                            break;
                                        }
                                        else if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name.Equals("unittitle"))
                                            break;
                                    while (!(reader2.NodeType == XmlNodeType.EndElement && reader2.Name.Equals("unittitle")))
                                        reader2.Read();
                                    break;

                                case "unitid":
                                    while (reader2.Read())
                                    {
                                        if (reader2.NodeType == XmlNodeType.Text)
                                        {
                                            Return_Package.Bib_Info.Add_Identifier(reader2.Value, "Main Identifier");
                                        }
                                        if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name.ToLower().Equals(nodeName))
                                            break;
                                    }
                                    break;

                                case "origination":
                                    while (reader2.Read())
                                    {
                                        if (reader2.NodeType == XmlNodeType.Element && reader2.Name.Equals("persname"))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.NodeType == XmlNodeType.Text)
                                                {
                                                    Return_Package.Bib_Info.Main_Entity_Name.Full_Name = Trim_Final_Punctuation(reader2.Value);
                                                    Return_Package.Bib_Info.Main_Entity_Name.Name_Type = Name_Info_Type_Enum.personal;
                                                }
                                                else if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name.ToLower().Equals(nodeName))
                                                    break;
                                            }
                                        }
                                        if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name.ToLower().Equals(nodeName))
                                            break;
                                    }
                                    break;

                                case "physdesc":
                                    while (reader2.Read())
                                    {
                                        if (reader2.NodeType == XmlNodeType.Element && reader2.Name.Equals("extent"))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.NodeType == XmlNodeType.Text)
                                                    Return_Package.Bib_Info.Original_Description.Extent = reader2.Value;
                                                else if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name.Equals("extent"))
                                                    break;
                                            }
                                        }
                                        if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name.Equals("physdesc"))
                                            break;
                                    }
                                    break;

                                case "abstract":
                                    while (reader2.Read())
                                    {
                                        if (reader2.NodeType == XmlNodeType.Text)
                                            Return_Package.Bib_Info.Add_Abstract(reader2.Value);
                                        else if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name.ToLower().Equals(nodeName))
                                            break;
                                    }
                                    break;

                                case "repository":
                                    while (reader2.Read())
                                    {
                                        if (reader2.NodeType == XmlNodeType.Element && reader2.Name.Equals("corpname"))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.NodeType == XmlNodeType.Text)
                                                    Return_Package.Bib_Info.Location.Holding_Name = reader2.Value;
                                                else if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name.ToLower().Equals(nodeName))
                                                    break;
                                            }
                                        }
                                        if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name.ToLower().Equals(nodeName))
                                            break;
                                    }
                                    break;

                                case "accessrestrict":
                                    Return_Package.Bib_Info.Add_Note(Clean_Text_Block(reader2.ReadInnerXml()), Note_Type_Enum.restriction);
                                    break;

                                case "userestrict":
                                    Return_Package.Bib_Info.Access_Condition.Text = Clean_Text_Block(reader2.ReadInnerXml());
                                    break;

                                case "acqinfo":
                                    Return_Package.Bib_Info.Add_Note(Clean_Text_Block(reader2.ReadInnerXml()), Note_Type_Enum.acquisition);
                                    break;

                                case "bioghist":
                                    Return_Package.Bib_Info.Add_Note(Clean_Text_Block(reader2.ReadInnerXml()), Note_Type_Enum.biographical);
                                    break;

                                case "scopecontent":
                                    Return_Package.Bib_Info.Add_Abstract(Clean_Text_Block(reader2.ReadInnerXml()), "", "summary", "Summary");
                                    break;

                                case "controlaccess":
                                    while (reader2.Read())
                                    {
                                        if (reader2.NodeType == XmlNodeType.Element && (reader2.Name.Equals("corpname") || reader2.Name.Equals("persname")))
                                        {
                                            string tagnamei = reader2.Name;
                                            string source = "";
                                            if (reader2.MoveToAttribute("source"))
                                                source = reader2.Value;
                                            while (reader2.Read())
                                            {
                                                if (reader2.NodeType == XmlNodeType.Text)
                                                {
                                                    Subject_Info_Name newName = new Subject_Info_Name();
                                                    //ToSee: where to add name? and ehat types
                                                    //ToDo: Type
                                                    newName.Full_Name = Trim_Final_Punctuation(reader2.Value);
                                                    newName.Authority = source;
                                                    Return_Package.Bib_Info.Add_Subject(newName);
                                                    if (tagnamei.StartsWith("corp"))
                                                        newName.Name_Type = Name_Info_Type_Enum.corporate;
                                                    else if (tagnamei.StartsWith("pers"))
                                                        newName.Name_Type = Name_Info_Type_Enum.personal;
                                                    else
                                                        newName.Name_Type = Name_Info_Type_Enum.UNKNOWN;
                                                }
                                                else if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name.Equals(tagnamei))
                                                    break;
                                            }
                                        }
                                        else if (reader2.NodeType == XmlNodeType.Element && (reader2.Name.Equals("subject")))
                                        {
                                            string tagnamei = reader2.Name;
                                            string source = "";
                                            if (reader2.MoveToAttribute("source"))
                                                source = reader2.Value;
                                            while (reader2.Read())
                                            {
                                                if (reader2.NodeType == XmlNodeType.Text)
                                                {
                                                    string subjectTerm = Trim_Final_Punctuation(reader2.Value.Trim());
                                                    if (subjectTerm.Length > 1)
                                                    {
                                                        Subject_Info_Standard subject = Return_Package.Bib_Info.Add_Subject();
                                                        subject.Authority = source;
                                                        if (subjectTerm.IndexOf("--") == 0)
                                                            subject.Add_Topic(subjectTerm);
                                                        else
                                                        {
                                                            while (subjectTerm.IndexOf("--") > 0)
                                                            {
                                                                string fragment = subjectTerm.Substring(0, subjectTerm.IndexOf("--")).Trim();
                                                                if (fragment.ToLower() == "florida")
                                                                    subject.Add_Geographic(fragment);
                                                                else
                                                                    subject.Add_Topic(fragment);

                                                                if (subjectTerm.Length > subjectTerm.IndexOf("--") + 3)
                                                                    subjectTerm = subjectTerm.Substring(subjectTerm.IndexOf("--") + 2);
                                                                else
                                                                    subjectTerm = String.Empty;
                                                            }
                                                            if (subjectTerm.Trim().Length > 0)
                                                            {
                                                                string fragment = subjectTerm.Trim();
                                                                if (fragment.ToLower() == "florida")
                                                                    subject.Add_Geographic(fragment);
                                                                else
                                                                    subject.Add_Topic(fragment);
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name.Equals(tagnamei))
                                                    break;
                                            }
                                        }
                                        if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name.ToLower().Equals(nodeName))
                                            break;
                                    }
                                    break;

                                case "dsc":
                                    eadInfo.Container_Hierarchy.Read(reader2);
                                    break;
                            }
                        }


                        if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name.Equals(GlobalVar.EAD_METADATA_MODULE_KEY))
                            break;
                    }

                    reader2.Close();
                }
                catch (Exception ee)
                {
                    Error_Message = "Error caught in EAD_reader2_Writer: " + ee.Message;
                    return false;
                }
            }


            // If there is a XSL, apply it to the description stored in the EAD sub-section of the item id
            if (XSL_Location.Length > 0)
            {
                try
                {
                    // Create the transform and load the XSL indicated
                    XslCompiledTransform transform = new XslCompiledTransform();
                    transform.Load(XSL_Location);

                    // Apply the transform to convert the XML into HTML
                    StringWriter results = new StringWriter();
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ProhibitDtd = false;
                    using (XmlReader transformreader = XmlReader.Create(new StringReader(eadInfo.Full_Description), settings))
                    {
                        transform.Transform(transformreader, null, results);
                    }
                    eadInfo.Full_Description = results.ToString();

                    // Get rid of the <?xml header
                    if (eadInfo.Full_Description.IndexOf("<?xml") >= 0)
                    {
                        int xml_end_index = eadInfo.Full_Description.IndexOf("?>");
                        eadInfo.Full_Description = eadInfo.Full_Description.Substring(xml_end_index + 2);
                    }

                    // Since this was successful, try to build the TOC list of included sections
                    SortedList<int, string> toc_sorter = new SortedList<int, string>();
                    string description = eadInfo.Full_Description;
                    int did = description.IndexOf("<a name=\"did\"");
                    int bioghist = description.IndexOf("<a name=\"bioghist\"");
                    int scopecontent = description.IndexOf("<a name=\"scopecontent\"");
                    int organization = description.IndexOf("<a name=\"organization\"");
                    int arrangement = description.IndexOf("<a name=\"arrangement\"");
                    int relatedmaterial = description.IndexOf("<a name=\"relatedmaterial\"");
                    int otherfindaid = description.IndexOf("<a name=\"otherfindaid\"");
                    int index = description.IndexOf("<a name=\"index\">");
                    int bibliography = description.IndexOf("<a name=\"bibliography\"");
                    int odd = description.IndexOf("<a name=\"odd\"");
                    int controlaccess = description.IndexOf("<a name=\"controlaccess\"");
                    int accruals = description.IndexOf("<a name=\"accruals\"");
                    int appraisal = description.IndexOf("<a name=\"appraisal\"");
                    int processinfo = description.IndexOf("<a name=\"processinfo\"");
                    int acqinfo = description.IndexOf("<a name=\"acqinfo\"");
                    int prefercite = description.IndexOf("<a name=\"prefercite\"");
                    int altformavail = description.IndexOf("<a name=\"altformavail\"");
                    int custodhist = description.IndexOf("<a name=\"custodhist\"");
                    int accessrestrict = description.IndexOf("<a name=\"accessrestrict\"");
                    int admininfo = description.IndexOf("<a name=\"admininfo\"");

                    if (did >= 0) toc_sorter[did] = "did";
                    if (bioghist >= 0) toc_sorter[bioghist] = "bioghist";
                    if (scopecontent >= 0) toc_sorter[scopecontent] = "scopecontent";
                    if (organization >= 0) toc_sorter[organization] = "organization";
                    if (arrangement >= 0) toc_sorter[arrangement] = "arrangement";
                    if (relatedmaterial >= 0) toc_sorter[relatedmaterial] = "relatedmaterial";
                    if (otherfindaid >= 0) toc_sorter[otherfindaid] = "otherfindaid";
                    if (index >= 0) toc_sorter[index] = "index";
                    if (bibliography >= 0) toc_sorter[bibliography] = "bibliography";
                    if (odd >= 0) toc_sorter[odd] = "odd";
                    if (controlaccess >= 0) toc_sorter[controlaccess] = "controlaccess";
                    if (accruals >= 0) toc_sorter[accruals] = "accruals";
                    if (appraisal >= 0) toc_sorter[appraisal] = "appraisal";
                    if (processinfo >= 0) toc_sorter[processinfo] = "processinfo";
                    if (acqinfo >= 0) toc_sorter[acqinfo] = "acqinfo";
                    if (prefercite >= 0) toc_sorter[prefercite] = "prefercite";
                    if (altformavail >= 0) toc_sorter[altformavail] = "altformavail";
                    if (custodhist >= 0) toc_sorter[custodhist] = "custodhist";
                    if (accessrestrict >= 0) toc_sorter[accessrestrict] = "accessrestrict";
                    if (admininfo >= 0) toc_sorter[admininfo] = "admininfo";

                    // Now, add each section back to the TOC list
                    foreach (string thisEadSection in toc_sorter.Values)
                    {
                        // Index needs to have its head looked up, everything else adds simply
                        if (thisEadSection != "index")
                        {
                            switch (thisEadSection)
                            {
                                case "did":
                                    eadInfo.Add_TOC_Included_Section("did", "Descriptive Summary");
                                    break;

                                case "bioghist":
                                    eadInfo.Add_TOC_Included_Section("bioghist", "Biographical / Historical Note");
                                    break;

                                case "scopecontent":
                                    eadInfo.Add_TOC_Included_Section("scopecontent", "Scope and Content");
                                    break;

                                case "accessrestrict":
                                    eadInfo.Add_TOC_Included_Section("accessrestrict", "Access or Use Restrictions");
                                    break;

                                case "relatedmaterial":
                                    eadInfo.Add_TOC_Included_Section("relatedmaterial", "Related or Separated Material");
                                    break;

                                case "admininfo":
                                    eadInfo.Add_TOC_Included_Section("admininfo", "Administrative Information");
                                    break;

                                case "altformavail":
                                    eadInfo.Add_TOC_Included_Section("altformavail", " &nbsp; &nbsp; Alternate Format Available");
                                    break;

                                case "prefercite":
                                    eadInfo.Add_TOC_Included_Section("prefercite", " &nbsp; &nbsp; Preferred Citation");
                                    break;

                                case "acqinfo":
                                    eadInfo.Add_TOC_Included_Section("acqinfo", " &nbsp; &nbsp; Acquisition Information");
                                    break;

                                case "processinfo":
                                    eadInfo.Add_TOC_Included_Section("processinfo", " &nbsp; &nbsp; Processing Information");
                                    break;

                                case "custodhist":
                                    eadInfo.Add_TOC_Included_Section("custodhist", " &nbsp; &nbsp; Custodial Work_History");
                                    break;

                                case "controlaccess":
                                    eadInfo.Add_TOC_Included_Section("controlaccess", "Selected Subjects");
                                    break;

                                case "otherfindaid":
                                    eadInfo.Add_TOC_Included_Section("otherfindaid", "Alternate Form of Finding Aid");
                                    break;
                            }
                        }
                        else
                        {
                            int end_link = eadInfo.Full_Description.IndexOf("</a>", index);
                            string index_title = eadInfo.Full_Description.Substring(index + 16, end_link - index - 16);
                            if (index_title.Length > 38)
                                index_title = index_title.Substring(0, 32) + "...";
                            eadInfo.Add_TOC_Included_Section("index", index_title);
                        }
                    }
                }
                catch (Exception ee)
                {
                    bool error = false;
                }
            }

            // Now, parse the container section as XML
            if (container_builder.Length > 0)
            {
                StringReader containerReader = new StringReader(container_builder.ToString());
                XmlTextReader xml_reader = new XmlTextReader(containerReader);
                xml_reader.Read();
                eadInfo.Container_Hierarchy.Read(xml_reader);
            }

            return true;
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

        private string Trim_Final_Punctuation(string ToBeTrimmed)
        {
            if (ToBeTrimmed.Length == 0)
                return ToBeTrimmed;

            if (ToBeTrimmed[ToBeTrimmed.Length - 1] == '.')
            {
                return ToBeTrimmed.Substring(0, ToBeTrimmed.Length - 1);
            }

            return ToBeTrimmed;
        }

        private string Clean_Text_Block(string InnerXML)
        {
            Regex rgx1 = new Regex("xmlns=\"[^\"]*\"");
            Regex rgx2 = new Regex("[ ]*>");
            Regex rgx3 = new Regex("[ ]*/>");

            string newInnerXML = rgx3.Replace(rgx2.Replace(rgx1.Replace(InnerXML, ""), ">"), "/>").Trim();
            if (newInnerXML.IndexOf("</head>") > 0)
                newInnerXML = newInnerXML.Substring(newInnerXML.IndexOf("</head>") + 7);

            while (newInnerXML.IndexOf("<chronlist>") > 0)
            {
                int start_index = newInnerXML.IndexOf("<chronlist>");
                int end_index = newInnerXML.IndexOf("</chronlist>");
                string newValue = String.Empty;
                if (start_index > 0)
                    newValue = newValue + newInnerXML.Substring(0, start_index);
                if (end_index < newInnerXML.Length - 12)
                    newValue = newValue + " " + newInnerXML.Substring(end_index + 12);
                newInnerXML = newValue;
            }

            while (newInnerXML.IndexOf("<arrangement>") > 0)
            {
                int start_index = newInnerXML.IndexOf("<arrangement>");
                int end_index = newInnerXML.IndexOf("</arrangement>");
                string newValue = String.Empty;
                if (start_index > 0)
                    newValue = newValue + newInnerXML.Substring(0, start_index);
                if (end_index < newInnerXML.Length - 14)
                    newValue = newValue + " " + newInnerXML.Substring(end_index + 14);
                newInnerXML = newValue;
            }

            return newInnerXML.Replace("<p>", "").Replace("</p>", "").Replace("\n", "").Replace("\r", "").Replace("<emph render=\"italic\">", "").Replace("</emph>", "").Replace("<emph>", "");
        }
    }
}