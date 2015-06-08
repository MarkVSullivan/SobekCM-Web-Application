#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Results
{
    /// <summary> Contains all the top-level information about a set of search results, while not containing any of the actual pages of results </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("resultSetStatistics")]
    public class Search_Results_Statistics
    {
	    private List<string> metadataLabels;

        /// <summary> Constructor for a new instance of the Search_Results_Statistics class </summary>
        public Search_Results_Statistics()
        {
            // Create the facet lists
            Aggregation_Facets = new List<Search_Facet_Aggregation>();
            Facet_Collections = new List<Search_Facet_Collection>();

            Total_Items = -1;
            Total_Titles = -1;
            All_Collections_Items = -1;
            All_Collections_Titles = -1;
            QueryTime = 0;
        }
 
        /// <summary> Constructor for a new instance of the Search_Results_Statistics class </summary>
        /// <param name="Metadata_Labels"> List of the metadata terms for each metadata value in the results </param>
		public Search_Results_Statistics(List<string> Metadata_Labels)
        {
            // Create the facet lists
            Aggregation_Facets = new List<Search_Facet_Aggregation>();
            Facet_Collections = new List<Search_Facet_Collection>();

            Total_Items = -1;
            Total_Titles = -1;
            All_Collections_Items = -1;
            All_Collections_Titles = -1;
            QueryTime = 0;

	        metadataLabels = Metadata_Labels;
        }

        /// <summary> Constructor for a new instance of the Search_Results_Statistics class </summary>
        /// <param name="Facet_Data"> DataSet containing the facets data to include within this class</param>
        /// <param name="Facet_Types"> Types of facets requested from the database for the related item aggreagtion </param>
        /// <param name="Total_Items"> Total number of items within the greater set of matching items/titles </param>
        /// <param name="Total_Titles"> Total number of titles within the greater set of matching items/titles</param>
		/// <param name="Metadata_Labels"> List of the metadata terms for each metadata value in the results </param>
		public Search_Results_Statistics(DataSet Facet_Data, List<short> Facet_Types, int Total_Items, int Total_Titles, List<string> Metadata_Labels)
        {            
            // Create the facet lists
            Aggregation_Facets = new List<Search_Facet_Aggregation>();
            Facet_Collections = new List<Search_Facet_Collection>();

            // Save the titles
            this.Total_Titles = Total_Titles;
            this.Total_Items = Total_Items;
            All_Collections_Items = -1;
            All_Collections_Titles = -1;

			metadataLabels = Metadata_Labels;

            // Convert facet table to facet lists
            Convert_Facet_Tables_To_Facet_Lists(Facet_Data, Facet_Types);
        }

        /// <summary> Constructor for a new instance of the Search_Results_Statistics class </summary>
        /// <param name="Facet_Data"> DataReader containg all the facet information to include within this class </param>
        /// <param name="Facet_Types"> Types of facets requested from the database for the related item aggreagtion </param>
		/// <param name="Metadata_Labels"> List of the metadata terms for each metadata value in the results </param>
		public Search_Results_Statistics(DbDataReader Facet_Data, List<short> Facet_Types, List<string> Metadata_Labels)
        {
            // Create the facet lists
            Aggregation_Facets = new List<Search_Facet_Aggregation>();
            Facet_Collections = new List<Search_Facet_Collection>();

            // Save the titles
            Total_Titles = Total_Titles;
            Total_Items = Total_Items;
            All_Collections_Items = -1;
            All_Collections_Titles = -1;

			metadataLabels = Metadata_Labels;

            if (Facet_Types != null)
            {
                // Convert facet table to facet lists
                Convert_Facet_Tables_To_Facet_Lists(Facet_Data, Facet_Types);
            }
        }

        #region Basic properties

        /// <summary> Time, in millseconds, required for this query on the search engine </summary>
        [DataMember(Name = "queryTime")]
        [XmlAttribute("queryTime")]
        [ProtoMember(1)]
        public int QueryTime { get; set; }

        /// <summary> Total number of titles matching the search parameters </summary>
        [DataMember(Name = "titles")]
        [XmlAttribute("titles")]
        [ProtoMember(2)]
        public int Total_Titles { get; set; }

        /// <summary> Total number of items matching the search parameters </summary>
        [DataMember(Name = "items")]
        [XmlAttribute("items")]
        [ProtoMember(3)]
        public int Total_Items { get; set; }

        /// <summary> Number of titles matching the search parameters, if the search is expanded to the all collections </summary>
        [DataMember(Name = "expandedTitles")]
        [XmlIgnore]
        [ProtoMember(4)]
        public int? All_Collections_Titles { get; set; }

        /// <summary> Number of items matching the search parameters, if the search is expanded to the all collections </summary>
        [DataMember(Name = "expandedItems")]
        [XmlIgnore]
        [ProtoMember(5)]
        public int? All_Collections_Items { get; set; }

        /// <summary> Number of titles matching the search parameters, if the search is expanded to the all collections  </summary>
        /// <remarks> This is for the XML serialization portions </remarks>
        [IgnoreDataMember]
        [XmlAttribute("expandedTitles")]
        public string All_Collections_Titles_AsString
        {
            get { return All_Collections_Titles.HasValue ? All_Collections_Titles.ToString() : null; }
            set
            {
                int temp;
                if (Int32.TryParse(value, out temp))
                    All_Collections_Titles = temp;
            }
        }

        /// <summary> Number of items matching the search parameters, if the search is expanded to the all collections </summary>
        /// <remarks> This is for the XML serialization portions </remarks>
        [IgnoreDataMember]
        [XmlAttribute("expandedItems")]
        public string All_Collections_Items_AsString
        {
            get { return All_Collections_Items.HasValue ? All_Collections_Items.ToString() : null; }
            set
            {
                int temp;
                if (Int32.TryParse(value, out temp))
                    All_Collections_Items = temp;
            }
        }

		/// <summary> List of the metadata labels associated with each of the values
		/// found in the title results in the page of results </summary>
		/// <remarks> This allows each aggregation to customize which values are returned
		/// in searches and browses.  This is used to add the labels for each metadata value
		/// in the table and brief views. </remarks>
        [DataMember(Name = "metadataLabels")]
        [XmlArray("metadataLabels")]
        [XmlArrayItem("label", typeof(string))]
        [ProtoMember(6)]
	    public List<string> Metadata_Labels
	    {
		    get
		    {
			    return metadataLabels;
		    }
	    }

        #endregion

        #region Facet properties and methods

        /// <summary> Gets the flag that indicates if this result set has facet information  </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public bool Has_Facet_Info
        {
            get
            {
                if ((Facet_Collections == null) || (Facet_Collections.Count == 0))
                    return false;

                return true;
            }
        }

        /// <summary> Gets the number of aggregation facets associated with this results set </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public int Aggregation_Facets_Count
        {
            get {
                return Aggregation_Facets == null ? 0 : Aggregation_Facets.Count;
            }
        }

        /// <summary> Gets the collection of aggregation facets associated with this results set </summary>
        [DataMember(Name = "agggregationFacets")]
        [XmlArray("agggregationFacets")]
        [XmlArrayItem("aggregationFacet", typeof(Search_Facet_Aggregation))]
        [ProtoMember(7)]
        public List<Search_Facet_Aggregation> Aggregation_Facets { get; private set; }

        /// <summary> Collection of all the facets associated with this results set </summary>
        [DataMember(Name = "facetCollections")]
        [XmlArray("facetCollections")]
        [XmlArrayItem("facetCollection", typeof(Search_Facet_Collection))]
        [ProtoMember(8)]
        public List<Search_Facet_Collection> Facet_Collections { get; set; }

        private void Convert_Facet_Tables_To_Facet_Lists(DbDataReader Reader, List<short> Facet_Types)
        {
            // Go to the next table
            if (!Reader.NextResult())
                return;

            // Incrementor going through tables (and skipping aggregation table maybe)
            if (Reader.FieldCount > 2)
            {
                // Read all the aggregation fields
                while (Reader.Read())
                {
                    Aggregation_Facets.Add(new Search_Facet_Aggregation(Reader.GetString(1), Reader.GetInt32(2), Reader.GetString(0)));
                }
            }

            // Add all the other facets, reading each subsequent table in the results
            int current_facet_index = 0;
            while (Reader.NextResult())
            {
                // Build this facet list
                if ((Reader.FieldCount == 2) && (Facet_Types.Count > current_facet_index))
                {
                    // Create the collection and and assifn the metadata type id
                    Search_Facet_Collection thisCollection = new Search_Facet_Collection(Facet_Types[current_facet_index]);

                    // Read all the individual facet values
                    while (Reader.Read())
                    {
                        thisCollection.Facets.Add(new Search_Facet(Reader.GetString(0), Reader.GetInt32(1)));
                    }

                    // If there was an id and facets added, save this to the search statistics
                    if ((thisCollection.MetadataTypeID > 0) && (thisCollection.Facets.Count > 0))
                    {
                        Facet_Collections.Add(thisCollection);
                    }
                }

                current_facet_index++;
            }
        }

        private void Convert_Facet_Tables_To_Facet_Lists( DataSet Facet_Data, List<short> Facet_Types )
        {
            // Incrementor going through tables (and skipping aggregation table maybe)
            int table_counter = 2;
            if ((Facet_Data.Tables.Count > 2) && (Facet_Data.Tables[2].Columns.Count > 2))
            {
                foreach (DataRow thisRow in Facet_Data.Tables[2].Rows)
                {
                    Aggregation_Facets.Add(new Search_Facet_Aggregation(thisRow[1].ToString(), Convert.ToInt32(thisRow[2]), thisRow[0].ToString()));
                }
                table_counter++;
            }

            // Add all the other facets, reading each subsequent table in the results
            int facet_index = 0;
            while (Facet_Data.Tables.Count > table_counter)
            {
                // Build this facet list
                if ((Facet_Data.Tables[table_counter].Columns.Count == 2) && (Facet_Types.Count > facet_index))
                {
                    // Create the collection and and assifn the metadata type id
                    Search_Facet_Collection thisCollection = new Search_Facet_Collection(Facet_Types[facet_index]);

                    // Read all the individual facet values
                    foreach (DataRow thisRow in Facet_Data.Tables[table_counter].Rows)
                    {
                        thisCollection.Facets.Add(new Search_Facet(thisRow[0].ToString(), Convert.ToInt32(thisRow[1])));
                    }

                    // If there was an id and facets added, save this to the search statistics
                    if ((thisCollection.MetadataTypeID > 0) && (thisCollection.Facets.Count > 0))
                    {
                        Facet_Collections.Add(thisCollection);
                    }
                }

                table_counter++;
                facet_index++;
            }
        }

        #endregion
    }
}
