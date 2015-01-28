using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Aggregations
{
    /// <summary> Complete information about a single metadata type, related to an item aggregation </summary>
    [Serializable, DataContract, ProtoContract]
    public class Complete_Item_Aggregation_Metadata_Type : IEquatable<Complete_Item_Aggregation_Metadata_Type>
    {
        /// <summary> Constructor for a new instance of the Complete_Item_Aggregation_Metadata_Type class </summary>
        public Complete_Item_Aggregation_Metadata_Type() { }

        /// <summary> Constructor for a new instance of the Complete_Item_Aggregation_Metadata_Type class </summary>
        /// <param name="ID"> Primary key for this metadata type, from the database</param>
        /// <param name="DisplayTerm"> Display term for this metadata type </param>
        /// <param name="SobekCode"> Code related to this metadata type, used for searching for example </param>
        public Complete_Item_Aggregation_Metadata_Type(short ID, string DisplayTerm, string SobekCode)
        {
            this.ID = ID;
            this.DisplayTerm = DisplayTerm;
            this.SobekCode = SobekCode;
        }

        /// <summary> Primary key for this metadata type, from the database </summary>
        [DataMember(Name = "id"), ProtoMember(1)]
        public short ID { get; set; }

        /// <summary> Display term for this metadata type </summary>
        [DataMember(Name = "term"), ProtoMember(2)]
        public string DisplayTerm { get; set; }

        /// <summary> Code related to this metadata type, used for searching for example </summary>
        [DataMember(Name = "code"), ProtoMember(3)]
        public string SobekCode { get; set; }

        /// <summary> Term used when performing a search against Solr/Lucene </summary>
        [DataMember(Name = "solr"), ProtoMember(4)]
        public string SolrCode { get; set; }

        /// <summary> Checks to see if this metadata type is equal to another, by using the database primary key </summary>
        /// <param name="other"> Other metadata type to compare to </param>
        /// <returns> TRUE if equal, otherwise FALSE </returns>
        public bool Equals(Complete_Item_Aggregation_Metadata_Type other)
        {
            return (other.ID == ID);
        }

        /// <summary> Returns the hashcode for this metata type </summary>
        /// <returns> Hascode, based on the primary key </returns>
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}
