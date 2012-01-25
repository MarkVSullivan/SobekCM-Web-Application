using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SobekCM.Bib_Package;

namespace SobekCM.Bib_Package.Readers.SubReaders
{
    /// <summary> Subreader reads a DarwinCore-compliant zoological taxonomy section of XML and stores the data in the provided digital resource object </summary>
    public class DarwinCore_SubReader
    {
        /// <summary> Reads the DarwinCore-compliant zoological taxonomy section of XML and stores the data in the provided digital resource </summary>
        /// <param name="r"> XmlTextReader from which to read the daitss data </param>
        /// <param name="package"> Digital resource object to save the data to </param>
        public static void Read_DarwinCore_Sec(XmlTextReader r, SobekCM_Item package)
        {
            // Loop through reading each XML node
            do
            {
                // If this is the end of this section, return
                if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "METS:mdWrap") || (r.Name == "mdWrap")))
                    return;

                // get the right division information based on node type
                if (r.NodeType == XmlNodeType.Element)
                {
                    string name = r.Name.ToLower();
                    if (name.IndexOf("dwc:") == 0)
                        name = name.Substring(4);

                    switch (name)
                    {
                        case "scientificname":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Zoological_Taxonomy.Scientific_Name = r.Value.Trim();
                            }
                            break;

                        case "higherclassification":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Zoological_Taxonomy.Higher_Classification = r.Value.Trim();
                            }
                            break;

                        case "kingdom":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Zoological_Taxonomy.Kingdom = r.Value;
                            }
                            break;

                        case "phylum":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Zoological_Taxonomy.Phylum = r.Value;
                            }
                            break;

                        case "class":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Zoological_Taxonomy.Class = r.Value;
                            }
                            break;

                        case "order":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Zoological_Taxonomy.Order = r.Value;
                            }
                            break;

                        case "family":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Zoological_Taxonomy.Family = r.Value;
                            }
                            break;

                        case "genus":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Zoological_Taxonomy.Genus = r.Value;
                            }
                            break;

                        case "specificepithet":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Zoological_Taxonomy.Specific_Epithet = r.Value;
                            }
                            break;

                        case "taxonrank":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Zoological_Taxonomy.Taxonomic_Rank = r.Value;
                            }
                            break;

                        case "vernacularname":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Zoological_Taxonomy.Common_Name = r.Value;
                            }
                            break;

                    }
                }

            } while (r.Read());
        }
    }
}
