using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using EngineAgnosticLayerDbAccess;
using SobekCM.Core.Configuration;

namespace SobekCM.Builder_Library.Settings
{
    public static class MultiInstance_Builder_Settings_Reader
    {

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


        private static void read_instances(XmlReader ReaderXml )
        {
            Single_Instance_Configuration singleInstance = new Single_Instance_Configuration();

            while (ReaderXml.Read())
            {
                // Only detect start elements.
                if (ReaderXml.IsStartElement())
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

                            //if (xmlReader.MoveToAttribute("canAbort"))
                            //{
                            //    if (xmlReader.Value.ToLower() == "false")
                            //        newDb.Can_Abort = false;
                            //}
                            //if (xmlReader.MoveToAttribute("isHosted"))
                            //{
                            //    if (xmlReader.Value.ToLower() == "true")
                            //        SettingsObject.Servers.isHosted = true;
                            //}
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

                        case "engine":
                            ReaderXml.Read();
                            singleInstance.Engine_URL = ReaderXml.Value;
                            break;
                    }
                }
                else if ((ReaderXml.NodeType == XmlNodeType.EndElement) && (ReaderXml.Name.ToLower() == "Instance"))
                {
                    // Esnure it has SOME name
                    if ( String.IsNullOrWhiteSpace(singleInstance.Name))
                        singleInstance.Name = "Connection" + ( MultiInstance_Builder_Settings.Instances.Count + 1);
                    MultiInstance_Builder_Settings.Instances.Add(singleInstance);
                }
            }
        }
    }
}
