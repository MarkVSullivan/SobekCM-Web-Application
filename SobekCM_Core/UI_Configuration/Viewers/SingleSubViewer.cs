using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.UI_Configuration.Viewers
{
    /// <summary> Configuration for a single subviewer, mapping from the 
    /// signifying URL segment to the subviewer class to utilize </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("SingleSubViewer")]
    public class SingleSubViewer
    {
        /// <summary> Viewer code that is mapped to this subviewer </summary>
        [DataMember(Name = "code")]
        [XmlAttribute("code")]
        [ProtoMember(1)]
        public string ViewerCode { get; set; }

        /// <summary> Flag indicates if this subviewer is enabled or disabled </summary>
        [DataMember(Name = "enabled")]
        [XmlAttribute("enabled")]
        [ProtoMember(2)]
        public bool Enabled { get; set; }

        /// <summary> Fully qualified (including namespace) name of the class used 
        /// for this subviewer </summary>
        [DataMember(Name = "class")]
        [XmlAttribute("class")]
        [ProtoMember(3)]
        public string Class { get; set; }

        /// <summary> Name of the assembly within which this class resides, unless this
        /// is one of the default subviewers included in the core code </summary>
        [DataMember(Name = "assembly", EmitDefaultValue = false)]
        [XmlAttribute("assembly")]
        [ProtoMember(4)]
        public string Assembly { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="SingleSubViewer"/> class </summary>
        public SingleSubViewer()
        {
            // Empty constructor for serialzation purposes
        }

        /// <summary> Constructor for a new instance of the <see cref="SingleSubViewer"/> class </summary>
        /// <param name="ViewerCode"> Viewer code that is mapped to this subviewer </param>
        /// <param name="Enabled"> Flag indicates if this subviewer is enabled or disabled </param>
        /// <param name="Class"> Fully qualified (including namespace) name of the class used 
        /// for this subviewer </param>
        /// <param name="Assembly"> Name of the assembly within which this class resides, unless this
        /// is one of the default subviewers included in the core code </param>
        public SingleSubViewer(string ViewerCode, bool Enabled, string Class, string Assembly)
        {
            this.ViewerCode = ViewerCode;
            this.Enabled = Enabled;
            this.Class = Class;
            this.Assembly = Assembly;
        }

        #region Methods that controls XML serialization

        /// <summary> Method suppresses XML Serialization of the Assembly property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeAssembly()
        {
            return (!String.IsNullOrEmpty(Assembly));
        }

        #endregion
    }
}
