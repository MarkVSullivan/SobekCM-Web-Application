#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

#endregion

namespace SobekCM.Resource_Object.Configuration
{
    /// <summary> Type of METS administrative section supported by this reader/writer </summary>
    public enum METS_amdSec_Type_Enum : byte
    {
        /// <summary> Not specified - DEFAULT </summary>
        UNSPECIFIED,

        /// <summary> digiProvMD amdSec</summary>
        digiProvMD,

        /// <summary> rightsMD amdSec </summary>
        rightsMD,

        /// <summary> sourceMD amdSec </summary>
        sourceMD,

        /// <summary> techMD amdSec </summary>
        techMD
    }

    /// <summary> Type of METS section supported by this reader/writer </summary>
    public enum METS_Section_Type_Enum : byte
    {
        /// <summary> Not specified - DEFAULT </summary>
        UNSPECIFIED,

        /// <summary> Administrative section </summary>
        amdSec,

        /// <summary> Descriptive (bibliographic) section </summary>
        dmdSec,
    }

    /// <summary> Configuration information about a reader/writer class used to read or
    /// write a section within a METS file </summary>
    public class METS_Section_ReaderWriter_Config
    {
        private METS_Section_ReaderWriter_Mapping defaultMapping;
        private List<METS_Section_ReaderWriter_Mapping> mappings;
        private Dictionary<string, string> options;

        /// <summary> Constructor for a new METS_Section_ReaderWriter_Config object </summary>
        public METS_Section_ReaderWriter_Config()
        {
            // Set some defaults
            isActive = true;
            Label = String.Empty;
            Code_Namespace = String.Empty;
            Code_Class = String.Empty;
            Code_Assembly = String.Empty;
            ID = String.Empty;
            METS_Section = METS_Section_Type_Enum.UNSPECIFIED;
            amdSecType = METS_amdSec_Type_Enum.UNSPECIFIED;

            // instantiate collections
            mappings = new List<METS_Section_ReaderWriter_Mapping>();
            options = new Dictionary<string, string>();
        }

        /// <summary> ID for this METS section reader/writer </summary>
        public string ID { get; set; }

        /// <summary> Label associated with this METS section reader/writer </summary>
        public string Label { get; set; }

        /// <summary> Namespace within which this reader/writer class appears </summary>
        /// <remarks> For all standard reader/writers, this returns 'SobekCM.Resource_Object.METS_Sec_ReaderWriters'.<br /><br />
        /// This is used for instantiating the reader/writer class. </remarks>
        public string Code_Namespace { get; set; }

        /// <summary> Class name for the associated reader/writer </summary>
        /// <remarks> This is used for instantiating the reader/writer class. </remarks>
        public string Code_Class { get; set; }

        /// <summary> Assembly name of the DLL which holds this associated reader/writer </summary>
        /// <remarks> For all standard reader/writers, this is an empty string, since they are in this DLL. <br /><br />
        ///  This is used for instantiating the reader/writer class. </remarks>
        public string Code_Assembly { get; set; }

        /// <summary> Flag indicates if this METS section reader/writer is active </summary>
        public bool isActive { get; set; }

        /// <summary> If this reader/writer targets an amdSec, the type of amdSec </summary>
        public METS_amdSec_Type_Enum amdSecType { get; set; }

        /// <summary> Which METS section this reader/writer targets </summary>
        public METS_Section_Type_Enum METS_Section { get; set; }

        /// <summary> METS section reader/writer object associated with this configuration </summary>
        public object ReaderWriterObject { get; private set;  }

        /// <summary> If there is an error while creating the reader/writer object, the error is stored here </summary>
        public string ReaderWriterObject_Creation_Error { get; private set; }

        /// <summary> Creates the reader/writer object, from the provided code assembly, namespace, and class </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Create_ReaderWriterObject()
        {
            try
            {
                // Using reflection, create an object from the class namespace/name
                //     System.Reflection.Assembly dllAssembly = System.Reflection.Assembly.LoadFrom("SobekCM_Bib_Package_3_0_5.dll");
                Assembly dllAssembly = Assembly.GetExecutingAssembly();
              //  Assembly dllAssembly = Assembly..LoadFrom( Code_Assembly );
                Type readerWriterType = dllAssembly.GetType(Code_Namespace + "." + Code_Class);
                ReaderWriterObject = Activator.CreateInstance(readerWriterType); 
                return true;
            }
            catch (Exception ee)
            {
                ReaderWriterObject_Creation_Error = "Unable to load class from assembly. ( " + Code_Namespace + "." + Code_Class + " ) : " + ee.Message;
                return false;
            }
        }

