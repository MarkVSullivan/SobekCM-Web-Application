#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Extensions;
using SobekCM.Core.MicroservicesClient;
using SobekCM.Core.Search;
using SobekCM.Core.Settings.DbItemViewers;
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



    /// <summary> Class provides context to constant settings from the database,
    /// based on the basic information about this instance of the application and server information </summary>
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
        private Dictionary<string, string> additionalSettingsDictionary;

        /// <summary> constructor sets all the values to default empty strings </summary>
        public InstanceWide_Settings()
        {
            // Define new empty collections
            dispositionLookup = new Dictionary<int, Disposition_Option>();
            Metadata_Search_Fields = new List<Metadata_Search_Field>();
            metadataFieldsByCode = new Dictionary<string, Metadata_Search_Field>();
            metadataFieldsByID = new Dictionary<short, Metadata_Search_Field>();
            metadataFieldsByFacetName = new Dictionary<string, Metadata_Search_Field>();
            metadataFieldsByDisplayName = new Dictionary<string, Metadata_Search_Field>();
            metadataFieldsByName = new Dictionary<string, Metadata_Search_Field>();
            additionalSettingsDictionary = new Dictionary<string, string>();
            Additional_Settings = new List<Simple_Setting>();
            Workflow_Types = new List<Workflow_Type>();
            Disposition_Options = new List<Disposition_Option>();
            DbItemViewers = new DbItemViewerTypes();

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

        /// <summary> Database connection string built from the system config file (usually sits in a config subfolder)</summary>
        [DataMember(Name = "dbConnection", EmitDefaultValue = false)]
        [XmlElement("dbConnection")]
        [ProtoMember(11)]
        public Database_Instance_Configuration Database_Connection { get; set; }

        /// <summary> All the item viewer information from the database,
        /// which includes which viewers are added by default, default orders, and the primary key 
        /// to which each individual digital resource is attached </summary>
        [DataMember(Name = "dbItemViewers", EmitDefaultValue = false)]
        [XmlElement("dbItemViewers")]
        [ProtoMember(12)]
        public DbItemViewerTypes DbItemViewers { get; set; }

        #region Methods related to the workflow types and disposition types

        /// <summary> Gets the list of all possible workflows </summary>
        [DataMember(Name = "workflowTypes", EmitDefaultValue = false)]
        [XmlArray("workflowTypes")]
        [XmlArrayItem("workflowType", typeof(Workflow_Type))]
        [ProtoMember(13)]
        public List<Workflow_Type> Workflow_Types { get; set; }

        /// <summary> Gets the list of all possible dispositions </summary>
        [DataMember(Name = "dispositionOptions", EmitDefaultValue = false)]
        [XmlArray("dispositionOptions")]
        [XmlArrayItem("dispositionOption", typeof(Disposition_Option))]
        [ProtoMember(14)]
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

        private Dictionary<int, KeyValuePair<int, string>> dispositionFutureTypes;
        private Dictionary<int, KeyValuePair<int, string>> dispositionPastTypes;

        /// <summary> Gets the list of all possible disposition types in future tense </summary>
        /// <remarks> This is used by the SMaRT tool and may be deprecated in the future </remarks>
        public List<string> Disposition_Types_Future
        {
            get
            {
                if (dispositionFutureTypes == null)
                {
                    dispositionFutureTypes = new Dictionary<int, KeyValuePair<int, string>>();
                    foreach (Disposition_Option thisOption in Disposition_Options)
                    {
                        dispositionFutureTypes[thisOption.Key] = new KeyValuePair<int, string>(thisOption.Key, thisOption.Future);
                    }
                }

                return dispositionFutureTypes.Values.Select(thisValue => thisValue.Value).ToList();
            }
        }

        /// <summary> Gets the list of all possible disposition types in past tense </summary>
        /// <remarks> This is used by the SMaRT tool and may be deprecated in the future </remarks>
        public List<string> Disposition_Types_Past
        {
            get
            {
                if (dispositionPastTypes == null)
                {
                    dispositionPastTypes = new Dictionary<int, KeyValuePair<int, string>>();
                    foreach (Disposition_Option thisOption in Disposition_Options)
                    {
                        dispositionPastTypes[thisOption.Key] = new KeyValuePair<int, string>(thisOption.Key, thisOption.Past);
                    }
                }

                return dispositionPastTypes.Values.Select(thisValue => thisValue.Value).ToList();
            }
        }

        #endregion

        #region Methods related to the metadata types (search fields)

        /// <summary> Gets the list of all metadata search fields </summary>
        [DataMember(Name = "metdataSearchFields", EmitDefaultValue = false)]
        [XmlArray("metdataSearchFields")]
        [XmlArrayItem("metdataSearchField", typeof(Metadata_Search_Field))]
        [ProtoMember(15)]
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


        #region Properties and methods related to additional custom settings 
       

        /// <summary> Additional custom settings associated with this SobekCM system at
        /// the highest level </summary>
        [DataMember(Name = "additionalSettings", EmitDefaultValue = false)]
        [XmlArray("additionalSettings")]
        [XmlArrayItem("setting", typeof(Simple_Setting))]
        [ProtoMember(21)]
        public List<Simple_Setting> Additional_Settings { get; set; }

        /// <summary> Add a remaining, unassigned, additional setting </summary>
        /// <param name="Key"> Name / key for this database setting </param>
        /// <param name="Value"> Current value for this setting  </param>
        public void Add_Additional_Setting(string Key, string Value)
        {
            Additional_Settings.Add(new Simple_Setting(Key, Value, -1));
            additionalSettingsDictionary[Key] = Value;
        }

        /// <summary> Get a flag indicating if a value is set for this key </summary>
        /// <param name="Key"> Name / key for this database setting </param>
        /// <returns> TRUE, if the setting exists, otherwise FALSE </returns>
        public bool Contains_Additional_Setting(string Key)
        {
            // Ensure the dictionary is built
            if ((additionalSettingsDictionary == null) || (additionalSettingsDictionary.Count != Additional_Settings.Count))
            {
                additionalSettingsDictionary = new Dictionary<string, string>();
                foreach (Simple_Setting thisSetting in Additional_Settings)
                {
                    additionalSettingsDictionary[thisSetting.Key] = thisSetting.Value;
                }
            }

            return ((additionalSettingsDictionary.ContainsKey(Key)) && (!String.IsNullOrEmpty(additionalSettingsDictionary[Key])));
        }

        /// <summary> Gets an additional setting value, by the key </summary>
        /// <param name="Key"> Name / key for this database setting </param>
        /// <returns> Value, or null </returns>
        public string Get_Additional_Setting(string Key)
        {
            // Ensure the dictionary is built
            if ((additionalSettingsDictionary == null) || (additionalSettingsDictionary.Count != Additional_Settings.Count))
            {
                additionalSettingsDictionary = new Dictionary<string, string>();
                foreach (Simple_Setting thisSetting in Additional_Settings)
                {
                    additionalSettingsDictionary[thisSetting.Key] = thisSetting.Value;
                }
            }

            if (additionalSettingsDictionary.ContainsKey(Key))
            {
                return additionalSettingsDictionary[Key];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Properties related to the list of extensions/plugins from the database

        /// <summary> List of basic extension/plugin information from the database </summary>
        [DataMember(Name = "additionalSettings", EmitDefaultValue = false)]
        [XmlArray("additionalSettings")]
        [XmlArrayItem("setting", typeof(Simple_Setting))]
        [ProtoMember(22)]
        public List<ExtensionInfo> DbExtensions { get; set; }

        /// <summary> Checks to see if the extension exists and is enabled, according to the database </summary>
        /// <param name="Code"> Code of the extensions, which must match the directory for the plug-in </param>
        /// <returns> TRUE if the extension exists and is enabled, otherwise FALSE </returns>
        public bool ExtensionEnabled(string Code)
        {
            if (DbExtensions == null) return false;

            foreach (ExtensionInfo thisExtension in DbExtensions)
            {
                if (String.Compare(Code, thisExtension.Code, StringComparison.OrdinalIgnoreCase) == 0)
                    return thisExtension.Enabled;
            }

            return false;
        }

        /// <summary> Gets the information from the database for a single extension </summary>
        /// <param name="Code"> Code of the extensions, which must match the directory for the plug-in </param>
        /// <returns> Extension information from the database, or NULL </returns>
        public ExtensionInfo ExtensionByCode(string Code)
        {
            if (DbExtensions == null) return null;

            foreach (ExtensionInfo thisExtension in DbExtensions)
            {
                if (String.Compare(Code, thisExtension.Code, StringComparison.OrdinalIgnoreCase) == 0)
                    return thisExtension;
            }

            return null;
        }

        #endregion


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

            // Fill up the additional settings dictionary

        }
    }
}
