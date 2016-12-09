#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.MARC;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;

#endregion

namespace SobekCM.Resource_Object.METS_Sec_ReaderWriters
{
    /// <summary> MARCxml reader that operates against a single METS section  </summary>
    public class MarcXML_METS_dmdSec_ReaderWriter : XML_Writing_Base_Type, iPackage_dmdSec_ReaderWriter
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
        public bool Write_dmdSec(TextWriter Output_Stream, SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            Write_MarcXML(Output_Stream, METS_Item, true);
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
            Read_MarcXML_Info(Input_XmlReader, Return_Package.Bib_Info, Return_Package, false);
            return true;
        }

        /// <summary> Flag indicates if this active reader/writer needs to append schema reference information
        /// to the METS XML header by analyzing the contents of the digital resource item </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> TRUE if the schema should be attached, otherwise fALSE </returns>
        public bool Schema_Reference_Required_Package(SobekCM_Item METS_Item)
        {
            // The MarcXML schema is currently included in the record which is written, not in the METS header
            return false;
        }

        /// <summary> Returns the schema namespace (xmlns) information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema namespace info for the METS header</returns>
        public string[] Schema_Namespace(SobekCM_Item METS_Item)
        {
            // The MarcXML schema is currently included in the record which is written, not in the METS head
            return new string[0];
        }

        /// <summary> Returns the schema location information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema location for the METS header</returns>
        public string[] Schema_Location(SobekCM_Item METS_Item)
        {
            // The MarcXML schema is currently included in the record which is written, not in the METS head
            return new string[0];
        }

        #endregion

        #region Static methods to write the MarcXML information

        /// <summary> Add the bibliographic information as MARC21 slim XML to the output stream for a given digital resource </summary>
        /// <param name="Output"> Output stream for this metadata to be written to </param>
        /// <param name="thisBib"> Source digital resource </param>
        /// <param name="Include_Schema"> Flag indicates whether the schema information should be included in the record tag </param>
        public static void Write_MarcXML(TextWriter Output, SobekCM_Item thisBib, bool Include_Schema)
        {
            // Get all the standard tags
            MARC_Record tags = thisBib.To_MARC_Record();

            // Start to build the XML result
            if (Include_Schema)
            {
                Output.Write("<record xmlns=\"http://www.loc.gov/MARC21/slim\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.loc.gov/MARC21/slim http://www.loc.gov/standards/marcxml/schema/MARC21slim.xsd\">\r\n");
            }
            else
            {
                Output.Write("<record>\r\n");
            }

            // Add the leader
            Output.Write("<leader>" + tags.Leader + "</leader>\r\n");

            foreach (MARC_Field thisTag in tags.Sorted_MARC_Tag_List)
            {
                if ((thisTag.Tag == 1) || (thisTag.Tag == 3) || (thisTag.Tag == 5) || (thisTag.Tag == 6) || (thisTag.Tag == 7) || (thisTag.Tag == 8))
                {
                    Output.Write("<controlfield tag=\"" + thisTag.Tag.ToString().PadLeft(3, '0') + "\">" + Convert_String_To_XML_Safe_Static(thisTag.Control_Field_Value) + "</controlfield>\r\n");
                }
                else
                {
                    Output.Write("<datafield tag=\"" + thisTag.Tag.ToString().PadLeft(3, '0') + "\" ind1=\"" + thisTag.Indicators[0] + "\" ind2=\"" + thisTag.Indicators[1] + "\">\r\n");

                    string[] splitter = thisTag.Control_Field_Value.Split("|".ToCharArray());
                    foreach (string subfield in splitter)
                    {
                        if ((subfield.Length > 2) && ((Char.IsLetter(subfield[0])) || (Char.IsNumber(subfield[0]))))
                        {
                            Output.Write("<subfield code=\"" + subfield[0] + "\">" + Convert_String_To_XML_Safe_Static(subfield.Substring(2).Trim()) + "</subfield>\r\n");
                        }
                    }

                    Output.Write("</datafield>\r\n");
                }
            }

            Output.Write("</record>\r\n");
        }

        #endregion

        #region Static methods to read the MarcXML information

	    /// <summary> Reads the MARC Core-compliant section of XML and stores the data in the provided digital resource </summary>
	    /// <param name="r"> XmlTextReader from which to read the marc data </param>
	    /// <param name="thisBibInfo">Bibliographic object into which most the values are read</param>
	    /// <param name="package"> Digital resource object to save the data to if this is reading the top-level bibDesc (OPTIONAL)</param>
	    /// <param name="Importing_Record"> Importing record flag is used to determine if special treatment should be applied to the 001 identifier.  If this is reading MarcXML from a dmdSec, this is set to false </param>
	    public static void Read_MarcXML_Info(XmlReader r, Bibliographic_Info thisBibInfo, SobekCM_Item package, bool Importing_Record )
	    {
			Read_MarcXML_Info(r, thisBibInfo, package, Importing_Record, null );
	    }

	    /// <summary> Reads the MARC Core-compliant section of XML and stores the data in the provided digital resource </summary>
        /// <param name="r"> XmlTextReader from which to read the marc data </param>
        /// <param name="thisBibInfo">Bibliographic object into which most the values are read</param>
        /// <param name="package"> Digital resource object to save the data to if this is reading the top-level bibDesc (OPTIONAL)</param>
        /// <param name="Importing_Record"> Importing record flag is used to determine if special treatment should be applied to the 001 identifier.  If this is reading MarcXML from a dmdSec, this is set to false </param>
		/// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
		public static void Read_MarcXML_Info(XmlReader r, Bibliographic_Info thisBibInfo, SobekCM_Item package, bool Importing_Record, Dictionary<string, object> Options )
        {
            // Create the MARC_XML_Reader to load everything into first
            MARC_Record record = new MARC_Record();

            // Read from the file
            record.Read_MARC_Info(r);

			// Handle optional mapping first for retaining the 856 as a related link
		    if ((Options != null) && (Options.ContainsKey("MarcXML_File_ReaderWriter.Retain_856_As_Related_Link")))
		    {
			    if (Options["MarcXML_File_ReaderWriter.Retain_856_As_Related_Link"].ToString().ToUpper() == "TRUE")
			    {
				    if ((record.Get_Data_Subfield(856, 'u').Length > 0) && (record.Get_Data_Subfield(856, 'y').Length > 0))
				    {
					    string url856 = record.Get_Data_Subfield(856, 'u');
					    string label856 = record.Get_Data_Subfield(856, 'y');

					    thisBibInfo.Location.Other_URL = url856;
					    thisBibInfo.Location.Other_URL_Note = label856;
				    }
			    }
		    }

		    // Now, load values into the bib package 
            // Load the date ( 260 |c )
            thisBibInfo.Origin_Info.MARC_DateIssued = Remove_Trailing_Punctuation(record.Get_Data_Subfield(260, 'c'));


            // Load the descriptions and notes about this item 
            Add_Descriptions(thisBibInfo, record);

            // Look for the 786  with special identifiers to map back into the source notes
            foreach (MARC_Field thisRecord in record[786])
            {
                if ((thisRecord.Indicators == "0 ") && (thisRecord.Subfield_Count == 1) && (thisRecord.has_Subfield('n')))
                    thisBibInfo.Add_Note(thisRecord.Subfields[0].Data, Note_Type_Enum.Source);
            }

            // Add the contents (505)
            if (record.Get_Data_Subfield(505, 'a').Length > 2)
            {
                thisBibInfo.Add_TableOfContents(record.Get_Data_Subfield(505, 'a'));
            }

            // Get the scale information (034)
            if (record.Get_Data_Subfield(034, 'b').Length > 2)
            {
                thisBibInfo.Add_Scale(record.Get_Data_Subfield(034, 'b'), "SUBJ034");
            }

            // Get the scale information (255)
            if ((record.Get_Data_Subfield(255, 'a').Length > 2) || (record.Get_Data_Subfield(255, 'b').Length > 2) || (record.Get_Data_Subfield(255, 'c').Length > 2))
            {
                thisBibInfo.Add_Scale(record.Get_Data_Subfield(255, 'a'), record.Get_Data_Subfield(255, 'b'), record.Get_Data_Subfield(255, 'c'), "SUBJ255");
            }

            // Get the coordinate information (034)
            if ((record.Get_Data_Subfield(034, 'd').Length > 0) && (record.Get_Data_Subfield(034, 'e').Length > 0) && (record.Get_Data_Subfield(034, 'f').Length > 0) && (record.Get_Data_Subfield(034, 'g').Length > 0))
            {
                // This is an extra metadata component
                GeoSpatial_Information geoInfo = package.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                if (geoInfo == null)
                {
                    geoInfo = new GeoSpatial_Information();
                    package.Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, geoInfo);
                }

	            if (geoInfo.Polygon_Count == 0)
	            {
		            try
		            {

			            string d_field = record.Get_Data_Subfield(034, 'd').Replace("O", "0");
			            string e_field = record.Get_Data_Subfield(034, 'e').Replace("O", "0");
			            string f_field = record.Get_Data_Subfield(034, 'f').Replace("O", "0");
			            string g_field = record.Get_Data_Subfield(034, 'g').Replace("O", "0");

			            double d_value = 1;
			            double e_value = 1;
			            double f_value = 1;
			            double g_value = 1;

			            if (d_field.Contains("."))
			            {
				            if (d_field.Contains("W"))
				            {
					            d_value = -1*Convert.ToDouble(d_field.Replace("W", ""));
				            }
				            else
				            {
					            d_value = Convert.ToDouble(d_field.Replace("E", ""));
				            }
			            }
			            else
			            {
				            d_value = Convert.ToDouble(d_field.Substring(1, 3)) + (Convert.ToDouble(d_field.Substring(4, 2))/60);

				            if ((d_field[0] == '-') || (d_field[0] == 'W'))
				            {
					            d_value = -1*d_value;
				            }
			            }

			            if (d_value < -180)
				            d_value = d_value + 360;

			            if (e_field.Contains("."))
			            {
				            if (e_field.Contains("W"))
				            {
					            e_value = -1*Convert.ToDouble(e_field.Replace("W", ""));
				            }
				            else
				            {
					            e_value = Convert.ToDouble(e_field.Replace("E", ""));
				            }
			            }
			            else
			            {
				            e_value = Convert.ToDouble(e_field.Substring(1, 3)) + (Convert.ToDouble(e_field.Substring(4, 2))/60);

				            if ((e_field[0] == '-') || (e_field[0] == 'W'))
				            {
					            e_value = -1*e_value;
				            }
			            }

			            if (e_value < -180)
				            e_value = e_value + 360;

			            if (f_field.Contains("."))
			            {
				            if (f_field.Contains("S"))
				            {
					            f_value = -1*Convert.ToDouble(f_field.Replace("S", ""));
				            }
				            else
				            {
					            f_value = Convert.ToDouble(f_field.Replace("N", ""));
				            }
			            }
			            else
			            {
				            f_value = Convert.ToDouble(f_field.Substring(1, 3)) + (Convert.ToDouble(f_field.Substring(4, 2))/60);

				            if ((f_field[0] == '-') || (f_field[0] == 'S'))
				            {
					            f_value = -1*f_value;
				            }
			            }

			            if (g_field.Contains("."))
			            {
				            if (g_field.Contains("S"))
				            {
					            g_value = -1*Convert.ToDouble(g_field.Replace("S", ""));
				            }
				            else
				            {
					            g_value = Convert.ToDouble(g_field.Replace("N", ""));
				            }
			            }
			            else
			            {
				            g_value = Convert.ToDouble(g_field.Substring(1, 3)) + (Convert.ToDouble(g_field.Substring(4, 2))/60);

				            if ((g_field[0] == '-') || (g_field[0] == 'S'))
				            {
					            g_value = -1*g_value;
				            }
			            }
			            Coordinate_Polygon polygon = new Coordinate_Polygon();
			            polygon.Add_Edge_Point(f_value, d_value);
			            polygon.Add_Edge_Point(g_value, d_value);
			            polygon.Add_Edge_Point(g_value, e_value);
			            polygon.Add_Edge_Point(f_value, e_value);
			            polygon.Label = "Map Coverage";
			            geoInfo.Add_Polygon(polygon);
		            }
		            catch {   }
	            }
            }

