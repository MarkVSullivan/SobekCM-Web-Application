using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using SobekCM.Resource_Object.Configuration;

namespace SobekCM.Library.Configuration
{
    /// <summary> Class keeps all the system-wide quality control profiles which 
    /// can be used within the system  </summary>
    public static class QualityControl_Configuration
    {
        private static bool attemptedRead;
        private static Dictionary<string, QualityControl_Profile> profiles;
        private static QualityControl_Profile defaultProfile;

        /// <summary> Static constructor for the QualityControl_Configuration class </summary>
        static QualityControl_Configuration()
        {
            // Declare all the new collections in this configuration 
            profiles = new Dictionary<string, QualityControl_Profile>();

            // Set some default values
            attemptedRead = false;
            defaultProfile = null;

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

        // Set the default profile with the default values 
        public static void Set_Default_Values()
        {
            // Clear everything, just in case
            Clear();

            // Create the default profile
            QualityControl_Profile newProfile = new QualityControl_Profile();
            newProfile.Default_Profile = true;
            newProfile.Profile_Name = "System Default";
            newProfile.Profile_Description = "Default profile used when no config file is present";
            Add_Profile(newProfile);

            // Add back cover
            QualityControl_Division_Config div1 = new QualityControl_Division_Config();
            div1.ID = 1;
            div1.TypeName = "Back Cover";
            div1.isActive = true;
            div1.isNameable = false;
            div1.BaseTypeName = "Cover";
            div1.Add_Translation(Web_Language_Enum.Spanish, "Portada Posterior");
            div1.Add_Translation(Web_Language_Enum.French, "Couverture Arrière");
            newProfile.Add_Division_Type(div1);

            // Add back matter
            QualityControl_Division_Config div2 = new QualityControl_Division_Config();
            div2.ID = 2;
            div2.TypeName = "Back Matter";
            div2.isActive = true;
            div2.isNameable = false;
            div2.Add_Translation(Web_Language_Enum.Spanish, "Materia Posterior");
            div2.Add_Translation(Web_Language_Enum.French, "Matière Arrière");
            newProfile.Add_Division_Type(div2);

            // Add chapter ( misorder of the object names here and below matters not)
            QualityControl_Division_Config div4 = new QualityControl_Division_Config();
            div4.ID = 3;
            div4.TypeName = "Chapter";
            div4.isActive = true;
            div4.isNameable = true;
            div4.Add_Translation(Web_Language_Enum.Spanish, "Capítulo");
            div4.Add_Translation(Web_Language_Enum.French, "Chapitre");
            newProfile.Add_Division_Type(div4);

            // Add front cover
            QualityControl_Division_Config div3 = new QualityControl_Division_Config();
            div3.ID = 4;
            div3.TypeName = "Front Cover";
            div3.isActive = true;
            div3.isNameable = false;
            div3.BaseTypeName = "Cover";
            div3.Add_Translation(Web_Language_Enum.Spanish, "Portada Delantera");
            div3.Add_Translation(Web_Language_Enum.French, "Couverture Frente");
            newProfile.Add_Division_Type(div3);

            // Add front matter
            QualityControl_Division_Config div5 = new QualityControl_Division_Config();
            div5.ID = 5;
            div5.TypeName = "Front Matter";
            div5.isActive = true;
            div5.isNameable = false;
            div5.Add_Translation(Web_Language_Enum.Spanish, "Materia Delantera");
            div5.Add_Translation(Web_Language_Enum.French, "Préliminaires");
            newProfile.Add_Division_Type(div5);

            // Add index
            QualityControl_Division_Config div6 = new QualityControl_Division_Config();
            div6.ID = 6;
            div6.TypeName = "Index";
            div6.isActive = true;
            div6.isNameable = true;
            div6.Add_Translation(Web_Language_Enum.Spanish, "Indice");
            div6.Add_Translation(Web_Language_Enum.French, "Indice");
            newProfile.Add_Division_Type(div6);

            // Add introduction
            QualityControl_Division_Config div7 = new QualityControl_Division_Config();
            div7.ID = 7;
            div7.TypeName = "Introduction";
            div7.isActive = true;
            div7.isNameable = false;
            div7.BaseTypeName = "Chapter";
            div7.Add_Translation(Web_Language_Enum.Spanish, "Introducción");
            div7.Add_Translation(Web_Language_Enum.French, "Introduction");
            newProfile.Add_Division_Type(div7);

            // Add spine
            QualityControl_Division_Config div8 = new QualityControl_Division_Config();
            div8.ID = 8;
            div8.TypeName = "Spine";
            div8.isActive = true;
            div8.isNameable = false;
            div8.Add_Translation(Web_Language_Enum.Spanish, "Canto");
            div8.Add_Translation(Web_Language_Enum.French, "Épine de livre");
            newProfile.Add_Division_Type(div8);

            // Add table of contents
            QualityControl_Division_Config div9 = new QualityControl_Division_Config();
            div9.ID = 9;
            div9.TypeName = "Table of Contents";
            div9.isActive = true;
            div9.isNameable = false;
            div9.BaseTypeName = "Contents";
            div9.Add_Translation(Web_Language_Enum.Spanish, "Contenidos");
            div9.Add_Translation(Web_Language_Enum.French, "Table des Matières");
            newProfile.Add_Division_Type(div9);

            // Add title page
            QualityControl_Division_Config div10 = new QualityControl_Division_Config();
            div10.ID = 10;
            div10.TypeName = "Title Page";
            div10.isActive = true;
            div10.isNameable = false;
            div10.BaseTypeName = "Title";
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
            catch ( Exception ee)
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
                if ((i != xml_safe.IndexOf("&amp;", i)) && (i != xml_safe.IndexOf("&quot;", i)) &&
                    (i != xml_safe.IndexOf("&gt;", i)) && (i != xml_safe.IndexOf("&lt;", i)))
                {
                    xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
                }

                i = xml_safe.IndexOf("&", i + 1);
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
            catch (Exception ee)
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


        private static void read_qc_profiles(XmlReader readerXml)
        {
            bool inPackage = false;
            bool inDivision = false;
            bool inFile = false;
            bool inDmdSec = true;
            METS_Writing_Profile profile = null;
            int unnamed_profile_counter = 1;

            while (readerXml.Read())
            {
                //if (readerXml.NodeType == XmlNodeType.Element)
                //{
                //    switch (readerXml.Name.ToLower())
                //    {
                //        case "profile":
                //            profile = new METS_Writing_Profile();
                //            if (readerXml.MoveToAttribute("name"))
                //                profile.Profile_Name = readerXml.Value.Trim();
                //            if (readerXml.MoveToAttribute("description"))
                //                profile.Profile_Description = readerXml.Value;
                //            if (readerXml.MoveToAttribute("isDefault"))
                //            {
                //                bool tempValue;
                //                if (bool.TryParse(readerXml.Value, out tempValue))
                //                {
                //                    profile.Default_Profile = tempValue;
                //                }
                //            }
                //            // Enforce a name for this profile (should have one according to XSD)
                //            if (profile.Profile_Name.Length == 0)
                //            {
                //                profile.Profile_Name = "Unnamed" + unnamed_profile_counter;
                //                unnamed_profile_counter++;
                //            }
                //            Add_METS_Writing_Profile(profile);
                //            break;

                //        case "package_scope":
                //            inPackage = true;
                //            inDivision = false;
                //            inFile = false;
                //            break;

                //        case "division_scope":
                //            inPackage = false;
                //            inDivision = true;
                //            inFile = false;
                //            break;

                //        case "file_scope":
                //            inPackage = false;
                //            inDivision = false;
                //            inFile = true;
                //            break;

                //        case "dmdsec":
                //            inDmdSec = true;
                //            break;

                //        case "amdsec":
                //            inDmdSec = false;
                //            break;

                //        case "readerwriterref":
                //            if (readerXml.MoveToAttribute("ID"))
                //            {
                //                string id = readerXml.Value.ToUpper();
                //                if ((readerWriters.ContainsKey(id)) && (profile != null))
                //                {
                //                    METS_Section_ReaderWriter_Config readerWriter = readerWriters[id];
                //                    if (inPackage)
                //                    {
                //                        if (inDmdSec)
                //                            profile.Add_Package_Level_DmdSec_Writer_Config(readerWriter);
                //                        else
                //                            profile.Add_Package_Level_AmdSec_Writer_Config(readerWriter);
                //                    }
                //                    else if (inDivision)
                //                    {
                //                        if (inDmdSec)
                //                            profile.Add_Division_Level_DmdSec_Writer_Config(readerWriter);
                //                        else
                //                            profile.Add_Division_Level_AmdSec_Writer_Config(readerWriter);
                //                    }
                //                    else if (inFile)
                //                    {
                //                        if (inDmdSec)
                //                            profile.Add_File_Level_DmdSec_Writer_Config(readerWriter);
                //                        else
                //                            profile.Add_File_Level_AmdSec_Writer_Config(readerWriter);
                //                    }
                //                }
                //            }
                //            break;
                //    }
                //}
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

        private List<QualityControl_Division_Config> divisionTypes;

        /// <summary> Constructor for a new instance of the QualityControl_Profile class </summary>
        public QualityControl_Profile()
        {
            divisionTypes = new List<QualityControl_Division_Config>();

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
        }

        /// <summary> Remove a division type from this profile </summary>
        /// <param name="Division_Config"> Division type to remove </param>
        public void Remove_Division_Type( QualityControl_Division_Config Division_Config )
        {
            divisionTypes.Remove(Division_Config);
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
                if ((i != xml_safe.IndexOf("&amp;", i)) && (i != xml_safe.IndexOf("&quot;", i)) &&
                    (i != xml_safe.IndexOf("&gt;", i)) && (i != xml_safe.IndexOf("&lt;", i)))
                {
                    xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
                }

                i = xml_safe.IndexOf("&", i + 1);
            }
            return xml_safe.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }
    }

    #endregion
}
