using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Aggregations
{
    /// <summary> Class holds all the aggregation hierarchy information </summary>
    [Serializable, DataContract, ProtoContract]
    public class Aggregation_Hierarchy
    {
        /// <summary> Constructor for a new instance of the Aggregation_Hierarchy class </summary>
        public Aggregation_Hierarchy()
        {
            Collections = new List<Item_Aggregation_Related_Aggregations>();
            Institutions = new List<Item_Aggregation_Related_Aggregations>();
        }

        /// <summary> List of collections, complete with children nodes </summary>
        [DataMember(Name = "collections"), ProtoMember(1)]
        public List<Item_Aggregation_Related_Aggregations> Collections { get; set; }

        /// <summary> List of institutions (not linked to a collection), complete with children nodes </summary>
        [DataMember(Name = "institutions"), ProtoMember(2)]
        public List<Item_Aggregation_Related_Aggregations> Institutions { get; set; }
    }
}
