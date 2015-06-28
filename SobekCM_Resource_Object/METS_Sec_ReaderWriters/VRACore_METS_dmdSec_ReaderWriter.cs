#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.VRACore;

#endregion

namespace SobekCM.Resource_Object.METS_Sec_ReaderWriters
{
    /// <summary> Class is used to read and write VRACore metadata developed by the Visual Resource
    /// Association for visual materials, such as museum artifacts.  This reader/writer is used to read and
    /// write this data as a dmdSec sections within a METS file.  </summary>
    /// <remarks> Previously, the VRACore information was encoded within the MODS dmdSec in the
    /// SobekCM METS files within the extension tag.  As such, the MODS reader/writer continues to 
    /// look for and read any VRACore data within a MODS files.  However, the default MODS
    /// writer will not write the data, as it should be written here within its own dmdSec.  </remarks>
    public class VRACore_METS_dmdSec_ReaderWriter : XML_Writing_Base_Type, iPackage_dmdSec_ReaderWriter
    {
        #region iPackage_dmdSec_ReaderWriter Members

        /// <summary> Flag indicates if this active reader/writer will write a dmdSec </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <param name="Options"> Instance wide options related to saving this item </param>
        /// <returns> TRUE if the package has data to be written, otherwise fALSE </returns>
        public bool Include_dmdSec(SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            VRACore_Info vraInfo = METS_Item.Get_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY) as VRACore_Info;
            if ((vraInfo == null) || (!vraInfo.hasData))
                return false;
            return true;
        }

        /// <summary> Writes the dmdSec for the entire package to the text writer </summary>
        /// <param name="Output_Stream">Stream to which the formatted text is written </param>
        /// <param name="METS_Item">Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_dmdSec(TextWriter Output_Stream, SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            VRACore_Info vraInfo = METS_Item.Get_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY) as VRACore_Info;
            if ((vraInfo == null) || (!vraInfo.hasData))
                return false;

            Output_Stream.WriteLine("<vra:vra>");
            if (vraInfo.Cultural_Context_Count > 0)
            {
                foreach (string thisValue in vraInfo.Cultural_Contexts)
                {
                    Output_Stream.WriteLine("<vra:culturalContext>" + Convert_String_To_XML_Safe(thisValue) + "</vra:culturalContext>");
                }
            }
            if (vraInfo.Inscription_Count > 0)
            {
                foreach (string thisValue in vraInfo.Inscriptions)
                {
                    Output_Stream.WriteLine("<vra:inscription>" + Convert_String_To_XML_Safe(thisValue) + "</vra:inscription>");
                }
            }
            if (vraInfo.Material_Count > 0)
            {
                foreach (VRACore_Materials_Info thisValue in vraInfo.Materials)
                {
                    if (thisValue.Type.Length > 0)
                    {
                        Output_Stream.WriteLine("<vra:material type=\"" + Convert_String_To_XML_Safe(thisValue.Type) + "\">" + Convert_String_To_XML_Safe(thisValue.Materials) + "</vra:material>");
                    }
                    else
                    {
                        Output_Stream.WriteLine("<vra:material>" + Convert_String_To_XML_Safe(thisValue.Materials) + "</vra:material>");
                    }
                }
            }
            if (vraInfo.Measurement_Count > 0)
            {
                foreach (VRACore_Measurement_Info thisValue in vraInfo.Measurements)
                {
                    if (thisValue.Units.Length > 0)
                    {
                        Output_Stream.WriteLine("<vra:measurements unit=\"" + Convert_String_To_XML_Safe(thisValue.Units) + "\">" + Convert_String_To_XML_Safe(thisValue.Measurements) + "</vra:measurements>");
                    }
                    else
                    {
                        Output_Stream.WriteLine("<vra:measurements>" + Convert_String_To_XML_Safe(thisValue.Measurements) + "</vra:measurements>");
                    }
                }
            }
            if (vraInfo.State_Edition_Count > 0)
            {
                foreach (string thisValue in vraInfo.State_Editions)
                {
                    Output_Stream.WriteLine("<vra:stateEdition>" + Convert_String_To_XML_Safe(thisValue) + "</vra:stateEdition>");
                }
            }
            if (vraInfo.Style_Period_Count > 0)
            {
                foreach (string thisValue in vraInfo.Style_Periods)
                {
                    Output_Stream.WriteLine("<vra:stylePeriod>" + Convert_String_To_XML_Safe(thisValue) + "</vra:stylePeriod>");
                }
            }
            if (vraInfo.Technique_Count > 0)
            {
                foreach (string thisValue in vraInfo.Techniques)
                {
                    Output_Stream.WriteLine("<vra:technique>" + Convert_String_To_XML_Safe(thisValue) + "</vra:technique>");
                }
            }
            Output_Stream.WriteLine("</vra:vra>");

