using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;



namespace SobekCM.Bib_Package.MARC
{
	/// <summary> Stores all the information from a MARC21 record </summary>
	/// <remarks>Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
	public class MARC_Record
	{
        private const int Group_Seperator = 29;
        private const int Record_Seperator = 30;
        private const int Unit_Seperator = 31;

		private string leader;
		private SortedList<int, List<MARC_Field>> fields;

        private bool errorFlag;
        private string controlNumber;

		/// <summary> Constructor for a new instance of the MARC_XML_Record class </summary>
		public MARC_Record()
		{
			leader = String.Empty;
            fields = new SortedList<int, List<MARC_Field>>();
            errorFlag = false;
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

                if (fields.ContainsKey(1))
                {
                    controlNumber = fields[1][0].Control_Field_Value;
                }
                else
                {
                    controlNumber = String.Empty;
                }
        
                return controlNumber; 
            }
        }

        /// <summary> Flag is set if there is an error detected while reading this MARC
        /// record from a MARC21 Exchange Format file </summary>
        /// <remarks> This is used when importing directly from MARC records into the SobekCM library </remarks>
        public bool Error_Flag
        {
            get { return errorFlag; }
            set { errorFlag = value; }
        }

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
                else
                {
                    return total_length_string + leader.Substring(5, 7) + total_directory_string + leader.Substring(17);
                }
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
                else
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

        #region Public methods to check if a field exists, or add a field

