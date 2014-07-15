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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SobekCM.Resource_Object.MARC.ErrorHandling;

#endregion

namespace SobekCM.Resource_Object.MARC.Parsers
{
    /// <summary> Enumeration indicates the type of CHARACTER encoding within the MARC record </summary>
    public enum Record_Character_Encoding : byte
    {
        /// <summary> Marc Character encoding </summary>
        MARC = 1,

        /// <summary> Unicode character encoding </summary>
        Unicode,

        /// <summary> Unrecognized character encoding value found  (treated as Unicode) </summary>
        UNRECOGNIZED
    }

    /// <summary> Enumeration indicates what action should be taken when errors are encountered during
    /// parsing a file </summary>
    public enum Action_On_Error_Encountered_Enum : byte
    {
        /// <summary> Throws an exception and stops processing a file immediately on the first error </summary>
        THROW_EXCEPTION = 1,

        /// <summary> [DEFAULT] Store the error in the record and return the record </summary>
        STORE_IN_RECORD
    }

    /// <summary> Parser steps through the records in a MARC21 Electronic Format file or stream. </summary>
    /// <remarks> Written by Mark Sullivan (2005) <br /><br />
    /// You can either pass in the stream or file to read into the constructor and immediately begin using Next() to step
    /// through them, or you can use the empty constructor and call the Parse methods for the first record. <br /><br />
    /// To  use the IEnumerable interface, you must pass in the Stream or filename in the constructor.</remarks>
    public class MARC21_Exchange_Format_Parser : IDisposable, IEnumerable<MARC_Record>, IEnumerator<MARC_Record>
    {
        // Stream used to read the Marc21 records

        // Constants used when parsing the Marc21 stream
        private const char END_OF_RECORD = (char) 29;
        private const char RECORD_SEPERATOR = (char) 30;
        private const char UNIT_SEPERATOR = (char) 31;
        private const int ALTERNATE_CHARACTER_SET_INDICATOR = 27;

        /// <summary> Variable indicates what action should be taken when an error is encountered
        /// while parsing a MARC21 file </summary>
        public static Action_On_Error_Encountered_Enum Action_On_Error = Action_On_Error_Encountered_Enum.STORE_IN_RECORD;

        #region Constructors 

        /// <summary> Constructor for a new instance of this class </summary>
        public MARC21_Exchange_Format_Parser()
        {
            // Constructor does nothing
        }

        /// <summary> Constructor for a new instance of this class </summary>
        /// <param name="Marc21_Stream"> Open stream from which to read Marc21 records </param>
        public MARC21_Exchange_Format_Parser(Stream Marc21_Stream)
        {
            // Create the new reader object
            reader = new BinaryReader(Marc21_Stream);
        }

        /// <summary> Constructor for a new instance of this class </summary>
        /// <param name="Marc21_File"> Name of the file to parse </param>
        public MARC21_Exchange_Format_Parser(string Marc21_File)
        {
            // Create the new reader object
            reader = new BinaryReader(File.Open(Marc21_File, FileMode.Open));
        }

        #endregion

        private BinaryReader reader;

        /// <summary> Begins parsing from a stream containing MARC21 Electronic format records. </summary>
        /// <param name="Marc21_Stream"> Open stream from which to read Marc21 records </param>
        /// <returns> A built record, or NULL if no records are contained within the file </returns>
        public MARC_Record Parse(Stream Marc21_Stream)
        {
            // Create the new reader object
            reader = new BinaryReader(Marc21_Stream);

            // Return the first record
            return parse_next_record();
        }

        /// <summary> Begins parsing a new MARC21 Electronic format file. </summary>
        /// <param name="Marc21_File"> Name of the file to parse </param>
        /// <returns> A built record, or NULL if no records are contained within the file </returns>
        public MARC_Record Parse(string Marc21_File)
        {
            // Create the new reader object
            reader = new BinaryReader(File.Open(Marc21_File, FileMode.Open));

            // Return the first record
            return parse_next_record();
        }


        /// <summary> Returns the next record in the MARC21 Electronic format file or stream </summary>
        /// <returns> Next object, or NULL </returns>
        public MARC_Record Next()
        {
            if (reader != null)
                return parse_next_record();
            return null;
        }

        /// <summary> Close the stream reader used for this parsing </summary>
        public void Close()
        {
            try
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
            catch
            {
            }
        }

        #region Method which actually parses the stream for the next record 

        /// <summary> Flag indicates if an end of file has been reached </summary>
        /// <remarks> Whenever the EOF is reached, the stream is closed automatically </remarks>
        public bool EOF_Flag { get; set; }

