using System;
using System.IO;
using System.Xml;
using EngineAgnosticLayerDbAccess;
using SobekCM.Core.MicroservicesClient;

namespace SobekCM.Builder_Library.Settings
{
    /// <summary> Reads the builder configuration file, which is configured differently than the standard configuration file </summary>
    public static class MultiInstance_Builder_Settings_Reader
    {
        /// <summary> Read all the information about the instance(s) for the builder to operate over </summary>
        /// <param name="ConfigFile"> Filename and path of the configuration </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool Read_Config(string ConfigFile)
        {
            try
            {
                // Clear any previously loaded data
                MultiInstance_Builder_Settings.Clear();

                // If the file does not exist log an error return FALSE
                if (!File.Exists(ConfigFile))
                {
                    MultiInstance_Builder_Settings.Add_Error("ERROR! Configuration file is missing!");
                    MultiInstance_Builder_Settings.Add_Error("Looked for '" + ConfigFile + "' but was missing");
                    return false;
                }

                // Open a stream to the configuration file
                StreamReader reader = new StreamReader(ConfigFile);
                XmlTextReader xmlReader = new XmlTextReader(reader);
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        string node_name = xmlReader.Name.ToLower();
                        switch (node_name)
                        {
                            case "instances":
                                read_instances(xmlReader.ReadSubtree());
                                break;

                            case "ghostscript_executable":
                                xmlReader.Read();
                                MultiInstance_Builder_Settings.Ghostscript_Executable = xmlReader.Value;
                                break;

                            case "imagemagick_executable":
                                xmlReader.Read();
                                MultiInstance_Builder_Settings.ImageMagick_Executable = xmlReader.Value;
                                break;

                            case "tesseract_executable":
                                xmlReader.Read();
                                MultiInstance_Builder_Settings.Tesseract_Executable = xmlReader.Value;
                                break;

                            case "pause_between_polls":
                                xmlReader.Read();
                                int testValue;
                                if (Int32.TryParse(xmlReader.Value, out testValue))
                                    MultiInstance_Builder_Settings.Override_Seconds_Between_Polls = testValue;
                                break;

                            case "connections":
                                // This is the old ( pre version 4.10.0 ) format of instance information
                                // This will remain backwardly compatible for a while
                                read_legacy_instance_config(xmlReader.ReadSubtree());
                                break;

                        }
                    }
                }

                xmlReader.Close();
                reader.Close();

                // Ensure some instances were found
                if (MultiInstance_Builder_Settings.Instances.Count == 0)
                {
                    MultiInstance_Builder_Settings.Add_Error("WARNING: Configuration file read successfully, but no instance configurations were found in the file.");
                    return false;
                }

                return true;
            }
            catch (Exception ee)
            {
                MultiInstance_Builder_Settings.Add_Error("EXCEPTION! Unexpected exception caught while trying to read the buidler configuration file");
                MultiInstance_Builder_Settings.Add_Error(ee.Message);
                return false;
            }
        }

        private static void read_legacy_instance_config(XmlReader ReaderXml)
        {
            while (ReaderXml.Read())
            {
                // Only detect start elements.
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    // Get element name and switch on it.
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "connection_string":
                            Single_Instance_Configuration singleInstance = new Single_Instance_Configuration();
                            if (ReaderXml.MoveToAttribute("active"))
                            {
                                if (ReaderXml.Value.ToLower() == "false")
                                    singleInstance.Is_Active = false;
                            }
                            if (ReaderXml.MoveToAttribute("name"))
                                singleInstance.Name = ReaderXml.Value.Trim();
                            if (ReaderXml.MoveToAttribute("type"))
                            {
                                if (ReaderXml.Value.ToLower() == "postgresql")
                                    singleInstance.DatabaseConnection.Database_Type = EalDbTypeEnum.PostgreSQL;
                            }
                            ReaderXml.Read();
                            singleInstance.DatabaseConnection.Connection_String = ReaderXml.Value;

                            // Add the default microservice endpoints then
                            singleInstance.Microservices.Add_Endpoint("Builder.Get_Builder_Settings", "[BASEURL]/engine/builder/settings/protobuf?IncludeDescs={0}", Microservice_Endpoint_Protocol_Enum.PROTOBUF);
                            singleInstance.Microservices.Add_Endpoint("Configuration.Extensions", "[BASEURL]/engine/config/extensions/protobuf", Microservice_Endpoint_Protocol_Enum.PROTOBUF);
                            singleInstance.Microservices.Add_Endpoint("Configuration.Metadata", "[BASEURL]/engine/config/metadata/protobuf", Microservice_Endpoint_Protocol_Enum.PROTOBUF);
                            singleInstance.Microservices.Add_Endpoint("Configuration.OAI_PMH", "[BASEURL]/engine/config/oaipmh/protobuf", Microservice_Endpoint_Protocol_Enum.PROTOBUF);

                            // Esnure it has SOME name
                            if (String.IsNullOrWhiteSpace(singleInstance.Name))
                                singleInstance.Name = "Connection" + (MultiInstance_Builder_Settings.Instances.Count + 1);

                            // Add this to the list of instances
                            MultiInstance_Builder_Settings.Instances.Add(singleInstance);
                            break;
                    }
                }
            }
        }

        private static void read_instances(XmlReader ReaderXml )
        {
            Single_Instance_Configuration singleInstance = new Single_Instance_Configuration();

            while (ReaderXml.Read())
            {
                // Only detect start elements.
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    // Get element name and switch on it.
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "instance":
                            singleInstance = new Single_Instance_Configuration();
                            if (ReaderXml.MoveToAttribute("active"))
                            {
                                if (ReaderXml.Value.ToLower() == "false")
                                    singleInstance.Is_Active = false;
                            }
                            if (ReaderXml.MoveToAttribute("name"))
                                singleInstance.Name = ReaderXml.Value.Trim();
                            break;

                        case "connection_string":
                            if (ReaderXml.MoveToAttribute("type"))
                            {
                                if (ReaderXml.Value.ToLower() == "postgresql")
                                    singleInstance.DatabaseConnection.Database_Type = EalDbTypeEnum.PostgreSQL;
                            }
                            ReaderXml.Read();
                            singleInstance.DatabaseConnection.Connection_String = ReaderXml.Value;
                            break;

                        case "microservices":
                            MicroservicesClient_Config_Reader.Read_Microservices_Client_Details(ReaderXml.ReadSubtree(), singleInstance.Microservices, String.Empty);
                            break;
                    }
                }
                else if ((ReaderXml.NodeType == XmlNodeType.EndElement) && (ReaderXml.Name.ToLower() == "instance"))
                {
                    // Esnure it has SOME name
                    if ( String.IsNullOrWhiteSpace(singleInstance.Name))
                        singleInstance.Name = "Connection" + ( MultiInstance_Builder_Settings.Instances.Count + 1);

                    // Add this to the list of instances
                    MultiInstance_Builder_Settings.Instances.Add(singleInstance);
                }
            }
        }
    }
}
