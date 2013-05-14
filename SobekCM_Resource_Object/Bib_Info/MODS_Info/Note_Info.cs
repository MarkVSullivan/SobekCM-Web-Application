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
        acquisition = 1,

        /// <summary> Additional physical form note </summary>
        additional_physical_form,

        /// <summary> Bibliographic note </summary>
        bibliography,

        /// <summary> Biographical note </summary>
        biographical,

        /// <summary> Reference citation note </summary>
        citation_reference,

        /// <summary> Creation credits note </summary>
        creation_credits,

        /// <summary> Sequential dates designation note </summary>
        dates_sequential_designation,

        /// <summary> Date and/or venue note </summary>
        date_venue,

        /// <summary> Default type note, which is just used for the PROJECTS to 
        /// specify the default type of items which are generated from that proejct file </summary>
        default_type,

        /// <summary> Donation note </summary>
        donation,

        /// <summary> Electronic access public note ( 856 |z ) </summary>
        electronic_access,

        /// <summary> Exhibitions note </summary>
        exhibitions,

        /// <summary> Funding note </summary>
        funding,

        /// <summary> Internal comments, not shown in the public interface </summary>
        internal_comments,

        /// <summary> Issuing body note </summary>
        issuing_body,

        /// <summary> Language note </summary>
        language,

        /// <summary> Local note </summary>
        local,

        /// <summary> Numbering peculiarities note </summary>
        numbering_peculiarities,

        /// <summary> Original location note </summary>
        original_location,

        /// <summary> Original version note </summary>
        original_version,

        /// <summary> Ownership note </summary>
        ownership,

        /// <summary> Performers note </summary>
        performers,

        /// <summary> Preferred citation note </summary>
        preferred_citation,

        /// <summary> Publications note </summary>
        publications,

        /// <summary> Publication status note, used for the Institutional Repository </summary>
        publication_status,

        /// <summary> Restriction on use note </summary>
        restriction,

        /// <summary> Source note (particularly used for dublin core type mapping and mapped into the 786 ) </summary>
        source,

        /// <summary> Source of description note ( maps to 788 ) </summary>
        source_of_description,

        /// <summary> Statement of responsibility note ( from the 245 field ) </summary>
        statement_of_responsibility,

        /// <summary> System details note </summary>
        system_details,

        /// <summary> Thesis note  </summary>
        thesis,

        /// <summary> Type of computer file or data note </summary>
        type_of_file_or_data,

        /// <summary> Version identification note </summary>
        version_identification
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
        public Note_Info() : base()
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
            get { return base.Convert_String_To_XML_Safe(note); }
        }

        #region IEquatable<Note_Info> Members

        /// <summary> Compares this object with another similarly typed object </summary>
        /// <param name="other">Similarly types object </param>
        /// <returns>TRUE if the two objects are sufficiently similar</returns>
        public bool Equals(Note_Info other)
        {
            if ((Note == other.Note) && (Note_Type == other.Note_Type))
                return true;
            else
                return false;
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
                return "<b>" + type + "</b> " + base.Convert_String_To_XML_Safe(note);
            }
            else
            {
                return base.Convert_String_To_XML_Safe(note);
            }
        }

        internal MARC_Field to_MARC_HTML()
        {
            if (String.IsNullOrEmpty(note))
                return null;

            MARC_Field returnValue = new MARC_Field();
            returnValue.Indicators = "  ";
            returnValue.Control_Field_Value = "|a " + note;

            switch (note_type)
            {
                case Note_Type_Enum.thesis:
                    returnValue.Tag = 502;
                    break;

                case Note_Type_Enum.bibliography:
                    returnValue.Tag = 504;
                    break;

                case Note_Type_Enum.restriction:
                    returnValue.Tag = 506;
                    break;

                case Note_Type_Enum.creation_credits:
                    returnValue.Tag = 508;
                    break;

                case Note_Type_Enum.citation_reference:
                    returnValue.Tag = 510;
                    break;

                case Note_Type_Enum.performers:
                    returnValue.Tag = 511;
                    if ((displayLabel != null) && (displayLabel == "cast"))
                        returnValue.Indicators = "1 ";
                    break;

                case Note_Type_Enum.type_of_file_or_data:
                    returnValue.Tag = 516;
                    break;

                case Note_Type_Enum.date_venue:
                    returnValue.Tag = 518;
                    if (displayLabel.Length > 0)
                        returnValue.Control_Field_Value = returnValue.Control_Field_Value + " |3 " + displayLabel;
                    break;

                case Note_Type_Enum.preferred_citation:
                    returnValue.Tag = 524;
                    if (!String.IsNullOrEmpty(displayLabel))
                        returnValue.Control_Field_Value = returnValue.Control_Field_Value + " |3 " + displayLabel;
                    break;

                case Note_Type_Enum.additional_physical_form:
                    returnValue.Tag = 530;
                    break;

                case Note_Type_Enum.original_version:
                    returnValue.Tag = 534;
                    break;

                case Note_Type_Enum.original_location:
                    returnValue.Tag = 535;
                    break;

                case Note_Type_Enum.funding:
                    returnValue.Tag = 536;
                    break;

                case Note_Type_Enum.system_details:
                    returnValue.Tag = 538;
                    break;

                case Note_Type_Enum.acquisition:
                    returnValue.Tag = 541;
                    break;

                case Note_Type_Enum.biographical:
                    returnValue.Tag = 545;
                    break;

                case Note_Type_Enum.language:
                    returnValue.Tag = 546;
                    break;

                case Note_Type_Enum.ownership:
                    returnValue.Tag = 561;
                    if (!String.IsNullOrEmpty(displayLabel))
                        returnValue.Control_Field_Value = returnValue.Control_Field_Value + " |3 " + displayLabel;
                    break;

                case Note_Type_Enum.version_identification:
                    returnValue.Tag = 562;
                    break;

                case Note_Type_Enum.publications:
                    returnValue.Tag = 581;
                    if (!String.IsNullOrEmpty(displayLabel))
                        returnValue.Control_Field_Value = returnValue.Control_Field_Value + " |3 " + displayLabel;
                    break;

                case Note_Type_Enum.exhibitions:
                    returnValue.Tag = 585;
                    if (!String.IsNullOrEmpty(displayLabel))
                        returnValue.Control_Field_Value = returnValue.Control_Field_Value + " |3 " + displayLabel;
                    break;

                case Note_Type_Enum.issuing_body:
                    returnValue.Tag = 550;
                    break;

                case Note_Type_Enum.numbering_peculiarities:
                    returnValue.Tag = 515;
                    break;

                case Note_Type_Enum.dates_sequential_designation:
                    returnValue.Tag = 362;
                    returnValue.Indicators = "1 ";
                    if (!String.IsNullOrEmpty(displayLabel))
                    {
                        returnValue.Control_Field_Value = returnValue.Control_Field_Value + " |z " + displayLabel;
                    }
                    break;

                case Note_Type_Enum.source:
                    returnValue.Tag = 786;
                    returnValue.Indicators = "0 ";
                    returnValue.Control_Field_Value = "|n " + note;
                    break;

                case Note_Type_Enum.local:
                    returnValue.Tag = 590;
                    break;

                case Note_Type_Enum.source_of_description:
                    returnValue.Tag = 588;
                    break;

                case Note_Type_Enum.internal_comments:
                    return null;

                case Note_Type_Enum.publication_status:
                    return null;

                case Note_Type_Enum.default_type:
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
            string actual_id = base.ID;
            if (base.ID.Length > 0)
            {
                id_string = " ID=\"" + actual_id + "\"";
            }

            if (note_type == Note_Type_Enum.NONE)
            {
                if (!String.IsNullOrEmpty(displayLabel))
                {
                    return "<mods:note" + id_string + " displayLabel=\"" + base.Convert_String_To_XML_Safe(displayLabel) + "\">" + base.Convert_String_To_XML_Safe(note) + "</mods:note>\r\n";
                }
                else
                {
                    return "<mods:note" + id_string + ">" + base.Convert_String_To_XML_Safe(note) + "</mods:note>\r\n";
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(displayLabel))
                {
                    return "<mods:note" + id_string + " type=\"" + Note_Type_String + "\" displayLabel=\"" + base.Convert_String_To_XML_Safe(displayLabel) + "\">" + base.Convert_String_To_XML_Safe(note) + "</mods:note>\r\n";
                }
                else
                {
                    return "<mods:note" + id_string + " type=\"" + Note_Type_String + "\">" + base.Convert_String_To_XML_Safe(note) + "</mods:note>\r\n";
                }
            }
        }

        internal void Add_MODS(TextWriter result)
        {
            result.Write(To_MODS());
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
                    case Note_Type_Enum.statement_of_responsibility:
                        return "Statement of Responsibility";

                    case Note_Type_Enum.thesis:
                        return "Thesis";

                    case Note_Type_Enum.bibliography:
                        return "Bibliography";

                    case Note_Type_Enum.restriction:
                        return "Restriction";

                    case Note_Type_Enum.creation_credits:
                        return "Creation/Production Credits";

                    case Note_Type_Enum.citation_reference:
                        return "Citation/Reference";

                    case Note_Type_Enum.performers:
                        return "Performers";

                    case Note_Type_Enum.date_venue:
                        return "Venue";

                    case Note_Type_Enum.preferred_citation:
                        return "Preferred Citation";

                    case Note_Type_Enum.additional_physical_form:
                        return "Additional Physical Form";

                    case Note_Type_Enum.original_version:
                        return "Original Version";

                    case Note_Type_Enum.original_location:
                        return "Original Location";

                    case Note_Type_Enum.funding:
                        return "Funding";

                    case Note_Type_Enum.system_details:
                        return "System Details";

                    case Note_Type_Enum.acquisition:
                        return "Acquisition";

                    case Note_Type_Enum.biographical:
                        return "Biographical";

                    case Note_Type_Enum.language:
                        return "Language";

                    case Note_Type_Enum.ownership:
                        return "Ownership";

                    case Note_Type_Enum.version_identification:
                        return "Version Identification";

                    case Note_Type_Enum.publications:
                        return "Publications";

                    case Note_Type_Enum.exhibitions:
                        return "Exhibitions";

                    case Note_Type_Enum.numbering_peculiarities:
                        return "Numbering Peculiarities";

                    case Note_Type_Enum.dates_sequential_designation:
                        return "Dates or Sequential Designation";

                    case Note_Type_Enum.issuing_body:
                        return "Issuing Body";

                    case Note_Type_Enum.donation:
                        return "Donation";

                    case Note_Type_Enum.internal_comments:
                        return "Internal Comments";

                    case Note_Type_Enum.publication_status:
                        return "Publication Status";

                    case Note_Type_Enum.default_type:
                        return "Default Type";

                    case Note_Type_Enum.source:
                        return "Source";

                    case Note_Type_Enum.source_of_description:
                        return "Source of Description";

                    case Note_Type_Enum.type_of_file_or_data:
                        return "Type of File or Data";

                    case Note_Type_Enum.local:
                        return "Local";

                    case Note_Type_Enum.electronic_access:
                        return "Electronic Access";

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
                    case Note_Type_Enum.statement_of_responsibility:
                        return "statement of responsibility";

                    case Note_Type_Enum.thesis:
                        return "thesis";

                    case Note_Type_Enum.bibliography:
                        return "bibliography";

                    case Note_Type_Enum.restriction:
                        return "restriction";

                    case Note_Type_Enum.creation_credits:
                        return "creation/production credits";

                    case Note_Type_Enum.citation_reference:
                        return "citation/reference";

                    case Note_Type_Enum.performers:
                        return "performers";

                    case Note_Type_Enum.date_venue:
                        return "venue";

                    case Note_Type_Enum.preferred_citation:
                        return "preferred citation";

                    case Note_Type_Enum.additional_physical_form:
                        return "additional physical form";

                    case Note_Type_Enum.original_version:
                        return "original version";

                    case Note_Type_Enum.original_location:
                        return "original location";

                    case Note_Type_Enum.funding:
                        return "funding";

                    case Note_Type_Enum.system_details:
                        return "system details";

                    case Note_Type_Enum.acquisition:
                        return "acquisition";

                    case Note_Type_Enum.biographical:
                        return "biographical";

                    case Note_Type_Enum.language:
                        return "language";

                    case Note_Type_Enum.ownership:
                        return "ownership";

                    case Note_Type_Enum.version_identification:
                        return "version identification";

                    case Note_Type_Enum.publications:
                        return "publications";

                    case Note_Type_Enum.exhibitions:
                        return "exhibitions";

                    case Note_Type_Enum.numbering_peculiarities:
                        return "numbering peculiarities";

                    case Note_Type_Enum.dates_sequential_designation:
                        return "dates or sequential designation";

                    case Note_Type_Enum.issuing_body:
                        return "issuing body";

                    case Note_Type_Enum.donation:
                        return "donation";

                    case Note_Type_Enum.internal_comments:
                        return "internal";

                    case Note_Type_Enum.publication_status:
                        return "publication status";

                    case Note_Type_Enum.default_type:
                        return "default type";

                    case Note_Type_Enum.source:
                        return "source";

                    case Note_Type_Enum.source_of_description:
                        return "source of description";

                    case Note_Type_Enum.type_of_file_or_data:
                        return "type of file or data";

                    case Note_Type_Enum.local:
                        return "local";

                    case Note_Type_Enum.electronic_access:
                        return "electronic access";

                    default:
                        return String.Empty;
                }
            }
            set
            {
                switch (value.ToLower())
                {
                    case "statement of responsibility":
                        Note_Type = Note_Type_Enum.statement_of_responsibility;
                        break;

                    case "thesis":
                        Note_Type = Note_Type_Enum.thesis;
                        break;

                    case "bibliography":
                        Note_Type = Note_Type_Enum.bibliography;
                        break;

                    case "restriction":
                        Note_Type = Note_Type_Enum.restriction;
                        break;

                    case "creation/production credits":
                        Note_Type = Note_Type_Enum.creation_credits;
                        break;

                    case "citation/reference":
                        Note_Type = Note_Type_Enum.citation_reference;
                        break;

                    case "performers":
                        Note_Type = Note_Type_Enum.performers;
                        break;

                    case "venue":
                        Note_Type = Note_Type_Enum.date_venue;
                        break;

                    case "preferred citation":
                        Note_Type = Note_Type_Enum.preferred_citation;
                        break;

                    case "additional physical form":
                        Note_Type = Note_Type_Enum.additional_physical_form;
                        break;

                    case "original version":
                        Note_Type = Note_Type_Enum.original_version;
                        break;

                    case "original location":
                        Note_Type = Note_Type_Enum.original_location;
                        break;

                    case "funding":
                        Note_Type = Note_Type_Enum.funding;
                        break;

                    case "system details":
                        Note_Type = Note_Type_Enum.system_details;
                        break;

                    case "acquisition":
                        Note_Type = Note_Type_Enum.acquisition;
                        break;

                    case "biographical":
                        Note_Type = Note_Type_Enum.biographical;
                        break;

                    case "language":
                        Note_Type = Note_Type_Enum.language;
                        break;

                    case "ownership":
                        Note_Type = Note_Type_Enum.ownership;
                        break;

                    case "version identification":
                        Note_Type = Note_Type_Enum.version_identification;
                        break;

                    case "publications":
                        Note_Type = Note_Type_Enum.publications;
                        break;

                    case "exhibitions":
                        Note_Type = Note_Type_Enum.exhibitions;
                        break;

                    case "numbering peculiarities":
                        Note_Type = Note_Type_Enum.numbering_peculiarities;
                        break;

                    case "dates or sequential designation":
                        Note_Type = Note_Type_Enum.dates_sequential_designation;
                        break;

                    case "issuing body":
                        Note_Type = Note_Type_Enum.issuing_body;
                        break;

                    case "donation":
                        Note_Type = Note_Type_Enum.donation;
                        break;

                    case "internal":
                    case "internal comments":
                        Note_Type = Note_Type_Enum.internal_comments;
                        break;

                    case "publication status":
                        Note_Type = Note_Type_Enum.publication_status;
                        break;

                    case "default type":
                        Note_Type = Note_Type_Enum.default_type;
                        break;

                    case "source":
                        Note_Type = Note_Type_Enum.source;
                        break;

                    case "source of description":
                        Note_Type = Note_Type_Enum.source_of_description;
                        break;

                    case "type of file or data":
                        Note_Type = Note_Type_Enum.type_of_file_or_data;
                        return;

                    case "local":
                        Note_Type = Note_Type_Enum.local;
                        return;

                    case "electronic access":
                    case "electonic access":
                        Note_Type = Note_Type_Enum.electronic_access;
                        return;

                    default:
                        Note_Type = Note_Type_Enum.NONE;
                        break;
                }
            }
        }

        #endregion
    }
}