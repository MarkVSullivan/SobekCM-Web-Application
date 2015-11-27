#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Search
{
    /// <summary> Contains all the relevant and linking information about a single metadata search field
    /// including information for the web application URLs, searching in the database, and searching within
    /// a Solr/Lucene index </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("MetadataSearchField")]
    public class Metadata_Search_Field
    {
        /// <summary> Term used for displaying this metadata field in searches and results </summary>
        [DataMember(Name = "display", EmitDefaultValue = false)]
        [XmlAttribute("display")]
        [ProtoMember(1)]
        public string Display_Term { get; set; }

        /// <summary> Term used for this metadata field when displaying facets </summary>
        [DataMember(Name = "facet", EmitDefaultValue = false)]
        [XmlAttribute("facet")]
        [ProtoMember(2)]
        public string Facet_Term { get; set; }

        /// <summary> Primary identifier for this metadata search field </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        [XmlAttribute("id")]
        [ProtoMember(3)]
        public short ID { get; set; }

        /// <summary> Field name for this search field in the Solr search indexes </summary>
        [DataMember(Name = "solr", EmitDefaultValue = false)]
        [XmlAttribute("solr")]
        [ProtoMember(4)]
        public string Solr_Field { get; set; }

        /// <summary> Code used within the web application for searches against this field (particularly in the URLs) </summary>
        [DataMember(Name = "code", EmitDefaultValue = false)]
        [XmlAttribute("code")]
        [ProtoMember(5)]
        public string Web_Code { get; set; }

        /// <summary> Name of this metadata search field (remains fairly constant, and links back to database name) </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        [XmlAttribute("name")]
        [ProtoMember(6)]
        public string Name { get; set; }

        /// <summary> Constructor for a new instance of the Metadata_Search_Field class </summary>
        /// <remarks> Empty constructor for serialization purposes </remarks>
        public Metadata_Search_Field()
        {
            // Empty constructor for serialization purposes
        }

        /// <summary> Constructor for a new instance of the Metadata_Search_Field class </summary>
        /// <param name="ID">Primary identifier for this metadata search field</param>
        /// <param name="Facet_Term">Term used for this metadata field when displaying facets</param>
        /// <param name="Display_Term">Term used for displaying this metadata field in searches and results</param>
        /// <param name="Web_Code">Code used within the web application for searches against this field (particularly in the URLs)</param>
        /// <param name="Solr_Field">Field name for this search field in the Solr search indexes</param>
        /// <param name="Name"> Name of this metadata search field (remains fairly constant, and links back to database name)</param>
        public Metadata_Search_Field(short ID, string Facet_Term, string Display_Term, string Web_Code, string Solr_Field, string Name )
        {
            this.ID = ID;
            this.Facet_Term = Facet_Term;
            this.Display_Term = Display_Term;
            this.Web_Code = Web_Code;
            this.Solr_Field = Solr_Field;
            this.Name = Name;
        }
    }
}
