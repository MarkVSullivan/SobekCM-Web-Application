using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SobekCM.Core.Configuration;

namespace SobekCM.Builder_Library.Settings
{
    public static class MultiInstance_Builder_Settings_Reader
    {

        public static bool Read_Config(string ConfigFile)
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
                        case "connection_string":
                            Database_Instance_Configuration newDb = new Database_Instance_Configuration();
                            //if (xmlReader.MoveToAttribute("type"))
                            //{
                            //    if (xmlReader.Value.ToLower() == "postgresql")
                            //        newDb.Database_Type = EalDbTypeEnum.PostgreSQL;
                            //}
                            //if (xmlReader.MoveToAttribute("active"))
                            //{
                            //    if (xmlReader.Value.ToLower() == "false")
                            //        newDb.Is_Active = false;
                            //}
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
                            //if (xmlReader.MoveToAttribute("name"))
                            //    newDb.Name = xmlReader.Value.Trim();

                            //xmlReader.Read();
                            //newDb.Connection_String = xmlReader.Value;
                            //if (newDb.Name.Length == 0)
                            //    newDb.Name = "Connection" + (SettingsObject.Database_Connections.Count + 1);
                            //SettingsObject.Database_Connections.Add(newDb);
                            break;

                        case "ghostscript_executable":
                            xmlReader.Read();
                            MultiInstance_Builder_Settings.Ghostscript_Executable = xmlReader.Value;
                            break;

                        case "imagemagick_executable":
                            xmlReader.Read();
                            MultiInstance_Builder_Settings.ImageMagick_Executable = xmlReader.Value;
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

            return true;
        }
    }
}
