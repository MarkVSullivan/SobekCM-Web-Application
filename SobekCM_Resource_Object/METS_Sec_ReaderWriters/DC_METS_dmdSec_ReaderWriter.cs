#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Metadata_Modules;

#endregion

namespace SobekCM.Resource_Object.METS_Sec_ReaderWriters
{
	/// <summary> Class reads and writes simple dublin core within a METS file </summary>
	/// <remarks> This also looks for the ETD-MS fields used for Electronic Theses and Dissertations.
	/// However, this will not be WRITTEN unless the <see cref="ETD_MS_DC_METS_dmdSec_ReaderWriter"/> class
	/// is utilized in the METS writing profile </remarks>
    public class DC_METS_dmdSec_ReaderWriter : XML_Writing_Base_Type, iPackage_dmdSec_ReaderWriter
    {
        #region iPackage_dmdSec_ReaderWriter Members

        /// <summary> Flag indicates if this active reader/writer will write a dmdSec </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
		/// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
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
        public virtual bool Write_dmdSec(TextWriter Output_Stream, SobekCM_Item METS_Item, Dictionary<string, object> Options)
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
            Read_Simple_Dublin_Core_Info(Input_XmlReader, Return_Package.Bib_Info, Return_Package);
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

        /// <summary> Class writes simple dublin core to an output stream </summary>
        /// <param name="Output"> Output stream to write to </param>
        /// <param name="BibInfo"> Bibliographic information to write </param>
        public static void Write_Simple_Dublin_Core(TextWriter Output, Bibliographic_Info BibInfo)
        {
            // Add all the titles
            Output.WriteLine("<dc:title>" + BibInfo.Main_Title + "</dc:title>");
            List<string> titles = new List<string> {BibInfo.Main_Title.ToString().Trim()};
	        if (BibInfo.Other_Titles_Count > 0)
            {
                foreach (Title_Info thisTitle in BibInfo.Other_Titles)
                {
                    if (!titles.Contains(thisTitle.ToString().Trim()))
                    {
                        Output.WriteLine("<dc:title>" + Convert_String_To_XML_Safe_Static(thisTitle.ToString()) + "</dc:title>");
                        titles.Add(thisTitle.ToString().Trim());
                    }
                }
            }

            // Series title maps to dc:relation
            if ((BibInfo.hasSeriesTitle) && (BibInfo.SeriesTitle.Title.Length > 0))
            {
                if (!titles.Contains(BibInfo.SeriesTitle.ToString().Trim()))
                {
                    Output.WriteLine("<dc:relation>" + Convert_String_To_XML_Safe_Static(BibInfo.SeriesTitle.ToString()) + "</dc:relation>");
                }
            }

            // Add all the creators
            List<string> contributors = new List<string>();
            if ((BibInfo.hasMainEntityName) && (BibInfo.Main_Entity_Name.Full_Name.Length > 0))
            {
                if ((BibInfo.Main_Entity_Name.Roles.Count == 0) || (BibInfo.Main_Entity_Name.Roles[0].Role.ToUpper() != "CONTRIBUTOR"))
                    Output.WriteLine("<dc:creator>" + Convert_String_To_XML_Safe_Static(BibInfo.Main_Entity_Name.ToString().Replace("<i>", "").Replace("</i>", "")) + "</dc:creator>");
                else
                    contributors.Add(BibInfo.Main_Entity_Name.ToString(false).Replace("<i>", "").Replace("</i>", ""));
            }
            if (BibInfo.Names_Count > 0)
            {
                foreach (Name_Info thisName in BibInfo.Names)
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
            if (BibInfo.Subjects_Count > 0)
            {
                foreach (Subject_Info thisSubject in BibInfo.Subjects)
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
            if (BibInfo.TemporalSubjects_Count > 0)
            {
                foreach (Temporal_Info thisTemporal in BibInfo.TemporalSubjects)
                {
                    if (thisTemporal.TimePeriod.Length > 0)
                    {
                        Output.WriteLine("<dc:coverage>" + Convert_String_To_XML_Safe_Static(thisTemporal.TimePeriod) + "</dc:coverage>");
                    }
                }
            }

            // Add the date issued
            if (BibInfo.Origin_Info.Date_Issued.Length > 0)
            {
                Output.WriteLine("<dc:date>" + Convert_String_To_XML_Safe_Static(BibInfo.Origin_Info.Date_Issued) + "</dc:date>");
            }

            // Add all descriptions/notes
            if ((BibInfo.Original_Description != null) && (BibInfo.Original_Description.Notes_Count > 0))
            {
                foreach (string physicalDescNote in BibInfo.Original_Description.Notes)
                {
                    Output.WriteLine("<dc:description>" + Convert_String_To_XML_Safe_Static(physicalDescNote) + "</dc:description>");
                }
            }
            if (BibInfo.Notes_Count > 0)
            {
                foreach (Note_Info thisNote in BibInfo.Notes)
                {
                    if (thisNote.Note_Type != Note_Type_Enum.source)
                        Output.WriteLine("<dc:description>" + Convert_String_To_XML_Safe_Static(thisNote.ToString().Replace("<b>", "(").Replace("</b>", ") ")) + "</dc:description>");
                }
            }

            // Add the format
            if (BibInfo.Original_Description.Extent.Length > 0)
            {
                Output.WriteLine("<dc:format>" + Convert_String_To_XML_Safe_Static(BibInfo.Original_Description.Extent) + "</dc:format>");
            }

            // Add all the other identifiers
            if (BibInfo.Identifiers_Count > 0)
            {
                foreach (Identifier_Info thisIdentifier in BibInfo.Identifiers)
                {
                    Output.WriteLine("<dc:identifier>" + Convert_String_To_XML_Safe_Static(thisIdentifier.Identifier) + "</dc:identifier>");
                }
            }

            // Add the language information
            if (BibInfo.Languages_Count > 0)
            {
                foreach (Language_Info thisLanguage in BibInfo.Languages)
                {
                    if (thisLanguage.Language_Text.Length > 0)
                    {
                        Output.WriteLine("<dc:language>" + Convert_String_To_XML_Safe_Static(thisLanguage.Language_Text) + "</dc:language>");
                    }
                }
            }

            // Add the subjects
            if (BibInfo.Subjects_Count > 0)
            {
                foreach (Subject_Info thisSubject in BibInfo.Subjects)
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
            if (BibInfo.Origin_Info.Publishers_Count > 0)
            {
                foreach (string publisher in BibInfo.Origin_Info.Publishers)
                {
                    Output.WriteLine("<dc:publisher>" + Convert_String_To_XML_Safe_Static(publisher) + "</dc:publisher>");
                }
            }

            // Add all publishers held in the more complete custom section
            if (BibInfo.Publishers.Count > 0)
            {
                foreach (Publisher_Info thisPublisher in BibInfo.Publishers)
                {
                    Output.WriteLine("<dc:publisher>" + Convert_String_To_XML_Safe_Static(thisPublisher.ToString()) + "</dc:publisher>");
                }
            }

            // Add the type
            string mods_type = BibInfo.Type.MODS_Type_String;
            if (mods_type.Length > 0)
            {
                Output.WriteLine("<dc:type>" + Convert_String_To_XML_Safe_Static(mods_type) + "</dc:type>");
            }
            if (BibInfo.Type.Uncontrolled_Types_Count > 0)
            {
                foreach (string thisType in BibInfo.Type.Uncontrolled_Types)
                {
                    Output.WriteLine("<dc:type>" + Convert_String_To_XML_Safe_Static(thisType) + "</dc:type>");
                }
            }

            // Add the relations
            if (BibInfo.RelatedItems_Count > 0)
            {
                foreach (Related_Item_Info thisRelatedItem in BibInfo.RelatedItems)
                {
                    if (thisRelatedItem.Main_Title.Title.Trim().Length > 0)
                    {
                        Output.WriteLine("<dc:relation>" + Convert_String_To_XML_Safe_Static(thisRelatedItem.Main_Title.Title) + "</dc:relation>");
                    }
                }
            }

            // Add the rights
            if (BibInfo.Access_Condition.Text.Length > 0)
            {
                Output.WriteLine("<dc:rights>" + Convert_String_To_XML_Safe_Static(BibInfo.Access_Condition.Text) + "</dc:rights>");
            }

            // Add the source note
            if (BibInfo.Notes_Count > 0)
            {
                foreach (Note_Info thisNote in BibInfo.Notes)
                {
                    if (thisNote.Note_Type == Note_Type_Enum.source)
                        Output.WriteLine("<dc:source>" + Convert_String_To_XML_Safe_Static(thisNote.Note) + "</dc:source>");
                }
            }

            //// Add the source information
            //if (BibInfo.Source.Statement.Length > 0)
            //{
            //    Output.WriteLine("<dc:source>" + BibInfo.Source.XML_Safe_Statement + "</dc:source>");
            //}
        }

        #endregion

        #region Static method to read dublin core information into a bibliographic object 

        /// <summary> Reads the Dublin Core-compliant section of XML and stores the data in the provided digital resource </summary>
        /// <param name="R"> XmlTextReader from which to read the dublin core data </param>
        /// <param name="BibInfo"> Digital resource object to save the data to </param>
        /// <param name="Return_Package"> The return package, if this is reading a top-level section of dublin core </param>
        public static void Read_Simple_Dublin_Core_Info(XmlReader R, Bibliographic_Info BibInfo, SobekCM_Item Return_Package )
        {
            while (R.Read())
            {
                if ((R.NodeType == XmlNodeType.EndElement) && ((R.Name == "METS:mdWrap") || (R.Name == "mdWrap")))
                    return;

                if (R.NodeType == XmlNodeType.Element)
                {
                    switch (R.Name)
                    {
                        case "dc:contributor":
                            R.Read();
                            if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0))
                            {
                                BibInfo.Add_Named_Entity(R.Value.Trim(), "Contributor");
                            }
                            break;

                        case "dc:coverage":
                            R.Read();
                            if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0))
                            {
                                Subject_Info_Standard thisSubject = new Subject_Info_Standard();
                                thisSubject.Add_Geographic(R.Value.Trim());
                                BibInfo.Add_Subject(thisSubject);
                            }
                            break;

                        case "dc:creator":
                            R.Read();
                            if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0))
                            {
                                if (BibInfo.Main_Entity_Name.hasData)
                                {
                                    BibInfo.Add_Named_Entity(R.Value.Trim());
                                }
                                else
                                {
                                    BibInfo.Main_Entity_Name.Full_Name = R.Value.Trim();
                                }
                            }
                            break;

                        case "dc:date":
                            R.Read();
                            if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0))
                            {
                                BibInfo.Origin_Info.Date_Issued = R.Value.Trim();
                            }
                            break;

