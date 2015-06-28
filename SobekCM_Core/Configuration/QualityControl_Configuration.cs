#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

#endregion

namespace SobekCM.Core.Configuration
{
    /// <summary> Class keeps all the system-wide quality control profiles which 
    /// can be used within the system  </summary>
    public static class QualityControl_Configuration
    {
        private static bool attemptedRead;
        private static Dictionary<string, QualityControl_Profile> profiles;
        private static QualityControl_Profile defaultProfile;
        private static string sobekcm_qc_configfilePath;

        
        /// <summary> Static constructor for the QualityControl_Configuration class </summary>
         static QualityControl_Configuration()
        {
            // Declare all the new collections in this configuration 
            profiles = new Dictionary<string, QualityControl_Profile>();

            // Set some default values
            attemptedRead = false;
            defaultProfile = null;

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            sobekcm_qc_configfilePath=(baseDirectory + "config\\default\\sobekcm_qc.config");

            if (!Read_Metadata_Configuration(sobekcm_qc_configfilePath))
                // Set default reader/writer values to have a baseline in case there is
                // no file to be read 
                Set_Default_Values();
        }



        private static void Clear()
        {
            profiles.Clear();
            defaultProfile = null;
        }

        /// <summary> Flag indicates if the method to read the configuration file has been called </summary>
        /// <remarks> Even if the read is unsuccesful for any reason, this returns TRUE to prevent 
        /// the read method from being called over and over </remarks>
        public static bool Attempted_To_Read_Config_File
        {
            get { return attemptedRead; }
        }


        /// <summary> Add a quality control profile with user settings, such as which 
        /// division types to include for selection </summary>
        /// <param name="New_Profile"> New profile to add </param>
        public static void Add_Profile(QualityControl_Profile New_Profile)
        {
            // Add to the dictionary of profiles
            profiles[New_Profile.Profile_Name] = New_Profile;

            // Was this the default profile?
            if (New_Profile.Default_Profile)
                defaultProfile = New_Profile;
        }

        /// <summary> Default quality control profile </summary>
        public static QualityControl_Profile Default_Profile
        {
            get
            {
                if (defaultProfile != null)
                    return defaultProfile;
                if (profiles.Count > 0)
                    return profiles[profiles.Keys.FirstOrDefault()];
                return null;
            }
        }

        #region Set default values (used if no config file is present)

        /// <summary> Set the default profile with the default values </summary>
        public static void Set_Default_Values()
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
        public static bool Save_To_Config_File(string FilePath)
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
              
