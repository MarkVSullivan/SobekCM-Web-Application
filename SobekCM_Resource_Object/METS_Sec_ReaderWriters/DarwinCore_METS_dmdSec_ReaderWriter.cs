#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SobekCM.Resource_Object.Metadata_Modules;

#endregion

namespace SobekCM.Resource_Object.METS_Sec_ReaderWriters
{
    /// <summary> Taxonomic hierarchy ( DarwinCore) METS subsection reader/writer </summary>
    public class DarwinCore_METS_dmdSec_ReaderWriter : XML_Writing_Base_Type, iPackage_dmdSec_ReaderWriter
    {
        #region iPackage_dmdSec_ReaderWriter Members

        /// <summary> Flag indicates if this active reader/writer will write a dmdSec </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns> TRUE if the package has data to be written, otherwise fALSE </returns>
        public bool Include_dmdSec(SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            Zoological_Taxonomy_Info taxonInfo = METS_Item.Get_Metadata_Module(GlobalVar.ZOOLOGICAL_TAXONOMY_METADATA_MODULE_KEY) as Zoological_Taxonomy_Info;
            if ((taxonInfo == null) || (!taxonInfo.hasData))
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
            Zoological_Taxonomy_Info taxonInfo = METS_Item.Get_Metadata_Module(GlobalVar.ZOOLOGICAL_TAXONOMY_METADATA_MODULE_KEY) as Zoological_Taxonomy_Info;
            if ((taxonInfo == null) || (!taxonInfo.hasData))
                return true;

            Output_Stream.WriteLine("<dwr:SimpleDarwinRecordSet>");
            Output_Stream.WriteLine("<dwr:SimpleDarwinRecord>");

            if (!String.IsNullOrEmpty(taxonInfo.Scientific_Name))
            {
                Output_Stream.WriteLine("<dwc:scientificName>" + Convert_String_To_XML_Safe(taxonInfo.Scientific_Name) + "</dwc:scientificName>");
            }

            if (!String.IsNullOrEmpty(taxonInfo.Higher_Classification))
            {
                Output_Stream.WriteLine("<dwc:higherClassification>" + Convert_String_To_XML_Safe(taxonInfo.Higher_Classification) + "</dwc:higherClassification>");
            }

            if (!String.IsNullOrEmpty(taxonInfo.Kingdom))
            {
                Output_Stream.WriteLine("<dwc:kingdom>" + Convert_String_To_XML_Safe(taxonInfo.Kingdom) + "</dwc:kingdom>");
            }

            if (!String.IsNullOrEmpty(taxonInfo.Phylum))
            {
                Output_Stream.WriteLine("<dwc:phylum>" + Convert_String_To_XML_Safe(taxonInfo.Phylum) + "</dwc:phylum>");
            }

            if (!String.IsNullOrEmpty(taxonInfo.Class))
            {
                Output_Stream.WriteLine("<dwc:class>" + Convert_String_To_XML_Safe(taxonInfo.Class) + "</dwc:class>");
            }

            if (!String.IsNullOrEmpty(taxonInfo.Order))
            {
                Output_Stream.WriteLine("<dwc:order>" + Convert_String_To_XML_Safe(taxonInfo.Order) + "</dwc:order>");
            }

            if (!String.IsNullOrEmpty(taxonInfo.Family))
            {
                Output_Stream.WriteLine("<dwc:family>" + Convert_String_To_XML_Safe(taxonInfo.Family) + "</dwc:family>");
            }

            if (!String.IsNullOrEmpty(taxonInfo.Genus))
            {
                Output_Stream.WriteLine("<dwc:genus>" + Convert_String_To_XML_Safe(taxonInfo.Genus) + "</dwc:genus>");
            }

            if (!String.IsNullOrEmpty(taxonInfo.Specific_Epithet))
            {
                Output_Stream.WriteLine("<dwc:specificEpithet>" + Convert_String_To_XML_Safe(taxonInfo.Specific_Epithet) + "</dwc:specificEpithet>");
            }

            if (!String.IsNullOrEmpty(taxonInfo.Taxonomic_Rank))
            {
                Output_Stream.WriteLine("<dwc:taxonRank>" + Convert_String_To_XML_Safe(taxonInfo.Taxonomic_Rank) + "</dwc:taxonRank>");
            }

            if (!String.IsNullOrEmpty(taxonInfo.Common_Name))
            {
                Output_Stream.WriteLine("<dwc:vernacularName>" + Convert_String_To_XML_Safe(taxonInfo.Common_Name) + "</dwc:vernacularName>");
            }

            Output_Stream.WriteLine("</dwr:SimpleDarwinRecord>");
            Output_Stream.WriteLine("</dwr:SimpleDarwinRecordSet>");
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
            Zoological_Taxonomy_Info taxonInfo = Return_Package.Get_Metadata_Module(GlobalVar.ZOOLOGICAL_TAXONOMY_METADATA_MODULE_KEY) as Zoological_Taxonomy_Info;
            if (taxonInfo == null)
            {
                taxonInfo = new Zoological_Taxonomy_Info();
                Return_Package.Add_Metadata_Module(GlobalVar.ZOOLOGICAL_TAXONOMY_METADATA_MODULE_KEY, taxonInfo);
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
                    if (name.IndexOf("dwc:") == 0)
                        name = name.Substring(4);

                    switch (name)
                    {
                        case "scientificname":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                taxonInfo.Scientific_Name = Input_XmlReader.Value.Trim();
                            }
                            break;

                        case "higherclassification":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                taxonInfo.Higher_Classification = Input_XmlReader.Value.Trim();
                            }
                            break;

                        case "kingdom":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                taxonInfo.Kingdom = Input_XmlReader.Value;
                            }
                            break;

