using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SobekCM.Bib_Package.MARC
{
	/// <summary> Parser steps through the records in a MARC21 Electronic Format file. <br /> <br /> </summary>
	/// <remarks> Written by Mark Sullivan (2005) </remarks>
	public class MARC21_Exchange_Format_Parser
	{

		private StreamReader reader = null;
		private const int Group_Seperator = 29;
		private const int Record_Seperator = 30;
		private const int Unit_Seperator = 31;

        private bool eof_flag = false;

		/// <summary> Constructor for a new instance of this class </summary>
		public MARC21_Exchange_Format_Parser()
		{
			// No work done here
		}

		/// <summary> Begins parsing a new MARC21 Electronic format file. </summary>
		/// <param name="fileName"> Name of the file to parse </param>
		/// <returns> A built record </returns>
		public MARC_Record Parse( string fileName )
		{
            // Create the new reader object
			reader = new StreamReader( fileName, System.Text.Encoding.UTF8 );

			// Return the first record
			return parse_next_record();
		}

		/// <summary> Moves to the next record in the MARC21 Electronic format tile </summary>
		/// <returns> Next object, or null </returns>
		public MARC_Record Next()
		{
			if ( reader != null )
				return parse_next_record();
			else
				return null;
		}
        
		private MARC_Record parse_next_record()
		{
			// Create the MARC record to return and subfield collection
			MARC_Record thisRecord = new MARC_Record();

			// Keep track of special characters.  These count as TWO characters to the 
			// streamer, but only one to the MARC21_Exchange format
			int unicode_chars = 0;

			// Create the StringBuilder object for this record
			StringBuilder thisRecordBuilder = new StringBuilder();
			
			// Read to first character
			int result = reader.Read();

			// Constinue until the EOF is reached, or the Group seperator
			while (( result > 0 ) && ( result != Group_Seperator ))
			{
				// Save this character
				thisRecordBuilder.Append( (char) result );

				// Read the next character
				result = reader.Read();
			}

			// Split this by the record seperator, to find the end of the directory
			string recordAsString = thisRecordBuilder.ToString();

			// If this is the empty string, then just return null (DONE!)
			if ( recordAsString.Length < 3 )
			{
				//return null;

                //set flag to indicate that the EOF has been reached
                EOF_Flag = true;

                // return a null value to end file processing of the MARC file
                return null;
			}

			int first_record_seperator = recordAsString.IndexOf( (char) Record_Seperator );
			if ( first_record_seperator < 0 )
			{
                throw new ApplicationException("Invalid Record Detected!" );
			}

			// Get the combined leader and directory, and then the variable fields
			string leader_and_directory = recordAsString.Substring( 0, first_record_seperator );
			string variable_fields = recordAsString.Substring( first_record_seperator );

			// Make sure the leader and directory length is correct
			if ( leader_and_directory.Length < 24 )
			{
                throw new ApplicationException("Invalid Leader and Directory Length!");
            }

			// Seperate the leader and directory
			thisRecord.Leader = leader_and_directory.Substring(0,24);
			string directory = leader_and_directory.Substring(24);

			// Build the return string
			StringBuilder returnVal = new StringBuilder();

			// Step all the variable fields indicated in the directory
			int directory_position = 0, field_offset, field_length;
			string record_field, variable_field_data;
			while ( directory_position < directory.Length )
			{
				// Get the field of the position
				record_field = directory.Substring( directory_position, 3 );

				// Get the offset
				field_offset = Convert.ToInt32( directory.Substring( directory_position + 7, 5 ));

				// Get the length of this field
				field_length = Convert.ToInt32( directory.Substring( directory_position + 3, 4 ));
 
				// Get the variable field now
				int get_field_start = field_offset - unicode_chars;
				int next_character = 0;
				StringBuilder variable_field_data_builder = new StringBuilder();
				int unicode_chars_in_string = 0;
				while ( ( next_character + unicode_chars_in_string ) < field_length )
				{
					string nextCharString = variable_fields.Substring( next_character + get_field_start, 1);
					double ascii_string = (double) nextCharString[0];
					char nextChar = variable_fields[ next_character + get_field_start ];
					double ascii = (double) nextChar;
					if ( ascii < 128 )
					{
						next_character++;
					}
					if (( ascii >= 128 ) && ( ascii <= 2047 ))
					{
						unicode_chars_in_string = unicode_chars_in_string + 1;
						next_character++;
					}
					if ( ascii > 2047 )
					{
						unicode_chars_in_string = unicode_chars_in_string + 2;
						next_character++;;
					}

					variable_field_data_builder.Append( nextChar );
				}

				variable_field_data = variable_field_data_builder.ToString();
				unicode_chars += unicode_chars_in_string;

				// Make sure this starts with the record seperator
				if ( variable_field_data[0] != ((char) Record_Seperator ))
				{
                    throw new ApplicationException("Unexpected character at beginning of variable field!");
				}

				// See if this row has an indicator
				string indicator = "";
				if (( variable_field_data.Length > 4 ) && ( variable_field_data[3] == ((char) Unit_Seperator )))
				{
					indicator = variable_field_data.Substring(1,2);
					variable_field_data = variable_field_data.Substring(3);
				}
				else
					variable_field_data = variable_field_data.Substring(1);

				// Is this split into seperate subfields?
				if (( variable_field_data.Length > 1 ) && ( variable_field_data[0] == ((char) Unit_Seperator )))
				{
					// Split this into subfields
					string[] subfields = variable_field_data.Substring(1).Split( new char[] { (char) Unit_Seperator  } );

                    // Create the new field
                    MARC_Field newField = new MARC_Field();
                    newField.Tag = Convert.ToInt32(record_field);
                    newField.Indicators = indicator;

					// Step through each subfield
					foreach( string thisSubfield in subfields )
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
					thisRecord.Add_Field( Convert.ToInt32( record_field ), variable_field_data );
				}

				// Step to the next position in the directory
				directory_position += 12;
			}

			return thisRecord;
		}

        /// <summary> Flag indicates if an end of file has been reached </summary>
        public bool EOF_Flag
        {
            get
            {
                return eof_flag;
            }
            set
            {
                eof_flag = value;
            }
        }
	}
}

