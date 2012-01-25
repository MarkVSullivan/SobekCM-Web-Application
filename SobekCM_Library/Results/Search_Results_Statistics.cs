#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

#endregion

namespace SobekCM.Library.Results
{
    /// <summary> Contains all the top-level information about a set of search results, while not containing any of the actual pages of results </summary>
     [Serializable]
    public class Search_Results_Statistics
    {
        /// <summary> Constructor for a new instance of the Search_Results_Statistics class </summary>
        public Search_Results_Statistics()
        {
            // Set default facet types
            First_Facets_MetadataTypeID = -1;
            Second_Facets_MetadataTypeID = -1;
            Third_Facets_MetadataTypeID = -1;
            Fourth_Facets_MetadataTypeID = -1;
            Fifth_Facets_MetadataTypeID = -1;
            Sixth_Facets_MetadataTypeID = -1;
            Seventh_Facets_MetadataTypeID = -1;
            Eighth_Facets_MetadataTypeID = -1;

            // Create the facet lists
            Aggregation_Facets = new List<Search_Facet_Aggregation>();
            First_Facets = new List<Search_Facet>();
            Second_Facets = new List<Search_Facet>();
            Third_Facets = new List<Search_Facet>();
            Fourth_Facets = new List<Search_Facet>();
            Fifth_Facets = new List<Search_Facet>();
            Sixth_Facets = new List<Search_Facet>();
            Seventh_Facets = new List<Search_Facet>();
            Eighth_Facets = new List<Search_Facet>();

            Total_Items = -1;
            Total_Titles = -1;
            All_Collections_Items = -1;
            All_Collections_Titles = -1;
            QueryTime = 0;

        }

        /// <summary> Constructor for a new instance of the Search_Results_Statistics class </summary>
        /// <param name="Facet_Data"> DataSet containing the facets data to include within this class</param>
        /// <param name="Facet_Types"> Types of facets requested from the database for the related item aggreagtion </param>
        /// <param name="Total_Items"> Total number of items within the greater set of matching items/titles </param>
        /// <param name="Total_Titles"> Total number of titles within the greater set of matching items/titles</param>
        public Search_Results_Statistics(DataSet Facet_Data, List<short> Facet_Types, int Total_Items, int Total_Titles)
        {            
            // Set default facet types
            First_Facets_MetadataTypeID = -1;
            Second_Facets_MetadataTypeID = -1;
            Third_Facets_MetadataTypeID = -1;
            Fourth_Facets_MetadataTypeID = -1;
            Fifth_Facets_MetadataTypeID = -1;
            Sixth_Facets_MetadataTypeID = -1;
            Seventh_Facets_MetadataTypeID = -1;
            Eighth_Facets_MetadataTypeID = -1;

            // Create the facet lists
            Aggregation_Facets = new List<Search_Facet_Aggregation>();
            First_Facets = new List<Search_Facet>();
            Second_Facets = new List<Search_Facet>();
            Third_Facets = new List<Search_Facet>();
            Fourth_Facets = new List<Search_Facet>();
            Fifth_Facets = new List<Search_Facet>();
            Sixth_Facets = new List<Search_Facet>();
            Seventh_Facets = new List<Search_Facet>();
            Eighth_Facets = new List<Search_Facet>();

            // Save the titles
            this.Total_Titles = Total_Titles;
            this.Total_Items = Total_Items;
            All_Collections_Items = -1;
            All_Collections_Titles = -1;

            // Convert facet table to facet lists
            Convert_Facet_Tables_To_Facet_Lists(Facet_Data, Facet_Types);
        }

