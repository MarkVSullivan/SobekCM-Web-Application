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
    /// <summary> Administrative setting information, including display information and the 
    /// current setting value and key </summary>
    [Serializable, DataContract, ProtoContract]
    public class Admin_Setting_Value
    {
        /// <summary> Name / key for this database setting </summary>
        [DataMember(Name = "key")]
        [XmlAttribute("key")]
        [ProtoMember(1)]
        public string Key { get; set; }

        /// <summary> Current value for this setting </summary>
        [DataMember(Name = "value", EmitDefaultValue = false)]
        [XmlElement("value")]
        [ProtoMember(2)]
        public string Value { get; set; }

        /// <summary> Label for the tab page under which this should appear </summary>
        [DataMember(Name = "tabPage", EmitDefaultValue = false)]
        [XmlAttribute("tabPage")]
        [ProtoMember(3)]
        public string TabPage { get; set; }

        /// <summary> Heading under which this setting should appear within the tab page </summary>
        [DataMember(Name = "heading", EmitDefaultValue = false)]
        [XmlAttribute("heading")]
        [ProtoMember(4)]
        public string Heading { get; set; }

        /// <summary> Gets or sets a value indicating whether this <see cref="Admin_Setting_Value"/> is hidden </summary>
        /// <value> TRUE if hidden; otherwise, FALSE </value>
        [DataMember(Name = "hidden")]
        [XmlAttribute("hidden")]
        [ProtoMember(5)]
        public bool Hidden { get; set; }

        /// <summary> Flag indicates if this field should be reserved to the host or system administrators </summary>
        /// <value> 0 = Not reserved, 1 = Non SuperAdmin can view, 2 = Non SuperAdmin should not see, 3 = Only SuperAdmin can view and they can't change it </value>
        [DataMember(Name = "reserved")]
        [XmlAttribute("reserved")]
        [ProtoMember(6)]
        public short Reserved { get; set; }

        /// <summary> Help text for this metadata element from the database </summary>
        [DataMember(Name = "help", EmitDefaultValue = false)]
        [XmlElement("help")]
        [ProtoMember(7)]
        public string Help { get; set; }

        /// <summary> If this setting has a small set of options, all of the possible settings </summary>
        [DataMember(Name = "options", EmitDefaultValue = false)]
        [XmlArray("options")]
        [XmlArrayItem("option", typeof(string))]
        [ProtoMember(8)]
        public List<string> Options { get; set; }

        /// <summary> Unique key to this setting </summary>
        [DataMember(Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(9)]
        public short SettingID { get; set; }

        /// <summary> Width of the admin entry box to be displayed for this setting </summary>
        [DataMember(Name = "width", EmitDefaultValue = false)]
        [XmlIgnore]
        [ProtoMember(10)]
        public short? Width { get; set; }

        /// <summary> Width of the admin entry box to be displayed, as a string for XML serialization </summary>
        /// <remarks> This is used for XML serialization primarily.</remarks>
        [IgnoreDataMember]
        [XmlAttribute("width")]
        public string Width_AsString
        {
            get { return Width.HasValue ? Width.ToString() : null; }
            set
            {
                short temp;
                if (Int16.TryParse(value, out temp))
                    Width = temp;
            }
        }

        /// <summary> Height of the admin entry box to be displayed for this setting </summary>
        /// <remarks> Any value over one means this will be a text area input </remarks>
        [DataMember(Name = "height", EmitDefaultValue = false)]
        [XmlIgnore]
        [ProtoMember(11)]
        public short? Height { get; set; }

        /// <summary> Height of the admin entry box to be displayed, as a string for XML serialization </summary>
        /// <remarks> This is used for XML serialization primarily.</remarks>
        [IgnoreDataMember]
        [XmlAttribute("height")]
        public string Height_AsString
        {
            get { return Height.HasValue ? Height.ToString() : null; }
            set
            {
                short temp;
                if (Int16.TryParse(value, out temp))
                    Height = temp;
            }
        }

        /// <summary> Add a new option to this setting value </summary>
        /// <param name="NewOption"> New option to add </param>
        public void Add_Option(string NewOption)
        {
            if ( Options == null ) Options = new List<string>();
            Options.Add(NewOption);
        }
       
    }
}
