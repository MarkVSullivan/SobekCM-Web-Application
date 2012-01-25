using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Text.RegularExpressions;
using SobekCM.Bib_Package.Bib_Info;

namespace SobekCM.Bib_Package.Readers
{
    /// <summary> Class is used to read EAD / finding guides files and EAD-formatted streams into
    /// the <see cref="SobekCM_Item"/> objects </summary>
    public class EAD_Reader
    {
        /// <summary> Constructor for a new instance of the EAD_Reader class </summary>
        public EAD_Reader()
        {
            // Do nothing 
        }

        /// <summary> Reads the container list and grabs the entire description section from an AED
        /// file into an existing item for display within SobekCM </summary>
        /// <param name="Item"> Digital resource object to enhance with EAD information </param>
        /// <param name="EAD_File"> Name and path of the EAD file to read </param>
        /// <param name="XSL_Location"> Location of the XSL file to load </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This does not parse any of the bibliographic section of the EAD into the item, just 
        /// imports all the relevant data into the <see cref="SobekCM_Item.EAD"/> property. </remarks>
        public static bool Add_EAD_Information(SobekCM.Bib_Package.SobekCM_Item Item, string EAD_File, string XSL_Location )
        {
            Stream reader = new FileStream(EAD_File, FileMode.Open, FileAccess.Read);
            bool returnValue = Add_EAD_Information(Item, reader, XSL_Location);
            reader.Close();
            return returnValue;
        }

