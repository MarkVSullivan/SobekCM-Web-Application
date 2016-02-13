#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.EAD
{
    /// <summary> Class stores information about the containers which hold a component object from this EAD.</summary>
    /// <remarks> This is an abstraction which replaced the strict box, folder, item structure.  Those types of containers are still the most commonly encountered though.<br /><br />
    /// This class contains information about a single level of the container hierarchy, and is generally used within a list of different types (i.e., one for the box information, one for the folder information, etc.. )</remarks>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("eadParentInfo")]
    public class EAD_Transfer_Parent_Container_Info
    {
        /// <summary> Title or label for this container </summary>
        [DataMember(EmitDefaultValue = false, Name = "title")]
        [XmlAttribute("title")]
        [ProtoMember(1)]
        public string Container_Title { get; set; }

        /// <summary> General type of this container ( usually 'box', 'folder', etc.. ) </summary>
        [DataMember(EmitDefaultValue = false, Name = "type")]
        [XmlAttribute("type")]
        [ProtoMember(2)]
        public string Container_Type { get; set; }

        /// <summary> Constructor for a new instance of the Parent_Container_Info class </summary>
        /// <param name="Container_Type"> General type of this container ( usually 'box', 'folder', etc.. )</param>
        /// <param name="Container_Title"> Title or label for this container</param>
        public EAD_Transfer_Parent_Container_Info(string Container_Type, string Container_Title)
        {
            this.Container_Title = Container_Title;
            this.Container_Type = Container_Type;
        }

        /// <summary> Constructor for a new instance of the Parent_Container_Info class </summary>
        /// <remarks> Empty constructor for serialization and deserialization purposes </remarks>
        public EAD_Transfer_Parent_Container_Info()
        {
            // Empty constructor for serialization and deserialization purposes
        }
    }
}