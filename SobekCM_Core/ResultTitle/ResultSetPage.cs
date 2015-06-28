#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.ResultTitle
{
    /// <summary> Wrapper object contains all the information about a single page of results </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("resultTitle")]
    public class ResultSetPage
    {
        /// <summary> Page number for this page of results </summary>
        [DataMember(EmitDefaultValue = false, Name = "page")]
        [XmlAttribute("page")]
        [ProtoMember(1)]
        public int Page { get; set; }

        /// <summary> Collection of results for this page of results within the larger set </summary>
        [DataMember(EmitDefaultValue = false, Name = "results")]
        [XmlArray("results")]
        [XmlArrayItem("title", typeof(ResultTitleInfo))]
        [ProtoMember(2)]
        public List<ResultTitleInfo> Results { get; set; }
    }
}
