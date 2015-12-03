using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Authentication;
using SobekCM.Core.Configuration.Engine;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Configuration.OAIPMH;
using SobekCM.Core.Settings;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Items.BriefItems.Mappers;

namespace SobekCM.Engine_Library.Configuration
{
    public class Configuration_Files_Reader
    {

        /// <summary> Refreshes the values from the database settings </summary>
        /// <returns> A fully builder instance-wide setting object </returns>
        public static InstanceWide_Configuration Read_Config_Files( List<string> ConfigurationDirectories , InstanceWide_Settings Settings)
        {
            InstanceWide_Configuration returnValue = new InstanceWide_Configuration();

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

            // With all the files to read collected and sorted, read each one
            foreach (string thisConfigFile in configFiles)
            {
                read_config_file(thisConfigFile, returnValue, Settings);
            }

            return returnValue;

            //// Try to read the SHIBBOLETH configuration file
            //if (File.Exists(Base_Directory + "\\config\\user\\sobekcm_shibboleth.config"))
            //{
            //    returnValue.Authentication.Shibboleth = Shibboleth_Configuration_Reader.Read_Config(Base_Directory + "\\config\\user\\sobekcm_shibboleth.config");
            //}
            //else if (File.Exists(Base_Directory + "\\config\\default\\sobekcm_shibboleth.config"))
            //{
            //    returnValue.Authentication.Shibboleth = Shibboleth_Configuration_Reader.Read_Config(Base_Directory + "\\config\\default\\sobekcm_shibboleth.config");
            //}

            //// Try to read the CONTACT FORM configuration file
            //if (File.Exists(Base_Directory + "\\config\\user\\sobekcm_contactform.config"))
            //{
            //    returnValue.ContactForm = ContactForm_Configuration_Reader.Read_Config(Base_Directory + "\\config\\user\\sobekcm_contactform.config");
            //}
            //else if (File.Exists(Base_Directory + "\\config\\default\\sobekcm_contactform.config"))
            //{
            //    returnValue.ContactForm = ContactForm_Configuration_Reader.Read_Config(Base_Directory + "\\config\\default\\sobekcm_contactform.config");
            //}

            //// Try to read the QUALITY CONTROL configuration file
            ////if (File.Exists(returnValue.Servers.Base_Directory + "\\config\\user\\sobekcm_qc.config"))
            ////{
            ////    QualityControl_Configuration.Read_Metadata_Configuration(returnValue.Servers.Base_Directory + "\\config\\user\\sobekcm_qc.config");
            ////}
            ////else if (File.Exists(returnValue.Servers.Base_Directory + "\\config\\default\\sobekcm_qc.config"))
            ////{
            ////    QualityControl_Configuration.Read_Metadata_Configuration(returnValue.Servers.Base_Directory + "\\config\\default\\sobekcm_qc.config");
            ////}

            //// Try to read the BRIEF ITEM MAPPING configuration file
            //if (File.Exists(Base_Directory + "\\config\\user\\sobekcm_brief_item_mapping.config"))
            //{
            //    BriefItem_Factory.Read_Config(Base_Directory + "\\config\\user\\sobekcm_brief_item_mapping.config");
            //}
            //else if (File.Exists(Base_Directory + "\\config\\default\\sobekcm_brief_item_mapping.config"))
            //{
            //    BriefItem_Factory.Read_Config(Base_Directory + "\\config\\default\\sobekcm_brief_item_mapping.config");
            //}

            //// Try to read the OAI-PMH configuration file
            //if (File.Exists(Base_Directory + "\\config\\user\\sobekcm_oaipmh.config"))
            //{
            //    returnValue.OAI_PMH = OAI_PMH_Configuration_Reader.Read_Config(Base_Directory + "\\config\\user\\sobekcm_oaipmh.config", Settings.System.System_Name, Settings.System.System_Abbreviation, Settings.Email.System_Email);
            //}
            //else if (File.Exists(Base_Directory + "\\config\\default\\sobekcm_oaipmh.config"))
            //{
            //    returnValue.OAI_PMH = OAI_PMH_Configuration_Reader.Read_Config(Base_Directory + "\\config\\default\\sobekcm_oaipmh.config", Settings.System.System_Name, Settings.System.System_Abbreviation, Settings.Email.System_Email);
            //}

            //// Load the OAI-PMH configuration file info into the OAI writer class ( in the resource object library )
            //if (returnValue.OAI_PMH == null)
            //{
            //    returnValue.OAI_PMH = new OAI_PMH_Configuration();
            //    returnValue.OAI_PMH.Set_Default();
            //}

            //OAI_PMH_Metadata_Writers.Clear();
            //foreach (OAI_PMH_Metadata_Format thisWriter in returnValue.OAI_PMH.Metadata_Prefixes)
            //{
            //    if (thisWriter.Enabled)
            //    {
            //        OAI_PMH_Metadata_Writers.Add_Writer(thisWriter.Prefix, thisWriter.Assembly, thisWriter.Namespace, thisWriter.Class);
            //    }
            //}

            return returnValue;
        }

