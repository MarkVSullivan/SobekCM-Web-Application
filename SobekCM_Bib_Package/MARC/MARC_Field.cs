using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SobekCM.Bib_Package.MARC
{
	/// <summary> Stores the for a field in a MARC21 record ( <see cref="MARC_Record"/> )</summary>
	/// <remarks>Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
	public class MARC_Field
	{
		private int tag;
		private char indicator1, indicator2;
        private List<MARC_Subfield> subfields;
		//private Dictionary<char, string> subfields;
        private string data;

        /// <summary> Constructor for a new instance of the MARC_Field class </summary>
		public MARC_Field()
		{
            subfields = new List<MARC_Subfield>();
            //subfields = new Dictionary<char, string>();
			tag = -1;
			indicator1 = ' ';
			indicator2 = ' ';
		}

        /// <summary> Constructor for a new instance of the MARC_Field class </summary>
        /// <param name="Tag">Tag for this data field</param>
        /// <param name="Control_Field_Value">Value for this control field </param>
        public MARC_Field( int Tag, string Control_Field_Value )
        {
            subfields = new List<MARC_Subfield>();
            //subfields = new Dictionary<char, string>();
            tag = Tag;
            data = Control_Field_Value;
            indicator1 = ' ';
            indicator2 = ' ';
        }

        /// <summary> Constructor for a new instance of the MARC_Field class </summary>
		/// <param name="Tag">Tag for this data field</param>
		/// <param name="Indicator1">First indicator</param>
		/// <param name="Indicator2">Second indicator</param>
		public MARC_Field( int Tag, char Indicator1, char Indicator2 )
		{
            subfields = new List<MARC_Subfield>();
            //subfields = new Dictionary<char, string>();
			tag = Tag;
			indicator1 = Indicator1;
			indicator2 = Indicator2;
		}

        /// <summary> Constructor for a new instance of the MARC_Field class </summary>
		/// <param name="Tag">Tag for this data field</param>
		/// <param name="Indicators">Indicators</param>
        /// <param name="Control_Field_Value">Value for this control field</param>
		public MARC_Field( int Tag, string Indicators, string Control_Field_Value )
		{
            subfields = new List<MARC_Subfield>();
            //subfields = new Dictionary<char, string>();
			tag = Tag;
            data = Control_Field_Value;

            if (Indicators.Length >= 2)
            {
                Indicator1 = Indicators[0];
                Indicator2 = Indicators[1];
            }
            else
            {
                if (Indicators.Length == 0)
                {
                    Indicator1 = ' ';
                    Indicator2 = ' ';
                }
                if (Indicators.Length == 1)
                {
                    Indicator1 = Indicators[0];
                    Indicator2 = ' ';
                }
            }
		}



        /// <summary> Gets or sets the data for this MARC XML field which does not exist 
        /// in any subfield </summary>
        /// <remarks> This is generally used for the control fields at the beginning of the MARC record </remarks>
        public string Control_Field_Value
        {
            get
            {
                if (String.IsNullOrEmpty(data))
                    return String.Empty;
                else
                    return data;
            }
            set
            {
                data = value;
            }
        }

        /// <summary> Gets the data for this MARC XML field's control field value ( as 
        /// opposed to existing in subfields ) as a XML formatted string </summary>
        public string Control_Field_Value_XML
        {
            get
            {
                string xml_safe = data;
                int i = xml_safe.IndexOf("&");
                while (i >= 0)
                {
                    if ((i != xml_safe.IndexOf("&amp;", i)) && (i != xml_safe.IndexOf("&quot;", i)) &&
                        (i != xml_safe.IndexOf("&gt;", i)) && (i != xml_safe.IndexOf("&lt;", i)))
                    {
                        xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
                    }

                    i = xml_safe.IndexOf("&", i + 1);
                }
                return xml_safe.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("[", "").Replace("]", "");
            }
        }

		/// <summary> Gets or sets the tag for this data field </summary>
		public int Tag
		{
			get	{	return tag;		}
			set	{	tag = value;	}
		}

		/// <summary> Gets or sets the first character of the indicator </summary>
		public char Indicator1
		{
			get	{	return indicator1;		}
			set	{	indicator1 = value;		}
		}

		/// <summary> Gets or sets the second character of the indicator </summary>
		public char Indicator2
		{
			get	{	return indicator2;		}
			set	{	indicator2 = value;		}
		}

		/// <summary> Gets or sets the complete indicator for this data field </summary>
		public string Indicators
		{
			get	{	return Indicator1.ToString() + Indicator2;		}
            set
            {
                if (value.Length >= 2)
                {
                    Indicator1 = value[0];
                    Indicator2 = value[1];
                }
                else
                {
                    if (value.Length == 0)
                    {
                        Indicator1 = ' ';
                        Indicator2 = ' ';
                    }
                    if (value.Length == 1)
                    {
                        Indicator1 = value[0];
                        Indicator2 = ' ';
                    }
                }
            }
		}

		/// <summary> Get the number of subfields in this data field </summary>
		public int Subfield_Count
		{
			get	{	return subfields.Count;		}
		}

		/// <summary> Gets the collection of subfields in this data field </summary>
        public List<MARC_Subfield> Subfields
		{
			get	{	return subfields;	}
		}

		/// <summary> Returns flag indicating if this data field has the indicated subfield </summary>
		/// <param name="Subfield_Code">Code for the subfield in question</param>
		/// <returns>TRUE if the subfield exists, otherwise FALSE</returns>
		public bool has_Subfield( char Subfield_Code )
		{
            foreach (MARC_Subfield subfield in subfields)
            {
                if (subfield.Subfield_Code == Subfield_Code)
                    return true;
            }
            return false;
		}

        /// <summary> Adds a new subfield code to this MARC field </summary>
        /// <param name="Subfield_Code"> Code for this subfield in the MARC record </param>
		/// <param name="Data"> Data stored for this subfield </param>
        public void Add_Subfield(char Subfield_Code, string Data)
        {
            subfields.Add(new MARC_Subfield(Subfield_Code, Data));
        }

        /// <summary> Gets the data from a particular subfield in this data field </summary>
        /// <param name="Subfield_Code"> Code for the subfield in question </param>
        /// <returns>The value of the subfield, or an empty string </returns>
        /// <remarks> If there are multiple instances of this subfield, then they are returned 
        /// together with a '|' delimiter between them </remarks>
        public string this[char Subfield_Code]
        {
            get
            {
                string returnValue = String.Empty;
                foreach (MARC_Subfield subfield in subfields)
                {
                    if (subfield.Subfield_Code == Subfield_Code)
                    {
                        if ( returnValue.Length == 0 )
                            returnValue = subfield.Data;
                        else
                            returnValue = returnValue + "|" + subfield.Data;
                    }
                }
                return returnValue;
            }
         }


		/// <summary> Returns this data field as a simple string value </summary>
		/// <returns> Data field as a string </returns>
		public override string ToString()
		{
			// Build the return value
            StringBuilder returnValue = new StringBuilder(Tag.ToString() + " " + indicator1 + indicator2 + " ");
            foreach ( MARC_Subfield thisField in subfields )
            {
                returnValue.Append("|" + thisField.Subfield_Code + " " + thisField.Data );
            }
            return returnValue.ToString();
		}
	}
}
