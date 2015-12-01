#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Configuration.OAIPMH
{
    /// <summary> Configuration inforation for serving OAI-PMH from this instance </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("OaiPmhConfig")]
    public class OAI_PMH_Configuration
    {
        /// <summary> OAI-PMG configuration information for this SobekCM instance </summary>
        public OAI_PMH_Configuration()
        {
            Metadata_Prefixes = new List<OAI_PMH_Metadata_Format>();
            Enabled = true;
        }

        /// <summary> Base for the identifiers associated with items within this repository </summary>
        [DataMember(Name = "identifierBase", EmitDefaultValue = false)]
        [XmlAttribute("identifierBase")]
        [ProtoMember(1)]
        public string Identifier_Base { get; set; }

        /// <summary> Flag indicates if OAI-PMH is allowed to be enabled for collections at all </summary>
        /// <remarks> The default value is TRUE, but can be overidden in the XML  </remarks>
        [DataMember(Name = "enabled")]
        [XmlAttribute("enabled")]
        [ProtoMember(2)]
        public bool Enabled { get; set; }

        /// <summary> Name of this repository for dispay within the Identify response of the OAI-PMG protocol </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        [XmlAttribute("name")]
        [ProtoMember(3)]
        public string Name { get; set;  }

        /// <summary> Identifier for this repository for display within the Identify response of the OAI-PMH protocol </summary>
        [DataMember(Name = "identifier", EmitDefaultValue = false)]
        [XmlAttribute("identifier")]
        [ProtoMember(4)]
        public string Identifier { get; set;  }

        /// <summary> List of the administrative emails for display within the Identify response of the OAI-PMH protocol </summary>
        [DataMember(Name = "adminEmails")]
        [XmlArray("adminEmails")]
        [XmlArrayItem("email", typeof(string))]
        [ProtoMember(5)]
        public List<string> Admin_Emails { get; private set; }

        /// <summary> List of additional descriptions (fully encoded as XML) for display within the Identify response of the OAI-PMG protocol </summary>
        [DataMember(Name = "descriptions")]
        [XmlArray("descriptions")]
        [XmlArrayItem("description", typeof(string))]
        [ProtoMember(6)]
        public List<string> Descriptions { get; private set;  }

        /// <summary> List of all the metadata formats available for this repository with pointers
        /// to the classes that create the OAI-PMH metadata for harvesting  </summary>
        [DataMember(Name = "metadataFormats")]
        [XmlArray("metadataFormats")]
        [XmlArrayItem("format", typeof(OAI_PMH_Metadata_Format))]
        [ProtoMember(7)]
        public List<OAI_PMH_Metadata_Format> Metadata_Prefixes { get; private set; }

        /// <summary> Any error associated with reading the configuration file into this object </summary>
        [DataMember(Name = "error", EmitDefaultValue = false)]
        [XmlAttribute("error")]
        [ProtoMember(8)]
        public string Error { get; set; }

        /// <summary> Add a new admin email address within the Identify response of the OAI-PMH protocol </summary>
        /// <param name="AdminEmail"> New admin email to add to the list </param>
        public void Add_Admin_Email(string AdminEmail)
        {
            if (Admin_Emails == null)
                Admin_Emails = new List<string>();

            string[] split = AdminEmail.Split("|,;".ToCharArray());
            foreach (string thisSplit in split)
            {
                string thisSplitTrim = thisSplit.Trim();
                if (!String.IsNullOrWhiteSpace(thisSplitTrim))
                {
                    if (!(Admin_Emails.Exists(s => s.IndexOf(thisSplitTrim, StringComparison.OrdinalIgnoreCase) >= 0)))
                        Admin_Emails.Add(thisSplitTrim);
                }
            }
        }

        /// <summary> Add a new description to be included within the Identify response of the OAI-PMH protocol </summary>
        /// <param name="NewDescription"> New custom description to be included </param>
        public void Add_Description(string NewDescription)
        {
            if (Descriptions == null)
                Descriptions = new List<string>();
            if (!(Descriptions.Exists(s => s.IndexOf(NewDescription, StringComparison.OrdinalIgnoreCase) >= 0)))
                Descriptions.Add(NewDescription);
        }

        #region Code to save this oai-pmh configuration to a XML file

        /// <summary> Save this OAI-PMH configuration to a XML config file </summary>
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
                writer.WriteLine("<Config>");

                writer.WriteLine(Enabled ? "\t<OAI-PMH Enabled=\"true\">" : "\t<OAI-PMH Enabled=\"false\">");

                writer.WriteLine("\t\t<Repository IdentifierBase=\"" + Identifier_Base + "\" />");

                // Add the basic identify information
                writer.WriteLine("\t\t<Identify>");
                writer.WriteLine("\t\t\t<Name>" + Convert_String_To_XML_Safe(Name) + "</Name>");
                writer.WriteLine("\t\t\t<Identifier>" + Convert_String_To_XML_Safe(Identifier) + "</Identifier>");

                if ((Admin_Emails != null) && (Admin_Emails.Count > 0))
                {
                    foreach (string thisEmail in Admin_Emails)
                    {
                        writer.WriteLine("\t\t\t<AdminEmail>" + Convert_String_To_XML_Safe(thisEmail) + "</AdminEmail>");
                    }
                }

                if ((Descriptions != null) && (Descriptions.Count > 0))
                {
                    foreach (string description in Descriptions)
                    {
                        writer.WriteLine("\t\t\t<Description>" + Convert_String_To_XML_Safe(description) + "</Description>");
                    }
                }
                writer.WriteLine("\t\t</Identify>");

                // Add the metadata prefixes and the associated assembly/class reference
                if ((Metadata_Prefixes != null) && (Metadata_Prefixes.Count > 0))
                {
                    writer.WriteLine("\t\t<MetadataPrefixes>");

                    foreach (OAI_PMH_Metadata_Format metadataFormat in Metadata_Prefixes)
                    {
                        writer.Write("\t\t\t<MetadataFormat Prefix=\"" + metadataFormat.Prefix + "\" Schema=\"" + metadataFormat.Schema + "\" MetadataNamespace=\"" + metadataFormat.MetadataNamespace + "\"");
                        if ( !String.IsNullOrEmpty(metadataFormat.Assembly))
                            writer.Write(" Assembly=\"" + Convert_String_To_XML_Safe(metadataFormat.Assembly) + "\"");
                        writer.WriteLine(" Namespace=\"" + metadataFormat.Namespace + "\" Class=\"" + metadataFormat.Class + "\" Enabled=\"" + metadataFormat.Enabled.ToString().ToLower() + "\" />");
                    }

                    writer.WriteLine("\t\t</MetadataPrefixes>");
                }


                writer.WriteLine("\t</OAI-PMH>");
                writer.WriteLine("</Config>");
                writer.Flush();
                writer.Close();
            }
            catch
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

        #region Code to set the default configuration ( used if no config file found )

        /// <summary> Sets the defautls for the metadata prefixes ( dublin core and marc21 ) </summary>
        public void Set_Default()
        {
            OAI_PMH_Metadata_Format dcFormat = new OAI_PMH_Metadata_Format
            {
                Class = "DC_OAI_Metadata_Type_Writer", 
                Enabled = true,
                MetadataNamespace = "http://www.openarchives.org/OAI/2.0/oai_dc/",
                Namespace = "SobekCM.Resource_Object.OAI.Writer",
                Prefix = "oai_dc",
                Schema = "http://www.openarchives.org/OAI/2.0/oai_dc.xsd"
            };
            Metadata_Prefixes.Add(dcFormat);

            OAI_PMH_Metadata_Format marcFormat = new OAI_PMH_Metadata_Format
            {
                Class = "MarcXML_OAI_PMH_Metadata_Type_Writer",
                Enabled = true,
                MetadataNamespace = "http://www.loc.gov/MARC21/slim",
                Namespace = "SobekCM.Resource_Object.OAI.Writer",
                Prefix = "marc21",
                Schema = "http://www.loc.gov/standards/marcxml/schema/MARC21slim.xsd"
            };
            Metadata_Prefixes.Add(marcFormat);

        }

        #endregion
    }
}
