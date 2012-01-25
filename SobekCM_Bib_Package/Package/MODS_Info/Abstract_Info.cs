using System;
using System.Text;
using SobekCM.Bib_Package.MARC;

namespace SobekCM.Bib_Package.Bib_Info
{
	/// <summary>Stores the information about any 'abstracts' associated with this digital resource. </summary>
	/// <remarks>This class extends the <see cref="XML_Node_Base_Type"/> class for writing to XML </remarks>
    [Serializable]
	public class Abstract_Info : XML_Node_Base_Type, IEquatable<Abstract_Info>
	{
		private string language;
		private string abstract_text;
        private string display_label;
        private string type;

		/// <summary> Constructor creates an empty instance of the abstract class </summary>
		public Abstract_Info( )
		{
			// Do nothing
		}

		/// <summary> Constructor creates an instance of the abstract class </summary>
		/// <param name="Abstract">Text of the abstract</param>
		/// <param name="Language">Language of the abstract</param>
        public Abstract_Info(string Abstract, string Language)
		{
			// Declare new values
			language = Language;
			abstract_text = Abstract;
		}

		/// <summary> Gets or sets the text of the abstract </summary>
		public string Abstract_Text
		{
			get		{	return abstract_text ?? String.Empty;		}
			set		{	abstract_text = value;		}
		}

		/// <summary> Gets or sets the language of the abstract </summary>
        /// <remarks>This is an uncontrolled field</remarks>
		public string Language
		{
            get { return language ?? String.Empty; }
			set		{	language = value;		}
		}

        /// <summary> Gets or sets the display label for the abstract </summary>
        /// <remarks>This attribute is intended to be used when additional text associated with the abstract is necessary for display. </remarks>
        public string Display_Label
        {
            get { return display_label ?? String.Empty; }
            set { display_label = value; }
        }

        /// <summary> Gets or sets the uncontrolled type for the abstract </summary>
        /// <remarks>There is no controlled list of abstract types.   Suggested values are: subject, review, scope and content, content advice</remarks>
        public string Type
        {
            get { return type ?? String.Empty; }
            set { type = value; }
        }

        /// <summary> Returns this abstract as a general string for display purposes </summary>
        /// <returns> This object in string format </returns>
        public override string ToString()
        {
            if ( String.IsNullOrEmpty( abstract_text ))
                return String.Empty;

            if (!String.IsNullOrEmpty(display_label))
            {
                return "<b>" + display_label + "</b> " + base.Convert_String_To_XML_Safe( abstract_text );
            }
            else
            {
                return base.Convert_String_To_XML_Safe(abstract_text);
            }
        }

        #region IEquatable<Note_Info> Members

        /// <summary> Compares this object with another similarly typed object </summary>
        /// <param name="other">Similarly types object </param>
        /// <returns>TRUE if the two objects are sufficiently similar</returns>
        public bool Equals(Abstract_Info other)
        {
            if ( Abstract_Text == other.Abstract_Text )
                return true;
            else
                return false;
        }

        #endregion  
        
        /// <summary> Writes this abstract as a MARC tag for aggregation into a MARC record </summary>
        /// <returns> Built MARC tag </returns>
        internal MARC_Field to_MARC_HTML()
        {
            MARC_Field returnValue = new MARC_Field();
            returnValue.Tag = 520;
            returnValue.Indicators = "  ";
            returnValue.Control_Field_Value = "|a " + abstract_text.Replace("|","&bar;");

            if (type != null)
            {
                switch (type)
                {
                    case "subject":
                        returnValue.Indicators = "0 ";
                        break;

                    case "review":
                        returnValue.Indicators = "1 ";
                        break;

                    case "scope and content":
                        returnValue.Indicators = "2 ";
                        break;

                    case "content advice":
                        returnValue.Indicators = "4 ";
                        break;
                }
            }

            return returnValue;

        }
        
        /// <summary> Writes this abstract as MODS to a writer writing to a stream ( either a file or web response stream )</summary>
        /// <param name="returnValue"> Writer to the MODS building stream </param>
        internal void Add_MODS( System.IO.TextWriter returnValue)
        {
            if (abstract_text.Length == 0)
                return;

            returnValue.Write( "<mods:abstract");
            base.Add_ID(returnValue);
            if ( !String.IsNullOrEmpty(type))
                returnValue.Write(" type=\"" + type + "\"");
            if (!String.IsNullOrEmpty(display_label))
                returnValue.Write(" displayLabel=\"" + base.Convert_String_To_XML_Safe(display_label) + "\"");
            if (!String.IsNullOrEmpty(language))
                returnValue.Write(" lang=\"" + language + "\"");
            returnValue.Write(">" + base.Convert_String_To_XML_Safe(abstract_text) + "</mods:abstract>\r\n");
        }
	}
}
