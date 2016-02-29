using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Xml;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Authentication;
using SobekCM.Core.Configuration.Engine;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Configuration.OAIPMH;
using SobekCM.Core.Settings;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.Citation;
using SobekCM.Core.UI_Configuration.TemplateElements;
using SobekCM.Core.UI_Configuration.Viewers;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Items.BriefItems.Mappers;
using SobekCM.Resource_Object.Configuration;

namespace SobekCM.Engine_Library.Configuration
{
    /// <summary> Reads all of the configuration files into the 
    /// configuration objects </summary>
    public class Configuration_Files_Reader
    {

        /// <summary> Refreshes the values from the database settings </summary>
        /// <returns> A fully builder instance-wide setting object </returns>
        public static InstanceWide_Configuration Read_Config_Files( InstanceWide_Settings Settings)
        {
            // Get the directories to read
            List<string> configurationDirectories = new List<string>();

            // Add the default configuration directory first
            configurationDirectories.Add(Path.Combine(Settings.Servers.Application_Server_Network, "config", "default"));

            // Add all of the plug-in foldersm but ensure they are sorted
            string plug_in_folder = Path.Combine(Settings.Servers.Application_Server_Network, "plugins");
            if (Directory.Exists(plug_in_folder))
            {
                // Get the list of subdirectories 
                string[] subdirs = Directory.GetDirectories(plug_in_folder);

                // Ensure it is sorted alphabetically
                SortedList<string, string> subdirs_sorted = new SortedList<string, string>();
                foreach (string thisSubDir in subdirs)
                {
                    // Get the directory name and add to the sorted list
                    string dirName = (new DirectoryInfo(thisSubDir)).Name;
                    subdirs_sorted.Add(dirName, thisSubDir);
                }

                // Now, add each folder correctly sorted
                foreach (string thisSubDir in subdirs_sorted.Values)
                {
                    configurationDirectories.Add(thisSubDir);
                    if (Directory.Exists(Path.Combine(thisSubDir, "config")))
                        configurationDirectories.Add(Path.Combine(thisSubDir, "config"));
                }
            }

            // Add the final user configuration directory last
            configurationDirectories.Add(Path.Combine(Settings.Servers.Application_Server_Network, "config", "user"));

            return Read_Config_Files(configurationDirectories, Settings);
        }

        /// <summary> Refreshes the values from the database settings </summary>
        /// <returns> A fully builder instance-wide setting object </returns>
        public static InstanceWide_Configuration Read_Config_Files(List<string> ConfigurationDirectories, InstanceWide_Settings Settings)
        {
            // Start to build the instance wide configuration
            InstanceWide_Configuration returnValue = new InstanceWide_Configuration();
            
            // Add an initial log, with the data
            returnValue.Add_Log("Beginning to read configuration files (" + DateTime.Now.ToShortDateString() + ")");

            // Log the directories to look within
            returnValue.Add_Log();
            returnValue.Add_Log("Looking in the following directories for config/xml files");
            foreach (string configDir in ConfigurationDirectories)
            {
                returnValue.Add_Log("     " + configDir);
            }

            // Step through and get the configuration files to be read (in folder and alphabetical order)
            List<string> configFiles = new List<string>();
            SortedList<string, string> filesSorted = new SortedList<string, string>();
            foreach (string thisConfigDir in ConfigurationDirectories)
            {
                if (Directory.Exists(thisConfigDir))
                {
                    // Get the list of XML and CONFIG files, and read them in alphabetical order
                    string[] files = SobekCM.Tools.SobekCM_File_Utilities.GetFiles(thisConfigDir, "*.xml|*.config");
                    if (files.Length > 0)
                    {
                        // Get all the files and sort by name
                        filesSorted.Clear();
                        foreach (string thisFile in files)
                        {
                            string filename = Path.GetFileName(thisFile);
                            if (filename != null)
                            {
                                filesSorted.Add(filename, thisFile);
                            }
                        }

                        // Add the files to the complete list
                        configFiles.AddRange(filesSorted.Values);
                    }
                }
            }

            // Log this
            returnValue.Add_Log();
            returnValue.Add_Log("Found the following config files to attempt to read:");
            foreach (string configFile in configFiles)
            {
                returnValue.Add_Log("     " + configFile );
            }

            // With all the files to read collected and sorted, read each one
            foreach (string thisConfigFile in configFiles)
            {
                if (read_config_file(thisConfigFile, returnValue, Settings))
                    returnValue.HasData = true;
            }


            // Now, perform some final clean-up functions here now that all the files have been read
            engine_config_finalize(returnValue);

            // Save the metadata configuration to the resource object library
            ResourceObjectSettings.MetadataConfig = returnValue.Metadata;

            returnValue.HasData = true;

            return returnValue;
        }

        private static bool read_config_file(string ConfigFile, InstanceWide_Configuration ConfigObj, InstanceWide_Settings Settings)
        {
            // If the file doesn't exist, that is strange.. but not an error per se
            if (!File.Exists(ConfigFile))
            {
                return true;    
            }

            // Add to the log
            ConfigObj.Add_Log();
            try
            {
                string file = Path.GetFileName(ConfigFile);
                DirectoryInfo dirInfo = new DirectoryInfo(Path.GetDirectoryName(ConfigFile));
                string directory = dirInfo.Name;
                string directory2 = dirInfo.Parent.Name;

                ConfigObj.Add_Log("Reading " + directory2 + "\\" + directory + "\\" + file);
            }
            catch
            {
                ConfigObj.Add_Log("Reading " + ConfigFile + " (Error parsing for logging)");
            }

            // Streams used for reading
            Stream readerStream = null;
            XmlTextReader readerXml = null;
            
            try
            {
                // Check the file for the last modified date
                DateTime lastModifiedDate = (new FileInfo(ConfigFile)).LastWriteTime;
                if (DateTime.Compare(ConfigObj.LatestDateTimeStamp, lastModifiedDate) < 0)
                    ConfigObj.LatestDateTimeStamp = lastModifiedDate;

                // Open a link to the file
                readerStream = new FileStream(ConfigFile, FileMode.Open, FileAccess.Read);

                // Try to read the XML
                readerXml = new XmlTextReader(readerStream);

                // Step through this configuration file
                while (readerXml.Read())
                {
                    if (readerXml.NodeType == XmlNodeType.Element)
                    {
                        switch (readerXml.Name.ToLower())
                        {
                            case "authentication":
                                ConfigObj.Add_Log("        Parsing AUTHENTICATION subtree");
                                read_authentication_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                            case "oai-pmh":
                                ConfigObj.Add_Log("        Parsing OAI-PMH subtree");
                                read_oai_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                            case "contactform":
                                ConfigObj.Add_Log("        Parsing CONTACTFORM subtree");
                                read_contactform_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                            case "briefitem_mapping":
                                ConfigObj.Add_Log("        Parsing BRIEFITEM_MAPPING subtree");
                                read_briefitem_mapping_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                            case "mapeditor":
                                ConfigObj.Add_Log("        Parsing MAPEDITOR subtree");
                                read_mapeditor_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                            case "engine":
                                ConfigObj.Add_Log("        Parsing ENGINE subtree");
                                if (readerXml.MoveToAttribute("ClearAll"))
                                {
                                    if ((readerXml.Value.Trim().ToLower() == "true") && ( ConfigObj.Engine != null ))
                                    {
                                        ConfigObj.Engine.ClearAll();
                                    }
                                    readerXml.MoveToElement();
                                }
                                read_engine_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                            case "qualitycontrol":
                                ConfigObj.Add_Log("        Parsing QUALITYCONTROL subtree");
                                read_quality_control_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                            case "metadata":
                                ConfigObj.Add_Log("        Parsing METADATA subtree");
                                read_metadata_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                            case "writerviewers":
                                ConfigObj.Add_Log("        Parsing WRITER/VIEWER subtree");
                                read_writer_viewer_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                            case "citation":
                                ConfigObj.Add_Log("        Parsing CITATION subtree");
                                read_citation_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                            case "templateelements":
                                ConfigObj.Add_Log("        Parsing TEMPLATE ELEMENTS subtree");
                                read_template_elements_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                        }
                    }
                }
            }
            catch (Exception ee)
            {
                ConfigObj.Add_Log("EXCEPTION CAUGHT in Configuration_Files_Reader.read_config_files");
                ConfigObj.Add_Log(ee.Message);
                ConfigObj.Add_Log(ee.StackTrace);

                ConfigObj.ErrorEncountered = true;
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

            return true;
        }

        #region Section reads all the Shibboleth information

        private static void read_authentication_details(XmlReader ReaderXml, InstanceWide_Configuration Config)
        {
            // Ensure the config object exists
            if (Config.UI.MapEditor == null)
                Config.UI.MapEditor = new MapEditor_Configuration();


            while (ReaderXml.Read())
            {
                // Only detect start elements.
                if (ReaderXml.IsStartElement())
                {
                    // Get element name and switch on it.
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "shibboleth":
                            // Ensure the object is there then
                            if (Config.Authentication.Shibboleth == null)
                                Config.Authentication.Shibboleth = new Shibboleth_Configuration();

                            // Read the attributes
                            if (ReaderXml.MoveToAttribute("UserIdentityAttribute"))
                                Config.Authentication.Shibboleth.UserIdentityAttribute = ReaderXml.Value.Trim();

                            if (ReaderXml.MoveToAttribute("URL"))
                                Config.Authentication.Shibboleth.ShibbolethURL = ReaderXml.Value.Trim();

                            if (ReaderXml.MoveToAttribute("Label"))
                                Config.Authentication.Shibboleth.Label = ReaderXml.Value.Trim();

                            if (ReaderXml.MoveToAttribute("Debug"))
                            {
                                if (String.Compare(ReaderXml.Value.Trim(), "true", StringComparison.OrdinalIgnoreCase) == 0)
                                    Config.Authentication.Shibboleth.Debug = true;
                            }

                            if (ReaderXml.MoveToAttribute("Enabled"))
                            {
                                if (String.Compare(ReaderXml.Value.Trim(), "false", StringComparison.OrdinalIgnoreCase) == 0)
                                    Config.Authentication.Shibboleth.Enabled = false;
                            }
                            ReaderXml.MoveToElement();
                            read_shibb_details(ReaderXml.ReadSubtree(), Config);

                            break;
                    }
                }
            }
        }

