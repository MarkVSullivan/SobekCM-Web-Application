using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Aggregations
{
    /// <summary> Arguments necessary for adding a new item aggregation to the system </summary>
    [Serializable, DataContract, ProtoContract]
    public class New_Aggregation_Arguments
    {
        /// <summary> Constructor for a new instance of the New_Aggregation_Arguments class </summary>
        public New_Aggregation_Arguments()
        {
            Active = true;
            Hidden = false;
        }

        /// <summary> Code for this item aggregation object </summary>
        [DataMember(Name = "code")]
        [ProtoMember(1)]
        public string Code { get; set; }

        /// <summary> Description of this item aggregation (in the default language ) </summary>
        /// <remarks> This field is displayed on the main home pages if this is part of a thematic collection </remarks>
        [DataMember(Name = "description")]
        [ProtoMember(2)]
        public string Description { get; set; }

        /// <summary> Flag indicating this item aggregation is active </summary>
        [DataMember(Name = "isActive")]
        [ProtoMember(3)]
        public bool Active { get; set; }

        /// <summary> Flag indicating this item aggregation is hidden from most displays </summary>
        /// <remarks> If this item aggregation is active, public users can still be directed to this item aggreagtion, but it will
        ///   not appear in the lists of subaggregations anywhere. </remarks>
        [DataMember(Name = "isHidden")]
        [ProtoMember(4)]
        public bool Hidden { get; set; }

        /// <summary> External link, generally only used if this is an institutional type aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "externalLink")]
        [ProtoMember(5)]
        public string External_Link { get; set; }

        /// <summary> Full name of this item aggregation </summary>
        [DataMember(Name = "name")]
        [ProtoMember(6)]
        public string Name { get; set; }

        /// <summary> Short name of this item aggregation </summary>
        /// <remarks> This is an alternate name used for breadcrumbs, etc.. </remarks>
        [DataMember(EmitDefaultValue = false, Name = "shortName")]
        [ProtoMember(7)]
        public string ShortName { get; set; }

        /// <summary> Code for the aggregation selected as the parent </summary>
        [DataMember(Name = "parent")]
        [ProtoMember(8)]
        public string ParentCode { get; set; }

        /// <summary> Type of item aggregation object </summary>
        [DataMember(Name = "type")]
        [ProtoMember(9)]
        public string Type { get; set; }

        /// <summary> Thematic heading for this new aggregation to be linked to </summary>
        [DataMember(EmitDefaultValue = false, Name = "thematicHeading")]
        [ProtoMember(10)]
        public string Thematic_Heading { get; set; }

        /// <summary> Name of the user that is creating this aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "user")]
        [ProtoMember(11)]
        public string User { get; set; }

    }
}