            // Add the abstract ( 520 |a )
            foreach (MARC_Field thisRecord in record[520])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    Abstract_Info newAbstract = new Abstract_Info();
                    switch (thisRecord.Indicator1)
                    {
                        case ' ':
                            newAbstract.Type = "summary";
                            newAbstract.Display_Label = "Summary";
                            break;

                        case '0':
                            newAbstract.Type = "subject";
                            newAbstract.Display_Label = "Subject";
                            break;

                        case '1':
                            newAbstract.Type = "review";
                            newAbstract.Display_Label = "Review";
                            break;

                        case '2':
                            newAbstract.Type = "scope and content";
                            newAbstract.Display_Label = "Scope and Content";
                            break;

                        case '4':
                            newAbstract.Type = "content advice";
                            newAbstract.Display_Label = "Content Advice";
                            break;

                        default:
                            newAbstract.Display_Label = "Abstract";
                            break;
                    }

                    if (thisRecord.has_Subfield('b'))
                    {
                        newAbstract.Abstract_Text = thisRecord['a'] + " " + thisRecord['b'];
                    }
                    else
                    {
                        newAbstract.Abstract_Text = thisRecord['a'];
                    }
                    thisBibInfo.Add_Abstract(newAbstract);
                }
            }

            // Load the format ( 300 )
            if (record.has_Field(300))
            {
                StringBuilder builder300 = new StringBuilder();
                if (record.Get_Data_Subfield(300, 'a').Length > 0)
                {
                    builder300.Append(record.Get_Data_Subfield(300, 'a').Replace(":", "").Replace(";", "").Trim());
                }
                builder300.Append(" : ");
                if (record.Get_Data_Subfield(300, 'b').Length > 0)
                {
                    builder300.Append(record.Get_Data_Subfield(300, 'b').Replace(";", "").Trim());
                }
                builder300.Append(" ; ");
                if (record.Get_Data_Subfield(300, 'c').Length > 0)
                {
                    builder300.Append(record.Get_Data_Subfield(300, 'c'));
                }
                thisBibInfo.Original_Description.Extent = builder300.ToString().Trim();
                if (thisBibInfo.Original_Description.Extent.Replace(" ", "").Replace(":", "").Replace(";", "") == "v.")
                    thisBibInfo.Original_Description.Extent = String.Empty;
            }

            // Load the current frequency (310)
            foreach (MARC_Field thisRecord in record[310])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    if (thisRecord.has_Subfield('b'))
                    {
                        thisBibInfo.Origin_Info.Add_Frequency(Remove_Trailing_Punctuation(thisRecord['a']).Replace("[", "(").Replace("]", ")") + "[" + thisRecord['b'].Replace("[", "(").Replace("]", ")") + "]");
                    }
                    else
                    {
                        thisBibInfo.Origin_Info.Add_Frequency(Remove_Trailing_Punctuation(thisRecord['a']).Replace("[", "(").Replace("]", ")"));
                    }
                }
            }

            // Load the previous frequency (321)
            foreach (MARC_Field thisRecord in record[321])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    if (thisRecord.has_Subfield('b'))
                    {
                        thisBibInfo.Origin_Info.Add_Frequency(Remove_Trailing_Punctuation(thisRecord['a']).Replace("[", "(").Replace("]", ")") + "[ FORMER " + thisRecord['b'].Replace("[", "(").Replace("]", ")") + "]");
                    }
                    else
                    {
                        thisBibInfo.Origin_Info.Add_Frequency(Remove_Trailing_Punctuation(thisRecord['a']).Replace("[", "(").Replace("]", ")") + "[ FORMER ]");
                    }
                }
            }

            // Load the edition ( 250 )
            if (record.has_Field(250))
            {
                if (record.Get_Data_Subfield(250, 'b').Length > 0)
                {
                    thisBibInfo.Origin_Info.Edition = record.Get_Data_Subfield(250, 'a').Replace("/", "").Replace("=", "").Trim() + " -- " + record.Get_Data_Subfield(250, 'b');
                }
                else
                {
                    thisBibInfo.Origin_Info.Edition = record.Get_Data_Subfield(250, 'a');
                }
            }

            // Load the language ( 008 )
            if (record.has_Field(8))
            {
                string field_08 = record[8][0].Control_Field_Value;
                if (field_08.Length > 5)
                {
                    // Get the language code
                    string languageCode = field_08.Substring(field_08.Length - 5, 3);

                    // Add as the language of the item
                    Language_Info thisLanguage = thisBibInfo.Add_Language(String.Empty, languageCode, String.Empty);

                    // Add as the language of the cataloging
                    thisBibInfo.Record.Add_Catalog_Language(new Language_Info(thisLanguage.Language_Text, thisLanguage.Language_ISO_Code, String.Empty));
                }
            }

            // Load any additional languages (041)
            foreach (MARC_Field thisRecord in record[041])
            {
                foreach (MARC_Subfield thisSubfield in thisRecord.Subfields)
                {
                    if ((thisSubfield.Subfield_Code == 'a') || (thisSubfield.Subfield_Code == 'b') || (thisSubfield.Subfield_Code == 'd') ||
                        (thisSubfield.Subfield_Code == 'e') || (thisSubfield.Subfield_Code == 'f') || (thisSubfield.Subfield_Code == 'g') ||
                        (thisSubfield.Subfield_Code == 'h'))
                    {
                        thisBibInfo.Add_Language(thisSubfield.Data);
                    }
                }
            }


            // Load the publisher ( 260 |b )
            if (record.has_Field(260))
            {
                string[] special_260_splitter = record[260][0].Control_Field_Value.Split("|".ToCharArray());
                Publisher_Info thisInfo = new Publisher_Info();
                foreach (string thisSplitter in special_260_splitter)
                {
                    if (thisSplitter.Length > 2)
                    {
                        if (thisSplitter[0] == 'a')
                        {
                            thisInfo.Add_Place(Remove_Trailing_Punctuation(thisSplitter.Substring(2).Replace(" :", "").Trim()));
                            thisInfo.Name = "[s.n.]";
                            thisBibInfo.Add_Publisher(thisInfo);
                        }

                        if (thisSplitter[0] == 'b')
                        {
                            string pubname = thisSplitter.Substring(2).Replace(";", "").Trim();
                            if ((pubname.Length > 1) && (pubname[pubname.Length - 1] == ','))
                            {
                                pubname = pubname.Substring(0, pubname.Length - 1);
                            }

                            thisInfo.Name = pubname;
                            thisBibInfo.Add_Publisher(thisInfo);
                            thisInfo = new Publisher_Info();
                        }

                        if (thisSplitter[0] == 'e')
                        {
                            thisInfo.Add_Place(thisSplitter.Substring(2).Replace("(", "").Replace(" :", "").Trim());
                        }

                        if (thisSplitter[0] == 'f')
                        {
                            string manname = thisSplitter.Substring(2).Replace(")", "").Trim();
                            if ((manname.Length > 1) && (manname[manname.Length - 1] == ','))
                            {
                                manname = manname.Substring(0, manname.Length - 1);
                            }

                            thisInfo.Name = manname;
                            thisBibInfo.Add_Manufacturer(thisInfo);
                            thisInfo = new Publisher_Info();
                        }
                    }
                }
            }

            // Load the dates from the 008
            string field_008 = String.Empty;
            if (record.has_Field(008))
            {
                field_008 = record[8][0].Control_Field_Value;
                if (field_008.Length > 14)
                {
                    // Save the two date points
                    thisBibInfo.Origin_Info.MARC_DateIssued_Start = field_008.Substring(7, 4).Trim();
                    thisBibInfo.Origin_Info.MARC_DateIssued_End = field_008.Substring(11, 4).Trim();

                    // See what type of dates they are (if they are special)
                    char date_type = field_008[6];
                    switch (date_type)
                    {
                        case 'r':
                            thisBibInfo.Origin_Info.Date_Reprinted = thisBibInfo.Origin_Info.MARC_DateIssued_Start;
                            break;

                        case 't':
                            thisBibInfo.Origin_Info.Date_Copyrighted = thisBibInfo.Origin_Info.MARC_DateIssued_End;
                            break;
                    }
                }

                if (field_008.Length > 5)
                {
                    thisBibInfo.Record.MARC_Creation_Date = field_008.Substring(0, 6);
                }
            }

            // Load the location from the 008
            if (field_008.Length > 17)
            {
                thisBibInfo.Origin_Info.Add_Place(String.Empty, field_008.Substring(15, 3), String.Empty);
            }

            // Load the main record number ( 001 )
            string idValue;
            string oclc = String.Empty;
            if (record.has_Field(1))
            {
                idValue = record[1][0].Control_Field_Value.Trim();
                if (idValue.Length > 0)
                {
                    thisBibInfo.Record.Main_Record_Identifier.Identifier = idValue;
                    if (Importing_Record)
                    {
                        if (Char.IsNumber(idValue[0]))
                        {
                            // Add this ALEPH number
                            if (thisBibInfo.ALEPH_Record != idValue)
                            {
                                thisBibInfo.Add_Identifier(idValue, "ALEPH");
                            }
                            thisBibInfo.Record.Record_Origin = "Imported from (ALEPH)" + idValue;
                        }
                        else
                        {
                            if (idValue.Length >= 7)
                            {
                                if ((idValue.IndexOf("ocm") == 0) || (idValue.IndexOf("ocn") == 0))
                                {
                                    oclc = idValue.Replace("ocn", "").Replace("ocm", "");
                                    if (thisBibInfo.OCLC_Record != oclc)
                                    {
                                        thisBibInfo.Add_Identifier(oclc, "OCLC");
                                    }
                                    thisBibInfo.Record.Record_Origin = "Imported from (OCLC)" + oclc;
                                }
                                else
                                {
                                    thisBibInfo.Add_Identifier(idValue.Substring(0, 7), "NOTIS");
                                    thisBibInfo.Record.Record_Origin = "Imported from (NOTIS)" + idValue.Substring(0, 7);
                                }
                            }
                        }
                    }
                }
            }

            // If this was OCLC record (non-local) look for a 599 added during time of export
            if (oclc.Length > 0)
            {
                if (record.has_Field(599))
                {
                    // Tracking box number will be in the |a field
                    if ((package != null) && (record[599][0].has_Subfield('a')))
                    {
                        package.Tracking.Tracking_Box = record[599][0]['a'];
                    }

                    // Disposition advice will be in the |b field
                    if ((package != null) && (record[599][0].has_Subfield('b')))
                    {
                        package.Tracking.Disposition_Advice_Notes = record[599][0]['b'];
                        string advice_notes_as_caps = package.Tracking.Disposition_Advice_Notes.ToUpper();
                        if ((advice_notes_as_caps.IndexOf("RETURN") >= 0) || (advice_notes_as_caps.IndexOf("RETAIN") >= 0))
                        {
                            package.Tracking.Disposition_Advice = 1;
                        }
                        else
                        {
                            if (advice_notes_as_caps.IndexOf("WITHDRAW") >= 0)
                            {
                                package.Tracking.Disposition_Advice = 2;
                            }
                            else if (advice_notes_as_caps.IndexOf("DISCARD") >= 0)
                            {
                                package.Tracking.Disposition_Advice = 3;
                            }
                        }
                    }

                    // Do not overlay record in the future will be in the |c field
                    if (record[599][0].has_Subfield('c'))
                    {
                        string record_overlay_notes = record[599][0]['c'].Trim();
                        if (record_overlay_notes.Length > 0)
                        {
                            if (package != null)
                            {
                                package.Tracking.Never_Overlay_Record = true;
                                package.Tracking.Internal_Comments = record_overlay_notes;
                            }
                            thisBibInfo.Record.Record_Content_Source = thisBibInfo.Record.Record_Content_Source + " (" + record_overlay_notes + ")";
                        }
                    }
                }
            }

            // Step through all of the identifiers
            foreach (MARC_Field thisRecord in record[35])
            {
                // Only continue if there is an id in this record
                if (thisRecord.has_Subfield('a'))
                {
                    // Was this the old NOTIS number?
                    if (thisRecord.Indicators == "9 ")
                    {
                        thisBibInfo.Add_Identifier(thisRecord['a'], "NOTIS");
                    }

                    // Was this the OCLC number?
                    if ((oclc.Length == 0) && (thisRecord['a'].ToUpper().IndexOf("OCOLC") >= 0))
                    {
                        thisBibInfo.Add_Identifier(thisRecord['a'].ToUpper().Replace("(OCOLC)", "").Trim(), "OCLC");
                    }

                    // Was this the BIB ID?
                    if ((package != null) && (thisRecord['a'].ToUpper().IndexOf("IID") >= 0))
                    {
                        package.BibID = thisRecord['a'].ToUpper().Replace("(IID)", "").Trim();
                    }
                }
            }

            // Also, look for the old original OCLC in the 776 10 |w
            if (thisBibInfo.OCLC_Record.Length == 0)
            {
                foreach (MARC_Field thisRecord in record[776])
                {
                    if ((thisRecord.Indicators == "1 ") && (thisRecord.has_Subfield('w')) && (thisRecord['w'].ToUpper().IndexOf("OCOLC") >= 0))
                    {
                        thisBibInfo.Add_Identifier(thisRecord['w'].ToUpper().Replace("(OCOLC)", "").Trim(), "OCLC");
                    }
                }
            }

            // Look for the LCCN in field 10
            if (record.Get_Data_Subfield(10, 'a').Length > 0)
                thisBibInfo.Add_Identifier(record.Get_Data_Subfield(10, 'a'), "LCCN");

            // Look for ISBN in field 20
            if (record.Get_Data_Subfield(20, 'a').Length > 0)
                thisBibInfo.Add_Identifier(record.Get_Data_Subfield(20, 'a'), "ISBN");

            // Look for ISSN in field 22
            if (record.Get_Data_Subfield(22, 'a').Length > 0)
                thisBibInfo.Add_Identifier(record.Get_Data_Subfield(22, 'a'), "ISSN");

            // Look for classification ( LCC ) in field 50
            if (record.Get_Data_Subfield(50, 'a').Length > 0)
            {
                string subfield_3 = String.Empty;
                if (record.Get_Data_Subfield(50, '3').Length > 0)
                {
                    subfield_3 = record.Get_Data_Subfield(50, '3');
                }
                if (record.Get_Data_Subfield(50, 'b').Length > 0)
                    thisBibInfo.Add_Classification(record.Get_Data_Subfield(50, 'a') + " " + record.Get_Data_Subfield(50, 'b'), "lcc").Display_Label = subfield_3;
                else
                    thisBibInfo.Add_Classification(record.Get_Data_Subfield(50, 'a'), "lcc").Display_Label = subfield_3;
            }

            // Look for classification ( DDC ) in field 82
            if (record.Get_Data_Subfield(82, 'a').Length > 0)
            {
                string subfield_2 = String.Empty;
                if (record.Get_Data_Subfield(82, '2').Length > 0)
                {
                    subfield_2 = record.Get_Data_Subfield(82, '2');
                }
                if (record.Get_Data_Subfield(82, 'b').Length > 0)
                    thisBibInfo.Add_Classification(record.Get_Data_Subfield(82, 'a') + " " + record.Get_Data_Subfield(82, 'b'), "ddc").Edition = subfield_2;
                else
                    thisBibInfo.Add_Classification(record.Get_Data_Subfield(82, 'a'), "ddc").Edition = subfield_2;
            }

            // Look for classification ( UDC ) in field 80
            if (record.Get_Data_Subfield(80, 'a').Length > 0)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(record.Get_Data_Subfield(80, 'a'));
                if (record.Get_Data_Subfield(80, 'b').Length > 0)
                    builder.Append(" " + record.Get_Data_Subfield(80, 'b'));
                if (record.Get_Data_Subfield(80, 'x').Length > 0)
                    builder.Append(" " + record.Get_Data_Subfield(80, 'x'));
                thisBibInfo.Add_Classification(builder.ToString(), "udc");
            }

            // Look for classification ( NLM ) in field 60
            if (record.Get_Data_Subfield(60, 'a').Length > 0)
            {
                if (record.Get_Data_Subfield(60, 'b').Length > 0)
                    thisBibInfo.Add_Classification(record.Get_Data_Subfield(60, 'a') + " " + record.Get_Data_Subfield(60, 'b'), "nlm");
                else
                    thisBibInfo.Add_Classification(record.Get_Data_Subfield(60, 'a'), "nlm");
            }

            // Look for classification ( SUDOCS or CANDOCS ) in field 86
            foreach (MARC_Field thisRecord in record[84])
            {
                string authority = String.Empty;
                switch (thisRecord.Indicator1)
                {
                    case '0':
                        authority = "sudocs";
                        break;

                    case '1':
                        authority = "candocs";
                        break;

                    default:
                        if (thisRecord.has_Subfield('2'))
                            authority = thisRecord['2'];
                        break;
                }

                if (thisRecord.has_Subfield('a'))
                    thisBibInfo.Add_Classification(thisRecord['a'], authority);
            }


            // Look for other classifications in field 084
            foreach (MARC_Field thisRecord in record[84])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    string subfield_2 = String.Empty;
                    if (thisRecord.has_Subfield('2'))
                    {
                        subfield_2 = thisRecord['2'];
                    }
                    if (thisRecord.has_Subfield('b'))
                        thisBibInfo.Add_Classification(thisRecord['a'] + " " + thisRecord['b'], subfield_2);
                    else
                        thisBibInfo.Add_Classification(thisRecord['a'], subfield_2);
                }
            }

            // Look for any other identifiers in field 24
            foreach (MARC_Field thisRecord in record[24])
            {
                string identifier_source = String.Empty;
                switch (thisRecord.Indicator1)
                {
                    case '0':
                        identifier_source = "isrc";
                        break;

                    case '1':
                        identifier_source = "upc";
                        break;

                    case '2':
                        identifier_source = "ismn";
                        break;

                    case '3':
                        identifier_source = "ian";
                        break;

                    case '4':
                        identifier_source = "sici";
                        break;

                    case '7':
                        identifier_source = thisRecord['2'];
                        break;
                }

                if (thisRecord.has_Subfield('d'))
                {
                    thisBibInfo.Add_Identifier(thisRecord['a'] + " (" + thisRecord['d'] + ")", identifier_source);
                }
                else
                {
                    thisBibInfo.Add_Identifier(thisRecord['a'], identifier_source);
                }
            }


            // Look for the ISSN in the 440 and 490 |x and LCCN in the 490 |l
            foreach (MARC_Field thisRecord in record[440])
            {
                if (thisRecord.has_Subfield('x'))
                {
                    thisBibInfo.Add_Identifier(thisRecord['x'], "ISSN");
                }
            }
            foreach (MARC_Field thisRecord in record[490])
            {
                if (thisRecord.has_Subfield('x'))
                {
                    thisBibInfo.Add_Identifier(thisRecord['x'], "ISSN");
                }
                if (thisRecord.has_Subfield('l'))
                {
                    thisBibInfo.Add_Identifier(thisRecord['l'], "LCCN");
                }
            }

            // Load all the MARC Content Sources (040)
            if (record.has_Field(40))
            {
                if (record.Get_Data_Subfield(40, 'a').Length > 0)
                {
                    thisBibInfo.Record.Add_MARC_Record_Content_Sources(record.Get_Data_Subfield(40, 'a'));
                }
                if (record.Get_Data_Subfield(40, 'b').Length > 0)
                {
                    thisBibInfo.Record.Add_MARC_Record_Content_Sources(record.Get_Data_Subfield(40, 'b'));
                }
                if (record.Get_Data_Subfield(40, 'c').Length > 0)
                {
                    thisBibInfo.Record.Add_MARC_Record_Content_Sources(record.Get_Data_Subfield(40, 'c'));
                }
                string modifying = record.Get_Data_Subfield(40, 'd');
                if (modifying.Length > 0)
                {
                    string[] modSplitter = modifying.Split("|".ToCharArray());
                    foreach (string split in modSplitter)
                    {
                        thisBibInfo.Record.Add_MARC_Record_Content_Sources(split.Trim());
                    }
                }
                if (record.Get_Data_Subfield(40, 'e').Length > 0)
                {
                    thisBibInfo.Record.Description_Standard = record.Get_Data_Subfield(40, 'e');
                }
            }

            // Add the spatial information ( 752, 662 )
            Add_Hierarchical_Subject(thisBibInfo, record, 752);
            Add_Hierarchical_Subject(thisBibInfo, record, 662);

            // Add all the subjects ( 600... 658, excluding 655 )
            Add_Personal_Name(thisBibInfo, record, 600, 4);
            Add_Corporate_Name(thisBibInfo, record, 610, 4);
            Add_Conference_Name(thisBibInfo, record, 611, 4);
            Add_Main_Title(thisBibInfo, record, 630, Title_Type_Enum.UNSPECIFIED, 1, 4);

            // Add all additional subjects
            // Letters indicate which fields are: TOPICAL, GEOGRAPHIC, TEMPORAL, GENRE, OCCUPATION
            Add_Subject(thisBibInfo, record, 648, "x", "z", "ay", "v", "");
            Add_Subject(thisBibInfo, record, 650, "ax", "z", "y", "v", "");
            Add_Subject(thisBibInfo, record, 651, "x", "az", "y", "v", "");
            Add_Subject(thisBibInfo, record, 653, "a", "", "", "", "");
            Add_Subject(thisBibInfo, record, 654, "av", "y", "z", "", "");
            Add_Subject(thisBibInfo, record, 655, "x", "z", "y", "av", "");
            Add_Subject(thisBibInfo, record, 656, "x", "z", "y", "v", "a");
            Add_Subject(thisBibInfo, record, 657, "ax", "z", "y", "v", "");
            Add_Subject(thisBibInfo, record, 690, "ax", "z", "y", "v", "");
            Add_Subject(thisBibInfo, record, 691, "x", "az", "y", "v", "");

            // Add the genres (655 -- again)
            foreach (MARC_Field thisRecord in record[655])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    if (thisRecord.has_Subfield('2'))
                        thisBibInfo.Add_Genre(thisRecord['a'], thisRecord['2']);
                    else
                        thisBibInfo.Add_Genre(thisRecord['a']);
                }
            }

            // Add the abbreviated title (210)
            foreach (MARC_Field thisRecord in record[210])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    Title_Info abbrTitle = new Title_Info(thisRecord['a'], Title_Type_Enum.Abbreviated);
                    if (thisRecord.has_Subfield('b'))
                        abbrTitle.Subtitle = thisRecord['b'];
                    thisBibInfo.Add_Other_Title(abbrTitle);
                }
            }

            // Add the title ( 245 |a, |b )
            Add_Main_Title(thisBibInfo, record, 245, Title_Type_Enum.UNSPECIFIED, 2, 1);

            // Add the translated titles ( 242 )
            Add_Main_Title(thisBibInfo, record, 242, Title_Type_Enum.Translated, 2, 2);

            // Add the alternative titles ( 246, 740 )
            Add_Main_Title(thisBibInfo, record, 246, Title_Type_Enum.Alternative, 0, 2);
            Add_Main_Title(thisBibInfo, record, 740, Title_Type_Enum.Alternative, 1, 2);

            // Add the uniform titles (130, 240, 730 )
            Add_Main_Title(thisBibInfo, record, 130, Title_Type_Enum.Uniform, 1, 2);
            Add_Main_Title(thisBibInfo, record, 240, Title_Type_Enum.Uniform, 2, 2);
            Add_Main_Title(thisBibInfo, record, 730, Title_Type_Enum.Uniform, 1, 2);

            // Add the series titles ( 440, 490 )
            Add_Main_Title(thisBibInfo, record, 440, Title_Type_Enum.UNSPECIFIED, 2, 3);
            Add_Main_Title(thisBibInfo, record, 490, Title_Type_Enum.UNSPECIFIED, 0, 3);

            // Add the creators and contributors ( 100, 110 , 111, 700, 710, 711, 720, 796, 797 )
            Add_Personal_Name(thisBibInfo, record, 100, 1);
            Add_Personal_Name(thisBibInfo, record, 700, 2);
            Add_Personal_Name(thisBibInfo, record, 796, 3);
            Add_Corporate_Name(thisBibInfo, record, 110, 1);
            Add_Corporate_Name(thisBibInfo, record, 710, 2);
            Add_Corporate_Name(thisBibInfo, record, 797, 3);
            Add_Conference_Name(thisBibInfo, record, 111, 1);
            Add_Conference_Name(thisBibInfo, record, 711, 2);

            // Add the Other Edition Value (775)
            foreach (MARC_Field thisRecord in record[775])
            {
                Related_Item_Info otherEditionItem = new Related_Item_Info();
                otherEditionItem.Relationship = Related_Item_Type_Enum.OtherVersion;
                if (thisRecord.has_Subfield('t'))
                    otherEditionItem.Main_Title.Title = thisRecord['t'];
                if (thisRecord.has_Subfield('x'))
                    otherEditionItem.Add_Identifier(thisRecord['x'], "issn");
                if (thisRecord.has_Subfield('z'))
                    otherEditionItem.Add_Identifier(thisRecord['z'], "isbn");
                if (thisRecord.has_Subfield('w'))
                {
                    string[] splitter = thisRecord['w'].Split("|".ToCharArray());
                    foreach (string thisSplitter in splitter)
                    {
                        if (thisSplitter.IndexOf("(DLC)sn") >= 0)
                        {
                            otherEditionItem.Add_Identifier(thisSplitter.Replace("(DLC)sn", "").Trim(), "lccn");
                        }
                        if (thisSplitter.IndexOf("(OCoLC)") >= 0)
                        {
                            otherEditionItem.Add_Identifier(thisSplitter.Replace("(OCoLC)", "").Trim(), "oclc");
                        }
                    }
                }
                thisBibInfo.Add_Related_Item(otherEditionItem);
            }

            // Add the Preceding Entry (780)
            foreach (MARC_Field thisRecord in record[780])
            {
                Related_Item_Info precedingItem = new Related_Item_Info();
                precedingItem.Relationship = Related_Item_Type_Enum.Preceding;
                if (thisRecord.has_Subfield('t'))
                    precedingItem.Main_Title.Title = thisRecord['t'];
                if (thisRecord.has_Subfield('x'))
                    precedingItem.Add_Identifier(thisRecord['x'], "issn");
                if (thisRecord.has_Subfield('z'))
                    precedingItem.Add_Identifier(thisRecord['z'], "isbn");
                if (thisRecord.has_Subfield('w'))
                {
                    string[] splitter = thisRecord['w'].Split("|".ToCharArray());
                    foreach (string thisSplitter in splitter)
                    {
                        if ((thisSplitter.IndexOf("(DLC)sn") >= 0) || (thisSplitter.IndexOf("(OCoLC)") >= 0))
                        {
                            if (thisSplitter.IndexOf("(DLC)sn") >= 0)
                            {
                                precedingItem.Add_Identifier(thisSplitter.Replace("(DLC)sn", "").Trim(), "lccn");
                            }
                            if (thisSplitter.IndexOf("(OCoLC)") >= 0)
                            {
                                precedingItem.Add_Identifier(thisSplitter.Replace("(OCoLC)", "").Trim(), "oclc");
                            }
                        }
                        else
                        {
                            precedingItem.Add_Identifier(thisSplitter.Trim(), String.Empty);
                        }
                    }
                    if (thisRecord.has_Subfield('o'))
                    {
                        if (thisRecord['o'].IndexOf("(SobekCM)") >= 0)
                            precedingItem.SobekCM_ID = thisRecord['o'].Replace("(SobekCM)", "").Trim();
                    }
                }
                thisBibInfo.Add_Related_Item(precedingItem);
            }

            // Add the Suceeding Entry (785)
            foreach (MARC_Field thisRecord in record[785])
            {
                Related_Item_Info succeedingItem = new Related_Item_Info();
                succeedingItem.Relationship = Related_Item_Type_Enum.Succeeding;
                if (thisRecord.has_Subfield('t'))
                    succeedingItem.Main_Title.Title = thisRecord['t'];
                if (thisRecord.has_Subfield('x'))
                    succeedingItem.Add_Identifier(thisRecord['x'], "issn");
                if (thisRecord.has_Subfield('z'))
                    succeedingItem.Add_Identifier(thisRecord['z'], "isbn");
                if (thisRecord.has_Subfield('w'))
                {
                    string[] splitter = thisRecord['w'].Split("|".ToCharArray());
                    foreach (string thisSplitter in splitter)
                    {
                        if ((thisSplitter.IndexOf("(DLC)sn") >= 0) || (thisSplitter.IndexOf("(OCoLC)") >= 0))
                        {
                            if (thisSplitter.IndexOf("(DLC)sn") >= 0)
                            {
                                succeedingItem.Add_Identifier(thisSplitter.Replace("(DLC)sn", "").Trim(), "lccn");
                            }
                            if (thisSplitter.IndexOf("(OCoLC)") >= 0)
                            {
                                succeedingItem.Add_Identifier(thisSplitter.Replace("(OCoLC)", "").Trim(), "oclc");
                            }
                        }
                        else
                        {
                            succeedingItem.Add_Identifier(thisSplitter.Trim(), String.Empty);
                        }
                    }
                }
                if (thisRecord.has_Subfield('o'))
                {
                    if (thisRecord['o'].IndexOf("(SobekCM)") >= 0)
                        succeedingItem.SobekCM_ID = thisRecord['o'].Replace("(SobekCM)", "").Trim();
                }
                thisBibInfo.Add_Related_Item(succeedingItem);
            }

            // Add the Other Relationship Entry (787)
            foreach (MARC_Field thisRecord in record[787])
            {
                Related_Item_Info otherRelationItem = new Related_Item_Info();
                otherRelationItem.Relationship = Related_Item_Type_Enum.UNKNOWN;
                if (thisRecord.has_Subfield('t'))
                    otherRelationItem.Main_Title.Title = thisRecord['t'];
                if (thisRecord.has_Subfield('x'))
                    otherRelationItem.Add_Identifier(thisRecord['x'], "issn");
                if (thisRecord.has_Subfield('z'))
                    otherRelationItem.Add_Identifier(thisRecord['z'], "isbn");
                if (thisRecord.has_Subfield('w'))
                {
                    string[] splitter = thisRecord['w'].Split("|".ToCharArray());
                    foreach (string thisSplitter in splitter)
                    {
                        if ((thisSplitter.IndexOf("(DLC)sn") >= 0) || (thisSplitter.IndexOf("(OCoLC)") >= 0))
                        {
                            if (thisSplitter.IndexOf("(DLC)sn") >= 0)
                            {
                                otherRelationItem.Add_Identifier(thisSplitter.Replace("(DLC)sn", "").Trim(), "lccn");
                            }
                            if (thisSplitter.IndexOf("(OCoLC)") >= 0)
                            {
                                otherRelationItem.Add_Identifier(thisSplitter.Replace("(OCoLC)", "").Trim(), "oclc");
                            }
                        }
                        else
                        {
                            otherRelationItem.Add_Identifier(thisSplitter.Trim(), String.Empty);
                        }
                    }
                }
                if (thisRecord.has_Subfield('o'))
                {
                    if (thisRecord['o'].IndexOf("(SobekCM)") >= 0)
                        otherRelationItem.SobekCM_ID = thisRecord['o'].Replace("(SobekCM)", "").Trim();
                }
                thisBibInfo.Add_Related_Item(otherRelationItem);
            }

            // Get the type of resource ( Leader/006, Leader/007, Serial 008/021 )
            string marc_type = String.Empty;
            switch (record.Leader[6])
            {
                case 'a':
                case 't':
                    thisBibInfo.SobekCM_Type = TypeOfResource_SobekCM_Enum.Book;
                    marc_type = "BKS";
                    break;

                case 'e':
                case 'f':
                    thisBibInfo.SobekCM_Type = TypeOfResource_SobekCM_Enum.Map;
                    marc_type = "MAP";
                    break;

                case 'c':
                case 'd':
                    thisBibInfo.SobekCM_Type = TypeOfResource_SobekCM_Enum.Book;
                    marc_type = "BKS";
                    break;

                case 'i':
                case 'j':
                    thisBibInfo.SobekCM_Type = TypeOfResource_SobekCM_Enum.Audio;
                    marc_type = "REC";
                    break;

                case 'k':
                    thisBibInfo.SobekCM_Type = TypeOfResource_SobekCM_Enum.Photograph;
                    marc_type = "VIS";
                    break;

                case 'g':
                    thisBibInfo.SobekCM_Type = TypeOfResource_SobekCM_Enum.Video;
                    marc_type = "VIS";
                    break;

                case 'r':
                    thisBibInfo.SobekCM_Type = TypeOfResource_SobekCM_Enum.Artifact;
                    marc_type = "VIS";
                    break;

                case 'm':
                    thisBibInfo.SobekCM_Type = TypeOfResource_SobekCM_Enum.Archival;
                    marc_type = "COM";
                    break;

                case 'p':
                    thisBibInfo.SobekCM_Type = TypeOfResource_SobekCM_Enum.Archival;
                    marc_type = "MIX";
                    break;

                case 'o':
                    marc_type = "VIS";
                    thisBibInfo.SobekCM_Type = TypeOfResource_SobekCM_Enum.Archival;
                    break;
            }
            if (record.Leader[7] == 'c')
                thisBibInfo.Type.Collection = true;
            if (record.Leader[7] == 's')
            {
                thisBibInfo.SobekCM_Type = TypeOfResource_SobekCM_Enum.Serial;

                if (field_008.Length > 22)
                {
                    if (field_008[21] == 'n')
                        thisBibInfo.SobekCM_Type = TypeOfResource_SobekCM_Enum.Newspaper;
                }
                marc_type = "CNR";
            }
            thisBibInfo.EncodingLevel = record.Leader[17].ToString().Replace("^", "#").Replace(" ", "#");

            if (field_008.Length > 35)
            {
                if ((marc_type == "BKS") || (marc_type == "CNR") || (marc_type == "MAP") || (marc_type == "COM") || (marc_type == "VIS"))
                {
                    switch (field_008[28])
                    {
                        case 'c':
                            thisBibInfo.Add_Genre("multilocal government publication", "marcgt");
                            break;

                        case 'f':
                            thisBibInfo.Add_Genre("federal government publication", "marcgt");
                            break;

                        case 'i':
                            thisBibInfo.Add_Genre("international intergovernmental publication", "marcgt");
                            break;

                        case 'l':
                            thisBibInfo.Add_Genre("local government publication", "marcgt");
                            break;

                        case 'm':
                            thisBibInfo.Add_Genre("multistate government publication", "marcgt");
                            break;

                        case 'o':
                            thisBibInfo.Add_Genre("government publication", "marcgt");
                            break;

                        case 's':
                            thisBibInfo.Add_Genre("government publication (state, provincial, terriorial, dependent)", "marcgt");
                            break;

                        case 'a':
                            thisBibInfo.Add_Genre("government publication (autonomous or semiautonomous component)", "marcgt");
                            break;
                    }
                }

                if ((marc_type == "BKS") || (marc_type == "CNR"))
                {
                    string nature_of_contents = field_008.Substring(24, 4);
                    if (nature_of_contents.IndexOf("a") >= 0)
                        thisBibInfo.Add_Genre("abstract or summary", "marcgt");
                    if (nature_of_contents.IndexOf("b") >= 0)
                        thisBibInfo.Add_Genre("bibliography", "marcgt");
                    if (nature_of_contents.IndexOf("c") >= 0)
                        thisBibInfo.Add_Genre("catalog", "marcgt");
                    if (nature_of_contents.IndexOf("d") >= 0)
                        thisBibInfo.Add_Genre("dictionary", "marcgt");
                    if (nature_of_contents.IndexOf("r") >= 0)
                        thisBibInfo.Add_Genre("directory", "marcgt");
                    if (nature_of_contents.IndexOf("k") >= 0)
                        thisBibInfo.Add_Genre("discography", "marcgt");
                    if (nature_of_contents.IndexOf("e") >= 0)
                        thisBibInfo.Add_Genre("encyclopedia", "marcgt");
                    if (nature_of_contents.IndexOf("q") >= 0)
                        thisBibInfo.Add_Genre("filmography", "marcgt");
                    if (nature_of_contents.IndexOf("f") >= 0)
                        thisBibInfo.Add_Genre("handbook", "marcgt");
                    if (nature_of_contents.IndexOf("i") >= 0)
                        thisBibInfo.Add_Genre("index", "marcgt");
                    if (nature_of_contents.IndexOf("w") >= 0)
                        thisBibInfo.Add_Genre("law report or digest", "marcgt");
                    if (nature_of_contents.IndexOf("g") >= 0)
                        thisBibInfo.Add_Genre("legal article", "marcgt");
                    if (nature_of_contents.IndexOf("v") >= 0)
                        thisBibInfo.Add_Genre("legal case and case notes", "marcgt");
                    if (nature_of_contents.IndexOf("l") >= 0)
                        thisBibInfo.Add_Genre("legislation", "marcgt");
                    if (nature_of_contents.IndexOf("j") >= 0)
                        thisBibInfo.Add_Genre("patent", "marcgt");
                    if (nature_of_contents.IndexOf("p") >= 0)
                        thisBibInfo.Add_Genre("programmed text", "marcgt");
                    if (nature_of_contents.IndexOf("o") >= 0)
                        thisBibInfo.Add_Genre("review", "marcgt");
                    if (nature_of_contents.IndexOf("s") >= 0)
                        thisBibInfo.Add_Genre("statistics", "marcgt");
                    if (nature_of_contents.IndexOf("n") >= 0)
                        thisBibInfo.Add_Genre("survey of literature", "marcgt");
                    if (nature_of_contents.IndexOf("t") >= 0)
                        thisBibInfo.Add_Genre("technical report", "marcgt");
                    if (nature_of_contents.IndexOf("m") >= 0)
                        thisBibInfo.Add_Genre("theses", "marcgt");
                    if (nature_of_contents.IndexOf("z") >= 0)
                        thisBibInfo.Add_Genre("treaty", "marcgt");
                    if (nature_of_contents.IndexOf("2") >= 0)
                        thisBibInfo.Add_Genre("offprint", "marcgt");
                    if (nature_of_contents.IndexOf("y") >= 0)
                        thisBibInfo.Add_Genre("yearbook", "marcgt");
                    if (nature_of_contents.IndexOf("5") >= 0)
                        thisBibInfo.Add_Genre("calendar", "marcgt");
                    if (nature_of_contents.IndexOf("6") >= 0)
                        thisBibInfo.Add_Genre("comic/graphic novel", "marcgt");

                    if (field_008[29] == '1')
                        thisBibInfo.Add_Genre("conference publication", "marcgt");
                }

                if (marc_type == "CNR")
                {
                    if (field_008[21] == 'd')
                        thisBibInfo.Add_Genre("database", "marcgt");
                    if (field_008[21] == 'l')
                        thisBibInfo.Add_Genre("loose-leaf", "marcgt");
                    if (field_008[21] == 'n')
                        thisBibInfo.Add_Genre("newspaper", "marcgt");
                    if (field_008[21] == 'p')
                        thisBibInfo.Add_Genre("periodical", "marcgt");
                    if (field_008[21] == 's')
                        thisBibInfo.Add_Genre("series", "marcgt");
                    if (field_008[21] == 'w')
                        thisBibInfo.Add_Genre("web site", "marcgt");

                    // Get the frequency
                    switch (field_008[18])
                    {
                        case 'a':
                            thisBibInfo.Origin_Info.Add_Frequency("annual", "marcfrequency");
                            break;

                        case 'b':
                            thisBibInfo.Origin_Info.Add_Frequency("bimonthly", "marcfrequency");
                            break;

                        case 'c':
                            thisBibInfo.Origin_Info.Add_Frequency("semiweekly", "marcfrequency");
                            break;

                        case 'd':
                            thisBibInfo.Origin_Info.Add_Frequency("daily", "marcfrequency");
                            break;

                        case 'e':
                            thisBibInfo.Origin_Info.Add_Frequency("biweekly", "marcfrequency");
                            break;

                        case 'f':
                            thisBibInfo.Origin_Info.Add_Frequency("semiannual", "marcfrequency");
                            break;

                        case 'g':
                            thisBibInfo.Origin_Info.Add_Frequency("biennial", "marcfrequency");
                            break;

                        case 'h':
                            thisBibInfo.Origin_Info.Add_Frequency("triennial", "marcfrequency");
                            break;

                        case 'i':
                            thisBibInfo.Origin_Info.Add_Frequency("three times a week", "marcfrequency");
                            break;

                        case 'j':
                            thisBibInfo.Origin_Info.Add_Frequency("three times a month", "marcfrequency");
                            break;

                        case 'k':
                            thisBibInfo.Origin_Info.Add_Frequency("continuously updated", "marcfrequency");
                            break;

                        case 'm':
                            thisBibInfo.Origin_Info.Add_Frequency("monthly", "marcfrequency");
                            break;

                        case 'q':
                            thisBibInfo.Origin_Info.Add_Frequency("quarterly", "marcfrequency");
                            break;

                        case 's':
                            thisBibInfo.Origin_Info.Add_Frequency("semimonthly", "marcfrequency");
                            break;

                        case 't':
                            thisBibInfo.Origin_Info.Add_Frequency("three times a year", "marcfrequency");
                            break;

                        case 'w':
                            thisBibInfo.Origin_Info.Add_Frequency("weekly", "marcfrequency");
                            break;

                        case 'z':
                            thisBibInfo.Origin_Info.Add_Frequency("other", "marcfrequency");
                            break;
                    }

                    // Get the regularity
                    switch (field_008[19])
                    {
                        case 'n':
                            thisBibInfo.Origin_Info.Add_Frequency("normalized irregular", "marcfrequency");
                            break;

                        case 'r':
                            thisBibInfo.Origin_Info.Add_Frequency("regular", "marcfrequency");
                            break;

                        case 'x':
                            thisBibInfo.Origin_Info.Add_Frequency("completely irregular", "marcfrequency");
                            break;
                    }
                }

                if (marc_type == "MAP")
                {
                    // Get the form of item
                    if (field_008[25] == 'e')
                        thisBibInfo.Add_Genre("atlas", "marcgt");
                    if (field_008[25] == 'd')
                        thisBibInfo.Add_Genre("globe", "marcgt");
                    if (field_008[25] == 'a')
                        thisBibInfo.Add_Genre("single map", "marcgt");
                    if (field_008[25] == 'b')
                        thisBibInfo.Add_Genre("map series", "marcgt");
                    if (field_008[25] == 'c')
                        thisBibInfo.Add_Genre("map serial", "marcgt");

                    // Get the projection, if there is one
                    if ((field_008.Substring(22, 2) != "  ") && (field_008.Substring(22, 2) != "||") && (field_008.Substring(22, 2) != "^^") && (field_008.Substring(22, 2) != "||"))
                    {
                        Subject_Info_Cartographics cartographicsSubject = new Subject_Info_Cartographics();
                        cartographicsSubject.ID = "SUBJ008";
                        cartographicsSubject.Projection = field_008.Substring(22, 2);
                        thisBibInfo.Add_Subject(cartographicsSubject);
                    }

                    // Get whether this is indexed
                    if (field_008[31] == '1')
                    {
                        thisBibInfo.Add_Genre("indexed", "marcgt");
                    }
                }

                if (marc_type == "REC")
                {
                    string nature_of_recording = field_008.Substring(30, 2);
                    if (nature_of_recording.IndexOf("a") >= 0)
                        thisBibInfo.Add_Genre("autobiography", "marcgt");
                    if (nature_of_recording.IndexOf("b") >= 0)
                        thisBibInfo.Add_Genre("biography", "marcgt");
                    if (nature_of_recording.IndexOf("c") >= 0)
                        thisBibInfo.Add_Genre("conference publication", "marcgt");
                    if (nature_of_recording.IndexOf("d") >= 0)
                        thisBibInfo.Add_Genre("drama", "marcgt");
                    if (nature_of_recording.IndexOf("e") >= 0)
                        thisBibInfo.Add_Genre("essay", "marcgt");
                    if (nature_of_recording.IndexOf("f") >= 0)
                        thisBibInfo.Add_Genre("fiction", "marcgt");
                    if (nature_of_recording.IndexOf("o") >= 0)
                        thisBibInfo.Add_Genre("folktale", "marcgt");
                    if (nature_of_recording.IndexOf("k") >= 0)
                        thisBibInfo.Add_Genre("humor, satire", "marcgt");
                    if (nature_of_recording.IndexOf("i") >= 0)
                        thisBibInfo.Add_Genre("instruction", "marcgt");
                    if (nature_of_recording.IndexOf("t") >= 0)
                        thisBibInfo.Add_Genre("interview", "marcgt");
                    if (nature_of_recording.IndexOf("j") >= 0)
                        thisBibInfo.Add_Genre("language instruction", "marcgt");
                    if (nature_of_recording.IndexOf("m") >= 0)
                        thisBibInfo.Add_Genre("memoir", "marcgt");
                    if (nature_of_recording.IndexOf("p") >= 0)
                        thisBibInfo.Add_Genre("poetry", "marcgt");
                    if (nature_of_recording.IndexOf("r") >= 0)
                        thisBibInfo.Add_Genre("rehearsal", "marcgt");
                    if (nature_of_recording.IndexOf("g") >= 0)
                        thisBibInfo.Add_Genre("reporting", "marcgt");
                    if (nature_of_recording.IndexOf("s") >= 0)
                        thisBibInfo.Add_Genre("sound", "marcgt");
                    if (nature_of_recording.IndexOf("l") >= 0)
                        thisBibInfo.Add_Genre("speech", "marcgt");
                }

                if (marc_type == "COM")
                {
                    switch (field_008[26])
                    {
                        case 'e':
                            thisBibInfo.Add_Genre("database", "marcgt");
                            break;

                        case 'f':
                            thisBibInfo.Add_Genre("font", "marcgt");
                            break;

                        case 'g':
                            thisBibInfo.Add_Genre("game", "marcgt");
                            break;

                        case 'a':
                            thisBibInfo.Add_Genre("numeric data", "marcgt");
                            break;

                        case 'h':
                            thisBibInfo.Add_Genre("sound", "marcgt");
                            break;
                    }
                }

                if (marc_type == "VIS")
                {
                    switch (field_008[33])
                    {
                        case 'a':
                            thisBibInfo.Add_Genre("art original", "marcgt");
                            break;

                        case 'c':
                            thisBibInfo.Add_Genre("art reproduction", "marcgt");
                            break;

                        case 'n':
                            thisBibInfo.Add_Genre("chart", "marcgt");
                            break;

                        case 'd':
                            thisBibInfo.Add_Genre("diorama", "marcgt");
                            break;

                        case 'f':
                            thisBibInfo.Add_Genre("filmstrip", "marcgt");
                            break;

                        case 'o':
                            thisBibInfo.Add_Genre("flash card", "marcgt");
                            break;

                        case 'k':
                            thisBibInfo.Add_Genre("graphic", "marcgt");
                            break;

                        case 'b':
                            thisBibInfo.Add_Genre("kit", "marcgt");
                            break;

                        case 'p':
                            thisBibInfo.Add_Genre("microscope slide", "marcgt");
                            break;

                        case 'q':
                            thisBibInfo.Add_Genre("model", "marcgt");
                            break;

                        case 'm':
                            thisBibInfo.Add_Genre("motion picture", "marcgt");
                            break;

                        case 'i':
                            thisBibInfo.Add_Genre("picture", "marcgt");
                            break;

                        case 'r':
                            thisBibInfo.Add_Genre("realia", "marcgt");
                            break;

                        case 's':
                            thisBibInfo.Add_Genre("slide", "marcgt");
                            break;

                        case 'l':
                            thisBibInfo.Add_Genre("technical drawing", "marcgt");
                            break;

                        case 'w':
                            thisBibInfo.Add_Genre("toy", "marcgt");
                            break;

                        case 't':
                            thisBibInfo.Add_Genre("transparency", "marcgt");
                            break;

                        case 'v':
                            thisBibInfo.Add_Genre("video recording", "marcgt");
                            break;
                    }
                }

                if (marc_type == "BKS")
                {
                    switch (field_008[34])
                    {
                        case 'a':
                            thisBibInfo.Add_Genre("autobiography", "marcgt");
                            break;

                        case 'b':
                            thisBibInfo.Add_Genre("individual biography", "marcgt");
                            break;

                        case 'c':
                            thisBibInfo.Add_Genre("collective biography", "marcgt");
                            break;
                    }

                    switch (field_008[33])
                    {
                        case 'a':
                            thisBibInfo.Add_Genre("comic strip", "marcgt");
                            break;

                        case 'd':
                            thisBibInfo.Add_Genre("drama", "marcgt");
                            break;

                        case 'e':
                            thisBibInfo.Add_Genre("essay", "marcgt");
                            break;

                        case 'h':
                            thisBibInfo.Add_Genre("humor, satire", "marcgt");
                            break;

                        case 'i':
                            thisBibInfo.Add_Genre("letter", "marcgt");
                            break;

                        case 'p':
                            thisBibInfo.Add_Genre("poetry", "marcgt");
                            break;

                        case 'f':
                            thisBibInfo.Add_Genre("novel", "marcgt");
                            break;

                        case 'j':
                            thisBibInfo.Add_Genre("short story", "marcgt");
                            break;

                        case 's':
                            thisBibInfo.Add_Genre("speech", "marcgt");
                            break;

                        case '0':
                            thisBibInfo.Add_Genre("non-fiction", "marcgt");
                            break;

                        case '1':
                            thisBibInfo.Add_Genre("fiction", "marcgt");
                            break;
                    }

                    if ((field_008[30] == 'h') || (field_008[31] == 'h'))
                    {
                        thisBibInfo.Add_Genre("history", "marcgt");
                    }

                    if (field_008[30] == '1')
                    {
                        thisBibInfo.Add_Genre("festschrift", "marcgt");
                    }
                }
            }

            // Look for target audience (521)
            foreach (MARC_Field thisRecord in record[521])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    if (thisRecord.has_Subfield('b'))
                    {
                        thisBibInfo.Add_Target_Audience(thisRecord['a'].Replace("[", "(").Replace("]", ")") + " [ " + thisRecord['b'].Replace("[", "(").Replace("]", ")") + " ]");
                    }
                    else
                    {
                        thisBibInfo.Add_Target_Audience(thisRecord['a'].Replace("[", "(").Replace("]", ")"));
                    }
                }
            }

            // Look for target audince (008/22)
            if ((marc_type == "BKS") || (marc_type == "COM") || (marc_type == "REC") || (marc_type == "SCO") || (marc_type == "VIS"))
            {
                if (field_008.Length > 22)
                {
                    switch (field_008[22])
                    {
                        case 'd':
                            thisBibInfo.Add_Target_Audience("adolescent", "marctarget");
                            break;

                        case 'e':
                            thisBibInfo.Add_Target_Audience("adult", "marctarget");
                            break;

                        case 'g':
                            thisBibInfo.Add_Target_Audience("general", "marctarget");
                            break;

                        case 'b':
                            thisBibInfo.Add_Target_Audience("primary", "marctarget");
                            break;

                        case 'c':
                            thisBibInfo.Add_Target_Audience("pre-adolescent", "marctarget");
                            break;

                        case 'j':
                            thisBibInfo.Add_Target_Audience("juvenile", "marctarget");
                            break;

                        case 'a':
                            thisBibInfo.Add_Target_Audience("preschool", "marctarget");
                            break;

                        case 'f':
                            thisBibInfo.Add_Target_Audience("specialized", "marctarget");
                            break;
                    }
                }
            }

            // Get any project codes ( 852 )
            if ((package != null) && (package.Behaviors.Aggregation_Count == 0))
            {
                foreach (MARC_Field thisRecord in record[852])
                {
                    if ((thisRecord.Indicators.Trim().Length == 0) && (thisRecord.has_Subfield('b')))
                    {
                        string allCodes = thisRecord['b'];
                        string[] splitAllCodes = allCodes.Split("|;".ToCharArray());
                        foreach (string splitCode in splitAllCodes)
                        {
                            package.Behaviors.Add_Aggregation(splitCode.Trim());
                        }
                    }
                }
            }
        }

        private static string Remove_Trailing_Punctuation(string source)
        {
            if (source.Length < 2)
                return source;

            if ((source[source.Length - 1] == '.') || (source[source.Length - 1] == ',') || (source[source.Length - 1] == ':') || (source[source.Length - 1] == '\\') || (source[source.Length - 1] == '/'))
            {
                return source.Substring(0, source.Length - 1).Trim();
            }

            return source;
        }

        private static void Add_Hierarchical_Subject(Bibliographic_Info thisBibInfo, MARC_Record record, int tag)
        {
            int subj_index = 1;

            foreach (MARC_Field thisRecord in record[tag])
            {
                Subject_Info_HierarchicalGeographic spatial = new Subject_Info_HierarchicalGeographic();

                // Look for 'a' first
                if (thisRecord.has_Subfield('a'))
                {
                    spatial.Country = Remove_Trailing_Punctuation(thisRecord['a']);
                }

                // Look for 'b' next
                if (thisRecord.has_Subfield('b'))
                {
                    spatial.State = Remove_Trailing_Punctuation(thisRecord['b']);
                }

                // Look for 'c' next
                if (thisRecord.has_Subfield('c'))
                {
                    spatial.County = Remove_Trailing_Punctuation(thisRecord['c']);
                }

                // Look for 'd' next
                if (thisRecord.has_Subfield('d'))
                {
                    spatial.City = Remove_Trailing_Punctuation(thisRecord['d']);
                }

                // Look for area
                if (thisRecord.has_Subfield('f'))
                {
                    spatial.Area = Remove_Trailing_Punctuation(thisRecord['f']);
                }

                // Look for authority
                if (thisRecord.has_Subfield('2'))
                {
                    spatial.Authority = thisRecord['2'];
                }

                // Now, add to the object
                spatial.ID = "SUBJ" + tag + "_" + subj_index;
                subj_index++;
                thisBibInfo.Add_Subject(spatial);
            }
        }

        private static void Add_Main_Title(Bibliographic_Info thisBibInfo, MARC_Record record, int tag, Title_Type_Enum type, int non_filling_type, int title_type)
        {
            // Step through each instance of this tag                    
            foreach (MARC_Field thisRecord in record[tag])
            {
                // Declare new title
                Title_Info newTitle = new Title_Info();
                newTitle.Title_Type = type;

                switch (non_filling_type)
                {
                    case 0:
                        newTitle.Title = Remove_Trailing_Punctuation(thisRecord['a']);
                        break;

                    case 1:
                        int non_filling_chars1 = 0;
                        try
                        {
                            non_filling_chars1 = Convert.ToInt16(thisRecord.Indicator1) - 48;
                        }
                        catch
                        {
                        }

                        if (non_filling_chars1 == 0)
                        {
                            newTitle.Title = Remove_Trailing_Punctuation(thisRecord['a']);
                        }
                        else
                        {
                            string complete_title = thisRecord['a'];
                            newTitle.NonSort = complete_title.Substring(0, non_filling_chars1);
                            newTitle.Title = Remove_Trailing_Punctuation(complete_title.Substring(non_filling_chars1));
                        }
                        break;

                    case 2:
                        int non_filling_chars2 = 0;
                        try
                        {
                            non_filling_chars2 = Convert.ToInt16(thisRecord.Indicator2) - 48;
                        }
                        catch
                        {
                        }

                        if (non_filling_chars2 == 0)
                        {
                            newTitle.Title = Remove_Trailing_Punctuation(thisRecord['a']);
                        }
                        else
                        {
                            string complete_title = thisRecord['a'];
                            newTitle.NonSort = complete_title.Substring(0, non_filling_chars2);
                            newTitle.Title = Remove_Trailing_Punctuation(complete_title.Substring(non_filling_chars2));
                        }
                        break;
                }

                newTitle.Title = newTitle.Title.Replace("âE", "É");

                if (thisRecord.has_Subfield('b'))
                    newTitle.Subtitle = Remove_Trailing_Punctuation(thisRecord['b'].Replace("/", ""));
                if (thisRecord.has_Subfield('n'))
                    newTitle.Add_Part_Number(thisRecord['n']);
                if (thisRecord.has_Subfield('p'))
                    newTitle.Add_Part_Name(thisRecord['p']);
                if (thisRecord.has_Subfield('y'))
                    newTitle.Language = thisRecord['y'];
                if (tag >= 700)
                    newTitle.Display_Label = "Uncontrolled";
                if (tag < 200)
                    newTitle.Display_Label = "Main Entry";
                if (tag == 246)
                {
                    switch (thisRecord.Indicator2)
                    {
                        case '0':
                            newTitle.Display_Label = "Portion of title";
                            break;

                        case '1':
                            newTitle.Display_Label = "Parallel title";
                            break;

                        case '2':
                            newTitle.Display_Label = "Distinctive title";
                            break;

                        case '3':
                            newTitle.Display_Label = "Other title";
                            break;

                        case '4':
                            newTitle.Display_Label = "Cover title";
                            break;

                        case '5':
                            newTitle.Display_Label = "Added title page title";
                            break;

                        case '6':
                            newTitle.Display_Label = "Caption title";
                            break;

                        case '7':
                            newTitle.Display_Label = "Running title";
                            break;

                        case '8':
                            newTitle.Display_Label = "Spine title";
                            break;

                        default:
                            newTitle.Display_Label = "Alternate title";
                            break;
                    }
                }
                if (thisRecord.has_Subfield('i'))
                    newTitle.Display_Label = thisRecord['i'].Replace(":", "");

                switch (title_type)
                {
                    case 1:
                        thisBibInfo.Main_Title = newTitle;
                        break;

                    case 2:
                        thisBibInfo.Add_Other_Title(newTitle);
                        break;

                    case 3:
                        thisBibInfo.SeriesTitle = newTitle;
                        break;

                    case 4:
                        Subject_Info_TitleInfo newTitleSubj = new Subject_Info_TitleInfo();
                        newTitleSubj.Set_Internal_Title(newTitle);
                        if (thisRecord.has_Subfield('v'))
                            newTitleSubj.Add_Genre(Remove_Trailing_Punctuation(thisRecord['v']));
                        if (thisRecord.has_Subfield('x'))
                            newTitleSubj.Add_Topic(Remove_Trailing_Punctuation(thisRecord['x']));
                        if (thisRecord.has_Subfield('y'))
                            newTitleSubj.Add_Temporal(Remove_Trailing_Punctuation(thisRecord['y']));
                        if (thisRecord.has_Subfield('z'))
                            newTitleSubj.Add_Geographic(Remove_Trailing_Punctuation(thisRecord['z']));
                        if (thisRecord.has_Subfield('2'))
                            newTitleSubj.Authority = thisRecord['2'];
                        switch (thisRecord.Indicator2)
                        {
                            case '0':
                                newTitleSubj.Authority = "lcsh";
                                break;

                            case '1':
                                newTitleSubj.Authority = "lcshac";
                                break;

                            case '2':
                                newTitleSubj.Authority = "mesh";
                                break;

                            case '3':
                                newTitleSubj.Authority = "nal";
                                break;

                            case '5':
                                newTitleSubj.Authority = "csh";
                                break;

                            case '6':
                                newTitleSubj.Authority = "rvm";
                                break;
                        }
                        thisBibInfo.Add_Subject(newTitleSubj);
                        break;
                }
            }
        }

        private static void Add_Personal_Name(Bibliographic_Info thisBibInfo, MARC_Record record, int tag, int name_type)
        {
            // Step through each instance of this tag                    
            foreach (MARC_Field thisRecord in record[tag])
            {
                // Create the name object
                Name_Info newName = new Name_Info();
                newName.Name_Type = Name_Info_Type_Enum.Personal;

                // Only continue if there is an id in this record
                if ((thisRecord.has_Subfield('a')) && (thisRecord['a'].ToUpper().IndexOf("PALMM") < 0))
                {
                    // Save the 'a' value
                    switch (thisRecord.Indicator1)
                    {
                        case '0':
                            newName.Given_Name = Remove_Trailing_Punctuation(thisRecord['a']);
                            newName.Full_Name = newName.Given_Name;
                            break;

                        case '1':
                            string tempName = Remove_Trailing_Punctuation(thisRecord['a']);
                            int tempCommaIndex = tempName.IndexOf(",");
                            if (tempCommaIndex > 0)
                            {
                                newName.Family_Name = tempName.Substring(0, tempCommaIndex).Trim();
                                newName.Given_Name = tempName.Substring(tempCommaIndex + 1).Trim();
                                newName.Full_Name = tempName;
                            }
                            else
                            {
                                newName.Family_Name = tempName;
                            }
                            break;

                        case '3':
                            newName.Family_Name = Remove_Trailing_Punctuation(thisRecord['a']);
                            newName.Full_Name = newName.Family_Name;
                            break;

                        default:
                            newName.Full_Name = Remove_Trailing_Punctuation(thisRecord['a']);
                            break;
                    }

                    if (thisRecord.has_Subfield('b'))
                        newName.Terms_Of_Address = thisRecord['b'];
                    if (thisRecord.has_Subfield('c'))
                    {
                        if (newName.Terms_Of_Address.Length > 0)
                        {
                            newName.Terms_Of_Address = newName.Terms_Of_Address + "; " + thisRecord['c'];
                        }
                        else
                        {
                            newName.Terms_Of_Address = thisRecord['c'];
                        }
                    }
                    if (thisRecord.has_Subfield('d'))
                        newName.Dates = Remove_Trailing_Punctuation(thisRecord['d']);
                    if (thisRecord.has_Subfield('e'))
                        newName.Add_Role(Remove_Trailing_Punctuation(thisRecord['e']));
                    if (thisRecord.has_Subfield('g'))
                        newName.Description = Remove_Trailing_Punctuation(thisRecord['g']);
                    if (thisRecord.has_Subfield('j'))
                    {
                        if (newName.Description.Length > 0)
                        {
                            newName.Description = newName.Description + "; " + thisRecord['j'];
                        }
                        else
                        {
                            newName.Description = thisRecord['j'];
                        }
                    }
                    if (thisRecord.has_Subfield('u'))
                        newName.Affiliation = Remove_Trailing_Punctuation(thisRecord['u']);
                    if (thisRecord.has_Subfield('q'))
                        newName.Display_Form = Remove_Trailing_Punctuation(thisRecord['q'].Replace("(", "").Replace(")", ""));

                    // Is there a relator code?
                    if (thisRecord.has_Subfield('4'))
                    {
                        // Get the relator code
                        string completeRelatorcode = thisRecord['4'];
                        string[] relatorCodesSplitter = completeRelatorcode.Split("|".ToCharArray());
                        foreach (string relatorcode in relatorCodesSplitter)
                        {
                            newName.Add_Role(relatorcode, "marcrelator", Name_Info_Role_Type_Enum.Code);
                        }
                    }

                    switch (name_type)
                    {
                        case 1:
                            thisBibInfo.Main_Entity_Name = newName;
                            break;

                        case 2:
                            thisBibInfo.Add_Named_Entity(newName);
                            break;

                        case 3:
                            thisBibInfo.Donor = newName;
                            break;

                        case 4:
                            Subject_Info_Name newNameSubj = new Subject_Info_Name();
                            newNameSubj.Set_Internal_Name(newName);
                            if (thisRecord.has_Subfield('v'))
                                newNameSubj.Add_Genre(Remove_Trailing_Punctuation(thisRecord['v']));
                            if (thisRecord.has_Subfield('x'))
                                newNameSubj.Add_Topic(Remove_Trailing_Punctuation(thisRecord['x']));
                            if (thisRecord.has_Subfield('y'))
                                newNameSubj.Add_Temporal(Remove_Trailing_Punctuation(thisRecord['y']));
                            if (thisRecord.has_Subfield('z'))
                                newNameSubj.Add_Geographic(Remove_Trailing_Punctuation(thisRecord['z']));
                            if (thisRecord.has_Subfield('2'))
                                newNameSubj.Authority = thisRecord['2'];
                            switch (thisRecord.Indicator2)
                            {
                                case '0':
                                    newNameSubj.Authority = "lcsh";
                                    break;

                                case '1':
                                    newNameSubj.Authority = "lcshac";
                                    break;

                                case '2':
                                    newNameSubj.Authority = "mesh";
                                    break;

                                case '3':
                                    newNameSubj.Authority = "nal";
                                    break;

                                case '5':
                                    newNameSubj.Authority = "csh";
                                    break;

                                case '6':
                                    newNameSubj.Authority = "rvm";
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        private static void Add_Corporate_Name(Bibliographic_Info thisBibInfo, MARC_Record record, int tag, int name_type)
        {
            // Step through each instance of this tag                    
            foreach (MARC_Field thisRecord in record[tag])
            {
                if ((name_type != 3) || (thisRecord.Indicator2 == '3'))
                {
                    // Create the name object
                    Name_Info newName = new Name_Info();
                    newName.Name_Type = Name_Info_Type_Enum.Corporate;

                    // Only continue if there is an id in this record
                    if ((thisRecord.has_Subfield('a')) && (thisRecord['a'].ToUpper().IndexOf("PALMM") < 0))
                    {
                        newName.Full_Name = Remove_Trailing_Punctuation(thisRecord['a']);
                        if (thisRecord.has_Subfield('b'))
                        {
                            newName.Full_Name = newName.Full_Name + " -- " + Remove_Trailing_Punctuation(thisRecord['b']);
                        }
                        if (thisRecord.has_Subfield('c'))
                            newName.Description = thisRecord['c'];
                        if (thisRecord.has_Subfield('d'))
                            newName.Dates = Remove_Trailing_Punctuation(thisRecord['d']);
                        if (thisRecord.has_Subfield('e'))
                            newName.Add_Role(Remove_Trailing_Punctuation(thisRecord['e']));
                        if (thisRecord.has_Subfield('u'))
                            newName.Affiliation = Remove_Trailing_Punctuation(thisRecord['u']);

                        // Is there a relator code?
                        if (thisRecord.has_Subfield('4'))
                        {
                            // Get the relator code
                            string relatorcode = thisRecord['4'];
                            newName.Add_Role(relatorcode, "marcrelator", Name_Info_Role_Type_Enum.Code);
                        }

                        switch (name_type)
                        {
                            case 1:
                                thisBibInfo.Main_Entity_Name = newName;
                                break;

                            case 2:
                                thisBibInfo.Add_Named_Entity(newName);
                                break;

                            case 3:
                                thisBibInfo.Donor = newName;
                                break;

                            case 4:
                                Subject_Info_Name newNameSubj = new Subject_Info_Name();
                                newNameSubj.Set_Internal_Name(newName);
                                if (thisRecord.has_Subfield('v'))
                                    newNameSubj.Add_Genre(Remove_Trailing_Punctuation(thisRecord['v']));
                                if (thisRecord.has_Subfield('x'))
                                    newNameSubj.Add_Topic(Remove_Trailing_Punctuation(thisRecord['x']));
                                if (thisRecord.has_Subfield('y'))
                                    newNameSubj.Add_Temporal(Remove_Trailing_Punctuation(thisRecord['y']));
                                if (thisRecord.has_Subfield('z'))
                                    newNameSubj.Add_Geographic(Remove_Trailing_Punctuation(thisRecord['z']));
                                if (thisRecord.has_Subfield('2'))
                                    newNameSubj.Authority = thisRecord['2'];
                                switch (thisRecord.Indicator2)
                                {
                                    case '0':
                                        newNameSubj.Authority = "lcsh";
                                        break;

                                    case '1':
                                        newNameSubj.Authority = "lcshac";
                                        break;

                                    case '2':
                                        newNameSubj.Authority = "mesh";
                                        break;

                                    case '3':
                                        newNameSubj.Authority = "nal";
                                        break;

                                    case '5':
                                        newNameSubj.Authority = "csh";
                                        break;

                                    case '6':
                                        newNameSubj.Authority = "rvm";
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }

        private static void Add_Conference_Name(Bibliographic_Info thisBibInfo, MARC_Record record, int tag, int name_type)
        {
            // Step through each instance of this tag                    
            foreach (MARC_Field thisRecord in record[tag])
            {
                // Create the name object
                Name_Info newName = new Name_Info();
                newName.Name_Type = Name_Info_Type_Enum.Conference;

                // Only continue if there is an id in this record
                if ((thisRecord.has_Subfield('a')) && (thisRecord['a'].ToUpper().IndexOf("PALMM") < 0))
                {
                    newName.Full_Name = Remove_Trailing_Punctuation(thisRecord['a']);

                    if (thisRecord.has_Subfield('c'))
                        newName.Description = thisRecord['c'].Replace(")", "").Replace("(", "");
                    if (thisRecord.has_Subfield('d'))
                        newName.Dates = Remove_Trailing_Punctuation(thisRecord['d']);
                    if (thisRecord.has_Subfield('e'))
                    {
                        newName.Full_Name = newName.Full_Name + " -- " + Remove_Trailing_Punctuation(thisRecord['e']);
                    }

                    if (thisRecord.has_Subfield('j'))
                        newName.Add_Role(Remove_Trailing_Punctuation(thisRecord['j']));
                    if (thisRecord.has_Subfield('u'))
                        newName.Affiliation = Remove_Trailing_Punctuation(thisRecord['u']);

                    // Is there a relator code?
                    if (thisRecord.has_Subfield('4'))
                    {
                        // Get the relator code
                        string relatorcode = thisRecord['4'];
                        newName.Add_Role(relatorcode, "marcrelator", Name_Info_Role_Type_Enum.Code);
                    }

                    switch (name_type)
                    {
                        case 1:
                            thisBibInfo.Main_Entity_Name = newName;
                            break;

                        case 2:
                            thisBibInfo.Add_Named_Entity(newName);
                            break;

                        case 4:
                            Subject_Info_Name newNameSubj = new Subject_Info_Name();
                            newNameSubj.Set_Internal_Name(newName);
                            if (thisRecord.has_Subfield('v'))
                                newNameSubj.Add_Genre(Remove_Trailing_Punctuation(thisRecord['v']));
                            if (thisRecord.has_Subfield('x'))
                                newNameSubj.Add_Topic(Remove_Trailing_Punctuation(thisRecord['x']));
                            if (thisRecord.has_Subfield('y'))
                                newNameSubj.Add_Temporal(Remove_Trailing_Punctuation(thisRecord['y']));
                            if (thisRecord.has_Subfield('z'))
                                newNameSubj.Add_Geographic(Remove_Trailing_Punctuation(thisRecord['z']));
                            if (thisRecord.has_Subfield('2'))
                                newNameSubj.Authority = thisRecord['2'];
                            switch (thisRecord.Indicator2)
                            {
                                case '0':
                                    newNameSubj.Authority = "lcsh";
                                    break;

                                case '1':
                                    newNameSubj.Authority = "lcshac";
                                    break;

                                case '2':
                                    newNameSubj.Authority = "mesh";
                                    break;

                                case '3':
                                    newNameSubj.Authority = "nal";
                                    break;

                                case '5':
                                    newNameSubj.Authority = "csh";
                                    break;

                                case '6':
                                    newNameSubj.Authority = "rvm";
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        private static void Add_Descriptions(Bibliographic_Info thisBibInfo, MARC_Record record)
        {
            // Look for any THESIS note
            foreach (MARC_Field thisRecord in record[502])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.Thesis);
                }
            }

            // Look for any BIBLIORAPHY note
            foreach (MARC_Field thisRecord in record[504])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.Bibliography);
                }
            }

            // Look for any RESTRICTION note
            foreach (MARC_Field thisRecord in record[506])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    thisBibInfo.Access_Condition.Text = thisRecord['a'];
                    // thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.restriction);
                }
            }

            // Look for any CREATION/PRODUCTION CREDITS note
            foreach (MARC_Field thisRecord in record[508])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.CreationCredits);
                }
            }

            // Look for any CREATION/PRODUCTION CREDITS note
            foreach (MARC_Field thisRecord in record[510])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.CitationReference);
                }
            }

            // Look for any PERFORMERS note
            foreach (MARC_Field thisRecord in record[511])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    if (thisRecord.Indicator1 == '1')
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.Performers, "cast");
                    }
                    else
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.Performers);
                    }
                }
            }

            // Look for any DATE/VENUE note
            foreach (MARC_Field thisRecord in record[518])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    if (thisRecord.has_Subfield('3'))
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.DateVenue, thisRecord['3']);
                    }
                    else
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.DateVenue);
                    }
                }
            }

            // Look for any PREFERRED CITATION note
            foreach (MARC_Field thisRecord in record[524])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    if (thisRecord.has_Subfield('3'))
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.PreferredCitation, thisRecord['3']);
                    }
                    else
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.PreferredCitation);
                    }
                }
            }

            // Look for any ADDITIONAL PHYSICAL FORM note
            foreach (MARC_Field thisRecord in record[530])
            {
                StringBuilder builder_530 = new StringBuilder();
                if (thisRecord.has_Subfield('3'))
                {
                    builder_530.Append(thisRecord['3'] + " ");
                }
                if (thisRecord.has_Subfield('a'))
                {
                    builder_530.Append(thisRecord['a'] + " ");
                }
                if (thisRecord.has_Subfield('b'))
                {
                    builder_530.Append(thisRecord['b'] + " ");
                }
                if (thisRecord.has_Subfield('c'))
                {
                    builder_530.Append(thisRecord['c'] + " ");
                }
                string complete_530 = builder_530.ToString().Trim();
                if (complete_530.Length > 0)
                {
                    thisBibInfo.Add_Note(complete_530, Note_Type_Enum.AdditionalPhysicalForm);
                }
            }

            // Look for any ORIGINAL VERSION note
            foreach (MARC_Field thisRecord in record[534])
            {
                StringBuilder builder_534 = new StringBuilder();
                if (thisRecord.has_Subfield('p'))
                {
                    builder_534.Append(thisRecord['p'] + " ");
                }
                if (thisRecord.has_Subfield('k'))
                {
                    builder_534.Append(thisRecord['k'] + " ");
                }
                if (thisRecord.has_Subfield('a'))
                {
                    builder_534.Append(thisRecord['a'] + " ");
                }
                if (thisRecord.has_Subfield('c'))
                {
                    builder_534.Append(thisRecord['c'] + " ");
                }
                if (thisRecord.has_Subfield('e'))
                {
                    builder_534.Append(thisRecord['e'] + " ");
                }
                if (thisRecord.has_Subfield('l'))
                {
                    builder_534.Append(thisRecord['l'] + " ");
                }
                if (thisRecord.has_Subfield('t'))
                {
                    builder_534.Append(thisRecord['t'] + " ");
                }
                if (thisRecord.has_Subfield('b'))
                {
                    builder_534.Append(thisRecord['b'] + " ");
                }
                if (thisRecord.has_Subfield('n'))
                {
                    builder_534.Append(thisRecord['n'] + " ");
                }
                string complete_534 = builder_534.ToString().Trim();
                if (complete_534.Length > 0)
                {
                    thisBibInfo.Add_Note(complete_534, Note_Type_Enum.OriginalVersion);
                }
            }

            // Look for any ORIGINAL LOCATION note
            foreach (MARC_Field thisRecord in record[535])
            {
                StringBuilder builder_535 = new StringBuilder();
                bool possibly_holding_location = true;
                if (thisRecord.has_Subfield('3'))
                {
                    builder_535.Append(thisRecord['3'] + " ");
                    possibly_holding_location = false;
                }
                if (thisRecord.has_Subfield('a'))
                {
                    builder_535.Append(thisRecord['a'] + " ");
                }
                if (thisRecord.has_Subfield('b'))
                {
                    builder_535.Append(thisRecord['b'] + " ");
                    possibly_holding_location = false;
                }
                if (thisRecord.has_Subfield('d'))
                {
                    builder_535.Append(thisRecord['d'] + " ");
                    possibly_holding_location = false;
                }
                string complete_535 = builder_535.ToString().Trim();
                if (complete_535.Length > 0)
                {
                    if ((possibly_holding_location) && (thisRecord.Indicator1 == '1'))
                    {
                        thisBibInfo.Location.Holding_Name = complete_535;
                    }
                    else
                    {
                        thisBibInfo.Add_Note(complete_535, Note_Type_Enum.OriginalLocation);
                    }
                }
            }

            // Look for any FUNDING note
            foreach (MARC_Field thisRecord in record[536])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.Funding);
                }
            }

            // Look for any SYSTEM DETAILS note
            foreach (MARC_Field thisRecord in record[538])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.SystemDetails);
                }
            }

            // Look for any ACQUISITION note
            foreach (MARC_Field thisRecord in record[541])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    if (thisRecord.Indicator1 != '0')
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.Acquisition);
                    }
                }
            }

            // Look for any BIOGRAPHICAL note
            foreach (MARC_Field thisRecord in record[545])
            {
                if (thisRecord.has_Subfield('a'))
                {
					if (thisRecord.has_Subfield('b'))
					{
						thisBibInfo.Add_Note(thisRecord['a'] + " " + thisRecord['b'], Note_Type_Enum.Biographical);
					}
					else
					{
						thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.Biographical);
					}
                    
                }
            }

            // Look for any LANGUAGE note
            foreach (MARC_Field thisRecord in record[546])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    if (thisRecord.has_Subfield('3'))
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.Language, thisRecord['3']);
                    }
                    else
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.Language);
                    }
                }
            }

            // Look for any OWNERSHIP note
            foreach (MARC_Field thisRecord in record[561])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    if (thisRecord.has_Subfield('3'))
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.Ownership, thisRecord['3']);
                    }
                    else
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.Ownership);
                    }
                }
            }

            // Look for any VERSION IDENTIFICATION note
            foreach (MARC_Field thisRecord in record[562])
            {
                StringBuilder builder_562 = new StringBuilder();
                if (thisRecord.has_Subfield('3'))
                {
                    builder_562.Append(thisRecord['3'] + " ");
                }
                if (thisRecord.has_Subfield('a'))
                {
                    builder_562.Append(thisRecord['a'] + " ");
                }
                if (thisRecord.has_Subfield('c'))
                {
                    builder_562.Append(thisRecord['c'] + " ");
                }
                if (thisRecord.has_Subfield('e'))
                {
                    builder_562.Append(thisRecord['e'] + " ");
                }
                if (thisRecord.has_Subfield('b'))
                {
                    builder_562.Append(thisRecord['b'] + " ");
                }
                string complete_562 = builder_562.ToString().Trim();
                if (complete_562.Length > 0)
                {
                    thisBibInfo.Add_Note(complete_562, Note_Type_Enum.VersionIdentification);
                }
            }

            // Look for any PUBLICATIONS note
            foreach (MARC_Field thisRecord in record[581])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    if (thisRecord.has_Subfield('3'))
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.Publications, thisRecord['3']);
                    }
                    else
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.Publications);
                    }
                }
            }

            // Look for any EXHIBITIONS note
            foreach (MARC_Field thisRecord in record[585])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    if (thisRecord.has_Subfield('3'))
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.Exhibitions, thisRecord['3']);
                    }
                    else
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.Exhibitions);
                    }
                }
            }

            // Look for statement of responsibility (from TITLE - 245 |c )
            foreach (MARC_Field thisRecord in record[245])
            {
                if (thisRecord.has_Subfield('c'))
                {
                    thisBibInfo.Add_Note(thisRecord['c'], Note_Type_Enum.StatementOfResponsibility);
                }
            }

            // Look for any publication dates or sequential designation note (362)
            foreach (MARC_Field thisRecord in record[362])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    if (thisRecord.has_Subfield('z'))
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.DatesSequentialDesignation, thisRecord['z']);
                    }
                    else
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.DatesSequentialDesignation);
                    }
                }
            }

            // Look for any numbering peculiarities note (515)
            foreach (MARC_Field thisRecord in record[515])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.NumberingPeculiarities);
                }
            }

            // Look for any issuing body note (550)
            foreach (MARC_Field thisRecord in record[550])
            {
                if (thisRecord.has_Subfield('a'))
                {
                    thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.IssuingBody);
                }
            }

            // Look for any other NOTES fields ( 500, 501, 513, 522, 525, 563, 567, 586 )
            int[] other_notes = new int[] {500, 501, 513, 522, 525, 563, 567, 586};
            foreach (int tagNumber in other_notes)
            {
                foreach (MARC_Field thisRecord in record[tagNumber])
                {
                    string displayLabel = String.Empty;
                    if (thisRecord.Indicator1 == ' ')
                    {
                        switch (tagNumber)
                        {
                            case 522:
                                displayLabel = "Geographic Coverage";
                                break;

                            case 525:
                                displayLabel = "Supplement Note";
                                break;

                            case 567:
                                displayLabel = "Methodology";
                                break;

                            case 586:
                                displayLabel = "Awards";
                                break;
                        }
                    }
                    if (thisRecord.has_Subfield('a'))
                    {
                        thisBibInfo.Add_Note(thisRecord['a'], Note_Type_Enum.NONE, displayLabel);
                    }
                }
            }
        }

        private static void Add_Subject(Bibliographic_Info thisBibInfo, MARC_Record record, int tag, string topical_codes, string geographic_codes, string temporal_codes, string genre_codes, string occupation_codes)
        {
            // Step through all of the subjects under this tag
            int subj_index = 1;
            string source;
            foreach (MARC_Field thisRecord in record[tag])
            {
                // Only continue if there is an id in this record
                if (thisRecord.has_Subfield('a'))
                {
                    // Was there  a source?
                    source = String.Empty;
                    if (thisRecord.has_Subfield('2'))
                    {
                        source = thisRecord['2'];
                    }
                    else if ((tag != 653))
                    {
                        switch (thisRecord.Indicator2)
                        {
                            case '0':
                                source = "lcsh";
                                break;

                            case '1':
                                source = "lcshac";
                                break;

                            case '2':
                                source = "mesh";
                                break;

                            case '3':
                                source = "nal";
                                break;

                            case '5':
                                source = "csh";
                                break;

                            case '6':
                                source = "rvm";
                                break;

                            case '9':
                                source = "local";
                                break;
                        }
                    }

                    Subject_Info_Standard obj = new Subject_Info_Standard();
                    obj.Authority = source;
                    obj.ID = "SUBJ" + tag + "_" + subj_index;
                    subj_index++;

                    // Add the topics
                    foreach (char thisTopicChar in topical_codes)
                    {
                        if (thisRecord.has_Subfield(thisTopicChar))
                        {
                            string topicString = thisRecord[thisTopicChar];
                            if (topicString.IndexOf("|") > 0)
                            {
                                string[] topicStringSplit = topicString.Split("|".ToCharArray());
                                foreach (string thisTopicString in topicStringSplit)
                                    obj.Add_Topic(Remove_Trailing_Punctuation(thisTopicString));
                            }
                            else
                            {
                                obj.Add_Topic(Remove_Trailing_Punctuation(topicString));
                            }
                        }
                    }

                    // Add the temporals
                    foreach (char thisTempChar in temporal_codes)
                    {
                        if (thisRecord.has_Subfield(thisTempChar))
                        {
                            string tempString = thisRecord[thisTempChar];
                            if (tempString.IndexOf("|") > 0)
                            {
                                string[] tempStringSplit = tempString.Split("|".ToCharArray());
                                foreach (string thisTempString in tempStringSplit)
                                    obj.Add_Temporal(Remove_Trailing_Punctuation(thisTempString));
                            }
                            else
                            {
                                obj.Add_Temporal(Remove_Trailing_Punctuation(tempString));
                            }
                        }
                    }

                    // Add the geographics
                    foreach (char thisGeoChar in geographic_codes)
                    {
                        if (thisRecord.has_Subfield(thisGeoChar))
                        {
                            string geoString = thisRecord[thisGeoChar];
                            if (geoString.IndexOf("|") > 0)
                            {
                                string[] geoStringSplit = geoString.Split("|".ToCharArray());
                                foreach (string thisGeoString in geoStringSplit)
                                    obj.Add_Geographic(Remove_Trailing_Punctuation(thisGeoString));
                            }
                            else
                            {
                                obj.Add_Geographic(Remove_Trailing_Punctuation(geoString));
                            }
                        }
                    }

                    // Add the genres
                    foreach (char thisGenreChar in genre_codes)
                    {
                        if (thisRecord.has_Subfield(thisGenreChar))
                        {
                            string genreString = thisRecord[thisGenreChar];
                            if (genreString.IndexOf("|") > 0)
                            {
                                string[] genreStringSplit = genreString.Split("|".ToCharArray());
                                foreach (string thisGenreString in genreStringSplit)
                                    obj.Add_Genre(Remove_Trailing_Punctuation(thisGenreString));
                            }
                            else
                            {
                                obj.Add_Genre(Remove_Trailing_Punctuation(genreString));
                            }
                        }
                    }

                    // Add the occupations
                    foreach (char thisOccChar in occupation_codes)
                    {
                        if (thisRecord.has_Subfield(thisOccChar))
                        {
                            string occString = thisRecord[thisOccChar].Replace("--", "|");
                            if (occString.IndexOf("|") > 0)
                            {
                                string[] occStringSplit = occString.Split("|".ToCharArray());
                                foreach (string thisOccString in occStringSplit)
                                    obj.Add_Occupation(Remove_Trailing_Punctuation(thisOccString.Trim()));
                            }
                            else
                            {
                                obj.Add_Occupation(Remove_Trailing_Punctuation(occString));
                            }
                        }
                    }

                    // Add this subject
                    thisBibInfo.Add_Subject(obj);
                }
            }
        }

        #endregion
    }
}