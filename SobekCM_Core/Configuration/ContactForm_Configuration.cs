using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using ProtoBuf;

namespace SobekCM.Core.Configuration
{
    /// <summary> Enumeration defines the different type of elements that can be included in the
    /// custom 'Contact Us' forms </summary>
    public enum ContactForm_Configuration_Element_Type_Enum : byte
    {
        /// <summary> Either a single check box, or a set of checkboxes (where multipe selections can 
        /// be checked) </summary>
        CheckBoxSet,

        /// <summary> Special textbox field includes email regex checking for valid input </summary>
        Email,

        /// <summary> Simple pararaph or line of text to explain something or inform the user filling
        /// out the 'Contact Us' form </summary>
        ExplanationText,

        /// <summary> Hidden value is included in the form (and subsequent email) but is not made
        /// available for editing by the user  </summary>
        HiddenValue,

        /// <summary> A set of radio buttons ( where only a single value can be selected ) </summary>
        RadioSet,

        /// <summary> Drop-down selection box allows the user to select a value from a drop down list </summary>
        SelectBox,

        /// <summary> Special textbox field is mapped to the subject line of the resulting email </summary>
        Subject,

        /// <summary> Area for a user to enter a large amount of text, or the main text for the body of the contact us email </summary>
        TextArea,

        /// <summary> Text box allows users to enter a free text answer to a simple query </summary>
        TextBox
    }

    /// <summary> Single element to appear within the Contact Us form's main section </summary>
    [Serializable, DataContract, ProtoContract]
    public class ContactForm_Configuration_Element
    {
        /// <summary> Constructor for a new instance of the ContactForm_Configuration_Element class </summary>
        /// <param name="Element_Type"> Type of element </param>
        public ContactForm_Configuration_Element(ContactForm_Configuration_Element_Type_Enum Element_Type )
        {
            this.Element_Type = Element_Type;
            Options = new List<string>();
            UserAttribute = Users.User_Object_Attribute_Mapping_Enum.NONE;
            AlwaysShow = false;
            Required = false;
            QueryText = new Web_Language_Translation_Lookup();
        }

        /// <summary> Type of the element to display with the Contact Us form </summary>
        [DataMember(Name = "type"), ProtoMember(1)]
        public ContactForm_Configuration_Element_Type_Enum Element_Type { get; set; }

        /// <summary> CSS class to apply for this HTML element </summary>
        [DataMember(Name = "cssClass", EmitDefaultValue = false), ProtoMember(2)]
        public string CssClass { get; set;  }

        /// <summary> Name of the element, which is also the name that appears in the output email </summary>
        [DataMember(Name = "name", EmitDefaultValue = false), ProtoMember(3)]
        public string Name { get; set; }

        /// <summary> Options from which the user can select ( for radio, checkboxes, and select ) </summary>
        [DataMember(Name = "options", EmitDefaultValue = false), ProtoMember(4)]
        public List<string> Options { get; set; }

        /// <summary> Text to display before the options for the user to enter answers or select options </summary>
        [DataMember(Name = "query", EmitDefaultValue = false), ProtoMember(5)]
        public Web_Language_Translation_Lookup QueryText { get; private set; }

        /// <summary> If this element could be either skipped or be auto-populated for logged on users, this indicates which
        /// field would replace thie field </summary>
        [DataMember(Name = "userArttribute", EmitDefaultValue = false), ProtoMember(6)]
        public Users.User_Object_Attribute_Mapping_Enum UserAttribute { get; set; }

        /// <summary> If this is mapped from a user attribute, this flag indicates whether it should be shown, even 
        /// if the user is logged on </summary>
        [DataMember(Name = "alwaysShow"), ProtoMember(7)]
        public bool AlwaysShow { get; set; }

        /// <summary> Flag indicates if this field requires a value </summary>
        [DataMember(Name = "required"), ProtoMember(8)]
        public bool Required { get; set; }
    }

    /// <summary> Class stores all the information about the contact us form either for the entire
    /// instance, or for a single aggregation </summary>
    [Serializable, DataContract, ProtoContract]
    public class ContactForm_Configuration
    {
        /// <summary> Constructor for a new instance of the Contact_Us_Configuration class </summary>
        public ContactForm_Configuration()
        {
            FormElements = new List<ContactForm_Configuration_Element>();
        }

        /// <summary> Error message in case there was an issue pulling this configuration from the configuration files </summary>
        [DataMember(Name = "error", EmitDefaultValue=false), ProtoMember(1)]
        public string Error { get; set; }

        /// <summary> Name for this contact us form, used for copying the form, or referencing during administrative work </summary>
        [DataMember(Name = "name"), ProtoMember(2)]
        public string Name { get; set; }

        /// <summary> Collection of form elements to be displayed for the user on a Contact Us form within the system </summary>
        [DataMember(Name = "elements"), ProtoMember(3)]
        public List<ContactForm_Configuration_Element> FormElements { get; set; }

        /// <summary> Add a new element to this contact form configuration file </summary>
        /// <param name="NewElement"> Element to add </param>
        public void Add_Element( ContactForm_Configuration_Element NewElement )
        {
            FormElements.Add(NewElement);
        }

        /// <summary> Returns the number of text areas within this template </summary>
        [IgnoreDataMember]
        public int TextAreaElementCount
        {
            get
            {
                return FormElements.Count(ThisElement => ThisElement.Element_Type == ContactForm_Configuration_Element_Type_Enum.TextArea);
            }
        }

        #region Code to save this shibboleth configuration to a XML file

