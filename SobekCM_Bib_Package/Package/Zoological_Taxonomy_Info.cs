using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package
{
    /// <summary> Class stores taxonomic information for resources which have 
    /// reference to one or more taxonomic records </summary>
    [Serializable]
    public class Zoological_Taxonomy_Info : XML_Writing_Base_Type
    {
        private string scientificName, higherClassification, kingdom, phylum, classs,
            order, family, genus, specificEpithet, taxonRank, commonName;

        /// <summary> Constructor for a new instance of the Zoological_Taxonomy_Info class </summary>
        public Zoological_Taxonomy_Info()
        {
            // Do nothing
        }

        #region Simple public properties

        /// <summary> Gets or sets scientific name associated with this zoological taxonomy </summary>
        public string Scientific_Name
        {
            get { return scientificName ?? String.Empty; }
            set { scientificName = value; }
        }

        /// <summary> Gets or sets higherClassification associated with this zoological taxonomy </summary>
        public string Higher_Classification
        {
            get { return higherClassification ?? String.Empty; }
            set { higherClassification = value; }
        }

        /// <summary> Gets or sets the kingdom classification associated with this zoological taxonomy </summary>
        public string Kingdom
        {
            get { return kingdom ?? String.Empty; }
            set { kingdom = value; }
        }

        /// <summary> Gets or sets the phylum classification associated with this zoological taxonomy </summary>
        public string Phylum
        {
            get { return phylum ?? String.Empty; }
            set { phylum = value; }
        }

        /// <summary> Gets or sets the class classification associated with this zoological taxonomy </summary>
        public string Class
        {
            get { return classs ?? String.Empty; }
            set { classs = value; }
        }

        /// <summary> Gets or sets the order classification associated with this zoological taxonomy </summary>
        public string Order
        {
            get { return order ?? String.Empty; }
            set { order = value; }
        }

        /// <summary> Gets or sets the family classification associated with this zoological taxonomy </summary>
        public string Family
        {
            get { return family ?? String.Empty; }
            set { family = value; }
        }

        /// <summary> Gets or sets the genus classification associated with this zoological taxonomy </summary>
        public string Genus
        {
            get { return genus ?? String.Empty; }
            set { genus = value; }
        }

        /// <summary> Gets or sets the name of the first or species epithet of the scientificName. </summary>
        public string Specific_Epithet
        {
            get { return specificEpithet ?? String.Empty; }
            set { specificEpithet = value; }

        }

        /// <summary> Gets or sets the taxonomic rank of the most specific name in the scientificName as it appears in the original record. </summary>
        public string Taxonomic_Rank
        {
            get { return taxonRank ?? String.Empty; }
            set { taxonRank = value; }
        }

        /// <summary> Gets or sets the common or vernacular name associated with this zoological taxonomy </summary>
        public string Common_Name
        {
            get { return commonName ?? String.Empty; }
            set { commonName = value; }
        }

        #endregion

        /// <summary> Flag indicates if this object holds any data in the subfields </summary>
        public bool hasData
        {
            get
            {
                if ((!String.IsNullOrEmpty(scientificName)) || (!String.IsNullOrEmpty(higherClassification)) ||
                    (!String.IsNullOrEmpty(kingdom)) || (!String.IsNullOrEmpty(phylum)) || (!String.IsNullOrEmpty(classs)) ||
                    (!String.IsNullOrEmpty(order)) || (!String.IsNullOrEmpty(family)) || (!String.IsNullOrEmpty(genus)) ||
                    (!String.IsNullOrEmpty(specificEpithet)) || (!String.IsNullOrEmpty(taxonRank)) || (!String.IsNullOrEmpty(commonName)))
                    return true;
                else
                    return false;
            }
        }

        /// <summary> Outputs this zoological taxonomy as a DarwinCore-compliant XML SimpleDarwinRecordSet </summary>
        public string To_Simple_Darwin_Core_RecordSet
        {
            get
            {
                StringBuilder resultsBuilder = new StringBuilder();

                resultsBuilder.AppendLine("<dwr:SimpleDarwinRecordSet>");
                resultsBuilder.AppendLine("<dwr:SimpleDarwinRecord>");

                if (!String.IsNullOrEmpty(scientificName))
                {
                    resultsBuilder.AppendLine("<dwc:scientificName>" + base.Convert_String_To_XML_Safe(scientificName) + "</dwc:scientificName>");
                }

                if (!String.IsNullOrEmpty(higherClassification))
                {
                    resultsBuilder.AppendLine("<dwc:higherClassification>" + base.Convert_String_To_XML_Safe(higherClassification) + "</dwc:higherClassification>");
                }

                if (!String.IsNullOrEmpty(kingdom))
                {
                    resultsBuilder.AppendLine("<dwc:kingdom>" + base.Convert_String_To_XML_Safe(kingdom) + "</dwc:kingdom>");
                }

                if (!String.IsNullOrEmpty(phylum))
                {
                    resultsBuilder.AppendLine("<dwc:phylum>" + base.Convert_String_To_XML_Safe(phylum) + "</dwc:phylum>");
                }

                if (!String.IsNullOrEmpty(classs))
                {
                    resultsBuilder.AppendLine("<dwc:class>" + base.Convert_String_To_XML_Safe(classs) + "</dwc:class>");
                }

                if (!String.IsNullOrEmpty(order))
                {
                    resultsBuilder.AppendLine("<dwc:order>" + base.Convert_String_To_XML_Safe(order) + "</dwc:order>");
                }

                if (!String.IsNullOrEmpty(family))
                {
                    resultsBuilder.AppendLine("<dwc:family>" + base.Convert_String_To_XML_Safe(family) + "</dwc:family>");
                }

                if (!String.IsNullOrEmpty(genus))
                {
                    resultsBuilder.AppendLine("<dwc:genus>" + base.Convert_String_To_XML_Safe(genus) + "</dwc:genus>");
                }

                if (!String.IsNullOrEmpty(specificEpithet))
                {
                    resultsBuilder.AppendLine("<dwc:specificEpithet>" + base.Convert_String_To_XML_Safe(specificEpithet) + "</dwc:specificEpithet>");
                }

                if (!String.IsNullOrEmpty(taxonRank))
                {
                    resultsBuilder.AppendLine("<dwc:taxonRank>" + base.Convert_String_To_XML_Safe(taxonRank) + "</dwc:taxonRank>");
                }

                if (!String.IsNullOrEmpty(commonName))
                {
                    resultsBuilder.AppendLine("<dwc:vernacularName>" + base.Convert_String_To_XML_Safe(commonName) + "</dwc:vernacularName>");
                }

                resultsBuilder.AppendLine("</dwr:SimpleDarwinRecord>");
                resultsBuilder.AppendLine("</dwr:SimpleDarwinRecordSet>");

                return resultsBuilder.ToString();
            }
        }
    }
}