        /// <summary> Gets the default mapping rom the METS file MDTYPE and OTHERMDTYPE attributes 
        /// which this is utilized when writing a METS file </summary>
        public METS_Section_ReaderWriter_Mapping Default_Mapping
        {
            get
            {
                if (defaultMapping != null)
                    return defaultMapping;
                if (mappings.Count > 0)
                    return mappings[0];
                return null;
            }
        }

        /// <summary> Number of mappings from the METS file MDTYPE and OTHERMDTYPE attributes 
        /// which this METS Section Reader/Writer supports </summary>
        public int Mappings_Count
        {
            get { return mappings.Count; }
        }

        /// <summary> Collection of all mappings from the METS file MDTYPE and OTHERMDTYPE attributes 
        /// which this METS Section Reader/Writer supports </summary>
        public ReadOnlyCollection<METS_Section_ReaderWriter_Mapping> Mappings
        {
            get { return new ReadOnlyCollection<METS_Section_ReaderWriter_Mapping>(mappings); }
        }


        /// <summary> Add a new mapping from the METS file MDTYPE and OTHERMDTYPE attributes 
        /// which this METS Section Reader/Writer supports </summary>
        /// <param name="New_Mapping"> New mapping </param>
        public void Add_Mapping(METS_Section_ReaderWriter_Mapping New_Mapping)
        {
            mappings.Add(New_Mapping);
            if (New_Mapping.isDefault)
                defaultMapping = New_Mapping;
        }

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

    /// <summary> Information about how a METS Section reader/writer maps into a METS file </summary>
    public class METS_Section_ReaderWriter_Mapping
    {
        /// <summary> Constructor for a new instance of the METS_Section_ReaderWriter_Mapping class </summary>
        public METS_Section_ReaderWriter_Mapping()
        {
            MD_Type = String.Empty;
            Other_MD_Type = String.Empty;
            Label = String.Empty;
            isDefault = false;
        }

        /// <summary> Constructor for a new instance of the METS_Section_ReaderWriter_Mapping class </summary>
        /// <param name="MD_Type"> Standard metadata type within the METS dmdSec/amdSec metadata definition tags (or OTHER)</param>
        /// <param name="isDefault"> Flag indicates if this is the default mapping, which means it would be used when writing a new METS file</param>
        public METS_Section_ReaderWriter_Mapping(string MD_Type, bool isDefault)
        {
            this.MD_Type = MD_Type;
            Other_MD_Type = String.Empty;
            Label = String.Empty;
            this.isDefault = isDefault;
        }

        /// <summary> Constructor for a new instance of the METS_Section_ReaderWriter_Mapping class </summary>
        /// <param name="MD_Type"> Standard metadata type within the METS dmdSec/amdSec metadata definition tags (or OTHER)</param>
        /// <param name="Label"> Label associated with this METS section within the METS dmdSec/amdSec metadata definition tags (used when writing)</param>
        /// <param name="isDefault"> Flag indicates if this is the default mapping, which means it would be used when writing a new METS file</param>
        public METS_Section_ReaderWriter_Mapping( string MD_Type, string Label, bool isDefault )
        {
            this.MD_Type = MD_Type;
            Other_MD_Type = String.Empty;
            this.Label = Label;
            this.isDefault = isDefault;
        }

        /// <summary> Constructor for a new instance of the METS_Section_ReaderWriter_Mapping class </summary>
        /// <param name="MD_Type"> Standard metadata type within the METS dmdSec/amdSec metadata definition tags (or OTHER)</param>
        /// <param name="Other_MD_Type"> Other metadata type within the METS dmdSec/amdSec metadata definition tags</param>
        /// <param name="Label"> Label associated with this METS section within the METS dmdSec/amdSec metadata definition tags (used when writing)</param>
        /// <param name="isDefault"> Flag indicates if this is the default mapping, which means it would be used when writing a new METS file</param>
        public METS_Section_ReaderWriter_Mapping(string MD_Type, string Other_MD_Type, string Label, bool isDefault)
        {
            this.MD_Type = MD_Type;
            this.Other_MD_Type = Other_MD_Type;
            this.Label = Label;
            this.isDefault = isDefault;
        }

        /// <summary> Standard metadata type within the METS dmdSec/amdSec metadata definition tags (or OTHER)</summary>
        public string MD_Type { get; set; }

        /// <summary> Other metadata type within the METS dmdSec/amdSec metadata definition tags </summary>
        public string Other_MD_Type { get; set; }

        /// <summary> Label associated with this METS section within the METS dmdSec/amdSec metadata definition tags </summary>
        public string Label { get; set; }

        /// <summary> Flag indicates if this is the default mapping, which means it would be used when writing 
        /// a new METS file </summary>
        public bool isDefault { get; set; }
    }
}