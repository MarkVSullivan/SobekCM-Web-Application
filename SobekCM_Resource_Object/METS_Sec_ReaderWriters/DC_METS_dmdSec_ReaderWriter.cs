#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM.Resource_Object.METS_Sec_ReaderWriters
{
    public class DC_METS_dmdSec_ReaderWriter : XML_Writing_Base_Type, iPackage_dmdSec_ReaderWriter
    {
        #region iPackage_dmdSec_ReaderWriter Members

        /// <summary> Flag indicates if this active reader/writer will write a dmdSec </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> TRUE if the package has data to be written, otherwise fALSE </returns>
        public bool Include_dmdSec(SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            return METS_Item.Bib_Info.hasData;
        }

        /// <summary> Writes the dmdSec for the entire package to the text writer </summary>
        /// <param name="Output_Stream">Stream to which the formatted text is written </param>
        /// <param name="METS_Item">Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_dmdSec(TextWriter Output_Stream, SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            Write_Simple_Dublin_Core(Output_Stream, METS_Item.Bib_Info);
            return true;
        }

        /// <summary> Reads the dmdSec at the current position in the XmlTextReader and associates it with the 
        /// entire package  </summary>
        /// <param name="Input_XmlReader"> Open XmlReader from which to read the metadata </param>
        /// <param name="Return_Package"> Package into which to read the metadata</param>
        /// <param name="Options"> Dictionary of any options which this METS section reader may utilize</param>
        /// <returns> TRUE if successful, otherwise FALSE</returns>
        public bool Read_dmdSec(XmlReader Input_XmlReader, SobekCM_Item Return_Package, Dictionary<string, object> Options)
        {
            Read_Simple_Dublin_Core_Info(Input_XmlReader, Return_Package.Bib_Info);
            return true;
        }

        /// <summary> Flag indicates if this active reader/writer needs to append schema reference information
        /// to the METS XML header by analyzing the contents of the digital resource item </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> TRUE if the schema should be attached, otherwise fALSE </returns>
        public bool Schema_Reference_Required_Package(SobekCM_Item METS_Item)
        {
            // If this reader/writer is being utilized, it will almost certainly have data to write.
            return true;
        }

        /// <summary> Returns the schema namespace (xmlns) information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema namespace info for the METS header</returns>
        public string[] Schema_Namespace(SobekCM_Item METS_Item)
        {
            // If this reader/writer is being utilized, it will almost certainly have data to write.
            return new string[] {"dc=\"http://purl.org/dc/elements/1.1/\""};
        }

        /// <summary> Returns the schema location information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema location for the METS header</returns>
        public string[] Schema_Location(SobekCM_Item METS_Item)
        {
            // If this reader/writer is being utilized, it will almost certainly have data to write.
            return new string[] {"    http://purl.org/dc/elements/1.1/\r\n    http://dublincore.org/schemas/xmls/simpledc20021212.xsd"};
        }

        #endregion

        #region Static method to write simple dublin core from a bibliographic object to an output stream

        public static void Write_Simple_Dublin_Core(TextWriter Output, Bibliographic_Info thisBibInfo)
        {
            // Add all the titles
            Output.WriteLine("<dc:title>" + thisBibInfo.Main_Title.ToString() + "</dc:title>");
            List<string> titles = new List<string>();
            titles.Add(thisBibInfo.Main_Title.ToString().Trim());
            if (thisBibInfo.Other_Titles_Count > 0)
            {
                foreach (Title_Info thisTitle in thisBibInfo.Other_Titles)
                {
                    if (!titles.Contains(thisTitle.ToString().Trim()))
                    {
                        Output.WriteLine("<dc:title>" + Convert_String_To_XML_Safe_Static(thisTitle.ToString()) + "</dc:title>");
                        titles.Add(thisTitle.ToString().Trim());
                    }
                }
            }

            // Series title maps to dc:relation
            if ((thisBibInfo.hasSeriesTitle) && (thisBibInfo.SeriesTitle.Title.Length > 0))
            {
                if (!titles.Contains(thisBibInfo.SeriesTitle.ToString().Trim()))
                {
                    Output.WriteLine("<dc:relation>" + Convert_String_To_XML_Safe_Static(thisBibInfo.SeriesTitle.ToString()) + "</dc:relation>");
                }
            }

            // Add all the creators
            List<string> contributors = new List<string>();
            if ((thisBibInfo.hasMainEntityName) && (thisBibInfo.Main_Entity_Name.Full_Name.Length > 0))
            {
                if ((thisBibInfo.Main_Entity_Name.Roles.Count == 0) || (thisBibInfo.Main_Entity_Name.Roles[0].Role.ToUpper() != "CONTRIBUTOR"))
                    Output.WriteLine("<dc:creator>" + Convert_String_To_XML_Safe_Static(thisBibInfo.Main_Entity_Name.ToString().Replace("<i>", "").Replace("</i>", "")) + "</dc:creator>");
                else
                    contributors.Add(thisBibInfo.Main_Entity_Name.ToString(false).Replace("<i>", "").Replace("</i>", ""));
            }
            if (thisBibInfo.Names_Count > 0)
            {
                foreach (Name_Info thisName in thisBibInfo.Names)
                {
                    if ((thisName.Roles.Count == 0) || (thisName.Roles[0].Role.ToUpper() != "CONTRIBUTOR"))
                        Output.WriteLine("<dc:creator>" + Convert_String_To_XML_Safe_Static(thisName.ToString().Replace("<i>", "").Replace("</i>", "")) + "</dc:creator>");
                    else
                        contributors.Add(thisName.ToString(false).Replace("<i>", "").Replace("</i>", ""));
                }
            }

            // Add any collected contributors
            if (contributors.Count > 0)
            {
                foreach (string thisContributor in contributors)
                {
                    Output.WriteLine("<dc:contributor>" + Convert_String_To_XML_Safe_Static(thisContributor) + "</dc:contributor>");
                }
            }

            // Add the coverages (hierarchical geographic)
            if (thisBibInfo.Subjects_Count > 0)
            {
                foreach (Subject_Info thisSubject in thisBibInfo.Subjects)
                {
                    if (thisSubject.Class_Type == Subject_Info_Type.Hierarchical_Spatial)
                    {
                        string subject_string = thisSubject.ToString();
                        if (subject_string.Length > 0)
                        {
                            Output.WriteLine("<dc:coverage>" + Convert_String_To_XML_Safe_Static(subject_string) + "</dc:coverage>");
                        }
                    }
                }
            }

            // Add the coverages (temporal)
            if (thisBibInfo.TemporalSubjects_Count > 0)
            {
                foreach (Temporal_Info thisTemporal in thisBibInfo.TemporalSubjects)
                {
                    if (thisTemporal.TimePeriod.Length > 0)
                    {
                        Output.WriteLine("<dc:coverage>" + Convert_String_To_XML_Safe_Static(thisTemporal.TimePeriod) + "</dc:coverage>");
                    }
                }
            }

            // Add the date issued
            if (thisBibInfo.Origin_Info.Date_Issued.Length > 0)
            {
                Output.WriteLine("<dc:date>" + Convert_String_To_XML_Safe_Static(thisBibInfo.Origin_Info.Date_Issued) + "</dc:date>");
            }

            // Add all descriptions/notes
            if ((thisBibInfo.Original_Description != null) && (thisBibInfo.Original_Description.Notes_Count > 0))
            {
                foreach (string physicalDescNote in thisBibInfo.Original_Description.Notes)
                {
                    Output.WriteLine("<dc:description>" + Convert_String_To_XML_Safe_Static(physicalDescNote) + "</dc:description>");
                }
            }
            if (thisBibInfo.Notes_Count > 0)
            {
                foreach (Note_Info thisNote in thisBibInfo.Notes)
                {
                    if (thisNote.Note_Type != Note_Type_Enum.source)
                        Output.WriteLine("<dc:description>" + Convert_String_To_XML_Safe_Static(thisNote.ToString().Replace("<b>", "(").Replace("</b>", ") ")) + "</dc:description>");
                }
            }

            // Add the format
            if (thisBibInfo.Original_Description.Extent.Length > 0)
            {
                Output.WriteLine("<dc:format>" + Convert_String_To_XML_Safe_Static(thisBibInfo.Original_Description.Extent) + "</dc:format>");
            }

            // Add all the other identifiers
            if (thisBibInfo.Identifiers_Count > 0)
            {
                foreach (Identifier_Info thisIdentifier in thisBibInfo.Identifiers)
                {
                    Output.WriteLine("<dc:identifier>" + Convert_String_To_XML_Safe_Static(thisIdentifier.Identifier) + "</dc:identifier>");
                }
            }

            // Add the language information
            if (thisBibInfo.Languages_Count > 0)
            {
                foreach (Language_Info thisLanguage in thisBibInfo.Languages)
                {
                    if (thisLanguage.Language_Text.Length > 0)
                    {
                        Output.WriteLine("<dc:language>" + Convert_String_To_XML_Safe_Static(thisLanguage.Language_Text) + "</dc:language>");
                    }
                }
            }

            // Add the subjects
            if (thisBibInfo.Subjects_Count > 0)
            {
                foreach (Subject_Info thisSubject in thisBibInfo.Subjects)
                {
                    if (thisSubject.Class_Type != Subject_Info_Type.Hierarchical_Spatial)
                    {
                        if (thisSubject.Class_Type == Subject_Info_Type.Standard)
                        {
                            Subject_Info_Standard standSubj = (Subject_Info_Standard) thisSubject;
                            if (standSubj.Geographics_Count > 0)
                            {
                                string[] geographics = new string[standSubj.Geographics_Count];
                                standSubj.Geographics.CopyTo(geographics, 0);
                                standSubj.Clear_Geographics();
                                string subject_string = thisSubject.ToString();
                                if (subject_string.Length > 0)
                                {
                                    Output.WriteLine("<dc:subject>" + Convert_String_To_XML_Safe_Static(subject_string.Replace("<i>", "").Replace("</i>", "")) + "</dc:subject>");
                                }
                                foreach (string thisGeographic in geographics)
                                {
                                    Output.WriteLine("<dc:coverage>" + Convert_String_To_XML_Safe_Static(thisGeographic) + "</dc:coverage>");
                                    standSubj.Add_Geographic(thisGeographic);
                                }
                            }
                            else
                            {
                                string subject_string = thisSubject.ToString();
                                if (subject_string.Length > 0)
                                {
                                    Output.WriteLine("<dc:subject>" + Convert_String_To_XML_Safe_Static(subject_string.Replace("<i>", "").Replace("</i>", "")) + "</dc:subject>");
                                }
                            }
                        }
                        else
                        {
                            string subject_string = thisSubject.ToString();
                            if (subject_string.Length > 0)
                            {
                                Output.WriteLine("<dc:subject>" + Convert_String_To_XML_Safe_Static(subject_string.Replace("<i>", "").Replace("</i>", "")) + "</dc:subject>");
                            }
                        }
                    }
                }
            }

            // Add all the publishers from the origin info section
            if (thisBibInfo.Origin_Info.Publishers_Count > 0)
            {
                foreach (string publisher in thisBibInfo.Origin_Info.Publishers)
                {
                    Output.WriteLine("<dc:publisher>" + Convert_String_To_XML_Safe_Static(publisher) + "</dc:publisher>");
                }
            }

            // Add all publishers held in the more complete custom section
            if (thisBibInfo.Publishers.Count > 0)
            {
                foreach (Publisher_Info thisPublisher in thisBibInfo.Publishers)
                {
                    Output.WriteLine("<dc:publisher>" + Convert_String_To_XML_Safe_Static(thisPublisher.ToString()) + "</dc:publisher>");
                }
            }

            // Add the type
            string mods_type = thisBibInfo.Type.MODS_Type_String;
            if (mods_type.Length > 0)
            {
                Output.WriteLine("<dc:type>" + Convert_String_To_XML_Safe_Static(mods_type) + "</dc:type>");
            }
            if (thisBibInfo.Type.Uncontrolled_Types_Count > 0)
            {
                foreach (string thisType in thisBibInfo.Type.Uncontrolled_Types)
                {
                    Output.WriteLine("<dc:type>" + Convert_String_To_XML_Safe_Static(thisType) + "</dc:type>");
                }
            }

            // Add the relations
            if (thisBibInfo.RelatedItems_Count > 0)
            {
                foreach (Related_Item_Info thisRelatedItem in thisBibInfo.RelatedItems)
                {
                    if (thisRelatedItem.Main_Title.Title.Trim().Length > 0)
                    {
                        Output.WriteLine("<dc:relation>" + Convert_String_To_XML_Safe_Static(thisRelatedItem.Main_Title.Title) + "</dc:relation>");
                    }
                }
            }

            // Add the rights
            if (thisBibInfo.Access_Condition.Text.Length > 0)
            {
                Output.WriteLine("<dc:rights>" + Convert_String_To_XML_Safe_Static(thisBibInfo.Access_Condition.Text) + "</dc:rights>");
            }

            // Add the source note
            if (thisBibInfo.Notes_Count > 0)
            {
                foreach (Note_Info thisNote in thisBibInfo.Notes)
                {
                    if (thisNote.Note_Type == Note_Type_Enum.source)
                        Output.WriteLine("<dc:source>" + Convert_String_To_XML_Safe_Static(thisNote.Note) + "</dc:source>");
                }
            }

            //// Add the source information
            //if (thisBibInfo.Source.Statement.Length > 0)
            //{
            //    Output.WriteLine("<dc:source>" + thisBibInfo.Source.XML_Safe_Statement + "</dc:source>");
            //}
        }

        #endregion

        #region Static method to read dublin core information into a bibliographic object 

        /// <summary> Reads the Dublin Core-compliant section of XML and stores the data in the provided digital resource </summary>
        /// <param name="r"> XmlTextReader from which to read the dublin core data </param>
        /// <param name="thisBibInfo"> Digital resource object to save the data to </param>
        public static void Read_Simple_Dublin_Core_Info(XmlReader r, Bibliographic_Info thisBibInfo)
        {
            while (r.Read())
            {
                if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "METS:mdWrap") || (r.Name == "mdWrap")))
                    return;

                if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.Name)
                    {
                        case "dc:contributor":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                thisBibInfo.Add_Named_Entity(r.Value.Trim(), "Contributor");
                            }
                            break;

                        case "dc:coverage":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                Subject_Info_Standard thisSubject = new Subject_Info_Standard();
                                thisSubject.Add_Geographic(r.Value.Trim());
                                thisBibInfo.Add_Subject(thisSubject);
                            }
                            break;

                        case "dc:creator":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                if (thisBibInfo.Main_Entity_Name.hasData)
                                {
                                    thisBibInfo.Add_Named_Entity(r.Value.Trim());
                                }
                                else
                                {
                                    thisBibInfo.Main_Entity_Name.Full_Name = r.Value.Trim();
                                }
                            }
                            break;

                        case "dc:date":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                thisBibInfo.Origin_Info.Date_Issued = r.Value.Trim();
                            }
                            break;

                        case "dc:description":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                thisBibInfo.Add_Note(r.Value.Trim());
                            }
                            break;

                        case "dc:format":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                thisBibInfo.Original_Description.Extent = r.Value.Trim();
                            }
                            break;

                        case "dc:identifier":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                thisBibInfo.Add_Identifier(r.Value.Trim());
                            }
                            break;

                        case "dc:language":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                thisBibInfo.Add_Language(r.Value.Trim());
                            }
                            break;

                        case "dc:publisher":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                thisBibInfo.Add_Publisher(r.Value.Trim());
                            }
                            break;

                        case "dc:relation":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                Related_Item_Info newRelatedItem = new Related_Item_Info();
                                newRelatedItem.Main_Title.Title = r.Value.Trim();
                                thisBibInfo.Add_Related_Item(newRelatedItem);
                            }
                            break;

                        case "dc:rights":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                thisBibInfo.Access_Condition.Text = r.Value.Trim();
                            }
                            break;

                        case "dc:source":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                thisBibInfo.Add_Note(r.Value, Note_Type_Enum.source);
                            }
                            break;

                        case "dc:subject":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                if (r.Value.IndexOf(";") > 0)
                                {
                                    string[] splitter = r.Value.Split(";".ToCharArray());
                                    foreach (string thisSplit in splitter)
                                    {
                                        thisBibInfo.Add_Subject(thisSplit.Trim(), String.Empty);
                                    }
                                }
                                else
                                {
                                    thisBibInfo.Add_Subject(r.Value.Trim(), String.Empty);
                                }
                            }
                            break;

                        case "dc:title":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                if (thisBibInfo.Main_Title.Title.Length == 0)
                                {
                                    thisBibInfo.Main_Title.Title = r.Value.Trim();
                                }
                                else
                                {
                                    thisBibInfo.Add_Other_Title(r.Value.Trim(), Title_Type_Enum.alternative);
                                }
                            }
                            break;

                        case "dc:type":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                thisBibInfo.Type.Add_Uncontrolled_Type(r.Value.Trim());
                            }
                            break;
                    }
                }
            }
        }

        #endregion
    }
}