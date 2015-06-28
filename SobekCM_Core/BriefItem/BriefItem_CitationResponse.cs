#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.BriefItem
{
    /// <summary> Citation-only brief item, primarily used for serialization </summary>
    /// <remarks> This primarily acts as a wrapper around a BriefItemInfo class object. </remarks>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("item")]
    public class BriefItem_CitationResponse
    {
        private BriefItemInfo briefItem;

        /// <summary> Constructor for a new instance of the BriefItem_CitationXmlResponse class </summary>
        public BriefItem_CitationResponse()
        {
            briefItem = new BriefItemInfo();
        }

        /// <summary> Constructor for a new instance of the BriefItem_CitationXmlResponse class </summary>
        /// <param name="fullItemInfo"> Full information object to be wrapped by this class </param>
        public BriefItem_CitationResponse(BriefItemInfo fullItemInfo)
        {
            briefItem = fullItemInfo;
        }

        /// <summary> Get the BriefItemInfo class wrapped by this class </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public BriefItemInfo BriefItem
        {
            get { return briefItem;  }
        }

        /// <summary> Bibliographic identifier for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "bibid")]
        [XmlAttribute("bibid")]
        [ProtoMember(1)]
        public string BibID
        {
            get { return briefItem.BibID;  }
            set { briefItem.BibID = value; }
        }

        /// <summary> Volume identifier for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "vid")]
        [XmlAttribute("vid")]
        [ProtoMember(2)]
        public string VID 
        {
            get { return briefItem.VID;  }
            set { briefItem.VID = value; }
        }

        /// <summary> Title for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "title")]
        [XmlElement("title")]
        [ProtoMember(3)]
        public string Title
        {
            get { return briefItem.Title;  }
            set { briefItem.Title = value; }
        }

        /// <summary> Namespace definition used within the brief item (generally within the citation)  </summary>
        [DataMember(EmitDefaultValue = false, Name = "namespaces")]
        [XmlArray("namespaces")]
        [XmlArrayItem("namespace", typeof(BriefItem_Namespace))]
        [ProtoMember(4)]
        public List<BriefItem_Namespace> Namespaces
        {
            get { return briefItem.Namespaces; }
            set { briefItem.Namespaces = value; }
        }

        /// <summary> Description/Citation elements for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "description")]
        [XmlArray("description")]
        [XmlArrayItem("descriptiveTerm", typeof(BriefItem_DescriptiveTerm))]
        public List<BriefItem_DescriptiveTerm> Description
        {
            get { return briefItem.Description; }
            set { briefItem.Description = value; }
        }
    }
}
