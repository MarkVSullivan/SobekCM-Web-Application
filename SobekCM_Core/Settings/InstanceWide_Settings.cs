#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.Configuration;
using SobekCM.Core.Search;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Core.Settings
{
    /// <summary> Enumeration indicates the type of email sending to support </summary>
    public enum Email_Method_Enum : byte
    {
        /// <summary> Send via database mail </summary>
        MsSqlDatabaseMail = 0,

        /// <summary> Send directly to the SMTP server </summary>
        SmtpDirect
    }



    /// <summary> Class provides context to constant settings based on the basic information about this instance of the application and server information </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("Settings")]
    public class InstanceWide_Settings : iSerializationEvents
    {
        private readonly Dictionary<int, Disposition_Option> dispositionLookup;
        private readonly Dictionary<string, Metadata_Search_Field> metadataFieldsByCode;
        private readonly Dictionary<short, Metadata_Search_Field> metadataFieldsByID;
        private readonly Dictionary<string, Metadata_Search_Field> metadataFieldsByFacetName;
        private readonly Dictionary<string, Metadata_Search_Field> metadataFieldsByDisplayName;
        private readonly Dictionary<string, Metadata_Search_Field> metadataFieldsByName;

        /// <summary> constructor sets all the values to default empty strings </summary>
        public InstanceWide_Settings()
        {
            // Define new empty collections
            Database_Connections = new List<Database_Instance_Configuration>();
            dispositionLookup = new Dictionary<int, Disposition_Option>();
            Metadata_Search_Fields = new List<Metadata_Search_Field>();
            metadataFieldsByCode = new Dictionary<string, Metadata_Search_Field>();
            metadataFieldsByID = new Dictionary<short, Metadata_Search_Field>();
            metadataFieldsByFacetName = new Dictionary<string, Metadata_Search_Field>();
            metadataFieldsByDisplayName = new Dictionary<string, Metadata_Search_Field>();
            metadataFieldsByName = new Dictionary<string, Metadata_Search_Field>();
            Additional_Settings = new Dictionary<string, string>();
            Workflow_Types = new List<Workflow_Type>();
            Disposition_Options = new List<Disposition_Option>();

            // Create the child setting objects
            Archive = new Archive_Settings();
            Builder = new Builder_Settings();
            Email = new Email_Settings();
            Florida = new Florida_Settings();
            MarcGeneration = new Marc21_Settings();
            Resources = new Resource_Settings();
            Search = new Search_Settings();
            Servers = new Server_Settings();
            Static = new Static_Settings();
            System = new System_Settings();

            // Create some of the configuration stuff
            Authentication = new Authentication_Configuration();
        }

        /// <summary> Settings from the database for built-in archiving functionality </summary>
        [DataMember(Name = "archive", EmitDefaultValue = false)]
        [XmlElement("archive")]
        [ProtoMember(1)]
        public Archive_Settings Archive { get; set; }

        /// <summary> Settings from the database for the builder functionality (values and modules) </summary>
        [DataMember(Name = "builder", EmitDefaultValue = false)]
        [XmlElement("builder")]
        [ProtoMember(2)]
        public Builder_Settings Builder { get; set; }

        /// <summary> Settings for emails, including email setup and main email addresses used in certain situations </summary>
        [DataMember(Name = "email", EmitDefaultValue = false)]
        [XmlElement("email")]
        [ProtoMember(3)]
        public Email_Settings Email { get; set; }

        /// <summary> Settings for specific to the Florida SUS schools </summary>
        [DataMember(Name = "florida", EmitDefaultValue = false)]
        [XmlElement("florida")]
        [ProtoMember(4)]
        public Florida_Settings Florida { get; set; }

        /// <summary> Setting information about the generation of Marc files, some constants, and information about the MARC feed </summary>
        [DataMember(Name = "marc21", EmitDefaultValue = false)]
        [XmlElement("marc21")]
        [ProtoMember(5)]
        public Marc21_Settings MarcGeneration { get; set; }

        /// <summary> Settings related to default values for the digital resource files 
        /// and how resource files should be handeled </summary>
        [DataMember(Name = "resources", EmitDefaultValue = false)]
        [XmlElement("resources")]
        [ProtoMember(6)]
        public Resource_Settings Resources { get; set; }

        /// <summary> Settings related to the behavior of the aggregation-wide searches within the system </summary>
        [DataMember(Name = "search", EmitDefaultValue = false)]
        [XmlElement("search")]
        [ProtoMember(7)]
        public Search_Settings Search { get; set; }

        /// <summary> Settings regarding the server architecture, include URLs and network locations </summary>
        [DataMember(Name = "servers", EmitDefaultValue = false)]
        [XmlElement("servers")]
        [ProtoMember(8)]
        public Server_Settings Servers { get; set; }

        /// <summary> Top-level settings that are fairly consistent, and don't really load from 
        /// any database or configuration value </summary>
        [DataMember(Name = "static", EmitDefaultValue = false)]
        [XmlElement("static")]
        [ProtoMember(9)]
        public Static_Settings Static { get; set; }

        /// <summary> Top-level settings that control basic operation and appearance of the entire SobekCM instance </summary>
        [DataMember(Name = "system", EmitDefaultValue = false)]
        [XmlElement("system")]
        [ProtoMember(10)]
        public System_Settings System { get; set; }

        /// <summary> Database connection string(s) built from the system config file (usually sits in a config subfolder)</summary>
        [DataMember(Name = "dbConnections", EmitDefaultValue = false)]
        [XmlArray("dbConnections")]
        [XmlArrayItem("dbConnection", typeof(Database_Instance_Configuration))]
        [ProtoMember(11)]
        public List<Database_Instance_Configuration> Database_Connections { get; set; }

        #region Methods related to the workflow types and disposition types

        /// <summary> Gets the list of all possible workflows </summary>
        [DataMember(Name = "workflowTypes", EmitDefaultValue = false)]
        [XmlArray("workflowTypes")]
        [XmlArrayItem("workflowType", typeof(Workflow_Type))]
        [ProtoMember(12)]
        public List<Workflow_Type> Workflow_Types { get; set; }

        /// <summary> Gets the list of all possible dispositions </summary>
        [DataMember(Name = "dispositionOptions", EmitDefaultValue = false)]
        [XmlArray("dispositionOptions")]
        [XmlArrayItem("dispositionOption", typeof(Disposition_Option))]
        [ProtoMember(13)]
        public List<Disposition_Option> Disposition_Options { get; set; }

        /// <summary> Gets the disposition term, in past tense, by primary key </summary>
        /// <param name="DispositionID"> Primary key for the disposition term </param>
        /// <returns> Term in past tense, or "UNKNOWN" </returns>
        public string Disposition_Term_Past(int DispositionID)
        {
            return dispositionLookup.ContainsKey(DispositionID) ? dispositionLookup[DispositionID].Past : "UNKNOWN";
        }

        /// <summary> Gets the disposition term, in future tense, by primary key </summary>
        /// <param name="DispositionID"> Primary key for the disposition term </param>
        /// <returns> Term in future tense, or "UNKNOWN" </returns>
        public string Disposition_Term_Future(int DispositionID)
        {
            return dispositionLookup.ContainsKey(DispositionID) ? dispositionLookup[DispositionID].Future : "UNKNOWN";
        }

        /// <summary> Gets the primary key for a disposition type, by the past tense term </summary>
        /// <param name="Disposition_Term"> Disposition term, in past tense </param>
        /// <returns> Primary key for the matching disposition, or -1 </returns>
        public int Disposition_ID_Past(string Disposition_Term)
        {
            foreach (Disposition_Option thisOption in Disposition_Options)
            {
                if (String.Compare(thisOption.Past, Disposition_Term, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return thisOption.Key;
                }
            }
            return -1;
        }

        /// <summary> Gets the primary key for a disposition type, by the future tense term </summary>
        /// <param name="Disposition_Term"> Disposition term, in future tense </param>
        /// <returns> Primary key for the matching disposition, or -1 </returns>
        public int Disposition_ID_Future(string Disposition_Term)
        {
            foreach (Disposition_Option thisOption in Disposition_Options)
            {
                if (String.Compare(thisOption.Future, Disposition_Term, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return thisOption.Key;
                }
            }
            return -1;
        }

        #endregion

        #region Methods related to the metadata types (search fields)

        /// <summary> Gets the list of all metadata search fields </summary>
        [DataMember(Name = "metdataSearchFields", EmitDefaultValue = false)]
        [XmlArray("metdataSearchFields")]
        [XmlArrayItem("metdataSearchField", typeof(Metadata_Search_Field))]
        [ProtoMember(14)]
        public List<Metadata_Search_Field> Metadata_Search_Fields { get; set; }

        /// <summary> Gets a single metadata search field, by SobekCM web code </summary>
        /// <param name="SobekCM_Code"> Code for this metadata search field, employed within the URLs </param>
        /// <returns> Metadata search field, or else NULL </returns>
        public Metadata_Search_Field Metadata_Search_Field_By_Code(string SobekCM_Code)
        {
             return metadataFieldsByCode.ContainsKey(SobekCM_Code) ? metadataFieldsByCode[SobekCM_Code] : null;
        }

        /// <summary> Gets a single metadata search field, by the metadata column name </summary>
        /// <param name="Metadata_Name"> Name for this field and name of the column in the database </param>
        /// <returns> Metadata search field, or else NULL </returns>
        public Metadata_Search_Field Metadata_Search_Field_By_Name(string Metadata_Name)
        {
            return metadataFieldsByName.ContainsKey(Metadata_Name.Replace("_", " ").ToLower()) ? metadataFieldsByName[Metadata_Name.Replace("_", " ").ToLower()] : null;
        }

        /// <summary> Gets a single metadata search field, by the display name </summary>
        /// <param name="Display_Name"> Name for this field and name of the column in the database </param>
        /// <returns> Metadata search field, or else NULL </returns>
        public Metadata_Search_Field Metadata_Search_Field_By_Display_Name(string Display_Name)
        {
            return metadataFieldsByDisplayName.ContainsKey(Display_Name.Replace("_", " ").ToLower()) ? metadataFieldsByDisplayName[Display_Name.Replace("_", " ").ToLower()] : null;
        }

        /// <summary> Gets a single metadata search field, by the facet name </summary>
        /// <param name="Facet_Name"> Name for this field for faceting purposes </param>
        /// <returns> Metadata search field, or else NULL </returns>
        public Metadata_Search_Field Metadata_Search_Field_By_Facet_Name(string Facet_Name)
        {
            return metadataFieldsByFacetName.ContainsKey(Facet_Name.Replace("_", " ").ToLower()) ? metadataFieldsByFacetName[Facet_Name.Replace("_", " ").ToLower()] : null;
        }
        
        /// <summary> Gets a single metadata search field, by primary identifiere </summary>
        /// <param name="MetadataTypeID"> Primary identifier for the metadata search field </param>
        /// <returns> Metadata search field, or else NULL </returns>
        public Metadata_Search_Field Metadata_Search_Field_By_ID(short MetadataTypeID)
        {
            return metadataFieldsByID.ContainsKey(MetadataTypeID) ? metadataFieldsByID[MetadataTypeID] : null;
        }

        #endregion

        /// <summary> Gets the user-in-process directory </summary>
        /// <param name="CurrentUser"> Current user, for which to find the in-process directory </param>
        /// <param name="DirectoryName"> Subdirectory requested </param>
        /// <returns> Full path to the requested user-in-process directory </returns>
        public string User_InProcess_Directory(User_Object CurrentUser, string DirectoryName)
        {
            // Determine the in process directory for this
            string userInProcessDirectory = Servers.In_Process_Submission_Location + "\\" + CurrentUser.UserName.Replace(".", "").Replace("@", "") + "\\" + DirectoryName;
            if (CurrentUser.ShibbID.Trim().Length > 0)
                userInProcessDirectory = Servers.In_Process_Submission_Location + "\\" + CurrentUser.ShibbID + "\\" + DirectoryName;

            return userInProcessDirectory;
        }


        /// <summary> Additional custom settings associated with this SobekCM system at
        /// the highest level </summary>
        [XmlIgnore]
        public Dictionary<string, string> Additional_Settings { get; set; }


        /// <summary> Configuration for authentication for this instance </summary>
        [DataMember(Name = "authentication", EmitDefaultValue = false)]
        [XmlElement("authentication")]
        [ProtoMember(15)]
        public Authentication_Configuration Authentication { get; set; }

        /// <summary> Configuration for the default contact form for this instance </summary>
        [DataMember(Name = "contactForm", EmitDefaultValue = false)]
        [XmlElement("contactForm")]
        [ProtoMember(16)]
        public ContactForm_Configuration ContactForm { get; set; }

        /// <summary> Configuration information for the map editor function for this instance </summary>
        //[DataMember(Name = "mapEditor", EmitDefaultValue = false)]
        //[XmlElement("mapEditor")]
        //[ProtoMember(17)]
        [XmlIgnore]
        public MapEditor_Configuration MapEditor { get; set; }

        /// <summary> Configuration for instance-wide OAI-PMH settings for this instance </summary>
        [DataMember(Name = "oai-pmh", EmitDefaultValue = false)]
        [XmlElement("oai-pmh")]
        [ProtoMember(18)]
        public OAI_PMH_Configuration OAI_PMH { get; set; }

        /// <summary> Configuration for the quality control tool for this instance </summary>
        //[DataMember(Name = "qcConfig", EmitDefaultValue = false)]
        //[XmlElement("qcConfig")]
        //[ProtoMember(20)]
        [XmlIgnore]
        public QualityControl_Configuration QualityControlTool { get; set; }

        #region Methods that controls XML serialization

        /// <summary> Method suppresses XML Serialization of the Florida property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeFlorida()
        {
            return (Florida != null) && ((!String.IsNullOrEmpty(Florida.FDA_Report_DropBox)) || (!String.IsNullOrEmpty(Florida.Mango_Union_Search_Base_URL)) || (!String.IsNullOrEmpty(Florida.Mango_Union_Search_Text)));
        }


        /// <summary> Method suppresses XML Serialization of the Archive property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeArchive()
        {
            return (Archive != null) && ((!String.IsNullOrEmpty(Archive.Archive_DropBox)) || (!String.IsNullOrEmpty(Archive.PostArchive_Files_To_Delete)) || (!String.IsNullOrEmpty(Archive.PreArchive_Files_To_Delete)));
        }

        /// <summary> Method suppresses XML Serialization of the MarcGeneration property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeMarcGeneration()
        {
            if (MarcGeneration == null)
                return false;
            
            return ((!String.IsNullOrEmpty(MarcGeneration.Cataloging_Source_Code)) || (!String.IsNullOrEmpty(MarcGeneration.Reproduction_Place)) ||
                    (!String.IsNullOrEmpty(MarcGeneration.Reproduction_Agency)) || (!String.IsNullOrEmpty(MarcGeneration.Location_Code)) ||
                    (!String.IsNullOrEmpty(MarcGeneration.XSLT_File)) || (!String.IsNullOrEmpty(MarcGeneration.MarcXML_Feed_Location)));
        }

        #endregion

        /// <summary> Method is called by the serializer after this item is unserialized </summary>
        public void PostUnSerialization()
        {
            // Populate the dictionaries for looking up metadata search fields by code, id, name, etc..
            metadataFieldsByCode.Clear();
            metadataFieldsByID.Clear();
            metadataFieldsByName.Clear();
            metadataFieldsByDisplayName.Clear();
            metadataFieldsByFacetName.Clear();

            // Now add the rest of the fields
            foreach (Metadata_Search_Field thisField in Metadata_Search_Fields)
            {
                // Add this to the collections
                if (thisField.ID >= -1)
                    metadataFieldsByID[thisField.ID] = thisField;

                if (thisField.Web_Code.Length > 0)
                {
                    metadataFieldsByCode[thisField.Web_Code] = thisField;
                    metadataFieldsByName[thisField.Name.ToLower().Replace("_", " ")] = thisField;
                    metadataFieldsByDisplayName[thisField.Display_Term.ToLower().Replace("_", " ")] = thisField;

                    if (thisField.Facet_Term.Length > 0)
                    {
                        metadataFieldsByFacetName[thisField.Facet_Term.Replace("_", " ").ToLower()] = thisField;
                    }
                }
            }

            // Fill up the disposition lookup object
            dispositionLookup.Clear();
            foreach (Disposition_Option dispOption in Disposition_Options)
            {
                dispositionLookup[dispOption.Key] = dispOption;
            }
        }
    }
}
