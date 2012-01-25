using System;
using System.Text;

namespace SobekCM.Bib_Package.Bib_Info
{
	/// <summary>Stores information about a single identifier associated with a digital resource </summary>
    /// <remarks>"identifier" contains a unique standard number or code that distinctively identifies a resource. 
    /// It includes manifestation, expression and work level identifiers. This should be repeated for each applicable identifier recorded, including invalid and canceled identifiers. </remarks>
    [Serializable]
	public class Identifier_Info : XML_Writing_Base_Type, IEquatable<Identifier_Info>
	{
		private string identifier;
        private string type;
        private string displayLabel;
        private int record_linkid;

		/// <summary> Constructor for an empty instance of the Identifier_Info class </summary>
		public Identifier_Info()
		{
            record_linkid = -1;
		}

        /// <summary> Constructor for a new instance of the Identifier_Info class </summary>
		/// <param name="Identifier">Identifier</param>
        public Identifier_Info( string Identifier )
		{
			identifier = Identifier;
            record_linkid = -1;
		}

		/// <summary> Constructor for a new instance of the Identifier_Info class </summary>
        /// <param name="Type">Uncontrolled Identifier Type</param>
		/// <param name="Identifier">Identifier</param>
        public Identifier_Info( string Identifier, string Type )
		{
			identifier = Identifier;
            type = Type;
            record_linkid = -1;
		}

        /// <summary> Constructor for a new instance of the Identifier_Info class </summary>
        /// <param name="Type">Uncontrolled Identifier Type</param>
        /// <param name="Identifier">Identifier</param>
        /// <param name="DisplayLabel">Additional text associated with the identifier necessary for display.</param>
        public Identifier_Info(string Identifier, string Type, string DisplayLabel)
        {
            identifier = Identifier;
            type = Type;
            displayLabel = DisplayLabel;
            record_linkid = -1;
        }

        /// <summary> Gets or sets the record link id which is used for saving to the tracking database  </summary>
        public int Record_LinkID
        {
            get { return record_linkid; }
            set { record_linkid = value; }
        }

		/// <summary> Gets or sets the identifier term for this identifier </summary>
		public string Identifier
		{
			get		{		return identifier ?? String.Empty;		}
			set		{		identifier = value;		}
		}

		/// <summary> Gets or sets the uncontrolled type of this identifier </summary>
        /// <remarks>There is no controlled list of identifier types</remarks>
        public string Type
		{
            get { return type ?? String.Empty; }
			set		{		type = value;		}
		}

        /// <summary> Gets or sets the additional text associated with the identifier which is necessary for display. </summary>
        public string Display_Label
        {
            get { return displayLabel ?? String.Empty; }
            set {   displayLabel = value;   }
        }

        /// <summary> Returns this identifier as a general string for display purposes </summary>
        /// <returns> This object in string format </returns>
        public override string ToString()
        {
            if (String.IsNullOrEmpty(identifier))
                return String.Empty;

            if ( !String.IsNullOrEmpty(type))
            {
                return base.Convert_String_To_XML_Safe(identifier + " (" + type + ")");
            }
            else
            {
                return base.Convert_String_To_XML_Safe(identifier);
            }
        }

        /// <summary> Clear the display label, identifier, and identifier type </summary>
        public void Clear()
        {
            displayLabel = null;
            identifier = null;
            type = null;
        }

        #region IEquatable Members

        /// <summary> Compares this object with another similarly typed object </summary>
        /// <param name="other">Similarly types object </param>
        /// <returns>TRUE if the two objects are sufficiently similar</returns>
        public bool Equals(Identifier_Info other)
        {
            if ( Identifier == other.Identifier )
                return true;
            else
                return false;
        }

        #endregion  

		/// <summary> Returns the identifier in Greenstone Archival format </summary>
		/// <returns>XML of this identifier in Greenstone Archival format </returns>
		internal string toGSA()
		{
            if (String.IsNullOrEmpty(identifier))
                return String.Empty;

            if (!String.IsNullOrEmpty(type))
			{
				return "<Metadata name=\"dc.Identifier\">(" + type + ") " + base.Convert_String_To_XML_Safe( identifier ) + "</Metadata>\r\n";
			}
			else
			{
				return "<Metadata name=\"dc.Identifier\">" + base.Convert_String_To_XML_Safe( identifier ) + "</Metadata>\r\n";
			}
		}

        /// <summary> Writes this identifier as MODS to a writer writing to a stream ( either a file or web response stream )</summary>
        /// <param name="returnValue"> Writer to the MODS building stream </param>
        internal void Add_MODS( System.IO.TextWriter returnValue)
        {
            if (String.IsNullOrEmpty(identifier))
                return;

            returnValue.Write( "<mods:identifier");
            if (!String.IsNullOrEmpty(type))
                returnValue.Write(" type=\"" + type + "\"");
            if (!String.IsNullOrEmpty(displayLabel))
                returnValue.Write(" displayLabel=\"" + displayLabel + "\"");
            returnValue.Write(">" + base.Convert_String_To_XML_Safe(identifier) + "</mods:identifier>\r\n");
        }
	}
}