        private static bool read_config_file(string ConfigFile, InstanceWide_Configuration ConfigObj, InstanceWide_Settings Settings)
        {
            Console.WriteLine(ConfigFile);

            // Streams used for reading
            Stream readerStream = null;
            XmlTextReader readerXml = null;



            try
            {
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
                                read_authentication_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                            case "oai-pmh":
                                read_oai_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                            case "contactform":
                                read_contactform_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                            case "briefitem_mapping":
                                read_briefitem_mapping_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                            case "mapeditor":
                                read_mapeditor_details(readerXml.ReadSubtree(), ConfigObj);
                                break;

                            case "engine":
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
                                read_quality_control_details(readerXml.ReadSubtree(), ConfigObj);
                                break;


                            // metadata

                        }
                    }
                }
            }
            catch (Exception ee)
            {
                //returnValue.Error = ee.Message;
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
            if (Config.MapEditor == null)
                Config.MapEditor = new MapEditor_Configuration();


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
                                config.Metadata_Prefixes.Add(component);
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

                                // Read the set here
                                ReaderXml.MoveToElement();
                                List<BriefItemMapping_Mapper> mapSet = read_mappingset_details(ReaderXml.ReadSubtree(), mappingObjDictionary);

                                // Save in the dictionary of mapping sets
                                if (id.Length > 0)
                                {
                                    BriefItemMapping_Set setObj = new BriefItemMapping_Set
                                    {
                                        SetName = id,
                                        Mappings = mapSet
                                    };

                                    Config.BriefItemMapping.MappingSets.Add(setObj);
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                //Read_Config_Error = ee.Message;
                return false;
            }

            return true;
        }

        private static List<BriefItemMapping_Mapper> read_mappingset_details(XmlReader ReaderXml, Dictionary<string, IBriefItemMapper> MappingObjDictionary)
        {
            // Create the empty return value
            List<BriefItemMapping_Mapper> returnValue = new List<BriefItemMapping_Mapper>();

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

                                returnValue.Add(mapperConfig);
                            }


                            break;
                    }
                }
            }

            return returnValue;
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
            if ( Config.MapEditor == null )
                Config.MapEditor = new MapEditor_Configuration();
            

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
                            Config.MapEditor.Collections.Add(collection);
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
                            read_microservices_details_restrictionranges(ReaderXml.ReadSubtree(), Config.Engine );
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
                            Engine_Path_Endpoint endpoint = new Engine_Path_Endpoint();
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
            Engine_Path_Endpoint endpoint = new Engine_Path_Endpoint();
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

        private static void read_microservices_details_restrictionranges(XmlReader ReaderXml, Engine_Server_Configuration Config )
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

    }
}
