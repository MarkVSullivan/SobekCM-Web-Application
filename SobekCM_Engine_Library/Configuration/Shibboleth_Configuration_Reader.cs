using System;
using System.IO;
using System.Xml;
using SobekCM.Core.Configuration;
using SobekCM.Core.Users;

namespace SobekCM.Engine_Library.Configuration
{
    /// <summary> Class is used to read the shibboleth configuration file </summary>
    public static class Shibboleth_Configuration_Reader
    {
        public static Shibboleth_Configuration Read_Config(string ConfigFile)
        {
            Shibboleth_Configuration returnValue = new Shibboleth_Configuration();

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
                            case "shibboleth":
                                read_shibb_details(readerXml, returnValue);
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

        private static void read_shibb_details(XmlReader readerXml, Shibboleth_Configuration config)
        {
            // Read the attributes
            if (readerXml.MoveToAttribute("UserIdentityAttribute"))
                config.UserIdentityAttribute = readerXml.Value.Trim();

            if (readerXml.MoveToAttribute("URL"))
                config.ShibbolethURL = readerXml.Value.Trim();

            if (readerXml.MoveToAttribute("Label"))
                config.Label = readerXml.Value.Trim();

            if (readerXml.MoveToAttribute("Debug"))
            {
                if (String.Compare(readerXml.Value.Trim(), "true", true) == 0)
                    config.Debug = true;
            }

            if (readerXml.MoveToAttribute("Enabled"))
            {
                if (String.Compare(readerXml.Value.Trim(), "false", true) == 0)
                    config.Enabled = false;
            }


            // Just step through the subtree of this
            readerXml.MoveToElement();
            XmlReader subTreeReader = readerXml.ReadSubtree();
            while (subTreeReader.Read())
            {
                if (subTreeReader.NodeType == XmlNodeType.Element)
                {
                    switch (subTreeReader.Name.ToLower())
                    {
                        case "mapping":
                            string serverVariable = null;
                            string userAttribute = null;
                            if (subTreeReader.MoveToAttribute("ServerVariable"))
                                serverVariable = subTreeReader.Value.Trim();
                            if (subTreeReader.MoveToAttribute("UserAttribute"))
                                userAttribute = subTreeReader.Value.Trim();
                            if ((!String.IsNullOrEmpty(serverVariable)) && (!String.IsNullOrEmpty(userAttribute)))
                            {
                                User_Object_Attribute_Mapping_Enum userAttrEnum = User_Object_Attribute_Mapping_Enum_Converter.ToEnum(userAttribute.ToUpper());
                                if (userAttrEnum != User_Object_Attribute_Mapping_Enum.NONE)
                                {
                                    config.Add_Attribute_Mapping(serverVariable, userAttrEnum);
                                }
                            }
                            break;

                        case "constant":
                            string userAttribute2 = null;
                            string constantValue = null;
                            if (subTreeReader.MoveToAttribute("UserAttribute"))
                                userAttribute2 = subTreeReader.Value.Trim();
                            if (!subTreeReader.IsEmptyElement)
                            {
                                subTreeReader.Read();
                                constantValue = subTreeReader.Value.Trim();
                            }
                            if ((!String.IsNullOrEmpty(userAttribute2)) && (!String.IsNullOrEmpty(constantValue)))
                            {
                                User_Object_Attribute_Mapping_Enum userAttrEnum = User_Object_Attribute_Mapping_Enum_Converter.ToEnum(userAttribute2.ToUpper());
                                if (userAttrEnum != User_Object_Attribute_Mapping_Enum.NONE)
                                {
                                    config.Add_Constant(userAttrEnum, constantValue);
                                }
                            }
                            break;

                        case "cansubmit":
                            string serverVariable2 = null;
                            string requiredValue = null;
                            if (subTreeReader.MoveToAttribute("ServerVariable"))
                                serverVariable2 = subTreeReader.Value.Trim();
                            if (subTreeReader.MoveToAttribute("Value"))
                                requiredValue = subTreeReader.Value.Trim();
                            if ((!String.IsNullOrEmpty(serverVariable2)) && (!String.IsNullOrEmpty(requiredValue)))
                            {
                                config.Add_CanSubmit_Indicator(serverVariable2, requiredValue);
                            }
                            break;
                    }
                }
            }
        }
    }
}
