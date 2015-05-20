using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Information about a single file within a digital resource </summary>
    [Serializable, DataContract, ProtoContract]
    public class BriefItem_File
    {
        /// <summary> Name for this file </summary>
        /// <remarks> If this is not in the resource folder, this may include a URL </remarks>
        [DataMember(Name = "name")]
        [XmlText]
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary> Width of the file (in pixels), if relevant and available </summary>
        /// <remarks> This could be the actual width, or the preferred viewing width </remarks>
        [DataMember(EmitDefaultValue = false, Name = "width")]
        [XmlIgnore]
        [ProtoMember(2)]
        public int? Width { get; set; }

        /// <summary> Width of the file (in pixels), if relevant and available </summary>
        /// <remarks> This is used for XML serialization primarily. &lt;br /&gt;
        /// This could be the actual width, or the preferred viewing width </remarks>
        [IgnoreDataMember]
        [XmlAttribute("width")]
        public string Width_AsString
        {
            get
            {
                if (Width.HasValue)
                    return Width.ToString();
                else
                    return null;
            }
            set
            {
                int temp;
                if (Int32.TryParse(value, out temp))
                    Width = temp;
            }
        }

        /// <summary> Height of the file (in pixels), if relevant and available </summary>
        /// <remarks> This could be the actual height, or the preferred viewing height </remarks>
        [DataMember(EmitDefaultValue = false, Name = "height")]
        [XmlIgnore]
        [ProtoMember(3)]
        public int? Height { get; set; }

        /// <summary> Height of the file (in pixels), if relevant and available </summary>
        /// <remarks> This is used for XML serialization primarily. &lt;br /&gt;
        /// This could be the actual height, or the preferred viewing height </remarks>
        [IgnoreDataMember]
        [XmlAttribute("height")]
        public string Height_AsString
        {
            get
            {
                if (Height.HasValue)
                    return Height.ToString();
                else
                    return null;
            }
            set
            {
                int temp;
                if (Int32.TryParse(value, out temp))
                    Height = temp;
            }
        }

        /// <summary> Other attributes associated with this file, that may be needed for display purposes </summary>
        [DataMember(EmitDefaultValue = false, Name = "attributes")]
        [XmlAttribute("attributes")]
        [ProtoMember(4)]
        public string Attributes { get; set; }
        
        /// <summary> Constructor for a new instance of the BriefItem_File class </summary>
        public BriefItem_File()
        {
            // Does nothing - needed for deserialization
        }

        /// <summary> Constructor for a new instance of the BriefItem_File class </summary>
        /// <param name="Name"> Name for this file </param>
        public BriefItem_File( string Name )
        {
            this.Name = Name;
        }
    }
}