        /// <summary> Constructor for a new instance of the Search_Results_Statistics class </summary>
        /// <param name="Facet_Data"> DataReader containg all the facet information to include within this class </param>
        /// <param name="Facet_Types"> Types of facets requested from the database for the related item aggreagtion </param>
        public Search_Results_Statistics( SqlDataReader Facet_Data, List<short> Facet_Types)
        {
            // Set default facet types
            First_Facets_MetadataTypeID = -1;
            Second_Facets_MetadataTypeID = -1;
            Third_Facets_MetadataTypeID = -1;
            Fourth_Facets_MetadataTypeID = -1;
            Fifth_Facets_MetadataTypeID = -1;
            Sixth_Facets_MetadataTypeID = -1;
            Seventh_Facets_MetadataTypeID = -1;
            Eighth_Facets_MetadataTypeID = -1;

            // Create the facet lists
            Aggregation_Facets = new List<Search_Facet_Aggregation>();
            First_Facets = new List<Search_Facet>();
            Second_Facets = new List<Search_Facet>();
            Third_Facets = new List<Search_Facet>();
            Fourth_Facets = new List<Search_Facet>();
            Fifth_Facets = new List<Search_Facet>();
            Sixth_Facets = new List<Search_Facet>();
            Seventh_Facets = new List<Search_Facet>();
            Eighth_Facets = new List<Search_Facet>();

            // Save the titles
            Total_Titles = Total_Titles;
            Total_Items = Total_Items;
            All_Collections_Items = -1;
            All_Collections_Titles = -1;

            if (Facet_Types != null)
            {
                // Convert facet table to facet lists
                Convert_Facet_Tables_To_Facet_Lists(Facet_Data, Facet_Types);
            }
        }

        #region Basic properties

        /// <summary> Time, in millseconds, required for this query on the search engine </summary>
        public int QueryTime { get; internal set; }

        /// <summary> Total number of titles matching the search parameters </summary>
        public int Total_Titles { get; internal set; }

        /// <summary> Total number of items matching the search parameters </summary>
        public int Total_Items { get; internal set; }

        /// <summary> Number of titles matching the search parameters, if the search is expanded to the all collections </summary>
        public int All_Collections_Titles { get; internal set; }

        /// <summary> Number of items matching the search parameters, if the search is expanded to the all collections </summary>
        public int All_Collections_Items { get; internal set; }

        #endregion

        #region Facet properties and methods