            return true;
        }

        /// <summary> Reads the dmdSec at the current position in the XmlTextReader and associates it with the 
        /// entire package  </summary>
        /// <param name="Input_XmlReader"> Open XmlReader from which to read the metadata </param>
        /// <param name="Return_Package"> Package into which to read the metadata</param>
        /// <param name="Options"> Dictionary of any options which this METS section reader may utilize</param>
        /// <returns> TRUE if successful, otherwise FALSE</returns>
        public bool Read_dmdSec(XmlReader Input_XmlReader, SobekCM_Item Return_Package, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists
            VRACore_Info vraInfo = Return_Package.Get_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY) as VRACore_Info;
            if (vraInfo == null)
            {
                vraInfo = new VRACore_Info();
                Return_Package.Add_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY, vraInfo);                   
            }

            // Loop through reading each XML node
            do
            {
                // If this is the end of this section, return
                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && ((Input_XmlReader.Name == "METS:mdWrap") || (Input_XmlReader.Name == "mdWrap")))
                    return true;

                // get the right division information based on node type
                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                {
                    string name = Input_XmlReader.Name.ToLower();
                    if (name.IndexOf("vra:") == 0)
                        name = name.Substring(4);

                    switch (name)
                    {
                        case "culturalcontext":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                if (Input_XmlReader.Value.Length > 0)
                                    vraInfo.Add_Cultural_Context(Input_XmlReader.Value);
                            }
                            break;

                        case "inscription":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                if (Input_XmlReader.Value.Length > 0)
                                    vraInfo.Add_Inscription(Input_XmlReader.Value);
                            }
                            break;

                        case "material":
                            string type = String.Empty;
                            if (Input_XmlReader.HasAttributes)
                            {
                                if (Input_XmlReader.MoveToAttribute("type"))
                                    type = Input_XmlReader.Value;
                            }
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                if (Input_XmlReader.Value.Length > 0)
                                    vraInfo.Add_Material(Input_XmlReader.Value, type);
                            }
                            break;

                        case "measurements":
                            string units = String.Empty;
                            if (Input_XmlReader.HasAttributes)
                            {
                                if (Input_XmlReader.MoveToAttribute("unit"))
                                    units = Input_XmlReader.Value;
                            }
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                if (Input_XmlReader.Value.Length > 0)
                                    vraInfo.Add_Measurement(Input_XmlReader.Value, units);
                            }
                            break;

                        case "stateedition":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                if (Input_XmlReader.Value.Length > 0)
                                    vraInfo.Add_State_Edition(Input_XmlReader.Value);
                            }
                            break;

                        case "styleperiod":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                if (Input_XmlReader.Value.Length > 0)
                                    vraInfo.Add_Style_Period(Input_XmlReader.Value);
                            }
                            break;

                        case "technique":
                            Input_XmlReader.Read();
                            if (Input_XmlReader.NodeType == XmlNodeType.Text)
                            {
                                if (Input_XmlReader.Value.Length > 0)
                                    vraInfo.Add_Technique(Input_XmlReader.Value);
                            }
                            break;
                    }
                }
            } while (Input_XmlReader.Read());

            return true;
        }

        /// <summary> Flag indicates if this active reader/writer needs to append schema reference information
        /// to the METS XML header by analyzing the contents of the digital resource item </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> TRUE if the schema should be attached, otherwise fALSE </returns>
        public bool Schema_Reference_Required_Package(SobekCM_Item METS_Item)
        {
            VRACore_Info vraInfo = METS_Item.Get_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY) as VRACore_Info;
            if (vraInfo == null) 
                return false;
            return vraInfo.hasData;
        }

        /// <summary> Returns the schema namespace (xmlns) information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema namespace info for the METS header</returns>
        public string[] Schema_Namespace(SobekCM_Item METS_Item)
        {
            return new string[] { "vra=\"http://www.loc.gov/standards/vracore/vra.xsd\"" };
        }

        /// <summary> Returns the schema location information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema location for the METS header</returns>
        public string[] Schema_Location(SobekCM_Item METS_Item)
        {
            return new string[] {  };
        }

        #endregion
    }
}
