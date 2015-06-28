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
using System.IO;
using System.Text;
using SobekCM.Resource_Object.MARC;

#endregion

namespace SobekCM.Resource_Object.Metadata_File_ReaderWriters.MARC.Writers
{
    /// <summary> MARC21 exchange format writer  </summary>
    public class MARC21_Exchange_Format_Writer : IDisposable
    {
        // Constants used when writing the Marc21 stream
        private const char GROUP_SEPERATOR = (char) 29;
        private const char RECORD_SEPERATOR = (char) 30;
        private const char UNIT_SEPERATOR = (char) 31;

        private StreamWriter writer;

        /// <summary> Constructor for a new instance of this class </summary>
        /// <param name="FileName"> Name of the output file </param>
        public MARC21_Exchange_Format_Writer(string FileName)
        {
            // Open the stream
            writer = new StreamWriter(FileName, false, Encoding.UTF8);
        }

        /// <summary> Append a single record to the file </summary>
        /// <param name="Record">New record to append </param>
        public void AppendRecord(MARC_Record Record)
        {
            writer.Write(To_Machine_Readable_Record(Record));
        }

        /// <summary> Append a list of records to the file </summary>
        /// <param name="Records">Collection of records to append </param>
        public void AppendRecords(IEnumerable<MARC_Record> Records)
        {
            foreach (MARC_Record record in Records)
            {
                writer.Write(To_Machine_Readable_Record(record));
            }
        }

        /// <summary> Close the stream writer used for this </summary>
        public void Close()
        {
            try
            {
                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                    writer = null;
                }
            }
            catch
            {
            }
        }

        #region Static methods converts a single MARC record to a Marc21-formatted string

        /// <summary> Returns a string which represents a record in machine readable record format. </summary>
        /// <param name="Record"> MARC record to convert to MARC21 </param>
        /// <returns> MARC record as MARC21 Exchange format record string</returns>
        public static string To_Machine_Readable_Record(MARC_Record Record)
        {
            // Create the stringbuilder for this
            StringBuilder directory = new StringBuilder(1000);
            StringBuilder completefields = new StringBuilder(2000);
            StringBuilder completeLine = new StringBuilder(200);

            // Step through each entry by key from the hashtable
            List<string> overallRecord = new List<string>();
            int runningLength = 0;

            // Step through each field ( control and data ) in the record
            foreach (MARC_Field thisEntry in Record.Sorted_MARC_Tag_List)
            {
                // Perpare to build this line
                if (completeLine.Length > 0)
                    completeLine.Remove(0, completeLine.Length);

                // Is this a control field (with no subfields) or a data field?
                if (thisEntry.Subfield_Count == 0)
                {
                    if (!String.IsNullOrEmpty(thisEntry.Control_Field_Value))
                    {
                        completeLine.Append(int_to_string(thisEntry.Tag, 3) + RECORD_SEPERATOR);
                        completeLine.Append(thisEntry.Control_Field_Value);
                        overallRecord.Add(completeLine.ToString());
                    }
                }
                else
                {
                    // Start this tag and add the indicator, if there is one
                    if (thisEntry.Indicators.Length == 0)
                        completeLine.Append(int_to_string(thisEntry.Tag, 3) + RECORD_SEPERATOR);
                    else
                        completeLine.Append(int_to_string(thisEntry.Tag, 3) + RECORD_SEPERATOR + thisEntry.Indicators);

                    // Build the complete line
                    foreach (MARC_Subfield thisSubfield in thisEntry.Subfields)
                    {
                        if (thisSubfield.Subfield_Code == ' ')
                        {
                            if (thisEntry.Indicators.Length == 0)
                                completeLine.Append(thisSubfield.Data);
                            else
                                completeLine.Append(UNIT_SEPERATOR.ToString() + thisSubfield.Data);
                        }
                        else
                        {
                            completeLine.Append(UNIT_SEPERATOR.ToString() + thisSubfield.Subfield_Code + thisSubfield.Data);
                        }
                    }

                    // Add this to the list
                    overallRecord.Add(completeLine.ToString());
                }
            }

            // Now, add these to the directory and completefields StringBuilders
            foreach (string thisLin in overallRecord)
            {
                // Add this line to the directory and fields
                directory.Append(thisLin.Substring(0, 3) + (int_to_string(adjusted_length(thisLin) - 3, 4)) + (int_to_string(runningLength, 5)));
                completefields.Append(thisLin.Substring(3));

                // Increment the running length
                runningLength += adjusted_length(thisLin) - 3;
            }

            // Get the length of just the directory, before we start appending more to it
            int directory_length = directory.Length;

            // Compile the return value
            directory.Append(completefields.ToString() + RECORD_SEPERATOR + GROUP_SEPERATOR);

            // Get the leader
            string leader = Record.Leader;

            // Insert the total length of this record
            runningLength += leader.Length + directory_length + 2;

            // Return the combination of these two fields, plus the end of record char
            return int_to_string(runningLength, 5) + leader.Substring(5, 7) + int_to_string(leader.Length + directory_length + 1, 5) +
                   leader.Substring(17) + directory;
        }

        private static string int_to_string(int Number, int LengthRequired)
        {
            // Verify the number fits
            if (Number.ToString().Length > LengthRequired)
                throw new ApplicationException("Number too large for field length!  Record may be too large!");

            // Return the value
            return Number.ToString().PadLeft(LengthRequired, '0');
        }

        private static int adjusted_length(string Line)
        {
            int length = 0;
            foreach (char thisChar in Line)
            {
                double ascii = thisChar;
                if (ascii < 128)
                {
                    length++;
                }
                if ((ascii >= 128) && (ascii <= 2047))
                {
                    length += 2;
                }
                if (ascii > 2047)
                {
                    length += 3;
                }
            }
            return length;
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
    }
}