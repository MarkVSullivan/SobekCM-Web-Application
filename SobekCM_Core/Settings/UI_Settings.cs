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
    /// <summary> System-wide settings related to UI elements, such as themes for helper libraries, etc.. </summary>
    [Serializable, DataContract, ProtoContract]
    public class UI_Settings
    {
        /// <summary> Constructor for a new instance of the UI_Settings class </summary>
        public UI_Settings()
        {
            Ace_Editor_Theme = "chrome";
        }

        /// <summary> Theme for the Ace Editor, used for CSS and javascript editing, as well as TEI editing if that 
        /// plug-in is enabled </summary>
        [DataMember(Name = "aceEditorTheme")]
        [XmlElement("aceEditorTheme")]
        [ProtoMember(1)]
        public string Ace_Editor_Theme { get; set; }
    }
}