        private MARC_Record parse_next_record()
        {
            // Create the MARC record to return and subfield collection
            MARC_Record thisRecord = new MARC_Record();

            try
            {
                // Some values to check the end of the file
                long file_length = reader.BaseStream.Length;

                // Create the StringBuilder object for this record
                StringBuilder leaderBuilder = new StringBuilder(30);

                // Read to first character
                int result = reader.Read();
                bool eof = false;

                // Read the leader and directory directly into a string, since this will not have specially
                // coded characters ( leader and directory end with a RECORD_SEPERATOR )
                int count = 0;
                while ((!eof) && (result != END_OF_RECORD) && (result != RECORD_SEPERATOR) && (count < 24))
                {
                    // Want to skip any special characters at the beginning (like encoding characters)
                    if (result < 127)
                    {
                        // Save this character directly
                        leaderBuilder.Append((char) result);
                        count++;
                    }

                    // Read the next character and increment the count
                    if (reader.BaseStream.Position < file_length)
                    {
                        result = reader.ReadByte();
                    }
                    else
                    {
                        eof = true;
                    }
                }

                // If this is the empty string, then just return null (DONE!)
                if (eof)
                {
                    //set flag to indicate that the EOF has been reached
                    EOF_Flag = true;

                    // Close the reader
                    Close();

                    // return a null value to end file processing of the MARC file
                    return null;
                }

                // Ensure the leader was correctly retrieved
                if (leaderBuilder.Length < 24)
                {
                    throw new ApplicationException("Error reading leader.  Either end of file, group seperator, or record seperator found prematurely.");
                }

                // Save the leader into the record 
                thisRecord.Leader = leaderBuilder.ToString();

                // Verify the type of character encoding used here
                Record_Character_Encoding encoding = Record_Character_Encoding.UNRECOGNIZED;
                switch (thisRecord.Leader[9])
                {
                    case ' ':
                        encoding = Record_Character_Encoding.MARC;
                        break;

                    case 'a':
                        encoding = Record_Character_Encoding.Unicode;
                        break;
                }

                // Now, read in all the directory information
                List<MARC21_Parser_Directory_Entry> directory_entries = new List<MARC21_Parser_Directory_Entry>();
                count = 0;
                int tag = 0;
                int field_length = 0;
                int starting_position = 0;
                short temp;
                while ((result != END_OF_RECORD) && (result != RECORD_SEPERATOR))
                {
                    // Set the temp value to zero here
                    temp = 0;
                    if (!Int16.TryParse(((char) result).ToString(), out temp))
                    {
                        if (Action_On_Error == Action_On_Error_Encountered_Enum.STORE_IN_RECORD)
                            thisRecord.Add_Error(MARC_Record_Parsing_Error_Type_Enum.Invalid_Directory_Encountered, "Found invalid (non-numeric) character in a directory entry.");
                        else
                            throw new ApplicationException("Found invalid (non-numeric) character in a directory entry.");
                    }

                    // Increment different values, depending on how far into this directory
                    // the reader has gotten.
                    switch (count)
                    {
                        case 0:
                        case 1:
                        case 2:
                            tag = (tag*10) + temp;
                            break;

                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            field_length = (field_length*10) + temp;
                            break;

                        case 7:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                            starting_position = (starting_position*10) + temp;
                            break;
                    }

                    // Read the next character
                    result = reader.Read();
                    count++;

                    // If this directory entry has been completely read, save it
                    // and reset the values for the next directory
                    if (count == 12)
                    {
                        directory_entries.Add(new MARC21_Parser_Directory_Entry((short) tag, (short) field_length, (short) starting_position));
                        tag = 0;
                        field_length = 0;
                        starting_position = 0;
                        count = 0;
                    }
                }

                // Use a memory stream to accumulate bytes (we don't yet know the character
                // encoding for this record, so needs to remain bytes )
                MemoryStream byteFieldBuilder = new MemoryStream();

                // Read all the data from the variable fields
                Dictionary<short, MARC21_Parser_Variable_Field_Data> field_data = new Dictionary<short, MARC21_Parser_Variable_Field_Data>();
                count = 0;
                int start_index = 0;
                short last_field_start_index = 0;
                result = reader.Read();
                while (result != END_OF_RECORD)
                {
                    // Was this the end of the field (or tag)?
                    if (result == RECORD_SEPERATOR)
                    {
                        // Get the value for this field
                        byte[] fieldAsByteArray = byteFieldBuilder.ToArray();

                        // Get the field as string, depending on the encoding
                        string fieldAsString;
                        switch (encoding)
                        {
                            case Record_Character_Encoding.MARC:
                                fieldAsString = Convert_Marc_Bytes_To_Unicode_String(thisRecord, fieldAsByteArray);
                                break;

                            default:
                                fieldAsString = Encoding.UTF8.GetString(fieldAsByteArray);
                                break;
                        }

                        // Clear the byte field builder (create new memory stream)
                        byteFieldBuilder = new MemoryStream();

                        // Add the field to the list of variable data
                        field_data.Add((short) start_index, new MARC21_Parser_Variable_Field_Data((short) start_index, fieldAsString));

                        // This may be the last field, so save this index
                        last_field_start_index = (short) start_index;

                        // Save the count as the next start index
                        start_index = count + 1;
                    }
                    else
                    {
                        // Save this byte
                        byteFieldBuilder.WriteByte((byte) result);
                    }

                    // Read the next character
                    result = reader.ReadByte();
                    count++;
                }

                // Now, step through the directory, retrieve each pre-converted field data,
                // and finish parsing
                int directory_error_correction = 0;
                foreach (MARC21_Parser_Directory_Entry directoryEntry in directory_entries)
                {
                    // Get the field
                    if (!field_data.ContainsKey((short) (directoryEntry.Starting_Position + directory_error_correction)))
                    {
                        while ((!field_data.ContainsKey((short) (directoryEntry.Starting_Position + directory_error_correction))) && (last_field_start_index > directoryEntry.Starting_Position + directory_error_correction))
                        {
                            directory_error_correction += 1;
                        }

                        // If this still didn't work, throw the exception
                        if (!field_data.ContainsKey((short) (directoryEntry.Starting_Position + directory_error_correction)))
                        {
                            if (Action_On_Error == Action_On_Error_Encountered_Enum.STORE_IN_RECORD)
                                thisRecord.Add_Error(MARC_Record_Parsing_Error_Type_Enum.Directory_Field_Mismatch_Unhandled);
                            else
                                throw new ApplicationException("Field indexes and directory information cannot be resolved with one another.");
                        }
                        else
                        {
                            // This worked, but add a warning none-the-less
                            thisRecord.Add_Warning(MARC_Record_Parsing_Warning_Type_Enum.Directory_Field_Mismatch_Handled);
                        }
                    }
                    MARC21_Parser_Variable_Field_Data fieldData = field_data[(short) (directoryEntry.Starting_Position + directory_error_correction)];
                    string variable_field_data = fieldData.Field_Data;

                    // See if this row has an indicator
                    string indicator = "";
                    if ((variable_field_data.Length > 3) && (variable_field_data[2] == (UNIT_SEPERATOR)))
                    {
                        indicator = variable_field_data.Substring(0, 2);
                        variable_field_data = variable_field_data.Substring(2);
                    }
                    else
                        variable_field_data = variable_field_data.Substring(0);

                    // Is this split into seperate subfields?
                    if ((variable_field_data.Length > 1) && (variable_field_data[0] == (UNIT_SEPERATOR)))
                    {
                        // Split this into subfields
                        string[] subfields = variable_field_data.Substring(1).Split(new[] {UNIT_SEPERATOR});

                        // Create the new field
                        MARC_Field newField = new MARC_Field {Tag = Convert.ToInt32(directoryEntry.Tag), Indicators = indicator};

                        // Step through each subfield
                        foreach (string thisSubfield in subfields)
                        {
                            // Add this subfield
                            newField.Add_Subfield(thisSubfield[0], thisSubfield.Substring(1));
                        }

                        // Add this entry to the current record
                        thisRecord.Add_Field(newField);
                    }
                    else
                    {
                        // Must be just one subfield
                        thisRecord.Add_Field(Convert.ToInt32(directoryEntry.Tag), variable_field_data);
                    }
                }

                // if this was MARC8 encoding originally, change the encoding specified in the 
                // leader, since this was converted to Unicode
                if (encoding == Record_Character_Encoding.MARC)
                {
                    thisRecord.Leader = thisRecord.Leader.Substring(0, 9) + "a" + thisRecord.Leader.Substring(10);
                }
            }
            catch (EndOfStreamException)
            {
                if (Action_On_Error == Action_On_Error_Encountered_Enum.STORE_IN_RECORD)
                    thisRecord.Add_Error(MARC_Record_Parsing_Error_Type_Enum.Unexpected_End_Of_Stream_Encountered);
                else
                    throw new ApplicationException("Unexpected end of stream encountered!  Input stream may be invalid format or truncated.");
            }

            return thisRecord;
        }

        #endregion

        #region Methods used for converting MARC character encoded fields to Unicode 

        private static string Convert_Marc_Bytes_To_Unicode_String(MARC_Record thisRecord, byte[] input)
        {
            string fieldAsString = Encoding.UTF8.GetString(input);

            int marcByte1 = -1;
            int marcByte2 = -1;
            int marcByte3 = -1;

            // Create the string builder to build the array
            StringBuilder builder = new StringBuilder(input.Length + 5);

            // Step through all the bytes in the array
            for (int i = 0; i < input.Length; i++)
            {
                // If any previous bytes, save them
                marcByte3 = marcByte2;
                marcByte2 = marcByte1;

                // Get this byte frmo the array
                marcByte1 = (int) input[i];

                // Try to convert the current byte to unicode character
                if (Append_Unicode_Character(thisRecord, builder, marcByte1, marcByte2, marcByte3))
                {
                    // Since the bytes were handled, clear them
                    marcByte1 = -1;
                    marcByte2 = -1;
                }
            }

            // Return the string
            return builder.ToString();
        }

