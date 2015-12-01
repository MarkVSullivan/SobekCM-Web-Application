#region Using directives

using System;
using System.IO;
using System.Xml;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.OAIPMH;

#endregion

namespace SobekCM.Engine_Library.Configuration
{
    /// <summary> Static class is used to read the configuration file defining oai-pmh elements and metadata prefixes </summary>
    public static class OAI_PMH_Configuration_Reader
    {
        /// <summary> Static class is used to read the configuration file defining oai-pmh elements and metadata prefixes </summary>
        /// <param name="ConfigFile"> Path and name of the configuration XML file to read </param>
        /// <param name="SystemName"> System name from the system-wide settings, used as a default name for OAI-PMH </param>
        /// <param name="SystemAbbreviation"> System identifyer from the system-wide settings, used as a default identifier for OAI-PMH </param>
        /// <param name="SystemEmail"> System email(s) from the system-wide settings, used as default admin email(s) for OAI-PMH </param>
        /// <returns> Fully configured OAI-PMH configuration object </returns>
        public static OAI_PMH_Configuration Read_Config(string ConfigFile, string SystemName, string SystemAbbreviation, string SystemEmail )
        {
            // Create config value and set some default values
            OAI_PMH_Configuration returnValue = new OAI_PMH_Configuration
            {
                Identifier = SystemAbbreviation,
                Name = SystemName,
                Identifier_Base = "oai:" + SystemAbbreviation.ToLower() + ":"
            };
            returnValue.Add_Admin_Email(SystemEmail);

            // Streams used for reading
            Stream readerStream = null;
            XmlTextReader readerXml = null;

            try
            {
                // Open a link to the file
                readerStream = new FileStream(ConfigFile, FileMode.Open, FileAccess.Read);

                // Open a XML reader connected to the file
                readerXml = new XmlTextReader(readerStream);

                while (readerXml.Read())
                {
                    if (readerXml.NodeType == XmlNodeType.Element)
                    {
                        switch (readerXml.Name.ToLower())
                        {
                            case "oai-pmh":
                                read_oai_details(readerXml.ReadSubtree(), returnValue);
                                break;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                returnValue.Error = ee.Message;
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

            return returnValue;
        }

        private static void read_oai_details(XmlReader readerXml, OAI_PMH_Configuration config)
        {
            bool baseSpecified = false;

            // Just step through the subtree of this
            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    switch (readerXml.Name.ToLower())
                    {
                        case "oai-pmh":
                            if (readerXml.Value.Trim().ToLower() == "false")
                                config.Enabled = false;
                            break;

                        case "repository":
                            if (readerXml.MoveToAttribute("IdentifierBase"))
                            {
                                baseSpecified = true;
                                config.Identifier_Base = readerXml.Value.Trim();
                            }
                            break;

                        case "identify":
                            read_oai_details_identify(readerXml.ReadSubtree(), config, baseSpecified );
                            break;

                        case "metadataprefixes":
                            read_oai_details_metadataPrefixes(readerXml.ReadSubtree(), config);
                            break;
                    }
                }
            }
        }

        private static void read_oai_details_identify(XmlReader readerXml, OAI_PMH_Configuration config, bool baseSpecified )
        {
            bool emailFound = false;

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
                            if (!emailFound)
                            {
                                config.Admin_Emails.Clear();
                                emailFound = true;
                            }
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
    }
}