                        case "dc:description":
                            R.Read();
                            if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0))
                            {
                                BibInfo.Add_Note(R.Value.Trim());
                            }
                            break;

                        case "dc:format":
                            R.Read();
                            if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0))
                            {
                                BibInfo.Original_Description.Extent = R.Value.Trim();
                            }
                            break;

                        case "dc:identifier":
                            R.Read();
                            if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0))
                            {
                                BibInfo.Add_Identifier(R.Value.Trim());
                            }
                            break;

                        case "dc:language":
                            R.Read();
                            if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0))
                            {
                                BibInfo.Add_Language(R.Value.Trim());
                            }
                            break;

                        case "dc:publisher":
                            R.Read();
                            if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0))
                            {
                                BibInfo.Add_Publisher(R.Value.Trim());
                            }
                            break;

                        case "dc:relation":
                            R.Read();
                            if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0))
                            {
                                Related_Item_Info newRelatedItem = new Related_Item_Info();
                                newRelatedItem.Main_Title.Title = R.Value.Trim();
                                BibInfo.Add_Related_Item(newRelatedItem);
                            }
                            break;

                        case "dc:rights":
                            R.Read();
                            if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0))
                            {
                                BibInfo.Access_Condition.Text = R.Value.Trim();
                            }
                            break;

                        case "dc:source":
                            R.Read();
                            if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0))
                            {
                                BibInfo.Add_Note(R.Value, Note_Type_Enum.source);
                            }
                            break;

                        case "dc:subject":
                            R.Read();
                            if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0))
                            {
                                if (R.Value.IndexOf(";") > 0)
                                {
                                    string[] splitter = R.Value.Split(";".ToCharArray());
                                    foreach (string thisSplit in splitter)
                                    {
                                        BibInfo.Add_Subject(thisSplit.Trim(), String.Empty);
                                    }
                                }
                                else
                                {
                                    BibInfo.Add_Subject(R.Value.Trim(), String.Empty);
                                }
                            }
                            break;

                        case "dc:title":
                            R.Read();
                            if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0))
                            {
                                if (BibInfo.Main_Title.Title.Length == 0)
                                {
                                    BibInfo.Main_Title.Title = R.Value.Trim();
                                }
                                else
                                {
                                    BibInfo.Add_Other_Title(R.Value.Trim(), Title_Type_Enum.alternative);
                                }
                            }
                            break;

                        case "dc:type":
                            R.Read();
                            if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0))
                            {
                                BibInfo.Type.Add_Uncontrolled_Type(R.Value.Trim());
                            }
                            break;

						case "thesis.degree.name":
							R.Read();
							if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0) && ( Return_Package != null ))
							{
								// Ensure the thesis object exists and is added
								Thesis_Dissertation_Info thesisInfo = Return_Package.Get_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY) as Thesis_Dissertation_Info;
								if (thesisInfo == null)
								{
									thesisInfo = new Thesis_Dissertation_Info();
									Return_Package.Add_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY, thesisInfo);
								}

								thesisInfo.Degree = R.Value.Trim();
							}
							break;

						case "thesis.degree.level":
							R.Read();
							if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0) && (Return_Package != null))
							{
								// Ensure the thesis object exists and is added
								Thesis_Dissertation_Info thesisInfo = Return_Package.Get_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY) as Thesis_Dissertation_Info;
								if (thesisInfo == null)
								{
									thesisInfo = new Thesis_Dissertation_Info();
									Return_Package.Add_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY, thesisInfo);
								}

								string temp = R.Value.Trim().ToLower();
								if ((temp == "doctorate") || (temp == "doctoral"))
									thesisInfo.Degree_Level = Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Doctorate;
								if ((temp == "masters") || (temp == "master's"))
									thesisInfo.Degree_Level = Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Masters;
								if ((temp == "bachelors") || (temp == "bachelor's"))
									thesisInfo.Degree_Level = Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Bachelors;
								if ((temp == "post-doctorate") || (temp == "post-doctoral"))
									thesisInfo.Degree_Level = Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Bachelors;
							}
							break;

						case "thesis.degree.discipline":
							R.Read();
							if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0) && (Return_Package != null))
							{
								// Ensure the thesis object exists and is added
								Thesis_Dissertation_Info thesisInfo = Return_Package.Get_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY) as Thesis_Dissertation_Info;
								if (thesisInfo == null)
								{
									thesisInfo = new Thesis_Dissertation_Info();
									Return_Package.Add_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY, thesisInfo);
								}

								thesisInfo.Add_Degree_Discipline(R.Value.Trim());
							}
							break;

						case "thesis.degree.grantor":
							R.Read();
							if ((R.NodeType == XmlNodeType.Text) && (R.Value.Trim().Length > 0) && (Return_Package != null))
							{
								// Ensure the thesis object exists and is added
								Thesis_Dissertation_Info thesisInfo = Return_Package.Get_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY) as Thesis_Dissertation_Info;
								if (thesisInfo == null)
								{
									thesisInfo = new Thesis_Dissertation_Info();
									Return_Package.Add_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY, thesisInfo);
								}

								thesisInfo.Degree_Grantor = R.Value.Trim();
							}
							break;
                    }
                }
            }
        }

        #endregion
    }
}