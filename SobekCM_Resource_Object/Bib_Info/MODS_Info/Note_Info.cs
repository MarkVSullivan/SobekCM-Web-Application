#region Using directives

using System;
using System.IO;
using SobekCM.Resource_Object.MARC;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{

    #region Note_Type_Enumeration definition

    /// <summary> Type of note enumeration  </summary>
    public enum Note_Type_Enum
    {
        /// <summary> No note type is included </summary>
        NONE = 0,

        /// <summary> Acquisitions note </summary>
        Acquisition = 1,

        /// <summary> Action note indicates action taken, or recommended action </summary>
        Action,

        /// <summary> Additional physical form note </summary>
        AdditionalPhysicalForm,

        /// <summary> Bibliographic note </summary>
        Bibliography,

        /// <summary> Biographical note </summary>
        Biographical,

        /// <summary> Reference citation note </summary>
        CitationReference,

        /// <summary> Creation credits note </summary>
        CreationCredits,

        /// <summary> Sequential dates designation note </summary>
        DatesSequentialDesignation,

        /// <summary> Date and/or venue note </summary>
        DateVenue,

        /// <summary> Default type note, which is just used for the PROJECTS to 
        /// specify the default type of items which are generated from that proejct file </summary>
        DefaultType,

        /// <summary> Donation note </summary>
        Donation,

        /// <summary> Electronic access public note ( 856 |z ) </summary>
        ElectronicAccess,

        /// <summary> Exhibitions note </summary>
        Exhibitions,

        /// <summary> Funding note </summary>
        Funding,

        /// <summary> Internal comments, not shown in the public interface </summary>
        InternalComments,

        /// <summary> Issuing body note </summary>
        IssuingBody,

        /// <summary> Language note </summary>
        Language,

        /// <summary> Local note </summary>
        Local,

        /// <summary> Numbering peculiarities note </summary>
        NumberingPeculiarities,

        /// <summary> Original location note </summary>
        OriginalLocation,

        /// <summary> Original version note </summary>
        OriginalVersion,

        /// <summary> Ownership note </summary>
        Ownership,

        /// <summary> Performers note </summary>
        Performers,

        /// <summary> Preferred citation note </summary>
        PreferredCitation,

        /// <summary> Publications note </summary>
        Publications,

        /// <summary> Publication status note, used for the Institutional Repository </summary>
        PublicationStatus,

        /// <summary> Restriction on use note </summary>
        Restriction,

        /// <summary> Source note (particularly used for dublin core type mapping and mapped into the 786 ) </summary>
        Source,

        /// <summary> Source of description note ( maps to 788 ) </summary>
        SourceOfDescription,

        /// <summary> Statement of responsibility note ( from the 245 field ) </summary>
        StatementOfResponsibility,

		/// <summary> Supplementary notes </summary>
		Supplements,

        /// <summary> System details note </summary>
        SystemDetails,

        /// <summary> Thesis note  </summary>
        Thesis,

        /// <summary> Type of computer file or data note </summary>
        TypeOfFileOrData,

        /// <summary> Version identification note </summary>
        VersionIdentification
    }

    #endregion

    /// <summary> General textual information relating to a resource</summary>
    [Serializable]
    public class Note_Info : XML_Node_Base_Type, IEquatable<Note_Info>
    {
        private string displayLabel;
        private string note;
        private Note_Type_Enum note_type;

        #region Constructors 

        /// <summary> Constructor for a new instance of the Note_Info class </summary>
        public Note_Info()
        {
            note_type = Note_Type_Enum.NONE;
        }

        /// <summary> Constructor for a new instance of the Note_Info class </summary>
        /// <param name="Note">Text of the note</param>
        public Note_Info(string Note)
        {
            note_type = Note_Type_Enum.NONE;
            note = Note;
        }

        /// <summary> Constructor for a new instance of the Note_Info class </summary>
        /// <param name="Note">Text of the note</param>
        /// <param name="Note_Type">Type of Note</param>
        public Note_Info(string Note, string Note_Type)
        {
            Note_Type_String = Note_Type;
            note = Note;
        }

        /// <summary> Constructor for a new instance of the Note_Info class </summary>
        /// <param name="Note">Text of the note</param>
        /// <param name="Note_Type">Type of Note</param>
        public Note_Info(string Note, Note_Type_Enum Note_Type)
        {
            note_type = Note_Type;
            note = Note;
        }

        /// <summary> Constructor for a new instance of the Note_Info class </summary>
        /// <param name="Note">Text of the note</param>
        /// <param name="Note_Type">Type of Note</param>
        /// <param name="Display_Label">Display Label for this note</param>
        public Note_Info(string Note, Note_Type_Enum Note_Type, string Display_Label)
        {
            note_type = Note_Type;
            note = Note;
            displayLabel = Display_Label;
        }

        /// <summary> Constructor for a new instance of the Note_Info class </summary>
        /// <param name="Note">Text of the note</param>
        /// <param name="Note_Type">Type of Note</param>
        /// <param name="Display_Label">Display Label for this note</param>
        public Note_Info(string Note, string Note_Type, string Display_Label)
        {
            Note_Type_String = Note_Type;
            note = Note;
            displayLabel = Display_Label;
        }

        #endregion

        /// <summary> Gets or sets the text of the note </summary>
        public string Note
        {
            get { return note ?? String.Empty; }
            set { note = value; }
        }

        /// <summary> Gets or sets the type of the note </summary>
        public Note_Type_Enum Note_Type
        {
            get { return note_type; }
            set { note_type = value; }
        }

        /// <summary> Gets or sets the any additional information needed for display of the note </summary>
        public string Display_Label
        {
            get { return displayLabel ?? String.Empty; }
            set { displayLabel = value; }
        }

        internal string Note_XML
        {
            get { return Convert_String_To_XML_Safe(note); }
        }

        #region IEquatable<Note_Info> Members

        /// <summary> Compares this object with another similarly typed object </summary>
        /// <param name="Other">Similarly types object </param>
        /// <returns>TRUE if the two objects are sufficiently similar</returns>
        public bool Equals(Note_Info Other)
        {
            return ( String.Compare(Note, Other.Note, StringComparison.Ordinal) == 0) && (Note_Type == Other.Note_Type);
        }

        #endregion

        /// <summary> Writes the information about this note as a simple string </summary>
        /// <returns> Note information as string </returns>
        public override string ToString()
        {
            if (String.IsNullOrEmpty(note))
                return String.Empty;

            string type = Note_Type_Display_String;
            if (!String.IsNullOrEmpty(type))
            {
                return "<b>" + type + "</b> " + Convert_String_To_XML_Safe(note);
            }
            
            return Convert_String_To_XML_Safe(note);
        }

        internal MARC_Field to_MARC_HTML()
        {
            if (String.IsNullOrEmpty(note))
                return null;

            MARC_Field returnValue = new MARC_Field
            {
                Indicators = "  ", 
                Control_Field_Value = "|a " + note
            };

            switch (note_type)
            {
                case Note_Type_Enum.Thesis:
                    returnValue.Tag = 502;
                    break;

                case Note_Type_Enum.Bibliography:
                    returnValue.Tag = 504;
                    break;

                case Note_Type_Enum.Restriction:
                    returnValue.Tag = 506;
                    break;

                case Note_Type_Enum.CreationCredits:
                    returnValue.Tag = 508;
                    break;

                case Note_Type_Enum.CitationReference:
                    returnValue.Tag = 510;
                    break;

                case Note_Type_Enum.Performers:
                    returnValue.Tag = 511;
                    if ((displayLabel != null) && (displayLabel == "cast"))
                        returnValue.Indicators = "1 ";
                    break;

                case Note_Type_Enum.TypeOfFileOrData:
                    returnValue.Tag = 516;
                    break;

                case Note_Type_Enum.DateVenue:
                    returnValue.Tag = 518;
					if (!String.IsNullOrEmpty(displayLabel))
                        returnValue.Control_Field_Value = returnValue.Control_Field_Value + " |3 " + displayLabel;
                    break;

                case Note_Type_Enum.PreferredCitation:
                    returnValue.Tag = 524;
                    if (!String.IsNullOrEmpty(displayLabel))
                        returnValue.Control_Field_Value = returnValue.Control_Field_Value + " |3 " + displayLabel;
                    break;

				case Note_Type_Enum.Supplements:
					returnValue.Tag = 525;
					if (!String.IsNullOrEmpty(displayLabel))
						returnValue.Control_Field_Value = returnValue.Control_Field_Value + " |3 " + displayLabel;
					break;

                case Note_Type_Enum.AdditionalPhysicalForm:
                    returnValue.Tag = 530;
                    break;

                case Note_Type_Enum.OriginalVersion:
                    returnValue.Tag = 534;
                    break;

                case Note_Type_Enum.OriginalLocation:
                    returnValue.Tag = 535;
                    break;

                case Note_Type_Enum.Funding:
                    returnValue.Tag = 536;
                    break;

                case Note_Type_Enum.SystemDetails:
                    returnValue.Tag = 538;
                    break;

                case Note_Type_Enum.Acquisition:
                    returnValue.Tag = 541;
                    break;

                case Note_Type_Enum.Biographical:
                    returnValue.Tag = 545;
                    break;

                case Note_Type_Enum.Language:
                    returnValue.Tag = 546;
                    break;

                case Note_Type_Enum.Ownership:
                    returnValue.Tag = 561;
                    if (!String.IsNullOrEmpty(displayLabel))
                        returnValue.Control_Field_Value = returnValue.Control_Field_Value + " |3 " + displayLabel;
                    break;

                case Note_Type_Enum.VersionIdentification:
                    returnValue.Tag = 562;
                    break;

                case Note_Type_Enum.Publications:
                    returnValue.Tag = 581;
                    if (!String.IsNullOrEmpty(displayLabel))
                        returnValue.Control_Field_Value = returnValue.Control_Field_Value + " |3 " + displayLabel;
                    break;

                case Note_Type_Enum.Action:
                    returnValue.Tag = 583;
                    if (!String.IsNullOrEmpty(displayLabel))
                        returnValue.Control_Field_Value = "|a " + displayLabel + " |l " + note;
                    break;

                case Note_Type_Enum.Exhibitions:
                    returnValue.Tag = 585;
                    if (!String.IsNullOrEmpty(displayLabel))
                        returnValue.Control_Field_Value = returnValue.Control_Field_Value + " |3 " + displayLabel;
                    break;

                case Note_Type_Enum.IssuingBody:
                    returnValue.Tag = 550;
                    break;

                case Note_Type_Enum.NumberingPeculiarities:
                    returnValue.Tag = 515;
                    break;

                case Note_Type_Enum.DatesSequentialDesignation:
                    returnValue.Tag = 362;
                    returnValue.Indicators = "1 ";
                    if (!String.IsNullOrEmpty(displayLabel))
                    {
                        returnValue.Control_Field_Value = returnValue.Control_Field_Value + " |z " + displayLabel;
                    }
                    break;

                case Note_Type_Enum.Source:
                    returnValue.Tag = 786;
                    returnValue.Indicators = "0 ";
                    returnValue.Control_Field_Value = "|n " + note;
                    break;

                case Note_Type_Enum.Local:
                    returnValue.Tag = 590;
                    break;

                case Note_Type_Enum.SourceOfDescription:
                    returnValue.Tag = 588;
                    break;

                case Note_Type_Enum.InternalComments:
                    return null;

                case Note_Type_Enum.PublicationStatus:
                    return null;

                case Note_Type_Enum.DefaultType:
                    return null;

                default:
                    returnValue.Tag = 500;
                    if (!String.IsNullOrEmpty(displayLabel))
                    {
                        switch (displayLabel.ToUpper())
                        {
                            case "GEOGRAPHIC COVERAGE":
                                returnValue.Tag = 522;
                                break;

                            case "SUPPLEMENT NOTE":
                                returnValue.Tag = 525;
                                break;

                            case "METHODOLOGY":
                                returnValue.Tag = 567;
                                break;

                            case "AWARDS":
                                returnValue.Tag = 586;
                                break;
                        }
                    }
                    break;
            }

            return returnValue;
        }

        internal string To_MODS()
        {
            if (String.IsNullOrEmpty(note))
                return String.Empty;

            string id_string = String.Empty;
            string actual_id = ID;
            if (ID.Length > 0)
            {
                id_string = " ID=\"" + actual_id + "\"";
            }

            if (note_type == Note_Type_Enum.NONE)
            {
                if (!String.IsNullOrEmpty(displayLabel))
                {
                    return "<mods:note" + id_string + " displayLabel=\"" + Convert_String_To_XML_Safe(displayLabel) + "\">" + Convert_String_To_XML_Safe(note) + "</mods:note>\r\n";
                }
                
                return "<mods:note" + id_string + ">" + Convert_String_To_XML_Safe(note) + "</mods:note>\r\n";
            }
            
            if (!String.IsNullOrEmpty(displayLabel))
            {
                return "<mods:note" + id_string + " type=\"" + Note_Type_String + "\" displayLabel=\"" + Convert_String_To_XML_Safe(displayLabel) + "\">" + Convert_String_To_XML_Safe(note) + "</mods:note>\r\n";
            }
            
            return "<mods:note" + id_string + " type=\"" + Note_Type_String + "\">" + Convert_String_To_XML_Safe(note) + "</mods:note>\r\n";
        }

        internal void Add_MODS(TextWriter Result)
        {
            Result.Write(To_MODS());
        }

        internal Note_Info Clone()
        {
            return new Note_Info(note, note_type, displayLabel);
        }

        #region Property to convert from the Note_Type_Enum to String and back

        /// <summary> Return the note type as a string </summary>
        public string Note_Type_Display_String
        {
            get
            {
                switch (note_type)
                {
                    case Note_Type_Enum.StatementOfResponsibility:
                        return "Statement of Responsibility";

                    case Note_Type_Enum.Thesis:
                        return "Thesis";

                    case Note_Type_Enum.Bibliography:
                        return "Bibliography";

                    case Note_Type_Enum.Restriction:
                        return "Restriction";

                    case Note_Type_Enum.CreationCredits:
                        return "Creation/Production Credits";

                    case Note_Type_Enum.CitationReference:
                        return "Citation/Reference";

                    case Note_Type_Enum.Performers:
                        return "Performers";

                    case Note_Type_Enum.DateVenue:
                        return "Venue";

                    case Note_Type_Enum.PreferredCitation:
                        return "Preferred Citation";

                    case Note_Type_Enum.AdditionalPhysicalForm:
                        return "Additional Physical Form";

                    case Note_Type_Enum.OriginalVersion:
                        return "Original Version";

                    case Note_Type_Enum.OriginalLocation:
                        return "Original Location";

                    case Note_Type_Enum.Funding:
                        return "Funding";

                    case Note_Type_Enum.SystemDetails:
                        return "System Details";

                    case Note_Type_Enum.Acquisition:
                        return "Acquisition";

                    case Note_Type_Enum.Biographical:
                        return "Biographical";

                    case Note_Type_Enum.Language:
                        return "Language";

                    case Note_Type_Enum.Ownership:
                        return "Ownership";

                    case Note_Type_Enum.VersionIdentification:
                        return "Version Identification";

                    case Note_Type_Enum.Publications:
                        return "Publications";

                    case Note_Type_Enum.Exhibitions:
                        return "Exhibitions";

                    case Note_Type_Enum.NumberingPeculiarities:
                        return "Numbering Peculiarities";

                    case Note_Type_Enum.DatesSequentialDesignation:
                        return "Dates or Sequential Designation";

                    case Note_Type_Enum.IssuingBody:
                        return "Issuing Body";

                    case Note_Type_Enum.Donation:
                        return "Donation";

                    case Note_Type_Enum.InternalComments:
                        return "Internal Comments";

                    case Note_Type_Enum.PublicationStatus:
                        return "Publication Status";

                    case Note_Type_Enum.DefaultType:
                        return "Default Type";

                    case Note_Type_Enum.Source:
                        return "Source";

                    case Note_Type_Enum.SourceOfDescription:
                        return "Source of Description";

                    case Note_Type_Enum.TypeOfFileOrData:
                        return "Type of File or Data";

                    case Note_Type_Enum.Local:
                        return "Local";

                    case Note_Type_Enum.ElectronicAccess:
                        return "Electronic Access";

					case Note_Type_Enum.Supplements:
						return "Supplements";

                    case Note_Type_Enum.Action:
                        return "Action";

                    default:
                        return String.Empty;
                }
            }
        }

        /// <summary> Returns this type of note as a string </summary>
        public string Note_Type_String
        {
            get
            {
                switch (note_type)
                {
                    case Note_Type_Enum.StatementOfResponsibility:
                        return "statement of responsibility";

                    case Note_Type_Enum.Thesis:
                        return "thesis";

                    case Note_Type_Enum.Bibliography:
                        return "bibliography";

                    case Note_Type_Enum.Restriction:
                        return "restriction";

                    case Note_Type_Enum.CreationCredits:
                        return "creation/production credits";

                    case Note_Type_Enum.CitationReference:
                        return "citation/reference";

                    case Note_Type_Enum.Performers:
                        return "performers";

                    case Note_Type_Enum.DateVenue:
                        return "venue";

                    case Note_Type_Enum.PreferredCitation:
                        return "preferred citation";

                    case Note_Type_Enum.AdditionalPhysicalForm:
                        return "additional physical form";

                    case Note_Type_Enum.OriginalVersion:
                        return "original version";

                    case Note_Type_Enum.OriginalLocation:
                        return "original location";

                    case Note_Type_Enum.Funding:
                        return "funding";

                    case Note_Type_Enum.SystemDetails:
                        return "system details";

                    case Note_Type_Enum.Acquisition:
                        return "acquisition";

                    case Note_Type_Enum.Biographical:
                        return "biographical";

                    case Note_Type_Enum.Language:
                        return "language";

                    case Note_Type_Enum.Ownership:
                        return "ownership";

                    case Note_Type_Enum.VersionIdentification:
                        return "version identification";

                    case Note_Type_Enum.Publications:
                        return "publications";

                    case Note_Type_Enum.Exhibitions:
                        return "exhibitions";

                    case Note_Type_Enum.NumberingPeculiarities:
                        return "numbering peculiarities";

                    case Note_Type_Enum.DatesSequentialDesignation:
                        return "dates or sequential designation";

                    case Note_Type_Enum.IssuingBody:
                        return "issuing body";

                    case Note_Type_Enum.Donation:
                        return "donation";

                    case Note_Type_Enum.InternalComments:
                        return "internal";

                    case Note_Type_Enum.PublicationStatus:
                        return "publication status";

                    case Note_Type_Enum.DefaultType:
                        return "default type";

                    case Note_Type_Enum.Source:
                        return "source";

                    case Note_Type_Enum.SourceOfDescription:
                        return "source of description";

                    case Note_Type_Enum.TypeOfFileOrData:
                        return "type of file or data";

                    case Note_Type_Enum.Local:
                        return "local";

                    case Note_Type_Enum.ElectronicAccess:
                        return "electronic access";

					case Note_Type_Enum.Supplements:
		                return "supplements";

                    case Note_Type_Enum.Action:
                        return "action";

                    default:
                        return String.Empty;
                }
            }
            set
            {
                switch (value.ToLower())
                {
                    case "statement of responsibility":
                        Note_Type = Note_Type_Enum.StatementOfResponsibility;
                        break;

                    case "thesis":
                        Note_Type = Note_Type_Enum.Thesis;
                        break;

                    case "bibliography":
                        Note_Type = Note_Type_Enum.Bibliography;
                        break;

                    case "restriction":
                        Note_Type = Note_Type_Enum.Restriction;
                        break;

                    case "creation/production credits":
                        Note_Type = Note_Type_Enum.CreationCredits;
                        break;

                    case "citation/reference":
                        Note_Type = Note_Type_Enum.CitationReference;
                        break;

                    case "performers":
                        Note_Type = Note_Type_Enum.Performers;
                        break;

                    case "venue":
                        Note_Type = Note_Type_Enum.DateVenue;
                        break;

                    case "preferred citation":
                        Note_Type = Note_Type_Enum.PreferredCitation;
                        break;

                    case "additional physical form":
                        Note_Type = Note_Type_Enum.AdditionalPhysicalForm;
                        break;

                    case "original version":
                        Note_Type = Note_Type_Enum.OriginalVersion;
                        break;

                    case "original location":
                        Note_Type = Note_Type_Enum.OriginalLocation;
                        break;

                    case "funding":
                        Note_Type = Note_Type_Enum.Funding;
                        break;

                    case "system details":
                        Note_Type = Note_Type_Enum.SystemDetails;
                        break;

                    case "acquisition":
                        Note_Type = Note_Type_Enum.Acquisition;
                        break;

                    case "biographical":
                        Note_Type = Note_Type_Enum.Biographical;
                        break;

                    case "language":
                        Note_Type = Note_Type_Enum.Language;
                        break;

                    case "ownership":
                        Note_Type = Note_Type_Enum.Ownership;
                        break;

                    case "version identification":
                        Note_Type = Note_Type_Enum.VersionIdentification;
                        break;

                    case "publications":
                        Note_Type = Note_Type_Enum.Publications;
                        break;

                    case "exhibitions":
                        Note_Type = Note_Type_Enum.Exhibitions;
                        break;

                    case "numbering peculiarities":
                        Note_Type = Note_Type_Enum.NumberingPeculiarities;
                        break;

                    case "dates or sequential designation":
                        Note_Type = Note_Type_Enum.DatesSequentialDesignation;
                        break;

                    case "issuing body":
                        Note_Type = Note_Type_Enum.IssuingBody;
                        break;

                    case "donation":
                        Note_Type = Note_Type_Enum.Donation;
                        break;

                    case "internal":
                    case "internal comments":
                        Note_Type = Note_Type_Enum.InternalComments;
                        break;

                    case "publication status":
                        Note_Type = Note_Type_Enum.PublicationStatus;
                        break;

                    case "default type":
                        Note_Type = Note_Type_Enum.DefaultType;
                        break;

                    case "source":
                        Note_Type = Note_Type_Enum.Source;
                        break;

                    case "source of description":
                        Note_Type = Note_Type_Enum.SourceOfDescription;
                        break;

                    case "type of file or data":
                        Note_Type = Note_Type_Enum.TypeOfFileOrData;
                        return;

                    case "local":
                        Note_Type = Note_Type_Enum.Local;
                        return;

                    case "electronic access":
                    case "electonic access":
                        Note_Type = Note_Type_Enum.ElectronicAccess;
                        return;

					case "supplements":
						Note_Type = Note_Type_Enum.Supplements;
		                break;

                    case "action":
                        Note_Type = Note_Type_Enum.Action;
                        break;

                    default:
                        Note_Type = Note_Type_Enum.NONE;
                        break;
                }
            }
        }

        #endregion
    }
}