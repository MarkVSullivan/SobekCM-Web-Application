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
    public class InstanceWide_Settings : iSerializationEvents
    {
        /// TODO: Remove the database information from this object, since this will be passed to the web via REST


        private readonly Dictionary<int, Disposition_Option> dispositionLookup;
        private readonly Dictionary<string, Metadata_Search_Field> metadataFieldsByCode;
        private readonly Dictionary<short, Metadata_Search_Field> metadataFieldsByID;
        private readonly Dictionary<string, Metadata_Search_Field> metadataFieldsByFacetName;
        private readonly Dictionary<string, Metadata_Search_Field> metadataFieldsByDisplayName;
        private readonly Dictionary<string, Metadata_Search_Field> metadataFieldsByName;

        



        /// <summary> constructor sets all the values to default empty strings </summary>
        public InstanceWide_Settings()
        {
            try
            {
                // Set some default values
                Database_Connections = new List<Database_Instance_Configuration>();

                Base_SobekCM_Location_Relative = String.Empty;

                

                
                
                Metadata_Help_URL_Base = String.Empty;
                Help_URL_Base = String.Empty;

                

                Kakadu_JP2_Create_Command = String.Empty;
                OCR_Command_Prompt = String.Empty;
                Builder_Override_Seconds_Between_Polls = -1;

                isHosted = false;


                


                



                // Define new empty collections
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
                System = new System_Settings();
            }
            catch (Exception ee)
            {
                // Do nothing here

            }
        }

        /// <summary> Settings from the database for built-in archiving functionality </summary>
        [DataMember(Name = "archive", EmitDefaultValue = false)]
        [XmlElement("archive")]
        [ProtoMember(1)]
        public Archive_Settings Archive { get; private set; }

        /// <summary> Settings from the database for the builder functionality (values and modules) </summary>
        [DataMember(Name = "builder", EmitDefaultValue = false)]
        [XmlElement("builder")]
        [ProtoMember(2)]
        public Builder_Settings Builder { get; private set; }

        /// <summary> Settings for emails, including email setup and main email addresses used in certain situations </summary>
        [DataMember(Name = "email", EmitDefaultValue = false)]
        [XmlElement("email")]
        [ProtoMember(3)]
        public Email_Settings Email { get; private set; }

        /// <summary> Settings for specific to the Florida SUS schools </summary>
        [DataMember(Name = "florida", EmitDefaultValue = false)]
        [XmlElement("florida")]
        [ProtoMember(4)]
        public Florida_Settings Florida { get; private set; }

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
        public Resource_Settings Resources { get; private set; }

        /// <summary> Settings related to the behavior of the aggregation-wide searches within the system </summary>
        [DataMember(Name = "search", EmitDefaultValue = false)]
        [XmlElement("search")]
        [ProtoMember(7)]
        public Search_Settings Search { get; private set; }

        /// <summary> Settings regarding the server architecture, include URLs and network locations </summary>
        [DataMember(Name = "servers", EmitDefaultValue = false)]
        [XmlElement("servers")]
        [ProtoMember(8)]
        public Server_Settings Servers { get; private set; }

        /// <summary> Top-level settings that control basic operation and appearance of the entire SobekCM instance </summary>
        [DataMember(Name = "system", EmitDefaultValue = false)]
        [XmlElement("system")]
        [ProtoMember(9)]
        public System_Settings System { get; private set; }




        /// <summary> Database connection string(s) built from the system config file (usually sits in a config subfolder)</summary>
        [DataMember]
        public List<Database_Instance_Configuration> Database_Connections { get; set; }

        #region Methods related to the workflow types and disposition types

        /// <summary> Gets the list of all possible workflows </summary>
        [DataMember]
        public List<Workflow_Type> Workflow_Types { get; set; }

        /// <summary> Gets the list of all possible dispositions </summary>
        [DataMember]
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
        [DataMember]
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

        #region Public Properties

        /// <summary> Additional custom settings associated with this SobekCM system at
        /// the highest level </summary>
        [DataMember]
        public Dictionary<string, string> Additional_Settings { get; set; }








        /// <summary> Relative location to the folders on the web server </summary>
        /// <remarks> This is only used when building pages in the SobekCM Builder, which allows for
        /// the replacement of all the relative references ( i.e., '/design/skins/dloc/dloc.css') with the full
        /// link ( i.e., 'http://example.edu/design/skins/dloc/dloc.css' ) </remarks>
        [DataMember]
        public string Base_SobekCM_Location_Relative { get; set; }




        /// <summary> Current version number associated with this SobekCM builder application </summary>
        [DataMember]
        public string Current_Builder_Version { get; set; }

        /// <summary> Current version number associated with this SobekCM digital repository web application </summary>
        [DataMember]
        public string Current_Web_Version { get; set; }








        /// <summary> Number of ticks that a complete package must age before being processed </summary>
        /// <value> This is currently set to 15 minutes (in ticks) </value>
        [DataMember]
        public long Complete_Package_Required_Aging { get; set; }




        /// <summary> Ghostscript executable file </summary>
        [DataMember]
        public string Ghostscript_Executable { get; set; }

        /// <summary> ImageMagick executable file </summary>
        [DataMember]
        public string ImageMagick_Executable { get; set; }









        /// <summary> Kakadu JPEG2000 script will override the specifications used when creating zoomable images </summary>
        [DataMember]
        public string Kakadu_JP2_Create_Command { get; set; }

        /// <summary> Directory where the local logs are written </summary>
        /// <remarks> This is determined the first time this class is referenced and is just
        /// a <b>Logs</b> subfolder uynder the application startup path</remarks>
        [DataMember]
        public string Local_Log_Directory { get; set; }

        /// <summary> Final destination of the processing log (usually the web server) </summary>
        [DataMember]
        public string Log_Files_Directory { get; set; }

        /// <summary> Final URL for the processing log, for any links between different failure logs </summary>
        [DataMember]
        public string Log_Files_URL { get; set; }

        /// <summary> Returns the network location for the main builder, which runs essentially
        /// without restrictions </summary>
        [DataMember]
        public string Main_Builder_Input_Folder { get; set; }

        /// <summary> Number of ticks that a metadata only package must age before being processed </summary>
        /// <value> This is currently set to 1 minute (in ticks) </value>
        [DataMember]
        public long METS_Only_Package_Required_Aging { get; set; }

        /// <summary> Command to launch the OCR engine against a single TIFF to produce a single TEXT file </summary>
        [DataMember]
        public string OCR_Command_Prompt { get; set; }



        /// <summary> Folder where files bound for archiving are placed </summary>
        [DataMember]
        public string Package_Archival_Folder { get; set; }

        /// <summary> List of possible page image extensions </summary>
        [DataMember]
        public List<string> Page_Image_Extensions { get; set; }

        /// <summary> Gets the list of reserved keywords that cannot be used
        /// for aggregation codes or aggregation aliases </summary>
        /// <remarks> These are all lower case </remarks>
        [DataMember]
        public List<string> Reserved_Keywords { get; set; }









        #endregion



        /// <summary> Number of seconds between polls, from the configuration file (not the database) </summary>
        /// <remarks> This is used if the SobekCM Builder is working between multiple instances. If the SobekCM
        /// Builder is only servicing a single instance, then the data can be pulled from the database. </remarks>
        [DataMember]
        public int Builder_Override_Seconds_Between_Polls { get; set; }

        /// <summary> Flag indicates if this is a 'hosted' solution of SobekCM, in which case
        /// certain fields should not be made available, even to "system admins" </summary>
        [DataMember]
        public bool isHosted { get; set; }


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


        #region Properties and derivative methods about the help page URLs

        /// <summary> Base URL for the metadata help </summary>
        [DataMember]
        public string Metadata_Help_URL_Base { get; set; }

        /// <summary> Base URL for most the help pages </summary>
        [DataMember]
        public string Help_URL_Base { get; set; }

        /// <summary> Get the URL for all metadata help pages which are used when users request 
        /// help while submitting a new item or editing an existing item </summary>
        /// <param name="Current_Base_URL"> Current base url for the current user's request </param>
        /// <returns> Base URL to use for the metadata help page links </returns>
        public string Metadata_Help_URL(string Current_Base_URL)
        {
            return Metadata_Help_URL_Base.Length > 0 ? Help_URL_Base : Current_Base_URL;
        }

        /// <summary> URL used for the main help pages about this system's basic functionality </summary>
        /// <param name="Current_Base_URL"> Current base url for the current user's request </param>
        /// <returns> Base URL to use for the main help page links </returns>
        public string Help_URL(string Current_Base_URL)
        {
            return Help_URL_Base.Length > 0 ? Help_URL_Base : Current_Base_URL;
        }

        #endregion

        #region Links to larger configuration and settings objects

        /// <summary> Configuration for authentication for this instance </summary>
        public Authentication_Configuration Authentication { get; set; }

        /// <summary> Configuration for the default contact form for this instance </summary>
        public ContactForm_Configuration ContactForm { get; set; }

        /// <summary> Configuration information for the map editor function for this instance </summary>
        public MapEditor_Configuration MapEditor { get; set; }



        /// <summary> Configuration for instance-wide OAI-PMH settings for this instance </summary>
        public OAI_PMH_Configuration OAI_PMH { get; set; }

        /// <summary> Configuration for the quality control tool for this instance </summary>
        public QualityControl_Configuration QualityControlTool { get; set; }

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