        private static bool Append_Unicode_Character(MARC_Record thisRecord, StringBuilder stringBuilder, int marcByte1, int marcByte2, int marcByte3)
        {
            // Check to see if an alternate character set is present
            if (marcByte1 == ALTERNATE_CHARACTER_SET_INDICATOR)
            {
                thisRecord.Add_Warning(MARC_Record_Parsing_Warning_Type_Enum.Alternate_Character_Set_Present);
            }

            // For the special characters in MARC encoding, return FALSE, 
            // indicating the byte was not yet handled. (Need the next byte(s))
            if (marcByte1 >= 224)
                return false;

            // Case where there is only one byte to handle (and not a special case returned already from above lines)
            if ((marcByte2 == -1) && (marcByte3 == -1))
            {
                stringBuilder.Append((char) marcByte1);
                return true;
            }

            // Is this just a two byte combination?
            if (marcByte3 == -1)
            {
                if (marcByte2 != -1)
                {
                    if (marcByte2 == 224)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                stringBuilder.Append((char) 0x1EA2);
                                return true;
                            case 69:
                                stringBuilder.Append((char) 0x1EBA);
                                return true;
                            case 73:
                                stringBuilder.Append((char) 0x1EC8);
                                return true;
                            case 79:
                                stringBuilder.Append((char) 0x1ECE);
                                return true;
                            case 85:
                                stringBuilder.Append((char) 0x1EE6);
                                return true;
                            case 89:
                                stringBuilder.Append((char) 0x1EF6);
                                return true;
                            case 97:
                                stringBuilder.Append((char) 0x1EA3);
                                return true;
                            case 101:
                                stringBuilder.Append((char) 0x1EBB);
                                return true;
                            case 105:
                                stringBuilder.Append((char) 0x1EC9);
                                return true;
                            case 111:
                                stringBuilder.Append((char) 0x1ECF);
                                return true;
                            case 117:
                                stringBuilder.Append((char) 0x1EE7);
                                return true;
                            case 121:
                                stringBuilder.Append((char) 0x1EF7);
                                return true;
                            case 172:
                                stringBuilder.Append((char) 0x1EDE);
                                return true;
                            case 173:
                                stringBuilder.Append((char) 0x1EEC);
                                return true;
                            case 188:
                                stringBuilder.Append((char) 0x1EDF);
                                return true;
                            case 189:
                                stringBuilder.Append((char) 0x1EED);
                                return true;
                        }
                    }
                    if (marcByte2 == 225)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                stringBuilder.Append((char) 0x00C0);
                                return true;
                            case 69:
                                stringBuilder.Append((char) 0x00C8);
                                return true;
                            case 73:
                                stringBuilder.Append((char) 0x00CC);
                                return true;
                            case 78:
                                stringBuilder.Append((char) 0x01F8);
                                return true;
                            case 79:
                                stringBuilder.Append((char) 0x00D2);
                                return true;
                            case 85:
                                stringBuilder.Append((char) 0x00D9);
                                return true;
                            case 87:
                                stringBuilder.Append((char) 0x1E80);
                                return true;
                            case 89:
                                stringBuilder.Append((char) 0x1EF2);
                                return true;
                            case 97:
                                stringBuilder.Append((char) 0x00E0);
                                return true;
                            case 101:
                                stringBuilder.Append((char) 0x00E8);
                                return true;
                            case 105:
                                stringBuilder.Append((char) 0x00EC);
                                return true;
                            case 110:
                                stringBuilder.Append((char) 0x01F9);
                                return true;
                            case 111:
                                stringBuilder.Append((char) 0x00F2);
                                return true;
                            case 117:
                                stringBuilder.Append((char) 0x00F9);
                                return true;
                            case 119:
                                stringBuilder.Append((char) 0x1E81);
                                return true;
                            case 121:
                                stringBuilder.Append((char) 0x1EF3);
                                return true;
                            case 172:
                                stringBuilder.Append((char) 0x1EDC);
                                return true;
                            case 173:
                                stringBuilder.Append((char) 0x1EEA);
                                return true;
                            case 188:
                                stringBuilder.Append((char) 0x1EDD);
                                return true;
                            case 189:
                                stringBuilder.Append((char) 0x1EEB);
                                return true;
                        }
                    }
                    if (marcByte2 == 226)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                stringBuilder.Append((char) 0x00C1);
                                return true;
                            case 67:
                                stringBuilder.Append((char) 0x0106);
                                return true;
                            case 69:
                                stringBuilder.Append((char) 0x00C9);
                                return true;
                            case 71:
                                stringBuilder.Append((char) 0x01F4);
                                return true;
                            case 73:
                                stringBuilder.Append((char) 0x00CD);
                                return true;
                            case 75:
                                stringBuilder.Append((char) 0x1E30);
                                return true;
                            case 76:
                                stringBuilder.Append((char) 0x0139);
                                return true;
                            case 77:
                                stringBuilder.Append((char) 0x1E3E);
                                return true;
                            case 78:
                                stringBuilder.Append((char) 0x0143);
                                return true;
                            case 79:
                                stringBuilder.Append((char) 0x00D3);
                                return true;
                            case 80:
                                stringBuilder.Append((char) 0x1E54);
                                return true;
                            case 82:
                                stringBuilder.Append((char) 0x0154);
                                return true;
                            case 83:
                                stringBuilder.Append((char) 0x015A);
                                return true;
                            case 85:
                                stringBuilder.Append((char) 0x00DA);
                                return true;
                            case 87:
                                stringBuilder.Append((char) 0x1E82);
                                return true;
                            case 89:
                                stringBuilder.Append((char) 0x00DD);
                                return true;
                            case 90:
                                stringBuilder.Append((char) 0x0179);
                                return true;
                            case 97:
                                stringBuilder.Append((char) 0x00E1);
                                return true;
                            case 99:
                                stringBuilder.Append((char) 0x0107);
                                return true;
                            case 101:
                                stringBuilder.Append((char) 0x00E9);
                                return true;
                            case 103:
                                stringBuilder.Append((char) 0x01F5);
                                return true;
                            case 105:
                                stringBuilder.Append((char) 0x00ED);
                                return true;
                            case 107:
                                stringBuilder.Append((char) 0x1E31);
                                return true;
                            case 108:
                                stringBuilder.Append((char) 0x013A);
                                return true;
                            case 109:
                                stringBuilder.Append((char) 0x1E3F);
                                return true;
                            case 110:
                                stringBuilder.Append((char) 0x0144);
                                return true;
                            case 111:
                                stringBuilder.Append((char) 0x00F3);
                                return true;
                            case 112:
                                stringBuilder.Append((char) 0x1E55);
                                return true;
                            case 114:
                                stringBuilder.Append((char) 0x0155);
                                return true;
                            case 115:
                                stringBuilder.Append((char) 0x015B);
                                return true;
                            case 117:
                                stringBuilder.Append((char) 0x00FA);
                                return true;
                            case 119:
                                stringBuilder.Append((char) 0x1E83);
                                return true;
                            case 121:
                                stringBuilder.Append((char) 0x00FD);
                                return true;
                            case 122:
                                stringBuilder.Append((char) 0x017A);
                                return true;
                            case 162:
                                stringBuilder.Append((char) 0x01FE);
                                return true;
                            case 165:
                                stringBuilder.Append((char) 0x01FC);
                                return true;
                            case 172:
                                stringBuilder.Append((char) 0x1EDA);
                                return true;
                            case 173:
                                stringBuilder.Append((char) 0x1EE8);
                                return true;
                            case 178:
                                stringBuilder.Append((char) 0x01FF);
                                return true;
                            case 181:
                                stringBuilder.Append((char) 0x01FD);
                                return true;
                            case 188:
                                stringBuilder.Append((char) 0x1EDB);
                                return true;
                            case 189:
                                stringBuilder.Append((char) 0x1EE9);
                                return true;
                            case 232:
                                stringBuilder.Append((char) 0x0344);
                                return true;
                        }
                    }
                    if (marcByte2 == 227)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                stringBuilder.Append((char) 0x00C2);
                                return true;
                            case 67:
                                stringBuilder.Append((char) 0x0108);
                                return true;
                            case 69:
                                stringBuilder.Append((char) 0x00CA);
                                return true;
                            case 71:
                                stringBuilder.Append((char) 0x011C);
                                return true;
                            case 72:
                                stringBuilder.Append((char) 0x0124);
                                return true;
                            case 73:
                                stringBuilder.Append((char) 0x00CE);
                                return true;
                            case 74:
                                stringBuilder.Append((char) 0x0134);
                                return true;
                            case 79:
                                stringBuilder.Append((char) 0x00D4);
                                return true;
                            case 83:
                                stringBuilder.Append((char) 0x015C);
                                return true;
                            case 85:
                                stringBuilder.Append((char) 0x00DB);
                                return true;
                            case 87:
                                stringBuilder.Append((char) 0x0174);
                                return true;
                            case 89:
                                stringBuilder.Append((char) 0x0176);
                                return true;
                            case 90:
                                stringBuilder.Append((char) 0x1E90);
                                return true;
                            case 97:
                                stringBuilder.Append((char) 0x00E2);
                                return true;
                            case 99:
                                stringBuilder.Append((char) 0x0109);
                                return true;
                            case 101:
                                stringBuilder.Append((char) 0x00EA);
                                return true;
                            case 103:
                                stringBuilder.Append((char) 0x011D);
                                return true;
                            case 104:
                                stringBuilder.Append((char) 0x0125);
                                return true;
                            case 105:
                                stringBuilder.Append((char) 0x00EE);
                                return true;
                            case 106:
                                stringBuilder.Append((char) 0x0135);
                                return true;
                            case 111:
                                stringBuilder.Append((char) 0x00F4);
                                return true;
                            case 115:
                                stringBuilder.Append((char) 0x015D);
                                return true;
                            case 117:
                                stringBuilder.Append((char) 0x00FB);
                                return true;
                            case 119:
                                stringBuilder.Append((char) 0x0175);
                                return true;
                            case 121:
                                stringBuilder.Append((char) 0x0177);
                                return true;
                            case 122:
                                stringBuilder.Append((char) 0x1E91);
                                return true;
                        }
                    }
                    if (marcByte2 == 228)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                stringBuilder.Append((char) 0x00C3);
                                return true;
                            case 69:
                                stringBuilder.Append((char) 0x1EBC);
                                return true;
                            case 73:
                                stringBuilder.Append((char) 0x0128);
                                return true;
                            case 78:
                                stringBuilder.Append((char) 0x00D1);
                                return true;
                            case 79:
                                stringBuilder.Append((char) 0x00D5);
                                return true;
                            case 85:
                                stringBuilder.Append((char) 0x0168);
                                return true;
                            case 86:
                                stringBuilder.Append((char) 0x1E7C);
                                return true;
                            case 89:
                                stringBuilder.Append((char) 0x1EF8);
                                return true;
                            case 97:
                                stringBuilder.Append((char) 0x00E3);
                                return true;
                            case 101:
                                stringBuilder.Append((char) 0x1EBD);
                                return true;
                            case 105:
                                stringBuilder.Append((char) 0x0129);
                                return true;
                            case 110:
                                stringBuilder.Append((char) 0x00F1);
                                return true;
                            case 111:
                                stringBuilder.Append((char) 0x00F5);
                                return true;
                            case 117:
                                stringBuilder.Append((char) 0x0169);
                                return true;
                            case 118:
                                stringBuilder.Append((char) 0x1E7D);
                                return true;
                            case 121:
                                stringBuilder.Append((char) 0x1EF9);
                                return true;
                            case 172:
                                stringBuilder.Append((char) 0x1EE0);
                                return true;
                            case 173:
                                stringBuilder.Append((char) 0x1EEE);
                                return true;
                            case 188:
                                stringBuilder.Append((char) 0x1EE1);
                                return true;
                            case 189:
                                stringBuilder.Append((char) 0x1EEF);
                                return true;
                        }
                    }
                    if (marcByte2 == 229)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                stringBuilder.Append((char) 0x0100);
                                return true;
                            case 69:
                                stringBuilder.Append((char) 0x0112);
                                return true;
                            case 71:
                                stringBuilder.Append((char) 0x1E20);
                                return true;
                            case 73:
                                stringBuilder.Append((char) 0x012A);
                                return true;
                            case 79:
                                stringBuilder.Append((char) 0x014C);
                                return true;
                            case 85:
                                stringBuilder.Append((char) 0x016A);
                                return true;
                            case 89:
                                stringBuilder.Append((char) 0x0232);
                                return true;
                            case 97:
                                stringBuilder.Append((char) 0x0101);
                                return true;
                            case 101:
                                stringBuilder.Append((char) 0x0113);
                                return true;
                            case 103:
                                stringBuilder.Append((char) 0x1E21);
                                return true;
                            case 105:
                                stringBuilder.Append((char) 0x012B);
                                return true;
                            case 111:
                                stringBuilder.Append((char) 0x014D);
                                return true;
                            case 117:
                                stringBuilder.Append((char) 0x016B);
                                return true;
                            case 121:
                                stringBuilder.Append((char) 0x0233);
                                return true;
                            case 165:
                                stringBuilder.Append((char) 0x01E2);
                                return true;
                            case 181:
                                stringBuilder.Append((char) 0x01E3);
                                return true;
                        }
                    }
                    if (marcByte2 == 230)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                stringBuilder.Append((char) 0x0102);
                                return true;
                            case 69:
                                stringBuilder.Append((char) 0x0114);
                                return true;
                            case 71:
                                stringBuilder.Append((char) 0x011E);
                                return true;
                            case 73:
                                stringBuilder.Append((char) 0x012C);
                                return true;
                            case 79:
                                stringBuilder.Append((char) 0x014E);
                                return true;
                            case 85:
                                stringBuilder.Append((char) 0x016C);
                                return true;
                            case 97:
                                stringBuilder.Append((char) 0x0103);
                                return true;
                            case 101:
                                stringBuilder.Append((char) 0x0115);
                                return true;
                            case 103:
                                stringBuilder.Append((char) 0x011F);
                                return true;
                            case 105:
                                stringBuilder.Append((char) 0x012D);
                                return true;
                            case 111:
                                stringBuilder.Append((char) 0x014F);
                                return true;
                            case 117:
                                stringBuilder.Append((char) 0x016D);
                                return true;
                        }
                    }
                    if (marcByte2 == 231)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                stringBuilder.Append((char) 0x0226);
                                return true;
                            case 66:
                                stringBuilder.Append((char) 0x1E02);
                                return true;
                            case 67:
                                stringBuilder.Append((char) 0x010A);
                                return true;
                            case 68:
                                stringBuilder.Append((char) 0x1E0A);
                                return true;
                            case 69:
                                stringBuilder.Append((char) 0x0116);
                                return true;
                            case 70:
                                stringBuilder.Append((char) 0x1E1E);
                                return true;
                            case 71:
                                stringBuilder.Append((char) 0x0120);
                                return true;
                            case 72:
                                stringBuilder.Append((char) 0x1E22);
                                return true;
                            case 73:
                                stringBuilder.Append((char) 0x0130);
                                return true;
                            case 77:
                                stringBuilder.Append((char) 0x1E40);
                                return true;
                            case 78:
                                stringBuilder.Append((char) 0x1E44);
                                return true;
                            case 79:
                                stringBuilder.Append((char) 0x022E);
                                return true;
                            case 80:
                                stringBuilder.Append((char) 0x1E56);
                                return true;
                            case 82:
                                stringBuilder.Append((char) 0x1E58);
                                return true;
                            case 83:
                                stringBuilder.Append((char) 0x1E60);
                                return true;
                            case 84:
                                stringBuilder.Append((char) 0x1E6A);
                                return true;
                            case 87:
                                stringBuilder.Append((char) 0x1E86);
                                return true;
                            case 88:
                                stringBuilder.Append((char) 0x1E8A);
                                return true;
                            case 89:
                                stringBuilder.Append((char) 0x1E8E);
                                return true;
                            case 90:
                                stringBuilder.Append((char) 0x017B);
                                return true;
                            case 97:
                                stringBuilder.Append((char) 0x0227);
                                return true;
                            case 98:
                                stringBuilder.Append((char) 0x1E03);
                                return true;
                            case 99:
                                stringBuilder.Append((char) 0x010B);
                                return true;
                            case 100:
                                stringBuilder.Append((char) 0x1E0B);
                                return true;
                            case 101:
                                stringBuilder.Append((char) 0x0117);
                                return true;
                            case 102:
                                stringBuilder.Append((char) 0x1E1F);
                                return true;
                            case 103:
                                stringBuilder.Append((char) 0x0121);
                                return true;
                            case 104:
                                stringBuilder.Append((char) 0x1E23);
                                return true;
                            case 109:
                                stringBuilder.Append((char) 0x1E41);
                                return true;
                            case 110:
                                stringBuilder.Append((char) 0x1E45);
                                return true;
                            case 111:
                                stringBuilder.Append((char) 0x022F);
                                return true;
                            case 112:
                                stringBuilder.Append((char) 0x1E57);
                                return true;
                            case 114:
                                stringBuilder.Append((char) 0x1E59);
                                return true;
                            case 115:
                                stringBuilder.Append((char) 0x1E61);
                                return true;
                            case 116:
                                stringBuilder.Append((char) 0x1E6B);
                                return true;
                            case 119:
                                stringBuilder.Append((char) 0x1E87);
                                return true;
                            case 120:
                                stringBuilder.Append((char) 0x1E8B);
                                return true;
                            case 121:
                                stringBuilder.Append((char) 0x1E8F);
                                return true;
                            case 122:
                                stringBuilder.Append((char) 0x017C);
                                return true;
                        }
                    }
                    if (marcByte2 == 232)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                stringBuilder.Append((char) 0x00C4);
                                return true;
                            case 69:
                                stringBuilder.Append((char) 0x00CB);
                                return true;
                            case 72:
                                stringBuilder.Append((char) 0x1E26);
                                return true;
                            case 73:
                                stringBuilder.Append((char) 0x00CF);
                                return true;
                            case 79:
                                stringBuilder.Append((char) 0x00D6);
                                return true;
                            case 85:
                                stringBuilder.Append((char) 0x00DC);
                                return true;
                            case 87:
                                stringBuilder.Append((char) 0x1E84);
                                return true;
                            case 88:
                                stringBuilder.Append((char) 0x1E8C);
                                return true;
                            case 89:
                                stringBuilder.Append((char) 0x0178);
                                return true;
                            case 97:
                                stringBuilder.Append((char) 0x00E4);
                                return true;
                            case 101:
                                stringBuilder.Append((char) 0x00EB);
                                return true;
                            case 104:
                                stringBuilder.Append((char) 0x1E27);
                                return true;
                            case 105:
                                stringBuilder.Append((char) 0x00EF);
                                return true;
                            case 111:
                                stringBuilder.Append((char) 0x00F6);
                                return true;
                            case 116:
                                stringBuilder.Append((char) 0x1E97);
                                return true;
                            case 117:
                                stringBuilder.Append((char) 0x00FC);
                                return true;
                            case 119:
                                stringBuilder.Append((char) 0x1E85);
                                return true;
                            case 120:
                                stringBuilder.Append((char) 0x1E8D);
                                return true;
                            case 121:
                                stringBuilder.Append((char) 0x00FF);
                                return true;
                        }
                    }
                    if (marcByte2 == 233)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                stringBuilder.Append((char) 0x01CD);
                                return true;
                            case 67:
                                stringBuilder.Append((char) 0x010C);
                                return true;
                            case 68:
                                stringBuilder.Append((char) 0x010E);
                                return true;
                            case 69:
                                stringBuilder.Append((char) 0x011A);
                                return true;
                            case 71:
                                stringBuilder.Append((char) 0x01E6);
                                return true;
                            case 72:
                                stringBuilder.Append((char) 0x021E);
                                return true;
                            case 73:
                                stringBuilder.Append((char) 0x01CF);
                                return true;
                            case 75:
                                stringBuilder.Append((char) 0x01E8);
                                return true;
                            case 76:
                                stringBuilder.Append((char) 0x013D);
                                return true;
                            case 78:
                                stringBuilder.Append((char) 0x0147);
                                return true;
                            case 79:
                                stringBuilder.Append((char) 0x01D1);
                                return true;
                            case 82:
                                stringBuilder.Append((char) 0x0158);
                                return true;
                            case 83:
                                stringBuilder.Append((char) 0x0160);
                                return true;
                            case 84:
                                stringBuilder.Append((char) 0x0164);
                                return true;
                            case 85:
                                stringBuilder.Append((char) 0x01D3);
                                return true;
                            case 90:
                                stringBuilder.Append((char) 0x017D);
                                return true;
                            case 97:
                                stringBuilder.Append((char) 0x01CE);
                                return true;
                            case 99:
                                stringBuilder.Append((char) 0x010D);
                                return true;
                            case 100:
                                stringBuilder.Append((char) 0x010F);
                                return true;
                            case 101:
                                stringBuilder.Append((char) 0x011B);
                                return true;
                            case 103:
                                stringBuilder.Append((char) 0x01E7);
                                return true;
                            case 104:
                                stringBuilder.Append((char) 0x021F);
                                return true;
                            case 105:
                                stringBuilder.Append((char) 0x01D0);
                                return true;
                            case 106:
                                stringBuilder.Append((char) 0x01F0);
                                return true;
                            case 107:
                                stringBuilder.Append((char) 0x01E9);
                                return true;
                            case 108:
                                stringBuilder.Append((char) 0x013E);
                                return true;
                            case 110:
                                stringBuilder.Append((char) 0x0148);
                                return true;
                            case 111:
                                stringBuilder.Append((char) 0x01D2);
                                return true;
                            case 114:
                                stringBuilder.Append((char) 0x0159);
                                return true;
                            case 115:
                                stringBuilder.Append((char) 0x0161);
                                return true;
                            case 116:
                                stringBuilder.Append((char) 0x0165);
                                return true;
                            case 117:
                                stringBuilder.Append((char) 0x01D4);
                                return true;
                            case 122:
                                stringBuilder.Append((char) 0x017E);
                                return true;
                        }
                    }
                    if (marcByte2 == 234)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                stringBuilder.Append((char) 0x00C5);
                                return true;
                            case 85:
                                stringBuilder.Append((char) 0x016E);
                                return true;
                            case 97:
                                stringBuilder.Append((char) 0x00E5);
                                return true;
                            case 117:
                                stringBuilder.Append((char) 0x016F);
                                return true;
                            case 119:
                                stringBuilder.Append((char) 0x1E98);
                                return true;
                            case 121:
                                stringBuilder.Append((char) 0x1E99);
                                return true;
                        }
                    }
                    if (marcByte2 == 238)
                    {
                        switch (marcByte1)
                        {
                            case 79:
                                stringBuilder.Append((char) 0x0150);
                                return true;
                            case 85:
                                stringBuilder.Append((char) 0x0170);
                                return true;
                            case 111:
                                stringBuilder.Append((char) 0x0151);
                                return true;
                            case 117:
                                stringBuilder.Append((char) 0x0171);
                                return true;
                        }
                    }
                    if (marcByte2 == 240)
                    {
                        switch (marcByte1)
                        {
                            case 67:
                                stringBuilder.Append((char) 0x00C7);
                                return true;
                            case 68:
                                stringBuilder.Append((char) 0x1E10);
                                return true;
                            case 69:
                                stringBuilder.Append((char) 0x0228);
                                return true;
                            case 71:
                                stringBuilder.Append((char) 0x0122);
                                return true;
                            case 72:
                                stringBuilder.Append((char) 0x1E28);
                                return true;
                            case 75:
                                stringBuilder.Append((char) 0x0136);
                                return true;
                            case 76:
                                stringBuilder.Append((char) 0x013B);
                                return true;
                            case 78:
                                stringBuilder.Append((char) 0x0145);
                                return true;
                            case 82:
                                stringBuilder.Append((char) 0x0156);
                                return true;
                            case 83:
                                stringBuilder.Append((char) 0x015E);
                                return true;
                            case 84:
                                stringBuilder.Append((char) 0x0162);
                                return true;
                            case 99:
                                stringBuilder.Append((char) 0x00E7);
                                return true;
                            case 100:
                                stringBuilder.Append((char) 0x1E11);
                                return true;
                            case 101:
                                stringBuilder.Append((char) 0x0229);
                                return true;
                            case 103:
                                stringBuilder.Append((char) 0x0123);
                                return true;
                            case 104:
                                stringBuilder.Append((char) 0x1E29);
                                return true;
                            case 107:
                                stringBuilder.Append((char) 0x0137);
                                return true;
                            case 108:
                                stringBuilder.Append((char) 0x013C);
                                return true;
                            case 110:
                                stringBuilder.Append((char) 0x0146);
                                return true;
                            case 114:
                                stringBuilder.Append((char) 0x0157);
                                return true;
                            case 115:
                                stringBuilder.Append((char) 0x015F);
                                return true;
                            case 116:
                                stringBuilder.Append((char) 0x0163);
                                return true;
                        }
                    }
                    if (marcByte2 == 241)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                stringBuilder.Append((char) 0x0104);
                                return true;
                            case 69:
                                stringBuilder.Append((char) 0x0118);
                                return true;
                            case 73:
                                stringBuilder.Append((char) 0x012E);
                                return true;
                            case 79:
                                stringBuilder.Append((char) 0x01EA);
                                return true;
                            case 85:
                                stringBuilder.Append((char) 0x0172);
                                return true;
                            case 97:
                                stringBuilder.Append((char) 0x0105);
                                return true;
                            case 101:
                                stringBuilder.Append((char) 0x0119);
                                return true;
                            case 105:
                                stringBuilder.Append((char) 0x012F);
                                return true;
                            case 111:
                                stringBuilder.Append((char) 0x01EB);
                                return true;
                            case 117:
                                stringBuilder.Append((char) 0x0173);
                                return true;
                        }
                    }
                    if (marcByte2 == 242)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                stringBuilder.Append((char) 0x1EA0);
                                return true;
                            case 66:
                                stringBuilder.Append((char) 0x1E04);
                                return true;
                            case 68:
                                stringBuilder.Append((char) 0x1E0C);
                                return true;
                            case 69:
                                stringBuilder.Append((char) 0x1EB8);
                                return true;
                            case 72:
                                stringBuilder.Append((char) 0x1E24);
                                return true;
                            case 73:
                                stringBuilder.Append((char) 0x1ECA);
                                return true;
                            case 75:
                                stringBuilder.Append((char) 0x1E32);
                                return true;
                            case 76:
                                stringBuilder.Append((char) 0x1E36);
                                return true;
                            case 77:
                                stringBuilder.Append((char) 0x1E42);
                                return true;
                            case 78:
                                stringBuilder.Append((char) 0x1E46);
                                return true;
                            case 79:
                                stringBuilder.Append((char) 0x1ECC);
                                return true;
                            case 82:
                                stringBuilder.Append((char) 0x1E5A);
                                return true;
                            case 83:
                                stringBuilder.Append((char) 0x1E62);
                                return true;
                            case 84:
                                stringBuilder.Append((char) 0x1E6C);
                                return true;
                            case 85:
                                stringBuilder.Append((char) 0x1EE4);
                                return true;
                            case 86:
                                stringBuilder.Append((char) 0x1E7E);
                                return true;
                            case 87:
                                stringBuilder.Append((char) 0x1E88);
                                return true;
                            case 89:
                                stringBuilder.Append((char) 0x1EF4);
                                return true;
                            case 90:
                                stringBuilder.Append((char) 0x1E92);
                                return true;
                            case 97:
                                stringBuilder.Append((char) 0x1EA1);
                                return true;
                            case 98:
                                stringBuilder.Append((char) 0x1E05);
                                return true;
                            case 100:
                                stringBuilder.Append((char) 0x1E0D);
                                return true;
                            case 101:
                                stringBuilder.Append((char) 0x1EB9);
                                return true;
                            case 104:
                                stringBuilder.Append((char) 0x1E25);
                                return true;
                            case 105:
                                stringBuilder.Append((char) 0x1ECB);
                                return true;
                            case 107:
                                stringBuilder.Append((char) 0x1E33);
                                return true;
                            case 108:
                                stringBuilder.Append((char) 0x1E37);
                                return true;
                            case 109:
                                stringBuilder.Append((char) 0x1E43);
                                return true;
                            case 110:
                                stringBuilder.Append((char) 0x1E47);
                                return true;
                            case 111:
                                stringBuilder.Append((char) 0x1ECD);
                                return true;
                            case 114:
                                stringBuilder.Append((char) 0x1E5B);
                                return true;
                            case 115:
                                stringBuilder.Append((char) 0x1E63);
                                return true;
                            case 116:
                                stringBuilder.Append((char) 0x1E6D);
                                return true;
                            case 117:
                                stringBuilder.Append((char) 0x1EE5);
                                return true;
                            case 118:
                                stringBuilder.Append((char) 0x1E7F);
                                return true;
                            case 119:
                                stringBuilder.Append((char) 0x1E89);
                                return true;
                            case 121:
                                stringBuilder.Append((char) 0x1EF5);
                                return true;
                            case 122:
                                stringBuilder.Append((char) 0x1E93);
                                return true;
                            case 172:
                                stringBuilder.Append((char) 0x1EE2);
                                return true;
                            case 173:
                                stringBuilder.Append((char) 0x1EF0);
                                return true;
                            case 188:
                                stringBuilder.Append((char) 0x1EE3);
                                return true;
                            case 189:
                                stringBuilder.Append((char) 0x1EF1);
                                return true;
                        }
                    }
                    if (marcByte2 == 243)
                    {
                        switch (marcByte1)
                        {
                            case 85:
                                stringBuilder.Append((char) 0x1E72);
                                return true;
                            case 117:
                                stringBuilder.Append((char) 0x1E73);
                                return true;
                        }
                    }
                    if (marcByte2 == 244)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                stringBuilder.Append((char) 0x1E00);
                                return true;
                            case 97:
                                stringBuilder.Append((char) 0x1E01);
                                return true;
                        }
                    }
                    if (marcByte2 == 247)
                    {
                        switch (marcByte1)
                        {
                            case 83:
                                stringBuilder.Append((char) 0x0218);
                                return true;
                            case 84:
                                stringBuilder.Append((char) 0x021A);
                                return true;
                            case 115:
                                stringBuilder.Append((char) 0x0219);
                                return true;
                            case 116:
                                stringBuilder.Append((char) 0x021B);
                                return true;
                        }
                    }
                    if (marcByte2 == 249)
                    {
                        switch (marcByte1)
                        {
                            case 72:
                                stringBuilder.Append((char) 0x1E2A);
                                return true;
                            case 104:
                                stringBuilder.Append((char) 0x1E2B);
                                return true;
                        }
                    }
                }

                return false;
            }
            else // This is a THREE byte combination
            {
                if (marcByte3 == 224)
                {
                    switch (marcByte2)
                    {
                        case 227:
                            switch (marcByte1)
                            {
                                case 65:
                                    stringBuilder.Append((char) 0x1EA8);
                                    return true;
                                case 69:
                                    stringBuilder.Append((char) 0x1EC2);
                                    return true;
                                case 79:
                                    stringBuilder.Append((char) 0x1ED4);
                                    return true;
                                case 97:
                                    stringBuilder.Append((char) 0x1EA9);
                                    return true;
                                case 101:
                                    stringBuilder.Append((char) 0x1EC3);
                                    return true;
                                case 111:
                                    stringBuilder.Append((char) 0x1ED5);
                                    return true;
                            }
                            break;
                        case 230:
                            switch (marcByte1)
                            {
                                case 65:
                                    stringBuilder.Append((char) 0x1EB2);
                                    return true;
                                case 97:
                                    stringBuilder.Append((char) 0x1EB3);
                                    return true;
                            }
                            break;
                    }
                }

                if (marcByte3 == 225)
                {
                    switch (marcByte2)
                    {
                        case 227:
                            switch (marcByte1)
                            {
                                case 65:
                                    stringBuilder.Append((char) 0x1EA6);
                                    return true;
                                case 69:
                                    stringBuilder.Append((char) 0x1EC0);
                                    return true;
                                case 79:
                                    stringBuilder.Append((char) 0x1ED2);
                                    return true;
                                case 97:
                                    stringBuilder.Append((char) 0x1EA7);
                                    return true;
                                case 101:
                                    stringBuilder.Append((char) 0x1EC1);
                                    return true;
                                case 111:
                                    stringBuilder.Append((char) 0x1ED3);
                                    return true;
                            }
                            break;
                        case 229:
                            switch (marcByte1)
                            {
                                case 69:
                                    stringBuilder.Append((char) 0x1E14);
                                    return true;
                                case 79:
                                    stringBuilder.Append((char) 0x1E50);
                                    return true;
                                case 101:
                                    stringBuilder.Append((char) 0x1E15);
                                    return true;
                                case 111:
                                    stringBuilder.Append((char) 0x1E51);
                                    return true;
                            }
                            break;
                        case 230:
                            switch (marcByte1)
                            {
                                case 65:
                                    stringBuilder.Append((char) 0x1EB0);
                                    return true;
                                case 97:
                                    stringBuilder.Append((char) 0x1EB1);
                                    return true;
                            }
                            break;
                        case 232:
                            switch (marcByte1)
                            {
                                case 85:
                                    stringBuilder.Append((char) 0x01DB);
                                    return true;
                                case 117:
                                    stringBuilder.Append((char) 0x01DC);
                                    return true;
                            }
                            break;
                    }
                }
                if (marcByte3 == 226)
                {
                    switch (marcByte2)
                    {
                        case 227:
                            switch (marcByte1)
                            {
                                case 65:
                                    stringBuilder.Append((char) 0x1EA4);
                                    return true;
                                case 69:
                                    stringBuilder.Append((char) 0x1EBE);
                                    return true;
                                case 79:
                                    stringBuilder.Append((char) 0x1ED0);
                                    return true;
                                case 97:
                                    stringBuilder.Append((char) 0x1EA5);
                                    return true;
                                case 101:
                                    stringBuilder.Append((char) 0x1EBF);
                                    return true;
                                case 111:
                                    stringBuilder.Append((char) 0x1ED1);
                                    return true;
                            }
                            break;
                        case 228:
                            switch (marcByte1)
                            {
                                case 79:
                                    stringBuilder.Append((char) 0x1E4C);
                                    return true;
                                case 85:
                                    stringBuilder.Append((char) 0x1E78);
                                    return true;
                                case 111:
                                    stringBuilder.Append((char) 0x1E4D);
                                    return true;
                                case 117:
                                    stringBuilder.Append((char) 0x1E79);
                                    return true;
                            }
                            break;
                        case 229:
                            switch (marcByte1)
                            {
                                case 69:
                                    stringBuilder.Append((char) 0x1E16);
                                    return true;
                                case 79:
                                    stringBuilder.Append((char) 0x1E52);
                                    return true;
                                case 101:
                                    stringBuilder.Append((char) 0x1E17);
                                    return true;
                                case 111:
                                    stringBuilder.Append((char) 0x1E53);
                                    return true;
                            }
                            break;
                        case 230:
                            switch (marcByte1)
                            {
                                case 65:
                                    stringBuilder.Append((char) 0x1EAE);
                                    return true;
                                case 97:
                                    stringBuilder.Append((char) 0x1EAF);
                                    return true;
                            }
                            break;
                        case 232:
                            switch (marcByte1)
                            {
                                case 73:
                                    stringBuilder.Append((char) 0x1E2E);
                                    return true;
                                case 85:
                                    stringBuilder.Append((char) 0x01D7);
                                    return true;
                                case 105:
                                    stringBuilder.Append((char) 0x1E2F);
                                    return true;
                                case 117:
                                    stringBuilder.Append((char) 0x01D8);
                                    return true;
                            }
                            break;
                        case 234:
                            switch (marcByte1)
                            {
                                case 65:
                                    stringBuilder.Append((char) 0x01FA);
                                    return true;
                                case 97:
                                    stringBuilder.Append((char) 0x01FB);
                                    return true;
                            }
                            break;
                        case 240:
                            switch (marcByte1)
                            {
                                case 67:
                                    stringBuilder.Append((char) 0x1E08);
                                    return true;
                                case 99:
                                    stringBuilder.Append((char) 0x1E09);
                                    return true;
                            }
                            break;
                    }
                }
                if (marcByte3 == 227)
                {
                    switch (marcByte2)
                    {
                        case 242:
                            switch (marcByte1)
                            {
                                case 65:
                                    stringBuilder.Append((char) 0x1EAC);
                                    return true;
                                case 69:
                                    stringBuilder.Append((char) 0x1EC6);
                                    return true;
                                case 79:
                                    stringBuilder.Append((char) 0x1ED8);
                                    return true;
                                case 97:
                                    stringBuilder.Append((char) 0x1EAD);
                                    return true;
                                case 101:
                                    stringBuilder.Append((char) 0x1EC7);
                                    return true;
                                case 111:
                                    stringBuilder.Append((char) 0x1ED9);
                                    return true;
                            }
                            break;
                    }
                }
                if (marcByte3 == 228)
                {
                    switch (marcByte2)
                    {
                        case 227:
                            switch (marcByte1)
                            {
                                case 65:
                                    stringBuilder.Append((char) 0x1EAA);
                                    return true;
                                case 69:
                                    stringBuilder.Append((char) 0x1EC4);
                                    return true;
                                case 79:
                                    stringBuilder.Append((char) 0x1ED6);
                                    return true;
                                case 97:
                                    stringBuilder.Append((char) 0x1EAB);
                                    return true;
                                case 101:
                                    stringBuilder.Append((char) 0x1EC5);
                                    return true;
                                case 111:
                                    stringBuilder.Append((char) 0x1ED7);
                                    return true;
                            }
                            break;
                        case 230:
                            switch (marcByte1)
                            {
                                case 65:
                                    stringBuilder.Append((char) 0x1EB4);
                                    return true;
                                case 97:
                                    stringBuilder.Append((char) 0x1EB5);
                                    return true;
                            }
                            break;
                    }
                }
                if (marcByte3 == 229)
                {
                    switch (marcByte2)
                    {
                        case 228:
                            switch (marcByte1)
                            {
                                case 79:
                                    stringBuilder.Append((char) 0x022C);
                                    return true;
                                case 111:
                                    stringBuilder.Append((char) 0x022D);
                                    return true;
                            }
                            break;
                        case 231:
                            switch (marcByte1)
                            {
                                case 65:
                                    stringBuilder.Append((char) 0x01E0);
                                    return true;
                                case 79:
                                    stringBuilder.Append((char) 0x0230);
                                    return true;
                                case 97:
                                    stringBuilder.Append((char) 0x01E1);
                                    return true;
                                case 111:
                                    stringBuilder.Append((char) 0x0231);
                                    return true;
                            }
                            break;
                        case 232:
                            switch (marcByte1)
                            {
                                case 65:
                                    stringBuilder.Append((char) 0x01DE);
                                    return true;
                                case 79:
                                    stringBuilder.Append((char) 0x022A);
                                    return true;
                                case 85:
                                    stringBuilder.Append((char) 0x01D5);
                                    return true;
                                case 97:
                                    stringBuilder.Append((char) 0x01DF);
                                    return true;
                                case 111:
                                    stringBuilder.Append((char) 0x022B);
                                    return true;
                                case 117:
                                    stringBuilder.Append((char) 0x01D6);
                                    return true;
                            }
                            break;
                        case 241:
                            switch (marcByte1)
                            {
                                case 79:
                                    stringBuilder.Append((char) 0x01EC);
                                    return true;
                                case 111:
                                    stringBuilder.Append((char) 0x01ED);
                                    return true;
                            }
                            break;
                        case 242:
                            switch (marcByte1)
                            {
                                case 76:
                                    stringBuilder.Append((char) 0x1E38);
                                    return true;
                                case 82:
                                    stringBuilder.Append((char) 0x1E5C);
                                    return true;
                                case 108:
                                    stringBuilder.Append((char) 0x1E39);
                                    return true;
                                case 114:
                                    stringBuilder.Append((char) 0x1E5D);
                                    return true;
                            }
                            break;
                    }
                }
                if (marcByte3 == 230)
                {
                    switch (marcByte2)
                    {
                        case 240:
                            switch (marcByte1)
                            {
                                case 69:
                                    stringBuilder.Append((char) 0x1E1C);
                                    return true;
                                case 101:
                                    stringBuilder.Append((char) 0x1E1D);
                                    return true;
                            }
                            break;
                        case 242:
                            switch (marcByte1)
                            {
                                case 65:
                                    stringBuilder.Append((char) 0x1EB6);
                                    return true;
                                case 97:
                                    stringBuilder.Append((char) 0x1EB7);
                                    return true;
                            }
                            break;
                    }
                }
                if (marcByte3 == 231)
                {
                    switch (marcByte2)
                    {
                        case 226:
                            switch (marcByte1)
                            {
                                case 83:
                                    stringBuilder.Append((char) 0x1E64);
                                    return true;
                                case 115:
                                    stringBuilder.Append((char) 0x1E65);
                                    return true;
                            }
                            break;
                        case 233:
                            switch (marcByte1)
                            {
                                case 83:
                                    stringBuilder.Append((char) 0x1E66);
                                    return true;
                                case 115:
                                    stringBuilder.Append((char) 0x1E67);
                                    return true;
                            }
                            break;
                        case 242:
                            switch (marcByte1)
                            {
                                case 83:
                                    stringBuilder.Append((char) 0x1E68);
                                    return true;
                                case 115:
                                    stringBuilder.Append((char) 0x1E69);
                                    return true;
                            }
                            break;
                    }
                }
                if (marcByte3 == 232)
                {
                    switch (marcByte2)
                    {
                        case 228:
                            switch (marcByte1)
                            {
                                case 79:
                                    stringBuilder.Append((char) 0x1E4E);
                                    return true;
                                case 111:
                                    stringBuilder.Append((char) 0x1E4F);
                                    return true;
                            }
                            break;
                        case 229:
                            switch (marcByte1)
                            {
                                case 85:
                                    stringBuilder.Append((char) 0x1E7A);
                                    return true;
                                case 117:
                                    stringBuilder.Append((char) 0x1E7B);
                                    return true;
                            }
                            break;
                    }
                }
                if (marcByte3 == 233)
                {
                    switch (marcByte2)
                    {
                        case 232:
                            switch (marcByte1)
                            {
                                case 85:
                                    stringBuilder.Append((char) 0x01D9);
                                    return true;
                                case 117:
                                    stringBuilder.Append((char) 0x01DA);
                                    return true;
                            }
                            break;
                    }
                }

                // Since this is a three byte combination, just need to handle it SOMEHOW
                // before the third byte is lost, or any introduced problem gets compounded.
                // So, default this to just Unicode encoding.
                stringBuilder.Append(Encoding.UTF8.GetString(new byte[] {(byte) marcByte3, (byte) marcByte2, (byte) marcByte1}));
                return true;
            }
        }

        #endregion

        #region Methods implementing IDisposable

        /// <summary> Close any open streams which may remain </summary>
        /// <remarks> Required to implement IDisposable </remarks>
        void IDisposable.Dispose()
        {
            Close();
        }

        /// <summary> Close any open streams which may remain </summary>
        /// <remarks> Required to implement IDisposable </remarks>
        public void Dispose()
        {
            Close();
        }

        #endregion

        #region Methods implementing IEnumerator

        /// <summary> Gets the IEnumerator for this (itself) </summary>
        /// <returns></returns>
        /// <remarks> Required to implement IEnumerator </remarks>
        public IEnumerator<MARC_Record> GetEnumerator()
        {
            return this;
        }

        /// <summary> Gets the IEnumerator for this (itself) </summary>
        /// <returns></returns>
        /// <remarks> Required to implement IEnumerator </remarks>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        #endregion

        #region Methods implementing IEnumerable

        /// <summary> Returns the current record </summary>
        /// <remarks> Required to implement IEnumerable </remarks>
        public MARC_Record Current { get; private set; }

        /// <summary> Returns the current record </summary>
        /// <remarks> Required to implement IEnumerable </remarks>
        object IEnumerator.Current
        {
            get { return Current; }
        }

        /// <summary> Moves to the next record, and returns TRUE if one existed </summary>
        /// <returns> TRUE if another record was found, otherwise FALSE </returns>
        /// <remarks> Required to implement IEnumerable </remarks>
        public bool MoveNext()
        {
            Current = parse_next_record();
            if ((Current == null) || (EOF_Flag))
                return false;
            return true;
        }

        /// <summary> Resets the base stream, if that is possible </summary>
        /// <remarks> Required to implement IEnumerable </remarks>
        public void Reset()
        {
            if (reader.BaseStream.CanSeek)
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        #endregion
    }
}