                        case "phylum":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                taxonInfo.Phylum = Input_XmlReader.Value;
                            }
                            break;

                        case "class":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                taxonInfo.Class = Input_XmlReader.Value;
                            }
                            break;

                        case "order":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                taxonInfo.Order = Input_XmlReader.Value;
                            }
                            break;

                        case "family":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                taxonInfo.Family = Input_XmlReader.Value;
                            }
                            break;

                        case "genus":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                taxonInfo.Genus = Input_XmlReader.Value;
                            }
                            break;

                        case "specificepithet":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                taxonInfo.Specific_Epithet = Input_XmlReader.Value;
                            }
                            break;

                        case "taxonrank":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                taxonInfo.Taxonomic_Rank = Input_XmlReader.Value;
                            }
                            break;

                        case "vernacularname":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                taxonInfo.Common_Name = Input_XmlReader.Value;
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
            Zoological_Taxonomy_Info taxonInfo = METS_Item.Get_Metadata_Module(GlobalVar.ZOOLOGICAL_TAXONOMY_METADATA_MODULE_KEY) as Zoological_Taxonomy_Info;
            if (taxonInfo == null)
                return false;

            return taxonInfo.hasData;
        }

        /// <summary> Returns the schema namespace (xmlns) information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema namespace info for the METS header</returns>
        public string[] Schema_Namespace(SobekCM_Item METS_Item)
        {
            return new string[] {"dwc=\"http://rs.tdwg.org/dwc/terms/\"", "dwr=\"http://rs.tdwg.org/dwc/xsd/simpledarwincore/\""};
        }

        /// <summary> Returns the schema location information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema location for the METS header</returns>
        public string[] Schema_Location(SobekCM_Item METS_Item)
        {
            return new string[]
                       {
                           "    http://rs.tdwg.org/dwc/terms/\r\n    http://rs.tdwg.org/dwc/xsd/tdwg_dwcterms.xsd",
                           "    http://rs.tdwg.org/dwc/xsd/simpledarwincore/\r\n    http://rs.tdwg.org/dwc/xsd/tdwg_dwc_simple.xsd"
                       };
        }

        #endregion
    }
}