        private static void read_shibb_details(XmlReader ReaderXml, InstanceWide_Configuration Config)
        {
            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "mapping":
                            string serverVariable = null;
                            string userAttribute = null;
                            if (ReaderXml.MoveToAttribute("ServerVariable"))
                                serverVariable = ReaderXml.Value.Trim();
                            if (ReaderXml.MoveToAttribute("UserAttribute"))
                                userAttribute = ReaderXml.Value.Trim();
                            if ((!String.IsNullOrEmpty(serverVariable)) && (!String.IsNullOrEmpty(userAttribute)))
                            {
                                User_Object_Attribute_Mapping_Enum userAttrEnum = User_Object_Attribute_Mapping_Enum_Converter.ToEnum(userAttribute.ToUpper());
                                if (userAttrEnum != User_Object_Attribute_Mapping_Enum.NONE)
                                {
                                    Config.Authentication.Shibboleth.Add_Attribute_Mapping(serverVariable, userAttrEnum);
                                }
                            }
                            break;

                        case "constant":
                            string userAttribute2 = null;
                            string constantValue = null;
                            if (ReaderXml.MoveToAttribute("UserAttribute"))
                                userAttribute2 = ReaderXml.Value.Trim();
                            if (!ReaderXml.IsEmptyElement)
                            {
                                ReaderXml.Read();
                                constantValue = ReaderXml.Value.Trim();
                            }
                            if ((!String.IsNullOrEmpty(userAttribute2)) && (!String.IsNullOrEmpty(constantValue)))
                            {
                                User_Object_Attribute_Mapping_Enum userAttrEnum = User_Object_Attribute_Mapping_Enum_Converter.ToEnum(userAttribute2.ToUpper());
                                if (userAttrEnum != User_Object_Attribute_Mapping_Enum.NONE)
                                {
                                    Config.Authentication.Shibboleth.Add_Constant(userAttrEnum, constantValue);
                                }
                            }
                            break;

                        case "cansubmit":
                            string serverVariable2 = null;
                            string requiredValue = null;
                            if (ReaderXml.MoveToAttribute("ServerVariable"))
                                serverVariable2 = ReaderXml.Value.Trim();
                            if (ReaderXml.MoveToAttribute("Value"))
                                requiredValue = ReaderXml.Value.Trim();
                            if ((!String.IsNullOrEmpty(serverVariable2)) && (!String.IsNullOrEmpty(requiredValue)))
                            {
                                Config.Authentication.Shibboleth.Add_CanSubmit_Indicator(serverVariable2, requiredValue);
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #region Section reads all the OAI-PMH information

        private static void read_oai_details(XmlReader readerXml, InstanceWide_Configuration config)
        {
            bool baseSpecified = false;

            // Ensure the OAI-PMH object exists
            if (config.OAI_PMH == null)
                config.OAI_PMH = new OAI_PMH_Configuration();

            // Just step through the subtree of this
            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    switch (readerXml.Name.ToLower())
                    {
                        case "oai-pmh":
                            if (readerXml.Value.Trim().ToLower() == "false")
                                config.OAI_PMH.Enabled = false;
                            break;

                        case "repository":
                            if (readerXml.MoveToAttribute("IdentifierBase"))
                            {
                                baseSpecified = true;
                                config.OAI_PMH.Identifier_Base = readerXml.Value.Trim();
                            }
                            break;

                        case "identify":
                            read_oai_details_identify(readerXml.ReadSubtree(), config.OAI_PMH, baseSpecified);
                            break;

                        case "metadataprefixes":
                            read_oai_details_metadataPrefixes(readerXml.ReadSubtree(), config.OAI_PMH);
                            break;
                    }
                }
            }
        }

