#region Using directives

using System;
using System.Collections.Generic;

#endregion

namespace SobekCM.Resource_Object.Configuration
{
    /// <summary> Standard metadata file types </summary>
    public enum Metadata_File_Type_Enum : byte
    {
        /// <summary> Dublin Core </summary>
        DC,

        /// <summary> Encoded Archival Descriptor (EAD) </summary>
        EAD,

        /// <summary> MARC21 Exchange Format </summary>
        MARC21,

        /// <summary> Marc21 in XML format </summary>
        MARCXML,

        /// <summary> Metadata Encoding and Transmission Scheme (METS) </summary>
        METS,

        /// <summary> Metadata Object Description Standard (MODS) </summary>
        MODS,

        /// <summary> Non-standard metadata </summary>
        OTHER
    }

    /// <summary> Configuration information about a reader/writer class used to read stand-along metadata files </summary>
    public class Metadata_File_ReaderWriter_Config
    {
        private Dictionary<string, string> options;

        /// <summary> Constructor for a new Metadata_File_ReaderWriter_Config object </summary>
        public Metadata_File_ReaderWriter_Config()
        {
            // Set some defaults
            MD_Type = Metadata_File_Type_Enum.OTHER;
            Other_MD_Type = String.Empty;
            Label = String.Empty;
            Code_Namespace = String.Empty;
            Code_Class = String.Empty;
            Code_Assembly = String.Empty;
            canRead = true;
            canWrite = true;

            // instantiate collections
            options = new Dictionary<string, string>();
        }

        /// <summary> Metadata type, if this is a standard metadata type </summary>
        public Metadata_File_Type_Enum MD_Type { get; set; }

        /// <summary> If this is a non-standard metadata type, code for this metadata type </summary>
        public string Other_MD_Type { get; set; }

        /// <summary> Short label which describes this metadata type </summary>
        public string Label { get; set; }

        /// <summary> Namespace within which this reader/writer class appears </summary>
        /// <remarks> For all standard reader/writers, this returns 'SobekCM.Resource_Object.Metadata_File_ReaderWriters'.<br /><br />
        /// This is used for instantiating the reader/writer class. </remarks>
        public string Code_Namespace { get; set; }

        /// <summary> Class name for the associated reader/writer </summary>
        /// <remarks> This is used for instantiating the reader/writer class. </remarks>
        public string Code_Class { get; set; }

        /// <summary> Assembly name of the DLL which holds this associated reader/writer </summary>
        /// <remarks> For all standard reader/writers, this is an empty string, since they are in this DLL. <br /><br />
        ///  This is used for instantiating the reader/writer class. </remarks>
        public string Code_Assembly { get; set; }

        /// <summary> Flag indicates if this class should be able to read a METS section </summary>
        public bool canRead { get; set; }

        /// <summary> Flag indicates if this class should be able to write a METS section </summary>
        public bool canWrite { get; set; }

        /// <summary> Dictionary of all the associated standard options, read from the metadata configuration XML file </summary>
        public Dictionary<string, string> Options
        {
            get { return options; }
        }

        /// <summary> Add a new option to the dictionary of associated standard options </summary>
        /// <param name="key"> Key of this option </param>
        /// <param name="value"> Value of this option </param>
        public void Add_Option(string key, string value)
        {
            options[key] = value;
        }
    }
}