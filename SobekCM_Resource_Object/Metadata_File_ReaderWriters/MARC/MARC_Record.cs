#region License and Copyright

//          SobekCM MARC Library ( Version 1.2 )
//          
//          Copyright (2005-2012) Mark Sullivan. ( Mark.V.Sullivan@gmail.com )
//          
//          This file is part of SobekCM MARC Library.
//          
//          SobekCM MARC Library is free software: you can redistribute it and/or modify
//          it under the terms of the GNU Lesser Public License as published by
//          the Free Software Foundation, either version 3 of the License, or
//          (at your option) any later version.
//            
//          SobekCM MARC Library is distributed in the hope that it will be useful,
//          but WITHOUT ANY WARRANTY; without even the implied warranty of
//          MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//          GNU Lesser Public License for more details.
//            
//          You should have received a copy of the GNU Lesser Public License
//          along with SobekCM MARC Library.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using SobekCM.Resource_Object.MARC.ErrorHandling;
using SobekCM.Resource_Object.MARC.Parsers;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters.MARC.Writers;

#endregion

namespace SobekCM.Resource_Object.MARC
{
    /// <summary> Stores all the information from a MARC21 record </summary>
    /// <remarks>Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    public class MARC_Record
    {
        private string controlNumber;
        private List<MARC_Record_Parsing_Error> errors;
        private readonly SortedList<int, List<MARC_Field>> fields;
        private string leader;
        private List<MARC_Record_Parsing_Warning> warnings;

        /// <summary> Constructor for a new instance of the MARC_XML_Record class </summary>
        public MARC_Record()
        {
            leader = String.Empty;
            fields = new SortedList<int, List<MARC_Field>>();
            Error_Flag = false;
        }

        #region Public properties

        /// <summary> Control number for this record from the 001 field </summary>
        /// <remarks> This is used when importing directly from MARC records into the SobekCM library </remarks>
        public string Control_Number
        {
            get
            {
                if (controlNumber != null)
                    return controlNumber;

                controlNumber = fields.ContainsKey(1) ? fields[1][0].Control_Field_Value : String.Empty;

                return controlNumber;
            }
        }

        /// <summary> Flag is set if there is an error detected while reading this MARC
        /// record from a MARC21 Exchange Format file </summary>
        /// <remarks> This is used when importing directly from MARC records into the SobekCM library </remarks>
        public bool Error_Flag { get; set; }

        /// <summary> Gets or sets the leader portion of this MARC21 Record </summary>
        public string Leader
        {
            get
            {
                // First, compute the overall length of this record
                int total_length = 0;
                int directory_length = 25;
                List<MARC_Field> all_tags = Sorted_MARC_Tag_List;
                foreach (MARC_Field thisTag in all_tags)
                {
                    total_length = total_length + 5 + thisTag.Control_Field_Value.Length;
                    directory_length += 12;
                }

                string total_length_string = (total_length.ToString()).PadLeft(5, '0');
                string total_directory_string = (directory_length.ToString()).PadLeft(5, '0');

                if (leader.Length == 0)
                {
                    return total_length_string + "nam  22" + total_directory_string + "3a 4500";
                }
                return total_length_string + leader.Substring(5, 7) + total_directory_string + leader.Substring(17);
            }
            set { leader = value; }
        }

        /// <summary> Gets the collection of MARC fields, by MARC tag number </summary>
        /// <param name="Tag"> MARC tag number to return all matching fields </param>
        /// <returns> Collection of matching tags, or an empty read only collection </returns>
        public ReadOnlyCollection<MARC_Field> this[int Tag]
        {
            get
            {
                if (fields.ContainsKey(Tag))
                    return new ReadOnlyCollection<MARC_Field>(fields[Tag]);
                return new ReadOnlyCollection<MARC_Field>(new List<MARC_Field>());
            }
        }

        /// <summary> Returns a list of all the MARC tags, sorted by tag number and
        /// ready to display as a complete MARC record </summary>
        public List<MARC_Field> Sorted_MARC_Tag_List
        {
            get
            {
                List<MARC_Field> returnValue = new List<MARC_Field>();

                foreach (List<MARC_Field> fields_by_tag in fields.Values)
                {
                    returnValue.AddRange(fields_by_tag);
                }

                return returnValue;
            }
        }

        #endregion

        #region Public methods to check if a field exists, add a field, etc...

        /// <summary> Gets a flag indicating if a particular field exists </summary>
        /// <param name="Tag">Tag for the MARC field to check</param>
        /// <returns>TRUE if the field exists, otherwise FALSE</returns>
        public bool has_Field(int Tag)
        {
            return fields.ContainsKey(Tag);
        }

        /// <summary> Add a new control field to this record </summary>
        /// <param name="Tag">Tag for new control field</param>
        /// <param name="Control_Field_Value">Data for the new control field</param>
        /// <returns>New control field object created and added</returns>
        public MARC_Field Add_Field(int Tag, string Control_Field_Value)
        {
            // Create the new control field
            MARC_Field newField = new MARC_Field(Tag, Control_Field_Value);

            // Either add this to the existing list, or create a new one
            if (fields.ContainsKey(Tag))
                fields[Tag].Add(newField);
            else
            {
                List<MARC_Field> newTagCollection = new List<MARC_Field> {newField};
                fields[Tag] = newTagCollection;
            }

            // Return the newlly built control field
            return newField;
        }

        /// <summary> Add a new data field to this record </summary>
        /// <param name="Tag">Tag for new data field</param>
        /// <param name="Indicator1">First indicator for new data field</param>
        /// <param name="Indicator2">Second indicator for new data field</param>
        /// <returns>New data field object created and added</returns>
        public MARC_Field Add_Field(int Tag, char Indicator1, char Indicator2)
        {
            // Create the new datafield
            MARC_Field newField = new MARC_Field(Tag, Indicator1, Indicator2);

            // Either add this to the existing list, or create a new one
            if (fields.ContainsKey(Tag))
                fields[Tag].Add(newField);
            else
            {
                List<MARC_Field> newTagCollection = new List<MARC_Field> {newField};
                fields[Tag] = newTagCollection;
            }

            // Return the newlly built data field
            return newField;
        }

        /// <summary> Add a new data field to this record </summary>
        /// <param name="Tag">Tag for new data field</param>
        /// <param name="Indicators">Both indicators</param>
        /// <param name="Control_Field_Value">Value for this control field </param>
        /// <returns>New data field object created and added</returns>
        public MARC_Field Add_Field(int Tag, string Indicators, string Control_Field_Value)
        {
            // Create the new datafield
            MARC_Field newField = new MARC_Field(Tag, Control_Field_Value) {Indicators = Indicators};

            // Either add this to the existing list, or create a new one
            if (fields.ContainsKey(Tag))
                fields[Tag].Add(newField);
            else
            {
                List<MARC_Field> newTagCollection = new List<MARC_Field> {newField};
                fields[Tag] = newTagCollection;
            }

            // Return the newlly built data field
            return newField;
        }

        /// <summary> Adds a new field to this record </summary>
        /// <param name="New_Field"> New field to add </param>
        public void Add_Field(MARC_Field New_Field)
        {
            if (New_Field == null)
                return;

            // Either add this to the existing list, or create a new one
            if (fields.ContainsKey(New_Field.Tag))
                fields[New_Field.Tag].Add(New_Field);
            else
            {
                List<MARC_Field> newTagCollection = new List<MARC_Field> {New_Field};
                fields[New_Field.Tag] = newTagCollection;
            }
        }

        /// <summary> Gets data from a particular subfield within a singular data field  </summary>
        /// <param name="Tag">Tag for new data field</param>
        /// <param name="Subfield">Code for the subfield in question</param>
        /// <returns>The value of the subfield, or an empty string </returns>
        /// <remarks> If there are multiple instances of this subfield, then they are returned 
        /// together with a '|' delimiter between them </remarks>
        public string Get_Data_Subfield(int Tag, char Subfield)
        {
            if ((fields.ContainsKey(Tag)) && (fields[Tag][0].has_Subfield(Subfield)))
                return fields[Tag][0][Subfield];
            return String.Empty;
        }

        /// <summary> Removes all occurrences of a tag </summary>
        /// <param name="Tag_Number"> Tag number of the MARC tags to remove </param>
        public void Remove_Tag(int Tag_Number)
        {
            // Remove from the list of fields
            if (fields.ContainsKey(Tag_Number))
                fields.Remove(Tag_Number);
        }

        #endregion

        #region Methods to read from a MarcXML file directly into this structure

        /// <summary> Reads the data from a MARC XML file into this record </summary>
        /// <param name="MARC_XML_File">Input MARC XML file</param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Read_From_MARC_XML_File(string MARC_XML_File)
        {
            return MARCXML_Parser.Read_From_MARC_XML_File(MARC_XML_File, this);
        }

        /// <summary> Reads the data from a XML Node Reader </summary>
        /// <param name="nodeReader">XML Node Reader </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Read_MARC_Info(XmlReader nodeReader)
        {
            return MARCXML_Parser.Read_MARC_Info(nodeReader, this);
        }

        #endregion

        #region Methods to get or save the MarcXML for this record 

        /// <summary> Saves this MARC records as MARC XML </summary>
        /// <param name="Filename"> Filename to save this MARC record as </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_MARC_XML(string Filename)
        {
            bool returnValue = false;
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(Filename, false, Encoding.UTF8);
                writer.Write(To_MARC_XML());
                writer.Flush();
                writer.Close();
                returnValue = true;
            }
            catch 
            {
                returnValue = false;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
            return returnValue;
        }

        /// <summary> Returns this MARC record as MARC XML </summary>
        /// <returns> This record as MARC XML </returns>
        public string To_MARC_XML()
        {
            return MARCXML_Writer.To_MarcXML(this);
        }

        #endregion

        #region Methods to get or save the MARC21 for this record

        /// <summary> Saves this MARC record as MARC21 Exchange format record data file </summary>
        /// <param name="Filename"> Filename to save this MARC record as  </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_MARC21(string Filename)
        {
            try
            {
                // This code below was added to prevent the resulting
                // MARC21 file from having the UTF-8 Byte-Order Marks encoding bytes
                // ( 0xEF,0xBB,0xBF ) included in the MARC21 file
                MemoryStream ms = new MemoryStream();

                StreamWriter writer = new StreamWriter(ms, Encoding.UTF8);
                writer.Write(To_Machine_Readable_Record());
                writer.Flush();

                ms.Seek(0, SeekOrigin.Begin);

                FileStream fs = File.Create(Filename);
                fs.Write(ms.GetBuffer(), 3, (int) (ms.Length - 3));
                fs.Flush();
                fs.Close();
                writer.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Returns a string which represents this record in machine readable record format. </summary>
        /// <returns> This MARC record as MARC21 Exchange format record string</returns>
        public string To_Machine_Readable_Record()
        {
            return MARC21_Exchange_Format_Writer.To_Machine_Readable_Record(this);
        }

        #endregion

        #region Method overrides the ToString() method 

        /// <summary> Outputs this record as a string </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Create the StringBuilder
            StringBuilder returnVal = new StringBuilder(2000);

            // Add the leader
            returnVal.Append("LDR " + Leader + "\r\n");

            // Step through each field in the collection
            foreach (int thisTag in fields.Keys)
            {
                List<MARC_Field> matchingFields = fields[thisTag];
                foreach (MARC_Field thisField in matchingFields)
                {
                    if (thisField.Subfield_Count == 0)
                    {
                        if (thisField.Control_Field_Value.Length > 0)
                        {
                            returnVal.Append(thisField.Tag.ToString().PadLeft(3, '0') + " " + thisField.Control_Field_Value + "\r\n");
                        }
                    }
                    else
                    {
                        returnVal.Append(thisField.Tag.ToString().PadLeft(3, '0') + " " + thisField.Indicators);

                        // Build the complete line
                        foreach (MARC_Subfield thisSubfield in thisField.Subfields)
                        {
                            if (thisSubfield.Subfield_Code == ' ')
                            {
                                returnVal.Append(" " + thisSubfield.Data);
                            }
                            else
                            {
                                returnVal.Append(" |" + thisSubfield.Subfield_Code + " " + thisSubfield.Data);
                            }
                        }

                        returnVal.Append("\r\n");
                    }
                }
            }

            // Return the built string
            return returnVal.ToString();
        }

        #endregion

        #region Methods to handle the list of warnings stored in this MARC record object

        /// <summary> Returns a flag indicating if there are any warnings associated with this record  </summary>
        public bool Has_Warnings
        {
            get { return (warnings != null) && (warnings.Count != 0); }
        }

        /// <summary> Returns the list of warnings associated with this MARC record  </summary>
        public ReadOnlyCollection<MARC_Record_Parsing_Warning> Warnings
        {
            get { return warnings == null ? null : new ReadOnlyCollection<MARC_Record_Parsing_Warning>(warnings); }
        }

        /// <summary> Add a new warning which occurred during parsing to this MARC record object </summary>
        /// <param name="Warning"> Warning object to add to the list </param>
        public void Add_Warning(MARC_Record_Parsing_Warning Warning)
        {
            // Ensure the list is built
            if (warnings == null)
                warnings = new List<MARC_Record_Parsing_Warning>();

            // If no other warning of the same type exists, add this
            if (!warnings.Contains(Warning))
                warnings.Add(Warning);
        }

        /// <summary> Add a new warning which occurred during parsing to this MARC record object </summary>
        /// <param name="Warning_Type"> Type of this warning </param>
        /// <param name="Warning_Details"> Any additional information about a warning </param>
        public void Add_Warning(MARC_Record_Parsing_Warning_Type_Enum Warning_Type, string Warning_Details)
        {
            // Ensure the list is built
            if (warnings == null)
                warnings = new List<MARC_Record_Parsing_Warning>();

            // Build this warning object
            MARC_Record_Parsing_Warning Warning = new MARC_Record_Parsing_Warning(Warning_Type, Warning_Details);

            // If no other warning of the same type exists, add this
            if (!warnings.Contains(Warning))
                warnings.Add(Warning);
        }

        /// <summary> Add a new warning which occurred during parsing to this MARC record object </summary>
        /// <param name="Warning_Type"> Type of this warning </param>
        public void Add_Warning(MARC_Record_Parsing_Warning_Type_Enum Warning_Type)
        {
            // Ensure the list is built
            if (warnings == null)
                warnings = new List<MARC_Record_Parsing_Warning>();

            // Build this warning object
            MARC_Record_Parsing_Warning Warning = new MARC_Record_Parsing_Warning(Warning_Type);

            // If no other warning of the same type exists, add this
            if (!warnings.Contains(Warning))
                warnings.Add(Warning);
        }

        #endregion

        #region Methods to handle the list of errors stored in this MARC record object

        /// <summary> Returns a flag indicating if there are any errors associated with this record  </summary>
        public bool Has_Errors
        {
            get { return (errors != null) && (errors.Count != 0); }
        }

        /// <summary> Returns the list of erors associated with this MARC record  </summary>
        public ReadOnlyCollection<MARC_Record_Parsing_Error> Errors
        {
            get { return errors == null ? null : new ReadOnlyCollection<MARC_Record_Parsing_Error>(errors); }
        }

        /// <summary> Add a new error which occurred during parsing to this MARC record object </summary>
        /// <param name="Error"> Error object to add to the list </param>
        public void Add_Error(MARC_Record_Parsing_Error Error)
        {
            // Ensure the list is built
            if (errors == null)
                errors = new List<MARC_Record_Parsing_Error>();

            // If no other error of the same type exists, add this
            if (!errors.Contains(Error))
                errors.Add(Error);
        }

        /// <summary> Add a new error which occurred during parsing to this MARC record object </summary>
        /// <param name="Error_Type"> Type of this error </param>
        /// <param name="Error_Details"> Any additional information about an error </param>
        public void Add_Error(MARC_Record_Parsing_Error_Type_Enum Error_Type, string Error_Details)
        {
            // Ensure the list is built
            if (errors == null)
                errors = new List<MARC_Record_Parsing_Error>();

            // Build this Error object
            MARC_Record_Parsing_Error Error = new MARC_Record_Parsing_Error(Error_Type, Error_Details);

            // If no other Error of the same type exists, add this
            if (!errors.Contains(Error))
                errors.Add(Error);
        }

        /// <summary> Add a new error which occurred during parsing to this MARC record object </summary>
        /// <param name="Error_Type"> Type of this error </param>
        public void Add_Error(MARC_Record_Parsing_Error_Type_Enum Error_Type)
        {
            // Ensure the list is built
            if (errors == null)
                errors = new List<MARC_Record_Parsing_Error>();

            // Build this error object
            MARC_Record_Parsing_Error Error = new MARC_Record_Parsing_Error(Error_Type);

            // If no other error of the same type exists, add this
            if (!errors.Contains(Error))
                errors.Add(Error);
        }

        #endregion
    }
}