        /// <summary> Save this quality control configuration to a XML config file </summary>
        /// <param name="FilePath"> File/path for the resulting XML config file </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_To_Config_File(string FilePath)
        {
            bool returnValue = true;
            StreamWriter writer = null;
            try
            {
                // Start the output file
                writer = new StreamWriter(FilePath, false, Encoding.UTF8);
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                writer.WriteLine("<SobekCM_Config xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");
                writer.WriteLine("\txmlns=\"http://sobekrepository.org/schemas/sobekcm_config\" ");
                writer.WriteLine("\txsi:schemaLocation=\"http://sobekrepository.org/schemas/sobekcm_config ");
                writer.WriteLine("\t\thttp://sobekrepository.org/schemas/sobekcm_config.xsd\">");
                if ( String.IsNullOrEmpty(Name))
                    writer.WriteLine("\t<ContactForm>");
                else
                    writer.WriteLine("\t<ContactForm Name=\"" + Convert_String_To_XML_Safe(Name) + "\">");

                if (FormElements.Count > 0)
                {
                    writer.WriteLine("\t\t<Elements>");

                    // Add the elements
                    foreach( ContactForm_Configuration_Element thisElement in FormElements )
                    {
                        string elementName = String.Empty;
                        switch( thisElement.Element_Type )
                        {
                            case ContactForm_Configuration_Element_Type_Enum.CheckBoxSet:
                                elementName = "CheckBoxSet";
                                break;

                            case ContactForm_Configuration_Element_Type_Enum.Email:
                                elementName = "Email";
                                break;

                            case ContactForm_Configuration_Element_Type_Enum.ExplanationText:
                                elementName = "ExplanationText";
                                break;

                            case ContactForm_Configuration_Element_Type_Enum.HiddenValue:
                                elementName = "HiddenValue";
                                break;

                            case ContactForm_Configuration_Element_Type_Enum.RadioSet:
                                elementName = "RadioSet";
                                break;

                            case ContactForm_Configuration_Element_Type_Enum.SelectBox:
                                elementName = "SelectBox";
                                break;

                            case ContactForm_Configuration_Element_Type_Enum.Subject:
                                elementName = "Subject";
                                break;

                            case ContactForm_Configuration_Element_Type_Enum.TextArea:
                                elementName = "TextArea";
                                break;

                            case ContactForm_Configuration_Element_Type_Enum.TextBox:
                                elementName = "TextBox";
                                break;
                        }
                        writer.Write("\t\t\t<" + elementName );
                        if (!String.IsNullOrEmpty(thisElement.Name))
                            writer.Write(" Name=\"" + thisElement.Name + "\"");

                        if (!String.IsNullOrEmpty(thisElement.QueryText.DefaultValue))
                        {
                            if ( thisElement.Element_Type != ContactForm_Configuration_Element_Type_Enum.ExplanationText )
                                writer.Write(" Query=\"" + Convert_String_To_XML_Safe(thisElement.QueryText.DefaultValue) + "\"");
                            else
                                writer.Write(" Text=\"" + Convert_String_To_XML_Safe(thisElement.QueryText.DefaultValue) + "\"");
                        }
                        if (!String.IsNullOrEmpty(thisElement.CssClass))
                            writer.Write(" CssClass=\"" + thisElement.CssClass + "\"");
                        if (thisElement.UserAttribute != Users.User_Object_Attribute_Mapping_Enum.NONE)
                        {
                            writer.Write(" UserAttribute=\"" + Users.User_Object_Attribute_Mapping_Enum_Converter.ToString(thisElement.UserAttribute) + "\"");
                            if (( thisElement.Element_Type != ContactForm_Configuration_Element_Type_Enum.ExplanationText ) && ( thisElement.Element_Type != ContactForm_Configuration_Element_Type_Enum.HiddenValue ))
                                writer.Write(" AlwaysShow=\"" + thisElement.AlwaysShow.ToString().ToLower() + "\"");
                        }
                        if ((thisElement.Element_Type != ContactForm_Configuration_Element_Type_Enum.ExplanationText) && (thisElement.Element_Type != ContactForm_Configuration_Element_Type_Enum.HiddenValue))
                        {
                            if ( thisElement.Required )
                                writer.Write(" Required=\"true\"");
                        }

                        if (((thisElement.Options == null) || (thisElement.Options.Count == 0)) && (thisElement.QueryText.Count <= 1))
                        {
                            writer.WriteLine(" />");
                        }
                        else
                        {
                            writer.WriteLine(">");

                            // Add the query in different languages
                            List<Web_Language_Translation_Value> translations = thisElement.QueryText.Values;
                            if (translations.Count > 0)
                            {
                                writer.WriteLine("\t\t\t\t<Translations>");
                                foreach (Web_Language_Translation_Value thisTranslation in translations)
                                {
                                    writer.WriteLine("\t\t\t\t\t<Language Code=\"" + Web_Language_Enum_Converter.Enum_To_Code( thisTranslation.Language ) + "\">" + Convert_String_To_XML_Safe(thisTranslation.Value) + "</Language>");
                                }
                                writer.WriteLine("\t\t\t\t</Translations>");
                            }

                            // Add the possible options
                            if (( thisElement.Options != null ) && ( thisElement.Options.Count > 0))
                            {
                                writer.WriteLine("\t\t\t\t<Options>");
                                foreach (string thisOption in thisElement.Options)
                                {
                                    writer.WriteLine("\t\t\t\t\t<Option>" + Convert_String_To_XML_Safe(thisOption) + "</Option>");
                                }
                                writer.WriteLine("\t\t\t\t</Options>");
                            }


                            writer.WriteLine("\t\t\t</" + elementName + ">");
                        }
                    }

                    writer.WriteLine("\t\t</Elements>");
                }
                writer.WriteLine("\t</ContactForm>");
                writer.WriteLine("</SobekCM_Config>");
                writer.Flush();
                writer.Close();
            }
            catch (Exception ee)
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
    }
}
