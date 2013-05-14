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
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Linq;

#endregion

namespace SobekCM.Resource_Object.MARC.Writers
{
    public class MARCXML_Writer : IDisposable
    {
        private StreamWriter writer;

        /// <summary> Constructor for a new instance of this class </summary>
        /// <param name="FileName"> Name of the output file </param>
        public MARCXML_Writer(string FileName)
        {
            // Open the stream
            writer = new StreamWriter(FileName, false, Encoding.UTF8);

            // Start the file
            const string indent = "    ";
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            writer.WriteLine("<collection xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
            writer.WriteLine(indent + "xsi:schemaLocation=\"http://www.loc.gov/MARC21/slim");
            writer.WriteLine(indent + indent + "http://www.loc.gov/standards/marcxml/schema/MARC21slim.xsd\"");
            writer.WriteLine(indent + "xmlns=\"http://www.loc.gov/MARC21/slim\">");
        }

        /// <summary> Append a single record to the file </summary>
        /// <param name="Record">New record to append </param>
        public void AppendRecord(MARC_Record Record)
        {
            writer.WriteLine(To_MarcXML(Record, false));
        }

        /// <summary> Append a list of records to the file </summary>
        /// <param name="Records">Collection of records to append </param>
        public void AppendRecords(IEnumerable<MARC_Record> Records)
        {
            foreach (MARC_Record record in Records)
            {
                writer.WriteLine(To_MarcXML(record, false));
            }
        }

        /// <summary> Close the stream writer used for this </summary>
        public void Close()
        {
            try
            {
                if (writer != null)
                {
                    writer.WriteLine("</collection>");
                    writer.Flush();
                    writer.Close();
                    writer = null;
                }
            }
            catch
            {
            }
        }

        #region Static methods converts a single MARC record to a MarcXML-formatted string 

        /// <summary> Returns this MARC record as a MarcXML-formatted string </summary>
        /// <param name="Record"> MARC record to convert to a MarcXML-formatted string</param>
        /// <returns> This record as MarcXML-formatted string with the XML and collection declarative tags around the record </returns>
        public static string To_MarcXML(MARC_Record Record)
        {
            return To_MarcXML(Record, true);
        }

        /// <summary> Returns this MARC record as a MarcXML-formatted string </summary>
        /// <param name="Record"> MARC record to convert to a MarcXML-formatted string</param>
        /// <param name="Include_Start_End_Tags"> Flag indicates whether to include the XML and collection declarative tags around the record </param>
        /// <returns> This record as MarcXML-formatted string </returns>
        public static string To_MarcXML(MARC_Record Record, bool Include_Start_End_Tags)
        {
            StringBuilder returnValue = new StringBuilder(5000);

            // Add the MARC XML header and start this collection
            if (Include_Start_End_Tags)
            {
                const string indent = "    ";
                returnValue.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
                returnValue.AppendLine("<collection xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
                returnValue.AppendLine(indent + "xsi:schemaLocation=\"http://www.loc.gov/MARC21/slim");
                returnValue.AppendLine(indent + indent + "http://www.loc.gov/standards/marcxml/schema/MARC21slim.xsd\"");
                returnValue.AppendLine(indent + "xmlns=\"http://www.loc.gov/MARC21/slim\">");
            }

            // Begin this record and add the leader
            XElement record_root = new XElement("record",
                                                new XElement("leader", Record.Leader)
                );

            // Step through each field in the collection
            foreach (MARC_Field thisField in Record.Sorted_MARC_Tag_List)
            {
                if (thisField.Subfield_Count == 0)
                {
                    if (thisField.Control_Field_Value.Length > 0)
                    {
                        // Create this new control field and add it to the root element
                        XElement controlField = new XElement("controlfield",
                                                             thisField.Control_Field_Value.Replace(Convert.ToChar((byte)0x1F), ' '),
                                                             new XAttribute("tag", thisField.Tag.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0'))
                            );
                        record_root.Add(controlField);
                    }
                }
                else
                {
                    // Create the new datafield element and add it to the root element
                    XElement dataField = new XElement("datafield",
                                                      new XAttribute("tag", thisField.Tag.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0')),
                                                      new XAttribute("ind1", thisField.Indicator1),
                                                      new XAttribute("ind2", thisField.Indicator2)
                        );
                    record_root.Add(dataField);

                    // Add each subfield
                    foreach (MARC_Subfield thisSubfield in thisField.Subfields)
                    {
                        // Create this subfield element and add it to the datafield
                        XElement subfield = new XElement("subfield",
                                                         thisSubfield.Data.Replace(Convert.ToChar((byte)0x1F), ' '),
                                                         new XAttribute("code", thisSubfield.Subfield_Code)
                            );
                        dataField.Add(subfield);
                    }
                }
            }

            // Add the XML text to the string builder
            returnValue.Append(record_root.ToString());

            // Close this collection, if requested
            if (Include_Start_End_Tags)
            {
                returnValue.AppendLine();
                returnValue.AppendLine("</collection>");
            }

            return returnValue.ToString();
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