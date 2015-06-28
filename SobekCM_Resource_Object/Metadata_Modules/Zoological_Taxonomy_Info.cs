#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules
{
    /// <summary> Class stores taxonomic information for resources which have 
    /// reference to one or more taxonomic records </summary>
    [Serializable]
    public class Zoological_Taxonomy_Info : iMetadata_Module
    {
        private string classs;
        private string commonName;
        private string family, genus;
        private string higherClassification, kingdom;
        private string order;
        private string phylum;
        private string scientificName;
        private string specificEpithet, taxonRank;

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
                return (!String.IsNullOrEmpty(scientificName)) || (!String.IsNullOrEmpty(higherClassification)) ||
                       (!String.IsNullOrEmpty(kingdom)) || (!String.IsNullOrEmpty(phylum)) || (!String.IsNullOrEmpty(classs)) ||
                       (!String.IsNullOrEmpty(order)) || (!String.IsNullOrEmpty(family)) || (!String.IsNullOrEmpty(genus)) ||
                       (!String.IsNullOrEmpty(specificEpithet)) || (!String.IsNullOrEmpty(taxonRank)) || (!String.IsNullOrEmpty(commonName));
            }
        }

        #region Methods/Properties to implement the iMetadata_Module interface

        /// <summary> Name for this metadata module </summary>
        /// <value> This always returns 'ZoologicalTaxonomy'</value>
        public string Module_Name
        {
            get { return GlobalVar.ZOOLOGICAL_TAXONOMY_METADATA_MODULE_KEY; }
        }

        /// <summary> Gets the metadata search terms and values to be saved to the database
        /// to allow searching to occur over the data in this metadata module </summary>
        public List<KeyValuePair<string, string>> Metadata_Search_Terms
        {
            get
            {
                List<KeyValuePair<string, string>> metadataTerms = new List<KeyValuePair<string, string>>();

                // Add the kingdom
                if ( !String.IsNullOrEmpty(kingdom))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT Kingdom", kingdom));
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT All Taxonomy", kingdom));
                }

                // Add the phylum
                if (!String.IsNullOrEmpty(phylum))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT Phylum", phylum));
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT All Taxonomy", phylum));
                }

                // Add the class
                if (!String.IsNullOrEmpty(classs))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT Class", classs));
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT All Taxonomy", classs));
                }

                // Add the order
                if (!String.IsNullOrEmpty(order))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT Order", order));
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT All Taxonomy", order));
                }

                // Add the kingdom
                if (!String.IsNullOrEmpty(family))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT Family", family));
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT All Taxonomy", family));
                }

                // Add the genus
                if (!String.IsNullOrEmpty(genus))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT Genus", genus));
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT All Taxonomy", genus));
                }

                // Add the species
                if (!String.IsNullOrEmpty(specificEpithet))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT Species", specificEpithet));
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT All Taxonomy", specificEpithet));
                }

                // Add the common name
                if (!String.IsNullOrEmpty(commonName))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT Common Name", commonName));
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT All Taxonomy", commonName));
                }

                // Add the common name
                if ((!String.IsNullOrEmpty(genus)) && (!String.IsNullOrEmpty(specificEpithet)))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT Scientific Name", genus + " " + specificEpithet ));
                    metadataTerms.Add(new KeyValuePair<string, string>("ZT All Taxonomy", genus + " " + specificEpithet));
                }

                return metadataTerms;
            }
        }

        /// <summary> Chance for this metadata module to perform any additional database work
        /// such as saving digital resource data into custom tables </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString"> Connection string for the current database </param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        /// <remarks> This module currently  does no additional processing in this method </remarks>
        public bool Save_Additional_Info_To_Database(int ItemID, string DB_ConnectionString, SobekCM_Item BibObject, out string Error_Message)
        {
            // Set the default error mesasge
            Error_Message = String.Empty;

            return true;
        }

        /// <summary> Chance for this metadata module to load any additional data from the 
        /// database when building this digital resource  in memory </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString">Connection string for the current database</param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        /// <remarks> This module currently  does no additional processing in this method </remarks>
        public bool Retrieve_Additional_Info_From_Database(int ItemID, string DB_ConnectionString, SobekCM_Item BibObject, out string Error_Message)
        {
            // Set the default error mesasge
            Error_Message = String.Empty;

            return true;
        }

        #endregion

        /// <summary> Return this zoological taxonomy as a single string </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (!String.IsNullOrEmpty(kingdom))
                builder.Append(kingdom);
            if (!String.IsNullOrEmpty(phylum))
            {
                if (builder.Length > 0)
                    builder.Append(" -- ");
                builder.Append(phylum);
            }
            if (!String.IsNullOrEmpty(classs))
            {
                if (builder.Length > 0)
                    builder.Append(" -- ");
                builder.Append(classs);
            }
            if (!String.IsNullOrEmpty(phylum))
            {
                if (builder.Length > 0)
                    builder.Append(" -- ");
                builder.Append(phylum);
            }
            if (!String.IsNullOrEmpty(order))
            {
                if (builder.Length > 0)
                    builder.Append(" -- ");
                builder.Append(order);
            }
            if (!String.IsNullOrEmpty(family))
            {
                if (builder.Length > 0)
                    builder.Append(" -- ");
                builder.Append(family);
            }
            if (!String.IsNullOrEmpty(genus))
            {
                if (builder.Length > 0)
                    builder.Append(" -- ");
                builder.Append(genus);
            }
            if (!String.IsNullOrEmpty(specificEpithet))
            {
                if (builder.Length > 0)
                    builder.Append(" -- ");
                builder.Append(specificEpithet);
            }
            if (builder.Length > 0 )
                return builder.ToString();
            if (!String.IsNullOrEmpty(commonName))
                return commonName;
            return String.Empty;
        }

        /// <summary> Clear all the data withiin this zoological taxonomy object </summary>
        public void Clear()
        {
            classs = String.Empty;
            commonName = String.Empty;
            family = String.Empty;
            genus = String.Empty;
            higherClassification = String.Empty;
            kingdom = String.Empty;
            order = String.Empty;
            phylum = String.Empty;
            scientificName = String.Empty;
            specificEpithet = String.Empty;
            taxonRank = String.Empty;

        }
    }
}