        /// <summary> Reads the container list and grabs the entire description section from an AED
        /// file stream into an existing item for display within SobekCM </summary>
        /// <param name="Item"> Digital resource object to enhance with EAD information </param>
        /// <param name="EAD_Stream"></param>
        /// <param name="XSL_Location"> Location of the XSL file to load </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This does not parse any of the bibliographic section of the EAD into the item, just 
        /// imports all the relevant data into the <see cref="SobekCM_Item.EAD"/> property. </remarks>
        public static bool Add_EAD_Information(SobekCM.Bib_Package.SobekCM_Item Item, Stream EAD_Stream, string XSL_Location)
        {
            // Use a string builder to seperate the description and container sections
            StringBuilder description_builder = new StringBuilder(20000);
            StringBuilder container_builder = new StringBuilder(20000);

            // Read through with a simple stream reader first
            StreamReader reader = new StreamReader(EAD_Stream);
            string line = reader.ReadLine();
            bool in_container_list = false;

            // Step through each line
            while (line != null)
            {
                if (!in_container_list)
                {
                    // Do not import the XML stylesheet portion
                    if ((line.IndexOf("xml-stylesheet") < 0) && ( line.IndexOf("<!DOCTYPE ") < 0 ))
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
                    if (line.IndexOf("</dsc>") >= 0 )
                        in_container_list = false;
                }

                // Get the next line
                line = reader.ReadLine();
            }

            // Close the reader
            reader.Close();

            // Just assign all the description section
            Item.EAD.Description = description_builder.ToString();

            // If there is a XSL, try to apply it
            if (XSL_Location.Length > 0)
            {
                try
                {
                    // Create the transform and load the XSL indicated
                    XslCompiledTransform transform = new XslCompiledTransform();
                    transform.Load(XSL_Location);

                    // Apply the transform to convert the XML into HTML
                    System.IO.StringWriter results = new System.IO.StringWriter();
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ProhibitDtd = false;
                    using (XmlReader transformreader = XmlReader.Create(new System.IO.StringReader(Item.EAD.Description), settings))
                    {
                        transform.Transform(transformreader, null, results);
                    }
                    Item.EAD.Description = results.ToString();

                    // Get rid of the <?xml header
                    if (Item.EAD.Description.IndexOf("<?xml") >= 0)
                    {
                        int xml_end_index = Item.EAD.Description.IndexOf("?>");
                        Item.EAD.Description = Item.EAD.Description.Substring(xml_end_index + 2);
                    }

                    // Since this was successful, try to build the TOC list of included sections
                    SortedList<int, string> toc_sorter = new SortedList<int, string>();
                    string description = Item.EAD.Description;
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
                                    Item.EAD.Add_TOC_Included_Section("did", "Descriptive Summary");
                                    break;

                                case "bioghist":
                                    Item.EAD.Add_TOC_Included_Section("bioghist", "Biographical / Historical Note");
                                    break;

                                case "scopecontent":
                                    Item.EAD.Add_TOC_Included_Section("scopecontent", "Scope and Content");
                                    break;

                                case "accessrestrict":
                                    Item.EAD.Add_TOC_Included_Section("accessrestrict", "Access or Use Restrictions");
                                    break;

                                case "relatedmaterial":
                                    Item.EAD.Add_TOC_Included_Section("relatedmaterial", "Related or Separated Material");
                                    break;

                                case "admininfo":
                                    Item.EAD.Add_TOC_Included_Section("admininfo", "Administrative Information");
                                    break;

                                case "altformavail":
                                    Item.EAD.Add_TOC_Included_Section("altformavail", " &nbsp; &nbsp; Alternate Format Available");
                                    break;

                                case "prefercite":
                                    Item.EAD.Add_TOC_Included_Section("prefercite", " &nbsp; &nbsp; Preferred Citation");
                                    break;

                                case "acqinfo":
                                    Item.EAD.Add_TOC_Included_Section("acqinfo", " &nbsp; &nbsp; Acquisition Information");
                                    break;

                                case "processinfo":
                                    Item.EAD.Add_TOC_Included_Section("processinfo", " &nbsp; &nbsp; Processing Information");
                                    break;

                                case "custodhist":
                                    Item.EAD.Add_TOC_Included_Section("custodhist", " &nbsp; &nbsp; Custodial History");
                                    break;

                                case "controlaccess":
                                    Item.EAD.Add_TOC_Included_Section("controlaccess", "Selected Subjects");
                                    break;

                                case "otherfindaid":
                                    Item.EAD.Add_TOC_Included_Section("otherfindaid", "Alternate Form of Finding Aid");
                                    break;
                            }
                        }
                        else
                        {
                            int end_link = Item.EAD.Description.IndexOf("</a>", index);
                            string index_title = Item.EAD.Description.Substring(index + 16, end_link - index - 16);
                            if (index_title.Length > 38)
                                index_title = index_title.Substring(0, 32) + "...";
                            Item.EAD.Add_TOC_Included_Section("index", index_title);
                        }
                    }
                }
                catch 
                {
                     
                }
            }
            
            // Now, parse the container section as XML
            if (container_builder.Length > 0)
            {
                StringReader containerReader = new StringReader(container_builder.ToString());
                XmlTextReader xml_reader = new XmlTextReader(containerReader);
                xml_reader.Read();
                Item.EAD.Container_Hierarchy.Read(xml_reader);
            }

            return true;
        }

        /// <summary> Reader reads an EAD file and loads the bibliographic information and container list 
        /// into the a digital resource object for use within the SobekCM library </summary>
        /// <param name="EAD_File"> Name and path of the EAD file to read </param>
        /// <returns> Digital resource object built from the EAD file </returns>
        public static SobekCM.Bib_Package.SobekCM_Item Read(string EAD_File)
        {
            Stream reader = new FileStream(EAD_File, FileMode.Open, FileAccess.Read);
            SobekCM.Bib_Package.SobekCM_Item returnValue = Read(reader);

            FileInfo eadFileInfo = new FileInfo(EAD_File);
            returnValue.Source_Directory = eadFileInfo.DirectoryName;
            if (returnValue.BibID.Length == 0)
                returnValue.BibID = eadFileInfo.Name.Replace(".xml", "");
            returnValue.VID = "00001";
            returnValue.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Archival;
            returnValue.Bib_Info.Type.Collection = true;
            reader.Close();
            return returnValue;
        }


        /// <summary> Imports the bibliographic and container list information from an EAD/finding guide 
        /// from an EAD file stream into a new <see cref="SobekCM_Item"/> object. </summary>
        /// <param name="EAD_Stream"> EAD XML Text Reader </param>
        public static SobekCM.Bib_Package.SobekCM_Item Read(Stream EAD_Stream)
        {
            SobekCM.Bib_Package.SobekCM_Item returnValue = new SobekCM.Bib_Package.SobekCM_Item();


            // Try to read the XML
            try
            {
                XmlTextReader reader = new XmlTextReader(EAD_Stream);

                // Initial doctype declaration sometimes throws an error for a missing EAD.dtd.
                bool ead_start_found = false;
                int error = 0;
                while ((!ead_start_found) && ( error < 5 ))
                {
                    try
                    {
                        reader.Read();
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name.ToLower() == "ead"))
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
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        string nodeName = reader.Name.ToLower();
                        switch (nodeName)
                        {
                            case "descrules":
                                string descrules_text = reader.ReadInnerXml().ToUpper();
                                if (descrules_text.IndexOf("FINDING AID PREPARED USING ") == 0)
                                {
                                    returnValue.Bib_Info.Record.Description_Standard = descrules_text.Replace("FINDING AID PREPARED USING ", "");
                                }
                                else
                                {
                                    string[] likely_description_standards = { "DACS", "APPM", "AACR2", "RDA", "ISADG", "ISAD", "MAD", "RAD" };
                                    foreach (string likely_standard in likely_description_standards)
                                    {
                                        if (descrules_text.IndexOf(likely_standard) >= 0)
                                        {
                                            returnValue.Bib_Info.Record.Description_Standard = likely_standard;
                                            break;
                                        }
                                    }
                                }
                                break;

                            case "unittitle":
                                while (reader.Read())
                                    if (reader.NodeType == XmlNodeType.Text)
                                    {
                                        returnValue.Bib_Info.Main_Title.Title = reader.Value;
                                        break;
                                    }
                                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("unittitle"))
                                        break;
                                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("unittitle")))
                                    reader.Read();
                                break;

                            case "unitid":
                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Text)
                                    {
                                        returnValue.Bib_Info.Add_Identifier(reader.Value, "Main Identifier");
                                    }
                                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name.ToLower().Equals(nodeName))
                                        break;
                                }
                                break;

                            case "origination":
                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("persname"))
                                    {
                                        while (reader.Read())
                                        {
                                            if (reader.NodeType == XmlNodeType.Text)
                                            {
                                                returnValue.Bib_Info.Main_Entity_Name.Full_Name = Trim_Final_Punctuation(reader.Value);
                                                returnValue.Bib_Info.Main_Entity_Name.Name_Type = Name_Info_Type_Enum.personal;
                                            }
                                            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.ToLower().Equals(nodeName))
                                                break;
                                        }
                                    }
                                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name.ToLower().Equals(nodeName))
                                        break;
                                }
                                break;

                            case "physdesc":
                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("extent"))
                                    {
                                        while (reader.Read())
                                        {
                                            if (reader.NodeType == XmlNodeType.Text)
                                                returnValue.Bib_Info.Original_Description.Extent = reader.Value;
                                            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("extent"))
                                                break;
                                        }
                                    }
                                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("physdesc"))
                                        break;
                                }
                                break;

                            case "abstract":
                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Text)
                                        returnValue.Bib_Info.Add_Abstract(reader.Value);
                                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.ToLower().Equals(nodeName))
                                        break;
                                }
                                break;

                            case "repository":
                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("corpname"))
                                    {
                                        while (reader.Read())
                                        {
                                            if (reader.NodeType == XmlNodeType.Text)
                                                returnValue.Bib_Info.Location.Holding_Name = reader.Value;
                                            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.ToLower().Equals(nodeName))
                                                break;
                                        }
                                    }
                                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name.ToLower().Equals(nodeName))
                                        break;
                                }
                                break;

                            case "accessrestrict":
                                returnValue.Bib_Info.Add_Note( Clean_Text_Block( reader.ReadInnerXml()), Note_Type_Enum.restriction);
                                break;

                            case "userestrict":
                                returnValue.Bib_Info.Access_Condition.Text = Clean_Text_Block(reader.ReadInnerXml());
                                break;

                            case "acqinfo":
                                returnValue.Bib_Info.Add_Note(Clean_Text_Block(reader.ReadInnerXml()), Note_Type_Enum.acquisition);
                                break;

                            case "bioghist":
                                returnValue.Bib_Info.Add_Note(Clean_Text_Block(reader.ReadInnerXml()), Note_Type_Enum.biographical);
                                break;

                            case "scopecontent":
                                returnValue.Bib_Info.Add_Abstract(Clean_Text_Block(reader.ReadInnerXml()), "", "summary", "Summary");
                                break;

                            case "controlaccess":
                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element && (reader.Name.Equals("corpname") || reader.Name.Equals("persname")))
                                    {
                                        string tagnamei = reader.Name;
                                        string source = "";
                                        if (reader.MoveToAttribute("source"))
                                            source = reader.Value;
                                        while (reader.Read())
                                        {
                                            if (reader.NodeType == XmlNodeType.Text)
                                            {
                                                Subject_Info_Name newName = new Subject_Info_Name();
                                                //ToSee: where to add name? and ehat types
                                                //ToDo: Type
                                                newName.Full_Name = Trim_Final_Punctuation(reader.Value);
                                                newName.Authority = source;
                                                returnValue.Bib_Info.Add_Subject(newName);
                                                if (tagnamei.StartsWith("corp"))
                                                    newName.Name_Type = Name_Info_Type_Enum.corporate;
                                                else if (tagnamei.StartsWith("pers"))
                                                    newName.Name_Type = Name_Info_Type_Enum.personal;
                                                else
                                                    newName.Name_Type = Name_Info_Type_Enum.UNKNOWN;
                                            }
                                            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals(tagnamei))
                                                break;
                                        }
                                    }
                                    else if (reader.NodeType == XmlNodeType.Element && (reader.Name.Equals("subject")))
                                    {
                                        string tagnamei = reader.Name;
                                        string source = "";
                                        if (reader.MoveToAttribute("source"))
                                            source = reader.Value;
                                        while (reader.Read())
                                        {
                                            if (reader.NodeType == XmlNodeType.Text)
                                            {
                                                string subjectTerm = Trim_Final_Punctuation(reader.Value.Trim());
                                                if ( subjectTerm.Length > 1 )
                                                {
                                                    Subject_Info_Standard subject = returnValue.Bib_Info.Add_Subject();
                                                    subject.Authority = source;
                                                    if ( subjectTerm.IndexOf("--") == 0 )
                                                        subject.Add_Topic( subjectTerm );
                                                    else
                                                    {
                                                        while( subjectTerm.IndexOf("--") > 0 )
                                                        {
                                                            string fragment = subjectTerm.Substring(0, subjectTerm.IndexOf("--")).Trim();
                                                            if ( fragment.ToLower() == "florida" )
                                                                subject.Add_Geographic(fragment);
                                                            else
                                                                subject.Add_Topic(fragment);

                                                            if ( subjectTerm.Length > subjectTerm.IndexOf("--") + 3 )
                                                                subjectTerm = subjectTerm.Substring( subjectTerm.IndexOf("--") + 2 );
                                                            else
                                                                subjectTerm = String.Empty;
                                                        }
                                                        if ( subjectTerm.Trim().Length > 0 )
                                                        {
                                                            string fragment = subjectTerm.Trim();
                                                            if ( fragment.ToLower() == "florida" )
                                                                subject.Add_Geographic(fragment);
                                                            else
                                                                subject.Add_Topic(fragment);
                                                        }

                                                    }
                                                
                                                }
                                            }
                                            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals(tagnamei))
                                                break;
                                        }
                                    }
                                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name.ToLower().Equals(nodeName))
                                        break;
                                }
                                break;

                            case "dsc":
                                returnValue.EAD.Container_Hierarchy.Read(reader);
                                break;

                        }
                    }


                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("ead"))
                        break;
                }

                reader.Close();

                return returnValue;
            }
            catch 
            {
                return null;
            }
        }

        private static string Trim_Final_Punctuation(string ToBeTrimmed)
        {
            if (ToBeTrimmed.Length == 0)
                return ToBeTrimmed;

            if (ToBeTrimmed[ToBeTrimmed.Length - 1] == '.')
            {
                return ToBeTrimmed.Substring(0, ToBeTrimmed.Length - 1);
            }

            return ToBeTrimmed;
        }

        private static string Clean_Text_Block(string InnerXML)
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
