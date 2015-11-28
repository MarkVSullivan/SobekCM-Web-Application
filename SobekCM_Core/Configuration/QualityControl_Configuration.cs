#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Configuration
{
    /// <summary> Class keeps all the system-wide quality control profiles which 
    /// can be used within the system  </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("QualityControlConfig")]
    public class QualityControl_Configuration
    {
        private static bool attemptedRead;
        private static string sobekcm_qc_configfilePath;
        private static Dictionary<string, QualityControl_Profile> profilesDictionary;
        
        /// <summary> Static constructor for the QualityControl_Configuration class </summary>
        public QualityControl_Configuration()
        {
            // Declare all the new collections in this configuration 
            profilesDictionary = new Dictionary<string, QualityControl_Profile>();
            Profiles = new List<QualityControl_Profile>();

            // Set some default values
            attemptedRead = false;
            DefaultProfile = null;

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            sobekcm_qc_configfilePath=(baseDirectory + "config\\default\\sobekcm_qc.config");

            if (!Read_Metadata_Configuration(sobekcm_qc_configfilePath))
            {
                // Set default reader/writer values to have a baseline in case there is
                // no file to be read 
                Set_Default_Values();
            }
        }


        /// <summary> Name of the default quality control profile to use, if no specific profile is indicated </summary>
        [DataMember(Name = "default", EmitDefaultValue = false)]
        [XmlAttribute("default")]
        [ProtoMember(1)]
        public string DefaultProfile { get; set; }


        /// <summary> List of all the quality control profiles </summary>
        [DataMember(Name = "profiles")]
        [XmlArray("profiles")]
        [XmlArrayItem("profile", typeof(QualityControl_Profile))]
        [ProtoMember(2)]
        public List<QualityControl_Profile> Profiles { get; set; }


        private void Clear()
        {
            Profiles.Clear();
            profilesDictionary.Clear();
            DefaultProfile = null;
        }

        /// <summary> Flag indicates if the method to read the configuration file has been called </summary>
        /// <remarks> Even if the read is unsuccesful for any reason, this returns TRUE to prevent 
        /// the read method from being called over and over </remarks>
        [XmlIgnore]
        [IgnoreDataMember]
        public bool Attempted_To_Read_Config_File
        {
            get { return attemptedRead; }
        }


        /// <summary> Add a quality control profile with user settings, such as which 
        /// division types to include for selection </summary>
        /// <param name="New_Profile"> New profile to add </param>
        public void Add_Profile(QualityControl_Profile New_Profile)
        {
            // Add to the dictionary of profiles
            Profiles.Add(New_Profile);
            profilesDictionary[New_Profile.Profile_Name] = New_Profile;

            // Was this the default profile?
            if (New_Profile.Default_Profile)
                DefaultProfile = New_Profile.Profile_Name;
        }

        /// <summary> Gets the default quality control profile </summary>
        /// <returns> Quality Control profile </returns>
        public QualityControl_Profile Get_Default_Profile()
        {
            if ((!String.IsNullOrEmpty(DefaultProfile)) && (profilesDictionary.ContainsKey(DefaultProfile)))
                return profilesDictionary[DefaultProfile];
            if (profilesDictionary.Count > 0)
                return profilesDictionary[profilesDictionary.Keys.FirstOrDefault()];
            return null;
        }

        #region Set default values (used if no config file is present)

        /// <summary> Set the default profile with the default values </summary>
        public void Set_Default_Values()
        {
            // Clear everything, just in case
            Clear();

            // Create the default profile
            QualityControl_Profile newProfile = new QualityControl_Profile
            {
                Default_Profile = true, 
                Profile_Name = "System Default", 
                Profile_Description = "Default profile used when no config file is present"
            };
            Add_Profile(newProfile);

            // Add back cover
            QualityControl_Division_Config div1 = new QualityControl_Division_Config
            {
                ID = 1, TypeName = "Back Cover", isActive = true, isNameable = false, BaseTypeName = "Cover"
            };
            div1.Add_Translation(Web_Language_Enum.Spanish, "Portada Posterior");
            div1.Add_Translation(Web_Language_Enum.French, "Couverture Arrière");
            newProfile.Add_Division_Type(div1);

            // Add back matter
            QualityControl_Division_Config div2 = new QualityControl_Division_Config
            {
                ID = 2, TypeName = "Back Matter", isActive = true, isNameable = false
            };
            div2.Add_Translation(Web_Language_Enum.Spanish, "Materia Posterior");
            div2.Add_Translation(Web_Language_Enum.French, "Matière Arrière");
            newProfile.Add_Division_Type(div2);

            // Add chapter ( misorder of the object names here and below matters not)
            QualityControl_Division_Config div4 = new QualityControl_Division_Config
            {
                ID = 3, TypeName = "Chapter", isActive = true, isNameable = true
            };
            div4.Add_Translation(Web_Language_Enum.Spanish, "Capítulo");
            div4.Add_Translation(Web_Language_Enum.French, "Chapitre");
            newProfile.Add_Division_Type(div4);

            // Add front cover
            QualityControl_Division_Config div3 = new QualityControl_Division_Config
            {
                ID = 4, TypeName = "Front Cover", isActive = true, isNameable = false, BaseTypeName = "Cover"
            };
            div3.Add_Translation(Web_Language_Enum.Spanish, "Portada Delantera");
            div3.Add_Translation(Web_Language_Enum.French, "Couverture Frente");
            newProfile.Add_Division_Type(div3);

            // Add front matter
            QualityControl_Division_Config div5 = new QualityControl_Division_Config
            {
                ID = 5, TypeName = "Front Matter", isActive = true, isNameable = false
            };
            div5.Add_Translation(Web_Language_Enum.Spanish, "Materia Delantera");
            div5.Add_Translation(Web_Language_Enum.French, "Préliminaires");
            newProfile.Add_Division_Type(div5);

            // Add index
            QualityControl_Division_Config div6 = new QualityControl_Division_Config
            {
                ID = 6, TypeName = "Index", isActive = true, isNameable = true
            };
            div6.Add_Translation(Web_Language_Enum.Spanish, "Indice");
            div6.Add_Translation(Web_Language_Enum.French, "Indice");
            newProfile.Add_Division_Type(div6);

            // Add introduction
            QualityControl_Division_Config div7 = new QualityControl_Division_Config
            {
                ID = 7, TypeName = "Introduction", isActive = true, isNameable = false, BaseTypeName = "Chapter"
            };
            div7.Add_Translation(Web_Language_Enum.Spanish, "Introducción");
            div7.Add_Translation(Web_Language_Enum.French, "Introduction");
            newProfile.Add_Division_Type(div7);

            // Add spine
            QualityControl_Division_Config div8 = new QualityControl_Division_Config
            {
                ID = 8, TypeName = "Spine", isActive = true, isNameable = false
            };
            div8.Add_Translation(Web_Language_Enum.Spanish, "Canto");
            div8.Add_Translation(Web_Language_Enum.French, "Épine de livre");
            newProfile.Add_Division_Type(div8);

            // Add table of contents
            QualityControl_Division_Config div9 = new QualityControl_Division_Config
            {
                ID = 9, TypeName = "Table of Contents", isActive = true, isNameable = false, BaseTypeName = "Contents"
            };
            div9.Add_Translation(Web_Language_Enum.Spanish, "Contenidos");
            div9.Add_Translation(Web_Language_Enum.French, "Table des Matières");
            newProfile.Add_Division_Type(div9);

            // Add title page
            QualityControl_Division_Config div10 = new QualityControl_Division_Config
            {
                ID = 10, TypeName = "Title Page", isActive = true, isNameable = false, BaseTypeName = "Title"
            };
            div10.Add_Translation(Web_Language_Enum.Spanish, "Titre");
            div10.Add_Translation(Web_Language_Enum.French, "Titulario");
            newProfile.Add_Division_Type(div10);

        }

        #endregion

        #region Code to save this qc configuration to a XML file

        /// <summary> Save this quality control configuration to a XML config file </summary>
        /// <param name="FilePath"> File/path for the resulting XML config file </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_To_Config_File(string FilePath)
        {
            bool returnValue = true;
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(FilePath, false, Encoding.UTF8);
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                writer.WriteLine("<SobekCM_Config xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");
                writer.WriteLine("\txmlns=\"http://digital.uflib.ufl.edu/metadata/sobekcm_config\" ");
                writer.WriteLine("\txsi:schemaLocation=\"http://digital.uflib.ufl.edu/metadata/sobekcm_config ");
                writer.WriteLine("\t\thttp://digital.uflib.ufl.edu/metadata/sobekcm_config/sobekcm_config.xsd\">");
                writer.WriteLine("\t<QualityControl>");
                writer.WriteLine("\t\t<Profiles>");
              
                foreach (QualityControl_Profile profile in profilesDictionary.Values)
                {
                    writer.Write("\t\t\t<Profile ");
                    if (profile.Default_Profile)
                        writer.Write("isDefault=\"true\" ");
                    writer.Write("name=\"" + Convert_String_To_XML_Safe(profile.Profile_Name) + "\" ");
                    writer.WriteLine("description=\"" + Convert_String_To_XML_Safe(profile.Profile_Description) + "\">");

                    foreach (QualityControl_Division_Config thisConfig in profile.Division_Types)
                    {
                        writer.Write("\t\t\t\t<DivisionType DivisionID=\"" + thisConfig.ID + "\" type=\"" + Convert_String_To_XML_Safe(thisConfig.TypeName) + "\" ");
                        if ( !thisConfig.isActive ) writer.Write( "isActive=\"false\" ");
                        writer.Write(thisConfig.isNameable ? "isNameable=\"true\" " : "isNameable=\"false\" ");
                        if (thisConfig.BaseTypeName.Length > 0)
                            writer.Write("base=\"" + Convert_String_To_XML_Safe(thisConfig.BaseTypeName) + "\" ");

                        if (thisConfig.hasTranslations)
                        {
                            writer.WriteLine(">");

                            thisConfig.Write_Translations(writer);

                            writer.WriteLine("\t\t\t\t</DivisionType>");

                        }
                        else
                        {
                            writer.WriteLine(" />");
                        }
                    }

                    writer.WriteLine("\t\t\t</Profile>");
                }

                writer.WriteLine("\t\t</Profiles>");
                writer.WriteLine("\t</QualityControl>");
                writer.WriteLine("</SobekCM_Config>");
                writer.Flush();
                writer.Close();
            }
            catch 
            {
                returnValue = false;
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }

            return returnValue;
        }

        /// <summary> Converts a basic string into an XML-safe string </summary>
        /// <param name="Element"> Element data to convert </param>
        /// <returns> Data converted into an XML-safe string</returns>
        private string Convert_String_To_XML_Safe(string Element)
        {
            if (Element == null)
                return string.Empty;

            string xml_safe = Element;
            int i = xml_safe.IndexOf("&");
            while (i >= 0)
            {
                if ((i != xml_safe.IndexOf("&amp;", i, StringComparison.Ordinal)) && (i != xml_safe.IndexOf("&quot;", i, StringComparison.Ordinal)) &&
                    (i != xml_safe.IndexOf("&gt;", i, StringComparison.Ordinal)) && (i != xml_safe.IndexOf("&lt;", i, StringComparison.Ordinal)))
                {
                    xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
                }

                i = xml_safe.IndexOf("&", i + 1, StringComparison.Ordinal);
            }
            return xml_safe.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        #endregion

        #region Code for reading the configuration from a XML file

        /// <summary> Read the metadata configuration from a correctly-formatted metadata configuration XML file </summary>
        /// <param name="Configuration_XML_File"> File/path for the metadata configuration XML file to read </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Read_Metadata_Configuration(string Configuration_XML_File)
        {
            attemptedRead = true;

            // Clear all the values first
            Clear();

            bool returnValue = true;
            // Streams used for reading
            Stream readerStream = null;
            XmlTextReader readerXml = null;

            try
            {
                // Open a link to the file
                readerStream = new FileStream(Configuration_XML_File, FileMode.Open, FileAccess.Read);

                // Open a XML reader connected to the file
                readerXml = new XmlTextReader(readerStream);

                while (readerXml.Read())
                {
                    if (readerXml.NodeType == XmlNodeType.Element)
                    {
                        switch (readerXml.Name.ToLower())
                        {
                            case "profiles":
                                read_qc_profiles(readerXml.ReadSubtree());
                                break;
                        }
                    }
                }
            }
            catch
            {
                returnValue = false;
            }
            finally
            {
                if (readerXml != null)
                {
                    readerXml.Close();
                }
                if (readerStream != null)
                {
                    readerStream.Close();
                }
            }

            // If there was an error while reading, use the system defaults
            if (!returnValue)
            {
                Clear();
                Set_Default_Values();
            }

            return returnValue;
        }


        private void read_qc_profiles(XmlReader ReaderXml)
        {
            int unnamed_profile_counter = 1;

            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "profile":
                            QualityControl_Profile profile = new QualityControl_Profile();
                            XmlReader child_readerXml = ReaderXml.ReadSubtree();
                            if (ReaderXml.MoveToAttribute("name"))
                                profile.Profile_Name = ReaderXml.Value.Trim();
                            if (ReaderXml.MoveToAttribute("description"))
                                profile.Profile_Description = ReaderXml.Value;
                            if (ReaderXml.MoveToAttribute("isDefault"))
                            {
                                bool tempValue;
                                if (bool.TryParse(ReaderXml.Value, out tempValue))
                                {
                                    profile.Default_Profile = tempValue;
                                }
                            }
                            // Enforce a name for this profile (should have one according to XSD)
                            if (profile.Profile_Name.Length == 0)
                            {
                                profile.Profile_Name = "Unnamed" + unnamed_profile_counter;
                                unnamed_profile_counter++;
                            }

                            

                            while (child_readerXml.Read())
                            {
                                if (child_readerXml.NodeType == XmlNodeType.Element && child_readerXml.Name.ToLower() == "divisiontype")
                                {
                                    if (child_readerXml.Name.ToLower() == "divisiontype")
                                    {
                                        QualityControl_Division_Config thisConfig = new QualityControl_Division_Config();
                                        if (child_readerXml.MoveToAttribute("type"))
                                        {
                                            thisConfig.TypeName = child_readerXml.Value;
                                        }
                                        if (child_readerXml.MoveToAttribute("isNameable"))
                                            thisConfig.isNameable = Convert.ToBoolean(child_readerXml.Value);
                                        profile.Add_Division_Type(thisConfig);

                                    }
                                }
                            }
                            Add_Profile(profile);
                            break;

                    }
                }
            }
        }

        #endregion
    }

    #region QualityControl_Profile class 

    /// <summary> Stores information about a single profile for performing quality 
    /// control online </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("QualityControlProfile")]
    public class QualityControl_Profile
    {
        /// <summary> Name associated with this profile </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string Profile_Name { get; set; }

        /// <summary> Description associated with this profile </summary>
        [DataMember(Name = "description", EmitDefaultValue = false)]
        [XmlAttribute("description")]
        [ProtoMember(2)]
        public string Profile_Description { get; set; }

        /// <summary> Flag indicates if this is the default profile </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public bool Default_Profile { get; internal set; }

        private Dictionary<string, QualityControl_Division_Config> divisionTypeLookup;

        /// <summary> Constructor for a new instance of the QualityControl_Profile class </summary>
        public QualityControl_Profile()
        {
            Division_Types = new List<QualityControl_Division_Config>();
            divisionTypeLookup = new Dictionary<string, QualityControl_Division_Config>();

            Default_Profile = false;
            Profile_Name = String.Empty;
            Profile_Description = String.Empty;
        }

        /// <summary> Constructor for a new instance of the QualityControl_Profile class </summary>
        /// <param name="Profile_Name"> Name associated with this profile </param>
        /// <param name="Profile_Description"> Description associated with this profile </param>
        /// <param name="Default_Profile"> Flag indicates if this is the default profile </param>
        public QualityControl_Profile( string Profile_Name, string Profile_Description, bool Default_Profile )
        {
            Division_Types = new List<QualityControl_Division_Config>();
            divisionTypeLookup = new Dictionary<string, QualityControl_Division_Config>();

            this.Default_Profile = Default_Profile;
            this.Profile_Name = Profile_Name;
            this.Profile_Description = Profile_Description;
        }

        /// <summary> Return the list of all division types in this profile </summary>
        [DataMember(Name = "divisionTypes")]
        [XmlArray("divisionTypes")]
        [XmlArrayItem("type", typeof(QualityControl_Division_Config))]
        [ProtoMember(3)]
        public List<QualityControl_Division_Config> Division_Types { get; set;  }


        /// <summary> Add a new division type to this profile </summary>
        /// <param name="Division_Config"> New division type to add </param>
        public void Add_Division_Type(QualityControl_Division_Config Division_Config )
        {
            Division_Types.Add(Division_Config);
            divisionTypeLookup[Division_Config.TypeName] = Division_Config;
        }

        /// <summary> Remove a division type from this profile </summary>
        /// <param name="Division_Config"> Division type to remove </param>
        public void Remove_Division_Type( QualityControl_Division_Config Division_Config )
        {
            Division_Types.Remove(Division_Config);
            divisionTypeLookup.Remove(Division_Config.TypeName);
        }

        /// <summary> Gets the configuration information for a single division type,
        /// from the type name </summary>
        /// <param name="TypeName"> Name of the division type to retrieve</param>
        /// <returns>Either NULL or the matching division config</returns>
        [XmlIgnore]
        [IgnoreDataMember]
        public QualityControl_Division_Config this[string TypeName]
        {
            get 
            {
                // Ensure the dictionary exists and is current
                if ((divisionTypeLookup == null) || (divisionTypeLookup.Count != Division_Types.Count))
                {
                    divisionTypeLookup = new Dictionary<string, QualityControl_Division_Config>();
                    foreach (QualityControl_Division_Config thisConfig in Division_Types)
                    {
                        divisionTypeLookup[thisConfig.TypeName] = thisConfig;
                    }
                }

                // Now, return the match
                return divisionTypeLookup.ContainsKey(TypeName) ? divisionTypeLookup[TypeName] : null;
            }
        }
    }

    #endregion

    #region QualityControl_Division_Config class 
 
    /// <summary> Configuration information for a single possible division type </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("QualityControlDivisionConfig")]
    public class QualityControl_Division_Config  
    {
        /// <summary> Key for this division configuration information </summary>
        /// <remarks> This may be used for updating the data or something, not currently used?? </remarks>
        [XmlIgnore]
        [IgnoreDataMember]
        internal int ID { get; set;  }

        /// <summary> Name of this type, which the user selects.  This is also the default name, if
        /// a translation for this name is not provided in a requested language </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string TypeName { get; set;  }

        /// <summary> Flag indicates if this division is currently active </summary>
        [DataMember(Name = "active", EmitDefaultValue = false)]
        [XmlAttribute("active")]
        [ProtoMember(2)]
        public bool isActive { get; set; }

        /// <summary> Flag indicates if the user will be asked for a name when this division is chosen </summary>
        [DataMember(Name = "nameable", EmitDefaultValue = false)]
        [XmlAttribute("nameable")]
        [ProtoMember(3)]
        public bool isNameable  {  get; set;  }

        /// <summary> If this is set, then the typename is actually used as the division label
        /// and this is used as the type within the METS structure map </summary>
        [DataMember(Name = "baseType", EmitDefaultValue = false)]
        [XmlAttribute("baseType")]
        [ProtoMember(4)]
        public string BaseTypeName { get; set; }

        /// <summary> List of all the translation values for this type, including the language and the term </summary>
        [DataMember(Name = "translations")]
        [XmlArray("translations")]
        [XmlArrayItem("translation", typeof(Web_Language_Translation_Value))]
        [ProtoMember(5)]
        public List<Web_Language_Translation_Value> TypeTranslations;

        private Dictionary<Web_Language_Enum, string> typeTranslationsDictionary;

        /// <summary> Constructor for a new instance of the QualityControl_Division_Config class </summary>
        public QualityControl_Division_Config()
        {
            TypeTranslations = new List<Web_Language_Translation_Value>();
            typeTranslationsDictionary = new Dictionary<Web_Language_Enum, string>();
            ID = -1;
            TypeName = String.Empty;
            isActive = true;
            isNameable = false;
            BaseTypeName = String.Empty;
        }

        /// <summary> Constructor for a new instance of the QualityControl_Division_Config class </summary>
        /// <param name="ID"> Key for this division configuration information </param>
        /// <param name="TypeName"> Name of this type, which the user selects.  This is also the default name, if
        /// a translation for this name is not provided in a requested language </param>
        /// <param name="isActive"> Flag indicates if this division is currently active </param>
        /// <param name="isNameable"> Flag indicates if the user will be asked for a name when this division is chosen </param>
        public QualityControl_Division_Config(int ID, string TypeName, bool isActive, bool isNameable)
        {
            TypeTranslations = new List<Web_Language_Translation_Value>();
            typeTranslationsDictionary = new Dictionary<Web_Language_Enum, string>();
            this.ID = ID;
            this.TypeName = TypeName;
            this.isActive = isActive;
            this.isNameable = isNameable;
            BaseTypeName = String.Empty;
        }

        /// <summary> Constructor for a new instance of the QualityControl_Division_Config class </summary>
        /// <param name="ID"> Key for this division configuration information </param>
        /// <param name="TypeName"> Name of this type, which the user selects.  This is also the default name, if
        /// a translation for this name is not provided in a requested language </param>
        /// <param name="isActive"> Flag indicates if this division is currently active </param>
        /// <param name="isNameable"> Flag indicates if the user will be asked for a name when this division is chosen </param>
        /// <param name="BaseTypeName"> If this is set, then the typename is actually used as the division label
        /// and this is used as the type within the METS structure map </param>
        public QualityControl_Division_Config(int ID, string TypeName, bool isActive, bool isNameable, string BaseTypeName )
        {
            TypeTranslations = new List<Web_Language_Translation_Value>();
            typeTranslationsDictionary = new Dictionary<Web_Language_Enum, string>();
            this.ID = ID;
            this.TypeName = TypeName;
            this.isActive = isActive;
            this.isNameable = isNameable;
            this.BaseTypeName = BaseTypeName;
        }

        /// <summary> Flag indicates if this division config has translations </summary>
        internal bool hasTranslations
        {
            get { return (TypeTranslations.Count > 0); }
        }

        internal void Write_Translations(StreamWriter writer)
        {
            foreach (KeyValuePair<Web_Language_Enum, string> translation in typeTranslationsDictionary)
            {
                writer.WriteLine("\t\t\t\t\t<Translation language=\"" + Web_Language_Enum_Converter.Enum_To_Code(translation.Key) + "\" text=\"" + Convert_String_To_XML_Safe(translation.Value) + "\" />");
            }
        }

        /// <summary> Get the type translated into the language specified </summary>
        /// <param name="Language"> UI language </param>
        /// <param name="useDefaultIfNotPresent"> Flag determines if the default type should be returned
        /// if the specified language does not exist </param>
        /// <returns> Transalted type, or an empty string </returns>
        public string Get_Translation( Web_Language_Enum Language, bool useDefaultIfNotPresent )
        {
            if (typeTranslationsDictionary.ContainsKey(Language))
                return typeTranslationsDictionary[Language];

            return useDefaultIfNotPresent ? TypeName : String.Empty;
        }

        /// <summary> Add a translated type for this division </summary>
        /// <param name="Language"> Language for this translation </param>
        /// <param name="Translation"> Translation of the type of this division </param>
        public void Add_Translation( Web_Language_Enum Language, string Translation )
        {
            TypeTranslations.Add(new Web_Language_Translation_Value(Language, Translation));
            typeTranslationsDictionary[Language] = Translation;
        }

        #region Methods that controls XML serialization

        /// <summary> Method suppresses XML Serialization of the BaseTypeName property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeBaseTypeName()
        {
            return (!String.IsNullOrEmpty(BaseTypeName));
        }

        /// <summary> Method suppresses XML Serialization of the TypeTranslations collection if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeTypeTranslations()
        {
            return (TypeTranslations != null) && (TypeTranslations.Count > 0);
        }

        #endregion


        /// <summary> Converts a basic string into an XML-safe string </summary>
        /// <param name="element"> Element data to convert </param>
        /// <returns> Data converted into an XML-safe string</returns>
        private static string Convert_String_To_XML_Safe(string element)
        {
            if (element == null)
                return string.Empty;

            string xml_safe = element;
            int i = xml_safe.IndexOf("&");
            while (i >= 0)
            {
                if ((i != xml_safe.IndexOf("&amp;", i, StringComparison.Ordinal)) && (i != xml_safe.IndexOf("&quot;", i, StringComparison.Ordinal)) &&
                    (i != xml_safe.IndexOf("&gt;", i, StringComparison.Ordinal)) && (i != xml_safe.IndexOf("&lt;", i, StringComparison.Ordinal)))
                {
                    xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
                }

                i = xml_safe.IndexOf("&", i + 1, StringComparison.Ordinal);
            }
            return xml_safe.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }
    }

    #endregion
}