        private static void read_oai_details_identify(XmlReader readerXml, OAI_PMH_Configuration config, bool baseSpecified)
        {
            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    switch (readerXml.Name.ToLower())
                    {
                        case "name":
                            readerXml.Read();
                            config.Name = readerXml.Value.Trim();
                            break;

                        case "identifier":
                            readerXml.Read();
                            config.Identifier = readerXml.Value.Trim();
                            if (!baseSpecified)
                                config.Identifier_Base = "oai:" + config.Identifier.ToLower() + ":";
                            break;

                        case "adminemail":
                            readerXml.Read();
                            config.Add_Admin_Email(readerXml.Value.Trim());
                            break;

                        case "description":
                            readerXml.Read();
                            config.Add_Description(readerXml.Value.Trim());
                            break;

                    }
                }
            }
        }

        private static void read_oai_details_metadataPrefixes(XmlReader readerXml, OAI_PMH_Configuration config)
        {
            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    switch (readerXml.Name.ToLower())
                    {
                        case "clear":
                            config.Clear_Metadata_Prefixes();
                            break;

                        case "metadataformat":
                            OAI_PMH_Metadata_Format component = new OAI_PMH_Metadata_Format();
                            if (readerXml.MoveToAttribute("Prefix"))
                                component.Prefix = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("Schema"))
                                component.Schema = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("MetadataNamespace"))
                                component.MetadataNamespace = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("Assembly"))
                                component.Assembly = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("Namespace"))
                                component.Namespace = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("Class"))
                                component.Class = readerXml.Value.Trim();
                            if ((readerXml.MoveToAttribute("Enabled")) && (readerXml.Value.Trim().ToLower() == "false"))
                                component.Enabled = false;
                            if ((!String.IsNullOrEmpty(component.Prefix)) && (!String.IsNullOrEmpty(component.Schema)) && (!String.IsNullOrEmpty(component.MetadataNamespace)) && (!String.IsNullOrEmpty(component.Class)))
                            {
                                config.Add_Metadata_Prefix(component);
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #region Section reads all the contact form information

        private static void read_contactform_details(XmlReader readerXml, InstanceWide_Configuration config)
        {
            // Ensure the contact form configuration exists
            if (config.ContactForm == null)
                config.ContactForm = new ContactForm_Configuration();

            // Read the attributes
            if (readerXml.MoveToAttribute("Name"))
                config.ContactForm.Name = readerXml.Value.Trim();

            // Just step through the subtree of this
            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    switch (readerXml.Name.ToLower())
                    {
                        case "elements":
                            read_contactform_elements(readerXml.ReadSubtree(), config.ContactForm);
                            break;
                    }
                }
            }
        }

        private static void read_contactform_elements(XmlReader readerXml, ContactForm_Configuration config)
        {
            // Just step through the subtree of this
            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    switch (readerXml.Name.ToLower())
                    {
                        case "explanationtext":
                            read_contactform_element(readerXml, config, ContactForm_Configuration_Element_Type_Enum.ExplanationText);
                            break;

                        case "hiddenvalue":
                            read_contactform_element(readerXml, config, ContactForm_Configuration_Element_Type_Enum.HiddenValue);
                            break;

                        case "textbox":
                            read_contactform_element(readerXml, config, ContactForm_Configuration_Element_Type_Enum.TextBox);
                            break;

                        case "selectbox":
                            read_contactform_element(readerXml, config, ContactForm_Configuration_Element_Type_Enum.SelectBox);
                            break;

                        case "subject":
                            read_contactform_element(readerXml, config, ContactForm_Configuration_Element_Type_Enum.Subject);
                            break;

                        case "email":
                            read_contactform_element(readerXml, config, ContactForm_Configuration_Element_Type_Enum.Email);
                            break;

                        case "radioset":
                            read_contactform_element(readerXml, config, ContactForm_Configuration_Element_Type_Enum.RadioSet);
                            break;

                        case "checkboxset":
                            read_contactform_element(readerXml, config, ContactForm_Configuration_Element_Type_Enum.CheckBoxSet);
                            break;

                        case "textarea":
                            read_contactform_element(readerXml, config, ContactForm_Configuration_Element_Type_Enum.TextArea);
                            break;
                    }
                }
            }
        }

        private static void read_contactform_element(XmlReader readerXml, ContactForm_Configuration config, ContactForm_Configuration_Element_Type_Enum type)
        {
            // Create the element object
            ContactForm_Configuration_Element newElement = new ContactForm_Configuration_Element(type);

            // Read the attributes
            if (readerXml.MoveToAttribute("Name"))
            {
                newElement.Name = readerXml.Value.Trim();
                if (String.IsNullOrEmpty(newElement.QueryText.DefaultValue))
                    newElement.QueryText.DefaultValue = newElement.Name.Replace("_", " ") + ":";
            }
            if (readerXml.MoveToAttribute("CssClass"))
                newElement.CssClass = readerXml.Value.Trim();
            if (readerXml.MoveToAttribute("Query"))
                newElement.QueryText.DefaultValue = readerXml.Value.Trim();
            else if (readerXml.MoveToAttribute("Text"))
                newElement.QueryText.DefaultValue = readerXml.Value.Trim();
            if (readerXml.MoveToAttribute("UserAttribute"))
            {
                string attr = readerXml.Value.Trim();
                newElement.UserAttribute = User_Object_Attribute_Mapping_Enum_Converter.ToEnum(attr);
            }
            if (readerXml.MoveToAttribute("AlwaysShow"))
            {
                string alwaysShow = readerXml.Value.Trim();
                switch (alwaysShow.ToLower())
                {
                    case "false":
                        newElement.AlwaysShow = false;
                        break;

                    case "true":
                        newElement.AlwaysShow = true;
                        break;
                }
            }
            if (readerXml.MoveToAttribute("Required"))
            {
                string required = readerXml.Value.Trim();
                switch (required.ToLower())
                {
                    case "false":
                        newElement.Required = false;
                        break;

                    case "true":
                        newElement.Required = true;
                        break;
                }
            }

            readerXml.MoveToElement();

            // Just step through the subtree of this
            XmlReader subTreeReader = readerXml.ReadSubtree();
            while (subTreeReader.Read())
            {
                if (subTreeReader.NodeType == XmlNodeType.Element)
                {
                    switch (subTreeReader.Name.ToLower())
                    {
                        case "option":
                            if (!subTreeReader.IsEmptyElement)
                            {
                                subTreeReader.Read();
                                if (newElement.Options == null)
                                    newElement.Options = new List<string>();
                                newElement.Options.Add(subTreeReader.Value.Trim());
                            }
                            break;

                        case "language":
                            if (!subTreeReader.IsEmptyElement)
                            {
                                if (subTreeReader.MoveToAttribute("Code"))
                                {
                                    string language_code = subTreeReader.Value.Trim();
                                    Web_Language_Enum enum_lang = Web_Language_Enum_Converter.Code_To_Enum(language_code);
                                    if (enum_lang != Web_Language_Enum.UNDEFINED)
                                    {
                                        subTreeReader.Read();
                                        newElement.QueryText.Add_Translation(enum_lang, subTreeReader.Value.Trim());
                                    }
                                }
                            }
                            break;
                    }
                }
            }


            config.Add_Element(newElement);
        }

        #endregion

        #region Section reads all the brief item mapping information

        /// <summary> Read the configuration file for the brief item mapping sets </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        private static bool read_briefitem_mapping_details(XmlReader ReaderXml, InstanceWide_Configuration Config )
        {
            // Ensure the brief item mapping exists
            if (Config.BriefItemMapping == null)
                Config.BriefItemMapping = new BriefItemMapping_Configuration();

            // During this process, small objects ( IBriefItemMappers ) which contain no data
            // but implement the mapping method will be created.  This dictionary helps to ensure
            // each one is created only once.
            Dictionary<string, IBriefItemMapper> mappingObjDictionary = new Dictionary<string, IBriefItemMapper>();

            try
            {
                while (ReaderXml.Read())
                {
                    if (ReaderXml.NodeType == XmlNodeType.Element)
                    {
                        switch (ReaderXml.Name.ToLower())
                        {
                            case "mappingset":
                                // Get the ID for this mapping set
                                string id = String.Empty;
                                if (ReaderXml.MoveToAttribute("ID"))
                                    id = ReaderXml.Value.Trim();

                                // Was this indicated as the default set?
                                if (ReaderXml.MoveToAttribute("Default"))
                                {
                                    if (String.Compare(ReaderXml.Value, "true", StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        if (id.Length > 0)
                                            Config.BriefItemMapping.DefaultSetName = id;
                                        else
                                        {
                                            Config.BriefItemMapping.DefaultSetName = "DEFAULT";
                                            id = "DEFAULT";
                                        }
                                    }
                                }

                                BriefItemMapping_Set thisSet = Config.BriefItemMapping.GetMappingSet(id);
                                if (thisSet == null)
                                {
                                    thisSet = new BriefItemMapping_Set {SetName = id};
                                    Config.BriefItemMapping.Add_MappingSet(thisSet);
                                }

                                // Read the set here
                                ReaderXml.MoveToElement();
                                read_mappingset_details(ReaderXml.ReadSubtree(), thisSet, mappingObjDictionary);
                                break;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Config.Add_Log("EXCEPTION CAUGHT in Configuration_Files_Reader.read_briefitem_mapping_details");
                Config.Add_Log(ee.Message);
                Config.Add_Log(ee.StackTrace);

                Config.ErrorEncountered = true;
                return false;
            }

            return true;
        }

        private static void read_mappingset_details(XmlReader ReaderXml, BriefItemMapping_Set ReturnValue, Dictionary<string, IBriefItemMapper> MappingObjDictionary)
        {
            // Just step through the subtree of this
            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "mapper":
                            // Read all the data for this mapper class
                            string mapperAssembly = String.Empty;
                            string mapperClass = String.Empty;
                            if (ReaderXml.MoveToAttribute("Assembly"))
                                mapperAssembly = ReaderXml.Value.Trim();
                            if (ReaderXml.MoveToAttribute("Class"))
                                mapperClass = ReaderXml.Value.Trim();

                            // Was this enabled?
                            bool enabled = true;
                            if (ReaderXml.MoveToAttribute("Default"))
                            {
                                if (String.Compare(ReaderXml.Value, "false", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    enabled = false;
                                }
                            }

                            // Add this (if enabled) to the list of mappers
                            if (enabled)
                            {
                                string error;
                                IBriefItemMapper mapper = get_or_create_mapper(mapperAssembly, mapperClass, MappingObjDictionary, out error);

                                BriefItemMapping_Mapper mapperConfig = new BriefItemMapping_Mapper
                                {
                                    Assembly = mapperAssembly,
                                    Class = mapperClass,
                                    Enabled = true,
                                    MappingObject = mapper
                                };

                                ReturnValue.Mappings.Add(mapperConfig);
                            }


                            break;
                    }
                }
            }
        }

        private static IBriefItemMapper get_or_create_mapper(string MapperAssembly, string MapperClass, Dictionary<string, IBriefItemMapper> MappingObjDictionary, out string ErrorMessage)
        {
            ErrorMessage = String.Empty;

            // Was this already created (for a different mapping set)?
            if (MappingObjDictionary.ContainsKey(MapperAssembly + "." + MapperClass))
                return MappingObjDictionary[MapperAssembly + "." + MapperClass];

            // Look for the standard classes, just to avoid having to use reflection
            // for these that are built right into the system
            if (String.IsNullOrEmpty(MapperAssembly))
            {
                IBriefItemMapper thisModule = null;
                switch (MapperClass)
                {
                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Abstracts_BriefItemMapper":
                        thisModule = new Abstracts_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Affiliations_BriefItemMapper":
                        thisModule = new Affiliations_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Aggregations_BriefItemMapper":
                        thisModule = new Aggregations_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Classifications_BriefItemMapper":
                        thisModule = new Classifications_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Containers_BriefItemMapper":
                        thisModule = new Containers_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Dates_BriefItemMapper":
                        thisModule = new Dates_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Donor_BriefItemMapper":
                        thisModule = new Donor_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Edition_BriefItemMapper":
                        thisModule = new Edition_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Files_BriefItemMapper":
                        thisModule = new Files_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Frequency_BriefItemMapper":
                        thisModule = new Frequency_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Genres_BriefItemMapper":
                        thisModule = new Genres_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.GeoSpatial_BriefItemMapper":
                        thisModule = new GeoSpatial_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Holding_Location_BriefItemMapper":
                        thisModule = new Holding_Location_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Identifiers_BriefItemMapper":
                        thisModule = new Identifiers_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.InternalVarious_BriefItemMapper":
                        thisModule = new InternalVarious_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Languages_BriefItemMapper":
                        thisModule = new Languages_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.LearningObjectMetadata_BriefItemMapper":
                        thisModule = new LearningObjectMetadata_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Links_BriefItemMapper":
                        thisModule = new Links_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Manufacturers_BriefItemMapper":
                        thisModule = new Manufacturers_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Names_BriefItemMapper":
                        thisModule = new Names_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Notes_BriefItemMapper":
                        thisModule = new Notes_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Physical_Description_BriefItemMapper":
                        thisModule = new Physical_Description_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Publisher_BriefItemMapper":
                        thisModule = new Publisher_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Related_Items_BriefItemMapper":
                        thisModule = new Related_Items_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.ResourceType_BriefItemMapper":
                        thisModule = new ResourceType_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Rights_BriefItemMapper":
                        thisModule = new Rights_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Rights_MD_BriefItemMapper":
                        thisModule = new Rights_MD_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Source_Institution_BriefItemMapper":
                        thisModule = new Source_Institution_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Subjects_BriefItemMapper":
                        thisModule = new Subjects_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Target_Audience_BriefItemMapper":
                        thisModule = new Target_Audience_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Temporal_Coverage_BriefItemMapper":
                        thisModule = new Temporal_Coverage_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Thesis_Dissertation_BriefItemMapper":
                        thisModule = new Thesis_Dissertation_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Titles_BriefItemMapper":
                        thisModule = new Titles_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.User_Tags_BriefItemMapper":
                        thisModule = new User_Tags_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.VRACore_BriefItemMapper":
                        thisModule = new VRACore_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Zoological_Taxonomy_BriefItemMapper":
                        thisModule = new Zoological_Taxonomy_BriefItemMapper();
                        break;
                }

                // Was this a match?
                if (thisModule != null)
                {
                    // Add to the dictionary to avoid looking this up again
                    MappingObjDictionary[MapperAssembly + "." + MapperClass] = thisModule;

                    // Return this standard IBriefItemMapper
                    return thisModule;
                }
            }

            // Try to retrieve this from the assembly using reflection
            object itemAsObj = Get_Mapper(MapperAssembly, MapperClass, out ErrorMessage);
            if ((itemAsObj == null) && (ErrorMessage.Length > 0))
            {
                return null;
            }


            // Ensure this implements the IBriefItemMapper class 
            IBriefItemMapper itemAsItem = itemAsObj as IBriefItemMapper;
            if (itemAsItem == null)
            {
                ErrorMessage = MapperClass + " loaded from assembly but does not implement the IBriefItemMapper interface!";
                return null;
            }


            // Add to the dictionary to avoid looking this up again
            MappingObjDictionary[MapperAssembly + "." + MapperClass] = itemAsItem;

            // Return this custom IBriefItemMapper
            return itemAsItem;
        }

        private static object Get_Mapper(string MapperAssembly, string MapperClass, out string ErrorMessage)
        {
            ErrorMessage = String.Empty;

            try
            {
                // Using reflection, create an object from the class namespace/name 
                Assembly dllAssembly = Assembly.GetExecutingAssembly();
                if (!String.IsNullOrEmpty(MapperAssembly))
                {
                    dllAssembly = Assembly.LoadFrom(MapperAssembly);
                }

                Type readerWriterType = dllAssembly.GetType(MapperClass);
                return Activator.CreateInstance(readerWriterType);
            }
            catch (Exception ee)
            {
                ErrorMessage = "Unable to load class from assembly. ( " + MapperClass + " ) : " + ee.Message;
                return null;
            }
        }

        #endregion

        #region Section reads all the map editor setting information

        private static void read_mapeditor_details(XmlReader ReaderXml, InstanceWide_Configuration Config)
        {
            // Ensure the config object exists
            if ( Config.UI.MapEditor == null )
                Config.UI.MapEditor = new MapEditor_Configuration();
            

            while (ReaderXml.Read())
            {
                // Only detect start elements.
                if (ReaderXml.IsStartElement())
                {
                    // Get element name and switch on it.
                    switch (ReaderXml.Name)
                    {
                        case "collection":
                            string collectionName = ReaderXml["id"];
                            MapEditor_Configuration_Collection collection = new MapEditor_Configuration_Collection
                            {
                                Name = collectionName
                            };

                            while (ReaderXml.Read())
                            {
                                if (ReaderXml.NodeType == XmlNodeType.Whitespace) continue;

                                if (ReaderXml.Name == "collection")
                                {
                                    if (ReaderXml.NodeType == XmlNodeType.EndElement)
                                        break;
                                }

                                if (!ReaderXml.IsStartElement()) continue;

                                string key = ReaderXml.Name;
                                if (ReaderXml.Read())
                                {
                                    string value = String.IsNullOrEmpty(ReaderXml.Value) ? "\"\"" : ReaderXml.Value;
                                    collection.Settings.Add(new Simple_Setting(key, value, -1));
                                }
                            }
                            Config.UI.MapEditor.Collections.Add(collection);
                            break;
                    }
                }
            }
        }

        #endregion

        #region Section reads all the engine endpoint information

        private static void read_engine_details(XmlReader ReaderXml, InstanceWide_Configuration Config)
        {
            // Ensure the config object exists
            if (Config.Engine == null)
                Config.Engine = new Engine_Server_Configuration();


            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "mapping":
                            read_microservices_details_mapping(ReaderXml.ReadSubtree(), Config.Engine, null );
                            break;

                        case "components":
                            read_microservices_details_components(ReaderXml.ReadSubtree(), Config.Engine );
                            break;

                        case "restrictionranges":
                            read_engine_details_restrictionranges(ReaderXml.ReadSubtree(), Config.Engine );
                            break;
                    }
                }
            }
        }

        private static void read_microservices_details_mapping(XmlReader ReaderXml, Engine_Server_Configuration Config, Engine_Path_Endpoint ParentSegment )
        {
            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "removeall":
                            if (ParentSegment != null)
                            {
                                if (ParentSegment.Children != null)
                                    ParentSegment.Children.Clear();
                            }
                            else
                            {
                                Config.RootPaths.Clear();
                            }
                            break;

                        case "path":
                            if (ReaderXml.MoveToAttribute("Segment"))
                            {
                                Engine_Path_Endpoint path;
                                string segment = ReaderXml.Value.Trim();

                                if (ParentSegment == null)
                                {
                                    if (Config.ContainsRootKey(segment.ToLower()))
                                        path = Config.GetRoot(segment.ToLower());
                                    else
                                    {
                                        path = new Engine_Path_Endpoint {Segment = segment};
                                        Config.AddRoot(segment.ToLower(), path);
                                    }
                                }
                                else
                                {
                                    if (ParentSegment.ContainsChildKey(segment.ToLower()))
                                    {
                                        path = ParentSegment.GetChild(segment.ToLower());
                                    }
                                    else
                                    {
                                        path = new Engine_Path_Endpoint { Segment = segment };
                                        ParentSegment.AddChild(path.Segment, path );
                                    }

                                }

                                ReaderXml.MoveToElement();
                                XmlReader subTreeReader = ReaderXml.ReadSubtree();
                                subTreeReader.Read();
                                read_microservices_details_mapping(subTreeReader, Config, path);
                            }
                            break;

                        case "complexendpoint":
                            // Read the top-endpoint information, before getting to each verb mapping
                            bool disabled_at_top = false;
                            Engine_Path_Endpoint endpoint = new Engine_Path_Endpoint { IsEndpoint = true };
                            if (ReaderXml.MoveToAttribute("Segment"))
                                endpoint.Segment = ReaderXml.Value.Trim();
                            if ((ReaderXml.MoveToAttribute("Enabled")) && (String.Compare(ReaderXml.Value.Trim(), "false", StringComparison.OrdinalIgnoreCase) == 0))
                                disabled_at_top = true;

                            // Now, read what remains
                            ReaderXml.MoveToElement();
                            XmlReader complexReader = ReaderXml.ReadSubtree();
                            complexReader.Read();
                            read_microservices_complex_endpoint_details(complexReader, endpoint, disabled_at_top);

                            // If a verb was mapped and there was a valid segment, add this
                            if ((!String.IsNullOrEmpty(endpoint.Segment)) && (endpoint.HasVerbMapping))
                            {
                                if (ParentSegment != null)
                                {
                                    // Add this endpoint
                                    ParentSegment.AddChild(endpoint.Segment, endpoint);
                                }
                            }
                            break;

                        case "endpoint":
                            read_microservices_simple_endpoint_details(ReaderXml, ParentSegment);
                            break;
                    }
                }
            }
        }

        private static void read_microservices_complex_endpoint_details(XmlReader ReaderXml, Engine_Path_Endpoint Endpoint, bool DisabledAtTop)
        {
            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    if (String.Compare(ReaderXml.Name, "verbmapping", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // Ensure verb is indicated first
                        if (ReaderXml.MoveToAttribute("Verb"))
                        {
                            Microservice_Endpoint_RequestType_Enum verb = Microservice_Endpoint_RequestType_Enum.ERROR;
                            switch (ReaderXml.Value.Trim().ToUpper())
                            {
                                case "DELETE":
                                    verb = Microservice_Endpoint_RequestType_Enum.DELETE;
                                    break;

                                case "GET":
                                    verb = Microservice_Endpoint_RequestType_Enum.GET;
                                    break;

                                case "POST":
                                    verb = Microservice_Endpoint_RequestType_Enum.POST;
                                    break;

                                case "PUT":
                                    verb = Microservice_Endpoint_RequestType_Enum.PUT;
                                    break;
                            }

                            // If a valid verb found, continue
                            if (verb != Microservice_Endpoint_RequestType_Enum.ERROR)
                            {
                                // Build the verb mapping
                                Engine_VerbMapping verbMapping = new Engine_VerbMapping(null, !DisabledAtTop, Microservice_Endpoint_Protocol_Enum.JSON, verb);
                                if (ReaderXml.MoveToAttribute("Method"))
                                    verbMapping.Method = ReaderXml.Value.Trim();
                                if ((!DisabledAtTop) && (ReaderXml.MoveToAttribute("Enabled")) && (String.Compare(ReaderXml.Value.Trim(), "false", StringComparison.OrdinalIgnoreCase) == 0))
                                    verbMapping.Enabled = false;
                                if (ReaderXml.MoveToAttribute("Protocol"))
                                {
                                    switch (ReaderXml.Value.Trim().ToUpper())
                                    {
                                        case "JSON":
                                            verbMapping.Protocol = Microservice_Endpoint_Protocol_Enum.JSON;
                                            break;

                                        case "JSON-P":
                                            verbMapping.Protocol = Microservice_Endpoint_Protocol_Enum.JSON_P;
                                            break;

                                        case "PROTOBUF":
                                            verbMapping.Protocol = Microservice_Endpoint_Protocol_Enum.PROTOBUF;
                                            break;

                                        case "SOAP":
                                            verbMapping.Protocol = Microservice_Endpoint_Protocol_Enum.SOAP;
                                            break;

                                        case "TEXT":
                                            verbMapping.Protocol = Microservice_Endpoint_Protocol_Enum.TEXT;
                                            break;

                                        case "XML":
                                            verbMapping.Protocol = Microservice_Endpoint_Protocol_Enum.XML;
                                            break;

                                        default:
                                            verbMapping.Protocol = Microservice_Endpoint_Protocol_Enum.JSON;
                                            break;
                                    }
                                }

                                // Get the mapping to componentid and restriction id
                                if (ReaderXml.MoveToAttribute("ComponentID"))
                                    verbMapping.ComponentId = ReaderXml.Value.Trim();
                                if (ReaderXml.MoveToAttribute("RestrictionRangeID"))
                                {
                                    verbMapping.RestrictionRangeSetId = ReaderXml.Value.Trim();
                                }


                                // If valid, add to this endpoint
                                if ((!String.IsNullOrEmpty(verbMapping.ComponentId)) && (!String.IsNullOrEmpty(verbMapping.Method)))
                                {
                                    // Add the verb mapping to the right spot
                                    switch (verbMapping.RequestType)
                                    {
                                        case Microservice_Endpoint_RequestType_Enum.DELETE:
                                            Endpoint.DeleteMapping = verbMapping;
                                            break;

                                        case Microservice_Endpoint_RequestType_Enum.GET:
                                            Endpoint.GetMapping = verbMapping;
                                            break;

                                        case Microservice_Endpoint_RequestType_Enum.POST:
                                            Endpoint.PostMapping = verbMapping;
                                            break;

                                        case Microservice_Endpoint_RequestType_Enum.PUT:
                                            Endpoint.PutMapping = verbMapping;
                                            break;

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void read_microservices_simple_endpoint_details(XmlReader ReaderXml, Engine_Path_Endpoint ParentSegment)
        {
            Engine_Path_Endpoint endpoint = new Engine_Path_Endpoint {IsEndpoint = true};

            string componentid = String.Empty;
            string restrictionid = String.Empty;
            string method = String.Empty;
            bool enabled = true;
            Microservice_Endpoint_Protocol_Enum protocol = Microservice_Endpoint_Protocol_Enum.JSON;

            if (ReaderXml.MoveToAttribute("Segment"))
                endpoint.Segment = ReaderXml.Value.Trim();
            if (ReaderXml.MoveToAttribute("ComponentID"))
                componentid = ReaderXml.Value.Trim();
            if (ReaderXml.MoveToAttribute("Method"))
                method = ReaderXml.Value.Trim();
            if (ReaderXml.MoveToAttribute("Enabled"))
            {
                if (String.Compare(ReaderXml.Value.Trim(), "false", StringComparison.OrdinalIgnoreCase) == 0)
                    enabled = false;
            }
            if (ReaderXml.MoveToAttribute("RestrictionRangeID"))
                restrictionid = ReaderXml.Value.Trim();
            if (ReaderXml.MoveToAttribute("Protocol"))
            {
                switch (ReaderXml.Value.Trim().ToUpper())
                {
                    case "JSON":
                        protocol = Microservice_Endpoint_Protocol_Enum.JSON;
                        break;

                    case "JSON-P":
                        protocol = Microservice_Endpoint_Protocol_Enum.JSON_P;
                        break;

                    case "PROTOBUF":
                        protocol = Microservice_Endpoint_Protocol_Enum.PROTOBUF;
                        break;

                    case "SOAP":
                        protocol = Microservice_Endpoint_Protocol_Enum.SOAP;
                        break;

                    case "XML":
                        protocol = Microservice_Endpoint_Protocol_Enum.XML;
                        break;

                    case "TEXT":
                        protocol = Microservice_Endpoint_Protocol_Enum.TEXT;
                        break;

                    default:
                        protocol = Microservice_Endpoint_Protocol_Enum.JSON;
                        break;
                }
            }

            ReaderXml.MoveToElement();

            if ((componentid.Length > 0) && (endpoint.Segment.Length > 0) && (method.Length > 0))
            {
                if (ParentSegment != null)
                {
                    // Add this endpoint
                    ParentSegment.AddChild(endpoint.Segment, endpoint);

                    // Add the verb mapping defaulted to GET
                    endpoint.GetMapping = new Engine_VerbMapping(method, enabled, protocol, Microservice_Endpoint_RequestType_Enum.GET, componentid, restrictionid);
                }
            }
        }

        private static void read_microservices_details_components(XmlReader ReaderXml, Engine_Server_Configuration Config )
        {
            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "component":
                            string Namespace = String.Empty;
                            Engine_Component component = new Engine_Component();
                            if (ReaderXml.MoveToAttribute("ID"))
                                component.ID = ReaderXml.Value.Trim();
                            if (ReaderXml.MoveToAttribute("Assembly"))
                                component.Assembly = ReaderXml.Value.Trim();
                            if (ReaderXml.MoveToAttribute("Namespace"))
                                Namespace = ReaderXml.Value.Trim() + ".";
                            if (ReaderXml.MoveToAttribute("Class"))
                                component.Class = Namespace + ReaderXml.Value.Trim();
                            if ((!String.IsNullOrEmpty(component.ID)) && (!String.IsNullOrEmpty(component.Class)))
                            {
                                // If the key already existed, remove the old one as it will be replaced
                                Config.Components.Add(component);

                            }
                            break;
                    }
                }
            }
        }

        private static void read_engine_details_restrictionranges(XmlReader ReaderXml, Engine_Server_Configuration Config )
        {
            Engine_RestrictionRange currentRange = null;

            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "range":
                            string rangeId = null;
                            if (ReaderXml.MoveToAttribute("ID"))
                                rangeId = ReaderXml.Value.Trim();

                            // Must have an ID to be valid
                            if (!String.IsNullOrEmpty(rangeId))
                            {
                                currentRange = null;

                                // Look for a matching range
                                foreach (Engine_RestrictionRange range in Config.RestrictionRanges)
                                {
                                    if (range.ID == rangeId)
                                    {
                                        currentRange = range;
                                        break;
                                    }
                                }

                                // If no range, create the new one
                                if (currentRange == null)
                                {
                                    currentRange = new Engine_RestrictionRange { ID = rangeId };
                                }

                                if (ReaderXml.MoveToAttribute("Label"))
                                    currentRange.Label = ReaderXml.Value.Trim();
                            }
                            else
                            {
                                // Missing ID in this range
                                currentRange = null;
                            }
                            break;

                        case "removeall":
                            if (currentRange != null)
                                currentRange.IpRanges.Clear();
                            break;

                        case "iprange":
                            if (currentRange != null)
                            {
                                Engine_IpRange singleIpRange = new Engine_IpRange();
                                if (ReaderXml.MoveToAttribute("Label"))
                                    singleIpRange.Label = ReaderXml.Value.Trim();
                                if (ReaderXml.MoveToAttribute("Start"))
                                    singleIpRange.StartIp = ReaderXml.Value.Trim();
                                if (ReaderXml.MoveToAttribute("End"))
                                    singleIpRange.EndIp = ReaderXml.Value.Trim();
                                if (singleIpRange.StartIp.Length > 0)
                                    currentRange.IpRanges.Add(singleIpRange);
                            }
                            break;
                    }

                }
                else if (ReaderXml.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Compare(ReaderXml.Name, "range", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if ((currentRange != null) && (!String.IsNullOrEmpty(currentRange.ID)))
                        {
                            if (!Config.RestrictionRanges.Contains(currentRange))
                                Config.RestrictionRanges.Add(currentRange);
                        }
                        currentRange = null;
                    }
                }
            }
        }

        #endregion

        #region Section finalizes the engine endpoint information

        private static void engine_config_finalize(InstanceWide_Configuration Configuration)
        {
            // If there was an error or nothing was read, do nothing
            if ((Configuration.Engine == null) || (Configuration.Engine.RootPaths == null) || (Configuration.Engine.RootPaths.Count == 0))
                return;

            // Build the dictionaries for all the components
            Dictionary<string, Engine_Component> components = new Dictionary<string, Engine_Component>(StringComparer.OrdinalIgnoreCase);
            foreach (Engine_Component thisComponent in Configuration.Engine.Components)
            {
                components[thisComponent.ID] = thisComponent;
            }

            // Ensure the current IP address for this server is added to any
            // restriction range.. should always be able to get to the range from the
            // local machine
            List<string> local_ips = get_local_ip_addresses();
            foreach (string localIp in local_ips)
            {
                foreach (Engine_RestrictionRange thisRange in Configuration.Engine.RestrictionRanges)
                {
                    thisRange.Add_IP_Range("Web server IP (added automatically)", localIp);
                }
            }

            // Build the dictionaries for all the restriction ranges
            Dictionary<string, Engine_RestrictionRange> restrictionRanges = new Dictionary<string, Engine_RestrictionRange>(StringComparer.OrdinalIgnoreCase);
            foreach (Engine_RestrictionRange thisRange in Configuration.Engine.RestrictionRanges)
            {
                restrictionRanges[thisRange.ID] = thisRange;
            }

            // Now, iterate through the tree of child pages
            foreach (Engine_Path_Endpoint pathEndpoint in Configuration.Engine.RootPaths)
            {
                engine_config_finalize_recurse_through_endpoints(pathEndpoint, components, restrictionRanges);
            }
        }

        private static List<string> get_local_ip_addresses()
        {
            List<string> returnValue = new List<string>();

            try
            {
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    foreach (var uipi in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (uipi.Address.AddressFamily != AddressFamily.InterNetwork) continue;

                        if ((uipi.IPv4Mask == null) || (uipi.Address.ToString() == "127.0.0.1")) continue; //ignore 127.0.0.1
                        returnValue.Add(uipi.Address.ToString());
                    }
                }
            }
            catch 
            {
                
            }

            return returnValue;
        }

        private static void engine_config_finalize_recurse_through_endpoints(Engine_Path_Endpoint pathEndpoint, Dictionary<string, Engine_Component> components, Dictionary<string, Engine_RestrictionRange> restrictionRanges)
        {
            // Handle this one first
            if (pathEndpoint.HasVerbMapping)
            {
                // Do the individual mappings
                engine_config_finalize_handle_single_mapping(pathEndpoint.GetMapping, components, restrictionRanges);
                engine_config_finalize_handle_single_mapping(pathEndpoint.PostMapping, components, restrictionRanges);
                engine_config_finalize_handle_single_mapping(pathEndpoint.PutMapping, components, restrictionRanges);
                engine_config_finalize_handle_single_mapping(pathEndpoint.DeleteMapping, components, restrictionRanges);
            }
            
            // Now, step through any children as well
            if ((pathEndpoint.Children != null) && (pathEndpoint.Children.Count > 0))
            {
                foreach (Engine_Path_Endpoint childNode in pathEndpoint.Children)
                {
                    engine_config_finalize_recurse_through_endpoints(childNode, components, restrictionRanges);
                }
            }

        }

        private static void engine_config_finalize_handle_single_mapping(Engine_VerbMapping mapping, Dictionary<string, Engine_Component> components, Dictionary<string, Engine_RestrictionRange> restrictionRanges)
        {
            // If the mapping didn't exist, do nothing
            if (mapping == null)
                return;

            // Map the component
            if ((!String.IsNullOrEmpty(mapping.ComponentId)) && (components.ContainsKey(mapping.ComponentId)))
            {
                mapping.Component = components[mapping.ComponentId];
            }
            else
            {
                mapping.Component = null;
            }

            // Map any restriction range
            if (!String.IsNullOrEmpty(mapping.RestrictionRangeSetId)) 
            {
                string[] restrictions = mapping.RestrictionRangeSetId.Split(" |,".ToCharArray());
                foreach (string thisRestriction in restrictions)
                {
                    if (restrictionRanges.ContainsKey(thisRestriction.Trim()))
                    {
                        mapping.Add_RestrictionRange(restrictionRanges[thisRestriction.Trim()]);
                    }
                }
            }
            else
            {
                mapping.RestrictionRanges = null;
            }
        }

        #endregion

        #region Section reads all the quality control tool information

        private static void read_quality_control_details(XmlReader ReaderXml, InstanceWide_Configuration Config)
        {
            // Ensure the config object exists
            if (Config.QualityControlTool == null)
                Config.QualityControlTool = new QualityControl_Configuration();


            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "profiles":
                            read_qc_profiles(ReaderXml.ReadSubtree(), Config.QualityControlTool);
                            break;
                    }
                }
            }
        }

        private static void read_qc_profiles(XmlReader ReaderXml, QualityControl_Configuration Config )
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

                            QualityControl_Division_Config thisConfig = new QualityControl_Division_Config();
                            while (child_readerXml.Read())
                            {
                                if (child_readerXml.NodeType == XmlNodeType.Element && child_readerXml.Name.ToLower() == "divisiontype")
                                {
                                    thisConfig = new QualityControl_Division_Config();

                                    if (child_readerXml.MoveToAttribute("type"))
                                    {
                                        thisConfig.TypeName = child_readerXml.Value;
                                    }
                                    if (child_readerXml.MoveToAttribute("isNameable"))
                                        thisConfig.isNameable = Convert.ToBoolean(child_readerXml.Value);
                                    if (child_readerXml.MoveToAttribute("base"))
                                    {
                                        string baseType = child_readerXml.Value;
                                        if (!String.Equals(baseType, thisConfig.TypeName, StringComparison.OrdinalIgnoreCase))
                                        {
                                            thisConfig.BaseTypeName = baseType;
                                        }
                                    }
                                    profile.Add_Division_Type(thisConfig);
                                }
                                else if (child_readerXml.NodeType == XmlNodeType.Element && child_readerXml.Name.ToLower() == "translation")
                                {
                                    if (thisConfig != null)
                                    {
                                        string language = (child_readerXml.MoveToAttribute("language")) ? child_readerXml.Value : String.Empty;
                                        string text = (child_readerXml.MoveToAttribute("text")) ? child_readerXml.Value : String.Empty;

                                        if ((!String.IsNullOrEmpty(language)) && (!String.IsNullOrEmpty(text)))
                                        {
                                            thisConfig.Add_Translation(Web_Language_Enum_Converter.Code_To_Enum(language), text );
                                        }
                                    }
                                }
                            }

                            Config.Add_Profile(profile);
                            break;

                    }
                }
            }
        }

        #endregion

        #region Section reads all the metadata configuration information

        private static bool read_metadata_details(XmlReader ReaderXml, InstanceWide_Configuration Config)
        {
            bool errorEncountered = false;
                
            // Ensure the config object exists
            if (Config.Metadata == null)
                Config.Metadata = new Metadata_Configuration();

            // Clear al the current (probably default) settings
            if ( Config.Metadata.isDefault )
                Config.Metadata.Clear();

            // Some collections to read into
            Dictionary<string, METS_Section_ReaderWriter_Config> readerWriters = new Dictionary<string, METS_Section_ReaderWriter_Config>();
            if ((Config.Metadata.METS_Section_File_ReaderWriter_Configs != null) && (Config.Metadata.METS_Section_File_ReaderWriter_Configs.Count > 0))
            {
                foreach (METS_Section_ReaderWriter_Config currentConfig in Config.Metadata.METS_Section_File_ReaderWriter_Configs)
                {
                    readerWriters[currentConfig.ID] = currentConfig;
                }
            }

            try
            {
                while (ReaderXml.Read())
                {
                    if (ReaderXml.NodeType == XmlNodeType.Element)
                    {
                        switch (ReaderXml.Name.ToLower())
                        {
                            case "clearall":
                                Config.Metadata.Clear();
                                break;

                            case "metadata_file_readerwriters":
                                read_metadata_file_readerwriter_configs(ReaderXml.ReadSubtree(), Config.Metadata);
                                break;

                            case "mets_sec_readerwriters":
                                read_mets_readerwriter_configs(ReaderXml.ReadSubtree(), readerWriters, Config.Metadata);
                                break;

                            case "mets_writing":
                                read_mets_writing_config(ReaderXml.ReadSubtree(), readerWriters, Config.Metadata);
                                break;

                            case "additional_metadata_modules":
                                read_metadata_modules_config(ReaderXml.ReadSubtree(), Config.Metadata);
                                break;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Config.Add_Log("EXCEPTION CAUGHT in Configuration_Files_Reader.read_metadata_details");
                Config.Add_Log(ee.Message);
                Config.Add_Log(ee.StackTrace);

                Config.ErrorEncountered = true;
                errorEncountered = true;
            }

            // If there was an error while reading, use the system defaults
            if (errorEncountered)
            {
                Config.Metadata.Clear();
                Config.Metadata.Set_Default_Values();
            }

            return !errorEncountered;
        }

        private static void read_metadata_file_readerwriter_configs(XmlReader ReaderXml, Metadata_Configuration Config)
        {
            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    if (ReaderXml.Name.ToLower() == "clear")
                        Config.Clear_Metadata_File_ReaderWriter_Config();
                    
                    if (ReaderXml.Name.ToLower() == "readerwriter")
                        read_metadata_file_readerwriter_config(ReaderXml.ReadSubtree(), Config );
                }
            }
        }

        private static void read_metadata_file_readerwriter_config(XmlReader ReaderXml, Metadata_Configuration Config )
        {
            Metadata_File_ReaderWriter_Config returnValue = new Metadata_File_ReaderWriter_Config();
            ReaderXml.Read();

            // Move to and save the basic attributes
            if (ReaderXml.MoveToAttribute("mdtype"))
            {
                switch (ReaderXml.Value.ToUpper())
                {
                    case "EAD":
                        returnValue.MD_Type = Metadata_File_Type_Enum.EAD;
                        break;

                    case "DC":
                        returnValue.MD_Type = Metadata_File_Type_Enum.DC;
                        break;

                    case "MARC21":
                        returnValue.MD_Type = Metadata_File_Type_Enum.MARC21;
                        break;

                    case "MARCXML":
                        returnValue.MD_Type = Metadata_File_Type_Enum.MARCXML;
                        break;

                    case "METS":
                        returnValue.MD_Type = Metadata_File_Type_Enum.METS;
                        break;

                    case "MODS":
                        returnValue.MD_Type = Metadata_File_Type_Enum.MODS;
                        break;

                    case "OTHER":
                        returnValue.MD_Type = Metadata_File_Type_Enum.OTHER;
                        if (ReaderXml.MoveToAttribute("othermdtype"))
                            returnValue.Other_MD_Type = ReaderXml.Value;
                        break;
                }
            }

            if (ReaderXml.MoveToAttribute("label"))
                returnValue.Label = ReaderXml.Value;
            if (ReaderXml.MoveToAttribute("namespace"))
                returnValue.Code_Namespace = ReaderXml.Value;
            if (ReaderXml.MoveToAttribute("class"))
                returnValue.Code_Class = ReaderXml.Value;
            if (ReaderXml.MoveToAttribute("assembly"))
                returnValue.Code_Assembly = ReaderXml.Value;
            if ((ReaderXml.MoveToAttribute("canRead")) && (ReaderXml.Value.ToLower() == "false"))
            {
                returnValue.canRead = false;
            }
            if ((ReaderXml.MoveToAttribute("canWrite")) && (ReaderXml.Value.ToLower() == "false"))
            {
                returnValue.canWrite = false;
            }

            while (ReaderXml.Read())
            {
                if ((ReaderXml.NodeType == XmlNodeType.Element) && (ReaderXml.Name.ToLower() == "option"))
                {
                    string key = String.Empty;
                    string value = String.Empty;
                    if (ReaderXml.MoveToAttribute("key"))
                        key = ReaderXml.Value;
                    if (ReaderXml.MoveToAttribute("value"))
                        value = ReaderXml.Value;
                    if ((key.Length > 0) && (value.Length > 0))
                        returnValue.Add_Option(key, value);
                }
            }

            Config.Add_Metadata_File_ReaderWriter(returnValue);
        }

        private static void read_mets_readerwriter_configs(XmlReader ReaderXml, Dictionary<string, METS_Section_ReaderWriter_Config> ReaderWriters, Metadata_Configuration Config )
        {
            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    if (ReaderXml.Name.ToLower() == "readerwriter")
                    {
                        METS_Section_ReaderWriter_Config singleReaderWriter = read_mets_section_readerwriter_config(ReaderXml.ReadSubtree());
                        ReaderWriters.Add(singleReaderWriter.ID.ToUpper(), singleReaderWriter);
                        Config.Add_METS_Section_ReaderWriter(singleReaderWriter);
                    }
                }
            }
        }

        private static METS_Section_ReaderWriter_Config read_mets_section_readerwriter_config(XmlReader ReaderXml )
        {
            METS_Section_ReaderWriter_Config returnValue = new METS_Section_ReaderWriter_Config();

            ReaderXml.Read();

            // Move to and save the basic attributes
            if (ReaderXml.MoveToAttribute("ID"))
                returnValue.ID = ReaderXml.Value;
            if (ReaderXml.MoveToAttribute("label"))
                returnValue.Label = ReaderXml.Value;
            if (ReaderXml.MoveToAttribute("namespace"))
                returnValue.Code_Namespace = ReaderXml.Value;
            if (ReaderXml.MoveToAttribute("class"))
                returnValue.Code_Class = ReaderXml.Value;
            if (ReaderXml.MoveToAttribute("assembly"))
                returnValue.Code_Assembly = ReaderXml.Value;
            if ((ReaderXml.MoveToAttribute("isActive")) && (ReaderXml.Value.ToLower() == "false"))
            {
                returnValue.isActive = false;
            }
            if (ReaderXml.MoveToAttribute("section"))
            {
                switch (ReaderXml.Value.ToLower())
                {
                    case "amdsec":
                        returnValue.METS_Section = METS_Section_Type_Enum.AmdSec;
                        if (ReaderXml.MoveToAttribute("amdSecType"))
                        {
                            switch (ReaderXml.Value.ToLower())
                            {
                                case "techmd":
                                    returnValue.AmdSecType = METS_amdSec_Type_Enum.TechMD;
                                    break;

                                case "rightsmd":
                                    returnValue.AmdSecType = METS_amdSec_Type_Enum.RightsMD;
                                    break;

                                case "digiprovmd":
                                    returnValue.AmdSecType = METS_amdSec_Type_Enum.DigiProvMD;
                                    break;

                                case "sourcemd":
                                    returnValue.AmdSecType = METS_amdSec_Type_Enum.SourceMD;
                                    break;

                            }
                        }
                        break;

                    case "dmdsec":
                        returnValue.METS_Section = METS_Section_Type_Enum.DmdSec;
                        break;
                }
            }

            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "mapping":
                            METS_Section_ReaderWriter_Mapping newMapping = new METS_Section_ReaderWriter_Mapping();
                            if (ReaderXml.MoveToAttribute("mdtype"))
                                newMapping.MD_Type = ReaderXml.Value;
                            if (ReaderXml.MoveToAttribute("othermdtype"))
                                newMapping.Other_MD_Type = ReaderXml.Value;
                            if (ReaderXml.MoveToAttribute("label"))
                                newMapping.Label = ReaderXml.Value;
                            if ((ReaderXml.MoveToAttribute("isDefault")) && (ReaderXml.Value.ToLower() == "true"))
                                newMapping.isDefault = true;
                            returnValue.Add_Mapping(newMapping);
                            break;

                        case "option":
                            string key = String.Empty;
                            string value = String.Empty;
                            if (ReaderXml.MoveToAttribute("key"))
                                key = ReaderXml.Value;
                            if (ReaderXml.MoveToAttribute("value"))
                                value = ReaderXml.Value;
                            if ((key.Length > 0) && (value.Length > 0))
                                returnValue.Add_Option(key, value);
                            break;
                    }
                }
            }

            return returnValue;
        }

        private static void read_metadata_modules_config(XmlReader ReaderXml, Metadata_Configuration Config )
        {
            while (ReaderXml.Read())
            {
                if ((ReaderXml.NodeType == XmlNodeType.Element) && (ReaderXml.Name.ToLower() == "metadatamodule"))
                {
                    // read all the values
                    Additional_Metadata_Module_Config module = new Additional_Metadata_Module_Config();
                    if (ReaderXml.MoveToAttribute("key"))
                        module.Key = ReaderXml.Value.Trim();
                    if (ReaderXml.MoveToAttribute("assembly"))
                        module.Code_Assembly = ReaderXml.Value;
                    if (ReaderXml.MoveToAttribute("namespace"))
                        module.Code_Namespace = ReaderXml.Value;
                    if (ReaderXml.MoveToAttribute("class"))
                        module.Code_Class = ReaderXml.Value;

                    // Only add if valid
                    if ((module.Key.Length > 0) && (module.Code_Class.Length > 0) && (module.Code_Namespace.Length > 0))
                    {
                        Config.Add_Metadata_Module_Config(module);
                    }
                }
            }
        }

        private static void read_mets_writing_config(XmlReader ReaderXml, Dictionary<string, METS_Section_ReaderWriter_Config> ReaderWriters, Metadata_Configuration Config )
        {
            bool inPackage = false;
            bool inDivision = false;
            bool inFile = false;
            bool inDmdSec = true;
            METS_Writing_Profile profile = null;
            int unnamed_profile_counter = 1;

            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "profile":
                            profile = new METS_Writing_Profile();
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
                            // Does this profile already exist?
                            METS_Writing_Profile existingProfile = Config.Get_Writing_Profile(profile.Profile_Name);
                            if (existingProfile != null)
                            {
                                profile = existingProfile;

                            }
                            else
                            {
                                Config.Add_METS_Writing_Profile(profile); 
                            }

                            break;

                        case "package_scope":
                            inPackage = true;
                            inDivision = false;
                            inFile = false;
                            break;

                        case "division_scope":
                            inPackage = false;
                            inDivision = true;
                            inFile = false;
                            break;

                        case "file_scope":
                            inPackage = false;
                            inDivision = false;
                            inFile = true;
                            break;

                        case "dmdsec":
                            inDmdSec = true;
                            break;

                        case "amdsec":
                            inDmdSec = false;
                            break;

                        case "readerwriterref":
                            if (ReaderXml.MoveToAttribute("ID"))
                            {
                                string id = ReaderXml.Value.ToUpper();
                                if ((ReaderWriters.ContainsKey(id)) && (profile != null))
                                {
                                    METS_Section_ReaderWriter_Config readerWriter = ReaderWriters[id];
                                    if (inPackage)
                                    {
                                        if (inDmdSec)
                                            profile.Add_Package_Level_DmdSec_Writer_Config(readerWriter);
                                        else
                                            profile.Add_Package_Level_AmdSec_Writer_Config(readerWriter);
                                    }
                                    else if (inDivision)
                                    {
                                        if (inDmdSec)
                                            profile.Add_Division_Level_DmdSec_Writer_Config(readerWriter);
                                        else
                                            profile.Add_Division_Level_AmdSec_Writer_Config(readerWriter);
                                    }
                                    else if (inFile)
                                    {
                                        if (inDmdSec)
                                            profile.Add_File_Level_DmdSec_Writer_Config(readerWriter);
                                        else
                                            profile.Add_File_Level_AmdSec_Writer_Config(readerWriter);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #region Section reads all the viewer/writer configuration information

        private static bool read_writer_viewer_details(XmlReader ReaderXml, InstanceWide_Configuration Config)
        {
            bool errorEncountered = false;

            try
            {
                while (ReaderXml.Read())
                {
                    if (ReaderXml.NodeType == XmlNodeType.Element)
                    {
                        switch (ReaderXml.Name.ToLower())
                        {
                            case "clearall":
                                Config.UI.WriterViewers.Clear();
                                break;

                            case "itemwriter":
                                if (ReaderXml.MoveToAttribute("class"))
                                    Config.UI.WriterViewers.Items.Class = ReaderXml.Value.Trim();
                                if (ReaderXml.MoveToAttribute("assembly"))
                                    Config.UI.WriterViewers.Items.Assembly = ReaderXml.Value.Trim();
                                ReaderXml.MoveToElement();
                                read_item_writer_viewer_configs(ReaderXml.ReadSubtree(), Config.UI.WriterViewers);
                                break;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Config.Add_Log("EXCEPTION CAUGHT in Configuration_Files_Reader.read_writer_viewer_details");
                Config.Add_Log(ee.Message);
                Config.Add_Log(ee.StackTrace);

                Config.ErrorEncountered = true;
                errorEncountered = true;
            }

            // If there was an error while reading, use the system defaults
            if (errorEncountered)
            {
                Config.Metadata.Clear();
                Config.Metadata.Set_Default_Values();
            }

            return !errorEncountered;
        }

        private static void read_item_writer_viewer_configs(XmlReader ReaderXml, WriterViewerConfig Config)
        {
            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        // viewerCode = "#j" 
                        // assembly = "" 
                        // class="SobekCM.Library.ItemViewer.Viewers.JPEG_ItemViewer_Prototyper" 
                        // enabled="true" 
                        // alwaysAdd="true" 
                        // pageFileExtensions="JPG|JPEG"
                        // fileExtensions

                        case "itemviewer":
                            ItemSubViewerConfig newConfig = new ItemSubViewerConfig();
                            if (ReaderXml.MoveToAttribute("type"))
                                newConfig.ViewerType = ReaderXml.Value.Trim();
                            if (ReaderXml.MoveToAttribute("viewerCode"))
                                newConfig.ViewerCode = ReaderXml.Value.Trim();
                            if (ReaderXml.MoveToAttribute("assembly"))
                                newConfig.Assembly = ReaderXml.Value.Trim();
                            if (ReaderXml.MoveToAttribute("class"))
                                newConfig.Class = ReaderXml.Value.Trim();
                            if (ReaderXml.MoveToAttribute("enabled"))
                            {
                                bool tempValue;
                                if (bool.TryParse(ReaderXml.Value, out tempValue))
                                {
                                    newConfig.Enabled = tempValue;
                                }
                            }
                            if (ReaderXml.MoveToAttribute("alwaysAdd"))
                            {
                                bool tempValue;
                                if (bool.TryParse(ReaderXml.Value, out tempValue))
                                {
                                    newConfig.AlwaysAdd = tempValue;
                                }
                            }
                            if (ReaderXml.MoveToAttribute("pageFileExtensions"))
                            {
                                newConfig.PageExtensions = ReaderXml.Value.Trim().Split(new char[] { '|', ',' });
                            }
                            if (ReaderXml.MoveToAttribute("fileExtensions"))
                            {
                                newConfig.FileExtensions = ReaderXml.Value.Trim().Split(new char[] { '|', ',' });
                            }

                            // Check for minimum requirements
                            if ((!String.IsNullOrEmpty(newConfig.ViewerType)) && (!String.IsNullOrEmpty(newConfig.ViewerCode)))
                                Config.Items.Add_Viewer(newConfig);
                            break;
                    }
                }
            }
        }

        #endregion

        #region Section reads the citation configuration information

        private static bool read_citation_details(XmlReader ReaderXml, InstanceWide_Configuration Config)
        {
            bool errorEncountered = false;

            try
            {
                while (ReaderXml.Read())
                {
                    if (ReaderXml.NodeType == XmlNodeType.Element)
                    {
                        switch (ReaderXml.Name.ToLower())
                        {
                            case "clearall":
                                Config.UI.CitationViewer.Clear();
                                break;

                            case "citationset":
                                string setName = "UNNAMED";
                                if (ReaderXml.MoveToAttribute("Name"))
                                    setName = ReaderXml.Value.Trim();
                                // Ensure a setname exists
                                if (String.IsNullOrEmpty(setName))
                                    setName = "AUTOSETNAME" + (Config.UI.CitationViewer.CitationSets.Count + 1);

                                // Is this the (new) default
                                if (ReaderXml.MoveToAttribute("Default"))
                                {
                                    if (String.Compare(ReaderXml.Value.Trim(), "true", StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        Config.UI.CitationViewer.DefaultCitationSet = setName;
                                    }
                                }

                                // Create the citation set and add it, or get the existing
                                CitationSet citationSet = Config.UI.CitationViewer.Add_CitationSet(setName);

                                // Read all the details for this citation set
                                ReaderXml.MoveToElement();
                                read_citation_set_details(ReaderXml.ReadSubtree(), citationSet);

                                break;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Config.Add_Log("EXCEPTION CAUGHT in Configuration_Files_Reader.read_citation_details");
                Config.Add_Log(ee.Message);
                Config.Add_Log(ee.StackTrace);

                Config.ErrorEncountered = true;
                errorEncountered = true;
            }

            // If there was an error while reading, use the system defaults
            if (errorEncountered)
            {
                Config.Metadata.Clear();
                Config.Metadata.Set_Default_Values();
            }

            return !errorEncountered;
        }


        private static void read_citation_set_details(XmlReader ReaderXml, CitationSet Config)
        {
            CitationFieldSet currFieldSet = null;
            CitationElement currElement = null;
            bool inElements = false;

            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "fieldset":
                            if (ReaderXml.MoveToAttribute("ID"))
                            {
                                // Pull the attributes, basic info about this field set
                                string fieldSetId = ReaderXml.Value.Trim();
                                string defaultHeading = fieldSetId;
                                string order = "append";
                                string afterid = "";
                                if (ReaderXml.MoveToAttribute("Heading"))
                                    defaultHeading = ReaderXml.Value.Trim();

                                // Does this field set already exist? ( in which case order and after id don't matter )
                                if (ReaderXml.MoveToAttribute("Order"))
                                    order = ReaderXml.Value.Trim().ToLower();
                                if (ReaderXml.MoveToAttribute("AfterID"))
                                    afterid = ReaderXml.Value.Trim();

                                // Ensure the order is one of the terms supported
                                if ((order != "first") && (order != "after") && (order != "append"))
                                    order = "append";

                                // Add this field set
                                currFieldSet = Config.AddFieldSet(fieldSetId, defaultHeading, order, afterid);
                            }
                            else
                            {
                                currFieldSet = null;
                            }
                            break;

                        case "language":
                            // Get the language code and term
                            string code = String.Empty;
                            if (ReaderXml.MoveToAttribute("Code"))
                                code = ReaderXml.Value.Trim().ToLower();
                            ReaderXml.Read();
                            string term = ReaderXml.Value.Trim();

                            // WHich level is this form?
                            if (!String.IsNullOrEmpty(code))
                            {
                                if (currElement != null)
                                {
                                    currElement.Add_Translation(Web_Language_Enum_Converter.Code_To_Enum(code), term);
                                }
                                else if (currFieldSet != null)
                                {
                                    currFieldSet.Add_Translation(Web_Language_Enum_Converter.Code_To_Enum(code), term);
                                }
                            }
                            break;

                        case "elements":
                            inElements = true;
                            break;

                        case "clear":
                            if ( currFieldSet != null )
                                currFieldSet.Clear_Elements();
                            break;

                        case "append":
                            // Collect the basic values
                            string appendMetadataTerm = (ReaderXml.MoveToAttribute("MetadataTerm")) ? ReaderXml.Value.Trim() : String.Empty;
                            string appendDisplayTerm = (ReaderXml.MoveToAttribute("DisplayTerm")) ? ReaderXml.Value.Trim() : appendMetadataTerm;
                            string appendItemProp = (ReaderXml.MoveToAttribute("ItemProp")) ? ReaderXml.Value.Trim() : null;
                            string appendSearchCode = (ReaderXml.MoveToAttribute("SearchCode")) ? ReaderXml.Value.Trim() : null;
                            currElement = new CitationElement
                            {
                                MetadataTerm = appendMetadataTerm,
                                DisplayTerm = appendDisplayTerm,
                                ItemProp = appendItemProp,
                                SearchCode = appendSearchCode
                            };

                            // Set the override display enumeration
                            string appendOverrideDisplay = (ReaderXml.MoveToAttribute("OverrideDisplayTerm")) ? ReaderXml.Value.Trim() : String.Empty;
                            switch (appendOverrideDisplay.ToLower())
                            {
                                case "subterm":
                                    currElement.OverrideDisplayTerm = CitationElement_OverrideDispayTerm_Enum.subterm;
                                    break;

                                default:
                                    currElement.OverrideDisplayTerm = CitationElement_OverrideDispayTerm_Enum.NONE;
                                    break;
                            }

                            // Only actually add it if the metadata term is valid
                            if ((!String.IsNullOrEmpty(appendMetadataTerm)) && (currFieldSet != null))
                                currFieldSet.Append_Element(currElement);
                            break;

                        case "insert":
                            // Collect the basic values
                            string insertMetadataTerm = (ReaderXml.MoveToAttribute("MetadataTerm")) ? ReaderXml.Value.Trim() : String.Empty;
                            string insertDisplayTerm = (ReaderXml.MoveToAttribute("DisplayTerm")) ? ReaderXml.Value.Trim() : insertMetadataTerm;
                            string insertItemProp = (ReaderXml.MoveToAttribute("ItemProp")) ? ReaderXml.Value.Trim() : null;
                            string insertSearchCode = (ReaderXml.MoveToAttribute("SearchCode")) ? ReaderXml.Value.Trim() : null;
                            currElement = new CitationElement
                            {
                                MetadataTerm = insertMetadataTerm,
                                DisplayTerm = insertDisplayTerm,
                                ItemProp = insertItemProp,
                                SearchCode = insertSearchCode
                            };

                            // Set the override display enumeration
                            string insertOverrideDisplay = (ReaderXml.MoveToAttribute("OverrideDisplayTerm")) ? ReaderXml.Value.Trim() : String.Empty;
                            switch (insertOverrideDisplay.ToLower())
                            {
                                case "subterm":
                                    currElement.OverrideDisplayTerm = CitationElement_OverrideDispayTerm_Enum.subterm;
                                    break;

                                default:
                                    currElement.OverrideDisplayTerm = CitationElement_OverrideDispayTerm_Enum.NONE;
                                    break;
                            }

                            // Only actually add it if the metadata term is valid
                            if ((!String.IsNullOrEmpty(insertMetadataTerm)) && (currFieldSet != null))
                            {
                                if (ReaderXml.MoveToAttribute("After"))
                                {
                                    currFieldSet.Insert_Element_After(currElement, ReaderXml.Value.Trim());
                                }
                                else if (ReaderXml.MoveToAttribute("Before"))
                                {
                                    currFieldSet.Insert_Element_Before(currElement, ReaderXml.Value.Trim());
                                }
                                else
                                {
                                    currFieldSet.Append_Element(currElement);
                                }
                            }
                            break;

                        case "remove":
                            if (( currFieldSet != null ) && ( ReaderXml.MoveToAttribute("Code")))
                                currFieldSet.Remove_Element(ReaderXml.Value.Trim());
                            currElement = null;
                            break;

                        case "sectionwriter":
                            if (currElement != null)
                            {
                                string assembly = (ReaderXml.MoveToAttribute("Assembly")) ? ReaderXml.Value.Trim() : null;
                                string writer_class = (ReaderXml.MoveToAttribute("Assembly")) ? ReaderXml.Value.Trim() : null;
                                if (!String.IsNullOrEmpty(writer_class))
                                {
                                    currElement.SectionWriter = new SectionWriter(assembly, writer_class);
                                }
                            }
                            break;

                    }
                }
                else if (ReaderXml.NodeType == XmlNodeType.EndElement)
                {
                    // This is closing out a field set or a single element
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "fieldset":
                            currFieldSet = null;
                            currElement = null;
                            inElements = false;
                            break;

                        case "append":
                        case "insert":
                        case "remove":
                            currElement = null;
                            break;

                        case "elements":
                            inElements = false;
                            break;
                    }
                }
            }
        }

        #endregion

        #region Section reads the template element configuration information

        private static bool read_template_elements_details(XmlReader ReaderXml, InstanceWide_Configuration Config)
        {
            bool errorEncountered = false;

            try
            {
                while (ReaderXml.Read())
                {
                    if (ReaderXml.NodeType == XmlNodeType.Element)
                    {
                        switch (ReaderXml.Name.ToLower())
                        {
                            case "clearall":
                                Config.UI.TemplateElements.Clear();
                                break;

                            case "templateelement":

                                // Build the new template element info
                                TemplateElement newElement = new TemplateElement();
                                if (ReaderXml.MoveToAttribute("type"))
                                    newElement.Type = ReaderXml.Value.Trim().ToLower();
                                if (ReaderXml.MoveToAttribute("subtype"))
                                    newElement.Subtype = ReaderXml.Value.Trim().ToLower();
                                if (ReaderXml.MoveToAttribute("assembly"))
                                    newElement.Assembly = ReaderXml.Value.Trim();
                                if (ReaderXml.MoveToAttribute("class"))
                                    newElement.Class = ReaderXml.Value.Trim();
                                if (ReaderXml.MoveToAttribute("image"))
                                    newElement.Image = ReaderXml.Value.Trim();

                                // add if the minimum requirements are met
                                if ((!String.IsNullOrEmpty(newElement.Type)) && (!String.IsNullOrEmpty(newElement.Class)))
                                    Config.UI.TemplateElements.Add_Element(newElement);

                                break;

                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Config.Add_Log("EXCEPTION CAUGHT in Configuration_Files_Reader.read_template_elements_details");
                Config.Add_Log(ee.Message);
                Config.Add_Log(ee.StackTrace);

                Config.ErrorEncountered = true;
                errorEncountered = true;
            }

            // If there was an error while reading, use the system defaults
            if (errorEncountered)
            {
                Config.Metadata.Clear();
                Config.Metadata.Set_Default_Values();
            }

            return !errorEncountered;
        }

        #endregion



    }
}