        /// <summary> Gets a flag indicating if a particular field exists </summary>
		/// <param name="Tag">Tag for the MARC field to check</param>
		/// <returns>TRUE if the field exists, otherwise FALSE</returns>
		public bool has_Field( int Tag )
		{
			return fields.ContainsKey( Tag );
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
                List<MARC_Field> newTagCollection = new List<MARC_Field>();
                newTagCollection.Add(newField);
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
		public MARC_Field Add_Field( int Tag, char Indicator1, char Indicator2 )
		{
			// Create the new datafield
			MARC_Field newField = new MARC_Field( Tag, Indicator1, Indicator2 );

            // Either add this to the existing list, or create a new one
            if (fields.ContainsKey(Tag))
                fields[Tag].Add(newField);
            else
            {
                List<MARC_Field> newTagCollection = new List<MARC_Field>();
                newTagCollection.Add(newField);
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
            MARC_Field newField = new MARC_Field(Tag, Control_Field_Value);
            newField.Indicators = Indicators;

            // Either add this to the existing list, or create a new one
            if (fields.ContainsKey(Tag))
                fields[Tag].Add(newField);
            else
            {
                List<MARC_Field> newTagCollection = new List<MARC_Field>();
                newTagCollection.Add(newField);
                fields[Tag] = newTagCollection;
            }

            // Return the newlly built data field
            return newField;
        }

        /// <summary> Adds a new field to this record </summary>
        /// <param name="New_Field"> New field to add </param>
        public void Add_Field( MARC_Field New_Field )
        {
            if (New_Field == null)
                return;

            // Either add this to the existing list, or create a new one
            if (fields.ContainsKey(New_Field.Tag))
                fields[New_Field.Tag].Add(New_Field);
            else
            {
                List<MARC_Field> newTagCollection = new List<MARC_Field>();
                newTagCollection.Add(New_Field);
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
            else
                return String.Empty;
        }

        #endregion

        #region Methods to read from a MARC XML file into this structure

        /// <summary> Reads the data from a MARC XML file into this record </summary>
		/// <param name="MARC_XML_File">Input MARC XML file</param>
		/// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Read_From_MARC_XML_File(string MARC_XML_File)
        {
            try
            {
                // Load this MXF File
                XmlDocument marcXML = new XmlDocument();
                marcXML.Load(MARC_XML_File);

                Stream reader = new FileStream(MARC_XML_File, FileMode.Open, FileAccess.Read);

                // create the node reader
                XmlTextReader nodeReader = new XmlTextReader(reader);

                return Read_MARC_Info(nodeReader);

            }
            catch
            {
                return false;
            }
        }


        /// <summary> Reads the data from a XML Node Reader </summary>
        /// <param name="nodeReader">XML Node Reader </param>
		/// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Read_MARC_Info(XmlTextReader nodeReader)
		{
            try
			{
				// Move to the this node
				move_to_node( nodeReader, "record" );

				// Get the leader information
                int tag = -1;
                string dataValue;
                char ind1 = ' ';
                char ind2 = ' ';
				char subfield;
				MARC_Field newField;
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
                                this.Leader = nodeReader.Value;
                                break;

                            case "controlfield":
                                // Get the tag
                                if (nodeReader.MoveToAttribute("tag"))
                                {
                                    // The tag should always be numeric per the schema, but just relaxing this
                                    // for invalid MARC so the rest of the data can be successfully read.
                                    if (Int32.TryParse(nodeReader.Value, out tag ))
                                    {
                                        // Move to the value and then add this
                                        nodeReader.Read();
                                        this.Add_Field(tag, nodeReader.Value);
                                    }
                                }
                                break;

                            case "datafield":
                                // Set the default indicators
                                ind1 = ' ';
                                ind2 = ' ';

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
                                newField = this.Add_Field(tag, ind1, ind2);

                                // Now, add each subfield
                                while (nodeReader.Read())
                                {
                                    if ((nodeReader.NodeType == XmlNodeType.EndElement) && (nodeReader.Name.Replace("marc:", "") == "datafield"))
                                        break;

                                    if ((nodeReader.NodeType == XmlNodeType.Element) && (nodeReader.Name.Replace("marc:", "") == "subfield"))
                                    {
                                        // Get the code
                                        nodeReader.MoveToFirstAttribute();
                                        if (nodeReader.Value.Length > 0)
                                        {
                                            subfield = nodeReader.Value[0];
                                        }
                                        else
                                        {
                                            subfield = ' ';
                                        }

                                        // Get the value
                                        nodeReader.Read();
                                        dataValue = nodeReader.Value;

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

        private void move_to_node(XmlTextReader nodeReader, string nodeName)
		{
			while ( nodeReader.Read() ) 
			{
                if (nodeReader.Name.Trim().Replace("marc:","") == nodeName)
                {
                    return;
                }
			}
        }

        #endregion

        /// <summary> Removes all occurrences of a tag </summary>
        /// <param name="Tag_Number"> Tag number of the MARC tags to remove </param>
        public void Remove_Tag(int Tag_Number)
        {
            // Remove from the list of fields
            if (fields.ContainsKey(Tag_Number))
                fields.Remove(Tag_Number);
        }

        /// <summary> Saves this MARC records as MARC XML </summary>
        /// <param name="Filename"> Filename to save this MARC record as </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_MARC_XML(string Filename)
        {
            try
            {
                StreamWriter writer = new StreamWriter(Filename, false);
                writer.Write(To_MARC_XML());
                writer.Flush();
                writer.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Returns this MARC record as MARC XML </summary>
        /// <returns> This record as MARC XML </returns>
        public string To_MARC_XML()
        {
            StringBuilder returnValue = new StringBuilder(5000);
            string indent = "     ";
            string fullIndent = indent + indent + indent;

            // Add the MARC XML header and start this collection
            returnValue.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n");
            returnValue.Append("<collection xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n");
            returnValue.Append(indent + "xsi:schemaLocation=\"http://www.loc.gov/MARC21/slim\r\n");
            returnValue.Append(indent + indent + "http://www.loc.gov/standards/marcxml/schema/MARC21slim.xsd\"\r\n");
            returnValue.Append(indent + "xmlns=\"http://www.loc.gov/MARC21/slim\">");

            // Begin this record
            returnValue.Append(indent + "<record>\r\n");

            // Add the leader
            returnValue.Append(fullIndent + "<leader>" + this.Leader + "</leader>\r\n");

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
                            returnValue.Append(fullIndent + "<controlfield tag=\"" + thisField.Tag.ToString().PadLeft(3, '0') + "\">" + thisField.Control_Field_Value_XML + "</controlfield>\r\n");
                        }
                    }
                    else
                    {
                        returnValue.Append(fullIndent + "<datafield tag=\"" + thisField.Tag.ToString().PadLeft(3, '0') + "\" ind1=\"" + thisField.Indicator1 + "\" ind2=\"" + thisField.Indicator2 + "\">\r\n");

                        // Add each subfield
                        foreach (MARC_Subfield thisSubfield in thisField.Subfields)
                        {
                            returnValue.Append(fullIndent + indent + "<subfield code=\"" + thisSubfield.Subfield_Code + "\">" + thisSubfield.Data_XML + "</subfield>\r\n");
                        }

                        returnValue.Append(fullIndent + "</datafield>\r\n");
                    }
                }
            }

            // End this record and collection
            returnValue.Append(indent + "</record>\r\n");
            returnValue.Append("</collection>\r\n");

            return returnValue.ToString();
        }

        /// <summary> Saves this MARC record as MARC21 Exchange format record data file </summary>
        /// <param name="Filename"> Filename to save this MARC record as  </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_MARC21(string Filename)
        {
            try
            {
                StreamWriter writer = new StreamWriter(Filename, false);
                writer.Write(To_Machine_Readable_Record());
                writer.Flush();
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
            // Create the stringbuilder for this
            StringBuilder directory = new StringBuilder(1000);
            StringBuilder completefields = new StringBuilder(2000);

            // Step through each entry by key from the hashtable
            ArrayList overallRecord = new ArrayList();
            int runningLength = 0, directory_length;



            //// Step through each control field entry in the collection
            //foreach (MARC21_Controlfield thisEntry in controlfields)
            //{
            //    // Start to build this list
            //    completeLine = new StringBuilder();
            //    completeLine.Append(int_to_string(thisEntry.Tag_Number, 3) + ((char)Record_Seperator));
            //    completeLine.Append(thisEntry.Field_Value);
            //    overallRecord.Add(completeLine.ToString());
            //}

            //// Step through each data field entry in the collection
            //foreach (MARC21_Datafield thisEntry in datafields)
            //{
            //    // Start to build this list
            //    // Start to build this list
            //    completeLine = new StringBuilder();
            //    if (thisEntry.Indicator.Length == 0)
            //        completeLine.Append(int_to_string(thisEntry.Tag_Number, 3) + ((char)Record_Seperator));
            //    else
            //        completeLine.Append(int_to_string(thisEntry.Tag_Number, 3) + ((char)Record_Seperator) + thisEntry.Indicator);

            //    // Build the complete line
            //    foreach (MARC21_Subfield thisSubfield in thisEntry.Subfields)
            //    {
            //        if (thisSubfield.Subfield_Code == ' ')
            //        {
            //            if (thisEntry.Indicator.Length == 0)
            //                completeLine.Append(thisSubfield.Data);
            //            else
            //                completeLine.Append(((char)Unit_Seperator).ToString() + thisSubfield.Data);
            //        }
            //        else
            //        {
            //            completeLine.Append(((char)Unit_Seperator).ToString() + thisSubfield.Subfield_Code + thisSubfield.Data);
            //        }
            //    }

            //    // Add this to the list
            //    overallRecord.Add(completeLine.ToString());
            //}

            // Now, add these to the directory and completefields StringBuilders
            foreach (string thisLin in overallRecord)
            {
                // Add this line to the directory and fields
                directory.Append(thisLin.Substring(0, 3) + (int_to_string(thisLin.Length - 3, 4)) + (int_to_string(runningLength, 5)));
                completefields.Append(thisLin.Substring(3));

                // Increment the running length
                runningLength += thisLin.Length - 3;
            }

            // Get the length of just the directory, before we start appending more to it
            directory_length = directory.Length;

            // Compile the return value
            directory.Append(completefields.ToString() + ((char)Record_Seperator) + ((char)Group_Seperator));

            // Insert the total length of this record
            runningLength += leader.Length + directory_length + 2;

            // Return the combination of these two fields, plus the end of record char
            return int_to_string(runningLength, 5) + leader.Substring(5, 7) + int_to_string(leader.Length + directory_length + 1, 5) +
                leader.Substring(17) + directory.ToString();
        }

        private string int_to_string(int number, int length_required)
        {
            string returnVal = number.ToString();
            while (returnVal.Length < length_required)
            {
                returnVal = "0" + returnVal;
            }

            // Throw an error if too long
            if (returnVal.Length > length_required)
                throw new ApplicationException("Number too large for field length!  Record may be too large!");

            // Return the value
            return returnVal;
        }


        /// <summary> Outputs this record as a string </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Create the StringBuilder
            StringBuilder returnVal = new StringBuilder(2000);

            // Add the leader
            returnVal.Append("LDR " + this.Leader + "\r\n");

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
    }
}