        /// <summary> Gets the flag that indicates if this result set has facet information  </summary>
        public bool Has_Facet_Info
        {
            get
            {
                if (((First_Facets != null) && (First_Facets.Count > 0)) ||
                    ((Second_Facets != null) && (Second_Facets.Count > 0)) ||
                    ((Third_Facets != null) && (Third_Facets.Count > 0)) ||
                    ((Fourth_Facets != null) && (Fourth_Facets.Count > 0)) ||
                    ((Fifth_Facets != null) && (Fifth_Facets.Count > 0)) ||
                    ((Sixth_Facets != null) && (Sixth_Facets.Count > 0)) ||
                    ((Seventh_Facets != null) && (Seventh_Facets.Count > 0)) ||
                    ((Eighth_Facets != null) && (Eighth_Facets.Count > 0)))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary> Gets the number of aggregation facets associated with this results set </summary>
        public int Aggregation_Facets_Count
        {
            get {
                return Aggregation_Facets == null ? 0 : Aggregation_Facets.Count;
            }
        }

        /// <summary> Gets the collection of aggregation facets associated with this results set </summary>
        public List<Search_Facet_Aggregation> Aggregation_Facets { get; private set; }

        /// <summary> Gets the number of facets associated with the first facet list in this results set </summary>
        public int First_Facets_Count
        {
            get {
                return First_Facets == null ? 0 : First_Facets.Count;
            }
        }

        /// <summary> Gets the collection of facets associated with the first facet list in this results set </summary>
        public List<Search_Facet> First_Facets { get; private set; }

        /// <summary> Gets the metadata type id for the metadata represented by the first facet list in this results set </summary>
        public short First_Facets_MetadataTypeID { get; private set; }

        /// <summary> Gets the number of facets associated with the second facet list in this results set </summary>
        public int Second_Facets_Count
        {
            get {
                return Second_Facets == null ? 0 : Second_Facets.Count;
            }
        }

        /// <summary> Gets the collection of facets associated with the second facet list in this results set </summary>
        public List<Search_Facet> Second_Facets { get; private set; }

        /// <summary> Gets the metadata type id for the metadata represented by the second facet list in this results set </summary>
        public short Second_Facets_MetadataTypeID { get; private set; }

        /// <summary> Gets the number of facets associated with the third facet list in this results set </summary>
        public int Third_Facets_Count
        {
            get {
                return Third_Facets == null ? 0 : Third_Facets.Count;
            }
        }

        /// <summary> Gets the collection of facets associated with the third facet list in this results set </summary>
        public List<Search_Facet> Third_Facets { get; private set; }

        /// <summary> Gets the metadata type id for the metadata represented by the third facet list in this results set </summary>
        public short Third_Facets_MetadataTypeID { get; private set; }

        /// <summary> Gets the number of facets associated with the fourth facet list in this results set </summary>
        public int Fourth_Facets_Count
        {
            get {
                return Fourth_Facets == null ? 0 : Fourth_Facets.Count;
            }
        }

        /// <summary> Gets the collection of facets associated with the fourth facet list in this results set </summary>
        public List<Search_Facet> Fourth_Facets { get; private set; }

        /// <summary> Gets the metadata type id for the metadata represented by the fourth facet list in this results set </summary>
        public short Fourth_Facets_MetadataTypeID { get; private set; }

        /// <summary> Gets the number of facets associated with the fifth facet list in this results set </summary>
        public int Fifth_Facets_Count
        {
            get {
                return Fifth_Facets == null ? 0 : Fifth_Facets.Count;
            }
        }

        /// <summary> Gets the collection of facets associated with the fifth facet list in this results set </summary>
        public List<Search_Facet> Fifth_Facets { get; private set; }

        /// <summary> Gets the metadata type id for the metadata represented by the fifth facet list in this results set </summary>
        public short Fifth_Facets_MetadataTypeID { get; private set; }

        /// <summary> Gets the number of facets associated with the sixth facet list in this results set </summary>
        public int Sixth_Facets_Count
        {
            get {
                return Sixth_Facets == null ? 0 : Sixth_Facets.Count;
            }
        }

        /// <summary> Gets the collection of facets associated with the sixth facet list in this results set </summary>
        public List<Search_Facet> Sixth_Facets { get; private set; }

        /// <summary> Gets the metadata type id for the metadata represented by the sixth facet list in this results set </summary>
        public short Sixth_Facets_MetadataTypeID { get; private set; }

        /// <summary> Gets the number of facets associated with the seventh facet list in this results set </summary>
        public int Seventh_Facets_Count
        {
            get {
                return Seventh_Facets == null ? 0 : Seventh_Facets.Count;
            }
        }

        /// <summary> Gets the collection of facets associated with the seventh facet list in this results set </summary>
        public List<Search_Facet> Seventh_Facets { get; private set; }

        /// <summary> Gets the metadata type id for the metadata represented by the seventh facet list in this results set </summary>
        public short Seventh_Facets_MetadataTypeID { get; private set; }

        /// <summary> Gets the number of facets associated with the eighth facet list in this results set </summary>
        public int Eighth_Facets_Count
        {
            get {
                return Eighth_Facets == null ? 0 : Eighth_Facets.Count;
            }
        }

        /// <summary> Gets the collection of facets associated with the eighth facet list in this results set </summary>
        public List<Search_Facet> Eighth_Facets { get; private set; }

        /// <summary> Gets the metadata type id for the metadata represented by the eighth facet list in this results set </summary>
        public short Eighth_Facets_MetadataTypeID { get; private set; }

        private void Convert_Facet_Tables_To_Facet_Lists(SqlDataReader reader, List<short> Facet_Types)
        {
            // Go to the next table
            if (!reader.NextResult())
                return;


            // Incrementor going through tables (and skipping aggregation table maybe)
            if (reader.FieldCount > 2)
            {
                // Read all the aggregation fields
                while (reader.Read())
                {
                    Aggregation_Facets.Add(new Search_Facet_Aggregation(reader.GetString(1), reader.GetInt32(2), reader.GetString(0)));
                }

                // Go to the next table
                if (!reader.NextResult())
                    return;
            }

            // Build the first facet list
            if ((reader.FieldCount == 2) && (Facet_Types.Count > 0))
            {
                // Assign the first facet type
                First_Facets_MetadataTypeID = Facet_Types[0];

                // Read all the individual facet values
                while (reader.Read())
                {
                    First_Facets.Add(new Search_Facet(reader.GetString(0), reader.GetInt32(1)));
                }

                // Go to the next table
                if (!reader.NextResult())
                    return;
            }

            // Build the second facet list
            if ((reader.FieldCount == 2) && (Facet_Types.Count > 1))
            {
                // Assign the second facet type
                Second_Facets_MetadataTypeID = Facet_Types[1];

                // Read all the individual facet values
                while (reader.Read())
                {
                    Second_Facets.Add(new Search_Facet(reader.GetString(0), reader.GetInt32(1)));
                }

                // Go to the next table
                if (!reader.NextResult())
                    return;
            }

            // Build the third facet list
            if ((reader.FieldCount == 2) && (Facet_Types.Count > 2))
            {
                // Assign the third facet type
                Third_Facets_MetadataTypeID = Facet_Types[2];

                // Read all the individual facet values
                while (reader.Read())
                {
                    Third_Facets.Add(new Search_Facet(reader.GetString(0), reader.GetInt32(1)));
                }

                // Go to the next table
                if (!reader.NextResult())
                    return;
            }

            // Build the fourth facet list
            if ((reader.FieldCount == 2) && (Facet_Types.Count > 3))
            {
                // Assign the fourth facet type
                Fourth_Facets_MetadataTypeID = Facet_Types[3];

                // Read all the individual facet values
                while (reader.Read())
                {
                    Fourth_Facets.Add(new Search_Facet(reader.GetString(0), reader.GetInt32(1)));
                }

                // Go to the next table
                if (!reader.NextResult())
                    return;
            }

            // Build the fifth facet list
            if ((reader.FieldCount == 2) && (Facet_Types.Count > 4))
            {
                // Assign the fifth facet type
                Fifth_Facets_MetadataTypeID = Facet_Types[4];

                // Read all the individual facet values
                while (reader.Read())
                {
                    Fifth_Facets.Add(new Search_Facet(reader.GetString(0), reader.GetInt32(1)));
                }

                // Go to the next table
                if (!reader.NextResult())
                    return;
            }

            // Build the sixth facet list
            if ((reader.FieldCount == 2) && (Facet_Types.Count > 5))
            {
                // Assign the sixth facet type
                Sixth_Facets_MetadataTypeID = Facet_Types[5];

                // Read all the individual facet values
                while (reader.Read())
                {
                    Sixth_Facets.Add(new Search_Facet(reader.GetString(0), reader.GetInt32(1)));
                }

                // Go to the next table
                if (!reader.NextResult())
                    return;
            }

            // Build the seventh facet list
            if ((reader.FieldCount == 2) && (Facet_Types.Count > 6))
            {
                // Assign the seventh facet type
                Seventh_Facets_MetadataTypeID = Facet_Types[6];

                // Read all the individual facet values
                while (reader.Read())
                {
                    Seventh_Facets.Add(new Search_Facet(reader.GetString(0), reader.GetInt32(1)));
                }

                // Go to the next table
                if (!reader.NextResult())
                    return;
            }

            // Build the eighth facet list
            if ((reader.FieldCount == 2) && (Facet_Types.Count > 7))
            {
                // Assign the eighth facet type
                Eighth_Facets_MetadataTypeID = Facet_Types[7];

                // Read all the individual facet values
                while (reader.Read())
                {
                    Eighth_Facets.Add(new Search_Facet(reader.GetString(0), reader.GetInt32(1)));
                }
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

            // Build the first facet list
            if ((Facet_Data.Tables.Count > table_counter) && (Facet_Data.Tables[table_counter].Columns.Count == 2) && ( Facet_Types.Count > 0 ))
            {
                First_Facets_MetadataTypeID = Facet_Types[0];
                foreach (DataRow thisRow in Facet_Data.Tables[table_counter].Rows)
                {
                    First_Facets.Add(new Search_Facet(thisRow[0].ToString(), Convert.ToInt32(thisRow[1])));
                }

                // Build the second facet list
                table_counter++;
                if ((Facet_Data.Tables.Count > table_counter) && (Facet_Data.Tables[table_counter].Columns.Count == 2) && (Facet_Types.Count > 1))
                {
                    Second_Facets_MetadataTypeID = Facet_Types[1];
                    foreach (DataRow thisRow in Facet_Data.Tables[table_counter].Rows)
                    {
                        Second_Facets.Add(new Search_Facet(thisRow[0].ToString(), Convert.ToInt32(thisRow[1])));
                    }

                    // Build the third facet list
                    table_counter++;
                    if ((Facet_Data.Tables.Count > table_counter) && (Facet_Data.Tables[table_counter].Columns.Count == 2) && (Facet_Types.Count > 2))
                    {
                        Third_Facets_MetadataTypeID = Facet_Types[2];
                        foreach (DataRow thisRow in Facet_Data.Tables[table_counter].Rows)
                        {
                            Third_Facets.Add(new Search_Facet(thisRow[0].ToString(), Convert.ToInt32(thisRow[1])));
                        }

                        // Build the fourth facet list
                        table_counter++;
                        if ((Facet_Data.Tables.Count > table_counter) && (Facet_Data.Tables[table_counter].Columns.Count == 2) && (Facet_Types.Count > 3))
                        {
                            Fourth_Facets_MetadataTypeID = Facet_Types[3];
                            foreach (DataRow thisRow in Facet_Data.Tables[table_counter].Rows)
                            {
                                Fourth_Facets.Add(new Search_Facet(thisRow[0].ToString(), Convert.ToInt32(thisRow[1])));
                            }

                            // Build the fifth facet list
                            table_counter++;
                            if ((Facet_Data.Tables.Count > table_counter) && (Facet_Data.Tables[table_counter].Columns.Count == 2) && (Facet_Types.Count > 4))
                            {
                                Fifth_Facets_MetadataTypeID = Facet_Types[4];
                                foreach (DataRow thisRow in Facet_Data.Tables[table_counter].Rows)
                                {
                                    Fifth_Facets.Add(new Search_Facet(thisRow[0].ToString(), Convert.ToInt32(thisRow[1])));
                                }

                                // Build the sixth facet list
                                table_counter++;
                                if ((Facet_Data.Tables.Count > table_counter) && (Facet_Data.Tables[table_counter].Columns.Count == 2) && (Facet_Types.Count > 5))
                                {
                                    Sixth_Facets_MetadataTypeID = Facet_Types[5];
                                    foreach (DataRow thisRow in Facet_Data.Tables[table_counter].Rows)
                                    {
                                        Sixth_Facets.Add(new Search_Facet(thisRow[0].ToString(), Convert.ToInt32(thisRow[1])));
                                    }

                                    // Build the seventh facet list
                                    table_counter++;
                                    if ((Facet_Data.Tables.Count > table_counter) && (Facet_Data.Tables[table_counter].Columns.Count == 2) && (Facet_Types.Count > 6))
                                    {
                                        Seventh_Facets_MetadataTypeID = Facet_Types[6];
                                        foreach (DataRow thisRow in Facet_Data.Tables[table_counter].Rows)
                                        {
                                            Seventh_Facets.Add(new Search_Facet(thisRow[0].ToString(), Convert.ToInt32(thisRow[1])));
                                        }

                                        // Build the eighth facet list
                                        table_counter++;
                                        if ((Facet_Data.Tables.Count > table_counter) && (Facet_Data.Tables[table_counter].Columns.Count == 2) && (Facet_Types.Count > 7))
                                        {
                                            Eighth_Facets_MetadataTypeID = Facet_Types[5];
                                            foreach (DataRow thisRow in Facet_Data.Tables[table_counter].Rows)
                                            {
                                                Eighth_Facets.Add(new Search_Facet(thisRow[0].ToString(), Convert.ToInt32(thisRow[1])));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
