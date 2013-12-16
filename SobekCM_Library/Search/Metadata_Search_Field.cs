namespace SobekCM.Library.Search
{
    /// <summary> Contains all the relevant and linking information about a single metadata search field
    /// including information for the web application URLs, searching in the database, and searching within
    /// a Solr/Lucene index </summary>
    public class Metadata_Search_Field
    {
        /// <summary> Term used for displaying this metadata field in searches and results </summary>
        public readonly string Display_Term;

        /// <summary> Term used for this metadata field when displaying facets </summary>
        public readonly string Facet_Term;

        /// <summary> Primary identifier for this metadata search field </summary>
        public readonly short ID;

        /// <summary> Field name for this search field in the Solr search indexes </summary>
        public readonly string Solr_Field;

        /// <summary> Code used within the web application for searches against this field (particularly in the URLs) </summary>
        public readonly string Web_Code;

	    /// <summary> Constructor for a new instance of the Metadata_Search_Field class </summary>
	    /// <param name="ID">Primary identifier for this metadata search field</param>
	    /// <param name="Facet_Term">Term used for this metadata field when displaying facets</param>
	    /// <param name="Display_Term">Term used for displaying this metadata field in searches and results</param>
	    /// <param name="Web_Code">Code used within the web application for searches against this field (particularly in the URLs)</param>
	    /// <param name="Solr_Field">Field name for this search field in the Solr search indexes</param>
	    public Metadata_Search_Field(short ID, string Facet_Term, string Display_Term, string Web_Code, string Solr_Field )
        {
            this.ID = ID;
            this.Facet_Term = Facet_Term;
            this.Display_Term = Display_Term;
            this.Web_Code = Web_Code;
            this.Solr_Field = Solr_Field;
        }
    }
}
