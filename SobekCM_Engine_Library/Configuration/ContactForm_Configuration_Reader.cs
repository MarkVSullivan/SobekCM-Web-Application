#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Engine_Library.Configuration
{
    /// <summary> Class is used to read the contact form configuration file, or the portion
    /// about the contact form from the aggregation file </summary>
    public static class ContactForm_Configuration_Reader
    {
        /// <summary> Read the contact form system configuration file from within the config subfolder on the web app </summary>
        /// <param name="ConfigFile"> Complete path and name of the configuration file to read </param>
        /// <returns> Built configuration object for the contact form </returns>
        public static ContactForm_Configuration Read_Config(string ConfigFile)
        {
            ContactForm_Configuration returnValue = new ContactForm_Configuration();

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
                            case "contactform":
                                Read_ContactForm_Details(readerXml.ReadSubtree(), returnValue);
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

        private static void Read_ContactForm_Details(XmlReader readerXml, ContactForm_Configuration config)
        {
            // Read the attributes
            if (readerXml.MoveToAttribute("Name"))
                config.Name = readerXml.Value.Trim();

            // Just step through the subtree of this
            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    switch (readerXml.Name.ToLower())
                    {
                        case "elements":
                            read_contactform_elements(readerXml.ReadSubtree(), config);
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
    }
}
