using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Settings
{
    /// <summary> Collection of all the administrative settings, which includes display information
    /// to accompany the current keys and values </summary>
    [Serializable, DataContract, ProtoContract]
    public class Admin_Setting_Collection
    {
        /// <summary> Constructor for a new instance of the <see cref="Admin_Setting_Collection"/> class </summary>
        public Admin_Setting_Collection()
        {
            Settings = new List<Admin_Setting_Value>();
        }

        /// <summary> Collection of settings </summary>
        [DataMember(Name = "settings")]
        [XmlArray("settings")]
        [XmlArrayItem("setting", typeof(Admin_Setting_Value))]
        [ProtoMember(1)]
        public List<Admin_Setting_Value> Settings { get; set; } 
    }
}