                foreach (QualityControl_Profile profile in profiles.Values)
                {
                    writer.Write("\t\t\t<Profile ");
                    if (profile.Default_Profile)
                        writer.Write("isDefault=\"true\" ");
                    writer.Write("name=\"" + Convert_String_To_XML_Safe(profile.Profile_Name) + "\" ");
                    writer.WriteLine("description=\"" + Convert_String_To_XML_Safe(profile.Profile_Description) + "\">");

                    foreach (QualityControl_Division_Config thisConfig in profile.All_Division_Types)
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
        private static string Convert_String_To_XML_Safe(string Element)
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
        public static bool Read_Metadata_Configuration(string Configuration_XML_File)
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


        private static void read_qc_profiles(XmlReader ReaderXml)
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

                                    //  while (readerXml.ReadToNextSibling("DivisionType"))
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
                   //         Add_METS_Writing_Profile(profile);
                            break;

                        case "package_scope":
                            break;

                        case "division_scope":
                            break;

                        case "file_scope":
                            break;

                        case "dmdsec":
                            break;

                        case "amdsec":
                            break;

                        //case "readerwriterref":
                        //    if (readerXml.MoveToAttribute("ID"))
                        //    {
                        //        string id = readerXml.Value.ToUpper();
                        //        if ((readerWriters.ContainsKey(id)) && (profile != null))
                        //        {
                        //            METS_Section_ReaderWriter_Config readerWriter = readerWriters[id];
                        //            if (inPackage)
                        //            {
                        //                if (inDmdSec)
                        //                    profile.Add_Package_Level_DmdSec_Writer_Config(readerWriter);
                        //                else
                        //                    profile.Add_Package_Level_AmdSec_Writer_Config(readerWriter);
                        //            }
                        //            else if (inDivision)
                        //            {
                        //                if (inDmdSec)
                        //                    profile.Add_Division_Level_DmdSec_Writer_Config(readerWriter);
                        //                else
                        //                    profile.Add_Division_Level_AmdSec_Writer_Config(readerWriter);
                        //            }
                        //            else if (inFile)
                        //            {
                        //                if (inDmdSec)
                        //                    profile.Add_File_Level_DmdSec_Writer_Config(readerWriter);
                        //                else
                        //                    profile.Add_File_Level_AmdSec_Writer_Config(readerWriter);
                        //            }
                        //        }
                        //    }
                        //    break;
                    }
                }
            }
        }

        #endregion
    }

    #region QualityControl_Profile class 

    /// <summary> Stores information about a single profile for performing quality 
    /// control online </summary>
    public class QualityControl_Profile
    {
        /// <summary> Name associated with this profile </summary>
        public string Profile_Name { get; internal set; }

        /// <summary> Description associated with this profile </summary>
        public string Profile_Description { get; internal set; }

        /// <summary> Flag indicates if this is the default profile </summary>
        public bool Default_Profile { get; internal set; }

        private readonly List<QualityControl_Division_Config> divisionTypes;

        private readonly Dictionary<string, QualityControl_Division_Config> divisionTypeLookup;

        /// <summary> Constructor for a new instance of the QualityControl_Profile class </summary>
        public QualityControl_Profile()
        {
            divisionTypes = new List<QualityControl_Division_Config>();
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
            divisionTypes = new List<QualityControl_Division_Config>();
            divisionTypeLookup = new Dictionary<string, QualityControl_Division_Config>();

            this.Default_Profile = Default_Profile;
            this.Profile_Name = Profile_Name;
            this.Profile_Description = Profile_Description;
        }

        /// <summary> Return the list of all division types in this profile </summary>
        public ReadOnlyCollection<QualityControl_Division_Config> All_Division_Types
        {
            get { return new ReadOnlyCollection<QualityControl_Division_Config>(divisionTypes); }
        }

        /// <summary> Add a new division type to this profile </summary>
        /// <param name="Division_Config"> New division type to add </param>
        public void Add_Division_Type(QualityControl_Division_Config Division_Config )
        {
            divisionTypes.Add(Division_Config);
            divisionTypeLookup[Division_Config.TypeName] = Division_Config;
        }

        /// <summary> Remove a division type from this profile </summary>
        /// <param name="Division_Config"> Division type to remove </param>
        public void Remove_Division_Type( QualityControl_Division_Config Division_Config )
        {
            divisionTypes.Remove(Division_Config);
            divisionTypeLookup.Remove(Division_Config.TypeName);
        }

        /// <summary> Gets the configuration information for a single division type,
        /// from the type name </summary>
        /// <param name="TypeName"> Name of the division type to retrieve</param>
        /// <returns>Either NULL or the matching division config</returns>
        public QualityControl_Division_Config  this[string TypeName]
        {
            get {
                return divisionTypeLookup.ContainsKey(TypeName) ? divisionTypeLookup[TypeName] : null;
            }
        }
    }

    #endregion

    #region QualityControl_Division_Config class 
 
    /// <summary> Configuration information for a single possible division type </summary>
    public class QualityControl_Division_Config  
    {
        /// <summary> Key for this division configuration information </summary>
        /// <remarks> This may be used for updating the data or something, not currently used?? </remarks>
        internal int ID { get; set;  }

        /// <summary> Name of this type, which the user selects.  This is also the default name, if
        /// a translation for this name is not provided in a requested language </summary>
        public string TypeName { get; internal set;  }

        /// <summary> Flag indicates if this division is currently active </summary>
        public bool isActive { get; internal set; }

        /// <summary> Flag indicates if the user will be asked for a name when this division is chosen </summary>
        public bool isNameable  {  get; internal set;  }

        /// <summary> If this is set, then the typename is actually used as the division label
        /// and this is used as the type within the METS structure map </summary>
        public string BaseTypeName { get; internal set; }

        private Dictionary<Web_Language_Enum, string> typeTranslations;

        /// <summary> Constructor for a new instance of the QualityControl_Division_Config class </summary>
        public QualityControl_Division_Config()
        {
            typeTranslations = new Dictionary<Web_Language_Enum, string>();
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
            typeTranslations = new Dictionary<Web_Language_Enum, string>();
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
            typeTranslations = new Dictionary<Web_Language_Enum, string>();
            this.ID = ID;
            this.TypeName = TypeName;
            this.isActive = isActive;
            this.isNameable = isNameable;
            this.BaseTypeName = BaseTypeName;
        }

        /// <summary> Flag indicates if this division config has translations </summary>
        internal bool hasTranslations
        {
            get { return (typeTranslations.Count > 0); }
        }

        internal void Write_Translations(StreamWriter writer)
        {
            foreach (KeyValuePair<Web_Language_Enum, string> translation in typeTranslations)
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
            if (typeTranslations.ContainsKey(Language))
                return typeTranslations[Language];

            return useDefaultIfNotPresent ? TypeName : String.Empty;
        }

        /// <summary> Add a translated type for this division </summary>
        /// <param name="Language"> Language for this translation </param>
        /// <param name="Translation"> Translation of the type of this division </param>
        public void Add_Translation( Web_Language_Enum Language, string Translation )
        {
            typeTranslations[Language] = Translation;
        }


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
