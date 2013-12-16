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
using System.Xml;

#endregion

namespace SobekCM.Resource_Object.MARC.Parsers
{
    /// <summary> Parser steps through the records in a MarcXML file or stream. </summary>
    /// <remarks> Written by Mark Sullivan for the University of Florida Digital Library<br /><br />
    /// You can either pass in the stream or file to read into the constructor and immediately begin using Next() to step
    /// through them, or you can use the empty constructor and call the Parse methods for the first record. <br /><br />
    /// To  use the IEnumerable interface, you must pass in the Stream or filename in the constructor.</remarks>
    public class MARCXML_Parser : IDisposable, IEnumerable<MARC_Record>, IEnumerator<MARC_Record>
    {
        // Stream used to read the MarcXML records
        private Stream baseStream;
        private string filename;
        private XmlTextReader reader;

        #region Constructors 

        /// <summary> Constructor for a new instance of this class </summary>
        public MARCXML_Parser()
        {
            // Constructor does nothing
        }

        /// <summary> Constructor for a new instance of this class </summary>
        /// <param name="MarcXML_Stream"> Open stream from which to read MarcXML records </param>
        public MARCXML_Parser(Stream MarcXML_Stream)
        {
            // Create the new reader object
            reader = new XmlTextReader(MarcXML_Stream);

            // Save the stream for resetting purposes
            baseStream = MarcXML_Stream;
        }

        /// <summary> Constructor for a new instance of this class </summary>
        /// <param name="MarcXML_File"> Name of the file to parse </param>
        public MARCXML_Parser(string MarcXML_File)
        {
            // Create the new reader object
            reader = new XmlTextReader(MarcXML_File);

            // Save the filename
            filename = MarcXML_File;
        }

        #endregion

        /// <summary> Begins parsing from a stream containing MarcXML records. </summary>
        /// <param name="MarcXML_Stream"> Open stream from which to read MarcXML records </param>
        /// <returns> A built record, or NULL if no records are contained within the file </returns>
        public MARC_Record Parse(Stream MarcXML_Stream)
        {
            // Create the new reader object
            reader = new XmlTextReader(MarcXML_Stream);

            // Save the stream for resetting purposes
            baseStream = MarcXML_Stream;
            filename = null;

            // Return the first record
            return parse_next_record();
        }

        /// <summary> Begins parsing a new MarcXML file. </summary>
        /// <param name="MarcXML_File"> Name of the file to parse </param>
        /// <returns> A built record, or NULL if no records are contained within the file </returns>
        public MARC_Record Parse(string MarcXML_File)
        {
            // Create the new reader object
            reader = new XmlTextReader(MarcXML_File);

            // Save the filename
            filename = MarcXML_File;
            baseStream = null;

            // Return the first record
            return parse_next_record();
        }

        /// <summary> Returns the next record in the MarcXML_File file or stream </summary>
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

            // Try to read this
            Read_MARC_Info(reader, thisRecord);

            // Return this record
            return thisRecord;
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
            if (baseStream != null)
            {
                if (baseStream.CanSeek)
                    baseStream.Seek(0, SeekOrigin.Begin);
                reader = new XmlTextReader(baseStream);
            }
            else if (!String.IsNullOrEmpty(filename))
            {
                if (reader != null)
                    Close();
                reader = new XmlTextReader(filename);
            }
        }

        #endregion

        #region Static methods read a single MARC record from a MarcXML file or nodereader 

        /// <summary> Reads the data from a MARC XML file into this record </summary>
        /// <param name="MARC_XML_File">Input MARC XML file</param>
        /// <param name="Record"> Record into which to read the contents of the MarcXML file </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public static bool Read_From_MARC_XML_File(string MARC_XML_File, MARC_Record Record)
        {
            try
            {
                // Load this MXF File
                XmlDocument marcXML = new XmlDocument();
                marcXML.Load(MARC_XML_File);

                Stream reader = new FileStream(MARC_XML_File, FileMode.Open, FileAccess.Read);

                // create the node reader
                XmlTextReader nodeReader = new XmlTextReader(reader);

                return Read_MARC_Info(nodeReader, Record);
            }
            catch
            {
                return false;
            }
        }


        /// <summary> Reads the data from a XML Node Reader </summary>
        /// <param name="nodeReader">XML Node Reader </param>
        /// <param name="Record"> Record into which to read the contents of the MarcXML file </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public static bool Read_MARC_Info(XmlReader nodeReader, MARC_Record Record)
        {
            try
            {
                // Move to the this node
                move_to_node(nodeReader, "record");

                // Get the leader information
                int tag = -1;
                while (nodeReader.Read())
                {
                    if ((nodeReader.NodeType == XmlNodeType.EndElement) && (nodeReader.Name == "record"))
                        return true;

                    if (nodeReader.NodeType == XmlNodeType.Element)
                    {
                        switch (nodeReader.Name.Trim().Replace("marc:", ""))
                        {
                            case "leader":
                                nodeReader.Read();
                                Record.Leader = nodeReader.Value;
                                break;

                            case "controlfield":
                                // Get the tag
                                if (nodeReader.MoveToAttribute("tag"))
                                {
                                    // The tag should always be numeric per the schema, but just relaxing this
                                    // for invalid MARC so the rest of the data can be successfully read.
                                    if (Int32.TryParse(nodeReader.Value, out tag))
                                    {
                                        // Move to the value and then add this
                                        nodeReader.Read();
                                        Record.Add_Field(tag, nodeReader.Value);
                                    }
                                }
                                break;

                            case "datafield":
                                // Set the default indicators
                                char ind1 = ' ';
                                char ind2 = ' ';

                                // Get the indicators if they exist
                                while (nodeReader.MoveToNextAttribute())
                                {
                                    if (nodeReader.Name.Trim() == "ind1")
                                    {
                                        string temp1 = nodeReader.Value;
                                        if (temp1.Length > 0)
                                            ind1 = temp1[0];
                                    }
                                    if (nodeReader.Name.Trim() == "ind2")
                                    {
                                        string temp2 = nodeReader.Value;
                                        if (temp2.Length > 0)
                                            ind2 = temp2[0];
                                    }
                                    if (nodeReader.Name.Trim() == "tag")
                                        tag = Convert.ToInt32(nodeReader.Value);
                                }

                                // Add this datafield
                                MARC_Field newField = Record.Add_Field(tag, ind1, ind2);

                                // Now, add each subfield
                                while (nodeReader.Read())
                                {
                                    if ((nodeReader.NodeType == XmlNodeType.EndElement) && (nodeReader.Name.Replace("marc:", "") == "datafield"))
                                        break;

                                    if ((nodeReader.NodeType == XmlNodeType.Element) && (nodeReader.Name.Replace("marc:", "") == "subfield"))
                                    {
                                        // Get the code
                                        nodeReader.MoveToFirstAttribute();
                                        char subfield = nodeReader.Value.Length > 0 ? nodeReader.Value[0] : ' ';

                                        // Get the value
                                        nodeReader.Read();
                                        string dataValue = nodeReader.Value;

                                        // Save this subfield
                                        newField.Add_Subfield(subfield, dataValue);

                                        // Do some special stuff if this is the 260
                                        if (tag == 260)
                                        {
                                            newField.Control_Field_Value = newField.Control_Field_Value + "|" + subfield + " " + dataValue + " ";
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void move_to_node(XmlReader nodeReader, string nodeName)
        {
            while (nodeReader.Read())
            {
                if (nodeReader.Name.Trim().Replace("marc:", "") == nodeName)
                {
                    return;
                }
            }
        }

        #endregion
    }
}