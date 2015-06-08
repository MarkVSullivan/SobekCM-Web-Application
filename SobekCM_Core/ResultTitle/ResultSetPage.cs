using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.ResultTitle
{
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("resultTitle")]
    public class ResultSetPage
    {
        /// <summary> Page number for this page of results </summary>
        [DataMember(EmitDefaultValue = false, Name = "page")]
        [XmlAttribute("page")]
        [ProtoMember(1)]
        public int Page { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "results")]
        [XmlArray("results")]
        [XmlArrayItem("title", typeof(ResultTitleInfo))]
        [ProtoMember(2)]
        public List<ResultTitleInfo> Results { get; set; }
    }
}
