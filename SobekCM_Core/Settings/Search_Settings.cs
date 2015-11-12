using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Settings
{
    /// <summary> Settings related to the behavior of the aggregation-wide searches within the system </summary>
    [Serializable, DataContract, ProtoContract]
    public class Search_Settings
    {
        /// <summary> Constructor for a new instance of the Search_Settings class </summary>
        public Search_Settings()
        {
            Facets_Collapsible = false;
            Can_Remove_Single_Term = true;
        }

        /// <summary> Flag indicates if users can remove a single part of their search term </summary>
        [DataMember(Name = "canRemoveSingleTerm")]
        [XmlElement("canRemoveSingleTerm")]
        [ProtoMember(1)]
        public bool Can_Remove_Single_Term { get; set; }

        /// <summary> Flag indicates if facets start out collapsed </summary>
        [DataMember(Name = "facetsCollapsible")]
        [XmlElement("facetsCollapsible")]
        [ProtoMember(2)]
        public bool Facets_Collapsible { get; set; }


        /// <summary> Flag indicates whether the facets should be pulled during a browse </summary>
        [DataMember(Name = "pullFacetsOnBrowse")]
        [XmlElement("pullFacetsOnBrowse")]
        [ProtoMember(3)]
        public bool Pull_Facets_On_Browse { get; set; }

        /// <summary> Flag indicates whether the facets should be pulled during a search </summary>
        [DataMember(Name = "pullFacetsOnSearch")]
        [XmlElement("pullFacetsOnSearch")]
        [ProtoMember(4)]
        public bool Pull_Facets_On_Search { get; set; }
    }
}
